using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "DropDownTest", Keywords = "MicroSuite")]
    public class DropDownTest : DatePickerTest
    {
        public DropDownTest()
            : base()
        {
            this.RunSteps += IsDropDownOpen;
            this.RunSteps += ClickDropDownButton;
            this.RunSteps += NavigateCalendarModes;
            this.RunSteps += NavigateMonths;
            this.RunSteps += NavigateYears;
            this.RunSteps += NavigateDecades;
            this.RunSteps += CloseDropDown_ESC;
            this.RunSteps += CloseDropDown_ClickAway;
        }

        /// <summary>
        /// Ensure DropDown is opening/closing properly.
        /// </summary>
        public TestResult IsDropDownOpen()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.Loaded += DatePicker_Loaded;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");
            datepicker.IsDropDownOpen = true;
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);

            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be true");

            datepicker.IsDropDownOpen = false;
            DispatcherHelper.DoEvents();

            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");
            Assert.IsFalse((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be false");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Click drop down button to open and select date to close
        /// </summary>
        public TestResult ClickDropDownButton()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.VerticalAlignment = VerticalAlignment.Center;
            datepicker.HorizontalAlignment = HorizontalAlignment.Center;

            bool opened = false;
            datepicker.CalendarOpened += (sender,e)=>
            {
                opened = true;
            };

            bool closed = false;
            datepicker.CalendarClosed += (sender, e) =>
            {
                closed = true;
            };

            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);

            InputHelper.Click(tdatepicker.Button);
            DispatcherHelper.DoEvents();

            tdatepicker = new TemplatedDatePicker(datepicker);

            Assert.IsTrue(opened, "CalendarOpened event never fired");
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be true");

            TemplatedCalendar tcalendar =new TemplatedCalendar(tdatepicker.Calendar);

            InputHelper.Click(tcalendar.Month.Days[18]);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(closed, "CalendarClosed event never fired");
            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");
            Assert.IsFalse((bool)datepicker.GetValue(DatePicker.IsDropDownOpenProperty), "IsDropDownOpen should be false");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure the popup does not close when navigating through the CalendarModes.
        /// </summary>
        public TestResult NavigateCalendarModes()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.VerticalAlignment = VerticalAlignment.Center;
            datepicker.HorizontalAlignment = HorizontalAlignment.Center;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            InputHelper.Click(tdatepicker.Button);

            tdatepicker = new TemplatedDatePicker(datepicker);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");

            TemplatedCalendar tcalendar = new TemplatedCalendar(tdatepicker.Calendar);

            InputHelper.Click(tcalendar.HeaderButton);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.AreEqual(tdatepicker.Calendar.DisplayMode, CalendarMode.Year);

            InputHelper.Click(tcalendar.Year.Items[1]);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.AreEqual(tdatepicker.Calendar.DisplayMode, CalendarMode.Month);

            InputHelper.Click(tcalendar.HeaderButton);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.AreEqual(tdatepicker.Calendar.DisplayMode, CalendarMode.Year);

            InputHelper.Click(tcalendar.HeaderButton);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.AreEqual(tdatepicker.Calendar.DisplayMode, CalendarMode.Decade);

            InputHelper.Click(tcalendar.Year.Items[4]);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.AreEqual(tdatepicker.Calendar.DisplayMode, CalendarMode.Year);


            InputHelper.Click(tdatepicker.Button);
	    	InputHelper.Click(tdatepicker.Button); //
            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure the popup does not close when navigating through the Months.
        /// </summary>
        public TestResult NavigateMonths()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.VerticalAlignment = VerticalAlignment.Center;
            datepicker.HorizontalAlignment = HorizontalAlignment.Center;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            tdatepicker.Calendar.DisplayMode = CalendarMode.Month;

            InputHelper.Click(tdatepicker.Button);

            tdatepicker = new TemplatedDatePicker(datepicker);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");

            TemplatedCalendar tcalendar = new TemplatedCalendar(tdatepicker.Calendar);

            bool nextclicked = false;
            bool prevclicked = false;
            tcalendar.NextButton.Click += (sender, e) =>
            {
                nextclicked = true;
            };
            tcalendar.PreviousButton.Click += (sender, e) =>
            {
                prevclicked = true;
            };

            InputHelper.Click(tcalendar.NextButton);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue(nextclicked, "NextButton.Click was not fired");

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue(prevclicked, "PreviousButton.Click was not fired");

            InputHelper.Click(tdatepicker.Button);
            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure the popup does not close when navigating through the Year.
        /// </summary>
        public TestResult NavigateYears()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.VerticalAlignment = VerticalAlignment.Center;
            datepicker.HorizontalAlignment = HorizontalAlignment.Center;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            tdatepicker.Calendar.DisplayMode = CalendarMode.Year;

            InputHelper.Click(tdatepicker.Button);
            tdatepicker = new TemplatedDatePicker(datepicker);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");

            TemplatedCalendar tcalendar = new TemplatedCalendar(tdatepicker.Calendar);

            bool nextclicked = false;
            bool prevclicked = false;
            tcalendar.NextButton.Click += (sender, e) =>
            {
                nextclicked = true;
            };
            tcalendar.PreviousButton.Click += (sender, e) =>
            {
                prevclicked = true;
            };

            InputHelper.Click(tcalendar.NextButton);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue(nextclicked, "NextButton.Click was not fired");

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue(prevclicked, "PreviousButton.Click was not fired");

            InputHelper.Click(tdatepicker.Button);
            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure the popup does not close when navigating through the Decade.
        /// </summary>
        public TestResult NavigateDecades()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.VerticalAlignment = VerticalAlignment.Center;
            datepicker.HorizontalAlignment = HorizontalAlignment.Center;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            tdatepicker.Calendar.DisplayMode = CalendarMode.Decade;

            InputHelper.Click(tdatepicker.Button);

            tdatepicker = new TemplatedDatePicker(datepicker);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");

            TemplatedCalendar tcalendar = new TemplatedCalendar(tdatepicker.Calendar);

            bool nextclicked = false;
            bool prevclicked = false;
            tcalendar.NextButton.Click += (sender, e) =>
            {
                nextclicked = true;
            };
            tcalendar.PreviousButton.Click += (sender, e) =>
            {
                prevclicked = true;
            };

            InputHelper.Click(tcalendar.NextButton);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue(nextclicked, "NextButton.Click was not fired");

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");
            Assert.IsTrue(prevclicked, "PreviousButton.Click was not fired");

            InputHelper.Click(tdatepicker.Button);
            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure the popup does not close when navigating through the Decade.
        /// </summary>
        public TestResult CloseDropDown_ESC()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.VerticalAlignment = VerticalAlignment.Center;
            datepicker.HorizontalAlignment = HorizontalAlignment.Center;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);

            InputHelper.Click(tdatepicker.Button);

            tdatepicker = new TemplatedDatePicker(datepicker);
            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");

            InputHelper.PressKey(System.Windows.Input.Key.Escape);

            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");
         
            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure the popup does not close when navigating through the Decade.
        /// </summary>
        public TestResult CloseDropDown_ClickAway()
        {
            DatePicker datepicker = new DatePicker();
            datepicker.VerticalAlignment = VerticalAlignment.Center;
            datepicker.HorizontalAlignment = HorizontalAlignment.Center;
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);
            tdatepicker = new TemplatedDatePicker(datepicker);

            InputHelper.Click(tdatepicker.Button);

            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.IsTrue(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be true");

            InputHelper.ClickAt(new Point(100, 100));

            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.IsFalse(tdatepicker.Calendar.IsVisible, "Calendar.IsVisible should be false");

            ResetTest();
            return TestResult.Pass;
        }
    }
}
