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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading;
using System.Windows.Forms.Integration;

using MS.Internal.Mita.Foundation;

//
// Testcase:    EventOrdering
// Description: Verify that a WF control recieves all the expected during it's lifecycle
//
namespace WindowsFormsHostTests
{
    public class EventOrdering : WPFReflectBase
    {
        #region Testcase setup
        public EventOrdering(string[] args) : base(args) { }


        // class vars
        private enum Operation { Move, Resize };
        private WindowsFormsHost _wfh;                   // our Host control
        private System.Windows.Forms.Button _wfBtn;      // our WF control
        private string _expHostEvents;                   // expected WFH events
        private string _expCtrlEvents;                   // expected WF Control events

        protected override void InitTest(TParams p)
        {
            // hacks to get window to show up !!!
            this.Topmost = true;
            this.Topmost = false;
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;

            // although not using Mita during the test, we are using it here
            // just to move the mouse out of the way (for test consistency)
            Mouse.Instance.Move(new System.Drawing.Point(0, 0));
            UseMITA = false;

            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            bool b = base.BeforeScenario(p, scenario);

            // debug - run specific scenario !!!
            //if (scenario.Name != "Scenario9") { return false; }

            return b;
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Verify Events and Order when a WF control hosted in a WFH and an AV window is Created, Shown and Closed")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // interpret as: 
            // step 1: Create WFH containing WF control
            // step 2: create new Avalon window, add WFH, show window, close window - checking events along the way

            // debug !!!
            GetControlData(p);

            // create WFH with WF control
            CreateWFHWithWFControl();

            // create Avalon window, look at events
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            Window w = new Window();
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when creating window '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when creating window '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // add DockPanel to window, look at events
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            DockPanel dp = new DockPanel();
            w.Content = dp;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when adding DockPanel to window '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when adding DockPanel to window '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // add WFH to DockPanel, look at events
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            dp.Children.Add(_wfh);
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when adding WFH to DockPanel '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when adding WFH to DockPanel '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "Initialized:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);


            // show window, look at events
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            w.Show();
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when showing window '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when showing window '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "IsVisibleChanged:SizeChanged:LayoutUpdated:LayoutUpdated:Loaded:LayoutUpdated:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "HandleCreated:BindingContextChanged:VisibleChanged:Invalidated:Layout:Resize:SizeChanged:ClientSizeChanged:Invalidated:BackColorChanged:VisibleChanged:Paint:";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // close window, look at events
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            w.Close();
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when closing window '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when closing window '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "IsVisibleChanged:Unloaded:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "VisibleChanged:";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            return sr;
        }

        // debug !!!
        private void GetControlData(TParams p)
        {
            p.log.WriteLine("Control Test using WinForms Button on WinForms Window");
            // create WF control
            System.Windows.Forms.Button wfBtn = new System.Windows.Forms.Button();
            wfBtn.Text = "WinForms";

            // set up our event handlers
            WFBUtil.AddHandlers(wfBtn);

            // create WinForms window, look at events
            WFBUtil.ResetEvents();
            Form1 f = new Form1();
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when creating window '{0}'", WFBUtil.GetEvents());
            //p.log.WriteLine("");

            // add WF Control to Window, look at events
            WFBUtil.ResetEvents();
            f.Controls.Add(wfBtn);
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when adding WF Control to WF Window '{0}'", WFBUtil.GetEvents());
            //p.log.WriteLine("");

            // show window, look at events
            WFBUtil.ResetEvents();
            f.Show();
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when showing window '{0}'", WFBUtil.GetEvents());
            //p.log.WriteLine("");

            // move the window left
            WFBUtil.ResetEvents();
            f.Left = f.Left / 2;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when moving window '{0}'", WFBUtil.GetEvents());
            //p.log.WriteLine("");

            // resize the window
            WFBUtil.ResetEvents();
            f.Width = 200;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when resizing window '{0}'", WFBUtil.GetEvents());
            //p.log.WriteLine("");

            // minimize the window
            WFBUtil.ResetEvents();
            f.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when setting minimized '{0}'", WFBUtil.GetEvents());
            //p.log.WriteLine("");

            // maximize the window
            WFBUtil.ResetEvents();
            f.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when setting maximized '{0}'", WFBUtil.GetEvents());
            //p.log.WriteLine("");

            // minimize the window again
            WFBUtil.ResetEvents();
            f.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when setting minimized '{0}'", WFBUtil.GetEvents());
            //p.log.WriteLine("");

            // restore the window
            WFBUtil.ResetEvents();
            f.WindowState = System.Windows.Forms.FormWindowState.Normal;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when setting normal '{0}'", WFBUtil.GetEvents());
            //p.log.WriteLine("");

            // close window, look at events
            WFBUtil.ResetEvents();
            f.Close();
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Ctrl Events when closing window '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");

            //Utilities.ActiveFreeze();

            //f.Close();
        }

