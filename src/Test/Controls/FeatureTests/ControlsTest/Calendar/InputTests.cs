using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    // [ DISABLED_WHILE_PORTING ]
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "InputTests", Keywords = "Localization_Suite,MicroSuite", Disabled=true)]
    public class CalendarInputTests : CalendarTest
    {
        public CalendarInputTests()
            : base()
        {
            this.RunSteps += DayButton_Click_None;
            this.RunSteps += DayButton_Click_SingleDate;
            this.RunSteps += DayButton_Click_SingleRange;
            this.RunSteps += DayButton_Click_Inactive;
            this.RunSteps += DayButton_Click_Blackout;
            this.RunSteps += DayButton_LeftMouseDown_SingleRange;
            this.RunSteps += DayButton_LeftMouseDown_SingleDate;
            this.RunSteps += DayButton_ShiftLeftMouseDown_SingleDate;
            this.RunSteps += DayButton_LeftMouseDownUp_SingleRange;
            this.RunSteps += DayButton_ShiftLeftMouseDownUp_SingleRange;
            this.RunSteps += DayButton_ShiftLeftMouseDownUp_MultipleRange;
            this.RunSteps += DayButton_CtrlLeftMouseDownUp_SingleRange;
            this.RunSteps += DayButton_CtrlClick_MultipleRange;
            this.RunSteps += Header_Click_Month;
            this.RunSteps += Header_Click_Year;
            this.RunSteps += Header_Click_Decade;
            this.RunSteps += CalendarButton_Click_Year;
            this.RunSteps += CalendarButton_Click_Decade;
            this.RunSteps += DayButton_Directional_Month;
            this.RunSteps += DayButton_Shift_Directional_Month_SingleDate;
            this.RunSteps += DayButton_Shift_Directional_Month_SingleRange;
            this.RunSteps += DayButton_Directional_Year;
            this.RunSteps += DayButton_Directional_Decade;
            this.RunSteps += Calendar_Switch_DisplayModes;
            this.RunSteps += KeyBoardSelection_MultipleMonths_SingleRange_01;
            this.RunSteps += KeyBoardSelection_MultipleMonths_MultipleRange_01;
            this.RunSteps += KeyBoardSelection_MultipleMonths_SingleRange_02;
            this.RunSteps += KeyBoardSelection_MultipleMonths_MultipleRange_02;
            this.RunSteps += DayButton_Deselect_CtrlClick_SingleDate;
            this.RunSteps += DayButton_Deselect_CtrlClick_MultipleRange;
        }

        /// <summary>
        /// Click DayButton with SelectionMode.None
        /// </summary>
        public TestResult DayButton_Click_None()
        {
            DateTime displaydate = new DateTime(2001, 12, 01);
            DateTime targetdate = new DateTime(2001, 12, 01);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = displaydate;
            calendar.SelectionMode = CalendarSelectionMode.None;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            InputHelper.Click(tcalendar.CalendarDayButtonByDate(targetdate));

            Assert.IsFalse(handled, "SelectedDateChanged event was not fired.");
            Assert.IsFalse(calendar.SelectedDate.HasValue, "SelectedDate should not have a value");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click DayButton with SelectionMode.SingleDate
        /// </summary>
        public TestResult DayButton_Click_SingleDate()
        {
            DateTime selecteddate = new DateTime(2001, 12, 01);
            DateTime targetdate = new DateTime(2001, 12, 25);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = selecteddate;
            calendar.SelectedDate = selecteddate;
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            InputHelper.Click(tcalendar.CalendarDayButtonByDate(targetdate));

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDate.Value, targetdate);
            Assert.AreEqual(calendar.SelectedDates.Count, 1);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click DayButton with SelectionMode.SingleRange
        /// </summary>
        public TestResult DayButton_Click_SingleRange()
        {
            DateTime selecteddate = new DateTime(2001, 05, 01);
            DateTime targetdate = new DateTime(2001, 05, 20);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = selecteddate;
            calendar.SelectedDate = selecteddate;
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            InputHelper.Click(tcalendar.CalendarDayButtonByDate(targetdate));

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDate.Value, targetdate);
            Assert.AreEqual(calendar.SelectedDates.Count, 1);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click Inactive DayButton 
        /// </summary>
        public TestResult DayButton_Click_Inactive()
        {
            DateTime displaydate = new DateTime(2008, 01, 01);

            bool displaychanged = false;

            Calendar calendar = new Calendar();
            calendar.DisplayDate = displaydate;
            calendar.DisplayDateChanged += (sender, e) =>
            {
                displaychanged = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.Month.Days[0]);
            Assert.IsTrue(displaychanged, "DisplayDateChanged event did not fire.");

            displaychanged = false;

            InputHelper.Click(tcalendar.Month.Days[tcalendar.Month.Days.Count - 1]);
            Assert.IsTrue(displaychanged, "DisplayDateChanged event did not fire.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click blacked out DayButton 
        /// </summary>
        public TestResult DayButton_Click_Blackout()
        {
            DateTime targetdate = new DateTime(2001, 12, 25);

            bool selectedchanged = false;

            Calendar calendar = new Calendar();
            calendar.DisplayDate = targetdate;
            calendar.BlackoutDates.Add(new CalendarDateRange(targetdate.AddDays(-1)));
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                selectedchanged = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(targetdate));
            Assert.IsTrue(selectedchanged, "SelectedDatesChanged did not fire.");
            Assert.AreEqual(calendar.SelectedDates.Count, 1);

            selectedchanged = false;

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(targetdate.AddDays(-1)));
            Assert.IsFalse(selectedchanged, "SelectedDatesChanged should not have fired.");
            Assert.AreEqual(calendar.SelectedDates.Count, 1);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// MouseDown on DayButton with SelectionMode.SingleDate
        /// </summary>
        public TestResult DayButton_LeftMouseDown_SingleDate()
        {
            DateTime selecteddate = new DateTime(2003, 07, 01);
            DateTime targetdate = new DateTime(2003, 07, 25);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = selecteddate;
            calendar.SelectedDate = selecteddate;
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(targetdate));

            Assert.AreEqual(calendar.SelectedDate.Value, targetdate);
            Assert.AreEqual(calendar.SelectedDates.Count, 1);

            InputHelper.LeftMouseUp();
            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// MouseDown on DayButton with SelectionMode.SingleRange
        /// </summary>
        public TestResult DayButton_LeftMouseDown_SingleRange()
        {
            DateTime selecteddate = new DateTime(2001, 05, 01);
            DateTime targetdate = new DateTime(2001, 05, 20);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = selecteddate;
            calendar.SelectedDate = selecteddate;
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(targetdate));

            Assert.IsFalse(calendar.SelectedDate.HasValue, "SelectedDate should not have a value");
            Assert.AreEqual(calendar.SelectedDates.Count, 0);

            InputHelper.LeftMouseUp();

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDate.Value, targetdate);
            Assert.AreEqual(calendar.SelectedDates.Count, 1);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// SHIFT + MouseDown on DayButton with SelectionMode.SingleDate
        /// </summary>
        public TestResult DayButton_ShiftLeftMouseDown_SingleDate()
        {
            DateTime selecteddate = new DateTime(2003, 07, 01);
            DateTime targetdate = new DateTime(2003, 07, 25);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = selecteddate;
            calendar.SelectedDate = selecteddate;
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(targetdate));

            Assert.AreEqual(calendar.SelectedDate.Value, targetdate);
            Assert.AreEqual(calendar.SelectedDates.Count, 1);

            InputHelper.LeftMouseUp();
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);
            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// CTRL + MouseDown on DayButton with SelectionMode.SingleDate
        /// </summary>
        public TestResult DayButton_CtrlLeftMouseDown_SingleDate()
        {
            DateTime selecteddate = new DateTime(2003, 07, 01);
            DateTime targetdate = new DateTime(2003, 07, 25);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = selecteddate;
            calendar.SelectedDate = selecteddate;
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(targetdate));

            Assert.AreEqual(calendar.SelectedDate.Value, targetdate);
            Assert.AreEqual(calendar.SelectedDates.Count, 1);

            InputHelper.LeftMouseUp();
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);
            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// MouseDown and MouseUp on DayButton with SelectionMode.SingleRange
        /// </summary>
        public TestResult DayButton_LeftMouseDownUp_SingleRange()
        {
            DateTime selecteddate = new DateTime(2007, 05, 01);
            DateTime startdate = new DateTime(2007, 05, 20);
            DateTime enddate = new DateTime(2007, 05, 25);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = selecteddate;
            calendar.SelectedDate = selecteddate;
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(startdate));
            Assert.IsFalse(calendar.SelectedDate.HasValue, "SelectedDate should not have a value");
            Assert.AreEqual(calendar.SelectedDates.Count, 0);

            InputHelper.LeftMouseUp(tcalendar.CalendarDayButtonByDate(enddate));

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDate.Value, startdate);
            Assert.AreEqual(calendar.SelectedDates.Count, 6);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// SHIFT + MouseDown and MouseUp on DayButton with SelectionMode.SingleRange
        /// </summary>
        public TestResult DayButton_ShiftLeftMouseDownUp_SingleRange()
        {
            DateTime displaydate = new DateTime(2008, 09, 01);
            DateTime startdate = new DateTime(2008, 09, 02);
            DateTime enddate = startdate.AddDays(10);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = displaydate;
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(startdate));

            Assert.IsFalse(calendar.SelectedDate.HasValue, "SelectedDate should not have a value");
            Assert.AreEqual(calendar.SelectedDates.Count, 0);

            InputHelper.LeftMouseUp(tcalendar.CalendarDayButtonByDate(enddate));
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDate.Value, displaydate);
            Assert.AreEqual(calendar.SelectedDates.Count, 12);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// SHIFT + MouseDown and MouseUp on DayButton with SelectionMode.MultipleRange
        /// </summary>
        public TestResult DayButton_ShiftLeftMouseDownUp_MultipleRange()
        {
            DateTime displaydate = new DateTime(2008, 09, 01);
            DateTime startdate_01 = new DateTime(2008, 09, 02);
            DateTime enddate_01 = new DateTime(2008, 09, 04);
            DateTime startdate_02 = new DateTime(2008, 09, 07);
            DateTime enddate_02 = new DateTime(2008, 09, 15);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = displaydate;
            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(startdate_01));

            Assert.IsFalse(calendar.SelectedDate.HasValue, "SelectedDate should not have a value");
            Assert.AreEqual(calendar.SelectedDates.Count, 0);

            InputHelper.LeftMouseUp(tcalendar.CalendarDayButtonByDate(enddate_01));
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDate.Value, displaydate);
            Assert.AreEqual(calendar.SelectedDates.Count, 4);

            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(startdate_02));

            Assert.AreEqual(calendar.SelectedDates.Count, 0);

            InputHelper.LeftMouseUp(tcalendar.CalendarDayButtonByDate(enddate_02));
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDate.Value, displaydate);
            Assert.AreEqual(calendar.SelectedDates.Count, 15);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// CTRL + MouseDown and MouseUp on DayButton with SelectionMode.SingleRange
        /// </summary>
        public TestResult DayButton_CtrlLeftMouseDownUp_SingleRange()
        {
            DateTime selecteddate = new DateTime(2007, 05, 01);
            DateTime new_selecteddate = new DateTime(2007, 05, 20);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = selecteddate;
            calendar.SelectedDate = selecteddate;
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(new_selecteddate));

            Assert.AreEqual(calendar.SelectedDates.Count, 0);
            Assert.IsFalse(calendar.SelectedDate.HasValue, "SelectedDate should not have a value");

            InputHelper.LeftMouseUp(tcalendar.CalendarDayButtonByDate(new_selecteddate));
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDates.Count, 1);
            Assert.IsTrue(calendar.SelectedDate.HasValue, "SelectedDate should have a value");
            Assert.AreEqual(calendar.SelectedDate.Value, new_selecteddate);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// CTRL + Click DayButton with SelectionMode.MultipleRange
        /// </summary>
        public TestResult DayButton_CtrlClick_MultipleRange()
        {
            DateTime displaydate = new DateTime(2007, 05, 01);
            DateTime selecteddate = new DateTime(2007, 05, 10);

            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = displaydate;
            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            Assert.AreEqual(calendar.SelectedDates.Count, 0);
            Assert.IsFalse(calendar.SelectedDate.HasValue, "SelectedDate should not have a value");

            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.Click(tcalendar.CalendarDayButtonByDate(selecteddate));
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDates.Count, 1);
            Assert.IsTrue(calendar.SelectedDate.HasValue, "SelectedDate should have a value");
            Assert.AreEqual(calendar.SelectedDate.Value, selecteddate);

            handled = false;
            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.Click(tcalendar.CalendarDayButtonByDate(selecteddate));
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.IsTrue(handled, "SelectedDateChanged event was not fired.");
            Assert.AreEqual(calendar.SelectedDates.Count, 0);
            Assert.IsFalse(calendar.SelectedDate.HasValue, "SelectedDate should not have a value");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click Header DisplayMode.Month
        /// </summary>
        public TestResult Header_Click_Month()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            Assert.IsTrue(tcalendar.HeaderButton.IsEnabled, "Header button should be enabled");
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Month);

            InputHelper.Click(tcalendar.HeaderButton);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click Header DisplayMode.Year
        /// </summary>
        public TestResult Header_Click_Year()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Year;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            Assert.IsTrue(tcalendar.HeaderButton.IsEnabled, "Header button should be enabled");
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);

            InputHelper.Click(tcalendar.HeaderButton);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click Header DisplayMode.Decade
        /// </summary>
        public TestResult Header_Click_Decade()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Decade;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            Assert.IsFalse(tcalendar.HeaderButton.IsEnabled, "Header button should not be enabled");
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);

            InputHelper.Click(tcalendar.HeaderButton);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click CalendarButton DisplayMode.Year
        /// </summary>
        public TestResult CalendarButton_Click_Year()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Year;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);

            InputHelper.Click(tcalendar.Year.Items[4]);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Month);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click CalendarButton DisplayMode.Decade
        /// </summary>
        public TestResult CalendarButton_Click_Decade()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Decade;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);

            InputHelper.Click(tcalendar.Year.Items[9]);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// KeyBoard Directional Support in DisplayMode.Month
        /// </summary>
        public TestResult DayButton_Directional_Month()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DirectionalMonthNavigation(calendar, CalendarSelectionMode.SingleDate);
            DirectionalMonthNavigation(calendar, CalendarSelectionMode.SingleRange);
            DirectionalMonthNavigation(calendar, CalendarSelectionMode.MultipleRange);

            ResetTest();
            return TestResult.Pass;
        }

        private void DirectionalMonthNavigation(Calendar calendar, CalendarSelectionMode mode)
        {
            bool handled = false;

            DateTime date = new DateTime(2008, 09, 15);
            calendar.SelectionMode = mode;
            calendar.DisplayDate = date;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));

            InputHelper.PressKey(System.Windows.Input.Key.Up);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddDays(-7));

            date = calendar.SelectedDate.Value;
            handled = false;
            InputHelper.PressKey(System.Windows.Input.Key.Right);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddDays(1));

            date = calendar.SelectedDate.Value;
            handled = false;
            InputHelper.PressKey(System.Windows.Input.Key.Left);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddDays(-1));

            date = calendar.SelectedDate.Value;
            handled = false;
            InputHelper.PressKey(System.Windows.Input.Key.Down);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddDays(7));

            date = calendar.SelectedDate.Value;
            handled = false;
            InputHelper.PressKey(System.Windows.Input.Key.Home);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, new DateTime(2008, 09, 1));

            date = calendar.SelectedDate.Value;
            handled = false;
            InputHelper.PressKey(System.Windows.Input.Key.End);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, new DateTime(2008, 09, 30));

            date = calendar.SelectedDate.Value;
            handled = false;
            InputHelper.PressKey(System.Windows.Input.Key.PageUp);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddMonths(-1));

            date = calendar.SelectedDate.Value;
            handled = false;
            InputHelper.PressKey(System.Windows.Input.Key.PageDown);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddMonths(1));
        }

        /// <summary>
        /// SHIFT + KeyBoard Directional Support in DisplayMode.Month SingleDate
        /// </summary>
        public TestResult DayButton_Shift_Directional_Month_SingleDate()
        {
            bool handled = false;
            DateTime date = new DateTime(2008, 09, 15);

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.DisplayDate = date;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddDays(-7));

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Right);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddDays(1));

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Left);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddDays(-1));

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddDays(7));

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Home);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, new DateTime(2008, 09, 1));

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.End);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, new DateTime(2008, 09, 30));

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.PageUp);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddMonths(-1));

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.PageDown);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDate.Value, date.AddMonths(1));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// SHIFT + KeyBoard Directional Support in DisplayMode.Month SingleRange
        /// </summary>
        public TestResult DayButton_Shift_Directional_Month_SingleRange()
        {
            bool handled = false;
            DateTime date = new DateTime(2008, 09, 15);

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.DisplayDate = date;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDates.Count, 8);

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Right);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDates.Count, 2);

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Left);
            InputHelper.PressKey(System.Windows.Input.Key.Left);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDates.Count, 3);

            date = calendar.SelectedDate.Value;
            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDates.Count, 15);

            date = new DateTime(2008, 09, 15);
            handled = false;
            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Home);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDates.Count, 15);

            handled = false;

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.End);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.AreEqual(calendar.SelectedDates.Count, 30);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// KeyBoard Directional Support in DisplayMode.Year
        /// </summary>
        public TestResult DayButton_Directional_Year()
        {
            DateTime date = new DateTime(2001, 12, 25);
            Calendar calendar = new Calendar();
            calendar.DisplayDate = date;
            calendar.DisplayMode = CalendarMode.Month;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.HeaderButton);

            CalendarButton cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Month, date.Month);
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year);

            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Month, date.Month - 4);
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.Left);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Month, date.Month - 1);
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Month, date.Month + 4);
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.Right);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Month, date.Month + 1);
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.End);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Month, 12);
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.Home);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Month, 1);
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year);

            int year = ((DateTime)cbtn.DataContext).Year;

            InputHelper.PressKey(System.Windows.Input.Key.PageUp);
            Assert.AreEqual(tcalendar.CurrentYear, year - 1);

            year = ((DateTime)cbtn.DataContext).Year;

            InputHelper.PressKey(System.Windows.Input.Key.PageDown);
            Assert.AreEqual(tcalendar.CurrentYear, year + 1);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// KeyBoard Directional Support in DisplayMode.Decade
        /// </summary>
        public TestResult DayButton_Directional_Decade()
        {
            DateTime date = new DateTime(2001, 12, 25);
            Calendar calendar = new Calendar();
            calendar.DisplayDate = date;
            calendar.DisplayMode = CalendarMode.Year;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.HeaderButton);

            CalendarButton cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year);

            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year + 4);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.Left);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year - 1);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year - 4);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.Right);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, date.Year + 1);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.End);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, tcalendar.MaxYear);

            date = ((DateTime)cbtn.DataContext);

            InputHelper.PressKey(System.Windows.Input.Key.Home);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.PressKey(System.Windows.Input.Key.Enter);
            InputHelper.Click(tcalendar.HeaderButton);
            InputHelper.Click(tcalendar.HeaderButton);

            cbtn = tcalendar.CalendarButtonWithSelectedDays();
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);
            Assert.IsNotNull(cbtn, "CalendarButton with DisplayDate should not be null");
            Assert.AreEqual(((DateTime)cbtn.DataContext).Year, tcalendar.MinYear);

            int currentmin = tcalendar.MinYear;
            int currentmax = tcalendar.MaxYear;

            InputHelper.PressKey(System.Windows.Input.Key.PageUp);
            Assert.AreEqual(tcalendar.MinYear, currentmin - 10);
            Assert.AreEqual(tcalendar.MaxYear, currentmax - 10);

            currentmin = tcalendar.MinYear;
            currentmax = tcalendar.MaxYear;

            InputHelper.PressKey(System.Windows.Input.Key.PageDown);
            Assert.AreEqual(tcalendar.MinYear, currentmin + 10);
            Assert.AreEqual(tcalendar.MaxYear, currentmax + 10);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Swtich between display modes with CTRL+UP and CTRL+DOWN
        /// </summary>
        public TestResult Calendar_Switch_DisplayModes()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Month);

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            InputHelper.Click(tcalendar.CalendarDayButtonByDate(DateTime.Today));

            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);

            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);

            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);

            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);

            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Month);

            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Month);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Do keyboard selection over multiple months in singlerange mode
        /// </summary>
        public TestResult KeyBoardSelection_MultipleMonths_SingleRange_01()
        {
            bool handled = false;
            DateTime date = new DateTime(2008, 06, 13);

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.DisplayDate = date;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.IsTrue(calendar.SelectedDate.HasValue, "Calendar.SelectedDate should have value");
            Assert.AreEqual(calendar.SelectedDate.Value, date);
            Assert.AreEqual(calendar.SelectedDates.Count, 22);
            Assert.AreEqual(calendar.SelectedDates[0], date);
            Assert.AreEqual(tcalendar.HeaderButton.Content, date.AddMonths(-1).ToString("Y"));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Do keyboard selection over multiple months in multiplerange mode
        /// </summary>
        public TestResult KeyBoardSelection_MultipleMonths_MultipleRange_01()
        {
            bool handled = false;
            DateTime date = new DateTime(2008, 06, 13);

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            calendar.DisplayDate = date;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.PressKey(System.Windows.Input.Key.Up);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.IsTrue(calendar.SelectedDate.HasValue, "Calendar.SelectedDate should have value");
            Assert.AreEqual(calendar.SelectedDate.Value, date);
            Assert.AreEqual(calendar.SelectedDates.Count, 22);
            Assert.AreEqual(calendar.SelectedDates[0], date);
            Assert.AreEqual(tcalendar.HeaderButton.Content, date.AddMonths(-1).ToString("Y"));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Do keyboard selection over multiple months in singlerange mode
        /// </summary>
        public TestResult KeyBoardSelection_MultipleMonths_SingleRange_02()
        {
            bool handled = false;
            DateTime date = new DateTime(2008, 06, 13);

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.DisplayDate = date;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.IsTrue(calendar.SelectedDate.HasValue, "Calendar.SelectedDate should have value");
            Assert.AreEqual(calendar.SelectedDate.Value, date);
            Assert.AreEqual(calendar.SelectedDates.Count, 50);
            Assert.AreEqual(calendar.SelectedDates[0], date);
            Assert.AreEqual(tcalendar.HeaderButton.Content, date.AddMonths(2).ToString("Y"));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Do keyboard selection over multiple months in multiplerange mode
        /// </summary>
        public TestResult KeyBoardSelection_MultipleMonths_MultipleRange_02()
        {
            bool handled = false;
            DateTime date = new DateTime(2008, 06, 13);

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            calendar.DisplayDate = date;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));

            InputHelper.PushKey(System.Windows.Input.Key.LeftShift);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftShift);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.IsTrue(calendar.SelectedDate.HasValue, "Calendar.SelectedDate should have value");
            Assert.AreEqual(calendar.SelectedDate.Value, date);
            Assert.AreEqual(calendar.SelectedDates.Count, 50);
            Assert.AreEqual(calendar.SelectedDates[0], date);
            Assert.AreEqual(tcalendar.HeaderButton.Content, date.AddMonths(2).ToString("Y"));

            ResetTest();
            return TestResult.Pass;
        }


        /// <summary>
        /// De-select day button with CTRL+Click in SingleDate mode
        /// </summary>
        public TestResult DayButton_Deselect_CtrlClick_SingleDate()
        {
            bool handled = false;
            int added = 0;
            int removed = 0;

            DateTime date = new DateTime(2008, 06, 13);

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.DisplayDate = date;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
                added = e.AddedItems.Count;
                removed = e.RemovedItems.Count;
            };
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.IsTrue(calendar.SelectedDate.HasValue, "Calendar.SelectedDate should have value");
            Assert.AreEqual(calendar.SelectedDate.Value, date);
            Assert.AreEqual(calendar.SelectedDates.Count, 1);
            Assert.AreEqual(calendar.SelectedDates[0], date);
            Assert.AreEqual(added, 1, "Added Items should be 1");
            Assert.AreEqual(removed, 0, "Removed Items should be 1");

            handled = false;
            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date));
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.IsFalse(calendar.SelectedDate.HasValue, "Calendar.SelectedDate should not have a value");
            Assert.IsNull(calendar.SelectedDate, "SelectedDate should be null");
            Assert.AreEqual(added, 0, "Added Items should be 0");
            Assert.AreEqual(removed, 1, "Removed Items should be 1");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// De-select day button with CTRL+Click in MultipleRange mode
        /// </summary>
        public TestResult DayButton_Deselect_CtrlClick_MultipleRange()
        {
            bool handled = false;
            int added = 0;
            int removed = 0;

            DateTime date = new DateTime(2008, 06, 13);

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Month;
            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            calendar.DisplayDate = date;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
                added = e.AddedItems.Count;
                removed = e.RemovedItems.Count;
            };
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.LeftMouseDown(tcalendar.CalendarDayButtonByDate(date));
            InputHelper.LeftMouseUp(tcalendar.CalendarDayButtonByDate(date.AddDays(5)));

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.IsTrue(calendar.SelectedDate.HasValue, "Calendar.SelectedDate should have value");
            Assert.AreEqual(calendar.SelectedDate.Value, date);
            Assert.AreEqual(calendar.SelectedDates.Count, 6);
            Assert.AreEqual(calendar.SelectedDates[0], date);
            Assert.AreEqual(added, 6, "Added Items should be 6");
            Assert.AreEqual(removed, 0, "Removed Items should be 1");

            handled = false;
            InputHelper.PushKey(System.Windows.Input.Key.LeftCtrl);
            InputHelper.Click(tcalendar.CalendarDayButtonByDate(date.AddDays(2)));
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftCtrl);

            Assert.IsTrue(handled, "SelectedDatesChanged was not fired");
            Assert.IsTrue(calendar.SelectedDate.HasValue, "Calendar.SelectedDate should have value");
            Assert.AreEqual(calendar.SelectedDate.Value, date);
            Assert.AreEqual(calendar.SelectedDates.Count, 5);
            Assert.AreEqual(calendar.SelectedDates[0], date);
            Assert.AreEqual(added, 0, "Added Items should be 0");
            Assert.AreEqual(removed, 1, "Removed Items should be 1");

            ResetTest();
            return TestResult.Pass;
        }
    }
}
