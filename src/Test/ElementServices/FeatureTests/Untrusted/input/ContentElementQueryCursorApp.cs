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

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify OnQueryCursor raises query cursor event on ContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementQueryCursorApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify OnQueryCursor raises query cursor event on ContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Cursor", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify OnQueryCursor raises query cursor event on ContentElement in Browser.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify OnQueryCursor raises query cursor event on ContentElement in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "ContentElementQueryCursorApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify OnQueryCursor raises query cursor event on ContentElement in HwndSource.")]
        [TestCase("1", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Verify OnQueryCursor raises query cursor event on ContentElement in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementQueryCursorApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            // Construct test element and child element
            InstrContentPanelHost host = new InstrContentPanelHost();
            _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);

            // Add event handling
            _contentElement.QueryCursor += new QueryCursorEventHandler(OnQuery);
            host.AddChild(_contentElement);

            DisplayMe(host, 10, 10, 200, 200);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Raising query cursor event....");
            QueryCursorEventArgs qeArgs = new QueryCursorEventArgs(Mouse.PrimaryDevice, Environment.TickCount);
            qeArgs.RoutedEvent = Mouse.QueryCursorEvent;
            qeArgs.Cursor = Cursors.SizeWE;
            _contentElement.RaiseEvent(qeArgs);

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

            // Note: for this test we are concerned about whether the event was raised.

            CoreLogger.LogStatus("Events found: " + _eventLog.Count);


            QueryCursorEventArgs args = null;
            if (_eventLog.Count > 0)
            {
                args = _eventLog[0];
            }

            // expect non-negative event count and correct text
            bool actual = false;

            if (_eventLog.Count == 1 && args.Cursor != null && args.Cursor == Cursors.SizeWE)
            {
                actual = true;
            }

            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard query cursor event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnQuery(object sender, QueryCursorEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from cursor: '" + args.Cursor.ToString() + "'");

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<QueryCursorEventArgs> _eventLog = new List<QueryCursorEventArgs>();

        private InstrContentPanel _contentElement;
    }
}
