// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify CommandManager StatusInvalidated works for element on key input.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <remarks>
    /// This test app works via command-enabling a regular element during the setup phase.
    /// A more typical scenario is to just use a command-aware element (usually a control) instead of a regular element.
    /// </remarks>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CommandManagerSuggestRequeryApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\Interfaces", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify CommandManager StatusInvalidated works for element on key input in HwndSource.")]
        [TestCase("1", @"Commanding\Interfaces", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify CommandManager StatusInvalidated works for element on key input in Browser.")]
        [TestCase("3", @"Commanding\Interfaces", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify CommandManager StatusInvalidated works for element on key input in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "CommandManagerSuggestRequeryApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\Interfaces", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify CommandManager StatusInvalidated works for element on key input in HwndSource.")]
        [TestCase("2", @"Commanding\Interfaces", "Window", TestCaseSecurityLevel.FullTrust, @"Verify CommandManager StatusInvalidated works for element on key input in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            TestApp app = new CommandManagerSuggestRequeryApp();
            exe.Run(app, "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing root element....");
            InstrPanel panel = new InstrPanel();

            CoreLogger.LogStatus("Attaching input events...");
            panel.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            panel.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            panel.KeyDown += new KeyEventHandler(OnKey);
            panel.KeyUp += new KeyEventHandler(OnKey);
            panel.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);
            panel.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseButton);

            // Put the test element on the screen
            CoreLogger.LogStatus("Showing window...");
            DisplayMe(panel, 10, 10, 100, 100);

            CoreLogger.LogStatus("Attaching CommandManager events...");
            _requerySuggested = new EventHandler(OnStatusInvalidated);
            CommandManager.RequerySuggested += _requerySuggested;

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object sender)
        {
            KeyboardHelper.EnsureFocus(_rootElement);

            CoreLogger.LogStatus("Invoking key down ...");
            KeyboardHelper.TypeKey(Key.P);

            CoreLogger.LogStatus("Invoking Click ...");
            MouseHelper.Click(_rootElement);

            CoreLogger.LogStatus("Suggesting another requery...");
            CommandManager.InvalidateRequerySuggested();

            CoreLogger.LogStatus("Draining queue...");
            DispatcherHelper.DoEvents(DispatcherPriority.ApplicationIdle);
            CoreLogger.LogStatus("Queue drained. Getting ready to validate...");

            base.DoExecute(sender);
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

            CoreLogger.LogStatus("StatusInvalidated events (expect 3): " + _queryLog.Count);
            this.Assert(_queryLog.Count == 3, "Incorrect number of StatusInvalidated events");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// If we are in this handler, someone is suggesting a requery!
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the event.</param>
        private void OnStatusInvalidated(object sender, EventArgs e)
        {
            CoreLogger.LogStatus(" [StatusInvalidated]");
            if (sender != null)
            {
                CoreLogger.LogStatus("  Status Invalidated sender Name: " + sender.ToString());
            }

            _queryLog.Add(e);
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello focusing from: '" + e.OldFocus + "' to '" + e.NewFocus + "'");
        }

        /// <summary>
        /// Standard mouse button event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("   Btn=" + e.ChangedButton.ToString() + ",State=" + e.ButtonState.ToString() + ",ClickCount=" + e.ClickCount);
        }

        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnKey(object sender, KeyEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: '" + e.Key.ToString() + "', " + e.KeyStates.ToString());
        }

        private List<EventArgs> _queryLog = new List<EventArgs>();
        private EventHandler _requerySuggested;
    }
}
