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
using MS.Internal.Mita.Foundation.Controls;
using System.Reflection;
using System.Windows.Automation.Text;

// Testcase:    ArrowkeysAndPgUpPgDwn
// Description: Verify that AV controls expecting Arrow Key and Page key input work as expected
public class ArrowkeysAndPgUpPgDwn : ReflectBase {

    #region Testcase setup
    public ArrowkeysAndPgUpPgDwn(string[] args) : base(args) { }

    ElementHost _host = new ElementHost();
    ElementHost _eh1 = new ElementHost();
    ElementHost _eh2 = new ElementHost();
    MyUserControl.UserControl1 _uc = new MyUserControl.UserControl1();
    MyUserControl.UserControl2 _uc2 = new MyUserControl.UserControl2();
    MyUserControl.UserControl3 _uc3 = new MyUserControl.UserControl3();
    ScenarioResult _sr;
    UIObject _uiApp;
    System.Windows.Forms.Button _wfButton1 = new System.Windows.Forms.Button();
    System.Windows.Controls.Button _avButton1 = new System.Windows.Controls.Button();
    System.Windows.Controls.Button _avButton2 = new System.Windows.Controls.Button();
    System.Windows.Controls.TextBox _avTextBox1 = new System.Windows.Controls.TextBox();
    System.Windows.Controls.ScrollViewer _avScrollViewer = new System.Windows.Controls.ScrollViewer();
    System.Windows.Controls.StackPanel _avStackPanel = new System.Windows.Controls.StackPanel();
    System.Windows.Controls.RadioButton _avRadioButton1 = new System.Windows.Controls.RadioButton();
    System.Windows.Controls.RadioButton _avRadioButton2 = new System.Windows.Controls.RadioButton();
    System.Windows.Controls.RadioButton _avRadioButton3 = new System.Windows.Controls.RadioButton();

