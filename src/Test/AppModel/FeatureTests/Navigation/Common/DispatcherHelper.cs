// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Threading;
using Microsoft.Test.Logging;

/******************************************************************************
 * 
 * Contains code that allows to process events from the dispatcher event queue
 * for a certain period of time or until operations with a certain dispatcher 
 * priority are being executed.
 * 
 *****************************************************************************/

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Helper that allows to process events for a period of time or until a priority is reached.
    /// </summary>
    internal static class DispatcherHelper
    {
        const int defaultTimeout = 30000;

        internal static object ExitFrameOperation(object obj)
        {
            DispatcherFrame frame = obj as DispatcherFrame;
            frame.Continue = false;
            return null;
        }

        internal static object TimeoutFrameOperation(object obj)
        {
            TimeoutFrame frame = obj as TimeoutFrame;
            frame.Continue = false;
            frame.TimedOut = true;
            return null;
        }
    }

    internal class FrameTimer : DispatcherTimer
    {
        DispatcherFrame _frame;
        DispatcherOperationCallback _callback;
        bool _isCompleted = false;

        public FrameTimer(DispatcherFrame frame, int milliseconds, DispatcherOperationCallback callback, DispatcherPriority priority)
            : base(priority)
        {
            this._frame = frame;
            this._callback = callback;
            Interval = TimeSpan.FromMilliseconds(milliseconds);
            Tick += new EventHandler(OnTick);
        }

        public DispatcherFrame Frame
        {
            get { return _frame; }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
        }

        void OnTick(object sender, EventArgs args)
        {
            _isCompleted = true;
            Stop();
            _callback(_frame);
        }
    }

    internal class TimeoutFrame : DispatcherFrame
    {
        bool _timedout = false;

        public bool TimedOut
        {
            get { return _timedout; }
            set { _timedout = value; }
        }
    }
}
