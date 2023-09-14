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
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Modeling;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Win32;
using System.Threading;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Threading
{
    /// <summary>
    /// DispatcherOperationPost Model class
    ///
    /// Model Name === Xtc File
    /// client\wcptests\Core\Base\Testcases\Threading\DispatcherOperationPost2.mbt === DispatcherOperationPostTestOrdering.xtc
    /// </summary>
    [Model(@"FeatureTests\ElementServices\DispatcherOperationPostTestOrdering.xtc", 1, @"Threading\Operation", "DispatcherOperationPostOrderModel", Description="Model for BeginInvoke testing Ordering", ExpandModelCases=true, Area = "ElementServices")]    // Testing Posting Items from multiple threads
    public class DispatcherOperationPost : CoreModel 
    {
        /// <summary>
        /// Creates a DispatcherOperationPost Model instance
        /// </summary>
        public DispatcherOperationPost(): base()
        {
            Name = "DispatcherOperationPost";
            Description = "DispatcherOperationPost Model";
            ModelPath = "MODEL_PATH_TOKEN";            

            //Attach Event Handlers
            //OnInitialize += new EventHandler(OnInitialize_Handler);
            //OnCleanUp += new EventHandler(OnCleanUp_Handler);
            OnGetCurrentState += new StateEventHandler(OnGetCurrentState_Handler);
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            base.OnEndCaseOnNestedPump += new EventHandler(OnEndCaseOnNestedPump_Handler);

            //Add StateVariables
            AddStateVariable("AmountPostedItems");
            AddStateVariable("DispatcherState");

            
            // Use on the DispatcherOperationPostTestOrdering.xtc            
            AddAction("StartDispatcher", new ActionHandler(StartDispatcher));


            
            AddAction("Posting", new ActionHandler(Posting));


            // Use on the DispatcherOperationPostTestOrdering.xtc
            AddAction("PostingItems", new ActionHandler(PostingItems));          

            //

        }


        /// <summary>
        /// Gets the current State of the Model
        /// </summary>
        /// <remarks>
        /// Attached to OnGetCurrentState event which is fired after
        /// each action to validate
        /// </remarks>
        /// <param name="sender">Model that fired the event</param>
        /// <param name="e">The current State in a StateEventArgs</param>
        private void OnGetCurrentState_Handler(object sender, StateEventArgs e)
        {
            e.State["AmountPostedItems"] = null; //
            e.State["DispatcherState"] = null; //
        }


        /// <summary>
        /// Sets the Model as necessary to begin the case with the given State
        /// </summary>
        private void OnBeginCase_Handler(object sender, StateEventArgs e)
        {
        }


        /// <summary>
        /// Sets the Model as necessary when a case ends with the given State
        /// </summary>
        private void OnEndCase_Handler(object sender, StateEventArgs e)
        {
        }

        /// <summary>
        /// Callback that is called when alls transitions for the current test case on the 
        /// XTC are completed. For example you may want to exited the Dispatcher or
        /// or close the nested pump.
        /// </summary>
        private void OnEndCaseOnNestedPump_Handler(object o,EventArgs args)
        {

        }        

        /// <summary>
        /// 
        /// </summary>
        private bool StartDispatcher(State endState, State inParams, State outParams)
        {
            //inParams["WhenDispatcherStarted"] - 
            //inParams["DispatcherType"] - 
            _automation = new DispatcherOperationPostAutomation(null);
            _automation.StartDispatcher(inParams["SeedForDispatcherType"], inParams["WhenToStart"]);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        private bool Posting(State endState, State inParams, State outParams)
        {
            //inParams["Priority"] - 
            //inParams["Arguments"] - 
            //inParams["FromThread"] - 

            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        private bool PostingItems(State endState, State inParams, State outParams)
        {
            //inParams["Priority"] - 
            //inParams["Arguments"] - 
            //inParams["FromThread"] - 
            _automation.PostItems(inParams["NumberOfPostItems"], inParams["SeedforPriority"], inParams["SeedforThread"],
            inParams["AmountForRecursiveCalls"],inParams["SeedForDelegateToUse"]);
            _automation.Dispose();

            return true;
        }
        DispatcherOperationPostAutomation _automation;
        
    }

    /// <summary>
    /// </summary>
    public enum WhenStartDispatcher
    {
        /// <summary>
        /// Start running the dispatcher before doing anything.
        /// </summary>
        Initial,

        /// <summary>
        /// After the first round of post items, we run the dispatcher
        /// </summary>
        AfterAllItemsPosted

    }

    /// <summary>
    /// </summary>
    public class DispatcherOperationPostAutomation : ModelAutomationBase
    {
        /// <summary>
        /// </summary>
        public DispatcherOperationPostAutomation(AsyncActionsManager asyncManager) : base(asyncManager)
        {
            for (int i = 0; i < _threadNames.Length;i ++)
            {
                List<ThreadDispatcherAction> list = new List<ThreadDispatcherAction>();
                _threadDispatcherActionHash.Add(_threadNames[i], list);
            }
        }

        /// <summary>
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// </summary>
        public void PostItem(string priority, string arguments, string fromThread)
        {
            
        }

        /// <summary>
        /// </summary>
        public void StartDispatcher(string seedForDispatcherType, string whenStartDispatcher)
        {
             _seedForDispatcherType = Int32.Parse(seedForDispatcherType);
            _whenStartDispatcher = (WhenStartDispatcher)Enum.Parse(typeof(WhenStartDispatcher), whenStartDispatcher);


        }

        /// <summary>
        /// </summary>
        public void PostItems(string numberOfPostItems, string seedForPriority, string seedForThread,
            string amountForRecursiveCalls, string seedForDelegateToUse )
        {

            DispatcherPriority[] priorityList = DispatcherPriorityHelper.GetValidDispatcherPriorities();


            // Parsing all data for the test case

            _numberOfPostItems = Int32.Parse(numberOfPostItems);
            _seedForPriority = Int32.Parse(seedForPriority);
            _seedForThread = Int32.Parse(seedForThread);
            _amountForRecursiveCalls = Int32.Parse(amountForRecursiveCalls);
            _seedForDelegateToUse = Int32.Parse(seedForDelegateToUse);
   

            Random randomCallback = new Random(_seedForDelegateToUse);
            Random randomPriority = new Random(_seedForPriority);
            Random randomThread = new Random(_seedForThread);
            Random randomDispatcherW = new Random();
            Random randomDispatcherType = new Random(_seedForDispatcherType);

            for (int i = 0; i < _dispatcherAmount; i++)
            {
                int indexDispatcherType = randomDispatcherType.Next(0,100) % 2;
                DispatcherType dType = DispatcherType.Avalon;

                if (indexDispatcherType == 1)
                {
                    dType = DispatcherType.Win32;
                }
                
                DispatcherWrapper dispatcherWrapper = new DispatcherWrapper(null,dType);
                _dispatcherWrapperList.Add(dispatcherWrapper);
                List<ThreadDispatcherAction> list = new List<ThreadDispatcherAction>();
                _threadDispatcherActionHash.Add(dispatcherWrapper, list);

                if (_whenStartDispatcher == WhenStartDispatcher.AfterAllItemsPosted)
                {
                    dispatcherWrapper.ValidateOrdering = true;
                }

            }

            // On this loop will are going to construct all the actions that are going
            // to be perform later. The reason of doing this it is how avoid parsing and
            // conversation during the execution time.

            for (int i = 0; i < _numberOfPostItems; i++)
            {
                // Building the Callback
                ThreadDispatcherAction action = null;
                
                int callbackInt = randomPriority.Next(0,100);


                string requestedCallback = "1_docallback";

                if (callbackInt % 2 == 0)
                {
                    requestedCallback = "1_obj";
                }
                               
                DispatcherWrapper.GetCallback(requestedCallback, ref action);
                // End building the Callback

                // Selecting the Priority
                
                int nextPriorityIndex  = randomPriority.Next(0,priorityList.Length); 
                DispatcherPriority priority = priorityList[nextPriorityIndex];
                action.Priority = priority;
                
                // End Selecting the priority


                // Choosing a dispatcher

                int nextDispatcherWrapper = randomDispatcherW.Next(0,_dispatcherWrapperList.Count-1);

                DispatcherWrapper dWrapper = _dispatcherWrapperList[nextDispatcherWrapper];
                action.SetDispatcherW(dWrapper); // The priority need to be setted before.

                // End Choosing a dispatcher


                // Selecting the Thread that will do the operation
                
                int nextThreadToUse = randomThread.Next(0,100) % 3; 

                // Adding the Action to the correct thread-actions list                
                AddThreadDispatcherAction(nextThreadToUse,action);
                
                // End Selecting the Thread that will do the operation
                
            }


            Log("Number of Items on the Expected on Dispatchers");

            ActionOverDispatchers(DispatchersAction.Print);
            

            if (System.Diagnostics.Debugger.IsAttached)
            {
                _timeout = TimeSpan.FromSeconds(1000000);
                s_isDebuggerAttached = true;
            }
            else
            {
                int ratio = _numberOfPostItems / 1000;  // 3 seconds for each 100 items
                if (ratio == 0)
                {
                    ratio = 1;
                }
                _timeout = TimeSpan.FromSeconds(1 * ratio);
            }

            // Now that we have all the actions parsed and ready
            // we start the execution.
            _closingTimer = new System.Timers.Timer();
            _closingTimer.Interval = _timeout.TotalMilliseconds;
            _closingTimer.Elapsed += new System.Timers.ElapsedEventHandler(ClosingTest);


            StartTestExecution();

            Log("Number of Items left on the Dispatcher");
            ActionOverDispatchers(DispatchersAction.Validate | DispatchersAction.Print);
            VerifyMultipleThreadTestCase(_multipleDispatcherThread);
            VerifyMultipleThreadTestCase(_multipleWorkerThreads);  
        }

        System.Timers.Timer _closingTimer = null;
                
        private void ClosingTest(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer closingTimer = (System.Timers.Timer)sender;
            
            closingTimer.Stop();
            
            ActionOverDispatchers(DispatchersAction.Shutdown);

        }

        void VerifyMultipleThreadTestCase(MultipleThreadTestCase testCase)
        {
            if (testCase != null)
            {
                if (testCase.ExceptionList.Length > 0)
                {

                    for (int i = 0; i < testCase.ExceptionList.Length; i++)
                    {
                        Log(testCase.ExceptionList[i].Message);
                    }

                    CoreLogger.LogTestResult(false,"");
                }
            }
            else
            {
                CoreLogger.LogTestResult(false,"The MultipleThreadTestCase was null");
            }
        }

        [FlagsAttribute]
        enum DispatchersAction : short
        {
            Shutdown = 4,
            Validate = 1,
            Print = 2            
        };
        
        void ActionOverDispatchers(DispatchersAction action)
        {
            for (int i =0;i < _dispatcherWrapperList.Count; i++)
            {
                if ( (action & DispatchersAction.Validate) == DispatchersAction.Validate)
                {
                    _dispatcherWrapperList[i].ValidateAllItemsBeenDispatched();
                }

                if  ( (action & DispatchersAction.Print) == DispatchersAction.Print)
                {
                    Log("* Items on Dispatcher: " + i.ToString());
                    _dispatcherWrapperList[i].PrintCount();
                }
                
                if  ( (action & DispatchersAction.Shutdown) == DispatchersAction.Shutdown)
                {
                    _dispatcherWrapperList[i].ShutDown();
                }
            }     
        }

        
    
        private static void StartDispatcherExe(ThreadCaseInfo threadCaseInfo, EventArgs args)
        {       
            object[] objectArray = (object[])threadCaseInfo.Argument;

            DispatcherWrapper dispatcherWrapper = (DispatcherWrapper)objectArray[0];
            CoreAutoResetEvent ev = (CoreAutoResetEvent)objectArray[1];
            List<ThreadDispatcherAction> threadDispatcherActionList = (List<ThreadDispatcherAction>)objectArray[2];
            WhenStartDispatcher wsd = (WhenStartDispatcher)objectArray[3];
            ManualResetEvent coordinationEventStartPostingEvent = (ManualResetEvent)objectArray[4];
            ManualResetEvent coordinationEventAllItemsPosted = (ManualResetEvent)objectArray[5];
            
            dispatcherWrapper.SetDispatcher(Dispatcher.CurrentDispatcher);
            ev.Set();

            if (wsd == WhenStartDispatcher.AfterAllItemsPosted)
            {               
                // Wait to start posting items
                coordinationEventStartPostingEvent.WaitOne();
                StartPostingItemDispatcherThread(threadDispatcherActionList);

                // Waiting for all the worker threads to finish posting items
                coordinationEventAllItemsPosted.WaitOne();
            }
            else
            {
                dispatcherWrapper.RealDispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object notUsed)
                    {
                        StartPostingItemDispatcherThread(threadDispatcherActionList);                        
                        return null;
                    }, null);                
            }


            dispatcherWrapper.Run();            
        }


        static void  StartPostingItemDispatcherThread(List<ThreadDispatcherAction> o)
        {
            List<ThreadDispatcherAction> threadDispatcherActionList = (List<ThreadDispatcherAction>)o;

            for(int i = 0; i < threadDispatcherActionList.Count; i++)
            {
                LogDebuggerAttach("Posting to this Priority: " + threadDispatcherActionList[i].Priority);
                threadDispatcherActionList[i].DispatcherW.BeginInvoke(threadDispatcherActionList[i]);
            }
            
            return;
        }
        
        void StartTestExecution()
        {

            List<ThreadCaseInfo> threadCaseInfoList = new List<ThreadCaseInfo>();
            CoreAutoResetEvent ev = new CoreAutoResetEvent(false,_dispatcherWrapperList.Count);

            for(int i=0; i < _dispatcherWrapperList.Count; i++)
            {
                object[] objectArray = {_dispatcherWrapperList[i], ev,
                    _threadDispatcherActionHash[_dispatcherWrapperList[i]], _whenStartDispatcher,
                    _coordinationEventStartPostingEvent,_coordinationEventAllItemsPosted};
                ThreadCaseInfo threadCaseInfo = new ThreadCaseInfo(objectArray);
                threadCaseInfo.ThreadStarted += new ThreadTestCaseCallback(StartDispatcherExe);
                threadCaseInfo.ThreadApartmentState = ApartmentState.STA;
                threadCaseInfoList.Add(threadCaseInfo);
            }          
            
            _multipleDispatcherThread = new MultipleThreadTestCase(ThreadCaseSynchronization.None);
            _multipleDispatcherThread.Initialize(threadCaseInfoList.ToArray());
            _multipleDispatcherThread.Start();

            ev.WaitOne();

            StartPosting();
            _closingTimer.Start();
            _multipleDispatcherThread.TestCompletedWaitHandle.WaitOne();
        }

        void StartPosting()
        {

            List<ThreadCaseInfo> threadCaseInfos = new List<ThreadCaseInfo>();
            CoreAutoResetEvent ev = new CoreAutoResetEvent(false, 2);
            
            for (int i = 0;  i < _threadNames.Length; i++)
            {
                List<ThreadDispatcherAction> actions = (List<ThreadDispatcherAction>)_threadDispatcherActionHash[_threadNames[i]];
                object[] objects = {actions, ev};

                ThreadCaseInfo info = new ThreadCaseInfo(objects);

                if (_threadNames[i].IndexOf("STA") != -1)
                {
                    info.ThreadApartmentState = ApartmentState.STA;
                }

                info.ThreadStarted += new ThreadTestCaseCallback(StartPostingThreadTestCaseCallback);
                threadCaseInfos.Add(info);               
            }
                       
            _multipleWorkerThreads = new MultipleThreadTestCase(ThreadCaseSynchronization.None);
            _multipleWorkerThreads.Initialize(threadCaseInfos.ToArray());

            // Signal Dispatcher Threads to start posting
            _coordinationEventStartPostingEvent.Set();
          
            if (_whenStartDispatcher == WhenStartDispatcher.AfterAllItemsPosted)
            {
                _multipleWorkerThreads.Run();

                // All Items from the Worker Threads are posted, we signal the dispatchers
                // threads
                _coordinationEventAllItemsPosted.Set();
            }   
            else
            {
                _multipleWorkerThreads.Start();
            }
        }

        /// <summary>
        /// The same callback it is for the STA thread and the MTA Thread
        /// </summary>        
        void StartPostingThreadTestCaseCallback(ThreadCaseInfo info, EventArgs args)
        {
            object[] objects = (object[])info.Argument;
            
            List<ThreadDispatcherAction> threadDispatcherActions = (List<ThreadDispatcherAction>)objects[0];            

            StartPostingItemDispatcherThread(threadDispatcherActions);

            if (objects[1] != null)
            {
                CoreAutoResetEvent ev = (CoreAutoResetEvent)objects[1];
                ev.Set();
            }            
        }
        
        // Add a ThreadDispatcherAction to the Bucket with the same thread name
        void AddThreadDispatcherAction(int index, ThreadDispatcherAction action)
        {
            List<ThreadDispatcherAction> list = null;

            if (index == 0)
            {
                list = (List<ThreadDispatcherAction>)_threadDispatcherActionHash[_threadNames[0]];
            }
            else if (index == 1)
            {
                list = (List<ThreadDispatcherAction>)_threadDispatcherActionHash[_threadNames[1]];
            }
            else if (index == 2)
            {
                list = (List<ThreadDispatcherAction>)_threadDispatcherActionHash[action.DispatcherW];                
            }
            else
            {
                throw new ArgumentException("The index was out of range.","index");
            }
            
            list.Add(action);
        }



        static void LogDebuggerAttach(string s)
        {
            if (s_isDebuggerAttached)
                CoreLogger.LogStatus(s);
        }

        static bool s_isDebuggerAttached = false;
        int _dispatcherAmount = 1;
        int _numberOfPostItems = 0;
        int _seedForPriority = 0;
        int _seedForThread = 0;
        int _seedForDispatcherType = 0;        
        int _amountForRecursiveCalls = 0;
        int _seedForDelegateToUse = 0;
        TimeSpan _timeout;
        Hashtable _threadDispatcherActionHash = new Hashtable(3);
        WhenStartDispatcher _whenStartDispatcher;
        string[] _threadNames = {"MTAThread","STAThread"};
        ManualResetEvent _coordinationEventStartPostingEvent = new ManualResetEvent(false);
        ManualResetEvent _coordinationEventAllItemsPosted = new ManualResetEvent(false);        
        List<DispatcherWrapper> _dispatcherWrapperList = new List<DispatcherWrapper>();
        MultipleThreadTestCase _multipleWorkerThreads = null;
        MultipleThreadTestCase _multipleDispatcherThread = null;
    }
}

//This file was generated using MDE on: Friday, February 04, 2005 4:32:54 PM

