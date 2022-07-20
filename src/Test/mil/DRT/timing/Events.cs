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
        void Events()
        {
            ParallelTimeline timeline, childTimeline;
            Clock clock;
            ClockGroup clockGroup;
            SyncTimeline syncTimeline;

            Console.WriteLine("Running Eventing module...");

            ///////////////////////

            Console.WriteLine("  Regular timeline completing");

            CurrentTime = 0;
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.BeginTime = TimeSpan.FromMilliseconds(0);
            timeline.Duration = new Duration(TimeSpan.FromMilliseconds(4));
            timeline.Name = "Eventing_timeline_1";

            // For this test also make sure adding CurrentStateInvalidated events to a 
            // timeline propagates to the clock
            timeline.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            timeline.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
            timeline.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            timeline.Completed += new EventHandler(OnCompleted);

            clock = timeline.CreateClock();

            // tick at 1: a tick at 0 is a special case that we don't support
            for (int i = 1; i <= 12; i += 1)
            {
                Console.WriteLine("  -----------------------------------Tick: " + i + " ms");
                Console.WriteLine("  --Progress: " + clock.CurrentProgress + "  State: " + clock.CurrentState);

                CurrentTime = (int)i;
                manager.Tick();

                VerifyStateInvalidated(i == 1 || i == 5);  // We should fire StateInvalidated at t=1 and t=5
                VerifyTimeInvalidated(i <= 5);
                VerifyGlobalSpeedInvalidated(i == 1 || i == 5);
                VerifyCompleted(i == 5);

                ResetEvents();
            }

            clock.CurrentStateInvalidated -= new EventHandler(OnCurrentStateInvalidated);  // Clean up
            clock.CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
            clock.CurrentGlobalSpeedInvalidated -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
            clock.Completed -= new EventHandler(OnCompleted);

            /////////////////////////////

            Console.WriteLine("  CurrentStateInvalidated firing initially when RepeatBehavior is set to Forever");

            CurrentTime = 0;
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.BeginTime = TimeSpan.FromMilliseconds(0);
            timeline.Duration = new Duration(TimeSpan.FromMilliseconds(2));
            timeline.RepeatBehavior = RepeatBehavior.Forever;
            timeline.Name = "Eventing_timeline_3";

            clock = timeline.CreateClock();
            clock.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            clock.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
            clock.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            clock.Completed += new EventHandler(OnCompleted);

            // tick at 1: a tick at 0 is a special case that we don't support
            for (int i = 1; i <= 12; i += 1)
            {
                Console.WriteLine("  -----------------------------------Tick: " + i + " ms");
                Console.WriteLine("  --Progress: " + clock.CurrentProgress + "  State: " + clock.CurrentState);

                CurrentTime = (int)i;
                manager.Tick();

                VerifyStateInvalidated(i == 1);  // Verify that we fire StateInvalidated properly
                VerifyTimeInvalidated(true);
                VerifyGlobalSpeedInvalidated(i == 1);
                VerifyCompleted(false);

                ResetEvents();
            }

            clock.CurrentStateInvalidated -= new EventHandler(OnCurrentStateInvalidated);  // Clean up
            clock.CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
            clock.CurrentGlobalSpeedInvalidated -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
            clock.Completed -= new EventHandler(OnCompleted);

            ////////////////////////////

            Console.WriteLine("  State changes from Stopped to Filling, when no Duration is specified");

            CurrentTime = 0;
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.BeginTime = TimeSpan.FromMilliseconds(3);
            timeline.Name = "Eventing_timeline_4";

            clock = timeline.CreateClock();
            clock.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            clock.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
            clock.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            clock.Completed += new EventHandler(OnCompleted);

            for (int i = 0; i <= 11; i += 1)
            {
                Console.WriteLine("  -----------------------------------Tick: " + i + " ms");
                Console.WriteLine("  --Progress: " + clock.CurrentProgress + "  State: " + clock.CurrentState);

                CurrentTime = (int)i;
                manager.Tick();

                VerifyStateInvalidated(i == 3);  // Verify that we fire StateInvalidated properly
                VerifyTimeInvalidated(i == 3);
                VerifyGlobalSpeedInvalidated(i == 3);
                VerifyCompleted(i == 3);

                ResetEvents();
            }

            clock.CurrentStateInvalidated -= new EventHandler(OnCurrentStateInvalidated);  // Clean up
            clock.CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
            clock.CurrentGlobalSpeedInvalidated -= new EventHandler(OnCurrentGlobalSpeedInvalidated);
            clock.Completed -= new EventHandler(OnCompleted);

            ////////////////////////////

            Console.WriteLine("  Test Completed event with specified Duration and RepeatBehavior with child timeline");

            CurrentTime = 0;
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.BeginTime = TimeSpan.FromMilliseconds(3);
            timeline.Name = "Eventing_timeline_5";

            childTimeline = new ParallelTimeline();
            childTimeline.BeginTime = TimeSpan.FromMilliseconds(2);
            childTimeline.Duration =  TimeSpan.FromMilliseconds(8);
            childTimeline.RepeatBehavior = new RepeatBehavior(0.5);
            childTimeline.Name = "Eventing_timeline_5a";
            timeline.Children.Add(childTimeline);

            clock = timeline.CreateClock();
            clockGroup = (ClockGroup)clock;
            clockGroup.Children[0].Completed += new EventHandler(OnCompleted);

            for (int i = 0; i <= 11; i += 1)
            {
                Console.WriteLine("  -----------------------------------Tick: " + i + " ms");
                Console.WriteLine("  --Progress: " + clock.CurrentProgress + "  State: " + clock.CurrentState);

                CurrentTime = (int)i;
                manager.Tick();

                VerifyCompleted(i == (3 + 2 + 4));

                ResetEvents();
            }

            clockGroup.Children[0].Completed -= new EventHandler(OnCompleted);

            ////////////////////////////

            Console.WriteLine("  Test Remove operation and RemoveRequested event with child timeline");

            CurrentTime = 0;
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.BeginTime = TimeSpan.FromMilliseconds(3);
            timeline.Name = "Eventing_timeline_6";

            childTimeline = new ParallelTimeline();
            childTimeline.BeginTime = TimeSpan.FromMilliseconds(2);
            childTimeline.Duration = TimeSpan.FromMilliseconds(4);
            childTimeline.RepeatBehavior = new RepeatBehavior(2.0);
            childTimeline.Name = "Eventing_timeline_6a";
            timeline.Children.Add(childTimeline);

            clock = timeline.CreateClock();
            clockGroup = (ClockGroup)clock;
            clockGroup.Children[0].CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            clockGroup.Children[0].CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
            clockGroup.Children[0].CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);

            clockGroup.Children[0].Completed += new EventHandler(OnCompleted);
            clockGroup.Children[0].RemoveRequested += new EventHandler(OnRemoveRequested);

            for (int i = 0; i <= 15; i += 1)
            {
                Console.WriteLine("  -----------------------------------Tick: " + i + " ms");
                Console.WriteLine("  --Progress: " + clock.CurrentProgress + "  State: " + clock.CurrentState);

                if (i == 9)
                {
                    clock.Controller.Remove();
                }

                CurrentTime = (int)i;
                manager.Tick();

                VerifyStateInvalidated(i == 5 || i == 9);  // Verify that we fire StateInvalidated properly
                VerifyTimeInvalidated(5 <= i && i <= 9);
                VerifyGlobalSpeedInvalidated(i == 5 || i == 9);
                VerifyCompleted(i == 9);
                VerifyRemoveRequested(i == 9);

                ResetEvents();
            }

            clockGroup.Children[0].CurrentStateInvalidated -= new EventHandler(OnCurrentStateInvalidated);  // Clean up
            clockGroup.Children[0].CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
            clockGroup.Children[0].CurrentGlobalSpeedInvalidated -= new EventHandler(OnCurrentGlobalSpeedInvalidated);

            clockGroup.Children[0].Completed -= new EventHandler(OnCompleted);
            clockGroup.Children[0].RemoveRequested -= new EventHandler(OnRemoveRequested);

            ////////////////////////////

            Console.WriteLine("  Test sync timeline as an only child with SlipBehavior.Slip on the root");

            CurrentTime = 0;
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.SlipBehavior = SlipBehavior.Slip;
            timeline.BeginTime = TimeSpan.FromMilliseconds(2);
            timeline.Name = "Eventing_sync_timeline_7";

            syncTimeline = new SyncTimeline();
            syncTimeline.BeginTime = TimeSpan.FromMilliseconds(3);
            syncTimeline.Duration = TimeSpan.FromMilliseconds(5);
            syncTimeline.RepeatBehavior = new RepeatBehavior(2.5);
            //syncTimeline.RepeatBehavior = new RepeatBehavior(1.0);
            syncTimeline.Name = "Eventing_sync_timeline_7a";
            timeline.Children.Add(syncTimeline);

            clock = timeline.CreateClock();
            clock.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);

            clockGroup = (ClockGroup)clock;
            clockGroup.Children[0].CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            clockGroup.Children[0].CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
            clockGroup.Children[0].CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);

            clockGroup.Children[0].Completed += new EventHandler(OnCompleted);

            for (int i = 0; i <= 40; i += 1)
            {
                Console.WriteLine("  -----------------------------------Tick: " + i + " ms");
                Console.WriteLine("  --Progress: " + clock.CurrentProgress + "  State: " + clock.CurrentState);

                CurrentTime = (int)i;
                manager.Tick();

                /*VerifyStateInvalidated(i == 5 || i == 15);  // Verify that we fire StateInvalidated properly
                VerifyTimeInvalidated(5 <= i && i <= 15);
                VerifyGlobalSpeedInvalidated(i == 5 || i == 15);
                VerifyCompleted(i == 15);*/

                ResetEvents();
            }

            clockGroup.Children[0].CurrentStateInvalidated -= new EventHandler(OnCurrentStateInvalidated);  // Clean up
            clockGroup.Children[0].CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
            clockGroup.Children[0].CurrentGlobalSpeedInvalidated -= new EventHandler(OnCurrentGlobalSpeedInvalidated);

            clockGroup.Children[0].Completed -= new EventHandler(OnCompleted);
            clock.CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);

            ////////////////////////////

            Console.WriteLine("  Test sync timeline as an only child with SlipBehavior.Grow on the root");

            CurrentTime = 0;
            manager.Restart();

            timeline = new ParallelTimeline();
            timeline.SlipBehavior = SlipBehavior.Grow;
            timeline.BeginTime = TimeSpan.FromMilliseconds(2);
            timeline.Name = "Eventing_sync_timeline_8";

            syncTimeline = new SyncTimeline();
            syncTimeline.BeginTime = TimeSpan.FromMilliseconds(3);
            syncTimeline.Duration = TimeSpan.FromMilliseconds(5);
            syncTimeline.RepeatBehavior = new RepeatBehavior(2.5);
            syncTimeline.Name = "Eventing_sync_timeline_8a";
            timeline.Children.Add(syncTimeline);

            clock = timeline.CreateClock();
            clock.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);

            clockGroup = (ClockGroup)clock;
            clockGroup.Children[0].CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            clockGroup.Children[0].CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
            clockGroup.Children[0].CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);

            clockGroup.Children[0].Completed += new EventHandler(OnCompleted);

            for (int i = 0; i <= 40; i += 1)
            {
                Console.WriteLine("  -----------------------------------Tick: " + i + " ms");
                Console.WriteLine("  --Progress: " + clock.CurrentProgress + "  State: " + clock.CurrentState);

                CurrentTime = (int)i;
                manager.Tick();

                /*VerifyStateInvalidated(i == 5 || i == 15);  // Verify that we fire StateInvalidated properly
                VerifyTimeInvalidated(5 <= i && i <= 15);
                VerifyGlobalSpeedInvalidated(i == 5 || i == 14 || i == 15);
                VerifyCompleted(i == 14 || i == 15);  // */

                ResetEvents();
            }

            clockGroup.Children[0].CurrentStateInvalidated -= new EventHandler(OnCurrentStateInvalidated);  // Clean up
            clockGroup.Children[0].CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
            clockGroup.Children[0].CurrentGlobalSpeedInvalidated -= new EventHandler(OnCurrentGlobalSpeedInvalidated);

            clockGroup.Children[0].Completed -= new EventHandler(OnCompleted);
            clock.CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
        }
    }
}
