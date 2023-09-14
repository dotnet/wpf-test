using System;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "BlackoutDatesTest")]
    public class BlackoutDatesTest : CalendarTest
    {
        public BlackoutDatesTest()
            : base()
        {
            this.RunSteps += SetToMaxValue;
            this.RunSteps += SetToMinValue;
            this.RunSteps += BlackoutDatesSingleDay;
            this.RunSteps += BlackoutDatesRange;
            this.RunSteps += SetBlackoutDatesRange;
            this.RunSteps += SetBlackoutDatesException;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MaxValue.
        /// </summary>
        public TestResult SetToMaxValue()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDate = DateTime.MaxValue;

            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MaxValue));
            Assert.AreEqual(calendar.BlackoutDates[0].End, DateTime.MaxValue);
            Assert.AreEqual(calendar.BlackoutDates[0].Start, DateTime.MaxValue);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MinValue.
        /// </summary>
        public TestResult SetToMinValue()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDate = DateTime.MinValue;

            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue));
            Assert.AreEqual(calendar.BlackoutDates[0].End, DateTime.MinValue);
            Assert.AreEqual(calendar.BlackoutDates[0].Start, DateTime.MinValue);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException when setting SelectedDate to a date in BlackoutDates 
        /// </summary>
        public TestResult BlackoutDatesSingleDay()
        {
            Calendar calendar = new Calendar();
            calendar.BlackoutDates.AddDatesInPast();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentOutOfRangeException), () =>
            {
                calendar.SelectedDate = DateTime.Today.AddDays(-1);
            });

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException when setting SelectedDate to a date in BlackoutDates 
        /// </summary>
        public TestResult BlackoutDatesRange()
        {
            Calendar calendar = new Calendar();
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.Today.AddDays(10)));
            calendar.SelectedDate = DateTime.Today.AddDays(-1);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today.AddDays(-1));
            Assert.AreEqual(calendar.SelectedDate.Value, calendar.SelectedDates[0]);
            calendar.SelectedDate = DateTime.Today.AddDays(11);
            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today.AddDays(11));
            Assert.AreEqual(calendar.SelectedDate.Value, calendar.SelectedDates[0]);

            Assert.IsExceptionThrown(typeof(ArgumentOutOfRangeException), () =>
            {
                calendar.SelectedDate = DateTime.Today.AddDays(5);
            });

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException when adding Date to BlackoutDates when it exists in SelectedDates
        /// </summary>
        public TestResult SetBlackoutDatesRange()
        {
            Calendar calendar = new Calendar();
            calendar.SelectedDate = DateTime.Today.AddDays(5);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentOutOfRangeException), () =>
            {
                calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.Today.AddDays(10)));
            });

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException when adding Date to BlackoutDates when it is DisplayDateStart
        /// </summary>
        public TestResult SetBlackoutDatesException()
        {
            Calendar calendar = new Calendar();
            calendar.SelectedDate = DateTime.Today;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentOutOfRangeException), () =>
            {
                calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)));
            });

            ResetTest();
            return TestResult.Pass;
        }

    }
}
