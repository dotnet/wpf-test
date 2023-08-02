using System;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "DPFirstDayOfWeekTest", Keywords = "Localization_Suite")]
    public class DPFirstDayOfWeekTest : DatePickerTest
    {
        public DPFirstDayOfWeekTest()
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
            DatePicker datepicker = new DatePicker();
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            DayOfWeek value = DayOfWeek.Thursday;
            datepicker.FirstDayOfWeek = value;

            Assert.AreEqual(value, (DayOfWeek)datepicker.GetValue(DatePicker.FirstDayOfWeekProperty));
            Assert.AreEqual(value, datepicker.FirstDayOfWeek);

            value = (DayOfWeek)3;
            datepicker.FirstDayOfWeek = value;
            Assert.AreEqual(value, datepicker.FirstDayOfWeek);
            Assert.AreEqual(value, (DayOfWeek)datepicker.GetValue(DatePicker.FirstDayOfWeekProperty));
            
            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure ArgumentException is thrown when casting DayOfWeek to value greater than 6
        /// </summary>
        public TestResult FirstDayOfWeekOutOfRangeException()
        {
            DatePicker datepicker = new DatePicker(); 
            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(ArgumentException), () =>
            {
                datepicker.FirstDayOfWeek = (DayOfWeek)7;
            });

            ResetTest();
            return TestResult.Pass;
        }
    }
}
