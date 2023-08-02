// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Markup;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Layout;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing various hyperlink tests.    
    /// </summary>
    [Test(2, "Hyperlink", "HyperlinkTests", MethodName = "Run")]
    public class HyperlinkTests : AvalonTest
    {
        #region Test case members

        private NavigationWindow _navWin;
        private Canvas _eRoot;                 
        private string _navFile = "SimpleNavigation.xaml";
        private string _loadFile;      
        private bool _visualVerify = true;
        private bool _hoverOnly;
        private bool _hoverOff;       
        private bool _justLeftDown;
        private bool _bringToFocus;
        private bool _tab;
        private bool _resize;      
        private bool _frameAsNavHost = false;
        private bool _skipTypicalVerification = false;
        private string _inputString;
        private string _testType;
        
        #endregion

        #region Constructor

        [Variation("Hyperlink_ImageNavigation.xaml", "Test1", "NavTest")]
        [Variation("BasicHyperlink.xaml", "Test2", "DefaultHoverStyle")]
        [Variation("Hyperlink_VisualTrigger.xaml", "Test3", "ChangeHoverStyle_HoverOn")]     
        [Variation("Hyperlink_FocusVisuals.xaml", "Test5", "ChangeFocusVisuals")]
        [Variation("Hyperlink_MultipleLinks.xaml", "Test6", "DefaultFocusVisuals")]
        [Variation("Hyperlink_FloaterNavigation.xaml", "Test7", "NavTest")]
        [Variation("Hyperlink_FigureNavigation.xaml", "Test8", "NavTest")]
        [Variation("Hyperlink_ListNavigation.xaml", "Test9", "NavTest")]
        [Variation("Hyperlink_TableNavigation.xaml", "Test10", "NavTest")]
        [Variation("Hyperlink_RTLNavigation.xaml", "Test11", "NavTest")]               
        [Variation("Hyperlink_MultipleLinks_RTL.xaml", "Test41", "DefaultFocusVisuals")]
        [Variation("Hyperlink_ResizeVisuals.xaml", "Test42", "resizewithfocusvisuals")]
        [Variation("BasicHyperlink.xaml", "Test43", "DefaultLeftMouseButtonDown", Priority = 3)]
        [Variation("Hyperlink_VisualTrigger.xaml", "Test44", "ChangeHoverStyle_HoverOff", Priority = 3)]
        [Variation("Hyperlink_FrameNavigate.xaml", "Test45", "FrameNavigate", Priority = 3)]
        public HyperlinkTests(string testXaml, string testName, string testType)
            : base()
        {
            _loadFile = testXaml;
            _inputString = testName;
            this._testType = testType;            
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(VerifyTest);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {
            FindTestToRun();

            _navWin = new NavigationWindow();
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 800;
            _navWin.Height = 600;
            _navWin.ShowsNavigationUI = false;
            _navWin.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());
            _navWin.Topmost = true;

            _eRoot = new Canvas();
            if (!_resize)
            {
                _eRoot.Width = 400;
                _eRoot.Height = 400;
            }
            UIElement content = (UIElement)XamlReader.Load(File.OpenRead(_loadFile));
            _eRoot.Children.Add(content);

            _navWin.Show();

            //When running under Test Center, need to bring focus to the test Window (from the console window of the test harness)
            Microsoft.Test.Input.Input.MoveToAndClick(_navWin);

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            _navWin.Content = _eRoot;

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
        private TestResult RunTest()
        {
            WaitFor(1000); //Wait for content

            if (_bringToFocus)
            {
                Hyperlink testHyperlink = LogicalTreeHelper.FindLogicalNode(_navWin, "hlink") as Hyperlink;
                testHyperlink.Focus();

                if (_tab)
                {
                    System.Windows.Input.Key tabKey = System.Windows.Input.Key.Tab;
                    Status("Pressing: " + tabKey.ToString());
                    Microsoft.Test.Input.Input.SendKeyboardInput(tabKey, true);  //down
                    Microsoft.Test.Input.Input.SendKeyboardInput(tabKey, false); //up
                }

                WaitForPriority(DispatcherPriority.ApplicationIdle);
                if (_resize)
                {
                    ResizeWindow();
                }
            }
            else
            {
                //Move the mouse to where we will click on the link.
                Status("Hovering over the Hyperlink");
                UserInput.MouseButton(_eRoot, 75, 60, "Move");

                //pause for a second and then click on link.  (This makes sure content has rendered first.)
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                ClickLink();
            }
            return TestResult.Pass;
        }

        /// <summary>
        /// VerifyTest: Verifies test by visual or api verification 
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult VerifyTest()
        {            
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            if (!_skipTypicalVerification)
            {
                if (_visualVerify)
                {
                    VisualVerification();
                }
                else
                {
                    APIVerification();
                }
            }

            return TestResult.Pass;
        }

        #endregion

        #region Methods

        private void FindTestToRun()
        {            
            switch (_testType.ToLower(CultureInfo.InvariantCulture))
            {
                case "defaulthoverstyle":
                    _hoverOnly = true;
                    break;

                case "changehoverstyle_hoveron":
                    _hoverOnly = true;
                    break;

                case "changefocusvisuals":
                    _bringToFocus = true;
                    break;

                case "defaultfocusvisuals":
                    _bringToFocus = true;
                    _tab = true;
                    break;

                case "resizewithfocusvisuals":
                    _bringToFocus = true;
                    _tab = true;
                    _resize = true;
                    _skipTypicalVerification = true;
                    break;

                case "changehoverstyle_hoveroff":
                    _hoverOnly = true;
                    _hoverOff = true;
                    break;

                case "framenavigate":
                    _visualVerify = false;
                    _frameAsNavHost = true;
                    break;

                case "defaultleftmousebuttondown":
                    _justLeftDown = true;
                    break;

                case "navtest":
                    _visualVerify = false;
                    break;

                default:
                    Status("Did not recognize cmd line param.");
                    break;
            }           
        }

        private void ClickLink()
        {
            if (!_hoverOnly)
            {
                if (_justLeftDown)
                {
                    Status("Left Mouse Button Down on the Hyperlink");
                    Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
                }
                else
                {
                    Status("Clicking on the Hyperlink");
                    Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
                    Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
                }
            }

            if (_hoverOff)
            {
                Status("Moving off the Hyperlink");
                UserInput.MouseButton(_eRoot, 70, 400, "Move");
            }
        }

        private TestResult ResizeWindow()
        {
            _navWin.Width = 200;
            //If we get here w/out a crash we pass (regresses Regression_Bug51)
            Status("Test has passed!");
            return TestResult.Pass;
        }

        private TestResult APIVerification()
        {
            Status("Checking to see if navigation was successful");
            WaitFor(1000); //Wait for app to navigate

            string source = "";
            bool verificationResult = false;

            if (!_frameAsNavHost)
            {
                source = _navWin.Source.ToString();
            }
            else
            {
                Frame navFrame = LogicalTreeHelper.FindLogicalNode(_navWin, "MyFrame") as Frame;
                source = navFrame.Source.ToString();
            }

            string[] file = source.Split('/');
            foreach (string s in file)
            {
                if (s == _navFile) { verificationResult = true; }
            }

            if (!verificationResult)
            {
                TestLog.Current.Result = TestResult.Fail;
                TestLog.Current.LogEvidence("Verification has failed!!");
                TestLog.Current.LogEvidence("Navigation Source after navigation: " + source);
                TestLog.Current.LogEvidence("Expected: " + _navFile);
            }
            return TestResult.Pass;
        }

        private TestResult VisualVerification()
        {
            Status("Using VScan to verify");
            string masterName = Common.ResolveName(this);
            masterName += _inputString;
            VScanCommon tool = new VScanCommon(_navWin, masterName, "FeatureTests\\FlowLayout\\Masters\\VSCAN");
            tool.Index.AddCriteria(MasterMetadata.OsVersionDimension, 1);            

            if (!tool.CompareImage())
            {
                TestLog.Current.Result = TestResult.Fail;
                TestLog.Current.LogEvidence("Verification has failed!!");                               
            }
            return TestResult.Pass;
        }               
                     
        #endregion
    }
}
