// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Wrap the DispatcherTimer and reuse the functionality
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Modeling;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using System.Diagnostics;

namespace Avalon.Test.CoreUI.Threading
{
    /// <summary>
    /// </summary>
    internal class DispatcherTimerWrapper
    {
        #region Public methods

        /// <summary>
        /// </summary>   
        public override string ToString()
        {
            return string.Format("Timer Created: time: {0}; ThreadCreation: {1}; TargetDispatcher: {2}", _duration.ToString(), Thread.CurrentThread.GetHashCode(), _timerDispatcher.RealDispatcher.Thread.GetHashCode());
        }


        /// <summary>
        /// </summary>
        public void CreateDispatcherTimer()
        {
            if (Thread.CurrentThread == null)
                GlobalLog.LogStatus("Current thread is null.");

            if (_timerDispatcher.RealDispatcher != null)
                GlobalLog.LogStatus(string.Format("Timer Created: [time: {0}] [ThreadCreation: {1}] [TargetDispatcher: {2}]", 
                    _duration.ToString(), Thread.CurrentThread.GetHashCode(), _timerDispatcher.RealDispatcher.Thread.GetHashCode()));
            else
                GlobalLog.LogStatus(string.Format("Timer Created: [time: {0}] [ThreadCreation: {1}] [TargetDispatcher: {2}]", 
                    _duration.ToString(), Thread.CurrentThread.GetHashCode(), "Win32 Dispatcher"));
                

            switch (_constructor)
            {

                case DispatcherTimerConstructor.Default:
                    _timer = new DispatcherTimer();
                    break;
                case DispatcherTimerConstructor.PriorityOnly:
                    _timer = new DispatcherTimer(_priority);
                    break;
                case DispatcherTimerConstructor.PriorityAndDispatcher:
                    if (_timerDispatcher.RealDispatcher != null)
                    {
                        _timer = new DispatcherTimer(_priority, _timerDispatcher.RealDispatcher);
                    }
                    else
                    {
                        // Hack: Model was not created with restriction that Win32 dispatcher type would not use this constructor.
                        _timer = new DispatcherTimer(_priority);
                    }
                    break;
                default:
                    throw new Microsoft.Test.TestSetupException("DispatcherTimerConstructor invalid or not set.");
            }

            _timer.Tag = this;
            _timer.Tick += new EventHandler(TickHandler);
            _timer.Interval = TimeSpan.FromMilliseconds(_duration);


            switch(_howToStart)
            {
                case TimerControl.StartStopMethod:
                    _watch = Stopwatch.StartNew();
                    _timer.Start();
                    break;
                case TimerControl.EnabledProperty:            
                    _watch = Stopwatch.StartNew();
                    _timer.IsEnabled = true;
                    break;
                default:
                    throw new Microsoft.Test.TestSetupException("HowToStart method invalid or not set.");
            }
        }

        /// <summary>
        /// </summary>
        public static void TickHandler(object o , EventArgs args)
        {
            DispatcherTimer timer = (DispatcherTimer)o;
            DispatcherTimerWrapper timerW = (DispatcherTimerWrapper)timer.Tag;

            timerW._watch.Stop();

            // Count is used to verify handler was invoked.
            timerW._timingList.Add(timerW._watch.ElapsedMilliseconds);

            if (timerW._isTimerStop)
            {
                throw new Microsoft.Test.TestValidationException("Timer is stopped. TickHandler should not have been invoked.");
            }

            TimerControl howToStop = timerW._howToStop;
            int repeatCount = timerW._repeatCount;            

            timerW._timesTickFired++;

            if (timerW._timesTickFired > repeatCount)
            {
                switch (howToStop)
                {
                    case TimerControl.StartStopMethod:
                        timer.Stop();
                        break;
                    case TimerControl.EnabledProperty:
                        timer.IsEnabled = false;
                        break;
                    default:
                        throw new Microsoft.Test.TestValidationException("HowToStop method invalid or not set.");
                }

                timerW._isTimerStop = true;
            }

            if (timer.Dispatcher != Dispatcher.CurrentDispatcher)
            {
                throw new Microsoft.Test.TestValidationException("Timer handler invoked on wrong thread.");
            }
        }

        /// <summary>
        /// Validation method asking if all timers were executed correctly.
        /// </summary>
        public bool ValidateTimeFire()
        {
            long time = _duration;

            if (_timingList.Count == 0 )
            {
                CoreLogger.LogStatus("Time handler was not invoked");
                return false;
            }
            
            for (int i = 0; i < _timingList.Count; i++)
            {
                long timing  = _timingList[i];
                if (timing < time - s_lowTolerance)
                {
                    CoreLogger.LogStatus("A time was fired sooner than expected.  Real: " + timing.ToString() + "; Expected: " + time.ToString());
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Private data

        private static readonly long s_lowTolerance = 50;
        private List<long> _timingList = new List<long>();
        private bool _isTimerStop = false;
        private int _timesTickFired = 0;

        private DispatcherTimer _timer = null;

        private Stopwatch _watch = null;
        
        #endregion

        #region Public properties.

        private DispatcherWrapper _timerDispatcher = null;
        public DispatcherWrapper TimerDispatcher
        {
            set { _timerDispatcher = value; }
        }

        private DispatcherPriority _priority = DispatcherPriority.Invalid;
        public DispatcherPriority Priority
        {
            set { _priority = value; }
        }

        private DispatcherTimerConstructor _constructor = DispatcherTimerConstructor.Invalid;
        public DispatcherTimerConstructor Constructor
        {
            set { _constructor = value; }
        }

        private TimerControl _howToStart;
        public TimerControl HowToStart
        {
            set { _howToStart = value; }
        }

        private TimerControl _howToStop;
        public TimerControl HowToStop
        {
            set { _howToStop = value; }
        }

        int _repeatCount = 0;
        public int RepeatCount
        {
            set { _repeatCount = value; }
        }

        int _duration = 0;
        public int Duration
        {
            set { _duration = value; }
        }

        public int TotalDuration
        {
            get { return _duration * _repeatCount; }
        }

        #endregion
    }

    internal enum DispatcherTimerConstructor { Default, PriorityOnly, PriorityAndDispatcher, Invalid=-1 };
    internal enum TimerControl { EnabledProperty, StartStopMethod, Invalid=-1 };
}


