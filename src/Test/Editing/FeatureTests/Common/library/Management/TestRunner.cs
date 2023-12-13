// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides an entry point for running test cases.

#region Namespaces.

using System;
using System.Threading;
using System.Windows.Threading;

using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Navigation;
using Test.Uis.Management;
using Test.Uis.Loggers;
using Test.Uis.Utils;
using Test.Uis.Wrappers;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Discovery;


#endregion Namespaces.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Management/TestRunner.cs $")]

namespace Test.Uis.Management
{
    /// <summary>Provides a utility class to run test cases.</summary>
    public static class TestRunner
    {

        /// <summary>
        /// _testThread
        /// </summary>
        public static Thread _testThread;

        /// <summary>
        /// _partialTrust
        /// </summary>
        public static bool _partialTrust = false;
        
        #region Public methods.

        /// <summary>Entry point for .deploy files.</summary>
        /// <example>
        /// To use this method in a deploy file, write the following
        /// code for your main page Loaded event.<code>...
        /// private void TopPanelLoaded(object sender, EventArgs e) {
        ///   Test.Uis.Management.TestRunner.DoDeployWindowLoaded();
        /// }
        /// </code></example>
        public static void DoDeployWindowLoaded()
        {
            DoDeployWindowLoaded(new string[] { });
        }

        /// <summary>Entry point for .deploy files with interactive testing.</summary>
        /// <param name="args">Arguments to add to command line.</param>
        /// <example>
        /// To use this method in a deploy file, write the following
        /// code for your main page Loaded event.<code>...
        /// private void TopPanelLoaded(object sender, EventArgs e) {
        ///   Test.Uis.Management.TestRunner.DoDeployWindowLoaded(testCaseName);
        /// }
        /// </code></example>
        public static void DoDeployWindowLoaded(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            try
            {
                string[] envArgs = ConfigurationSettings.GetEnvironmentCommandLineArgs();
                string[] temp = new string[args.Length + envArgs.Length];
                envArgs.CopyTo(temp, 0);
                args.CopyTo(temp, envArgs.Length);
                args = temp;
                envArgs = temp = null;
                ConfigurationSettings settings = new ConfigurationSettings(args);
                SetupStandaloneTest();
                InternalMain();
            }
            catch (Exception exception)
            {
                HandleOutermostException(exception, "outermost deploy handler");
            }
        }


        /// <summary>Entry point for .exe and .dll files.</summary>
        /// <param name="args">Command-line arguments.</param>
        /// <example><p>
        /// To use this method in a custom exe, write the
        /// following line in any one of your classes.
        /// </p><code>...
        /// [STAThread] public static void Main(string[] args) { TestRunner.DoMain(args); }
        /// </code></example>
        public static void DoMain(string[] args)
        {

            if (args[args.Length - 1] == "/pt")
            {
                _partialTrust = true;
                args[args.Length - 1] = "";
            }
            TestWorkerThread(args);
        }

        private static void TestWorkerThread(object obj)
        {
            string[] args = obj as string[];
            ConfigurationSettings settings;
            bool attachDebugger;

            try
            {
                settings = new ConfigurationSettings(args);

                attachDebugger = settings.GetArgumentAsBool("WaitForDebuggerAttach");
                if (attachDebugger)
                {
                    Win32.CreateAttachDebuggerMessageBox();
                }

                // Special flag for creating a version report on the domain.
                if (settings.GetArgumentAsBool("CreateVersionReport"))
                {
                    CreateVersionReport();
                    return;
                }

                // If the Failure Analysis component handles this run,
                // exit immediately.
                if (Test.Uis.Analysis.FailureAnalysisForm.DoAnalysis(settings))
                {
                    return;
                }

                SetupStandaloneTest();
                InternalMain();
            }
            catch (Exception exception)
            {
                HandleOutermostException(exception, "outermost handler");
            }
        }

        /// <summary>Handles exceptions that terminate the test case.</summary>
        /// <param name="exception">Exception caught.</param>
        /// <param name="catcher">
        /// Description of where the description was caught.
        /// </param>
        /// <remarks>
        /// The exception is logged, the test case marked as failed, and
        /// application shutdown is initiated if required.
        /// </remarks>
        public static void HandleOutermostException(Exception exception,
            string catcher)
        {
            const bool continueExecution = true;
            ExceptionDumpKinds kinds = GetExceptionDumpKinds();
            Logger.Current.DumpException(exception, kinds);
            Logger.Current.ReportResult(false,
                "Exception [" + GetInnermostMessage(exception) +
                "] caught at " + catcher, continueExecution);
            try
            {
                if (!ConfigurationSettings.Current.GetArgumentAsBool("NoExit", false))
                {
                    ShutdownApplication();
                }
            }
            catch (Exception shutdownException)
            {
                Log("Exception while shutting down.");
                Log(shutdownException.ToString());
            }
        }

        /// <summary>
        /// Sets up the exception handler for the current Dispatcher to avoid
        /// displaying a UI, which typically interferes with automated test
        /// case execution.
        /// </summary>
        public static void SetupApplicationExceptionHandler()
        {
            Application application = Application.Current;
            if (application == null)
            {
                throw new InvalidOperationException(
                    "There is no UI context on which to set exception handler");
            }

            application.DispatcherUnhandledException +=
                new DispatcherUnhandledExceptionEventHandler(DispatcherException);
        }

