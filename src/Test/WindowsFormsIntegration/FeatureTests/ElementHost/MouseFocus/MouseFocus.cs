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
using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;
using System.Threading;
using System.Reflection;

//
// Testcase:    MouseFocus
// Description: Verify that when clicking on either WF or AV control, Focus is correct
// Author:      a-rickyt
//
public class MouseFocus : ReflectBase
{
    #region Testcase setup

    Edit _edit1;
    Edit _edit2;
    SWF.Button wfButton = new SWF.Button();
    SWC.Button avButton = new SWC.Button();
    SWC.Button avOkButton = new SWC.Button();
    SWC.Button avCancelButton = new SWC.Button();
    SWC.StackPanel stackPanel = new SWC.StackPanel();
    ElementHost elementHost1 = new ElementHost();
    ElementHost elementHost2 = new ElementHost();
    public SWF.Label label1 = new SWF.Label();
    public SWF.Label label2 = new SWF.Label();
    MouseWinFormWindow winform;

    public MouseFocus(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "MouseFocus";
        this.Size = new System.Drawing.Size(400, 400);

        label1.AutoSize = true;
        label1.Location = new System.Drawing.Point(100, 150);
        Controls.Add(label1);
        label2.AutoSize = true;
        label2.Location = new System.Drawing.Point(100, 180);
        Controls.Add(label2);

        wfButton.Name = "wfButton";
        wfButton.Text = "WinForm Button";
        wfButton.AutoSize = true;
        wfButton.Location = new System.Drawing.Point(100, 300);
        wfButton.GotFocus += new EventHandler(wfButton_GotFocus);
        wfButton.LostFocus += new EventHandler(wfButton_LostFocus);
        Controls.Add(wfButton);

        avButton.Name = "avButton";
        avButton.Content = "Avalon Button";
        avButton.GotFocus += new RoutedEventHandler(avButton_GotFocus);
        avButton.LostFocus += new RoutedEventHandler(avButton_LostFocus);

        avOkButton.Name = "avOkButton";
        avOkButton.Content = "Avalon OK Button";
        avOkButton.GotFocus += new RoutedEventHandler(avOkButton_GotFocus);
        avOkButton.LostFocus += new RoutedEventHandler(avOkButton_LostFocus);

        avCancelButton.Name = "avCancelButton";
        avCancelButton.Content = "Avalon Cancel Button";
        avCancelButton.GotFocus += new RoutedEventHandler(avCancelButton_GotFocus);
        avCancelButton.LostFocus += new RoutedEventHandler(avCancelButton_LostFocus);

        stackPanel.Children.Add(avOkButton);
        stackPanel.Children.Add(avCancelButton);
        stackPanel.Orientation = SWC.Orientation.Horizontal;

        //Create Element Host 1
        elementHost1.Name = "elementHost1";
        elementHost1.Child = avButton;
        elementHost1.Size = new System.Drawing.Size(150, 100);
        elementHost1.Location = new System.Drawing.Point(100, 20);
        elementHost1.BackColor = Color.Red;
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2.Name = "elementHost2";
        elementHost2.Child = stackPanel; 
        elementHost2.Location = new System.Drawing.Point(100, 230);
        elementHost2.BackColor = Color.Red;
        elementHost2.AutoSize = true;
        Controls.Add(elementHost2);

        base.InitTest(p);
    }

    void wfButton_GotFocus(object sender, EventArgs e)
    {
        label1.Text = "WinForm Button got focus.";
    }
    void wfButton_LostFocus(object sender, EventArgs e)
    {
        label1.Text = "WinForm Button lost focus.";
    }

    void avButton_GotFocus(object sender, RoutedEventArgs e)
    {
        label2.Text = "Avalon Button got focus.";
    }
    void avButton_LostFocus(object sender, RoutedEventArgs e)
    {
        label2.Text = "Avalon Button lost focus.";
    }

    void avOkButton_GotFocus(object sender, RoutedEventArgs e)
    {
        label1.Text = "Avalon OK Button got focus.";
    }
    void avOkButton_LostFocus(object sender, RoutedEventArgs e)
    {
        label1.Text = "Avalon OK Button lost focus.";
    }

    void avCancelButton_GotFocus(object sender, RoutedEventArgs e)
    {
        label2.Text = "Avalon Cancel Button got focus.";
    }
    void avCancelButton_LostFocus(object sender, RoutedEventArgs e)
    {
        label2.Text = "Avalon Cancel Button lost focus.";
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        if (scenario.Name == "Scenario2")
        {
            winform = new MouseWinFormWindow(this);
            winform.Show();
        }
        else if (scenario.Name == "Scenario4")
        {
            avOkButton.CaptureMouse();
        }
        else if (scenario.Name == "Scenario5")
        {
            avOkButton.CaptureMouse();
            avOkButton.ReleaseMouseCapture();
        }
        return base.BeforeScenario(p, scenario);
    }

