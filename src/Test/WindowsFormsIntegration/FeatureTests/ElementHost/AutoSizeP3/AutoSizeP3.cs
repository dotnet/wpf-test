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
using System.Reflection;
using Microsoft.Test.Display;

//
// Testcase:    AutoSizeP3
// Description: Verify that sizing for static control in a static container is correct 
//              with explicit control sizing.
// Author:      a-rickyt
//
public class AutoSizeP3 : ReflectBase
{
    #region Test case setup

    ElementHost elementHost1;
    SWC.Button avButton;
    Bitmap bmp;


    public AutoSizeP3(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "AutoSizeP3 Test";
        this.Size = new System.Drawing.Size(400, 400);
        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        Controls.Clear();

        avButton = new SWC.Button();
        avButton.Content = "Av Button";
        avButton.Background = System.Windows.Media.Brushes.White;

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Child = avButton;
        elementHost1.BackColor = Color.Red;
        elementHost1.Location = new System.Drawing.Point(50, 50);
        Controls.Add(elementHost1);
        SWF.Application.DoEvents();

        elementHost1.AutoSize = true;
        elementHost1.Child = avButton;

        Utilities.SleepDoEvents(10);

        return base.BeforeScenario(p, scenario);
    }

    [Scenario("Verify the button is sized to it's content and the EH is the size of the button.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check size properties
        sr.IncCounters(elementHost1.Width < 200, "Failed at AutoSize=true on EH (width).", p.log);
        sr.IncCounters(elementHost1.Height < 100, "Failed at AutoSize=true on EH (height).", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 75, 23, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 75, 23, Color.White) + "%", p.log);

        //Add more text
        avButton.Content = "Avalon Button that is much longer than usually expected";
        this.Width += 1; //weird stuff to re-render button, but doesn't repro outside of test case
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(elementHost1.Width > 200, "Failed at AutoSize=true on EH with long text string.", p.log);
        sr.IncCounters(elementHost1.Height < 100, "Failed at AutoSize=true on EH (height).", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 281, 23, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 281, 23, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Explicitly set the size of the AV control the same as EH.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avButton.Width = 200;
        avButton.Height = 100;
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(200, (int)Monitor.ConvertScreenToLogical(Dimension.Width, elementHost1.Width), "Failed width at AutoSize=true on EH.", p.log);
        sr.IncCounters(100, (int)Monitor.ConvertScreenToLogical(Dimension.Height, elementHost1.Height), "Failed height at AutoSize=true on EH.", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 200, 100, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 200, 100, Color.White) + "%", p.log);

        //Add more text
        avButton.Content = "Avalon Button that is much longer than usually expected";
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(200, (int)Monitor.ConvertScreenToLogical(Dimension.Width, elementHost1.Width), "Failed at AutoSize=true on EH with long text string.", p.log);
        sr.IncCounters(100, (int)Monitor.ConvertScreenToLogical(Dimension.Height, elementHost1.Height), "Failed at AutoSize=true on EH with long text string.", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 200, 100, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 200, 100, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Explicitly set the size of the button and verify that it is not the same size as the EH.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avButton.Width = 100;
        avButton.Height = 100;
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(100, (int)Monitor.ConvertScreenToLogical(Dimension.Width, elementHost1.Width), "Failed at AutoSize=true on EH.", p.log);
        sr.IncCounters(100, (int)Monitor.ConvertScreenToLogical(Dimension.Height, elementHost1.Height), "Failed at AutoSize=true on EH.", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 100, 100, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 100, 100, Color.White) + "%", p.log);

        //Add more text
        avButton.Content = "Avalon Button that is much longer than usually expected";
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(100, (int)Monitor.ConvertScreenToLogical(Dimension.Width, elementHost1.Width), "Failed at AutoSize=true on EH with long text string.", p.log);
        sr.IncCounters(100, (int)Monitor.ConvertScreenToLogical(Dimension.Height, elementHost1.Height), "Failed at AutoSize=true on EH with long text string.", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 100, 100, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 100, 100, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Explicitly set the size of the AV control  > size of EH and verify it is clipped.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avButton.Width = 250;
        avButton.Height = 100;
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(Monitor.ConvertScreenToLogical(Dimension.Width, elementHost1.Width) > 249, "Failed at AutoSize=true on EH.", p.log);
        sr.IncCounters(100, (int)Monitor.ConvertScreenToLogical(Dimension.Height, elementHost1.Height), "Failed at AutoSize=true on EH.", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 250, 100, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 250, 100, Color.White) + "%", p.log);

        //Add more text
        avButton.Content = "Avalon Button that is much longer than usually expected";
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(Monitor.ConvertScreenToLogical(Dimension.Width, elementHost1.Width) > 249, "Failed at AutoSize=true on EH with long text string.", p.log);
        sr.IncCounters(100, (int)Monitor.ConvertScreenToLogical(Dimension.Height, elementHost1.Height), "Failed at AutoSize=true on EH with long text string.", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 250, 100, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 100, 100, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Set the EH size bigger than an explicitly size button and verify that the HostContainer is visible.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.AutoSize = false;

        avButton.Width = 100;
        avButton.Height = 50;
        
        elementHost1.Width = 200;
        elementHost1.Height = 200;
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(200, elementHost1.Width, "Failed at AutoSize=true on EH.", p.log);
        sr.IncCounters(200, elementHost1.Height, "Failed at AutoSize=true on EH.", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 100, 125, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 100, 125, 100, 50, Color.White) + "%", p.log);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 200, 200, Color.Red) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 200, 200, Color.Red) + "%", p.log);

        //Add more text
        avButton.Content = "Avalon Button that is much longer than usually expected";
        Utilities.SleepDoEvents(20);

        //Check size properties
        sr.IncCounters(200, elementHost1.Width, "Failed at AutoSize=true on EH with long text string.", p.log);
        sr.IncCounters(200, elementHost1.Height, "Failed at AutoSize=true on EH with long text string.", p.log);

        bmp = Utilities.GetBitmapOfControl(this);
        sr.IncCounters(BitmapsColorPercent(bmp, 100, 125, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 100, 125, 100, 50, Color.White) + "%", p.log);
        sr.IncCounters(BitmapsColorPercent(bmp, 50, 50, 200, 200, Color.Red) >= 1,
            "Bitmap Failed at AutoSize=true on EH. EH is not sized to fit contents of hosted control. " +
            "Percent match: " + BitmapsColorPercent(bmp, 50, 50, 200, 200, Color.Red) + "%", p.log);

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
//@ Verify the button is sized to it's content and the EH is the size of the button.
//@ Explicitly set the size of the AV control the same as EH.
//@ Explicitly set the size of the button and verify that it is not the same size as the EH.
//@ Explicitly set the size of the AV contol  > size of EH and verify it is clipped.
//@ Set the EH size bigger than an explicitly size button and verify that the HostContainer is visible.