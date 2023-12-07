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
using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;
using System.Threading;
using System.Reflection;

//
// Testcase:    MouseFocus
// Description: Verify that when clicking on either WF or AV control, Focus is correct
//
public class MouseFocus : ReflectBase
{
    #region Testcase setup

    Edit _edit1;
    Edit _edit2;
    SWF.Button _wfButton = new SWF.Button();
    SWC.Button _avButton = new SWC.Button();
    SWC.Button _avOkButton = new SWC.Button();
    SWC.Button _avCancelButton = new SWC.Button();
    SWC.StackPanel _stackPanel = new SWC.StackPanel();
    ElementHost _elementHost1 = new ElementHost();
    ElementHost _elementHost2 = new ElementHost();
    public SWF.Label label1 = new SWF.Label();
    public SWF.Label label2 = new SWF.Label();
    MouseWinFormWindow _winform;

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

        _wfButton.Name = "wfButton";
        _wfButton.Text = "WinForm Button";
        _wfButton.AutoSize = true;
        _wfButton.Location = new System.Drawing.Point(100, 300);
        _wfButton.GotFocus += new EventHandler(wfButton_GotFocus);
        _wfButton.LostFocus += new EventHandler(wfButton_LostFocus);
        Controls.Add(_wfButton);

        _avButton.Name = "avButton";
        _avButton.Content = "Avalon Button";
        _avButton.GotFocus += new RoutedEventHandler(avButton_GotFocus);
        _avButton.LostFocus += new RoutedEventHandler(avButton_LostFocus);

        _avOkButton.Name = "avOkButton";
        _avOkButton.Content = "Avalon OK Button";
        _avOkButton.GotFocus += new RoutedEventHandler(avOkButton_GotFocus);
        _avOkButton.LostFocus += new RoutedEventHandler(avOkButton_LostFocus);

        _avCancelButton.Name = "avCancelButton";
        _avCancelButton.Content = "Avalon Cancel Button";
        _avCancelButton.GotFocus += new RoutedEventHandler(avCancelButton_GotFocus);
        _avCancelButton.LostFocus += new RoutedEventHandler(avCancelButton_LostFocus);

        _stackPanel.Children.Add(_avOkButton);
        _stackPanel.Children.Add(_avCancelButton);
        _stackPanel.Orientation = SWC.Orientation.Horizontal;

        //Create Element Host 1
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _avButton;
        _elementHost1.Size = new System.Drawing.Size(150, 100);
        _elementHost1.Location = new System.Drawing.Point(100, 20);
        _elementHost1.BackColor = Color.Red;
        Controls.Add(_elementHost1);

        //Create Element Host 2
        _elementHost2.Name = "elementHost2";
        _elementHost2.Child = _stackPanel; 
        _elementHost2.Location = new System.Drawing.Point(100, 230);
        _elementHost2.BackColor = Color.Red;
        _elementHost2.AutoSize = true;
        Controls.Add(_elementHost2);

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
            _winform = new MouseWinFormWindow(this);
            _winform.Show();
        }
        else if (scenario.Name == "Scenario4")
        {
            _avOkButton.CaptureMouse();
        }
        else if (scenario.Name == "Scenario5")
        {
            _avOkButton.CaptureMouse();
            _avOkButton.ReleaseMouseCapture();
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
    SWF.Button _wfButton = new SWF.Button();
    MouseFocus _mainWindow;

    public MouseWinFormWindow(MouseFocus mainWindow)
    {
        this._mainWindow = mainWindow;

        this.SuspendLayout();
        // 
        // wfButton
        // 
        this._wfButton.Location = new System.Drawing.Point(95, 63);
        this._wfButton.Name = "wfButton";
        this._wfButton.Text = "WinForm Button";
        this._wfButton.AutoSize = true;
        this._wfButton.GotFocus += new EventHandler(wfButton_GotFocus);
        this._wfButton.LostFocus += new EventHandler(wfButton_LostFocus);
        // 
        // winform
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(292, 266);
        this.Controls.Add(this._wfButton);
        this.Name = "MouseWinFormWindow";
        this.StartPosition = SWF.FormStartPosition.CenterScreen;
        this.Text = "MouseWinFormWindow";
        this.TopMost = true;
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    void wfButton_GotFocus(object sender, EventArgs e)
    {
        _mainWindow.label1.Text = "WinForm Button on separate window got focus.";
    }

    void wfButton_LostFocus(object sender, EventArgs e)
    {
        _mainWindow.label1.Text = "WinForm Button lost focus.";
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

