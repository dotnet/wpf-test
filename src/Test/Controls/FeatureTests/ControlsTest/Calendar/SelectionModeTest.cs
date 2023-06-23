using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "SelectionModeTest", Keywords = "Localization_Suite")]
    public class SelectionModeTest : CalendarTest
    {
        public SelectionModeTest()
            : base()
        {
            this.RunSteps += SelectionMode;
            this.RunSteps += SelectionModeOutOfRangeException;
            this.RunSteps += SelectionModeChanged;
        }

        /// <summary>
        /// Verify SelectionMode property
        /// </summary>
        public TestResult SelectionMode()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today.AddDays(10));
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar from never fired loaded event.");
            Assert.AreEqual(calendar.SelectionMode, CalendarSelectionMode.SingleRange);
            Assert.IsTrue(calendar.SelectedDates.Count == 11, "SelectedDates.Count should be equal to 11");

            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(calendar.SelectedDates.Count == 0, "SelectedDate.Count should be equal to 0 after switching CalendarSelectionMode");
            Assert.AreEqual(calendar.SelectionMode, CalendarSelectionMode.MultipleRange);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure ArgumentException is thrown casting int greate than 3 to CalendarSelectionMode
        /// </summary>
        public TestResult SelectionModeOutOfRangeException()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentException), () =>
            {
                calendar.SelectionMode = (CalendarSelectionMode)7;
            });

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify SelectionModeChanged event
        /// </summary>
        public TestResult SelectionModeChanged()
        {
            bool changed = false;

            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.None;
            calendar.SelectionModeChanged += (sender, e) =>
            {
                changed = true;
            };

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.SelectionMode, CalendarSelectionMode.None);

            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(changed, "SelectionModeChanged was not fired");
            Assert.AreEqual(calendar.SelectionMode, CalendarSelectionMode.SingleDate);

            changed = false;
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(changed, "SelectionModeChanged was not fired");
            Assert.AreEqual(calendar.SelectionMode, CalendarSelectionMode.SingleRange);

            changed = false;
            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(changed, "SelectionModeChanged was not fired");
            Assert.AreEqual(calendar.SelectionMode, CalendarSelectionMode.MultipleRange);
            
            ResetTest();
            return TestResult.Pass;
        }
    }
}
