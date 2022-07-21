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
        void ParallelContainer()
        {
            ParallelTimeline parentTimeline;
            ParallelTimeline[] timeline = new ParallelTimeline[2];

            // Run the parallel container tests
            Console.WriteLine("Parallel container tests");

            // Test with EndSync=LastChild
            Console.WriteLine("  Container EndSync=LastChild, Timeline 0 Duration=100, Timeline 1 Begin=50, Duration=100");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            parentTimeline = new ParallelTimeline();
            parentTimeline.Name = "Parallel_time_container";

            timeline[0] = new ParallelTimeline();
            timeline[0].Name = "Timeline_1_0";
            timeline[0].Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            parentTimeline.Children.Add(timeline[0]);

            timeline[1] = new ParallelTimeline();
            timeline[1].Name = "Timeline_1_1";
            timeline[1].BeginTime = new TimeSpan(0, 0, 0, 0, 50);
            timeline[1].Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            parentTimeline.Children.Add(timeline[1]);

            CreateClocks(parentTimeline);

            Step(0, new State(parentTimeline, Enabled, Active),
                    new State(timeline[0], Enabled, Active),
                    new State(timeline[1], Enabled, Inactive));
            Step(49, new State(parentTimeline, Enabled, Active),
                     new State(timeline[0], Enabled, Active),
                     new State(timeline[1], Enabled, Inactive));
            Step(50, new State(parentTimeline, Enabled, Active),
                     new State(timeline[0], Enabled, Active),
                     new State(timeline[1], Enabled, Active));
            Step(99, new State(parentTimeline, Enabled, Active),
                     new State(timeline[0], Enabled, Active),
                     new State(timeline[1], Enabled, Active));
            Step(100, new State(parentTimeline, Enabled, Active),
                      new State(timeline[0], Enabled, Inactive),
                      new State(timeline[1], Enabled, Active));
            Step(149, new State(parentTimeline, Enabled, Active),
                      new State(timeline[0], Enabled, Inactive),
                      new State(timeline[1], Enabled, Active));
            Step(150, new State(parentTimeline, Enabled, Inactive),
                      new State(timeline[0], Disabled, Inactive),
                      new State(timeline[1], Disabled, Inactive));
            

            // Test with EndSync=AllChildren
            Console.WriteLine("  Container EndSync=AllChildren, Timeline 0 Duration=100, Timeline 1 Begin=50, Duration=100");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            parentTimeline = new ParallelTimeline();
            parentTimeline.Name = "Parallel_time_container";

            timeline[0] = new ParallelTimeline();
            timeline[0].Name = "Timeline_2_0";
            timeline[0].Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            parentTimeline.Children.Add(timeline[0]);

            timeline[1] = new ParallelTimeline();
            timeline[1].Name = "Timeline_2_1";
            timeline[1].BeginTime = new TimeSpan(0, 0, 0, 0, 50);
            timeline[1].Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            parentTimeline.Children.Add(timeline[1]);

            CreateClocks(parentTimeline);

            Step(0, new State(parentTimeline, Enabled, Active),
                    new State(timeline[0], Enabled, Active),
                    new State(timeline[1], Enabled, Inactive));
            Step(49, new State(parentTimeline, Enabled, Active),
                     new State(timeline[0], Enabled, Active),
                     new State(timeline[1], Enabled, Inactive));
            Step(50, new State(parentTimeline, Enabled, Active),
                     new State(timeline[0], Enabled, Active),
                     new State(timeline[1], Enabled, Active));
            Step(99, new State(parentTimeline, Enabled, Active),
                     new State(timeline[0], Enabled, Active),
                     new State(timeline[1], Enabled, Active));
            Step(100, new State(parentTimeline, Enabled, Active),
                      new State(timeline[0], Enabled, Inactive),
                      new State(timeline[1], Enabled, Active));
            Step(149, new State(parentTimeline, Enabled, Active),
                      new State(timeline[0], Enabled, Inactive),
                      new State(timeline[1], Enabled, Active));
            Step(150, new State(parentTimeline, Enabled, Inactive),
                      new State(timeline[0], Disabled, Inactive),
                      new State(timeline[1], Disabled, Inactive));
            

            // Test repeating/reversing parent
            Console.WriteLine("  Container EndSync=AllChildren, AutoReverse=true, RepeatBehavior=450x, Timeline 0 Duration=100, Timeline 1 Begin=50, Duration=100");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            parentTimeline = new ParallelTimeline();
            parentTimeline.Name = "Parallel_time_container";
            parentTimeline.AutoReverse = true;
            parentTimeline.RepeatBehavior = new RepeatBehavior(new TimeSpan(0, 0, 0, 0, 450));
            parentTimeline.FillBehavior = FillBehavior.Stop;

            timeline[0] = new ParallelTimeline();
            timeline[0].Name = "Timeline_3_0";
            timeline[0].Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            timeline[0].FillBehavior = FillBehavior.Stop;
            parentTimeline.Children.Add(timeline[0]);

            timeline[1] = new ParallelTimeline();
            timeline[1].Name = "Timeline_3_1";
            timeline[1].BeginTime = new TimeSpan(0, 0, 0, 0, 50);
            timeline[1].Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            timeline[1].FillBehavior = FillBehavior.Stop;
            parentTimeline.Children.Add(timeline[1]);

            CreateClocks(parentTimeline);

            Step(0, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                    new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                    new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(49, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                     new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                     new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(50, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                     new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                     new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(99, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                     new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                     new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(100, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                      new State(timeline[0], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                      new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(149, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                      new State(timeline[0], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                      new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(150, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)));
            Step(199, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)),
                       new State(timeline[1], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)));
            Step(200, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)),
                       new State(timeline[1], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)));
            Step(249, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)));
            Step(250, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)));
            Step(299, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)));
            Step(300, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(2)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(349, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(2)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(350, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(2)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(399, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(2)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(400, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(2)),
                       new State(timeline[0], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                       new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(449, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(2)),
                       new State(timeline[0], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                       new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(450, new State(parentTimeline, Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                       new State(timeline[0], Disabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                       new State(timeline[1], Disabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            

            // Test repeating/reversing parent and child
            Console.WriteLine("  Container EndSync=LastChild, AutoReverse=true, Timeline 0 Duration=100, AutoReverse=true, RepeatBehavior=210x, Timeline 1 Begin=50, Duration=100");
            CurrentTime = 0;
            //manager.RootClock.Children.Clear();
            manager.Restart();

            parentTimeline = new ParallelTimeline();
            parentTimeline.Name = "Parallel_time_container";
            parentTimeline.AutoReverse = true;
            parentTimeline.FillBehavior = FillBehavior.Stop;

            timeline[0] = new ParallelTimeline();
            timeline[0].Name = "Timeline_4_0";
            timeline[0].Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            timeline[0].AutoReverse = true;
            timeline[0].RepeatBehavior = new RepeatBehavior(new TimeSpan(0, 0, 0, 0, 210));
            timeline[0].FillBehavior = FillBehavior.Stop;
            parentTimeline.Children.Add(timeline[0]);

            timeline[1] = new ParallelTimeline();
            timeline[1].Name = "Timeline_4_1";
            timeline[1].BeginTime = new TimeSpan(0, 0, 0, 0, 50);
            timeline[1].Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
            timeline[1].FillBehavior = FillBehavior.Stop;
            parentTimeline.Children.Add(timeline[1]);

            CreateClocks(parentTimeline);

            Step(0, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(49, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(50, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(99, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(100, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(149, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)));
            Step(150, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(199, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(200, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(2)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(209, new State(parentTimeline, Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, ForwardProgressing, CurrentIteration(2)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(210, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)));
            Step(219, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(2)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)));
            Step(220, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, Reversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)));
            Step(269, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, Reversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)));
            Step(270, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, Reversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)));
            Step(271, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, Reversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)));
            Step(319, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, Reversed, ForwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)));
            Step(320, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)));
            Step(369, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)));
            Step(370, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)));
            Step(371, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)));
            Step(419, new State(parentTimeline, Enabled, Active, Reversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[0], Enabled, Active, NonReversed, BackwardProgressing, CurrentIteration(1)),
                       new State(timeline[1], Enabled, Inactive, NonReversed, BackwardProgressing, CurrentIteration(null)));
            Step(420, new State(parentTimeline, Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                       new State(timeline[0], Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                       new State(timeline[1], Disabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
            Step(421, new State(parentTimeline, Enabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                       new State(timeline[0], Disabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)),
                       new State(timeline[1], Disabled, Inactive, NonReversed, ForwardProgressing, CurrentIteration(null)));
        }
    }
}
