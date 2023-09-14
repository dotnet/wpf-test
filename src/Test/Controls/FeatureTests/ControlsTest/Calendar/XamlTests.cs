using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "XamlTests")]
    public class CalendarXamlTest : CalendarTest
    {
        public CalendarXamlTest()
            : base()
        {
            this.RunSteps += CalendarDefault;
            this.RunSteps += CalendarBlackoutDates;
            this.RunSteps += CalendarDisplayDate;
            this.RunSteps += CalendarDisplayDates;
            this.RunSteps += CalendarDisplayDate2;
            this.RunSteps += CalendarDisplayDateStartEnd;
            this.RunSteps += CalendarDisplayMode;
            this.RunSteps += CalendarFirstDayOfWeek;
            this.RunSteps += CalendarIsTodayHighlighted;
            this.RunSteps += CalendarSelectedDate;
            this.RunSteps += CalendarSelectedDate2;
            this.RunSteps += CalendarSelectedDates;
            this.RunSteps += CalendarSelectionModes;
        }

        /// <summary>
        /// Default Calendar
        /// Loads this xaml
        /// <wpf:Calendar x:Name="calendar" />
        /// </summary>
        public TestResult CalendarDefault()
        {
            FrameworkElement content = LoadXaml("calendar.xaml");
            content.Loaded += this.Calendar_Loaded;
            
            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar BlackoutDates Test
        /// Loads this xaml
        /// <wpf:Calendar Name="calendar" DisplayDate="4/15/2008">
        ///   <wpf:Calendar.BlackoutDates>
        ///     <wpf:CalendarDateRange Start="4/1/2008" End="4/5/2008">
        ///     <wpf:CalendarDateRange Start="4/27/2008" End="4/30/2008">
        ///   <Calendar.BlackoutDates/>	
        /// </wpf:Calendar>
        /// </summary>
        public TestResult CalendarBlackoutDates()
        {
            FrameworkElement content = LoadXaml("calendarblackoutdates.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar calendar = FindCalendar("calendar");

            Assert.IsNotNull(calendar, "Calendar is null");

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            Assert.AreEqual(calendar.BlackoutDates.Count, 2);
            Assert.IsTrue(tcalendar.CalendarDayButtonByDay(1).IsBlackedOut, "DayButton 1 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDay(2).IsBlackedOut, "DayButton 2 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDay(3).IsBlackedOut, "DayButton 3 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDay(4).IsBlackedOut, "DayButton 4 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDay(5).IsBlackedOut, "DayButton 5 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDay(27).IsBlackedOut, "DayButton 27 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDay(28).IsBlackedOut, "DayButton 28 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDay(29).IsBlackedOut, "DayButton 29 should be blackedout");
            Assert.IsTrue(tcalendar.CalendarDayButtonByDay(30).IsBlackedOut, "DayButton 30 should be blackedout");
            
            Assert.IsNull(calendar.SelectedDate, "SelectedDate should be null");

            InputHelper.Click(tcalendar.CalendarDayButtonByDay(3));
            Assert.IsNull(calendar.SelectedDate, "SelectedDate should be null");
            
            InputHelper.Click(tcalendar.CalendarDayButtonByDay(28));
            Assert.IsNull(calendar.SelectedDate, "SelectedDate should be null");
            
            InputHelper.Click(tcalendar.CalendarDayButtonByDay(15));
            Assert.IsNotNull(calendar.SelectedDate, "SelectedDate should be not null");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar DisplayDate Test
        /// Load this XAML
        /// <wpf:Calendar x:Name="calendar" DisplayDate="2008/1/1" />
        /// </summary>
        public TestResult CalendarDisplayDate()
        {
            FrameworkElement content = LoadXaml("calendardisplaydate.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar calendar = FindCalendar("calendar");

            Assert.IsNotNull(calendar, "Calendar is null");
            Assert.AreEqual(calendar.DisplayDate, new DateTime(2008, 1, 1));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar DisplayDate/Start/End Test
        /// Load this XAML
        /// <wpf:Calendar Name="calendar" DisplayDate="6/13/2008" DisplayDateStart="2008/1/1" DisplayDateEnd="2008/12/31" />
        /// </summary>
        public TestResult CalendarDisplayDates()
        {
            FrameworkElement content = LoadXaml("calendardisplaydates.xaml");
            content.Loaded += this.Calendar_Loaded;
            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar calendar = FindCalendar("calendar");

            Assert.IsNotNull(calendar, "Calendar is null");
            Assert.AreEqual(calendar.DisplayDate, new DateTime(2008, 6, 13));
            Assert.AreEqual(calendar.DisplayDateStart, new DateTime(2008, 1, 1));
            Assert.AreEqual(calendar.DisplayDateEnd, new DateTime(2008, 12, 31));

            DateTime startdate = calendar.DisplayDate;

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(-1).Month);

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(-2).Month);

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(-3).Month);

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(-4).Month);

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(-5).Month);

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(-5).Month);

            calendar.DisplayDate = startdate;
            DispatcherHelper.DoEvents();

            InputHelper.Click(tcalendar.NextButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(1).Month);

            InputHelper.Click(tcalendar.NextButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(2).Month);

            InputHelper.Click(tcalendar.NextButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(3).Month);

            InputHelper.Click(tcalendar.NextButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(4).Month);

            InputHelper.Click(tcalendar.NextButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(5).Month);

            InputHelper.Click(tcalendar.NextButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(6).Month);

            InputHelper.Click(tcalendar.NextButton);
            Assert.AreEqual(calendar.DisplayDate.Month, startdate.AddMonths(6).Month);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar DisplayDate Test
        /// Load this XAML
        /// <wpf:Calendar>
        ///   <wpf:Calendar.DisplayDate>
        ///     <sys:DateTime>08/28/2002</sys:DateTime>
        ///   </wpf:Calendar.DisplayDate>
        /// </wpf:Calendar>
        /// </summary>
        public TestResult CalendarDisplayDate2()
        {
            FrameworkElement content = LoadXaml("calendardisplaydate2.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar calendar = FindCalendar("calendar");

            Assert.IsNotNull(calendar, "Calendar is null");
            Assert.AreEqual(calendar.DisplayDate, new DateTime(2002, 08, 28));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar DisplayDate Start/End Test
        /// Load this XAML
        /// <wpf:Calendar Name="calendar" DisplayDateStart="2008/1/1" DisplayDateEnd="2008/12/31" />
        /// </summary>
        public TestResult CalendarDisplayDateStartEnd()
        {
            FrameworkElement content = LoadXaml("calendardisplaydatestartend.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar calendar = FindCalendar("calendar");
            bool displaychanged = false;
            calendar.DisplayDateChanged += (sender, e) =>
            {
                displaychanged = true;
            };

            Assert.IsNotNull(calendar, "Calendar is null");

            DateTime start = new DateTime(2008, 1, 1);
            DateTime end = new DateTime(2008, 12, 31);

            Assert.AreEqual(calendar.DisplayDateStart, start);
            Assert.AreEqual(calendar.DisplayDateEnd, end);

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            
            calendar.DisplayDate = start;
            displaychanged = false;
            DispatcherHelper.DoEvents();

            InputHelper.Click(tcalendar.PreviousButton);
            DispatcherHelper.DoEvents();

            Assert.IsFalse(displaychanged, "DisplayDateChanged should not have fired whene trying to Navigate beyond DisplayDateStart");

            calendar.DisplayDate = end;
            displaychanged = false;
            DispatcherHelper.DoEvents();

            InputHelper.Click(tcalendar.NextButton);
            DispatcherHelper.DoEvents();

            Assert.IsFalse(displaychanged, "DisplayDateChanged should not have fired whene trying to Navigate beyond DisplayDateEnd");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar DisplayMode Test
        /// Load this XAML
        /// <wpf:Calendar Name="decade" DisplayMode="Decade" />
        /// <wpf:Calendar Name="month" DisplayMode="Month" />
        /// <wpf:Calendar Name="year" DisplayMode="Year" />
        /// </summary>
        public TestResult CalendarDisplayMode()
        {
            FrameworkElement content = LoadXaml("calendardisplaymode.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar decadecalendar = FindCalendar("decade");
            Calendar yearcalendar = FindCalendar("year");
            Calendar monthcalendar = FindCalendar("month");

            DispatcherHelper.DoEvents();

            Assert.IsNotNull(decadecalendar, "Decade Mode Calendar is null");
            Assert.IsNotNull(yearcalendar, "Year Mode Calendar is null");
            Assert.IsNotNull(monthcalendar, "Month Mode Calendar is null");

            Assert.AreEqual(decadecalendar.DisplayMode, CalendarMode.Decade);
            Assert.AreEqual(yearcalendar.DisplayMode, CalendarMode.Year);
            Assert.AreEqual(monthcalendar.DisplayMode, CalendarMode.Month);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar FirstDayOfWeek Test
        /// Load this XAML
        /// <wpf:Calendar Name="calendar" FirstDayOfWeek="Thursday"/>
        /// </summary>
        public TestResult CalendarFirstDayOfWeek()
        {
            FrameworkElement content = LoadXaml("calendarfirstdayofweek.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar calendar = FindCalendar("calendar");
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(calendar, "Calendar is null");
            Assert.AreEqual(calendar.FirstDayOfWeek, DayOfWeek.Thursday);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar IsTodayHighlighted Test
        /// Load this XAML
        /// <wpf:Calendar Name="false" IsTodayHighlighted="false" />
        /// <wpf:Calendar Name="true" IsTodayHighlighted="true" />
        /// </summary>
        public TestResult CalendarIsTodayHighlighted()
        {
            FrameworkElement content = LoadXaml("calendaristodayhighlighted.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar truecalendar = FindCalendar("true");
            Calendar falsecalendar = FindCalendar("false");
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(truecalendar, "Calendar is null");
            Assert.IsNotNull(falsecalendar, "Calendar is null");
            Assert.IsTrue(truecalendar.IsTodayHighlighted, "IsTodayHighlighted should be true");
            Assert.IsFalse(falsecalendar.IsTodayHighlighted, "IsTodayHighlighted should be false");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar SelectedDate Test
        /// Load this XAML
        /// <wpf:Calendar x:Name="calendar" SelectedDate="2007/12/31" />
        /// </summary>
        public TestResult CalendarSelectedDate()
        {
            FrameworkElement content = LoadXaml("calendarselecteddate.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar calendar = FindCalendar("calendar");

            Assert.IsNotNull(calendar, "Calendar is null");
            Assert.IsTrue(calendar.SelectedDate.HasValue, "SelectedDate should have value.");
            Assert.AreEqual(calendar.SelectedDate.Value, new DateTime(2007, 12, 31));
            Assert.AreEqual(calendar.SelectedDates.Count, 1);
            Assert.AreEqual(calendar.SelectedDates[0], new DateTime(2007, 12, 31));

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            Assert.AreEqual(calendar.DisplayDate, DateTime.Today);
            Assert.AreEqual(tcalendar.HeaderButton.Content, DateTime.Today.ToString("Y"));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar SelectedDate Test
        /// Load this XAML
        /// <wpf:Calendar>
        ///   <wpf:Calendar.SelectedDate>
        ///     <sys:DateTime>09/01/2008</sys:DateTime>   
        ///   </wpf:Calendar.SelectedDate>
        /// </wpf:Calendar>
        /// </summary>
        public TestResult CalendarSelectedDate2()
        {
            FrameworkElement content = LoadXaml("calendarselecteddate2.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar calendar = FindCalendar("calendar");

            Assert.IsNotNull(calendar, "Calendar is null");
            Assert.IsTrue(calendar.SelectedDate.HasValue, "SelectedDate should have value.");
            Assert.AreEqual(calendar.SelectedDate.Value, new DateTime(2008, 09, 01));
            Assert.AreEqual(calendar.SelectedDates.Count, 1);
            Assert.AreEqual(calendar.SelectedDates[0], new DateTime(2008, 09, 01));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Calendar SelectedDates Test
        /// Load this XAML
        /// <wpf:Calendar>
        ///   <wpf:Calendar.SelectedDates>
        ///     <sys:DateTime>08/24/2002</sys:DateTime>
        ///     <sys:DateTime>08/26/2002</sys:DateTime>
        ///     <sys:DateTime>08/28/2002</sys:DateTime>
        ///   </wpf:Calendar.SelectedDates>
        /// </wpf:Calendar>
        /// </summary>
        public TestResult CalendarSelectedDates()
        {
            FrameworkElement content = LoadXaml("calendarselecteddates.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar calendar = FindCalendar("calendar");

            Assert.IsNotNull(calendar, "Calendar is null");
            Assert.AreEqual(calendar.DisplayDate, new DateTime(2002, 08, 01));
            Assert.IsTrue(calendar.SelectedDate.HasValue, "SelectedDate should have value.");
            Assert.AreEqual(calendar.SelectedDate.Value, new DateTime(2002, 08, 24));
            Assert.AreEqual(calendar.SelectedDates.Count, 3);

            ResetTest();
            return TestResult.Pass;
        }
    
        /// <summary>
        /// Calendar SelectionMode Test
        /// Load this XAML
        /// <wpf:Calendar Name="none" SelectionMode="None" />
        /// <wpf:Calendar Name="singledate" SelectionMode="SingleDate" />
        /// <wpf:Calendar Name="singlerange" SelectionMode="SingleRange" />
        /// <wpf:Calendar Name="multirange" SelectionMode="MultipleRange" />
        /// </summary>
        public TestResult CalendarSelectionModes()
        {
            FrameworkElement content = LoadXaml("calendarselectionmode.xaml");
            content.Loaded += this.Calendar_Loaded;

            this.TestUI.Children.Add(content);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(content, "Content is null");
            Assert.IsTrue(isLoaded, "Content was not loaded from xaml file");

            Calendar none = FindCalendar("none");
            Calendar singledate= FindCalendar("singledate");
            Calendar singlerange = FindCalendar("singlerange");
            Calendar multiple= FindCalendar("multirange");

            DispatcherHelper.DoEvents();

            Assert.IsNotNull(none, "None SelectionMode  Calendar is null");
            Assert.IsNotNull(singledate, "SingleDate SelectionMode Calendar is null");
            Assert.IsNotNull(singlerange, "SingleRange SelectionMode  Calendar is null");
            Assert.IsNotNull(multiple, "MultipleRange SelectionMode  Calendar is null");

            Assert.AreEqual(none.SelectionMode, CalendarSelectionMode.None);
            Assert.AreEqual(singledate.SelectionMode, CalendarSelectionMode.SingleDate);
            Assert.AreEqual(singlerange.SelectionMode, CalendarSelectionMode.SingleRange);
            Assert.AreEqual(multiple.SelectionMode, CalendarSelectionMode.MultipleRange);

            ResetTest();
            return TestResult.Pass;
        }
    }
}
