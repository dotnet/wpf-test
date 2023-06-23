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


//
// Testcase:    DragDropBetween2EH
// Description: Drag And Drop Between two EHs
// Author:      pachan
//
//
public class DragDropBetween2EH : ReflectBase
{

    private System.Windows.DragDropEffects _ddEffect;

    #region Testcase setup
    public DragDropBetween2EH(string[] args) : base(args) { }


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
    ElementHost eh1 = null;
    ElementHost eh2 = null;
    System.Windows.Controls.Button avButton1 = null;
    System.Windows.Controls.TextBox avText2 = null;
    
    //Mita wrappers for TextBoxes
    MS.Internal.Mita.Foundation.Controls.Button _button1 = null;
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
        }
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        this.Controls.Clear();
        avButton1 = null;
        avText2 = null;
        eh1 = null;
        eh2 = null;
        base.AfterScenario(p, scenario, result);
    }


    #region SCENARIOSETUP
    void CommonSetup(TParams p)
    {
        avButton1 = new System.Windows.Controls.Button();
        avButton1.Name = "AVButton1";
        avButton1.Content = "Avalon Button 1";
        avButton1.AllowDrop = true;

        avText2 = new System.Windows.Controls.TextBox();
        avText2.Name = "AVTextBox2";
        avText2.Text = "Avalon Text Box 2";
        avText2.AllowDrop = true;

        //Creat Element Host1
        eh1 = new ElementHost();
        eh1.Dock = DockStyle.Top;
        eh1.Child = avButton1;
        eh1.Size = new System.Drawing.Size(60, 25);
        Controls.Add(eh1);


        //Creat Element Host 2 
        eh2 = new ElementHost();
        eh2.Dock = DockStyle.Bottom;
        eh2.Child = avText2;
        eh2.Size = new System.Drawing.Size(60, 25);
        Controls.Add(eh2);

        avButton1.PreviewDragEnter += delegate(object o, System.Windows.DragEventArgs eArgs)
        {
            p.log.WriteLine("Got Drag Enter: " + avButton1);

            if (eArgs.Data.GetDataPresent(typeof(string)))
            {
                string s = (String)eArgs.Data.GetData(typeof(String));
                eArgs.Effects = _ddEffect;
            }
            eArgs.Handled = true;
        };

        avButton1.PreviewDrop += delegate(object o, System.Windows.DragEventArgs eArgs)
        {
            p.log.WriteLine("Got Drop event " + avButton1);
            System.Windows.Controls.TextBox txtBox = o as System.Windows.Controls.TextBox;
            if (eArgs.Data.GetDataPresent(typeof(string)) &&
                ((eArgs.Effects & System.Windows.DragDropEffects.Move) == System.Windows.DragDropEffects.Move ||
                    (eArgs.Effects & System.Windows.DragDropEffects.Copy) == System.Windows.DragDropEffects.Copy))
            {
                string s = (String)eArgs.Data.GetData(typeof(String));
                avButton1.Content = s;
            }
            eArgs.Handled = true;
        };

        avText2.PreviewMouseDown += delegate(object o, System.Windows.Input.MouseButtonEventArgs eArgs)
        {
            p.log.WriteLine("Got MouseDown " + avText2);
            System.Windows.Controls.TextBox tb = o as System.Windows.Controls.TextBox;
            if (tb.SelectedText.Equals(tb.Text) == true)
            {
                System.Windows.DragDropEffects dde = System.Windows.DragDrop.DoDragDrop(tb, tb.Text, _ddEffect);
                if ((dde & System.Windows.DragDropEffects.Move) == System.Windows.DragDropEffects.Move)
                    tb.Text = "";
                eArgs.Handled = true;
            }
        };
    }
    
    private void SetupScenario1(TParams p)
    {
        CommonSetup(p);
        avText2.Text = "Scenario1";
        _ddEffect = System.Windows.DragDropEffects.Copy;
    }

    private void SetupScenario2(TParams p)
    {
        CommonSetup(p);
        avText2.Text = "Scenario2";
        _ddEffect = System.Windows.DragDropEffects.Move;
    }

    private void SetupScenario3(TParams p)
    {
        CommonSetup(p);
        avText2.Text  = "Scenario3";
        _ddEffect = System.Windows.DragDropEffects.None;
    } 
    #endregion

    #region Scenarios
    [Scenario("Drag drop text from Avalon control to another Avalon conrol. Controls are on diff EH.  DragDropEffects = Copy.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        GetEditControls(p);  //Get Mita wrappers
        DragAndDrop(_edit2, _button1); //Do Drag and Drop work around
        p.log.WriteLine();
        p.log.WriteLine("After Drag Drop : Button1  =" + _button1.Name);
        p.log.WriteLine("After Drag Drop : TextBox2 =" + _edit2.Value);

        if ((_button1.Name == "Scenario1") && (_edit2.Value == "Scenario1"))
            sr.IncCounters(true);
        else    
            sr.IncCounters(false, "Drag Drop failed.", p.log);
        return sr;
    }

    [Scenario("Drag drop text from Avalon control to another Avalon conrol. Controls are on diff EH.  DragDropEffects = Move.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        GetEditControls(p);  //Get Mita wrappers
        DragAndDrop(_edit2, _button1); //Do Drag and Drop work around
        p.log.WriteLine();
        p.log.WriteLine("After Drag Drop : Button1  =" + _button1.Name);
        p.log.WriteLine("After Drag Drop : TextBox2 =" + _edit2.Value);

        if ((_button1.Name == "Scenario2") && (_edit2.Value == String.Empty))
            sr.IncCounters(true);
        else
            sr.IncCounters(false, "Drag Drop failed.", p.log);
         return sr;
    }

    [Scenario("Drag drop text from Avalon control to another Avalon conrol. Controls are on diff EH.  DragDropEffects = None.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        GetEditControls(p);  //Get Mita wrappers
        DragAndDrop(_edit2, _button1); //Do Drag and Drop work around
        p.log.WriteLine();
        p.log.WriteLine("After Drag Drop : Button1  =" + _button1.Name);
        p.log.WriteLine("After Drag Drop : TextBox2 =" + _edit2.Value);
        
        //After a move drag drop Avlon Text box should be empty
        if ((_button1.Name== "Avalon Button 1") && (_edit2.Value == "Scenario3"))
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
        UIObject uiAvButton1 = null;
        UIObject uiAvTextBox2 = null;
        try
        {
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("DragDrop"));
            uiAvButton1 = uiApp.Descendants.Find(UICondition.CreateFromId("AVButton1"));
            uiAvTextBox2 = uiApp.Descendants.Find(UICondition.CreateFromId("AVTextBox2"));
            _button1  = new MS.Internal.Mita.Foundation.Controls.Button(uiAvButton1);
            _edit2 = new Edit(uiAvTextBox2);
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to Get Mita wrapper controls" + ex.ToString());
        }
    }

    public void DragAndDrop(UIObject dragSource, UIObject dropTarget)
    {
        using (PointerInput.Activate(Mouse.Instance))
        {
            dragSource.Click();
            //dragSource.SendKeys("{HOME}");
            //dragSource.SendKeys("+{END}");

            /*System.Drawing.Rectangle sourceRect = dragSource.BoundingRectangle;
            System.Drawing.Point sourcePoint = new System.Drawing.Point
            (
                sourceRect.X + sourceRect.Height / 2,
                sourceRect.Y + sourceRect.Height / 2
            );
            Utilities.SleepDoEvents(2);
            Mouse.Instance.Move(sourcePoint);*/

	    dragSource.DoubleClick();
            Mouse.Instance.Press(PointerButtons.Primary);
            Utilities.SleepDoEvents(2);

            System.Drawing.Rectangle targetRect = dropTarget.BoundingRectangle;
            System.Drawing.Point targetPoint = new System.Drawing.Point
            (
                targetRect.X + targetRect.Height / 2,
                targetRect.Y + targetRect.Height / 2
            );


            Mouse.Instance.Move(targetPoint);
            Mouse.Instance.Release(PointerButtons.Primary);
            Utilities.SleepDoEvents(2);
        }
    }
 #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Drag drop text from Avalon control to another Avalon conrol. Controls are on diff EH.  DragDropEffects = Copy.
//@ Drag drop text from Avalon control to another Avalon conrol. Controls are on diff EH.  DragDropEffects = Move.
//@ Drag drop text from Avalon control to another Avalon conrol. Controls are on diff EH.  DragDropEffects = None.
