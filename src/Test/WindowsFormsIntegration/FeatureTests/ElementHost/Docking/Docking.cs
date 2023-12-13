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
// Testcase:    Docking
// Description: Verify docking scenarios for Avalon controls in EH host.
//
public class Docking : ReflectBase
{
    #region Test case setup

    ElementHost _elementHost1;
    SWC.TextBox _avTextBox;

    public Docking(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "Docking Test";
        this.Size = new System.Drawing.Size(400, 400);
        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        _avTextBox = new SWC.TextBox();
        _avTextBox.Text = "TextBox";

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Child = _avTextBox;
        _elementHost1.BackColor = Color.White;
        _elementHost1.Size = new System.Drawing.Size(100, 50);
        _elementHost1.Location = new System.Drawing.Point(150, 50);
        Controls.Add(_elementHost1);

        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        base.AfterScenario(p, scenario, result);
    }

    [Scenario("Add an Avalon TextBox to an EH and EH.Dock = None.")]
    public ScenarioResult Scenario1(TParams p)
    {
	MyPause();
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Dock = SWF.DockStyle.None;

        MyPause();

        Bitmap bmp = Utilities.GetBitmapOfControl(this);

        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 100, 50, Color.White) >= 75,
          "Failed at Dockstyle.None. Percent match: " +
          BitmapsColorPercent(bmp, 150, 50, 100, 50, Color.White) +
          "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon TextBox to an EH and EH.Dock = Top.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Dock = SWF.DockStyle.Top;

        MyPause();

        Bitmap bmp = Utilities.GetBitmapOfControl(this);

        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, bmp.Width, 50, Color.White) >= 75,
          "Failed at Dockstyle.Top. Percent match: " +
          BitmapsColorPercent(bmp, 0, 0, bmp.Width, 50, Color.White) + 
          "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon TextBox to an EH and EH.Dock = Left.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Dock = SWF.DockStyle.Left;

        MyPause();

        Bitmap bmp = Utilities.GetBitmapOfControl(this);

        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 100, bmp.Height, Color.White) >= 75,
          "Failed at Dockstyle.Left. Percent match: " +
          BitmapsColorPercent(bmp, 0, 0, 100, bmp.Height, Color.White) +
          "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon TextBox to an EH and EH.Dock = Bottom.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Dock = SWF.DockStyle.Bottom;

        MyPause();

        Bitmap bmp = Utilities.GetBitmapOfControl(this);

        sr.IncCounters(BitmapsColorPercent(bmp, 0, bmp.Height - 50, bmp.Width, 50, Color.White) >= 75,
          "Failed at Dockstyle.Bottom. Percent match: " +
          BitmapsColorPercent(bmp, 0, bmp.Height - 50, bmp.Width, 50, Color.White) +
          "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon TextBox to an EH and EH.Dock = Right.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Dock = SWF.DockStyle.Right;

        MyPause();

        Bitmap bmp = Utilities.GetBitmapOfControl(this);

        sr.IncCounters(BitmapsColorPercent(bmp, bmp.Width - 100, 0, 100, bmp.Height, Color.White) >= 75,
            "Failed at Dockstyle.Right. Percent match: " +
          BitmapsColorPercent(bmp, 0, 0, bmp.Width, 50, Color.White) + 
          "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon TextBox to an EH and EH.Dock = Fill.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Dock = SWF.DockStyle.Fill;

        MyPause();

        Bitmap bmp = Utilities.GetBitmapOfControl(this);

        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, bmp.Width, bmp.Height, Color.White) >= 75,
            "Failed at Dockstyle.Fill. Percent match: " +
          BitmapsColorPercent(bmp, 0, 0, bmp.Width, 50, Color.White) + 
          "%", p.log);

        return sr;
    }
    #endregion

    #region Utilities

    private void MyPause()
    {
        SWF.Application.DoEvents();
        System.Threading.Thread.Sleep(200);
    }

    /// <summary>
    /// Returns the percentage of matching pixels to total pixels with the same ARGB value as the given color,
    /// in the area specified by width and height starting at location (x, y).  The algorithm checks each
    /// pixel in the bitmap and increments the match counter each time it finds a color match.
    /// </summary>
    /// <param name="bmp">Bitmap to search.</param>
    /// <param name="x">X-axis starting location.</param>
    /// <param name="y">Y-axis starting location.</param>
    /// <param name="width">Width of area to search.</param>
    /// <param name="height">Height of area to search.</param>
    /// <param name="color">Color to search for.</param>
    /// <returns>Percentage of matching pixels to total pixels.</returns>
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
//@ Add an Avalon TextBox to an EH and EH.Dock = None.
//@ Add an Avalon TextBox to an EH and EH.Dock = Top.
//@ Add an Avalon TextBox to an EH and EH.Dock = Left.
//@ Add an Avalon TextBox to an EH and EH.Dock = Bottom.
//@ Add an Avalon TextBox to an EH and EH.Dock = Right.
//@ Add an Avalon TextBox to an EH and EH.Dock = Fill.