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


// Testcase:    FocusPreservation
// Description: Verify that when clicking on either WF or AV control, Focus is correct
public class FocusPreservation : ReflectBase
{
    #region Testcase setup

    Edit _edit1;
    Edit _edit2;
    Edit _edit3;
    Edit _edit4;
    Edit _edit5;
    Edit _form;
    SWF.Button _wfButton1 = new SWF.Button();
    SWF.MenuStrip _menuStrip1 = new SWF.MenuStrip();
    SWC.Button _avButton1 = new SWC.Button();
    SWC.Button _avButton2 = new SWC.Button();
    SWC.Button _avButton3 = new SWC.Button();
    SWC.StackPanel _stackPanel = new SWC.StackPanel();
    ElementHost _elementHost1 = new ElementHost();
    ElementHost _elementHost2 = new ElementHost();
    public SWF.Label label1 = new SWF.Label();
    public SWF.Label label2 = new SWF.Label();
    public SWF.Label label3 = new SWF.Label();
    SWF.Form _newForm = new SWF.Form();

    private const string wfButton1GotFocus = "WinForm Button 1 Got Focus";
    private const string wfButton1LostFocus = "WinForm Button 1 Lost Focus";
    private const string avButton1GotFocus = "Avalon Button 1 Got Focus";
    private const string avButton1LostFocus = "Avalon Button 1 Lost Focus";
    private const string avButton2GotFocus = "Avalon Button 2 Got Focus";
    private const string avButton2LostFocus = "Avalon Button 2 Lost Focus";
    private const string avButton3GotFocus = "Avalon Button 3 Got Focus";
    private const string avButton3LostFocus = "Avalon Button 3 Lost Focus";
    private const string avTextBox1GotFocus = "Avalon TextBox 1 Got Focus";
    private const string avTextBox1LostFocus = "Avalon TextBox 1 Lost Focus";

    public FocusPreservation(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "FocusPreservation";
        this.Size = new System.Drawing.Size(400, 400);

        label1.AutoSize = true;
        label1.Location = new System.Drawing.Point(100, 150);
        Controls.Add(label1);
        label2.AutoSize = true;
        label2.Location = new System.Drawing.Point(100, 180);
        Controls.Add(label2);
        label3.AutoSize = true;
        label3.Location = new System.Drawing.Point(100, 210);
        Controls.Add(label3);

        _avButton1.Name = "avButton1";
        _avButton1.Content = "Avalon Button _1";
        _avButton1.GotFocus += new RoutedEventHandler(avButton1_GotFocus);
        _avButton1.LostFocus += new RoutedEventHandler(avButton1_LostFocus);

        _avButton2.Name = "avButton2";
        _avButton2.Content = "Avalon Button _2";
        _avButton2.GotFocus += new RoutedEventHandler(avButton2_GotFocus);
        _avButton2.LostFocus += new RoutedEventHandler(avButton2_LostFocus);

        _avButton3.Name = "avButton3";
        _avButton3.Content = "Avalon Button _3";
        _avButton3.GotFocus += new RoutedEventHandler(avButton3_GotFocus);
        _avButton3.LostFocus += new RoutedEventHandler(avButton3_LostFocus);

        SWC.TextBox avTextBox1 = new SWC.TextBox();
        avTextBox1.Name = "avTextBox1";
        avTextBox1.GotFocus += new RoutedEventHandler(avTextBox1_GotFocus);
        avTextBox1.LostFocus += new RoutedEventHandler(avTextBox1_LostFocus);

        _stackPanel.Children.Add(_avButton1);
        _stackPanel.Children.Add(_avButton2);
        _stackPanel.Children.Add(_avButton3);
        _stackPanel.Children.Add(avTextBox1);

        //Create Element Host 1
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _stackPanel;
        _elementHost1.AutoSize = true;
        _elementHost1.Location = new System.Drawing.Point(100, 20);
        _elementHost1.BackColor = Color.Red;
        Controls.Add(_elementHost1);

        _wfButton1.Name = "wfButton1";
        _wfButton1.Text = "&WinForm Button";
        _wfButton1.AutoSize = true;
        _wfButton1.Location = new System.Drawing.Point(100, 300);
        _wfButton1.GotFocus += new EventHandler(wfButton1_GotFocus);
        _wfButton1.LostFocus += new EventHandler(wfButton1_LostFocus);
        Controls.Add(_wfButton1);

        _menuStrip1 = new SWF.MenuStrip();
        SWF.ToolStripMenuItem item1 = new SWF.ToolStripMenuItem();
        SWF.ToolStripMenuItem item2 = new SWF.ToolStripMenuItem();
        SWF.ToolStripMenuItem item3 = new SWF.ToolStripMenuItem();
        _menuStrip1.Items.AddRange(new SWF.ToolStripItem[] { item1 });
        _menuStrip1.Text = "menuStrip1";
        item1.DropDownItems.AddRange(new SWF.ToolStripItem[] { item2, item3 });
        item1.Name = "toolStripMenuItem1";
        item1.Text = "File";
        item2.Name = "testToolStripMenuItem1";
        item2.ShortcutKeys = ((SWF.Keys)((SWF.Keys.Control | SWF.Keys.T)));
        item2.Text = "Test";
        item3.Name = "item2ToolStripMenuItem1";
        item3.Text = "Exit";
        this.Controls.Add(this._menuStrip1);

        base.InitTest(p);
    }

