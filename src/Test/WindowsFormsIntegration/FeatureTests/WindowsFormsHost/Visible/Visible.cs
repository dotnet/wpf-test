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
using MS.Internal.Mita.Foundation;

//
// Testcase:    Visible
// Description: Verify that the WF's Visible Property and WPF's IsVisible Property work properly
//
namespace WindowsFormsHostTests
{
    public class Visible : WPFReflectBase
    {
        #region Testcase setup
        public Visible(string[] args) : base(args) { }

        // class vars !!!
        private enum ContainerType { DockPanel, Grid, StackPanel, Canvas, WrapPanel};
        private DockPanel _dp;
        private Grid _grid;
        private StackPanel _stack;
        private Canvas _canvas;
        private WrapPanel _wrap;

        private enum TestType { Single, Complex, Container };
        private bool _debug = false;         // set this true for TC debugging

        // host controls
        private WindowsFormsHost _wfh1;

        // WF controls
        private System.Windows.Forms.TextBox _tb1;
        private System.Windows.Forms.Button _btn1;
        private System.Windows.Forms.DataGridView _dgv;
        private System.Windows.Forms.UserControl _userctl;
        private System.Windows.Forms.Panel _panel;

        protected override void InitTest(TParams p)
        {
            // hacks to get window to show up !!!
            this.Topmost = true;
            this.Topmost = false;
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;

            this.Width = 500;
            this.Height = 500;
            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            bool b = base.BeforeScenario(p, scenario);

            // debug - run specific scenario !!!
            //if (scenario.Name != "Scenario3") { return false; }

            this.Title = currentScenario.Name;

            return b;
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("WFH with single control")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            
            // iterate through each container type
            foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
            {
                // debug - only do one !!!
                //if (contType != ContainerType.DockPanel) { continue; }

                TestSetup(p, contType, TestType.Single);
                DoSingleControlTest(p, sr, contType);
            }
            //!!!KeepRunningTests = false;
            return sr;
        }

        [Scenario("WFH with complex control")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // iterate through each container type
            foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
            {
                // debug - only do one !!!
                //if (contType != ContainerType.Canvas) { continue; }

                TestSetup(p, contType, TestType.Complex);
                DoComplexControlTest(p, sr, contType);
            }

            return sr;
        }

        [Scenario("WFH with container control")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // iterate through each container type
            foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
            {
                // debug - only do one !!!
                //if (contType != ContainerType.WrapPanel) { continue; }

                TestSetup(p, contType, TestType.Container);
                DoContainerControlTest(p, sr, contType);
            }

            return sr;
        }

        [Scenario("WF control Visable transitions (true -> true, true->false, false ->true)")]
        public ScenarioResult Scenario4(TParams p)
        {
            // Transition tests are performed within other Scenarios
            return new ScenarioResult(true, "Performed elsewhere", p.log);
        }

        #region Scenario Helper functions

