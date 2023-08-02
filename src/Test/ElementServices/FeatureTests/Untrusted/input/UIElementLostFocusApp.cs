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
using System.Windows.Controls;
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
    /// Verify UIElement LostKeyboardFocus works to cancel focus element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementLostFocusApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Focus","HwndSource", TestCaseSecurityLevel.FullTrust,@"Verify UIElement LostKeyboardFocus works to cancel focus element in hwndsource.")]
        [TestCase("1",@"CoreInput\Focus","Browser", @"Verify UIElement LostKeyboardFocus works to cancel focus element in browser.")]
        [TestCase("3",@"CoreInput\Focus","Window", TestCaseSecurityLevel.FullTrust,@"Verify UIElement LostKeyboardFocus works to cancel focus element in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "UIElementLostFocusApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Keyboard","HwndSource",  TestCaseSecurityLevel.FullTrust,@"Verify UIElement LostKeyboardFocus works to cancel focus element in hwndsource.")]
        [TestCase("2",@"CoreInput\Keyboard","Window", TestCaseSecurityLevel.FullTrust, @"Verify UIElement LostKeyboardFocus works to cancel focus element in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementLostFocusApp(),"Run");
            
        }


        private FrameworkElement[] _panel;

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {


            CoreLogger.LogStatus("Constructing window....");
            
            {
                // Construct related Win32 window
             


                // Construct test element
                Canvas cvs = new Canvas();
                _panel = new FrameworkElement[] { new InstrFrameworkPanel(), new InstrFrameworkPanel() };
                foreach (FrameworkElement p in _panel)
                {
                    // It's necessary to enable each framework element to receive focus.
                    CoreLogger.LogStatus("Panel focusable? " + p.Focusable + ". Turning it on...");
                    p.Focusable = true;
                }

                // first element (source) - we cancel losing focus from here
                Canvas.SetTop (_panel[0], 0);
                Canvas.SetLeft (_panel[0], 0);
                _panel[0].Height = 40;
                _panel[0].Width = 40;
                _panel[0].GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                _panel[0].LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
                _panel[0].PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnPreviewFocus);
                
                // second element (target) - we attempt to set focus here later.
                Canvas.SetTop (_panel[1], 50);
                Canvas.SetLeft (_panel[1], 50);
                _panel[1].Height = 40;
                _panel[1].Width = 40;
                
                cvs.Children.Add(_panel[0]);
                cvs.Children.Add(_panel[1]);

                // Put the test element on the screen
                _rootElement = cvs;

                DisplayMe(_rootElement, 10, 10, 100, 100);   

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
            _bFocusAPI = _panel[0].Focus();
            CoreLogger.LogStatus("Focused? "+_bFocusAPI);
            
            {
                UIElement newElement = new InstrPanel();
                CoreLogger.LogStatus("Setting focus to the new element....");
                _bNewFocusAPI = _panel[1].Focus();
                CoreLogger.LogStatus("Focused new? "+_bNewFocusAPI);
            }
            
            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Stores result of Focus API call on first element.
        /// </summary>
        private bool _bFocusAPI;

        /// <summary>
        /// Stores result of Focus API call on second element.
        /// </summary>
        private bool _bNewFocusAPI;
        
        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender) 
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need focus to be on the original element and not on the new one.
            // We also need a PreviewLostKeyboardFocus event (which we canceled).
            
            CoreLogger.LogStatus("Focus set to original via API? " + (_bFocusAPI));
            CoreLogger.LogStatus("Focus set to new via API?      " + (_bNewFocusAPI));
            bool bFocusIM = InputHelper.GetFocusedElement()!=null;
            CoreLogger.LogStatus("Focus set via InputManager?  " + (bFocusIM));
            
            CoreLogger.LogStatus("Events found: "+_eventLog.Count);
            KeyboardFocusChangedEventArgs fceArgs = null;
            KeyboardFocusChangedEventArgs fceArgs1 = null;
            if (_eventLog.Count >= 2)
            {
                fceArgs = _eventLog[0] as KeyboardFocusChangedEventArgs;
                CoreLogger.LogStatus("Event args: "+_eventLog[0].GetType().ToString());
                fceArgs1 = _eventLog[0] as KeyboardFocusChangedEventArgs;
                CoreLogger.LogStatus("Event args: "+_eventLog[1].GetType().ToString());
            }
            
            // expect non-negative event count
            bool actual = _bFocusAPI && !_bNewFocusAPI && bFocusIM && (_eventLog.Count==2) && (fceArgs !=null) && (fceArgs1 !=null);
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
            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"]");
            CoreLogger.LogStatus("   Hello focusing from: '" + args.OldFocus + "' to '"+ args.NewFocus + "'");

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
            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"]");
            CoreLogger.LogStatus("   Hello focusing from: '" + args.OldFocus + "' to '"+ args.NewFocus + "'");

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();
    }
}
