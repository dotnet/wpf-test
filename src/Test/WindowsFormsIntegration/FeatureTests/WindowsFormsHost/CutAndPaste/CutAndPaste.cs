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
// Testcase:    CutAndPaste
// Description: Verify that clipboard functionality works
// Author:      pachan
//
namespace WindowsFormsHostTests
{

public class CutAndPaste : WPFReflectBase
{
    #region TestVariables

    private delegate void myEventHandler(object sender);
    private static MethodInfo[] mi;
    private int ScenarioIndex = 0;
    private UIObject uiApp;
    private bool debug = false;         // set this true for TC debugging
    private System.Drawing.Bitmap bmp1;
    private System.Drawing.Bitmap bmp2;

    private SWC.Grid AVGrid;
    private SWC.RichTextBox AV1RichTextBox;
    private SWC.RichTextBox AV2RichTextBox;
    private SW.Window AV2Window;

    private WindowsFormsHost wfh1;
    private WindowsFormsHost wfh2;
    private SWF.FlowLayoutPanel WF1FlowLayoutPanel;
    private SWF.FlowLayoutPanel WF2FlowLayoutPanel;
    private SWF.TextBox WF1TextBox1;
    private SWF.TextBox WF1TextBox2;
    private SWF.RichTextBox WF1RichTextBox;
    private SWF.TextBox WF2TextBox1;
    private SWF.TextBox WF2TextBox2;
    private SWF.RichTextBox WF2RichTextBox;

    private SWF.Form form;
    private SWF.FlowLayoutPanel FRMFlowLayoutPanel;
    private SWF.TextBox FRMTextBox1;
    private SWF.TextBox FRMTextBox2;
    private SWF.RichTextBox FRMRichTextBox;

    private const string WindowTitleName = "CutAndPasteTest";
    private const string AV2WindowName = "AV2Window";

    private const string AVGridName = "AVGrid";
    private const string AV1RichTextBoxName = "AV1RichTextBox";
    private const string AV2RichTextBoxName = "AV2RichTextBox";

    private const string WF1Name = "WF1";
    private const string WF2Name = "WF2";
    private const string WF1FlowLayoutPanelName = "WF1FlowLayoutPanel";
    private const string WF2FlowLayoutPanelName = "WF2FlowLayoutPanel";
    private const string WF1TextBox1Name = "WF1TextBox1";
    private const string WF1TextBox2Name = "WF1TextBox2";
    private const string WF1RichTextBoxName = "WF1RichTextBox";
    private const string WF2TextBox1Name = "WF2TextBox1";
    private const string WF2TextBox2Name = "WF2TextBox2";
    private const string WF2RichTextBoxName = "WF2RichTextBox";

    private const string FRMName = "WinFormWindow";
    private const string FRMTextBox1Name = "FRMTextBox1";
    private const string FRMTextBox2Name = "FRMTextBox2";
    private const string FRMRichTextBoxName = "FRMRichTextBox";
    private const string FRMFlowLayoutPanelName = "FRMFlowLayoutPanel";
    #endregion

    #region Testcase setup
    public CutAndPaste(string[] args) : base(args) { }


    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        ScenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
        HideShowGroup(ScenarioIndex);
        // move the mouse point to the corner
//        Mouse.Instance.Move(new System.Drawing.Point(0, 0));
        Utilities.SleepDoEvents(2);
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        if (ScenarioIndex == 4)
            AV2Window.Close();
        else if (ScenarioIndex == 5)
            form.Close();
        base.AfterScenario(p, scenario, result);
    }

