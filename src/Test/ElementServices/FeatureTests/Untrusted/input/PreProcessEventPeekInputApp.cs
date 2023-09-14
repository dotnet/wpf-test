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
using Microsoft.Test.Input;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify InputManager PreProcessInputEventArgs.PeekInput works as expected.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class PreProcessEventPeekInputApp: TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All","All","2",@"CoreInput\InputManager",TestCaseSecurityLevel.FullTrust,"Verify InputManager PreProcessInputEventArgs.PeekInput works as expected.")]
        [TestCaseContainerAttribute("TestApplicationStub","Browser","2",@"CoreInput\InputManager","Verify InputManager PreProcessInputEventArgs.PeekInput works as expected.")]
        public void LaunchTest()
        {
            Run();
        } 


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            CoreLogger.LogStatus("Constructing window....");
            
            {
                // SPECIAL NOTE: According to breaking change ,
                // in order for activation input events to be heard,
                // activation input event handlers must be attached BEFORE creating the source.

                // Attaching InputManagerEvents
                InputManagerHelper.CurrentHelper.PreProcessInput += new PreProcessInputEventHandler(Current_PreProcessInput);
                
                // Construct test element, add event handling
                _rootElement = new InstrPanel();

                // Put the test element on the screen
                DisplayMe(_rootElement, 50, 50, 50, 30);
            }
            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

            return _hwnd;
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

            // For this test we expect our PeekInput call to have picked up the input item.
            
            CoreLogger.LogStatus("Input element peeked? (expect something) '"+_peekInputItem + "'");
            
            // expect non-negative event count
            bool actual = (_peekInputItem != null);
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
        private ArrayList _eventLog = new ArrayList();

        private void Current_PreProcessInput(object sender, PreProcessInputEventArgs args)
        {
            CoreLogger.LogStatus("Preprocess!    "+args.ToString()+" [sender="+sender.GetType().ToString()+"]");
            CoreLogger.LogStatus(" Canceled?     "+args.Canceled);

            InputEventArgs inputEventArgs = args.StagingItem.Input;

            CoreLogger.LogStatus(" Staging item: " + inputEventArgs.RoutedEvent.Name + " [" + inputEventArgs.Timestamp + "]");
            if (inputEventArgs.RoutedEvent == InputManagerHelper.InputReportEvent)
            {
                InputReportEventArgsWrapper irArgs = new InputReportEventArgsWrapper(inputEventArgs);
                InputReportWrapper ir = irArgs.Report;
                CoreLogger.LogStatus(" Report:       " + ir.Mode + "," + ir.Name + " [" + ir.Timestamp + "]");
            }
            _eventLog.Add(args);

            ProcessInputEventArgs argsWrapper = args;

            // Peek input, stash any reported value.
            if (_peekInputItem == null)
            {
                _peekInputItem = argsWrapper.PeekInput();
            }

        }

        private StagingAreaInputItem _peekInputItem = null;

    }
}