        [Scenario("Verify Events and Order when a WF control is added and removed from a WFH")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // create DockPanel, add to main window
            DockPanel dp = new DockPanel();
            this.Content = dp;

            // create WFH, add to DockPanel
            WindowsFormsHost wfh = new WindowsFormsHost();
            dp.Children.Add(wfh);

            // create WF control
            System.Windows.Forms.Button wfBtn = new System.Windows.Forms.Button();
            wfBtn.Text = "WinForms";

            // set up our handlers
            WFHUtil.AddHandlers(wfh);
            WFBUtil.AddHandlers(wfBtn);
            WPFReflectBase.DoEvents();

            // add WF control to WFH, look at events
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            wfh.Child = wfBtn;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when adding WFC to WFH '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when adding WFC to WFH '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "SizeChanged:LayoutUpdated:LayoutUpdated:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "ParentChanged:FontChanged:Layout:ForeColorChanged:BackColorChanged:HandleCreated:BindingContextChanged:MarginChanged:Invalidated:Layout:Resize:SizeChanged:ClientSizeChanged:Paint:";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // remove WF control from WFH
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();

            wfh.Child = null;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when removing WFC from WFH '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when removing WFC from WFH '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "SizeChanged:LayoutUpdated:LayoutUpdated:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "VisibleChanged:VisibleChanged:ParentChanged:Invalidated:FontChanged:Layout:Invalidated:ForeColorChanged:Invalidated:BackColorChanged:BindingContextChanged:";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            return sr;
        }

        [Scenario("Verify Events and Order when a WFH with a WF control is added and removed from a AV Window")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // interpret as: 
            // step 1: Create WFH containing WF control
            // step 2: Create AV window (with panel object such as DockPanel), show window
            // step 3: Add WFH to DockPanel, Remove WFH, check events along the way

            // create WFH with WF control
            CreateWFHWithWFControl();

            // create, show new Avalon window with DockPanel
            Window w = new Window();
            DockPanel dp = new DockPanel();
            w.Content = dp;
            w.Show();

            // let any events play out
            WPFReflectBase.DoEvents();

            // add WFH to window, catch events
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            dp.Children.Add(_wfh);
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when adding WFH to DockPanel '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when adding WFH to DockPanel '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "Initialized:IsVisibleChanged:SizeChanged:LayoutUpdated:LayoutUpdated:LayoutUpdated:Loaded:LayoutUpdated:LayoutUpdated:LayoutUpdated:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "HandleCreated:BindingContextChanged:VisibleChanged:Invalidated:Layout:Resize:SizeChanged:ClientSizeChanged:Invalidated:BackColorChanged:VisibleChanged:Paint:";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // remove WFH from window (by replacing with empty dockpanel), catch events
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            w.Content = dp;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when removing WFH from window '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when removing WFH from window '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            w.Close();

            return sr;
        }

        [Scenario("Verify Events and Order when a AV window with a WFH and WF control, is moved around")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            TestBehavior(p, sr, Operation.Move);
            return sr;
        }

        [Scenario("Verify Events and Order when a AV window with a WFH and WF control, is resized")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            TestBehavior(p, sr, Operation.Resize);
            return sr;
        }

        [Scenario("Verify Events and Order when a AV window with a WFH and WF control, is maximized and minimized")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // create WFH with WF control
            CreateWFHWithWFControl();

            // create, show new Avalon window with DockPanel
            Window w = new Window();
            DockPanel dp = new DockPanel();
            w.Content = dp;
            w.Show();

            // add WFH to DockPanel
            dp.Children.Add(_wfh);

            // let any events play out
            WPFReflectBase.DoEvents();

            // perform operations, catch events

            // minimize the window
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            w.WindowState = WindowState.Minimized;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when setting minimized '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when setting minimized '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // maximize the window
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            w.WindowState = WindowState.Maximized;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when setting maximized '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when setting maximized '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "SizeChanged:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "Invalidated:Layout:Resize:SizeChanged:ClientSizeChanged:Paint:";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // minimize the window again
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            w.WindowState = WindowState.Minimized;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when setting minimized '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when setting minimized '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // restore the window
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            w.WindowState = WindowState.Normal;
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when setting normal '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when setting normal '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "Paint:";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // clean up window
            w.Close();

            return sr;
        }

