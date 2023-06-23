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
using System.Drawing;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;

//
// Testcase:    WindowsOSBug1532320
// Description: Cut, Copy and Paste image between WF control and AV control
// Author:      pachan
//
namespace WindowsFormsHostTests
{

public class WindowsOSBug1532320 : WPFReflectBase 
{
    #region TestVariables
    private delegate void myEventHandler(object sender);
    private bool debug = false;         // set this true for TC debugging
    private TParams _tp;

    private SWC.StackPanel AVStackPanel;
    private SWC.RichTextBox AVRichTextBox;

    private WindowsFormsHost wfh;
    private SWF.RichTextBox WFRichTextBox;

    private const string WindowTitleName = "WindowsOSBug1532320Test";

    private const string AVStackPanelName = "AVStackPanel";
    private const string AVRichTextBoxName = "AVRichTextBox";

    private const string WFName = "WFN";
    private const string WFRichTextBoxName = "WFRichTextBox";
    #endregion

    #region Testcase setup
    public WindowsOSBug1532320(string[] args) : base(args) { }

    protected override void InitTest(TParams p) 
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
   //     this.UseMITA = true;
        _tp = p;
        TestSetup();
        base.InitTest(p);
        this.Top = 0;
        this.Left = 0;
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Cut, Copy and Paste image from AV RichTextBox into WF RichTextBox")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();

        //create a image off the AV Button
        Bitmap orgBMP = new Bitmap(@"beany.bmp");
        SWF.Clipboard.SetImage(orgBMP);
        Size bmpSize = new Size(orgBMP.Width, orgBMP.Height);

        p.log.WriteLine("Copy and Paste from AV RichTextBox into WF RichTextBox");
        // paste bitmap into AVRichTextBox
        AVRichTextBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new myEventHandler(PasteBitmap), AVRichTextBox);

        // copy bitmap from AVRichTextBox
        AVRichTextBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new myEventHandler(CopyBitmap), AVRichTextBox);
        try
        {
            Bitmap AV_rtbBMP = new Bitmap(SWF.Clipboard.GetImage(), bmpSize);
            if (debug) AV_rtbBMP.Save(@"AV_rtbImage.bmp");
            // paste bitmap into WFRichTextBox
            while (WFRichTextBox.IsHandleCreated == false)
                System.Windows.Forms.Application.DoEvents();
            WFRichTextBox.Invoke(new myEventHandler(PasteBitmap), WFRichTextBox);
            Utilities.SleepDoEvents(10);

            // copy bitmap fromWFRichTextBox
            WFRichTextBox.Invoke(new myEventHandler(CopyBitmap), WFRichTextBox);
            Bitmap WF_rtbBMP = new Bitmap(SWF.Clipboard.GetImage());
            if (debug) WF_rtbBMP.Save(@"WF_rtbImage.bmp");
            WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(AV_rtbBMP, WF_rtbBMP), "Bitmaps are different");
        }
        catch (ArgumentNullException)
        {
            WPFMiscUtils.IncCounters(sr, p.log, false, "Bitmap fail to copy");
        }

        if (sr.FailCount != 0)
            p.log.LogKnownBug(BugDb.WindowsOSBugs, 1532320, "Cannot copy image from Avalon RichTextBox to WinForm RichTextBox or Wordpad");

        return sr;
    }

    [Scenario("Cut, Copy and Paste image from WF RichTextBox into AV RichTextBox")]
    public ScenarioResult Scenario2(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();

        //create a image off the AV Button
        Bitmap orgBMP = new Bitmap(@"beany.bmp");
        SWF.Clipboard.SetImage(orgBMP);
        Size bmpSize = new Size(orgBMP.Width, orgBMP.Height);

        while (WFRichTextBox.IsHandleCreated == false)
            System.Windows.Forms.Application.DoEvents();

        // paste bitmap into WFRichTextBox
        WFRichTextBox.Invoke(new myEventHandler(PasteBitmap), WFRichTextBox);
        Utilities.SleepDoEvents(10);

        // copy bitmap from WFRichTextBox
        WFRichTextBox.Invoke(new myEventHandler(CopyBitmap), WFRichTextBox);
        Bitmap WF_rtbBMP = new Bitmap(SWF.Clipboard.GetImage(), bmpSize);
        if (debug) WF_rtbBMP.Save(@"WF_rtbImage2.bmp");

        // paste bitmap into AVRichTextBox
        AVRichTextBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new myEventHandler(PasteBitmap), AVRichTextBox);

        // copy bitmap from AVRichTextBox
        AVRichTextBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new myEventHandler(CopyBitmap), AVRichTextBox);
        try
        {
            Bitmap AV_rtbBMP = new Bitmap(SWF.Clipboard.GetImage(), bmpSize);
            if (debug) AV_rtbBMP.Save(@"AV_rtbImage2.bmp");
            Utilities.SleepDoEvents(10);

            WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(AV_rtbBMP, WF_rtbBMP), "Bitmaps are different");
        }
        catch (ArgumentNullException)
        {
            WPFMiscUtils.IncCounters(sr, p.log, false, "Bitmap fail to copy");
        }

        // bitmap is not identical
        if (sr.FailCount != 0)
            p.log.LogKnownBug(BugDb.WindowsOSBugs, 1532320, "Cannot copy image from Avalon RichTextBox to WinForm RichTextBox or Wordpad");

        return sr;
    }

    #endregion

    private void TestSetup()
    {
        _tp.log.WriteLine("TestSetup -- Start ");

        #region SetupAVControl
        AVStackPanel = new SWC.StackPanel();
        AVRichTextBox = new SWC.RichTextBox();
        AVStackPanel.Name = AVStackPanelName;
        AVRichTextBox.Name = AVRichTextBoxName;
        AVRichTextBox.Width = 250;
        AVRichTextBox.Height = 150;
        #endregion

        #region SetupWFControl
        wfh = new WindowsFormsHost();
        WFRichTextBox = new SWF.RichTextBox();
        wfh.Name = WFName;
        wfh.Width = 250;
        wfh.Height = 150;
        WFRichTextBox.Name = WFRichTextBoxName;
        wfh.Child = WFRichTextBox;
        #endregion

        #region LayoutWindow
        this.Title = WindowTitleName;
        AVStackPanel.Children.Add(AVRichTextBox);
        AVStackPanel.Children.Add(wfh);
        this.Content = AVStackPanel;
        #endregion

        _tp.log.WriteLine("TestSetup -- End ");
    }

    #region Utilities

    void PasteBitmap(object sender)
    {
        if (sender.GetType() == typeof(SWC.RichTextBox))
        {
            SWC.RichTextBox rtb = sender as SWC.RichTextBox;
            rtb.SelectAll();
            rtb.Paste();
        }
        else if (sender.GetType() == typeof(SWF.RichTextBox))
        {
            SWF.RichTextBox rtb = sender as SWF.RichTextBox;
            rtb.SelectAll();
            rtb.Paste();
        }
    }

    void CopyBitmap(object sender)
    {
        if (sender.GetType() == typeof(SWC.RichTextBox))
        {
            SWC.RichTextBox rtb = sender as SWC.RichTextBox;
            rtb.SelectAll();
            rtb.Copy();
        }
        else if (sender.GetType() == typeof(SWF.RichTextBox))
        {
            SWF.RichTextBox rtb = sender as SWF.RichTextBox;
            rtb.SelectAll();
            rtb.Copy();
        }
    }

    #endregion
}
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Cut, Copy and Paste image from AV RichTextBox into WF RichTextBox

//@ Cut, Copy and Paste image from WF RichTextBox into AV RichTextBox

