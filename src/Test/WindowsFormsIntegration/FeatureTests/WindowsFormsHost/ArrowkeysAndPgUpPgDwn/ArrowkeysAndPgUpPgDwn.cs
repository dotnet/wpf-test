// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using SW = System.Windows;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;


// Testcase:    ArrowkeysAndPgUpPgDwn
// Description: Verify that WF controls expecting Arrow Key and Page key input work as expected
namespace WindowsFormsHostTests
{

public class ArrowkeysAndPgUpPgDwn : WPFReflectBase
{
    #region TestVariables

    private delegate void myEventHandler(object sender);
    private static MethodInfo[] s_mi;
    private UIObject _uiApp;
    private int _scenarioIndex = 0;
    private string _WFEvents;                        // event sequence string

    #region AVControls
    private SWC.ScrollViewer _AVScrollViewer;
    private SWC.DockPanel _AVDockPanel;
    private SWC.DockPanel _AVDockPanel2;
    private SWC.Button _AVButton1;
    private SWC.Button _AVButton2;
    private SWC.Button _AVButton3;
    private SWC.Button _AVButton4;
    private SWC.Button _AVButton5;
    private SWC.CheckBox _AVCheckBox1;
    private SWC.CheckBox _AVCheckBox2;
    private SWC.CheckBox _AVCheckBox3;
    private SWC.CheckBox _AVCheckBox4;
    private SWC.RadioButton _AVRadioButton1;
    private SWC.RadioButton _AVRadioButton2;
    private SWC.RadioButton _AVRadioButton3;
    private SWC.RadioButton _AVRadioButton4;
    #endregion

    #region WFControls
    private WindowsFormsHost _wfh1;
    private WindowsFormsHost _wfh2;
    private WindowsFormsHost _wfh3;
    private WindowsFormsHost _wfh4;
    private WindowsFormsHost _wfh5;
    private WindowsFormsHost _wfh6;
    private WindowsFormsHost _wfh7;
    private SWF.FlowLayoutPanel _WF1FlowLayoutPanel;
    private SWF.FlowLayoutPanel _WF2FlowLayoutPanel;
    private SWF.Button _WF1Button1;
    private SWF.Button _WF1Button2;
    private SWF.Button _WF1Button3;
    private SWF.Button _WF1Button4;
    private SWF.CheckBox _WF1CheckBox1;
    private SWF.CheckBox _WF1CheckBox2;
    private SWF.CheckBox _WF1CheckBox3;
    private SWF.CheckBox _WF1CheckBox4;
    private SWF.GroupBox _WF2GroupBox;
    private SWF.RadioButton _WF2RadioButton1;
    private SWF.RadioButton _WF2RadioButton2;
    private SWF.RadioButton _WF2RadioButton3;
    private SWF.RadioButton _WF2RadioButton4;
    private SWF.NumericUpDown _WF3NumericUpDown;
    private SWF.VScrollBar _WF4VScrollBar;
    private SWF.HScrollBar _WF5HScrollBar;
    private SWF.TextBox _WF6TextBox;
    private SWF.DataGridView _WF7DataGridView;
    #endregion

    #region ControlNames
    private const string WindowTitleName = "ArrowkeysAndPgUpPgDwnTest";

    private const string AVScrollViewerName = "AVScrollViewer";
    private const string AVDockPanelName = "AVDockPanel";
    private const string AVDockPanel2Name = "AVDockPanel2";
    private const string AVButton1Name = "AVButton1";
    private const string AVButton2Name = "AVButton2";
    private const string AVButton3Name = "AVButton3";
    private const string AVButton4Name = "AVButton4";
    private const string AVButton5Name = "AVButton5";
    private const string AVCheckBox1Name = "AVCheckBox1";
    private const string AVCheckBox2Name = "AVCheckBox2";
    private const string AVCheckBox3Name = "AVCheckBox3";
    private const string AVCheckBox4Name = "AVCheckBox4";
    private const string AVRadioButton1Name = "AVRadioButton1";
    private const string AVRadioButton2Name = "AVRadioButton2";
    private const string AVRadioButton3Name = "AVRadioButton3";
    private const string AVRadioButton4Name = "AVRadioButton4";

    private const string WF1Name = "WF1";
    private const string WF1FlowLayoutPanelName = "WF1FlowLayoutPanel";
    private const string WF1Button1Name = "WF1Button1";
    private const string WF1Button2Name = "WF1Button2";
    private const string WF1Button3Name = "WF1Button3";
    private const string WF1Button4Name = "WF1Button4";
    private const string WF1CheckBox1Name = "WF1CheckBox1";
    private const string WF1CheckBox2Name = "WF1CheckBox2";
    private const string WF1CheckBox3Name = "WF1CheckBox3";
    private const string WF1CheckBox4Name = "WF1CheckBox4";

    private const string WF2Name = "WF2";
    private const string WF2FlowLayoutPanelName = "WF2FlowLayoutPanel";
    private const string WF2GroupBoxName = "WF2GroupBox";
    private const string WF2RadioButton1Name = "WF2RadioButton1";
    private const string WF2RadioButton2Name = "WF2RadioButton2";
    private const string WF2RadioButton3Name = "WF2RadioButton3";
    private const string WF2RadioButton4Name = "WF2RadioButton4";

    private const string WF3Name = "WF3";
    private const string WF3NumericUpDownName = "WF3NumericUpDown";

    private const string WF4Name = "WF4";
    private const string WF4VScrollBarName = "WF4VScrollBar";

    private const string WF5Name = "WF5";
    private const string WF5HScrollBarName = "WF5HScrollBar";

    private const string WF6Name = "WF6";
    private const string WF6TextBoxName = "WF6TextBox";

    private const string WF7Name = "WF7";
    private const string WF7DataGridViewName = "WF7DataGridView";
    #endregion

    #endregion

