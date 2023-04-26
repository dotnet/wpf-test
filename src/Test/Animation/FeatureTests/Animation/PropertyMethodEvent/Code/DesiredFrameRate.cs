// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Display;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.UnitTests</area>
    /// <priority>0</priority>
    /// <description>
    /// Verify DesiredFrameRate set on an Animation
    /// </description>
    /// </summary>

    // [DISABLE WHILE PORTING]
    // [Test(2, "Animation.PropertyMethodEvent.UnitTests", "DesiredFrameRateTest")]
    public class DesiredFrameRateTest : WindowTest
    {
        #region Test case members

        private ClockManager                _myClockManager;
        private VisualVerifier              _verifier;
        private bool                        _testPassed      = true;
        private string                      _resultInfo      = "-------------RESULTS-------------\n";
        private AnimationClock              _clock1          = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          DesiredFrameRateTest Constructor
        ******************************************************************************/
        public DesiredFrameRateTest()
        {
            InitializeSteps += new TestStep(StartTest);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Creates the window content and starts an Animation.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult StartTest()
        {
            GlobalLog.LogStatus("---StartTest---");

            Window.Width       = 500;
            Window.Height      = 500;
            Window.Title       = "animation"; 

            int[] times = new int[]{2550,6500,8000};
            _myClockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);

            RectAnimation anim1 = new RectAnimation();
            anim1.To                = new Rect(50,50,300,50);
            anim1.BeginTime         = TimeSpan.FromMilliseconds(5000);
            anim1.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim1.FillBehavior      = FillBehavior.HoldEnd;
            Timeline.SetDesiredFrameRate(anim1,1);

            RectangleGeometry myRectGeom1 = new RectangleGeometry();
            myRectGeom1.Rect = new Rect(50,50,1,50);
            _clock1 = anim1.CreateClock();
            myRectGeom1.ApplyAnimationClock(RectangleGeometry.RectProperty, _clock1);

            Path myPath = new Path();
            myPath.Data = myRectGeom1;

            myPath.Fill = System.Windows.Media.Brushes.Red;

            Canvas myCanvas = new Canvas();
            myCanvas.Children.Add(myPath);

            Window.Content = (myCanvas);
            Window.Show();

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            _myClockManager.hostManager.Resume();

            return TestResult.Pass;
        }
    

        /******************************************************************************
        * Function:          OnTimeTicked
        ******************************************************************************/
        /// <summary>
        /// OnTimeTicked:  Verifies the Animation at specified intervals.
        /// </summary>
        /// <returns></returns>
        public void OnTimeTicked(object sender, TimeControlArgs e)
        {        
            BoundingBoxProperties screencap = (BoundingBoxProperties) _verifier.getBoundingBoxProperties(Colors.Red);
            Single r = screencap.right;
            _resultInfo += "(At " + e.curTime + ")  Actual Right position: " + r + "\n";



            if (e.curTime < 6300)
            {
                _resultInfo += "(At " + e.curTime + ") Expected Right position: <= 51" + "\n";
                if (r > 51f)
                {
                    _testPassed = false;
                    _resultInfo += "Result: FALSE\n";
                }
                else
                {
                    _resultInfo += "Result: TRUE\n";
                }
            }
            if (e.curTime > 6300 && e.curTime < 7000)
            {
                double expected = 200;

                // DPI adjustment
                expected = Math.Floor( Monitor.ConvertLogicalToScreen(Dimension.Width, expected) );

                _resultInfo += "(At " + e.curTime + ") Expected Right position: " + expected + "\n";
                if (r != expected)
                {
                    _testPassed = false;
                    _resultInfo += "Result: FALSE\n";
                }
                else
                {
                    _resultInfo += "Result: TRUE\n";
                }
            }
            if (e.curTime > 8000)
            {
                double expected = 350;

                // DPI adjustment
                expected = Math.Floor( Monitor.ConvertLogicalToScreen(Dimension.Width, expected) );

                _resultInfo += "(At " + e.curTime + ") Expected Right position: " + expected +"\n";
                if (r != expected)
                {
                    _testPassed = false;
                    _resultInfo += "Result: FALSE\n";
                }
                else
                {
                    _resultInfo += "Result: TRUE\n";
                }
            }

            _resultInfo += "--------------------------------------\n";

            if (e.lastTick) 
            {
                GlobalLog.LogEvidence(_resultInfo);

                if (_testPassed)
                {
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    Signal("TestFinished", TestResult.Fail);
                }
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }
        #endregion
    }
}
