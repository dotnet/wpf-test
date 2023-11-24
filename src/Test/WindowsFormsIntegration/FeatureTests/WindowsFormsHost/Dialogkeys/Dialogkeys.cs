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
// Author:      pachan
//
namespace WindowsFormsHostTests
{

public class Dialogkeys : WPFReflectBase
{
    #region TestVariables

    private delegate void myEventHandler(object sender);
    private int ScenarioIndex = 0;
    private bool debug = false;         // set this true for TC debugging
    private TParams _tp;
    private string Events = String.Empty;
    private UIObject uiApp;
    private SWF.DialogResult dialogResult;

    private SWC.StackPanel AVStackPanel;
    private SWC.Button AVAcceptButton;
    private SWC.Button AVCancelButton;
    private SWC.TextBox AVTextBox;

    private WindowsFormsHost wfh1;
    private WindowsFormsHost wfh2;
    private SWF.Form WF1Form;
    private SWF.FlowLayoutPanel WF1FlowLayoutPanel;
    private SWF.FlowLayoutPanel WF2FlowLayoutPanel;
    private SWF.Button WF1AcceptButton;
    private SWF.Button WF1CancelButton;
    private SWF.TextBox WF1TextBox;
    private SWF.TextBox WF2TextBox;
    private SWF.Button WF2StartButton;

