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

// Testcase:    AutoSize
// Description: Verify that auto-sizing for static control in a static container is correct.
public class AutoSize : ReflectBase
{
    #region Test case setup

    ElementHost _elementHost1;
    SWC.Button _avButton;

    public AutoSize(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "AutoSize Test";
        this.Size = new System.Drawing.Size(400, 400);
        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        _avButton = new SWC.Button();
        _avButton.Content = "Av Button";
        _avButton.Background = System.Windows.Media.Brushes.White;

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Child = _avButton;
        _elementHost1.BackColor = Color.Red;
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

    [Scenario("AutoSize=true on EH. Verify EH is sized to fit contents of hosted avalon Windows.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.AutoSize = true;

        SWC.StackPanel stackPanel = new SWC.StackPanel();
        _elementHost1.Child = stackPanel;
        stackPanel.Children.Add(_avButton);
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(true, _elementHost1.AutoSize, "Failed at AutoSize=true on EH.", p.log);
        sr.IncCounters(_elementHost1.Width < 200, "Failed at AutoSize=true on EH (width).", p.log);
        sr.IncCounters(_elementHost1.Height < 100, "Failed at AutoSize=true on EH (height).", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, _elementHost1.Width, _elementHost1.Height, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted avalon Windows. " +
            "Percent match: " + BitmapsColorPercent(bmp, 150, 50, _elementHost1.Width, _elementHost1.Height, Color.White) + "%. Expected >= 1.", p.log);


        //Add more text
        _avButton.Content = "Av Button that is much longer than usually expected";
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(_elementHost1.Width > 200, "Failed at AutoSize=true on EH with long text string.", p.log);
        sr.IncCounters(_elementHost1.Height < 100, "Failed at AutoSize=true on EH (height).", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        bmp.Save("1.jpg");
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, this.ClientSize.Width - 150, _elementHost1.Height, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH with longer text. EH is not sized to fit contents of hosted avalon Windows. " +
            "Percent match: " + BitmapsColorPercent(bmp, 150, 50, this.ClientSize.Width - 150, _elementHost1.Height, Color.White) + "%. Expected >= 1.", p.log);

        return sr;
    }

    [Scenario("AutoSize=false on EH and EH size to less than content of Avalon control. " +
        "Verify contents of Avalon control is clipped.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.AutoSize = false;
        _elementHost1.Size = new System.Drawing.Size(70, 50);
        _avButton.Content = "Av Button with more content than usual";
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 70, "Width Failed at AutoSize=false and EH size to less than " +
            "content of Avalon control.", p.log);
        sr.IncCounters(_elementHost1.Height == 50, "Height Failed at AutoSize=false and EH size to less than " +
            "content of Avalon control.", p.log);
        sr.IncCounters(_elementHost1.Left == 150, "Left Failed at AutoSize=false and EH size to less than " +
            "content of Avalon control.", p.log);
        sr.IncCounters(_elementHost1.Top == 50, "Top Failed at AutoSize=false and EH size to less than " +
            "content of Avalon control.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 70, 50, Color.White) >= 1 &&
            BitmapsColorPercent(bmp, 250, 50, 100, 100, this.BackColor) == 100 &&
            BitmapsColorPercent(bmp, 150, 100, 100, 100, this.BackColor) == 100,
            "Bitmap Failed at AutoSize=false, Verify contents of Avalon control is clipped. " +
            "Percent match: " + BitmapsColorPercent(bmp, 150, 50, 70, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("AutoSize=true on EH and increase Font size of hosted Avalon control. Verify" +
        "EH is sized to fit contents of hosted avalon Window.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        _elementHost1.AutoSize = true;

        SWC.StackPanel stackPanel = new SWC.StackPanel();
        _elementHost1.Child = stackPanel;
        stackPanel.Children.Add(_avButton);
        Utilities.SleepDoEvents(20);
        //Increase font size
        _avButton.FontSize = 50;

        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width > 200, "Width Failed at AutoSize=true and FontSize increase.", p.log);
        sr.IncCounters(_elementHost1.Height < 100, "Height Failed at AutoSize=true and FontSize increase.", p.log);
        sr.IncCounters(_elementHost1.Left == 150, "Left Failed at AutoSize=true and FontSize increase.", p.log);
        sr.IncCounters(_elementHost1.Top == 50, "Top Failed at AutoSize=true and FontSize increase.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 224, 64, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true and FontSize increase. EH should resize to fit contents of " +
            "hosted avalon Window.", p.log);

        return sr;
    }

    [Scenario("AutoSize=false on EH and increase Font size of hosted Avalon control. " +
        "Verify contents of Avalon control are clipped.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.AutoSize = false;

        _avButton.FontSize = 60;
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 200, "Width Failed at AutoSize=false and FontSize increase.", p.log);
        sr.IncCounters(_elementHost1.Height == 100, "Height Failed at AutoSize=false and FontSize increase.", p.log);
        sr.IncCounters(_elementHost1.Left == 150, "Left Failed at AutoSize=false and FontSize increase.", p.log);
        sr.IncCounters(_elementHost1.Top == 50, "Top Failed at AutoSize=false and FontSize increase.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 200, 100, Color.White) >= 1 &&
            BitmapsColorPercent(bmp, 350, 50, 30, 100, this.BackColor) == 100 &&
            BitmapsColorPercent(bmp, 150, 150, 200, 100, this.BackColor) == 100,
            "Bitmap Failed at AutoSize=false and FontSize increase. Percent match: " +
            BitmapsColorPercent(bmp, 150, 50, 200, 100, Color.White) + "%", p.log);

        _avButton.FontSize = 80;
        Utilities.SleepDoEvents(20);

        //Check size and location properties
        sr.IncCounters(_elementHost1.Width == 200, "Width Failed at AutoSize=false and FontSize increase.", p.log);
        sr.IncCounters(_elementHost1.Height == 100, "Height Failed at AutoSize=false and FontSize increase.", p.log);
        sr.IncCounters(_elementHost1.Left == 150, "Left Failed at AutoSize=false and FontSize increase.", p.log);
        sr.IncCounters(_elementHost1.Top == 50, "Top Failed at AutoSize=false and FontSize increase.", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 200, 100, Color.White) >= 1 &&
            BitmapsColorPercent(bmp, 350, 50, 30, 100, this.BackColor) == 100 &&
            BitmapsColorPercent(bmp, 150, 150, 200, 100, this.BackColor) == 100,
            "Bitmap Failed at AutoSize=false and FontSize increase. Percent match: " +
            BitmapsColorPercent(bmp, 150, 50, 200, 100, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Verify that the HostContainer is not visible with AutoSize = true.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.AutoSize = true;
        Utilities.SleepDoEvents(20);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 75, 23, Color.White) >= 1 &&
            BitmapsColorPercent(bmp, 350, 50, 10, 100, this.BackColor) == 100 &&
            BitmapsColorPercent(bmp, 150, 101, 100, 100, this.BackColor) == 100,
            "Bitmap Failed at AutoSize = true (HostContainer should not be visible)." +
            "Percent match: " + BitmapsColorPercent(bmp, 150, 50, 75, 23, Color.White) + "%. Expected >= 1.", p.log);

        //Set MaxWidth and MaxHeight of hosted element to smaller than element host.  Element
        //host should resize to fit hosted element.
        _avButton.MaxWidth = 50;
        _avButton.MaxHeight = 50;
        _elementHost1.Width = 150;
        _elementHost1.Height = 150;
        Utilities.SleepDoEvents(20);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 75, 23, Color.White) >= 1 &&
            BitmapsColorPercent(bmp, 225, 50, 100, 100, this.BackColor) == 100 &&
            BitmapsColorPercent(bmp, 150, 101, 100, 100, this.BackColor) == 100,
            "Bitmap Failed after resize at AutoSize = true (HostContainer should not be visible). " +
            "Percent match: " + BitmapsColorPercent(bmp, 150, 50, 75, 23, Color.White) + "%. Expected >= 1.", p.log);


        return sr;
    }

    [Scenario("Verify that the HostContainer is not visible with AutoSize = false.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.AutoSize = false;
        Utilities.SleepDoEvents(20);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 200, 100, Color.White) >= 1,
            "Bitmap Failed at AutoSize = true (HostContainer should not be visible). Percent match: " +
            BitmapsColorPercent(bmp, 150, 50, 200, 100, Color.White) + "%", p.log);

        _elementHost1.Width = 150;
        Utilities.SleepDoEvents(20);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 150, 100, Color.White) >= 1,
            "Bitmap Failed at AutoSize = true (HostContainer should not be visible). Percent match: " +
            BitmapsColorPercent(bmp, 150, 50, 150, 100, Color.White) + "%", p.log);

        _elementHost1.Height = 150;
        Utilities.SleepDoEvents(20);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 50, 150, 150, Color.White) >= 1,
            "Bitmap Failed at AutoSize = true (HostContainer should not be visible). Percent match: " +
            BitmapsColorPercent(bmp, 150, 50, 150, 150, Color.White) + "%", p.log);

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
//@ AutoSize=true on EH. Verify EH is sized to fit contents of hosted avalon Windows. 
//@ AutoSize=false on EH and EH size to less than content of Avalon control. Verify contents of Avalon control is clipped.
//@ AutoSize=true on EH and increase Font size of hosted Avalon control. Verify EH is sized to fit contents of hosted avalon Windows. 
//@ AutoSize=false on EH and increase Font size of hosted Avalon control. Verify contents of Avalon control are clipped.
//@ Verify that the HostContainer is not visible with AutoSize = true. 
//@ Verify that the HostContainer is not visible with AutoSize = false.