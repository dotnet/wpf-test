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
    /// Verify Mouse DirectlyOver property on a point-animated drawing visual.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseDirectlyOverPointAnimationVisualApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse DirectlyOver property on a point-animated drawing visual in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify Mouse DirectlyOver property on a point-animated drawing visual in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse DirectlyOver property on a point-animated drawing visual in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MouseDirectlyOverPointAnimationVisualApp",
                "Run",
                hostType);
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify Mouse DirectlyOver property on a point-animated drawing visual in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Verify Mouse DirectlyOver property on a point-animated drawing visual in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseDirectlyOverPointAnimationVisualApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            InstrFrameworkPanel cvs = new InstrFrameworkPanel();

            CoreLogger.LogStatus("Creating visual element...");
            _drawingVisual = new DrawingVisual();
            DrawingContext myDrawingContext = _drawingVisual.RenderOpen();

            // Build animation
            _pointAnimation = new PointAnimation();

            _pointAnimation.From = new Point(50, 50);
            _pointAnimation.To = new Point(250, 250);
            _pointAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(3000));
            _pointAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            _pointAnimation.CurrentStateInvalidated += new EventHandler(OnAnimationEnded);
            _pointAnimation.AutoReverse = true;
            _pointAnimation.RepeatBehavior = new RepeatBehavior(1);

            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Orange);
            Pen myPen = new Pen(Brushes.Black, 10);
            _ellipseGeometry = new EllipseGeometry(new Point(50, 50), 50, 75);
            myDrawingContext.DrawGeometry(mySolidColorBrush, myPen, _ellipseGeometry);

            myDrawingContext.Close();

            CoreLogger.LogStatus("Adding visual element to canvas...");
            cvs.Children.Add(_drawingVisual);

            // Put the test element on the screen
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Moving mouse...");
            MouseHelper.Move(_rootElement);
            Assert(Mouse.DirectlyOver == _rootElement, "Oh no, mouse not directly over window content when it should be");
            CoreLogger.LogStatus("Mouse moved!");

            CoreLogger.LogStatus("Beginning animation...");
            _ellipseGeometry.BeginAnimation(EllipseGeometry.CenterProperty, _pointAnimation);

            CoreLogger.LogStatus("Waiting for animation to complete...");
            DispatcherHelper.DoEvents(5000);

            base.DoExecute(arg);

            return null;
        }


        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            Assert(Mouse.DirectlyOver == _rootElement, "Oh no, mouse not directly over window content when it should be");

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
            CoreLogger.LogStatus("Animation state = "+((Clock)sender).CurrentState);
            if (((Clock)sender).CurrentState != System.Windows.Media.Animation.ClockState.Active)
            {
                // We are done.
                CoreLogger.LogStatus("Animation has ended.");
            }
        }

        private PointAnimation _pointAnimation;

        private Geometry _ellipseGeometry;

        private DrawingVisual _drawingVisual;
    }
}

