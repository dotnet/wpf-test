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
    /// <priority>0</priority>


    [Test(0, "Animation.LowLevelScenarios.Regressions", "PauseStoryChildrenTest")]
    public class PauseStoryChildrenTest : XamlTest
    {

        #region Test case members

        private Button              _beginElement;
        private Button              _pauseElement;
        private Canvas              _animatedElement;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          PauseStoryChildrenTest Constructor
        ******************************************************************************/
        public PauseStoryChildrenTest() : base(@"PauseStoryChildren.xaml")
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
            _beginElement    = (Button)AnimationUtilities.FindElement(RootElement, "Begin");
            _pauseElement    = (Button)AnimationUtilities.FindElement(RootElement, "Pause");
            _animatedElement = (Canvas)AnimationUtilities.FindElement(RootElement, "John");
            
            if (_beginElement == null)
            {
                GlobalLog.LogEvidence("The Begin button was not found.");
                return TestResult.Fail;
            }
            else if (_pauseElement == null)
            {
                GlobalLog.LogEvidence("The Pause button was not found.");
                return TestResult.Fail;
            }
            else if (_animatedElement == null)
            {
                GlobalLog.LogEvidence("The animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogStatus("The elements were found.");
                
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
                UserInput.MouseLeftClickCenter(_beginElement);
            }
            else if (_tickCount == 2)
            {
                UserInput.MouseLeftClickCenter(_pauseElement);
            }
            else if (_tickCount == 4)
            {
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

            double actValue = (double)_animatedElement.GetValue(Canvas.LeftProperty);
            double expValue = 200d;
            double tolerance = 50d;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue <= expValue+tolerance && actValue >= expValue-tolerance)
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
