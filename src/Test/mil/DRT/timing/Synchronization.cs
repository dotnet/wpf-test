// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;

using System.Windows.Media.Animation;

namespace DRT
{
    internal partial class TimingSuite : DrtTestSuite
    {
        void Synchronization()
        {
            Timeline testTimeline, baselineTimeline;
            Clock testClock, baselineClock;

            Console.WriteLine("Running Synchronization module...");

            //////////////////////////
            
            Console.WriteLine("  Test generated controllable sync timelines");

            // Use a fixed random seed to produce the same series of random numbers
            // each time we run DrtTiming.  This makes the DRT completely predictable
            // and its results are 100% reproducible.
            Random rand = new Random(0);

            for (int pass = 0; pass < 35; pass++)
            {
                Console.WriteLine("---- Pass {0}:", pass);

                int randomSeed = rand.Next();
                SyncScenarioGenerator scenario1 = new SyncScenarioGenerator(randomSeed);

                if (pass == 12)
                {
                    Console.WriteLine("  Pass disabled due to bug {Clock.cs:3240}");
                    continue;
                }

                CurrentTime = 0;
                manager.Restart();

                testTimeline = scenario1.SyncTree;
                testClock = testTimeline.CreateClock();

                baselineTimeline = scenario1.BaselineTree;
                baselineClock = baselineTimeline.CreateClock();

                for (int i = 0; i < 15; i++)
                {
                    Console.WriteLine("  --CurrentTime: " + testClock.CurrentTime + "  State: " + testClock.CurrentState);
                    Console.WriteLine("  --CurrentTime: " + baselineClock.CurrentTime + "  State: " + baselineClock.CurrentState);

                    VerifyTreeEquivalence(testClock, baselineClock, pass, randomSeed);

                    Console.WriteLine("  -----------------------------------Tick: " + i + " ms");

                    CurrentTime = (int)i;
                    manager.Tick();

                    /*VerifyStateInvalidated(i == 5 || i == 15);  // Verify that we fire StateInvalidated properly
                    VerifyTimeInvalidated(5 <= i && i <= 15);
                    VerifyGlobalSpeedInvalidated(i == 5 || i == 14 || i == 15);
                    VerifyCompleted(i == 14 || i == 15);  // */

                    ResetEvents();
                }

            }

            //////////////
        }


        // Private helper for verifying that properties of two timing trees match
        private void VerifyTreeEquivalence(Clock testRoot, Clock baselineRoot, int pass, int randomSeed)
        {
            if (testRoot is ClockGroup)
            {
                Debug.Assert(baselineRoot is ClockGroup);  // Test and baseline tree structures must match

                ClockCollection testChildren = ((ClockGroup)testRoot).Children;
                if (testChildren != null)
                {
                    ClockCollection baselineChildren = ((ClockGroup)baselineRoot).Children;
                    Debug.Assert(baselineChildren != null);  // Must have similar tree structure

                    for (int n = 0; n < testChildren.Count; n++)
                    {
                        // Recursively verify equivalence
                        VerifyTreeEquivalence(testChildren[n], baselineChildren[n], pass, randomSeed);
                    }
                }
            }

            // The base case for recursion: compare CurrentTime and CurrentState
            DRT.Assert(testRoot.CurrentTime == baselineRoot.CurrentTime,
                "Mismatch between Sync and baseline Clock's CurrentTime, pass = {0}, random seed = {1}", pass, randomSeed);

            DRT.Assert(testRoot.CurrentState == baselineRoot.CurrentState,
                "Mismatch between Sync and baseline Clock's CurrentState, random seed = {0}", randomSeed);
        }
    }
}
