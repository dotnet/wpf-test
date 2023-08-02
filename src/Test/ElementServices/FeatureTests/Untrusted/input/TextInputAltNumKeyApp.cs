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
    /// Verify TextInput events fire on characters entered via Alt-Numeric keypad keys.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <



    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class TextInputAltNumKeyApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All", "All", "1", @"CoreInput\Keyboard", TestCaseSecurityLevel.FullTrust, "Verify TextInput events fire on characters entered via Alt-Numeric keypad keys")]
        [TestCaseContainerAttribute("TestApplicationStub", "Browser", "1", @"CoreInput\Keyboard", "Verify TextInput events fire on characters entered via Alt-Numeric keypad keys")]
        public void LaunchTest()
        {
            Run();
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test element, add event handling
            _rootElement = new InstrPanel();
            _rootElement.TextInput += new TextCompositionEventHandler(OnText);

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);

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
                    KeyboardHelper.EnsureFocus(_rootElement);
                },                

                delegate
                {
                    Key[] keys1 = {Key.NumPad0, Key.NumPad2,  Key.NumPad4, Key.NumPad8};
                    KeyboardHelper.TypeKey(keys1, ModifierKeys.Alt);
                },

                delegate
                {
                    Key[] keys3 = {Key.NumPad0, Key.NumPad1,  Key.NumPad6, Key.NumPad7};
                    KeyboardHelper.TypeKey(keys3, ModifierKeys.Alt);
                },                

                delegate
                {
                    Key[] keys1 = {Key.NumPad0, Key.NumPad0,  Key.NumPad6, Key.NumPad5};
                    KeyboardHelper.TypeKey(keys1, ModifierKeys.Alt);
                },

                delegate
                {
                    Key[] keys2 = {Key.NumPad0, Key.NumPad0, Key.NumPad0, Key.NumPad9};
                    KeyboardHelper.TypeKey(keys2, ModifierKeys.Alt);
                },                
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

            // Note: for this test we are concerned about whether the event fires and has the proper text in it.

            // We expect string with our character entered: 0248 and 0167 via numeric keypad.
            // 0009 is a legally enterable character, so it too fires an event.

            CoreLogger.LogStatus("Events found (expect 4) : " + _eventLog.Count);
            this.Assert(_eventLog.Count == 4, "Whoops, event count not correct");

            string expected0 = "\xF8"; // 0248
            TextCompositionEventArgs args = _eventLog[0];
            string actual0 = args.Text;
            CoreLogger.LogStatus(" Text: '" + actual0 + "' (expected='" + expected0 + "'");

            string expected1 = "\xA7"; // 0167
            args = _eventLog[1];
            string actual1 = args.Text;
            CoreLogger.LogStatus(" Text: '" + actual1 + "' (expected='" + expected1 + "'");

            string expected2 = "A"; // 0065
            args = _eventLog[2];
            string actual2 = args.Text;
            CoreLogger.LogStatus(" Text: '" + actual2 + "' (expected='" + expected2 + "'");

            string expected3 = "\x09"; // 0009
            args = _eventLog[3];
            string actual3 = args.Text;
            CoreLogger.LogStatus(" Text: '" + actual3 + "' (expected='" + expected3 + "'");

            this.Assert((actual0 == expected0) && (actual1 == expected1) && (actual2 == expected2) && (actual3 == expected3), "Unexpected text mismatch");

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
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: " + e.Text + (e.Text.Length > 0 ? (" [" + (uint)(e.Text[0]) + "]") : ""));

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<TextCompositionEventArgs> _eventLog = new List<TextCompositionEventArgs>();
    }
}
