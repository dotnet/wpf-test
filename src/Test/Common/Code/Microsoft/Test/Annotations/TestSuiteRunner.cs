// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//  Description: 
//  Creator: derekme
//  Date Created: 1/25/06
//---------------------------------------------------------------------

using Annotations.Test.Framework.Internal;
using Microsoft.Test.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Security;
using System.Security.Permissions;

namespace Annotations.Test.Framework
{	
    /// <summary>
    /// Class which handles running a TestSuite.
    /// </summary>
    public class TestSuiteRunner
    {
        #region Public Methods

        /// <summary>
        /// Run all variations of the given TestCases.
        /// </summary>
        /// <param name="tests"></param>
        internal void RunTests(IList<TestCase> tests)
        {
            NumberOfVariationsComplete = 0;
            SetupErrorHandlers();

            TestCaseCollection = tests;
            TestCaseEnumerator = TestCaseCollection.GetEnumerator();
            RunNextTestCase();
        }        

        /// <summary>
        /// Run all the given TestVariations.
        /// </summary>
        /// <param name="variations"></param>
        internal void RunVariations(IList<TestVariation> variations)
        {            
            SetupErrorHandlers();

            VariationEnumerator = variations.GetEnumerator();            
            RunNextVariation();
        }

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Get/Set runner mode.
        /// </summary>
        public RunnerMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Signaled when runner if finished running.
        /// </summary>
        public event FinishedEventHandler Finished;
        /// <summary>
        /// Delegate for FinshedEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void FinishedEventHandler(object sender, TestSuiteCompleteEventArgs e);

        #endregion

        #region Protected Properties

        /// <summary>
        /// Test currently being run.
        /// </summary>
        internal TestCase CurrentTestCase
        {
            get
            {
                return TestCaseEnumerator.Current;
            }
        }

        /// <summary>
        /// Suite currently being run.
        /// </summary>
        protected TestSuite CurrentSuite
        {
            get
            {
                if (CurrentVariation == null)
                    return null;
                return CurrentVariation.TestCase.Suite;
            }
        }

        /// <summary>
        /// Variation currently being run.
        /// </summary>
        internal TestVariation CurrentVariation
        {
            get
            {
                if (VariationEnumerator == null)
                    return null;
                return VariationEnumerator.Current;
            }
        }


        #endregion

        #region Private Methods

        private void SetupErrorHandlers()
        {
            // Catch and handle ALL Debug.Assert failures.
            // Renamed this in .NET Core, Debug.Listeners no longer exists,
            // for the purposed of catching Debug.Assert Trace.Listeners works the same.
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TestSuiteTraceListener());

            // Catch and handle exceptions thrown from Dispatcher items.
            // Make sure we don't register duplicate event handlers.
            Dispatcher.CurrentDispatcher.UnhandledException -= HandleDispatcherExceptions;
            Dispatcher.CurrentDispatcher.UnhandledException += HandleDispatcherExceptions;
        }

        /// <summary>
        /// Iterate the TestCaseEnumerator.  If there are no more TestCases to run, signal Finished.
        /// </summary>
        private void RunNextTestCase()
        {
            if (TestCaseEnumerator.MoveNext())
            {
                // Don't automatically run test cases that are marked as Disabled.
                if (TestCaseEnumerator.Current.IsDisabled)
                    RunNextTestCase();
                else
                    RunVariations(TestCaseEnumerator.Current.Variations);
            }
            else
            {
                SignalComplete();
            }
        }

