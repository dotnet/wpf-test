// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Reflection;
using SWC=System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using SWF=System.Windows.Forms;
using System.Windows.Controls;


/// <TestCase>
/// Visible
/// </TestCase>
/// <summary>
/// Verify that the WF's Visible Property and WPF's IsVisible Property work properly
/// </summary>
public class Visible : ReflectBase {

    private SWC.Button _ehBtn;
    private ElementHost _eh1;
    
    #region Testcase setup
    public Visible(string[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Size = new System.Drawing.Size(500, 500);
        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        this.Text = "";
        base.AfterScenario(p, scenario, result);
    }

    [Scenario("@ EH with single control.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        try
        {
            _ehBtn = new SWC.Button();
            _ehBtn.Content = "I am a Button";
            //Add the ElementHost control
            _eh1 = new ElementHost();
            _eh1.Child = _ehBtn;
            _eh1.Size = new System.Drawing.Size(200, 200);
            _eh1.BackColor = System.Drawing.Color.Crimson;
            _eh1.Child = _ehBtn;
            this.Controls.Add(_eh1);

            bool originallyVisible = _eh1.Visible;
            bool newVisible = !originallyVisible;

            _eh1.Visible = newVisible;
            this.Text = newVisible.ToString();
            Utilities.SleepDoEvents(5, 100);
            if (_eh1.Visible == originallyVisible)
            {
                sr.IncCounters(false, String.Format("Visible property did not change. Expected: {0}, Actual: {1} ", newVisible, _eh1.Visible.ToString()), p.log);
            }
            else
            {
                //Reset it back again.
                _eh1.Visible = originallyVisible;
                this.Text = originallyVisible.ToString();
                Utilities.SleepDoEvents(5, 100);
                SWF.Application.DoEvents();
                if (_eh1.Visible != originallyVisible)
                    sr.IncCounters(false, String.Format("Allow drop property did not change. Expected: {0}, Actual: {1} ", originallyVisible, _eh1.Cursor.ToString()), p.log);

                sr.IncCounters(true);
            }
        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        Utilities.SleepDoEvents(1, 1000);
        return sr;
    }
    
    [Scenario("EH with composite control")]
    public ScenarioResult Scenario2(TParams p) {

        ScenarioResult sr = new ScenarioResult();
        try
        {
            StackPanel stkPanel = new StackPanel();
           
            System.Windows.Controls.Button btn = new SWC.Button();
            btn.Content = "I am a Button";

            SWC.ListBox list  = new SWC.ListBox();
            list.Items.Add("One");
            list.Items.Add("Two");
            
            SWC.Label lbl = new SWC.Label();
            lbl.Content = "I am a Label";

            stkPanel.Children.Add(btn);
            stkPanel.Children.Add(list);
            stkPanel.Children.Add(lbl);

            //Add the ElementHost control
            _eh1 = new ElementHost();
            _eh1.Child = stkPanel;
            _eh1.Size = new System.Drawing.Size(200, 200);
            _eh1.BackColor = System.Drawing.Color.Crimson;
            this.Controls.Add(_eh1);

            bool originallyVisible = _eh1.Visible;
            bool newVisible = !originallyVisible;

            _eh1.Visible = newVisible;
            this.Text = _eh1.Visible.ToString(); 
            Utilities.SleepDoEvents(5, 100);
            if (_eh1.Visible == originallyVisible)
            {
                sr.IncCounters(false, String.Format("Visible property did not change. Expected: {0}, Actual: {1} ", newVisible, _eh1.Visible.ToString()), p.log);
            }
            else
            {
                //Reset it back again.
                _eh1.Visible = originallyVisible;
                this.Text = _eh1.Visible.ToString();
                Utilities.SleepDoEvents(5, 100);
                if (_eh1.Visible != originallyVisible)
                    sr.IncCounters(false, String.Format("Allow drop property did not change. Expected: {0}, Actual: {1} ", originallyVisible, _eh1.Cursor.ToString()), p.log);

                sr.IncCounters(true);
            }
        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        return sr;
    }

    [Scenario("EH control Visible transitions (true -> true, true->false, false ->true, false, false)")]
    public ScenarioResult Scenario3(TParams p) {

        ScenarioResult sr = new ScenarioResult();
        try
        {
            _ehBtn = new SWC.Button();
            _ehBtn.Content = "I am a Button";
            //Add the ElementHost control
            _eh1 = new ElementHost();
            _eh1.Size = new System.Drawing.Size(200, 200);
            _eh1.BackColor = System.Drawing.Color.Cyan;
            _eh1.Child = _ehBtn;
            this.Controls.Add(_eh1);
            
            _eh1.Visible = true;
            this.Text = _eh1.Visible.ToString();
            Utilities.SleepDoEvents(1, 500);
            sr.IncCounters(_eh1.Visible == true, String.Format("Visible property did not change. Expected: {0}, Actual: {1} ", true.ToString(), _eh1.Visible.ToString()), p.log);
           
            _eh1.Visible = false;
            this.Text = _eh1.Visible.ToString();
            Utilities.SleepDoEvents(1, 500);
            sr.IncCounters(_eh1.Visible == false, String.Format("Visible property did not change. Expected: {0}, Actual: {1} ", false.ToString(), _eh1.Visible.ToString(), p.log), p.log);
            
            _eh1.Visible = true;
            this.Text = _eh1.Visible.ToString();
            Utilities.SleepDoEvents(1, 500);
            sr.IncCounters(_eh1.Visible == true, String.Format("Visible property did not change. Expected: {0}, Actual: {1} ", true.ToString(), _eh1.Visible.ToString(), p.log), p.log);

            _eh1.Visible = false;
            this.Text = _eh1.Visible.ToString();
            Utilities.SleepDoEvents(1, 500);
            sr.IncCounters(_eh1.Visible == false, String.Format("Visible property did not change. Expected: {0}, Actual: {1} ", false.ToString(), _eh1.Visible.ToString(), p.log), p.log);
        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        return sr;
    }

    #endregion
    //==========================================
    // Scenarios
    //==========================================   
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ EH with single control

//@ EH with composite control

//@ EH control Visible transitions (true -> true, true->false, false ->true, false, false)
