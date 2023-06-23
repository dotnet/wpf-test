using System;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "DPDisplayDateTest", Keywords = "Localization_Suite")]
    public class DPDisplayDateTest : DatePickerTest
    {
        public DPDisplayDateTest()
            : base()
        {
            this.RunSteps += DisplayDatePropertySetValue;
            this.RunSteps += DisplayDateStartEnd;
            this.RunSteps += DisplayDateRangeEnd;
            this.RunSteps += DatePropertiesSetToMaxValue;
            this.RunSteps += DatePropertiesSetToMinValue;
        }

        /// <summary>
        /// Set the value of DisplayDateProperty.
        /// </summary>
        public TestResult DisplayDatePropertySetValue()
        {
            DatePicker datepicker = new DatePicker();
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            DateTime value = DateTime.Today.AddMonths(3);
            datepicker.DisplayDate = value;

            Assert.AreEqual(datepicker.DisplayDate, value);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Set the value of DisplayDateStart and DisplayDateEnd
        /// </summary>
        public TestResult DisplayDateStartEnd()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.DisplayDateStart = new DateTime(2005, 12, 30);
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();


            DateTime value = new DateTime(2005, 12, 15);
            datepicker.DisplayDate = value;

            Assert.AreEqual(datepicker.DisplayDateStart.Value, datepicker.DisplayDate);

            value = new DateTime(2005, 12, 30);
            datepicker.DisplayDate = value;

            Assert.AreEqual(datepicker.DisplayDate, value);

            value = DateTime.MaxValue;
            datepicker.DisplayDate = value;

            Assert.AreEqual(datepicker.DisplayDate, value);

            datepicker.DisplayDateEnd = new DateTime(2010, 12, 30);
            Assert.AreEqual(datepicker.DisplayDateEnd.Value, datepicker.DisplayDate);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDate/Start/End are equal after updating DisplayDateStart
        /// </summary>
        public TestResult DisplayDateRangeEnd()
        {
            DatePicker datepicker = new DatePicker();
            DateTime value = new DateTime(2000, 1, 30);
            datepicker.DisplayDate = value;
            datepicker.DisplayDateEnd = value;
            datepicker.DisplayDateStart = value;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datepicker.DisplayDateStart.Value, value);
            Assert.AreEqual(datepicker.DisplayDateEnd.Value, value);

            value = value.AddMonths(2);
            datepicker.DisplayDateStart = value;

            Assert.AreEqual(datepicker.DisplayDateStart.Value, value);
            Assert.AreEqual(datepicker.DisplayDateEnd.Value, value);
            Assert.AreEqual(datepicker.DisplayDate, value);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MaxValue.
        /// </summary>
        public TestResult DatePropertiesSetToMaxValue()
        {
            DatePicker datepicker = new DatePicker();
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            datepicker.DisplayDate = DateTime.MaxValue;
            Assert.AreEqual(datepicker.DisplayDate, DateTime.MaxValue);

            datepicker.SelectedDate = DateTime.MaxValue;
            Assert.AreEqual(datepicker.SelectedDate.Value, DateTime.MaxValue);

            datepicker.DisplayDateStart = DateTime.MaxValue;
            Assert.AreEqual(datepicker.DisplayDateStart.Value, DateTime.MaxValue);

            datepicker.DisplayDateEnd = DateTime.MaxValue;
            Assert.AreEqual(datepicker.DisplayDateEnd.Value, DateTime.MaxValue);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MinValue.
        /// </summary>
        public TestResult DatePropertiesSetToMinValue()
        {
            DatePicker datepicker = new DatePicker();
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            datepicker.DisplayDate = DateTime.MinValue;
            Assert.AreEqual(datepicker.DisplayDate, DateTime.MinValue);

            datepicker.DisplayDateStart = DateTime.MinValue;
            Assert.AreEqual(datepicker.DisplayDateStart.Value, DateTime.MinValue);

            datepicker.DisplayDateEnd = DateTime.MinValue;
            Assert.AreEqual(datepicker.DisplayDateEnd.Value, DateTime.MinValue);

            datepicker.SelectedDate = null;

            datepicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue));
            Assert.AreEqual(datepicker.BlackoutDates[0].End, DateTime.MinValue);
            Assert.AreEqual(datepicker.BlackoutDates[0].Start, DateTime.MinValue);

            ResetTest();
            return TestResult.Pass;
        }
    }
}
