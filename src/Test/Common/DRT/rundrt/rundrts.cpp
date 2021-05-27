//+----------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  Abstract:
//      Harness for running drts.
//
//  History:
//      2005/09/19 
//
//-----------------------------------------------------------------------------

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <assert.h>

#include <windows.h>
#include <tchar.h>
#include <psapi.h>

#include <shlobj.h>

#define SPI_GETMENUSHOWDELAY       0x006A
#define SPI_SETMENUSHOWDELAY       0x006B

#define STRSAFE_NO_DEPRECATE
#include <strsafe.h>

#define MAX_DRTS 2048
#define MIN(a, b) ((a) < (b) ? (a) : (b))

// Globals

// default filenames
char g_szDrtFilename[MAX_PATH] = "rundrtlist.txt";
char g_szOutFilename[MAX_PATH] = "rundrtslog.xml";

char g_szSlacker[MAX_PATH] = "NoOwner";

#define DOWNLEVEL_RGBRAST_DLL   "RGB9Rast_2.dll"
#define VISTA_RGBRAST_DLL       "RGB9Rast.dll"
char g_szRGBRastModulePath[MAX_PATH] = "";  // Initialized by InitializeRGBRastGlobals

// helper command lines
char g_szInstallSEETestDll[] = "installseedrtutil.cmd /i" ; 
char g_szUninstallSEETestDll[] = "installseedrtutil.cmd /u" ; 
char g_szHostingSetup[MAX_PATH] = "HostingSetup.cmd";
char g_szHostingCleanup[MAX_PATH] = "HostingCleanup.cmd";
char g_szInstallWisptis[] = "RegisterTabletRef.cmd /idrt";
char g_szUninstallWisptis[] = "RegisterTabletRef.cmd /udrt";
char g_szCheckWisptisConfig[] = "RegisterTabletRef.cmd /checkdrt";

char g_szCheckPrintConfig[] = "RegisterPrintRef.cmd /checkdrt";

char g_pszSystemDirectory[MAX_PATH] = "";  // Initialized by PrivateOnVista

// operating systems
#define OS_XP      0x01
#define OS_VISTA   0X02
#define OS_ALL     OS_XP | OS_VISTA 

// architecture and flavor 
#define ARCH_NONE    0x00
#define ARCH_X86    0x01
#define ARCH_IA64    0x02
#define ARCH_AMD64    0x04
#define ARCH_ARM    0x08
#define ARCH_ALL    ARCH_X86 | ARCH_IA64 | ARCH_AMD64 | ARCH_ARM

// Current operating system - we currently only differentiate between Vista and all others
UINT g_uCurrentOS = OS_XP;

// NOTE: alternatively, could use GetSystemInfo at runtime to determine CPU architecture
#if defined(_X86_)
UINT g_uArchToRun = ARCH_X86;
#elif defined(_IA64_)
UINT g_uArchToRun = ARCH_IA64;
#elif defined(_AMD64_)
UINT g_uArchToRun = ARCH_AMD64;
#elif defined(_ARM_)
UINT g_uArchToRun = ARCH_ARM;
#endif


// drt structure
typedef struct {
    char* pszDrtName;    // command line of the drt
    char* pszTeam;        // team that owns it
    char* pszOwner;        // alias of person who owns it
    char* pszArgs;        // command line args (optional)
    UINT uArch;            // architectures to run for
    UINT uOS;             // operating systems to run on
    int  iResult;        // 0 = not run, +1 = success, -1 = reported failure, -2 = timeout failure
    double ftElapsed;		// seconds elapsed to complete the DRT
} DRT_ENTRY;

DRT_ENTRY * g_rpDrtList[MAX_DRTS];

// defines for DRT_ENTRY.iResult
#define DRT_NOT_RUN     0
#define DRT_SUCCESS  1
#define DRT_REPORTED_FAILURE -1
#define DRT_TIMEOUT_FAILURE -2

// alternate list of drts, if not using file (e.g. -Validate:  or -Drt: flags)
char *g_rgszDrt[MAX_DRTS];
bool g_fUseDrtListFile = true;


// tracking variables
bool g_fSetConfigCalled = false;
UINT g_cDrt = 0;
UINT g_rgcSuccess[MAX_DRTS] = {0};
UINT g_rgcFailure[MAX_DRTS] = {0};
UINT g_cSuccess = 0;
UINT g_cFailure = 0;
double g_rgTime[MAX_DRTS][3]= {0};
PROCESS_MEMORY_COUNTERS g_rgMemory[MAX_DRTS] = {0};
double g_dTimeTotal = 0.0;
UINT g_cTotalDrtsRun = 0; // this will be g_cDrt * g_cRuns below
UINT g_cTotalRunSoFar = 0;
bool g_fPassed = false;        // final value of whether this run passed or failed
UINT g_cOldMenuShowDelay = 0;
UINT g_cOldForegroundLockTimeout = 0;

// controlling flags
UINT g_cRuns = 1;
bool g_fBreakOnFailure = false;
bool g_fSkipConfig = false;
DWORD g_dwTimeout = INFINITE;
bool g_fValidate = false;
bool g_fDetails = false;
bool g_fVerbose = false;
bool g_fInfo = false;
bool g_NotRunCountsAsPassed = true;
bool g_fPrivateOnVista = false;

// number of runs for a -Validate:  pass
UINT g_uNumValidateRuns = 100;

enum ActionEnum { actionSet, actionCheck, actionRevert };

// function prototypes
bool SetConfig();
bool RevertConfig();
bool CheckConfig();
void GetOSVersion();
bool MoveProductBinaries(ActionEnum action);

bool DoPrivateOnVista(ActionEnum actionEnum);

// list of product binaries that need to be moved away from local directory
// to ensure only installed binaries get loaded by DRT apps.
// Loading local assemblies in verification runs can produce fake results
// and also cause outright failures on cinch when bits are PRS signed.
// 
char* g_szProductBinaries[] =
{
    { "milcore.dll" },
    { "WindowsCodecs.dll" },
    { "WindowsCodecsExt.dll" },
    { "wmphoto.dll" },
    { "photometadatahandler.dll" },
    NULL    // required terminator
};

char* g_HideProdBins = "HideProdBins";

/*++

Routine Description:

    FileTimeToSeconds

    Converts a FILETIME to seconds
--*/
double
FileTimeToSeconds(FILETIME *pft)
{
    double seconds = 0.0;
    SYSTEMTIME systime;
    FileTimeToSystemTime(pft, &systime);

    seconds = ((((systime.wHour * 60.0) + systime.wMinute) *60.0) +
               systime.wSecond + (systime.wMilliseconds / 1000.0));
    
    return seconds;
        
}

/*++

Routine Description:

    PrintUsage

    Prints the message on how to use this program
--*/
void PrintUsage()
{
    printf("\nRunDrts command line options:\n"
        "  (either - or / will work)\n\n"
        "  [-ExitOnFailure] - exit on first failing drt\n"
        "  [-Verbose] - Verbose output\n"
        "  [-Outfile filename] - output to the specified file (def=%s)\n"
        "  [-RunList filename] - alternate list of drts to run (def=%s)\n"
        "  [-Drt drtname [drtname ...]] - drts to run, overrides runlist\n"
        "  [-Loop nnnn] - number of times to run the drts (def=1)\n"
        "  [-Timeout nnnn] - max. number of ms to give each drt to run\n"
        "  [-SkipConfig] - just run drts, don't setup or restore config\n"
        "  [-SetupShell] - make setup changes and launch DRT shell.  Other options are ignored.\n"
        "  [-CheckConfig] - just check the config, don't run drts\n"
        "  [-Validate drtname] - run validation check on the specified drt\n"
        "  [-PrivateOnVista] - setup testing of a private build on Vista\n"
        "  [-Info] - turn on verbose info messages for this tool\n\n"
        "  [-?] or [-Help] - this message\n\n",
        g_szOutFilename, g_szDrtFilename);
    exit(1);
}


