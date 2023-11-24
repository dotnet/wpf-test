using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Media;
using SWM=System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using SD=System.Drawing;
using SWF=System.Windows.Forms;
using System.Windows.Controls;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

//
// Testcase:    ColorsAndThemes
// Description: Test existing spec defined Mappings
// Author:      sameerm
//
public class ColorsAndThemes : ReflectBase
{
    #region Testcase setup
    public ColorsAndThemes(string[] args) : base(args) { }



    protected override void InitTest(TParams p)
    {
        _tp = p;
        base.InitTest(p);
    }

    
    
    #region Class Vars and other definitions
    private bool debug = false;
    private ElementHost eh;
    private TParams _tp;
    private static DockPanel dp;                        // our Dockpanel
    private static System.Windows.Controls.Button avBtn;                        // our Avalon button
    #endregion

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        //Avalon Button  
        avBtn = new SWC.Button();
        avBtn.Content = (char)0x2588; // UNICODE block character (so that we can fill up the control for the foreColor tests)
        avBtn.MaxWidth = avBtn.Width = 100;
        avBtn.MaxHeight = avBtn.Height = 100;
        avBtn.FontSize = 14;
        avBtn.FontWeight = System.Windows.FontWeights.Bold;

        dp = new DockPanel();
        dp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        dp.VerticalAlignment = VerticalAlignment.Center;

        eh = new ElementHost();
        eh.Size = new SD.Size(200, 200);

        if (scenario.Name == "Scenario5" || scenario.Name == "Scenario6")
        {
            avBtn.FontSize = 200;
            eh.Child = avBtn;
        }
        else
        {
            dp.Children.Add(avBtn);
            eh.Child = dp;
        }
        Controls.Add(eh);

