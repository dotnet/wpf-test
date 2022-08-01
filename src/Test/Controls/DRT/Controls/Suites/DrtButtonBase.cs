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
using System.Windows.Automation.Provider;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Reflection;

namespace DRT
{

    public enum ButtonTest
    {
        Basic,
        Input,
        ContentControlTest,
    }


    public class ButtonBaseSuite : DrtTestSuite
    {
        public ButtonBaseSuite(ButtonTest test) : base("ButtonBase." + test.ToString())
        {
            Contact = "Microsoft";
            _test = test;
        }

        ButtonTest _test;

        ButtonImpl _buttonBase;
        Button _button;
        Button _buttonOnPress;
        Button _buttonRichContent;
        CheckBox _checkBox;
        ToggleButton _toggleButton;
        RadioButton _radioButton, _radioButton2;

        CheckBox _checkBox2, _checkBox3;
        ListBoxItem _listItem2, _listItem3; // for bool <-> Nullable<bool> databinding
        TextBox _textBox;

        public override DrtTest[] PrepareTests()
        {
            Border rootBorder = new Border();
            rootBorder.Background = Brushes.BlanchedAlmond;

            StackPanel sp = new StackPanel();
            sp.Orientation = System.Windows.Controls.Orientation.Vertical;

            Thickness margin = new Thickness(5);

            rootBorder.Child = sp;

            _buttonBase = new ButtonImpl();
            _buttonBase.Margin = margin;
            _buttonBase.Content = "ButtonBase";

            _button = new Button();
            _button.Margin = margin;
            _button.Content = "Button";

            _buttonOnPress = new Button();
            _buttonOnPress.Margin = margin;
            _buttonOnPress.ClickMode = ClickMode.Press;
            _buttonOnPress.Content = "Button(OnPress)";

            _checkBox = new CheckBox();
            _checkBox.Margin = margin;
            _checkBox.Content = "CheckBox";

            StackPanel rbl = new StackPanel();

            _radioButton = new RadioButton();
            _radioButton.Margin = margin;
            _radioButton.Content = "RadioButton";

            _radioButton2 = new RadioButton();
            _radioButton2.Margin = margin;
            _radioButton2.Content = "RadioButton2";

            rbl.Children.Add(_radioButton);
            rbl.Children.Add(_radioButton2);

            Object [] objList = new Object[3];

            objList[0] = "string1";
            objList[2] = "string2";
            objList[1] = new Hyperlink(new Run("My Hyperlink"));

            _buttonRichContent = new Button();
            _buttonRichContent.Content = objList;

            _toggleButton = new ToggleButton();
            _toggleButton.Margin = margin;
            _toggleButton.Content = "Toggle Button";

            _checkBox2 = new CheckBox();
            _checkBox2.Margin = margin;
            _checkBox2.Content = "(source of binding)";

            _checkBox3 = new CheckBox();
            _checkBox3.Margin = margin;
            _checkBox3.Content = "(target of binding)";

            ListBox lb = new ListBox();
            lb.SelectionMode = SelectionMode.Multiple;

            lb.Items.Add(_listItem2 = new ListBoxItem());
            lb.Items.Add(_listItem3 = new ListBoxItem());
            _listItem2.Content = "Target of an IsChecked binding";
            _listItem3.Content = "IsSelected = binding to IsChecked";

            // Set up bindings both directions
            Binding binding = new Binding("IsSelected");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = _listItem2;
            _checkBox2.SetBinding(ToggleButton.IsCheckedProperty, binding);

            binding = new Binding("IsChecked");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = _checkBox3;
            _listItem3.SetBinding(ListBoxItem.IsSelectedProperty, binding);

            // Create TextBox used as CommandTarget for _button
            _textBox = new TextBox();

            sp.Children.Add(_buttonBase);
            sp.Children.Add(_button);
            sp.Children.Add(_buttonOnPress);
            sp.Children.Add(_checkBox);
            sp.Children.Add(rbl);
            sp.Children.Add(_buttonRichContent);
            sp.Children.Add(_toggleButton);
            sp.Children.Add(_checkBox2);
            sp.Children.Add(_checkBox3);
            sp.Children.Add(lb);
            sp.Children.Add(_textBox);

            // Small size test
            for (int i = 20; i >= 0; i--)
            {
                Button btn = new Button();
                btn.Content = i;
                btn.Width = (double)i;
                btn.Height = (double)i;
                sp.Children.Add(btn);
            }

            DRT.Show(rootBorder);

            _buttonBase.Click += new RoutedEventHandler(OnButtonClick);
            _button.Click += new RoutedEventHandler(OnButtonClick);
            _buttonOnPress.Click += new RoutedEventHandler(OnButtonClick);
            _checkBox.Click += new RoutedEventHandler(OnButtonClick);
            _radioButton.Click += new RoutedEventHandler(OnButtonClick);
            _radioButton2.Click += new RoutedEventHandler(OnButtonClick);
            _toggleButton.Click += new RoutedEventHandler(OnButtonClick);

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();
                tests.Add(new DrtTest(Start));

                switch (_test)
                {
                    case ButtonTest.Basic:
                        tests.Add(new DrtTest(BasicTest1));
                        tests.Add(new DrtTest(BasicTest2));
                        tests.Add(new DrtTest(BasicTest3));
                        tests.Add(new DrtTest(BasicTest4));
                        tests.Add(new DrtTest(BasicTest5));
                        break;

                    case ButtonTest.Input:
                        tests.Add(new DrtTest(InputTest));
                        break;

                    case ButtonTest.ContentControlTest:
                        tests.Add(new DrtTest(ContentControlTest));
                        break;
                }

                tests.Add(new DrtTest(Cleanup));
                return tests.ToArray();
            }

