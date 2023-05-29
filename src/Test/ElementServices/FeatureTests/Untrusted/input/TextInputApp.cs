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
    /// Verify Text event fires on an alphanumeric key down.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class TextInputApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Keyboard","HwndSource", TestCaseSecurityLevel.FullTrust,@"Compile and Verify Text event fires on an alphanumeric key down in HwndSource.")]
        [TestCase("0",@"CoreInput\Keyboard","Browser", @"Compile and Verify Text event fires on an alphanumeric key down in Browser.")]
        [TestCase("3",@"CoreInput\Keyboard","Window", TestCaseSecurityLevel.FullTrust,@"Compile and Verify Text event fires on an alphanumeric key down in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "TextInputApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Keyboard","HwndSource",  TestCaseSecurityLevel.FullTrust,@"Verify Text event fires on an alphanumeric key down in HwndSource.")]
        [TestCase("0",@"CoreInput\Keyboard","Window", TestCaseSecurityLevel.FullTrust, @"Verify Text event fires on an alphanumeric key down in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new TextInputApp(),"Run");
            
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
            _rootElement.PreviewTextInput += new TextCompositionEventHandler(OnPreviewText);

            InputMethod.SetPreferredImeState(_rootElement, InputMethodState.Off);

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);

            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

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
                    KeyboardHelper.TypeKey(Key.X);
                }                   
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
            
            CoreLogger.LogStatus("Events found: "+_eventLog.Count);
            
            // expect non-negative event count
            int actual = (_eventLog.Count);
            int expected = 2;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Standard text event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnText(object sender, TextCompositionEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Debug.WriteLine ("   Hello from: " + args.Text);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Standard preview text event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnPreviewText(object sender, TextCompositionEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Debug.WriteLine ("   Hello from: " + args.Text);

            // Continue routing this event (don't block it)
            args.Handled = false;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();
    }
}
