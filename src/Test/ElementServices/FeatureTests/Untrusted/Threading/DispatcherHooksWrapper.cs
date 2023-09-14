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
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Win32;

using Microsoft.Test.Discovery;
using Microsoft.Test.Modeling;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.Threading
{

    /// <summary>
    /// Maps to all the events available on the DispatcherHooks class.
    /// </summary>
    public enum DispatcherNotification
    {
        /// <summary>
        /// </summary>
        Posted = 0,

        /// <summary>
        /// </summary>
        Aborted,

        /// <summary>
        /// </summary>        
        Completed,

        /// <summary>
        /// </summary>
        PriorityChanged,

        /// <summary>
        /// </summary>        
        Inactive        
    }

    /// <summary>
    /// </summary>
    public delegate void DispatcherHooksWrapperEventHandler(object o, DispatcherHooksWrapperEventArgs args);

    /// <summary>
    /// Convinience wrapper that is tied to the DispatcherWrapper
    /// </summary>
    public class DispatcherHooksWrapper
    {        

        /// <summary>
        /// This is a convinience class that wraps a Dispatcher object
        /// </summary>
        public DispatcherHooksWrapper(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// This is a convinience class that wraps a Dispatcher object
        /// </summary>
        public DispatcherHooksWrapper(DispatcherWrapper dw)
        {
            _dw = dw;
            _dispatcher = _dw.RealDispatcher;
        }


        /// <summary>
        /// The sender is the Dispatcher where the action happened.
        /// The args contains more information about the specified event raised.
        /// </summary>
        public event DispatcherHooksWrapperEventHandler Listener;


        /// <summary>
        /// Counter of all the Notification.
        /// </summary>
        public Dictionary<DispatcherNotification, int> Counters
        {
            get
            {
                _counters[DispatcherNotification.Posted] = _posted;
                _counters[DispatcherNotification.Aborted] = _aborted;
                _counters[DispatcherNotification.Completed] = _completed;
                _counters[DispatcherNotification.PriorityChanged] = _pc;                
                _counters[DispatcherNotification.Inactive] = _inactive;                
                
                return _counters;
            }
        }


        private Dictionary<DispatcherNotification, int> _counters = new Dictionary<DispatcherNotification, int>(5);

        /// <summary>
        /// Last Noticication that was raise for this DispatcherHooksWrapper.
        /// </summary>
        public DispatcherHooksWrapperEventArgs GetLastNotification()
        {
            return _lastNotification;
        }
            

        /// <summary>
        /// Convinience wrapper to add and remove events found at the DispatcherHoooks.
        /// </summary>
        public void AddRemoveEvents(DispatcherNotification notification, bool add)
        {
            int action = (int)notification;

            if (_dispatcher == null && _dw.RealDispatcher != null)
            {
                _dispatcher = _dw.RealDispatcher;
            }

            switch(action)
            {
                case 0:
                    
                    if (add)
                    {
                        DispatcherHelper.GetHooks(_dispatcher).OperationPosted += new DispatcherHookEventHandler(DispatcherHookHandlerPosted);
                    }
                    else
                    {
                        DispatcherHelper.GetHooks(_dispatcher).OperationPosted -= new DispatcherHookEventHandler(DispatcherHookHandlerPosted);
                    }
                    
                    break;

                case 1:

                    if (add)
                    {
                        DispatcherHelper.GetHooks(_dispatcher).OperationAborted += new DispatcherHookEventHandler(DispatcherHookHandlerAborted);
                    }
                    else
                    {
                        DispatcherHelper.GetHooks(_dispatcher).OperationAborted -= new DispatcherHookEventHandler(DispatcherHookHandlerAborted);
                    }

                    break;

                case 2:

                    if (add)
                    {
                        DispatcherHelper.GetHooks(_dispatcher).OperationCompleted += new DispatcherHookEventHandler(DispatcherHookHandlerCompleted);
                    }
                    else
                    {
                        DispatcherHelper.GetHooks(_dispatcher).OperationCompleted -= new DispatcherHookEventHandler(DispatcherHookHandlerCompleted);
                    }

                    break;

                case 3:

                    if (add)
                    {
                        DispatcherHelper.GetHooks(_dispatcher).OperationPriorityChanged += new DispatcherHookEventHandler(DispatcherHookHandlerPC);
                    }
                    else
                    {
                        DispatcherHelper.GetHooks(_dispatcher).OperationPriorityChanged -= new DispatcherHookEventHandler(DispatcherHookHandlerPC);
                    }


                    break;                            

                case 4:

                    if (add)
                    {
                        DispatcherHelper.GetHooks(_dispatcher).DispatcherInactive += new EventHandler(DispatcherHookHandlerInactive);
                    }
                    else
                    {
                        DispatcherHelper.GetHooks(_dispatcher).DispatcherInactive -= new EventHandler(DispatcherHookHandlerInactive);
                    }

                    break;          

            }

        }


        private void DispatcherHookHandlerPosted(object sender, DispatcherHookEventArgs e)
        {
            Interlocked.Increment(ref _posted);
            OnListener(sender, new DispatcherHooksWrapperEventArgs(DispatcherNotification.Posted, e));
        }

        private void DispatcherHookHandlerAborted(object sender, DispatcherHookEventArgs e)
        {
            Interlocked.Increment(ref _aborted);            
            OnListener(sender, new DispatcherHooksWrapperEventArgs(DispatcherNotification.Aborted, e));
        }

        private void DispatcherHookHandlerPC(object sender, DispatcherHookEventArgs e)
        {
            Interlocked.Increment(ref _pc);            
            OnListener(sender, new DispatcherHooksWrapperEventArgs(DispatcherNotification.PriorityChanged, e));
        }

        private void DispatcherHookHandlerInactive(object sender, EventArgs e)
        {
            Interlocked.Increment(ref _inactive);            
            OnListener(sender, new DispatcherHooksWrapperEventArgs(DispatcherNotification.Inactive, null));
        }        

        private void DispatcherHookHandlerCompleted(object sender, DispatcherHookEventArgs e)
        {
            Interlocked.Increment(ref _completed);            
            OnListener(sender, new DispatcherHooksWrapperEventArgs(DispatcherNotification.Completed, e));
        }        

        private void OnListener(object o, DispatcherHooksWrapperEventArgs args)
        {
            if (Listener != null)
            {
                Listener(o,args);
            }
            _lastNotification = args;
        }

        private object _syncRoot = new object();
        private DispatcherWrapper _dw;
        private Dispatcher _dispatcher;
        private DispatcherHooksWrapperEventArgs _lastNotification = null;


        private int _posted = 0;
        private int _aborted = 0;
        private int _pc = 0;
        private int _completed = 0;        
        private int _inactive = 0;                
        
    }

    /// <summary>
    /// </summary>
    public class DispatcherHooksWrapperEventArgs
    {
        /// <summary>
        /// </summary>
        public DispatcherHooksWrapperEventArgs(DispatcherNotification notification, 
            DispatcherHookEventArgs args)
        {
            Notification = notification;
            Args = args;
            if (args != null)
            {
                Operation = args.Operation;
            }
        }
                
        /// <summary>
        /// </summary>
        public DispatcherNotification Notification;

        /// <summary>
        /// </summary>
        public DispatcherHookEventArgs Args;

        /// <summary>
        /// </summary>
        public DispatcherOperation Operation;        
    }
}