    [Scenario("Click back and forth between a WF and AV control and verify that Focus is correct")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "MouseFocus", "wfButton", "MouseFocus", "avButton")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at wfButton.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "WinForm Button got focus.", 
            "Failed at GotFocus on WinForm Button.", p.log);

        _edit2.Click(PointerButtons.Primary); //click on Avalon button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at wfButton.HasKeyboardFocus, expected LostFocus. HasKeyboardFocus=" 
            + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "WinForm Button lost focus.",
            "Failed at LostFocus on WinForm Button.", p.log);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Button got focus.",
            "Failed at GotFocus on Avalon Button.", p.log);

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);
        
        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at wfButton.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "WinForm Button got focus.",
            "Failed at GotFocus on WinForm Button.", p.log);

        sr.IncCounters(!_edit2.HasKeyboardFocus,
            "Failed at avButton.HasKeyboardFocus, expected LostFocus. HasKeyboardFocus=" + 
            _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Button lost focus.", p.log, BugDb.WindowsOSBugs, 1543521,
            "Known bug 1543521, Failed at LostFocus on Avalon Button.");

        _edit2.Click(PointerButtons.Primary); //click on Avalon button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at wfButton.HasKeyboardFocus, expected LostFocus. HasKeyboardFocus=" + 
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "WinForm Button lost focus.",
            "Failed at LostFocus on WinForm Button.", p.log);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Button got focus.",
            "Failed at GotFocus on Avalon Button.", p.log);
        
        return sr;
    }

    [Scenario("Click back and forth between a WF control on a seperate WF Form and an AV control and verify that focus is correct")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p, "MouseWinFormWindow", "wfButton", "MouseFocus", "avButton")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at wfButton.HasKeyboardFocus on separate window. HasKeyboardFocus=" + 
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "WinForm Button on separate window got focus.", 
            "Failed at GotFocus on WinForm Button on separate window.", p.log);

        _edit2.Click(PointerButtons.Primary); //click on Avalon button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at wfButton.HasKeyboardFocus on separate window, expected LostFocus. HasKeyboardFocus=" + 
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "WinForm Button lost focus.",
            "Failed at LostFocus on WinForm Button on separate window.", p.log);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Button got focus.",
            "Failed at GotFocus on Avalon Button.", p.log);

        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at wfButton.HasKeyboardFocus on separate window. HasKeyboardFocus=" + 
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "WinForm Button on separate window got focus.",
            "Failed at GotFocus on WinForm Button on separate window.", p.log);

        sr.IncCounters(!_edit2.HasKeyboardFocus,
            "Failed at avButton.HasKeyboardFocus, expected LostFocus. HasKeyboardFocus=" + 
            _edit2.HasKeyboardFocus, p.log);

        _edit2.Click(PointerButtons.Primary); //click on Avalon button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at wfButton.HasKeyboardFocus on separate window, expected LostFocus. HasKeyboardFocus=" + 
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "WinForm Button lost focus.",
            "Failed at LostFocus on WinForm Button on separate window.", p.log);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Button got focus.",
            "Failed at GotFocus on Avalon Button.", p.log);

        return sr;
    }

    [Scenario("Click back and forth between 2 EH controls in the same EH and verify that Focus is correct")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "MouseFocus", "avOkButton", "MouseFocus", "avCancelButton")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        _edit1.Click(PointerButtons.Primary); //click on Avalon OK button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avOkButton.HasKeyboardFocus. HasKeyboardFocus=" + 
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "Avalon OK Button got focus.",
            "Failed at GotFocus on Avalon OK Button. Actual: " + label1.Text, p.log);

        _edit2.Click(PointerButtons.Primary); //click on Avalon Cancel button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at avOkButton.HasKeyboardFocus, expected LostFocus. HasKeyboardFocus=" +
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "Avalon OK Button lost focus.",
            "Failed at LostFocus on Avalon OK Button. Actual: " + label1.Text, p.log);

        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avCancelButton.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Cancel Button got focus.",
            "Failed at GotFocus on Avalon Cancel Button. Actual: " + label2.Text, p.log);

        _edit1.Click(PointerButtons.Primary); //click on Avalon OK button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avOkButton.HasKeyboardFocus. HasKeyboardFocus=" +
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "Avalon OK Button got focus.",
            "Failed at GotFocus on Avalon OK Button. Actual: " + label1.Text, p.log);

        sr.IncCounters(!_edit2.HasKeyboardFocus,
            "Failed at avCancelButton.HasKeyboardFocus, expected LostFocus. HasKeyboardFocus=" +
            _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Cancel Button lost focus.",
            "Failed at LostFocus on Avalon Cancel Button. Actual: " + label2.Text, p.log);

        _edit2.Click(PointerButtons.Primary); //click on Avalon Cancel button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at avOkButton.HasKeyboardFocus, expected LostFocus. HasKeyboardFocus=" +
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "Avalon OK Button lost focus.",
            "Failed at LostFocus on Avalon OK Button. Actual: " + label1.Text, p.log);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avCancelButton.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Cancel Button got focus.",
            "Failed at GotFocus on Avalon Cancel Button. Actual: " + label2.Text, p.log);

        return sr;
    }

    [Scenario("CaptureMouse and make sure that the events are routed to the right control even " + 
        "when the mouse moves.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "MouseFocus", "avOkButton", "MouseFocus", "avCancelButton")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        _edit2.Click(PointerButtons.Primary); //click on Avalon Cancel button
        Utilities.SleepDoEvents(10);
        //avOkButton.CaptureMouse() in BeforeScenario, so if avCancelButton is clicked, 
        //events should still be routed to avOkButton

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avOkButton.HasKeyboardFocus. HasKeyboardFocus=" +
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "Avalon OK Button got focus.",
            "Failed at GotFocus on Avalon OK Button. Actual: " + label1.Text, p.log);

        sr.IncCounters(!_edit2.HasKeyboardFocus,
            "Failed at avCancelButton.HasKeyboardFocus, expected LostFocus. HasKeyboardFocus=" +
            _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Cancel Button lost focus.",
            "Failed at LostFocus on Avalon Cancel Button. Actual: " + label2.Text, p.log);

        return sr;
    }

    [Scenario("ReleaseCaptureMouse and now make sure that the events are routed to the correct " + 
        "controls and not the previous control that had the mose captured.")]
    public ScenarioResult Scenario5(TParams p)
    {
        //avOkButton.CaptureMouse() then avOkButton.ReleaseMouseCapture() in BeforeScenario
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "MouseFocus", "avOkButton", "MouseFocus", "avCancelButton")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        _edit2.Click(PointerButtons.Primary); //click on Avalon Cancel button
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at avOkButton.HasKeyboardFocus, expected LostFocus. HasKeyboardFocus=" +
            _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == "Avalon OK Button lost focus.",
            "Failed at LostFocus on Avalon OK Button. Actual: " + label1.Text, p.log);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avCancelButton.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(label2.Text == "Avalon Cancel Button got focus.",
            "Failed at GotFocus on Avalon Cancel Button. Actual: " + label2.Text, p.log);

        return sr;
    }

    #endregion

    #region Utilities

    //Gets Mita wrapper controls from control1 and control2 and passes them to _edit1 and 
    //_edit2 respectively.
    bool GetEditControls(TParams p, String window1, String control1, String window2, String control2)
    {
        UIObject uiApp = null;
        UIObject uiControl1 = null;
        UIObject uiControl2 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window1));
            uiControl1 = uiApp.Descendants.Find(UICondition.CreateFromId(control1));
            _edit1 = new Edit(uiControl1);
            
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window2));
            uiControl2 = uiApp.Descendants.Find(UICondition.CreateFromId(control2));
            _edit2 = new Edit(uiControl2);

            return true;
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls" + ex.ToString());
            return false;
        }
    }
    
    #endregion
}

