// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
//
namespace WindowsFormsHostTests
{
    public class CutAndPaste : WPFReflectBase
    {
        #region TestVariables

        private delegate void myEventHandler(object sender);
        private static MethodInfo[] s_mi;
        private int _scenarioIndex = 0;
        private UIObject _uiApp;
        private bool _debug = false;         // set this true for TC debugging
        private System.Drawing.Bitmap _bmp1;
        private System.Drawing.Bitmap _bmp2;

        private SWC.Grid _AVGrid;
        private SWC.RichTextBox _AV1RichTextBox;
        private SWC.RichTextBox _AV2RichTextBox;
        private SW.Window _AV2Window;

        private WindowsFormsHost _wfh1;
        private WindowsFormsHost _wfh2;
        private SWF.FlowLayoutPanel _WF1FlowLayoutPanel;
        private SWF.FlowLayoutPanel _WF2FlowLayoutPanel;
        private SWF.TextBox _WF1TextBox1;
        private SWF.TextBox _WF1TextBox2;
        private SWF.RichTextBox _WF1RichTextBox;
        private SWF.TextBox _WF2TextBox1;
        private SWF.TextBox _WF2TextBox2;
        private SWF.RichTextBox _WF2RichTextBox;

        private SWF.Form _form;
        private SWF.FlowLayoutPanel _FRMFlowLayoutPanel;
        private SWF.TextBox _FRMTextBox1;
        private SWF.TextBox _FRMTextBox2;
        private SWF.RichTextBox _FRMRichTextBox;

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
            _scenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
            HideShowGroup(_scenarioIndex);
            // move the mouse point to the corner
    //        Mouse.Instance.Move(new System.Drawing.Point(0, 0));
            Utilities.SleepDoEvents(2);
            return base.BeforeScenario(p, scenario);
        }

        protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
        {
            if (_scenarioIndex == 4)
                _AV2Window.Close();
            else if (_scenarioIndex == 5)
                _form.Close();
            base.AfterScenario(p, scenario, result);
        }

        protected override void InitTest(TParams p) 
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            s_mi = GetAllScenarios(this);
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
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl1;
            UIObject ctrl2;

