// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace Microsoft.Test.Debug
{
    /// <summary>
    /// Enum indicating whether a remote session is User or Kernel mode.
    /// </summary>
    internal enum DebugRequestType
    {
        /// <summary>
        /// User debugger
        /// </summary>
        User,
        /// <summary>
        /// Kernel debuger
        /// </summary>
        Kernel
    }

    /// <summary>
    /// This class is only left here as a reference for how this system once worked 
    /// in pre V1 RTM.  It may be useful to reference when trying to reimplement
    /// pieces of its functionality.  Under no conditions should this ever be re-enabled
    /// directly as it is complete trash.
    /// </summary>
    internal sealed class DefunctDebugger
    {
        #region Private Members

        static string cdbInstallLocation = @"%SYSTEMDRIVE%\debuggers\cdb.exe";

        #endregion

        #region Protected Members

        /// <summary>
        /// Characters of interest when parsing cdb spew.
        /// </summary>
        protected static char[] BreakOnChars = new char[] { ' ', ',', ')' };

        #endregion

        #region Abstract Members

        /// <summary>
        /// Send a command to remote session but don't retrieve its output.
        /// </summary>
        /// <remarks>Removes any perf overhead around parsing the standard output
        /// of the cdb session if we don't really care about the spew from an individual
        /// command.</remarks>
        /// <param name="cmd">Command to execute.</param>
        public void SendCommand(string cmd)
        {
            throw new NotImplementedException("This class is deprecated, and only exists for reference purposes.");
        }

        /// <summary>
        /// Send command to the remote session and and return the output text.
        /// </summary>
        /// <remarks>Inserts start/end echo tags around the given cmd and then
        /// parses the standard output of the process to find the output of
        /// the given cmd.</remarks>
        /// <param name="cmd">Command to execute</param>
        /// <returns>cdb output associated with given command.</returns>
        public string SendCommandWithOutput(string cmd)
        {
            throw new NotImplementedException("This class is deprecated, and only exists for reference purposes.");
        }

        /// <summary>
        /// Base class will report errors or messages using this api.
        /// </summary>
        /// <param name="msg"></param>
        protected void LogString(string msg)
        {
            throw new NotImplementedException("This class is deprecated, and only exists for reference purposes.");
        }

        /// <summary>
        /// Type of cdb session we are debugging.
        /// </summary>
        public DebugRequestType RequestType
        {
            get { throw new NotImplementedException("This class is deprecated, and only exists for reference purposes."); }
            set { throw new NotImplementedException("This class is deprecated, and only exists for reference purposes."); }
        }

        /// <summary>
        /// Id of process we are attached to.
        /// </summary>
        public int ProcessId
        {
            get { throw new NotImplementedException("This class is deprecated, and only exists for reference purposes."); }
            set { throw new NotImplementedException("This class is deprecated, and only exists for reference purposes."); }
        }

        /// <summary>
        /// Get whether we are debugging a dump file or a live remote.
        /// </summary>
        public bool IsDumpFile
        {
            get { throw new NotImplementedException("This class is deprecated, and only exists for reference purposes."); }
        }

        /// <summary>
        /// Path to lookup symbols.
        /// </summary>
        public string SymbolsPath
        {
            get
            {
                return string.Empty; // 
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Verify that debuggers are properly installed
        /// </summary>
        /// <exception cref="InvalidOperationException">If debuggers are not installed.</exception>
        public static void EnsureDebuggers()
        {
            // In the future we may want to verify the version of the debuggers as well,
            // pass "-version" to cdb to get this info.
            string cdbPath = Environment.ExpandEnvironmentVariables(cdbInstallLocation);
            if (!File.Exists(cdbPath))
            {
                throw new InvalidOperationException(string.Format("Cannot debug failure, debuggers are not installed on this machine.  Please install debuggers from http://dbg to '{0}'.", cdbPath));
            }
        }

        /// <summary>
        /// Prepares session for debugging by setting up symbols etc.
        /// </summary>
        public void Initialize()
        {
            SetupSymbols();
            if (GetIsKernelDebugSession())
            {
                RequestType = DebugRequestType.Kernel;
                ProcessId = FindProcessIdFromKernel();
            }

            if (RequestType != DebugRequestType.Kernel && GetIsManagedSession())
            {
                LoadManagedModules();
            }
        }

        /// <summary>
        /// Try and find the exception that triggered the crash and then return the exception type, 
        /// its message, and callstack.
        /// </summary>
        /// <remarks>
        /// 






        public bool FindException(out string callStack, out string exceptionInfo, out string exceptionMessage)
        {
            callStack = string.Empty;
            exceptionInfo = string.Empty;
            exceptionMessage = string.Empty;

            bool result = false;
            string procName = "ResolveFullCallStack";

            // first check if mscorwks is present - that will identify this to be a
            // managed crash
            if (RequestType != DebugRequestType.Kernel &&
                SendCommandWithOutput("lm mmscorwks").ToLower().IndexOf("mscorwks") >= 0)
            {

                //load SOS managed extension dll
                //SendCommandWithOutput(".load msvcr80");
                SendCommandWithOutput(".loadby sos mscorwks");
                SendCommandWithOutput(".cxr");

                // test to make sure that cordll worked, if not - we got bad symbols/sos combo
                // otherwise continue "..as planned."
                SendCommandWithOutput(".cordll -u -l");

                // we are thinking a good way to find exception obejct
                // is to go back to !threads
                // now we attempt to get successful !thread output
                // if we succeed here, we will go on
                string bangThreads = string.Empty;
                if ((bangThreads = SendCommandWithOutput("!threads")).ToLower().IndexOf("failed") < 0)
                {
                    string threadObject = string.Empty;

                    string[] straThreadSplit = bangThreads.Split(new char[] { '\n', '\r' });

                    for (int i = straThreadSplit.Length - 1; result == false && i >= 0; i--)
                    {
                        string strLine = straThreadSplit[i];
                        string[] straLineSplit = strLine.Split(' ');

                        try
                        {
                            int j = 0;

                            // dead threads are marked with "XXX"
                            // so we will by pass them by moving to the next line
                            if (straLineSplit[j] == "X")
                            {
                                continue;
                            }

                            while (j < straLineSplit.Length && straLineSplit[j] == "") { j++; }
                            if (j >= straLineSplit.Length)
                            {
                                continue;
                            }

                            // parsing of !Threads output starts from the bottom of the list
                            // where we encounter things like "0:010>" which is obviously not
                            // a thread id. we will make sure parsing succeded before we
                            // add to the threadsIds list
                            uint iThread = 0;

                            // NOTE: we have cant use TryParse because its only available in CLR2
                            // but this code base needs to be V1 complaint to make IE folks happy
                            try
                            {
                                iThread = UInt32.Parse(straLineSplit[j], NumberStyles.Integer);
                            }
                            catch (FormatException)
                            {
                                // unparsable - go to the next line
                                continue;
                            }

                            // 


                            j++;

                            while (straLineSplit[j] == "") { j++; }

                            j++;

                            while (straLineSplit[j] == "") { j++; }

                            j++;

                            while (straLineSplit[j] == "") { j++; }

                            // the 4th parameter in the string will be our thread object

                            threadObject = straLineSplit[j];

                            // in case if thread we are looking at has an Exception
                            // written all over it - we will just switch to it and
                            // tell caller we found thread with exception

                            if (strLine.IndexOf("Exception") > 0)
                            {
                                SendCommandWithOutput("~" + iThread.ToString() + "s");
                            }
                            else
                            {
                                continue;
                            }

                            // we were able to switch to a thread that has exception
                            string kpOutput = SendCommandWithOutput("kp100");
                            string oldkpOutput = kpOutput;

                            // attempt to switch to context pointer if one available first
                            int pos = kpOutput.ToLower().LastIndexOf(" _context * ");
                            if (pos >= 0)
                            {
                                // get the remaining line
                                kpOutput = kpOutput.Substring(kpOutput.IndexOf("=", pos) + 2);
                                string address = kpOutput.Substring(0, kpOutput.IndexOfAny(BreakOnChars));

                                string cxrOutput = SendCommandWithOutput(".cxr " + address);

                                // making sure we got what we want
                                if (cxrOutput.ToLower().IndexOf("??") >= 0)
                                {
                                    // if not reset .cxr to default scope and try the other way
                                    SendCommandWithOutput(".cxr");
                                    kpOutput = oldkpOutput;
                                }
                                else
                                {
                                    // found what we looked for!
                                    result = true;
                                }
                            }

                            if (result == false)
                            {
                                // not the same on 64bit machines, CLR rtm 50727
                                // method has different name: "internalunhandledexceptionfilter_worker"
                                // attempt to recover just in case we could
                                pos = kpOutput.ToLower().IndexOf("mscorwks!internalunhandledexceptionfilter_worker(struct _exception_pointers * ");

                                if (pos < 0)
                                {
                                    pos = kpOutput.ToLower().IndexOf("mscorwks!internalunhandledexceptionfilter(struct _exception_pointers * ");
                                }

                                if (pos >= 0)
                                {
                                    result = ChangeContextRecordFromExceptionPointer(kpOutput, pos);
                                }

                                if (pos < 0 || result == false)
                                {
                                    pos = kpOutput.ToLower().IndexOf("kernel32!unhandledexceptionfilter(struct _exception_pointers * ");

                                    if (pos >= 0)
                                    {
                                        result = ChangeContextRecordFromExceptionPointer(kpOutput, pos);
                                    }
                                }

                                // special case when exception and debugbreak are both present (wierd huh?)
                                pos = kpOutput.ToLower().IndexOf("mscorwks!dodebugbreak");

                                if (pos < 0 || result == false)
                                {
                                    // special case when we hit a failfast - in such case we need to capture callstack
                                    pos = kpOutput.ToLower().IndexOf("mscorwks!systemnative::failfast");

                                    if (pos >= 0)
                                    {
                                        // construct exception info/tag for the failure record
                                        // using output of .lastevent
                                        string lastEvent2 = SendCommandWithOutput(".lastevent");
                                        LogString("lastevent : " + lastEvent2);

                                        string[] straLine3 = lastEvent2.Split(new char[] { ':' });
                                        // trim first space and up through \n
                                        try
                                        {
                                            exceptionInfo = straLine3[2].Trim().Substring(0, straLine3[2].Trim().IndexOf(")") + 1);
                                        }
                                        catch
                                        {
                                            // cant parse - bad remote, so we let it go
                                            callStack = "badf00d";
                                            // premature termination because its a bad remote
                                            break;
                                        }
                                        break;
                                    }
                                }

                                // sometimes we already are in the correct context once we switched to the thread
                                // will check on that now

                                pos = kpOutput.ToLower().IndexOf("mscorwks!raisetheexceptioninternalonly");

                                if (pos >= 0)
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogString(procName + " EXCEPTION: ");
                            LogString(e.Message);
                            LogString(e.StackTrace);
                        }
                    }

                    // only do this we ever succeeded
                    // for cases with debugger breaks/faifast and such
                    // we fill exceptionInfo and exceptionMessage separately
                    if (result == true)
                    {
                        GetExceptionInformation(threadObject, ref exceptionInfo, ref exceptionMessage);

                        // dont want to do this if we already got bad news
                        if (callStack != "badf00d")
                        {
                            // make sure we always have lines
                            SendCommandWithOutput(".lines -e");

                            // in case if all of the above has failed - get default callstack           
                            callStack = SendCommandWithOutput("k");

                            callStack = FixWrapping(callStack);

                            // special cases for MIL guys to use milx to extract stack captures
                            if (callStack.ToLower().IndexOf("system.windows.media.mediacontext.notifypartitioniszombie") >= 0)
                            {
                                callStack = FixWrapping(DoMilx());
                            }

                            // got callstack - now return
                            return true;
                        }
                    }
                }
#if BANG_PE_SUPPORT
                // split all threads into an array of lines
                string[] straThreads = callStack.Split(new char[] {'\n', '\r'});
                string strAddress = string.Empty;
                bool bDone = false;

                // parse through the list we got
                for(int i = straThreads.Length - 1; bDone == false && i > 0; i--) {
                    // will walk array of !threads output'ed strings from bottom to top
                    // since this will give us better chance to find what we want
                
                    string strLine = straThreads[i];

                    // if given line is a valid thread info, first digit should parse
                    // as thread id number, otherwise we dont care about that thread
                    // this will also include so called "dead" threads where "XXX" is used
                    // for id value (thank you CLR/SOS writers for that! :( )

                    LogString("strLine: " + strLine);

                    // stop when we encountered one with "exception" on it
                    if(strLine.ToLower().IndexOf("xxx") < 0 &&
                        strLine.ToLower().IndexOf("exception") >= 0) {
                        // split that line into pieces
                        string[] straLine = strLine.Split(new char[] {'(', ')'});
                                        
                        // we successed
                        if (straLine.Length > 1) {
                            // the last one would be our exception address
                            // (since we can have more than one ()'ed value
                            // we will always attmept to pick the very last one)
                            strAddress = straLine[straLine.Length - 2];

                            // in case of nested exceptions, our address will be
                            // the second from the end of the string
                            if(strAddress.ToLower().IndexOf("nested") >= 0) {
                                // adjusting accordingly
                                strAddress = straLine[straLine.Length - 4];
                            }
                               
                            // print that exception again
                            callStack = SendCommandWithOutput("!pe " 
                                + strAddress);

                            // some times exception object isnt working
                            // which would mean current thread has an unmanaged exception
                            // the following just prepares us for such cases
                            if(callStack.ToLower().IndexOf("not a valid exception object") >= 0) {
                                strAddress = string.Empty;
                            }
                            else {
                                bDone = true;
                            }
                        }
                    }
                }

                if(strAddress != string.Empty) {  
                    // if invalid object or nested
                    if(callStack.ToLower().IndexOf("invalid object") >= 0 ||
                        callStack.ToLower().IndexOf("-nested") >= 0) {

                        // try to get nested exception
                        callStack = SendCommandWithOutput("!pe -nested " 
                            + strAddress);
                    }
                    
                    string printExceptionTag = "!printexception";

                    while(callStack.ToLower().IndexOf(printExceptionTag) >= 0) {
                        // do so
                        int addressLength = 8;
                        if(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").IndexOf("64") >= 0) {
                            addressLength *= 2;
                        }
                        strAddress = callStack.Substring(
                            callStack.ToLower().IndexOf(printExceptionTag) + printExceptionTag.Length + 1, addressLength);
                        callStack = SendCommandWithOutput("!pe " 
                            + strAddress);
                    }

                    if (callStack != String.Empty) { 
                        callStack = callStack.Replace(callStack + "\n", "");

                        // NOTE: we must do this first, before we clip callstack data
                        // to only contain stack trace and nothing else

                        // fill out exception Type and Message to pass back to references
                        string tagExceptionType = "Exception type:";
                        string tagExceptionMessage = "Message:";

                        // get exception type (in case of multiple exception print outs
                        // we only consider the very last one hence LastIndexOf is used
                        int iCount = callStack.LastIndexOf(tagExceptionType);
                        if(iCount >= 0 && callStack.IndexOf("\n", iCount) - (iCount + tagExceptionType.Length + 1) >= 0) {
                            // fill out passed reference
                            exceptionInfo = callStack.Substring(iCount + tagExceptionType.Length + 1, 
                                callStack.IndexOf("\n", iCount) - (iCount + tagExceptionType.Length + 1));
                        }

                        // get exception message
                        iCount = callStack.LastIndexOf(tagExceptionMessage);
                        if(iCount >= 0 && callStack.IndexOf("\n", iCount) - (iCount + tagExceptionMessage.Length + 1) >= 0) {
                            // filled out passed reference
                            exceptionMessage = callStack.Substring(iCount + tagExceptionMessage.Length + 1, 
                                callStack.IndexOf("\n", iCount) - (iCount + tagExceptionMessage.Length + 1));
                        }
         
                        // now we will cut out a piece with call stack only
            
                        // specify markers that we will use
                        string tagFrom = "StackTrace (generated):";
                        string tagTo = "HResult:";

                        // calculate positions in test between the markers
                        int cutFrom = callStack.LastIndexOf(tagFrom);
                        int cutTo = callStack.LastIndexOf(tagTo) + 18;

                        LogString(cutFrom + " -> " + cutTo);

                        // now cut out what we want
                        if(cutFrom >= 0 && cutTo - cutFrom >= 0) {
                            callStack = callStack.Substring(cutFrom, cutTo - cutFrom);
                        }
                    }

                    // everything succeeded here, print to logs and exit
                    LogString("exceptionInfo : " + exceptionInfo);
                    LogString("exceptionMessage : " + exceptionMessage);

                    // buh-bye!
                    return true;
                }
            }
#endif
            }

            // in case if all of the above has failed - get default callstack           
            callStack = SendCommandWithOutput("k");

            if (callStack == "")
            {
                // we timed out - will come again later
                callStack = "timeout";
                return false;
            }

            // if this is attached debug break we will walk all threads
            // and see if we can find Debugger.Break or Debugger.Launch
            // which would indicate we have an assert on callstacl
            if (result == false)
            {
                if (callStack.ToLower().IndexOf("ntdll!dbguiremotebreakin") >= 0)
                {
                    // get list of all threads
                    callStack = SendCommandWithOutput("~");
                    // split into individual list items
                    string[] threads = callStack.Split(new char[] { '\r', '\n' });
                    // walk through each instance 
                    foreach (string thread in threads)
                    {
                        // get parts seperated by space char
                        string[] parts = thread.Split(new char[] { ' ', '\t' }, 5);

                        // only continue we got something to work with
                        if (parts.Length > 1)
                        {
                            // find thread Id
                            int partNumber = 0;

                            // skip any empty spaces
                            // FYI: clr2 has Split with a parameter that will
                            // do that for you!
                            // 
                            while (parts[partNumber] == "" ||
                                parts[partNumber] == "#" ||
                                parts[partNumber] == ".")
                            {
                                partNumber++;
                                continue;
                            }

                            // get thread Id
                            int threadId = 0;
                            try
                            {
                                threadId = int.Parse(parts[partNumber].ToString(), NumberStyles.Number);
                            }
                            catch
                            {
                                Console.WriteLine(parts[partNumber]);
                                continue;
                            }

                            // switch to a thread by its found Id
                            SendCommandWithOutput("~" + threadId + "s");

                            // get callstack of the switch to thread
                            callStack = SendCommandWithOutput("k");

                            // check to see if this is what we are looking for
                            if (callStack.ToLower().IndexOf("debugger.break") >= 0 ||
                                callStack.ToLower().IndexOf("debugger.launch") >= 0 ||
                                callStack.ToLower().IndexOf("mscorwks!systemnative::failfast") >= 0 ||
                                callStack.ToLower().IndexOf("assert") >= 0)
                            {

                                // found it! - break out of here and report this callstack

                                // make sure we always have lines
                                SendCommandWithOutput(".lines -e");

                                // get callstack and exit
                                callStack = SendCommandWithOutput("k");

                                // found what we want
                                result = true;

                                break;
                            }
                        }
                    }
                }
            }

            // in case this is a common unexpected error from mil
            // we are using newly added milx extension commands to retrieve better
            // callstack
            if (callStack.ToLower().IndexOf("milcore!milunexpectederror") >= 0)
            {
                callStack = DoMilx();
                result = true;
            }

            // do one more check before we give up into !analyze
            if (result == false)
            {
                // want to make sure we are not on a debugger.break thread already
                // in which case !analyze will be useless (pending fixes from debuggers team)
                if (callStack.ToLower().IndexOf("debugger.break") >= 0 ||
                    callStack.ToLower().IndexOf("debugger.launch") >= 0 ||
                    callStack.ToLower().IndexOf("mscorwks!systemnative::failfast") >= 0 ||
                    callStack.ToLower().IndexOf("assert") >= 0)
                {

                    // found it! - break out of here and report this callstack

                    // make sure we always have lines
                    SendCommandWithOutput(".lines -e");

                    // get callstack and exit
                    callStack = SendCommandWithOutput("k");

                    // found what we want
                    result = true;
                }
            }

            // havent found what we want?
            if (result == false)
            {
                // utilize !analyze to get failing thread
                // switch to that thread and get whatever callstack it got

                // switch to a thread by its found Id
                callStack = SendCommandWithOutput("!analyze -v -f");
                string oldCallStack = callStack;

                // get callstack from !analyze output

                // specify markers that we will use
                string tagStackTextFrom = "STACK_TEXT:";
                string tagStackTextTo = "FOLLOWUP_IP:";

                // calculate positions in test between the markers
                int cutFrom = callStack.LastIndexOf(tagStackTextFrom);
                int cutTo = callStack.LastIndexOf(tagStackTextTo) + tagStackTextFrom.Length;

                // now cut out what we want
                if (cutFrom >= 0 && cutTo - cutFrom >= 0)
                {
                    callStack = callStack.Substring(cutFrom, cutTo - cutFrom);
                }

                string tagStackCommand = "STACK_COMMAND:";
                string stackCommand = string.Empty;

                // get exception message
                int iCount = oldCallStack.LastIndexOf(tagStackCommand);
                if (iCount >= 0 && oldCallStack.IndexOf(";", iCount) - (iCount + tagStackCommand.Length + 1) >= 0)
                {
                    // filled out passed reference
                    stackCommand = oldCallStack.Substring(iCount + tagStackCommand.Length + 1,
                        oldCallStack.IndexOf(";", iCount) - (iCount + tagStackCommand.Length + 1));

                    // switch to a thread by its found Id
                    SendCommandWithOutput(stackCommand);
                }

                // get callstack of the switch to thread
                callStack = SendCommandWithOutput("k");
            }

            callStack = FixWrapping(callStack);

            // construct exception info/tag for the failure record
            // using output of .lastevent
            string lastEvent = SendCommandWithOutput(".lastevent");
            LogString("lastevent : " + lastEvent);

            if (lastEvent.ToLower().IndexOf("exit") >= 0)
            {
                callStack = "badf00d";
            }
            else
            {
                string[] straLine2 = lastEvent.Split(new char[] { ':', '\n' });
                // trim first space and up through \n
                try
                {
                    // need to walk all elements to cover cases with kd
                    for (int i = 0; i < straLine2.Length && exceptionInfo == string.Empty; i++)
                    {
                        exceptionInfo = straLine2[i].Trim().Substring(0, straLine2[i].Trim().IndexOf(")") + 1);
                    }
                }
                catch
                {
                    // cant parse - bad remote, so we let it go
                    callStack = "badf00d";
                    // premature termination because its a bad remote
                    return false;
                }
            }

            LogString("exceptionInfo : " + exceptionInfo);
            LogString("exceptionMessage : " + exceptionMessage);

            return true;
        }

        /// <summary>
        /// Create a full dump of the current session.
        /// </summary>
        /// <param name="filename">Name/location to write dump file to.</param>
        public void CreateFullDump(string filename)
        {
            SendCommandWithOutput(string.Format(".dump /ma {0}", filename));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strStack"></param>
        /// <param name="iPosition"></param>
        /// <returns></returns>
        bool ChangeContextRecordFromExceptionPointer(String strStack, int iPosition)
        {
            if (strStack != String.Empty)
            {
                // get the remaining line            
                strStack = strStack.Substring(strStack.IndexOf("=", iPosition) + 2);
                string address = strStack.Substring(0, strStack.IndexOfAny(BreakOnChars));

                string[] modules = new string[] { "mscorwks", "kernel32" };

                foreach (string module in modules)
                {
                    string dtOutput = SendCommandWithOutput("dt " +
                        module + "!_EXCEPTION_POINTERS " + address);

                    string[] lines = dtOutput.Split(new char[] { '\r', '\n' });

                    if (lines.Length > 1)
                    {
                        string[] columns = lines[1].Split(new char[] { ':' });

                        if (columns.Length > 1)
                        {
                            string address2 = columns[1].Substring(0, columns[1].LastIndexOf(" "));

                            if (address2.ToLower().IndexOf("(null)") < 0 && SendCommandWithOutput(".cxr " + address2).ToLower().IndexOf("error") < 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strThreadObj"></param>
        /// <param name="exceptionInfo"></param>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        private bool GetExceptionInformation(String strThreadObj, ref String exceptionInfo, ref String exceptionMessage)
        {
            bool bReturn = false;
            String[] straFrames;
            String[] straLine;
            String strAddress;
            String strTemp;
            String strStack;
            int iCount;
            String strExceptionInfo;

            strStack = SendCommandWithOutput("kb");
            if (strStack != String.Empty)
            {
                strStack = strStack.ToLower();
                straFrames = strStack.Split('\n');
                foreach (String strLine in straFrames)
                {
                    if (strLine.ToLower().IndexOf("msvcr80!_except_handler") > 0 ||
                        strLine.ToLower().IndexOf("verifier!_except_handler") > 0 ||
                        strLine.ToLower().IndexOf("vfbasics!_except_handler") > 0 ||
                        strLine.ToLower().IndexOf("kernel32!_except_handler") > 0 ||
                        strLine.ToLower().IndexOf("msvcrt!_except_handler") > 0 ||
                        strLine.ToLower().IndexOf("kernel32!_except_handler") > 0 ||
                        strLine.ToLower().IndexOf("mscorwks!_except_handler") > 0 ||
                        strLine.ToLower().IndexOf("ntdll!executehandler") > 0
                        )
                    {
                        straLine = strLine.Split(new char[] { ' ', ')' });
                        if (straLine.Length > 3)
                        {
                            strAddress = straLine[4];
                            strExceptionInfo = SendCommandWithOutput(".cxr "
                                + strAddress);

                            // if we got a bad exception context data, keep on looking!
                            if (strExceptionInfo.ToLower().IndexOf("??") >= 0)
                            {
                                continue;
                            }
                        }
                        // 
                        break;
                    }

                    if (strLine.ToLower().IndexOf("mscorwks!raisetheexception") > 0)
                    {
                        straLine = strLine.Split(' ');
                        if (straLine.Length > 3)
                        {
                            strAddress = straLine[2];
                            strExceptionInfo = SendCommandWithOutput("!do "
                                + strAddress);

                            bReturn = DumpExceptionInformation(strExceptionInfo,
                                ref exceptionInfo, ref exceptionMessage);
                        }
                        break;
                    }
                }
            }

            if ((bReturn == false) && (strThreadObj != String.Empty))
            {
                strTemp = SendCommandWithOutput("dt mscorwks!Thread " + strThreadObj);
                iCount = strTemp.IndexOf("m_LastThrownObjectHandle");
                if (iCount > 0)
                {
                    strTemp = strTemp.Substring(strTemp.IndexOf("0x", iCount));
                    strTemp = strTemp.Substring(0, strTemp.IndexOf("\n"));

                    if (strTemp.ToLower().IndexOf(" ") >= 0)
                    {
                        strTemp = strTemp.Substring(0, strTemp.ToLower().IndexOf(' '));
                    }
                    strExceptionInfo = SendCommandWithOutput("!do poi(" + strTemp.Trim() + ")");

                    bReturn = DumpExceptionInformation(strExceptionInfo, ref exceptionInfo,
                        ref exceptionMessage);
                }
            }

            return bReturn;
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="strExceptionInfo"></param>
        /// <param name="exceptionInfo"></param>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        private bool DumpExceptionInformation(String strExceptionInfo,
            ref String exceptionInfo, ref String exceptionMessage)
        {
            bool bReturn = false;
            int iCount;
            String strTemp;

            iCount = strExceptionInfo.IndexOf("_innerException");
            if (iCount > 0)
            {
                iCount -= 9;
                strTemp = strExceptionInfo.Substring(iCount, 8);

                while (strTemp.CompareTo("00000000") != 0)
                {
                    strExceptionInfo = SendCommandWithOutput("!do " + strTemp);

                    iCount = strExceptionInfo.IndexOf("_innerException");
                    if (iCount > 0)
                    {
                        iCount -= 9;
                        strTemp = strExceptionInfo.Substring(iCount, 8);
                    }
                    else
                        break;
                }

                iCount = strExceptionInfo.IndexOf("Name:");
                if (iCount >= 0)
                {
                    exceptionInfo = strExceptionInfo.Substring(iCount + 6, strExceptionInfo.IndexOf("\n", iCount) - (iCount + 6));
                }
                else
                    return bReturn;

                iCount = strExceptionInfo.IndexOf("_message");
                if (iCount > 0)
                {
                    iCount -= 9;
                    strTemp = strExceptionInfo.Substring(iCount, 8);

                    strExceptionInfo = SendCommandWithOutput("!do " + strTemp);
                    if (strExceptionInfo != String.Empty)
                    {
                        iCount = strExceptionInfo.IndexOf("String:");
                        if (iCount >= 0)
                        {
                            try
                            {
                                exceptionMessage = strExceptionInfo.Substring(iCount + 8, strExceptionInfo.IndexOf("\n", iCount) - (iCount + 8));
                            }
                            catch (System.ArgumentOutOfRangeException exception)
                            {
                                LogString(FormatttedExceptionMessage.GetMessage(exception));
                                exceptionMessage = "<unavailable>";
                            }
                        }
                    }
                }
                bReturn = true;
            }
            return bReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strtofix"></param>
        /// <returns></returns>
        private string FixWrapping(String strtofix)
        {
            string[] strtokens = strtofix.Split(new char[] { '\n' });
            try
            {
                const string hexdigit = "(\\d|a|b|c|d|e|f)";
                string hexword = hexdigit + hexdigit + hexdigit + hexdigit;
                string hexword64 = hexdigit + hexdigit + hexdigit + hexdigit + "`" + hexdigit + hexdigit + hexdigit + hexdigit;
                hexword += hexword;
                Regex rFuncCall = new Regex(hexword + "\\s" + hexword);
                Regex rFuncCall64 = new Regex(hexword64 + "\\s" + hexword64);
                Regex rHeaders = new Regex(@"[milx]|Child|Module|" + hexdigit + ":" + hexdigit + "\\S*>"); // ??? what the ---- is this do?
                int isize = strtokens.Length;
                string strtemp;
                for (int i = 0; i < isize; i++)
                {
                    if (rFuncCall.IsMatch(strtokens[i]) || rFuncCall64.IsMatch(strtokens[i]) || rHeaders.IsMatch(strtokens[i]))
                    {
                        strtokens[i] = strtokens[i] + "\n";
                    }
                    else if ((strtemp = strtokens[i].Trim()).Length > 7)
                    {
                        if (strtemp.ToLower().Substring(0, 5) == "error" || strtokens[i].ToLower().Trim().Substring(0, 7) == "warning" || strtokens[i].Trim().IndexOf('*') >= 0)
                            strtokens[i] = String.Empty;
                    }
                }
            }
            catch (Exception exception)
            {
                LogString(FormatttedExceptionMessage.GetMessage(exception));
            }
            finally
            {
                strtofix = String.Join("", strtokens);
            }
            return strtofix;
        }

        /// <summary>
        /// Uses the milx.dll extension to debug a case where there is
        /// a MilUnexpectedError frame on the stack.
        /// </summary>
        /// <returns>
        /// The callstack for the error, an empty string if unable to run
        /// the proper analysis.
        /// </returns>
        private string DoMilx()
        {
            string callStack = string.Empty;

            // need to figure out what platform we are on
            string vertarget = SendCommandWithOutput("vertarget");
            string[] lines = vertarget.Split('\n');

            // default to x86
            string platform = "x86";

            // make the choice
            if (lines.Length > 0)
            {
                if (lines[0].ToLower().IndexOf("x64") >= 0)
                {
                    platform = "amd64";
                }
                if (lines[0].ToLower().IndexOf("ia64") >= 0)
                {
                    platform = "ia64";
                }
            }

            // define search path where to load milx extension from
            string milxPath = @"\\hydrogen\builds$\VBL_WCP_AVALON_DEV_STRESS\" + platform + @"fre\latest\dbg\files\bin\pri\milx.dll";

            // lets do this thing
            string milxResponse = SendCommandWithOutput("!load " + milxPath);

            // if no errors - let the magic happen!!
            if (milxResponse.IndexOf("error") < 0)
            {
                // Get the output of milx

                // Need to force reloading of symbols for what !dumpcaptures deals with.
                SendCommandWithOutput(".reload /f milcore.dll");
                SendCommandWithOutput(".reload /f windowscodecs.dll");

                callStack = SendCommandWithOutput("!dumpcaptures");

                if (callStack.ToLower().IndexOf("captured stack associated with the selected thread is empty.") >= 0)
                {
                    callStack = SendCommandWithOutput("!dumpcaptures -m milcore");
                }

                /*
                // spilit by lines
                lines = milxResponse.Split(new char[] {'\r', '\n'});

                callStack = string.Empty;

                // parse through
                for(int i = 0; i < lines.Length; i++) {
                    // consider everything but comments for now
                    // TODO: comments must go to the Comments LorTag filed
                    if(lines[i].ToLower().IndexOf("[milx]") < 0) {
                        callStack += lines[i] + @"\r\n";
                    }
                }
                */
            }

            return callStack;
        }

        /// <summary>
        /// Load managed debuggin extensions.
        /// </summary>
        private void LoadManagedModules()
        {
            //load SOS managed extension dll
            //SendCommandWithOutput(".load msvcr80");
            SendCommand(".loadby sos mscorwks");
            SendCommand(".cxr");

            // test to make sure that cordll worked, if not - we got bad symbols/sos combo
            // otherwise continue "..as planned."
            SendCommand(".cordll -u -l");
        }

        /// <summary>
        /// Determine if the current session is managed or unmanaged.
        /// </summary>
        /// <returns>True if managed.</returns>
        private bool GetIsManagedSession()
        {
            // check if mscorwks is present - that will identify this to be a managed crash
            return SendCommandWithOutput("lm mmscorwks").ToLower().IndexOf("mscorwks") >= 0;
        }

        /// <summary>
        /// Configure symbols for current session.
        /// </summary>
        private void SetupSymbols()
        {
            SendCommand(".symfix");

            // only want to utilize findthebuild service on vista+ machines
            string verTargetLower = SendCommandWithOutput("vertarget").ToLowerInvariant();
            if (!verTargetLower.Contains("windows xp") && !verTargetLower.Contains("windows server"))
            {
                SendCommand("!findthebuild -s");
            }

            string symbolServers = ""; // TODO: lor - LorDbg.SQLHelper.GetConfigValue("SymbolServer");
            if (string.IsNullOrEmpty(symbolServers) == true)
            {
                symbolServers = @"SRV*\\symbols\symbols;SRV*\\urtdist\builds\symbols";
            }
            string nt_symbol_path = Environment.GetEnvironmentVariable("_NT_SYMBOL_PATH");
            if (string.IsNullOrEmpty(nt_symbol_path) == true)
            {
                nt_symbol_path = symbolServers;
            }
            else
            {
                nt_symbol_path += ";" + symbolServers;
            }
            string retString = SendCommandWithOutput(@".sympath+ " + nt_symbol_path);
            if (retString.ToLower().IndexOf("the debugger does not have a current process or thread") >= 0)
            {
                //callStack = "badf00d";
                // premature termination because its a bad remote
                throw new ApplicationException("Bad remote: badf00d");
            }

            EnsureSymbols();
        }

        /// <summary>
        /// Check to make sure that symbols are properly configured.
        /// </summary>
        /// <returns>True if symbols are loaded, false otherwise.</returns>
        private void EnsureSymbols()
        {
            int tryCount = 2;

            // check to see if we got working symbols
            while (
                (
                    SendCommandWithOutput("lm mmscorwks").ToLower().IndexOf("mscorwks") >= 0
                    &&
                    SendCommandWithOutput(".reload /f mscorwks.dll").ToLower().IndexOf("defaulted to export symbols") >= 0
                )
                ||
                (
                    SendCommandWithOutput("lm mmilcore").ToLower().IndexOf("milcore") >= 0
                    &&
                    SendCommandWithOutput(".reload /f milcore.dll").ToLower().IndexOf("defaulted to export symbols") >= 0
                )
                ||
                (
                    SendCommandWithOutput("lm mwindosbase*").ToLower().IndexOf("windowsbase") >= 0
                    &&
                    SendCommandWithOutput(".reload /f windowsbase*").ToLower().IndexOf("defaulted to export symbols") >= 0)
                )
            {
                if (--tryCount == 0)
                {
                    if (IsDumpFile)
                    {
                        // Add some additional information to help interactive debuggers
                        // triage symbols.
                        SendCommandWithOutput("* Unable to resolve symbols.");
                        SendCommandWithOutput("!sym noisy");
                        SendCommandWithOutput(".sympath");
                        SendCommandWithOutput(".reload /f milcore.dll");
                    }
                    throw new ApplicationException("Unable to resolve symbols.");
                }

                string symbolsPath = SymbolsPath;

                if (symbolsPath != string.Empty)
                {
                    string symbolsPathAvalon = symbolsPath + @"\Symbols.pri\retail\dll;" + symbolsPath +
                        @"\Symbols.pri\retail\exe";
                    SendCommandWithOutput(@".sympath+ " + symbolsPathAvalon);

                    string symbolsPathTests = symbolsPath + @"\Symbols.pri\NTTEST\WINDOWSTEST\Client\dll;" + symbolsPath +
                        @"\Symbols.pri\NTTEST\WINDOWSTEST\Client\exe";
                    SendCommandWithOutput(@".sympath+ " + symbolsPathTests);
                }
            }
        }

        /// <summary>
        /// Determine the Id of the processes that the current kernel session is attached to.
        /// </summary>
        /// <returns>Process id or -1 id cannot be found.</returns>
        private int FindProcessIdFromKernel()
        {
            String strProcessInfo;
            String strLine;
            String[] strLineArray;
            bool bNext = false;

            SendCommandWithOutput(".reload");
            if (RequestType == DebugRequestType.Kernel)
            {
                strProcessInfo = SendCommandWithOutput("!Process");
                if (strProcessInfo == String.Empty || strProcessInfo.ToLower().IndexOf("nt symbols are incorrect, please fix symbols") >= 0)
                {
                    SendCommand("* **** Symbols cannot be properly resolved on target machine; you will need to report this crash manually. Consider updating/reinstalling OS on your target machine.");
                    return -1;
                }

                strLine = strProcessInfo.Substring(0, strProcessInfo.IndexOf('\n'));
                strLineArray = strLine.Split(' ');
                foreach (String strTok in strLineArray)
                {
                    if (bNext == true)
                    {
                        return int.Parse(strTok, System.Globalization.NumberStyles.HexNumber);
                    }

                    if (strTok.StartsWith("Cid") == true)
                    {
                        bNext = true;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Determine if the current session is a Kernel debug session.
        /// </summary>
        private bool GetIsKernelDebugSession()
        {
            string kdPrompt = string.Empty;

            // make sure we always first disable lines while looking for stuff
            kdPrompt = SendCommandWithOutput(".lines -d");
            if (kdPrompt.ToLower().IndexOf("kd>") >= 0)
            {
                return true;
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    /// Formats exception messages for stress logging.
    /// </summary>
    internal sealed class FormatttedExceptionMessage
    {
        // disallow this class construction
        private FormatttedExceptionMessage()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Formats the contents of clipboard as an error report.
        /// </summary>
        /// <returns>StringBuilder</returns>
        public static String GetMessage(Exception exception)
        {
            try
            {
                if (exception == null)
                {
                    return String.Empty;
                }

                StringBuilder strBuilder = new StringBuilder();
                int levelCount = 0;
                string innerException = String.Empty;
                string msg = String.Empty;

                msg = exception.Message;

                if (exception.InnerException != null)
                {
                    innerException = exception.InnerException.ToString();
                }

                strBuilder.AppendFormat("ERROR REPORT (Exception levels including inner exceptions. Level 0 denotes outermost exception){1}{1}------------START OF ERROR REPORT------------{1}{1}Level            : {0}{1}Error Message    : {2}{1}Source           : {3}{1}Inner Exception  : {4}{1}Call Stack       : {5}{1}Trace            : {1}{6}{1}",
                    levelCount.ToString(),
                    Environment.NewLine,
                    msg,
                    exception.TargetSite.ToString(),
                    innerException,
                    exception.StackTrace, "");

                while (exception.InnerException != null)
                {
                    if (exception.InnerException.InnerException != null)
                    {
                        innerException = exception.InnerException.InnerException.ToString();
                    }
                    else
                    {
                        innerException = String.Empty;
                    }

                    levelCount++;
                    strBuilder.AppendFormat("Level            : {0}{1}Error Message    : {2}{1}Source           : {3}{1}Inner Exception  : {4}{1}{1}",
                        levelCount.ToString(),
                        Environment.NewLine,
                        exception.InnerException.Message,
                        exception.InnerException.TargetSite.ToString(),
                        innerException);

                    exception = exception.InnerException;

                }
                strBuilder.AppendFormat("--------------END OF ERROR REPORT------------");
                return (strBuilder.ToString());
            }
            catch (Exception excp)
            {
                return "Error in formatting exception message. Message: " + excp.Message;
            }
        }
    }
}
