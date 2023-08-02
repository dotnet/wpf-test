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
using System.Windows.Navigation;
using System.Windows.Input;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Viewer.FlowDocumentReader</area>
    /// <owner>Microsoft</owner>
    /// <priority>0</priority>
    /// <description>
    /// Testing commands in FlowDocumentReader.
    /// </description>
    /// </summary>
    [Test(0, "Viewer.FlowDocumentReader", "CommandsTest", MethodName = "Run")]
    public class CommandsTest : WindowTest
    {
        #region Test case members

        private FlowDocumentReader _viewer;
        private FlowDocument _fd;
        private NavigationWindow _navWin;
        private string _inputXaml;
        private string _inputString = "";
        private string _viewingMode = "";

        #endregion

        #region Constructor

        [Variation("reptiles_Flow.xaml", "ZoomInCommand", "Page")]
        [Variation("reptiles_Flow.xaml", "ZoomInCommand", "TwoPage")]
        [Variation("reptiles_Flow.xaml", "ZoomInCommand", "Scroll")]
        [Variation("reptiles_Flow.xaml", "ZoomOutCommand", "Page")]
        [Variation("reptiles_Flow.xaml", "ZoomOutCommand", "TwoPage")]
        [Variation("reptiles_Flow.xaml", "ZoomOutCommand", "Scroll")]
        [Variation("reptiles_Flow.xaml", "ExecuteCommand", "Page")]
        [Variation("reptiles_Flow.xaml", "ExecuteCommand", "TwoPage")]

        public CommandsTest(string xamlFile, string testValue, string testViewMode)
            : base()
        {
            _inputXaml = xamlFile;
            _inputString = testValue;
            _viewingMode = testViewMode;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentReader and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            //Hide the parent window, so it won't overlay the Animation window.
            Window.Visibility = Visibility.Hidden;
            _navWin = new NavigationWindow();

            Status("Initialize ....");
            _fd = (FlowDocument)XamlReader.Load(File.OpenRead(_inputXaml));
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
                    TestLog.Current.Result = TestResult.Fail;
                    break;
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
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "zoomincommand":
                    for (double orig = _viewer.Zoom; _viewer.CanIncreaseZoom; orig = _viewer.Zoom)
                    {
                        NavigationCommands.IncreaseZoom.Execute(null, _viewer);
                        Status(_viewer.Zoom.ToString() + " , " + _viewer.MaxZoom.ToString() + " ; ");
                        WaitForPriority(DispatcherPriority.ApplicationIdle);

                        if (!DoubleUtil.AreClose(_viewer.Zoom, Math.Min(orig + _viewer.ZoomIncrement, _viewer.MaxZoom)))
                        {
                            TestLog.Current.LogEvidence("Test Failed: ZoomIn command not work on itself");
                            TestLog.Current.Result = TestResult.Fail;
                            break;
                        }
                    }

                    //the zoom should be equal to maxzoom when zoom cannot be increased anymore
                    if (!DoubleUtil.AreClose(_viewer.Zoom, _viewer.MaxZoom))
                    {
                        TestLog.Current.LogEvidence("Test failed to Zoom in properly the final Zoom != MaxZoom value.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    break;
                case "zoomoutcommand":
                    for (double orig = _viewer.Zoom; _viewer.CanDecreaseZoom; orig = _viewer.Zoom)
                    {
                        NavigationCommands.DecreaseZoom.Execute(null, _viewer);
                        Status(_viewer.Zoom.ToString() + " , " + _viewer.MinZoom.ToString() + " ; ");
                        WaitForPriority(DispatcherPriority.ApplicationIdle);

                        if (!DoubleUtil.AreClose(_viewer.Zoom, Math.Max(orig - _viewer.ZoomIncrement, _viewer.MinZoom)))
                        {
                            TestLog.Current.LogEvidence("Test Failed: ZoomOut command not work on itself");
                            TestLog.Current.Result = TestResult.Fail;
                            break;
                        }
                    }

                    //the zoom should be equal to minzoom when zoom cannot be decreased anymore
                    if (!DoubleUtil.AreClose(_viewer.Zoom, _viewer.MinZoom))
                    {
                        TestLog.Current.LogEvidence("Test failed to Zoom out properly. The final Zoom != MinZoom value.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    break;
                case "executecommand":
                    for (int i = 0; i < 5; i++)
                    {
                        NavigationCommands.NextPage.Execute(null, _viewer);
                        WaitFor(500);
                    }
                    VerifyCommandTest("NextPage");

                    NavigationCommands.FirstPage.Execute(null, _viewer);
                    WaitFor(500);
                    VerifyCommandTest("FirstPage");

                    NavigationCommands.LastPage.Execute(null, _viewer);
                    WaitFor(500);
                    VerifyCommandTest("LastPage");

                    for (int i = 0; i < 3; i++)
                    {
                        NavigationCommands.PreviousPage.Execute(null, _viewer);
                        WaitFor(500);
                    }
                    
                    VerifyCommandTest("PreviousPage");
                    break;

                default:
                    TestLog.Current.LogEvidence("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    TestLog.Current.Result = TestResult.Fail;
                    break;
            }
            return TestResult.Pass;
        }

        #endregion

        #region Verification

        private void VerifyCommandTest(string input)
        {
            if (_viewer.ViewingMode == FlowDocumentReaderViewingMode.Page)
            {
                switch (input)
                {
                    case "NextPage":
                        if (_viewer.PageNumber != 6)
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute NextPage command correctly. Expected PageNumber: 6 Actual: " + _viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                    case "FirstPage":
                        if ((_viewer.PageNumber != 1) && (_viewer.CanGoToPreviousPage != false))
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute FirstPage command correctly. Expected CanGoToPreviousPage: false Actual: " + _viewer.CanGoToPreviousPage + " Expected PageNumber: 1 Actual: " + _viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                    case "GoToPage":
                        if (_viewer.PageNumber != 8)
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute GoToPage command correctly. Expected PageNumber: 8 Actual: " + _viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                    case "LastPage":
                        if ((_viewer.PageNumber != _viewer.PageCount) && (_viewer.CanGoToNextPage != false))
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute LastPage command correctly. Expected CanGoToNextPage: false Actual: " + _viewer.CanGoToNextPage + " EXpected PageNumber: " + _viewer.PageCount + " Actual: " + _viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                    case "PreviousPage":
                        if (_viewer.PageNumber != ((_viewer.PageCount) - 3))
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute PreviousPage command correctly. Expected PageNumber : " + (_viewer.PageCount - 3) + "Actual: " + _viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                }
            }
            else if (_viewer.ViewingMode == FlowDocumentReaderViewingMode.TwoPage)
            {
                switch (input)
                {
                    case "NextPage":
                        if (_viewer.PageNumber != 11)
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute NextPage command correctly. Expected PageNumber: 11 Actual: " + _viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                    case "FirstPage":
                        if ((_viewer.PageNumber != 1) && (_viewer.CanGoToPreviousPage != false))
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute FirstPage command correctly. Expected CanGoToPreviousPage: false Actual: " + _viewer.CanGoToPreviousPage + " Expected PageNumber: 1 Actual: " + _viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                    case "GoToPage":
                        if (_viewer.PageNumber != 8)
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute GoToPage command correctly. Expected PageNumber: 8 Actual: " + _viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                    case "LastPage":
                        if (((_viewer.PageNumber != _viewer.PageCount) || (_viewer.PageNumber != _viewer.PageCount - 1)) && (_viewer.CanGoToNextPage != false))
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute LastPage command correctly. Expected CanGoToNextPage: false Actual: " + _viewer.CanGoToNextPage + " Expected PageNumber: " + _viewer.PageCount + " Actual: " +_viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                    //(3 * 2) is in reference to executing the PreviousPage command 3 times.  In 2 page view we would go back 2 pages for every PrevioussPage execution
                    case "PreviousPage":
                        if (_viewer.PageNumber != ((_viewer.PageCount) - (3 * 2)) && _viewer.PageNumber != ((_viewer.PageCount) - (3 * 2) - 1))
                        {
                            TestLog.Current.LogEvidence("Test Failed, did not execute PreviousPage command correctly. Expected PageNumber: " + ((_viewer.PageCount) - (3 * 2)) + " Actual: " + _viewer.PageNumber);
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        break;
                }
            }
            else if (_viewer.ViewingMode == FlowDocumentReaderViewingMode.Scroll)
            {
                TestLog.Current.LogEvidence("Test Failed, The viewermode is Scroll so no NavigationCommands can be executed");
                TestLog.Current.Result = TestResult.Fail;
            }            
        }
        #endregion
    }
}
