using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test for NullReferenceException in VirtualizingStackPanel
    /// </description>

    /// </summary>
    [Test(0, "ComboBox", "ComboBoxRegressionTest16", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class ComboBoxRegressionTest16 : RegressionXamlTest
    {
        #region private

        private ComboBox comboBox;

        #endregion

        #region Constructor

        public ComboBoxRegressionTest16()
            : base(@"ComboBoxRegressionTest16.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Repro);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            Status("Setup specific for ComboBoxRegressionTest16");

            comboBox = (ComboBox)RootElement.FindName("cmb");
            Assert.AssertTrue("Unable to find ComboBox from the resources", comboBox != null);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup for ComboBoxRegressionTest16 was successful");
            return TestResult.Pass;
        }

        private TestResult Repro()
        {
            Status("Repro");
            comboBox.Focus();
            TestResult result = TestResult.Fail;
            QueueHelper.WaitTillQueueItemsProcessed();

            try
            {
                LogComment("Press key P to select the combobox item.");
                UserInput.KeyPress(System.Windows.Input.Key.P.ToString());
                QueueHelper.WaitTillQueueItemsProcessed();

                ComboBoxItem item = comboBox.SelectedValue as ComboBoxItem;
                Assert.AssertTrue("The ComboBox's selected item is null.", item != null);
                Assert.AssertEqual("The ComboBox's selected Value is wrong. ", "P", item.Content.ToString());

                LogComment("Repro was successful");
                result = TestResult.Pass;
            }
            catch (NullReferenceException ex)
            {
                LogComment("Case failed due to bug : NullReferenceException in VirtualizingStackPanel\n" + ex.ToString());
            }
            catch (Exception ex)
            {
                LogComment("Unknown exceptions:" + ex.ToString());
            }

            return result;
        }

        #endregion Test Steps
    }

}
