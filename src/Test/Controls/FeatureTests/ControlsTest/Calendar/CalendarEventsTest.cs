using System;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    // [ DISABLED_WHILE_PORTING ]
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "CalendarEventsTest", Disabled=true)]
    public class CalendarEventsTest : CalendarTest
    {
        public CalendarEventsTest()
            : base()
        {
            this.RunSteps += DateSelectedEvent;
            this.RunSteps += DisplayDateChangedEvent;
            this.RunSteps += DisplayDateChangedEventArgs;
            this.RunSteps += DisplayDateChangedDisplayDateStart;
            this.RunSteps += DisplayDateChangedDisplayDateEnd;
            this.RunSteps += DisplayDateChangedNavigation_01;
            this.RunSteps += DisplayDateChangedNavigation_02;
        }

        /// <summary>
        /// Ensure DateSelected event is fired.
        /// </summary>
        public TestResult DateSelectedEvent()
        {
            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;

            calendar.SelectedDatesChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime value = new DateTime(2000, 10, 10);
            calendar.SelectedDate = value;

            Assert.IsTrue(handled, "Event not handled.");
            Assert.AreEqual(calendar.ToString(), value.ToString());

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDateChanged event is fired.
        /// </summary>
        public TestResult DisplayDateChangedEvent()
        {
            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;

            calendar.DisplayDateChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime value = new DateTime(2000, 10, 10);
            calendar.DisplayDate = value;
            Assert.IsTrue(handled, "Event not handled.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDateChanged event is fired.
        /// </summary>
        public TestResult DisplayDateChangedEventArgs()
        {
            bool handled = false;

            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;

            calendar.DisplayDateChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime value = new DateTime(2000, 10, 10);
            calendar.DisplayDate = value;
            Assert.IsTrue(handled, "Event not handled.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDateChanged event is fired when DisplayDateStart is updated
        /// </summary>
        public TestResult DisplayDateChangedDisplayDateStart()
        {
            bool handled = false;
            DateTime today = DateTime.Today;
            DateTime noupdate = DateTime.Today.AddDays(-30);
            DateTime update = DateTime.Today.AddDays(30);

            Calendar calendar = new Calendar();
            calendar.DisplayDate = today;
            calendar.DisplayDateChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDateStart = noupdate;
            Assert.IsFalse(handled, "Event should not have fired.");

            calendar.DisplayDateStart = update;
            Assert.IsTrue(handled, "Event not handled.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDateChanged event is fired when DisplayDateEnd is updated
        /// </summary>
        public TestResult DisplayDateChangedDisplayDateEnd()
        {
            bool handled = false;
            DateTime today = DateTime.Today;
            DateTime noupdate = DateTime.Today.AddDays(30);
            DateTime update = DateTime.Today.AddDays(-30);

            Calendar calendar = new Calendar();
            calendar.DisplayDate = today;
            calendar.DisplayDateChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDateEnd = noupdate;
            Assert.IsFalse(handled, "Event should not have fired.");

            calendar.DisplayDateEnd = update;
            Assert.IsTrue(handled, "Event not handled.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDateChanged event is fired when Navigating calendar
        /// </summary>
        public TestResult DisplayDateChangedNavigation_01()
        {
            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = DateTime.Today;
            calendar.DisplayDateChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.NextButton);
            Assert.IsTrue(handled, "Event not handled.");

            handled = false;

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.IsTrue(handled, "Event not handled.");

            handled = false;

            InputHelper.Click(tcalendar.HeaderButton);
            Assert.IsFalse(handled, "Event should not have fired.");

            InputHelper.Click(tcalendar.Year.Items[4]);
            Assert.IsTrue(handled, "Event not handled.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDateChanged event is fired when Navigating calendar
        /// </summary>
        public TestResult DisplayDateChangedNavigation_02()
        {
            bool handled = false;
            Calendar calendar = new Calendar();
            calendar.DisplayDate = DateTime.Today;
            calendar.DisplayDateChanged += (sender, e) =>
            {
                handled = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            InputHelper.Click(tcalendar.NextButton);
            Assert.IsTrue(handled, "Event not handled.");

            handled = false;

            InputHelper.Click(tcalendar.PreviousButton);
            Assert.IsTrue(handled, "Event not handled.");

            handled = false;

            InputHelper.Click(tcalendar.HeaderButton);
            Assert.IsFalse(handled, "Event should not have fired.");

            InputHelper.Click(tcalendar.HeaderButton);
            Assert.IsFalse(handled, "Event should not have fired.");
            InputHelper.Click(tcalendar.Year.Items[2]);
            Assert.IsTrue(handled, "Event not handled.");

            InputHelper.Click(tcalendar.Year.Items[6]);
            Assert.IsTrue(handled, "Event not handled.");

            ResetTest();
            return TestResult.Pass;
        }
    }
}
