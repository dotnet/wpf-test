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
    /// Verify element-level MouseGesture on child overrides same default MouseGesture on parent.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseGestureOverrideChildDefaultGesturesApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\InputBindings\MouseBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify element-level MouseGesture on child overrides same default MouseGesture on parent in HwndSource.")]
        [TestCase("3", @"Commanding\InputBindings\MouseBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify element-level MouseGesture on child overrides same default MouseGesture on parent in window.")]
        [TestCase("1", @"Commanding\InputBindings\MouseBindings", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify element-level MouseGesture on child overrides same default MouseGesture on parent in Browser.")]
        [TestCase("3", @"Commanding\InputBindings\MouseBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify element-level MouseGesture on child overrides same default MouseGesture on parent in NavigationWindow.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "MouseGestureOverrideChildDefaultGesturesApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\InputBindings\MouseBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify element-level MouseGesture on child overrides same default MouseGesture on parent in HwndSource.")]
        [TestCase("2", @"Commanding\InputBindings\MouseBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Verify element-level MouseGesture on child overrides same default MouseGesture on parent in window.")]
        [TestCase("2", @"Commanding\InputBindings\MouseBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify element-level MouseGesture on child overrides same default MouseGesture on parent in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseGestureOverrideChildDefaultGesturesApp(), "Run");

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
            childElement.Height = 5;
            childElement.Width = 80;
            Canvas.SetTop(childElement, 5);
            Canvas.SetLeft(childElement, 5);
            childElement.Focusable = true;

            cvs.Children.Add(childElement);

            CoreLogger.LogStatus("Setting up bindings....");
            CommandBinding sampleCommandBinding = new CommandBinding(SampleCommand);
            CommandBinding childCommandBinding = new CommandBinding(ApplicationCommands.Copy);

            MouseGesture commonGesture = new MouseGesture(MouseAction.LeftClick);
            MouseBinding childMouseBinding = new MouseBinding(ApplicationCommands.Copy, commonGesture);

            CoreLogger.LogStatus("Attaching events to bindings....");
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSampleCanvas);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQueryCanvas);
            childCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSampleChild);
            childCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQueryChild);

            CoreLogger.LogStatus("Adding bindings....");
            cvs.CommandBindings.Add(sampleCommandBinding);
            childElement.CommandBindings.Add(childCommandBinding);
            childElement.InputBindings.Add(childMouseBinding);

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
            UIElement panel = cvs.Children[0];

            CoreLogger.LogStatus("Setting initial focus...");
            bool bFocus = cvs.Focus();
            CoreLogger.LogStatus("Root element focused? (expect true) " + bFocus);

            CoreLogger.LogStatus("Invoking mouse click ...");
            MouseHelper.Click(cvs);
            DispatcherHelper.DoEventsPastInput();

            CoreLogger.LogStatus("Setting secondary focus...");
            bFocus = panel.Focus();
            CoreLogger.LogStatus("Child element focused? (expect true) " + bFocus);

            CoreLogger.LogStatus("Invoking mouse click again...");
            MouseHelper.Click(panel);
            DispatcherHelper.DoEventsPastInput();

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

            // For this test we are looking for the child (not the Canvas) to execute a command.
            // We're also expecting the command to be the child's, not the default from the parent's default gestures.

            CoreLogger.LogStatus("Command event count: (expect 1) " + _sampleCommandLog.Count);
            this.Assert(_sampleCommandLog.Count == 1, "Too many commands executed! (expected 2)");

            Canvas cvs = (Canvas)_rootElement;
            UIElement child = cvs.Children[0];

            CoreLogger.LogStatus("Command event #1 source: (expect child element) " + _sampleCommandLog[0].Source);
            RoutedCommand executedCommand1 = _sampleCommandLog[0].Command as RoutedCommand;
            this.Assert(executedCommand1 != null, "Source #1 command not routed!");
            this.Assert(executedCommand1 == ApplicationCommands.Copy, "Source #1 command not expected Copy!");
            this.Assert(_sampleCommandLog[0].Source == child, "Oops .. unexpected #1 source");

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
        private void OnSampleCanvas(object sender, ExecutedRoutedEventArgs e)
        {
            // Set test flag
            _sampleCommandLog.Add(e);

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
        private void OnQueryCanvas(object sender, CanExecuteRoutedEventArgs e)
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
        /// If we are in this CommandEvent Handler, a command has been invoked.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSampleChild(object sender, ExecutedRoutedEventArgs e)
        {
            // Set test flag
            _sampleCommandLog.Add(e);

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
        private void OnQueryChild(object sender, CanExecuteRoutedEventArgs e)
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
                    MouseGesture commonGesture = new MouseGesture(MouseAction.LeftClick);
                    InputGestureCollection defaultGestures = new InputGestureCollection();
                    defaultGestures.Add(commonGesture);

                    s_sampleCommand = new RoutedCommand("Sample", typeof(MouseGestureOverrideChildDefaultGesturesApp), defaultGestures);
                }
                return s_sampleCommand;
            }
        }
        private static RoutedCommand s_sampleCommand = null;

        private RoutedCommand _cmd = null;

        private List<ExecutedRoutedEventArgs> _sampleCommandLog = new List<ExecutedRoutedEventArgs>();
    }
}
