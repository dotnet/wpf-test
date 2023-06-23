using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Reflection;
using System.Threading;

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
using System.Windows.Forms;

//
// Testcase:    ClientConfig
// Description: Just need to verify that a WF control hosted in WFH can still work with the conf
// Author:      pachan
//
namespace MyAppProperties
{

    using System;

    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {

        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(ConstString.SomeAppSettingString)]
        public string MyAppSetting
        {
            get
            {
                return ((string)(this["MyAppSetting"]));
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(ConstString.SomeUserSettingString)]
        public string MyUserSetting
        {
            get
            {
                return ((string)(this["MyUserSetting"]));
            }
            set
            {
                this["MyUserSetting"] = value;
            }
        }
    }
}

public struct ConstString
{
    public const string SomeAppSettingString = "This is some app setting";
    public const string SomeUserSettingString = "This is some user setting";
    public const string SomeNewUserSettingString = "This is new user setting";
}

namespace WindowsFormsHostTests
{

public class ClientConfig : WPFReflectBase 
{
    #region TestVariables
    private delegate void myEventHandler(object sender);
    private bool debug = false;         // set this true for TC debugging
    private TParams _tp;
    private UIObject uiApp;
    private string PropVal;
    private string WFFormKey;

    private SWC.StackPanel AVStackPanel;
    private SWC.Label AVLabel;

    private WindowsFormsHost wfh;
    private SWF.Button WFAppButton;
    private SWF.Button WFReadUserButton;
    private SWF.Button WFWriteUserButton;

    private SWF.FlowLayoutPanel WFFlowLayoutPanel;

    private SWF.Form WFForm;
    private SWF.ToolStripContainer WFToolStripContainer1;
    private SWF.ToolStrip WFToolStrip1;
    private SWF.ToolStripLabel WFToolStripLabel1;

    private const string WFFormName = "WFForm";
    private const string WFToolStripContainer1Name = "WFToolStripContainer1";
    private const string WFToolStrip1Name = "WFToolStrip1";
    private const string WFToolStripLabel1Name = "WFToolStripLabel1";


    private const string WindowTitleName = "ClientConfig";
    private const string AVStackPanelName = "AVStackPanel";
    private const string AVLabelName = "AVLabel";
   
    private const string WFHName = "WFH";
    private const string WFFlowLayoutPanelName = "WFFlowLayoutPanel";
    private const string WFAppButtonName = "WFAppButton";
    private const string WFReadUserButtonName = "WFReadUserButton";
    private const string WFWriteUserButtonName = "WFWriteUserButton";

    #endregion

    #region Testcase setup

    
    public ClientConfig(string[] args) : base(args) { }


    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        MyAppProperties.Settings.Default.MyUserSetting = ConstString.SomeUserSettingString;
        return base.BeforeScenario(p, scenario);
    }

