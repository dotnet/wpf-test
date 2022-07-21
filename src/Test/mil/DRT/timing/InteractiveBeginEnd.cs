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
        void InteractiveBeginEnd()
        {
            // A DoubleAnimation has NeedsTicksWhenActive = true
            // This test queries values during the active period so this is necessary.
            DoubleAnimation timeline;   

            // Run interactive begin/end tests
            Console.WriteLine("Testing interactive Begin/End events");

            // Test with Begin before enabling
            Console.WriteLine("  Begin before enabling");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new DoubleAnimation();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 50);
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 50));

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Step(10, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Clock(timeline).Controller.Begin();
            Step(10, new State(timeline, Enabled, Active, TimelineTime(0)));
            Step(59, new State(timeline, Enabled, Active, TimelineTime(49)));
            Step(60, new State(timeline, Enabled, Inactive, TimelineTime(50)));


            // Test with Begin after disabling
            Console.WriteLine("  Begin after disabling; Stop and Begin afterwards");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new DoubleAnimation();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 50);
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 50));

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Step(49, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Step(50, new State(timeline, Enabled, Active, TimelineTime(0)));
            Step(99, new State(timeline, Enabled, Active, TimelineTime(49)));
            Step(100, new State(timeline, Enabled, Inactive, TimelineTime(50)));
            Step(110, new State(timeline, Enabled, Inactive, TimelineTime(50)));
            Clock(timeline).Controller.Begin();
            Step(110, new State(timeline, Enabled, Active, TimelineTime(0)));
            Step(130, new State(timeline, Enabled, Active, TimelineTime(20)));
            Clock(timeline).Controller.Stop();
            Step(130, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Step(150, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Clock(timeline).Controller.Begin();
            Step(150, new State(timeline, Enabled, Active, TimelineTime(0)));
            Step(170, new State(timeline, Enabled, Active, TimelineTime(20)));

            // Test with Begin and no scheduled begin
            Console.WriteLine("  Interactive begin and change speed ratios");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new DoubleAnimation();
            timeline.Name = "Timeline";
            timeline.BeginTime = null;
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 50));

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Step(50, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Step(60, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Clock(timeline).Controller.Begin();
            Step(60, new State(timeline, Enabled, Active, TimelineTime(0)));
            Step(80, new State(timeline, Enabled, Active, TimelineTime(20)));
            Clock(timeline).Controller.SpeedRatio = 2.5;
            Step(80, new State(timeline, Enabled, Active, TimelineTime(20)));
            Step(90, new State(timeline, Enabled, Active, TimelineTime(45)));
            Clock(timeline).Controller.SpeedRatio = 0;
            Step(90, new State(timeline, Enabled, Active, TimelineTime(45)));
            Step(120, new State(timeline, Enabled, Active, TimelineTime(45)));
            Clock(timeline).Controller.SpeedRatio = 1.0;
            Step(120, new State(timeline, Enabled, Active, TimelineTime(45)));
            Step(124, new State(timeline, Enabled, Active, TimelineTime(49)));
            Step(125, new State(timeline, Enabled, Inactive, TimelineTime(50)));


            // Interactive begin interrupted
            Console.WriteLine("  Begin=50, At t=25 Begin");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new DoubleAnimation();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 50);
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 50));

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Step(25, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Clock(timeline).Controller.Begin();
            Step(25, new State(timeline, Enabled, Active, TimelineTime(0)));
            Step(49, new State(timeline, Enabled, Active, TimelineTime(24)));
            Step(74, new State(timeline, Enabled, Active, TimelineTime(49)));
            Step(100, new State(timeline, Enabled, Inactive, TimelineTime(50)));


            // Scheduled begin interrupted by interactive begin
            Console.WriteLine("  Begin=50, At t=75 Begin()");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            timeline = new DoubleAnimation();
            timeline.Name = "Timeline";
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0, 50);
            timeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 50));

            CreateClocks(timeline);

            Step(0, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Step(49, new State(timeline, Enabled, Inactive, TimelineTime(null)));
            Step(50, new State(timeline, Enabled, Active, TimelineTime(0)));
            Step(75, new State(timeline, Enabled, Active, TimelineTime(25)));
            Clock(timeline).Controller.Begin();
            Step(75, new State(timeline, Enabled, Active, TimelineTime(0)));
            Step(124, new State(timeline, Enabled, Active, TimelineTime(49)));
            Step(125, new State(timeline, Enabled, Inactive, TimelineTime(50)));
        }
    }
}
