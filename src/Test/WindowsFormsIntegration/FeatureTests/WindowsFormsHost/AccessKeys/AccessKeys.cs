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

//
// Testcase:    AccessKeys
// Description: Verify that WF controls with Accessor Keys work as expected
// Author:      pachan
//
namespace WindowsFormsHostTests
{

public class AccessKeys : WPFReflectBase {

    #region TestVariables

    private static MethodInfo[] mi;

    private delegate void myEventHandler(object sender);
   
    private UIObject uiApp;
    private int ScenarioIndex = 0;

    //private enum EventState { Off = 0, Started, Ended };
    //private EventState state = EventState.Off;

    public SWI.RoutedCommand CopyCommand;
    public SWI.RoutedCommand PasteCommand;
    public SWI.RoutedCommand PasteFnCommand;

    private SWC.StackPanel av_StackPanel;
    private SWC.Label av_Label;
    private SWC.TextBox av_TextBox;
    private SWC.CheckBox av_CheckBox;
    private SWC.Button av_Button;
    private SWC.CheckBox av_CheckBoxSameAccessKey;
    private SWC.Menu av_Menu;
    private SWC.MenuItem av_MenuItem;
    private SWC.MenuItem av_SubMenuItem1;
    private SWC.MenuItem av_SubMenuItem2;
    private SWC.MenuItem av_SubMenuItem3;

    private WindowsFormsHost wfh;
    private SWF.FlowLayoutPanel wf_FlowLayoutPanel;
    private SWF.CheckBox wf_CheckBox;
    private SWF.Label wf_Label;
    private SWF.TextBox wf_TextBox;
    private SWF.CheckBox wf_CheckBoxSameAccessKey;
    private SWF.MenuStrip wf_MenuStrip;
    private SWF.ToolStripMenuItem wf_StripMenuItem;
    private SWF.ToolStripMenuItem wf_StripMenuItemCopy;
    private SWF.ToolStripMenuItem wf_StripMenuItemPaste;
    private SWF.ToolStripMenuItem wf_StripMenuItemPasteFn;
    private SWF.HelpProvider wf_HelpProvider;

    private WindowsFormsHost wfh2;
    private SWF.FlowLayoutPanel wf_FlowLayoutPanel2;
    private SWF.CheckBox wf_CheckBox2;
    private SWF.Label wf_Label2;
    private SWF.TextBox wf_TextBox2;

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
        ScenarioIndex = Convert.ToInt32(scenario.Name.Substring(8)) - 1;
        return base.BeforeScenario(p, scenario);
    }

    protected override void InitTest(TParams p) 
    {
        this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        mi = GetAllScenarios(this);
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
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        uiApp.SetFocus();

        // Alt-A to AV CheckBox
        p.log.WriteLine("Setting focus to AV CheckBox");
        uiApp.SendKeys("%A");
        WPFMiscUtils.IncCounters(sr, true, HasFocus(AVCheckBoxName), "AV CheckBox not getting focus", p.log);

        // Alt-Z to WF CheckBox
        p.log.WriteLine("Setting focus to WF CheckBox");
        uiApp.SendKeys("%Z");
        WPFMiscUtils.IncCounters(sr, true, HasFocus(WFCheckBoxName), "WF CheckBox not getting focus", p.log);

        // Alt-B to AV TextBox
        p.log.WriteLine("Setting focus to AV TextBox");
        uiApp.SendKeys("%B");
        WPFMiscUtils.IncCounters(sr, true, HasFocus(AVTextBoxName), "AV TextBox not getting focus", p.log);

        // Alt-Y to WF TextBox
        p.log.WriteLine("Setting focus to WF TextBox");
        // Known bug !!!
//        p.log.LogKnownBug(BugDb.WindowsOSBugs, 1534333, "require to press the access key twice to get into a WinForm TextBox within WFH");
//        bug has been closed as WinForm bug -- for now, we need to do send key twice
        uiApp.SendKeys("%Y");
        uiApp.SendKeys("%Y");
        WPFMiscUtils.IncCounters(sr, true, HasFocus(WFTextBoxName), "WF CheckBox not getting focus", p.log);

        return sr;
    }

