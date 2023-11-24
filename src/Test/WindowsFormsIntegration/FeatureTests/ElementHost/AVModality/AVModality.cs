using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using SWI = System.Windows.Input;
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

//
// Testcase:    AVModality
// Description: Verify the modality of Avalon windows launched from WinForms
// Author:      a-rickyt
//
public class AVModality : ReflectBase
{
    #region Testcase setup

    SWF.Button wfButton1;
    SWF.Button wfButton2;
    SWF.Button wfButton3;
    Edit _edit1;
    System.Windows.Window window1;
    System.Windows.Window window2;
    Bitmap bmp;

    public AVModality(string[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "AVModality";
        this.Size = new System.Drawing.Size(400, 400);
        this.Left = 0;
        this.Top = 260;

        wfButton1 = new SWF.Button();
        wfButton1.Name = "wfButton1";
        wfButton1.Text = "Launch a Modal AV Window";
        wfButton1.AutoSize = true;
        wfButton1.Click += new EventHandler(wfButton1_Click);
        this.Controls.Add(wfButton1);

        wfButton2 = new SWF.Button();
        wfButton2.Name = "wfButton2";
        wfButton2.Text = "Launch a Non Modal AV Window";
        wfButton2.AutoSize = true;
        wfButton2.Location = new System.Drawing.Point(0, 50);
        wfButton2.Click += new EventHandler(wfButton2_Click);
        this.Controls.Add(wfButton2);

        wfButton3 = new SWF.Button();
        wfButton3.Name = "wfButton3";
        wfButton3.Text = "Launch a Another Modal AV Window";
        wfButton3.AutoSize = true;
        wfButton3.Location = new System.Drawing.Point(0, 100);
        wfButton3.Click += new EventHandler(wfButton3_Click);
        this.Controls.Add(wfButton3);
	
	Utilities.SleepDoEvents(20);

        base.InitTest(p);
    }

    void wfButton1_Click(object sender, EventArgs e)
    {
        InitializeAvalonWindow();
        window1.ShowDialog();
    }

    void wfButton2_Click(object sender, EventArgs e)
    {
        InitializeAvalonWindow();
        window1.Show();
    }

    void wfButton3_Click(object sender, EventArgs e)
    {
        InitializeAnotherAvalonWindow();
        window2.ShowDialog();
    }

    void avButton1_Click(object sender, RoutedEventArgs e)
    {
        window1.Close();
    }
    void avButton2_Click(object sender, RoutedEventArgs e)
    {
        window2.Close();
    }

    void InitializeAvalonWindow()
    {
        window1 = new System.Windows.Window();
        SWC.Button avButton1 = new SWC.Button();
        SWC.Label label = new SWC.Label();
        SWC.StackPanel stackPanel = new SWC.StackPanel();

        window1.Title = "AvalonWindow";
        window1.Width = 300;
        window1.Height = 300;
        window1.Top = 0;
        window1.Left = 0;
        label.Content = "I am an Avalon Window";
        avButton1.Name = "avButton1";
        avButton1.Content = "Close Window";
        avButton1.Click += new RoutedEventHandler(avButton1_Click);
        stackPanel.Children.Add(label);
        stackPanel.Children.Add(avButton1);
        window1.Content = stackPanel;
    }

    void InitializeAnotherAvalonWindow()
    {
        window2 = new System.Windows.Window();
        SWC.Button avButton2 = new SWC.Button();
        SWC.Label label = new SWC.Label();
        SWC.StackPanel stackPanel = new SWC.StackPanel();

        window2.Title = "AnotherAvalonWindow";
        window2.Width = 300;
        window2.Height = 300;
        window2.Top = 0;
        window2.Left = 300;
        label.Content = "I am another Avalon Window";
        avButton2.Name = "avButton2";
        avButton2.Content = "Close Window";
        avButton2.Click += new RoutedEventHandler(avButton2_Click);
        stackPanel.Children.Add(label);
        stackPanel.Children.Add(avButton2);
        window2.Content = stackPanel;
    }

    #endregion

    #region Scenarios

    [Scenario("Launch a Modal AV Window from a WF Form.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "AVModality", "wfButton1"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(300);

        //Verify that Modal AV Window is launched.
        bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) >= 75,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%. Expected >= 75.", p.log);

        //Verify that the window is really modal
        if (!GetEditControls(p, "AVModality", "wfButton2"))
        {
            return new ScenarioResult(false);
        }
        bool modality = false;
        try
        {
            _edit1.SetFocus();
        }
        catch (Exception)
        {
            modality = true;
        }
        sr.IncCounters(modality, "Failed at Modality check. Window is not modal.", p.log);

        //Close modal window
        if (!GetEditControls(p, "AvalonWindow", "avButton1"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);
        //Verify that Modal AV Window has been closed.
        bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) <= 40,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%. Expected <= 40.", p.log);

        return sr;
    }

    [Scenario("Launch a non Modal AV Windows from a WF Form.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "AVModality", "wfButton2"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        //Verify that Non-Modal AV Window is launched.
        bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) >= 75,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%. Expected >= 75", p.log);

        //Verify that the window is really non-modal
        if (!GetEditControls(p, "AVModality", "wfButton2"))
        {
            return new ScenarioResult(false);
        }
        _edit1.SetFocus();
        Utilities.SleepDoEvents(20);
        bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) <= 75,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%. Expected <= 75", p.log);

        //Close non-modal window
        if (!GetEditControls(p, "AvalonWindow", "avButton1"))
        {
            return new ScenarioResult(false);
        }
        _edit1.SetFocus();
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        //Verify that Non-Modal AV Window has been closed.
        bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) <= 40,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%. Expected <= 40", p.log);

