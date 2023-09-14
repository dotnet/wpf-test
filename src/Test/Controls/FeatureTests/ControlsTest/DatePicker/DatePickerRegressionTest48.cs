using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test 
    /// </description>
    /// </summary>
    [Test(0, "DatePicker", "DatePickerRegressionTest48", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DatePickerRegressionTest48 : XamlTest
    {
        private Button button;
        private DatePicker datepicker;
        private DatePickerTextBox textbox;

        #region Constructor

        public DatePickerRegressionTest48()
            : base(@"DatePickerRegressionTest48.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Test);
        }

        #endregion
        
        #region Test Steps

        public TestResult Setup()
        {
            Status("Setup specific for DatePickerRegressionTest48");

            button = (Button)RootElement.FindName("button");
            if (button == null)
            {
                throw new TestValidationException("Fail: button is null.");
            }

            datepicker = (DatePicker)RootElement.FindName("datePicker");
            if (datepicker == null)
            {
                throw new TestValidationException("Fail: datePicker is null.");
            }

            textbox = Microsoft.Test.Controls.VisualTreeHelper.GetVisualChild<DatePickerTextBox, DatePicker>(datepicker);

            LogComment("Setup for DatePickerRegressionTest48 was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            button = null;
            datepicker = null;
            textbox = null;
            return TestResult.Pass;
        }

        private TestResult Test()
        {
            Status("Test");

            datepicker.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            // Ensure both DatePicker and DatePickerTextBox IsKeyboardFocusWithin is true
            if (!datepicker.IsKeyboardFocusWithin)
            {
                throw new TestValidationException("Fail: Datepicker IsKeyboardFocusWithin is false.");
            }

            if (!textbox.IsKeyboardFocusWithin)
            {
                throw new TestValidationException("Fail: DatePickerTextBox IsKeyboardFocusWithin is false.");
            }

            // Ensure DatePicker.IsFocused is false
            if (datepicker.IsFocused)
            {
                throw new TestValidationException("Fail: Datepicker IsFocused is true.");
            }

            // Ensure DatePickerTextBox.IsFocused is true
            if (!textbox.IsFocused)
            {
                throw new TestValidationException("Fail: DatePickerTextBox IsFocused is false.");
            }

            return TestResult.Pass;
        }                

        #endregion Test Steps   
    }
}
