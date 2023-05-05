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

//using AvalonTools.FrameworkUtils;

using System.Collections;


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

        /// <summary>
        /// Change the priority from Inactive to ApplicationIdle
        /// </summary>
        private static void SchedulePriorityChange(DispatcherOperation op, TimeSpan timeSpan)
        {
            DispatcherTimer dTimer = new DispatcherTimer(DispatcherPriority.Background);
            dTimer.Tick += new System.EventHandler(ChangePriority);
            dTimer.Tag = op;
            dTimer.Interval = timeSpan;
            dTimer.Start();
        }

        /// <summary>
        /// Change priority of the DispatcherOperation
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void ChangePriority(object obj, EventArgs args)
        {
            DispatcherTimer dTimer = obj as DispatcherTimer;
            dTimer.Stop();
            DispatcherOperation op = dTimer.Tag as DispatcherOperation;
            op.Priority = DispatcherPriority.ApplicationIdle;
        }

        /// <summary>
        /// Used to post a method to the context of your ApplicationWindow after a certain amount of time has passed
        /// </summary>
        /// <param name="postMethod">Method to post</param>
        /// <param name="TimerInterval">int time in milliseconds</param>
        public static void PostNextTestStep(DispatcherOperationCallback postMethod, int timerInterval)
        {
            System.Object args = new System.Object();

            System.TimeSpan timespan = new System.TimeSpan(0, 0, 0, 0, timerInterval);

            DispatcherOperation op = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Inactive, postMethod, args);

            SchedulePriorityChange(op, timespan);
        }

        /// <summary>
        /// Post an item to UIContext
        /// </summary>
        /// <param name="postMethod">Method to post</param>
        public static void PostNextTestStep(DispatcherOperationCallback postMethod)
        {
            System.Object args = new System.Object();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, postMethod, args);
        }
    }

}
