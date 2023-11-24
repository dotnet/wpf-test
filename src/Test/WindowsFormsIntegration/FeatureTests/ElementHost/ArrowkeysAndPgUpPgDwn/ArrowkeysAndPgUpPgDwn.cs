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

//
// Testcase:    ArrowkeysAndPgUpPgDwn
// Description: Verify that AV controls expecting Arrow Key and Page key input work as expected
// Author:      a-larryl
//
public class ArrowkeysAndPgUpPgDwn : ReflectBase {

    #region Testcase setup
    public ArrowkeysAndPgUpPgDwn(string[] args) : base(args) { }

    ElementHost host = new ElementHost();
    ElementHost eh1 = new ElementHost();
    ElementHost eh2 = new ElementHost();
    MyUserControl.UserControl1 uc = new MyUserControl.UserControl1();
    MyUserControl.UserControl2 uc2 = new MyUserControl.UserControl2();
    MyUserControl.UserControl3 uc3 = new MyUserControl.UserControl3();
    ScenarioResult sr;
    UIObject uiApp;
    System.Windows.Forms.Button wfButton1 = new System.Windows.Forms.Button();
    System.Windows.Controls.Button avButton1 = new System.Windows.Controls.Button();
    System.Windows.Controls.Button avButton2 = new System.Windows.Controls.Button();
    System.Windows.Controls.TextBox avTextBox1 = new System.Windows.Controls.TextBox();
    System.Windows.Controls.ScrollViewer avScrollViewer = new System.Windows.Controls.ScrollViewer();
    System.Windows.Controls.StackPanel avStackPanel = new System.Windows.Controls.StackPanel();
    System.Windows.Controls.RadioButton avRadioButton1 = new System.Windows.Controls.RadioButton();
    System.Windows.Controls.RadioButton avRadioButton2 = new System.Windows.Controls.RadioButton();
    System.Windows.Controls.RadioButton avRadioButton3 = new System.Windows.Controls.RadioButton();

