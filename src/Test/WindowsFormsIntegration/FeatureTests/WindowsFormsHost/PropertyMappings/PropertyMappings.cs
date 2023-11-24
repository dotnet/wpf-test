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
using System.Runtime.InteropServices;       // needed for DllImport - remove this if remove that - !!!
using SD = System.Drawing;

//
// Testcase:    PropertyMappings
// Description: Test existing spec defined Mappings
// Author:      a-wboyde
//
namespace WindowsFormsHostTests
{

public class PropertyMappings : WPFReflectBase
{
    #region Testcase setup
    public PropertyMappings(string[] args) : base(args) { }

    #region Class Vars and other definitions

    // Each property is unique unto itself and therefore has a separate "verification" function.
    // Each properties verification function is declared as a delegate so it can be more easily called.
    // define the delegate for the standard verification function
    delegate void VerifyMappingDelegate(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped);

    // class vars
    private static DockPanel dp;                        // our Dockpanel
    private static Button avBtn;                        // our Avalon button
    private static WindowsFormsHost wfh;                // our Windows Forms Host control
    private static System.Windows.Forms.Button wfBtn;   // our Windows Forms button

    // (these all have to be declared static because they are used in array initializers)
    // define some color brushes to play with
    private static SolidColorBrush brBlue = Brushes.Blue;
    private static SolidColorBrush brRed = Brushes.Red;
    private static SolidColorBrush brGreen = Brushes.Green;
    private static SolidColorBrush brYellow = Brushes.Yellow;

    // define some gradient brushes
    private static LinearGradientBrush brLinYellowBlue = new LinearGradientBrush(Colors.Yellow, Colors.Blue, 90);
    private static LinearGradientBrush brLinRedGreen = new LinearGradientBrush(Colors.Red, Colors.Green, 0);
    private static RadialGradientBrush brRadYellowBlue = new RadialGradientBrush(Colors.Yellow, Colors.Blue);
    private static RadialGradientBrush brRadRedGreen = new RadialGradientBrush(Colors.Red, Colors.Green);

    // create enum for "brush type" to aid in verification
    private enum myBrushType { Solid, LinearYB, LinearRG, RadialYB, RadialRG, Bitmap };

    // define a tile brush (with a bitmap)
    // !!! bitmap image file must exist with testcase (School dir) or there will be trouble
    // this will throw a "System.TypeInitializationException was unhandled" if bitmap not found at runtime !!!
    private static string BitmapPath = "beany.bmp";
    private static BitmapImage imgBeany = new BitmapImage(new Uri(BitmapPath, UriKind.Relative));
    private static ImageBrush brImgBeany = new ImageBrush(imgBeany);

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

        // debug - run specific scenario !!!
        //if (scenario.Name != "ScenarioBackground") { return false; }

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

        // create hosted WF control, add to WFH
        wfBtn = new System.Windows.Forms.Button();
        wfBtn.Text = "Winforms Button";
        wfBtn.Size = new System.Drawing.Size(200, 100);
        wfh.Child = wfBtn;

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

    [Scenario("Verify FlowDirection Property Map")]
    public ScenarioResult ScenarioFlowDirection(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        TestMappedProperty(sr, p, "FlowDirection", VerifyMappingFlowDirection);
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
        TestMappedProperty(sr, p, "FontSize", VerifyMappingFontSize);
        return sr;
    }

    [Scenario("Verify FontStyle Property Map")]
    public ScenarioResult ScenarioFontStyle(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        TestMappedProperty(sr, p, "FontStyle", VerifyMappingFontStyle);
        return sr;
    }

