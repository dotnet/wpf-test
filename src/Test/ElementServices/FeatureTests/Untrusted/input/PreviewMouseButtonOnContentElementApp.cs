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
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using System.Windows.Markup;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Click mouse buttons and verify that Event handlers onePreviewMouseLeftButtonDown, 
    /// PreviewMouseRightButtonDown, PreviewMouseLeftButtonUp, and PreviewMouseRightButtonUp have been 
    /// called event ButtonDown/UP change Handled to true.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class PreviewMouseButtonOnContentElementApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse\TunnelCe", "HwndSource", @"Compile and Verify MouseButton tunnel events on ContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse\TunnelCe", "Browser", @"Compile and Verify MouseButton tunnel events on ContentElement in Browser.")]
        [TestCase("2", @"CoreInput\Mouse\TunnelCe", "Window", @"Compile and Verify MouseButton tunnel events on ContentElement in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "PreviewMouseButtonOnContentElementApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Mouse\TunnelCe", "HwndSource", @"Verify MouseButton tunnel events on ContentElement in HwndSource.")]
        [TestCase("1", @"CoreInput\Mouse\TunnelCe", "Window", @"Verify MouseButton tunnel events on ContentElement in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new PreviewMouseButtonOnContentElementApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");

            // Construct test element and child element
            InstrContentPanelHost host = new InstrContentPanelHost();
            InstrContentPanel contentElement = new InstrContentPanel("rootLeaf", "Sample", host);

            host.AddChild(contentElement);


            //Add event handler to MouseDown and MouseUp to change handled to be true.
            host.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
            host.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));
            contentElement.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
            contentElement.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));

            //Add handlers to clr events
            host.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnParentPreviewMouseLeftButtonUp);
            host.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnParentPreviewMouseLeftButtonDown);
            host.PreviewMouseRightButtonUp += new MouseButtonEventHandler(OnParentPreviewMouseRightButtonUp);
            host.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnParentPreviewMouseRightButtonDown);

            contentElement.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonUp);
            contentElement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown);
            contentElement.PreviewMouseRightButtonUp += new MouseButtonEventHandler(OnChildPreviewMouseRightButtonUp);
            contentElement.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnChildPreviewMouseRightButtonDown);

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
                    MouseHelper.Click(MouseButton.Middle, _rootElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton1,_rootElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton2, _rootElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Right, _rootElement);
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

            // Note: for this test we are concerned about whether events fire for each button press.
            // 5 down = 5 events
            // We are also need to inspect the proper button state properties corresponding to each event.

            CoreLogger.LogStatus("Events found: (expect 10) " + _buttonLog.Count);

            if (!_parentPreviewMouseLeftButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseLeftButtonUp event on the Parent has not been called.");
            }
            if (!_parentPreviewMouseLeftButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseLeftButtonDown event on the Parent has not been called.");
            }
            if (!_parentPreviewMouseRightButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseRightButtonDown event on the Parent has not been called.");
            }
            if (!_parentPreviewMouseRightButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseRightButtonUp event on the Parent has not been called.");
            }

            bool parentEventsCalled = _parentPreviewMouseLeftButtonUpCalled && 
                _parentPreviewMouseLeftButtonDownCalled &&
                _parentPreviewMouseRightButtonDownCalled &&
                _parentPreviewMouseRightButtonUpCalled;

            if (!_childPreviewMouseLeftButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseLeftButtonUp event on the Child has been called.");
            }
            if (!_childPreviewMouseLeftButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseLeftButtonDown event on the Child has been called.");
            }
            if (!_childPreviewMouseRightButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseRightButtonDown event on the Child has been called.");
            }
            if (!_childPreviewMouseRightButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseRightButtonUp event on the Child has been called.");
            }

            bool childEventsCalled = _childPreviewMouseLeftButtonUpCalled && 
                _childPreviewMouseLeftButtonDownCalled &&
                _childPreviewMouseRightButtonDownCalled &&
                _childPreviewMouseRightButtonUpCalled;

            Assert(parentEventsCalled && childEventsCalled, "Error - not all events raised properly");

            this.TestPassed = true;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private void OnParentPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _parentPreviewMouseLeftButtonUpCalled = true;
        }
        bool _parentPreviewMouseLeftButtonUpCalled = false;

        private void OnParentPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _parentPreviewMouseLeftButtonDownCalled = true;
        }
        bool _parentPreviewMouseLeftButtonDownCalled = false;

        private void OnParentPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _parentPreviewMouseRightButtonUpCalled = true;
        }
        bool _parentPreviewMouseRightButtonUpCalled = false;

        private void OnParentPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _parentPreviewMouseRightButtonDownCalled = true;
        }
        bool _parentPreviewMouseRightButtonDownCalled = false;


        private void OnChildPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _childPreviewMouseLeftButtonUpCalled = true;
        }
        bool _childPreviewMouseLeftButtonUpCalled = false;

        private void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _childPreviewMouseLeftButtonDownCalled = true;
        }
        bool _childPreviewMouseLeftButtonDownCalled = false;

        private void OnChildPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _childPreviewMouseRightButtonUpCalled = true;
        }
        bool _childPreviewMouseRightButtonUpCalled = false;

        private void OnChildPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _childPreviewMouseRightButtonDownCalled = true;
        }
        bool _childPreviewMouseRightButtonDownCalled = false;


        /// <summary>
        /// Standard mouse event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("Button=" + e.ChangedButton.ToString() + ",State=" + e.ButtonState.ToString() + ",ClickCount=" + e.ClickCount);
            CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: " +
                                e.LeftButton.ToString() + "," +
                                e.RightButton.ToString() + "," +
                                e.MiddleButton.ToString() + "," +
                                e.XButton1.ToString() + "," +
                                e.XButton2.ToString()
                                );

            // Store a record of our buttons pressed.
            _buttonLog.Add(e.ChangedButton);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired buttons.
        /// </summary>
        private List<MouseButton> _buttonLog = new List<MouseButton>();
    }
}
