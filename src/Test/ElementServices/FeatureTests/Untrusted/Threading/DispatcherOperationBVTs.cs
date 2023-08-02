// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;

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
    * CLASS:          DispatcherOperationBVTs
    ******************************************************************************/
    [Test(0, "Threading.Operation", "DispatcherOperationBVTs")]
    public class DispatcherOperationBVTs : TestCaseBase
    {
        #region Private Data
        private static object s_rootSync = new Object();
        private WorkerThreadTestCase _testCase =  null;
        private DispatcherOperation _dispatcherOperation = null;
        private int _itemsCount = 0;
        private string _testName = "";
        #endregion


        #region Constructor

        [Variation("Operation_CompleteTwoThreads")]
        [Variation("Operation_CompleteTwoThreadsTimeout")]
        [Variation("Operation_Complete_SingleThread")]
        [Variation("Operation_CompleteTimeout_SingleThread")]
        [Variation("Operation_CompleteAborted_SingleThread")]

        /******************************************************************************
        * Function:          DispatcherOperationBVTs Constructor
        ******************************************************************************/
        public DispatcherOperationBVTs(string arg)
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
                case "Operation_CompleteTwoThreads":
                    Operation_CompleteTwoThreads();
                    break;
                case "Operation_CompleteTwoThreadsTimeout":
                    Operation_CompleteTwoThreadsTimeout();
                    break;
                case "Operation_Complete_SingleThread":
                    Operation_Complete_SingleThread();
                    break;
                case "Operation_CompleteTimeout_SingleThread":
                    Operation_CompleteTimeout_SingleThread();
                    break;
                case "Operation_CompleteAborted_SingleThread":
                    Operation_CompleteAborted_SingleThread();
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
        * Function:          Operation_CompleteTwoThreads
        ******************************************************************************/
        ///<summary>
        /// Worker Thread calling Complete on an operation and waiting to be dispatched.
        ///</summary>
        public void Operation_CompleteTwoThreads()
        {
            TestCaseFailed = true;

            _testCase = new WorkerThreadTestCase(true);
            _testCase.Initialize(
                new ThreadTestCaseCallback(DispatcherThreadStarted),
                null, 
                null,
                new ThreadTestCaseCallback(WorkerThreadStarted),
                null);

            _testCase.Run();
            FinalReportFailure();
        }

        private void DispatcherThreadStarted(ThreadCaseInfo o, EventArgs args)
        {
            for (int i = 0; i < 10; i++)
            {
                DispatcherHelper.EnqueueNormalCallback(Dispatcher.CurrentDispatcher, 
                    (DispatcherOperationCallback) delegate (object notUsed)
                    {
                        lock(s_rootSync)
                        {
                            CoreLogger.LogStatus("Normal Priority has been dispatched");                           
                            _itemsCount++;
                        }
                        CoreLogger.LogStatus("Sleeping 150 Milliseconds");
                        Thread.Sleep(150);

                        return null;
                    }, 
                    null);
            }


            // By this time when Idle is perform, the background priority must be processed
            CoreLogger.LogStatus("Posting a ShutDown Dispatcher a SystemIdle Priority");
            DispatcherHelper.ShutDownPrioritySystemIdle();
        }

        private void WorkerThreadStarted(ThreadCaseInfo o, EventArgs args)
        {
            _dispatcherOperation = DispatcherHelper.BeginInvokeWrapper(_testCase.CurrentDispatcher, 
                DispatcherPriority.Background, 
                (DispatcherOperationCallback) delegate (object notUsed)
                {
                    CoreLogger.LogStatus("Background Priority has been dispatched, from a worker thread");
                    lock (s_rootSync)
                    {
                        if (_itemsCount != 10)
                        {
                            throw new Microsoft.Test.TestValidationException("Operation Complete didn't wait. Dispatching");
                        }
                        TestCaseFailed = false;
                    }
                    return null;
                }, 
                null);
            
            CoreLogger.LogStatus("Calling Complete on the WorkerThread. This should wait that all Normal items get dispatched");
            _dispatcherOperation.Wait();
            CoreLogger.LogStatus("Exiting Complete on the WorkerThread. All Normal items must be dispatched");

            lock (s_rootSync)
            {

                if (_dispatcherOperation.Status != DispatcherOperationStatus.Completed)
                {
                    throw new Microsoft.Test.TestValidationException("Operation WasCompleted return false");
                }


                if (_itemsCount != 10)
                {
                    throw new Microsoft.Test.TestValidationException("Operation Complete didn't wait. After the Complete call");
                }
            }
        }

        /******************************************************************************
        * Function:          Operation_CompleteTwoThreadsTimeout
        ******************************************************************************/
        ///<summary>
        /// Worker Thread calling Complete with a timeout on an operation. Not waiting to be dispatched
        ///</summary>
        public void Operation_CompleteTwoThreadsTimeout()
        {
            TestCaseFailed = true;

            _testCase = new WorkerThreadTestCase(true);
            _testCase.Initialize(new ThreadTestCaseCallback(DispatcherThreadStartedTest2),
                null, 
                null,
                new ThreadTestCaseCallback(WorkerThreadStartedTest2),
                null);

            _testCase.Run();
            FinalReportFailure();
        }

        private void DispatcherThreadStartedTest2(ThreadCaseInfo o, EventArgs args)
        {

            for (int i = 0; i < 10; i++)
            {
                DispatcherHelper.EnqueueNormalCallback(Dispatcher.CurrentDispatcher, 
                    (DispatcherOperationCallback) delegate (object notUsed)
                    {
                        lock(s_rootSync)
                        {
                            CoreLogger.LogStatus("Normal Priority has been dispatched");                           
                            _itemsCount++;
                        }
                        CoreLogger.LogStatus("Sleeping 150 Milliseconds");
                        Thread.Sleep(150);

                        return null;
                    }, 
                    null);
            }


            // By this time when Idle is perform, the background priority must be processed
            CoreLogger.LogStatus("Posting a ShutDown Dispatcher a SystemIdle Priority");
            DispatcherHelper.ShutDownPrioritySystemIdle();
        }

        private void WorkerThreadStartedTest2(ThreadCaseInfo o, EventArgs args)
        {

            
            _dispatcherOperation = DispatcherHelper.BeginInvokeWrapper(_testCase.CurrentDispatcher, 
                DispatcherPriority.Background, 
                (DispatcherOperationCallback) delegate (object notUsed)
                {
                    CoreLogger.LogStatus("Background Priority has been dispatched, from a worker thread");
                    lock (s_rootSync)
                    {
                        if (_itemsCount != 10)
                        {
                            throw new Microsoft.Test.TestValidationException("Operation Complete didn't wait. Dispatching");
                        }
                        TestCaseFailed = false;
                    }
                    return null;
                }, 
                null);
            
            CoreLogger.LogStatus("Calling Complete with Timeout 1 sec. on the WorkerThread");
            _dispatcherOperation.Wait(TimeSpan.FromSeconds(1));
            CoreLogger.LogStatus("Exiting Complete on the WorkerThread. Not All Normal items must be dispatched");

            lock (s_rootSync)
            {

                if (_dispatcherOperation.Status == DispatcherOperationStatus.Completed)
                {
                    throw new Microsoft.Test.TestValidationException("Operation WasCompleted return true.");
                }
                
                if (_itemsCount == 10)
                {
                    throw new Microsoft.Test.TestValidationException("Operation Complete didn't wait. After the Complete call");
                }
            }
        }

        /******************************************************************************
        * Function:          Operation_CompleteTwoThreads
        ******************************************************************************/
        ///<summary>
        ///</summary>
        public void Operation_Complete_SingleThread()
        {

            DispatcherHelper.EnqueueNormalCallback(
                Dispatcher.CurrentDispatcher,
                new DispatcherOperationCallback(test1HandlerOneItemOp), 
                null);

            for (int i=0; i<10;i++)                       
            {
                DispatcherHelper.EnqueueNormalCallback(
                    Dispatcher.CurrentDispatcher,
                    new DispatcherOperationCallback(operationCompleteDispatched), 
                    null);
            }

            DispatcherHelper.RunDispatcher();            
        }

        object test1HandlerOneItemOp(object o)
        {
            _dispatcherOperation = DispatcherHelper.BeginInvokeWrapper(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(test1BGItemOp),
                null);

            exitOpAsync();

            _dispatcherOperation.Wait();
            
            return null;
        }

        object test1BGItemOp(object o)
        {
            CoreLogger.LogStatus("Run this before exiting");
            return null;
        }

        /******************************************************************************
        * Function:          Operation_CompleteTwoThreads
        ******************************************************************************/
        ///<summary>
        ///</summary>
        public void Operation_CompleteTimeout_SingleThread()
        {

            DispatcherHelper.EnqueueNormalCallback(
                Dispatcher.CurrentDispatcher,
                new DispatcherOperationCallback(test2HandlerOneItemOp), 
                null);

            CoreLogger.LogStatus("Enqueued 10 items at Normal Priority");

            for (int i=0; i<10;i++)                       
            {
                DispatcherHelper.EnqueueNormalCallback(
                    Dispatcher.CurrentDispatcher,
                    new DispatcherOperationCallback(operationCompleteDispatched),
                    null);
            }

            DispatcherHelper.RunDispatcher();            
        }

        object test2HandlerOneItemOp(object o)
        {

            CoreLogger.LogStatus("Enqueued 1 item at Background");
            _dispatcherOperation = DispatcherHelper.BeginInvokeWrapper(
                Dispatcher.CurrentDispatcher,
                DispatcherPriority.Background, 
                new DispatcherOperationCallback(exceptionBackgroundItemOp), 
                null);

            CoreLogger.LogStatus("Complete the Background item with timeout 1 sec");
            _dispatcherOperation.Wait(TimeSpan.FromSeconds(1));
            CoreLogger.LogStatus("Complete has timeout");
            
            if (!_dispatcherOperation.Abort())
            {
                throw new Microsoft.Test.TestValidationException("The operation wasn't aborted.");
            }

            if (_dispatcherOperation.Status == DispatcherOperationStatus.Completed)
            {
                throw new Microsoft.Test.TestValidationException("WasCompleted property return true.");
            }

            exitOpAsync();
            
            return null;
        }

        /******************************************************************************
        * Function:          Operation_CompleteTwoThreads
        ******************************************************************************/
        ///<summary>
        ///</summary>
        [Test(0, @"Threading\Operation", "Operation_CompleteAborted_SingleThread", Area="ElementServices")]
        public void Operation_CompleteAborted_SingleThread()
        {

            DispatcherHelper.EnqueueNormalCallback(
                Dispatcher.CurrentDispatcher,
                new DispatcherOperationCallback(test3HandlerOneItemOp), 
                null);

            CoreLogger.LogStatus("Enqueued 10 items at Normal Priority");

            for (int i=0; i<10;i++)                       
            {
                DispatcherHelper.EnqueueNormalCallback(
                    Dispatcher.CurrentDispatcher,
                    new DispatcherOperationCallback(operationCompleteDispatched),
                    null);

                if (i == 5)
                {
                    DispatcherHelper.EnqueueNormalCallback(
                        Dispatcher.CurrentDispatcher,
                        new DispatcherOperationCallback(abortingOperationItemOp),
                        null);
                }
            }

            DispatcherHelper.RunDispatcher();            
        }

        private object test3HandlerOneItemOp(object o)
        {
            CoreLogger.LogStatus("Enqueued 1 item at Background");
            _dispatcherOperation = DispatcherHelper.BeginInvokeWrapper(
                Dispatcher.CurrentDispatcher,
                DispatcherPriority.Background, 
                new DispatcherOperationCallback(operationCompleteDispatched), 
                null);

            CoreLogger.LogStatus("Complete the Background item no timeout");
            _dispatcherOperation.Wait();

            exitOpAsync();
            
            return null;
        }

        private void exitOpAsync()
        {
            DispatcherHelper.BeginInvokeWrapper(
                Dispatcher.CurrentDispatcher,
                DispatcherPriority.Background, 
                new DispatcherOperationCallback(exitOp), 
                null);
        }

        private object abortingOperationItemOp(object o)
        {
            _dispatcherOperation.Abort();
            return null;
        }

        private object operationCompleteDispatched(object o)
        {
            CoreLogger.LogStatus("Normal Priority Idle is been dispatched");
            _itemsCount ++;
            CoreLogger.LogStatus("Wait 250 Milliseconds");
            Thread.Sleep(200);
            
            return null;
        }

        private object exceptionBackgroundItemOp(object o)
        {
            throw new Microsoft.Test.TestValidationException("This item should not be called");
        }

        private object exitOp(object o)
        {
            CoreLogger.LogStatus("Shutdown the dispatcher");
            DispatcherHelper.ShutDown();
            return null;
        }
    }
    #endregion
}



