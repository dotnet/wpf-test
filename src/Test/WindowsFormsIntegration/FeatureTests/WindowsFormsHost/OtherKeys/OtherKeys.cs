using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;

using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms.Integration;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MS.Internal.Mita.Foundation.Waiters;

//
// Testcase:    OtherKeys
// Description: This test catches any keys not listed in other tests
// Author:      a-wboyde
//
namespace WindowsFormsHostTests
{

public class OtherKeys : WPFReflectBase
{
    #region Testcase setup
    public OtherKeys(string[] args) : base(args) { }


    // class vars
    private string appTitle = "Other Keys TC";      // so that Mita can find it
    private DockPanel _dp;
    private System.Windows.Forms.VScrollBar vScr;
    private System.Windows.Forms.HScrollBar hScr;
    private System.Windows.Forms.TextBox wfTB1;
    private string WFEvents;                        // event sequence string

    protected override void InitTest(TParams p)
    {
        // make window a better size
        this.Width = 600;
        this.Height = 300;

        // set up for Mita
        this.Title = appTitle;
        UseMITA = true;

        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        bool b = base.BeforeScenario(p, scenario);

        // debug - run specific scenario !!!
        //if (scenario.Name != "Scenario1") { return false; }

        // create DockPanel, add to window
        _dp = new DockPanel();
        this.Content = _dp;

        // run setup for particular scenario
        // must do all setup here because Scenarios are called on separate thread
        SetupScenario(p, scenario.Name);

        Utilities.SleepDoEvents(5);

        return b;
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Using a WF scrollbar verify that Pg UP and Pg DWN keys work")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
	Utilities.SleepDoEvents(10);
        int expVal;

        // --- Horizontal Scroll --- //

        // focus on Vertical scrollbar
        p.log.WriteLine("Testing Vertical Scroll bar");
        FindAndFocus(p, "vScroll");
	Utilities.SleepDoEvents(10);

        // Known bug !!!
        // when this bug is fixed, we can complete this Scenario !!!
        //p.log.LogKnownBug(BugDb.VSWhidbey, 579016, "Scroll bars do not respond to keyboard");
        // Note: need to set TabStop = true on scroll bars

        // End
        PressKeys("{END}");
        WPFMiscUtils.IncCounters(sr, 91, vScr.Value, "After {END} Value not as expected", p.log);

        // Home
        PressKeys("{HOME}");
        WPFMiscUtils.IncCounters(sr, 0, vScr.Value, "After {HOME} Value not as expected", p.log);

        // scroll car is now at top
        expVal = 0;
        //!!!p.log.WriteLine("Vertical Scroll value = {0}", vScr.Value);

        // PG Down
        PressKeys("{PGDN}");
        PressKeys("{PGDN}");
        expVal += 2* vScr.LargeChange;
        WPFMiscUtils.IncCounters(sr, expVal, vScr.Value, "After {PGDN} Value not as expected", p.log);

        // PG Up
        PressKeys("{PGUP}");
        expVal -= vScr.LargeChange;
        WPFMiscUtils.IncCounters(sr, expVal, vScr.Value, "After {PGUP} Value not as expected", p.log);

        // Arrow Down
        PressKeys("{DOWN}");
        PressKeys("{DOWN}");
        expVal += 2* vScr.SmallChange;
        WPFMiscUtils.IncCounters(sr, expVal, vScr.Value, "After {DOWN} Value not as expected", p.log);

        // Arrow Up
        PressKeys("{UP}");
        expVal -= 1;
        WPFMiscUtils.IncCounters(sr, expVal, vScr.Value, "After {END} Value not as expected", p.log);

        // --- Horizontal Scroll --- //

        // focus on Horizontal scrollbar
        p.log.WriteLine("Testing Horizontal Scroll bar");
        FindAndFocus(p, "hScroll");

        // End
        PressKeys("{END}");
        WPFMiscUtils.IncCounters(sr, 91, hScr.Value, "After {END} Value not as expected", p.log);

        // Home
        PressKeys("{HOME}");
        WPFMiscUtils.IncCounters(sr, 0, hScr.Value, "After {HOME} Value not as expected", p.log);

        // scroll car is now at left
        expVal = 0;

        // PG Down
        PressKeys("{PGDN}");
        PressKeys("{PGDN}");
        expVal += 2 * hScr.LargeChange;
        WPFMiscUtils.IncCounters(sr, expVal, hScr.Value, "After {PGDN} Value not as expected", p.log);

        // PG Up
        PressKeys("{PGUP}");
        expVal -= hScr.LargeChange;
        WPFMiscUtils.IncCounters(sr, expVal, hScr.Value, "After {PGUP} Value not as expected", p.log);

        // Arrow Down
        PressKeys("{DOWN}");
        PressKeys("{DOWN}");
        expVal += 2 * hScr.SmallChange;
        WPFMiscUtils.IncCounters(sr, expVal, hScr.Value, "After {DOWN} Value not as expected", p.log);

        // Arrow Up
        PressKeys("{UP}");
        expVal -= 1;
        WPFMiscUtils.IncCounters(sr, expVal, hScr.Value, "After {END} Value not as expected", p.log);

        return sr;
    }