        // Helper routine for Scenario1
        private void DoSingleControlTest(TParams p, ScenarioResult sr, ContainerType contType)
        {
            // get "control" bitmaps
            // save bitmaps to local files for comparison during debugging

            // have to set topmost so can get bitmap !!!
            this.Topmost = true;
            using (PointerInput.Activate(Mouse.Instance))
            {
                Mouse.Instance.Move(new System.Drawing.Point(0, 0));
            }

            MyPause();

            // twiddle with window to get colors right !!!
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;
            //Utilities.ActiveFreeze("getting control bitmaps");

            MyPause();
            System.Drawing.Bitmap bmpVisible = Utilities.GetBitmapOfControl(_btn1, true);

            _btn1.Visible = false;
            MyPause();
            System.Drawing.Bitmap bmpNotVisible = Utilities.GetBitmapOfControl(_btn1, true);
            _btn1.Visible = true;

            _wfh1.Visibility = Visibility.Collapsed;
            MyPause();
            System.Drawing.Bitmap bmpCollapsed = Utilities.GetBitmapOfControl(_btn1, true);

            _wfh1.Visibility = Visibility.Hidden;
            MyPause();
            System.Drawing.Bitmap bmpHidden = Utilities.GetBitmapOfControl(_btn1, true);
            _wfh1.Visibility = Visibility.Visible;
            this.Topmost = false;

            if (_debug)
            {
                bmpVisible.Save("_Visible.bmp");
                bmpNotVisible.Save("_NotVisible.bmp");
                bmpCollapsed.Save("_Collapsed.bmp");
                bmpHidden.Save("_Hidden.bmp");
            }
            //Utilities.ActiveFreeze("after getting control bitmaps");

            // iterate through Visibility/Visible settings
            // Container level
            foreach (Visibility contVis in Enum.GetValues(typeof(Visibility)))
            {
                if (_debug) { p.log.WriteLine("contVis = '{0}'", contVis); }

                // WF Host control
                foreach (Visibility wfhVis in Enum.GetValues(typeof(Visibility)))
                {
                    if (_debug) { p.log.WriteLine("\twfhVis = '{0}'", wfhVis); }

                    // WF control
                    //foreach (bool wfcVis in new bool[] { true, false })
                    //{
                    //    if (debug) { p.log.WriteLine("\t\twfcVis = '{0}'", wfcVis); }

                        // reset controls !!!
                        if (true)
                        {
                            SetPanelVisibility(contType, Visibility.Visible);
                            _wfh1.Visibility = Visibility.Visible;
                            _btn1.Visible = true;
                            MyPause();
                        }

                        // set states to test values
                        //if (debug) { p.log.WriteLine("setting state to {0} {1} {2}", contVis, wfhVis, wfcVis); }
                        //if (debug) { p.log.WriteLine("setting state to  {0} {1}", contVis, wfhVis); }
                        p.log.WriteLine("setting state to  {0} {1}", contVis, wfhVis);
                        SetPanelVisibility(contType, contVis);
                        _wfh1.Visibility = wfhVis;
                        //_btn1.Visible = wfcVis;
                        MyPause();

                        // this area seems to still be in flux pending Layout Spec/Code changes !!!
                        // calc expected states !!!
                        Visibility expContVis = contVis;
                        Visibility expHostVis = wfhVis;
                    
                        // if Host is visible, then hosted control will be visible
                        bool expCtrlVis = (expHostVis == Visibility.Visible);

                        // log expected/actual state settings
                        if (_debug)
                        {
                            p.log.WriteLine("expected state is {0} {1} {2}",
                                expContVis, expHostVis, expCtrlVis);
                            p.log.WriteLine("state is now      {0} {1} {2}",
                                GetPanelVisibility(contType), _wfh1.Visibility, _btn1.Visible);
                        }

                        // compare expected/actual values
                        Visibility actContVis = GetPanelVisibility(contType);
                        WPFMiscUtils.IncCounters(sr, expContVis, actContVis, "Container.Visibility not set properly", p.log);
                        WPFMiscUtils.IncCounters(sr, expHostVis, _wfh1.Visibility, "WindowsFormsHost.Visibility not set properly", p.log);
                        WPFMiscUtils.IncCounters(sr, expCtrlVis, _btn1.Visible, "WinFormCtrl.Visible not set properly", p.log);

                        //Utilities.ActiveFreeze("after set states, wait for layout to catch up");
                        MyPause();

                        // get bitmap of our control
                        // compare bitmap with that of what it is supposed to look like
                        if (true)
                        {
                            // have to set topmost so can get bitmap !!!
                            this.Topmost = true;
                            MyPause();

                            // get bitmap of current state of control (save to file for debug)
                            System.Drawing.Bitmap bmpBtn = Utilities.GetBitmapOfControl(_btn1, true);
                            if (_debug) { bmpBtn.Save("_control.bmp"); }
                MyPause();
                            this.Topmost = false;
                MyPause();
                            // !!!
                            // decide which "control" bitmap our button is expected to look like
                            // if container is visible, then control should match that of panel
                            bool bMatch;
                MyPause();
                            if (contVis == Visibility.Visible)
                            {
                                if (wfhVis == Visibility.Visible)
                                {
                                    LogDebug(p, "Comparing with Visible");
                                    bMatch = BitmapsCloseEnough(bmpVisible, bmpBtn);
                                }
                                else if (wfhVis == Visibility.Hidden)
                                {
                                    LogDebug(p, "Comparing with Hidden");
                                    bMatch = BitmapsCloseEnough(bmpHidden, bmpBtn);
                                }
                                else
                                {
                                    LogDebug(p, "Comparing with Collapsed");
                                    bMatch = BitmapsCloseEnough(bmpCollapsed, bmpBtn);
                                }
                            }
                            else
                            {
                                LogDebug(p, "Comparing with NotVisible");
                                bMatch = BitmapsCloseEnough(bmpNotVisible, bmpBtn);
                            }

                            WPFMiscUtils.IncCounters(sr, p.log, bMatch, "Control does not match bitmap image");
                            if (!bMatch)
                            {
                                p.log.WriteLine("bMatch is {0}", bMatch);
                                p.log.LogKnownBug(BugDb.WindowsOSBugs, 1548627, "Control not painting correctly");
                                //Utilities.ActiveFreeze("Bitmap doesn't match");
                            }
                        }

                        if (_debug) { p.log.WriteLine(""); }
                    //}
                }
            }
        }

