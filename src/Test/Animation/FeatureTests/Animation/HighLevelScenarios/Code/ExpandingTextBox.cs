// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/****************  Animation Integration Test *****************
*    Major Actions:
*       (a) Create a new window containing a TextBox.
*       (b) Create a Width animation on the TextBox, to be generated via UIAutomation KeyPress events.
*       (d) Use a DispatcherTimer to invoke KeyPress actions, to trigger the animation.
*    Pass Conditions:
*       After the animations are done:
*           (a) GetCurrentValue on the AnimationClock returns the correct value.
*           (b) The actual Width of the TextBox is correct.
*    How verified:
*       The result of the comparisons between actual and expected values is passed to TestResult.
*
*  Framework:        A CLR executable is created.
*  Area:             Animation/Timing
*  Dependencies:     TestRuntime.dll
*  Support Files:
**********************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Documents;
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
    /// <area>Animation.HighLevelScenarios</area>
    /// <priority>2</priority>
    /// <description>
    /// Scenario Test: Animate the width of a TextBox whenever a key is pressed.
    /// </description>
    /// </summary>
    [Test(2, "Animation.HighLevelScenarios", "ExpandingTextBoxTest")]
    public class ExpandingTextBoxTest : WindowTest
    {
        #region Test case members

        private DispatcherTimer             _aTimer;
        private AnimationClock              _clock1;
        private TextBox                     _textBox1;
        private double                      _initialWidth        = 20d;
        private double                      _byValue             = 10d;
        private double                      _actValue1           = 0;
        private TimeSpan                    _TIMER_INTERVAL      = TimeSpan.FromMilliseconds(1000);
        private int                         _dispatcherTickCount = 0;
        private int                         _keyPressCount       = 0;
        private bool                        _testPassed          = false;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ExpandingTextBoxTest Constructor
        ******************************************************************************/
        public ExpandingTextBoxTest()
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
        /// <returns>Returns TestResult.Pass</returns>
        TestResult CreateTree()
        {
            Window.Width           = 650d;
            Window.Height          = 350d;
            Window.Left            = 5d;
            Window.Top             = 5d;

            Canvas body  = new Canvas();
            Window.Content = body;
            body.Width                = 650;
            body.Height               = 350;
            body.Background           = Brushes.OrangeRed;

            _textBox1  = new TextBox();
            body.Children.Add(_textBox1);
            _textBox1.Width               = 20d;
            _textBox1.Height              = 50d;
            _textBox1.Background          = Brushes.LightYellow;
            _textBox1.FontSize            = 24d;
            Canvas.SetTop  (_textBox1, 150d);
            Canvas.SetLeft (_textBox1, 10d); 

            Window.Activate();

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
            //Use a DispatcherTimer to initiate an Animation.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = _TIMER_INTERVAL;
            _aTimer.Start();
            
            GlobalLog.LogStatus("---DispatcherTimer Started---");
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Fires when the DispatcherTimer advances.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)          
        {
            _dispatcherTickCount++;
            GlobalLog.LogStatus("---Tick #" + _dispatcherTickCount);

            if (_dispatcherTickCount == 1)
            {
                _textBox1.KeyDown             += new KeyEventHandler(OnKeyDown);

                DoubleAnimation anim1 = new DoubleAnimation();
                anim1.By                        = _byValue;
                anim1.BeginTime                 = null;
                anim1.Duration                  = new Duration(TimeSpan.FromMilliseconds(200));
                anim1.CurrentStateInvalidated   += new EventHandler(OnCurrentStateInvalidated);

                _clock1 = anim1.CreateClock();
                _textBox1.ApplyAnimationClock(TextBox.WidthProperty, _clock1);

                GlobalLog.LogStatus("---Animation Created---");
            }
            else if (_dispatcherTickCount > 1 && _dispatcherTickCount < 7)
            {
                _keyPressCount++;
                _textBox1.Focus();
                UserInput.KeyPress("A", true);
                UserInput.KeyPress("A", false);
            }
            else
            {
                _aTimer.Stop();
                _clock1.Controller.Stop();
            }
        }

        /******************************************************************************
        * Function:          OnKeyDown
        ******************************************************************************/
        /// <summary>
        /// Fires when a Key is pressed.
        /// </summary>
        /// <returns></returns>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            GlobalLog.LogStatus("---KeyDown---Width--" + _textBox1.Width);
            double currentValue = _textBox1.Width;
            _textBox1.Width = currentValue;

            _clock1.Controller.Begin();  //Start a new animation for every key press.
        }
          
        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        /// <summary>
        /// Fires when the Animation's State is invalidated.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            AnimationClock AC = (AnimationClock)((Clock)sender);
            GlobalLog.LogStatus("---CurrentStateInvalidated--" + AC.CurrentState);
            
            if (((Clock)sender).CurrentState != ClockState.Stopped)
            {
                //Obtaining GetCurrentValue when the clock is still active.
                _actValue1 = (double)AC.GetCurrentValue(0d,0d);
            }
            else
            {
                double expValue1   = _byValue;
                GlobalLog.LogEvidence("-----GetCurrentValue: " + _actValue1 + " --- Expected: " + expValue1);

                double expValue2   = _initialWidth + ((_keyPressCount-1) * _byValue);
                double actValue2   = _textBox1.Width;
                GlobalLog.LogEvidence("-----TextBox Width:   " + actValue2 + " --- Expected: " + expValue2);

                _testPassed = (expValue1 == _actValue1 && expValue2 == actValue2);
                
                if (_testPassed)
                {
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    Signal("TestFinished", TestResult.Fail);
                }
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult object, indicating Pass or Fail</returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }

        #endregion
    }
}
