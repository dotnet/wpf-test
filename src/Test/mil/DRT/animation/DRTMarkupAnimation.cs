// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using DRT;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace DRTAnimation
{
    public sealed class DRTMarkupAnimationTestSuite : DrtTestSuite
    {
        public DRTMarkupAnimationTestSuite() : base("Markup Animation Tests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.LoadXamlFile(@"DrtFiles\DrtAnimation\DRTMarkupAnimation.xaml");

            return new DrtTest[] 
            {
                new DrtTest(RunTest),
            };
        }

        private void RunTest()
        {
            FrameworkElement progressRectangle = (FrameworkElement)DRT.FindElementByID("progressRectangle");

            Storyboard storyboard = new Storyboard();
            Storyboard.SetTargetProperty(storyboard, new PropertyPath(Rectangle.WidthProperty));
            
            if (!DRTAnimation.IsInteractive)
            {
                storyboard.CurrentStateInvalidated += new EventHandler(OnProgressBarAnimationCurrentStateInvalidated);
            }

            DoubleAnimation anim1 = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            storyboard.Children.Add(anim1);
            anim1.AccelerationRatio = 1.0;

            DoubleAnimation anim2 = new DoubleAnimation(105, TimeSpan.FromSeconds(1.5));
            storyboard.Children.Add(anim2);
            anim2.BeginTime = anim1.Duration.TimeSpan;
            anim2.DecelerationRatio = 1.0;

            progressRectangle.BeginStoryboard(storyboard);

            DRT.Suspend();
        }

        private void OnProgressBarAnimationCurrentStateInvalidated(object sender, EventArgs args)
        {
            Clock clock = (Clock)sender;

            if (clock.CurrentState != ClockState.Active)
            {
                DRT.Resume();

                Console.WriteLine("    DRTMarkupAnimationTestSuite.RunTest() completed.");
            }
        }
    }
}
