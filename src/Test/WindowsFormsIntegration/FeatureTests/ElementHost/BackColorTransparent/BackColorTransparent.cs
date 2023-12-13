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
// Testcase:    BackColorTransparent
// Description: Verify BackColor=Transparent and BackColorTransparent properties. 
//
public class BackColorTransparent : ReflectBase
{
    #region Testcase setup

    ElementHost _elementHost1;
    SWC.StackPanel _stackPanel;
    SWC.Button _avButton1;
    Bitmap _bmp;

    public BackColorTransparent(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "BackColorTransparent";
        this.Size = new System.Drawing.Size(400, 400);

        base.InitTest(p);
    }

    private void BackColorTransparent_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
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
        _elementHost1 = new ElementHost();
        this.Controls.Add(_elementHost1);

        _avButton1 = new SWC.Button();
        _avButton1.Content = "Avalon Button";

        _stackPanel = new SWC.StackPanel();
        _stackPanel.Children.Add(_avButton1);

        _elementHost1.Child = _stackPanel;

        if (scenario.Name == "Scenario3")
        {
            this.Paint += new System.Windows.Forms.PaintEventHandler(BackColorTransparent_Paint);
        }

        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult sr)
    {
        this.BackColor = SD.SystemColors.Control;
        this.BackgroundImage = null;
        this.Paint -= BackColorTransparent_Paint;
        this.Controls.Clear();
        base.AfterScenario(p, scenario, sr);
    }

    [Scenario("SolidBackColor.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        this.BackColor = Color.Red;

        Utilities.SleepDoEvents(10);
        TestProperties(sr, p, Color.Red, false, Color.Red, 75);

        _elementHost1.BackColorTransparent = true;
        Utilities.SleepDoEvents(10);
        TestProperties(sr, p, Color.Red, true, Color.Red, 75);

        _elementHost1.BackColor = Color.White;
        Utilities.SleepDoEvents(10);
        TestProperties(sr, p, Color.White, true, Color.Red, 75);

        _elementHost1.BackColorTransparent = true;
        Utilities.SleepDoEvents(10);
        TestProperties(sr, p, Color.White, true, Color.Red, 75);

        _elementHost1.BackColor = Color.Transparent;
        Utilities.SleepDoEvents(10);
        TestProperties(sr, p, Color.Transparent, true, Color.Red, 75);

        this.BackColor = Color.Green;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Transparent, true, Color.Red, 75);

        _elementHost1.BackColor = Color.Red;
        Utilities.SleepDoEvents(10);
        TestProperties(sr, p, Color.Red, true, Color.Green, 75);

        _elementHost1.BackColor = Color.Blue;
        Utilities.SleepDoEvents(10);
        TestProperties(sr, p, Color.Blue, true, Color.Green, 75);

        _elementHost1.BackColorTransparent = false;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Blue, false, Color.Blue, 75);

        _elementHost1.BackColor = Color.Transparent;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Transparent, false, Color.Green, 75);

        _elementHost1.BackColor = Color.Green;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Green, false, Color.Green, 75);

        return sr;
    }

    [Scenario("Some other background bitmap.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        Bitmap background = new Bitmap("Greenstone.bmp");
        Color greenstone = Color.FromArgb(140, 148, 123);
        this.BackgroundImage = background;

        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, SD.SystemColors.Control, false, SD.SystemColors.Control, 75);

        _elementHost1.BackColorTransparent = true;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, SD.SystemColors.Control, true, greenstone, 30);

        _elementHost1.BackColor = Color.Transparent;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Transparent, true, greenstone, 30);

        _elementHost1.BackColorTransparent = false;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Transparent, false, greenstone, 30);

        _elementHost1.BackColor = Color.Blue;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Blue, false, Color.Blue, 75);

        _elementHost1.BackColorTransparent = true;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Blue, true, greenstone, 30);

        return sr;
    }

    [Scenario("Some other background gradient.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        _avButton1.Background = System.Windows.Media.Brushes.Yellow;

        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, SD.SystemColors.Control, false, SD.SystemColors.Control, 75);

        _elementHost1.BackColorTransparent = true;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, SD.SystemColors.Control, true, Color.Blue, 1.5);

        _elementHost1.BackColor = Color.Transparent;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Transparent, true, Color.Blue, 1.5);

        _elementHost1.BackColorTransparent = false;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Transparent, false, Color.Blue, 1.5);

        _elementHost1.BackColor = Color.Blue;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Blue, false, Color.Blue, 1.5);

        _elementHost1.BackColorTransparent = true;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Blue, true, Color.Blue, 1.5);

        return sr;
    }

    [Scenario("Some other background animated background.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        _avButton1.Background = System.Windows.Media.Brushes.Yellow;

        _elementHost1.Location = new System.Drawing.Point(0, 0);

        try
        {
            SWF.PictureBox background = new SWF.PictureBox();
            background.Image = new Bitmap("Cowboy_on_computer.gif");
            background.Size = new System.Drawing.Size(218, 218);
            this.Controls.Add(background);
        }
        catch
        {
            throw new Exception("Error: Cowboy_on_computer.gif file not found.");
        }
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, SD.SystemColors.Control, false, SD.SystemColors.Control, 75);

        _elementHost1.BackColorTransparent = true;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, SD.SystemColors.Control, true, SD.SystemColors.Control, 75);

        _elementHost1.BackColor = Color.Transparent;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Transparent, true, SD.SystemColors.Control, 75);

        _elementHost1.BackColorTransparent = false;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Transparent, false, SD.SystemColors.Control, 75);

        _elementHost1.BackColor = Color.Blue;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Blue, false, Color.Blue, 75);

        _elementHost1.BackColorTransparent = true;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Blue, true, SD.SystemColors.Control, 75);

        return sr;
    }

    #endregion

    #region Utilities

    void TestProperties(ScenarioResult sr, TParams p, Color ehBackColor, bool ehBackColorTransparent,
        Color bitmapBackColor, double percent)
    {
        sr.IncCounters(ehBackColor, _elementHost1.BackColor,
            "Failed at elementHost1.BackColor.", p.log);
        sr.IncCounters(_elementHost1.BackColorTransparent, ehBackColorTransparent,
            "Failed at elementHost1.BackColorTransparent.", p.log);
        _bmp = Utilities.GetBitmapOfControl(_elementHost1);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, bitmapBackColor) >= percent,
            "Bitmap Failed at elementHost1.BackColor=" + bitmapBackColor + ". Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, bitmapBackColor) + "%", p.log);
    }

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
//@ SolidBackColor.
//@ Some other background bitmap.
//@ Some other background gradient.
//@ Some other background animated background.