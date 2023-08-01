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
using Microsoft.Test.Discovery;
using Microsoft.Test.Modeling;
using Microsoft.Test.Win32;
using System.Runtime.InteropServices;
using Microsoft.Test.Threading;
namespace Avalon.Test.CoreUI.Threading
{

    /// <summary>
    /// </summary>
    internal enum InvokingThread
    {
        /// <summary>
        /// </summary>
        Dispatcher = 0,

        /// <summary>
        /// </summary>
        STA = 1,

        /// <summary>
        /// </summary>        
        MTA = 2,

        /// <summary>
        /// </summary>
        ThreadPool = 3
    }


    /// <summary>
    /// </summary>
    internal enum DispatcherState
    {
        /// <summary>
        /// </summary>
        NotRunning = 0,

        /// <summary>
        /// </summary>
        Running = 1,

        /// <summary>
        /// </summary>        
        ShutdownStarted = 2,

        /// <summary>
        /// </summary>
        ShutdownFinished = 3,

        /// <summary>
        /// </summary>
        Shutdown = 4,

        /// <summary>
        /// </summary>
        NoCreated = 5

    }


    
    
    /// <summary>
    /// This class encapsulates the Dispatcher.Invoke call.
    /// </summary>
    class InvokeWrapper
    {
        public InvokeWrapper(
            Dispatcher dispatcher,
            InvokingThread invokingThread,            
            DispatcherPriority priority,
            InvokeCallbackTypes invokeCallbackType,
            EventHandler callback,
            TimeSpan span)
        {

            Init(
                dispatcher,
                invokingThread,
                priority,
                invokeCallbackType,
                callback,
                span,
                true);
        }

        public InvokeWrapper(
            Dispatcher dispatcher,
            InvokingThread invokingThread,            
            DispatcherPriority priority,
            InvokeCallbackTypes invokeCallbackType,
            EventHandler callback)
        {
            Init(
                dispatcher,
                invokingThread,
                priority,
                invokeCallbackType,
                callback,
                new TimeSpan(0),
                false);

        }


        void Init(
            Dispatcher dispatcher,
            InvokingThread invokingThread,
            DispatcherPriority priority,
            InvokeCallbackTypes invokeCallbackType,
            EventHandler callback,
            TimeSpan span,
            bool isTimeSpanSet)
        {
            _isTimeSpanSet = isTimeSpanSet;
            _dispatcher = dispatcher;
            _priority = priority;
            _invokeCallbackType = invokeCallbackType;
            _callback = callback;
            _span = span;
            _invokingThread = invokingThread;
        }
            

        /// <summary>
        /// To avoid running into scenarios where one dispatcher is
        /// Invoking on another dispatcher that it is already
        /// waiting on it (deadlock by design), we don't do the
        /// real Invoke call here.  We rely on a threadpool thread to simulate
        /// another thread calling as a foreign thread.
        /// </summary>
        public void Invoke()
        {            
            if (_invokingThread == InvokingThread.Dispatcher)
            {
                InvokeWrapperHelper.InvokeUsingSameDispatcherThread(this);
            }
            else if (_invokingThread == InvokingThread.ThreadPool)
            {
                InvokeWrapperHelper.AddInvokeToThreadPool(this);
            }
            else if (_invokingThread == InvokingThread.STA)
            {
                InvokeWrapperHelper.AddInvokeToThreadPool(this);
            }
            else if (_invokingThread == InvokingThread.MTA)
            {
                InvokeWrapperHelper.AddInvokeToThreadPool(this);
            }                       
        }
    
        static void CommonValidation(object o)
        {
            InvokeWrapper inWrapper = (InvokeWrapper)o;
            inWrapper._callback(o, EventArgs.Empty);
        }

        


