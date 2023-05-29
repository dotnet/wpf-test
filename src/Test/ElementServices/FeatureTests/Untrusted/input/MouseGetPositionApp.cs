// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Mouse GetPosition() returns correct point for animated element.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseGetPositionApp : TestApp
    {
        AnimationClock _animationClock;

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse GetPosition() returns correct point for animated element in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify Mouse GetPosition() returns correct point for animated element in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse GetPosition() returns correct point for animated element in window.")]
        [TestCaseTimeout(@"120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MouseGetPositionApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify Mouse GetPosition() returns correct point for animated element in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Verify Mouse GetPosition() returns correct point for animated element in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseGetPositionApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            Canvas cvs = new Canvas();
            FrameworkElement panel = new InstrFrameworkPanel();
            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);
            panel.Height = 90;
            panel.Width = 90;

            // Add element to canvas
            cvs.Children.Add(panel);

            // Put the test element on the screen
            CoreLogger.LogStatus("Showing window....");
            _rootElement = cvs;

            // Building animation to shrink panel width under mouse.
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = panel.Width;
            anim.BeginTime = null;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
            anim.RepeatBehavior = new RepeatBehavior(1.0);
            anim.AutoReverse = false;
            anim.FillBehavior = FillBehavior.HoldEnd;
            anim.CurrentStateInvalidated += new EventHandler(OnAnimationEnded);

            // Attach animation to element.
            _animatingEl = panel;

            // Add Animation.  It isn't started yet.
            _animationClock = anim.CreateClock();
            _animatingEl.ApplyAnimationClock(Canvas.LeftProperty, _animationClock);

            DisplayMe(_rootElement, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            Canvas cvs = (Canvas)_rootElement;
            FrameworkElement panel = (FrameworkElement)(cvs.Children[0]);

            CoreLogger.LogStatus("Moving to element...");
            MouseHelper.Move(panel);

            CoreLogger.LogStatus("Animating...");
            _animationClock.Controller.Begin();

            // Give the animation time to complete.
            AnimationTimeline anim = _animationClock.Timeline;
            DispatcherHelper.DoEvents((int)anim.Duration.TimeSpan.TotalMilliseconds + 100);

            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            // We want our element to be to the left of the mouse position
            Point pos = Mouse.GetPosition(_animatingEl);
            CoreLogger.LogStatus("X position: " + pos.X);

            Assert(pos.X < 0.0, "GetPosition report not correct (expected negative number) ");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Event handler to run when an animation has ended.
        /// </summary>
        /// <param name="sender">Animation sending the event.</param>
        /// <param name="e">Not used.</param>
        private void OnAnimationEnded(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != System.Windows.Media.Animation.ClockState.Active)
            {
                // We are done.
                CoreLogger.LogStatus("Animation has ended.");
            }
        }

        private UIElement _animatingEl = null;
    }
}

