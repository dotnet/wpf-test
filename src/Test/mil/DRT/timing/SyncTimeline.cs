// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// This file is for testing sync on a clock that moves erratically (progressing at 50% speed)

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DRT
{
    #region SyncTimeline


    /// <summary>
    /// SyncTimeline test class.
    /// </summary>
    public class SyncTimeline : Timeline
    {
        #region Timeline

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

        #endregion
    }

    #endregion
};