    [Scenario("Verify that End and Home keys work (use a text box).")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        int pos;

        // Note: initial text is "TextBox1"

        // focus on textbox
        FindAndFocus(p, "wfTB1");

        // verify initial text
        string txt = wfTB1.Text;
        p.log.WriteLine("Initial TextBox text = '{0}'", txt);
        WPFMiscUtils.IncCounters(sr, "TextBox1", txt, "Text not as expected", p.log);

        // End
        PressKeys("{END}");
        pos = GetCursorPos(p);
        p.log.WriteLine("After End, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 8, pos, "Cursor not at expected position", p.log);

        // Home
        PressKeys("{HOME}");
        pos = GetCursorPos(p);
        p.log.WriteLine("After Home, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 0, pos, "Cursor not at expected position", p.log);

        // Right
        PressKeys("{RIGHT}");
        PressKeys("{RIGHT}");
        PressKeys("{RIGHT}");
        pos = GetCursorPos(p);
        p.log.WriteLine("After 3 rights, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 3, pos, "Cursor not at expected position", p.log);

        // Left
        PressKeys("{LEFT}");
        PressKeys("{LEFT}");
        pos = GetCursorPos(p);
        p.log.WriteLine("After 2 lefts, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 1, pos, "Cursor not at expected position", p.log);

        return sr;
    }

    [Scenario("Verify that Delete Key works")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // focus on TextBox
        FindAndFocus(p, "wfTB1");

        // Note: initial text is "TextBox1"

        // move to front, delete first character
        PressKeys("{HOME}");
        PressKeys("{DEL}");
        WPFMiscUtils.IncCounters(sr, "extBox1", wfTB1.Text, "First char not deleted", p.log);

        // move three chars in, delete char
        PressKeys("{RIGHT 3}");
        PressKeys("{DEL}");
        WPFMiscUtils.IncCounters(sr, "extox1", wfTB1.Text, "Middle char not deleted", p.log);

        // move to end, try to delete char (should not be able to)
        PressKeys("{END}");
        PressKeys("{DEL}");
        WPFMiscUtils.IncCounters(sr, "extox1", wfTB1.Text, "String not as expected", p.log);

        // try backspace to delete last char (should be able to)
        PressKeys("{BKSP}");
        WPFMiscUtils.IncCounters(sr, "extox", wfTB1.Text, "String not as expected", p.log);

        return sr;
    }

    [Scenario("SpaceBar for CheckBoxes, Button, RadioButton...")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // CheckBox
        p.log.WriteLine("-- CheckBox --");
        FindAndFocus(p, "wfCB");
        WFEvents = "";
        PressKeys(" ");
        p.log.WriteLine("After: WFEvents = '{0}'", WFEvents);
        WPFMiscUtils.IncCounters(sr, "wfCB_CheckedChanged:", WFEvents, "Should be 1 hit", p.log);

        // Button
        p.log.WriteLine("-- Button --");
        FindAndFocus(p, "wfBtn");
        WFEvents = "";
        PressKeys(" ");
        p.log.WriteLine("After: WFEvents = '{0}'", WFEvents);
        WPFMiscUtils.IncCounters(sr, "wfBtn_Click:", WFEvents, "Should be 1 hit", p.log);

        // Radio Buttons
        // Note: using Mita to set focus on radio button seems to generate click event? !!!
        p.log.WriteLine("-- Radio Buttons --");
        FindAndFocus(p, "wfRad1");
        Utilities.SleepDoEvents(5);
        WFEvents = "";
        p.log.WriteLine("Before: WFEvents = '{0}'", WFEvents);
        PressKeys(" ");
        p.log.WriteLine("After: WFEvents = '{0}'", WFEvents);
        WPFMiscUtils.IncCounters(sr, p.log, EventInList("wfRad1_Click:", WFEvents), "Did not get Click event");

        FindAndFocus(p, "wfRad2");
        Utilities.SleepDoEvents(5);
        WFEvents = "";
        p.log.WriteLine("Before: WFEvents = '{0}'", WFEvents);
        PressKeys(" ");
        p.log.WriteLine("After: WFEvents = '{0}'", WFEvents);
        WPFMiscUtils.IncCounters(sr, p.log, EventInList("wfRad2_Click:", WFEvents), "Did not get Click event");

        return sr;
    }

    [Scenario("Misc. Text in a MultiLine text box.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        int pos;

        // Note: initial text is "TextBox1"

        // There is nothing really "Interop" here - this Scenario could be superceded
        // by existing WinForms TextBox tests. !!!
        
        // focus on textbox
        FindAndFocus(p, "wfTB1");

        // verify initial text
        string txt = wfTB1.Text;
        p.log.WriteLine("Initial TextBox text = '{0}'", txt);
        WPFMiscUtils.IncCounters(sr, "TextBox1", txt, "Text not as expected", p.log);

        // Add additional text
        PressKeys("{END}");
        PressKeys("{ENTER}");
        PressKeys("Have A");
        PressKeys("{ENTER}");
        PressKeys("Nice Day");
        pos = GetCursorPos(p);
        p.log.WriteLine("After Adding text, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 26, pos, "Cursor not at expected position", p.log);

        // text is now:
        // TextBox1         8 chars + CRLF
        // Have A           6 chars + CRLF
        // Nice Day         8 chars

        // got to beginning of previous line
        PressKeys("{UP}");
        PressKeys("{HOME}");
        pos = GetCursorPos(p);
        p.log.WriteLine("After Move, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 10, pos, "Cursor not at expected position", p.log);

        // End (end of line)
        PressKeys("{END}");
        pos = GetCursorPos(p);
        p.log.WriteLine("After End, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 16, pos, "Cursor not at expected position", p.log);

        // Home (start of line)
        PressKeys("{HOME}");
        pos = GetCursorPos(p);
        p.log.WriteLine("After Home, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 10, pos, "Cursor not at expected position", p.log);

        // Ctrl-End (end of text)
        PressKeys("^{END}");
        pos = GetCursorPos(p);
        p.log.WriteLine("After Ctrl-End, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 26, pos, "Cursor not at expected position", p.log);

        // Ctrl-Home (start of text)
        PressKeys("^{HOME}");
        pos = GetCursorPos(p);
        p.log.WriteLine("After Ctrl-Home, Position = {0}", pos);
        WPFMiscUtils.IncCounters(sr, 0, pos, "Cursor not at expected position", p.log);

        // Unicode / Arabic
        PressKeys("\x0622\x062e\x0631");
        PressKeys("{ENTER}");

        return sr;
    }

    #region Scenario setup

    /// <summary>
    /// Function to set up window for a particular Scenario
    /// </summary>
    /// <param name="p"></param>
    /// <param name="scenario"></param>
    private void SetupScenario(TParams p, string scenario)
    {
        // create controls in window for particular Scenario
        // which scenario? (I think this is cleaner than "switch")
        if (scenario == "Scenario1")
        {
            // 1. Using a WF scrollbar verify that Pg UP and Pg DWN keys work
            CreateHostWithScroll(p);
        }
        else if (scenario == "Scenario2")
        {
            // 2. Verify that End and Home keys work (use a text box).
            CreateHostWithTextbox(p, false);
        }
        else if (scenario == "Scenario3")
        {
            // 3. Verify that Delete Key works
            CreateHostWithTextbox(p, false);
        }
        else if (scenario == "Scenario4")
        {
            // 4. SpaceBar for CheckBoxes, Button, RadioButton...
            CreateHostWithControls(p);
        }
        else if (scenario == "Scenario5")
        {
            // 5. Misc. Text in a MultiLine text box.
            CreateHostWithTextbox(p, true);
        }
        else
        {
            // Did someone add a new Scenario?
            throw new ArgumentException("Unexpected Scenario name '{0}'", scenario);
        }
    }

    /// <summary>
    /// Helper to create WFHs with winForms ScrollBars and add to DockPanel
    /// </summary>
    /// <param name="p"></param>
    private void CreateHostWithScroll(TParams p)
    {
        p.log.WriteLine("Adding two WFH with winForms ScrollBars");

        // create WFH with winForms Vertical ScrollBar
        WindowsFormsHost wfh1 = new WindowsFormsHost();
        vScr = new System.Windows.Forms.VScrollBar();
        vScr.Name = "vScroll";
        vScr.TabStop = true;
        DockPanel.SetDock(wfh1, Dock.Left);
        wfh1.Child = vScr;

        // create WFH with winForms Horizontal ScrollBar
        WindowsFormsHost wfh2 = new WindowsFormsHost();
        hScr = new System.Windows.Forms.HScrollBar();
        hScr.Name = "hScroll";
        hScr.TabStop = true;
        DockPanel.SetDock(wfh2, Dock.Bottom);
        wfh2.Child = hScr;

        // add WFH to window
        _dp.Children.Add(wfh1);
        _dp.Children.Add(wfh2);

        // add another Avalon control to fill up window
        Button avBtn = new Button();
        avBtn.Content = "Avalon";
        _dp.Children.Add(avBtn);
    }

    /// <summary>
    /// Helper to create WFH with Panel with WinForms TextBoxe and add to DockPanel
    /// </summary>
    /// <param name="p"></param>
    /// <param name="bMultiLine"></param>
    private void CreateHostWithTextbox(TParams p, bool bMultiLine)
    {
        p.log.WriteLine("Adding WFH with Panel with winForms TextBox");

        // create WFH with Panel with WF Textboxe
        WindowsFormsHost wfh = new WindowsFormsHost();

        // TextBox 1
        wfTB1 = new System.Windows.Forms.TextBox();
        wfTB1.Name = "wfTB1";
        wfTB1.Text = "TextBox1";

        if (bMultiLine)
        {
            wfTB1.Multiline = true;
            wfTB1.Height = wfTB1.Height * 4;
            //wfTB1.Text += "\nHave A\nNice Day";
        }

        // add TextBox to Panel
        System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
        panel.Controls.Add(wfTB1);

        // add Panel to WFH
        wfh.Child = panel;

        // add WFH to window
        _dp.Children.Add(wfh);
    }

    /// <summary>
    /// Helper to create WFH with FlowLayoutPanel with misc WinForms controls
    /// </summary>
    /// <param name="p"></param>
    private void CreateHostWithControls(TParams p)
    {
        p.log.WriteLine("Adding WFH with FLP with misc winForms controls");

        // create WFH with FlowLayoutPanel with 4 WF Textboxes
        WindowsFormsHost wfh = new WindowsFormsHost();

        // CheckBox
        System.Windows.Forms.CheckBox wfCB = new System.Windows.Forms.CheckBox();
        wfCB.Name = "wfCB";
        wfCB.Text = "CheckBox";
        wfCB.CheckedChanged += new EventHandler(wfCB_CheckedChanged);

        // Button
        System.Windows.Forms.Button wfBtn = new System.Windows.Forms.Button();
        wfBtn.Name = "wfBtn";
        wfBtn.Text = "Button";
        wfBtn.Click += new EventHandler(wfBtn_Click);

        // Radio Button 1
        System.Windows.Forms.RadioButton wfRad1 = new System.Windows.Forms.RadioButton();
        wfRad1.Name = "wfRad1";
        wfRad1.Text = "Radio 1";
        wfRad1.Click += new EventHandler(wfRad_Click);

        // Radio Button 2
        System.Windows.Forms.RadioButton wfRad2 = new System.Windows.Forms.RadioButton();
        wfRad2.Name = "wfRad2";
        wfRad2.Text = "Radio 2";
        wfRad2.Click += new EventHandler(wfRad_Click);

        // Radio Button 3
        System.Windows.Forms.RadioButton wfRad3 = new System.Windows.Forms.RadioButton();
        wfRad3.Name = "wfRad3";
        wfRad3.Text = "Radio 3";
        wfRad3.Click += new EventHandler(wfRad_Click);

        // add controls to FLP
        System.Windows.Forms.FlowLayoutPanel flp = new System.Windows.Forms.FlowLayoutPanel();
        flp.Controls.AddRange(new System.Windows.Forms.Control[] { wfCB, wfBtn, wfRad1, wfRad2, wfRad3 });

        // add FLP to WFH
        wfh.Child = flp;

        // add WFH to window
        _dp.Children.Add(wfh);
    }

    // Event handler for Radio Button
    void wfRad_Click(object sender, EventArgs e)
    {
        string name = ((System.Windows.Forms.RadioButton)sender).Name;
        WFEvents += (name + "_Click:");
        //!!!scenarioParams.log.WriteLine("in wfRad_Click '{0}'", name);
    }

    // Event handler for Button
    void wfBtn_Click(object sender, EventArgs e)
    {
        string name = ((System.Windows.Forms.Button)sender).Name;
        WFEvents += (name + "_Click:");
        //!!!scenarioParams.log.WriteLine("in wfBtn_Click '{0}'", name);
    }

    // Event handler for CheckBox
    void wfCB_CheckedChanged(object sender, EventArgs e)
    {
        string name = ((System.Windows.Forms.CheckBox)sender).Name;
        WFEvents += (name + "_CheckedChanged:");
        //!!!scenarioParams.log.WriteLine("in wfCB_CheckedChanged '{0}'", name);
    }

    #endregion

    #region Helper functions

    // need this for GetCursorPos
    public delegate int MyDelegateType();

    /// <summary>
    /// Helper function to query cursor position within TextBox
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    private int GetCursorPos(TParams p)
    {
        // define delegate that queries cursor position
        MyDelegateType MethodGetCursorPos = delegate
        {
            // determine cursor position based on "SelectionStart"
            return wfTB1.SelectionStart;
        };

        // call method on thread which created TextBox
        int idx = (int)wfTB1.Invoke(MethodGetCursorPos);

        return idx;
    }

    /// <summary>
    /// Helper function to locate a control by name and set focus to it
    /// </summary>
    /// <param name="p"></param>
    /// <param name="ctrlName"></param>
    private void FindAndFocus(TParams p, string ctrlName)
    {
        p.log.WriteLine("FindAndFocus: '{0}'", ctrlName);

        try
        {
            // find app window
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(appTitle));
            //UIObject uiApp = UIObject.Root.Descendants.Find(UICondition.CreateFromName(appTitle));
            p.log.WriteLine("Found app: uiApp = '{0}'", uiApp);

            // find control by name
            UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(ctrlName));
            p.log.WriteLine("Found control: ctrl = '{0}'", ctrl);

            // focus on control
            // for some reason, sometimes this takes some time !!! Scenario4, debug?
            //p.log.WriteLine("Before Set focus to control");
            ctrl.SetFocus();
            //p.log.WriteLine("After set focus to control");
        }
        catch (Exception e)
        {
            p.log.WriteLine("Got exception '{0}'", e.Message);
        }
    }

    /// <summary>
    /// Helper function to press specified keys, process events, and pause
    /// </summary>
    /// <param name="keys"></param>
    private void PressKeys(string keys)
    {
        // !!!
        //private UIObject uiAppMain;
        // find main app window
        //uiAppMain = UIObject.Root.Children.Find(UICondition.CreateFromName(appTitle));
        // only send keys to main app window
        //uiAppMain.SendKeys(keys);

        Keyboard.Instance.SendKeys(keys);

        Utilities.SleepDoEvents(5);
        //System.Threading.Thread.Sleep(500);
    }

    /// <summary>
    /// Helper to determine if specific event is included in list of events
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="evtList"></param>
    /// <returns></returns>
    private bool EventInList(string evt, string evtList)
    {
        if (evtList.IndexOf(evt) == -1) { return false; }
        else { return true; }
    }

    #endregion

    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Using a WF scrollbar verify that Pg UP and Pg DWN keys work

//@ Verify that End and Home keys work (use a text box).

//@ Verify that Delete Key works

//@ SpaceBar for CheckBoxes, Button, RadioButton...

//@ Misc. Text in a MultiLine text box.