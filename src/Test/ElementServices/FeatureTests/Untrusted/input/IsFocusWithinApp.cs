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
    /// Verify IInputElement IsKeyboardFocusWithin for a UIElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class IsFocusWithinApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", @"Compile and Verify IInputElement IsKeyboardFocusWithin for a UIElement in HwndSource.")]
        [TestCase("0", @"CoreInput\Focus", "Browser", @"Compile and Verify IInputElement IsKeyboardFocusWithin for a UIElement in Browser.")]
        [TestCase("2", @"CoreInput\Focus", "Window", @"Compile and Verify IInputElement IsKeyboardFocusWithin for a UIElement in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "IsFocusWithinApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Focus", "HwndSource", @"Verify IInputElement IsKeyboardFocusWithin for a UIElement in HwndSource.")]
        [TestCase("0", @"CoreInput\Focus", "Window", @"Verify IInputElement IsKeyboardFocusWithin for a UIElement in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new IsFocusWithinApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {


            CoreLogger.LogStatus("Constructing tree....");

            // Construct test element with focusability
            Canvas cvs = new Canvas();
            cvs.Focusable = true;

            _panel = new UIElement[] { new InstrPanel(), new InstrPanel() };

            // first element (source) - we set focus here
            Canvas.SetTop(_panel[0], 0);
            Canvas.SetLeft(_panel[0], 0);

            // second element (target) - we leave focus alone
            Canvas.SetTop(_panel[1], 50);
            Canvas.SetLeft(_panel[1], 50);

            // Put the test element on the screen
            cvs.Children.Add(_panel[0]);
            cvs.Children.Add(_panel[1]);

            DisplayMe(cvs, 0, 0, 100, 100);

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
            CoreLogger.LogStatus("Clearing capture....");
            bool bCaptured = Mouse.Capture(null);
            CoreLogger.LogStatus("Capture remains? (expect false) " + bCaptured);

            CoreLogger.LogStatus("Focusing on the parent....");
            bool bFocus = _rootElement.Focus();
            CoreLogger.LogStatus("Focus set? (expect true) " + bFocus);
            CoreLogger.LogStatus("Startup focused element: " + Keyboard.FocusedElement);

            // STEP 1
            CoreLogger.LogStatus("Saving startup focus values (on startup)....");

            bool bWasFocused = _rootElement.IsKeyboardFocused;
            CoreLogger.LogStatus("Root element focused? (on startup) (expect true) " + bWasFocused);

            bool bWasFocusWithin = ((FrameworkElement)_rootElement).IsKeyboardFocusWithin;
            bool bWasWithinFocusedChild = _panel[0].IsKeyboardFocusWithin;
            bool bWasFocusWithinNonFocusedChild = _panel[1].IsKeyboardFocusWithin;

            CoreLogger.LogStatus("Focus within parent,focusedchild,nonfocusedchild? (on startup) (expect true,false,false) " +
                bWasFocusWithin + "," +
                bWasWithinFocusedChild + "," +
                bWasFocusWithinNonFocusedChild
            );

            _bWasStartupFocusedCorrectly = (bWasFocused) && (bWasFocusWithin) && (!bWasWithinFocusedChild) && (!bWasFocusWithinNonFocusedChild);

            // STEP 2
            CoreLogger.LogStatus("Focusing on the parent....");
            _rootElement.Focus();

            // STEP 3
            CoreLogger.LogStatus("Saving parent focus values (on parent)....");

            bWasFocused = _rootElement.IsKeyboardFocused;
            CoreLogger.LogStatus("Root element focused? (on parent) (expect true) " + bWasFocused);

            bWasFocusWithin = ((FrameworkElement)_rootElement).IsKeyboardFocusWithin;
            bWasWithinFocusedChild = _panel[0].IsKeyboardFocusWithin;
            bWasFocusWithinNonFocusedChild = _panel[1].IsKeyboardFocusWithin;

            CoreLogger.LogStatus("Focus within parent,focusedchild,nonfocusedchild? (on parent) (expect true,false,false) " +
                bWasFocusWithin + "," +
                bWasWithinFocusedChild + "," +
                bWasFocusWithinNonFocusedChild
            );

            _bWasParentFocusedCorrectly = (bWasFocused) && (bWasFocusWithin) && (!bWasWithinFocusedChild) && (!bWasFocusWithinNonFocusedChild);

            // STEP 4
            CoreLogger.LogStatus("Focusing on a panel....");
            KeyboardHelper.EnsureFocus(_panel[0]);

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

            bool bFocused = _rootElement.IsKeyboardFocused;
            CoreLogger.LogStatus("Root element focused? (expect false) " + bFocused);

            bool bFocusWithin = ((FrameworkElement)_rootElement).IsKeyboardFocusWithin;
            bool bFocusWithinFocusedChild = _panel[0].IsKeyboardFocusWithin;
            bool bFocusWithinNonFocusedChild = _panel[1].IsKeyboardFocusWithin;
            CoreLogger.LogStatus("Focus within parent,focusedchild,nonfocusedchild? (expect true,true,false)  " +
                bFocusWithin + "," +
                bFocusWithinFocusedChild + "," +
                bFocusWithinNonFocusedChild
            );

            bool expected = (!bFocused) && (bFocusWithin) && (bFocusWithinFocusedChild) && (!bFocusWithinNonFocusedChild) && (_bWasStartupFocusedCorrectly) && (_bWasParentFocusedCorrectly);
            bool actual = true;
            bool eventFound = (expected == actual);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Store collection of panel elements on our canvas.
        /// </summary>
        private UIElement[] _panel;

        /// <summary>
        /// Were FocusWithin properties set correctly at app startup?
        /// </summary>
        private bool _bWasStartupFocusedCorrectly = false;

        /// <summary>
        /// Were FocusWithin properties set correctly after focus set to parent?
        /// </summary>
        private bool _bWasParentFocusedCorrectly = false;

    }
}