/*++

Routine Description

    Returns true if command will launch an script (bat or cmd)

-- */
bool
DoesCommandLaunchScriptOrShellCmd ( PCSTR pszCommandLine )
{
    bool fScript = false;
    UINT uEndOfApplication = 0;

    fScript = ( (_strnicmp(pszCommandLine, "mklink ", 7) == 0) ||
        (_strnicmp(pszCommandLine, "rmdir ", 6) == 0) );

    if (!fScript)
    {
        // search for first 
        while (   pszCommandLine[uEndOfApplication]
               && !isspace(pszCommandLine[uEndOfApplication]))
        {
            uEndOfApplication++;
        }
        

        if (uEndOfApplication > 4)
        {
            PCSTR pszExtension = &pszCommandLine[uEndOfApplication-4];

            fScript = (   (_strnicmp(pszExtension, ".bat", 4) == 0)
                       || (_strnicmp(pszExtension, ".cmd", 4) == 0));
        }
    }

    return fScript;
}

/*++

Routine Description:

    Helper function to launch a command line.  Used primarily for config stuff.

--*/
bool
RunCommandLine ( PCSTR pszCmd )
{
    bool fSuccess = false;
    DWORD dwStatus = 0;
    PROCESS_INFORMATION procinfo;
    STARTUPINFOA si;
    HRESULT hr;
    char szCommandLine[MAX_PATH];

    ZeroMemory( &si, sizeof(si) );
    si.cb = sizeof(si);
    ZeroMemory( &procinfo, sizeof(procinfo) );

    if (DoesCommandLaunchScriptOrShellCmd(pszCmd))
    {
        hr = StringCbPrintfA(szCommandLine, sizeof(szCommandLine),
                             "cmd.exe /c \"%s & exit /b\"", pszCmd);
    }
    else
    {
        hr = StringCbCopyA(szCommandLine, sizeof(szCommandLine),
                           pszCmd);
    }

    // Make sure any pending output is sent before child process takes over
    fflush(stdout);

    if (SUCCEEDED(hr) &&
        CreateProcessA(NULL,
                      szCommandLine,
                      NULL,
                      NULL,
                      FALSE,
                      NULL,
                      NULL,
                      NULL,
                      &si,
                      &procinfo))
    {
        dwStatus = WaitForSingleObject(procinfo.hProcess, INFINITE);

        if (dwStatus == WAIT_TIMEOUT)
        {
            TerminateProcess(procinfo.hProcess, 0xFFFFFFFF);
        }
        else
        {
            GetExitCodeProcess(procinfo.hProcess, &dwStatus);            
        }

        if (dwStatus)
        {
            printf("FAILED COMMAND: %s\n", pszCmd);
        }
        else
        {
            fSuccess = true;
        }

        CloseHandle(procinfo.hThread);
        CloseHandle(procinfo.hProcess);
    }

    return fSuccess;
}

/*++

Routine Description:

    ParseArgs
    
    Parse the command line arguments
--*/
void ParseArgs(int argc, char *argv[])
{
    for (int i = 0; i < argc; i++)
    {
        if (argv[i][0] == '/' || argv[i][0] == '-')
        {
            char* pszArg = &argv[i][1];
            if (_stricmp(pszArg, "ExitOnFailure" ) == 0 )    // ExitOnFailure
            {
                g_fBreakOnFailure = TRUE;
            }
            else if ( _stricmp(pszArg, "Verbose") == 0 )    // Verbose
            {
                g_fVerbose = TRUE;
            }
            else if ( _stricmp(pszArg, "Info") == 0 )   // Info
            {
                g_fInfo = TRUE;
            }
            else if ( _stricmp(pszArg, "Validate:" ) == 0 || _stricmp(pszArg, "Validate" ) == 0)    // Validate
            {
                i++;
                g_cRuns = g_uNumValidateRuns;
                // get filename of drt to validate
                if ( i < argc )
                {
                    if (argv[i][0] != '/' && argv[i][0] != '-')
                    {
                        // override the drt file name
                        g_fValidate = true;
                        g_fUseDrtListFile = false;
                        char* pszArg2 = &argv[i][0];
                        size_t len = MIN(strlen(pszArg2)+1, MAX_PATH);
                        g_rgszDrt[g_cDrt] = (char*)malloc(len);
                        if ( g_rgszDrt[g_cDrt] )
                        {
                            strcpy_s(g_rgszDrt[g_cDrt], len, pszArg2);
                            g_cDrt++;
                        }
                    }
                }
                if ( !g_fValidate )
                {
                    // if false, they didn't give us a filename, the rotten scoundrels...
                    printf("must specify a drt name after -Validate:\n");
                    PrintUsage();
                }
            }
            else if ( _stricmp(pszArg, "SkipConfig") == 0 )        // SkipConfig
            {
                g_fSkipConfig = TRUE;
            }
            else if ( _stricmp(pszArg, "SetupShell") == 0 ||  _stricmp(pszArg, "Setup") == 0)    // SetupShell
            {
                if (SetConfig())
                {
                    printf("RunDrts:  configuration setup\n");
                    RunCommandLine( "cmd.exe /K \"echo Type exit when done.&PROMPT $P$_DRT Shell$G\"" );
                    RevertConfig();
                    exit(0);
                }
                printf("RunDrts:  FAILED configuration setup\n");
                exit(1);
            }
            else if ( _stricmp(pszArg, "CheckConfig") == 0 )    // CheckConfig
            {
                // don't need output here, CheckConfig will provide it
                exit(CheckConfig() ? 0 : 1);
            }
            else if ( _stricmp(pszArg, "PrivateOnVista") == 0 )    // PrivateOnVista
            {
                g_fPrivateOnVista = true;
            }
            else if ( _stricmp(pszArg, "Loop:") == 0 || _stricmp(pszArg, "Loop") == 0 )    // Loop
            {
                i++;
                // get # to loop
                if ( i < argc )
                {
                    if (argv[i][0] != '/' && argv[i][0] != '-')
                    {
                        // 
                        g_cRuns = atoi(&argv[i][0]);
                        if ( g_cRuns <= 0 )
                        {
                            printf("must specify non-zero, non-negative, number of loops after -Loop:\n");
                            PrintUsage();
                        }
                    }
                    else
                    {
                        printf("must specify number of loops after -Loop:\n");
                        PrintUsage();
                    }
                }
            }
            else if ( _stricmp(pszArg, "Timeout:") == 0 || _stricmp(pszArg, "Timeout") == 0 )    // Timeout
            {
                i++;
                // get timeout value
                if ( i < argc )
                {
                    if (argv[i][0] != '/' && argv[i][0] != '-')
                    {
                        // 
                        char* pszArg2 = &argv[i][0];
                        if ( _stricmp ( pszArg2, "infinite" ) == 0 )
                        {
                            g_dwTimeout = INFINITE;
                        }
                        else
                        {
                            g_dwTimeout = atoi(pszArg2);
                            if ( g_dwTimeout == 0 )
                            {
                                printf("must specify non-zero timeout value\n");
                                PrintUsage();
                            }
                        }
                    }
                    else
                    {
                        printf("must specify timeout value after -Timeout:\n");
                        PrintUsage();
                    }
                }
            }
            else if ( _stricmp(pszArg, "?") == 0 || _stricmp(pszArg, "Help") == 0 )    // Help, ?
            {
                PrintUsage();
            }
            else if ( _stricmp(pszArg, "outfile:" ) == 0 || _stricmp(pszArg, "outfile" ) == 0)    // outfile
            {
                i++;
                // get filename
                if ( i < argc )
                {
                    if (argv[i][0] != '/' && argv[i][0] != '-')
                    {
                        // override the logfile name
                        char* pszArg2 = &argv[i][0];
                        size_t len = MIN(strlen(pszArg2)+1, sizeof(g_szOutFilename));
                        strcpy_s(g_szOutFilename, len, pszArg2);
                    }
                    else
                    {
                        printf("must specify an output file name after -Outfile:\n");
                        PrintUsage();
                    }
                }
            }
            else if ( _stricmp(pszArg, "RunList:" ) == 0 || _stricmp(pszArg, "RunList" ) == 0)    // Runlist
            {
                i++;
                // get filename
                if ( i < argc )
                {
                    if (argv[i][0] != '/' && argv[i][0] != '-')
                    {
                        // override the drt file name
                        char* pszArg2 = &argv[i][0];
                        size_t len = MIN(strlen(pszArg2)+1, sizeof(g_szDrtFilename));
                        strcpy_s(g_szDrtFilename, len, pszArg2);
                    }
                    else
                    {
                        printf("must specify a file name after -RunList:\n");
                        PrintUsage();
                    }
                }
            }
            else if (_stricmp(pszArg, "Drt:" ) == 0 || _stricmp(pszArg, "Drt" ) == 0 )    // Drt
            {
                g_fUseDrtListFile = false;
                i++;
                // get drtname
                while ( i < argc )
                {
                    if (argv[i][0] != '/' && argv[i][0] != '-')
                    {
                        // override the drt file, don't use it
                        g_fUseDrtListFile = false;
                        char* pszArg2 = &argv[i][0];
                        size_t len = MIN(strlen(pszArg2)+1, MAX_PATH);
                        g_rgszDrt[g_cDrt] = (char*)malloc(len);
                        if ( g_rgszDrt[g_cDrt] )
                        {
                            strcpy_s(g_rgszDrt[g_cDrt], len, pszArg2);
                            g_cDrt++;
                        }
                        i++;
                    }
                    else
                    {
                        // we're done with drts.  back up the arg counter and break out of the loop 
                        // (we back up the counter because we don't want to process an arg with 
                        // - or / here, but rather let the outer for loop do it).
                        i--;
                        break;
                    }
                }
            }
            else
            {
                printf("rundrts: unknown option %s\n", pszArg);
                // Usage prints a message and exits
                PrintUsage();
            }
        }
        else
        {
            printf("rundrts: unknown option %s\n", *argv);
            // Usage prints a message and exits
            PrintUsage();
        }
    }

}