    [Scenario("Navigate between an WF control in one WFH and another WF control in another WFH via accessor key")]
    public ScenarioResult Scenario2(TParams p)
    {
         ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        uiApp.SetFocus();

        // Alt-Z to WF CheckBox in WFH1
        p.log.WriteLine("Setting focus to WF CheckBox in WFH1");
        uiApp.SendKeys("%Z");
        WPFMiscUtils.IncCounters(sr, true, HasFocus(WFCheckBoxName), "WF CheckBox in WFH1 not getting focus", p.log);

        // Alt-W to WF TextBox in WFH2
        p.log.WriteLine("Setting focus to WF TextBox in WFH2");
        // Known bug !!!
//        p.log.LogKnownBug(BugDb.WindowsOSBugs, 1534333, "require to press the access key twice to get into a WinForm TextBox within WFH");
//        bug has been closed as WinForm bug -- for now, we need to do send key twice
        uiApp.SendKeys("%W");
        uiApp.SendKeys("%W");
        WPFMiscUtils.IncCounters(sr, true, HasFocus(WFTextBoxName2), "WF CheckBox in WFH2 not getting focus", p.log);

        // Alt-Y to WF TextBox in WFH1
        p.log.WriteLine("Setting focus to WF TextBox in WFH1");
        // Known bug !!!
//        p.log.LogKnownBug(BugDb.WindowsOSBugs, 1534333, "require to press the access key twice to get into a WinForm TextBox within WFH");
//        bug has been closed as WinForm bug -- for now, we need to do send key twice
        uiApp.SendKeys("%Y");
        uiApp.SendKeys("%Y");
        WPFMiscUtils.IncCounters(sr, true, HasFocus(WFTextBoxName), "WF CheckBox in WFH1 not getting focus", p.log);

        // Alt-X to WF CheckBox in WFH2
        p.log.WriteLine("Setting focus to WF CheckBox in WFH2");
        uiApp.SendKeys("%X");
        WPFMiscUtils.IncCounters(sr, true, HasFocus(WFCheckBoxName2), "WF CheckBox in WFH2 not getting focus", p.log);

        return sr;
    }

    [Scenario("Navigate between an AV control and WF control that have the same accessor key")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        uiApp.SetFocus(); 

        // bug 1534371 has been filed but being resolved as spec issue
        // need to set the focus to a AV control in order to make it behave as expected (WF control get the foucs)
        // if the focus is on the AV Main windows (with upApp.SetFocus()), the AV control will get the focus on this test
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(AVButtonName));
        ctrl.SetFocus();

        // Alt-S to AV/WF CheckBox with the same AccessKey
        p.log.WriteLine("Setting focus to WF/AV CheckBox -- WF CheckBox should get the focus");
        uiApp.SendKeys("%S");
        WPFMiscUtils.IncCounters(sr, true, HasFocus(WFCheckBoxSameAccessKeyName), "WF CheckBox not getting focus", p.log);
        WPFMiscUtils.IncCounters(sr, false, HasFocus(AVCheckBoxSameAccessKeyName), "AV CheckBox is getting focus", p.log);

