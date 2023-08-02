using System;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "DPBlackoutDatesTest")]
    public class DPBlackoutDatesTest : DatePickerTest
    {
        public DPBlackoutDatesTest()
            : base()
        {
            this.RunSteps += BlackoutDatesSingleDay;
            this.RunSteps += SetBlackoutDatesRange;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException when setting SelectedDate to a date in BlackoutDates 
        /// </summary>
        public TestResult BlackoutDatesSingleDay()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.BlackoutDates.AddDatesInPast();
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentOutOfRangeException), () =>
            {
                datepicker.SelectedDate = DateTime.Today.AddDays(-1);
            });

            ResetTest();
            return TestResult.Pass;
        }


        /// <summary>
        /// Throw ArgumentOutOfRangeException when adding Date to BlackoutDates when it exists in SelectedDates
        /// </summary>
        public TestResult SetBlackoutDatesRange()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.SelectedDate = DateTime.Today.AddDays(5);
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentOutOfRangeException), () =>
            {
                datepicker.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.Today.AddDays(10)));
            });

            ResetTest();
            return TestResult.Pass;
        }

    }
}
