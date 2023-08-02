// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          DispatcherSyncContextBVT
    ******************************************************************************/
    /// <summary>
    /// Main Class that holds the Misc tests for Dispatcher Class
    /// </summary>
    [Test(0, "Threading.SyncContext", "DispatcherSyncContextBVT")]
    public class DispatcherSyncContextBVT : AvalonTest
    {
        #region Data
        public delegate void TestCallback(object o);
        private static bool s_isSend = true;
        private static SynchronizationContext s_syncContext = null;
        private static Dispatcher s_dispatcher = null;
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("CreateCopySyncContext")]
        [Variation("CallingSendPostFromWorkerThreadPost")]
        [Variation("CallingSendPostFromWorkerThreadSend")]

        /******************************************************************************
        * Function:          DispatcherSyncContextBVT Constructor
        ******************************************************************************/
        public DispatcherSyncContextBVT(string arg)
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
                case "CreateCopySyncContext":
                    CreateCopySyncContext();
                    break;
                case "CallingSendPostFromWorkerThreadPost":
                    CallingSendPostFromWorkerThread("post");
                    break;
                case "CallingSendPostFromWorkerThreadSend":
                    CallingSendPostFromWorkerThread("send");
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
        * Function:          CreateCopySyncContext
        ******************************************************************************/
        /// <summary>
        /// Testing calling Send or Post to a stored SyncContext from other thread works.
        /// Copy the DispatcherSynchronizationContext.
        /// </summary>
        public void CreateCopySyncContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;
            
            DispatcherHelper.EnqueueBackgroundCallback((DispatcherOperationCallback) delegate(object o)
                {
                    
                    SynchronizationContext syncCtx = SynchronizationContext.Current;

                    CoreLogger.LogStatus("Calling CreateCopy.");                    
                    s_syncContext = syncCtx.CreateCopy();                                        

                    s_syncContext.Post((SendOrPostCallback) delegate(object o1)
                        {
                            if (s_dispatcher != Dispatcher.CurrentDispatcher)
                            {
                                 CoreLogger.LogTestResult(false,"The Dispatcher doesn't match with the expected.");
                            }

                            CoreLogger.LogStatus("ShutDown the dispatcher.");                                              
                            DispatcherHelper.ShutDownPriorityBackground(s_dispatcher);
                        }, null);
                    
                    return null;
                }, null);
            
            DispatcherHelper.RunDispatcher();
            
        }

        /******************************************************************************
        * Function:          CallingSendPostFromWorkerThread
        ******************************************************************************/
        /// <summary>
        /// Testing calling Send or Post to a stored SyncContext from other thread works.
        /// Calling SyncContext.Post from a WorkerThread.
        /// </summary>
        public void CallingSendPostFromWorkerThread(string param)
        {
            if (param == "post")
            {
                s_isSend = false;
            }
            
            WorkerThreadTestCase testCase = new WorkerThreadTestCase(true);
            testCase.Initialize((ThreadTestCaseCallback)delegate(ThreadCaseInfo info, EventArgs args)
                {
                    if (SynchronizationContext.Current is DispatcherSynchronizationContext)
                    {
                        CoreLogger.LogTestResult(false,"The SyncContext is not the expected.");
                    }

                }, (ThreadTestCaseCallback)delegate(ThreadCaseInfo info, EventArgs args)
                {
                    s_dispatcher = Dispatcher.CurrentDispatcher;
                    s_syncContext = SynchronizationContext.Current;
                    // Making sure we yield to the other thread.
                    Thread.Sleep(0);
                    // Simulating the dispatcher is busy.
                    Thread.Sleep(4000);
                    
                }, 
                (ThreadTestCaseCallback)delegate(ThreadCaseInfo info, EventArgs args)
                {
                    // Dispatcher already exists.
                    if (SynchronizationContext.Current is DispatcherSynchronizationContext)
                    {
                        CoreLogger.LogTestResult(false,"The SyncContext was not restore.");
                    }
                }, 
                (ThreadTestCaseCallback) delegate(ThreadCaseInfo info, EventArgs args)
                {
                    while(s_syncContext == null)
                    {
                        Thread.Sleep(0);
                    }

                    
                    SynchronizationContext workerThreadSyncContext = SynchronizationContext.Current;
                    Stopwatch watch = Stopwatch.StartNew();
                    object[] oArray = {watch, workerThreadSyncContext};

                    if (s_isSend)
                    {
                        CoreLogger.LogStatus("Calling SyncContext.Send");                    
                        // The dispatcher is been simulated busy.
                        s_syncContext.Send(new SendOrPostCallback(HandlerOne), oArray);
                    }
                    else
                    {
                        CoreLogger.LogStatus("Calling SyncContext.Post");                    
                        // The dispatcher is been simulated busy.
                        s_syncContext.Post(new SendOrPostCallback(HandlerOne), oArray);
                    }

                    if (s_isSend && watch.ElapsedMilliseconds < 3800)
                    {
                        CoreLogger.LogTestResult(false,"The send operation didn't work as expected. Expected Time: Above 3800; Real Time: " + watch.ElapsedMilliseconds.ToString());
                    } else if (!s_isSend && watch.ElapsedMilliseconds > 1000)
                    {
                        CoreLogger.LogTestResult(false,"The Post operation didn't work as expected. Expected Time: Below 1000; Real Time: " + watch.ElapsedMilliseconds.ToString());
                    }

                }, 
                null);

            testCase.Run();
            
            if (testCase.ExceptionList.Length > 0)
            {
                CoreLogger.LogTestResult(false,"An exception was caught.");                                
            }
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          HandlerOne
        ******************************************************************************/
        private void HandlerOne(object o)
        {
            object[] objArray = (object[])o;
            Stopwatch w = (Stopwatch)(objArray[0]);
            w.Stop();

            SynchronizationContext oldWorkerThreadSyncContext = (SynchronizationContext)(objArray[1]);

            if (w.ElapsedMilliseconds < 3800)
            {
                CoreLogger.LogTestResult(false,"The operation didn't work as expected. Expected Time: Above 3800; Real Time: " + w.ElapsedMilliseconds.ToString());
            }

            if (s_dispatcher != Dispatcher.CurrentDispatcher)
            {
                CoreLogger.LogTestResult(false,"Dispatchers doesn't match.");
            }

            SynchronizationContext currentSyncContext = SynchronizationContext.Current;

            if ( !(currentSyncContext is DispatcherSynchronizationContext))
            {
                CoreLogger.LogTestResult(false,"The type of SyncContext is not the expected one.");
            }

            if (currentSyncContext == oldWorkerThreadSyncContext)
            {
                CoreLogger.LogTestResult(false,"The current SyncContext is not the expected.");                                
            }            

            DispatcherHelper.ShutDownPriorityBackground(s_dispatcher);

        }
        #endregion
    }
}
