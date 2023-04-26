// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;   //UserInput
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Regressions</area>
    /// Scenario:  remove the animation and set the animated DP to its held value.
    /// </summary>
    [Test(2, "Animation.HighLevelScenarios.Regressions", "RemoveAndHoldTest")]
    public class RemoveAndHoldTest : WindowTest
    {
        #region Test case members

        private Slider              _slider1         = null;
        private double              _fromValue       = 5d;
        private double              _toValue         = 240d;
        private DispatcherTimer     _aTimer          = null;
        private int                 _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          RemoveAndHoldTest Constructor
        ******************************************************************************/
        public RemoveAndHoldTest()
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
        /// Creates the window content and starts a DispatcherTimer used for controlling 
        /// verification of the Animation.
        /// </summary>
        /// <returns>Returns success</returns>
        TestResult CreateTree()
        {
            Window.Width        = 200d;
            Window.Height       = 150d;

            Canvas body  = new Canvas();
            Window.Content = body;
            body.Background = Brushes.MidnightBlue;
            
            _slider1 = new Slider();
            body.Children.Add(_slider1);
            _slider1.Height          = 100d;
            _slider1.Background      = Brushes.LightBlue;
            _slider1.Orientation     = Orientation.Vertical;
            _slider1.Minimum         = 5d;
            _slider1.Maximum         = 255d;
            _slider1.SmallChange     = 1d;
            _slider1.LargeChange     = 16d;
            _slider1.Value           = 5d;

            DoubleAnimation animation = new DoubleAnimation(_fromValue, _toValue, TimeSpan.FromSeconds(2));
            animation.CurrentStateInvalidated += new EventHandler(OnCurrentState);

            _slider1.BeginAnimation(Slider.ValueProperty, animation);

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnCurrentState
        ******************************************************************************/
        private void OnCurrentState(object sender, EventArgs args)
        {
            GlobalLog.LogStatus("----CurrentStateInvalidated Fired----");

            if (((Clock)sender).CurrentState == ClockState.Filling)
            {
                double finalValue = (double)_slider1.GetValue(Slider.ValueProperty);
                Clock clock = ((Clock)sender);
                clock.Controller.Remove();
                _slider1.Value = finalValue;  //Set to 'held' value, so thumb can be moved via UI.
            }
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
            _aTimer.Interval = TimeSpan.FromSeconds(3);
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
                UserInput.MouseLeftClickCenter(_slider1);
            }
            else if (_tickCount == 2)
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

            //NOTE: the animation itself is not verified.  Instead, the Value property is checked
            //after the mouse click to determine if the mouse click successfully moved the thumb.
            double actValue = (double)_slider1.GetValue(Slider.ValueProperty);
            
            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Value: < " + _toValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue < _toValue)
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
