// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Test.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify repeating animation forces MouseEnter and MouseLeave to fire multiple times.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <remarks>
    /// Was disabled until new Input Framework BVT was created.
    /// </remarks>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseLeaveDoubleAnimationRepeatApp : TestApp
    {
        AnimationClock _animationClock;
        
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify repeating animation forces MouseEnter and MouseLeave to fire multiple times. in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify repeating animation forces MouseEnter and MouseLeave to fire multiple times. in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify repeating animation forces MouseEnter and MouseLeave to fire multiple times. in window.")]
        [TestCaseTimeout(@"120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MouseLeaveDoubleAnimationRepeatApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify repeating animation forces MouseEnter and MouseLeave to fire multiple times. in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Verify repeating animation forces MouseEnter and MouseLeave to fire multiple times. in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseLeaveDoubleAnimationRepeatApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            // Build canvas for this window.
            Canvas canvas = new Canvas();

            // Build element for canvas.
            FrameworkElement panel = new InstrFrameworkPanel();
            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);

            // Size element to window so it is visible.
            double winHeight = 100;
            double winWidth = 100;
            panel.Height = winHeight * 0.9;
            panel.Width = winWidth * 0.9;
            panel.MouseEnter += new MouseEventHandler(OnMouse);
            panel.MouseLeave += new MouseEventHandler(OnMouse);

            // Add element to canvas.
            canvas.Children.Add(panel);

            _rootElement = canvas;

            // Building animation to shrink panel width under mouse.
            DoubleAnimation anim = new DoubleAnimation();

            anim.From = panel.Width;
            anim.To = panel.Width * 0.1;
            anim.BeginTime = null;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            anim.RepeatBehavior = new RepeatBehavior(2.0);
            anim.AutoReverse = true;
            anim.FillBehavior = FillBehavior.HoldEnd;
            anim.CurrentStateInvalidated += new EventHandler(OnAnimationCurrentStateInvalidated);

            // Attach animation to element.
            _animatingEl = panel;

            // Add Animation.  It isn't started yet.
            _animationClock = anim.CreateClock();
            _animatingEl.ApplyAnimationClock(FrameworkElement.WidthProperty, _animationClock);

            // Put the test element on the screen
            DisplayMe(_rootElement, 1, 1, (int)winWidth, (int)winHeight);

            MouseHelper.Move(panel);           

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Starting the animation");

            // Start the clock.
            _animationClock.Controller.Begin();

            // Give the animation time to complete.
            AnimationTimeline anim = _animationClock.Timeline;
            DispatcherHelper.DoEvents((5 * ((int)anim.Duration.TimeSpan.TotalMilliseconds)));

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

            this.TestPassed = false;
            try
            {
                // We want our element to have fired a particular event.
                // Only the mouse leave event triggered by the animation
                // should be in the log when it has completed.
                CoreLogger.LogStatus("Event log: " + _eventLog.Count);
                Assert(_eventLog.Count == 5, "Event count not correct (expect 5)");

                // Log final test results.
                TestPassed = (true);
                CoreLogger.LogStatus("Test passed");
            }
            catch (Microsoft.Test.TestValidationException e)
            {
                // Log final test results.
                TestPassed = false;
                CoreLogger.LogStatus("Assert failed: " + e.Message);
            }

            CoreLogger.LogStatus("Test pass status? " + TestPassed);

            return null;
        }

        /// <summary>
        /// Event handler to run when mouse has left an element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouse(object sender, MouseEventArgs e)
        {
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            // Record this event.
            _eventLog.Add(e);
        }


        /// <summary>
        /// Event handler to run when an animation has ended.
        /// </summary>
        /// <param name="sender">Animation sending the event.</param>
        /// <param name="e">Not used.</param>
        private void OnAnimationCurrentStateInvalidated(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != System.Windows.Media.Animation.ClockState.Active)
            {
                // We are done.
                CoreLogger.LogStatus("Animation has ended. Removing event handlers...");
                UIElement panel = ((Canvas)_rootElement).Children[0];
                panel.MouseEnter -= new MouseEventHandler(OnMouse);
                panel.MouseLeave -= new MouseEventHandler(OnMouse);
            }
        }

        private UIElement _animatingEl;

        private List<MouseEventArgs> _eventLog = new List<MouseEventArgs>();
    }
}

