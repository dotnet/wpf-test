using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Reflection;

using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWD = System.Windows.Documents;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;
using SWS = System.Windows.Shapes;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;

//
// Testcase:    MouseFocus
// Description: Verify that clicking on either WF or AV control, Focus is correct
// Author:      a-rickyt
//

namespace WindowsFormsHostTests
{

public class MouseFocus : WPFReflectBase
{
    #region Testcase setup
    
    Edit _edit1;
    Edit _edit2;
    SWC.StackPanel stackPanel = new SWC.StackPanel();
    WindowsFormsHost winformsHost1 = new WindowsFormsHost();
    SWF.Button wfButton1 = new SWF.Button();
    SWF.Button wfButton2 = new SWF.Button();
    SWF.FlowLayoutPanel panel = new SWF.FlowLayoutPanel();
    SWC.Button avButton1 = new SWC.Button();
    SWC.Button avButton2 = new SWC.Button();
    SWC.Label label1 = new SWC.Label();
    SWC.Label label2 = new SWC.Label();
    String labelText1;
    String labelText2;
    SW.Window window = new SW.Window();

    public MouseFocus(string[] args) : base(args) { }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        if (scenario.Name == "Scenario1")
        {
            avButton1.Name = "avButton1";
            avButton1.Content = "Avalon Button";
            avButton1.GotFocus += new System.Windows.RoutedEventHandler(avButton1_GotFocus);
            avButton1.LostFocus += new System.Windows.RoutedEventHandler(avButton1_LostFocus);
            stackPanel.Children.Add(avButton1);
        }

        if (scenario.Name == "Scenario2")
        {
            avButton2.Name = "avButton2";
            avButton2.Content = "AvalonWindow";
            avButton2.GotKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(avButton2_GotFocus);
            avButton2.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(avButton2_LostFocus);
            window.Content = avButton2;

            window.Name = "AvalonWindow";
            window.Title = "AvalonWindow";
            window.SizeToContent = SW.SizeToContent.WidthAndHeight;
            window.Top = 0;
            window.Left = 0;
            window.Topmost = true;
            window.Show();

            Utilities.SleepDoEvents(10);
        }

        if (scenario.Name == "Scenario3" || scenario.Name == "Scenario4")
        {
            panel.Controls.Clear();

            wfButton2.Name = "wfButton2";
            wfButton2.Text = "WinFormButton2";
            wfButton2.GotFocus += new EventHandler(wfButton2_GotFocus);
            wfButton2.LostFocus += new EventHandler(wfButton2_LostFocus);

            winformsHost1.Child = panel;
            panel.Controls.Add(wfButton1);
            panel.Controls.Add(wfButton2);
        }

        if (scenario.Name == "Scenario4")
        {
            wfButton2.Capture = true;
        }
        return base.BeforeScenario(p, scenario);
    }

    void wfButton2_LostFocus(object sender, EventArgs e)
    {
        label2.Content = labelText2 = "WinFormButton2 lost focus.";
    }

    void wfButton2_GotFocus(object sender, EventArgs e)
    {
        label1.Content = labelText1 = "WinFormButton2 got focus.";
    }

