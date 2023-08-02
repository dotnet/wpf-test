using System;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "DisplayDateTest", Keywords = "Localization_Suite")]
    public class DisplayDateTest : CalendarTest
    {
        public DisplayDateTest()
            : base()
        {
            this.RunSteps += ArePropertiesNullable;
            this.RunSteps += SetToMaxValue;
            this.RunSteps += SetToMinValue;
            this.RunSteps += DisplayDatePropertySetValue;
            this.RunSteps += DisplayDateStartEnd;
            this.RunSteps += DisplayDateRangeEnd;
            this.RunSteps += DisplayDateEndEqualsSelectedDate;
            this.RunSteps += DisplayDateRangeStartEqualsSelectedDate;
            this.RunSteps += DatePropertiesSetToMaxValue;
            this.RunSteps += DatePropertiesSetToMinValue;
            this.RunSteps += DisplayDateChangedEvent;
            this.RunSteps += DisplayDateChangedEventArgs;
        }

        /// <summary>
        /// Ensure Nullable Properties can be set to null.
        /// </summary>
        public TestResult ArePropertiesNullable()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime value = DateTime.Today;

            calendar.DisplayDateStart = null;
            Assert.IsNull(calendar.DisplayDateStart, "DisplayDateStart was not set to null.");

            calendar.DisplayDateStart = value;
            Assert.AreEqual(calendar.DisplayDateStart.Value, value);

            calendar.DisplayDateStart = null;
            Assert.IsNull(calendar.DisplayDateStart, "DisplayDateStart was not set back to null.");

            calendar.DisplayDateEnd = null;
            Assert.IsNull(calendar.DisplayDateEnd, "DisplayDateEnd was not set to null.");

            calendar.DisplayDateEnd = value;
            Assert.AreEqual(calendar.DisplayDateEnd.Value, value);

            calendar.DisplayDateEnd = null;
            Assert.IsNull(calendar.DisplayDateEnd, "DisplayDateEnd was not set back to null.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MaxValue.
        /// </summary>
        public TestResult SetToMaxValue()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDate = DateTime.MaxValue;

            Assert.AreEqual(calendar.DisplayDate, DateTime.MaxValue);

            calendar.DisplayDateEnd = DateTime.MinValue;
            calendar.DisplayDateStart = DateTime.MaxValue;

            Assert.AreEqual(calendar.DisplayDateStart.Value, DateTime.MaxValue);
            Assert.AreEqual(calendar.DisplayDateEnd.Value, DateTime.MaxValue);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MinValue.
        /// </summary>
        public TestResult SetToMinValue()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDate = DateTime.MinValue;

            Assert.AreEqual(calendar.DisplayDate, DateTime.MinValue);

            calendar.DisplayDateStart = DateTime.MinValue;
            calendar.DisplayDateEnd = DateTime.MinValue;

            Assert.AreEqual(calendar.DisplayDateStart.Value, DateTime.MinValue);
            Assert.AreEqual(calendar.DisplayDateEnd.Value, DateTime.MinValue);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Set the value of DisplayDateProperty.
        /// </summary>
        public TestResult DisplayDatePropertySetValue()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime value = DateTime.Today.AddMonths(3);
            calendar.DisplayDate = value;

            Assert.AreEqual(calendar.DisplayDate, value);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Set the value of DisplayDateStart and DisplayDateEnd
        /// </summary>
        public TestResult DisplayDateStartEnd()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.DisplayDateStart = new DateTime(2005, 12, 30);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime value = new DateTime(2005, 12, 15);
            calendar.DisplayDate = value;

            Assert.AreEqual(calendar.DisplayDateStart.Value, calendar.DisplayDate);

            value = new DateTime(2005, 12, 30);
            calendar.DisplayDate = value;

            Assert.AreEqual(calendar.DisplayDate, value);

            value = DateTime.MaxValue;
            calendar.DisplayDate = value;

            Assert.AreEqual(calendar.DisplayDate, value);

            calendar.DisplayDateEnd = new DateTime(2010, 12, 30);
            Assert.AreEqual(calendar.DisplayDateEnd.Value, calendar.DisplayDate);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDate/Start/End are equal after updating DisplayDateStart
        /// </summary>
        public TestResult DisplayDateRangeEnd()
        {
            Calendar calendar = new Calendar();
            DateTime value = new DateTime(2000, 1, 30);
            calendar.DisplayDate = value;
            calendar.DisplayDateEnd = value;
            calendar.DisplayDateStart = value;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.DisplayDateStart.Value, value);
            Assert.AreEqual(calendar.DisplayDateEnd.Value, value);

            value = value.AddMonths(2);
            calendar.DisplayDateStart = value;

            Assert.AreEqual(calendar.DisplayDateStart.Value, value);
            Assert.AreEqual(calendar.DisplayDateEnd.Value, value);

            Assert.AreEqual(calendar.DisplayDate, value);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDateEnd is not less than SelectedDate
        /// </summary>
        public TestResult DisplayDateEndEqualsSelectedDate()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDate = DateTime.MaxValue;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();
            
            Assert.AreEqual((DateTime)calendar.SelectedDate, DateTime.MaxValue);

            calendar.DisplayDateEnd = DateTime.MaxValue.AddDays(-1);
            
            Assert.AreEqual((DateTime)calendar.DisplayDateEnd, DateTime.MaxValue);

            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DisplayDateStart is not greater than SelectedDate
        /// </summary>
        public TestResult DisplayDateRangeStartEqualsSelectedDate()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDate = DateTime.MinValue;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.AreEqual((DateTime)calendar.SelectedDate, DateTime.MinValue);
            calendar.DisplayDateStart = DateTime.MinValue.AddDays(1);
            Assert.AreEqual((DateTime)calendar.DisplayDateStart, DateTime.MinValue);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MaxValue.
        /// </summary>
        public TestResult DatePropertiesSetToMaxValue()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDate = DateTime.MaxValue;
            Assert.AreEqual(calendar.DisplayDate, DateTime.MaxValue);

            calendar.SelectedDate = DateTime.MaxValue;
            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.MaxValue);

            calendar.DisplayDateStart = DateTime.MaxValue;
            Assert.AreEqual(calendar.DisplayDateStart.Value, DateTime.MaxValue);

            calendar.DisplayDateEnd = DateTime.MaxValue;
            Assert.AreEqual(calendar.DisplayDateEnd.Value, DateTime.MaxValue);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MinValue.
        /// </summary>
        public TestResult DatePropertiesSetToMinValue()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDate = DateTime.MinValue;
            Assert.AreEqual(calendar.DisplayDate, DateTime.MinValue);

            calendar.DisplayDateStart = DateTime.MinValue;
            Assert.AreEqual(calendar.DisplayDateStart.Value, DateTime.MinValue);

            calendar.DisplayDateEnd = DateTime.MinValue;
            Assert.AreEqual(calendar.DisplayDateEnd.Value, DateTime.MinValue);

            calendar.SelectedDate = null;

            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue));
            Assert.AreEqual(calendar.BlackoutDates[0].End, DateTime.MinValue);
            Assert.AreEqual(calendar.BlackoutDates[0].Start, DateTime.MinValue);

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
            DateTime? added = null;
            DateTime? removed = null;

            Calendar calendar = new Calendar();
            calendar.DisplayDate = DateTime.Today;
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;

            calendar.DisplayDateChanged += (sender, e) =>
            {
                handled = true;
                added = e.AddedDate;
                removed = e.RemovedDate;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime value = new DateTime(2000, 10, 10);
            calendar.DisplayDate = value;

            Assert.IsTrue(handled, "Event not handled.");
            Assert.IsTrue(added.HasValue, "DisplayDate that was added does not have a value.");
            Assert.AreEqual(added.Value, value);
            Assert.IsTrue(removed.HasValue, "DisplayDate that was removed does not have a value.");
            Assert.AreEqual(removed.Value, DateTime.Today);

            handled = false;
            calendar.DisplayDate = DateTime.Today;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(handled, "Event not handled.");
            Assert.IsTrue(added.HasValue, "DisplayDate that was added does not have a value.");
            Assert.AreEqual(added.Value, DateTime.Today);
            Assert.IsTrue(removed.HasValue, "DisplayDate that was removed does not have a value.");
            Assert.AreEqual(removed.Value, value);

            ResetTest();
            return TestResult.Pass;
        }
    }
}
