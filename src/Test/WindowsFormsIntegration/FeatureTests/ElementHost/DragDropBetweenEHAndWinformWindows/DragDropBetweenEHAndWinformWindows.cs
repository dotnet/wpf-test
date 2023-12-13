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

//
// Testcase:    DragDropBetweenelementHost1AndWinformWindows
// Description: Drag and drop between Element Host and WinForm Windows
//
public class DragDropBetweenEHAndWinformWindows : ReflectBase
{
    #region Testcase setup

    WinFormWindows _winform;
    ElementHost _elementHost1;
    SWC.TextBox _avTextBox;
    Edit _edit1;
    Edit _edit2;

    public DragDropBetweenEHAndWinformWindows(string[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "DragDrop";
        this.Size = new System.Drawing.Size(400, 400);
        base.InitTest(p);
    }

    #endregion

    #region Scenarios Setup

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        _winform = new WinFormWindows();
        _winform.Show();

        _winform.wfTextBox.DragDrop += delegate(object o, SWF.DragEventArgs eArgs)
        {
            if (eArgs.Data.GetDataPresent(typeof(string)))
            {
                string s = (String)eArgs.Data.GetData(typeof(String));
                _winform.wfTextBox.Text = s;
            }
        };

        //Create an Avalon TextBox
        _avTextBox = new SWC.TextBox();
        _avTextBox.Name = "avTextBox";
        _avTextBox.Text = "Avalon Text Box";
        _avTextBox.AllowDrop = true;

        //Create Element Host and set Child = avTextBox
        _elementHost1 = new ElementHost();
        _elementHost1.Dock = SWF.DockStyle.Top;
        _elementHost1.Child = _avTextBox;
        _elementHost1.Size = new System.Drawing.Size(60, 25);
        Controls.Add(_elementHost1);

        switch (scenario.Name)
        {
            case "Scenario1":
                SetupScenario1(p);
                break;

            case "Scenario2":
                SetupScenario2(p);
                break;

            case "Scenario3":
                SetupScenario3(p);
                break;

            case "Scenario4":
                SetupScenario4(p);
                break;

            case "Scenario5":
                SetupScenario5(p);
                break;

            case "Scenario6":
                SetupScenario6(p);
                break;
        }
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult sr)
    {
        this.Controls.Clear();
        _avTextBox.Text = "";
        _winform.Close();
        base.AfterScenario(p, scenario, sr);
    }

    private void SetupScenario1(TParams p)
    {
        _winform.wfTextBox.DragEnter += delegate(object sender, SWF.DragEventArgs eArgs)
        {
            if (eArgs.Data.GetDataPresent(typeof(string)))
                eArgs.Effect = SWF.DragDropEffects.Copy;
            else
                eArgs.Effect = SWF.DragDropEffects.None; ;
        };
    }

    private void SetupScenario2(TParams p)
    {
        _winform.wfTextBox.DragEnter += delegate(object sender, SWF.DragEventArgs eArgs)
        {
            if (eArgs.Data.GetDataPresent(typeof(string)))
                eArgs.Effect = SWF.DragDropEffects.Move;
            else
                eArgs.Effect = SWF.DragDropEffects.None; ;
        };
    }

    private void SetupScenario3(TParams p)
    {
        _winform.wfTextBox.DragEnter += delegate(object sender, SWF.DragEventArgs eArgs)
        {
            eArgs.Effect = SWF.DragDropEffects.None;
        };
    }

    private void SetupScenario4(TParams p)
    {
        _avTextBox.Text = "";
        _winform.wfTextBox.Text = "WinForm Text Box";
        _winform.wfTextBox.MouseDown += delegate(object sender, SWF.MouseEventArgs eArgs)
        {
            _winform.wfTextBox.DoDragDrop(_winform.wfTextBox.Text, SWF.DragDropEffects.Copy);
        };
    }

    private void SetupScenario5(TParams p)
    {
        _avTextBox.Text = "";
        _winform.wfTextBox.Text = "WinForm Text Box";
        _winform.wfTextBox.MouseDown += delegate(object sender, SWF.MouseEventArgs eArgs)
        {
            _winform.wfTextBox.DoDragDrop(_winform.wfTextBox.Text, SWF.DragDropEffects.Move);
            _winform.wfTextBox.Text = "";
        };
    }

    private void SetupScenario6(TParams p)
    {
        _avTextBox.Text = "";
        _winform.wfTextBox.Text = "WinForm Text Box";
        _winform.wfTextBox.MouseDown += delegate(object sender, SWF.MouseEventArgs eArgs)
        {
            _winform.wfTextBox.DoDragDrop(_winform.wfTextBox.Text, SWF.DragDropEffects.None);
        };
    }

