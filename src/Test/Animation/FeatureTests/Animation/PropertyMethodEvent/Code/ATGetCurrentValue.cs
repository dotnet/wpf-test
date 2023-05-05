// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
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
    /// <area>Animation.PropertyMethodEvent</area>
    /// <priority>2</priority>
    /// <description>
    /// Verify GetCurrentValue on an AnimationTimeline, after the animation has finished.
    /// Scenario: AnimationTimeline's GetCurrentValue property.
    /// </description>
    /// </summary>
    [Test(2, "Animation.PropertyMethodEvent", "ATGetCurrentValueTest")]
    public class ATGetCurrentValueTest : WindowTest
    {
        #region Test case members

        private DispatcherTimer         _aTimer      = null;
        private Frame                   _frame1;
        private SolidColorBrush         _SCB;
        private AnimationTimeline       _AT;
        private AnimationClock          _clock;
        private double                  _toValue     = 0.1d;
        private double                  _actValue1   = 0d;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ATGetCurrentValueTest Constructor
        ******************************************************************************/
        public ATGetCurrentValueTest()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Carries out initialization of the Window.
        /// </summary>
        /// <returns>Returns TestResult</returns>
        TestResult Initialize()
        {
            Window.Left                 = 0d;
            Window.Top                  = 0d;
            Window.Height               = 300;
            Window.Width                = 300;

            Canvas body = new Canvas();
            body.Background = Brushes.BlueViolet;

            _SCB = new SolidColorBrush();
            _SCB.Color = Colors.DeepPink;
            
            _frame1 = new Frame();
            body.Children.Add(_frame1);
            _frame1.BorderBrush      = _SCB;
            _frame1.BorderThickness  = new Thickness(5d);
            _frame1.Background       = Brushes.MistyRose;
            _frame1.Content          = "Avalon!";
            _frame1.Height           = 200d;
            _frame1.Width            = 200d;
            Canvas.SetTop  (_frame1, 50d);
            Canvas.SetLeft (_frame1, 50d);   

            Window.Content = body;

            DoubleAnimation anim1 = new DoubleAnimation();
            anim1.To                = _toValue;
            anim1.BeginTime         = TimeSpan.FromMilliseconds(0);
            anim1.Duration          = new Duration(TimeSpan.FromSeconds(1));

            _AT = (AnimationTimeline)anim1;
            _clock = _AT.CreateClock();
            _clock.CurrentStateInvalidated  += new EventHandler(OnCurrentState);
            _SCB.ApplyAnimationClock(SolidColorBrush.OpacityProperty, _clock);

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,3000);
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
            Signal("AnimationDone", TestResult.Pass);
            _aTimer.Stop();
        }
          
        /******************************************************************************
        * Function:          OnCurrentState
        ******************************************************************************/
        private void OnCurrentState(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != ClockState.Active)
            {
                AnimationClock animationClock = (AnimationClock)sender;
                AnimationTimeline animationTimeline = (AnimationTimeline)animationClock.Timeline;
                _actValue1 = (double)animationTimeline.GetCurrentValue(0d, 0d, animationClock);
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

            double actValue2 = (double)_AT.GetCurrentValue(0d, 0d, _clock);
            double expValue = _toValue;

            GlobalLog.LogEvidence("------------------RESULTS------------------");
            GlobalLog.LogEvidence("--Actual Value [via event sender]:   " + _actValue1);
            GlobalLog.LogEvidence("--Actual Value [via AnimationClock]: " + actValue2);
            GlobalLog.LogEvidence("--Expected Value: " + expValue);
            
            if (_actValue1 == expValue && actValue2 == expValue)
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
