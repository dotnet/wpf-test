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
    /// Regression Test   
    /// Verify DatePicker Calendar popup should be closed after press Escape key.
    /// </description>
    /// </summary>
    [Test(0, "DatePicker", "DatePickerRegressionTest49", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DatePickerRegressionTest49 : XamlTest
    {
        private DatePicker datePicker;

        #region Constructor

        public DatePickerRegressionTest49()
            : base(@"DatePickerRegressionTest49.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestPopup);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for DatePickerRegressionTest49");

            datePicker = (DatePicker)RootElement.FindName("datePicker");

            LogComment("Setup for DatePickerRegressionTest49 was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            datePicker = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// 1, Mouse click on DatePicker Calendar button to open Calendar popup; verify the popup is opened.
        /// 2, Press Escape key to close Calendar popup; verify the popup is closed.
        /// </summary>
        private TestResult TestPopup()
        {
            Status("TestPopup");

            Popup datePickerPopup = datePicker.Template.FindName("PART_Popup", datePicker) as Popup;

            if (datePickerPopup.IsOpen)
            {
                throw new TestValidationException("Fail: DatePicker Popup is opened.");
            }

            Button datePickerButton = datePicker.Template.FindName("PART_Button", datePicker) as Button;
            InputHelper.MouseClickButtonCenter(datePickerButton, ClickMode.Release, System.Windows.Input.MouseButton.Left);

            if (!datePickerPopup.IsOpen)
            {
                throw new TestValidationException("Fail: DatePicker Popup is not opened.");
            }

            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Escape);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            if (datePickerPopup.IsOpen)
            {
                throw new TestValidationException("Fail: DatePicker Popup is opened.");
            }

            LogComment("TestPopup was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps   
    }
}
