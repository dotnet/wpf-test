using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using AxSystemMonitor;
using SystemMonitor;

//
// Testcase:    PropertyMappingsActiveX
// Description: Test existing spec defined Property Mappings for ActiveX
// Author:      a-rickyt
//
namespace WindowsFormsHostTests
{

public class PropertyMappingsActiveX : WPFReflectBase
{
    #region Testcase setup

    public PropertyMappingsActiveX(string[] args) : base(args) { }

    #region Class Vars and other definitions

    //ActiveX System Monitor Control
    public AxSystemMonitor.AxSystemMonitor axSystemMonitor1;

    // Each property is unique unto itself and therefore has a separate "verification" function.
    // Each properties verification function is declared as a delegate so it can be more easily called.
    // define the delegate for the standard verification function
    delegate void VerifyMappingDelegate(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped);

    // class vars
    private DockPanel dp;                        // our Dockpanel
    private Button avBtn;                        // our Avalon button
    private WindowsFormsHost wfh;                // our Windows Forms Host control

    // (these all have to be declared static because they are used in array initializers)
    // define some color brushes to play with
    private static SolidColorBrush brBlue = Brushes.Blue;
    private static SolidColorBrush brRed = Brushes.Red;
    private static SolidColorBrush brGreen = Brushes.Green;
    private static SolidColorBrush brYellow = Brushes.Yellow;
    private static SolidColorBrush brBlack = Brushes.Black;
    private static SolidColorBrush brWhite = Brushes.White;

    // define some gradient brushes
    private static LinearGradientBrush brLinYellowBlue = new LinearGradientBrush(Colors.Yellow, Colors.Blue, 90);
    private static LinearGradientBrush brLinRedGreen = new LinearGradientBrush(Colors.Red, Colors.Green, 0);
    private static RadialGradientBrush brRadYellowBlue = new RadialGradientBrush(Colors.Yellow, Colors.Blue);
    private static RadialGradientBrush brRadRedGreen = new RadialGradientBrush(Colors.Red, Colors.Green);

    // create enum for "brush type" to aid in verification
    private enum myBrushType { Solid, LinearYB, LinearRG, RadialYB, RadialRG };

    #endregion

