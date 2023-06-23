using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
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
    /// Ensure popup panel shows up in the right location
    /// </summary>
    [Test(0, "Popup", TestCaseSecurityLevel.FullTrust, "PopupRegressionTest67")]
    public class PopupRegressionTest67 : XamlTest
    {
        private Panel popupPanel;

        public PopupRegressionTest67()
            : base(@"PopupRegressionTest67.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            popupPanel = (Panel)RootElement.FindName("popupPanel");
            if (popupPanel == null)
            {
                throw new TestValidationException("Popup Panel is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        /// <summary>
        /// Validating popupPanel.PointToScreen respects Popup�s HorizontalOffset and VerticalOffset
        /// </summary>
        /// <returns></returns>
        public TestResult RunTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            Assert.AssertEqual(String.Format("PopupPanel ActualWidth is not {0}", 10), 10.0, popupPanel.ActualWidth);
            Assert.AssertEqual(String.Format("PopupPanel ActualHeight is not {0}", 100), 100.0, popupPanel.ActualHeight);
            Assert.AssertEqual(String.Format("PopupPanel PointToScreen is not {0}", new Point(100, 100)), new Point(100, 100), popupPanel.PointToScreen(new Point()));

            return TestResult.Pass;
        }
    } 
}