        [Scenario("Verify Events and Order when a WFH and WF control, is covered and not covered.")]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // due to Z-order limitations, all WFH objects will *always* be "on top of" any Avalon object
            // so it is not possible to have a WFH "covered" by an Avalon control - WFH will always be on top

            // interpret as: 
            // step 1: Create WFH containing WF control
            // step 2: Create AV window with Canvas, show window
            // step 3: Add WFH and a Avalon control to Canvas
            // step 4: Move Avalon control so that it partially covers WFH, check events along the way
            // step 5: Move Avalon control so that it uncovers WFH, check events along the way

            // create WFH with WF control
            CreateWFHWithWFControl();

            // create, show new Avalon window with Canvas
            Window w = new Window();
            Canvas can = new Canvas();
            w.Content = can;
            w.Show();

            // create Avalon button
            Button b = new Button();
            b.Content = "Avalon";

            // initially position children so Avalon button and WFH don't overlap
            Canvas.SetLeft(_wfh, 100);
            Canvas.SetTop(_wfh, 100);
            Canvas.SetLeft(b, 150);
            Canvas.SetTop(b, 150);

            // add WFH first, then button so button supposedly covers WFH
            can.Children.Add(_wfh);
            can.Children.Add(b);

            // let any events play out
            WPFReflectBase.DoEvents();

            // move Avalon button so it partially covers WFH
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            Canvas.SetTop(b, 110);
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when covering WFH '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when covering WFH '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // move Avalon button so it uncovers WFH
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            Canvas.SetTop(b, 150);
            WPFReflectBase.DoEvents();
            p.log.WriteLine("Host Events when uncovering WFH '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events when uncovering WFH '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");
            _expHostEvents = "LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:";
            WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // clean up window
            w.Close();

            return sr;
        }

        [Scenario("Verify Events and Order when a AV window with a WFH and WF control, shows an animation over the the WFH and WF control.  See TC:1072439")]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // create WFH with WF control
            CreateWFHWithWFControl();

            // create, show new Avalon window
            Window w = new Window();
            w.Show();

            // let any events play out
            WPFReflectBase.DoEvents();

            // start the animation
            SetupAnimation(w, _wfh);
            WPFReflectBase.DoEvents();

            //p.log.WriteLine("Host Events during animation sequence '{0}'", WFHUtil.GetEvents());
            p.log.WriteLine("Ctrl Events during animation sequence '{0}'", WFBUtil.GetEvents());
            p.log.WriteLine("");

