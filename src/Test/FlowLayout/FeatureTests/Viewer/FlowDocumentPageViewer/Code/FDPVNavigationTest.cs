// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Navigation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Viewer.FlowDocumentPageViewer</area>
    /// <owner>Microsoft</owner>
    /// <priority>2</priority>
    /// <description>
    /// Testing navigation in FlowDocumentPageViewer by clicking hyperlink and using bringintoView property.
    /// </description>
    /// </summary>
    [Test(0, "Viewer.FlowDocumentPageViewer", "FDPVNavigationTest", MethodName = "Run")]
    public class FDPVNavigationTest : WindowTest
    {
        private FlowDocumentPageViewer _viewer;
        private FlowDocument _fd;
        private NavigationWindow _navWin;
        private int _currentPageNumber;
        private string _inputXaml;
        private string _inputString = "";

        [Variation("reptiles_Flow.xaml", "NavigateHyperlink")]
        [Variation("reptiles_Flow.xaml", "BringIntoView")]
        public FDPVNavigationTest(string xamlFile, string testValue)
            : base()
        {
            _inputXaml = xamlFile;
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            //Hide the parent window, so it won't overlay the Animation window.
            Window.Visibility = Visibility.Hidden;
            _navWin = new NavigationWindow();

            Status("Initialize ....");
            _fd = (FlowDocument)XamlReader.Load(File.OpenRead(_inputXaml));
            _viewer = new FlowDocumentPageViewer();
            _viewer.Document = _fd;
            _navWin.Content = _viewer;

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
            ContentPosition cp;
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "navigatehyperlink":
                    Hyperlink h1 = LogicalTreeHelper.FindLogicalNode(_viewer, "hlink") as Hyperlink;
                    h1.DoClick();

                    ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetPageNumberCompleted += new GetPageNumberCompletedEventHandler(FDRTest_GetPageNumberCompleted);
                    Paragraph p1 = LogicalTreeHelper.FindLogicalNode(_viewer, "physChar") as Paragraph;
                    cp = ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetObjectPosition(p1);
                    ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetPageNumberAsync(cp, this);
                    WaitForPriority(DispatcherPriority.ApplicationIdle);

                    if ((_viewer.MasterPageNumber) - 1 == _currentPageNumber)
                    {
                        LogComment("Test Passed, Testcase has navigated to the right page. ExpectedPage : " + ((_viewer.MasterPageNumber) - 1) + " ActualPage: " + _currentPageNumber);
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test Failed, Testcase failed to navigate to the correct page.ExpectedPage : " + ((_viewer.MasterPageNumber) - 1) + " ActualPage: " + _currentPageNumber);
                        Log.Result = TestResult.Fail;
                    }
                    break;

                case "bringintoview":
                    Paragraph para1 = LogicalTreeHelper.FindLogicalNode(_viewer, "tablepara") as Paragraph;
                    para1.BringIntoView();

                    ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetPageNumberCompleted += new GetPageNumberCompletedEventHandler(FDRTest_GetPageNumberCompleted);
                    cp = ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetObjectPosition(para1);
                    ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetPageNumberAsync(cp, this);
                    WaitForPriority(DispatcherPriority.ApplicationIdle);

                    if ((_viewer.MasterPageNumber) - 1 == _currentPageNumber)
                    {
                        LogComment("Test Passed, Table in the testcase was brought to view correctly");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test Failed, Testcase faield to BringIntoView");
                        Log.Result = TestResult.Fail;
                    }
                    break;

                default:
                    Status("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private void FDRTest_GetPageNumberCompleted(object sender, GetPageNumberCompletedEventArgs e)
        {
            if (e.UserState != this) return;
            _currentPageNumber = e.PageNumber;
        }
    }
}