    [Scenario("Verify FontWeight Property Map")]
    public ScenarioResult ScenarioFontWeight(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
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

    // New Scenerio to regress bug !!! still in development
    // commented out so that driver will not think it is valid Scenario - yet
    // either need to finish this or pull it out a-wboyde !!!
    //[Scenario("Test issues from Windows OS Bug 1538006")]
    //public ScenarioResult ScenarioBug1538006(TParams p)
    //{
    //    ScenarioResult sr = new ScenarioResult();
    //    RegressBug1538006(p);
    //    return sr;
    //}

    // this is still in development !!!
    // this function is only called from ScenarioBug1538006()
    private void RegressBug1538006(TParams p)
    {
        InsureFontInstalled(p);
        Utilities.ActiveFreeze(currentScenario.Name);

        //string FontFileName = "D:\\School\\fredchick.ttf";
        //string FontFileName = "D:/School/chick.ttf";
        //string FontFileName = "D:\\Windows\\Fonts\\wingding.ttf";
        //string FontFileName = "D:/Windows/Fonts/wingding.ttf";
        //string FontFileName = "#WingDing";

        // this works
        // load font that is a system font
        //string FontFileName = "WingDings";
        string FontFileName = "Chick";

        string str = "This is a test";

        // load Chick font !!!
        FontFamily testFont = new FontFamily(FontFileName);

        // set Font to Avalon button
        avBtn.FontFamily = testFont;
        avBtn.Content = str;

        try
        {
            // set Font to WFH
            wfh.FontFamily = testFont;
        }
        catch (ArgumentException e)
        {
            p.log.WriteLine("Got exception '{0}'", e);
        }

        wfBtn.Text = str;
        //MyPause();

        Utilities.ActiveFreeze("after");
    }

    #region Magic Font functions

    [DllImport("gdi32.dll")]
    static extern int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);
    private const int FR_PRIVATE = 0x10;
    private const int FR_NOT_ENUM = 0x20;

    [DllImport("gdi32.dll")]
    static extern int AddFontResource(string lpszFilename);

    private void InsureFontInstalled(TParams p)
    {
        string fontName = "Chick";
        string fontFile = "chick.ttf";

        // check for our font
        p.log.WriteLine("Before - have {0} fonts", Fonts.SystemFontFamilies.Count);
        if (IsFontInstalled(fontName))
        {
            p.log.WriteLine("-- found our font");
        }

        //Maui.Core.Utilities.Process.RunAsDelegate del = new Maui.Core.Utilities.Process.RunAsDelegate(InstallFont);
        //Maui.Core.Utilities.Process.RunAsAdmin(del);

        InstallFont(p, fontFile);
        MyPause();
        Utilities.ActiveFreeze("after call to install font");

        // check for our font
        p.log.WriteLine("After - have {0} fonts", Fonts.SystemFontFamilies.Count);
        if (IsFontInstalled(fontName))
        {
            p.log.WriteLine("-- found our font");
        }
    }

    private static void InstallFont(TParams p, string fontFile)
    {
        //int rc = AddFontResourceEx(fontFile, FR_PRIVATE, IntPtr.Zero);
        //p.log.WriteLine("after AddFontResourceEx rc={0}", rc);

        int rc = AddFontResource(fontFile);
        p.log.WriteLine("after AddFontResource rc={0}", rc);
    }

    private bool IsFontInstalled(string name)
    {
        foreach (FontFamily fn in Fonts.SystemFontFamilies)
        {
            if (fn.Source == name) { return true; }

            //p.log.WriteLine("Source = '{0}'", fn.Source);
            //if (fn.Source == "Chick")
            //{
            //    p.log.WriteLine("-- Source = '{0}'", fn.Source);
            //}
        }
        return false;
    }
    #endregion

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

        // bug: this throws an exception !!!
        //PropertyTranslator pt2 = wfh.PropertyMap[prop];
        //PropertyTranslator ptfred = wfh.PropertyMap["Fred"];

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
        System.Threading.Thread.Sleep(10);
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
        new MyBackgroundType("Yellow/Blue linear", brLinYellowBlue, System.Drawing.Color.Empty, myBrushType.LinearYB),
        new MyBackgroundType("Red/Green linear", brLinRedGreen, System.Drawing.Color.Empty, myBrushType.LinearRG),
        new MyBackgroundType("Yellow/Blue radial", brRadYellowBlue, System.Drawing.Color.Empty, myBrushType.RadialYB),
        new MyBackgroundType("Red/Green radial", brRadRedGreen, System.Drawing.Color.Empty, myBrushType.RadialRG),
        new MyBackgroundType("Bitmap image", brImgBeany, System.Drawing.Color.Empty, myBrushType.Bitmap),
    };

    private static void VerifyMappingBackground(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current values
        System.Windows.Media.Brush origBackHost = wfh.Background;
        System.Windows.Media.Brush origBackBtn = avBtn.Background;
        System.Drawing.Color origColor = wfBtn.BackColor;
        System.Drawing.Image origImage = wfBtn.BackgroundImage;

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
            System.Drawing.Color wfColor = wfBtn.BackColor;
            System.Drawing.Image wfImage = wfBtn.BackgroundImage;
            if (bIsMapped)
            {
                // mapping, so should have changed

                // Note: have to be carefull when comparing colors - must compare by RGB not by name
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
                        //p.log.LogKnownBug(BugDb.VSWhidbey, 575560, msg);
                        p.log.LogKnownBug(BugDb.WindowsOSBugs, 1538004, msg);
                    }
                }

