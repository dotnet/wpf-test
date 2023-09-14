// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          DispatcherMessageLoopServiceBVT
    ******************************************************************************/
    /// <summary>
    /// BVT to test that the DIspatcherHooks work appropiately.
    /// Making sure that by default DispatcherHooks is null.  Pay for Play.
    /// </summary>
    [Test(0, "Threading.MessageLoopService", TestCaseSecurityLevel.FullTrust, "DispatcherMessageLoopServiceBVT")]
    public class DispatcherMessageLoopServiceBVT : AvalonTest
    {
        #region Private Data
        private int _count = 0; // Nesting Count.
        private DispatcherFrame _frame = null;
        private AutoResetEvent _ev = new AutoResetEvent(false);
        private bool _called = false;
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("InvalidPushFrameCalls")]
        [Variation("InvalidPushFrameCallsonShutdownFinished")]
        [Variation("ExitAllFramesNestedLoopsTrue")]
        [Variation("ExitAllFramesNestedLoopsTrueOneFalse")]
        [Variation("ExitingOnShutdown")]
        [Variation("WakeupDispatcherAndLeaveFromDifferentThread")]
        [Variation("ReenteringDispatcherLoopAfterExitAllFrameCall")]
        [Variation("ReusingDispatcherFrameonMultipleThreadsPushFrame")]

        /******************************************************************************
        * Function:          DispatcherMessageLoopServiceBVT Constructor
        ******************************************************************************/
        public DispatcherMessageLoopServiceBVT(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "InvalidPushFrameCalls":
                    InvalidPushFrameCalls();
                    break;
                case "InvalidPushFrameCallsonShutdownFinished":
                    InvalidPushFrameCallsonShutdownFinished();
                    break;
                case "ExitAllFramesNestedLoopsTrue":
                    ExitAllFramesNestedLoopsTrue();
                    break;
                case "ExitAllFramesNestedLoopsTrueOneFalse":
                    ExitAllFramesNestedLoopsTrueOneFalse();
                    break;
                case "ExitingOnShutdown":
                    ExitingOnShutdown();
                    break;
                case "WakeupDispatcherAndLeaveFromDifferentThread":
                    WakeupDispatcherAndLeaveFromDifferentThread();
                    break;
                case "ReenteringDispatcherLoopAfterExitAllFrameCall":
                    ReenteringDispatcherLoopAfterExitAllFrameCall();
                    break;
                case "ReusingDispatcherFrameonMultipleThreadsPushFrame":
                    ReusingDispatcherFrameonMultipleThreadsPushFrame();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public and Private Members
        /******************************************************************************
        * Function:          InvalidPushFrameCalls
        ******************************************************************************/
        ///<summary>
        /// Validates PushFrame is called with invalid parameters.
        ///</summary>
        public void InvalidPushFrameCalls()
        {
            bool exceptionThrown = false;
            
            try
            {
                Dispatcher.PushFrame(null);
            }
            catch(ArgumentNullException)
            {
                exceptionThrown = true;
            }

            if (!exceptionThrown)
            {
                GlobalLog.LogEvidence("FAIL: ArgNullException expected.");
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
            
            exceptionThrown = false;
            
            try
            {
                Dispatcher.PushFrame(new DispatcherFrame());
            }
            catch(InvalidOperationException)
            {
                exceptionThrown = true;
            }

            if (!exceptionThrown)
            {
                GlobalLog.LogEvidence("FAIL: InvalidOperationException expected. The Dispatcher has been shut down.");
            }
            
        }
        
        /******************************************************************************
        * Function:          InvalidPushFrameCallsonShutdownFinished
        ******************************************************************************/
        ///<summary>
        /// Validates PushFrame is not possible to called on ShutdownFinished event.
        ///</summary>
        public void InvalidPushFrameCallsonShutdownFinished()
        {
            Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
        
            Dispatcher.CurrentDispatcher.ShutdownFinished += delegate
                {
                    DispatcherFrame frame = new DispatcherFrame(false);
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (DispatcherOperationCallback)delegate (object o)
                        {
                            ((DispatcherFrame)o).Continue = false;

                            _called = true;
                            
                            return null;
                        }, frame);

                    Dispatcher.PushFrame(frame);                   
                };
            
            Dispatcher.Run();

            if (!_called)
            {
                GlobalLog.LogEvidence("FAIL: The test code was not called.");
            }
            
        }

        /******************************************************************************
        * Function:          ExitAllFramesNestedLoopsTrue
        ******************************************************************************/
        ///<summary>
        /// ExitAllFrames with all DispatcherFrames = true.
        ///</summary>
        public void ExitAllFramesNestedLoopsTrue()
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestOneN1));
        }

        private void NestOneN1(object o, EventArgs args)
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestTwoN1));
        }

        private void NestTwoN1(object o, EventArgs args)
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestThreeN1));
        }

        private void NestThreeN1(object o, EventArgs args)
        {
            _frame = (DispatcherFrame)o;
            DispatcherHelper.ExitFrames();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal,
                (EventHandler)delegate
                {
                    throw new Exception("We should not dispatched this message.");
                    
                },
                null, null);
        }

        /******************************************************************************
        * Function:          ExitAllFramesNestedLoopsTrueOneFalse
        ******************************************************************************/
        ///<summary>
        /// Nesting pump calling ExitAllFrames with intermediate DispatcherFrame = false.
        ///</summary>
        public void ExitAllFramesNestedLoopsTrueOneFalse()
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestOneN2));
        }

        private void NestOneN2(object o, EventArgs args)
        {
            _frame = new DispatcherFrame(false);
            DispatcherHelper.EnterLoopNormal(_frame, new EventHandler(NestTwoN2));
        }

        private void NestTwoN2(object o, EventArgs args)
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestThreeN2));
        }

        private void NestThreeN2(object o, EventArgs args)
        {
            DispatcherFrame frame = (DispatcherFrame)o;
            DispatcherHelper.ExitFrames();
            DispatcherHelper.EnqueueNormalCallback(
                (DispatcherOperationCallback)delegate(object x)
                {
                    _frame.Continue = false;
                    return null;
                },
                null);
        }

        /******************************************************************************
        * Function:          ExitingOnShutdown
        ******************************************************************************/
        ///<summary>
        /// Validating exiting after calling Shutdown.
        ///</summary>
        public void ExitingOnShutdown()
        {
            DispatcherHelper.ValidHasShutdownStatus(false, false);
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestOneN3));
            DispatcherHelper.ValidHasShutdownStatus(true, true);
        }

        private void NestOneN3(object o, EventArgs args)
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestTwoN3));
            DispatcherHelper.ValidHasShutdownStatus(true, false);
        }

        private void NestTwoN3(object o, EventArgs args)
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestThreeN3));
            DispatcherHelper.ValidHasShutdownStatus(true, false);
        }

        private void NestThreeN3(object o, EventArgs args)
        {
            DispatcherHelper.ValidHasShutdownStatus(false, false);
            DispatcherHelper.ShutDown();
            DispatcherHelper.ValidHasShutdownStatus(true, false);

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal,
                (EventHandler)delegate
                {
                    throw new Exception("We should not dispatched this message.");

                },
                null, null);
            
        }

        /******************************************************************************
        * Function:          WakeupDispatcherAndLeaveFromDifferentThread
        ******************************************************************************/
        /// <summary>
        /// DispatcherThread is sleeping, other thread ask to leave using
        /// DispatcherFrame.Continue = false;
        /// </summary>
        public void WakeupDispatcherAndLeaveFromDifferentThread()
        {
           DispatcherFrame frame = new DispatcherFrame(true);
           ThreadPool.QueueUserWorkItem(delegate(object o)
            {
                DispatcherFrame pushedFrame = (DispatcherFrame)o;
                Thread.Sleep(10000);
                _count++;
                pushedFrame.Continue = false;

            }, frame);
           DispatcherHelper.PushFrame(frame);  

            if (_count < 1)
            {
                GlobalLog.LogEvidence("FAIL: The Dispatcher exit the loop for the wrong reason.");
            }
        }

        /******************************************************************************
        * Function:          ReenteringDispatcherLoopAfterExitAllFrameCall
        ******************************************************************************/
        /// <summary>
        /// Entering Multiple nested loops and exiting multiple times.
        /// </summary>
        public void ReenteringDispatcherLoopAfterExitAllFrameCall()
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestOneN5));
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestOneN5));
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestOneN5));
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestOneN5));
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestOneN5));
            if (_count != 5)
            {
                GlobalLog.LogEvidence("FAIL: Expecting 5. Actual: " + _count.ToString());
            }
            DispatcherHelper.ValidHasShutdownStatus(false, false);
        }

        private void NestOneN5(object o, EventArgs args)
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestTwoN5));
        }

        private void NestTwoN5(object o, EventArgs args)
        {
            DispatcherHelper.EnterLoopNormal(true, new EventHandler(NestThreeN5));
        }

        private void NestThreeN5(object o, EventArgs args)
        {
            _count++;
            DispatcherFrame frame = (DispatcherFrame)o;
            DispatcherHelper.ExitFrames();            
        }

        /******************************************************************************
        * Function:          ReusingDispatcherFrameonMultipleThreadsPushFrame
        ******************************************************************************/
        /// <summary>
        /// Reusing a DispatcherFrame on multiple Dispatcher threads.
        /// </summary>
        public void ReusingDispatcherFrameonMultipleThreadsPushFrame()
        {

            ThreadCaseInfo threadOne = new ThreadCaseInfo();
            threadOne.ThreadStarted += delegate
                {           
                    _ev.WaitOne();

                    bool exceptionThrown = false;
                    
                    try
                    {                    
                        DispatcherHelper.PushFrame(_frame);
                    }
                    catch(InvalidOperationException)
                    {
                        exceptionThrown = true;
                    }
                    finally
                    {
                        _ev.Set();
                    }

                    if (!exceptionThrown)
                    {
                        GlobalLog.LogEvidence("FAIL: Expecting an exception");
                    }                                                           
                };

            ThreadCaseInfo threadTwo = new ThreadCaseInfo();
            threadTwo.ThreadStarted += delegate
                {                
                    _frame = new DispatcherFrame();
                    _ev.Set();
                    Thread.Sleep(500);
                    _ev.WaitOne();                
                };

            ThreadCaseInfo[] caseInfos = {threadOne,threadTwo};
            
            MultipleThreadTestCase testCase = new MultipleThreadTestCase(ThreadCaseSynchronization.None);
            testCase.Initialize(caseInfos);
            testCase.Run();

            if (testCase.ExceptionList.Length != 0)
            {
                GlobalLog.LogEvidence("FAIL: An unexpected exception was caught");                
            }
        }
        #endregion
    }
}





