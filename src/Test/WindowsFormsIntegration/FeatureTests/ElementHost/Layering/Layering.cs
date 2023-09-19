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
using SD = System.Drawing;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;


// Testcase:    Layering
// Description: Verify z-ordering and painting of avalon elements on EH.
public class Layering : ReflectBase
{
    #region Test case setup

    ElementHost _elementHost1;
    SWF.Button _wfButton1;
    Bitmap _bmp;

    public Layering(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "Layering Test";
        this.Size = new System.Drawing.Size(400, 400);

        //Add WF button to partially cover EH
        _wfButton1 = new SWF.Button();
        _wfButton1.Text = "WinForm Button";
        _wfButton1.Size = new System.Drawing.Size(100, 25);
        _wfButton1.BackColor = Color.Blue;
        _wfButton1.ForeColor = Color.White;
        Controls.Add(_wfButton1);

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.BackColor = Color.Red;
        Controls.Add(_elementHost1);

        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    [Scenario("Add a WF control to partially cover EH and verify no painting issues.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        Utilities.SleepDoEvents(5);
        //Verify no painting issues on Element Host
        BitmapTest(sr, p, 0, 0, 200, 100, Color.Red, 85);

        //Verify no painting issues on WF button
        BitmapTest(sr, p, 0, 0, 100, 25, Color.Blue, 40);
        BitmapTest(sr, p, 0, 0, 100, 25, Color.White, 1);

        //Add AV control to EH
        SWC.Button avButton1 = new SWC.Button();
        avButton1.Content = "Avalon Button";
        avButton1.Background = SWM.Brushes.White;
        _elementHost1.Child = avButton1;
        Utilities.SleepDoEvents(20);

        //Verify no painting issues on Element Host
        BitmapTest(sr, p, 0, 0, 200, 100, Color.White, 75);

        //Verify no painting issues on WF button
        BitmapTest(sr, p, 0, 0, 100, 25, Color.Blue, 40);
        BitmapTest(sr, p, 0, 0, 100, 25, Color.White, 1);

        return sr;
    }

    [Scenario("Remove partially covered WF control overlapping EH and verify no painting issues.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.Child = null;
        Utilities.SleepDoEvents(5);

        //Remove WF button
        Controls.Remove(_wfButton1);
        Utilities.SleepDoEvents(5);

        //Verify no painting issues on Element Host
        BitmapTest(sr, p, 0, 0, 200, 100, Color.Red, 100);

        //Restore WF button to test on AV control
        Controls.Add(_wfButton1);
        _wfButton1.BringToFront();
        Utilities.SleepDoEvents(5);

        //Verify no painting issues on Element Host
        BitmapTest(sr, p, 0, 0, 200, 100, Color.Red, 85);

        //Verify no painting issues on WF button
        BitmapTest(sr, p, 0, 0, 100, 25, Color.Blue, 40);
        BitmapTest(sr, p, 0, 0, 100, 25, Color.White, 1);

        //Add AV control to EH
        SWC.Button avButton1 = new SWC.Button();
        avButton1.Content = "Avalon Button";
        avButton1.Background = SWM.Brushes.White;
        _elementHost1.Child = avButton1;
        Utilities.SleepDoEvents(20);

        //Remove WF button
        Controls.Remove(_wfButton1);

        //Verify no painting issues on Element Host
        BitmapTest(sr, p, 0, 0, 200, 100, Color.White, 75);

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
        sr.IncCounters(BitmapsColorPercent(_bmp, x, y, width, height, color) >= percent,
            "Bitmap Failed at Color=" + color + ". Percent match: " +
            BitmapsColorPercent(_bmp, x, y, width, height, color) + "%. Expected: greater than " +
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
//@ Add a WF control to partially cover EH and verify no painting issues.
//@ Remove partially covered WF control overlapping EH and verify no painting issues.
