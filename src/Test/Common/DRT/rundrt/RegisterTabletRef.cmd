@echo off
setlocal

@echo RegisterTabletRef.cmd: Configures DRT version of wisptis (wisptis_.exe and tpcps.dll) [Current directory = %cd%]

set ARG=%1

if "%ARG%" == "/idrt" goto installwisptisdrt
if "%ARG%" == "/udrt" goto uninstallwisptisdrt
if "%ARG%" == "/checkdrt" goto checkconfigdrt

REM Invalid command line.
echo Unrecognized command line argument '%1' while trying to install Tablet Pen Service.
echo    /idrt     - [Re]Install DRT version of pen service
echo    /udrt     - Uninstall DRT version pen service (and restore)
echo    /checkdrt - Checks if DRT version of pen service is set up properly
exit /b 1


:installwisptisdrt
@REM Currently this is disabled until we move to using a new stylus input injection technique.
goto installsuccess

call :IsTabletPC
if not ERRORLEVEL 1 echo FAILURE - DRTs are not supported on a TabletPC OS&exit /b 1
call :killwisptis
call :install
if ERRORLEVEL 1 exit /b 1
:installsuccess
echo RegisterTabletRef.cmd: SUCCESS (install)
exit /b 0


:uninstallwisptisdrt
@REM Currently this is disabled until we move to using a new stylus input injection technique.
goto uninstallsuccess

call :IsTabletPC
if not ERRORLEVEL 1 echo NOTICE - Uninstall disabled on TabletPC OS&goto noptabletpc
call :killwisptis
call :uninstall
if ERRORLEVEL 1 exit /b 1
:noptabletpc
:uninstallsuccess
echo RegisterTabletRef.cmd: SUCCESS (uninstall)
exit /b 0


:checkconfigdrt
@REM Currently this is disabled until we move to using a new stylus input injection technique.
goto checkconfigok

@echo Checking to be sure pen service is set up properly (wisptis_.exe)

REM We don't currently support running DRTs on a tabletpc OS so fail if tabletpc.
call :IsTabletPC
if not ERRORLEVEL 1 echo %~n0: FAILED - Drts are not supported on a TabletPC OS&exit /b 1

reg query HKCR\CLSID\{A5B020FD-E04B-4e67-B65A-E7DEED25B2CF}\LocalServer32 | findstr /I /C:wisptis_.exe
if ERRORLEVEL 1 echo %~n0: FAILED - Wisptis_.exe not registered properly&exit /b 1

reg query HKLM\SOFTWARE\Classes\Interface\{A56AB812-2AC7-443D-A87A-F1EE1CD5A0E6} /v ""
if ERRORLEVEL 1 echo %~n0: FAILED - Wisptis_.exe Proper proxy stub tpcps.dll not registered  (wisptis_.exe) not registered properly (ITabletManagerDrt not found)&exit /b 1
reg query HKCR\CLSID\{9E358D23-02B2-4CCD-9FEE-6B75EE8DD5CA}\InprocServer32 /v ""
if ERRORLEVEL 1 echo %~n0: FAILED - Wisptis_.exe proxy stub tpcps.dll not registered properly.&exit /b 1
:checkconfigok
echo RegisterTabletRef.cmd: SUCCESS (check config)
exit /b 0


:IsTabletPC
reg query "HKLM\System\CurrentControlSet\Control\Session Manager\WPA\TabletPC" /v installed | findstr /I /C:0x1
if ERRORLEVEL 1 exit /b 1
exit /b 0


:killwisptis
@echo shutting down pen service
@tasklist >_
@findstr /I /C:"wisptisxp_.exe" _
@if errorlevel 1 goto WisptisXpDrtNotRunning
@taskkill /F /IM wisptisxp_.exe
:WisptisXpDrtNotRunning
@findstr /I /C:"wisptis_.exe" _
@if errorlevel 1 goto WisptisDrtNotRunning
@taskkill /F /IM wisptis_.exe
:WisptisDrtNotRunning
@findstr /I /C:"wisptis.exe" _
@if errorlevel 1 goto WisptisNotRunning
@taskkill /F /IM wisptis.exe
:WisptisNotRunning

exit /b 0


:install

REM tpcps.dll must be registered
@echo registering tpcps.dll
if not exist "%ProgramFiles%\Common Files\Microsoft Shared\Ink\tpcps.dll" goto regtpcps
%windir%\system32\regsvr32 /u /s "%ProgramFiles%\Common Files\Microsoft Shared\Ink\tpcps.dll"
:regtpcps
%windir%\system32\regsvr32 /s %~dp0tpcps.dll
@if errorlevel 1 echo %~n0: FAILED to register tpcps.dll&exit /b 1

REM For 64 bit platforms we also need to register 32 bit version of tpcps.dll
if /I %PROCESSOR_ARCHITECTURE% == x86 goto checkwisptis
@echo registering x86 version of tpcps.dll for 64 to 32 bit interop
%windir%\syswow64\regsvr32 /s %~dp0tpcps_x86.dll
@if errorlevel 1 echo %~n0: FAILED to register tpcps_x86.dll&exit /b 1

:checkwisptis
REM wisptis_.exe must be registered
if not exist "%windir%\System32\wisptis.exe" goto regwisptis
@echo unregistering wisptis.exe (%windir%\System32\wisptis.exe /UnregServer)
%windir%\System32\wisptis.exe /UnregServer
:regwisptis
@echo registering wisptis_.exe
%~dp0wisptis_.exe /RegServer
@if errorlevel 1 echo %~n0: FAILED to register wisptis_.exe&exit /b 1

@echo launching pen service
start %~dp0wisptis_.exe
@tasklist >_
@findstr /I /C:"wisptis_.exe" _
@if errorlevel 1 echo %~n0: FAILED to launch wisptis_.exe&exit /b 1
exit /b 0


:uninstall

@echo unregistering wisptis_.exe (and re-registering existing one)

@rem Unregister wisptis_.exe and tpcps.dll
%~dp0wisptis_.exe /UnregServer
%windir%\system32\regsvr32 /u /s %~dp0tpcps.dll
REM For 64 bit platforms we also need to unregister the 32 bit version of tpcps.dll
if /I %PROCESSOR_ARCHITECTURE% == x86 goto reregwisptis
%windir%\syswow64\regsvr32 /u /s %~dp0tpcps_x86.dll
@if errorlevel 1 echo %~n0: FAILED to register tpcps_x86.dll&exit /b 1

:reregwisptis
@rem We only re-register wisptis and tpcps.dll if we find wisptis in system32 dir.
if not exist %windir%\System32\wisptis.exe goto nocleanup
%windir%\System32\wisptis.exe /RegServer
@if errorlevel 1 echo %~n0: FAILED to re-register wisptis.exe&exit /b 1

@echo unregistering tpcps.dll (and re-registering existing one)
if not exist "%ProgramFiles%\Common Files\Microsoft Shared\Ink\tpcps.dll" echo %~n0: FAILED to find original tpcps.dll&exit /b 1
%windir%\system32\regsvr32 /s "%ProgramFiles%\Common Files\Microsoft Shared\Ink\tpcps.dll"
@if errorlevel 1 echo %~n0: FAILED to re-register original tpcps.dll&exit /b 1
start %windir%\System32\wisptis.exe

:nocleanup
exit /b 0
