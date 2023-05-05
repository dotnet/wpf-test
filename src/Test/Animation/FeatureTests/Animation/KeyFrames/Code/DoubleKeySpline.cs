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
    /// Verify DoubleAnimationUsingKeyFrames applied to a SolidColorBrush on a Border
    /// </description>
    /// </summary>
    
    [Test(0, "Animation.KeyFrames.UnitTests", "DoubleKeySplineTest")]
    public class DoubleKeySplineTest : WindowTest
    {
        #region Test case members

        private ClockManager                    _myClockManager;
        private VisualVerifier                  _verifier;
        private DoubleAnimationUsingKeyFrames   _anim;
        private Color                           _actColor;
        private AnimationClock                  _clock               = null;
        private bool                            _testPassed          = true;
        private string                          _resultInfo          = "Results: ";
        
        private int                             _expCounter          = 0;
        private float                           _expTolerance        = .2f;
        private Double[]                        _expValues           = {0,0.70,.922,1,1};
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          DoubleKeySplineTest Constructor
        ******************************************************************************/
        public DoubleKeySplineTest()
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
            Window.Title                = "DoubleKeySplineBVT"; 

            Canvas myCanvas = new Canvas();
            Window.Content = myCanvas;

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            _anim = new DoubleAnimationUsingKeyFrames();
            _anim.BeginTime      = TimeSpan.FromMilliseconds(5000);
            _anim.Duration       = new Duration(TimeSpan.FromMilliseconds(2000));
            _anim.AutoReverse    = true;
            _anim.FillBehavior   = FillBehavior.HoldEnd;

            DoubleKeyFrameCollection myKeyFrames = new DoubleKeyFrameCollection();
            myKeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,0)),new KeySpline(.5f,0,1,.5f)));
            myKeyFrames.Add(new SplineDoubleKeyFrame(1, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,1500)),new KeySpline(0,.5f,.5f,1)));
            _anim.KeyFrames = myKeyFrames;

            SolidColorBrush scBrush = new SolidColorBrush(Colors.Blue);
            scBrush.Opacity = 0;
            
            _clock = _anim.CreateClock();
            scBrush.ApplyAnimationClock(SolidColorBrush.OpacityProperty, _clock);

            Border border = new Border();
            border.Background       = scBrush;
            border.BorderThickness  = new Thickness(2.0);
            border.Width            = 200.0;
            border.Height           = 200.0;

            myCanvas.Children.Add(border);  

            int[] times = new int[]{5001,5501,6001,6501,7001};
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
            _actColor = _verifier.getColorAtPoint(180,50);   

            double floatValue = 1-Math.Abs((double)(Decimal.Round(_actColor.R,3) / 255));
            _resultInfo += "\n Opacity at 180,25 was " + (1 -Decimal.Round(_actColor.R,3) / 255) + " expected "  + (_expValues[_expCounter]).ToString();

            if (  Math.Abs(((floatValue - _expValues[_expCounter]))) >= _expTolerance)
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
