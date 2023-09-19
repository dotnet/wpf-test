// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms.Integration;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Controls;


// Testcase:    Tabs
// Description: Verify that the Tab key work correctly across WF/AV
namespace WindowsFormsHostTests
{
    public class Tabs : WPFReflectBase
    {
        #region Testcase setup
        public Tabs(string[] args) : base(args) { }

        // class vars
        private string _appTitle = "Tabs TC";        // so that Mita can find it
        private DockPanel _dp;
        private string _events = string.Empty;       // string of logged Focus events

        protected override void InitTest(TParams p)
        {
            // make window a better size
            this.Width = 600;
            this.Height = 300;
            // set up for Mita
            this.Title = _appTitle;
            UseMITA = true;

            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            bool b = base.BeforeScenario(p, scenario);

            // debug - run specific scenario !!!
            //if (scenario.Name != "Scenario7") { return false; }

            // create DockPanel, add to window
            _dp = new DockPanel();
            this.Content = _dp;

            // run setup for particular scenario
            // must do all setup here because Scenarios are called on separate thread
            SetupScenario(p, scenario.Name);

            Utilities.SleepDoEvents(10);

            return b;
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("FW/RV tab between WF controls in a WFH (should wrap within a WFH - as long as there are no other avalon controls besides the WFH)")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(10);

            // first, try going forwards, starting with wfTB1
            // should follow sequence 1 2 4 3
            p.log.WriteLine("Tabbing forwards:");
            _events = "";                    // init event string
            FindAndFocus(p, "wfTB1");       // focus on first control

            PressKeys(p, "{TAB}", 5);
        Utilities.SleepDoEvents(10);

            // examine results
            // Note: Windows OS Regression_Bug44 used to happen here
            string expResults = "wfTB1:wfTB2:wfTB4:wfTB3:wfTB1:wfTB2:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // now, try going backwards, starting with wfTB1
            // should follow sequence 1 3 4 2
            p.log.WriteLine("Tabbing backwards:");
            _events = "";                    // init event string
            FindAndFocus(p, "wfTB1");       // focus on first control

            PressKeys(p, "+{TAB}", 5);

            // examine results
            // Note: Windows OS Regression_Bug44 used to happen here
            expResults = "wfTB1:wfTB3:wfTB4:wfTB2:wfTB1:wfTB3:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            return sr;
        }

        [Scenario("FW/RV tab between AV controls and WF controls in a WFH (should not wrap within a WFH)")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // first, try going forwards, starting with wfTB1
            // should follow sequence a1 a2 w1 w2 w4 w3
            p.log.WriteLine("Tabbing forwards:");
            _events = "";                    // init event string
            FindAndFocus(p, "avTB1");       // focus on first control

            PressKeys(p, "{TAB}", 10);

            // examine results
            string expResults = "avTB1:avTB2:wfTB1:wfTB2:wfTB4:wfTB3:avTB1:avTB2:wfTB1:wfTB2:wfTB4:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // now, try going backwards, starting with wfTB1
            // should follow sequence a1 w3 w4 w2 w1 a2
            p.log.WriteLine("Tabbing backwards:");
            _events = "";                    // init event string
            FindAndFocus(p, "avTB1");       // focus on first control

            PressKeys(p, "+{TAB}", 10);

            // examine results
            expResults = "avTB1:wfTB3:wfTB4:wfTB2:wfTB1:avTB2:avTB1:wfTB3:wfTB4:wfTB2:wfTB1:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            return sr;
        }

