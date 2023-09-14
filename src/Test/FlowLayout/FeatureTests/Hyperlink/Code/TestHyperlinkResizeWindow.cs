// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test reflow of Hyperlink on Window Resize
//
//				 Tests (different verification methods):
//						1.  "VisualFlow": Uses Vscan to visually verify the reflow.
//						2.  "Navigation": Verifies that the link is still functional after reflow (clicked in "new" space occupied.)
//				                                
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
using System;
using System.Globalization;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;

using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Hyperlink resize window.   
    /// </summary>
    [Test(2, "Hyperlink", "HyperlinkReflowOnResizeWindow", MethodName = "Run")]
    public class TestHyperlinkReflowOnResizeWindow : AvalonTest
    {
        #region Test case members

        private NavigationWindow _navWin;
        private Hyperlink _hl;
        private FlowDocumentScrollViewer _eRoot;
        private string _inputString;
        private string _navFile = "SimpleNavigation.xaml";
        private bool _result;       

        #endregion

        #region Constructor

        [Variation("VisualFlow")]
        [Variation("Navigation")]

        public TestHyperlinkReflowOnResizeWindow(string testValue)
            : base()
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
            RunSteps += new TestStep(VerifyTest);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            _navWin = new NavigationWindow();
            //Removing Navigation UI so we don't need different masters for different themes
            _navWin.ShowsNavigationUI = false;

            Status("Initialize");
            Paragraph para = new Paragraph();

            _eRoot = new FlowDocumentScrollViewer();
            _eRoot.Document = new FlowDocument(para);

            _hl = new Hyperlink();
            string uri = System.IO.Path.Combine(Environment.CurrentDirectory, _navFile);
            _hl.NavigateUri = new Uri(uri);
            _hl.FontSize = 15;
            _hl.FontFamily = new FontFamily("Tahoma");
            _hl.Inlines.Clear();
            _hl.Inlines.Add(new Run("This is a Hyperlink that flows across several lines.  This test resizes the windows to make sure that the Hyperlink adjusts apporopriately."));

            para.Inlines.Add(_hl);

            _navWin.Content = _eRoot;           
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 800;
            _navWin.Height = 600;
            _navWin.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());            
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
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Content rendered");
            
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            ResizeWindow();

            return TestResult.Pass;
        }

        /// <summary>
        /// VerifyTest: Verifies the test result
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            TestResult tempResult = TestResult.Unknown;
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "visualflow":
                    tempResult = VisualFlowVerification();
                    break;
                case "navigation":
                    Status("Clicking on the Hyperlink...");
                    Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
                    Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
                    //pause for a second and then verify test.  (This makes sure content has rendered first.)
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    tempResult = NavigationVerification();
                    break;
                default:
                    LogComment("Invalid entry");
                    break;
            }
            return tempResult;
        }

        #endregion

        #region Methods

        private void ResizeWindow()
        {
            //Resize window to trigger reflowing of Hyperlink
            _navWin.Width = 200;
            WaitFor(1000);
            //Move the mouse to where we will click on the link.
            Status("Hovering over the Hyperlink...");
            UserInput.MouseButton(_eRoot, 70, 95, "Move");
        }

        private TestResult VisualFlowVerification()
        {
            Status("Using VScan to verify...");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            
            string masterName = Common.ResolveName(this);
            masterName += _inputString;
            
            VScanCommon tool = new VScanCommon(_navWin, masterName, "FeatureTests\\FlowLayout\\Masters\\VSCAN");

            if (tool.CompareImage())
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        private TestResult NavigationVerification()
        {
            Status("Checking to see if navigation was successful...");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            string source = _navWin.Source.ToString();
            string[] file = source.Split('/');
            foreach (string s in file)
            {
                if (s == _navFile) { _result = true; }
            }

            if (_result)
            {
                Status("avigation has succeeded!  Test has passed!");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Test has failed!!");
                LogComment("NavigationWindow.Source after navigation: " + _navWin.Source.ToString());
                LogComment("Expected: " + _navFile);
                return TestResult.Fail;
            }
        }
        #endregion
    }
}
