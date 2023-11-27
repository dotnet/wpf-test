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
using SD = System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.IO;


//
// Testcase:    Opacity
// Description: Verify opacity works and can be set to any value and no exception will be thrown.
//
public class Opacity : ReflectBase
{
    #region Testcase setup

    ElementHost _elementHost1;
    SWC.StackPanel _stackPanel;
    SWC.Button _avButton1;
    Bitmap _bmp;

    public Opacity(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "Opacity";
        this.Size = new System.Drawing.Size(400, 400);

        _avButton1 = new SWC.Button();
        _avButton1.Content = "WPF";

        _stackPanel = new SWC.StackPanel();
        _stackPanel.Children.Add(_avButton1);

        _elementHost1 = new ElementHost();
        _elementHost1.Child = _stackPanel;
        this.Controls.Add(_elementHost1);

        base.InitTest(p);
    }

    private void Opacity_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
        LinearGradientBrush gradient = new LinearGradientBrush(new System.Drawing.Point(10, 10),
            new System.Drawing.Point(20, 20), Color.Blue, Color.White);
        gradient.WrapMode = WrapMode.TileFlipXY;
        e.Graphics.FillRectangle(gradient, new System.Drawing.Rectangle(0, 0, 400, 400));
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        if (scenario.Name != "Scenario1")
        {
            _avButton1 = new SWC.Button();
            _avButton1.Content = "WPF";

            _stackPanel = new SWC.StackPanel();
            _stackPanel.Children.Add(_avButton1);

            _elementHost1 = new ElementHost();
            _elementHost1.Child = _stackPanel;
            this.Controls.Add(_elementHost1);
        }
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult sr)
    {
        this.BackColor = SD.SystemColors.Control;
        this.BackgroundImage = null;
        this.Paint -= Opacity_Paint;

        Controls.Clear();

        base.AfterScenario(p, scenario, sr);
    }

    [Scenario("Form opacity  verify that 1 is opaque (completely visible).")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        this.Opacity = 1;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 1.0, "Failed at this.Opacity=1.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, SD.SystemColors.Control) >= 70,
            "Bitmap Failed at elementHost1.BackColor=" + SD.SystemColors.Control + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, SD.SystemColors.Control) + "%", p.log);

        return sr;
    }

    [Scenario("EH verify that 0 is transparent (invisible).")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        this.Opacity = 0;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.0, "Failed at this.Opacity=0.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, SD.SystemColors.Control) <= 10,
            "Bitmap Failed at elementHost1.BackColor=" + SD.SystemColors.Control + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, SD.SystemColors.Control) + "%", p.log);

        return sr;
    }

    [Scenario("EH Simple background (single color), form opacity = .25")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        _elementHost1.BackColor = Color.Red;

        this.Opacity = 0;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.0, "Failed at this.Opacity=0.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) <= 10,
            "Bitmap Failed at Opacity=0, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        this.Opacity = 0.25;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.25, "Failed at this.Opacity=0.25.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) <= 10,
            "Bitmap Failed at Opacity=0.25, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        this.Opacity = 0.5;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.5, "Failed at this.Opacity=0.5.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) <= 10,
            "Bitmap Failed at Opacity=0.5, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        this.Opacity = 0.75;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.75, "Failed at this.Opacity=0.75.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) <= 10,
            "Bitmap Failed at Opacity=0.75, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        this.Opacity = 1;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 1.0, "Failed at this.Opacity=1.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) >= 70,
            "Bitmap Failed at Opacity=1.0, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        return sr;
    }

    [Scenario("EH complex background (gradient), form opacity =.9")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        this.Paint += new System.Windows.Forms.PaintEventHandler(Opacity_Paint);
        _elementHost1.BackColorTransparent = true;
        _elementHost1.BackColor = Color.Transparent;

        this.Opacity = 0;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.0, "Failed at this.Opacity=0.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) <= 1,
            "Bitmap Failed at Opacity=0, elementHost1.BackColor=" + Color.Blue + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) + "%", p.log);

        this.Opacity = 0.5;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.5, "Failed at this.Opacity=0.5.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) <= 1,
            "Bitmap Failed at Opacity=0.5, elementHost1.BackColor=" + Color.Blue + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) + "%", p.log);

        this.Opacity = 0.9;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.9, "Failed at this.Opacity=0.9.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) <= 1,
            "Bitmap Failed at Opacity=0.9, elementHost1.BackColor=" + Color.Blue + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) + "%", p.log);

        this.Opacity = 1;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 1.0, "Failed at this.Opacity=1.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) >= 1.5,
            "Bitmap Failed at Opacity=1.0, elementHost1.BackColor=" + Color.Blue + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) + "%", p.log);

        return sr;
    }

    [Scenario("EH complex background (bitmap), form opacity =.9")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        try
        {
            Bitmap background = new Bitmap("Greenstone.bmp");
            _elementHost1.BackgroundImage = background;
        }
        catch(Exception)
        {
            throw new Exception("Error: Greenstone.bmp file not found.");
        }
        Color greenstone = Color.FromArgb(140, 148, 123);

        this.Opacity = 0;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.0, "Failed at this.Opacity=0.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) <= 1,
            "Bitmap Failed at Opacity=0, elementHost1.BackColor=" + greenstone + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) + "%. Expected: <= 1%", p.log);

        this.Opacity = 0.5;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.5, "Failed at this.Opacity=0.5.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) <= 1,
            "Bitmap Failed at Opacity=0.5, elementHost1.BackColor=" + greenstone + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) + "%. Expected: <= 1%", p.log);

        this.Opacity = 0.9;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.9, "Failed at this.Opacity=0.9.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) <= 30,
            "Bitmap Failed at Opacity=0.9, elementHost1.BackColor=" + greenstone + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) + "%. Expected: <= 30%", p.log);

        this.Opacity = 1;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 1.0, "Failed at this.Opacity=1.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) >= 30, p.log, 
            BugDb.WindowsOSBugs, 1600040, 
            "Bitmap Failed at Opacity=1.0, elementHost1.BackColor=" + greenstone + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) + "%. Expected: >= 30%");

        return sr;
    }

    [Scenario("EH Parent Simple background (single color), form opacity = .25")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        this.BackColor = Color.Red;

        this.Opacity = 0;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.0, "Failed at this.Opacity=0.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) <= 10,
            "Bitmap Failed at Opacity=0, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        this.Opacity = 0.25;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.25, "Failed at this.Opacity=0.25.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) <= 10,
            "Bitmap Failed at Opacity=0.25, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        this.Opacity = 0.5;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.5, "Failed at this.Opacity=0.5.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) <= 10,
            "Bitmap Failed at Opacity=0.5, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        this.Opacity = 0.75;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.75, "Failed at this.Opacity=0.75.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) <= 10,
            "Bitmap Failed at Opacity=0.75, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        this.Opacity = 1;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 1.0, "Failed at this.Opacity=1.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) >= 70,
            "Bitmap Failed at Opacity=1.0, elementHost1.BackColor=" + Color.Red + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        return sr;
    }

    [Scenario("EH Parent complex background (gradient), form opacity =.9")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        this.Paint += new System.Windows.Forms.PaintEventHandler(Opacity_Paint);
        _elementHost1.BackColorTransparent = true;
        _elementHost1.BackColor = Color.Transparent;

        this.Opacity = 0;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.0, "Failed at this.Opacity=0.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) <= 1,
            "Bitmap Failed at Opacity=0, elementHost1.BackColor=" + Color.Blue + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) + "%", p.log);

        this.Opacity = 0.5;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.5, "Failed at this.Opacity=0.5.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) <= 1,
            "Bitmap Failed at Opacity=0.5, elementHost1.BackColor=" + Color.Blue + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) + "%", p.log);

        this.Opacity = 0.9;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.9, "Failed at this.Opacity=0.9.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) <= 1,
            "Bitmap Failed at Opacity=0.9, elementHost1.BackColor=" + Color.Blue + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) + "%", p.log);

        this.Opacity = 1;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 1.0, "Failed at this.Opacity=1.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) >= 1.5,
            "Bitmap Failed at Opacity=1.0, elementHost1.BackColor=" + Color.Blue + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Blue) + "%", p.log);

        return sr;
    }

    [Scenario("EH Parent complex background (bitmap), form opacity =.3")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        try
        {
            Bitmap background = new Bitmap("Greenstone.bmp");
            //elementHost1.BackgroundImage = background;
            this.BackgroundImage = background;
        }
        catch
        {
            throw new Exception("Error: Greenstone.bmp file not found.");
        }
        Color greenstone = Color.FromArgb(140, 148, 123);
        _elementHost1.BackColorTransparent = true;
        _elementHost1.BackColor = Color.Transparent;

        this.Opacity = 0;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.0, "Failed at this.Opacity=0.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) <= 1,
            "Bitmap Failed at Opacity=0, elementHost1.BackColor=" + greenstone + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) + "%. Expected: <= 1%", p.log);

        this.Opacity = 0.3;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.3, "Failed at this.Opacity=0.3.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) <= 1,
            "Bitmap Failed at Opacity=0.3, elementHost1.BackColor=" + greenstone + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) + "%. Expected: <= 1%", p.log);

        this.Opacity = 0.9;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 0.9, "Failed at this.Opacity=0.75.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) <= 30,
            "Bitmap Failed at Opacity=0.9, elementHost1.BackColor=" + greenstone + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) + "%. Expected: <= 30%", p.log);

        this.Opacity = 1;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(this.Opacity, 1.0, "Failed at this.Opacity=1.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) >= 30,
            "Bitmap Failed at Opacity=1.0, elementHost1.BackColor=" + greenstone + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, greenstone) + "%. Expected: >= 30%", p.log);

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
//@ Form opacity  verify that 1 is opaque (completely visible).
//@ EH verify that 0 is transparent (invisible).
//@ EH Simple background (single color), form opacity = 0.25.
//@ EH complex background (gradiant), form opacity = 0.9.
//@ EH complex background (bitmap), form  opacity = 0.9.
//@ EH Parent Simple background (single color), form  opacity = 0.25.
//@ EH Parent complex background (gradiant), form  opacity = 0.9.
//@ EH Parent complex background (bitmap), form  opacity = 0.3.