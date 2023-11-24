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
// Testcase:    DCDCSizing
// Description: Verify that Dynamic WF Controls work in Dynmaic AV Containers
// Author:      a-wboyde
//
namespace WindowsFormsHostTests
{

public class DCDCSizing : WPFReflectBase
{
    #region Testcase setup
    public DCDCSizing(string[] args) : base(args) { }

    // class vars
    private StackPanel sp;
    private SWF.FlowLayoutPanel wfFLP;
    private WindowsFormsHost wfh1;
    private int buttonCount = 15;
    private int btnWidth;
    private int btnHeight;
    private SD.Point originPanel;           // upper left point of SP

    protected override void InitTest(TParams p)
    {
        // hacks to get window to show up !!!
        this.Topmost = true;
        this.Topmost = false;
        this.WindowState = WindowState.Maximized;
        this.WindowState = WindowState.Normal;

        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        bool b = base.BeforeScenario(p, scenario);

        // debug - run specific scenario !!!
        //if (scenario.Name != "Scenario1") { return false; }

        this.Title = currentScenario.Name;
        this.Width = 500;
        this.Height = 300;

        return b;
    }

    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("1) Verify that the FLP Wraps its contents to the available space and that the HostContainer is not shown")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        CreateTestControls();

        // Note: HorizontalAlignment now defaults to Stretch
        // verify this first (before changing it!)
        WPFMiscUtils.IncCounters(sr, HorizontalAlignment.Stretch, wfh1.HorizontalAlignment,
            "HorizontalAlignment should default to Stretch", p.log);

        // we need to capture the coordinates of the upper left corner of the SP
        // do this by setting Alignment to "Stretch" and peeking at WF properties
        // Note: this value will be valid unless window is moved during test!
        wfh1.HorizontalAlignment = HorizontalAlignment.Stretch;
        originPanel = wfh1.Child.Parent.PointToScreen(new SD.Point(0, 0));
        p.log.WriteLine("Got SP Origin as ({0},{1})", originPanel.X, originPanel.Y);

        // iterate through horizontal alignment settings
        foreach (HorizontalAlignment ha in Enum.GetValues(typeof(HorizontalAlignment)))
        {
            // set alignment
            p.log.WriteLine("HorizontalAlignment is '{0}'", ha);
            wfh1.HorizontalAlignment = ha;

            // check that window resizes properly
            ExerciseWindow(p, sr);
        }

        return sr;
    }

    private void ExerciseWindow(TParams p, ScenarioResult sr)
    {
        // Note: we cannot tell window width/height, but we can tell StackPanel
        // width/height - and we can control StackPanel size with window size.

        // Logic: we have a bunch of buttons (15).  If we set the window width
        // and have the window automatically resize its height, we can tell if
        // our buttons are being distributed into appropriate numbers of rows/columns.

        // show various number of columns, forcing window to resize
        // examine window size to confirm things flow right

        this.SizeToContent = SizeToContent.Height;
        p.log.WriteLine("Button Width   {0}", btnWidth);
        p.log.WriteLine("Button Height  {0}", btnHeight);

        // vary from 2 to 5 columns
        for (int col = 2; col < 8; col++)
        {
            // calculate width - rough estimate
            // basically want to set window width "within a button column" to force wrap
            int width = col * (btnWidth + 8);   // button width plus gap between columns
            width += (btnWidth / 2);            // half a column to insure between columns
            width += 8;                         // allow for window border width

            // set window width, which should reflow buttons and adjust panel height
            this.Width = width;
            MyPause();

            // calculate expected number of rows of buttons
            // Note: is there a saner way to do this?
            int expRows = buttonCount / col;    // integer math - truncate remainder
            // if is not evenly divisible, we will have an extra row
            if (buttonCount % col > 0) { expRows++; }
            p.log.WriteLine("");
            p.log.WriteLine("Setting window for {0} Columns", col);
            p.log.WriteLine("Expect to have {0} Rows", expRows);

            // calculate panel height from row count
            // (this could be sensitive to layout/platform/other)
            int expPanelHeight = expRows * btnHeight;   // height of single button
            expPanelHeight += expRows * 6;              // gap between rows of buttons
            p.log.WriteLine("Expect Panel Height to be {0}", expPanelHeight);

            // check window height
            p.log.WriteLine("Panel Width   {0}", wfFLP.Size.Width);
            p.log.WriteLine("Panel Height  {0}", wfFLP.Size.Height);

            WPFMiscUtils.IncCounters(sr, expPanelHeight, wfFLP.Size.Height,
                "Panel not expected height", p.log);

            // verify that WFH is horizontally aligned properly
            CheckAlignment(p, sr);

            // Verify "that the HostContainer is not shown"
            CheckForUnderpants(p, sr);
        }
    }

