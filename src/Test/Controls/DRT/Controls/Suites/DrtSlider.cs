// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Globalization;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace DRT
{
    public enum SliderTest
    {
        Slider,
    }

    public class SliderSuite : DrtTestSuite
    {
        public SliderSuite(SliderTest test) : base(test.ToString())
        {
            Contact = "Microsoft";
            _test = test;
        }

        SliderTest _test;

        public override DrtTest[] PrepareTests()
        {
            Keyboard.Focus(null);

            _mainBorder = new Border();

            _mainBorder.Background = SystemColors.ControlBrush;

            _mainCanvas = new Canvas();
            _mainCanvas.Width = 500;
            _mainCanvas.Height = 500;

            _vslider1 = new Slider();
            _vslider1.Orientation = System.Windows.Controls.Orientation.Vertical;
            _vslider1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(_vslider1_OnValueChanged);
            _vslider1.SetValue(Canvas.LeftProperty, 10d);
            _vslider1.SetValue(Canvas.TopProperty, 10d);
            _vslider1.Value = _vslider1StartValue = 50d;
            _vslider1.SmallChange = 5.0d;
            _vslider1.LargeChange = 20.0d;
            _vslider1.Maximum = 100d;

            _hslider1 = new Slider();
            _hslider1.ValueChanged += new RoutedPropertyChangedEventHandler<double>(_hslider1_OnValueChanged);
            _hslider1.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(_hslider1_OnThumbDragStarted));
            _hslider1.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(_hslider1_OnThumbDragDelta));
            _hslider1.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(_hslider1_OnThumbDragCompleted));
            _hslider1.SetValue(Canvas.LeftProperty, 40d);
            _hslider1.SetValue(Canvas.TopProperty, 10d);
            _hslider1.Value = _hslider1StartValue = 5d;

            _dummyButton = new Button();
            _dummyButton.Content = "Dummy";
            _dummyButton.SetValue(Canvas.LeftProperty, 40d);
            _dummyButton.SetValue(Canvas.TopProperty, 50d);

            _hslider2 = new Slider();
            _hslider2.Minimum = 0d;
            _hslider2.LargeChange = 50d;
            _hslider2.Maximum = 200d;
            _hslider2.Value = 0;
            _hslider2.Width = 104;
            _hslider2.ValueChanged += new RoutedPropertyChangedEventHandler<double>(_hslider2_OnValueChanged);
            _hslider2.SetValue(Canvas.LeftProperty, 40d);
            _hslider2.SetValue(Canvas.TopProperty, 70d);


            _hslider3 = new Slider();
            _hslider3.Minimum = 0d;
            _hslider3.LargeChange = 15d;
            _hslider3.SmallChange = 15d;
            _hslider3.Maximum = 55d;
            _hslider3.Value = 0;
            _hslider3.TickPlacement = TickPlacement.Both;
            _hslider3.IsSnapToTickEnabled = true;
            _hslider3.TickFrequency = 23d;
            _hslider3.Width = 104;
            _hslider3.ValueChanged += new RoutedPropertyChangedEventHandler<double>(_hslider3_OnValueChanged);
            _hslider3.SetValue(Canvas.LeftProperty, 40d);
            _hslider3.SetValue(Canvas.TopProperty, 100d);

            _mainCanvas.Children.Add(_vslider1);
            _mainCanvas.Children.Add(_hslider1);
            _mainCanvas.Children.Add(_dummyButton);
            _mainCanvas.Children.Add(_hslider2);
            _mainCanvas.Children.Add(_hslider3);
            _mainBorder.Child = _mainCanvas;

            System.Windows.Documents.AdornerDecorator adorner = new System.Windows.Documents.AdornerDecorator();
            adorner.Child = _mainBorder;
            DRT.Show(adorner);

            List<DrtTest> tests = new List<DrtTest>();
            if (!DRT.KeepAlive)
            {
                tests.Add(new DrtTest(Start));
                tests.Add(new DrtTest(SliderKeyboardTest));
                tests.Add(new DrtTest(SliderMouseTest));
                tests.Add(new DrtTest(Cleanup));
            }
            return tests.ToArray();
        }

        /// <summary>
        /// First test. Block input and setup timer.
        /// </summary>
        private void Start()
        {
            if (!DRT.KeepAlive)
            {
                _suicideTimer = new DispatcherTimer();
                _suicideTimer.Interval = new TimeSpan(0, 5, 0);
                _suicideTimer.Tick += new EventHandler(OnTimeout);
                _suicideTimer.Start();
            }
        }

        /// <summary>
        /// Last test. Stop the timer and unblock the input.
        /// </summary>
        public void Cleanup()
        {
            if (_suicideTimer != null)
            {
                _suicideTimer.Stop();
            }
        }

        private void SliderKeyboardTest()
        {
            if (DRT.Verbose) Console.WriteLine("\n---Slider Keyboard Tests");

            DRT.ResumeAt(new DrtTest(KeyboardTestProc));
        }

        private void SliderMouseTest()
        {
            if (DRT.Verbose) Console.WriteLine("\n---Slider Mouse Tests");

            DRT.ResumeAt(new DrtTest(MouseTestProc));
        }

        private enum KeyboardTestStep
        {
            Start,          // Make sure that DRT window is active
            PrepareFocus,   // Move focus to first slider
            TestKeyUp,      // Increase small
            TestKeyDown,    // Decrease small
            TestKeyRight,   // Increase small
            TestKeyLeft,    // Decrease small
            TestPageUp,     // Increase large
            TestPageDown,   // Decrease small
            TestTab,        // One Tab, go to next slider

            TestKeyUp2,      // Increase small
            TestKeyDown2,    // Decrease small
            TestKeyRight2,   // Increase small
            TestKeyLeft2,    // Decrease small
            TestPageUp2,     // Increase large
            TestPageDown2,   // Decrease small

            TestTab2,        // One Tab, go to dummy button

            End,             // End keyboard test
        }

        private void KeyboardTestProc()
        {
            if (DRT.Verbose) Console.WriteLine("Keyboard test = " + _keyboardTestStep);

            switch (_keyboardTestStep)
            {
                case KeyboardTestStep.Start:
                    break;

                case KeyboardTestStep.PrepareFocus:
                    _vslider1.Focus();
                    break;

                case KeyboardTestStep.TestKeyUp:
                    if (!_vslider1.IsKeyboardFocused)
                        Console.WriteLine("_vslider1 is not focused!");
                    PressKey(Key.Up);
                    break;

                case KeyboardTestStep.TestKeyDown:
                    PressKey(Key.Down);
                    break;

                case KeyboardTestStep.TestKeyRight:
                    PressKey(Key.Right);
                    break;

                case KeyboardTestStep.TestKeyLeft:
                    PressKey(Key.Left);
                    break;

                case KeyboardTestStep.TestPageUp:
                    PressKey(Key.PageUp);
                    break;

                case KeyboardTestStep.TestPageDown:
                    PressKey(Key.PageDown);
                    break;

                case KeyboardTestStep.TestTab:
                    PressKey(Key.Tab);
                    break;

                case KeyboardTestStep.TestKeyUp2:
                    PressKey(Key.Up);
                    break;

                case KeyboardTestStep.TestKeyDown2:
                    PressKey(Key.Down);
                    break;

                case KeyboardTestStep.TestKeyRight2:
                    PressKey(Key.Right);
                    break;

                case KeyboardTestStep.TestKeyLeft2:
                    PressKey(Key.Left);
                    break;

                case KeyboardTestStep.TestPageUp2:
                    PressKey(Key.PageUp);
                    break;

                case KeyboardTestStep.TestPageDown2:
                    PressKey(Key.PageDown);
                    break;

                case KeyboardTestStep.TestTab2:
                    PressKey(Key.Tab);
                    break;

                case KeyboardTestStep.End:
                    break;
            }
            DRT.ResumeAt(new DrtTest(KeyboardTestVerifyProc));
        }

        private void KeyboardTestVerifyProc()
        {
            string sTest = _keyboardTestStep.ToString();
            switch (_keyboardTestStep)
            {
                case KeyboardTestStep.Start:
                    if (!EnsureInputFocus(false))
                    {
                        // Try to activate main window again
                        if (_retryCount < 10)
                        {
                            if (DRT.Verbose)
                                Console.WriteLine("Cannot activate main window, try again..." + _mouseTestStep);
                            _retryCount++;

                            // Do some special trick to really active the main window
                            _vslider1.Focus();
                            DRT.PressKey(Key.Tab);

                            DRT.ResumeAt(new DrtTest(KeyboardTestProc));
                            return;
                        }
                    }
                    break;

                case KeyboardTestStep.PrepareFocus:
                    // Make sure that _vslider1 is focused
                    if (!_vslider1.IsKeyboardFocused)
                    {
                        // Something wrong, try to set focus on the Slider again.
                        if (_retryCount < 10)
                        {
                            if (DRT.Verbose) Console.WriteLine("Cannot move focus to _vslider1, retry... (Test step:{0})" + _mouseTestStep);
                            _retryCount++;
                            DRT.ResumeAt(new DrtTest(KeyboardTestProc));
                            return;
                        }
                        else
                        {
                            DRT.Assert(false, string.Format("Cannot move focus to _vslider1, Test step:{0}", sTest));
                        }
                    }
                    break;

                case KeyboardTestStep.TestKeyUp:
                    VerifyValue((RangeBase)_vslider1, _vslider1StartValue + _vslider1.SmallChange, sTest);
                    break;

                case KeyboardTestStep.TestKeyDown:
                    VerifyValue((RangeBase)_vslider1, _vslider1StartValue, sTest);
                    break;

                case KeyboardTestStep.TestKeyRight:
                    VerifyValue((RangeBase)_vslider1, _vslider1StartValue + _vslider1.SmallChange, sTest);
                    break;

                case KeyboardTestStep.TestKeyLeft:
                    VerifyValue((RangeBase)_vslider1, _vslider1StartValue, sTest);
                    break;

                case KeyboardTestStep.TestPageUp:
                    VerifyValue((RangeBase)_vslider1, _vslider1StartValue + _vslider1.LargeChange, sTest);
                    break;

                case KeyboardTestStep.TestPageDown:
                    VerifyValue((RangeBase)_vslider1, _vslider1StartValue, sTest);
                    break;

                case KeyboardTestStep.TestTab:
                    DRT.Assert(_hslider1.IsKeyboardFocusWithin, "Expected focus to be at _hslider1");
                    break;

                case KeyboardTestStep.TestKeyUp2:
                    VerifyValue((RangeBase)_hslider1, _hslider1StartValue + _hslider1.SmallChange, sTest);
                    break;

                case KeyboardTestStep.TestKeyDown2:
                    VerifyValue((RangeBase)_hslider1, _hslider1StartValue, sTest);
                    break;

                case KeyboardTestStep.TestKeyRight2:
                    VerifyValue((RangeBase)_hslider1, _hslider1StartValue + _hslider1.SmallChange, sTest);
                    break;

                case KeyboardTestStep.TestKeyLeft2:
                    VerifyValue((RangeBase)_hslider1, _hslider1StartValue, sTest);
                    break;

                case KeyboardTestStep.TestPageUp2:
                    VerifyValue((RangeBase)_hslider1, _hslider1StartValue + _hslider1.LargeChange, sTest);
                    break;

                case KeyboardTestStep.TestPageDown2:
                    VerifyValue((RangeBase)_hslider1, _hslider1StartValue, sTest);
                    break;

                case KeyboardTestStep.TestTab2:
                    DRT.Assert(_dummyButton.IsKeyboardFocusWithin, "Expected focus to be at _dummyButton");
                    break;

                case KeyboardTestStep.End:
                    break;
            }
            if (_keyboardTestStep != KeyboardTestStep.End)
            {
                _keyboardTestStep++;
                DRT.ResumeAt(new DrtTest(KeyboardTestProc));
            }
        }

        private enum MouseTestStep
        {
            Start,
            MoveToHSlider2,
            ClickToMaximum,                // Click Increase Button until we reach maimum value
            DragToTheLeft_Prepare,
            DragToTheLeft_PressMouse,      // Drag to Minimum - Normal HorizontalSlider
            DragToTheLeft_MoveMouseLeft,  // Drag to Minimum - Normal HorizontalSlider
            DragToTheLeft_ReleaseMouse,    // Drag to Minimum - Normal HorizontalSlider
            /*
            DragToTheRight2,    // Drag to Maximum - HorizontalSlider(Min:20, Max:200)
            DragToTheLeft2,     // Drag to Minimum - HorizontalSlider(Min:20, Max:200)*/
            MoveToHSlider3,
            ClickToMaximumTick,
            End,
        }

        private void MouseTestProc()
        {
            if (DRT.Verbose) Console.WriteLine("Mouse test = " + _mouseTestStep);

            switch (_mouseTestStep)
            {
                case MouseTestStep.Start:
                    _retryCount = 0;
                    _hslider2.Focus();
                    break;

                case MouseTestStep.MoveToHSlider2:
                    DRT.MoveMouse(_hslider2, 0.90 + _retryCount * 0.01 , 0.5);
                    break;

                case MouseTestStep.ClickToMaximum:
                    DRT.ClickMouse();
                    DRT.ClickMouse();
                    DRT.ClickMouse();
                    DRT.ClickMouse();
                    break;

                case MouseTestStep.DragToTheLeft_Prepare:
                    _ishslider1GotDragStartedEvent = false;
                    _ishslider1GotDragCompletedEvent = false;
                    _hslider1DragDeltaEventArgs = null;

                    _hslider1_thumb = FindThumb(_hslider1);

                    if (_hslider1_thumb == null)
                    {
                        DRT.Assert(false, "Cannot find Thumb in _hslider1");
                    }
                    DRT.MoveMouse((UIElement)(_latestMouseTarget = _hslider1_thumb), _latestMousePosX = (0.5 + _retryCount + 0.01), _latestMousePosY = 0.5);
                    break;

                case MouseTestStep.DragToTheLeft_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.DragToTheLeft_MoveMouseLeft:
                    // Mouse the mouse to the left 2 pixel
                    DRT.Assert(_latestMouseTarget == _hslider1_thumb, "latestMouseTarget is not a _hslider1_thumb");

                    double thumbWidth = _hslider1_thumb.ActualWidth;
                    double distance = (1.0 / thumbWidth) * 2;
                    DRT.MoveMouse((UIElement)_latestMouseTarget, _latestMousePosX - distance, _latestMousePosY);
                    break;

                case MouseTestStep.DragToTheLeft_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;

/*
                case MouseTestStep.DragToTheRight:
                    break;

                case MouseTestStep.DragToTheLeft:
                    break;
*/
                case MouseTestStep.MoveToHSlider3:
                    DRT.MoveMouse(_hslider3, 0.90 + _retryCount * 0.01 , 0.5);
                    break;

                case MouseTestStep.ClickToMaximumTick:
                    DRT.ClickMouse();
                    DRT.ClickMouse();
                    DRT.ClickMouse();
                    DRT.ClickMouse();
                    break;

                case MouseTestStep.End:
                    _retryCount = 0;
                    break;
            }
            DRT.ResumeAt(new DrtTest(MouseTestVerifyProc));
        }

        private void MouseTestVerifyProc()
        {
            switch (_mouseTestStep)
            {
                case MouseTestStep.Start:
                    break;

                case MouseTestStep.MoveToHSlider2:
                    // Try to move mouse to slider 10 times
                    if (!_hslider2.IsMouseOver)
                    {
                        if (_retryCount < 10)
                        {
                            _retryCount++;
                            DRT.ResumeAt(new DrtTest(MouseTestProc));
                            return;
                        }
                        else
                        {
                            DRT.Assert(false, string.Format("Cannot move mouse to _hslider2, Test step:{0}", _mouseTestStep.ToString()));
                        }
                    }
                    break;

                case MouseTestStep.ClickToMaximum:
                    VerifyValue((RangeBase)_hslider2, 200d, _mouseTestStep.ToString());
                    break;

                case MouseTestStep.DragToTheLeft_Prepare:
                    if (!_hslider1_thumb.IsMouseOver)
                    {
                        if (_retryCount < 10)
                        {
                            _retryCount++;
                            DRT.ResumeAt(new DrtTest(MouseTestProc));
                            return;
                        }
                        else
                        {
                            DRT.Assert(false, string.Format("Cannot move mouse to the Thumb of _hslider1, Test step:{0}", _mouseTestStep.ToString()));
                        }
                    }
                    break;

                case MouseTestStep.DragToTheLeft_PressMouse:
                    // At this point, the Mouse should pressed directly on the Thumb
                    DRT.Assert(_hslider1_thumb.IsDragging, "_hslider1_thumb.IsDragging is not true. Please verify that mouse is over the Thumb and Leftbutton is down.");
                    // and slider got Thumb.DragStarted event
                    DRT.Assert(_ishslider1GotDragStartedEvent, "_hslider1_thumb didn't fire DragStartedEvent.");
                    break;
                case MouseTestStep.DragToTheLeft_MoveMouseLeft:
                    // Vertify that slider got Thumb DragDelta event with correct distance
                    if (DRT.Verbose)
                        Console.WriteLine("_hslider1 DragDeltaEvenArgs: X={0}, Y={1}", _hslider1DragDeltaEventArgs.HorizontalChange, _hslider1DragDeltaEventArgs.VerticalChange);
                    DRT.Assert(_hslider1DragDeltaEventArgs.HorizontalChange == -2, "Mouse didn't move 2 pixel to the left");
                    break;

                case MouseTestStep.DragToTheLeft_ReleaseMouse:
                    // Verify that slider got Thumb.DragCompleted event
                    DRT.Assert(_ishslider1GotDragCompletedEvent, "_hslider1_thumb didn't fire DragCompletedEvent");
                    break;

/*
                    case MouseTestStep.DragToTheRight:
                    break;

                case MouseTestStep.DragToTheLeft:
                    break;
*/
                case MouseTestStep.MoveToHSlider3:
                    // Try to move mouse to slider 10 times
                    if (!_hslider3.IsMouseOver)
                    {
                        if (_retryCount < 10)
                        {
                            _retryCount++;
                            DRT.ResumeAt(new DrtTest(MouseTestProc));
                            return;
                        }
                        else
                        {
                            DRT.Assert(false, string.Format("Cannot move mouse to _hslider3, Test step:{0}", _mouseTestStep.ToString()));
                        }
                    }
                    break;

                case MouseTestStep.ClickToMaximumTick:
                    VerifyValue((RangeBase)_hslider3, 55d, _mouseTestStep.ToString());
                    break;
                
                case MouseTestStep.End:
                    break;
            }

            if (_mouseTestStep != MouseTestStep.End)
            {
                _mouseTestStep++;
                DRT.ResumeAt(new DrtTest(MouseTestProc));
            }
        }

        private void VerifyValue(RangeBase control, double expectedValue, string testStep)
        {
            DRT.Assert(control.Value == expectedValue,
                string.Format("Invalid value. Expected:{0}, Actual:{1}, Test step:{2}",
                    expectedValue, control.Value, testStep.ToString()));
        }

        private Border _mainBorder;
        private Canvas _mainCanvas;
        private Slider _vslider1;
        private Slider _hslider1;
        private Thumb _hslider1_thumb;
        private bool _ishslider1GotDragStartedEvent = false;
        private DragDeltaEventArgs _hslider1DragDeltaEventArgs = null;
        private bool _ishslider1GotDragCompletedEvent = false;

        private Slider _hslider2;
        private Slider _hslider3;

        private Button _dummyButton;
        private double _vslider1StartValue;
        private double _hslider1StartValue;

        private int _retryCount;

        private KeyboardTestStep _keyboardTestStep = KeyboardTestStep.Start;
        private MouseTestStep _mouseTestStep = MouseTestStep.Start;

        private object _latestMouseTarget = null;
        private double _latestMousePosX = 0.0;
        private double _latestMousePosY = 0.0;

        #region Suicide Timer

        DispatcherTimer _suicideTimer;

        private void OnTimeout(object sender, EventArgs e)
        {
            throw new TimeoutException();
        }

        private class TimeoutException : Exception
        {
            public TimeoutException() : base("Timeout expired, quitting") { }
        }

        #endregion

        private void PressKey(Key k)
        {
            EnsureInputFocus(true);
            DRT.PressKey(k);
        }

        private Slider FindSlider(Visual v)
        {
            if (v != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(v);
                for(int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(v, i) is Slider)
                        return (Slider)(VisualTreeHelper.GetChild(v, i));
                    else
                    {
                        Slider s = FindSlider((Slider)(VisualTreeHelper.GetChild(v,i)));
                        if (s != null)
                            return s;
                    }
                }
            }
            return null;
        }

        private Thumb FindThumb(DependencyObject v)
        {
            if (v != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(v);

                for(int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(v,i) is Thumb)
                        return (Thumb)(VisualTreeHelper.GetChild(v,i));
                    else
                    {
                        Thumb s = FindThumb(VisualTreeHelper.GetChild(v,i));

                        if (s != null)
                            return s;
                    }
                }
            }

            return null;
        }

        // Make sure that test window is active and got input focus
        private bool EnsureInputFocus(bool fAssert)
        {
            bool result = UnsafeNativeMethods.SetForegroundWindow(new HandleRef(null, ((DrtControls)DRT).MainWindow.Handle));
            if (fAssert)
                DRT.Assert(result, "Main window is not active, this may causes keyboard or mouse input problem.");
            return result;
        }

        protected void _vslider1_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (DRT.Verbose)
                Console.WriteLine("_vslider1.OnValueChanged {0} -> {1}", args.OldValue, args.NewValue);
        }

        protected void _hslider1_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (DRT.Verbose)
                Console.WriteLine("_hslider1.OnValueChanged {0} -> {1}", args.OldValue, args.NewValue);
        }

        protected void _hslider1_OnThumbDragStarted(object sender, DragStartedEventArgs args)
        {
            _ishslider1GotDragStartedEvent = true;
            if (DRT.Verbose)
                Console.WriteLine("_hslider1.OnThumbDragStarted HorizontalOffset:{0}, VerticalOffset:{1}", args.HorizontalOffset, args.VerticalOffset);
        }

        protected void _hslider1_OnThumbDragDelta(object sender, DragDeltaEventArgs args)
        {
            _hslider1DragDeltaEventArgs = args;
            if (DRT.Verbose)
                Console.WriteLine("_hslider1.OnThumbDragDelta HorizontalChange:{0}, VerticalChange:{1}", args.HorizontalChange, args.VerticalChange);
        }

        protected void _hslider1_OnThumbDragCompleted(object sender, DragCompletedEventArgs args)
        {
            _ishslider1GotDragCompletedEvent = true;
            if (DRT.Verbose)
                Console.WriteLine("_hslider1.OnThumbDragDelta HorizontalChange:{0}, VerticalChange:{1} Canceld:{2}", args.HorizontalChange, args.VerticalChange, args.Canceled);
        }

        protected void _hslider2_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (DRT.Verbose)
                Console.WriteLine("_hslider2.OnValueChanged {0} -> {1}", args.OldValue, args.NewValue);
        }

        protected void _hslider3_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            if (DRT.Verbose)
                Console.WriteLine("_hslider3.OnValueChanged {0} -> {1}", args.OldValue, args.NewValue);
        }
    }

    [SuppressUnmanagedCodeSecurity()]
    internal class UnsafeNativeMethods
    {
        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal static extern bool SetForegroundWindow(HandleRef hWnd);
    }
}

