// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
using Microsoft.Test.Display;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>
    /// <priority>2</priority>
    /// <description>
    /// Verify an animation within a nested Storyboard.
    /// Scenario: Storyboards nested three levels deep.
    /// </description>
    /// </summary>

    // [DISABLE WHILE PORTING]
    // [Test(2, "Storyboards.LowLevelScenarios.Regressions", "NestedStoryboardsTest")]
    public class NestedStoryboardsTest : XamlTest
    {
        #region Test case members

        private VisualVerifier      _verifier;
        private Rectangle           _animatedElement;
        private DispatcherTimer     _aTimer = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          NestedStoryboardsTest Constructor
        ******************************************************************************/
        public NestedStoryboardsTest() : base(@"NestedStoryboards.xaml")
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Looks in the Markup for the Animated element.
        /// </summary>
        /// <returns>TestResult=Success if the element is found</returns>
        private TestResult Initialize()          
        {
            Window.Title = "Animation";
            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            _animatedElement = (Rectangle)RootElement.FindName("rectangle1");

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,5000);
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
           * Function:          CheckColor
           ******************************************************************************/
        /// <summary>
        /// CheckColor: retrieve the color at the specified point and compare it to the expected color.
        /// </summary>
        private bool CheckColor()
        {
            bool passed         = true;
            int x1              = 255;
            int y1              = 75;

            // DPI adjustment
            x1 = (int)(Monitor.ConvertLogicalToScreen(Dimension.Width, x1));
            
            //At each Tick, check the color at the specified point.
            Color actualColor = _verifier.getColorAtPoint(x1, y1);
            Color expectedColor = Colors.MidnightBlue;
            
            float tolerance  = 0.001f;

            passed = AnimationUtilities.CompareColors(expectedColor, actualColor, tolerance);

            GlobalLog.LogEvidence("---------- Result at (" + x1 + "," + y1 + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actualColor.ToString());
            GlobalLog.LogEvidence(" Expected : " + expectedColor.ToString());

            return passed;
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

            double actValue1 = _animatedElement.StrokeThickness;
            double expValue1 = 10d;

            bool colorPassed = CheckColor();

            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Value : " + expValue1);
            GlobalLog.LogEvidence("Actual Value   : " + actValue1);
            GlobalLog.LogEvidence("ColorCheck     : " + colorPassed);

            if ((actValue1 == expValue1) && colorPassed)
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