                // BackgroundImage - only check if need to (not a Solid color)
                if (bt.brushType!= myBrushType.Solid)
                {
                    // call function to do "best guess" at what background looks like
                    bool b = VerifyBackground(p, bt.brushType);
                    WPFMiscUtils.IncCounters(sr, p.log, b, "BackgroundImage does not look correct");
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
	origImage = wfBtn.BackgroundImage;
        MyPause();

        // verify WF control follows suit
        // BackColor
        // Note: VSWhidbey Bug 575559 / WindowsOS Bug 1538015 used to happen here
        WPFMiscUtils.IncCounters(sr, origColor.ToArgb(), wfBtn.BackColor.ToArgb(),
            "BackColor values don't match (restoring original)", p.log);

        // BackgroundImage
        WPFMiscUtils.IncCounters(sr, origImage, wfBtn.BackgroundImage,
            "BackgroundImage values don't match (restoring original)", p.log);
    }

    /// <summary>
    /// Helper to verify BackgroundImage looks like a Gradient.  Given a clue about what to expect (brushtype),
    /// make a "best guess" if the WF Control Background looks like what it is supposed to.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="brushType"></param>
    /// <returns></returns>
    private static bool VerifyBackground(TParams p, myBrushType brushType)
    {
        // get rectangle of our WF control, being careful to ignore 2-pixel border
        System.Drawing.Rectangle wfRect = new System.Drawing.Rectangle(2, 2,
            wfBtn.ClientRectangle.Width - 4, wfBtn.ClientRectangle.Height - 4);

        // convert to screen coordinated, get bitmap of WF control
        System.Drawing.Rectangle wfRectSc = wfBtn.RectangleToScreen(wfRect);
        System.Drawing.Bitmap bmpCtrl = Utilities.GetScreenBitmap(wfRectSc);

        // vars to use for when looking at center of image (10 x 10 pixel chunk at center)
        int midX = (wfRect.Width / 2) - 5;
        int midY = (wfRect.Height / 2) - 5;

        // common vars
        double pcMid1;
        double pcMid2;
        double pcCorUL;
        double pcCorUR;
        double pcCorLL;
        double pcCorLR;

        switch (brushType)
        {
            case myBrushType.LinearRG:
                // color changes from red (left) to green (right)
                double pcR = BitmapsColorPercent(bmpCtrl, 0, 0, 10, wfRect.Height, SD.Color.Red);
                double pcG = BitmapsColorPercent(bmpCtrl, wfRect.Width - 10, 0, 10, wfRect.Height, SD.Color.Green);
                p.log.WriteLine("  pcR = {0} pcG = {1}", pcR, pcG);

                // pass if both sections match
                if (PercentageInRange(pcR, 100, 100) && PercentageInRange(pcG, 90, 100))
                    return true;
                else
                    return false;

            case myBrushType.LinearYB:
                // color changes from yellow (top) to blue (bottom)
                double pcY = BitmapsColorPercent(bmpCtrl, 0, 0, wfRect.Width, 10, SD.Color.Yellow);
                double pcB = BitmapsColorPercent(bmpCtrl, 0, wfRect.Height - 10, wfRect.Width, 10, SD.Color.Blue);
                p.log.WriteLine("  pcY = {0} pcB = {1}", pcY, pcB);

                // pass if both sections match
                if (PercentageInRange(pcY, 100, 100) && PercentageInRange(pcB, 100, 100))
                    return true;
                else
                    return false;

            case myBrushType.RadialRG:
                // color changes from red (middle) to green (corners)

                // middle (10 pixels around center) color + black text
                pcMid1 = BitmapsColorPercent(bmpCtrl, midX, midY, 10, 10, SD.Color.Red);
                pcMid2 = BitmapsColorPercent(bmpCtrl, midX, midY, 10, 10, SD.Color.Black);

                // upper left
                pcCorUL = BitmapsColorPercent(bmpCtrl, 0, 0, 10, 10, SD.Color.Green);

                // upper right
                pcCorUR = BitmapsColorPercent(bmpCtrl, wfRect.Width - 10, 0, 10, 10, SD.Color.Green);

                // lower left
                pcCorLL = BitmapsColorPercent(bmpCtrl, 0, wfRect.Height - 10, 10, 10, SD.Color.Green);

                // lower right
                pcCorLR = BitmapsColorPercent(bmpCtrl, wfRect.Width - 10, wfRect.Height - 10, 10, 10, SD.Color.Green);

                p.log.WriteLine("  pcMid1 = {0}/{1} pcG = {2}/{3}/{4}/{5}", pcMid1, pcMid2, pcCorUL, pcCorUR, pcCorLL, pcCorLR);
                
                // pass if center and all corners match
                //lower the percent since black fore-color doesn't contain only black due to smoothing. 
                if (PercentageInRange(pcMid1 + pcMid2, 50, 100) &&
                    PercentageInRange(pcCorUL, 100, 100) &&
                    PercentageInRange(pcCorUR, 100, 100) &&
                    PercentageInRange(pcCorLL, 100, 100) &&
                    PercentageInRange(pcCorLR, 100, 100))
                    return true;
                else
                    return false;

            case myBrushType.RadialYB:
                // color changes from yellow (middle) to blue (corners)

                // middle (10 pixels around center) color + black text
                pcMid1 = BitmapsColorPercent(bmpCtrl, midX, midY, 10, 10, SD.Color.Yellow);
                pcMid2 = BitmapsColorPercent(bmpCtrl, midX, midY, 10, 10, SD.Color.Black);

                // upper left
                pcCorUL = BitmapsColorPercent(bmpCtrl, 0, 0, 10, 10, SD.Color.Blue);

                // upper right
                pcCorUR = BitmapsColorPercent(bmpCtrl, wfRect.Width - 10, 0, 10, 10, SD.Color.Blue);

                // lower left
                pcCorLL = BitmapsColorPercent(bmpCtrl, 0, wfRect.Height - 10, 10, 10, SD.Color.Blue);

                // lower right
                pcCorLR = BitmapsColorPercent(bmpCtrl, wfRect.Width - 10, wfRect.Height - 10, 10, 10, SD.Color.Blue);

                p.log.WriteLine("  pcMid1 = {0}/{1} pcG = {2}/{3}/{4}/{5}", pcMid1, pcMid2, pcCorUL, pcCorUR, pcCorLL, pcCorLR);

                // pass if center and all corners match
                //lower the percent since black fore-color doesn't contain only black due to smoothing. 
                if (PercentageInRange(pcMid1 + pcMid2, 50, 100) &&
                    PercentageInRange(pcCorUL, 50, 100) &&
                    PercentageInRange(pcCorUR, 100, 100) &&
                    PercentageInRange(pcCorLL, 100, 100) &&
                    PercentageInRange(pcCorLR, 100, 100))
                    return true;
                else
                    return false;


            case myBrushType.Bitmap:
                // verify percentages of known colors in bitmap we are using
                double pcBlue = BitmapsColorPercent(bmpCtrl, 0, 0, wfRect.Width, wfRect.Height, SD.Color.FromArgb(0, 255, 255));
                double pcGreen = BitmapsColorPercent(bmpCtrl, 0, 0, wfRect.Width, wfRect.Height, SD.Color.FromArgb(0, 255, 0));
                double pcYellow = BitmapsColorPercent(bmpCtrl, 0, 0, wfRect.Width, wfRect.Height, SD.Color.FromArgb(255, 255, 0));
                double pcPink = BitmapsColorPercent(bmpCtrl, 0, 0, wfRect.Width, wfRect.Height, SD.Color.FromArgb(255, 0, 255));
                double pcGray = BitmapsColorPercent(bmpCtrl, 0, 0, wfRect.Width, wfRect.Height, SD.Color.FromArgb(128, 128, 128));
                double pcWhite = BitmapsColorPercent(bmpCtrl, 0, 0, wfRect.Width, wfRect.Height, SD.Color.FromArgb(255, 255, 255));

                p.log.WriteLine("  Blue={0} Green={1} Yellow={2} Pink={3} Gray={4} White={5}",
                    pcBlue, pcGreen, pcYellow, pcPink, pcGray, pcWhite);

                // pass if all components match
                if (PercentageInRange(pcBlue, 1, 2) &&          // 1.15740740740741
                    PercentageInRange(pcGreen, 1.5, 2.5) &&     // 1.98659132445638
                    PercentageInRange(pcYellow, 1.5, 2.5) &&    // 1.98239528551714
                    PercentageInRange(pcPink, 1, 2) &&          // 1.41103464995711
                    PercentageInRange(pcGray, 6, 11) &&          // 6.68941852224833
                    PercentageInRange(pcWhite, 70, 73))         // 72.0063593301257
                    return true;
                else
                    return false;

            default:
                // should never get here
                return false;
        }
    }

