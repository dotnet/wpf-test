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
using System.Windows.Navigation;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify focus is regained after showing WinForms message box.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FocusAfterWinFormsMessageBoxApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify focus is regained after showing WinForms message box in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify focus is regained after showing WinForms message box in Browser.")]
        [TestCase("3", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify focus is regained after showing WinForms message box in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FocusAfterWinFormsMessageBoxApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify focus is regained after showing WinForms message box in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Verify focus is regained after showing WinForms message box in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FocusAfterWinFormsMessageBoxApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            InstrFrameworkPanel[] canvases = new InstrFrameworkPanel[] { new InstrFrameworkPanel() };

            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 100, 100);

            KeyboardHelper.EnsureFocus(canvases[0]);

            CoreLogger.LogStatus("Adding event handlers....");
            canvases[0].GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            canvases[0].LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {

            CoreLogger.LogStatus("Focusing....");

            CoreLogger.LogStatus("Focused element: (should be our panel) " + InputHelper.GetFocusedElement());

            CoreLogger.LogStatus("Showing message box....");
            AutoCloseMessageBox mb = new AutoCloseWinFormsMessageBox(5000, "Wait 5 seconds for box to close", "Wait");
            mb.Show();
            CoreLogger.LogStatus("Shown message box. Getting ready to verify....");

            DispatcherHelper.DoEvents();

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

            CoreLogger.LogStatus("Focus events found: (expect 4) " + _eventLog.Count);
            Assert(_eventLog.Count == 4, "Oh no - incorrect number of events");

            CoreLogger.LogStatus("Focused element: (should be our panel) " + InputHelper.GetFocusedElement());
            Assert(InputHelper.GetFocusedElement() == _rootElement, "Oh no - focus not correct");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

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
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]  [" + sender.ToString() + "]");
            CoreLogger.LogStatus("   Hello focusing from: '" + e.OldFocus + "' to '" + e.NewFocus + "'");

            // Don't route this event any more.
            e.Handled = true;
        }

        private List<KeyboardFocusChangedEventArgs> _eventLog = new List<KeyboardFocusChangedEventArgs>();
    }
}

