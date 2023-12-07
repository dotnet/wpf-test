// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Microsoft.Test.Display;


//
// Testcase:    PaddingsAndMargins
// Description: Verify that setting the padding and margins on a WFH flow to the WF ctrl
//
namespace WindowsFormsHostTests
{
    public class PaddingsAndMargins : WPFReflectBase
    {
        // class vars
        private StackPanel _sp;
        private WindowsFormsHost _wfh1;
        private SWF.Button _wfBtn;
        private WindowsFormsHost _wfh2;
        private SWF.Button _wfBtn2;
        private SD.Rectangle _origRect;
        private SD.Rectangle _origRect2;

        #region Testcase setup
        public PaddingsAndMargins(string[] args) : base(args) { }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            bool b = base.BeforeScenario(p, scenario);

            // debug - run specific scenario !!!
            //if (scenario.Name != "Scenario1") { return false; }

            this.Width = 300;
            this.Height = 300;
            CreateTestControls();
            this.Title = scenario.Name;
            this.Topmost = true;
            this.Topmost = false;

            return b;
        }

        protected override void AfterScenario(TParams p, System.Reflection.MethodInfo scenario, ScenarioResult result)
        {
            //Utilities.ActiveFreeze(currentScenario.Name);
            base.AfterScenario(p, scenario, result);
        }

