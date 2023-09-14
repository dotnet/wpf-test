using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Glob = System.Globalization;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "CalendarDayButtonTests", Keywords = "Localization_Suite")]
    public class CalendarDayButtonTests : CalendarTest
    {
        public CalendarDayButtonTests()
            : base()
        {
            this.RunSteps += CalendarDayButtonProperties;
        }

        /// <summary>
        /// Ensure all default values are correct.
        /// </summary>
        public TestResult CalendarDayButtonProperties()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.FirstDayOfWeek = DayOfWeek.Sunday;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            CalendarDayButton todaybutton = tcalendar.CalendarDayButtonByDate(DateTime.Today);
            Assert.IsTrue(todaybutton.IsToday, "CalendarDayButton.IsToday should be true");

            InputHelper.LeftMouseDown(todaybutton);
            // this has changed
            Assert.IsFalse(todaybutton.IsPressed, "CalendarDayButton.IsPressed should be false");
            InputHelper.LeftMouseUp(todaybutton);
            Assert.IsFalse(todaybutton.IsPressed, "CalendarDayButton.IsPressed should be false");
            Assert.IsTrue(todaybutton.IsSelected, "CalendarDayButton.IsSelected should be true");

            calendar.DisplayDate = new DateTime(2008, 11, 01);
            calendar.BlackoutDates.Add(new CalendarDateRange(new DateTime(2008, 11, 5), new DateTime(2008, 11, 11)));
            calendar.SelectedDates.AddRange(new DateTime(2008, 11, 24), new DateTime(2008, 11, 28));
            DispatcherHelper.DoEvents();

            foreach (CalendarDayButton daybutton in tcalendar.Month.Days)
            {
                int day = tcalendar.Month.Days.IndexOf(daybutton);

                // 0 - 5 IsInactive
                if (day >= 0 && day <= 5)
                {
                    Assert.IsTrue(daybutton.IsInactive, string.Format("CalendarDayButton[{0}].IsInactive should be true", day));
                }
                // 6 - 9 !IsInactive
                if (day >= 6 && day <= 9)
                {
                    Assert.IsFalse(daybutton.IsInactive, string.Format("CalendarDayButton[{0}].IsInactive should be false", day));
                }
                // 10 - 16 IsBlackedOut
                if (day >= 10 && day <= 16)
                {
                    Assert.IsTrue(daybutton.IsBlackedOut, string.Format("CalendarDayButton[{0}].IsBlackedOut should be true", day));
                }
                // 17 - 28 !IsInactive
                if (day >= 17 && day <= 28)
                {
                    Assert.IsFalse(daybutton.IsInactive, string.Format("CalendarDayButton[{0}].IsInactive should be false", day));
                }
                // 29 - 33 IsSelected
                if (day >= 29 && day <= 33)
                {
                    Assert.IsTrue(daybutton.IsSelected, string.Format("CalendarDayButton[{0}].IsSelected should be true", day));
                }
                // 34 - 35 !IsInactive
                if (day >= 34 && day <= 35)
                {
                    Assert.IsFalse(daybutton.IsInactive, string.Format("CalendarDayButton[{0}].IsInactive should be false", day));
                }
                // 36 - 41 IsInactive
                if (day >= 36 && day <= 35)
                {
                    Assert.IsTrue(daybutton.IsInactive, string.Format("CalendarDayButton[{0}].IsInactive should be true", day));
                }
            }


            ResetTest();
            return TestResult.Pass;
        }
    }
}