    protected override void InitTest(TParams p) {
        base.InitTest(p);
        UseMita = true;
        this.Text = "ArrowkeysAndPgUpPgDwnTest";
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("ArrowkeysAndPgUpPgDwnTest"));
        this.Controls.Clear();
        host.Child = null;
        switch (scenario.Name)
        {
            case "Scenario1":
                host.Child = uc;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario2":
                host.Child = uc2;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario3":
                host.Child = uc3;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario4":
                wfButton1.Top = 100;
                wfButton1.Name = "wfButton1";
                wfButton1.Text = "wfButton1";
                host.Child = uc3;
                this.Controls.Add(host);
                this.Controls.Add(wfButton1);
                sr = new ScenarioResult();
                break;
            case "Scenario5":
                wfButton1.Top = 100;
                wfButton1.Name = "wfButton1";
                wfButton1.Text = "wfButton1";
                host.Child = uc3;
                this.Controls.Add(host);
                this.Controls.Add(wfButton1);
                sr = new ScenarioResult();
                break;
            case "Scenario6":
                wfButton1.Top = 100;
                wfButton1.Name = "wfButton1";
                wfButton1.Text = "wfButton1";
                host.Child = uc3;
                this.Controls.Add(host);
                this.Controls.Add(wfButton1);
                sr = new ScenarioResult();
                break;
            case "Scenario7":
                eh1.Top = 10;
                eh1.AutoSize = true;
                eh2.Top = 50;
                eh2.AutoSize = true;
                avButton1.Name = "avButton1";
                avButton1.Content = "avButton1";
                avButton2.Name = "avButton2";
                avButton2.Content = "avButton2";
                eh1.Child = avButton1;
                eh2.Child = avButton2;
                this.Controls.Add(eh1);
                this.Controls.Add(eh2);
                sr = new ScenarioResult();
                break;
            case "Scenario8":
                host.Width = 250;
                host.Height = 50;
                host.BackColor = System.Drawing.Color.Red;

                avTextBox1.Name = "avTextBox1";
                avTextBox1.Text = "This is line 1\nThis is line 2\nThis is line 3\nThis is line 4\nThis is line 5\nThis is line 6";
                avTextBox1.TextWrapping = System.Windows.TextWrapping.Wrap;
                avTextBox1.Width = 200;
                avTextBox1.Height = 75;
                avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                avTextBox1.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;

                host.Child = avTextBox1;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario9":
                host.Width = 250;
                host.Height = 75;
                host.BackColor = System.Drawing.Color.Blue;

                avTextBox1.Name = "avTextBox1";
                avTextBox1.Text = "This is a line of text";
                avTextBox1.Width = 200;
                avTextBox1.Height = 75;
                avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                host.Child = avTextBox1;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario10":
                host.Width = 250;
                host.Height = 75;
                host.BackColor = System.Drawing.Color.Blue;

                avTextBox1.Name = "avTextBox1";
                avTextBox1.Text = "This is a line of text";
                avTextBox1.Width = 200;
                avTextBox1.Height = 75;
                avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                host.Child = avTextBox1;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario11":
                host.Width = 250;
                host.Height = 75;
                host.BackColor = System.Drawing.Color.Blue;

                avTextBox1.Name = "avTextBox1";
                avTextBox1.Text = "This is a line of text";
                avTextBox1.Width = 200;
                avTextBox1.Height = 75;
                avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                host.Child = avTextBox1;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario12":
                host.Width = 250;
                host.Height = 50;
                host.BackColor = System.Drawing.Color.Red;

                avTextBox1.Name = "avTextBox1";
                avTextBox1.Text = "This is line 1\nThis is line 2\nThis is line 3\nThis is line 4\nThis is line 5\nThis is line 6";
                avTextBox1.TextWrapping = System.Windows.TextWrapping.Wrap;
                avTextBox1.Width = 200;
                avTextBox1.Height = 50;
                avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                avTextBox1.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                avScrollViewer.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                avScrollViewer.Content = avTextBox1;
                host.Child = avScrollViewer;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario13":
                host.Width = 250;
                host.Height = 50;
                host.BackColor = System.Drawing.Color.Cyan;

                avRadioButton1.Content = "Radio 1";
                avRadioButton2.Content = "Radio 2";
                avRadioButton3.Content = "Radio 3";

                avRadioButton1.Name = "avRadioButton1";
                avRadioButton2.Name = "avRadioButton2";
                avRadioButton3.Name = "avRadioButton3";

                avStackPanel.Children.Add(avRadioButton1);
                avStackPanel.Children.Add(avRadioButton2);
                avStackPanel.Children.Add(avRadioButton3);
                host.Child = avStackPanel;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario14":
                host.Width = 250;
                host.Height = 50;
                host.BackColor = System.Drawing.Color.Red;

                avTextBox1.Name = "avTextBox1";
                avTextBox1.Clear();
                for (int i = 1; i <= 40; i++)
                {
                    avTextBox1.Text += "This is line " + i.ToString() + "\n";
                }
                avTextBox1.TextWrapping = System.Windows.TextWrapping.Wrap;
                avTextBox1.Width = 200;
                avTextBox1.Height = 100;
                avTextBox1.Background = System.Windows.Media.Brushes.Cyan;
                avTextBox1.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                avScrollViewer.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                avScrollViewer.Content = avTextBox1;
                host.Child = avScrollViewer;
                this.Controls.Add(host);
                sr = new ScenarioResult();
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
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            uiAvButton3 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            //get focus to the user control
            uiAvButton1.Click();
            Utilities.SleepDoEvents(2);
            //send two down-arrows and see if we are on button 3
            uiApp.SendKeys("{DOWN 2}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send two up-arrows and see if we are on button 1
            uiApp.SendKeys("{UP 2}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
            //now send one left-arrow and see if we are on button 3
            uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send one down-arrow and see if we are on button 1
            uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return sr;
    }

    [Scenario("Move between Avalon Control's in the same EH via Arrow Keys. Avalon controls are hosted in a Grid layout. We want to test directional navigation here.")]
    public ScenarioResult Scenario2(TParams p) {
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiAvButton3 = null;
        try
        {
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            uiAvButton3 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            //get focus to the user control
            uiAvButton1.Click();
            //send two down-arrows and see if we are on button 3
            uiApp.SendKeys("{DOWN 2}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send two up-arrows and see if we are on button 1
            uiApp.SendKeys("{UP 2}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
            //now send one left-arrow and see if we are on button 3
            uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send one down-arrow and see if we are on button 1
            uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Move between Avalon Control's in the same EH via Arrow Keys. Avalon controls are hosted on a Canvas. We want to test directional navigation here.")]
    public ScenarioResult Scenario3(TParams p) {
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        UIObject uiAvButton3 = null;
        try
        {
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton2"));
            uiAvButton3 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            //get focus to the user control
            uiAvButton1.Click();
            //send two down-arrows and see if we are on button 3
            uiApp.SendKeys("{DOWN 2}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send two up-arrows and see if we are on button 1
            uiApp.SendKeys("{UP 2}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
            //now send one left-arrow and see if we are on button 3
            uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus", p.log);
            //now send one down-arrow and see if we are on button 1
            uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return sr;
    }

    [Scenario("Navigate into an Avalon control from a WF control using Right/Down arrow. Expected: selects first hosted element.")]
    public ScenarioResult Scenario4(TParams p)
    {
        UIObject uiAvButton1 = null;
        UIObject uiWfButton1 = null;
        try
        {
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiWfButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            //get focus to the wf button
            uiWfButton1.SetFocus();
            //send right-arrow and see if we are on usercontrol button 1
            uiApp.SendKeys("{RIGHT 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus (right-arrow)", p.log);
            //get focus back to the wf button
            uiWfButton1.SetFocus();
            //send down-arrow and see if we are on usercontrol button 1
            uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton1.HasKeyboardFocus, "uiAvButton1 did not receive keyboard focus (down-arrow)", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Navigate into a Avalon control from a WF control. Left/Up Arrow Expected: Selects last hosted element.")]
    public ScenarioResult Scenario5(TParams p)
    {
        UIObject uiAvButton3 = null;
        UIObject uiWfButton1 = null;
        try
        {
            uiAvButton3 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            uiWfButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            //get focus to the wf button
            uiWfButton1.SetFocus();
            //send left-arrow and see if we are on usercontrol button 3
            uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus (left-arrow)", p.log);
            //get focus back to the wf button
            uiWfButton1.SetFocus();
            //send up-arrow and see if we are on usercontrol button 3
            uiApp.SendKeys("{UP 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton3.HasKeyboardFocus, "uiAvButton3 did not receive keyboard focus (up-arrow)", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Navigate out of an Avalon control to a WF control.")]
    public ScenarioResult Scenario6(TParams p)
    {
        UIObject uiAvButton1 = null;
        UIObject uiAvButton3 = null;
        UIObject uiWfButton1 = null;
        try
        {
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvButton3 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton3"));
            uiWfButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            //get focus to the last av button
            uiAvButton3.SetFocus();
            //send down-arrow and see if we are on the wf button
            uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiWfButton1.HasKeyboardFocus, "uiWfButton1 did not receive keyboard focus (down-arrow)", p.log);
            //get to the first av button
            uiAvButton1.SetFocus();
            //send up-arrow and see if we are on the wf button
            uiApp.SendKeys("{UP 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiWfButton1.HasKeyboardFocus, "uiWfButton1 did not receive keyboard focus (up-arrow)", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Navigate out Avalon control (hosted on EH1) into another Avalon control(hosted on EH2)")]
    public ScenarioResult Scenario7(TParams p)
    {
        UIObject uiAvButton1 = null;
        UIObject uiAvButton2 = null;
        try
        {
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("avButton1"));
            uiAvButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("avButton2"));
            //get focus to eh1
            uiAvButton1.SetFocus();
            //send down-arrow and see if we are on eh2
            uiApp.SendKeys("{DOWN 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton2.HasKeyboardFocus, "uiAvButton2 did not receive keyboard focus (down-arrow)", p.log);
            //get to the first av button
            uiAvButton1.SetFocus();
            //send left-arrow and see if we are on eh2
            uiApp.SendKeys("{LEFT 1}");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(uiAvButton2.HasKeyboardFocus, "uiAvButton2 did not receive keyboard focus (left-arrow)", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Arrow keys work for a horizontal/vertical scroll bar in a EH.")]
    public ScenarioResult Scenario8(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            //send page down and see if we are on line 4
            uiavTextBox1.SendKeys("{PGDN}{HOME}+{END}");
            Utilities.SleepDoEvents(2);
            //see if we got it
            sr.IncCounters(avTextBox1.SelectedText == "This is line 4\n", "Text did not scroll", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Verify that Shift+Arrow's work")]
    public ScenarioResult Scenario9(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            //try to select first word ('This') in textbox
            uiavTextBox1.SendKeys("{HOME}+{RIGHT 4}");
            Utilities.SleepDoEvents(2);
            //see if we got it
            sr.IncCounters(avTextBox1.SelectedText == "This", "Shift-Arrow did not work", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Verify that CTRL+Arrow's work")]
    public ScenarioResult Scenario10(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            Utilities.SleepDoEvents(5);
            //try to select second word ('is') in textbox - we first do a ctrl-right arrow to get to it
            uiavTextBox1.SendKeys("{HOME}^{RIGHT}+{RIGHT 2}");
            Utilities.SleepDoEvents(25);
            //see if we got it
            sr.IncCounters(avTextBox1.SelectedText == "is", "Ctrl-Arrow did not work", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Verify that Shift+CTRL+Arrows's work")]
    public ScenarioResult Scenario11(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            Utilities.SleepDoEvents(5);
            //try to select the first two words ('This is') in textbox
            //see previous two scenarios for details
            uiavTextBox1.SendKeys("{HOME}^+{RIGHT 2}");
            Utilities.SleepDoEvents(25);
            //see if we got it
            sr.IncCounters(avTextBox1.SelectedText == "This is ", "Ctrl-Arrow did not work", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Using a Avalon scrollbar on EH verify that Pg UP and Pg DWN keys work")]
    public ScenarioResult Scenario12(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            //send page down and see if we are on line 4
            uiavTextBox1.SendKeys("{PGDN}{HOME}+{END}");
            Utilities.SleepDoEvents(2);
            //see if we got it
            sr.IncCounters(avTextBox1.SelectedText == "This is line 4\n", "Text did not scroll", p.log);
            p.log.WriteLine("In scenario 12, selected text is: |" + avTextBox1.SelectedText + "|");
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("???? Verify that Arrow keys cycle through a set of Radio buttons in a group box.  Need to check spec.")]
    public ScenarioResult Scenario13(TParams p)
    {
        UIObject uiavRadioButton1 = null;
        UIObject uiavRadioButton2 = null;
        UIObject uiavRadioButton3 = null;
        try
        {
            uiavRadioButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("avRadioButton1"));
            uiavRadioButton2 = uiApp.Descendants.Find(UICondition.CreateFromId("avRadioButton2"));
            MS.Internal.Mita.Foundation.Controls.RadioButton rb2 = new MS.Internal.Mita.Foundation.Controls.RadioButton(uiavRadioButton2);
            uiavRadioButton3 = uiApp.Descendants.Find(UICondition.CreateFromId("avRadioButton3"));
            MS.Internal.Mita.Foundation.Controls.RadioButton rb3 = new MS.Internal.Mita.Foundation.Controls.RadioButton(uiavRadioButton3);
            //set focus to first radio button
            uiavRadioButton1.SetFocus();
            //send down arrow, press space bar - see if uiavRadioButton2 was set
            uiApp.SendKeys("{DOWN 1} ");
            Utilities.SleepDoEvents(2);
            //see if we got it
            sr.IncCounters(rb2.IsSelected, "avRadioButton2 did not get 'Checked'", p.log);
            //send down arrow, press space bar - see if uiavRadioButton3 was set
            uiApp.SendKeys("{DOWN 1} ");
            Utilities.SleepDoEvents(2);
            //see if we got it
            sr.IncCounters(rb3.IsSelected, "avRadioButton3 did not get 'Checked'", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("????When a EH's parent is veritically scrolled, verify that a vertical scroll bar on a EH textbox works as expected.")]
    public ScenarioResult Scenario14(TParams p)
    {
        UIObject uiavTextBox1 = null;
        try
        {
            uiavTextBox1 = uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            //send page down and see if we are on line 8
            uiavTextBox1.SendKeys("{PGDN}{HOME}+{END}");
            Utilities.SleepDoEvents(2);
            //see if we got it
            sr.IncCounters(avTextBox1.SelectedText == "This is line 8\n", "Text did not scroll", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
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