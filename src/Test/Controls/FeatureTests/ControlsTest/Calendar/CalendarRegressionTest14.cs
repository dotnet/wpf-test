using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// CalendarRegressionTest14
    /// </description>
    /// </summary>
    [Test(1, "Calendar", "CalendarRegressionTest14")]
    public class CalendarRegressionTest14 : XamlTest
    {
        #region Private Members

        private Calendar calendar;

        #endregion

        #region Public Members

        public CalendarRegressionTest14()
            : base(@"CalendarRegressionTest14.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        public TestResult Setup()
        {
            Status("Setup");

            calendar = (Calendar)RootElement.FindName("calendar");

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            calendar = null;
            return TestResult.Pass;
        }


        /// <summary>
        /// 1, Mouse click on the first active button on the default Calendar
        /// 2, Shift mouse click on the second active button
        /// 3, Validate the first two active buttons are selected and highlighted
        /// 4, Programmatically clear Calendar SelectedDates and add the forth active button to Calendar SelectedDates collection
        /// 5, Validate the first two active buttons are unselected and unhighlighted and validate the forth active button is selected and highlighted
        /// </summary>
        /// <returns></returns>
        public TestResult Repro()
        {
            Collection<CalendarDayButton> buttons = Microsoft.Test.Controls.VisualTreeHelper.GetVisualChildren<CalendarDayButton>(calendar);

            var activeButtons =
                from b in buttons
                where b.IsInactive == false
                select b;

            if (activeButtons.Count<CalendarDayButton>() < 5)
            {
                throw new TestValidationException(String.Format("Fail: there're not enough active buttons to test this scenairo; {0} buttons only.", activeButtons.Count<CalendarDayButton>()));
            }

            CalendarDayButton firstActiveButton = activeButtons.ElementAt<CalendarDayButton>(0);

            InputHelper.MouseClickButtonCenter(firstActiveButton, ClickMode.Release, System.Windows.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            // Validate the first button is selected and highlighted
            // CalendarDayButton is selected and highlighted when Opacity is 0.75
            ValidateCalendarDayButtonSelectedBackground(firstActiveButton, 0.75);

            CalendarDayButton secondActiveButton = activeButtons.ElementAt<CalendarDayButton>(1);

            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftShift);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            InputHelper.MouseClickButtonCenter(secondActiveButton, ClickMode.Release, System.Windows.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftShift);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            ValidateCalendarDayButtonSelectedBackground(secondActiveButton, 0.75);

            // Move away the mouse cursor
            Microsoft.Test.Input.Mouse.MoveTo(new System.Drawing.Point());
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            Nullable<DateTime> date = secondActiveButton.DataContext as Nullable<DateTime>;
            DateTime newDate = date.Value.Add(TimeSpan.FromDays(2));

            calendar.SelectedDates.Clear();
            calendar.SelectedDates.Add(newDate);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            // Validate the first two buttons are unselected and unhighlighted
            ValidateCalendarDayButtonSelectedBackground(firstActiveButton, 0);
            ValidateCalendarDayButtonSelectedBackground(secondActiveButton, 0);

            CalendarDayButton newActiveButton = activeButtons.ElementAt<CalendarDayButton>(3);
            ValidateCalendarDayButtonSelectedBackground(newActiveButton, 0.75);

            return TestResult.Pass;
        }

        private void ValidateCalendarDayButtonSelectedBackground(CalendarDayButton calendarDayButton, double opacity)
        {
            Collection<Rectangle> rects = Microsoft.Test.Controls.VisualTreeHelper.GetVisualChildren<Rectangle>(calendarDayButton);

            foreach (Rectangle rect in rects)
            {
                if (String.Compare(rect.Name, "SelectedBackground") == 0)
                {
                    if (rect.Opacity != opacity)
                    {
                        throw new TestValidationException("Fail: targetRect is not " + opacity);
                    }
                }
            }
        }

        #endregion
    } 
}