    protected override void InitTest(TParams p) 
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        mi = GetAllScenarios(this);
        this.UseMITA = true;
        TestSetup(p);
        base.InitTest(p);
        this.Top = 0;
        this.Left = 0;
    }
    #endregion
    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios

    [Scenario("Cut, Copy and Paste between WF controls in a single WFH")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl1;
        UIObject ctrl2;

        p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
        ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
        ctrl2 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox2Name));
        TestCutPaste(sr, p, ctrl1, ctrl2);

        p.log.WriteLine("--- Cut, Copy and Paste between TextBox and RichTextBox ---");
        ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
        ctrl2 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1RichTextBoxName));
        TestCutPaste(sr, p, ctrl1, ctrl2);
        Utilities.SleepDoEvents(2);

        return sr;
    }

    [Scenario("Cut, Copy and Paste between WF controls in seperate WFH's")]
    public ScenarioResult Scenario2(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl1;
        UIObject ctrl2;

        p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
        ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
        ctrl2 = uiApp.Descendants.Find(UICondition.CreateFromId(WF2TextBox2Name));
        TestCutPaste(sr, p, ctrl1, ctrl2);

        p.log.WriteLine("--- Cut, Copy and Paste between TextBox and RichTextBox ---");
        ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
        ctrl2 = uiApp.Descendants.Find(UICondition.CreateFromId(WF2RichTextBoxName));
        TestCutPaste(sr, p, ctrl1, ctrl2);

        return sr;
    }

    [Scenario("Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox using it's clipboard functionality")]
    public ScenarioResult Scenario3(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl1;
        UIObject ctrl2;

        p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
        ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
        ctrl2 = uiApp.Descendants.Find(UICondition.CreateFromId(AV1RichTextBoxName));
        TestCutPaste(sr, p, ctrl1, ctrl2);

        return sr;
    }

    [Scenario("Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox on a different AV window")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject uiAVWindow = UIObject.Root.Children.Find(UICondition.CreateFromName(AV2WindowName));
        UIObject ctrl1;
        UIObject ctrl2;

        p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
        ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
        ctrl2 = uiAVWindow.Descendants.Find(UICondition.CreateFromId(AV2RichTextBoxName));
        TestCutPaste(sr, p, ctrl1, ctrl2);

        return sr;
    }

    [Scenario("Cut, Copy and Paste between a WF control in a WFH and a WF RichTextbox on a different Windows Form")]
    public ScenarioResult Scenario5(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject uiWFWindow = UIObject.Root.Children.Find(UICondition.CreateFromId(FRMName));
        UIObject ctrl1;
        UIObject ctrl2;

        p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
        ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
        ctrl2 = uiWFWindow.Descendants.Find(UICondition.CreateFromId(FRMTextBox1Name));
        TestCutPaste(sr, p, ctrl1, ctrl2);

        p.log.WriteLine("--- Cut, Copy and Paste between TextBox and RichTextBox ---");
        ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
        ctrl2 = uiWFWindow.Descendants.Find(UICondition.CreateFromId(FRMRichTextBoxName));
        TestCutPaste(sr, p, ctrl1, ctrl2);

        return sr;
    }

    [Scenario("Cut, Copy and Paste between WF controls in a single WFH and Notepad")]
    public ScenarioResult Scenario6(TParams p)
    {
        System.Diagnostics.Process myProcess = System.Diagnostics.Process.Start(Environment.SystemDirectory + @"\notepad.exe");
        Utilities.SleepDoEvents(30);
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject uiNotepad = UIObject.Root.Children.Find(UICondition.CreateFromClassName("Notepad"));
        UIObject ctrl1;
        UIObject ctrl2;

        p.log.WriteLine("--- Cut, Copy and Paste - from WF TextBox to Notepad ---");
        ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
        ctrl2 = uiNotepad.Descendants.Find(UICondition.CreateFromClassName("Edit"));
        TestCutPaste(sr, p, ctrl1, ctrl2);

        p.log.WriteLine("--- Cut, Copy and Paste - from Notepad to WF TextBox ---");
        TestCutPaste(sr, p, ctrl2, ctrl1);

        myProcess.Kill();
        return sr;
    }

    [Scenario("Cut, Copy and Paste Image between WF controls in a single WFH ")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl1 = uiApp.Descendants.Find(UICondition.CreateFromId(AV1RichTextBoxName));
        UIObject ctrl2 = uiApp.Descendants.Find(UICondition.CreateFromId(WF1RichTextBoxName));
        UIObject ctrl3 = uiApp.Descendants.Find(UICondition.CreateFromId(WF2RichTextBoxName));

        System.Drawing.Bitmap bmpOriginal = Utilities.GetBitmapOfControl(WF1RichTextBox, true);
        System.Drawing.Bitmap bmpSource;
        System.Drawing.Bitmap bmpTarget;

        //p.log.WriteLine("Copy and Paste from AV RichTextBox into WF RichTextBox");
        //TestCutPasteImage(sr, p, ctrl1, ctrl3, false);
        //p.log.WriteLine("Cut and Paste from AV RichTextBox into WF RichTextBox");
        //TestCutPasteImage(sr, p, ctrl1, ctrl3, true);

        //p.log.WriteLine("Copy and Paste from WF RichTextBox into AV RichTextBox");
        //TestCutPasteImage(sr, p, ctrl2, ctrl1, false);
        //AV1RichTextBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new myEventHandler(GetBitmap), AV1RichTextBox);
        //WF1RichTextBox.Invoke(new myEventHandler(GetWFBitmap), WF1RichTextBox);

        p.log.WriteLine("Copy and Paste from WF RichTextBox into another WF RichTextBox");
        TestCutPasteImage(sr, p, ctrl2, ctrl3, false);
        ctrl1.SetFocus();
        bmpSource = Utilities.GetBitmapOfControl(WF1RichTextBox, true);
        bmpTarget = Utilities.GetBitmapOfControl(WF2RichTextBox, true);
        if (debug)
        {
            bmpSource.Save("_bmpSource.bmp");
            bmpTarget.Save("_bmpTarget.bmp");
        }
        WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(bmpSource, bmpTarget), "bitmap should match");

        p.log.WriteLine("Cut and Paste from WF RichTextBox into another WF RichTextBox");
        TestCutPasteImage(sr, p, ctrl2, ctrl3, true);
        ctrl1.SetFocus();
        bmpTarget = Utilities.GetBitmapOfControl(WF2RichTextBox, true);
        WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(bmpOriginal, bmpTarget), "bitmap should match");
        WPFMiscUtils.IncCounters(sr, String.Empty, GetTextboxText(ctrl2), "Source text is not being cut", p.log);

        //KeepRunningTests = false;
        return sr;

    }

    #endregion

    #region TestFunction
    private void TestCutPaste(ScenarioResult sr, TParams p, UIObject fromObj, UIObject toObj)
    {
        string expVal = String.Empty;
        string CompareString = String.Empty;
        string fromObjectName = fromObj.AutomationId;
        string toObjectName = toObj.AutomationId;

        // make sure we have some text to test with
        if (fromObjectName.Length < 3)
            fromObjectName = "FromObject";
        if (toObjectName.Length < 3)
            toObjectName = "ToObject";

        p.log.WriteLine("Cut and Paste");
        //reset text
        toObj.SendKeys("^{HOME}");
        toObj.SendKeys("^+{END}");
        toObj.SendKeys(toObjectName);
        fromObj.SendKeys("^{HOME}");
        fromObj.SendKeys("^+{END}");
        fromObj.SendKeys(fromObjectName);

        fromObj.SendKeys("^{HOME}");
        fromObj.SendKeys("^+{END}");
        fromObj.SendKeys("^x");     // cut
        toObj.SendKeys("{END}");
        toObj.SendKeys("^v");     // paste
        Utilities.SleepDoEvents(2);

        expVal = toObjectName + fromObjectName;
        CompareString = GetTextboxText(toObj);
        WPFMiscUtils.IncCounters(sr, expVal, CompareString, "Pasted text not as expected", p.log);
        CompareString = GetTextboxText(fromObj);
        WPFMiscUtils.IncCounters(sr, String.Empty, CompareString, "Source text is not being cut", p.log);

        p.log.WriteLine("Copy and Paste");
        //reset text
        toObj.SendKeys("^{HOME}");
        toObj.SendKeys("^+{END}");
        toObj.SendKeys(toObjectName);
        fromObj.SendKeys("^{HOME}");
        fromObj.SendKeys("^+{END}");
        fromObj.SendKeys(fromObjectName);
        Utilities.SleepDoEvents(2);

        fromObj.SendKeys("^{HOME}");
        fromObj.SendKeys("^+{END}");
        fromObj.SendKeys("^c");     // copy
        toObj.SendKeys("{END}");
        toObj.SendKeys("^v");     // paste
        Utilities.SleepDoEvents(2);

        expVal = toObjectName + fromObjectName;
        CompareString = GetTextboxText(toObj);
        WPFMiscUtils.IncCounters(sr, expVal, CompareString, "Pasted text not as expected", p.log);
        CompareString = GetTextboxText(fromObj);
        WPFMiscUtils.IncCounters(sr, fromObjectName, CompareString, "Source text is changed", p.log);

        p.log.WriteLine("Do the Paste one more time");
        toObj.SendKeys("{END}");
        toObj.SendKeys("^v");     // paste
        Utilities.SleepDoEvents(2);

        expVal += fromObjectName;
        CompareString = GetTextboxText(toObj);
        WPFMiscUtils.IncCounters(sr, expVal, CompareString, "Pasted text not as expected", p.log);

        p.log.WriteLine("Do copy twice and then do the paste");
        //reset text
        fromObj.SendKeys("^{HOME}");
        fromObj.SendKeys("^+{END}");
        fromObj.SendKeys(fromObjectName);
        toObj.SendKeys("^{HOME}");
        toObj.SendKeys("^+{END}");
        toObj.SendKeys(toObjectName);
        Utilities.SleepDoEvents(2);

        fromObj.SendKeys("^{HOME}");
        fromObj.SendKeys("^+{END}");
        fromObj.SendKeys("^c");     // copy
        fromObj.SendKeys("^c");     // copy
        toObj.SendKeys("{END}");
        toObj.SendKeys("^v");     // paste
        Utilities.SleepDoEvents(2);

        expVal = toObjectName + fromObjectName;
        CompareString = GetTextboxText(toObj);
        WPFMiscUtils.IncCounters(sr, expVal, CompareString, "Pasted text not as expected", p.log);
        CompareString = GetTextboxText(fromObj);
        WPFMiscUtils.IncCounters(sr, fromObjectName, CompareString, "Source text is changed", p.log);

        p.log.WriteLine("Copy and Paste on Selected text");
        //reset text
        fromObj.SendKeys("^{HOME}");
        fromObj.SendKeys("^+{END}");
        fromObj.SendKeys(fromObjectName);
        toObj.SendKeys("^{HOME}");
        toObj.SendKeys("^+{END}");
        toObj.SendKeys(toObjectName);
        Utilities.SleepDoEvents(2);

        fromObj.SendKeys("^{HOME}");
        fromObj.SendKeys("+{RIGHT}");
        fromObj.SendKeys("+{RIGHT}");
        fromObj.SendKeys("+{RIGHT}");
        fromObj.SendKeys("^c");     // copy
        toObj.SendKeys("{END}");
        toObj.SendKeys("^v");     // paste
        Utilities.SleepDoEvents(2);

        expVal = toObjectName + fromObjectName.Substring(0,3);
        CompareString = GetTextboxText(toObj);
        WPFMiscUtils.IncCounters(sr, expVal, CompareString, "Pasted text not as expected", p.log);
        CompareString = GetTextboxText(fromObj);
        WPFMiscUtils.IncCounters(sr, fromObjectName, CompareString, "Source text is changed", p.log);
    }

    private void TestCutPasteImage(ScenarioResult sr, TParams p, UIObject ctrl1, UIObject ctrl2, bool bCut)
    {
        if (!bCut)
        {
            p.log.WriteLine("Copy and Paste");
            //reset Destination 
            ctrl2.SendKeys("^{HOME}");
            ctrl2.SendKeys("^+{END}");
            ctrl2.SendKeys("{DEL}");
            Utilities.SleepDoEvents(2);

            ctrl1.SendKeys("^{HOME}");
            ctrl1.SendKeys("^+{END}");
            ctrl1.SendKeys("^c");     // copy 
            ctrl2.SendKeys("{END}");
            ctrl2.SendKeys("^v");     // paste
            Utilities.SleepDoEvents(2);
        }
        else
        {
            p.log.WriteLine("Cut and Paste");
            //reset Destination
            ctrl2.SendKeys("^{HOME}");
            ctrl2.SendKeys("^+{END}");
            ctrl2.SendKeys("{DEL}");
            Utilities.SleepDoEvents(2);

            ctrl1.SendKeys("^{HOME}");
            ctrl1.SendKeys("^+{END}");
            ctrl1.SendKeys("^x");     // cut 
            ctrl2.SendKeys("{END}");
            ctrl2.SendKeys("^v");     // paste 
            Utilities.SleepDoEvents(2);
        }
    }

    private void TestSetup(TParams p)
    {
        p.log.WriteLine("TestSetup -- Start ");

        #region SetupAVControl

        AVGrid = new SWC.Grid();
        AV1RichTextBox = new SWC.RichTextBox();

        AV2Window = new SW.Window();
        AV2RichTextBox = new SWC.RichTextBox();

        AVGrid.Name = AVGridName;
        AVGrid.ShowGridLines = true;
        AVGrid.ColumnDefinitions.Add(new SWC.ColumnDefinition());
        AVGrid.ColumnDefinitions.Add(new SWC.ColumnDefinition());
        AVGrid.RowDefinitions.Add(new SWC.RowDefinition());
        AVGrid.RowDefinitions.Add(new SWC.RowDefinition());
        AV1RichTextBox.Name = AV1RichTextBoxName;
        AV2RichTextBox.Name = AV2RichTextBoxName;

        AV1RichTextBox.Width = 250;
        AV1RichTextBox.Height = 150;

        AV2RichTextBox.Width = 250;
        AV2RichTextBox.Height = 150;

        #endregion

        #region SetupWFControl
        wfh1 = new WindowsFormsHost();
        wfh2 = new WindowsFormsHost();
        WF1FlowLayoutPanel = new SWF.FlowLayoutPanel();
        WF2FlowLayoutPanel = new SWF.FlowLayoutPanel();

        WF1TextBox1 = new SWF.TextBox();
        WF1TextBox2 = new SWF.TextBox();
        WF1RichTextBox = new SWF.RichTextBox();

        WF2TextBox1 = new SWF.TextBox();
        WF2TextBox2 = new SWF.TextBox();
        WF2RichTextBox = new SWF.RichTextBox();

        wfh1.Name = WF1Name;
        WF1FlowLayoutPanel.Name = WF1FlowLayoutPanelName;
        WF1TextBox1.Width = 250;
        WF1TextBox1.Height = 150;
        WF1TextBox1.Name = WF1TextBox1Name;
        WF1TextBox1.WordWrap = true;
        WF1TextBox1.Multiline = true;
        WF1TextBox2.Width = 250;
        WF1TextBox2.Height = 150;
        WF1TextBox2.Name = WF1TextBox2Name;
        WF1TextBox2.WordWrap = true;
        WF1TextBox2.Multiline = true;
        WF1RichTextBox.Width = 250;
        WF1RichTextBox.Height = 150;
        WF1RichTextBox.Name = WF1RichTextBoxName;

        wfh2.Name = WF2Name;
        WF2FlowLayoutPanel.Name = WF2FlowLayoutPanelName; 
        WF2TextBox1.Name = WF2TextBox1Name;
        WF2TextBox1.Width = 250;
        WF2TextBox1.Height = 150;
        WF2TextBox1.WordWrap = true;
        WF2TextBox1.Multiline = true;
        WF2TextBox2.Width = 250;
        WF2TextBox2.Height = 150;
        WF2TextBox2.Name = WF2TextBox2Name;
        WF2TextBox2.WordWrap = true;
        WF2TextBox2.Multiline = true;
        WF2RichTextBox.Width = 250;
        WF2RichTextBox.Height = 150;
        WF2RichTextBox.Name = WF2RichTextBoxName;
   
        wfh1.Child = WF1FlowLayoutPanel;
        WF1FlowLayoutPanel.Controls.Add(WF1RichTextBox);
        WF1FlowLayoutPanel.Controls.Add(WF1TextBox1);
        WF1FlowLayoutPanel.Controls.Add(WF1TextBox2);
        WF1FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;

        wfh2.Child = WF2FlowLayoutPanel;
        WF2FlowLayoutPanel.Controls.Add(WF2RichTextBox);
        WF2FlowLayoutPanel.Controls.Add(WF2TextBox1);
        WF2FlowLayoutPanel.Controls.Add(WF2TextBox2);
        WF2FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;

        #endregion

        #region LayoutWindow
        this.Title = WindowTitleName;

        AVGrid.Children.Add(wfh1);
        AVGrid.Children.Add(wfh2);
        AVGrid.Children.Add(AV1RichTextBox);

        SWC.Grid.SetColumn(wfh1, 0);
        SWC.Grid.SetRow(wfh1, 0);

        SWC.Grid.SetColumn(wfh2, 1);
        SWC.Grid.SetRow(wfh2, 0);

        SWC.Grid.SetColumn(AV1RichTextBox, 0);
        SWC.Grid.SetRow(AV1RichTextBox, 1);
        SWC.Grid.SetColumnSpan(AV1RichTextBox, 2);

        this.Content = AVGrid;

        #endregion

        #region SecondAVWindow
        AV2Window.Title = AV2Window.Name = AV2WindowName;
        AV2Window.Content = AV2RichTextBox;
        AV2Window.SizeToContent = SW.SizeToContent.WidthAndHeight;
        AV2Window.Top = 0;
        AV2Window.Left = 450;
        #endregion

        #region WinFormWindow
        form = new SWF.Form();
        FRMTextBox1 = new SWF.TextBox();
        FRMTextBox2 = new SWF.TextBox();
        FRMRichTextBox = new SWF.RichTextBox();
        FRMFlowLayoutPanel = new SWF.FlowLayoutPanel();

        form.Name = FRMName;
        FRMRichTextBox.Name = FRMRichTextBoxName;
        FRMFlowLayoutPanel.Name = FRMFlowLayoutPanelName;
        FRMTextBox1.Name = FRMTextBox1Name;
        FRMTextBox2.Name = FRMTextBox2Name;

        FRMTextBox1.Width = 250;
        FRMTextBox1.Height = 150;
        FRMTextBox1.WordWrap = true;
        FRMTextBox1.Multiline = true;

        FRMTextBox2.Width = 250;
        FRMTextBox2.Height = 150;
        FRMTextBox2.WordWrap = true;
        FRMTextBox2.Multiline = true;

        FRMRichTextBox.Width = 250;
        FRMRichTextBox.Height = 150;

        FRMFlowLayoutPanel.Controls.Add(FRMTextBox1);
        FRMFlowLayoutPanel.Controls.Add(FRMTextBox2);
        FRMFlowLayoutPanel.Controls.Add(FRMRichTextBox);
        FRMFlowLayoutPanel.AutoSize = true;
        FRMFlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;

        form.Controls.Add(FRMFlowLayoutPanel);
        form.AutoSize = true;
        form.Top = 0;
        form.Left = 450;

        #endregion

        p.log.WriteLine("TestSetup -- End ");
    }

    /// <summary>
    /// hide or show WFH windows
    /// wfh1 -- Button, CheckBox
    /// wfh2 -- Radio Button
    /// wfh3 -- NumericUpDown
    /// wfh4 -- Vertical ScrollBar
    /// wfh5 -- Horizontal ScrollBar
    /// wfh6 -- TextBox
    /// wfh7 -- DataGridView
    /// </summary>
    /// <param name="ScenarioIndex"></param> zero base index
    private void HideShowGroup(int ScenarioIndex)
    {
        switch (ScenarioIndex)
        {
            case 1: //@ Cut, Copy and Paste between WF controls in a single WFH
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                break;
            case 2: //@ Cut, Copy and Paste between WF controls in seperate WFH's
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Visible;
                AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                break;
            case 3: //@ Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox using it's clipboard functionality
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                AV1RichTextBox.Visibility = System.Windows.Visibility.Visible;
                break;
            case 4: //@ Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox on a different AV window
                AV2Window.Show();
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                break;
            case 5: //@ Cut, Copy and Paste between a WF control in a WFH and a WF RichTextbox on a different Windows Form
                form.Show();
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                break;
            case 6: //@ Cut, Copy and Paste between WF controls in a single WFH and Notepad
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Collapsed;
                AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                break;
            case 7: //@ Cut, Copy and Paste Image between WF controls in a single WFH 
                // Clear the control
                AV1RichTextBox.SelectAll();
                AV1RichTextBox.Cut();
                WF1RichTextBox.SelectAll();
                WF1RichTextBox.Cut();
                WF2RichTextBox.SelectAll();
                WF2RichTextBox.Cut();

                bmp1 = new System.Drawing.Bitmap("beany.bmp");
                bmp2 = new System.Drawing.Bitmap("tulip_farm.bmp");
                // Copy the first bitmap to the clipboard. -- using System.Windows.Clipboard
                SW.Clipboard.SetDataObject(bmp1);
                // Paste the bitmap to the AV RichTextBox
                AV1RichTextBox.Paste();

                // Copy the second bitmap to the clipboard. -- using System.Windows.Forms.Clipboard
                SWF.Clipboard.SetDataObject(bmp2);
                // Paster the second bitmap to the WF RichTextBox
                WF1RichTextBox.Paste();
                // Get the format for the object type.
                //SWF.DataFormats.Format myFormat = SWF.DataFormats.GetFormat(SWF.DataFormats.Bitmap);
                //WF1RichTextBox.Paste(myFormat);

                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Visible;
                AV1RichTextBox.Visibility = System.Windows.Visibility.Visible;
                break;
            default:
                wfh1.Visibility = System.Windows.Visibility.Visible;
                wfh2.Visibility = System.Windows.Visibility.Visible;
                AV1RichTextBox.Visibility = System.Windows.Visibility.Visible;
                break;
        }
    }
    #endregion

    #region HelperFunction
    // TODO :: need to figure out why the DocumentRange.GetText() returns 
    // 1+ char for WF RichTextBox
    private string GetTextboxText(UIObject ctrl)
    {
        string text = String.Empty;
        int TrimNChar = 0;

        if (ctrl.ClassName.IndexOf("WindowsForms10.RichEdit") != -1)
            TrimNChar = 1;

        // get text from textbox
        MitaControl.Edit edit1 = new MitaControl.Edit(ctrl);

        //text = edit1.Value;
        text = edit1.DocumentRange.GetText(-1);
        text = text.Substring(0, text.Length - TrimNChar);

        return text;
    }

    //void GetBitmap(object sender)
    //{
    //    SWC.RichTextBox rtb = sender as SWC.RichTextBox;
    //    rtb.SelectAll();
    //    rtb.Copy();
    //    System.Drawing.Image bmp = SWF.Clipboard.GetImage();
    //    bmp.Save("xxx.bmp");
    //}

    //void GetWFBitmap(object sender)
    //{
    //    SWF.RichTextBox rtb = sender as SWF.RichTextBox;
    //    rtb.SelectAll();
    //    rtb.Copy();
    //    System.Drawing.Image bmp = SWF.Clipboard.GetImage();
    //    bmp.Save("yyy.bmp");
    //}    
#endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Cut, Copy and Paste between WF controls in a single WFH

//@ Cut, Copy and Paste between WF controls in seperate WFH's

//@ Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox using it's clipboard functionality

//@ Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox on a different AV window

//@ Cut, Copy and Paste between a WF control in a WFH and a WF RichTextbox on a different Windows Form

//@ Cut, Copy and Paste between WF controls in a single WFH and Notepad

//@ Cut, Copy and Paste Image between WF controls in a single WFH 
