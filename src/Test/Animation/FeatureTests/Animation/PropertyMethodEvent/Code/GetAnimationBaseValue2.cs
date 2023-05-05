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
    /// <area>Animation.PropertyMethodEvent.Regressions</area>
    /// <priority>2</priority>

    [Test(2, "Animation.PropertyMethodEvent.Regressions", "GetAnimationBaseValue2Test")]
    public class GetAnimationBaseValue2Test : WindowTest
    {
        #region Test case members

        private string                          _inputString     = "";
        private TextBox                         _textbox1;
        private DoubleAnimation                 _animDouble;
        private AnimationClock                  _clock;
        private double                          _expBaseValue;
        private double                          _baseValue       = 100;
        private double                          _fromValue       = 200;
        private double                          _toValue         = 300;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        private bool                            _passed          = true;
        
        #endregion


        #region Constructor
        
        [Variation("BaseValueSet", Priority=0)]
        [Variation("NoBaseValueSet")]
        
        /******************************************************************************
        * Function:          GetAnimationBaseValue2Test Constructor
        ******************************************************************************/
        public GetAnimationBaseValue2Test(string testValue)
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
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.SpringGreen;
            body.Height     = 300d;
            body.Width      = 300d;
            
            _textbox1 = new TextBox();
            body.Children.Add(_textbox1);
            _textbox1.Text               = "Avalon!";
            _textbox1.Height             = 75d;
            
            if (_inputString == "BaseValueSet")
            {
                _textbox1.Width = _baseValue;
                _expBaseValue = _baseValue;
            }
            else
            {
                _expBaseValue = 0d;
                _expBaseValue = Double.NaN;
            }

            _animDouble = new DoubleAnimation();
            _animDouble.From             = _fromValue;
            _animDouble.To               = _toValue;
            _animDouble.BeginTime        = TimeSpan.FromMilliseconds(0);
            _animDouble.Duration         = new Duration(TimeSpan.FromMilliseconds(2000));
            _animDouble.CurrentStateInvalidated += new EventHandler(OnCurrentState);

            Window.Content = body;
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          OnCurrentState
        ******************************************************************************/
        /// <summary>
        /// Fires when the the CurrentStateInvalidated event fires on the AnimationClock.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentState(object sender, EventArgs args)
        {
            GlobalLog.LogStatus("----CurrentStateInvalidated Fired----");
            
            double actBaseValue = (double)_textbox1.GetAnimationBaseValue(TextBox.WidthProperty);
           
            GlobalLog.LogEvidence("Expected Base Value:  " + _expBaseValue);
            GlobalLog.LogEvidence("Actual Base Value:    " + actBaseValue);
            
            
            if (_inputString == "BaseValueSet")
            {
                if (actBaseValue != _expBaseValue)
                {
                    _passed = false;
                }
            }
            else if (_inputString == "NoBaseValueSet")
            {
                if (Double.IsNaN(actBaseValue) != true)
                {
                    _passed = false;
                }
            }
            else
            {
                GlobalLog.LogEvidence("FAIL!!! Incorrect inputString");
                _passed = false;
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
                _clock = _animDouble.CreateClock();
                _textbox1.ApplyAnimationClock(FrameworkElement.WidthProperty, _clock);
                _clock.Controller.Begin();

            }
            else if (_tickCount == 2)
            {
                _clock.Controller.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 3)
            {
                _clock.Controller.Pause();
            }
            else if (_tickCount == 4)
            {
                _clock.Controller.Resume();
            }
            else if (_tickCount == 5)
            {
                _clock.Controller.SkipToFill();
            }
            else if (_tickCount == 6)
            {
                _clock.Controller.Stop();
            }
            else if (_tickCount == 7)
            {
                _clock.Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(500), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 8)
            {
                _clock.Controller.Remove();
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
            
            if (_passed)
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
