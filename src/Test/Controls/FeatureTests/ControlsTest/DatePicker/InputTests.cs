using System;
using System.Globalization;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;
using System.Windows.Input;

namespace Microsoft.Test.Controls
{
    [Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "InputTests", Keywords = "Localization_Suite")]
    public class DatePickerInputTests : DatePickerTest
    {
        public DatePickerInputTests()
            : base()
        {
            this.RunSteps += TextBox_Input;
        }

        /// <summary>
        /// Click DatePickerTextBox
        /// </summary>
        public TestResult TextBox_Input()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.DisplayDate = new DateTime(2008, 10, 01);
            datepicker.BlackoutDates.Add(new CalendarDateRange(new DateTime(2008, 10, 10), new DateTime(2008, 10, 15)));
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);

            InputHelper.Click(tdatepicker.TextBox);
            Assert.IsTrue(tdatepicker.TextBox.IsFocused, "DatePickerTextBox should be focused");

            InputHelper.PushKey(System.Windows.Input.Key.LeftAlt);
            InputHelper.PressKey(System.Windows.Input.Key.Down);
            InputHelper.ReleaseKey(System.Windows.Input.Key.LeftAlt);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be true");

            TemplatedCalendar tcalendar = new TemplatedCalendar(tdatepicker.Calendar);
            InputHelper.Click(tcalendar.Month.Days[17]);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be true");

            InputHelper.Click(tcalendar.Month.Days[0]);
            DispatcherHelper.DoEvents();

            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");
            Assert.IsFalse((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be false");

            InputHelper.Click(tdatepicker.Button);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be true");

            InputHelper.Click(tcalendar.CalendarDayButtonByDay(15));
            DispatcherHelper.DoEvents();

            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");
            Assert.IsFalse((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be false");

            ResetTest();
            return TestResult.Pass;
        }
    }
}
