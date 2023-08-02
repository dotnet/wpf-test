// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;

namespace Avalon.Test.CoreUI.Threading
{

    /// <summary>
    /// This class generalize the usage for creating DispatcherTimer
    /// </summary>
    public static class DispatcherTimerHelper
    {

        /// <summary>
        /// Creates a dispatcher timer with sometimespan, callback.  The last argument is 
        /// stored on the Tag property inside of the timer. The dispatcher is created on the current dispatcher and 
        /// with Normal Priority
        /// </summary>
        static public DispatcherTimer CreateNormalPriority(TimeSpan span, EventHandler callback, object o)
        {
            return Create(DispatcherPriority.Normal, Dispatcher.CurrentDispatcher,span, callback, o);
        }
	
        /// <summary>
        /// Creates a dispatcher timer with sometimespan, callback.  The last argument is 
        /// stored on the Tag property inside of the timer. The dispatcher is created on the current dispatcher and 
        /// with Normal Priority
        /// </summary>
        static public DispatcherTimer CreateBackgroundPriority(TimeSpan span, EventHandler callback, object o)
        {
            return Create(DispatcherPriority.Background, Dispatcher.CurrentDispatcher,span, callback, o);
        }

        /// <summary>
        /// Creates a dispatcher timer with some priority, timespan, callback.  The last argument is 
        /// stored on the Tag property inside of the timer. The dispatcher is created on the current dispatcher
        /// </summary>
        static public DispatcherTimer Create(DispatcherPriority priority, TimeSpan span, EventHandler callback, object o)
        {
            return Create(priority, Dispatcher.CurrentDispatcher,span, callback, o);
        }

        /// <summary>
        /// Creates a dispatcher timer with some priority, timespan, callback.  The last argument is 
        /// stored on the Tag property inside of the timer. The dispatcher is created on the current dispatcher
        /// </summary>
        static public DispatcherTimer Create(DispatcherPriority priority, Dispatcher dispatcher, TimeSpan span, EventHandler callback, object o)
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
    }
}

