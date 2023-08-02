using System;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;
using System.Globalization;

namespace Microsoft.Test.Controls
{
    [Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "DatePickerFormatTest")]
    public class DatePickerFormatTest : DatePickerTest
    {
        public DatePickerFormatTest()
            : base()
        {
            this.RunSteps += DatePickerFormatException;
            this.RunSteps += DatePickerFormatLong;
            this.RunSteps += DatePickerFormatShort;
            this.RunSteps += DatePickerDateFormats;
        }

        /// <summary>
        /// Throw ArgumentException when casting DatePickerFormat to anything greater than 1
        /// </summary>
        public TestResult DatePickerFormatException()
        {
            DatePicker datepicker = new DatePicker();
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentException), () =>
            {
                datepicker.SelectedDateFormat = (DatePickerFormat)2;
            });

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify correct string formats for SelectedDateFormat.Long
        /// </summary>
        public TestResult DatePickerFormatLong() 
        {
            DateTime date = new DateTime(2008, 1, 10);

            DatePicker datepicker = new DatePicker();
            datepicker.SelectedDateFormat = DatePickerFormat.Long;
            datepicker.SelectedDate = date;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);

            Assert.AreEqual(datepicker.Text, date.ToString("D"));
            Assert.AreEqual(tdatepicker.TextBox.Text, date.ToString("D"));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify correct string formats for SelectedDateFormat.Short
        /// </summary>
        public TestResult DatePickerFormatShort() 
        {
            DateTime date = new DateTime(2008, 1, 10);

            DatePicker datepicker = new DatePicker();
            datepicker.SelectedDateFormat = DatePickerFormat.Short;
            datepicker.SelectedDate = date;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);

            Assert.AreEqual(datepicker.Text, date.ToString("d"));
            Assert.AreEqual(tdatepicker.TextBox.Text, date.ToString("d"));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify correct strings with DateTimeFormats
        /// </summary>
        public TestResult DatePickerDateFormats()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += DatePicker_Loaded;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            DateTimeFormatInfo dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            DateTime d = new DateTime(2008, 1, 10);

            datepicker.Text = string.Format(CultureInfo.CurrentCulture, d.ToString(dateFormatInfo.ShortDatePattern, dateFormatInfo));

            datepicker.SelectedDateFormat = DatePickerFormat.Long;
            DispatcherHelper.DoEvents();
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, d.ToString(dateFormatInfo.LongDatePattern, dateFormatInfo)), datepicker.Text);

            datepicker.SelectedDateFormat = DatePickerFormat.Short;
            DispatcherHelper.DoEvents();
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, d.ToString(dateFormatInfo.ShortDatePattern, dateFormatInfo)), datepicker.Text);

            ResetTest();
            return TestResult.Pass;
        }
    }
}
