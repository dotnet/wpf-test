// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// This file is for testing sync on a clock that moves erratically.
// This is a class with methods for generating a Timing tree structure for testing.


#if DEBUG
#define TRACE
#endif // DEBUG


using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace DRT
{
    #region SyncScenarioGenerator



    /// <summary>
    /// ControllableSyncTimeline test class.
    /// </summary>
    public class SyncScenarioGenerator
    {
        public SyncScenarioGenerator(int initial_seed)
        {
            _random = new Random(initial_seed);
            GenerateTimelineTree();
        }

        public Timeline SyncTree
        {
            get
            {
                return _syncTree;
            }
        }

        public Timeline BaselineTree
        {
            get
            {
                return _baselineTree;
            }
        }


        private void GenerateTimelineTree()
        {
            GenerateTimelineTreeRecursive(out _syncTree, out _baselineTree, 0);
        }

        private void GenerateTimelineTreeRecursive(out Timeline syncRoot, out Timeline baselineRoot, int depth)
        {
            // Probability of getting a ParallelTimeline at certain depth level:
            // 0: 48%    1: 40%    2: 32%    3: 24%    4: 16%    5: 8%
            double probabilityOfChildren = (depth > 5) ? 0 : ((double)(6 - depth)) * .08;
            double probabilityOfAlternativeNode = (1 - probabilityOfChildren) / 2;

            int randNodeType = RandomDistribution(probabilityOfChildren, probabilityOfAlternativeNode,
                                                                         probabilityOfAlternativeNode);
            
            if (randNodeType == 0)
            {
                syncRoot = new ParallelTimeline();
                baselineRoot = new ParallelTimeline();

                int numChildren = RandomDistribution(.45, .30, .20, .05);  // From 0 to 3 children
                for (int c = 0; c < numChildren; c++)
                {
                    Timeline syncChild, baselineChild;
                    GenerateTimelineTreeRecursive(out syncChild, out baselineChild, depth + 1);

                    ((ParallelTimeline)syncRoot).Children.Add(syncChild);
                    ((ParallelTimeline)baselineRoot).Children.Add(baselineChild);
                }
            }
            else if (randNodeType == 1)
            {
                syncRoot = new DoubleAnimation();
                baselineRoot = new DoubleAnimation();
            }
            else  // (randNodeType == 2)
            {
                syncRoot = new ControllableSyncTimeline();
                baselineRoot = new DoubleAnimation();  // This is where sync vs. baseline is different
            }


            // Initialize parameters for this timeline:
            TimeSpan? beginTime = RandomBeginTime();
            syncRoot.BeginTime = beginTime;
            baselineRoot.BeginTime = beginTime;

            Duration duration = RandomDuration();
            syncRoot.Duration = duration;
            baselineRoot.Duration = duration;

            RepeatBehavior repeatBehavior = RandomRepeatBehavior();
            syncRoot.RepeatBehavior = repeatBehavior;
            baselineRoot.RepeatBehavior = repeatBehavior;
        }


        private TimeSpan? RandomBeginTime()
        {
            //                                    <--0   1    2    3  null-->
            int beginTimeHint = RandomDistribution(.40, .15, .15, .15, .15);

            if (beginTimeHint < 4)  // Use 4==Automatic, don't set any value
            {
                // The range is [0, 3]
                return TimeSpan.FromMilliseconds((double)beginTimeHint);
            }
            else  // beginTimeHint == 4, use automatic begin time
            {
                return null;
            }
        }

        private Duration RandomDuration()
        {
            //                                  <--0    1    2    3    4    5  Forev Auto-->
            int durationHint = RandomDistribution(.10, .05, .05, .10, .10, .10, .10, .40);

            if (durationHint == 7)
            {
                return Duration.Automatic;
            }
            else if (durationHint == 6)
            {
                return Duration.Forever;
            }
            else
            {
                return TimeSpan.FromMilliseconds((double)durationHint);
            }
        }

        private RepeatBehavior RandomRepeatBehavior()
        {
            int repeatBehaviorType = RandomDistribution(.80, .15, .05);

            if (repeatBehaviorType == 0)  // Use IterationCount; the most likely scenario at 60% is the default
            {
                //                                       <-- 0    .5  1.0  1.5  2.0  2.5  3.0-->
                int iterationCountHint = RandomDistribution(.02, .05, .75, .04, .05, .04, .05);
                return new RepeatBehavior(((double)iterationCountHint) / 2);
            }
            else if (repeatBehaviorType == 1)  // Use RepeatDuration
            {
                //                                       <-- 0    1    2    3    4    5    6    7    8    9    10-->
                int iterationCountHint = RandomDistribution(.02, .10, .10, .10, .10, .10, .10, .10, .10, .10, .08);
                return new RepeatBehavior(TimeSpan.FromMilliseconds((double)iterationCountHint));
            }
            else  // repeatBehaviorType == 2, use Forever
            {
                return RepeatBehavior.Forever;
            }
        }


        /// <summary>
        /// Return an integer in the range [0, probability.Length) == [0, maxValue] such that
        /// probability of getting N = probability[N], as specified by the parameter array.
        /// </summary>
        /// <param name="probability">Probabilities of positive integers starting from 0.</param>
        /// <returns></returns>
        /// <example>
        /// Example: to get an integer with 20% of getting 0, 50% of getting 1, and 30% of getting 2:
        ///   int n = RandomDistribution(.20, .50, .30);
        /// </example>
        private int RandomDistribution(params double[] probability)
        {
            int maxValue = probability.Length - 1;  // Largest possible output value
            Debug.Assert(maxValue >= 0);

            double random = _random.NextDouble();
            double cutoff = 0;

            for (int n = 0; n < maxValue; n++)
            {
                cutoff += probability[n];
                Debug.Assert(cutoff <= 1);  // The sum of probabilities should never exceed 100%

                if (random < cutoff)  // We have reached the probability cutoff from this percentage
                {
                    return n;
                }
            }

            // If we got here, n == maxValue and we should be in the last distribution group.
            // Use this opportunity to ensure that the sum of probabilities is 100% with ~1% tolerance.
            Debug.Assert(Math.Abs(cutoff + probability[maxValue] - 1.00) <= .011);
            return maxValue;
        }


        Random _random;  // Our deterministic random seed
        Timeline _syncTree, _baselineTree;
    }

    #endregion
}
