// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
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
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify mouse capture forces mouse button event to fire when element is positioned away from mouse cursor.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CaptureDoubleAnimationPositionApp : TestApp
    {
        AnimationClock _animationClock;

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Capture", "HwndSource", @"Compile and Verify mouse capture forces mouse button event to fire when element is positioned away from mouse cursor in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Browser", @"Compile and Verify mouse capture forces mouse button event to fire when element is positioned away from mouse cursor in Browser.")]
        [TestCase("3", @"CoreInput\Capture", "Window", @"Compile and Verify mouse capture forces mouse button event to fire when element is positioned away from mouse cursor in window.")]
        [TestCaseTimeout(@"120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "CaptureDoubleAnimationPositionApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Capture", "HwndSource", @"Verify mouse capture forces mouse button event to fire when element is positioned away from mouse cursor in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Window", @"Verify mouse capture forces mouse button event to fire when element is positioned away from mouse cursor in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CaptureDoubleAnimationPositionApp(), "Run");

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
            panel.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButtonDown);
            panel.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseButtonUp);

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

        //private Style BuildStyleWithAnimation()
        //{
        //    Style newStyle = new Style(typeof(InstrFrameworkPanel));
        //    newStyle.Storyboards.Add(_mainTimeline);

        //    return newStyle;

        //}

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

            CoreLogger.LogStatus("Mousing down...");
            MouseHelper.PressButton();

            CoreLogger.LogStatus("Animating...");
            _animationClock.Controller.Begin();

            // Give the animation time to complete.
            AnimationTimeline anim = _animationClock.Timeline;
            DispatcherHelper.DoEvents((int)anim.Duration.TimeSpan.TotalMilliseconds + 100);

            CoreLogger.LogStatus("Mousing up...");
            MouseHelper.ReleaseButton();

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

            Canvas cvs = (Canvas)_rootElement;
            FrameworkElement panel = (FrameworkElement)(cvs.Children[0]);

            // We want our element to have fired a particular event.
            CoreLogger.LogStatus("Event log: " + _eventLog.Count);
            this.Assert(_eventLog.Count == 1, "Event count not correct (expected 1) ");
            this.Assert(Mouse.DirectlyOver != panel, "Mouse still over panel (expected 1) ");

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

        /// <summary>
        /// Event handler to run when mouse is down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" Mouse is down. Capturing....");

            // Capture this element.
            ((IInputElement)(sender)).CaptureMouse();
        }

        /// <summary>
        /// Event handler to run when mouse is up.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
        {

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("Button=" + e.ChangedButton.ToString() + ",State=" + e.ButtonState.ToString() + ",ClickCount=" + e.ClickCount);
            CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: " +
                                e.LeftButton.ToString() + "," +
                                e.RightButton.ToString() + "," +
                                e.MiddleButton.ToString() + "," +
                                e.XButton1.ToString() + "," +
                                e.XButton2.ToString()
                                );

            // Capture this element.
            CoreLogger.LogStatus(" Mouse is up. Uncapturing....");
            ((IInputElement)(sender)).ReleaseMouseCapture();

            // Record this event.
            _eventLog.Add(e);

            // Don't route this event any more.
            e.Handled = true;
        }

        private UIElement _animatingEl = null;

        private List<MouseButtonEventArgs> _eventLog = new List<MouseButtonEventArgs>();
    }
}

