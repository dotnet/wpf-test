// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Collections;
using Microsoft.Test.Logging;
using Microsoft.Test;

/******************************************************************************
 * 
 * Contains code that helps waiting for a signal sent asynchronously.
 * 
 *****************************************************************************/

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Contains code that helps waiting for a signal sent asynchronously.
    /// </summary>
    public class DispatcherSignalHelper
    {
        // private data
        private SignalTable _signals = new SignalTable();
        const int defaultTimeout = 30000;

        internal Result WaitForSignal()
        {
            return WaitForSignal("Default", 30000);
        }

        internal Result WaitForSignal(string name)
        {
            return WaitForSignal(name, defaultTimeout);
        }

        internal Result WaitForSignal(int timeout)
        {
            return WaitForSignal("Default", timeout);
        }

        internal Result WaitForSignal(string name, int timeout)
        {
            TimeoutFrame frame = new TimeoutFrame();
            AutoSignal signal = _signals[name];
            signal.Frame = frame;

            FrameTimer timeoutTimer = new FrameTimer(frame, timeout, new DispatcherOperationCallback(DispatcherHelper.TimeoutFrameOperation), DispatcherPriority.Send);
            timeoutTimer.Start();

            //Pump the dispatcher
            Dispatcher.PushFrame(frame);

            //abort the operations that did not get processed
            signal.Frame = null;
            if (!timeoutTimer.IsCompleted)
            {
                timeoutTimer.Stop();
            }

            if (frame.TimedOut)
                GlobalLog.LogStatus("A Timeout occurred.");

            Result result = signal.Result;
            signal.Reset();
            return result;
        }

        internal void Signal(Result result)
        {
            Signal("Default", result);
        }

        internal void Signal(string name, Result result)
        {
            _signals[name].Signal(result);
        }

        internal class AutoSignal
        {
            DispatcherFrame _frame;
            Microsoft.Test.Result _signalResult = Microsoft.Test.Result.Ignore;
            bool _isSet = false;

            public DispatcherFrame Frame
            {
                get { return _frame; }
                set
                {
                    _frame = value;
                    if (value != null)
                        _frame.Continue = !_isSet;
                }
            }

            public Result Result
            {
                get { return _signalResult; }
            }

            public void Signal(Result result)
            {
                _isSet = true;
                _signalResult = result;
                if (_frame != null)
                    _frame.Continue = false;
            }

            public void Reset()
            {
                _isSet = false;
                _signalResult = Microsoft.Test.Result.Ignore;
            }
        }

        internal class SignalTable
        {
            Hashtable _table = new Hashtable();

            public AutoSignal this[string name]
            {
                get
                {
                    AutoSignal signal;
                    lock (_table)
                    {
                        signal = _table[name] as AutoSignal;
                        if (signal == null)
                            _table[name] = signal = new AutoSignal();
                    }
                    return signal;
                }
            }
        }
    }
}
