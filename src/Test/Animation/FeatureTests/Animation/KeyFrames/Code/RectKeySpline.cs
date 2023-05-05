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
    /// <area>Animation.KeyFrames.UnitTests</area>
    /// <priority>0</priority>
    /// <description>
    /// Verify RectAnimationUsingKeyFrames applied to a LinearGradientBrush on a Border
    /// </description>
    /// </summary>
    
    // [DISABLE WHILE PORTING]
    // [Test(0, "Animation.KeyFrames.UnitTests", "RectKeySplineTest")]
    public class RectKeySplineTest : WindowTest
    {
        #region Test case members

        private ClockManager                    _myClockManager;
        private VisualVerifier                  _verifier;
        private AnimationClock                  _clock               = null;
        private bool                            _testPassed          = true;
        private string                          _resultInfo          = "Results: ";
        
        private int                             _expCounter          = 0;
        private float                           _expTolerance        = 20f;
        private Color[]                         _expColors           = {Colors.White,Colors.White,Colors.White,Colors.Red};
        private float[]                         _expWidths           = {50,170,217,241};
        private float                           _actWidth;
        private Color                           _actColor;
        private int                             _xValidationPosition = 275;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          RectKeySplineTest Constructor
        ******************************************************************************/
        public RectKeySplineTest()
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
            Window.Title                = "RectKeySpline"; 

            RectKeyFrameCollection myKeyFrames = new RectKeyFrameCollection();
            myKeyFrames.Add(new SplineRectKeyFrame(new Rect(50,50,50,100), KeyTime.FromPercent(0f),new KeySpline(.5f,0,1,.5f)));
            myKeyFrames.Add(new SplineRectKeyFrame(new Rect(50,50,250,100), KeyTime.FromPercent(1f),new KeySpline(0,.5f,.5f,1)));

            RectAnimationUsingKeyFrames anim = new RectAnimationUsingKeyFrames();
            anim.KeyFrames          = myKeyFrames;
            anim.BeginTime          = TimeSpan.FromMilliseconds(5000);
            anim.Duration           = new Duration(TimeSpan.FromMilliseconds(4000));
            anim.FillBehavior       = FillBehavior.HoldEnd;

            RectangleGeometry myRectGeom = new RectangleGeometry();
            myRectGeom.Rect =  new Rect(50,50,50,100);

            _clock = anim.CreateClock();
            myRectGeom.ApplyAnimationClock(RectangleGeometry.RectProperty, _clock);

            Path myPath = new Path();
            myPath.Data = myRectGeom;
            myPath.Fill = System.Windows.Media.Brushes.Red;

            Window.Content = (myPath);
            Window.Show();

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            int[] times = new int[]{500,6005,7005,8005};
            _myClockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);

            _myClockManager.hostManager.Resume();

            // adjust for DPI settings
            _xValidationPosition = (int)Monitor.ConvertLogicalToScreen(Dimension.Width, _xValidationPosition);
            
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
            _actColor = _verifier.getColorAtPoint(_xValidationPosition,100);   
            _actWidth = ((BoundingBoxProperties)_verifier.getBoundingBoxProperties(Colors.Red)).width ;

            _resultInfo += "\n Color at " + _xValidationPosition + ",150 was " + _actColor.ToString() + " expected " + _expColors[_expCounter];

            if (_actColor != _expColors[_expCounter]) 
            { 
                _testPassed = false; 
            }
 
            // adjust for DPI settings
            _expWidths[_expCounter] = (float)Monitor.ConvertLogicalToScreen(Dimension.Width, _expWidths[_expCounter]);

            _resultInfo += "\n width was " + _actWidth + " Expected " + _expWidths[_expCounter];

            if (Math.Abs((_actWidth - _expWidths[_expCounter])) >= _expTolerance)
            { 
                _testPassed = false; 
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
