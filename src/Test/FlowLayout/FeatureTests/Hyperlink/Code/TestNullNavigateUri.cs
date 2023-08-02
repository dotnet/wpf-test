// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing navigate to a null uri   
    /// </summary>
    [Test(3, "Hyperlink", "NullNavigateUri", MethodName="Run")]
    public class TestNullNavigateUri : AvalonTest
    {       
        private NavigationWindow _navWin;
        private Hyperlink _hl;
        private Canvas _eRoot;
                
        public TestNullNavigateUri()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
            RunSteps += new TestStep(VerifyTest);           
        }

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _navWin = new NavigationWindow();
            
            Status("Initialize");
            _eRoot = new Canvas();
            _eRoot.Height = 160;
            _eRoot.Width = 160;
            _eRoot.Background = Brushes.Tan;

            Paragraph para = new Paragraph();

            FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
            tp.Document = new FlowDocument(para);
            tp.Height = 160;
            tp.Width = 160;

            _hl = new Hyperlink();
            _hl.NavigateUri = null;
            _hl.FontSize = 20;
            _hl.Inlines.Clear();
            _hl.Inlines.Add(new Run("This is the content.  It needs to be long enough so that it wraps to more than one line."));
            _hl.RequestNavigate += new RequestNavigateEventHandler(hl_RequestNavigate);

            para.Inlines.Add(_hl);
            _eRoot.Children.Add(tp);

            _navWin.Content = _eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 800;
            _navWin.Height = 600;
            _navWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _navWin.Close();
            return TestResult.Pass;
        }

        private void hl_RequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            LogComment("RequestNavigate event hit...");
            LogComment("This event should not be hit b/c navigation should not happen!!");
            Log.Result = TestResult.Fail;
        }
       
        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            LogComment("Content rendered");

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            ClickLink();

            if (Log.Result != TestResult.Fail)
                return TestResult.Pass;
            else
                return TestResult.Fail;
        }

        /// <summary>
        /// VerifyTest: Verifies the test result
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            LogComment("Verify Test...");

            //Verify that navigation did not happen).
            if (_navWin.Source == null)
            {
                LogComment("Navigation did not happen (as expected)");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Navigation should not have happened!!");
                return TestResult.Fail;
            }                       
        }
       
        private void ClickLink()
        {
            LogComment("Click the Hyperlink...");
            UserInput.MouseButton(_eRoot, 50, 50, "LeftDown");
            UserInput.MouseButton(_eRoot, 50, 50, "LeftUp");
        }     
    }
}
