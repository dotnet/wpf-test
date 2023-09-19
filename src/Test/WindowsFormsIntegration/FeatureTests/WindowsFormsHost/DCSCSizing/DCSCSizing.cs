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
using Microsoft.Test.Display;


// Testcase:    DCSCSizing
// Description: Verify that WF Controls strech/dont strech to fill their container
namespace WindowsFormsHostTests
{
    public class DCSCSizing : WPFReflectBase
    {
        #region Testcase setup
        public DCSCSizing(string[] args) : base(args) { }

        // class vars
        private StackPanel _sp;
        private WindowsFormsHost _wfh1;
        private SWF.Button _wfBtn;
        private SWF.MonthCalendar _wfCal;
        private int _wfCalUnitWidth;         // screen width of calendar for single month
        private int _wfCalUnitHeight;        // screen height of calendar for single month

        private enum ControlType { Button, Calendar };

        protected override void InitTest(TParams p)
        {
            // hacks to get window to show up !!!
            this.Topmost = true;
            this.Topmost = false;
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;

            // get calendar dimensions (in case is different depending on platform)
            SWF.MonthCalendar mon = new System.Windows.Forms.MonthCalendar();
            mon.CalendarDimensions = new System.Drawing.Size(1, 1);
            _wfCalUnitWidth = mon.Width;
            _wfCalUnitHeight = mon.Height;

            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            bool b = base.BeforeScenario(p, scenario);

            // debug - run specific scenario !!!
            //if (scenario.Name != "Scenario3") { return false; }

            this.SizeToContent = SizeToContent.Manual;      // changed by some tests
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
        [Scenario("1) Verify that the WF Button is sized to the width of the StackPanel")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            
            // setup
            CreateTestControls(ControlType.Button);

            int xMin = 50;
            int xMax = (int)this.ActualWidth * 2;
            int yMin = 50;
            int yMax = (int)this.ActualHeight * 2;
            int origBtnHeight = _wfBtn.Height;

            // loop changing window size
            for (int i = 0; i < 10; i++)
            {
                // decide size to change to
                // (use "range" feature of p.ru.GetSize and convert to doubles)
                SD.Size newSize = p.ru.GetSize(xMin, xMax, yMin, yMax);

                // resize window
                p.log.WriteLine("Changing Window to ({0} x {1})", newSize.Width, newSize.Height);
                this.Width = newSize.Width;
                this.Height = newSize.Height;
                MyPause();

                // verify that WF button is "sized to the width of the StackPanel"
                // (the height should not be affected)
                WPFMiscUtils.IncCounters(sr, (int)Monitor.ConvertScreenToLogical(Dimension.Width, _wfBtn.Width), (int)_sp.ActualWidth, "WF Button and Panel not same Width", p.log);
                WPFMiscUtils.IncCounters(sr, _wfBtn.Height, origBtnHeight, "WF Button Height should be unchanged", p.log);
            }

            return sr;
        }

        [Scenario("2) Set AutoSize=false and verify that the control can be positioned with the alignement properties of the wfh and that the HostContainer is not visible")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // setup
            CreateTestControls(ControlType.Button);
            TestAlignment(p, sr, ControlType.Button);

            // for this test, we want to be able to make the button smaller as well as larger
            _wfBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

            // have main window automatically adjust size based on contents
            // so can cheat and look at window size to see if stuff changed
            this.SizeToContent = SizeToContent.WidthAndHeight;
            MyPause();

            LogSizes(p, ControlType.Button, "initial");

            // Note: AutoSize defaults to false
            p.log.WriteLine("Autosize defaults to '{0}'", _wfBtn.AutoSize);
            WPFMiscUtils.IncCounters(sr, false, _wfBtn.AutoSize, "Button.AutoSize should default to false", p.log);

            // save current sizes
            SD.Size origBtnSize = _wfBtn.Size;
            double origPanelWidth = _sp.ActualWidth;
            double origPanelHeight = _sp.ActualHeight;

            // play with padding
            int padDelta = 10;
            _wfh1.Padding = new Thickness(padDelta);
            MyPause();
            LogSizes(p, ControlType.Button, "after set padding");
        

            // button and panel sizes should have increased by twice the padding (both sides)
            WPFMiscUtils.IncCounters(sr, origBtnSize.Width + 2 * padDelta, _wfBtn.Size.Width, "Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, origBtnSize.Height + 2 * padDelta, _wfBtn.Size.Height, "Height not correct", p.log);
            WPFMiscUtils.IncCounters(sr, (int)origPanelWidth + (int)Monitor.ConvertScreenToLogical(Dimension.Width, 2 * padDelta), (int)_sp.ActualWidth, "Panel Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, (int)origPanelHeight + (int)Monitor.ConvertScreenToLogical(Dimension.Height, 2 * padDelta), (int)_sp.ActualHeight, "Panel Height not correct", p.log);
            CheckForUnderpants(p, sr);

            // reset padding (need AutoSizeMode.GrowAndShrink)
            _wfh1.Padding = new Thickness(0);
            MyPause();
            LogSizes(p, ControlType.Button, "after reset padding");

            // play with margins
            int marDelta = 25;
            _wfh1.Margin = new Thickness(marDelta);
            MyPause();
            LogSizes(p, ControlType.Button, "after set margins");

            // button size should remain the same
            WPFMiscUtils.IncCounters(sr, origBtnSize.Width, _wfBtn.Size.Width, "Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, origBtnSize.Height - (int)Monitor.ConvertLogicalToScreen(Dimension.Height, 0.9), _wfBtn.Size.Height, "Height not correct", p.log);

            // panel size should have increased by twice the margin (both sides)
            WPFMiscUtils.IncCounters(sr, origPanelWidth + 2 * marDelta, _sp.ActualWidth, "Panel Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, (int)origPanelHeight + 2 * marDelta, (int)_sp.ActualHeight, "Panel Height not correct", p.log);
            CheckForUnderpants(p, sr);

            return sr;
        }

        [Scenario("3) use a MonthCalendar instead of a button and verify that it is not stretched, unless it is sized big enough to display 2 months and that the HostContainer is not visible")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // setup
            CreateTestControls(ControlType.Calendar);

            // get current width of calendar
            int origWidth = _wfCal.Width;
            int origHeight = _wfCal.Height;
            p.log.WriteLine("Original Calendar ({0} x {1})", origWidth, origHeight);

            // randomize based on half-width to double width (of two month calendar)
            int xMin = origWidth / 2;
            int xMax = origWidth * 2;
            int yMin = origHeight;
            int yMax = origHeight * 2;

            // loop changing window size
            for (int i = 0; i < 10; i++)
            {
                // decide size to change to
                // (use "range" feature of p.ru.GetSize and convert to doubles)
                SD.Size newSize = p.ru.GetSize(xMin, xMax, yMin, yMax);

                // resize window (just playing with width here)
                p.log.WriteLine("Changing Window to ({0} x {1})", newSize.Width, newSize.Height);
                this.Width = newSize.Width;
                //this.Height = newSize.Height;
                MyPause();
                // calculate expected dimensions of calendar
                // for some reason, WF calendars are 7 pixels narrower when displayed in Avalon

                SD.Size calDim = _wfCal.CalendarDimensions;
                int expCalWidth = origWidth * calDim.Width + (calDim.Width - 1) * 4;
                int expCalHeight = origHeight * calDim.Height + (calDim.Height - 1) * 4;
                p.log.WriteLine("Calendar Dimensions ({0} x {1})", calDim.Width, calDim.Height);
                // Verify "that it is not stretched"
                p.log.WriteLine("StackPanel = {0} x {1}", _sp.ActualWidth, _sp.ActualHeight);
                p.log.WriteLine("Calendar = {0} x {1}", _wfCal.Width, _wfCal.Height);

                // Note: Regression_Bug22 "MonthCalendar wider than panel" used to happen here
                // 

                // calendar control should always be less wider than panel
                //WPFMiscUtils.IncCounters(sr, p.log, wfCal.Width <= (int)sp.ActualWidth, "Calendar wider than Panel");
                if (_wfCal.Width > (int)_sp.ActualWidth)
                {
                    p.log.WriteLine("-- MonthCalendar is wider than panel, but that's ok - Regression_Bug22");
                    p.log.WriteLine("-- wfCal.Width = '{0}' sp.ActualWidth = '{1}'", _wfCal.Width, _sp.ActualWidth);
                    //p.log.LogKnownBug(BugDb.WindowsOSBugs, 22, "MonthCalendar wider than panel");
                }

                // calendar should be expected size (this may be testing too much?)
                WPFMiscUtils.IncCounters(sr, expCalWidth, _wfCal.Width, "Calendar not correct width", p.log);
                WPFMiscUtils.IncCounters(sr, expCalHeight, _wfCal.Height, "Calendar not correct Height", p.log);

                // Verify "that the HostContainer is not visible"
                CheckForUnderpants(p, sr);
            }

            return sr;
        }

        [Scenario("4) use a MonthCalendar  instead of a button and verify that the control can be positioned with the alignement properties of the wfh and that the HostContainer is not visible")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // setup
            CreateTestControls(ControlType.Calendar);
            TestAlignment(p, sr, ControlType.Calendar);

            // have main window automatically adjust size based on contents
            // so can cheat and look at window size to see if stuff changed
            this.SizeToContent = SizeToContent.WidthAndHeight;
            MyPause();
            LogSizes(p, ControlType.Calendar, "initial");

            // Note: AutoSize defaults to false
            p.log.WriteLine("Autosize defaults to '{0}'", _wfCal.AutoSize);
            WPFMiscUtils.IncCounters(sr, false, _wfCal.AutoSize, "MonthCalendar.AutoSize should default to false", p.log);

            // save current sizes
            SD.Size origCalSize = _wfCal.Size;
            double origPanelWidth = _sp.ActualWidth;
            double origPanelHeight = _sp.ActualHeight;

            // play with padding
            int padDelta = 10;
            _wfh1.Padding = new Thickness(padDelta);
            MyPause();
            LogSizes(p, ControlType.Calendar, "after set padding");

            // Note from docs: "Setting the Padding property will have no effect on the appearance of the MonthCalendar."

            // calendar and panel sizes should have increased by twice the padding (both sides)
            WPFMiscUtils.IncCounters(sr, origCalSize.Width, _wfCal.Size.Width, "Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, origCalSize.Height, _wfCal.Size.Height, "Height not correct", p.log);
            WPFMiscUtils.IncCounters(sr, origPanelWidth, _sp.ActualWidth, "Panel Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, origPanelHeight, _sp.ActualHeight, "Panel Height not correct", p.log);
            CheckForUnderpants(p, sr);

            // reset padding
            _wfh1.Padding = new Thickness(0);
            MyPause();
            LogSizes(p, ControlType.Calendar, "after reset padding");

            // play with margins
            int marDelta = 25;
            _wfh1.Margin = new Thickness(marDelta);
            MyPause();
            LogSizes(p, ControlType.Calendar, "after set margins");

            // calendar size should remain the same
            WPFMiscUtils.IncCounters(sr, origCalSize.Width, _wfCal.Size.Width, "Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, origCalSize.Height, _wfCal.Size.Height, "Height not correct", p.log);

            // panel size should have increased by twice the margin (both sides)
            WPFMiscUtils.IncCounters(sr, origPanelWidth + 2 * marDelta, _sp.ActualWidth, "Panel Width not correct", p.log);
            WPFMiscUtils.IncCounters(sr, (int)origPanelHeight + 2 * marDelta, (int)_sp.ActualHeight, "Panel Height not correct", p.log);
            CheckForUnderpants(p, sr);

            return sr;
        }

        [Scenario("5) Set the WF Button to Dock.Fill and verify that the Button appears (VSWhidbey 572360")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // setup
            CreateTestControls(ControlType.Button);

            // Note: AutoSize defaults to false
            p.log.WriteLine("Autosize defaults to '{0}'", _wfBtn.AutoSize);
            WPFMiscUtils.IncCounters(sr, false, _wfBtn.AutoSize, "Button.AutoSize should default to false", p.log);

            // save bitmap of original control
            SD.Bitmap bmpOrig = Utilities.GetBitmapOfControl(_wfBtn, true);
            // debug !!!
            //bmpOrig.Save("_button.bmp");

            // iterate through the dock settings
            foreach (SWF.DockStyle ds in Enum.GetValues(typeof(SWF.DockStyle)))
            {
                p.log.WriteLine("Trying for DockStyle '{0}'", ds.ToString());
                _wfBtn.Dock = ds;
                MyPause();

                // Verify "that the Button appears"

                // get bitmap of button now
                SD.Bitmap bmpCur = Utilities.GetBitmapOfControl(_wfBtn, true);
                // debug !!!
                //bmpCur.Save("_button.bmp");

                // should match bitmap we grabbed earlier
                WPFMiscUtils.IncCounters(sr, p.log, Utilities.BitmapsIdentical(bmpOrig, bmpCur), "Bitmaps don't match");
                CheckForUnderpants(p, sr);
            }

            return sr;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Create WF Control in a WFH in a DockPanel.  Also adds surrounding Avalon buttons.
        /// </summary>
        /// <param name="ctrl"></param>
        private void CreateTestControls(ControlType ctrl)
        {
            // create dynamic panel
            _sp = new StackPanel();

            // avalon button
            Button avBtn1 = new Button();
            avBtn1.Content = "Avalon Button 1";
            _sp.Children.Add(avBtn1);

            // create WF host control
            _wfh1 = new WindowsFormsHost();

            // create WF control, add to WFH
            if (ctrl == ControlType.Button)
            {
                // create WF Button
                _wfBtn = new System.Windows.Forms.Button();
                _wfBtn.Text = "Winforms Test Button";
                _wfh1.Child = _wfBtn;

                // set colors so can see host background
                _wfBtn.Parent.Paint += new System.Windows.Forms.PaintEventHandler(Parent_Paint);
                _wfBtn.BackColor = SD.Color.LightBlue;
            }
            else if (ctrl == ControlType.Calendar)
            {
                // create WF Calendar
                _wfCal = new System.Windows.Forms.MonthCalendar();
                _wfh1.Child = _wfCal;

                // set colors so can see host background
                _wfCal.Parent.Paint += new System.Windows.Forms.PaintEventHandler(Parent_Paint);
            }
            else
            {
                throw new ArgumentException("Unknown ControlType '{0}'", ctrl.ToString());
            }

            _sp.Children.Add(_wfh1);

            // avalon button
            Button avBtn2 = new Button();
            avBtn2.Content = "Avalon Button 2";
            _sp.Children.Add(avBtn2);

            // add panel to window
            this.Background = Brushes.Beige;
            this.Content = _sp;
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
            SD.Bitmap bmpHost = Utilities.GetBitmapOfControl(_wfh1.Child.Parent, true);

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

        /// <summary>
        /// Helper routine to log current size of CheckBox control and WFH
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ctrl"></param>
        /// <param name="desc"></param>
        private void LogSizes(TParams p, ControlType ctrl, string desc)
        {
            if (ctrl == ControlType.Button)
            {
                p.log.WriteLine("Button = ({0},{1}) Host = ({2},{3}) Panel = ({4},{5}) - {6}",
                    _wfBtn.Size.Width, _wfBtn.Size.Height,
                    _wfh1.ActualWidth, _wfh1.ActualHeight,
                    _sp.ActualWidth, _sp.ActualHeight, desc);
            }
            else if (ctrl == ControlType.Calendar)
            {
                p.log.WriteLine("Calendar = ({0},{1}) Host = ({2},{3}) Panel = ({4},{5}) - {6}",
                    _wfCal.Size.Width, _wfCal.Size.Height,
                    _wfh1.ActualWidth, _wfh1.ActualHeight,
                    _sp.ActualWidth, _sp.ActualHeight, desc);
            }
        }

        #endregion

        #region Alignment Test functions
        private void TestAlignment(TParams p, ScenarioResult sr, ControlType ctrl)
        {
            bool bStretch;          // is our control expected to stretch when asked?

            // create new window
            Window mainWindow = new Window();
            mainWindow.SizeToContent = SizeToContent.WidthAndHeight;
            mainWindow.Title = "Alignment Test Window";

            // stackpanel (fixed size)
            // (color chosen is that which is not found in MonthCalendar)
            StackPanel myStackPanel = new StackPanel();
            myStackPanel.Background = Brushes.LightGreen;
            myStackPanel.Width = 300;
            myStackPanel.Height = 250;

            // host with control
            WindowsFormsHost myHost = new WindowsFormsHost();
            myHost.Background = Brushes.Yellow;
            if (ctrl == ControlType.Button)
            {
                // want a Button (can stretch)
                p.log.WriteLine("Checking Alignment using Button");
                SWF.Button wfBtn = new System.Windows.Forms.Button();
                wfBtn.Text = "WinForms";
                wfBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                myHost.Child = wfBtn;
                bStretch = true;
            }
            else
            {
                // want a Calendar (should not stretch)
                p.log.WriteLine("Checking Alignment using MonthCalendar");
                SWF.MonthCalendar wfCal = new System.Windows.Forms.MonthCalendar();
                wfCal.CalendarDimensions = new System.Drawing.Size(1, 1);
                myHost.Child = wfCal;
                bStretch = false;
            }
            myStackPanel.Children.Add(myHost);

            mainWindow.Content = myStackPanel;
            mainWindow.Show();

            // get origin of StackPanel (need for getting bitmaps)
            myHost.VerticalAlignment = VerticalAlignment.Top;
            myHost.HorizontalAlignment = HorizontalAlignment.Left;
            MyPause();
            SD.Point origin = myHost.Child.PointToScreen(new System.Drawing.Point(0, 0));

            // Horizontal placement inside Vertical panel
            myStackPanel.Orientation = Orientation.Vertical;
            foreach (HorizontalAlignment ha in Enum.GetValues(typeof(HorizontalAlignment)))
            {
                // set host alignment
                myHost.HorizontalAlignment = ha;
                MyPause();
                p.log.WriteLine("HorizontalAlignment = '{0}'", ha);

                // decide where control should be, offset to origin, grab bitmap
                SD.Rectangle expCtrl = GetExpectedLocation(myStackPanel, myHost, ha, VerticalAlignment.Top, bStretch);
                expCtrl.Offset(origin.X, origin.Y);
                SD.Bitmap bmp = Utilities.GetScreenBitmap(expCtrl);
                //!!!bmp.Save("_control.bmp");

                // bitmap should not have any StackPanel background in it
                bool bCanSeeBackground = Utilities.ContainsColor(bmp, SD.Color.LightGreen);
                WPFMiscUtils.IncCounters(sr, p.log, true, "Should not be able to see Panel background");
            }

            // Vertical placement inside Horizontal panel
            myStackPanel.Orientation = Orientation.Horizontal;
            foreach (VerticalAlignment va in Enum.GetValues(typeof(VerticalAlignment)))
            {
                // set host alignment
                myHost.VerticalAlignment = va;
                MyPause();
                p.log.WriteLine("VerticalAlignment = '{0}'", va);

                // decide where control should be, offset to origin, grab bitmap
                SD.Rectangle expCtrl = GetExpectedLocation(myStackPanel, myHost, HorizontalAlignment.Left, va, bStretch);
                expCtrl.Offset(origin.X, origin.Y);
                SD.Bitmap bmp = Utilities.GetScreenBitmap(expCtrl);
                //!!!bmp.Save("_control.bmp");

                // bitmap should not have any StackPanel background in it
                bool bCanSeeBackground = Utilities.ContainsColor(bmp, SD.Color.LightGreen);
                WPFMiscUtils.IncCounters(sr, p.log, true, "Should not be able to see Panel background");
            }

            mainWindow.Close();
        }

        /// <summary>
        /// Based on the Horizontal and Vertical alignment settings of a Host within a StackPanel, determine
        /// where the bounding rectangle of the hosted control "should" be.
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="host"></param>
        /// <param name="ha"></param>
        /// <param name="va"></param>
        /// <param name="bStretch"></param>
        /// <returns></returns>
        private SD.Rectangle GetExpectedLocation(StackPanel sp, WindowsFormsHost host, HorizontalAlignment ha, VerticalAlignment va, bool bStretch)
        {
            int posX = 0, posY = 0, sizeX = 0, sizeY = 0;

            // Note: for "Center" have to be careful if space to allocate is odd amount
            // in this case, the Topmost/Leftmost section "wins" the extra pixel, so have to adjust

            switch (ha)
            {
                case HorizontalAlignment.Left:
                    posX = 0;
                    sizeX = (int)host.ActualWidth;
                    break;

                case HorizontalAlignment.Right:
                    posX = (int)sp.ActualWidth - (int)host.ActualWidth;
                    sizeX = (int)host.ActualWidth;
                    break;

                case HorizontalAlignment.Center:
                    posX = ((int)sp.ActualWidth - (int)host.ActualWidth) / 2;
                    if ((sp.ActualWidth - host.ActualWidth) % 2 == 1) { posX++; }
                    sizeX = (int)host.ActualWidth;
                    break;

                case HorizontalAlignment.Stretch:
                    // if control should not stretch, treat like Center
                    if (bStretch)
                    {
                        posX = 0;
                        sizeX = (int)sp.ActualWidth;
                    }
                    else
                    {
                        posX = ((int)sp.ActualWidth - (int)host.ActualWidth) / 2;
                        if ((sp.ActualWidth - host.ActualWidth) % 2 == 1) { posX++; }
                        sizeX = (int)host.ActualWidth;
                    }
                    break;
            }

            switch (va)
            {
                case VerticalAlignment.Top:
                    posY = 0;
                    sizeY = (int)host.ActualHeight;
                    break;

                case VerticalAlignment.Bottom:
                    posY = (int)sp.ActualHeight - (int)host.ActualHeight;
                    sizeY = (int)host.ActualHeight;
                    break;

                case VerticalAlignment.Center:
                    posY = ((int)sp.ActualHeight - (int)host.ActualHeight) / 2;
                    if ((sp.ActualHeight - host.ActualHeight) % 2 == 1) { posY++; }
                    sizeY = (int)host.ActualHeight;
                    break;

                case VerticalAlignment.Stretch:
                    // if control should not stretch, treat like Center
                    if (bStretch)
                    {
                        posY = 0;
                        sizeY = (int)sp.ActualHeight;
                    }
                    else
                    {
                        posY = ((int)sp.ActualHeight - (int)host.ActualHeight) / 2;
                        if ((sp.ActualHeight - host.ActualHeight) % 2 == 1) { posY++; }
                        sizeY = (int)host.ActualHeight;
                    }
                    break;
            }

            // create rectangle to return
            SD.Rectangle rect = new System.Drawing.Rectangle(posX, posY, sizeX, sizeY);

            return rect;
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
            StackPanel _stackPanel;
            WindowsFormsHost _host;
            public Win()
            {
                Width = 200;
                _stackPanel = new StackPanel();

                SWF.Button button = new SWF.Button();

                Button avButton = new Button();
                avButton.Content = "AutoSize false";
                avButton.Click += delegate { button.AutoSize = false; };
                _stackPanel.Children.Add(avButton);

                avButton = new Button();
                avButton.Content = "MonthCalendar";
                avButton.Click += delegate { _host.Child = new SWF.MonthCalendar(); };
                _stackPanel.Children.Add(avButton);

                avButton = new Button();
                avButton.Content = "WebBrowser";
                avButton.Click += delegate
                {
                    SWF.WebBrowser wb = new SWF.WebBrowser();
                    wb.Url = new Uri("http://www.microsoft.com");
                    _host.Child = wb;
                };
                _stackPanel.Children.Add(avButton);

                avButton = new Button();
                avButton.Content = "Positioned MonthCalendar";
                avButton.Click += delegate
                {
                    SWF.MonthCalendar cal = new SWF.MonthCalendar();
                    _host.Child = cal;
                    cal.Location = new SD.Point(150, 150);
                };
                _stackPanel.Children.Add(avButton);

                //Which dock won't make much difference here: default is dock Fill now, though.
                avButton = new Button();
                avButton.Content = "Dock Top button";
                avButton.Click += delegate
                {
                    button = new SWF.Button();
                    button.Dock = SWF.DockStyle.Top;
                    button.Text = "I'm dock Top";
                    _host.Child = button;
                };
                _stackPanel.Children.Add(avButton);

                avButton = new Button();
                avButton.Content = "Dock Left button";
                avButton.Click += delegate
                {
                    button = new SWF.Button();
                    button.Dock = SWF.DockStyle.Left;
                    button.Text = "I'm dock Left";
                    _host.Child = button;
                };
                _stackPanel.Children.Add(avButton);

                avButton = new Button();
                avButton.Content = "Dock Fill button";
                avButton.Click += delegate
                {
                    button = new SWF.Button();
                    button.Dock = SWF.DockStyle.Fill;
                    button.Text = "I'm dock Fill";
                    _host.Child = button;
                };
                _stackPanel.Children.Add(avButton);

                _host = new WindowsFormsHost();
                _host.Background = Brushes.Blue;

                this.Background = Brushes.Beige;
                button.Text = "WinForms Button";
                _host.Child = button;
                _stackPanel.Children.Add(_host);
                Label label2 = new Label();
                label2.Content = "Another Avalon UIElement";
                label2.Background = Brushes.OrangeRed;
                _stackPanel.Children.Add(label2);
                this.AddChild(_stackPanel);
                button.Parent.BackColor = SD.Color.HotPink;
            }
        }
        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Verify that the WF Button is sized to the width of the StackPanel

//@ 2) Set AutoSize=false and verify that the control can be positioned with the alignement properties of the wfh and that the HostContainer is not visible

//@ 3) use a MonthCalendar instead of a button and verify that it is not stretched, unless it is sized big enough to display 2 months and that the HostContainer is not visible

//@ 4) use a MonthCalendar  instead of a button and verify that the control can be positioned with the alignement properties of the wfh and that the HostContainer is not visible

//@ 5) Set the WF Button to Dock.Fill and verify that the Button appears (VSWhidbey 572360