    #endregion

    #region Scenarios

    [Scenario("Drag drop text from Avalon control to WinForm control " + 
        "in another window - DragDropEffects = Copy.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p)) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        DragAndDrop(_edit1, _edit2); //Do Drag and Drop work around

        MyPause();
        p.log.WriteLine("After Drag Drop : Avalon Text Box   : " + _edit1.Value);
        p.log.WriteLine("After Drag Drop : Winforms Text Box : " + _edit2.Value);

        //After a copy dragdrop, both textboxes should contain "Avalon Text Box"
        sr.IncCounters((_edit1.Value == "Avalon Text Box") && (_edit2.Value == "Avalon Text Box"),
            "Failed at Drag drop text from Avalon control, DragDropEffects = Copy.", p.log);

        return sr;
    }

    [Scenario("Drag drop text from Avalon control to WinForm control " + 
        "in another window - DragDropEffects = Move.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p)) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        DragAndDrop(_edit1, _edit2); //Do Drag and Drop work around

        MyPause();
        p.log.WriteLine("After Drag Drop : Avalon Text Box   : " + _edit1.Value);
        p.log.WriteLine("After Drag Drop : Winforms Text Box : " + _edit2.Value);

        //After a move dragdrop, Avalon Text box should be empty
        sr.IncCounters((_edit1.Value == "") && (_edit2.Value == "Avalon Text Box"),
            "Failed at Drag drop text from Avalon control, DragDropEffects = Move.", p.log);

        return sr;
    }

    [Scenario("Drag drop text from Avalon control to WinForm control " +
        "in another window - DragDropEffects = None.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p)) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        DragAndDrop(_edit1, _edit2); //Do Drag and Drop work around

        MyPause();
        p.log.WriteLine("After Drag Drop : Avalon Text Box   : \"" + _edit1.Value + "\"");
        p.log.WriteLine("After Drag Drop : Winforms Text Box : \"" + _edit2.Value + "\"");

        //With none for dragdrop, WinForms textbox should be empty
        sr.IncCounters((_edit1.Value == "Avalon Text Box") && (_edit2.Value == ""),
            "Failed at Drag drop text from Avalon control, DragDropEffects = None.", p.log);

        return sr;
    }

    [Scenario("Drag drop text from WinForm control to Avalon control " +
        "in another window - DragDropEffects = Copy.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p)) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        DragAndDropFromWinForm(_edit2, _edit1); //Do Drag and Drop work around

        MyPause();
        p.log.WriteLine("After Drag Drop : Avalon Text Box   : \"" + _edit1.Value + "\"");
        p.log.WriteLine("After Drag Drop : Winforms Text Box : \"" + _edit2.Value + "\"");

        //After a copy dragdrop, both textboxes should contain "WinForm Text Box"
        sr.IncCounters((_edit1.Value == "WinForm Text Box") && (_edit2.Value == "WinForm Text Box"),
            "Failed at Drag drop text from WinForm control, DragDropEffects = Copy.", p.log);

        return sr;
    }

    [Scenario("Drag drop text from WinForm control to Avalon control " +
        "in another window - DragDropEffects = Move.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p)) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        DragAndDropFromWinForm(_edit2, _edit1); //Do Drag and Drop work around

        MyPause();
        p.log.WriteLine("After Drag Drop : Avalon Text Box   : \"" + _edit1.Value + "\"");
        p.log.WriteLine("After Drag Drop : Winforms Text Box : \"" + _edit2.Value + "\"");

        //After a move dragdrop, WinForm Text box should be empty
        sr.IncCounters((_edit1.Value == "WinForm Text Box") && (_edit2.Value == ""),
            "Failed at Drag drop text from WinForm control, DragDropEffects = Move.", p.log);

        return sr;
    }

    [Scenario("Drag drop text from WinForms control to Avalon control " +
        "in another window - DragDropEffects = None.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        if (!GetEditControls(p)) //Get Mita wrappers
        {
            return new ScenarioResult(false);
        }
        DragAndDropFromWinForm(_edit2, _edit1); //Do Drag and Drop work around

        MyPause();
        p.log.WriteLine("After Drag Drop : Avalon Text Box   : \"" + _edit1.Value + "\"");
        p.log.WriteLine("After Drag Drop : Winforms Text Box : \"" + _edit2.Value + "\"");

        //With none for dragdrop, Avalon textbox should be empty
        sr.IncCounters((_edit1.Value == "") && (_edit2.Value == "WinForm Text Box"),
            "Failed at Drag drop text from WinForm control, DragDropEffects = None.", p.log);

        return sr;
    }
    
    #endregion

    #region Helper Functions

    void MyPause()
    {
	Utilities.SleepDoEvents(10);
    }

    //Gets Mita wrapper controls from Avalon textbox and WinForm textbox, and passes them to _edit1 and 
    //_edit2 respectively.
    bool GetEditControls(TParams p)
    {
        UIObject uiApp = null;
        UIObject uiavTextBox = null;
        UIObject uiwfTextBox = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("DragDrop"));
            uiavTextBox = uiApp.Descendants.Find(UICondition.CreateFromId("avTextBox"));
            _edit1 = new Edit(uiavTextBox);

            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("WinFormWindows"));
            uiwfTextBox = uiApp.Descendants.Find(UICondition.CreateFromId("wfTextBox"));
            _edit2 = new Edit(uiwfTextBox);

            return true;
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls" + ex.ToString());
            return false;
        }
    }

    //Automates the dragging of text from dragSource and dropping to dropTarget
    //
    public void DragAndDrop(UIObject dragSource, UIObject dropTarget)
    {
        using (PointerInput.Activate(Mouse.Instance))
        {
            dragSource.Click();
            dragSource.SendKeys("{HOME}");
            dragSource.SendKeys("+{END}");

            System.Drawing.Rectangle sourceRect = dragSource.BoundingRectangle;
            System.Drawing.Point sourcePoint = new System.Drawing.Point
            (
                sourceRect.X + sourceRect.Height / 2,
                sourceRect.Y + sourceRect.Height / 2
            );
            Mouse.Instance.Move(sourcePoint);
            PointerInput.Press(PointerButtons.Primary);

            System.Drawing.Rectangle targetRect = dropTarget.BoundingRectangle;
            System.Drawing.Point targetPoint = new System.Drawing.Point
            (
                targetRect.X + targetRect.Height / 2,
                targetRect.Y + targetRect.Height / 2
            );
            Mouse.Instance.Move(targetPoint);
            Mouse.Instance.Release(PointerButtons.Primary);
        }
    }

    //Automates the dragging of text from dragSource and dropping to dropTarget, used for
    //the scenarios where the dragSource is the WinForm control (uses MouseDown).
    public void DragAndDropFromWinForm(UIObject dragSource, UIObject dropTarget)
    {
        using (PointerInput.Activate(Mouse.Instance))
        {
            System.Drawing.Rectangle sourceRect = dragSource.BoundingRectangle;
            System.Drawing.Point sourcePoint = new System.Drawing.Point
            (
                sourceRect.X + sourceRect.Height / 2,
                sourceRect.Y + sourceRect.Height / 2
            );
            Mouse.Instance.Move(sourcePoint);
            PointerInput.Press(PointerButtons.Primary);

            System.Drawing.Rectangle targetRect = dropTarget.BoundingRectangle;
            System.Drawing.Point targetPoint = new System.Drawing.Point
            (
                targetRect.X + targetRect.Height / 2,
                targetRect.Y + targetRect.Height / 2
            );
            Mouse.Instance.Move(targetPoint);
            Mouse.Instance.Release(PointerButtons.Primary);
        }
    }
    
    #endregion
}

#region WinFormWindows

//class WinFormWindow creates a new WinForm window with one TextBox
public class WinFormWindows : SWF.Form
{
    public SWF.TextBox wfTextBox = new SWF.TextBox();

    public WinFormWindows()
    {
        this.SuspendLayout();
        // 
        // wfTextBox
        // 
        this.wfTextBox.Location = new System.Drawing.Point(95, 63);
        this.wfTextBox.Name = "wfTextBox";
        this.wfTextBox.Size = new System.Drawing.Size(100, 20);
        this.wfTextBox.TabIndex = 0;
        this.wfTextBox.AllowDrop = true;
        // 
        // winform
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(292, 266);
        this.Controls.Add(this.wfTextBox);
        this.Name = "WinFormWindows";
        this.StartPosition = SWF.FormStartPosition.CenterScreen;
        this.Text = "WinFormWindows";
        this.TopMost = true;
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}

#endregion

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Drag drop text from Avalon control to WinForm control in another window - DragDropEffects = Copy.
//@ Drag drop text from Avalon control to WinForm control in another window - DragDropEffects = Move.
//@ Drag drop text from Avalon control to WinForm control in another window - DragDropEffects = None.
//@ Drag drop text from WinForm control to Avalon control in another window - DragDropEffects = Copy.
//@ Drag drop text from WinForm control to Avalon control in another window - DragDropEffects = Move.
//@ Drag drop text from WinForms control to Avalon control in another window - DragDropEffects = None.
