// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for mouse selection on TextBox and RichTextBox controls.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Input/MouseEvents.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Microsoft.Test.Display;

    #endregion Namespaces.

    /// <summary>Verifies that mouse events are fired correctly.</summary>
    /// <remarks>
    /// Input is generated and events are verified. Only Preview events
    /// are verified, as the TextEditor may choose to handle some events
    /// itself. Also verifies that events can be programmatically raised.
    /// </remarks>
    [Test(0, "Input", "MouseEvents", MethodParameters="/TestCaseType=MouseEvents /InputMonitorEnabled:False ", Timeout=300)]
    [TestOwner("Microsoft"), TestTactics("344, 345"), TestBugs("494,495"), TestLastUpdatedOn("June 12, 2006")]
    public class MouseEvents: CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _box = new TextBox();
            _box.Background = Brushes.Red;
            _insideX += (int) MainWindow.Left;
            _insideY += (int) MainWindow.Top;
            MainWindow.Content = _box;
            HookMouseEvents();

            // After hooking up all loggers, allow manual investigation
            // when launched from command line.
            _investigating = Settings.GetArgumentAsBool("Investigating");
            if (_investigating)
            {
                return;
            }

            QueueHelper.Current.QueueDelegate(WaitForInitialLayout);
        }

        private void WaitForInitialLayout()
        {
            Log("Moving mouse into the window...");
            MouseInput.MouseDrag(40, 200, _insideX, _insideY);
            QueueHelper.Current.QueueDelegate(CheckMovedIn);
        }

        private void HookMouseEvents()
        {
            _box.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(TextBoxMouseDown);
            _box.MouseEnter += new MouseEventHandler(TextBoxMouseEnter);
            _box.MouseLeave += new MouseEventHandler(TextBoxMouseLeave);
            _box.PreviewMouseMove += new MouseEventHandler(TextBoxMouseMove);
            _box.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(TextBoxMouseUp);
            _box.PreviewMouseWheel += new MouseWheelEventHandler(TextBoxMouseWheel);
        }

        private void TextBoxMouseDown(object sender, MouseButtonEventArgs args)
        {
            Log("PreviewMouseDown event fired");
            _mouseDownFired = true;
        }

        private void TextBoxMouseEnter(object sender, MouseEventArgs args)
        {
            Log("MouseEnter event fired");
            _mouseEnterFired = true;
        }

        private void TextBoxMouseLeave(object sender, MouseEventArgs args)
        {
            Log("MouseLeave event fired");
            _mouseLeaveFired = true;
        }

        private void TextBoxMouseMove(object sender, MouseEventArgs args)
        {
            Log("PreviewMouseMove event fired");
            _mouseMoveFired = true;
        }

        private void TextBoxMouseUp(object sender, MouseButtonEventArgs args)
        {
            Log("PreviewMouseUp event fired");
            _box.Text = "text modified";
            _mouseUpFired = true;
        }

        private void TextBoxMouseWheel(object sender, MouseWheelEventArgs args)
        {
            Log("PreviewMouseWheel event fired");
            _mouseWheelFired = true;
        }

        private void CheckMovedIn()
        {
            Verifier.Verify(_mouseEnterFired, "MouseEnter event fired.", true);
            Verifier.Verify(_mouseMoveFired, "PreviewMouseMove event fired.", true);
            Verifier.Verify(!_mouseDownFired, "PreviewMouseDown event not fired yet.", true);
            Verifier.Verify(!_mouseUpFired, "PreviewMouseUp event not fired yet.", true);

            Log("Clicking with the left button...");
            _mouseEnterFired = false;
            _mouseMoveFired = false;
            MouseInput.MouseClick();
            QueueHelper.Current.QueueDelegate(CheckClicked);
        }

        private void CheckClicked()
        {
            Verifier.Verify(_mouseDownFired, "PreviewMouseDown event fired.", true);
            Verifier.Verify(_mouseUpFired, "PreviewMouseUp event fired.", true);

            _mouseDownFired = false;
            _mouseUpFired = false;

            // These may get set by mouse capturing.
            _mouseMoveFired = false;
            _mouseEnterFired = false;

            Log("Rotating the mouse wheel...");
            MouseInput.MouseWheel(2);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckWheel));
        }

        private void CheckWheel()
        {
            Verifier.Verify(_mouseWheelFired, "PreviewMouseWheel event fired.", true);

            Log("Moving the mouse outside the window...");
            //MouseInput.MouseDrag(insideX, insideY,
            //    (int)(MainWindow.RenderSize.Width) + 30, insideY);
            MouseInput.MouseClick((int)(MainWindow.RenderSize.Width) -100, 5);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckMovedOut));
        }

        private void CheckMovedOut()
        {
            Verifier.Verify(!_mouseEnterFired, "MouseEnter event not fired when leaving.", true);
            Verifier.Verify(_mouseLeaveFired, "MouseLeave event fired.", true);

            CheckProgrammaticEvents();

            Logger.Current.ReportSuccess();
        }

        private void CheckProgrammaticEvents()
        {
            TextBox textBox;        // Text box to test.
            RoutedEventArgs args;   // Routed event arguments to raise.

            Log("Checking programmatic events can be handled...");

            textBox = new TextBox();
            args = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left);
            args.RoutedEvent=Mouse.MouseDownEvent;
            args.Source=textBox;

            textBox.RaiseEvent(args);
        }

        #region Private fields.

        private TextBox _box;
        private int _insideX = 40;
        private int _insideY = 80;
        private bool _investigating;

        private bool _mouseDownFired = false;
        private bool _mouseEnterFired = false;
        private bool _mouseLeaveFired = false;
        private bool _mouseMoveFired = false;
        private bool _mouseUpFired = false;
        private bool _mouseWheelFired = false;

        #endregion Private fields.
    }
}
