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

//
// Testcase:    ArrowkeysAndPgUpPgDwn
// Description: Verify that WF controls expecting Arrow Key and Page key input work as expected
// Author:      pachan
//
namespace WindowsFormsHostTests
{

public class ArrowkeysAndPgUpPgDwn : WPFReflectBase
{
    #region TestVariables

    private delegate void myEventHandler(object sender);
    private static MethodInfo[] mi;
    private UIObject uiApp;
    private int ScenarioIndex = 0;
    private string WFEvents;                        // event sequence string

    #region AVControls
    private SWC.ScrollViewer AVScrollViewer;
    private SWC.DockPanel AVDockPanel;
    private SWC.DockPanel AVDockPanel2;
    private SWC.Button AVButton1;
    private SWC.Button AVButton2;
    private SWC.Button AVButton3;
    private SWC.Button AVButton4;
    private SWC.Button AVButton5;
    private SWC.CheckBox AVCheckBox1;
    private SWC.CheckBox AVCheckBox2;
    private SWC.CheckBox AVCheckBox3;
    private SWC.CheckBox AVCheckBox4;
    private SWC.RadioButton AVRadioButton1;
    private SWC.RadioButton AVRadioButton2;
    private SWC.RadioButton AVRadioButton3;
    private SWC.RadioButton AVRadioButton4;
    #endregion

    #region WFControls
    private WindowsFormsHost wfh1;
    private WindowsFormsHost wfh2;
    private WindowsFormsHost wfh3;
    private WindowsFormsHost wfh4;
    private WindowsFormsHost wfh5;
    private WindowsFormsHost wfh6;
    private WindowsFormsHost wfh7;
    private SWF.FlowLayoutPanel WF1FlowLayoutPanel;
    private SWF.FlowLayoutPanel WF2FlowLayoutPanel;
    private SWF.Button WF1Button1;
    private SWF.Button WF1Button2;
    private SWF.Button WF1Button3;
    private SWF.Button WF1Button4;
    private SWF.CheckBox WF1CheckBox1;
    private SWF.CheckBox WF1CheckBox2;
    private SWF.CheckBox WF1CheckBox3;
    private SWF.CheckBox WF1CheckBox4;
    private SWF.GroupBox WF2GroupBox;
    private SWF.RadioButton WF2RadioButton1;
    private SWF.RadioButton WF2RadioButton2;
    private SWF.RadioButton WF2RadioButton3;
    private SWF.RadioButton WF2RadioButton4;
    private SWF.NumericUpDown WF3NumericUpDown;
    private SWF.VScrollBar WF4VScrollBar;
    private SWF.HScrollBar WF5HScrollBar;
    private SWF.TextBox WF6TextBox;
    private SWF.DataGridView WF7DataGridView;
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
        ScenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
        HideShowGroup(ScenarioIndex);
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
        mi = GetAllScenarios(this);
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
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF NumericUpDown control");
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF3NumericUpDownName));

        WFEvents = String.Empty;;
        ctrl.SetFocus();

        // Key up for 15 times
        expVal = String.Empty; 
        for (i = 0; i < 15; i++)
        {
            expVal += "WF3NumericUpDown-" + (i+1).ToString() + ":";
            uiApp.SendKeys("{UP}");
        }
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {UP} Value not as expected", p.log);

        // Key down for 15 times
        expVal = String.Empty; 
        WFEvents = String.Empty;
        for (i = 15; i > 0; i--)
        {
            expVal += "WF3NumericUpDown-" + (i-1).ToString() + ":";
            uiApp.SendKeys("{DOWN}");
        }
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {DOWN} Value not as expected", p.log);
        return sr;

    }
    [Scenario("Move between WF Control's in the same WFH via Arrow Keys.")]
    public ScenarioResult Scenario1(TParams p) 
    {
        //p.log.WriteLine("{0} - Test Run Start", GetScenarioAttribute(mi[ScenarioIndex]).Description);
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF1Button1Name));

        p.log.WriteLine("Testing Arrow Keys on WF Button/CheckBox controls");

        // Press a mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT}
        expVal += String.Empty;
        WFEvents = String.Empty;

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

        uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(2);
        uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(2);
        uiApp.SendKeys("{LEFT}");
        Utilities.SleepDoEvents(2);
        uiApp.SendKeys("{LEFT}");
        Utilities.SleepDoEvents(2);
        uiApp.SendKeys("{DOWN}");
        Utilities.SleepDoEvents(2);
        uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(2);
        uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(2);
        uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(2);
        //        uiApp.SendKeys("{RIGHT}{RIGHT}{DOWN}{UP}{UP}");
