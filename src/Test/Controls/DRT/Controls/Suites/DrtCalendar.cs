// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class DrtCalendarSuite : DrtTestSuite
    {
        public DrtCalendarSuite()
            : base("Calendar")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            string fileName = DRT.BaseDirectory + "DrtCalendar.xaml";
            LoadXamlPage(fileName);

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();

                // Blackout dates, selected date & display date are already tested via DatePicker
                tests.Add(new DrtTest(TestKeyboardNavigation));
                tests.Add(new DrtTest(TestSelectionModes));
                tests.Add(new DrtTest(TestDisplayModes));
                tests.Add(new DrtTest(TestSpecialDays));
                tests.Add(new DrtTest(TestBug733526));
                tests.Add(new DrtTest(TestBug733527));
                tests.Add(new DrtTest(TestBug751373));
                tests.Add(new DrtTest(TestBug739537));
                tests.Add(new DrtTest(TestBug752690));

                return tests.ToArray();
            }
            else
            {
                _testCalendar.SelectionMode = CalendarSelectionMode.MultipleRange;
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
            _testCalendar = (Calendar)DRT.FindVisualByID("TestCalendar", root);
            _testButton = (Button)DRT.FindVisualByID("TestButton", root);
        }


        private void TestKeyboardNavigation()
        {
            DRT.PrepareToSendInput();

            ClickCalendar();

            for (int i = 0; i < 32; i++)
            {
                DRT.SendKeyboardInput(Key.Right, true);
                DRT.SendKeyboardInput(Key.Right, false);
                DrtBase.WaitForRender();
            }

            for (int i = 0; i < 2; i++)
            {
                DRT.SendKeyboardInput(Key.Right, true);
                DRT.SendKeyboardInput(Key.Right, false);
                DrtBase.WaitForRender();

                DRT.SendKeyboardInput(Key.Down, true);
                DRT.SendKeyboardInput(Key.Down, false);
                DrtBase.WaitForRender();

                DRT.SendKeyboardInput(Key.Left, true);
                DRT.SendKeyboardInput(Key.Left, false);
                DrtBase.WaitForRender();

                DRT.SendKeyboardInput(Key.Up, true);
                DRT.SendKeyboardInput(Key.Up, false);
                DrtBase.WaitForRender();
            }
        }

        private void TestSelectionModes()
        {
            ClickCalendar();
            ClickButton();

            _testCalendar.DisplayDate = DateTime.Today;
            _testCalendar.SelectedDate = DateTime.Today;
            DRT.Assert(_testCalendar.SelectedDate != null);
            _testCalendar.SelectionMode = CalendarSelectionMode.None;
            DRT.Assert(_testCalendar.SelectedDate == null);

            _testCalendar.SelectionMode = CalendarSelectionMode.SingleDate;
            DRT.Assert(_testCalendar.SelectedDates.Count == 0);
            _testCalendar.SelectedDate = DateTime.Today;
            DRT.Assert(_testCalendar.SelectedDates.Count == 1);
            _testCalendar.SelectedDate = null;
            DRT.Assert(_testCalendar.SelectedDates.Count == 0);
            _testCalendar.SelectedDates.Add(DateTime.Today);
            DRT.Assert(_testCalendar.SelectedDate == DateTime.Today);
            bool invalid = false;
            try
            {
                _testCalendar.SelectedDates.Add(DateTime.Today + TimeSpan.FromDays(1));
            }
            catch
            {
                invalid = true;
            }
            DRT.Assert(invalid);
            DRT.Assert(_testCalendar.SelectedDates.Count == 1); // make sure it wasn't reset

            _testCalendar.SelectionMode = CalendarSelectionMode.SingleRange;
            
           _testCalendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today + TimeSpan.FromDays(1));
            DRT.Assert(_testCalendar.SelectedDates.Count == 2);

            _testCalendar.SelectionMode = CalendarSelectionMode.MultipleRange;

            _testCalendar.SelectedDates.AddRange(DateTime.Today + TimeSpan.FromDays(0), DateTime.Today + TimeSpan.FromDays(1));
            _testCalendar.SelectedDates.AddRange(DateTime.Today + TimeSpan.FromDays(4), DateTime.Today + TimeSpan.FromDays(6));
            _testCalendar.SelectedDates.AddRange(DateTime.Today + TimeSpan.FromDays(9), DateTime.Today + TimeSpan.FromDays(10));

            DRT.Assert(_testCalendar.SelectedDates.Count == 7);

            _testCalendar.SelectedDates.Clear();
            DRT.Assert(_testCalendar.SelectedDates.Count == 0);
            DRT.Assert(_testCalendar.SelectedDate == null);

        }

        private void TestDisplayModes()
        {
            _testCalendar.DisplayMode = CalendarMode.Year;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            ClickCalendar();

            for (int i = 0; i < 14; i++)
            {
                DRT.SendKeyboardInput(Key.Right, true);
                DRT.SendKeyboardInput(Key.Right, false);
                DrtBase.WaitForRender();
            }

            _testCalendar.DisplayMode = CalendarMode.Decade;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            ClickCalendar();

            for (int i = 0; i < 14; i++)
            {
                DRT.SendKeyboardInput(Key.Right, true);
                DRT.SendKeyboardInput(Key.Right, false);
                DrtBase.WaitForRender();
            }

            _testCalendar.DisplayMode = CalendarMode.Month;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private void TestSpecialDays()
        {
            ClickCalendar();
            _testCalendar.DisplayDate = DateTime.Today + TimeSpan.FromDays(1);

            for (int i = 0; i < 5; i++)
            {
                _testCalendar.IsTodayHighlighted = !_testCalendar.IsTodayHighlighted;
                DrtBase.WaitForRender();
            }

            _testCalendar.FirstDayOfWeek = DayOfWeek.Monday;
            DrtBase.WaitForRender();
            _testCalendar.FirstDayOfWeek = DayOfWeek.Tuesday;
            DrtBase.WaitForRender();
            _testCalendar.FirstDayOfWeek = DayOfWeek.Wednesday;
            DrtBase.WaitForRender();
            _testCalendar.FirstDayOfWeek = DayOfWeek.Thursday;
            DrtBase.WaitForRender();
            _testCalendar.FirstDayOfWeek = DayOfWeek.Friday;
            DrtBase.WaitForRender();
            _testCalendar.FirstDayOfWeek = DayOfWeek.Saturday;
            DrtBase.WaitForRender();
            _testCalendar.FirstDayOfWeek = DayOfWeek.Sunday;
            DrtBase.WaitForRender();
        }

        private void TestBug733526()
        {
            _testCalendar.DisplayMode = CalendarMode.Month;
            _testCalendar.SelectedDate = new DateTime(2009, 8, 12);
            _testCalendar.DisplayDate = new DateTime(2009, 8, 10);

            CycleTab();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare((DateTime)_testCalendar.SelectedDate, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

            _testCalendar.DisplayDate = new DateTime(2009, 9, 10);

            CycleTab();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare(_testCalendar.DisplayDate, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);
        }

        private void TestBug733527()
        {
            _testCalendar.SelectedDate = new DateTime(2009, 8, 12);
            _testCalendar.DisplayDateStart = new DateTime(2009, 7, 20); 
            _testCalendar.DisplayDate = new DateTime(2009, 7, 14);

            CycleTab();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare((DateTime)_testCalendar.DisplayDateStart, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

            _testCalendar.DisplayDateStart = null;
        }

        private void TestBug751373()
        {
            ClickCalendar();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            _testCalendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            _testCalendar.DisplayMode = CalendarMode.Month;
            _testCalendar.SelectedDate = new DateTime(2009, 1, 1);
            _testCalendar.DisplayDate = new DateTime(2009, 1, 2);
            Keyboard.Focus(_testCalendar); // should move  focus to the SelectedDate

            DrtBase.WaitForRender();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare((DateTime)_testCalendar.SelectedDate, ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

            ClickCalendarDay(new DateTime(2009, 1, 7));

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare(new DateTime(2009, 1, 7), ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

            ClickCalendarNextMonth();

            DRT.SendKeyboardInput(Key.LeftShift, true);
            ClickCalendarDay(new DateTime(2009, 2, 4));
            DRT.SendKeyboardInput(Key.LeftShift, false);

            DrtBase.WaitForRender();

            DRT.Assert(Keyboard.FocusedElement is CalendarDayButton);
            DRT.Assert(DateTime.Compare(new DateTime(2009, 2, 4), ((DateTime)((CalendarDayButton)Keyboard.FocusedElement).DataContext)) == 0);

        }

        private void TestBug739537()
        {
            _testCalendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            _testCalendar.DisplayMode = CalendarMode.Month;
            _testCalendar.SelectedDate = null;
            _testCalendar.DisplayDate = new DateTime(2009, 1, 1);
            
            DragMouse(_testCalendar, new Vector(1.0 / 2.0, 1.0 / 2.0), new Vector(1.3, 0.0));

            DRT.Assert(_testCalendar.SelectedDates.Count > 1);
        }

        private void TestBug752690()
        {
            _testCalendar.DisplayMode = CalendarMode.Month;
            _testCalendar.SelectedDate = null;
            _testCalendar.DisplayDate = new DateTime(2009, 2, 2);

            ClickCalendar();
            
            DRT.Assert(_testCalendar.DisplayDate.Month == 2);

            int abort = 10;
            while (abort-- > 0 && _testCalendar.DisplayDate.Month == 2)
            {
                DRT.PressKey(Key.Up);
                DrtBase.WaitForRender();
            }

            DRT.Assert(abort != 0);
            DRT.Assert(_testCalendar.DisplayDate.Month == 1);
            DRT.Assert(_testCalendar.DisplayDate.Day == 1);
        }

        private void DragMouse(FrameworkElement e, Vector start, Vector end)
        {
            DRT.MoveMouse(_testCalendar, start.X, start.Y);
            DRT.MouseButtonDown();
            DrtBase.WaitForRender();

            for (int i = 0; i < 25; i++)
            {
                Vector point = start + (end - start) * ((double)i / 24.0);
                DRT.MoveMouse(_testCalendar, point.X, point.Y);
                DrtBase.WaitForRender();
            }

            DRT.MouseButtonUp();
            DrtBase.WaitForRender();
        }

        private void CycleTab()
        {
            int abort = 50;
            do
            {
                DRT.SendKeyboardInput(Key.Tab, true);
                DRT.SendKeyboardInput(Key.Tab, false);
                DrtBase.WaitForPriority(DispatcherPriority.Background);
            } while (abort-- > 0 && !_testCalendar.IsKeyboardFocusWithin);

            DRT.Assert(abort != 0);
        }

        private bool ClickCalendarDay(DateTime day)
        {
            CalendarDayButton dayButton = FindDayButton(_testCalendar, day);

            if (dayButton != null)
            {
                // click on the datepicker.
                DRT.MoveMouse(dayButton, 1.0 / 2.0, 1.0 / 2.0);
                DrtBase.WaitForPriority(DispatcherPriority.Background);
                DRT.ClickMouse();
                DrtBase.WaitForPriority(DispatcherPriority.Background);

                return true;
            }

            return false;
        }

        private CalendarDayButton FindDayButton(DependencyObject parent, DateTime day)
        {
            CalendarDayButton calendarDay = parent as CalendarDayButton;

            if (calendarDay != null && calendarDay.DataContext is DateTime && (DateTime)calendarDay.DataContext == day)
            {
                return calendarDay;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null)
                {
                    var childDay = FindDayButton(child, day);
                    if (childDay != null)
                    {
                        return childDay;
                    }
                }
            }

            return null;
        }

        private void ClickCalendar()
        {
            // click on the datepicker.
            DRT.MoveMouse(_testCalendar, 1.0 / 2.0, 1.0 / 2.0);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private void ClickCalendarNextMonth()
        {
            // click on the datepicker.
            DRT.MoveMouse(_testCalendar, 15.0 / 16.0, 1.0 / 16.0);
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

        private Calendar _testCalendar;
        private Button _testButton;
    }
}
