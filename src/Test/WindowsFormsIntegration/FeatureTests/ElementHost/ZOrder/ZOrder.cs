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
using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;


// Testcase:    ZOrder
// Description: Verify the z-order of an ElementHost and its Avalon control.
public class ZOrder : ReflectBase
{
    #region Testcase setup

    ElementHost _elementHost1;
    SWC.Button _avButton;
    SWF.Button _wfButton;
    SWC.ContextMenu _avContextMenu;
    SWF.Label _label;
    Bitmap _bmp;
    Edit _edit1;

    public ZOrder(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "ZOrder";
        this.Size = new System.Drawing.Size(350, 300);
        this.StartPosition = SWF.FormStartPosition.Manual;
        this.Location = new System.Drawing.Point(0, 0);
        this.UseMita = true;
        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        _avButton = new SWC.Button();
        _avButton.Background = System.Windows.Media.Brushes.White;
        _avButton.Content = "Avalon Button";

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _avButton;
        _elementHost1.Location = new System.Drawing.Point(0, 0);
        _elementHost1.BackColor = Color.White;
        Controls.Add(_elementHost1);

        _wfButton = new SWF.Button();
        _wfButton.Name = "wfButton";
        _wfButton.Size = new System.Drawing.Size(200, 100);
        _wfButton.Text = "WinForm Button";
        _wfButton.BackColor = Color.Red;
        Controls.Add(_wfButton);
        Utilities.SleepDoEvents(10);

        _label = new SWF.Label();
        _label.Height = 50;
        _label.Dock = SWF.DockStyle.Bottom;
        Controls.Add(_label);

        if (scenario.Name == "Scenario2")
        {

            _avContextMenu = new SWC.ContextMenu();
            _avContextMenu.Background = System.Windows.Media.Brushes.Blue;
            SWC.MenuItem menuItem1 = new SWC.MenuItem();
            SWC.MenuItem menuItem2 = new SWC.MenuItem();
            SWC.MenuItem menuItem3 = new SWC.MenuItem();
            SWC.MenuItem menuItem4 = new SWC.MenuItem();
            menuItem1.Header = "Avalon Menu Item 1";
            menuItem2.Header = "Avalon Menu Item 2";
            menuItem3.Header = "Avalon Menu Item 3";
            menuItem4.Header = "Avalon Menu Item 4";
            _avContextMenu.Items.Add(menuItem1);
            _avContextMenu.Items.Add(menuItem2);
            _avContextMenu.Items.Add(menuItem3);
            _avContextMenu.Items.Add(menuItem4);
            _avContextMenu.Opened += new RoutedEventHandler(avContextMenu_Opened);
            _avButton.ContextMenu = _avContextMenu;
        }

        if (scenario.Name == "Scenario3")
        {

            SWC.ToolTip avToolTip = new SWC.ToolTip();
            avToolTip.Content = "This is an Avalon ToolTip.";
            avToolTip.StaysOpen = true;
            avToolTip.Opened += new RoutedEventHandler(avToolTip_Opened);
            _avButton.ToolTip = avToolTip;

            SWF.ToolTip wfToolTip = new SWF.ToolTip();
            wfToolTip.ToolTipTitle = "WinForm ToolTip";
            wfToolTip.ShowAlways = true;
            wfToolTip.Popup += new System.Windows.Forms.PopupEventHandler(wfToolTip_Popup);
            wfToolTip.SetToolTip(_wfButton, "This is a WinForm ToolTip.");
        }
  
        return base.BeforeScenario(p, scenario);
    }

    void wfToolTip_Popup(object sender, System.Windows.Forms.PopupEventArgs e)
    {
        _label.Text = "WinForm ToolTip opened.";
    }

    void avToolTip_Opened(object sender, RoutedEventArgs e)
    {
        _label.Text = "Avalon ToolTip opened.";
    }

    void avContextMenu_Opened(object sender, RoutedEventArgs e)
    {
        _label.Text = "Avalon Context Menu opened.";
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        base.AfterScenario(p, scenario, result);
    }

    [Scenario("Hosted element and its controls should respect EH z-Order.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Check initial properties
        sr.IncCounters(this.Controls.GetChildIndex(_elementHost1) == 0,
            "Failed at GetChildIndex(elementHost1). Expected: 0, Actual: " +
            this.Controls.GetChildIndex(_elementHost1), p.log);
        sr.IncCounters(this.Controls.GetChildIndex(_wfButton) == 1,
            "Failed at GetChildIndex(wfButton). Expected: 1, Actual: " +
            this.Controls.GetChildIndex(_wfButton), p.log);
	Utilities.SleepDoEvents(50);
        _bmp = Utilities.GetScreenBitmap(new Rectangle(8, 34, 200, 100));
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.White) >= 80,
            "Bitmap Failed at initial properties. Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.White) + "%", p.log);

        _elementHost1.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
        {
            _elementHost1.SendToBack();
        });
        Utilities.SleepDoEvents(10);

        //Check SendToBack of the z-order properties
        sr.IncCounters(this.Controls.GetChildIndex(_elementHost1) == 2,
            "Failed at GetChildIndex(elementHost1) after elementHost1.SendToBack. Expected: 2, Actual: " +
            this.Controls.GetChildIndex(_elementHost1), p.log);
        sr.IncCounters(this.Controls.GetChildIndex(_wfButton) == 0,
            "Failed at GetChildIndex(wfButton) after elementHost1.SendToBack. Expected: 0, Actual: " +
            this.Controls.GetChildIndex(_wfButton), p.log);

