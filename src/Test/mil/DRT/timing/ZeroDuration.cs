// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using System.Windows.Media.Animation;

namespace DRT
{
    internal partial class TimingSuite : DrtTestSuite
    {
        void ZeroDuration()
        {
            Console.WriteLine("Zero Duration Timeline test");

            ParallelTimeline timeline = new ParallelTimeline();
            timeline.Duration = TimeSpan.Zero;

            // RepeatBehavior: 1x (default)

            timeline.AutoReverse = false;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 1.0);
            timeline.AutoReverse = true;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 0.0);

            // RepeatBehavior: TimeSpan.Zero
            timeline.RepeatBehavior = new RepeatBehavior(TimeSpan.Zero);

            timeline.AutoReverse = false;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 1.0);
            timeline.AutoReverse = true;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 0.0);

            // RepeatBehavior: 2 seconds
            timeline.RepeatBehavior = new RepeatBehavior(TimeSpan.FromSeconds(2.0));

            timeline.AutoReverse = false;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 1.0);
            timeline.AutoReverse = true;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 0.0);

            // RepeatBehavior: Forever
            timeline.RepeatBehavior = RepeatBehavior.Forever;

            timeline.AutoReverse = false;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 1.0);
            timeline.AutoReverse = true;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 0.0);

            // RepeatBehavior: 0.3x
            timeline.RepeatBehavior = new RepeatBehavior(0.3);

            timeline.AutoReverse = false;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 0.3);
            timeline.AutoReverse = true;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 0.6);

            // RepeatBehavior: 0.6x
            timeline.RepeatBehavior = new RepeatBehavior(0.6);

            timeline.AutoReverse = false;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 0.6);
            timeline.AutoReverse = true;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 1, 0.8);

            // RepeatBehavior: 42.3x
            timeline.RepeatBehavior = new RepeatBehavior(42.3);

            timeline.AutoReverse = false;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 43, 0.3);
            timeline.AutoReverse = true;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 43, 0.6);

            // RepeatBehavior: 42.6x
            timeline.RepeatBehavior = new RepeatBehavior(42.6);

            timeline.AutoReverse = false;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 43, 0.6);
            timeline.AutoReverse = true;
            ZeroDuration_TestCurrentProgresAndIteration(timeline, 43, 0.8);
        }

        void ZeroDuration_TestCurrentProgresAndIteration(
            Timeline timeline,
            Int32 expectedIteration,
            Double expectedProgress)
        {
            ParallelTimeline parentTimeline = new ParallelTimeline();
            parentTimeline.Duration = TimeSpan.FromSeconds(1.0);
            parentTimeline.Children.Add(timeline);

            ClockGroup parent = parentTimeline.CreateClock();
            Clock child = parent.Children[0];

            parent.Controller.SeekAlignedToLastTick(TimeSpan.FromSeconds(0.5), TimeSeekOrigin.BeginTime);

            Console.WriteLine(
                    String.Format(
                        "  RepeatBehavior=\"{0}\" AutoReverse=\"{1}\"\n" +
                        "    CurrentIteration: '{2}' (Expected: '{3}')\n" +
                        "    CurrentProgress: '{4}' (Expected: '{5}')\n",
                        timeline.RepeatBehavior,
                        timeline.AutoReverse,
                        child.CurrentIteration,
                        expectedIteration,
                        child.CurrentProgress,
                        expectedProgress));

            if (child.CurrentIteration != expectedIteration)
            {
                throw new Exception(
                    String.Format(
                        "On a clock with Duration=\"0\", RepeatBehavior=\"{0}\", and AutoReverse=\"{1}\" the CurrentIteration value is incorrect. Expected: '{2}', Actual: '{3}'",
                        timeline.RepeatBehavior,
                        timeline.AutoReverse,
                        expectedIteration,
                        child.CurrentIteration));
            }

            if (!AreApproximatelyEqual(child.CurrentProgress.Value, expectedProgress))
            {
                throw new Exception(
                    String.Format(
                        "On a clock with Duration=\"0\", RepeatBehavior=\"{0}\", and AutoReverse=\"{1}\" the CurrentProgress value is incorrect. Expected: '{2}', Actual: '{3}'",
                        timeline.RepeatBehavior,
                        timeline.AutoReverse,
                        expectedProgress,
                        child.CurrentProgress));
            }
        }

        private bool AreApproximatelyEqual(Double value1, Double value2)
        {
            return value1 > (value2 - 0.0001)
                && value1 < (value2 + 0.0001);
        }
    }
}
