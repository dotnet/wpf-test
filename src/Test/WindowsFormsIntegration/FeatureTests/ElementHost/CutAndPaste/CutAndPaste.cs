// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using System.Windows.Shapes;
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
// Testcase:    CutAndPaste
// Description: This test is to verify that clipboard functionality works 
//              1) within a EH, 2) Across multiple EH's, and 3) between EH's and Avalon.
//
public class CutAndPaste : ReflectBase
{
    #region Testcase setup

    Edit _edit1;
    Edit _edit2;
    ElementHost _elementHost1 = new ElementHost();
    ElementHost _elementHost2 = new ElementHost();
    ElementHost _elementHost3 = new ElementHost();
    ElementHost _elementHost4 = new ElementHost();
    ElementHost _elementHost5 = new ElementHost();
    SWC.StackPanel _stackPanel1 = new SWC.StackPanel();
    SWC.StackPanel _stackPanel2 = new SWC.StackPanel();
    SWC.TextBox _avTextBox1 = new SWC.TextBox();
    SWC.TextBox _avTextBox2 = new SWC.TextBox();
    SWC.TextBox _avTextBox3 = new SWC.TextBox();
    SWC.TextBox _avTextBox4 = new SWC.TextBox();
    SWC.RichTextBox _avRichTextBox1 = new SWC.RichTextBox();
    SWC.RichTextBox _avRichTextBox2 = new SWC.RichTextBox();
    SWF.RichTextBox _wfRichTextBox1 = new SWF.RichTextBox();
    public SWF.Label label1 = new SWF.Label();
    public SWF.Label label2 = new SWF.Label();
    Process _notepad;

    public CutAndPaste(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "CutAndPaste";
        this.Size = new System.Drawing.Size(400, 400);

        _avTextBox1.Name = "avTextBox1";
        _avTextBox1.Text = "Avalon TextBox 1";

        _avTextBox2.Name = "avTextBox2";

        _avTextBox3.Name = "avTextBox3";
        _avTextBox3.Text = "Avalon TextBox 3";

        _avTextBox4.Name = "avTextBox4";

        _stackPanel1.Children.Add(_avTextBox1);
        _stackPanel1.Children.Add(_avTextBox2);

        _avRichTextBox1.Name = "avRichTextBox1";
        _avRichTextBox2.Name = "avRichTextBox2";
        _avRichTextBox1.Height = 50;
        _avRichTextBox2.Height = 50;
        _avRichTextBox1.SelectionChanged += new RoutedEventHandler(avRichTextBox1_SelectionChanged);
        _avRichTextBox2.SelectionChanged += new RoutedEventHandler(avRichTextBox2_SelectionChanged);

        Ellipse ellipse = new Ellipse();
        ellipse.Width = 100;
        ellipse.Height = 30;
        ellipse.Fill = System.Windows.Media.Brushes.Blue;
        ellipse.Stroke = System.Windows.Media.Brushes.Red;
        new System.Windows.Documents.InlineUIContainer(ellipse, _avRichTextBox1.Selection.Start);

        _wfRichTextBox1.Name = "wfRichTextBox1";
        _wfRichTextBox1.Location = new System.Drawing.Point(20, 180);
	_wfRichTextBox1.Width = 150;
        _wfRichTextBox1.SelectionChanged += new EventHandler(wfRichTextBox1_SelectionChanged);
        Controls.Add(_wfRichTextBox1);

        label1.AutoSize = true;
        Controls.Add(label1);
        label2.AutoSize = true;
        Controls.Add(label2);

        //Create Element Host 1
        _elementHost1.Name = "elementHost1";
        _elementHost1.Child = _stackPanel1;
        _elementHost1.Location = new System.Drawing.Point(20, 20);
        _elementHost1.BackColor = Color.Red;
        _elementHost1.AutoSize = true;
        Controls.Add(_elementHost1);

        //Create Element Host 2
        _elementHost2.Name = "elementHost2";
        _elementHost2.Child = _avTextBox3;
        _elementHost2.Location = new System.Drawing.Point(20, 100);
        _elementHost2.BackColor = Color.Red;
        _elementHost2.AutoSize = true;
        Controls.Add(_elementHost2);

        //Create Element Host 3
        _elementHost3.Name = "elementHost3";
        _elementHost3.Child = _avTextBox4;
        _elementHost3.Location = new System.Drawing.Point(20, 130);
        _elementHost3.BackColor = Color.Red;
        _elementHost3.AutoSize = true;
        Controls.Add(_elementHost3);

        //Create Element Host 4
        _elementHost4.Name = "elementHost4";
        _elementHost4.Child = _avRichTextBox1;
        _elementHost4.Location = new System.Drawing.Point(180, 20);
        _elementHost4.BackColor = Color.Red;
        Controls.Add(_elementHost4);

        //Create Element Host 5
        _elementHost5.Name = "elementHost5";
        _elementHost5.Child = _avRichTextBox2;
        _elementHost5.Location = new System.Drawing.Point(180, 160);
        _elementHost5.BackColor = Color.Red;
        Controls.Add(_elementHost5);

        base.InitTest(p);
    }

