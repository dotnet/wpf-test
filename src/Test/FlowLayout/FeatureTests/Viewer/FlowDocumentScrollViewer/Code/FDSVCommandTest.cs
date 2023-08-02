// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
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
    /// <area>Viewer.FlowDocumentPageViewer</area>
    /// <owner>Microsoft</owner>
    /// <priority>0</priority>
    /// <description>
    /// Testing commands in FlowDocumentScrollViewer.
    /// </description>
    /// </summary>
    [Test(0, "Viewer.FlowDocumentScrollViewer", "FDSVCommandsTest", MethodName = "Run")]
    public class FDSVCommandsTest : WindowTest
    {
        private FlowDocumentScrollViewer _viewer;
        private NavigationWindow _navWin;
        private string _inputXaml;
        private string _inputString = "";
        private ScrollViewer _sv;
        private const double TOLERRENCE = 0.1;
        private const double HEIGHT_TOLERRENCE = 10;

        [Variation("FlowDocumentScrollViewer.xaml", "ZoomInCommand")]
        [Variation("FlowDocumentScrollViewer.xaml", "ZoomOutCommand")]
        public FDSVCommandsTest(string xamlFile, string testValue)
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
            _viewer = (FlowDocumentScrollViewer)XamlReader.Load(File.OpenRead(_inputXaml));
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
                    _sv = FindScrollViewer(_viewer);

                    // repeat increasing zoom till zoom cannot be increased.
                    for (double origHeight = _sv.ExtentHeight, origZoom = _viewer.Zoom;
                        _viewer.CanIncreaseZoom;
                        origHeight = _sv.ExtentHeight, origZoom = _viewer.Zoom)
                    {
                        NavigationCommands.IncreaseZoom.Execute(null, _viewer);
                        WaitForPriority(DispatcherPriority.ApplicationIdle);
                        Status("Current: Height=" + _sv.ExtentHeight + " Zoom=" + _viewer.Zoom + " Orig: Height="
                            + origHeight + ", Zoom=" + origZoom);

                        // it should increase scroll viewer's extent height when zoom is increased.
                        if (_sv.ExtentHeight - origHeight < HEIGHT_TOLERRENCE)
                        {
                            LogComment("Test failed to Zoom in properly");
                            Log.Result = TestResult.Fail;
                            break;
                        }
                    }

                    // the zoom must be equal to max zoom when zoom cannot be increased.
                    if (Math.Abs(_viewer.Zoom - _viewer.MaxZoom) < TOLERRENCE)
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
                    _sv = FindScrollViewer(_viewer);

                    // repeat decreasing zoom till zoom cannot be descreased.
                    for (double origHeight = _sv.ExtentHeight, origZoom = _viewer.Zoom;
                        _viewer.CanDecreaseZoom;
                        origHeight = _sv.ExtentHeight, origZoom = _viewer.Zoom)
                    {
                        NavigationCommands.DecreaseZoom.Execute(null, _viewer);
                        WaitForPriority(DispatcherPriority.ApplicationIdle);
                        Status("Current: Height=" + _sv.ExtentHeight + " Zoom=" + _viewer.Zoom + " Orig: Height="
                            + origHeight + ", Zoom=" + origZoom);

                        // scroll viewer's extent height should be descreased when zoom gets decreased.
                        if (origHeight - _sv.ExtentHeight < HEIGHT_TOLERRENCE)
                        {
                            LogComment("Test failed to Zoom out properly");
                            Log.Result = TestResult.Fail;
                            break;
                        }
                    }

                    // the zoom must be equal to min zoom when zoom cannot be descreased.
                    if (Math.Abs(_viewer.Zoom - _viewer.MinZoom) < TOLERRENCE)
                    {
                        LogComment("Test Passed, The ZoomOut command zoomed correctly to the final zoom");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test failed to Zoom out properly to the final zoom");
                        Log.Result = TestResult.Fail;
                    }
                    break;

                default:
                    Status("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // find the scroll viewer from its visual tree
        private static ScrollViewer FindScrollViewer(FrameworkElement fe)
        {
            return (ScrollViewer)LayoutUtility.GetChildFromVisualTree(fe, typeof(ScrollViewer));
        }
    }
}
