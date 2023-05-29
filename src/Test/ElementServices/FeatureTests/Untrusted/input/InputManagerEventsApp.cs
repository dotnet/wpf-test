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
using Microsoft.Test.Input;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify InputManager notification events fire on no input.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events. Invoked by test extender BasicInputTests.txr.
    /// </description>
    /// <author>Microsoft</author>
 
    public class InputManagerEventsApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "InputManagerEventsApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new InputManagerEventsApp(), "Run");

        }

        /// <summary>
        /// 
        /// </summary>
        public InputManagerEventsApp()
            : base()
        {
            // SPECIAL NOTE: According to breaking change ,
            // in order for activation input events to be heard,
            // activation input event handlers must be attached BEFORE creating the source.
            // Also, in the browser container, 
            // the PresentationHost is initialized and raises the events on a source before the case runs.
            // Hence, we attach these handlers in the app constructor, not the DoSetup phase.        

            CoreLogger.LogStatus("Attaching InputManager event handlers....");
            InputManagerHelper.CurrentHelper.PostNotifyInput += new NotifyInputEventHandler(Current_PostNotifyInput);
            InputManagerHelper.CurrentHelper.PostProcessInput += new ProcessInputEventHandler(Current_PostProcessInput);
            InputManagerHelper.CurrentHelper.PreNotifyInput += new NotifyInputEventHandler(Current_PreNotifyInput);
            InputManagerHelper.CurrentHelper.PreProcessInput += new PreProcessInputEventHandler(Current_PreProcessInput);
        
       }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");
            
            // Construct test element, add nonessential event handling
            InstrPanel panel = new InstrPanel();
            panel.Focusable = true;
            panel.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            panel.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            panel.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);
            panel.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnFocus);

            CoreLogger.LogStatus("Setting startup focus to our root element...");
            bool bFocus1 = panel.Focus();
            CoreLogger.LogStatus("Did focus succeed on root element? (expect true) " + bFocus1);

            DisplayMe(panel, 50, 50, 200, 200);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            //
            // Set focus to element.
            // Event handlers should be called and recorded.
            //
            CoreLogger.LogStatus("Setting focus to our root element...");
            bool bFocus1 = _rootElement.Focus();
            CoreLogger.LogStatus("Did focus succeed on root element? " + bFocus1.ToString());

            DispatcherHelper.DoEvents(1);

            //
            // Remove event handlers and set focus to null.
            // Event handlers should not be called.
            //
            CoreLogger.LogStatus("Removing InputManager event handlers...");
            InputManagerHelper.CurrentHelper.PostNotifyInput -= new NotifyInputEventHandler(Current_PostNotifyInput);
            InputManagerHelper.CurrentHelper.PostProcessInput -= new ProcessInputEventHandler(Current_PostProcessInput);
            InputManagerHelper.CurrentHelper.PreNotifyInput -= new NotifyInputEventHandler(Current_PreNotifyInput);
            InputManagerHelper.CurrentHelper.PreProcessInput -= new PreProcessInputEventHandler(Current_PreProcessInput);

            CoreLogger.LogStatus("Removing focus - set to null...");
            MouseHelper.Move(_rootElement, MouseLocation.Center, true);
            MouseHelper.Click(_rootElement);

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

            // For this test we expect 2 events (PreviewInputReport, InputReport) 
            // x 4 preview report and input handlers
            // = 8 event log items

            CoreLogger.LogStatus("Total InputManager Events found: " + _eventLog.Count);
            CoreLogger.LogStatus("Report Events found: (expect 8) " + _reportEventLog.Count);

            // expect non-negative event count
            int actual = (_reportEventLog.Count);
            int expected = 8;
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
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello focusing from: " + e.OldFocus + " to " + e.NewFocus);

            // Continue routing this event (don't want to block InputManager events).
            e.Handled = false;
        }

        private void Current_PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            CoreLogger.LogStatus("Postprocess!   " + e.ToString() + " [sender=" + sender.GetType().ToString() + "]");

            CheckInputManager(e);

            InputEventArgs inputArgs = e.StagingItem.Input;
            CoreLogger.LogStatus(" Staging item: " + inputArgs.RoutedEvent.Name + " [" + inputArgs.Timestamp + "]");
            if (InputHelper.IsInputReport(inputArgs.RoutedEvent) || InputHelper.IsPreviewInputReport(inputArgs.RoutedEvent))
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;
                CoreLogger.LogStatus(" Report:       mode=" + ir.Mode + ", type=" + ir.Type.ToString() + " [" + ir.Timestamp + "]");
                _reportEventLog.Add(ir);
            }
            _eventLog.Add(e);
            CoreLogger.LogStatus("===== END");
        }

        private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
        {
            CoreLogger.LogStatus("Postnotify!    " + e.ToString() + " [sender=" + sender.GetType().ToString() + "]");

            CheckInputManager(e);

            InputEventArgs inputArgs = e.StagingItem.Input;
            CoreLogger.LogStatus(" Staging item: " + inputArgs.RoutedEvent.Name + " [" + inputArgs.Timestamp + "]");
            if (InputHelper.IsInputReport(inputArgs.RoutedEvent) || InputHelper.IsPreviewInputReport(inputArgs.RoutedEvent))
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;
                CoreLogger.LogStatus(" Report:       mode=" + ir.Mode + ", type=" + ir.Type.ToString() + " [" + ir.Timestamp + "]");
                _reportEventLog.Add(ir);
            }
            _eventLog.Add(e);
        }

        private void Current_PreNotifyInput(object sender, NotifyInputEventArgs e)
        {
            CoreLogger.LogStatus("Prenotify!     " + e.ToString() + " [sender=" + sender.GetType().ToString() + "]");

            CheckInputManager(e);

            InputEventArgs inputArgs = e.StagingItem.Input;
            CoreLogger.LogStatus(" Staging item: " + inputArgs.RoutedEvent.Name + " [" + inputArgs.Timestamp + "]");
            if (InputHelper.IsInputReport(inputArgs.RoutedEvent) || InputHelper.IsPreviewInputReport(inputArgs.RoutedEvent))
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;
                CoreLogger.LogStatus(" Report:       mode=" + ir.Mode + ", type=" + ir.Type.ToString() + " [" + ir.Timestamp + "]");
                _reportEventLog.Add(ir);
            }
            _eventLog.Add(e);
        }

        private void Current_PreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            CoreLogger.LogStatus("===== BEGIN\nPreprocess!    " + e.ToString() + " [sender=" + sender.GetType().ToString() + "]");
            CoreLogger.LogStatus(" Canceled?     " + e.Canceled);

            CheckInputManager(e);

            InputEventArgs inputArgs = e.StagingItem.Input;
            CoreLogger.LogStatus(" Staging item: " + inputArgs.RoutedEvent.Name + " [" + inputArgs.Timestamp + "]");
            if (InputHelper.IsInputReport(inputArgs.RoutedEvent) || InputHelper.IsPreviewInputReport(inputArgs.RoutedEvent))
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;
                CoreLogger.LogStatus(" Report:       mode=" + ir.Mode + ", type=" + ir.Type.ToString() + " [" + ir.Timestamp + "]");
                _reportEventLog.Add(ir);
            }
            _eventLog.Add(e);
        }

        private void CheckInputManager(NotifyInputEventArgs e)
        {
            if (InputManagerHelper.Current != e.InputManager)
            {
                throw new Microsoft.Test.TestValidationException("NotifyInputEventArgs.InputManager does not match the current InputManager.");
            }
        }

        /// <summary>
        /// Store record of our fired report events.
        /// </summary>
        private List<InputReportWrapper> _reportEventLog = new List<InputReportWrapper>();

        /// <summary>
        /// Store record of our fired IM events.
        /// </summary>
        private List<NotifyInputEventArgs> _eventLog = new List<NotifyInputEventArgs>();
    }
}
