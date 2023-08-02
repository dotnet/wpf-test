// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Documents;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Part1.RegressionTest</area>
    /// <owner>Microsoft</owner>
    /// <priority>1</priority>
    /// <description>
    /// Regression coverage for Part1 Regression_Bug46.  Adorner for TextBox validation errors does not disappear when Expander closes.
    /// </description>
    /// </summary>
    [Test(1, "Part1.RegressionTests", "Binding Validation Adorner Bug", MethodName = "Run", TestParameters = "content=BindingValidationAdornerBug.xaml")]
    public class BindingValidationAdornerBug : AvalonTest
    {
        private Window _testWin;
        private Expander _testExpander;
        private TextBox _testTextBox;
        private const string badBindingInput = "5a5";

        public BindingValidationAdornerBug()
        {
            InitializeSteps += new TestStep(CreateWindow);
            InitializeSteps += new TestStep(GetTestElements);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(VerifyTest);
        }

        /// <summary>
        /// Create a Window and load with content.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult CreateWindow()
        {
            _testWin = new Window();
            _testWin.Content = (FrameworkElement)XamlReader.Load(File.OpenRead(DriverState.DriverParameters["content"].ToLowerInvariant()));
            _testWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        /// <summary>
        /// Retrieve test elements for the test from the Window's content.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult GetTestElements()
        {
            StackPanel sp = _testWin.Content as StackPanel;
            if (sp == null)
            {
                TestLog.Current.LogEvidence("Did not find the expected content at the Window root.");
                return TestResult.Fail;
            }

            _testExpander = sp.FindName("testExpander") as Expander;
            _testTextBox = sp.FindName("testTextBox") as TextBox;

            if (_testExpander == null || _testTextBox == null)
            {
                TestLog.Current.LogEvidence("Failed to find test elements critical to this test!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// Enter some invalid text into the TextBox and collapse the Expander
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTest()
        {
            Microsoft.Test.Input.Input.MoveToAndClick(_testTextBox);
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            _testTextBox.Text = badBindingInput;
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            _testExpander.IsExpanded = false;
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify the test by getting the adorners for the TextBox and seeing if any are still visible after the Expander has collapsed.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            AdornerLayer textBoxAdornerLayer = AdornerLayer.GetAdornerLayer(_testTextBox);
            if (textBoxAdornerLayer == null)
            {
                TestLog.Current.LogEvidence("Failed to find an AdornerLayer on the TextBox!");
                return TestResult.Fail;
            }

            Adorner[] textBoxAdorners = textBoxAdornerLayer.GetAdorners(_testTextBox);
            if (textBoxAdorners == null)
            {
                TestLog.Current.LogEvidence("Failed to find any adorners on the TextBox's AdornerLayer!");
                return TestResult.Fail;
            }

            foreach (Adorner textBoxAdorner in textBoxAdorners)
            {
                if (textBoxAdorner.IsVisible)
                {
                    TestLog.Current.LogEvidence("There should be no adorner layers visible on the TextBox because it they should be hidden by the Expander!");
                    return TestResult.Fail;
                }
            }
            return TestResult.Pass;
        }
    }
}
