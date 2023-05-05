// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
    /// <area>Storyboards.HighLevelScenarios.Regressions</area>

    [Test(2, "Storyboards.HighLevelScenarios.Regressions", "MouseEnterLeaveTest")]
    public class MouseEnterLeaveTest : XamlTest
    {

        #region Test case members

        private Page                _page                = null;
        private Button              _button3             = null;
        private Button              _button4             = null;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          MouseEnterLeaveTest Constructor
        ******************************************************************************/
        public MouseEnterLeaveTest() : base(@"MouseEnterLeave.xaml")
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
            _page    = (Page)AnimationUtilities.FindElement(RootElement, "PageElement");
            _button3 = (Button)AnimationUtilities.FindElement(RootElement, "B3");
            _button4 = (Button)AnimationUtilities.FindElement(RootElement, "B4");
            
            if (_button3 == null || _button4 == null)
            {
                GlobalLog.LogEvidence("At least one element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The elements were found.");
                
                _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _aTimer.Tick += new EventHandler(OnTick);
                _aTimer.Interval = new TimeSpan(0,0,0,0,500);
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
                UserInput.MouseLeftClickCenter(_button3);
            }
            else if (_tickCount == 2)
            {
                UserInput.MouseLeftClickCenter(_button4);
            }
            else if (_tickCount == 3)
            {
                //Finish the test and verify the results at a point when the B4 animation has
                //started, but the B3 animation is still shrinking the button.  The test will
                //pass if button B4 has begun the expand.
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

            ScaleTransform scaleTransform = (ScaleTransform)_button4.RenderTransform;
            double actValue = scaleTransform.ScaleX;

            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            GlobalLog.LogEvidence("Expected Value: > 1");

            if (actValue > 1d)
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
