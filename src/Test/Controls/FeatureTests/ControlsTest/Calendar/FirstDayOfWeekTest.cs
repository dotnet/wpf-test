using System;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "FirstDayOfWeekTest", Keywords = "Localization_Suite")]
    public class FirstDayOfWeekTest : CalendarTest
    {
        public FirstDayOfWeekTest()
            : base()
        {
            this.RunSteps += FirstDayOfWeekPropertySetValue;
            this.RunSteps += FirstDayOfWeekOutOfRangeException;
        }

        /// <summary>
        /// Set the value of FirstDayOfWeekProperty.
        /// </summary>
        public TestResult FirstDayOfWeekPropertySetValue()
        {
            Calendar calendar = new Calendar();
            DayOfWeek value = DayOfWeek.Thursday;
            calendar.FirstDayOfWeek = value;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(value, (DayOfWeek)calendar.GetValue(Calendar.FirstDayOfWeekProperty));
            Assert.AreEqual(value, calendar.FirstDayOfWeek);

            value = (DayOfWeek)3;
            calendar.FirstDayOfWeek = value;
            Assert.AreEqual(value, calendar.FirstDayOfWeek);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure ArgumentException is thrown when casting DayOfWeek to value greater than 6
        /// </summary>
        public TestResult FirstDayOfWeekOutOfRangeException()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentException), () =>
            {
                calendar.FirstDayOfWeek = (DayOfWeek)7;
            });

            ResetTest();
            return TestResult.Pass;
        }
    }
}
