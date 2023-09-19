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


// Testcase:    WindowsOSBug1607556
// Description: Copy and Paste from AV to WFH RichTextBox and verify font attribute remain the same
namespace WindowsFormsHostTests
{
    public class WindowsOSBug1607556 : WPFReflectBase {

        #region TestVariables

        private delegate void myEventHandler(object sender);
        private UIObject _uiApp;
        private System.Drawing.Font _WFFont;

        private SWC.StackPanel _AVStackPanel;
        private SWC.RichTextBox _AVRichTextBox;

        private WindowsFormsHost _wfh;
        private SWF.RichTextBox _WFRichTextBox;

        private const string WindowTitleName = "WindowsOSBug1607556Test";
        private const string AVStackPanelName = "AVStackPanel";
        private const string AVRichTextBoxName = "AVRichTextBox";

        private const string WFName = "WFH";
        private const string WFRichTextBoxName = "WFRichTextBox";

        #endregion

        #region Testcase setup
        public WindowsOSBug1607556(string[] args) : base(args) { }

        protected override void InitTest(TParams p) 
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            TestSetup(p);
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            this.UseMITA = true;
            base.InitTest(p);
            this.Top = 0;
            this.Left = 0;
        }
        #endregion
        
        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Copy and Paste from AV to WFH RichTextBox and verify font attribute remain the same")]
        public ScenarioResult Scenario1(TParams p) 
        {
            System.Drawing.Font beforeFont;
            System.Drawing.Font afterFont;
            ScenarioResult sr = new ScenarioResult();
            _uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
            UIObject ctrl1 = _uiApp.Descendants.Find(UICondition.CreateFromId(WFRichTextBoxName));
            UIObject ctrl2 = _uiApp.Descendants.Find(UICondition.CreateFromId(AVRichTextBoxName));
            
            p.log.WriteLine("--- Copy from WF RichTextBox to AV RichTextBox ---");
            ctrl1.SendKeys("^{HOME}");
            _WFRichTextBox.Invoke(new myEventHandler(GetFontAttribute), _WFRichTextBox);
            beforeFont = _WFFont;
            ctrl1.SendKeys("^+{END}");
            Utilities.SleepDoEvents(10);
            ctrl1.SendKeys("^c");     // copy
            ctrl2.SendKeys("{END}");
            ctrl2.SendKeys("^v");     // paste
            Utilities.SleepDoEvents(10);

            p.log.WriteLine("--- Copy from AV RichTextBox back to AV RichTextBox ---");
            ctrl2.SendKeys("^{HOME}");
            ctrl2.SendKeys("^+{END}");
            ctrl2.SendKeys("^c");     // copy
            ctrl1.SendKeys("^{HOME}");
            ctrl1.SendKeys("^+{END}");
            ctrl1.SendKeys("^v");     // paste
            ctrl1.SendKeys("^{HOME}");
            Utilities.SleepDoEvents(10);
            _WFRichTextBox.Invoke(new myEventHandler(GetFontAttribute), _WFRichTextBox);
            afterFont = _WFFont;

            WPFMiscUtils.IncCounters(sr, beforeFont, afterFont, "before and after Font attribute are different", p.log);
            if (sr.FailCount > 0)
                p.log.LogKnownBug(BugDb.WindowsOSBugs, 48, "Font attribute is being changed on copying text from AV RichTextBox to WF RichTextBox");

            return sr;
        }

        #endregion

        private void TestSetup(TParams p)
        {
            p.log.WriteLine("TestSetup -- Start ");

            #region SetupAVControl
            _AVStackPanel = new SWC.StackPanel();
            _AVRichTextBox = new SWC.RichTextBox();
            _AVStackPanel.Name = AVStackPanelName;
            _AVRichTextBox.Name = AVRichTextBoxName;
            _AVRichTextBox.Width = 250;
            _AVRichTextBox.Height = 150;
            #endregion

            #region SetupWFControl
            _wfh = new WindowsFormsHost();
            _WFRichTextBox = new SWF.RichTextBox();
            _wfh.Name = WFName;
            _wfh.Width = 250;
            _wfh.Height = 150;
            _WFRichTextBox.Name = WFRichTextBoxName;
            _WFRichTextBox.Font = new System.Drawing.Font("Webdings", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            _wfh.Child = _WFRichTextBox;
            #endregion

            #region LayoutWindow
            this.Title = WindowTitleName;
            _AVStackPanel.Children.Add(_AVRichTextBox);
            _AVStackPanel.Children.Add(_wfh);
            this.Content = _AVStackPanel;
            _WFRichTextBox.Text = "This is a test";
            #endregion

            p.log.WriteLine("TestSetup -- End ");
        }

        private void GetFontAttribute(object sender)
        {
            SWF.RichTextBox rtb = sender as SWF.RichTextBox;
            _WFFont = rtb.SelectionFont;
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Copy and Paste from AV to WFH RichTextBox and verify font attribute remain the same
