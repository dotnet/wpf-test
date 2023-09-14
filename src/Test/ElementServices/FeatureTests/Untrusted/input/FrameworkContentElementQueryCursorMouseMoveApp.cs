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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
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
    /// Verify QueryCursor event is raised when mouse moves over same content element.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkContentElementQueryCursorMouseMoveApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All", "All", "2", @"CoreInput\Cursor", TestCaseSecurityLevel.FullTrust, "Verify QueryCursor event is raised when mouse moves over same content element.")]
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
            CoreLogger.LogStatus("Constructing tree....");
            // Build canvas for this window
            Canvas[] canvases = new Canvas[] { new Canvas() };

            foreach (Canvas cvs in canvases)
            {
                InstrContentPanelHost btn = new InstrContentPanelHost();
                btn.Height = 70.00;
                btn.Width = 70.00;
                Canvas.SetTop(btn, 15.00);
                Canvas.SetLeft(btn, 15.00);

                // Store content element 
                FrameworkContentElement contentElement = new InstrFrameworkContentPanel("rootLeaf", "Sample", btn);

                contentElement.QueryCursor += new QueryCursorEventHandler(OnQuery);
                InputManagerHelper.CurrentHelper.PostProcessInput += new ProcessInputEventHandler(OnProcess);
                (btn).AddChild(contentElement);

                // Add everything to the visual tree
                cvs.Children.Add(btn);
            }

            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            Canvas cvs = (Canvas)_rootElement;
            UIElement panel = cvs.Children[0];

            CoreLogger.LogStatus("Moving to element....");
            MouseHelper.Move(panel);

            base.DoExecute(arg);
            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            // We want our element to have some QueryCursor events fired on it.

            CoreLogger.LogStatus("Events found (expect 1 or more): " + _eventLog.Count);
            Assert(_eventLog.Count >= 1, "Event count not correct");

            // Log final test results
            bool eventFound = true;

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Event handler to run when a cursor is queried.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnQuery(object sender, QueryCursorEventArgs e)
        {
            // Log some debugging data
            FrameworkContentElement fe = sender as FrameworkContentElement;
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] [" + ((fe != null) ? fe.ToString() : "") + "]");
            CoreLogger.LogStatus("   Cursor='" + ((e.Cursor != null) ? e.Cursor.ToString() : "") + "'");

            _eventLog.Add(e);
        }

        /// <summary>
        /// Standard process event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnProcess(object sender, ProcessInputEventArgs e)
        {
            // Use InputManager and StagingItem to find input event
            InputEventArgs inputArgs = e.StagingItem.Input;
            RoutedEvent reid = inputArgs.RoutedEvent;
            CoreLogger.LogStatus("/  InputName      =  " + reid.Name + " [" + inputArgs.Timestamp.ToString() + "]");

            // Input Report stuff
            bool isReport = InputHelper.IsInputReport(reid) || InputHelper.IsPreviewInputReport(reid);
            if (isReport)
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;
                CoreLogger.LogStatus("/    ReportType   =  " + ir.Name);

                if (ir.Name == "RawMouseInputReport")
                {
                    // raw mouse stuff
                    RawMouseInputReportWrapper rkim = new RawMouseInputReportWrapper(ir);

                    CoreLogger.LogStatus("/      Raw        =  '" + rkim.Actions.ToString() + "',X=" + rkim.X + ",Y=" + rkim.Y + ",WHEEL=" + rkim.Wheel);
                }
            }
        }

        private List<QueryCursorEventArgs> _eventLog = new List<QueryCursorEventArgs>();

    }
}
