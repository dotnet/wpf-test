// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
* Description:
* stress test engine. This can be used only with Avalon UI. 
*
*
\***************************************************************************/

using System;
using System.Threading; using System.Windows.Threading;


namespace Test.Uis.Stress
{
    /// <summary>
    /// This is the state for the stress engine
    /// </summary>
    public enum UisStressStatus : uint
    {
        /// <summary>
        /// stress stopped
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// stress suspended
        /// </summary>
        Suspended = 1,

        /// <summary>
        /// stress running
        /// </summary>
        Running = 2,

        /// <summary>
        /// stress pending start
        /// </summary>
        WaitingStart = 3
    }

    /// <summary>
    /// This is ths stress engine class. It manages the interval and timer firing stuff so
    /// main class doesn't need to worry about all those
    /// </summary>
    public class UisStress
    {
        private DispatcherTimer                 _stressTimer;
        private EventHandler            _timerHandler;
        private OnStressProc            _timerEventProc;

        /// <summary>
        /// These are variables for delay start a stress
        /// </summary>
        private DispatcherTimer                 _delayStressTimer;
        private EventHandler            _delayTimerHandler;
        private OnStressProc            _delayTimerEventProc;
        private TimeSpan                _timerInterval;
        
        private UisStressStatus         _stressStatus;
        private uint 			_iStressProcCalledFromStart;

        /// <summary>
        /// ctor
        /// </summary>
        public UisStress()
        {
            this._stressTimer = null;
            this._timerHandler = null;
            this._timerEventProc = null;

            this._delayStressTimer = null;
            this._delayTimerHandler = null;
            this._delayTimerEventProc = null;
            
            this._iStressProcCalledFromStart = 0;
            
            this._stressStatus = UisStressStatus.Stopped;
        }

        /// <summary>
        /// we need to stop stress here
        /// </summary>
        ~UisStress()
        {
            if (this._stressStatus != UisStressStatus.Stopped)
            {
                this.StopStress();
            }
        }

        /// <summary>
        /// This is the timer proc to RawTimer.
        /// If StressStatus is not running we don't call 
        /// the delegate
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void OnTimerEvent(object source, EventArgs e)
        {
            if (this._stressStatus == UisStressStatus.Running)
            {
                this._iStressProcCalledFromStart++;
            	
                this._timerEventProc(this._iStressProcCalledFromStart);
            }
        }

        /// <summary>
        /// We can start stress only if:
        /// 1. StressStatus == Stopped, or
        /// 2. StressStatus == WaitingStart
        /// </summary>
        /// <param name="timerEventHandler">event handler when the timer fires</param>
        /// <param name="stressInterval">Timer interval</param>
        
        public void StartStress(OnStressProc timerEventHandler, TimeSpan stressInterval)
        {
            if (this._stressStatus == UisStressStatus.Stopped
                || this._stressStatus == UisStressStatus.WaitingStart)
            {
                if (this._stressStatus == UisStressStatus.WaitingStart)
                {
                    this._delayStressTimer.Tick -= this._delayTimerHandler;

                    this._delayTimerHandler = null;
                    this._delayTimerEventProc = null;
                }

                this._timerEventProc = timerEventHandler;

                this._stressTimer = new DispatcherTimer();

                this._stressTimer.Interval = stressInterval;

                this._timerHandler = new EventHandler(OnTimerEvent);

                this._stressTimer.Tick += this._timerHandler;

                this._stressStatus = UisStressStatus.Running;
            }
        }

        /// <summary>
        /// When this timer event happens, we stop the 
        /// waiting start timer and call StartStress
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void DelayTimerEventInternal(object source, EventArgs e)
        {
            this.StartStress(this._delayTimerEventProc, this._timerInterval);
        }

        /// <summary>
        /// This function allows main program to wait for an initial time
        /// before stress goes on
        /// </summary>
        /// <param name="timerEventHandler"></param>
        /// <param name="stressInterval"></param>
        /// <param name="delayStart"></param>
        public void DelayStartStress(OnStressProc timerEventHandler, TimeSpan stressInterval, TimeSpan delayStart)
        {
            if (this._stressStatus == UisStressStatus.Stopped)
            {
                this._delayStressTimer = new DispatcherTimer();

                this._delayStressTimer.Interval = delayStart;
                
                this._timerInterval = stressInterval;
            
                this._delayTimerHandler = new EventHandler(DelayTimerEventInternal);

                this._delayStressTimer.Tick += this._delayTimerHandler;

                this._delayTimerEventProc = timerEventHandler;

                this._stressStatus = UisStressStatus.WaitingStart;
            }
        }

        /// <summary>
        /// To suspend stress
        /// </summary>
        public void SuspendStress()
        {
            if (this._stressStatus != UisStressStatus.Running)
            {
                return;
            }
            else
            {
                this._stressStatus = UisStressStatus.Suspended;
            }
        }

        /// <summary>
        /// To resume stress 
        /// </summary>
        public void ResumeStress()
        {
            if (this._stressStatus != UisStressStatus.Suspended)
            {
                return;
            }
            else
            {
                this._stressStatus = UisStressStatus.Running;
            }
        }

        /// <summary>
        /// get and set properties 
        /// </summary>
        public TimeSpan StressInterval
        {
            get
            {
                return this._stressTimer.Interval;
            }
            set
            {
                OnStressProc timerEventProcTemp = this._timerEventProc;
                this.StopStress();
                this.StartStress(timerEventProcTemp, value);
            }
        }

        /// <summary>
        /// expose StressStatus to outside world 
        /// </summary>
        public UisStressStatus StressStatus
        {
            get
            {
                return this._stressStatus;
            }
        }

        /// <summary>
        /// Stop the stress, kill the timer 
        /// </summary>
        public void StopStress()
        {
            if (this._stressStatus == UisStressStatus.WaitingStart)
            {
                this._delayStressTimer.Tick -= this._delayTimerHandler;

                this._delayTimerHandler = null;
                this._delayTimerEventProc = null;

                this._stressStatus = UisStressStatus.Stopped;
                
            }
            else if (this._stressStatus != UisStressStatus.Stopped)
            {
                this._stressTimer.Tick -= this._timerHandler;

                this._timerEventProc = null;

                this._timerHandler = null;
                                
                this._stressStatus = UisStressStatus.Stopped;
            }
        }

        /// <summary>
        /// This is the callback for the stress 
        /// </summary>
        public delegate void OnStressProc(uint iStressProcCalledFromStart);
    }
}
