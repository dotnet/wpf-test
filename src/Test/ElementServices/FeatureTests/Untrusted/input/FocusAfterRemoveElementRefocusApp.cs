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
    /// Verify focus is lost and regained after removing element with keyboard focus, then refocusing to element in tree.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FocusAfterRemoveElementRefocusApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify focus is lost and regained after removing element with keyboard focus, then refocusing to element in tree in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify focus is lost and regained after removing element with keyboard focus, then refocusing to element in tree in Browser.")]
        [TestCase("3", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify focus is lost and regained after removing element with keyboard focus, then refocusing to element in tree in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FocusAfterRemoveElementRefocusApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify focus is lost and regained after removing element with keyboard focus, then refocusing to element in tree in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Window", TestCaseSecurityLevel.FullTrust, @"Verify focus is lost and regained after removing element with keyboard focus, then refocusing to element in tree in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FocusAfterRemoveElementRefocusApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            Canvas[] canvases = new Canvas[] { new Canvas() };
            foreach (Canvas cvs in canvases)
            {
                FrameworkElement panel = new InstrFrameworkPanel();

                panel.Focusable = true;
                panel.Name = "nonfocusbtn" + DateTime.Now.Ticks;
                panel.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                panel.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                Canvas.SetTop (panel, 0);
                Canvas.SetLeft (panel, 0);
                panel.Height = 40;
                panel.Width = 40;

                FrameworkElement panel2 = new InstrFrameworkPanel();

                panel2.Focusable = true;
                panel2.Name = "focusbtn" + DateTime.Now.Ticks;
                panel2.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                panel2.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                Canvas.SetTop (panel2, 50);
                Canvas.SetLeft (panel2, 50);
                panel2.Height = 40;
                panel2.Width = 40;
                panel2.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocusStateChange);
                
                cvs.Children.Add(panel);
                cvs.Children.Add(panel2);

            }
            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 100, 100);

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

            Canvas cvs = (Canvas)(_rootElement);
            // focus on element 1
            cvs.Children[0].Focus();
            // focus on element 2
            cvs.Children[1].Focus();

            Assert(Keyboard.FocusedElement == cvs.Children[1], "Focus failed");

            FrameworkElement e2 = (FrameworkElement)(cvs.Children[1]);
            // zap element 2
            cvs.Children.Remove(e2);

            Keyboard.Focus(null);

            // root element has focus
            CoreLogger.LogStatus("Keyboard focused: " + Keyboard.FocusedElement);
            Assert(Keyboard.FocusedElement != cvs.Children[0], "Focus failed (expect non-removed element)");

            Canvas par = cvs;
            CoreLogger.LogStatus("Children remaining: " + par.Children.Count);
            Assert(par.Children.Count == 1, "Element removal failed (expect 1 of 2 children left)");

            // Refocus
            par.Children[0].Focus();

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

            Canvas cvs = (Canvas)(_rootElement);

            // 3 focus events, 2 lose focus events
            CoreLogger.LogStatus("Event log: (expect 5) " + _eventLog.Count);
            Assert(_eventLog.Count == 5, "Wrong number of focus events (expect 5)");

            // first element has focus
            CoreLogger.LogStatus("Keyboard focused: " + Keyboard.FocusedElement);
            Assert(Keyboard.FocusedElement == cvs.Children[0], "Focus failed (expect child element)");

            // first element is targeted
            CoreLogger.LogStatus("Keyboard target: " + Keyboard.PrimaryDevice.Target);
            Assert(Keyboard.PrimaryDevice.Target == cvs.Children[0], "Target failed (expect child element)");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

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
            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"]");
            CoreLogger.LogStatus("   Hello focusing from: '" + args.OldFocus + "' to '"+ args.NewFocus + "'");

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnFocusStateChange(object sender, KeyboardFocusChangedEventArgs args)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   HEY BABY - Hello focusing from: '" + args.OldFocus + "' to '" + args.NewFocus + "'");

            // Don't route this event any more.
            args.Handled = true;
        }

        private List<KeyboardFocusChangedEventArgs> _eventLog = new List<KeyboardFocusChangedEventArgs>();
    }
}

