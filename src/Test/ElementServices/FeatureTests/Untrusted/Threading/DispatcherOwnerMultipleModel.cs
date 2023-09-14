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
    /// Stateless Model About the a Dispatcher.FromThread using Multiple Threads
    /// The model can be found at     
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\DispatcherOwnerMultiple.mbt
    /// </summary>
    [Model(@"FeatureTests\ElementServices\DispatcherOwnerMultiple.xtc", 1, @"Threading", TestCaseSecurityLevel.FullTrust, "DispatcherOwnerMultipleModel", 
        Description = "Stateless Model to test Dispatcher.FromThread multiple dispatcher.",
        ExpandModelCases = true, Area = "ElementServices")]    
    public class DispatcherOwnerMultipleModel : CoreModel 
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        public DispatcherOwnerMultipleModel() : base()
        {
            // Initialize properties
            Name = "DispatcherOwnerMultiple";
            Description = "Model DispatcherOwnerMultiple";
            ModelPath = "MODEL_PATH_TOKEN";

            _automation = new DispatcherOwnerMultipleModelAutomation(AsyncActions);

            // Add actions
            AddAction("AddThreads", new ActionHandler(AddThreads));
            AddAction("Query", new ActionHandler(Query));
            AddAction("Repeat", new ActionHandler(Repeat));
            AddAction("WaitForThreadsToEnd", new ActionHandler(WaitForThreadsToEnd));
        }

        private bool AddThreads(State endState, State inParameters, State outParameters)
        {
            _automation.AddThreads(inParameters);
            return true;
        }

        private bool Query(State endState, State inParameters, State outParameters)
        {
            _automation.Query(inParameters);
            return true;
        }

        private bool Repeat(State endState, State inParameters, State outParameters)
        {
            _automation.Repeat(inParameters);
            return true;
        }

        private bool WaitForThreadsToEnd(State endState, State inParameters, State outParameters)
        {
            _automation.WaitForThreadsToEnd(inParameters);
            return true;
        }        
        
        private DispatcherOwnerMultipleModelAutomation _automation;
    }

    ///<summary>
    /// This class implements the automation requeries for the model 
    /// %SDXROOT%\testsrc\windowstestdata\REDIST\client\wcptests\core\Base\Testcases\Threading\DispatcherOwnerMultiple.mbt
    /// This model only tests a single dispatcher.
    ///</summary>
    internal class DispatcherOwnerMultipleModelAutomation : ModelAutomationBase
    {
        private enum WaitForThreadToFinish
        {
            All = 0,
            Half= 1,
            None = 2
        }

        ///<summary>
        ///</summary> 
        public DispatcherOwnerMultipleModelAutomation(AsyncActionsManager asyncManager) : base(asyncManager) { }

        /// <summary>
        /// </summary>
        /// <param name="dictionary"></param>
        public void AddThreads(IDictionary dictionary)
        {
            if (dictionary != null)
            {
                _amountOfDispatchers = Int32.Parse((string)dictionary["AmountDispatcher"]);            
            }

            ResetMultiWaitSignal();
            s_workerThreadStartSignal.Reset();

            // Start the worker threads
            for (int i = 0; i < _amountOfDispatchers; i++)
            {
                Thread t = new Thread(new ThreadStart(WorkerThreadCallback));
                IncrementOutstandingWaitCount();

                lock (s_workerThreads)
                {
                    s_workerThreads.Add(new WeakReference(t));
                }

                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }

            // Signal the worker threads to start
            s_workerThreadStartSignal.Set();
            
            // Wait for all of the worker threads to finish starting
            s_multiWaitSignal.WaitOne();
        }

        /// <summary>
        /// </summary>
        /// <param name="dictionary"></param>
        public void Query(IDictionary dictionary)
        {
            if (dictionary != null)
            {
                _times = Int32.Parse((string)dictionary["Times"]);
            }

            int count = (_times == 2) ? 10 : 1;
            
            for (int i = 0; i < count ; i++)
            {
                ResetMultiWaitSignal();               
                s_workerThreadStartSignal.Reset();

                for (int j = 0; j < s_workerThreads.Count; j++)
                {
                    Thread t = s_workerThreads[j].Target as Thread;
                    IncrementOutstandingWaitCount();

                    if (t != null)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(VerificationWorkItem), t);   
                    }
                    else
                    {
                        s_workerThreads.RemoveAt(j);
                        j--;
                    }
                }

                // Signal the worker threads to start
                s_workerThreadStartSignal.Set();
                
                // Wait for all of the worker threads to finish
                s_multiWaitSignal.WaitOne();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dictionary"></param>
        public void WaitForThreadsToEnd(IDictionary dictionary)
        {
            if (dictionary != null)
            {
                _waitForThreadToFinish = (WaitForThreadToFinish)Int32.Parse((string)dictionary["AmountOfThreads"]);
            }

            if (_waitForThreadToFinish == WaitForThreadToFinish.All)
            {
                ShutdownAllDispatchers();
                CollectGarbage(_waitForThreadToFinish);
            }
        }

        /// <summary>
        /// Shutdown all the Dispatchers
        /// </summary>
        private void ShutdownAllDispatchers()
        {
            int index = 0;

            while (index < s_dispatchers.Count)
            {
                Dispatcher d = s_dispatchers[index].Target as Dispatcher;

                if (d != null)
                {
                    DispatcherHelper.ShutDown(d);
                    index++;
                }
                else
                {
                    s_dispatchers.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Repeat the same actions some amount of times.
        /// </summary>
        /// <param name="dictionary"></param>
        public void Repeat(IDictionary dictionary)
        {
            if (dictionary != null)
            {
                _repeatCount = Int32.Parse((string)dictionary["Times"]);
            }

            for (int i = 0; i < _repeatCount; i++)
            {
                AddThreads(null);
                Query(null);
                WaitForThreadsToEnd(null);
            }

            ShutdownAllDispatchers();

            if (s_exceptionList.Count > 0)
            {
                CoreLogger.LogTestResult(false, "An exception was caught.");
            }
        }

        private void CollectGarbage(WaitForThreadToFinish amount)
        {
            do
            {
                for (int i = 0; i < s_dispatchers.Count; i++)
                {
                    if (!s_dispatchers[i].IsAlive)
                    {
                        s_dispatchers.RemoveAt(i);
                        i--;
                    }                    
                }                

                for (int i = 0; i < s_workerThreads.Count; i++)
                {
                    if (!s_workerThreads[i].IsAlive)
                    {
                        s_workerThreads.RemoveAt(i);
                        i--;
                    }                    
                }    

                GC.WaitForPendingFinalizers();
                GC.Collect(); 
                GC.WaitForPendingFinalizers();                
            }
            while (!IsClean(amount));
        }

        private bool IsClean(WaitForThreadToFinish amount)
        {
            if (WaitForThreadToFinish.All == amount)
            {
                return (s_dispatchers.Count == 0 && s_workerThreads.Count == 0);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// This is the entry point for our worker threads
        /// </summary>
        static private void WorkerThreadCallback()
        {
            // Don't go any further until we've been signaled that all the threads have been started
            s_workerThreadStartSignal.WaitOne();

            lock (s_dispatchers)
            {
                s_dispatchers.Add(new WeakReference(Dispatcher.CurrentDispatcher));
            }

            DecrementOutstandingWaitCount();
            DispatcherHelper.RunDispatcher();
        }

        /// <summary>
        /// A work item callback that verifies the test has succeeded.
        /// </summary>
        /// <param name="targetThread">The target thread</param>
        static private void VerificationWorkItem(object targetThread)
        {
            try
            {
                Thread thread = (Thread)targetThread;

                s_workerThreadStartSignal.WaitOne();

                Dispatcher dispatcher = Dispatcher.FromThread(thread);
                Dispatcher d = null;

                // Find dispatcher
                for (int i = 0; i < s_dispatchers.Count; i++)
                {
                    d = s_dispatchers[i].Target as Dispatcher;

                    if (d == dispatcher)
                    {
                        break;
                    }
                    else
                    {
                        d = null;
                    }
                }

                if (d == null)
                {
                    CoreLogger.LogTestResult(false, "The dispatcher was not found.");
                }
            }
            catch (Exception e)
            {
                lock (s_exceptionList)
                {
                    s_exceptionList.Add(e);
                }
            }
            finally
            {
                // Signal the main thread that it can continue
                DecrementOutstandingWaitCount();
            }
        }

        // These three methods are needed because WaitHandle.WaitAll() is not supported for multiple STA threads (which is our case)
        static private void ResetMultiWaitSignal()
        {
            lock (s_waitCountSyncObject)
            {
                s_outstandingWaitCount = 0;
                s_multiWaitSignal.Reset();
            }
        }

        static private void IncrementOutstandingWaitCount()
        {
            lock (s_waitCountSyncObject)
            {
                s_outstandingWaitCount++;
            }
        }

        static private void DecrementOutstandingWaitCount()
        {
            lock (s_waitCountSyncObject)
            {
                s_outstandingWaitCount--;

                if (s_outstandingWaitCount == 0)
                {
                    s_multiWaitSignal.Set();
                }
            }
        }

        private int _times = 1;
        private int _repeatCount = 1;
        private int _amountOfDispatchers = 0;
        private WaitForThreadToFinish _waitForThreadToFinish;

        static private ManualResetEvent s_workerThreadStartSignal = new ManualResetEvent(false);

        static private List<WeakReference> s_workerThreads = new List<WeakReference>();
        static private List<WeakReference> s_dispatchers = new List<WeakReference>();
        static private List<Exception> s_exceptionList = new List<Exception>();
        
        // These members are needed because WaitHandle.WaitAll() is not supported for multiple STA threads (which is our case)
        static private ManualResetEvent s_multiWaitSignal = new ManualResetEvent(false);
        static private int s_outstandingWaitCount = 0;
        static private object s_waitCountSyncObject = new object();
    }
}
