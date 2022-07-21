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
        void Pause()
        {
            // Use DoubleAnimations because it has NeedsTicksWhenActive == true.
            // This test sometimes assumes the clocks tick when active.
            ParallelTimeline rootTimeline;
            DoubleAnimation childTimeline;
            DoubleAnimation singleTimeline;

            // Begin immediately followed by pause
            Console.WriteLine("  Single timeline Begin=100, Duration=100, pause at t=100, resume at t=150");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            rootTimeline = new ParallelTimeline();
            rootTimeline.Name = "Timeline_0";
            rootTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            rootTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));

            CreateClocks(rootTimeline);

            Step(0, new State(rootTimeline, Inactive, TimelineTime(null)));
            Step(99, new State(rootTimeline, Inactive, TimelineTime(null)));
            Step(100, new State(rootTimeline, Active, TimelineTime(0)));
            Clock(rootTimeline).Controller.Pause();
            Step(100, new State(rootTimeline, Active, TimelineTime(0)));
            Step(150, new State(rootTimeline, Active, TimelineTime(0)));
            Clock(rootTimeline).Controller.Resume();
            Step(150, new State(rootTimeline, Active, TimelineTime(0)));
            Step(249, new State(rootTimeline, Active, TimelineTime(99)));
            Step(250, new State(rootTimeline, Inactive, TimelineTime(100)));


            // Begin immediately followed by pause after enable
            Console.WriteLine("  Single timeline Begin=Indefinite, Duration=100, Begin() and Pause before Enable at t=0, resume at t=150");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            rootTimeline = new ParallelTimeline();
            rootTimeline.Name = "Timeline_0";
            rootTimeline.BeginTime = null;
            rootTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));

            CreateClocks(rootTimeline);

            Clock(rootTimeline).Controller.Begin();
            Clock(rootTimeline).Controller.Pause();
            Step(0, new State(rootTimeline, Active, TimelineTime(0)));
            Step(150, new State(rootTimeline, Active, TimelineTime(0)));
            Clock(rootTimeline).Controller.Resume();
            Step(150, new State(rootTimeline, Active, TimelineTime(0)));
            Step(249, new State(rootTimeline, Active, TimelineTime(99)));
            Step(250, new State(rootTimeline, Inactive, TimelineTime(100)));


            // Individual timeline
            Console.WriteLine("  Single timeline Begin=100, Duration=100, pause at t=150, resume at t=275, pause at t=300, resume at t=325");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            singleTimeline = new DoubleAnimation();
            singleTimeline.Name = "Timeline_0";
            singleTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            singleTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));

            CreateClocks(singleTimeline);

            Step(0, new State(singleTimeline, Inactive, TimelineTime(null)));
            Step(99, new State(singleTimeline, Inactive, TimelineTime(null)));
            Step(100, new State(singleTimeline, Active, TimelineTime(0)));
            Step(150, new State(singleTimeline, Active, TimelineTime(50)));
            Clock(singleTimeline).Controller.Pause();
            Step(150, new State(singleTimeline, Active, TimelineTime(50)));
            Step(275, new State(singleTimeline, Active, TimelineTime(50)));
            Clock(singleTimeline).Controller.Resume();
            Step(275, new State(singleTimeline, Active, TimelineTime(50)));
            Step(300, new State(singleTimeline, Active, TimelineTime(75)));
            Clock(singleTimeline).Controller.Pause();
            Step(300, new State(singleTimeline, Active, TimelineTime(75)));
            Step(325, new State(singleTimeline, Active, TimelineTime(75)));
            Clock(singleTimeline).Controller.Resume();
            Step(325, new State(singleTimeline, Active, TimelineTime(75)));
            Step(349, new State(singleTimeline, Active, TimelineTime(99)));
            Step(350, new State(singleTimeline, Inactive, TimelineTime(100)));


            // Container and child, EndSync, pause the container
            Console.WriteLine("  Container Begin=10, Timeline Begin=10, Duration=50, pause container at t=30, resume at t=50");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            rootTimeline = new ParallelTimeline();
            rootTimeline.Name = "Time_container";
            rootTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 10);

            childTimeline = new DoubleAnimation();
            childTimeline.Name = "Timeline_0";
            childTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 10);
            childTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 50));
            rootTimeline.Children.Add(childTimeline);

            CreateClocks(rootTimeline);

            Step(0, new State(rootTimeline, Inactive, TimelineTime(null)),
                    new State(childTimeline, Inactive, TimelineTime(null)));
            Step(9, new State(rootTimeline, Inactive, TimelineTime(null)),
                    new State(childTimeline, Inactive, TimelineTime(null)));
            Step(10, new State(rootTimeline, Active, TimelineTime(0)),
                     new State(childTimeline, Inactive, TimelineTime(null)));
            Step(19, new State(rootTimeline, Active, TimelineTime(9)),
                     new State(childTimeline, Inactive, TimelineTime(null)));
            Step(20, new State(rootTimeline, Active, TimelineTime(10)),
                     new State(childTimeline, Active, TimelineTime(0)));
            Step(30, new State(rootTimeline, Active, TimelineTime(20)),
                     new State(childTimeline, Active, TimelineTime(10)));
            Clock(rootTimeline).Controller.Pause();
            Step(30, new State(rootTimeline, Active, TimelineTime(20)),
                     new State(childTimeline, Active, TimelineTime(10)));
            Step(50, new State(rootTimeline, Active, TimelineTime(20)),
                     new State(childTimeline, Active, TimelineTime(10)));
            Clock(rootTimeline).Controller.Resume();
            Step(50, new State(rootTimeline, Active, TimelineTime(20)),
                     new State(childTimeline, Active, TimelineTime(10)));
            Step(89, new State(rootTimeline, Active, TimelineTime(59)),
                     new State(childTimeline, Active, TimelineTime(49)));
            Step(90, new State(rootTimeline, Inactive, TimelineTime(60)),
                     new State(childTimeline, Inactive, TimelineTime(50)));


            // Seek while paused
            Console.WriteLine("  Timeline 1 Begin=100, Duration=100, pause at t=10, seek +80, resume");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            singleTimeline = new DoubleAnimation();
            singleTimeline.Name = "Timeline";
            singleTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            singleTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));

            CreateClocks(singleTimeline);

            // Step 0 ensures a tick at time 0.  Since BeginTime is an offset from 
            // the next tick this means that the timeline actually will start at 100,
            // which is the intent below.
            Step(0, new State(singleTimeline, Inactive, TimelineTime(null)));
            Step(99, new State(singleTimeline, Inactive, TimelineTime(null)));
            Step(100, new State(singleTimeline, Active, TimelineTime(0)));
            Step(110, new State(singleTimeline, Active, TimelineTime(10)));
            Clock(singleTimeline).Controller.Pause();
            Step(120, new State(singleTimeline, Active, TimelineTime(20)));
            Step(130, new State(singleTimeline, Active, TimelineTime(20)));
            Clock(singleTimeline).Controller.Seek(TimeSpan.FromMilliseconds(90), TimeSeekOrigin.BeginTime);
            Step(140, new State(singleTimeline, Active, TimelineTime(90)));
            Step(150, new State(singleTimeline, Active, TimelineTime(90)));
            Clock(singleTimeline).Controller.Resume();
            Step(160, new State(singleTimeline, Active, TimelineTime(90)));
            Step(169, new State(singleTimeline, Active, TimelineTime(99)));
            Step(170, new State(singleTimeline, Inactive, TimelineTime(100)));


            //
            // Calling Pause in a Fill state should pause
            //
            Console.WriteLine("  Timeline Begin=0, Duration=100, skip to fill at t=10, pause at t=20");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            singleTimeline = new DoubleAnimation();
            singleTimeline.Name = "Timeline";
            singleTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 0);
            singleTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));

            CreateClocks(singleTimeline);

            Step(0, new State(singleTimeline, Active, TimelineTime(0)));
            Step(10, new State(singleTimeline, Active, TimelineTime(10)));
            Clock(singleTimeline).Controller.SkipToFill();
            Step(20, new State(singleTimeline, Filling, NotPaused));
            Clock(singleTimeline).Controller.Pause();
            Step(30, new State(singleTimeline, Filling, Paused));
        
            //
            // Calling Pause when Stopped should pause
            //
            Console.WriteLine("  Timeline Begin=0, Duration=100, stopped at t = 50, paused at t=60");
            Clock(singleTimeline).Controller.Resume();
            Step(40, new State(singleTimeline, Filling, NotPaused));
            Clock(singleTimeline).Controller.Stop();
            Step(50, new State(singleTimeline, Stopped, NotPaused));
            Clock(singleTimeline).Controller.Pause();
            Step(50, new State(singleTimeline, Stopped, Paused));
            

            //
            // Seek, SkipToFill, Stop, and Begin should not unpause a Paused clock.
            //
            Console.WriteLine("  Timeline Begin=0, Duration=100, Seek, SkipToFill, Stop, and Begin a paused clock");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            singleTimeline = new DoubleAnimation();
            singleTimeline.Name = "Timeline";
            singleTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 0);
            singleTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));

            CreateClocks(singleTimeline);
            Step(0, new State(singleTimeline, Active, TimelineTime(0)));
            Step(10, new State(singleTimeline, Active, TimelineTime(10)));
            Clock(singleTimeline).Controller.Pause();
            Step(20, new State(singleTimeline, Active, Paused));
            Clock(singleTimeline).Controller.Seek(TimeSpan.FromMilliseconds(75), TimeSeekOrigin.BeginTime);
            Step(30, new State(singleTimeline, Active, Paused, TimelineTime(75)));
            Clock(singleTimeline).Controller.SkipToFill();
            Step(40, new State(singleTimeline, Filling, Paused, TimelineTime(100)));
            Clock(singleTimeline).Controller.Stop();
            Step(50, new State(singleTimeline, Stopped, Paused));
            Clock(singleTimeline).Controller.Begin();
            Step(60, new State(singleTimeline, Active, Paused, TimelineTime(0)));
        }
    }
}
