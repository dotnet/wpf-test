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
    /// Verify Keyboard Focus fails for invalid input element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FocusInvalidInputElementApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Focus","HwndSource",@"Compile and Verify   Keyboard Focus fails for invalid input element in window in HwndSource.")]
        [TestCase("2",@"CoreInput\Focus","Browser",@"Compile and Verify   Keyboard Focus fails for invalid input element in window in Browser.")]
        [TestCase("2",@"CoreInput\Focus","Window",@"Compile and Verify   Keyboard Focus fails for invalid input element in window in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "FocusInvalidInputElementApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Focus","HwndSource",@"Verify   Keyboard Focus fails for invalid input element in window in HwndSource.")]
        [TestCase("2",@"CoreInput\Focus","Window",@"Verify   Keyboard Focus fails for invalid input element in window in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FocusInvalidInputElementApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {


            CoreLogger.LogStatus("Constructing window....");
            
            // Construct related Win32 window


            // Construct test element, add event handling
            _rootElement = new InstrPanel();
            _rootElement.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            _rootElement.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            _rootElement.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnPreviewFocus);
            _rootElement.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnPreviewFocus);

            // Construct invalid input element, add event handling
            _dio = new TestDependencyInputObject();
            _dio.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            _dio.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            _dio.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnPreviewFocus);
            _dio.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnPreviewFocus);

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Invalid input element.
        /// </summary>
        private IInputElement _dio;

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg) 
        {
            CoreLogger.LogStatus("Startup focused element: " + Keyboard.FocusedElement);

            _baselineEventLogCount = _eventLog.Count;
            CoreLogger.LogStatus("Events raised so far: " + _baselineEventLogCount);

            CoreLogger.LogStatus("Setting focus to the invalid input element....");
            try
            {
                Keyboard.Focus(_dio);
            }
            catch (InvalidOperationException e)
            {
                _exceptionLog.Add(e);
            }
            
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

            // For this test we need no additional focus events to fire on setting focus to invalid input element.
            // (Events come from the root element on startup, NOT the invalid input element.)
            // We also need an invalid operation exception to occur.

            int newEventLogCount = _eventLog.Count;

            CoreLogger.LogStatus("New events found: (expect 0) " + (newEventLogCount - _baselineEventLogCount));
            CoreLogger.LogStatus("Exceptions found: (expect 1) "+_exceptionLog.Count);
            if (_exceptionLog.Count == 1)
            {
                CoreLogger.LogStatus("Logged exception (expect InvalidOperationException):\n"+(Exception)_exceptionLog[0]);                
            }

            bool actual = (_exceptionLog.Count == 1) && ((newEventLogCount == _baselineEventLogCount));
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
        /// <param name="e">Event-specific arguments.</param>
        private void OnFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");
            CoreLogger.LogStatus("   Hello focusing from: " + e.OldFocus + " to "+ e.NewFocus);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Standard preview focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnPreviewFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");
            CoreLogger.LogStatus("   Hello focusing from: " + e.OldFocus + " to "+ e.NewFocus);

            // Continue routing this event (no cancel on preview)
            e.Handled = false;
        }

        private int _baselineEventLogCount = 0;

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<KeyboardFocusChangedEventArgs> _eventLog = new List<KeyboardFocusChangedEventArgs>();

        /// <summary>
        /// Store record of our fired exceptions.
        /// </summary>
        private List<InvalidOperationException> _exceptionLog = new List<InvalidOperationException>();

    }
}
