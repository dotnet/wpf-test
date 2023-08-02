// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Avalon.Test.Framework.Dispatchers
{
    /******************************************************************************
    * CLASS:          UnhandledExceptionTest
    ******************************************************************************/
    [Test(0, @"Dispatcher.UnhandledException", TestCaseSecurityLevel.FullTrust, "UnhandledExceptionTest")]
    public class UnhandledExceptionTest : WindowTest
    {
        #region Constructor
        private int _numverOfHandleronUnhanderedEventInvoked =0;
        private int _numverOfHandleronUnhanderedEventFilterInvoked =0;
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("TestRemovedHandlers")]
        [Variation("UnhandledException1")]
        [Variation("UnhandledException2")]
        [Variation("UnhandledException3")]
        [Variation("UnhandledException4")]
        [Variation("UnhandledException5")]

        /******************************************************************************
        * Function:          UnhandledExceptionTest Constructor
        ******************************************************************************/
        public UnhandledExceptionTest(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "TestRemovedHandlers":
                    TestRemovedHandlers();
                    break;
                case "UnhandledException1":
                    UnhandledException1();
                    break;
                case "UnhandledException2":
                    UnhandledException2();
                    break;
                case "UnhandledException3":
                    UnhandledException3();
                    break;
                case "UnhandledException4":
                    UnhandledException4();
                    break;
                case "UnhandledException5":
                    UnhandledException5();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /// <summary>
        /// Scenario: all handlers on UnhandledEvent and UnhandledEventFiler has been removed.
        /// Thus no handler was invoked and the exception was caught in the catch. 
        /// </summary>
        public void TestRemovedHandlers()
        {
            GlobalLog.LogStatus("Verify that after all handlers have been removed, no handler is invoked.");
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledExceptionFilter.");
            dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(HandlerOnUnhandledExceptionFilterRequestCatch);
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledException.");
            dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandlerOnUnhandledExceptionNotHandled);
            
            GlobalLog.LogStatus("Removing EventHandler on Dispatcher.UnhandledExceptionFilter.");
            dispatcher.UnhandledExceptionFilter -= new DispatcherUnhandledExceptionFilterEventHandler(HandlerOnUnhandledExceptionFilterRequestCatch);
            GlobalLog.LogStatus("Removing EventHandler on Dispatcher.UnhandledException.");
            dispatcher.UnhandledException -= new DispatcherUnhandledExceptionEventHandler(HandlerOnUnhandledExceptionNotHandled);
            bool CaughtCorrectException = false;

            try
            {
                dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new NoArgCallback(ThrowAnException));
                GlobalLog.LogStatus("Run Dispatcher.");
                DispatcherHelper.RunDispatcher();
            }
            catch (Exception e)
            {
                if (String.Equals(e.InnerException.Message, "Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.", StringComparison.InvariantCulture))
                {
                    CaughtCorrectException = true;
                }
                else // We just caught a mystery exception, log it for giggles / debugability.
                {
                    GlobalLog.LogStatus("ERROR: Caught unexpected exception: " + e.Message + "\n" + e.StackTrace);
                }
            }
            finally
            {
                Verification(CaughtCorrectException, 0, 0);
            }
        }
        /// <summary>
        /// Implement the senaro defined in the line 1 on the spec.
        /// </summary>
        public void UnhandledException1()
        {
            GlobalLog.LogStatus("Test Unhandled Exception for Dispatcher : case 1.");

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledExceptionFilter.");
            dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(HandlerOnUnhandledExceptionFilterRequestCatch);
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledException.");
            dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandlerOnUnhandledExceptionHandled);
            bool CaughtCorrectException = true;
            try
            {
                dispatcher.Invoke(DispatcherPriority.Normal,
                    new NoArgCallback(ThrowAnException));
                GlobalLog.LogStatus("Run Dispatcher.");
                DispatcherHelper.RunDispatcher();
            }
            catch (Exception e) 
            {
                // should be no exception here.
                GlobalLog.LogStatus("ERROR: Caught unexpected exception: " + e.Message + "\n" + e.StackTrace);
                CaughtCorrectException = false;
            }
            finally
            {
                Verification(CaughtCorrectException, 1, 1);
            }
        }
        /// <summary>
        /// Implement the senaro defined in the line 2 on the spec.
        /// </summary>
        public void UnhandledException2()
        {
            GlobalLog.LogStatus("Test Unhandled Exception for Dispatcher.");
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledExceptionFilter : case 2.");
            dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(HandlerOnUnhandledExceptionFilterRequestCatch);
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledException.");
            dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandlerOnUnhandledExceptionNotHandled);
            bool CaughtCorrectException = false;
            try
            {
                dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new NoArgCallback(ThrowAnException));
                GlobalLog.LogStatus("Run Dispatcher.");
                DispatcherHelper.RunDispatcher();
            }
            catch (Exception e)
            {
                if (String.Equals(e.InnerException.Message, "Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.", StringComparison.InvariantCulture))
                {
                    CaughtCorrectException = true;
                }
                else
                {
                    GlobalLog.LogStatus("ERROR: Caught unexpected exception: " + e.Message + "\n" + e.StackTrace);
                }

            }
            finally
            {
                Verification(CaughtCorrectException, 1, 1);
            }
        }

        /// <summary>
        /// Implement the senaro defined in the line 3 on the spec.
        /// </summary>
        public void UnhandledException3()
        {
            GlobalLog.LogStatus("Test Unhandled Exception for Dispatcher.");
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledExceptionFilter : case 3.");
            dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(HandlerOnUnhandledExceptionFilterNotRequestCatch);
            dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(HandlerOnUnhandledExceptionFilterRequestCatch);
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledException.");
            dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandlerOnUnhandledExceptionNotHandled);
            dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandlerOnUnhandledExceptionHandled);
            bool CaughtCorrectException = false;
            try
            {
                dispatcher.Invoke(DispatcherPriority.Normal,
                    new NoArgCallback(ThrowAnException));
                GlobalLog.LogStatus("Run Dispatcher.");
                DispatcherHelper.RunDispatcher();
            }
            catch (Exception e)
            {
                if (String.Equals(e.InnerException.Message, "Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.", StringComparison.InvariantCulture))
                {
                    CaughtCorrectException = true;
                }
                else
                {
                    GlobalLog.LogStatus("ERROR: Caught unexpected exception: " + e.Message + "\n" + e.StackTrace);
                }
            }
            finally
            {
                Verification(CaughtCorrectException, 0, 2);
            }
        }

        /// <summary>
        /// Implement the senaro defined in the line 4 on the spec.
        /// </summary>
        public void UnhandledException4()
        {
            GlobalLog.LogStatus("Test Unhandled Exception for Dispatcher.");
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledExceptionFilter : case 4.");
            dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(HandlerOnUnhandledExceptionFilterRequestCatch);
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledException.");
            dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandlerOnUnhandledExceptionHandled);
            dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandlerOnUnhandledExceptionNotHandled);
            bool CaughtCorrectException = true;

            dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new NoArgCallback(ThrowAnException));
            GlobalLog.LogStatus("Run Dispatcher.");
            try
            {
                DispatcherHelper.RunDispatcher();
            }
            catch (Exception e)
            {
                // should be no exception here.
                CaughtCorrectException = false;
                GlobalLog.LogStatus("ERROR: Caught unexpected exception: " + e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                Verification(CaughtCorrectException, 1, 1);
            }
        }
        /// <summary>
        /// Implement the senaro defined in the line 5 on the spec.
        /// </summary>
        //[TestCasePriority("1")]
        //[CoreTestsLoader(CoreTestsTestType.MethodBase)]
        //[TestCaseArea(@"Dispatcher\UnhandledException\5")]
        //[TestCaseMethod("TestCase5")]
        //[TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        public void UnhandledException5()
        {
            GlobalLog.LogStatus("Test Unhandled Exception for Dispatcher.");            

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledExceptionFilter.");
            dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(HandlerOnUnhandledExceptionFilterNotRequestCatchPushPrame);
            GlobalLog.LogStatus("Adding EventHandler on Dispatcher.UnhandledException.");
            dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandlerOnUnhandledExceptionHandledPushFrame);
            bool CaughtCorrectException = false;
            try
            {
                dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new NoArgCallback(ThrowAnException));
                GlobalLog.LogStatus("Run Dispatcher.");
                DispatcherHelper.RunDispatcher();
            }
            catch (Exception e)
            {
                if (String.Equals(e.InnerException.Message, "Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.", StringComparison.InvariantCulture))
                {
                    CaughtCorrectException = true;
                }
                else
                {
                    GlobalLog.LogStatus("ERROR: Caught unexpected exception: " + e.Message + "\n" + e.StackTrace);
                }
            }
            finally
            {
                Verification(CaughtCorrectException, 0, 1);
            }
        }
        #endregion


        #region Private Members
        private void Verification(bool CaughtCorrectException, int numverOfHandleronUnhanderedEventShouldInvoke, int numverOfHandleronUnhanderedEventFilterShouldInvoke)
        {
            GlobalLog.LogStatus("Begin Verification");
            if (_numverOfHandleronUnhanderedEventInvoked < numverOfHandleronUnhanderedEventShouldInvoke)
            {
                throw new Microsoft.Test.TestValidationException("Number of handler invoked on UnhandledException should be larger than or equal: " + numverOfHandleronUnhanderedEventShouldInvoke + ", actually: " + _numverOfHandleronUnhanderedEventInvoked + ".");
            }
            if (_numverOfHandleronUnhanderedEventFilterInvoked < numverOfHandleronUnhanderedEventFilterShouldInvoke)
            {
                throw new Microsoft.Test.TestValidationException("Number of handler invoked on UnhandledExceptionFilter should be larger than or equal: " + numverOfHandleronUnhanderedEventFilterShouldInvoke + ", actually: " + _numverOfHandleronUnhanderedEventFilterInvoked + ".");
            }
            if (!CaughtCorrectException)
            {
                throw new Microsoft.Test.TestValidationException("Wrong exception caught.");
            }
            GlobalLog.LogStatus("End Verification: Success");
        }
        private void HandlerOnUnhandledExceptionFilterRequestCatch(object sender, DispatcherUnhandledExceptionFilterEventArgs args)
        {
            // 
            string expectedExceptionText = "Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.";
            GlobalLog.LogStatus("-1-- In HandlerOnUnhandledExceptionFilterRequestCatch");
            Microsoft.Test.TestSetupException exception = args.Exception.InnerException as Microsoft.Test.TestSetupException;
            args.RequestCatch = true;
            GlobalLog.LogStatus("-1a-- " + _numverOfHandleronUnhanderedEventFilterInvoked);
            _numverOfHandleronUnhanderedEventFilterInvoked += 1;
            GlobalLog.LogStatus("-1a-- " + _numverOfHandleronUnhanderedEventFilterInvoked);
            if (!String.Equals(exception.Message, expectedExceptionText, StringComparison.InvariantCulture))
            {
                throw new Microsoft.Test.TestValidationException("In handler on UnhandledExceptionFilter, the exception is not the one expected. \nExpected:" + expectedExceptionText + "\nactual: " + exception.Message);
            }
            //Check stack trace
            CheckMostRecentStackTrace("ExceptionFilter");
        }
        private void HandlerOnUnhandledExceptionFilterNotRequestCatchPushPrame(object sender, DispatcherUnhandledExceptionFilterEventArgs args)
        {
            CheckMostRecentStackTrace("ExceptionFilter");
            HandlerOnUnhandledExceptionFilterNotRequestCatch(sender, args);
            DispatcherFrame frame = new DispatcherFrame();
            DispatcherHelper.BeginInvokeWrapper(Dispatcher.CurrentDispatcher, DispatcherPriority.Background, new DispatcherOperationCallback(DiscontinueFrame), null);
            DispatcherHelper.PushFrame(frame);
        }

        private object DiscontinueFrame(object  frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }

        private void HandlerOnUnhandledExceptionFilterNotRequestCatch(object sender, DispatcherUnhandledExceptionFilterEventArgs args)
        {
            GlobalLog.LogStatus("-2-- In HandlerOnUnhandledExceptionFilterNotRequestCatch");
            Microsoft.Test.TestSetupException exception = args.Exception.InnerException as Microsoft.Test.TestSetupException;            
            args.RequestCatch = false;
            _numverOfHandleronUnhanderedEventFilterInvoked += 1;
            if (!String.Equals(exception.Message, "Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.", StringComparison.InvariantCulture))
            {
                throw new Microsoft.Test.TestValidationException("In handler on UnhandledExceptionFilter, the exception is not the one expected.");
            }
            //Check stack trace
            CheckMostRecentStackTrace("ExceptionFilter");
        }

        private void HandlerOnUnhandledExceptionHandledPushFrame(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Microsoft.Test.TestSetupException exception = args.Exception.InnerException as Microsoft.Test.TestSetupException;
            if (!String.Equals(exception.Message, "Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.", StringComparison.InvariantCulture))
            {
                throw new Microsoft.Test.TestValidationException("In handler on UnhandledException, the exception is not the one expected.");
            }
            if (_numverOfHandleronUnhanderedEventInvoked == 0)
            {
                throw new Microsoft.Test.TestValidationException("UnhandledExceptionFilter should be invoked before UnhandledException.");
            }
            args.Handled = true;
            _numverOfHandleronUnhanderedEventInvoked += 1;
            
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            DispatcherFrame frame = new DispatcherFrame();

            DispatcherHelper.ShutDownPriorityBackground(dispatcher);
            DispatcherHelper.PushFrame(frame);
            CheckMostRecentStackTrace("CatchException");
        }

        private void HandlerOnUnhandledExceptionHandled(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Microsoft.Test.TestSetupException exception = args.Exception.InnerException as Microsoft.Test.TestSetupException;
            if (!String.Equals(exception.Message, "Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.", StringComparison.InvariantCulture))
            {
                throw new Microsoft.Test.TestValidationException("In handler on UnhandledException, the exception is not the one expected.");
            }
            if (_numverOfHandleronUnhanderedEventFilterInvoked == 0)
            {
                throw new Microsoft.Test.TestValidationException("UnhandledExceptionFilter should be invoked before UnhandledException.");
            }
            args.Handled = true;
            _numverOfHandleronUnhanderedEventInvoked += 1;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            //Check stack trace
            CheckMostRecentStackTrace("CatchException");            
            DispatcherHelper.ShutDownPriorityBackground(dispatcher);
        }
        private void CheckMostRecentStackTrace(string frameMethodName)
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            if (st != null)
            {
                StackFrame[] frames = st.GetFrames();
                int i;
                for (i = 0; i < frames.Length; i++)
                {
                    if (frames[i].GetMethod().Name.Contains("HandlerOn"))
                        break;
                }
                if (frames.Length > (i + 2)) // So as to avoid the IndexOutOfBounds exception that has been silently plaguing this test for a while... 
                {
                    if (!frames[i + 1].GetMethod().Name.Contains(frameMethodName) && !frames[i + 2].GetMethod().Name.Contains(frameMethodName))
                    {
                        throw new Microsoft.Test.TestValidationException("Most recent frame is not: " + frameMethodName + ".");
                    }
                }
                else
                {
                    GlobalLog.LogStatus("TODO: Fix faulty validation code that checks for frames outside of the current stacktrace's range");
                }
            }
            else
            {
                GlobalLog.LogStatus("CheckMostRecentStackTrace got a null stacktrace!");
            }
            return;
        }

        private void HandlerOnUnhandledExceptionNotHandled(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Microsoft.Test.TestSetupException exception = args.Exception.InnerException as Microsoft.Test.TestSetupException;
            if (!String.Equals(exception.Message, "Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.", StringComparison.InvariantCulture))
            {
                throw new Microsoft.Test.TestValidationException("In handler on UnhandledException, the exception is not the one expected.");
            }
            if (_numverOfHandleronUnhanderedEventFilterInvoked == 0)
            {
                throw new Microsoft.Test.TestValidationException("UnhandledExceptionFilter should be invoked before UnhandledException.");
            }
            args.Handled = false;
            _numverOfHandleronUnhanderedEventInvoked += 1;
            CheckMostRecentStackTrace("CatchException");
        }

        private object ThrowAnException()
        {
            throw new Microsoft.Test.TestSetupException("Exception thrown inside Dispatcher.Invoke / Dispatcher.BeginInvoke.");
        }
        #endregion
    }

    /// <summary>
    /// A delegate without any parameter.
    /// </summary>
    /// <returns></returns>
    public delegate object NoArgCallback();   
}