//        uiApp.SendKeys("{LEFT}{DOWN}{UP}{DOWN}{UP}{UP}{RIGHT}");
//        Utilities.SleepDoEvents(20);
        
        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        //p.log.WriteLine("{0} - Test Run End", GetScenarioAttribute(mi[ScenarioIndex]).Description);
        return sr;
    }

    [Scenario("Verify that Arrow Keys wrap focus between controls in the same WFH")]
    public ScenarioResult Scenario2(TParams p) 
    {
         int i = 0;
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF Button/CheckBox controls");
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF1Button1Name));

        // Press Down 15 times.. should wrap around within WFH1 
        WFEvents = String.Empty;
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
            uiApp.SendKeys("{DOWN}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {DOWN} Value not as expected", p.log);

        // Press Up 15 times.. should wrap around within WFH1 
        WFEvents = String.Empty;
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
            uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {UP} Value not as expected", p.log);

        // Press Left 15 times.. should wrap around within WFH1 
        WFEvents = String.Empty;
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
            uiApp.SendKeys("{LEFT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {LEFT} Value not as expected", p.log);

        // Press Right 15 times.. should wrap around within WFH1 
        WFEvents = String.Empty;
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
            uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {RIGHT} Value not as expected", p.log);
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
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF DGV controls");
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF7DataGridViewName));

        expVal = String.Empty; 
        WFEvents = String.Empty;
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
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("Verify that Arrow keys cycle through a set of Radio buttons in a group box")]
    public ScenarioResult Scenario6(TParams p) 
    {
        int i = 0;
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF Radio Button controls");
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF2RadioButton1Name));

        // Press Down 15 times.. should wrap around within WFH1 
        WFEvents = String.Empty;
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
            uiApp.SendKeys("{DOWN}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {DOWN} Value not as expected", p.log);

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
        WFEvents = String.Empty;

        // focus on the first WF button
        ctrl.SetFocus();

        for (i = 0; i < 15; i++)
            uiApp.SendKeys("{UP}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {UP} Value not as expected", p.log);

        // Press Left 15 times.. should wrap around within WFH1 
        WFEvents = String.Empty;
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
            uiApp.SendKeys("{LEFT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {LEFT} Value not as expected", p.log);

        // Press Right 15 times.. should wrap around within WFH1 
        WFEvents = String.Empty;
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
            uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After repeated {RIGHT} Value not as expected", p.log);

        // Press a mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT}
        WFEvents = String.Empty;
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

        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("When a WFH's parent is vertically scrolled, verify that a vertical scroll bar on a WF textbox works as expected.")]
    public ScenarioResult Scenario7(TParams p)
    {
        string expVal = String.Empty;
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF6TextBoxName));

        p.log.WriteLine("Testing on AV Vertical Scroll Bar");

        WFEvents = String.Empty;
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
        int lineNo = ((int)(WF6TextBox.Height / WF6TextBox.Font.Height)) - 1;
        expVal = lineNo.ToString() + ": of WinForm Text Box Control";
        ctrl.SendKeys("^{HOME}{PGDN}{HOME}+{END}");
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, expVal, WF6TextBox.SelectedText, "Text did not scroll", p.log);

        return sr;
    }

    [Scenario("Verify that Shift+Arrow's work")]
    public ScenarioResult Scenario8(TParams p) 
    {
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Shift+Arrow Keys on WF Button / CheckBox controls");
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF1Button1Name));

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
        WFEvents = String.Empty;

        // focus on the first WF button
        ctrl.SetFocus();

        uiApp.SendKeys("+{UP}");
        uiApp.SendKeys("+{RIGHT}");
        uiApp.SendKeys("+{LEFT}");
        uiApp.SendKeys("+{LEFT}");
        uiApp.SendKeys("+{DOWN}");
        uiApp.SendKeys("+{UP}");
        uiApp.SendKeys("+{UP}");
        uiApp.SendKeys("+{UP}");
        uiApp.SendKeys("+{RIGHT}");
        uiApp.SendKeys("+{RIGHT}");
        uiApp.SendKeys("+{DOWN}");
        uiApp.SendKeys("+{UP}");
        uiApp.SendKeys("+{UP}");
        uiApp.SendKeys("+{LEFT}");
        uiApp.SendKeys("+{DOWN}");
        uiApp.SendKeys("+{UP}");
        uiApp.SendKeys("+{DOWN}");
        uiApp.SendKeys("+{UP}");
        uiApp.SendKeys("+{UP}");
        uiApp.SendKeys("+{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After mixture of SHIFT + {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("Verify that CTRL+Arrow's work")]
    public ScenarioResult Scenario9(TParams p) 
    {
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Shift+Arrow Keys on both AV and WF controls");
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(AVButton1Name));

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
        WFEvents = String.Empty;

        // focus on the first WF button
        ctrl.SetFocus();

        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{UP}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{RIGHT}");
        uiApp.SendKeys("^{RIGHT}");
        uiApp.SendKeys("^{RIGHT}");
        uiApp.SendKeys("^{RIGHT}");
        uiApp.SendKeys("^{LEFT}");
        uiApp.SendKeys("^{LEFT}");
        uiApp.SendKeys("^{LEFT}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        uiApp.SendKeys("^{DOWN}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After mixture of SHIFT + {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("Verify that Shift+CTRL+Arrows's work")]
    public ScenarioResult Scenario10(TParams p) 
    {
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Shift+Arrow Keys on both AV and WF controls");
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(AVButton1Name));

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
        WFEvents = String.Empty;

        // focus on the first WF button
        ctrl.SetFocus();

        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{UP}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{RIGHT}");
        uiApp.SendKeys("+^{RIGHT}");
        uiApp.SendKeys("+^{RIGHT}");
        uiApp.SendKeys("+^{RIGHT}");
        uiApp.SendKeys("+^{LEFT}");
        uiApp.SendKeys("+^{LEFT}");
        uiApp.SendKeys("+^{LEFT}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        uiApp.SendKeys("+^{DOWN}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After mixture of SHIFT + {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    [Scenario("Verifty that Arrow keys work with a scrollable control (Form or Panel) - AutoScrole=true with children outside of control bounds.")]
    public ScenarioResult Scenario11(TParams p) 
    {
        string expVal = String.Empty; 
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));

        p.log.WriteLine("Testing Arrow Keys on WF Button/CheckBox controls");
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WF1Button1Name));

        // Press a mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT}
        expVal = String.Empty; 
        WFEvents = String.Empty;

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

        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{RIGHT}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{LEFT}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{DOWN}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{UP}");
        uiApp.SendKeys("{RIGHT}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, WFEvents, "After mixture of {UP}, {DOWN}, {LEFT}, and {RIGHT} Value not as expected", p.log);
        return sr;
    }

    #endregion
    
    #region HelperFunction

    private void TestSetup(TParams p)
    {
        p.log.WriteLine("TestSetup -- Start ");

        #region SetupAVControl

        AVScrollViewer = new SWC.ScrollViewer();
        AVDockPanel = new SWC.DockPanel();
        AVDockPanel2 = new SWC.DockPanel();
        AVButton1 = new SWC.Button();
        AVButton2 = new SWC.Button();
        AVButton3 = new SWC.Button();
        AVButton4 = new SWC.Button();
        AVButton5 = new SWC.Button();
        AVCheckBox1 = new SWC.CheckBox();
        AVCheckBox2 = new SWC.CheckBox();
        AVCheckBox3 = new SWC.CheckBox();
        AVCheckBox4 = new SWC.CheckBox();
        AVRadioButton1 = new SWC.RadioButton();
        AVRadioButton2 = new SWC.RadioButton();
        AVRadioButton3 = new SWC.RadioButton();
        AVRadioButton4 = new SWC.RadioButton();

        AVButton1.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVButton2.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVButton3.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVButton4.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVCheckBox1.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVCheckBox2.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVCheckBox3.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVCheckBox4.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVRadioButton1.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVRadioButton2.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVRadioButton3.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVRadioButton4.GotFocus += new System.Windows.RoutedEventHandler(AVControl_GotFocus);
        AVScrollViewer.ScrollChanged += new System.Windows.Controls.ScrollChangedEventHandler(AVScrollViewer_ScrollChanged);

        AVScrollViewer.Name = AVScrollViewerName;
        AVDockPanel.Name = AVDockPanelName;
        AVDockPanel2.Name = AVDockPanel2Name;
        AVButton1.Content = AVButton1.Name = AVButton1Name;
        AVButton2.Content = AVButton2.Name = AVButton2Name;
        AVButton3.Content = AVButton3.Name = AVButton3Name;
        AVButton4.Content = AVButton4.Name = AVButton4Name;
        AVButton5.Content = AVButton5.Name = AVButton5Name;
        AVCheckBox1.Content = AVCheckBox1.Name = AVCheckBox1Name;
        AVCheckBox2.Content = AVCheckBox2.Name = AVCheckBox2Name;
        AVCheckBox3.Content = AVCheckBox3.Name = AVCheckBox3Name;
        AVCheckBox4.Content = AVCheckBox4.Name = AVCheckBox4Name;
        AVRadioButton1.Content = AVRadioButton1.Name = AVRadioButton1Name;
        AVRadioButton2.Content = AVRadioButton2.Name = AVRadioButton2Name;
        AVRadioButton3.Content = AVRadioButton3.Name = AVRadioButton3Name;
        AVRadioButton4.Content = AVRadioButton4.Name = AVRadioButton4Name;

#endregion

        #region SetupWFControl
        wfh1 = new WindowsFormsHost();
        wfh2 = new WindowsFormsHost();
        wfh3 = new WindowsFormsHost();
        wfh4 = new WindowsFormsHost();
        wfh5 = new WindowsFormsHost();
        wfh6 = new WindowsFormsHost();
        wfh7 = new WindowsFormsHost();
        WF1FlowLayoutPanel = new SWF.FlowLayoutPanel();
        WF2FlowLayoutPanel = new SWF.FlowLayoutPanel();
        WF1Button1 = new SWF.Button();
        WF1Button2 = new SWF.Button();
        WF1Button3 = new SWF.Button();
        WF1Button4 = new SWF.Button();
        WF1CheckBox1 = new SWF.CheckBox();
        WF1CheckBox2 = new SWF.CheckBox();
        WF1CheckBox3 = new SWF.CheckBox();
        WF1CheckBox4 = new SWF.CheckBox();
        WF2GroupBox = new SWF.GroupBox();
        WF2RadioButton1 = new SWF.RadioButton();
        WF2RadioButton2 = new SWF.RadioButton();
        WF2RadioButton3 = new SWF.RadioButton();
        WF2RadioButton4 = new SWF.RadioButton();
        WF3NumericUpDown = new SWF.NumericUpDown();
        WF4VScrollBar = new SWF.VScrollBar();
        WF5HScrollBar = new SWF.HScrollBar();
        WF6TextBox = new SWF.TextBox();
        WF7DataGridView = new SWF.DataGridView();

        WF1Button1.GotFocus += new EventHandler(WFControl_GotFocus);
        WF1Button2.GotFocus += new EventHandler(WFControl_GotFocus);
        WF1Button3.GotFocus += new EventHandler(WFControl_GotFocus);
        WF1Button4.GotFocus += new EventHandler(WFControl_GotFocus);
        WF1CheckBox1.GotFocus += new EventHandler(WFControl_GotFocus);
        WF1CheckBox2.GotFocus += new EventHandler(WFControl_GotFocus);
        WF1CheckBox3.GotFocus += new EventHandler(WFControl_GotFocus);
        WF1CheckBox4.GotFocus += new EventHandler(WFControl_GotFocus);
        WF2RadioButton1.GotFocus += new EventHandler(WFControl_GotFocus);
        WF2RadioButton2.GotFocus += new EventHandler(WFControl_GotFocus);
        WF2RadioButton3.GotFocus += new EventHandler(WFControl_GotFocus);
        WF2RadioButton4.GotFocus += new EventHandler(WFControl_GotFocus);
        WF6TextBox.GotFocus += new EventHandler(WFControl_GotFocus);
        WF7DataGridView.CellEnter += new DataGridViewCellEventHandler(WFDataGridView_CellEnter);
        WF3NumericUpDown.ValueChanged += new EventHandler(WFNumericUpDown_ValueChanged);


        wfh1.Name = WF1Name;
        WF1FlowLayoutPanel.Name = WF1FlowLayoutPanelName;
        WF1Button1.Text = WF1Button1.Name = WF1Button1Name;
        WF1Button2.Text = WF1Button2.Name = WF1Button2Name;
        WF1Button3.Text = WF1Button3.Name = WF1Button3Name;
        WF1Button4.Text = WF1Button4.Name = WF1Button4Name;
        WF1CheckBox1.Text = WF1CheckBox1.Name = WF1CheckBox1Name;
        WF1CheckBox2.Text = WF1CheckBox2.Name = WF1CheckBox2Name;
        WF1CheckBox3.Text = WF1CheckBox3.Name = WF1CheckBox3Name;
        WF1CheckBox4.Text = WF1CheckBox4.Name = WF1CheckBox4Name;

        wfh2.Name = WF2Name;
        WF2FlowLayoutPanel.Name = WF2FlowLayoutPanelName;
        WF2RadioButton1.Text = WF2RadioButton1.Name = WF2RadioButton1Name;
        WF2RadioButton2.Text = WF2RadioButton2.Name = WF2RadioButton2Name;
        WF2RadioButton3.Text = WF2RadioButton3.Name = WF2RadioButton3Name;
        WF2RadioButton4.Text = WF2RadioButton4.Name = WF2RadioButton4Name;
        WF2GroupBox.Text = WF2GroupBox.Name = WF2GroupBoxName;

        wfh3.Name = WF3Name;
        WF3NumericUpDown.Name = WF3NumericUpDownName;

        wfh4.Name = WF4Name;
        WF4VScrollBar.Name = WF4VScrollBarName;
        WF4VScrollBar.TabStop = true;

        wfh5.Name = WF5Name;
        WF5HScrollBar.Name = WF5HScrollBarName;
        WF5HScrollBar.TabStop = true;

        wfh6.Name = WF6Name;
        WF6TextBox.Name = WF6TextBoxName;
        WF6TextBox.WordWrap = true;
        WF6TextBox.Multiline = true;
        WF6TextBox.ScrollBars = ScrollBars.Vertical;
        WF6TextBox.Text = String.Empty;
        for (int i = 0; i < 50; i++)
            WF6TextBox.Text += i.ToString() + ": of WinForm Text Box Control" + Environment.NewLine;

        wfh7.Name = WF7Name;
        WF7DataGridView.Name = WF7DataGridViewName;
        WF7DataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        WF7DataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        WF7DataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        WF7DataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        WF7DataGridView.Rows.Add(5);
        WF7DataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        foreach (System.Windows.Forms.DataGridViewRow row in WF7DataGridView.Rows)
        {
            foreach (System.Windows.Forms.DataGridViewCell cell in row.Cells)
                cell.Value = "Cell" + cell.RowIndex + "_" + cell.ColumnIndex;
        }

        wfh1.Child = WF1FlowLayoutPanel;
        WF1FlowLayoutPanel.Controls.Add(WF1Button1);
        WF1FlowLayoutPanel.Controls.Add(WF1Button2);
        WF1FlowLayoutPanel.Controls.Add(WF1Button3);
        WF1FlowLayoutPanel.Controls.Add(WF1Button4);
        WF1FlowLayoutPanel.Controls.Add(WF1CheckBox1);
        WF1FlowLayoutPanel.Controls.Add(WF1CheckBox2);
        WF1FlowLayoutPanel.Controls.Add(WF1CheckBox3);
        WF1FlowLayoutPanel.Controls.Add(WF1CheckBox4);
        WF1FlowLayoutPanel.AutoScroll = true;
        WF1FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;


        wfh2.Child = WF2FlowLayoutPanel;
        WF2FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
        WF2FlowLayoutPanel.AutoSize = true;
        WF2FlowLayoutPanel.Controls.Add(WF2GroupBox);
        WF2GroupBox.Height = 150;
        WF2GroupBox.Controls.Add(WF2RadioButton1);
        WF2GroupBox.Controls.Add(WF2RadioButton2);
        WF2GroupBox.Controls.Add(WF2RadioButton3);
        WF2GroupBox.Controls.Add(WF2RadioButton4);
        WF2RadioButton1.Dock = DockStyle.Bottom;
        WF2RadioButton2.Dock = DockStyle.Bottom;
        WF2RadioButton3.Dock = DockStyle.Bottom;
        WF2RadioButton4.Dock = DockStyle.Bottom;

        wfh3.Child = WF3NumericUpDown;
        wfh4.Child = WF4VScrollBar;
        wfh5.Child = WF5HScrollBar;
        wfh7.Child = WF7DataGridView;

        wfh6.Height = 200;
        wfh6.Width = 300;
        wfh6.Child = WF6TextBox;
        #endregion

        #region LayoutWindow
        this.Title = WindowTitleName;

        AVDockPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        AVDockPanel.Children.Add(AVButton1);
        AVDockPanel.Children.Add(AVButton2);
        AVDockPanel.Children.Add(AVCheckBox1);
        AVDockPanel.Children.Add(AVCheckBox2);
        AVDockPanel.Children.Add(AVRadioButton1);
        AVDockPanel.Children.Add(AVRadioButton2);

        AVDockPanel.Children.Add(wfh1);
        AVDockPanel.Children.Add(wfh2);
        AVDockPanel.Children.Add(wfh3);
        AVDockPanel.Children.Add(wfh4);
        AVDockPanel.Children.Add(wfh5);

        AVButton5.Height = 300;
        AVDockPanel2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        AVDockPanel2.Children.Add(wfh6);
        AVDockPanel2.Children.Add(AVButton5);
        SWC.DockPanel.SetDock(wfh6, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVButton5, SWC.Dock.Top);
        AVScrollViewer.Content = AVDockPanel2;
        AVScrollViewer.Height = 300;
        AVDockPanel.Children.Add(AVScrollViewer);

        AVDockPanel.Children.Add(wfh7);

        AVDockPanel.Children.Add(AVButton3);
        AVDockPanel.Children.Add(AVButton4);
        AVDockPanel.Children.Add(AVCheckBox3);
        AVDockPanel.Children.Add(AVCheckBox4);
        AVDockPanel.Children.Add(AVRadioButton3);
        AVDockPanel.Children.Add(AVRadioButton4);

        SWC.DockPanel.SetDock(AVButton1, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVButton2, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVCheckBox1, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVCheckBox2, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVRadioButton1, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVRadioButton2, SWC.Dock.Top);
        SWC.DockPanel.SetDock(wfh1, SWC.Dock.Top);
        SWC.DockPanel.SetDock(wfh2, SWC.Dock.Top);
        SWC.DockPanel.SetDock(wfh3, SWC.Dock.Top);
        SWC.DockPanel.SetDock(wfh4, SWC.Dock.Top);
        SWC.DockPanel.SetDock(wfh5, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVScrollViewer, SWC.Dock.Top);
        SWC.DockPanel.SetDock(wfh7, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVButton3, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVButton4, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVCheckBox3, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVCheckBox4, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVRadioButton3, SWC.Dock.Top);
        SWC.DockPanel.SetDock(AVRadioButton4, SWC.Dock.Top);

        this.Content = AVDockPanel;
        #endregion

        p.log.WriteLine("TestSetup -- End ");
    }

    #region EventHandler
    // Event handler for Controls 
    void WFControl_GotFocus(object sender, EventArgs e)
    {
        string name = ((SWF.Control)sender).Name;
        WFEvents += (name + "_GotFocus:");
    }

    void WFNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        SWF.NumericUpDown n = sender as SWF.NumericUpDown;
        WFEvents += (n.Name + "-" + n.Value + ":");
    }

    void AVControl_GotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        string name = ((SWC.Control)sender).Name;
        WFEvents += (name + "_GotFocus:");
    }

    void WFDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
        SWF.DataGridView v = sender as SWF.DataGridView;
        WFEvents += ("Row-" + v.CurrentCell.RowIndex.ToString() + " Column-" + v.CurrentCell.ColumnIndex.ToString() + ":");
    }

    void AVScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
    {
        SWC.ScrollViewer v = sender as SWC.ScrollViewer;
        WFEvents += ("VerticalOffset=" + v.VerticalOffset.ToString() + ":");
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
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                wfh3.Visibility = System.Windows.Visibility.Collapsed;
                wfh4.Visibility = System.Windows.Visibility.Collapsed;
                wfh5.Visibility = System.Windows.Visibility.Collapsed;
                AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with NumericUpDown -- wfh3
            case 3:  //@ Arrow Keys work for NumericUpDown control in a WFH
                wfh1.Visibility = System.Windows.Visibility.Collapsed;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                wfh3.Visibility = System.Windows.Visibility.Visible;
                wfh4.Visibility = System.Windows.Visibility.Collapsed;
                wfh5.Visibility = System.Windows.Visibility.Collapsed;
                AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with Vertical and Horizontal ScrollBar -- wfh4 and wfh5
            case 4:  //@ Arrow keys work for a horizontal scroll bar in a WFH
                wfh1.Visibility = System.Windows.Visibility.Collapsed;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                wfh3.Visibility = System.Windows.Visibility.Collapsed;
                wfh4.Visibility = System.Windows.Visibility.Visible;
                wfh5.Visibility = System.Windows.Visibility.Visible;
                AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with DataGridView -- wfh7
            case 5:  //@ Arrow Keys work with DGV (Up, Down, Left and Right)
                wfh1.Visibility = System.Windows.Visibility.Collapsed;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                wfh3.Visibility = System.Windows.Visibility.Collapsed;
                wfh4.Visibility = System.Windows.Visibility.Collapsed;
                wfh5.Visibility = System.Windows.Visibility.Collapsed;
                AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                wfh7.Visibility = System.Windows.Visibility.Visible;
                break;
            // test with Radio Button -- wfh2
            case 6:  //@ Verify that Arrow keys cycle through a set of Radio buttons in a group box
                wfh1.Visibility = System.Windows.Visibility.Collapsed;
                wfh2.Visibility = System.Windows.Visibility.Visible;
                wfh3.Visibility = System.Windows.Visibility.Collapsed;
                wfh4.Visibility = System.Windows.Visibility.Collapsed;
                wfh5.Visibility = System.Windows.Visibility.Collapsed;
                AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with TextBox -- wfh6
            case 7:  //@ When a WFH's parrent is veritically scrolled, verify that a vertical scroll bar on a WF textbox works as expected.
                wfh1.Visibility = System.Windows.Visibility.Collapsed;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                wfh3.Visibility = System.Windows.Visibility.Collapsed;
                wfh4.Visibility = System.Windows.Visibility.Collapsed;
                wfh5.Visibility = System.Windows.Visibility.Collapsed;
                AVScrollViewer.Visibility = System.Windows.Visibility.Visible;
                wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            // test with Button and Check Box -- wfh1 and wfh2
            case 9:  //@ Verify that CTRL+Arrow's work
            case 10:  //@ Verify that Shift+CTRL+Arrows's work
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Visible;
                wfh3.Visibility = System.Windows.Visibility.Collapsed;
                wfh4.Visibility = System.Windows.Visibility.Collapsed;
                wfh5.Visibility = System.Windows.Visibility.Collapsed;
                AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            case 11:  //@ Verifty that Arrow keys work with a scrollable control (Form or Panel) - AutoScrole=true with children outside of control bounds.
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh1.Height = 150;
                wfh1.Width = 100;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                wfh3.Visibility = System.Windows.Visibility.Collapsed;
                wfh4.Visibility = System.Windows.Visibility.Collapsed;
                wfh5.Visibility = System.Windows.Visibility.Collapsed;
                AVScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                wfh7.Visibility = System.Windows.Visibility.Collapsed;
                break;
            default:
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Visible;
                wfh3.Visibility = System.Windows.Visibility.Visible;
                wfh4.Visibility = System.Windows.Visibility.Visible;
                wfh5.Visibility = System.Windows.Visibility.Visible;
                AVScrollViewer.Visibility = System.Windows.Visibility.Visible;
                wfh7.Visibility = System.Windows.Visibility.Visible;
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
