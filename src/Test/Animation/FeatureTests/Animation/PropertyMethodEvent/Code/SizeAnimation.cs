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
    /// Verify SizeAnimation applied to ArcSegments on multiple Paths
    /// </description>
    /// </summary>
    
    [Test(0, "Animation.PropertyMethodEvent.UnitTests", "SizeAnimationTest")]
    public class SizeAnimationTest : WindowTest
    {
        #region Test case members

        private ClockManager        _myClockManager;
        private SideBySideVerifier  _sideBySide;

        private bool                _testPassed          = true;
        private string              _resultInfo          = "Results: ";
        
        private AnimationClock      _clock1              = null;
        private AnimationClock      _clock2              = null;
        private AnimationClock      _clock3              = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          SizeAnimationTest Constructor
        ******************************************************************************/
        public SizeAnimationTest()
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
            Window.Title                = "SizeAnimation"; 

            int[] times = new int[]{4500,5500,6000,6500,7025};
            _myClockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);

            SizeAnimation anim1 = new SizeAnimation();
            anim1.By                = new Size(75,75);
            anim1.BeginTime         = TimeSpan.FromMilliseconds(5000);
            anim1.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim1.FillBehavior      = FillBehavior.HoldEnd;

            ArcSegment mySegment1 = new ArcSegment();
            mySegment1.Point            = new Point(105,120);
            mySegment1.RotationAngle    = 25;
            mySegment1.Size             = new Size(1,1);
            mySegment1.IsLargeArc       = true;

            _clock1 = anim1.CreateClock();
            mySegment1.ApplyAnimationClock(ArcSegment.SizeProperty, _clock1);

            PathFigure myPathFigure1 = new PathFigure();
            myPathFigure1.StartPoint = new Point (150,150);
            myPathFigure1.Segments.Add(new LineSegment(new Point(145,155),true));
            myPathFigure1.Segments.Add(mySegment1);
            myPathFigure1.Segments.Add(new LineSegment(new Point(145,155),true));

            Path myPath1 = new Path();
            myPath1.Data                = new PathGeometry(new PathFigure[] { myPathFigure1 });
            myPath1.Fill                = System.Windows.Media.Brushes.Red;
            myPath1.StrokeThickness     = 5;
            myPath1.Stroke              = System.Windows.Media.Brushes.Blue;

            SizeAnimation anim2 = new SizeAnimation();
            anim2.From              = new Size(10,10);
            anim2.To                = new Size(65,65);
            anim2.BeginTime         = TimeSpan.FromMilliseconds(5000);
            anim2.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));

            ArcSegment mySegment2 = new ArcSegment();
            mySegment2.Point            = new Point(205,220);
            mySegment2.RotationAngle    = 75;
            mySegment2.Size             = new Size(45,45);
            mySegment2.IsLargeArc       = true;

            _clock2 = anim2.CreateClock();
            mySegment2.ApplyAnimationClock(ArcSegment.SizeProperty, _clock2);

            PathFigure myPathFigure2 = new PathFigure();
            myPathFigure2.StartPoint = new Point (250,250);
            myPathFigure2.Segments.Add(new LineSegment(new Point(245,255),true));
            myPathFigure2.Segments.Add(mySegment2);
            myPathFigure2.Segments.Add(new LineSegment(new Point(245,255),true));

            Path myPath2 = new Path();
            myPath2.Data = new PathGeometry(new PathFigure[] { myPathFigure2 });
            myPath2.Fill = System.Windows.Media.Brushes.Blue;
            myPath2.StrokeThickness = 5;
            myPath2.Stroke = System.Windows.Media.Brushes.Red;

            SizeAnimation anim3 = new SizeAnimation();
            anim3.To                = new Size(100,100);
            anim3.BeginTime         = TimeSpan.FromMilliseconds(5000);
            anim3.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim3.FillBehavior      = FillBehavior.HoldEnd;

            ArcSegment mySegment3 = new ArcSegment();
            mySegment3.Point            = new Point(305,120);
            mySegment3.RotationAngle    = 25;
            mySegment3.Size             = new Size(1,1);
            mySegment3.IsLargeArc       = true;

            _clock3 = anim3.CreateClock();
            mySegment3.ApplyAnimationClock(ArcSegment.SizeProperty, _clock3);

            PathFigure myPathFigure3 = new PathFigure();
            myPathFigure3.StartPoint = new Point (350,150);
            myPathFigure3.Segments.Add(new LineSegment(new Point(345,155),true));
            myPathFigure3.Segments.Add(mySegment3);
            myPathFigure3.Segments.Add(new LineSegment(new Point(345,155),true));

            Path myPath3 = new Path();
            myPath3.Data                = new PathGeometry(new PathFigure[] { myPathFigure3 });
            myPath3.Fill                = System.Windows.Media.Brushes.Green;
            myPath3.StrokeThickness     = 5;
            myPath3.Stroke              = System.Windows.Media.Brushes.Yellow;

            Canvas myCanvas = new Canvas();
            myCanvas.Children.Add(myPath1);
            myCanvas.Children.Add(myPath2);
            myCanvas.Children.Add(myPath3);

            Window.Content = (myCanvas);

            _sideBySide = new SideBySideVerifier(Window);
            _sideBySide.RegisterAnimation(mySegment1,ArcSegment.SizeProperty, _clock1);
            _sideBySide.RegisterAnimation(mySegment2,ArcSegment.SizeProperty, _clock2);
            _sideBySide.RegisterAnimation(mySegment3,ArcSegment.SizeProperty, _clock3);

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
                _resultInfo+="\n*********************\n";
            }

            _resultInfo += _sideBySide.verboseLog;

            if (e.lastTick) 
            {
                GlobalLog.LogEvidence(_resultInfo);

                if (_testPassed)
                {
                    Signal("AnimationDone", TestResult.Pass);
                }
                else
                {
                    Signal("AnimationDone", TestResult.Fail);
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
            return WaitForSignal("AnimationDone");
            
        }
        #endregion
    }
}
