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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "TextBoxAssertTest")]
    public class TextBoxAssertTest : XamlTest
    {
        #region Test case members

        private TextBox             _animatedElement;
        private DispatcherTimer     _aTimer = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          TextBoxAssertTest Constructor
        ******************************************************************************/
        public TextBoxAssertTest() : base(@"TextBoxAssert.xaml")
        {
            InitializeSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult</returns>
        TestResult StartTimer()
        {
            _animatedElement = (TextBox)AnimationUtilities.FindElement(RootElement, "textbox1");

            GlobalLog.LogStatus("----The animated element was found----");

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
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
                UserInput.MouseRightClickCenter(_animatedElement);
            }
            else if (_tickCount == 2)
            {
                UserInput.MouseLeftDown(_animatedElement);
            }
            else
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
            //This test case will automatically pass if no Assert occurs on a debug build. 
            //It will therefore automatically pass on  a retail build.
            WaitForSignal("AnimationDone");
            
            return TestResult.Pass;
        }

        #endregion
    }
}
