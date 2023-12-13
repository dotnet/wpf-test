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

//
// Testcase:    MouseFocus
// Description: Verify that clicking on either WF or AV control, Focus is correct
//
namespace WindowsFormsHostTests
{
    public class MouseFocus : WPFReflectBase
    {
        #region Testcase setup
        
        Edit _edit1;
        Edit _edit2;
        SWC.StackPanel _stackPanel = new SWC.StackPanel();
        WindowsFormsHost _winformsHost1 = new WindowsFormsHost();
        SWF.Button _wfButton1 = new SWF.Button();
        SWF.Button _wfButton2 = new SWF.Button();
        SWF.FlowLayoutPanel _panel = new SWF.FlowLayoutPanel();
        SWC.Button _avButton1 = new SWC.Button();
        SWC.Button _avButton2 = new SWC.Button();
        SWC.Label _label1 = new SWC.Label();
        SWC.Label _label2 = new SWC.Label();
        String _labelText1;
        String _labelText2;
        SW.Window _window = new SW.Window();

        public MouseFocus(string[] args) : base(args) { }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            if (scenario.Name == "Scenario1")
            {
                _avButton1.Name = "avButton1";
                _avButton1.Content = "Avalon Button";
                _avButton1.GotFocus += new System.Windows.RoutedEventHandler(avButton1_GotFocus);
                _avButton1.LostFocus += new System.Windows.RoutedEventHandler(avButton1_LostFocus);
                _stackPanel.Children.Add(_avButton1);
            }

            if (scenario.Name == "Scenario2")
            {
                _avButton2.Name = "avButton2";
                _avButton2.Content = "AvalonWindow";
                _avButton2.GotKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(avButton2_GotFocus);
                _avButton2.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(avButton2_LostFocus);
                _window.Content = _avButton2;

                _window.Name = "AvalonWindow";
                _window.Title = "AvalonWindow";
                _window.SizeToContent = SW.SizeToContent.WidthAndHeight;
                _window.Top = 0;
                _window.Left = 0;
                _window.Topmost = true;
                _window.Show();

                Utilities.SleepDoEvents(10);
            }

            if (scenario.Name == "Scenario3" || scenario.Name == "Scenario4")
            {
                _panel.Controls.Clear();

                _wfButton2.Name = "wfButton2";
                _wfButton2.Text = "WinFormButton2";
                _wfButton2.GotFocus += new EventHandler(wfButton2_GotFocus);
                _wfButton2.LostFocus += new EventHandler(wfButton2_LostFocus);

                _winformsHost1.Child = _panel;
                _panel.Controls.Add(_wfButton1);
                _panel.Controls.Add(_wfButton2);
            }

            if (scenario.Name == "Scenario4")
            {
                _wfButton2.Capture = true;
            }
            return base.BeforeScenario(p, scenario);
        }

        void wfButton2_LostFocus(object sender, EventArgs e)
        {
            _label2.Content = _labelText2 = "WinFormButton2 lost focus.";
        }

        void wfButton2_GotFocus(object sender, EventArgs e)
        {
            _label1.Content = _labelText1 = "WinFormButton2 got focus.";
        }