        [Scenario("Controls that eat tabs (textbox that has acceptstab= true, DGV) verify that you can get out with CTRL+TAB between WFC in a single WFH")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // first, try going forwards, starting with wfTB1
            // tab sequence is: w1 w2 w4 w3
            // but wfTB2 eats tabs
            p.log.WriteLine("Tabbing forwards:");
            _events = "";                    // init event string
            FindAndFocus(p, "wfTB1");       // focus on first control

            // verify initial text in wfTB2
                p.log.WriteLine("Current text in wfTB2 is '{0}'", GetTextboxText(p, "wfTB2"));
            string expStr = "TB2 - Index 1 AT";
            string actStr = GetTextboxText(p, "wfTB2");
            WPFMiscUtils.IncCounters(sr, expStr, actStr, "Textbox Text not as expected", p.log);

            // press TAB 2 times - should get stuck in wfTB2
            PressKeys(p, "{TAB}", 2);
            string expResults = "wfTB1:wfTB2:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // verify text in wfTB2 - should have been replaced with {TAB}
            // (it seems default TestBox edit setting is "select all" which causes replacement of contents)
            p.log.WriteLine("Current text in wfTB2 is '{0}'", GetTextboxText(p, "wfTB2"));
            expStr = "\t";
            actStr = GetTextboxText(p, "wfTB2");
            WPFMiscUtils.IncCounters(sr, expStr, actStr, "Textbox Text not as expected", p.log);

            // now try CTRL-TAB to get out of wfTB2 and get to wfTB4
            PressKeys(p, "^{TAB}", 1);
            expResults = "wfTB1:wfTB2:wfTB4:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // now try TAB again to go to wfTB3
            PressKeys(p, "{TAB}", 1);
            expResults = "wfTB1:wfTB2:wfTB4:wfTB3:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // replace text in wfTB2
            FindAndFocus(p, "wfTB2");
            PressKeys(p, "Hi Nate!", 1);

            // now, try going backwards, starting with wfTB1
            // reverse tab sequence is: w1 w3 w4 w2
            // but wfTB2 eats tabs
            p.log.WriteLine("Tabbing backwards:");
            _events = "";                    // init event string
            FindAndFocus(p, "wfTB1");       // focus on first control

            // verify initial text in wfTB2
            p.log.WriteLine("Current text in wfTB2 is '{0}'", GetTextboxText(p, "wfTB2"));
            expStr = "\tHi Nate!";
            actStr = GetTextboxText(p, "wfTB2");
            WPFMiscUtils.IncCounters(sr, expStr, actStr, "Textbox Text not as expected", p.log);

            // press SHIFT-TAB 4 times - should get stuck in wfTB2
            // Note: Windows OS Regression_Bug44 used to happen here
            PressKeys(p, "+{TAB}", 4);
            expResults = "wfTB1:wfTB3:wfTB4:wfTB2:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // verify text in wfTB2 - should have been replaced with {SHIFT-TAB}
            p.log.WriteLine("Current text in wfTB2 is '{0}'", GetTextboxText(p, "wfTB2"));
            expStr = "\tHi Nate!\t";
            actStr = GetTextboxText(p, "wfTB2");
            WPFMiscUtils.IncCounters(sr, expStr, actStr, "Textbox Text not as expected", p.log);

            // now try CTRL-SHIFT-TAB to get out of wfTB2 and get to wfTB1
            // Note: Windows OS Regression_Bug44 used to happen here
            PressKeys(p, "^+{TAB}", 1);
            expResults = "wfTB1:wfTB3:wfTB4:wfTB2:wfTB1:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // now try SHIFT-TAB again to go to wfTB3
            // Note: Windows OS Regression_Bug44 used to happen here
            PressKeys(p, "+{TAB}", 1);
            expResults = "wfTB1:wfTB3:wfTB4:wfTB2:wfTB1:wfTB3:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            return sr;
        }

