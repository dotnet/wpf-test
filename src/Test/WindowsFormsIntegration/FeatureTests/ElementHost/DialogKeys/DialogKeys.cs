// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
//
public class DialogKeys : ReflectBase
{
    #region Testcase setup

    Edit _edit1;
    SWF.Button _wfOkButton = new SWF.Button();
    SWF.Button _wfCancelButton = new SWF.Button();
    SWC.Button _avOkButton = new SWC.Button();
    SWC.Button _avCancelButton = new SWC.Button();
    ElementHost _elementHost1 = new ElementHost();
    ElementHost _elementHost2 = new ElementHost();
    ElementHost _elementHost3 = new ElementHost();
    SWC.StackPanel _stackPanel = new SWC.StackPanel();
    SWC.TextBox _avTextBox = new SWC.TextBox();
    SWC.TextBox _avTextBox2 = new SWC.TextBox();
    SWC.RichTextBox _avRichTextBox = new SWC.RichTextBox();
    SWF.TextBox _wfTextBox = new SWF.TextBox();
    SWF.Label _label = new SWF.Label();

    public DialogKeys(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "DialogKeys";
        this.Size = new System.Drawing.Size(400, 400);
        this.AcceptButton = _wfOkButton;
        this.CancelButton = _wfCancelButton;

        _wfTextBox.Name = "wfTextBox";
        _wfTextBox.Text = "WinForm TextBox";
        _wfTextBox.AcceptsReturn = false;
        _wfTextBox.Multiline = true;
        _wfTextBox.Size = new System.Drawing.Size(150, 100);
        _wfTextBox.Location = new System.Drawing.Point(200, 50);
        Controls.Add(_wfTextBox);

        _label.Width = 200;
        _label.Location = new System.Drawing.Point(120, 200);
        Controls.Add(_label);

        _wfOkButton.Text = "OK";
        _wfOkButton.Location = new System.Drawing.Point(115, 330);
        _wfOkButton.Click += new EventHandler(wfOkButton_Click);
        Controls.Add(_wfOkButton);

        _wfCancelButton.Text = "Cancel";
        _wfCancelButton.Location = new System.Drawing.Point(200, 330);
        _wfCancelButton.Click += new EventHandler(wfCancelButton_Click);
        Controls.Add(_wfCancelButton);

        _avTextBox.Name = "avTextBox";
        _avTextBox.Text = "Avalon TextBox";
        _avTextBox.AcceptsReturn = false;

        _avTextBox2.Name = "avTextBox2";
        _avTextBox2.Text = "Avalon TextBox 2";
        _avTextBox2.AcceptsReturn = false;

        _avRichTextBox.Name = "avRichTextBox";
        _avRichTextBox.AppendText("Avalon Rich Text Box");
        _avRichTextBox.AcceptsReturn = false;

        _avOkButton.Name = "avOkButton";
        _avOkButton.Content = "OK";
        _avOkButton.IsDefault = true;
        _avOkButton.Click += new RoutedEventHandler(avOkButton_Click);

        _avCancelButton.Name = "avCancelButton";
        _avCancelButton.Content = "Cancel";
        _avCancelButton.IsCancel = true;
        _avCancelButton.Click += new RoutedEventHandler(avCancelButton_Click);

        _stackPanel.Name = "stackPanel";
        _stackPanel.Orientation = SWC.Orientation.Horizontal;
        _stackPanel.Children.Add(_avTextBox2);
        _stackPanel.Children.Add(_avOkButton);
        _stackPanel.Children.Add(_avCancelButton);

        //Create Element Host 1
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _avTextBox;
        _elementHost1.Size = new System.Drawing.Size(150, 100);
        _elementHost1.Location = new System.Drawing.Point(40, 50);
        _elementHost1.BackColor = Color.Red;
        Controls.Add(_elementHost1);

        //Create Element Host 2
        _elementHost2.Name = "elementHost2";
        _elementHost2.Child = _stackPanel; 
        _elementHost2.Location = new System.Drawing.Point(70, 230);
        _elementHost2.BackColor = Color.Red;
        _elementHost2.AutoSize = true;
        Controls.Add(_elementHost2);

        //Create Element Host 3
        _elementHost3.Name = "elementHost3";
        _elementHost3.Child = _avRichTextBox;
        _elementHost3.Size = new System.Drawing.Size(150, 50);
        _elementHost3.Location = new System.Drawing.Point(120, 270);
        _elementHost3.BackColor = Color.Red;
        Controls.Add(_elementHost3);

        base.InitTest(p);
    }

    void wfOkButton_Click(object sender, EventArgs e)
    {
        _label.Text = "WinForm OK button clicked.";
    }

    void wfCancelButton_Click(object sender, EventArgs e)
    {
        _label.Text = "WinForm Cancel button clicked.";
    }

    void avOkButton_Click(object sender, RoutedEventArgs e)
    {
        _label.Text = "Avalon OK button clicked.";
    }

    void avCancelButton_Click(object sender, RoutedEventArgs e)
    {
        _label.Text = "Avalon Cancel button clicked.";
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

        sr.IncCounters(_label.Text == "WinForm OK button clicked.", 
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

        sr.IncCounters(_label.Text == "WinForm Cancel button clicked.",
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
        sr.IncCounters(_label.Text == "WinForm OK button clicked.",
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
        sr.IncCounters(_label.Text == "WinForm Cancel button clicked.",
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
        sr.IncCounters(_label.Text == "WinForm OK button clicked.",
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
        sr.IncCounters(_label.Text == "WinForm Cancel button clicked.",
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
        sr.IncCounters(_label.Text == "WinForm OK button clicked.",
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
        sr.IncCounters(_label.Text == "Avalon OK button clicked.",
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
        sr.IncCounters(_label.Text == "Avalon Cancel button clicked.",
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