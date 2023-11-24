using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using SWF = System.Windows.Forms;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;
using System.Threading;
using System.Reflection;

//
// Testcase:    DialogKeys
// Description: When a WF control has dialog keys assigned and is hosted in an EH, we need 
//              to make sure that those dialog keys still function correctly.
// Author:      a-rickyt
//
public class DialogKeys : ReflectBase
{
    #region Testcase setup

    Edit _edit1;
    SWF.Button wfOkButton = new SWF.Button();
    SWF.Button wfCancelButton = new SWF.Button();
    SWC.Button avOkButton = new SWC.Button();
    SWC.Button avCancelButton = new SWC.Button();
    ElementHost elementHost1 = new ElementHost();
    ElementHost elementHost2 = new ElementHost();
    ElementHost elementHost3 = new ElementHost();
    SWC.StackPanel stackPanel = new SWC.StackPanel();
    SWC.TextBox avTextBox = new SWC.TextBox();
    SWC.TextBox avTextBox2 = new SWC.TextBox();
    SWC.RichTextBox avRichTextBox = new SWC.RichTextBox();
    SWF.TextBox wfTextBox = new SWF.TextBox();
    SWF.Label label = new SWF.Label();

    public DialogKeys(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "DialogKeys";
        this.Size = new System.Drawing.Size(400, 400);
        this.AcceptButton = wfOkButton;
        this.CancelButton = wfCancelButton;

        wfTextBox.Name = "wfTextBox";
        wfTextBox.Text = "WinForm TextBox";
        wfTextBox.AcceptsReturn = false;
        wfTextBox.Multiline = true;
        wfTextBox.Size = new System.Drawing.Size(150, 100);
        wfTextBox.Location = new System.Drawing.Point(200, 50);
        Controls.Add(wfTextBox);

        label.Width = 200;
        label.Location = new System.Drawing.Point(120, 200);
        Controls.Add(label);

        wfOkButton.Text = "OK";
        wfOkButton.Location = new System.Drawing.Point(115, 330);
        wfOkButton.Click += new EventHandler(wfOkButton_Click);
        Controls.Add(wfOkButton);

        wfCancelButton.Text = "Cancel";
        wfCancelButton.Location = new System.Drawing.Point(200, 330);
        wfCancelButton.Click += new EventHandler(wfCancelButton_Click);
        Controls.Add(wfCancelButton);

        avTextBox.Name = "avTextBox";
        avTextBox.Text = "Avalon TextBox";
        avTextBox.AcceptsReturn = false;

        avTextBox2.Name = "avTextBox2";
        avTextBox2.Text = "Avalon TextBox 2";
        avTextBox2.AcceptsReturn = false;

        avRichTextBox.Name = "avRichTextBox";
        avRichTextBox.AppendText("Avalon Rich Text Box");
        avRichTextBox.AcceptsReturn = false;

        avOkButton.Name = "avOkButton";
        avOkButton.Content = "OK";
        avOkButton.IsDefault = true;
        avOkButton.Click += new RoutedEventHandler(avOkButton_Click);

        avCancelButton.Name = "avCancelButton";
        avCancelButton.Content = "Cancel";
        avCancelButton.IsCancel = true;
        avCancelButton.Click += new RoutedEventHandler(avCancelButton_Click);

        stackPanel.Name = "stackPanel";
        stackPanel.Orientation = SWC.Orientation.Horizontal;
        stackPanel.Children.Add(avTextBox2);
        stackPanel.Children.Add(avOkButton);
        stackPanel.Children.Add(avCancelButton);

        //Create Element Host 1
        elementHost1.Name = "elementHost1";
        elementHost1.Child = avTextBox;
        elementHost1.Size = new System.Drawing.Size(150, 100);
        elementHost1.Location = new System.Drawing.Point(40, 50);
        elementHost1.BackColor = Color.Red;
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2.Name = "elementHost2";
        elementHost2.Child = stackPanel; 
        elementHost2.Location = new System.Drawing.Point(70, 230);
        elementHost2.BackColor = Color.Red;
        elementHost2.AutoSize = true;
        Controls.Add(elementHost2);

        //Create Element Host 3
        elementHost3.Name = "elementHost3";
        elementHost3.Child = avRichTextBox;
        elementHost3.Size = new System.Drawing.Size(150, 50);
        elementHost3.Location = new System.Drawing.Point(120, 270);
        elementHost3.BackColor = Color.Red;
        Controls.Add(elementHost3);

        base.InitTest(p);
    }

    void wfOkButton_Click(object sender, EventArgs e)
    {
        label.Text = "WinForm OK button clicked.";
    }

    void wfCancelButton_Click(object sender, EventArgs e)
    {
        label.Text = "WinForm Cancel button clicked.";
    }

    void avOkButton_Click(object sender, RoutedEventArgs e)
    {
        label.Text = "Avalon OK button clicked.";
    }

    void avCancelButton_Click(object sender, RoutedEventArgs e)
    {
        label.Text = "Avalon Cancel button clicked.";
    }

    #endregion

    #region Scenarios

    [Scenario("Form with OK button assigned and focus on Avalon element hosted on EH.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "avTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary); //focus on Avalon element (textbox)
        _edit1.SendKeys("{ENTER}");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "WinForm OK button clicked.", 
            "Failed at OK button with focus on Avalon element.", p.log);

