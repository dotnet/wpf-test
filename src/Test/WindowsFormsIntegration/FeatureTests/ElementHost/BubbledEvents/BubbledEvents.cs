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
// Author:      a-larryl
//
public class BubbledEvents : ReflectBase {

    #region Testcase setup
    public BubbledEvents(string[] args) : base(args) { }

    ElementHost host = new ElementHost();
    System.Windows.Controls.TextBox avTextbox = new System.Windows.Controls.TextBox();
    System.Windows.Controls.DockPanel avDockPanel = new System.Windows.Controls.DockPanel();
    System.Windows.Controls.Grid avGrid = new System.Windows.Controls.Grid();
    System.Windows.Controls.Button avButton = new System.Windows.Controls.Button();
    ScenarioResult sr;
    UIObject uiApp;
    System.Windows.Controls.StackPanel avStackPanel = new System.Windows.Controls.StackPanel();
    string RoutingStrategy;
    string ExpectedRoutingStrategy = System.Windows.RoutingStrategy.Bubble.ToString();

    protected override void InitTest(TParams p) {
        base.InitTest(p);
        UseMita = true;
        this.Text = "BubbledEventsTest";
        avTextbox.Name = "avTextbox";
        avTextbox.Background = System.Windows.Media.Brushes.Cyan;
        avStackPanel.Name = "avStackPanel";
        avStackPanel.Background = System.Windows.Media.Brushes.Cornsilk;
        avGrid.Name = "avGrid";
        avGrid.Background = System.Windows.Media.Brushes.Aqua;
        host.BackColor = System.Drawing.Color.Red;
        avTextbox.KeyDown += new System.Windows.Input.KeyEventHandler(avControl_KeyDown);
        avStackPanel.KeyDown += new System.Windows.Input.KeyEventHandler(avControl_KeyDown);
        avGrid.KeyDown += new System.Windows.Input.KeyEventHandler(avControl_KeyDown);
        avButton.Name = "avButton";
        avButton.KeyDown += new System.Windows.Input.KeyEventHandler(avControl_KeyDown);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("BubbledEventsTest"));
        this.Controls.Clear();
        host.Child = null;
        avStackPanel.Children.Clear();
        avGrid.Children.Clear();
        avDockPanel.Children.Clear();
        RoutingStrategy = string.Empty;
        switch (scenario.Name)
        {
            case "Scenario1":
                avStackPanel.Children.Add(avTextbox);
                host.Child = avStackPanel;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario2":
                avGrid.Children.Add(avStackPanel);
                avStackPanel.Children.Add(avTextbox);
                host.Child = avGrid;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario3":
                avGrid.Children.Add(avStackPanel);
                avStackPanel.Children.Add(avDockPanel);
                avDockPanel.Children.Add(avTextbox);
                host.Child = avGrid;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
            case "Scenario4":
                avGrid.Children.Add(avStackPanel);
                avStackPanel.Children.Add(avDockPanel);
                avDockPanel.Children.Add(avButton);
                avButton.Content = avTextbox;
                host.Child = avGrid;
                this.Controls.Add(host);
                sr = new ScenarioResult();
                break;
        }
        return base.BeforeScenario(p, scenario);
    }

    void avControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        //assign RoutingStrategy to global string so a given scenario can examine it.
        //RoutingStrategy += ((System.Windows.FrameworkElement)sender).Name + ":" + e.RoutedEvent.RoutingStrategy.ToString() + "::";

        //Changed 4/27/06 per Nathan Enright to only look at the routing strategy itself:
        RoutingStrategy = e.RoutedEvent.RoutingStrategy.ToString();
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
            uiavTextbox = uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            //type a character in the AV textbox.  See that the top level StackPanel gets the bubbled event.
            uiavTextbox.SendKeys("x");
            Utilities.SleepDoEvents(10);
            sr.IncCounters(RoutingStrategy == ExpectedRoutingStrategy, "Bubbled event not seen for AV Textbox", p.log);
            if (RoutingStrategy != ExpectedRoutingStrategy)
            {
                p.log.WriteLine("Scenario 1 expected: " + ExpectedRoutingStrategy);
                p.log.WriteLine("Scenario 1 received: " + RoutingStrategy);
            }
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return sr;
    }

    [Scenario("Verify that bubbled events work for Grid->StackPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario2(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            //type a character in the AV textbox.  See that the top level Grid gets the bubbled event.
            uiavTextbox.SendKeys("x");
            Utilities.SleepDoEvents(10);
            sr.IncCounters(RoutingStrategy == ExpectedRoutingStrategy, "Bubbled event not seen for AV Textbox", p.log);
            if (RoutingStrategy != ExpectedRoutingStrategy)
            {
                p.log.WriteLine("Scenario 2 expected: " + ExpectedRoutingStrategy);
                p.log.WriteLine("Scenario 2 received: " + RoutingStrategy);
            }
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return sr;
    }

    [Scenario("Verify that bubbled events work for Grid->StackPanel->DockPanel->TextBox inside EH boundary.")]
    public ScenarioResult Scenario3(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            //type a character in the AV textbox.  See that the top level Grid gets the bubbled event.
            uiavTextbox.SendKeys("x");
            Utilities.SleepDoEvents(10);
            sr.IncCounters(RoutingStrategy == ExpectedRoutingStrategy, "Bubbled event not seen for AV Textbox", p.log);
            if (RoutingStrategy != ExpectedRoutingStrategy)
            {
                p.log.WriteLine("Scenario 3 expected: " + ExpectedRoutingStrategy);
                p.log.WriteLine("Scenario 3 received: " + RoutingStrategy);
            }
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return sr;
    }

    [Scenario("Verify that bubbled events work for Grid->StackPanel->DockPanel->Button->TextBox inside EH boundary.")]
    public ScenarioResult Scenario4(TParams p)
    {
        UIObject uiavTextbox = null;
        try
        {
            uiavTextbox = uiApp.Descendants.Find(UICondition.CreateFromId("avTextbox"));
            //type a character in the AV textbox.  See that the top level Grid gets the bubbled event.
            uiavTextbox.SendKeys("x");
            Utilities.SleepDoEvents(10);
            sr.IncCounters(RoutingStrategy == ExpectedRoutingStrategy, "Bubbled event not seen for AV Textbox", p.log);
            if (RoutingStrategy != ExpectedRoutingStrategy)
            {
                p.log.WriteLine("Scenario 4 expected: " + ExpectedRoutingStrategy);
                p.log.WriteLine("Scenario 4 received: " + RoutingStrategy);
            }
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return sr;
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