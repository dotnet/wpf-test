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
        void Container()
        {
            ParallelTimeline parentTimeline;
            ParallelTimeline timeline;

            Console.WriteLine("Testing active duration with one child");

            // Test a container with implicit duration determined wholly by a child
            Console.WriteLine("  Container Duration implicit, timeline Duration=100");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            parentTimeline = new ParallelTimeline();
            parentTimeline.Name = "ContainerSuite_container_1";
            parentTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);

            timeline = new ParallelTimeline();
            timeline.Name = "ContainerSuite_timeline_1";
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            parentTimeline.Children.Add(timeline);

            CreateClocks(parentTimeline);

            Step(0, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive));
            Step(99, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive));
            Step(100, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active));
            Step(199, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active));
            Step(200, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive));
            

            // Test a container with implicit duration and child with begin offset
            Console.WriteLine("  Container Duration implicit, timeline Begin=100 and Duration=100");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            parentTimeline = new ParallelTimeline();
            parentTimeline.Name = "ContainerSuite_container_2";
            parentTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);

            timeline = new ParallelTimeline();
            timeline.Name = "ContainerSuite_timeline_2";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            parentTimeline.Children.Add(timeline);

            CreateClocks(parentTimeline);

            Step(0, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive));
            Step(99, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive));
            Step(100, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Inactive));
            Step(199, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Inactive));
            Step(200, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active));
            Step(299, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active));
            Step(300, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive));
            

            // Test a container with implicit duration and child with auto-reverse and repeat
            Console.WriteLine("  Container Duration implicit, timeline Begin=100, Duration=100, AutoReverse=true, RepeatBehavior=450x");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            parentTimeline = new ParallelTimeline();
            parentTimeline.Name = "ContainerSuite_container_3";
            parentTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);

            timeline = new ParallelTimeline();
            timeline.Name = "ContainerSuite_timeline_3";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            timeline.AutoReverse = true;
            timeline.RepeatBehavior = new RepeatBehavior(new TimeSpan(0, 0, 0, 0, 450));
            parentTimeline.Children.Add(timeline);

            CreateClocks(parentTimeline);

            Step(0, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(99, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(100, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(199, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(null)));
            Step(200, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(299, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, NonReversed, CurrentIteration(1)));
            Step(300, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, Reversed, CurrentIteration(1)));
            Step(399, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, Reversed, CurrentIteration(1)));
            Step(400, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, NonReversed, CurrentIteration(2)));
            Step(499, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, NonReversed, CurrentIteration(2)));
            Step(500, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, Reversed, CurrentIteration(2)));
            Step(599, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, Reversed, CurrentIteration(2)));
            Step(600, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, NonReversed, CurrentIteration(3)));
            Step(649, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active, NonReversed, CurrentIteration(3)));
            Step(650, new State(parentTimeline, Enabled, Inactive), new State(timeline, Enabled, Inactive, NonReversed, CurrentIteration(3)));


            // Test a container with duration that clips the child
            Console.WriteLine("  Container Duration=150, timeline Begin=100, Duration=100");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            parentTimeline = new ParallelTimeline();
            parentTimeline.Name = "ContainerSuite_container_4";
            parentTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            parentTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 150));

            timeline = new ParallelTimeline();
            timeline.Name = "ContainerSuite_timeline_4";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            parentTimeline.Children.Add(timeline);

            CreateClocks(parentTimeline);

            Step(0, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive));
            Step(99, new State(parentTimeline, Enabled, Inactive), new State(timeline, Disabled, Inactive));
            Step(100, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Inactive));
            Step(199, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Inactive));
            Step(200, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active));
            Step(249, new State(parentTimeline, Enabled, Active), new State(timeline, Enabled, Active));
            Step(250, new State(parentTimeline, Enabled, Inactive), new State(timeline, Enabled, Active));            


            // Test a container that auto-reverses with a child inside
            Console.WriteLine("  Container Duration=Auto(100), Auto-Reverse=true, timeline Begin=10, Duration=90");
            CurrentTime = 0;

            manager.Restart();

            parentTimeline = new ParallelTimeline();
            parentTimeline.Name = "ContainerSuite_container_5";
            parentTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 0);
            parentTimeline.AutoReverse = true;

            timeline = new ParallelTimeline();
            timeline.Name = "ContainerSuite_timeline_5";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 10);
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 90));
            parentTimeline.Children.Add(timeline);

            CreateClocks(parentTimeline);

            Step(0, new State(parentTimeline, Enabled, Active, NonReversed), new State(timeline, Disabled, Inactive, NonReversed));
            Step(9, new State(parentTimeline, Enabled, Active, NonReversed), new State(timeline, Disabled, Inactive, NonReversed));
            Step(10, new State(parentTimeline, Enabled, Active, NonReversed), new State(timeline, Enabled, Active, NonReversed));
            Step(99, new State(parentTimeline, Enabled, Active, NonReversed), new State(timeline, Enabled, Active, NonReversed));
            Step(100, new State(parentTimeline, Enabled, Active, Reversed), new State(timeline, Enabled, Inactive, NonReversed));
            Step(101, new State(parentTimeline, Enabled, Active, Reversed), new State(timeline, Enabled, Active, Reversed));
            Step(189, new State(parentTimeline, Enabled, Active, Reversed), new State(timeline, Enabled, Active, Reversed));
            Step(190, new State(parentTimeline, Enabled, Active, Reversed), new State(timeline, Enabled, Active, Reversed));
            Step(191, new State(parentTimeline, Enabled, Active, Reversed), new State(timeline, Enabled, Inactive, Reversed));
            Step(200, new State(parentTimeline, Enabled, Inactive, Reversed), new State(timeline, Enabled, Inactive, Reversed));
        }
    }
}
