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
    //[Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "TextTest", Keywords = "Localization_Suite")]
    public class TextTest : DatePickerTest
    {
        public TextTest()
            : base()
        {
            this.RunSteps += DatePickerText;
            this.RunSteps += InvalidTextBeforeLoad;
            this.RunSteps += ValidTextBeforeLoad;
            this.RunSteps += TextNull;
            this.RunSteps += TextParse;
        }


        /// <summary>
        /// Ensure Text property reflects the changes in the TextBox part of the DatePicker.
        /// </summary>
        public TestResult DatePickerText()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += DatePicker_Loaded;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            DateTimeFormatInfo dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            datepicker.Text = "test";
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);

            Assert.IsFalse(tdatepicker.TextBox.Text == "test", "DatePickerTextBox.Text should not be equal to 'test'");
            Assert.AreEqual(tdatepicker.Watermark.Content.ToString(), string.Format(CultureInfo.CurrentCulture, ResourceHelper.GetString("DatePicker_WatermarkText"), dateFormatInfo.ShortDatePattern.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty((string)datepicker.GetValue(DatePicker.TextProperty)), "DatePicker.TextProperty should be null or empty");

            datepicker.Text = DateTime.Today.ToString();
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datepicker.SelectedDate.Value, DateTime.Today);
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, DateTime.Today.ToString(dateFormatInfo.ShortDatePattern, dateFormatInfo)), datepicker.Text);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure TextBox text property reflects the changes in the Text property of the DatePicker.
        /// </summary>
        public TestResult InvalidTextBeforeLoad()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Text = "testbeforeload";
            datepicker.Loaded += DatePicker_Loaded;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            Assert.IsTrue(string.IsNullOrEmpty((string)datepicker.GetValue(DatePicker.TextProperty)), "DatePicker.TextProperty should be null or empty");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure TextBox text property reflects the changes in the Text property of the DatePicker.
        /// </summary>
        public TestResult ValidTextBeforeLoad()
        {
            DatePicker datepicker = new DatePicker();

            DateTime date = new DateTime(2005, 5, 5);
            datepicker.Text = date.ToString();

            datepicker.Loaded += DatePicker_Loaded;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            DateTimeFormatInfo dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            Assert.AreEqual(datepicker.Text, string.Format(CultureInfo.CurrentCulture, date.ToString(dateFormatInfo.ShortDatePattern, dateFormatInfo)));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure Text property reflects the changes in the TextBox part of the DatePicker.
        /// </summary>
        public TestResult TextNull()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.SelectedDate = DateTime.Today;
            datepicker.Loaded += DatePicker_Loaded;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");
            Assert.IsFalse(string.IsNullOrEmpty(datepicker.Text), "DatePicker.Text should not be null");

            datepicker.Text = null;
            DispatcherHelper.DoEvents();

            Assert.IsNull(datepicker.SelectedDate, "SelectedDate should be null after setting Text == null");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure Text is parsed in "D", "d" or "G" formats.
        /// </summary>
        public TestResult TextParse()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += DatePicker_Loaded;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            DateTimeFormatInfo dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

            DateTime d = new DateTime(2003, 10, 10);
            datepicker.Text = d.ToString("d");
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datepicker.SelectedDate.Value, new DateTime(2003, 10, 10));

            d = new DateTime(2003, 12, 10);
            datepicker.Text = d.ToString("D");
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datepicker.SelectedDate.Value, new DateTime(2003, 12, 10));

            d = new DateTime(2003, 11, 10);
            datepicker.Text = d.ToString("G");
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datepicker.SelectedDate.Value, new DateTime(2003, 11, 10));

            ResetTest();
            return TestResult.Pass;
        }
    }
}
