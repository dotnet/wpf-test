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


// Testcase:    RTL
// Description: Verify RTL for WF controls hosted in
namespace WindowsFormsHostTests
{
    public class RTL : WPFReflectBase
    {
        #region Testcase setup
        public RTL(string[] args) : base(args) { }

        // class vars
        private enum ContainerType { DockPanel, Grid, StackPanel, Canvas, WrapPanel };
        private DockPanel _dp;
        private Grid _grid;
        private StackPanel _stack;
        private Canvas _canvas;
        private WrapPanel _wrap;

        private enum TestType { Single, Complex, Container };

        // WF Host controls
        private WindowsFormsHost _wfh1;
        private WindowsFormsHost _wfh2;
        private WindowsFormsHost _wfh3;

        // WF controls
        private System.Windows.Forms.TextBox _tb1;
        private System.Windows.Forms.Button _wfbtn1;
        private System.Windows.Forms.DataGridView _dgv;
        private System.Windows.Forms.DataGridView _dgv2;
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
            return base.BeforeScenario(p, scenario);
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("WFH with single control.")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // iterate through each container type
            foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
            {
                FlowDirection flowdir;
                TestSetup(p, contType, TestType.Single);
                MyPause();

                // verify initial RTL states - all should be "Left To Right"
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, this.FlowDirection, "Initial state incorrect - window", p.log);
                flowdir = GetPanelFlowDirection(contType);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, flowdir, "Initial state incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh1.FlowDirection, "Initial state incorrect - WFH1", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _tb1.RightToLeft, "Initial state incorrect - textbox", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh2.FlowDirection, "Initial state incorrect - WFH2", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _wfbtn1.RightToLeft, "Initial state incorrect - button", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh3.FlowDirection, "Initial state incorrect - WFH3", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _dgv.RightToLeft, "Initial state incorrect - dgv", p.log);

                // change WFC host (transition LeftToRight -> RightToLeft)
                _wfh3.FlowDirection = FlowDirection.RightToLeft;
                MyPause();

                // verify RTL states - only WFH3 should be reversed
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, this.FlowDirection, "FlowDirection incorrect - window", p.log);
                flowdir = GetPanelFlowDirection(contType);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, flowdir, "FlowDirection incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh1.FlowDirection, "FlowDirection incorrect - WFH1", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _tb1.RightToLeft, "RightToLeft incorrect - textbox", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh2.FlowDirection, "FlowDirection incorrect - WFH2", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _wfbtn1.RightToLeft, "RightToLeft incorrect - button", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.RightToLeft, _wfh3.FlowDirection, "FlowDirection incorrect - WFH3", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.Yes, _dgv.RightToLeft, "RightToLeft incorrect - dgv", p.log);

                // change WFC host (transition  RightToLeft -> LeftToRight)
                _wfh3.FlowDirection = FlowDirection.LeftToRight;
                MyPause();