    [Scenario("2) With the WFH HorizontalAlignment set to possible values, resize the AV Window and verify that the FLP resizes to fit all available space")]
    public ScenarioResult Scenario2(TParams p)
    {
        // WFH HorizontalAlignment defaults to stretch, so this may be duplicate
        // this test is performed as part of Scenarion 1
        return new ScenarioResult(true, "Tested as part of Scenario 1", p.log);
    }

    #endregion

    #region Verification Helpers

    /// <summary>
    /// Helper function to examine layout of StackPanel to verify that WFH is horizontally
    /// aligned properly.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sr"></param>
    private void CheckAlignment(TParams p, ScenarioResult sr)
    {
        HorizontalAlignment ha = wfh1.HorizontalAlignment;
        p.log.WriteLine("Verifying that HorizontalAlignment is {0}", ha.ToString());

        // we can get the width & height of the WFH and the SP
        // and the expected alignment of the WFH within the SP
        // plus we saved the coordinates of the upper left of the SP (which should not change)
        // from this, we can construct expected rectangles of the various components

        // get width of wfh, width of panel
        // decide what should be exposed
        // verify proper color

        int originX = originPanel.X;
        int originY = originPanel.Y;
        int widthLeft;          // expected width of "left" filler side
        int widthRight;         // expected width of "right" filler side
        int widthMiddle;        // expected width of middle (ie the WFH)
        int heightAll = (int)wfh1.ActualHeight;

        // decide layout based on current HorizontalAlignment setting
        switch (ha)
        {
            case HorizontalAlignment.Left:
                // should have host left justified in panel, with extra space on right side
                widthLeft = 0;
                widthMiddle = (int)wfh1.ActualWidth;
                widthRight = (int)(sp.ActualWidth - wfh1.ActualWidth);
                break;

            case HorizontalAlignment.Right:
                // should have host right justified in panel, with extra space on left side
                widthLeft = (int)(sp.ActualWidth - wfh1.ActualWidth);
                widthMiddle = (int)wfh1.ActualWidth;
                widthRight = 0;
                break;

            case HorizontalAlignment.Center:
                // host should be centered in panel, with extra space divided left/right
                widthMiddle = (int)wfh1.ActualWidth;
                widthLeft = (int)(sp.ActualWidth - wfh1.ActualWidth) / 2;
                widthRight = (int)(sp.ActualWidth - wfh1.ActualWidth) / 2;

                // Note: when amount to be divided between left and right is odd, it
                // appears that the left side gets the extra pixel width
                if ((int)(sp.ActualWidth - wfh1.ActualWidth) % 2 != 0)
                {
                    widthLeft++;
                }
                break;

            case HorizontalAlignment.Stretch:
                // host should fill up entire area, with no space left or right
                widthLeft = 0;
                widthMiddle = (int)sp.ActualWidth;
                widthRight = 0;
                break;

            default:
                // should never get here unless new enum value added
                throw new ArgumentException("Unknown  HorizontalAlignment '{0}'", ha.ToString());
        }

        // sanity check
        int sum = widthLeft + widthMiddle + widthRight;
        WPFMiscUtils.IncCounters(sr, (int)sp.ActualWidth, sum, "Panel sizes do not add up", p.log);

        // build our rectangles
        SD.Rectangle rectLeft = new SD.Rectangle(originX, originY, widthLeft, heightAll);
        SD.Rectangle rectWfh = new SD.Rectangle(originX + widthLeft, originY, widthMiddle, heightAll);
        SD.Rectangle rectRight = new SD.Rectangle(originX + widthLeft + widthMiddle, originY,
            widthRight, heightAll);

        // verify that each rectangle is either empty or contains the right color
        VerifyEndRectangle(p, sr, rectLeft, SD.Color.Yellow, "_left.bmp");
        VerifyMiddleRectangle(p, sr, rectWfh, SD.Color.LightBlue, SD.Color.Yellow, "_host.bmp");
        VerifyEndRectangle(p, sr, rectRight, SD.Color.Yellow, "_right.bmp");
    }

