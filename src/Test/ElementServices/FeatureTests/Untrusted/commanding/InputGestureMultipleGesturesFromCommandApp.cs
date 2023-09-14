// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Commanding
{
    /******************************************************************************
    * CLASS:          InputGestureMultipleGesturesFromCommandApp
    ******************************************************************************/
    /// <summary>
    /// Verify ApplicationCommands commands with multiple default input gestures can be executed.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [Test(1, "Commanding.CoreCommanding.Library", TestCaseSecurityLevel.FullTrust, "InputGestureMultipleGesturesFromCommandApp")]
    public class InputGestureMultipleGesturesFromCommandApp : TestApp
    {
        #region Private Data
        // Stores sample command objects.
        private RoutedCommand _cutCommand;
        private RoutedCommand _copyCommand;
        private RoutedCommand _pasteCommand;
        private RoutedCommand _propertiesCommand;
        // Store record of our fired events.
        private List<ExecutedRoutedEventArgs> _cutCommandLog = new List<ExecutedRoutedEventArgs>();
        private List<ExecutedRoutedEventArgs> _copyCommandLog = new List<ExecutedRoutedEventArgs>();
        private List<ExecutedRoutedEventArgs> _pasteCommandLog = new List<ExecutedRoutedEventArgs>();
        private List<ExecutedRoutedEventArgs> _propertiesCommandLog = new List<ExecutedRoutedEventArgs>();
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          InputGestureMultipleGesturesFromCommandApp Constructor
        ******************************************************************************/
        public InputGestureMultipleGesturesFromCommandApp()
        {
            GlobalLog.LogStatus("In InputGestureMultipleGesturesFromCommandApp constructor");
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            ExeStubContainerCore exe = new ExeStubContainerCore();
            TestApp app = new InputGestureMultipleGesturesFromCommandApp();
            exe.Run(app, "RunTestApp");

            //Any test failure will be caught by an Assert during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          DoSetup
        ******************************************************************************/
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test element
            _rootElement = new InstrPanel();

            // Set up command with this Binding as a default
            _cutCommand = ApplicationCommands.Cut;
            GlobalLog.LogStatus("Command constructed: Command=" + _cutCommand.ToString());
            _copyCommand = ApplicationCommands.Copy;
            GlobalLog.LogStatus("Command constructed: Command=" + _copyCommand.ToString());
            _pasteCommand = ApplicationCommands.Paste;
            GlobalLog.LogStatus("Command constructed: Command=" + _pasteCommand.ToString());
            _propertiesCommand = ApplicationCommands.Properties;
            GlobalLog.LogStatus("Command constructed: Command=" + _propertiesCommand.ToString());

            // Create a normal command Binding using this command.
            CommandBinding cutCommandBinding = new CommandBinding(_cutCommand);
            CommandBinding copyCommandBinding = new CommandBinding(_copyCommand);
            CommandBinding pasteCommandBinding = new CommandBinding(_pasteCommand);
            CommandBinding propertiesCommandBinding = new CommandBinding(_propertiesCommand);

            // Attach events to bindings
            cutCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSampleCut);
            copyCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSampleCopy);
            pasteCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSamplePaste);
            propertiesCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSampleProperties);

            // Add this Binding to test element
            _rootElement.CommandBindings.Add(cutCommandBinding);
            _rootElement.CommandBindings.Add(copyCommandBinding);
            _rootElement.CommandBindings.Add(pasteCommandBinding);
            _rootElement.CommandBindings.Add(propertiesCommandBinding);

            // Put the test element on the screen
            DisplayMe(_rootElement,10, 10, 100, 100);
    
            return null;
        }

        /******************************************************************************
        * Function:          InputCallback
        ******************************************************************************/
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
                    DoBeforeExecute();
                },
                //new VoidOp(new TestOpVoidHandler(DoBeforeExecute)),
                // input gesture cut
                delegate
                {
                    KeyboardHelper.TypeKey(Key.X, ModifierKeys.Control);                    
                },
                // input gesture cut
                delegate
                {
                    KeyboardHelper.TypeKey(Key.Delete, ModifierKeys.Shift);
                },
                // input gesture copy
                delegate
                {
                    KeyboardHelper.TypeKey(Key.Insert, ModifierKeys.Control);                
                },   
                // input gesture copy
                delegate
                {
                    KeyboardHelper.PressKey(Key.RightCtrl);
                    KeyboardHelper.TypeKey(Key.Insert);
                    KeyboardHelper.ReleaseKey(Key.RightCtrl);                    
                },
                // input gesture paste
                delegate
                {
                    KeyboardHelper.PressKey(Key.RightShift);
                    KeyboardHelper.TypeKey(Key.Insert);
                    KeyboardHelper.ReleaseKey(Key.RightShift);                    
                },
                delegate
                {
                    KeyboardHelper.PressKey(Key.RightCtrl);
                    KeyboardHelper.TypeKey(Key.V);
                    KeyboardHelper.ReleaseKey(Key.RightCtrl);                    
                },
                // input gesture properties
                delegate
                {
                    KeyboardHelper.TypeKey(Key.F4);
                },
            };
            return ops;
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          DoValidate
        ******************************************************************************/
        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.BeginVariation("InputGestureMultipleGesturesFromCommandApp");
            GlobalLog.LogStatus("Validating...");

            // For this test we are looking if our command is set up to use defaults.
            // If so, we want to make sure that if our command Binding has no local bindings,
            // we use the non-local (Command.Defaults) binding

            GlobalLog.LogStatus("Command Bindings found: (expect 4) " + _rootElement.CommandBindings.Count);
            GlobalLog.LogStatus("Input Bindings found: (expect 0) " + _rootElement.InputBindings.Count);
            GlobalLog.LogStatus(" Events found cut: (expect 2) " + _cutCommandLog.Count);
            GlobalLog.LogStatus(" Events found copy: (expect 2) " + _copyCommandLog.Count);
            GlobalLog.LogStatus(" Events found paste: (expect 2) " + _pasteCommandLog.Count);
            GlobalLog.LogStatus(" Events found properties: (expect 1) " + _propertiesCommandLog.Count);

            bool bindingsFound;
            if ((_rootElement.CommandBindings.Count == 4) &&
                (_rootElement.InputBindings.Count == 0) &&
                (_cutCommandLog.Count == 2) &&
                (_copyCommandLog.Count == 2) &&
                (_pasteCommandLog.Count == 2) &&
                (_propertiesCommandLog.Count == 1)
                )
            {
                bindingsFound = true;
            }
            else
            {
                bindingsFound = false;
            }

            GlobalLog.LogStatus("Setting log result to " + bindingsFound);
            this.TestPassed = bindingsFound;

            GlobalLog.LogStatus("Validation complete!");
            CoreLogger.LogTestResult(this.TestPassed, "Passed if event found.  Failed if not.");
            CoreLogger.EndVariation();

            return null;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          DoBeforeExecute
        ******************************************************************************/
        /// <summary>
        /// Execute stuff right before the test operations.
        /// </summary>
        private void DoBeforeExecute()
        {
            GlobalLog.LogStatus("Setting focus to the element....");
            bool bFocus = _rootElement.Focus();
            GlobalLog.LogStatus("Focus set via API?           " + (bFocus));
            GlobalLog.LogStatus("Focus set via InputManager?  " + (InputManager.Current.PrimaryKeyboardDevice.FocusedElement != null));

            // Now our TestOps will fire....
        }

        /******************************************************************************
        * Function:          OnSampleCut
        ******************************************************************************/
        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSampleCut(object sender, ExecutedRoutedEventArgs e)
        {
            // We are executing a command!
            _cutCommandLog.Add(e);

            GlobalLog.LogStatus("In command event:");
            GlobalLog.LogStatus(" Command:            " + e.Command.ToString());
            if (sender != null)
            {
                GlobalLog.LogStatus(" command sender Name: " + sender.ToString());
            }
        }

        /******************************************************************************
        * Function:          OnSampleCopy
        ******************************************************************************/
        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSampleCopy(object sender, ExecutedRoutedEventArgs e)
        {
            // We are executing a command!
            _copyCommandLog.Add(e);

            GlobalLog.LogStatus("In command event:");
            GlobalLog.LogStatus(" Command:            " + e.Command.ToString());
            if (sender != null)
            {
                GlobalLog.LogStatus(" command sender Name: " + sender.ToString());
            }
        }

        /******************************************************************************
        * Function:          OnSamplePaste
        ******************************************************************************/
        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSamplePaste(object sender, ExecutedRoutedEventArgs e)
        {
            // We are executing a command!
            _pasteCommandLog.Add(e);

            GlobalLog.LogStatus("In command event:");
            GlobalLog.LogStatus(" Command:            " + e.Command.ToString());
            if (sender != null)
            {
                GlobalLog.LogStatus(" command sender Name: " + sender.ToString());
            }
        }

        /******************************************************************************
        * Function:          OnSampleProperties
        ******************************************************************************/
        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSampleProperties(object sender, ExecutedRoutedEventArgs e)
        {
            // We are executing a command!
            _propertiesCommandLog.Add(e);

            GlobalLog.LogStatus("In command event:");
            GlobalLog.LogStatus(" Command:            " + e.Command.ToString());
            if (sender != null)
            {
                GlobalLog.LogStatus(" command sender Name: " + sender.ToString());
            }
        }
        #endregion
    }
}
