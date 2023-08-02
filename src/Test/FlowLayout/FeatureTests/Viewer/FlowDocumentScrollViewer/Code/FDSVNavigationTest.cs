// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Globalization;
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
    /// <area>Viewer.FlowDocumentPageViewer</area>
    /// <owner>Microsoft</owner>
    /// <priority>0</priority>
    /// <description>
    /// Testing Navigation in FlowDocumentScrollViewer.
    /// </description>
    /// </summary>
    [Test(0, "Viewer.FlowDocumentScrollViewer", "FDSVNavigationTest", MethodName = "Run")]
    public class FDSVNavigationTest : WindowTest
    {       
        private FlowDocumentScrollViewer _viewer;
        private NavigationWindow _navWin;
        private string _inputXaml;
        private string _inputString = "";        
        private ScrollViewer _sv;
                    
        [Variation("FlowDocumentScrollViewer.xaml", "NavigateHyperlink")]
        [Variation("FlowDocumentScrollViewer.xaml", "BringIntoView")]
        public FDSVNavigationTest(string xamlFile, string testValue)
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
            bool testCaseSet = true;
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "navigatehyperlink":
                    // find the hyperlink
                    Hyperlink hl = LogicalTreeHelper.FindLogicalNode(_viewer.Document, "hlink") as Hyperlink;
                    if (hl == null)
                    {
                        LogComment("Test failed: no hlink hyper link found in LogicalChildren");
                        Log.Result = TestResult.Fail;
                        break;
                    }

                    // invoke the hyperlink
                    hl.DoClick();

                    WaitForPriority(DispatcherPriority.ApplicationIdle);

                    // scroll viewer should have scrolled to the position that the hyperlink refers
                    _sv = FindScrollViewer(_viewer);
                    if (_sv.VerticalOffset == 0)
                    {
                        LogComment("Test failed to navigate to the hyper link.");
                        Log.Result = TestResult.Fail;
                    }
                    else
                    {
                        LogComment("Test passed.");
                        Log.Result = TestResult.Pass;
                    }

                    break;
                case "bringintoview":
                    // find the paragraph
                    Paragraph para = LogicalTreeHelper.FindLogicalNode(_viewer.Document, "tablepara") as Paragraph;
                    if (para == null)
                    {
                        LogComment("Test failed: tablepara not found in LogicalChildren");
                        Log.Result = TestResult.Fail;
                        break;
                    }

                    // bring it to view
                    para.BringIntoView();

                    WaitForPriority(DispatcherPriority.ApplicationIdle);

                    // scroll viewer should have scrolled to the position where the paragraph is located.
                    _sv = FindScrollViewer(_viewer);
                    if (_sv.VerticalOffset == 0)
                    {
                        LogComment("Test failed to bring paragraph to view.");
                        Log.Result = TestResult.Fail;
                    }
                    else
                    {
                        LogComment("Test passed.");
                        Log.Result = TestResult.Pass;
                    }
                    break;
                default:
                    Status("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    testCaseSet = false;
                    break;
            }
            
            if (testCaseSet)
            {
                if (Log.Result == TestResult.Pass)
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
            else
            {
                return TestResult.Fail;
            }
        }

        // find the scroll viewer from its visual tree
        private static ScrollViewer FindScrollViewer(FrameworkElement fe)
        {
            return (ScrollViewer)LayoutUtility.GetChildFromVisualTree(fe, typeof(ScrollViewer));
        }
    }
}