/*++

Routine Description:

    LoadDrtsFromFile

    Reads a list of Drts to run from the specified file

 format:  
    name,archList,owner,team[,args]

 name - filename of the drt.  e.g. DrtFooTest.exe
 archList - architectures to run the drt for. 
   archList : arch [ | arch ]
   Possible values for arch
       all     - all architectures
       x86     - run for x86
       ia64    - run for IA64
       amd64   - run for amd64
 owner - alias of the SDE that owns the DRT
 team - name of the Avalon FT that owns the drt
 args - optional command line to send to the drt

--*/
bool 
LoadDrtsFromFile()
{
    UINT count = g_cDrt;
    BOOL fError = FALSE;
    FILE *pf = fopen(g_szDrtFilename, "r");
    if (pf)
    {
        char szLine[MAX_PATH];
        char szOrgLine[MAX_PATH];
        char szArchList[MAX_PATH];
        char szOSList[MAX_PATH];
        while ((count < MAX_DRTS) && fgets(szLine, MAX_PATH, pf))
        {
            size_t len = strlen(szLine);
            if (len > 0 && szLine[0] != '#')        // skip comments #
            {
                // maybe it's a keeper, load the drt
                DRT_ENTRY *pDrt = (DRT_ENTRY*)malloc(sizeof(DRT_ENTRY));
                if (pDrt)
                {
                    ZeroMemory(pDrt, sizeof(DRT_ENTRY));
                    int iParam = 0;
                    strcpy_s(szOrgLine, sizeof(szOrgLine), szLine);	// preserve original line for error messages
                    char* pszToken = strtok(szLine, ",\r\n");
                    while (pszToken != NULL)
                    {
                        size_t len = 0;
                        switch (iParam)
                        {
                        case 0:
                            // drtname
                            len = strlen(pszToken)+1;
                            pDrt->pszDrtName = (char *)malloc(len);
                            if ( pDrt->pszDrtName )
                                strcpy_s(pDrt->pszDrtName, len, pszToken);
                            break;

                        case 1:
                            // arch: all, x86|ia64|amd64
                            // expect a list of CPU architectures:
                            // since strtok isn't reentrant, remember cpuList to ---- apart later
                            strcpy_s(szArchList, sizeof(szArchList), pszToken);
                            break;

                        case 2:
                            // arch: all, xp | vista 
                            // expect a list of operation systems:
                            // since strtok isn't reentrant, remember cpuList to ---- apart later
                            strcpy_s(szOSList, sizeof(szOSList), pszToken);
                            break;

                        case 3:
                            // owner
                            len = strlen(pszToken)+1;
                            pDrt->pszOwner = (char *)malloc(len);
                            if (pDrt->pszOwner)
                                strcpy_s(pDrt->pszOwner, len, pszToken);
                            break;

                        case 4:
                            // team
                            len = strlen(pszToken)+1;
                            pDrt->pszTeam = (char *)malloc(len);
                            if (pDrt->pszTeam)
                                strcpy_s(pDrt->pszTeam, len, pszToken);
                            break;

                        case 5:
                            // command line
                            len = strlen(pszToken)+1;
                            pDrt->pszArgs = (char *)malloc(len);
                            if (pDrt->pszArgs)
                                strcpy_s(pDrt->pszArgs, len, pszToken);
                            break;

                        default:
                            // unknown
                            break;
                        }

                        iParam++;
                        pszToken = strtok(NULL, ",\r\n");
                    }


                    // now ---- apart list of CPUs
                    char* pszCPU = strtok(szArchList, "|" );
                    while (pszCPU != NULL)
                    {
                        if (_stricmp(pszCPU, "all") == 0)
                        {
                            pDrt->uArch = ARCH_ALL;
                            break;
                        }
                        else if (_stricmp(pszCPU, "x86") == 0)
                        {
                            pDrt->uArch |= ARCH_X86;
                        } 
                        else if (_stricmp(pszCPU, "ia64") == 0)
                        {
                            pDrt->uArch |= ARCH_IA64;
                        } 
                        else if (_stricmp(pszCPU, "amd64") == 0)
                        {
                            pDrt->uArch |= ARCH_AMD64;
                        }
                        else if (_stricmp(pszCPU, "arm") == 0)
                        {
                            pDrt->uArch |= ARCH_ARM;
                        }
                        else
                        {
                            printf("ERROR: Unknown CPU architecture '%s'in line\n%s\n", pszCPU, szOrgLine);
                            fError = TRUE;
                        }
                        pszCPU = strtok( NULL, "|" );
                    }

                    // parse list of operating systems
                    char* pszOS = strtok(szOSList, "|" );
                    while (pszOS != NULL)
                    {
                        if (_stricmp(pszOS, "all") == 0)
                        {
                            pDrt->uOS = OS_ALL;
                            break;
                        }
                        else if (_stricmp(pszOS, "xp") == 0)
                        {
                            pDrt->uOS |= OS_XP;
                        } 
                        else if (_stricmp(pszOS, "vista") == 0)
                        {
                            pDrt->uOS |= OS_VISTA;
                        } 
                        else
                        {
                            printf("ERROR: Unknown operating system '%s'in line\n%s\n", pszOS, szOrgLine);
                            fError = TRUE;
                        }
                        pszOS = strtok( NULL, "|" );
                    }


                    // validate needed fields
                    if (pDrt->pszOwner == NULL)
                    {
                        // set owner name to NoOwner
                        size_t len = strlen(g_szSlacker)+1;
                        pDrt->pszOwner = (char *)malloc(len);
                        if (pDrt->pszOwner)
                            strcpy_s(pDrt->pszOwner, len, g_szSlacker);

                    }
                    if (pDrt->pszDrtName == NULL)
                    {
                        // it doesn't have a name!!
                        printf("ERROR: Missing DRT name \n%s\n", szOrgLine);
                        fError = TRUE;
                    }

                    if (fError)
                    {
                        // delete the whole thing, 
                        free(pDrt);
                        break;
                    }
                    else
                    {
                        g_rpDrtList[count] = pDrt;
                        g_cDrt = ++count;
                    }
                }
            }
        }

        fclose(pf);
    }
    else
    {
        printf("ERROR: cannot open drt file '%s'\n", g_szDrtFilename);
        fError = TRUE;
    }
    return (!fError);
}

