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
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify InputManager multiple handlers on ProcessInput events fire in REVERSE order of when they were added.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class InputManagerMultipleProcessEventsApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\InputManager", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify InputManager multiple handlers on ProcessInput events fire in REVERSE order of when they were added in HwndSource.")]
        [TestCase("2", @"CoreInput\InputManager", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify InputManager multiple handlers on ProcessInput events fire in REVERSE order of when they were added in Browser.")]
        [TestCase("2", @"CoreInput\InputManager", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify InputManager multiple handlers on ProcessInput events fire in REVERSE order of when they were added in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "InputManagerMultipleProcessEventsApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\InputManager", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify InputManager multiple handlers on ProcessInput events fire in REVERSE order of when they were added in HwndSource.")]
        [TestCase("2", @"CoreInput\InputManager", "Window", TestCaseSecurityLevel.FullTrust, @"Verify InputManager multiple handlers on ProcessInput events fire in REVERSE order of when they were added in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new InputManagerMultipleProcessEventsApp(),"Run");
            
        }
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            CoreLogger.LogStatus("Constructing window....");
            
            // SPECIAL NOTE: According to breaking change ,
            // in order for activation input events to be heard,
            // activation input event handlers must be attached BEFORE creating the source.

            // Attaching InputManagerEvents
            _preProcessHandlers = new PreProcessInputEventHandler[] {
                new PreProcessInputEventHandler(First_PreProcessInput),
                new PreProcessInputEventHandler(Second_PreProcessInput),
            };
            _processHandlers = new ProcessInputEventHandler[] {
                new ProcessInputEventHandler(First_PostProcessInput),
                new ProcessInputEventHandler(Second_PostProcessInput),
            };
            InputManagerHelper.CurrentHelper.PreProcessInput += _preProcessHandlers[0];
            InputManagerHelper.CurrentHelper.PreProcessInput += _preProcessHandlers[1];
            InputManagerHelper.CurrentHelper.PostProcessInput += _processHandlers[0];
            InputManagerHelper.CurrentHelper.PostProcessInput += _processHandlers[1];
            
            // Construct test element, add event handling
            _rootElement = new InstrPanel();

            // Put the test element on the screen
            DisplayMe(_rootElement, 50, 50, 50, 30);

            KeyboardHelper.EnsureFocus(_rootElement);

            return _hwnd;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender) 
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we expect 4 events (PreviewInputReport, InputReport, PreviewGotKeyboardFocus, GotKeyboardFocus) 
            // x 2 handlers
            // x 2 event logs
            // = 16 event log items (4 per event)

            CoreLogger.LogStatus("pre  Events found (expect 8): "+_preEventHandlerLog.Count);
            CoreLogger.LogStatus("post Events found (expect 8): "+_postEventHandlerLog.Count);

            // We also verify the order of events in the event stream....
            // in REVERSE order of when they were added to the Input Manager.
            
            bool bExpectedPreOrder = false;
            if (_preEventHandlerLog.Count>=6)
            {
                // second, first, second, first
                bExpectedPreOrder = ((PreProcessInputEventHandler)_preEventHandlerLog[0] == _preProcessHandlers[1]) &&
                                    ((PreProcessInputEventHandler)_preEventHandlerLog[1] == _preProcessHandlers[0]) &&
                                    ((PreProcessInputEventHandler)_preEventHandlerLog[2] == _preProcessHandlers[1]) &&
                                    ((PreProcessInputEventHandler)_preEventHandlerLog[3] == _preProcessHandlers[0]) &&
                                    ((PreProcessInputEventHandler)_preEventHandlerLog[4] == _preProcessHandlers[1]) &&
                                    ((PreProcessInputEventHandler)_preEventHandlerLog[5] == _preProcessHandlers[0]);
            }
            CoreLogger.LogStatus("Expected PreProcessInput events in reverse order? " + bExpectedPreOrder);
            
            bool bExpectedPostOrder = false;
            if (_postEventHandlerLog.Count>=6)
            {
                // second, first, second, first
                bExpectedPostOrder = ((ProcessInputEventHandler)_postEventHandlerLog[0] == _processHandlers[1]) &&
                                    ((ProcessInputEventHandler)_postEventHandlerLog[1] == _processHandlers[0]) &&
                                    ((ProcessInputEventHandler)_postEventHandlerLog[2] == _processHandlers[1]) &&
                                    ((ProcessInputEventHandler)_postEventHandlerLog[3] == _processHandlers[0]) &&
                                    ((ProcessInputEventHandler)_postEventHandlerLog[4] == _processHandlers[1]) &&
                                    ((ProcessInputEventHandler)_postEventHandlerLog[5] == _processHandlers[0]);
            }
            CoreLogger.LogStatus("Expected PostProcessInput events in reverse order? " + bExpectedPreOrder);
            
            // expect non-negative event count and correct order of events
            bool actual = (_preEventHandlerLog.Count==8) && (_postEventHandlerLog.Count==8) && bExpectedPreOrder && bExpectedPostOrder;
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Store record of our fired PreProcessInput events.
        /// </summary>
        private ArrayList _preEventHandlerLog = new ArrayList();

        /// <summary>
        /// Store record of our fired PostProcessInput events.
        /// </summary>
        private ArrayList _postEventHandlerLog = new ArrayList();

        /// <summary>
        /// Store record of our PreProcessInput event handlers.
        /// </summary>
        private PreProcessInputEventHandler[] _preProcessHandlers;

        /// <summary>
        /// Store record of our PostProcessInput event handlers.
        /// </summary>
        private ProcessInputEventHandler[] _processHandlers;
       

        private void First_PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            CoreLogger.LogStatus("Postprocess #1!   "+e.ToString()+" [sender="+sender.GetType().ToString()+"]");

            InputEventArgs inputArgs = e.StagingItem.Input;
            CoreLogger.LogStatus(" Staging item: " + inputArgs.RoutedEvent.Name + " [" + inputArgs.Timestamp + "]");
            if (InputHelper.IsInputReport(inputArgs.RoutedEvent) || InputHelper.IsPreviewInputReport(inputArgs.RoutedEvent))
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;
                CoreLogger.LogStatus(" Report:       mode=" + ir.Mode + ", type=" + ir.Type.ToString() + " [" + ir.Timestamp + "]");
            }
            _postEventHandlerLog.Add(_processHandlers[0]);
        }

        private void Second_PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            CoreLogger.LogStatus("Postprocess #2!   "+e.ToString()+" [sender="+sender.GetType().ToString()+"]");

            InputEventArgs inputArgs = e.StagingItem.Input;
            CoreLogger.LogStatus(" Staging item: " + inputArgs.RoutedEvent.Name + " [" + inputArgs.Timestamp + "]");
            if (InputHelper.IsInputReport(inputArgs.RoutedEvent) || InputHelper.IsPreviewInputReport(inputArgs.RoutedEvent))
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;
                CoreLogger.LogStatus(" Report:       mode=" + ir.Mode + ", type=" + ir.Type.ToString() + " [" + ir.Timestamp + "]");
            }
            _postEventHandlerLog.Add(_processHandlers[1]);
        }

        private void First_PreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            CoreLogger.LogStatus("Preprocess #1!    "+e.ToString()+" [sender="+sender.GetType().ToString()+"]");
            CoreLogger.LogStatus(" Canceled?     "+e.Canceled);

            InputEventArgs inputArgs = e.StagingItem.Input;
            CoreLogger.LogStatus(" Staging item: " + inputArgs.RoutedEvent.Name + " [" + inputArgs.Timestamp + "]");
            if (InputHelper.IsInputReport(inputArgs.RoutedEvent) || InputHelper.IsPreviewInputReport(inputArgs.RoutedEvent))
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;
                CoreLogger.LogStatus(" Report:       mode=" + ir.Mode + ", type=" + ir.Type.ToString() + " [" + ir.Timestamp + "]");
            }
            _preEventHandlerLog.Add(_preProcessHandlers[0]);
        }

        private void Second_PreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            CoreLogger.LogStatus("Preprocess #2!    "+e.ToString()+" [sender="+sender.GetType().ToString()+"]");
            CoreLogger.LogStatus(" Canceled?     "+e.Canceled);

            InputEventArgs inputArgs = e.StagingItem.Input;
            CoreLogger.LogStatus(" Staging item: " + inputArgs.RoutedEvent.Name + " [" + inputArgs.Timestamp + "]");
            if (InputHelper.IsInputReport(inputArgs.RoutedEvent) || InputHelper.IsPreviewInputReport(inputArgs.RoutedEvent))
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;
                CoreLogger.LogStatus(" Report:       mode=" + ir.Mode + ", type=" + ir.Type.ToString() + " [" + ir.Timestamp + "]");
            }
            _preEventHandlerLog.Add(_preProcessHandlers[1]);
        }

    }
}
