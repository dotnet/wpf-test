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

//
// Testcase:    PositionAndSize
// Description: Verify that sizing for static control in a static container is correct.
//
public class PositionAndSize : ReflectBase
{
    #region Test case setup

    ElementHost _elementHost1;
    ElementHost _elementHost2;
    SWC.Button _avButton;
    SWC.ComboBox _avComboBox;

    public PositionAndSize(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "Position And Size Test";
        this.Size = new System.Drawing.Size(400, 400);
        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        _avButton = new SWC.Button();
        _avButton.Content = "AV Button";
        _avButton.Background = System.Windows.Media.Brushes.White;

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Child = _avButton;
        _elementHost1.BackColor = Color.White;
        _elementHost1.Location = new System.Drawing.Point(150, 50);
        Controls.Add(_elementHost1);
        SWF.Application.DoEvents();

        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        base.AfterScenario(p, scenario, result);
    }

    [Scenario("Element Host and hosted control should be sized to default 200, 100.")]
    public ScenarioResult Scenario1(TParams p)
    {
        _avComboBox = new SWC.ComboBox();
        _avComboBox.Text = "AV ComboBox";

        //Create Element Host 2
        _elementHost2 = new ElementHost();
        _elementHost2.Child = _avComboBox;
        _elementHost2.Location = new System.Drawing.Point(150, 200);
        Controls.Add(_elementHost2);
        
        ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(20);

        //Check default size properties
        sr.IncCounters(_elementHost1.Width == 200, "Failed at default width 200.", p.log);
        sr.IncCounters(_elementHost1.Height == 100, "Failed at default height 100.", p.log);

        //Check second element host default size properties
        sr.IncCounters(_elementHost2.Width == 200, "Failed at default width 200.", p.log);
        sr.IncCounters(_elementHost2.Height == 100, "Failed at default height 100.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 200, 100, Color.White) >= 1,
            "Bitmap Failed at default size 200, 100. Percent match: " +
            BitmapsColorPercent(bmp, 150, 50, 200, 100, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Change Bounds. Expected: Resize/Reposition hosted control.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        _elementHost1.Bounds = new Rectangle(10, 10, 100, 100);
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 100, "Failed at width 100.", p.log);
        sr.IncCounters(_elementHost1.Height == 100, "Failed at height 100.", p.log);
        sr.IncCounters(_elementHost1.Left == 10, "Left Failed at Change Bounds.", p.log);
        sr.IncCounters(_elementHost1.Top == 10, "Top Failed at Change Bounds.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 10, 10, 100, 100, Color.White) >= 1,
            "Bitmap Failed at Change Bounds. Percent match: " +
            BitmapsColorPercent(bmp, 10, 10, 100, 100, Color.White) + "%", p.log);

        //Set bounds smaller than MinimumSize
        _elementHost1.MinimumSize = new System.Drawing.Size(50, 50);
        _elementHost1.Bounds = new Rectangle(10, 10, 20, 20);
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 50, "Width Failed at Change Bounds at MinimumSize 50,50.", p.log);
        sr.IncCounters(_elementHost1.Height == 50, "Height Failed at Change Bounds at MinimumSize 50,50.", p.log);
        sr.IncCounters(_elementHost1.Left == 10, "Left Failed at Change Bounds at MinimumSize 50,50.", p.log);
        sr.IncCounters(_elementHost1.Top == 10, "Top Failed at Change Bounds at MinimumSize 50,50.", p.log);

        //Set bounds larger than MaximumSize
        _elementHost1.MaximumSize = new System.Drawing.Size(210, 210);
        _elementHost1.Bounds = new Rectangle(10, 10, 260, 260);
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 210, "Width Failed at MaximumSize 210,210.", p.log);
        sr.IncCounters(_elementHost1.Height == 210, "Height Failed at MaximumSize 210,210.", p.log);
        sr.IncCounters(_elementHost1.Left == 10, "Left Failed at Change Bounds at MaximumSize 210,210.", p.log);
        sr.IncCounters(_elementHost1.Top == 10, "Top Failed at Change Bounds at MaximumSize 210,210.", p.log);