        /// <summary>
        /// Iterate the VariationEnumerator.  If there is another variation run it, signal Finished.
        /// </summary>
        private void RunNextVariation()
        {
            if (VariationEnumerator.MoveNext())
            {
                if (!CurrentVariation.TestCase.IsDisabled)
                {
                    // Instantiate a logger that wrapps the ClientTestRuntime implementation.                                       
                    variationLog = new TestLog(CurrentVariation.ToString());
                    CurrentSuite._caseNumber = CurrentVariation.TestCase.Id;

                    InitializeVariation();      // Use a command line to initialize the state for given Variation.
                    // Make sure that we don't try to continue running a test if it failed during setup.
                    if (CurrentVariation != null &&
                        CurrentVariation.Result != VariationResult.Failed &&
                        Mode != RunnerMode.SetupOnly)
                    {                        
                        GlobalLog.LogEvidence("Running Test Variation");
                        RunVariation();             // Run test variation, will close logger at the end.
                        // NOTE: Cleanup is done when the result of the test is logged, may be asynchronous from the call to Run.
                    }                   
                }
                else
                {
                    //just in case disabled case gets ran.
                    GlobalLog.LogEvidence("This case is disabled.");
                    NumberOfVariationsComplete++;
                    EndVariation();
                }
            }
            else
            {
                EndVariation();
            }
        }

        /// <summary>
        /// Intiailize the TestSuite for running a specific variation, e.g. parse arguments
        /// and set the proper properties.
        /// </summary>
        private void InitializeVariation()
        {            
            GlobalLog.LogStatus("Starting Setup Stage");
            CurrentSuite.ProcessArgs(CurrentVariation.Parameters);

            if (CurrentVariation.TestCase.SetupMethod != null)
            {
                Invoke(CurrentVariation.TestCase.SetupMethod);
            }
        }

        /// <summary>
        /// Run test with the current setup.
        /// </summary>
        protected virtual void RunVariation()
        {
            // Deprecated: this is for backwards compat, eventually should remove...
            if (CurrentVariation.TestCase.TestMethod == null)
            {
                Invoke(CurrentSuite.GetType().GetMethod("RunTestCase", BindingFlags.Instance | BindingFlags.NonPublic));
            }
            else
            {
                Invoke(CurrentVariation.TestCase.TestMethod);
            }            
        }


        /// <summary>
        /// Set the result of the current variation, clean up, and then start running the next variation (if any).
        /// </summary>
        protected virtual object LogAndShutdownVariation(object arg)
        {            
            // Don't run cleanup or start the next variation if HoldAtEnd.
            if (Mode != RunnerMode.HoldAtEnd)
            {                
                // If the harness hasn't terminated the process, cleanup and run next variation.
                // Do this before Logging the variation becuase it may affect the result.
                if (CurrentVariation.TestCase.CleanupMethod != null)
                {
                    // If we've already failed then ignore any exceptions during cleanup.
                    if (CurrentVariation.Result != VariationResult.Failed)
                    {
                        // CurrentSuite.CurrentLogger.SetStage(VariationStage.Cleanup); - setting stage to cleanup messes up the logging.
                        Invoke(CurrentVariation.TestCase.CleanupMethod);
                    }
                    else
                    {
                        try
                        {
                            CurrentVariation.TestCase.CleanupMethod.Invoke(CurrentSuite, null);
                        }
                        catch
                        {
                            // Ignore.
                        }
                    }
                }

                // Log variation after we have run the cleanup incase there is a 
                LogVariation();

                //
                // Closing the logger flushes the Result to the underlying test harness,
                // Do this after the variation has been logged.
                //                
                if (variationLog != null)
                {                                                         
                    variationLog.Close();
                }               

                // Increment complete count.
                NumberOfVariationsComplete++;
                
                RunNextVariation();
            }

            return null;
        }

        /// <summary>
        /// Run when all variations are complete, shutdown the test.
        /// </summary>
        private void EndVariation()
        {            
            // If we are only running Variaitions then we are finished.
            if (TestCaseCollection == null || TestCaseCollection.Count == 0)
            {
                SignalComplete();
            }
            // Otherwise start the next TestCase.
            else
            {                
                RunNextTestCase();
            }
        }

        /// <summary>
        /// Signal to TestSuite that run is complete.
        /// </summary>
        private void SignalComplete()
        {           
            Dispatcher.CurrentDispatcher.UnhandledException -= HandleDispatcherExceptions;
            if (Finished != null)
                Finished(this, new TestSuiteCompleteEventArgs(NumberOfVariationsComplete));
        }

