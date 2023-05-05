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


    [Test(2, "Animation.LowLevelScenarios.Regressions", "DiscreteToTest")]
    public class DiscreteToTest : WindowTest
    {
        #region Test case members

        private string                          _inputString     = "";
        private SkewTransform                   _skewTransform   = null;
        private double                          _baseValue       = 0d;
        private double                          _toValue         = 30d;
        private double                          _actValue1       = 0;
        private double                          _actValue2       = 0;
        private double                          _actValue3       = 0;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor

        [Variation("TimeSpan")]
        [Variation("Paced", Priority=0)]
        [Variation("Percent")]
        [Variation("Uniform")]
        
        /******************************************************************************
        * Function:          DiscreteToTest Constructor
        ******************************************************************************/
        public DiscreteToTest(string testValue)
        {
            _inputString = testValue;
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
            polygon1.Stroke             = Brushes.Purple;
            polygon1.StrokeThickness    = 2d;
            polygon1.Fill               = Brushes.PaleGreen;

            Canvas.SetTop  (polygon1, 150d);
            Canvas.SetLeft (polygon1, 150d);

            _skewTransform = new SkewTransform();
            _skewTransform.AngleX    = _baseValue;
            polygon1.RenderTransform = _skewTransform;
            
            DoubleAnimationUsingKeyFrames animSkew = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            
            switch (_inputString)
            {
                case "TimeSpan" :
                    DKFC.Add(new DiscreteDoubleKeyFrame(_toValue, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
                    break;
                case "Paced" :
                    DKFC.Add(new DiscreteDoubleKeyFrame(_toValue, KeyTime.Paced));
                    break;
                case "Percent" :
                    DKFC.Add(new DiscreteDoubleKeyFrame(_toValue, KeyTime.FromPercent(0)));
                    break;
                case "Uniform" :
                    DKFC.Add(new DiscreteDoubleKeyFrame(_toValue, KeyTime.Uniform));
                    break;
            }
            animSkew.KeyFrames = DKFC;
            animSkew.Duration = new Duration(TimeSpan.FromSeconds(2));
            AnimationClock clock1 = animSkew.CreateClock();
            _skewTransform.ApplyAnimationClock(SkewTransform.AngleXProperty, clock1);

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,500);
            _aTimer.Start();
            
            GlobalLog.LogStatus("----DispatcherTimer Started----");
            
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
            _tickCount++;
            
            if (_tickCount == 1)
            {
                _actValue1 = (double)_skewTransform.GetValue(SkewTransform.AngleXProperty);  
            }
            else if (_tickCount == 3)
            {
                _actValue2 = (double)_skewTransform.GetValue(SkewTransform.AngleXProperty);  
            }
            else if (_tickCount == 6)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
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
            WaitForSignal("AnimationDone");

            _actValue3 = (double)_skewTransform.GetValue(SkewTransform.AngleXProperty);

            double expValue1  = 0d;
            double expValue2  = 0d;
            double expValue3  = 0d;

            switch (_inputString)
            {
                case "TimeSpan" :
                    expValue1 = _toValue;
                    expValue2 = _toValue;
                    expValue3 = _toValue;
                    break;
                case "Paced" :
                    expValue1 = _baseValue;
                    expValue2 = _baseValue;
                    expValue3 = _toValue;
                    break;
                case "Percent" :
                    expValue1 = _toValue;
                    expValue2 = _toValue;
                    expValue3 = _toValue;
                    break;
                case "Uniform" :
                    expValue1 = _baseValue;
                    expValue2 = _baseValue;
                    expValue3 = _toValue;
                    break;
            }

            bool b1 = (_actValue1 == expValue1);
            bool b2 = (_actValue2 == expValue2);
            bool b3 = (_actValue3 == expValue3);

            GlobalLog.LogEvidence("------------RESULTS------------");
            GlobalLog.LogEvidence("Act Value 1:   " + _actValue1);
            GlobalLog.LogEvidence("Exp Value 1:   " + expValue1);
            GlobalLog.LogEvidence("Act Value 2:   " + _actValue2);
            GlobalLog.LogEvidence("Exp Value 2:   " + expValue2);
            GlobalLog.LogEvidence("Act Value 3:   " + _actValue3);
            GlobalLog.LogEvidence("Exp Value 3:   " + expValue3);
            GlobalLog.LogEvidence("-------------------------------");
            GlobalLog.LogEvidence("Comparisons: " + b1 + "/" + b2 + "/" + b3);
            GlobalLog.LogEvidence("-------------------------------");

            if ( b1 && b2 && b3)
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
