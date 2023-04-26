// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Animation;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// SyncTimeline test class.
    /// </summary>
    public class SyncTimeline : Timeline
    {
        /// <summary>
        /// Called by the Clock to create a type-specific clock for this
        /// timeline.
        /// </summary>
        /// <returns>
        /// A clock for this timeline.
        /// </returns>
        /// <remarks>
        /// If a derived class overrides this method, it should only create
        /// and return an object of a class inheriting from Clock.
        /// </remarks>
        protected override Clock AllocateClock()
        {
            SyncClock syncClock = new SyncClock(this);

            return syncClock;
        }

        /// <summary>
        /// Called by the base Freezable class to make this object
        /// frozen.
        /// </summary>
        protected override bool FreezeCore(bool isChecking)
        {
            bool canFreeze = base.FreezeCore(isChecking);
            if (!canFreeze)
            {
                return false;
            }

            return canFreeze;
        }

        /// <summary>
        /// Creates a new SyncClock using this SyncTimeline.
        /// </summary>
        /// <returns>A new SyncClock.</returns>
        public new SyncClock CreateClock()
        {
            return (SyncClock)base.CreateClock();
        }

        /// <summary>
        /// Return the duration from a specific clock
        /// </summary>
        /// <param name="clock">
        /// The Clock whose natural duration is desired.
        /// </param>
        /// <returns>
        /// A Duration quantity representing the natural duration.
        /// </returns>
        protected override Duration GetNaturalDurationCore(Clock clock)
        {
            SyncClock sc = (SyncClock)clock;
            return sc.SyncNaturalDuration;
        }

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
        /// </summary>
        /// <returns>The new Freezable.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new SyncTimeline();
        }
    }
    
    
    /// <summary>
    /// Test class for sync
    /// </summary>
    public class SyncClock : Clock
    {
        /// <summary>
        /// Creates a SyncClock object.
        /// </summary>
        protected internal SyncClock(SyncTimeline timeline)
            : base(timeline)
        {
            if (ITimeBVT.syncDurationTime == null)
            {
                _secretDuration = GetDuration();
            }
            else
            {
                //Emulating the duration returned by Media, as specified by each test case.
                _secretDuration = new Duration((TimeSpan)ITimeBVT.syncDurationTime);
            }
        }

        /// <summary>
        /// Overrides CanSlip value with true
        /// </summary>
        protected override bool GetCanSlip()
        {
            return true;
        }

        /// <summary>
        /// Get the time (e.g. simulated media time) to synchronize to
        /// </summary>
        protected override TimeSpan GetCurrentTimeCore()
        {
            //EXPLANATION: three variables are set by each test case, to simulate slip scenarios 
            //produced by a MediaTimeline ("SyncTimeline"):
            //    o slipBegin: the begin time of the slip period, in milliseconds.
            //    o slipDuration: the length of the slip period, in milliseconds.
            //    o syncDurationTime: the 'natural duration', which may be specified when no Duration
            //      is specified by the Sync Timeline.

            // GetCurrentTimeCore is called twice for most ticks; skip the second one.
            if (ITimeBVT.tickNumber != _previousTickNumber)
            {
                if (this.Timeline.BeginTime == null ||
                   (this.Timeline.Duration == Duration.Automatic && !_secretDuration.HasTimeSpan))
                {
                    _syncCurrentTime = TimeSpan.Zero;
                    _syncTime        = TimeSpan.Zero;
                }
                else
                {
                    double testDuration;
                    if (_secretDuration.HasTimeSpan)
                    {
                        testDuration  = (double)_secretDuration.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        testDuration  = (double)this.Timeline.Duration.TimeSpan.TotalMilliseconds;
                    }

                    double testStartTime = (double)((TimeSpan)this.Timeline.BeginTime).TotalMilliseconds;
                    double slipStartTime = testStartTime + ITimeBVT.slipBegin;
                    double slipEndTime   = slipStartTime + ITimeBVT.slipDuration;

                    if ( (_syncCurrentTime.TotalMilliseconds > slipStartTime
                          && _syncCurrentTime.TotalMilliseconds < slipEndTime)
                          || _paused)
                    {
                        //In Slip period: do nothing.
                    }
                    else
                    {
                        if (_secretDuration.HasTimeSpan)
                        {
                            //Scenario:  simulating the duration of Media, by setting _secretDuration
                            //to syncDurationTime, specified by the test case.
                            if (_syncTime <= _secretDuration.TimeSpan)
                            {
                                _syncTime = _syncTime + TimeSpan.FromTicks((long)((testDuration) * 1000));
                            }
                            else
                            {
                                _syncTime = _secretDuration.TimeSpan;
                            }
                        }
                        else
                        {
                            _syncTime = _syncTime + TimeSpan.FromTicks((long)((testDuration) * 1000));
                        }
                    }

                    //Update the 'sync current time' each time.
                    _syncCurrentTime = _syncCurrentTime + TimeSpan.FromTicks((long)((testDuration) * 1000));

                    //Console.WriteLine("--------------_syncCurrentTime: " + _syncCurrentTime.TotalMilliseconds);
                    //Console.WriteLine("--------------_syncTime:        " + _syncTime.TotalMilliseconds);
                }
            }
            _previousTickNumber = ITimeBVT.tickNumber;
            return _syncTime;
        }


        public Duration SyncNaturalDuration
        {
            get
            {
                return _secretDuration;
            }
        }

        private Duration GetDuration()
        {
            Duration duration = Timeline.Duration;

            if (duration == Duration.Automatic)
            {
                duration = NaturalDuration;
                if (duration == Duration.Automatic)
                {
                    duration = Duration.Forever;
                }
            }

            return duration;
        }

        /// <summary>
        /// Called when we are stopped. This is the same as pausing and seeking
        /// to the beginning.
        /// </summary>
        protected override void Stopped()
        {
            // Only perform the operation if we're controlling a player
            Console.WriteLine("---Stopped---");
            if (this != null)
            {
                //bugs.child1.SpeedRatio = 1d;
                _syncTime = TimeSpan.FromTicks(0);
            }
        }

        /// <summary>
        /// Called when our speed changes. A discontinuous time movement may or
        /// may not have occurred.
        /// </summary>
        protected override void SpeedChanged()
        {
            Console.WriteLine("---SpeedChanged---");
            Sync();
        }

        /// <summary>
        /// Called when we have a discontinuous time movement, but no change in
        /// speed
        /// </summary>
        protected override void DiscontinuousTimeMovement()
        {
            Console.WriteLine("---DiscontinuousTimeMovement---");
            Sync();
        }

        private void Sync()
        {
            if (this != null)
            {
                if (this.CurrentTime == null)
                {
                    _syncTime = TimeSpan.FromTicks(0);
                }
                else
                {
                     _syncTime = this.CurrentTime.Value;
                }

                double? currentSpeed = this.CurrentGlobalSpeed;
                if (currentSpeed == null || currentSpeed == 0)
                {
                    _paused = true;
                }
                else
                {
                    _paused = false;
                }
            }
        }

        int _previousTickNumber      = 0;
        bool _paused                 = false;
        TimeSpan _syncTime          = TimeSpan.Zero;
        TimeSpan _syncCurrentTime   = TimeSpan.Zero;
        Duration _secretDuration;
    }
 }
