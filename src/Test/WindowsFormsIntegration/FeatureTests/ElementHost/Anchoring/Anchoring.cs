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
// Testcase:    Anchoring
// Description: Verify anchoring scenarios for Avalon controls in EH host.
// Author:      a-rickyt
//
public class Anchoring : ReflectBase
{
    #region Testcase setup

    ElementHost elementHost1;
    SWC.Button avButton;

    public Anchoring(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "Anchoring Test";
        this.Size = new System.Drawing.Size(300, 300);
        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        avButton = new SWC.Button();
        avButton.Background = System.Windows.Media.Brushes.White;
        avButton.Content = "Button";

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Child = avButton;
        elementHost1.Size = new System.Drawing.Size(100, 50);
        elementHost1.Location = new System.Drawing.Point(0, 0);
        elementHost1.BackColor = Color.White;
        Controls.Add(elementHost1);
        SWF.Application.DoEvents();

        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        this.Size = new System.Drawing.Size(300, 300);
        base.AfterScenario(p, scenario, result);
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = None.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.None;
        SWF.Application.DoEvents();
        Utilities.SleepDoEvents(10);
        this.Size = new System.Drawing.Size(600, 600);
        SWF.Application.DoEvents();
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Width Failed at AnchorStyles.None.", p.log);
        sr.IncCounters(elementHost1.Height == 50, "Height Failed at AnchorStyles.None.", p.log);
        sr.IncCounters(elementHost1.Left == 150, "Left Failed at AnchorStyles.None.", p.log);
        sr.IncCounters(elementHost1.Top == 150, "Top Failed at AnchorStyles.None.", p.log);

        Utilities.SleepDoEvents(10);
        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        SWF.Application.DoEvents();
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 150, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.None. Percent match: " +
            BitmapsColorPercent(bmp, 150, 150, 100, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = Top.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.Top;
        this.Size = new System.Drawing.Size(600, 600);
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Width Failed at AnchorStyles.Top.", p.log);
        sr.IncCounters(elementHost1.Height == 50, "Height Failed at AnchorStyles.Top.", p.log);
        sr.IncCounters(elementHost1.Left == 150, "Left Failed at AnchorStyles.Top.", p.log);
        sr.IncCounters(elementHost1.Top == 0, "Top Failed at AnchorStyles.Top.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 0, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.Top. Percent match: " +
            BitmapsColorPercent(bmp, 150, 0, 100, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = Left.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.Left;
        this.Size = new System.Drawing.Size(600, 600);
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Width Failed at AnchorStyles.Left.", p.log);
        sr.IncCounters(elementHost1.Height == 50, "Height Failed at AnchorStyles.Left.", p.log);
        sr.IncCounters(elementHost1.Left == 0, "Left Failed at AnchorStyles.Left.", p.log);
        sr.IncCounters(elementHost1.Top == 150, "Top Failed at AnchorStyles.Left.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 150, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.Left. Percent match: " +
            BitmapsColorPercent(bmp, 0, 150, 100, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = Bottom.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.Bottom;
        this.Size = new System.Drawing.Size(600, 600);
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Width Failed at AnchorStyles.Bottom.", p.log);
        sr.IncCounters(elementHost1.Height == 50, "Height Failed at AnchorStyles.Bottom.", p.log);
        sr.IncCounters(elementHost1.Left == 150, "Left Failed at AnchorStyles.Bottom.", p.log);
        sr.IncCounters(elementHost1.Top == 300, "Top Failed at AnchorStyles.Bottom.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 150, 300, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.Bottom. Percent match: " +
            BitmapsColorPercent(bmp, 150, 300, 100, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = Right.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.Right;
        this.Size = new System.Drawing.Size(600, 600);
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Failed at AnchorStyles.Right.", p.log);
        sr.IncCounters(elementHost1.Height == 50, "Failed at AnchorStyles.Right.", p.log);
        sr.IncCounters(elementHost1.Left == 300, "Failed at AnchorStyles.Right.", p.log);
        sr.IncCounters(elementHost1.Top == 150, "Failed at AnchorStyles.Right.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 300, 150, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.Right. Percent match: " +
            BitmapsColorPercent(bmp, 300, 150, 100, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = Top, Right.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.Top | SWF.AnchorStyles.Right;
        this.Size = new System.Drawing.Size(600, 600);
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Failed at AnchorStyles.Top, Right.", p.log);
        sr.IncCounters(elementHost1.Height == 50, "Failed at AnchorStyles.Top, Right.", p.log);
        sr.IncCounters(elementHost1.Left == 300, "Failed at AnchorStyles.Top, Right.", p.log);
        sr.IncCounters(elementHost1.Top == 0, "Failed at AnchorStyles.Top, Right.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 300, 0, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.Top, Right. Percent match: " +
            BitmapsColorPercent(bmp, 300, 0, 100, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = Top, Left.")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.Top | SWF.AnchorStyles.Left;
        this.Size = new System.Drawing.Size(600, 600);
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Failed at AnchorStyles.Top, Left.", p.log);
        sr.IncCounters(elementHost1.Height == 50, "Failed at AnchorStyles.Top, Left.", p.log);
        sr.IncCounters(elementHost1.Left == 0, "Failed at AnchorStyles.Top, Left.", p.log);
        sr.IncCounters(elementHost1.Top == 0, "Failed at AnchorStyles.Top, Left.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.Top, Left. Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 100, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = Bottom, Left.")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.Bottom | SWF.AnchorStyles.Left;
        this.Size = new System.Drawing.Size(600, 600);
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Failed at AnchorStyles.Bottom, Left.", p.log);
        sr.IncCounters(elementHost1.Height == 50, "Failed at AnchorStyles.Bottom, Left.", p.log);
        sr.IncCounters(elementHost1.Left == 0, "Failed at AnchorStyles.Bottom, Left.", p.log);
        sr.IncCounters(elementHost1.Top == 300, "Failed at AnchorStyles.Bottom, Left.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 300, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.Bottom, Left. Percent match: " +
            BitmapsColorPercent(bmp, 0, 300, 100, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = Bottom, Right.")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.Bottom | SWF.AnchorStyles.Right;
        this.Size = new System.Drawing.Size(600, 600);
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Failed at AnchorStyles.Bottom, Right.", p.log);
        sr.IncCounters(elementHost1.Height == 50, "Failed at AnchorStyles.Bottom, Right.", p.log);
        sr.IncCounters(elementHost1.Left == 300, "Failed at AnchorStyles.Bottom, Right.", p.log);
        sr.IncCounters(elementHost1.Top == 300, "Failed at AnchorStyles.Bottom, Right.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 300, 300, 100, 50, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.Bottom, Right. Percent match: " +
            BitmapsColorPercent(bmp, 300, 300, 100, 50, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Add an Avalon Button to an EH and EH.Anchor = Top, Bottom, Right.")]
    public ScenarioResult Scenario10(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        elementHost1.Anchor = SWF.AnchorStyles.Top | SWF.AnchorStyles.Bottom | SWF.AnchorStyles.Right;
        this.Size = new System.Drawing.Size(600, 600);
        Utilities.SleepDoEvents(10);

        //Check size and location properties
        sr.IncCounters(elementHost1.Width == 100, "Width Failed at AnchorStyles.Top, Bottom, Right.", p.log);
        sr.IncCounters(elementHost1.Height == 350, "Height Failed at AnchorStyles.Top, Bottom, Right.", p.log);
        sr.IncCounters(elementHost1.Left == 300, "Left Failed at AnchorStyles.Top, Bottom, Right.", p.log);
        sr.IncCounters(elementHost1.Top == 0, "Top Failed at AnchorStyles.Top, Bottom, Right.", p.log);

        Bitmap bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 300, 0, 100, 350, Color.White) >= 1,
            "Bitmap Failed at AnchorStyles.Top, Bottom, Right. Percent match: " +
            BitmapsColorPercent(bmp, 300, 0, 100, 350, Color.White) + "%", p.log);

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
//@ Add an Avalon TextBox to an EH and EH.Anchor = None.
//@ Add an Avalon TextBox to an EH and EH.Anchor = Top.
//@ Add an Avalon TextBox to an EH and EH.Anchor = Left.
//@ Add an Avalon TextBox to an EH and EH.Anchor = Bottom.
//@ Add an Avalon TextBox to an EH and EH.Anchor = Right.
//@ Add an Avalon Button to an EH and EH.Anchor = Top, Right.
//@ Add an Avalon Button to an EH and EH.Anchor = Top, Left.
//@ Add an Avalon Button to an EH and EH.Anchor = Bottom, Left.
//@ Add an Avalon Button to an EH and EH.Anchor = Bottom, Right.
//@ Add an Avalon Button to an EH and EH.Anchor = Top, Bottom, Right.