            // expected event list should be nothing but "LayoutUpdated:"
            bool bEventsOk = OnlyLayoutUpdated(WFHUtil.GetEvents());
            WPFMiscUtils.IncCounters(sr, true, bEventsOk, "Did not get correct events", p.log);
            //expHostEvents = "";
            //WPFMiscUtils.IncCounters(sr, expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
            _expCtrlEvents = "";
            WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);

            // clean up window
            w.Close();

            return sr;
        }

        [Scenario("WindowsOS Bug 1547402 - Verify Winforms Controls can still get Validating/Validated messages")]
        public ScenarioResult Scenario9(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            string expEvents;

            // Note: this code block is intentionally surrounded by "if (true)" so as to
            // create a "scope" for the variable "w".  The idea was to then have w go out
            // of scope, and thus be cleaned up by the Garbage Collector, in a vain attempt
            // to force a "Disposed" event on the window.  (so far, this is not working as hoped)
            // need to update this part of the TC
            if (true)
            {
                // create new Avalon window
                Window w = new Window();

                // add some controls
                _wfhValidate = new WindowsFormsHost();
                System.Windows.Forms.CheckBox checkBox = new System.Windows.Forms.CheckBox();
                checkBox.Checked = true;
                checkBox.Text = "Cancel closing";
                _wfhValidate.Child = checkBox;
                w.Content = _wfhValidate;

                // set up event handlers needed for our test
                checkBox.Validating += new System.ComponentModel.CancelEventHandler(checkBox_Validating);
                checkBox.Validated += new EventHandler(checkBox_Validated);
                w.Closed += new EventHandler(w_Closed);
                checkBox.Disposed += new EventHandler(checkBox_Disposed);       // !!!
                //NOTE: this is to get focus into the child, which it's currently not: lots of focus issues
                ((System.Windows.Forms.ContainerControl)(_wfhValidate.Child.Parent)).ActiveControl = checkBox;

                // set up the workaround we are testing
                // we want to watch for "Closing" message and manually do Validation
                w.Closing += new System.ComponentModel.CancelEventHandler(w_Closing);

                // show window
                w.Show();

                // let any events play out
                WPFReflectBase.DoEvents();

                // perform test

                // set control to fail Validation, then try to close window
                p.log.WriteLine("Attempting to close window which fails Validation");
                _bPassValidation = false;
                _strEvents = "";
                w.Close();
                p.log.WriteLine("strEvents = '{0}'", _strEvents);

                // check events - should get Validating, but not Validated; Closing, but not Closed
                expEvents = "w_Closing:checkBox_Validating:";
                WPFMiscUtils.IncCounters(sr, expEvents, _strEvents, "Did not get expected events", p.log);

                // set control to pass Validation, then try to close window
                p.log.WriteLine("Attempting to close window which passes Validation");
                _bPassValidation = true;
                _strEvents = "";
                w.Close();
                p.log.WriteLine("strEvents = '{0}'", _strEvents);

                // check events - should get Validating, Validated, Closing, and Closed
                expEvents = "w_Closing:checkBox_Validating:checkBox_Validated:w_Closed:";
                WPFMiscUtils.IncCounters(sr, expEvents, _strEvents, "Did not get expected events", p.log);

                //checkBox.Dispose();
                w = null;
            }

            //// Create the AppDomain      
            //AppDomain newDomain = AppDomain.CreateDomain("newDomain");
            //Console.WriteLine("Host domain: " + AppDomain.CurrentDomain.FriendlyName);
            //Console.WriteLine("child domain: " + newDomain.FriendlyName);

            //// Unload the application domain.
            //AppDomain.Unload(newDomain);

            //// start new app to test Dispose !!!
            //Application app = new Application();
            //app.Run(new Win());

            //Console.WriteLine("The highest generation is {0}", GC.MaxGeneration);
            //Console.WriteLine("Generation: {0}", GC.GetGeneration(w));
            //Console.WriteLine("Total Memory: {0}", GC.GetTotalMemory(false));

            // pause !!!
            for (int i = 0; i < 10; i++)
            {
                WPFReflectBase.DoEvents();
                System.Threading.Thread.Sleep(500);
            }

            //!!!Utilities.ActiveFreeze("before");
            
            // force Garbage collector to run !!!
            // another failed attempt !!!
            GC.Collect();
            WPFReflectBase.DoEvents();
            GC.Collect(0);
            WPFReflectBase.DoEvents();
            GC.Collect(1);
            WPFReflectBase.DoEvents();
            GC.Collect(2);
            WPFReflectBase.DoEvents();
            GC.WaitForPendingFinalizers();
            WPFReflectBase.DoEvents();

            //!!!Utilities.ActiveFreeze("after");

            // let any events play out
            WPFReflectBase.DoEvents();
            return sr;
        }

        void checkBox_Disposed(object sender, EventArgs e)
        {
            _strEvents += "checkBox_Disposed:";
            // the following line executes *after* the TC exits
            // may mess up log file so have commented out for now
            //scenarioParams.log.WriteLine("-- Got checkBox_Disposed message");
        }

        #region Helpers for Scenario9
        private WindowsFormsHost _wfhValidate;
        private bool _bPassValidation;
        private string _strEvents;

        void checkBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _strEvents += "checkBox_Validating:";
            // we are being Validated - cancel if we want to fail Validation
            e.Cancel = !_bPassValidation;
            scenarioParams.log.WriteLine("-- Got checkBox_Validating message, setting Cancel={0}", e.Cancel);
        }

        void checkBox_Validated(object sender, EventArgs e)
        {
            _strEvents += "checkBox_Validated:";
            scenarioParams.log.WriteLine("-- Got checkBox_Validated message");
        }

        void w_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // our window is attempting to close - we must manually validate our WFH controls
            _strEvents += "w_Closing:";

            // for each WFH, (of which we have one) manually call it's hosted control's Validate method
            // if the Validate method returns false, we want to cancel the window close
            bool bVal = ((System.Windows.Forms.ContainerControl)_wfhValidate.Child.Parent).Validate(true);
            scenarioParams.log.WriteLine("-- in w_Closing, bVal is {0}", bVal);
            e.Cancel = !bVal;
        }

        void w_Closed(object sender, EventArgs e)
        {
            // our window has just closed
            _strEvents += "w_Closed:";
        }
        #endregion

        /// <summary>
        /// Twisted little helper function that examines a string (such as an event list) and verifies
        /// that it contains nothing but "LayoutUpdated:" events.  It does this by repeatedly searching
        /// the input string for this key value and, if found, removing it from the input string.  Thus, if
        /// the string really only contains repeated occurances of the key string, the resultant string will
        /// be blank.  Otherwise, it will be non-blank, being the extra events that did not match the key.
        /// As a bonus, it counts these events (but does not do anything with the count) for debugging.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool OnlyLayoutUpdated(string str)
        {
            string key = "LayoutUpdated:";
            int idx;
            int cnt = 0;

            // while find key in string, remove occurance
            // Note: IndexOf is sneakey - it will return "0" (not -1) when the string is empty
            for (idx = str.IndexOf(key); str != "" && idx != -1; )
            {
                cnt++;
                str = str.Remove(idx, key.Length);
            }

            // if string contained nothing but the key, it should now be blank
            return (str == "");
        }

        /// <summary>
        /// Helper function for animation Scenario.  Sets up window with an Avalon button that is animated
        /// so it moves over a WFH.  Events are processed and event list is cleared just before animation
        /// starts so all logged events are those resulting from actual animation (not window creation, etc.)
        /// </summary>
        /// <param name="w"></param>
        /// <param name="wfh"></param>
        private void SetupAnimation(Window w, WindowsFormsHost wfh)
        {
            // (this code adapted from that found in MSDN docs under "AnimationTimeline Class")

            // use a canvas so can position, move elements
            Canvas myCanvas = new Canvas();

            // Create and set the animated Button
            Button aButton = new Button();
            aButton.Content = "Avalon Button";
            aButton.Width = 200;

            // add the WFH to the canvas, set location
            Canvas.SetLeft(wfh, 200);
            Canvas.SetTop(wfh, 200);
            myCanvas.Children.Add(wfh);

            // Add the animated Button to the canvas
            // (don't do Canvas.SetTop because we are animating it)
            Canvas.SetLeft(aButton, 100);
            myCanvas.Children.Add(aButton);
            w.Content = myCanvas;

            // let any pre-animation events play out
            WPFReflectBase.DoEvents();

            // reset event list so only get animation events
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();

            // Animate the Button's Location
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 150;
            myDoubleAnimation.To = 250;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));

            // Apply the animation to the Button's Top property
            aButton.BeginAnimation(Canvas.TopProperty, myDoubleAnimation);
        }

        #endregion

        #region Helper functions
        /// <summary>
        /// Helper function to create WFH with a WF Button control.  Also sets up event handlers.
        /// Updates class vars for wfh and wfBtn
        /// </summary>
        private void CreateWFHWithWFControl()
        {
            // create WFH
            _wfh = new WindowsFormsHost();

            // create WF control
            _wfBtn = new System.Windows.Forms.Button();
            _wfBtn.Text = "WinForms";

            // add WF control to  WFH
            _wfh.Child = _wfBtn;

            // set up our event handlers
            WFHUtil.AddHandlers(_wfh);
            WFBUtil.AddHandlers(_wfBtn);

            // let any events play themselves out
            WPFReflectBase.DoEvents();
        }

        // helper function to create WFH with WF control in an Avalon window,
        // then perform a specified operation on it, and check events
        private void TestBehavior(TParams p, ScenarioResult sr, Operation op)
        {
            // create WFH with WF control
            CreateWFHWithWFControl();

            // create, show new Avalon window with DockPanel
            Window w = new Window();
            DockPanel dp = new DockPanel();
            w.Content = dp;
            w.Show();

            // add WFH to DockPanel
            dp.Children.Add(_wfh);

            // let any events play out
            WPFReflectBase.DoEvents();

            // perform operation, catch events
            //Utilities.ActiveFreeze(string.Format("before operation '{0}'", op.ToString()));
            WPFReflectBase.DoEvents();
            WFHUtil.ResetEvents();
            WFBUtil.ResetEvents();
            switch (op)
            {
                case Operation.Move:
                    // move the window left
                    w.Left = w.Left / 2;
                    WPFReflectBase.DoEvents();
                    p.log.WriteLine("Host Events when moving window '{0}'", WFHUtil.GetEvents());
                    p.log.WriteLine("Ctrl Events when moving window '{0}'", WFBUtil.GetEvents());
                    p.log.WriteLine("");
                    _expHostEvents = "";
                    WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
                    _expCtrlEvents = "";
                    WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);
                    break;

                case Operation.Resize:
                    // resize the window
                    w.Width = 200;
                    WPFReflectBase.DoEvents();
                    p.log.WriteLine("Host Events when resizing window '{0}'", WFHUtil.GetEvents());
                    p.log.WriteLine("Ctrl Events when resizing window '{0}'", WFBUtil.GetEvents());
                    p.log.WriteLine("");
                    _expHostEvents = "SizeChanged:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:LayoutUpdated:";
                    WPFMiscUtils.IncCounters(sr, _expHostEvents, WFHUtil.GetEvents(), "Did not get correct events", p.log);
                    _expCtrlEvents = "Invalidated:Layout:Resize:SizeChanged:ClientSizeChanged:Paint:";
                    WPFMiscUtils.IncCounters(sr, _expCtrlEvents, WFBUtil.GetEvents(), "Did not get correct events", p.log);
                    break;

                default:
                    throw (new Exception("Invalid operation specified"));
            }
            //Utilities.ActiveFreeze(string.Format("after operation '{0}'", op.ToString()));

            w.Close();
        }
        #endregion

        #region Windows Form
        public partial class Form1 : System.Windows.Forms.Form
        {
            public Form1()
            {
            }
        }
        #endregion

        // Added as a part of creating new AppDomain for Disposed issue
        public class Win : Window
        {
            Button _avBtn;
            public Win()
            {
                //host = new WindowsFormsHost();
                //host.Child = new System.Windows.Forms.MonthCalendar();
                //Content = host;
                //Height = 160;
                //Width = 200;
                //this.SizeChanged += new SizeChangedEventHandler(Win_SizeChanged);

                //DockPanel dp = new DockPanel();
                //this.Content = dp;
                StackPanel sp = new StackPanel();
                this.Content = sp;

                WindowsFormsHost wfh = new WindowsFormsHost();
                System.Windows.Forms.Button wfBtn = new System.Windows.Forms.Button();
                wfBtn.Text = "Winforms";
                wfh.Child = wfBtn;
                sp.Children.Add(wfh);

                Button avBtn2 = new Button();
                avBtn2.Content = "Avalon";
                sp.Children.Add(avBtn2);

                _avBtn = new Button();
                _avBtn.Content = "Status";
                sp.Children.Add(_avBtn);

                //wfBtn.GotFocus += new EventHandler(wfBtn_GotFocus);
                //wfBtn.LostFocus += new EventHandler(wfBtn_LostFocus);
                wfBtn.ChangeUICues += new System.Windows.Forms.UICuesEventHandler(wfBtn_ChangeUICues);
                wfBtn.Disposed += new EventHandler(wfBtn_Disposed);
            }

            void wfBtn_Disposed(object sender, EventArgs e)
            {
                scenarioParams.log.WriteLine("wfBtn_Disposed");
                //throw new Exception("wfBtn_Disposed");
            }

            void wfBtn_ChangeUICues(object sender, System.Windows.Forms.UICuesEventArgs e)
            {
                _avBtn.Content += " x";
                _avBtn.Background = System.Windows.Media.Brushes.Yellow;
            }

            void wfBtn_LostFocus(object sender, EventArgs e)
            {
                _avBtn.Background = System.Windows.Media.Brushes.Red;
            }

            void wfBtn_GotFocus(object sender, EventArgs e)
            {
                _avBtn.Background = System.Windows.Media.Brushes.Green;
            }
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify Events and Order when a WF control hosted in a WFH and an AV window is Created, Shown and Closed

//@ Verify Events and Order when a WF control is added and removed from a WFH

//@ Verify Events and Order when a WFH with a WF control is added and removed from a AV Window

//@ Verify Events and Order when a AV window with a WFH and WF control, is moved around

//@ Verify Events and Order when a AV window with a WFH and WF control, is resized

//@ Verify Events and Order when a AV window with a WFH and WF control, is maximized and minimized

//@ Verify Events and Order when a WFH and WF control, is covered and not covered.

//@ Verify Events and Order when a AV window with a WFH and WF control, shows an animation over the the WFH and WF control.  See TC:1072439

