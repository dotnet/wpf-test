// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides logging services for test cases.

#region Namespaces.

using Microsoft.Test.Loaders;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Win32 = Test.Uis.Wrappers.Win32;
using Test.Uis.Data;
using Test.Uis.Utils;
using Test.Uis.Management;

#endregion Namespaces.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Loggers/Loggers.cs $")]

namespace Test.Uis.Loggers
{
    #region Dump configuration.

    /// <summary>Kinds of information that can be reported while dumping an exception.</summary>
    [Flags]
    public enum ExceptionDumpKinds
    {
        /// <summary>Dumps the exception itself.</summary>
        ExceptionDump = 0x0001,
        /// <summary>Dumps a stack trace (usu. included in exception too).</summary>
        TraceDump = 0x0002,
        /// <summary>Dumps loaded assembly names.</summary>
        AssemblyDump = 0x004,
        /// <summary>Dumps details about loaded assemblies.</summary>
        DetailedAssemblyDump = 0x008,
        /// <summary>Dumps information about the current application domain.</summary>
        AppDomainDump = 0x010,
        /// <summary>Dumps information about the thread queue.</summary>
        ThreadQueueDump = 0x020,
        /// <summary>Dumps all exceptions (instead of innermost).</summary>
        ChainedExceptions = 0x040,
        /// <summary>Dumps information about the Avalon queues.</summary>
        AvalonQueueDump = 0x080,
        /// <summary>Dumps information about the current input locale.</summary>
        InputLocaleDump = 0x100,
        /// <summary>Dumps all kinds of information.</summary>
        All =
            ExceptionDump | DetailedAssemblyDump | AppDomainDump |
            ThreadQueueDump | ChainedExceptions | AvalonQueueDump |
            InputLocaleDump,
        /// <summary>Default information dump.</summary>
        Default = ExceptionDump | ThreadQueueDump | AppDomainDump |
            AvalonQueueDump | InputLocaleDump,
    }

    #endregion Dump configuration.

    #region Log event support.

    /// <summary>Describes a log request.</summary>
    public class LogEventArgs : EventArgs
    {
        #region Constructors.
        /// <summary>
        /// Creates a new LogEventArgs instance.
        /// </summary>
        /// <param name="text">Text being logged.</param>
        public LogEventArgs(string text)
            : base()
        {
            if (text == null) text = String.Empty;
            this._text = text;
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>
        /// Text of message being logged.
        /// </summary>
        public string Text
        {
            get { return this._text; }
        }

        #endregion Public properties.

        #region Private fields.

        /// <summary>Text of message being logged.</summary>
        private readonly string _text;

        #endregion Private fields.
    }

    /// <summary>
    /// Fires when logging has been requested.
    /// </summary>
    public delegate void LogEventHandler(object sender, LogEventArgs e);

    #endregion Log event support.

