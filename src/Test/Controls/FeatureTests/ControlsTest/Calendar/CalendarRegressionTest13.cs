using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// CalendarRegressionTest13
    /// </description>
    /// </summary>
    [Test(1, "Calendar", "CalendarRegressionTest13")]
    public class CalendarRegressionTest13 : XamlTest
    {
        #region Private Members

        private Calendar calendar;

        #endregion

        #region Public Members

        public CalendarRegressionTest13()
            : base(@"CalendarRegressionTest13.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        public TestResult Setup()
        {
            Status("Setup");

            calendar = (Calendar)RootElement.FindName("calendar");

            MethodInfo findDayButtonFromDay = calendar.GetType().GetMethod("FindDayButtonFromDay", BindingFlags.NonPublic | BindingFlags.Instance);
            CalendarDayButton button = (CalendarDayButton)findDayButtonFromDay.Invoke(calendar, new Object[] { new DateTime(2009, 3, 31) });

            InputHelper.MouseClickButtonCenter(button, button.ClickMode, System.Windows.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            calendar = null;
            return TestResult.Pass;
        }

        public TestResult Repro()
        {
            // Validate directional navigation works when the last day is selected
            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Right);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            if (((DateTime)calendar.SelectedDate).Day == 31)
            {
                throw new TestValidationException("Fail: calendar.SelectedDate.Day == 31; Press right arrow key doesn't change date.");
            }

            return TestResult.Pass;
        }

        #endregion
    } 
}