    void avButton2_LostFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        label2.Content = labelText2 = "Avalon Button lost focus.";
    }

    void avButton2_GotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        label1.Content = labelText1 = "Avalon Button got focus.";
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        if (scenario.Name == "Scenario1")
        {
            stackPanel.Children.Remove(avButton1);
        }
        if (scenario.Name == "Scenario2")
        {
            window.Close();
        }
        label1.Content = "";
        label2.Content = "";
        labelText1 = "";
        labelText2 = "";
        base.AfterScenario(p, scenario, result);
    }

    protected override void InitTest(TParams p) 
    {
        this.Title = "MouseFocus";
        this.UseMITA = true;
        this.Topmost = true;

        label1.Height = 30;
        label2.Height = 30;

        wfButton1.Name = "wfButton1";
        wfButton1.Text = "WinForm Button";
        wfButton1.GotFocus += new EventHandler(wfButton1_GotFocus);
        wfButton1.LostFocus += new EventHandler(wfButton1_LostFocus);
        winformsHost1.Child = wfButton1;

        stackPanel.Children.Add(label1);
        stackPanel.Children.Add(label2);
        stackPanel.Children.Add(winformsHost1);

        this.Content = stackPanel;

        base.InitTest(p);
    }

    void avButton1_LostFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        label2.Content = labelText2 = "Avalon Button lost focus.";
    }

    void avButton1_GotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        label1.Content = labelText1 = "Avalon Button got focus.";
    }

    void wfButton1_LostFocus(object sender, EventArgs e)
    {
        label2.Content = labelText2 = "WinForm Button lost focus.";
    }

    void wfButton1_GotFocus(object sender, EventArgs e)
    {
        label1.Content = labelText1 = "WinForm Button got focus.";
    }
    #endregion


    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios

    [Scenario("Click back and forth between a WF and AV control and verify that Focus is correct.")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "MouseFocus", "wfButton1", "MouseFocus", "avButton1"))
        {
            return new ScenarioResult(false);
        }
        Utilities.SleepDoEvents(10);

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText1 == "WinForm Button got focus.",
            "Failed at GotFocus on WinForm button.\nExpected: WinForm Button got focus." +
            "\nActual: " + labelText1);

        _edit2.Click(PointerButtons.Primary); //click on Avalon button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus, 
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText1 == "Avalon Button got focus.",
            "Failed at GotFocus on Avalon button.\nExpected: Avalon Button got focus." +
            "\nActual: " + labelText1);

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText2 == "Avalon Button lost focus.",
            "Failed at LostFocus on Avalon button.\nExpected: Avalon Button lost focus." +
            "\nActual: " + labelText2);

        _edit2.Click(PointerButtons.Primary); //click on Avalon button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText2 == "WinForm Button lost focus.",
            "Failed at LostFocus on WinForm button.\nExpected: WinForm Button lost focus." + 
            "\nActual: " + labelText2);

        Utilities.SleepDoEvents(10);

        return sr; 
    }

    [Scenario("Click back and forth between a AV control on a separate Window and an WF control and verify that focus is correct.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "MouseFocus", "wfButton1", "AvalonWindow", "avButton2"))
        {
            return new ScenarioResult(false);
        }
        Utilities.SleepDoEvents(10);

        _edit1.SetFocus();
        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText1 == "WinForm Button got focus.",
            "Failed at GotFocus on WinForm button.\nExpected: WinForm Button got focus." +
            "\nActual: " + labelText1);

        _edit2.Click(PointerButtons.Primary); //click on Avalon button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText1 == "Avalon Button got focus.",
            "Failed at GotFocus on Avalon button.\nExpected: Avalon Button got focus." +
            "\nActual: " + labelText1);

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText2 == "Avalon Button lost focus.",
            "Failed at LostFocus on Avalon button.\nExpected: Avalon Button lost focus." +
            "\nActual: " + labelText2);

        _edit2.Click(PointerButtons.Primary); //click on Avalon button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText2 == "WinForm Button lost focus.",
            "Failed at LostFocus on WinForm button.\nExpected: WinForm Button lost focus." +
            "\nActual: " + labelText2);

        Utilities.SleepDoEvents(10);


        return sr;
    }

    [Scenario("Click back and forth between 2 WF controls in the same WFH and verify that Focus is correct.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "MouseFocus", "wfButton1", "MouseFocus", "wfButton2"))
        {
            return new ScenarioResult(false);
        }
        Utilities.SleepDoEvents(10);

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText1 == "WinForm Button got focus.",
            "Failed at GotFocus on WinForm button.\nExpected: WinForm Button got focus." +
            "\nActual: " + labelText1);

        _edit2.Click(PointerButtons.Primary); //click on WinFormButton2
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText1 == "WinFormButton2 got focus.",
            "Failed at GotFocus on WinFormButton2.\nExpected: WinFormButton2 got focus." +
            "\nActual: " + labelText1);

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText2 == "WinFormButton2 lost focus.",
            "Failed at LostFocus on WinFormButton2.\nExpected: WinFormButton2 lost focus." +
            "\nActual: " + labelText2);

        _edit2.Click(PointerButtons.Primary); //click on WinFormButton2
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText2 == "WinForm Button lost focus.",
            "Failed at LostFocus on WinForm button.\nExpected: WinForm Button lost focus." +
            "\nActual: " + labelText2);

        Utilities.SleepDoEvents(10);

        return sr;
    }

    [Scenario("MouseCapture Control.Capture should work.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        
        if (!GetEditControls(p, "MouseFocus", "wfButton1", "MouseFocus", "wfButton2"))
        {
            return new ScenarioResult(false);
        }
        Utilities.SleepDoEvents(10);

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        //wfButton2.Capture has been set to true
        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus == false,
            "Failed at wfButton1.HasKeyboardFocus==false. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText1 == "WinFormButton2 got focus.",
            "Failed at GotFocus on WinFormButton2.\nExpected: WinFormButton2 got focus." +
            "\nActual: " + labelText1);

        _edit2.Click(PointerButtons.Primary); //click on WinFormButton2
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText1 == "WinFormButton2 got focus.",
            "Failed at GotFocus on WinFormButton2.\nExpected: WinFormButton2 got focus." +
            "\nActual: " + labelText1);

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText2 == "WinFormButton2 lost focus.",
            "Failed at LostFocus on WinFormButton2.\nExpected: WinFormButton2 lost focus." +
            "\nActual: " + labelText2);

        _edit2.Click(PointerButtons.Primary); //click on WinFormButton2
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        WPFMiscUtils.IncCounters(sr, p.log, labelText2 == "WinForm Button lost focus.",
            "Failed at LostFocus on WinForm button.\nExpected: WinForm Button lost focus." +
            "\nActual: " + labelText2);

        Utilities.SleepDoEvents(10);

        return sr;
    }
    #endregion

    #region Helper Functions 

    bool GetEditControls(TParams p, string window1, string control1, string window2, string control2)
    {
        try
        {
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window1));
            BreadthFirstDescendantsNavigator bfTB1 = new BreadthFirstDescendantsNavigator(uiApp, UICondition.CreateFromId(control1));
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window2));
            BreadthFirstDescendantsNavigator bfTB2 = new BreadthFirstDescendantsNavigator(uiApp, UICondition.CreateFromId(control2));

            _edit1 = new Edit(bfTB1[0]);
            _edit2 = new Edit(bfTB2[0]);
            return true;
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to get Mita wrapper controls. " + ex.ToString());
            return false;
        }
    }
  
    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Click back and forth between a WF and AV control and verify that Focus is correct.
//@ Click back and forth between a AV control on a separate Window and an WF control and verify that focus is correct.
//@ Click back and forth between 2 WF controls in the same WFH and verify that Focus is correct.
//@ MouseCapture Control.Capture should work.
