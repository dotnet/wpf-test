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
using System.Windows.Forms;
using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Controls;
using System.Windows.Controls;
using System.Reflection;
using System.Drawing;


// Testcase:    WindowsOSBug1530870
// Description: State should not be altered after Right-click on textbox and return
public class WindowsOSBug1530870 : ReflectBase
{
    #region CLASSVARS
    private UIObject _uiApp;
    private bool _debug = false;
    private ElementHost _eh1 = null;
    private ElementHost _eh2 = null;
    private System.Windows.Controls.TextBox _avText1 = null;
    private System.Windows.Controls.TextBox _avText2 = null;
    private const string WindowTitleName = "WindowsOSBug1530870";
    private const string avText1Name = "AVTextBox1";
    private const string avText2Name = "AVTextBox2";
    private TParams _tp;
    #endregion

    #region Testcase setup
    public WindowsOSBug1530870(string[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = WindowTitleName;
        _tp = p;
        TCSetup(p);
        this.Size = new System.Drawing.Size(100, 100);
        base.InitTest(p);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================

    #region SCENARIOSETUP
    void TCSetup(TParams p)
    {
        _avText1 = new System.Windows.Controls.TextBox();
        _avText1.Text = _avText1.Name = avText1Name;

        _avText2 = new System.Windows.Controls.TextBox();
        _avText2.Text = _avText2.Name = avText2Name;

        //Creat Element Host1
        _eh1 = new ElementHost();
        _eh1.Dock = DockStyle.Top;
        _eh1.Child = _avText1;
        _eh1.Size = new System.Drawing.Size(60, 25);
        Controls.Add(_eh1);

        //Creat Element Host 2 
        _eh2 = new ElementHost();
        _eh2.Dock = DockStyle.Bottom;
        _eh2.Child = _avText2;
        _eh2.Size = new System.Drawing.Size(60, 25);
        Controls.Add(_eh2);
    }
    #endregion

    #region Scenarios
    [Scenario("State should not be altered after Right-click on textbox and return")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        try
        {
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(avText1Name));
            UIObject ctrl2 = _uiApp.Descendants.Find(UICondition.CreateFromId(avText2Name));

            Bitmap avText1BeforeBitmap = null;
            Bitmap avText2BeforeBitmap = null;
            Bitmap avText1AfterBitmap = null;
            Bitmap avText2AfterBitmap = null;

            using (PointerInput.Activate(Mouse.Instance))
            {
                p.log.WriteLine("--- Click on the AV TextBox 2 and then take a snap shot");
                System.Drawing.Point pt = _eh2.PointToScreen(new System.Drawing.Point(_eh2.Width-10, _eh2.Height-10));
                Mouse.Instance.Move(pt);
                PointerInput.Click(PointerButtons.Primary, 1);
                Utilities.SleepDoEvents(10);
                avText1BeforeBitmap = Utilities.GetBitmapOfControl(_eh1);
                if (_debug) avText1BeforeBitmap.Save(@"avText1BeforeBitmap.bmp");
                avText2BeforeBitmap = Utilities.GetBitmapOfControl(_eh2);
                if (_debug) avText2BeforeBitmap.Save(@"avText2BeforeBitmap.bmp");

                p.log.WriteLine("--- Reset the focus back to AV TextBox 1");
                ctrl1.SetFocus();
                Utilities.SleepDoEvents(10);

                p.log.WriteLine("--- Right Click on the AV TextBox 2, do an ESC, and take a snap shot after");
                Mouse.Instance.Move(pt);
                PointerInput.Click(PointerButtons.Secondary, 1);
                Utilities.SleepDoEvents(10);
                _uiApp.SendKeys("{ESC}");
                Utilities.SleepDoEvents(30);
                avText1AfterBitmap = Utilities.GetBitmapOfControl(_eh1);
                if (_debug) avText1AfterBitmap.Save(@"avText1AfterBitmap.bmp");
                avText2AfterBitmap = Utilities.GetBitmapOfControl(_eh2);
                if (_debug) avText2AfterBitmap.Save(@"avText2AfterBitmap.bmp");
            }

            p.log.WriteLine("--- Compare bitmap before and after");
            sr.IncCounters(Utilities.BitmapsIdentical(avText1BeforeBitmap, avText1AfterBitmap), "AV TextBox 1 before and after not the same", p.log);
            sr.IncCounters(Utilities.BitmapsIdentical(avText2BeforeBitmap, avText2AfterBitmap), "AV TextBox 2 before and after not the same", p.log);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls: " + ex.ToString());
        }
        return sr;
    }
    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ State should not be altered after Right-click on textbox and return
