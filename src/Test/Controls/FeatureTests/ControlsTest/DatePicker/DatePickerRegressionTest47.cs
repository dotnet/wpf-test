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
    /// internally.
    /// </description>
    /// </summary>
    [Test(0, "DatePicker", "DatePickerRegressionTest47", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DatePickerRegressionTest47 : XamlTest
    {
        private DatePicker datepicker;
        private CheckBox checkbox1;
        private CheckBox checkbox2;

        #region Constructor

        public DatePickerRegressionTest47()
            : base(@"DatePickerRegressionTest47.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for DatePickerRegressionTest47");

            datepicker = (DatePicker)RootElement.FindName("datepicker");
            checkbox1 = (CheckBox)RootElement.FindName("checkbox1");
            checkbox2 = (CheckBox)RootElement.FindName("checkbox2");

            LogComment("Setup for DatePickerRegressionTest47 was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            datepicker = null;
            checkbox1 = null;
            checkbox2 = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure datepicker's popup doesn't open on disabled state
        /// </summary>
        private TestResult Repro()
        {
            // Disable datepicker
            checkbox1.IsChecked = false;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            if (datepicker.IsEnabled != false)
            {
                throw new TestValidationException("Fail: datepicker is enabled after made IsEnabled checkbox to false.");
            }

            // Try to open datepicker popup
            checkbox2.IsChecked = true;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            Popup datePickerPopup = datepicker.Template.FindName("PART_Popup", datepicker) as Popup;

            if (datePickerPopup.IsOpen)
            {
                throw new TestValidationException("Fail: DatePicker's Popup is opened.");
            }

            return TestResult.Pass;
        }                

        #endregion Test Steps   
    }
}
