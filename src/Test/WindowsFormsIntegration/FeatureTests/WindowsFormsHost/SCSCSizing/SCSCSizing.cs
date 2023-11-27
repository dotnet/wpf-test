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


//
// Testcase:    SCSCSizing
// Description: Verify that sizing for static control in a static container is correct
//
namespace WindowsFormsHostTests
{
    public class SCSCSizing : WPFReflectBase
    {
        #region Testcase setup
        public SCSCSizing(string[] args) : base(args) { }

        // class vars
        private Canvas _canvas;
        private WindowsFormsHost _wfh1;
        private SWF.CheckBox _chkbx;

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
            //if (scenario.Name != "Scenario6") { return false; }

            this.Title = currentScenario.Name;
            this.Width = 500;
            this.Height = 300;

            // create test controls
            CreateTestControls();

            return b;
        }

        private void CreateTestControls()
        {
            // create WF control
            // (want to use CheckBox rather than Button because Buttons are too dynamic)
            _chkbx = new SWF.CheckBox();
            _chkbx.Text = "Check Me";

            // create WF host control
            _wfh1 = new WindowsFormsHost();
            _wfh1.Child = _chkbx;

            // set colors so can see host background
            _chkbx.Parent.Paint += new System.Windows.Forms.PaintEventHandler(Parent_Paint);
            _chkbx.BackColor = SD.Color.LightBlue;
            this.Background = Brushes.Beige;

            // create canvas
            _canvas = new Canvas();
            Canvas.SetLeft(_wfh1, 100);
            Canvas.SetTop(_wfh1, 100);
            _canvas.Children.Add(_wfh1);

            // add canvas to window
            this.Content = _canvas;
        }
        
        void Parent_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.Clear(System.Drawing.Color.HotPink);
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("1) Verify the control is sized to it's content and the WFH is the size of the button")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Note: AutoSize defaults to false
            p.log.WriteLine("Autosize defaults to '{0}'", _chkbx.AutoSize);
            WPFMiscUtils.IncCounters(sr, false, _chkbx.AutoSize, "CheckBox.AutoSize should default to false", p.log);

            // make AutoSize true
            _chkbx.AutoSize = true;
            p.log.WriteLine("Autosize is '{0}'", _chkbx.AutoSize);

            TestCheckBoxText(p, sr, false);

