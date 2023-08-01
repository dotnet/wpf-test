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
    /// Verify Text event fires on a non-alphabetic-character Shift-key down.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class TextInputShiftCharApp : TestApp
    {


        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All", "All", "1", @"CoreInput\Keyboard", TestCaseSecurityLevel.FullTrust, "Verify Text event fires on a non-alphabetic-character Shift-key down")]
        [TestCaseContainerAttribute("TestApplicationStub", "Browser", "1", @"CoreInput\Keyboard", "Verify Text event fires on a non-alphabetic-character Shift-key down")]
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
            CoreLogger.LogStatus("Constructing window....");

            // Construct test element, add event handling
            _rootElement = new InstrPanel();
            _rootElement.TextInput += new TextCompositionEventHandler(OnText);
            _rootElement.PreviewTextInput += new TextCompositionEventHandler(OnPreviewText);

            InputMethod.SetPreferredImeState(_rootElement, InputMethodState.Off);

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
            // Enter Shift-1 via keyboard

            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    KeyboardHelper.EnsureFocus(_rootElement);
                },

              
                delegate
                {
                    KeyboardHelper.TypeKey(Key.D1, ModifierKeys.Shift);
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

            // Note: for this test we are concerned about whether the event fires exactly once.
            // We are also concerned that the exact text is entered.

            CoreLogger.LogStatus("text Events found: (expect 1) " + _textEventLog.Count);
            CoreLogger.LogStatus("previewtext Events found: (expect 1) " + _previewTextEventLog.Count);
            this.Assert(_textEventLog.Count == 1, "Incorrect text Events found");
            this.Assert(_previewTextEventLog.Count == 1, "Incorrect previewtext Events found");

            TextCompositionEventArgs e = _textEventLog[0];
            string argsText = e.Text;
            CoreLogger.LogStatus("Text found: (expect '!') " + argsText + "'");
            this.Assert(argsText == "!", "Incorrect text found");

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
            CoreLogger.LogStatus("   Hello from: " + e.Text);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Standard preview text event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnPreviewText(object sender, TextCompositionEventArgs e)
        {
            // Set test flag
            _previewTextEventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: " + e.Text);

            // Continue routing this event (don't block it)
            e.Handled = false;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<TextCompositionEventArgs> _textEventLog = new List<TextCompositionEventArgs>();

        private List<TextCompositionEventArgs> _previewTextEventLog = new List<TextCompositionEventArgs>();

    }
}
