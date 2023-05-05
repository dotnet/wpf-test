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

    [Test(2, "Animation.LowLevelScenarios.Regressions", "SeekAndStopTwiceTest")]
    public class SeekAndStopTwiceTest : WindowTest
    {
        #region Test case members

        private Button                          _button1;
        private DoubleAnimation                 _animDouble;
        private AnimationClock                  _clock;
        private double                          _fromValue       = 180;
        private double                          _toValue         = 0;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          SeekAndStopTwiceTest Constructor
        ******************************************************************************/
        public SeekAndStopTwiceTest()
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
            body.Background = Brushes.PeachPuff;
            body.Height     = 300d;
            body.Width      = 300d;
            
            _button1 = new Button();
            body.Children.Add(_button1);
            _button1.Content      = "Avalon!";
            Canvas.SetTop  (_button1, 180d);
            Canvas.SetLeft (_button1, 100d);          

            _animDouble = new DoubleAnimation();
            _animDouble.From             = _fromValue;
            _animDouble.To               = _toValue;
            _animDouble.BeginTime        = TimeSpan.FromMilliseconds(0);
            _animDouble.Duration         = new Duration(TimeSpan.FromMilliseconds(8000));

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
                _clock = _animDouble.CreateClock();
                _button1.ApplyAnimationClock(Canvas.TopProperty, _clock);
                _clock.Controller.Begin();

            }
            else if (_tickCount == 2)
            {
                _clock.Controller.Seek(TimeSpan.FromMilliseconds(2000), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 3)
            {
                _clock.Controller.Stop();
            }
            else if (_tickCount == 4)
            {
                _clock.Controller.Stop();
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

            double actValue = (double)Canvas.GetTop(_button1);
            double expValue = _fromValue;
            
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
