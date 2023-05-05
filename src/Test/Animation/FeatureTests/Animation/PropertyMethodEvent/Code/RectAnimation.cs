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
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.UnitTests</area>
    /// <priority>0</priority>
    /// <description>
    /// Verify RectAnimation applied to RectangleGeometries on multiple Paths
    /// </description>
    /// </summary>
    
    [Test(0, "Animation.PropertyMethodEvent.UnitTests", "RectAnimationTest")]
    public class RectAnimationTest : WindowTest
    {
        #region Test case members

        private ClockManager        _myClockManager;
        private SideBySideVerifier  _sideBySide;

        private bool                _testPassed          = true;
        private string              _resultInfo          = "Results: ";
        
        AnimationClock              _clock1              = null;
        AnimationClock              _clock2              = null;
        AnimationClock              _clock3              = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          RectAnimationTest Constructor
        ******************************************************************************/
        public RectAnimationTest()
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

            Window.Width                = 500;
            Window.Height               = 500;
            Window.Title                = "RectAnimation"; 

            int[] times = new int[]{4500,5500,6000,6500,7050};
            _myClockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);

            RectAnimation anim1 = new RectAnimation();
            anim1.By                = new Rect(0,0,50,85);
            anim1.BeginTime         = TimeSpan.FromMilliseconds(5000);
            anim1.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim1.FillBehavior      = FillBehavior.HoldEnd;

            RectangleGeometry myRectGeom1 = new RectangleGeometry();
            myRectGeom1.Rect = new Rect(50,50,10,75);
            _clock1 = anim1.CreateClock();
            myRectGeom1.ApplyAnimationClock(RectangleGeometry.RectProperty, _clock1);

            Path myPath = new Path();
            myPath.Data = myRectGeom1;
            myPath.Fill = System.Windows.Media.Brushes.Red;

            RectAnimation anim2 = new RectAnimation();
            anim2.From              = new Rect(110,105,50,100);
            anim2.To                = new Rect(175,155,450,150);
            anim2.BeginTime         = TimeSpan.FromMilliseconds(5000);
            anim2.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));

            RectangleGeometry myRectGeom2 = new RectangleGeometry();
            myRectGeom2.Rect = new Rect(50,150,50,75);
            _clock2 = anim2.CreateClock();
            myRectGeom2.ApplyAnimationClock(RectangleGeometry.RectProperty, _clock2);

            Path myPath2 = new Path();
            myPath2.Data = myRectGeom2;
            myPath2.Fill = System.Windows.Media.Brushes.Blue;

            RectAnimation anim3 = new RectAnimation();
            anim3.To                = new Rect(75,300,75,400);
            anim3.BeginTime         = TimeSpan.FromMilliseconds(5000);
            anim3.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim3.FillBehavior      = FillBehavior.HoldEnd;

            RectangleGeometry myRectGeom3 = new RectangleGeometry();
            myRectGeom3.Rect = new Rect(50,250,50,75);
            _clock3 = anim3.CreateClock();
            myRectGeom3.ApplyAnimationClock(RectangleGeometry.RectProperty, _clock3);

            Path myPath3 = new Path();
            myPath3.Data = myRectGeom3;
            myPath3.Fill = System.Windows.Media.Brushes.Yellow;

            Canvas myCanvas = new Canvas();
            myCanvas.Children.Add(myPath);
            myCanvas.Children.Add(myPath2);
            myCanvas.Children.Add(myPath3);

            Window.Content = (myCanvas);

            _sideBySide = new SideBySideVerifier(Window);
            _sideBySide.RegisterAnimation(myRectGeom1,RectangleGeometry.RectProperty, _clock1);
            _sideBySide.RegisterAnimation(myRectGeom2,RectangleGeometry.RectProperty, _clock2);
            _sideBySide.RegisterAnimation(myRectGeom3,RectangleGeometry.RectProperty, _clock3);

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
            bool tempResult = _sideBySide.Verify(e.curTime);
            if (!tempResult)
            {
                _testPassed = false;
                _resultInfo +="\n*********************\n";
            }

            _resultInfo += _sideBySide.verboseLog;

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
