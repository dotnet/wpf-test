// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using SW = System.Windows;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;


using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;


// Testcase:    AccessKeys
// Description: Verify that WF controls with Accessor Keys work as expected
namespace WindowsFormsHostTests
{

    public class AccessKeys : WPFReflectBase {

        #region TestVariables

        private static MethodInfo[] s_mi;

        private delegate void myEventHandler(object sender);
    
        private UIObject _uiApp;
        private int _scenarioIndex = 0;

        //private enum EventState { Off = 0, Started, Ended };
        //private EventState state = EventState.Off;

        public SWI.RoutedCommand CopyCommand;
        public SWI.RoutedCommand PasteCommand;
        public SWI.RoutedCommand PasteFnCommand;

        private SWC.StackPanel _av_StackPanel;
        private SWC.Label _av_Label;
        private SWC.TextBox _av_TextBox;
        private SWC.CheckBox _av_CheckBox;
        private SWC.Button _av_Button;
        private SWC.CheckBox _av_CheckBoxSameAccessKey;
        private SWC.Menu _av_Menu;
        private SWC.MenuItem _av_MenuItem;
        private SWC.MenuItem _av_SubMenuItem1;
        private SWC.MenuItem _av_SubMenuItem2;
        private SWC.MenuItem _av_SubMenuItem3;

        private WindowsFormsHost _wfh;
        private SWF.FlowLayoutPanel _wf_FlowLayoutPanel;
        private SWF.CheckBox _wf_CheckBox;
        private SWF.Label _wf_Label;
        private SWF.TextBox _wf_TextBox;
        private SWF.CheckBox _wf_CheckBoxSameAccessKey;
        private SWF.MenuStrip _wf_MenuStrip;
        private SWF.ToolStripMenuItem _wf_StripMenuItem;
        private SWF.ToolStripMenuItem _wf_StripMenuItemCopy;
        private SWF.ToolStripMenuItem _wf_StripMenuItemPaste;
        private SWF.ToolStripMenuItem _wf_StripMenuItemPasteFn;
        private SWF.HelpProvider _wf_HelpProvider;

        private WindowsFormsHost _wfh2;
        private SWF.FlowLayoutPanel _wf_FlowLayoutPanel2;
        private SWF.CheckBox _wf_CheckBox2;
        private SWF.Label _wf_Label2;
        private SWF.TextBox _wf_TextBox2;

        private const string WindowTitleName = "AccessKeysTest";

        private const string AVButtonName = "AVButton";
        private const string AVCheckBoxName = "AVCheckBox_A";
        private const string AVLabelName = "AVLabel_B";
        private const string AVTextBoxName = "AVTextBox";
        private const string AVCheckBoxSameAccessKeyName = "AVCheckBox_SameAccessKey";
        private const string AVMenuName = "AVMenu";
        private const string AVMenuItemName = "AVMenuItem";
        private const string AVSubMenuItem1Name = "AVSubMenuItem1";
        private const string AVSubMenuItem2Name = "AVSubMenuItem2";
        private const string AVSubMenuItem3Name = "AVSubMenuItem3";

        private const string WFCheckBoxSameAccessKeyName = "WFCheckBox&SameAccessKey";
        private const string WFCheckBoxName = "WFCheckBox1&Z";
        private const string WFLabelName = "WFLabel1&Y";
        private const string WFTextBoxName = "WFTextBox1";
        private const string WFFlowLayoutPanelName = "WFFlowLayoutPanelText1";
        private const string WFHostName = "WFH1";
        private const string WFMenuStripName = "WFMenuStrip";
        private const string WFStripMenuItemName = "WFStripMenuItem";
        private const string WFStripMenuItemCopyName = "WFStripMenuItemCopy";
        private const string WFStripMenuItemPasteName = "WFStripMenuItemPaste";
        private const string WFStripMenuItemPasteFnName = "WFStripMenuItemPasteFn";
        private const string WFHelpProviderName = "WFHelpProvider";

        private const string WFCheckBoxName2 = "WFCheckBox2&X";
        private const string WFLabelName2 = "WFLabel2&W";
        private const string WFTextBoxName2 = "WFTextBox2";
        private const string WFFlowLayoutPanelName2 = "WFFlowLayoutPanelText2";
        private const string WFHostName2 = "WFH2";

        #endregion

        #region Testcase setup
        public AccessKeys(string[] args) : base(args) { }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            //state = EventState.Off;
            _scenarioIndex = Convert.ToInt32(scenario.Name.Substring(8)) - 1;
            return base.BeforeScenario(p, scenario);
        }

