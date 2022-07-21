// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media.Animation;


namespace DRT
{
    public enum PropertyType
    {
        Enabled                 = 0x00000001,
        Active                  = 0x00000002,
        Reversed                = 0x00000004,
        CurrentIteration        = 0x00000008,
        TimelineTime            = 0x00000010,
        ForwardProgressing      = 0x00000020,
        CurrentState            = 0x00000040,
        Paused                  = 0x00000080
    }

    public class Property
    {
        public Property(PropertyType type)
        {
            this.type = type;
        }
        public PropertyType type;
    }

    public class Enabled : Property
    {
        public Enabled(bool enabled)
            : base(PropertyType.Enabled)
        {
            this.enabled = enabled;
        }
        public bool enabled;
    }

    public class Active : Property
    {
        public Active(bool active)
            : base(PropertyType.Active)
        {
            this.active = active;
        }
        public bool active;
    }

    public class Reversed : Property
    {
        public Reversed(bool reversed)
            : base(PropertyType.Reversed)
        {
            this.reversed = reversed;
        }
        public bool reversed;
    }

    public class CurrentIteration : Property
    {
        public CurrentIteration(Nullable<Int32> currentIteration)
            : base(PropertyType.CurrentIteration)
        {
            this.currentIteration = currentIteration;
        }
        public Nullable<Int32> currentIteration;
    }

    public class TimelineTime : Property
    {
        public TimelineTime(Nullable<TimeSpan> timelineTime)
            : base(PropertyType.TimelineTime)
        {
            this.timelineTime = timelineTime;
        }
        public Nullable<TimeSpan> timelineTime;
    }

    public class ForwardProgressing : Property
    {
        public ForwardProgressing(bool forwardProgressing)
            : base(PropertyType.ForwardProgressing)
        {
            this.forwardProgressing = forwardProgressing;
        }
        public bool forwardProgressing;
    }

    public class CurrentState : Property
    {
        public CurrentState(ClockState state)
            : base(PropertyType.CurrentState)
        {
            this.currentState = state;
        }
        public ClockState currentState;
    }

    public class Paused : Property
    {
        public Paused(bool paused)
            : base(PropertyType.Paused)
        {
            this.paused = paused;
        }
        public bool paused;
    }

    public class State
    {
        public State(Timeline timeline, params Property[] list)
        {
            this.timeline = timeline;
            foreach(Property property in list)
            {
                switch(property.type)
                {
                    case PropertyType.Enabled:
                        this.enabled = ((Enabled)property).enabled;
                        SetSpecified(PropertyType.Enabled);
                        break;
                    case PropertyType.Active:
                        this.active = ((Active)property).active;
                        SetSpecified(PropertyType.Active);
                        break;
                    case PropertyType.Reversed:
                        this.reversed = ((Reversed)property).reversed;
                        SetSpecified(PropertyType.Reversed);
                        break;
                    case PropertyType.CurrentIteration:
                        this.currentIteration = ((CurrentIteration)property).currentIteration;
                        SetSpecified(PropertyType.CurrentIteration);
                        break;
                    case PropertyType.TimelineTime:
                        this.timelineTime = ((TimelineTime)property).timelineTime;
                        SetSpecified(PropertyType.TimelineTime);
                        break;
                    case PropertyType.ForwardProgressing:
                        this.forwardProgressing = ((ForwardProgressing)property).forwardProgressing;
                        SetSpecified(PropertyType.ForwardProgressing);
                        break;
                    case PropertyType.CurrentState:
                        this.currentState = ((CurrentState)property).currentState;
                        SetSpecified(PropertyType.CurrentState);
                        break;
                    case PropertyType.Paused:
                        this.paused = ((Paused)property).paused;
                        SetSpecified(PropertyType.Paused);
                        break;
                }
            }
        }
        internal bool Verify(TimingSuite suite)
        {
            bool success = true;
            bool temp;
            Clock clock = suite.Clock(timeline);

            temp = !Specified(PropertyType.Active) || ((clock.CurrentState == ClockState.Active) == active);
            suite.DRT.Assert(temp,
                            "{0} is {1}active at time t={2}",
                            clock.Timeline.Name,
                            (active ? "in" : ""),
                            suite.CurrentTime);
            success &= temp;

            temp = !Specified(PropertyType.CurrentIteration) || (clock.CurrentIteration == currentIteration);
            suite.DRT.Assert(temp,
                            "{0} is on period {1} at time t={2}",
                            clock.Timeline.Name,
                            clock.CurrentIteration,
                            suite.CurrentTime);
            success &= temp;

            temp = !Specified(PropertyType.TimelineTime) || (clock.CurrentTime == timelineTime);
            suite.DRT.Assert(temp,
                            "{0}'s time is {1} at time t={2}",
                            clock.Timeline.Name,
                            clock.CurrentTime,
                            suite.CurrentTime);
            success &= temp;

            temp = !Specified(PropertyType.CurrentState) || (clock.CurrentState == currentState);
            suite.DRT.Assert(temp,
                            "{0}'s state is {1} at time t={2}",
                            clock.Timeline.Name,
                            clock.CurrentState,
                            suite.CurrentTime);
            success &= temp;

            temp = !Specified(PropertyType.Paused) || (clock.IsPaused == paused);
            suite.DRT.Assert(temp,
                            "{0} is {1}paused at time t={2}",
                            clock.Timeline.Name,
                            (clock.IsPaused ? "" : "not "),
                            suite.CurrentTime);
            success &= temp;

            return success;
        }
        private bool Specified(PropertyType type)
        {
            return (flags & (int)type) != 0;
        }
        private void SetSpecified(PropertyType type)
        {
            flags |= (int)type;
        }
        private Timeline timeline;
        private int flags;
        private bool enabled;
        private bool active;
        private bool reversed;
        private Nullable<Int32> currentIteration;
        private Nullable<TimeSpan> timelineTime;
        private bool forwardProgressing;
        private ClockState currentState;
        private bool paused;
    }

}
