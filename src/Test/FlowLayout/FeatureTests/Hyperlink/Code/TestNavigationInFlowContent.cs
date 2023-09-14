// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Markup;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Navigation in FlowContent    
    /// </summary>
    [Test(2, "Hyperlink", "NavigationInFlowContent", MethodName = "Run")]
    public class TestNavigationInFlowContent : AvalonTest
    {       
        private NavigationWindow _navWin;
        private bool _result;
        private string _navFile = "SimpleNavigation.xaml";
        private Canvas _eRoot;
        private IDocumentPaginatorSource _paginator;
        
        public TestNavigationInFlowContent()
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
            _eRoot.Width = 400;
            _eRoot.Height = 400;

            _paginator = XamlReader.Load(File.OpenRead("HyperlinkInFlowContent.xaml")) as IDocumentPaginatorSource;
            _paginator.DocumentPaginator.PageSize = new Size(400, 400);

            MyDocumentPageView1 dpv = new MyDocumentPageView1();
            dpv.DocumentPaginator = _paginator.DocumentPaginator;
            dpv.AddContentToLogicalTree(_paginator);

            _eRoot.Children.Add(dpv);

            _navWin.Content = _eRoot;
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 800;
            _navWin.Height = 600;
            _navWin.ShowsNavigationUI = false;
            _navWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _navWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Content rendered");
            return ClickLink();
        }

        /// <summary>
        /// VerifyTest: Verifies the test result
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            Status("Verify Test");

            Status("Checking to see if navigation was successful");
            string source = _navWin.Source.ToString();
            string[] file = source.Split('/');
            foreach (string s in file)
            {
                if (s == _navFile) { _result = true; }
            }

            if (_result)
            {
                Status("Navigation has succeeded!  Test has passed!");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Test has failed!!");
                LogComment("NavigationWindow.Source after navigation = " + source);
                return TestResult.Fail;
            }
        }
       
        private TestResult ClickLink()
        {
            Status("Invoking Hyperlink...");
            Hyperlink hl = LogicalTreeHelper.FindLogicalNode(_paginator as FlowDocument, "hlink") as Hyperlink;
            hl.DoClick();
            return TestResult.Pass;
        }

    }

    public class MyDocumentPageView1 : DocumentPageView
    {
        public void AddContentToLogicalTree(object content)
        {
            this.AddLogicalChild((FrameworkContentElement)content);
        }
    }
}