            return new DrtTest[] {};
        }

        DispatcherTimer _suicideTimer = null;

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

        public void Cleanup()
        {
            if (_suicideTimer != null)
            {
                _suicideTimer.Stop();
            }
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            throw new TimeoutException();
        }

        private class TimeoutException : Exception
        {
            public TimeoutException() : base("Timeout expired, quitting") { }
        }

        public static DependencyProperty ClickCountProperty = DependencyProperty.RegisterAttached("ClickCount", typeof(int), typeof(ButtonBaseSuite));

        public int GetClickCount(DependencyObject d) { return (int)d.GetValue(ClickCountProperty); }

        public void OnButtonClick(object sender, RoutedEventArgs e)
        {
            _clickCount++;

            ButtonBase buttonBase = (ButtonBase)sender;

            int clickCount = (int)buttonBase.GetValue(ClickCountProperty);
            buttonBase.SetValue(ClickCountProperty, ++clickCount);

            string content = ((string)buttonBase.Content);
            int i = content.IndexOf(" ");
            string baseString = content.Substring(0, ((i == -1) ? content.Length : i));
            buttonBase.Content = baseString + " " + clickCount;
        }

        public void OnIsCheckedChanged(object sender, RoutedEventArgs e)
        {
            _fired = true;
        }

        int _clickCount = 0;
        bool _fired = false;

        #region Basic Test

        public void BasicTest1()
        {
            _checkBox.Checked += new RoutedEventHandler(OnIsCheckedChanged);
            _checkBox.Unchecked += new RoutedEventHandler(OnIsCheckedChanged);

            // Regression Test 

            DRT.Assert(_buttonRichContent.RenderSize.Width < 250d, "Button with rich content seems to have incorrect size (too big).");

            // Verify default prop values
            DRT.Assert(!_button.IsPressed, "ButtonBase IsPressed should be false by default.");

            // Verify Click event
            _fired = false;
            DoClick(_button);
        }

