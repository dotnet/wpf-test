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
using System.Threading;
using System.Reflection;


// Testcase:    PaddingsAndMargins
// Description: Verify that setting the padding and margins on a EH flow to hosted controls.
public class PaddingsAndMargins : ReflectBase
{
    #region Testcase setup

    ElementHost _elementHost1;
    SWC.Button _avButton;
    SWC.Button _avButton2 = new SWC.Button();

    public PaddingsAndMargins(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "PaddingsAndMargins";
        this.Size = new System.Drawing.Size(300, 300);
        _avButton2.Content = "Padding Reference";
        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        _avButton = new SWC.Button();
        _avButton.Background = System.Windows.Media.Brushes.White;
        _avButton.Content = "Button";

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Child = _avButton;
        _elementHost1.Location = new System.Drawing.Point(0, 0);
        _elementHost1.BackColor = Color.White;
        Controls.Add(_elementHost1);
        SWF.Application.DoEvents();

        if (scenario.Name == "Scenario6" || scenario.Name == "Scenario7" || scenario.Name == "Scenario8"
            || scenario.Name == "Scenario9" || scenario.Name == "Scenario10")
        {
            Controls.Remove(_elementHost1);

            SWF.FlowLayoutPanel flowLayoutPanel = new SWF.FlowLayoutPanel();
            flowLayoutPanel.Size = new System.Drawing.Size(300, 150);
            flowLayoutPanel.Controls.Add(_elementHost1);
            Controls.Add(flowLayoutPanel);

            Utilities.SleepDoEvents(10);
        }

        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        base.AfterScenario(p, scenario, result);
    }

