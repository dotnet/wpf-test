// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;
using System.IO.Compression;

namespace Microsoft.Test.Diagnostics
{
    /// <summary>
    /// Contains all the information about an exception that is extracted
    /// from a cdb session.
    /// </summary>
    public class ExceptionInfo
    {
        #region Private Fields

        private string type;
        private string message;
        private string stackTrace;
        private ExceptionInfo innerException;

        #endregion

        #region Public Properties

        /// <summary>
        /// Exception.Type
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        /// <summary>
        /// Exception.Message
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        /// <summary>
        /// Exception.StackTrace
        /// </summary>
        public string StackTrace
        {
            get { return stackTrace; }
            set { stackTrace = value; }
        }
        /// <summary>
        /// Exception.InnerException
        /// </summary>
        public ExceptionInfo InnerException
        {
            get { return innerException; }
            set { innerException = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Custom formatting for when this is printed.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("Type: " + Type);
            output.AppendLine("Message: " + Message);
            output.AppendLine("StackTrace: ");
            output.AppendLine(StackTrace);
            if (InnerException != null)
            {
                output.AppendLine("InnerException:");
                output.AppendLine(new string('=', 50));
                output.AppendLine(InnerException.ToString());
                output.AppendLine(new string('=', 50));
            }
            return output.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Wrapper for creating and interacting with a CDB session.
    /// </summary>
    public class CdbSession : IDisposable
    {
        #region Private Members

        static string cdbInstallLocation = @"%SYSTEMDRIVE%\debuggers\cdb.exe";

        private Process targetProcess;
        private CdbCommunicationProxy cdbProxy;

        private bool isInitialized = false;
        private bool? isManagedSession = null;
        private IDebugStrategy debugStrategy;
        private string symbolsPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Attach a cdb debugger to process with given id.
        /// </summary>
        /// <param name="pid">Process id to attach to.</param>
        public CdbSession(int pid)
            : this(Process.GetProcessById(pid))
        {
            
        }

        /// <summary>
        /// Attach a cdb debugger to the given process.
        /// </summary>
        /// <param name="process">Process to debug.</param>
        public CdbSession(Process process) 
        {
            if (process == null)
                throw new ArgumentNullException("process");
            if (process.HasExited)
                throw new ApplicationException(string.Format("Cannot attach to process '{0}' is has already exited.", process.Id));

            targetProcess = process;
            cdbProxy = AttachDebugger(process);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// End the cdb session.
        /// </summary>
        /// <returns>Full text of session.</returns>
        public string Close()
        {
            // Exit cdb session.
            cdbProxy.SendCommand("q");
            return cdbProxy.GetFullLog();
        }

        /// <summary>
        /// Extract information about the exception that caused the crash.
        /// </summary>
        /// <returns></returns>
        public ExceptionInfo GetExceptionInfo()
        {
            EnsureInitialized();
            return debugStrategy.GetExceptionInfo();
        }

        /// <summary>
        /// Create a full dump of the current crash to a unique filename.
        /// </summary>
        /// <returns>Name of dmp file.</returns>
        public string CreateDump()
        {
            string fullDmp = string.Format("{0}_{1}.FULL.dmp.gz", Process.ProcessName, Process.Id);
            // Make dmp file path absolute so we always know where it went...
            string cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            fullDmp = Path.Combine(cwd, fullDmp);
            CreateDump(fullDmp);
            return fullDmp;
        }

        /// <summary>
        /// Create a full dump of the current crash.
        /// </summary>
        /// <param name="outputFilename"></param>
        public void CreateDump(string outputFilename)
        {
            string memoryDumpName = Path.Combine(Path.GetDirectoryName(outputFilename), Path.GetFileNameWithoutExtension(outputFilename));

            //create memory dump.
            cdbProxy.SendCommand(string.Format(".dump /ma {0}", memoryDumpName));

            //read from memery dump.
            byte[] buffer = null;
            using(FileStream dumpStream = File.OpenRead(memoryDumpName))
            {
                buffer = new byte[dumpStream.Length];
                dumpStream.Read(buffer, 0, buffer.Length);
            }
            using(FileStream outStream = File.Create(outputFilename))
            {
                using (GZipStream gZipStream = new GZipStream(outStream, CompressionMode.Compress))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }
            }

            //delete memory dump.
            File.Delete(memoryDumpName);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Process that CdbSession is attached to.
        /// </summary>
        public Process Process
        {
            get { return targetProcess; }
        }

        /// <summary>
        /// ';' delimited list of paths to search for symbols.
        /// </summary>
        /// <remarks>
        /// These paths will be appended to _NT_SYMBOLS_PATH envirnoment variable.
        /// </remarks>
        public string SymbolsPath
        {
            get { return symbolsPath; }
            set { symbolsPath = value; }
        }

        /// <summary>
        /// True if the current session is managed.
        /// </summary>
        /// <returns>True if managed.</returns>
        public bool IsManagedSession
        {
            get
            {
                if (isManagedSession == null)
                {
                    // check if mscorwks is present - that will identify this to be a managed crash
                    isManagedSession = cdbProxy.SendCommand("lm mmscorwks").ToLower().IndexOf("mscorwks") >= 0;
                }
                return (bool)isManagedSession;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Verify that symbols have been loaded and session is ready to be interactive.
        /// </summary>
        private void EnsureInitialized()
        {
            if (!isInitialized)
            {
                string fullSymbolPath = string.Empty;

                // Append explicitly defined symbol path if provided..
                if (!string.IsNullOrEmpty(SymbolsPath))
                {
                    fullSymbolPath += SymbolsPath + ";";
                }

                // Append _NT_SYMBOL_PATH environment variable if set.
                string nt_symbol_path = Environment.GetEnvironmentVariable("_NT_SYMBOL_PATH");
                if (!string.IsNullOrEmpty(nt_symbol_path))
                {
                    fullSymbolPath += nt_symbol_path + ";";
                }

                LoadSymbols(fullSymbolPath);

                if (IsManagedSession)
                {
                    debugStrategy = new ManagedDebugStrategy(cdbProxy);
                }
                else
                {
                    throw new NotImplementedException("Haven't implemented debugging support for native crashes yet...");
                }
            }
        }

        /// <summary>
        /// Load symbols for the current process.
        /// </summary>
        /// <param name="fullSymbolPath"></param>
        private void LoadSymbols(string fullSymbolPath)
        {
            // .symfix automatically restores symbol path to \\symbols\symbols.
            cdbProxy.SendCommand(".symfix");
            string retString = cdbProxy.SendCommand(@".sympath+ " + fullSymbolPath);
            if (retString.ToLower().IndexOf("the debugger does not have a current process or thread") >= 0)
            {
                //callStack = "badf00d";
                // premature termination because its a bad remote
                throw new ApplicationException("Bad remote: badf00d");
            }
            cdbProxy.SendCommand(".reload");

            // 


        }

        /// <summary>
        /// Start an cdb process that will attach to the given process..
        /// </summary>
        /// <param name="process">Process to attach debugger to.</param>
        /// <returns>Active cdb process.</returns>
        /// <exception cref="ApplicationException">Process fails to start or ExecutionService is not running.</exception>
        private static CdbCommunicationProxy AttachDebugger(Process process)
        {
            EnsureDebuggers();

            ExecutionServiceClient executionService = ExecutionServiceClient.Connect();
            if (executionService == null)
                throw new ApplicationException("ExecutionService not found.");

            string systemDrive = Environment.ExpandEnvironmentVariables("%SYSTEMDRIVE%");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = string.Format("{0}\\debuggers\\cdb.exe", systemDrive);
            startInfo.Arguments = string.Format("-p {0}", process.Id);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;

            if (!File.Exists(startInfo.FileName))
            {
                throw new InvalidOperationException(string.Format("Cannot debug failure, debuggers are not installed on this machine.  Please install debuggers from http://dbg to '{0}'.", startInfo.FileName));
            }

            // Launch CDB as an elevated process so that it has permissions to attach.
            Process session = executionService.StartProcess(
                startInfo, 
                ExecutionUserContext.FirstActiveSessionUser, 
                ExecutionPrivileges.Administrator, 
                true);
            if (session.HasExited)
            {
                throw new ApplicationException(string.Format("Remote debugging session failed to startup: Commandline='{0} {1}', exit code='{2}'.", startInfo.FileName, startInfo.Arguments, session.ExitCode));
            }
            
            session.PriorityClass = ProcessPriorityClass.High;
            return new CdbCommunicationProxy(session);
        }

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

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Close session.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion
    }

    /// <summary>
    /// Module which handles sending messages to CDB and parsing its responses.
    /// </summary>
    class CdbCommunicationProxy
    {
        #region Private Fields

        StringBuilder fullCdbLog = new StringBuilder();
        private Process target;

        #endregion

        #region Constructor

        public CdbCommunicationProxy(Process target)
        {
            this.target = target;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Send command to the remote session and and return the output text.
        /// </summary>
        /// <remarks>Inserts start/end echo tags around the given cmd and then
        /// parses the standard output of the process to find the output of
        /// the given cmd.</remarks>
        /// <param name="cmd">Command to execute</param>
        /// <returns>cdb output associated with given command.</returns>
        public string SendCommand(string cmd)
        {
            string transactionId = Guid.NewGuid().ToString();
            string cmdStart = string.Format("<START: {0}>", transactionId);
            string cmdDone = string.Format("<END: {0}>", transactionId);
            target.StandardInput.WriteLine(".echo " + cmdStart);
            DoSendCommand(cmd);
            target.StandardInput.WriteLine(".echo " + cmdDone);
            // Read all output up until the command we just executed.
            ReadOutputTo(cmdStart);
            // Read all output associated with the given command.
            return ReadOutputTo(cmdDone);
        }

        /// <summary>
        /// Returns the full output log of this session.
        /// </summary>
        /// <returns></returns>
        public string GetFullLog()
        {
            return fullCdbLog.ToString();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get Process that corresponds to the cdb instance being wrapped.
        /// </summary>
        public Process Process
        {
            get { return target; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Send a command to remote session but don't retrieve its output.
        /// </summary>
        /// <remarks>Removes any perf overhead around parsing the standard output
        /// of the cdb session if we don't really care about the spew from an individual
        /// command.</remarks>
        /// <param name="cmd">Command to execute.</param>
        private void DoSendCommand(string cmd)
        {
            target.StandardInput.WriteLine(".echo " + cmd); // Make all calls appear in log.
            target.StandardInput.WriteLine(cmd);
        }

        /// <summary>
        /// Read StandardOutput of remote session until a line with the
        /// given tag appears.  
        /// </summary>
        /// <param name="tag">Text to look for in standard out.</param>
        /// <returns>Return all text NOT including the line with the specified tag.</returns>
        private string ReadOutputTo(string tag)
        {
            StringBuilder result = new StringBuilder();
            string line;
            while (!target.StandardOutput.EndOfStream && !(line = target.StandardOutput.ReadLine()).Contains(tag))
            {
                CheckForErrors(line);
                fullCdbLog.AppendLine(line);
                result.AppendLine(line);
            }
            return result.ToString();
        }

        /// <summary>
        /// Parse a single line of CDB spew looking for error codes.
        /// </summary>
        /// <param name="line">Single line of CDB output.</param>
        /// <exception cref="ApplicationException">If error code is detected.</exception>
        private void CheckForErrors(string line)
        {
            if (line.Contains("REMOTE error"))
            {
                throw new ApplicationException("Error occurred while communicating with remote cdb session: " + line);
            }
        }

        #endregion
    }

    /// <summary>
    /// Instead of having all debugging logic for various types of sessions, we'll encapsulate this
    /// logic into a specilized module.  We'll at least of a Managed and Native implementation, perhaps
    /// more for different teams that have specialized requirements.
    /// </summary>
    interface IDebugStrategy
    {
        /// <summary>
        /// Retrieve the Exception information from the current session.
        /// </summary>
        /// <returns></returns>
        ExceptionInfo GetExceptionInfo();
    }

     /// <summary>
    /// Logic for debugging managed crashes.
    /// </summary>
    class ManagedDebugStrategy : IDebugStrategy
    {
        #region Private Fields

        CdbCommunicationProxy session;

        #endregion

        #region Constructor

        public ManagedDebugStrategy(CdbCommunicationProxy cdbProxy)
        {
            session = cdbProxy;

            // Load SOS extensions. For CLR40, there is no mscorwks.dll, SOS is installed
            // in the same directory as CLR.exe. 
#if TESTBUILD_CLR40
            string result = session.SendCommand(".loadby sos clr");
#else
            string result = session.SendCommand(".loadby sos mscorwks");
#endif
            if (result.ToLowerInvariant().Contains("unable to find module"))
            {
                throw new ApplicationException("Failed to load 'SOS'.");
            }
            session.SendCommand(".cxr");
        }

        #endregion

        #region IDebugStrategy Members

        /// <summary>
        /// Extract the Exception info from the current session.
        /// </summary>
        /// <returns></returns>
        public ExceptionInfo GetExceptionInfo()
        {
            ThreadInfo threadInfo = FindThreadWithException();
            ExceptionInfo exceptionInfo = GetExceptionInfo(threadInfo.ExceptionAddress);
            exceptionInfo.StackTrace += "\n\nCLRSTACK:\n" + GetClrStack(threadInfo.ThreadNum);
            return exceptionInfo;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method uses !clrstack to find the stackptr of the thread which the exception occurred on.
        /// </summary>
        /// <remarks>
        /// The actual commands to execute will be:
        /// ~Ns         # switch to thread number N
        /// !clrstack   # dump clr stack for the current thread.
        /// 
        /// output of !clrstack looks like:
        /// 
        /// OS Thread Id: 0x1070 (0)
        /// ESP       EIP
        /// 0012e610 77060f34 [HelperMethodFrame: 0012e610]
        /// 0012e6b4 0b001782 Microsoft.LiveLabs.Test.Framework.Assert.Fail(System.String)
        /// ...
        /// </remarks>
        /// <param name="threadNum">Number of the thread we are interested in.</param>
        /// <param name="topStackPtr">esp value of the top frame of the clr stack.</param>
        /// <returns>Full Clr stack for the current thread.</returns>
        private string GetClrStack(int threadNum, out string topStackPtr)
        {
            session.SendCommand(string.Format("~{0}s", threadNum)); // switch context to thread.
            string output = session.SendCommand("!clrstack");       // dump managed stack.
            string[] lines = output.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            if (lines.Length < 2)
            {
                throw new ApplicationException("!clrstack returned no lines, debugger state is unknown: " + output);
            }

            topStackPtr = null;
            StringBuilder stack = new StringBuilder();
            Regex framePattern = new Regex(@"(?<esp>[a-f|0-9]{8,16}) (?<eip>[a-f|0-9]{8,16}) (?<frame>.*)");
            Match match;
            foreach (string line in lines)
            {
                if ((match = framePattern.Match(line)).Success)
                {
                    string frame = match.Groups["frame"].Value;
                    stack.AppendLine(frame);

                    // Don't get the stack point of the first element blindly because it
                    // may be a [HelperMethodFrame] or something which won't yield the 
                    // correct user stack.
                    if (string.IsNullOrEmpty(topStackPtr) && !frame.StartsWith("["))
                    {
                        topStackPtr = match.Groups["esp"].Value;
                    }
                }
            }

            if (string.IsNullOrEmpty(topStackPtr))
            {
                throw new ApplicationException("Failed to locate StackPtr for thread " + threadNum);
            }
            return stack.ToString();
        }
        private string GetClrStack(int threadNum)
        {
            string stackPtr;
            return GetClrStack(threadNum, out stackPtr);
        }

        /// <summary>
        /// Parses the output of !pe [address].
        /// </summary>
        /// <example>
        /// Output of !pe looks like:
        /// 
        /// Exception object: 0187b9c8
        /// Exception type: System.ArgumentNullException
        /// Message: Value cannot be null.
        /// InnerException: none
        /// StackTrace (generated):
        /// SP       IP       Function
        /// ...
        /// </example>
        /// <param name="address"></param>
        /// <returns></returns>
        private ExceptionInfo GetExceptionInfo(string exceptionAddress)
        {
            ExceptionInfo exceptionInfo = new ExceptionInfo();

            // Note: Fixed Type/Message Patterns:  These were brittle based off a hard coded 
            // # of spaces after the first word.  " +" means "any number of spaces" and \b means "on a word boundary".
            // This fixes many common ExecHarnessDebugger failures, and should thus reduce the number of dumps in a given test run.
            Regex typePattern = new Regex(@"exception type: +\b(?<type>.+)\b", RegexOptions.IgnoreCase);
            Regex messagePattern = new Regex(@"message: +\b(?<message>.+)\b", RegexOptions.IgnoreCase); 
            Regex innerExceptionPattern = new Regex(@"innerexception:.*?printexception (?<address>[a-f|0-9]{8,16})", RegexOptions.IgnoreCase);

            // If !pe isn't given an address it dumps the highest level exception on the current thread.
            string[] lines = session.SendCommand("!pe " + exceptionAddress).Split(new string[] { "\r\n" }, StringSplitOptions.None);
            if (lines.Length > 1 && lines[1].Contains("The current thread is unmanaged"))
            {
                throw new ApplicationException("Cannot execute '!pe' command, current thread is unmanaged.");
            }
            
            Match match;
            StringBuilder stacktrace = null;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                // If we have already reached the StackTrace portion of the output,
                // just blindly append it to the existing trace.
                if (stacktrace != null)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        // Once we reach the blank line after the stacktrace
                        // there isn't any more interesting content so we can stop.
                        break;
                    }
                    else
                    {
                        stacktrace.AppendLine(line);
                    }
                }
                else
                {
                    // Type.
                    if ((match = typePattern.Match(line)).Success)
                    {
                        exceptionInfo.Type = match.Groups["type"].Value;
                    }
                    // Message.
                    else if ((match = messagePattern.Match(line)).Success)
                    {
                        exceptionInfo.Message = match.Groups["message"].Value;
                    }
                    // InnerException.
                    else if ((match = innerExceptionPattern.Match(line)).Success)
                    {
                        string innerAddress = match.Groups["address"].Value;
                        if (!string.IsNullOrEmpty(innerAddress) && 
                            !innerAddress.Contains("none"))
                        {
                            // Recurse through inner exceptions.
                            exceptionInfo.InnerException = GetExceptionInfo(innerAddress);
                        }
                    }
                    // Beginning of StackTrace.
                    else if (line.ToLowerInvariant().StartsWith("stacktrace"))
                    {
                        stacktrace = new StringBuilder();
                    }
                }
            }

            if (string.IsNullOrEmpty(exceptionInfo.Type))
                throw new ApplicationException("Failed to determine Exception Type.");

            if (stacktrace != null)
            {
                exceptionInfo.StackTrace = stacktrace.ToString();
            }
            return exceptionInfo;
        }

        /// <summary>
        /// Parse the output of !Threads to determine what the address of the Exception object is.
        /// </summary>
        /// <returns>Information about the thread which exception was thrown on.</returns>
        private ThreadInfo FindThreadWithException()
        {
            // !Threads produces lines like:
            //        ID OSID ThreadOBJ    State     GC       Context       Domain   Count APT Exception
            //    0    1 271c 000afcd8      6020 Enabled  0187c124:0187dfe8 00077518     0 STA System.ArgumentNullException (0187b9c8)
            Regex threadPattern = new Regex(
                    string.Join(@"\s*", new string[] {
                            @"(?<threadnum>\d+)",
                            @"(?<id>\d+)",
                            @"(?<osid>\S+)",
                            @"(?<threadobj>\S+)",
                            @"(?<state>\S+)",
                            @"(?<gc>\S+)",
                            @"(?<context>\S+)",
                            @"(?<domain>\S+)",
                            @"(?<count>\S+)",
                            @"(?<apt>\S+)",
                            @"(?<exception>.*)",
                        }
                ));
            // The exception field will be of the format: System.ArgumentNullException (0187b9c8)
            Regex exceptionPattern = new Regex(@"(?<type>.*?exception\S*)\s*\((?<address>.*?)\)", RegexOptions.IgnoreCase);

            Match match;
            ThreadInfo threadInfo = new ThreadInfo();
            string[] threads = session.SendCommand("!threads").Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string thread in threads)
            {
                // If line is a thread definition.
                if ((match = threadPattern.Match(thread)).Success)
                {
                    string threadId = match.Groups["id"].Value;
                    string exceptionField = match.Groups["exception"].Value;
                    Match innerMatch;
                    if ((innerMatch = exceptionPattern.Match(exceptionField)).Success)
                    {
                        threadInfo.ThreadNum = int.Parse(match.Groups["threadnum"].Value);
                        threadInfo.ExceptionType = innerMatch.Groups["type"].Value;
                        threadInfo.ExceptionAddress = innerMatch.Groups["address"].Value;
                        break; // We found the exception address.
                    }
                }
            }

            if (string.IsNullOrEmpty(threadInfo.ExceptionAddress))
            {
                throw new ApplicationException("Failed to locate Exception address by parsing threads.");
            }

            return threadInfo;
        }

        #endregion

        /// <summary>
        /// Datastructure which contains data of interest about threads in a session.
        /// </summary>
        struct ThreadInfo
        {
            // Thread number to exception occurred on.
            public int ThreadNum;
            // Type of exception.
            public string ExceptionType;
            // Address of exception that can be passed to !pe.
            public string ExceptionAddress;
        }
    }
}
