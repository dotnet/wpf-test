// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
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
    /// Verify that the focus has been set correctly for: the scenario:
    /// Root is focusable, has not child, focus on one element and framework application.
    /// </summary>
    /// <description>
    /// This is part of a collection of test case for dev work item: 
    /// Input : Active Window should have focus, when there is no explicit focus
    /// </description>
    /// <author>Microsoft</author>
 
    /// <



    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CoreApplicationFoucusInitialize5 : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", @"Compile and Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", @"Compile and Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in Browser.")]
        [TestCase("2", @"CoreInput\Focus", "Window", @"Compile and Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);
            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "CoreApplicationFoucusInitialize5",
                "Run",
                hostType, null, null);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Focus", "HwndSource", @"Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in HwndSource.")]
        [TestCase("1", @"CoreInput\Focus", "Window", @"Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CoreApplicationFoucusInitialize5(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            CoreLogger.LogStatus("Constructing tree....");

            // Construct test element and child element
            InstrContentPanelHost host = new InstrContentPanelHost();
            host.Focusable = true;
            _frameworkContentElement = new InstrFrameworkContentPanel("rootLeaf", "Sample", host);
            _frameworkContentElement.Focusable = true;
            host.AddChild(_frameworkContentElement);

            // Put the test element on the screen
            DisplayMe(host, 0, 0, 100, 100);

            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            // STEP 0
            CoreLogger.LogStatus("Focusing on the parent....");
            bool bFocus = _rootElement.Focus();
            CoreLogger.LogStatus("Focus set? (expect true) " + bFocus);
            CoreLogger.LogStatus("Startup focused element: " + Keyboard.FocusedElement);

            // STEP 1
            CoreLogger.LogStatus("Saving startup focus values (on startup)....");

            _bWasFocused = _rootElement.IsKeyboardFocused;
            CoreLogger.LogStatus("Root element focused? " + _bWasFocused);

            // STEP 2
            CoreLogger.LogStatus("Focusing on the child....");
            _frameworkContentElement.Focus();

            // STEP 3

            _bIsKeyboardFocused = _rootElement.IsKeyboardFocused;
            CoreLogger.LogStatus("Root element is focused after focus on child? " + _bIsKeyboardFocused);

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

            // Note: for this test we need to make sure the root does not have focus
            // We also need to make sure that an element within it does have focus.

            this.TestPassed = true;


            CoreLogger.LogStatus("In DoValidate, bWasFocused (expect true): " + _bWasFocused);
            if (!_bWasFocused)
            {
                CoreLogger.LogStatus("root element should get the focus when there focus has not been set on any element");
                this.TestPassed = false;
            }

            if (_bIsKeyboardFocused)
            {
                CoreLogger.LogStatus("Focus should have moved away.");
                this.TestPassed = false;
            }
            if (!_frameworkContentElement.IsKeyboardFocused)
            {
                CoreLogger.LogStatus("Focus should have moved to the child.");
                this.TestPassed = false;
            }

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Store content element on our canvas.
        /// </summary>
        private InstrFrameworkContentPanel _frameworkContentElement;

        private bool _bWasFocused;

        private bool _bIsKeyboardFocused = false;

    }
}