    private static bool PercentageInRange(double pc, double low, double high)
    {
        if (pc < low) { return false; }
        if (pc > high) { return false; }
        return true;
    }

    // BitmapsColorPercent
    // Starting at (x, y), look through the area of pixels in bmp specified by width and height 
    // for color match of specified color.
    // For every pixel that matches specified color, increment match counter.
    // Return percentage of matching pixels to total pixels.
    private static double BitmapsColorPercent(SD.Bitmap bmp, int x, int y, int width, int height, SD.Color color)
    {
        double total = 0;
        double match = 0;
        for (int col = x; col < x + width; col++)
        {
            for (int row = y; row < y + height; row++)
            {
                if (Utilities.ColorsMatch(bmp.GetPixel(col, row), color))
                {
                    match++;
                }
                total++;
            }
        }
        return match * 100 / total;
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

    private static void VerifyMappingCursor(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        System.Windows.Input.Cursor origCur = wfh.Cursor;
        System.Windows.Forms.Cursor origCur2 = wfBtn.Cursor;

        foreach (MyCursorType curType in MyCursorList)
        {
            p.log.WriteLine("Checking '{0}'", curType.avalon.ToString());

            // set (Avalon) property in WFH
            wfh.Cursor = curType.avalon;
            avBtn.Cursor = curType.avalon;
            MyPause();

            // check setting in WF control (WinForms)
            System.Windows.Forms.Cursor actCur2 = wfBtn.Cursor;
            if (bIsMapped)
            {
                // mapping, so should have changed
                WPFMiscUtils.IncCounters(sr, curType.winforms, actCur2,
                    "Cursor values don't match (mapping)", p.log);

                // debug !!!
                if (actCur2 != curType.winforms)
                {
                    //MessageBox.Show(string.Format("values don't match '{0}'", curType.avalon.ToString()));
                }
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origCur2, actCur2,
                    "Cursor values don't match (not mapping)", p.log);

                // debug !!!
                if (actCur2 != origCur2)
                {
                    //MessageBox.Show(string.Format("values don't match '{0}'", curType.avalon.ToString()));
                }
            }
        }

        // restore original value
        wfh.Cursor = origCur;
        avBtn.Cursor = origCur;
        MyPause();

        // verify WF control follows suit
        // Note: VSWhidbey Bug 574027 used to happen here
        WPFMiscUtils.IncCounters(sr, origCur2, wfBtn.Cursor,
            "Cursor values don't match (restoring original)", p.log);
    }
    #endregion
    #region Test functions for FlowDirection property