    // Second WF dialog
    private SWF.Form WFDialogForm;
    private SWF.FlowLayoutPanel WFDialogFlowLayoutPanel;
    private SWF.Button WFDialogAcceptButton;
    private SWF.Button WFDialogCancelButton;
    private SWF.TextBox WFDialogTextBox;

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
        ScenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
        Utilities.SleepDoEvents(10);
        ScenarioSetup(ScenarioIndex);
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
        Events = String.Empty;
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBoxName));
        ctrl.SetFocus();
        ctrl.SendKeys("{ENTER}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "Accept Button not getting the click event", p.log);
        return sr;
    }    
    
    [Scenario("WF control with Cancel button assigned running in a WFH (ESC)")]
    public ScenarioResult Scenario2(TParams p) 
    {
        p.log.WriteLine("Set focus to the WF Text box and send in a {ESC} keystroke - should get a click event on the WF Cancel Button");
        ScenarioResult sr = new ScenarioResult();
        string expVal = "WF1CancelButton_Click:";
        Events = String.Empty;
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBoxName));
        ctrl.SetFocus();
        ctrl.SendKeys("{ESC}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "Cancel Button not getting the click event", p.log);
        return sr;
    } 

    [Scenario("AV control with OK button assigned with a WFH on the app with a control (ENTER)")]
    public ScenarioResult Scenario3(TParams p) 
    {
        p.log.WriteLine("Set focus to the WF Text box and send in a {ENTER} keystroke - should get a click event on the AV Accept Button");
        ScenarioResult sr = new ScenarioResult();
        string expVal = "AVAcceptButton_Click:";
        Events = String.Empty;
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF2TextBoxName));
        ctrl.SetFocus();
        ctrl.SendKeys("{ENTER}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "Accept Button not getting the click event", p.log);
        return sr;
    }

    [Scenario("AV control with Cancel button assigned with a WFH on the app with a control (CANCEL)")]
    public ScenarioResult Scenario4(TParams p) 
    {
        p.log.WriteLine("Set focus to the WF Text box and send in a {ESC} keystroke - should get a click event on the AV Cancel Button");
        ScenarioResult sr = new ScenarioResult();
        string expVal = "AVCancelButton_Click:";
        Events = String.Empty;
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF2TextBoxName));
        ctrl.SetFocus();
        ctrl.SendKeys("{ESC}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "Cancel Button not getting the click event", p.log);
        return sr;
    }

    [Scenario("Both WF and AV control with OK button assigned (ENTER)")]
    public ScenarioResult Scenario5(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string expVal = String.Empty;
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrlWF = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBoxName));
        UIObject ctrlAV = uiApp.Descendants.Find(UICondition.CreateFromId(AVTextBoxName));

        p.log.WriteLine("Set focus to the WF Text box and send in a {ENTER} keystroke - should get a click event on the WF Accept Button");
        expVal = "WF1AcceptButton_Click:";
        Events = String.Empty;
        ctrlWF.SetFocus();
        ctrlWF.SendKeys("{ENTER}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "WF Accept Button not getting the click event", p.log);

        p.log.WriteLine("Set focus to the AV TextBox and send in a {ENTER} keystroke - should get a click event on the AV Accept Button");
        expVal = "AVAcceptButton_Click:";
        Events = String.Empty;
        ctrlAV.SetFocus();
        ctrlAV.SendKeys("{ENTER}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "AV Accept Button not getting the click event", p.log);

        return sr;
    }

    [Scenario("Both WF and AV control with CANCEL button assigned (CANCEL)")]
    public ScenarioResult Scenario6(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string expVal = String.Empty;
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrlWF = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBoxName));
        UIObject ctrlAV = uiApp.Descendants.Find(UICondition.CreateFromId(AVTextBoxName));

        p.log.WriteLine("Set focus to the WF Text box and send in a {ESC} keystroke - should get a click event on the WF Cancel Button");
        expVal = "WF1CancelButton_Click:";
        Events = String.Empty;
        ctrlWF.SetFocus();
        ctrlWF.SendKeys("{ESC}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "WF Cancel Button not getting the click event", p.log);

        p.log.WriteLine("Set focus to the AV TextBox and send in a {ESC} keystroke - should get a click event on the AV Cancel Button");
        expVal = "AVCancelButton_Click:";
        Events = String.Empty;
        ctrlAV.SetFocus();
        ctrlAV.SendKeys("{ESC}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "AV Cancel Button not getting the click event", p.log);

        return sr;
    }

    [Scenario("WF control with OK button assigned launched as a modal dialog from a WFH control")]
    public ScenarioResult Scenario7(TParams p) 
    {
        string expVal = String.Empty;
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF2StartButtonName));
        ctrl.Click(PointerButtons.Primary); //click to start the WF Dialog
        Utilities.SleepDoEvents(20);

        // send in the ENTER keystroke to the WF Dialog
        UIObject uiWFDialog = UIObject.Root.Children.Find(UICondition.CreateFromName(WFDialogFormName));
        UIObject ctrlWFDialogTB = uiWFDialog.Descendants.Find(UICondition.CreateFromId(WFDialogTextBoxName));

        p.log.WriteLine("Set focus to the WF Text box and send in a {ENTER} keystroke - should get a click event on the WF Accept Button");
        expVal = "WFDialogAcceptButton_Click:";
        Events = String.Empty;
        ctrlWFDialogTB.SetFocus();
        ctrlWFDialogTB.SendKeys("{ENTER}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "WF Accept Button not getting the click event", p.log);
        
        // make sure we are getting the DialogResult.OK
        WPFMiscUtils.IncCounters(sr, SWF.DialogResult.OK, dialogResult, "Not getting the correct DialogResult value", p.log);

        return sr;
    }

    [Scenario("WF control with Cancel button assigned launched as a modal dialog from a WFH control")]
    public ScenarioResult Scenario8(TParams p)
    {
        string expVal = String.Empty;
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF2StartButtonName));
        ctrl.Click(PointerButtons.Primary); //click to start the WF Dialog
        Utilities.SleepDoEvents(20);

        // send in the ESC keystroke to the WF Dialog
        UIObject uiWFDialog = UIObject.Root.Children.Find(UICondition.CreateFromName(WFDialogFormName));
        UIObject ctrlWFDialogTB = uiWFDialog.Descendants.Find(UICondition.CreateFromId(WFDialogTextBoxName));

        p.log.WriteLine("Set focus to the WF Text box and send in a {ESC} keystroke - should get a click event on the WF Cancel Button");
        expVal = "WFDialogCancelButton_Click:";
        Events = String.Empty;
        ctrlWFDialogTB.SetFocus();
        ctrlWFDialogTB.SendKeys("{ESC}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, Events, "WF Accept Button not getting the click event", p.log);

        // make sure we are getting the DialogResult.Cancel
        WPFMiscUtils.IncCounters(sr, SWF.DialogResult.Cancel, dialogResult, "Not getting the correct DialogResult value", p.log);

        return sr;
    }

    #endregion
    


    #region HelperFunction 
 
    private void TestSetup()
    {
        myWriteLine("TestSetup -- Start ");

        #region SetupAVControl
        AVStackPanel = new SWC.StackPanel();
        AVTextBox = new SWC.TextBox();
        AVAcceptButton = new SWC.Button();
        AVCancelButton = new SWC.Button();

        AVStackPanel.Name = AVStackPanelName;
        AVAcceptButton.Content = AVAcceptButton.Name = AVAcceptButtonName;
        AVCancelButton.Content = AVCancelButton.Name = AVCancelButtonName;
        AVTextBox.Text = AVTextBox.Name = AVTextBoxName;
        AVAcceptButton.IsDefault = true;
        AVCancelButton.IsCancel = true;
        AVAcceptButton.Click += new System.Windows.RoutedEventHandler(AVControl_Click);
        AVCancelButton.Click += new System.Windows.RoutedEventHandler(AVControl_Click);
        #endregion

        #region SetupWFControl
        wfh1 = new WindowsFormsHost();
        WF1Form = new SWF.Form();
        WF1FlowLayoutPanel = new SWF.FlowLayoutPanel();
        WF1TextBox = new SWF.TextBox();
        WF1AcceptButton = new SWF.Button();
        WF1CancelButton = new SWF.Button();

        WF1Form.TopLevel = false;
        WF1Form.CancelButton = WF1CancelButton;
        WF1Form.AcceptButton = WF1AcceptButton;
        WF1Form.Controls.Add(WF1FlowLayoutPanel);
        WF1FlowLayoutPanel.Controls.Add(WF1TextBox);
        WF1FlowLayoutPanel.Controls.Add(WF1AcceptButton);
        WF1FlowLayoutPanel.Controls.Add(WF1CancelButton);
        WF1FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
        wfh1.Child = WF1Form;
        wfh1.Name = WF1Name;
        WF1Form.Name = WF1Form.Text = WF1FormName;
        WF1FlowLayoutPanel.Name = WF1FlowLayoutPanelName;
        WF1TextBox.Text = WF1TextBox.Name = WF1TextBoxName;
        WF1AcceptButton.Text = WF1AcceptButton.Name = WF1AcceptButtonName;
        WF1CancelButton.Text = WF1CancelButton.Name = WF1CancelButtonName;
        WF1AcceptButton.Width = WF1CancelButton.Width = WF1TextBox.Width = 150;
        WF1AcceptButton.Click += new EventHandler(WFControl_Click);
        WF1CancelButton.Click += new EventHandler(WFControl_Click);
        
        wfh2 = new WindowsFormsHost();
        WF2FlowLayoutPanel = new SWF.FlowLayoutPanel();
        WF2FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
        WF2TextBox = new SWF.TextBox();
        WF2StartButton = new SWF.Button();
        wfh2.Child = WF2FlowLayoutPanel;
        WF2FlowLayoutPanel.Controls.Add(WF2TextBox);
        WF2FlowLayoutPanel.Controls.Add(WF2StartButton);

        wfh2.Name = WF2Name;
        WF2FlowLayoutPanel.Name = WF2FlowLayoutPanelName;
        WF2TextBox.Text = WF2TextBox.Name = WF2TextBoxName;
        WF2StartButton.Text = WF2StartButton.Name = WF2StartButtonName;
        WF2StartButton.Width = WF2TextBox.Width = 150;
        WF2StartButton.Click += new EventHandler(WF2StartButton_Click);

        wfh1.Width = wfh2.Width = 300;
        wfh1.Height = wfh2.Height = 150;
        #endregion

        #region LayoutWindow
        this.Title = WindowTitleName;
        AVStackPanel.Children.Add(wfh1);
        AVStackPanel.Children.Add(wfh2);
        AVStackPanel.Children.Add(AVTextBox);
        AVStackPanel.Children.Add(AVAcceptButton);
        AVStackPanel.Children.Add(AVCancelButton);
        this.Content = AVStackPanel;
        #endregion

        myWriteLine("TestSetup -- End ");
    }

    void WF2StartButton_Click(object sender, EventArgs e)
    {
        // start the WF Dialog Box on a separate thread
        dialogResult = SWF.DialogResult.None;
        Thread t = new Thread(new ThreadStart(WFDialogThread));
        t.SetApartmentState(ApartmentState.STA);
        t.Start();
    }

    private void WFDialogThread() 
    {
        #region SetupWFDialog
        // WF Dialog
        WFDialogForm = new SWF.Form();
        WFDialogFlowLayoutPanel = new SWF.FlowLayoutPanel();
        WFDialogTextBox = new SWF.TextBox();
        WFDialogAcceptButton = new SWF.Button();
        WFDialogCancelButton = new SWF.Button();
        WFDialogForm.CancelButton = WFDialogCancelButton;
        WFDialogForm.AcceptButton = WFDialogAcceptButton;
        WFDialogForm.Controls.Add(WFDialogFlowLayoutPanel);
        WFDialogFlowLayoutPanel.Controls.Add(WFDialogTextBox);
        WFDialogFlowLayoutPanel.Controls.Add(WFDialogAcceptButton);
        WFDialogFlowLayoutPanel.Controls.Add(WFDialogCancelButton);
        WFDialogFlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
        WFDialogForm.Name = WFDialogForm.Text = WFDialogFormName;
        WFDialogFlowLayoutPanel.Name = WFDialogFlowLayoutPanelName;
        WFDialogTextBox.Text = WFDialogTextBox.Name = WFDialogTextBoxName;
        WFDialogAcceptButton.Text = WFDialogAcceptButton.Name = WFDialogAcceptButtonName;
        WFDialogCancelButton.Text = WFDialogCancelButton.Name = WFDialogCancelButtonName;
        WFDialogAcceptButton.Width = WFDialogCancelButton.Width = WFDialogTextBox.Width = 150;
        WFDialogAcceptButton.Click += new EventHandler(WFControl_Click);
        WFDialogCancelButton.Click += new EventHandler(WFControl_Click);
        WFDialogForm.Width = 300;
        WFDialogForm.Height = 200;
        WFDialogAcceptButton.DialogResult = SWF.DialogResult.OK;
        WFDialogCancelButton.DialogResult = SWF.DialogResult.Cancel;
        #endregion

        dialogResult = WFDialogForm.ShowDialog();
    }

    void AVControl_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Events += ((SWC.Control)sender).Name + "_Click:";
    }

    void WFControl_Click(object sender, EventArgs e)
    {
        Events += ((SWF.Control)sender).Name + "_Click:";
    }

    void myWriteLine(string s)
    {
        if (debug)
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
                wfh1.Visibility = SW.Visibility.Visible;
                wfh2.Visibility = SW.Visibility.Collapsed;
                AVTextBox.Visibility = SW.Visibility.Collapsed;
                AVAcceptButton.Visibility = SW.Visibility.Collapsed;
                AVCancelButton.Visibility = SW.Visibility.Collapsed;
                break;

            case 3: //@ AV control with OK button assigned with a WFH on the app with a control (ENTER)
            case 4: //@ AV control with Cancel button assigned with a WFH on the app with a control (CANCEL)
                wfh1.Visibility = SW.Visibility.Collapsed;
                wfh2.Visibility = SW.Visibility.Visible;
                AVTextBox.Visibility = SW.Visibility.Collapsed;
                AVAcceptButton.Visibility = SW.Visibility.Visible;
                AVCancelButton.Visibility = SW.Visibility.Visible;
                break;
            case 5: //@ Both WF and AV control with OK button assigned (ENTER)
            case 6: //@ Both WF and AV control with CANCEL button assigned (CANCEL)
                wfh1.Visibility = SW.Visibility.Visible;
                wfh2.Visibility = SW.Visibility.Collapsed;
                AVTextBox.Visibility = SW.Visibility.Visible;
                AVAcceptButton.Visibility = SW.Visibility.Visible;
                AVCancelButton.Visibility = SW.Visibility.Visible;
                break;
            case 7: //@ WF control with OK button assigned launched as a modal dialog from a WFH control
            case 8: //@ WF control with Cancel button assigned launched as a modal dialog from a WFH control
                wfh1.Visibility = SW.Visibility.Visible;
                wfh2.Visibility = SW.Visibility.Visible;
                AVTextBox.Visibility = SW.Visibility.Visible;
                AVAcceptButton.Visibility = SW.Visibility.Visible;
                AVCancelButton.Visibility = SW.Visibility.Visible;
                break;
            default:  
                wfh1.Visibility = SW.Visibility.Visible;
                wfh2.Visibility = SW.Visibility.Visible;
                AVTextBox.Visibility = SW.Visibility.Visible;
                AVAcceptButton.Visibility = SW.Visibility.Visible;
                AVCancelButton.Visibility = SW.Visibility.Visible;
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

