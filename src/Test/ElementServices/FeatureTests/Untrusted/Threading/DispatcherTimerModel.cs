// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI;
using System.Windows.Threading;
using Microsoft.Test.Modeling;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using System.Diagnostics;
using Microsoft.Test.Threading;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Threading
{
    /// <summary>
    /// DispatcherTimerModel Model class
    /// F:\NT\testsrc\windowstestdata\REDIST\client\wcptests\Core\Base\Testcases\Threading\DispatcherTimeModel.mbt
    /// </summary>
    // Testing Posting Items from multiple threads    
    [Model(@"FeatureTests\ElementServices\DispatcherTimerTiming.xtc",1,@"Threading\Timing", "DispatcherTimerModel",
       Description = "Creating DispatcherTimers from Multiple Dispatcher Threads and From ThreadPool", ExpandModelCases=true, Area = "ElementServices")]    
    [Model(@"FeatureTests\ElementServices\DispatcherTimerTimingSingleDispatcher.xtc",1,@"Threading\Timing", "DispatcherTimerModel_SingleDispatcher",
      Description = "Creating DispatcherTimer on 1 Dispatcher from the same dispatcher or ThreadPool", ExpandModelCases = true, Area = "ElementServices")]   
    [Model(@"FeatureTests\ElementServices\DispatcherTimerTimingSingleDispatcher.xtc",1,3,0,@"Threading\Timing", "DispatcherTimerModel",
      Description = "Creating DispatcherTimer on 1 Dispatcher from the same dispatcher or ThreadPool", ExpandModelCases = true, Area = "ElementServices")]   
    public class DispatcherTimerModel : CoreModel 
    {
        /// <summary>
        /// Creates a DispatcherTimerModel Model instance
        /// </summary>
        public DispatcherTimerModel(): base()
        {
            Name = "DispatcherTimerModel";
            Description = "DispatcherTimerModel Model";
            ModelPath = "MODEL_PATH_TOKEN";
            
            //Add Action Handlers
            AddAction("SetupTestCase", new ActionHandler(SetupTestCase));
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Handler for SetupTestCase</remarks>
        private bool SetupTestCase(State endState, State inParams, State outParams)
        {
            DispatcherTimerModelTiming testcase = new DispatcherTimerModelTiming();
            testcase.SetupTestCase(inParams);
            return true;
        }

    }

    /// <summary>
    /// Automation test cases for DispatcherTimer
    /// </summary>   
    public class DispatcherTimerModelTiming : ModelAutomationBase
    {
        /// <summary>
        /// </summary>   
        public DispatcherTimerModelTiming() : base(null)
        {   
        }
        static new void Log(string s)
        {
            CoreLogger.LogStatus(s);
        }

        /// <summary>
        /// Only action in this model, includes execution and validation.
        /// </summary>   
        public void SetupTestCase(IDictionary dictionary)
        {
            // Number of dispatchers in test.
            int amountOfDispatchers = 1;            
            string argAmountOfDispatchers = (string)dictionary["AmountOfDispatchers"];
            if (String.Compare(argAmountOfDispatchers,"Single", true) != 0)
            {
                amountOfDispatchers = 3;
            }

            // Create dispatcher wrappers.
            int seedRandomDispatcherType = Int32.Parse((string)dictionary["SeedForDispatcherType"]);
            Random randomDispatcherType = new Random(seedRandomDispatcherType);
            Log("Creating " + amountOfDispatchers + " dispatchers.");
            for (int i = 0; i < amountOfDispatchers; i++)
            {
                // Dispatcher type - WPF Dispatcher or PInvoked Win32 generic message pump.
                DispatcherType dispatcherType = DispatcherType.Avalon;
                if (amountOfDispatchers != 1)
                {
                    int x = randomDispatcherType.Next(0,99);
                    if (x % 2 == 0)
                    {
                        Log(" #" + i + " Win32 dispatcher");
                        dispatcherType = DispatcherType.Win32;
                    }
                    else
                    {
                        Log(" #" + i + " WPF dispatcher");
                    }
                }                
                
                DispatcherWrapper dW = new DispatcherWrapper(null,dispatcherType);
                _dispatcherWrapperList.Add(dW);

                // Create timer list hash for each dispatcher.
                _threadActions.Add(dW, new List<DispatcherTimerWrapper>());
            }

            // Some timers may be started from ThreadPool.
            _threadActions.Add("ThreadPool", new List<DispatcherTimerWrapper>());
            
            // Number of timers in test.
            string argAmountOfTimers = (string)dictionary["AmountOfTimers"];
            int amountOfTimers = 1;
            if (String.Compare(argAmountOfTimers,"Single", true) != 0)
            {
                // NOTE: We used this also as seed for random number for choosing constructor that will be used
                amountOfTimers = Int32.Parse((string)dictionary["AmountMultipleTimers"]);
            }

            // Cache all possible priorities.
            DispatcherPriority[] priorities = DispatcherPriorityHelper.GetValidDispatcherPriorities();

            int seedRandomPriority = Int32.Parse((string)dictionary["SeedForPriority"]);
            int seedForStartDispatcherTimer = Int32.Parse((string)dictionary["SeedForStartDispatcherTimer"]);
            int seedForStopTimer = Int32.Parse((string)dictionary["SeedForStop"]);
            int seedForRepeatTime = Int32.Parse((string)dictionary["SeedForRepeatTime"]);
            int seedForDispatcher = Int32.Parse((string)dictionary["SeedForDispatcher"]);
            int seedForThreadCreateDispatcherTimer = Int32.Parse((string)dictionary["SeedForThreadCreateDispatcherTimer"]);
            int seedForTime = Int32.Parse((string)dictionary["SeedForTime"]);
            
            Random randomConstructor = new Random(amountOfTimers);
            Random randomPriority = new Random(seedRandomPriority);
            Random randomstartDispatcherTimer = new Random(seedForStartDispatcherTimer);    
            Random randomStop = new Random(seedForStopTimer);
            Random randomRepeatTime = new Random(seedForRepeatTime);
            Random randomDispatcher = new Random(seedForDispatcher);
            Random randomThreadCreateDispatcherTimer = new Random(seedForThreadCreateDispatcherTimer);
            Random randomTime = new Random(seedForTime);

            int cachedMaximumTime = 0;

            //
            // Construct all the DispatcherTimerWrappers needed for the test case.
            //
            Log("Creating dispatcher timer wrappers");
            for (int i = 0; i < amountOfTimers; i++)
            {
                DispatcherTimerWrapper timer = new DispatcherTimerWrapper();

                timer.Duration = _intervalTimes[randomTime.Next(0, 100) % _intervalTimes.Length];
                timer.RepeatCount = randomRepeatTime.Next(0, 100) % 4;

                if (cachedMaximumTime < timer.TotalDuration)
                {
                    cachedMaximumTime = timer.TotalDuration;
                }

                // Method for starting the timer.
                if (randomstartDispatcherTimer.Next(0, 100) % 2 == 0)
                {
                    timer.HowToStart = TimerControl.EnabledProperty;
                }
                else
                {
                    timer.HowToStart = TimerControl.StartStopMethod;
                }

                // Method for stopping the timer.
                if (randomStop.Next(0, 100) % 2 == 0)
                {
                    timer.HowToStop = TimerControl.EnabledProperty;
                }
                else
                {
                    timer.HowToStop = TimerControl.StartStopMethod;
                }


                //
                // Choose priority.
                //
                DispatcherPriority priority = priorities[randomPriority.Next(0, 1000) % priorities.Length];

                // Not inactive.
                if (priority == DispatcherPriority.Inactive)
                {
                    priority = DispatcherPriority.SystemIdle;
                }

                timer.Priority = priority;



                //
                // Choose constructor.
                //
                int timerDispatcherIndex = 0;   // Dispatcher timer will run on.
                int createTimerThreadIndex = 0; // Thread that will create dispatcher timer.

                DispatcherTimerConstructor timerCtor = (DispatcherTimerConstructor)(randomConstructor.Next(0, 100) % 3);
                if (timerCtor == DispatcherTimerConstructor.PriorityOnly)
                {
                    timer.Constructor = DispatcherTimerConstructor.PriorityOnly;

                    // Create and execute timer in same thread, same dispatcher.
                    createTimerThreadIndex = randomThreadCreateDispatcherTimer.Next(0, 100) % _dispatcherWrapperList.Count;
                    timerDispatcherIndex = createTimerThreadIndex;
                }
                else
                {
                    timer.Constructor = DispatcherTimerConstructor.PriorityAndDispatcher;

                    // Choose timer dispatcher
                    timerDispatcherIndex = randomDispatcher.Next(0, 100) % _dispatcherWrapperList.Count;

                    // Create possiblity of running timer on extra threadpool thread.
                    createTimerThreadIndex = randomThreadCreateDispatcherTimer.Next(0, 100) % (_dispatcherWrapperList.Count + 1); 
                }

                timer.TimerDispatcher = _dispatcherWrapperList[timerDispatcherIndex];

                if (timerCtor == DispatcherTimerConstructor.Default)
                {
                    // If default constructor, make priority background.
                    timer.Priority = DispatcherPriority.Background;
                }

                //
                // Select the thread where the action will be performed.
                // This could be a DispatcherWrapper thread or ThreadPool thread.
                //
                List<DispatcherTimerWrapper> dispatcherTimerList = null;

                if (createTimerThreadIndex < _dispatcherWrapperList.Count)
                {
                    DispatcherWrapper chosenDispatcherW = _dispatcherWrapperList[createTimerThreadIndex];
                    dispatcherTimerList = (List<DispatcherTimerWrapper>)_threadActions[chosenDispatcherW];
                }
                else
                {
                    dispatcherTimerList = (List<DispatcherTimerWrapper>)_threadActions["ThreadPool"];
                }

                dispatcherTimerList.Add(timer);
            }               

            //
            // Run each dispatcher thread.
            //
            _multipleDispatcherThreadCase = new MultipleThreadTestCase(ThreadCaseSynchronization.None);
            CoreAutoResetEvent ev = new CoreAutoResetEvent(false, _dispatcherWrapperList.Count);
            List<ThreadCaseInfo> threadInfos = new List<ThreadCaseInfo>();

            for (int i=0;i<_dispatcherWrapperList.Count;i++)
            {
                DispatcherWrapper currentDispatcherW = _dispatcherWrapperList[i];
                List<DispatcherTimerWrapper> timerList = (List<DispatcherTimerWrapper>)_threadActions[currentDispatcherW];
                object[] arguments = {ev, currentDispatcherW,timerList};
                ThreadCaseInfo threadCaseInfo = new ThreadCaseInfo(arguments);
                threadCaseInfo.ThreadApartmentState = ApartmentState.STA;
                threadCaseInfo.ThreadStarted += new ThreadTestCaseCallback(DispatcherThreadHandler);
                threadInfos.Add(threadCaseInfo);
            }            

            _multipleDispatcherThreadCase.Initialize(threadInfos.ToArray());
            _multipleDispatcherThreadCase.Start();

            // Getting actions that are going to be perform by the ThreadPool
            List<DispatcherTimerWrapper> timerListForThreadPool = (List<DispatcherTimerWrapper>)_threadActions["ThreadPool"];            

            // Waiting for all dispatcher to be up and runnning            
            ev.WaitOne();

            // Setting the threadPool to do some work
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolHandler), timerListForThreadPool);
            
            
            if (System.Diagnostics.Debugger.IsAttached)
            {
                _timeout = TimeSpan.FromMilliseconds(cachedMaximumTime + 200000);
            }
            else
            {
                _timeout = TimeSpan.FromMilliseconds(cachedMaximumTime + 10000);
            }

            // Now that we have all the actions parsed and ready
            // we start the execution.
            _closingTimer = new System.Timers.Timer();
            _closingTimer.Interval = _timeout.TotalMilliseconds;
            _closingTimer.Elapsed += new System.Timers.ElapsedEventHandler(ClosingTest);
            _closingTimer.Start();

            // Now we sit and wait for all the timer  and dispatcher :)

            _multipleDispatcherThreadCase.TestCompletedWaitHandle.WaitOne();

            // The Dispatchers suppose to be done by now.
            // It is time to validate all the individuals timers

            ValidateAllTimers();
        }

        /// <summary>
        /// A CLR timer will stop all the Dispatchers.
        /// </summary>
        private void ClosingTest(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer closingTimer = (System.Timers.Timer)sender;
            
            closingTimer.Stop();
            
            ShutdownAllDispatcherW();   
        }

        /// <summary>
        /// Shut Down all dispatcher
        /// </summary>
        void ShutdownAllDispatcherW()
        {
            for (int i=0;i < _dispatcherWrapperList.Count; i++)
            {
                _dispatcherWrapperList[i].ShutDown();
            }   
        }

        /// <summary>
        /// Validate all the Timers that we built.  Also we validate that all the dispatcher didn't
        /// throw any exception.
        /// </summary>
        void ValidateAllTimers()
        {
            //
            // Verify there are no exceptions on any of the threads.
            //

            if (_multipleDispatcherThreadCase.ExceptionList.Length > 0)
            {
                CoreLogger.LogTestResult(false,"Exception on one of the Dispatcher threads:");

                for (int i = 0 ; i <  _multipleDispatcherThreadCase.ExceptionList.Length; i++)
                {
                    CoreLogger.LogStatus(_multipleDispatcherThreadCase.ExceptionList[i].Message);
                    CoreLogger.LogStatus(_multipleDispatcherThreadCase.ExceptionList[i].StackTrace.ToString());
                }
            }

            //
            // Verify that all the timers are done.
            //                                          
            foreach (DictionaryEntry entry in _threadActions)
            {
                List<DispatcherTimerWrapper> timerList = (List<DispatcherTimerWrapper>)entry.Value;

                for(int i = 0; i < timerList.Count; i++)
                {
                    if (timerList[i].ValidateTimeFire() == false)
                    {
                        CoreLogger.LogTestResult(false, "DispatcherTimer did not execute correctly.");
                        return;
                    }
                }
            }            
        }


        /// <summary>
        /// </summary>   
        public static void DispatcherThreadHandler(ThreadCaseInfo info, EventArgs args)
        {
            object[] arguments = (object[])info.Argument;

            CoreAutoResetEvent ev = (CoreAutoResetEvent)arguments[0];
            DispatcherWrapper dispatcherW = (DispatcherWrapper)arguments[1];
            List<DispatcherTimerWrapper> timerList = (List<DispatcherTimerWrapper>)arguments[2];
                
            dispatcherW.SetDispatcher(Dispatcher.CurrentDispatcher);

            DispatcherHelper.EnqueueBackgroundCallback(Dispatcher.CurrentDispatcher,
                (DispatcherOperationCallback) delegate(object notUsed)
                {
                    // Signal to start creating DispatcherTimer
                    ev.Set();
                    return null;
                },
                null);

            for (int i=0; i<timerList.Count;i++)
            {
                DispatcherHelper.EnqueueBackgroundCallback(Dispatcher.CurrentDispatcher,
                    (DispatcherOperationCallback) delegate(object dispatcherTimerW)
                    {
                        // Creating dispatchertimers async                        
                        DispatcherTimerWrapper dw = (DispatcherTimerWrapper)dispatcherTimerW;
                        dw.CreateDispatcherTimer();
                        return null;
                    },
                    timerList[i]);
            }

            dispatcherW.Run();
            Log(Thread.CurrentThread.GetHashCode().ToString() + " is exiting.");
        }


        /// <summary>
        /// </summary>   
        public static void ThreadPoolHandler(object state)
        {
            if (state != null)
            {
                List<DispatcherTimerWrapper> timerList = (List<DispatcherTimerWrapper>)state;            
                for (int i=0; i<timerList.Count;i++)
                {
                    timerList[i].CreateDispatcherTimer();
                }
            }
        }


        #region Private data

        private TimeSpan _timeout ;
        private System.Timers.Timer _closingTimer = null; 
        private Hashtable _threadActions = new Hashtable();
        private AutoResetEvent _ev = new AutoResetEvent(false);
        private MultipleThreadTestCase _multipleDispatcherThreadCase = null;
        private List<DispatcherWrapper> _dispatcherWrapperList = new List<DispatcherWrapper>();

        private readonly int[] _intervalTimes = { 0, 500, 1000, 1500, 2000, 2500 }; // In milliseconds

        #endregion
    }
}


