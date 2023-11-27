// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Automation;
using MS.Internal.Mita.Foundation;

//
// Testcase:    Tabs
// Description: Verify that the Tab key work correctly across WF/AV
//
public class Tabs : ReflectBase {
    #region Testcase setup
    public Tabs(string[] args) : base(args) { }

    TabsUserControl.UserControl1 _uc = new TabsUserControl.UserControl1();
    ElementHost _host = new ElementHost();
    ScenarioResult _sr = new ScenarioResult();
    System.Windows.Forms.TextBox _wfTextBox1 = new TextBox();
    
    protected override void InitTest(TParams p) {
        base.InitTest(p);
        UseMita = true;
        this.Text = "TabsTest";
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        switch (scenario.Name)
        {
            case "Scenario1":
                this.Controls.Clear();
                _host.Child = _uc;
                this.Controls.Add(_host);
                _uc.tb1.AcceptsTab = false;
                _uc.tb1.IsTabStop = true;
                _uc.tb2.IsTabStop = true;
                _uc.bt2.IsTabStop = true;
                _sr = new ScenarioResult();
                break;

            case "Scenario2":
                this.Controls.Clear();
                _host.TabStop = false;
                _host.Child = _uc;
                this.Controls.Add(_host);
                this.Controls.Add(_wfTextBox1);
                _wfTextBox1.Name = "WFTextBox1";
                _wfTextBox1.Text = "WFTextBox1";
                _wfTextBox1.Top = 200;
                _uc.tb1.AcceptsTab = false;
                _uc.tb1.IsTabStop = true;
                _uc.tb2.IsTabStop = true;
                _uc.bt2.IsTabStop = true;
                _sr = new ScenarioResult();
                break;

            case "Scenario3":
                this.Controls.Clear();
                _host.TabStop = true;
                _host.Child = _uc;
                this.Controls.Add(_host);
                this.Controls.Add(_wfTextBox1);
                _wfTextBox1.Name = "WFTextBox1";
                _wfTextBox1.Text = "WFTextBox1";
                _wfTextBox1.Top = 200;
                _uc.tb1.AcceptsTab = false;
                _uc.tb1.IsTabStop = true;
                _uc.tb2.IsTabStop = true;
                _uc.bt2.IsTabStop = true;
                _sr = new ScenarioResult();
                break;

            case "Scenario4":
                this.Controls.Clear();
                _host.Child = _uc;
                this.Controls.Add(_host);
                this.Controls.Add(_wfTextBox1);
                _wfTextBox1.Name = "WFTextBox1";
                _wfTextBox1.Text = "WFTextBox1";
                _wfTextBox1.Top = 200;
                _uc.tb1.AcceptsTab = false;
                _uc.tb1.IsTabStop = true;
                _uc.tb2.IsTabStop = false;
                _uc.bt2.IsTabStop = false;
                _sr = new ScenarioResult();
                break;

            case "Scenario5":
                this.Controls.Clear();
                _host.Child = _uc;
                this.Controls.Add(_host);
                _uc.tb1.AcceptsTab = true;
                _uc.tb1.IsTabStop = true;
                _uc.tb2.IsTabStop = true;
                _uc.bt2.IsTabStop = true;
                _sr = new ScenarioResult();
                break;

            case "Scenario6":
                this.Controls.Clear();
                _host.Child = _uc;
                this.Controls.Add(_host);
                this.Controls.Add(_wfTextBox1);
                _wfTextBox1.Name = "WFTextBox1";
                _wfTextBox1.Text = "WFTextBox1";
                _wfTextBox1.Top = 200;
                _uc.tb1.AcceptsTab = true;
                _uc.tb1.IsTabStop = true;
                _uc.tb2.IsTabStop = true;
                _uc.bt2.IsTabStop = true;
                _sr = new ScenarioResult();
                break;

            case "Scenario7":
                this.Controls.Clear();
                _host.Child = _uc;
                this.Controls.Add(_host);
                this.Controls.Add(_wfTextBox1);
                _wfTextBox1.Name = "WFTextBox1";
                _wfTextBox1.Text = "WFTextBox1";
                _wfTextBox1.Top = 200;
                _uc.tb1.AcceptsTab = false;
                _uc.tb1.IsTabStop = true;
                _uc.tb2.IsTabStop = false;
                _uc.bt2.IsTabStop = false;
                _sr = new ScenarioResult();
                break;

            case "Scenario8":
                this.Controls.Clear();
                _host.Child = _uc;
                this.Controls.Add(_host);
                _uc.tb1.AcceptsTab = true;
                _uc.tb1.IsTabStop = false;
                _uc.tb2.IsTabStop = true;
                _uc.bt2.IsTabStop = true;
                _sr = new ScenarioResult();
                break;

            case "Scenario9":
                this.Controls.Clear();
                _host.Child = _uc;
                this.Controls.Add(_host);
                this.Controls.Add(_wfTextBox1);
                _wfTextBox1.Name = "WFTextBox1";
                _wfTextBox1.Text = "WFTextBox1";
                _wfTextBox1.Top = 200;
                _uc.tb1.AcceptsTab = false;
                _uc.tb1.IsTabStop = true;
                _uc.tb2.IsTabStop = true;
                _uc.bt2.IsTabStop = true;
                _sr = new ScenarioResult();
                break;

        }
        return base.BeforeScenario(p, scenario);
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("FW/RV tab between multiple Avalon controls in a EH  (should wrap within EH and maintain tab order per SWC.Control.TabIndex)  as long as there are no other WinForm controls besides the EH.")]
    public ScenarioResult Scenario1(TParams p) {
        UIObject uiApp = null;
        UIObject uiAvTextBox1 = null;
        UIObject uiAvButton2 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TabsTest"));
            uiAvTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvTextBox1);
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvButton2 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvButton2);
            //get focus to the user control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvButton2);
            //set focus to last control
            edAvButton2.SetFocus();
            //tab to the 'next' control - should wrap to the first control, which is AVTextBox1
            edAvButton2.SendKeys("{TAB}");
            _sr.IncCounters(edAvTextBox1.HasKeyboardFocus, p.log, BugDb.WindowsOSBugs, 1547068, "AVTextBox1 did not receive keyboard focus");
            //set focus on first control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvTextBox1);
            //shift-tab to the 'next' control - should wrap to the last control, which is AVButton2
            edAvTextBox1.SendKeys("+{TAB}");
            _sr.IncCounters(uiAvButton2.HasKeyboardFocus, p.log, BugDb.WindowsOSBugs, 1547068, "AVButton2 did not receive keyboard focus");
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("FW/RV tab between Winform controls and EH with EH.TabStop = false.  ")]
    public ScenarioResult Scenario2(TParams p) {
        UIObject uiApp = null;
        UIObject uiAvTextBox1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiWfTextBox1 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TabsTest"));
            uiAvTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvTextBox1);
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvButton2 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvButton2);
            uiWfTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("WFTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edWfTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiWfTextBox1);
            //get focus to the user control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvButton2);
            //set focus on last control
            edAvButton2.SetFocus();
            //tab to the 'next' control - should switch to the WF control, which is WFTextBox1
            edAvButton2.SendKeys("{TAB}");
            _sr.IncCounters(edWfTextBox1.HasKeyboardFocus, "(host.tabStop=false)WFTextBox1 did not receive keyboard focus", p.log);
            //set focus on first av control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvTextBox1);
            //shift-tab to the 'next' control - should switch to the WF control, which is WFTextBox1
            edAvTextBox1.SendKeys("+{TAB}");
            Utilities.SleepDoEvents(10);
            _sr.IncCounters(edWfTextBox1.HasKeyboardFocus, "(host.tabStop=false)WFTextBox1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("FW/RV tab between Winform controls and EH with EH.TabStop = true. ")]
    public ScenarioResult Scenario3(TParams p) {
        UIObject uiApp = null;
        UIObject uiAvTextBox1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiWfTextBox1 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TabsTest"));
            uiAvTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvTextBox1);
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvButton2 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvButton2);
            uiWfTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("WFTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edWfTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiWfTextBox1);
            //get focus to the user control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvButton2);
            //set focus on last av control
            edAvButton2.SetFocus();
            //tab to the 'next' control - should switch to the WF control, which is WFTextBox1
            edAvButton2.SendKeys("{TAB}");
            Utilities.SleepDoEvents(10);
            _sr.IncCounters(edWfTextBox1.HasKeyboardFocus, "(host.tabStop=false)WFTextBox1 did not receive keyboard focus", p.log);
            //set focus on first av control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvTextBox1);
            //shift-tab to the 'next' control - should switch to the WF control, which is WFTextBox1
            edAvTextBox1.SendKeys("+{TAB}");
            Utilities.SleepDoEvents(10);
            _sr.IncCounters(edWfTextBox1.HasKeyboardFocus, "(host.tabStop=false)WFTextBox1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("FW/RV tab between Winform controls and EH with multiple Avalon elements. Some elements have TabStop = false. Should not focus on elements with TabStop=false.")]
    public ScenarioResult Scenario4(TParams p) {
        UIObject uiApp = null;
        UIObject uiAvTextBox1 = null;
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiWfTextBox1 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TabsTest"));
            uiAvTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvTextBox1);
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvButton1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvButton1);
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvButton2 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvButton2);
            uiWfTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("WFTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edWfTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiWfTextBox1);
            //get focus to the user control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvButton2);
            //set focus on first av control
            edAvTextBox1.SetFocus();
            //tab to the 'next' control - should skip AVTextBox2 and go to AVButton1
            edAvTextBox1.SendKeys("{TAB}");
            _sr.IncCounters(edAvButton1.HasKeyboardFocus, "AVButton1 did not receive keyboard focus", p.log);
            //set focus to third av control
            edAvButton1.SetFocus();
            //tab to the 'next' control - should skip AVButton2 and go to WFTextBox1
            edAvButton1.SendKeys("{TAB}");
            _sr.IncCounters(edWfTextBox1.HasKeyboardFocus, "WFTextBox1 did not receive keyboard focus", p.log);
            //set focus to third av control
            edAvButton1.SetFocus();
            //shift-tab to the 'next' control - should skip AVTextBox2 and go to AVTextBox1
            edAvButton1.SendKeys("+{TAB}");
            _sr.IncCounters(edAvTextBox1.HasKeyboardFocus, "AVTextBox1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Controls that eat tabs (textbox that has acceptstab= true) verify that you can get out with CTRL+TAB between Avalon control in a single EH")]
    public ScenarioResult Scenario5(TParams p) {
        UIObject uiApp = null;
        UIObject uiAvTextBox1 = null;
        UIObject uiAvTextBox2 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TabsTest"));
            uiAvTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvTextBox1);
            uiAvTextBox2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox2"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvTextBox2 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvTextBox2);
            //get focus to the user control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvTextBox1);
            //set focus on first av control
            edAvTextBox1.SetFocus();
            //hit 'tab' - since AcceptsTabs=true, focus should stay here
            edAvTextBox1.SendKeys("{TAB}");
            _sr.IncCounters(edAvTextBox1.HasKeyboardFocus, "AVTextBox1 did not receive keyboard focus", p.log);
            //now hit 'ctrl-tab' - focus should move to AVTextBox2
            edAvTextBox1.SendKeys("^{TAB}");
            _sr.IncCounters(edAvTextBox2.HasKeyboardFocus, "AVTextBox2 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Controls that eat tabs (textbox that has acceptstab= true) verify that you can get out with CTRL+TAB between AvControl to WinForm control.")]
    public ScenarioResult Scenario6(TParams p) {
        UIObject uiApp = null;
        UIObject uiAvTextBox1 = null;
        UIObject uiWfTextBox1 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TabsTest"));
            uiAvTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvTextBox1);
            uiWfTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("WFTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edWfTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiWfTextBox1);
            //get focus to the user control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvTextBox1);
            //set focus on first av control
            edAvTextBox1.SetFocus();
            //hit 'tab' - since AcceptsTabs=true, focus should stay here
            edAvTextBox1.SendKeys("{TAB}");
            _sr.IncCounters(edAvTextBox1.HasKeyboardFocus, "AVTextBox1 did not receive keyboard focus", p.log);
            //now hit 'ctrl-shift-tab' - focus should move to WFTextBox1
            edAvTextBox1.SendKeys("^+{TAB}");
            _sr.IncCounters(edWfTextBox1.HasKeyboardFocus, "WFTextBox1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Verify that Avalon controls that have TabStop=false do not get tabs.")]
    public ScenarioResult Scenario7(TParams p) {
        UIObject uiApp = null;
        UIObject uiAvTextBox1 = null;
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiWfTextBox1 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TabsTest"));
            uiAvTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvTextBox1);
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvButton1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvButton1);
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            MS.Internal.Mita.Foundation.Controls.Edit edAvButton2 = new MS.Internal.Mita.Foundation.Controls.Edit(uiAvButton2);
            uiWfTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("WFTextBox1"));
            MS.Internal.Mita.Foundation.Controls.Edit edWfTextBox1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiWfTextBox1);
            //get focus to the user control
            Mouse.Instance.Click(MS.Internal.Mita.Foundation.MouseButtons.Primary, uiAvTextBox1);
            //set focus on first av control
            edAvTextBox1.SetFocus();
            //tab to the 'next' control - should skip AVTextBox2 and go to AVButton1
            edAvTextBox1.SendKeys("{TAB}");
            _sr.IncCounters(edAvButton1.HasKeyboardFocus, "AVButton1 did not receive keyboard focus", p.log);
            //set focus to third av control
            edAvButton1.SetFocus();
            //tab to the 'next' control - should skip AVButton2 and go to WFTextBox1
            edAvButton1.SendKeys("{TAB}");
            _sr.IncCounters(edWfTextBox1.HasKeyboardFocus, "WFTextBox1 did not receive keyboard focus", p.log);
            //set focus to third av control
            edAvButton1.SetFocus();
            //shift-tab to the 'next' control - should skip AVTextBox2 and go to AVTextBox1
            edAvButton1.SendKeys("+{TAB}");
            _sr.IncCounters(edAvTextBox1.HasKeyboardFocus, "AVTextBox1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("TextBox with TabStop=false AcceptTabs=true and click with mouse on the TextBox.")]
    public ScenarioResult Scenario8(TParams p) {
        UIObject uiApp = null;
        UIObject uiAvTextBox1 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TabsTest"));
            uiAvTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox1"));
            //now click control of interest
            uiAvTextBox1.Click();
            Utilities.SleepDoEvents(2);
            //did we get focus?
            _sr.IncCounters(uiAvTextBox1.HasKeyboardFocus, "AVTextBox1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Cyclic testing with mouse click.")]
    public ScenarioResult Scenario9(TParams p) {
        UIObject uiApp = null;
        UIObject uiAvTextBox1 = null;
        UIObject uiAvTextBox2 = null;
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiWfTextBox1 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TabsTest"));
            uiAvTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox1"));
            uiAvTextBox2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox2"));
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            uiWfTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("WFTextBox1"));
            //get focus to the user control and click AVTextBox1, then see if we got focus
            uiAvTextBox1.Click();
            Utilities.SleepDoEvents(2, 100);
            _sr.IncCounters(uiAvTextBox1.HasKeyboardFocus, "AVTextBox1 did not receive keyboard focus", p.log);
            //click AVTextBox2, then see if we get focus
            uiAvTextBox2.Click();
            Utilities.SleepDoEvents(2, 100);
            _sr.IncCounters(uiAvTextBox2.HasKeyboardFocus, "AVTextBox2 did not receive keyboard focus", p.log);
            //click AVButton1, then see if we get focus
            uiAvButton1.Click();
            Utilities.SleepDoEvents(2, 100);
            _sr.IncCounters(uiAvButton1.HasKeyboardFocus, "AVButton1 did not receive keyboard focus", p.log);
            //click AVButton2, then see if we get focus
            uiAvButton2.Click();
            Utilities.SleepDoEvents(2, 100);
            _sr.IncCounters(uiAvButton2.HasKeyboardFocus, "AVButton2 did not receive keyboard focus", p.log);
            //click WFTextBox1, then see if we get focus
            uiWfTextBox1.Click();
            Utilities.SleepDoEvents(2, 100);
            _sr.IncCounters(uiWfTextBox1.HasKeyboardFocus, "WFTextBox1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }
    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ FW/RV tab between multiple Avalon controls in a EH  (should wrap within EH and maintain tab order per SWC.Control.TabIndex)  as long as there are no other WinForm controls besides the EH.

//@ FW/RV tab between Winform controls and EH with EH.TabStop = false.  

//@ FW/RV tab between Winform controls and EH with EH.TabStop = true. 

//@ FW/RV tab between Winform controls and EH with multiple Avalon elements. Some elements have TabStop = false. Should not focus on elements with TabStop=false.

//@ Controls that eat tabs (textbox that has acceptstab= true) verify that you can get out with CTRL+TAB between Avalon control in a single EH

//@ Controls that eat tabs (textbox that has acceptstab= true) verify that you can get out with CTRL+TAB between AvControl to WinForm control.

//@ Verify that Avalon controls that have TabStop=false do not get tabs.

//@ TextBox with TabStop=false AcceptTabs=true and click with mouse on the TextBox.

//@ Cyclic testing with mouse click.