        #endregion Public methods.


        #region Private methods.

        /// <summary>
        /// A delegate calls this method when an exception reaches the context
        /// dispatcher.
        /// </summary>
        private static void DispatcherException(object sender,
            DispatcherUnhandledExceptionEventArgs args)
        {
            if (args.Exception is ThreadAbortException)
            {
                args.Handled = true;
                return;
            }
            HandleOutermostException(args.Exception, "TestRunner.DispatcherException");
            args.Handled = true;
        }

        /// <summary>
        /// Writes a log of the current file versions to the console.
        /// </summary>
        private static void CreateVersionReport()
        {
            System.Console.WriteLine(VersionInformationAttribute.CreateHtmlReport());
        }

        /// <summary>
        /// Best-effort to read kind of exception dump requested. May
        /// fail silently, in which case it returns a verbose request.
        /// </summary>
        private static ExceptionDumpKinds GetExceptionDumpKinds()
        {
            try
            {
                string s;
                ConfigurationSettings cs = ConfigurationSettings.Current;
                if (cs == null || !cs.HasArgument("DumpKinds", out s))
                    return ExceptionDumpKinds.Default;
                return (ExceptionDumpKinds)
                    Enum.Parse(typeof(ExceptionDumpKinds), s);
            }
            catch (Exception)
            {
                return ExceptionDumpKinds.All;
            }
        }

        /// <summary>
        /// Gets the exception message of the innermost exception.
        /// </summary>
        private static string GetInnermostMessage(Exception exception)
        {
            if (exception == null)
            {
                return String.Empty;
            }
            while (exception.InnerException != null)
                exception = exception.InnerException;
            return exception.Message;
        }

        /// <summary>Runs the test case.</summary>
        /// <remarks>
        /// This code is common to all test cases, regardless of
        /// their entry point and bootstrapping.
        /// </remarks>
        public static void InternalMain()
        {
            ConfigurationSettings settings = ConfigurationSettings.Current;
            System.Diagnostics.Debug.Assert(settings != null);

            Logger.Current.KeepApplicationOnFinish = settings.GetArgumentAsBool("NoExit", false);

            // If a combinatorial specification was provided, run the
            // combinations instead of a single specific case.
            string combinatorialFile = settings.GetArgument(Coordinator.CombFileArgument);
            if (combinatorialFile.Length > 0)
            {
                RunCombinatorial(combinatorialFile);
                return;
            }

            // Read the arguments using the xml switch.
            settings.BuildXmlArguments();

            string typeName = settings.GetArgument("TestCaseType").Trim();
            if (typeName.Length == 0)
            {
                throw new InvalidOperationException(
                    "TestCaseType setting not specified\n" +
                    "Available test cases:\n" +
                    String.Join("\n", TestFinder.ListAvailableTestCases(false)));
            }



            TestFinder.RunNamedTestCase(typeName);

        }


        /// <summary>
        /// Runs all the test cases derived from the specified combinatorial file.
        /// </summary>
        /// <param name="combinatorialFile">File with combinatorial information.</param>
        private static void RunCombinatorial(string combinatorialFile)
        {
            // Allow the test case coordinator to run all combinations.
            Coordinator c = new Coordinator();
            c.Settings = ConfigurationSettings.Current;
            c.RunTestCombinations(combinatorialFile);
            return;
        }

        /// <summary>
        /// Sets up test services for a standalone test.
        /// </summary>
        private static void SetupStandaloneTest()
        {
            string outputFile = ConfigurationSettings.Current.GetArgument("OutputFile");
            string logName = "EditingLogger";
            logName = DriverState.TestName;

            if (outputFile != "")
            {
                Logger.Current.LogToFile(outputFile);
            }
            else if (!System.Diagnostics.Debugger.IsAttached && Variation.Current == null)
            {
                TestLog logger = new TestLog(logName);
                Logger.Current.TestLog = logger;
                ConfigurationSettings.Current.TestLog = logger;
            }
        }

        /// <summary>Shuts down the application.</summary>
        /// <remarks>
        /// Having a separate method for this allows the test case failure
        /// reporting to continue despite a missing library - very useful
        /// when troubleshooting setup problems.
        /// </remarks>
        public static void ShutdownApplication()
        {
            LogOnShutDown();
            ConfigurationSettings.Current.ReinstateOriginalKeyboardState();
            if (Application.Current != null)
                Application.Current.Shutdown();

            //Notify the Application monitor to shut down the browser in which Avalon is hosted.
           ApplicationMonitor.NotifyStopMonitoring();
        }

        private static void LogOnShutDown()
        {
            if (Logger.Current.TestLog != null)
            {
                Logger.Current.TestLog.Result = (Loggers.Logger.CasePassed) ? TestResult.Pass : TestResult.Fail;

                GlobalLog.LogEvidence(DriverState.DriverParameters["Class"]);
                GlobalLog.LogEvidence(DriverState.DriverParameters["Assembly"].Split(',')[0] + ".exe " + DriverState.DriverParameters["MethodParams"]);

                Logger.Current.TestLog.Close();
                Logger.Current.TestLog = null;
            }
        }

        #region Convenience methods.

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="text">Message to log.</param>
        private static void Log(string text)
        {
            Logger.Current.Log(text);
        }

        #endregion Convenience methods.

        #endregion Private methods.

    }
}
