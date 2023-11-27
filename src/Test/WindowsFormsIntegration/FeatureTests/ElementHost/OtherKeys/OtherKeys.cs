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
// Testcase:    OtherKeys
// Description: This test catches any keys not listed in other tests.
//
public class OtherKeys : ReflectBase
{
    #region Testcase setup

    Edit _edit1;
    ElementHost _elementHost1;
    ElementHost _elementHost2;
    ElementHost _elementHost3;
    ElementHost _elementHost4;
    SWC.TextBox _avTextBox;
    SWC.Button _avButton;
    SWC.CheckBox _avCheckBox;
    SWC.RadioButton _avRadioButton1;
    SWC.RadioButton _avRadioButton2;
    SWC.StackPanel _stackPanel;
    SWF.Label _label = new SWF.Label();


    public OtherKeys(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "OtherKeys";
        this.Size = new System.Drawing.Size(400, 500);

        //Create label
        _label.Width = 200;
        _label.Location = new System.Drawing.Point(120, 430);
        Controls.Add(_label);

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Size = new System.Drawing.Size(150, 100);
        _elementHost1.Location = new System.Drawing.Point(100, 50);
        _elementHost1.BackColor = Color.Red;
        Controls.Add(_elementHost1);

        //Create AV TextBox
        _avTextBox = new SWC.TextBox();
        _avTextBox.Name = "avTextBox";
        _avTextBox.AcceptsReturn = true;
        _avTextBox.Width = 150;
        _avTextBox.FontFamily = new System.Windows.Media.FontFamily("Arial");
        _avTextBox.FontSize = 11;
        _avTextBox.TextWrapping = TextWrapping.Wrap;
        _elementHost1.Child = _avTextBox;

        //Create Element Host 2
        _elementHost2 = new ElementHost();
        _elementHost2.Size = new System.Drawing.Size(150, 50);
        _elementHost2.Location = new System.Drawing.Point(100, 200);
        _elementHost2.BackColor = this.BackColor;
        Controls.Add(_elementHost2);

        //Create AV Button
        _avButton = new SWC.Button();
        _avButton.Content = "Avalon Button";
        _avButton.Name = "avButton";
        _elementHost2.Child = _avButton;

        //Create Element Host 3
        _elementHost3 = new ElementHost();
        _elementHost3.Size = new System.Drawing.Size(150, 50);
        _elementHost3.Location = new System.Drawing.Point(100, 275);
        _elementHost3.BackColor = this.BackColor;
        Controls.Add(_elementHost3);

        //Create AV CheckBox
        _avCheckBox = new SWC.CheckBox();
        _avCheckBox.Content = "Avalon CheckBox";
        _avCheckBox.Name = "avCheckBox";
        _avCheckBox.Unchecked += new RoutedEventHandler(avCheckBox_Unchecked);
        _elementHost3.Child = _avCheckBox;

        //Create Element Host 4
        _elementHost4 = new ElementHost();
        _elementHost4.Size = new System.Drawing.Size(150, 50);
        _elementHost4.Location = new System.Drawing.Point(100, 330);
        _elementHost4.BackColor = this.BackColor;
        Controls.Add(_elementHost4);

        //Create AV RadioButton
        _avRadioButton1 = new SWC.RadioButton();
        _avRadioButton1.Content = "Avalon RadioButton 1";
        _avRadioButton1.Name = "avRadioButton1";
        _avRadioButton2 = new SWC.RadioButton();
        _avRadioButton2.Content = "Avalon RadioButton 2";
        _avRadioButton2.Name = "avRadioButton2";
        _avRadioButton2.Click += new RoutedEventHandler(avRadioButton_Click);
        _stackPanel = new SWC.StackPanel();
        _stackPanel.Name = "stackPanel";
        _stackPanel.Children.Add(_avRadioButton1);
        _stackPanel.Children.Add(_avRadioButton2);
        _elementHost4.Child = _stackPanel;

        base.InitTest(p);
    }

    void avButton_Click(object sender, RoutedEventArgs e)
    {
        _label.Text = "SpaceBar works for Button";
    }

