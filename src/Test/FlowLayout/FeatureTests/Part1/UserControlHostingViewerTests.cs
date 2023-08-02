// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

using Microsoft.Test.TestTypes;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.Layout;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Regression coverage for Part1 Regression_Bug50 where the Find Dialog control is not available when a UserControl hosts a FlowDocument viewer
    /// and the UserControl Focusable property == false   
    /// </summary>
    [Test(0, "Part1.RegressionTests", "UserControlHostingViewer", MethodName = "Run")]    
    public class UserControlHostingViewer : AvalonTest
    {       
        private string _viewerType;
        private DependencyObject _flowdocumentViewer;
        private static string s_textInput = "findme";
        private Window _testWin;

        [Variation("FDR")]
        [Variation("FDSV")]
        [Variation("FDPV")]
        public UserControlHostingViewer(string viewerType)
            : base()
        {
            this._viewerType = viewerType;

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(VerifyTest);           
        }

        /// <summary>
        /// Creates content for the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {                                  
            Paragraph p = new Paragraph(new Run("Some content for this test."));           
                        
            FlowDocument fd = new FlowDocument();
            fd.Blocks.Add(p);
           
            switch(_viewerType)
            {
                case "FDR":
                    {
                        FlowDocumentReader FDR = new FlowDocumentReader();
                        FDR.Document = fd;                        
                        _flowdocumentViewer = FDR;
                        break;
                    }
                case "FDSV":
                    {
                        FlowDocumentScrollViewer FDSV = new FlowDocumentScrollViewer();
                        FDSV.Document = fd;                       
                        _flowdocumentViewer = FDSV;
                        break;
                    }
                case "FDPV":
                    {
                        FlowDocumentPageViewer FDPV = new FlowDocumentPageViewer();
                        FDPV.Document = fd;                        
                        _flowdocumentViewer = FDPV;
                        break;
                    }
            }

            UserControl userControl = new UserControl();
            userControl.Focusable = false;
            userControl.Content = _flowdocumentViewer;

            _testWin = new Window();
            _testWin.Height = 500;
            _testWin.Width = 800;
            _testWin.Left = 0;
            _testWin.Top = 0;
            _testWin.Content = userControl;
            _testWin.Topmost = true;
            _testWin.Show();

            WaitForPriority(DispatcherPriority.ApplicationIdle);             
            MTI.Input.MoveToAndClick(_testWin);
               
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// Open the Find dialod and type some text into it.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTest()
        {            
            // Invoke Find dialog;  This should also bring keyboard focus to the Find TextBox
            MTI.UserInput.KeyDown("F3");
            MTI.UserInput.KeyUp("F3");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // Type some text that should go into the Find TextBox
            TypeText(s_textInput);
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that the typed text can be found in the TextBox
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {            
            // Find the find tool bar
            FrameworkElement findToolBar = TestHelperW.FindtheFindToolBar(_flowdocumentViewer) as FrameworkElement;
            if (findToolBar == null)
            {
                return ReportMissingComponent("FlowDocumentReader's Find toolbar");
            }

            // Get the find Textbox
            TextBox findTextBox = TestHelperW.FindtheTextBox(findToolBar) as TextBox;
            if (findTextBox == null)
            {
                return ReportMissingComponent("FlowDocumentReader's Find TextBox");
            }

            // Verify that the typed text is in the Find textBox
            if (findTextBox.Text.ToLowerInvariant() != s_textInput)
            {
                TestLog.Current.LogEvidence(string.Format("The Find dialog does not contain the text that was expected! Expected: {0} Found: {1}", s_textInput, findTextBox.Text));
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Failure info.
        /// </summary>        
        private TestResult ReportMissingComponent(string expectedComponent)
        {
            TestLog.Current.LogEvidence(string.Format("Did not find an expected componet of this test! Looking for: {0}", expectedComponent));
            return TestResult.Fail;
        }

        /// <summary>
        /// Sends a string of keyboard input.
        /// </summary>        
        private void TypeText(string text)
        {
            char[] letters = text.ToCharArray();
            foreach (char letter in letters)
            {
                MTI.UserInput.KeyPress(letter.ToString().ToUpperInvariant());
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }
        }
    }
}
