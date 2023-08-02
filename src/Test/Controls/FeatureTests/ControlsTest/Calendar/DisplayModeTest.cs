using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "DisplayModeTest", Keywords = "Localization_Suite")]
    public class DisplayModeTest : CalendarTest
    {
        public DisplayModeTest()
            : base()
        {
            this.RunSteps += DisplayModeOutOfRangeException;
            this.RunSteps += DisplayModeYearToMonth;
            this.RunSteps += DisplayModeYearToDecade;
            this.RunSteps += DisplayModeDecadeToYear;
            this.RunSteps += DisplayModeDecadeToMonthToYear;
            this.RunSteps += DisplayModeDecadeMinValue;
            this.RunSteps += DisplayModeDecadeMaxValue;
            this.RunSteps += DisplayModeOneWeek;
            this.RunSteps += DisplayModeDisplayDate;
            this.RunSteps += DisplayModeDecadeDisplayDate;
            this.RunSteps += DisplayModeChangedEvent;
            this.RunSteps += DisplayModeChangedEventArgs;
        }

        /// <summary>
        /// Ensure ArgumentException is thrown when casting CalendarMode to invalid value
        /// Possible values: CalendarMode.Month = 0, CalendarMode.Year = 1, CalendarMode.Decade = 2
        /// </summary>
        public TestResult DisplayModeOutOfRangeException()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentException), () =>
            {
                calendar.DisplayMode = (CalendarMode)4;
            });

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure CalendarMode changes from Year to Month when clicking item in  Yearview
        /// </summary>
        public TestResult DisplayModeYearToMonth()
        {
            bool displayModeChanged = false;

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Year;
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);

            calendar.DisplayModeChanged += (sender, e) =>
            {
                displayModeChanged = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar from never fired loaded event.");
            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Year, "DisplayMode should be CalendarMode.Year");

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            Assert.IsTrue(tcalendar.Month.UI.Visibility == Visibility.Hidden, "MonthView should be collapesed when in CalendarMode.Year");
            Assert.IsTrue(tcalendar.Year.UI.Visibility == Visibility.Visible, "YearView should be visible when in CalendarMode.Year");

            InputHelper.Click(tcalendar.Year.Items[0]);

            Assert.IsTrue(displayModeChanged, "DisplayModeChanged never fired");
            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Month, "DisplayMode should have changed to CalendarMode.Month");

            Assert.IsTrue(tcalendar.Month.UI.Visibility == Visibility.Visible, "MonthView should be visible when in CalendarMode.Month");
            Assert.IsTrue(tcalendar.Year.UI.Visibility == Visibility.Hidden, "YearView should be Hidden when in CalendarMode.Month");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure CalendarMode changes from Year to Decade when clicking item in HeaderButton
        /// </summary>
        public TestResult DisplayModeYearToDecade()
        {
            bool displayModeChanged = false;

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Year;
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);

            calendar.DisplayModeChanged += new EventHandler<CalendarModeChangedEventArgs>(delegate
            {
                displayModeChanged = true;
            });

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar from never fired loaded event.");
            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Year, "DisplayMode should be CalendarMode.Year");

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            Assert.IsTrue(tcalendar.Month.UI.Visibility == Visibility.Hidden, "MonthView should be collapesed when in CalendarMode.Year");
            Assert.IsTrue(tcalendar.Year.UI.Visibility == Visibility.Visible, "YearView should be visible when in CalendarMode.Year");

            InputHelper.Click(tcalendar.HeaderButton);

            Assert.IsTrue(displayModeChanged, "DisplayModeChanged never fired");
            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Decade, "DisplayMode should have changed to CalendarMode.Decade");

            Assert.IsTrue(tcalendar.Month.UI.Visibility == Visibility.Hidden, "MonthView should be collapesed when in CalendarMode.Year");
            Assert.IsTrue(tcalendar.Year.UI.Visibility == Visibility.Visible, "YearView should be visible when in CalendarMode.Year");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure CalendarMode changes from Decade to Year when clicking item in Yearview
        /// </summary>
        public TestResult DisplayModeDecadeToYear()
        {
            bool displayModeChanged = false;

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Decade;
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);

            calendar.DisplayModeChanged += new EventHandler<CalendarModeChangedEventArgs>(delegate
            {
                displayModeChanged = true;
            });

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar from never fired loaded event.");
            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Decade, "DisplayMode should be CalendarMode.Decade");

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            Assert.IsTrue(tcalendar.Month.UI.Visibility == Visibility.Hidden, "MonthView should be collapesed when in CalendarMode.Decade");
            Assert.IsTrue(tcalendar.Year.UI.Visibility == Visibility.Visible, "YearView should be visible when in CalendarMode.Decade");

            InputHelper.Click(tcalendar.Year.Items[6]);

            Assert.IsTrue(displayModeChanged, "DisplayModeChanged never fired");
            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Year, "DisplayMode should have changed to CalendarMode.Year");

            Assert.IsTrue(tcalendar.Month.UI.Visibility == Visibility.Hidden, "MonthView should be collapesed when in CalendarMode.Year");
            Assert.IsTrue(tcalendar.Year.UI.Visibility == Visibility.Visible, "YearView should be visible when in CalendarMode.Year");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure CalendarMode changes from Decade to Month then back to Year by manually setting property
        /// </summary>
        public TestResult DisplayModeDecadeToMonthToYear()
        {
            bool displayModeChanged = false;

            Calendar calendar = new Calendar();
            calendar.DisplayDate = new DateTime(2000, 1, 1);
            calendar.DisplayDateStart = new DateTime(2000, 1, 1);
            calendar.DisplayDateEnd = new DateTime(2000, 1, 1);
            calendar.DisplayMode = CalendarMode.Decade;
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);

            calendar.DisplayModeChanged += (sender, e) =>
            {
                displayModeChanged = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar from never fired loaded event.");
            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Decade, "DisplayMode should be CalendarMode.Decade");

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            Assert.IsTrue(tcalendar.Month.UI.Visibility == Visibility.Hidden, "MonthView should be collapesed when in CalendarMode.Decade");
            Assert.IsTrue(tcalendar.Year.UI.Visibility == Visibility.Visible, "YearView should be visible when in CalendarMode.Decade");

            Assert.IsFalse(tcalendar.HeaderButton.IsEnabled, "HeaderButton should be disabled in CalendarMode.Decade");
            Assert.IsFalse(tcalendar.PreviousButton.IsEnabled, "PreviousButton should be disabled in CalendarMode.Decade");
            Assert.IsFalse(tcalendar.NextButton.IsEnabled, "NextButton should be disabled in CalendarMode.Decade");

            calendar.DisplayMode = CalendarMode.Month;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(displayModeChanged, "DisplayModeChanged never fired");

            Assert.IsTrue(tcalendar.HeaderButton.IsEnabled, "HeaderButton should now be enabled in CalendarMode.Month");
            Assert.IsFalse(tcalendar.PreviousButton.IsEnabled, "PreviousButton should still be disabled due to DisplayDate range");
            Assert.IsFalse(tcalendar.NextButton.IsEnabled, "NextButton should still be disabled due to DisplayDate range");

            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Month, "DisplayMode should be CalendarMode.Month");
            Assert.IsTrue(tcalendar.Month.UI.Visibility == Visibility.Visible, "MonthView should be visible when in CalendarMode.Month");
            Assert.IsTrue(tcalendar.Year.UI.Visibility == Visibility.Hidden, "YearView should be Hidden when in CalendarMode.Decade");

            displayModeChanged = false;
            calendar.DisplayMode = CalendarMode.Year;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(displayModeChanged, "DisplayModeChanged never fired");
            Assert.IsTrue(tcalendar.Month.UI.Visibility == Visibility.Hidden, "MonthView should be collapesed when in CalendarMode.Year");
            Assert.IsTrue(tcalendar.Year.UI.Visibility == Visibility.Visible, "YearView should be visible when in CalendarMode.Year");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure next/previous button states in Decade
        /// </summary>
        public TestResult DisplayModeDecadeMinValue()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayDate = DateTime.MinValue;
            calendar.DisplayMode = CalendarMode.Decade;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            Assert.IsFalse(tcalendar.PreviousButton.IsEnabled, "PreviousButton should be disabled.");
            Assert.IsTrue(tcalendar.NextButton.IsEnabled, "NextButton should be enabled.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure next/previous button states in Decade
        /// </summary>
        public TestResult DisplayModeDecadeMaxValue()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayDate = DateTime.MaxValue;
            calendar.DisplayMode = CalendarMode.Decade;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            Assert.IsFalse(tcalendar.NextButton.IsEnabled, "NextButton should be disabled");
            Assert.IsTrue(tcalendar.PreviousButton.IsEnabled, "PreviousButton should be enabled");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure button states in Decade
        /// </summary>
        public TestResult DisplayModeOneWeek()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayDateStart = new DateTime(2000, 2, 2);
            calendar.DisplayDateEnd = new DateTime(2000, 2, 5);
            calendar.DisplayMode = CalendarMode.Year;

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            Assert.IsTrue(tcalendar.Year.Items[1].IsEnabled, "YearView should have second month (Children[1]) enabled");
            calendar.DisplayMode = CalendarMode.Decade;
            DispatcherHelper.DoEvents();
            Assert.IsTrue(tcalendar.Year.Items[1].IsEnabled, "YearView should have second year (Children[1]) enabled");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify correct header button content
        /// </summary>
        public TestResult DisplayModeDisplayDate()
        {
            Calendar calendar = new Calendar();
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();
            Assert.IsTrue(isLoaded, "Calendar from never fired loaded event.");

            calendar.DisplayMode = CalendarMode.Year;
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            Assert.AreEqual(tcalendar.HeaderButton.Content.ToString(), calendar.DisplayDate.Year.ToString());

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayMode updates to Decade mode after calendar is loaded
        /// </summary>
        public TestResult DisplayModeDecadeDisplayDate()
        {
            Calendar calendar = new Calendar();
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();
            Assert.IsTrue(isLoaded, "Calendar from never fired loaded event.");

            calendar.DisplayMode = CalendarMode.Decade;
            DispatcherHelper.DoEvents();
            
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);
         
            ResetTest();

            return TestResult.Pass;
        }

        /// <summary>
        /// Update CalendarMode with api calls
        /// </summary>
        public TestResult DisplayModeChangedEvent()
        {
            bool displayModeChanged = false;

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Year;
            calendar.DisplayModeChanged += (sender, e) =>
            {
                displayModeChanged = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayMode = CalendarMode.Month;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(displayModeChanged, "DisplayModeChanged never fired");
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Month);

            displayModeChanged = false;
            calendar.DisplayMode = CalendarMode.Decade;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(displayModeChanged, "DisplayModeChanged never fired");
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);

            displayModeChanged = false;
            calendar.DisplayMode = CalendarMode.Year;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(displayModeChanged, "DisplayModeChanged never fired");
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify DisplayModeChangedEventArgs NewMode and OldMode
        /// </summary>
        public TestResult DisplayModeChangedEventArgs()
        {
            CalendarMode oldmode = CalendarMode.Year;
            CalendarMode newmode = CalendarMode.Year;

            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Year;
            calendar.DisplayModeChanged += (sender, e) =>
            {
                oldmode = e.OldMode;
                newmode = e.NewMode;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayMode = CalendarMode.Month;
            DispatcherHelper.DoEvents();

            Assert.AreEqual(oldmode, CalendarMode.Year);
            Assert.AreEqual(newmode, CalendarMode.Month);
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Month);

            calendar.DisplayMode = CalendarMode.Decade;
            DispatcherHelper.DoEvents();

            Assert.AreEqual(oldmode, CalendarMode.Month);
            Assert.AreEqual(newmode, CalendarMode.Decade);
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);

            calendar.DisplayMode = CalendarMode.Year;
            DispatcherHelper.DoEvents();

            Assert.AreEqual(oldmode, CalendarMode.Decade);
            Assert.AreEqual(newmode, CalendarMode.Year);
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);

            ResetTest();
            return TestResult.Pass;
        }
    }
}