        this.Width = 300;
        this.Height = 300;
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, System.Reflection.MethodInfo scenario, ScenarioResult result)
    {
        this.Controls.Clear();
        avBtn = null;
        dp = null;
        eh = null;
        this.ForeColor = System.Drawing.Color.Black;
        this.BackColor = System.Drawing.SystemColors.Control;
        base.AfterScenario(p, scenario, result);
    }
    #endregion

    #region Scenarios
    //==========================================
    // Scenarios
    //==========================================
    [Scenario("Set backcolor of the EH with hosted control background set to White")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avBtn.Background = System.Windows.Media.Brushes.White;
        eh.BackColor = System.Drawing.Color.Red;
        SWF.Application.DoEvents();
        Utilities.SleepDoEvents(2);
        Bitmap ehBitmap = CreateBitmap(eh);

        Utilities.SleepDoEvents(2);
        Bitmap avBtnBitmap = CreateBitmap(avBtn);

        if (debug) ehBitmap.Save(@"eh.bmp");
        if (debug) avBtnBitmap.Save(@"avBtn.bmp");

        // DockPanel Background should have a default of null
        sr.IncCounters(null, dp.Background, "DockPanel default Background not null", p.log);

        // Check and make sure the eh back color is changed
        sr.IncCounters(System.Drawing.Color.Red, eh.BackColor, "Failed at eh.BackColor.", p.log);

        // ElementHost (DockPanel) should inherent the BackColor
        sr.IncCounters(BitmapsColorPercent(ehBitmap, 5, 5, 50, 50, System.Drawing.Color.Red) >= 95,
            "Bitmap Failed at eh.BackColor=" + System.Drawing.Color.Red + ". Percent match: " +
            BitmapsColorPercent(ehBitmap, 5, 5, 50, 50, System.Drawing.Color.Red) + "%", p.log);

        // Hosted control should get the eh BackColor
        sr.IncCounters(BitmapsColorPercent(avBtnBitmap, 5, 5, 50, 50, System.Drawing.Color.White) >= 80,
            "Bitmap Failed at avBtn.Background=" + System.Drawing.Color.White + ". Percent match: " +
            BitmapsColorPercent(avBtnBitmap, 5, 5, 50, 50, System.Drawing.Color.White) + "%", p.log); 

        return sr;
    }
    [Scenario("Set backcolor of the EH with hosted control background set to null")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avBtn.Background = null;
        //dp.Background = System.Windows.Media.Brushes.Aqua;
        //        avBtn.Background = null;
        eh.BackColor = System.Drawing.Color.White;
        SWF.Application.DoEvents();
        Utilities.SleepDoEvents(2);
        Bitmap ehBitmap = CreateBitmap(eh);
        Utilities.SleepDoEvents(2);
        Bitmap avBtnBitmap = CreateBitmap(avBtn);

        // DockPanel Background should have a default of null
        sr.IncCounters(null, dp.Background, "DockPanel default Background not null", p.log);

        // Check and make sure the eh back color is changed
        sr.IncCounters(System.Drawing.Color.White, eh.BackColor, "Failed at eh.BackColor.", p.log);

        // ElementHost (DockPanel) should inherent the BackColor
        sr.IncCounters(BitmapsColorPercent(ehBitmap, 5, 5, 50, 50, System.Drawing.Color.White) >= 95,
            "Bitmap Failed at eh.BackColor=" + System.Drawing.Color.White + ". Percent match: " +
            BitmapsColorPercent(ehBitmap, 5, 5, 50, 50, System.Drawing.Color.White) + "%", p.log);

        // Hosted control should get the eh BackColor
        sr.IncCounters(BitmapsColorPercent(avBtnBitmap, 5, 5, 50, 50, System.Drawing.Color.White) >= 80,
            "Bitmap Failed at avBtn.Background=" + System.Drawing.Color.White + ". Percent match: " +
            BitmapsColorPercent(avBtnBitmap, 5, 5, 50, 50, System.Drawing.Color.White) + "%", p.log); return sr;
    }

    [Scenario("Set backcolor of the EH parent with hosted control background set to White")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avBtn.Background = System.Windows.Media.Brushes.White;
        this.BackColor = System.Drawing.Color.Red;
        SWF.Application.DoEvents();
        Utilities.SleepDoEvents(2);
        Bitmap ehBitmap = CreateBitmap(eh);
        Utilities.SleepDoEvents(2);
        Bitmap avBtnBitmap = CreateBitmap(avBtn);

        // DockPanel Background should have a default of null
        sr.IncCounters(null, dp.Background, "DockPanel default Background not null", p.log);

        // Check and make sure the eh back color is changed
        sr.IncCounters(System.Drawing.Color.Red, eh.BackColor, "Failed at eh.BackColor.", p.log);

        // ElementHost (DockPanel) should inherent the BackColor
        sr.IncCounters(BitmapsColorPercent(ehBitmap, 5, 5, 50, 50, System.Drawing.Color.Red) >= 95,
            "Bitmap Failed at eh.BackColor=" + System.Drawing.Color.Red + ". Percent match: " +
            BitmapsColorPercent(ehBitmap, 5, 5, 50, 50, System.Drawing.Color.Red) + "%", p.log);

        // Hosted control should get the eh BackColor
        sr.IncCounters(BitmapsColorPercent(avBtnBitmap, 5, 5, 50, 50, System.Drawing.Color.White) >= 80,
            "Bitmap Failed at avBtn.Background=" + System.Drawing.Color.White + ". Percent match: " +
            BitmapsColorPercent(avBtnBitmap, 5, 5, 50, 50, System.Drawing.Color.White) + "%", p.log);

        return sr;
    }
    [Scenario("Set backcolor of the EH parent with hosted control background set to null")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avBtn.Background = null;
        this.BackColor = System.Drawing.Color.White;
        SWF.Application.DoEvents();
        Utilities.SleepDoEvents(2);
        Bitmap ehBitmap = CreateBitmap(eh);
        Utilities.SleepDoEvents(2);
        Bitmap avBtnBitmap = CreateBitmap(avBtn);

        // DockPanel Background should have a default of null
        sr.IncCounters(null, dp.Background, "DockPanel default Background not null", p.log);

        // Check and make sure the eh back color is changed
        sr.IncCounters(System.Drawing.Color.White, eh.BackColor, "Failed at eh.BackColor.", p.log);

        // ElementHost (DockPanel) should inherent the BackColor
        sr.IncCounters(BitmapsColorPercent(ehBitmap, 5, 5, 50, 50, System.Drawing.Color.White) >= 95,
            "Bitmap Failed at eh.BackColor=" + System.Drawing.Color.White + ". Percent match: " +
            BitmapsColorPercent(ehBitmap, 5, 5, 50, 50, System.Drawing.Color.White) + "%", p.log);

        // Hosted control should get the eh BackColor
        sr.IncCounters(BitmapsColorPercent(avBtnBitmap, 5, 5, 50, 50, System.Drawing.Color.White) >= 80,
            "Bitmap Failed at avBtn.Background=" + System.Drawing.Color.White + ". Percent match: " +
            BitmapsColorPercent(avBtnBitmap, 5, 5, 50, 50, System.Drawing.Color.White) + "%", p.log); 

	return sr;
    }
    [Scenario("Set forecolor of the EH with hosted control foreground set to Blue")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avBtn.Foreground = System.Windows.Media.Brushes.Blue;
        eh.ForeColor = System.Drawing.Color.Red;
        SWF.Application.DoEvents();

        Utilities.SleepDoEvents(2);
        Bitmap avBtnBitmap = CreateBitmap(avBtn);
//        Utilities.ActiveFreeze("aaa");

        // Check and make sure the eh fore color is changed
        sr.IncCounters(System.Drawing.Color.Red, eh.ForeColor, "Failed at eh.ForeColor.", p.log);

        // Hosted control ForeColor should stay 
        sr.IncCounters(BitmapsColorPercent(avBtnBitmap, 40, 40, 10, 10, System.Drawing.Color.Blue) >= 20,
            "Bitmap Failed at avBtn.Foreground=" + System.Drawing.Color.Blue + ". Percent match: " +
            BitmapsColorPercent(avBtnBitmap, 40, 40, 10, 10, System.Drawing.Color.Blue) + "%", p.log); 

        return sr;
    }

    [Scenario("Set forecolor of the EH parent with hosted control foreground set to Blue")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        avBtn.Foreground = System.Windows.Media.Brushes.Blue;
        this.ForeColor = System.Drawing.Color.Red;
        SWF.Application.DoEvents();

        Utilities.SleepDoEvents(2);
        Bitmap avBtnBitmap = CreateBitmap(avBtn);
        //        Utilities.ActiveFreeze("aaa");

        // Check and make sure the eh fore color is changed
        sr.IncCounters(System.Drawing.Color.Red, eh.ForeColor, "Failed at eh.ForeColor.", p.log);

        // Hosted control ForeColor should stay 
        sr.IncCounters(BitmapsColorPercent(avBtnBitmap, 40, 40, 10, 10, System.Drawing.Color.Blue) >= 20,
            "Bitmap Failed at avBtn.Foreground=" + System.Drawing.Color.Blue + ". Percent match: " +
            BitmapsColorPercent(avBtnBitmap, 40, 40, 10, 10, System.Drawing.Color.Blue) + "%", p.log);

        return sr;
    }
    //    {
