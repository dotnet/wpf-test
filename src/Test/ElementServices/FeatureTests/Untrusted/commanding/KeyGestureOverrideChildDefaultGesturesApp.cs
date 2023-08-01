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
    /// Verify element-level KeyGesture on child overrides same default KeyGesture on parent.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyGestureOverrideChildDefaultGesturesApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\InputBindings\KeyBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify element-level KeyGesture on child overrides same default KeyGesture on parent in HwndSource.")]
        [TestCase("2", @"Commanding\InputBindings\KeyBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify element-level KeyGesture on child overrides same default KeyGesture on parent in window.")]
        [TestCase("2", @"Commanding\InputBindings\KeyBindings", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify element-level KeyGesture on child overrides same default KeyGesture on parent in Browser.")]
        [TestCase("2", @"Commanding\InputBindings\KeyBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify element-level KeyGesture on child overrides same default KeyGesture on parent in NavigationWindow.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "KeyGestureOverrideChildDefaultGesturesApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\InputBindings\KeyBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify element-level KeyGesture on child overrides same default KeyGesture on parent in HwndSource.")]
        [TestCase("1", @"Commanding\InputBindings\KeyBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Verify element-level KeyGesture on child overrides same default KeyGesture on parent in window.")]
        [TestCase("1", @"Commanding\InputBindings\KeyBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify element-level KeyGesture on child overrides same default KeyGesture on parent in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyGestureOverrideChildDefaultGesturesApp(), "Run");
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

            KeyGesture commonGesture = new KeyGesture(Key.F13);
            KeyBinding childKeyBinding = new KeyBinding(ApplicationCommands.Copy, commonGesture);

            CoreLogger.LogStatus("Attaching events to bindings....");
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);
            childCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            childCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            CoreLogger.LogStatus("Adding bindings....");
            cvs.CommandBindings.Add(sampleCommandBinding);
            childElement.CommandBindings.Add(childCommandBinding);
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
            Canvas cvs = (Canvas)_rootElement;
            UIElement child = cvs.Children[0];

            CoreLogger.LogStatus("Setting initial focus...");
            bool bFocus = _rootElement.Focus();
            DispatcherHelper.DoEventsPastInput();
            CoreLogger.LogStatus("Root element focused? (expect true) " + bFocus);

            CoreLogger.LogStatus("Invoking type key ...");
            KeyboardHelper.TypeKey(Key.F13);
            DispatcherHelper.DoEventsPastInput();

            CoreLogger.LogStatus("Setting secondary focus...");
            bFocus = child.Focus();
            DispatcherHelper.DoEventsPastInput();
            CoreLogger.LogStatus("Child element focused? (expect true) " + bFocus);

            CoreLogger.LogStatus("Invoking type key again...");
            KeyboardHelper.TypeKey(Key.F13);

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

            // For this test we are looking for the child and the parent Canvas to execute a command.
            // We're also expecting the command to be the child's when the child is in focus.
            // We're also expecting the command to be the parent's from default gestures when the parent is in focus.

            CoreLogger.LogStatus("Command event count: (expect 2) " + _sampleCommandLog.Count);
            this.Assert(_sampleCommandLog.Count == 2, "Incorrect number of commands executed! (expected 2)");

            Canvas cvs = (Canvas)_rootElement;
            UIElement child = cvs.Children[0];

            CoreLogger.LogStatus("Command event #1 source: (expect root canvas) " + _sampleCommandLog[0].Source);
            CoreLogger.LogStatus("Command event #2 source: (expect child element) " + _sampleCommandLog[1].Source);

            RoutedCommand executedCommand1 = _sampleCommandLog[0].Command as RoutedCommand;
            RoutedCommand executedCommand2 = _sampleCommandLog[1].Command as RoutedCommand;
            this.Assert(executedCommand1 != null, "Source #1 command not routed!");
            this.Assert(executedCommand2 != null, "Source #2 command not routed!");

            this.Assert(executedCommand1 == SampleCommand, "Source #1 command not expected SampleCommand!");
            this.Assert(executedCommand2 == ApplicationCommands.Copy, "Source #2 command not expected Copy!");

            this.Assert(_sampleCommandLog[0].Source == cvs, "Oops .. unexpected #1 source");
            this.Assert(_sampleCommandLog[1].Source == child, "Oops .. unexpected #2 source");

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
            _sampleCommandLog.Add(e);

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
                    KeyGesture commonGesture = new KeyGesture(Key.F13);
                    InputGestureCollection defaultGestures = new InputGestureCollection();
                    defaultGestures.Add(commonGesture);

                    s_sampleCommand = new RoutedCommand("Sample", typeof(KeyGestureOverrideChildDefaultGesturesApp), defaultGestures);
                }
                return s_sampleCommand;
            }
        }
        private static RoutedCommand s_sampleCommand = null;

        private List<ExecutedRoutedEventArgs> _sampleCommandLog = new List<ExecutedRoutedEventArgs>();
    }
}