        void avButton2_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _label2.Content = _labelText2 = "Avalon Button lost focus.";
        }

        void avButton2_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _label1.Content = _labelText1 = "Avalon Button got focus.";
        }

        protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
        {
            if (scenario.Name == "Scenario1")
            {
                _stackPanel.Children.Remove(_avButton1);
            }
            if (scenario.Name == "Scenario2")
            {
                _window.Close();
            }
            _label1.Content = "";
            _label2.Content = "";
            _labelText1 = "";
            _labelText2 = "";
            base.AfterScenario(p, scenario, result);
        }

        protected override void InitTest(TParams p) 
        {
            this.Title = "MouseFocus";
            this.UseMITA = true;
            this.Topmost = true;

            _label1.Height = 30;
            _label2.Height = 30;

            _wfButton1.Name = "wfButton1";
            _wfButton1.Text = "WinForm Button";
            _wfButton1.GotFocus += new EventHandler(wfButton1_GotFocus);
            _wfButton1.LostFocus += new EventHandler(wfButton1_LostFocus);
            _winformsHost1.Child = _wfButton1;

            _stackPanel.Children.Add(_label1);
            _stackPanel.Children.Add(_label2);
            _stackPanel.Children.Add(_winformsHost1);

            this.Content = _stackPanel;

            base.InitTest(p);
        }

        void avButton1_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _label2.Content = _labelText2 = "Avalon Button lost focus.";
        }

        void avButton1_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _label1.Content = _labelText1 = "Avalon Button got focus.";
        }

        void wfButton1_LostFocus(object sender, EventArgs e)
        {
            _label2.Content = _labelText2 = "WinForm Button lost focus.";
        }

        void wfButton1_GotFocus(object sender, EventArgs e)
        {
            _label1.Content = _labelText1 = "WinForm Button got focus.";
        }
        #endregion


        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios

        [Scenario("Click back and forth between a WF and AV control and verify that Focus is correct.")]
        public ScenarioResult Scenario1(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetEditControls(p, "MouseFocus", "wfButton1", "MouseFocus", "avButton1"))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(10);

            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == "WinForm Button got focus.",
                "Failed at GotFocus on WinForm button.\nExpected: WinForm Button got focus." +
                "\nActual: " + _labelText1);

            _edit2.Click(PointerButtons.Primary); //click on Avalon button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus, 
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == "Avalon Button got focus.",
                "Failed at GotFocus on Avalon button.\nExpected: Avalon Button got focus." +
                "\nActual: " + _labelText1);

            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "Avalon Button lost focus.",
                "Failed at LostFocus on Avalon button.\nExpected: Avalon Button lost focus." +
                "\nActual: " + _labelText2);

            _edit2.Click(PointerButtons.Primary); //click on Avalon button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "WinForm Button lost focus.",
                "Failed at LostFocus on WinForm button.\nExpected: WinForm Button lost focus." + 
                "\nActual: " + _labelText2);

            Utilities.SleepDoEvents(10);

            return sr; 
        }

        [Scenario("Click back and forth between a AV control on a separate Window and an WF control and verify that focus is correct.")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetEditControls(p, "MouseFocus", "wfButton1", "AvalonWindow", "avButton2"))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(10);

            _edit1.SetFocus();
            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == "WinForm Button got focus.",
                "Failed at GotFocus on WinForm button.\nExpected: WinForm Button got focus." +
                "\nActual: " + _labelText1);

            _edit2.Click(PointerButtons.Primary); //click on Avalon button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == "Avalon Button got focus.",
                "Failed at GotFocus on Avalon button.\nExpected: Avalon Button got focus." +
                "\nActual: " + _labelText1);

            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "Avalon Button lost focus.",
                "Failed at LostFocus on Avalon button.\nExpected: Avalon Button lost focus." +
                "\nActual: " + _labelText2);

            _edit2.Click(PointerButtons.Primary); //click on Avalon button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "WinForm Button lost focus.",
                "Failed at LostFocus on WinForm button.\nExpected: WinForm Button lost focus." +
                "\nActual: " + _labelText2);

            Utilities.SleepDoEvents(10);


            return sr;
        }

        [Scenario("Click back and forth between 2 WF controls in the same WFH and verify that Focus is correct.")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetEditControls(p, "MouseFocus", "wfButton1", "MouseFocus", "wfButton2"))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(10);

            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == "WinForm Button got focus.",
                "Failed at GotFocus on WinForm button.\nExpected: WinForm Button got focus." +
                "\nActual: " + _labelText1);

            _edit2.Click(PointerButtons.Primary); //click on WinFormButton2
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == "WinFormButton2 got focus.",
                "Failed at GotFocus on WinFormButton2.\nExpected: WinFormButton2 got focus." +
                "\nActual: " + _labelText1);

            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "WinFormButton2 lost focus.",
                "Failed at LostFocus on WinFormButton2.\nExpected: WinFormButton2 lost focus." +
                "\nActual: " + _labelText2);

            _edit2.Click(PointerButtons.Primary); //click on WinFormButton2
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "WinForm Button lost focus.",
                "Failed at LostFocus on WinForm button.\nExpected: WinForm Button lost focus." +
                "\nActual: " + _labelText2);

            Utilities.SleepDoEvents(10);

            return sr;
        }

        [Scenario("MouseCapture Control.Capture should work.")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            
            if (!GetEditControls(p, "MouseFocus", "wfButton1", "MouseFocus", "wfButton2"))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(10);

            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(10);

            //wfButton2.Capture has been set to true
            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus == false,
                "Failed at wfButton1.HasKeyboardFocus==false. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == "WinFormButton2 got focus.",
                "Failed at GotFocus on WinFormButton2.\nExpected: WinFormButton2 got focus." +
                "\nActual: " + _labelText1);

            _edit2.Click(PointerButtons.Primary); //click on WinFormButton2
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == "WinFormButton2 got focus.",
                "Failed at GotFocus on WinFormButton2.\nExpected: WinFormButton2 got focus." +
                "\nActual: " + _labelText1);

            _edit1.Click(PointerButtons.Primary); //click on WinForm button
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "WinFormButton2 lost focus.",
                "Failed at LostFocus on WinFormButton2.\nExpected: WinFormButton2 lost focus." +
                "\nActual: " + _labelText2);

            _edit2.Click(PointerButtons.Primary); //click on WinFormButton2
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "WinForm Button lost focus.",
                "Failed at LostFocus on WinForm button.\nExpected: WinForm Button lost focus." +
                "\nActual: " + _labelText2);

            Utilities.SleepDoEvents(10);

            return sr;
        }
        #endregion

        #region Helper Functions 

        bool GetEditControls(TParams p, string window1, string control1, string window2, string control2)
        {
            try
            {
                UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window1));
                BreadthFirstDescendantsNavigator bfTB1 = new BreadthFirstDescendantsNavigator(uiApp, UICondition.CreateFromId(control1));
                uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window2));
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
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Click back and forth between a WF and AV control and verify that Focus is correct.
//@ Click back and forth between a AV control on a separate Window and an WF control and verify that focus is correct.
//@ Click back and forth between 2 WF controls in the same WFH and verify that Focus is correct.
//@ MouseCapture Control.Capture should work.