    #region Testcase setup
    public ArrowkeysAndPgUpPgDwn(string[] args) : base(args) { }


    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        _scenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
        HideShowGroup(_scenarioIndex);
        // move the mouse point to the corner
        Mouse.Instance.Move(new System.Drawing.Point(0, 0));
        Utilities.SleepDoEvents(10);
        return base.BeforeScenario(p, scenario);
    }

    protected override void InitTest(TParams p) 
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        this.SizeToContent = System.Windows.SizeToContent.Height;
        this.Width = 400;
        s_mi = GetAllScenarios(this);
        this.UseMITA = true;
        TestSetup(p);
        base.InitTest(p);
        this.Top = 0;
        this.Left = 0;
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================

    #region Scenarios
    [Scenario("Arrow Keys work for NumericUpDown control in a WFH")]
    public ScenarioResult Scenario3(TParams p) 
    {
        int i = 0;
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF NumericUpDown control");
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF3NumericUpDownName));

        _WFEvents = String.Empty;;
        ctrl.SetFocus();

        // Key up for 15 times
        expVal = String.Empty; 
        for (i = 0; i < 15; i++)
        {
            expVal += "WF3NumericUpDown-" + (i+1).ToString() + ":";
            _uiApp.SendKeys("{UP}");
        }
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {UP} Value not as expected", p.log);

        // Key down for 15 times
        expVal = String.Empty; 
        _WFEvents = String.Empty;
        for (i = 15; i > 0; i--)
        {
            expVal += "WF3NumericUpDown-" + (i-1).ToString() + ":";
            _uiApp.SendKeys("{DOWN}");
        }
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {DOWN} Value not as expected", p.log);
        return sr;

    }
    [Scenario("Move between WF Control's in the same WFH via Arrow Keys.")]
    public ScenarioResult Scenario1(TParams p) 
    {
        //p.log.WriteLine("{0} - Test Run Start", GetScenarioAttribute(mi[ScenarioIndex]).Description);
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1Button1Name));

        p.log.WriteLine("Testing Arrow Keys on WF Button/CheckBox controls");

        // Press a mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT}
        expVal += String.Empty;
        _WFEvents = String.Empty;

        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        //expVal += "WF1CheckBox2_GotFocus:";
        //expVal += "WF1CheckBox3_GotFocus:";
        //expVal += "WF1CheckBox4_GotFocus:";
        //expVal += "WF1CheckBox3_GotFocus:";
        //expVal += "WF1CheckBox2_GotFocus:";
        //expVal += "WF1CheckBox1_GotFocus:";
        //expVal += "WF1CheckBox2_GotFocus:";
        //expVal += "WF1CheckBox1_GotFocus:";
        //expVal += "WF1CheckBox2_GotFocus:";
        //expVal += "WF1CheckBox1_GotFocus:";
        //expVal += "WF1Button4_GotFocus:";
        //expVal += "WF1CheckBox1_GotFocus:";

        // focus on the first WF button
        ctrl.SetFocus();

        _uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(2);
        _uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(2);
        _uiApp.SendKeys("{LEFT}");
        Utilities.SleepDoEvents(2);
        _uiApp.SendKeys("{LEFT}");
        Utilities.SleepDoEvents(2);
        _uiApp.SendKeys("{DOWN}");
        Utilities.SleepDoEvents(2);
        _uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(2);
        _uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(2);
        _uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(2);
        //        uiApp.SendKeys("{RIGHT}{RIGHT}{DOWN}{UP}{UP}");
