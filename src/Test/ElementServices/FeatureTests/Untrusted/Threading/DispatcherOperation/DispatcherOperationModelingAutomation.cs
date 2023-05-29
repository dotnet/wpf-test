// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Collections;
using Microsoft.Win32;
using Microsoft.Test.Modeling;
using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Logging;
using Avalon.Test.CoreUI.Win32;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Threading
{
    
    /// <summary>
    /// Modeling Automation for Operation.Complete() this is the automation for the 
    /// the model located at base\testcases\threading\dispatcheroperationmodel.mbt
    /// on WindowsTestData depot
    /// </summary>
    public class DispatcherOperationModelingAutomation : ModelAutomationBase
    {

        /// <summary>
        /// </summary>
        public DispatcherOperationModelingAutomation(AsyncActionsManager asyncManager):base(asyncManager)
        {
            lock (s_globalObj)
            {
                s_counter++;
            }
            _threadTestMTA = new ThreadTest("TestCount: " + s_counter.ToString(), ApartmentState.MTA);
            _threadTestSTA = new ThreadTest("TestCount: " + s_counter.ToString(), ApartmentState.STA);
        }


        /// <summary>
        /// </summary>
        public override void Dispose()
        {
            if (!IsDisposed)
            {
                base.Dispose();
                Log("Dispatcher Dispose");
                if (DispatcherRunning == DispatcherType.Avalon)
                {
                    // HACK becuase ShutDown Post a Quit Message that have weird behavior          
                    DispatcherHelper.ShutDown(_dispatcher);
                    //Win32GenericMessagePump.Run().
                }
                else if (DispatcherRunning == DispatcherType.Win32)
                {
                    //_dispatcher.InvokeShutdown().

                    // HACK becuase ShutDown Post a Quit Message that have weird behavior
                    //if (Win32GenericMessagePump.Count == 0)
                    //{
                    //    Win32GenericMessagePump.Run().
                    //}
                    Win32GenericMessagePump.Stop();
                }

                DispatcherRunning = DispatcherType.None;
                _threadTestMTA.Stop();
                _threadTestSTA.Stop();
                
            }
        }


        /// <summary>
        /// </summary>
        public void ChooseDispatcher(string dispatcherType)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            GetModelNextAction(null);
            Log("Dispatcher choose: " + dispatcherType);
            if (String.Compare("Win32", dispatcherType,true) == 0)
            {
                DispatcherRunning = DispatcherType.Win32;
                Win32GenericMessagePump.Run();
            }
            else if (String.Compare("Avalon", dispatcherType,true) == 0)
            {
                DispatcherRunning = DispatcherType.Avalon;
                DispatcherHelper.RunDispatcher();
            }
            else
            {
                DispatcherRunning = DispatcherType.None;
                throw new ArgumentException("You only can pass Win32 or Avalon as option");
            }   
            
        }

        /// <summary>
        /// </summary>
        public void KeepDispatcherBusy(TimeSpan timeSpan)
        {
            Log("Keep busy for : " + timeSpan.TotalMilliseconds.ToString());
            
            if (timeSpan.TotalMilliseconds != 0)
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new DispatcherOperationCallback(callbackKeepDispatcherBusy),
                    timeSpan);
            }

        }

        object callbackKeepDispatcherBusy(object o)
        {
            TimeSpan timeSpan = (TimeSpan)o;
            Thread.Sleep((int)timeSpan.TotalMilliseconds);
            Log("Keep busy Done" );
            return null;
        }



        /// <summary>
        /// </summary>
        public  void Validate()
        {
            if (!_ispassed)
            {
                throw new Microsoft.Test.TestValidationException("The Item was not executed");
            }

            if (_threadTestMTA.Exception != null)
            {
                Log("Exception ocurred on a worker thread");
                Log(_threadTestMTA.Exception.Message);
                Log(_threadTestMTA.Exception.StackTrace.ToString());
                throw _threadTestMTA.Exception;
            }


            if (_threadTestSTA.Exception != null)
            {
                Log("Exception ocurred on a worker thread");
                Log(_threadTestSTA.Exception.Message);
                Log(_threadTestSTA.Exception.StackTrace.ToString());
                throw _threadTestSTA.Exception;
            }

            GetNextActionOnAvalonQueue(_dispatcher);
        }


        /// <summary>
        /// </summary>
        public void EnqueueOperation(string enqueuedThread, string priority)
        {            
            DispatcherPriority dPriority = (DispatcherPriority)Enum.Parse(typeof(DispatcherPriority), priority);

            Log("Enqueued " + enqueuedThread +"; " + priority);

            if (String.Compare("DispatcherThread", enqueuedThread,true) == 0)  
            {
                _operation = DispatcherHelper.BeginInvokeWrapper(_dispatcher,
                    dPriority, 
                    new DispatcherOperationCallback (enqueueCallback), 
                    null);
            }else if (String.Compare("WorkerThreadMTA", enqueuedThread,true) == 0)  
            {
                _threadTestMTA.BeginInvoke(new TestThreadOperationCallback (threadEnqueueCallback), dPriority);
            }else if (String.Compare("WorkerThreadSTA", enqueuedThread,true) == 0)  
            {
                _threadTestSTA.BeginInvoke(new TestThreadOperationCallback (threadEnqueueCallback), dPriority);
            }
            else
            {
                throw new ArgumentException("");
            }

        }

        object enqueueCallback(object o)
        {
            
            Log("Enqueued Item Dispatched on Dispatcher");
            Thread.Sleep(0);
            _ispassed = true;
            GetNextActionOnAvalonQueue(_dispatcher);
            return null;
        }



        object threadEnqueueCallback(object o)
        {
            Log("Enqueued on WorkerThread");
            DispatcherPriority dPriority = (DispatcherPriority)o;
            
            _operation = DispatcherHelper.BeginInvokeWrapper(_dispatcher,
                dPriority, 
                new DispatcherOperationCallback (enqueueCallback), 
                null);
            return null;
        }


        /// <summary>
        /// </summary>
        public void CompleteOperation(string enqueuedThread, string priority, string keepBusyTime, string completeThread, string timeoutComplete)
        {
            int milliseconds = Int32.Parse(keepBusyTime);
            
            KeepDispatcherBusy(TimeSpan.FromMilliseconds(milliseconds));

            EnqueueOperation(enqueuedThread, priority);
            
            Log("Complete Operation: " +  completeThread + "; " + timeoutComplete);
            milliseconds = -1;

            if (timeoutComplete != "None")
            {
                milliseconds = Int32.Parse(timeoutComplete);
            }       

            if (String.Compare("DispatcherThread", completeThread,true) == 0)  
            {
                Log("DispatcherThread Complete Operation");
                while (_operation == null)
                {
                    Thread.Sleep(0);
                }

                if (milliseconds == -1)
                {
                    _operation.Wait();

                    if (_operation.Status != DispatcherOperationStatus.Completed)
                    {
                        throw new Microsoft.Test.TestValidationException("WasCompleted return false");
                    }
                }
                else
                {
                    _operation.Wait(TimeSpan.FromMilliseconds(milliseconds)); 
                    
                }
                Log("DispatcherThread Complete Operation Exited");                            
                
            }else if (String.Compare("WorkerThreadMTA", completeThread,true) == 0)  
            {
                _threadTestMTA.BeginInvoke(new TestThreadOperationCallback (threadCompleteCallback), milliseconds);
            }else if (String.Compare("WorkerThreadSTA", completeThread,true) == 0)  
            {
                _threadTestSTA.BeginInvoke(new TestThreadOperationCallback (threadCompleteCallback), milliseconds);
            }
            else
            {
                throw new ArgumentException("");
            }


        }

        object threadCompleteCallback(object o)
        {
            Log("WorkerThread Complete Operation");
            int milliseconds = (int)o;

            while (_operation == null)
            {
                Thread.Sleep(0);
            }

            if (milliseconds == -1)
            {
                _operation.Wait();

                if (_operation.Status != DispatcherOperationStatus.Completed)
                {
                    throw new Microsoft.Test.TestValidationException("WasCompleted return false");
                }                
            }
            else
            {
                _operation.Wait(TimeSpan.FromMilliseconds(milliseconds));            
            }
            Log("WorkerThread " + Thread.CurrentThread.Name + "Complete Operation Exited");    


            return null;
        }


        bool _ispassed = false;        
        object _instanceObj = new Object();
        ThreadTest _threadTestSTA = null;
        ThreadTest _threadTestMTA = null;
        Dispatcher  _dispatcher = null;
        DispatcherOperation _operation = null;

        static int s_counter = 0;
        static object s_globalObj = new Object();        
    }




}


