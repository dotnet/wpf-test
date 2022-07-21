// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//  Description: Test helper to create content to render hosted in a DocumentViewer.

using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using System.Security.Permissions;
using System.Security;

namespace Avalon.Test.Annotations
{
    public class DispatcherHelper
    {

        #region DoEvents
        /// <summary>
        /// Empties the queue at all priorities above SystemIdle.  This
        /// effectively enables a caller to do all pending Avalon work
        /// before continuing.
        /// </summary>

        /// <remarks>
        /// This enqueues a dummy SystemIdle item and pushes a
        /// dispatcher frame.  When the item is eventually
        /// dispatched, it discontinues the dispatcher frame and control
        /// returns to the caller of DoEvents().
        /// 

        /// Pushing a frame causes the dispatcher to pump messages in 
        /// a nested loop.  Those messages are the way all Avalon
        /// work gets initiated.
        /// </remarks>
        static public void DoEvents()
        {
            DispatcherHelper.DoEvents(0);

        }

        /// <summary>
        /// Empties the queue at all priority until the specified time expires after that time
        /// it will drain the queue past the specified priority.  This
        /// effectively enables a caller to do all pending Avalon work
        /// before continuing.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="minimumWait">An optional minimum number of milliseconds to empty the queue repeatedly. Default: 0.</param>
        /// <param name="priority"></param>
        static public void DoEvents(int minimumWait, DispatcherPriority priority)
        {
            // Create a timer for the minimum wait time.
            // When the time passes, the Tick handler will be called,
            // which allows us to stop the dispatcher frame.
            DispatcherTimer timer = new DispatcherTimer(priority);
            timer.Tick += new EventHandler(OnDispatched);
            timer.Interval = TimeSpan.FromMilliseconds(minimumWait);

            // Run a dispatcher frame.            
            DispatcherFrame dispatcherFrame = new DispatcherFrame();
            timer.Tag = dispatcherFrame;
            timer.Start();            
            Dispatcher.PushFrame(dispatcherFrame);
        }

        /// <summary>
        /// Empties the queue at all priority until the specified time expires after that time
        /// it will drain the queue past SystemIdle priority.  This
        /// effectively enables a caller to do all pending Avalon work
        /// before continuing.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="minimumWait">An optional minimum number of milliseconds to empty the queue repeatedly. Default: 0.</param>
        static public void DoEvents(int minimumWait)
        {
            DoEvents(minimumWait, DispatcherPriority.SystemIdle);
        }


        /// <summary>
        /// Empties the queue at all priorities above or equal to Input.  This
        /// effectively enables a caller to do all pending Avalon work
        /// before continuing.
        /// </summary>

        static public void DoEventsPastInput()
        {
            DoEvents(0, DispatcherPriority.Input);
        }



        /// <summary>
        /// Empties the queue at all priorities above or equal the specified priority.  This
        /// effectively enables a caller to do all pending Avalon work
        /// before continuing.
        /// </summary>
        static public void DoEvents(DispatcherPriority minimumPriority)
        {
            DoEvents(0, DispatcherPriority.Input);
        }


        /// <summary>
        /// Dummy SystemIdle dispatcher item.  This discontinues the current
        /// dispatcher frame so control can return to the caller of DoEvents().
        /// </summary>
        static private void OnDispatched(object sender, EventArgs args)
        {
            // Stop the timer now.
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            DispatcherFrame frame = (DispatcherFrame)timer.Tag;
            frame.Continue = false;
        }

        #endregion DoEvents
    }  // class DispatcherHelper
} // end namespace
