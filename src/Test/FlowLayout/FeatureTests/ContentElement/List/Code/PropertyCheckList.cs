// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Markup;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing commands in FlowDocumentPageViewer.   
    /// </summary>
    [Test(0, "List", "PropertyCheckListTest", MethodName = "Run")]
    public class PropertyCheckListTest : AvalonTest
    {
        #region Test case members

        private FlowDocumentScrollViewer _eRoot;
        private Window _w;
        private string _inputXaml;
        private int _inputID;
        
        #endregion

        #region Constructor

        [Variation("BVT_PropertyCheck_List.xaml", 1)]
        [Variation("BVT_PropertyCheck_List.xaml", 2)]
        [Variation("BVT_PropertyCheck_List.xaml", 3)]
        [Variation("BVT_PropertyCheck_List.xaml", 4)]
        public PropertyCheckListTest(string xamlFile, int testID)
            : base()
        {
            CreateLog = false;
            _inputXaml = xamlFile;
            _inputID = testID;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        #endregion

        #region Test Steps
       
        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {            
            _w = new Window();
                       
            _eRoot = (FlowDocumentScrollViewer)XamlReader.Load(File.OpenRead(_inputXaml));
            _w.Content = _eRoot;
            _w.Width = 800;
            _w.Height = 600;
            _w.Left = 0;
            _w.Top = 0;
            _w.Topmost = true;
            _w.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            switch (_inputID)
            {
                case 1: VerifyListPropertyValues("List1_Default", TextMarkerStyle.Disc, -1, 1);
                    break;
                case 2: VerifyListPropertyValues("List2_MarkerStyle", TextMarkerStyle.Square, -1, 1);
                    break;
                case 3: VerifyListPropertyValues("List3_StartIndex", TextMarkerStyle.Decimal, -1, 5);
                    break;
                case 4: VerifyListPropertyValues("List4_MarkerOffset", TextMarkerStyle.Disc, 22, 1);
                    break;
            }
            return TestResult.Pass;
        }

        #endregion

        private void VerifyListPropertyValues(string id, TextMarkerStyle marker, double offset, int startIndex)
        {
            List testList = LogicalTreeHelper.FindLogicalNode(_eRoot, id) as List;
            
            // Verify Marker
            TestLog log = new TestLog("Verify MarkerStyle");
            if (testList.MarkerStyle != marker)
            {
                log.LogEvidence("Failed: " + id + " MarkerStyle is incorrect.  Expected " + marker + ", Received " + testList.MarkerStyle);
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            // Verify Offset
            log = new TestLog("Verify MarkerOffset");
            if (offset > 0)
            {
                if (testList.MarkerOffset != offset)
                {
                    log.LogEvidence("Failed: " + id + " MarkerOffset is incorrect.  Expected " + offset + ", Received " + testList.MarkerOffset);
                    log.Result = TestResult.Fail;
                }
                else
                {
                    log.Result = TestResult.Pass;
                }

            }
            else
            {
                if (!double.IsNaN(testList.MarkerOffset))
                {
                    log.LogEvidence("Failed: " + id + " MarkerOffset is incorrect.  Expected double.Nan, Received " + testList.MarkerOffset);
                    log.Result = TestResult.Fail;
                }
                else
                {
                    log.Result = TestResult.Pass;
                }
            }
            log.Close();

            // Verify StartIndex
            log = new TestLog("Verify StartIndex");
            if (testList.StartIndex != startIndex)
            {
                log.LogEvidence("Failed: " + id + " StartIndex is incorrect.  Expected " + startIndex + ", Received " + testList.StartIndex.ToString());
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            // Verify Padding
            log = new TestLog("Verify Padding");
            if ((testList.Padding.ToString() != "Auto,Auto,Auto,Auto") && (testList.Padding.ToString() != "Auto;Auto;Auto;Auto"))
            {
                log.LogEvidence("Failed: " + id + " Padding is incorrect.  Expected Auto,Auto,Auto,Auto or Auto;Auto;Auto;Auto, Received " + testList.Padding);
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
        }
    }
}