#region MouseWinFormWindow

//class MouseWinFormWindow creates a new WinForm window with one TextBox
public class MouseWinFormWindow : SWF.Form
{
    SWF.Button wfButton = new SWF.Button();
    MouseFocus mainWindow;

    public MouseWinFormWindow(MouseFocus mainWindow)
    {
        this.mainWindow = mainWindow;

        this.SuspendLayout();
        // 
        // wfButton
        // 
        this.wfButton.Location = new System.Drawing.Point(95, 63);
        this.wfButton.Name = "wfButton";
        this.wfButton.Text = "WinForm Button";
        this.wfButton.AutoSize = true;
        this.wfButton.GotFocus += new EventHandler(wfButton_GotFocus);
        this.wfButton.LostFocus += new EventHandler(wfButton_LostFocus);
        // 
        // winform
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(292, 266);
        this.Controls.Add(this.wfButton);
        this.Name = "MouseWinFormWindow";
        this.StartPosition = SWF.FormStartPosition.CenterScreen;
        this.Text = "MouseWinFormWindow";
        this.TopMost = true;
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    void wfButton_GotFocus(object sender, EventArgs e)
    {
        mainWindow.label1.Text = "WinForm Button on separate window got focus.";
    }

    void wfButton_LostFocus(object sender, EventArgs e)
    {
        mainWindow.label1.Text = "WinForm Button lost focus.";
    }
}

#endregion

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Click back and forth between a WF and AV control and verify that Focus is correct.
//@ Click back and forth between a WF control on a seperate WF Form and an AV control and verify that focus is correct.
//@ Click back and forth between 2 EH controls in the same EH and verify that Focus is correct.
//@ CaptureMouse and make sure that the events are routed to the right control even when the mouse moves.
//@ ReleaseCaptureMouse and now make sure that the events are routed to the correct controls and not the previous control that had the mose captured.