        return sr;
    }

    [Scenario("Verify CTRL+T fires menu item as expected (gotta hook up a WF menu strip item with CTRL+T first).")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        uiApp.SetFocus();

        // Make a copy of the WF TextBox Text by using the Copy and Paste Menu hooks
        UIObject uiWFTextBox = uiApp.Descendants.Find(UICondition.CreateFromId(WFTextBoxName));
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
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        uiApp.SetFocus();

        // Make a copy of the AV TextBox Text by using the Copy and Paste Menu hooks
        UIObject uiAVTextBox = uiApp.Descendants.Find(UICondition.CreateFromId(AVTextBoxName));
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
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        uiApp.SetFocus(); 

        // Test for the WFH
        // F2 as the Paste operation
        // Make a copy of the WF TextBox Text by using the Copy and Paste Menu hooks
        UIObject uiWFTextBox = uiApp.Descendants.Find(UICondition.CreateFromId(WFTextBoxName));
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
        uiApp.SetFocus();
        UIObject uiAVTextBox = uiApp.Descendants.Find(UICondition.CreateFromId(AVTextBoxName));
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
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        uiApp.SetFocus();

        // Set the focus to WF TextBox control
        UIObject uiWFTextBox = uiApp.Descendants.Find(UICondition.CreateFromId(WFTextBoxName));
        uiWFTextBox.SetFocus();
        // press the F1 key
        uiWFTextBox.SendKeys("{F1}");
        WPFReflectBase.DoEvents();
        System.Threading.Thread.Sleep(200);

        bool bHelpDisplayed;
        try
        {
            Utilities.SleepDoEvents(40);
            UIObject uiHelpWindow = uiApp.Descendants.Find(UICondition.CreateFromName("AccessKeys Help"));
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
        av_StackPanel = new SWC.StackPanel();
        av_CheckBox = new SWC.CheckBox();
        av_Button = new SWC.Button();
        av_TextBox = new SWC.TextBox();
        av_Label = new SWC.Label();
        av_CheckBoxSameAccessKey = new SWC.CheckBox();

        av_Button.Content = av_Button.Name = AVButtonName;
        av_CheckBox.Content = av_CheckBox.Name = AVCheckBoxName;
        av_CheckBoxSameAccessKey.Content = av_CheckBoxSameAccessKey.Name = AVCheckBoxSameAccessKeyName;
        av_TextBox.Text = av_TextBox.Name = AVTextBoxName;
        av_Label.Content = av_Label.Name = AVLabelName;
        av_Label.Target = av_TextBox;
        #endregion

        #region SetupAvMenu
        // Setup AV Menu 
        // Ctrl-T ==> Copy
        // Ctrl-U ==> Paste
        av_Menu = new System.Windows.Controls.Menu();
        av_MenuItem = new System.Windows.Controls.MenuItem();
        av_SubMenuItem1 = new System.Windows.Controls.MenuItem();
        av_SubMenuItem2 = new System.Windows.Controls.MenuItem();
        av_SubMenuItem3 = new System.Windows.Controls.MenuItem();

        av_Menu.Items.Add(av_MenuItem);
        av_MenuItem.Items.Add(av_SubMenuItem1);
        av_MenuItem.Items.Add(av_SubMenuItem2);
        av_MenuItem.Items.Add(av_SubMenuItem3);

        av_Menu.Name = AVMenuName;

        av_MenuItem.Name = AVMenuItemName;
        av_MenuItem.Header = AVMenuItemName;

        av_SubMenuItem1.Name = AVSubMenuItem1Name;
        av_SubMenuItem1.Header = AVSubMenuItem1Name;
        av_SubMenuItem1.Command = SWI.ApplicationCommands.Copy;
        av_SubMenuItem1.InputGestureText = "Ctrl+T";

        av_SubMenuItem2.Name = AVSubMenuItem2Name;
        av_SubMenuItem2.Header = AVSubMenuItem2Name;
        av_SubMenuItem2.Command = SWI.ApplicationCommands.Paste;
        av_SubMenuItem2.InputGestureText = "Ctrl+U";

        av_SubMenuItem3.Name = AVSubMenuItem3Name;
        av_SubMenuItem3.Header = AVSubMenuItem3Name;
        av_SubMenuItem3.Command = SWI.ApplicationCommands.Paste;
        av_SubMenuItem3.InputGestureText = "F3";

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
        wfh = new WindowsFormsHost();
        wf_FlowLayoutPanel = new FlowLayoutPanel();
        wf_CheckBox = new CheckBox();
        wf_TextBox = new TextBox();
        wf_Label = new Label();
        wf_CheckBoxSameAccessKey = new CheckBox();
        wf_HelpProvider = new HelpProvider();

        #region SetupWFMenu
        wf_MenuStrip = new MenuStrip();
        wf_StripMenuItem = new ToolStripMenuItem();
        wf_StripMenuItemCopy = new ToolStripMenuItem();
        wf_StripMenuItemPaste = new ToolStripMenuItem();
        wf_StripMenuItemPasteFn = new ToolStripMenuItem();
        
        wf_MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { wf_StripMenuItem });

        wf_MenuStrip.Location = new System.Drawing.Point(0, 0);
        wf_MenuStrip.Text = wf_MenuStrip.Name = WFMenuStripName;
        wf_MenuStrip.Size = new System.Drawing.Size(442, 24);

        wf_StripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            wf_StripMenuItemCopy,
            wf_StripMenuItemPaste,
            wf_StripMenuItemPasteFn});
        wf_StripMenuItem.Text = wf_StripMenuItem.Name = WFStripMenuItemName;
        wf_StripMenuItem.Size = new System.Drawing.Size(35, 20);
        // 
        wf_StripMenuItemCopy.Text = wf_StripMenuItemCopy.Name = WFStripMenuItemCopyName;
        wf_StripMenuItemCopy.ShortcutKeys = ((SWF.Keys)((SWF.Keys.Control | SWF.Keys.M)));
        wf_StripMenuItemCopy.Size = new System.Drawing.Size(177, 22);
        wf_StripMenuItemCopy.Click += new EventHandler(wf_StripMenuItemCopy_Click);
        // 
        wf_StripMenuItemPaste.Text = wf_StripMenuItemPaste.Name = WFStripMenuItemPasteName;
        wf_StripMenuItemPaste.ShortcutKeys = ((SWF.Keys)((SWF.Keys.Control | SWF.Keys.N)));
        wf_StripMenuItemPaste.Size = new System.Drawing.Size(177, 22);
        wf_StripMenuItemPaste.Click += new EventHandler(wf_StripMenuItemPaste_Click);
        // 
        wf_StripMenuItemPasteFn.Text = wf_StripMenuItemPasteFn.Name = WFStripMenuItemPasteFnName;
        wf_StripMenuItemPasteFn.ShortcutKeys = SWF.Keys.F2;
        wf_StripMenuItemPasteFn.Size = new System.Drawing.Size(177, 22);
        wf_StripMenuItemPasteFn.Click += new EventHandler(wf_StripMenuItemPaste_Click);
        // 

        // test.htm file need to be in the School subdirectory
        wf_HelpProvider.HelpNamespace = @"AccessKeys.chm";
        wf_HelpProvider.SetShowHelp(wf_TextBox, true);
        #endregion

        wfh.Name = WFHostName;
        wf_FlowLayoutPanel.Name = WFFlowLayoutPanelName;
        wf_CheckBox.Text = wf_CheckBox.Name = WFCheckBoxName;
        wf_CheckBoxSameAccessKey.Text = wf_CheckBoxSameAccessKey.Name = WFCheckBoxSameAccessKeyName;
        wf_TextBox.Text = wf_TextBox.Name = WFTextBoxName;
        wf_TextBox.Width = 300;
        wf_Label.Text = wf_Label.Name = WFLabelName;
        wf_Label.UseMnemonic = true;
        wf_CheckBox.TabIndex = 1;
        wf_Label.TabIndex = 2;
        wf_TextBox.TabIndex = 3;
        wf_TextBox.Width = 300;
        wf_CheckBox.AutoSize = true;
        wf_Label.AutoSize = true;
        wf_CheckBoxSameAccessKey.AutoSize = true;

        wf_FlowLayoutPanel.Controls.Add(wf_MenuStrip);
        wf_FlowLayoutPanel.Controls.Add(wf_CheckBoxSameAccessKey);
        wf_FlowLayoutPanel.Controls.Add(wf_CheckBox);
        wf_FlowLayoutPanel.Controls.Add(wf_Label);
        wf_FlowLayoutPanel.Controls.Add(wf_TextBox);

        wf_FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
        wf_FlowLayoutPanel.AutoSize = true;
        wfh.Child = wf_FlowLayoutPanel;
        #endregion

        #region SetupWFH2
        // Second Set of WF Controls
        wfh2 = new WindowsFormsHost();
        wf_FlowLayoutPanel2 = new FlowLayoutPanel();
        wf_CheckBox2 = new CheckBox();
        wf_TextBox2 = new TextBox();
        wf_Label2 = new Label();

        wfh2.Name = WFHostName;
        wf_FlowLayoutPanel2.Name = WFFlowLayoutPanelName2;
        wf_CheckBox2.Text = wf_CheckBox2.Name = WFCheckBoxName2;
        wf_TextBox2.Text = wf_TextBox2.Name = WFTextBoxName2;
        wf_Label2.Text = wf_Label2.Name = WFLabelName2;
        wf_Label2.UseMnemonic = true;
        wf_CheckBox2.TabIndex = 4;
        wf_Label2.TabIndex = 5;
        wf_TextBox2.TabIndex = 6;
        wf_TextBox2.Width = 300;
        wf_CheckBox2.AutoSize = true;
        wf_Label2.AutoSize = true;

        wf_FlowLayoutPanel2.Controls.Add(wf_CheckBox2);
        wf_FlowLayoutPanel2.Controls.Add(wf_Label2);
        wf_FlowLayoutPanel2.Controls.Add(wf_TextBox2);

        wf_FlowLayoutPanel2.FlowDirection = SWF.FlowDirection.TopDown;
        wfh2.Child = wf_FlowLayoutPanel2;
        #endregion
        
        av_StackPanel.Children.Add(av_Menu);
        av_StackPanel.Children.Add(av_Button);
        av_StackPanel.Children.Add(av_CheckBoxSameAccessKey);
        av_StackPanel.Children.Add(av_CheckBox);
        av_StackPanel.Children.Add(av_Label);
        av_StackPanel.Children.Add(av_TextBox);
        av_StackPanel.Children.Add(wfh);
        av_StackPanel.Children.Add(wfh2);
        this.Content = av_StackPanel;

        p.log.WriteLine("TestSetup -- End ");
    }

    void wf_StripMenuItemCopy_Click(object sender, EventArgs e)
    {
        if (wf_TextBox.SelectionLength > 0)
            // Copy the selected text to the Clipboard.
            wf_TextBox.Copy();
    }

    void wf_StripMenuItemPaste_Click(object sender, EventArgs e)
    {
        // Determine if there is any text in the Clipboard to paste into the text box.
        if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
        {
            wf_TextBox.Paste();
        }
    }

  
    private bool HasFocus(string ctrlName)
    {
        // find control by name
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(ctrlName));
        MitaControl.Window wnd = new MitaControl.Window(ctrl);
        return wnd.HasKeyboardFocus;
    }

    private string GetTextboxText(string ctrlName)
    {
        string text = String.Empty;

        // find control by name
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(ctrlName));
        
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