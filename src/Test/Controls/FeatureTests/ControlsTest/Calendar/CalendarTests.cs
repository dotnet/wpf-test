using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Glob = System.Globalization;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "CalendarTests", Keywords = "Localization_Suite")]
    public class CalendarTests : CalendarTest
    {
        //public string CalendarXaml = "<toolkit:Calendar SelectedDate=\"2008/04/30\" DisplayDateStart=\"2020/04/30\" DisplayDateEnd=\"2010/04/30\" DisplayDate=\"2000/02/02\" xmlns:toolkit=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation" />";
        public string CalendarXaml = "<Calendar SelectedDate=\"2008/04/30\" DisplayDateStart=\"2020/04/30\" DisplayDateEnd=\"2010/04/30\" DisplayDate=\"2000/02/02\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" />";

        public CalendarTests()
            : base()
        { 
            this.RunSteps += Create;
            this.RunSteps += CreateInXaml;
            this.RunSteps += CheckDefaultValues;
        }

        /// <summary>
        /// Create a Calendar control.
        /// </summary>
        public TestResult Create()
        {
            Calendar calendar = new Calendar();
            Assert.IsNotNull(calendar, "Calendar should not be null.");
            return TestResult.Pass;
        }

        /// <summary>
        /// Create a Calendar in XAML markup.
        /// </summary>
        public TestResult CreateInXaml()
        {
            object _calendar = XamlReader.Parse(CalendarXaml);
            Assert.IsInstanceOfType(_calendar, typeof(Calendar));
            
            Calendar calendar = _calendar as Calendar;
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar from XAML never fired loaded event.");
            Assert.AreEqual(calendar.SelectedDate.Value, new DateTime(2008, 4, 30));
            Assert.AreEqual(calendar.DisplayDateStart.Value, new DateTime(2008, 4, 30));
            Assert.AreEqual(calendar.DisplayDate, new DateTime(2008, 4, 30));
            Assert.AreEqual(calendar.DisplayDateEnd.Value, new DateTime(2010, 4, 30));

            ResetTest();

            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure all default values are correct.
        /// </summary>
        public TestResult CheckDefaultValues()
        {
            Calendar calendar = new Calendar();

            Assert.AreEqual(calendar.DisplayDate, DateTime.Today);
            Assert.IsNull(calendar.DisplayDateStart, "Default DisplayDateStart should be null.");
            Assert.IsNull(calendar.DisplayDateEnd, "Default DisplayDateEnd should be null.");
            Assert.AreEqual(calendar.FirstDayOfWeek, Glob.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
            Assert.IsTrue(calendar.IsTodayHighlighted, "Today is not highlighted.");
            Assert.IsNull(calendar.SelectedDate, "Default SelectedDate should be null.");
            Assert.IsTrue(calendar.SelectedDates.Count == 0, "SelectedDates.Count should be == 0.");
            Assert.IsTrue(calendar.IsEnabled, "Calendar control in not enabled.");
            
            Assert.IsTrue(calendar.BlackoutDates.Count == 0, "BlackoutDates.Count should be == 0");
            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Month, "Calendar.DisplayMode should be == CalendarMode.Month");
            Assert.IsTrue(calendar.SelectionMode == CalendarSelectionMode.SingleDate, "Calendar.SelectionMode should be == CalendarSelectionMode.SingleDate");
            
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();
            
            Assert.IsTrue(isLoaded, "Calendar never fired loaded event");

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            Assert.IsTrue(tcalendar.PreviousButton.IsEnabled, "PreviousButton is not enabled.");
            Assert.IsTrue(tcalendar.NextButton.IsEnabled, "NextButton is not enabled.");
            Assert.IsTrue(tcalendar.HeaderButton.IsEnabled, "HeaderButton is not enabled.");
            Assert.IsTrue(tcalendar.HeaderButton.Content.ToString() == calendar.DisplayDate.ToString("Y", Glob.DateTimeFormatInfo.CurrentInfo), "HeaderButton.Content is not equal to DisplayDate.");
            
            ResetTest();

            return TestResult.Pass;
        }
    }
}
