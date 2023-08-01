// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;


namespace Microsoft.Test.Threading
{

    /// <summary>
    /// This class contains multiples methods that abstract some functionality around
    /// threading.  For example: Timers or executing a delegate on a WaitHandle
    /// </summary>
    public class ThreadingHelper
    {

        /// <summary>
        /// Creates a dispatcher timer with sometimespan, callback.  The last argument is 
        /// stored on the Tag property inside of the timer. The dispatcher is created on the current dispatcher and 
        /// with Normal Priority
        /// </summary>
        static public DispatcherTimer DispatcherTimerHelper(TimeSpan span, EventHandler callback, object o)
        {
            return DispatcherTimerHelper(DispatcherPriority.Normal, Dispatcher.CurrentDispatcher,span, callback, o);
        }
	
        /// <summary>
        /// Creates a dispatcher timer with some priority, timespan, callback.  The last argument is 
        /// stored on the Tag property inside of the timer. The dispatcher is created on the current dispatcher
        /// </summary>
        static public DispatcherTimer DispatcherTimerHelper(DispatcherPriority priority, TimeSpan span, EventHandler callback, object o)
        {
            return DispatcherTimerHelper(priority, Dispatcher.CurrentDispatcher,span, callback, o);
        }

        /// <summary>
        /// Creates a dispatcher timer with some priority, timespan, callback.  The last argument is 
        /// stored on the Tag property inside of the timer. The dispatcher is created on the current dispatcher
        /// </summary>
        static public DispatcherTimer DispatcherTimerHelper(DispatcherPriority priority, Dispatcher dispatcher, TimeSpan span, EventHandler callback, object o)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            if (callback == null)
                throw new ArgumentNullException("callback");
            
            DispatcherTimer dTimer = new DispatcherTimer(priority, dispatcher);
            dTimer.Tag = o;
            dTimer.Interval = span;
            dTimer.Tick += new EventHandler(callback);
            dTimer.Start();

            return dTimer;

        }

        /// <summary>
        /// Pass the WaitHandle that after you signal it will BeginInvoke the callback passed as parameter
        /// with the priority on the parameter
        /// </summary>
        static public RegisteredWaitHandle BeginInvokeOnSignalHandle(WaitHandle handle, Dispatcher dispatcher, DispatcherPriority priority, DispatcherOperationCallback callback, object o)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            if (callback == null)
                throw new ArgumentNullException("callback");
            
            RegisteredWaitHandle rHandle = ThreadPool.RegisterWaitForSingleObject (
                handle, new WaitOrTimerCallback(waitorTimerCallback), 
                new SignalPackage(dispatcher,callback, priority, o), 180000, false);

            return rHandle;

        }

        /// <summary>
        /// The threadpool is going to call this method when the WaitHandle is signaled
        /// </summary>
        static private void waitorTimerCallback(object o, bool timeOut)
        {

            if (!timeOut)
            {
                SignalPackage sp = (SignalPackage)o;

                sp._Dispatcher.BeginInvoke(
                    sp._Priority, 
                    sp._Callback,
                    sp._Tag
                    );            
            }
        }

        /// <summary>
        /// This class is for internal purposes for the BeginInvokeSignalHandle
        /// </summary>
        class SignalPackage
        {
            /// <summary>
            /// 
            /// </summary>
            public SignalPackage(Dispatcher d,DispatcherOperationCallback callback, DispatcherPriority p, object t)
            {
                _Dispatcher = d;
                _Priority = p;
                _Tag = t;
                _Callback = callback;
            }

            /// <summary>
            /// 
            /// </summary>            
            public Dispatcher _Dispatcher = null;

            /// <summary>
            /// 
            /// </summary>            
            public DispatcherPriority _Priority;

            /// <summary>
            /// 
            /// </summary>
            public object _Tag = null;

            /// <summary>
            /// 
            /// </summary>
            public DispatcherOperationCallback _Callback = null;
            
        }
    }
}

