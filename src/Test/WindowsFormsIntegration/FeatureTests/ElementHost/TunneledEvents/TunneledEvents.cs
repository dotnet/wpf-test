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
// Author:      a-larryl
//
public class TunneledEvents : ReflectBase {

    #region Testcase setup
    public TunneledEvents(string[] args) : base(args) { }

    ElementHost host = new ElementHost();
    System.Windows.Controls.TextBox avTextbox = new System.Windows.Controls.TextBox();
    System.Windows.Controls.DockPanel avDockPanel = new System.Windows.Controls.DockPanel();
    System.Windows.Controls.Grid avGrid = new System.Windows.Controls.Grid();
    ScenarioResult sr;
    UIObject uiApp;
    System.Windows.Controls.StackPanel avStackPanel = new System.Windows.Controls.StackPanel();
    string RoutingStrategy;


    protected override void InitTest(TParams p) {
        base.InitTest(p);
        UseMita = true;
        this.Text = "TunneledEventsTest";
        avTextbox.Name = "avTextbox";
        avTextbox.Background = System.Windows.Media.Brushes.Cyan;
        avStackPanel.Name = "avStackPanel";
        avStackPanel.Background = System.Windows.Media.Brushes.Cornsilk;
        host.BackColor = System.Drawing.Color.Red;
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("TunneledEventsTest"));
        this.Controls.Clear();
        host.Child = null;
        avStackPanel.Children.Clear();
        avGrid.Children.Clear();
        switch (scenario.Name)
        {
            case "Scenario1":
                avStackPanel.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(avControl_PreviewKeyDown);
                avStackPanel.Children.Add(avTextbox);
                host.Child = avStackPanel;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario2":
                avGrid.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(avControl_PreviewKeyDown);
                avGrid.Children.Add(avStackPanel);
                avStackPanel.Children.Add(avTextbox);
                host.Child = avGrid;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario3":
                avGrid.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(avControl_PreviewKeyDown);
                avGrid.Children.Add(avStackPanel);
                avStackPanel.Children.Add(avDockPanel);
                avDockPanel.Children.Add(avTextbox);
                host.Child = avGrid;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
        }
        return base.BeforeScenario(p, scenario);
    }

    void avControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        //assign RoutingStrategy to global string so a given scenario can examine it.
        RoutingStrategy = e.RoutedEvent.RoutingStrategy.ToString();
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
            uiavTextbox = uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            RoutingStrategy = "";
            //type a character in the AV textbox.  See that the top level StackPanel gets the tunneled event.
            uiavTextbox.SendKeys("X");
            Utilities.SleepDoEvents(10);
            sr.IncCounters(RoutingStrategy == System.Windows.RoutingStrategy.Tunnel.ToString(), "Tunneled event not seen for AV Textbox", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Verify that tunneled events work for Grid->StackPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario2(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            RoutingStrategy = "";
            //type a character in the AV textbox.  See that the top level Grid gets the tunneled event.
            uiavTextbox.SendKeys("X");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(RoutingStrategy == System.Windows.RoutingStrategy.Tunnel.ToString(), "Tunneled event not seen for AV Textbox", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Verify that tunneled events work for Grid->StackPanel->DockPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario3(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            RoutingStrategy = "";
            //type a character in the AV textbox.  See that the top level Grid gets the tunneled event.
            uiavTextbox.SendKeys("X");
            Utilities.SleepDoEvents(2);
            sr.IncCounters(RoutingStrategy == System.Windows.RoutingStrategy.Tunnel.ToString(), "Tunneled event not seen for AV Textbox", p.log);
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
//@ Verify that tunneled events work for StackPanel-&gt;TextBox inside EH boundary.
//@ Verify that tunneled events work for Grid-&gt;StackPanel-&gt;TextBox inside EH boundary.
//@ Verify that tunneled events work for Grid-&gt;StackPanel-&gt;DockPanel-&gt;TextBox inside EH boundary.