        // Helper routine for Scenario2
        private void DoComplexControlTest(TParams p, ScenarioResult sr, ContainerType contType)
        {
            // get "control" bitmaps
            using (PointerInput.Activate(Mouse.Instance))
            {
                Mouse.Instance.Move(new System.Drawing.Point(0, 0));
            }

            // have to set topmost so can get bitmap !!!
            this.Topmost = true;
            MyPause();

            // twiddle with window to get colors right !!!
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;
            //Utilities.ActiveFreeze("getting control bitmaps");

            MyPause();
            System.Drawing.Bitmap bmpVisible = Utilities.GetBitmapOfControl(_userctl, true);

            _userctl.Visible = false;
            MyPause();
            System.Drawing.Bitmap bmpNotVisible = Utilities.GetBitmapOfControl(_userctl, true);
            _userctl.Visible = true;

            _wfh1.Visibility = Visibility.Collapsed;
            MyPause();
            System.Drawing.Bitmap bmpCollapsed = Utilities.GetBitmapOfControl(_userctl, true);

            _wfh1.Visibility = Visibility.Hidden;
            MyPause();
            System.Drawing.Bitmap bmpHidden = Utilities.GetBitmapOfControl(_userctl, true);
            _wfh1.Visibility = Visibility.Visible;
            this.Topmost = false;

            if (_debug)
            {
                bmpVisible.Save("_Visible.bmp");
                bmpNotVisible.Save("_NotVisible.bmp");
                bmpCollapsed.Save("_Collapsed.bmp");
                bmpHidden.Save("_Hidden.bmp");
            }
            //Utilities.ActiveFreeze("after getting control bitmaps");

            // iterate through Visibility/Visible settings
            // Container level
            foreach (Visibility contVis in Enum.GetValues(typeof(Visibility)))
            {
                if (_debug) { p.log.WriteLine("contVis = '{0}'", contVis); }

                // WF Host control
                foreach (Visibility wfhVis in Enum.GetValues(typeof(Visibility)))
                {
                    if (_debug) { p.log.WriteLine("\twfhVis = '{0}'", wfhVis); }

                    // UserControl control
                    foreach (bool usrVis in new bool[] { true, false })
                    {
                        if (_debug) { p.log.WriteLine("\t\tusrVis = '{0}'", usrVis); }

                        // WF control
                        //foreach (bool wfcVis in new bool[] { true, false })
                        //{
                        //    if (debug) { p.log.WriteLine("\t\t\twfcVis = '{0}'", wfcVis); }

                            // reset controls !!!
                            if (true)
                            {
                                SetPanelVisibility(contType, Visibility.Visible);
                                _wfh1.Visibility = Visibility.Visible;
                                _userctl.Visible = true;
                                _dgv.Visible = true;
                                MyPause();
                            }

                            // set states to test values
                            //if (debug) { p.log.WriteLine("setting state to  {0} {1} {2} {3}", contVis, wfhVis, usrVis, wfcVis); }
                            //if (debug) { p.log.WriteLine("setting state to  {0} {1} {2}", contVis, wfhVis, usrVis); }
                            p.log.WriteLine("setting state to  {0} {1} {2}", contVis, wfhVis, usrVis);
                            SetPanelVisibility(contType, contVis);
                            _wfh1.Visibility = wfhVis;
                            _userctl.Visible = usrVis;
                            //_dgv.Visible = wfcVis;
                            MyPause();

                            // this area seems to still be in flux pending Layout Spec/Code changes !!!
                            // calc expected states !!!
                            Visibility expContVis = contVis;
                            Visibility expHostVis = wfhVis;
                            bool expUserVis = usrVis;
                            bool expCtrlVis = expUserVis;

                            // log expected/actual state settings
                            if (_debug)
                            {
                                p.log.WriteLine("expected state is {0} {1} {2} {3}", 
                                    expContVis, expHostVis, expUserVis, expCtrlVis);
                                p.log.WriteLine("state is now      {0} {1} {2} {3}", 
                                    GetPanelVisibility(contType), _wfh1.Visibility, _userctl.Visible, _dgv.Visible);
                            }

                            // compare expected/actual values
                            Visibility actContVis = GetPanelVisibility(contType);
                            WPFMiscUtils.IncCounters(sr, expContVis, actContVis, "Container.Visibility not set properly", p.log);
                            WPFMiscUtils.IncCounters(sr, expHostVis, _wfh1.Visibility, "WindowsFormsHost.Visibility not set properly", p.log);
                            WPFMiscUtils.IncCounters(sr, expUserVis, _userctl.Visible, "WinForm UserCtrl.Visible not set properly", p.log);
                            WPFMiscUtils.IncCounters(sr, expCtrlVis, _dgv.Visible, "WinFormCtrl.Visible not set properly", p.log);

                            if (true)
                            {
                                // get bitmap of our control
                                // still developing this for Scenario - waiting for Layout coding to be complete !!!
                                // have to set topmost so can get bitmap !!!
                                this.Topmost = true;
                                MyPause();

                                // get bitmap of current state of button (save to file for debug)
                                System.Drawing.Bitmap bmpCtrl = Utilities.GetBitmapOfControl(_userctl, true);
                                if (_debug) { bmpCtrl.Save("_control.bmp"); }

                                this.Topmost = false;

                                // !!!
                                // decide which "control" bitmap our button is expected to look like
                                // if container is visible, then control should match that of panel
                                bool bMatch;
                                //if (contVis == Visibility.Visible && usrVis && wfcVis)
                                //if (contVis == Visibility.Visible && usrVis)
                                if (contVis == Visibility.Visible)
                                {
                                    if (wfhVis == Visibility.Visible)
                                    {
                                        if (usrVis)
                                        {
                                            LogDebug(p, "Comparing with Visible");
                                            bMatch = BitmapsCloseEnough(bmpVisible, bmpCtrl);
                                        }
                                        else
                                        {
                                            LogDebug(p, "Comparing with NotVisible");
                                            bMatch = BitmapsCloseEnough(bmpNotVisible, bmpCtrl);
                                        }
                                    }
                                    else if (wfhVis == Visibility.Hidden)
                                    {
                                        LogDebug(p, "Comparing with Hidden");
                                        bMatch = BitmapsCloseEnough(bmpHidden, bmpCtrl);
                                    }
                                    else
                                    {
                                        LogDebug(p, "Comparing with Collapsed");
                                        bMatch = BitmapsCloseEnough(bmpCollapsed, bmpCtrl);
                                    }
                                }
                                else
                                {
                                    LogDebug(p, "Comparing with NotVisible");
                                    bMatch = BitmapsCloseEnough(bmpNotVisible, bmpCtrl);
                                }

                                WPFMiscUtils.IncCounters(sr, p.log, bMatch, "Control does not match bitmap image");
                                if (!bMatch)
                                {
                                    p.log.WriteLine("bMatch is {0}", bMatch);
                                    p.log.LogKnownBug(BugDb.WindowsOSBugs, 1548627, "Control not painting correctly");
                                    //Utilities.ActiveFreeze("Bitmap doesn't match");
                                }
                            }

                            if (_debug) { p.log.WriteLine(""); }
                        //}
                    }
                }
            }
        }