                // verify RTL states - only WFH3 should be reversed
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, this.FlowDirection, "FlowDirection incorrect - window", p.log);
                flowdir = GetPanelFlowDirection(contType);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, flowdir, "FlowDirection incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh1.FlowDirection, "FlowDirection incorrect - WFH1", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _tb1.RightToLeft, "RightToLeft incorrect - textbox", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh2.FlowDirection, "FlowDirection incorrect - WFH2", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _wfbtn1.RightToLeft, "RightToLeft incorrect - button", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh3.FlowDirection, "FlowDirection incorrect - WFH3", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _dgv.RightToLeft, "RightToLeft incorrect - dgv", p.log);
            }

            return sr;
        }

        [Scenario("WFH with complex control.")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // iterate through each container type
            foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
            {
                FlowDirection flowdir;
                TestSetup(p, contType, TestType.Complex);
                MyPause();

                // verify initial RTL states - all should be "Left To Right"
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, this.FlowDirection, "Initial state incorrect - window", p.log);
                flowdir = GetPanelFlowDirection(contType);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, flowdir, "Initial state incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh1.FlowDirection, "Initial state incorrect - WFH1", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh2.FlowDirection, "Initial state incorrect - WFH2", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh3.FlowDirection, "Initial state incorrect - WFH3", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _userctl.RightToLeft, "Initial state incorrect - userctrl", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _dgv.RightToLeft, "RightToLeft incorrect - dgv", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _tb1.RightToLeft, "RightToLeft incorrect - textbox", p.log);

                // change WFC host (transition LeftToRight -> RightToLeft)
                _wfh1.FlowDirection = FlowDirection.RightToLeft;
                MyPause();

                // verify RTL states - WFH1 and children should be reversed
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, this.FlowDirection, "FlowDirection incorrect - window", p.log);
                flowdir = GetPanelFlowDirection(contType);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, flowdir, "FlowDirection incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.RightToLeft, _wfh1.FlowDirection, "FlowDirection incorrect - WFH1", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh2.FlowDirection, "FlowDirection incorrect - WFH2", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh3.FlowDirection, "FlowDirection incorrect - WFH3", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.Yes, _userctl.RightToLeft, "RightToLeft incorrect - userctrl", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.Yes, _dgv.RightToLeft, "RightToLeft incorrect - dgv", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.Yes, _tb1.RightToLeft, "RightToLeft incorrect - textbox", p.log);

                // change WFC host (transition RightToLeft -> LeftToRight)
                _wfh1.FlowDirection = FlowDirection.LeftToRight;
                MyPause();

                // verify RTL states - WFH1 and children should be reversed
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, this.FlowDirection, "FlowDirection incorrect - window", p.log);
                flowdir = GetPanelFlowDirection(contType);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, flowdir, "FlowDirection incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh1.FlowDirection, "FlowDirection incorrect - WFH1", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh2.FlowDirection, "FlowDirection incorrect - WFH2", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh3.FlowDirection, "FlowDirection incorrect - WFH3", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _userctl.RightToLeft, "RightToLeft incorrect - userctrl", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _dgv.RightToLeft, "RightToLeft incorrect - dgv", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _tb1.RightToLeft, "RightToLeft incorrect - textbox", p.log);
            }

            return sr;
        }

        [Scenario("WFH with container control.")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // iterate through each container type
            foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
            {
                FlowDirection flowdir;
                TestSetup(p, contType, TestType.Container);
                MyPause();

                // verify initial RTL states - all should be "Left To Right"
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, this.FlowDirection, "Initial state incorrect - window", p.log);
                flowdir = GetPanelFlowDirection(contType);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, flowdir, "Initial state incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh1.FlowDirection, "Initial state incorrect - WFH1", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _panel.RightToLeft, "Initial state incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _tb1.RightToLeft, "Initial state incorrect - textbox", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _dgv.RightToLeft, "Initial state incorrect - dgv", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _wfbtn1.RightToLeft, "Initial state incorrect - button", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _dgv2.RightToLeft, "Initial state incorrect - dgv", p.log);

                // change WFC host (transition LeftToRight -> RightToLeft)
                _wfh1.FlowDirection = FlowDirection.RightToLeft;
                MyPause();

                // verify RTL states - WFH1 and children should be reversed
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, this.FlowDirection, "FlowDirection incorrect - window", p.log);
                flowdir = GetPanelFlowDirection(contType);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, flowdir, "FlowDirection incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.RightToLeft, _wfh1.FlowDirection, "FlowDirection incorrect - WFH1", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.Yes, _panel.RightToLeft, "RightToLeft incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.Yes, _tb1.RightToLeft, "RightToLeft incorrect - textbox", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.Yes, _dgv.RightToLeft, "RightToLeft incorrect - dgv", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.Yes, _wfbtn1.RightToLeft, "RightToLeft incorrect - button", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.Yes, _dgv2.RightToLeft, "RightToLeft incorrect - dgv", p.log);

                // change WFC host (transition RightToLeft -> LeftToRight)
                _wfh1.FlowDirection = FlowDirection.LeftToRight;
                MyPause();

                // verify RTL states - WFH1 and children should be reversed
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, this.FlowDirection, "FlowDirection incorrect - window", p.log);
                flowdir = GetPanelFlowDirection(contType);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, flowdir, "FlowDirection incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, FlowDirection.LeftToRight, _wfh1.FlowDirection, "FlowDirection incorrect - WFH1", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _panel.RightToLeft, "RightToLeft incorrect - panel", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _tb1.RightToLeft, "RightToLeft incorrect - textbox", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _dgv.RightToLeft, "RightToLeft incorrect - dgv", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _wfbtn1.RightToLeft, "RightToLeft incorrect - button", p.log);
                WPFMiscUtils.IncCounters(sr, System.Windows.Forms.RightToLeft.No, _dgv2.RightToLeft, "RightToLeft incorrect - dgv", p.log);
            }

            return sr;
        }

        [Scenario("WF control FlowDirection/RTL transitions (yes/no, etc.)")]
        public ScenarioResult Scenario4(TParams p)
        {
            // this test is performed within the first 3 Scenarios
            return new ScenarioResult(true, "Performed elsewhere", p.log);
        }

        #region Helper functions
        /// <summary>
        /// Helper function to set up app for particular Scenario.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="contType"></param>
        /// <param name="testType"></param>
        private void TestSetup(TParams p, ContainerType contType, TestType testType)
        {
            // update app title bar and log file
            string str = string.Format("Container type: {0} using {1} control",
                contType.ToString(), testType.ToString());
            this.Title = str;
            p.log.WriteLine(str);

            // create WF host controls
            _wfh1 = new WindowsFormsHost();
            _wfh2 = new WindowsFormsHost();
            _wfh3 = new WindowsFormsHost();

            // 3 WFH each containing single control - TextBox, Button, DataGridView
            if (testType == TestType.Single)
            {
                // create WF controls (one for each host)
                _tb1 = new System.Windows.Forms.TextBox();
                _wfbtn1 = new System.Windows.Forms.Button();
                _dgv = new System.Windows.Forms.DataGridView();

                // throw some random text into controls
                _tb1.Text = p.ru.GetString(10);
                _wfbtn1.Text = p.ru.GetString(10);
                SetupDGV(_dgv);

                // add WF controls to host controls
                _wfh1.Child = _tb1;
                _wfh2.Child = _wfbtn1;
                _wfh3.Child = _dgv;
            }

            // 1 WFH containing complex control - usercontrol with DataGridView
            // 2 WFH each containing single control - TextBox, Button
            else if (testType == TestType.Complex)
            {
                // create WF usercontrol
                _userctl = new System.Windows.Forms.UserControl();
                _userctl.Dock = System.Windows.Forms.DockStyle.Fill;

                // add WF controls to usercontrol
                _dgv = new System.Windows.Forms.DataGridView();
                _tb1 = new System.Windows.Forms.TextBox();
                _userctl.Controls.Add(_dgv);
                _userctl.Controls.Add(_tb1);

                // throw some random text into controls
                _tb1.Text = p.ru.GetString(10);
                SetupDGV(_dgv);

                // add WF usercontrol to host control, add to dock panel
                _wfh1.Child = _userctl;

                // create stuff for other two controls !!! VSWhidbey Regression_Bug43
                // have to be careful to not tamper with vars used in panel
                System.Windows.Forms.TextBox _tb2 = new System.Windows.Forms.TextBox();
                _wfbtn1 = new System.Windows.Forms.Button();
                _tb2.Text = p.ru.GetString(10);
                _wfbtn1.Text = p.ru.GetString(10);
                _wfh2.Child = _tb2;
                _wfh3.Child = _wfbtn1;
            }

            // 1 WFH containing container - panel with TextBox, DataGridView, Button, DataGridView
            // 2 WFH each containing single control - TextBox, Button
            else if (testType == TestType.Container)
            {
                // create WF container with three controls
                _panel = new System.Windows.Forms.Panel();
                _panel.Dock = System.Windows.Forms.DockStyle.Fill;

                // add 3 WF controls to panel
                _tb1 = new System.Windows.Forms.TextBox();
                _dgv = new System.Windows.Forms.DataGridView();
                _wfbtn1 = new System.Windows.Forms.Button();
                _dgv2 = new System.Windows.Forms.DataGridView();

                _tb1.Location = new System.Drawing.Point(10, 10);
                _dgv.Location = new System.Drawing.Point(10, 100);
                _wfbtn1.Location = new System.Drawing.Point(100, 10);
                _dgv2.Location = new System.Drawing.Point(100, 100);

                _panel.Controls.AddRange(new System.Windows.Forms.Control[] { _tb1, _dgv, _wfbtn1, _dgv2 });

                // throw some random text into control
                _tb1.Text = p.ru.GetString(10);
                _wfbtn1.Text = p.ru.GetString(10);
                SetupDGV(_dgv);
                SetupDGV(_dgv2);

                // add WF panel to host control, add to dock panel
                _wfh1.Child = _panel;

                // create stuff for other two controls !!! VSWhidbey Regression_Bug43
                // have to be careful to not tamper with vars used in panel
                System.Windows.Forms.TextBox _tb2 = new System.Windows.Forms.TextBox();
                System.Windows.Forms.Button _wfbtn2 = new System.Windows.Forms.Button();
                _tb2.Text = p.ru.GetString(10);
                _wfbtn2.Text = p.ru.GetString(10);
                _wfh2.Child = _tb2;
                _wfh3.Child = _wfbtn2;
            }

            else
            {
                // new TestType?
                throw new ArgumentException("Unknown TestType '{0}'", testType.ToString());
            }

            // create appropriate panel and add hosts
            CreatePanel(contType);
        }

        /// <summary>
        /// Helper function used to create Avalon panel of proper type and add to window.
        /// Also adds our WFH controls to panel and sets up initial locations.
        /// </summary>
        /// <param name="contType"></param>
        private void CreatePanel(ContainerType contType)
        {
            if (contType == ContainerType.Canvas)
            {
                _canvas = new Canvas();
                this.Content = _canvas;

                // have to set explicit locations for Canvas
                Canvas.SetLeft(_wfh1, 50);
                Canvas.SetLeft(_wfh2, 150);
                Canvas.SetLeft(_wfh3, 300);

                // add host controls to canvas
                _canvas.Children.Add(_wfh1);
                _canvas.Children.Add(_wfh2);
                _canvas.Children.Add(_wfh3);
            }
            else if (contType == ContainerType.DockPanel)
            {
                _dp = new DockPanel();
                this.Content = _dp;

                // add host controls to dock panel
                _dp.Children.Add(_wfh1);
                _dp.Children.Add(_wfh2);
                _dp.Children.Add(_wfh3);
            }
            else if (contType == ContainerType.Grid)
            {
                _grid = new Grid();
                this.Content = _grid;

                // have to define Columns/Rows for Grid
                _grid.ColumnDefinitions.Add(new ColumnDefinition());
                _grid.ColumnDefinitions.Add(new ColumnDefinition());
                _grid.ColumnDefinitions.Add(new ColumnDefinition());
                _grid.RowDefinitions.Add(new RowDefinition());
                Grid.SetColumn(_wfh1, 0);
                Grid.SetColumn(_wfh2, 1);
                Grid.SetColumn(_wfh3, 2);

                // add host controls to grid
                _grid.Children.Add(_wfh1);
                _grid.Children.Add(_wfh2);
                _grid.Children.Add(_wfh3);
            }
            else if (contType == ContainerType.StackPanel)
            {
                _stack = new StackPanel();
                this.Content = _stack;

                // add host controls to stack panel
                _stack.Children.Add(_wfh1);
                _stack.Children.Add(_wfh2);
                _stack.Children.Add(_wfh3);
            }
            else if (contType == ContainerType.WrapPanel)
            {
                _wrap = new WrapPanel();
                this.Content = _wrap;

                // add host controls to wrap panel
                _wrap.Children.Add(_wfh1);
                _wrap.Children.Add(_wfh2);
                _wrap.Children.Add(_wfh3);
            }
            else
            {
                // unknown ContainerType?
                throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
            }
        }

        // Helper function to put stuff into DataGridView control
        private void SetupDGV(System.Windows.Forms.DataGridView dgv)
        {
            // do stuff with DGV
            dgv.Columns.Add("Col1", "Column 1");
            dgv.Columns.Add("Col2", "Column 2");
            dgv.Rows.Add(4);
            dgv.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        /// <summary>
        /// Helper function used to retrieve FlowDirection property from the panel we are using.
        /// </summary>
        /// <param name="contType"></param>
        /// <returns></returns>
        private FlowDirection GetPanelFlowDirection(ContainerType contType)
        {
            if (contType == ContainerType.Canvas)
            {
                return(_canvas.FlowDirection);
            }
            else if (contType == ContainerType.DockPanel)
            {
                return (_dp.FlowDirection);
            }
            else if (contType == ContainerType.Grid)
            {
                return (_grid.FlowDirection);
            }
            else if (contType == ContainerType.StackPanel)
            {
                return (_stack.FlowDirection);
            }
            else if (contType == ContainerType.WrapPanel)
            {
                return (_wrap.FlowDirection);
            }
            else
            {
                // unknown ContainerType?
                throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
            }
        }

        private static void MyPause()
        {
            // uncommenting next line causes exception !!!
            WPFReflectBase.DoEvents();

            System.Threading.Thread.Sleep(500);
        }
        #endregion

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ WFH with single control.

//@ WFH with complex control.

//@ WFH with container control.

//@ WF control FlowDirection/RTL transitions (yes/no, etc.)