    void wfRichTextBox1_SelectionChanged(object sender, EventArgs e)
    {
        label1.Text = _wfRichTextBox1.Text;
    }
    void avRichTextBox1_SelectionChanged(object sender, RoutedEventArgs e)
    {
        System.Windows.Documents.TextRange textRange1 = new System.Windows.Documents.TextRange(
            _avRichTextBox1.Document.ContentStart, _avRichTextBox1.Document.ContentEnd);

        label1.Text = textRange1.Text;
    }
    void avRichTextBox2_SelectionChanged(object sender, RoutedEventArgs e)
    {
        System.Windows.Documents.TextRange textRange2 = new System.Windows.Documents.TextRange(
            _avRichTextBox2.Document.ContentStart, _avRichTextBox2.Document.ContentEnd);

        label2.Text = textRange2.Text;
    }

    #endregion

    #region Scenarios

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        if(scenario.Name == "Scenario4")
        {
            WinFormWindow window = new WinFormWindow(this);
            window.Show();
        }
        if (scenario.Name == "Scenario5")
        {
            WinFormWindow window = new WinFormWindow(this);
            window.Show();
        }
        if (scenario.Name == "Scenario6")
        {
            _notepad = Process.Start(Environment.SystemDirectory + @"\notepad.exe");
	    Utilities.SleepDoEvents(10);
        }
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult sr)
    {
        if (scenario.Name == "Scenario6")
        {
            _notepad.Kill();
        }

        base.AfterScenario(p, scenario, sr);
    }

    [Scenario("Cut, Copy and Paste between EH controls in a single EH.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "CutAndPaste", "avTextBox1", "CutAndPaste", "avTextBox2")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        Utilities.SleepDoEvents(10);

        //Cut and Paste
        Cut(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(_edit1.Value == "" && _edit2.Value == "Avalon TextBox 1",
            "Failed at Cut and Paste between EH controls in a single EH. \nTextBox1: " +
            _edit1.Value + "\nTextBox2: " + _edit2.Value, p.log);

        //Copy and Paste
        Copy(_edit2);
        Paste(_edit1);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(_edit1.Value == "Avalon TextBox 1" && _edit2.Value == "Avalon TextBox 1",
            "Failed at Copy and Paste between EH controls in a single EH. \nTextBox1: " +
            _edit1.Value + "\nTextBox2: " + _edit2.Value, p.log);

        return sr;
    }

    [Scenario("Cut, Copy and Paste between EH controls in separate EH's.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "CutAndPaste", "avTextBox3", "CutAndPaste", "avTextBox4")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        Utilities.SleepDoEvents(10);

        //Cut and Paste
        Cut(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(_edit1.Value == "" && _edit2.Value == "Avalon TextBox 3",
            "Failed at Cut and Paste between EH controls in separate EH's. \nTextBox1: " +
            _edit1.Value + "\nTextBox2: " + _edit2.Value, p.log);

        //Copy and Paste
        Copy(_edit2);
        Paste(_edit1);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(_edit1.Value == "Avalon TextBox 3" && _edit2.Value == "Avalon TextBox 3",
            "Failed at Copy and Paste between EH controls in separate EH's. \nTextBox1: " +
            _edit1.Value + "\nTextBox2: " + _edit2.Value, p.log);

        return sr;
    }

    [Scenario("Cut, Copy and Paste between a EH control in a EH and an Winform RichTextbox using it's clipboard functionality.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "CutAndPaste", "avTextBox3", "CutAndPaste", "wfRichTextBox1")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        Utilities.SleepDoEvents(10);

        //Cut and Paste from Avalon TextBox to WinForm RichTextBox
        Cut(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        //Need to use label.Text instead of _edit2.Value because Value is not supported for RichTextBox
        sr.IncCounters(_edit1.Value == "" && label1.Text == "Avalon TextBox 3",
            "Failed at Cut and Paste between EH control and Winform RichTextbox. \nTextBox1: " +
            _edit1.Value + "\nRichTextBox: " + label1.Text, p.log);

        //Cut and Paste from WinForm RichTextBox to Avalon TextBox
        Cut(_edit2);
        Paste(_edit1);
        Utilities.SleepDoEvents(10);
        //Need to use label.Text instead of _edit2.Value because Value is not supported for RichTextBox
        sr.IncCounters(_edit1.Value == "Avalon TextBox 3" && label1.Text == "",
            "Failed at Cut and Paste between EH control and Winform RichTextbox. \nTextBox1: " +
            _edit1.Value + "\nRichTextBox2: " + label1.Text, p.log);

        //Copy and Paste
        Copy(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(_edit1.Value == "Avalon TextBox 3" && label1.Text == "Avalon TextBox 3",
            "Failed at Copy and Paste between EH control and Winform RichTextbox. \nTextBox1: " +
            _edit1.Value + "\nRichTextBox2: " + label1.Text, p.log);

        return sr;
    }

    [Scenario("Cut, Copy and Paste between a EH control in a EH and an Winform RichTextbox on a different Winform window.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "CutAndPaste", "avTextBox3", "WinFormWindow", "wfRichTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        Utilities.SleepDoEvents(10);

        //Cut and Paste from Avalon TextBox to WinForm RichTextBox
        Cut(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        //Need to use label.Text instead of _edit2.Value because Value is not supported for RichTextBox
        sr.IncCounters(_edit1.Value == "" && label1.Text == "Avalon TextBox 3",
            "Failed at Cut and Paste between EH control and Winform RichTextbox. \nTextBox1: " +
            _edit1.Value + "\nRichTextBox: " + label1.Text, p.log);

        //Cut and Paste from WinForm RichTextBox to Avalon TextBox
        Cut(_edit2);
        Paste(_edit1);
        Utilities.SleepDoEvents(10);
        //Need to use label.Text instead of _edit2.Value because Value is not supported for RichTextBox
        sr.IncCounters(_edit1.Value == "Avalon TextBox 3" && label1.Text == "",
            "Failed at Cut and Paste between EH control and Winform RichTextbox. \nTextBox1: " +
            _edit1.Value + "\nRichTextBox: " + label1.Text, p.log);

        //Copy and Paste
        Copy(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(_edit1.Value == "Avalon TextBox 3" && label1.Text == "Avalon TextBox 3",
            "Failed at Copy and Paste between EH control and Winform RichTextbox. \nTextBox1: " +
            _edit1.Value + "\nRichTextBox: " + label1.Text, p.log);
        
        return sr;
    }

    [Scenario("Cut, Copy and Paste between a EH control in a EH and a EH RichTextbox on a different Windows Form.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "CutAndPaste", "avTextBox3", "WinFormWindow", "avRichTextBox")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }

        Utilities.SleepDoEvents(10);

        //Cut and Paste from Avalon TextBox to Avalon RichTextBox
        Cut(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        //Need to use label.Text instead of _edit2.Value because Value is not supported for RichTextBox
        sr.IncCounters(_edit1.Value == "" && label1.Text == "Avalon TextBox 3\r\n",
            "Failed at Cut and Paste from Avalon TextBox to Avalon RichTextBox. \nTextBox1: " +
            _edit1.Value + "\nRichTextBox: " + label1.Text, p.log);

        //Cut and Paste from Avalon RichTextBox to Avalon TextBox
        Cut(_edit2);
        Paste(_edit1);
        Utilities.SleepDoEvents(10);
        //Need to use label.Text instead of _edit2.Value because Value is not supported for RichTextBox
        sr.IncCounters(_edit1.Value == "Avalon TextBox 3" && label1.Text == "",
            "Failed at Cut and Paste from Avalon RichTextBox to Avalon TextBox. \nTextBox1: " +
            _edit1.Value + ". Expected: Avalon TextBox 3.\nRichTextBox: " + label1.Text +
            ". Expected: .", p.log);

        //Copy and Paste
        Copy(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(_edit1.Value == "Avalon TextBox 3" && label1.Text == "Avalon TextBox 3\r\n",
            "Failed at Copy and Paste between EH control and Avalon RichTextbox. \nTextBox1: " +
            _edit1.Value + ". Expected: Avalon TextBox 3.\nRichTextBox: " + label1.Text + 
            ". Expected: Avalon TextBox 3.", p.log);

        return sr;
    }

    [Scenario("Cut, Copy and Paste between EH controls in a single EH and Notepad.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("CutAndPaste"));
        UIObject uiControl1 = uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox3"));
        _edit1 = new Edit(uiControl1);

        Utilities.SleepDoEvents(20);
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromClassName("Notepad"));
        UIObject uiControl2 = uiApp.Descendants.Find(UICondition.CreateFromClassName("Edit"));
        _edit2 = new Edit(uiControl2);

        Utilities.SleepDoEvents(10);

        //Cut and Paste from Avalon TextBox to Notepad
        Cut(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.Value == "",
            "Failed at Cut and Paste from Avalon TextBox to Notepad. \nTextBox1: " +
            _edit1.Value + "\nNotepad: " + label1.Text, p.log);

        //Cut and Paste from Notepad to Avalon TextBox
        Cut(_edit2);
        Paste(_edit1);
        Utilities.SleepDoEvents(10);

        sr.IncCounters(_edit1.Value == "Avalon TextBox 3",
            "Failed at Cut and Paste from Avalon RichTextBox to Notepad. \nTextBox1: " +
            _edit1.Value + "\nNotepad: " + label1.Text, p.log);

        //Copy and Paste
        Copy(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(_edit1.Value == "Avalon TextBox 3",
            "Failed at Copy and Paste between EH control and Notepad. \nTextBox1: " +
            _edit1.Value + "\nNotepad: " + label1.Text, p.log);

        return sr;
    }

    //Scenario temporarily removed.  Avalon currently does not support copying images (by design).
    //See WindowsOSBugs 1532320 for more details.
    /*
    [Scenario("Cut, Copy and Paste Image between EH controls in a single EH.")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "CutAndPaste", "avRichTextBox1", "CutAndPaste", "avRichTextBox2")) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        Bitmap bmpOriginal = Utilities.GetBitmapOfControl(elementHost4);
        bmpOriginal.Save("ellipse.jpg");
        Bitmap bmpEmpty = Utilities.GetBitmapOfControl(elementHost5);
        Bitmap bmp1;
        Bitmap bmp2;
        Utilities.SleepDoEvents(10);

        //Cut and Paste from avRichTextBox1 to avRichTextBox2
        Cut(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        bmp1 = Utilities.GetBitmapOfControl(elementHost4);
        bmp2 = Utilities.GetBitmapOfControl(elementHost5);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(Utilities.BitmapsIdentical(bmp1, bmpEmpty) && 
            Utilities.BitmapsIdentical(bmp2, bmpOriginal),
            "Failed at Cut and Paste Image from avRichTextBox1 to avRichTextBox2.", p.log);

        //Cut and Paste from avRichTextBox2 to avRichTextBox1
        Cut(_edit2);
        Paste(_edit1);
        Utilities.SleepDoEvents(10);
        bmp1 = Utilities.GetBitmapOfControl(elementHost4);
        bmp2 = Utilities.GetBitmapOfControl(elementHost5);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(Utilities.BitmapsIdentical(bmp1, bmpOriginal) &&
            Utilities.BitmapsIdentical(bmp2, bmpEmpty),
            "Failed at Cut and Paste Image from avRichTextBox2 to avRichTextBox1.", p.log);

        //Copy and Paste
        Copy(_edit1);
        Paste(_edit2);
        Utilities.SleepDoEvents(10);
        bmp1 = Utilities.GetBitmapOfControl(elementHost4);
        bmp2 = Utilities.GetBitmapOfControl(elementHost5);
        Utilities.SleepDoEvents(10);
        sr.IncCounters(Utilities.BitmapsIdentical(bmp1, bmpOriginal) &&
            Utilities.BitmapsIdentical(bmp2, bmpOriginal),
            "Failed at Copy and Paste Image between EH controls in a single EH.", p.log);
       
        p.log.LogKnownBug(BugDb.WindowsOSBugs, 1532320, 
            "Known bug: Cannot cut/copy/paste image from Avalon RichTextBox");
       
        return sr;
    }
    */

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

    public void Cut(UIObject source)
    {
        using (PointerInput.Activate(Mouse.Instance))
        {
            source.SendKeys("{HOME}");
            System.Threading.Thread.Sleep(100);
            source.SendKeys("+{END}");
            System.Threading.Thread.Sleep(100);

            source.SendKeys("^x");
        }
    }

    public void Copy(UIObject source)
    {
        using (PointerInput.Activate(Mouse.Instance))
        {
            source.SendKeys("{HOME}");
            System.Threading.Thread.Sleep(100);
            source.SendKeys("+{END}");
            System.Threading.Thread.Sleep(100);

            source.SendKeys("^c");
        }
    }

    public void Paste(UIObject source)
    {
        using (PointerInput.Activate(Mouse.Instance))
        {
            System.Threading.Thread.Sleep(100);
            source.SendKeys("^v");
        }
    }

    #endregion
}

#region WinFormWindow

//class WinFormWindow creates a new WinForm window with one RichTextBox
public class WinFormWindow : SWF.Form
{
    CutAndPaste _parent;
    public SWF.RichTextBox wfRichTextBox = new SWF.RichTextBox();
    public SWC.RichTextBox avRichTextBox = new SWC.RichTextBox();
    ElementHost _elementHost1 = new ElementHost();

    public WinFormWindow(CutAndPaste parent)
    {
        this._parent = parent;
        this.SuspendLayout();
        // 
        // wfRichTextBox
        // 
        this.wfRichTextBox.Location = new System.Drawing.Point(95, 63);
        this.wfRichTextBox.Name = "wfRichTextBox";
        this.wfRichTextBox.Size = new System.Drawing.Size(130, 20);
        this.wfRichTextBox.SelectionChanged += new EventHandler(wfRichTextBox_SelectionChanged);
        this.Controls.Add(this.wfRichTextBox);
        // 
        // avRichTextBox
        // 
        this.avRichTextBox.Name = "avRichTextBox";
        this.avRichTextBox.SelectionChanged += new RoutedEventHandler(avRichTextBox_SelectionChanged);
        // 
        // elementHost1
        // 
        this._elementHost1.Location = new System.Drawing.Point(25, 123);
        this._elementHost1.Name = "elementHost1";
        this._elementHost1.Child = avRichTextBox;
        //this.elementHost1.AutoSize = true;
        this.Controls.Add(this._elementHost1);
        // 
        // winform
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(292, 266);
        this.Name = "WinFormWindow";
        this.StartPosition = SWF.FormStartPosition.CenterScreen;
        this.Text = "WinFormWindow";
        this.TopMost = true;
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    void avRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        System.Windows.Documents.TextRange textRange = new System.Windows.Documents.TextRange(
            avRichTextBox.Document.ContentStart, avRichTextBox.Document.ContentEnd);

        _parent.label1.Text = textRange.Text;
    }

    void wfRichTextBox_SelectionChanged(object sender, EventArgs e)
    {
        _parent.label1.Text = wfRichTextBox.Text;
    }
}

#endregion

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Cut, Copy and Paste between EH controls in a single EH.
//@ Cut, Copy and Paste between EH controls in separate EH's.
//@ Cut, Copy and Paste between a EH control in a EH and an Winform RichTextbox using it's clipboard functionality.
//@ Cut, Copy and Paste between a EH control in a EH and an Winform RichTextbox on a different Winform window.
//@ Cut, Copy and Paste between a EH control in a EH and a EH RichTextbox on a different Windows Form.
//@ Cut, Copy and Paste between EH controls in a single EH and Notepad.
// Cut, Copy and Paste Image between EH controls in a single EH.