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
    /// <area>Animation.Layers.Regressions</area>

    [Test(2, "Animation.Layers.Regressions", "CheckBoxHandoffTest")]
    public class CheckBoxHandoffTest : XamlTest
    {

        #region Test case members

        private Border              _animatedElement;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        private Color               _actValue1;
        private Color               _actValue2;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          CheckBoxHandoffTest Constructor
        ******************************************************************************/
        public CheckBoxHandoffTest() : base(@"CheckBoxHandoff.xaml")
        {
            InitializeSteps += new TestStep(GetAnimatedElement);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          GetAnimatedElement
        ******************************************************************************/
        /// <summary>
        /// Looks in the Markup for the element that is animated.
        /// </summary>
        /// <returns>TestResult=Success if the animated element is found</returns>
        private TestResult GetAnimatedElement()          
        {
            CheckBox checkBox = (CheckBox)LogicalTreeHelper.FindLogicalNode((Page)Window.Content,"AnimateTemplate");
            FrameworkElement fe = (FrameworkElement)checkBox.Template.FindName("border1", checkBox);
            _animatedElement = (Border)fe;

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
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult=Success</returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,750);
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
            
            switch (_tickCount)
            {
                case 1:
                    UserInput.MouseLeftClickCenter(_animatedElement);
                    break;
                case 2:
                    _actValue1 = GetActualFillColor(); //Get the color after the previous animation.
                    break;
                case 3:
                    UserInput.MouseLeftClickCenter(_animatedElement);
                    break;
                case 4:
                    _actValue2 = GetActualFillColor(); //Get the color after the previous animation.
                    break;
                case 5:
                    UserInput.MouseLeftClickCenter(_animatedElement);
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

            SolidColorBrush brush = (SolidColorBrush)_animatedElement.GetValue(Border.BackgroundProperty);
            Color actValue3 = GetActualFillColor();
            
            Color expValue1 = Colors.Red;
            Color expValue2 = Colors.Blue;
            Color expValue3 = Colors.White;
            
            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Value [1st Animation]: " + expValue1);
            GlobalLog.LogEvidence("Actual Value   [1st Animation]: " + _actValue1);
            GlobalLog.LogEvidence("Expected Value [2nd Animation]: " + expValue2);
            GlobalLog.LogEvidence("Actual Value   [2nd Animation]: " + _actValue2);
            GlobalLog.LogEvidence("Expected Value [3rd Animation]: " + expValue3);
            GlobalLog.LogEvidence("Actual Value   [3rd Animation]: " + actValue3);
            
            if (_actValue1 == expValue1 && _actValue2 == expValue2 && actValue3 == expValue3)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          GetActualFillColor
        ******************************************************************************/
        /// <summary>
        /// Determines the color of the animatedElement.
        /// </summary>
        /// <returns></returns>
        Color GetActualFillColor()
        {
            SolidColorBrush brush = (SolidColorBrush)_animatedElement.GetValue(CheckBox.BackgroundProperty);
            Color color = Color.FromRgb(brush.Color.R, brush.Color.G, brush.Color.B);
            return color;
        }

        #endregion
    }
}
