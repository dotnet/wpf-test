using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Regression Test 
    /// Ensure opening combobox without crash
    /// </summary>
    [Test(0, "Popup", TestCaseSecurityLevel.FullTrust, "PopupRegressionTest66")]
    public class PopupRegressionTest66 : XamlTest
    {
        private ComboBox comboBox;

        public PopupRegressionTest66()
            : base(@"PopupRegressionTest66.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            comboBox = (ComboBox)RootElement.FindName("comboBox");
            if (comboBox == null)
            {
                throw new TestValidationException("ComboBox is null");
            }

            Window.Height = 350;
            Window.Width = 525;

            // Move half of the window off the screen on the left side 
            Window.Left = -(Window.ActualWidth / 2);

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        /// <summary>
        /// Validating no crash after multiple clicks on a combobox when window left half is off screen
        /// </summary>
        /// <returns></returns>
        public TestResult RunTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            InputHelper.MouseMoveToElementCenter(comboBox);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            for (int i = 0; i < 4; i++)
            {
                Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
            }

            return TestResult.Pass;
        }
    } 
}
