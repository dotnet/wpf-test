// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "ChildValueTest")]
    public class ChildValueTest : WindowTest
    {
        #region Test case members

        private Polyline            _polyline1          = null;
        private Storyboard          _storyboard;
        private double              _baseValue           = 2d;
        private double              _fromValue           = 1d;
        private double              _toValue             = 10d;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          ChildValueTest Constructor
        ******************************************************************************/
        public ChildValueTest()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            Window.Width        = 600d;
            Window.Height       = 400d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.Title        = "Animation";

            Canvas body = new Canvas();
            body.Background = Brushes.Magenta;

            _polyline1 = new Polyline();
            body.Children.Add(_polyline1);
            _polyline1.Stroke            = Brushes.MistyRose;
            _polyline1.StrokeThickness   = _baseValue;
            _polyline1.FillRule          = FillRule.EvenOdd;
            Point Point4 = new Point(1,   150);
            Point Point5 = new Point(110, 180);
            Point Point6 = new Point(120, 140);
            PointCollection myPointCollection2 = new PointCollection();
            myPointCollection2.Add(Point4);
            myPointCollection2.Add(Point5);
            myPointCollection2.Add(Point6);
            _polyline1.Points = myPointCollection2;
            Canvas.SetTop  (_polyline1, 50d);
            Canvas.SetLeft (_polyline1, 50d);          
            
            Window.Content = body;

            CreateStoryboard();

            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromMilliseconds(2500);
            _aTimer.Start();
            
            GlobalLog.LogStatus("----DispatcherTimer Started----");
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          CreateStoryboard
        ******************************************************************************/
        /// <summary>
        /// Create and begin the Animation.
        /// </summary>
        /// <returns></returns>
        private void CreateStoryboard()
        {
            DoubleAnimationUsingKeyFrames animKeyFrame = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            DKFC.Add(new LinearDoubleKeyFrame(_fromValue, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            DKFC.Add(new LinearDoubleKeyFrame(_toValue,   KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));
            animKeyFrame.KeyFrames = DKFC;

            animKeyFrame.BeginTime      = TimeSpan.FromSeconds(1);
            animKeyFrame.FillBehavior   = FillBehavior.Stop;

            _storyboard = new Storyboard();
            _storyboard.Name = "story";
            _storyboard.Children.Add(animKeyFrame);
            PropertyPath path1 = new PropertyPath("(0)", new DependencyProperty[] { Shape.StrokeThicknessProperty });
            Storyboard.SetTargetProperty(_storyboard, path1);
            
        }

        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)          
        {
            _tickCount++;
            
            if (_tickCount == 1)
            {
                _storyboard.Begin(_polyline1, false);  //The KeyFrame animation is offset by 1 sec.
            }
            else if (_tickCount == 2)
            {
                _storyboard.Begin(_polyline1, false);
                _storyboard.Pause(_polyline1);
            }
            else if (_tickCount == 3)
            {
                _aTimer.Stop();

                double expValue = _baseValue;
                double actValue = _polyline1.StrokeThickness;

                GlobalLog.LogEvidence("---------- Result ------");
                GlobalLog.LogEvidence(" Actual   : " + actValue);
                GlobalLog.LogEvidence(" Expected : " + expValue);

                if (actValue == expValue)
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
