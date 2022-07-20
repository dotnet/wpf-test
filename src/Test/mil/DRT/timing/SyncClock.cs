// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// This file is for testing sync on a clock that moves erratically (progressing at 50% speed)


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
    public class SyncClock : Clock
    {
        /// <summary>
        /// Creates a SyncClock object.
        /// </summary>
        protected internal SyncClock(SyncTimeline timeline)
            : base(timeline)
        {
            _secretDuration = GetDuration();
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
            _syncTime = _syncTime + TimeSpan.FromTicks(5000);
            if (_secretDuration.HasTimeSpan && _syncTime > _secretDuration.TimeSpan)
            {
                _syncTime = _secretDuration.TimeSpan;
            }
            return _syncTime;
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

            return duration;  // More state to compute
        }


        TimeSpan _syncTime        = TimeSpan.Zero;
        Duration _secretDuration;
    }
}
