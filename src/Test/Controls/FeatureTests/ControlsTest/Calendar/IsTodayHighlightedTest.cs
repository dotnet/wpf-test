using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "IsTodayHighlightedTest", Keywords = "Localization_Suite")]
    public class IsTodayHighlightedTest : CalendarTest
    {
        public IsTodayHighlightedTest()
            : base()
        {
            this.RunSteps += IsToday;
        }
        
        /// <summary>
        /// Verify IsToday propery.
        /// </summary>
        public TestResult IsToday()
        {
            Calendar calendar = new Calendar();
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar did not fire loaded event.");

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            CalendarDayButton day = tcalendar.CalendarDayButtonByDate(DateTime.Today);

            Assert.IsTrue(day.IsToday, "DayButton.IsToday should be true");

            tcalendar = new TemplatedCalendar(calendar);
            day = tcalendar.CalendarDayButtonByDate(DateTime.Today.AddDays(1));
            if(day == null)
                day = tcalendar.CalendarDayButtonByDate(DateTime.Today.AddDays(-1));
            Assert.IsFalse(day.IsToday, "DayButton.IsToday should not be true");

            ResetTest();

            return TestResult.Pass;
        }
    }
}
