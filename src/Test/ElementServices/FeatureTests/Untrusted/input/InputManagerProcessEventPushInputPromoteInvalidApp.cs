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
    /// Verify PushInput(InputEventArgs,promote) fails to put invalid staging item into staging area.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <



    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class InputManagerProcessEventPushInputPromoteInvalidApp : TestApp
    {


        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All","All","2",@"CoreInput\InputManager",TestCaseSecurityLevel.FullTrust,"Verify PPushInput(InputEventArgs,promote) fails to put invalid staging item into staging area.")]
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
                // Attaching InputManagerEvents
                InputManagerHelper.CurrentHelper.PreProcessInput += new PreProcessInputEventHandler(OnPreProcess);
                InputManagerHelper.CurrentHelper.PostProcessInput += new ProcessInputEventHandler(OnProcess);

                // Construct related Win32 window


                // Construct test element, add event handling
                _rootElement = new InstrPanel();

                // Put the test element on the screen
                DisplayMe(_rootElement, 10, 10, 100, 100);
            }

            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);
            return _hwnd;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            // Right-click test window to trigger Input Manager event collection.
            InputCallback[] ops = new InputCallback[] {
                delegate
                {
                    MouseHelper.Click(MouseButton.Right,_rootElement);
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

            // For this test we expect 3 staging area input items.
            // First item is valid (non-null), but has null Input.
            // Second and third items are also valid, but have non-null input representing the MouseWheel push input.
            // Second item is return value from PreProcessInputEventArgs.PushInput.
            // Second item is return value from ProcessInputEventArgs.PeekInput.
            CoreLogger.LogStatus("Events found: " + _stagingItemLog.Count);

            // expect non-negative staging area item count. First one has null input, rest don't. 
            bool actual = (_stagingItemLog.Count == 3) && 
                 (_stagingItemLog[0] != null) && (((StagingAreaInputItem)_stagingItemLog[0]).Input == null) && 
                 (_stagingItemLog[1] != null) && (((StagingAreaInputItem)_stagingItemLog[1]).Input is MouseWheelEventArgs) && 
                 (_stagingItemLog[2] != null) && (((StagingAreaInputItem)_stagingItemLog[2]).Input is MouseWheelEventArgs)
                 ;
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            CoreLogger.LogStatus("Validation complete!");
            return null;
        }

        /// <summary>
        /// Standard PreProcessInput event handler.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnPreProcess(object sender, PreProcessInputEventArgs e)
        {
            // Make sure we have a Button2Press before running Push Test
            // InputManager, StagingItem, Input
            RoutedEvent reid = e.StagingItem.Input.RoutedEvent;

            // Input Report stuff
            if (InputHelper.IsPreviewInputReport(reid))
            {
                InputEventArgs inputArgs = e.StagingItem.Input;
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;

                if (ir.Name == "RawMouseInputReport")
                {
                    RawMouseInputReportWrapper rkim = new RawMouseInputReportWrapper(ir);

                    if ((rkim.Actions & Microsoft.Test.Input.RawMouseActions.Button2Press) != 0)
                    {
                        // Push Test
                        CoreLogger.LogStatus(" [ ... PreProcessInput ... ]");

                        // Peek at our current staging item
                        CoreLogger.LogStatus("Peeking...."); 
                        StagingAreaInputItem saii = e.PeekInput();
                        CoreLogger.LogStatus("Peeked item exists? {" + (saii != null) + "}");
                        if (saii != null)
                        {
                            CoreLogger.LogStatus("Peeked item input exists? [" + (saii.Input != null) + "]");
                        }

                        // Create valid argument package to be pushed in.
                        InputManager im = sender as InputManager;
                        MouseWheelEventArgs wargs = new MouseWheelEventArgs(im.PrimaryMouseDevice, Environment.TickCount, 0);
                        wargs.RoutedEvent= Mouse.MouseWheelEvent;

                        // Push in our args plus a promotion from the current staging item.
                        CoreLogger.LogStatus("Pushing....");
                        StagingAreaInputItem saiiPush = e.PushInput(wargs, saii);
                        CoreLogger.LogStatus("Peeked item exists after push? {" + (saiiPush != null) + "}");
                        if (saiiPush != null)
                        {
                            CoreLogger.LogStatus("Peeked item input exists? [" + (saiiPush.Input != null) + "]");
                        }

                        _stagingItemLog.Add(saii);
                        _stagingItemLog.Add(saiiPush);
                    }
                }
            }
        }

        /// <summary>
        /// Standard PostProcessInput event handler.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnProcess(object sender, ProcessInputEventArgs e)
        {
            // Make sure we have a Button2Press before running Peek Test
            // InputManager, StagingItem, Input
            RoutedEvent reid = e.StagingItem.Input.RoutedEvent;

            // Input Report stuff
            if (InputHelper.IsPreviewInputReport(reid))
            {
                InputEventArgs inputArgs = e.StagingItem.Input;
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;

                if (ir.Name == "RawMouseInputReport")
                {
                    RawMouseInputReportWrapper rkim = new RawMouseInputReportWrapper(ir);

                    if ((rkim.Actions & Microsoft.Test.Input.RawMouseActions.Button2Press) != 0)
                    {
                        // Peek Test
                        CoreLogger.LogStatus(" [ ... PostProcessInput ... ]");

                        CoreLogger.LogStatus("Peeking...."); 
                        StagingAreaInputItem saii = e.PeekInput();

                        _stagingItemLog.Add(saii);
                        CoreLogger.LogStatus("Peeked item exists? {" + (saii == null) + "}");
                        if (saii != null)
                        {
                            CoreLogger.LogStatus("Peeked item input exists? [" + (saii.Input == null) + "]");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Store record of our staging items.
        /// </summary>
        private ArrayList _stagingItemLog = new ArrayList();
    }
}
