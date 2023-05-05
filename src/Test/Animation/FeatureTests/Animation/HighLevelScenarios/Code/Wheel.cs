// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/****************  Animation Integration Test *****************
*    Major Actions:
*       (a) Create a new window containing a TextBox and a ScrollViewer.
*       (b) Set up animations on the TextBox that are generated via UIAutomation MouseWheel events
*           on the ScrollViewer.
*       (d) Use a DispatcherTimer to invoke MouseWheel actions, to trigger the animations.
*    Pass Conditions:
*           (a) the actual values of the animated DPs are correct (captured via MouseWheel events)
*           (b) CurrentStateInvalidated fires the correct number of times.
*    How verified:
*       The result of the comparisons between actual and expected values is passed to
*       GlobalLog.LogStatus.
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
    /// Scenario Test: Verify Animation via the MouseWheel event
    /// </description>
    /// </summary>
    [Test(2, "Animation.HighLevelScenarios", "WheelTest")]
    public class WheelTest : WindowTest
    {
        #region Test case members
        private DispatcherTimer             _aTimer;
        private int                         _dispatcherTickCount     = 0;
        private AnimationClock              _clock1;
        private TextBox                     _textBox1;
        private ScrollViewer                _scrollViewer;
        private ScaleTransform              _ST;
        private DoubleAnimation             _anim2;
        private double                      _initialScale            = 1d;
        private double                      _initialLeft             = 0d;
        private double                      _scaleByValue            = 0.5d;
        private double                      _leftByValue             = 50d;
        private double                      _mouseWheelFirings       = 0;
        private double                      _actualFirings           = 0;
        private double                      _actualScaleX            = 0;
        private double                      _actualScaleY            = 0;
        private double                      _actualLeft              = 0;
        private Duration                    _DURATION_TIME           = new Duration(TimeSpan.FromMilliseconds(500));
        private TimeSpan                    _TIMER_INTERVAL          = new TimeSpan(0,0,0,0,1000);
        private bool                        _testPassed              = false;
        private StringBuilder               _results                 = new StringBuilder();
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          WheelTest Constructor
        ******************************************************************************/
        public WheelTest()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(CreateAnimation);
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
            Window.Title            = "Animation Scenario";
            Window.Width            = 800d;
            Window.Height           = 450d;
            Window.Left             = 5d;
            Window.Top              = 5d;
            Window.ContentRendered  += new EventHandler(OnContentRendered);

            Canvas body  = new Canvas();
            Window.Content = body;
            body.Width                = 800d;
            body.Height               = 450d;
            body.Background           = Brushes.OrangeRed;

            _textBox1  = new TextBox();
            body.Children.Add(_textBox1);
            _textBox1.Width               = 100d;
            _textBox1.Height              = 50d;
            Canvas.SetTop  (_textBox1, 100d);
            Canvas.SetLeft (_textBox1, _initialLeft); 
            _textBox1.Background          = Brushes.LightYellow;
            _textBox1.TextWrapping        = TextWrapping.Wrap;
            _textBox1.AcceptsReturn       = true;
            _textBox1.PreviewMouseWheel  += new MouseWheelEventHandler(OnMouseWheel);
            for (int i=0; i<20; i++)
            {
                _textBox1.Text += "When in the Course of human events,...\n";
            }

            _scrollViewer = new ScrollViewer();
            _scrollViewer.Background = Brushes.LemonChiffon;
            Canvas.SetTop  (_scrollViewer, 0d);
            Canvas.SetLeft (_scrollViewer, 600d); 
            _scrollViewer.Width = 150d;
            _scrollViewer.Height = 150d;           
            for (int i=0; i<20; i++)
            {
                _scrollViewer.Content += "When in the Course of human events,...\n";
            }

            _scrollViewer.PreviewMouseWheel   += new MouseWheelEventHandler(OnMouseWheel);
            body.Children.Add(_scrollViewer);

            Window.Activate();

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult CreateAnimation()
        {
            DoubleAnimation anim1 = new DoubleAnimation();
            anim1.BeginTime                 = null;
            anim1.Duration                  = _DURATION_TIME;
            anim1.By                        = _leftByValue;
            anim1.CurrentStateInvalidated   += new EventHandler(OnCurrentStateInvalidated);

            _clock1 = anim1.CreateClock();
            _textBox1.ApplyAnimationClock(Canvas.LeftProperty, _clock1);

            _anim2 = new DoubleAnimation();
            _anim2.BeginTime                 = TimeSpan.FromMilliseconds(10);
            _anim2.Duration                  = _DURATION_TIME;
            _anim2.By                        = _scaleByValue;
            _anim2.CurrentStateInvalidated   += new EventHandler(OnCurrentStateInvalidated);

            _ST = new ScaleTransform();
            _ST.ScaleX  = _initialScale;
            _ST.ScaleY  = _initialScale;

            _textBox1.RenderTransform = _ST;
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// OnContentRendered:  fires when the page content has finished rendering.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            _results.Append("----ContentRendered Fired----\n");
            
            _scrollViewer.Focus();
            
            //Use a DispatcherTimer to move the mouse wheel.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = _TIMER_INTERVAL;
            _aTimer.Start();
        }
        
        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// OnTick:  fires when the DispatcherTimer advances.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)          
        {
            _dispatcherTickCount++;
            _results.Append("------------------------------------------Tick #" + _dispatcherTickCount + "\n");
            
            if (_dispatcherTickCount < 6)
            {
                UserInput.MouseWheel(_scrollViewer, 10, 20, 5);
            }
            else
            {
                _aTimer.Stop();
                _clock1.Controller.Stop();
                _ST.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                _ST.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                FinishTest();
            }
        }
          
        /******************************************************************************
        * Function:          OnMouseWheel
        ******************************************************************************/
        /// <summary>
        /// OnMouseWheel:  invoked when the MouseWheel event fires.
        /// </summary>
        /// <returns></returns>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _mouseWheelFirings++;
            _results.Append("---MouseWheel--- #" + _mouseWheelFirings + "\n");
            
            //Capture results of the previous animation.
            _actualLeft = Canvas.GetLeft(_textBox1);
            Canvas.SetLeft(_textBox1, _actualLeft);
            _results.Append("---Left--- : " + _actualLeft + "\n");

            _actualScaleX = _ST.ScaleX;
            _results.Append("---ScaleX---: " + _actualScaleX + "\n");
            _actualScaleY = _ST.ScaleY;
            _results.Append("---ScaleY---: " + _actualScaleY + "\n");
            
            //Start the Animations each time the wheel is turned.
            _clock1.Controller.Begin();
            _ST.BeginAnimation(ScaleTransform.ScaleXProperty, _anim2);
            _ST.BeginAnimation(ScaleTransform.ScaleYProperty, _anim2);
        }
        
        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated:  invoked when the CurrentStateInvalidated event fires.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            //results.Append("---OnCurrentStateInvalidated---" + ((Clock)sender).CurrentState) + "\n");
            _actualFirings++;
        }
        
        /******************************************************************************
        * Function:          FinishTest
        ******************************************************************************/
        /// <summary>
        /// FinishTest:  verifies the Animation.
        /// </summary>
        /// <returns></returns>
        private void FinishTest()
        {
            GlobalLog.LogStatus(_results.ToString());

            //Verify the results of the Animations (note: the very last animation is not checked).
            double expectedLeft     = _initialLeft  + (_leftByValue  * (_mouseWheelFirings - 1));
            double expectedScaleX   = _initialScale + (_scaleByValue * (_mouseWheelFirings - 1));
            double expectedScaleY   = _initialScale + (_scaleByValue * (_mouseWheelFirings - 1));
            double expectedFirings  = 6.0 * _mouseWheelFirings;
            
            GlobalLog.LogEvidence("--------RESULTS--------------------------------");
            GlobalLog.LogEvidence("---Left:   Actual:      " + _actualLeft +    " / Expected: " + expectedLeft);
            GlobalLog.LogEvidence("---ScaleX: Actual:      " + _actualScaleX +  " / Expected: " + expectedScaleX);
            GlobalLog.LogEvidence("---ScaleY: Actual:      " + _actualScaleY +  " / Expected: " + expectedScaleY);
            GlobalLog.LogEvidence("---Event Firing: Actual " + _actualFirings + " / Expected: " + expectedFirings);
            GlobalLog.LogEvidence("-----------------------------------------------");
            
            if (_actualLeft == expectedLeft
                && _actualScaleX  == expectedScaleX
                && _actualScaleY  == expectedScaleY
                && _actualFirings == expectedFirings)
            {
                _testPassed = true;
            }

            Signal("AnimationDone", TestResult.Pass);
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
            WaitForSignal("AnimationDone");
            
            if (_testPassed)
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
