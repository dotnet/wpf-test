// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Markup;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing various pagination apis.   
    /// </summary>
    [Test(2, "Pagination", "PaginationTest", MethodName = "Run", Timeout = 20)]
    public class PaginationTests : AvalonTest
    {
        private Window _testWin;
        private FlowDocument _fd;
        private string _testName;                                   
        private int _paginationProgressCount = 0;
        private ContentPosition _ecp;
        private ContentPosition _dummyContentPosition;
        private object _paginatorUserState = new object();
        private bool _pageDestroyed = false;        
        private double _pageHeight;
        private double _pageWidth;
        private bool _isBackgroundPaginationEnabled;
        private Size _iDocPagPageSize;       
        private int _pageNumber;      
        private DispatcherFrame _frame;                
        private IDocumentPaginatorSource _iDocPag;
        private DocumentPageView _docPagVw;        
        private Paragraph _boxParent;
        private Canvas _box;        

        [Variation("Property", 0, true)]
        [Variation("Property", 0, false)]
        [Variation("GetPage", 0, true)]
        [Variation("GetPageAsync", 3, true)]
        [Variation("GetPageAsync", 3, false)]      
        [Variation("GetPageNumber", 8, true)]
        [Variation("GetPageNumber", 8, false)]
        [Variation("GetPageNumberAsync", 8, true)]
        [Variation("GetPageNumberAsync", 8, false)]     
        [Variation("GetObjectPosition", 0, true)]
        [Variation("GetObjectPosition", 0, false)]
        [Variation("ComputePageCount", 0, false)]
        [Variation("ComputePageCountAsync", 0, false)]     
        [Variation("PagesChanged", 8, true)]        
        [Variation("PaginationProgress", 0, true)]
        [Variation("PaginationProgress", 0, false)]
        [Variation("GetPageNegInput", -1, true)]
        [Variation("GetPageAsyncNegInput", -1, true)]
        [Variation("GetObjectPositionNotInTree", 0, true)]
        [Variation("GetPageNumberContentPositionNotInTree", 0, true)]
        [Variation("GetPageNumberAsyncContentPositionNotInTree", 0, true)]
        [Variation("PageDestroyed", 8, true)]     
        [Variation("AsyncCancel", 0, true)]
        [Variation("AsyncCancel", 0, false)]
        public PaginationTests(string testName, int pageNumber, bool isBackgroundPaginationEnabled)
            : base()
        {
            _pageHeight = 400;
            _pageWidth = 700;
            this._testName = testName;           
            this._pageNumber = pageNumber;
            this._isBackgroundPaginationEnabled = isBackgroundPaginationEnabled;            

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(ShowContent);
            SetAdditionalTestSteps();            
        }

        private TestResult Initialize()
        {
            _frame = new DispatcherFrame(true);    
            
            _testWin = new Window();
            _testWin.Content = Content();
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).PaginationCompleted += new EventHandler(PaginationTests_PaginationCompleted);

            //Get the content position of the object we will be looking for in some tests
            _box = _fd.FindName("CanvasBox") as Canvas;
            _ecp = ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetObjectPosition(_box);

            //Get Paragraph where the Canvas is so we can remove it in some tests
            _boxParent = _fd.FindName("BoxParent") as Paragraph;

            //Get a ContentPosition for an object not in the document tht is used in some tests
            Paragraph dummyParagraph = new Paragraph();
            FlowDocument dummyFlowDocument = new FlowDocument(dummyParagraph);
            IDocumentPaginatorSource dummyIDocumentPaginatorSource = dummyFlowDocument;
            _dummyContentPosition = ((DynamicDocumentPaginator)(dummyIDocumentPaginatorSource.DocumentPaginator)).GetObjectPosition(dummyParagraph);

            if (_testName == "PaginationProgress")
            {
                ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).PaginationProgress += new PaginationProgressEventHandler(PaginationTests_PaginationProgress);
            }                      
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

        private TestResult ShowContent()
        {
            _testWin.Show();

            //When IDocumentPaginator.IsBackgroundPaginationEnabled is false there is no PaginationCompleted event so we must do a physical wait   
            //If enabled, we wait for the event
            if (!_isBackgroundPaginationEnabled)
            {
                WaitFor(1000);
            }
            else
            {
                WaitForEvent();
            }
            return TestResult.Pass;
        }

        private void SetAdditionalTestSteps()
        {
            switch (_testName)
            {
                case "Property":
                    {
                        RunSteps += new TestStep(RunPropertyTest);
                        break;
                    }
                case "GetPage":
                    {
                        RunSteps += new TestStep(RunGetPageTest);
                        break;
                    }
                case "GetPageAsync":
                    {
                        RunSteps += new TestStep(RunGetPageAsyncTest);
                        break;
                    }              
                case "GetPageNumber":
                    {
                        RunSteps += new TestStep(RunGetPageNumberTest);
                        break;
                    }
                case "GetPageNumberAsync":
                    {
                        RunSteps += new TestStep(RunGetPageNumberAsyncTest);
                        break;
                    }               
                case "GetObjectPosition":
                    {
                        RunSteps += new TestStep(RunGetObjectPositionTest);
                        break;
                    }
                case "ComputePageCount":
                    {
                        RunSteps += new TestStep(RunComputePageCountTest);
                        break;
                    }
                case "ComputePageCountAsync":
                    {
                        RunSteps += new TestStep(RunComputePageCountAsyncTest);
                        break;
                    }                
                case "PagesChanged":
                    {
                        RunSteps += new TestStep(RunPagesChangedTest);
                        break;
                    }
                case "PaginationProgress":
                    {
                        RunSteps += new TestStep(RunPaginationProgressTest);
                        break;
                    }
                case "GetPageNegInput":
                    {
                        RunSteps += new TestStep(RunGetPageNegInputTest);
                        break;
                    }
                case "GetPageAsyncNegInput":
                    {
                        RunSteps += new TestStep(RunGetPageAsyncNegInputTest);
                        break;
                    }
                case "GetObjectPositionNotInTree":
                    {
                        RunSteps += new TestStep(RunGetObjectPositionNotInTreeTest);
                        break;
                    }
                case "GetPageNumberContentPositionNotInTree":
                    {
                        RunSteps += new TestStep(RunGetPageNumberContentPositionNotInTreeTest);
                        break;
                    }
                case "GetPageNumberAsyncContentPositionNotInTree":
                    {
                        RunSteps += new TestStep(RunGetPageNumberAsyncContentPositionNotInTreeTest);
                        break;
                    }
                case "PageDestroyed":
                    {
                        RunSteps += new TestStep(RunPageDestroyedTest);
                        break;
                    }
                case "AsyncCancel":
                    {
                        RunSteps += new TestStep(RunAsyncCancelTest);
                        break;
                    }               
                default:
                    {
                        TestLog.Current.LogEvidence("Failed to find a test to run!");
                        TestLog.Current.Result = TestResult.Fail;
                        break;
                    }
            }
        }
        
        private FrameworkElement Content()
        {                                                            
            //Create an IDocumentPaginator
            _fd = XamlReader.Load(File.OpenRead("PaginationFlowDocumentContent.xaml")) as FlowDocument;
            _iDocPag = _fd as IDocumentPaginatorSource;            
            _iDocPagPageSize = new Size(_pageWidth, _pageHeight);
            _iDocPag.DocumentPaginator.PageSize = _iDocPagPageSize;

            //set BackgroundPagination to false if specified
            if (!_isBackgroundPaginationEnabled)
            {
                ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).IsBackgroundPaginationEnabled = false;
            }

            //Set the DocumentPageView up
            _docPagVw = new DocumentPageView();           
            _docPagVw.Height = _pageHeight;
            _docPagVw.Width = _pageWidth;
            _docPagVw.DocumentPaginator = _iDocPag.DocumentPaginator;            

            return _docPagVw;
        }

        private TestResult RunPropertyTest()
        {
            if (_iDocPag.DocumentPaginator.PageSize.Width != _pageWidth || _iDocPag.DocumentPaginator.PageSize.Height != _pageHeight || ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).IsBackgroundPaginationEnabled != _isBackgroundPaginationEnabled)
            {
                TestLog.Current.LogEvidence("IDocumentPaginator properties were not as expected.");
                TestLog.Current.LogEvidence("IDocumentPaginator.PageSize.Width: " + _iDocPag.DocumentPaginator.PageSize.Width.ToString() + " expected: " + _pageWidth.ToString());
                TestLog.Current.LogEvidence("IDocumentPaginator.PageSize.Height: " + _iDocPag.DocumentPaginator.PageSize.Height.ToString() + " expected: " + _pageHeight.ToString());
                TestLog.Current.LogEvidence("IDocumentPaginator.IsBackgroundPaginationEnabled: " + ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).IsBackgroundPaginationEnabled.ToString() + " expected " + _isBackgroundPaginationEnabled);
                TestLog.Current.Result = TestResult.Fail;
            }

            if (_docPagVw.PageNumber != _pageNumber)
            {
                TestLog.Current.LogEvidence("DocumentPageView.PageNumber property not as expected.");
                TestLog.Current.LogEvidence("PageNumber: " + _docPagVw.PageNumber.ToString() + " expected: " + _pageNumber);
                TestLog.Current.Result = TestResult.Fail;
            }
            
            DocumentPage page = _docPagVw.DocumentPage;
            if (page == DocumentPage.Missing)
            {
                TestLog.Current.LogEvidence("The DocumentPage returned is empty (DocumentPage.Missing)!!");
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult RunGetPageTest()
        {
            if (!_iDocPag.DocumentPaginator.IsPageCountValid)
            {
                TestLog.Current.LogEvidence("IsPageCountValid is false, should be true.");
                TestLog.Current.Result = TestResult.Fail;
            }

            DocumentPage dp = _iDocPag.DocumentPaginator.GetPage(_pageNumber);
            if (dp == DocumentPage.Missing)
            {
                TestLog.Current.LogEvidence("Did not get the page.");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                if (dp.Size != _iDocPagPageSize)
                {
                    TestLog.Current.LogEvidence("DocumentPage.Size was not as expected.");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            return TestResult.Pass;
        }

        private TestResult RunGetPageAsyncTest()
        {
            _iDocPag.DocumentPaginator.GetPageCompleted += new GetPageCompletedEventHandler(DocumentPaginator_GetPageCompleted);
            _frame.Continue = true;
            _iDocPag.DocumentPaginator.GetPageAsync(_pageNumber, _paginatorUserState);

            WaitForEvent();
            
            return TestResult.Pass;            
        }

        private TestResult RunGetPageNumberTest()
        {           
            int ePage = ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetPageNumber(_ecp);            
            
            if (ePage != _pageNumber)
            {
                TestLog.Current.LogEvidence("Element did not start on the expected page!");                
                TestLog.Current.LogEvidence("GetPageForContentPosition reports that Element is on page: " + ePage.ToString() + " expected: " + _pageNumber);
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult RunGetPageNumberAsyncTest()
        {
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetPageNumberCompleted += new GetPageNumberCompletedEventHandler(PaginationTests_GetPageNumberCompleted);
            _frame.Continue = true;
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetPageNumberAsync(_ecp);

            WaitForEvent();
            
            return TestResult.Pass;
        }

        private TestResult RunGetObjectPositionTest()
        {            
            ContentPosition cp = ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetObjectPosition(_box);
            if (null == cp || cp == ContentPosition.Missing)
            {
                TestLog.Current.LogEvidence("GetObjectPosition(): Returned ContentPosition.Missing!");
                TestLog.Current.Result = TestResult.Fail;
            }            
            return TestResult.Pass;
        }

        private TestResult RunComputePageCountTest()
        {            
            int pgCountBefore = _iDocPag.DocumentPaginator.PageCount;            
            _iDocPag.DocumentPaginator.ComputePageCount();
            
            if (!_iDocPag.DocumentPaginator.IsPageCountValid)
            {
                TestLog.Current.LogEvidence("ComputePageCount(): IsPageCountValid is false!");
                TestLog.Current.Result = TestResult.Fail;
            }

            if (_iDocPag.DocumentPaginator.PageCount <= pgCountBefore)
            {
                TestLog.Current.LogEvidence("ComputePageCount(): Incorrect PageCount");
                TestLog.Current.LogEvidence("Expected: >" + pgCountBefore + " Actual: " + _iDocPag.DocumentPaginator.PageCount);
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult RunComputePageCountAsyncTest()
        {            
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).ComputePageCountCompleted += new System.ComponentModel.AsyncCompletedEventHandler(PaginationTests_ComputePageCountCompleted);

            int pgCountBefore = _iDocPag.DocumentPaginator.PageCount;
            _frame.Continue = true;
            _iDocPag.DocumentPaginator.ComputePageCountAsync();

            WaitForEvent();

            if (!_iDocPag.DocumentPaginator.IsPageCountValid)
            {
                TestLog.Current.LogEvidence("ComputePageCount(): IsPageCountValid is false!");
                TestLog.Current.Result = TestResult.Fail;
            }

            if (_iDocPag.DocumentPaginator.PageCount <= pgCountBefore)
            {
                TestLog.Current.LogEvidence("ComputePageCount(): Incorrect PageCount");
                TestLog.Current.LogEvidence("Expected: >" + pgCountBefore + "Actual: " + _iDocPag.DocumentPaginator.PageCount);
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult RunPagesChangedTest()
        {
            _iDocPag.DocumentPaginator.PagesChanged += new PagesChangedEventHandler(PaginationTests_PagesChanged);

            _frame.Continue = true;
            ChangeContent();

            WaitForEvent();

            return TestResult.Pass;
        }

        private TestResult RunPaginationProgressTest()
        {
            if (_paginationProgressCount != _iDocPag.DocumentPaginator.PageCount)
            {
                TestLog.Current.LogEvidence("The PaginationProgress count did not match the PageCount!");
                TestLog.Current.LogEvidence("PaginationProgress count: " + _paginationProgressCount + "PageCount: " + _iDocPag.DocumentPaginator.PageCount);
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult RunGetPageNegInputTest()
        {                    
            try
            {
                _iDocPag.DocumentPaginator.GetPage(_pageNumber);
                TestLog.Current.LogEvidence("GetPage(): Did not get an Exception when trying negative input for IDocumentPaginatorSource.GetPage()!");
                TestLog.Current.Result = TestResult.Fail;
            }
            catch (System.ArgumentOutOfRangeException)
            {}                        
            return TestResult.Pass;
        }

        private TestResult RunGetPageAsyncNegInputTest()
        {            
            try
            {
                _iDocPag.DocumentPaginator.GetPageAsync(_pageNumber, _paginatorUserState);
                TestLog.Current.LogEvidence("GetPageAsync(): Did not get an Exception when trying negative input for IDocumentPaginatorSource.GetPageAsync()!");
                TestLog.Current.Result = TestResult.Fail;
            }
            catch (System.ArgumentOutOfRangeException)
            {}
            return TestResult.Pass;
        }
       
        private TestResult RunGetObjectPositionNotInTreeTest()
        {                     
            ChangeContent();                       

            ContentPosition cp = ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetObjectPosition(_box);            
            if (cp != ContentPosition.Missing)
            {
                TestLog.Current.LogEvidence("GetObjectPosition(): Did not get ContentPosition.Missing when trying to request a ContentPosition for an Object that does not exist in the document.");
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;                     
        }
        
        private TestResult RunGetPageNumberContentPositionNotInTreeTest()
        {                       
            try
            {
                int page = ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetPageNumber(_dummyContentPosition);
                TestLog.Current.LogEvidence("GetPageNumber(): Did not get an Exception when trying to get a Page for a ContentPosition that does not exist in the document!");
                TestLog.Current.Result = TestResult.Fail;
            }
            catch (System.ArgumentException)
            {}
            return TestResult.Pass;
        }

        private TestResult RunGetPageNumberAsyncContentPositionNotInTreeTest()
        {           
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetPageNumberCompleted += new GetPageNumberCompletedEventHandler(PaginationTests_GetPageNumberCompleted_CPNotInTree);
            try
            {
                ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetPageNumberAsync(_dummyContentPosition, _paginatorUserState);
                TestLog.Current.LogEvidence("GetPageNumberAsync(): Did not get an Exception when trying to get a Page for a ContentPosition that does not exist in the document!");
                TestLog.Current.Result = TestResult.Fail;
            }
            catch (System.ArgumentException)
            {}
            return TestResult.Pass;
        }

        private TestResult RunPageDestroyedTest()
        {
            DocumentPage dpDestroy = _iDocPag.DocumentPaginator.GetPage(_pageNumber);
            dpDestroy.PageDestroyed += new EventHandler(dpDestroy_PageDestroyed);
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).PaginationCompleted += new EventHandler(PaginationTests_PaginationCompleted_DestroyPage);

            _frame.Continue = true;
            ChangeContent();

            WaitForEvent();
            
            if (!_pageDestroyed)
            {
                TestLog.Current.LogEvidence("Did not get the PageDestroyed event after changing content on a DocumentPage");                
                TestLog.Current.Result = TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult RunAsyncCancelTest()
        {                       
            //Our "Async" calls really behave synchronously, so we can't really cancel Async calls
            //This test just makes sure that we don't blow up when we call the cancel methods.

            //Cancel GetPageAsync
            _iDocPag.DocumentPaginator.GetPageAsync(_pageNumber, _paginatorUserState);
            _iDocPag.DocumentPaginator.CancelAsync(_paginatorUserState);

            //Cancel GetPageNumberAsync
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetPageNumberAsync(_ecp);
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).CancelAsync(_paginatorUserState);

            //Cancel ComputePageAsync
            _iDocPag.DocumentPaginator.ComputePageCountAsync(_paginatorUserState);
            _iDocPag.DocumentPaginator.CancelAsync(_paginatorUserState);
           
            //Cancel all Async calls
            object paginatorUserState2 = new object();
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).GetPageNumberAsync(_ecp);            
            _iDocPag.DocumentPaginator.ComputePageCountAsync(_paginatorUserState);
            _iDocPag.DocumentPaginator.GetPageAsync(_pageNumber, paginatorUserState2);
            _iDocPag.DocumentPaginator.CancelAsync(null);
            
            return TestResult.Pass;
        }  

        private void PaginationTests_PaginationCompleted(object sender, EventArgs e)
        {
            ((DynamicDocumentPaginator)(_iDocPag.DocumentPaginator)).PaginationCompleted -= new EventHandler(PaginationTests_PaginationCompleted);                      
            _frame.Continue = false;
        }

        private void PaginationTests_PaginationCompleted_DestroyPage(object sender, EventArgs e)
        {
            _frame.Continue = false;
        }

        private void dpDestroy_PageDestroyed(object sender, EventArgs e)
        {           
            _pageDestroyed = true;
        }       
        
        private void DocumentPaginator_GetPageCompleted(object sender, GetPageCompletedEventArgs e)
        {
            if (_paginatorUserState != e.UserState)
            {
                TestLog.Current.LogEvidence("GetPageCompletedEventArgs.UserState is not as expected!");
                TestLog.Current.Result = TestResult.Fail;                
            }

            if (e.Cancelled)
            {
                TestLog.Current.LogEvidence("GetPageAsync was cancelled!");
                TestLog.Current.Result = TestResult.Fail;                
            }

            if (e.Error != null)
            {
                TestLog.Current.LogEvidence("GetPageAsync encountered an error!");
                TestLog.Current.LogEvidence(e.Error.ToString());
                TestLog.Current.Result = TestResult.Fail;
            }

            if (e.PageNumber == _pageNumber)
            {                
                DocumentPage dp = e.DocumentPage;
                if (dp == DocumentPage.Missing)
                {
                    TestLog.Current.LogEvidence("Did not get the page.");
                    TestLog.Current.Result = TestResult.Fail;
                }
                else
                {                    
                    if (dp.Size != _iDocPagPageSize)
                    {
                        TestLog.Current.LogEvidence("DocumentPage.Size was not as expected.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else
            {
                TestLog.Current.LogEvidence("Did not get the page we requested!!");
                TestLog.Current.LogEvidence("Got page: " + e.PageNumber + " requested page: " + _pageNumber);
                TestLog.Current.Result = TestResult.Fail;
            }
            _frame.Continue = false;
        }
       
        private void PaginationTests_GetPageNumberCompleted(object sender, GetPageNumberCompletedEventArgs e)
        {            
            if (e.Cancelled)
            {
                TestLog.Current.LogEvidence("GetPageNumberAsync was cancelled!");
                TestLog.Current.Result = TestResult.Fail;                
            }

            if (e.Error != null)
            {
                TestLog.Current.LogEvidence("GetPageNumberAsync encountered an error!");
                TestLog.Current.LogEvidence(e.Error.ToString());
                TestLog.Current.Result = TestResult.Fail;
            }
            
            if (e.PageNumber != _pageNumber)
            {
                TestLog.Current.LogEvidence("Did not get the page we requested!!");                
                TestLog.Current.LogEvidence("Got page: " + e.PageNumber + " requested page: " + _pageNumber);
                TestLog.Current.Result = TestResult.Fail;
            }
            _frame.Continue = false;
        }
        
        private void PaginationTests_PagesChanged(object sender, PagesChangedEventArgs e)
        {            
            if (e.Start != _pageNumber)
            {
                TestLog.Current.LogEvidence("IDocumentPaginator.PagesChanged.Start was invalid.");                
                TestLog.Current.LogEvidence("e.Start: " + e.Start.ToString() + " expected: " + _pageNumber);
                TestLog.Current.Result = TestResult.Fail;
            }
            _frame.Continue = false;
        }

        private void PaginationTests_PaginationProgress(object sender, PaginationProgressEventArgs e)
        {
            if (e.Count != 1 || e.Start != _paginationProgressCount)
            {
                TestLog.Current.LogEvidence("PaginationProgress event args not as expected!!");
                TestLog.Current.LogEvidence("args.Count: " + e.Count.ToString() + " expected: 1");
                TestLog.Current.LogEvidence("args.Start: " + e.Start.ToString() + " expected: " + _paginationProgressCount);
                TestLog.Current.Result = TestResult.Fail;
            }
            _paginationProgressCount++;
        }

        private void PaginationTests_GetPageNumberCompleted_CPNotInTree(object sender, GetPageNumberCompletedEventArgs e)
        {            
            TestLog.Current.LogEvidence("GetPageNumberCompleted: Should not get this event!  There should have been an exception!");
            TestLog.Current.Result = TestResult.Fail;
        }      

        private void PaginationTests_ComputePageCountCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                TestLog.Current.LogEvidence("GetPageNumberAsync was cancelled!");
                TestLog.Current.Result = TestResult.Fail;
            }

            if (e.Error != null)
            {
                TestLog.Current.LogEvidence("ComputePageCountAsync encountered an error!");
                TestLog.Current.LogEvidence(e.Error.ToString());
                TestLog.Current.Result = TestResult.Fail;
            }
            _frame.Continue = false;
        }

        private void ChangeContent()
        {
            _boxParent.Inlines.Clear();
            _boxParent.Inlines.Add(new Run(String.Empty));
            WaitForPriority(DispatcherPriority.ApplicationIdle);
        }

        private void WaitForEvent()
        {            
            Dispatcher.PushFrame(_frame);            
        }
    }
}


