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
    ///Regression Test  Verify that on clicking DatePicker directly, textbox is editable
    ///</summary>
    [Test(0, "DatePicker", "RegressionTest53", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DatePickerRegressionTest53 : XamlTest
    {        
        private DatePicker datePicker;
        private RichTextBox richTextBox;

        #region Constructor

        //This test refers to DatePickerRegressionTest52.xaml since there are no special requirements and one less xaml to worry about.
        public DatePickerRegressionTest53() : base(@"DatePickerRegressionTest52.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestEditDatePicker);
        }
        
        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize setup for the tests. 
        /// </summary>
        /// <returns>True if all fine, false otherwise</returns>
        public TestResult Setup()
        {
            Status("Setup for DatePickerRegressionTest53");            
            datePicker = (DatePicker)RootElement.FindName("datePicker");
            Assert.AssertTrue("Unable to find datepicker from the resources", datePicker != null);

            richTextBox = (RichTextBox)RootElement.FindName("richTextBox");
            Assert.AssertTrue("Unable to find richtextbox from the resources", richTextBox != null);

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            datePicker = null;
            richTextBox = null;
            return TestResult.Pass;
        }

        public TestResult TestEditDatePicker()
        {
            TemplatedDatePicker tdatePicker = new TemplatedDatePicker(datePicker);

            InputHelper.Click(tdatePicker.TextBox);
            InputHelper.PressKey(System.Windows.Input.Key.T);
            QueueHelper.WaitTillQueueItemsProcessed();
            Assert.AssertTrue("DatePicker textbox should be editable when clicked", tdatePicker.TextBox.Text == "t");
            return TestResult.Pass;
        }

        #endregion


    }
}