        protected override void InitTest(TParams p) 
        {
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            s_mi = GetAllScenarios(this);
            this.UseMITA = true;
            base.InitTest(p);
            TestSetup(p);
            MyPause();
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Navigate between an AV control and WF control via accessor key")]
        public ScenarioResult Scenario1(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            _uiApp.SetFocus();

            // Alt-A to AV CheckBox
            p.log.WriteLine("Setting focus to AV CheckBox");
            _uiApp.SendKeys("%A");
            WPFMiscUtils.IncCounters(sr, true, HasFocus(AVCheckBoxName), "AV CheckBox not getting focus", p.log);

            // Alt-Z to WF CheckBox
            p.log.WriteLine("Setting focus to WF CheckBox");
            _uiApp.SendKeys("%Z");
            WPFMiscUtils.IncCounters(sr, true, HasFocus(WFCheckBoxName), "WF CheckBox not getting focus", p.log);

            // Alt-B to AV TextBox
            p.log.WriteLine("Setting focus to AV TextBox");
            _uiApp.SendKeys("%B");
            WPFMiscUtils.IncCounters(sr, true, HasFocus(AVTextBoxName), "AV TextBox not getting focus", p.log);

            // Alt-Y to WF TextBox
            p.log.WriteLine("Setting focus to WF TextBox");

            _uiApp.SendKeys("%Y");
            _uiApp.SendKeys("%Y");
            WPFMiscUtils.IncCounters(sr, true, HasFocus(WFTextBoxName), "WF CheckBox not getting focus", p.log);

            return sr;
        }

        [Scenario("Navigate between an WF control in one WFH and another WF control in another WFH via accessor key")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            _uiApp.SetFocus();

            // Alt-Z to WF CheckBox in WFH1
            p.log.WriteLine("Setting focus to WF CheckBox in WFH1");
            _uiApp.SendKeys("%Z");
            WPFMiscUtils.IncCounters(sr, true, HasFocus(WFCheckBoxName), "WF CheckBox in WFH1 not getting focus", p.log);

            // Alt-W to WF TextBox in WFH2
            p.log.WriteLine("Setting focus to WF TextBox in WFH2");

            _uiApp.SendKeys("%W");
            _uiApp.SendKeys("%W");
            WPFMiscUtils.IncCounters(sr, true, HasFocus(WFTextBoxName2), "WF CheckBox in WFH2 not getting focus", p.log);

            // Alt-Y to WF TextBox in WFH1
            p.log.WriteLine("Setting focus to WF TextBox in WFH1");

            _uiApp.SendKeys("%Y");
            _uiApp.SendKeys("%Y");
            WPFMiscUtils.IncCounters(sr, true, HasFocus(WFTextBoxName), "WF CheckBox in WFH1 not getting focus", p.log);

            // Alt-X to WF CheckBox in WFH2
            p.log.WriteLine("Setting focus to WF CheckBox in WFH2");
            _uiApp.SendKeys("%X");
            WPFMiscUtils.IncCounters(sr, true, HasFocus(WFCheckBoxName2), "WF CheckBox in WFH2 not getting focus", p.log);

            return sr;
        }

        [Scenario("Navigate between an AV control and WF control that have the same accessor key")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            _uiApp.SetFocus(); 
            
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(AVButtonName));
            ctrl.SetFocus();

            // Alt-S to AV/WF CheckBox with the same AccessKey
            p.log.WriteLine("Setting focus to WF/AV CheckBox -- WF CheckBox should get the focus");
            _uiApp.SendKeys("%S");
            WPFMiscUtils.IncCounters(sr, true, HasFocus(WFCheckBoxSameAccessKeyName), "WF CheckBox not getting focus", p.log);
            WPFMiscUtils.IncCounters(sr, false, HasFocus(AVCheckBoxSameAccessKeyName), "AV CheckBox is getting focus", p.log);

            return sr;
        }

        [Scenario("Verify CTRL+T fires menu item as expected (gotta hook up a WF menu strip item with CTRL+T first).")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            _uiApp.SetFocus();

            // Make a copy of the WF TextBox Text by using the Copy and Paste Menu hooks
            UIObject uiWFTextBox = _uiApp.Descendants.Find(UICondition.CreateFromId(WFTextBoxName));
            string CompareString = GetTextboxText(WFTextBoxName);
            uiWFTextBox.SetFocus();
            //uiWFTextBox.Highlight();
            uiWFTextBox.SendKeys("{HOME}");
            uiWFTextBox.SendKeys("^+{END}");
            uiWFTextBox.SendKeys("^m");
            uiWFTextBox.SendKeys("{END}");
            uiWFTextBox.SendKeys("^n");

            WPFMiscUtils.IncCounters(sr, CompareString + CompareString, GetTextboxText(WFTextBoxName), "Menu item did not receive the click event", p.log);
            return sr;
        }