/*++

Routine Description:

    LoadDrtsFromArray

    Reads a list of Drts to run from the memory array.
    Used for such flags as -Drt and -Validate
--*/
void
LoadDrtsFromArray()
{
    for (UINT count = 0; count < g_cDrt; count++ ) 
    {
        g_rpDrtList[count] = (DRT_ENTRY*)malloc(sizeof(DRT_ENTRY));
        if ( g_rpDrtList[count] )
        {
            ZeroMemory(g_rpDrtList[count], sizeof(DRT_ENTRY) );
            // set drtname
            size_t len = strlen(g_rgszDrt[count])+1;
            g_rpDrtList[count]->pszDrtName = (char *)malloc(len);
            if ( g_rpDrtList[count]->pszDrtName )
                strcpy_s(g_rpDrtList[count]->pszDrtName, len, g_rgszDrt[count]);
            free (g_rgszDrt[count]);
            g_rgszDrt[count] = NULL;

            // assume they want to run for this arch, whatever it is (otherwise they wouldn't have flagged it)
            g_rpDrtList[count]->uArch = g_uArchToRun;

            // assume they want to run on this operating system, whatever it is (otherwise they wouldn't have flagged it)
            g_rpDrtList[count]->uOS = g_uCurrentOS;

            // we don't know the owner, but we need something for the output log
            len = strlen(g_szSlacker)+1;
            g_rpDrtList[count]->pszOwner = (char *)malloc(len);
            if ( g_rpDrtList[count]->pszOwner )
                strcpy_s(g_rpDrtList[count]->pszOwner, len, g_szSlacker);

            // again, don't know the team, but we should put something here just in case we use it 
            // for output (Saves us having to special case the log file generation).
            len = strlen(g_szSlacker)+1;
            g_rpDrtList[count]->pszTeam = (char *)malloc(len);
            if ( g_rpDrtList[count]->pszTeam )
                strcpy_s(g_rpDrtList[count]->pszTeam, len, g_szSlacker);

        }
    }
}

/*++

Routine Description:

    WriteLogFile

    writes the result of the run to the log file
--*/
void
WriteLogFile()
{
    UINT u;
    FILE *pf = fopen(g_szOutFilename, "w");
    if (pf)
    {
        // write header
        fprintf(pf, "<DrtRun status=\"%s\" TotalTime=\"%8.2lf\">\r\n", 
            g_fPassed ? "Passed" : "Failed", g_dTimeTotal);

        if (g_cFailure)
        {
            // there were failures
            fprintf(pf, "\t<Failures Total=\"%i\" of=\"%i\">\r\n", g_cFailure, g_cTotalDrtsRun);
            for (u = 0; u < g_cDrt; u++)
            {
                if (g_rpDrtList[u]->iResult == DRT_REPORTED_FAILURE )
                {
                    // see if there is a log file
                    char szTestLogFile[MAX_PATH];
                    strcpy_s(szTestLogFile, sizeof(szTestLogFile), g_rpDrtList[u]->pszDrtName);
                    if ( strlen(szTestLogFile) < MAX_PATH - 3 )
                        strcat_s(szTestLogFile, sizeof(szTestLogFile), ".log");
                    FILE *pfTestLog = fopen(szTestLogFile, "r");
                    if ( !pfTestLog)
                    {
                        // strip the original extension from the drtname and try again.
                        strcpy_s(szTestLogFile, sizeof(szTestLogFile), g_rpDrtList[u]->pszDrtName);
                        size_t iPos = strlen(szTestLogFile);
                        while ( iPos > 0 )
                        {
                            if ( szTestLogFile[iPos] == '.' )
                            {
                                szTestLogFile[iPos] = '\0';
                                if ( strlen(szTestLogFile) < MAX_PATH - 3 )
                                    strcat_s( szTestLogFile, sizeof(szTestLogFile), ".log");

                                pfTestLog = fopen(szTestLogFile, "r");
                                break;
                            }
                            --iPos;
                        }
                    }

                    if (pfTestLog)
                    {
                        fprintf(pf, "\t\t<Drt name=\"%s\" owner=\"%s\" status=\"Failed\" duration=\"%.3lf\">\n\t\t\t<![CDATA[\n", 
                            g_rpDrtList[u]->pszDrtName, g_rpDrtList[u]->pszOwner, g_rpDrtList[u]->ftElapsed);
                        char szLogLine[MAX_PATH];
                        while ( fgets(szLogLine, MAX_PATH, pfTestLog))
                        {
                            fputs(szLogLine, pf);
                        }
                        fputs("\n\t\t\t]]>\n\t\t</Drt>\n", pf);
                        fclose ( pfTestLog );
                    }
                    else
                    {
                        fprintf(pf, "\t\t<Drt name=\"%s\" owner=\"%s\" status=\"Failed\" duration=\"%.3lf\"/>\n", 
                            g_rpDrtList[u]->pszDrtName, g_rpDrtList[u]->pszOwner, g_rpDrtList[u]->ftElapsed);
                    }
                }
                else if (g_rpDrtList[u]->iResult == DRT_TIMEOUT_FAILURE )
                {
                    fprintf(pf, "\t\t<Timeout drt=\"%s\" owner=\"%s\" status=\"Timeout\"/>\n", 
                        g_rpDrtList[u]->pszDrtName, g_rpDrtList[u]->pszOwner );
                }

            }

            fputs("\t</Failures>\r\n", pf);
        }

        if ( g_cSuccess + g_cFailure < g_cTotalDrtsRun )
        {
            fprintf(pf, "\t<NotRun Total=\"%i\" of=\"%i\" >\r\n", 
                g_cTotalDrtsRun - (g_cSuccess + g_cFailure), g_cTotalDrtsRun );
            for (u = 0; u < g_cDrt; u++)

                if ( g_rpDrtList[u]->iResult == DRT_NOT_RUN )
                {
                    fprintf(pf, "\t\t<Drt name=\"%s\" owner=\"%s\" status=\"DidNotRun\"/>\r\n", 
                        g_rpDrtList[u]->pszDrtName, g_rpDrtList[u]->pszOwner );
                }
            fputs("\t</NotRun>\r\n", pf);
        }


        fprintf(pf, "\t<Passed Total=\"%i\" of=\"%i\" >\r\n", g_cSuccess, g_cTotalDrtsRun);
        for (u = 0; u < g_cDrt; u++)
        {
            if (g_rpDrtList[u]->iResult == DRT_SUCCESS )
            {
                fprintf(pf, "\t\t<Drt name=\"%s\" owner=\"%s\" status=\"Passed\" duration=\"%.3lf\"/>\r\n", 
                    g_rpDrtList[u]->pszDrtName, g_rpDrtList[u]->pszOwner, g_rpDrtList[u]->ftElapsed);
            }
        }
        fputs("\t</Passed>\r\n", pf);

        // close header
        fputs("</DrtRun>", pf);
        fclose(pf);
    }
}

