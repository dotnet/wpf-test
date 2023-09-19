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
using System.Drawing;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;


// Testcase:    WindowsOSBug1532320
// Description: Cut, Copy and Paste image between WF control and AV control
namespace WindowsFormsHostTests
{
    public class WindowsOSBug1532320 : WPFReflectBase 
    {
        #region TestVariables
        private delegate void myEventHandler(object sender);
        private bool _debug = false;         // set this true for TC debugging
        private TParams _tp;

        private SWC.StackPanel _AVStackPanel;
        private SWC.RichTextBox _AVRichTextBox;

        private WindowsFormsHost _wfh;
        private SWF.RichTextBox _WFRichTextBox;

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
            _AVRichTextBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new myEventHandler(PasteBitmap), _AVRichTextBox);

            // copy bitmap from AVRichTextBox
            _AVRichTextBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new myEventHandler(CopyBitmap), _AVRichTextBox);
            try
            {
                Bitmap AV_rtbBMP = new Bitmap(SWF.Clipboard.GetImage(), bmpSize);
                if (_debug) AV_rtbBMP.Save(@"AV_rtbImage.bmp");
                // paste bitmap into WFRichTextBox
                while (_WFRichTextBox.IsHandleCreated == false)
                    System.Windows.Forms.Application.DoEvents();
                _WFRichTextBox.Invoke(new myEventHandler(PasteBitmap), _WFRichTextBox);
                Utilities.SleepDoEvents(10);

                // copy bitmap fromWFRichTextBox
                _WFRichTextBox.Invoke(new myEventHandler(CopyBitmap), _WFRichTextBox);
                Bitmap WF_rtbBMP = new Bitmap(SWF.Clipboard.GetImage());
                if (_debug) WF_rtbBMP.Save(@"WF_rtbImage.bmp");
                WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(AV_rtbBMP, WF_rtbBMP), "Bitmaps are different");
            }
            catch (ArgumentNullException)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Bitmap fail to copy");
            }

            if (sr.FailCount != 0)
                p.log.LogKnownBug(BugDb.WindowsOSBugs, 1, "Cannot copy image from Avalon RichTextBox to WinForm RichTextBox or Wordpad");

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

            while (_WFRichTextBox.IsHandleCreated == false)
                System.Windows.Forms.Application.DoEvents();

            // paste bitmap into WFRichTextBox
            _WFRichTextBox.Invoke(new myEventHandler(PasteBitmap), _WFRichTextBox);
            Utilities.SleepDoEvents(10);

            // copy bitmap from WFRichTextBox
            _WFRichTextBox.Invoke(new myEventHandler(CopyBitmap), _WFRichTextBox);
            Bitmap WF_rtbBMP = new Bitmap(SWF.Clipboard.GetImage(), bmpSize);
            if (_debug) WF_rtbBMP.Save(@"WF_rtbImage2.bmp");

            // paste bitmap into AVRichTextBox
            _AVRichTextBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new myEventHandler(PasteBitmap), _AVRichTextBox);

            // copy bitmap from AVRichTextBox
            _AVRichTextBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new myEventHandler(CopyBitmap), _AVRichTextBox);
            try
            {
                Bitmap AV_rtbBMP = new Bitmap(SWF.Clipboard.GetImage(), bmpSize);
                if (_debug) AV_rtbBMP.Save(@"AV_rtbImage2.bmp");
                Utilities.SleepDoEvents(10);

                WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(AV_rtbBMP, WF_rtbBMP), "Bitmaps are different");
            }
            catch (ArgumentNullException)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Bitmap fail to copy");
            }

            // bitmap is not identical
            if (sr.FailCount != 0)
                p.log.LogKnownBug(BugDb.WindowsOSBugs, 1, "Cannot copy image from Avalon RichTextBox to WinForm RichTextBox or Wordpad");

            return sr;
        }

        #endregion

        private void TestSetup()
        {
            _tp.log.WriteLine("TestSetup -- Start ");

            #region SetupAVControl
            _AVStackPanel = new SWC.StackPanel();
            _AVRichTextBox = new SWC.RichTextBox();
            _AVStackPanel.Name = AVStackPanelName;
            _AVRichTextBox.Name = AVRichTextBoxName;
            _AVRichTextBox.Width = 250;
            _AVRichTextBox.Height = 150;
            #endregion

            #region SetupWFControl
            _wfh = new WindowsFormsHost();
            _WFRichTextBox = new SWF.RichTextBox();
            _wfh.Name = WFName;
            _wfh.Width = 250;
            _wfh.Height = 150;
            _WFRichTextBox.Name = WFRichTextBoxName;
            _wfh.Child = _WFRichTextBox;
            #endregion

            #region LayoutWindow
            this.Title = WindowTitleName;
            _AVStackPanel.Children.Add(_AVRichTextBox);
            _AVStackPanel.Children.Add(_wfh);
            this.Content = _AVStackPanel;
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
