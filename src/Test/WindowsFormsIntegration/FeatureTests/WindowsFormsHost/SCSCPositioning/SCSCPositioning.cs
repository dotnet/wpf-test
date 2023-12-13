// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SD = System.Drawing;

//
// Testcase:    SCSCPositioning
// Description: Verify that the position for the static control in a static container is correct
//
namespace WindowsFormsHostTests
{
    public class SCSCPositioning : WPFReflectBase
    {
        #region Testcase setup
        public SCSCPositioning(string[] args) : base(args) { }

        // class vars
        private Canvas _can;
        private WindowsFormsHost _wfh1;
        private SWF.Button _wfBtn;
        private double _ctrlXpos;
        private double _ctrlYpos;
        private int _xoffset;            // X,Y offset from window origin to client area origin
        private int _yoffset;            // (so that test will work regardless of themes, border size, etc.)
        private int _ctrlWidth;          // keep track of original control dimensions
        private int _ctrlHeight;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("1) Canvas - change the position of the WFH and verify that the WF control is in the correct location before and after the change")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // setup
            CreateTestControls();

            // define range !!!
            // allow for border widths and title bar
            double clientX = this.ActualWidth - 8;
            double clientY = this.ActualHeight - 30;

            // define so button can go partway off screen
            // want to have some remain, so can check for color
            // (this is *a* way, may not be the best way!)
            long xMin = -(_wfBtn.Width) + 5;
            long xMax = Convert.ToInt64(clientX - _wfBtn.Width - 5);
            long yMin = -(_wfBtn.Height) + 5;
            long yMax = Convert.ToInt64(clientY - _wfBtn.Height - 5);

            // log stuff
            p.log.WriteLine("Window = ({0} x {1})", this.ActualWidth, this.ActualHeight);
            p.log.WriteLine("Button = ({0} x {1})", _wfBtn.Width, _wfBtn.Height);
            p.log.WriteLine("Range: X = ({0} to {1}) Y = ({2} to {3})", xMin, xMax, yMin, yMax);

            // try numerous times
            for (int i = 0; i < 50; i++)
            {
                // randomly decide new position for WFH
                // (don't have "p.ru.GetDouble(double min, double max)" so use Int64, convert to double)
                _ctrlXpos = (double)p.ru.GetInt64(xMin, xMax);
                _ctrlYpos = (double)p.ru.GetInt64(yMin, yMax);

                // "change the position of the WFH"
                Canvas.SetLeft(_wfh1, _ctrlXpos);
                Canvas.SetTop(_wfh1, _ctrlYpos);
                MyPause();

                // debug !!!
                string str = String.Format("X={0} Y={1}", _ctrlXpos, _ctrlYpos);
                p.log.WriteLine(str);
                this.Title = str;

                // "verify that the WF control is in the correct location"
                // "before and after the change"
                // (since in loop, this "after" test is also the "before" test for next iteration)
                VerifyHostPosition(p, sr);
            }

            return sr;
        }

        #region Helpers

        /// <summary>
        /// Create WF Control in a WFH in a Canvas.
        /// </summary>
        private void CreateTestControls()
        {
            // create Canvas
            _can = new Canvas();

            // create WF host control
            _wfh1 = new WindowsFormsHost();
            _wfh1.Background = Brushes.Yellow;

            // create WF Button
            _wfBtn = new SWF.Button();
            _wfBtn.Text = "Here I am!";
            _wfh1.Child = _wfBtn;

            // add to Canvas, set initial position
            _can.Children.Add(_wfh1);
            _ctrlXpos = 0;
            _ctrlYpos = 0;
            Canvas.SetLeft(_wfh1, _ctrlXpos);
            Canvas.SetTop(_wfh1, _ctrlYpos);

            // add canvas to window
            this.Background = Brushes.Beige;
            this.Content = _can;
            MyPause();

            // save offset values (based on position of control placed at 0,0)
            SD.Rectangle ctrlRect = _wfBtn.RectangleToScreen(_wfBtn.ClientRectangle);
            _xoffset = ctrlRect.X - (int)this.RestoreBounds.X;
            _yoffset = ctrlRect.Y - (int)this.RestoreBounds.Y;
            _ctrlWidth = _wfBtn.Width;
            _ctrlHeight = _wfBtn.Height;
            scenarioParams.log.WriteLine("Got Xoffset= {0} Yoffset= {1}", _xoffset, _yoffset);
        }

        /// <summary>
        /// Helper function to pause to let events get processed (waits on both message pumps)
        /// </summary>
        private static void MyPause()
        {
            WPFReflectBase.DoEvents();
            SWF.Application.DoEvents();
            System.Threading.Thread.Sleep(300);
        }

        /// <summary>
        /// Helper to verify position of control by looking at where it should be and checking color
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sr"></param>
        private void VerifyHostPosition(TParams p, ScenarioResult sr)
        {
            // get rectangle representing screen coordinates of WF button
            SD.Rectangle wfBtnRect = _wfBtn.RectangleToScreen(_wfBtn.ClientRectangle);

            // calculate what control position should be (based on offset of control at 0,0)
            double expX = this.RestoreBounds.X + _ctrlXpos + _xoffset;
            double expY = this.RestoreBounds.Y + _ctrlYpos + _yoffset;

            // compare position, size of control
            WPFMiscUtils.IncCounters(sr, expX, (double)wfBtnRect.X, "X position not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, expY, (double)wfBtnRect.Y, "Y position not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, _ctrlWidth, wfBtnRect.Width, "Control Width not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, _ctrlHeight, wfBtnRect.Height, "Control Height not as expected", p.log);

            // get bitmap of hosted control
            this.Topmost = true;
            SD.Bitmap bmp = Utilities.GetBitmapOfControl(_wfBtn, true);
            this.Topmost = false;

            // check for yellow
            WPFMiscUtils.IncCounters(sr, p.log, Utilities.ContainsColor(bmp, SD.Color.Yellow),
                "Cannot find color in control");
        }

        #endregion

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Canvas - change the position of the WFH and verify that the WF control is in the correct location before and after the change
