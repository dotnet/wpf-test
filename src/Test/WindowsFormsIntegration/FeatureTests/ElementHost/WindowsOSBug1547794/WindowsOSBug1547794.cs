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

//
// Testcase:    WindowsOSBug1547794
// Description: Verify that setting the enabled property of ElementHost to false does not 
//              expose the backcolor of ElementHost
//
public class WindowsOSBug1547794 : ReflectBase
{
    #region Test case setup

    ElementHost _elementHost1;

    public WindowsOSBug1547794(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "WindowsOSBug1547794 Test";
        this.Size = new System.Drawing.Size(400, 400);
        base.InitTest(p);

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.BackColor = Color.Black;
        Controls.Add(_elementHost1);

        SWC.TextBox avTextBox1 = new SWC.TextBox();
        avTextBox1.Text = "Avalon TextBox";
        avTextBox1.Background = SWM.Brushes.White;
        _elementHost1.Child = avTextBox1;
    }

    #endregion

    #region Scenarios

    [Scenario("Verify setting Enabled to false")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        Utilities.SleepDoEvents(20);
        //Verify initial properties
        sr.IncCounters(_elementHost1.Enabled == true, "Failed at Enabled==true", p.log);
        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 200, 100, Color.White) >= 80,
          "Bitmap failed at Color.White >= 80%. Percent: " + 
          BitmapsColorPercent(bmp, 0, 0, 200, 100, Color.White) + "%", p.log);

        _elementHost1.Enabled = false;

        //Verify that the black BackColor of ElementHost is not exposed.
        //The faded black is by design.
        Utilities.SleepDoEvents(20);
        sr.IncCounters(_elementHost1.Enabled == false, "Failed at Enabled==False", p.log);
        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 200, 100, Color.Black) <= 1,
          "Bitmap failed at Color.Black <= 1%. Percent: " +
          BitmapsColorPercent(bmp, 0, 0, 200, 100, Color.Black) + "%", p.log);

        return sr;
    }

    #endregion

    #region Utilities

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
//@ Verify setting Enabled to false