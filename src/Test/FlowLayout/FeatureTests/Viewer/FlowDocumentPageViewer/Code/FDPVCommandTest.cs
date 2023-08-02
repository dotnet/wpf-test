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
    /// Testing commands in FlowDocumentPageViewer.   
    /// </summary>
    [Test(0, "Viewer.FlowDocumentPageViewer", "FDPVCommandTest", MethodName = "Run")]
    public class FDPVCommandsTest : WindowTest
    {
        private FlowDocumentPageViewer _viewer;
        private FlowDocument _fd;
        private NavigationWindow _navWin;
        private string _inputXaml;
        private string _inputString = "";

        [Variation("reptiles_Flow.xaml", "ZoomInCommand")]
        [Variation("reptiles_Flow.xaml", "ZoomOutCommand")]
        [Variation("reptiles_Flow.xaml", "ExecuteCommand")]
        public FDPVCommandsTest(string xamlFile, string testValue)
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
                            LogComment("Test Failed: ZoomIn command not work on itself");
                            Log.Result = TestResult.Fail;
                            break;
                        }
                    }

                    //the zoom shoudl be equal to maxzoom when zoom cannot be increased anymore
                    if (DoubleUtil.AreClose(_viewer.Zoom, _viewer.MaxZoom))
                    {
                        LogComment("Test Passed, The ZoomIn command zoomed correctly and Zoom == MaxZoom value");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test failed to Zoom in properly the final Zoom != MaxZoom value.");
                        Log.Result = TestResult.Fail;
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
                            LogComment("Test Failed: ZoomOut command not work on itself");
                            Log.Result = TestResult.Fail;
                            break;
                        }
                    }

                    //the zoom should be equal to minzoom when zoom cannot be decreased anymore
                    if (DoubleUtil.AreClose(_viewer.Zoom, _viewer.MinZoom))
                    {
                        LogComment("Test Passed, The ZoomOut command zoomed correctly and Zoom == MinZoom value");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test failed to Zoom out properly. The final Zoom != MinZoom value.");
                        Log.Result = TestResult.Fail;
                    }
                    break;

                case "executecommand":
                    for (int i = 0; i < 5; i++)
                    {
                        NavigationCommands.NextPage.Execute(null, _viewer);
                        WaitForPriority(DispatcherPriority.ApplicationIdle);
                    }
                    Log.Result = VerifyCommandTest("NextPage");
                    NavigationCommands.FirstPage.Execute(null, _viewer);
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    Log.Result = VerifyCommandTest("FirstPage");
                    NavigationCommands.LastPage.Execute(null, _viewer);
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    Log.Result = VerifyCommandTest("LastPage");
                    for (int i = 0; i < 3; i++)
                    {
                        NavigationCommands.PreviousPage.Execute(null, _viewer);
                        WaitForPriority(DispatcherPriority.ApplicationIdle);
                    }
                    Log.Result = VerifyCommandTest("PreviousPage");
                    break;

                default:
                    Status("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult VerifyCommandTest(string input)
        {
            switch (input)
            {
                case "NextPage":
                    if (_viewer.MasterPageNumber == 6)
                    {
                        LogComment("Test Passed, Test executed the NextPage command correctly  MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test Failed, Test did not execute NextPage command correctly  MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Fail;
                    }
                    break;

                case "FirstPage":
                    if ((_viewer.MasterPageNumber == 1) && (_viewer.CanGoToPreviousPage == false))
                    {
                        LogComment("Test Passed, Test executed the FirstPage command correctly; CanGoToPreviousPage : " + _viewer.CanGoToPreviousPage + " / CanGoToNextPage: " + _viewer.CanGoToNextPage + " / MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test Failed, Test did not execute FirstPage command correctly; CanGoToPreviousPage : " + _viewer.CanGoToPreviousPage + " / CanGoToNextPage: " + _viewer.CanGoToNextPage + " / MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Fail;
                    }
                    break;

                case "GoToPage":
                    if (_viewer.MasterPageNumber == 8)
                    {
                        LogComment("Test Passed, Test executed the GoToPage command correctly MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test Failed, Test did not execute GoToPage command correctly MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Fail;
                    }
                    break;

                case "LastPage":
                    if ((_viewer.MasterPageNumber == _viewer.PageCount) && (_viewer.CanGoToNextPage == false))
                    {
                        LogComment("Test Passed, Test executed the LastPage command correctly ; CanGoToPreviousPage : " + _viewer.CanGoToPreviousPage + " / CanGoToNextPage: " + _viewer.CanGoToNextPage + " / MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test Failed, Test did not execute LastPage command correctly; CanGoToPreviousPage : " + _viewer.CanGoToPreviousPage + " / CanGoToNextPage: " + _viewer.CanGoToNextPage + " / MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Fail;
                    }
                    break;

                case "PreviousPage":
                    if (_viewer.MasterPageNumber == ((_viewer.PageCount) - 3))
                    {
                        LogComment("Test Passed, Test executed the PreviousPage command correctly MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test Failed, Test did not execute PreviousPage command correctly MasterPageNumber : " + _viewer.MasterPageNumber);
                        Log.Result = TestResult.Fail;
                    }
                    break;

                default:
                    Status("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