    protected override void InitTest(TParams p) 
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        _tp = p;
        this.UseMITA = true;
        WFFormKey = p.ru.GetString(10);
        TestSetup();
        base.InitTest(p);
        this.Top = 0;
        this.Left = 0;
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Verify a WF Control in a WFH can read an app setting")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        PropVal = String.Empty;
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WFAppButtonName));
        ctrl.Click();
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, ConstString.SomeAppSettingString,  PropVal, "not able to get the App setting", p.log);
        return sr;
    } 

    [Scenario("Verify a WF Control in a WFH can read a user setting")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        PropVal = String.Empty;
        Utilities.SleepDoEvents(10);
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(WFReadUserButtonName));
        ctrl.Click();
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, ConstString.SomeUserSettingString, PropVal, "not able to get the User setting", p.log);
        return sr;
    } 

    [Scenario("Verify a WF Control in a WFH can write a user setting")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        PropVal = String.Empty;
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject writeCtrl = uiApp.Descendants.Find(UICondition.CreateFromId(WFWriteUserButtonName));
        UIObject readCtrl = uiApp.Descendants.Find(UICondition.CreateFromId(WFReadUserButtonName));
        writeCtrl.Click();
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, ConstString.SomeNewUserSettingString, PropVal, "not able to get the User setting", p.log);

        readCtrl.Click();
        Utilities.SleepDoEvents(10);
        WPFMiscUtils.IncCounters(sr, ConstString.SomeNewUserSettingString, PropVal, "not able to set the User setting", p.log);
        return sr;
    } 

    [Scenario("Verify that a WF ToolStrip control can sucessfully save/read config settings.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        SetupWF();
        WFForm.Show();
        Utilities.SleepDoEvents(10);

        using (PointerInput.Activate(Mouse.Instance))
        {
            System.Drawing.Point pt = WFToolStrip1.PointToScreen(new System.Drawing.Point(10, 4));
            Mouse.Instance.Move(pt);
            PointerInput.Press(PointerButtons.Primary);
            pt.X += 100;
            Mouse.Instance.Move(pt);
            Mouse.Instance.Release(PointerButtons.Primary);
            Utilities.SleepDoEvents(10);
        }
        System.Drawing.Point beforePt = WFToolStrip1.Location;

        WFForm.Close();
        Utilities.SleepDoEvents(10);

        SetupWF();
        WFForm.Show();
        Utilities.SleepDoEvents(10);
        System.Drawing.Point afterPt = WFToolStrip1.Location;
        WFForm.Close();

        WPFMiscUtils.IncCounters(sr, beforePt.X, afterPt.X, "not able to save the ToolStrip setting", p.log);
        WPFMiscUtils.IncCounters(sr, beforePt.Y, afterPt.Y, "not able to save the ToolStrip setting", p.log);
        
        return sr;
    }   

    #endregion


    private void TestSetup()
    {
        myWriteLine("TestSetup -- Start ");

        #region SetupAVControl
        AVStackPanel = new SWC.StackPanel();
        AVLabel = new SWC.Label();
        AVLabel.Width = 400;
        AVLabel.Height = 50;
        AVStackPanel.Name = AVStackPanelName;
        AVLabel.Name = AVLabelName;
            AVLabel.Content = AVLabelName;
        #endregion

        #region SetupWFControl
        wfh = new WindowsFormsHost();
        WFFlowLayoutPanel = new SWF.FlowLayoutPanel();
        WFAppButton = new SWF.Button();
        WFReadUserButton = new SWF.Button();
        WFWriteUserButton = new SWF.Button();
        WFFlowLayoutPanel.Controls.Add(WFAppButton);
        WFFlowLayoutPanel.Controls.Add(WFReadUserButton);
        WFFlowLayoutPanel.Controls.Add(WFWriteUserButton);
        WFFlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown; 
        wfh.Child = WFFlowLayoutPanel;
        wfh.Name = WFHName;
        WFAppButton.Text = WFAppButton.Name = WFAppButtonName;
        WFReadUserButton.Text = WFReadUserButton.Name = WFReadUserButtonName;
        WFWriteUserButton.Text = WFWriteUserButton.Name = WFWriteUserButtonName;
        WFAppButton.Click += new EventHandler(WFAppButton_Click);
        WFReadUserButton.Click += new EventHandler(WFReadUserButton_Click);
        WFWriteUserButton.Click += new EventHandler(WFWriteUserButton_Click);
        #endregion

        #region LayoutWindow
        this.Title = WindowTitleName;
        AVStackPanel.Children.Add(wfh);
        AVStackPanel.Children.Add(AVLabel);
        this.Content = AVStackPanel;
        #endregion

        myWriteLine("TestSetup -- End ");
    }

    private void SetupWF()
    {
        WFForm = new SWF.Form();
        WFToolStripContainer1 = new SWF.ToolStripContainer();
        WFToolStrip1 = new SWF.ToolStrip();
        WFToolStripLabel1 = new SWF.ToolStripLabel();


        WFToolStripContainer1.ContentPanel.Size = new System.Drawing.Size(213, 242);
        WFToolStripContainer1.Dock = SWF.DockStyle.Fill;
        // 
        // ToolStripContainer1.LeftToolStripPanel
        // 
        WFToolStripContainer1.LeftToolStripPanel.Controls.Add(WFToolStrip1);
        WFToolStripContainer1.Location = new System.Drawing.Point(0, 0);
        WFToolStripContainer1.Text = WFToolStripContainer1.Name = WFToolStripContainer1Name;
        WFToolStripContainer1.Size = new System.Drawing.Size(292, 266);
        WFToolStripContainer1.TabIndex = 0;
        // 
        // ToolStrip1
        // 
        WFToolStrip1.Dock = SWF.DockStyle.None;
        WFToolStrip1.Items.AddRange(new SWF.ToolStripItem[] {
        WFToolStripLabel1});
        WFToolStrip1.Location = new System.Drawing.Point(0, 3);
        WFToolStrip1.Name = WFToolStrip1Name;
        WFToolStrip1.Size = new System.Drawing.Size(79, 73);
        WFToolStrip1.TabIndex = 0;
        // 
        // ToolStripLabel1
        // 
        WFToolStripLabel1.Text = WFToolStripLabel1.Name = WFToolStripLabel1Name;
        WFToolStripLabel1.Size = new System.Drawing.Size(77, 13);

        WFForm.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        WFForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        WFForm.ClientSize = new System.Drawing.Size(292, 266);
        WFForm.Controls.Add(WFToolStripContainer1);
        WFForm.Text = WFForm.Name = WFFormName;
        WFForm.FormClosing += new FormClosingEventHandler(WFForm_FormClosing);
        WFForm.Load += new EventHandler(WFForm_Load);
    }

    void WFForm_Load(object sender, EventArgs e)
    {
        SWF.ToolStripManager.LoadSettings(WFForm, WFFormKey);
    }

    void WFForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        SWF.ToolStripManager.SaveSettings(WFForm, WFFormKey);
    }

    void WFAppButton_Click(object sender, EventArgs e)
    {
        AVLabel.Content = PropVal = MyAppProperties.Settings.Default.MyAppSetting;
    }

    void WFWriteUserButton_Click(object sender, EventArgs e)
    {
        MyAppProperties.Settings.Default.MyUserSetting = ConstString.SomeNewUserSettingString;
        MyAppProperties.Settings.Default.Save();
        AVLabel.Content = PropVal = MyAppProperties.Settings.Default.MyUserSetting;
    }

    void WFReadUserButton_Click(object sender, EventArgs e)
    {
        AVLabel.Content = PropVal = MyAppProperties.Settings.Default.MyUserSetting;
    }

    void myWriteLine(string s)
    {
        if (debug)
        {
            myWriteLine(s);
        }
    }
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify a WF Control in a WFH can read an app setting

//@ Verify a WF Control in a WFH can read a user setting

//@ Verify a WF Control in a WFH can write a user setting

//@ Verify that a WF ToolStrip control can sucessfully save/read config settings.

