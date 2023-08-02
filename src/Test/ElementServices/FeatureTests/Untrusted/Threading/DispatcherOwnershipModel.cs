// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public enum ThreadState
    {
        /// <summary>
        /// 
        /// </summary>
        Null,
        
        /// <summary>
        /// 
        /// </summary>
        NoStarted,
        
        /// <summary>
        /// 
        /// </summary>
        Running,
        
        /// <summary>
        /// 
        /// </summary>
        GarbagedCollected,
        
        /// <summary>
        /// 
        /// </summary>
        Exited
    }


    /****************  DispatcherOperationCompleteModel Model *****************
     *  Description: 
     *  Area: 
      *  Dependencies: ClientTestLibrary.dll
     *  Revision History:
     **********************************************************/

    /// <summary>
    /// Stateless Model About the a Single Dispatcher.Invoke behavior
    /// The model can be found at     
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\DispatcherOwnershipModel.mbt
    /// This model only test a single dispatcher
    /// </summary>
    [Model(@"FeatureTests\ElementServices\DispatcherOwnershipModel.xtc", 1, @"Threading\DispatcherOwnershipModel", TestCaseSecurityLevel.FullTrust, "DispatcherOwnershipModel",
       Description = "Stateless Model to test Dispatcher.FromThread and Dispatcher.Thread using a single dispatcher.", ExpandModelCases = true, Area = "ElementServices")]    
    public class DispatcherOwnershipModel : CoreModel 
    {
        /// <summary>
        /// Creates a SingleInvokeModel Model instance
        /// </summary>
        public DispatcherOwnershipModel(): base()
        {
            Name = "DispatcherOwnershipModel";
            Description = "Model DispatcherOwnershipModel";
            ModelPath = "MODEL_PATH_TOKEN";

            //Attach Event Handlers
            OnInitialize += new EventHandler(OnInitialize_Handler);
            OnCleanUp += new EventHandler(OnCleanUp_Handler);
            
            //Add Action Handlers
            AddAction("FromThread", new ActionHandler(FromThread));


        }

        /// <summary>
        /// Initializes the Model
        /// </summary>
        private void OnInitialize_Handler(object sender, EventArgs e){}

        /// <summary>
        /// Cleans up the Model
        /// </summary>
        private void OnCleanUp_Handler(object sender, EventArgs e) {}

        /// <summary>
        /// Handler for Setup
        /// </summary>
        private bool FromThread( State endState, State inParameters, State outParameters )
        {
            LogComment( "Action FromThread" );

            LogComment( inParameters.ToString() );

            bool succeed = true;

            _automation = new DispatcherOwnershipModelAutomation(AsyncActions);
            _automation.FromThread(inParameters);

            return succeed;
        }

        DispatcherOwnershipModelAutomation _automation;
        
    }

    ///<summary>
    /// This class implements the automation requeries for the model 
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\DispatcherOwnershipModel.mbt
    /// This model only test a single dispatcher.
    ///</summary>     
    internal class DispatcherOwnershipModelAutomation : ModelAutomationBase
    {
        ///<summary>
        ///</summary>     
        public DispatcherOwnershipModelAutomation(AsyncActionsManager asyncManager):base(asyncManager){}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        public void FromThread(IDictionary dictionary)
        {
            /*
               dictionary["ThreadState"] - // 
               dictionary["DispatcherState"] - // 
               dictionary["AmountOfThreadsCalling"] - // 
               dictionary["TargetThread"] - // 
               dictionary["AmountOfTimesCalling"] - // 
            */
            // Parsing Section.
            
            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;

            _ds = ParsingDispatcherState((string)dictionary["DispatcherState"]);
            _ts = ParsingThreadState((string)dictionary["ThreadState"]);
            _targetCurrentThread = ParsingCurrentTargetThread((string)dictionary["DispatcherState"]);
            _multipleCalls = ParsingCallMultipleTimes((string)dictionary["AmountOfTimesCalling"]);
            
            Dispatcher dispatcher = null;

            if (_ts == ThreadState.Null)
            {
                dispatcher = AvalonFromThread(null);
                if (dispatcher != null)
                {
                    LogFailed("Passing Null should return null.");
                    return;
                }
            }

            _thread = new Thread(new ParameterizedThreadStart(TestWorkerThread));
            _thread.SetApartmentState(ApartmentState.STA);

            if (_ts == ThreadState.NoStarted)
            {
                dispatcher = AvalonFromThread(_thread);

                if (dispatcher != null)
                {
                    LogFailed("Passing No Started Thread should return null.");
                    return;
                }
            }

            _threadWeak = new WeakReference(_thread);
            _thread.Start(dictionary);

            _thread.Join();
            

            if (_ts == ThreadState.GarbagedCollected)
            {
                
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration);
                GC.WaitForPendingFinalizers();                    

                if (s_dispatcher.Thread != _thread)
                {
                    LogFailed("Thread property should return the corresponding thread.");
                }

                if (currentDispatcher != Dispatcher.FromThread(Thread.CurrentThread))
                {
                    LogFailed("Wrong dispatcher");
                }
            }      

            if (s_exceptionList.Count > 0)
            {
                LogFailed("An exception was caught.");
            }
        }

        void CallFromThread()
        {
            int count;

            if (_multipleCalls)
            {
                count = 10;
            }
            else
                count = 1;
            
            for (int i =0; i<1;i++)
            {
                Dispatcher d = AvalonFromThread(_thread);


                if (d != null && d.Thread != _thread)
                {
                    LogFailed("The dispatcher thread property doesn't match." + count);
                }

                if (_ds == DispatcherState.NoCreated && d != null)
                {
                    LogFailed("The dispatcher was not created.");
                }
                else
                {
                    if (d != s_dispatcher)
                    {
                        LogFailed("The dispatcher doesn't match.");
                    }
                }            
            }
        }


        void MultipleThreadsAction()
        {
            Thread t1 = new Thread( (ThreadStart) delegate
            {
                try
                {
                    Thread.Sleep(1);
                    CallFromThread();
                    UnblockThread();
                }
                catch(Exception e)
                {
                    lock(s_syncRoot)
                    {
                        s_exceptionList.Add(e);
                    }
                    CoreLogger.LogStatus(e.Message);
                    CoreLogger.LogStatus(e.StackTrace.ToString());                    
                }
            });
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();

            for (int i = 0; i <= 10; i++)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                        Thread.Sleep(1);
                        CallFromThread();
                        UnblockThread();                        
                    }
                    catch(Exception e)
                    {
                        lock(s_syncRoot)
                        {
                            s_exceptionList.Add(e);
                        }
                        CoreLogger.LogStatus(e.Message);
                        CoreLogger.LogStatus(e.StackTrace.ToString());                                            
                    }                    
                });
            }
            s_ev.WaitOne();
        }

        /// <summary>
        /// This method is called for multiple threads.
        /// When it reaches 11, signals the thread.
        /// </summary>
        void UnblockThread()
        {
            int i = Interlocked.Increment(ref s_count);
            if (i >= 11)
            {
                s_ev.Set();
            }
        }

        /// <summary>
        /// The Test thread that it will create the Dispatcher.
        /// </summary>
        /// <param name="o"></param>
        private void TestWorkerThread(object o)
        {        
            IDictionary dictionary = (IDictionary)o;
            DispatcherState ds = ParsingDispatcherState((string)dictionary["DispatcherState"]);
            
            if (DispatcherState.NoCreated != ds)
            {
                s_dispatcher = Dispatcher.CurrentDispatcher;

                s_dispatcher.ShutdownStarted += delegate
                    {
                        if (_ds == DispatcherState.ShutdownStarted)
                        {
                            CallingTargetingThread();
                        }            
                    };
                
                s_dispatcher.ShutdownFinished += delegate
                    {
                        if (_ds == DispatcherState.ShutdownFinished)
                        {
                            CallingTargetingThread();
                        }            
                    };


                if (ds == DispatcherState.Running)
                {
                    s_dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (DispatcherOperationCallback)delegate
                        {
                            CallingTargetingThread();

                            DispatcherHelper.ShutDown();
                            return null;
                        }, null);
                }


                if (ds == DispatcherState.Shutdown || ds == DispatcherState.ShutdownStarted ||
                    ds == DispatcherState.ShutdownFinished)
                {
                    DispatcherHelper.ShutDown();
                }
                else if (DispatcherState.NotRunning != ds)
                {                    
                    DispatcherHelper.RunDispatcher();
                }

                if (ds == DispatcherState.Shutdown)
                {
                    CallingTargetingThread();
                }
            }
            else
            {               
                CallingTargetingThread();
            }
        }


        void CallingTargetingThread()
        {
            if (_targetCurrentThread)
            {
                CallFromThread();
            }
            else
            {
                MultipleThreadsAction();
            }
        }

        static private void LogFailed(string s)
        {
            CoreLogger.LogTestResult(false, s);
        }

        private Dispatcher AvalonFromThread(Thread thread)
        {
            return Dispatcher.FromThread(thread);
        }
        
        private static DispatcherState ParsingDispatcherState(string state)
        {
            DispatcherState ds = (DispatcherState)Enum.Parse(typeof(DispatcherState),state,true /* ignore case */ );
            return ds;
        }

        private static ThreadState ParsingThreadState(string state)
        {
            ThreadState ds = (ThreadState)Enum.Parse(typeof(ThreadState),state,true /* ignore case */ );
            return ds;
        }

        private static bool ParsingCurrentTargetThread(string state)
        {
            if (String.Compare(state, "Current", true) == 0)
            {
                return true;
            }
            return false;
        }

        private static bool ParsingCallMultipleTimes(string state)
        {
            if (String.Compare(state, "Multiple", true) == 0)
            {
                return true;
            }
            return false;
        }
        
        static int s_count = 0;
        static AutoResetEvent s_ev = new AutoResetEvent(false);
        static Dispatcher s_dispatcher = null;
        DispatcherState _ds = DispatcherState.Running; 
        ThreadState _ts = ThreadState.Running;
        WeakReference _threadWeak = null;
        Thread _thread = null;
        bool _targetCurrentThread = false;
        bool _multipleCalls = false;

        static List<Exception> s_exceptionList = new List<Exception>();
        static object s_syncRoot = new object();
        
    }
}



