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
    /// Verify KeyDown event fires on a key down for ContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementKeyDownApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify KeyDown event fires on a key down for ContentElement in HwndSource.")]
        [TestCase("1", @"CoreInput\Keyboard", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify KeyDown event fires on a key down for ContentElement in Browser.")]
        [TestCase("3", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify KeyDown event fires on a key down for ContentElement in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "ContentElementKeyDownApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify KeyDown event fires on a key down for ContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Verify KeyDown event fires on a key down for ContentElement in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementKeyDownApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test element and child element
            InstrContentPanelHost host = new InstrContentPanelHost();

            _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);

            // Add event handling
            _contentElement.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);
            _contentElement.KeyDown += new KeyEventHandler(OnKeyDown);
            host.AddChild(_contentElement);

            DisplayMe(host, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    KeyboardHelper.EnsureFocus(_contentElement);
                },               
                delegate
                {
                    KeyboardHelper.TypeKey(Key.F2);
                }
            };
            return ops;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about whether both preview and regular events are fired.
            // Hence, we are concerned if 2 events are found.

            CoreLogger.LogStatus("Events found: (expect 2) " + _eventLog.Count);
            this.Assert(_eventLog.Count == 2, "Incorrect number of events found");
            
            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: " + e.Key.ToString() + ", " + e.KeyStates.ToString());

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Standard preview key event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: " + e.Key.ToString() + ", " + e.KeyStates.ToString());

            // Keep routing this event.
            e.Handled = false;
        }


        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<KeyEventArgs> _eventLog = new List<KeyEventArgs>();

        private InstrContentPanel _contentElement;
    }
}