        [Scenario("Verify CTRL+T fires menu item as expected (gotta hook up a AV menu item with CTRL+T first).")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            _uiApp.SetFocus();

            // Make a copy of the AV TextBox Text by using the Copy and Paste Menu hooks
            UIObject uiAVTextBox = _uiApp.Descendants.Find(UICondition.CreateFromId(AVTextBoxName));
            string CompareString = GetTextboxText(AVTextBoxName);
            uiAVTextBox.SetFocus();
            uiAVTextBox.SendKeys("{HOME}");
            uiAVTextBox.SendKeys("^+{END}");
            uiAVTextBox.SendKeys("^t");
            uiAVTextBox.SendKeys("{END}");
            uiAVTextBox.SendKeys("^u");

            WPFMiscUtils.IncCounters(sr, CompareString + CompareString, GetTextboxText(AVTextBoxName), "Menu item did not receive the click event", p.log);
            //WPFMiscUtils.IncCounters(sr, EventState.Ended, state, "Menu item did not receive the click event", p.log);

            return sr;
        }

        [Scenario("Use a function key as an accessor")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            _uiApp.SetFocus(); 

            // Test for the WFH
            // F2 as the Paste operation
            // Make a copy of the WF TextBox Text by using the Copy and Paste Menu hooks
            UIObject uiWFTextBox = _uiApp.Descendants.Find(UICondition.CreateFromId(WFTextBoxName));
            uiWFTextBox.SetFocus();
            string CompareString = GetTextboxText(WFTextBoxName);
            //uiWFTextBox.Highlight();
            uiWFTextBox.SendKeys("{HOME}");
            uiWFTextBox.SendKeys("^+{END}");
            uiWFTextBox.SendKeys("^m");
            uiWFTextBox.SendKeys("{END}");
            uiWFTextBox.SendKeys("{F2}");

            WPFMiscUtils.IncCounters(sr, CompareString+CompareString, GetTextboxText(WFTextBoxName), "Menu item did not receive the click event", p.log);

            // Test for the AV
            // F3 as the Paste operation
            // Make a copy of the AV TextBox Text by using the Copy and Paste Menu hooks
            _uiApp.SetFocus();
            UIObject uiAVTextBox = _uiApp.Descendants.Find(UICondition.CreateFromId(AVTextBoxName));
            uiAVTextBox.SetFocus();
            CompareString = GetTextboxText(AVTextBoxName);
            uiAVTextBox.SendKeys("{HOME}");
            uiAVTextBox.SendKeys("^+{END}");
            uiAVTextBox.SendKeys("^t");
            uiAVTextBox.SendKeys("{END}");
            uiAVTextBox.SendKeys("{F3}");

            WPFMiscUtils.IncCounters(sr, CompareString + CompareString, GetTextboxText(AVTextBoxName), "Menu item did not receive the click event", p.log);
            return sr;
        }

        [Scenario("use F1 to bring up a help provider")]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            _uiApp.SetFocus();

            // Set the focus to WF TextBox control
            UIObject uiWFTextBox = _uiApp.Descendants.Find(UICondition.CreateFromId(WFTextBoxName));
            uiWFTextBox.SetFocus();
            // press the F1 key
            uiWFTextBox.SendKeys("{F1}");
            WPFReflectBase.DoEvents();
            System.Threading.Thread.Sleep(200);

