// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

////////////////////////////////////////////////////////////////////////////////////////
/// 
/// Helper classes
/// 
/// ////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

using System.Collections;
using Microsoft.Test.RenderingVerification;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Helper functions to block till all queue items are processed.
    /// </summary>
    public static class QueueHelper
    {

        /// <summary>
        /// Helper function to block till for a certain time.
        /// </summary>
        /// <param name="timeout">The time span to block for.</param>
        public static void WaitTillTimeout(TimeSpan timeout)
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            // Set a timer to terminate our loop in the frame after the
            // timeout has expired.
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
            timer.Interval = timeout;
            timer.Tick += delegate(object sender, EventArgs e)
            {
                ((DispatcherTimer)sender).IsEnabled = false;
                frame.Continue = false;
            };
            timer.Start();

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Helper function to block till all queue items are processed, also introduce time
        /// lag between subsequent test runs
        /// </summary>
        /// <param name="postMethod">Method to post</param>
        public static void WaitTillQueueItemsProcessed()
        {
            System.TimeSpan timespan = new System.TimeSpan(0, 0, 0, 0, 1000);
            WaitTillTimeout(timespan);
        }
    }
}