    [Scenario("Set the top padding on the EH and verify that the Avalon Control honors the setting.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(10);

        //Check initial padding properties
        sr.IncCounters(_elementHost1.Padding.Top == this.Padding.Top,
            "Failed at elementHost1.Padding.Top.  Expected: " + this.Padding.Top + ".  Actual: " +
            _elementHost1.Padding.Top, p.log);
        sr.IncCounters(_avButton.Padding.Top == _avButton2.Padding.Top,
            "Failed at avButton.Padding.Top.  Expected: " + _avButton2.Padding.Top + ".  Actual: " +
            _avButton.Padding.Top, p.log);

        _elementHost1.Padding = new SWF.Padding(0, 50, 0, 0);
        Utilities.SleepDoEvents(10);

        //Check padding properties
        sr.IncCounters(_elementHost1.Padding.Top == 50, 
            "Failed at elementHost1.Padding.Top.  Expected: 50.  Actual: " +
            _elementHost1.Padding.Top, p.log);
        sr.IncCounters(_avButton.Padding.Top == _avButton2.Padding.Top,
            "Failed at avButton.Padding.Top.  Expected: " + _avButton2.Padding.Top + ".  Actual: " + 
            _avButton.Padding.Top, p.log);

        return sr;
    }

    [Scenario("Set the bottom padding on the EH and verify that the Avalon Control honors the setting.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial padding properties
        sr.IncCounters(_elementHost1.Padding.Bottom == this.Padding.Bottom,
            "Failed at elementHost1.Padding.Bottom.  Expected: " + this.Padding.Bottom + ".  Actual: " +
            _elementHost1.Padding.Bottom, p.log);
        sr.IncCounters(_avButton.Padding.Bottom == _avButton2.Padding.Bottom,
            "Failed at avButton.Padding.Bottom.  Expected: " + _avButton2.Padding.Bottom + ".  Actual: " +
            _avButton.Padding.Bottom, p.log);

        _elementHost1.Padding = new SWF.Padding(0, 0, 0, 50);
        Utilities.SleepDoEvents(10);

        //Check padding properties
        sr.IncCounters(_elementHost1.Padding.Bottom == 50,
            "Failed at elementHost1.Padding.Bottom.  Expected: 50.  Actual: " +
            _elementHost1.Padding.Bottom, p.log);
        sr.IncCounters(_avButton.Padding.Bottom == _avButton2.Padding.Bottom,
            "Failed at avButton.Padding.Bottom.  Expected: " + _avButton2.Padding.Bottom + ".  Actual: " +
            _avButton.Padding.Bottom, p.log);

        return sr;
    }

    [Scenario("Set the left padding on the EH and verify that the Avalon Control honors the setting.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial padding properties
        sr.IncCounters(_elementHost1.Padding.Left == this.Padding.Left,
            "Failed at elementHost1.Padding.Left.  Expected: " + this.Padding.Left + ".  Actual: " +
            _elementHost1.Padding.Left, p.log);
        sr.IncCounters(_avButton.Padding.Left == _avButton2.Padding.Left,
            "Failed at avButton.Padding.Left.  Expected: " + _avButton2.Padding.Left + ".  Actual: " +
            _avButton.Padding.Left, p.log);

        _elementHost1.Padding = new SWF.Padding(50, 0, 0, 0);
        Utilities.SleepDoEvents(10);

        //Check padding properties
        sr.IncCounters(_elementHost1.Padding.Left == 50,
            "Failed at elementHost1.Padding.Left.  Expected: 50.  Actual: " +
            _elementHost1.Padding.Left, p.log);
        sr.IncCounters(_avButton.Padding.Left == _avButton2.Padding.Left,
            "Failed at avButton.Padding.Left.  Expected: " + _avButton2.Padding.Left + ".  Actual: " +
            _avButton.Padding.Left, p.log);

        return sr;
    }

    [Scenario("Set the right padding on the EH and verify that the Avalon Control honors the setting.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial padding properties
        sr.IncCounters(_elementHost1.Padding.Right == this.Padding.Right,
            "Failed at elementHost1.Padding.Right.  Expected: " + this.Padding.Right + ".  Actual: " +
            _elementHost1.Padding.Right, p.log);
        sr.IncCounters(_avButton.Padding.Right == _avButton2.Padding.Right,
            "Failed at avButton.Padding.Right.  Expected: " + _avButton2.Padding.Right + ".  Actual: " +
            _avButton.Padding.Right, p.log);

        _elementHost1.Padding = new SWF.Padding(0, 0, 50, 0);
        Utilities.SleepDoEvents(10);

        //Check padding properties
        sr.IncCounters(_elementHost1.Padding.Right == 50,
            "Failed at elementHost1.Padding.Right.  Expected: 50.  Actual: " +
            _elementHost1.Padding.Right, p.log);
        sr.IncCounters(_avButton.Padding.Right == _avButton2.Padding.Right,
            "Failed at avButton.Padding.Right.  Expected: " + _avButton2.Padding.Right + ".  Actual: " +
            _avButton.Padding.Right, p.log);

        return sr;
    }

    [Scenario("Set the padding(All) on the EH and verify that the Avalon Control honors the setting.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial padding properties
        sr.IncCounters(_elementHost1.Padding.All == 0,
            "Failed at elementHost1.Padding.All.  Expected: " + this.Padding.Top + ".  Actual: " +
            _elementHost1.Padding.All, p.log);
        sr.IncCounters(_avButton.Padding.Top == _avButton2.Padding.Top && 
            _avButton.Padding.Bottom == _avButton2.Padding.Bottom &&
            _avButton.Padding.Left == _avButton2.Padding.Left &&
            _avButton.Padding.Right == _avButton2.Padding.Right,
            "Failed at avButton.Padding.All.  Expected: " + _avButton2.Padding.ToString() + ".  Actual: " +
            _avButton.Padding.ToString(), p.log);

        _elementHost1.Padding = new SWF.Padding(10);
        Utilities.SleepDoEvents(10);

        //Check padding properties
        sr.IncCounters(_elementHost1.Padding.All == 10,
            "Failed at elementHost1.Padding.All.  Expected: 10.  Actual: " +
            _elementHost1.Padding.All, p.log);
        sr.IncCounters(_avButton.Padding.Top == _avButton2.Padding.Top &&
            _avButton.Padding.Bottom == _avButton2.Padding.Bottom &&
            _avButton.Padding.Left == _avButton2.Padding.Left &&
            _avButton.Padding.Right == _avButton2.Padding.Right,
            "Failed at avButton.Padding.All.  Expected: " + _avButton2.Padding.ToString() + ".  Actual: " +
            _avButton.Padding.ToString(), p.log);

        return sr;
    }

    [Scenario("Host EH inside a FLP, Set the top margin on the EH and verify that the EH is positioned correctly.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial margin properties
        sr.IncCounters(_elementHost1.Margin.Top == 3,
            "Failed at elementHost1.Margin.Top.  Expected: 3.  Actual: " +
            _elementHost1.Margin.Top, p.log);
        sr.IncCounters(_avButton.Margin.Top == 0,
            "Failed at avButton.Margin.Top.  Expected: 0.  Actual: " +
            _avButton.Margin.Top, p.log);

        _elementHost1.Margin = new SWF.Padding(0, 50, 0, 0);
        
        Utilities.SleepDoEvents(10);

        //Check margin properties
        sr.IncCounters(_elementHost1.Margin.Top == 50,
            "Failed at elementHost1.Margin.Top.  Expected: 50.  Actual: " +
            _elementHost1.Margin.Top, p.log);
        sr.IncCounters(_avButton.Margin.Top == 0,
            "Failed at avButton.Margin.Top.  Expected: 0.  Actual: " +
            _avButton.Margin.Top, p.log);

        //Check location properties
        sr.IncCounters(_elementHost1.Left == 0 && _elementHost1.Top == 50,
            "Failed at elementHost1.Margin.Top.  Expected: 50.  Actual: " +
            _elementHost1.Top, p.log);

        return sr;
    }

    [Scenario("Host EH inside a FLP, Set the bottom margin on the EH and and verify that the EH is positioned correctly.")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial margin properties
        sr.IncCounters(_elementHost1.Margin.Bottom == 3,
            "Failed at elementHost1.Margin.Bottom.  Expected: 3.  Actual: " +
            _elementHost1.Margin.Top, p.log);
        sr.IncCounters(_avButton.Margin.Bottom == 0,
            "Failed at avButton.Margin.Bottom.  Expected: 0.  Actual: " +
            _avButton.Margin.Bottom, p.log);

        _elementHost1.Margin = new SWF.Padding(0, 0, 0, 50);

        Utilities.SleepDoEvents(10);

        //Check margin properties
        sr.IncCounters(_elementHost1.Margin.Bottom == 50,
            "Failed at elementHost1.Margin.Bottom.  Expected: 50.  Actual: " +
            _elementHost1.Margin.Bottom, p.log);
        sr.IncCounters(_avButton.Margin.Bottom == 0,
            "Failed at avButton.Margin.Bottom.  Expected: 0.  Actual: " +
            _avButton.Margin.Bottom, p.log);

        //Check location properties
        sr.IncCounters(_elementHost1.Left == 0 && _elementHost1.Top == 0,
            "Failed at elementHost1.Top.  Expected: 0.  Actual: " +
            _elementHost1.Top, p.log);

        return sr;
    }

    [Scenario("Host EH inside a FLP, Set the left margin on the EH and and verify that the EH is positioned correctly.")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial margin properties
        sr.IncCounters(_elementHost1.Margin.Left == 3,
            "Failed at elementHost1.Margin.Left.  Expected: 3.  Actual: " +
            _elementHost1.Margin.Left, p.log);
        sr.IncCounters(_avButton.Margin.Left == 0,
            "Failed at avButton.Margin.Left.  Expected: 0.  Actual: " +
            _avButton.Margin.Left, p.log);

        _elementHost1.Margin = new SWF.Padding(50, 0, 0, 0);

        Utilities.SleepDoEvents(10);

        //Check margin properties
        sr.IncCounters(_elementHost1.Margin.Left == 50,
            "Failed at elementHost1.Margin.Left.  Expected: 50.  Actual: " +
            _elementHost1.Margin.Left, p.log);
        sr.IncCounters(_avButton.Margin.Left == 0,
            "Failed at avButton.Margin.Left.  Expected: 0.  Actual: " +
            _avButton.Margin.Left, p.log);

        //Check location properties
        sr.IncCounters(_elementHost1.Left == 50 && _elementHost1.Top == 0,
            "Failed at elementHost1.Left.  Expected: 50.  Actual: " +
            _elementHost1.Left, p.log);

        return sr;
    }

    [Scenario("Host EH inside a FLP, Set the right margin on the EH and and verify that the EH is positioned correctly.")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial margin properties
        sr.IncCounters(_elementHost1.Margin.Right == 3,
            "Failed at elementHost1.Margin.Right.  Expected: 3.  Actual: " +
            _elementHost1.Margin.Right, p.log);
        sr.IncCounters(_avButton.Margin.Right == 0,
            "Failed at avButton.Margin.Right.  Expected: 0.  Actual: " +
            _avButton.Margin.Right, p.log);

        _elementHost1.Margin = new SWF.Padding(0, 0, 50, 0);

        Utilities.SleepDoEvents(10);

        //Check margin properties
        sr.IncCounters(_elementHost1.Margin.Right == 50,
            "Failed at elementHost1.Margin.Right.  Expected: 50.  Actual: " +
            _elementHost1.Margin.Right, p.log);
        sr.IncCounters(_avButton.Margin.Right == 0,
            "Failed at avButton.Margin.Right.  Expected: 0.  Actual: " +
            _avButton.Margin.Right, p.log);

        //Check location properties
        sr.IncCounters(_elementHost1.Left == 0 && _elementHost1.Top == 0,
            "Failed at elementHost1.Left.  Expected: 0.  Actual: " +
            _elementHost1.Left, p.log);

        return sr;
    }

    [Scenario("Host EH inside a FLP, Set the margin(All) on the EH and and verify that the EH is positioned correctly.")]
    public ScenarioResult Scenario10(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial margin properties
        sr.IncCounters(_elementHost1.Margin.All == 3,
            "Failed at elementHost1.Margin.All.  Expected: 3.  Actual: " +
            _elementHost1.Margin.All, p.log);
        sr.IncCounters(_avButton.Margin.Top == 0 && _avButton.Margin.Bottom == 0 &&
            _avButton.Margin.Left == 0 && _avButton.Margin.Right == 0,
            "Failed at avButton.Margin.All.  Expected: 0.  Actual: " +
            _avButton.Padding.Top, p.log);

        _elementHost1.Margin = new SWF.Padding(50);
        Utilities.SleepDoEvents(10);

        //Check padding properties
        sr.IncCounters(_elementHost1.Margin.All == 50,
            "Failed at elementHost1.Margin.All.  Expected: 10.  Actual: " +
            _elementHost1.Margin.All, p.log);
        sr.IncCounters(_avButton.Margin.Top == 0 && _avButton.Margin.Bottom == 0 &&
            _avButton.Margin.Left == 0 && _avButton.Margin.Right == 0,
            "Failed at avButton.Margin.All.  Expected: 0.  Actual: " +
            _avButton.Padding.Top, p.log);

        //Check location properties
        sr.IncCounters(_elementHost1.Left == 50 && _elementHost1.Top == 50,
            "Failed at elementHost1.Margin.Top.  Expected: 50.  Actual: " +
            _elementHost1.Top, p.log);

        return sr;
    }

    #endregion
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Set the top padding on the EH and verify that the Avalon Control honors the setting.
//@ Set the bottom padding on the EH and verify that the Avalon Control honors the setting.
//@ Set the left padding on the EH and verify that the Avalon Control honors the setting.
//@ Set the right padding on the EH and verify that the Avalon Control honors the setting.
//@ Set the padding(All) on the EH and verify that the Avalon Control honors the setting.
//@ Host EH inside a FLP, Set the top margin on the EH and verify that the EH is positioned correctly.
//@ Host EH inside a FLP, Set the bottom margin on the EH and and verify that the EH is positioned correctly.
//@ Host EH inside a FLP, Set the left margin on the EH and and verify that the EH is positioned correctly.
//@ Host EH inside a FLP, Set the right margin on the EH and and verify that the EH is positioned correctly.
//@ Host EH inside a FLP, Set the margin(All) on the EH and and verify that the EH is positioned correctly.
