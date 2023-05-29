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
    /// Verify non-focusable element does not kill focus from second window.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MultiWindowFocusableApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify non-focusable element does not kill focus from second window in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify non-focusable element does not kill focus from second window in Browser.")]
        [TestCase("3", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify non-focusable element does not kill focus from second window in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MultiWindowFocusableApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify non-focusable element does not kill focus from second window in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Verify non-focusable element does not kill focus from second window in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MultiWindowFocusableApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            Canvas[] canvases = new Canvas[] { new Canvas(), new Canvas() };

            foreach (Canvas cvs in canvases)
            {
                // Add panel to each canvas
                FrameworkElement panel = new InstrFrameworkPanel();
                Canvas.SetTop(panel, 0);
                Canvas.SetLeft(panel, 100);
                panel.Height = 40;
                panel.Width = 50;
                cvs.Children.Add(panel);

                // Save panel for later
                _controlCollection.Add(panel);
            }

            // First canvas has focusable element in focus, second canvas has non-focusable element.
            ((FrameworkElement)(_controlCollection[0])).Focusable = true;
            ((FrameworkElement)(_controlCollection[1])).Focusable = false;

            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 200, 100);
            DisplayMe(canvases[1], 230, 10, 200, 100);

            return null;
        }


        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            // Focus on focusable element.
            ((UIElement)(_controlCollection[0])).Focus();
            _origHwnd = NativeMethods.GetFocus();

            // Focus on non-focusable element.
            ((UIElement)(_controlCollection[1])).Focus();
            CoreLogger.LogStatus("Getting ready to verify focus...");

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

            // What has the focus?
            IntPtr newHwnd = NativeMethods.GetFocus();

            // Validate expected conditions
            CoreLogger.LogStatus("Original hwnd: (expect non-0) " + _origHwnd.ToInt32());
            Assert(_origHwnd.ToInt32() != 0, "Error .... expected non-zero original hwnd");
            CoreLogger.LogStatus("New hwnd: (expect same as original) " + newHwnd.ToInt32());
            Assert(newHwnd.ToInt32() == _origHwnd.ToInt32(), "Whoops, expected newHwnd to match original one");

            this.TestPassed = true;
            TestContainer.CurrentSurface[1].Close();
            CoreLogger.LogStatus("Test pass status? " + TestPassed);

            return null;
        }

        private ArrayList _controlCollection = new ArrayList();

        IntPtr _origHwnd = IntPtr.Zero;
    }
}

