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
using SWM = System.Windows.Media;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;


// Testcase:    DefaultPropertyValues
// Description: Verify Default Property values for Element Host.
public class DefaultPropertyValues : ReflectBase
{
    #region Test case setup

    ElementHost _elementHost1;

    public DefaultPropertyValues(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "DefaultPropertyValues Test";
        this.Size = new System.Drawing.Size(400, 400);
        base.InitTest(p);

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.BackColor = Color.Red;
        Controls.Add(_elementHost1);
    }

    #endregion

    #region Scenarios

    [Scenario("AutoSize=False")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(_elementHost1.AutoSize == false, "Failed at AutoSize=False", p.log);

        SWC.Button avButton1 = new SWC.Button();
        avButton1.Content = "Avalon Button";
        avButton1.Background = SWM.Brushes.White;
        _elementHost1.Child = avButton1;

        sr.IncCounters(_elementHost1.AutoSize == false, "Failed at AutoSize=False", p.log);

        return sr;
    }

    [Scenario("Dock=None")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(_elementHost1.Dock == SWF.DockStyle.None, "Failed at Dock=None", p.log);

        return sr;
    }

    [Scenario("Anchor=Top, Left")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(_elementHost1.Anchor == (SWF.AnchorStyles.Top | SWF.AnchorStyles.Left), 
            "Failed at Anchor=Top, Left. Actual: " + _elementHost1.Anchor.ToString(), p.log);

        return sr;
    }

    [Scenario("Top = Left = 0")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(_elementHost1.Top == 0 && _elementHost1.Left == 0, "Failed at Top = Left = 0", p.log);

        return sr;
    }

    [Scenario("Width = 200")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(200, _elementHost1.Width, "Failed at Width = 200", p.log);

        return sr;
    }

    [Scenario("Height = 100")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(100, _elementHost1.Height, "Failed at Height = 100", p.log);

        return sr;
    }

    [Scenario("Margin = {3,3,3,3}")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        sr.IncCounters(3, _elementHost1.Margin.Top, "Failed at Margin.Top", p.log);
        sr.IncCounters(3, _elementHost1.Margin.Left, "Failed at Margin.Left", p.log);
        sr.IncCounters(3, _elementHost1.Margin.Right, "Failed at Margin.Right", p.log);
        sr.IncCounters(3, _elementHost1.Margin.Bottom, "Failed at Margin.Bottom", p.log);
        sr.IncCounters(_elementHost1.Margin.All == 3, "Failed at Margin = {3,3,3,3}", p.log);

        return sr;
    }

    [Scenario("MinimumSize = 0,0")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(_elementHost1.MinimumSize == new System.Drawing.Size(0, 0),
            "Failed at MinimumSize = 0,0", p.log);

        return sr;
    }

    [Scenario("MaximumSize = 0,0")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(_elementHost1.MaximumSize == new System.Drawing.Size(0, 0),
            "Failed at MaximumSize = 0,0", p.log);

        return sr;
    }

    [Scenario("Visible = true")]
    public ScenarioResult Scenario10(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(_elementHost1.Visible == true, "Failed at Visible = true", p.log);

        return sr;
    }

    [Scenario("Enabled = true")]
    public ScenarioResult Scenario11(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(_elementHost1.Enabled == true, "Failed at Enabled = true", p.log);

        return sr;
    }

    #endregion
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ AutoSize=False
//@ Dock=None
//@ Anchor=Top, Left
//@ Top = Left = 0
//@ Width = 200  // same default size as a System.Windows.Forms.Panel
//@ Height = 200
//@ Margin = {3,3,3,3}  // standard default margin for WF controls
//@ MinimumSize = 0,0
//@ MaximumSize = 0,0 // interpreted by the  layout engine as unconstrained
//@ Visible  =  true
//@ Enabled  =  true