        public void BasicTest2()
        {
            DRT.Assert(_clickCount == 1, "Click event should have fired, click count was " + _clickCount);
            _clickCount = 0;

            // Default Values
            DRT.Assert(_checkBox.IsChecked == false, "1. IsChecked default value should be false.");
            DRT.Assert(!_checkBox.IsPressed, "2. IsPressed default value should be false.");
            DRT.Assert(!_checkBox.IsThreeState, "3. IsThreeState default value should be false.");

            DoToggle(_checkBox);
            DRT.Assert(_checkBox.IsChecked == true, "4. IsChecked should be true.");
            DRT.Assert(_fired, "5. IsCheckedChanged event should be fired.");
            _fired = false;

            DoToggle(_checkBox);
            DRT.Assert(_checkBox.IsChecked == false, "6. IsChecked should be false.");
            DRT.Assert(_fired, "7. IsCheckedChanged event should be fired.");
            _fired = false;

            _checkBox.IsThreeState = true;
            DoToggle(_checkBox);
            DRT.Assert(_checkBox.IsChecked == true, "8. IsChecked should be true.");

            DoToggle(_checkBox);
            DRT.Assert(_checkBox.IsChecked == null, "9. IsChecked should be null.");

            _checkBox.Checked -= new RoutedEventHandler(OnIsCheckedChanged);
            _checkBox.Unchecked -= new RoutedEventHandler(OnIsCheckedChanged);
            _fired = false;
            _clickCount = 0;

            // ToggleButton tests (no Input tests for togglebutton.)
            _toggleButton.IsChecked = false;

            // clicking ToggleButton should toggle IsChecked state
            InvokeButtonBase(_toggleButton);
            DRT.Assert(_toggleButton.IsChecked == true, "Clicking ToggleButton should change IsChecked to true, was " + _toggleButton.IsChecked);

            DoToggle(_toggleButton);
            DRT.Assert(_toggleButton.IsChecked == false, "Toggling should have changed IsChecked from true to false, was " + _toggleButton.IsChecked);
            DoToggle(_toggleButton);
            DRT.Assert(_toggleButton.IsChecked == true, "Toggling should have changed IsChecked from false to true, was " + _toggleButton.IsChecked);

            // Test data transfer (make sure Nullable<bool> <-> bool conversion works)
            _checkBox2.IsChecked = true;
            DRT.Assert(_listItem2.IsSelected, "setting _checkBox2.IsChecked = true should have changed _listItem2.IsSelected to true");
            _listItem2.IsSelected = false;
            DRT.Assert(_checkBox2.IsChecked == false, "setting _listItem2.IsSelected = false should have changed _checkBox2.IsChecked to false");

            _checkBox3.IsChecked = true;
            DRT.Assert(_listItem3.IsSelected, "setting _checkBox3.IsChecked = true should have changed _listItem3.IsSelected to true");
            _listItem3.IsSelected = false;
            DRT.Assert(_checkBox3.IsChecked == false, "setting _listItem3.IsSelected = false should have changed _checkBox3.IsChecked to false");

            // Command, Command.Target test
            _textBox.Text = "Hello";
            _button.CommandTarget = (IInputElement)_textBox;
            _button.Command = ApplicationCommands.SelectAll;
            DoClick(_button);
        }

        public void BasicTest3()
        {
            _button.Command = ApplicationCommands.Cut;
            DoClick(_button);
        }

        public void BasicTest4()
        {
            _button.Command = ApplicationCommands.Paste;
            DoClick(_button);
            DoClick(_button);
        }
        public void BasicTest5()
        {
            DRT.Assert(_textBox.Text == "HelloHello", "TextBox should contain HelloHello text SelectAll,Cut,Paste,Paste commands");
        }