        // Helper routine for Scenario3
        private void DoContainerControlTest(TParams p, ScenarioResult sr, ContainerType contType)
        {
            // get "control" bitmaps
            using (PointerInput.Activate(Mouse.Instance))
            {
                Mouse.Instance.Move(new System.Drawing.Point(0, 0));
            }

            // have to set topmost so can get bitmap !!!
            this.Topmost = true;
            MyPause();

            // twiddle with window to get colors right !!!
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;
            //Utilities.ActiveFreeze("getting control bitmaps");

            MyPause();
            System.Drawing.Bitmap bmpVisible = Utilities.GetBitmapOfControl(_panel, true);

            _panel.Visible = false;
            MyPause();
            System.Drawing.Bitmap bmpNotVisible = Utilities.GetBitmapOfControl(_panel, true);
            _panel.Visible = true;

            _wfh1.Visibility = Visibility.Collapsed;
            MyPause();
            System.Drawing.Bitmap bmpCollapsed = Utilities.GetBitmapOfControl(_panel, true);

            _wfh1.Visibility = Visibility.Hidden;
            MyPause();
            System.Drawing.Bitmap bmpHidden = Utilities.GetBitmapOfControl(_panel, true);
            _wfh1.Visibility = Visibility.Visible;
            this.Topmost = false;

            if (_debug)
            {
                bmpVisible.Save("_Visible.bmp");
                bmpNotVisible.Save("_NotVisible.bmp");
                bmpCollapsed.Save("_Collapsed.bmp");
                bmpHidden.Save("_Hidden.bmp");
            }
            //Utilities.ActiveFreeze("after getting control bitmaps");

            // iterate through Visibility/Visible settings
            // Container level
            foreach (Visibility contVis in Enum.GetValues(typeof(Visibility)))
            {
                if (_debug) { p.log.WriteLine("contVis = '{0}'", contVis); }

                // WF Host control
                foreach (Visibility wfhVis in Enum.GetValues(typeof(Visibility)))
                {
                    if (_debug) { p.log.WriteLine("\twfhVis = '{0}'", wfhVis); }

                    // Panel control
                    foreach (bool panVis in new bool[] { true, false })
                    {
                        if (_debug) { p.log.WriteLine("\t\tpanVis = '{0}'", panVis); }

                        // WF control
                        //foreach (bool wfcVis in new bool[] { true, false })
                        //{
                        //    if (debug) { p.log.WriteLine("\t\t\twfcVis = '{0}'", wfcVis); }

                            // reset controls !!!
                            if (true)
                            {
                                SetPanelVisibility(contType, Visibility.Visible);
                                _wfh1.Visibility = Visibility.Visible;
                                _panel.Visible = true;
                                _dgv.Visible = true;
                                _tb1.Visible = true;
                                MyPause();
                            }

                            // set states to test values
                            //if (debug) { p.log.WriteLine("setting state to  {0} {1} {2} {3}", contVis, wfhVis, panVis, wfcVis); }
                            //if (debug) { p.log.WriteLine("setting state to  {0} {1} {2}", contVis, wfhVis, panVis); }
                            p.log.WriteLine("setting state to  {0} {1} {2}", contVis, wfhVis, panVis);
                            SetPanelVisibility(contType, contVis);
                            _wfh1.Visibility = wfhVis;
                            _panel.Visible = panVis;
                            //_dgv.Visible = wfcVis;
                            MyPause();

                            // this area seems to still be in flux pending Layout Spec/Code changes !!!
                            // calc expected states !!!
                            Visibility expContVis = contVis;
                            Visibility expHostVis = wfhVis;
                            bool expPanelVis = panVis;
                            bool expCtrlVis = expPanelVis;

                            // log expected/actual state settings
                            if (_debug)
                            {
                                p.log.WriteLine("expected state is {0} {1} {2} {3}", 
                                    expContVis, expHostVis, expPanelVis, expCtrlVis);
                                p.log.WriteLine("state is now      {0} {1} {2} {3}", 
                                    GetPanelVisibility(contType), _wfh1.Visibility, _panel.Visible, _dgv.Visible);
                            }

                            // compare expected/actual values
                            Visibility actContVis = GetPanelVisibility(contType);
                            WPFMiscUtils.IncCounters(sr, expContVis, actContVis, "Container.Visibility not set properly", p.log);
                            WPFMiscUtils.IncCounters(sr, expHostVis, _wfh1.Visibility, "WindowsFormsHost.Visibility not set properly", p.log);
                            WPFMiscUtils.IncCounters(sr, expPanelVis, _panel.Visible, "WinForm Panel.Visible not set properly", p.log);
                            WPFMiscUtils.IncCounters(sr, expCtrlVis, _dgv.Visible, "WinFormCtrl.Visible not set properly", p.log);

                            // get bitmap of our control
                            if (true)
                            {
                                // have to set topmost so can get bitmap !!!
                                this.Topmost = true;
                                MyPause();

                                // get bitmap of current state of button (save to file for debug)
                                System.Drawing.Bitmap bmpCtrl = Utilities.GetBitmapOfControl(_panel, true);
                                if (_debug) { bmpCtrl.Save("_control.bmp"); }

                                this.Topmost = false;

                                // !!!
                                // decide which "control" bitmap our button is expected to look like
                                // if container is visible, then control should match that of panel
                                bool bMatch;
                                //if (contVis == Visibility.Visible && usrVis && wfcVis)
                                if (contVis == Visibility.Visible)
                                {
                                    if (wfhVis == Visibility.Visible)
                                    {
                                        if (panVis)
                                        {
                                            LogDebug(p, "Comparing with Visible");
                                            bMatch = BitmapsCloseEnough(bmpVisible, bmpCtrl);
                                        }
                                        else
                                        {
                                            LogDebug(p, "Comparing with NotVisible");
                                            bMatch = BitmapsCloseEnough(bmpNotVisible, bmpCtrl);
                                        }
                                    }
                                    else if (wfhVis == Visibility.Hidden)
                                    {
                                        LogDebug(p, "Comparing with Hidden");
                                        bMatch = BitmapsCloseEnough(bmpHidden, bmpCtrl);
                                    }
                                    else
                                    {
                                        LogDebug(p, "Comparing with Collapsed");
                                        bMatch = BitmapsCloseEnough(bmpCollapsed, bmpCtrl);
                                    }
                                }
                                else
                                {
                                    LogDebug(p, "Comparing with NotVisible");
                                    bMatch = BitmapsCloseEnough(bmpNotVisible, bmpCtrl);
                                }

                                WPFMiscUtils.IncCounters(sr, p.log, bMatch, "Control does not match bitmap image");
                                if (!bMatch)
                                {
                                    p.log.WriteLine("bMatch is {0}", bMatch);
                                    p.log.LogKnownBug(BugDb.WindowsOSBugs, 1548627, "Control not painting correctly");
                                    //Utilities.ActiveFreeze("Bitmap doesn't match");
                                }
                            }

                            if (_debug) { p.log.WriteLine(""); }
                        //}
                    }
                }
            }
        }
        #endregion