        return sr;
    }

    [Scenario("Launch a Modal AV Window from a WF Form that launches a Modal AV.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "AVModality", "wfButton1"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        p.log.WriteLine("Verify that Modal AV Window is launched.");
        bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) >= 50,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%", p.log);

        //Try to launch second modal AV window, should throw exception
        if (!GetEditControls(p, "AVModality", "wfButton3"))
        {
            return new ScenarioResult(false);
        }
        bool modality = false;
        try
        {
            _edit1.SetFocus();
        }
        catch (Exception)
        {
            modality = true;
        }
        sr.IncCounters(modality, "Failed at Modality check. Window is not modal.", p.log);

        //Close modal window
        if (!GetEditControls(p, "AvalonWindow", "avButton1"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        p.log.WriteLine("Verify that Modal AV Window has been closed.");
        bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) <= 40,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%", p.log);

        //Launch second modal AV window
        if (!GetEditControls(p, "AVModality", "wfButton3"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        p.log.WriteLine("Verify that Second Modal AV Window is launched.");
        bmp = Utilities.GetScreenBitmap(new Rectangle(300, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) >= 50,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%", p.log);

        //Close second modal window
        if (!GetEditControls(p, "AnotherAvalonWindow", "avButton2"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        p.log.WriteLine("Verify that Modal AV Window has been closed.");
        bmp = Utilities.GetScreenBitmap(new Rectangle(300, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) <= 40,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%", p.log);

        return sr;
    }

    [Scenario("Launch a Modal AV Window from a WF Form that launches a Non-Modal AV.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //First, launch Non-Modal AV Window
        if (!GetEditControls(p, "AVModality", "wfButton2"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        p.log.WriteLine("Verify that Non-Modal AV Window is launched.");
        bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) >= 50,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%", p.log);

        //Launch second (modal) AV window
        if (!GetEditControls(p, "AVModality", "wfButton3"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        p.log.WriteLine("Verify that second (modal) AV Window is launched.");
        bmp = Utilities.GetScreenBitmap(new Rectangle(300, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) >= 50,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%", p.log);

        //Verify modality of second (modal) AV window, should throw exception if it is modal
        if (!GetEditControls(p, "AVModality", "wfButton3"))
        {
            return new ScenarioResult(false);
        }
        bool modality = false;
        try
        {
            _edit1.SetFocus();
        }
        catch (Exception)
        {
            modality = true;
        }
        sr.IncCounters(modality, "Failed at Modality check. Window is not modal.", p.log);

        //Close second (modal) window
        if (!GetEditControls(p, "AnotherAvalonWindow", "avButton2"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        p.log.WriteLine("Verify that second (modal) AV Window has been closed.");
        bmp = Utilities.GetScreenBitmap(new Rectangle(300, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) <= 40,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%", p.log);

        //Close non-modal window
        if (!GetEditControls(p, "AvalonWindow", "avButton1"))
        {
            return new ScenarioResult(false);
        }
        _edit1.Click();
        Utilities.SleepDoEvents(20);

        p.log.WriteLine("Verify that Non-Modal AV Window has been closed.");
        bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
        Utilities.SleepDoEvents(20);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) <= 40,
            "Bitmap Failed at Color=" + Color.White + ". Percent match: " +
            BitmapsColorPercent(bmp, 0, 0, 300, 300, Color.White) + "%", p.log);

        return sr;
    }

    #endregion

    #region Helper Functions

    //Gets Mita wrapper controls from Avalon textbox and WinForm textbox, and passes them to _edit1 and 
    //_edit2 respectively.
    bool GetEditControls(TParams p, String window1, String control1)
    {
        UIObject uiApp = null;
        UIObject uiControl = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window1));
            uiControl = uiApp.Descendants.Find(UICondition.CreateFromId(control1));
            _edit1 = new Edit(uiControl);

            return true;
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls" + ex.ToString());
            return false;
        }
    }

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
//@ Launch a Modal AV Window from a WF Form.
//@ Launch a non Modal AV Windows from a WF Form.
//@ Launch a Modal AV Window from a WF Form that launches a Modal AV.
//@ Launch a Modal AV Window from a WF Form that launches a Non-Modal AV.