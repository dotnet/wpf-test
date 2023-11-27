// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
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
// Testcase:    Commands
// Description: Command Key Combination can be bound to any level of UI hierarchy in Avalon. 
//              We need to verify that the command handler executes correctly.
//
public class Commands : ReflectBase
{
    #region Testcase setup

    Edit _edit1;
    Edit _edit2;
    CommandBinding _binding;
    ElementHost _elementHost1;
    ElementHost _elementHost2;
    SWC.Button _avButton1;
    SWC.Button _avButton2;
    SWC.Button _avButton3;
    SWC.StackPanel _stackPanel;
    SWF.Label _label;
    SWF.Button _wfButton1;
    int _counter;


    public Commands(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "Commands";
        this.Size = new System.Drawing.Size(400, 400);

        _binding = new CommandBinding(ApplicationCommands.Copy);
        _binding.Executed += new ExecutedRoutedEventHandler(binding_Executed);

        base.InitTest(p);
    }

    void binding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        _counter++;
        _label.Text = "CommandBinding executed by " + sender.ToString() + 
            ". Number of times: " + _counter;
        Utilities.SleepDoEvents(10);
    }

    #endregion

    #region Scenario Setup

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        _label = new SWF.Label();
        _label.Width = 400;
        _label.Height = 50;
        Controls.Add(_label);

        switch (scenario.Name)
        {
            case "Scenario1":
                SetupScenario1();
                break;

            case "Scenario2":
                SetupScenario2();
                break;

            case "Scenario3":
                SetupScenario3();
                break;

            case "Scenario4":
                SetupScenario4();
                break;

            case "Scenario5":
                SetupScenario5();
                break;
        }
        _counter = 0;
        Utilities.SleepDoEvents(20);
        return base.BeforeScenario(p, scenario);
    }

    void SetupScenario1()
    {
        _avButton1 = new SWC.Button();
        _avButton1.Name = "avButton1";
        _avButton1.Content = "avButton1";

        _avButton1.CommandBindings.Add(_binding);

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _avButton1;
        _elementHost1.Location = new System.Drawing.Point(20, 50);
        Controls.Add(_elementHost1);
    }

    void SetupScenario2()
    {
        _avButton1 = new SWC.Button();
        _avButton1.Name = "avButton1";
        _avButton1.Content = "avButton1";

        _avButton2 = new SWC.Button();
        _avButton2.Name = "avButton2";
        _avButton2.Content = "avButton2";

        _stackPanel = new SWC.StackPanel();
        _stackPanel.CommandBindings.Add(_binding);
        _stackPanel.Children.Add(_avButton1);
        _stackPanel.Children.Add(_avButton2);

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _stackPanel;
        _elementHost1.Location = new System.Drawing.Point(20, 50);
        Controls.Add(_elementHost1);
    }

    void SetupScenario3()
    {
        _avButton1 = new SWC.Button();
        _avButton1.Name = "avButton1";
        _avButton1.Content = "avButton1";

        _avButton2 = new SWC.Button();
        _avButton2.Name = "avButton2";
        _avButton2.Content = "avButton2";

        _avButton3 = new SWC.Button();
        _avButton3.Name = "avButton3";
        _avButton3.Content = "avButton3";

        _stackPanel = new SWC.StackPanel();
        _stackPanel.Children.Add(_avButton1);
        _stackPanel.Children.Add(_avButton2);
        _stackPanel.Children.Add(_avButton3);

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _stackPanel;
        _elementHost1.Location = new System.Drawing.Point(20, 50);
        Controls.Add(_elementHost1);

        _avButton1.CommandBindings.Add(_binding);
        _avButton2.CommandBindings.Add(_binding);
    }

    void SetupScenario4()
    {
        _avButton1 = new SWC.Button();
        _avButton1.Name = "avButton1";
        _avButton1.Content = "avButton1";

        _avButton2 = new SWC.Button();
        _avButton2.Name = "avButton2";
        _avButton2.Content = "avButton2";

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _avButton1;
        _elementHost1.Location = new System.Drawing.Point(20, 50);
        Controls.Add(_elementHost1);

        //Create Element Host 2
        _elementHost2 = new ElementHost();
        _elementHost2.Name = "elementHost2";
        _elementHost2.Child = _avButton2;
        _elementHost2.Location = new System.Drawing.Point(20, 220);
        Controls.Add(_elementHost2);

        _avButton1.CommandBindings.Add(_binding);
        _avButton2.CommandBindings.Add(_binding);
    }

    void SetupScenario5()
    {
        _label.Location = new System.Drawing.Point(0, 50);

        SWF.ToolStripMenuItem menuItem = new SWF.ToolStripMenuItem();
        menuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C;
        menuItem.Text = "Ctrl-C";
        menuItem.Click += new EventHandler(menuItem_Click);

        SWF.MenuStrip wfMenuStrip = new SWF.MenuStrip();
        wfMenuStrip.Items.Add(menuItem);
        Controls.Add(wfMenuStrip);

        _wfButton1 = new SWF.Button();
        _wfButton1.Name = "wfButton1";
        _wfButton1.Text = "wfButton1";
        _wfButton1.AutoSize = true;
        _wfButton1.Location = new System.Drawing.Point(0, 100);
        Controls.Add(_wfButton1);

        _avButton1 = new SWC.Button();
        _avButton1.Name = "avButton1";
        _avButton1.Content = "avButton1";

        _avButton2 = new SWC.Button();
        _avButton2.Name = "avButton2";
        _avButton2.Content = "avButton2";

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _avButton1;
        _elementHost1.Location = new System.Drawing.Point(20, 140);
        Controls.Add(_elementHost1);

        //Create Element Host 2
        _elementHost2 = new ElementHost();
        _elementHost2.Name = "elementHost2";
        _elementHost2.Child = _avButton2;
        _elementHost2.Location = new System.Drawing.Point(20, 260);
        Controls.Add(_elementHost2);

        _avButton1.CommandBindings.Add(_binding);
        _avButton2.CommandBindings.Add(_binding);
    }

    void menuItem_Click(object sender, EventArgs e)
    {
        _label.Text = "WinForm menu command executed.";
        Utilities.SleepDoEvents(10);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult sr)
    {
        Controls.Clear();
        Utilities.SleepDoEvents(10);
        base.AfterScenario(p, scenario, sr);
    }

    #endregion

    #region Scenarios

    [Scenario("Avalon control having focus and it binds a command key combination. Command should execute.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "Commands", "avButton1", "avButton1")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        sr.IncCounters(_label.Text == "",
            "Failed at initial state. Label: "
            + _label.Text, p.log);

        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_label.Text == "CommandBinding executed by " + _avButton1.ToString() +
            ". Number of times: " + _counter, "Failed at CommandBinding executed. Label: " 
            + _label.Text, p.log);

        
        return sr;
    }

    [Scenario("Avalon control having focus but does not bind a command key combination, but a parent element does. Command should execute when first instance is found.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "Commands", "avButton1", "avButton2")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        sr.IncCounters(_label.Text == "",
            "Failed at initial state. Label: "
            + _label.Text, p.log);

        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_label.Text == "CommandBinding executed by " + _stackPanel.ToString() +
            ". Number of times: " + _counter, "Failed at CommandBinding executed. Label: "
            + _label.Text + ". Expected number of times: " + _counter, p.log);

        _edit2.SetFocus();
        _edit2.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_label.Text == "CommandBinding executed by " + _stackPanel.ToString() +
            ". Number of times: " + _counter, "Failed at CommandBinding executed. Label: "
            + _label.Text + ". Expected number of times: " + _counter, p.log);

        return sr;
    }

    [Scenario("Multiple Avalon elements in EH binding to the same command key combination. Check Avalon behavior.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "Commands", "avButton1", "avButton2")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        sr.IncCounters(_label.Text == "",
            "Failed at initial state. Label: "
            + _label.Text, p.log);
        
        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_label.Text == "CommandBinding executed by " + _avButton1 +
            ". Number of times: " + _counter, "Failed at CommandBinding executed. Label: "
            + _label.Text + ". Expected number of times: " + _counter, p.log);

        _edit2.SetFocus();
        _edit2.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_label.Text == "CommandBinding executed by " + _avButton2 +
            ". Number of times: " + _counter, "Failed at CommandBinding executed. Label: "
            + _label.Text + ". Expected number of times: " + _counter, p.log);

        return sr;
    }

    [Scenario("Two EH host with controls mapped to the same command. Check Avalon Scenario. Only the control having focus should get this.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "Commands", "avButton1", "avButton2")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        sr.IncCounters(_label.Text == "",
            "Failed at initial state. Label: "
            + _label.Text, p.log);

        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_label.Text == "CommandBinding executed by " + _avButton1 +
            ". Number of times: " + _counter, "Failed at CommandBinding executed. Label: "
            + _label.Text + ". Expected number of times: " + _counter, p.log);

        _edit2.SetFocus();
        _edit2.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_label.Text == "CommandBinding executed by " + _avButton2 +
            ". Number of times: " + _counter, "Failed at CommandBinding executed. Label: "
            + _label.Text + ". Expected number of times: " + _counter, p.log);

        return sr;
    }

    [Scenario("EH and Winform have same shortcut. EH control binds this shortcut to a command. If focus is within element host EH wins. If on WinForm the form wins.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "Commands", "avButton1", "wfButton1")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        sr.IncCounters(_label.Text == "", "Failed at initial state. Label: "
            + _label.Text, p.log);

        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_label.Text == "CommandBinding executed by " + _avButton1 +
            ". Number of times: " + _counter, "Failed at CommandBinding executed. Label: "
            + _label.Text + ". Expected number of times: " + _counter, p.log);

        _edit2.SetFocus();
        _edit2.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_label.Text == "WinForm menu command executed.",
            "Failed at WinForm menu command executed. Label: "
            + _label.Text, p.log);

        return sr;
    }

    #endregion

    #region Utilities

    //Gets Mita wrapper controls from control1 and control2 and passes them to _edit1 and 
    //_edit2 respectively.
    bool GetEditControls(TParams p, String window, String control1, String control2)
    {
        UIObject uiApp = null;
        UIObject uiControl1 = null;
        UIObject uiControl2 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window));
            uiControl1 = uiApp.Descendants.Find(UICondition.CreateFromId(control1));
            _edit1 = new Edit(uiControl1);

            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window));
            uiControl2 = uiApp.Descendants.Find(UICondition.CreateFromId(control2));

            _edit2 = new Edit(uiControl2);
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
//@ Avalon control having focus and it binds a command key combination. Command should execute.
//@ Avalon control having focus but does not bind a command key combination, but a parent element does. Command should execute when first instance is found.
//@ Multiple Avalon elements in EH binding to the same command key combination. Check Avalon behavior.
//@ Two EH host with controls mapped to the same command. Check Avalon Scenario. Only the control having focus should get this.
//@ EH and Winform have same shortcut. EH control binds this shortcut to a command. If focus is within element host EH wins. If on WinForm the form wins.