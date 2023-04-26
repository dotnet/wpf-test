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
using System.Windows.Input;
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


    [Test(2, "Animation.HighLevelScenarios.Regressions", "RenderTransformValueTest")]
    public class RenderTransformValueTest : XamlTest
    {

        #region Test case members

        private Button              _animatedElement;
        private Button              _otherElement;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        private double              _actValue1;
        private double              _actValue2;
        private double              _actValue3;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          RenderTransformValueTest Constructor
        ******************************************************************************/
        public RenderTransformValueTest() : base(@"RenderTransformValue.xaml")
        {
            InitializeSteps += new TestStep(Animate);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          Animate
        ******************************************************************************/
        /// <summary>
        /// Looks in the Markup for the element that is animated.
        /// </summary>
        /// <returns>TestResult=Success if the animated element is found</returns>
        private TestResult Animate()          
        {
            _animatedElement = (Button)RootElement.FindName("button1");
            _otherElement = (Button)RootElement.FindName("button2");
            _animatedElement.Click += new RoutedEventHandler(OnClick);
            _otherElement.Click += new RoutedEventHandler(OnClick);

            if (_animatedElement == null)
            {
                GlobalLog.LogEvidence("The animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The animated element was found.");
                return TestResult.Pass;
            }
        }
        
        /******************************************************************************
        * Function:          OnMouseOver
        ******************************************************************************/
        /// <summary>
        /// Event handler fired when the button is clicked with the left mouse button.
        /// </summary>
        /// <returns></returns>
        private void OnClick(object sender, RoutedEventArgs e)
        {
            GlobalLog.LogStatus("---Button Clicked---");

            DoubleAnimation scaleAnimation = new DoubleAnimation();
            if (_animatedElement.IsMouseOver == true)
            {
                scaleAnimation.From = 1.0d;
                scaleAnimation.To   = 2.0d;
            }
            else
            {
                scaleAnimation.From = 2.0d;
                scaleAnimation.To   = 1.0d;
            }
            scaleAnimation.BeginTime    = TimeSpan.FromSeconds(0);
            scaleAnimation.Duration     = new TimeSpan(0,0,0,0,250);

            ScaleTransform scale = new ScaleTransform(1.0, 1.0, 0, 30);

            _animatedElement.RenderTransform = scale;
            
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult=Success</returns>
        TestResult StartTimer()
        {
            GlobalLog.LogStatus("---The animated element was found---");

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
            _aTimer.Start();
            GlobalLog.LogStatus("---DispatcherTimer Started---");

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
            
            switch (_tickCount)
            {
                case 1:
                    UserInput.MouseLeftClickCenter(_animatedElement);
                    break;
                case 2:
                    _actValue1 = CheckValue();  //Check value resulting from previous move.
                    break;
                case 3:
                    UserInput.MouseLeftClickCenter(_otherElement);
                    break;
                case 4:
                    UserInput.MouseLeftClickCenter(_animatedElement);
                    break;
                case 5:
                    _actValue2 = CheckValue();  //Check value resulting from previous move.
                    break;
                case 6:
                    UserInput.MouseLeftClickCenter(_otherElement);
                    break;
                case 7:
                    UserInput.MouseLeftClickCenter(_animatedElement);
                    break;
                case 8:
                    _actValue3 = CheckValue();  //Check value resulting from previous move.
                    break;
                case 9:
                    UserInput.MouseLeftClickCenter(_otherElement);
                    break;
                default:
                    Signal("AnimationDone", TestResult.Pass);
                    _aTimer.Stop();
                    break;
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

            double expValue = 2d;
            
            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Value [1st Animation]: " + expValue);
            GlobalLog.LogEvidence("Actual Value   [1st Animation]: " + _actValue1);
            GlobalLog.LogEvidence("Expected Value [2nd Animation]: " + expValue);
            GlobalLog.LogEvidence("Actual Value   [2nd Animation]: " + _actValue2);
            GlobalLog.LogEvidence("Expected Value [3rd Animation]: " + expValue);
            GlobalLog.LogEvidence("Actual Value   [3rd Animation]: " + _actValue3);
            
            if (_actValue1 == expValue && _actValue2 == expValue && _actValue3 == expValue)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          CheckValue
        ******************************************************************************/
        /// <summary>
        /// Returns the value of the Animated dp.
        /// </summary>
        /// <returns></returns>
        private double CheckValue()
        {
            if (_animatedElement.RenderTransform != null)
            {
                ScaleTransform currentScaleTransform = _animatedElement.RenderTransform as ScaleTransform;
                if (currentScaleTransform != null)
                {
                    return currentScaleTransform.ScaleX;
                }
            }

            return 0;
        }

        #endregion
    }
}
