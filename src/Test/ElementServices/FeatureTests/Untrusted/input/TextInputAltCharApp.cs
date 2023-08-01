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
    /// Verify TextInput events don't fire on Alt-letter keys.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class TextInputAltCharApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify TextInput events don't fire on Alt-letter keys in HwndSource.")]
        [TestCase("3", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify TextInput events don't fire on Alt-letter keys in window.")]
        [TestCaseTimeout(@"120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "TextInputAltCharApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify TextInput events don't fire on Alt-letter keys in HwndSource.")]
        [TestCase("2", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Verify TextInput events don't fire on Alt-letter keys in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new TextInputAltCharApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            CoreLogger.LogStatus("Constructing window....");

            // Construct test element, add event handling
            _rootElement = new InstrPanel();
            _rootElement.KeyDown += new KeyEventHandler(OnKey);
            _rootElement.TextInput += new TextCompositionEventHandler(OnText);

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);


            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            // Enter Alt-A via keyboard

            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {                    
                    KeyboardHelper.EnsureFocus(_rootElement);
                },
                delegate
                {
                    Key[] keys1 = {Key.A};
                    KeyboardHelper.TypeKey(keys1, ModifierKeys.Alt);
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

            // Note: for this test we are concerned about whether text is received when pressing Alt+letter key.

            // We expect KeyDown events, but no Text events. 2, not 3!

            CoreLogger.LogStatus("key Events found (expect 2):  " + _eventLog.Count);
            CoreLogger.LogStatus("text Events found (expect 0): " + _textEventLog.Count);

            this.Assert(_eventLog.Count == 2, "Key event count not correct");
            this.Assert(_textEventLog.Count == 0, "Key event count not correct");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard text event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnText(object sender, TextCompositionEventArgs e)
        {
            // Set test flag
            _textEventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: '" + e.Text + "'");

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnKey(object sender, KeyEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: '" + e.Key.ToString() + "', " + e.KeyStates.ToString());

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<KeyEventArgs> _eventLog = new List<KeyEventArgs>();

        private List<TextCompositionEventArgs> _textEventLog = new List<TextCompositionEventArgs>();
    }
}
