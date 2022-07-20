// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;

using System.Windows.Media.Animation;

namespace DRT
{
    internal partial class TimingSuite : DrtTestSuite
    {
        void SingleNode()
        {
            ParallelTimeline timeline;

            Console.WriteLine("Single time node test");

            // Test the effect of the Duration property
            Console.WriteLine("  Begin=100 and Duration=100");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.Name = "Timeline";

            // HOT 

            if (   !timeline.BeginTime.HasValue
                || timeline.BeginTime.Value != new TimeSpan(0))
            {
                throw new Exception("The default BeginTime value for a Timeline should be TimeSpan(0).");
            }

            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            timeline.Duration = new TimeSpan(0, 0, 0, 0, 100);

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive));
            Step(99, new State(timeline, Enabled, Inactive));
            Step(100, new State(timeline, Enabled, Active));
            Step(199, new State(timeline, Enabled, Active));
            Step(200, new State(timeline, Enabled, Inactive));


            // Test the default begin time
            Console.WriteLine("  Begin=Unspecified and Duration=100");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.Name = "Timeline";
            timeline.Duration = new TimeSpan(0, 0, 0, 0, 100);

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Active));
            Step(99, new State(timeline, Enabled, Active));
            Step(100, new State(timeline, Enabled, Inactive));


            // Test indefinite duration
            Console.WriteLine("  Begin=200, Duration=Indefinite");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 200);
            timeline.Duration = Duration.Forever;

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive));
            Step(199, new State(timeline, Enabled, Inactive));
            Step(200, new State(timeline, Enabled, Active));


            // Test the effect of the AutoReverse property
            Console.WriteLine("  Begin=100, Duration=100 and AutoReverse=true");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            timeline.Duration = new TimeSpan(0, 0, 0, 0, 100);
            timeline.AutoReverse = true;

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, NonReversed));
            Step(99, new State(timeline, Enabled, Inactive, NonReversed));
            Step(100, new State(timeline, Enabled, Active, NonReversed));
            Step(199, new State(timeline, Enabled, Active, NonReversed));
            Step(200, new State(timeline, Enabled, Active, Reversed));
            Step(299, new State(timeline, Enabled, Active, Reversed));
            Step(300, new State(timeline, Enabled, Inactive, NonReversed));


            // Test integer repeat count
            Console.WriteLine("  Begin=100, Duration=100 and RepeatCount=2x");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            timeline.Duration = new TimeSpan(0, 0, 0, 0, 100);
            timeline.RepeatBehavior = new RepeatBehavior(2.0);

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(99, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(100, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(199, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(200, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(2)));
            Step(299, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(2)));
            Step(300, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(2)));


            // Test fractional repeat count
            Console.WriteLine("  Begin=100, Duration=100 and RepeatCount=2.5x");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            timeline.Duration = new TimeSpan(0, 0, 0, 0, 100);
            timeline.RepeatBehavior = new RepeatBehavior(2.5);

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(99, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(100, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(199, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(299, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(2)));
            Step(300, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(3)));
            Step(349, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(3)));
            Step(350, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(3)));


            // Test repeat duration, integer periods
            Console.WriteLine("  Begin=100, Duration=100 and RepeatBehavior=0:0:0.200");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            timeline.Duration = new TimeSpan(0, 0, 0, 0, 100);
            timeline.RepeatBehavior = new RepeatBehavior(new TimeSpan(0, 0, 0, 0, 200));

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(99, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(100, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(199, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(200, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(2)));
            Step(299, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(2)));
            Step(300, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(2)));


            // Test repeat duration, fractional period
            Console.WriteLine("  Begin=100, Duration=100 and RepeatDuration=0:0:0.250");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            timeline.Duration = new TimeSpan(0, 0, 0, 0, 100);
            timeline.RepeatBehavior = new RepeatBehavior(new TimeSpan(0, 0, 0, 0, 250));
            timeline.FillBehavior = FillBehavior.Stop;

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(99, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(100, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(199, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(200, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(2)));
            Step(299, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(2)));
            Step(300, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(3)));
            Step(349, new State(timeline, Enabled, Active, NonReversed, CurrentIteration(3)));
            Step(350, new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));


            // Test NeedsTicksWhenActive
            // Test with Begin before enabling
            Console.WriteLine("  Timeline not requiring ticks when active");
            CurrentTime = 0;
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 50);
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 50));

            // Since this timeline isn't an Animation, NeedsTicksWhenActive is false.
            // This means that during its active period it typically won't get ticked, 
            // so querying its value at t=59 should not necessarily yield 49.
            //
            // The only subtlety here is that since 10 is on a boundary, the timeline will
            // be ticked on the next frame in order to be consistent with reversing timelines. 
            // This is why the tick at 11 has an effect but the ticks at 20 and 59 don't.

            CreateClocks(timeline);

            Step(10, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Clock(timeline).Controller.Begin();
            Step(10, new State(timeline, Enabled, Active, TimelineTime(0)));
            Step(11, new State(timeline, Enabled, Active, TimelineTime(1)));
            Step(20, new State(timeline, Enabled, Active, TimelineTime(1)));
            Step(59, new State(timeline, Enabled, Active, TimelineTime(1)));
            Step(60, new State(timeline, Enabled, Inactive, TimelineTime(50)));
        }
    }
}
