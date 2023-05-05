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
    /// <area>Animation.LowLevelScenarios.Regressions</area>


    [Test(2, "Animation.LowLevelScenarios.Regressions", "ScaleToZeroTest")]
    public class ScaleToZeroTest : WindowTest
    {
        #region Test case members

        private ScaleTransform                  _scaleTransform  = null;
        private double                          _baseValue       = 1d;
        private double                          _fromValue       = 2d;
        private double                          _toValue         = 0d;
        private DispatcherTimer                 _aTimer          = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ScaleToZeroTest Constructor
        ******************************************************************************/
        public ScaleToZeroTest()
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

            Canvas body = new Canvas();
            body.Background = Brushes.MidnightBlue;

            Polygon polygon1 = new Polygon();
            body.Children.Add(polygon1);
            PointCollection points = new PointCollection();
            points.Add(new Point(0, 0));
            points.Add(new Point(0, 150));
            points.Add(new Point(150, 150));
            points.Add(new Point(150, 0));
            polygon1.Points = points;
            polygon1.Stroke             = Brushes.Red;
            polygon1.StrokeThickness    = 2d;
            polygon1.Fill               = Brushes.LightYellow;

            Canvas.SetTop  (polygon1, 50d);
            Canvas.SetLeft (polygon1, 50d);

            _scaleTransform = new ScaleTransform();
            _scaleTransform.ScaleX    = _baseValue;
            _scaleTransform.ScaleY    = _baseValue;
            polygon1.RenderTransform = _scaleTransform;

            Window.Content = body;

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
            _aTimer.Start();
            
            GlobalLog.LogStatus("----DispatcherTimer Started----");
            
            DoubleAnimationUsingKeyFrames animScale = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            DKFC.Add(new DiscreteDoubleKeyFrame(_fromValue, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500))));
            DKFC.Add(new DiscreteDoubleKeyFrame(_toValue,   KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1500))));
            animScale.KeyFrames = DKFC;

            AnimationClock clock1 = animScale.CreateClock();
            _scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleXProperty, clock1);
            _scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleYProperty, clock1);
            
            return TestResult.Pass;
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
            Signal("AnimationDone", TestResult.Pass);
            _aTimer.Stop();
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
            WaitForSignal("AnimationDone");

            double actValue1 = (double)_scaleTransform.GetValue(ScaleTransform.ScaleXProperty);
            double actValue2 = (double)_scaleTransform.GetValue(ScaleTransform.ScaleYProperty);
            double expValue  = _toValue;

            GlobalLog.LogEvidence("------------RESULTS------------");
            GlobalLog.LogEvidence("Act Value 1: " + actValue1);
            GlobalLog.LogEvidence("Act Value 2: " + actValue2);
            GlobalLog.LogEvidence("Exp Value:   " + expValue);
            GlobalLog.LogEvidence("-------------------------------");

            if ( actValue1 == expValue && actValue2 == expValue)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion
    }
}
