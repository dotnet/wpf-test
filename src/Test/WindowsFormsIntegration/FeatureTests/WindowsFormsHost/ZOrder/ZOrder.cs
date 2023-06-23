using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;

using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using MS.Internal.Mita.Foundation;

//
// Testcase:    ZOrder
// Description: Verify that the z-order of a WFH and WF control is always on top.
// Author:      a-wboyde
//
namespace WindowsFormsHostTests
{

public class ZOrder : WPFReflectBase
{
    #region Testcase setup
    public ZOrder(string[] args) : base(args) { }


    protected override void InitTest(TParams p)
    {
        // get window to show up
        this.Topmost = true;
        this.Topmost = false;

        // make window a better size
        this.Width = 500;
        this.Height = 500;
        
        base.InitTest(p);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("1) Draw an Avalon Rect over the WF Button and verify that the button is still displayed")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // create Canvas, add to window
        Canvas can = new Canvas();
        this.Content = can;

        // draw Host with WF Button
        WindowsFormsHost wfh = new WindowsFormsHost();
        wfh.Background = Brushes.Yellow;
        SWF.Button wfBtn = new SWF.Button();
        wfBtn.Text = "Windows Forms Button";
        wfh.Child = wfBtn;

        // add WFH to canvas
        can.Children.Add(wfh);
        wfh.SetValue(Canvas.LeftProperty, 30d);
        wfh.SetValue(Canvas.TopProperty, 30d);

        // get bitmap of button (before)
        WPFReflectBase.DoEvents();
        this.Topmost = true;
        using (PointerInput.Activate(Mouse.Instance))
        {
            Mouse.Instance.Move(new System.Drawing.Point(0, 0));
        }
        SD.Bitmap bmp1 = Utilities.GetBitmapOfControl(wfBtn, true);
        this.Topmost = false;

        // draw rectangle over button
        Rectangle rect = new Rectangle();
        rect.Width = 200;
        rect.Height = 100;
        rect.Fill = Brushes.HotPink;
        can.Children.Add(rect);

        // "verify that the button is still displayed"
        WPFReflectBase.DoEvents();
        this.Topmost = true;
        using (PointerInput.Activate(Mouse.Instance))
        {
            Mouse.Instance.Move(new System.Drawing.Point(0, 0));
        }
        SD.Bitmap bmp2 = Utilities.GetBitmapOfControl(wfBtn, true);
        this.Topmost = false;
        
        // if WFH is displayed "on top of" rectangle, even though the rectangle was
        // added to the Canvas after it, the two bitmaps should match exactly.
        WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(bmp1, bmp2),
            "WFH is not visible over Avalon rectangle");

        return sr;
    }

    [Scenario("2) Add the WFH to an AV Tab page and verify that the WFH is only displayed when its tab is chosen")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // create StackPanel, add to window
        StackPanel sp = new StackPanel();
        this.Content = sp;

        // create tab control, add to StackPanel
        TabControl tabControl = new TabControl();
        sp.Children.Add(tabControl);

        // create tab with simple text
        TabItem tabItem = new TabItem();
        tabItem.Header = "Tab 1";
        tabItem.Content = "Just text on this page";
        tabControl.Items.Add(tabItem);

        // create tab with Avalon button
        Button avBtn = new Button();
        avBtn.Content = "Avalon Button";
        tabItem = new TabItem();
        tabItem.Header = "Tab 2";
        tabItem.Content = avBtn;
        tabControl.Items.Add(tabItem);

        // create tab with WFH with WF Button - make it yellow
        WindowsFormsHost wfh = new WindowsFormsHost();
        wfh.Background = Brushes.Yellow;
        SWF.Button wfBtn = new SWF.Button();
        wfBtn.Text = "Winforms Button";
        wfh.Child = wfBtn;
        tabItem = new TabItem();
        tabItem.Header = "WindowsFormsHost";
        tabItem.Content = wfh;
        tabControl.Items.Add(tabItem);

        // create tab with Avalon Label (to show that WFH goes away when switch tab 3->4)
        Label avLab = new Label();
        avLab.Content = "Avalon Label";
        tabItem = new TabItem();
        tabItem.Header = "Tab 4";
        tabItem.Content = avLab;
        tabControl.Items.Add(tabItem);

        // add another Avalon element to StackPanel
        Button avBtn2 = new Button();
        avBtn2.Content = "Another Avalon Button";
        avBtn2.Background = Brushes.Green;
        sp.Children.Add(avBtn2);

        // turn to tab with WFH, get rectangle of WFH
        // Note: may not need to turn to the page just to get the rectangle
        tabControl.SelectedIndex = 2;
        WPFReflectBase.DoEvents();
        SD.Rectangle wfRect = wfBtn.RectangleToScreen(wfBtn.ClientRectangle);
        SD.Bitmap bmpOrig = Utilities.GetBitmapOfControl(wfBtn, true);

        // flip through the tabs
        for (int i = 0; i < tabControl.Items.Count; i++ )
        {
            // turn to tab page
            tabControl.SelectedIndex = i;
            Utilities.SleepDoEvents(20);

            // grab bitmap of area where WF Button may be
            this.Topmost = true;
            SD.Bitmap bmp1 = Utilities.GetBitmapOfControl(wfBtn, true);
            this.Topmost = false;

            // logic: WF Button in WFH is "visible" if we can find Yellow in bitmap
            // WF should only be visible on third tab (#2)
            if (i == 2)
            {
                // we are on tab page with the WFH, we should see it
                WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(bmpOrig, bmp1),
                    "Cannot find evidence of WFH on tab where it should be visible");
            }
            else
            {
                // we are on tab page other than one with WFH, we should not see it
                WPFMiscUtils.IncCounters(sr, p.log,
                    !Utilities.BitmapsIdentical(bmpOrig, bmp1),
                    "Found evidence of WFH on tab page where it should not be visible");
            }
        }
        
        return sr;
    }

    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Draw an Avalon Rect over the WF Button and verify that the button is still displayed

//@ 2) Add the WFH to an AV Tab page and verify that the WFH is only displayed when its tab is chosen