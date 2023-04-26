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
    /// Verify ColorAnimationUsingKeyFrames applied to a SolidColorBrush on a Border
    /// </description>
    /// </summary>
    
    [Test(0, "Animation.KeyFrames.UnitTests", "ColorKeySplineTest")]
    public class ColorKeySplineTest : WindowTest
    {
        #region Test case members

        private ClockManager        _myClockManager;
        private VisualVerifier      _verifier;
        private AnimationClock      _clock               = null;
        private bool                _testPassed          = true;
        private string              _resultInfo          = "Results: ";
        
        private int                 _expCounter          = 0;
        private Color               _actColor;
        private float               _expTolerance        = .60f;
        private Color[]             _expColors = {Colors.Black,Color.FromArgb(255,0xC9,0,0),Color.FromArgb(255,255,0,0)};
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ColorKeySplineTest Constructor
        ******************************************************************************/
        public ColorKeySplineTest()
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
            Window.Title                = "ColorKeySpline"; 

            Canvas myCanvas = new Canvas();
            Window.Content = myCanvas;

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            ColorAnimationUsingKeyFrames anim = new ColorAnimationUsingKeyFrames();

            ColorKeyFrameCollection myKeyFrames = new ColorKeyFrameCollection();
            myKeyFrames.Add(new SplineColorKeyFrame(Colors.Black, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,0)),new KeySpline(0,0,1,1)));
            myKeyFrames.Add(new SplineColorKeyFrame(Colors.Red, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,2000)),new KeySpline(0,0,1,1)));

            anim.KeyFrames = myKeyFrames;

            anim.BeginTime = TimeSpan.FromMilliseconds(2000);
            anim.Duration       = new Duration(TimeSpan.FromMilliseconds(2000));

            SolidColorBrush scBrush = new SolidColorBrush(Colors.Black);
            _clock = anim.CreateClock();
            scBrush.ApplyAnimationClock(SolidColorBrush.ColorProperty, _clock);

            Border border = new Border();
            border.Background       = scBrush;
            border.BorderThickness  = new Thickness(2.0);
            border.Width            = 200.0;
            border.Height           = 200.0;

            myCanvas.Children.Add(border);  

            int[] times = new int[]{1900,3000,6050};
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
            _actColor = _verifier.getColorAtPoint(180,25);   

            _resultInfo += "\n Color at 180,25 was " + _actColor.ToString() + " expected " + (_expColors[_expCounter]).ToString();

            if (Math.Abs(Math.Round((double)(Decimal.Round(_actColor.R,3) - _expColors[_expCounter].R) / _expColors[_expCounter].R,4)) >= _expTolerance)    //actColor.IsClose(expColors[expCounter])) 
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
