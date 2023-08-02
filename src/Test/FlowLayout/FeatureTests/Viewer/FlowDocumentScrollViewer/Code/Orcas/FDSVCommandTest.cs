// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Input;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing commands in FlowDocumentScrollViewer - Indic   
    /// </summary>
    [Test(0, "Viewer.FlowDocumentScrollViewer", "FDSVCommandIndic", MethodName = "Run")]
    public class FDSVCommandTestIndic : AvalonTest
    {
        #region Test case members
        private FlowDocumentScrollViewer _viewer;
        private Window _testWin;
        private string _inputXaml;
        private string _inputString = "";
        #endregion

        #region Test variations
        [Variation("FlowDocumentScrollViewerIndic", "ZoomInCommand")]
        [Variation("FlowDocumentScrollViewerIndic", "ZoomOutCommand")]
        #endregion

        #region Constructor       
        public FDSVCommandTestIndic(string xamlFile, string testValue)
            : base()
        {
            _inputXaml = xamlFile + ".xaml";
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);           
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
            Status("Initialize ....");
            _viewer = (FlowDocumentScrollViewer)XamlReader.Load(File.OpenRead(_inputXaml));
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
            double zoom = -1;
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "zoomincommand":
                    while (_viewer.CanIncreaseZoom)
                    {
                        zoom = _viewer.Zoom;
                        NavigationCommands.IncreaseZoom.Execute(null, _viewer);
                        Status(_viewer.Zoom.ToString() + " , " + _viewer.MaxZoom.ToString() + " ; ");
                        WaitForPriority(DispatcherPriority.ApplicationIdle);

                        if (zoom >= _viewer.Zoom)
                        {
                            TestLog.Current.LogEvidence("Test Failed: ZoomIn Command did not work.");
                            TestLog.Current.LogEvidence(string.Format("Last zoom: {0} Current zoom: {1}", zoom, _viewer.Zoom));
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                    break;
                case "zoomoutcommand":
                    while (_viewer.CanDecreaseZoom)
                    {
                        zoom = _viewer.Zoom;
                        NavigationCommands.DecreaseZoom.Execute(null, _viewer);
                        Status(_viewer.Zoom.ToString() + " , " + _viewer.MinZoom.ToString() + " ; ");
                        WaitForPriority(DispatcherPriority.ApplicationIdle);

                        if (zoom <= _viewer.Zoom)
                        {
                            TestLog.Current.LogEvidence("Test Failed: ZoomOut Command did not work.");
                            TestLog.Current.LogEvidence(string.Format("Last zoom: {0} Current zoom: {1}", zoom, _viewer.Zoom));
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                    break;
                default:
                    TestLog.Current.LogEvidence("Error !!! SettingTestCases: Failure to match the argument. ");
                    TestLog.Current.Result = TestResult.Fail;
                    break;
            }
            return TestResult.Pass;
        }
        #endregion              
    }
}
