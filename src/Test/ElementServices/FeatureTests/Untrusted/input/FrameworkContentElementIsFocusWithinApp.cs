// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Threading;
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
using System.Windows.Threading;
using Microsoft.Test.Threading;
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
    /// Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkContentElementIsFocusWithinApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in Browser.")]
        [TestCase("2", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FrameworkContentElementIsFocusWithinApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in HwndSource.")]
        [TestCase("1", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Verify IFrameworkInputElement IsKeyboardFocusWithin for a FrameworkContentElement in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkContentElementIsFocusWithinApp(), "Run");

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
            host.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButtonDown);
            _frameworkContentElement = new InstrFrameworkContentPanel("rootLeaf", "Sample", host);
            host.AddChild(_frameworkContentElement);

            // Put the test element on the screen
            DisplayMe(host, 0, 0, 100, 100);

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
            // Position mouse for test, release capture.

            CoreLogger.LogStatus("Moving mouse to target and clicking...");
            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);
            MouseHelper.Click(_rootElement);
            CoreLogger.LogStatus("Move and click complete.");

            CoreLogger.LogStatus("Clearing capture....");
            bool bCaptured = Mouse.Capture(null);
            CoreLogger.LogStatus("Capture remains? (expect false) " + bCaptured);

            // STEP 1
            CoreLogger.LogStatus("Focusing on the parent....");
            _rootElement.Focus();

            // STEP 2
            CoreLogger.LogStatus("Saving parent focus values (on parent)....");

            bool bWasFocused = _rootElement.IsKeyboardFocused;
            CoreLogger.LogStatus("Root element focused? (expect yes) " + bWasFocused);

            bool bWasFocusWithin = ((FrameworkElement)_rootElement).IsKeyboardFocusWithin;
            bool bWasFocusWithinFocusedChild = _frameworkContentElement.IsKeyboardFocusWithin;
            CoreLogger.LogStatus("Focus within parent,focusedcontent (expect yes,no) " +
                bWasFocusWithin + "," +
                bWasFocusWithinFocusedChild
            );

            _bWasParentFocusedCorrectly = (bWasFocused) && (bWasFocusWithin) && (!bWasFocusWithinFocusedChild);

            // STEP 3
            CoreLogger.LogStatus("Focusing on a content element....");
            _frameworkContentElement.Focusable = true;
            bool bResult = _frameworkContentElement.Focus();
            CoreLogger.LogStatus("Content element focused? " + bResult);

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
            CoreLogger.LogStatus("Root element focused? (expect no) " + bFocused);

            bool bFocusWithin = ((FrameworkElement)_rootElement).IsKeyboardFocusWithin;
            bool bFocusWithinFocusedChild = _frameworkContentElement.IsKeyboardFocusWithin;
            CoreLogger.LogStatus("Focus within parent,focusedcontent (expect yes,yes) " +
                bFocusWithin + "," +
                bFocusWithinFocusedChild
            );

            bool expected = (!bFocused) && (bFocusWithin) && (bFocusWithinFocusedChild) && (_bWasParentFocusedCorrectly);
            bool actual = true;
            bool eventFound = (expected == actual);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Event handler to run when mouse is down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            // Focus canvas.
            bool bFocus = ((UIElement)sender).Focus();
            CoreLogger.LogStatus("  First element focused? " + bFocus);

            e.Handled = true;
        }

        /// <summary>
        /// Store content element on our canvas.
        /// </summary>
        private InstrFrameworkContentPanel _frameworkContentElement;

        /// <summary>
        /// Were FocusWithin properties set correctly after focus set to parent?
        /// </summary>
        private bool _bWasParentFocusedCorrectly = false;

    }
}