	Utilities.SleepDoEvents(10);
        _bmp = Utilities.GetScreenBitmap(new Rectangle(8, 34, 200, 100));
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) >= 80,
            "Bitmap Failed at SendToBack. Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        _elementHost1.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
        {
            _elementHost1.BringToFront();
        });
        Utilities.SleepDoEvents(10);

        //Check BringToFront of the z-order properties
        sr.IncCounters(this.Controls.GetChildIndex(_elementHost1) == 0,
            "Failed at GetChildIndex(elementHost1) after elementHost1.BringToFront. Expected: 0, Actual: " +
            this.Controls.GetChildIndex(_elementHost1), p.log);
        sr.IncCounters(this.Controls.GetChildIndex(_wfButton) == 1,
            "Failed at GetChildIndex(wfButton) after elementHost1.BringToFront. Expected: 1, Actual: " +
            this.Controls.GetChildIndex(_wfButton), p.log);

        _bmp = Utilities.GetScreenBitmap(new Rectangle(8, 34, 200, 100));
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.White) >= 80,
            "Bitmap Failed at SendToBack. Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.White) + "%", p.log);

        _wfButton.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
        {
            _wfButton.BringToFront();
        });
        Utilities.SleepDoEvents(10);

        //Check BringToFront of the z-order properties
        sr.IncCounters(this.Controls.GetChildIndex(_elementHost1) == 1,
            "Failed at GetChildIndex(elementHost1) after wfButton.BringToFront. Expected: 1, Actual: " +
            this.Controls.GetChildIndex(_elementHost1), p.log);
        sr.IncCounters(this.Controls.GetChildIndex(_wfButton) == 0,
            "Failed at GetChildIndex(wfButton) after wfButton.BringToFront. Expected: 0, Actual: " +
            this.Controls.GetChildIndex(_wfButton), p.log);

        _bmp = Utilities.GetScreenBitmap(new Rectangle(8, 34, 200, 100));
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) >= 80,
            "Bitmap Failed at wfButton.BringToFront(). Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.Red) + "%", p.log);

        _wfButton.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
        {
            _wfButton.SendToBack();
        });
        Utilities.SleepDoEvents(10);

        //Check SendToBack of the z-order properties
        sr.IncCounters(this.Controls.GetChildIndex(_elementHost1) == 0,
            "Failed at GetChildIndex(elementHost1) after wfButton.SendToBack. Expected: 0, Actual: " +
            this.Controls.GetChildIndex(_elementHost1), p.log);
        sr.IncCounters(this.Controls.GetChildIndex(_wfButton) == 2,
            "Failed at GetChildIndex(wfButton) after wfButton.SendToBack. Expected: 2, Actual: " +
            this.Controls.GetChildIndex(_wfButton), p.log);

        _bmp = Utilities.GetScreenBitmap(new Rectangle(8, 34, 200, 100));
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.White) >= 80,
            "Bitmap Failed at wfButton.SendToBack(). Percent match: " +
            BitmapsColorPercent(_bmp, 0, 0, 200, 100, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("z-Ordering on Av context menu.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        
        if(!GetEditControls(p, this.Text, "elementHost1"))
        {
            return new ScenarioResult(false);
        }

        //Check initial properties
        _edit1.Click(PointerButtons.Secondary);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(_label.Text == "Avalon Context Menu opened.",
            "Failed at Avalon Context Menu.", p.log);

        _elementHost1.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
        {
            _elementHost1.SendToBack();
        });
        Utilities.SleepDoEvents(10);

        if (!GetEditControls(p, this.Text, "wfButton"))
        {
            return new ScenarioResult(false);
        }
        //Check SendToBack of the z-order ContextMenu properties
        _edit1.Click(PointerButtons.Secondary);
        Utilities.SleepDoEvents(20);

        sr.IncCounters(_label.Text == "WinForm Context Menu opened.",
            "Failed at WinForm Context Menu.", p.log);
        _edit1.Click(PointerButtons.Primary);
        return sr;
    }

    [Scenario("z-Ordering on ToolTip.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, this.Text, "elementHost1"))
        {
            return new ScenarioResult(false);
        }

        //Check initial properties
        _edit1.MovePointer();
        Utilities.SleepDoEvents(20);
        sr.IncCounters(_label.Text == "Avalon ToolTip opened.",
            "Failed at Avalon ToolTip.", p.log);

        _elementHost1.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
        {
            _elementHost1.SendToBack();
        });
        Utilities.SleepDoEvents(10);

        if (!GetEditControls(p, this.Text, "wfButton"))
        {
            return new ScenarioResult(false);
        }
        //Check SendToBack of the z-order ContextMenu properties
        _edit1.MovePointer();
        Utilities.SleepDoEvents(20);

        sr.IncCounters(_label.Text == "WinForm ToolTip opened.",
            "Failed at WinForm ToolTip.", p.log);

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

    //Gets Mita wrapper control from control1 and passes it to _edit1
    bool GetEditControls(TParams p, String window1, String control1)
    {
        UIObject uiApp = null;
        UIObject uiControl1 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window1));
            uiControl1 = uiApp.Descendants.Find(UICondition.CreateFromId(control1));
            _edit1 = new Edit(uiControl1);

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
//@ Hosted element and its controls should respect EH z-Order.
//@ z-Ordering on Av context menu.
//@ z-Ordering on ToolTip.
