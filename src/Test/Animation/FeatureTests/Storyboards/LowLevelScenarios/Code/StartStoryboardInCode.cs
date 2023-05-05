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
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>


    [Test(2, "Storyboards.LowLevelScenarios.Regressions", "StartStoryboardInCodeTest")]
    public class StartStoryboardInCodeTest : XamlTest
    {

        #region Test case members

        private Image               _animatedElement;
        private Storyboard          _storyboard          = null;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          StartStoryboardInCodeTest Constructor
        ******************************************************************************/
        public StartStoryboardInCodeTest() : base(@"StartStoryboardInCode.xaml")
        {
            InitializeSteps += new TestStep(GetStoryboard);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        
        /******************************************************************************
        * Function:          GetStoryboard
        ******************************************************************************/
        /// <summary>
        /// Retrieves the animated element from the markup, and then starts a Timer to control the
        /// timing of the verification.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult GetStoryboard()
        {
            _animatedElement = (Image)AnimationUtilities.FindElement(RootElement, "Animate");

            if (_animatedElement == null)
            {
                GlobalLog.LogEvidence("The animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                BeginStoryboard beginStoryboard = AnimationUtilities.GetBeginStoryboard(RootElement, _animatedElement);

                if (beginStoryboard == null)
                {
                    GlobalLog.LogEvidence("The BeginStoryboard was not found.");
                    return TestResult.Fail;
                }
                else
                {
                    _storyboard = (Storyboard)beginStoryboard.Storyboard;

                    if (_storyboard == null)
                    {
                        GlobalLog.LogEvidence("The storyboard was not found.");
                        return TestResult.Fail;
                    }
                    else
                    {
                        _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                        _aTimer.Tick += new EventHandler(OnTick);
                        _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
                        _aTimer.Start();
                        GlobalLog.LogStatus("----DispatcherTimer Started----");

                        //Must first reset BeginTime from the Null value specified in Markup.
                        _storyboard.BeginTime = TimeSpan.FromSeconds(0);

                        //Start the Storyboard.
                        _storyboard.Begin(_animatedElement);

                        return TestResult.Pass;
                    }
                }
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

            double actValue = (double)_animatedElement.GetValue(Image.OpacityProperty);
            double expValue = 0d;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value:   " + expValue);
            GlobalLog.LogEvidence("Actual Value:     " + actValue);
            
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