        /// <summary>
        /// Log the result of the test to the Avalon AutomationFramework.
        /// </summary>
        private void LogVariation()
        {            
            if (variationLog != null)
            {
                if (CurrentVariation.Result == VariationResult.Passed)
                {
                    variationLog.LogStatus(CurrentVariation.Message);
                    variationLog.Result = TestResult.Pass;
                }
                else
                {
                    variationLog.LogEvidence(CurrentVariation.Message);
                    variationLog.Result = TestResult.Fail;
                }      
            }                                      
        }

        /// <summary>
        /// Store the State of the current variation as PASSED.
        /// </summary>
        /// <remarks>Result is note actually logged to the harness until LogVariation is called.</remarks>
        private void SetVariationPassed(string message)
        {
            SetVariationState(VariationResult.Passed, message);
        }

        /// <summary>
        /// Store the State of the current variation as FAILED.
        /// </summary>
        /// <remarks>Result is note actually logged to the harness until LogVariation is called.</remarks>
        private void SetVariationFailed(Exception e)
        {
            SetVariationFailed(e.GetType().FullName + " - " + e.Message + "\nFailure Trace:\n" + e.StackTrace);
        }

        /// <summary>
        /// Store the State of the current variation as FAILED.
        /// </summary>
        /// <remarks>Result is note actually logged to the harness until LogVariation is called.</remarks>
        private void SetVariationFailed(string message)
        {
            SetVariationState(VariationResult.Failed, message);
        }

