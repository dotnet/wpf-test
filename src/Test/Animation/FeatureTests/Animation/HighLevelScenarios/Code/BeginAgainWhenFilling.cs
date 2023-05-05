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
using System.Windows.Shapes;
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

    [Test(2, "Animation.HighLevelScenarios.Regressions", "BeginAgainWhenFillingTest")]
    public class BeginAgainWhenFillingTest : XamlTest
    {

        #region Test case members

        private Button              _animatedElement;
        private TextBox             _focusElement;
        private Storyboard          _story1;
        private Storyboard          _story2;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          BeginAgainWhenFillingTest Constructor
        ******************************************************************************/
        public BeginAgainWhenFillingTest() : base(@"BeginAgainWhenFilling.xaml")
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
        /// Retrieves the animated elements from the markups.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("----Animate----");

            _animatedElement = (Button)AnimationUtilities.FindElement(RootElement,  "button1");
            _focusElement    = (TextBox)AnimationUtilities.FindElement(RootElement, "textbox1");


            if (_animatedElement == null || _focusElement == null)
            {
                GlobalLog.LogEvidence("The animated and/or focus element was not found.");
                return TestResult.Fail;
            }
            else
            {
                _animatedElement.MouseEnter += new MouseEventHandler(OnMouseEnter);
                _animatedElement.MouseLeave += new MouseEventHandler(OnMouseLeave);

                _story1 = (Storyboard)RootElement.FindResource("story1");
                _story2 = (Storyboard)RootElement.FindResource("story2");

                if (_story1 == null || _story2 == null)
                {
                    GlobalLog.LogEvidence("A Storyboard was not found.");
                    return TestResult.Fail;
                }
                else
                {
                    return TestResult.Pass;
                }
            }
        }

        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult StartTimer()
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
                UserInput.MouseMove(_animatedElement,20,20);
            }
            else if (_tickCount == 2)
            {
                UserInput.MouseLeftClickCenter(_focusElement);
            }
            else if (_tickCount == 3)
            {
                UserInput.MouseMove(_animatedElement,20,20);
            }
            else
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
            }
        }

        /******************************************************************************
        * Function:          OnMouseEnter
        ******************************************************************************/
        private void OnMouseEnter(object sender, MouseEventArgs args)
        {           
            _focusElement.Text += "E";
            _story2.Stop(_animatedElement);
            _story1.Begin(_animatedElement, true);
        }

        /******************************************************************************
        * Function:          OnMouseLeave
        ******************************************************************************/
        private void OnMouseLeave(object sender, MouseEventArgs args)
        { 
            _focusElement.Text += "L";
            _story1.Stop(_animatedElement);
            _story2.Begin(_animatedElement, true);
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

            double actValue = (double)_animatedElement.GetValue(Button.WidthProperty);
            double expValue = 150d;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value  : " + actValue);
            
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