        [Scenario("Controls that eat tabs (textbox that has acceptstab= true, DGV) verify that you can get out with CTRL+TAB between WFC to an Avalon Control")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // first, try going forwards, starting with wfTB1
            // tab sequence is: a1 a2 w1 w2 w4 w3
            // but wfTB2 eats tabs
            p.log.WriteLine("Tabbing forwards:");
            _events = "";                    // init event string
            FindAndFocus(p, "avTB1");       // focus on first control

            // verify initial text in wfTB2
            p.log.WriteLine("Current text in wfTB2 is '{0}'", GetTextboxText(p, "wfTB2"));
            string expStr = "TB2 - Index 1 AT";
            string actStr = GetTextboxText(p, "wfTB2");
            WPFMiscUtils.IncCounters(sr, expStr, actStr, "Textbox Text not as expected", p.log);

            // press TAB 4 times - should get stuck in wfTB2
            PressKeys(p, "{TAB}", 4);
            string expResults = "avTB1:avTB2:wfTB1:wfTB2:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // verify text in wfTB2 - should have been replaced with {TAB}
            // (it seems default TestBox edit setting is "select all" which causes replacement of contents)
            p.log.WriteLine("Current text in wfTB2 is '{0}'", GetTextboxText(p, "wfTB2"));
            expStr = "\t";
            actStr = GetTextboxText(p, "wfTB2");
            WPFMiscUtils.IncCounters(sr, expStr, actStr, "Textbox Text not as expected", p.log);

            // now try CTRL-TAB to get out of wfTB2 and get to wfTB4
            PressKeys(p, "^{TAB}", 1);
            expResults = "avTB1:avTB2:wfTB1:wfTB2:wfTB4:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // now try TAB again to go to wfTB3
            PressKeys(p, "{TAB}", 1);
            expResults = "avTB1:avTB2:wfTB1:wfTB2:wfTB4:wfTB3:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            return sr;
        }

        [Scenario("Verify that WF controls that have TabStop=false do not get tabs.")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            _events = "";                    // init event string
            FindAndFocus(p, "wfTB1");       // focus on first control

            PressKeys(p, "{TAB}", 5);

            // examine results (should skip wfTB4)
            // Note: Windows OS Regression_Bug44 used to happen here
            string expResults = "wfTB1:wfTB2:wfTB3:wfTB1:wfTB2:wfTB3:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            return sr;
        }

        [Scenario("FW and/or RV tab between WF controls in a WFH ")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            //Utilities.ActiveFreeze("xxx");
            // going forwards twice, backward 3 times, forward 4 times and backward 5 times starting with wfTB1
            // should follow sequence 1 2 4 -> 2 1 3 -> 2 1 4 3 -> 4 2 1 3 4 

            p.log.WriteLine("Tabbing forwards:");
            _events = "";                    // init event string
            FindAndFocus(p, "wfTB1");       // focus on first control

            PressKeys(p, "{TAB}", 2);

            // examine results
            string expResults = "wfTB1:wfTB2:wfTB4:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // now, try going backwards ( 3 Tabs)
            p.log.WriteLine("Tabbing backwards:");
            _events = "";                    // init event string

            PressKeys(p, "+{TAB}", 3);

            // examine results
            // Note: Windows OS Regression_Bug44 used to happen here
            expResults = "wfTB2:wfTB1:wfTB3:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);


            // now, try going forwards ( 4 Tabs)
            p.log.WriteLine("Tabbing forwards:");
            _events = "";                    // init event string

            PressKeys(p, "{TAB}", 4);

            // examine results
            // Note: Windows OS Regression_Bug44 used to happen here
            expResults = "wfTB1:wfTB2:wfTB4:wfTB3:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            // now, try going backwards ( 5 Tabs)
            p.log.WriteLine("Tabbing backwards:");
            _events = "";                    // init event string

            PressKeys(p, "+{TAB}", 5);

            // examine results
            // Note: Windows OS Regression_Bug44 used to happen here
            expResults = "wfTB4:wfTB2:wfTB1:wfTB3:wfTB4:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events", p.log);

            return sr;
        }

        [Scenario("Windows OS Regression_Bug45 - Verify that WF controls in a WFH get Enter/GotFocus/LostFocus events")]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            p.log.WriteLine("Testing Focus events:");
        
            FindAndFocus(p, "wfTB1");       // focus on first control
            _events = "";                    // init event string
            PressKeys(p, "{TAB}", 1);       // tab into second control

