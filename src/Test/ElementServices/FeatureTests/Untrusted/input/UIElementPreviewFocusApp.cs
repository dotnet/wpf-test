// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
    /// Verify UIElement PreviewGotKeyboardFocus works for element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementPreviewFocusApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", @"Compile and Verify UIElement PreviewGotKeyboardFocus works for element in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", @"Compile and Verify UIElement PreviewGotKeyboardFocus works for element in Browser.")]
        [TestCase("2", @"CoreInput\Focus", "Window", @"Compile and Verify UIElement PreviewGotKeyboardFocus works for element in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "UIElementPreviewFocusApp",
                "Run",
                hostType, null, null);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Focus", "HwndSource", @"Verify UIElement PreviewGotKeyboardFocus works for element in HwndSource.")]
        [TestCase("1", @"CoreInput\Focus", "Window", @"Verify UIElement PreviewGotKeyboardFocus works for element in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementPreviewFocusApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing test element....");
            _rootElement = new InstrPanel();

            CoreLogger.LogStatus("Adding event handlers....");
            _rootElement.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            _rootElement.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnPreviewFocus);

            // Put the test element on the screen
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
            _baselineEventLogCount = _eventLog.Count;
            CoreLogger.LogStatus("Events raised so far: "+_baselineEventLogCount);
            
            CoreLogger.LogStatus("Setting focus to the element....");
            _rootElement.Focusable = true;
            _bFocusAPI = _rootElement.Focus();

            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Stores result of Focus API call.
        /// </summary>
        private bool _bFocusAPI;

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            CoreLogger.LogStatus("Focus set via API?           (expect no) " + (_bFocusAPI));
            bool bFocusIM = InputHelper.GetFocusedElement() != _rootElement;
            CoreLogger.LogStatus("Element in focus IM: (expect non-root El) '" + InputHelper.GetFocusedElement() + "'");
            CoreLogger.LogStatus("Focus set via InputManager?  (expect true) " + (bFocusIM));

            int nExecutedEventHandlers = _eventLog.Count - _baselineEventLogCount;
            CoreLogger.LogStatus("Additional events raised: (expect 1) " + nExecutedEventHandlers);

            // expect non-negative event count
            bool actual = !_bFocusAPI && bFocusIM && (nExecutedEventHandlers == 1);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello focusing from: " + e.OldFocus + " to " + e.NewFocus);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Standard preview focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnPreviewFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello focusing from: " + e.OldFocus + " to " + e.NewFocus);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<KeyboardFocusChangedEventArgs> _eventLog = new List<KeyboardFocusChangedEventArgs>();

        /// <summary>
        /// Store count of our fired events before starting test execution, as a baseline to measure events raised during the test.
        /// </summary>
        private int _baselineEventLogCount = 0;
        
    }
}
