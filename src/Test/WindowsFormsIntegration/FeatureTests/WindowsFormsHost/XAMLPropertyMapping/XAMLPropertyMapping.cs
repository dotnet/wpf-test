// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Threading;
//using SWF = System.Windows.Forms;
//using SD = System.Drawing;
//using MS.Internal.Mita.Foundation;


// Testcase:    XAMLPropertyMapping
// Description: Verify that setting properties in XAML on WFH work
namespace WindowsFormsHostTests
{
    public class XAMLPropertyMapping : WPFReflectBase
    {
        // class vars
        // define names of XAML files we are using
        private const string XamlTestFile1 = "XAMLPropertyMappingTest1.xaml";
        private const string XamlTestFile2 = "XAMLPropertyMappingTest2.xaml";

        // define a structure that maps a WFH control name with a generic item
        // (for each property type we are testing, we can create a list of things to check for
        // and having a generic 'object' allows us to use this structure for many lists)
        private struct MyMapType
        {
            public string hostName;                         // name of WFH control in XAML
            public object propValue;                        // what the property is supposed to be
            public MyMapType(string name, object val)
            {
                hostName = name;
                propValue = val;
            }
        }

        #region Testcase setup
        public XAMLPropertyMapping(string[] args) : base(args) { }

        //[STAThread]
        //public static void Main(string[] args)
        //{
        //    Window win2 = new XAMLPropertyMapping(null).WindowFromXamlFile(XamlTestFile2);
        //
        //    System.Windows.Application app = new System.Windows.Application();
        //
        //    app.Run(new XAMLPropertyMapping(args));
        //}


        protected override void InitTest(TParams p)
        {
        Window win2 = new XAMLPropertyMapping(null).WindowFromXamlFile(XamlTestFile2);

            Type t = typeof(System.Windows.Forms.Integration.WindowsFormsHost);
            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            bool b = base.BeforeScenario(p, scenario);

            // debug - run specific scenario !!!
            //if (scenario.Name != "Scenario2") { return false; }

            this.Title = currentScenario.Name;

            return b;
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Verify that each property we map can be set in XAML")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // have one or more xaml files that demonstrate each property
            // load xaml files into windows
            // examine windows to verify properties are set as in xaml

            // how best to distribute properties into xaml files?
            // each xaml file is loaded into single window
            // can load several windows from xaml
            // function for each mapped property, pass in window to use

            // load first xaml files into window, test stuff
            Window win1 = WindowFromXamlFile(XamlTestFile1);
            if (win1 != null)
            {
                // show windows
                win1.Show();

                // perform tests - one function call for each mapped property
                CheckPropertyAllowDrop(p, sr, win1);
                CheckPropertyBackground(p, sr, win1);
                CheckPropertyCursor(p, sr, win1);
                CheckPropertyFlowDirection(p, sr, win1);
                CheckPropertyForceCursor(p, sr, win1);
                CheckPropertyIsEnabled(p, sr, win1);
                CheckPropertyVisibility(p, sr, win1);
                CheckPropertyForeground(p, sr, win1);
                CheckPropertyPadding(p, sr, win1);

                // IsVisible is a read-only property - cannot be set in XAML !!!
                // possibly remove Scenario from test?
                //CheckPropertyIsVisible(p, sr, win1);

                // clean up
                win1.Close();
            }

            // load second xaml files into window, test stuff
            Window win2 = WindowFromXamlFile(XamlTestFile2);
            if (win2 != null)
            {
                // show windows
                win2.Show();

                // debug - uncomment following line to see my wonderfully tacky XAML page
                //Utilities.ActiveFreeze(currentScenario.Name);

                // perform tests - one function call for each mapped property
                CheckPropertyFontStyle(p, sr, win2);
                CheckPropertyFontWeight(p, sr, win2);
                CheckPropertyFontFamily(p, sr, win2);
                CheckPropertyFontSize(p, sr, win2);

                // clean up
                win2.Close();
            }

            return sr;
        }

        [Scenario("Verify that Background works with different brushes (gradiant/solid).")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // This is actually tested as part of Scenario1
            // (the part that converts XAML to code)
            WPFMiscUtils.IncCounters(sr, p.log, true, "Done as part of Scenario1");

            return sr;
        }

