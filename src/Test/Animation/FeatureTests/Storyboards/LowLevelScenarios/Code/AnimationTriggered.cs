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
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>
    /// <priority>2</priority>
    /// <description>
    /// Regression Test :  Property triggered animations don't reflect initial values.
    /// </description>
    /// </summary>
    [Test(2, "Storyboards.LowLevelScenarios.Regressions", "AnimationTriggeredTest")]
    public class AnimationTriggeredTest : XamlTest
    {

        #region Test case members

        private CheckBox            _animatedElement1;
        private CheckBox            _animatedElement2;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          AnimationReuseTest Constructor
        ******************************************************************************/
        public AnimationTriggeredTest() : base(@"AnimationTriggered.xaml")
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
        /// Retrieves the animated elements from the markup, and then begins the Animations.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult Animate()
        {
            _animatedElement1 = (CheckBox)RootElement.FindName("anim1");
            _animatedElement2 = (CheckBox)RootElement.FindName("anim2");
            
            if (_animatedElement1 == null || _animatedElement2 == null)
            {
                GlobalLog.LogEvidence("An animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The animated elements were found.");
                

                return TestResult.Pass;
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,4000);
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

            if ( _animatedElement1.Background.GetType() == typeof(LinearGradientBrush) )
            {
                GlobalLog.LogEvidence("The animation was not applied correctly on XP - Ignore");
                return TestResult.Ignore;
            }
            else
            {

                Color actColor = (Color)((SolidColorBrush)_animatedElement1.Background).Color;
                Color expColor = Colors.Yellow;
            
                GlobalLog.LogEvidence("----Verifying the Animation----");
                GlobalLog.LogEvidence("Expected NOT To  be: " + expColor);
                GlobalLog.LogEvidence("Actual Value       : " + actColor);

                bool b1 = AnimationUtilities.CompareColors(expColor, actColor, 0f);
            
                if (!b1)
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            } 
        }

        #endregion
    }
}
