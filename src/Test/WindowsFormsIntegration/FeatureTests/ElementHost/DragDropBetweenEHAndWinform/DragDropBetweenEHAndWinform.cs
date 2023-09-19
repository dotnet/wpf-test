// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using SWF=System.Windows.Forms;
using SWC=System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using System.Windows.Controls;
using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Controls;
using System.Reflection;


// Testcase:    DragDropBetweenEHAndWinform
// Description: Drag And Drop Between WinForm and EH
public class DragDropBetweenEHAndWinform : ReflectBase
{
    #region Testcase setup
    public DragDropBetweenEHAndWinform(string[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.UseMita = true;
        this.Text = "DragDrop";
        this.Size = new System.Drawing.Size(100, 100);
        base.InitTest(p);
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================

    #region CLASSVARS
    ElementHost _eh = null;
    System.Windows.Forms.TextBox _wfText = null;
    System.Windows.Controls.TextBox _avText = null;
    //Mita wrappers for TextBoxes
    Edit _edit1 = null;
    Edit _edit2 = null;
    #endregion

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
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

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        this.Controls.Clear();
        _wfText = null ;
        _avText = null;
        _eh = null;
        base.AfterScenario(p, scenario, result);
    }

    #region SCENARIOSETUP
    private void SetupScenario1(TParams p)
    {        
        _avText = new System.Windows.Controls.TextBox();
        _avText.Name = "AVTextBox";
        _avText.Text = "Avalon Text Box";        
        
        //Create a Winforms TextBox
        _wfText = new System.Windows.Forms.TextBox();
        _wfText.Name = "WFTextBox";
        _wfText.Text = "Winforms Text Box";
        _wfText.AllowDrop = true;
        _wfText.Dock = DockStyle.Bottom;
        Controls.Add(_wfText);

        //Creat Element Host 
        _eh = new ElementHost();
        _eh.Dock = DockStyle.Top;
        _eh.Child = _avText;
        _eh.Size = new System.Drawing.Size(60, 25);
        Controls.Add(_eh);

        SetupEventAVToWF(p, System.Windows.DragDropEffects.Copy);
    }

    private  void SetupScenario2(TParams p)
    {        
        _avText = new System.Windows.Controls.TextBox();
        _avText.Name = "AVTextBox";
        _avText.Text = "Avalon Text Box";
        
        //Create a Winforms TextBox
        _wfText = new System.Windows.Forms.TextBox();
        _wfText.Name = "WFTextBox";
        _wfText.Text = "Winforms Text Box";
        _wfText.AllowDrop = true;
        _wfText.Dock = DockStyle.Bottom;
        Controls.Add(_wfText);

        //Creat Element Host 
        _eh = new ElementHost();
        _eh.Dock = DockStyle.Top;
        _eh.Child = _avText;
        _eh.Size = new System.Drawing.Size(60, 25);
        Controls.Add(_eh);

        SetupEventAVToWF(p, System.Windows.DragDropEffects.Move);
    }

    private void SetupScenario3(TParams p)
    {
        _avText = new System.Windows.Controls.TextBox();
        _avText.Name = "AVTextBox";
        _avText.Text = "Avalon Text Box";

        //Create a Winforms TextBox
        _wfText = new System.Windows.Forms.TextBox();
        _wfText.Name = "WFTextBox";
        _wfText.Text = "Winforms Text Box";
        _wfText.AllowDrop = true;
        _wfText.Dock = DockStyle.Bottom;
        Controls.Add(_wfText);

        //Creat Element Host 
        _eh = new ElementHost();
        _eh.Dock = DockStyle.Top;
        _eh.Child = _avText;
        _eh.Size = new System.Drawing.Size(60, 25);
        Controls.Add(_eh);

        SetupEventAVToWF(p, System.Windows.DragDropEffects.None);
    }

    private void SetupScenario4(TParams p)
    {
        //Avalon TextBox 
        _avText = new System.Windows.Controls.TextBox();
        _avText.Name = "AVTextBox";
        _avText.Text = "Avalon Text Box";
        _avText.AllowDrop = true; 

        //Create a Winforms TextBox
        _wfText = new System.Windows.Forms.TextBox();
        _wfText.Name = "WFTextBox";
        _wfText.Text = "Winforms Text Box";
        _wfText.AllowDrop = true;
        _wfText.Dock = DockStyle.Bottom;
        Controls.Add(_wfText);

        //Creat Element Host 
        _eh = new ElementHost();
        _eh.Dock = DockStyle.Top;
        _eh.Child = _avText;
        _eh.Size = new System.Drawing.Size(60, 25);
        Controls.Add(_eh);
        SetupEventWFToAV(p, System.Windows.Forms.DragDropEffects.Copy);
    }

    private void SetupScenario5(TParams p)
    {
        //Avalon TextBox 
        _avText = new System.Windows.Controls.TextBox();
        _avText.Name = "AVTextBox";
        _avText.Text = "Avalon Text Box";
        _avText.AllowDrop = true;

        //Create a Winforms TextBox
        _wfText = new System.Windows.Forms.TextBox();
        _wfText.Name = "WFTextBox";
        _wfText.Text = "Winforms Text Box";
        _wfText.AllowDrop = true;
        _wfText.Dock = DockStyle.Bottom;
        Controls.Add(_wfText);

        //Creat Element Host 
        _eh = new ElementHost();
        _eh.Dock = DockStyle.Top;
        _eh.Child = _avText;
        _eh.Size = new System.Drawing.Size(60, 25);
        Controls.Add(_eh);

        SetupEventWFToAV(p, System.Windows.Forms.DragDropEffects.Move);
    }

    private void SetupScenario6(TParams p)
    {
        _avText = new System.Windows.Controls.TextBox();
        _avText.Name = "AVTextBox";
        _avText.Text = "Avalon Text Box";
        _avText.AllowDrop = true;

        //Create a Winforms TextBox
        _wfText = new System.Windows.Forms.TextBox();
        _wfText.Name = "WFTextBox";
        _wfText.Text = "Winforms Text Box";
        _wfText.AllowDrop = true;
        _wfText.Dock = DockStyle.Bottom;
        Controls.Add(_wfText);

        //Creat Element Host 
        _eh = new ElementHost();
        _eh.Dock = DockStyle.Top;
        _eh.Child = _avText;
        _eh.Size = new System.Drawing.Size(60, 25);
        Controls.Add(_eh);

        SetupEventWFToAV(p, System.Windows.Forms.DragDropEffects.None);
    }
    #endregion

    #region Scenarios
    //WF TextBox is the source and Avalon TextBox the target
    [Scenario("Drag drop text from Avalon control to Winform conrol - DragDropEffects = Copy.")]
    public ScenarioResult Scenario1(TParams p)    
    {
        ScenarioResult sr = new ScenarioResult();
        GetEditControls(p);  //Get Mita wrappers
        DragAndDrop(_edit1, _edit2); //Do Drag and Drop work around

        System.Threading.Thread.Sleep(1000);
        p.log.WriteLine("After Drag Drop : Avalon Text Box   :" + _edit1.Value);
        p.log.WriteLine("After Drag Drop : Winforms Text Box :" + _edit2.Value);

        if ((_edit1.Value == "Avalon Text Box") && (_edit2.Value == "Avalon Text Box"))
        {
            sr.IncCounters(true);
        }
        else
        {
            sr.IncCounters(false, "Drag Drop failed", p.log);
        }
        return sr;
    }

    [Scenario("Drag drop text from Avalon control to Winform conrol - DragDropEffects = Move.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        GetEditControls(p);  //Get Mita wrappers
        DragAndDrop(_edit1, _edit2); //Do Drag and Drop work around
        System.Threading.Thread.Sleep(1000);
        p.log.WriteLine("After Drag Drop : Avalon Text Box   :" + _edit1.Value);
        p.log.WriteLine("After Drag Drop : Winforms Text Box :" + _edit2.Value);

        //After a move drag drop Avlon Text box should be empty
        if ((_edit1.Value == "") && (_edit2.Value == "Avalon Text Box"))
        {
            sr.IncCounters(true);
        }
        else
        {
            sr.IncCounters(false, "Drag Drop failed", p.log);
        }
        return sr;
    }

    [Scenario("Drag drop text from Avalon control to Winform conrol - DragDropEffects = None.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        try
        {
            GetEditControls(p);  //Get Mita wrappers
            DragAndDrop(_edit1, _edit2); //Do Drag and Drop work around

        }
        catch (Exception)
        {            
            throw;
        }

        System.Threading.Thread.Sleep(1000);
        p.log.WriteLine("After Drag Drop : Avalon Text Box   :" + _edit1.Value);
        p.log.WriteLine("After Drag Drop : Winforms Text Box :" + _edit2.Value);

        if ((_edit1.Value == "Avalon Text Box") && (_edit2.Value == "Winforms Text Box"))
        {
            sr.IncCounters(true);
        }
        else
        {
            sr.IncCounters(false, "Drag Drop failed", p.log);
        }
        return sr;
    }

    //Avalon Text is the source and WF TextBox the target
    [Scenario("Drag drop text from Winform conrol to Avalon Control on EH - DragDropEffects = Copy.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        GetEditControls(p);  //Get Mita wrappers
        DragAndDrop(_edit2, _edit1); //Do Drag and Drop work around

        System.Threading.Thread.Sleep(1000);
        p.log.WriteLine("After Drag Drop : Avalon Text Box   :" + _edit1.Value);
        p.log.WriteLine("After Drag Drop : Winforms Text Box :" + _edit2.Value);

        if ((_edit1.Value == "Winforms Text Box") && (_edit2.Value == "Winforms Text Box"))
        {
            sr.IncCounters(true);
        }
        else
        {
            sr.IncCounters(false, "Drag Drop failed", p.log);
        }
        return sr;
    }
    
    [Scenario("Drag drop text from Winform conrol to Avalon Control on EH - DragDropEffects = Move.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        GetEditControls(p);  //Get Mita wrappers
        DragAndDrop(_edit2, _edit1); //Do Drag and Drop work around

        System.Threading.Thread.Sleep(1000);
        p.log.WriteLine("After Drag Drop : Avalon Text Box   :" + _edit1.Value);
        p.log.WriteLine("After Drag Drop : Winforms Text Box :" + _edit2.Value);

        //After a move drag drop Winforms Text box should be empty
        if ((_edit1.Value == "Winforms Text Box") && (_edit2.Value == ""))
        {
            sr.IncCounters(true);
        }
        else
        {
            sr.IncCounters(false, "Drag Drop failed", p.log);
        }
        return sr;
    }

    [Scenario("Drag drop text from Winform conrol to Avalon Control on EH - DragDropEffects = None.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        try
        {
            GetEditControls(p);  //Get Mita wrappers
            DragAndDrop(_edit2, _edit1); //Do Drag and Drop work around

        }
        catch (Exception)
        {
            throw;
        }

        System.Threading.Thread.Sleep(1000);
        p.log.WriteLine("After Drag Drop : Avalon Text Box   :" + _edit1.Value);
        p.log.WriteLine("After Drag Drop : Winforms Text Box :" + _edit2.Value);

        //nothing should be changed after a drag drop none 
        if ((_edit1.Value == "Avalon Text Box") && (_edit2.Value == "Winforms Text Box"))
        {
            sr.IncCounters(true);
        }
        else
        {
            sr.IncCounters(false, "Drag Drop failed", p.log);
        }
        return sr;
    }
    #endregion

    #region Helper Functions

    void GetEditControls(TParams p)
    {
        UIObject uiApp = null;
        UIObject uiAvTextBox = null;
        UIObject uiWfTextBox = null;
        try        
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("DragDrop"));
            uiAvTextBox = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox"));
            uiWfTextBox  = uiApp.Descendants.Find(UICondition.CreateFromId("WFTextBox"));
            _edit1 = new Edit(uiAvTextBox);
            _edit2 = new Edit(uiWfTextBox);
        }
        catch (Exception ex )
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls" + ex.ToString());
        }
    }

    public void DragAndDrop(UIObject dragSource, UIObject dropTarget)
    {
        using (PointerInput.Activate(Mouse.Instance))
        {
            //dragSource.Click();
            //dragSource.SendKeys("{HOME}");
            //dragSource.SendKeys("+{END}");
            
            System.Drawing.Rectangle sourceRect = dragSource.BoundingRectangle;
            System.Drawing.Rectangle targetRect = dropTarget.BoundingRectangle;

            System.Drawing.Point sourcePoint = new System.Drawing.Point
            (
                sourceRect.X + sourceRect.Height / 2,
                sourceRect.Y + sourceRect.Height / 2
            );
            System.Drawing.Point targetPoint = new System.Drawing.Point
            (
                targetRect.X + targetRect.Height / 2,
                targetRect.Y + targetRect.Height / 2
            );
            Utilities.SleepDoEvents(2);

            Mouse.Instance.Move(sourcePoint);
            PointerInput.Press(PointerButtons.Primary);
            Utilities.SleepDoEvents(2);

            Mouse.Instance.Move(targetPoint);
            Mouse.Instance.Release(PointerButtons.Primary);
            Utilities.SleepDoEvents(2);
        }
    }

    void SetupEventWFToAV(TParams p, System.Windows.Forms.DragDropEffects _ddEffect)
    {
        _avText.PreviewDragEnter += delegate(object o, System.Windows.DragEventArgs eArgs)
        {
            p.log.WriteLine("Got Drag Enter: " + _avText);
            if (eArgs.Data.GetDataPresent(typeof(string)))
                eArgs.Effects = (System.Windows.DragDropEffects)_ddEffect;
            else
                eArgs.Effects = System.Windows.DragDropEffects.None;
            eArgs.Handled = true;
        };

        _avText.PreviewDrop += delegate(object o, System.Windows.DragEventArgs eArgs)
        {
            p.log.WriteLine("Got Drop event " + _avText);
            System.Windows.Controls.TextBox txtBox = o as System.Windows.Controls.TextBox;
            if (eArgs.Data.GetDataPresent(typeof(string)))
            {
                string s = (String)eArgs.Data.GetData(typeof(String));
                _avText.Text = s;
            }
            eArgs.Handled = true;
        };
        _wfText.MouseDown += delegate(object o, System.Windows.Forms.MouseEventArgs eArgs)
        {
            p.log.WriteLine("Got Mouse Down");
            System.Windows.Forms.TextBox txtBox = o as System.Windows.Forms.TextBox;
            System.Windows.Forms.DragDropEffects dde = txtBox.DoDragDrop(txtBox.Text, _ddEffect);
            if ((dde & System.Windows.Forms.DragDropEffects.Move) == System.Windows.Forms.DragDropEffects.Move)
                txtBox.Text = "";
        };
    }

    void SetupEventAVToWF(TParams p, System.Windows.DragDropEffects _ddEffect)
    {
        _wfText.DragEnter += delegate(object o, System.Windows.Forms.DragEventArgs eArgs)
        {
            p.log.WriteLine("Got Drag Enter: " + _wfText);
            if (eArgs.Data.GetDataPresent(typeof(string)))
                eArgs.Effect = (System.Windows.Forms.DragDropEffects)_ddEffect;
            else
                eArgs.Effect = System.Windows.Forms.DragDropEffects.None;
        };

        _wfText.DragDrop += delegate(object o, System.Windows.Forms.DragEventArgs eArgs)
        {
            p.log.WriteLine("Got Drop event " + _wfText);
            System.Windows.Controls.TextBox txtBox = o as System.Windows.Controls.TextBox;
            if (eArgs.Data.GetDataPresent(typeof(string)))
            {
                string s = (String)eArgs.Data.GetData(typeof(String));
                _wfText.Text = s;
            }
        };
        _avText.PreviewMouseDown += delegate(object o, System.Windows.Input.MouseButtonEventArgs eArgs)
        {
            p.log.WriteLine("Got Mouse Down");
            System.Windows.Controls.TextBox txtBox = o as System.Windows.Controls.TextBox;
            System.Windows.DragDropEffects dde = System.Windows.DragDrop.DoDragDrop(txtBox, txtBox.Text, _ddEffect);
            if ((dde & System.Windows.DragDropEffects.Move) == System.Windows.DragDropEffects.Move)
                txtBox.Text = "";
//            eArgs.Handled = true;
        };
    }

    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Drag drop text from Avalon control to Winform conrol - DragDropEffects = Copy.
//@ Drag drop text from Avalon control to Winform conrol - DragDropEffects = Move.
//@ Drag drop text from Avalon control to Winform conrol - DragDropEffects = None.
//@ Drag drop text from Winform conrol to Avalon Control on EH - DragDropEffects = Copy.
//@ Drag drop text from Winform conrol to Avalon Control on EH - DragDropEffects = Move.
//@ Drag drop text from Winform conrol to Avalon Control on EH - DragDropEffects = None.