    /// <summary>
    /// Given rectangle that is either left or right side, verify that it has proper color
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sr"></param>
    /// <param name="rect"></param>
    /// <param name="clr"></param>
    /// <param name="filename"></param>
    private static void VerifyEndRectangle(TParams p, ScenarioResult sr, SD.Rectangle rect, SD.Color clr, string filename)
    {
        SD.Bitmap bmp;
        if ((rect.Width == 0) || (rect.Height == 0))
        {
            // empty rectangle
            bmp = new System.Drawing.Bitmap(1, 1);
        }
        else
        {
            // valid rectangle
            bmp = Utilities.GetScreenBitmap(rect);

            // rectangle should be entirely the color of the SP background
            bool b = BitmapAllOneColor(bmp, clr);
            WPFMiscUtils.IncCounters(sr, p.log, b, "Rectangle not all expected color");
        }

        // debug !!!
        //bmp.Save(filename);
    }

    /// <summary>
    /// Helper to determine if specified bitmap is entirely the specified color
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="clr">Color that all pixels should be</param>
    /// <returns></returns>
    private static bool BitmapAllOneColor(SD.Bitmap bmp, SD.Color clr)
    {
        // look through all pixels
        // if find one that is not specified color, return false
        // if get all the way through, all must match, so return true
        for (int row = 0; row < bmp.Height; row++)
            for (int col = 0; col < bmp.Width; col++)
                if (!Utilities.ColorsMatch(bmp.GetPixel(col, row), clr))
                    return false;
        return true;
    }

    /// <summary>
    /// Given rectangle that represents the WFH, verify it has the right colors
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sr"></param>
    /// <param name="rect"></param>
    /// <param name="clrGood"></param>
    /// <param name="clrBad"></param>
    /// <param name="filename"></param>
    private static void VerifyMiddleRectangle(TParams p, ScenarioResult sr, SD.Rectangle rect, SD.Color clrGood, SD.Color clrBad, string filename)
    {
        SD.Bitmap bmp;
        if ((rect.Width == 0) || (rect.Height == 0))
        {
            // empty rectangle
	    p.log.WriteLine("* empty rectangle, new 1x1 pixel bmp created*");
            bmp = new System.Drawing.Bitmap(1, 1);
        }
        else
        {
            // valid rectangle
            bmp = Utilities.GetScreenBitmap(rect);

            // rectangle should contain blue from buttons, but no yellow from SP
            bool b = BitmapHas1stColorButNot2nd(p, bmp, clrGood, clrBad);
            WPFMiscUtils.IncCounters(sr, p.log, b, "Rectangle not expected colors");
        }

        // debug !!!
        //bmp.Save(filename);
    }

