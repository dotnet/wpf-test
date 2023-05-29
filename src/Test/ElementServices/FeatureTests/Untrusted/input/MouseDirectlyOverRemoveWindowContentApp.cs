// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
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
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Mouse DirectlyOver property on a window after removing content.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseDirectlyOverRemoveWindowContentApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse DirectlyOver property on a window after removing content in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify Mouse DirectlyOver property on a window after removing content in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse DirectlyOver property on a window after removing content in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MouseDirectlyOverRemoveWindowContentApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify Mouse DirectlyOver property on a window after removing content in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Verify Mouse DirectlyOver property on a window after removing content in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseDirectlyOverRemoveWindowContentApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            // Build canvas for this window
            _cvs = new Canvas();

            InstrFrameworkPanel panel = new InstrFrameworkPanel();
            panel.Name = "nOnLostCapturebtn" + DateTime.Now.Ticks;
            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);
            panel.Height = 40;
            panel.Width = 40;
            _cvs.Children.Add(panel);

            // Put the test element on the screen
            DisplayMe(_cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Moving mouse to target...");
            MouseHelper.Move(_cvs.Children[0]);

            IInputElement mouseDirectlyOver = Mouse.DirectlyOver;
            CoreLogger.LogStatus("Mouse over anything? (should be the original window content) " +
                ((mouseDirectlyOver != null) ? mouseDirectlyOver.GetType().ToString() : "()"));

            CoreLogger.LogStatus("Zapping Window content...");
            this.TestContainer.NavigateToObject(null);

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

            IInputElement mouseDirectlyOver = Mouse.DirectlyOver;
            CoreLogger.LogStatus("Mouse over anything? (should be something other than the original window content) " +
                ((mouseDirectlyOver != null) ? mouseDirectlyOver.GetType().ToString() : "()"));
            Assert(mouseDirectlyOver != _cvs, "Oh no, mouse isn't over anything");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private Canvas _cvs;
    }
}

