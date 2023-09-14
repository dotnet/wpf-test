using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Avalon.Test.ComponentModel;
using System.Windows.Input;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Verify that no exception is thrown when getting a coerced current value
    /// that was a deferred reference.
    /// </description>

    /// </summary>
    [Test(0, "ElementServices", "ControlLocalValuesRegressionTest54", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class ControlLocalValuesRegressionTest54 : XamlTest
    {
        private Button debugButton;
        private ComboBox testComboBox;

        #region Constructor

        public ControlLocalValuesRegressionTest54()
            : base(@"ControlLocalValuesRegressionTest54.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestSelector);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for ResourceDictionaryRegressionTest56");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            testComboBox = (ComboBox)RootElement.FindName("testComboBox");
            Assert.AssertTrue("Unable to find testComboBox from the resources", testComboBox != null);

			// uncomment to pause test and debug manually
            //Debug();

            LogComment("Setup for ResourceDictionaryRegressionTest56 was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            debugButton = null;
            testComboBox = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. Open the combobox and select one of the items
        /// 2. Open the combobox again and try to select an item by typing with the keyboard
        /// 
        /// Verify typing works as expected.
        /// </summary>
        private TestResult TestSelector()
        {
            Status("TestSelector");

            LogComment("open the dropdown");
            UserInput.MouseLeftClickCenter(testComboBox);
            QueueHelper.WaitTillQueueItemsProcessed();

            Assert.AssertTrue(string.Format(
                "testComboBox.SelectedIndex should have been updated to index 0. Actual: {0}", testComboBox.SelectedIndex),
                testComboBox.SelectedIndex == -1);
            
            LogComment("select the first item");            
            ComboBoxItem comboBoxItem = (ComboBoxItem)testComboBox.ItemContainerGenerator.ContainerFromIndex(0);
            testComboBox.SelectedItem = comboBoxItem;
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("close the dropdown");
            testComboBox.IsDropDownOpen = false;
            QueueHelper.WaitTillQueueItemsProcessed();
                        
            // verify the selected index changed accordingly
            Assert.AssertTrue(string.Format(
                "testComboBox.SelectedIndex should have been updated to index 0. Actual: {0}", testComboBox.SelectedIndex), 
                testComboBox.SelectedIndex == 0);

            LogComment("open the dropdown again");
            UserInput.MouseLeftClickCenter(testComboBox);
            QueueHelper.WaitTillQueueItemsProcessed();

            WaitFor(500);
                        
            LogComment("select the value at index 1");
            UserInput.KeyPress(System.Windows.Input.Key.Down.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("press enter to close the dropdown");
            UserInput.KeyPress(System.Windows.Input.Key.Enter.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            // verify the selected index changed accordingly
            Assert.AssertTrue(string.Format(
                "testComboBox.SelectedIndex should have been updated to index 1. Actual: {0}", testComboBox.SelectedIndex),
                testComboBox.SelectedIndex == 1);

            //
            // Getting to this point without an exception means the test has passed
            //

            LogComment("TestSelector was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps   
   
        #region Helpers

        private void Debug()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            debugButton.MouseLeftButtonDown += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }        

        #endregion Helpers
    }
}