        #endregion

        #region Property Check functions

        #region AllowDrop
        // this is a list of WindowsFormsHost controls (by name) and what the 'AllowDrop' property should be
        private static MyMapType[] s_myAllowDropList = {
            new MyMapType("wfhAD1", true),
            new MyMapType("wfhAD2", false),
        };

        private void CheckPropertyAllowDrop(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing AllowDrop Property");
            // for each item in AllowDrop list
            foreach (MyMapType curType in s_myAllowDropList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.AllowDrop, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region Background
        // this is a list of WindowsFormsHost controls (by name) and what the 'Background' property should be
        // (this is only valid for properties defined (in XAML) *not* using "Static Resources" - see below)
        private static MyMapType[] s_myBackgroundList = {
            new MyMapType("wfhBGBlue", Brushes.Blue),
            //new MyMapType("wfhBGRed", Brushes.Red),
            new MyMapType("wfhBGGreen", Brushes.Green),
        };

        private void CheckPropertyBackground(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing Background Property");

            // for each item in Background list
            foreach (MyMapType curType in s_myBackgroundList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.Background, "Property does not match", p.log);
                }
            }

            // It is fine when we are comparing simple property settings, because we can create a match in code.
            // However, when more complex settings are defined in XAML (such as when defining a "StaticResource"
            // in XAML which is then linked to a control in XAML) it is not as likely to create an "exact match"
            // in code.  For example, setting Background="Red" can be compared directly with "Brushes.Red" in code.
            // But, if you set Background="{StaticResource MyRed}" where MyRed is defined simply as "Red",
            // you can not compare directly with "Brushes.Red" in code.  We need to be able to compare such
            // things, because more complex objects (radial gradients) are better defined as Static Resources.
            // So, how do we test these things?  How about we set up both an Avalon element (such as a Label)
            // and a WFH with the same {StaticResource xxx} mapping.  Then, we can load these into code, get
            // access to each of these elements, and compare the properties directly.  Both mappings have been
            // "converted" from XAML to code the same way, so if the WFH conversion is getting through (which
            // is what we are testing here) then, in theory, it should match exactly what the Label has.
            // Whew.  All that to get a part of a single Scenario working. 

            // now check properties that are defined using static resources
            CheckBackgroundResource(p, sr, w, "labBGRed", "wfhBGRed");
            CheckBackgroundResource(p, sr, w, "labBGYellowBlueLin", "wfhBGYellowBlueLin");
            CheckBackgroundResource(p, sr, w, "labBGRedGreenLin", "wfhBGRedGreenLin");
            CheckBackgroundResource(p, sr, w, "labBGYellowBlueRad", "wfhBGYellowBlueRad");
            CheckBackgroundResource(p, sr, w, "labBGBitmap", "wfhBGBitmap");
        }

        /// <summary>
        /// Helper function to compare Background property of two UI elements defined in XAML file
        /// one element is a Label, the other is a WindowsFormsHost
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sr"></param>
        /// <param name="w"></param>
        /// <param name="idLabel"></param>
        /// <param name="idHost"></param>
        private void CheckBackgroundResource(TParams p, ScenarioResult sr, Window w, string idLabel, string idHost)
        {
            p.log.WriteLine("Comparing label '{0}' with host '{1}'", idLabel, idHost);

            // get handle to Avalon label
            System.Windows.Controls.Label lab = (System.Windows.Controls.Label)w.FindName(idLabel);
            //p.log.WriteLine("labRed = '{0}'", labRed);

            // find host control in window
            WindowsFormsHost wfh = GetHostByName(p, sr, w, idHost);

            // compare backgound
            WPFMiscUtils.IncCounters(sr, lab.Background, wfh.Background, "Property does not match", p.log);
        }

