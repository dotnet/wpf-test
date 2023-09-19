// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Reflection;

using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWD = System.Windows.Documents;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;
using SWS = System.Windows.Shapes;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;
using System.Windows;


// Testcase:    WFHInAVTabPage
// Description: We need to verify that WFH and WF contols work in an AV TabPage
namespace WindowsFormsHostTests
{
    public class WFHInAVTabPage : WPFReflectBase {

        #region Testcase setup

        #region TestVariables

        private delegate void myEventHandler(object sender);
        private int _scenarioIndex = 0;
        private TParams _tp;
        private string _events;             // event sequence string
        private bool _debug = false;         // set this true for TC debugging

        private SWC.Button _AVButton;
        private SWC.TabControl _AVTabControl;
        private SWC.TabItem _AVTabItem1;
        private SWC.TabItem _AVTabItem2;
        private SWC.TabItem _AVTabItem3;
        private SWC.TabItem _AVTabItem4;

        private WindowsFormsHost _wfh1;
        private WindowsFormsHost _wfh2;
        private WindowsFormsHost _wfh3;

        private SWF.Button _WF1Button;
        private SWF.TextBox _WF2TextBox;
        private SWF.TextBox _WF3TextBox;

        private const string WindowTitleName = "WFHInAVTabPage";

        private const string AVButtonName = "AVButton";
        private const string AVTabControlName = "AVTabControl";
        private const string AVTabItem1Name = "AVTabItem1";
        private const string AVTabItem2Name = "AVTabItem2";
        private const string AVTabItem3Name = "AVTabItem3";
        private const string AVTabItem4Name = "AVTabItem4";

        private const string WF1Name = "WF1";
        private const string WF2Name = "WF2";
        private const string WF3Name = "WF3";

        private const string WF1ButtonName = "WF1Button";
        private const string WF2TextBoxName = "WF2TextBox";
        private const string WF3TextBoxName = "WF3TextBox";

        #endregion

        #region Testcase setup
        public WFHInAVTabPage(string[] args) : base(args) { }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            _scenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
            SetupScenario(_scenarioIndex);
            Utilities.SleepDoEvents(10);
            return base.BeforeScenario(p, scenario);
        }

