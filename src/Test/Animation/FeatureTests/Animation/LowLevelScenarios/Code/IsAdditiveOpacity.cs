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
    /// <area>Animation.LowLevelScenarios.Regressions</area>
    /// <priority>2</priority>
    /// <description>

    [Test(2, "Animation.LowLevelScenarios.Regressions", "IsAdditiveOpacityTest")]
    public class IsAdditiveOpacityTest : XamlTest
    {

        #region Test case members

        private TextBlock           _animatedElement     = null;
        private Button              _focusElement        = null;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        private bool                _passed1             = false;
        private bool                _passed2             = false;
        private bool                _passed3             = false;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          IsAdditiveOpacityTest Constructor
        ******************************************************************************/
        public IsAdditiveOpacityTest() : base(@"IsAdditiveOpacity.xaml")
        {
            InitializeSteps += new TestStep(GetElements);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          GetElements
        ******************************************************************************/
        /// <summary>
        /// Retrieves the animated element from the markup, and then starts a Timer to control the
        /// timing of the verification.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult GetElements()
        {
            _animatedElement     = (TextBlock)AnimationUtilities.FindElement(RootElement, "AnimatedElement");
            _focusElement        = (Button)AnimationUtilities.FindElement(RootElement, "FocusElement");
            
            if (_animatedElement == null)
            {
                GlobalLog.LogEvidence("The animated Button was not found.");
                return TestResult.Fail;
            }
            else if (_focusElement == null)
            {
                GlobalLog.LogEvidence("The focus Button was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The elements were found.");
                
                _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _aTimer.Tick += new EventHandler(OnTick);
                _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
                _aTimer.Start();
                GlobalLog.LogStatus("----DispatcherTimer Started----");
                
                return TestResult.Pass;
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
            _tickCount++;
            
            if (_tickCount == 1)
            {
                UserInput.MouseLeftClickCenter(_animatedElement);
            }
            else if (_tickCount == 2)
            {
                UserInput.MouseLeftClickCenter(_focusElement);
            }
            else if (_tickCount == 3)
            {
                _passed1 = CompareValues("#1", 0.6d);
                UserInput.MouseLeftClickCenter(_animatedElement);
            }
            else if (_tickCount == 4)
            {
                UserInput.MouseLeftClickCenter(_focusElement);
            }
            else if (_tickCount == 5)
            {
                _passed2 = CompareValues("#2", 0.9d);
                UserInput.MouseLeftClickCenter(_animatedElement);
            }
            else if (_tickCount == 6)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
            }
        }

        /******************************************************************************
        * Function:          CompareValues
        ******************************************************************************/
        /// <summary>
        /// Compares actual vs. expected Opacity, and returns true or false.
        /// </summary>
        /// <returns></returns>
        private bool CompareValues(string testNo, double exp)
        {
            double tolerance = 0.05d;
            double act = (double)_animatedElement.GetValue(Button.OpacityProperty);
            
            GlobalLog.LogEvidence("----Verification -- " + testNo + "-----");
            GlobalLog.LogEvidence("Expected Value: " + exp);
            GlobalLog.LogEvidence("Actual Value:   " + act);
            
            if (act <= exp+tolerance && act >= exp-tolerance)
            {
                return true;
            }
            else
            {
                return false;
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

            _passed3 = CompareValues("#3", 1.2d);
            
            GlobalLog.LogEvidence("----FINAL RESULT: " + _passed1 + "/" + _passed2 + "/" + _passed3);

            if (_passed1 && _passed2 && _passed3)
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
