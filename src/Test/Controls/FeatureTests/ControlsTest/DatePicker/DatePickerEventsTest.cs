using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    // Disabling for .NET Core 3, Fix and re-enable.
    //[Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "DatePickerEventsTest")]
    public class DatePickerEventsTest : DatePickerTest
    {
        public DatePickerEventsTest()
            : base()
        {
            this.RunSteps += DropDownOpenClosedEvent_API;
            this.RunSteps += DateSelectedEvent;
            this.RunSteps += TextParseErrorEvent_1;
            this.RunSteps += TextParseErrorEvent_2;
            this.RunSteps += TextParseErrorEvent_3;
            this.RunSteps += DateValidationErrorEvent_1;
            this.RunSteps += DateValidationErrorEvent_2;
        }

        /// <summary>
        /// Ensure DropDownOpened/Closed events are fired.
        /// </summary>
        public TestResult DropDownOpenClosedEvent_API()
        {
            bool open_handled = false;
            bool close_handled = false;

            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += DatePicker_Loaded;

            datepicker.CalendarOpened += (sender, e) =>
            {
                open_handled = true;
            };

            datepicker.CalendarClosed += (sender, e) =>
            {
                close_handled = true;
            };

            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            datepicker.IsDropDownOpen = true;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(open_handled, "DropDownOpened event not handled.");

            datepicker.IsDropDownOpen = false;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(close_handled, "DropDownClosed event not handled.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateSelected event is fired.
        /// </summary>
        public TestResult DateSelectedEvent()
        {
            bool handled = false;

            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += DatePicker_Loaded;

            datepicker.SelectedDateChanged += (sender, e) =>
            {
                handled = true;

                DatePicker dp = sender as DatePicker;
                Assert.IsTrue(e.AddedItems.Count == 1, "SelectionChangedEventArgs.AddedItems count should be equal to 1");
                Assert.AreEqual((DateTime)e.AddedItems[0], new DateTime(2000, 10, 10));
                Assert.IsTrue(e.RemovedItems.Count == 0, "SelectionChangedEventArgs.RemovedItems count should be equalt to 0.");
            };

            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            DateTime value = new DateTime(2000, 10, 10);
            datepicker.SelectedDate = value;

            DispatcherHelper.DoEvents();

            Assert.IsTrue(handled, "SelectedDate");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure TextParseError event is fired.
        /// </summary>
        public TestResult TextParseErrorEvent_1()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += new RoutedEventHandler(DatePicker_Loaded);
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();
            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            bool handled = false;

            datepicker.DateValidationError += (sender, e) =>
            {
                handled = true;
            };

            datepicker.Text = "errortest";

            DispatcherHelper.DoEvents();

            Assert.IsTrue(handled, "DateValidationError event not handled");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure TextParseError event is fired.
        /// </summary>
        public TestResult TextParseErrorEvent_2()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += new RoutedEventHandler(DatePicker_Loaded);
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();
            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            bool handled = false;

            datepicker.DateValidationError += (sender, e) =>
            {
                handled = true;
            };

            datepicker.Text = "errortest";

            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            DateTimeFormatInfo dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

            Assert.IsTrue(handled, "DateValidationError event not handled");
            Assert.AreEqual(tdatepicker.Watermark.Content.ToString(), string.Format(CultureInfo.CurrentCulture, ResourceHelper.GetString("DatePicker_WatermarkText"), dateFormatInfo.ShortDatePattern.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(datepicker.Text), "DatePicker.Text should be null or empty");
            Assert.IsTrue(string.IsNullOrEmpty(tdatepicker.TextBox.Text), "DatePickerTextBox.Text should be null or empty");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure TextParseError event is fired.
        /// </summary>
        public TestResult TextParseErrorEvent_3()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += new RoutedEventHandler(DatePicker_Loaded);
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();
            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            bool handled = false;

            datepicker.SelectedDate = new DateTime(2003, 10, 10);
            datepicker.DateValidationError += (sender, e) =>
            {
                handled = true;
            };

            datepicker.Text = "errortest"; 
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            DateTimeFormatInfo dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            DateTime d = new DateTime(2003, 10, 10);

            Assert.IsTrue(handled, "DateValidationError event not handled");
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, d.ToString(dateFormatInfo.ShortDatePattern, dateFormatInfo)), datepicker.Text);
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, d.ToString(dateFormatInfo.ShortDatePattern, dateFormatInfo)), tdatepicker.TextBox.Text);
       
            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateValidationError event is fired.
        /// </summary>
        public TestResult DateValidationErrorEvent_1()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += new RoutedEventHandler(DatePicker_Loaded);

            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();
            
            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            bool handled = false;

            datepicker.BlackoutDates.Add(new CalendarDateRange(new DateTime(2000, 2, 2)));
            datepicker.DateValidationError += (sender, e) =>
            {
                handled = true;
                e.ThrowException = true;
            };

            Assert.IsExceptionThrown(typeof(ArgumentOutOfRangeException), () => 
            { 
                datepicker.Text = "2000/02/02"; 
            });
            Assert.IsTrue(handled, "DateValidationError event not handled");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateValidationError event is fired.
        /// </summary>
        public TestResult DateValidationErrorEvent_2()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += new RoutedEventHandler(DatePicker_Loaded);

            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");

            datepicker.BlackoutDates.Add(new CalendarDateRange(new DateTime(2000, 2, 2)));
            datepicker.DateValidationError += (sender, e) =>
            {
                e.ThrowException = false;
            };

            Assert.IsNull(datepicker.SelectedDate, "DatePicker.SelectedDate should be null");

            ResetTest();
            return TestResult.Pass;
        }

        // 
    }
}
