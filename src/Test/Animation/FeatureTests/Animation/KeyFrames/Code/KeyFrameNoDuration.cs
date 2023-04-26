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
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.KeyFrames.Regressions</area>

    [Test(2, "Animation.KeyFrames.Regressions", "KeyFrameNoDurationTest")]
    public class KeyFrameNoDurationTest : XamlTest
    {

        #region Test case members

        private TextBox             _animatedElement;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          KeyFrameNoDurationTest Constructor
        ******************************************************************************/
        public KeyFrameNoDurationTest() : base(@"KeyFrameNoDuration.xaml")
        {
            InitializeSteps += new TestStep(GetElement);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        
        /******************************************************************************
        * Function:          GetElement
        ******************************************************************************/
        /// <summary>
        /// Retrieves the animated element from the markup, and then starts a Timer to control the
        /// timing of the verification.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult GetElement()
        {
            _animatedElement = (TextBox)AnimationUtilities.FindElement(RootElement, "textbox1");
            
            if (_animatedElement == null)
            {
                GlobalLog.LogEvidence("The animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The animated element was found.");
                
                _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _aTimer.Tick += new EventHandler(OnTick);
                _aTimer.Interval = new TimeSpan(0,0,0,0,1500);
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

            bool actValue = (bool)_animatedElement.GetValue(TextBox.IsEnabledProperty);
            bool expValue = false;

            EventTrigger eventTrigger = (EventTrigger)RootElement.Triggers[0];
            BeginStoryboard beginStoryboard = (BeginStoryboard)eventTrigger.Actions[0];
            Storyboard storyboard = (Storyboard)beginStoryboard.Storyboard;
            Duration actStoryDuration = storyboard.Duration;
            BooleanAnimationUsingKeyFrames anim = (BooleanAnimationUsingKeyFrames)storyboard.Children[0];
            Duration actAnimDuration = anim.Duration;

            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            Duration expDuration = Duration.Automatic;
            GlobalLog.LogEvidence("Expected Duration:              " + expDuration);
            GlobalLog.LogEvidence("Actual Duration(Storyboard):    " + actStoryDuration);
            GlobalLog.LogEvidence("Actual Duration(Animation):     " + actAnimDuration);
            
            if (actValue == expValue && actStoryDuration == expDuration && actAnimDuration == expDuration)
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