            p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
            ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
            ctrl2 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox2Name));
            TestCutPaste(sr, p, ctrl1, ctrl2);

            p.log.WriteLine("--- Cut, Copy and Paste between TextBox and RichTextBox ---");
            ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
            ctrl2 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1RichTextBoxName));
            TestCutPaste(sr, p, ctrl1, ctrl2);
            Utilities.SleepDoEvents(2);

            return sr;
        }

        [Scenario("Cut, Copy and Paste between WF controls in seperate WFH's")]
        public ScenarioResult Scenario2(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl1;
            UIObject ctrl2;

            p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
            ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
            ctrl2 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF2TextBox2Name));
            TestCutPaste(sr, p, ctrl1, ctrl2);

            p.log.WriteLine("--- Cut, Copy and Paste between TextBox and RichTextBox ---");
            ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
            ctrl2 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF2RichTextBoxName));
            TestCutPaste(sr, p, ctrl1, ctrl2);

            return sr;
        }

        [Scenario("Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox using it's clipboard functionality")]
        public ScenarioResult Scenario3(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl1;
            UIObject ctrl2;

            p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
            ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
            ctrl2 = _uiApp.Descendants.Find(UICondition.CreateFromId(AV1RichTextBoxName));
            TestCutPaste(sr, p, ctrl1, ctrl2);

            return sr;
        }

        [Scenario("Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox on a different AV window")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject uiAVWindow = UIObject.Root.Children.Find(UICondition.CreateFromName(AV2WindowName));
            UIObject ctrl1;
            UIObject ctrl2;

            p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
            ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
            ctrl2 = uiAVWindow.Descendants.Find(UICondition.CreateFromId(AV2RichTextBoxName));
            TestCutPaste(sr, p, ctrl1, ctrl2);

            return sr;
        }

        [Scenario("Cut, Copy and Paste between a WF control in a WFH and a WF RichTextbox on a different Windows Form")]
        public ScenarioResult Scenario5(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject uiWFWindow = UIObject.Root.Children.Find(UICondition.CreateFromId(FRMName));
            UIObject ctrl1;
            UIObject ctrl2;

            p.log.WriteLine("--- Cut, Copy and Paste between two TextBox ---");
            ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
            ctrl2 = uiWFWindow.Descendants.Find(UICondition.CreateFromId(FRMTextBox1Name));
            TestCutPaste(sr, p, ctrl1, ctrl2);

            p.log.WriteLine("--- Cut, Copy and Paste between TextBox and RichTextBox ---");
            ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
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
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject uiNotepad = UIObject.Root.Children.Find(UICondition.CreateFromClassName("Notepad"));
            UIObject ctrl1;
            UIObject ctrl2;

            p.log.WriteLine("--- Cut, Copy and Paste - from WF TextBox to Notepad ---");
            ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1TextBox1Name));
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
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(AV1RichTextBoxName));
            UIObject ctrl2 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF1RichTextBoxName));
            UIObject ctrl3 = _uiApp.Descendants.Find(UICondition.CreateFromId(WF2RichTextBoxName));

            System.Drawing.Bitmap bmpOriginal = Utilities.GetBitmapOfControl(_WF1RichTextBox, true);
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
            bmpSource = Utilities.GetBitmapOfControl(_WF1RichTextBox, true);
            bmpTarget = Utilities.GetBitmapOfControl(_WF2RichTextBox, true);
            if (_debug)
            {
                bmpSource.Save("_bmpSource.bmp");
                bmpTarget.Save("_bmpTarget.bmp");
            }
            WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(bmpSource, bmpTarget), "bitmap should match");

            p.log.WriteLine("Cut and Paste from WF RichTextBox into another WF RichTextBox");
            TestCutPasteImage(sr, p, ctrl2, ctrl3, true);
            ctrl1.SetFocus();
            bmpTarget = Utilities.GetBitmapOfControl(_WF2RichTextBox, true);
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

            _AVGrid = new SWC.Grid();
            _AV1RichTextBox = new SWC.RichTextBox();

            _AV2Window = new SW.Window();
            _AV2RichTextBox = new SWC.RichTextBox();

            _AVGrid.Name = AVGridName;
            _AVGrid.ShowGridLines = true;
            _AVGrid.ColumnDefinitions.Add(new SWC.ColumnDefinition());
            _AVGrid.ColumnDefinitions.Add(new SWC.ColumnDefinition());
            _AVGrid.RowDefinitions.Add(new SWC.RowDefinition());
            _AVGrid.RowDefinitions.Add(new SWC.RowDefinition());
            _AV1RichTextBox.Name = AV1RichTextBoxName;
            _AV2RichTextBox.Name = AV2RichTextBoxName;

            _AV1RichTextBox.Width = 250;
            _AV1RichTextBox.Height = 150;

            _AV2RichTextBox.Width = 250;
            _AV2RichTextBox.Height = 150;

            #endregion

            #region SetupWFControl
            _wfh1 = new WindowsFormsHost();
            _wfh2 = new WindowsFormsHost();
            _WF1FlowLayoutPanel = new SWF.FlowLayoutPanel();
            _WF2FlowLayoutPanel = new SWF.FlowLayoutPanel();

            _WF1TextBox1 = new SWF.TextBox();
            _WF1TextBox2 = new SWF.TextBox();
            _WF1RichTextBox = new SWF.RichTextBox();

            _WF2TextBox1 = new SWF.TextBox();
            _WF2TextBox2 = new SWF.TextBox();
            _WF2RichTextBox = new SWF.RichTextBox();

            _wfh1.Name = WF1Name;
            _WF1FlowLayoutPanel.Name = WF1FlowLayoutPanelName;
            _WF1TextBox1.Width = 250;
            _WF1TextBox1.Height = 150;
            _WF1TextBox1.Name = WF1TextBox1Name;
            _WF1TextBox1.WordWrap = true;
            _WF1TextBox1.Multiline = true;
            _WF1TextBox2.Width = 250;
            _WF1TextBox2.Height = 150;
            _WF1TextBox2.Name = WF1TextBox2Name;
            _WF1TextBox2.WordWrap = true;
            _WF1TextBox2.Multiline = true;
            _WF1RichTextBox.Width = 250;
            _WF1RichTextBox.Height = 150;
            _WF1RichTextBox.Name = WF1RichTextBoxName;

            _wfh2.Name = WF2Name;
            _WF2FlowLayoutPanel.Name = WF2FlowLayoutPanelName; 
            _WF2TextBox1.Name = WF2TextBox1Name;
            _WF2TextBox1.Width = 250;
            _WF2TextBox1.Height = 150;
            _WF2TextBox1.WordWrap = true;
            _WF2TextBox1.Multiline = true;
            _WF2TextBox2.Width = 250;
            _WF2TextBox2.Height = 150;
            _WF2TextBox2.Name = WF2TextBox2Name;
            _WF2TextBox2.WordWrap = true;
            _WF2TextBox2.Multiline = true;
            _WF2RichTextBox.Width = 250;
            _WF2RichTextBox.Height = 150;
            _WF2RichTextBox.Name = WF2RichTextBoxName;
    
            _wfh1.Child = _WF1FlowLayoutPanel;
            _WF1FlowLayoutPanel.Controls.Add(_WF1RichTextBox);
            _WF1FlowLayoutPanel.Controls.Add(_WF1TextBox1);
            _WF1FlowLayoutPanel.Controls.Add(_WF1TextBox2);
            _WF1FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;

            _wfh2.Child = _WF2FlowLayoutPanel;
            _WF2FlowLayoutPanel.Controls.Add(_WF2RichTextBox);
            _WF2FlowLayoutPanel.Controls.Add(_WF2TextBox1);
            _WF2FlowLayoutPanel.Controls.Add(_WF2TextBox2);
            _WF2FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;

            #endregion

            #region LayoutWindow
            this.Title = WindowTitleName;

            _AVGrid.Children.Add(_wfh1);
            _AVGrid.Children.Add(_wfh2);
            _AVGrid.Children.Add(_AV1RichTextBox);

            SWC.Grid.SetColumn(_wfh1, 0);
            SWC.Grid.SetRow(_wfh1, 0);

            SWC.Grid.SetColumn(_wfh2, 1);
            SWC.Grid.SetRow(_wfh2, 0);

            SWC.Grid.SetColumn(_AV1RichTextBox, 0);
            SWC.Grid.SetRow(_AV1RichTextBox, 1);
            SWC.Grid.SetColumnSpan(_AV1RichTextBox, 2);

            this.Content = _AVGrid;

            #endregion

            #region SecondAVWindow
            _AV2Window.Title = _AV2Window.Name = AV2WindowName;
            _AV2Window.Content = _AV2RichTextBox;
            _AV2Window.SizeToContent = SW.SizeToContent.WidthAndHeight;
            _AV2Window.Top = 0;
            _AV2Window.Left = 450;
            #endregion

            #region WinFormWindow
            _form = new SWF.Form();
            _FRMTextBox1 = new SWF.TextBox();
            _FRMTextBox2 = new SWF.TextBox();
            _FRMRichTextBox = new SWF.RichTextBox();
            _FRMFlowLayoutPanel = new SWF.FlowLayoutPanel();

            _form.Name = FRMName;
            _FRMRichTextBox.Name = FRMRichTextBoxName;
            _FRMFlowLayoutPanel.Name = FRMFlowLayoutPanelName;
            _FRMTextBox1.Name = FRMTextBox1Name;
            _FRMTextBox2.Name = FRMTextBox2Name;

            _FRMTextBox1.Width = 250;
            _FRMTextBox1.Height = 150;
            _FRMTextBox1.WordWrap = true;
            _FRMTextBox1.Multiline = true;

            _FRMTextBox2.Width = 250;
            _FRMTextBox2.Height = 150;
            _FRMTextBox2.WordWrap = true;
            _FRMTextBox2.Multiline = true;

            _FRMRichTextBox.Width = 250;
            _FRMRichTextBox.Height = 150;

            _FRMFlowLayoutPanel.Controls.Add(_FRMTextBox1);
            _FRMFlowLayoutPanel.Controls.Add(_FRMTextBox2);
            _FRMFlowLayoutPanel.Controls.Add(_FRMRichTextBox);
            _FRMFlowLayoutPanel.AutoSize = true;
            _FRMFlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;

            _form.Controls.Add(_FRMFlowLayoutPanel);
            _form.AutoSize = true;
            _form.Top = 0;
            _form.Left = 450;

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
                    _wfh1.Visibility = System.Windows.Visibility.Visible;
                    _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                    _AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 2: //@ Cut, Copy and Paste between WF controls in seperate WFH's
                    _wfh1.Visibility = System.Windows.Visibility.Visible;
                    _wfh2.Visibility = System.Windows.Visibility.Visible;
                    _AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 3: //@ Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox using it's clipboard functionality
                    _wfh1.Visibility = System.Windows.Visibility.Visible;
                    _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                    _AV1RichTextBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 4: //@ Cut, Copy and Paste between a WF control in a WFH and an Avalon RichTextbox on a different AV window
                    _AV2Window.Show();
                    _wfh1.Visibility = System.Windows.Visibility.Visible;
                    _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                    _AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 5: //@ Cut, Copy and Paste between a WF control in a WFH and a WF RichTextbox on a different Windows Form
                    _form.Show();
                    _wfh1.Visibility = System.Windows.Visibility.Visible;
                    _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                    _AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 6: //@ Cut, Copy and Paste between WF controls in a single WFH and Notepad
                    _wfh1.Visibility = System.Windows.Visibility.Visible;
                    _wfh2.Visibility = System.Windows.Visibility.Collapsed;
                    _AV1RichTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 7: //@ Cut, Copy and Paste Image between WF controls in a single WFH 
                    // Clear the control
                    _AV1RichTextBox.SelectAll();
                    _AV1RichTextBox.Cut();
                    _WF1RichTextBox.SelectAll();
                    _WF1RichTextBox.Cut();
                    _WF2RichTextBox.SelectAll();
                    _WF2RichTextBox.Cut();

                    _bmp1 = new System.Drawing.Bitmap("beany.bmp");
                    _bmp2 = new System.Drawing.Bitmap("tulip_farm.bmp");
                    // Copy the first bitmap to the clipboard. -- using System.Windows.Clipboard
                    SW.Clipboard.SetDataObject(_bmp1);
                    // Paste the bitmap to the AV RichTextBox
                    _AV1RichTextBox.Paste();

                    // Copy the second bitmap to the clipboard. -- using System.Windows.Forms.Clipboard
                    SWF.Clipboard.SetDataObject(_bmp2);
                    // Paster the second bitmap to the WF RichTextBox
                    _WF1RichTextBox.Paste();
                    // Get the format for the object type.
                    //SWF.DataFormats.Format myFormat = SWF.DataFormats.GetFormat(SWF.DataFormats.Bitmap);
                    //WF1RichTextBox.Paste(myFormat);

                    _wfh1.Visibility = System.Windows.Visibility.Visible;
                    _wfh2.Visibility = System.Windows.Visibility.Visible;
                    _AV1RichTextBox.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    _wfh1.Visibility = System.Windows.Visibility.Visible;
                    _wfh2.Visibility = System.Windows.Visibility.Visible;
                    _AV1RichTextBox.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }
        #endregion

        #region HelperFunction
        
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