        /// <summary>
        /// In here we execute the REAL Invoke call.
        /// </summary>
        internal void InvokeInternal()
        {
            if (_invokeCallbackType == InvokeCallbackTypes.One_Param_DispatcherOperationCallback)
            {
                // Using DispatcherOperationCallback.                
                if (!_isTimeSpanSet)
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        (DispatcherOperationCallback)delegate(object o)
                        {
                            CommonValidation(o);
                            return null;
                        }, this);
                }
                else
                {
                    _almostInvokeCalled = true;
                    _result = _dispatcher.Invoke(_priority,
                        _span,
                        (DispatcherOperationCallback)delegate(object o)
                        {
                            CommonValidation(o);
                            return null;
                        }, this);
                }
            }
            else if (_invokeCallbackType == InvokeCallbackTypes.One_Param_SendOrPost)
            {
                // Using SendOrPostCallback.
                if (!_isTimeSpanSet)
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        (SendOrPostCallback)delegate(object o)
                        {
                            CommonValidation(o);
                        }, this);
                }
                else
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        _span,                        
                        (SendOrPostCallback)delegate(object o)
                        {
                            CommonValidation(o);
                        }, this);
                }
            }
            else if (_invokeCallbackType == InvokeCallbackTypes.One_Param_Generic)
            {
                // Using a generic 1 parameter delegate.
                if (!_isTimeSpanSet)
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        (OneParamGeneric)delegate(object o)
                        {
                            CommonValidation(o);
                            return 0;
                        }, this);
                }
                else
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        _span,
                        (OneParamGeneric)delegate(object o)
                        {
                            CommonValidation(o);
                            return 0;
                        }, this);

                }                    
            }
            else if (_invokeCallbackType == InvokeCallbackTypes.Two_Param_Generic)
            {
                //

                if (!_isTimeSpanSet)
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        (TwoParamGeneric)delegate(object o1, object o2)
                        {
                            CommonValidation(o1);
                        }, this, new object());
                }
                else
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        _span,
                        (TwoParamGeneric)delegate(object o1, object o2)
                        {
                            CommonValidation(o1);
                        }, this, new object());
                }
                
            }
            else if (_invokeCallbackType == InvokeCallbackTypes.Three_Param_Generic)                
            {
                // Using a generic 3 parameters delegate.
                if (!_isTimeSpanSet)
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        (ThreeParamGeneric)delegate(object o1, object o2, object o3)
                        {
                            CommonValidation(o1);
                        }, this, new object(), new object());
                }
                else
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        _span,
                        (ThreeParamGeneric)delegate(object o1, object o2, object o3)
                        {
                            CommonValidation(o1);
                        }, this, new object(), new object());

                }
                
            }
            else //if (_invokeCallbackType == invokeCallbackTypes.Zero_Param_Generic)
            {
                s_hackWrapper = this;
                // Using a generic 0 parameters delegate.
                if (!_isTimeSpanSet)
                {
                    _almostInvokeCalled = true;                    
                    _result = _dispatcher.Invoke(_priority,
                        (ZeroParamGeneric)delegate()
                        {
                            CommonValidation(s_hackWrapper);
                            return 1;
                        });
                }
                else
                {
                    _almostInvokeCalled = true;
                    _result = _dispatcher.Invoke(_priority,
                        _span,
                        (ZeroParamGeneric)delegate()
                        {
                            CommonValidation(s_hackWrapper);                            
                            return 1;
                        });
                }

            }           
        }

        internal object Result
        {
            get
            {
                return _result;
            }
        }

        object _result = null;


        /// <summary>
        /// </summary>        
        internal Dispatcher Dispatcher
        {
            get
            {
                return _dispatcher;
            }
        }

        /// <summary>
        /// </summary>        
        internal DispatcherPriority Priority
        {
            get
            {
                return _priority;
            }
        }

        /// <summary>
        /// </summary>        
        internal InvokeCallbackTypes InvokeCallbackKind
        {
            get
            {
                return _invokeCallbackType;
            }
        }

        /// <summary>
        /// </summary>        
        internal InvokingThread InvokeThread
        {
            get
            {
                return _invokingThread;
            }
        }        


        /// <summary>
        /// </summary>        
        internal bool AlmostInvokeCalled
        {
            get
            {
                return _almostInvokeCalled;
            }
        }    

        private static InvokeWrapper s_hackWrapper;
        bool _almostInvokeCalled = false;  
        Dispatcher _dispatcher;
        DispatcherPriority _priority;
        InvokeCallbackTypes _invokeCallbackType;
        EventHandler _callback;
        TimeSpan _span;
        InvokingThread _invokingThread;
        bool _isTimeSpanSet = false;
        
    }
}

