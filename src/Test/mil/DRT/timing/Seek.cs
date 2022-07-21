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
        void Seek()
        {
            ParallelTimeline rootTimeline;
            DoubleAnimation singleTimeline;
            DoubleAnimation childTimeline;
            DoubleAnimation childTimeline2;

            Console.WriteLine("Running seek test");

            // Individual timeline, backward fall-out seek
            Console.WriteLine("  Single timeline Begin=100, Duration=100, Seek=0 at t=150");
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

            // Queue up a seek
            Clock(singleTimeline).Controller.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);

            // Move the time manager immediately to 99ms
            Clock(singleTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(99), TimeSeekOrigin.BeginTime);
            Step(new State(singleTimeline, Active, TimelineTime(99)));

            // Move the time manager immediately to 100ms
            Clock(singleTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(100), TimeSeekOrigin.BeginTime);
            Step(new State(singleTimeline, Inactive, TimelineTime(100)));

            // Verify that the seek will actually happen
            Step(150, new State(singleTimeline, Active, TimelineTime(0)));
            Step(249, new State(singleTimeline, Active, TimelineTime(99)));
            Step(250, new State(singleTimeline, Inactive, TimelineTime(100)));


            // Individual timeline, backward seek
            Console.WriteLine("  Single timeline Begin=100, Duration=100, Seek=-25 at t=150");
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
            Clock(singleTimeline).Controller.Seek(TimeSpan.FromMilliseconds(25), TimeSeekOrigin.BeginTime);
            Step(150, new State(singleTimeline, Active, TimelineTime(25)));
            Step(224, new State(singleTimeline, Active, TimelineTime(99)));
            Step(225, new State(singleTimeline, Inactive, TimelineTime(100)));


            // Individual timeline, forward seek
            Console.WriteLine("  Single timeline Begin=100, Duration=100, Seek=+25 at t=150");
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
            Clock(singleTimeline).Controller.Seek(TimeSpan.FromMilliseconds(75), TimeSeekOrigin.BeginTime);
            Step(150, new State(singleTimeline, Active, TimelineTime(75)));
            // 

            Step(175, new State(singleTimeline, Inactive, TimelineTime(100)));


            // Individual timeline, forward fall-out seek
            Console.WriteLine("  Single timeline Begin=100, Duration=100, Seek=+100 at t=150");
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
            Clock(singleTimeline).Controller.Seek(TimeSpan.FromMilliseconds(100), TimeSeekOrigin.BeginTime);
            Step(150, new State(singleTimeline, Inactive, TimelineTime(100)));


            // Accumulated seek
            Console.WriteLine("  Single timeline Begin=100, Duration=100, Seek=+10 at t=150 and +20 at t=160");
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
            Clock(singleTimeline).Controller.Seek(TimeSpan.FromMilliseconds(60), TimeSeekOrigin.BeginTime);
            Step(150, new State(singleTimeline, Active, TimelineTime(60)));
            Step(160, new State(singleTimeline, Active, TimelineTime(70)));
            Clock(singleTimeline).Controller.Seek(TimeSpan.FromMilliseconds(80), TimeSeekOrigin.BeginTime);
            Step(160, new State(singleTimeline, Active, TimelineTime(80)));
           
            Step(179, new State(singleTimeline, Active, TimelineTime(99)));
            Step(180, new State(singleTimeline, Inactive, TimelineTime(100)));

            
            // Container and children
            Console.WriteLine("  Parallel container, Timeline 1 Begin=0, Duration=75, Timeline 2 Begin=25, Duration=75, Seek=-20 at t=60");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            rootTimeline = new ParallelTimeline();
            rootTimeline.Name = "Parallel_Container";
            rootTimeline.FillBehavior = FillBehavior.Stop;

            childTimeline = new DoubleAnimation();
            childTimeline.Name = "Timeline_1";
            childTimeline.BeginTime = new TimeSpan(0);
            childTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 75));
            childTimeline.FillBehavior = FillBehavior.Stop;
            rootTimeline.Children.Add(childTimeline);

            childTimeline2 = new DoubleAnimation();
            childTimeline2.Name = "Timeline_2";
            childTimeline2.BeginTime = new TimeSpan(0, 0, 0, 0, 25);
            childTimeline2.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 75));
            childTimeline2.FillBehavior = FillBehavior.Stop;
            rootTimeline.Children.Add(childTimeline2);

            CreateClocks(rootTimeline);

            Step(0, new State(rootTimeline, Active, TimelineTime(0)),
                       new State(childTimeline, Active, TimelineTime(0)),
                       new State(childTimeline2, Inactive, TimelineTime(null)));
            Step(24, new State(rootTimeline, Active, TimelineTime(24)),
                       new State(childTimeline, Active, TimelineTime(24)),
                       new State(childTimeline2, Inactive, TimelineTime(null)));
            Step(25, new State(rootTimeline, Active, TimelineTime(25)),
                       new State(childTimeline, Active, TimelineTime(25)),
                       new State(childTimeline2, Active, TimelineTime(0)));
            Step(60, new State(rootTimeline, Active, TimelineTime(60)),
                       new State(childTimeline, Active, TimelineTime(60)),
                       new State(childTimeline2, Active, TimelineTime(35)));

            // Move the timeline immediately to 25ms
            Clock(rootTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(25), TimeSeekOrigin.BeginTime);
            Step(new State(rootTimeline, Active, TimelineTime(25)),
                   new State(childTimeline, Active, TimelineTime(25)),
                   new State(childTimeline2, Active, TimelineTime(0)));

            // Move the timeline immediately to 75ms
            Clock(rootTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(75), TimeSeekOrigin.BeginTime);
            Step(new State(rootTimeline, Active, TimelineTime(75)),
                   new State(childTimeline, Inactive, TimelineTime(null)),
                   new State(childTimeline2, Active, TimelineTime(50)));

            // Move the timeline immediately to 0ms
            Clock(rootTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
            Step(new State(rootTimeline, Active, TimelineTime(0)),
                   new State(childTimeline, Active, TimelineTime(0)),
                   new State(childTimeline2, Inactive, TimelineTime(null)));

            // Move the timeline immediately to 5ms before the end of the timeline
            Clock(rootTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(-5), TimeSeekOrigin.Duration);
            Step(new State(rootTimeline, Active, TimelineTime(95)),
                   new State(childTimeline, Inactive, TimelineTime(null)),
                   new State(childTimeline2, Active, TimelineTime(70)));

            Clock(rootTimeline).Controller.Seek(TimeSpan.FromMilliseconds(40), TimeSeekOrigin.BeginTime);
            Step(60, new State(rootTimeline, Active, TimelineTime(40)),
                       new State(childTimeline, Active, TimelineTime(40)),
                       new State(childTimeline2, Active, TimelineTime(15)));
            Step(94, new State(rootTimeline, Active, TimelineTime(74)),
                       new State(childTimeline, Active, TimelineTime(74)),
                       new State(childTimeline2, Active, TimelineTime(49)));
            Step(95, new State(rootTimeline, Active, TimelineTime(75)),
                       new State(childTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline2, Active, TimelineTime(50)));
            Step(119, new State(rootTimeline, Active, TimelineTime(99)),
                       new State(childTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline2, Active, TimelineTime(74)));
            Step(120, new State(rootTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline2, Inactive, TimelineTime(null)));


            // Container and children, state-changing seek
            Console.WriteLine("  Parallel Container Begin=100, Timeline 1 Begin=0, Duration=75, Timeline 2 Begin=25, Duration=75, Seek=+30 at t=20");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            rootTimeline = new ParallelTimeline();
            rootTimeline.Name = "Parallel_Container";
            rootTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            rootTimeline.FillBehavior = FillBehavior.Stop;

            childTimeline = new DoubleAnimation();
            childTimeline.Name = "Timeline_1";
            childTimeline.BeginTime = new TimeSpan(0);
            childTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 75));
            childTimeline.FillBehavior = FillBehavior.Stop;
            rootTimeline.Children.Add(childTimeline);

            childTimeline2 = new DoubleAnimation();
            childTimeline2.Name = "Timeline_2";
            childTimeline2.BeginTime = new TimeSpan(0, 0, 0, 0, 25);
            childTimeline2.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 75));
            childTimeline2.FillBehavior = FillBehavior.Stop;
            rootTimeline.Children.Add(childTimeline2);

            CreateClocks(rootTimeline);

            // Step 0 ensures a tick at time 0.  Since BeginTime is an offset from 
            // the next tick this means that the timeline actually will start at 100,
            // which is the intent below.
            Step(0, new State(rootTimeline, Inactive, TimelineTime(null)));
            Step(99, new State(rootTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline2, Inactive, TimelineTime(null)));
            Step(100, new State(rootTimeline, Active, TimelineTime(0)),
                       new State(childTimeline, Active, TimelineTime(0)),
                       new State(childTimeline2, Inactive, TimelineTime(null)));
            Step(120, new State(rootTimeline, Active, TimelineTime(20)),
                       new State(childTimeline, Active, TimelineTime(20)),
                       new State(childTimeline2, Inactive, TimelineTime(null)));
            Clock(rootTimeline).Controller.Seek(TimeSpan.FromMilliseconds(50), TimeSeekOrigin.BeginTime);
            Step(120, new State(rootTimeline, Active, TimelineTime(50)),
                       new State(childTimeline, Active, TimelineTime(50)),
                       new State(childTimeline2, Active, TimelineTime(25)));
            Step(144, new State(rootTimeline, Active, TimelineTime(74)),
                       new State(childTimeline, Active, TimelineTime(74)),
                       new State(childTimeline2, Active, TimelineTime(49)));
            Step(145, new State(rootTimeline, Active, TimelineTime(75)),
                       new State(childTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline2, Active, TimelineTime(50)));
            Step(169, new State(rootTimeline, Active, TimelineTime(99)),
                       new State(childTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline2, Active, TimelineTime(74)));
            Step(170, new State(rootTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline, Inactive, TimelineTime(null)),
                       new State(childTimeline2, Inactive, TimelineTime(null)));


            // Container and repeating children
            Console.WriteLine("  Parallel Container Begin=100, Timeline1 Begin=0,Duration=75, RepeatCount=5, Timeline2 Begin=25, Duration=25, RepeatCount=15");
            CurrentTime = 0;

            manager.Restart();

            rootTimeline = new ParallelTimeline();
            rootTimeline.Name = "Parallel_Container";
            rootTimeline.BeginTime = new TimeSpan(0, 0, 0, 0, 100);
            rootTimeline.FillBehavior = FillBehavior.Stop;

            childTimeline = new DoubleAnimation();
            childTimeline.Name = "Timeline_1";
            childTimeline.BeginTime = new TimeSpan(0);
            childTimeline.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 75));
            childTimeline.FillBehavior = FillBehavior.Stop;
            childTimeline.RepeatBehavior = new RepeatBehavior(5);
            rootTimeline.Children.Add(childTimeline);

            childTimeline2 = new DoubleAnimation();
            childTimeline2.Name = "Timeline_2";
            childTimeline2.BeginTime = new TimeSpan(0, 0, 0, 0, 25);
            childTimeline2.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 25));
            childTimeline2.RepeatBehavior = new RepeatBehavior(14);
            childTimeline2.FillBehavior = FillBehavior.Stop;
            rootTimeline.Children.Add(childTimeline2);

            CreateClocks(rootTimeline);

            // Move the timeline immediately to 25ms
            Clock(rootTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(25), TimeSeekOrigin.BeginTime);
            Step(new State(rootTimeline, Active, TimelineTime(25)),
                   new State(childTimeline, Active, TimelineTime(25), CurrentIteration(1)),
                   new State(childTimeline2, Active, TimelineTime(0), CurrentIteration(1)));

            // Move the timeline immediately to 75ms
            Clock(rootTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(75), TimeSeekOrigin.BeginTime);
            Step(new State(rootTimeline, Active, TimelineTime(75)),
                   new State(childTimeline, Active, TimelineTime(0), CurrentIteration(2)),
                   new State(childTimeline2, Active, TimelineTime(0), CurrentIteration(3)));

            // Move the timeline immediately to 0ms
            Clock(rootTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
            Step(new State(rootTimeline, Active, TimelineTime(0)),
                   new State(childTimeline, Active, TimelineTime(0)),
                   new State(childTimeline2, Inactive, TimelineTime(null)));

            // Move the timeline immediately to 5ms before the end of the timeline (370 ms)
            Clock(rootTimeline).Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(-5), TimeSeekOrigin.Duration);
            Step(new State(rootTimeline, Active, TimelineTime(370)),
                   new State(childTimeline, Active, TimelineTime(70), CurrentIteration(5)),
                   new State(childTimeline2, Active, TimelineTime(20), CurrentIteration(14)));
        }
    }
}
