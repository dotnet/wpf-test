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
    /// <area>Animation.LowLevelScenarios.Regressions</area>


    [Test(0, "Animation.LowLevelScenarios.Regressions", "ColorAndHeightTest")]
    public class ColorAndHeightTest : XamlTest
    {

        #region Test case members

        private Button              _animatedElement;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          ColorAndHeightTest Constructor
        ******************************************************************************/
        public ColorAndHeightTest() : base(@"ColorAndHeight.xaml")
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
            _animatedElement = (Button)RootElement.FindName("button1");
            
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
                _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
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

            SolidColorBrush brush = (SolidColorBrush)_animatedElement.GetValue(Button.BackgroundProperty);
            Color actValue1 = Color.FromRgb(brush.Color.R, brush.Color.G, brush.Color.B);
            Color expValue1 = Colors.Blue;

            double actValue2     = (double)_animatedElement.GetValue(Button.HeightProperty);
            double expValue2     = 200d;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value [Color]:  " + expValue1);
            GlobalLog.LogEvidence("Actual Value   [Color]:  " + actValue1);
            GlobalLog.LogEvidence("Expected Value [Height]: " + expValue2);
            GlobalLog.LogEvidence("Actual Value   [Height]: " + actValue2);
            
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
