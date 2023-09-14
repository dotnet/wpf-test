// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
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
    /// Verify focus and ActiveElement works for elements in multiple windows.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <





    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MultiWindowFocusActiveElementApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainer("All", "All", "1", @"CoreInput\Focus", "", @"1", TestCaseSecurityLevel.FullTrust, @"Verify focus and ActiveElement works for elements in multiple windows")]
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
                Control panel = new Button();
                panel.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                panel.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                Canvas.SetTop (panel, 0);
                Canvas.SetLeft (panel, 0);
                panel.Height = 40;
                panel.Width = 40;

                cvs.Children.Add(panel);
                FocusManager.SetFocusedElement(cvs, (IInputElement)panel);

                _controlCollection.Add(panel);
            }
            ((FrameworkElement)_controlCollection[0]).Name = "btnWindowA";
            ((FrameworkElement)_controlCollection[1]).Name = "btnWindowB";

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

            CoreLogger.LogStatus("Event log (expect 5): " + _eventLog.Count);
            Assert(_eventLog.Count == 5, "Unexpected number of events");

            this.TestPassed = true;
            TestContainer.CurrentSurface[1].Close();
            CoreLogger.LogStatus("Test pass status? " + TestPassed);

            return null;
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            string senderID = (sender is FrameworkElement) ? ((FrameworkElement)sender).Name : "";

            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"] [" + senderID + "]");

            string oldFocus = ((args.OldFocus != null) ? ((FrameworkElement)args.OldFocus).Name : ("(null)"));
            string newFocus = ((args.NewFocus != null) ? ((FrameworkElement)args.NewFocus).Name : ("(null)"));

            CoreLogger.LogStatus("   Hello focusing from: '" + oldFocus + "' to '"+ newFocus + "'");

            // Don't route this event any more.
            args.Handled = true;
        }

        private ArrayList _eventLog = new ArrayList();

        private ArrayList _controlCollection = new ArrayList();
    }
}

