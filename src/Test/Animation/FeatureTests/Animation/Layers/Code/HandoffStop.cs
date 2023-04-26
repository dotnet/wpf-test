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
    /// <area>Animation.Layers.Regressions</area>
    /// <priority>2</priority>

    [Test(2, "Animation.Layers.Regressions", "HandoffStopTest")]
    public class HandoffStopTest : XamlTest
    {

        #region Test case members

        private Button              _animatedElement;
        private Button              _focusElement;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          HandoffStopTest Constructor
        ******************************************************************************/
        public HandoffStopTest() : base(@"HandoffStop.xaml")
        {
            InitializeSteps += new TestStep(GetElements);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          GetElements
        ******************************************************************************/
        /// <summary>
        /// Looks in the Markup for elements.
        /// </summary>
        /// <returns>TestResult=Success if the elements are found</returns>
        private TestResult GetElements()          
        {
            _animatedElement = (Button)LogicalTreeHelper.FindLogicalNode((Grid)Window.Content,"button1");
            _focusElement    = (Button)AnimationUtilities.FindElement(RootElement, "button2");

            if (_animatedElement == null)
            {
                GlobalLog.LogEvidence("The animated element was not found.");
                return TestResult.Fail;
            }
            else if (_focusElement == null)
            {
                GlobalLog.LogEvidence("The focus element was not found.");
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
        /// <returns>Returns success</returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,500);
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
            else if (_tickCount == 9)
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

            SolidColorBrush brush = (SolidColorBrush)_animatedElement.GetValue(Button.BackgroundProperty);
            Color actValue = Color.FromRgb(brush.Color.R, brush.Color.G, brush.Color.B);
            
            Color expValue = Colors.Yellow;
            
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
