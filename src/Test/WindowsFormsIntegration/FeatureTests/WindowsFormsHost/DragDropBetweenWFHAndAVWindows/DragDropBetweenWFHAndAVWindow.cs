using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Reflection;

using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWD = System.Windows.Documents;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;
using SWS = System.Windows.Shapes;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;
//
// Testcase:    DragDropBetweenWFHAndAVWindows
// Description: verify that drag and drop works between a WF controls and an AV control
// Author:      pachan
//
namespace WindowsFormsHostTests
{

public class DragDropBetweenWFHAndAVWindows : WPFReflectBase
{

    #region TestVariables

    private delegate void myEventHandler(object sender);
    private int ScenarioIndex = 0;
    private bool debug = false;         // set this true for TC debugging
    private TParams _tp;
    private MitaControl.Edit _edit1;
    private MitaControl.Edit _edit2;
    private SWF.DragDropEffects _ddEffect;

    private SW.Window AVWindow;
    private SWC.TextBox AVTextBox;

    private WindowsFormsHost wfh;
    private SWF.TextBox WFTextBox;


    private const string AVWindowTitleName = "AVWindow";
    private const string AVTextBoxName = "AVTextBox";

    private const string WindowTitleName = "MainWindow";
    private const string WFName = "WFH";
    private const string WFTextBoxName = "WFTextBox";

    #endregion

    #region Testcase setup
    public DragDropBetweenWFHAndAVWindows(string[] args) : base(args) { }


    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        ScenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
        SetupText(ScenarioIndex, p);
        Utilities.SleepDoEvents(10);
        return base.BeforeScenario(p, scenario);
    }

