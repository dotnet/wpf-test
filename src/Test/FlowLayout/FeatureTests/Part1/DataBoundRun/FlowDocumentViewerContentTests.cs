// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Coverage for Runs with bound text source in FlowDocument viewers.   
    /// </summary>
    [Test(1, "Part1.DataBoundRun", "Bound Content in Document ", MethodName = "Run")]
    public class FlowDocumentViewerContentTests : AvalonTest
    {
        private Window _testWindow;
        private string _viewerType;
        private DataBoundRunCommon _dataBoundRunCommon;
           
        [Variation("FlowDocumentReader", false)]
        [Variation("FlowDocumentPageViewer", false)]
        [Variation("FlowDocumentScrollViewer", false)]
        [Variation("FlowDocumentReader", true)]
        [Variation("FlowDocumentPageViewer", true)]
        [Variation("FlowDocumentScrollViewer", true)] 
        public FlowDocumentViewerContentTests(string viewerType, bool updateBindingSource)
        {
            this._viewerType = viewerType;

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            if (updateBindingSource)
            {
                RunSteps += new TestStep(UpdateBindingSource);
            }
            RunSteps += new TestStep(VerifyTest);
        }

        /// <summary>
        /// Creates content for the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _testWindow = new Window();
            _testWindow.Title = DriverState.TestName;
            _dataBoundRunCommon = new DataBoundRunCommon(_testWindow);
            _testWindow.Content = _dataBoundRunCommon.CreateDocumentViewerWithBoundedRun(_viewerType, "This test verifies that content from a Run with a bound Text property shows as expected. ", " This is content after the Bounded Run.");
            _testWindow.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWindow.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// Change the content of the Window title so that the source of the bounded Run is updated.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult UpdateBindingSource()
        {
            _testWindow.Title = "Window Title has been updated";
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that the bounded content is in the Document.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            Paragraph paragraphWithBoundedRun = _dataBoundRunCommon.DocumentParagraph;           
            foreach (Inline inline in paragraphWithBoundedRun.Inlines)
            {
                TextRange textRange = new TextRange(inline.ContentStart, inline.ContentEnd);
                TestLog.Current.LogStatus(string.Format("Found Inline with text: '{0}'", textRange.Text));

                if (textRange.Text.Contains(_testWindow.Title))
                {
                    return TestResult.Pass;
                }
            }             
            TestLog.Current.LogEvidence("Failed to find the content that was expected in the Bounded Run!");
            TestLog.Current.LogEvidence(string.Format("Looking for Inline containing: '{0}'", _testWindow.Title));

            return TestResult.Fail;                     
        }        
    }
}
