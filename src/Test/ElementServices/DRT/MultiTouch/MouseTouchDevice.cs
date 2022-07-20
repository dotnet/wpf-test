// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace DRT
{
    public class MouseTouchDevice : TouchDevice
    {
        public static void UseMouse()
        {
            if (_mouseDevice == null)
            {
                _rightIsDown = false;
                _mouseDevice = new MouseTouchDevice(0);
                _rightMouseDevice = new TestTouchDevice(1);
                InputManager.Current.PostProcessInput += new ProcessInputEventHandler(_mouseDevice.PostProcessInput);
            }
        }

        public static void StopMouse()
        {
            if (_mouseDevice != null)
            {
                InputManager.Current.PostProcessInput -= new ProcessInputEventHandler(_mouseDevice.PostProcessInput);
                _mouseDevice = null;
                _rightMouseDevice = null;
            }
        }

        private void PostProcessInput(object sender, ProcessInputEventArgs e)
        {
            InputEventArgs inputEventArgs = e.StagingItem.Input;
            if ((inputEventArgs != null) && !inputEventArgs.Handled && (inputEventArgs.Device == Mouse.PrimaryDevice))
            {
                if (inputEventArgs.RoutedEvent == Mouse.MouseMoveEvent)
                {
                    if (!_rightIsDown)
                    {
                        OnMove();
                    }
                }
                else if (inputEventArgs.RoutedEvent == Mouse.MouseDownEvent)
                {
                    MouseButton button = ((MouseButtonEventArgs)inputEventArgs).ChangedButton;
                    if (button == MouseButton.Left)
                    {
                        if (!_leftIsDown)
                        {
                            UpdateActiveSource();
                            OnActivate();
                            OnDown();
                            _leftIsDown = true;
                        }
                    }
                    else if (button == MouseButton.Right)
                    {
                        if (_rightIsDown)
                        {
                            _rightMouseDevice.OnUp();
                            _rightMouseDevice.OnDeactivate();
                            _rightIsDown = false;
                        }
                        else
                        {
                            _rightIsDown = true;
                            _rightMouseDevice.CurrentTouchPoint = GetMouseTouchPoint(_rightMouseDevice, null, TouchAction.Down); 
                            _rightMouseDevice.UpdateActiveSource(Mouse.PrimaryDevice.ActiveSource);
                            _rightMouseDevice.OnActivate();
                            _rightMouseDevice.OnDown();
                        }
                    }
                }
                else if (inputEventArgs.RoutedEvent == Mouse.MouseUpEvent)
                {
                    MouseButton button = ((MouseButtonEventArgs)inputEventArgs).ChangedButton;
                    if (button == MouseButton.Left && _leftIsDown)
                    {
                        OnUp();
                        OnDeactivate();
                        _leftIsDown = false;
                    }
                }
            }
        }

        private static MouseTouchDevice _mouseDevice;
        private static TestTouchDevice _rightMouseDevice;
        private static bool _rightIsDown;
        private static bool _leftIsDown;

        public MouseTouchDevice(int id)
            : base(id)
        {
        }

        /// <summary>
        ///     Provides the current position.
        /// </summary>
        /// <param name="relativeTo">Defines the coordinate space.</param>
        /// <returns>The current position in the coordinate space of relativeTo.</returns>
        public override TouchPoint GetTouchPoint(IInputElement relativeTo)
        {
            return GetMouseTouchPoint(this, relativeTo, _lastAction);
        }

        private static TouchPoint GetMouseTouchPoint(TouchDevice device, IInputElement relativeTo, TouchAction action)
        {
            Point position = Mouse.GetPosition(relativeTo);
            return new TouchPoint(device, position, new Rect(position.X, position.Y, 1.0, 1.0), action);
        }

        /// <summary>
        ///     Provides all of the known points the device hit since the last reported position update.
        /// </summary>
        /// <param name="relativeTo">Defines the coordinate space.</param>
        /// <returns>A list of points in the coordinate space of relativeTo.</returns>
        public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
        {
            TouchPointCollection points = new TouchPointCollection();

            TouchPoint point = GetTouchPoint(relativeTo);
            if (point != null)
            {
                points.Add(point);
            }

            return points;
        }

        private void UpdateActiveSource()
        {
            SetActiveSource(Mouse.PrimaryDevice.ActiveSource);
        }

        private void OnActivate()
        {
            Activate();
        }

        private void OnDeactivate()
        {
            Deactivate();
        }

        private void OnDown()
        {
            _lastAction = TouchAction.Down;
            ReportDown();
        }

        private void OnMove()
        {
            _lastAction = TouchAction.Move;
            ReportMove();
        }

        private void OnUp()
        {
            _lastAction = TouchAction.Up;
            ReportUp();
        }

        private TouchAction _lastAction = TouchAction.Move;
    }
}
