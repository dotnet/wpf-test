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
    /// Verify non-focusable element does not kill focus from second source.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MultiSourceFocusableApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify non-focusable element does not kill focus from second source in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify non-focusable element does not kill focus from second source in Browser.")]
        [TestCase("3", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify non-focusable element does not kill focus from second source in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MultiSourceFocusableApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify non-focusable element does not kill focus from second source in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Verify non-focusable element does not kill focus from second source in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MultiSourceFocusableApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            // Set up canvases to go inside windows
            Canvas[] canvases = new Canvas[] { new Canvas() };

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

            // Set up window source
            _surface = new SurfaceCore("HwndSource", 230, 10, 200, 100);
            _surface.Show();
            _testHwnd = PresentationHelper.GetHwnd(_surface.GetPresentationSource());
            Assert(HandleRef.ToIntPtr(_testHwnd) != IntPtr.Zero, "Whoops, expected non-zero test presentation source");

            // First canvas has focusable element in focus, second canvas has non-focusable element.
            ((FrameworkElement)(_controlCollection[0])).Focusable = false;
            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 200, 100);

            return null;
        }


        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            // Focus on second window.
            Interop.SetFocus(_testHwnd);
            _origHwnd = Interop.GetFocus();

            // Focus on non-focusable element.
            ((UIElement)(_controlCollection[0])).Focus();
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
            HandleRef newHwnd = Interop.GetFocus();

            // Validate expected conditions
            CoreLogger.LogStatus("Original hwnd: (expect non-0) " + HandleRef.ToIntPtr(_origHwnd).ToInt32());
            Assert(HandleRef.ToIntPtr(_origHwnd) != IntPtr.Zero, "Error .... expected non-zero original hwnd");
            CoreLogger.LogStatus("New hwnd: (expect same as original) " + HandleRef.ToIntPtr(newHwnd).ToInt32());
            Assert(HandleRef.ToIntPtr(newHwnd) == HandleRef.ToIntPtr(_origHwnd), "Whoops, expected newHwnd to match original one");

            this.TestPassed = true;
            CoreLogger.LogStatus("Test pass status? " + TestPassed);

            _surface.Close();

            return null;
        }

        Surface _surface = null;

        private HandleRef _testHwnd;
        private HandleRef _origHwnd;

        private ArrayList _controlCollection = new ArrayList();

    }
}