//        ScenarioResult sr = new ScenarioResult();
//        avBtn.Foreground = System.Windows.Media.Brushes.HotPink;
////       eh.ForeColor = System.Drawing.Color.Transparent;
//        eh.ForeColor = System.Drawing.Color.HotPink;
//        SWF.Application.DoEvents();
//        Utilities.ActiveFreeze("aaa");
//        return sr;
//    }


#endregion
    #region Utilities

    private Bitmap CreateBitmap(object o)
    {
        Bitmap bmp;
        System.Drawing.Point pt = new System.Drawing.Point();
        System.Drawing.Size sz = new System.Drawing.Size();
        if (o.GetType() == typeof(ElementHost))
        {
            ElementHost ctrl = o as ElementHost;
            pt = ctrl.PointToScreen(new System.Drawing.Point(0, 0));
            sz.Width = (int)ctrl.Width;
            sz.Height = (int)ctrl.Height;
        }
        else if (o.GetType() == typeof(System.Windows.Controls.Button))
        {
            System.Windows.Controls.Button ctrl = o as System.Windows.Controls.Button;
            System.Windows.Point wPt = ctrl.PointToScreen(new System.Windows.Point(0, 0));
            pt.X = (int)wPt.X;
            pt.Y = (int)wPt.Y;
            sz.Width = (int)ctrl.ActualWidth;
            sz.Height = (int)ctrl.ActualHeight;
        }
        bmp = new Bitmap(sz.Width, sz.Height);
        Graphics retG = Graphics.FromImage(bmp);
        retG.CopyFromScreen(pt, new System.Drawing.Point(0, 0), sz, CopyPixelOperation.SourceCopy);
        return bmp;
    }

    // BitmapsColorPercent
    // Starting at (x, y), look through the area of pixels in bmp specified by width and height 
    // for color match of specified color.
    // For every pixel that matches specified color, increment match counter.
    // Return percentage of matching pixels to total pixels.
    private static double BitmapsColorPercent(Bitmap bmp, int x, int y, int width, int height, System.Drawing.Color color)
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
//@ Set backcolor of the EH with hosted control background set to White
//@ Set backcolor of the EH with hosted control background set to null
//@ Set backcolor of the EH parent with hosted control background set to White
//@ Set backcolor of the EH parent with hosted control background set to null
//@ Set forecolor of the EH with hosted control foreground set to Blue
//@ Set forecolor of the EH parent with hosted control foreground set to Blue