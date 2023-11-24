using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Microsoft.Test.Display;


//
// Testcase:    PaddingsAndMargins
// Description: Verify that setting the padding and margins on a WFH flow to the WF ctrl
// Author:      a-wboyde
//
namespace WindowsFormsHostTests
{

public class PaddingsAndMargins : WPFReflectBase
{
    // class vars
    private StackPanel sp;
    private WindowsFormsHost wfh1;
    private SWF.Button wfBtn;
    private WindowsFormsHost wfh2;
    private SWF.Button wfBtn2;
    private SD.Rectangle origRect;
    private SD.Rectangle origRect2;

    #region Testcase setup
    public PaddingsAndMargins(string[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        bool b = base.BeforeScenario(p, scenario);

        // debug - run specific scenario !!!
        //if (scenario.Name != "Scenario1") { return false; }

        this.Width = 300;
        this.Height = 300;
        CreateTestControls();
        this.Title = scenario.Name;
        this.Topmost = true;
        this.Topmost = false;

        return b;
    }

    protected override void AfterScenario(TParams p, System.Reflection.MethodInfo scenario, ScenarioResult result)
    {
        //Utilities.ActiveFreeze(currentScenario.Name);
        base.AfterScenario(p, scenario, result);
    }

    private void CreateTestControls()
    {
        // create panel
        sp = new StackPanel();

        // add button before the host
        Button avBtn1 = new Button();
        avBtn1.Content = "Avalon Button 1";
        sp.Children.Add(avBtn1);

        // create WF host control, add to panel
        wfh1 = new WindowsFormsHost();
        wfh1.Background = System.Windows.Media.Brushes.Yellow;
        sp.Children.Add(wfh1);

        // create WF Button
        wfBtn = new SWF.Button();
        wfBtn.Text = "Here I am!";
        wfh1.Child = wfBtn;

        // add panel to window
        this.Background = System.Windows.Media.Brushes.Beige;
        this.Content = sp;

        // create second host with WF button
        wfh2 = new WindowsFormsHost();
        wfBtn2 = new SWF.Button();
        wfBtn2.Text = "I can see that.";
        wfh2.Child = wfBtn2;
        wfh2.Background = System.Windows.Media.Brushes.LightSkyBlue;
        sp.Children.Add(wfh2);

        Utilities.SleepDoEvents(10);

        // we are now in default state
        // record current size, position
        origRect = wfBtn.RectangleToScreen(wfBtn.ClientRectangle);
        origRect2 = wfBtn2.RectangleToScreen(wfBtn2.ClientRectangle);
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("1) Set the top padding on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial padding properties
        VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

        // "Set the top padding on the WFH"
        wfh1.Padding = new Thickness(0d, 50d, 0d, 0d);
        Utilities.SleepDoEvents(10);

        // Check padding properties
        VerifyPadding(p, sr, 0d, 50d, 0d, 0d);

        return sr;
    }

    [Scenario("2) Set the bottom padding on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial padding properties
        VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

        // "Set the bottom padding on the WFH"
        wfh1.Padding = new Thickness(0d, 0d, 0d, 50d);
        Utilities.SleepDoEvents(10);

        // Check padding properties
        VerifyPadding(p, sr, 0d, 0d, 0d, 50d);

        return sr;
    }

    [Scenario("3) Set the left padding on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial padding properties
        VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

        // "Set the left padding on the WFH"
        wfh1.Padding = new Thickness(50d, 0d, 0d, 0d);
        Utilities.SleepDoEvents(10);

        // Check padding properties
        VerifyPadding(p, sr, 50d, 0d, 0d, 0d);

        return sr;
    }

    [Scenario("4) Set the right padding on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial padding properties
        VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

        // "Set the right padding on the WFH"
        wfh1.Padding = new Thickness(0d, 0d, 50d, 0d);
        Utilities.SleepDoEvents(10);

        // Check padding properties
        VerifyPadding(p, sr, 0d, 0d, 50d, 0d);

        return sr;
    }

    [Scenario("5) Set the top margin on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial margin properties
        VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

        // "Set the top margin on the WFH"
        wfh1.Margin = new Thickness(0d, 50d, 0d, 0d);
        Utilities.SleepDoEvents(10);

        // Check margin properties
        VerifyMargin(p, sr, 0d, 50d, 0d, 0d);

        return sr;
    }

    [Scenario("6) Set the bottom margin on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial margin properties
        VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

        // "Set the bottom margin on the WFH"
        wfh1.Margin = new Thickness(0d, 0d, 0d, 50d);
        Utilities.SleepDoEvents(10);

        // Check margin properties
        VerifyMargin(p, sr, 0d, 0d, 0d, 50d);

        return sr;
    }

    [Scenario("7) Set the left margin on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial margin properties
        VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

        // "Set the left margin on the WFH"
        wfh1.Margin = new Thickness(50d, 0d, 0d, 0d);
        Utilities.SleepDoEvents(10);

        // Check margin properties
        VerifyMargin(p, sr, 50d, 0d, 0d, 0d);

        return sr;
    }

    [Scenario("8) Set the right margin on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial margin properties
        VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

        // "Set the right margin on the WFH"
        wfh1.Margin = new Thickness(0d, 0d, 50d, 0d);
        Utilities.SleepDoEvents(10);

        // Check margin properties
        VerifyMargin(p, sr, 0d, 0d, 50d, 0d);

        return sr;
    }

    [Scenario("9) Set the padding (all) on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial padding properties
        VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

        // "Set the padding on the WFH"
        wfh1.Padding = new Thickness(5d, 10d, 15d, 20d);
        Utilities.SleepDoEvents(10);

        // Check padding properties
        VerifyPadding(p, sr, 5d, 10d, 15d, 20d);

        // "Set the padding on the WFH"
        wfh1.Padding = new Thickness(30d);
        Utilities.SleepDoEvents(10);

        // Check padding properties
        VerifyPadding(p, sr, 30d, 30d, 30d, 30d);

        return sr;
    }

    [Scenario("10) Set the margin (all) on the WFH and verify that the WF Control honors the setting")]
    public ScenarioResult Scenario10(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // Check initial margin properties
        VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

        // "Set the margin on the WFH"
        wfh1.Margin = new Thickness(5d, 10d, 15d, 20d);
        Utilities.SleepDoEvents(10);

        // Check margin properties
        VerifyMargin(p, sr, 5d, 10d, 15d, 20d);

        // "Set the margin on the WFH"
        wfh1.Margin = new Thickness(30d);
        Utilities.SleepDoEvents(10);

        // Check margin properties
        VerifyMargin(p, sr, 30d, 30d, 30d, 30d);

        return sr;
    }

    #region Helper functions

    private void VerifyPadding(TParams p, ScenarioResult sr, double left, double top, double right, double bottom)
    {
        // Not only do we need to verify that the Padding settings in the WFH and the WF control are set
        // properly, we also need to actually "look" at where the control was positioned on the screen.
        // We also need to check the control that was "pushed out of the way" by changes in our main WFH.

        // check WFH
        WPFMiscUtils.IncCounters(sr, left, wfh1.Padding.Left, "Host Left Padding not correct", p.log);
        WPFMiscUtils.IncCounters(sr, top, wfh1.Padding.Top, "Host Top Padding not correct", p.log);
        WPFMiscUtils.IncCounters(sr, right, wfh1.Padding.Right, "Host Right Padding not correct", p.log);
        WPFMiscUtils.IncCounters(sr, bottom, wfh1.Padding.Bottom, "Host Bottom Padding not correct", p.log);

        // check WF Control (Padding should match what WFH is set to)
        WPFMiscUtils.IncCounters(sr, left, (double)wfBtn.Padding.Left, "Control Left Padding not correct", p.log);
        WPFMiscUtils.IncCounters(sr, top, (double)wfBtn.Padding.Top, "Control Top Padding not correct", p.log);
        WPFMiscUtils.IncCounters(sr, right, (double)wfBtn.Padding.Right, "Control Right Padding not correct", p.log);
        WPFMiscUtils.IncCounters(sr, bottom, (double)wfBtn.Padding.Bottom, "Control Bottom Padding not correct", p.log);

        // where is our control now?
        SD.Rectangle curRect = wfBtn.RectangleToScreen(wfBtn.ClientRectangle);
        //p.log.WriteLine("origRect  = {0}", origRect.ToString());
        //p.log.WriteLine("curRect   = {0}", curRect.ToString());

        // compare current location with where we think it should be
        // (position, width should be unaffected (in stackpanel), height is affected by top/bottom)
        WPFMiscUtils.IncCounters(sr, origRect.X, curRect.X, "Control X not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect.Y, curRect.Y, "Control Y not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect.Width, curRect.Width, "Control Width not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect.Height + (int)top + (int)bottom, curRect.Height, "Control Height not correct", p.log);

        // what about the control following it?
        SD.Rectangle curRect2 = wfBtn2.RectangleToScreen(wfBtn2.ClientRectangle);
        //p.log.WriteLine("origRect2 = {0}", origRect2.ToString());
        //p.log.WriteLine("curRect2  = {0}", curRect2.ToString());

        // compare current location with where we think it should be
        // (Y position is affected by top and bottom, width and height should not change)
        WPFMiscUtils.IncCounters(sr, origRect2.X, curRect2.X, "Control2 X not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect2.Y + (int)top + (int)bottom, curRect2.Y, "Control2 Y not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect2.Width, curRect2.Width, "Control2 Width not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect2.Height, curRect2.Height, "Control2 Height not correct", p.log);
    }

    private void VerifyMargin(TParams p, ScenarioResult sr, double left, double top, double right, double bottom)
    {
        // Not only do we need to verify that the Margin settings in the WFH and the WF control are set
        // properly, we also need to actually "look" at where the control was positioned on the screen.
        // We also need to check the control that was "pushed out of the way" by changes in our main WFH.

        // check WFH
        WPFMiscUtils.IncCounters(sr, left, wfh1.Margin.Left, "Host Left Margin not correct", p.log);
        WPFMiscUtils.IncCounters(sr, top, wfh1.Margin.Top, "Host Top Margin not correct", p.log);
        WPFMiscUtils.IncCounters(sr, right, wfh1.Margin.Right, "Host Right Margin not correct", p.log);
        WPFMiscUtils.IncCounters(sr, bottom, wfh1.Margin.Bottom, "Host Bottom Margin not correct", p.log);

        // check WF Control
        WPFMiscUtils.IncCounters(sr, 0, wfBtn.Margin.Left, "Control Left Margin not correct", p.log);
        WPFMiscUtils.IncCounters(sr, 0, wfBtn.Margin.Top, "Control Top Margin not correct", p.log);
        WPFMiscUtils.IncCounters(sr, 0, wfBtn.Margin.Right, "Control Right Margin not correct", p.log);
        WPFMiscUtils.IncCounters(sr, 0, wfBtn.Margin.Bottom, "Control Bottom Margin not correct", p.log);

        // where is our control now?
        SD.Rectangle curRect = wfBtn.RectangleToScreen(wfBtn.ClientRectangle);
        //p.log.WriteLine("origRect  = {0}", origRect.ToString());
        //p.log.WriteLine("curRect   = {0}", curRect.ToString());

        // compare current location with where we think it should be
        // (position is affected by left and top, width is affected by left and right, height should not change)
        WPFMiscUtils.IncCounters(sr, origRect.X + (int)left, curRect.X, "Control X not correct", p.log);
        WPFMiscUtils.IncCounters(sr, (origRect.Y + Math.Round(Monitor.ConvertLogicalToScreen(Dimension.Height, top))).ToString(), curRect.Y.ToString(), "Control Y not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect.Width - (int)left - (int)right, curRect.Width, "Control Width not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect.Height, curRect.Height, "Control Height not correct", p.log);

        // what about the control following it?
        SD.Rectangle curRect2 = wfBtn2.RectangleToScreen(wfBtn2.ClientRectangle);
        //p.log.WriteLine("origRect2 = {0}", origRect2.ToString());
        //p.log.WriteLine("curRect2  = {0}", curRect2.ToString());

        // compare current location with where we think it should be
        // (Y position is affected by top and bottom, width and height should not change)
        WPFMiscUtils.IncCounters(sr, origRect2.X, curRect2.X, "Control2 X not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect2.Y + (int)Math.Round(Monitor.ConvertLogicalToScreen(Dimension.Height, top)) + (int)bottom, curRect2.Y, "Control2 Y not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect2.Width, curRect2.Width, "Control2 Width not correct", p.log);
        WPFMiscUtils.IncCounters(sr, origRect2.Height, curRect2.Height, "Control2 Height not correct", p.log);
    }
    #endregion

    #endregion
}
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Set the top padding on the WFH and verify that the WF Control honors the setting

//@ 2) Set the bottom padding on the WFH and verify that the WF Control honors the setting

//@ 3) Set the left padding on the WFH and verify that the WF Control honors the setting

//@ 4) Set the right padding on the WFH and verify that the WF Control honors the setting

//@ 5) Set the top margin on the WFH and verify that the WF Control honors the setting

//@ 6) Set the bottom margin on the WFH and verify that the WF Control honors the setting

//@ 7) Set the left margin on the WFH and verify that the WF Control honors the setting

//@ 8) Set the right margin on the WFH and verify that the WF Control honors the setting

//@ 9) Set the padding (all) on the WFH and verify that the WF Control honors the setting

//@ 10) Set the margin (all) on the WFH and verify that the WF Control honors the setting

