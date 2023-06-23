using System;
using System.Globalization;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    // Disabling for .NET Core 3, Fix and re-enable.
    //[Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "DPSelectedDateTest", Keywords = "Localization_Suite")]
    public class DPSelectedDateTest : DatePickerTest
    {
        public DPSelectedDateTest()
            : base()
        {
            this.RunSteps += ClearDatePickerTextBox;
            this.RunSteps += SelectedDatesChangedEvent;
            this.RunSteps += SelectedDatePropertySetValue;
        }

        /// <summary>
        /// Ensure WaterMarkedTextBox gets cleared when the SelectedDate is set to null.
        /// </summary>
        public TestResult ClearDatePickerTextBox()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.SelectedDate = DateTime.Today;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            DateTimeFormatInfo dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            DateTime date = new DateTime(2003, 10, 10);
            datepicker.SelectedDate = date;
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);

            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, date.ToString(dateFormatInfo.ShortDatePattern, dateFormatInfo)), datepicker.Text);
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, date.ToString(dateFormatInfo.ShortDatePattern, dateFormatInfo)), tdatepicker.TextBox.Text);

            datepicker.SelectedDate = null;
            DispatcherHelper.DoEvents();

            tdatepicker = new TemplatedDatePicker(datepicker);

            Assert.AreEqual(tdatepicker.Watermark.Content, string.Format(CultureInfo.CurrentCulture, ResourceHelper.GetString("DatePicker_WatermarkText"), dateFormatInfo.ShortDatePattern.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(datepicker.Text), "DatePicker.Text should be null or empty");
            Assert.IsTrue(string.IsNullOrEmpty(tdatepicker.TextBox.Text), "DatePickerTextBoxt.Text should be null or empty");

            ResetTest();
            return TestResult.Pass;
        }

        private bool handled = false;

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            handled = true;
        }

        /// <summary>
        /// Ensure DateSelected event is fired.
        /// </summary>
        public TestResult SelectedDatesChangedEvent()
        {
            DatePicker datePicker = new DatePicker();

            datePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(datePicker_SelectedDateChanged);

            this.TestUI.Children.Add(datePicker);
            DispatcherHelper.DoEvents();

            DateTime value = new DateTime(2000, 10, 10);
            datePicker.SelectedDate = value;

            Assert.IsTrue(handled, "Event not handled.");
            Assert.AreEqual(datePicker.ToString(), value.ToString());

            datePicker.SelectedDateChanged -= new EventHandler<SelectionChangedEventArgs>(datePicker_SelectedDateChanged);

            ResetTest();
            return TestResult.Pass;
        }


        /// <summary>
        /// Set the value of SelectedDateProperty.
        /// </summary>
        public TestResult SelectedDatePropertySetValue()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.SelectedDate = DateTime.Today;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datepicker.SelectedDate.Value, DateTime.Today);
            Assert.AreEqual((DateTime)datepicker.GetValue(DatePicker.SelectedDateProperty), DateTime.Today);

            datepicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
            datepicker.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(1), DateTime.MaxValue));
            datepicker.SelectedDate = DateTime.Today;
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datepicker.SelectedDate.Value, DateTime.Today);
            Assert.AreEqual((DateTime)datepicker.GetValue(DatePicker.SelectedDateProperty), DateTime.Today);

            ResetTest();
            return TestResult.Pass;
        }
    }
}
