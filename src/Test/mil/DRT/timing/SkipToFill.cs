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
        void SkipToFill()
        {
            ParallelTimeline rootTimeline;

            Console.WriteLine("Running SkipToFill test");

            //
            // SkipToFill on a clock that has never begun.
            //
            Console.WriteLine("  Single timeline, SkipToFill before the clock has begun");
            CurrentTime = 0;
            manager.Restart();

            rootTimeline = new ParallelTimeline();
            rootTimeline.Name = "Timeline_0";
            rootTimeline.BeginTime = null;
            rootTimeline.Duration = new Duration(TimeSpan.FromMilliseconds(10));

            CreateClocks(rootTimeline);

            // Tick the TimeManager at time = 2.  Since BeginTime is null
            // the call to CreateClock never started this timeline
            Step(2, new State(rootTimeline, new CurrentState(ClockState.Stopped)));

            // Queue up a SkipToFill
            Clock(rootTimeline).Controller.SkipToFill();

            // Tick the TimeManager at 3ms.  The timeline should
            // be filling.
            Step(3, new State(rootTimeline, new CurrentState(ClockState.Filling)));


            //
            // SkipToFill on a clock with a pending interactive begin 
            //
            Console.WriteLine("  Single timeline, SkipToFill following a Begin");
            CurrentTime = 0;
            manager.Restart();

            rootTimeline = new ParallelTimeline();
            rootTimeline.Name = "Timeline_0";
            rootTimeline.BeginTime = null;
            rootTimeline.Duration = new Duration(TimeSpan.FromMilliseconds(10));

            CreateClocks(rootTimeline);

            // Tick the TimeManager at time = 3.  Since BeginTime is null
            // the call to CreateClock never started this timeline
            Step(3, new State(rootTimeline, new CurrentState(ClockState.Stopped)));

            // Queue up a Begin and two SkipToFills
            Clock(rootTimeline).Controller.Begin();
            Clock(rootTimeline).Controller.SkipToFill();
            Clock(rootTimeline).Controller.SkipToFill();

            Step(5, new State(rootTimeline, new CurrentState(ClockState.Filling)));

            //
            // SkipToFill on clock that has begun
            //
            Console.WriteLine("  Single timeline, SkipToFill on a clock that has begun");
            CurrentTime = 0;
            manager.Restart();

            rootTimeline = new ParallelTimeline();
            rootTimeline.Name = "Timeline_0";
            rootTimeline.BeginTime = null;
            rootTimeline.Duration = new Duration(TimeSpan.FromMilliseconds(10));

            CreateClocks(rootTimeline);

            // Tick the TimeManager at time = 3.  Since BeginTime is null
            // the call to CreateClock never started this timeline
            Step(3, new State(rootTimeline, new CurrentState(ClockState.Stopped)));

            // Queue up a Begin
            Clock(rootTimeline).Controller.Begin();

            Step(5, new State(rootTimeline, new CurrentState(ClockState.Active)));

            // Queue up a SkipToFill
            Clock(rootTimeline).Controller.SkipToFill();

            Step(7, new State(rootTimeline, new CurrentState(ClockState.Filling)));

            //
            // SkipToFill on clock that is already filling
            //
            Console.WriteLine("  Single timeline, SkipToFill on a clock that is filling");

            // Queue up a SkipToFill
            Clock(rootTimeline).Controller.SkipToFill();

            Step(9, new State(rootTimeline, new CurrentState(ClockState.Filling)));
        }
    }
}