    /// <summary>Logging services for test cases.</summary>
    /// <example>The following sample shows different ways in which this
    /// class can be used.
    /// <code>
    /// public void DoLogging() {
    ///   Logger logger = Logger.Current;
    ///   logger.Log("Simple text.");
    ///   logger.Log("Formatted text: {0}, {1}", myString, myInt);
    ///   logger.Log(myObject);
    ///   logger.ReportSuccess();
    /// }
    /// </code></example>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public class Logger : IDisposable
    {
        #region Constructors.

        /// <summary>Hidden constructor.</summary>
        /// <remarks>Access through the Current property.</remarks>
        private Logger() { }

        #endregion Constructors.

        #region Public methods.

        #region IDisposable implementation.

        /// <summary>Disposed of any resources in use.</summary>
        public void Dispose()
        {
            if (_streamWriter != null)
            {
                _streamWriter.Close();
                _streamWriter = null;
            }
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable implementation.

        #region Logging support.

        /// <summary>
        /// Logs a message according to the application setup.
        /// </summary>
        /// <param name="message">Message to log.</param>
        public void Log(string message)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            LogFromThread(Win32.GetCurrentProcessId(),
                Win32.GetCurrentThreadId(), message);
        }

        /// <summary>
        /// Logs a message according to the application setup.
        /// </summary>
        /// <param name="format">
        /// A String containing zero or more format specifications.
        /// </param>
        /// <param name="args">
        /// An Object array containing zero or more objects to be formatted.
        /// </param>
        /// <example>The following sample shows how to use this method.<code>
        /// Logger.Current.Log("Value {0} found, {1} expected.", found, expected);
        /// </code></example>
        public void Log(string format, params object[] args)
        {
            Log(String.Format(format, args));
        }

        /// <summary>Logs a message according to the application setup.</summary>
        /// <param name="obj">Object to log.</param>
        public void Log(object obj)
        {
            if (obj == null)
                Log("<null>");
            else
                Log(obj.ToString());
        }

        /// <summary>
        /// Logs from a specific process and id.
        /// </summary>
        /// <param name="processId">Reporting process id.</param>
        /// <param name="threadId">Reporting thread id.</param>
        /// <param name="message">Message to log.</param>
        /// <example><code>
        /// Logger.Current.LogFromThread(
        ///   Win32.GetCurrentProcessId(), Win32.GetCurrentThreadId(),
        ///   "Hello, world!");
        /// </code></example>
        public void LogFromThread(int processId, int threadId, string message)
        {
            if (message == null) return;

            // Process id and thread id are only processed after we
            // know that we are (a) not buffer a composite log, and
            // (b) we are not sending this to another process.
            if (this._redirectionBuffer != null)
            {
                _redirectionBuffer.Append(message);
                _redirectionBuffer.Append(System.Environment.NewLine);
            }
            else
            {
                string threadPrefix = String.Format(
                    "[process={0};thread={1}] ", processId, threadId);
                if (threadPrefix != _lastThreadPrefix)
                {
                    message = threadPrefix + message;
                    _lastThreadPrefix = threadPrefix;
                }

                if (_streamWriter != null)
                {
                    _streamWriter.WriteLine(message);
                    _streamWriter.Flush();
                }
                else if (this.TestLog != null)
                {
                    this.TestLog.LogStatus(message);
                }
                else
                {
                    System.Console.WriteLine(message);
                }

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Log(0, string.Empty, message + "\n");
                }
                if (LogEvent != null)
                {
                    LogEvent(this, new LogEventArgs(message));
                }
            }
        }