            return sr;
        }

        [Scenario("2) Set AutoSize=false on the control. Verify the control is not sized to it's content and the WFH is the size of the control")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // Note: AutoSize defaults to false
            p.log.WriteLine("Autosize defaults to '{0}'", _chkbx.AutoSize);
            WPFMiscUtils.IncCounters(sr, false, _chkbx.AutoSize, "CheckBox.AutoSize should default to false", p.log);

            // make AutoSize false
            _chkbx.AutoSize = false;
            p.log.WriteLine("Autosize is '{0}'", _chkbx.AutoSize);

            TestCheckBoxText(p, sr, true);

            return sr;
        }

        private void TestCheckBoxText(TParams p, ScenarioResult sr, bool bFixedSize)
        {
            // set text string to first test value (so can set initial size)
            _chkbx.Text = p.ru.GetString(1, 1);
            MyPause();

            // save original control size
            SD.Size origSize = _chkbx.Size;
            LogCheckboxHostSize(p);

            // lets try increasing length text strings
            for (int i = 2; i < 50; i += 2)
            {
                // set text to random string of specified length
                _chkbx.Text = p.ru.GetString(i, i);
                MyPause();

                // check host size
                LogCheckboxHostSize(p);
                int hostWidth = (int)_wfh1.ActualWidth;
                int hostHeight = (int)_wfh1.ActualHeight;

                if (bFixedSize)
                {
                    // verify "control is not sized to it's content" (stays fixed size)
                    WPFMiscUtils.IncCounters(sr, origSize.Width, _chkbx.Size.Width, "Width different", p.log);
                    WPFMiscUtils.IncCounters(sr, origSize.Height, _chkbx.Size.Height, "Height different", p.log);
                }
                else
                {
                    // verify "control is sized to it's content" (grows in size)
                    // Note: this is a checkbox - it should stay the same height, but get wider
                    bool bWider = _chkbx.Size.Width > origSize.Width;
                    WPFMiscUtils.IncCounters(sr, p.log, bWider, "Width should have increased");
                    WPFMiscUtils.IncCounters(sr, origSize.Height, _chkbx.Size.Height, "Height different", p.log);
                }

                // verify the control and host are the same size
                WPFMiscUtils.IncCounters(sr, hostWidth, (int)Monitor.ConvertScreenToLogical(Dimension.Width, _chkbx.Width), "Host and Control not same width", p.log);
                WPFMiscUtils.IncCounters(sr, hostHeight, (int)Monitor.ConvertScreenToLogical(Dimension.Height, _chkbx.Height), "Host and Control not same height", p.log);

                // check that host container is not visible
                // We are painting the host container background Hot Pink.
                // Grab a bitmap of the control and look for that color
                // get bitmap
                SD.Bitmap bmpCtrl = Utilities.GetBitmapOfControl(_chkbx, true);
                // debug !!!
                //bmpCtrl.Save("_comboboxOrig.bmp");

                // does the bitmap have any Pink in it?
                bool bBleedover = Utilities.ContainsColor(bmpCtrl, SD.Color.HotPink);
                WPFMiscUtils.IncCounters(sr, p.log, !bBleedover, "I can see the Host Control's underpants!");
            }
        }

        [Scenario("3) Set AutoSize=false and make the WFH size smaller than the control.  Verify that the control is clipped")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            string longText = "This is a Windows Forms Checkbox.  Doesn't it look nice?";

            // Note: AutoSize defaults to false
            p.log.WriteLine("Autosize defaults to '{0}'", _chkbx.AutoSize);
            WPFMiscUtils.IncCounters(sr, false, _chkbx.AutoSize, "CheckBox.AutoSize should default to false", p.log);

            // set long CheckBox text
            _chkbx.AutoSize = true;
            _chkbx.Text = longText;
            MyPause();

            // save current sizes
            SD.Size origCB = _chkbx.Size;
            double origHostW = _wfh1.ActualWidth;
            double origHostH = _wfh1.ActualHeight;
            p.log.WriteLine("Original WFH ({0},{1})", origHostW, origHostH);

            // get bitmap
            SD.Bitmap bmpCtrl = Utilities.GetBitmapOfControl(_chkbx, true);
            // debug !!!
            //bmpCtrl.Save("_comboboxOrig.bmp");

            // make host smaller
            for (double len = origHostW - 10; len > origHostW / 2; len -= 10)
            {
                // set host width
                _wfh1.Width = len;
                MyPause();

                LogCheckboxHostSize(p);

                // "Verify that the button is clipped"

                // CheckBox should still be same size (even though cannot see all of it)
                WPFMiscUtils.IncCounters(sr, origCB.Width, _chkbx.Width, "CheckBox Control not same width", p.log);
                WPFMiscUtils.IncCounters(sr, origCB.Height, _chkbx.Height, "CheckBox Control not same height", p.log);

                // Host should be smaller (narrower) than original
                int hostWidth = (int)_wfh1.ActualWidth;
                int hostHeight = (int)_wfh1.ActualHeight;
                WPFMiscUtils.IncCounters(sr, p.log, hostWidth <= _chkbx.Width, "Host should be narrower");
                WPFMiscUtils.IncCounters(sr, p.log, hostHeight == (int)Monitor.ConvertScreenToLogical(Dimension.Height, _chkbx.Height), "Host should be same height");

                // Here's the plan, Stan:
                // The CheckBox should be "clipped", which means that (in this case) the far end
                // of the CheckBox is not visible because it is being truncated and covered with the background.
                // Take the rectangle that represents the location of the CB on the screen.  Divide
                // that into the area to the "left" of a dividing line and the area to the "right."
                // (the dividing line is the "expected end of the control")
                // The stuff on the right should be the color of the background, and not the combobox

                // create rectangle
                int expWidth = (int)_wfh1.Width;
                SD.Rectangle rectL = new System.Drawing.Rectangle(0, 0, expWidth, _chkbx.ClientSize.Height);
                SD.Rectangle rectR = new System.Drawing.Rectangle((int)Monitor.ConvertLogicalToScreen(Dimension.Width, expWidth)+5, 0, _chkbx.ClientSize.Width - expWidth, _chkbx.ClientSize.Height);
                SD.Bitmap bmpL = Utilities.GetBitmapOfControl(_chkbx, rectL, true);
                SD.Bitmap bmpR = Utilities.GetBitmapOfControl(_chkbx, rectR, true);

                // debug !!!
                //bmpL.Save("_comboboxL.bmp");
                bmpR.Save("_comboboxR.bmp");

                // does the right side have any CB backgound in it?
                bool bBleedover = Utilities.ContainsColor(bmpR, SD.Color.LightBlue);
                WPFMiscUtils.IncCounters(sr, p.log, !bBleedover, "Control not being clipped properly");
            }

            return sr;
        }

        [Scenario("4) Verify that the HostContainer is not visible with AutoSize = true")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true, "Covered in Scenario 1");
            return sr;
        }

        [Scenario("5) Verify that the HostContainer is not visible with AutoSize = false")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true, "Covered in Scenario 2");
            return sr;
        }

        [Scenario("6) Explicitly set the size of the WFH and verify that it is the same size as the control")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // set text
            _chkbx.Text = "WinForms CheckBox With Long Name";
            MyPause();

            // set AutoSize false (is default)
            p.log.WriteLine("Autosize is '{0}'", _chkbx.AutoSize);
            WPFMiscUtils.IncCounters(sr, false, _chkbx.AutoSize, "CheckBox.AutoSize should default to false", p.log);

            // get current size (of WFH)
            int origWidth = (int)_wfh1.ActualWidth;
            int origHeight = (int)_wfh1.ActualHeight;
            LogCheckboxHostSize(p);

            // change CheckBox size
            for (int i = 0; i < 20; i++)
            {
                // decide new size
                // (yes, we are converting from doubles to ints and back again, but this is
                // fine for what we are doing here - also p.ru.GetDouble() does not support a range)
                SD.Size newSize = p.ru.GetSize(0, 2 * origWidth, 0, 2 * origHeight);

                // set size
                p.log.WriteLine("Setting WFH size to ({0},{1})", newSize.Width, newSize.Height);
                _wfh1.Width = (double)newSize.Width;
                _wfh1.Height = (double)newSize.Height;

                MyPause();
                //LogCheckboxHostSize(p);

                // debug !!!
                //bool bSameWidth = _chkbx.Width == (int)_wfh1.ActualWidth;
                //bool bSameHeight = _chkbx.Height == (int)_wfh1.ActualHeight;
                //if (!bSameWidth || !bSameHeight)
                //{
                //    // get bitmap
                //    SD.Bitmap bmpCtrl = Utilities.GetBitmapOfControl(_chkbx, true);
                //    bmpCtrl.Save("_combobox.bmp");
                //    Utilities.ActiveFreeze("Width/Height mismatch - Check bitmap");
                //}

                // verify control and host are same size
                WPFMiscUtils.IncCounters(sr, (int)Monitor.ConvertScreenToLogical(Dimension.Width, _chkbx.Width), (int)_wfh1.ActualWidth, "Not same width", p.log);
                WPFMiscUtils.IncCounters(sr, (int)Monitor.ConvertScreenToLogical(Dimension.Height, _chkbx.Height), (int)_wfh1.ActualHeight, "Not same height", p.log);
            }

            return sr;
        }

        #region Helpers

        /// <summary>
        /// Helper routine to log current size of CheckBox control and WFH
        /// </summary>
        /// <param name="p"></param>
        private void LogCheckboxHostSize(TParams p)
        {
            SD.Size btnSize = _chkbx.Size;
            double curHostW = _wfh1.ActualWidth;
            double curHostH = _wfh1.ActualHeight;
            p.log.WriteLine("CheckBox size= ({0},{1}) Host size = ({2},{3})",
                btnSize.Width, btnSize.Height, curHostW, curHostH);
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

        #endregion

        #region Original Test code found in Testcase
        private void Test()
        {
            Win w = new Win();
            w.Show();
        }

        public class Win : Window
        {
            WindowsFormsHost _host;
            SWF.MonthCalendar _cal;
            bool _oof = false;
            public Win()
            {
                SWF.Button button = new SWF.Button();
                button.Click += delegate
                {
                    _cal.CalendarDimensions = _oof ? new SD.Size(1, 1) : new SD.Size(2, 2);
                    _oof ^= true;
                };
                _host = new WindowsFormsHost();
                _host.Background = Brushes.Blue;
                this.Background = Brushes.Beige;
                button.Text = "WF Button with some text";
                _host.Child = button;

                Canvas canvas = new Canvas();
                canvas.Children.Add(_host);
                _host.SetValue(Canvas.LeftProperty, 25d);
                _host.SetValue(Canvas.TopProperty, 30d);

                Button autoSizeButton = new Button();
                autoSizeButton.Content = "AutoSize = " + (!button.AutoSize).ToString();
                autoSizeButton.Click += delegate
                {
                    button.AutoSize ^= true;
                    //button.Dock = SWF.DockStyle.None;
                    autoSizeButton.Content = "AutoSize = " + (!button.AutoSize).ToString();
                };
                autoSizeButton.SetValue(Canvas.LeftProperty, 25d);
                autoSizeButton.SetValue(Canvas.TopProperty, 200d);
                canvas.Children.Add(autoSizeButton);

                Button avButton = new Button();
                avButton.Content = "Make button bigger";
                avButton.Click += delegate
                {
                    button.Size = new System.Drawing.Size(button.Width + 20, button.Height + 20);
                };
                avButton.SetValue(Canvas.LeftProperty, 25d);
                avButton.SetValue(Canvas.TopProperty, 230d);
                canvas.Children.Add(avButton);

                avButton = new Button();
                avButton.Content = "Make WFH bigger";
                avButton.Click += delegate
                {
                    if (double.IsNaN(_host.Width))
                    {
                        _host.Width = 200;
                        _host.Height = 50;
                    }
                    else
                    {
                        _host.Width += 20;
                        _host.Height += 10;
                    }
                };
                avButton.SetValue(Canvas.LeftProperty, 25d);
                avButton.SetValue(Canvas.TopProperty, 260d);
                canvas.Children.Add(avButton);

                avButton = new Button();
                avButton.Content = "Make WFH smaller";
                avButton.Click += delegate
                {
                    if (double.IsNaN(_host.Width))
                    {
                        _host.Width = 100;
                        _host.Height = 50;
                    }
                    else
                    {
                        _host.Width -= 20;
                        _host.Height -= 10;
                    }
                };
                avButton.SetValue(Canvas.LeftProperty, 25d);
                avButton.SetValue(Canvas.TopProperty, 150d);
                canvas.Children.Add(avButton);

                Grid sp = new Grid();
                for (int i = 0; i < 5; i++)
                {
                    ColumnDefinition def = new ColumnDefinition();
                    sp.ColumnDefinitions.Add(def);
                }
                sp.Height = 150;
                WindowsFormsHost wfh = new WindowsFormsHost();
                _cal = new SWF.MonthCalendar();
                _cal.AutoSize = false;
                _cal.CalendarDimensions = new System.Drawing.Size(2, 2);
                wfh.Child = _cal;

                sp.Children.Add(wfh);
                sp.SetValue(Canvas.LeftProperty, 25d);
                sp.SetValue(Canvas.TopProperty, 300d);
                canvas.Children.Add(sp);
                sp.Background = Brushes.Blue;
                wfh.Background = Brushes.Yellow;
                Content = canvas;
            }
        }

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Verify the control is sized to it's content and the WFH is the size of the control

//@ 2) Set AutoSize=false on the control. Verify the button is not sized to it's content and the WFH is the size of the control

//@ 3) Set AutoSize=false and make the WFH size smaller than the control.  Verify that the control is clipped

//@ 4) Verify that the HostContainer is not visible with AutoSize = true

//@ 5) Verify that the HostContainer is not visible with AutoSize = false

//@ 6) Explicitly set the size of the WFH and verify that it is the same size as the control