        /// <summary>
        /// Store the State of the current variation.
        /// </summary>
        /// <remarks>Result is note actually logged to the harness until LogVariation is called.</remarks>
        private void SetVariationState(VariationResult state, string message)
        {                
            CurrentVariation.Message = message;
            CurrentVariation.Result = state;
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
#if TESTBUILD_CLR40
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
#endif
        private void Invoke(MethodInfo method)
        {
            try
            {
                method.Invoke(CurrentSuite, null);
            }
            catch (Exception e)
            {
                // Don't handle certain types of exceptions.
                if (!IsHandlable(e))
                    throw;

                // Don't try and handle it if we haven't initialized our TestVariation yet.
                if (CurrentVariation == null)
                    throw;

                // Unwrap TargetInvoationExceptions to get at the "intersting" information.
                if (e.GetType().Equals(typeof(TargetInvocationException)))
                    HandleException(e.InnerException);
                else
                    HandleException(e);
            }
        }

        /// <summary>
        /// Handle any kind of exception and convert it into a pass or fail state for the 
        /// test.
        /// 
        /// If FAIL: fail and shutdown test immediately.
        /// If PASS: process remaining context tasks and then pass and shutdown.
        /// </summary>
        /// <param name="e"></param>
        private void HandleException(Exception e)
        {
            // Ignore future exceptions if the test has already been logged as a failure
            // because this is probably a side-effect of the original failure.
            if (CurrentVariation.Result == VariationResult.Failed)
                return;

            if (e is TestPassedException)
            {
                SetVariationPassed(e.Message);
                /*
                 * Allow all remaining context tasks to be processed before logging PASSED.  This will
                 * hopefully catch exceptions that might be thrown asynchronously.
                 */
                CurrentSuite.BeginInvoke(new DispatcherOperationCallback(LogAndShutdownVariation), null, DispatcherPriority.SystemIdle);
            }
            else
            {
                if (e is TestFailedException)
                {
                    SetVariationFailed(e);
                }
                else
                {
                    string message = e.ToString();

                    // If this exception was thrown from Proxy framework then append the proxy stack trace
                    // for debugging.
                    if (e.Data.Contains("ProxyStackTrace"))
                        message = e.Message + ":\n" + e.Data["ProxyStackTrace"] + "\n" + e.StackTrace;
                    SetVariationFailed("Unexpected exception was thrown:\n" + e.GetType().FullName + " - " + message);

                }

                // Shutdown immediately (do not process further context tasks).                
                LogAndShutdownVariation(null);
            }
        }

        private void HandleResult(TestResultException e)
        {
            if (e is TestPassedException)
            {
                SetVariationPassed(e.Message);
                /*
                 * Allow all remaining context tasks to be processed before logging PASSED.  This will
                 * hopefully catch exceptions that might be thrown asynchronously.
                 */
                CurrentSuite.BeginInvoke(new DispatcherOperationCallback(LogAndShutdownVariation), null, DispatcherPriority.SystemIdle);
            }
            else if (e is TestFailedException)
            {
                SetVariationFailed(e);
                // Shutdown immediately (do not process further context tasks).                
                LogAndShutdownVariation(null);
            }
        }

        /// <summary>
        /// Called when an exception is triggered by any ContextDelegates in the 
        /// application's context.  Will decide the pass or fail state of the current
        /// test case based on what type of exception triggered this handler 
        /// (e.g. TestFailedException -> FAIL, TestPassedException -> PASS).
        /// Causes immediate termination of the application.
        /// </summary>
        private void HandleDispatcherExceptions(object sender, DispatcherUnhandledExceptionEventArgs args)
        {            
            // If our variation isn't initialized yet just let it pass through.
            if (CurrentVariation != null)
            {
                args.Handled = true;
                if (args.Exception is TargetInvocationException && args.Exception.InnerException != null)
                    HandleException(args.Exception.InnerException);
                else
                    HandleException(args.Exception);
            }
        }

        // We will allow certain kinds of exceptions to pass through the Framework unhandled so that
        // automation tools such as LorDbg can generate detailed dump files for analysis.
        private bool IsHandlable(Exception e)
        {
            return !(   
                e is AccessViolationException ||
                e.InnerException is AccessViolationException ||
                e is System.Runtime.InteropServices.SEHException ||
                e.InnerException is System.Runtime.InteropServices.SEHException);
        }

        #endregion               

        #region Fields

        private IList<TestVariation> TestVariationCollection = new List<TestVariation>();
        IEnumerator<TestVariation> VariationEnumerator;
        
        private IList<TestCase> TestCaseCollection;
        IEnumerator<TestCase> TestCaseEnumerator;

        private int NumberOfVariationsComplete = 0;
        private RunnerMode _mode = RunnerMode.Normal;
        private TestLog variationLog;

        #endregion

        #region Private Classes 

        /// <summary>
        /// Trace Listener that prevents failed Debug.Asserts from popping up a dialog.  Instead, it 
        /// captures the failure and logs it to the command line.  This enables clean automation and reporting
        /// of tests that show Debug.Assert errors.
        /// </summary>
        private class TestSuiteTraceListener : DefaultTraceListener
        {
            public override void Fail(string message)
            {
                throw new TestFailedException("Debug.Assert FAILED: " + message);
            }

            public override void Fail(string message, string detailMessage)
            {
                throw new TestFailedException("Debug.Assert FAILED: " + message + "\n" + detailMessage);
            }
        }

        #endregion        
    }

    /// <summary>
    /// Mode of execution.
    /// </summary>
    public enum RunnerMode
    {
        /// <summary>
        /// Run test standalon.
        /// </summary>
        Normal,
        /// <summary>
        /// Only execute Variation's Setup.
        /// </summary>
        SetupOnly,
        /// <summary>
        /// Wait at the end of the Test phase.
        /// </summary>
        HoldAtEnd
    }

    /// <summary>
    /// Event fired when TestSuite is complete.
    /// </summary>
    public class TestSuiteCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Create completed event.
        /// </summary>
        /// <param name="numberOfVaritionsRun">Number of variations run before event was fired.</param>
        public TestSuiteCompleteEventArgs(int numberOfVaritionsRun)
        {
            NumberOfVariationsRun = numberOfVaritionsRun;
        }

        /// <summary>
        /// Number of variations run before event was fired.
        /// </summary>
        public int NumberOfVariationsRun = 0;

    }
}
