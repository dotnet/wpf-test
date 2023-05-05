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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(0, "Animation.HighLevelScenarios.Regressions", "BeginAgainWhenStoppedTest")]
    public class BeginAgainWhenStoppedTest : WindowTest
    {
        #region Test case members

        private Canvas              _canvas2         = null;
        private DependencyProperty  _dp              = Canvas.WidthProperty;
        private DoubleAnimation     _animDouble;
        private double              _baseValue       = 10d;
        private double              _fromValue       = 50d;
        private double              _toValue         = 200d;
        private int                 _animCount       = 0;
        private DispatcherTimer     _aTimer          = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          BeginAgainWhenStoppedTest Constructor
        ******************************************************************************/
        public BeginAgainWhenStoppedTest()
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
            Window.Width        = 250d;
            Window.Height       = 200d;

            Canvas body  = new Canvas();
            Window.Content = body;
            body.Background = Brushes.Plum;

            _animDouble = new DoubleAnimation(_fromValue, _toValue, TimeSpan.FromSeconds(1.0));
            _animDouble.FillBehavior = FillBehavior.Stop;
            _animDouble.CurrentStateInvalidated += new EventHandler(OnCurrentState);
            
            _canvas2 = new Canvas();
            body.Children.Add(_canvas2);
            _canvas2.Width       = _baseValue;
            _canvas2.Height      = 50d;
            _canvas2.Background  = Brushes.DarkMagenta;

            _canvas2.BeginAnimation(_dp, _animDouble);

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,4000);
            _aTimer.Start();
            
            GlobalLog.LogStatus("----DispatcherTimer Started----");
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnCurrentState
        ******************************************************************************/
        /// <summary>
        /// Invoked when the CurrentStateInvalidated event fires on the Animation.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private void OnCurrentState(object sender, EventArgs args)
        {
            GlobalLog.LogStatus("----CurrentStateInvalidated Fired----");
            _animCount++;
            
            if (_animCount < 4)
            {
                if (((Clock)sender).CurrentState == ClockState.Stopped)
                {
                    DoubleAnimation anim = new DoubleAnimation(_fromValue, _toValue, TimeSpan.FromSeconds(1.0));
                    anim.CurrentStateInvalidated += new EventHandler(OnCurrentState);

                    _canvas2.BeginAnimation(_dp, _animDouble);
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

            double actValue = (double)_canvas2.GetValue(_dp);
            double expValue = _baseValue;
            
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
