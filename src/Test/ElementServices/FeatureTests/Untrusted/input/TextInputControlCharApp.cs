// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify TextComposition events fire on Control-letter keys.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class TextInputControlCharApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All", "All", "1", @"CoreInput\Keyboard", TestCaseSecurityLevel.FullTrust, "Verify TextComposition events fire on Control-letter keys")]
        [TestCaseContainerAttribute("TestApplicationStub", "Browser", "1", @"CoreInput\Keyboard", "", "0", TestCaseSecurityLevel.PartialTrust, "Verify TextComposition events fire on Control-letter keys")]
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

            // Construct test element
            Canvas cvs = new Canvas();
            InstrFrameworkPanel panel = new InstrFrameworkPanel();
            panel.Focusable = true;
            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);
            panel.Height = 40;
            panel.Width = 40;
            cvs.Children.Add(panel);

            // Add event handling
            panel.KeyDown += new KeyEventHandler(OnKey);
            panel.PreviewTextInput += new TextCompositionEventHandler(OnPreviewText);
            panel.TextInput += new TextCompositionEventHandler(OnText);

            // Put the test element on the screen
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }


        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            // Enter Ctrl-Enter via keyboard

            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    DoBeforeExecute();
                },

                delegate
                {
                    KeyboardHelper.TypeKey(Key.Enter, ModifierKeys.Control);
                },
            };
            return ops;
        }

        /// <summary>
        /// Execute stuff right before the test operations.
        /// </summary>
        private void DoBeforeExecute()
        {
            CoreLogger.LogStatus("Setting focus to the element....");
            Canvas cvs = (Canvas)_rootElement;
            bool bFocus = cvs.Children[0].Focus();
            CoreLogger.LogStatus("Focus set via API?           " + (bFocus));
            CoreLogger.LogStatus("Focus set via InputManager?  " + (InputHelper.GetFocusedElement() != null));

            // Now our TestOps will fire....
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about whether text is received when pressing Ctrl+letter key.

            // We expect 2 KeyDown events, but one Text events. ControlText is set.

            CoreLogger.LogStatus("key Events found: (expect 2)  " + _keyEventLog.Count);
            CoreLogger.LogStatus("text Events found: (expect 1) " + _textCompositionEventLog.Count);
            if (_textCompositionEventLog.Count > 0)
            {
                CoreLogger.LogStatus("text Event text: (expect '') " + _textCompositionEventLog[0].Text);
                CoreLogger.LogStatus("text Event control text: (expect character 10) " + _textCompositionEventLog[0].ControlText);
            }
            bool stringFound = (_textCompositionEventLog.Count == 1) &&
                (_keyEventLog.Count == 2) &&
                (_textCompositionEventLog[0].ControlText == "\u000a");

            CoreLogger.LogStatus("Setting log result to " + stringFound);
            this.TestPassed = stringFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard preview text event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnPreviewText(object sender, TextCompositionEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]  [" + sender.ToString() + "]");
            CoreLogger.LogStatus("   Hello from: " + String.Format("{0:X}", e.Text));

            // Continue routing this event.
            e.Handled = false;
        }

        /// <summary>
        /// Standard text event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnText(object sender, TextCompositionEventArgs e)
        {
            // Set test flag
            _textCompositionEventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]  [" + sender.ToString() + "]");
            CoreLogger.LogStatus("   Hello from: " + String.Format("{0:X}", e.Text));

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
            _keyEventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]  [" + sender.ToString() + "]");
            CoreLogger.LogStatus("   Hello from: '" + e.Key.ToString() + "', " + e.KeyStates.ToString());

            // Continue routing this event.
            e.Handled = false;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<KeyEventArgs> _keyEventLog = new List<KeyEventArgs>();

        private List<TextCompositionEventArgs> _textCompositionEventLog = new List<TextCompositionEventArgs>();
    }
}
