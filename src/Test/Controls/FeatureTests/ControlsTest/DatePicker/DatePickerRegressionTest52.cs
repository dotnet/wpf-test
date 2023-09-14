using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Input;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Controls
{
    ///<summary>
    ///Regression Test  Verify that Datepicker gets focus using keyboard
    ///</summary>
    [Test(0, "DatePicker", "RegressionTest52", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DatePickerRegressionTest52 : XamlTest
    {        
        private DatePicker datePicker;
        private RichTextBox richTextBox;

        #region Constructor

        public DatePickerRegressionTest52() : base(@"DatePickerRegressionTest52.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestFocusOnTab);            
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

            richTextBox = (RichTextBox)RootElement.FindName("richTextBox");
            Assert.AssertTrue("Unable to find calendar from the resources", richTextBox != null);

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            datePicker = null;
            richTextBox = null;
            return TestResult.Pass;
        }

        public TestResult TestFocusOnTab()
        {
            TemplatedDatePicker tdatePicker = new TemplatedDatePicker(datePicker);            

            InputHelper.Click(richTextBox);            
            InputHelper.PressKey(System.Windows.Input.Key.Tab);
            QueueHelper.WaitTillQueueItemsProcessed();
            Assert.AssertTrue("DatePicker did not get focus", tdatePicker.TextBox.IsKeyboardFocused != false);            
            return TestResult.Pass;
        }        

        #endregion


    }
}