/*++

Routine Description:

    PrintSummary
    
    Prints summary of the run to stdout
--*/
void
PrintSummary()
{
    printf("RunDrts:*************** SUMMARY *******************\n\n");
    printf("RunDrts: Total Time      %8.2lf seconds. \n", g_dTimeTotal);
    printf("RunDrts: Drts Run:       %d\n", (g_cSuccess+g_cFailure));
    printf("RunDrts: Drts Succeeded: %d\n", g_cSuccess);
    printf("RunDrts: Drts Failed:    %d\n", g_cFailure);

    if (g_cFailure)
    {
        printf("RunDrts: The following DRTs failed at least once\n");
        for (UINT u = 0; u < g_cDrt; u++)
        {
            if (g_rgcFailure[u])
            {
                printf("\t  %-40s\n", g_rpDrtList[u]->pszDrtName);
            }
        }
    }

    if (g_fDetails)
    {
        printf("\nRunDrts: Per drt details. Memory sizes in 4 KB pages\n");
        printf("\t%-40s %7s %7s %8s %8s %7s %7s %7s %7s %7s %7s %7s\n",
               "Drt Name", "Succeed","Fail","Tot Time","Ave Time","KM Time","UM Time", "Faults", "Peak WS", "PPool","NPPool","PFile");
        for (UINT u = 0; u < g_cDrt; u++)
        {
            if (g_rgcSuccess[u] || g_rgcFailure[u])
            {
                UINT cRuns = (g_rgcSuccess[u] + g_rgcFailure[u]);
                printf("\t%-40s %7d %7d %8.3lf %8.3lf %7.3lf %7.3lf ",
                       g_rpDrtList[u]->pszDrtName, 
                       g_rgcSuccess[u], 
                       g_rgcFailure[u], 
                       g_rgTime[u][0],
                       g_rgTime[u][0]/cRuns,
                       g_rgTime[u][1]/cRuns,
                       g_rgTime[u][2]/cRuns);
            
                printf("%7d %7d %7d %7d %7d\n",
                       g_rgMemory[u].PageFaultCount / cRuns,
                       static_cast<int>(g_rgMemory[u].PeakWorkingSetSize /0x1000 /cRuns),
                       static_cast<int>(g_rgMemory[u].QuotaPeakPagedPoolUsage / 0x1000 / cRuns),
                       static_cast<int>(g_rgMemory[u].QuotaPeakNonPagedPoolUsage / 0x1000 / cRuns),
                       static_cast<int>(g_rgMemory[u].PeakPagefileUsage / 0x1000 / cRuns)); 
            }
        }
    }
                       
}

/*++

Routine Description

    Deletes any existing log file for the specified drt

-- */
void 
DeleteDrtLogFile ( PCSTR pszDrtName )
{
    char szDrtTestLog[MAX_PATH];

    strcpy_s(szDrtTestLog, sizeof(szDrtTestLog), pszDrtName);
    if ( strlen(szDrtTestLog) < MAX_PATH - 4 )
    {
        strcat_s(szDrtTestLog, sizeof(szDrtTestLog), ".log");
        DeleteFileA ( szDrtTestLog );
    }

    // next, trip the original extension from the drtname and try again.
    strcpy_s(szDrtTestLog, sizeof(szDrtTestLog), pszDrtName);
    size_t iPos = strlen(szDrtTestLog);
    while ( iPos > 0 )
    {
        if ( szDrtTestLog[iPos] == '.' )
        {
            szDrtTestLog[iPos] = '\0';
            if ( strlen(szDrtTestLog) < MAX_PATH - 4 )
                strcat_s(szDrtTestLog, sizeof(szDrtTestLog), ".log");

            DeleteFileA ( szDrtTestLog );
            break;
        }
        --iPos;
    }
}

/*++

Routine Description:

    RunDrt

    Spawns a process to run the specified drt
--*/
DWORD RunDrt(UINT uDrt, 
             DWORD dwTimeout,
             char *szText)
{
    DWORD dwStatus = 0;
    PROCESS_INFORMATION procinfo;
    STARTUPINFOA si;
    PROCESS_MEMORY_COUNTERS memcounters;
    ULONGLONG ftStart, ftEnd, ftKernel, ftUser, ftElapsed;

    HRESULT hr = S_OK;
    bool fRunScript;
    char szCommandLine[MAX_PATH] = "";
    const PCSTR pszDrtName = g_rpDrtList[uDrt]->pszDrtName;

    ZeroMemory( &si, sizeof(si) );
    si.cb = sizeof(si);
    ZeroMemory( &procinfo, sizeof(procinfo) );

    assert(uDrt < g_cDrt);

    if ( !(g_rpDrtList[uDrt]->uArch & g_uArchToRun) )
    {
        // skip due to arch
        printf("\nRunDrts:  %-40s Skipping Drt #%d  %d of %d\n  Drt is not enabled for this architecture",
               pszDrtName, uDrt+1, g_cTotalRunSoFar + 1, g_cTotalDrtsRun);
        g_cTotalRunSoFar++;
        return 0;
    }

    if ( !(g_rpDrtList[uDrt]->uOS & g_uCurrentOS) )
    {
        // skip due to operating system
        printf("\nRunDrts:  %-40s Skipping Drt #%d  %d of %d\n  Drt is not enabled on this operating system",
               pszDrtName, uDrt+1, g_cTotalRunSoFar + 1, g_cTotalDrtsRun);
        g_cTotalRunSoFar++;
        return 0;
    }

    fRunScript = DoesCommandLaunchScriptOrShellCmd(pszDrtName);

    if (fRunScript)
    {
        hr = StringCbPrintfA(szCommandLine, sizeof(szCommandLine),
                             "cmd.exe /c \"");//%s & exit /b\"", pszCmd);
    }

    if (SUCCEEDED(hr))
    {
        hr = StringCbCatA(szCommandLine, sizeof(szCommandLine),
                          pszDrtName);
    }


    if (   SUCCEEDED(hr)
        && g_rpDrtList[uDrt]->pszArgs)
    {
        hr = StringCbCatA(szCommandLine, sizeof(szCommandLine), " ");
        if (SUCCEEDED(hr))
        {
            hr = StringCbCatA(szCommandLine, sizeof(szCommandLine),
                              g_rpDrtList[uDrt]->pszArgs);
        }
    }

    if (   SUCCEEDED(hr)
        && g_fVerbose)
    {
        hr = StringCbCatA(szCommandLine, sizeof(szCommandLine),
                          " -Verbose");
    }


    if (   SUCCEEDED(hr)
        && fRunScript)
    {
        hr = StringCbCatA(szCommandLine, sizeof(szCommandLine),
                          " & exit /b\"");
    }

    // delete any existing log file for this drt so there is no possible confusion about old results
    DeleteDrtLogFile ( pszDrtName );

    if (SUCCEEDED(hr))
    {
        printf("\nRunDrts:  %-40s Starting %s Drt #%d  %d of %d\n",
               szCommandLine, szText, uDrt+1, g_cTotalRunSoFar + 1, g_cTotalDrtsRun);

        // Make sure any pending output is sent before child process takes over
        fflush(stdout);

        if (CreateProcessA(NULL,
                          szCommandLine,
                          NULL,
                          NULL,
                          FALSE,
                          NULL,
                          NULL,
                          NULL,
                          &si,
                          &procinfo))
        {
            dwStatus = WaitForSingleObject(procinfo.hProcess, dwTimeout);

            if (dwStatus == WAIT_TIMEOUT)
            {
                TerminateProcess(procinfo.hProcess, 0xFFFFFFFF);
                g_rpDrtList[uDrt]->iResult = DRT_TIMEOUT_FAILURE;
            }        

            GetProcessTimes(procinfo.hProcess,
                            (FILETIME *)&ftStart,
                            (FILETIME *)&ftEnd,
                            (FILETIME *)&ftKernel,
                            (FILETIME *)&ftUser);

            ftElapsed = (ftEnd - ftStart);
            g_rgTime[uDrt][0] += FileTimeToSeconds((FILETIME *)&ftElapsed);
            g_rgTime[uDrt][1] += FileTimeToSeconds((FILETIME *)&ftUser);
            g_rgTime[uDrt][2] += FileTimeToSeconds((FILETIME *)&ftKernel);

            GetProcessMemoryInfo(procinfo.hProcess, 
                                 &memcounters, sizeof(PROCESS_MEMORY_COUNTERS));
            g_rgMemory[uDrt].PageFaultCount += memcounters.PageFaultCount; 
            g_rgMemory[uDrt].PeakWorkingSetSize += memcounters.PeakWorkingSetSize; 
            g_rgMemory[uDrt].QuotaPeakPagedPoolUsage += memcounters.QuotaPeakPagedPoolUsage; 
            g_rgMemory[uDrt].QuotaPeakNonPagedPoolUsage += memcounters.QuotaPeakNonPagedPoolUsage; 
            g_rgMemory[uDrt].PeakPagefileUsage += memcounters.PeakPagefileUsage; 

            GetExitCodeProcess(procinfo.hProcess, &dwStatus);            

            if (dwStatus)
            {
                g_rgcFailure[uDrt]++;
                g_cFailure++;

                // set the failure to "reported" if it hasn't already been set to something else
                if ( g_rpDrtList[uDrt]->iResult == DRT_NOT_RUN )
                    g_rpDrtList[uDrt]->iResult = DRT_REPORTED_FAILURE; 
            }
            else
            {
                g_rgcSuccess[uDrt]++;
                g_cSuccess++;
                g_rpDrtList[uDrt]->iResult = DRT_SUCCESS;
            }
            g_rpDrtList[uDrt]->ftElapsed = FileTimeToSeconds((FILETIME *)&ftElapsed);
            CloseHandle(procinfo.hThread);
            CloseHandle(procinfo.hProcess);

            printf("RunDrts:  %-40s completed in %8.3lf sec Peak WS %d pages with code 0x%8x\n",
                   szCommandLine, 
                   FileTimeToSeconds((FILETIME *)&ftElapsed), 
                   static_cast<int>(memcounters.PeakWorkingSetSize / 0x1000),
                   dwStatus);
        }
        else
        {
            printf("RunDrts:  %-40s failed to start\n", szCommandLine);
            dwStatus = 0xFFFFFFFF;
        }
    }
    else
    {
    }

    g_cTotalRunSoFar++;
    return dwStatus;
}
/*++

Routine Description:

    RunAll

    Runs all the DRTs in the run list in order
--*/
DWORD 
RunAll(BOOL fBreakOnFailure,
       DWORD dwTimeout)
{
    DWORD dwStatus = 0;
    for (UINT n = 0; n < g_cRuns; n++ )
    {
        for (UINT i = 0; 
            (i < g_cDrt) && ((dwStatus == 0) || !fBreakOnFailure);  
            i++)
        {
            if ( RunDrt(i, dwTimeout, "") != 0 )
                dwStatus = 1;
        }
    }

    return dwStatus;
}

