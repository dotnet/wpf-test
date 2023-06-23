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

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify that owners in the ResourceDictionary are removed when
    /// replaced through trigger actions.
    /// </description>

    /// </summary>
    [Test(0, "ResourceDictionary", "ResourceDictionaryRegressionTest56", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.0+,4.0Client+")] //Disable for 3.X since this is a bug fix for 4.0 RTM that wasn't backported.
    public class ResourceDictionaryRegressionTest56 : XamlTest
    {
        private Button debugButton;
        private Button testButton;
        private ResourceDictionary rd;

        #region Constructor

        public ResourceDictionaryRegressionTest56()
            : base(@"ResourceDictionaryRegressionTest56.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestTemplateTrigger);
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

            testButton = (Button)RootElement.FindName("btn_TestButton");
            Assert.AssertTrue("Unable to find testButton from the resources", testButton != null);

            rd = (ResourceDictionary)RootElement.FindResource("rd");
            Assert.AssertTrue("Unable to find rd from the resources", rd != null);

            testButton.MouseEnter += testButton_MouseEnter;
            testButton.MouseLeave += testButton_MouseLeave;

            //Debug();

            LogComment("Setup for ResourceDictionaryRegressionTest56 was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            debugButton = null;
            testButton.MouseEnter -= testButton_MouseEnter;
            testButton.MouseLeave -= testButton_MouseLeave;
            testButton = null;
            rd = null;
            return TestResult.Pass;
        }

        void testButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            LogComment("Correctly entered MouseLeave, count: " + DoCollectAndGetCount());
        }

        void testButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            LogComment("Correctly entered MouseEnter");
        }

        /// <summary>
        /// 1. Mouse over the button, move mouse away, mouse over several times
        /// 2. Do a garbage collect
        /// 3. if the owner count never goes down after ten tries then it has failed.
        /// </summary>
        private TestResult TestTemplateTrigger()
        {
            Status("TestTemplateTrigger");

            bool isSuccess = false;
            int maxOwnerCount = Int32.MinValue;
            int maxIterations = 5;
            while(maxIterations > 0 && !isSuccess)
            {
                LogComment("do mouse over");
                UserInput.MouseMove(testButton, (int)testButton.ActualWidth / 2, (int)testButton.ActualHeight / 2);
                this.WaitFor(500);

                LogComment("do mouse away");
                UserInput.MouseLeftClickCenter(debugButton);
                this.WaitFor(500);

                int count = DoCollectAndGetCount();
                LogComment(string.Format("owner count after collect: {0}", count));

                if (count > maxOwnerCount)
                {
                    maxOwnerCount = count;
                }
                else
                {
                    isSuccess = true;
                }
                maxIterations--;
            }

            if (!isSuccess)
            {
                throw new TestValidationException(string.Format(
                    "Owner count for the ResourceDictionary never went down in value after the multiple garbage collects. Final count: {0}",
                    maxOwnerCount));
            }

            LogComment("TestTemplateTrigger was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private int GetOwnerCount(ResourceDictionary rd)
        {
            int retVal = -1;

            IEnumerable initialFEs = GetOwnerFEs(rd);
            if (initialFEs != null)
            {
                int counter = 0;
                foreach (object fe in initialFEs)
                {
                    counter++;
                }
                retVal = counter;
            }
            return retVal;
        }

        private IEnumerable GetOwnerFEs(ResourceDictionary resourceDictionary)
        {
            return (IEnumerable)(typeof(ResourceDictionary).InvokeMember("_ownerFEs",
                                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                                    null, resourceDictionary, null));
        }

        private int DoCollectAndGetCount()
        {
            // Force GC
            for (int i = 0; i < 5; i++)
            {
                GC.Collect(GC.MaxGeneration);
                GC.WaitForPendingFinalizers();
            }

            return GetOwnerCount(rd);
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
}
