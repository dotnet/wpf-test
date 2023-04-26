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
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>


    [Test(2, "Storyboards.LowLevelScenarios.Regressions", "NestedTimelines")]
    public class NestedTimelinesTest : XamlTest
    {
        #region Test case members

        private TextBox             _animatedElement;
        private DispatcherTimer     _aTimer = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          NestedTimelinesTest Constructor
        ******************************************************************************/
        public NestedTimelinesTest() : base(@"NestedTimelines.xaml")
        {
            InitializeSteps += new TestStep(GetElement);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          GetElement
        ******************************************************************************/
        /// <summary>
        /// Looks in the Markup for the Animated element.
        /// </summary>
        /// <returns>TestResult=Success if the element is found</returns>
        private TestResult GetElement()          
        {
            _animatedElement = (TextBox)RootElement.FindName("textbox1");

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
        /// <returns>Returns TestResult</returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,4500);
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
        /// <returns>TestResult</returns>
        TestResult Verify()
        {
            WaitForSignal("AnimationDone");
            
            double actValue = _animatedElement.Height;
            double expValue = 50d;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value : " + expValue);
            GlobalLog.LogEvidence("Actual Value   : " + actValue);
            
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