        void InvokeButtonBase(ButtonBase b)
        {
            MethodInfo info = typeof(ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            if (info == null) throw new Exception("Could not find ButtonBase.OnClick method");
            info.Invoke(b, new object[] {});
        }

        #endregion

        #region Input tests

        enum InputTestStep
        {
            Start,
            // Try clicking on the button base derivative
            Test1_ButtonBaseMoveTo,
            Test1_ButtonBaseMouseDown,
            Test1_ButtonBaseMouseUp,
            Test1_Verify,

            // Invoke with spacebar
            Test2_Focus,
            Test2_SpaceBarDown,
            Test2_SpaceBarUp,
            Test2_Verify,

            // Invoke with Enter
            Test3_EnterDown,
            Test3_EnterUp,
            Test3_Verify,

            // Mouse down, mouse off and release
            Test4_ButtonBaseMoveTo,
            Test4_ButtonBaseMouseDown,
            Test4_ButtonBaseMouseOff,
            Test4_ButtonBaseMouseUp,
            Test4_Verify,

            // OnPress: Make sure it clicks on down
            Test5_OnPress_ButtonMoveTo,
            Test5_OnPress_ButtonMouseDown,
            Test5_OnPress_ButtonMouseUp,
            Test5_OnPress_Verify,

            // OnPress: Spacebar click
            Test6_OnPress_SpaceBarDown,
            Test6_OnPress_SpaceBarUp,
            Test6_OnPress_Verify,

            // OnPress: Invoke with Enter
            Test7_OnPress_EnterDown,
            Test7_OnPress_EnterUp,
            Test7_OnPress_Verify,

            // Invoke cancel with Spacebar + other key
            Test8_CancelInvoke_SpaceDown,
            Test8_CancelInvoke_EscDown,
            Test8_CancelInvoke_SpaceUp,
            Test8_CancelInvoke_Verify,

            // Invoke shound not cancel with mouse button down + other key
            Test9_NotCancelInvoke_MouseDown,
            Test9_NotCancelInvoke_EscDown,
            Test9_NotCancelInvoke_MouseUp,
            Test9_NotCancelInvoke_Verify,

            // Invoke shound not cancel with mouse button down + other key
            Test10_CaptureAndSpace_MouseDown,
            Test10_CaptureAndSpace_SpaceDown,
            Test10_CaptureAndSpace_SpaceUp,
            Test10_CaptureAndSpace_MouseUp,
            Test10_CaptureAndSpace_Verify,
            End,
        }

        InputTestStep _inputTestStep;

        public void InputTest()
        {
            if (DRT.Verbose) Console.WriteLine("InputTest: " + _inputTestStep);

            switch (_inputTestStep)
            {
                case InputTestStep.Start:
                    _clickCount = 0;
                    break;

                // Make sure clicking a button works

                case InputTestStep.Test1_ButtonBaseMoveTo:
                    DRT.MoveMouse(_buttonBase, 0.5, 0.5);
                    break;

                case InputTestStep.Test1_ButtonBaseMouseDown:
                    DRT.MouseButtonDown();
                    break;

                case InputTestStep.Test1_ButtonBaseMouseUp:
                    DRT.Assert(_buttonBase.IsPressed, "After mouse down _buttonBase.IsPressed should be true");
                    DRT.Assert(_buttonBase.IsKeyboardFocused, "After mouse down _buttonBase.IsKeyboardFocused should be true");
                    DRT.Assert(Mouse.Captured == _buttonBase, "After mouse down, Mouse.Captured should be " + _buttonBase + ", was " + Mouse.Captured);
                    DRT.Assert(_clickCount == 0, "Click event should not have fired yet");
                    DRT.MouseButtonUp();
                    break;

                case InputTestStep.Test1_Verify:
                    DRT.Assert(_clickCount == 1, "After mouse up, clickCount should be 1, was " + _clickCount);
                    DRT.Assert(Mouse.Captured == null, "After clicking the button, Mouse.Captured should be null, was " + Mouse.Captured);

                    _clickCount = 0;
                    break;

                ////////////////////////////

                case InputTestStep.Test2_Focus:
                    _buttonBase.Focus();
                    break;

                case InputTestStep.Test2_SpaceBarDown:
                    DRT.Assert(_buttonBase.IsKeyboardFocused, "ButtonBase should have focus");
                    DRT.SendKeyboardInput(Key.Space, true);
                    break;

                case InputTestStep.Test2_SpaceBarUp:
                    DRT.Assert(_buttonBase.IsPressed, "ButtonBase should be pressed");
                    DRT.Assert(_clickCount == 0, "Button click count should be 0, was " + _clickCount);
                    DRT.SendKeyboardInput(Key.Space, false);
                    break;

                case InputTestStep.Test2_Verify:
                    DRT.Assert(_clickCount == 1, "Button should have clicked once by pressing spacebar, clickcount is " + _clickCount);
                    _clickCount = 0;
                    break;

                ////////////////////

                case InputTestStep.Test3_EnterDown:
                    DRT.Assert(_buttonBase.IsKeyboardFocused, "ButtonBase should have focus");
                    DRT.SendKeyboardInput(Key.Enter, true);
                    break;

                case InputTestStep.Test3_EnterUp:
                    DRT.Assert(_clickCount >= 1, "ButtonBase click count should be at least 1, was " + _clickCount);
                    DRT.SendKeyboardInput(Key.Enter, false);
                    break;

                case InputTestStep.Test3_Verify:
                    DRT.Assert(!_buttonBase.IsPressed, "ButtonBase should not be pressed");
                    _clickCount = 0;
                    break;

                ////////////////////////////////

                case InputTestStep.Test4_ButtonBaseMoveTo:
                    DRT.MoveMouse(_buttonBase, 0.4, 0.4);
                    break;

                case InputTestStep.Test4_ButtonBaseMouseDown:
                    DRT.MouseButtonDown();
                    break;

                case InputTestStep.Test4_ButtonBaseMouseOff:
                    DRT.Assert(_buttonBase.IsPressed, "After mouse down _buttonBase.IsPressed should be true");
                    DRT.Assert(Mouse.Captured == _buttonBase, "After mouse down, Mouse.Captured should be " + _buttonBase + ", was " + Mouse.Captured);
                    DRT.MoveMouse((UIElement)DRT.RootElement, 0, 0);
                    break;

                case InputTestStep.Test4_ButtonBaseMouseUp:
                    DRT.Assert(!_buttonBase.IsPressed, "After mouse off _buttonBase.IsPressed should be false");
                    DRT.Assert(Mouse.Captured == _buttonBase, "After mouse off, Mouse.Captured should still be " + _buttonBase + ", was " + Mouse.Captured);
                    DRT.Assert(_clickCount == 0, "Click event should not have fired");
                    DRT.MouseButtonUp();
                    break;

                case InputTestStep.Test4_Verify:
                    DRT.Assert(_clickCount == 0, "After mouse up off the buttonbase, clickCount should be 0, was " + _clickCount);
                    DRT.Assert(Mouse.Captured == null, "After mouse up, Mouse.Captured should be null, was " + Mouse.Captured);
                    _clickCount = 0;
                    break;

                ///////////////////////////////

                case InputTestStep.Test5_OnPress_ButtonMoveTo:
                    DRT.MoveMouse(_buttonOnPress, 0.5, 0.5);
                    break;

                case InputTestStep.Test5_OnPress_ButtonMouseDown:
                    DRT.MouseButtonDown();
                    break;

                case InputTestStep.Test5_OnPress_ButtonMouseUp:
                    DRT.Assert(_buttonOnPress.IsPressed, "After mouse down _buttonOnPress.IsPressed should be true");
                    DRT.Assert(_buttonOnPress.IsKeyboardFocused, "After mouse down _buttonOnPress.IsKeyboardFocused should be true");
                    DRT.Assert(Mouse.Captured == _buttonOnPress, "After mouse down, Mouse.Captured should be " + _buttonOnPress + ", was " + Mouse.Captured);
                    DRT.Assert(_clickCount == 1, "Click event should have fired on press");
                    DRT.MouseButtonUp();
                    break;

                case InputTestStep.Test5_OnPress_Verify:
                    DRT.Assert(_clickCount == 1, "After mouse up, clickCount should still be 1, was " + _clickCount);
                    DRT.Assert(Mouse.Captured == null, "After mouse up off button, Mouse.Captured should be null, was " + Mouse.Captured);
                    _clickCount = 0;
                    break;

                ////////////////////////////

                case InputTestStep.Test6_OnPress_SpaceBarDown:
                    DRT.Assert(_buttonOnPress.IsKeyboardFocused, "ButtonOnPress should still have focus");
                    DRT.SendKeyboardInput(Key.Space, true);
                    break;

                case InputTestStep.Test6_OnPress_SpaceBarUp:
                    DRT.Assert(Mouse.Captured == _buttonOnPress, "Mouse.Captured should be _buttonOnPress, was " + Mouse.Captured);
                    DRT.Assert(_buttonOnPress.IsPressed, "ButtonOnPress should be pressed");
                    DRT.Assert(_clickCount == 1, "After pressing spacebar down, ButtonOnPress Click count should be 1, was " + _clickCount);
                    DRT.SendKeyboardInput(Key.Space, false);
                    break;

                case InputTestStep.Test6_OnPress_Verify:
                    DRT.Assert(_clickCount == 1, "ButtonOnPress should not have clicked again by releasing spacebar, click count is " + _clickCount);
                    _clickCount = 0;
                    break;

                ///////////////////////////////

                case InputTestStep.Test7_OnPress_EnterDown:
                    DRT.Assert(_buttonOnPress.IsKeyboardFocused, "_buttonOnPress should have focus");
                    DRT.SendKeyboardInput(Key.Enter, true);
                    break;

                case InputTestStep.Test7_OnPress_EnterUp:
                    DRT.Assert(_clickCount >= 1, "_buttonOnPress click count should be at least 1, was " + _clickCount);
                    DRT.SendKeyboardInput(Key.Enter, false);
                    break;

                case InputTestStep.Test7_OnPress_Verify:
                    DRT.Assert(!_buttonOnPress.IsPressed, "_buttonOnPress should not be pressed");
                    _clickCount = 0;
                    _buttonBase.Focus();
                    break;

                ///////////////////////////////
                // Invoke cancel with Spacebar + other key
                case InputTestStep.Test8_CancelInvoke_SpaceDown:
                    DRT.Assert(_buttonBase.IsKeyboardFocused, "ButtonBase should have focus");
                    DRT.SendKeyboardInput(Key.Space, true);
                    break;

                case InputTestStep.Test8_CancelInvoke_EscDown:
                    DRT.Assert(_buttonBase.IsPressed, "ButtonBase should be pressed");
                    DRT.PressKey(Key.Escape);
                    break;

                case InputTestStep.Test8_CancelInvoke_SpaceUp:
                    DRT.Assert(!_buttonBase.IsPressed, "ButtonBase should not be pressed");
                    DRT.SendKeyboardInput(Key.Space, false);
                    break;

                case InputTestStep.Test8_CancelInvoke_Verify:
                    DRT.Assert(_clickCount == 0, "ButtonBase should not invoke by releasing spacebar when canceled by Esc key, click count is " + _clickCount);
                    DRT.MoveMouse(_buttonBase, 0.5, 0.5);
                    break;

                ///////////////////////////////
                // Invoke should not cancel with mouse button down + other key
                case InputTestStep.Test9_NotCancelInvoke_MouseDown:
                    DRT.MouseButtonDown();
                    break;

                case InputTestStep.Test9_NotCancelInvoke_EscDown:
                    DRT.Assert(_buttonBase.IsPressed, "ButtonBase should be pressed");
                    DRT.PressKey(Key.Escape);
                    break;

                case InputTestStep.Test9_NotCancelInvoke_MouseUp:
                    DRT.Assert(_buttonBase.IsPressed, "After mouse down _buttonBase.IsPressed should be true");
                    DRT.Assert(_buttonBase.IsKeyboardFocused, "After mouse down _buttonBase.IsKeyboardFocused should be true");
                    DRT.Assert(Mouse.Captured == _buttonBase, "After mouse down, Mouse.Captured should be " + _buttonBase + ", was " + Mouse.Captured);
                    DRT.Assert(_clickCount == 0, "Click event should not have fired yet");
                    DRT.MouseButtonUp();
                    break;

                case InputTestStep.Test9_NotCancelInvoke_Verify:
                    DRT.Assert(_clickCount == 1, "ButtonBase should invoke by releasing mousebutton when canceled by Esc key, click count is " + _clickCount);
                    _clickCount = 0;
                    DRT.MoveMouse(_buttonBase, 0.5, 0.5);
                    break;

                /////////////////////////////////////////
                case InputTestStep.Test10_CaptureAndSpace_MouseDown:
                    DRT.MouseButtonDown();
                    break;

                case InputTestStep.Test10_CaptureAndSpace_SpaceDown:
                    DRT.Assert(_buttonBase.IsPressed, "ButtonBase should be pressed");
                    DRT.Assert(Mouse.Captured == _buttonBase, "Mouse.Captured should be _buttonBase, was " + Mouse.Captured);
                    DRT.SendKeyboardInput(Key.Space, true);
                    break;

                case InputTestStep.Test10_CaptureAndSpace_SpaceUp:
                    DRT.Assert(Mouse.Captured == _buttonBase, "Mouse.Captured should be _buttonBase, was " + Mouse.Captured);
                    DRT.Assert(_buttonBase.IsPressed, "ButtonBase should be pressed");
                    DRT.SendKeyboardInput(Key.Space, false);
                    break;

                case InputTestStep.Test10_CaptureAndSpace_MouseUp:
                    DRT.Assert(Mouse.Captured == _buttonBase, "Mouse.Captured should be _buttonBase, was " + Mouse.Captured);
                    DRT.Assert(_clickCount == 0, "Click event should not have fired yet");
                    DRT.MouseButtonUp();
                    break;

                case InputTestStep.Test10_CaptureAndSpace_Verify:
                    DRT.Assert(Mouse.Captured == null, "Mouse.Captured should be null, was " + Mouse.Captured);
                    DRT.Assert(_clickCount == 1, "Click event should have fired when mouse button was released");
                    _clickCount = 0;
                    break;


                case InputTestStep.End:
                    break;
                }

            _inputTestStep++;
            if (_inputTestStep <= InputTestStep.End)
            {
                DRT.RepeatTest();
            }
        }


        #endregion

        #region ContentControl Test
        public void ContentControlTest()
        {
            // ContentControl Enumeration Test (
            ContentControl cc = new ContentControl();
            string singularContent = "Singular Content";

            ((IAddChild)cc).AddText(singularContent);
            DRT.Assert((string)cc.Content == singularContent, "Content was not the string passed to AddText");

            IEnumerator enumerator = LogicalTreeHelper.GetChildren(cc).GetEnumerator();

            DRT.Assert(enumerator.MoveNext(), "Could not move next to first content");
            DRT.Assert((string)enumerator.Current == singularContent, "enumerator.Current is not the expected string, was " + enumerator.Current);
            DRT.Assert(!enumerator.MoveNext(), "MoveNext returned true on second call for singular content");

            try
            {
                object bad = enumerator.Current;
                // Should not get the the following line, expected and InvalidOperationException.
                DRT.Assert(false, "Should have thrown InvalidOperationException on enumerator.get_Current");
            }
            catch (InvalidOperationException) { }

            enumerator.Reset();

            cc.Content = "new content";

            try
            {
                enumerator.MoveNext();
                DRT.Assert(false, "Move next should have thrown InvalidOperationException because Content changed");
            }
            catch (InvalidOperationException)
            {
            }
        }
        #endregion ContentControl Test

        /// <summary>
        /// Helper. Invokes Click on a given uielement. 
        /// </summary>
        private void DoClick(UIElement uielement)
        {
            if (uielement != null)
            {
                AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(uielement);
                if (ap != null)
                {
                    IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                    if (iip != null)
                    {
                        iip.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Helper. Invokes Toggle on a given uielement. 
        /// </summary>
        private void DoToggle(UIElement uielement)
        {
            if (uielement != null)
            {
                AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(uielement);
                if (ap != null)
                {
                    IToggleProvider itp = (IToggleProvider)ap.GetPattern(PatternInterface.Toggle);
                    if (itp != null)
                    {
                        itp.Toggle();
                    }
                }
            }
        }
    }

    public class ButtonImpl : ButtonBase
    {
        public ButtonImpl()
        {
            // <Canvas Height="20" Width="100">        
            //    <Rectangle
            //    Width="100%" Height="100%"  
            //    RadiusX="10" RadiusY="10" 
            //    Fill="LightGreen" 
            //    Stroke="CadetBlue" StrokeThickness="2"
            //    />
            //    <ContentPresenter 
            //    TextPanel.TextTrimming    = "*Alias(Target=TextTrimming)"
            //    Margin="15,13,15,15"/>
            // </Canvas>
            
            Style style = new Style(typeof(ButtonImpl));

            FrameworkElementFactory rootFef = new FrameworkElementFactory(typeof(Canvas));
            rootFef.SetValue(FrameworkElement.HeightProperty, 20d);
            rootFef.SetValue(FrameworkElement.WidthProperty, 100d);

            FrameworkElementFactory rectFef = new FrameworkElementFactory(typeof(Rectangle));
            rectFef.SetValue(FrameworkElement.HeightProperty, 20d);
            rectFef.SetValue(FrameworkElement.WidthProperty, 100d);
            rectFef.SetValue(Rectangle.RadiusXProperty, 10d);
            rectFef.SetValue(Rectangle.RadiusYProperty, 10d);
            rectFef.SetValue(Shape.FillProperty, Brushes.LightGreen);
            rectFef.SetValue(Shape.StrokeProperty, Brushes.CadetBlue);
            rectFef.SetValue(Shape.StrokeThicknessProperty, 2d);

            FrameworkElementFactory cpFef = new FrameworkElementFactory(typeof(ContentPresenter));
            cpFef.SetValue(Control.MarginProperty, new Thickness(15,13,15,15));

            rootFef.AppendChild(rectFef);
            rootFef.AppendChild(cpFef);

            ControlTemplate template = new ControlTemplate(typeof(ButtonImpl));
            template.VisualTree = rootFef;
            style.Setters.Add(new Setter(Control.TemplateProperty, template));
            
            Style = style;
        }

    }
}