        /// <summary>Logs an image.</summary>
        /// <param name="image">The image to log.</param>
        /// <param name="name">A name for the image.</param>
        /// <remarks>Logged images are saved to the current directory.
        /// The image name used to build a file name for the image.
        /// To configure your test case to have the images available
        /// when running in the lab environment, add the following
        /// line: &amp;WORKDIR&amp;\*.png</remarks>
        /// <example>The following sample shows how to use this method.<code>
        /// public void VerifyElementProperty(Element e) {
        ///   using (Bitmap b = BitmapCapture.CreateBitmapFromElement(e)) {
        ///     Logger.Current.LogImage(b, "element_tested");
        ///     // Verify some property on the element image.
        ///   }
        /// }</code></example>
        public void LogImage(System.Drawing.Image image, string name)
        {
            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            if (image != null)
            {
                if (_combinationIndex != -1)
                {
                    name += "-" + _combinationIndex;
                }

                if (TestRunner._partialTrust)
                {
                    image.Save("\\\\wpf\\TestScratch\\Editing\\tempDataStore\\" + name + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    image.Save(name + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }

                //Microsoft.Test.Logging.TestLog _testLog = new Microsoft.Test.Logging.TestLog("Editing Image Logger");
                //_testLog.LogFile(name + ".png");
                if (this.TestLog != null)
                {
                    //MemoryStream imageStream = new MemoryStream();
                    //image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                    //imageStream.Seek(0, SeekOrigin.Begin);
                    if (TestRunner._partialTrust)
                    {
                        this.TestLog.LogFile(DriverState.ExecutionDirectory+"\\" + name + ".png");
                    }
                    else
                    {
                        this.TestLog.LogFile(name + ".png");
                    }

                }
                else
                {
                    if (TestRunner._partialTrust)
                    {
                        GlobalLog.LogFile(DriverState.ExecutionDirectory + "\\" + name + ".png");
                    }
                    else
                    {
                        GlobalLog.LogFile(name + ".png");
                    }
                    //image.Save(name + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        /// <summary>
        /// Quit method. To be called from test code or otherwise to finish
        /// code flow execution. Can be used for abnormal termination in
        /// case of unexpected errors.
        /// </summary>
        /// <param name="result">Whether the test succeeded.</param>
        /// <deprecated sdk="True" replace="Logger.ReportResult" />
        public void Quit(bool result)
        {
            const bool continueExecution = false;
            Log(" (" + result.ToString() + ") - quiting..");
            if (result == true)
            {
                // log passed
                Log("Test case has PASSED!");
                ReportResult(true, "Test case has PASSED", continueExecution);
            }
            else
            {
                // abnormal termination detected.
                Log("Quit(" + result.ToString() + ") - abnormal termination requested!");
                Log("Test case has FAILED!");
                throw (new Exception("Abnormal termination requested."));
            }
        }


        /// <summary>Reports the result of a test case.</summary>
        /// <param name="testPassed">Whether the test was successful.</param>
        /// <param name="message">Object to log.</param>
        public void ReportResult(bool testPassed, string message)
        {
            ReportResult(testPassed, message, false);
        }

        /// <summary>
        /// Reports the result of a test case.
        /// </summary>
        /// <param name="testPassed">Whether the test was successful.</param>
        /// <param name="message">Message to log.</param>
        /// <param name="continueExecution">
        /// If set to true, does not shut down the application.
        /// </param>
        public void ReportResult(bool testPassed, string message, bool continueExecution)
        {
            if (testPassed == false)
            {
                CasePassed = false;
            }
            if (_streamWriter != null)
            {
                _streamWriter.WriteLine(message);
                _streamWriter.WriteLine(LogPassPrefix + testPassed.ToString());
                _streamWriter.Flush();
            }
            else if (this.TestLog != null)
            {
                this.TestLog.LogStatus(message);
                this.TestLog.Result = (testPassed) ? TestResult.Pass : TestResult.Fail;
            }
            else
            {
                Log("PASS: {0}", message);
            }

            ReportContinueExecution(continueExecution);
        }

        /// <summary>
        /// Reports the result of a test case.
        /// </summary>
        /// <param name="testResult">Whether the test was successful.</param>
        /// <param name="message">Message to log.</param>
        /// <param name="continueExecution">
        /// If set to true, does not shut down the application.
        /// </param>
        public void ReportResult(TestResult testResult, string message, bool continueExecution)
        {
            if (testResult != TestResult.Pass)
            {
                CasePassed = false;
            }
            if (_streamWriter != null)
            {
                _streamWriter.WriteLine(message);
                _streamWriter.WriteLine(LogPassPrefix + testResult.ToString());
                _streamWriter.Flush();
            }
            else if (this.TestLog != null)
            {
                this.TestLog.LogStatus(message);
                this.TestLog.Result = testResult;
            }
            else
            {
                Log("PASS: {0}", message);
            }

            ReportContinueExecution(continueExecution);
        }

        /// <summary>Reports the results for multiple test cases.</summary>
        /// <param name="passCount">Number of test cases that passed.</param>
        /// <param name="failCount">Number of test cases that failed.</param>
        /// <param name="message">Message to log.</param>
        /// <param name="continueExecution">If set to true, does not shut down the application.</param>
        public void ReportResults(int passCount, int failCount,
            string message, bool continueExecution)
        {
            if (failCount > 0)
            {
                CasePassed = false;
            }
            if (_streamWriter != null)
            {
                _streamWriter.WriteLine(message);
                _streamWriter.WriteLine(LogPassPrefix + (failCount == 0));
                _streamWriter.Flush();
            }
            else if (this.TestLog != null)
            {
                this.TestLog.LogStatus("passCount: " + passCount.ToString());
                this.TestLog.LogStatus("failCount: " + failCount.ToString());
                this.TestLog.LogStatus(message);
            }
            else
            {
                Log("PASS/FAIL ({0} / {1}): {2}", passCount, failCount, message);
            }

            ReportContinueExecution(continueExecution);
        }

        /// <summary>
        /// Reports the stage code for the running test case.
        /// </summary>
        /// <param name='stageCode'>
        /// Code for stage, typically for AutomationFramework.
        /// </param>
        /// <remarks>
        /// Values typically used are AutomationFramework.STAGE_INIT,
        /// AutomationFramework.STAGE_RUN or AutomationFramework.STAGE_CLEANUP.
        /// </remarks>
        public void ReportStage(int stageCode)
        {
            if (this.TestLog != null)
                this.TestLog.Stage = (TestStage)stageCode;
        }

        /// <summary>
        /// Reports a successful test run and shuts down the application.
        /// </summary>
        public void ReportSuccess()
        {
            ReportResult(true, "Test passed successfully.", false);
        }

        /// <summary>Queues the reporting of a successful test run.</summary>
        public void QueueSuccess()
        {
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(
                new Test.Uis.Utils.SimpleHandler(ReportSuccess));
        }

        /// <summary>Dumps an exception.</summary>
        /// <param name="exception">Exception to dump.</param>
        public void DumpException(Exception exception)
        {
            DumpException(exception, ExceptionDumpKinds.Default);
        }

        /// <summary>Dumps an exception.</summary>
        /// <param name="exception">Exception to dump.</param>
        /// <param name="kinds">Kinds of dump information to include.</param>
        /// <remarks>
        /// The Reflection permission is asserted to get
        /// a stack trace. The FileIOPermission is asserted to get
        /// filestamps on assemblies and the appdomain location. The
        /// SecurityPermission is asserted to get unmanaged access
        /// to the thread queue.
        /// </remarks>
        public void DumpException(Exception exception, ExceptionDumpKinds kinds)
        {
            System.Security.Permissions.PermissionState state =
                System.Security.Permissions.PermissionState.Unrestricted;
            System.Security.PermissionSet perms =
                new System.Security.PermissionSet(state);
            perms.AddPermission(
                new System.Security.Permissions.ReflectionPermission(state));
            perms.AddPermission(
                new System.Security.Permissions.FileIOPermission(state));
            perms.AddPermission(
                new System.Security.Permissions.SecurityPermission(state));
            perms.Assert();
            _redirectionBuffer = new StringBuilder();

            try
            {
                //InternalDumpException(exception, kinds);
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new InternalDumpExceptionDelegate(InternalDumpException), exception, kinds);
            }
            catch (Exception internalDumpException)
            {
                Line("");
                Line("Exception ocurred while dumping exception.");
                Line("Internal dumping exception follows.");
                Line("");
                Line(internalDumpException.ToString());
            }

            DumpExceptionLoggingBuffer();
        }


        #endregion Logging support.

        #region Log redirection.

        /// <summary>
        /// Redirects logging to a file. Note that this cannot be reverted
        /// at the moment.
        /// </summary>
        /// <param name='fileName'>Name of file to redirect output to.</param>
        public void LogToFile(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (_streamWriter != null)
            {
                throw new InvalidOperationException(
                    "Logging file already opened.");
            }

            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            _streamWriter = new StreamWriter(fileName);
        }

        /// <summary>
        /// Processed the logs specified file and returns whether the log
        /// indicates a test pass or failure.
        /// </summary>
        /// <param name='fileName'>Name of file to redirect output to.</param>
        /// <returns>
        /// true if the log indicates the case succeeded, false otherwise.
        /// </returns>
        public bool ProcessLog(string fileName)
        {
            List<string> logs;  // Log statements.
            bool success;       // Whether the log indicates successful execution.

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            logs = new List<string>();

            // Parse the logged information and get the pass/fail value.
            ParseLog(fileName, logs, out success);

            // Write out the logged information to the real output.
            for (int i = 0; i < logs.Count; i++)
            {
                Log(logs[i]);
            }

            return success;
        }

        #endregion Log redirection.

        #endregion Public methods.


        #region Public properties.

        /// <summary>Retrieves the current logger.</summary>
        /// <remarks>
        /// This provides simple access to the test case logger.
        /// There is no reasonable scenario in which a test case would
        /// use more than one logger, therefore we follow the
        /// AppDomain.CurrentDomain access pattern.
        /// </remarks>
        public static Logger Current
        {
            get
            {
                lock (s_staticLock)
                {
                    if (s_singleton == null)
                    {
                        LoggerTraceListener listener;   // Listener to hook up.

                        s_singleton = new Logger();

                        listener = new LoggerTraceListener(s_singleton);
                        System.Diagnostics.Trace.Listeners.Add(listener);
                    }
                }
                return s_singleton;
            }
        }

        /// <summary>
        /// An AutomationFramework instance that can be used to log messages.
        /// </summary>
        /// <remarks>
        /// To enable logging to the framework, set this property
        /// to a valid object. If this property is null, the default,
        /// messages get logged to System.Console.
        /// </remarks>
        public TestLog TestLog
        {
            get { return this._testLog; }
            set { this._testLog = value; }
        }

        /// <summary>
        /// When different from -1, the index of the currently running
        /// combination.
        /// </summary>
        public int CombinationIndex
        {
            get { return _combinationIndex; }
            set { _combinationIndex = value; }
        }

        /// <summary>
        /// Whether to keep the application running when the test case
        /// if finished. false by default, can be overriden by
        /// the NoExit configuration setting.
        /// </summary>
        public bool KeepApplicationOnFinish
        {
            get { return _keepApplicationOnFinish; }
            set { _keepApplicationOnFinish = value; }
        }

        #endregion Public properties.


        #region Public events.

        /// <summary>Fires when a string is being logged.</summary>
        public LogEventHandler LogEvent;

        #endregion Public events.


        #region Private methods.

        /// <summary>Hidden constructor.</summary>
        ~Logger()
        {
            if (_streamWriter != null)
            {
                _streamWriter.Close();
                _streamWriter = null;
            }
        }

        /// <summary>
        /// Gets a descriptive name for the specified operations.
        /// </summary>
        private string GetOperationName(DispatcherOperation contextOperation)
        {
            Delegate callback;
            object target;

            System.Diagnostics.Debug.Assert(contextOperation != null);

            callback = (Delegate)ReflectionUtils.GetField(contextOperation, "_callback");
            target = callback.Target;
            if (target != null)
            {
                return callback.Method.Name + " on " + target.ToString();
            }
            else
            {
                return callback.Method.DeclaringType + "." + callback.Method.Name;
            }
        }

        /// <summary>Dumps all Avalon queues in the current context.</summary>
        private void DumpAvalonQueue()
        {
            Dispatcher dispatcher;
            /*            object priorityQueue;
                        int priorityItemCount;
                        int maxPriority;
                        object sequentialPriorityItem;
                        int index;
                        DispatcherOperation dispatcherOperation;
            */
            Line("");
            Line("Dumping Avalon queue.");

            dispatcher = Dispatcher.CurrentDispatcher;
            if (dispatcher == null)
            {
                Line("There is no current Dispatcher on the thread.");
                Line("Avalon queues cannot be dumped.");
                return;
            }

            try
            {
                // The queue structure changed. You need to update this code
                // Talked with eduardot before implementing this.

                /*                // Get the priority queue object.
                                priorityQueue = ReflectionUtils.GetField(dispatcher, "_queue");
                                if (priorityQueue == null)
                                {
                                    throw new Exception("Unable to retrieve valid _queue field.");
                                }

                                // Print information about the priority queue.
                                priorityItemCount = (int)ReflectionUtils.GetField(priorityQueue, "_count");
                                maxPriority = (int)ReflectionUtils.GetField(priorityQueue, "_maxPriority");
                                Line("Item count in sequential chain: " + priorityItemCount);
                                Line("Max priority: " + priorityItemCount);

                                // Print information about the sequential chain.
                                index = 0;
                                sequentialPriorityItem = ReflectionUtils.GetField(priorityQueue, "_head");
                                while (sequentialPriorityItem != null)
                                {
                                    dispatcherOperation = (DispatcherOperation)
                                        ReflectionUtils.GetField(sequentialPriorityItem, "_operation");

                                    Line("Sequential item " + index + " - " +
                                        GetOperationName(dispatcherOperation) +
                                        " (IsCompleted=" + dispatcherOperation.WasCompleted +
                                        ";Result=" + dispatcherOperation.Result +
                                        ";Priority=" + dispatcherOperation.Priority + ")");

                                    sequentialPriorityItem = ReflectionUtils.GetField(
                                        sequentialPriorityItem, "_sequentialNext");
                                    index++;
                                }
                */
            }
            catch (Exception e)
            {
                Line("Unable to dump the Avalon queue.");
                Line("A likely cause is change in private fields.");
                Line("Please update Logger.DumpAvalonQueue in Loggers.cs if this is the case.");
                Line("Exception found when dumping queue follows.");
                Line(e.ToString());
            }
        }

        /// <summary>
        /// Dumps the logging buffer used when dumping an exception.
        /// </summary>
        /// <remarks>
        /// Redirecting the logging to a buffer enables the
        /// output to be pretty-printed.
        /// </remarks>
        private void DumpExceptionLoggingBuffer()
        {
            if (_redirectionBuffer != null)
            {
                StringBuilder temp = _redirectionBuffer;
                _redirectionBuffer = null;
                Log(temp);
            }
        }

        /// <summary>Dumps information about the current input locale.</summary>
        private void DumpInputLocale()
        {
            string inputLocale;                 // Current input locale.
            LanguageIdentifierData language;    // Language data for input locale.
            string result;                      // Result of dumping.

            inputLocale = KeyboardInput.GetActiveInputLocaleString();
            language = LanguageIdentifierData.FindByIdentifier(inputLocale.Substring(4));

            result = "Input locale on dumping thread: " + inputLocale;
            result += " - " + ((language == null) ? "unknown" : language.Language);

            Line("");
            Line(result);
        }

        /// <summary>
        /// Shuts down the current application. This references a type from
        /// the PresentationFramework library, and is only meant to be
        /// called from ShutdownApplication().
        /// </summary>
        private void InternalShutdownApplication()
        {
            if (Application.Current != null)
            {
                if (Logger.Current.TestLog != null)
                {
                    TestResult currentResult = Logger.Current.TestLog.Result;
                    if (currentResult == TestResult.Unknown || currentResult == TestResult.Pass)
                    {
                        // don't override previously-set Ignore
                        Logger.Current.TestLog.Result = (Loggers.Logger.CasePassed) ? TestResult.Pass : TestResult.Fail;
                    }
                    GlobalLog.LogEvidence(DriverState.DriverParameters["Class"]);
                    GlobalLog.LogEvidence(DriverState.DriverParameters["Assembly"].Split(',')[0] + ".exe " + DriverState.DriverParameters["MethodParams"]);
                    Logger.Current.TestLog.Close();
                    Logger.Current.TestLog = null;
                }
                ConfigurationSettings.Current.ReinstateOriginalKeyboardState();
                Application.Current.Shutdown();
            }

            //Notify the loader to shut down the browser in which Avalon is hosted
            ApplicationMonitor.NotifyStopMonitoring();
        }

        /// <summary>Continues execution after reporting results if requested.</summary>
        /// <param name="continueExecution">If set to true, does not shut down the application.</param>
        private void ReportContinueExecution(bool continueExecution)
        {
            if (!continueExecution)
            {
                if (!KeepApplicationOnFinish)
                {
                    Dispose();
                    ShutdownApplication();
                }
            }
        }

        /// <summary>
        /// Shuts down the current application, handling the case in which
        /// an incorrect setup prevents up from loading the
        /// PresentationFramework library.
        /// </summary>
        public void ShutdownApplication()
        {
            if (KeepApplicationOnFinish)
            {
                return;
            }
            try
            {
                InternalShutdownApplication();
            }
            catch (System.IO.FileNotFoundException fnf)
            {
                // In this case, we log the exception and exit. Because
                // typically the Application was not loaded in the first
                // place, there is nothing to clean up anyway.
                Log("File not found exception when shutting down application.");
                Log(fnf.Message);
                if (fnf.Message.IndexOf("PresentationFramework") != -1)
                {
                    Log("Known problem: incorrect setup prevents loading " +
                        "PresentationFramework");
                }
            }
        }

        /// <summary>
        /// Populates the specified ArrayList with input from
        /// the given file name, and sets the success value
        /// to true if the test case passed, false otherwise.
        /// </summary>
        private static void ParseLog(string fileName,
            List<string> logs, out bool success)
        {
            System.Diagnostics.Debug.Assert(fileName != null);
            System.Diagnostics.Debug.Assert(logs != null);

            success = false;

            StreamReader reader = new StreamReader(fileName);
            try
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(LogPassPrefix))
                    {
                        string passString = line.Substring(LogPassPrefix.Length);
                        success = bool.Parse(passString);
                    }
                    logs.Add(line);
                }
            }
            finally
            {
                reader.Close();
            }
        }

        #region Exception logging.

        /// <summary>Logs information about an exception.</summary>
        /// <param name="exception">Exception to log.</param>
        /// <param name="kinds">Kinds of information to log.</param>
        /// <remarks>
        /// Unlike DumpException, this method assumes all security
        /// checks will be satisfied.
        /// </remarks>
        private void InternalDumpException(Exception exception,
            ExceptionDumpKinds kinds)
        {
            Line("");
            Line("FAILURE");
            Line("-------");

            if ((kinds & ExceptionDumpKinds.AppDomainDump) != 0)
            {
                DumpAppDomain(AppDomain.CurrentDomain);
            }

            // Detailed assembly dumps "trumps" a regular name dump.
            if ((kinds & ExceptionDumpKinds.DetailedAssemblyDump) != 0)
            {
                DumpDetailedAssemblies(AppDomain.CurrentDomain);
            }
            else if ((kinds & ExceptionDumpKinds.AssemblyDump) != 0)
            {
                DumpAssemblyNames(AppDomain.CurrentDomain);
            }

            // Calculate the exception that is targeted for logging purposes.
            bool chainedExceptions = (kinds &
                ExceptionDumpKinds.ChainedExceptions) != 0;
            Exception targetException;

            if (chainedExceptions)
            {
                targetException = exception;
            }
            else
            {
                Exception child = exception;
                while (child.InnerException != null)
                {
                    child = child.InnerException;
                }
                targetException = child;
            }


            if ((kinds & ExceptionDumpKinds.TraceDump) != 0)
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new DumpTraceDelegate(DumpTrace), targetException);
                //DumpTrace(targetException);
            }

