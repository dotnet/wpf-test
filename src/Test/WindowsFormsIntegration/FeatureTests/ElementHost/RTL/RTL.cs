// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
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
using System.Threading;
using System.Reflection;

//
// Testcase:    RTL
// Description: Verify Right-To-Left works for Element Host controls hosted on Winform.  
//              Also verify it works properly with FlowDirection.
//
public class RTL : ReflectBase
{
    #region Testcase setup

    ElementHost _elementHost1;
    ElementHost _elementHost2;
    SWC.TextBox _avTextBox1;
    SWC.Button _avButton;
    SWC.ListView _listView;
    SWC.GridView _gridView;
    List<Type> _panelTypes;

    public RTL(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "RTL";
        this.Size = new System.Drawing.Size(400, 400);

        _panelTypes = GetDerivedType("System.Windows.Forms.dll", typeof(SWF.Panel));

        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult sr)
    {
        this.Controls.Clear();
        base.AfterScenario(p, scenario, sr);
    }

    [Scenario("Element Host with single control.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        
        _avTextBox1 = new SWC.TextBox();
        _avButton = new SWC.Button();

        _avTextBox1.Text = "Avalon TextBox!";
        _avButton.Content = "Avalon Button!";

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Child = _avTextBox1;
        _elementHost1.Location = new System.Drawing.Point(100, 20);
        Controls.Add(_elementHost1);

        //Create Element Host 2
        _elementHost2 = new ElementHost();
        _elementHost2.Child = _avButton;
        _elementHost2.Location = new System.Drawing.Point(100, 200);
        Controls.Add(_elementHost2);

        Utilities.SleepDoEvents(10);

        // verify initial RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost2.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avButton.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        _elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except elementHost1 and avTextBox1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        _elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        _elementHost2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except elementHost2 and avButton
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost2.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avButton.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);

        _elementHost2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        return sr;
    }

    [Scenario("Element Host with complex control.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        _gridView = new SWC.GridView();

        SWC.GridViewColumn column1 = new SWC.GridViewColumn();
        column1.Header = "First Name";
        column1.Width = 100;
        SWC.GridViewColumn column2 = new SWC.GridViewColumn();
        column2.Header = "Last Name";
        column2.Width = 100;
        SWC.GridViewColumn column3 = new SWC.GridViewColumn();
        column3.Header = "Address";
        column3.Width = 150;

        _gridView.Columns.Add(column1);
        _gridView.Columns.Add(column2);
        _gridView.Columns.Add(column3);

        _listView = new SWC.ListView();
        _listView.View = _gridView;

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Child = _listView;
        _elementHost1.Width = 350;
        _elementHost1.Location = new System.Drawing.Point(25, 20);
        Controls.Add(_elementHost1);

        Utilities.SleepDoEvents(10);

        // verify initial RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_listView.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with complex control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        _elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - elementHost1 and  listView should be "Right to Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_listView.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with complex control.", p.log);

        _elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - only this should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(_listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        return sr;
    }

    [Scenario("Element Host with container control.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in _panelTypes)
        {
            int i = t.GetConstructors().Length;
            if (t == typeof(SWF.TabPage) || t.GetConstructors().Length == 0 || t.IsAbstract)
                continue;

            Object o;
            try
            {
                o = Activator.CreateInstance(t);
            }
            catch (MissingMethodException)
            {
                continue;
            }

            string containerType = "Container type: " + t.ToString();
            _avTextBox1 = new SWC.TextBox();
            _avTextBox1.Text = "Avalon TextBox!";
            _avTextBox1.TextWrapping = TextWrapping.Wrap;

            p.log.WriteLine(containerType);

            SWF.Panel panel = o as SWF.Panel;
            panel.BackColor = Color.Aqua;

            //Create Element Host 1
            _elementHost1 = new ElementHost();
            _elementHost1.Child = _avTextBox1;
            _elementHost1.Location = new System.Drawing.Point(25, 20);
            panel.Controls.Add(_elementHost1);

            Controls.Add(panel);

            Utilities.SleepDoEvents(10);

            // verify initial RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Right To Left"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with container control.", p.log);

            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            _elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right" except elementHost1 and avTextBox1
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with container control.", p.log);

            _elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Inherit;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            panel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Right to Left" except this
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with container control.", p.log);

            panel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Utilities.SleepDoEvents(10);

            // verify RTL states - only this should be "Right To Left"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);
        }
        return sr;
    }

    [Scenario("EH control FlowDirection/RTL transitions (yes/no, no/yes).")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        _avTextBox1 = new SWC.TextBox();
        _avTextBox1.Text = "Avalon TextBox!";

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Child = _avTextBox1;
        _elementHost1.Location = new System.Drawing.Point(100, 20);
        Controls.Add(_elementHost1);

        Utilities.SleepDoEvents(10);

        // verify initial RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        
        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);
        
        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        
        _elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except elementHost1 and avTextBox1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);

        _avTextBox1.FlowDirection = FlowDirection.LeftToRight;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except elementHost1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        _elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        _avTextBox1.FlowDirection = FlowDirection.RightToLeft;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except avTextBox1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);

        _avTextBox1.FlowDirection = FlowDirection.LeftToRight;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - only this should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        _elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Inherit;
        Utilities.SleepDoEvents(10);
        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Right To Left" except avTextBox1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(_avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        
        return sr;
    }

    #endregion

    #region Utilities

    List<Type> GetDerivedType(string assembly, Type requestedType)
    {
        List<Type> list = new List<Type>();

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (a.FullName.Contains("System.Windows.Forms") == false)
                continue;

            foreach (Type t in a.GetTypes())
            {
                if (!t.IsClass || !t.IsPublic) continue;
                Boolean derivedFromRequestedType = false;
                Type baseType = t.BaseType;
                while ((baseType != null) && !derivedFromRequestedType)
                {
                    derivedFromRequestedType = (baseType == requestedType);
                    baseType = baseType.BaseType;
                }
                if (!derivedFromRequestedType) continue;
                list.Add(t);
            }
        }
        return list;
    }

    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Element Host with single control.
//@ Element Host with complex control.
//@ Element Host with container control.
//@ EH control FlowDirection/RTL transitions (yes/no, no/yes).