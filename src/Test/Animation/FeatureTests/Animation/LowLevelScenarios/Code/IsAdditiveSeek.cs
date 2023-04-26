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
    /// <area>Animation.LowLevelScenarios.Regressions</area>
    /// <priority>2</priority>

    [Test(2, "Animation.LowLevelScenarios.Regressions", "IsAdditiveSeekTest")]
    public class IsAdditiveSeekTest : WindowTest
    {
        #region Test case members

        private Rectangle                       _rectangle;
        private DoubleAnimationUsingKeyFrames   _animKeyFrame;
        private AnimationClock                  _clock;
        private double                          _baseValue       = 50d;
        private double                          _fromValue       = 0d;
        private double                          _toValue         = 100d;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          IsAdditiveSeekTest Constructor
        ******************************************************************************/
        public IsAdditiveSeekTest()
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
            body.Background = Brushes.Sienna;
            body.Height     = 300d;
            body.Width      = 300d;

            _rectangle = new Rectangle();
            body.Children.Add(_rectangle);
            _rectangle.SetValue(Canvas.LeftProperty, 10d);
            _rectangle.SetValue(Canvas.TopProperty, 10d);
            _rectangle.Fill              = Brushes.OrangeRed;
            _rectangle.Height            = 50d;
            _rectangle.Width             = _baseValue;
            _rectangle.Name              = "TheRectangle";
            _rectangle.StrokeThickness   = 1d;
            _rectangle.Stroke            = Brushes.Black;
            
            _animKeyFrame = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            DKFC.Add(new DiscreteDoubleKeyFrame(_fromValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,0))));
            DKFC.Add(new DiscreteDoubleKeyFrame(_toValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,3000))));
            _animKeyFrame.KeyFrames = DKFC;

            _animKeyFrame.BeginTime      = TimeSpan.FromMilliseconds(0);
            _animKeyFrame.Duration       = new Duration(TimeSpan.FromMilliseconds(4000));
            _animKeyFrame.IsAdditive     = true;

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,750);
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
                _clock = _animKeyFrame.CreateClock();
                _rectangle.ApplyAnimationClock(FrameworkElement.WidthProperty, _clock);
                _clock.Controller.Begin();
                _clock.Controller.Pause();
                _clock.Controller.Seek(TimeSpan.FromMilliseconds(1000), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 2)
            {
                //Seek past the end of the Animation.
                _clock.Controller.Seek(TimeSpan.FromMilliseconds(3500), TimeSeekOrigin.BeginTime);
            }
            else
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

            double actValue = (double)_rectangle.GetValue(FrameworkElement.WidthProperty);
            double expValue = _toValue + _baseValue;  //IsAdditive=true, so must add in base value.
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue == expValue)
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
