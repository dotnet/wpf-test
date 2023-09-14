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
using Microsoft.Test.Modeling;
using Microsoft.Test.Win32;
using System.Runtime.InteropServices;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
namespace Avalon.Test.CoreUI.Threading
{

    /// <summary>
    /// This class is a way to schedule actions perform on a DispatcherOperationWrappers.
    /// </summary>   
    class OperationAction
    {
        /// <summary>
        /// Name for the Action to Perform.
        /// </summary>   
        public string Name = null;

        /// <summary>
        /// Argument use on the action.
        /// </summary>   
        public object Arg = null;

        /// <summary>
        /// Reference to the operation where the action will be performed.
        /// </summary>           
        public DispatcherOperationWrapper Operation = null;

        /// <summary>
        /// Reference to the DispatcherWrapper where the action will be perform.
        /// </summary>           
        public DispatcherWrapper DW = null;
    }

    /// <summary>
    /// Types of callbacks that can be used on the BeginInvoke.
    /// </summary>   
    enum InvokeCallbackTypes
    {
        One_Param_DispatcherOperationCallback,
        One_Param_SendOrPost,
        One_Param_Generic,
        Two_Param_Generic,
        Three_Param_Generic,
        Zero_Param_Generic,
    }

    /// <summary>
    /// Generic 0 argument delegate to exercise all the callpaths for
    /// BeginInvoke.
    /// </summary>
    public delegate int ZeroParamGeneric();

    /// <summary>
    /// Generic 1 argument delegate to exercise all the callpaths for
    /// BeginInvoke.
    /// </summary>
    public delegate int OneParamGeneric(object o);

    /// <summary>
    /// Generic 2 arguments delegate to exercise all the callpaths for
    /// BeginInvoke.
    /// </summary>
    public delegate void TwoParamGeneric(object o1, object o2);

    /// <summary>
    /// Generic 3 arguments delegate to exercise all the callpaths for
    /// BeginInvoke.    
    /// </summary>
    public delegate void ThreeParamGeneric(object o1, object o2, object o3);