        #region Helper functions
        // Helper function used to set the Visibility setting of container control we are using
        // (Note: for this purpose, I believe if-then-else looks cleaner than switch-case)
        private void SetPanelVisibility(ContainerType contType, Visibility vis)
        {
            // create container
            if (contType == ContainerType.Canvas)
            {
                _canvas.Visibility = vis;
            }
            else if (contType == ContainerType.DockPanel)
            {
                _dp.Visibility = vis;
            }
            else if (contType == ContainerType.Grid)
            {
                _grid.Visibility = vis;
            }
            else if (contType == ContainerType.StackPanel)
            {
                _stack.Visibility = vis;
            }
            else if (contType == ContainerType.WrapPanel)
            {
                _wrap.Visibility = vis;
            }
            else
            {
                throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
            }
        }

        // Helper function used to retrieve Visibility setting of container control we are using
        // (Note: for this purpose, I believe if-then-else looks cleaner than switch-case)
        private Visibility GetPanelVisibility(ContainerType contType)
        {
            // create container
            if (contType == ContainerType.Canvas)
            {
                return (_canvas.Visibility);
            }
            else if (contType == ContainerType.DockPanel)
            {
                return (_dp.Visibility);
            }
            else if (contType == ContainerType.Grid)
            {
                return (_grid.Visibility);
            }
            else if (contType == ContainerType.StackPanel)
            {
                return (_stack.Visibility);
            }
            else if (contType == ContainerType.WrapPanel)
            {
                return (_wrap.Visibility);
            }
            else
            {
                throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
            }
        }

