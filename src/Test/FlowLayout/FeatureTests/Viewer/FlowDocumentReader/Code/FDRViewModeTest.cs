// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Globalization;
using System.Collections.Generic;
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
    /// <area>Viewer.FlowDocumentReader</area>
    /// <owner>Microsoft</owner>
    /// <priority>2</priority>
    /// <description>
    /// Testing ViewMode feature in FlowDocumentReader.
    /// </description>
    /// </summary>
    [Test(0, "Viewer.FlowDocumentReader", "ViewModeTest", MethodName = "Run")]
    public class ViewModeTest : AvalonTest
    {
        private FlowDocumentReader _viewer;
        private FlowDocument _fd;
        private NavigationWindow _navWin;
        private string _inputXaml;
        private string _inputString = "";
        private string _viewingMode = "";
        private FrameworkElement _element;
        private int _COUNT_OF_VIEWS = 3;

        [Variation("FlowDocumentSample1x.xaml", "ViewingMode", "Page")]
        [Variation("FlowDocumentSample1x.xaml", "ViewingMode", "TwoPage")]
        [Variation("FlowDocumentSample1x.xaml", "ViewingMode", "Scroll")]
        [Variation("FlowDocumentSample1x.xaml", "SwitchMode", "Page")]
        public ViewModeTest(string xamlFile, string testValue, string testViewMode)
            : base()
        {
            _inputXaml = xamlFile;
            _inputString = testValue;
            _viewingMode = testViewMode;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentReader and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
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
                    Status("Error !!! SettingViewMode: Unexpected failure to match the argument. ");
                    return TestResult.Fail;
            }
            _element = _viewer;
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
        /// RunTests: runs the tests where we test the Find functionality.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTest()
        {
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "switchmode":
                    FrameworkElement swtch = TestHelperW.FindSwitchViewingModeVisual(_viewer) as FrameworkElement;
                    if (swtch == null)
                    {
                        LogComment("Test failed: cannot find switch view mode visual");
                        Log.Result = TestResult.Fail;
                        break;
                    }

                    FlowDocumentReaderViewingMode origMode = _viewer.ViewingMode;
                    List<FlowDocumentReaderViewingMode> echoedModes = new List<FlowDocumentReaderViewingMode>();
                    for (int i = 0; i < _COUNT_OF_VIEWS; ++i)
                    {
                        FlowDocumentReader.SwitchViewingModeCommand.Execute(null, _viewer);
                        Status(_viewer.ViewingMode.ToString());
                        WaitForPriority(DispatcherPriority.ApplicationIdle);

                        echoedModes.Add(origMode);
                        if (origMode == _viewer.ViewingMode)
                        {
                            LogComment("Test failed: ViewingMode is not switched correctly");
                            Log.Result = TestResult.Fail;
                            break;
                        }
                        origMode = _viewer.ViewingMode;
                        if (!TestHelperW.HasView(_viewer, _viewer.ViewingMode))
                        {
                            LogComment("Test failed: view not dislplayed correctly");
                            Log.Result = TestResult.Fail;
                            break;
                        }
                    }

                    if (echoedModes.Count != _COUNT_OF_VIEWS)
                    {
                        LogComment("Test failed: ViewingMode is switched to all views");
                        Log.Result = TestResult.Fail;
                        break;
                    }

                    LogComment("Test Passed");
                    Log.Result = TestResult.Pass;
                    break;

                case "viewingmode":
                    if (TestHelperW.HasView(_viewer, _viewer.ViewingMode))
                    {
                        LogComment("Test Passed");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test failed: ViewMode is not consistent with view");
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
