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
using SD = System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Test.Display;

//
// Testcase:    DeviceDPI
// Description: Verify a ElementHost's dimensions are scaled to its output device.
//
public class DeviceDPI : ReflectBase
{
    #region Test case setup

    ElementHost _elementHost1;
    Bitmap _bmp;

    public DeviceDPI(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "DeviceDPI Test";
        this.Size = new System.Drawing.Size(400, 400);

        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        Controls.Clear();

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Size = new System.Drawing.Size(200, 100);
        _elementHost1.BackColor = Color.Red;
        Controls.Add(_elementHost1);

        SWC.Button avButton1 = new SWC.Button();
        avButton1.Content = "Avalon Button";
        avButton1.Background = SWM.Brushes.White;
        _elementHost1.Child = avButton1;

        return base.BeforeScenario(p, scenario);
    }

    #endregion

    #region Scenarios

    [Scenario("AutoScaleMode=Font")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        Utilities.SleepDoEvents(20);

        //Verify initial state
        BitmapTest(sr, p, 0, 0, 200, 100, Color.White, 85);

        //change the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(20);
        BitmapTest(sr, p, 0, 0, 333, 192, Color.White, 83);

        //change the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(20);

        BitmapTest(sr, p, 0, 0, 133, 84, Color.White, 81);

        return sr;
    }

    [Scenario("AutoScaleMode=None.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        this.AutoScaleMode = SWF.AutoScaleMode.None;
        Utilities.SleepDoEvents(10);

        //Verify initial state
        BitmapTest(sr, p, 0, 0, 200, 100, Color.White, 85);

        //change the font
        this.Font = new Font("Impact", 25, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(10);

        BitmapTest(sr, p, 0, 0, 200, 100, Color.White, 75);

        //change the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(10);

        BitmapTest(sr, p, 0, 0, 200, 100, Color.White, 85);

        return sr;
    }

    [Scenario("AutoScaleMode=Inherit.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        this.AutoScaleMode = SWF.AutoScaleMode.Inherit;
        Utilities.SleepDoEvents(10);

        //Verify initial state
        BitmapTest(sr, p, 0, 0, 200, 100, Color.White, 85);

        //change the font
        this.Font = new Font("Impact", 14, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(10);

        BitmapTest(sr, p, 0, 0, 200, 100, Color.White, 80);

        //change the font
        this.Font = new Font("Impact", 30, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(10);

        BitmapTest(sr, p, 0, 0, 200, 100, Color.White, 70);

        return sr;
    }

    #endregion

    #region Utilities

    //Takes a bitmap of the form and checks the color of pixels of the bitmap in the given area by 
    //calling BitmapsColorPercent.
    private void BitmapTest(ScenarioResult sr, TParams p, int x, int y,
        int width, int height, Color color, int percent)
    {
        _bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(_bmp, x, y, 
		(int)Monitor.ConvertScreenToLogical(Dimension.Width, width), 
		(int)Monitor.ConvertScreenToLogical(Dimension.Height, height), color) >= percent,
            "Bitmap Failed at Color=" + color + ". Percent match: " +
            (BitmapsColorPercent(_bmp, x, y, 
		(int)Monitor.ConvertScreenToLogical(Dimension.Width, width), 
		(int)Monitor.ConvertScreenToLogical(Dimension.Height, height), color)) + "%. Expected: more than " + 
            percent + "%", p.log);
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
//@ AutoScaleMode=Font. 
//@ AutoScaleMode=None.
//@ AutoScaleMode=Inherit.