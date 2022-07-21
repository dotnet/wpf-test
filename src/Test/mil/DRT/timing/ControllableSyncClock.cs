// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// This file is for testing sync on a clock that moves erratically.
// This particular ControllableSyncClock's behavior is controlled by its public interface.
// The file contains ControllableSyncClock class.
// AnimationTimeline and AnimationClock are used as base classes to insure that NeedsTicksWhenActive==true.


#if DEBUG
#define TRACE
#endif // DEBUG


using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DRT
{
    /// <summary>
    /// Test class for sync
    /// </summary>
    public class ControllableSyncClock : AnimationClock
    {
        /// <summary>
        /// Creates a ControllableSyncClock object.
        /// </summary>
        protected internal ControllableSyncClock(ControllableSyncTimeline timeline)
            : base(timeline)
        {
            _trueDuration = GetDuration();
            _advanceRate = timeline.SpeedRatio;
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
            if (!_currentTimeQueried)  // We start on the first tick we are queried
            {
                _currentTimeQueried = true;
                _syncTime = TimeSpan.Zero;

                Timeline ourTimeline = this.Timeline;
                if ((!ourTimeline.Duration.HasTimeSpan && !ourTimeline.RepeatBehavior.HasDuration)
                    || ourTimeline.RepeatBehavior == RepeatBehavior.Forever)
                {
                    AdvanceSyncTime();  // Temporary workaround for simulating "perfect" timeline behavior
                }
                _previousTimeManagerTime = TimingSuite.ManagerCurrentTimeStatic;
            }
            else if (TimingSuite.ManagerCurrentTimeStatic != _previousTimeManagerTime)
            {
                AdvanceSyncTime();
                _previousTimeManagerTime = TimingSuite.ManagerCurrentTimeStatic;
            }

            return _syncTime;
        }


        private void AdvanceSyncTime()
        {
            _syncTime = _syncTime + TimeSpan.FromMilliseconds(_advanceRate);
            if (_trueDuration.HasTimeSpan && _syncTime > _trueDuration.TimeSpan)
            {
                _syncTime = _trueDuration.TimeSpan;
            }
        }


        /// <summary>
        /// Internal method for starting the media
        /// </summary>
        protected override void DiscontinuousTimeMovement()
        {
            _syncTime = TimeSpan.Zero;
        }

        public Duration SyncNaturalDuration
        {
            get
            {
                return _trueDuration;
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

            return duration;  // More state to compute
        }


        public double AdvanceRate
        {
            set { _advanceRate = value; }
            get { return _advanceRate;  }
        }


        TimeSpan _syncTime = TimeSpan.Zero;
        Duration _trueDuration;

        double _advanceRate = 1;

        bool _currentTimeQueried = false;
        long _previousTimeManagerTime = -1;
    }
}