    /// <summary>
    /// Helper to determine if bitmap contains one color and does not contain another
    /// (other colors are allowed)
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="clrGood">Color that should be present</param>
    /// <param name="clrBad">Color that should not be present</param>
    /// <returns></returns>
    private static bool BitmapHas1stColorButNot2nd(TParams p, SD.Bitmap bmp, SD.Color clrGood, SD.Color clrBad)
    {
        bool has1st = Utilities.ContainsColor(bmp, clrGood);
        bool has2nd = Utilities.ContainsColor(bmp, clrBad);
	p.log.WriteLine("Blue from buttons: " + has1st);
	p.log.WriteLine("No yellow from SP: " + !has2nd);
        return (has1st && !has2nd);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Create WF Control in a WFH in a DockPanel.  Also adds surrounding Avalon buttons.
    /// </summary>
    /// <param name="ctrl"></param>
    private void CreateTestControls()
    {
        // create dynamic panel
        sp = new StackPanel();

        // avalon button
        Button avBtn1 = new Button();
        avBtn1.Content = "Avalon Button 1";
        sp.Children.Add(avBtn1);

        // create WF host control
        wfh1 = new WindowsFormsHost();

        // create FlowLayoutPanel
        wfFLP = new System.Windows.Forms.FlowLayoutPanel();
        wfh1.Child = wfFLP;

        // set colors so can see host background
        wfFLP.Parent.Paint += new System.Windows.Forms.PaintEventHandler(Parent_Paint);
        wfFLP.BackColor = SD.Color.LightBlue;

        // create buttons
        for (int i = 0; i < buttonCount; i++)
        {
            SWF.Button btn = new SWF.Button();
            btn.Text = "Button " + i;
            wfFLP.Controls.Add(btn);
            btnWidth = btn.Width;
            btnHeight = btn.Height;
        }

        sp.Children.Add(wfh1);

        // avalon button
        Button avBtn2 = new Button();
        avBtn2.Content = "Avalon Button 2";
        sp.Children.Add(avBtn2);

        // add panel to window
        this.Background = Brushes.Yellow;
        this.Content = sp;
        MyPause();
    }

    void Parent_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
        e.Graphics.Clear(System.Drawing.Color.HotPink);
    }

    /// <summary>
    /// Helper function to check if host container of WFH is visible.  Grabs bitmap of host container
    /// and looks for the HotPink color.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sr"></param>
    private void CheckForUnderpants(TParams p, ScenarioResult sr)
    {
        // check that host container is not visible
        // We are painting the host container background Hot Pink.
        // Grab a bitmap of the host object and look for that color
        p.log.WriteLine("Checking for signs of host container");

        // get bitmap of WFH
        // we want to look at the "host container" which is the parent of the hosted child
        SD.Bitmap bmpHost = Utilities.GetBitmapOfControl(wfh1.Child.Parent, true);

        // debug !!!
        //bmpHost.Save("_wfh.bmp");

        // does the bitmap have any Pink in it?
        bool bBleedover = Utilities.ContainsColor(bmpHost, SD.Color.HotPink);
        WPFMiscUtils.IncCounters(sr, p.log, !bBleedover, "I can see the Host Control's underpants!");
    }

    /// <summary>
    /// Helper function to pause to let events get processed (waits on both message pumps)
    /// </summary>
    private static void MyPause()
    {
        for (int i = 0; i < 2; i++)
        {
            WPFReflectBase.DoEvents();
            SWF.Application.DoEvents();
            System.Threading.Thread.Sleep(100);
        }
    }

    #endregion

    #region Original Test code found in Testcase
    private void Test()
    {
        Win w = new Win();
        w.Show();
    }

    public class Win : Window
    {
        int buttonCount = 15;
        SWF.FlowLayoutPanel flp;
        StackPanel stackPanel;
        Label label;
        WindowsFormsHost host;
        public Win()
        {
            Width = 200;
            stackPanel = new StackPanel();
            label = new Label();
            label.Content = "An Avalon UIElement";
            label.Height = 200;
            label.Background = Brushes.OrangeRed;
            stackPanel.Children.Add(label);
            host = new WindowsFormsHost();
            host.Background = Brushes.Blue;
            this.Background = Brushes.Beige;
            flp = new SWF.FlowLayoutPanel();
            flp.BackColor = SD.Color.Yellow;
            flp.Size = new System.Drawing.Size(170, 300);
            host.Child = flp;
            for (int i = 0; i < buttonCount; i++)
            {
                SWF.Button btn = new SWF.Button();
                btn.Text = "Button " + i;
                flp.Controls.Add(btn);
            }
            stackPanel.Children.Add(host);
            Label label2 = new Label();
            label2.Content = "Another Avalon UIElement";
            label2.Background = Brushes.OrangeRed;
            stackPanel.Children.Add(label2);
            this.AddChild(stackPanel);
        }
    }
    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Verify that the FLP Wraps its contents to the available space and that the HostContainer is not shown

//@ 2) With the WFH HorizontalAlignment set to possible values, resize the AV Window and verify that the FLP resizes to fit all available space