    protected override void InitTest(TParams p) 
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        _tp = p;
        TestSetup();
        this.UseMITA = true;
        base.InitTest(p);
        this.Top = 0;
        this.Left = 0;
        Utilities.SleepDoEvents(10);
    }

    protected override void EndTest(TParams p)
    {
        AVWindow.Close();
        base.EndTest(p);
    }
    #endregion


    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("DragDropCopy Text -- WFH to AV")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        myWriteLine("DragDropCopy Text -- from WF to AV TextBox");
        _ddEffect = SWF.DragDropEffects.Copy;

        GetEditControls(WindowTitleName, WFTextBoxName, AVWindowTitleName, AVTextBoxName);
        DragAndDrop(_edit1, _edit2);
        GetEditControls(WindowTitleName, WFTextBoxName, AVWindowTitleName, AVTextBoxName);

        myWriteLine(String.Format("WFTextBox: {0}; AVTextBox: {1}", _edit1.Value, _edit2.Value));
        WPFMiscUtils.IncCounters(sr, p.log, _edit1.Value.Equals(_edit2.Value), "DragDrop Copy not successful.");
        return sr; 
    }

    [Scenario("DragDropCopy Text -- AV to WFH")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        myWriteLine("DragDropCopy Text -- from AV to WF TextBox");
        _ddEffect = SWF.DragDropEffects.Copy;

        GetEditControls(AVWindowTitleName, AVTextBoxName, WindowTitleName, WFTextBoxName);
        DragAndDrop(_edit1, _edit2);
        GetEditControls(AVWindowTitleName, AVTextBoxName, WindowTitleName, WFTextBoxName);

        myWriteLine(String.Format("AVTextBox: {0}; WFTextBox: {1}", _edit1.Value, _edit2.Value));
        WPFMiscUtils.IncCounters(sr, p.log, _edit1.Value.Equals(_edit2.Value), "DragDrop Copy not successful.");
        return sr;
    }

    [Scenario("DragDropNone Text -- WFH to AV")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        myWriteLine("DragDropNone Text -- from WF to AV TextBox");
        _ddEffect = SWF.DragDropEffects.None;

        Utilities.SleepDoEvents(10);
        GetEditControls(WindowTitleName, WFTextBoxName, AVWindowTitleName, AVTextBoxName);
        string str = _edit1.Value;
        DragAndDrop(_edit1, _edit2);
        GetEditControls(WindowTitleName, WFTextBoxName, AVWindowTitleName, AVTextBoxName);

        myWriteLine(String.Format("WFTextBox: {0}; AVTextBox: {1}", _edit1.Value, _edit2.Value));
        WPFMiscUtils.IncCounters(sr, p.log, _edit1.Value.Equals(str) && _edit2.Value.Equals(""), "DragDrop None not successful.");
        return sr;
    }

    [Scenario("DragDropNone Text -- AV to WFH")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        myWriteLine("DragDropNone Text -- from AV to WF TextBox");
        _ddEffect = SWF.DragDropEffects.None;

        GetEditControls(AVWindowTitleName, AVTextBoxName, WindowTitleName, WFTextBoxName);
        string str = _edit1.Value;
        DragAndDrop(_edit1, _edit2);
        GetEditControls(AVWindowTitleName, AVTextBoxName, WindowTitleName, WFTextBoxName);

        myWriteLine(String.Format("AVTextBox: {0}; WFTextBox: {1}", _edit1.Value, _edit2.Value));
        WPFMiscUtils.IncCounters(sr, p.log, _edit1.Value.Equals(str) && _edit2.Value.Equals(""), "DragDrop None not successful.");
        return sr;
    }

    [Scenario("DragDropMove Text -- WFH to AV")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        myWriteLine("DragDropMove Text -- from WF to AV TextBox");
        _ddEffect = SWF.DragDropEffects.Move;

        GetEditControls(WindowTitleName, WFTextBoxName, AVWindowTitleName, AVTextBoxName);
        string str = _edit1.Value;
        DragAndDrop(_edit1, _edit2);
        GetEditControls(WindowTitleName, WFTextBoxName, AVWindowTitleName, AVTextBoxName);

        myWriteLine(String.Format("WFTextBox: {0}; AVTextBox: {1}", _edit1.Value, _edit2.Value));
        WPFMiscUtils.IncCounters(sr, p.log, _edit1.Value.Equals("") && _edit2.Value.Equals(str), "DragDrop Move not successful.");
        return sr;
    }

    [Scenario("DragDropMove Text -- AV to WFH")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        myWriteLine("DragDropMove Text -- from AV to WF TextBox");
        _ddEffect = SWF.DragDropEffects.Move;

        GetEditControls(AVWindowTitleName, AVTextBoxName, WindowTitleName, WFTextBoxName);
        string str = _edit1.Value;
        DragAndDrop(_edit1, _edit2);
        GetEditControls(AVWindowTitleName, AVTextBoxName, WindowTitleName, WFTextBoxName);
        
        myWriteLine(String.Format("AVTextBox: {0}; WFTextBox: {1}", _edit1.Value, _edit2.Value));
        WPFMiscUtils.IncCounters(sr, p.log, _edit1.Value.Equals("") && _edit2.Value.Equals(str), "DragDrop Move not successful.");
        return sr;
    }


    #endregion

    #region HelperFunction 
 
    private void TestSetup()
    {
        myWriteLine("TestSetup -- Start ");

        #region SetupAVControl
        AVWindow = new SW.Window();
        AVTextBox = new SWC.TextBox();

        AVTextBox.Name = AVTextBoxName;
        AVTextBox.Width = 250;
        AVTextBox.Height = 150;
        AVTextBox.AllowDrop = true;
        AVTextBox.PreviewDrop += new SW.DragEventHandler(AVTextBox_PreviewDrop);
        AVTextBox.PreviewDragEnter += new SW.DragEventHandler(AVTextBox_PreviewDragEnter);
        AVTextBox.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(AVTextBox_PreviewMouseDown);
        #endregion

        #region SetupWFControl
        wfh = new WindowsFormsHost();
        WFTextBox = new SWF.TextBox();
        wfh.Name = WFName;
        WFTextBox.Name = WFTextBoxName;
        WFTextBox.AllowDrop = true;
        WFTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(WFTextBox_MouseDown);
        WFTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(WFTextBox_DragDrop);
        WFTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(WFTextBox_DragEnter);

        wfh.Child = WFTextBox;
        wfh.Width = 250;
        wfh.Height = 150;
        #endregion

        #region LayoutWindow
        AVWindow.Title = AVWindow.Name = AVWindowTitleName;
        AVWindow.Content = AVTextBox;
        AVWindow.SizeToContent = SW.SizeToContent.WidthAndHeight;
        AVWindow.Top = 0;
        AVWindow.Left = 400;
        AVWindow.Show();

        this.Name = this.Title = WindowTitleName;
        this.Content = wfh;
        #endregion

        myWriteLine("TestSetup -- End ");
    }

    void myWriteLine(string s)
    {
        if (debug)
        {
            _tp.log.WriteLine(s);
        }
    }

    #region AVEventHandler
    void AVTextBox_PreviewDrop(object sender, SW.DragEventArgs e)
    {
        myWriteLine("AV TextBox PreviewDragDrop:");
        SWC.TextBox tb = sender as SWC.TextBox;
        if (e.Data.GetDataPresent("Text"))
        {
            myWriteLine("AV DragDropEffect: " + e.Effects.ToString());
            if ((e.Effects & SW.DragDropEffects.Move) == SW.DragDropEffects.Move ||
                (e.Effects & SW.DragDropEffects.Copy) == SW.DragDropEffects.Copy)
            {
                tb.Text = (string)e.Data.GetData("Text");
                myWriteLine("Text Set: " + tb.Text);
            }
        }
        else
            myWriteLine("Text Data Not Present!!!");
        e.Handled = true;
    }

    void AVTextBox_PreviewDragEnter(object sender, SW.DragEventArgs e)
    {
        myWriteLine("AV TextBox PreviewDragEnter:");
        SW.IDataObject data = e.Data;
        e.Effects = (SW.DragDropEffects)_ddEffect;
        myWriteLine("AV DragDropEffect: " + e.Effects.ToString());
        if (e.Data.GetDataPresent("Text"))
            myWriteLine("Text Data Present");
        else
            myWriteLine("Text Data Not Present!!!");
        e.Handled = true;
    }

    void AVTextBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        myWriteLine("AV TextBox MouseDown:");
        SWC.TextBox tb = sender as SWC.TextBox;
        SW.DragDropEffects dde = SW.DragDrop.DoDragDrop(tb, tb.Text, (SW.DragDropEffects)_ddEffect);
        myWriteLine("Started DragDrop with text: " + tb.Text);
        myWriteLine("AV DragDropEffect: " + dde.ToString());
        if ((dde & SW.DragDropEffects.Move) == SW.DragDropEffects.Move)
            tb.Text = "";
        e.Handled = true;
    }
    #endregion

    #region WFEventHandler
    void WFTextBox_DragEnter(object sender, SWF.DragEventArgs e)
    {
        myWriteLine("WF TextBox DragEnter:");
        SWF.IDataObject data = e.Data;
        e.Effect = (SWF.DragDropEffects)_ddEffect;
        myWriteLine("WF DragDropEffect: " + e.Effect.ToString());
        if (e.Data.GetDataPresent("Text"))
            myWriteLine("Text Data Present");
        else
            myWriteLine("Text Data Not Present!!!");
    }

    void WFTextBox_DragDrop(object sender, SWF.DragEventArgs e)
    {
        myWriteLine("WF TextBox PreviewDragDrop:");
        SWF.TextBox tb = sender as SWF.TextBox;
        if (e.Data.GetDataPresent("Text"))
        {
            myWriteLine("WF DragDropEffect: " + e.Effect.ToString());
            if ((e.Effect & SWF.DragDropEffects.Move) == SWF.DragDropEffects.Move ||
                (e.Effect & SWF.DragDropEffects.Copy) == SWF.DragDropEffects.Copy)
            {
                tb.Text = (string)e.Data.GetData("Text");
                myWriteLine("Text Set: " + tb.Text);
            }
        }
        else
            myWriteLine("Text Data Not Present!!!");
    }

    void WFTextBox_MouseDown(object sender, SWF.MouseEventArgs e)
    {
        myWriteLine("WF TextBox MouseDown:");
        SWF.TextBox tb = sender as SWF.TextBox;
        SWF.DragDropEffects dde = tb.DoDragDrop(tb.Text, (SWF.DragDropEffects)_ddEffect);
        myWriteLine("Started DragDrop with text: " + tb.Text);
        myWriteLine("WF DragDropEffect: " + dde.ToString());

        if ((dde & SWF.DragDropEffects.Move) == SWF.DragDropEffects.Move)
            tb.Text = "";
    }
    #endregion

    void GetEditControls(string app1, string ctrl1, string app2, string ctrl2)
    {
        UIObject uiApp1 = UIObject.Root.Children.Find(UICondition.CreateFromId(app1));
        BreadthFirstDescendantsNavigator bfTB1 = new BreadthFirstDescendantsNavigator(uiApp1, UICondition.CreateFromId(ctrl1));
        UIObject uiApp2 = UIObject.Root.Children.Find(UICondition.CreateFromId(app2));
        BreadthFirstDescendantsNavigator bfTB2 = new BreadthFirstDescendantsNavigator(uiApp2, UICondition.CreateFromId(ctrl2));

        _edit1 = new MitaControl.Edit(bfTB1[0]);
        _edit2 = new MitaControl.Edit(bfTB2[0]);
        _edit1.SetFocus();
        //Utilities.SleepDoEvents(15);
    }

    public void DragAndDrop(UIObject dragSource, UIObject dropTarget)
    {
        using (PointerInput.Activate(Mouse.Instance))
        {
            System.Drawing.Rectangle sourceRect = dragSource.BoundingRectangle;
            System.Drawing.Point sourcePoint = new System.Drawing.Point
            (
            sourceRect.X + sourceRect.Height / 2,
            sourceRect.Y + sourceRect.Height / 2
            );

            System.Drawing.Rectangle targetRect = dropTarget.BoundingRectangle;
            System.Drawing.Point targetPoint = new System.Drawing.Point
            (
            targetRect.X + targetRect.Height / 2,
            targetRect.Y + targetRect.Height / 2
            );

            Mouse.Instance.Move(sourcePoint);
            PointerInput.Press(PointerButtons.Primary);
            Utilities.SleepDoEvents(2);

            Mouse.Instance.Move(targetPoint);
            Mouse.Instance.Release(PointerButtons.Primary);
        }
    }

    private void SetupText(int ScenarioIndex, TParams p)
    {
        switch (ScenarioIndex)
        {
            case 1: //@ DragDropCopy Text -- WFH to AV 
            case 3: //@ DragDropNone Text -- WFH to AV 
            case 5: //@ DragDropMove Text -- WFH to AV 
                WFTextBox.Text = p.ru.GetString(10);
                AVTextBox.Text = "";
                break;
            case 2: //@ DragDropCopy Text -- AV to WFH 
            case 4: //@ DragDropNone Text -- AV to WFH
            case 6: //@ DragDropMove Text -- AV to WFH
                AVTextBox.Text = p.ru.GetString(10);
                WFTextBox.Text = ""; 
                break;
            default:
                AVTextBox.Text = p.ru.GetString(10);
                WFTextBox.Text = p.ru.GetString(10);
                break;
        }
    }
  
    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ DragDropCopy Text -- WFH to AV
//@ DragDropCopy Text -- AV to WFH
//@ DragDropNone Text -- WFH to AV
//@ DragDropNone Text -- AV to WFH
//@ DragDropMove Text -- WFH to AV
//@ DragDropMove Text -- AV to WFH
