// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using System.Windows.Media.Animation;

namespace DRT
{
    internal partial class TimingSuite : DrtTestSuite
    {
        #region DrtTestSuite

        public TimingSuite() : base("Main timing suite")
        {
            Contact = "Microsoft";
        }


        public override DrtTest[] PrepareTests()
        {
            // Replace the clock on the time manager
            manager = new InternalTimeManager();

            manager.Stop();
            CurrentTime = 0;
            manager.Start();

            // return the list of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(Container),
                new DrtTest(Events),
                new DrtTest(Synchronization),
                new DrtTest(InteractiveBeginEnd),
                new DrtTest(ParallelContainer),
                new DrtTest(Pause),
                new DrtTest(Seek),
                new DrtTest(SingleNode),
                new DrtTest(SkipToFill),
                new DrtTest(ZeroDuration),
                new DrtTest(NullTimeManager)
            };
        }
        #endregion // DrtTestSuite
  
        /// <summary>
        /// Gets and sets the current system time for use by the time manager.
        /// </summary>
        public long CurrentTime
        {
            get
            {
                return manager.CurrentTime;
            }

            set
            {
                managerCurrentTimeStatic = value;
                manager.CurrentTime = value;
            }
        }
        /// Called when the suite is completed.
        /// </summary>
        public override void ReleaseResources()
        {
            if (manager != null)
            {
                manager.Stop();
                manager = null;
            }
        }


        /// <summary>
        /// Creates ticking clocks for the specified timeline tree and
        /// stores the IDs in a dictionary so the clocks can be retrieved
        /// later for state operations.
        /// </summary>
        /// <param name="rootTimeline">
        /// The root timeline to create clocks for.
        /// </param>
        public void CreateClocks(Timeline rootTimeline)
        {
            Clock rootClock = rootTimeline.CreateClock();

            clockTable = new Dictionary<string, Clock>();
            AddClockIDs(rootClock);
        }


        /// <summary>
        /// Ticks at a given time and verifies the correctness of the state
        /// </summary>
        /// <param name="time">
        /// The time to tick at, in milliseconds.
        /// </param>
        public void Step(int time, params State[] list)
        {
            CurrentTime = time;
            manager.Tick();

            Step(list);
        }

        /// <summary>
        /// Verify the correct state immediately without ticking
        /// </summary>
        public void Step(params State[] list)
        {
            foreach (State state in list)
            {
                state.Verify(this);
            }
        }

        /// <summary>
        /// Adds the IDs of the given clock and all of its descendents
        /// to the clock table.
        /// </summary>
        /// <param name="clock">
        /// The root of the Clock subtree.
        /// </param>
        private void AddClockIDs(Clock clock)
        {
            clockTable[clock.Timeline.Name] = clock;
            ClockGroup clockGroup = clock as ClockGroup;

            if (clockGroup != null)
            {
                foreach (Clock child in clockGroup.Children)
                {
                    AddClockIDs(child);
                }
            }
        }
        /// <summary>
        /// Finds the clock that corresponds to the given timeline.
        /// </summary>
        /// <param name="timeline">
        /// The timeline to look for.
        /// </param>
        /// <returns>
        /// The clock corresponding to the timeline.
        /// </returns>
        /// <remarks>
        /// Identification is done via the ID property of the timeline
        /// and the clock table.
        /// </remarks>
        public Clock Clock(Timeline timeline)
        {
            return clockTable[timeline.Name];
        }