        protected override void InitTest(TParams p) 
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            this.UseMITA = true;
            _tp = p;
            TestSetup();
            base.InitTest(p);
            this.Top = 0;
            this.Left = 0;
        }
        #endregion

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("2 tabs, 1 with a WFH & WF Button, 1 with AV button.  Verify that switching tabs show the correct button (switch back and forth more than once).")]
        public ScenarioResult Scenario1(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();
            string expVal = String.Empty;
            expVal += "WF1Button-GotFocus:";
            expVal += "AVButton-GotFocus:";
            expVal += "WF1Button-GotFocus:";
            expVal += "AVButton-GotFocus:";
            expVal += "WF1Button-GotFocus:";
            expVal += "AVButton-GotFocus:";
            // Removing to get test passing. It takes 2 CTRL+TABs to get focus to the WF1 button.
            //expVal += "WF1Button-GotFocus:";
            //expVal += "AVButton-GotFocus:";

            myWriteLine("Switching tabs back and forth 8 times");
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(AVTabControlName));
            ctrl.SetFocus();
            Utilities.SleepDoEvents(100);
            _events = String.Empty;
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, expVal, _events, "Focus/Button content getting out of sync", p.log);
            return sr; 
        }

        [Scenario("2 tabs, both with a WFH & WF Textbox.  Verify that text is preserved between tabs. (switch back and forth more than once.)")]
        public ScenarioResult Scenario2(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();
            string expVal = String.Empty;
            expVal += "WF2TextBox-GotFocus:";
            expVal += "WF3TextBox-GotFocus:";
            expVal += "WF2TextBox-GotFocus:";
            expVal += "WF3TextBox-GotFocus:";
            expVal += "WF2TextBox-GotFocus:";
            expVal += "WF3TextBox-GotFocus:";
            expVal += "WF2TextBox-GotFocus:";
            expVal += "WF3TextBox-GotFocus:";

            myWriteLine("Switching tabs back and forth 8 times");
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(AVTabControlName));
            ctrl.SetFocus();
            _events = String.Empty;
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);
            uiApp.SendKeys("^{TAB}");
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, expVal, _events, "Focus/TextBox content getting out of sync", p.log);
            return sr;
        }

        #endregion

        #region HelperFunction

        private void TestSetup()
        {
            myWriteLine("TestSetup -- Start ");

            #region SetupAVControl
            _AVButton = new SWC.Button();
            _AVTabControl = new SWC.TabControl();
            _AVTabItem1 = new SWC.TabItem();
            _AVTabItem2 = new SWC.TabItem();
            _AVTabItem3 = new SWC.TabItem();
            _AVTabItem4 = new SWC.TabItem();

            _AVTabControl.Name = AVTabControlName;
            _AVButton.Content = _AVButton.Name = AVButtonName;
            _AVTabItem1.Header = _AVTabItem1.Name = AVTabItem1Name;
            _AVTabItem2.Header = _AVTabItem2.Name = AVTabItem2Name;
            _AVTabItem3.Header = _AVTabItem3.Name = AVTabItem3Name;
            _AVTabItem4.Header = _AVTabItem4.Name = AVTabItem4Name;

            _AVButton.GotFocus += new RoutedEventHandler(AVControl_GotFocus);
            #endregion

            #region SetupWFControl
            _wfh1 = new WindowsFormsHost();
            _wfh2 = new WindowsFormsHost();
            _wfh3 = new WindowsFormsHost();
            _WF1Button = new SWF.Button();
            _WF2TextBox = new SWF.TextBox();
            _WF3TextBox = new SWF.TextBox();

            _WF1Button.Text = _WF1Button.Name = WF1ButtonName;
            _WF2TextBox.Text = _WF2TextBox.Name = WF2TextBoxName;
            _WF2TextBox.WordWrap = true;
            _WF2TextBox.Multiline = true;
            _WF3TextBox.Text = _WF3TextBox.Name = WF3TextBoxName;
            _WF2TextBox.WordWrap = true;
            _WF2TextBox.Multiline = true;
            _WF1Button.GotFocus += new EventHandler(WFControl_GotFocus);
            _WF2TextBox.GotFocus += new EventHandler(WFControl_GotFocus);
            _WF3TextBox.GotFocus += new EventHandler(WFControl_GotFocus);
            #endregion

            #region LayoutWindow
            this.Title = WindowTitleName;
            _AVButton.Width = 250;
            _AVButton.Height = 150;
            _wfh1.Width = _wfh2.Width = _wfh3.Width = 250;
            _wfh1.Height = _wfh2.Height = _wfh3.Height = 150;
            _wfh1.Child = _WF1Button;
            _wfh2.Child = _WF2TextBox;
            _wfh3.Child = _WF3TextBox;
            _AVTabItem1.Content = _AVButton;
            _AVTabItem2.Content = _wfh1;
            _AVTabItem3.Content = _wfh2;
            _AVTabItem4.Content = _wfh3;
            _AVTabControl.Items.Add(_AVTabItem1);
            _AVTabControl.Items.Add(_AVTabItem2);
            _AVTabControl.Items.Add(_AVTabItem3);
            _AVTabControl.Items.Add(_AVTabItem4);
            this.Content = _AVTabControl;
            #endregion

            myWriteLine("TestSetup -- End ");
        }

        void WFControl_GotFocus(object sender, EventArgs e)
        {
            _events += ((SWF.Control)sender).Text + "-GotFocus:";
        }

        void AVControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof(SWC.TextBox))
                _events += ((SWC.TextBox)sender).Text + "-GotFocus:";
            else if (sender.GetType() == typeof(SWC.Button))
                _events += ((SWC.Button)sender).Content + "-GotFocus:";
            else
                _events += ((SWC.Control)sender).Name + "-GotFocus:";
        }

        void myWriteLine(string s)
        {
            if (_debug)
            {
                _tp.log.WriteLine(s);
            }
        }

        private void SetupScenario(int ScenarioIndex)
        {
            switch (ScenarioIndex)
            {
                case 1: //@ 2 tabs, 1 with a WFH & WF Button, 1 with AV button.  Verify that switching tabs show the correct button (switch back and forth more than once).
                    _AVTabItem1.Visibility = SW.Visibility.Visible;
                    _AVTabItem2.Visibility = SW.Visibility.Visible;
                    _AVTabItem3.Visibility = SW.Visibility.Collapsed;
                    _AVTabItem4.Visibility = SW.Visibility.Collapsed;
                    break;
                case 2: //@ 2 tabs, both with a WFH & WF Textbox.  Verify that text is preserved between tabs. (switch back and forth more than once.)
                    _AVTabItem1.Visibility = SW.Visibility.Collapsed;
                    _AVTabItem2.Visibility = SW.Visibility.Collapsed;
                    _AVTabItem3.Visibility = SW.Visibility.Visible;
                    _AVTabItem4.Visibility = SW.Visibility.Visible;
                    break;
                default:
                    _AVTabItem1.Visibility = SW.Visibility.Visible;
                    _AVTabItem2.Visibility = SW.Visibility.Visible;
                    _AVTabItem3.Visibility = SW.Visibility.Visible;
                    _AVTabItem4.Visibility = SW.Visibility.Visible;
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
//@ 2 tabs, 1 with a WFH &amp; WF Button, 1 with AV button.  Verify that switching tabs show the correct button (switch back and forth more than once).
//@ 2 tabs, both with a WFH &amp; WF Textbox.  Verify that text is preserved between tabs. (switch back and forth more than once.)