            bool bHelpDisplayed;
            try
            {
                Utilities.SleepDoEvents(40);
                UIObject uiHelpWindow = _uiApp.Descendants.Find(UICondition.CreateFromName("AccessKeys Help"));
                bHelpDisplayed = true;
                //uiHelpWindow.Highlight();
                uiHelpWindow.SendKeys("%{F4}");
            }
            catch (Exception ex)
            {
                p.log.WriteLine(ex.Message);
                bHelpDisplayed = false;
            }
            WPFMiscUtils.IncCounters(sr, true, bHelpDisplayed, "Help file does not get displayed", p.log);
    //        KeepRunningTests = false;
            return sr;
        }

        #endregion


        #region HelperFunction
        // Helper function to set up app for particular Scenario
        private static void MyPause()
        {
            WPFReflectBase.DoEvents();
            System.Threading.Thread.Sleep(10);
        }

        private void TestSetup(TParams p)
        {
            p.log.WriteLine("TestSetup -- Start ");

            this.Title = WindowTitleName;

            #region SetupAVControl
            _av_StackPanel = new SWC.StackPanel();
            _av_CheckBox = new SWC.CheckBox();
            _av_Button = new SWC.Button();
            _av_TextBox = new SWC.TextBox();
            _av_Label = new SWC.Label();
            _av_CheckBoxSameAccessKey = new SWC.CheckBox();

            _av_Button.Content = _av_Button.Name = AVButtonName;
            _av_CheckBox.Content = _av_CheckBox.Name = AVCheckBoxName;
            _av_CheckBoxSameAccessKey.Content = _av_CheckBoxSameAccessKey.Name = AVCheckBoxSameAccessKeyName;
            _av_TextBox.Text = _av_TextBox.Name = AVTextBoxName;
            _av_Label.Content = _av_Label.Name = AVLabelName;
            _av_Label.Target = _av_TextBox;
            #endregion

            #region SetupAvMenu
            // Setup AV Menu 
            // Ctrl-T ==> Copy
            // Ctrl-U ==> Paste
            _av_Menu = new System.Windows.Controls.Menu();
            _av_MenuItem = new System.Windows.Controls.MenuItem();
            _av_SubMenuItem1 = new System.Windows.Controls.MenuItem();
            _av_SubMenuItem2 = new System.Windows.Controls.MenuItem();
            _av_SubMenuItem3 = new System.Windows.Controls.MenuItem();

            _av_Menu.Items.Add(_av_MenuItem);
            _av_MenuItem.Items.Add(_av_SubMenuItem1);
            _av_MenuItem.Items.Add(_av_SubMenuItem2);
            _av_MenuItem.Items.Add(_av_SubMenuItem3);

            _av_Menu.Name = AVMenuName;

            _av_MenuItem.Name = AVMenuItemName;
            _av_MenuItem.Header = AVMenuItemName;

            _av_SubMenuItem1.Name = AVSubMenuItem1Name;
            _av_SubMenuItem1.Header = AVSubMenuItem1Name;
            _av_SubMenuItem1.Command = SWI.ApplicationCommands.Copy;
            _av_SubMenuItem1.InputGestureText = "Ctrl+T";

            _av_SubMenuItem2.Name = AVSubMenuItem2Name;
            _av_SubMenuItem2.Header = AVSubMenuItem2Name;
            _av_SubMenuItem2.Command = SWI.ApplicationCommands.Paste;
            _av_SubMenuItem2.InputGestureText = "Ctrl+U";

            _av_SubMenuItem3.Name = AVSubMenuItem3Name;
            _av_SubMenuItem3.Header = AVSubMenuItem3Name;
            _av_SubMenuItem3.Command = SWI.ApplicationCommands.Paste;
            _av_SubMenuItem3.InputGestureText = "F3";

            // Ctrl-T ===> copy
            SWI.InputGestureCollection CopyInputs = new SWI.InputGestureCollection();
            CopyInputs.Add(new SWI.KeyGesture(SWI.Key.T, SWI.ModifierKeys.Control));
            CopyCommand = new SWI.RoutedCommand("Ctrl-T", typeof(AccessKeys), CopyInputs);
            SWI.CommandBinding cmdBindingCopy = new SWI.CommandBinding(CopyCommand);
            cmdBindingCopy.Executed += delegate { SWI.ApplicationCommands.Copy.Execute(null, null); };
            //cmdBindingCopy.Executed += delegate { state = EventState.Started; };
            CommandBindings.Add(cmdBindingCopy);

            // Ctrl-U ===> paste
            SWI.InputGestureCollection PasteInputs = new SWI.InputGestureCollection();
            PasteInputs.Add(new SWI.KeyGesture(SWI.Key.U, SWI.ModifierKeys.Control));
            PasteCommand = new SWI.RoutedCommand("Ctrl-U", typeof(AccessKeys), PasteInputs);
            SWI.CommandBinding cmdBindingPaste = new SWI.CommandBinding(PasteCommand);
            cmdBindingPaste.Executed += delegate { SWI.ApplicationCommands.Paste.Execute(null, null); };
            //cmdBindingPaste.Executed += delegate { state = EventState.Ended; };
            CommandBindings.Add(cmdBindingPaste);

            // F3 ===> paste
            SWI.InputGestureCollection PasteFnInputs = new SWI.InputGestureCollection();
            PasteFnInputs.Add(new SWI.KeyGesture(SWI.Key.F3));
            PasteFnCommand = new SWI.RoutedCommand("F3", typeof(AccessKeys), PasteFnInputs);
            SWI.CommandBinding cmdBindingPasteFn = new SWI.CommandBinding(PasteFnCommand);
            cmdBindingPasteFn.Executed += delegate { SWI.ApplicationCommands.Paste.Execute(null, null); };
            CommandBindings.Add(cmdBindingPasteFn);
            #endregion

            #region SetupWFH1
            // First Set of WF Controls
            _wfh = new WindowsFormsHost();
            _wf_FlowLayoutPanel = new FlowLayoutPanel();
            _wf_CheckBox = new CheckBox();
            _wf_TextBox = new TextBox();
            _wf_Label = new Label();
            _wf_CheckBoxSameAccessKey = new CheckBox();
            _wf_HelpProvider = new HelpProvider();

            #region SetupWFMenu
            _wf_MenuStrip = new MenuStrip();
            _wf_StripMenuItem = new ToolStripMenuItem();
            _wf_StripMenuItemCopy = new ToolStripMenuItem();
            _wf_StripMenuItemPaste = new ToolStripMenuItem();
            _wf_StripMenuItemPasteFn = new ToolStripMenuItem();
            
            _wf_MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _wf_StripMenuItem });

            _wf_MenuStrip.Location = new System.Drawing.Point(0, 0);
            _wf_MenuStrip.Text = _wf_MenuStrip.Name = WFMenuStripName;
            _wf_MenuStrip.Size = new System.Drawing.Size(442, 24);

            _wf_StripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                _wf_StripMenuItemCopy,
                _wf_StripMenuItemPaste,
                _wf_StripMenuItemPasteFn});
            _wf_StripMenuItem.Text = _wf_StripMenuItem.Name = WFStripMenuItemName;
            _wf_StripMenuItem.Size = new System.Drawing.Size(35, 20);
            
            _wf_StripMenuItemCopy.Text = _wf_StripMenuItemCopy.Name = WFStripMenuItemCopyName;
            _wf_StripMenuItemCopy.ShortcutKeys = ((SWF.Keys)((SWF.Keys.Control | SWF.Keys.M)));
            _wf_StripMenuItemCopy.Size = new System.Drawing.Size(177, 22);
            _wf_StripMenuItemCopy.Click += new EventHandler(wf_StripMenuItemCopy_Click);
            
            _wf_StripMenuItemPaste.Text = _wf_StripMenuItemPaste.Name = WFStripMenuItemPasteName;
            _wf_StripMenuItemPaste.ShortcutKeys = ((SWF.Keys)((SWF.Keys.Control | SWF.Keys.N)));
            _wf_StripMenuItemPaste.Size = new System.Drawing.Size(177, 22);
            _wf_StripMenuItemPaste.Click += new EventHandler(wf_StripMenuItemPaste_Click);
            
            _wf_StripMenuItemPasteFn.Text = _wf_StripMenuItemPasteFn.Name = WFStripMenuItemPasteFnName;
            _wf_StripMenuItemPasteFn.ShortcutKeys = SWF.Keys.F2;
            _wf_StripMenuItemPasteFn.Size = new System.Drawing.Size(177, 22);
            _wf_StripMenuItemPasteFn.Click += new EventHandler(wf_StripMenuItemPaste_Click);
            

            // test.htm file need to be in the School subdirectory
            _wf_HelpProvider.HelpNamespace = @"AccessKeys.chm";
            _wf_HelpProvider.SetShowHelp(_wf_TextBox, true);
            #endregion

            _wfh.Name = WFHostName;
            _wf_FlowLayoutPanel.Name = WFFlowLayoutPanelName;
            _wf_CheckBox.Text = _wf_CheckBox.Name = WFCheckBoxName;
            _wf_CheckBoxSameAccessKey.Text = _wf_CheckBoxSameAccessKey.Name = WFCheckBoxSameAccessKeyName;
            _wf_TextBox.Text = _wf_TextBox.Name = WFTextBoxName;
            _wf_TextBox.Width = 300;
            _wf_Label.Text = _wf_Label.Name = WFLabelName;
            _wf_Label.UseMnemonic = true;
            _wf_CheckBox.TabIndex = 1;
            _wf_Label.TabIndex = 2;
            _wf_TextBox.TabIndex = 3;
            _wf_TextBox.Width = 300;
            _wf_CheckBox.AutoSize = true;
            _wf_Label.AutoSize = true;
            _wf_CheckBoxSameAccessKey.AutoSize = true;

            _wf_FlowLayoutPanel.Controls.Add(_wf_MenuStrip);
            _wf_FlowLayoutPanel.Controls.Add(_wf_CheckBoxSameAccessKey);
            _wf_FlowLayoutPanel.Controls.Add(_wf_CheckBox);
            _wf_FlowLayoutPanel.Controls.Add(_wf_Label);
            _wf_FlowLayoutPanel.Controls.Add(_wf_TextBox);

            _wf_FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
            _wf_FlowLayoutPanel.AutoSize = true;
            _wfh.Child = _wf_FlowLayoutPanel;
            #endregion

            #region SetupWFH2
            // Second Set of WF Controls
            _wfh2 = new WindowsFormsHost();
            _wf_FlowLayoutPanel2 = new FlowLayoutPanel();
            _wf_CheckBox2 = new CheckBox();
            _wf_TextBox2 = new TextBox();
            _wf_Label2 = new Label();

            _wfh2.Name = WFHostName;
            _wf_FlowLayoutPanel2.Name = WFFlowLayoutPanelName2;
            _wf_CheckBox2.Text = _wf_CheckBox2.Name = WFCheckBoxName2;
            _wf_TextBox2.Text = _wf_TextBox2.Name = WFTextBoxName2;
            _wf_Label2.Text = _wf_Label2.Name = WFLabelName2;
            _wf_Label2.UseMnemonic = true;
            _wf_CheckBox2.TabIndex = 4;
            _wf_Label2.TabIndex = 5;
            _wf_TextBox2.TabIndex = 6;
            _wf_TextBox2.Width = 300;
            _wf_CheckBox2.AutoSize = true;
            _wf_Label2.AutoSize = true;

            _wf_FlowLayoutPanel2.Controls.Add(_wf_CheckBox2);
            _wf_FlowLayoutPanel2.Controls.Add(_wf_Label2);
            _wf_FlowLayoutPanel2.Controls.Add(_wf_TextBox2);

            _wf_FlowLayoutPanel2.FlowDirection = SWF.FlowDirection.TopDown;
            _wfh2.Child = _wf_FlowLayoutPanel2;
            #endregion
            
            _av_StackPanel.Children.Add(_av_Menu);
            _av_StackPanel.Children.Add(_av_Button);
            _av_StackPanel.Children.Add(_av_CheckBoxSameAccessKey);
            _av_StackPanel.Children.Add(_av_CheckBox);
            _av_StackPanel.Children.Add(_av_Label);
            _av_StackPanel.Children.Add(_av_TextBox);
            _av_StackPanel.Children.Add(_wfh);
            _av_StackPanel.Children.Add(_wfh2);
            this.Content = _av_StackPanel;

            p.log.WriteLine("TestSetup -- End ");
        }

        void wf_StripMenuItemCopy_Click(object sender, EventArgs e)
        {
            if (_wf_TextBox.SelectionLength > 0)
                // Copy the selected text to the Clipboard.
                _wf_TextBox.Copy();
        }

        void wf_StripMenuItemPaste_Click(object sender, EventArgs e)
        {
            // Determine if there is any text in the Clipboard to paste into the text box.
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                _wf_TextBox.Paste();
            }
        }
    
        private bool HasFocus(string ctrlName)
        {
            // find control by name
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(ctrlName));
            MitaControl.Window wnd = new MitaControl.Window(ctrl);
            return wnd.HasKeyboardFocus;
        }

        private string GetTextboxText(string ctrlName)
        {
            string text = String.Empty;

            // find control by name
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(ctrlName));
            
            // get text from textbox
            MitaControl.Edit edit1 = new MitaControl.Edit(ctrl);

            //text = edit1.Value;
            text = edit1.DocumentRange.GetText(-1);

            return text;
        }

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Navigate between an AV control and WF control via accessor key

//@ Navigate between an WF control in one WFH and another WF control in another WFH via accessor key

//@ Navigate between an AV control and WF control that have the same accessor key

//@ Verify CTRL+T fires menu item as expected (gotta hook up a WF menu strip item with CTRL+T first).

//@ Verify CTRL+T fires menu item as expected (gotta hook up a AV menu item with CTRL+T first).

//@ Use a function key as an accessor

//@ use F1 to bring up a help provider
