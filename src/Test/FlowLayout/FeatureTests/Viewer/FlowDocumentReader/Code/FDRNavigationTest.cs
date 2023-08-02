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
using Microsoft.Test.Layout;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Tests navigation in FlowDocumentReader by clicking hyperlink and using BringintoView method.   
    /// </summary>
    [Test(0, "Viewer.FlowDocumentReader", "NavigationTest", MethodName = "Run")]
    public class NavigationTest : AvalonTest
    {        
        private FlowDocumentReader _viewer;
        private FlowDocument _fd;        
        private int _currentPageNumber;
        private string _contentFile;
        private string _testToRun;
        private string _viewingMode;
        private NavigationWindow _navWin;

        [Variation("reptiles_Flow.xaml", "NavigateHyperlink", "Page")]
        [Variation("reptiles_Flow.xaml", "NavigateHyperlink", "TwoPage")]
        [Variation("reptiles_Flow.xaml", "NavigateHyperlink", "Scroll", Keywords = "MicroSuite")]
        [Variation("reptiles_Flow.xaml", "BringIntoView", "Page")]
        [Variation("reptiles_Flow.xaml", "BringIntoView", "TwoPage")]
        [Variation("reptiles_Flow.xaml", "BringIntoView", "Scroll")]
        public NavigationTest(string contentFile, string testToRun, string viewingMode)
            : base()
        {
            this._contentFile = contentFile;
            this._testToRun = testToRun;
            this._viewingMode = viewingMode;

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }
       
        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentReader and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            Status("Initialize");
            _navWin = new NavigationWindow();
            _navWin.Width = 800;
            _navWin.Height = 600;
            
            _fd = (FlowDocument)XamlReader.Load(File.OpenRead(_contentFile));
            _viewer = new FlowDocumentReader();
            _viewer.Document = _fd;
            _navWin.Content = _viewer;

            Status("Setting the FlowDocumentReader viewmode");
            switch (_viewingMode.ToLower(CultureInfo.InvariantCulture))
            {
                case "page":
                    _viewer.ViewingMode = FlowDocumentReaderViewingMode.Page;
                    break;
                case "twopage":
                    _viewer.ViewingMode = FlowDocumentReaderViewingMode.TwoPage;
                    break;
                case "scroll":
                    _viewer.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
                    break;
                default:
                    TestLog.Current.LogEvidence("Error !!! SettingViewMode: Unexpected failure to match the argument. ");
                    return TestResult.Fail;
            }
            
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
            switch (_testToRun.ToLower(CultureInfo.InvariantCulture))
            {
                case "navigatehyperlink":
                    Hyperlink h1 = LogicalTreeHelper.FindLogicalNode(_viewer, "hlink") as Hyperlink;
                    h1.DoClick();
                    if ((_viewer.ViewingMode == FlowDocumentReaderViewingMode.Page) || (_viewer.ViewingMode == FlowDocumentReaderViewingMode.TwoPage))
                    {
                        ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetPageNumberCompleted += new GetPageNumberCompletedEventHandler(FDRTest_GetPageNumberCompleted);
                        Paragraph p1 = LogicalTreeHelper.FindLogicalNode(_viewer, "physChar") as Paragraph;
                        ContentPosition cp = ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetObjectPosition(p1);
                        ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetPageNumberAsync(cp, this);
                        WaitForPriority(DispatcherPriority.ApplicationIdle);

                        if (_viewer.ViewingMode == FlowDocumentReaderViewingMode.Page)
                        {
                            if ((_viewer.PageNumber) - 1 == _currentPageNumber)
                            {
                                Status("Test Passed, Testcase has navigated to the right page. ExpectedPage : " + ((_viewer.PageNumber) - 1) + " ActualPage: " + _currentPageNumber);
                                return TestResult.Pass;
                            }
                            else
                            {
                                TestLog.Current.LogEvidence("Test Failed, Testcase failed to navigate to the correct page.ExpectedPage : " + ((_viewer.PageNumber) - 1) + " ActualPage: " + _currentPageNumber);
                                return TestResult.Fail;
                            }
                        }
                        else if (_viewer.ViewingMode == FlowDocumentReaderViewingMode.TwoPage)
                        {
                            if ((_viewer.PageNumber) == _currentPageNumber)
                            {
                                Status("Test Passed, Testcase has navigated to the right page.ExpectedPage : " + (_viewer.PageNumber) + " ActualPage: " + _currentPageNumber);
                                return TestResult.Pass;
                            }
                            else
                            {
                                TestLog.Current.LogEvidence("Test Failed, Testcase failed to navigate to the correct page.ExpectedPage : " + (_viewer.PageNumber) + " ActualPage: " + _currentPageNumber);
                                return TestResult.Fail;
                            }
                        }
                    }
                    else
                    {
                        return VerifyScrollViewerOffset();
                    }
                    break;
                case "bringintoview":
                    Paragraph para1 = LogicalTreeHelper.FindLogicalNode(_viewer, "tablepara") as Paragraph;
                    para1.BringIntoView();
                    if ((_viewer.ViewingMode == FlowDocumentReaderViewingMode.Page) || (_viewer.ViewingMode == FlowDocumentReaderViewingMode.TwoPage))
                    {
                        ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetPageNumberCompleted += new GetPageNumberCompletedEventHandler(FDRTest_GetPageNumberCompleted);
                        ContentPosition cp = ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetObjectPosition(para1);
                        ((DynamicDocumentPaginator)(((IDocumentPaginatorSource)_fd).DocumentPaginator)).GetPageNumberAsync(cp, this);
                        WaitForPriority(DispatcherPriority.ApplicationIdle);
                        if (_viewer.ViewingMode == FlowDocumentReaderViewingMode.Page)
                        {
                            if ((_viewer.PageNumber) - 1 == _currentPageNumber)
                            {
                                Status("Test Passed, Table in the testcase was brought to view correctly");
                                return TestResult.Pass;
                            }
                            else
                            {                                
                                TestLog.Current.LogEvidence(string.Format("Failed BringIntoView. ExpectedPage : {0} ActualPage: {1}", (_viewer.PageNumber - 1), _currentPageNumber));
                                return TestResult.Fail;
                            }
                        }
                        else if (_viewer.ViewingMode == FlowDocumentReaderViewingMode.TwoPage)
                        {
                            if (_viewer.PageNumber == _currentPageNumber)
                            {
                                Status("Test Passed, Table in the testcase was brought to view correctly");
                                return TestResult.Pass;
                            }
                            else
                            {                               
                                TestLog.Current.LogEvidence(string.Format("Failed BringIntoView. ExpectedPage : {0} ActualPage: {1}", _viewer.PageNumber, _currentPageNumber));
                                return TestResult.Fail;
                            }
                        }
                    }
                    else
                    {
                        return VerifyScrollViewerOffset();
                    }
                    break;
            }
            return TestResult.Fail;     
        }       

        private void FDRTest_GetPageNumberCompleted(object sender, GetPageNumberCompletedEventArgs e)
        {
            if (e.UserState != this) return;
            _currentPageNumber = e.PageNumber;
        }      

        private TestResult VerifyScrollViewerOffset()
        {
            WaitFor(500);

            int expectedValueMax = 5000;
            int expectedValueMin = 3000;

            ScrollViewer sv = (ScrollViewer)LayoutUtility.GetChildFromVisualTree(_viewer, typeof(ScrollViewer));
            if (sv != null)
            {
                if ((sv.VerticalOffset > expectedValueMin) && (sv.VerticalOffset < expectedValueMax))
                {
                    return TestResult.Pass;
                }
                else
                {
                    TestLog.Current.LogEvidence(string.Format("Expected a ScrollViewer vertical offset between {0} and {1}, actual value: {2}", expectedValueMin, expectedValueMax, sv.VerticalOffset));
                    return TestResult.Fail;
                }
            }
            else
            {
                TestLog.Current.LogEvidence("Did not find a ScrollViewer in the VisualTree.  Cannot verify offset.");
                return TestResult.Fail;
            }
        }
    }
}
