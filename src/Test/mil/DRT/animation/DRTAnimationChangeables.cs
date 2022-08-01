// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// DRTAnimationFreezables.cs

using DRT;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace DRTAnimation
{
    public sealed partial class NonVisualTestSuite : DrtTestSuite
    {
        private int _ChangedCount1;
        private Rectangle _rectangle;

        public void FreezablesTest()
        {
            SingleAnimationUsingKeyFrames anim = new SingleAnimationUsingKeyFrames();

            double doubleTemp;
            LinearSingleKeyFrame keyFrame;

            SingleKeyFrameCollection keyFrames = new SingleKeyFrameCollection();

            keyFrames.Add(new LinearSingleKeyFrame(0.0f, KeyTime.FromPercent(0.0)));
            keyFrames.Add(new LinearSingleKeyFrame(1.0f, KeyTime.FromPercent(1.0)));

            anim.KeyFrames = keyFrames;

            keyFrame = (LinearSingleKeyFrame)keyFrames[1];
            keyFrame.KeyTime = KeyTime.FromPercent(0.5);
            keyFrames[1] = keyFrame;

            doubleTemp = keyFrames[1].KeyTime.Percent;
            DRTAnimation.Check(
                0.5, doubleTemp,
                "keyFrames[1].KeyTime.Percent");

            keyFrame = (LinearSingleKeyFrame)anim.KeyFrames[1];
            keyFrame.KeyTime = KeyTime.FromPercent(0.75);
            anim.KeyFrames[1] = keyFrame;

            doubleTemp = anim.KeyFrames[1].KeyTime.Percent;
            DRTAnimation.Check(
                0.75, doubleTemp,
                "anim.KeyFrames[1].KeyTime.Percent");

            keyFrame = (LinearSingleKeyFrame)anim.KeyFrames[0];
            keyFrame.Value = 5.0f;
            anim.KeyFrames[0] = keyFrame;

            keyFrame = (LinearSingleKeyFrame)anim.KeyFrames[1];
            keyFrame.KeyTime = KeyTime.FromPercent(1.0);
            keyFrame.Value = 5.0f;
            anim.KeyFrames[1] = keyFrame;

            // Set a duration so that we can resolve the keyframes.
            anim.Duration = TimeSpan.FromSeconds(10);

            {
                ParallelTimeline t1 = new ParallelTimeline();
                ParallelTimeline t2 = new ParallelTimeline();

                ClockGroup c1 = t1.CreateClock();
                ClockGroup c2 = t2.CreateClock();

                if (c1.Children.Equals(c2.Children))
                {
                    throw new Exception("Clock collections for two different ClockGroups should not be equal.");
                }
            }

            {
                ParallelTimeline p1 = new ParallelTimeline();
                p1.Changed += new EventHandler(OnAnimationChanged1);

                DoubleAnimation a1 = new DoubleAnimation();
                p1.Children.Add(a1);
                _ChangedCount1 = 0;
                a1.To = 500;

                if (_ChangedCount1 != 1)
                {
                    throw new Exception("TimelineCollection.Add has not correctly propagated parent Changed handlers to new child.");
                }

                p1.Children.Remove(a1);
                _ChangedCount1 = 0;
                a1.To = 250;

                if (_ChangedCount1 != 0)
                {
                    throw new Exception("TimelineCollection.Remove has not correctly removed changed parent Changed handlers from removed child.");
                }

                p1.Children.Insert(0, a1);
                _ChangedCount1 = 0;
                a1.To = 500;

                if (_ChangedCount1 != 1)
                {
                    throw new Exception("TimelineCollection.Insert has not correctly propagated parent Changed handlers to new child.");
                }

                p1.Children.Clear();
                _ChangedCount1 = 0;
                a1.To = 250;

                if (_ChangedCount1 != 0)
                {
                    throw new Exception("TimelineCollection.Clear has not correctly removed changed parent Changed handlers from removed child.");
                }

                p1.Changed -= new EventHandler(OnAnimationChanged1);
            }

            // String Converter Tests
            {
                RepeatBehavior count = new RepeatBehavior(7);
                RepeatBehavior duration = new RepeatBehavior(TimeSpan.FromSeconds(3));
                RepeatBehavior forever = RepeatBehavior.Forever;

                DRTAnimation.Check("7x", count.ToString(), "ToString() test of a RepeatBehavior with an IterationCount of 7");
                DRTAnimation.Check("00:00:03", duration.ToString(), "ToString() test of a RepeatBehavior with a RepeatDuration of 3 seconds");
                DRTAnimation.Check("Forever", forever.ToString(), "ToString() test of a RepeatBehavior.Forever");
            }

            //
            // RC0 Task 40719: Clocks use frozen copies of timelines 
            // 
            AnimationClock clock = anim.CreateClock();

            DRTAnimation.Check(clock.Timeline.IsFrozen, "A clock should always reference a frozen timeline");
            DRTAnimation.Check(!anim.IsFrozen, "Creating a clock should have no side-effect on the animation");
            DRTAnimation.Check(clock.Timeline != anim, "A clock's timeline should be a frozen copy of the original timeline if the original wasn't frozen");

            float value = (float)clock.GetCurrentValue(5.0f, 5.0f);
        
            anim.Freeze();

            DRTAnimation.Check(anim.IsFrozen, "Animations should be able to become frozen.");

            clock = anim.CreateClock();

            DRTAnimation.Check(clock.Timeline == anim, "A frozen animation should not be copied when used to create a Clock.");
            

            // Test for memory leak.  Ensure that if two Animatables are being
            // animated by the same AnimationClock that they are not keeping the
            // other from being collected.

            DoubleAnimation danim = new DoubleAnimation(0.0, 1.0, new TimeSpan(0, 0, 0, 0, 1000));
            clock = danim.CreateClock();

            Brush brush1 = new SolidColorBrush(Colors.White);
            Brush brush2 = new SolidColorBrush(Colors.Black);
            Brush brush3 = new SolidColorBrush(Colors.Red);

            // NOTE: Sometimes the PropertyAnimationClockCollections were being
            // left on the stack and were causing false failures of this test
            // because they were not being garbage collected and had references
            // to the brushes.
            // By calling out to a function to apply these properties we ensure
            // the PropertyAnimationClockCollections will be off the stack and
            // collected when we do the GC.Collect();
            brush1.ApplyAnimationClock(Brush.OpacityProperty, clock);
            brush2.ApplyAnimationClock(Brush.OpacityProperty, clock);

            WeakReference brush1Ref = new WeakReference(brush1);
            WeakReference brush2Ref = new WeakReference(brush2);
            WeakReference brush3Ref = new WeakReference(brush3);

            brush1 = null;
            brush3 = null;

            GC.Collect();
            GC.GetTotalMemory(true);

            // Test to make sure we throw the right exception when trying to 
            // add animations to a frozen Freezable.

            try
            {
                ColorAnimation colorAnim = new ColorAnimation();

                Brush brush = new SolidColorBrush();
                brush.Freeze();
                brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            }
            catch (InvalidOperationException)
            {
                // Great, we expected this exception.
            }
            catch (Exception e)
            {
                // We didn't expect this exception, test failed.
                throw new Exception("Trying to add an animation to a frozen Freezable should throw an InvalidOperationException", e);
            }

            // Test to make sure null is allowed as a parameter to BeginAnimation on Animatable.
            try
            {
                Brush myBrush = new SolidColorBrush();
                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
            }
            catch
            {
                throw new Exception("Null should be allowed as a parameter to BeginAnimation method on Animatable.");
            }

            // Test to make sure null is allowed as a parameter to BeginAnimation on UIElement.
            try
            {
                Rectangle myRect = new Rectangle();
                myRect.BeginAnimation(Rectangle.WidthProperty, null);
            }
            catch
            {
                throw new Exception("Null should be allowed as a parameter to BeginAnimation method on UIElement.");
            }

            // Test to make sure null is allowed as a parameter to BeginAnimation on ContentElement.
            try
            {
                Bold myBold = new Bold();
                myBold.BeginAnimation(Bold.FontFamilyProperty, null);
            }
            catch
            {
                throw new Exception("Null should be allowed as a parameter to BeginAnimation method on ContentElement.");
            }

            // Test that we throw with an animation of the wrong type on Animatable.
            try
            {
                Brush myBrush = new SolidColorBrush();
                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, new DoubleAnimation());
            }
            catch (ArgumentException)
            {
                // Great, we expected this exception.
            }
            catch (Exception e)
            {
                // We didn't expect this exception, test failed.
                throw new Exception("Animatable should throw an ArgumentException when calling BeginAnimation with an animation of an incompatible type.", e);
            }

            // Test that we throw with an animation of the wrong type on UIElement.
            try
            {
                Rectangle myRect = new Rectangle();
                myRect.BeginAnimation(Rectangle.WidthProperty, new ColorAnimation());
            }
            catch (ArgumentException)
            {
                // Great, we expected this exception.
            }
            catch (Exception e)
            {
                // We didn't expect this exception, test failed.
                throw new Exception("UIElement should throw an ArgumentException when calling BeginAnimation with an animation of an incompatible type.", e);
            }

            // Test that we throw with an animation of the wrong type on ContentElement.
            try
            {
                Bold myBold = new Bold();
                myBold.BeginAnimation(Bold.FontFamilyProperty, new DoubleAnimation());
            }
            catch (ArgumentException)
            {
                // Great, we expected this exception.
            }
            catch (Exception e)
            {
                // We didn't expect this exception, test failed.
                throw new Exception("ContentElement should throw an ArgumentException when calling BeginAnimation with an animation of an incompatible type.", e);
            }

            // Test that we're not raising too many or too few Changed events.
            {
                DoubleAnimation anim1 = new DoubleAnimation();
                anim1.Changed += new EventHandler(OnAnimationChanged1);
                anim1.From = 1.0;
                anim1.To = 2.0;
                anim1.By = 3.0;
                anim1.IsAdditive = true;
                anim1.IsCumulative = true;
                anim1.RepeatBehavior = RepeatBehavior.Forever;

                if (_ChangedCount1 != 6)
                {
                    throw new Exception("DoubleAnimation is raising the wrong number of Changed events. Expected 6, Received " + _ChangedCount1.ToString());
                }
            }

            // Empty DoubleAnimation serialization.
            {
                string expectedString = "<DoubleAnimation xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />";
                string alternateString = "<DoubleAnimation xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />";
                DoubleAnimation anim1 = new DoubleAnimation();

                string outputString = XamlWriter.Save(anim1);

                Console.WriteLine(String.Format(
                        "\nDefault DoubleAnimation serialization\nExpected:\n     {0}\nAlternate:\n    {1}\nReceived:\n     {2}\n",
                        expectedString,
                        alternateString,
                        outputString));

                if (!XamlStringsEqual(outputString, expectedString) && !XamlStringsEqual(outputString, alternateString)) 
                {
                    throw new Exception("Empty DoubleAnimation serialization failed, see log for discrepancy.");
                }
            }

            // DoubleAnimation property serialization.
            {
                string expectedString = "<DoubleAnimation xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" BeginTime=\"00:00:05\" By=\"{x:Null}\" Duration=\"00:00:02.5000000\" FillBehavior=\"Stop\" From=\"1\" IsAdditive=\"False\" IsCumulative=\"True\" RepeatBehavior=\"1.5x\" To=\"2\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" />";
                string alternateString = "<DoubleAnimation To=\"2\" From=\"1\" BeginTime=\"00:00:05\" By=\"{x:Null}\" FillBehavior=\"Stop\" RepeatBehavior=\"1.5x\" Duration=\"00:00:02.5000000\" IsCumulative=\"True\" IsAdditive=\"False\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" />";
                DoubleAnimation anim1 = new DoubleAnimation();
                anim1.From = 1.0;
                anim1.To = 2.0;
                anim1.By = null;
                anim1.BeginTime = TimeSpan.FromSeconds(5.0);
                anim1.Duration = TimeSpan.FromSeconds(2.5);
                anim1.FillBehavior = FillBehavior.Stop;
                anim1.RepeatBehavior = new RepeatBehavior(1.5);
                anim1.IsAdditive = false;
                anim1.IsCumulative = true;

                string outputString = XamlWriter.Save(anim1);

                Console.WriteLine(String.Format(
                        "\nDoubleAnimation property serialization\nExpected:\n     {0}\nAlternate:\n    {1}\nReceived:\n     {2}\n",
                        expectedString,
                        alternateString,
                        outputString));

                if (!XamlStringsEqual(outputString, expectedString) && !XamlStringsEqual(outputString, alternateString)) 
                {
                    throw new Exception("DoubleAnimation property serialization failed, see log for discrepancy.");
                }
            }

            // Test Rotation3D typed properties working OK.
            {
                _ChangedCount1 = 0;

                Rotation3DAnimation rotation3DAnim = new Rotation3DAnimation();
                rotation3DAnim.Changed += new EventHandler(OnAnimationChanged1);

                AxisAngleRotation3D fromValue = new AxisAngleRotation3D();
                AxisAngleRotation3D toValue = new AxisAngleRotation3D();
                AxisAngleRotation3D byValue = new AxisAngleRotation3D();

                rotation3DAnim.From = fromValue;

                if (_ChangedCount1 != 1)
                {
                    throw new Exception("Rotation3DAnimation: _ChangedCount1 != 1");
                }

                fromValue.Angle = 90;

                if (_ChangedCount1 != 2)
                {
                    throw new Exception("Rotation3DAnimation: _ChangedCount1 != 2");
                }

                rotation3DAnim.To = toValue;
                rotation3DAnim.By = byValue;

                if (_ChangedCount1 != 4)
                {
                    throw new Exception("Rotation3DAnimation: _ChangedCount1 != 4");
                }

                toValue.Angle = 180;

                if (_ChangedCount1 != 5)
                {
                    throw new Exception("Rotation3DAnimation: _ChangedCount1 != 5");
                }

                byValue.Angle = 270;

                if (_ChangedCount1 != 6)
                {
                    throw new Exception("Rotation3DAnimation: _ChangedCount1 != 6");
                }

                rotation3DAnim.Freeze();

                // We will receive 4 changes here:
                // 1 each for From, To, and By values becoming frozen.
                // 1 for the Rotation3DAnimation itself becoming frozen.
                // Which will bring the total changed count from 6 to 10.

                if (_ChangedCount1 != 10)
                {
                    throw new Exception("Rotation3DAnimation: _ChangedCount1 != 10");
                }

                if (!fromValue.IsFrozen
                    || !toValue.IsFrozen
                    || !byValue.IsFrozen)
                {
                    throw new Exception("Rotation3DAnimation: From, To, and/or By values did not freeze when animation was frozen.");
                }
            }


            // Test that cloning of Timelines is being done correctly
            // 
            {
                ParallelTimeline parent = new ParallelTimeline();
                parent.BeginTime    = TimeSpan.FromMilliseconds(0);
                parent.Name         = "Parent";

                ParallelTimeline child = new ParallelTimeline();
                child.BeginTime     = TimeSpan.FromMilliseconds(0);
                child.Duration      = new System.Windows.Duration(TimeSpan.FromMilliseconds(10));
                child.Name          = "Child";

                child.Freeze();

                parent.Children.Add(child);

                ClockGroup parentClock = parent.CreateClock();
            

            // 


            
                BindingOperations.SetBinding(parent, Timeline.DurationProperty, new Binding());
                if (parent.CanFreeze)
                {
                    throw new Exception("Expecting a timeline that can't be frozen");
                }

                // The real test is here: CreateClock will create a copy of the Timeline.
                // If it was using the wrong clone method, we'd get an assertion that we
                // can't Freeze the timeline.
                parentClock = parent.CreateClock();

                if (!parentClock.Timeline.IsFrozen)
                {
                    throw new Exception("GetCurrentValueAsFrozen didn't freeze the result");
                }
            }
            
            

            Console.WriteLine("    NonVisualTestSuite.FreezablesTest() completed.");
        }

        private void BeginAnimationTest()
        {
            DoubleAnimation animation1 = new DoubleAnimation();
            animation1.Freeze();

            DoubleAnimation animation2 = new DoubleAnimation();
            animation2.Freeze();

            Button button = new Button();

            // HasAnimatedProperties
            if (button.HasAnimatedProperties != false)
            {
                throw new Exception("'button.HasAnimatedProperties' should return false when no animation has been set.");
            }

            // BeginAnimation
            button.BeginAnimation(Button.OpacityProperty, animation1);

            if (!button.HasAnimatedProperties)
            {
                throw new Exception("'button.BeginAnimation(Button.OpacityProperty, animation1);' failed.");
            }

            // Handoff
            button.BeginAnimation(Button.OpacityProperty, animation2);

            if (!button.HasAnimatedProperties)
            {
                throw new Exception("'button.BeginAnimation(Button.OpacityProperty, animation2);' failed.");
            }

            // Compose
            button.BeginAnimation(Button.OpacityProperty, animation1, HandoffBehavior.Compose);

            if (!button.HasAnimatedProperties)
            {
                throw new Exception("'button.BeginAnimation(Button.OpacityProperty, animation1, HandoffBehavior.Compose);' failed.");
            }

            // Remove all
            button.BeginAnimation(Button.OpacityProperty, null);

            if (button.HasAnimatedProperties)
            {
                throw new Exception("'button.BeginAnimation(Button.OpacityProperty, null);' failed.");
            }

            Console.WriteLine("    NonVisualTestSuite.AnimationCollectionTest() completed.");
        }


        private void ApplyAnimationClockTest1()
        {
            _rectangle = new Rectangle();
            _rectangle.Width = 50;

            DoubleAnimation anim = new DoubleAnimation();
            anim.To = 100;
            anim.Duration = TimeSpan.Zero;
            anim.CurrentStateInvalidated += new EventHandler(ApplyAnimationClockTest1_OnCurrentStateInvalidated);

            // Animation clock is already filling.
            _rectangle.ApplyAnimationClock(Rectangle.WidthProperty, anim.CreateClock());

            // wait till the next tick to query the value
            DRT.Suspend();
        }

        private void ApplyAnimationClockTest1_OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            Clock clock = (Clock)sender;

            if (clock.CurrentState != ClockState.Filling)
            {
                throw new Exception("Clock state after ApplyAnimationClock([To=100,Duration=0]) should be filling");
            }

            if (_rectangle.Width != 100)
            {
                throw new Exception("Rectangle.Width after ApplyAnimationClock([To=100,Duration=0]) should be 100");
            }

            DRT.Resume();
        }

        private void ApplyAnimationClockTest2()
        {
            // Apply a composed By animation. Value should be 150.
            DoubleAnimation anim = new DoubleAnimation();
            anim.By = 50;
            anim.Duration = TimeSpan.Zero;
            anim.CurrentStateInvalidated += new EventHandler(ApplyAnimationClockTest2_OnCurrentStateInvalidated);

            _rectangle.ApplyAnimationClock(Rectangle.WidthProperty, anim.CreateClock(), HandoffBehavior.Compose);

            // wait till the next tick to query the value
            DRT.Suspend();
        }

        private void ApplyAnimationClockTest2_OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            Clock clock = (Clock)sender;

            if (clock.CurrentState != ClockState.Filling)
            {
                throw new Exception("Clock state after ApplyAnimationClock([By=50,Duration=0]) should be filling");
            }

            if (_rectangle.Width != 150)
            {
                throw new Exception("Rectangle.Width after ApplyAnimationClock([By=50,Duration=0]) should be 150.");
            }

            DRT.Resume();
        }

        private void ApplyAnimationClockTest3()
        {
            // Apply a composed null clock. Nothing should happen.
            _rectangle.ApplyAnimationClock(Rectangle.WidthProperty, null, HandoffBehavior.Compose);

            if (_rectangle.Width != 150)
            {
                throw new Exception("Rectangle.Width after ApplyAnimationClock([null], HandoffBehavior.Compose) should be 150.");
            }

            // Clear animations with a null clock. Value should go back to the 
            // base value of 50.

            _rectangle.ApplyAnimationClock(Rectangle.WidthProperty, null);

            if (_rectangle.Width != 50)
            {
                throw new Exception("Rectangle.Width after ApplyAnimationClock([null]) should be 50.");
            }

            Console.WriteLine("    NonVisualTestSuite.ApplyAnimationClockTest() completed.");
        }


        private void InvalidValuesTest()
        {
            bool caught = false;
            AnimationClock clock;

            // Invalid To value
            // Leaving To unspecified results in DoubleAnimationBase.CloneCurrentValue
            // using whatever default values we feel like passing in.  This allows us to
            // attempt to animate an invalid value.
            //
            _rectangle = new Rectangle();
            _rectangle.Width = 50;          //
            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.Duration = TimeSpan.FromSeconds(5);
            widthAnimation.From = 100;
            // purposely leave To unspecified so that I can 
            // convince it to pick up an invalid value in CloneCurrentValue below
            
            clock = widthAnimation.CreateClock();
            _rectangle.ApplyAnimationClock(FrameworkElement.WidthProperty, clock);

            // Call SeekAlignedToLastTick in order to convince the clock to be Active without having to wait for a Tick.
            // Internally it calls DoubleAnimationBase.CloneCurrentValue and passes in a valid value for
            // both the default and destination (since From and Width are both specified).  
            clock.Controller.SeekAlignedToLastTick(TimeSpan.FromSeconds(1), TimeSeekOrigin.BeginTime);

            try
            {
                // now that the clock has started we can manually pass in a bad value to
                // CloneCurrentValue. It will throw: To wasn't specified so it has
                // no choice but to use the NaN I'm giving it here.
                clock.GetCurrentValue(0.1, double.NaN);
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }
            finally
            {
                if (!caught)
                {
                    throw new Exception("Error: Avalon did not throw when attempting to animate to an invalid value");
                }

                clock.Controller.Stop();
            }
            
            
            
            //
            // Invalid From value.  This time it's Rect.Empty that's invalid.
            // This was prompted by 

            caught = false;
            clock = null;
            ImageDrawing testImageDrawing = new ImageDrawing();
            testImageDrawing.Rect = Rect.Empty;  // set a value that's invalid for animation
            RectAnimation rectAnimation = new RectAnimation();
            rectAnimation.To = new Rect(10, 10, 100, 100);
            rectAnimation.Duration = new Duration(TimeSpan.FromSeconds(5));

            clock = rectAnimation.CreateClock();
            testImageDrawing.ApplyAnimationClock(ImageDrawing.RectProperty, clock);

            try
            {
                // This seek throws because we've set the DP (Rect) to an invalid value (Rect.Empty).
                clock.Controller.SeekAlignedToLastTick(TimeSpan.FromSeconds(1), TimeSeekOrigin.BeginTime);
            }
            catch (AnimationException)
            {
                caught = true;
            }
            finally
            {
                if (!caught)
                {
                    throw new Exception("Error: Avalon did not throw when attempting to animate from an invalid value");
                }

                // stop the animation
                clock.Controller.Stop();
            }            
             
        }
     
        private void OnChanged(object sender, EventArgs args)
        {
        }

        private void OnAnimationChanged1(object sender, EventArgs args)
        {
            _ChangedCount1++;
        }
        
        private static bool XamlStringsEqual(string xamlString, string expectedXamlString)
        {
            return Microsoft.Test.Markup.CompareResult.Equivalent == Microsoft.Test.Markup.XamlComparer.Compare(xamlString, expectedXamlString).Result;
        }
    }
}
