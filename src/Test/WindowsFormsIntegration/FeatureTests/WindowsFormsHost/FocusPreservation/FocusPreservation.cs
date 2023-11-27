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
// Testcase:    FocusPreservation
// Description: Verify that a controls focus is preserved when switching to AV controls
//
namespace WindowsFormsHostTests
{
    public class FocusPreservation : WPFReflectBase
    {
        #region Testcase setup

        Edit _edit1;
        Edit _edit2;
        Edit _edit3;
        Edit _edit4;
        Edit _edit5;
        Edit _edit6;
        Edit _form;

        SWC.StackPanel _stackPanel = new SWC.StackPanel();
        WindowsFormsHost _winformsHost1 = new WindowsFormsHost();
        SWF.Button _wfButton1 = new SWF.Button();
        SWF.Button _wfButton2 = new SWF.Button();
        SWF.TextBox _wfTextBox1 = new SWF.TextBox();
        SWF.FlowLayoutPanel _panel = new SWF.FlowLayoutPanel();
        SWC.Button _avButton1 = new SWC.Button();
        SWC.Button _avButton2 = new SWC.Button();
        SWC.Label _label1 = new SWC.Label();
        SWC.Label _label2 = new SWC.Label();
        SWC.Label _label3 = new SWC.Label();
        String _labelText1 = "";
        String _labelText2 = "";
        String _labelText3 = "";
        SW.Window _win;

        private const string wfButton1GotFocus = "WinForm Button 1 Got Focus";
        private const string wfButton1LostFocus = "WinForm Button 1 Lost Focus";
        private const string wfButton2GotFocus = "WinForm Button 2 Got Focus";
        private const string wfButton2LostFocus = "WinForm Button 2 Lost Focus";
        private const string avButton1GotFocus = "Avalon Button 1 Got Focus";
        private const string avButton1LostFocus = "Avalon Button 1 Lost Focus";
        private const string avButton2GotFocus = "Avalon Button 2 Got Focus";
        private const string avButton2LostFocus = "Avalon Button 2 Lost Focus";
        private const string wfTextBox1GotFocus = "WinForm TextBox 1 Got Focus";
        private const string wfTextBox1LostFocus = "WinForm TextBox 1 Lost Focus";

        private delegate void myEventHandler(object sender);

        public FocusPreservation(string[] args) : base(args) { }

        protected override void InitTest(TParams p)
        {
            this.Title = "FocusPreservation";
            this.UseMITA = true;
            this.Topmost = true;

            //Call helper function AddMenu to create menu
            AddMenu();

            _avButton1.Name = "avButton1";
            _avButton1.Content = "_Avalon Button 1";
            _avButton1.GotFocus += new System.Windows.RoutedEventHandler(avButton1_GotFocus);
            _avButton1.LostFocus += new System.Windows.RoutedEventHandler(avButton1_LostFocus);
            _avButton1.Click += new System.Windows.RoutedEventHandler(avButton1_Click);
            _stackPanel.Children.Add(_avButton1);

            _avButton2.Name = "avButton2";
            _avButton2.Content = "Avalon _Button 2";
            _avButton2.GotFocus += new System.Windows.RoutedEventHandler(avButton2_GotFocus);
            _avButton2.LostFocus += new System.Windows.RoutedEventHandler(avButton2_LostFocus);
            _stackPanel.Children.Add(_avButton2);

            _wfButton1.Name = "wfButton1";
            _wfButton1.Text = "WinForm Button &1";
            _wfButton1.AutoSize = true;
            _wfButton1.GotFocus += new EventHandler(wfButton1_GotFocus);
            _wfButton1.LostFocus += new EventHandler(wfButton1_LostFocus);

            _wfButton2.Name = "wfButton2";
            _wfButton2.Text = "WinForm Button &2";
            _wfButton2.AutoSize = true;
            _wfButton2.GotFocus += new EventHandler(wfButton2_GotFocus);
            _wfButton2.LostFocus += new EventHandler(wfButton2_LostFocus);

            _wfTextBox1.Name = "wfTextBox1";
            _wfTextBox1.Text = "WinForm TextBox 1";
            _wfTextBox1.AutoSize = true;
            _wfTextBox1.GotFocus += new EventHandler(wfTextBox1_GotFocus);
            _wfTextBox1.LostFocus += new EventHandler(wfTextBox1_LostFocus);

            _panel.Controls.Add(_wfButton1);
            _panel.Controls.Add(_wfButton2);
            _panel.Controls.Add(_wfTextBox1);
            _winformsHost1.Child = _panel;

            _stackPanel.Children.Add(_winformsHost1);

            _label1.Height = 30;
            _label2.Height = 30;
            _label2.Height = 30;
            _stackPanel.Children.Add(_label1);
            _stackPanel.Children.Add(_label2);
            _stackPanel.Children.Add(_label3);

            this.Content = _stackPanel;

            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            return base.BeforeScenario(p, scenario);
        }

        protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
        {
            _label1.Content = "";
            _label2.Content = "";
            _label3.Content = "";
            _labelText1 = "";
            _labelText2 = "";
            _labelText3 = "";
            base.AfterScenario(p, scenario, result);
        }

        void wfButton1_LostFocus(object sender, EventArgs e)
        {
            _label2.Content = _labelText2 = wfButton1LostFocus;
        }
        void wfButton1_GotFocus(object sender, EventArgs e)
        {
            _label1.Content = _labelText1 = wfButton1GotFocus;
        }

        void wfButton2_LostFocus(object sender, EventArgs e)
        {
            _label2.Content = _labelText2 = wfButton2LostFocus;
        }
        void wfButton2_GotFocus(object sender, EventArgs e)
        {
            _label1.Content = _labelText1 = wfButton2GotFocus;
        }

        void wfTextBox1_LostFocus(object sender, EventArgs e)
        {
            _label3.Content = _labelText3 = wfTextBox1LostFocus;
        }
        void wfTextBox1_GotFocus(object sender, EventArgs e)
        {
            _label3.Content = _labelText3 = wfTextBox1GotFocus;
        }

        void avButton2_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _label2.Content = _labelText2 = avButton2LostFocus;
        }
        void avButton2_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _label1.Content = _labelText1 = avButton2GotFocus;
        }

        void avButton1_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _label2.Content = _labelText2 = avButton1LostFocus;
        }
        void avButton1_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _label1.Content = _labelText1 = avButton1GotFocus;
        }

        //if button already has focus (from a previous scenario), it will not fire GotFocus event.
        //this Click event is to get past that
        void avButton1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _label1.Content = _labelText1 = avButton1GotFocus;
        }

        #endregion


        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios

        [Scenario("Focus is saved when navigating between AV and WFH (mouse)")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetAllEditControls(p))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(2);
            _edit1.SetFocus();
            _edit1.Click(PointerButtons.Primary); //click on Avalon button 1
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton1GotFocus,
                "Failed at avButton1.GotFocus");
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "",
                "Failed at initial state");

            _edit2.Click(PointerButtons.Primary); //click on Avalon button 2
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton2GotFocus,
                "Failed.\nExpected: " + avButton2GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == avButton1LostFocus,
                "Failed.\nExpected: " + avButton1LostFocus +
                "\nActual: " + _labelText2);

            _edit3.Click(PointerButtons.Primary); //click on WinForm button 1
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit3.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit3.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == wfButton1GotFocus,
                "Failed.\nExpected: " + wfButton1GotFocus + 
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == avButton2LostFocus,
                "Failed.\nExpected: " + avButton2LostFocus +
                "\nActual: " + _labelText2);

            _edit4.Click(PointerButtons.Primary); //click on WinForm button 2
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit4.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit4.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == wfButton2GotFocus,
                "Failed.\nExpected: " + wfButton2GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == wfButton1LostFocus,
                "Failed.\nExpected: " + wfButton1LostFocus +
                "\nActual: " + _labelText2);

            _edit5.Click(PointerButtons.Primary); //click on WinForm textbox 1
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit5.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText3 == wfTextBox1GotFocus,
                "Failed.\nExpected: " + wfTextBox1GotFocus + 
                "\nActual: " + _labelText3);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == wfButton2LostFocus,
                "Failed.\nExpected: " + wfButton2LostFocus +
                "\nActual: " + _labelText2);

            _edit1.Click(PointerButtons.Primary); //click on Avalon button 1
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton1GotFocus,
                "Failed.\nExpected: " + avButton2GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText3 == wfTextBox1LostFocus,
                "Failed.\nExpected: " + wfTextBox1LostFocus +
                "\nActual: " + _labelText3);

            return sr;
        }

        [Scenario("Focus is saved when navigating between AV and WFH (keyboard - TAB).")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetAllEditControls(p))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(2);
            _edit1.SetFocus();
            _form.SendKeys("{TAB}"); //first tab goes to avButton2
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit2.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton2GotFocus,
                "Failed.\nExpected: " + avButton2GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == avButton1LostFocus,
                "Failed.\nExpected: " + avButton1LostFocus +
                "\nActual: " + _labelText2);

            _form.SendKeys("{TAB}"); //tab goes to wfButton1
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit3.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit3.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == wfButton1GotFocus,
                "Failed.\nExpected: " + wfButton1GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == avButton2LostFocus,
                "Failed.\nExpected: " + avButton2LostFocus +
                "\nActual: " + _labelText2);

            _form.SendKeys("{TAB}"); //tab goes to wfButton2
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit4.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit4.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == wfButton2GotFocus,
                "Failed.\nExpected: " + wfButton2GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == wfButton1LostFocus,
                "Failed.\nExpected: " + wfButton1LostFocus +
                "\nActual: " + _labelText2);

            _form.SendKeys("{TAB}"); //tab goes to wfTextBox1
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit5.HasKeyboardFocus,
                "Failed at avButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText3 == wfTextBox1GotFocus,
                "Failed.\nExpected: " + wfTextBox1GotFocus +
                "\nActual: " + _labelText3);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == wfButton2LostFocus,
                "Failed.\nExpected: " + wfButton2LostFocus +
                "\nActual: " + _labelText2);

            _form.SendKeys("{TAB}"); //tab goes to menuitem1
            _form.SendKeys("{TAB}"); //tab goes to menuitem2
            _form.SendKeys("{TAB}"); //tab goes to avButton1
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton1GotFocus,
                "Failed.\nExpected: " + avButton2GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText3 == wfTextBox1LostFocus,
                "Failed.\nExpected: " + wfTextBox1LostFocus +
                "\nActual: " + _labelText3);

            return sr;
        }

        [Scenario("Focus is saved when navigating between AV and WFH (keyboard - AccessorKey).")]
        public ScenarioResult Scenario3(TParams p)
        {
            //AccessorKeys should not move focus
            ScenarioResult sr = new ScenarioResult();

            if (!GetAllEditControls(p))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(2);
            _edit1.SetFocus();
            _form.SendKeys("%a");
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus after Alt-a. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton1GotFocus,
                "Failed.\nExpected: " + avButton1GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "",
                "Failed at initial state");

            _form.SendKeys("%b");
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus after Alt-b. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton1GotFocus,
                "Failed.\nExpected: " + avButton1GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "",
                "Failed at initial state");

            _form.SendKeys("%1");
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus after Alt-1. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton1GotFocus,
                "Failed.\nExpected: " + avButton1GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "",
                "Failed at initial state");

            _form.SendKeys("%2");
            Utilities.SleepDoEvents(2);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus after Alt-2. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton1GotFocus,
                "Failed.\nExpected: " + avButton1GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "",
                "Failed at initial state");

            return sr;
        }

        [Scenario("Focus is saved when navigating between AV Menu and WFH control.")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetAllEditControls(p))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(2);

            _edit1.Click(PointerButtons.Primary); //click on Avalon button 1
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton1GotFocus,
                "Failed.\nExpected: " + avButton1GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "",
                "Failed at initial state");

            _form.SendKeys("%e"); //select AV menu
            _form.SendKeys("%"); //return
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit1.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == avButton1GotFocus,
                "Failed.\nExpected: " + avButton1GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "",
                "Failed at initial state");

            _edit3.Click(PointerButtons.Primary); //click on WinForm button 1
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit3.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit3.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == wfButton1GotFocus,
                "Failed at wfButton1.GotFocus");
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == avButton1LostFocus,
                "Failed at avButton1.LostFocus");

            _form.SendKeys("%e"); //select AV menu
            _form.SendKeys("%"); //return
            Utilities.SleepDoEvents(5);

            //WPFMiscUtils.IncCounters(sr, p.log, _edit3.HasKeyboardFocus,
            //    "Failed at wfButton1.HasKeyboardFocus. HasKeyboardFocus=" + _edit3.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == wfButton1GotFocus,
                "Failed at wfButton1.GotFocus. Actual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == wfButton1LostFocus,
                "Failed at avButton1.LostFocus. Actual: " + _labelText2);

            _edit4.Click(PointerButtons.Primary); //click on WinForm button 2
            Utilities.SleepDoEvents(5);

            WPFMiscUtils.IncCounters(sr, p.log, _edit4.HasKeyboardFocus,
                "Failed at wfButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit4.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == wfButton2GotFocus,
                "Failed at wfButton2.GotFocus. Actual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == wfButton1LostFocus,
                "Failed at wfButton1.LostFocus. Actual: " + _labelText2);

            _edit6.Click(PointerButtons.Primary); //click on menu item 1
            _form.SendKeys("%"); //return
            Utilities.SleepDoEvents(5);

            //WPFMiscUtils.IncCounters(sr, p.log, _edit4.HasKeyboardFocus,
            //    "Failed at wfButton2.HasKeyboardFocus. HasKeyboardFocus=" + _edit6.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == wfButton2GotFocus,
                "Failed at wfButton2.GotFocus. Actual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == wfButton2LostFocus,
                "Failed at wfButton2.LostFocus. Actual: " + _labelText2);

            return sr;
        }

        [Scenario("Focus is saved when the window is minimized.")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetAllEditControls(p))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(5);
            _edit3.SetFocus();
            _edit3.Click(PointerButtons.Primary); //click on WinForm button 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF button 1 has focus)
            WPFMiscUtils.IncCounters(sr, p.log, _edit3.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus.  HasKeyboardFocus=" + _edit3.HasKeyboardFocus);

            //Minimize window
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, 
                new myEventHandler(MinimizeWindow), this);
            Utilities.SleepDoEvents(5);

            //Restore window
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(RestoreWindow), this);
            Utilities.SleepDoEvents(20);

            //Verify that WF button 1 still has focus
            WPFMiscUtils.IncCounters(sr, p.log, _edit3.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus.  HasKeyboardFocus=" + _edit3.HasKeyboardFocus);

            //Test other WF control, textbox
            _edit5.Click(PointerButtons.Primary); //click on WinForm textbox 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF textbox 1 has focus)
            WPFMiscUtils.IncCounters(sr, p.log, _edit5.HasKeyboardFocus,
                "Failed at wfTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus);

            //Minimize window
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, 
                new myEventHandler(MinimizeWindow), this);
            Utilities.SleepDoEvents(5);

            //Restore window
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(RestoreWindow), this);
            Utilities.SleepDoEvents(20);

            //Verify that WF textbox 1 still has focus
            WPFMiscUtils.IncCounters(sr, p.log, _edit5.HasKeyboardFocus,
                "Failed at wfTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus);

            if (sr.FailCount > 0)
            {
                p.log.LogKnownBug(BugDb.WindowsOSBugs, 1562663,
                    "Keyboard focus is lost when the form is minimized, or when returning from a modal dialog");
            }
            return sr;
        }

        [Scenario("Focus is saved when the window is maximized.")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetAllEditControls(p))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(5);
            _edit3.SetFocus();
            _edit3.Click(PointerButtons.Primary); //click on WinForm button 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF button 1 has focus)
            VerifyWfButton1HasFocus(sr, p);

            //Minimize window
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(MaximizeWindow), this);
            Utilities.SleepDoEvents(5);

            //Restore window
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(RestoreWindow), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfButton1HasFocus(sr, p);

            //Test other WF control, textbox
            _edit5.Click(PointerButtons.Primary); //click on WinForm textbox 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF textbox 1 has focus)
            VerifyWfTextBox1HasFocus(sr, p);

            //Minimize window
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(MaximizeWindow), this);
            Utilities.SleepDoEvents(5);

            //Restore window
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(RestoreWindow), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF textbox 1 still has focus
            VerifyWfTextBox1HasFocus(sr, p);

            return sr;
        }

        [Scenario("Focus is saved when the window is resized.")]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetAllEditControls(p))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(5);
            _edit3.SetFocus();
            _edit3.Click(PointerButtons.Primary); //click on WinForm button 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF button 1 has focus)
            VerifyWfButton1HasFocus(sr, p);

            //Resize window smaller
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(ResizeWindowSmaller), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfButton1HasFocus(sr, p);

            //Resize window bigger
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(ResizeWindowBigger), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfButton1HasFocus(sr, p);

            //Undo resize
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(UndoResizeWindow), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfButton1HasFocus(sr, p);

            //Test other WF control, textbox
            _edit5.Click(PointerButtons.Primary); //click on WinForm textbox 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF textbox 1 has focus)
            VerifyWfTextBox1HasFocus(sr, p);

            //Resize window smaller
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(ResizeWindowSmaller), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfTextBox1HasFocus(sr, p);

            //Resize window bigger
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(ResizeWindowBigger), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfTextBox1HasFocus(sr, p);

            //Undo resize
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(UndoResizeWindow), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfTextBox1HasFocus(sr, p);

            return sr;
        }

        [Scenario("Focus is saved when the window is moved.")]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetAllEditControls(p))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(5);
            _edit3.SetFocus();
            _edit3.Click(PointerButtons.Primary); //click on WinForm button 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF button 1 has focus)
            VerifyWfButton1HasFocus(sr, p);

            //Resize window smaller
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(MoveToTheRight), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfButton1HasFocus(sr, p);

            //Resize window bigger
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(MoveToTheBottom), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfButton1HasFocus(sr, p);

            //Undo resize
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(MoveToTheTopLeft), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfButton1HasFocus(sr, p);

            //Test other WF control, textbox
            _edit5.Click(PointerButtons.Primary); //click on WinForm textbox 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF textbox 1 has focus)
            VerifyWfTextBox1HasFocus(sr, p);

            //Resize window smaller
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(MoveToTheRight), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfTextBox1HasFocus(sr, p);

            //Resize window bigger
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(MoveToTheBottom), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfTextBox1HasFocus(sr, p);

            //Undo resize
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(MoveToTheTopLeft), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            VerifyWfTextBox1HasFocus(sr, p);

            return sr;
        }

        [Scenario("Focus is saved after returning from a modal dialog.")]
        public ScenarioResult Scenario9(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetAllEditControls(p))
            {
                return new ScenarioResult(false);
            }
            Utilities.SleepDoEvents(5);
            _edit3.SetFocus();
            _edit3.Click(PointerButtons.Primary); //click on WinForm button 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF button 1 has focus)
            WPFMiscUtils.IncCounters(sr, p.log, _edit3.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus.  HasKeyboardFocus=" + _edit3.HasKeyboardFocus);

            //Launch modal dialog
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(LaunchModal), this);
            Utilities.SleepDoEvents(5);

            //Close modal dialog
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(CloseModal), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            //WPFMiscUtils.IncCounters(sr, p.log, _edit3.HasKeyboardFocus,
            //    "Failed at wfButton1.HasKeyboardFocus.  HasKeyboardFocus=" + _edit3.HasKeyboardFocus);

            //Test other WF control, textbox
            _edit5.Click(PointerButtons.Primary); //click on WinForm textbox 1
            Utilities.SleepDoEvents(5);

            //Verify initial state (WF textbox 1 has focus)
            WPFMiscUtils.IncCounters(sr, p.log, _edit5.HasKeyboardFocus,
                "Failed at wfTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus);

            //Launch modal dialog
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(LaunchModal), this);
            Utilities.SleepDoEvents(5);

            //Close modal dialog
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new myEventHandler(CloseModal), this);
            Utilities.SleepDoEvents(5);

            //Verify that WF button 1 still has focus
            //WPFMiscUtils.IncCounters(sr, p.log, _edit5.HasKeyboardFocus,
            //    "Failed at wfTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus);

            //Resolution: Won't Fix
            if (sr.FailCount > 0)
            {
                p.log.LogKnownBug(BugDb.WindowsOSBugs, 1562663,
                    "Keyboard focus is lost when the form is minimized, or when returning from a modal dialog");
            }
            return sr;
        }

        #endregion

        #region Delegates

        void LaunchModal(object sender)
        {
            _win = new SW.Window();
            _win.Topmost = true;
            _win.ShowDialog();
        }
        void CloseModal(object sender)
        {
            _win.Close();
        }

        void MinimizeWindow(object sender)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        void MaximizeWindow(object sender)
        {
            this.WindowState = System.Windows.WindowState.Maximized;
        }
        void RestoreWindow(object sender)
        {
            this.WindowState = System.Windows.WindowState.Normal;
        }

        void ResizeWindowSmaller(object sender)
        {
            this.Width = 300;
            this.Height = 300;
        }
        void ResizeWindowBigger(object sender)
        {
            this.Width = 1000;
            this.Height = 700;
        }
        void UndoResizeWindow(object sender)
        {
            this.Width = 800;
            this.Height = 600;
        }

        void MoveToTheRight(object sender)
        {
            this.Left = 300;
        }
        void MoveToTheBottom(object sender)
        {
            this.Top = 300;
        }
        void MoveToTheTopLeft(object sender)
        {
            this.Top = 0;
            this.Left = 0;
        }

        #endregion

        #region Helper Functions

        private void VerifyWfButton1HasFocus(ScenarioResult sr, TParams p)
        {
            WPFMiscUtils.IncCounters(sr, p.log, _edit3.HasKeyboardFocus,
                "Failed at wfButton1.HasKeyboardFocus.  HasKeyboardFocus=" + _edit3.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText1 == wfButton1GotFocus,
                "Failed.\nExpected: " + wfButton1GotFocus +
                "\nActual: " + _labelText1);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == "",
                "Failed.\nExpected: " + "" +
                "\nActual: " + _labelText2);
        }

        private void VerifyWfTextBox1HasFocus(ScenarioResult sr, TParams p)
        {
            WPFMiscUtils.IncCounters(sr, p.log, _edit5.HasKeyboardFocus,
                "Failed at wfTextBox1.HasKeyboardFocus. HasKeyboardFocus=" + _edit5.HasKeyboardFocus);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText3 == wfTextBox1GotFocus,
                "Failed.\nExpected: " + wfTextBox1GotFocus +
                "\nActual: " + _labelText3);
            WPFMiscUtils.IncCounters(sr, p.log, _labelText2 == wfButton1LostFocus,
                "Failed.\nExpected: " + wfButton1LostFocus +
                "\nActual: " + _labelText2);
        }

        private void AddMenu()
        {
            SWC.Menu menu = new SWC.Menu();
            menu.Background = System.Windows.Media.Brushes.LightBlue;
            menu.Name = "avMenu1";

            SWC.MenuItem mi1 = new SWC.MenuItem();
            mi1.Width = 60;
            mi1.Header = "_File";
            menu.Items.Add(mi1);

            SWC.MenuItem mi2 = new SWC.MenuItem();
            mi2.Width = 60;
            mi2.Header = "_Edit";
            menu.Items.Add(mi2);

            SWC.MenuItem mia = new SWC.MenuItem();
            mia.Header = "_Cut";
            mia.InputGestureText = "Ctrl+X";
            mi2.Items.Add(mia);

            SWC.MenuItem mib = new SWC.MenuItem();
            mib.Command = System.Windows.Input.ApplicationCommands.Copy;
            mib.Header = "_Copy";
            mi2.Items.Add(mib);

            SWC.MenuItem mic = new SWC.MenuItem();
            mic.Command = System.Windows.Input.ApplicationCommands.Paste;
            mic.Header = "_Paste";
            mi2.Items.Add(mic);

            menu.HorizontalAlignment = SW.HorizontalAlignment.Left;
            _stackPanel.Children.Add(menu);
        }

        //Gets Mita wrapper controls for all controls
        bool GetAllEditControls(TParams p)
        {
            try
            {
                UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("FocusPreservation"));
                _form = new Edit(uiApp);

                UIObject uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("avButton1"));
                _edit1 = new Edit(uiControl);
                uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("avButton2"));
                _edit2 = new Edit(uiControl);
                uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("wfButton1"));
                _edit3 = new Edit(uiControl);
                uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("wfButton2"));
                _edit4 = new Edit(uiControl);
                uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("wfTextBox1"));
                _edit5 = new Edit(uiControl);
                uiControl = uiApp.Descendants.Find(UICondition.CreateFromId("avMenu1"));
                _edit6 = new Edit(uiControl);

                _form.SetFocus();
                return true;
            }
            catch (Exception ex)
            {
                p.log.WriteLine("Failed to Get Mita wrapper controls" + ex.ToString());
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
//@ Focus is saved when navigating between AV and WFH (mouse).
//@ Focus is saved when navigating between AV and WFH (keyboard - TAB).
//@ Focus is saved when navigating between AV and WFH (keyboard - AccessorKey).
//@ Focus is saved when navigating between AV Menu and WFH control.
//@ Focus is saved when the window is minimized.
//@ Focus is saved when the window is maximized.
//@ Focus is saved when the window is resized.
//@ Focus is saved when the window is moved.
//@ Focus is saved after returning from a modal dialog.