// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Windows.Markup;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using MTI = Microsoft.Test.Input;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>    
    /// Verifies that hyperlinks in RichTextBoxes navigate when Ctrl+clicked on.
    /// </summary>   
    [Test(1, "Part1.RegressionTests", "HyperlinkInRichTextBox ", MethodName = "Run")]    
    public class HyperlinkInRichTextBoxTests : AvalonTest
    {
        private NavigationWindow _navWin;               
        private const string navigateSource = "SimpleNavigation.xaml";
        private string _viewerType;

        [Variation("FlowDocumentReader")]
        [Variation("FlowDocumentPageViewer")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("DocumentViewer")]
        public HyperlinkInRichTextBoxTests(string viewerType)
            : base()
        {
            this._viewerType = viewerType;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunClickHyperlinkTest);
            RunSteps += new TestStep(VerifyNavigation);            
        }

        /// <summary>
        /// Creates content for the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {            
            _navWin = new NavigationWindow();                      
            _navWin.Content = (FrameworkElement)XamlReader.Load(File.OpenRead("HyperlinkInRichTextBox_" + _viewerType + ".xaml"));
            _navWin.Topmost = true;
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
        /// Hold down Crtl and click the Hyperlink to trigger navigation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunClickHyperlinkTest()
        {
            Grid mainGrid = _navWin.Content as Grid;
            TextBlock testLink = mainGrid.FindName("testLink") as TextBlock;

            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, true);
            MTI.Input.MoveToAndClick(testLink);           
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, false);

            return TestResult.Pass;
        }
       
        /// <summary>
        /// Verify that navigation has occurred by checking the source of the NavigationWindow.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyNavigation()
        {            
            if(!HyperlinkCommon.NavigationWindowSourceContainsString(_navWin, navigateSource))
            {
                //It is possible that navigation has not finished; so we wait.
                WaitFor(2000);
                if (!HyperlinkCommon.NavigationWindowSourceContainsString(_navWin, navigateSource))
                {
                    return TestResult.Fail;
                }
            }
            return TestResult.Pass;
        }       
    }
}