            if ((kinds & ExceptionDumpKinds.ExceptionDump) != 0)
            {
                System.IO.FileNotFoundException fileNotFound;
                System.ComponentModel.Win32Exception win32Exception;
                System.TypeLoadException typeLoadException;

                Line("");
                Line("EXCEPTION OBJECT:");
                Line(targetException.ToString());
                fileNotFound = targetException as System.IO.FileNotFoundException;
                if (fileNotFound != null)
                {
                    Log("Name of file not found: [" + fileNotFound.FileName + "]");
                }

                win32Exception = targetException as System.ComponentModel.Win32Exception;
                if (win32Exception != null)
                {
                    Log("Win32Exception NativeErrorCode (Win32 err): {0}", win32Exception.NativeErrorCode.ToString());
                    Log("Win32Exception ErrorCode (hr): {0}", win32Exception.ErrorCode.ToString());
                }

                typeLoadException = targetException as System.TypeLoadException;
                if (typeLoadException != null)
                {
                    Log("TypeLoadException Message:  " + typeLoadException.Message);
                    Log("TypeLoadException TypeName: " + typeLoadException.TypeName);
                }

                if (!chainedExceptions)
                {
                    Log("");
                    Log("Original full stack trace:");
                    Log(exception.StackTrace.ToString());
                }
            }

