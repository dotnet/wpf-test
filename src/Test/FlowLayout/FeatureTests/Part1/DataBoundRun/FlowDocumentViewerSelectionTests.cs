// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Controls;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Logging;
using Mti = Microsoft.Test.Input;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Coverage for content selection that contain Runs with bound text source in FlowDocument viewers.  
    /// </summary>
    [Test(1, "Part1.DataBoundRun", "Bound Content in Selection ", MethodName = "Run")]
    public class FlowDocumentViewerSelectionTests : AvalonTest
    {
        private Window _testWindow;
        private string _viewerType;            
        private FrameworkElement _documentViewer;
        private DataBoundRunCommon _dataBoundRunCommon;
           
        [Variation("FlowDocumentReader", false)]
        [Variation("FlowDocumentPageViewer", false)]
        [Variation("FlowDocumentScrollViewer", false)]
        [Variation("FlowDocumentReader", true)]
        [Variation("FlowDocumentPageViewer", true)]
        [Variation("FlowDocumentScrollViewer", true)] 
        public FlowDocumentViewerSelectionTests(string viewerType, bool updateBindingSourceAfterSelection)
        {
            this._viewerType = viewerType;

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(SelectSomeBoundedContent);
            if (updateBindingSourceAfterSelection)
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
            _testWindow.Topmost = true;
            _testWindow.Title = DriverState.TestName;
            _dataBoundRunCommon = new DataBoundRunCommon(_testWindow);
            _documentViewer = _dataBoundRunCommon.CreateDocumentViewerWithBoundedRun(_viewerType, "This test verifies Selection in Runs with a bound Text property. ", " This is content following the bounded Run.");
            _testWindow.Content = _documentViewer;
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
        /// Make a selection in the FlowDocument that contains the bounded Run.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult SelectSomeBoundedContent()
        {           
            Mti.UserInput.MouseButton(_documentViewer, 10, 10, "Move");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            
            Mti.Input.SendMouseInput(0, 0, 0, Mti.SendMouseInputFlags.LeftDown);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            
            Mti.UserInput.MouseButton(_documentViewer, 500, 200, "Move");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            
            Mti.Input.SendMouseInput(0, 0, 0, Mti.SendMouseInputFlags.LeftUp);
                    
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that the bounded content is in the Selection.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {           
            TextSelection textSelection = _dataBoundRunCommon.GetDocumentSelection(_documentViewer);            
            if (!textSelection.Text.Contains(_testWindow.Title))
            {
                TestLog.Current.LogEvidence("Failed to find the content that was expected in the Bounded Run!");
                TestLog.Current.LogEvidence(string.Format("Looking for Selection containing: '{0}'", _testWindow.Title));
                TestLog.Current.LogEvidence(string.Format("Instead Selection contains: '{0}'", textSelection.Text));
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }            
        }        
    }
}
