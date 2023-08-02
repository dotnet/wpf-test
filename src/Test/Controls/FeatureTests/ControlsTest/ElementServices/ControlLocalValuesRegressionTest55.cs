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
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Verifies that the property system works correctly when there
    /// is a defered and coercedWithCurrentValue effectiveValue.
    /// </description>

    /// </summary>
    [Test(0, "ElementServices", "ControlLocalValuesRegressionTest55", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class ControlLocalValuesRegressionTest55 : XamlTest
    {
        private Button debugButton;
        private Button testButton;
        private Style theme1;
        private Style theme2;
        private Label label;

        #region Constructor

        public ControlLocalValuesRegressionTest55()
            : base(@"ControlLocalValuesRegressionTest55.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestControlLocalValues);
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

            label = (Label)RootElement.FindName("label");
            Assert.AssertTrue("Unable to find label from the resources", label != null);    

            testButton = (Button)RootElement.FindName("testButton");
            Assert.AssertTrue("Unable to find testButton from the resources", testButton != null);

            theme1 = (Style)RootElement.FindResource("Theme1");
            Assert.AssertTrue("Unable to find theme1 from the resources", theme1 != null);

            theme2 = (Style)RootElement.FindResource("Theme2");
            Assert.AssertTrue("Unable to find theme2 from the resources", theme2 != null);

            testButton.Click += ChangeTheme;        

            // uncomment to pause test and debug manually
            //Debug();

            LogComment("Setup for ResourceDictionaryRegressionTest56 was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            debugButton = null;
            label = null;
            testButton.Click -= ChangeTheme;
            testButton = null;
            theme1 = null;
            theme2 = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. Click the "Change Theme" button        
        /// 
        /// Verify no exception is thrown and test completes successfully.
        /// </summary>
        private TestResult TestControlLocalValues()
        {
            Status("TestControlLocalValues");

            LogComment("1. Click the \"Change Theme\" button");
            ChangeTheme(null, null);            
            QueueHelper.WaitTillQueueItemsProcessed();         

            //
            // Getting to this point without an exception means the test has passed
            //

            LogComment("TestControlLocalValues was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            label.Style = (label.Style == theme1) ? theme2 : theme1;
        }

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

    public class TestStrings : ObservableCollection<String>
    {
        public TestStrings()
        {
            Add("Red");
            Add("Green");
            Add("Blue");
        }
    }   
}