    void avTextBox1_GotFocus(object sender, RoutedEventArgs e)
    {
        label1.Text = avTextBox1GotFocus;
    }
    void avTextBox1_LostFocus(object sender, RoutedEventArgs e)
    {
        label2.Text = avTextBox1LostFocus;
    }

    void wfButton1_GotFocus(object sender, EventArgs e)
    {
        label3.Text = wfButton1GotFocus;
    }
    void wfButton1_LostFocus(object sender, EventArgs e)
    {
        label3.Text = wfButton1LostFocus;
    }

    void avButton1_GotFocus(object sender, RoutedEventArgs e)
    {
        label1.Text = avButton1GotFocus;
    }
    void avButton1_LostFocus(object sender, RoutedEventArgs e)
    {
        label2.Text = avButton1LostFocus;
    }

    void avButton2_GotFocus(object sender, RoutedEventArgs e)
    {
        label1.Text = avButton2GotFocus;
    }
    void avButton2_LostFocus(object sender, RoutedEventArgs e)
    {
        label2.Text = avButton2LostFocus;
    }

    void avButton3_GotFocus(object sender, RoutedEventArgs e)
    {
        label1.Text = avButton3GotFocus;
    }
    void avButton3_LostFocus(object sender, RoutedEventArgs e)
    {
        label2.Text = avButton3LostFocus;
    }

    void avButton2_Click(object sender, RoutedEventArgs e)
    {
        SWF.Button wfButton2 = new SWF.Button();
        wfButton2.Name = "wfButton2";
        wfButton2.Text = "OK";
        wfButton2.Click += new EventHandler(wfButton2_Click);
        _newForm.Controls.Add(wfButton2);
        _newForm.AcceptButton = wfButton2;
        _newForm.Name = "newForm";
        _newForm.ShowDialog();
    }

    void wfButton2_Click(object sender, EventArgs e)
    {
        _newForm.Close();
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        _avButton1.Focus();

        if (scenario.Name == "Scenario9")
        {
            _avButton2.Click += new RoutedEventHandler(avButton2_Click);
        }

        return base.BeforeScenario(p, scenario);
    }

    [Scenario("Focus is saved when navigating between Form and EH (mouse)")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        GetAllEditControls(p);

        //Verify initial state
        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        _edit5.Click();
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at !avTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit4.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1LostFocus, label2.Text,
            "Failed at LostFocus on Avalon TextBox 1.", p.log);
        sr.IncCounters(_edit5.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus, p.log);
        sr.IncCounters(wfButton1GotFocus, label3.Text,
            "Failed at GotFocus on WinForm Button 1.", p.log);

