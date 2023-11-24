using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms.Integration;

using SWF = System.Windows.Forms;
using SD = System.Drawing;

//
// Testcase:    ColorsAndThemes
// Description: Forecolor and backcolor
// Author:      a-wboyde
//

namespace WindowsFormsHostTests
{

public class ColorsAndThemes : WPFReflectBase
{
    #region Testcase setup
    public ColorsAndThemes(string[] args) : base(args) { }

    // class vars
    private StackPanel sp;
    private WindowsFormsHost wfh1;
    private WindowsFormsHost wfh2;
    private SWF.Button wfb1;
    private SWF.Button wfb2;
    private SWF.Button wfb3;
    private Label lab1;

    // open issues !!!
    // Bug 1559016 - WindowsFormsHost -- background color on a WF button (within a System.Windows.Controls.Page object) change after restore from minimize (fixed 6228?)
    // Bug 1538004 - WindowsFormsHost:  Background transparency not working (active)

    // define some color brushes to play with
    private static SolidColorBrush brBlue = Brushes.Blue;
    private static SolidColorBrush brRed = Brushes.Red;
    private static SolidColorBrush brGreen = Brushes.Green;
    private static SolidColorBrush brYellow = Brushes.Yellow;

    // define some gradient brushes
    private static LinearGradientBrush brLinYellowBlue = new LinearGradientBrush(Colors.Yellow, Colors.Blue, 90);
    private static LinearGradientBrush brLinRedGreen = new LinearGradientBrush(Colors.Red, Colors.Green, 0);
    private static RadialGradientBrush brRadGreenYellow = new RadialGradientBrush(Colors.Green, Colors.Yellow);
    private static RadialGradientBrush brRadBlueRed = new RadialGradientBrush(Colors.Blue, Colors.Red);

    // create enum for "brush type" to aid in verification
    private enum myBrushType { Solid, LinearYB, LinearRG, RadialGY, RadialBR };

    // define color mapping structure
    private struct MyColorMapType
    {
        public string testDesc;
        public myBrushType brushType;                   // "type of brush" for verification purpose
        public System.Windows.Media.Brush avBrush;      // Avalon flavored color thing
        public System.Drawing.Color wfColor;            // Matching Winforms flavored color
        public MyColorMapType(string d, myBrushType t, System.Windows.Media.Brush b, System.Drawing.Color c)
        {
            testDesc = d;
            brushType = t;
            avBrush = b;
            wfColor = c;
        }
    }

    // create a list of Avalon colors to try out
    // this is a list of defined (Avalon) "input" values and their corresponding (WinForms) "output" values
    private static MyColorMapType[] MyColorList = {
        new MyColorMapType("Solid Yellow", myBrushType.Solid, brYellow, SD.Color.Yellow),
        new MyColorMapType("Solid Red", myBrushType.Solid, brRed, SD.Color.Red),
        new MyColorMapType("Solid Blue", myBrushType.Solid, brBlue, SD.Color.Blue),
        new MyColorMapType("Solid Green", myBrushType.Solid, brGreen, SD.Color.Green),
        new MyColorMapType("Linear Yellow/Blue", myBrushType.LinearYB, brLinYellowBlue, SD.Color.Yellow),
        new MyColorMapType("Linear Red/Green", myBrushType.LinearRG, brLinRedGreen, SD.Color.Red),
        new MyColorMapType("Radial Green/Yellow", myBrushType.RadialGY, brRadGreenYellow, SD.Color.Green),
        new MyColorMapType("Radial Blue/Red", myBrushType.RadialBR, brRadBlueRed, SD.Color.Blue),
    };

    protected override void InitTest(TParams p)
    {
        this.Topmost = true;
        this.Topmost = false;
        this.Width = 500;
        this.Height = 500;

        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        bool b = base.BeforeScenario(p, scenario);

        // debug - run specific scenario !!!
        //if (scenario.Name != "Scenario5") { return false; }
        
        this.Title = currentScenario.Name;

        return b;
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Set forecolor of the WFH and verify that the child get's it")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        SetUpControls();

        // loop through colors
        foreach (MyColorMapType item in MyColorList)
        {
            p.log.WriteLine("Testing '{0}'", item.testDesc);

            // "Set forecolor of the WFH"
            wfh1.Foreground = item.avBrush;
            wfh2.Foreground = item.avBrush;
            MyPause();

            // "verify that the child get's it"
            VerifyControlForeColor(p, sr, wfb1, item.wfColor);
            VerifyControlForeColor(p, sr, wfb2, item.wfColor);
            VerifyControlForeColor(p, sr, wfb3, item.wfColor);
        }
        return sr;
    }

