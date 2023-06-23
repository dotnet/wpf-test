using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "DatePickerTests", Keywords = "Localization_Suite")]
    public class DatePickerTests : DatePickerTest
    {
        public string DatePickerXaml = "<DatePicker SelectedDate=\"2008/04/30\" DisplayDateStart=\"2020/04/30\" DisplayDateEnd=\"2010/04/30\" DisplayDate=\"2000/02/02\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" />";
        
        public DatePickerTests()
            : base()
        {
            this.RunSteps += Create;
            this.RunSteps += CreateInXaml;
            this.RunSteps += CheckDefaultValues;
            this.RunSteps += CheckValuesAfterLoad;
            this.RunSteps += PropertiesAreNullable;
        }

        /// <summary>
        /// Create a DatePicker control.
        /// </summary>
        public TestResult Create()
        {
            DatePicker datepicker = new DatePicker();
            Assert.IsNotNull(datepicker, "DatePicker should not be null.");

            return TestResult.Pass;
        }

        /// <summary>
        /// Create a DatePicker in XAML markup.
        /// </summary>
        public TestResult CreateInXaml()
        {
            object _datepicker = XamlReader.Parse(DatePickerXaml);
            Assert.IsInstanceOfType(_datepicker, typeof(DatePicker));

            DatePicker datepicker = _datepicker as DatePicker;
            datepicker.Loaded += new RoutedEventHandler(DatePicker_Loaded);

            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker from XAML never fired loaded event.");
            Assert.AreEqual(datepicker.SelectedDate.Value, new DateTime(2008, 4, 30));
            Assert.AreEqual(datepicker.DisplayDateStart.Value, new DateTime(2008, 4, 30));
            Assert.AreEqual(datepicker.DisplayDate, new DateTime(2008, 4, 30));
            Assert.AreEqual(datepicker.DisplayDateEnd.Value, new DateTime(2010, 4, 30));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure all default values are correct.
        /// </summary>
        public TestResult CheckDefaultValues()
        {
            DatePicker datepicker = new DatePicker();
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datepicker.DisplayDate, DateTime.Today);
            Assert.IsNull(datepicker.DisplayDateStart, "DisplayDateStart should be null.");
            Assert.IsNull(datepicker.DisplayDateEnd, "DisplayDateEnd should be null.");
            Assert.AreEqual(datepicker.FirstDayOfWeek, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
            Assert.IsFalse(datepicker.IsDropDownOpen, "IsDropDownOpen should be false.");
            Assert.IsTrue(datepicker.IsTodayHighlighted, "IsTodayHighlighted should be true.");
            Assert.IsNull(datepicker.SelectedDate, "SelectedDate should be null.");
            Assert.AreEqual(datepicker.SelectedDateFormat, DatePickerFormat.Short);
            Assert.IsTrue(datepicker.BlackoutDates.Count == 0, "BlackoutDates should be == 0");
            Assert.AreEqual(datepicker.Text, string.Empty);
            DateTime actual = (DateTime)datepicker.GetValue(DatePicker.DisplayDateProperty);
            Assert.AreEqual(actual, DateTime.Today);
            Assert.IsNull((DateTime?)datepicker.GetValue(DatePicker.DisplayDateEndProperty), "DatePicker.DisplayDateEndProperty should be null.");
            Assert.IsNull((DateTime?)datepicker.GetValue(DatePicker.DisplayDateStartProperty), "DatePicker.DisplayDateStartProperty should be null.");
            Assert.AreEqual((DayOfWeek)datepicker.GetValue(DatePicker.FirstDayOfWeekProperty), DayOfWeek.Sunday);
            Assert.IsFalse((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "DatePicker.IsDropDownOpenProperty should be false.");
            Assert.IsTrue((bool)datepicker.GetValue(DatePicker.IsTodayHighlightedProperty), "DatePicker.IsTodayHighlightedProperty should be true.");
            Assert.IsNull((DateTime?)datepicker.GetValue(DatePicker.SelectedDateProperty), "DatePicker.SelectedDateProperty should be null.");
            Assert.AreEqual((DatePickerFormat)datepicker.GetValue(DatePicker.SelectedDateFormatProperty), DatePickerFormat.Short);
            Assert.AreEqual((string)datepicker.GetValue(DatePicker.TextProperty), string.Empty);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify properties set on DatePicker before load are reflected on the DatePicker after load.
        /// </summary>
        public TestResult CheckValuesAfterLoad()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.DisplayDateStart = DateTime.Today.AddDays(-100);
            datepicker.DisplayDateEnd = DateTime.Today.AddDays(1000);
            datepicker.FirstDayOfWeek = DayOfWeek.Wednesday;
            datepicker.IsDropDownOpen = true;
            datepicker.IsTodayHighlighted = false;
            datepicker.SelectedDate = DateTime.Today.AddDays(200);
            datepicker.DisplayDate = DateTime.Today.AddDays(500);
            datepicker.SelectedDateFormat = DatePickerFormat.Long;
            datepicker.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(300), DateTime.Today.AddDays(500)));
            datepicker.Text = "testbeforeload";

            datepicker.Loaded += new RoutedEventHandler(DatePicker_Loaded);
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event");
            Assert.AreEqual(datepicker.DisplayDate, DateTime.Today.AddDays(500));
            Assert.AreEqual(datepicker.DisplayDateStart.Value, DateTime.Today.AddDays(-100));
            Assert.AreEqual(datepicker.DisplayDateEnd.Value, DateTime.Today.AddDays(1000));
            Assert.AreEqual(datepicker.FirstDayOfWeek, DayOfWeek.Wednesday);
            Assert.IsTrue(datepicker.IsDropDownOpen, "IsDropDownOpen should be true");
            Assert.IsFalse(datepicker.IsTodayHighlighted, "IsTodayHighlighted should be false");
            Assert.AreEqual(datepicker.SelectedDate.Value, DateTime.Today.AddDays(200));
            Assert.AreEqual(datepicker.SelectedDateFormat, DatePickerFormat.Long);
            Assert.AreEqual(datepicker.BlackoutDates[0].Start, DateTime.Today.AddDays(300));
            Assert.AreEqual(datepicker.BlackoutDates[0].End, DateTime.Today.AddDays(500));

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure Nullable Properties can be set to null.
        /// </summary>
        public TestResult PropertiesAreNullable()
        {
            DatePicker datepicker = new DatePicker();
            DateTime value = DateTime.Today;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            datepicker.SelectedDate = null;
            Assert.IsNull(datepicker.SelectedDate, "SelectedDate should be null [1].");

            datepicker.SelectedDate = value;
            Assert.AreEqual((DateTime)datepicker.SelectedDate,value);

            datepicker.SelectedDate = null;
            Assert.IsNull(datepicker.SelectedDate, "SelectedDate should be null [2].");

            ResetTest();
            return TestResult.Pass;
        }
    }
}