        // Helper function to set up app for particular Scenario
        // Will create Avalon/WinForms controls based on desired Container Type and Test Type
        // Basically creates desired WFH control sandwiched between two Avalon buttons within specified container
        // contType - container type, the kind of high level Avalon container we are using
        // testType - test type, based on whats being tested in Scenario
        private void TestSetup(TParams p, ContainerType contType, TestType testType)
        {
            // update app title bar and log file
            string str = string.Format("Container type: {0} using {1}", contType.ToString(), testType.ToString());
            this.Title = str;
            p.log.WriteLine(str);

            // avalon button (left side)
            System.Windows.Controls.Button btn1 = new Button();
            btn1.Content = "Avalon Button 1";
            
            // create WF host control
            _wfh1 = new WindowsFormsHost();

            // WFH containing simple control - WF button
            if (testType == TestType.Single)
            {
                // create WF controls
                //_tb1 = new System.Windows.Forms.TextBox();
                _btn1 = new System.Windows.Forms.Button();
                //_cbx = new System.Windows.Forms.CheckBox();
                //_dgv = new System.Windows.Forms.DataGridView();

                // throw some random text into controls
                //_tb1.Text = p.ru.GetString(10);
                _btn1.Text = "Push Me";
                //_cbx.Text = "My Checkbox";
                //SetupDGV();

                // add WF controls to host control
                //_wfh1.Children.Add(_tb1);
                _wfh1.Child =_btn1;
            }

            // WFH containing WF usercontrol containing DataGridView
            else if (testType == TestType.Complex)
            {
                // create WF usercontrol
                _userctl = new System.Windows.Forms.UserControl();
                _userctl.Dock = System.Windows.Forms.DockStyle.Fill;

                // add WF control to usercontrol
                _dgv = new System.Windows.Forms.DataGridView();
                _userctl.Controls.Add(_dgv);

                // throw some random text into control
                SetupDGV();

                // add WF usercontrol to host control
                System.Drawing.Size uSz = _userctl.Size;
                _wfh1.Child = _userctl;

                // !!! workaround - must manually change size of WFH after add UC
                _wfh1.Width = uSz.Width;
                _wfh1.Height = uSz.Height;

                // !!! when usercontrol added to wfh, it's size is set to 0,0 and cannot be changed
                //_userctl.Size = new System.Drawing.Size(100, 100);
            }

            // WFH containing WF container (Panel) containing TextBox and DataGridView
            else if (testType == TestType.Container)
            {
                // create WF container with three controls
                _panel = new System.Windows.Forms.Panel();
                _panel.Dock = System.Windows.Forms.DockStyle.Fill;

                // create, add 3 WF controls to panel
                _tb1 = new System.Windows.Forms.TextBox();
                _dgv = new System.Windows.Forms.DataGridView();

                // assign locations for WF controls
                //_tb1.Location = new System.Drawing.Point(10, 10);
                //_dgv.Location = new System.Drawing.Point(100, 100);
                _tb1.Location = new System.Drawing.Point(50, 160);
                _dgv.Location = new System.Drawing.Point(0, 0);
                _dgv.Height = 150;
                //_panel.Size = new System.Drawing.Size(_panel.Size.Width, _panel.Size.Height + 70);
                //_panel.Size = new System.Drawing.Size(240, 150 + 70);
                _panel.Size = new System.Drawing.Size(_dgv.Size.Width, _dgv.Size.Height + 70);

                _panel.Controls.AddRange(new System.Windows.Forms.Control[] { _tb1, _dgv });

                // throw some random text into controls
                _tb1.Text = p.ru.GetString(10);
                SetupDGV();

                // add WF panel to host control
                System.Drawing.Size pSz = _panel.Size;
                _wfh1.Child =_panel;

                // !!! workaround - must manually change size of WFH after add Panel
                _wfh1.Width = pSz.Width;
                _wfh1.Height = pSz.Height;

                // !!! when panel added to wfh, it's size is set to 0,0 and cannot be changed
                //_panel.Size = new System.Drawing.Size(100, 100);
            }

            else
            {
                // unknown TestType?
                throw new ArgumentException("Unknown TestType '{0}'", testType.ToString());
            }

            // avalon button (right side)
            System.Windows.Controls.Button btn2 = new Button();
            btn2.Content = "Avalon Button 2";

            // create (Avalon) container, add controls, set as application content
            if (contType == ContainerType.Canvas)
            {
                _canvas = new Canvas();

                // have to set explicit locations for Canvas
                Canvas.SetLeft(btn1, 50);
                Canvas.SetLeft(_wfh1, 150);
                Canvas.SetLeft(btn2, 300);

                _canvas.Children.Add(btn1);
                _canvas.Children.Add(_wfh1);
                _canvas.Children.Add(btn2);
                this.Content = _canvas;
            }
            else if (contType == ContainerType.DockPanel)
            {
                _dp = new DockPanel();
                _dp.Children.Add(btn1);
                _dp.Children.Add(_wfh1);
                _dp.Children.Add(btn2);
                this.Content = _dp;
            }
            else if (contType == ContainerType.Grid)
            {
                _grid = new Grid();

                // have to define Columns/Rows for Grid
                _grid.ColumnDefinitions.Add(new ColumnDefinition());
                _grid.ColumnDefinitions.Add(new ColumnDefinition());
                _grid.ColumnDefinitions.Add(new ColumnDefinition());
                _grid.RowDefinitions.Add(new RowDefinition());
                Grid.SetColumn(btn1, 0);
                Grid.SetColumn(_wfh1, 1);
                Grid.SetColumn(btn2, 2);

                _grid.Children.Add(btn1);
                _grid.Children.Add(_wfh1);
                _grid.Children.Add(btn2);
                this.Content = _grid;
            }
            else if (contType == ContainerType.StackPanel)
            {
                _stack = new StackPanel();
                _stack.Children.Add(btn1);
                _stack.Children.Add(_wfh1);
                _stack.Children.Add(btn2);
                this.Content = _stack;
            }
            else if (contType == ContainerType.WrapPanel)
            {
                _wrap = new WrapPanel();
                _wrap.Children.Add(btn1);
                _wrap.Children.Add(_wfh1);
                _wrap.Children.Add(btn2);
                this.Content = _wrap;
            }
            else
            {
                // unknown ContainerType?
                throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
            }
        }

