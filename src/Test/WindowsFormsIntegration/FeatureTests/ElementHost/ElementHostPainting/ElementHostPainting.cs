using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using SWF = System.Windows.Forms;
using SWC = System.Windows.Controls;
using SWM = System.Windows.Media;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

//
// Testcase:    ElementHostPainting
// Description: Verify that an ElementHost is rendered correctly. 
// Author:      a-rickyt
//
public class ElementHostPainting : ReflectBase
{
    #region Test case setup

    Bitmap bmp;
    ElementHost elementHost1;
    ElementHost elementHost2;
    ElementHost elementHost3;
    ElementHost elementHost4;


    public ElementHostPainting(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "ElementHostPainting Test";
        this.Size = new System.Drawing.Size(500, 500);
        this.Location = new System.Drawing.Point(0, 0);
        base.InitTest(p);
    }

    #endregion

    #region Scenarios

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        base.AfterScenario(p, scenario, result);
    }

    [Scenario("Show a form that contains ElementHost. Verify the ElementHost is painted correctly.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Location = new System.Drawing.Point(0, 0);
        Controls.Add(elementHost1);
        elementHost1.BackColor = Color.White;

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.Location = new System.Drawing.Point(0, 200);
        Controls.Add(elementHost2);
        elementHost2.BackColor = Color.White;

        //Create Element Host 3
        elementHost3 = new ElementHost();
        elementHost3.Location = new System.Drawing.Point(200, 0);
        Controls.Add(elementHost3);
        elementHost3.BackColor = Color.White;

        //Create Element Host 4
        elementHost4 = new ElementHost();
        elementHost4.Location = new System.Drawing.Point(200, 200);
        Controls.Add(elementHost4);
        elementHost4.BackColor = Color.White;

        Utilities.SleepDoEvents(5);

        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.White, 100);
        Verify(sr, p, elementHost3, Color.White, 100);
        Verify(sr, p, elementHost4, Color.White, 100);

        elementHost1.BackColor = Color.White;
        elementHost2.BackColor = Color.Yellow;
        elementHost3.BackColor = Color.Blue;
        elementHost4.BackColor = Color.Green;
        Utilities.SleepDoEvents(5);

        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        AddControls(); //Add controls helper function
        Utilities.SleepDoEvents(10);

        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        return sr;
    }

    [Scenario("Resize an elementHost. Verify ElementHost was painted correctly. ")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.Location = new System.Drawing.Point(0, 0);
        Controls.Add(elementHost1);
        elementHost1.BackColor = Color.White;

        elementHost1.Size = new System.Drawing.Size(100, 50);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);

        elementHost1.Size = new System.Drawing.Size(300, 300);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);

        elementHost1.Size = new System.Drawing.Size(200, 200);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.Location = new System.Drawing.Point(0, 200);
        elementHost2.Size = new System.Drawing.Size(100, 250);
        Controls.Add(elementHost2);
        elementHost2.BackColor = Color.White;

        //Create Element Host 3
        elementHost3 = new ElementHost();
        elementHost3.Location = new System.Drawing.Point(200, 0);
        elementHost3.Height = 50;
        Controls.Add(elementHost3);
        elementHost3.BackColor = Color.White;

        //Create Element Host 4
        elementHost4 = new ElementHost();
        elementHost4.Location = new System.Drawing.Point(200, 200);
        elementHost4.Height = 250;
        elementHost4.Width = 250;
        Controls.Add(elementHost4);
        elementHost4.BackColor = Color.White;

        Utilities.SleepDoEvents(5);

        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.White, 100);
        Verify(sr, p, elementHost3, Color.White, 100);
        Verify(sr, p, elementHost4, Color.White, 100);

        elementHost1.BackColor = Color.White;
        elementHost2.BackColor = Color.Yellow;
        elementHost3.BackColor = Color.Blue;
        elementHost4.BackColor = Color.Green;
        Utilities.SleepDoEvents(5);

        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        AddControls(); //Add controls helper function
        Utilities.SleepDoEvents(10);

        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        return sr;
    }

    [Scenario("Verify that an ElementHost is painted correctly after the parent-form was minimized and restored.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.BackColor = Color.White;
        elementHost1.Location = new System.Drawing.Point(0, 0);
        elementHost1.Size = new System.Drawing.Size(200, 200);
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.BackColor = Color.White;
        elementHost2.Location = new System.Drawing.Point(0, 200);
        elementHost2.Size = new System.Drawing.Size(100, 250);
        Controls.Add(elementHost2);

        //Create Element Host 3
        elementHost3 = new ElementHost();
        elementHost3.BackColor = Color.White;
        elementHost3.Location = new System.Drawing.Point(200, 0);
        elementHost3.Height = 50;
        Controls.Add(elementHost3);

        //Create Element Host 4
        elementHost4 = new ElementHost();
        elementHost4.BackColor = Color.Green;
        elementHost4.Location = new System.Drawing.Point(200, 200);
        elementHost4.Height = 250;
        elementHost4.Width = 250;
        Controls.Add(elementHost4);

        AddControls(); //Add controls helper function

        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
        Utilities.SleepDoEvents(20);
        this.WindowState = System.Windows.Forms.FormWindowState.Normal;
        Utilities.SleepDoEvents(10);

        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        return sr;
    }

    [Scenario("Verify that an ElementHost is painted correctly after the parent-form was maximized and restored.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.BackColor = Color.White;
        elementHost1.Location = new System.Drawing.Point(0, 0);
        elementHost1.Size = new System.Drawing.Size(200, 200);
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.BackColor = Color.White;
        elementHost2.Location = new System.Drawing.Point(0, 200);
        elementHost2.Size = new System.Drawing.Size(100, 250);
        Controls.Add(elementHost2);

        //Create Element Host 3
        elementHost3 = new ElementHost();
        elementHost3.BackColor = Color.White;
        elementHost3.Location = new System.Drawing.Point(200, 0);
        elementHost3.Height = 50;
        Controls.Add(elementHost3);

        //Create Element Host 4
        elementHost4 = new ElementHost();
        elementHost4.BackColor = Color.Green;
        elementHost4.Location = new System.Drawing.Point(200, 200);
        elementHost4.Height = 250;
        elementHost4.Width = 250;
        Controls.Add(elementHost4);

        AddControls(); //Add controls helper function

        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        Utilities.SleepDoEvents(20);
        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        this.WindowState = System.Windows.Forms.FormWindowState.Normal;
        Utilities.SleepDoEvents(20);
        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        return sr;
    }

    [Scenario("Verify that an docked ElementHost is painted correctly while the form is resized.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.BackColor = Color.White;
        elementHost1.Dock = SWF.DockStyle.Left;
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.BackColor = Color.Yellow;
        elementHost2.Dock = SWF.DockStyle.Right;
        Controls.Add(elementHost2);

        //Create Element Host 3
        elementHost3 = new ElementHost();
        elementHost3.BackColor = Color.Blue;
        elementHost3.Dock = SWF.DockStyle.Top;
        Controls.Add(elementHost3);

        //Create Element Host 4
        elementHost4 = new ElementHost();
        elementHost4.BackColor = Color.Green;
        elementHost4.Dock = SWF.DockStyle.Bottom;
        Controls.Add(elementHost4);

        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        //Resize
        this.Size = new System.Drawing.Size(700, 700);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        this.Size = new System.Drawing.Size(300, 300);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);

        //elementHost2 is clipped, verify it separately
        bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(5);
        sr.IncCounters(BitmapsColorPercent(bmp, 200, 100, this.ClientSize.Width - 200,
            this.ClientSize.Height - 200, Color.Yellow) >= 100,
            "Bitmap Failed at Color=" + Color.Yellow + ". Percent match: " +
            BitmapsColorPercent(bmp, 200, 100, this.ClientSize.Width - 200,
            this.ClientSize.Height - 200, Color.Yellow) + "%", p.log);

        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);
        
        this.Size = new System.Drawing.Size(500, 500);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        AddControls(); //Add controls helper function

        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        //Resize
        this.Size = new System.Drawing.Size(700, 700);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        this.Size = new System.Drawing.Size(300, 300);
        Utilities.SleepDoEvents(10);
        Verify(sr, p, elementHost1, Color.White, 75);

        //elementHost2 is clipped, verify it separately
        bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 200, 100, this.ClientSize.Width - 200,
            this.ClientSize.Height - 200, Color.Yellow) >= 100,
            "Bitmap Failed at Color=" + Color.Yellow + ". Percent match: " +
            BitmapsColorPercent(bmp, 200, 100, this.ClientSize.Width - 200,
            this.ClientSize.Height - 200, Color.Yellow) + "%", p.log);

        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        this.Size = new System.Drawing.Size(500, 500);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        return sr;
    }

    [Scenario("Verify that an ElementHost is painted correctly while the parent-form is moved. ")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.BackColor = Color.White;
        elementHost1.Dock = SWF.DockStyle.Left;
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.BackColor = Color.Yellow;
        elementHost2.Dock = SWF.DockStyle.Right;
        Controls.Add(elementHost2);

        //Create Element Host 3
        elementHost3 = new ElementHost();
        elementHost3.BackColor = Color.Blue;
        elementHost3.Dock = SWF.DockStyle.Top;
        Controls.Add(elementHost3);

        //Create Element Host 4
        elementHost4 = new ElementHost();
        elementHost4.BackColor = Color.Green;
        elementHost4.Dock = SWF.DockStyle.Bottom;
        Controls.Add(elementHost4);

        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        //Move parent form
        this.Location = new System.Drawing.Point(50, 150);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        this.Location = new System.Drawing.Point(400, 50);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        this.Location = new System.Drawing.Point(0, 0);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        AddControls(); //Add controls helper function

        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        //Move parent form
        this.Location = new System.Drawing.Point(200, 50);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        this.Location = new System.Drawing.Point(100, 100);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        this.Location = new System.Drawing.Point(0, 0);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        return sr;
    }

    [Scenario("Use another window to cover/uncover the parent-form and verify that an ElementHost is rendered correctly after the parent-form is uncovered.")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.BackColor = Color.White;
        elementHost1.Dock = SWF.DockStyle.Left;
        Controls.Add(elementHost1);

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.BackColor = Color.Yellow;
        elementHost2.Dock = SWF.DockStyle.Right;
        Controls.Add(elementHost2);

        //Create Element Host 3
        elementHost3 = new ElementHost();
        elementHost3.BackColor = Color.Blue;
        elementHost3.Dock = SWF.DockStyle.Top;
        Controls.Add(elementHost3);

        //Create Element Host 4
        elementHost4 = new ElementHost();
        elementHost4.BackColor = Color.Green;
        elementHost4.Dock = SWF.DockStyle.Bottom;
        Controls.Add(elementHost4);

        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        //Cover with another form
        SWF.Form newForm = new SWF.Form();
        newForm.Size = new System.Drawing.Size(600, 600);
        newForm.StartPosition = SWF.FormStartPosition.Manual;
        newForm.Location = new System.Drawing.Point(0, 0);
        newForm.Text = "New Form";
        newForm.Show();
        Utilities.SleepDoEvents(20);
        newForm.Close();

        Utilities.SleepDoEvents(10);
        Verify(sr, p, elementHost1, Color.White, 100);
        Verify(sr, p, elementHost2, Color.Yellow, 100);
        Verify(sr, p, elementHost3, Color.Blue, 100);
        Verify(sr, p, elementHost4, Color.Green, 100);

        AddControls(); //Add controls helper function

        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        //Cover with another form
        newForm = new SWF.Form();
        newForm.Size = new System.Drawing.Size(600, 600);
        newForm.StartPosition = SWF.FormStartPosition.Manual;
        newForm.Location = new System.Drawing.Point(0, 0);
        newForm.Text = "New Form";
        newForm.Show();
        Utilities.SleepDoEvents(20);
        newForm.Close();

        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, Color.White, 75);
        Verify(sr, p, elementHost2, Color.Yellow, 75);
        Verify(sr, p, elementHost3, Color.White, 75);
        Verify(sr, p, elementHost4, Color.Green, 75);

        return sr;
    }

    [Scenario("Set the BackgroundImage on the parent-form. Set the BackColor=Color.Transparent on an ElementHost. Verify that the parent-form is rendered without issues while resizing (Check WindowsOSBug#1372737).")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        Bitmap background = new Bitmap("Greenstone.bmp");
        //(140,148,123) is the dominant color of Greenstone.bmp
        Color greenstone = Color.FromArgb(140, 148, 123);
        this.BackgroundImage = background;

        //Create Element Host 1
        elementHost1 = new ElementHost();
        Controls.Add(elementHost1);
        elementHost1.BackColorTransparent = true;

        //Create Element Host 2
        elementHost2 = new ElementHost();
        elementHost2.BackColorTransparent = true;
        elementHost2.BackColor = Color.Transparent;
        elementHost2.Dock = SWF.DockStyle.Right;
        Controls.Add(elementHost2);

        //Create Element Host 3
        elementHost3 = new ElementHost();
        elementHost3.BackColorTransparent = true;
        elementHost3.BackColor = Color.Transparent;
        elementHost3.Dock = SWF.DockStyle.Bottom;
        Controls.Add(elementHost3);

        Utilities.SleepDoEvents(20);
        Verify(sr, p, elementHost1, greenstone, 40);
        Verify(sr, p, elementHost2, greenstone, 40);
        Verify(sr, p, elementHost3, greenstone, 40);

        //Resize
        this.Size = new System.Drawing.Size(700, 700);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, greenstone, 40);
        Verify(sr, p, elementHost2, greenstone, 40);
        Verify(sr, p, elementHost3, greenstone, 40);

        this.Size = new System.Drawing.Size(300, 300);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, greenstone, 40);
        Verify(sr, p, elementHost2, greenstone, 40);
        Verify(sr, p, elementHost3, greenstone, 40);

        this.Size = new System.Drawing.Size(500, 500);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, greenstone, 40);
        Verify(sr, p, elementHost2, greenstone, 40);
        Verify(sr, p, elementHost3, greenstone, 40);

        //Add controls
        MyButton avButton = new MyButton();
        avButton.Content = "Avalon Button";
        avButton.Background = SWM.Brushes.Transparent;
        elementHost1.Child = avButton;

        SWC.Label avLabel = new SWC.Label();
        avLabel.Content = "Avalon Label";
        avLabel.Background = SWM.Brushes.Transparent;
        elementHost2.Child = avLabel;

        SWC.TextBox avTextBox = new SWC.TextBox();
        avTextBox.Text = "Avalon TextBox";
        avTextBox.Background = SWM.Brushes.Transparent;
        elementHost3.Child = avTextBox;

        Utilities.SleepDoEvents(5);

        Verify(sr, p, elementHost1, greenstone, 14);
        Verify(sr, p, elementHost2, greenstone, 30);
        Verify(sr, p, elementHost3, greenstone, 30);

        //Resize
        this.Size = new System.Drawing.Size(700, 700);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, greenstone, 14);
        Verify(sr, p, elementHost2, greenstone, 30);
        Verify(sr, p, elementHost3, greenstone, 30);

        this.Size = new System.Drawing.Size(300, 300);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, greenstone, 14);
        Verify(sr, p, elementHost2, greenstone, 30);
        Verify(sr, p, elementHost3, greenstone, 30);

        this.Size = new System.Drawing.Size(500, 500);
        Utilities.SleepDoEvents(5);
        Verify(sr, p, elementHost1, greenstone, 14);
        Verify(sr, p, elementHost2, greenstone, 30);
        Verify(sr, p, elementHost3, greenstone, 30);

        //p.log.LogKnownBug(BugDb.WindowsOSBugs, 1615988, "BackgroundImage doesn't show on transparent ElementHost");
        return sr;
    }

    [Scenario("Add an AV ScrollViewer control to an AV textbox.  Ensure the ElementHost background is not visible in the lower right-hand corner (Check WindowsOSBug#1595357).")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.BackColor = Color.Red;
        elementHost1.Location = new System.Drawing.Point(0, 0);
        Controls.Add(elementHost1);

        //Add AV textbox
        SWC.TextBox avTextBox = new SWC.TextBox();
        avTextBox.Text = "Avalon TextBox";

        //Add ScrollViewer
        SWC.ScrollViewer scrollViewer = new SWC.ScrollViewer();
        scrollViewer.Content = avTextBox;
        scrollViewer.VerticalScrollBarVisibility = SWC.ScrollBarVisibility.Visible;
        scrollViewer.HorizontalScrollBarVisibility = SWC.ScrollBarVisibility.Visible;
        elementHost1.Child = scrollViewer;
        elementHost1.BackColorTransparent = true;

        Utilities.SleepDoEvents(10);
        Verify(sr, p, elementHost1, Color.White, 60);

        bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(BitmapsColorPercent(bmp, 0, 0, 200, 100, Color.Red) == 0, p.log, 
            BugDb.WindowsOSBugs, 1595357, "ElementHost background is visible.");

        return sr;
    }

    #endregion

    #region Utilities

    //Helper function to add controls to the 4 element hosts
    private void AddControls()
    {
        MyButton avButton = new MyButton();
        avButton.Content = "Avalon Button";
        avButton.Background = SWM.Brushes.White;

        elementHost1.Child = avButton;

        SWC.Label avLabel = new SWC.Label();
        avLabel.Content = "Avalon Label";
        avLabel.Background = SWM.Brushes.Yellow;
        elementHost2.Child = avLabel;

        SWC.TextBox avTextBox = new SWC.TextBox();
        avTextBox.Text = "Avalon TextBox";
        elementHost3.Child = avTextBox;

        SWC.StackPanel avStackPanel = new SWC.StackPanel();
        SWC.RadioButton avRadioButton1 = new SWC.RadioButton();
        SWC.RadioButton avRadioButton2 = new SWC.RadioButton();
        avRadioButton1.Content = "Avalon RadioButton 1";
        avRadioButton2.Content = "Avalon RadioButton 2";
        avStackPanel.Children.Add(avRadioButton1);
        avStackPanel.Children.Add(avRadioButton2);
        elementHost4.Child = avStackPanel;
        Utilities.SleepDoEvents(2);
    }

    class MyButton : System.Windows.Controls.Primitives.ButtonBase
    {

    }

    //Verifies the painting of an element host by taking a bitmap of the form and checking 
    //the color of pixels of the bitmap in the element host area by calling BitmapsColorPercent.
    private void Verify(ScenarioResult sr, TParams p, ElementHost elementHost, Color color, int percent)
    {
        bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(2);
        sr.IncCounters(BitmapsColorPercent(bmp, elementHost.Location.X, elementHost.Location.Y,
            elementHost.Width, elementHost.Height, color) >= percent,
            "Bitmap Failed at Color=" + color + ". Percent match: " +
            BitmapsColorPercent(bmp, elementHost.Location.X, elementHost.Location.Y,
            elementHost.Width, elementHost.Height, color) + "%", p.log);
    }

    /// <summary>
    /// Returns the percentage of matching pixels to total pixels with the same ARGB value as the given color,
    /// in the area specified by width and height starting at location (x, y).  The algorithm checks each
    /// pixel in the bitmap and increments the match counter each time it finds a color match.
    /// </summary>
    /// <param name="bmp">Bitmap to search.</param>
    /// <param name="x">X-axis starting location.</param>
    /// <param name="y">Y-axis starting location.</param>
    /// <param name="width">Width of area to search.</param>
    /// <param name="height">Height of area to search.</param>
    /// <param name="color">Color to search for.</param>
    /// <returns>Percentage of matching pixels to total pixels.</returns>
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
//@ Show a form that contains ElementHost. Verify the ElementHost is painted correctly.
//@ Resize an elementHost. Verify ElementHost was painted correctly. 
//@ Verify that an ElementHost is painted correctly after the parent-form was minimized and restored.
//@ Verify that an ElementHost is painted correctly after the parent-form was maximized and restored.
//@ Verify that an docked ElementHost is painted correctly while the form is resized.
//@ Verify that an ElementHost is painted correctly while the parent-form is moved. 
//@ Use another window to cover/uncover the parent-form and verify that an ElementHost is rendered correctly after the parent-form is uncovered.
//@ Set the BackgroundImage on the parent-form. Set the BackColor=Color.Transparent on an ElementHost. Verify that the parent-form is rendered without issues while resizing (Check WindowsOSBug#1372737).
//@ Add an AV ScrollViewer control to an AV textbox.  Ensure the ElementHost background is not visible in the lower right-hand corner (Check WindowsOSBug#1595357).