        #endregion
        #region Cursor
        // this is a list of WindowsFormsHost controls (by name) and what the 'Cursor' property should be
        private static MyMapType[] s_myCursorList = {
            new MyMapType("wfhAppStarting", System.Windows.Input.Cursors.AppStarting),
            new MyMapType("wfhArrow", System.Windows.Input.Cursors.Arrow),
            new MyMapType("wfhArrowCD", System.Windows.Input.Cursors.ArrowCD),
            new MyMapType("wfhCross", System.Windows.Input.Cursors.Cross),
            new MyMapType("wfhHand", System.Windows.Input.Cursors.Hand),
            new MyMapType("wfhHelp", System.Windows.Input.Cursors.Help),
            new MyMapType("wfhIBeam", System.Windows.Input.Cursors.IBeam),
            new MyMapType("wfhNo", System.Windows.Input.Cursors.No),
            new MyMapType("wfhNone", System.Windows.Input.Cursors.None),
            new MyMapType("wfhPen", System.Windows.Input.Cursors.Pen),
            new MyMapType("wfhScrollAll", System.Windows.Input.Cursors.ScrollAll),
            new MyMapType("wfhScrollE", System.Windows.Input.Cursors.ScrollE),
            new MyMapType("wfhScrollN", System.Windows.Input.Cursors.ScrollN),
            new MyMapType("wfhScrollNE", System.Windows.Input.Cursors.ScrollNE),
            new MyMapType("wfhScrollNS", System.Windows.Input.Cursors.ScrollNS),
            new MyMapType("wfhScrollNW", System.Windows.Input.Cursors.ScrollNW),
            new MyMapType("wfhScrollS", System.Windows.Input.Cursors.ScrollS),
            new MyMapType("wfhScrollSE", System.Windows.Input.Cursors.ScrollSE),
            new MyMapType("wfhScrollSW", System.Windows.Input.Cursors.ScrollSW),
            new MyMapType("wfhScrollW", System.Windows.Input.Cursors.ScrollW),
            new MyMapType("wfhScrollWE", System.Windows.Input.Cursors.ScrollWE),
            new MyMapType("wfhSizeAll", System.Windows.Input.Cursors.SizeAll),
            new MyMapType("wfhSizeNESW", System.Windows.Input.Cursors.SizeNESW),
            new MyMapType("wfhSizeNS", System.Windows.Input.Cursors.SizeNS),
            new MyMapType("wfhSizeNWSE", System.Windows.Input.Cursors.SizeNWSE),
            new MyMapType("wfhSizeWE", System.Windows.Input.Cursors.SizeWE),
            new MyMapType("wfhUpArrow", System.Windows.Input.Cursors.UpArrow),
            new MyMapType("wfhWait", System.Windows.Input.Cursors.Wait),
        };

