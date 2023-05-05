// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.KeyFrames.UnitTests</area>
    /// <priority>0</priority>
    /// <description>
    /// Verify PointAnimationUsingKeyFrames applied to a LinearGradientBrush on a Border
    /// </description>
    /// </summary>
    
    // [DISABLE WHILE PORTING]
    // [Test(0, "Animation.KeyFrames.UnitTests", "PointKeySplineTest")]
    public class PointKeySplineTest : WindowTest
    {
        #region Test case members

        private ClockManager                    _myClockManager;
        private VisualVerifier                  _verifier;
        private AnimationClock                  _clock               = null;
        private bool                            _testPassed          = true;
        private string                          _resultInfo          = "Results: ";
        
        private int                             _expCounter          = 0;
        private Color                           _actColor;
        private float                           _expTolerance        = .30f;
        private Color[]                         _expColors           = {Colors.Red,Colors.Blue};
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          PointKeySplineTest Constructor
        ******************************************************************************/
        public PointKeySplineTest()
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
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult StartTest()
        {
            GlobalLog.LogStatus("---StartTest---");

            Window.Width                = 500d;
            Window.Height               = 500d;
            Window.Left                 = 0d;
            Window.Top                  = 0d;
            Window.Title                = "PointKeySpline"; 

            PointAnimationUsingKeyFrames anim = new PointAnimationUsingKeyFrames();

            PointKeyFrameCollection myKeyFrames = new PointKeyFrameCollection();
            myKeyFrames.Add(new SplinePointKeyFrame(new Point(0,1), KeyTime.FromPercent(0f),new KeySpline(.5f,0,1,.5f)));
            myKeyFrames.Add(new SplinePointKeyFrame(new Point(1,0), KeyTime.FromPercent(1f),new KeySpline(0,.5f,.5f,1)));

            anim.KeyFrames      = myKeyFrames;
            anim.BeginTime      = TimeSpan.FromMilliseconds(3000);
            anim.Duration       = new Duration(TimeSpan.FromMilliseconds(2000));

            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint        = new Point(0.0, 0.0);
            brush.EndPoint          = new Point(0.0, 1.0);
            brush.MappingMode       = BrushMappingMode.RelativeToBoundingBox;
            brush.GradientStops     = new GradientStopCollection();
            brush.GradientStops.Add(new GradientStop(Colors.Red, 0.0));
            brush.GradientStops.Add(new GradientStop(Colors.Blue, 1.0));
            _clock = anim.CreateClock();
            brush.ApplyAnimationClock(LinearGradientBrush.EndPointProperty, _clock);

            Border myBorder = new Border();
            myBorder.HorizontalAlignment = HorizontalAlignment.Left;
            myBorder.VerticalAlignment      = VerticalAlignment.Top;
            myBorder.Width                  = 250;
            myBorder.Height                 = 250;
            myBorder.Background             = brush;

            Canvas myCanvas = new Canvas();
            myCanvas.HorizontalAlignment    = HorizontalAlignment.Left;
            myCanvas.VerticalAlignment      = VerticalAlignment.Top;
            myCanvas.Width                  = 50.0;
            myCanvas.Height                 = 50.0;

            myCanvas.Children.Add(myBorder);  
            Window.Content = myCanvas;

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            int[] times = new int[]{2000,6000};
            _myClockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);

            _myClockManager.hostManager.Resume();
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          OnTimeTicked
        ******************************************************************************/
        /// <summary>
        /// OnTimeTicked:  Verifies the Animation at specified intervals.
        /// </summary>
        private void OnTimeTicked(object sender, TimeControlArgs e)
        {
            _actColor = _verifier.getColorAtPoint(249,1);   


            if (_actColor != _expColors[_expCounter]) 

            if ((Math.Abs(Math.Round((double)(Decimal.Round(_actColor.R,3) - _expColors[_expCounter].R) / _expColors[_expCounter].R,4)) >= _expTolerance) &&
                (Math.Abs(Math.Round((double)(Decimal.Round(_actColor.B,3) - _expColors[_expCounter].B) / _expColors[_expCounter].B,4)) >= _expTolerance))
            { 
                _testPassed = false; 
                _resultInfo += "\n Color at 249,1 was " + _actColor.ToString() + " expected " + _expColors[_expCounter];
            }

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
            _expCounter ++;
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