        // Helper function to put stuff into DataGridView control
        private void SetupDGV()
        {
            // add Columns/Rows to DGV
            _dgv.Columns.Add("Col1", "Column 1");
            _dgv.Columns.Add("Col2", "Column 2");
            _dgv.Rows.Add(4);
            _dgv.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        private static void MyPause()
        {
            // does this still cause exception? !!!
            for (int i = 0; i < 2; i++)
            {
                WPFReflectBase.DoEvents();
                System.Threading.Thread.Sleep(10);
            }
        }

        private void LogDebug(TParams p, string msg)
        {
            if (_debug) { p.log.WriteLine(msg); }
        }

        /// <summary>
        /// Utilities.BitmapsIdentical allows *no* tolerance when comparing bitmaps pixel by pixel.
        /// This allows the "standard" 8 pixel "tolerance" setting afforded by ColorsMatch, which is sufficient.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool BitmapsCloseEnough(System.Drawing.Bitmap a, System.Drawing.Bitmap b)
        {
            if ((a == null) || (b == null))
                return false;
            else if ((a.Width != b.Width) || (a.Height != b.Height))
                return false;

            for (int i = 0; i < a.Width; i++)
            {
                for (int j = 0; j < a.Height; j++)
                {
                    System.Drawing.Color ac = a.GetPixel(i, j);
                    System.Drawing.Color bc = b.GetPixel(i, j);

                    if (!Utilities.ColorsMatch(ac, bc))
                    {
                        Console.WriteLine("a({0}, {1}): {2}", i, j, a.GetPixel(i, j).ToString());
                        Console.WriteLine("b({0}, {1}): {2}", i, j, b.GetPixel(i, j).ToString());
                        return false;
                    }

                    //if (ignoreTransparency && (ac.A == 0 || bc.A == 0))
                    //    continue;
                    //else if (ac.ToArgb() != bc.ToArgb())
                    //{
                    //    Console.WriteLine("a({0}, {1}): {2}", i, j, a.GetPixel(i, j).ToString());
                    //    Console.WriteLine("b({0}, {1}): {2}", i, j, b.GetPixel(i, j).ToString());
                    //    return false;
                    //}
                }
            }

            return true;
        }

        #endregion

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ WFH with single control
//@ WFH with complex control
//@ WFH with container control
//@ WF control Visable transitions (true -&gt; true, true-&gt;false, false -&gt;true)