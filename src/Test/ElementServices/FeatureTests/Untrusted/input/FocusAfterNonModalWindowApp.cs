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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.Threading;
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
    /// Verify Keyboard Focus returns to focused element after dismissing modeless window.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <




    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FocusAfterNonModalWindowApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Focus", "HwndSource", @"Compile and Verify Keyboard Focus returns to focused element after dismissing modeless window in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", @"1", @"Compile and Verify Keyboard Focus returns to focused element after dismissing modeless window in Browser.")]
        [TestCase("3", @"CoreInput\Focus", "Window", @"Compile and Verify Keyboard Focus returns to focused element after dismissing modeless window in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FocusAfterNonModalWindowApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", @"Verify Keyboard Focus returns to focused element after dismissing modeless window in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Window", @"Verify Keyboard Focus returns to focused element after dismissing modeless window in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FocusAfterNonModalWindowApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");
            Canvas[] canvases = new Canvas[] { new Canvas() };
            foreach (Canvas cvs in canvases)
            {
                // Construct test element, add event handling

                // Focusable panel 1
                InstrFrameworkPanel panel = new InstrFrameworkPanel();
                panel.Focusable = true;
                panel.Name = "nOnLostKeyboardFocusbtn" + DateTime.Now.Ticks;
                Canvas.SetTop(panel, 0);
                Canvas.SetLeft(panel, 0);
                panel.Height = 40;
                panel.Width = 40;

                // Focusable panel 2 - with events attached
                InstrFrameworkPanel panel2 = new InstrFrameworkPanel();
                panel2.Focusable = true;
                panel2.Name = "focusbtn" + DateTime.Now.Ticks;
                Canvas.SetTop(panel2, 50);
                Canvas.SetLeft(panel2, 50);
                panel2.Height = 40;
                panel2.Width = 40;
                panel2.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnLostKeyboardFocus);
                panel2.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus);

                cvs.Children.Add(panel);
                cvs.Children.Add(panel2);
            }

            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 100, 100);

            DispatcherHelper.DoEvents(DispatcherPriority.Input);

            // With test element on the screen, set focus and then show our window

            Debug.Assert(canvases[0].Children.Count == 2, "Canvas children not added correctly");
            CoreLogger.LogStatus("Setting focus to all our elements (leaving focus on our target element)...");
            bool bFocus1 = canvases[0].Children[0].Focus();
            bool bFocus2 = canvases[0].Children[1].Focus();
            CoreLogger.LogStatus("Did focus succeed on first element? " + bFocus1.ToString());
            CoreLogger.LogStatus("Did focus succeed on second element? " + bFocus2.ToString());

            IntPtr hwndFocus = NativeMethods.GetFocus();
            CoreLogger.LogStatus("Hwnd with Focus? " + hwndFocus.ToString());

            CoreLogger.LogStatus("Popping up NonModalWindow...");
            _nonModalSurface = TestContainer.DisplayObject(new InstrFrameworkPanel(), 250, 50, 200, 200);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Destroying old window...");
            _nonModalSurface.Close();

            CoreLogger.LogStatus("Switching to our window (simulate Alt+Tab)...");
            NativeMethods.SetForegroundWindow(_hwnd);

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

            // For this test we need 3 focus events to fire on our target element.
            // 1 on setup, 1 lose-focus on showing new window, 1 get-focus on switching to old window.
            CoreLogger.LogStatus("Events found: " + _eventLog.Count);

            bool actual = (_eventLog.Count == 3);
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
        /// <param name="args">Event-specific arguments.</param>
        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello focusing from: '" + args.OldFocus + "' to '" + args.NewFocus + "'");

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   HEY BABY - Hello focusing from: '" + args.OldFocus + "' to '" + args.NewFocus + "'");

            // Don't route this event any more.
            args.Handled = true;
        }


        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<KeyboardFocusChangedEventArgs> _eventLog = new List<KeyboardFocusChangedEventArgs>();

        /// <summary>
        /// Store reference to our test window.
        /// </summary>
        private Surface _nonModalSurface = null;
    }
}