            // examine results
            string expResults = "Enter:GotFocus:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events Entering WF control", p.log);

            _events = "";                    // init event string
            PressKeys(p, "{TAB}", 1);       // tab out of second control

            // examine results
            expResults = "Leave:LostFocus:";
            p.log.WriteLine("Expected events = '{0}'", expResults);
            p.log.WriteLine("Actual   events = '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, expResults, _events, "Did not get expected events Leaving WF control", p.log);

            return sr;
        }

        #region Scenario setup

        /// <summary>
        /// Function to set up window for a particular Scenario
        /// </summary>
        /// <param name="p"></param>
        /// <param name="scenario"></param>
        private void SetupScenario(TParams p, string scenario)
        {
            // create controls in window for particular Scenario

            // which scenario? (I think this is cleaner than "switch")
            if (scenario == "Scenario1")
            {
                // FW/RV tab between WF controls in a WFH (should wrap within a WFH - as long as there are no other avalon controls besides the WFH)
                CreateHostWithKids(p, false, false);
            }
            else if (scenario == "Scenario2")
            {
                // FW/RV tab between AV controls and WF controls in a WFH (should not wrap within a WFH)
                CreateAvalonControls(p);
                CreateHostWithKids(p, false, false);
            }
            else if (scenario == "Scenario3")
            {
                // Controls that eat tabs (textbox that has acceptstab= true, DGV) verify that you can get out with CTRL+TAB between WFC in a single WFH
                CreateHostWithKids(p, true, false);
            }
            else if (scenario == "Scenario4")
            {
                // Controls that eat tabs (textbox that has acceptstab= true, DGV) verify that you can get out with CTRL+TAB between WFC to an Avalon Control
                CreateAvalonControls(p);
                CreateHostWithKids(p, true, false);
            }
            else if (scenario == "Scenario5")
            {
                // Verify that WF controls that have TabStop=false do not get tabs.
                CreateHostWithKids(p, false, true);
            }
            else if (scenario == "Scenario6")
            {
                // FW and/or RV tab between WF controls in a WFH 
                CreateHostWithKids(p, false, false);
            }
            else if (scenario == "Scenario7")
            {
                // Windows OS Regression_Bug45 - Verify that WF controls in a WFH get Enter/GotFocus/LostFocus events
                CreateHostWithKids2(p);
            }
            else
            {
                // Did someone add a new Scenario?
                throw new ArgumentException("Unexpected Scenario name '{0}'", scenario);
            }
        }

        /// <summary>
        /// Helper to create 2 Avalon controls and add to DockPanel
        /// </summary>
        /// <param name="p"></param>
        private void CreateAvalonControls(TParams p)
        {
            p.log.WriteLine("Adding 2 Avalon TextBoxes");

            // create 2 Avalon TextBoxes
            TextBox avTB1 = new TextBox();
            avTB1.Name = "avTB1";
            avTB1.TabIndex = 0;
            avTB1.Text = "ATB1 - Tab Index 0";
            avTB1.GotFocus += new RoutedEventHandler(this.GotFocusAV);

            TextBox avTB2 = new TextBox();
            avTB2.Name = "avTB2";
            avTB2.TabIndex = 1;
            avTB2.Text = "ATB2 - Tab Index 1";
            avTB2.GotFocus += new RoutedEventHandler(this.GotFocusAV);

            // add TextBoxes to DockPanel
            _dp.Children.Add(avTB1);
            _dp.Children.Add(avTB2);
        }

        /// <summary>
        /// Helper to create WFH with FlowLayoutPanel with 4 WinForms TextBoxes and add to DockPanel
        /// </summary>
        /// <param name="p"></param>
        /// <param name="bAcceptsTabs">Make one TextBox Accept Tabs?</param>
        /// <param name="bNoTabstop">Make one TextBox have no Tab Stop?</param>
        private void CreateHostWithKids(TParams p, bool bAcceptsTabs, bool bNoTabstop)
        {
            p.log.WriteLine("Adding WFH with FLP with 4 winForms TextBoxes");

            // create WFH with FlowLayoutPanel with 4 WF Textboxes
            WindowsFormsHost wfh = new WindowsFormsHost();

            // TextBox 1
            System.Windows.Forms.TextBox wfTB1 = new System.Windows.Forms.TextBox();
            wfTB1.Name = "wfTB1";
            wfTB1.TabIndex = 0;
            wfTB1.Text = "TB1 - Index 0";
            wfTB1.GotFocus += new EventHandler(this.GotFocusWF);

            // TextBox 2
            System.Windows.Forms.TextBox wfTB2 = new System.Windows.Forms.TextBox();
            wfTB2.Name = "wfTB2";
            wfTB2.TabIndex = 1;
            wfTB2.Text = "TB2 - Index 1";
            wfTB2.GotFocus += new EventHandler(this.GotFocusWF);

            // TextBox 3
            System.Windows.Forms.TextBox wfTB3 = new System.Windows.Forms.TextBox();
            wfTB3.Name = "wfTB3";
            wfTB3.TabIndex = 3;
            wfTB3.Text = "TB3 - Index 3";
            wfTB3.GotFocus += new EventHandler(this.GotFocusWF);

            // TextBox 4
            System.Windows.Forms.TextBox wfTB4 = new System.Windows.Forms.TextBox();
            wfTB4.Name = "wfTB4";
            wfTB4.TabIndex = 2;
            wfTB4.Text = "TB4 - Index 2";
            wfTB4.GotFocus += new EventHandler(this.GotFocusWF);

            // accepts tabs?
            if (bAcceptsTabs)
            {
                // "AcceptsTabs" is only valid for "Multiline" controls,
                // so we must make this TextBox Multiline=true
                p.log.WriteLine("Setting TextBox 2 AcceptsTab = true (and Multiline = true)");
                wfTB2.Multiline = true;
                wfTB2.AcceptsTab = true;
                wfTB2.Text = "TB2 - Index 1 AT";
            }

            if (bNoTabstop)
            {
                p.log.WriteLine("Setting TextBox 4 TabStop = false");
                wfTB4.TabStop = false;
                wfTB4.Text = "TB4 - Index 2 NS";
            }

            // add TB's to FLP
            System.Windows.Forms.FlowLayoutPanel flp = new System.Windows.Forms.FlowLayoutPanel();
            flp.Controls.AddRange(new System.Windows.Forms.Control[] { wfTB1, wfTB2, wfTB3, wfTB4 });

            // add FLP to WFH
            wfh.Child = flp;

            // add WFH to window
            _dp.Children.Add(wfh);
        }

        private void CreateHostWithKids2(TParams p)
        {
            p.log.WriteLine("Adding WFH with FLP with 4 winForms TextBoxes");

            // create WFH with FlowLayoutPanel with 4 WF Textboxes
            WindowsFormsHost wfh = new WindowsFormsHost();

            // TextBox 1
            System.Windows.Forms.TextBox wfTB1 = new System.Windows.Forms.TextBox();
            wfTB1.Name = "wfTB1";
            wfTB1.TabIndex = 0;
            wfTB1.Text = "TB1 - Index 0";

            // TextBox 2
            System.Windows.Forms.TextBox wfTB2 = new System.Windows.Forms.TextBox();
            wfTB2.Name = "wfTB2";
            wfTB2.TabIndex = 1;
            wfTB2.Text = "TB2 - Index 1";

            // add additional event handlers needed for this Scenario
            p.log.WriteLine("Adding additional event handlers to TB2");
            wfTB2.Enter += new EventHandler(wfTB_Enter);
            wfTB2.Leave += new EventHandler(wfTB_Leave);
            wfTB2.GotFocus += new EventHandler(wfTB_GotFocus);
            wfTB2.LostFocus += new EventHandler(wfTB_LostFocus);

            // TextBox 3
            System.Windows.Forms.TextBox wfTB3 = new System.Windows.Forms.TextBox();
            wfTB3.Name = "wfTB3";
            wfTB3.TabIndex = 3;
            wfTB3.Text = "TB3 - Index 3";

            // TextBox 4
            System.Windows.Forms.TextBox wfTB4 = new System.Windows.Forms.TextBox();
            wfTB4.Name = "wfTB4";
            wfTB4.TabIndex = 2;
            wfTB4.Text = "TB4 - Index 2";

            // add TB's to FLP
            System.Windows.Forms.FlowLayoutPanel flp = new System.Windows.Forms.FlowLayoutPanel();
            flp.Controls.AddRange(new System.Windows.Forms.Control[] { wfTB1, wfTB2, wfTB3, wfTB4 });

            // add FLP to WFH
            wfh.Child = flp;

            // add WFH to window
            _dp.Children.Add(wfh);
        }

        void wfTB_Enter(object sender, EventArgs e)
        {
            _events += "Enter:";
        }

        void wfTB_Leave(object sender, EventArgs e)
        {
            _events += "Leave:";
        }

        void wfTB_GotFocus(object sender, EventArgs e)
        {
            _events += "GotFocus:";
        }

        void wfTB_LostFocus(object sender, EventArgs e)
        {
            _events += "LostFocus:";
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Event handler for Avalon controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotFocusAV(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            _events += c.Name + ":";
        }

        /// <summary>
        /// Event handler for WinForms controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotFocusWF(object sender, EventArgs e)
        {
            System.Windows.Forms.Control c = (System.Windows.Forms.Control)sender;
            _events += c.Name + ":";
        }

        /// <summary>
        /// Helper function to locate a control by name and set focus to it
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ctrlName"></param>
        private void FindAndFocus(TParams p, string ctrlName)
        {
            p.log.WriteLine("FindAndFocus: '{0}'", ctrlName);

            try
            {
                // find app window
                UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(_appTitle));
                //UIObject uiApp = UIObject.Root.Descendants.Find(UICondition.CreateFromName(appTitle));
                p.log.WriteLine("Found app: uiApp = '{0}'", uiApp);

                // find control by name
                UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(ctrlName));
                p.log.WriteLine("Found control: ctrl = '{0}'", ctrl);

                // focus on control
                ctrl.SetFocus();
            }
            catch (Exception e)
            {
                p.log.WriteLine("Got exception '{0}'", e.Message);
            }
        }

        /// <summary>
        /// Helper function to query text from TextBox control by name. Uses Mita Magic to avoid cross thread error
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ctrlName"></param>
        /// <returns></returns>
        private string GetTextboxText(TParams p, string ctrlName)
        {
            string text = String.Empty;

            try
            {
                // find app window
                UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(_appTitle));

                // find control by name
                UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(ctrlName));

                // get text from textbox
                Edit edit1 = new Edit(ctrl);

                //text = edit1.Value;
                text = edit1.DocumentRange.GetText(-1);
            }
            catch (Exception e)
            {
                p.log.WriteLine("Got exception '{0}'", e.Message);
            }

            return text;
        }

        /// <summary>
        /// Helper function used to simulate key presses
        /// </summary>
        /// <param name="p"></param>
        /// <param name="key">string of key(s) to press</param>
        /// <param name="count">number of times to press keys</param>
        private void PressKeys(TParams p, string key, int count)
        {
            for (int i = 0; i < count; i++)
            {
                // send the keystrokes
                Keyboard.Instance.SendKeys(key);
                Utilities.SleepDoEvents(2);

                // pause
                //System.Threading.Thread.Sleep(500);
            }
        }
        #endregion

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ FW/RV tab between WF controls in a WFH (should wrap within a WFH - as long as there are no other avalon controls besides the WFH)
//@ FW/RV tab between AV controls and WF controls in a WFH (should not wrap within a WFH)
//@ Controls that eat tabs (textbox that has acceptstab= true, DGV) verify that you can get out with CTRL+TAB between WFC in a single WFH
//@ Controls that eat tabs (textbox that has acceptstab= true, DGV) verify that you can get out with CTRL+TAB between WFC to an Avalon Control
//@ Verify that WF controls that have TabStop=false do not get tabs.
//@ FW and/or RV tab between WF controls in a WFH 
//@ Windows OS Regression_Bug45 - Verify that WF controls in a WFH get Enter/GotFocus/LostFocus events