    protected override void InitTest(TParams p) {
        base.InitTest(p);
        UseMita = true;
        this.Text = "ArrowkeysAndPgUpPgDwnTest";
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("ArrowkeysAndPgUpPgDwnTest"));
        this.Controls.Clear();
        _host.Child = null;
        switch (scenario.Name)
        {
            case "Scenario1":
                _host.Child = _uc;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario2":
                _host.Child = _uc2;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario3":
                _host.Child = _uc3;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario4":
                _wfButton1.Top = 100;
                _wfButton1.Name = "wfButton1";
                _wfButton1.Text = "wfButton1";
                _host.Child = _uc3;
                this.Controls.Add(_host);
                this.Controls.Add(_wfButton1);
                _sr = new ScenarioResult();
                break;
            case "Scenario5":
                _wfButton1.Top = 100;
                _wfButton1.Name = "wfButton1";
                _wfButton1.Text = "wfButton1";
                _host.Child = _uc3;
                this.Controls.Add(_host);
                this.Controls.Add(_wfButton1);
                _sr = new ScenarioResult();
                break;
            case "Scenario6":
                _wfButton1.Top = 100;
                _wfButton1.Name = "wfButton1";
                _wfButton1.Text = "wfButton1";
                _host.Child = _uc3;
                this.Controls.Add(_host);
                this.Controls.Add(_wfButton1);
                _sr = new ScenarioResult();
                break;
            case "Scenario7":
                _eh1.Top = 10;
                _eh1.AutoSize = true;
                _eh2.Top = 50;
                _eh2.AutoSize = true;
                _avButton1.Name = "avButton1";
                _avButton1.Content = "avButton1";
                _avButton2.Name = "avButton2";
                _avButton2.Content = "avButton2";
                _eh1.Child = _avButton1;
                _eh2.Child = _avButton2;
                this.Controls.Add(_eh1);
                this.Controls.Add(_eh2);
                _sr = new ScenarioResult();
                break;
            case "Scenario8":
                _host.Width = 250;
                _host.Height = 50;
                _host.BackColor = System.Drawing.Color.Red;

                _avTextBox1.Name = "avTextBox1";
                _avTextBox1.Text = "This is line 1\nThis is line 2\nThis is line 3\nThis is line 4\nThis is line 5\nThis is line 6";
                _avTextBox1.TextWrapping = System.Windows.TextWrapping.Wrap;
                _avTextBox1.Width = 200;
                _avTextBox1.Height = 75;
                _avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                _avTextBox1.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;

                _host.Child = _avTextBox1;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario9":
                _host.Width = 250;
                _host.Height = 75;
                _host.BackColor = System.Drawing.Color.Blue;

                _avTextBox1.Name = "avTextBox1";
                _avTextBox1.Text = "This is a line of text";
                _avTextBox1.Width = 200;
                _avTextBox1.Height = 75;
                _avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                _host.Child = _avTextBox1;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario10":
                _host.Width = 250;
                _host.Height = 75;
                _host.BackColor = System.Drawing.Color.Blue;

                _avTextBox1.Name = "avTextBox1";
                _avTextBox1.Text = "This is a line of text";
                _avTextBox1.Width = 200;
                _avTextBox1.Height = 75;
                _avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                _host.Child = _avTextBox1;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario11":
                _host.Width = 250;
                _host.Height = 75;
                _host.BackColor = System.Drawing.Color.Blue;

                _avTextBox1.Name = "avTextBox1";
                _avTextBox1.Text = "This is a line of text";
                _avTextBox1.Width = 200;
                _avTextBox1.Height = 75;
                _avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                _host.Child = _avTextBox1;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario12":
                _host.Width = 250;
                _host.Height = 50;
                _host.BackColor = System.Drawing.Color.Red;

                _avTextBox1.Name = "avTextBox1";
                _avTextBox1.Text = "This is line 1\nThis is line 2\nThis is line 3\nThis is line 4\nThis is line 5\nThis is line 6";
                _avTextBox1.TextWrapping = System.Windows.TextWrapping.Wrap;
                _avTextBox1.Width = 200;
                _avTextBox1.Height = 50;
                _avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                _avTextBox1.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                _avScrollViewer.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                _avScrollViewer.Content = _avTextBox1;
                _host.Child = _avScrollViewer;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario13":
                _host.Width = 250;
                _host.Height = 50;
                _host.BackColor = System.Drawing.Color.Cyan;

                _avRadioButton1.Content = "Radio 1";
                _avRadioButton2.Content = "Radio 2";
                _avRadioButton3.Content = "Radio 3";

                _avRadioButton1.Name = "avRadioButton1";
                _avRadioButton2.Name = "avRadioButton2";
                _avRadioButton3.Name = "avRadioButton3";

                _avStackPanel.Children.Add(_avRadioButton1);
                _avStackPanel.Children.Add(_avRadioButton2);
                _avStackPanel.Children.Add(_avRadioButton3);
                _host.Child = _avStackPanel;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario14":
                _host.Width = 250;
                _host.Height = 50;
                _host.BackColor = System.Drawing.Color.Red;

                _avTextBox1.Name = "avTextBox1";
                _avTextBox1.Clear();
                for (int i = 1; i <= 40; i++)
                {
                    _avTextBox1.Text += "This is line " + i.ToString() + "\n";
                }
                _avTextBox1.TextWrapping = System.Windows.TextWrapping.Wrap;
                _avTextBox1.Width = 200;
                _avTextBox1.Height = 100;
                _avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                _avTextBox1.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                _avScrollViewer.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                _avScrollViewer.Content = _avTextBox1;
                _host.Child = _avScrollViewer;
                this.Controls.Add(_host);
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
    [Scenario("Move between Avalon Control's in the same EH via Arrow Keys. Avalon controls are hosted in a Stack layout.")]
    public ScenarioResult Scenario1(TParams p) {
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiAvButton3 = null;
        try
        {
            uiAvButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvButton2 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            uiAvButton3 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            //get focus to the user control
            uiAvButton1.Click();
            Utilities.SleepDoEvents(2);
            //send two down-arrows and see if we are on button 3
            _uiApp.SendKeys("{DOWN 2}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send two up-arrows and see if we are on button 1
            _uiApp.SendKeys("{UP 2}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
            //now send one left-arrow and see if we are on button 3
            _uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send one down-arrow and see if we are on button 1
            _uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return _sr;
    }

    [Scenario("Move between Avalon Control's in the same EH via Arrow Keys. Avalon controls are hosted in a Grid layout. We want to test directional navigation here.")]
    public ScenarioResult Scenario2(TParams p) {
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiAvButton3 = null;
        try
        {
            uiAvButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvButton2 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            uiAvButton3 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            //get focus to the user control
            uiAvButton1.Click();
            //send two down-arrows and see if we are on button 3
            _uiApp.SendKeys("{DOWN 2}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send two up-arrows and see if we are on button 1
            _uiApp.SendKeys("{UP 2}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
            //now send one left-arrow and see if we are on button 3
            _uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send one down-arrow and see if we are on button 1
            _uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Move between Avalon Control's in the same EH via Arrow Keys. Avalon controls are hosted on a Canvas. We want to test directional navigation here.")]
    public ScenarioResult Scenario3(TParams p) {
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiAvButton3 = null;
        try
        {
            uiAvButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvButton2 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            uiAvButton3 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            //get focus to the user control
            uiAvButton1.Click();
            //send two down-arrows and see if we are on button 3
            _uiApp.SendKeys("{DOWN 2}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send two up-arrows and see if we are on button 1
            _uiApp.SendKeys("{UP 2}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
            //now send one left-arrow and see if we are on button 3
            _uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send one down-arrow and see if we are on button 1
            _uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return _sr;
    }

    [Scenario("Navigate into an Avalon control from a WF control using Right/Down arrow. Expected: selects first hosted element.")]
    public ScenarioResult Scenario4(TParams p)
    {
        UIObject uiAvButton1 = null;
        UIObject uiWfButton1 = null;
        try
        {
            uiAvButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiWfButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            //get focus to the wf button
            uiWfButton1.SetFocus();
            //send right-arrow and see if we are on usercontrol button 1
            _uiApp.SendKeys("{RIGHT 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus (right-arrow)", p.log);
            //get focus back to the wf button
            uiWfButton1.SetFocus();
            //send down-arrow and see if we are on usercontrol button 1
            _uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus (down-arrow)", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Navigate into a Avalon control from a WF control. Left/Up Arrow Expected: Selects last hosted element.")]
    public ScenarioResult Scenario5(TParams p)
    {
        UIObject uiAvButton3 = null;
        UIObject uiWfButton1 = null;
        try
        {
            uiAvButton3 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            uiWfButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            //get focus to the wf button
            uiWfButton1.SetFocus();
            //send left-arrow and see if we are on usercontrol button 3
            _uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus (left-arrow)", p.log);
            //get focus back to the wf button
            uiWfButton1.SetFocus();
            //send up-arrow and see if we are on usercontrol button 3
            _uiApp.SendKeys("{UP 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus (up-arrow)", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Navigate out of an Avalon control to a WF control.")]
    public ScenarioResult Scenario6(TParams p)
    {
        UIObject uiAvButton1 = null;
        UIObject uiAvButton3 = null;
        UIObject uiWfButton1 = null;
        try
        {
            uiAvButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvButton3 = _uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            uiWfButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            //get focus to the last av button
            uiAvButton3.SetFocus();
            //send down-arrow and see if we are on the wf button
            _uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiWfButton1.HasKeyboardFocus, "uiWfButton1 did not receive keyboard focus (down-arrow)", p.log);
            //get to the first av button
            uiAvButton1.SetFocus();
            //send up-arrow and see if we are on the wf button
            _uiApp.SendKeys("{UP 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiWfButton1.HasKeyboardFocus, "uiWfButton1 did not receive keyboard focus (up-arrow)", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Navigate out Avalon control (hosted on EH1) into another Avalon control(hosted on EH2)")]
    public ScenarioResult Scenario7(TParams p)
    {
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        try
        {
            uiAvButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("avButton1"));
            uiAvButton2 = _uiApp.Descendants.Find(UICondition.CreateFromId("avButton2"));
            //get focus to eh1
            uiAvButton1.SetFocus();
            //send down-arrow and see if we are on eh2
            _uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton2.HasKeyboardFocus, "uiAvButton2 did not receive keyboard focus (down-arrow)", p.log);
            //get to the first av button
            uiAvButton1.SetFocus();
            //send left-arrow and see if we are on eh2
            _uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(uiAvButton2.HasKeyboardFocus, "uiAvButton2 did not receive keyboard focus (left-arrow)", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Arrow keys work for a horizontal/vertical scroll bar in a EH.")]
    public ScenarioResult Scenario8(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            //send page down and see if we are on line 4
            uiavTextBox1.SendKeys("{PGDN}{HOME}+{END}");
            Utilities.SleepDoEvents(2);
            //see if we got it
            _sr.IncCounters(_avTextBox1.SelectedText == "This is line 4\n", "Text did not scroll", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Verify that Shift+Arrow's work")]
    public ScenarioResult Scenario9(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            //try to select first word ('This') in textbox
            uiavTextBox1.SendKeys("{HOME}+{RIGHT 4}");
            Utilities.SleepDoEvents(2);
            //see if we got it
            _sr.IncCounters(_avTextBox1.SelectedText == "This", "Shift-Arrow did not work", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Verify that CTRL+Arrow's work")]
    public ScenarioResult Scenario10(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            Utilities.SleepDoEvents(5);
            //try to select second word ('is') in textbox - we first do a ctrl-right arrow to get to it
            uiavTextBox1.SendKeys("{HOME}^{RIGHT}+{RIGHT 2}");
            Utilities.SleepDoEvents(25);
            //see if we got it
            _sr.IncCounters(_avTextBox1.SelectedText == "is", "Ctrl-Arrow did not work", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Verify that Shift+CTRL+Arrows's work")]
    public ScenarioResult Scenario11(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            Utilities.SleepDoEvents(5);
            //try to select the first two words ('This is') in textbox
            //see previous two scenarios for details
            uiavTextBox1.SendKeys("{HOME}^+{RIGHT 2}");
            Utilities.SleepDoEvents(25);
            //see if we got it
            _sr.IncCounters(_avTextBox1.SelectedText == "This is ", "Ctrl-Arrow did not work", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Using a Avalon scrollbar on EH verify that Pg UP and Pg DWN keys work")]
    public ScenarioResult Scenario12(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            //send page down and see if we are on line 4
            uiavTextBox1.SendKeys("{PGDN}{HOME}+{END}");
            Utilities.SleepDoEvents(2);
            //see if we got it
            _sr.IncCounters(_avTextBox1.SelectedText == "This is line 4\n", "Text did not scroll", p.log);
            p.log.WriteLine("In scenario 12, selected text is: |" + _avTextBox1.SelectedText + "|");
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("???? Verify that Arrow keys cycle through a set of Radio buttons in a group box.  Need to check spec.")]
    public ScenarioResult Scenario13(TParams p)
    {
        UIObject uiavRadioButton1 = null;
        UIObject uiavRadioButton2 = null;
        UIObject uiavRadioButton3 = null;
        try
        {
            uiavRadioButton1 = _uiApp.Descendants.Find(UICondition.CreateFromId("avRadioButton1"));
            uiavRadioButton2 = _uiApp.Descendants.Find(UICondition.CreateFromId("avRadioButton2"));
            MS.Internal.Mita.Foundation.Controls.RadioButton rb2 = new MS.Internal.Mita.Foundation.Controls.RadioButton(uiavRadioButton2);
            uiavRadioButton3 = _uiApp.Descendants.Find(UICondition.CreateFromId("avRadioButton3"));
            MS.Internal.Mita.Foundation.Controls.RadioButton rb3 = new MS.Internal.Mita.Foundation.Controls.RadioButton(uiavRadioButton3);
            //set focus to first radio button
            uiavRadioButton1.SetFocus();
            //send down arrow, press space bar - see if uiavRadioButton2 was set
            _uiApp.SendKeys("{DOWN 1} ");
            Utilities.SleepDoEvents(2);
            //see if we got it
            _sr.IncCounters(rb2.IsSelected, "avRadioButton2 did not get 'Checked'", p.log);
            //send down arrow, press space bar - see if uiavRadioButton3 was set
            _uiApp.SendKeys("{DOWN 1} ");
            Utilities.SleepDoEvents(2);
            //see if we got it
            _sr.IncCounters(rb3.IsSelected, "avRadioButton3 did not get 'Checked'", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("????When a EH's parent is veritically scrolled, verify that a vertical scroll bar on a EH textbox works as expected.")]
    public ScenarioResult Scenario14(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            //send page down and see if we are on line 8
            uiavTextBox1.SendKeys("{PGDN}{HOME}+{END}");
            Utilities.SleepDoEvents(2);
            //see if we got it
            _sr.IncCounters(_avTextBox1.SelectedText == "This is line 8\n", "Text did not scroll", p.log);
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
//@ Move between Avalon Control's in the same EH via Arrow Keys. Avalon controls are hosted in a Stack layout.

//@ Move between Avalon Control's in the same EH via Arrow Keys. Avalon controls are hosted in a Grid layout. We want to test directional navigation here.

//@ Move between Avalon Control's in the same EH via Arrow Keys. Avalon controls are hosted on a Canvas. We want to test directional navigation here.

//@ Navigate into an Avalon control from a WF control using Right/Down arrow. Expected: selects first hosted element.

//@ Navigate into a Avalon control from a WF control. Left/Up Arrow Expected: Selects last hosted element.

//@ Navigate out of an Avalon control to a WF control.

//@ Navigate out Avalon control (hosted on EH1) into another Avalon control(hosted on EH2)

//@ Arrow keys work for a horizontal/vertical scroll bar in a EH.

//@ Verify that Shift+Arrow's work

//@ Verify that CTRL+Arrow's work

//@ Verify that Shift+CTRL+Arrows's work

//@ Using a Avalon scrollbar on EH verify that Pg UP and Pg DWN keys work

//@ ???? Verify that Arrow keys cycle through a set of Radio buttons in a group box.  Need to check spec.

//@ ????When a EH's parent is veritically scrolled, verify that a vertical scroll bar on a EH textbox works as expected.