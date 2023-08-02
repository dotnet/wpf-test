// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          AsyncOperationManagerIntegrationBVT
    ******************************************************************************/
    /// <summary>
    /// BVT to test that the DIspatcherHooks work appropiately.
    /// Making sure that by default DispatcherHooks is null.  Pay for Play.
    /// </summary>
    [Test(0, "Threading.Operation", TestCaseSecurityLevel.FullTrust, "DispatcherHooks")]
    public class TestsDispatcherHooks : AvalonTest
    {
        #region Private Data
        private static object s_globalSyncRoot = new object();
        private static List<DispatcherHooks> s_hooksList = new List<DispatcherHooks>();
        private static ManualResetEvent s_ev = new ManualResetEvent(false);
        private DispatcherQueueWrapper _queueWrapper;
        private Dispatcher _dispatcher = null;
        private int _isDispatcherInactiveFired = 0;
        private DispatcherFrame _frame = null;
        private bool _isExitRequested = false;
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("DispatcherHooksPayForPlay")]
        [Variation("DispatcherInactiveEvent")]

        /******************************************************************************
        * Function:          TestsDispatcherHooks Constructor
        ******************************************************************************/
        public TestsDispatcherHooks(string arg)
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
                case "DispatcherHooksPayForPlay":
                    DispatcherHooksPayForPlay();
                    break;
                case "DispatcherInactiveEvent":
                    DispatcherInactiveEvent();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /******************************************************************************
        * Function:          DispatcherHooksPayForPlay
        ******************************************************************************/
        /// <summary>
        /// </summary>
        public void DispatcherHooksPayForPlay()
        {
            FieldInfo field =  typeof(Dispatcher).GetField("_hooks", 
                BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);

            object oValue = field.GetValue(Dispatcher.CurrentDispatcher);

            if (oValue != null)
            {
                CoreLogger.LogTestResult(false,"The Dispatcher._hooks should be null.");
            }
        }

        /******************************************************************************
        * Function:          DispatcherInactiveEvent
        ******************************************************************************/
        ///<summary>
        /// Listen for Dispatcher Inactive event when the queue is empty twice.
        ///</summary>
        public void DispatcherInactiveEvent()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _queueWrapper = new DispatcherQueueWrapper(_dispatcher);
            DispatcherHelper.GetHooks().DispatcherInactive += 
                delegate
                {       
                    CoreLogger.LogStatus("DispatcherHooks:Inactive called.");                    

                    // Counter for Inactive events
                    _isDispatcherInactiveFired++;
                };

            Post();

            _frame = new DispatcherFrame();
            DispatcherHelper.PushFrame(_frame);

            // We are waiting for 3 Inactive events.  We received 3 events because
            // the last one comes from that we wake up the dispatcher that it is 
            // blocked. I just add a range.
            if (_isDispatcherInactiveFired < 3 || _isDispatcherInactiveFired > 6)
            {
                CoreLogger.LogTestResult(false,"DispatcherHook.Inactive event was fired different than 3 times. Actual: " + _isDispatcherInactiveFired.ToString());
            }
            
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          Post
        ******************************************************************************/
        private void Post()
        {
            for (int i = 1; i <= 100; i++)
            {
                DispatcherPriority dp = Microsoft.Test.Threading.DispatcherPriorityHelper.GetRandomValidDispatcherPriorityForDispatcherInvoking();

                DispatcherHelper.EnqueueCallback(
                    _dispatcher,
                    dp, 
                    delegate (object o)
                    {

                        // We know that there are not more items on the queue to 
                        // process. We can set our flag thet lastEnqueuedItem is been processed.
                        if (_queueWrapper.MaxPriority == DispatcherPriority.Inactive)
                        {
                            CoreLogger.LogStatus("Last Item been proccessed.");

                            if (_isExitRequested)
                            {

                                // Start a timer to stop the dispatcher.
                                CoreLogger.LogStatus("Setting a timer to exit the nested pump.");
                                System.Timers.Timer timer = new System.Timers.Timer();
                                timer.Elapsed += delegate (object oa, System.Timers.ElapsedEventArgs args)
                                    {
                                        ((System.Timers.Timer)oa).Stop();
                                        _frame.Continue = false;
                                        
                                    };
                                timer.Interval = 5000;
                                timer.Start();
                                

                            }
                            else
                            {
                                // We post a timer for a second round of Posting 100 items.
                                // In this way we could get a second Inactive event.
                                System.Timers.Timer timer = new System.Timers.Timer();
                                timer.Elapsed += delegate(object oa, System.Timers.ElapsedEventArgs args)
                                    {
                                       ((System.Timers.Timer)oa).Stop();
                                        Post();
                                        _isExitRequested = true;
                                        CoreLogger.LogStatus("Exit Requested");
                                    };
                                timer.Interval = 2000;
                                timer.Start();
                                
                            }
                        }
                        return null;
                    }, null);
            }
        }
        #endregion
    }
}

