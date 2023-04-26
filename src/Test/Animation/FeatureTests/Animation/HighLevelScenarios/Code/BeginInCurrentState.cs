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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "BeginInCurrentStateTest")]
    public class BeginInCurrentStateTest : WindowTest
    {
        #region Test case members

        private Rectangle           _rectangle1;
        private double              _fromValue       = 0d;
        private double              _toValue         = 0.75d;
        private int                 _animCount       = 0;
        private DispatcherTimer     _aTimer          = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          BeginInCurrentStateTest Constructor
        ******************************************************************************/
        public BeginInCurrentStateTest()
        {
            InitializeSteps += new TestStep(CreateTree);
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
            body.Background = Brushes.Cornsilk;

            DoubleAnimation animation = new DoubleAnimation(_fromValue, _toValue, TimeSpan.FromSeconds(1.0));
            animation.CurrentStateInvalidated += new EventHandler(OnCurrentStateChanged);
            
            _rectangle1 = new Rectangle();
            body.Children.Add(_rectangle1);
            _rectangle1.Width    = 100;
            _rectangle1.Height   = 100;
            _rectangle1.Fill     = Brushes.DodgerBlue;
            _rectangle1.BeginAnimation(Rectangle.OpacityProperty, animation);
            
            //Start the Timer.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,4000);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnCurrentStateChanged
        ******************************************************************************/
        private void OnCurrentStateChanged(object sender, EventArgs args)
        {
            GlobalLog.LogStatus("----CurrentStateInvalidated Fired----");
            _animCount++;
            
            if (_animCount < 4)
            {
                if (((Clock)sender).CurrentState == ClockState.Filling)
                {
                    DoubleAnimation anim = new DoubleAnimation(_fromValue, _toValue, TimeSpan.FromSeconds(1.0));
                    anim.CurrentStateInvalidated += new EventHandler(OnCurrentStateChanged);

                    _rectangle1.BeginAnimation(Rectangle.OpacityProperty, anim);
                }
            }
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

            double actValue = (double)_rectangle1.GetValue(UIElement.OpacityProperty);
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + _toValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue == _toValue)
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
