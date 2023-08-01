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
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;



using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test;
using Microsoft.Test.Win32;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify MouseWheel events fire on a mouse wheel for ContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events. Invoked by test extender BasicInputTests.txr.
    /// </description>
    /// <author>Microsoft</author>
 
    public class ContentElementMouseWheelApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "ContentElementMouseWheelApp",
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
            exe.Run(new ContentElementMouseWheelApp(), "Run");

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

            // Construct child element
            _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);
            _contentElement.Focusable = true;
            _contentElement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown);
            _contentElement.MouseWheel += new MouseWheelEventHandler(OnMouseWheel);
            _contentElement.PreviewMouseWheel += new MouseWheelEventHandler(OnPreviewMouseWheel);

            host.AddChild(_contentElement);

            // Put the test element on the screen
            DisplayMe(host, 10, 10, 100, 100);

            return null;
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
                    MouseHelper.Click(_rootElement);
                },
                delegate
                {
                    MouseHelper.MoveWheel(MouseWheelDirection.Forward, 1);
                }                
            };
            return ops;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about whether wheel events fire
            // and the wheel delta is non-zero

            // expect events
            CoreLogger.LogStatus("Events found: " + _eventLog.Count);

            // expect non-zero delta
            int delta = 0;
            if (_eventLog.Count > 1)
            {
                MouseWheelEventArgs meArgs = _eventLog[1] as MouseWheelEventArgs;
                if (meArgs != null)
                {
                    delta = meArgs.Delta;
                }
            }
            CoreLogger.LogStatus("Delta found: " + delta);

            // expect non-negative event count
            bool actual = (_eventLog.Count == 2) && (delta != 0);
            bool expected = true;

            bool bResult = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + bResult);
            this.TestPassed = bResult;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }



        /// <summary>
        /// Standard mouse wheel event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseWheel(object sender, MouseWheelEventArgs args)
        {
            // Set test flag
            CoreLogger.LogStatus(" Adding event: " + args.ToString());
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");

            Point pt = args.GetPosition(null);

            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("   Delta=" + args.Delta);

            // Don't route this event any more.
            args.Handled = true;
        }


        /// <summary>
        /// Standard preview mouse wheel event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            // Set test flag
            CoreLogger.LogStatus(" Adding event: " + args.ToString());
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");

            Point pt = args.GetPosition(null);

            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("   Delta=" + args.Delta);

            // Keep routing this event.
            args.Handled = false;
        }

        /// <summary>
        /// Focus the test content element on click. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CoreLogger.LogStatus("Setting Keyboard focus on content element.");

            ContentElement c = (ContentElement)sender;
            if (c.Focus())
            {
                CoreLogger.LogStatus(" Focus successfully set on content element.");
            }
            else
            {
                throw new TestValidationException("Failed to set focus on target content element.");
            }

        }


        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();

        private InstrContentPanel _contentElement;
    }
}
