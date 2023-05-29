// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify KeyGesture on parent overrides same KeyGesture on child.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyGestureOverrideChildApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\InputBindings\KeyBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify KeyGesture on parent overrides same KeyGesture on child in HwndSource.")]
        [TestCase("3", @"Commanding\InputBindings\KeyBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify KeyGesture on parent overrides same KeyGesture on child in window.")]
        [TestCase("0", @"Commanding\InputBindings\KeyBindings", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify KeyGesture on parent overrides same KeyGesture on child in Browser.")]
        [TestCase("3", @"Commanding\InputBindings\KeyBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify KeyGesture on parent overrides same KeyGesture on child in NavigationWindow.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "KeyGestureOverrideChildApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\InputBindings\KeyBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify KeyGesture on parent overrides same KeyGesture on child in HwndSource.")]
        [TestCase("0", @"Commanding\InputBindings\KeyBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Verify KeyGesture on parent overrides same KeyGesture on child in window.")]
        [TestCase("2", @"Commanding\InputBindings\KeyBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify KeyGesture on parent overrides same KeyGesture on child in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyGestureOverrideChildApp(), "Run");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            Canvas cvs = new Canvas();
            cvs.Focusable = true;

            FrameworkElement childElement = new InstrFrameworkPanel();
            childElement.Height = 80;
            childElement.Width = 80;
            Canvas.SetTop(childElement, 5);
            Canvas.SetLeft(childElement, 5);
            childElement.Focusable = true;

            cvs.Children.Add(childElement);

            CoreLogger.LogStatus("Setting up bindings....");
            CommandBinding sampleCommandBinding = new CommandBinding(SampleCommand);
            CommandBinding childCommandBinding = new CommandBinding(ApplicationCommands.Copy);

            KeyGesture commonGesture = new KeyGesture(Key.F24, ModifierKeys.Alt);
            this.Assert(commonGesture.Modifiers == ModifierKeys.Alt, "gesture Modifiers has incorrect value");
            this.Assert(commonGesture.Key == Key.F24, "gesture Key has incorrect value");

            KeyBinding sampleKeyBinding = new KeyBinding(SampleCommand, commonGesture);
            this.Assert(sampleKeyBinding.Gesture == commonGesture, "binding Gesture has incorrect value");
            this.Assert(sampleKeyBinding.Key == Key.F24, "binding Key has incorrect value");
            this.Assert(sampleKeyBinding.Modifiers == ModifierKeys.Alt, "binding Modifiers has incorrect value");
            this.Assert(sampleKeyBinding.Command == SampleCommand, "binding RoutedCommand has incorrect value");

            KeyBinding childKeyBinding = new KeyBinding(ApplicationCommands.Copy, commonGesture);

            CoreLogger.LogStatus("Attaching events to bindings....");
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            CoreLogger.LogStatus("Adding bindings....");
            cvs.CommandBindings.Add(sampleCommandBinding);
            childElement.CommandBindings.Add(childCommandBinding);
            cvs.InputBindings.Add(sampleKeyBinding);
            childElement.InputBindings.Add(childKeyBinding);

            // Put the test element on the screen
            CoreLogger.LogStatus("Showing window...");
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object sender)
        {
            CoreLogger.LogStatus("Setting initial focus...");
            bool bFocus = _rootElement.Focus();
            CoreLogger.LogStatus("Root element focused? (expect true) " + bFocus);

            CoreLogger.LogStatus("Invoking type key ...");
            KeyboardHelper.TypeKey(Key.F24, ModifierKeys.Alt);

            base.DoExecute(sender);
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

            // For this test we are looking for just the parent's command to be invoked.
            CoreLogger.LogStatus("Command event count: (expect 1) " + _commandLog.Count);
            CoreLogger.LogStatus("Command value: (expect SampleCommand) " + _cmd);

            this.Assert(_commandLog.Count == 1, "Incorrect number of  commands executed! (expected 1)");
            this.Assert(_cmd != null, "Found null command on keyup (expected non-null)");
            this.Assert(_cmd == SampleCommand, "Incorrect command was invoked (expected sample command)");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, a command has been invoked.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSample(object sender, ExecutedRoutedEventArgs e)
        {
            // Set test flag
            _commandLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");

            if (sender != null)
            {
                CoreLogger.LogStatus("  command sender Name: " + sender.ToString());
            }
            _cmd = e.Command as RoutedCommand;
            if (_cmd != null)
            {
                CoreLogger.LogStatus("  command name:        " + _cmd.Name);
            }
        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <remarks>
        /// Strategy: For our desired command, enable the appropriate Binding on the queried element.
        /// </remarks>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");
            if (sender != null)
            {
                CoreLogger.LogStatus("  command sender Name: " + sender.ToString());
            }

            RoutedCommand cmd = e.Command as RoutedCommand;
            if (cmd != null)
            {
                CoreLogger.LogStatus("  command name:        " + cmd.Name);
            }


            // Turn on command Binding
            if (e.Command == SampleCommand)
            {
                CoreLogger.LogStatus(" Turning on command '" + e.Command.ToString() + "'");
            }

            // Show we are handled and we wish to accept commands!
            e.CanExecute = true;
        }

        /// <summary>
        /// Sample command belonging to this class.
        /// </summary>
        public static RoutedCommand SampleCommand
        {
            get
            {
                if (s_sampleCommand == null)
                {
                    s_sampleCommand = new RoutedCommand("Sample", typeof(KeyGestureOverrideChildApp), null);
                }
                return s_sampleCommand;
            }
        }
        private static RoutedCommand s_sampleCommand = null;

        private RoutedCommand _cmd = null;

        private List<ExecutedRoutedEventArgs> _commandLog = new List<ExecutedRoutedEventArgs>();
    }
}
