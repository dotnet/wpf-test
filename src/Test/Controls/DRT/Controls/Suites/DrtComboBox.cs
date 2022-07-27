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
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Reflection;

namespace DRT
{

    public class ComboBoxSuite : DrtTestSuite
    {
        public ComboBoxSuite() : base("ComboBox")
        {
            Contact = "Microsoft";
        }

        ComboBox _combobox;
        ScrollViewer _scrollViewer;
        Button _buttonInComboBox;

        public override DrtTest[] PrepareTests()
        {

            Border b = new Border();
            b.Background = System.Windows.Media.Brushes.White;

            Canvas canvas = new Canvas();

            // Disable slide so there is no dependency on animation
            canvas.Resources.Add(SystemParameters.ComboBoxPopupAnimationKey, PopupAnimation.None);

            canvas.Width = 200;
            canvas.Height = 100;
            b.Child = canvas;

            ComboBox combobox = new ComboBox();
            combobox.MinWidth = 150;
            combobox.DropDownOpened += new EventHandler(OnDropDownToggle);
            combobox.DropDownClosed += new EventHandler(OnDropDownToggle);
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "Testing";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "Hello";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "World";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "Grape";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "Orange";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "Pear";
            combobox.Items.Add(cbi);

            _buttonInComboBox = new Button();
            _buttonInComboBox.Content = "Button in ComboBox";
            combobox.Items.Add(_buttonInComboBox);

            for (int i = 0; i < 5; i++)
            {
                cbi = new ComboBoxItem();
                cbi.Content = "Item " + i;
                combobox.Items.Add(cbi);
            }

            ComboBox = combobox;
            combobox.SelectedIndex = 0;

            Canvas.SetTop(combobox, 20);
            Canvas.SetLeft(combobox, 20);

            canvas.Children.Add(combobox);

            _combobox.MouseMove += new MouseEventHandler(_combobox_MouseMove);

            DRT.Show(b);

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] {
                    new DrtTest(Start),
                    new DrtTest(Test),
                    new DrtTest(Bugs),
                    new DrtTest(Cleanup),
                };
            }
            else
            {
                return new DrtTest[] {};
            }
        }

        bool _popupOpenEvent;
        bool _popupOpen;

        private void OnDropDownToggle(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            _popupOpenEvent = true;
            _popupOpen = combo.IsDropDownOpen;

            if (_popupOpen && _scrollViewer == null)
            {
                _scrollViewer = DRT.FindElementByID("DropDownScrollViewer", combo) as ScrollViewer;
            }
        }

        private void Start()
        {
        }

        private void Cleanup()
        {
        }

        TestStep _counter = TestStep.Start;
        bool _shouldRepeat = false;

        enum TestStep
        {
            Start,
            SetText,
            VerifyText,
            SetUnmatchedTextForTextSearch,
            VerifyUnmatchedTextForTextSearch,
            SetMatchedTextForTextSearch,
            VerifyMatchedTextForTextSearch,
            ClearText,
            MoveToComboBox,
            MouseDown,
            MouseUp,
            VerifyDropDownOpened,
            MouseOverFirstItem,
            MouseDownOnFirstItem,
            MouseOverSecondItem,
            MouseUpOnSecondItem,
            VerifySelectionChanged,
            MoveToComboBox2,
            ClickMouse,
            VerifyDropDownOpened2,
            MoveToLastItem,
            ClickLastItem,
            VerifySelectionChanged2,

            TimerTest_Start,
            TimerTest_VerifyDropDownOpened,
            TimerTest_MouseOverFirstItem,
            TimerTest_MouseDown,
            TimerTest_MouseOverSecondItem,
            TimerTest_MouseUp,
            TimerTest_VerifySelectionChanged,

            DismissTest_OpenDropDown,
            DismissTest_WaitForCapture,
            DismissTest_MouseOutAndClick,
            DismissTest_VerifyDismissed,

            KeyboardTest_PressDown,
            KeyboardTest_Verify,
            KeyboardTest_PressDown2,
            KeyboardTest_Verify2,

            // Test that we can mouse down on the combobox, move to an item, and mouse up
            ComboBox1_MoveTo,
            ComboBox1_MouseDown,
            ComboBox1_VerifyMouseDown,
            ComboBox1_MoveToItem2,
            ComboBox1_MouseUpOnItem2,
            ComboBox1_VerifySelected,

            // Test that we can mouse down on the CB, mouse up off, and then select an item.
            ComboBox2_MoveTo,
            ComboBox2_MouseDown,
            ComboBox2_MoveOff,
            ComboBox2_MouseUp,
            ComboBox2_VerifyMouseUp,
            ComboBox2_MoveToItem1,
            ComboBox2_ClickMouse,
            ComboBox2_VerifySelected,

            // Test that we can mouse down on the CB, over an item, and then up off the CB to close it (and not select anything).
            ComboBox3_MoveTo,
            ComboBox3_MouseDown,
            ComboBox3_VerifyMouseDown,
            ComboBox3_MoveToItem3,
            ComboBox3_MoveOffCB,
            ComboBox3_MouseUp,
            ComboBox3_VerifyNotSelected,

            // Test that if we use the mouse wheel over the combobox that the selection changes
            // and if we use the mouse wheel over an open combobox the selection doesn't change.
            ComboBox4_MoveTo,
            ComboBox4_MouseWheelDown,
            ComboBox4_MouseWheelUp,
            ComboBox4_OpenDropDown,
            ComboBox4_MouseWheelDown2,
            ComboBox4_Verify,

            // Do a simple verification of combobox keyboard navigation.
            ComboBoxScrolling_MoveTo_And_OpenDropDown,
            ComboBoxScrolling_PressKeyDown,
            ComboBoxScrolling_PressKeyPageDown,
            ComboBoxScrolling_PressKeyEnd,
            ComboBoxScrolling_PressKeyHome,
            ComboBoxScrolling_VerifyPressKeyHome,
            ComboBoxScrolling_MoveToItem3,
            ComboBoxScrolling_MouseDown,
            ComboBoxScrolling_MouseUp,
            ComboBoxScrolling_Verify,

            // Test StaysOpenOnEdit property
            ComboBoxStaysOpenFalse_MoveTo_And_OpenDropDown,
            ComboBoxStaysOpenFalse_Click,
            ComboBoxStaysOpenFalse_Verify,
            ComboBoxStaysOpenTrue_MoveTo_And_OpenDropDown,
            ComboBoxStaysOpenTrue_Click,
            ComboBoxStaysOpenTrue_Verify,

            End,
        }

        private void WaitForPopupAnimationDelay()
        {
            int animationDelay = DrtControls.PopupAnimationDelay;
            Console.WriteLine("Waiting for Popup Animation ({0}) ms", animationDelay);
            DRT.Pause(animationDelay);
        }

        private void Test()
        {
            if (DRT.Verbose) Console.WriteLine(_counter.ToString());
            switch (_counter)
            {
                case TestStep.SetText:
                    _combobox.Text = "This is some text";
                    _combobox.IsEditable = true;
                    break;
                case TestStep.VerifyText:
                    DRT.Assert(_combobox.Text == "This is some text", "ComboBox's text is not the set value");

                    TextBox editableSite = DRT.FindElementByID("PART_EditableTextBox", _combobox) as TextBox;
                    DRT.Assert(editableSite.Text == "This is some text", "EditableTextBox's text is not the set value");
                    break;
                case TestStep.SetUnmatchedTextForTextSearch:
                    _combobox.IsTextSearchCaseSensitive = true;
                    _combobox.Text = "testing";
                    break;
                case TestStep.VerifyUnmatchedTextForTextSearch:
                    DRT.Assert(_combobox.SelectedIndex == -1, "ComboBox's selection should not have been matched");
                    _combobox.IsTextSearchCaseSensitive = false;
                    break;
                case TestStep.SetMatchedTextForTextSearch:
                    _combobox.IsTextSearchCaseSensitive = true;
                    _combobox.Text = "Testing";
                    break;
                case TestStep.VerifyMatchedTextForTextSearch:
                    DRT.Assert(_combobox.SelectedIndex == 0, "ComboBox's selection index should have been 0 but it is " + _combobox.SelectedIndex);
                    _combobox.IsTextSearchCaseSensitive = false;
                    break;
                case TestStep.ClearText:
                    _combobox.IsEditable = false;
                    _combobox.Text = string.Empty;
                    break;
                case TestStep.MoveToComboBox:
                    DRT.WaitForCompleteRender();

                    // Click on the dropdown button
                    DRT.MoveMouse(_combobox, 0.95, 0.5);
                    break;

                case TestStep.MouseDown:
                    if (_comboBoxMouseMoveCount == 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ComboBox did not receive any mouse move events.  Perhaps another");
                        Console.WriteLine("window was in front, or ContentRendered fired too soon.");

                        IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(DRT.RootElement)).Handle;
                        IntPtr foreground = GetForegroundWindow();
                        if (hwnd != foreground)
                        {
                            Console.WriteLine("Foreground window did not match DRT window: DRT window {0:X}, Foreground Window {1:X}", hwnd, foreground);
                        }

                        Console.WriteLine("ComboBox size was: " + _combobox.RenderSize);
                        RECT r = new RECT();
                        GetWindowRect(hwnd, ref r);
                        Console.WriteLine("DRT window size was: ({0}, {1}) - ({2}, {3})", r.left, r.top, r.right, r.bottom);

                        Console.WriteLine("Test will tentatively succeed.");
                        DRT.WriteDelayedOutput();

                        System.Environment.Exit(0);
                    }

                    DRT.MouseButtonDown();
                    WaitForPopupAnimationDelay();
                    break;

                case TestStep.MouseUp:
                    DRT.Assert(_popupOpenEvent, "IsPopupOpenChanged did not fire when mousing down on ComboBox");
                    DRT.Assert(_popupOpen, "Popup isn't open");
                    _popupOpenEvent = false;
                    DRT.MouseButtonUp();
                    break;

                case TestStep.VerifyDropDownOpened:
                    DRT.WaitForCompleteRender();

                    DRT.MoveMouse(_combobox, 0.2, 0.5);
                    break;

                case TestStep.MouseOverFirstItem:
                    MouseOverElement = _combobox.Items[0] as FrameworkElement;
                    break;

                case TestStep.MouseDownOnFirstItem:
                    if (!VerifyMouseOverElement(20)) _shouldRepeat = true;
                    else
                    {
                        DRT.MouseButtonDown();
                    }
                    break;

                case TestStep.MouseOverSecondItem:
                    DRT.MoveMouse(_combobox.Items[1] as UIElement, 0.5, 0.5);
                    break;

                case TestStep.MouseUpOnSecondItem:
                    DRT.MouseButtonUp();
                    WaitForPopupAnimationDelay();
                    break;

                case TestStep.VerifySelectionChanged:
                    DRT.Assert(_popupOpenEvent, "IsPopupOpenChanged did not fire (second time)");
                    DRT.Assert(!_popupOpen, "Popup didn't close");
                    DRT.Assert(((ComboBoxItem)_combobox.SelectedItem).Content.Equals("Hello"), "Didn't select the correct item");
                    _popupOpenEvent = false;
                    break;

                    // Now try clicking on an item that will be outside the window
                case TestStep.MoveToComboBox2:
                    // Click on the dropdown button
                    DRT.MoveMouse(_combobox, 0.95, 0.5);
                    break;

                case TestStep.ClickMouse:
                    DRT.ClickMouse();
                    WaitForPopupAnimationDelay();
                    break;

                case TestStep.VerifyDropDownOpened2:
                    DRT.Assert(_popupOpenEvent, "IsPopupOpenChanged did not fire (third time)");
                    DRT.Assert(_popupOpen, "Popup isn't open");
                    _popupOpenEvent = false;
                    break;

                case TestStep.MoveToLastItem:
                    MouseOverElement = _combobox.Items[_combobox.Items.Count - 1] as FrameworkElement;
                    break;

                case TestStep.ClickLastItem:
                    if (!VerifyMouseOverElement(20)) _shouldRepeat = true;
                    else
                    {
                        DRT.ClickMouse();
                        WaitForPopupAnimationDelay();
                    }
                    break;

                case TestStep.VerifySelectionChanged2:
                    {
                        DRT.Assert(_combobox.SelectedIndex == _combobox.Items.Count - 1, "Last item in the combobox should be selected.  SelectedIndex = " + _combobox.SelectedIndex + ", but should be " + (_combobox.Items.Count - 1) + ".");
                        PresentationSource source = PresentationSource.FromVisual(DRT.RootElement);
                        IntPtr mainHwnd = ((HwndSource)source).Handle;
                        IntPtr activeHwnd = GetForegroundWindow();
                        string activeWindowTitle = GetWindowTitle(activeHwnd);

                        DRT.Assert(mainHwnd == activeHwnd, "Main DRT window " + mainHwnd.ToString("X") + " should be foreground but instead hwnd "
                            + activeHwnd.ToString("X") + " (" + activeWindowTitle + ") was foreground.  " +
                            "\n\n\n *** Are WS_EX_LAYERED windows actually rendering? ***\n\n It appears I've clicked through the popup. Please run drtcombobox /hold,\n click the combobox and see if the dropdown is visible. \n\n");
                    }
                    break;

                case TestStep.TimerTest_Start:
                    _popupOpenEvent = false;
                    _timer = new Timer(new TimerCallback(TimerTest), null, 0, Timeout.Infinite);
                    DRT.Pause(100);
                    break;

                case TestStep.TimerTest_VerifyDropDownOpened:
                    if (!_popupOpenEvent || !_popupOpen)
                    {
                        if (_repeatedTestCount++ < 10)
                        {
                            DRT.Pause(500);
                            DRT.RepeatTest();
                            return;
                        }
                    }

                    DRT.Assert(_popupOpenEvent, "IsPopupOpenChanged did not fire (first time - multithread)");
                    DRT.Assert(_popupOpen, "Popup isn't open");
                    _popupOpenEvent = false;
                    DRT.MoveMouse(_combobox, 0.2, 0.5);

                    _repeatedTestCount = 0;
                    break;

                case TestStep.TimerTest_MouseOverFirstItem:
                    MouseOverElement = _combobox.Items[0] as FrameworkElement;
                    break;

                case TestStep.TimerTest_MouseDown:
                    if (!VerifyMouseOverElement(20)) _shouldRepeat = true;
                    else
                    {
                        DRT.MouseButtonDown();
                    }
                    break;

                case TestStep.TimerTest_MouseOverSecondItem:
                    MouseOverElement = _combobox.Items[1] as FrameworkElement;
                    break;

                case TestStep.TimerTest_MouseUp:
                    DRT.MouseButtonUp();
                    break;

                case TestStep.TimerTest_VerifySelectionChanged:
                    DRT.Assert(_popupOpenEvent, "IsPopupOpenChanged did not fire (second time - multithread)");
                    DRT.Assert(!_popupOpen, "Popup didn't close");
                    DRT.Assert(((ComboBoxItem)_combobox.SelectedItem).Content.Equals("Hello"), "Didn't select the correct item");
                    _popupOpenEvent = false;
                    break;

                case TestStep.DismissTest_OpenDropDown:
                    // Try to click outside the dropdown (in the app) to close it
                    _combobox.IsDropDownOpen = true;
                    break;

                case TestStep.DismissTest_WaitForCapture:
                    if (_repeatedTestCount++ < 10)
                    {
                        if (Mouse.Captured == null)
                        {
                            if (DRT.Verbose) Console.WriteLine("Mouse.Captured was null, waiting until popup opens and takes capture");

                            _repeatedTestCount++;
                            DRT.Pause(500);
                            DRT.RepeatTest();
                            return;
                        }
                    }
                    else
                    {
                        DRT.Assert(false, "Popup never took mouse capture");
                    }
                    _repeatedTestCount = 0;
                    break;

                case TestStep.DismissTest_MouseOutAndClick:
                    DRT.MoveMouse(DRT.RootElement as FrameworkElement, 0.01, 0.01);
                    DRT.ClickMouse();
                    WaitForPopupAnimationDelay();
                    break;

                case TestStep.DismissTest_VerifyDismissed:
                    DRT.Assert(!_combobox.IsDropDownOpen, "ComboBox dropdown should have closed on click outside");
                    DRT.Assert(Mouse.Captured == null, "ComboBox drop down should have released mouse capture but Mouse.Captured is " + Mouse.Captured);
                    break;

                case TestStep.KeyboardTest_PressDown:
                    _combobox.SelectedIndex = 2;
                    ((ComboBoxItem)_combobox.Items[4]).IsEnabled = false;
                    DRT.SendKeyboardInput(Key.Down, true);
                    DRT.SendKeyboardInput(Key.Down, false);
                    break;

                case TestStep.KeyboardTest_Verify:
                    DRT.Assert(_combobox.SelectedIndex == 3, "Pressing down should have selected index 3, but index " + _combobox.SelectedIndex + " was selected");
                    break;

                case TestStep.KeyboardTest_PressDown2:
                    DRT.SendKeyboardInput(Key.Down, true);
                    DRT.SendKeyboardInput(Key.Down, false);
                    break;

                case TestStep.KeyboardTest_Verify2:
                    DRT.Assert(_combobox.SelectedIndex == 5, "Pressing down should have selected index 5, but index " + _combobox.SelectedIndex + " was selected (should have skipped ---- item)");
                    break;

                case TestStep.ComboBox1_MoveTo:
                    DRT.MoveMouse(_combobox, 0.5, 0.5);
                    break;

                case TestStep.ComboBox1_MouseDown:
                    DRT.MouseButtonDown();
                    WaitForPopupAnimationDelay();
                    break;

                case TestStep.ComboBox1_VerifyMouseDown:
                    DRT.Assert(_combobox.IsDropDownOpen, "Mousing down on the combobox did not open the drop down at (" + _counter + ")");
                    break;

                case TestStep.ComboBox1_MoveToItem2:
                    MouseOverElement = _combobox.Items[2] as FrameworkElement;
                    break;

                case TestStep.ComboBox1_MouseUpOnItem2:
                    if (!VerifyMouseOverElement(20)) _shouldRepeat = true;
                    else
                    {
                        DRT.MouseButtonUp();
                        WaitForPopupAnimationDelay();
                    }
                    break;

                case TestStep.ComboBox1_VerifySelected:
                    DRT.Assert(_combobox.SelectedIndex == 2, "Selected Index should have been 2, was " + _combobox.SelectedIndex + " at (" + _counter + ")");
                    DRT.Assert(!_combobox.IsDropDownOpen, "After mouse selecting an item in the combobox, the combobox should be closed." + " at (" + _counter + ")");
                    DRT.Assert(Mouse.Captured == null, "After the combobox closes, nothing should have capture.  Mouse.Captured is " + Mouse.Captured + " at (" + _counter + ")");
                    break;

                //////////

                case TestStep.ComboBox2_MoveTo:
                    DRT.MoveMouse(_combobox, 0.5, 0.5);
                    break;

                case TestStep.ComboBox2_MouseDown:
                    DRT.MouseButtonDown();
                    WaitForPopupAnimationDelay();
                    break;

                case TestStep.ComboBox2_MoveOff:
                    DRT.Assert(_combobox.IsDropDownOpen, "Mousing down on the combobox did not open the drop down at (" + _counter + ")");
                    DRT.MoveMouse(DRT.RootElement as FrameworkElement, 0, 0);
                    break;

                case TestStep.ComboBox2_MouseUp:
                    DRT.MouseButtonUp();
                    break;

                case TestStep.ComboBox2_VerifyMouseUp:
                    DRT.Assert(_combobox.IsDropDownOpen, "Mousing up off the combobox should not close the drop down at (" + _counter + ")");
                    break;

                case TestStep.ComboBox2_MoveToItem1:
                    MouseOverElement = _combobox.Items[1] as FrameworkElement;
                    break;

                case TestStep.ComboBox2_ClickMouse:
                    if (!VerifyMouseOverElement(20)) _shouldRepeat = true;
                    else
                    {
                        DRT.ClickMouse();
                        WaitForPopupAnimationDelay();
                    }
                    break;

                case TestStep.ComboBox2_VerifySelected:
                    DRT.Assert(_combobox.SelectedIndex == 1, "Selected Index should have been 1, was " + _combobox.SelectedIndex + " at (" + _counter + ")");
                    DRT.Assert(!_combobox.IsDropDownOpen, "After mousing on a comboboxitem and then releasing the mouse off, combobox should have closed." + " at (" + _counter + ")");
                    DRT.Assert(Mouse.Captured == null, "After the combobox closes, nothing should have capture.  Mouse.Captured is " + Mouse.Captured + " at (" + _counter + ")");
                    break;

                //////////////

                case TestStep.ComboBox3_MoveTo:
                    DRT.MoveMouse(_combobox, 0.5, 0.5);
                    break;

                case TestStep.ComboBox3_MouseDown:
                    DRT.MouseButtonDown();
                    WaitForPopupAnimationDelay();
                    break;

                case TestStep.ComboBox3_VerifyMouseDown:
                    DRT.Assert(_combobox.IsDropDownOpen, "Mousing down on the combobox did not open the drop down at (" + _counter + ")");
                    break;

                case TestStep.ComboBox3_MoveToItem3:
                    MouseOverElement = _combobox.Items[3] as FrameworkElement;
                    break;


                case TestStep.ComboBox3_MoveOffCB:
                    if (!VerifyMouseOverElement(20)) _shouldRepeat = true;
                    else
                    {
                        DRT.MoveMouse(DRT.RootElement as FrameworkElement, 0, 0);
                    }
                    break;

                case TestStep.ComboBox3_MouseUp:
                    DRT.MouseButtonUp();
                    WaitForPopupAnimationDelay();
                    break;

                case TestStep.ComboBox3_VerifyNotSelected:
                    DRT.Assert(_combobox.SelectedIndex == 1, "Selected Index should have been 1 (selection should not have changed), was " + _combobox.SelectedIndex + " at (" + _counter + ")");
                    DRT.Assert(!_combobox.IsDropDownOpen, "After mouse selecting an item in the combobox, the combobox should be closed." + " at (" + _counter + ")");
                    DRT.Assert(Mouse.Captured == null, "After the combobox closes, nothing should have capture.  Mouse.Captured is " + Mouse.Captured + " at (" + _counter + ")");
                    break;

                case TestStep.ComboBox4_MoveTo:
                    DRT.MoveMouse(_combobox, 0.5, 0.5);
                    _combobox.SelectedIndex = 1;
                    break;

                case TestStep.ComboBox4_MouseWheelDown:
                    DRT.MouseWheelDown();
                    break;

                case TestStep.ComboBox4_MouseWheelUp:
                    DRT.Assert(_combobox.SelectedIndex == 2, "Mouse wheel down should have moved selection from 1 to 2 but selection was " + _combobox.SelectedIndex);
                    DRT.MouseWheelUp();
                    break;

                case TestStep.ComboBox4_OpenDropDown:
                    DRT.Assert(_combobox.SelectedIndex == 1, "Mouse wheel up should have moved selection from 2 to 1 but selection was " + _combobox.SelectedIndex);
                    _combobox.IsDropDownOpen = true;
                    break;

                case TestStep.ComboBox4_MouseWheelDown2:
                    DRT.MouseWheelDown();
                    break;

                case TestStep.ComboBox4_Verify:
                    DRT.Assert(_combobox.SelectedIndex == 1, "Mouse wheel down with drop down open should not have changed selection but selection was " + _combobox.SelectedIndex);
                    _combobox.IsDropDownOpen = false;
                    break;


                case TestStep.ComboBoxScrolling_MoveTo_And_OpenDropDown:
                    DRT.MoveMouse(_combobox, 0.2, 0.2);
                    _combobox.MaxDropDownHeight = 100;
                    _combobox.SelectedIndex = -1;
                    _combobox.IsDropDownOpen = true;
                    WaitForPopupAnimationDelay();
                    break;

                case TestStep.ComboBoxScrolling_PressKeyDown:
                    DRT.Assert(_combobox.IsDropDownOpen, "Set IsDropDownOpen=true but dropdown is closed");
                    DRT.PressKey(Key.Down);
                    break;

                case TestStep.ComboBoxScrolling_PressKeyPageDown:
                    DRT.Assert(Keyboard.FocusedElement == _combobox.Items[0], "Focused item should be item 0, was " + Keyboard.FocusedElement);
                    DRT.Assert(_combobox.SelectedIndex == -1, "No item should be selected but SelectedIndex was " + _combobox.SelectedIndex);
                    DRT.PressKey(Key.PageDown);
                    break;

                case TestStep.ComboBoxScrolling_PressKeyEnd:
                    DRT.Assert(Keyboard.FocusedElement == _combobox.Items[(int)(_scrollViewer.ViewportHeight) - 1], "Focused item should be item " + ((int)(_scrollViewer.ViewportHeight) - 1) + ", was " + Keyboard.FocusedElement);
                    DRT.PressKey(Key.End);
                    break;

                case TestStep.ComboBoxScrolling_PressKeyHome:
                    DRT.Assert(Keyboard.FocusedElement == _combobox.Items[_combobox.Items.Count - 1], "Focused item should be last item, was " + Keyboard.FocusedElement);
                    DRT.PressKey(Key.Home);
                    break;

                case TestStep.ComboBoxScrolling_VerifyPressKeyHome:
                    DRT.Assert(Keyboard.FocusedElement == _combobox.Items[0], "Focused item should be first item, was " + Keyboard.FocusedElement);
                    DRT.Assert(_combobox.SelectedIndex == -1, "No item should be selected but SelectedIndex was " + _combobox.SelectedIndex);
                    break;

                case TestStep.ComboBoxScrolling_MoveToItem3:
                    MouseOverElement = _combobox.Items[3] as ComboBoxItem;
                    break;

                case TestStep.ComboBoxScrolling_MouseDown:
                    if (!VerifyMouseOverElement(20)) _shouldRepeat = true;
                    else
                    {
                        DRT.Assert(Keyboard.FocusedElement == _combobox.Items[3], "Focused item should be item 3, was " + Keyboard.FocusedElement);
                        DRT.Assert(_combobox.SelectedIndex == -1, "No item should be selected but SelectedIndex was " + _combobox.SelectedIndex);

                        DRT.MouseButtonDown();
                    }
                    break;

                case TestStep.ComboBoxScrolling_MouseUp:
                    DRT.MouseButtonUp();
                    break;


                case TestStep.ComboBoxScrolling_Verify:
                    DRT.Assert(Keyboard.FocusedElement == _combobox, "Focus should be on combobox, was " + Keyboard.FocusedElement);
                    DRT.Assert(_combobox.SelectedIndex == 3, "Selected index should be 3, was " + _combobox.SelectedIndex);
                    break;

                case TestStep.ComboBoxStaysOpenFalse_MoveTo_And_OpenDropDown:
                    DRT.Assert(_combobox.StaysOpenOnEdit == false, "StaysOpenOnEdit property should be false by default");
                    _combobox.IsEditable = true;
                    DRT.MoveMouse(_combobox, 0.2, 0.5);
                    _combobox.IsDropDownOpen = true;
                    break;

                case TestStep.ComboBoxStaysOpenFalse_Click:
                    DRT.Assert(_combobox.IsDropDownOpen == true, "_comboBox should have DropDown open");
                    DRT.ClickMouse();
                    break;

                case TestStep.ComboBoxStaysOpenFalse_Verify:
                    DRT.Assert(_combobox.IsDropDownOpen == false, "DropDown didn't close when StaysOpenOnEdit == false");
                    break;

                case TestStep.ComboBoxStaysOpenTrue_MoveTo_And_OpenDropDown:
                    _combobox.StaysOpenOnEdit = true;
                    DRT.Assert(_combobox.StaysOpenOnEdit == true, "Unable to set StaysOpenOnEdit property to true");
                    DRT.Assert(_combobox.IsEditable == true, "_combobox should be editable");
                    DRT.MoveMouse(_combobox, 0.2, 0.5);
                    _combobox.IsDropDownOpen = true;
                    break;

                case TestStep.ComboBoxStaysOpenTrue_Click:
                    DRT.Assert(_combobox.IsDropDownOpen == true, "_comboBox should have DropDown open");
                    DRT.ClickMouse();
                    break;

                case TestStep.ComboBoxStaysOpenTrue_Verify:
                    DRT.Assert(_combobox.IsDropDownOpen == true, "DropDown didn't stay open when StaysOpenOnEdit == false");
                    _combobox.IsDropDownOpen = false;
                    _combobox.StaysOpenOnEdit = false;
                    DRT.Assert(_combobox.StaysOpenOnEdit == false, "Unable to set StaysOpenOnEdit property to false");
                    break;

            }

            if (_counter != TestStep.End)
            {
                if (_shouldRepeat == true)
                {
                    _shouldRepeat = false;
                    DRT.Pause(500);
                    _counter--;
                }
                else
                {
                    _counter++;
                }
                DRT.RepeatTest();
            }

        }

        Timer _timer;

        private void TimerTest(object state)
        {
            _combobox.Dispatcher.Invoke(
                DispatcherPriority.Send,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    if (DRT.Verbose)
                    {
                        Console.WriteLine("Showing DropDown in other thread.");
                    }

                    _combobox.IsDropDownOpen = true;
                    return null;
                },
                null);
        }


        BugsStep _bugCounter = BugsStep.Start;


        enum BugsStep
        {
            Start,

            RegressionBug1_Prepare,
            RegressionBug1_Select1,
            RegressionBug1_Select1_Verify,
            RegressionBug1_Select2,
            RegressionBug1_Select2_Verify,

            RegressionBug2_Prepare,
            RegressionBug2_Select2,
            RegressionBug2_Select2_Verify,

            End,
        }

        private TextBox _textbox;

        private void Bugs()
        {
            if (DRT.Verbose) Console.WriteLine(_bugCounter.ToString());
            switch (_bugCounter)
            {
                case BugsStep.Start:
                    DRT.LoadXamlFile("DrtComboBox.xaml");
                    DRT.Show(DRT.RootElement);

                    break;

                // Selecting UIElements
                case BugsStep.RegressionBug1_Prepare:
                    ComboBox = DRT.FindElementByID("RegressionBug1") as ComboBox;

                    DRT.Assert(_combobox.SelectedIndex == 4, "Selected index should be 4 but is " + _combobox.SelectedIndex);
                    break;

                case BugsStep.RegressionBug1_Select1:
                    _combobox.SelectedIndex = 1;
                    break;

                case BugsStep.RegressionBug1_Select1_Verify:
                    DRT.Assert(_combobox.SelectedIndex == 1, "Selected index should be 1 but is " + _combobox.SelectedIndex);
                    break;

                case BugsStep.RegressionBug1_Select2:
                    _combobox.SelectedIndex = 2;
                    break;

                case BugsStep.RegressionBug1_Select2_Verify:
                    DRT.Assert(_combobox.SelectedIndex == 2, "Selected index should be 2 but is " + _combobox.SelectedIndex);
                    DRT.Assert(_combobox.Text == "2", "Text should be \"2\" but is: " + _combobox.Text);
                    break;


                // DataBinding to Text Property of non-editable comboboxes
                case BugsStep.RegressionBug2_Prepare:
                    ComboBox = DRT.FindElementByID("RegressionBug2CB") as ComboBox;
                    _textbox = DRT.FindElementByID("RegressionBug2TB") as TextBox;

                    DRT.Assert(_combobox.SelectedIndex == -1, "Selected index should be -1 but is " + _combobox.SelectedIndex);
                    break;

                case BugsStep.RegressionBug2_Select2:
                    _combobox.SelectedIndex = 2;
                    break;

                case BugsStep.RegressionBug2_Select2_Verify:
                    DRT.Assert(_combobox.SelectedIndex == 2, "Selected index should be 2 but is " + _combobox.SelectedIndex);
                    DRT.Assert(_combobox.Text == "Three", "ComboBox.Text should be \"Three\" but is: " + _combobox.Text);
                    DRT.Assert(_textbox.Text == "Three", "TextBox.Text should be \"Three\" but is: " + _textbox.Text);
                    break;


                case BugsStep.End:

                    break;

            }

            if (_bugCounter != BugsStep.End)
            {
                _bugCounter++;
                DRT.RepeatTest();
            }

        }



        #region Instrumentation for MouseOverElement (copied from DrtMenu)
        // 

        int _repeatedTestCount = 0;
        FrameworkElement _currentMouseOver;
        bool _verifyMouseOver;

        private FrameworkElement MouseOverElement
        {
            get { return _currentMouseOver; }
            set
            {
                _currentMouseOver = value;
                _verifyMouseOver = true;
                DRT.MoveMouse(_currentMouseOver, 0.1 + _repeatedTestCount * 0.025, 0.5);
            }
        }

        private ComboBox ComboBox
        {
            get { return _combobox; }
            set
            {
                if (_combobox != value)
                {
                    _combobox = value;
                    _scrollViewer = null;
                }
            }
        }

        /// <summary>
        ///     Verifies that mouse is over the requested item.
        /// </summary>
        /// <returns>Whether the test should continue or not</returns>
        private bool VerifyMouseOverElement(int numRetries)
        {
            if (_verifyMouseOver)
            {
                // If the mouse isn't over the element it should be over, it might be b/c the window that has capture hasn't
                // been released yet or the window isn't open
                if (!VerifyMouseOver())
                {
                    if (_repeatedTestCount < numRetries)
                    {
                        if (DRT.Verbose) Console.WriteLine("Mouse should have been over element: " + MouseOverElement);
                        if (DRT.Verbose) Console.WriteLine("Repeating.");

                        _repeatedTestCount++;
                        return false;
                    }
                    else
                    {
                        DRT.Assert(false, "Mouse should have been over " + MouseOverElement);
                    }
                }

                _verifyMouseOver = false;
            }

            _repeatedTestCount = 0;
            return true;
        }

        private bool VerifyMouseOver()
        {
            DRT.WaitForCompleteRender();

            if (DRT.Verbose)
            {
                Console.Write("Mouse.DirectlyOver = " + Mouse.DirectlyOver);
                if (Mouse.DirectlyOver is TextBlock)
                {
                    Console.Write(" (Text=" + (Mouse.DirectlyOver as TextBlock).Text + ")");
                }

                Console.WriteLine();
            }


            PresentationSource sourcePopup = PresentationSource.FromVisual(_currentMouseOver);
            IntPtr windowPopup = ((HwndSource)sourcePopup).Handle;

            // Check that the popup is not size 1x1 -- 
            RECT rect = new RECT();
            GetWindowRect(windowPopup, ref rect);

            if (rect.right - rect.left == 1 && rect.bottom - rect.top == 1)
            {
                Console.WriteLine();
                Console.WriteLine("Warning: Popup was size 1x1, bug 947874.  Test will tentatively succeed");
                Console.WriteLine();
                System.Environment.Exit(0);
            }


            POINT pt = new POINT();
            GetCursorPos(pt);
            IntPtr windowOver = WindowFromPoint(pt.x, pt.y);
            if (windowOver != windowPopup)
            {
                if (DRT.Verbose) Console.WriteLine("The mouse is not over the popup window.  It is over " + windowOver.ToString("X") + ", but it should be over " + windowPopup.ToString("X") + ".  Popup might not be open yet.");
                return false;
            }

            IInputElement mouseOver = Mouse.DirectlyOver;
            if (!_currentMouseOver.IsMouseOver)
            {
                string overtext = (mouseOver is TextBlock) ? ("(" + ((TextBlock)mouseOver).Text + ")") : "";
                if (DRT.Verbose) Console.Write("Moved mouse over " + _currentMouseOver
                    + " but IsMouseOver was false.  Mouse.DirectlyOver = " + mouseOver + " " + overtext + ".");

                return false;
            }

            DRT.Assert(_currentMouseOver.IsMouseOver, "Moved mouse over " + _currentMouseOver
                + " but IsMouseOver was false.  Mouse.DirectlyOver = " + mouseOver + ". ");

            return true;
        }

        #endregion

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetForegroundWindow();

        public static string GetWindowTitle(IntPtr hwnd)
        {
            StringBuilder sb = new StringBuilder(500);
            GetWindowText(new HandleRef(null, hwnd), sb, 500);
            return sb.ToString();
        }

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr _WindowFromPoint(POINTSTRUCT pt);
        internal static IntPtr WindowFromPoint(int x, int y)
        {
            POINTSTRUCT ps = new POINTSTRUCT(x, y);
            return _WindowFromPoint(ps);
        }

        private struct POINTSTRUCT {
            internal int x;
            internal int y;

            internal POINTSTRUCT(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool GetCursorPos([In, Out] POINT pt);

        [StructLayout(LayoutKind.Sequential)]
        public class POINT {
            public int x = 0;
            public int y = 0;

            public POINT() {
            }

            public POINT(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref RECT rect);

        private void _combobox_MouseMove(object sender, MouseEventArgs e)
        {
            _comboBoxMouseMoveCount++;
        }

        int _comboBoxMouseMoveCount = 0;
    }

    public class EditableComboBoxSuite : DrtTestSuite
    {
        public EditableComboBoxSuite() : base("ComboBox.Editable")
        {
            Contact = "Microsoft";
        }

        ComboBox _combobox, _combobox_codestyle, _noneditable;

        Popup _focusAdorner;

        DispatcherTimer _focusTimer;

        public override DrtTest[] PrepareTests()
        {
            Binding binding;
            CollectionViewSource cvs;
            Border b = new Border();
            b.Margin = new Thickness(5);

            StackPanel sp = new StackPanel();
            sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

            b.Child = sp;
            sp.Width = 400;

            _combobox_codestyle = new ComboBox();
            _combobox_codestyle.Name = "ComboBoxCodeStyle";
            // Force Style to come from in-code style
            _combobox_codestyle.Style = _combobox_codestyle.FindResource(typeof(ComboBox)) as Style;
            _combobox_codestyle.Margin = new Thickness(5);
            _combobox_codestyle.IsEditable = true;
            binding = new Binding();
            cvs = new CollectionViewSource();  //use separate CollViews for independent currency
            cvs.Source = _stringData;
            binding.Source = cvs;
            _combobox_codestyle.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            _combobox_codestyle.SelectedIndex = 0;
            _combobox_codestyle.Width = 200;
            sp.Children.Add(_combobox_codestyle);

            _combobox = new ComboBox();
            _combobox.Name = "EditableComboBox";
            _combobox.Margin = new Thickness(5);
            binding = new Binding();
            cvs = new CollectionViewSource();  //use separate CollViews for independent currency
            cvs.Source = _stringData;
            binding.Source = cvs;
            _combobox.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            _combobox.SelectedIndex = 2;
            _combobox.Width = 200;
            sp.Children.Add(_combobox);

            _noneditable = new ComboBox();
            _noneditable.Name = "NonEditableComboBox";
            _noneditable.Margin = new Thickness(5);
            binding = new Binding();
            cvs = new CollectionViewSource();  //use separate CollViews for independent currency
            cvs.Source = _stringData;
            binding.Source = cvs;
            _noneditable.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            _noneditable.SelectedIndex = 4;
            sp.Children.Add(_noneditable);

            CheckBox showFocus = new CheckBox();
            showFocus.Content = "Show Focus?";
            showFocus.Checked += new RoutedEventHandler(showFocus_Checked);
            showFocus.Unchecked += new RoutedEventHandler(showFocus_Unchecked);

            sp.Children.Add(showFocus);

            CheckBox isEditable = new CheckBox();
            isEditable.Content = "Editable 1?";
            isEditable.IsChecked = true;
            isEditable.Checked += new RoutedEventHandler(isEditable_Checked);
            isEditable.Unchecked += new RoutedEventHandler(isEditable_Unchecked);

            sp.Children.Add(isEditable);

            CheckBox isEditable2 = new CheckBox();
            isEditable2.Content = "Editable 2?";
            isEditable2.IsChecked = false;
            isEditable2.Checked += new RoutedEventHandler(isEditable2_Checked);
            isEditable2.Unchecked += new RoutedEventHandler(isEditable2_Unchecked);
            sp.Children.Add(isEditable2);

            CheckBox isReadOnly = new CheckBox();

            isReadOnly.Content = "ReadOnly?";
            isReadOnly.Checked += new RoutedEventHandler(isReadOnly_Checked);
            isReadOnly.Unchecked += new RoutedEventHandler(isReadOnly_Unchecked);
            sp.Children.Add(isReadOnly);

            ListBox statusLB = new ListBox();

            statusLB.Width = 350;
            statusLB.Margin = new Thickness(10);
            statusLB.Items.Add(MakeBoundText("ComboBoxCodeStyle.Text", "Text", _combobox_codestyle));
            statusLB.Items.Add(MakeBoundText("ComboBoxCodeStyle.SelectedIndex", "SelectedIndex", _combobox_codestyle));
            statusLB.Items.Add(MakeBoundText("ComboBox.Text", "Text", _combobox));
            statusLB.Items.Add(MakeBoundText("ComboBox.SelectedIndex", "SelectedIndex", _combobox));
            sp.Children.Add(statusLB);

            _focusAdorner = new Popup();

            _focusTimer = new DispatcherTimer();
            _focusTimer.Interval = TimeSpan.FromMilliseconds(100);
            _focusTimer.Tick += new EventHandler(OnTick);
            //_focusTimer.Start();

            Border margin = new Border();
            margin.Background = System.Windows.Media.Brushes.White;
            margin.BorderBrush = new LinearGradientBrush(Colors.Blue, Colors.Green, 30);
            margin.BorderThickness = new Thickness(2);
            margin.Child = b;

            DRT.Show(margin);

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] {
                    new DrtTest(Start),
                    new DrtTest(TestClassic),
                    new DrtTest(Cleanup),
                };
            }
            else
            {
                return new DrtTest[] {};
            }
        }

        private TextBlock MakeBoundText(string label, string path, DependencyObject bindingSource)
        {
            TextBlock t = new TextBlock();

            t.Inlines.Add(new Run(label + " : "));

            TextBlock text = new TextBlock();

            Binding binding = new Binding(path);
            binding.Mode = BindingMode.OneWay;

            binding.Source = bindingSource;
            text.SetBinding(TextBlock.TextProperty, binding);
            t.Inlines.Add(new InlineUIContainer(text));
            return t;
        }

        private void Start()
        {
        }

        private void Cleanup()
        {
        }

        private string[] _stringData = new string[]
                         {
                            "<Not Looked At>",
                            "Bug Understood",
                            "Fix built",
                            "Fix Checked in Main",
                            "Fix Checked In Private Branch",
                            "Fix Checked In VBL",
                            "Fix Ready For Check In",
                            "Fix Tested/Verified",
                            "Investigating Cause",
                            "No fix needed",
                            "Testing fix",
                            "Waiting For Customer",
                            "Waiting For External Team",
                            "Waiting For Hardware",
                            "Waiting for Software",
                            "Waiting For Vendor Dev Work",
                            "Waiting For Vendor To Sign License",
                            "Working on Fix",
                            "Working on repro",
                         };

        class EditableCBTest
        {
            public EditableCBTest(Key key, string expectedPrefix, int expectedIndex, string expectedText)
            {
                Key = key;
                ExpectedPrefix = expectedPrefix;
                ExpectedIndex = expectedIndex;
                ExpectedText = expectedText;
            }

            public Key Key;

            public string ExpectedPrefix;

            public int ExpectedIndex;

            public string ExpectedText;
        }

        EditableCBTest[] _testClassic = new EditableCBTest[]
                         {
                             new EditableCBTest(Key.F, "f", 2, "Fix built"),
                             new EditableCBTest(Key.I, "fi", 2, "Fix built"),
                             new EditableCBTest(Key.N, "fin", -1, "Fin"),
                             new EditableCBTest(Key.Back, "fi", 2, "Fi"),
                             new EditableCBTest(Key.Back, "f", 2, "F"),
                             new EditableCBTest(Key.Back, "", -1, ""),
                             new EditableCBTest(Key.W, "w", 11, "Waiting For Customer"),
                             new EditableCBTest(Key.O, "wo", 17, "Working on Fix"),
                         };

        ComboBox _testCB;
        int _testIndex;
        EditableCBTest[] _tests;

        TextBox _editableTextBoxSite;

        private void TestClassic()
        {
            _testCB = _combobox_codestyle;
            _testIndex = -1;
            _tests = _testClassic;
            FindIsEditableTextBox(_testCB);
            /*
            _editableTextBoxSite = typeof(ComboBox).GetProperty("InternalEditableTextBoxSite",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty).GetValue(_testCB, null) as TextBox;
            */

            DRT.Assert(_editableTextBoxSite != null, "ComboBox does not have a TextBox in its style");

            DRT.ResumeAt(new DrtTest(PressKey));
        }

        private void PressKey()
        {
            if (_testIndex >= 0)
            {
                EditableCBTest test = _tests[_testIndex];

                DRT.SendKeyboardInput(test.Key, true);
                DRT.SendKeyboardInput(test.Key, false);
            }

            DRT.ResumeAt(new DrtTest(Verify));
        }

        private void Verify()
        {
            if (_testIndex == -1)
            {
                _testCB.Focus();
                DRT.Assert(_testCB.IsKeyboardFocusWithin, "Called focus, but Focus was not on the ComboBox.  Focus is: " + Keyboard.FocusedElement);

                _editableTextBoxSite.Text = String.Empty;
            }
            else
            {
                EditableCBTest test = _tests[_testIndex];

                DRT.Assert(_editableTextBoxSite.Text.ToUpper().StartsWith(test.ExpectedPrefix.ToUpper()),
                    String.Format("Prefix of TextBox ({0}) did not match expected prefix ({1})", _editableTextBoxSite.Text, test.ExpectedPrefix));

                DRT.Assert(_testCB.SelectedIndex == test.ExpectedIndex,
                    String.Format("SelectedIndex of ComboBox ({0}) did not match expected index ({1})", _testCB.SelectedIndex, test.ExpectedIndex));

                DRT.Assert(_testCB.Text == test.ExpectedText,
                    String.Format("Text of ComboBox ({0}) did not match expected text ({1})", _testCB.Text, test.ExpectedText));
            }

            // Go on to the next test
            if (++_testIndex < _tests.Length)
            {
                DRT.ResumeAt(new DrtTest(PressKey));
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            FrameworkElement fe = Keyboard.FocusedElement as FrameworkElement;

            if (fe != null)
            {
                _focusAdorner.IsOpen = true;
                _focusAdorner.PlacementTarget = fe;
                _focusAdorner.Placement = PlacementMode.Relative;

                Border b = new Border();

                b.BorderBrush = Brushes.OrangeRed;
                b.BorderThickness = new Thickness(1);
                b.Width = fe.Width;
                b.Height = fe.Height;

                _focusAdorner.Child = b;
            }
            else
            {
                _focusAdorner.IsOpen = false;
            }
        }

        private void showFocus_Checked(object sender, RoutedEventArgs e)
        {
            if (!_focusTimer.IsEnabled)
            {
                _focusTimer.Start();
            }
        }

        private void showFocus_Unchecked(object sender, RoutedEventArgs e)
        {
            _focusTimer.Stop();
            _focusAdorner.IsOpen = false;
        }

        private void isEditable_Checked(object sender, RoutedEventArgs e)
        {
            _combobox_codestyle.IsEditable = true;
        }

        private void isEditable_Unchecked(object sender, RoutedEventArgs e)
        {
            _combobox_codestyle.IsEditable = false;
        }

        private void isEditable2_Checked(object sender, RoutedEventArgs e)
        {
            _combobox.IsEditable = true;
        }

        private void isEditable2_Unchecked(object sender, RoutedEventArgs e)
        {
            _combobox.IsEditable = false;
        }

        private void isReadOnly_Checked(object sender, RoutedEventArgs e)
        {
            _combobox.IsReadOnly = true;
            _combobox_codestyle.IsReadOnly = true;
        }

        private void isReadOnly_Unchecked(object sender, RoutedEventArgs e)
        {
            _combobox.IsReadOnly = false;
            _combobox_codestyle.IsReadOnly = false;
        }

        private void FindIsEditableTextBox(DependencyObject visual)
        {
            // Only allow IsEditableTextBoxProperty to be set on TextBoxes.
            TextBox tb = visual as TextBox;

            if (tb != null && tb.Name == "PART_EditableTextBox")
            {
                _editableTextBoxSite = tb;
            }
            else
            {
                // Go into the children and look for the property.
                int count = VisualTreeHelper.GetChildrenCount(visual);
                for(int i = 0; i < count; i++)
                {
                    DependencyObject v = VisualTreeHelper.GetChild(visual, i);
                    FindIsEditableTextBox(v);

                    // Stop immediately if we found a child with the property set
                    if (_editableTextBoxSite != null)
                    {
                        break;
                    }
                }
            }
        }
    }

    public class ComboBoxTestSuite : DrtTestSuite
    {
        public ComboBoxTestSuite() : base("ComboBox.Test")
        {
            Contact = "Microsoft";
        }

        ComboBox _combobox;

        public override DrtTest[] PrepareTests()
        {
            Border b = new Border();

            b.Background = System.Windows.Media.Brushes.White;

            Canvas canvas = new Canvas();

            b.Child = canvas;

            ComboBox combobox = new ComboBox();

            combobox.DropDownOpened += new EventHandler(OnDropDownToggle);
            combobox.DropDownClosed += new EventHandler(OnDropDownToggle);

            ComboBoxItem cbi = new ComboBoxItem();

            cbi.Content = "Testing";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "Hello";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "World";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "Grape";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "Orange";
            combobox.Items.Add(cbi);
            cbi = new ComboBoxItem();
            cbi.Content = "Pear";
            combobox.Items.Add(cbi);
            for (int i = 0; i < 10; i++)
            {
                cbi = new ComboBoxItem();
                cbi.Content = "Item " + i;
                combobox.Items.Add(cbi);
            }

            _combobox = combobox;
            combobox.SelectedIndex = 0;
            Canvas.SetTop(combobox, 20);
            Canvas.SetLeft(combobox, 20);
            canvas.Children.Add(combobox);
            DRT.Show(b);

            return new DrtTest[] {
            };
        }

        private void OnDropDownToggle(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;

            if (combo.IsDropDownOpen)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1000);
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;
            timer.Stop();

            Console.WriteLine("Adding an item.");
            // add an element to cause a relayout and hopefully a recreate of the render target
            ComboBoxItem cbi = new ComboBoxItem();

            cbi.Content = "adding this item should cause a relayout";
            _combobox.Items.Add(cbi);
        }

    }
}

