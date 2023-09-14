// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Interop;
using System.Windows.Input;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing hyperlink browser navigation   
    /// </summary>    
    [Test(2, "Hyperlink", "HyperlinkBrowserNavigation", MethodName = "Run")]
    public class HyperlinkBrowserNavigationTests : AvalonTest
    {
        #region Test case members

        private NavigationWindow _navWin;        
        private Hyperlink _h1;
        private Hyperlink _h2;
        private Hyperlink _h3;
        private Hyperlink _linkIntoView;
        private FlowDocumentReader _fdReader = null;          
        private string _loadFile;
        private IEnumerator _links;       
        private string _windowSource = "none";
        private int _pgNum;       
        private bool _showNavUI;       
        private bool _flowDocumentAsRoot;
        private int _testToRun;
        private System.Drawing.Bitmap _master;       
        private List<string> _actions = new List<string>();       

        #endregion

        #region Constructor
        
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 1, true, true)]
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 1, false, true)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 1, true, false)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 1, false, false)]
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 2, true, true)]
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 2, false, true)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 2, true, false)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 2, false, false)]
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 3, true, true)]
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 3, false, true)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 3, true, false)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 3, false, false)]
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 4, true, true)]
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 4, false, true)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 4, true, false)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 4, false, false)]
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 5, true, true)]
        [Variation("NavJournaling_Paginated_FDRoot.xaml", 5, false, true)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 5, true, false)]
        [Variation("NavJournaling_Paginated_NotRoot.xaml", 5, false, false)]
        [Variation("NavJournaling_Bottomless_FDRoot.xaml", 6, false, true)]
        [Variation("NavJournaling_Bottomless_NotRoot.xaml", 6, false, false)]
        [Variation("NavJournaling_Bottomless_FDRoot.xaml", 7, false, true)]
        [Variation("NavJournaling_Bottomless_NotRoot.xaml", 7, false, false)]
        [Variation("NavJournaling_Bottomless_FDRoot2.xaml", 8, false, true)]
        [Variation("NavJournaling_Bottomless_NotRoot2.xaml", 8, false, false)]
        [Variation("NavJournaling_StartPage.xaml ", 9, true, true)]
        [Variation("NavJournaling_StartPage.xaml", 9, false, true)]        
        public HyperlinkBrowserNavigationTests(string testXaml, int testToRun, bool showNavUI, bool flowDocumentAsRoot)
            : base()
        {          
            this._flowDocumentAsRoot = flowDocumentAsRoot;
            this._showNavUI = showNavUI;
            this._testToRun = testToRun;
            _loadFile = testXaml;
                              
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);             
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {          
            _navWin = new NavigationWindow();
            _navWin.Top = 0;
            _navWin.Left = 0;
            _navWin.Width = 800;
            _navWin.Height = 600;

            if (!_showNavUI)
            {
                _navWin.ShowsNavigationUI = false;
            }

            _navWin.Topmost = true;
            _navWin.Show();
            _navWin.Navigate(new Uri(System.IO.Path.Combine(System.Environment.CurrentDirectory, _loadFile)));

            WaitFor(1000); //Wait for app to navigate

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
            SetUpActions(_testToRun);

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            DoActions();

            return TestResult.Pass;
        }
       
        #endregion

        #region Methods        
       
        private TestResult DoActions()
        {
            TestResult actionResult = TestResult.Unknown;

            foreach (string action in _actions)
            {
                Status("Executing command: " + action);               
                WaitFor(800);
                switch (action)
                {
                    case "nav_Link":
                        {
                            if (_links.MoveNext())
                            {
                                Hyperlink hl = _links.Current as Hyperlink;
                                hl.DoClick();                                                               
                            }
                            break;
                        }

                    case "nav_Back":
                        {
                            Status("NavigationWindow.CanGoBack = " + _navWin.CanGoBack);
                            _navWin.GoBack();
                            break;
                        }

                    case "nav_Forward":
                        {
                            _navWin.GoForward();
                            break;
                        }

                    case "next_Page":
                        {
                            NextPage();                                                   
                            break;
                        }

                    case "get_Master":
                        {
                            _master = CaptureScreen();                            
                            break;
                        }

                    case "verify_Visual":
                        {
                            System.Drawing.Bitmap compare = CaptureScreen();
                            actionResult = CompareImage(_master, compare);                            
                            break;
                        }

                    case "set_ExpectedAPIValues":
                        {
                            SetExpectedAPIValues();                           
                            break;
                        }

                    case "verify_API":
                        {
                            actionResult = VerifyNavTestByAPI();                            
                            break;
                        }

                    case "hl_BringIntoView":
                        {
                            _linkIntoView.BringIntoView();                                                   
                            break;
                        }
                }
            }
            return actionResult;
        }
                     
        private TestResult VerifyNavTestByAPI()
        {           
            if (_flowDocumentAsRoot)
            {
                if (_windowSource != "none" && _windowSource.ToLower(new CultureInfo("en-US", true)) != _navWin.Source.ToString().ToLower(new CultureInfo("en-US", true)))
                {
                    TestLog.Current.LogEvidence("Did not get the expected value for NavigationWindow.Source");
                    TestLog.Current.LogEvidence("Got: " + _navWin.Source.ToString().ToLower(new CultureInfo("en-US", true)) + " Expected: " + _windowSource);
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            else
            {
                GetFlowDocumentReader(_navWin, ref _fdReader);

                if (_fdReader == null)
                {
                    TestLog.Current.LogEvidence("There is not a FlowDocumentReader in the tree of the DependencyObject provided");
                    TestLog.Current.Result = TestResult.Fail;
                }
                else
                {
                    if (_pgNum != _fdReader.PageNumber)
                    {
                        TestLog.Current.LogEvidence("Did not get the expected value for FlowDocumentReader.PageNumber");
                        TestLog.Current.LogEvidence("Got: " + _fdReader.PageNumber + " Expected: " + _pgNum);
                        TestLog.Current.Result = TestResult.Fail;
                    }                    
                }
            }
            return TestResult.Pass;
        }

        private TestResult CompareImage(Bitmap first, Bitmap second)
        {           
            Status("Comparing captures");
            ImageComparator comparator = new ImageComparator();
            comparator.ChannelsInUse = ChannelCompareMode.ARGB;

            IImageAdapter firstCapture = new ImageAdapter(first);
            IImageAdapter secondCapture = new ImageAdapter(second);
            bool exactMatch = comparator.Compare(firstCapture, secondCapture, true);

            if (!exactMatch)
            {
                TestLog.Current.Result = TestResult.Fail;
                TestLog.Current.LogEvidence("Visual comparison Failed!");                
                LogComment("Dumping captures to log");
                first.Save("FirstCapture.tif", System.Drawing.Imaging.ImageFormat.Tiff);
                GlobalLog.LogFile("FirstCapture.tif");
                second.Save("SecondCapture.tif", System.Drawing.Imaging.ImageFormat.Tiff);
                GlobalLog.LogFile("SecondCapture.tif");
            }
            return TestResult.Pass;
        }

        private Bitmap CaptureScreen()
        {
            Bitmap capture;
            IntPtr hwnd = IntPtr.Zero;
            WindowInteropHelper iwh = new WindowInteropHelper(_navWin);

            if (iwh == null)
            {
                throw new ApplicationException("Could not find the hwnd of the main window");
            }

            hwnd = iwh.Handle;
            Status("Screen being captured");
            capture = ImageUtility.CaptureScreen(hwnd, true);

            return capture;
        }

        private void SetExpectedAPIValues()
        {
            _fdReader = null;
            GetFlowDocumentReader(_navWin, ref _fdReader);
            if (_fdReader == null)
            {
                Status("There is not a FlowDocumentReader in the tree of the DependencyObject provided");
            }
            else
            {
                _pgNum = _fdReader.PageNumber;
                _windowSource = _navWin.Source.ToString();
            }
        }

        private void NextPage()
        {
            _fdReader = null;
            GetFlowDocumentReader(_navWin, ref _fdReader);
            if (_fdReader == null)
            {
                Status("There is not a FlowDocumentReader in the tree of the DependencyObject provided");
            }
            else
            {
                NavigationCommands.NextPage.Execute(null, _fdReader);
            }
        }

        private void GetFlowDocumentReader(DependencyObject o, ref FlowDocumentReader fdReader)
        {
            GetFlowDocumentReader(o, ref fdReader, 0);
        }

        private void GetFlowDocumentReader(DependencyObject o, ref FlowDocumentReader fdReader, int indent)
        {
            DependencyObject obj;

            string indentStr = new string(' ', indent);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                obj = VisualTreeHelper.GetChild(o, i);
                if (obj != null)
                {                    
                    if (obj is FlowDocumentReader)
                    {
                        fdReader = obj as FlowDocumentReader;
                        return;
                    }
                    GetFlowDocumentReader(obj as DependencyObject, ref fdReader, indent + 2);
                }
            }
        }      

        private TestResult SetUpActions(int test)
        {          
            switch (test)
            {
                case 1: //Navigate 2 link intra doc, nav to html, nav 1 Back
                    {                        
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Link");
                        _actions.Add("set_ExpectedAPIValues");
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Back");
                        _actions.Add("verify_API");
                        break;
                    }

                case 2:  //Navigate 2 link intra doc, nav to html, nav 3 Back 
                    {                        
                        _actions.Add("hl_BringIntoView");
                        if (_flowDocumentAsRoot)
                        {
                            _actions.Add("set_ExpectedAPIValues");
                        }
                        _actions.Add("nav_Link");
                        if (!_flowDocumentAsRoot)
                        {
                            _actions.Add("set_ExpectedAPIValues");
                        }
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Back");
                        _actions.Add("nav_Back");
                        _actions.Add("nav_Back");
                        _actions.Add("verify_API");
                        break;
                    }

                case 3:  //Navigate 2 link intra doc, nav 2 Back 
                    {                        
                        _actions.Add("hl_BringIntoView");
                        if (_flowDocumentAsRoot)
                        {
                            _actions.Add("set_ExpectedAPIValues");
                        }
                        _actions.Add("nav_Link");
                        if (!_flowDocumentAsRoot)
                        {
                            _actions.Add("set_ExpectedAPIValues");
                        }
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Back");
                        _actions.Add("nav_Back");
                        _actions.Add("verify_API");
                        break;
                    }

                case 4:  //Navigate 2 link intra doc, nav to html, 1 Back, 1 Forward 
                    {
                        string path = DriverState.ExecutionDirectory;// System.Environment.CurrentDirectory;
                        string[] pathParts = path.Split('\\');

                        _windowSource = "file:///";
                        for (int i = 0; i < pathParts.Length; i++)
                        {
                            _windowSource += string.Format("{0}/", pathParts[i]);
                        }
                        _windowSource += "html.html";                        
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Back");
                        _actions.Add("nav_Forward");
                        _flowDocumentAsRoot = true;
                        _actions.Add("verify_API");

                        // This works around a random crashing 
                        CleanUpSteps += new TestStep(HyperlinkBrowserNavigationTests_CleanUpSteps);
                        break;
                    }

                case 5:  //Navigate 2 link intra doc, nav to html, 1 Back, 1 Forward, 1 Back 
                    {                        
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Link");
                        _actions.Add("set_ExpectedAPIValues");
                        _actions.Add("nav_Link");
                        _actions.Add("nav_Back");
                        _actions.Add("nav_Forward");
                        _actions.Add("nav_Back");
                        _actions.Add("verify_API");
                        break;
                    }

                case 6:  //Nav in Bottomless - 2 link intra doc 1 Back
                    {
                        _fdReader = null;
                        GetFlowDocumentReader(_navWin, ref _fdReader);
                        if (_fdReader != null)
                        {
                            _fdReader.ViewingMode = FlowDocumentReaderViewingMode.Scroll;                            
                            _actions.Add("nav_Link");
                            _actions.Add("get_Master");
                            _actions.Add("nav_Link");
                            _actions.Add("nav_Back");
                            _actions.Add("verify_Visual");
                        }
                        else
                        {
                            TestLog.Current.Result = TestResult.Fail;
                            TestLog.Current.LogEvidence("There is not a FlowDocumentReader in the tree of the DependencyObject provided");
                        }
                        break;
                    }

                case 7:  //Nav in Bottomless - 2 link intra doc, nav to html, 1 Back
                    {
                        _fdReader = null;
                        GetFlowDocumentReader(_navWin, ref _fdReader);
                        if (_fdReader != null)
                        {
                            _fdReader.ViewingMode = FlowDocumentReaderViewingMode.Scroll;                            
                            _actions.Add("nav_Link");
                            _actions.Add("nav_Link");
                            _actions.Add("get_Master");
                            _actions.Add("nav_Link");
                            _actions.Add("nav_Back");
                            _actions.Add("verify_Visual");
                        }
                        else
                        {
                            TestLog.Current.Result = TestResult.Fail;
                            TestLog.Current.LogEvidence("There is not a FlowDocumentReader in the tree of the DependencyObject provided");
                        }
                        break;
                    }

                case 8:  //Nav in Bottomless - 2 link intra doc, 2 Back
                    {
                        _fdReader = null;
                        GetFlowDocumentReader(_navWin, ref _fdReader);
                        if (_fdReader != null)
                        {
                            _fdReader.ViewingMode = FlowDocumentReaderViewingMode.Scroll;                            
                            _actions.Add("hl_BringIntoView");
                            if (_flowDocumentAsRoot)
                            {
                                _actions.Add("get_Master");
                            }
                            _actions.Add("nav_Link");
                            if (!_flowDocumentAsRoot)
                            {
                                _actions.Add("get_Master");
                            }
                            _actions.Add("nav_Link");
                            _actions.Add("nav_Link");
                            _actions.Add("nav_Back");
                            _actions.Add("nav_Back");
                            _actions.Add("nav_Back");
                            _actions.Add("verify_Visual");
                        }
                        else
                        {
                            TestLog.Current.Result = TestResult.Fail;
                            TestLog.Current.LogEvidence("There is not a FlowDocumentReader in the tree of the DependencyObject provided");
                        }
                        break;
                    }

                case 9:  //Nav to document, change page, nav back, nav forward
                    {                        
                        _actions.Add("nav_Link");
                        _actions.Add("next_Page");
                        _actions.Add("next_Page");
                        _actions.Add("set_ExpectedAPIValues");
                        _actions.Add("nav_Back");
                        _actions.Add("nav_Forward");
                        _actions.Add("verify_API");
                        break;
                    }
            }

            SetHyperlinkList();
            _linkIntoView = _h1;
           
            return TestResult.Pass;
        }
      
        private void SetHyperlinkList()
        {
            List<Hyperlink> hlinks = new List<Hyperlink>();
            _h1 = LogicalTreeHelper.FindLogicalNode(_navWin, "h1") as Hyperlink;
            hlinks.Add(_h1);
            _h2 = LogicalTreeHelper.FindLogicalNode(_navWin, "h2") as Hyperlink;
            hlinks.Add(_h2);
            _h3 = LogicalTreeHelper.FindLogicalNode(_navWin, "h3") as Hyperlink;
            hlinks.Add(_h3);

            _links = hlinks.GetEnumerator();
        }

        private TestResult HyperlinkBrowserNavigationTests_CleanUpSteps()
        {
            _navWin.Navigate(new Uri(System.IO.Path.Combine(System.Environment.CurrentDirectory, _loadFile)));
            WaitFor(800);
            return TestResult.Pass;
        }

        #endregion
    }
}