/*++

Routine Description:

    configure machine to run drts

-- */

bool
SetConfig()
{
    bool fSuccess;

    if (g_fSetConfigCalled)
    {
        printf("Internal error: SetConfig called multiple times.\n");
        return false;
    }

    g_fSetConfigCalled = true;

    fSuccess = !!SystemParametersInfo(SPI_GETMENUSHOWDELAY, 0, &g_cOldMenuShowDelay, 0);
    if (fSuccess)
    {
        printf("Note: previous Menu Show Delay setting was %u.\n", g_cOldMenuShowDelay);
    }

    fSuccess = fSuccess && !!SystemParametersInfo(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, &g_cOldForegroundLockTimeout, 0);
    if (fSuccess)
    {
        printf("Note: previous Foreground Lock Timeout setting was %u.\n", g_cOldForegroundLockTimeout);
    }

    fSuccess = fSuccess &&
        RunCommandLine ( g_szHostingSetup );

    // Gac the SEE Test DLL 
    fSuccess = fSuccess && 
        RunCommandLine( g_szInstallSEETestDll) ; 
    
    // Setup Drt version of wisptis (wisptis_.exe)
    fSuccess = fSuccess &&
        RunCommandLine( g_szInstallWisptis );

    // Set SPI MENUSHOWDELAY to be 10 ms.  This speeds up menu tests significantly.
    fSuccess = fSuccess &&
        SystemParametersInfo(SPI_SETMENUSHOWDELAY, 10 /* new menushow delay */, 0, 0);

    // Set SPI FOREGROUNDLOCKTIMEOUT to be 0 ms.  This allows our DRTS to pop to the foreground no matter what.
    fSuccess = fSuccess &&
        SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0 /* new foreground lock timeout */, 0, 0);

    // Prepare for PrivateOnVista
    if (g_fPrivateOnVista)
    {
        fSuccess = fSuccess && DoPrivateOnVista(actionSet);
    }

    // Hide Product Binaries
    fSuccess = fSuccess &&
        MoveProductBinaries(actionSet);

    //
    // add more config here, but remember to add verification code to
    // CheckConfig and cleanup to RevertConfig as well
    //

    if (fSuccess)
    {
        // The configuration check is done via a spawned process so that
        // shims can be verified.  This also helps verify that all other
        // settings are properly inherited.
        printf("Test environment configured.  Verifying...\n");
        fSuccess = RunCommandLine( "RunDRTs.exe /CheckConfig" );
    }

    if (!fSuccess)
    {
        // Don't show errors because they probably come from failing to
        // to setup in the first place.
        printf("Trying to restore after unsuccessful setup. Expect errors...\n");
        RevertConfig();
    }

    return fSuccess;
}


/*++

Routine Description:

    check RGBRast install to see if it is okay

    returns TRUE if okay, FALSE if not
-- */
bool
CheckRGBRastInstall()    
{
    bool fInstallOkay;

    //
    // Ensure that the RGBRast is can be loaded.
    //
    HMODULE hRGBRastLoad = LoadLibraryA(g_szRGBRastModulePath);

    fInstallOkay = (hRGBRastLoad != NULL);

    if (!fInstallOkay)
    {
        printf(
            "Test load of %s failed!\n"
            "Test apps that use software 3D may fail.\nInstall WPF once on your DRT machine using wpfSetup.exe to have rgb9rast installed.\n",
            g_szRGBRastModulePath
            );
    }

    FreeLibrary(hRGBRastLoad);

    return fInstallOkay;
}

/*++

Routine Description:

    check SEE Test config to see if it is okay

    returns TRUE if okay, FALSE if not
-- */
bool
CheckSEETestConfig()    
{
    //
  return TRUE ; 
}


/*++

Routine Description:

    Gets the current OS version.  We currently only differentiate between
    Windows Vista and all other supported Windows versions.

-- */
void
GetOSVersion()
{
    OSVERSIONINFO osvi;

    ZeroMemory(&osvi, sizeof(osvi));
    osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
    
    GetVersionEx(&osvi);

    if (osvi.dwMajorVersion >= 6)
    {
        g_uCurrentOS = OS_VISTA;
    }
    else 
    {
        g_uCurrentOS = OS_XP;
    }
}

/*++

Routine Description:

    check configuration to see if it is okay

    returns TRUE if okay, FALSE if not
-- */
bool
CheckConfig()
{
    bool fConfigOkay = true;

    if (g_fSetConfigCalled)
    {
        printf("Internal error: CheckConfig may not be called after SetConfig!\n"
               "Use RunCommandLine(\"RunDrts.exe /CheckConfig\"); as needed.\n");
        return false;
    }

    // 


    fConfigOkay = MoveProductBinaries(actionCheck) && fConfigOkay;

    fConfigOkay = CheckRGBRastInstall() && fConfigOkay;
    fConfigOkay = CheckSEETestConfig() && fConfigOkay; 

    // Check wisptis config to be sure wisptis_.exe is used.
    fConfigOkay = fConfigOkay && RunCommandLine( g_szCheckWisptisConfig );

    // Check Print config
    fConfigOkay = fConfigOkay && RunCommandLine( g_szCheckPrintConfig );

    // Check for PrivateOnVista
    if (g_fPrivateOnVista)
    {
        fConfigOkay = fConfigOkay && DoPrivateOnVista(actionCheck);
    }

    if (fConfigOkay)
    {
        printf( "Configuration is okay\n" );
    }
    else
    {
        printf( "Configuration is NOT okay!\n" );
    }

    return fConfigOkay;
}

