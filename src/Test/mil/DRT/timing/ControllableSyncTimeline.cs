// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// This file is for testing sync on a clock that moves erratically.
// The file contains ControllableSyncTimeline class.
// AnimationTimeline and AnimationClock are used as base classes to insure that NeedsTicksWhenActive==true.


#if DEBUG
#define TRACE
#endif // DEBUG


using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DRT
{
    #region ControllableSyncTimeline



    /// <summary>
    /// ControllableSyncTimeline test class.
    /// </summary>
    public class ControllableSyncTimeline : AnimationTimeline
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
            ControllableSyncClock ControllableSyncClock = new ControllableSyncClock(this);

            return ControllableSyncClock;
        }

        /// <summary>
        /// Returns the type of the target property;
        /// This method was implemented to avoid compile errors.
        /// </summary>
        public override sealed Type TargetPropertyType
        {
            get
            {
                ReadPreamble();

                return typeof(TimeSpan);
            }
        }

        /// <summary>
        /// Creates a new ControllableSyncClock using this ControllableSyncTimeline.
        /// </summary>
        /// <returns>A new ControllableSyncClock.</returns>
        public new ControllableSyncClock CreateClock()
        {
            return (ControllableSyncClock)base.CreateClock();
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
            ControllableSyncClock sc = (ControllableSyncClock)clock;
            return sc.SyncNaturalDuration;
        }

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
        /// </summary>
        /// <returns>The new Freezable.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new ControllableSyncTimeline();
        }

        #endregion
    }

    #endregion
}
