// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 1 $
 
*
******************************************************************************/
using System;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Threading
{
    
    /// <summary>
    /// TestCase for testing actions against DispatcheOperation in Avalon.
    /// </summary>    
    [TestDefaults]
    public class DispatcherOperationsOveralTesting : TestCaseBase
    {
        /// <summary>
        /// Testing doing a lot of actions on DispatcherOperation, single|multiple thread
        /// and a worker thread.
        /// </summary>
        //[TestCaseSeedBased(1,10,"0",@"Threading\Operation","-DThreads=1 -AmountOps=20 -AmountActions=20",TestCaseSecurityLevel.PartialTrust,".")]
        //[TestCaseSeedBased(10,11,"0",@"Threading\Operation","-DThreads=1 -AmountOps=100 -AmountActions=100",TestCaseSecurityLevel.PartialTrust,".")]
        //[TestCaseSeedBased(1,10,"0",@"Threading\Operation","-DThreads=3 -AmountOps=100 -AmountActions=100",TestCaseSecurityLevel.PartialTrust,".")]
        //[TestCaseSeedBased(100,200,"1",@"Threading\Operation","-DThreads=5 -AmountOps=200 -AmountActions=200",TestCaseSecurityLevel.PartialTrust,".")]
        //[TestCaseSeedBased(201,400,"2",@"Threading\Operation","-DThreads=7 -AmountOps=200 -AmountActions=200",TestCaseSecurityLevel.PartialTrust,".")]
        public void UsingSeedForCreateTestCase() 
        {
            //SeedBasedTestCaseInfo testcaseinfo = (SeedBasedTestCaseInfo)TestCaseInfo.GetCurrentInfo();            
            //IDictionary dictionary = TestCaseInfo.GetCurrentParams();
            
            // Setting up the variables for the test case
            int seed = Int32.Parse((string)DriverState.DriverParameters["Seed"]);
            int amountOps = Int32.Parse((string)DriverState.DriverParameters["amountops"]);
            int amountOfDispatcherThread = Int32.Parse((string)DriverState.DriverParameters["dthreads"]);
            int amountActions = Int32.Parse((string)DriverState.DriverParameters["amountactions"]);
            
            Random generalRandom = new Random(seed);
            List<DispatcherThreadCaseInfo> wrapperList = new List<DispatcherThreadCaseInfo>();

            s_ev = new  CoreAutoResetEvent(false, amountOfDispatcherThread);

            for (int i = 0 ; i < amountOfDispatcherThread; i++)
            {
                // By default we use Avalon Dispatcher.
                DispatcherType dType = DispatcherType.Avalon;
            
                // Choosing the dispatcherType
                if (generalRandom.Next(0,1000)%2 == 1)
                {
                    dType = DispatcherType.Win32;
                }
                
  
                DispatcherWrapper currentDispatcherW = new DispatcherWrapper(null,dType);

                //HACK! for 
                currentDispatcherW.ForceDispatcherShutdown = true;

                DispatcherPriority[] priorities = DispatcherPriorityHelper.GetValidDispatcherPriorities();
                Array callbackTypes = Enum.GetValues(typeof(InvokeCallbackTypes));

                // Creating total the OperationWrappers per thread.
                List<DispatcherOperationWrapper> opList = new List<DispatcherOperationWrapper>();
                for (int z = 0; z < amountOps; z++)
                {
                    // Choose a priority for the BeginInvokeCall
                    DispatcherPriority priority = priorities[generalRandom.Next(0,1000) % priorities.Length];

                    // Choose a callback type
                    InvokeCallbackTypes callbackType;

                    // We do this because this test cases are not prepare to handle Zero_Param validations.

                    do 
                    {
                        callbackType = (InvokeCallbackTypes)callbackTypes.GetValue(generalRandom.Next(0,1000) % callbackTypes.Length);
                    }
                    while(callbackType == InvokeCallbackTypes.Zero_Param_Generic);
                        
                    opList.Add(new DispatcherOperationWrapper(currentDispatcherW, priority,callbackType));
                }

                // Adding these ops to a dictionary so we can get the current ops
                // per dispatcher lookup.
                _opsPerDispatcherList.Add(currentDispatcherW,opList);


                // Now let's build the list of action that will be perform
                // over the random operation created before.
                List<OperationAction> actionList = new List<OperationAction>();                
                for (int j = 0; j < amountActions; j++)
                {

                    int notificationAction = generalRandom.Next(0,1000);

                    // Choose the thread to perform the action.
                    int threadIndex =0 ;

                    
                    OperationAction opAction = null;
                    if (notificationAction <= 50)
                    {
                        opAction = new OperationAction();
                        // Choose the thread to perform the action.
                        threadIndex = generalRandom.Next(0,1000) % _threadNames.Length;

                        opAction.Name = "DispatcherNotification";
                        opAction.DW = currentDispatcherW;

                        int addorremove = generalRandom.Next(0,1000) % 2;                        
                        int eventCount = generalRandom.Next(0,1000) % 5;

                        object[] oArray = {addorremove, eventCount};
                        opAction.Arg = oArray;


                        // Adding our created action to a list depending on which       
                        // thread the operation will be perform.                
                        AddAction(actionList, opAction, threadIndex);
                        
                    }

                    opAction = new OperationAction();

                    // Choose the thread to perform the action.
                    threadIndex = generalRandom.Next(0,1000) % _threadNames.Length;
               
                    // Choose an action.
                    int actionIndex = generalRandom.Next(0,1000) % _actions.Length;

                    // Choose an operation.
                    int opIndex = generalRandom.Next(0,1000) % opList.Count;


                    // Building the OperationAction, with the action to be performed and
                    // over which operation.

                    opAction.Operation = opList[opIndex];
                    opAction.Name = _actions[actionIndex];

                     
                    // PriorityChange requieres a new Priority, we choose a priority here.
                    if (opAction.Name == "PriorityChange")
                    {
                        opAction.Arg = priorities[generalRandom.Next(0,1000) % priorities.Length];            
                    }

                    // Adding our created action to a list depending on which       
                    // thread the operation will be perform.                
                    AddAction(actionList, opAction, threadIndex);

                }
                
                // Adding the actionList to our hashtable for future lookup
                // base on the dispatcherw.
                _dispatcherWList.Add(currentDispatcherW, actionList);

                
                // Add the current dispatcher to our special harness to start dispatcherWs on different.
                // threads.
                DispatcherThreadCaseInfo c = new DispatcherThreadCaseInfo(currentDispatcherW, this);
                c.DispatcherFirstIdleDispatched += new ThreadTestCaseCallback(DispatcherThreadHandler);
                wrapperList.Add(c);
                
            }

            // Setup the DispatcherThread to start running.
            MultipleDispatcherThreadTestCase testCase = new MultipleDispatcherThreadTestCase();
            testCase.InitializeDispatcherThreads(wrapperList.ToArray());

            
            s_evManual = new ManualResetEvent(false);

            // Setup the worker thread.
            ThreadCaseInfo threadInfo = new ThreadCaseInfo();
            threadInfo.ThreadStarted += delegate(ThreadCaseInfo info, EventArgs args)
                {
                    // Waiting for all the dispatcherThreads BeginInvoke all the operations.
                    s_ev.WaitOne();

                    // Signal the dispatchers threads to start running their actions.                   
                    s_evManual.Set();
                    
                    // We are going to BeginInvoke all the operations for this dispatcher.
                    for (int i = 0; i < _workerThreadList.Count; i++)
                    {                        
                        OperationAction opAction = _workerThreadList[i];
                        PerformOperationAction(opAction);

                    }
                };

            // Starting the Worker Thread.
            ThreadCaseInfo[] threadCaseInfo = {threadInfo};
            MultipleThreadTestCase workerThreads = new MultipleThreadTestCase();
            workerThreads.Initialize(threadCaseInfo);
            workerThreads.Start();

            // Make the dispatchers threads to run.
            testCase.Run();
            CoreLogger.LogStatus("The dispatcher threads exit.");            
            
            // Make sure that we wait for the worker thread to finish.
            // There could be a 
            CoreLogger.LogStatus("Waiting for the workerthread to finish.");            
            workerThreads.TestCompletedWaitHandle.WaitOne();


            CoreLogger.LogStatus("Validations");            
            if (workerThreads.ExceptionList.Length != 0)
            {
                CoreLogger.LogTestResult(false, "An exception was caught.");
            }


            if (testCase.ExceptionList.Length != 0)
            {
                CoreLogger.LogTestResult(false, "An exception was caught.");
            }            

        }


        void AddAction(List<OperationAction> actionList, OperationAction opAction, int threadIndex)
        {
            // Adding our created action to a list depending on which
            // thread the operation will be perform.
            if (threadIndex == 0)
            {
                actionList.Add(opAction);                
            }
            else
            {
                _workerThreadList.Add(opAction);
            }
        }

        static void PerformOperationAction(OperationAction opAction)
        {
            if (opAction.Name == "DispatcherNotification")
            {
                object[] oArray = (object[])opAction.Arg;                                

                bool  addorremove = ((int)oArray[0]) == 1;                        
                int eventCount = (int)oArray[1];

                opAction.DW.Hooks.AddRemoveEvents((DispatcherNotification)eventCount , addorremove);
                
            }
            else
            {
                opAction.Operation.DoAction(opAction);
            }
        }

        private static void DispatcherThreadHandler(ThreadCaseInfo info, EventArgs args)
        {
            DispatcherOperationsOveralTesting testCase = (DispatcherOperationsOveralTesting)info.Argument;
            DispatcherWrapper currentDW = DispatcherWrapper.FromCurrentDispatcher();
           
            List<DispatcherOperationWrapper> opList = testCase._opsPerDispatcherList[currentDW];

            // Saving the list of actions for this thread.
            t_threadActions = new Queue<OperationAction>(testCase._dispatcherWList[currentDW]);

            // We are going to BeginInvoke all the operations for this dispatcher.
            for (int i = 0; i < opList.Count; i++)
            {
                opList[i].BeginInvoke(
                    // Each beginInvoke will perform any action already determinated
                    (EventHandler) delegate
                    {
                        // Find the next action.
                        if (t_threadActions.Count > 0)
                        {
                            OperationAction opAction = t_threadActions.Dequeue();
                            PerformOperationAction(opAction);

                            

                        }

                        // Should Shutdown?
                        DispatcherWrapper currDW = DispatcherWrapper.FromCurrentDispatcher();
                        if ((currDW.DispatcherQueue.MaxPriority == DispatcherPriority.Inactive || currDW.DispatcherQueue.Count == 0) 
                            && !currDW.RealDispatcher.HasShutdownStarted)
                        {
                            currDW.ShutDown();
                            Console.WriteLine("Shutdown");

//Debugging to know the amount of values.
//                            foreach (KeyValuePair<DispatcherNotification, int> kvp in currDW.Hooks.Counters)
//                            {
//                                CoreLogger.LogStatus(kvp.Key.ToString() + "=" + kvp.Value.ToString());
//                            }
                        }
                    }
                );
            }

            // Signal the worker thread to start runnng.
            s_ev.Set();

            // Wait for all the thread to be ready.
            s_evManual.WaitOne();
        }

        [ThreadStatic]
        static Queue<OperationAction> t_threadActions = null;

        static CoreAutoResetEvent s_ev = null;
        static ManualResetEvent s_evManual = null;
        Dictionary<DispatcherWrapper, List<OperationAction>> _dispatcherWList = new Dictionary<DispatcherWrapper, List<OperationAction>>();
        Dictionary<DispatcherWrapper, List<DispatcherOperationWrapper>> _opsPerDispatcherList = new Dictionary<DispatcherWrapper, List<DispatcherOperationWrapper>>();
        List<OperationAction> _workerThreadList = new List<OperationAction>();        

        readonly string[] _actions = {"Wait", "Wait_Timeout", "PriorityChange", "Abort"};
        readonly string[] _threadNames = {"CurrentDispatcher", "Worker"};
        
    }
}