    void avCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _label.Text = "SpaceBar works for CheckBox";
    }

    void avRadioButton_Click(object sender, RoutedEventArgs e)
    {
        _label.Text = "SpaceBar works for RadioButton";
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        _avTextBox.Text = "";
        return base.BeforeScenario(p, scenario);
    }

    [Scenario("Verify that Home and End key works (using a text box).")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "avTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary);

        _edit1.SendKeys("Avalon");
        _edit1.SendKeys("{HOME}");
        _edit1.SendKeys("Hello ");

        sr.IncCounters(_edit1.Value == "Hello Avalon", "Failed at Home key.  The textbox contained: "
            + _edit1.Value, p.log);

        _edit1.SendKeys("{END}");
        _edit1.SendKeys(" TextBox");
        Utilities.SleepDoEvents(20);
        sr.IncCounters(_edit1.Value == "Hello Avalon TextBox", "Failed at End key with single line of text.  " + 
            "The textbox contained: " + _edit1.Value, p.log);

        _edit1.SendKeys(" contains a really long string of text so we can see the behavior of home and end " +
            "in multi-line textbox.");
        Utilities.SleepDoEvents(2);
        _edit1.SendKeys("{HOME}");
        Utilities.SleepDoEvents(2);
        _edit1.SendKeys("a ");
        Utilities.SleepDoEvents(10);

	string expectedString = "Hello Avalon TextBox contains a really long string of text so we can " +
            "see the behavior of home and end in a multi-line textbox.";

        sr.IncCounters(_edit1.Value == expectedString, "Failed at End key with multi-line text. \n" + 
            "Actual:   " + _edit1.Value + "\nExpected: " + expectedString, p.log);

        return sr;
    }

    [Scenario("Verify that Delete key works (using a text box).")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "avTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary);
        _edit1.SendKeys("Hello Avalon TextBox");
        _edit1.SendKeys("{HOME}{DELETE}");

        sr.IncCounters(_edit1.Value == "ello Avalon TextBox", "Failed at Delete key.  The textbox contained: "
            + _edit1.Value, p.log);

        _edit1.SendKeys("{DELETE}{DELETE}{DELETE}{DELETE}{DELETE}");

        sr.IncCounters(_edit1.Value == "Avalon TextBox", "Failed at Delete key.  The textbox contained: "
            + _edit1.Value, p.log);

        return sr;
    }

    [Scenario("Verify that SpaceBar works for Button.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "avButton")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        _edit1.Click(PointerButtons.Primary);
        _avButton.Click += new RoutedEventHandler(avButton_Click);
        _edit1.SendKeys(" ");
 
        Utilities.SleepDoEvents(20);
        sr.IncCounters(_label.Text == "SpaceBar works for Button", "Failed at SpaceBar on Button", p.log);

        return sr;
    }

    [Scenario("Verify that SpaceBar works for CheckBox.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "avCheckBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary);
        _edit1.SendKeys(" ");

        Utilities.SleepDoEvents(20);
        sr.IncCounters(_label.Text == "SpaceBar works for CheckBox", "Failed at SpaceBar on CheckBox", p.log);

        return sr;
    }

    [Scenario("Verify that SpaceBar works for RadioButton.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "avRadioButton1")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary);
        Utilities.SleepDoEvents(10);

        //tab to RadioButton2, then SpaceBar to 'click' it
        SWF.SendKeys.SendWait("{TAB}");
        SWF.SendKeys.SendWait(" ");

        Utilities.SleepDoEvents(10);
        sr.IncCounters(_label.Text == "SpaceBar works for RadioButton", "Failed at SpaceBar on RadioButton", p.log);

        return sr;
    }

    [Scenario("Verify that special keys work with misc. text in a multi-line text box.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "avTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        _edit1.Click(PointerButtons.Primary);
        _edit1.SendKeys("Avalon TextBox Line 1. Hello from Avalon Line 2. The last one Line 3");

        //Delete 'A' from first line
        _edit1.SendKeys("{UP}{UP}{UP}{UP}{HOME}{DELETE}");

        // Unicode / Arabic
        _edit1.SendKeys("{END}{ENTER}\x0622\x062e\x0631");
        Utilities.SleepDoEvents(2);
        _edit1.SendKeys("{BACKSPACE}{BACKSPACE}{BACKSPACE}{BACKSPACE}");
	Utilities.SleepDoEvents(2);

        sr.IncCounters(_edit1.Value == "valon TextBox Line 1. Hello from Avalon Line 2. The last one Line 3",
            "Failed at misc. text in a multi-line textbox: " + _edit1.Value, p.log);

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
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("OtherKeys"));
            uiTb1 = uiApp.Descendants.Find(UICondition.CreateFromId(controlName));
            _edit1 = new Edit(uiTb1);
            return true;
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls. " + ex.Message);
            return false;
        }
    }
    
    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Verify that Home and End key works (using a text box)
//@ Verify that Delete key works (using a text box)
//@ Verify that SpaceBar works for Button.
//@ Verify that SpaceBar works for CheckBox.
//@ Verify that SpaceBar works for RadioButton.
//@ Verify that special keys work with misc. text in a multi-line text box.