/*++

Routine Description:

    reset configuration on machine after running drts

-- */
bool
RevertConfig()
{
    bool fSuccess = true;

    if (!g_fSetConfigCalled)
    {
        printf("Warning: RevertConfig may only be called after SetConfig\n"
               "         as not all state is saved to disk.\n");
    }

    // Restore Product Binaries
    if (!MoveProductBinaries(actionRevert))
    {
        printf("Restore error: failed to restore product binaries.\n");
        fSuccess = false;
    }

    // Cleanup for PrivateOnVista
    if (g_fPrivateOnVista)
    {
        // Want to revert our install, even if other revert operations have failed
        fSuccess = DoPrivateOnVista(actionRevert) && fSuccess;
    }

    if (g_cOldMenuShowDelay <= 0)
    {
        g_cOldMenuShowDelay = 400;
    }

    if (!SystemParametersInfo(SPI_SETMENUSHOWDELAY, g_cOldMenuShowDelay, 0, SPIF_SENDCHANGE))
    {
        printf("Restore error: failed to restore menu show delay settings.\n");
        fSuccess = false;
    }

    if (g_cOldForegroundLockTimeout <= 0)
    {
        g_cOldForegroundLockTimeout = 200000;
    }

    if (!SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, g_cOldForegroundLockTimeout, 0, SPIF_SENDCHANGE))
    {
        printf("Restore error: failed to restore foregound lock timeout settings.\n");
        fSuccess = false;
    }

    // Disable SEE drt util
    if (!RunCommandLine( g_szUninstallSEETestDll ))
    {
        printf("Restore error: failed to uninstall seedrtutil .\n");
        fSuccess = false;
    }
    
    // Disable Drt version of wisptis and restore previous wisptis (as needed)
    if (!RunCommandLine( g_szUninstallWisptis ))
    {
        printf("Restore error: failed to uninstall Drt version of wisptis (wisptis_.exe).\n");
        fSuccess = false;
    }

    // Clear ClickOnce cache
    if (!RunCommandLine( g_szHostingCleanup ))
    {
        printf("Restore error: failed to clear fusion cache.\n");
        fSuccess = false;
    }

    return fSuccess;
}


/*++

Routine Description:

    clean up any memory, etc.

-- */
void
Cleanup()
{

    for (UINT u = 0; u < g_cDrt; u++)
    {
        if ( g_rpDrtList[u] )
        {
            if (g_rpDrtList[u]->pszDrtName )
                free (g_rpDrtList[u]->pszDrtName);
            if (g_rpDrtList[u]->pszOwner )
                free (g_rpDrtList[u]->pszOwner);
            if (g_rpDrtList[u]->pszArgs )
                free (g_rpDrtList[u]->pszArgs);
            free ( g_rpDrtList[u]);
        }
    }

    if ( !g_fUseDrtListFile)
    {
        // need to clean up other stuff
        for (UINT u = 0; u < g_cDrt; u++ )
        {
            if ( g_rgszDrt[g_cDrt] )
                free (g_rgszDrt[g_cDrt]);
        }
    }
}

/*++

Routine Description:

    InitializeRGBRastGlobals

    Initialize RGB rast install/check config globals
--*/
void InitializeRGBRastGlobals()
{
    //
    // Determine RGB rast install path and name
    //

    SHGetFolderPathA( NULL, CSIDL_SYSTEM, NULL, SHGFP_TYPE_CURRENT,
                      g_szRGBRastModulePath );

    StringCbCatA(g_szRGBRastModulePath, sizeof(g_szRGBRastModulePath),
                 g_uCurrentOS == OS_VISTA ?
                    "\\" VISTA_RGBRAST_DLL :
                    "\\" DOWNLEVEL_RGBRAST_DLL
                 );
}


/*++

Routine Description:

    FileExists

    returns true if the file exists
--*/
bool FileExists(char* pszFilename)
{
    WIN32_FIND_DATAA findFileData;
    ZeroMemory( &findFileData, sizeof(findFileData) );
    HANDLE hFind = INVALID_HANDLE_VALUE;
    hFind = FindFirstFileA(pszFilename, &findFileData);

    // check if the source file exists
    if (hFind == INVALID_HANDLE_VALUE)
        return false;

    FindClose(hFind);
    return true;
}

/*++

Routine Description:

    DirExists

    returns true if the directory exists
--*/
bool DirExists(char* pszDirname)
{
    WIN32_FIND_DATAA findFileData;
    HANDLE hFind = INVALID_HANDLE_VALUE;
    hFind = FindFirstFileA(pszDirname, &findFileData);

    // check if the source file exists
    if (hFind == INVALID_HANDLE_VALUE)
        return false;
    FindClose(hFind);

    return (0 != (findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY));
}


/*++

Routine Description:

    MkLink

    set up DLL redirection
--*/
bool MkLink(char* pszPath)
{
    char szCmd[MAX_PATH * 2];

    // This command is only available on Vista...
    if (g_uCurrentOS != OS_VISTA)
    {
        printf("\nERROR - mklink command is only available on Vista machines.\n\n");
        return false;
    }	

    if (FAILED(StringCbPrintfA(szCmd, sizeof(szCmd), "mklink /D %s %s\\PrivateOnVista > NUL", pszPath, g_pszSystemDirectory)))
        return false;
    if (g_fVerbose)
    {
        printf(szCmd);
        printf("\n");
    }
    return RunCommandLine(szCmd);
}

/*++

Routine Description:

    RmLink

    remove DLL redirection
--*/
bool RmLink(char* pszPath)
{
    char szCmd[MAX_PATH * 2];
    if (FAILED(StringCbPrintfA(szCmd, sizeof(szCmd), "rmdir %s", pszPath)))
        return false;
    if (g_fVerbose)
    {
        printf(szCmd);
        printf("\n");
    }
    return RunCommandLine(szCmd);
}

/*++

Routine Description:

    DoPrivateOnVista()

    loop through all executables in this directory, and...

    actionSet => mklink /D XXX.exe %systemdirectory%\\PrivateOnVista
    actionCheck => Check if links all exist
    actionRemove => rmdir xxx.exe.local
--*/
bool DoPrivateOnVista(ActionEnum actionEnum)
{
    bool fSuccess = true;
    WIN32_FIND_DATAA findFileData;
    HANDLE hFind = INVALID_HANDLE_VALUE;
    char szTargetPath[MAX_PATH + 1]; // Path of private directory containing DLLs
    char szPath[MAX_PATH + 1];  // Path for all .exes in current directory
    DWORD dwError, rc;


    // Create path for all .exes
    if (0 == (rc = GetCurrentDirectoryA(sizeof(szPath), szPath)))
    {
        printf("PrivateOnVista: Cannot get current directory, error=%d\n", GetLastError());
        return false;
    }
    if (FAILED(StringCbCatA(szPath, sizeof(szPath), "\\*.exe")))
        return false;

    // If we are reverting, we don't need to check for the target location 
    if (actionEnum != actionRevert)
    {
        // This only works on Vista...
        if (g_uCurrentOS != OS_VISTA)
        {
            printf("\nPrivateOnVista: ERROR - Option is only available on Vista machines.\n\n");
            return false;
        }		

        // Get the system directory is...
        SHGetFolderPathA( NULL, CSIDL_SYSTEM, NULL, SHGFP_TYPE_CURRENT, g_pszSystemDirectory );

        // Check to see if DLL redirection is setup (by the presence of the private directory)
        if (FAILED(StringCbPrintfA(szTargetPath, sizeof(szTargetPath), "%s\\PrivateOnVista", g_pszSystemDirectory)))
            return false;

        if (!FileExists(szTargetPath))
        {
            printf("\nPrivateOnVista: ERROR - DLL Redirection was not installed.  Use the -PrivateOnVista option with WpfSetup.exe.\n");
            return false;
        }
    }

    hFind = FindFirstFileA(szPath, &findFileData);

    if (hFind == INVALID_HANDLE_VALUE && actionEnum != actionRevert) 
    {
        printf ("PrivateOnVista: cannot find exe files to process. Error is %u\n", GetLastError());
        return false;
    } 
    else 
    {
        char szFilePath[MAX_PATH + 1];   // Path to individual .exe file
        do
        {
            if (FAILED(StringCbPrintfA(szFilePath, sizeof(szFilePath), "%s.local", findFileData.cFileName)))
            {
                fSuccess = false;
                break;
            }
            if (actionEnum == actionSet)
            {
                if (!FileExists(szFilePath))
                {
                    if (!MkLink(szFilePath))
                    {
                        printf("PrivateOnVista: ERROR - failed to create link %s\n", szFilePath);
                        fSuccess = false;
                        break;
                    }
                }
                else                
                    printf("PrivateOnVista: %s already exists.\n", szFilePath);
            }
            else if (actionEnum == actionRevert)
            {
                if (FileExists(szFilePath))
                {
                    if (!RmLink(szFilePath))
                    {
                        printf("PrivateOnVista: ERROR - failed to remove link %s\n", szFilePath);
                        fSuccess = false;
                    }
                }
                else
                    printf("PrivateOnVista: %s does not exist.  Nothing to remove.\n", szFilePath);
            }
            else if (actionEnum == actionCheck)
            {
                if (!FileExists(szFilePath))
                {
                    printf("PrivateOnVista: %s does not exist.  DLL redirection is not setup properly.\n", szFilePath);
                    fSuccess = false;
                }
            }
            else // unexpected
            {
                fSuccess = false;
                break;
            }
        }
        while (FindNextFileA(hFind, &findFileData) != 0) ;  // do .. while

        if (fSuccess)
        {
            dwError = GetLastError();
            FindClose(hFind);
            if (dwError != ERROR_NO_MORE_FILES) 
            {
                printf ("PrivateOnVista: FindNextFile error. Error is %u\n", dwError);
                return false;
            }
        }
    }
    return fSuccess;
}