        private void CheckPropertyCursor(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing Cursor Property");
            // for each item in Cursor list
            foreach (MyMapType curType in s_myCursorList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.Cursor, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region FlowDirection
        // this is a list of WindowsFormsHost controls (by name) and what the 'FlowDirection' property should be
        private static MyMapType[] s_myFlowDirectionList = {
            new MyMapType("wfhRTL", FlowDirection.RightToLeft),
            new MyMapType("wfhLTR", FlowDirection.LeftToRight),
        };

        private void CheckPropertyFlowDirection(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing FlowDirection Property");
            // for each item in FlowDirection list
            foreach (MyMapType curType in s_myFlowDirectionList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.FlowDirection, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region FontStyle
        // this is a list of WindowsFormsHost controls (by name) and what the 'FontStyle' property should be
        private static MyMapType[] s_myFontStyleList = {
            new MyMapType("wfhFont1", FontStyles.Italic),
            new MyMapType("wfhFont2", FontStyles.Normal),
            new MyMapType("wfhFont3", FontStyles.Oblique),
            new MyMapType("wfhFont4", FontStyles.Italic),
            new MyMapType("wfhFont5", FontStyles.Normal),
            new MyMapType("wfhFont6", FontStyles.Oblique),
            new MyMapType("wfhFont7", FontStyles.Italic),
            new MyMapType("wfhFont8", FontStyles.Normal),
            new MyMapType("wfhFont9", FontStyles.Oblique),
            new MyMapType("wfhFont10", FontStyles.Italic),
            new MyMapType("wfhFont11", FontStyles.Normal),
            new MyMapType("wfhFont12", FontStyles.Oblique),
            new MyMapType("wfhFont13", FontStyles.Italic),
            new MyMapType("wfhFont14", FontStyles.Normal),
            new MyMapType("wfhFont15", FontStyles.Oblique),
            new MyMapType("wfhFont16", FontStyles.Italic),
            new MyMapType("wfhFont17", FontStyles.Normal),
            new MyMapType("wfhFont18", FontStyles.Oblique),
            new MyMapType("wfhFont19", FontStyles.Italic),
            new MyMapType("wfhFont20", FontStyles.Normal),
            new MyMapType("wfhFont21", FontStyles.Oblique),
            new MyMapType("wfhFont22", FontStyles.Italic),
            new MyMapType("wfhFont23", FontStyles.Normal),
            new MyMapType("wfhFont24", FontStyles.Oblique),
            new MyMapType("wfhFont25", FontStyles.Italic),
            new MyMapType("wfhFont26", FontStyles.Normal),
            new MyMapType("wfhFont27", FontStyles.Oblique),
            new MyMapType("wfhFont28", FontStyles.Italic),
            new MyMapType("wfhFont29", FontStyles.Normal),
            new MyMapType("wfhFont30", FontStyles.Oblique),
            new MyMapType("wfhFont31", FontStyles.Italic),
            new MyMapType("wfhFont32", FontStyles.Normal),
            new MyMapType("wfhFont33", FontStyles.Oblique),
            new MyMapType("wfhFont34", FontStyles.Italic),
            new MyMapType("wfhFont35", FontStyles.Normal),
        };

        private void CheckPropertyFontStyle(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing FontStyle Property");
            // for each item in FontStyle list
            foreach (MyMapType curType in s_myFontStyleList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.FontStyle, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region FontWeight
        // this is a list of WindowsFormsHost controls (by name) and what the 'FontWeight' property should be
        private static MyMapType[] s_myFontWeightList = {
            new MyMapType("wfhFont1", FontWeights.Black),
            new MyMapType("wfhFont2", FontWeights.Bold),
            new MyMapType("wfhFont3", FontWeights.DemiBold),
            new MyMapType("wfhFont4", FontWeights.ExtraBlack),
            new MyMapType("wfhFont5", FontWeights.ExtraBold),
            new MyMapType("wfhFont6", FontWeights.ExtraLight),
            new MyMapType("wfhFont7", FontWeights.Heavy),
            new MyMapType("wfhFont8", FontWeights.Light),
            new MyMapType("wfhFont9", FontWeights.Medium),
            new MyMapType("wfhFont10", FontWeights.Normal),
            new MyMapType("wfhFont11", FontWeights.Regular),
            new MyMapType("wfhFont12", FontWeights.SemiBold),
            new MyMapType("wfhFont13", FontWeights.Thin),
            new MyMapType("wfhFont14", FontWeights.UltraBlack),
            new MyMapType("wfhFont15", FontWeights.UltraBold),
            new MyMapType("wfhFont16", FontWeights.UltraLight),
            new MyMapType("wfhFont17", FontWeights.Black),
            new MyMapType("wfhFont18", FontWeights.Bold),
            new MyMapType("wfhFont19", FontWeights.DemiBold),
            new MyMapType("wfhFont20", FontWeights.ExtraBlack),
            new MyMapType("wfhFont21", FontWeights.ExtraBold),
            new MyMapType("wfhFont22", FontWeights.ExtraLight),
            new MyMapType("wfhFont23", FontWeights.Heavy),
            new MyMapType("wfhFont24", FontWeights.Light),
            new MyMapType("wfhFont25", FontWeights.Medium),
            new MyMapType("wfhFont26", FontWeights.Normal),
            new MyMapType("wfhFont27", FontWeights.Regular),
            new MyMapType("wfhFont28", FontWeights.SemiBold),
            new MyMapType("wfhFont29", FontWeights.Thin),
            new MyMapType("wfhFont30", FontWeights.UltraBlack),
            new MyMapType("wfhFont31", FontWeights.UltraBold),
            new MyMapType("wfhFont32", FontWeights.UltraLight),
            new MyMapType("wfhFont33", FontWeights.Black),
            new MyMapType("wfhFont34", FontWeights.Bold),
            new MyMapType("wfhFont35", FontWeights.DemiBold),
        };

        private void CheckPropertyFontWeight(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing FontWeight Property");
            // for each item in FontWeight list
            foreach (MyMapType curType in s_myFontWeightList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.FontWeight, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region FontFamily
        // this is a list of WindowsFormsHost controls (by name) and what the 'FontFamily' property should be
        private static MyMapType[] s_myFontFamilyList = {
            new MyMapType("wfhFont1", "Arial"),
            new MyMapType("wfhFont2", "Comic Sans MS"),
            new MyMapType("wfhFont3", "Courier New"),
            new MyMapType("wfhFont4", "Estrangelo Edessa"),
            new MyMapType("wfhFont5", "Franklin Gothic"),
            new MyMapType("wfhFont6", "Gautami"),
            new MyMapType("wfhFont7", "Georgia"),
            new MyMapType("wfhFont8", "Global Monospace"),
            new MyMapType("wfhFont9", "Global Sans Serif"),
            new MyMapType("wfhFont10", "Global Serif"),
            new MyMapType("wfhFont11", "Global User Interface"),
            new MyMapType("wfhFont12", "Impact"),
            new MyMapType("wfhFont13", "Kartika"),
            new MyMapType("wfhFont14", "Latha"),
            new MyMapType("wfhFont15", "Lucida Console"),
            new MyMapType("wfhFont16", "Lucida Sans"),
            new MyMapType("wfhFont17", "Lucida Sans Unicode"),
            new MyMapType("wfhFont18", "Mangal"),
            new MyMapType("wfhFont19", "Marlett"),
            new MyMapType("wfhFont20", "Microsoft Sans Serif"),
            new MyMapType("wfhFont21", "MV Boli"),
            new MyMapType("wfhFont22", "Nina"),
            new MyMapType("wfhFont23", "Palatino Linotype"),
            new MyMapType("wfhFont24", "Raavi"),
            new MyMapType("wfhFont25", "Shruti"),
            new MyMapType("wfhFont26", "Sylfaen"),
            new MyMapType("wfhFont27", "Symbol"),
            new MyMapType("wfhFont28", "Tahoma"),
            new MyMapType("wfhFont29", "Times New Roman"),
            new MyMapType("wfhFont30", "Trebuchet MS"),
            new MyMapType("wfhFont31", "Tunga"),
            new MyMapType("wfhFont32", "Verdana"),
            new MyMapType("wfhFont33", "Vrinda"),
            new MyMapType("wfhFont34", "Webdings"),
            new MyMapType("wfhFont35", "Wingdings"),
        };

        private void CheckPropertyFontFamily(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing FontFamily Property");
            // for each item in FontFamily list
            foreach (MyMapType curType in s_myFontFamilyList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.FontFamily.Source, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region FontSize
        // this is a list of WindowsFormsHost controls (by name) and what the 'FontSize' property should be
        private static MyMapType[] s_myFontSizeList = {
            new MyMapType("wfhFont1", (double)4),
            new MyMapType("wfhFont2", (double)6),
            new MyMapType("wfhFont3", (double)8),
            new MyMapType("wfhFont4", (double)10),
            new MyMapType("wfhFont5", (double)12),
            new MyMapType("wfhFont6", (double)14),
            new MyMapType("wfhFont7", (double)16),
            new MyMapType("wfhFont8", (double)18),
            new MyMapType("wfhFont9", (double)20),
            new MyMapType("wfhFont10", (double)22),
            new MyMapType("wfhFont11", (double)24),
            new MyMapType("wfhFont12", (double)26),
            new MyMapType("wfhFont13", (double)28),
            new MyMapType("wfhFont14", (double)30),
            new MyMapType("wfhFont15", (double)32),
            new MyMapType("wfhFont16", (double)3),
            new MyMapType("wfhFont17", (double)5),
            new MyMapType("wfhFont18", (double)7),
            new MyMapType("wfhFont19", (double)9),
            new MyMapType("wfhFont20", (double)11),
            new MyMapType("wfhFont21", (double)13),
            new MyMapType("wfhFont22", (double)15),
            new MyMapType("wfhFont23", (double)17),
            new MyMapType("wfhFont24", (double)19),
            new MyMapType("wfhFont25", (double)21),
            new MyMapType("wfhFont26", (double)23),
            new MyMapType("wfhFont27", (double)25),
            new MyMapType("wfhFont28", (double)27),
            new MyMapType("wfhFont29", (double)29),
            new MyMapType("wfhFont30", (double)31),
            new MyMapType("wfhFont31", (double)10),
            new MyMapType("wfhFont32", (double)10),
            new MyMapType("wfhFont33", (double)10),
            new MyMapType("wfhFont34", (double)10),
            new MyMapType("wfhFont35", (double)10),
        };

        private void CheckPropertyFontSize(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing FontSize Property");
            // for each item in FontSize list
            foreach (MyMapType curType in s_myFontSizeList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    // ideally, I would like to use the item in the "MyMapType" structure as a double
                    // but I get an "invalid case" if I try the following:  (that's why I cast to double in array init)
                    //double expVal = (double)curType.propValue;
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.FontSize, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region ForceCursor
        // this is a list of WindowsFormsHost controls (by name) and what the 'ForceCursor' property should be
        private static MyMapType[] s_myForceCursorList = {
            new MyMapType("wfhFC1", true),
            new MyMapType("wfhFC2", false),
        };
        private void CheckPropertyForceCursor(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing ForceCursor Property");
            // for each item in ForceCursor list
            foreach (MyMapType curType in s_myForceCursorList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.ForceCursor, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region Foreground
        // this is a list of WindowsFormsHost controls (by name) and what the 'Foreground' property should be
        // (this is only valid for properties defined (in XAML) *not* using "Static Resources" - see below)
        private static MyMapType[] s_myForegroundList = {
            new MyMapType("wfhFGBlue", Brushes.Blue),
            //new MyMapType("wfhFGRed", Brushes.Red),
            new MyMapType("wfhFGGreen", Brushes.Green),
        };

        private void CheckPropertyForeground(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing Foreground Property");
            // for each item in Foreground list
            foreach (MyMapType curType in s_myForegroundList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.Foreground, "Property does not match", p.log);
                }
            }

            // see note in "CheckPropertyBackground"

            // now check properties that are defined using static resources
            CheckForegroundResource(p, sr, w, "labFGRed", "wfhFGRed");
            CheckForegroundResource(p, sr, w, "labFGYellowBlueLin", "wfhFGYellowBlueLin");
            CheckForegroundResource(p, sr, w, "labFGRedGreenLin", "wfhFGRedGreenLin");
            CheckForegroundResource(p, sr, w, "labFGYellowBlueRad", "wfhFGYellowBlueRad");
            CheckForegroundResource(p, sr, w, "labFGBitmap", "wfhFGBitmap");
        }

        /// <summary>
        /// Helper function to compare Foreground property of two UI elements defined in XAML file
        /// one element is a Label, the other is a WindowsFormsHost
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sr"></param>
        /// <param name="w"></param>
        /// <param name="idLabel"></param>
        /// <param name="idHost"></param>
        private void CheckForegroundResource(TParams p, ScenarioResult sr, Window w, string idLabel, string idHost)
        {
            p.log.WriteLine("Comparing label '{0}' with host '{1}'", idLabel, idHost);

            // get handle to Avalon label
            System.Windows.Controls.Label lab = (System.Windows.Controls.Label)w.FindName(idLabel);
            //p.log.WriteLine("labRed = '{0}'", labRed);

            // find host control in window
            WindowsFormsHost wfh = GetHostByName(p, sr, w, idHost);

            // compare foregound
            WPFMiscUtils.IncCounters(sr, lab.Foreground, wfh.Foreground, "Property does not match", p.log);
        }
        #endregion
        #region IsEnabled
        // this is a list of WindowsFormsHost controls (by name) and what the 'IsEnabled' property should be
        private static MyMapType[] s_myIsEnabledList = {
            new MyMapType("wfhIsEnabledTrue", true),
            new MyMapType("wfhIsEnabledFalse", false),
        };

        private void CheckPropertyIsEnabled(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing IsEnabled Property");
            // for each item in IsEnabled list
            foreach (MyMapType curType in s_myIsEnabledList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.IsEnabled, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region IsVisible
        // this is a list of WindowsFormsHost controls (by name) and what the 'IsVisible' property should be
        private static MyMapType[] s_myIsVisibleList = {
            new MyMapType("wfhIsVisibleTrue", true),
            new MyMapType("wfhIsVisibleFalse", false),
        };

        private void CheckPropertyIsVisible(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing IsVisible Property");
            // for each item in IsVisible list
            foreach (MyMapType curType in s_myIsVisibleList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.IsVisible, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region Padding
        // this is a list of WindowsFormsHost controls (by name) and what the 'Padding' property should be
        private static MyMapType[] s_myPaddingList = {
            new MyMapType("wfhPad1", new Thickness(4)),
            new MyMapType("wfhPadL", new Thickness(10, 0, 0, 0)),
            new MyMapType("wfhPadT", new Thickness(0, 10, 0, 0)),
            new MyMapType("wfhPadR", new Thickness(0, 0, 10, 0)),
            new MyMapType("wfhPadB", new Thickness(0, 0, 0, 10)),
        };

        private void CheckPropertyPadding(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing Padding Property");
            // for each item in Padding list
            foreach (MyMapType curType in s_myPaddingList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.Padding, "Property does not match", p.log);
                }
            };
        }
        #endregion
        #region Visibility
        // this is a list of WindowsFormsHost controls (by name) and what the 'Visibility' property should be
        private static MyMapType[] s_myVisibilityList = {
            new MyMapType("wfhVisVis", Visibility.Visible),
            new MyMapType("wfhVisHid", Visibility.Hidden),
            new MyMapType("wfhVisCol", Visibility.Collapsed),
            new MyMapType("wfhVisVis2", Visibility.Visible),
        };

        private void CheckPropertyVisibility(TParams p, ScenarioResult sr, Window w)
        {
            p.log.WriteLine("Testing Visibility Property");
            // for each item in Visibility list
            foreach (MyMapType curType in s_myVisibilityList)
            {
                p.log.WriteLine("Checking '{0}' for '{1}' ({2})",
                    curType.hostName, curType.propValue.ToString(), curType.propValue.GetType());

                // find host control in window
                WindowsFormsHost wfh = GetHostByName(p, sr, w, curType.hostName);
                if (wfh != null)
                {
                    // get property value, compare with expected value
                    //bool b = (wfh.Cursor == curType.cursor);
                    //p.log.WriteLine("  Value is '{0}'", b);
                    WPFMiscUtils.IncCounters(sr, curType.propValue, wfh.Visibility, "Property does not match", p.log);
                }
            };
        }
        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Helper function to read Xaml code from file and create a Window
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private Window WindowFromXamlFile(string filename)
        {
            // spruce this up, improve error checking, maybe make a library function? !!!

            Window w = null;
            System.IO.FileStream fs = null;

            // try to open file
            try
            {
                // open file
                fs = System.IO.File.Open(filename, System.IO.FileMode.Open);
            }
            catch (System.IO.FileNotFoundException e)
            {
                // log fact that we could not open the file, continue
                scenarioParams.log.WriteLine("WindowFromXamlFile: got exception '{0}'", e.Message);
                return null;
            }

            // try to parse file into Window
            try
            {
                // read file, parse into window
                w = (Window)System.Windows.Markup.XamlReader.Load(fs);
            }
            catch (System.Windows.Markup.XamlParseException e)
            {
                scenarioParams.log.WriteLine("WindowFromXamlFile2: got exception '{0}'", e.Message);
                fs.Close();
                return null;
            }
            catch (Exception e)
            {
                scenarioParams.log.WriteLine("WindowFromXamlFile3: got exception '{0}'", e.Message);
                fs.Close();
                return null;
            }

            if (fs != null)
            {
                fs.Close();
            }

            return w;
        }

        /// <summary>
        /// Get access to WindowsFormsHost control with specified name in a window 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sr"></param>
        /// <param name="w"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static WindowsFormsHost GetHostByName(TParams p, ScenarioResult sr, Window w, string name)
        {
            WindowsFormsHost wfh = (WindowsFormsHost)w.FindName(name);
            if (wfh == null)
            {
                // log as failure, write meaningful message to log file
                string msg = String.Format("Cannot find WindowsFormsHost named '{0}' in '{1}'", name, w.Title);
                WPFMiscUtils.IncCounters(sr, p.log, false, msg);
            }
            return wfh;
        }
        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify that each property we map can be set in XAML

//@ Verify that Background works with different brushes (gradiant/solid).
