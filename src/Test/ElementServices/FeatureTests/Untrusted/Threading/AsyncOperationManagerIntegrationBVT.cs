// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
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
    /// BVTs for testing DispatcherSynchronizationContext in Avalon
    /// Testing calling Send or Post to a stored SyncContext from other thread works.
    /// </summary>
    [Test(0, "Threading.SyncContext", TestCaseSecurityLevel.PartialTrust, "AsyncOperationManagerIntegrationBVT")]
    public class AsyncOperationManagerIntegrationBVT : AvalonTest
    {

        #region Constructor
        /******************************************************************************
        * Function:          AsyncOperationManagerIntegrationBVT Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public AsyncOperationManagerIntegrationBVT()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// </summary>
        TestResult StartTest()
        {
            DispatcherHelper.EnqueueBackgroundCallback((DispatcherOperationCallback)delegate(object o)
                {
                    FakeReader fr = new FakeReader();
                    fr.StartCompleted += (EventHandler)delegate(object o1, EventArgs args)
                        {
                            AsyncOperationHelper.IsHandlerExecuted = true;
                            int[] iArray = (int[])(((AsyncCompletedEventArgs)args).UserState);

                            for (int i =1; i<=10;i++)
                            {
                                if ( iArray[i] != i)
                                {
                                    CoreLogger.LogTestResult(false,"Value is not the expected.");
                                }
                            }

                            if (((AsyncCompletedEventArgs)args).Error != null)
                            {
                                CoreLogger.LogTestResult(false,"An exception was caught");
                                CoreLogger.LogStatus(((AsyncCompletedEventArgs)args).Error.StackTrace.ToString());
                            }

                            if (AsyncOperationHelper.Dispatcher != Dispatcher.CurrentDispatcher)
                            {
                                CoreLogger.LogTestResult(false,"The dispatcher doesn't match.");
                            }
                            DispatcherHelper.ShutDownPriorityBackground(AsyncOperationHelper.Dispatcher);
                        };

                    fr.Start(false);

                    return null;
                }, null);

            AsyncOperationHelper.Dispatcher = Dispatcher.CurrentDispatcher;
            DispatcherHelper.RunDispatcher();

            if (!AsyncOperationHelper.IsHandlerExecuted)
            {
                CoreLogger.LogTestResult(false,"The handler is not executed.");
            }            

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }

    /******************************************************************************
    * CLASS:          AsyncOperationHelper
    ******************************************************************************/
    /// <summary>
    /// Testing calling Send or Post to a stored SyncContext from other thread works.
    /// </summary>
    public static class AsyncOperationHelper
    {
        /// <summary>
        /// </summary>
        public static bool IsHandlerExecuted = false;

        /// <summary>
        /// </summary>
        public static Dispatcher Dispatcher = null;

    }

    /******************************************************************************
    * CLASS:          FakeReader
    ******************************************************************************/
    /// <summary>
    /// </summary>
    public class FakeReader
    {
        static int s_count = 0;

        static object s_syncRootGlobal = new Object();
        
        /// <summary>
        /// </summary>
        public void Start(bool isSend)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(this);
            ThreadPool.QueueUserWorkItem(delegate (object o)
                {
                    Exception exp = null;
                    
                    AsyncOperation asyncOperation = (AsyncOperation)o;            
                    int[] iArray = new int[11];


                    lock(s_syncRootGlobal)
                    {
                        s_count++;
                        iArray[0] = s_count;
                    }
                    
                    try
                    {
                            
                        for (int i =1; i<=10;i++)
                        {
                            iArray[i] = i;
                            Thread.Sleep(200);
                        }
                    }
                    catch(Exception e)
                    {
                        exp = e;
                    }

                    AsyncCompletedEventArgs arg = new AsyncCompletedEventArgs(exp,false, iArray);

                    if (!isSend)
                    {
                        asyncOperation.Post((SendOrPostCallback) delegate (object o1)
                            {
                                OnStartCompleted((AsyncCompletedEventArgs)o1);
                            }, arg);
                    }
                    else
                    {
                       /* asyncOperation.Send((SendOrPostCallback) delegate (object o1)
                            {
                                OnStartCompleted((AsyncCompletedEventArgs)o1);
                            }, arg);
                            */
                    }
                },
                asyncOp);
        }

        public event EventHandler StartCompleted;


        private void OnStartCompleted(AsyncCompletedEventArgs arg)
        {
            if (StartCompleted != null)
            {
                StartCompleted(this, arg);
            }
        }
    }
}