    /// <summary>
    /// This is a convinience wrapper around DispatcherOperation.
    /// In this way we chould validate that the operations perform over
    /// the dispatcheroperation are correct and the result expected.
    /// </summary>
    class DispatcherOperationWrapper
    {               
        /// <summary>
        /// Constructor a DOW on the specified DispatcherWrapper and the specified periority
        /// the the specified callback.
        /// </summary>
        public DispatcherOperationWrapper(DispatcherWrapper dispatcherW, DispatcherPriority priority, InvokeCallbackTypes beginInvokeCallbackType)
        {
            
            _beginInvokeCallbackType = beginInvokeCallbackType;
            _priority = priority;
            _dispatcherW = dispatcherW;


            // The compiler was complaining the we never use this
            // so this is a hack.
            if (!_isDispatched)
                _isDispatched=false;
            if (!_isAbortedNotification)
                _isAbortedNotification=false;
            if (!_timerFired)
            _timerFired = false;
          
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Abort()
        {

            bool isAborted = _op.Abort();
            if (isAborted)
            {
                if (!_isAbortedNotification || _op.Status != DispatcherOperationStatus.Aborted)
                {
                    throw new Microsoft.Test.TestValidationException("The Aborted event was not fired.");
                }
            }
            else
            {
                if (_op.Status == DispatcherOperationStatus.Pending)
                {
                    CoreLogger.LogStatus("Possible a known race condition.");
                    if (DispatcherHelper.IsDispatcherOperationEnqueued(_op))
                    {
                        CoreLogger.LogTestResult(false,"The operation cannot be pending");
                        throw new Microsoft.Test.TestValidationException("The operation cannot be pending.");
                    }
                }
            }
            return isAborted;
        }

        EventHandler _callback = null;
            
        /// <summary>
        /// Calls BeginInvoke using the parameter passed on the constructor.
        /// </summary>
        public void BeginInvoke()        
        {
            BeginInvoke(null);
        }


        /// <summary>
        /// Calls BeginInvoke using the parameter passed on the constructor.
        /// </summary>
        public void BeginInvoke(EventHandler callback)
        {
            _callback = callback;
            if (_beginInvokeCallbackType == InvokeCallbackTypes.One_Param_DispatcherOperationCallback)
            {
                // Using DispatcherOperationCallback.
                
                SetOperation(_dispatcherW.RealDispatcher.BeginInvoke(_priority,  
                    (DispatcherOperationCallback) delegate(object o)
                    {
                        CommonValidation(o);                        
                        return null;
                    }, this));
            }
            else if (_beginInvokeCallbackType == InvokeCallbackTypes.One_Param_SendOrPost)
            {
                // Using SendOrPostCallback.
                
                SetOperation(_dispatcherW.RealDispatcher.BeginInvoke(_priority, 
                    (SendOrPostCallback)delegate (object o)
                    {
                        CommonValidation(o);                        
                    }, this));
            }
            else if (_beginInvokeCallbackType == InvokeCallbackTypes.One_Param_Generic)                
            {
                // Using a generic 1 parameter delegate.
                
                SetOperation(_dispatcherW.RealDispatcher.BeginInvoke(_priority, 
                    (OneParamGeneric)delegate(object o)
                    {
                        CommonValidation(o);                                                
                        return 0;
                    }, this));
            }
            else if (_beginInvokeCallbackType == InvokeCallbackTypes.Two_Param_Generic)                
            {
                // Using a generic 2 parameters delegate.
                
                SetOperation(_dispatcherW.RealDispatcher.BeginInvoke(_priority, 
                    (TwoParamGeneric)delegate(object o1, object o2)
                    {
                        CommonValidation(o1);                                                
                    }, this, new object()));
            }
            else if (_beginInvokeCallbackType == InvokeCallbackTypes.Three_Param_Generic)                
            {
                // Using a generic 3 parameters delegate.
                
                SetOperation(_dispatcherW.RealDispatcher.BeginInvoke(_priority, 
                    (ThreeParamGeneric)delegate(object o1, object o2, object o3)
                    {
                        CommonValidation(o1);                                                
                    }, this, new object(), new object()));
            }
            else if (_beginInvokeCallbackType == InvokeCallbackTypes.Zero_Param_Generic)
            {
                SetOperation(_dispatcherW.RealDispatcher.BeginInvoke(_priority, 
                    (ZeroParamGeneric)delegate
                    {
                        s_zeroSpecialArg = this;
                        CommonValidation(this);                                                
                        return 1;
                    }));

            }
        }


        static DispatcherOperationWrapper s_zeroSpecialArg = null;
            
        /// <summary>
        /// This is a common validation that it is called everytime that a BeginInvoke call
        /// is dispatched.
        /// </summary>
        private static void CommonValidation(object o)
        {
            DispatcherOperationWrapper opW = ((DispatcherOperationWrapper)o);

            if (opW.Status != DispatcherOperationStatus.Executing)
            {
                throw new Microsoft.Test.TestValidationException("The status is not Executing for this operation");
            }
            
            if (opW._isDispatched)
            {
                throw new Microsoft.Test.TestValidationException("This item was executing before.");                
            }
            
            opW._isDispatched = true;

            if (opW._callback != null)
            {
                opW._callback(opW, EventArgs.Empty);
            }


            if (opW.Status != DispatcherOperationStatus.Executing)
            {
                throw new Microsoft.Test.TestValidationException("The status is not Executing for this operation");
            }
        }

        /// <summary>
        /// Return the DispatcherWrapper where this will be executed.
        /// </summary>
        public DispatcherWrapper DispatcherW
        {
            get
            {
                return _dispatcherW;
            }
        }

        /// <summary>
        /// Perform the action over the dispatcherOperation specified on the
        /// string passed as parameter.
        /// </summary>
        public void DoAction(OperationAction operationAction)
        {
            Log(operationAction.Name);      
            
            if (operationAction.Name == "Wait")
            {
                this.Wait();
            }
            else if (operationAction.Name == "Wait_Timeout")
            {
                this.Wait(TimeSpan.FromMilliseconds(50));               
            }
            else if (operationAction.Name == "PriorityChange")
            {                
                this.Priority = (DispatcherPriority)operationAction.Arg;
            }
            else if (operationAction.Name == "Abort")
            {                
                this.Abort();
            }
        }
        

        /// <summary>
        /// Set and return the priority for the DispatcherOperation.
        /// </summary>        
        public DispatcherPriority Priority
        {
            get
            {
                return _op.Priority;
            }
            set
            {
                _priority = value;
                _op.Priority = value;
                if (Priority == DispatcherPriority.Inactive && _isSomeThreadWaitingCount > 0)
                {
                    AddTimerForInactivePriorityandWait();
                }

            }
        }

        /// <summary>
        /// Return the status for the DOp.
        /// </summary>     
        public DispatcherOperationStatus Status
        {
            get
            {
                if (_op != null)
                {
                    return _op.Status;
                }
                return DispatcherOperationStatus.Completed;                
            }
        }

        /// <summary>
        /// Wait for the DOp to be dispatched.
        /// </summary> 
        public void Wait()
        {
            Wait(TimeSpan.MinValue);
        }



        /// <summary>
        /// Wait for the DOp to be dispatched for the specified amount of time.
        /// </summary> 
        public void Wait(TimeSpan timeout)
        {
            
            // Another Hack!
            AddTimerForInactivePriorityandWait();

            if (Dispatcher.CurrentDispatcher == _op.Dispatcher)
            {
                InvokeWrapperHelper.WaitUsingSameDispatcherThread(this, timeout);                
            }
            else
            {
                InvokeWrapperHelper.AddWaitToThreadPool(this, timeout);
            }

        }


        public int AbortedCount
        {
            get
            {
               return _abortedCount;
            }
        }


        public int CompletedCount
        {
            get
            {
               return _completedCount;
            }
        }


        public int PriorityChangedCount
        {
            get
            {
               return _prioritychangedCount;
            }
        }

       
        /// <summary>
        /// This is the real method that calls into DispatcherOperation.Wait(*)  
        /// Only the special helper funtion must call this method. Use the 
        /// public API (Wait).
        /// </summary>
        /// <param name="timeSpan">If TimeSpan.MinValue is passed, DispatcherOperation.Wait will be called.
        /// If any other time is passed we will use the DispatcherOperation.Wait(TimeSpan) API.</param>
        internal void WaitInternal(TimeSpan timeSpan)
        {
            try
            {
                Interlocked.Increment(ref _isSomeThreadWaitingCount);
                if (timeSpan == TimeSpan.MinValue)
                {
                    Log("Wait Time : Infinite");
                    _op.Wait();
                }
                else
                {
                    Log("Wait Time : " + timeSpan.ToString());
                    _op.Wait(timeSpan);
                }
            }
            finally
            {
                Interlocked.Decrement(ref _isSomeThreadWaitingCount);
            }
        }



        /// <summary>
        /// This method is called every time that a DOp is aborted.
        /// </summary>
        private  void AbortedHandler(object o, EventArgs args)
        {     
            Interlocked.Increment(ref _abortedCount);
            
            if (_isDispatched)
            {
                throw new Microsoft.Test.TestValidationException("The Aborted event was fired but the Operation was executed.");
            }
            if (_isAbortedNotification)
            {
                throw new Microsoft.Test.TestValidationException("The Aborted event was fired but the Aborted test flag should be false.");
            }   
            _isAbortedNotification = true;
        }
        
        private void AddTimerForInactivePriorityandWait()
        {

            // If we have an inactive item that we call Wait on it, 
            // we need to promote this item to a different priority.

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += delegate (object source, System.Timers.ElapsedEventArgs e)
            {
                _timerFired = true;
                ((System.Timers.Timer)source).Close();

                if (Priority == DispatcherPriority.Inactive)
                {
                    Priority = DispatcherPriority.Normal;
                }
            };
            timer.Interval=70;
            timer.Start();
        
        }

        bool _timerFired = false;

        /// <summary>
        /// This method is called every time that a DOp is dispatched.
        /// </summary>
        private  void CompletedHandler(object o, EventArgs args)
        {
            Interlocked.Increment(ref _completedCount);            
            if (!_isDispatched)
            {
                throw new Microsoft.Test.TestValidationException("The Completed event was fired but the handler was not executed.");
            }
            if (_isAbortedNotification)
            {
                throw new Microsoft.Test.TestValidationException("The Completed event was fired but the operation was aborted.");
            }                
        }

        /// <summary>
        /// During BeginInvoke we call this API to create a binding 
        /// between the DOW and DOp.
        /// </summary>        
        private void SetOperation(DispatcherOperation op)
        {
            if (_op != null)
            {
                throw new InvalidOperationException("Harness Exception no Avalon Code.");
            }

            if (op == null)
            {
                throw new ArgumentNullException();
            }
            
            _op = op;
            
            _op.Completed += new EventHandler(CompletedHandler);
            _op.Aborted += new EventHandler(AbortedHandler);

            if (this.Status != DispatcherOperationStatus.Pending)
            {
                throw new Microsoft.Test.TestValidationException("Expecting Status Pending for this DispatcherOperation.");                
            }
        }

        void Log(string s)
        {
            Console.WriteLine(this.GetHashCode() + " " + s);
        }


        static object s_waitInstance = new object();
        DispatcherPriority _priority;
        InvokeCallbackTypes _beginInvokeCallbackType;
        bool _isDispatched = false;


        int _abortedCount = 0;
        int _completedCount = 0;
        int _prioritychangedCount = 0;

        /// <summary>
        /// This is use to keep track for Wait calls perform over this item.
        /// You should use Interlocked api to increase or decrease the value
        /// to avoid multiple thread issues.
        /// </summary>
        internal int _isSomeThreadWaitingCount = 0;

        bool _isAbortedNotification = false;
        DispatcherOperation _op = null;
        DispatcherWrapper _dispatcherW = null;

    }

}


