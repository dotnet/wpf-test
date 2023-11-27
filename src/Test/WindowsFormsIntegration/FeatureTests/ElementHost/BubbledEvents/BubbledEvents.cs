// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

using System.Windows.Forms.Integration;
using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Controls;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;

//
// Testcase:    BubbledEvents
// Description: Verify Bubbled events work in EH
//
public class BubbledEvents : ReflectBase {
    #region Testcase setup
    public BubbledEvents(string[] args) : base(args) { }

    ElementHost _host = new ElementHost();
    System.Windows.Controls.TextBox _avTextbox = new System.Windows.Controls.TextBox();
    System.Windows.Controls.DockPanel _avDockPanel = new System.Windows.Controls.DockPanel();
    System.Windows.Controls.Grid _avGrid = new System.Windows.Controls.Grid();
    System.Windows.Controls.Button _avButton = new System.Windows.Controls.Button();
    ScenarioResult _sr;
    UIObject _uiApp;
    System.Windows.Controls.StackPanel _avStackPanel = new System.Windows.Controls.StackPanel();
    string _routingStrategy;
    string _expectedRoutingStrategy = System.Windows.RoutingStrategy.Bubble.ToString();

    protected override void InitTest(TParams p) {
        base.InitTest(p);
        UseMita = true;
        this.Text = "BubbledEventsTest";
        _avTextbox.Name = "avTextbox";
        _avTextbox.Background = System.Windows.Media.Brushes.Cyan;
        _avStackPanel.Name = "avStackPanel";
        _avStackPanel.Background = System.Windows.Media.Brushes.Cornsilk;
        _avGrid.Name = "avGrid";
        _avGrid.Background = System.Windows.Media.Brushes.Aqua;
        _host.BackColor = System.Drawing.Color.Red;
        _avTextbox.KeyDown += new System.Windows.Input.KeyEventHandler(avControl_KeyDown);
        _avStackPanel.KeyDown += new System.Windows.Input.KeyEventHandler(avControl_KeyDown);
        _avGrid.KeyDown += new System.Windows.Input.KeyEventHandler(avControl_KeyDown);
        _avButton.Name = "avButton";
        _avButton.KeyDown += new System.Windows.Input.KeyEventHandler(avControl_KeyDown);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("BubbledEventsTest"));
        this.Controls.Clear();
        _host.Child = null;
        _avStackPanel.Children.Clear();
        _avGrid.Children.Clear();
        _avDockPanel.Children.Clear();
        _routingStrategy = string.Empty;
        switch (scenario.Name)
        {
            case "Scenario1":
                _avStackPanel.Children.Add(_avTextbox);
                _host.Child = _avStackPanel;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario2":
                _avGrid.Children.Add(_avStackPanel);
                _avStackPanel.Children.Add(_avTextbox);
                _host.Child = _avGrid;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario3":
                _avGrid.Children.Add(_avStackPanel);
                _avStackPanel.Children.Add(_avDockPanel);
                _avDockPanel.Children.Add(_avTextbox);
                _host.Child = _avGrid;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario4":
                _avGrid.Children.Add(_avStackPanel);
                _avStackPanel.Children.Add(_avDockPanel);
                _avDockPanel.Children.Add(_avButton);
                _avButton.Content = _avTextbox;
                _host.Child = _avGrid;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
        }
        return base.BeforeScenario(p, scenario);
    }

    void avControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        //assign RoutingStrategy to global string so a given scenario can examine it.
        //RoutingStrategy += ((System.Windows.FrameworkElement)sender).Name + ":" + e.RoutedEvent.RoutingStrategy.ToString() + "::";

        //Changed 4/27/06 per Nathan Enright to only look at the routing strategy itself:
        _routingStrategy = e.RoutedEvent.RoutingStrategy.ToString();
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Verify that bubbled events work for StackPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario1(TParams p) {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            //type a character in the AV textbox.  See that the top level StackPanel gets the bubbled event.
            uiavTextbox.SendKeys("x");
            Utilities.SleepDoEvents(10);
            _sr.IncCounters(_routingStrategy == _expectedRoutingStrategy, "Bubbled event not seen for AV Textbox", p.log);
            if (_routingStrategy != _expectedRoutingStrategy)
            {
                p.log.WriteLine("Scenario 1 expected: " + _expectedRoutingStrategy);
                p.log.WriteLine("Scenario 1 received: " + _routingStrategy);
            }
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return _sr;
    }

    [Scenario("Verify that bubbled events work for Grid->StackPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario2(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            //type a character in the AV textbox.  See that the top level Grid gets the bubbled event.
            uiavTextbox.SendKeys("x");
            Utilities.SleepDoEvents(10);
            _sr.IncCounters(_routingStrategy == _expectedRoutingStrategy, "Bubbled event not seen for AV Textbox", p.log);
            if (_routingStrategy != _expectedRoutingStrategy)
            {
                p.log.WriteLine("Scenario 2 expected: " + _expectedRoutingStrategy);
                p.log.WriteLine("Scenario 2 received: " + _routingStrategy);
            }
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return _sr;
    }

    [Scenario("Verify that bubbled events work for Grid->StackPanel->DockPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario3(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            //type a character in the AV textbox.  See that the top level Grid gets the bubbled event.
            uiavTextbox.SendKeys("x");
            Utilities.SleepDoEvents(10);
            _sr.IncCounters(_routingStrategy == _expectedRoutingStrategy, "Bubbled event not seen for AV Textbox", p.log);
            if (_routingStrategy != _expectedRoutingStrategy)
            {
                p.log.WriteLine("Scenario 3 expected: " + _expectedRoutingStrategy);
                p.log.WriteLine("Scenario 3 received: " + _routingStrategy);
            }
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return _sr;
    }

    [Scenario("Verify that bubbled events work for Grid->StackPanel->DockPanel->Button->TextBox inside EH boundary.")]
    public ScenarioResult Scenario4(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            //type a character in the AV textbox.  See that the top level Grid gets the bubbled event.
            uiavTextbox.SendKeys("x");
            Utilities.SleepDoEvents(10);
            _sr.IncCounters(_routingStrategy == _expectedRoutingStrategy, "Bubbled event not seen for AV Textbox", p.log);
            if (_routingStrategy != _expectedRoutingStrategy)
            {
                p.log.WriteLine("Scenario 4 expected: " + _expectedRoutingStrategy);
                p.log.WriteLine("Scenario 4 received: " + _routingStrategy);
            }
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return _sr;
    }

    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify that bubbled events work for StackPanel-&gt;TextBox inside EH boundary.
//@ Verify that bubbled events work for Grid-&gt;StackPanel-&gt;TextBox inside EH boundary.
//@ Verify that bubbled events work for Grid-&gt;StackPanel-&gt;DockPanel-&gt;TextBox inside EH boundary.
//@ Verify that bubbled events work for Grid-&gt;StackPanel-&gt;DockPanel-&gt;Button-&gt;TextBox inside EH boundary.