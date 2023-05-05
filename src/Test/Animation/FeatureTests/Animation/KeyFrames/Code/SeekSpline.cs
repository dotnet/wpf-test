// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
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

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.KeyFrames.Regressions</area>

    [Test(2, "Animation.KeyFrames.Regressions", "SeekSplineTest")]
    public class SeekSplineTest : WindowTest
    {
        #region Test case members

        private Rectangle           _rectangle1       = null;
        private AnimationClock      _clock1          = null;
        private RotateTransform     _rotateTransform = null;
        private double              _fromValue       = -67d;
        private double              _toValue         = 400d;
        private DispatcherTimer     _aTimer          = null;
        private int                 _tickCount       = 0;
        private bool                _passed1         = false;
        private bool                _passed2         = false;
        private int                 _timeCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          SeekSplineTest Constructor
        ******************************************************************************/
        public SeekSplineTest()
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
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.MediumTurquoise;

            _rotateTransform = new RotateTransform();
            _rotateTransform.Angle = 0d;

            _rectangle1 = new Rectangle();
            body.Children.Add(_rectangle1);
            _rectangle1.SetValue(Canvas.LeftProperty, 10d);
            _rectangle1.SetValue(Canvas.TopProperty, 10d);
            _rectangle1.Fill              = Brushes.DarkViolet;
            _rectangle1.Height            = 150d;
            _rectangle1.Width             = 150d;
            _rectangle1.RenderTransform   = _rotateTransform;

            DoubleAnimationUsingKeyFrames animKeyFrame = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            
            KeySpline keySpline = new KeySpline(0, 0, 1, 1);
            DKFC.Add(new SplineDoubleKeyFrame(_fromValue, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), keySpline));
            DKFC.Add(new SplineDoubleKeyFrame(_toValue));
            animKeyFrame.KeyFrames = DKFC;
            animKeyFrame.Duration = new Duration(TimeSpan.FromSeconds(4));

            _clock1 = animKeyFrame.CreateClock();
            _rotateTransform.ApplyAnimationClock(RotateTransform.AngleProperty, _clock1);

            Window.Content = body;

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts the DispatcherTimer.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1500);
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
                //Seek to the beginning of the Animation.
                _clock1.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
                _clock1.Controller.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
                GlobalLog.LogStatus("----Seeking to the Beginning----");
            }
            else if (_tickCount == 2)
            {
                //Seek to the end of the Animation.
                _clock1.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
                _clock1.Controller.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.Duration);
                GlobalLog.LogStatus("----Seeking to the End----");
            }
            else if (_tickCount == 3)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
            }
        }

        /******************************************************************************
           * Function:          OnCurrentTimeInvalidated
           ******************************************************************************/
        private void OnCurrentTimeInvalidated(object sender, EventArgs e)
        {
            double actValue = _rotateTransform.CloneCurrentValue().Angle;
            GlobalLog.LogStatus(_timeCount + " -- " + actValue);

            if (actValue == _fromValue)
            {
                if (_timeCount == 0)
                {
                    _passed1 = true;
                    GlobalLog.LogEvidence("---------------------------------------");
                    GlobalLog.LogEvidence("-----Result: Seek to the Beginning-----");
                    GlobalLog.LogEvidence("Expected Value: " + _fromValue);
                    GlobalLog.LogEvidence("Actual Value:   " + actValue);
                    GlobalLog.LogEvidence("---------------------------------------");
                    _clock1.CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
                }
            }

            if (actValue == _toValue)
            {
                _passed2 = true;
                GlobalLog.LogEvidence("---------------------------------------");
                GlobalLog.LogEvidence("-----Result: Seek to the End-----");
                GlobalLog.LogEvidence("Expected Value: " + _toValue);
                GlobalLog.LogEvidence("Actual Value:   " + actValue);
                GlobalLog.LogEvidence("---------------------------------------");
                _clock1.CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
            }
            
            _timeCount++;
        }

        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            double actValue = _rotateTransform.CloneCurrentValue().Angle;
            GlobalLog.LogStatus("----CurrentStateInvalidated [" + ((Clock)sender).CurrentState + "]  " + actValue);
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
            
            if (_passed1 && _passed2)
            {
                return TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("-----Expected value(s) not found: " + _fromValue + " and/or " + _toValue);
                return TestResult.Fail;
            }
        }

        #endregion
    }
}
