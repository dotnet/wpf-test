// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using DRT;
using System;
using System.Windows.Media.Animation;

namespace DRTAnimation
{
    public partial class NonVisualTestSuite : DrtTestSuite
    {
        void KeyFrameAnimationTest()
        {
            // Test some well formed linear key frames.
            {
                TimeSpan keyTime0 = TimeSpan.Zero;
                TimeSpan keyTime1 = TimeSpan.FromMilliseconds(100.0);
                TimeSpan keyTime2 = TimeSpan.FromMilliseconds(200.0);

                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.Duration = TimeSpan.FromSeconds(1.0);
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(142.0, keyTime0));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(242.0, keyTime1));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(342.0, keyTime1));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(442.0, keyTime2));
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, keyTime0, 142.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromMilliseconds(50.0), 192.0);
                SeekAndTestCurrentValue(clock, keyTime1, 342.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromMilliseconds(150.0), 392.0);
                SeekAndTestCurrentValue(clock, keyTime2, 442.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromMilliseconds(250.0), 442.0);
            }

            // Test oddly specified linear key frames.
            {
                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.Duration = TimeSpan.FromSeconds(100);
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, TimeSpan.FromSeconds(01)));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromPercent(0.25)));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(2.0, KeyTime.Uniform));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(3.0, KeyTime.Uniform));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(4.0, KeyTime.Paced));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(5.0, TimeSpan.FromSeconds(05)));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(6.0, KeyTime.Uniform));
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(7.0, TimeSpan.FromSeconds(15)));
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(01.0), 0.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(03.0), 2.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(05.0), 5.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(07.5), 4.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(10.0), 6.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(12.5), 4.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(15.0), 7.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(12.5), 4.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(17.5), 4.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(20.0), 2.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(22.5), 1.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(25.0), 1.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(30.0), 1.0);
            }

            // Test unspecified ending key frames with Duration.
            {
                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.Duration = TimeSpan.FromSeconds(100);
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, TimeSpan.FromSeconds(50))); //  50
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, TimeSpan.FromSeconds(20))); //  20
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(2.0, KeyTime.Uniform));          //  40
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(3.0, KeyTime.Uniform));          //  60
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(4.0, KeyTime.Paced));            //  80
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(5.0, KeyTime.Uniform));          // 100
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(10.0), 0.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(20.0), 1.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(30.0), 1.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(40.0), 2.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(45.0), 1.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(50.0), 0.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(55.0), 1.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(60.0), 3.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(70.0), 3.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(80.0), 4.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(90.0), 4.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(100.0), 5.0);
            }

            // Test unspecified ending key frames with natural duration.
            {
                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, TimeSpan.FromSeconds(100))); // 100
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, TimeSpan.FromSeconds(20)));  //  20
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(2.0, KeyTime.Uniform));           //  40
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(3.0, KeyTime.Uniform));           //  60
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(4.0, KeyTime.Paced));             //  80
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(5.0, KeyTime.Uniform));           // 100
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(10.0), 0.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(20.0), 1.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(30.0), 1.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(40.0), 2.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(50.0), 2.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(60.0), 3.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(70.0), 3.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(80.0), 4.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(90.0), 2.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(100.0), 5.0);
            }

            // Test paced beginning key frame.
            {
                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.Paced));               //   0
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.Uniform));             //  50
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(2.0, TimeSpan.FromSeconds(100)));   // 100
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds( 0.0), 0.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(25.0), 0.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(50.0), 1.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(75.0), 1.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(100.0), 2.0);
            }

            // Test uniform beginning key frame.
            {
                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.Uniform));             //  30
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(2.0, KeyTime.Uniform));             //  60
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(3.0, TimeSpan.FromSeconds(90)));   //   90
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(15.0), 0.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(30.0), 1.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(45.0), 1.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(60.0), 2.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(75.0), 2.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(90.0), 3.0);
            }

            // Test all key frames unspecified, will use default calculationDuration of 1.0.
            {
                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.Uniform)); // 0.25
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(2.0, KeyTime.Uniform)); // 0.50
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(3.0, KeyTime.Paced));   // 0.75
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(4.0, KeyTime.Uniform)); // 1.00
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(0.125), 0.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(0.250), 1.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(0.375), 1.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(0.500), 2.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(0.625), 2.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(0.750), 3.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(0.875), 3.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(1.000), 4.0);
            }

            // Single uniform key frame with Duration specified.
            {
                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.Duration = TimeSpan.FromSeconds(2);
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.Uniform)); // 0
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(00.0), 0.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(01.0), 0.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(02.0), 1.0);
            }

            // Single paced key frame with Duration specified.
            {
                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.Duration = TimeSpan.FromSeconds(2);
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.Paced)); // 1
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(00.0), 0.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(01.0), 0.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(02.0), 1.0);
            }

            // Key frames past the duration.
            {
                DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
                anim.Duration = TimeSpan.FromSeconds(10);
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, TimeSpan.FromSeconds(05))); // 5
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(2.0, TimeSpan.FromSeconds(15))); // 15
                anim.KeyFrames.Add(new LinearDoubleKeyFrame(3.0, TimeSpan.FromSeconds(25))); // 25
                anim.Freeze();

                AnimationClock clock = anim.CreateClock();

                if (clock.CurrentState != ClockState.Stopped)
                {
                    throw new Exception("A newly created clock should have an initial CurrentState of 'Stopped'.");
                }

                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(02.5), 0.5);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(05.0), 1.0);
                SeekAndTestCurrentValue(clock, TimeSpan.FromSeconds(10.0), 1.5);
            }

            Console.WriteLine("    NonVisualTestSuite.KeyFrameAnimationTest() completed.");
        }

        void SeekAndTestCurrentValue(AnimationClock animationClock, TimeSpan seekOffset, Double expectedValue)
        {
            animationClock.Controller.SeekAlignedToLastTick(seekOffset, TimeSeekOrigin.BeginTime);

            Double currentValue = (Double)animationClock.GetCurrentValue(0.0, 0.0);

            if (!ApproximatelyEquals(currentValue, expectedValue))
            {
                throw new Exception(String.Format("When seeked to {0} the AnimationClock should have a current value of '{1}', instead the value is '{2}'", seekOffset, expectedValue, currentValue));
            }
        }

        private bool ApproximatelyEquals(double value1, double value2)
        {
            if (   value1 > value2 - 0.0001
                && value1 < value2 + 0.0001)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