    [Scenario("Set the backcolor of the WFH and verify that the child get's it")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        SetUpControls();

        // loop through colors
        foreach (MyColorMapType item in MyColorList)
        {
            p.log.WriteLine("Testing '{0}'", item.testDesc);

            // "Set the backcolor of the WFH"
            wfh1.Background = item.avBrush;
            wfh2.Background = item.avBrush;
            //lab1.Background = item.avBrush;         // comparison: how does Avalon Label react?
            MyPause();

            // "verify that the child get's it"
            // Note: BackGround images of WF controls that are in a WF panel DO NOT get set
            if (item.brushType == myBrushType.Solid)
            {
                VerifyControlBackColor(p, sr, wfb1, item.wfColor, item.brushType);
                VerifyControlBackColor(p, sr, wfb2, item.wfColor, item.brushType);
            }
            VerifyControlBackColor(p, sr, wfb3, item.wfColor, item.brushType);
        }
        return sr;
    }

    [Scenario("Set backcolor of the WFH parent and verify that the WFH and it's children get it.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        SetUpControls();

        // loop through colors
        foreach (MyColorMapType item in MyColorList)
        {
            p.log.WriteLine("Testing '{0}'", item.testDesc);

            // "Set backcolor of the WFH parent"
            sp.Background = item.avBrush;
            MyPause();

            // force repaint - see Windows OS Bug 1526236 !!!
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;
            MyPause();

            // "verify that the WFH gets it" !!!
            // Note: WFH.Background will always be null - color "shows through"
            WPFMiscUtils.IncCounters(sr, null, wfh1.Background, "WFH1 not null", p.log);
            WPFMiscUtils.IncCounters(sr, null, wfh2.Background, "WFH2 not null", p.log);

            /***    
             * Bug: When changing the background color while running the application, the 
             * background of the WindowsFormsHost do not change (repaint problem).
             * BugID: WindowsOSBugs 1526236 (Closed as Won't Fix)
            ***/

            // "verify that the children get it"
            // Note: BackGround images of WF controls that are in a WF panel DO NOT get set
            //if (item.brushType == myBrushType.Solid)
            //{
            //    VerifyControlBackColor(p, sr, wfb1, item.wfColor, item.brushType);
            //    VerifyControlBackColor(p, sr, wfb2, item.wfColor, item.brushType);
            //}
            //VerifyControlBackColor(p, sr, wfb3, item.wfColor, item.brushType);
        }
        return sr;
    }

    [Scenario("Set forecolor of the WFH parent and verify that the WFH and it's children get it.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        SetUpControls();

        // loop through colors
        foreach (MyColorMapType item in MyColorList)
        {
            p.log.WriteLine("Testing '{0}'", item.testDesc);

            // "Set forecolor of the WFH parent"
            // Note: you cannot set the Foreground of an Avalon panel, but you *can* set
            // the Foreground of the Avalon window, which is like setting the "WFH parent"
            //sp.Foreground = item.avBrush;
            this.Foreground = item.avBrush;
            MyPause();

            // "verify that the WFH and it's children get it"
            WPFMiscUtils.IncCounters(sr, item.avBrush, wfh1.Foreground, "WFH1 not correct ForeColor", p.log);
            WPFMiscUtils.IncCounters(sr, item.avBrush, wfh2.Foreground, "WFH2 not correct ForeColor", p.log);
            VerifyControlForeColor(p, sr, wfb1, item.wfColor);
            VerifyControlForeColor(p, sr, wfb2, item.wfColor);
            VerifyControlForeColor(p, sr, wfb3, item.wfColor);
        }
        return sr;
    }

    [Scenario("WindowsOSBug 1538004 - Set WFH background transparency and verify it works as expected.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        SetUpControls();

        // set Background of StackPanel to simple Blue brush
        sp.Background = brBlue;

        // "Set WFH background transparency"
        wfh1.Background = Brushes.Transparent;
        wfh2.Background = Brushes.Transparent;

        // "verify it works as expected"
        // vary opacity of WFH, at each opacity level, we need to look for different shade of blue
        for (double op = 0.0; op <= 1.0; op += .2)
        {
            // as opacity varies from 0 to 1, Red and Green components will vary from 255 to 0
            // but Blue component will remain at 255 (lets do some icky math)
            // Note: have to insure range from 0-255 and not 256
            int expR = (int)((1 - op) * (double)256);
            if (expR == 256) { expR = 255; }
            int expG = (int)((1 - op) * (double)256);
            if (expG == 256) { expG = 255; }
            int expB = 255;

            // Note: during development, 80% was a good value, but may need to decrease this if too high
            // Lab machines have less rich color schemes so the colors do not match as well at low Opacity
            // Basically, we want to verify that the color is present to some degree
            TestOpacity(p, sr, op, SD.Color.FromArgb(expR, expG, expB), 10);
        }

        return sr;
    }
    private void TestOpacity(TParams p, ScenarioResult sr, double op, SD.Color expClr, double expPer)
    {
        // set opacity
        p.log.WriteLine("Setting Opacity to = {0}", op);
        p.log.WriteLine("  checking for color {0}", expClr.ToString());
        sp.Opacity = op;

        // Note: need to do this to force WFH to reapply background
        wfh1.PropertyMap.Apply("Background");
        wfh2.PropertyMap.Apply("Background");
        MyPause();

        // get bitmap of control
        this.Topmost = true;
        SD.Bitmap bmpCtrl = Utilities.GetBitmapOfControl(wfb3);
        this.Topmost = false;

        // examine bitmap of WF control, look for specified color
        double actPer = BitmapsColorPercent(bmpCtrl, 0, 0, bmpCtrl.Width, bmpCtrl.Height, expClr);
        p.log.WriteLine("  actPer = {0}", actPer);

        // check concentrations of specified color
        WPFMiscUtils.IncCounters(sr, p.log, actPer > expPer, "Color does not appear correct. " +
		"Expected: >" + expPer + ". Actual: " + actPer);

        // debug !!!
        //bmpCtrl.Save("_control.bmp");
        //Utilities.ActiveFreeze(currentScenario.Name);
        //p.log.WriteLine("");
    }

    #region Helper Functions

    private void SetUpControls()
    {
        // set up controls
        sp = new StackPanel();

        // create some random labels as separators
        lab1 = new Label();
        lab1.Content = "Avalon Label 1";
        lab1.Background = Brushes.LightBlue;
        Label lab2 = new Label();
        lab2.Content = "Avalon Label 2";
        lab2.Background = Brushes.LightGreen;
        Label lab3 = new Label();
        lab3.Content = "Avalon Label 3";
        lab3.Background = Brushes.LightPink;

        // create WFH with panel with multiple WF controls
        wfh1 = new WindowsFormsHost();
        SWF.Panel wfp = new SWF.Panel();

        // create two wf buttons
        wfb1 = new SWF.Button();
        wfb1.Text = "HELLO FROM WF BUTTON 1";
        wfb1.Width = 200;
        wfb2 = new SWF.Button();
        wfb2.Text = "Hello from wf button 2";
        wfb2.Width = 200;

        // put buttons in panel
        wfb2.Top = wfb1.Bottom;
        wfp.Controls.Add(wfb1);
        wfp.Controls.Add(wfb2);
        wfh1.Child = wfp;

        // create WFH with another WF control
        wfh2 = new WindowsFormsHost();

        // note extra spaces in text so that center of button is free of text!
        // this helps when attempting to validate "Radial Gradients"
        wfb3 = new SWF.Button();
        wfb3.Text = "HELLO FROM     WF BUTTON 3";
        wfb3.Width = 200;
        wfh2.Child = wfb3;

        // add stuff to StackPanel
        sp.Children.Add(lab1);
        sp.Children.Add(wfh1);
        sp.Children.Add(lab2);
        sp.Children.Add(wfh2);
        sp.Children.Add(lab3);

        this.Content = sp;
    }

    private static void MyPause()
    {
        WPFReflectBase.DoEvents();
        SWF.Application.DoEvents();
        System.Threading.Thread.Sleep(500);
    }

    private void VerifyControlForeColor(TParams p, ScenarioResult sr, SWF.Control wfCtrl, SD.Color wfClr)
    {
        // check that property is set
        WPFMiscUtils.IncCounters(sr, wfClr.ToArgb(), wfCtrl.ForeColor.ToArgb(),
            "Control.ForeColor property not correct", p.log);

        // examine bitmap of WF control, look for specified color
        this.Topmost = true;
        SD.Bitmap bmpCtrl = Utilities.GetBitmapOfControl(wfCtrl);
        this.Topmost = false;
        WPFMiscUtils.IncCounters(sr, p.log, Utilities.ContainsColor(bmpCtrl, wfClr),
            "Cannot find any specified color in control");
    }

    private void VerifyControlBackColor(TParams p, ScenarioResult sr, SWF.Control wfCtrl, SD.Color wfClr, myBrushType brush)
    {
        bool b;

        // get rectangle of our WF control, being careful to ignore 2-pixel border
        SD.Rectangle wfRect = new SD.Rectangle(2, 2,
            wfCtrl.ClientRectangle.Width - 4, wfCtrl.ClientRectangle.Height - 4);

        // convert to screen coordinates, get bitmap of WF control
        SD.Rectangle wfRectSc = wfCtrl.RectangleToScreen(wfRect);
        this.Topmost = true;
        SD.Bitmap bmpCtrl = Utilities.GetScreenBitmap(wfRectSc);
        this.Topmost = false;

        // debug !!!
        //bmpCtrl.Save("_control.bmp");
        //Utilities.ActiveFreeze(currentScenario.Name);

        // vars to use for when looking at center of image (4 x 4 pixel chunk at center)
        int midX = (wfRect.Width / 2) - 2;
        int midY = (wfRect.Height / 2) - 2;

        // common vars
        double pcMid;
        double pcCorUL;
        double pcCorUR;
        double pcCorLL;
        double pcCorLR;

        // how we check the "BackColor" depends on the kind of brush we are using
        // "solid" color brushes only affect WF.BackColor property
        // non "solid" brushes only affect WF.BackgroundImage property
        switch (brush)
        {
            case myBrushType.Solid:
                // BackColor property will be set
                // check that BackColor property is set
                p.log.WriteLine("  checking control BackColor for {0}", wfClr);
                WPFMiscUtils.IncCounters(sr, wfClr.ToArgb(), wfCtrl.BackColor.ToArgb(),
                    "Control.BackColor property not correct", p.log);

                // examine bitmap of WF control, look for specified color
                string bstring = brush.ToString();
                bmpCtrl.Save(bstring+".bmp");
                p.log.WriteLine("  checking control bitmap for {0}", wfClr);
                WPFMiscUtils.IncCounters(sr, p.log, Utilities.ContainsColor(bmpCtrl, wfClr),
                    "Cannot find any specified color in control");
                break;

            case myBrushType.LinearRG:
                // BackColor property will not be set
                // color changes from red (left) to green (right)
                double pcR = BitmapsColorPercent(bmpCtrl, 0, 0, 5, wfRect.Height, SD.Color.Red);
                double pcG = BitmapsColorPercent(bmpCtrl, wfRect.Width - 5, 0, 5, wfRect.Height, SD.Color.Green);
                p.log.WriteLine("  pcR = {0} pcG = {1}", pcR, pcG);

                // pass if both sections match
                b = (PercentageInRange(pcR, 100, 100) && PercentageInRange(pcG, 100, 100));
                WPFMiscUtils.IncCounters(sr, p.log, b, "Backcolor does not look like horizontal gradient");
                break;

            case myBrushType.LinearYB:
                // BackColor property will not be set
                // color changes from yellow (top) to blue (bottom)
                // (control is short and wide, so just look at one row)
                // (also, since distance is so short, colors change rapidly, need to use own own yellow/blue)
                double pcY = BitmapsColorPercent(bmpCtrl, 0, 0, wfRect.Width, 1, SD.Color.FromArgb(230,230,26));
                double pcB = BitmapsColorPercent(bmpCtrl, 0, wfRect.Height - 1, wfRect.Width, 1, SD.Color.FromArgb(26,26,229));
                p.log.WriteLine("  pcY = {0} pcB = {1}", pcY, pcB);

                // pass if both sections match at least 50%
                b = (PercentageInRange(pcY, 50, 100) && PercentageInRange(pcB, 50, 100));
                WPFMiscUtils.IncCounters(sr, p.log, b, "Backcolor does not look like vertical gradient");
                break;

            case myBrushType.RadialBR:
                // BackColor property will not be set
                // color changes from blue (middle) to red (corners)

                // middle (4 pixels around center) color + black text
                pcMid = BitmapsColorPercent(bmpCtrl, midX, midY, 4, 4, SD.Color.Blue);

                // upper left
                pcCorUL = BitmapsColorPercent(bmpCtrl, 0, 0, 6, 6, SD.Color.Red);

                // upper right
                pcCorUR = BitmapsColorPercent(bmpCtrl, wfRect.Width - 6, 0, 6, 6, SD.Color.Red);

                // lower left
                pcCorLL = BitmapsColorPercent(bmpCtrl, 0, wfRect.Height - 6, 6, 6, SD.Color.Red);

                // lower right
                pcCorLR = BitmapsColorPercent(bmpCtrl, wfRect.Width - 6, wfRect.Height - 6, 6, 6, SD.Color.Red);

                p.log.WriteLine("  pcMid = {0} pcG = {1}/{2}/{3}/{4}", pcMid, pcCorUL, pcCorUR, pcCorLL, pcCorLR);
                bmpCtrl.Save("c.bmp");
                // pass if center and all corners match
                // (Note: center area varies so much, just need to check for at least 20% correct color)
                b = (PercentageInRange(pcMid, 20, 100) &&
                    PercentageInRange(pcCorUL, 100, 100) &&
                    PercentageInRange(pcCorUR, 100, 100) &&
                    PercentageInRange(pcCorLL, 100, 100) &&
                    PercentageInRange(pcCorLR, 100, 100));
                WPFMiscUtils.IncCounters(sr, p.log, b, "Backcolor does not look like radial gradient");
                break;

            case myBrushType.RadialGY:
                // BackColor property will not be set
                // color changes from green (middle) to yellow (corners)

                // middle (4 pixels around center) color + black text
                pcMid = BitmapsColorPercent(bmpCtrl, midX, midY, 4, 4, SD.Color.Green);

                // upper left
                pcCorUL = BitmapsColorPercent(bmpCtrl, 0, 0, 6, 6, SD.Color.Yellow);

                // upper right
                pcCorUR = BitmapsColorPercent(bmpCtrl, wfRect.Width - 6, 0, 6, 6, SD.Color.Yellow);

                // lower left
                pcCorLL = BitmapsColorPercent(bmpCtrl, 0, wfRect.Height - 6, 6, 6, SD.Color.Yellow);

                // lower right
                pcCorLR = BitmapsColorPercent(bmpCtrl, wfRect.Width - 6, wfRect.Height - 6, 6, 6, SD.Color.Yellow);

                p.log.WriteLine("  pcMid = {0} pcG = {1}/{2}/{3}/{4}", pcMid, pcCorUL, pcCorUR, pcCorLL, pcCorLR);

                // pass if center and all corners match
                // (Note: center area varies so much, just need to check for at least 20% correct color)
                b = (PercentageInRange(pcMid, 20, 100) &&
                    PercentageInRange(pcCorUL, 100, 100) &&
                    PercentageInRange(pcCorUR, 100, 100) &&
                    PercentageInRange(pcCorLL, 100, 100) &&
                    PercentageInRange(pcCorLR, 100, 100));
                WPFMiscUtils.IncCounters(sr, p.log, b, "Backcolor does not look like radial gradient");
                break;

            default:
                // Unknown Brush Type?  That's just not right!  fail TC with error.
                WPFMiscUtils.IncCounters(sr, p.log, false, "Testcase error - Unknown Brush Type");
                break;
        }
    }

    /// <summary>
    /// Helper function to decide if double value is within specified range
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="low"></param>
    /// <param name="high"></param>
    /// <returns></returns>
    private static bool PercentageInRange(double pc, double low, double high)
    {
        if (pc < low) { return false; }
        if (pc > high) { return false; }
        return true;
    }

    // BitmapsColorPercent
    // Starting at (x, y), look through the area of pixels in bmp specified by width and height 
    // for color match of specified color.
    // For every pixel that matches specified color, increment match counter.
    // Return percentage of matching pixels to total pixels.
    private static double BitmapsColorPercent(SD.Bitmap bmp, int x, int y, int width, int height, SD.Color color)
    {
        double total = 0;
        double match = 0;
        for (int col = x; col < x + width; col++)
        {
            for (int row = y; row < y + height; row++)
            {
                if (Utilities.ColorsMatch(bmp.GetPixel(col, row), color, 30))
                {
                    match++;
                }
                total++;
            }
        }
        return match * 100 / total;
    }

    #endregion

    #endregion
}

}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Set forecolor of the WFH and verify that the child get's it

//@ Set the backcolor of the WFH and verify that the child get's it

//@ Set backcolor of the WFH parent and verify that the WFH and it's children get it.

//@ Set forecolor of the WFH parent and verify that the WFH and it's children get it.

//@ WindowsOSBug 1538004 - Set WFH background transparency and verify it works as expected.