/*++

Routine Description:

    MoveProductBinaries

    remove product binaries from local directory to force DRT apps to 
    load binaries installed and GAC-ed from setup.
    Binaries are not deleted but instead moved to a HideProdBins directory.
    Set bRestore==TRUE to restore binaries back from the hidden directory
--*/
bool MoveProductBinaries(ActionEnum action)
{
    char szCurrDirPath[MAX_PATH];       // path of current directory (DRT dir)
    char szHideDirPath[MAX_PATH];       // path of directory for hiding the binaries
    char szOrigPath[MAX_PATH];          // file path of product binary in DRT dir
    char szHidePath[MAX_PATH];          // file path of product binary in Hidden dir
    DWORD rc = 0;

    if (0 == (rc = GetCurrentDirectoryA(sizeof(szCurrDirPath), szCurrDirPath)))
    {
        printf("MoveProductBinaries: Cannot get current directory, error=%d\n", GetLastError());
        return false;
    }

    if (FAILED(StringCbPrintfA(szHideDirPath, sizeof(szHideDirPath), "%s\\%s", szCurrDirPath, g_HideProdBins)))
        return false;

    if (actionSet == action && !DirExists(szHideDirPath))
    {
        if (!CreateDirectoryA(szHideDirPath, NULL))
        {
            printf("MoveProductBinaries: Cannot create directory for hiding product binaries, error=%d\n", GetLastError());
            return false;
        }
        else if (g_fInfo)
        {
            printf("MoveProductBinaries: created directory for hiding product binaries: %s\\\n", g_HideProdBins);
        }
    }

    bool fSuccess = true;
    char* pszBinary;
    for (int i = 0; NULL != (pszBinary = g_szProductBinaries[i]); ++i)
    {
        if (FAILED(StringCbPrintfA(szOrigPath, sizeof(szOrigPath), "%s\\%s", szCurrDirPath, pszBinary)))
            return false;

        if (FAILED(StringCbPrintfA(szHidePath, sizeof(szHidePath), "%s\\%s\\%s", szCurrDirPath, g_HideProdBins, pszBinary)))
            return false;

        if (actionSet == action)
        {   // move to hidden directory
            if (FileExists(szOrigPath))
            {
                // overwrite any existing files in the "hide" directory
                if (FAILED(MoveFileExA(szOrigPath, szHidePath, MOVEFILE_REPLACE_EXISTING)))
                {
                    printf("MoveProductBinaries: Unable to move %s to %s\\ , error=%d\n", pszBinary, g_HideProdBins, GetLastError());
                    return false;   // do not continue trying to hide files; abort!
                }
                else if (g_fInfo)
                {
                    printf("MoveProductBinaries: moved %s to %s\\ \n", pszBinary, g_HideProdBins);
                }
            }
            else if (g_fInfo)
            {
                printf("MoveProductBinaries: %s does not exist in current directory.  Nothing to hide.\n", pszBinary);
            }
        }
        else if (actionRevert == action)
        {   // restore from hidden directory to local dir
            if (FileExists(szOrigPath))
            {
                if (g_fInfo)
                {
                    printf("MoveProductBinaries: %s already exists in current directory.  Not restoring.\n", pszBinary);
                }
            }
            else
            {
                if (!FileExists(szHidePath))
                {
                    if (g_fInfo)
                    {
                        printf("MoveProductBinaries: Could not find %s to restore. \n", pszBinary);
                    }
                }
                else if (FAILED(MoveFileA(szHidePath, szOrigPath)))
                {
                    printf("MoveProductBinaries: Could not restore %s , error=%d\n", pszBinary, GetLastError());
                    fSuccess = false;   // continue to attempt to restore other files
                }
                else if (g_fInfo)
                {
                    printf("MoveProductBinaries: restored %s from %s\\\n", pszBinary, g_HideProdBins);
                }
            }
        }
        else // actionCheck == action
        {
            if (FileExists(szOrigPath))
            {
                printf("MoveProductBinaries: %s exists in current directory.  It should be hidden away during DRT run.\n", pszBinary);
                fSuccess = false;   // continue to check for other files that should be hidden
            }
        }
    }
    return fSuccess;
}

/*++

Routine Description:

    main

    RunDrts main program
--*/
int _cdecl main(int argc, char *argv[])
{
    DWORD dwStatus = 0;

    // Call this first - the current OS is used later to make decisions
    GetOSVersion();

    InitializeRGBRastGlobals();


    // Parse command line argument
    argc--; argv++;

    ZeroMemory( &g_rpDrtList, sizeof(DRT_ENTRY*)*MAX_DRTS);
    ZeroMemory( &g_rgszDrt, sizeof(char*)*MAX_DRTS); 
    ZeroMemory( &g_rgTime, sizeof(ULONGLONG)*3*MAX_DRTS);
    ZeroMemory( &g_rgcSuccess, sizeof(UINT)*MAX_DRTS);
    ZeroMemory( &g_rgcFailure, sizeof(UINT)*MAX_DRTS);
    ZeroMemory( &g_rgMemory, sizeof(PROCESS_MEMORY_COUNTERS) * MAX_DRTS);

    ParseArgs(argc, argv); 


    // delete any existing logfile so there is no confusion
    DeleteFileA ( g_szOutFilename );

    // Load the list of drts
    if (g_fUseDrtListFile)
    {
        if (!LoadDrtsFromFile())
        {
            // bad syntax in DRT file, abort this run
            printf("RunDrts:  Cannot read DRT config file '%s' - aborting drt run\n", g_szDrtFilename);
            Cleanup();
            exit(1);
        }
    }
    else if (g_cDrt > 0)
    {
        LoadDrtsFromArray();
    }


    if (g_cDrt)
    {
        if ( !g_fSkipConfig )
        {
            if (!SetConfig())
            {
                // don't run if config is broken
                printf("RunDrts:  Invalid Machine Configuration - aborting drt run\n");
                Cleanup();
                exit(1);
            }
        }
        else
        {        
            // just check the config and print any warnings, but go ahead and run regardless
            if (!CheckConfig())
            {
                printf("RunDrts:  Invalid Machine Configuration - continuing anyway...\n");
            }
        }
        g_cTotalDrtsRun = g_cDrt * g_cRuns;

        time_t tstart, tend;
        tstart = time(NULL);
        dwStatus = RunAll(g_fBreakOnFailure, g_dwTimeout);

        tend = time(NULL);
        g_dTimeTotal = difftime(tend, tstart);

        if ( !g_fSkipConfig )
            RevertConfig();

        if ( g_NotRunCountsAsPassed )
        {
            // if we're not really counting not run as failures, then just look for actual failures
            g_fPassed = (g_cFailure == 0 );
        }
        else
        {
            // for RTM, we want this
            g_fPassed = (g_cSuccess == g_cTotalDrtsRun);
        }

        WriteLogFile();
        PrintSummary();
    }
    else
    {
        printf("RunDrts:  no drts found in list.\n" );
    }

    Cleanup();

    return dwStatus;
}
