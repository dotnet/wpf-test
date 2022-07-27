// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

namespace DRT
{
    public class DrtDatePickerSuite : DrtTestSuite
    {
        public DrtDatePickerSuite()
            : base("DatePicker")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            string fileName = DRT.BaseDirectory + "DrtDatePicker.xaml";
            LoadXamlPage(fileName);

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();

                tests.Add(new DrtTest(TestTypingInvalidDate));
                tests.Add(new DrtTest(TestTypingValidDate));
                tests.Add(new DrtTest(TestPopupInit));
                tests.Add(new DrtTest(TestDisplayDate));
                tests.Add(new DrtTest(TestDisplayDateRange));
                tests.Add(new DrtTest(TestBlackoutDates));
                tests.Add(new DrtTest(TestBug640652));
                tests.Add(new DrtTest(TestBug658309_751375));
                tests.Add(new DrtTest(TestBug751369));
                tests.Add(new DrtTest(TestBug614706));
                return tests.ToArray();
            }
            else
            {
                return new DrtTest[]{};
            }
        }

        private void LoadXamlPage(string fileName)
        {
            System.IO.Stream stream = File.OpenRead(fileName);
            Visual root = (Visual)XamlReader.Load(stream);
            InitTree(root);

            DRT.Show(root);
        }

        private void InitTree(DependencyObject root)
        {
            _testDatePicker = (DatePicker)DRT.FindVisualByID("TestDatePicker", root);
            _testButton = (Button)DRT.FindVisualByID("TestButton", root);
        }


        private void TestTypingInvalidDate()
        {
            DRT.PrepareToSendInput();
            
            ClickDatePicker();

            DRT.Assert(string.IsNullOrEmpty(_testDatePicker.Text));
            
            // type valid date
            string text = "asdf";
            DRT.SendString(text);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.Assert(_testDatePicker.Text == text);
            DRT.Assert(_testDatePicker.SelectedDate == null);

            ClickButton();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.Assert(string.IsNullOrEmpty(_testDatePicker.Text));
        }

        private void TestTypingValidDate()
        {
            ClickDatePicker();

            DRT.Assert(string.IsNullOrEmpty(_testDatePicker.Text));

            // type invalid date
            var target = DateTime.Today + TimeSpan.FromDays(1);
            string text = target.ToString("d");
            DRT.SendString(text);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.Assert(_testDatePicker.Text == text);

            ClickButton();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.Assert(_testDatePicker.Text == text);
            DRT.Assert(_testDatePicker.SelectedDate == target);
        }

        private void TestPopupInit()
        {
            _testDatePicker.IsTodayHighlighted = false;
            _testDatePicker.SelectedDate = new DateTime(2009, 8, 14);
            _testDatePicker.DisplayDate = new DateTime(2009, 10, 10);
            _testDatePicker.IsDropDownOpen = true;
            
            DrtBase.WaitForRender();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare(_testDatePicker.DisplayDate, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

            _testDatePicker.IsDropDownOpen = false;
            _testDatePicker.DisplayDate = new DateTime(2009, 9, 10);
            _testDatePicker.SelectedDate = new DateTime(2009, 8, 13);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // setting SelectedDate should also set DisplayDate.
            DRT.Assert(_testDatePicker.DisplayDate == _testDatePicker.SelectedDate);

            _testDatePicker.IsDropDownOpen = true;
            DrtBase.WaitForRender();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare((DateTime)_testDatePicker.SelectedDate, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

            _testDatePicker.IsDropDownOpen = false;
            _testDatePicker.DisplayDate = new DateTime(2009, 11, 1);
            _testDatePicker.SelectedDate = new DateTime(2009, 11, 2);
            _testDatePicker.IsDropDownOpen = true;
            DrtBase.WaitForRender();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare((DateTime)_testDatePicker.SelectedDate, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);
        }

        private void TestDisplayDate()
        {
            _testDatePicker.IsDropDownOpen = true;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testDatePicker.IsDropDownOpen);
            _testDatePicker.SelectedDate = DateTime.Today;
            for (int i = 0; i < 40; i++)
            {
                var target = DateTime.Today + TimeSpan.FromDays(i);
                
                _testDatePicker.DisplayDate = target;
                DrtBase.WaitForRender();
            }
        }

        private void TestDisplayDateRange()
        {
            _testDatePicker.IsDropDownOpen = true;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testDatePicker.IsDropDownOpen);
            
            _testDatePicker.DisplayDateStart = DateTime.Today;

            for (int i = 0; i < 40; i++)
            {
                var target = DateTime.Today + TimeSpan.FromDays(i);
                _testDatePicker.DisplayDateEnd = target; 
                DrtBase.WaitForRender();
            }
        }

        private void TestBlackoutDates()
        {
            _testDatePicker.IsDropDownOpen = true;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testDatePicker.IsDropDownOpen);
            _testDatePicker.SelectedDate = null;
            _testDatePicker.DisplayDateStart = null;
            _testDatePicker.DisplayDateEnd = null;
            for (int i = 0; i < 40; i++)
            {
                var target = DateTime.Today + TimeSpan.FromDays(i);
                _testDatePicker.BlackoutDates.Add(new CalendarDateRange(target));
                _testDatePicker.DisplayDate = target;
                DrtBase.WaitForRender();
            }

            _testDatePicker.BlackoutDates.Clear();
        }

        private void TestBug640652()
        {
            _testDatePicker.IsDropDownOpen = false;
            _testDatePicker.SelectedDate = new DateTime(2009, 8, 30);
            _testDatePicker.DisplayDate = new DateTime(2009, 8, 29);
            _testDatePicker.IsDropDownOpen = true;

            DrtBase.WaitForRender();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare((DateTime)_testDatePicker.SelectedDate, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

            _testDatePicker.IsDropDownOpen = false;
        }

        private void TestBug658309_751375()
        {
            _testDatePicker.Text = "";
            _testDatePicker.IsDropDownOpen = false;
            DrtBase.WaitForRender();

            ClickDatePicker();

            // type valid date
            var target = new DateTime(2009, 1, 1);
            string text = target.ToString("d");
            DRT.SendString(text);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            ClickDatePickerDropdown();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare(target, (DateTime)_testDatePicker.SelectedDate) == 0);
            DRT.Assert(DateTime.Compare(target, _testDatePicker.DisplayDate) == 0);
            DRT.Assert(DateTime.Compare(target, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

            DRT.PressKey(Key.Escape);
            DrtBase.WaitForRender();

            _testDatePicker.Text = "";
            target = new DateTime(2010, 1, 1);
            text = target.ToString("d");
            DRT.SendString(text);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
          
            ClickDatePickerDropdown();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare(target, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

            DRT.PressKey(Key.Escape);
            DrtBase.WaitForRender();
        }

        private void TestBug751369()
        {
            _testDatePicker.SelectedDate = new DateTime(2009, 8, 10);
            _testDatePicker.DisplayDate = new DateTime(2009, 8, 10);
            _testDatePicker.IsDropDownOpen = true;
            DrtBase.WaitForRender();

            // put the calendar in Year mode
            DRT.SendKeyboardInput(Key.LeftCtrl, true);
            DRT.PressKey(Key.Up);
            DRT.SendKeyboardInput(Key.LeftCtrl, false);
            DrtBase.WaitForRender();

            DRT.PressKey(Key.Enter);
            DrtBase.WaitForRender();

            // make sure the popup is still open & the focus hasn't changed.
            DRT.Assert(_testDatePicker.IsDropDownOpen == true);
            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare((DateTime)_testDatePicker.SelectedDate, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);
        }

        private void TestBug614706()
        {
            _testDatePicker.IsDropDownOpen = false;
            ClickDatePicker();

            DRT.Assert(_testDatePicker.IsKeyboardFocusWithin);

            ClickDatePickerDropdown();
            
            DRT.Assert(_testDatePicker.IsDropDownOpen == true);

            ClickButton();

            DRT.Assert(_testDatePicker.IsDropDownOpen == false);
            DRT.Assert(!_testDatePicker.IsKeyboardFocusWithin);
        }

        private void ClickDatePicker()
        {
            // click on the datepicker.
            DRT.MoveMouse(_testDatePicker, 1.0 / 4.0, 1.0 / 16.0);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private void ClickDatePickerDropdown()
        {
            // click on the datepicker.
            DRT.MoveMouse(_testDatePicker, 15.0 / 16.0, 1.0 / 16.0);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private void ClickButton()
        {
            // click on the button.
            DRT.MoveMouse(_testButton, 0.5, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private DatePicker _testDatePicker;
        private Button _testButton;
    }
}
