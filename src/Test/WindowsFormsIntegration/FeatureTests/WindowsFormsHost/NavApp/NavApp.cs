// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Reflection;
using System.Threading;

using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWD = System.Windows.Documents;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;
using SWS = System.Windows.Shapes;
using SWN = System.Windows.Navigation;
using SWT = System.Windows.Threading;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;


// Testcase:    NavApp
// Description: Verify that dialog keys (i.e. Enter, ESC) work for Modal and non Modal WF
namespace WindowsFormsHostTests
{
    public class NavApp : WPFReflectBase
    {
        #region TestVariables

        private delegate void myEventHandler(object sender);

        private int _scenarioIndex = 0;
        private bool _debug = false;         // set this true for TC debugging
        private TParams _tp;
        private string _events = String.Empty;
        private UIObject _uiApp;
        private bool _bCanGoBack = false;
        private bool _bCanGoForward = false;
        private string _currentPage = String.Empty;
        private enum DispatchFunction { GetCurrentPage, CanGoBack, CanGoForward };


        private SWN.NavigationWindow _AVNavigationWindow;
        private SWC.Page _AVPage1;
        private SWC.Page _AVPage2;
        private SWC.StackPanel _page1AVStackPanel;
        private SWC.StackPanel _page2AVStackPanel;
        private SWC.Button _page1AVButton;
        private SWC.Button _page2AVButton;
        private SWC.TextBox _page1AVTextBox;
        private SWC.TextBox _page2AVTextBox;

        private WindowsFormsHost _page1wfh1;
        private WindowsFormsHost _page1wfh2;
        private WindowsFormsHost _page2wfh1;
        private WindowsFormsHost _page2wfh2;
        private SWF.Button _page1WF1Button;
        private SWF.TextBox _page1WF2TextBox;
        private SWF.Button _page2WF1Button;
        private SWF.TextBox _page2WF2TextBox;

        private const string WindowTitleName = "NavApp";
        private const string AVNavigationWindowName = "AVNavigationWindow";
        private const string AVPage1Name = "AVPage1";
        private const string AVPage2Name = "AVPage2";
        private const string Page1AVStackPanelName = "Page1AVStackPanel";
        private const string Page2AVStackPanelName = "Page2AVStackPanel";
        private const string Page1AVButtonName = "Page1AVButton";
        private const string Page2AVButtonName = "Page2AVButton";
        private const string Page1AVTextBoxName = "Page1AVTextBox";
        private const string Page2AVTextBoxName = "Page2AVTextBox";

        private const string Page1wfh1Name = "Page1wfh1";
        private const string Page1wfh2Name = "Page1wfh2";
        private const string Page2wfh1Name = "Page2wfh1";
        private const string Page2wfh2Name = "Page2wfh2";

        private const string Page1WF1ButtonName = "Page1WF1Button";
        private const string Page1WF2TextBoxName = "Page1WF2TextBox";

        private const string Page2WF1ButtonName = "Page2WF1Button";
        private const string Page2WF2TextBoxName = "Page2WF2TextBox";

        private const string TBValidationMsg = "TextBox Validation is being called";

        #endregion

        #region Testcase setup
        public NavApp(string[] args) : base(args) { }


        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            _scenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
            ScenarioSetup(_scenarioIndex);
        Utilities.SleepDoEvents(10);
            return base.BeforeScenario(p, scenario);
        }

        protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
        {
            _AVNavigationWindow.Close();
            base.AfterScenario(p, scenario, result);
        }
        protected override void InitTest(TParams p)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            this.Visibility = System.Windows.Visibility.Hidden;
            _tp = p;
            this.UseMITA = true;
            base.InitTest(p);
        }
        #endregion


        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Navigate to and from a page that has a WFH + WF Control and verify that the control works")]
        public ScenarioResult Scenario1(TParams p) 
        {
            String expVal = String.Empty;
            _events = String.Empty;
            ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(10);
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AVNavigationWindowName));
            UIObject ctrlForward = _uiApp.Descendants.Find(UICondition.CreateFromId("BrowseForward"));
            UIObject ctrlBackward = _uiApp.Descendants.Find(UICondition.CreateFromId("BrowseBack"));
            UIObject ctrlAVBtn1 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page1AVButtonName));
            UIObject ctrlWFBtn1 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page1WF1ButtonName));
        Utilities.SleepDoEvents(10);
            myWriteLine("both Back and Forward arrows should be disabled initially");
            WPFMiscUtils.IncCounters(sr, p.log, ctrlBackward.IsEnabled == false, "Back button is enabled");
            WPFMiscUtils.IncCounters(sr, p.log, ctrlForward.IsEnabled==false, "Forward button is enabled");

            myWriteLine("click on Page1AVButton to bring up the second page");
            ctrlAVBtn1.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(20);

            WPFMiscUtils.IncCounters(sr, p.log, ctrlBackward.IsEnabled == true, "Back button does not get enabled");
            WPFMiscUtils.IncCounters(sr, p.log, ctrlForward.IsEnabled==false, "Forward button does not get disabled");

            UIObject ctrlWFBtn2 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page2WF1ButtonName));
            UIObject ctrlAVBtn2 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page2AVButtonName));

            myWriteLine("click forward and backward and click on WF button 5 times");
            for (int i = 0; i < 5; i++)
            {
                myWriteLine("Backward");
                ctrlBackward.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
                expVal += AVPage1Name + "::";
                _events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
                Utilities.SleepDoEvents(5);

                myWriteLine("Click on the WF button - should be on Page 1"); // should be on Page1 at all time
                expVal += Page1WF1ButtonName + "::";
                if (ctrlWFBtn1.IsOffscreen == false)
                {
                    ctrlWFBtn1.Click(PointerButtons.Primary);
                    Utilities.SleepDoEvents(5);
                }
                if (ctrlWFBtn2.IsOffscreen == false)
                {
                    ctrlWFBtn2.Click(PointerButtons.Primary);
                    Utilities.SleepDoEvents(5);
                }
                
                myWriteLine("Forward");
                ctrlForward.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
                expVal += AVPage2Name + "::";
                _events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
                Utilities.SleepDoEvents(5);

                myWriteLine("Click on the WF button - should be on Page 2"); // should be on Page2 at all time
                expVal += Page2WF1ButtonName + "::"; 

                if (ctrlWFBtn1.IsOffscreen == false)
                {
                    ctrlWFBtn1.Click(PointerButtons.Primary);
                    Utilities.SleepDoEvents(5);
                }
                if (ctrlWFBtn2.IsOffscreen == false)
                {
                    ctrlWFBtn2.Click(PointerButtons.Primary);
                    Utilities.SleepDoEvents(5);
                }

            }
            //p.log.LogKnownBug(BugDb.WindowsOSBugs, 29, "Navigation Arrows are disabled after focus change from AV control to WF control and then click on the Left/Right Navigation arrow");
            //p.log.LogKnownBug(BugDb.WindowsOSBugs, 30, "Navigation arrows are not enabled when the navigation is initiated from a WFH control while the focus is on a AV control");
            WPFMiscUtils.IncCounters(sr, expVal, _events, "Page displayed not in the right order", p.log);
            return sr;
        }

        [Scenario("Verify that Text in a WF Textbox on Page1 remians when navigating to page 2 and back.")]
        public ScenarioResult Scenario2(TParams p) 
        {
            String expVal = String.Empty;
            _events = String.Empty;
            ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(5);
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AVNavigationWindowName));
            UIObject ctrlForward = _uiApp.Descendants.Find(UICondition.CreateFromId("BrowseForward"));
            UIObject ctrlBackward = _uiApp.Descendants.Find(UICondition.CreateFromId("BrowseBack"));
            UIObject ctrlAVBtn1 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page1AVButtonName));
            UIObject ctrlWFtb = _uiApp.Descendants.Find(UICondition.CreateFromId(Page1WF2TextBoxName));
            MitaControl.Edit editWFtb= new MitaControl.Edit(ctrlWFtb);

            myWriteLine("click on Page1AVButton to bring up the second page");
            ctrlAVBtn1.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);

            myWriteLine("Go Back to Page1");
            ctrlBackward.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);

            editWFtb.SetValue(Page1WF2TextBoxName + Page1WF2TextBoxName);
            Utilities.SleepDoEvents(5);

            myWriteLine("Go to Page 2 the back to Page1");
            ctrlForward.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);
            ctrlBackward.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);

            myWriteLine("Check on the Text in the WF TextBox");
            WPFMiscUtils.IncCounters(sr, Page1WF2TextBoxName + Page1WF2TextBoxName, editWFtb.Value, "WF TextBox content is not being preserved", p.log);
            return sr;
        }

        [Scenario("WF Validation on a WF Text box in an AV Page.  Navigate to another page and verify Validation doesn't break the app (we don't expect validation to work here).")]
        public ScenarioResult Scenario3(TParams p) 
        {
            String expVal = String.Empty;
            _events = String.Empty;
            ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(5);
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AVNavigationWindowName));
            UIObject ctrlForward = _uiApp.Descendants.Find(UICondition.CreateFromId("BrowseForward"));
            UIObject ctrlBackward = _uiApp.Descendants.Find(UICondition.CreateFromId("BrowseBack"));
            UIObject ctrlAVBtn1 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page1AVButtonName));
            UIObject ctrlWFtb = _uiApp.Descendants.Find(UICondition.CreateFromId(Page1WF2TextBoxName));
            MitaControl.Edit editWFtb = new MitaControl.Edit(ctrlWFtb);

            ctrlWFtb.SendKeys(Page1WF2TextBoxName + Page1WF2TextBoxName);
            Utilities.SleepDoEvents(5);
            ctrlWFtb.Click(PointerButtons.Primary);

            myWriteLine("click on Page1AVButton to bring up the second page");
            ctrlAVBtn1.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);

            myWriteLine("Go Back to Page1");
            ctrlBackward.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);
            
            myWriteLine("Check on the Text in the WF TextBox");
            WPFMiscUtils.IncCounters(sr, Page1WF2TextBoxName + Page1WF2TextBoxName, editWFtb.Value, "WF TextBox content is not being preserved", p.log);

            myWriteLine("Make sure TB Validation being called once");
            WPFMiscUtils.IncCounters(sr, TBValidationMsg, _events, "WF TextBox Validating event not being called", p.log);

            return sr;
        }

        [Scenario("Navigate to another page form a WF control in a WFH.")]
        public ScenarioResult Scenario4(TParams p) 
        {
            String expVal = String.Empty;
            _events = String.Empty;
            ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(5);
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AVNavigationWindowName));
            UIObject ctrlForward = _uiApp.Descendants.Find(UICondition.CreateFromId("BrowseForward"));
            UIObject ctrlBackward = _uiApp.Descendants.Find(UICondition.CreateFromId("BrowseBack"));
            UIObject ctrlAVBtn1 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page1AVButtonName));
            UIObject ctrlWFBtn1 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page1WF1ButtonName));

            myWriteLine("both Back and Forward arrows should be disabled initially");
            WPFMiscUtils.IncCounters(sr, p.log, ctrlBackward.IsEnabled == false, "Back button is enabled");
            WPFMiscUtils.IncCounters(sr, p.log, ctrlForward.IsEnabled == false, "Forward button is enabled");

            myWriteLine("click on Page1AVButton to bring up the second page");
            ctrlWFBtn1.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);
            expVal += AVPage2Name + "::";
            _events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
            Utilities.SleepDoEvents(5);
            WPFMiscUtils.IncCounters(sr, p.log, ctrlBackward.IsEnabled == true, "Back button does not get enabled");
            WPFMiscUtils.IncCounters(sr, p.log, ctrlForward.IsEnabled == false, "Forward button does not get disabled");

            UIObject ctrlWFBtn2 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page2WF1ButtonName));
            UIObject ctrlAVBtn2 = _uiApp.Descendants.Find(UICondition.CreateFromId(Page2AVButtonName));

            myWriteLine("click forward and backward and click on AV button 5 times");
            for (int i = 0; i < 5; i++)
            {
                myWriteLine("Backward");
                ctrlBackward.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
                expVal += AVPage1Name + "::";
                _events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
                Utilities.SleepDoEvents(5);

                myWriteLine("Click on the AV button - should be on Page 1"); // should be on Page1 at all time
                expVal += Page1AVButtonName + "::";
                if (ctrlAVBtn1.IsOffscreen == false)
                {
                    ctrlAVBtn1.Click(PointerButtons.Primary);
                    Utilities.SleepDoEvents(5);
                }
                if (ctrlAVBtn2.IsOffscreen == false)
                {
                    ctrlAVBtn2.Click(PointerButtons.Primary);
                    Utilities.SleepDoEvents(5);
                }

                myWriteLine("Forward");
                ctrlForward.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
                expVal += AVPage2Name + "::";
                _events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
                Utilities.SleepDoEvents(5);

                myWriteLine("Click on the AV button - should be on Page 2"); // should be on Page2 at all time
                expVal += Page2AVButtonName + "::";

                if (ctrlAVBtn1.IsOffscreen == false)
                {
                    ctrlAVBtn1.Click(PointerButtons.Primary);
                    Utilities.SleepDoEvents(5);
                }
                if (ctrlAVBtn2.IsOffscreen == false)
                {
                    ctrlAVBtn2.Click(PointerButtons.Primary);
                    Utilities.SleepDoEvents(5);
                }
            }
            //p.log.LogKnownBug(BugDb.WindowsOSBugs, 29, "Navigation Arrows are disabled after focus change from AV control to WF control and then click on the Left/Right Navigation arrow");
            //p.log.LogKnownBug(BugDb.WindowsOSBugs, 30, "Navigation arrows are not enabled when the navigation is initiated from a WFH control while the focus is on a AV control");
            WPFMiscUtils.IncCounters(sr, expVal, _events, "Page displayed not in the right order", p.log);
            return sr;
        }

        #endregion


        #region HelperFunction

        private void TestSetup()
        {
            myWriteLine("TestSetup -- Start ");

            #region SetupAVControl
            _AVNavigationWindow = new System.Windows.Navigation.NavigationWindow();
            _AVPage1 = new System.Windows.Controls.Page();
            _AVPage2 = new System.Windows.Controls.Page();
            _page1AVStackPanel = new System.Windows.Controls.StackPanel();
            _page2AVStackPanel = new System.Windows.Controls.StackPanel();
            _page1AVButton = new System.Windows.Controls.Button();
            _page2AVButton = new System.Windows.Controls.Button();
            _page1AVTextBox = new System.Windows.Controls.TextBox();
            _page2AVTextBox = new System.Windows.Controls.TextBox();

            _AVNavigationWindow.Title = _AVNavigationWindow.Name = AVNavigationWindowName;
            _AVPage1.Name = _AVPage1.Title = AVPage1Name;
            _AVPage2.Name = _AVPage2.Title = AVPage2Name;
            _page1AVStackPanel.Name = Page1AVStackPanelName;
            _page2AVStackPanel.Name = Page2AVStackPanelName;
            _page1AVButton.Content = _page1AVButton.Name = Page1AVButtonName;
            _page2AVButton.Content = _page2AVButton.Name = Page2AVButtonName;
            _page1AVTextBox.Text = _page1AVTextBox.Name = Page1AVTextBoxName;
            _page2AVTextBox.Text = _page2AVTextBox.Name = Page2AVTextBoxName;
            _page1AVTextBox.Width = _page2AVTextBox.Width = 300;
            _page1AVTextBox.Height = _page2AVTextBox.Height = 150;
            #endregion

            #region SetupWFControl
            _page1wfh1 = new WindowsFormsHost();
            _page1wfh2 = new WindowsFormsHost();
            _page2wfh1 = new WindowsFormsHost();
            _page2wfh2 = new WindowsFormsHost();

            _page1WF1Button = new System.Windows.Forms.Button();
            _page2WF1Button = new System.Windows.Forms.Button();
            _page1WF2TextBox = new System.Windows.Forms.TextBox();
            _page2WF2TextBox = new System.Windows.Forms.TextBox();

            _page1wfh1.Name = Page1wfh1Name;
            _page1wfh2.Name = Page1wfh2Name;
            _page2wfh1.Name = Page2wfh1Name;
            _page2wfh2.Name = Page2wfh2Name;
            _page1WF1Button.Text = _page1WF1Button.Name = Page1WF1ButtonName;
            _page2WF1Button.Text = _page2WF1Button.Name = Page2WF1ButtonName;
            _page1WF2TextBox.Text = _page1WF2TextBox.Name = Page1WF2TextBoxName;
            _page2WF2TextBox.Text = _page2WF2TextBox.Name = Page2WF2TextBoxName;

            _page1wfh1.Child = _page1WF1Button;
            _page2wfh1.Child = _page2WF1Button;
            _page1wfh2.Child = _page1WF2TextBox;
            _page2wfh2.Child = _page2WF2TextBox;

            _page1wfh1.Width = _page2wfh1.Width = _page1wfh2.Width = _page2wfh2.Width = 300;
            _page1wfh1.Height = _page2wfh1.Height = _page1wfh2.Height = _page2wfh2.Height = 200;
            #endregion

            #region LayoutWindow
            _page1AVStackPanel.Children.Add(_page1wfh1);
            _page1AVStackPanel.Children.Add(_page1wfh2);
            _page1AVStackPanel.Children.Add(_page1AVTextBox);
            _page1AVStackPanel.Children.Add(_page1AVButton);

            _page2AVStackPanel.Children.Add(_page2wfh1);
            _page2AVStackPanel.Children.Add(_page2wfh2);
            _page2AVStackPanel.Children.Add(_page2AVTextBox);
            _page2AVStackPanel.Children.Add(_page2AVButton);

            _AVPage1.Content = _page1AVStackPanel;
            _AVPage2.Content = _page2AVStackPanel;

            _AVNavigationWindow.Content = _AVPage1;
            _AVNavigationWindow.ShowsNavigationUI = true;
            _AVNavigationWindow.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            _AVNavigationWindow.Show();
            #endregion

            myWriteLine("TestSetup -- End ");
        }

        #region EventHandler
        void WFButton_Click(object sender, EventArgs e)
        {
            _events += ((SWF.Button)sender).Name + "::";
        }

        void Page1WF1Button_Click(object sender, EventArgs e)
        {
            _AVNavigationWindow.Content = _AVPage2;
        }

        void AVButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _events += ((SWC.Button)sender).Name + "::";
        }

        void Page1AVButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _AVNavigationWindow.Content = _AVPage2;
        }

        void Page1WF2TextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _events += TBValidationMsg;
            this.Title = _events;
        }
        #endregion

        void CanGoBack(object sender)
        {
            _bCanGoBack = ((SWN.NavigationWindow)sender).CanGoBack;
        }

        void CanGoForward(object sender)
        {
            _bCanGoForward = ((SWN.NavigationWindow)sender).CanGoForward;
        }

        void GetCurrentPage(object sender)
        {
            SWC.Page pg = ((SWN.NavigationWindow)sender).Content as SWC.Page;
            _currentPage = pg.Name.ToString();
        }

        object myDispatcher(DispatchFunction df)
        {
            object retObj = null;
            switch (df)
            {
                case DispatchFunction.GetCurrentPage:
                    _AVNavigationWindow.Dispatcher.Invoke(SWT.DispatcherPriority.Normal, new myEventHandler(GetCurrentPage), _AVNavigationWindow);
                    retObj = _currentPage;
                    break;
                case DispatchFunction.CanGoBack:
                    _AVNavigationWindow.Dispatcher.Invoke(SWT.DispatcherPriority.Normal, new myEventHandler(CanGoBack), _AVNavigationWindow);
                    retObj = _bCanGoBack;
                    break;
                case DispatchFunction.CanGoForward:
                    _AVNavigationWindow.Dispatcher.Invoke(SWT.DispatcherPriority.Normal, new myEventHandler(CanGoForward), _AVNavigationWindow);
                    retObj = _bCanGoForward;
                    break;
            }
            return retObj;
        }

        void myWriteLine(string s)
        {
            if (_debug)
            {
                myWriteLine(s);
            }
        }

        private void ScenarioSetup(int ScenarioIndex)
        {
            switch (ScenarioIndex)
            {
                case 1: //@ Navigate to and from a page that has a WFH + WF Control and verify that the control works
                    TestSetup();
                    _page1AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    _page2AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    _page1AVButton.Visibility = System.Windows.Visibility.Visible;
                    _page2AVButton.Visibility = System.Windows.Visibility.Visible;
                    _page1wfh1.Visibility = System.Windows.Visibility.Visible;
                    _page2wfh1.Visibility = System.Windows.Visibility.Visible;
                    _page1wfh2.Visibility = System.Windows.Visibility.Collapsed;
                    _page2wfh2.Visibility = System.Windows.Visibility.Collapsed;

                    _page1WF1Button.Click += new EventHandler(WFButton_Click);
                    _page2WF1Button.Click += new EventHandler(WFButton_Click);
                    _page1AVButton.Click += new System.Windows.RoutedEventHandler(Page1AVButton_Click);
                    _page2AVButton.Click += new System.Windows.RoutedEventHandler(AVButton_Click);
                    break;
                case 2: //@ Verify that Text in a WF Textbox on Page1 remians when navigating to page 2 and back.
                    TestSetup();
                    _page1AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    _page2AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    _page1wfh1.Visibility = System.Windows.Visibility.Collapsed;
                    _page2wfh1.Visibility = System.Windows.Visibility.Collapsed;
                    _page1wfh2.Visibility = System.Windows.Visibility.Visible;
                    _page2wfh2.Visibility = System.Windows.Visibility.Visible;
                    _page1AVButton.Click += new System.Windows.RoutedEventHandler(Page1AVButton_Click);
                    break;
                case 3: //@ WF Validation on a WF Text box in an AV Page.  Navigate to another page and verify Validation doesn't break the app (we don't expect validation to work here).
                    TestSetup();
                    _page1AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    _page2AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    _page1wfh1.Visibility = System.Windows.Visibility.Collapsed;
                    _page2wfh1.Visibility = System.Windows.Visibility.Collapsed;
                    _page1wfh2.Visibility = System.Windows.Visibility.Visible;
                    _page2wfh2.Visibility = System.Windows.Visibility.Visible;
                    _page1AVButton.Click += new System.Windows.RoutedEventHandler(Page1AVButton_Click);
                    _page1WF2TextBox.Validating += new System.ComponentModel.CancelEventHandler(Page1WF2TextBox_Validating);
                    break;
                case 4: //@ Verify that Text in a WF Textbox on Page1 remians when navigating to page 2 and back.
                    TestSetup();
                    _page1AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    _page2AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    _page1AVButton.Visibility = System.Windows.Visibility.Visible;
                    _page2AVButton.Visibility = System.Windows.Visibility.Visible;
                    _page1wfh1.Visibility = System.Windows.Visibility.Visible;
                    _page2wfh1.Visibility = System.Windows.Visibility.Visible;
                    _page1wfh2.Visibility = System.Windows.Visibility.Collapsed;
                    _page2wfh2.Visibility = System.Windows.Visibility.Collapsed;

                    _page1WF1Button.Click += new EventHandler(Page1WF1Button_Click); // click to show the second page
                    _page2WF1Button.Click += new EventHandler(WFButton_Click); // click to show the second page
                    _page1AVButton.Click += new System.Windows.RoutedEventHandler(AVButton_Click);
                    _page2AVButton.Click += new System.Windows.RoutedEventHandler(AVButton_Click);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Navigate to and from a page that has a WFH + WF Control and verify that the control works

//@ Verify that Text in a WF Textbox on Page1 remians when navigating to page 2 and back.

//@ WF Validation on a WF Text box in an AV Page.  Navigate to another page and verify Validation doesn't break the app (we don't expect validation to work here).

//@ Navigate to another page form a WF control in a WFH.
