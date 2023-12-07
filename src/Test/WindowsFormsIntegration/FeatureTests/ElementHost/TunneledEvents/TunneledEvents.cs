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
// Testcase:    TunneledEvents
// Description: Verify Tunneled events work in EH
//
public class TunneledEvents : ReflectBase {
    #region Testcase setup
    public TunneledEvents(string[] args) : base(args) { }

    ElementHost _host = new ElementHost();
    System.Windows.Controls.TextBox _avTextbox = new System.Windows.Controls.TextBox();
    System.Windows.Controls.DockPanel _avDockPanel = new System.Windows.Controls.DockPanel();
    System.Windows.Controls.Grid _avGrid = new System.Windows.Controls.Grid();
    ScenarioResult _sr;
    UIObject _uiApp;
    System.Windows.Controls.StackPanel _avStackPanel = new System.Windows.Controls.StackPanel();
    string _routingStrategy;

    protected override void InitTest(TParams p) {
        base.InitTest(p);
        UseMita = true;
        this.Text = "TunneledEventsTest";
        _avTextbox.Name = "avTextbox";
        _avTextbox.Background = System.Windows.Media.Brushes.Cyan;
        _avStackPanel.Name = "avStackPanel";
        _avStackPanel.Background = System.Windows.Media.Brushes.Cornsilk;
        _host.BackColor = System.Drawing.Color.Red;
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TunneledEventsTest"));
        this.Controls.Clear();
        _host.Child = null;
        _avStackPanel.Children.Clear();
        _avGrid.Children.Clear();
        switch (scenario.Name)
        {
            case "Scenario1":
                _avStackPanel.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(avControl_PreviewKeyDown);
                _avStackPanel.Children.Add(_avTextbox);
                _host.Child = _avStackPanel;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario2":
                _avGrid.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(avControl_PreviewKeyDown);
                _avGrid.Children.Add(_avStackPanel);
                _avStackPanel.Children.Add(_avTextbox);
                _host.Child = _avGrid;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
            case "Scenario3":
                _avGrid.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(avControl_PreviewKeyDown);
                _avGrid.Children.Add(_avStackPanel);
                _avStackPanel.Children.Add(_avDockPanel);
                _avDockPanel.Children.Add(_avTextbox);
                _host.Child = _avGrid;
                this.Controls.Add(_host);
                _sr = new ScenarioResult();
                break;
        }
        return base.BeforeScenario(p, scenario);
    }

    void avControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        //assign RoutingStrategy to global string so a given scenario can examine it.
        _routingStrategy = e.RoutedEvent.RoutingStrategy.ToString();
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Verify that tunneled events work for StackPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario1(TParams p) {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            _routingStrategy = "";
            //type a character in the AV textbox.  See that the top level StackPanel gets the tunneled event.
            uiavTextbox.SendKeys("X");
            Utilities.SleepDoEvents(10);
            _sr.IncCounters(_routingStrategy == System.Windows.RoutingStrategy.Tunnel.ToString(), "Tunneled event not seen for AV Textbox", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Verify that tunneled events work for Grid->StackPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario2(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            _routingStrategy = "";
            //type a character in the AV textbox.  See that the top level Grid gets the tunneled event.
            uiavTextbox.SendKeys("X");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(_routingStrategy == System.Windows.RoutingStrategy.Tunnel.ToString(), "Tunneled event not seen for AV Textbox", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return _sr;
    }

    [Scenario("Verify that tunneled events work for Grid->StackPanel->DockPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario3(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = _uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            _routingStrategy = "";
            //type a character in the AV textbox.  See that the top level Grid gets the tunneled event.
            uiavTextbox.SendKeys("X");
            Utilities.SleepDoEvents(2);
            _sr.IncCounters(_routingStrategy == System.Windows.RoutingStrategy.Tunnel.ToString(), "Tunneled event not seen for AV Textbox", p.log);
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
//@ Verify that tunneled events work for StackPanel-&gt;TextBox inside EH boundary.
//@ Verify that tunneled events work for Grid-&gt;StackPanel-&gt;TextBox inside EH boundary.
//@ Verify that tunneled events work for Grid-&gt;StackPanel-&gt;DockPanel-&gt;TextBox inside EH boundary.
