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
using System.Windows.Documents;
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
    /// <area>Animation.LowLevelScenarios.Regressions</area>
    /// <priority>2</priority>


    [Test(2, "Animation.LowLevelScenarios.Regressions", "GlyphsBidiLevelTest")]
    public class GlyphsBidiLevelTest : XamlTest
    {

        #region Test case members

        private Glyphs              _animatedElement1;
        private Glyphs              _animatedElement2;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          GlyphsBidiLevelTest Constructor
        ******************************************************************************/
        public GlyphsBidiLevelTest() : base(@"GlyphsBidiLevel.xaml")
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
        /// Looks in the Markup for the Animated elements.
        /// </summary>
        /// <returns>TestResult=Success if the elements are found</returns>
        private TestResult GetElements()          
        {
            _animatedElement1 = (Glyphs)RootElement.FindName("glyphs1");
            _animatedElement2 = (Glyphs)RootElement.FindName("glyphs2");

            if (_animatedElement1 == null)
            {
                GlobalLog.LogEvidence("The first animated element was not found.");
                return TestResult.Fail;
            }
            else if (_animatedElement2 == null)
            {
                GlobalLog.LogEvidence("The second animated element was not found.");
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
        /// <returns>Returns success</returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
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
            
            Int32 actValue1 = (Int32)_animatedElement1.BidiLevel;
            Int32 expValue1 = 2;
            Int32 actValue2 = (Int32)_animatedElement2.BidiLevel;
            Int32 expValue2 = 1;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value 1 : " + expValue1);
            GlobalLog.LogEvidence("Actual Value 1   : " + actValue1);
            GlobalLog.LogEvidence("Expected Value 2 : " + expValue2);
            GlobalLog.LogEvidence("Actual Value 2   : " + actValue2);
            
            if (actValue1 == expValue1 && actValue2 == expValue2)
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