        return sr;
    }

    [Scenario("Form with ESC button assigned and focus on Avalon element hosted on EH.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "avTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary); //focus on Avalon element (textbox)
        _edit1.SendKeys("{ESC}");

        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "WinForm Cancel button clicked.",
            "Failed at ESC button with focus on Avalon element.", p.log);
 
        return sr;
    }

    [Scenario("Avalon element on EH with OK button assigned and focus on a WF control. " + 
        "Expected OK from wfOkButton.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "wfTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary); //focus on WinForm control (textbox)
        _edit1.SendKeys("{ENTER}");

        Utilities.SleepDoEvents(10);
        sr.IncCounters(label.Text == "WinForm OK button clicked.",
            "Failed at Avalon element on EH with OK button assigned and focus on a WF control.", p.log);

        return sr;
    }

    [Scenario("Avalon element on EH with Cancel button assigned and focus on a WF control. " + 
        "Expected Cancel from wfCancelButton.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "wfTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary); //focus on WinForm control (textbox)
        _edit1.SendKeys("{ESC}");

        Utilities.SleepDoEvents(10);
        sr.IncCounters(label.Text == "WinForm Cancel button clicked.",
            "Failed at Avalon element on EH with Cancel button assigned and focus on a WF control.", p.log);

        return sr;
    }

    [Scenario("Avalon element on EH and Winform control both have OK button assigned. " +
       "Expected OK from wfOkButton.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(10);
        if (!GetEditControls(p, "avTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary); //focus on Avalon element (textbox)
        _edit1.SendKeys("{ENTER}");

        Utilities.SleepDoEvents(10);
        sr.IncCounters(label.Text == "WinForm OK button clicked.",
           "Failed at Avalon element on EH and Winform control both have OK button assigned.", p.log);

        return sr;
    }

    [Scenario("Avalon element on EH and Winform control both have ESC button assigned. " +
        "Expected Cancel from wfCancelButton.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "avTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary); //focus on Avalon element (textbox)
        _edit1.SendKeys("{ESC}");

        Utilities.SleepDoEvents(10);
        sr.IncCounters(label.Text == "WinForm Cancel button clicked.",
            "Failed at Avalon element on EH and Winform control both have ESC button assigned.", p.log);

        return sr;
    }

    [Scenario("Avalon RichText on EH with acceptReturn==false. Hitting Enter should not invoke Accept button.")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "avRichTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary); //focus on Avalon element (textbox)
        _edit1.SendKeys("{ENTER}");

        Utilities.SleepDoEvents(10);
        sr.IncCounters(label.Text == "WinForm OK button clicked.",
           "Failed at Avalon RichText on EH with acceptReturn==false.", p.log);

        return sr;
    }

    [Scenario("AV control with OK button assigned with a EH on the app with a control (ENTER).")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetAvEditControls(p, "elementHost2")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary); //focus on Avalon element (textbox)
        _edit1.SendKeys("{ENTER}");

        Utilities.SleepDoEvents(10);
        sr.IncCounters(label.Text == "Avalon OK button clicked.",
           "Failed at AV control with OK button assigned with a EH on the app with a control (ENTER).", p.log);

        return sr;
    }

    [Scenario("AV control with Cancel button assigned with a EH on the app with a control (ESC).")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetAvEditControls(p, "elementHost2")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary); //focus on Avalon element (textbox)
        _edit1.SendKeys("{ESC}");

        Utilities.SleepDoEvents(10);
        sr.IncCounters(label.Text == "Avalon Cancel button clicked.",
           "Failed at AV control with Cancel button assigned with a EH on the app with a control (ESC).", p.log);

        return sr;
    }

    #endregion

    #region Utilities

    //Gets Mita wrapper control for controlName and passes it to _edit1
    bool GetEditControls(TParams p, String controlName)
    {
        UIObject uiApp = null;
        UIObject uiTb1 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("DialogKeys"));
            uiTb1 = uiApp.Descendants.Find(UICondition.CreateFromId(controlName));
            _edit1 = new Edit(uiTb1);
            return true;
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls" + ex.ToString());
            return false;
        }
    }
    
    //Gets Mita wrapper control of first child for controlName and passes it to _edit1.
    //Mita issue with accessibility in ElementHost, cannot get controls other than FirstChild.
    //The other controls just don't show up on MitaSpy.
    bool GetAvEditControls(TParams p, String controlName)
    {
        UIObject uiApp = null;
        UIObject uiTb1 = null;
        UIObject uiTb2 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("DialogKeys"));
            uiTb1 = uiApp.Descendants.Find(UICondition.CreateFromId(controlName));
            uiTb2 = new UIObject(uiTb1.FirstChild);
            _edit1 = new Edit(uiTb2);
            return true;
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls" + ex.ToString());
            return false;
        }
    }
    
    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Form with OK button assigned and focus on Avalon element hosted on EH.
//@ Form with ESC button assigned and focus on Avalon element hosted on EH.
//@ Avalon element on EH with OK button assigned and focus on a WF control.
//@ Avalon element on EH with Cancel button assigned and focus on a WF control.
//@ Avalon element on EH and Winform control both have OK button assigned.
//@ Avalon element on EH and Winform control both have ESC button assigned.
//@ Avalon RichText on EH with acceptReturn==false. Hitting Enter should not invoke Accept button.
//@ AV control with OK button assigned with a EH on the app with a control (ENTER).
//@ AV control with Cancel button assigned with a EH on the app with a control (ESC).