        private void CreateTestControls()
        {
            // create panel
            _sp = new StackPanel();

            // add button before the host
            Button avBtn1 = new Button();
            avBtn1.Content = "Avalon Button 1";
            _sp.Children.Add(avBtn1);

            // create WF host control, add to panel
            _wfh1 = new WindowsFormsHost();
            _wfh1.Background = System.Windows.Media.Brushes.Yellow;
            _sp.Children.Add(_wfh1);

            // create WF Button
            _wfBtn = new SWF.Button();
            _wfBtn.Text = "Here I am!";
            _wfh1.Child = _wfBtn;

            // add panel to window
            this.Background = System.Windows.Media.Brushes.Beige;
            this.Content = _sp;

            // create second host with WF button
            _wfh2 = new WindowsFormsHost();
            _wfBtn2 = new SWF.Button();
            _wfBtn2.Text = "I can see that.";
            _wfh2.Child = _wfBtn2;
            _wfh2.Background = System.Windows.Media.Brushes.LightSkyBlue;
            _sp.Children.Add(_wfh2);

            Utilities.SleepDoEvents(10);

            // we are now in default state
            // record current size, position
            _origRect = _wfBtn.RectangleToScreen(_wfBtn.ClientRectangle);
            _origRect2 = _wfBtn2.RectangleToScreen(_wfBtn2.ClientRectangle);
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("1) Set the top padding on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial padding properties
            VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

            // "Set the top padding on the WFH"
            _wfh1.Padding = new Thickness(0d, 50d, 0d, 0d);
            Utilities.SleepDoEvents(10);

            // Check padding properties
            VerifyPadding(p, sr, 0d, 50d, 0d, 0d);

            return sr;
        }

        [Scenario("2) Set the bottom padding on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial padding properties
            VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

            // "Set the bottom padding on the WFH"
            _wfh1.Padding = new Thickness(0d, 0d, 0d, 50d);
            Utilities.SleepDoEvents(10);

            // Check padding properties
            VerifyPadding(p, sr, 0d, 0d, 0d, 50d);

            return sr;
        }

        [Scenario("3) Set the left padding on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial padding properties
            VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

            // "Set the left padding on the WFH"
            _wfh1.Padding = new Thickness(50d, 0d, 0d, 0d);
            Utilities.SleepDoEvents(10);

            // Check padding properties
            VerifyPadding(p, sr, 50d, 0d, 0d, 0d);

            return sr;
        }

        [Scenario("4) Set the right padding on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial padding properties
            VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

            // "Set the right padding on the WFH"
            _wfh1.Padding = new Thickness(0d, 0d, 50d, 0d);
            Utilities.SleepDoEvents(10);

            // Check padding properties
            VerifyPadding(p, sr, 0d, 0d, 50d, 0d);

            return sr;
        }

        [Scenario("5) Set the top margin on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial margin properties
            VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

            // "Set the top margin on the WFH"
            _wfh1.Margin = new Thickness(0d, 50d, 0d, 0d);
            Utilities.SleepDoEvents(10);

            // Check margin properties
            VerifyMargin(p, sr, 0d, 50d, 0d, 0d);

            return sr;
        }

        [Scenario("6) Set the bottom margin on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial margin properties
            VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

            // "Set the bottom margin on the WFH"
            _wfh1.Margin = new Thickness(0d, 0d, 0d, 50d);
            Utilities.SleepDoEvents(10);

            // Check margin properties
            VerifyMargin(p, sr, 0d, 0d, 0d, 50d);

            return sr;
        }

        [Scenario("7) Set the left margin on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial margin properties
            VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

            // "Set the left margin on the WFH"
            _wfh1.Margin = new Thickness(50d, 0d, 0d, 0d);
            Utilities.SleepDoEvents(10);

            // Check margin properties
            VerifyMargin(p, sr, 50d, 0d, 0d, 0d);

            return sr;
        }

        [Scenario("8) Set the right margin on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial margin properties
            VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

            // "Set the right margin on the WFH"
            _wfh1.Margin = new Thickness(0d, 0d, 50d, 0d);
            Utilities.SleepDoEvents(10);

            // Check margin properties
            VerifyMargin(p, sr, 0d, 0d, 50d, 0d);

            return sr;
        }

        [Scenario("9) Set the padding (all) on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario9(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial padding properties
            VerifyPadding(p, sr, 0d, 0d, 0d, 0d);

            // "Set the padding on the WFH"
            _wfh1.Padding = new Thickness(5d, 10d, 15d, 20d);
            Utilities.SleepDoEvents(10);

            // Check padding properties
            VerifyPadding(p, sr, 5d, 10d, 15d, 20d);

            // "Set the padding on the WFH"
            _wfh1.Padding = new Thickness(30d);
            Utilities.SleepDoEvents(10);

            // Check padding properties
            VerifyPadding(p, sr, 30d, 30d, 30d, 30d);

            return sr;
        }

        [Scenario("10) Set the margin (all) on the WFH and verify that the WF Control honors the setting")]
        public ScenarioResult Scenario10(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Check initial margin properties
            VerifyMargin(p, sr, 0d, 0d, 0d, 0d);

            // "Set the margin on the WFH"
            _wfh1.Margin = new Thickness(5d, 10d, 15d, 20d);
            Utilities.SleepDoEvents(10);

            // Check margin properties
            VerifyMargin(p, sr, 5d, 10d, 15d, 20d);

            // "Set the margin on the WFH"
            _wfh1.Margin = new Thickness(30d);
            Utilities.SleepDoEvents(10);

            // Check margin properties
            VerifyMargin(p, sr, 30d, 30d, 30d, 30d);

            return sr;
        }

        #region Helper functions

        private void VerifyPadding(TParams p, ScenarioResult sr, double left, double top, double right, double bottom)
        {
            // Not only do we need to verify that the Padding settings in the WFH and the WF control are set
            // properly, we also need to actually "look" at where the control was positioned on the screen.
            // We also need to check the control that was "pushed out of the way" by changes in our main WFH.

            // check WFH
            WPFMiscUtils.IncCounters(sr, left, _wfh1.Padding.Left, "Host Left Padding not correct", p.log);
            WPFMiscUtils.IncCounters(sr, top, _wfh1.Padding.Top, "Host Top Padding not correct", p.log);
            WPFMiscUtils.IncCounters(sr, right, _wfh1.Padding.Right, "Host Right Padding not correct", p.log);
            WPFMiscUtils.IncCounters(sr, bottom, _wfh1.Padding.Bottom, "Host Bottom Padding not correct", p.log);

            // check WF Control (Padding should match what WFH is set to)
            WPFMiscUtils.IncCounters(sr, left, (double)_wfBtn.Padding.Left, "Control Left Padding not correct", p.log);
            WPFMiscUtils.IncCounters(sr, top, (double)_wfBtn.Padding.Top, "Control Top Padding not correct", p.log);
            WPFMiscUtils.IncCounters(sr, right, (double)_wfBtn.Padding.Right, "Control Right Padding not correct", p.log);
            WPFMiscUtils.IncCounters(sr, bottom, (double)_wfBtn.Padding.Bottom, "Control Bottom Padding not correct", p.log);

            // where is our control now?
            SD.Rectangle curRect = _wfBtn.RectangleToScreen(_wfBtn.ClientRectangle);
            //p.log.WriteLine("origRect  = {0}", origRect.ToString());
            //p.log.WriteLine("curRect   = {0}", curRect.ToString());

            // compare current location with where we think it should be
            // (position, width should be unaffected (in stackpanel), height is affected by top/bottom)
            WPFMiscUtils.IncCounters(sr, _origRect.X, curRect.X, "Control X not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect.Y, curRect.Y, "Control Y not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect.Width, curRect.Width, "Control Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect.Height + (int)top + (int)bottom, curRect.Height, "Control Height not correct", p.log);

            // what about the control following it?
            SD.Rectangle curRect2 = _wfBtn2.RectangleToScreen(_wfBtn2.ClientRectangle);
            //p.log.WriteLine("origRect2 = {0}", origRect2.ToString());
            //p.log.WriteLine("curRect2  = {0}", curRect2.ToString());

            // compare current location with where we think it should be
            // (Y position is affected by top and bottom, width and height should not change)
            WPFMiscUtils.IncCounters(sr, _origRect2.X, curRect2.X, "Control2 X not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect2.Y + (int)top + (int)bottom, curRect2.Y, "Control2 Y not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect2.Width, curRect2.Width, "Control2 Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect2.Height, curRect2.Height, "Control2 Height not correct", p.log);
        }

        private void VerifyMargin(TParams p, ScenarioResult sr, double left, double top, double right, double bottom)
        {
            // Not only do we need to verify that the Margin settings in the WFH and the WF control are set
            // properly, we also need to actually "look" at where the control was positioned on the screen.
            // We also need to check the control that was "pushed out of the way" by changes in our main WFH.

            // check WFH
            WPFMiscUtils.IncCounters(sr, left, _wfh1.Margin.Left, "Host Left Margin not correct", p.log);
            WPFMiscUtils.IncCounters(sr, top, _wfh1.Margin.Top, "Host Top Margin not correct", p.log);
            WPFMiscUtils.IncCounters(sr, right, _wfh1.Margin.Right, "Host Right Margin not correct", p.log);
            WPFMiscUtils.IncCounters(sr, bottom, _wfh1.Margin.Bottom, "Host Bottom Margin not correct", p.log);

            // check WF Control
            WPFMiscUtils.IncCounters(sr, 0, _wfBtn.Margin.Left, "Control Left Margin not correct", p.log);
            WPFMiscUtils.IncCounters(sr, 0, _wfBtn.Margin.Top, "Control Top Margin not correct", p.log);
            WPFMiscUtils.IncCounters(sr, 0, _wfBtn.Margin.Right, "Control Right Margin not correct", p.log);
            WPFMiscUtils.IncCounters(sr, 0, _wfBtn.Margin.Bottom, "Control Bottom Margin not correct", p.log);

            // where is our control now?
            SD.Rectangle curRect = _wfBtn.RectangleToScreen(_wfBtn.ClientRectangle);
            //p.log.WriteLine("origRect  = {0}", origRect.ToString());
            //p.log.WriteLine("curRect   = {0}", curRect.ToString());

            // compare current location with where we think it should be
            // (position is affected by left and top, width is affected by left and right, height should not change)
            WPFMiscUtils.IncCounters(sr, _origRect.X + (int)left, curRect.X, "Control X not correct", p.log);
            WPFMiscUtils.IncCounters(sr, (_origRect.Y + Math.Round(Monitor.ConvertLogicalToScreen(Dimension.Height, top))).ToString(), curRect.Y.ToString(), "Control Y not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect.Width - (int)left - (int)right, curRect.Width, "Control Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect.Height, curRect.Height, "Control Height not correct", p.log);

            // what about the control following it?
            SD.Rectangle curRect2 = _wfBtn2.RectangleToScreen(_wfBtn2.ClientRectangle);
            //p.log.WriteLine("origRect2 = {0}", origRect2.ToString());
            //p.log.WriteLine("curRect2  = {0}", curRect2.ToString());

            // compare current location with where we think it should be
            // (Y position is affected by top and bottom, width and height should not change)
            WPFMiscUtils.IncCounters(sr, _origRect2.X, curRect2.X, "Control2 X not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect2.Y + (int)Math.Round(Monitor.ConvertLogicalToScreen(Dimension.Height, top)) + (int)bottom, curRect2.Y, "Control2 Y not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect2.Width, curRect2.Width, "Control2 Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, _origRect2.Height, curRect2.Height, "Control2 Height not correct", p.log);
        }
        #endregion

        #endregion
    }
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Set the top padding on the WFH and verify that the WF Control honors the setting

//@ 2) Set the bottom padding on the WFH and verify that the WF Control honors the setting

//@ 3) Set the left padding on the WFH and verify that the WF Control honors the setting

//@ 4) Set the right padding on the WFH and verify that the WF Control honors the setting

//@ 5) Set the top margin on the WFH and verify that the WF Control honors the setting

//@ 6) Set the bottom margin on the WFH and verify that the WF Control honors the setting

//@ 7) Set the left margin on the WFH and verify that the WF Control honors the setting

//@ 8) Set the right margin on the WFH and verify that the WF Control honors the setting

//@ 9) Set the padding (all) on the WFH and verify that the WF Control honors the setting

//@ 10) Set the margin (all) on the WFH and verify that the WF Control honors the setting

