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
// Author:      a-rickyt
//
public class RTL : ReflectBase
{
    #region Testcase setup

    ElementHost elementHost1;
    ElementHost elementHost2;
    SWC.TextBox avTextBox1;
    SWC.Button avButton;
    SWC.ListView listView;
    SWC.GridView gridView;
    List<Type> panelTypes;


    public RTL(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "RTL";
        this.Size = new System.Drawing.Size(400, 400);

        panelTypes = GetDerivedType("System.Windows.Forms.dll", typeof(SWF.Panel));

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
        
        avTextBox1 = new SWC.TextBox();
        avButton = new SWC.Button();

        avTextBox1.Text = "Avalon TextBox!";
        avButton.Content = "Avalon Button!";

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Child = avTextBox1;
        elementHost1.Location = new System.Drawing.Point(100, 20);
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.Child = avButton;
        elementHost2.Location = new System.Drawing.Point(100, 200);
        Controls.Add(elementHost2);

        Utilities.SleepDoEvents(10);

        // verify initial RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost2.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avButton.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except elementHost1 and avTextBox1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        elementHost2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except elementHost2 and avButton
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost2.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avButton.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);

        elementHost2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost2.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avButton.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        return sr;
    }

    [Scenario("Element Host with complex control.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        gridView = new SWC.GridView();

        SWC.GridViewColumn column1 = new SWC.GridViewColumn();
        column1.Header = "First Name";
        column1.Width = 100;
        SWC.GridViewColumn column2 = new SWC.GridViewColumn();
        column2.Header = "Last Name";
        column2.Width = 100;
        SWC.GridViewColumn column3 = new SWC.GridViewColumn();
        column3.Header = "Address";
        column3.Width = 150;

        gridView.Columns.Add(column1);
        gridView.Columns.Add(column2);
        gridView.Columns.Add(column3);

        listView = new SWC.ListView();
        listView.View = gridView;

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Child = listView;
        elementHost1.Width = 350;
        elementHost1.Location = new System.Drawing.Point(25, 20);
        Controls.Add(elementHost1);

        Utilities.SleepDoEvents(10);

        // verify initial RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(listView.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with complex control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - elementHost1 and  listView should be "Right to Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(listView.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with complex control.", p.log);

        elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - only this should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with complex control.", p.log);
        sr.IncCounters(listView.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with complex control.", p.log);

        return sr;
    }

    [Scenario("Element Host with container control.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in panelTypes)
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
            avTextBox1 = new SWC.TextBox();
            avTextBox1.Text = "Avalon TextBox!";
            avTextBox1.TextWrapping = TextWrapping.Wrap;

            p.log.WriteLine(containerType);

            SWF.Panel panel = o as SWF.Panel;
            panel.BackColor = Color.Aqua;

            //Create Element Host 1
            elementHost1 = new ElementHost();
            elementHost1.Child = avTextBox1;
            elementHost1.Location = new System.Drawing.Point(25, 20);
            panel.Controls.Add(elementHost1);

            Controls.Add(panel);

            Utilities.SleepDoEvents(10);

            // verify initial RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Right To Left"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with container control.", p.log);

            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right" except elementHost1 and avTextBox1
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with container control.", p.log);

            elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Inherit;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            panel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Right to Left" except this
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with container control.", p.log);

            panel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            Utilities.SleepDoEvents(10);

            // verify RTL states - only this should be "Right To Left"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);

            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            Utilities.SleepDoEvents(10);

            // verify RTL states - all should be "Left To Right"
            sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(panel.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with container control.", p.log);
            sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with container control.", p.log);
        }
        return sr;
    }

    [Scenario("EH control FlowDirection/RTL transitions (yes/no, no/yes).")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avTextBox1 = new SWC.TextBox();
        avTextBox1.Text = "Avalon TextBox!";

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Child = avTextBox1;
        elementHost1.Location = new System.Drawing.Point(100, 20);
        Controls.Add(elementHost1);

        Utilities.SleepDoEvents(10);

        // verify initial RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        
        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);
        
        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        
        elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except elementHost1 and avTextBox1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);

        avTextBox1.FlowDirection = FlowDirection.LeftToRight;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except elementHost1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        avTextBox1.FlowDirection = FlowDirection.RightToLeft;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right" except avTextBox1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.RightToLeft, "Failed at Element Host with single control.", p.log);

        avTextBox1.FlowDirection = FlowDirection.LeftToRight;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - only this should be "Right To Left"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        elementHost1.RightToLeft = System.Windows.Forms.RightToLeft.Inherit;
        Utilities.SleepDoEvents(10);
        this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Right To Left" except avTextBox1
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.Yes, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);

        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        Utilities.SleepDoEvents(10);

        // verify RTL states - all should be "Left To Right"
        sr.IncCounters(this.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(elementHost1.RightToLeft == SWF.RightToLeft.No, "Failed at Element Host with single control.", p.log);
        sr.IncCounters(avTextBox1.FlowDirection == FlowDirection.LeftToRight, "Failed at Element Host with single control.", p.log);
        
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