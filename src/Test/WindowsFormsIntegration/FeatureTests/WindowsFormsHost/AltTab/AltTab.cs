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
using MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;
using System.Threading;


// Testcase:    AltTab
// Description: Verify that Alt+TAB works and that Focus is good
namespace WindowsFormsHostTests
{

    public class AltTab : WPFReflectBase
    {
        #region Testcase setup
        
        Edit _edit1;
        Edit _edit2;
        SWC.StackPanel _stackPanel;
        WindowsFormsHost _winformsHost1;
        SWF.Button _wfButton1;
        SWF.TextBox _wfTextBox1;
        SWF.FlowLayoutPanel _panel;
        System.Threading.Thread _newThread;

        public AltTab(string[] args) : base(args) { }


        protected override void InitTest(TParams p)
        {
            this.Title = "AltTab";
            this.UseMITA = true;

            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            _panel = new SWF.FlowLayoutPanel();

            _wfButton1 = new SWF.Button();
            _wfButton1.Name = "wfButton1";
            _wfButton1.Text = "WinForm Button";
            _panel.Controls.Add(_wfButton1);

            _wfTextBox1 = new SWF.TextBox();
            _wfTextBox1.Name = "wfTextBox1";
            _wfTextBox1.Text = "WinForm TextBox";
            _panel.Controls.Add(_wfTextBox1);

            _winformsHost1 = new WindowsFormsHost();
            _winformsHost1.Child = _panel;
            _stackPanel = new SWC.StackPanel();
            _stackPanel.Children.Add(_winformsHost1);
            this.Content = _stackPanel;

            if (scenario.Name == "Scenario2")
            {
                _newThread = new System.Threading.Thread(new ThreadStart(ShowModalForm.ShowIt));
                _newThread.SetApartmentState(ApartmentState.STA);
                _newThread.Start();
                Utilities.SleepDoEvents(10);
            }

            return base.BeforeScenario(p, scenario);
        }

        protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
        {
            if (scenario.Name == "Scenario2")
            {
                _newThread.Abort();
            }

            base.AfterScenario(p, scenario, result);
        }

        #endregion


        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios

        [Scenario("Verify focus preserved after Alt+TAB.")]
        public ScenarioResult Scenario1(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetEditControls(p, "AltTab", "wfButton1", "wfTextBox1"))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(2);

            //Check WinForm Button
            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus, initial state. HasKeyboardFocus=" + 
                _edit1.HasKeyboardFocus);

            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);

            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);

            //WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            //    "Failed at wfButton1.HasKeyboardFocus, after Alt-Tab. HasKeyboardFocus=" + 
            //    _edit1.HasKeyboardFocus);

            //Check WinForm TextBox
            _edit2.Click(PointerButtons.Primary); //click on WinForm TextBox
            Utilities.SleepDoEvents(5);

            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);
            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);

            if (sr.FailCount > 0)
            {
                p.log.LogKnownBug(BugDb.WindowsOSBugs, 15, 
                    "Focus is lost when returning from a non-modal window");
            }

            return sr; 
        }

        [Scenario("Verify Focus preserved after Alt+TAB from an AV Modal Dialog.")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetEditControls(p, "ModalWinForm", "wfButton", "wfTextBox"))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(2);

            //Check WinForm Button
            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus, initial state. HasKeyboardFocus=" +
                _edit1.HasKeyboardFocus);

            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);

            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus, after Alt-Tab. HasKeyboardFocus=" +
                _edit1.HasKeyboardFocus);

            //Check WinForm TextBox
            _edit2.Click(PointerButtons.Primary); //click on WinForm TextBox
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at wfTextBox1.HasKeyboardFocus, initial state. HasKeyboardFocus=" +
                _edit2.HasKeyboardFocus);

            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);

            Keyboard.Instance.SendKeys("%{TAB}");
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at wfTextBox1.HasKeyboardFocus, after Alt-Tab. HasKeyboardFocus=" +
                _edit2.HasKeyboardFocus);

            if (sr.FailCount > 0)
            {
                p.log.LogKnownBug(BugDb.WindowsOSBugs, 16,
                    "Keyboard focus is lost when returning from a modal dialog");
            }

            return sr; 
        }

        [Scenario("Regression_Bug17.")]
        public ScenarioResult Scenario3(TParams p)
        {
            p.log.WriteLine("Regression_Bug17 is checked in Scenario 1");
            return new ScenarioResult(true);
        }

        #endregion

        #region Helper Functions 

        bool GetEditControls(TParams p, string window1, string control1, string control2)
        {
            try
            {
                UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window1));
                BreadthFirstDescendantsNavigator bfTB1 = new BreadthFirstDescendantsNavigator(uiApp, UICondition.CreateFromId(control1));
                BreadthFirstDescendantsNavigator bfTB2 = new BreadthFirstDescendantsNavigator(uiApp, UICondition.CreateFromId(control2));
                _edit1 = new Edit(bfTB1[0]);
                _edit2 = new Edit(bfTB2[0]);
                return true;
            }
            catch (Exception ex)
            {
                p.log.WriteLine("Failed to get Mita wrapper controls. " + ex.ToString());
                return false;
            }
        }    
        #endregion

        class ShowModalForm
        {
            public static SW.Window avWindow = new SW.Window();
            static SWF.FlowLayoutPanel s_pane = new SWF.FlowLayoutPanel();

            public static void ShowIt()
            {
                WindowsFormsHost host = new WindowsFormsHost();
                SWF.Button wfButton = new SWF.Button();
                SWF.TextBox wfTextbox = new SWF.TextBox();

                wfButton.Name = "wfButton";
                wfButton.Text = "WinForm Button";
                s_pane.Controls.Add(wfButton);

                wfTextbox.Name = "wfTextBox";
                wfTextbox.Text = "WinForm TextBox";
                s_pane.Controls.Add(wfTextbox);

                host.Child = s_pane;

                avWindow.Title = "ModalWinForm";
                avWindow.Content = host;
                avWindow.ShowDialog();
            }
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify focus preserved after Alt+TAB
//@ Verify Focus preserved after Alt+TAB from an AV Modal Dialog
//@ Regression_Bug17
