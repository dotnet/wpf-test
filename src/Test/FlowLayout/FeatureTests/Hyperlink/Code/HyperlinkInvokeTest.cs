// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing hyperlink invoke.   
    /// </summary>
    [Test(2, "Hyperlink", "HyperlinkDoClickTest", MethodName = "Run")]
    public class TestHyperlinkInvokeTest : AvalonTest
    {
        private NavigationWindow _navWin;
        private Hyperlink _hl;
        private Canvas _eRoot;
        private string _navFile = "SimpleNavigation.xaml";
        private bool _result;

        public TestHyperlinkInvokeTest()
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

            string uri = System.IO.Path.Combine(Environment.CurrentDirectory, _navFile);

            _hl = new Hyperlink();
            _hl.NavigateUri = new Uri(uri);
            _hl.FontSize = 20;
            _hl.Inlines.Clear();
            _hl.Inlines.Add(new Run("This is the content.  It needs to be long enough so that it wraps to more than one line."));
            _hl.Focusable = true;
            _hl.RequestNavigate += new RequestNavigateEventHandler(hl_RequestNavigate);

            para.Inlines.Add(_hl);
            _eRoot.Children.Add(tp);

            _navWin.Content = _eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 300;
            _navWin.Height = 300;

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
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            LogComment("Content rendered");

            //pause for a second and then begin test.  (This makes sure content has rendered first.)
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TestDoClick();

            return TestResult.Pass;
        }

        //Tests DoClick
        private void TestDoClick()
        {
            _hl.DoClick();
        }

        private TestResult VerifyTest()
        {
            LogComment("Verify Test...");

            string source = _navWin.Source.ToString();
            string[] file = source.Split('/');
            foreach (string s in file)
            {
                if (s == _navFile)
                {
                    _result = true;
                }
            }

            if (_result)
            {
                LogComment("Navigation has succeeded!  Test has passed!");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Test has failed!!");
                LogComment("NavigationWindow.Source after navigation: " + _navWin.Source.ToString());
                LogComment("Expected: " + _navFile);
                return TestResult.Fail;
            }
        }
    }
}
