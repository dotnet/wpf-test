// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
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
    /// Verify ContentElement Focus works for element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementFocusApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Focus","HwndSource",@"Compile and Verify ContentElement Focus works for element in HwndSource.")]
        [TestCase("1",@"CoreInput\Focus","Browser",@"Compile and Verify ContentElement Focus works for element in Browser.")]
        [TestCase("3",@"CoreInput\Focus","Window",@"Compile and Verify ContentElement Focus works for element in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "ContentElementFocusApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Focus","HwndSource",@"Verify ContentElement Focus works for element in HwndSource.")]
        [TestCase("2",@"CoreInput\Focus","Window",@"and Verify ContentElement Focus works for element in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementFocusApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {


            CoreLogger.LogStatus("Constructing tree....");
            
            {
                // Construct test element
                InstrContentPanelHost host = new InstrContentPanelHost();

                // Construct child element
                _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);

                // Add event handling
                _contentElement.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                _contentElement.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                _contentElement.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnPreviewFocus);
                _contentElement.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnPreviewFocus);
                host.AddChild(_contentElement);

                DisplayMe(host,10, 10, 100, 100);

            }
            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg) 
        {
            CoreLogger.LogStatus("Setting focus to the element....");

            _contentElement.Focusable = true;
            _bFocusAPI = _contentElement.Focus();
            
            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Stores result of Focus API call.
        /// </summary>
        private bool _bFocusAPI;

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender) 
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need 2 focus measurements to BOTH be true and to match each other.
            // We also need a GotKeyboardFocus event and its matching unhandled Preview.
            
            CoreLogger.LogStatus("Focus set via API?           " + (_bFocusAPI));
            bool bFocusIM = InputHelper.GetFocusedElement()!=null;
            CoreLogger.LogStatus("Focus set via InputManager?  " + (bFocusIM));
            CoreLogger.LogStatus("Events found: "+_eventLog.Count);
            
            // expect non-negative event count
            bool actual = _bFocusAPI && bFocusIM && (_eventLog.Count==2);
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
        private void OnFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Debug.WriteLine ("   Hello focusing from: " + args.OldFocus + " to "+ args.NewFocus);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Standard preview focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnPreviewFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Debug.WriteLine ("   Hello focusing from: " + args.OldFocus + " to "+ args.NewFocus);

            // Continue routing this event (no cancel on preview)
            args.Handled = false;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();

        private InstrContentPanel _contentElement;
    }
}
