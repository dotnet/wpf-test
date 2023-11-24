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
// Author:      a-rickyt
//

public class Commands : ReflectBase
{
    #region Testcase setup

    Edit _edit1;
    Edit _edit2;
    CommandBinding binding;
    ElementHost elementHost1;
    ElementHost elementHost2;
    SWC.Button avButton1;
    SWC.Button avButton2;
    SWC.Button avButton3;
    SWC.StackPanel stackPanel;
    SWF.Label label;
    SWF.Button wfButton1;
    int counter;


    public Commands(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "Commands";
        this.Size = new System.Drawing.Size(400, 400);

        binding = new CommandBinding(ApplicationCommands.Copy);
        binding.Executed += new ExecutedRoutedEventHandler(binding_Executed);

        base.InitTest(p);
    }

    void binding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        counter++;
        label.Text = "CommandBinding executed by " + sender.ToString() + 
            ". Number of times: " + counter;
        Utilities.SleepDoEvents(10);
    }

    #endregion

    #region Scenario Setup

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        label = new SWF.Label();
        label.Width = 400;
        label.Height = 50;
        Controls.Add(label);

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
        counter = 0;
        Utilities.SleepDoEvents(20);
        return base.BeforeScenario(p, scenario);
    }

    void SetupScenario1()
    {
        avButton1 = new SWC.Button();
        avButton1.Name = "avButton1";
        avButton1.Content = "avButton1";

        avButton1.CommandBindings.Add(binding);

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Name = "elementHost1";
        elementHost1.Child = avButton1;
        elementHost1.Location = new System.Drawing.Point(20, 50);
        Controls.Add(elementHost1);
    }

    void SetupScenario2()
    {
        avButton1 = new SWC.Button();
        avButton1.Name = "avButton1";
        avButton1.Content = "avButton1";

        avButton2 = new SWC.Button();
        avButton2.Name = "avButton2";
        avButton2.Content = "avButton2";

        stackPanel = new SWC.StackPanel();
        stackPanel.CommandBindings.Add(binding);
        stackPanel.Children.Add(avButton1);
        stackPanel.Children.Add(avButton2);

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Name = "elementHost1";
        elementHost1.Child = stackPanel;
        elementHost1.Location = new System.Drawing.Point(20, 50);
        Controls.Add(elementHost1);
    }

    void SetupScenario3()
    {
        avButton1 = new SWC.Button();
        avButton1.Name = "avButton1";
        avButton1.Content = "avButton1";

        avButton2 = new SWC.Button();
        avButton2.Name = "avButton2";
        avButton2.Content = "avButton2";

        avButton3 = new SWC.Button();
        avButton3.Name = "avButton3";
        avButton3.Content = "avButton3";

        stackPanel = new SWC.StackPanel();
        stackPanel.Children.Add(avButton1);
        stackPanel.Children.Add(avButton2);
        stackPanel.Children.Add(avButton3);

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Name = "elementHost1";
        elementHost1.Child = stackPanel;
        elementHost1.Location = new System.Drawing.Point(20, 50);
        Controls.Add(elementHost1);

        avButton1.CommandBindings.Add(binding);
        avButton2.CommandBindings.Add(binding);
    }

    void SetupScenario4()
    {
        avButton1 = new SWC.Button();
        avButton1.Name = "avButton1";
        avButton1.Content = "avButton1";

        avButton2 = new SWC.Button();
        avButton2.Name = "avButton2";
        avButton2.Content = "avButton2";

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Name = "elementHost1";
        elementHost1.Child = avButton1;
        elementHost1.Location = new System.Drawing.Point(20, 50);
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.Name = "elementHost2";
        elementHost2.Child = avButton2;
        elementHost2.Location = new System.Drawing.Point(20, 220);
        Controls.Add(elementHost2);

        avButton1.CommandBindings.Add(binding);
        avButton2.CommandBindings.Add(binding);
    }

    void SetupScenario5()
    {
        label.Location = new System.Drawing.Point(0, 50);

        SWF.ToolStripMenuItem menuItem = new SWF.ToolStripMenuItem();
        menuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C;
        menuItem.Text = "Ctrl-C";
        menuItem.Click += new EventHandler(menuItem_Click);

        SWF.MenuStrip wfMenuStrip = new SWF.MenuStrip();
        wfMenuStrip.Items.Add(menuItem);
        Controls.Add(wfMenuStrip);

        wfButton1 = new SWF.Button();
        wfButton1.Name = "wfButton1";
        wfButton1.Text = "wfButton1";
        wfButton1.AutoSize = true;
        wfButton1.Location = new System.Drawing.Point(0, 100);
        Controls.Add(wfButton1);

        avButton1 = new SWC.Button();
        avButton1.Name = "avButton1";
        avButton1.Content = "avButton1";

        avButton2 = new SWC.Button();
        avButton2.Name = "avButton2";
        avButton2.Content = "avButton2";

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Name = "elementHost1";
        elementHost1.Child = avButton1;
        elementHost1.Location = new System.Drawing.Point(20, 140);
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.Name = "elementHost2";
        elementHost2.Child = avButton2;
        elementHost2.Location = new System.Drawing.Point(20, 260);
        Controls.Add(elementHost2);

        avButton1.CommandBindings.Add(binding);
        avButton2.CommandBindings.Add(binding);
    }

    void menuItem_Click(object sender, EventArgs e)
    {
        label.Text = "WinForm menu command executed.";
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

        sr.IncCounters(label.Text == "",
            "Failed at initial state. Label: "
            + label.Text, p.log);

        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "CommandBinding executed by " + avButton1.ToString() +
            ". Number of times: " + counter, "Failed at CommandBinding executed. Label: " 
            + label.Text, p.log);

        
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

        sr.IncCounters(label.Text == "",
            "Failed at initial state. Label: "
            + label.Text, p.log);

        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "CommandBinding executed by " + stackPanel.ToString() +
            ". Number of times: " + counter, "Failed at CommandBinding executed. Label: "
            + label.Text + ". Expected number of times: " + counter, p.log);

        _edit2.SetFocus();
        _edit2.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "CommandBinding executed by " + stackPanel.ToString() +
            ". Number of times: " + counter, "Failed at CommandBinding executed. Label: "
            + label.Text + ". Expected number of times: " + counter, p.log);

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

        sr.IncCounters(label.Text == "",
            "Failed at initial state. Label: "
            + label.Text, p.log);
        
        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "CommandBinding executed by " + avButton1 +
            ". Number of times: " + counter, "Failed at CommandBinding executed. Label: "
            + label.Text + ". Expected number of times: " + counter, p.log);

        _edit2.SetFocus();
        _edit2.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "CommandBinding executed by " + avButton2 +
            ". Number of times: " + counter, "Failed at CommandBinding executed. Label: "
            + label.Text + ". Expected number of times: " + counter, p.log);

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

        sr.IncCounters(label.Text == "",
            "Failed at initial state. Label: "
            + label.Text, p.log);

        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "CommandBinding executed by " + avButton1 +
            ". Number of times: " + counter, "Failed at CommandBinding executed. Label: "
            + label.Text + ". Expected number of times: " + counter, p.log);

        _edit2.SetFocus();
        _edit2.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "CommandBinding executed by " + avButton2 +
            ". Number of times: " + counter, "Failed at CommandBinding executed. Label: "
            + label.Text + ". Expected number of times: " + counter, p.log);

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

        sr.IncCounters(label.Text == "", "Failed at initial state. Label: "
            + label.Text, p.log);

        _edit1.SetFocus();
        _edit1.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "CommandBinding executed by " + avButton1 +
            ". Number of times: " + counter, "Failed at CommandBinding executed. Label: "
            + label.Text + ". Expected number of times: " + counter, p.log);

        _edit2.SetFocus();
        _edit2.SendKeys("^c");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(label.Text == "WinForm menu command executed.",
            "Failed at WinForm menu command executed. Label: "
            + label.Text, p.log);

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