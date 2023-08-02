using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System;
using Microsoft.Test.Input;
using Avalon.Test.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Verify that the binding on ComboBox.TextProperty is not lost
    /// when SelectedItemProperty is set before TextProperty.
    /// </description>

    /// </summary>
    [Test(0, "ComboBox", "ComboBoxRegressionTest15", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class ComboBoxRegressionTest15 : RegressionXamlTest
    {
        private TestDataClass testDataClass;        
        private TextBlock textBlock;
        private ContentPresenter contentPresenter;        

        #region Constructor

        public ComboBoxRegressionTest15()
            : base(@"ComboBoxRegressionTest15.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestTextBinding);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            Status("Setup specific for ComboBoxRegressionTest15");

            testDataClass = new TestDataClass();
            RootElement.DataContext = testDataClass;

            textBlock = (TextBlock)RootElement.FindName("textBlock");
            Assert.AssertTrue("Unable to find textBlock from the resources", textBlock != null);

            contentPresenter = (ContentPresenter)RootElement.FindName("contentPresenter");
            Assert.AssertTrue("Unable to find contentPresenter from the resources", contentPresenter != null);

            QueueHelper.WaitTillQueueItemsProcessed();

            // Uncomment for ad-hoc debugging
            //base.Setup();

            LogComment("Setup for ComboBoxRegressionTest15 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            textBlock = null;
            contentPresenter = null;
            return base.CleanUp();
        }
        /// <summary>
        /// 1. select the combobox and type 'abcd'
        /// 2. then press TAB
        /// 
        /// Verify that the value 'Current' has updated to 'abcd'.
        /// </summary>
        private TestResult TestTextBinding()
        {
            Status("TestTextBinding");

            string prevText = textBlock.Text;

            UserInput.MouseLeftClickCenter(contentPresenter);
            QueueHelper.WaitTillQueueItemsProcessed();

            // type in the text
            UserInput.KeyPress(System.Windows.Input.Key.A.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.KeyPress(System.Windows.Input.Key.B.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.KeyPress(System.Windows.Input.Key.C.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.KeyPress(System.Windows.Input.Key.D.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            // press tab
            UserInput.KeyPress(System.Windows.Input.Key.Tab, true);
            QueueHelper.WaitTillQueueItemsProcessed();

            // verify the behavior
            if (textBlock.Text == prevText)
            {
                throw new TestValidationException(string.Format(
                    "TextBlock.Text binding did not update.  Expected: abcd, Actual: {0}",
                    textBlock.Text));
            }           

            LogComment("TestTextBinding was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps       
    }

    public class TestDataClass
    {
        private ObservableCollection<string> list = new ObservableCollection<string>();

        public TestDataClass()
        {
            list.Add("abc");
            list.Add("abcd");

            Current = "abc";
        }

        public string Current { get; set; }

        public ObservableCollection<string> Items
        {
            get { return list; }
        }
    }

    
}
