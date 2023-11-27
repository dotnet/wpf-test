// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Controls;
using System.Threading;

//
// Testcase:    AltTab
// Description: Verify that Alt+Tab works and that Focus is good
//
public class AltTab : ReflectBase
{
    #region Testcase setup
    public AltTab(string[] args) : base(args) { }

    ElementHost _host = new ElementHost();
    System.Windows.Controls.TextBox _avTextbox = new System.Windows.Controls.TextBox();
    System.Windows.Forms.TextBox _wfTextbox = new System.Windows.Forms.TextBox();
    ScenarioResult _sr = new ScenarioResult();
    UIObject _uiApp = null;
    UIObject _uiWinForm = null;
    System.Threading.Thread _newThread;

    protected override void InitTest(TParams p) {
        base.InitTest(p);
        UseMita = true;
        this.Text = "AltTabTest";
        _avTextbox.Text = "AV TextBox";
        _avTextbox.Name = "AVTextBox";
        _wfTextbox.Top = 25;
        _wfTextbox.Width = 100;
        _wfTextbox.Name = "WFTextBox";
        _wfTextbox.Text = "WF TextBox";
        _host.AutoSize = true;
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("AltTabTest"));
        _sr = new ScenarioResult();
        switch (scenario.Name)
        {
            case "Scenario1":
                _host.BackColor = System.Drawing.Color.Red;
                _avTextbox.Background = System.Windows.Media.Brushes.AliceBlue;
                _host.Child = _avTextbox;
                this.Controls.Add(_host);
                this.Controls.Add(_wfTextbox);
                break;
            case "Scenario2":
                //need to show modal form on a separate thread.
                //NOTE:  DON'T RUN THIS IN DEBUG MODE.
                //       Doing so causes an MDA exception when the app ends.  Sameer said this 
                //       could safely be ignored (4/3/2006).
                _newThread = new System.Threading.Thread(new ThreadStart(ShowModalForm.ShowIt));
                _newThread.SetApartmentState(ApartmentState.STA);
                _newThread.Start();
                break;
        }
        return base.BeforeScenario(p, scenario);
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Verify focus preserved after Alt+TAB.")]
    public ScenarioResult Scenario1(TParams p) {
        UIObject uiAvTextbox;
        UIObject uiWfTextbox;
        try
        {
            uiAvTextbox = _uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox"));
            uiWfTextbox = _uiApp.Descendants.Find(UICondition.CreateFromId("WFTextBox"));

            //click in wf textbox - see that we received keyboard focus
            uiWfTextbox.Click();
            Utilities.SleepDoEvents(10);
            //do we have focus?
            _sr.IncCounters(uiWfTextbox.HasKeyboardFocus, "WFTextBox did not receive keyboard focus", p.log);
            //execute Alt-Tab
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(10);
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(20);
            //do we have focus?
            _sr.IncCounters(uiWfTextbox.HasKeyboardFocus, "WFTextBox did not retain keyboard focus", p.log);

            //click in av textbox - see that we received keyboard focus
            uiAvTextbox.Click();
            Utilities.SleepDoEvents(10);
            //do we have focus?
            _sr.IncCounters(uiAvTextbox.HasKeyboardFocus, "AVTextBox did not receive keyboard focus", p.log);
            //execute Alt-Tab
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(10);
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(10);
            //do we have focus?
            _sr.IncCounters(uiAvTextbox.HasKeyboardFocus, "AVTextBox did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Verify Focus preserved after Alt+TAB from a WF Modal Dialog.")]
    public ScenarioResult Scenario2(TParams p) {
        UIObject uiAvTextbox;
        UIObject uiWfTextbox;
        try
        {
            Utilities.SleepDoEvents(20);//allow time for modal form to start up
            _uiWinForm = UIObject.Root.Children.Find(UICondition.CreateFromName("ModalWinForm"));
            uiAvTextbox = _uiWinForm.Descendants.Find(UICondition.CreateFromId("AVTextBox"));
            uiWfTextbox = _uiWinForm.Descendants.Find(UICondition.CreateFromId("WFTextBox"));

            //click in wf textbox - see that we received keyboard focus
            uiWfTextbox.Click();
            Utilities.SleepDoEvents(10);
            //do we have focus?
            _sr.IncCounters(uiWfTextbox.HasKeyboardFocus, "WFTextBox did not receive keyboard focus", p.log);
            //execute Alt-Tab
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(10);
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(10);
            //do we have focus?
            _sr.IncCounters(uiWfTextbox.HasKeyboardFocus, "WFTextBox did not retain keyboard focus", p.log);

            //click in av textbox - see that we received keyboard focus
            uiAvTextbox.Click();
            Utilities.SleepDoEvents(10);
            //do we have focus?
            _sr.IncCounters(uiAvTextbox.HasKeyboardFocus, "AVTextBox did not receive keyboard focus", p.log);
            //execute Alt-Tab
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(10);
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(10);
            //do we have focus?
            _sr.IncCounters(uiAvTextbox.HasKeyboardFocus, "AVTextBox did not retain keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        ShowModalForm.WinForm.DialogResult = DialogResult.OK;
        return _sr;
    }

    #endregion

}

class ShowModalForm
{
    public static Form WinForm = new Form();
    ShowModalForm() { }
    public static void ShowIt()
    {
        ElementHost host = new ElementHost();
        System.Windows.Controls.TextBox AvTextbox = new System.Windows.Controls.TextBox();
        System.Windows.Forms.TextBox WfTextbox = new System.Windows.Forms.TextBox();
        AvTextbox.Text = "AV TextBox";
        AvTextbox.Name = "AVTextBox";
        WfTextbox.Top = 25;
        WfTextbox.Width = 100;
        WfTextbox.Name = "WFTextBox";
        WfTextbox.Text = "WF TextBox";
        host.AutoSize = true;
        host.BackColor = System.Drawing.Color.Red;
        AvTextbox.Background = System.Windows.Media.Brushes.AliceBlue;
        host.Child = AvTextbox;
        WinForm.Text = "ModalWinForm";
        WinForm.Controls.Add(host);
        WinForm.Controls.Add(WfTextbox);        
        WinForm.ShowDialog();
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify focus preserved after Alt+TAB.

//@ Verify Focus preserved after Alt+TAB from a WF Modal Dialog.