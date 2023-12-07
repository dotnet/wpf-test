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
using System.Windows.Forms.Integration;

using SWF = System.Windows.Forms;
using SD = System.Drawing;

//
// Testcase:    FontPropagation
// Description: Verify that Fonts set on the WPF parrent get propagated to the WFH and it's ctrs
//
namespace WindowsFormsHostTests
{
    public class FontPropagation : WPFReflectBase
    {
        #region Testcase setup
        public FontPropagation(string[] args) : base(args) { }

        // class vars
        private StackPanel _sp;
        private WindowsFormsHost _wfh1;
        private WindowsFormsHost _wfh2;
        private SWF.Button _wfb1;
        private SWF.Button _wfb2;
        private SWF.Button _wfb3;
        private Button _avb1;
        private bool _debug = false;      // used for TC debugging

        // define font item structure
        // used to contain a particular instance of an Avalon font variation to use as test
        private struct MyFontItemType
        {
            public string testDesc;         // friendly name of test - for logging
            public string Family;           // FontFamily
            public double Size;             // FontSize
            public FontWeight Weight;       // FontWeight
            public FontStyle Style;         // FontStyle
            public MyFontItemType(string d, string f, double sz, FontWeight w, FontStyle s)
            {
                testDesc = d;
                Family = f;
                Size = sz;
                Weight = w;
                Style = s;
            }
        }

        // create a list of Font variations to try out
        private static MyFontItemType[] s_myFontList = {
            new MyFontItemType("1", "Arial", 10, FontWeights.Bold, FontStyles.Italic),
            new MyFontItemType("2", "Wingdings", 15, FontWeights.Medium, FontStyles.Normal),
            new MyFontItemType("3", "Microsoft Sans Serif", 20, FontWeights.Light, FontStyles.Oblique),
            new MyFontItemType("4", "Georgia", 5, FontWeights.ExtraBlack, FontStyles.Italic),
            new MyFontItemType("5", "Impact", 10, FontWeights.DemiBold, FontStyles.Normal),
            new MyFontItemType("6", "Symbol", 14, FontWeights.ExtraLight, FontStyles.Oblique),
            new MyFontItemType("7", "Courier New", 6, FontWeights.Heavy, FontStyles.Italic),
            new MyFontItemType("8", "Lucida Console", 30, FontWeights.Regular, FontStyles.Normal),
            new MyFontItemType("9", "Palatino Linotype", 15, FontWeights.UltraLight, FontStyles.Oblique),
        };

        protected override void InitTest(TParams p)
        {
            // resize window, put on top
            this.Topmost = true;
            this.Topmost = false;
            this.Width = 500;
            this.Height = 500;

            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            bool b = base.BeforeScenario(p, scenario);

            // debug - run specific scenario !!!
            //if (scenario.Name != "Scenario10") { return false; }

            this.Title = currentScenario.Name;

            return b;
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Set WFH parent")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // "Set WFH parent"
                SetFontOnParent(item);
                MyPause();

                // "verify that Fonts get propagated to the WFH and it's controls"
                VerifyFontInHost(p, sr, _wfh1, item);
                VerifyFontInHost(p, sr, _wfh2, item);
                VerifyFontInWFControl(p, sr, _wfb1, item);
                VerifyFontInWFControl(p, sr, _wfb2, item);
                VerifyFontInWFControl(p, sr, _wfb3, item);
            }
            return sr;
        }

