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
// Testcase:    WindowsOSBug1370842 (BackColorTransparent)
// Description: If the BackColorTransparent is explicitly set to false, setting the 
//              BackColor to Color.Transparent should not override what was explicitly 
//              set and make BackColorTransparent=true. 
//
public class WindowsOSBug1370842 : ReflectBase
{
    #region Testcase setup

    ElementHost _elementHost1;
    SWC.StackPanel _stackPanel;
    SWC.Button _avButton1;
    Bitmap _bmp;

    public WindowsOSBug1370842(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "BackColorTransparent";
        this.Size = new System.Drawing.Size(400, 400);

        this.BackColor = Color.Red;

        _avButton1 = new SWC.Button();
        _avButton1.Content = "Avalon Button";

        _stackPanel = new SWC.StackPanel();
        _stackPanel.Children.Add(_avButton1);

        _elementHost1 = new ElementHost();
        _elementHost1.Child = _stackPanel;

        this.Controls.Add(_elementHost1);

        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult sr)
    {
        this.BackColor = SD.SystemColors.Control;
        this.Controls.Clear();
        base.AfterScenario(p, scenario, sr);
    }

    [Scenario("BackColorTransparent is explicitly set to false.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(5);

        TestProperties(sr, p, Color.Red, false, Color.Red, 75);

        _elementHost1.BackColorTransparent = true;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Red, true, Color.Red, 75);

        _elementHost1.BackColorTransparent = false;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Red, false, Color.Red, 75);

        _elementHost1.BackColor = Color.Transparent;
        Utilities.SleepDoEvents(20);
        TestProperties(sr, p, Color.Transparent, false, Color.Red, 75);

        return sr;
    }

    #endregion

    #region Utilities

    void TestProperties(ScenarioResult sr, TParams p, Color ehBackColor, bool ehBackColorTransparent,
        Color bitmapBackColor, double percent)
    {
        this.Text = "elementHost1.BackColorTransparent = " + ehBackColorTransparent;
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
//@ BackColorTransparent is explicitly set to false.