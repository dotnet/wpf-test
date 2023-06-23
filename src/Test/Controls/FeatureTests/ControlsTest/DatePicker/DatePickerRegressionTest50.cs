using System;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// DatePickerRegressionTest50
    /// </description>
    /// </summary>
    [Test(1, "DatePicker", "DatePickerRegressionTest50")]
    public class DatePickerRegressionTest50 : XamlTest
    {
        #region Private Members

        private DatePicker datepicker;
        private Button button1;
        private Button button2;

        #endregion

        #region Public Members

        public DatePickerRegressionTest50()
            : base(@"DatePickerRegressionTest50.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        public TestResult Setup()
        {
            Status("Setup");

            datepicker = (DatePicker)RootElement.FindName("datepicker");
            button1 = (Button)RootElement.FindName("button1");
            button2 = (Button)RootElement.FindName("button2");

            // Set focus on the first button
            button1.Focus();

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            datepicker = null;
            button1 = null;
            button2 = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// 1, Ensure we can't tab to Focusable is false DatePicker
        /// 2, Ensure we can't type inside of Focusable is false DatePicker
        /// </summary>
        public TestResult Repro()
        {
            // Ensure we can't tab to Focusable is false DatePicker
            if (!button1.IsFocused)
            {
                throw new TestValidationException("Fail: the first button is not focused.");
            }

            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Tab);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            if (datepicker.IsFocused)
            {
                throw new TestValidationException("Fail: the datepicker is focused after tab.");
            }

            // Ensure we can't type inside of Focusable is false DatePicker
            DatePickerTextBox textbox = datepicker.Template.FindName("PART_TextBox", datepicker) as DatePickerTextBox;

            // Mouse click on DatePickerTextBox to try to set focus on it
            InputHelper.MouseClickCenter(textbox, System.Windows.Input.MouseButton.Left);

            // Try to type "abc" on DatePickerTextBox
            string inputString = "abc";
            Microsoft.Test.Input.Keyboard.Type(inputString);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            if (String.Compare(textbox.Text, inputString) == 0)
            {
                throw new TestValidationException("Fail: it's able to type text inside of DatePickerTextBox that has Focusable=false.");
            }

            return TestResult.Pass;
        }

        #endregion
    } 
}
