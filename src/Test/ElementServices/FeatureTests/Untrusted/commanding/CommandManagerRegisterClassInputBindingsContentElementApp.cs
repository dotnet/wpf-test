// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify CommandManager RegisterClassInputBindings for ContentElement in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandManager RegisterClassInputBindings for ContentElement in window.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\InputBindings\MouseBindings")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandManagerRegisterClassInputBindingsContentElementApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandManagerRegisterClassInputBindingsContentElementApp();
            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus("App run!");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            // Construct test element
            InstrContentPanelHost host = new InstrContentPanelHost();
            _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);
            host.AddChild(_contentElement);

            // Set up commands, links
            RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command=" + sampleCommand.ToString());
            CommandBinding sampleCommandBinding = new CommandBinding(sampleCommand);
            CoreLogger.LogStatus("Command link constructed: CommandBinding=" + sampleCommandBinding.ToString());

            // Set up mouse bindings
            MouseGesture gesture = new MouseGesture(MouseAction.MiddleDoubleClick);
            MouseBinding sampleMouseBinding = new MouseBinding(sampleCommand, gesture);
            CoreLogger.LogStatus("Command link MouseBinding constructed: MouseBinding=" + sampleMouseBinding.ToString());

            // Attach events to links
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            // Add links to test element
            _contentElement.CommandBindings.Add(sampleCommandBinding);
            CommandManager.RegisterClassInputBinding(typeof(InstrContentPanel), sampleMouseBinding);

            // Put the test element on the screen
            DisplayMe(host, 10, 10, 200, 200);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            _contentElement.Focus();

            InputCallback[] ops = new InputCallback[] {
                delegate
                {
                    MouseHelper.Click(MouseButton.Middle,_rootElement);
                    MouseHelper.Click(MouseButton.Middle);                    
                }                
            };
            return ops;
            // Now our TestOps will fire....
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we are just looking for a command invocation (Execute event).
            // We also want to make sure the event sender was the content element.

            CoreLogger.LogStatus("Events found: (expect 1) " + _commandLog.Count);
            if (_commandLog.Count > 0)
            {
                CoreLogger.LogStatus(" command sender: (expect InstrContentPanel) " + _commandLog[0].Source.GetType().ToString());
            }

            bool actual = (_commandLog.Count == 1) && (_commandLog[0].Source.GetType() == typeof(InstrContentPanel));
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<ExecutedRoutedEventArgs> _commandLog = new List<ExecutedRoutedEventArgs>();

        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSample(object sender, ExecutedRoutedEventArgs e)
        {
            // We are executing a command! Save who sent it.
            _commandLog.Add(e);

            CoreLogger.LogStatus("In command event:");
            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            if (sender != null)
            {
                CoreLogger.LogStatus(" command sender Name: " + sender.ToString());
            }
            CoreLogger.LogStatus(" Events found: " + _commandLog.Count);


        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            // if we are in this handler, the case passes!
            CoreLogger.LogStatus("In query event:");
            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            if (sender != null)
            {
                CoreLogger.LogStatus(" command sender Name: " + sender.ToString());
            }

            // Show we are handled and we wish to accept commands!
            e.CanExecute = true;
        }

        private ContentElement _contentElement;
    }
}
