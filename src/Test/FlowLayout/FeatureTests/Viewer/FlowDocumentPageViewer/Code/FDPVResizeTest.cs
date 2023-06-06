// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Markup;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Viewer.FlowDocumentPageViewer</area>
    /// <owner>Microsoft</owner>
    /// <priority>0</priority>
    /// <description>
    /// Testing resize in FlowDocumentPageViewer .
    /// </description>
    /// </summary>
    [Test(0, "Viewer.FlowDocumentPageViewer", "FDPVResize", MethodName = "Run")]
    public class FDPVResizeTest : AvalonTest
    {
        #region Test case members
        private FlowDocumentPageViewer _viewer;
        private FlowDocument _fd;
        private Window _testWin;
        private int _cnt = 0;        
        private int _initialPageCount;
        private string _inputXaml;
        private string _inputString;
        #endregion
        
        #region Constructor
        [Variation("reptiles_Flow.xaml", "ResizeMainWindow")]
        [Variation("reptiles_Flow.xaml", "ResizeFlowDocument")]
        [Variation("FlowDocumentSample1IndicText.xaml", "ResizeMainWindow")]
        [Variation("FlowDocumentSample1IndicText.xaml", "ResizeFlowDocument")]
        public FDPVResizeTest(string xamlFile, string testValue)
            : base()
        {
            _inputXaml = xamlFile;
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Test Steps
       
        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {           
            _testWin = new Window();
            _testWin.Width = 800;
            _testWin.Height = 600;
            Status("Initialize ....");
            _fd = (FlowDocument)XamlReader.Load(File.OpenRead(_inputXaml));
            _viewer = new FlowDocumentPageViewer();
            _viewer.Document = _fd;
            ((DynamicDocumentPaginator)(_viewer.Document.DocumentPaginator)).PaginationCompleted += new EventHandler(FDPVNavigationTest_PaginationCompleted);

            _testWin.Content = _viewer;
            _testWin.Show();
           
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            _initialPageCount = _viewer.PageCount;
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "resizemainwindow":                   
                    Status("...........Resizing the window ..............");
                    _testWin.SizeChanged += new SizeChangedEventHandler(DoNothing);                    
                                  
                    ResizeWindow(1000, 1000);
                     
                    ResizeWindow(800, 600);
                    
                    break;
                case "resizeflowdocument":                   
                    Status("......Resize the FlowDocument.....");
                    ViewerResizeHelper.FlowDocumentResized += new EventHandler(ViewerResizeHelper_FlowDocumentResized);
                    ViewerResizeHelper.ResizeFlowDocument(_fd, 300, 300);
                    WaitForSignal();
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    ViewerResizeHelper.ResizeFlowDocument(_fd, 200, 500);
                    WaitForSignal();
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    ViewerResizeHelper.ResizeFlowDocument(_fd, 500, 100);
                    WaitForSignal();
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    ViewerResizeHelper.ResizeFlowDocument(_fd, 500, 600);
                    WaitForSignal();
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    ViewerResizeHelper.ResizeFlowDocument(_fd, 1000, 1000);
                    WaitForSignal();
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    ViewerResizeHelper.ResizeFlowDocument(_fd, Double.NaN, Double.NaN);
                    WaitForSignal();
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    break;
                default:
                    Status("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    return TestResult.Fail;                    
            }
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify: Verifies tests by comparing page count.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Verify()
        {
            if (_cnt > 3)
            {
                if (_testWin.ActualWidth != 800 && _testWin.ActualHeight != 600)
                {
                    TestLog.Current.LogEvidence(string.Format("Problem: Width and Height are not as expected! Width: {0} Height: {1}", _testWin.Width, _testWin.Height));                    
                }
                
                if (_viewer.PageCount == _initialPageCount)
                {
                    LogComment("Test Passed, testcase resized correctly InitialPageCount = " + _initialPageCount + " ; FinalPageCount = " + _viewer.PageCount);
                    return TestResult.Pass;
                }
                else
                {
                    TestLog.Current.LogEvidence("Test Failed, testcase did not resize correctly InitialPageCount = " + _initialPageCount + " ; FinalPageCount = " + _viewer.PageCount);
                    return TestResult.Fail;
                }
            }
            else
            {
                TestLog.Current.LogEvidence("Test Failed, The Window and/or Pagination did not happen enough times.");
                return TestResult.Fail;
            }
        }

        private void ResizeWindow(double newWidth, double newHeight)
        {
            _testWin.Width = newWidth;
            _testWin.Height = newHeight;
            WaitForPriority(DispatcherPriority.ApplicationIdle);
        }

        #endregion

        #region event handlers
        private void FDPVNavigationTest_PaginationCompleted(object sender, EventArgs e)
        {
            _cnt++;            
        }

        private void ViewerResizeHelper_FlowDocumentResized(object sender, EventArgs e)
        {
            Signal(TestResult.Pass);
            _cnt++;
        }

        private void DoNothing(object sender, EventArgs e)
        {                        
            Signal(TestResult.Pass);
            _cnt++;
        }
        #endregion
    }

    public class ViewerResizeHelper
    {
        public static event EventHandler FlowDocumentResized;
        public int count = 0;
        private static int s_waitTime = 600;

        internal struct FlowDocumentElementResizeData
        {
            public FlowDocument fe;
            public double newWidth, newHeight;

            public FlowDocumentElementResizeData(FlowDocument fe, int newWidth, int newHeight)
            {
                this.fe = fe;
                this.newWidth = newWidth;
                this.newHeight = newHeight;
            }
        }

        public static void ResizeFlowDocument(Object obj, double width, double height)
        {
            FlowDocumentElementResizeData data;
            data.fe = (FlowDocument)obj;
            data.newWidth = width;
            data.newHeight = height;

            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background, data.fe.Dispatcher);
            timer.Tick += new EventHandler(ResizeFlowDocumentHandler);
            timer.Interval = TimeSpan.FromMilliseconds(s_waitTime);
            timer.Tag = data;
            timer.Start();
        }

        private static void ResizeFlowDocumentHandler(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            FlowDocumentElementResizeData data = (FlowDocumentElementResizeData)(timer.Tag);

            data.fe.PageWidth = data.newWidth;
            data.fe.PageHeight = data.newHeight;
            data.fe.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(NotifyResize), data.fe);
            timer.Stop();
        }

        private static object NotifyResize(object o)
        {
            if (FlowDocumentResized != null)
            {
                FlowDocumentResized(o, EventArgs.Empty);
            }
            return null;
        }
    }
}