//        uiApp.SendKeys("{LEFT}{DOWN}{UP}{DOWN}{UP}{UP}{RIGHT}");
//        Utilities.SleepDoEvents(20);
        
        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        //p.log.WriteLine("{0} - Test Run End", GetScenarioAttribute(mi[ScenarioIndex]).Description);
        return sr;
    }

    [Scenario("Verify that Arrow Keys wrap focus between controls in the same WFH")]
    public ScenarioResult Scenario2(TParams p) 
    {
         int i = 0;
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF Button/CheckBox controls");
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1Button1Name));

        // Press Down 15 times.. should wrap around within WFH1 
        _WFEvents = String.Empty;
        expVal = String.Empty; 
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1Button2_GotFocus:";
        expVal += "WF1Button3_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1Button2_GotFocus:";
        expVal += "WF1Button3_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";

        // focus on the first WF button
        ctrl.SetFocus();

        for (i = 0; i < 15; i++)
            _uiApp.SendKeys("{DOWN}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {DOWN} Value not as expected", p.log);

        // Press Up 15 times.. should wrap around within WFH1 
        _WFEvents = String.Empty;
        expVal = String.Empty; 
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1Button3_GotFocus:";
        expVal += "WF1Button2_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1Button3_GotFocus:";
        expVal += "WF1Button2_GotFocus:";

        // focus on the first WF button
        ctrl.SetFocus();

        for (i = 0; i < 15; i++)
            _uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {UP} Value not as expected", p.log);

        // Press Left 15 times.. should wrap around within WFH1 
        _WFEvents = String.Empty;
        expVal = String.Empty; 
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1Button3_GotFocus:";
        expVal += "WF1Button2_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1Button3_GotFocus:";
        expVal += "WF1Button2_GotFocus:";

        // focus on the first WF button
        ctrl.SetFocus();

        for (i = 0; i < 15; i++)
            _uiApp.SendKeys("{LEFT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {LEFT} Value not as expected", p.log);

        // Press Right 15 times.. should wrap around within WFH1 
        _WFEvents = String.Empty;
        expVal = String.Empty; 
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1Button2_GotFocus:";
        expVal += "WF1Button3_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1Button2_GotFocus:";
        expVal += "WF1Button3_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";

        // focus on the first WF button
        ctrl.SetFocus();
        
        for (i = 0; i < 15; i++)
            _uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {RIGHT} Value not as expected", p.log);
        return sr;
    }

    /*
    [Scenario("Arrow keys work for a horizontal scroll bar in a WFH")]
    public ScenarioResult Scenario4(TParams p)
    {
        int expVal;
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        // focus on Vertical scrollbar
        p.log.WriteLine("Testing Vertical Scroll bar");
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF4VScrollBarName));
        ctrl.SetFocus();

        // End
        ctrl.SendKeys("{END}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, 91, WF4VScrollBar.Value, "After {END} Value not as expected", p.log);

        // Home
        ctrl.SendKeys("{HOME}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, 0, WF4VScrollBar.Value, "After {HOME} Value not as expected", p.log);

        // scroll car is now at top
        expVal = 0;

        // PG Down
        ctrl.SendKeys("{PGDN}");
        ctrl.SendKeys("{PGDN}");
        Utilities.SleepDoEvents(10);

        expVal += 2 * WF4VScrollBar.LargeChange;
        WPFMiscUtils.IncCounters(sr, expVal, WF4VScrollBar.Value, "After {PGDN} Value not as expected", p.log);

        // PG Up
        ctrl.SendKeys("{PGUP}");
        Utilities.SleepDoEvents(10);

        expVal -= WF4VScrollBar.LargeChange;
        WPFMiscUtils.IncCounters(sr, expVal, WF4VScrollBar.Value, "After {PGUP} Value not as expected", p.log);

        // Arrow Down
        ctrl.SendKeys("{DOWN}");
        ctrl.SendKeys("{DOWN}");
        Utilities.SleepDoEvents(10);

        expVal += 2 * WF4VScrollBar.SmallChange;
        WPFMiscUtils.IncCounters(sr, expVal, WF4VScrollBar.Value, "After {DOWN} Value not as expected", p.log);

        // Arrow Up
        ctrl.SendKeys("{UP}");
        Utilities.SleepDoEvents(10);

        expVal -= 1;
        WPFMiscUtils.IncCounters(sr, expVal, WF4VScrollBar.Value, "After {END} Value not as expected", p.log);

        // --- Horizontal Scroll --- //

        // focus on Horizontal scrollbar
        p.log.WriteLine("Testing Horizontal Scroll bar");
        ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF5HScrollBarName));
        ctrl.SetFocus();


        // End
        ctrl.SendKeys("{END}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, 91, WF5HScrollBar.Value, "After {END} Value not as expected", p.log);

        // Home
        ctrl.SendKeys("{HOME}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, 0, WF5HScrollBar.Value, "After {HOME} Value not as expected", p.log);

        // scroll car is now at left
        expVal = 0;

        // PG Down
        ctrl.SendKeys("{PGDN}");
        ctrl.SendKeys("{PGDN}");
        Utilities.SleepDoEvents(10);
        expVal += 2 * WF5HScrollBar.LargeChange;
        WPFMiscUtils.IncCounters(sr, expVal, WF5HScrollBar.Value, "After {PGDN} Value not as expected", p.log);

        // PG Up
        ctrl.SendKeys("{PGUP}");
        Utilities.SleepDoEvents(10);
        expVal -= WF5HScrollBar.LargeChange;
        WPFMiscUtils.IncCounters(sr, expVal, WF5HScrollBar.Value, "After {PGUP} Value not as expected", p.log);

        // Arrow Down
        ctrl.SendKeys("{DOWN}");
        ctrl.SendKeys("{DOWN}");
        Utilities.SleepDoEvents(10);
        expVal += 2 * WF5HScrollBar.SmallChange;
        WPFMiscUtils.IncCounters(sr, expVal, WF5HScrollBar.Value, "After {DOWN} Value not as expected", p.log);

        // Arrow Up
        ctrl.SendKeys("{UP}");
        Utilities.SleepDoEvents(10);
        expVal -= 1;
        WPFMiscUtils.IncCounters(sr, expVal, WF5HScrollBar.Value, "After {END} Value not as expected", p.log);
        return sr;
    }
    */

    [Scenario("Arrow Keys work with DGV (Up, Down, Left and Right)")]
    public ScenarioResult Scenario5(TParams p) 
    {
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF DGV controls");
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF7DataGridViewName));

        expVal = String.Empty; 
        _WFEvents = String.Empty;
        expVal += "Row-0 Column-0:";
        expVal += "Row-1 Column-0:";
        expVal += "Row-2 Column-0:";
        expVal += "Row-3 Column-0:";
        expVal += "Row-2 Column-0:";
        expVal += "Row-2 Column-1:";
        expVal += "Row-2 Column-2:";
        expVal += "Row-2 Column-3:";
        expVal += "Row-3 Column-3:";
        expVal += "Row-4 Column-3:";
        expVal += "Row-3 Column-3:";
        expVal += "Row-3 Column-2:";
        expVal += "Row-3 Column-3:";
        expVal += "Row-3 Column-2:";
        expVal += "Row-3 Column-1:";
        expVal += "Row-3 Column-0:";
        expVal += "Row-4 Column-0:";
        expVal += "Row-5 Column-0:";
        expVal += "Row-4 Column-0:";
        expVal += "Row-3 Column-0:";
        expVal += "Row-2 Column-0:";
        expVal += "Row-1 Column-0:";

        ctrl.SetFocus();
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("Verify that Arrow keys cycle through a set of Radio buttons in a group box")]
    public ScenarioResult Scenario6(TParams p) 
    {
        int i = 0;
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF Radio Button controls");
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF2RadioButton1Name));

        // Press Down 15 times.. should wrap around within WFH1 
        _WFEvents = String.Empty;
        expVal = String.Empty; 
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";

        // focus on the first WF button
        ctrl.SetFocus();

        for (i = 0; i < 15; i++)
            _uiApp.SendKeys("{DOWN}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {DOWN} Value not as expected", p.log);

        // Press Up 15 times.. should wrap around within WFH1 
        expVal = String.Empty; 
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        _WFEvents = String.Empty;

        // focus on the first WF button
        ctrl.SetFocus();

        for (i = 0; i < 15; i++)
            _uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {UP} Value not as expected", p.log);

        // Press Left 15 times.. should wrap around within WFH1 
        _WFEvents = String.Empty;
        expVal = String.Empty; 
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";

        // focus on the first WF button
        ctrl.SetFocus();

        for (i = 0; i < 15; i++)
            _uiApp.SendKeys("{LEFT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {LEFT} Value not as expected", p.log);

        // Press Right 15 times.. should wrap around within WFH1 
        _WFEvents = String.Empty;
        expVal = String.Empty; 
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";

        // focus on the first WF button
        ctrl.SetFocus();

        for (i = 0; i < 15; i++)
            _uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After repeated {RIGHT} Value not as expected", p.log);

        // Press a mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT}
        _WFEvents = String.Empty;
        expVal = String.Empty; 
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton3_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton2_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";

        // focus on the first WF Radio button
        ctrl.SetFocus();

        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("When a WFH's parent is vertically scrolled, verify that a vertical scroll bar on a WF textbox works as expected.")]
    public ScenarioResult Scenario7(TParams p)
    {
        string expVal = String.Empty;
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF6TextBoxName));

        p.log.WriteLine("Testing on AV Vertical Scroll Bar");

        _WFEvents = String.Empty;
        expVal = String.Empty;
        expVal += "VerticalOffset=200:";   // ExtentHeight (200 - wfh + 300 - button) - ViewportHeight (200 - scrollviewer)
        expVal += "VerticalOffset=0:";
        expVal += "VerticalOffset=200:";   // ExtentHeight (200 - wfh + 300 - button) - ViewportHeight (200 - scrollviewer)
        expVal += "VerticalOffset=0:";
        expVal += "VerticalOffset=20:";
        expVal += "VerticalOffset=40:";
        expVal += "VerticalOffset=60:";
        expVal += "VerticalOffset=0:";
	
	/* We cannot call AVScrollViewer from the test harness thread
        p.log.WriteLine("Scroll the AV Vertical Scroll Bar up and down and make sure the Text Box control does not get any message");
        AVScrollViewer.ScrollToBottom();
        Utilities.SleepDoEvents(10);
        AVScrollViewer.ScrollToTop();
        Utilities.SleepDoEvents(10);
        AVScrollViewer.ScrollToEnd();
        Utilities.SleepDoEvents(10);
        AVScrollViewer.ScrollToHome();
        Utilities.SleepDoEvents(10);
        AVScrollViewer.ScrollToVerticalOffset(20);
        Utilities.SleepDoEvents(10);
        AVScrollViewer.ScrollToVerticalOffset(40);
        Utilities.SleepDoEvents(10);
        AVScrollViewer.ScrollToVerticalOffset(60);
        Utilities.SleepDoEvents(10);
        AVScrollViewer.ScrollToTop();
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "Not getting the correct Event", p.log);
	*/

        p.log.WriteLine("Send page down and see if we are on line 8");
        int lineNo = ((int)(_WF6TextBox.Height / _WF6TextBox.Font.Height)) - 1;
        expVal = lineNo.ToString() + ": of WinForm Text Box Control";
        ctrl.SendKeys("^{HOME}{PGDN}{HOME}+{END}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, _WF6TextBox.SelectedText, "Text did not scroll", p.log);

        return sr;
    }

    [Scenario("Verify that Shift+Arrow's work")]
    public ScenarioResult Scenario8(TParams p) 
    {
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Shift+Arrow Keys on WF Button / CheckBox controls");
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1Button1Name));

        // Press a mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT}
        expVal = String.Empty; 
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        _WFEvents = String.Empty;

        // focus on the first WF button
        ctrl.SetFocus();

        _uiApp.SendKeys("+{UP}");
        _uiApp.SendKeys("+{RIGHT}");
        _uiApp.SendKeys("+{LEFT}");
        _uiApp.SendKeys("+{LEFT}");
        _uiApp.SendKeys("+{DOWN}");
        _uiApp.SendKeys("+{UP}");
        _uiApp.SendKeys("+{UP}");
        _uiApp.SendKeys("+{UP}");
        _uiApp.SendKeys("+{RIGHT}");
        _uiApp.SendKeys("+{RIGHT}");
        _uiApp.SendKeys("+{DOWN}");
        _uiApp.SendKeys("+{UP}");
        _uiApp.SendKeys("+{UP}");
        _uiApp.SendKeys("+{LEFT}");
        _uiApp.SendKeys("+{DOWN}");
        _uiApp.SendKeys("+{UP}");
        _uiApp.SendKeys("+{DOWN}");
        _uiApp.SendKeys("+{UP}");
        _uiApp.SendKeys("+{UP}");
        _uiApp.SendKeys("+{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After mixture of SHIFT + {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("Verify that CTRL+Arrow's work")]
    public ScenarioResult Scenario9(TParams p) 
    {
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Shift+Arrow Keys on both AV and WF controls");
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(AVButton1Name));

        // Press a mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT}

        expVal = String.Empty; 
        expVal += "AVButton1_GotFocus:";
        expVal += "AVRadioButton4_GotFocus:";
        expVal += "AVRadioButton3_GotFocus:";
        expVal += "AVCheckBox4_GotFocus:";
        expVal += "AVCheckBox3_GotFocus:";
        expVal += "AVButton4_GotFocus:";
        expVal += "AVButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "AVRadioButton2_GotFocus:";
        expVal += "AVRadioButton1_GotFocus:";
        expVal += "AVCheckBox2_GotFocus:";
        expVal += "AVCheckBox1_GotFocus:";
        expVal += "AVCheckBox2_GotFocus:";
        expVal += "AVRadioButton1_GotFocus:";
        expVal += "AVRadioButton2_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "AVButton3_GotFocus:";
        expVal += "AVButton4_GotFocus:";
        expVal += "AVCheckBox3_GotFocus:";
        expVal += "AVCheckBox4_GotFocus:";
        expVal += "AVRadioButton3_GotFocus:";
        expVal += "AVRadioButton4_GotFocus:";
        expVal += "AVButton1_GotFocus:";
        expVal += "AVButton2_GotFocus:";
        expVal += "AVCheckBox1_GotFocus:";
        expVal += "AVCheckBox2_GotFocus:";
        expVal += "AVRadioButton1_GotFocus:";
        expVal += "AVRadioButton2_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "AVButton3_GotFocus:";
        expVal += "AVButton4_GotFocus:";
        expVal += "AVCheckBox3_GotFocus:";
        expVal += "AVCheckBox4_GotFocus:";
        expVal += "AVRadioButton3_GotFocus:";
        expVal += "AVRadioButton4_GotFocus:";
        expVal += "AVButton1_GotFocus:";
        expVal += "AVButton2_GotFocus:";
        _WFEvents = String.Empty;

        // focus on the first WF button
        ctrl.SetFocus();

        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{UP}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{RIGHT}");
        _uiApp.SendKeys("^{RIGHT}");
        _uiApp.SendKeys("^{RIGHT}");
        _uiApp.SendKeys("^{RIGHT}");
        _uiApp.SendKeys("^{LEFT}");
        _uiApp.SendKeys("^{LEFT}");
        _uiApp.SendKeys("^{LEFT}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        _uiApp.SendKeys("^{DOWN}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After mixture of SHIFT + {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("Verify that Shift+CTRL+Arrows's work")]
    public ScenarioResult Scenario10(TParams p) 
    {
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Shift+Arrow Keys on both AV and WF controls");
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(AVButton1Name));

        // Press a mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT}

        expVal = String.Empty; 
        expVal += "AVButton1_GotFocus:";
        expVal += "AVRadioButton4_GotFocus:";
        expVal += "AVRadioButton3_GotFocus:";
        expVal += "AVCheckBox4_GotFocus:";
        expVal += "AVCheckBox3_GotFocus:";
        expVal += "AVButton4_GotFocus:";
        expVal += "AVButton3_GotFocus:";
        expVal += "WF2RadioButton4_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "AVRadioButton2_GotFocus:";
        expVal += "AVRadioButton1_GotFocus:";
        expVal += "AVCheckBox2_GotFocus:";
        expVal += "AVCheckBox1_GotFocus:";
        expVal += "AVCheckBox2_GotFocus:";
        expVal += "AVRadioButton1_GotFocus:";
        expVal += "AVRadioButton2_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "AVButton3_GotFocus:";
        expVal += "AVButton4_GotFocus:";
        expVal += "AVCheckBox3_GotFocus:";
        expVal += "AVCheckBox4_GotFocus:";
        expVal += "AVRadioButton3_GotFocus:";
        expVal += "AVRadioButton4_GotFocus:";
        expVal += "AVButton1_GotFocus:";
        expVal += "AVButton2_GotFocus:";
        expVal += "AVCheckBox1_GotFocus:";
        expVal += "AVCheckBox2_GotFocus:";
        expVal += "AVRadioButton1_GotFocus:";
        expVal += "AVRadioButton2_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF2RadioButton1_GotFocus:";
        expVal += "AVButton3_GotFocus:";
        expVal += "AVButton4_GotFocus:";
        expVal += "AVCheckBox3_GotFocus:";
        expVal += "AVCheckBox4_GotFocus:";
        expVal += "AVRadioButton3_GotFocus:";
        expVal += "AVRadioButton4_GotFocus:";
        expVal += "AVButton1_GotFocus:";
        expVal += "AVButton2_GotFocus:";
        _WFEvents = String.Empty;

        // focus on the first WF button
        ctrl.SetFocus();

        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{UP}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{RIGHT}");
        _uiApp.SendKeys("+^{RIGHT}");
        _uiApp.SendKeys("+^{RIGHT}");
        _uiApp.SendKeys("+^{RIGHT}");
        _uiApp.SendKeys("+^{LEFT}");
        _uiApp.SendKeys("+^{LEFT}");
        _uiApp.SendKeys("+^{LEFT}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        _uiApp.SendKeys("+^{DOWN}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After mixture of SHIFT + {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("Verifty that Arrow keys work with a scrollable control (Form or Panel) - AutoScrole=true with children outside of control bounds.")]
    public ScenarioResult Scenario11(TParams p) 
    {
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF Button/CheckBox controls");
        UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1Button1Name));

        // Press a mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT}
        expVal = String.Empty; 
        _WFEvents = String.Empty;

        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1Button1_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox4_GotFocus:";
        expVal += "WF1CheckBox3_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1CheckBox2_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";
        expVal += "WF1Button4_GotFocus:";
        expVal += "WF1CheckBox1_GotFocus:";

        // focus on the first WF button
        ctrl.SetFocus();

        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{RIGHT}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{LEFT}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{DOWN}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{UP}");
        _uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, _WFEvents, "After mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    #endregion
    
    #region HelperFunction

    private void TestSetup(TParams p)
    {
        p.log.WriteLine("TestSetup -- Start ");

        #region SetupAVControl

        _AVScrollViewer = new SWC.ScrollViewer();
        _AVDockPanel = new SWC.DockPanel();
        _AVDockPanel2 = new SWC.DockPanel();
        _AVButton1 = new SWC.Button();
        _AVButton2 = new SWC.Button();
        _AVButton3 = new SWC.Button();
        _AVButton4 = new SWC.Button();
        _AVButton5 = new SWC.Button();
        _AVCheckBox1 = new SWC.CheckBox();
        _AVCheckBox2 = new SWC.CheckBox();
        _AVCheckBox3 = new SWC.CheckBox();
        _AVCheckBox4 = new SWC.CheckBox();
        _AVRadioButton1 = new SWC.RadioButton();
        _AVRadioButton2 = new SWC.RadioButton();
        _AVRadioButton3 = new SWC.RadioButton();
        _AVRadioButton4 = new SWC.RadioButton();

        _AVButton1.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVButton2.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVButton3.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVButton4.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVCheckBox1.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVCheckBox2.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVCheckBox3.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVCheckBox4.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVRadioButton1.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVRadioButton2.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVRadioButton3.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVRadioButton4.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        _AVScrollViewer.ScrollChanged += new System.Windows.Controls.ScrollChangedEventHandler(AVScrollViewer_ScrollChanged);

        _AVScrollViewer.Name = AVScrollViewerName;
        _AVDockPanel.Name = AVDockPanelName;
        _AVDockPanel2.Name = AVDockPanel2Name;
        _AVButton1.Content = _AVButton1.Name = AVButton1Name;
        _AVButton2.Content = _AVButton2.Name = AVButton2Name;
        _AVButton3.Content = _AVButton3.Name = AVButton3Name;
        _AVButton4.Content = _AVButton4.Name = AVButton4Name;
        _AVButton5.Content = _AVButton5.Name = AVButton5Name;
        _AVCheckBox1.Content = _AVCheckBox1.Name = AVCheckBox1Name;
        _AVCheckBox2.Content = _AVCheckBox2.Name = AVCheckBox2Name;
        _AVCheckBox3.Content = _AVCheckBox3.Name = AVCheckBox3Name;
        _AVCheckBox4.Content = _AVCheckBox4.Name = AVCheckBox4Name;
        _AVRadioButton1.Content = _AVRadioButton1.Name = AVRadioButton1Name;
        _AVRadioButton2.Content = _AVRadioButton2.Name = AVRadioButton2Name;
        _AVRadioButton3.Content = _AVRadioButton3.Name = AVRadioButton3Name;
        _AVRadioButton4.Content = _AVRadioButton4.Name = AVRadioButton4Name;

#endregion

        #region SetupWFControl
        _wfh1 = new WindowsFormsHost();
        _wfh2 = new WindowsFormsHost();
        _wfh3 = new WindowsFormsHost();
        _wfh4 = new WindowsFormsHost();
        _wfh5 = new WindowsFormsHost();
        _wfh6 = new WindowsFormsHost();
        _wfh7 = new WindowsFormsHost();
        _WF1FlowLayoutPanel = new SWF.FlowLayoutPanel();
        _WF2FlowLayoutPanel = new SWF.FlowLayoutPanel();
        _WF1Button1 = new SWF.Button();
        _WF1Button2 = new SWF.Button();
        _WF1Button3 = new SWF.Button();
        _WF1Button4 = new SWF.Button();
        _WF1CheckBox1 = new SWF.CheckBox();
        _WF1CheckBox2 = new SWF.CheckBox();
        _WF1CheckBox3 = new SWF.CheckBox();
        _WF1CheckBox4 = new SWF.CheckBox();
        _WF2GroupBox = new SWF.GroupBox();
        _WF2RadioButton1 = new SWF.RadioButton();
        _WF2RadioButton2 = new SWF.RadioButton();
        _WF2RadioButton3 = new SWF.RadioButton();
        _WF2RadioButton4 = new SWF.RadioButton();
        _WF3NumericUpDown = new SWF.NumericUpDown();
        _WF4VScrollBar = new SWF.VScrollBar();
        _WF5HScrollBar = new SWF.HScrollBar();
        _WF6TextBox = new SWF.TextBox();
        _WF7DataGridView = new SWF.DataGridView();

        _WF1Button1.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF1Button2.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF1Button3.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF1Button4.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF1CheckBox1.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF1CheckBox2.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF1CheckBox3.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF1CheckBox4.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF2RadioButton1.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF2RadioButton2.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF2RadioButton3.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF2RadioButton4.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF6TextBox.GotFocus += new EventHandler(WFControl_GotFocus);
        _WF7DataGridView.CellEnter += new DataGridViewCellEventHandler(WFDataGridView_CellEnter);
        _WF3NumericUpDown.ValueChanged += new EventHandler(WFNumericUpDown_ValueChanged);


        _wfh1.Name = WF1Name;
        _WF1FlowLayoutPanel.Name = WF1FlowLayoutPanelName;
        _WF1Button1.Text = _WF1Button1.Name = WF1Button1Name;
        _WF1Button2.Text = _WF1Button2.Name = WF1Button2Name;
        _WF1Button3.Text = _WF1Button3.Name = WF1Button3Name;
        _WF1Button4.Text = _WF1Button4.Name = WF1Button4Name;
        _WF1CheckBox1.Text = _WF1CheckBox1.Name = WF1CheckBox1Name;
        _WF1CheckBox2.Text = _WF1CheckBox2.Name = WF1CheckBox2Name;
        _WF1CheckBox3.Text = _WF1CheckBox3.Name = WF1CheckBox3Name;
        _WF1CheckBox4.Text = _WF1CheckBox4.Name = WF1CheckBox4Name;

        _wfh2.Name = WF2Name;
        _WF2FlowLayoutPanel.Name = WF2FlowLayoutPanelName;
        _WF2RadioButton1.Text = _WF2RadioButton1.Name = WF2RadioButton1Name;
        _WF2RadioButton2.Text = _WF2RadioButton2.Name = WF2RadioButton2Name;
        _WF2RadioButton3.Text = _WF2RadioButton3.Name = WF2RadioButton3Name;
        _WF2RadioButton4.Text = _WF2RadioButton4.Name = WF2RadioButton4Name;
        _WF2GroupBox.Text = _WF2GroupBox.Name = WF2GroupBoxName;

        _wfh3.Name = WF3Name;
        _WF3NumericUpDown.Name = WF3NumericUpDownName;

        _wfh4.Name = WF4Name;
        _WF4VScrollBar.Name = WF4VScrollBarName;
        _WF4VScrollBar.TabStop = true;

        _wfh5.Name = WF5Name;
        _WF5HScrollBar.Name = WF5HScrollBarName;
        _WF5HScrollBar.TabStop = true;

        _wfh6.Name = WF6Name;
        _WF6TextBox.Name = WF6TextBoxName;
        _WF6TextBox.WordWrap = true;
        _WF6TextBox.Multiline = true;
        _WF6TextBox.ScrollBars = ScrollBars.Vertical;
        _WF6TextBox.Text = String.Empty;
        for (int i = 0; i < 50; i++)
            _WF6TextBox.Text += i.ToString() + ": of WinForm Text Box Control" + Environment.NewLine;

        _wfh7.Name = WF7Name;
        _WF7DataGridView.Name = WF7DataGridViewName;
        _WF7DataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        _WF7DataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        _WF7DataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        _WF7DataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        _WF7DataGridView.Rows.Add(5);
        _WF7DataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        foreach (System.Windows.Forms.DataGridViewRow row in _WF7DataGridView.Rows)
        {
            foreach (System.Windows.Forms.DataGridViewCell cell in row.Cells)
                cell.Value = "Cell" + cell.RowIndex + "_" + cell.ColumnIndex;
        }

        _wfh1.Child = _WF1FlowLayoutPanel;
        _WF1FlowLayoutPanel.Controls.Add(_WF1Button1);
        _WF1FlowLayoutPanel.Controls.Add(_WF1Button2);
        _WF1FlowLayoutPanel.Controls.Add(_WF1Button3);
        _WF1FlowLayoutPanel.Controls.Add(_WF1Button4);
        _WF1FlowLayoutPanel.Controls.Add(_WF1CheckBox1);
        _WF1FlowLayoutPanel.Controls.Add(_WF1CheckBox2);
        _WF1FlowLayoutPanel.Controls.Add(_WF1CheckBox3);
        _WF1FlowLayoutPanel.Controls.Add(_WF1CheckBox4);
        _WF1FlowLayoutPanel.AutoScroll = true;
        _WF1FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;


        _wfh2.Child = _WF2FlowLayoutPanel;
        _WF2FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
        _WF2FlowLayoutPanel.AutoSize = true;
        _WF2FlowLayoutPanel.Controls.Add(_WF2GroupBox);
        _WF2GroupBox.Height = 150;
        _WF2GroupBox.Controls.Add(_WF2RadioButton1);
        _WF2GroupBox.Controls.Add(_WF2RadioButton2);
        _WF2GroupBox.Controls.Add(_WF2RadioButton3);
        _WF2GroupBox.Controls.Add(_WF2RadioButton4);
        _WF2RadioButton1.Dock = DockStyle.Bottom;
        _WF2RadioButton2.Dock = DockStyle.Bottom;
        _WF2RadioButton3.Dock = DockStyle.Bottom;
        _WF2RadioButton4.Dock = DockStyle.Bottom;

        _wfh3.Child = _WF3NumericUpDown;
        _wfh4.Child = _WF4VScrollBar;
        _wfh5.Child = _WF5HScrollBar;
        _wfh7.Child = _WF7DataGridView;

        _wfh6.Height = 200;
        _wfh6.Width = 300;
        _wfh6.Child = _WF6TextBox;
        #endregion

        #region LayoutWindow
        this.Title = WindowTitleName;

        _AVDockPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        _AVDockPanel.Children.Add(_AVButton1);
        _AVDockPanel.Children.Add(_AVButton2);
        _AVDockPanel.Children.Add(_AVCheckBox1);
        _AVDockPanel.Children.Add(_AVCheckBox2);
        _AVDockPanel.Children.Add(_AVRadioButton1);
        _AVDockPanel.Children.Add(_AVRadioButton2);

        _AVDockPanel.Children.Add(_wfh1);
        _AVDockPanel.Children.Add(_wfh2);
        _AVDockPanel.Children.Add(_wfh3);
        _AVDockPanel.Children.Add(_wfh4);
        _AVDockPanel.Children.Add(_wfh5);

        _AVButton5.Height = 300;
        _AVDockPanel2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        _AVDockPanel2.Children.Add(_wfh6);
        _AVDockPanel2.Children.Add(_AVButton5);
        SWC.DockPanel.SetDock(_wfh6, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVButton5, SWC.Dock.Top);
        _AVScrollViewer.Content = _AVDockPanel2;
        _AVScrollViewer.Height = 300;
        _AVDockPanel.Children.Add(_AVScrollViewer);

        _AVDockPanel.Children.Add(_wfh7);

        _AVDockPanel.Children.Add(_AVButton3);
        _AVDockPanel.Children.Add(_AVButton4);
        _AVDockPanel.Children.Add(_AVCheckBox3);
        _AVDockPanel.Children.Add(_AVCheckBox4);
        _AVDockPanel.Children.Add(_AVRadioButton3);
        _AVDockPanel.Children.Add(_AVRadioButton4);

        SWC.DockPanel.SetDock(_AVButton1, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVButton2, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVCheckBox1, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVCheckBox2, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVRadioButton1, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVRadioButton2, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_wfh1, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_wfh2, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_wfh3, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_wfh4, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_wfh5, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVScrollViewer, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_wfh7, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVButton3, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVButton4, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVCheckBox3, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVCheckBox4, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVRadioButton3, SWC.Dock.Top);
        SWC.DockPanel.SetDock(_AVRadioButton4, SWC.Dock.Top);

        this.Content = _AVDockPanel;
        #endregion

        p.log.WriteLine("TestSetup -- End ");
    }

    #region EventHandler
    // Event handler for Controls 
    void WFControl_GotFocus(object sender, EventArgs e)
    {
        string name = ((SWF.Control)sender).Name;
        _WFEvents += (name + "_GotFocus:");
    }

    void WFNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        SWF.NumericUpDown n = sender as SWF.NumericUpDown;
        _WFEvents += (n.Name + "-" + n.Value + ":");
    }

    void AVControl_GotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        string name = ((SWC.Control)sender).Name;
        _WFEvents += (name + "_GotFocus:");
    }

    void WFDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
        SWF.DataGridView v = sender as SWF.DataGridView;
        _WFEvents += ("Row-" + v.CurrentCell.RowIndex.ToString() + " Column-" + v.CurrentCell.ColumnIndex.ToString() + ":");
    }

    void AVScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
    {
        SWC.ScrollViewer v = sender as SWC.ScrollViewer;
        _WFEvents += ("VerticalOffset=" + v.VerticalOffset.ToString() + ":");
    }

    #endregion

    /// <summary>
    /// hide or show WFH windows
    /// wfh1 -- Button, CheckBox
    /// wfh2 -- Radio Button
    /// wfh3 -- NumericUpDown
    /// wfh4 -- Vertical ScrollBar
    /// wfh5 -- Horizontal ScrollBar
    /// wfh6 -- TextBox
    /// wfh7 -- DataGridView
    /// </summary>
    /// <param name="ScenarioIndex"></param> zero base index
    private void HideShowGroup(int ScenarioIndex)
    {
        switch (ScenarioIndex)
        {
            // test with Button and Check Box -- wfh1 and wfh2
            case 1:  //@ Move between WF Control's in the same WFH via Arrow Keys.
            case 2:  //@ Verify that Arrow Keys wrap focus between controls in the same WFH
            case 8:  //@ Verify that Shift+Arrow's work
                _wfh1.Visibility = System.Windows.Visibility.Visible;
                _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                _wfh3.Visibility = System.Windows.Visibility.Collapsed;
                _wfh4.Visibility = System.Windows.Visibility.Collapsed;
                _wfh5.Visibility = System.Windows.Visibility.Collapsed;
                _AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                _wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with NumericUpDown -- wfh3
            case 3:  //@ Arrow Keys work for NumericUpDown control in a WFH
                _wfh1.Visibility = System.Windows.Visibility.Collapsed;
                _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                _wfh3.Visibility = System.Windows.Visibility.Visible;
                _wfh4.Visibility = System.Windows.Visibility.Collapsed;
                _wfh5.Visibility = System.Windows.Visibility.Collapsed;
                _AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                _wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with Vertical and Horizontal ScrollBar -- wfh4 and wfh5
            case 4:  //@ Arrow keys work for a horizontal scroll bar in a WFH
                _wfh1.Visibility = System.Windows.Visibility.Collapsed;
                _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                _wfh3.Visibility = System.Windows.Visibility.Collapsed;
                _wfh4.Visibility = System.Windows.Visibility.Visible;
                _wfh5.Visibility = System.Windows.Visibility.Visible;
                _AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                _wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with DataGridView -- wfh7
            case 5:  //@ Arrow Keys work with DGV (Up, Down, Left and Right)
                _wfh1.Visibility = System.Windows.Visibility.Collapsed;
                _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                _wfh3.Visibility = System.Windows.Visibility.Collapsed;
                _wfh4.Visibility = System.Windows.Visibility.Collapsed;
                _wfh5.Visibility = System.Windows.Visibility.Collapsed;
                _AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                _wfh7.Visibility = System.Windows.Visibility.Visible;
                break;
            // test with Radio Button -- wfh2
            case 6:  //@ Verify that Arrow keys cycle through a set of Radio buttons in a group box
                _wfh1.Visibility = System.Windows.Visibility.Collapsed;
                _wfh2.Visibility = System.Windows.Visibility.Visible;
                _wfh3.Visibility = System.Windows.Visibility.Collapsed;
                _wfh4.Visibility = System.Windows.Visibility.Collapsed;
                _wfh5.Visibility = System.Windows.Visibility.Collapsed;
                _AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                _wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with TextBox -- wfh6
            case 7:  //@ When a WFH's parrent is veritically scrolled, verify that a vertical scroll bar on a WF textbox works as expected.
                _wfh1.Visibility = System.Windows.Visibility.Collapsed;
                _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                _wfh3.Visibility = System.Windows.Visibility.Collapsed;
                _wfh4.Visibility = System.Windows.Visibility.Collapsed;
                _wfh5.Visibility = System.Windows.Visibility.Collapsed;
                _AVScrollViewer.Visibility = System.Windows.Visibility.Visible;
                _wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with Button and Check Box -- wfh1 and wfh2
            case 9:  //@ Verify that CTRL+Arrow's work
            case 10:  //@ Verify that Shift+CTRL+Arrows's work
                _wfh1.Visibility = System.Windows.Visibility.Visible;
                _wfh2.Visibility = System.Windows.Visibility.Visible;
                _wfh3.Visibility = System.Windows.Visibility.Collapsed;
                _wfh4.Visibility = System.Windows.Visibility.Collapsed;
                _wfh5.Visibility = System.Windows.Visibility.Collapsed;
                _AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                _wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            case 11:  //@ Verifty that Arrow keys work with a scrollable control (Form or Panel) - AutoScrole=true with children outside of control bounds.
                _wfh1.Visibility = System.Windows.Visibility.Visible;
                _wfh1.Height = 150;
                _wfh1.Width = 100;
                _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                _wfh3.Visibility = System.Windows.Visibility.Collapsed;
                _wfh4.Visibility = System.Windows.Visibility.Collapsed;
                _wfh5.Visibility = System.Windows.Visibility.Collapsed;
                _AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                _wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            default:
                _wfh1.Visibility = System.Windows.Visibility.Visible;
                _wfh2.Visibility = System.Windows.Visibility.Visible;
                _wfh3.Visibility = System.Windows.Visibility.Visible;
                _wfh4.Visibility = System.Windows.Visibility.Visible;
                _wfh5.Visibility = System.Windows.Visibility.Visible;
                _AVScrollViewer.Visibility = System.Windows.Visibility.Visible;
                _wfh7.Visibility = System.Windows.Visibility.Visible;
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
//@ Arrow Keys work for NumericUpDown control in a WFH
//@ Move between WF Control's in the same WFH via Arrow Keys.
//@ Verify that Arrow Keys wrap focus between controls in the same WFH
//@ Arrow keys work for a horizontal scroll bar in a WFH
//@ Arrow Keys work with DGV (Up, Down, Left and Right)
//@ Verify that Arrow keys cycle through a set of Radio buttons in a group box
//@ When a WFH's parrent is veritically scrolled, verify that a vertical scroll bar on a WF textbox works as expected.
//@ Verify that Shift+Arrow's work
//@ Verify that CTRL+Arrow's work
//@ Verify that Shift+CTRL+Arrows's work
//@ Verifty that Arrow keys work with a scrollable control (Form or Panel) - AutoScrole=true with children outside of control bounds.
