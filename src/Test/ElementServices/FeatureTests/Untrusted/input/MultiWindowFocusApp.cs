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
    /// Verify focus works for elements in multiple windows.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MultiWindowFocusApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainer("All","All","1", @"CoreInput\Focus", TestCaseSecurityLevel.FullTrust, @"Verify focus works for elements in multiple windows")]
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
            CoreLogger.LogStatus("Constructing tree....");

            Canvas[] canvases = new Canvas[] { new Canvas(), new Canvas() };
            foreach (Canvas cvs in canvases)
            {
                FrameworkElement panel = new InstrFrameworkPanel();
                panel.Focusable = true;
                panel.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                panel.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                Canvas.SetTop(panel, 0);
                Canvas.SetLeft(panel, 0);
                panel.Height = 40;
                panel.Width = 40;

                FrameworkElement panel2 = new InstrFrameworkPanel();
                panel2.Focusable = true;
                panel2.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                panel2.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                Canvas.SetTop(panel2, 50);
                Canvas.SetLeft(panel2, 50);
                panel2.Height = 40;
                panel2.Width = 40;

                cvs.Children.Add(panel);
                cvs.Children.Add(panel2);

                _controlCollection.Add(panel);
            }
            ((FrameworkElement)(canvases[0].Children[0])).Name = "btnWindowA";

            ((FrameworkElement)(canvases[1].Children[0])).Name = "btnWindowB";

            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 100, 100);

            DisplayMe(canvases[1], 125, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            FrameworkElement[] btns = new FrameworkElement[] { _controlCollection[0] as FrameworkElement, _controlCollection[1] as FrameworkElement };

            CoreLogger.LogStatus("Focusing on button A...");
            btns[0].Focus();
            CoreLogger.LogStatus("Focusing on button B...");
            btns[1].Focus();

            CoreLogger.LogStatus("Getting ready to verify focus...");

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

            this.TestPassed = false;

            // We want our element to have fired a particular event.
            // Only the mouse leave event triggered by the animation
            // should be in the log when it has completed.
            CoreLogger.LogStatus("Event log: (expect 3) " + _eventLog.Count);
            Assert(_eventLog.Count == 3, "Event count not correct (expected 3)");

            // Log final test results.
            this.TestPassed = true;
            CoreLogger.LogStatus("Test pass status? " + TestPassed);

            TestContainer.CurrentSurface[1].Close();

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
            string senderID = (sender is FrameworkElement) ? ((FrameworkElement)sender).Name : "";

            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] [" + senderID + "]");

            string oldFocus = ((e.OldFocus != null) ? ((FrameworkElement)e.OldFocus).Name : ("(null)"));
            string newFocus = ((e.NewFocus != null) ? ((FrameworkElement)e.NewFocus).Name : ("(null)"));

            CoreLogger.LogStatus("   Hello focusing from: '" + oldFocus + "' to '" + newFocus + "'");

            // Don't route this event any more.
            e.Handled = true;
        }

        private List<KeyboardFocusChangedEventArgs> _eventLog = new List<KeyboardFocusChangedEventArgs>();

        private List<FrameworkElement> _controlCollection = new List<FrameworkElement>();
    }
}