        _edit1.Click();
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit5.HasKeyboardFocus,
            "Failed at !wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus, p.log);
        sr.IncCounters(wfButton1LostFocus, label3.Text,
            "Failed at LostFocus on WinForm Button 1.", p.log);
        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        _edit2.Click();
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at !avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1LostFocus, label2.Text,
            "Failed at LostFocus on Avalon Button 1.", p.log);
        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        _edit3.Click();
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit2.HasKeyboardFocus,
            "Failed at !avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2LostFocus, label2.Text,
            "Failed at LostFocus on Avalon Button 2.", p.log);
        sr.IncCounters(_edit3.HasKeyboardFocus,
            "Failed at avButton3.HasKeyboardFocus. HasKeyboardFocus=" + _edit3.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton3GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 3.", p.log);

        _edit4.Click();
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit3.HasKeyboardFocus,
            "Failed at !avButton3.HasKeyboardFocus. HasKeyboardFocus=" + _edit3.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton3LostFocus, label2.Text,
            "Failed at LostFocus on Avalon Button 3.", p.log);
        sr.IncCounters(_edit4.HasKeyboardFocus,
            "Failed at avTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit4.HasKeyboardFocus, p.log);
        sr.IncCounters(avTextBox1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon TextBox 1.", p.log);

        _edit5.Click();
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit4.HasKeyboardFocus,
            "Failed at !avTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit4.HasKeyboardFocus, p.log);
        sr.IncCounters(avTextBox1LostFocus, label2.Text,
            "Failed at LostFocus on Avalon TextBox 1.", p.log);
        sr.IncCounters(_edit5.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus, p.log);
        sr.IncCounters(wfButton1GotFocus, label3.Text,
            "Failed at GotFocus on WinForm Button 1.", p.log);

        _edit1.Click();
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit5.HasKeyboardFocus,
            "Failed at !wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus, p.log);
        sr.IncCounters(wfButton1LostFocus == label3.Text, p.log, BugDb.WindowsOSBugs, 5,
            "Failed at LostFocus on WinForm Button 1.");
        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == avButton1GotFocus,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        _edit5.Click();
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at !avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1LostFocus, label2.Text,
            "Failed at LostFocus on Avalon TextBox 1.", p.log);
        sr.IncCounters(_edit5.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus, p.log);
        sr.IncCounters(wfButton1GotFocus, label3.Text,
            "Failed at GotFocus on WinForm Button 1.", p.log);

        _edit1.Click();
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit5.HasKeyboardFocus,
            "Failed at !wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus, p.log);
        sr.IncCounters(wfButton1LostFocus, label3.Text,
            "Failed at LostFocus on WinForm Button 1.", p.log);
        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        return sr;
    }

    [Scenario("Focus is saved when navigating between Form and EH (keyboard - TAB)")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        GetAllEditControls(p);

        //Verify initial state
        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit1.HasKeyboardFocus,
            "Failed at !avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1LostFocus, label2.Text,
            "Failed at LostFocus on Avalon Button 1.", p.log);
        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit2.HasKeyboardFocus,
            "Failed at !avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2LostFocus, label2.Text,
            "Failed at LostFocus on Avalon Button 2.", p.log);
        sr.IncCounters(_edit3.HasKeyboardFocus,
            "Failed at avButton3.HasKeyboardFocus. HasKeyboardFocus=" + _edit3.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton3GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 3.", p.log);

        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit3.HasKeyboardFocus,
            "Failed at !avButton3.HasKeyboardFocus. HasKeyboardFocus=" + _edit3.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton3LostFocus, label2.Text,
            "Failed at LostFocus on Avalon Button 3.", p.log);
        sr.IncCounters(_edit4.HasKeyboardFocus,
            "Failed at avTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit4.HasKeyboardFocus, p.log);
        sr.IncCounters(avTextBox1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon TextBox 1.", p.log);

        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit4.HasKeyboardFocus,
            "Failed at !avTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit4.HasKeyboardFocus, p.log);
        sr.IncCounters(avTextBox1LostFocus, label2.Text,
            "Failed at LostFocus on Avalon TextBox 1.", p.log);
        sr.IncCounters(_edit5.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus, p.log);
        sr.IncCounters(wfButton1GotFocus, label3.Text,
            "Failed at GotFocus on WinForm Button 1.", p.log);

        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(!_edit5.HasKeyboardFocus,
            "Failed at !wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus, p.log);
        sr.IncCounters(wfButton1LostFocus == label3.Text, p.log, BugDb.WindowsOSBugs, 5,
            "Failed at LostFocus on WinForm Button 1.");
        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(label1.Text == avButton1GotFocus,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        return sr;
    }

    [Scenario("Focus is saved when navigating between Form and EH (keyboard - AccessorKey)")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        GetAllEditControls(p);

        //Verify initial state
        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        _form.SendKeys("%w");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        _form.SendKeys("%2");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        _form.SendKeys("%3");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        _form.SendKeys("%w");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        _form.SendKeys("%1");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.HasKeyboardFocus,
            "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton1GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 1.", p.log);

        return sr;
    }

    [Scenario("Focus is saved when navigating between Form Menu and EH control")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        GetAllEditControls(p);
        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        //Verify initial state
        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        _form.SendKeys("%f");
        Utilities.SleepDoEvents(10);
        _form.SendKeys("{ENTER}");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        _form.SendKeys("{TAB}");
        _form.SendKeys("%f");
        Utilities.SleepDoEvents(10);
        _form.SendKeys("{ENTER}");
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit3.HasKeyboardFocus,
            "Failed at avButton3.HasKeyboardFocus. HasKeyboardFocus=" + _edit3.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton3GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 3.", p.log);

        return sr;
    }

    [Scenario("Focus is saved when the window is minimized")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        GetAllEditControls(p);
        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        //Verify initial state
        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        this.WindowState = SWF.FormWindowState.Minimized;
        Utilities.SleepDoEvents(10);
        
        this.WindowState = SWF.FormWindowState.Normal;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit2.HasKeyboardFocus, p.log, BugDb.WindowsOSBugs, 6,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        return sr;
    }

    [Scenario("Focus is saved when the window is maximized")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        GetAllEditControls(p);
        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        //Verify initial state
        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        this.WindowState = SWF.FormWindowState.Maximized;
        Utilities.SleepDoEvents(10);

        this.WindowState = SWF.FormWindowState.Normal;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        return sr;
    }

    [Scenario("Focus is saved when the window is resized")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        GetAllEditControls(p);
        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        //Verify initial state
        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        this.Size = new System.Drawing.Size(500, 500);
        Utilities.SleepDoEvents(10);

        this.Size = new System.Drawing.Size(300, 300);
        Utilities.SleepDoEvents(10);

        this.Size = new System.Drawing.Size(400, 400);
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        return sr;
    }

    [Scenario("Focus is saved when the window is moved")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        GetAllEditControls(p);
        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        //Verify initial state
        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        System.Drawing.Point start = this.Location;

        this.Location = new System.Drawing.Point(0, 0);
        Utilities.SleepDoEvents(10);

        this.Location = new System.Drawing.Point(400, 0);
        Utilities.SleepDoEvents(10);

        this.Location = new System.Drawing.Point(0, 300);
        Utilities.SleepDoEvents(10);

        this.Location = new System.Drawing.Point(400, 300);
        Utilities.SleepDoEvents(10);

        this.Location = start;
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        return sr;
    }

    [Scenario("Focus is saved after returning from a modal dialog")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        GetAllEditControls(p);
        _form.SendKeys("{TAB}");
        Utilities.SleepDoEvents(10);

        //Verify initial state
        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        //Launch modal dialog
        _edit2.Click();
        Utilities.SleepDoEvents(20);

        //Close modal dialog
        UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("FocusPreservation"));
        UIObject uiDialog = uiApp.Descendants.Find(UICondition.CreateFromId("newForm"));
        UIObject uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("wfButton2"));
        Edit button = new Edit(uiControl);
        button.Click();

        Utilities.SleepDoEvents(20);

        sr.IncCounters(_edit2.HasKeyboardFocus,
            "Failed at avButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus, p.log);
        sr.IncCounters(avButton2GotFocus, label1.Text,
            "Failed at GotFocus on Avalon Button 2.", p.log);

        return sr;
    }

    #endregion

    #region Utilities

    //Gets Mita wrapper controls for all controls
    bool GetAllEditControls(TParams p)
    {
        try
        {
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("FocusPreservation"));
            _form = new Edit(uiApp);

            UIObject uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("avButton1"));
            _edit1 = new Edit(uiControl);
            uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("avButton2"));
            _edit2 = new Edit(uiControl);
            uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("avButton3"));
            _edit3 = new Edit(uiControl);
            uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox1"));
            _edit4 = new Edit(uiControl);
            uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            _edit5 = new Edit(uiControl);

            _form.SetFocus();
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

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Focus is saved when navigating between Form and EH (mouse)
//@ Focus is saved when navigating between Form and EH (keyboard - TAB)
//@ Focus is saved when navigating between Form and EH (keyboard - AccessorKey)
//@ Focus is saved when navigating between Form Menu and EH control 
//@ Focus is saved when the window is minimized
//@ Focus is saved when the window is maximized
//@ Focus is saved when the window is resized
//@ Focus is saved when the window is moved
//@ Focus is saved after returning from a modal dialog
