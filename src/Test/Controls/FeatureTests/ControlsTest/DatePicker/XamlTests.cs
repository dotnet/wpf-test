using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "XamlTests")]
    public class DatePickerXamlTest : DatePickerTest
    {
        public DatePickerXamlTest()
            : base()
        {
            this.RunSteps += DatePickerDefault;
            this.RunSteps += DatePickerBlackoutDates;
            this.RunSteps += DatePickerDisplayDate;
            this.RunSteps += DatePickerDisplayDate2;
            this.RunSteps += DatePickerDisplayDateStartEnd;
            this.RunSteps += DatePickerFirstDayOfWeek;
            this.RunSteps += DatePickerIsTodayHighlighted;
            this.RunSteps += DatePickerSelectedDate;
            this.RunSteps += DatePickerSelectedDate2;
            this.RunSteps += DatePickerIsDropDownOpen;
            this.RunSteps += DatePickerSelectedDateFormat;
            this.RunSteps += DatePickerText;
        }

        /// <summary>
        /// Default DatePicker
        /// Loads this xaml
        /// <wpf:DatePicker x:Name="datepicker" />
        /// </summary>
        public TestResult DatePickerDefault()
        {
            FrameworkElement content = LoadXaml("datepicker.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker BlackoutDates Test
        /// Loads this xaml
        /// <wpf:DatePicker Name="datepicker">
        ///   <wpf:DatePicker.BlackoutDates>
        ///     <wpf:DatePickerDateRange StartDate="9/26/2008" EndDate="9/29/2008"/>
        ///   </wpf:DatePicker.BlackoutDates>
        /// </wpf:DatePicker>
        /// </summary>
        public TestResult DatePickerBlackoutDates()
        {
            FrameworkElement content = LoadXaml("datepickerblackoutdates.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepicker = FindDatePicker("datepicker");

            Assert.IsNotNull(datepicker, "Calendar is null");

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            InputHelper.Click(tdatepicker.Button);

            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be true");

            TemplatedCalendar tcalendar = new TemplatedCalendar(tdatepicker.Calendar);

            Assert.AreEqual(tdatepicker.Calendar.BlackoutDates.Count, 1);
            Assert.IsTrue(tcalendar.CalendarDayButtonByDate(new DateTime(2008, 09, 26)).IsBlackedOut, "DayButton 1 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDate(new DateTime(2008, 09, 27)).IsBlackedOut, "DayButton 1 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDate(new DateTime(2008, 09, 28)).IsBlackedOut, "DayButton 1 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDate(new DateTime(2008, 09, 29)).IsBlackedOut, "DayButton 1 should be blackedout");

            Assert.IsNull(datepicker.SelectedDate, "SelectedDate should be null");

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(new DateTime(2008, 09, 29)));
            Assert.IsNull(datepicker.SelectedDate, "SelectedDate should be null");

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(new DateTime(2008, 09, 26)));
            Assert.IsNull(datepicker.SelectedDate, "SelectedDate should be null");

            InputHelper.Click(tcalendar.CalendarDayButtonByDate(new DateTime(2008, 09, 15)));
            Assert.IsNotNull(datepicker.SelectedDate, "SelectedDate should be not null");

            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");
            Assert.IsFalse((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be false");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker DisplayDate Test
        /// Loads this xaml
        /// <wpf:DatePicker DisplayDate="02/01/2008"/>
        /// </summary>
        public TestResult DatePickerDisplayDate()
        {
            FrameworkElement content = LoadXaml("datepickerdisplaydate.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepicker = FindDatePicker("datepicker");
            datepicker.IsDropDownOpen = true;
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(datepicker, "DatePicker is null");
            Assert.AreEqual(datepicker.DisplayDate, new DateTime(2008, 2, 1));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker DisplayDate Test
        /// Loads this xaml
        /// <wpf:DatePicker Name="datepicker">
        ///   <wpf:DatePicker.DisplayDate>
        ///     <sys:DateTime>08/28/2002</sys:DateTime>
        ///   </wpf:DatePicker.DisplayDate>
        /// </wpf:DatePicker>
        /// </summary>
        public TestResult DatePickerDisplayDate2()
        {
            FrameworkElement content = LoadXaml("datepickerdisplaydate2.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepicker = FindDatePicker("datepicker");
            datepicker.IsDropDownOpen = true;
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(datepicker, "DatePicker is null");
            Assert.AreEqual(datepicker.DisplayDate, new DateTime(2002, 08, 28));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker DisplayDate Start/End Test
        /// Loads this xaml
        /// <wpf:DatePicker Name="datepicker" DisplayDateStart="2008/1/1" DisplayDateEnd="2008/12/31" />
        /// </summary>
        public TestResult DatePickerDisplayDateStartEnd()
        {
            FrameworkElement content = LoadXaml("datepickerdisplaydatestartend.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepicker = FindDatePicker("datepicker");
            datepicker.IsDropDownOpen = true;
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(datepicker, "DatePicker is null");
            Assert.IsTrue(datepicker.DisplayDateStart.HasValue, "DatePicker DisplayDateStart has no value");
            Assert.IsTrue(datepicker.DisplayDateEnd.HasValue, "DatePicker DisplayDateEnd has no value");

            Assert.AreEqual(datepicker.DisplayDateStart.Value, new DateTime(2008, 1, 1));
            Assert.AreEqual(datepicker.DisplayDateEnd.Value, new DateTime(2008, 12, 31));
            
            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker DisplayDate Start/End Test
        /// Loads this xaml
        /// <wpf:DatePicker Name="datepicker" FirstDayOfWeek="Wednesday" />
        /// </summary>
        public TestResult DatePickerFirstDayOfWeek()
        {
            FrameworkElement content = LoadXaml("datepickerfirstdayofweek.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepicker = FindDatePicker("datepicker");

            Assert.IsNotNull(datepicker, "DatePicker is null");
            Assert.AreEqual(datepicker.FirstDayOfWeek, DayOfWeek.Wednesday);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker IsTodayHighlighted Test
        /// Loads this xaml
        /// <wpf:DatePicker Name="false" IsTodayHighlighted="false" />
        /// <wpf:DatePicker Name="true" IsTodayHighlighted="true" />
        /// </summary>
        public TestResult DatePickerIsTodayHighlighted()
        {
            FrameworkElement content = LoadXaml("datepickeristodayhighlighted.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker truedatepicker = FindDatePicker("true");
            DatePicker falsedatepicker = FindDatePicker("false");
            DispatcherHelper.DoEvents();

            truedatepicker.IsDropDownOpen = true;
            falsedatepicker.IsDropDownOpen = true;

            Assert.IsNotNull(truedatepicker, "DatePicker is null");
            Assert.IsNotNull(falsedatepicker, "DatePicker is null");
            Assert.IsTrue(truedatepicker.IsTodayHighlighted, "IsTodayHighlighted should be true");
            Assert.IsFalse(falsedatepicker.IsTodayHighlighted, "IsTodayHighlighted should be false");

            ResetTest();
            return TestResult.Pass;
        }
        
        /// <summary>
        /// DatePicker SelectedDate Test
        /// Load this XAML
        /// <wpf:DatePicker x:Name="datepicker" SelectedDate="01/01/2008" />
        /// </summary>
        public TestResult DatePickerSelectedDate()
        {
            FrameworkElement content = LoadXaml("datepickerselecteddate.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepicker = FindDatePicker("datepicker");

            Assert.IsNotNull(datepicker, "DatePicker is null");
            Assert.IsTrue(datepicker.SelectedDate.HasValue, "SelectedDate should have value.");
            Assert.AreEqual(datepicker.SelectedDate.Value, new DateTime(2008, 1, 1));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker SelectedDate Test
        /// Load this XAML
        /// <wpf:DatePicker>
        ///   <wpf:DatePicker.SelectedDate>
        ///     <sys:DateTime>08/28/2002</sys:DateTime>   
        ///   </wpf:DatePicker.SelectedDate>
        /// </wpf:DatePicker>
        /// </summary>
        public TestResult DatePickerSelectedDate2()
        {
            FrameworkElement content = LoadXaml("datepickerselecteddate2.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepicker = FindDatePicker("datepicker");

            Assert.IsNotNull(datepicker, "DatePicker is null");
            Assert.IsTrue(datepicker.SelectedDate.HasValue, "SelectedDate should have value.");
            Assert.AreEqual(datepicker.SelectedDate.Value, new DateTime(2002, 08, 28));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker IsDropDownOpen Test
        /// Load this XAML
        /// <wpf:DatePicker Name="closed" IsDropDownOpen="false" />
        /// <wpf:DatePicker Name="opened" IsDropDownOpen="true" />
        /// </summary>
        public TestResult DatePickerIsDropDownOpen()
        {
            FrameworkElement content = LoadXaml("datepickerisdropdownopen.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepickeropened = FindDatePicker("opened");
            DatePicker datepickerclosed = FindDatePicker("closed");

            Assert.IsNotNull(datepickeropened, "DatePicker [opened] is null");
            Assert.IsNotNull(datepickerclosed, "DatePicker [closed] is null");

            TemplatedDatePicker topened = new TemplatedDatePicker(datepickeropened);
            TemplatedDatePicker tclosed = new TemplatedDatePicker(datepickerclosed);

            Assert.IsTrue(topened.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(topened.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue((bool)datepickeropened.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be true");
            Assert.IsFalse(tclosed.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tclosed.Calendar.IsVisible, "Calendar.IsVisible should be false");
            Assert.IsFalse((bool)datepickerclosed.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be false");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker SelectedDateFormat Test
        /// Load this XAML
        /// <wpf:DatePicker Name="datepicker" SelectedDateFormat="long" SelectedDate="1979/09/02"/>
        /// </summary>
        public TestResult DatePickerSelectedDateFormat()
        {
            FrameworkElement content = LoadXaml("datepickerselectedformat.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepicker = FindDatePicker("datepicker");
            Assert.IsNotNull(datepicker, "DatePicker is null");

            DateTime date = new DateTime(1979, 09, 02);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            Assert.AreEqual(tdatepicker.TextBox.Text, date.ToString("D"));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// DatePicker SelectedText Test
        /// Load this XAML
        ///  <wpf:DatePicker Name="datepicker" Text="01/01/2009"/>
        /// </summary>
        public TestResult DatePickerText()
        {
            FrameworkElement content = LoadXaml("datepickertext.xaml");
            content.Loaded += this.DatePicker_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            DatePicker datepicker = FindDatePicker("datepicker");
            Assert.IsNotNull(datepicker, "DatePicker is null");

            DateTime date = new DateTime(2009, 01, 01);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            Assert.AreEqual(tdatepicker.TextBox.Text, date.ToString("d"));
            Assert.IsTrue(datepicker.SelectedDate.HasValue, "Selected date has no value");
            Assert.AreEqual(datepicker.SelectedDate.Value, date);

            ResetTest();
            return TestResult.Pass;
        }
    }
}
