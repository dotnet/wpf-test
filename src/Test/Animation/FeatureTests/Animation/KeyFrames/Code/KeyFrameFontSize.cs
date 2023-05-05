// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
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
    /// <priority>2</priority>

    [Test(2, "Animation.KeyFrames.Regressions", "KeyFrameFontSizeTest")]
    public class KeyFrameFontSizeTest : WindowTest
    {
        #region Test case members

        private Button                          _button1         = null;
        private DoubleAnimationUsingKeyFrames   _animKeyFrame;
        private double                          _actValue        = 0;
        private AnimationClock                  _clock1          = null;
        private double                          _baseValue       = 18d;
        private double                          _fromValue       = 12d;
        private double                          _toValue         = 24d;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          KeyFrameFontSizeTest Constructor
        ******************************************************************************/
        public KeyFrameFontSizeTest()
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
            body.Background = Brushes.SpringGreen;
            body.Height     = 300d;
            body.Width      = 300d;

            _button1 = new Button();
            body.Children.Add(_button1);
            _button1.SetValue(Canvas.LeftProperty, 50d);
            _button1.SetValue(Canvas.TopProperty,  50d);
            _button1.Content         = "Avalon!";
            _button1.FontSize        = _baseValue;
            
            _animKeyFrame = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            DKFC.Add(new DiscreteDoubleKeyFrame(_fromValue, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            DKFC.Add(new DiscreteDoubleKeyFrame(_toValue,   KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2))));
            _animKeyFrame.KeyFrames = DKFC;

            _animKeyFrame.BeginTime  = null;
            _animKeyFrame.Duration   = new Duration(TimeSpan.FromSeconds(3));
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
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
                _clock1 = _animKeyFrame.CreateClock();
                _clock1.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
                _button1.ApplyAnimationClock(Button.FontSizeProperty, _clock1);

                _clock1.Controller.Begin();
                _clock1.Controller.Pause();
                _clock1.Controller.Seek(TimeSpan.FromSeconds(2), TimeSeekOrigin.BeginTime);
            }
            else
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
            }
        }

        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("--------CurrentStateInvalidated [" + ((Clock)sender).CurrentState + "]  " + _actValue);
            _actValue = (double)_button1.GetValue(Button.FontSizeProperty);

            Clock CL = (Clock)sender;
            AnimationClock AC = (AnimationClock)CL;
            GlobalLog.LogStatus("--------AC:     " + (double)AC.GetCurrentValue(0d, 0d));
            GlobalLog.LogStatus("--------clock1: " + (double)_clock1.GetCurrentValue(0d, 0d));
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

            double expValue = _toValue;

            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value:       " + expValue);
            GlobalLog.LogEvidence("Actual Value:         " + _actValue);

            if (_actValue == expValue)
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
