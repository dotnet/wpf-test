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

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;

//
// Testcase:    Dialogkeys
// Description: Verify that dialog keys (i.e. Enter, ESC) work for Modal and non Modal WF
//
namespace WindowsFormsHostTests
{
    public class Dialogkeys : WPFReflectBase
    {
        #region TestVariables

        private delegate void myEventHandler(object sender);
        private int _scenarioIndex = 0;
        private bool _debug = false;         // set this true for TC debugging
        private TParams _tp;
        private string _events = String.Empty;
        private UIObject _uiApp;
        private SWF.DialogResult _dialogResult;

        private SWC.StackPanel _AVStackPanel;
        private SWC.Button _AVAcceptButton;
        private SWC.Button _AVCancelButton;
        private SWC.TextBox _AVTextBox;

        private WindowsFormsHost _wfh1;
        private WindowsFormsHost _wfh2;
        private SWF.Form _WF1Form;
        private SWF.FlowLayoutPanel _WF1FlowLayoutPanel;
        private SWF.FlowLayoutPanel _WF2FlowLayoutPanel;
        private SWF.Button _WF1AcceptButton;
        private SWF.Button _WF1CancelButton;
        private SWF.TextBox _WF1TextBox;
        private SWF.TextBox _WF2TextBox;
        private SWF.Button _WF2StartButton;

        // Second WF dialog
        private SWF.Form _WFDialogForm;
        private SWF.FlowLayoutPanel _WFDialogFlowLayoutPanel;
        private SWF.Button _WFDialogAcceptButton;
        private SWF.Button _WFDialogCancelButton;
        private SWF.TextBox _WFDialogTextBox;

        private const string WindowTitleName = "Dialogkeys";

        private const string WF1Name = "WFH1";
        private const string WF2Name = "WFH2";
        private const string WF1FormName = "WF1Form";
        private const string WF1FlowLayoutPanelName = "WF1FlowLayoutPanel";
        private const string WF2FlowLayoutPanelName = "WF2FlowLayoutPanel";
        private const string WF1AcceptButtonName = "WF1AcceptButton";
        private const string WF1CancelButtonName = "WF1CancelButton";
        private const string WF1TextBoxName = "WF1TextBox";
        private const string WF2TextBoxName = "WF2TextBox";
        private const string WF2StartButtonName = "WF2StartButton";

        private const string WFDialogFormName = "WFDialogForm";
        private const string WFDialogFlowLayoutPanelName = "WFDialogFlowLayoutPanel";
        private const string WFDialogAcceptButtonName = "WFDialogAcceptButton";
        private const string WFDialogCancelButtonName = "WFDialogCancelButton";
        private const string WFDialogTextBoxName = "WFDialogTextBox";

        private const string AVStackPanelName = "AVStackPanel";
        private const string AVAcceptButtonName = "AVAcceptButton";
        private const string AVCancelButtonName = "AVCancelButton";
        private const string AVTextBoxName = "AVTextBox";

        #endregion

        #region Testcase setup
        public Dialogkeys(string[] args) : base(args) { }


        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            _scenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
            Utilities.SleepDoEvents(10);
            ScenarioSetup(_scenarioIndex);
            return base.BeforeScenario(p, scenario);
        }

        protected override void InitTest(TParams p) 
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            _tp = p;
            this.UseMITA = true;
            TestSetup();
            base.InitTest(p);
            this.Top = 0;
            this.Left = 0;
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios

        [Scenario("WF control with OK button assigned running in a WFH (ENTER)")]
        public ScenarioResult Scenario1(TParams p) 
        {
            p.log.WriteLine("Set focus to the WF Text box and send in a {ENTER} keystroke - should get a click event on the AV Accept Button");
            ScenarioResult sr = new ScenarioResult();
            string expVal = "WF1AcceptButton_Click:";
            _events = String.Empty;
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBoxName));
            ctrl.SetFocus();
            ctrl.SendKeys("{ENTER}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "Accept Button not getting the click event", p.log);
            return sr;
        }    
        
        [Scenario("WF control with Cancel button assigned running in a WFH (ESC)")]
        public ScenarioResult Scenario2(TParams p) 
        {
            p.log.WriteLine("Set focus to the WF Text box and send in a {ESC} keystroke - should get a click event on the WF Cancel Button");
            ScenarioResult sr = new ScenarioResult();
            string expVal = "WF1CancelButton_Click:";
            _events = String.Empty;
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBoxName));
            ctrl.SetFocus();
            ctrl.SendKeys("{ESC}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "Cancel Button not getting the click event", p.log);
            return sr;
        } 

        [Scenario("AV control with OK button assigned with a WFH on the app with a control (ENTER)")]
        public ScenarioResult Scenario3(TParams p) 
        {
            p.log.WriteLine("Set focus to the WF Text box and send in a {ENTER} keystroke - should get a click event on the AV Accept Button");
            ScenarioResult sr = new ScenarioResult();
            string expVal = "AVAcceptButton_Click:";
            _events = String.Empty;
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF2TextBoxName));
            ctrl.SetFocus();
            ctrl.SendKeys("{ENTER}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "Accept Button not getting the click event", p.log);
            return sr;
        }

        [Scenario("AV control with Cancel button assigned with a WFH on the app with a control (CANCEL)")]
        public ScenarioResult Scenario4(TParams p) 
        {
            p.log.WriteLine("Set focus to the WF Text box and send in a {ESC} keystroke - should get a click event on the AV Cancel Button");
            ScenarioResult sr = new ScenarioResult();
            string expVal = "AVCancelButton_Click:";
            _events = String.Empty;
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF2TextBoxName));
            ctrl.SetFocus();
            ctrl.SendKeys("{ESC}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "Cancel Button not getting the click event", p.log);
            return sr;
        }

        [Scenario("Both WF and AV control with OK button assigned (ENTER)")]
        public ScenarioResult Scenario5(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();
            string expVal = String.Empty;
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrlWF = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBoxName));
            UIObject ctrlAV = _uiApp.Descendants.Find(UICondition.CreateFromId(AVTextBoxName));

            p.log.WriteLine("Set focus to the WF Text box and send in a {ENTER} keystroke - should get a click event on the WF Accept Button");
            expVal = "WF1AcceptButton_Click:";
            _events = String.Empty;
            ctrlWF.SetFocus();
            ctrlWF.SendKeys("{ENTER}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "WF Accept Button not getting the click event", p.log);

            p.log.WriteLine("Set focus to the AV TextBox and send in a {ENTER} keystroke - should get a click event on the AV Accept Button");
            expVal = "AVAcceptButton_Click:";
            _events = String.Empty;
            ctrlAV.SetFocus();
            ctrlAV.SendKeys("{ENTER}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "AV Accept Button not getting the click event", p.log);

            return sr;
        }

        [Scenario("Both WF and AV control with CANCEL button assigned (CANCEL)")]
        public ScenarioResult Scenario6(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();
            string expVal = String.Empty;
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrlWF = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBoxName));
            UIObject ctrlAV = _uiApp.Descendants.Find(UICondition.CreateFromId(AVTextBoxName));

            p.log.WriteLine("Set focus to the WF Text box and send in a {ESC} keystroke - should get a click event on the WF Cancel Button");
            expVal = "WF1CancelButton_Click:";
            _events = String.Empty;
            ctrlWF.SetFocus();
            ctrlWF.SendKeys("{ESC}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "WF Cancel Button not getting the click event", p.log);

            p.log.WriteLine("Set focus to the AV TextBox and send in a {ESC} keystroke - should get a click event on the AV Cancel Button");
            expVal = "AVCancelButton_Click:";
            _events = String.Empty;
            ctrlAV.SetFocus();
            ctrlAV.SendKeys("{ESC}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "AV Cancel Button not getting the click event", p.log);

            return sr;
        }

        [Scenario("WF control with OK button assigned launched as a modal dialog from a WFH control")]
        public ScenarioResult Scenario7(TParams p) 
        {
            string expVal = String.Empty;
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF2StartButtonName));
            ctrl.Click(PointerButtons.Primary); //click to start the WF Dialog
            Utilities.SleepDoEvents(20);

            // send in the ENTER keystroke to the WF Dialog
            UIObject uiWFDialog = UIObject.Root.Children.Find(UICondition.CreateFromName(WFDialogFormName));
            UIObject ctrlWFDialogTB = uiWFDialog.Descendants.Find(UICondition.CreateFromId(WFDialogTextBoxName));

            p.log.WriteLine("Set focus to the WF Text box and send in a {ENTER} keystroke - should get a click event on the WF Accept Button");
            expVal = "WFDialogAcceptButton_Click:";
            _events = String.Empty;
            ctrlWFDialogTB.SetFocus();
            ctrlWFDialogTB.SendKeys("{ENTER}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "WF Accept Button not getting the click event", p.log);
            
            // make sure we are getting the DialogResult.OK
            WPFMiscUtils.IncCounters(sr, SWF.DialogResult.OK, _dialogResult, "Not getting the correct DialogResult value", p.log);

            return sr;
        }

        [Scenario("WF control with Cancel button assigned launched as a modal dialog from a WFH control")]
        public ScenarioResult Scenario8(TParams p)
        {
            string expVal = String.Empty;
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF2StartButtonName));
            ctrl.Click(PointerButtons.Primary); //click to start the WF Dialog
            Utilities.SleepDoEvents(20);

            // send in the ESC keystroke to the WF Dialog
            UIObject uiWFDialog = UIObject.Root.Children.Find(UICondition.CreateFromName(WFDialogFormName));
            UIObject ctrlWFDialogTB = uiWFDialog.Descendants.Find(UICondition.CreateFromId(WFDialogTextBoxName));

            p.log.WriteLine("Set focus to the WF Text box and send in a {ESC} keystroke - should get a click event on the WF Cancel Button");
            expVal = "WFDialogCancelButton_Click:";
            _events = String.Empty;
            ctrlWFDialogTB.SetFocus();
            ctrlWFDialogTB.SendKeys("{ESC}");
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, expVal, _events, "WF Accept Button not getting the click event", p.log);

            // make sure we are getting the DialogResult.Cancel
            WPFMiscUtils.IncCounters(sr, SWF.DialogResult.Cancel, _dialogResult, "Not getting the correct DialogResult value", p.log);

            return sr;
        }

        #endregion
        


        #region HelperFunction 
    
        private void TestSetup()
        {
            myWriteLine("TestSetup -- Start ");

            #region SetupAVControl
            _AVStackPanel = new SWC.StackPanel();
            _AVTextBox = new SWC.TextBox();
            _AVAcceptButton = new SWC.Button();
            _AVCancelButton = new SWC.Button();

            _AVStackPanel.Name = AVStackPanelName;
            _AVAcceptButton.Content = _AVAcceptButton.Name = AVAcceptButtonName;
            _AVCancelButton.Content = _AVCancelButton.Name = AVCancelButtonName;
            _AVTextBox.Text = _AVTextBox.Name = AVTextBoxName;
            _AVAcceptButton.IsDefault = true;
            _AVCancelButton.IsCancel = true;
            _AVAcceptButton.Click += new System.Windows.RoutedEventHandler(AVControl_Click);
            _AVCancelButton.Click += new System.Windows.RoutedEventHandler(AVControl_Click);
            #endregion

            #region SetupWFControl
            _wfh1 = new WindowsFormsHost();
            _WF1Form = new SWF.Form();
            _WF1FlowLayoutPanel = new SWF.FlowLayoutPanel();
            _WF1TextBox = new SWF.TextBox();
            _WF1AcceptButton = new SWF.Button();
            _WF1CancelButton = new SWF.Button();

            _WF1Form.TopLevel = false;
            _WF1Form.CancelButton = _WF1CancelButton;
            _WF1Form.AcceptButton = _WF1AcceptButton;
            _WF1Form.Controls.Add(_WF1FlowLayoutPanel);
            _WF1FlowLayoutPanel.Controls.Add(_WF1TextBox);
            _WF1FlowLayoutPanel.Controls.Add(_WF1AcceptButton);
            _WF1FlowLayoutPanel.Controls.Add(_WF1CancelButton);
            _WF1FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
            _wfh1.Child = _WF1Form;
            _wfh1.Name = WF1Name;
            _WF1Form.Name = _WF1Form.Text = WF1FormName;
            _WF1FlowLayoutPanel.Name = WF1FlowLayoutPanelName;
            _WF1TextBox.Text = _WF1TextBox.Name = WF1TextBoxName;
            _WF1AcceptButton.Text = _WF1AcceptButton.Name = WF1AcceptButtonName;
            _WF1CancelButton.Text = _WF1CancelButton.Name = WF1CancelButtonName;
            _WF1AcceptButton.Width = _WF1CancelButton.Width = _WF1TextBox.Width = 150;
            _WF1AcceptButton.Click += new EventHandler(WFControl_Click);
            _WF1CancelButton.Click += new EventHandler(WFControl_Click);
            
            _wfh2 = new WindowsFormsHost();
            _WF2FlowLayoutPanel = new SWF.FlowLayoutPanel();
            _WF2FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
            _WF2TextBox = new SWF.TextBox();
            _WF2StartButton = new SWF.Button();
            _wfh2.Child = _WF2FlowLayoutPanel;
            _WF2FlowLayoutPanel.Controls.Add(_WF2TextBox);
            _WF2FlowLayoutPanel.Controls.Add(_WF2StartButton);

            _wfh2.Name = WF2Name;
            _WF2FlowLayoutPanel.Name = WF2FlowLayoutPanelName;
            _WF2TextBox.Text = _WF2TextBox.Name = WF2TextBoxName;
            _WF2StartButton.Text = _WF2StartButton.Name = WF2StartButtonName;
            _WF2StartButton.Width = _WF2TextBox.Width = 150;
            _WF2StartButton.Click += new EventHandler(WF2StartButton_Click);

            _wfh1.Width = _wfh2.Width = 300;
            _wfh1.Height = _wfh2.Height = 150;
            #endregion

            #region LayoutWindow
            this.Title = WindowTitleName;
            _AVStackPanel.Children.Add(_wfh1);
            _AVStackPanel.Children.Add(_wfh2);
            _AVStackPanel.Children.Add(_AVTextBox);
            _AVStackPanel.Children.Add(_AVAcceptButton);
            _AVStackPanel.Children.Add(_AVCancelButton);
            this.Content = _AVStackPanel;
            #endregion

            myWriteLine("TestSetup -- End ");
        }

        void WF2StartButton_Click(object sender, EventArgs e)
        {
            // start the WF Dialog Box on a separate thread
            _dialogResult = SWF.DialogResult.None;
            Thread t = new Thread(new ThreadStart(WFDialogThread));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void WFDialogThread() 
        {
            #region SetupWFDialog
            // WF Dialog
            _WFDialogForm = new SWF.Form();
            _WFDialogFlowLayoutPanel = new SWF.FlowLayoutPanel();
            _WFDialogTextBox = new SWF.TextBox();
            _WFDialogAcceptButton = new SWF.Button();
            _WFDialogCancelButton = new SWF.Button();
            _WFDialogForm.CancelButton = _WFDialogCancelButton;
            _WFDialogForm.AcceptButton = _WFDialogAcceptButton;
            _WFDialogForm.Controls.Add(_WFDialogFlowLayoutPanel);
            _WFDialogFlowLayoutPanel.Controls.Add(_WFDialogTextBox);
            _WFDialogFlowLayoutPanel.Controls.Add(_WFDialogAcceptButton);
            _WFDialogFlowLayoutPanel.Controls.Add(_WFDialogCancelButton);
            _WFDialogFlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
            _WFDialogForm.Name = _WFDialogForm.Text = WFDialogFormName;
            _WFDialogFlowLayoutPanel.Name = WFDialogFlowLayoutPanelName;
            _WFDialogTextBox.Text = _WFDialogTextBox.Name = WFDialogTextBoxName;
            _WFDialogAcceptButton.Text = _WFDialogAcceptButton.Name = WFDialogAcceptButtonName;
            _WFDialogCancelButton.Text = _WFDialogCancelButton.Name = WFDialogCancelButtonName;
            _WFDialogAcceptButton.Width = _WFDialogCancelButton.Width = _WFDialogTextBox.Width = 150;
            _WFDialogAcceptButton.Click += new EventHandler(WFControl_Click);
            _WFDialogCancelButton.Click += new EventHandler(WFControl_Click);
            _WFDialogForm.Width = 300;
            _WFDialogForm.Height = 200;
            _WFDialogAcceptButton.DialogResult = SWF.DialogResult.OK;
            _WFDialogCancelButton.DialogResult = SWF.DialogResult.Cancel;
            #endregion

            _dialogResult = _WFDialogForm.ShowDialog();
        }

        void AVControl_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _events += ((SWC.Control)sender).Name + "_Click:";
        }

        void WFControl_Click(object sender, EventArgs e)
        {
            _events += ((SWF.Control)sender).Name + "_Click:";
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
                case 1: //@ WF control with OK button assigned running in a WFH (ENTER)
                case 2: //@ WF control with Cancel button assigned running in a WFH (ESC)
                    _wfh1.Visibility = SW.Visibility.Visible;
                    _wfh2.Visibility = SW.Visibility.Collapsed;
                    _AVTextBox.Visibility = SW.Visibility.Collapsed;
                    _AVAcceptButton.Visibility = SW.Visibility.Collapsed;
                    _AVCancelButton.Visibility = SW.Visibility.Collapsed;
                    break;

                case 3: //@ AV control with OK button assigned with a WFH on the app with a control (ENTER)
                case 4: //@ AV control with Cancel button assigned with a WFH on the app with a control (CANCEL)
                    _wfh1.Visibility = SW.Visibility.Collapsed;
                    _wfh2.Visibility = SW.Visibility.Visible;
                    _AVTextBox.Visibility = SW.Visibility.Collapsed;
                    _AVAcceptButton.Visibility = SW.Visibility.Visible;
                    _AVCancelButton.Visibility = SW.Visibility.Visible;
                    break;
                case 5: //@ Both WF and AV control with OK button assigned (ENTER)
                case 6: //@ Both WF and AV control with CANCEL button assigned (CANCEL)
                    _wfh1.Visibility = SW.Visibility.Visible;
                    _wfh2.Visibility = SW.Visibility.Collapsed;
                    _AVTextBox.Visibility = SW.Visibility.Visible;
                    _AVAcceptButton.Visibility = SW.Visibility.Visible;
                    _AVCancelButton.Visibility = SW.Visibility.Visible;
                    break;
                case 7: //@ WF control with OK button assigned launched as a modal dialog from a WFH control
                case 8: //@ WF control with Cancel button assigned launched as a modal dialog from a WFH control
                    _wfh1.Visibility = SW.Visibility.Visible;
                    _wfh2.Visibility = SW.Visibility.Visible;
                    _AVTextBox.Visibility = SW.Visibility.Visible;
                    _AVAcceptButton.Visibility = SW.Visibility.Visible;
                    _AVCancelButton.Visibility = SW.Visibility.Visible;
                    break;
                default:  
                    _wfh1.Visibility = SW.Visibility.Visible;
                    _wfh2.Visibility = SW.Visibility.Visible;
                    _AVTextBox.Visibility = SW.Visibility.Visible;
                    _AVAcceptButton.Visibility = SW.Visibility.Visible;
                    _AVCancelButton.Visibility = SW.Visibility.Visible;
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
//@ WF control with OK button assigned running in a WFH (ENTER)

//@ WF control with Cancel button assigned running in a WFH (ESC)

//@ AV control with OK button assigned with a WFH on the app with a control (ENTER)

//@ AV control with Cancel button assigned with a WFH on the app with a control (CANCEL)

//@ Both WF and AV control with OK button assigned (ENTER)

//@ Both WF and AV control with CANCEL button assigned (CANCEL)

//@ WF control with OK button assigned launched as a modal dialog from a WFH control

//@ WF control with Cancel button assigned launched as a modal dialog from a WFH control