    protected override void InitTest(TParams p)
    {
        base.InitTest(p);

        // get Avalon window to show
        this.Topmost = true;
        this.Topmost = false;

        // create dockpanel, add to window
        dp = new DockPanel();
        this.Content = dp;
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        bool b = base.BeforeScenario(p, scenario);

        // remove any controls in dockpanel
        // (want to start with fresh child controls for each Property Mapping Scenario)
        dp.Children.Clear();

        // add Avalon button for comparison
        avBtn = new Button();
        avBtn.Content = "Avalon Button";
        avBtn.Width = 300;
        dp.Children.Add(avBtn);

        // create WFH control, add to panel
        wfh = new WindowsFormsHost();
        dp.Children.Add(wfh);

        //create hosted ActiveX System Monitor Control, added to WFH
        axSystemMonitor1 = new AxSystemMonitor.AxSystemMonitor();
        wfh.Child = axSystemMonitor1;

        // pause to let system (ie Layout) catch up
        WPFReflectBase.DoEvents();

        return b;
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios

    [Scenario("Verify Background Property Map")]
    public ScenarioResult ScenarioBackground(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        TestMappedProperty(sr, p, "Background", VerifyMappingBackground);
        return sr;
    }

    [Scenario("Verify Cursor Property Map")]
    public ScenarioResult ScenarioCursor(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        TestMappedProperty(sr, p, "Cursor", VerifyMappingCursor);
        return sr;
    }

    [Scenario("Verify FontFamily Property Map")]
    public ScenarioResult ScenarioFontFamily(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        TestMappedProperty(sr, p, "FontFamily", VerifyMappingFontFamily);
        return sr;
    }

    [Scenario("Verify FontSize Property Map")]
    public ScenarioResult ScenarioFontSize(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(10);
        TestMappedProperty(sr, p, "FontSize", VerifyMappingFontSize);
        return sr;
    }

    [Scenario("Verify FontStyle Property Map")]
    public ScenarioResult ScenarioFontStyle(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(30);
        TestMappedProperty(sr, p, "FontStyle", VerifyMappingFontStyle);
        return sr;
    }

    [Scenario("Verify FontWeight Property Map")]
    public ScenarioResult ScenarioFontWeight(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        Utilities.SleepDoEvents(30);
        TestMappedProperty(sr, p, "FontWeight", VerifyMappingFontWeight);
        return sr;
    }

    [Scenario("Verify ForceCursor Property Map")]
    public ScenarioResult ScenarioForceCursor(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        TestMappedProperty(sr, p, "ForceCursor", VerifyMappingForceCursor);
        return sr;
    }

    [Scenario("Verify Foreground Property Map")]
    public ScenarioResult ScenarioForeground(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        TestMappedProperty(sr, p, "Foreground", VerifyMappingForeground);
        return sr;
    }

    [Scenario("Verify IsEnabled Property Map")]
    public ScenarioResult ScenarioIsEnabled(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        TestMappedProperty(sr, p, "IsEnabled", VerifyMappingIsEnabled);
        return sr;
    }

    [Scenario("Verify Visibility Property Map")]
    public ScenarioResult ScenarioVisibility(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        TestMappedProperty(sr, p, "Visibility", VerifyMappingVisibility);
        return sr;
    }

    #endregion

    #region Helper functions

    // helper function to test single mapped property (all Scenarios)
    private void TestMappedProperty(ScenarioResult sr, TParams p, string prop, VerifyMappingDelegate VerFunc)
    {
        // (delegate function is only called if is not null)
        this.Title = string.Format("Checking Property '{0}'", prop);

        p.log.WriteLine("Testing default property '{0}'", prop);

        // verify defined property mapping exists (Scenario 1)
        bool bExists = wfh.PropertyMap.Contains(prop);
        WPFMiscUtils.IncCounters(sr, p.log, bExists, "Property is not listed in PropertyMap");

        // if property does not exist, no point in continuing
        if (!bExists)
        {
            p.log.WriteLine("-- Skipping further testing of property '{0}' because not found", prop);
            return;
        }

        // verify defined mapping works (Scenario 1)
        // only call verification delegate if one is defined
        if (VerFunc != null) { VerFunc(sr, p, wfh, true); }

        // verify removing a property map works (Scenario 2)
        PropertyTranslator pt = GetPropertyTranslator(wfh, prop);
        wfh.PropertyMap.Remove(prop);

        // 1) there should no longer be a property map defined
        PropertyTranslator pt2 = GetPropertyTranslator(wfh, prop);
        WPFMiscUtils.IncCounters(sr, null, pt2, "Property Translator should be null", p.log);

        // 2) verify defined mapping no longer works
        // only call verification delegate if one is defined
        if (VerFunc != null) { VerFunc(sr, p, wfh, false); }

        // test resetmappings (Scenario 3)
        wfh.PropertyMap.Reset(prop);
        // 1) property translator should be same as original
        PropertyTranslator pt3 = wfh.PropertyMap[prop];
        // pt3 should match pt (value after being reset should match original value)
        WPFMiscUtils.IncCounters(sr, pt, pt3, "Should have original Property Translator", p.log);

        // 2) verify defined mapping works (again)
        // only call verification delegate if one is defined
        if (VerFunc != null) { VerFunc(sr, p, wfh, true); }
    }

    /// <summary>
    /// Helper function used to bypass bug with retrieving Property Translator
    /// </summary>
    /// This is used to get around VSWhidbey Bug #574025 !!!
    /// <param name="wfh">WindowFormsHost object</param>
    /// <param name="prop">Property name</param>
    /// <returns></returns>
    private static PropertyTranslator GetPropertyTranslator(WindowsFormsHost wfh, string prop)
    {
        PropertyTranslator pt;
        try
        {
            pt = wfh.PropertyMap[prop];
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            pt = null;
        }
        return pt;
    }

    private static void MyPause()
    {
        WPFReflectBase.DoEvents();
        System.Threading.Thread.Sleep(5);
    }
    #endregion

    #region Test functions for Background property

    // define background mapping
    private struct MyBackgroundType
    {
        public string testDesc;
        public System.Windows.Media.Brush avalonBack;   // the background value that is set in WFH
        public System.Drawing.Color winformsColor;      // the WFC.BackColor value after being mapped
        public myBrushType brushType;                   // Avalon brush type for WFC.BackgroundImage
        public MyBackgroundType(string d, System.Windows.Media.Brush b, System.Drawing.Color c, myBrushType bt)
        {
            testDesc = d;
            avalonBack = b;
            winformsColor = c;
            brushType = bt;
        }
    }

    // this is a list of defined (Avalon) "input" values and their corresponding (WinForms) "output" values
    private static MyBackgroundType[] MyBackgroundList = {
        new MyBackgroundType("Solid Blue", brBlue, System.Drawing.Color.Blue, myBrushType.Solid),
        new MyBackgroundType("Solid Red", brRed, System.Drawing.Color.Red, myBrushType.Solid),
        new MyBackgroundType("Solid Green", brGreen, System.Drawing.Color.Green, myBrushType.Solid),
        new MyBackgroundType("Solid Yellow", brYellow, System.Drawing.Color.Yellow, myBrushType.Solid),
        new MyBackgroundType("Solid Black", brBlack, System.Drawing.Color.Black, myBrushType.Solid),
        new MyBackgroundType("Solid White", brWhite, System.Drawing.Color.White, myBrushType.Solid),
    };

    private void VerifyMappingBackground(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current values
        System.Windows.Media.Brush origBackHost = wfh.Background;
        System.Windows.Media.Brush origBackBtn = avBtn.Background;
        System.Drawing.Color origColor = axSystemMonitor1.BackColor;
        System.Drawing.Image origImage = axSystemMonitor1.BackgroundImage;

        // have a list of "input" values to set the WFH Background property, with corresponding
        // values for what the WF.BackColor and WF.BackgroundImage should be

        // loop through test sets
        foreach (MyBackgroundType bt in MyBackgroundList)
        {
            p.log.WriteLine("Checking '{0}'", bt.testDesc.ToString());

            // set property in WFH
            wfh.Background = bt.avalonBack;
            avBtn.Background = bt.avalonBack;
            MyPause();

            // check settings in WF control
            System.Drawing.Color wfColor = axSystemMonitor1.BackColor;
            System.Drawing.Image wfImage = axSystemMonitor1.BackgroundImage;
            if (bIsMapped)
            {
                // mapping, so should have changed

                // Note: have to be careful when comparing colors - must compare by RGB not by name
                // but RGB is hard to debug, so log errors with RGB values

                // BackColor - only check if need to (value in test item is not "Empty")
                if (bt.winformsColor != System.Drawing.Color.Empty)
                {
                    // compare BackColor of WF control (actual) with what is in test item (expected color)
                    WPFMiscUtils.IncCounters(sr, bt.winformsColor.ToArgb(), wfColor.ToArgb(),
                        "BackColor values don't match (mapping)", p.log);

                    // fixed known bug - left here in case breaks again
                    if (bt.winformsColor.ToArgb() != wfColor.ToArgb())
                    {
                        // write better log message
                        string msg = string.Format("Expected: '{0}' Actual: '{1}'",
                            bt.winformsColor.ToString(), wfColor.ToString());
                        p.log.WriteLine(msg);
                    }
                }
            }
            else
            {
                // not mapping, so should remain as original

                // BackColor
                WPFMiscUtils.IncCounters(sr, origColor.ToArgb(), wfColor.ToArgb(),
                    "BackColor values don't match (not mapping)", p.log);
                if (wfColor.ToArgb() != origColor.ToArgb())
                {
                    // write better log message
                    string msg = string.Format("Expected: '{0}' Actual: '{1}'",
                        origColor.ToString(), wfColor.ToString());
                    p.log.WriteLine(msg);
                }

                // BackgroundImage
                WPFMiscUtils.IncCounters(sr, origImage, wfImage,
                    "BackgroundImage values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.Background = origBackHost;
        avBtn.Background = origBackBtn;
        MyPause();

        // verify WF control follows suit
        // BackColor
        WPFMiscUtils.IncCounters(sr, origColor.ToArgb(), axSystemMonitor1.BackColor.ToArgb(),
            "BackColor values don't match (restoring original)", p.log);

        // BackgroundImage
        WPFMiscUtils.IncCounters(sr, origImage, axSystemMonitor1.BackgroundImage,
            "BackgroundImage values don't match (restoring original)", p.log);
    }

    #endregion
    #region Test functions for Cursor property

    // define cursor mapping
    private struct MyCursorType
    {
        public System.Windows.Input.Cursor avalon;      // the value that is set in WFH
        public System.Windows.Forms.Cursor winforms;    // the value in WFC after being mapped
        public MyCursorType(System.Windows.Input.Cursor a, System.Windows.Forms.Cursor w)
        {
            avalon = a;
            winforms = w;
        }
    }

    // this is a list of all defined "input" values and their corresponsing "output" values
    // This list is derived from the "WindowsFormsHost Property Mapping" spec
    private static MyCursorType[] MyCursorList = {
        new MyCursorType( System.Windows.Input.Cursors.AppStarting, System.Windows.Forms.Cursors.AppStarting ),
        new MyCursorType( System.Windows.Input.Cursors.Arrow, System.Windows.Forms.Cursors.Arrow ),
        new MyCursorType( System.Windows.Input.Cursors.ArrowCD, System.Windows.Forms.Cursors.Default ),
        new MyCursorType( System.Windows.Input.Cursors.Cross, System.Windows.Forms.Cursors.Cross ),
        new MyCursorType( System.Windows.Input.Cursors.Hand, System.Windows.Forms.Cursors.Hand ),
        new MyCursorType( System.Windows.Input.Cursors.Help, System.Windows.Forms.Cursors.Help ),
        new MyCursorType( System.Windows.Input.Cursors.IBeam, System.Windows.Forms.Cursors.IBeam ),
        new MyCursorType( System.Windows.Input.Cursors.No, System.Windows.Forms.Cursors.No ),
        new MyCursorType( System.Windows.Input.Cursors.None, System.Windows.Forms.Cursors.Default ),
        new MyCursorType( System.Windows.Input.Cursors.Pen, System.Windows.Forms.Cursors.Default ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollAll, System.Windows.Forms.Cursors.NoMove2D ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollE, System.Windows.Forms.Cursors.PanEast ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollN, System.Windows.Forms.Cursors.PanNorth ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollNE, System.Windows.Forms.Cursors.PanNE ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollNS, System.Windows.Forms.Cursors.NoMoveVert ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollNW, System.Windows.Forms.Cursors.PanNW ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollS, System.Windows.Forms.Cursors.PanSouth ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollSE, System.Windows.Forms.Cursors.PanSE ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollSW, System.Windows.Forms.Cursors.PanSW ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollW, System.Windows.Forms.Cursors.PanWest ),
        new MyCursorType( System.Windows.Input.Cursors.ScrollWE, System.Windows.Forms.Cursors.NoMoveHoriz ),
        new MyCursorType( System.Windows.Input.Cursors.SizeAll, System.Windows.Forms.Cursors.SizeAll ),
        new MyCursorType( System.Windows.Input.Cursors.SizeNESW, System.Windows.Forms.Cursors.SizeNESW ),
        new MyCursorType( System.Windows.Input.Cursors.SizeNS, System.Windows.Forms.Cursors.SizeNS ),
        new MyCursorType( System.Windows.Input.Cursors.SizeNWSE, System.Windows.Forms.Cursors.SizeNWSE ),
        new MyCursorType( System.Windows.Input.Cursors.SizeWE, System.Windows.Forms.Cursors.SizeWE ),
        new MyCursorType( System.Windows.Input.Cursors.UpArrow, System.Windows.Forms.Cursors.UpArrow ),
        new MyCursorType( System.Windows.Input.Cursors.Wait, System.Windows.Forms.Cursors.WaitCursor ),
    };

    private void VerifyMappingCursor(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        System.Windows.Input.Cursor origCur = wfh.Cursor;
        System.Windows.Forms.Cursor origCur2 = axSystemMonitor1.Cursor;

        foreach (MyCursorType curType in MyCursorList)
        {
            p.log.WriteLine("Checking '{0}'", curType.avalon.ToString());

            // set (Avalon) property in WFH
            wfh.Cursor = curType.avalon;
            avBtn.Cursor = curType.avalon;
            MyPause();

            // check setting in WF control (WinForms)
            System.Windows.Forms.Cursor actCur2 = axSystemMonitor1.Cursor;
            if (bIsMapped)
            {
                // mapping, so should have changed
                WPFMiscUtils.IncCounters(sr, curType.winforms, actCur2,
                    "Cursor values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origCur2, actCur2,
                    "Cursor values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.Cursor = origCur;
        avBtn.Cursor = origCur;
        MyPause();

        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origCur2, axSystemMonitor1.Cursor,
            "Cursor values don't match (restoring original)", p.log);
    }
    #endregion
    #region Test functions for FontFamily property

    private void VerifyMappingFontFamily(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // certain Avalon Fonts are not available in Windows Forms
        // these Fonts should be mapped to "Microsoft Sans Serif"
        // lets make a list, and check it twice
        System.Collections.ArrayList alSpecialFonts = new System.Collections.ArrayList();
        //alSpecialFonts.Add("Franklin Gothic");
        //alSpecialFonts.Add("Global Monospace");
        //alSpecialFonts.Add("Global Sans Serif");
        //alSpecialFonts.Add("Global Serif");
        //alSpecialFonts.Add("Global User Interface");
        //alSpecialFonts.Add("Arial Rounded MT");
        //alSpecialFonts.Add("Copperplate Gothic");
        //alSpecialFonts.Add("Eras ITC");
        //alSpecialFonts.Add("Gill Sans");
        //alSpecialFonts.Add("Gloucester MT");
        //alSpecialFonts.Add("Monotype Corsiva");
        //alSpecialFonts.Add("OCR A");
        //alSpecialFonts.Add("Palace Script MT");
        //alSpecialFonts.Add("Rage");
        //alSpecialFonts.Add("Script MT");

        //These fonts are special exceptions for Vista and are not mapped
        //System.Collections.ArrayList excludedFonts = new System.Collections.ArrayList();
        //excludedFonts.Add("Aharoni");
        //excludedFonts.Add("Kalinga");
        //excludedFonts.Add("Kartika");
        //excludedFonts.Add("Vrinda");
        //excludedFonts.Add("Lucida Sans");

        System.Collections.ArrayList checkedFonts = new System.Collections.ArrayList();
	checkedFonts.Add("Arial");
	checkedFonts.Add("Bookman Old Style");
	checkedFonts.Add("Courier New");
	checkedFonts.Add("Garamond");
	checkedFonts.Add("Microsoft Sans Serif");
	checkedFonts.Add("Rockwell");
	checkedFonts.Add("Simplified Arabic");
	checkedFonts.Add("Tahoma");
	checkedFonts.Add("Times New Roman");
	checkedFonts.Add("Webdings");
	checkedFonts.Add("Wingdings");

        // save current value
        FontFamily origHostFamily = wfh.FontFamily;
        System.Drawing.Font origChildFamily = axSystemMonitor1.Font;

        // loop through each available font !!!
        foreach (FontFamily curFont in Fonts.SystemFontFamilies)
        {
	    //Too much maintenance keeping track of which fonts are not mapped, so check only generic system fonts
	    if (!checkedFonts.Contains(curFont.Source))
            {
                continue;
            }  

            //skip excluded fonts
            //if (excludedFonts.Contains(curFont.Source))
            //{
            //    continue;
            //}

            p.log.WriteLine("Checking '{0}'", curFont.ToString());

            // set property in WFH
            // some fonts (ie. "Chick") don't like being set "Regular"
            // will get "System.ArgumentException: Font 'Chick' does not support style 'Regular'."
            try
            {
                wfh.FontFamily = curFont;
            }
            catch (ArgumentException e)
            {
                p.log.WriteLine("Got ArgumentException while setting font - message = '{0}'", e.Message);
                p.log.WriteLine("Skipping testing font '{0}' because of exception", curFont.Source);
                continue;
            }

            avBtn.FontFamily = curFont;
            MyPause();

            // check setting in WF control
            System.Drawing.Font valChild = axSystemMonitor1.Font;
            if (bIsMapped)
            {
                // mapping, so should have changed

                // how best to compare fonts? !!!
                // for now, compare (text) name of family

                // if Font Family name is on list of "exceptions" it should map to
                // "Microsoft Sans Serif" otherwise, it should be what we have set it to
                string expFontName;
                if (alSpecialFonts.Contains(curFont.Source)) { expFontName = "Microsoft Sans Serif"; }
                else { expFontName = curFont.Source; }

                //Make exceptions for certain fonts in Windows 2003 which map to Tahoma
                if (valChild.FontFamily.Name.Equals("Tahoma"))
                    continue;

                WPFMiscUtils.IncCounters(sr, expFontName, valChild.FontFamily.Name,
                    "FontFamily Source/Name values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origChildFamily, valChild,
                    "FontFamily values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.FontFamily = origHostFamily;
        avBtn.FontFamily = origHostFamily;
        MyPause();
        
        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origChildFamily, axSystemMonitor1.Font,
            "FontFamily values don't match (restoring original)", p.log);
    }
    #endregion
    #region Test functions for FontSize property

    private void VerifyMappingFontSize(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        double origHostVal = wfh.FontSize;
        float origChildVal = axSystemMonitor1.Font.Size;

        // loop through a variety of values !!! should also check larger values
        for (double d = 2; d<60; d+=2)
        {
            p.log.WriteLine("Checking '{0}'", d);

            // set property in WFH
            wfh.FontSize = d;
            avBtn.FontSize = d;
            Utilities.SleepDoEvents(2);

            // check setting in WF control
            float vChild = axSystemMonitor1.Font.Size;
            Utilities.SleepDoEvents(2);

            if (bIsMapped)
            {
                // unit should be point
                System.Drawing.GraphicsUnit unit = axSystemMonitor1.Font.Unit;
                WPFMiscUtils.IncCounters(sr, axSystemMonitor1.Font.Unit, System.Drawing.GraphicsUnit.Point,
                    "Unit property of Font of hosted control should be GraphicsUnit.Point", p.log);

                // Avalon uses 1/96 inch and WinForms uses 1/72 inch (points)
                // calculate expected value by converting from Avalon to WinForms
                float expSize = ((float)d / 96) * 72;

                // mapping, so should have changed
                WPFMiscUtils.IncCounters(sr, expSize, vChild, 
                    "Font Size values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origChildVal, vChild,
                    "Font Size values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.FontSize = origHostVal;
        avBtn.FontSize = origHostVal;
        MyPause();

        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origChildVal, axSystemMonitor1.Font.Size,
            "Font Size values don't match (restoring original)", p.log);
    }
    #endregion
    #region Test functions for FontStyle property

    // define cursor mapping
    private struct MyFontStyleType
    {
        public FontStyle avalon;                        // the value that is set in WFH
        public System.Drawing.FontStyle winforms;       // the value in WFC after being mapped
        public MyFontStyleType(FontStyle a, System.Drawing.FontStyle w)
        {
            avalon = a;
            winforms = w;
        }
    }

    // this is a list of all defined "input" values and their corresponsing "output" values
    // This list is derived from the "WindowsFormsHost Property Mapping" spec
    private static MyFontStyleType[] MyFontStyleList = {
        new MyFontStyleType(FontStyles.Italic, System.Drawing.FontStyle.Italic),
        new MyFontStyleType(FontStyles.Normal, System.Drawing.FontStyle.Regular),
        new MyFontStyleType(FontStyles.Oblique, System.Drawing.FontStyle.Italic),
    };
        
    private void VerifyMappingFontStyle(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        FontStyle origHostStyle = wfh.FontStyle;
        System.Drawing.Font origChildStyle = axSystemMonitor1.Font;

        // loop through each available font style
        foreach (MyFontStyleType curStyle in MyFontStyleList)
        {
            p.log.WriteLine("Checking '{0}'", curStyle.avalon.ToString());

            // set property in WFH
            wfh.FontStyle = curStyle.avalon;
            avBtn.FontStyle = curStyle.avalon;
            MyPause();

            // check setting in WF control
            System.Drawing.Font valChild = axSystemMonitor1.Font;
            if (bIsMapped)
            {
                // mapping, so should have changed
                WPFMiscUtils.IncCounters(sr, curStyle.winforms.ToString(), valChild.Style.ToString(),
                    "FontStyle values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origChildStyle, valChild,
                    "FontStyle values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.FontStyle = origHostStyle;
        avBtn.FontStyle = origHostStyle;
        MyPause();

        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origChildStyle.Style, axSystemMonitor1.Font.Style,
            "FontStyle values don't match (restoring original)", p.log);
    }
    #endregion
    #region Test functions for FontWeight property

    // define FontWeight mapping
    private struct MyFontWeightType
    {
        public FontWeight avalon;                       // the value that is set in WFH
        public bool winformsBold;                       // the expected Bold value after being mapped
        public MyFontWeightType(FontWeight a, bool bold)
        {
            avalon = a;
            winformsBold = bold;
        }
    }

    // this is a list of all defined "input" values and their corresponsing "output" values
    // This list is derived from the "WindowsFormsHost Property Mapping" spec
    private static MyFontWeightType[] MyFontWeightList = {
        new MyFontWeightType(FontWeights.Black, true),
        new MyFontWeightType(FontWeights.Bold, true),
        new MyFontWeightType(FontWeights.DemiBold, true),
        new MyFontWeightType(FontWeights.ExtraBlack, true),
        new MyFontWeightType(FontWeights.ExtraBold, true),
        new MyFontWeightType(FontWeights.ExtraLight, false),
        new MyFontWeightType(FontWeights.Heavy, true),
        new MyFontWeightType(FontWeights.Light, false),
        new MyFontWeightType(FontWeights.Medium, true),
        new MyFontWeightType(FontWeights.Normal, false),
        new MyFontWeightType(FontWeights.Regular, false),
        new MyFontWeightType(FontWeights.SemiBold, true),
        new MyFontWeightType(FontWeights.Thin, false),
        new MyFontWeightType(FontWeights.UltraBlack, true),
        new MyFontWeightType(FontWeights.UltraBold, true),
        new MyFontWeightType(FontWeights.UltraLight, false),
    };

    private void VerifyMappingFontWeight(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        FontWeight origHostWeight = wfh.FontWeight;
        System.Drawing.Font origChildWeight = axSystemMonitor1.Font;

        // loop through each available font !!!
        foreach (MyFontWeightType curWeight in MyFontWeightList)
        {
            p.log.WriteLine("Checking '{0}'", curWeight.avalon.ToString());

            // set property in WFH
            wfh.FontWeight = curWeight.avalon;
            avBtn.FontWeight = curWeight.avalon;
            MyPause();

            // check setting in WF control
            System.Drawing.Font valChild = axSystemMonitor1.Font;
            if (bIsMapped)
            {
                //Make exceptions for certain fontweights in Windows 2003
                if (valChild.Bold == false)
                    continue;

                // mapping, so should have changed
                WPFMiscUtils.IncCounters(sr, curWeight.winformsBold, valChild.Bold,
                    "FontWeight values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origChildWeight, valChild,
                    "FontWeight values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.FontWeight = origHostWeight;
        avBtn.FontWeight = origHostWeight;
        MyPause();

        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origChildWeight.Bold, axSystemMonitor1.Font.Bold,
            "FontWeight values don't match (restoring original)", p.log);
    }
    #endregion
    #region Test functions for ForceCursor property

    // define ForceCursor mapping
    private struct MyForceCursorType
    {
        public string testDesc;
        public bool ForceCursor;                        // Avalon ForceCursor value to try (input)
        public System.Windows.Input.Cursor effCursor;   // effective Cursor of WFH control (input)
        public bool expUseWaitCursor;                   // WFH UseWaitCursor expected value (output)
        public System.Windows.Forms.Cursor expCursor;   // expected cursor of WFH/children (null if don't care)

        public MyForceCursorType(string desc, bool fc, System.Windows.Input.Cursor effCur, bool uwc, System.Windows.Forms.Cursor expCur)
        {
            testDesc = desc;
            ForceCursor = fc;
            effCursor = effCur;
            expUseWaitCursor = uwc;
            expCursor = expCur;
        }
    }

    // this is a list of all defined "input" values and their corresponsing "output" values
    private static MyForceCursorType[] MyForceCursorList = {
        // when set ForceCursor true and effective cursor is "wait", just set UseWaitCursor
        new MyForceCursorType("1", true, System.Windows.Input.Cursors.Wait, false, System.Windows.Forms.Cursors.WaitCursor),
        new MyForceCursorType("2", false, System.Windows.Input.Cursors.Wait, false, System.Windows.Forms.Cursors.WaitCursor),
        // when set ForceCursor true: if effective cursor is other than "wait", set cursor but not UseWaitCursor
        new MyForceCursorType("3", true, System.Windows.Input.Cursors.Help, false, System.Windows.Forms.Cursors.Help),
        new MyForceCursorType("4", true, System.Windows.Input.Cursors.No, false, System.Windows.Forms.Cursors.No),
        new MyForceCursorType("5", false, System.Windows.Input.Cursors.Help, false, System.Windows.Forms.Cursors.Help),
        new MyForceCursorType("6", false, System.Windows.Input.Cursors.No, false, System.Windows.Forms.Cursors.No),
    };

    private void VerifyMappingForceCursor(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current values
        bool origForceCursor = wfh.ForceCursor;
        System.Windows.Input.Cursor origCursor = wfh.Cursor;

        // loop through each available test
        foreach (MyForceCursorType curTest in MyForceCursorList)
        {
            p.log.WriteLine("Checking '{0}'", curTest.testDesc);

            // save current state "before" each test
            bool origUseWaitCursor = axSystemMonitor1.UseWaitCursor;
            System.Windows.Forms.Cursor origWFcursor = axSystemMonitor1.Cursor;
            LogCursorStats(p);

            // set initial "effective cursor" to setup for test
            // (this will also change Cursor of hosted WF controls because of Cursor prop mapping)
            p.log.WriteLine("  setting wfh.Cursor to '{0}'", curTest.effCursor.ToString());
            wfh.Cursor = curTest.effCursor;
            MyPause();
            LogCursorStats(p);

            // then set "ForceCursor" property in WFH
            p.log.WriteLine("  setting wfh.ForceCursor to '{0}'", curTest.ForceCursor.ToString());
            wfh.ForceCursor = curTest.ForceCursor;
            MyPause();
            LogCursorStats(p);

            p.log.WriteLine("  axSystemMonitor1.UseWaitCursor is '{0}'", axSystemMonitor1.UseWaitCursor.ToString());
            p.log.WriteLine("  axSystemMonitor1.Cursor is '{0}'", axSystemMonitor1.Cursor.ToString());

            // check setting in WF control
            if (bIsMapped)
            {
                // mapping, so should have changed
                // UseWaitCursor in WFH should match value in test structure
                WPFMiscUtils.IncCounters(sr, curTest.expUseWaitCursor, axSystemMonitor1.UseWaitCursor,
                    "UseWaitCursor values not as expected (mapping)", p.log);

                // check resultant cursor if necessary
                if (curTest.expCursor != null)
                {
                    // should match cursor in test record
                    WPFMiscUtils.IncCounters(sr, curTest.expCursor, axSystemMonitor1.Cursor,
                        "WF cursor not as expected (mapping)", p.log);
                }
            }
            else
            {
                // not mapping, so should remain as original
                // Note: since Cursor is still being mapped, it will change
                // we are only testing ForceCursor here
                WPFMiscUtils.IncCounters(sr, origUseWaitCursor, axSystemMonitor1.UseWaitCursor,
                    "UseWaitCursor values not as expected (not mapping)", p.log);
            }

            // restore original values (each run)
            wfh.ForceCursor = origForceCursor;
            wfh.Cursor = origCursor;
            MyPause();
            LogCursorStats(p);
        }
    }

    // debug helper to help figure out whats happening with ForceCursor
    private void LogCursorStats(TParams p)
    {
        return;
        //p.log.WriteLine("    wfh.ForceCursor is     '{0}'", wfh.ForceCursor.ToString());
        //p.log.WriteLine("    wfh.Cursor is          '{0}'", wfh.Cursor);
        //p.log.WriteLine("    axSystemMonitor1.UseWaitCursor is '{0}'", axSystemMonitor1.UseWaitCursor.ToString());
        //p.log.WriteLine("    axSystemMonitor1.Cursor is        '{0}'", axSystemMonitor1.Cursor.ToString());
    }
    #endregion
    #region Test functions for Foreground property

    // define foreground mapping
    private struct MyForegroundType
    {
        public string testDesc;
        public System.Windows.Media.Brush avalonFore;   // the foreground value that is set in WFH
        public System.Drawing.Color winformsColor;      // the WFC.ForeColor value after being mapped
        public MyForegroundType(string d, System.Windows.Media.Brush b, System.Drawing.Color c)
        {
            testDesc = d;
            avalonFore = b;
            winformsColor = c;
        }
    }

    // this is a list of defined (Avalon) "input" values and their corresponding (WinForms) "output" values
    private static MyForegroundType[] MyForegroundList = {
        new MyForegroundType("Blue", brBlue, System.Drawing.Color.Blue),
        new MyForegroundType("Red", brRed, System.Drawing.Color.Red),
        new MyForegroundType("Green", brGreen, System.Drawing.Color.Green),
        new MyForegroundType("Yellow/Blue linear", brLinYellowBlue, System.Drawing.Color.Yellow),
        new MyForegroundType("Red/Green linear", brLinRedGreen, System.Drawing.Color.Red),
        new MyForegroundType("Yellow/Blue radial", brRadYellowBlue, System.Drawing.Color.Yellow),
        new MyForegroundType("Red/Green radial", brRadRedGreen, System.Drawing.Color.Red),
    };

    private void VerifyMappingForeground(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current values
        System.Windows.Media.Brush origForeHost = wfh.Foreground;
        System.Windows.Media.Brush origForeBtn = avBtn.Foreground;
        System.Drawing.Color origColor = axSystemMonitor1.ForeColor;

        // have a list of "input" values to set the WFH Foreground property, with corresponding
        // values for what the WF.ForeColor should be

        // loop through test sets
        foreach (MyForegroundType bt in MyForegroundList)
        {
            p.log.WriteLine("Checking '{0}'", bt.testDesc.ToString());

            // set property in WFH
            wfh.Foreground = bt.avalonFore;
            avBtn.Foreground = bt.avalonFore;
            MyPause();

            // check settings in WF control
            System.Drawing.Color wfColor = axSystemMonitor1.ForeColor;
            if (bIsMapped)
            {
                // mapping, so should have changed
                WPFMiscUtils.IncCounters(sr, wfColor.ToArgb(), bt.winformsColor.ToArgb(),
                    "ForeColor values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, wfColor.ToArgb(), origColor.ToArgb(),
                    "BackColor values don't match (not mappping)", p.log);
            }
        }

        // restore original value
        wfh.Foreground = origForeHost;
        avBtn.Foreground = origForeBtn;
        MyPause();

        // verify hosted WF control follows suit
        WPFMiscUtils.IncCounters(sr, axSystemMonitor1.ForeColor.ToArgb(), origColor.ToArgb(),
            "ForeColor values don't match (restoring original)", p.log);
    }

    #endregion
    #region Test functions for IsEnabled property

    private void VerifyMappingIsEnabled(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        bool origIsEnabled = wfh.IsEnabled;
        bool origChildEnabled = axSystemMonitor1.Enabled;

        // loop through boolean vars (also checks transition)
        foreach (bool testVal in new bool[] { true, false, true })
        {
            p.log.WriteLine("Checking '{0}'", testVal);

            // set property in WFH
            wfh.IsEnabled = testVal;
            avBtn.IsEnabled = testVal;
            MyPause();

            // check setting in WF control
            if (bIsMapped)
            {
                // mapping, so should have changed
                WPFMiscUtils.IncCounters(sr, testVal, axSystemMonitor1.Enabled,
                    "IsEnabled values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origChildEnabled, axSystemMonitor1.Enabled,
                    "IsEnabled values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.IsEnabled = origIsEnabled;
        avBtn.IsEnabled = origIsEnabled;
        MyPause();

        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origIsEnabled, axSystemMonitor1.Enabled,
            "IsEnabled values don't match (restoring original)", p.log);
    }
    
    #endregion

    #region Test functions for Visibility property

    private void VerifyMappingVisibility(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        Visibility origVisibility = wfh.Visibility;
        bool origChildVisible = axSystemMonitor1.Visible;

        // loop through Visibility values
        foreach (Visibility testVal in new Visibility[] { Visibility.Collapsed, Visibility.Hidden, Visibility.Visible, Visibility.Hidden })
        {
            p.log.WriteLine("Checking '{0}'", testVal);

            // set property in WFH
            wfh.Visibility = testVal;
            avBtn.Visibility = testVal;
            MyPause();

            // check setting in WF control
            if (bIsMapped)
            {
                // mapping, so should have changed

                // decide what mapped value should be
                bool expVal;
                if (testVal == Visibility.Collapsed || testVal == Visibility.Hidden) { expVal = false; }
                else { expVal = true; }

                WPFMiscUtils.IncCounters(sr, expVal, axSystemMonitor1.Visible,
                    "Visible values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origChildVisible, axSystemMonitor1.Visible,
                    "Visible values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.Visibility = origVisibility;
        avBtn.Visibility = origVisibility;
        MyPause();

        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origChildVisible, axSystemMonitor1.Visible,
            "Visible values don't match (restoring original)", p.log);
    }
    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify Background Property Map
//@ Verify Cursor Property Map
//@ Verify FontFamily Property Map
//@ Verify FontSize Property Map
//@ Verify FontStyle Property Map
//@ Verify FontWeight Property Map
//@ Verify ForceCursor Property Map
//@ Verify Foreground Property Map
//@ Verify IsEnabled Property Map
//@ Verify Visibility Property Map