            if ((kinds & ExceptionDumpKinds.InputLocaleDump) != 0)
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new SimpleHandler(DumpInputLocale));
            }

            if ((kinds & ExceptionDumpKinds.ThreadQueueDump) != 0)
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new SimpleHandler(DumpThreadQueue));
            }

            //if ((kinds & ExceptionDumpKinds.AvalonQueueDump) != 0)
            //{
            //    //DumpAvalonQueue();
            //}
        }

        /// <summary>Logs the stack trace for an exception.</summary>
        /// <param name="exception">Exception to log.</param>
        private void DumpTrace(Exception exception)
        {
            Line("");
            Line("Stack trace at exception:");
            Log(new StackTrace(exception, true).ToString());
        }

        /// <summary>
        /// Gets the CodeBase property guaranteeing that no exception
        /// is thrown.
        /// </summary>
        private string SafeGetCodeBase(Assembly assembly)
        {
            try
            {
                return assembly.Location;
            }
            catch (Exception)
            {
                return "[unavailable]";
            }
        }

        /// <summary>
        /// Logs detailed information about the assemblies loaded in
        /// an AppDomain.
        /// </summary>
        /// <param name="appDomain">
        /// Application domain from which to take assemblies to log.
        /// </param>
        private void DumpDetailedAssemblies(AppDomain appDomain)
        {
            Line("");
            Line("Assembly information");

            Assembly[] assemblies = appDomain.GetAssemblies();

            foreach (System.Reflection.Assembly a in assemblies)
            {
                Line("");
                Line("Assembly:    " + a.GetName().Name);
                Line("  " + a.FullName);
                Line("  Code Base: " + SafeGetCodeBase(a));
                Line("  Loaded modules:");

                System.Reflection.Module[] modules = a.GetLoadedModules();

                foreach (System.Reflection.Module m in modules)
                {
                    string file = m.FullyQualifiedName;
                    string lastWrite;

                    //
                    // Libraries embedded in containers will have an invalid
                    // filename and throw an exception.
                    //
                    try
                    {
                        lastWrite = System.IO.File.GetLastWriteTime(file)
                            .ToString();
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        lastWrite = "[directory not found]";
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        lastWrite = "[file not found]";
                    }
                    catch (System.ArgumentException)
                    {
                        lastWrite = "[unknown]";
                    }
                    Line("    " + file + " (last written " + lastWrite + ")");
                }
            }
        }

        /// <summary>Logs loaded assembly names from an AppDomain.</summary>
        /// <param name="appDomain">
        /// Application domain from which to take assembly names to log.
        /// </param>
        private void DumpAssemblyNames(AppDomain appDomain)
        {
            Line("");

            System.Reflection.Assembly[] assemblies = appDomain.GetAssemblies();

            foreach (System.Reflection.Assembly a in assemblies)
            {
                Line("Assembly: " + a.GetName().Name);
            }
        }

        /// <summary>Logs information about a given application domain.</summary>
        /// <param name="appDomain">Application domain to log.</param>
        private void DumpAppDomain(AppDomain appDomain)
        {
            Line("");
            Line("Base directory resolver used to probe for assemblies:");
            Line(appDomain.BaseDirectory);
        }

        /// <summary>Logs the messages in the native thread queue.</summary>
        private void DumpThreadQueue()
        {
            const int MaxMessagesToDrain = 48;
            const int MaxPaintMessagesToDrain = 4;

            int drainCount;     // Number of messages drained.
            int paintCount;     // Number of paint messages drained in a row.
            string sb = "";   // StringBuilder used to build messages.
            Win32.MSG msg;      // Message being received.

            drainCount = 0;
            paintCount = 0;
            msg = new Win32.MSG();



            while (Win32.PeekMessage(msg, Win32.HWND.NULL, 0, 0, Win32.PM_NOREMOVE) &&
                (drainCount < MaxMessagesToDrain) &&
                (paintCount < MaxPaintMessagesToDrain))
            {
                Win32.TranslateMessage(msg);
                string str = Win32.GetMessageDescription(msg);
                if (sb.Contains(str) == false)
                {
                    sb += str + ", ";
                }
                drainCount++;
                if (msg.message.ToUInt32() == Win32.WM_PAINT)
                {
                    paintCount++;
                }
                else
                {
                    paintCount = 0;
                }
            }


            Line("");

            if (drainCount == MaxMessagesToDrain)
                Line("Maximum amount of messages drained: " + drainCount);
            else if (paintCount == MaxPaintMessagesToDrain)
                Line("Maximum amount of consecutive paint messages drained: " + MaxPaintMessagesToDrain);
            else
                Line("Messages drained: " + drainCount);

            Line(sb);
        }

        /// <summary>Writes a single logical line of text.</summary>
        /// <param name="content">Line content.</param>
        /// <remarks>
        /// This method was previously used to implement smart line breaking,
        /// but this tends to make output messy.
        /// </remarks>
        private void Line(string content)
        {
            Log(content);
        }

        #endregion Exception logging.

        #endregion Private methods.


        #region Private data.

        /// <summary>static value to keep track of test cases pass/fail</summary>
        public static bool CasePassed = true;

        /// <summary>Harness TestLog</summary>
        private TestLog _testLog;

        /// <summary>Whether to keep the application running when a test finishes.</summary>
        private bool _keepApplicationOnFinish;

        /// <summary>Buffer to redirect logging to if assigned.</summary>
        private StringBuilder _redirectionBuffer;

        /// <summary>Last message prefix for thread identifying.</summary>
        private string _lastThreadPrefix;

        /// <summary>When different from -1, the index of the currently running combination.</summary>
        private int _combinationIndex = -1;

        /// <summary>Writer to redirect to if assigned.</summary>
        private StreamWriter _streamWriter;

        /// <summary>Prefix used in log file.</summary>
        private const string LogPassPrefix = "Test.Uis.Loggers.Logger - Pass: ";

        #region Private static data.

        /// <summary>Singleton logger instance.</summary>
        private static Logger s_singleton;

        /// <summary>Lock for accessing static logger data.</summary>
        private static object s_staticLock = new object();

        delegate void DumpTraceDelegate(Exception e);
        delegate void InternalDumpExceptionDelegate(Exception e, ExceptionDumpKinds kinds);

        #endregion Private static data.

        #endregion Private data.
    }
}