        [Scenario("Set WFH parent twice")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // "Set WFH parent twice"
                SetFontOnParent(item);
                MyPause();
                SetFontOnParent(item);
                MyPause();

                // "verify that Fonts get propagated to the WFH and it's controls"
                VerifyFontInHost(p, sr, _wfh1, item);
                VerifyFontInHost(p, sr, _wfh2, item);
                VerifyFontInWFControl(p, sr, _wfb1, item);
                VerifyFontInWFControl(p, sr, _wfb2, item);
                VerifyFontInWFControl(p, sr, _wfb3, item);
            }
            return sr;
        }

        [Scenario("Set WFH child and make sure WFH parent doesn't change")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // define Avalon font that will compare with what we will use for WF test font
            MyFontItemType expFont1 = new MyFontItemType("Expected", "Arial", 20, FontWeights.Normal, FontStyles.Italic);
            MyFontItemType expFont3 = new MyFontItemType("Expected", "Impact", 20, FontWeights.Bold, FontStyles.Italic);

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // set test font on parent
                SetFontOnParent(item);
                MyPause();

                // "Set WFH child"
                _wfb1.Font = new SD.Font("Arial", 15, SD.FontStyle.Italic);
                _wfb3.Font = new SD.Font("Impact", 15, SD.FontStyle.Italic | SD.FontStyle.Bold);
                MyPause();

                // "make sure WFH parent doesn't change"
                // compare current font of WFH child with what we set it to
                VerifyFontInHost(p, sr, _wfh1, item);
                VerifyFontInHost(p, sr, _wfh2, item);
                VerifyFontInWFControl(p, sr, _wfb1, expFont1);
                VerifyFontInWFControl(p, sr, _wfb2, item);
                VerifyFontInWFControl(p, sr, _wfb3, expFont3);
            }
            return sr;
        }

        [Scenario("Set WFH child then WFH parent and make sure WFH child doesn't change to WFH parent.")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // define Avalon font that will compare with what we will use for WF test font
            MyFontItemType expFont1 = new MyFontItemType("Expected", "Arial", 20, FontWeights.Normal, FontStyles.Italic);
            MyFontItemType expFont3 = new MyFontItemType("Expected", "Impact", 20, FontWeights.Bold, FontStyles.Italic);

            // "Set WFH child"
            _wfb1.Font = new SD.Font("Arial", 15, SD.FontStyle.Italic);
            _wfb3.Font = new SD.Font("Impact", 15, SD.FontStyle.Italic | SD.FontStyle.Bold);

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // "Set WFH parent"
                SetFontOnParent(item);
                MyPause();

                // "make sure WFH child doesn't change to WFH parent"
                // compare current font of WFH child with what we set it to
                VerifyFontInHost(p, sr, _wfh1, item);
                VerifyFontInHost(p, sr, _wfh2, item);
                VerifyFontInWFControl(p, sr, _wfb1, expFont1);
                VerifyFontInWFControl(p, sr, _wfb2, item);
                VerifyFontInWFControl(p, sr, _wfb3, expFont3);
            }
            return sr;
        }

        [Scenario("Set WFH parent then WFH child")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // define Avalon font that will compare with what we will use for WF test font
            MyFontItemType expFont1 = new MyFontItemType("Expected", "Arial", 20, FontWeights.Normal, FontStyles.Italic);
            MyFontItemType expFont3 = new MyFontItemType("Expected", "Impact", 20, FontWeights.Bold, FontStyles.Italic);

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // "Set WFH parent"
                SetFontOnParent(item);
                MyPause();

                // "Set WFH child"
                _wfb1.Font = new SD.Font("Arial", 15, SD.FontStyle.Italic);
                _wfb3.Font = new SD.Font("Impact", 15, SD.FontStyle.Italic | SD.FontStyle.Bold);

                // "verify that Fonts get propagated appropriately"
                // compare current font of WFH child with what we set it to
                VerifyFontInHost(p, sr, _wfh1, item);
                VerifyFontInHost(p, sr, _wfh2, item);
                VerifyFontInWFControl(p, sr, _wfb1, expFont1);
                VerifyFontInWFControl(p, sr, _wfb2, item);
                VerifyFontInWFControl(p, sr, _wfb3, expFont3);
            }
            return sr;
        }

        [Scenario("Set WFH ")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // define Avalon font for window (for comparison)
            MyFontItemType origFont = new MyFontItemType("Original", "Arial", 20, FontWeights.Normal, FontStyles.Italic);
            SetFontOnParent(origFont);

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // "Set WFH"
                // setting WFH1 should affect WFH1, wfb1, and wfb2 - but not WFH2 or wfb3
                SetFontOnHost(_wfh1,item);
                MyPause();

                // "verify that Fonts get propagated to the WFH and it's controls"
                VerifyFontInHost(p, sr, _wfh1, item);
                VerifyFontInHost(p, sr, _wfh2, origFont);
                VerifyFontInWFControl(p, sr, _wfb1, item);
                VerifyFontInWFControl(p, sr, _wfb2, item);
                VerifyFontInWFControl(p, sr, _wfb3, origFont);
            }
            return sr;
        }

        [Scenario("Set WFH twice")]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // define Avalon font for window (for comparison)
            MyFontItemType origFont = new MyFontItemType("Original", "Arial", 20, FontWeights.Normal, FontStyles.Italic);
            SetFontOnParent(origFont);

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // "Set WFH twice"
                // setting WFH2 should affect WFH2 and wfb3 - but not WFH1, wfb1, wfb2
                SetFontOnHost(_wfh2, item);
                MyPause();
                SetFontOnHost(_wfh2, item);
                MyPause();

                // "verify that Fonts get propagated to the WFH and it's controls"
                VerifyFontInHost(p, sr, _wfh1, origFont);
                VerifyFontInHost(p, sr, _wfh2, item);
                VerifyFontInWFControl(p, sr, _wfb1, origFont);
                VerifyFontInWFControl(p, sr, _wfb2, origFont);
                VerifyFontInWFControl(p, sr, _wfb3, item);
            }
            return sr;
        }

        [Scenario("Set WFH child and make sure WFH doesn't change")]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // define Avalon font for window (for comparison)
            MyFontItemType origFont = new MyFontItemType("Original", "Microsoft Sans Serif", 20, FontWeights.Normal, FontStyles.Italic);
            SetFontOnParent(origFont);

            // define Avalon font that will compare with what we will use for WF test font
            MyFontItemType expFont1 = new MyFontItemType("Expected", "Arial", 20, FontWeights.Normal, FontStyles.Italic);
            MyFontItemType expFont3 = new MyFontItemType("Expected", "Impact", 20, FontWeights.Bold, FontStyles.Italic);

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // set test font on host
                SetFontOnHost(_wfh1, item);
                MyPause();

                // "Set WFH child"
                _wfb1.Font = new SD.Font("Arial", 15, SD.FontStyle.Italic);
                _wfb3.Font = new SD.Font("Impact", 15, SD.FontStyle.Italic | SD.FontStyle.Bold);
                MyPause();

                // "make sure WFH doesn't change"
                // compare current font of WFH child with what we set it to
                VerifyFontInHost(p, sr, _wfh1, item);                // we set this in test
                VerifyFontInHost(p, sr, _wfh2, origFont);            // original window font
                VerifyFontInWFControl(p, sr, _wfb1, expFont1);       // we set this
                VerifyFontInWFControl(p, sr, _wfb2, item);           // should match parent (wfh1)
                VerifyFontInWFControl(p, sr, _wfb3, expFont3);       // we set this
            }
            return sr;
        }

        [Scenario("Set WFH child then WFH and make sure child doesn't change to WFH")]
        public ScenarioResult Scenario9(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // define Avalon font for window (for comparison)
            MyFontItemType origFont = new MyFontItemType("Original", "Microsoft Sans Serif", 20, FontWeights.Normal, FontStyles.Italic);
            SetFontOnParent(origFont);

            // define Avalon font that will compare with what we will use for WF test font
            MyFontItemType expFont1 = new MyFontItemType("Expected", "Arial", 20, FontWeights.Normal, FontStyles.Italic);
            MyFontItemType expFont3 = new MyFontItemType("Expected", "Impact", 20, FontWeights.Bold, FontStyles.Italic);

            // "Set WFH child"
            _wfb1.Font = new SD.Font("Arial", 15, SD.FontStyle.Italic);
            _wfb3.Font = new SD.Font("Impact", 15, SD.FontStyle.Italic | SD.FontStyle.Bold);

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // "Set WFH"
                SetFontOnHost(_wfh1, item);
                MyPause();

                // "make sure child doesn't change to WFH"
                // compare current font of WFH child with what we set it to
                VerifyFontInHost(p, sr, _wfh1, item);                // we set this
                VerifyFontInHost(p, sr, _wfh2, origFont);            // original window font
                VerifyFontInWFControl(p, sr, _wfb1, expFont1);       // we set this
                VerifyFontInWFControl(p, sr, _wfb2, item);           // should match parent (wfh1)
                VerifyFontInWFControl(p, sr, _wfb3, expFont3);       // we set this
            }
            return sr;
        }

        [Scenario("Set WFH then child")]
        public ScenarioResult Scenario10(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            SetUpControls();

            // define Avalon font for window (for comparison)
            MyFontItemType origFont = new MyFontItemType("Original", "Microsoft Sans Serif", 20, FontWeights.Normal, FontStyles.Italic);
            SetFontOnParent(origFont);

            // define Avalon font that will compare with what we will use for WF test font
            MyFontItemType expFont1 = new MyFontItemType("Expected", "Arial", 20, FontWeights.Normal, FontStyles.Italic);
            MyFontItemType expFont3 = new MyFontItemType("Expected", "Impact", 20, FontWeights.Bold, FontStyles.Italic);

            // loop through test fonts
            foreach (MyFontItemType item in s_myFontList)
            {
                p.log.WriteLine("Testing '{0}'", item.testDesc);

                // "Set WFH"
                SetFontOnHost(_wfh1, item);
                MyPause();

                // "Set WFH child"
                _wfb1.Font = new SD.Font("Arial", 15, SD.FontStyle.Italic);
                _wfb3.Font = new SD.Font("Impact", 15, SD.FontStyle.Italic | SD.FontStyle.Bold);

                // "verify that Fonts get propagated appropriately"
                // compare current font of WFH child with what we set it to
                VerifyFontInHost(p, sr, _wfh1, item);                // we set this
                VerifyFontInHost(p, sr, _wfh2, origFont);            // original window font
                VerifyFontInWFControl(p, sr, _wfb1, expFont1);       // we set this
                VerifyFontInWFControl(p, sr, _wfb2, item);           // should match parent (wfh1)
                VerifyFontInWFControl(p, sr, _wfb3, expFont3);       // we set this
            }
            return sr;
        }

        #region Font Comparison functions

        /// <summary>
        /// Compare current font in WFH with indicated test font
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sr"></param>
        /// <param name="wfh"></param>
        /// <param name="item"></param>
        private void VerifyFontInHost(TParams p, ScenarioResult sr, WindowsFormsHost wfh, MyFontItemType item)
        {
            // compare Font properties in WFH with that of test item
            if (_debug) { p.log.WriteLine("  Checking font in '{0}'", wfh.Name); }
            WPFMiscUtils.IncCounters(sr, item.Family, wfh.FontFamily.Source, "Font Family not correct", p.log);
            WPFMiscUtils.IncCounters(sr, item.Size, wfh.FontSize, "Font Size not correct", p.log);
            WPFMiscUtils.IncCounters(sr, item.Style, wfh.FontStyle, "Font Style not correct", p.log);
            WPFMiscUtils.IncCounters(sr, item.Weight, wfh.FontWeight, "Font Weight not correct", p.log);
        }

        /// <summary>
        /// Compare current font in WinForms Control with indicated test font - must convert test font
        /// "flavor" from "Avalon" to "WinForms"
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sr"></param>
        /// <param name="ctrl"></param>
        /// <param name="item"></param>
        private void VerifyFontInWFControl(TParams p, ScenarioResult sr, SWF.Control ctrl, MyFontItemType item)
        {
            // compare Font properties in WF control with that of test item
            if (_debug) { p.log.WriteLine("  Checking font in '{0}'", ctrl.Name); }

            // can compare Font family names directly
            WPFMiscUtils.IncCounters(sr, item.Family, ctrl.Font.Name, "Font Family not correct", p.log);
            
            // convert Avalon units to WF units
            double actSize = ((ctrl.Font.Size) * 96) / 72;
            WPFMiscUtils.IncCounters(sr, item.Size, actSize, "Font Size not correct", p.log);

            // compare Avalon Font Style to WF Font
            bool bStyle = CompareFontStyles(ctrl.Font, item.Style);
            WPFMiscUtils.IncCounters(sr, p.log, bStyle, "Font Style not correct");

            // compare Avalon Font Weight to WF Font
            bool bWeights = CompareFontWeights(ctrl.Font, item.Weight);
            WPFMiscUtils.IncCounters(sr, p.log, bWeights, "Font Weight not correct");
        }

        /// <summary>
        /// Helper used to compare Avalon FontStyle with equivalent WinForms settings
        /// </summary>
        /// <param name="wfFont"></param>
        /// <param name="avFontStyle"></param>
        /// <returns></returns>
        private bool CompareFontStyles(SD.Font wfFont, FontStyle avFontStyle)
        {
            // Conversion Notes:
            // Avalon       Winforms
            // Italic       Italic "on"
            // Normal       Italic "off"
            // Oblique      Italic "on"

            // WF style will either have Italic or not
            if (avFontStyle == FontStyles.Italic)
            {
                return (wfFont.Italic);
            }
            else if (avFontStyle == FontStyles.Normal)
            {
                return (!wfFont.Italic);
            }
            else if (avFontStyle == FontStyles.Oblique)
            {
                return (wfFont.Italic);
            }

            // should never get here - if do, treat as failure so will end up in log
            return false;
        }

        /// <summary>
        /// Helper used to compare Avalon FontWeight with equivalent WinForms settings
        /// </summary>
        /// <param name="wfFont"></param>
        /// <param name="avFontWeight"></param>
        /// <returns></returns>
        private bool CompareFontWeights(SD.Font wfFont, FontWeight avFontWeight)
        {
            // WF style will either have Bold or not

            // Yes, a "switch" statement would be more clear here - however, you
            // cannot use "switch" syntax because avFontWeight is not an "int"

            // if is on list of Weights that should be bold:
            if (avFontWeight == FontWeights.Black || 
                avFontWeight == FontWeights.Bold ||
                avFontWeight == FontWeights.DemiBold ||
                avFontWeight == FontWeights.ExtraBold ||
                avFontWeight == FontWeights.Heavy ||
                avFontWeight == FontWeights.Medium ||
                avFontWeight == FontWeights.SemiBold ||
                avFontWeight == FontWeights.ExtraBlack ||
                avFontWeight == FontWeights.UltraBlack ||
                avFontWeight == FontWeights.UltraBold)
            {
                        return (wfFont.Bold);
            }

            // if is on list of Weights that should not be bold:
            else if
                (
                avFontWeight == FontWeights.ExtraLight ||
                avFontWeight == FontWeights.Light ||
                avFontWeight == FontWeights.Normal ||
                avFontWeight == FontWeights.Regular ||
                avFontWeight == FontWeights.Thin ||
                avFontWeight == FontWeights.UltraLight
                )
            {
                return (!wfFont.Bold);
            }

            // is not on either list?  That's just not right.
            else
            {
                throw new ArgumentException("Unknown FontWeight value", avFontWeight.ToString());
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Sets up controls for us to play with
        /// Window containing StackPanel containing two WindowsFormsHosts
        /// First WFH contains two WF buttons in a panel; second contains one WF button
        /// Add some Avalon Labels for seasoning, mix, bake until golden brown.  Serve.
        /// </summary>
        private void SetUpControls()
        {
            // set up controls
            _sp = new StackPanel();

            // create some random labels as separators
            Label lab1 = new Label();
            lab1.Content = "Avalon Label 1";
            lab1.Background = Brushes.LightBlue;
            Label lab2 = new Label();
            lab2.Content = "Avalon Label 2";
            lab2.Background = Brushes.LightGreen;
            Label lab3 = new Label();
            lab3.Content = "Avalon Label 3";
            lab3.Background = Brushes.LightPink;
            Label lab4 = new Label();
            lab4.Content = "Avalon Label 4";
            lab4.Background = Brushes.LightCyan;

            // create WFH with panel with multiple WF controls
            _wfh1 = new WindowsFormsHost();
            _wfh1.Name = "WFH_1";
            SWF.Panel wfp = new SWF.Panel();
            wfp.Dock = SWF.DockStyle.Fill;

            // create two wf buttons
            _wfb1 = new SWF.Button();
            _wfb1.Name = "WF Btn 1";
            _wfb1.Text = "This is a WinForms Button";
            _wfb1.AutoSize = true;
            _wfb2 = new SWF.Button();
            _wfb2.Name = "WF Btn 2";
            _wfb2.Text = "Have a Nice Day!";
            _wfb2.AutoSize = true;

            // put buttons in panel
            _wfb2.Top = _wfb1.Bottom * 2;
            wfp.Controls.Add(_wfb1);
            wfp.Controls.Add(_wfb2);
            _wfh1.Child = wfp;

            // create WFH with another WF control
            _wfh2 = new WindowsFormsHost();
            _wfh2.Name = "WFH_2";
            _wfb3 = new SWF.Button();
            _wfb3.Name = "WF Btn 3";
            _wfb3.Text = "Are we having fun yet?";
            _wfb3.Width = 200;
            _wfh2.Child = _wfb3;

            // create Avalon button for comparison
            _avb1 = new Button();
            _avb1.Content = "This is an Avalon Button";

            // add stuff to StackPanel
            _sp.Children.Add(lab1);
            _sp.Children.Add(_wfh1);
            _sp.Children.Add(lab2);
            _sp.Children.Add(_avb1);
            _sp.Children.Add(lab3);
            _sp.Children.Add(_wfh2);
            _sp.Children.Add(lab4);

            this.Content = _sp;
        }

        /// <summary>
        /// Assign test font item to main window
        /// </summary>
        /// <param name="item"></param>
        private void SetFontOnParent(MyFontItemType item)
        {
            // assign font test item to main window, as "parent" of host control
            // (cannot set font at StackPanel level)
            this.FontFamily = new FontFamily(item.Family);
            this.FontSize = item.Size;
            this.FontStyle = item.Style;
            this.FontWeight = item.Weight;
        }

        /// <summary>
        /// Assign test font item to specified WFH
        /// </summary>
        /// <param name="wfh"></param>
        /// <param name="item"></param>
        private void SetFontOnHost(WindowsFormsHost wfh, MyFontItemType item)
        {
            // assign font test item directly to specific WFH
            wfh.FontFamily = new FontFamily(item.Family);
            wfh.FontSize = item.Size;
            wfh.FontStyle = item.Style;
            wfh.FontWeight = item.Weight;
        }

        /// <summary>
        /// Standard pause to allow fonts, etc. time to mingle
        /// </summary>
        private static void MyPause()
        {
            WPFReflectBase.DoEvents();
            SWF.Application.DoEvents();
            System.Threading.Thread.Sleep(200);
        }

        #endregion

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Set WFH parent

//@ Set WFH parent twice

//@ Set WFH child and make sure WFH parent doesn't change

//@ Set WFH child then WFH parent and make sure WFH child doesn't change to WFH parent.

//@ Set WFH parent then WFH child

//@ Set WFH 

//@ Set WFH twice

//@ Set WFH child and make sure WFH doesn't change

//@ Set WFH child then WFH and make sure child doesn't change to WFH

//@ Set WFH then child
