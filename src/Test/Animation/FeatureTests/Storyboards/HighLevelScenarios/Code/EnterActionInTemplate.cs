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
    /// <area>Storyboards.HighLevelScenarios.Regressions</area>

    // [DISABLE WHILE PORTING]
    // [Test(2, "Storyboards.HighLevelScenarios.Regressions", "EnterActionInTemplateTest")]
    public class EnterActionInTemplateTest : XamlTest
    {

        #region Test case members
        private DispatcherTimer     _aTimer      = null;
        #endregion

        #region Constructor

        /******************************************************************************
        * Function:          EnterActionInTemplateTest Constructor
        ******************************************************************************/
        public EnterActionInTemplateTest() : base(@"EnterActionInTemplate.xaml")
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
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
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
            _aTimer.Stop();

            Signal("AnimationDone", TestResult.Pass);
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
            //NOTE: this test verifies the effect of Property Triggers that are set -BEFORE-
            //the ControlTemplate is applied.

            WaitForSignal("AnimationDone");

            float tolerance = 0.10f;
            int x1          = 50;
            int y1          = 50;
            int x2          = 150;
            int y2          = 50;
            int x3          = 295;
            int y3          = 50;

            Window.Title = "Animation";
            VisualVerifier verifier = new VisualVerifier();
            verifier.InitRender(Window);

            Color actColor1 = verifier.getColorAtPoint(x1,y1);
            Color expColor1 = Colors.White;
            Color actColor2 = verifier.getColorAtPoint(x2,y2);
            Color expColor2 = Colors.Red;
            Color actColor3 = verifier.getColorAtPoint(x3,y3);
            Color expColor3 = Colors.Blue;

            bool b1 = AnimationUtilities.CompareColors(expColor1, actColor1, tolerance);
            bool b2 = AnimationUtilities.CompareColors(expColor2, actColor2, tolerance);
            bool b3 = AnimationUtilities.CompareColors(expColor3, actColor3, tolerance);

            GlobalLog.LogEvidence("---------- Result at (" + x1 + "," + y1 + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actColor1.ToString());
            GlobalLog.LogEvidence(" Expected : " + expColor1.ToString());
            GlobalLog.LogEvidence("---------- Result at (" + x2 + "," + y2 + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actColor2.ToString());
            GlobalLog.LogEvidence(" Expected : " + expColor2.ToString());
            GlobalLog.LogEvidence("---------- Result at (" + x3 + "," + y3 + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actColor3.ToString());
            GlobalLog.LogEvidence(" Expected : " + expColor3.ToString());
            GlobalLog.LogEvidence("---------------------------------------------------");
            GlobalLog.LogEvidence("----FINAL RESULTS: " + b1 + "/" + b2 + "/" + b3);
            GlobalLog.LogEvidence("---------------------------------------------------");
            
            if (b1 && b2 && b3)
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
