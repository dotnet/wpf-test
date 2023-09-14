using System.Windows.Controls;
using System.Windows.Input;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Regression Test  Verify that Datepicker does not lose keyboard focus in the pop-up
    /// </summary>
    [Test(0, "DatePicker", "RegressionTest51", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DatePickerRegressionTest51 : XamlTest
    {
        private DatePicker datePicker;

        #region Constructor

        //Using xaml related to another test. This test does not require any special xaml so re-using existing one. 
        public DatePickerRegressionTest51() : base(@"DatePickerRegressionTest52.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestEscPopup);
            RunSteps += new TestStep(TestEscSecondtime);
        }
        
        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize setup for the tests. 
        /// </summary>
        /// <returns>True if all fine, false otherwise</returns>
        public TestResult Setup()
        {
            Status("Setup for DatePickerRegressionTest52");            
            datePicker = (DatePicker)RootElement.FindName("datePicker");
            Assert.AssertTrue("Unable to find calendar from the resources", datePicker != null);            
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            datePicker = null;
            return TestResult.Pass;
        }

        public TestResult TestEscPopup()
        {
            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datePicker);
            InputHelper.Click(tdatepicker.Button);
            DispatcherHelper.DoEvents();            
                        
            //Press Ctrl-Up once when Popup is down.             
            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftCtrl);
            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Up);
            Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftCtrl);
            DispatcherHelper.DoEvents();            

            //Now press Esc. Popup should go away
            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Escape);
            DispatcherHelper.DoEvents();            

            //Check if Pop goes away
            Assert.AssertTrue("DatePicker Pop up did not go away on pressing Escape", datePicker.IsDropDownOpen != true);
            return TestResult.Pass;

        }

        public TestResult TestEscSecondtime()
        {
            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datePicker);            
            datePicker.IsDropDownOpen = true;
            DispatcherHelper.DoEvents();            

            //Press Ctrl-Up once when Popup is down.             
            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftCtrl);
            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Up);
            Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftCtrl);
            DispatcherHelper.DoEvents();            

            //Close the Calendar            
            datePicker.IsDropDownOpen = false;
            DispatcherHelper.DoEvents();
            QueueHelper.WaitTillTimeout(System.TimeSpan.FromSeconds(2));

            //Re-open the Calendar second time
            datePicker.IsDropDownOpen = true;
            DispatcherHelper.DoEvents();            

            //Now press Esc. Popup should go away
            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Escape);
            DispatcherHelper.DoEvents();

            //Check if Pop goes away
            Assert.AssertTrue("DatePicker Pop up did not go away on pressing Escape, i.e. lost keyboard focus", datePicker.IsDropDownOpen != true);
            return TestResult.Pass;
        }
        #endregion
    }
}