        return sr;
    }

    [Scenario("Change Top. Expected: Reposition hosted control.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Top = 5;
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 200, "Width Failed at Change Top.", p.log);
        sr.IncCounters(_elementHost1.Height == 100, "Height Failed at Change Top.", p.log);
        sr.IncCounters(_elementHost1.Left == 150, "Left Failed at Change Top.", p.log);
        sr.IncCounters(_elementHost1.Top == 5, "Top Failed at Change Top.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 5, 200, 100, Color.White) >= 1,
            "Bitmap Failed at Change Top. Percent match: " + 
            BitmapsColorPercent(bmp, 150, 5, 200, 100, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Change Left.  Expected: Reposition hosted control.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Left = 5;
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 200, "Width Failed at Change Left.", p.log);
        sr.IncCounters(_elementHost1.Height == 100, "Height Failed at Change Left.", p.log);
        sr.IncCounters(_elementHost1.Left == 5, "Left Failed at Change Left.", p.log);
        sr.IncCounters(_elementHost1.Top == 50, "Top Failed at Change Left.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 5, 50, 200, 100, Color.White) >= 1,
            "Bitmap Failed at Change Left. Percent match: " +
            BitmapsColorPercent(bmp, 5, 50, 200, 100, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Change Location. Expected: Reposition hosted control.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Location = new System.Drawing.Point(180, 250);
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 200, "Width Failed at Change Location.", p.log);
        sr.IncCounters(_elementHost1.Height == 100, "Height Failed at Change Location.", p.log);
        sr.IncCounters(_elementHost1.Left == 180, "Left Failed at Change Bounds at Change Location.", p.log);
        sr.IncCounters(_elementHost1.Top == 250, "Top Failed at Change Bounds at Change Location.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 180, 250, 200, 100, Color.White) >= 1,
            "Bitmap Failed at Change Location. Percent match: " +
            BitmapsColorPercent(bmp, 180, 250, 200, 100, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Change Size. Expected: Resize hosted control.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Size = new System.Drawing.Size(230, 230);
        Utilities.SleepDoEvents(20);
 
        //Check size properties
        sr.IncCounters(_elementHost1.Width == 230, "Width Failed at width 230.", p.log);
        sr.IncCounters(_elementHost1.Height == 230, "Height Failed at height 230.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 230, 230, Color.White) >= 1,
            "Bitmap Failed at Change Size. Percent match: " +
            BitmapsColorPercent(bmp, 150, 50, 230, 230, Color.White) + "%", p.log);

        //Set size smaller than MinimumSize
        _elementHost1.MinimumSize = new System.Drawing.Size(50, 50);
        _elementHost1.Size = new System.Drawing.Size(30, 30);
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(_elementHost1.Width == 50, "Width Failed at MinimumSize 50,50.", p.log);
        sr.IncCounters(_elementHost1.Height == 50, "Height Failed at MinimumSize 50,50.", p.log);

        //Set size larger than MaximumSize
        _elementHost1.MaximumSize = new System.Drawing.Size(100, 100);
        _elementHost1.Size = new System.Drawing.Size(200, 200);
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(_elementHost1.Width == 100, "Width Failed at MaximumSize 100,100.", p.log);
        sr.IncCounters(_elementHost1.Height == 100, "Height Failed at MaximumSize 100,100.", p.log);

        return sr;
    }

    [Scenario("Change Width.  Expected: Resize hosted control, clipping should occur.")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Width = 60;
        Utilities.SleepDoEvents(20);
  
        //Check size properties
        sr.IncCounters(_elementHost1.Width == 60, "Failed at width 60.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, _elementHost1.Left, _elementHost1.Top, _elementHost1.Width, _elementHost1.Height, Color.White) >= 1 &&
            BitmapsColorPercent(bmp, 210, 50, 60, 100, this.BackColor) == 100,
            "Bitmap Failed at Change Width. Percent match: " +
            BitmapsColorPercent(bmp, _elementHost1.Left, _elementHost1.Top, _elementHost1.Width, _elementHost1.Height, Color.White) + "%", p.log);

        //Set width smaller than MinimumSize
        _elementHost1.MinimumSize = new System.Drawing.Size(50, 50);
        _elementHost1.Width = 20;
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(_elementHost1.Width == 50, "Failed at MinimumSize 50,50.", p.log);

        //Set width larger than MaximumSize
        _elementHost1.MaximumSize = new System.Drawing.Size(100, 100);
        _elementHost1.Width = 120;
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(_elementHost1.Width == 100, "Failed at MaximumSize 100,100.", p.log);

        return sr;
    }

    [Scenario("Change Height.  Expected: Resize hosted control.")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Height = 50;
        Utilities.SleepDoEvents(20);
        
        //Check size properties
        sr.IncCounters(_elementHost1.Height == 50, "Failed at height 50.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 200, 30, Color.White) >= 1,
            "Bitmap Failed at Change Height. Percent match: " +
            BitmapsColorPercent(bmp, 150, 50, 200, 30, Color.White) + "%", p.log);

        //Set height smaller than MinimumSize
        _elementHost1.MinimumSize = new System.Drawing.Size(50, 50);
        _elementHost1.Height = 20;
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(_elementHost1.Height == 50, "Failed at MinimumSize 50,50.", p.log);

        //Set height larger than MaximumSize
        _elementHost1.MaximumSize = new System.Drawing.Size(100, 100);
        _elementHost1.Height = 120;
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(_elementHost1.Height == 100, "Failed at MaximumSize 100,100.", p.log);

        return sr;
    }

    [Scenario("Change ClientSize.  Expected: Resize hosted control.")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.ClientSize = new System.Drawing.Size(100, 210);
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 100, "Width Failed at Change Location.", p.log);
        sr.IncCounters(_elementHost1.Height == 210, "Height Failed at Change Location.", p.log);
        sr.IncCounters(_elementHost1.Left == 150, "Left Failed at Change Bounds at Change Location.", p.log);
        sr.IncCounters(_elementHost1.Top == 50, "Top Failed at Change Bounds at Change Location.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 100, 210, Color.White) >= 1,
            "Bitmap Failed at Change ClientSize. Percent match: " +
            BitmapsColorPercent(bmp, 150, 50, 100, 210, Color.White) + "%", p.log);
        return sr;
    }

    #endregion

    #region Utilities

    // BitmapsColorPercent
    // Starting at (x, y), look through the area of pixels in bmp specified by width and height 
    // for color match of specified color.
    // For every pixel that matches specified color, increment match counter.
    // Return percentage of matching pixels to total pixels.
    private static double BitmapsColorPercent(Bitmap bmp, int x, int y, int width, int height, Color color)
    {
        double total = 0;
        double match = 0;
        for (int col = x; col < x + width; col++)
        {
            for (int row = y; row < y + height; row++)
            {
                if (Utilities.ColorsMatch(bmp.GetPixel(col, row), color))
                {
                    match++;
                }
                total++;
            }
        }
        return match * 100 / total;
    }

    #endregion
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Element Host and hosted control should be sized to default 200, 100.
//@ Change Bounds. Expected: Resize/Reposition hosted control
//@ Change Top. Expected: Reposition hosted control
//@ Change Left.  Expected: Reposition hosted control
//@ Change Location. Expected: Reposition hosted control
//@ Change Size. Expected: Resize hosted control
//@ Change Width.  Expected: Resize hosted control, clipping should occur.
//@ Change Height.  Expected: Resize hosted control
//@ Change ClientSize.  Expected: Resize hosted control