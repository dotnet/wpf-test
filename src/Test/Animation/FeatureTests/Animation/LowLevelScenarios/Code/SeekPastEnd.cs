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

    [Test(2, "Animation.LowLevelScenarios.Regressions", "SeekPastEndTest")]
    public class SeekPastEndTest : WindowTest
    {
        #region Test case members

        private Rectangle           _rectangle;
        private TranslateTransform  _translateTransform;
        private Storyboard          _storyboard;
        private double              _baseValue       = 20d;
        private double              _fromValue       = 50d;
        private double              _toValue         = 80d;
        private DispatcherTimer     _aTimer          = null;
        private int                 _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          SeekPastEndTest Constructor
        ******************************************************************************/
        public SeekPastEndTest()
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
            body.Background = Brushes.Red;
            body.Height     = 200d;
            body.Width      = 200d;

            _translateTransform = new TranslateTransform();
            _translateTransform.X = _baseValue;

            _rectangle = new Rectangle();
            body.Children.Add(_rectangle);
            _rectangle.SetValue(Canvas.LeftProperty, 10d);
            _rectangle.SetValue(Canvas.TopProperty, 10d);
            _rectangle.Fill              = Brushes.White;
            _rectangle.Height            = 50d;
            _rectangle.Width             = 50d;
            _rectangle.Name              = "TheRectangle";
            _rectangle.StrokeThickness   = 1d;
            _rectangle.Stroke            = Brushes.Teal;
            _rectangle.RenderTransform   = _translateTransform;
            
            DoubleAnimationUsingKeyFrames animKeyFrame = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            DKFC.Add(new DiscreteDoubleKeyFrame(_fromValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,0))));
            DKFC.Add(new DiscreteDoubleKeyFrame(_toValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,2000))));
            animKeyFrame.KeyFrames = DKFC;

            animKeyFrame.BeginTime      = TimeSpan.FromMilliseconds(0);
            animKeyFrame.Duration       = new Duration(TimeSpan.FromMilliseconds(2000));
            animKeyFrame.IsAdditive     = true;
            
            _storyboard = new Storyboard();
            _storyboard.Name = "story";
            _storyboard.Children.Add(animKeyFrame);
            PropertyPath path1 = new PropertyPath("(0).(1)", new DependencyProperty[] { UIElement.RenderTransformProperty, TranslateTransform.XProperty });
            Storyboard.SetTargetProperty(_storyboard, path1);

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
                _storyboard.Begin(_rectangle, true);
                _storyboard.Pause(_rectangle);
                _storyboard.Seek(_rectangle, TimeSpan.FromMilliseconds(1000), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 2)
            {
                //Seek past the end of the Animation.
                _storyboard.Seek(_rectangle, TimeSpan.FromMilliseconds(5000), TimeSeekOrigin.BeginTime);
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

            double actValue = (double)_translateTransform.GetValue(TranslateTransform.XProperty);
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
