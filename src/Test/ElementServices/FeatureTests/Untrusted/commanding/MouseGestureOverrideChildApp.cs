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
    /// Verify MouseGesture on parent overrides same MouseGesture on child.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseGestureOverrideChildApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\InputBindings\MouseBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify MouseGesture on parent overrides same MouseGesture on child in HwndSource.")]
        [TestCase("2", @"Commanding\InputBindings\MouseBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify MouseGesture on parent overrides same MouseGesture on child in window.")]
        [TestCase("0", @"Commanding\InputBindings\MouseBindings", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify MouseGesture on parent overrides same MouseGesture on child in Browser.")]
        [TestCase("2", @"Commanding\InputBindings\MouseBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify MouseGesture on parent overrides same MouseGesture on child in NavigationWindow.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "MouseGestureOverrideChildApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\InputBindings\MouseBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify MouseGesture on parent overrides same MouseGesture on child in HwndSource.")]
        [TestCase("0", @"Commanding\InputBindings\MouseBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Verify MouseGesture on parent overrides same MouseGesture on child in window.")]
        [TestCase("1", @"Commanding\InputBindings\MouseBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify MouseGesture on parent overrides same MouseGesture on child in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseGestureOverrideChildApp(), "Run");

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
            cvs.Background = Brushes.Transparent;
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
            CommandBinding copyCommandBinding = new CommandBinding(ApplicationCommands.Copy);

            MouseGesture commonGesture = new MouseGesture(MouseAction.RightClick);

            MouseBinding sampleMouseBinding = new MouseBinding(SampleCommand, commonGesture);
            
            MouseBinding copyMouseBinding = new MouseBinding(ApplicationCommands.Copy, commonGesture);

            CoreLogger.LogStatus("Attaching events to bindings....");
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);
            sampleCommandBinding.PreviewCanExecute += new CanExecuteRoutedEventHandler(OnQuery);
            copyCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            copyCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);
            copyCommandBinding.PreviewCanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            CoreLogger.LogStatus("Adding bindings....");
            cvs.CommandBindings.Add(sampleCommandBinding);
            cvs.InputBindings.Add(sampleMouseBinding);
            childElement.CommandBindings.Add(copyCommandBinding);
            childElement.InputBindings.Add(copyMouseBinding);

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
            CoreLogger.LogStatus("Setting initial focus...");
            UIElement panel = cvs.Children[0];
            this.Assert(panel != null, "Whoops, expected panel does not exist!");
            bool bFocus = panel.Focus();
            CoreLogger.LogStatus("Child element focused? (expect true) " + bFocus);

            CoreLogger.LogStatus("Trying with ContinueRouting=true...");
            _continueRouting = true;
            MouseHelper.Click(MouseButton.Right, panel, MouseLocation.Center);
            MouseHelper.MoveOnPrimaryMonitor(MouseLocation.TopLeft, true);

            CoreLogger.LogStatus("Trying with ContinueRouting=false...");
            _continueRouting = false;
            MouseHelper.Click(MouseButton.Right, panel, MouseLocation.Center);
            MouseHelper.MoveOnPrimaryMonitor(MouseLocation.TopLeft, true);

            CoreLogger.LogStatus("Trying with ContinueRouting=true...");
            _continueRouting = true;
            MouseHelper.Click(MouseButton.Right, panel, MouseLocation.Center);
            MouseHelper.MoveOnPrimaryMonitor(MouseLocation.TopLeft, true);

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
            this.Assert(_commandLog.Count == 2, "Incorrect number of commands executed!");

            foreach (ExecutedRoutedEventArgs args in _commandLog)
            {
                RoutedCommand routedCommand = (RoutedCommand)args.Command;
                CoreLogger.LogStatus("Command value: (expect SampleCommand) " + routedCommand.Name);
                this.Assert(routedCommand == SampleCommand, "Incorrect command was invoked");
            }

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
            if (cmd == SampleCommand)
            {
                CoreLogger.LogStatus("   Turning on command '" + cmd.Name + "'");
                e.CanExecute = true;
            }
            else
            {
                e.ContinueRouting = _continueRouting;
                e.CanExecute = false;
            }
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
                    s_sampleCommand = new RoutedCommand("Sample", typeof(MouseGestureOverrideChildApp), null);
                }
                return s_sampleCommand;
            }
        }

        private static RoutedCommand s_sampleCommand = null;

        private List<ExecutedRoutedEventArgs> _commandLog = new List<ExecutedRoutedEventArgs>();

        private bool _continueRouting = false;
    }
}
