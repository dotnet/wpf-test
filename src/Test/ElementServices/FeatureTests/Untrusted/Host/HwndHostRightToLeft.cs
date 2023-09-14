// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Collections;

using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;

using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Threading;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;

namespace Avalon.Test.CoreUI.Hosting
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///</remarks>
    [TestDefaults]
    public class HwndHostRightToLeft
    {
        Window _win = null;
        StackPanel _panel = null;
        Canvas _canvas = null;
        Win32ButtonCtrl _host = null;

        Button _leftButton;     // Names are relative to Left to Right layout (the left button should appear on the right in RTL).
        Button _rightButton;

        /// <summary>
        /// Test setting RightToLeft (and other) FlowDirections on a DockPanel or StackPanel containing
        /// an HwndHost.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Test(0, @"Hosting\RightToLeft", TestCaseSecurityLevel.FullTrust, "HwndHost FlowDirection RightToLeft test", Area = "AppModel")]
        public void Run()
        {
            CoreLogger.BeginVariation();
            using (CoreLogger.AutoStatus("HwndHost in RightToLeft layout"))
            {
                CreateRightToLeftWindow();


                if (VerifyRightToLeftLayout() == false)
                {
                    CoreLogger.LogTestResult(false, "Incorrect right to left layout :(");
                    return;
                }

                if (MouseVerifyRightToLeftLayout() == false)
                {
                    return;
                }

                // Helpful pause.
                Microsoft.Test.Threading.DispatcherHelper.DoEvents(1000);
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Verify correct layout by directly comparing screen coordinates of the controls.
        /// </summary>
        /// <returns></returns>
        private bool VerifyRightToLeftLayout()
        {
            CoreLogger.LogStatus("left button x: " + GetXOffsetInWindow(_leftButton));
            CoreLogger.LogStatus("host button x: " + GetXOffsetInWindow(_host));
            CoreLogger.LogStatus("right button x: " + GetXOffsetInWindow(_rightButton));

            if (GetXOffsetInWindow(_rightButton) > GetXOffsetInWindow(_host))
            {
                CoreLogger.LogTestResult(false, "Right button is not left of Host in RTL layout.");
                return false;
            }

            if (GetXOffsetInWindow(_host) > GetXOffsetInWindow(_leftButton))
            {
                CoreLogger.LogTestResult(false, "Left button should be right of Host in RTL layout.");
                return false;
            }

            CoreLogger.LogStatus("Controls have correct relative screen coordinates", ConsoleColor.Green);

            return true;
        }

        private double GetXOffsetInWindow(Visual v)
        {
            return GetXOffset(v, _win);
        }

        /// <summary>
        /// Return x offset of a visual. Convenient wrapper for TransformToVisual.
        /// </summary>
        private double GetXOffset(Visual v, Visual t)
        {
            Transform feTransform = (Transform)v.TransformToVisual(t);            

            return (feTransform.Value).OffsetX;
        }

        private List<object> _eventList;

        /// <summary>
        /// Verify correct layout by moving the mouse over the controls from left to right and
        /// comparing the events recorded by their move handler.
        /// </summary>
        /// <returns></returns>
        private bool MouseVerifyRightToLeftLayout()
        {
            _eventList = new List<object>();

            // Move mouse across controls.
            MouseHelper.MoveOutside((UIElement)_rightButton, MouseLocation.CenterLeft);
            MouseHelper.MoveOutside((UIElement)_leftButton, MouseLocation.CenterRight);


            CoreLogger.LogStatus("Recorded events:");
            foreach(object e in _eventList)
            {
                CoreLogger.LogStatus("    " + e.ToString());
            }

            if (_eventList.Count != 3)
            {
                CoreLogger.LogTestResult(false, "Incorrect number of move events recorded, expected 3 recorded " + _eventList.Count);
                return false;
            }

            if (_eventList[0] != _rightButton)
            {
                CoreLogger.LogTestResult(false, "First move event was recorded on " + _eventList[0].ToString() + ", expected right button");
                return false;
            }

            if (_eventList[1] != _host)
            {
                CoreLogger.LogTestResult(false, "Second move event was recorded on " + _eventList[1].ToString() + ", expected host");
                return false;
            }

            if (_eventList[2] != _leftButton)
            {
                CoreLogger.LogTestResult(false, "Third move event was recorded on " + _eventList[2].ToString() + ", expected right button");
                return false;
            }

            CoreLogger.LogStatus("Correct mouse events recorded", ConsoleColor.Green);

            return true;
        }

        private IMouseEvents _controlEvents;

        private void CreateRightToLeftWindow()
        {
            _win = new Window();
            _canvas = new Canvas();
            _panel = new StackPanel();

            _panel.Background = System.Windows.Media.Brushes.Green;
            _panel.Orientation = Orientation.Horizontal;
            _panel.FlowDirection = FlowDirection.RightToLeft;

            //
            // Create left button, should appear on the right side of the container in RTL layout.
            //
            _leftButton = new Button();
            _leftButton.Content = "First Button";
            _leftButton.Name = "_leftButton";
            _leftButton.Width = 250;

            ((UIElement)_leftButton).AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(MouseMoveHandler), true);

            _panel.Children.Add(_leftButton);

            //
            // Create hwnd host.
            //
            _host = new Win32ButtonCtrl();
            _host.Width = 100;
            _host.Name = "host";

            _controlEvents = (IMouseEvents)_host;
            _controlEvents.MouseMove += new EventHandler(MouseMoveHandler);

            _panel.Children.Add(_host);

            //
            // Create right button, should appear on the left side of the container in RTL layout.
            //
            _rightButton = new Button();
            _rightButton.Content = "Last Button";
            _rightButton.Name = "_rightButton";
            _rightButton.Width = 250;

            ((UIElement)_rightButton).AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(MouseMoveHandler), true);

            _panel.Children.Add(_rightButton);

            _canvas.Children.Add(_panel);

            _win.Content = _canvas;

            _win.Show();
        }

        void GetHwndHostScreenPoint()
        {
            NativeStructs.RECT rcHwndPos = new NativeStructs.RECT();
            NativeMethods.GetWindowRect(new HandleRef(null, _host.Handle), ref rcHwndPos);

            CoreLogger.LogStatus("HWND Window Rect Point X " + rcHwndPos.left);
            CoreLogger.LogStatus("HWND Window Rect Point Y " + rcHwndPos.top);

        }

        /// <summary>
        /// Record control sending move message if different from the previous control.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        void MouseMoveHandler(object o, EventArgs args)
        {
            if ((_eventList.Count == 0) || (_eventList[_eventList.Count - 1] != o))
            {
                CoreLogger.LogStatus("Move handler, adding " + o);
                _eventList.Add(o);
            }
        }



    }
}







