// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
//
namespace MyAppProperties
{
    using System;

    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {
        private static Settings s_defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default
        {
            get
            {
                return s_defaultInstance;
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
        private bool _debug = false;         // set this true for TC debugging
        private TParams _tp;
        private UIObject _uiApp;
        private string _propVal;
        private string _WFFormKey;

        private SWC.StackPanel _AVStackPanel;
        private SWC.Label _AVLabel;

        private WindowsFormsHost _wfh;
        private SWF.Button _WFAppButton;
        private SWF.Button _WFReadUserButton;
        private SWF.Button _WFWriteUserButton;

        private SWF.FlowLayoutPanel _WFFlowLayoutPanel;

        private SWF.Form _WFForm;
        private SWF.ToolStripContainer _WFToolStripContainer1;
        private SWF.ToolStrip _WFToolStrip1;
        private SWF.ToolStripLabel _WFToolStripLabel1;

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
            _WFFormKey = p.ru.GetString(10);
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
            _propVal = String.Empty;
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WFAppButtonName));
            ctrl.Click();
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, ConstString.SomeAppSettingString,  _propVal, "not able to get the App setting", p.log);
            return sr;
        } 

        [Scenario("Verify a WF Control in a WFH can read a user setting")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _propVal = String.Empty;
            Utilities.SleepDoEvents(10);
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WFReadUserButtonName));
            ctrl.Click();
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, ConstString.SomeUserSettingString, _propVal, "not able to get the User setting", p.log);
            return sr;
        } 

        [Scenario("Verify a WF Control in a WFH can write a user setting")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _propVal = String.Empty;
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject writeCtrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WFWriteUserButtonName));
            UIObject readCtrl = _uiApp.Descendants.Find(UICondition.CreateFromId(WFReadUserButtonName));
            writeCtrl.Click();
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, ConstString.SomeNewUserSettingString, _propVal, "not able to get the User setting", p.log);

            readCtrl.Click();
            Utilities.SleepDoEvents(10);
            WPFMiscUtils.IncCounters(sr, ConstString.SomeNewUserSettingString, _propVal, "not able to set the User setting", p.log);
            return sr;
        } 

        [Scenario("Verify that a WF ToolStrip control can sucessfully save/read config settings.")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetupWF();
            _WFForm.Show();
            Utilities.SleepDoEvents(10);

            using (PointerInput.Activate(Mouse.Instance))
            {
                System.Drawing.Point pt = _WFToolStrip1.PointToScreen(new System.Drawing.Point(10, 4));
                Mouse.Instance.Move(pt);
                PointerInput.Press(PointerButtons.Primary);
                pt.X += 100;
                Mouse.Instance.Move(pt);
                Mouse.Instance.Release(PointerButtons.Primary);
                Utilities.SleepDoEvents(10);
            }
            System.Drawing.Point beforePt = _WFToolStrip1.Location;

            _WFForm.Close();
            Utilities.SleepDoEvents(10);

            SetupWF();
            _WFForm.Show();
            Utilities.SleepDoEvents(10);
            System.Drawing.Point afterPt = _WFToolStrip1.Location;
            _WFForm.Close();

            WPFMiscUtils.IncCounters(sr, beforePt.X, afterPt.X, "not able to save the ToolStrip setting", p.log);
            WPFMiscUtils.IncCounters(sr, beforePt.Y, afterPt.Y, "not able to save the ToolStrip setting", p.log);
            
            return sr;
        }   

        #endregion


        private void TestSetup()
        {
            myWriteLine("TestSetup -- Start ");

            #region SetupAVControl
            _AVStackPanel = new SWC.StackPanel();
            _AVLabel = new SWC.Label();
            _AVLabel.Width = 400;
            _AVLabel.Height = 50;
            _AVStackPanel.Name = AVStackPanelName;
            _AVLabel.Name = AVLabelName;
                _AVLabel.Content = AVLabelName;
            #endregion

            #region SetupWFControl
            _wfh = new WindowsFormsHost();
            _WFFlowLayoutPanel = new SWF.FlowLayoutPanel();
            _WFAppButton = new SWF.Button();
            _WFReadUserButton = new SWF.Button();
            _WFWriteUserButton = new SWF.Button();
            _WFFlowLayoutPanel.Controls.Add(_WFAppButton);
            _WFFlowLayoutPanel.Controls.Add(_WFReadUserButton);
            _WFFlowLayoutPanel.Controls.Add(_WFWriteUserButton);
            _WFFlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown; 
            _wfh.Child = _WFFlowLayoutPanel;
            _wfh.Name = WFHName;
            _WFAppButton.Text = _WFAppButton.Name = WFAppButtonName;
            _WFReadUserButton.Text = _WFReadUserButton.Name = WFReadUserButtonName;
            _WFWriteUserButton.Text = _WFWriteUserButton.Name = WFWriteUserButtonName;
            _WFAppButton.Click += new EventHandler(WFAppButton_Click);
            _WFReadUserButton.Click += new EventHandler(WFReadUserButton_Click);
            _WFWriteUserButton.Click += new EventHandler(WFWriteUserButton_Click);
            #endregion

            #region LayoutWindow
            this.Title = WindowTitleName;
            _AVStackPanel.Children.Add(_wfh);
            _AVStackPanel.Children.Add(_AVLabel);
            this.Content = _AVStackPanel;
            #endregion

            myWriteLine("TestSetup -- End ");
        }

        private void SetupWF()
        {
            _WFForm = new SWF.Form();
            _WFToolStripContainer1 = new SWF.ToolStripContainer();
            _WFToolStrip1 = new SWF.ToolStrip();
            _WFToolStripLabel1 = new SWF.ToolStripLabel();


            _WFToolStripContainer1.ContentPanel.Size = new System.Drawing.Size(213, 242);
            _WFToolStripContainer1.Dock = SWF.DockStyle.Fill;
            // 
            // ToolStripContainer1.LeftToolStripPanel
            // 
            _WFToolStripContainer1.LeftToolStripPanel.Controls.Add(_WFToolStrip1);
            _WFToolStripContainer1.Location = new System.Drawing.Point(0, 0);
            _WFToolStripContainer1.Text = _WFToolStripContainer1.Name = WFToolStripContainer1Name;
            _WFToolStripContainer1.Size = new System.Drawing.Size(292, 266);
            _WFToolStripContainer1.TabIndex = 0;
            // 
            // ToolStrip1
            // 
            _WFToolStrip1.Dock = SWF.DockStyle.None;
            _WFToolStrip1.Items.AddRange(new SWF.ToolStripItem[] {
            _WFToolStripLabel1});
            _WFToolStrip1.Location = new System.Drawing.Point(0, 3);
            _WFToolStrip1.Name = WFToolStrip1Name;
            _WFToolStrip1.Size = new System.Drawing.Size(79, 73);
            _WFToolStrip1.TabIndex = 0;
            // 
            // ToolStripLabel1
            // 
            _WFToolStripLabel1.Text = _WFToolStripLabel1.Name = WFToolStripLabel1Name;
            _WFToolStripLabel1.Size = new System.Drawing.Size(77, 13);

            _WFForm.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            _WFForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            _WFForm.ClientSize = new System.Drawing.Size(292, 266);
            _WFForm.Controls.Add(_WFToolStripContainer1);
            _WFForm.Text = _WFForm.Name = WFFormName;
            _WFForm.FormClosing += new FormClosingEventHandler(WFForm_FormClosing);
            _WFForm.Load += new EventHandler(WFForm_Load);
        }

        void WFForm_Load(object sender, EventArgs e)
        {
            SWF.ToolStripManager.LoadSettings(_WFForm, _WFFormKey);
        }

        void WFForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SWF.ToolStripManager.SaveSettings(_WFForm, _WFFormKey);
        }

        void WFAppButton_Click(object sender, EventArgs e)
        {
            _AVLabel.Content = _propVal = MyAppProperties.Settings.Default.MyAppSetting;
        }

        void WFWriteUserButton_Click(object sender, EventArgs e)
        {
            MyAppProperties.Settings.Default.MyUserSetting = ConstString.SomeNewUserSettingString;
            MyAppProperties.Settings.Default.Save();
            _AVLabel.Content = _propVal = MyAppProperties.Settings.Default.MyUserSetting;
        }

        void WFReadUserButton_Click(object sender, EventArgs e)
        {
            _AVLabel.Content = _propVal = MyAppProperties.Settings.Default.MyUserSetting;
        }

        void myWriteLine(string s)
        {
            if (_debug)
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
