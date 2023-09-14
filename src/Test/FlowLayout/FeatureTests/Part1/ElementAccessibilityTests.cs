// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Navigation;

using Microsoft.Test.TestTypes;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.Layout;
using Microsoft.Test.Layout.VisualScan;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Part1.RegressionTest</area>
    /// <owner>Microsoft</owner>
    /// <priority>1</priority>
    /// <description>
    /// Test element accessibility in a FlowDocumentScrollViewer when IsSelectionEnabled=False
    /// This is regression coverage for Part1 Regression_Bug48
    /// These tests verify:
    /// 1. That elements can receive focus and be acted upon in a FlowDocumentScrollViewer with IsSelectionEnabled==false.
    /// 2. That an element in a FlowDocumentScrollViewer where IsSelectionEnabled==false gets a focus rect.
    /// </description>
    /// </summary>
    [Test(0, "Part1.RegressionTests", "ElementAccessibility", MethodName = "Run")]    
    public class ElementAccessibilityRegressionTest : AvalonTest
    {
        private NavigationWindow _navWin;        
        private Hyperlink _hl;
        private const string navigateSource = "SimpleNavigation.xaml";         
        private string _testName;

        [Variation("ElementFocus")]
        [Variation("FocusRect")]
        public ElementAccessibilityRegressionTest(string testName)
            : base()
        {
            this._testName = testName;            
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);

            if (testName == "ElementFocus")
            {
                RunSteps += new TestStep(RunElementFocusTest);
                RunSteps += new TestStep(VerifyNavigation);
            }
            else
            {
                RunSteps += new TestStep(RunFocusVisualTest);
                RunSteps += new TestStep(VerifyFocusVisual);
            }
        }

        /// <summary>
        /// Creates content for the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {            
            _hl = new Hyperlink(new Run("Click Me!"));
            _hl.NavigateUri = new Uri(System.IO.Path.Combine(System.Environment.CurrentDirectory, navigateSource));
           
            Paragraph p = new Paragraph();           

            if (_testName == "ElementFocus")
            {
                Hyperlink hl2 = new Hyperlink(new Run("Hyperlink 2"));
                Hyperlink hl3 = new Hyperlink(new Run("Hyperlink 3"));
                Button btn = new Button();
                btn.Content = "Button";
                p.Inlines.Add(hl2);
                p.Inlines.Add(btn);
                p.Inlines.Add(hl3);
            }

            p.Inlines.Add(_hl);
            
            FlowDocument fd = new FlowDocument();
            fd.Blocks.Add(p);

            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.IsSelectionEnabled = false;
            fdsv.Document = fd;

            _navWin = new NavigationWindow();
            _navWin.Height = 500;
            _navWin.Width = 500;
            _navWin.ShowsNavigationUI = false;
            _navWin.Content = fdsv;
            _navWin.Topmost = true;
            _navWin.Show();

            WaitForPriority(DispatcherPriority.ApplicationIdle);             
            MTI.Input.MoveToAndClick(_navWin);
               
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _navWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// Bring focus to the TextBox on the page, press Tab to navigate to the Hyperlink, press Enter to navigate the Hyperlink.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunElementFocusTest()
        {                      
            //Press tab 5 times to get to the last hyperlink
            PressTab(5);
           
            //Hyperlink hl should have focus
            if (!_hl.IsFocused)
            {
                TestLog.Current.LogEvidence("The hyperlink in this test should have focus, but it does not!");
                return TestResult.Fail;
            }
          
            //Press enter to navigate the Hyperlink
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Enter, true);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Enter, false);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
          
            return TestResult.Pass;
        }

        /// <summary>
        /// Bring focus to the Hyperlink on the page.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunFocusVisualTest()
        {            
            //Press tab 2 times to bring the Focus Rect to the Hyperlink.
            PressTab(2);

            //The Hyperlink should have focus
            if (!_hl.IsFocused)
            {
                TestLog.Current.LogEvidence("The hyperlink in this test should have focus, but it does not!");
                return TestResult.Fail;
            }           
            return TestResult.Pass;
        }
       
        /// <summary>
        /// Verify that a focus visual appears around the hl by image comparison.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyFocusVisual()
        {            
            string masterName = Common.ResolveName(this);
            masterName += _testName;
            VScanCommon tool = new VScanCommon(_navWin, masterName, "FeatureTests\\FlowLayout\\Masters\\VSCAN");           

            if (!tool.CompareImage())
            {
                TestLog.Current.LogEvidence("Visual Verification of the Focus Rect around the Hyperlink failed!");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that navigation has occurred by checking the source of the NavigationWindow.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyNavigation()
        {
            if (!HyperlinkCommon.NavigationWindowSourceContainsString(_navWin, navigateSource))
            {
                //It is possible that navigation has not finished; so we wait.
                WaitFor(2000);
                if (HyperlinkCommon.NavigationWindowSourceContainsString(_navWin, navigateSource))
                {
                    return TestResult.Fail;
                }
            }
            return TestResult.Pass;     
        }

        /// <summary>
        /// Presses tab any number of times.
        /// </summary>        
        private void PressTab(int count)
        {            
            for (int i = 0; i < count; i++)
            {
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Tab, true);
                MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Tab, false);
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }
        }
    }
}