        /// <summary>
        /// Event handler for CurrentStateInvalidated
        /// </summary>
        /// <param name="subject">The firing clock</param>
        /// <param name="args">Arguments</param>
        public void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            Console.WriteLine("--CurrentStateInvalidated fired-----CurrentState: " + ((Clock)subject).CurrentState);
            stateInvalidated = true;
        }

        /// <summary>
        /// Verify that the last tick used the right values of StateInvalidated
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        public void VerifyStateInvalidated(bool expectedValue)
        {
            if (expectedValue)  // == true
            {
                DRT.Assert(stateInvalidated == true, "StateInvalidated failed to fire at time t = {0}.", CurrentTime);
            }
            else  // expectedValue == false
            {
                DRT.Assert(stateInvalidated == false, "StateInvalidated fired erroneously at t = {0}", CurrentTime);
            }
        }


        /// <summary>
        /// Event handler for CurrentTimeInvalidated
        /// </summary>
        /// <param name="subject">The firing clock</param>
        /// <param name="args">Arguments</param>
        public void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
            Console.WriteLine("--CurrentTimeInvalidated fired-----CurrentTime: " + ((Clock)subject).CurrentTime);
            timeInvalidated = true;
        }

        /// <summary>
        /// Verify that the last tick used the right values of TimeInvalidated
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        public void VerifyTimeInvalidated(bool expectedValue)
        {
            if (expectedValue)  // == true
            {
                DRT.Assert(timeInvalidated == true, "TimeInvalidated failed to fire at time t = {0}.", CurrentTime);
            }
            else  // expectedValue == false
            {
                DRT.Assert(timeInvalidated == false, "TimeInvalidated fired erroneously at t = {0}", CurrentTime);
            }
        }


        /// <summary>
        /// Event handler for CurrentGlobalSpeedInvalidated
        /// </summary>
        /// <param name="subject">The firing clock</param>
        /// <param name="args">Arguments</param>
        public void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
        {
            Console.WriteLine("--CurrentGlobalSpeedInvalidated fired-----CurrentGlobalSpeed: " + ((Clock)subject).CurrentGlobalSpeed);
            speedInvalidated = true;
        }

        /// <summary>
        /// Verify that the last tick used the right values of GlobalSpeedInvalidated
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        public void VerifyGlobalSpeedInvalidated(bool expectedValue)
        {
            if (expectedValue)  // == true
            {
                DRT.Assert(speedInvalidated == true, "GlobalSpeedInvalidated failed to fire at time t = {0}.", CurrentTime);
            }
            else  // expectedValue == false
            {
                DRT.Assert(speedInvalidated == false, "GlobalSpeedInvalidated fired erroneously at t = {0}", CurrentTime);
            }
        }


        /// <summary>
        /// Event handler for Completed
        /// </summary>
        /// <param name="subject">The firing clock</param>
        /// <param name="args">Arguments</param>
        public void OnCompleted(object subject, EventArgs args)
        {
            Console.WriteLine("--Completed fired-----");
            completedFired = true;
        }

        /// <summary>
        /// Verify that the last tick used the right values of Completed
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        public void VerifyCompleted(bool expectedValue)
        {
            if (expectedValue)  // == true
            {
                DRT.Assert(completedFired == true, "Completed failed to fire at time t = {0}.", CurrentTime);
            }
            else  // expectedValue == false
            {
                DRT.Assert(completedFired == false, "Completed fired erroneously at t = {0}", CurrentTime);
            }
        }


        /// <summary>
        /// Event handler for Completed
        /// </summary>
        /// <param name="subject">The firing clock</param>
        /// <param name="args">Arguments</param>
        public void OnRemoveRequested(object subject, EventArgs args)
        {
            Console.WriteLine("--RemoveRequested fired-----");
            removeRequested = true;
        }

        /// <summary>
        /// Verify that the last tick used the right values of RemoveRequested
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        public void VerifyRemoveRequested(bool expectedValue)
        {
            if (expectedValue)  // == true
            {
                DRT.Assert(removeRequested == true, "RemoveRequested failed to fire at time t = {0}.", CurrentTime);
            }
            else  // expectedValue == false
            {
                DRT.Assert(removeRequested == false, "RemoveRequested fired erroneously at t = {0}", CurrentTime);
            }
        }

        /// <summary>
        /// Reset event fired checkmarks for the previous tick(s)
        /// </summary>
        public void ResetEvents()
        {
            stateInvalidated = false;
            timeInvalidated = false;
            speedInvalidated = false;
            completedFired = false;
            removeRequested = false;
        }


        #region State management
        public Property Enabled
        {
            get
            {
                return new Enabled(true);
            }
        }
        public Property Disabled
        {
            get
            {
                return new Enabled(false);
            }
        }
        public Property Active
        {
            get
            {
                return new CurrentState(ClockState.Active);
            }
        }

        public Property Filling
        {
            get
            {
                return new CurrentState(ClockState.Filling);
            }
        }

        public Property Stopped
        {
            get
            {
                return new CurrentState(ClockState.Stopped);
            }
        }

        // todo: deprecate this in favor of stopped or filling
        public Property Inactive
        {
            get
            {
                return new Active(false);
            }
        }
        public Property Reversed
        {
            get
            {
                return new Reversed(true);
            }
        }
        public Property NonReversed
        {
            get
            {
                return new Reversed(false);
            }
        }



        public Property Paused
        {
            get
            {
                return new Paused(true);
            }
        }

        public Property NotPaused
        {
            get
            {
                return new Paused(false);
            }
        }

        public Property CurrentIteration(Nullable<Int32> iteration)
        {
            return new CurrentIteration(iteration);
        }
        public Property TimelineTime(Nullable<TimeSpan> time)
        {
            return new TimelineTime(time);
        }
        public Property TimelineTime(int timeMilliseconds)
        {
            return new TimelineTime(TimeSpan.FromMilliseconds(timeMilliseconds));
        }
        public Property ForwardProgressing
        {
            get
            {
                return new ForwardProgressing(true);
            }
        }
        public Property BackwardProgressing
        {
            get
            {
                return new ForwardProgressing(false);
            }
        }
        public static long ManagerCurrentTimeStatic
        {
            get
            {
                return managerCurrentTimeStatic;
            }
        }
        #endregion // State management


        #region Data
        /// <summary>
        /// This static public variable allows the ControlledSyncClock class to read back
        /// the assigned TimeManager time values.  This is used to check if the clock is/are
        /// being queried multiple times from the same TimeManager tick.
        /// </summary>
        private static long managerCurrentTimeStatic = -1;
        /// <summary>
        /// A time manager object, initialized and started
        /// The TimeManager class is internal to PresentationCore
        /// so we must use a wrapper class
        /// </summary>
        private InternalTimeManager manager;
        /// <summary>
        /// The list of clocks used for the current test.
        /// </summary>
        private Dictionary<string, Clock> clockTable;
        /// <summary>
        /// Whether stateInvalidated fired at the last tick.
        /// </summary>
        private bool stateInvalidated, timeInvalidated, speedInvalidated, completedFired, removeRequested;
        #endregion // Data
    }
}