    private static void VerifyMappingFlowDirection(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        FlowDirection orig = wfh.FlowDirection;
        System.Windows.Forms.RightToLeft origChild = wfBtn.RightToLeft;

        // loop through boolean vars (also checks transition)
        foreach (FlowDirection testVal in Enum.GetValues(typeof(FlowDirection)))
        {
            p.log.WriteLine("Checking '{0}'", testVal.ToString());

            // set property in WFH
            wfh.FlowDirection = testVal;
            avBtn.FlowDirection = testVal;
            MyPause();

            // check setting in WF control
            System.Windows.Forms.RightToLeft valChild = wfBtn.RightToLeft;
            if (bIsMapped)
            {
                // mapping, so should have changed

                // decide what mapped value should be
                System.Windows.Forms.RightToLeft expVal;
                if (testVal == FlowDirection.RightToLeft) { expVal = System.Windows.Forms.RightToLeft.Yes; }
                else { expVal = System.Windows.Forms.RightToLeft.No; }

                WPFMiscUtils.IncCounters(sr, expVal, valChild,
                    "RightToLeft values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origChild, valChild,
                    "RightToLeft values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.FlowDirection = orig;
        avBtn.FlowDirection = orig;
        MyPause();

        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origChild, wfBtn.RightToLeft,
            "RightToLeft values don't match (restoring original)", p.log);
    }
    #endregion
    #region Test functions for FontFamily property


    private static void VerifyMappingFontFamily(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // certain Avalon Fonts are not available in Windows Forms, some other fonts cannot be Regular.
        // lets make a list, and skip them
        //System.Collections.ArrayList alSpecialFonts = new System.Collections.ArrayList();
        //alSpecialFonts.Add("Franklin Gothic");
        //alSpecialFonts.Add("Global Monospace");
        //alSpecialFonts.Add("Global Sans Serif");
        //alSpecialFonts.Add("Global Serif");
        //alSpecialFonts.Add("Global User Interface");
        //alSpecialFonts.Add("Kartika");
        //alSpecialFonts.Add("Lucida Sans");
        //alSpecialFonts.Add("Vrinda");
        //alSpecialFonts.Add("Arial Rounded MT");
        //alSpecialFonts.Add("Copperplate Gothic");
        //alSpecialFonts.Add("Eras ITC");
        //alSpecialFonts.Add("Gill Sans");
        //alSpecialFonts.Add("Gloucester MT");
        //alSpecialFonts.Add("Kalinga");
        //alSpecialFonts.Add("OCR A");
        //alSpecialFonts.Add("Aharoni");
        //alSpecialFonts.Add("Monotype Corsiva");
        //alSpecialFonts.Add("Palace Script MT");
        //alSpecialFonts.Add("Rage");
        //alSpecialFonts.Add("Script MT");
        //alSpecialFonts.Add("Guttman Yad");
	//alSpecialFonts.Add("MS Farsi");

        System.Collections.ArrayList checkedFonts = new System.Collections.ArrayList();
	checkedFonts.Add("Arial");
	checkedFonts.Add("Bookman Old Style");
	checkedFonts.Add("Courier New");
	checkedFonts.Add("Garamond");
	checkedFonts.Add("Microsoft Sans Serif");
	checkedFonts.Add("Rockwell");
	checkedFonts.Add("Tahoma");
	checkedFonts.Add("Times New Roman");
	checkedFonts.Add("Webdings");
	checkedFonts.Add("Wingdings");

        // save current value
        FontFamily origHostFamily = wfh.FontFamily;
        System.Drawing.Font origChildFamily = wfBtn.Font;

        // loop through each available font !!!
        foreach (FontFamily curFont in Fonts.SystemFontFamilies)
        {
            p.log.WriteLine("Checking '{0}'", curFont.ToString());

	    //Too much maintenance keeping track of which fonts are not mapped, so check only generic system fonts
	    if (!checkedFonts.Contains(curFont.Source))
            {
                continue;
            }  

            //if (alSpecialFonts.Contains(curFont.Source))
            //    continue;

            wfh.FontFamily = curFont;
            avBtn.FontFamily = curFont;
            WPFReflectBase.DoEvents();

            // check setting in WF control
            System.Drawing.Font valChild = wfBtn.Font;
            if (bIsMapped)
            {
                // mapping, so should have changed

                // how best to compare fonts? !!!
                // for now, compare (text) name of family

                string expFontName = curFont.Source;

                // Note: VSWhidbey Bug 575776 used to happen here
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
        WPFReflectBase.DoEvents();

        // verify WF control follows suit
        // Note: VSWhidbey Bug 575775 / WindowsOSBugs Bug 1538024 used to happen here
        WPFMiscUtils.IncCounters(sr, origChildFamily, wfBtn.Font,
            "FontFamily values don't match (restoring original)", p.log);
    }

    #endregion
    #region Test functions for FontSize property

    private static void VerifyMappingFontSize(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        double origHostVal = wfh.FontSize;
        float origChildVal = wfBtn.Font.Size;

        // loop through a variety of values !!! should also check larger values
        for (double d = 2; d<60; d+=2)
        {
            p.log.WriteLine("Checking '{0}'", d);

            // set property in WFH
            wfh.FontSize = d;
            avBtn.FontSize = d;
            MyPause();

            // check setting in WF control
            float vChild = wfBtn.Font.Size;
            if (bIsMapped)
            {
                // unit should be point
                System.Drawing.GraphicsUnit unit = wfBtn.Font.Unit;
                WPFMiscUtils.IncCounters(sr, wfBtn.Font.Unit, System.Drawing.GraphicsUnit.Point,
                    "Unit property of Font of hosted control should be GraphicsUnit.Point", p.log);

                // Avalon uses 1/96 inch and WinForms uses 1/72 inch (points)
                // calculate expected value by converting from Avalon to WinForms !!! is this ok?
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
        WPFMiscUtils.IncCounters(sr, origChildVal, wfBtn.Font.Size,
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
        
    private static void VerifyMappingFontStyle(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        FontStyle origHostStyle = wfh.FontStyle;
        System.Drawing.Font origChildStyle = wfBtn.Font;

        // loop through each available font style
        foreach (MyFontStyleType curStyle in MyFontStyleList)
        {
            p.log.WriteLine("Checking '{0}'", curStyle.avalon.ToString());

            // set property in WFH
            wfh.FontStyle = curStyle.avalon;
            avBtn.FontStyle = curStyle.avalon;
            MyPause();

            // check setting in WF control
            System.Drawing.Font valChild = wfBtn.Font;
            if (bIsMapped)
            {
                // mapping, so should have changed

                // how best to compare fonts? !!!
                if (curStyle.winforms.ToString() != valChild.Style.ToString())
                {
                    //MessageBox.Show("Font Styles don't match");
                }
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
        // Note: VSWhidbey Bug 575775 / WindowsOSBugs Bug 1538024 used to happen here
        WPFMiscUtils.IncCounters(sr, origChildStyle.Style, wfBtn.Font.Style,
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

    private static void VerifyMappingFontWeight(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        FontWeight origHostWeight = wfh.FontWeight;
        System.Drawing.Font origChildWeight = wfBtn.Font;

        // loop through each available font !!!
        //foreach (bool testVal in new bool[] { true, false, true })
        foreach (MyFontWeightType curWeight in MyFontWeightList)
        {
            p.log.WriteLine("Checking '{0}'", curWeight.avalon.ToString());

            // set property in WFH
            wfh.FontWeight = curWeight.avalon;
            avBtn.FontWeight = curWeight.avalon;
            MyPause();

            // check setting in WF control
            System.Drawing.Font valChild = wfBtn.Font;
            if (bIsMapped)
            {
                // mapping, so should have changed
                // Note: VSWhidbey Bug 575563 used to happen here
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
        // Note: VSWhidbey Bug 575775 / WindowsOSBugs Bug 1538024 used to happen here
        WPFMiscUtils.IncCounters(sr, origChildWeight.Bold, wfBtn.Font.Bold,
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

    private static void VerifyMappingForceCursor(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current values
        bool origForceCursor = wfh.ForceCursor;
        System.Windows.Input.Cursor origCursor = wfh.Cursor;

        // loop through each available test
        foreach (MyForceCursorType curTest in MyForceCursorList)
        {
            p.log.WriteLine("Checking '{0}'", curTest.testDesc);

            // save current state "before" each test
            bool origUseWaitCursor = wfBtn.UseWaitCursor;
            System.Windows.Forms.Cursor origWFcursor = wfBtn.Cursor;
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

            p.log.WriteLine("  wfBtn.UseWaitCursor is '{0}'", wfBtn.UseWaitCursor.ToString());
            p.log.WriteLine("  wfBtn.Cursor is '{0}'", wfBtn.Cursor.ToString());

            // check setting in WF control
            if (bIsMapped)
            {
                // mapping, so should have changed
                // UseWaitCursor in WFH should match value in test structure
                WPFMiscUtils.IncCounters(sr, curTest.expUseWaitCursor, wfBtn.UseWaitCursor,
                    "UseWaitCursor values not as expected (mapping)", p.log);

                // debug !!!
                if (curTest.expUseWaitCursor != wfBtn.UseWaitCursor)
                {
                    p.log.LogKnownBug(BugDb.VSWhidbey, 576193, "ForceCursor issues");
                    //MessageBox.Show("UseWaitCursor values not as expected");
                }

                // check resultant cursor if necessary
                if (curTest.expCursor != null)
                {
                    // should match cursor in test record
                    WPFMiscUtils.IncCounters(sr, curTest.expCursor, wfBtn.Cursor,
                        "WF cursor not as expected (mapping)", p.log);

                    // debug !!!
                    if (curTest.expCursor != wfBtn.Cursor)
                    {
                        p.log.LogKnownBug(BugDb.VSWhidbey, 576193, "ForceCursor issues");
                        //MessageBox.Show("WF control Cursor not as expected");
                    }
                }
            }
            else
            {
                // not mapping, so should remain as original
                // Note: since Cursor is still being mapped, it will change
                // we are only testing ForceCursor here
                WPFMiscUtils.IncCounters(sr, origUseWaitCursor, wfBtn.UseWaitCursor,
                    "UseWaitCursor values not as expected (not mapping)", p.log);
            }

            // restore original values (each run)
            wfh.ForceCursor = origForceCursor;
            wfh.Cursor = origCursor;
            MyPause();
            LogCursorStats(p);
            //p.log.WriteLine("");
        }

        // verify WF control follows suit !!!
        //WPFMiscUtils.IncCounters(sr, origChildWeight, wfBtn.Font,
        //    "FontWeight values don't match (restoring original)", p.log);
    }

    // debug helper to help figure out whats happening with ForceCursor
    private static void LogCursorStats(TParams p)
    {
        return;
        //p.log.WriteLine("    wfh.ForceCursor is     '{0}'", wfh.ForceCursor.ToString());
        //p.log.WriteLine("    wfh.Cursor is          '{0}'", wfh.Cursor);
        //p.log.WriteLine("    wfBtn.UseWaitCursor is '{0}'", wfBtn.UseWaitCursor.ToString());
        //p.log.WriteLine("    wfBtn.Cursor is        '{0}'", wfBtn.Cursor.ToString());
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

    private static void VerifyMappingForeground(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current values
        System.Windows.Media.Brush origForeHost = wfh.Foreground;
        System.Windows.Media.Brush origForeBtn = avBtn.Foreground;
        System.Drawing.Color origColor = wfBtn.ForeColor;

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
            System.Drawing.Color wfColor = wfBtn.ForeColor;
            if (bIsMapped)
            {
                // mapping, so should have changed
                WPFMiscUtils.IncCounters(sr, wfColor.ToArgb(), bt.winformsColor.ToArgb(),
                    "ForeColor values don't match (mapping)", p.log);
                // debug !!!
                if (wfColor.ToArgb() != bt.winformsColor.ToArgb())
                {
                    //MessageBox.Show(string.Format("ForeColor values don't match A={0} E={1}",
                    //    wfColor.ToString(), bt.winformsColor.ToString()));
                }
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, wfColor.ToArgb(), origColor.ToArgb(),
                    "BackColor values don't match (not mappping)", p.log);
                // debug !!!
                if (wfColor.ToArgb() != origColor.ToArgb())
                {
                    //MessageBox.Show("BackColor values don't match");
                }
            }
        }

        // restore original value
        wfh.Foreground = origForeHost;
        avBtn.Foreground = origForeBtn;
        MyPause();

        // verify hosted WF control follows suit
        WPFMiscUtils.IncCounters(sr, wfBtn.ForeColor.ToArgb(), origColor.ToArgb(),
            "ForeColor values don't match (restoring original)", p.log);
        // debug !!!
        if (wfBtn.ForeColor.ToArgb() != origColor.ToArgb())
        {
            //MessageBox.Show(string.Format("ForeColor values don't match A={0} E={1}",
            //    wfBtn.ForeColor.ToString(), origColor.ToString()));
        }
    }

    #endregion
    #region Test functions for IsEnabled property

    private static void VerifyMappingIsEnabled(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        bool origIsEnabled = wfh.IsEnabled;
        bool origChildEnabled = wfBtn.Enabled;

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
                WPFMiscUtils.IncCounters(sr, testVal, wfBtn.Enabled,
                    "IsEnabled values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origChildEnabled, wfBtn.Enabled,
                    "IsEnabled values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.IsEnabled = origIsEnabled;
        avBtn.IsEnabled = origIsEnabled;
        MyPause();

        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origIsEnabled, wfBtn.Enabled,
            "IsEnabled values don't match (restoring original)", p.log);
    }
    #endregion
    #region Test functions for Visibility property

    private static void VerifyMappingVisibility(ScenarioResult sr, TParams p, WindowsFormsHost wfh, bool bIsMapped)
    {
        // save current value
        Visibility origVisibility = wfh.Visibility;
        bool origChildVisible = wfBtn.Visible;

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

                WPFMiscUtils.IncCounters(sr, expVal, wfBtn.Visible,
                    "Visible values don't match (mapping)", p.log);
            }
            else
            {
                // not mapping, so should remain as original
                WPFMiscUtils.IncCounters(sr, origChildVisible, wfBtn.Visible,
                    "Visible values don't match (not mapping)", p.log);
            }
        }

        // restore original value
        wfh.Visibility = origVisibility;
        avBtn.Visibility = origVisibility;
        MyPause();

        // verify WF control follows suit
        WPFMiscUtils.IncCounters(sr, origChildVisible, wfBtn.Visible,
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
//@ Verify FlowDirection Property Map
//@ Verify FontFamily Property Map
//@ Verify FontSize Property Map
//@ Verify FontStyle Property Map
//@ Verify FontWeight Property Map
//@ Verify ForceCursor Property Map
//@ Verify Foreground Property Map
//@ Verify IsEnabled Property Map
//@ Verify Visibility Property Map