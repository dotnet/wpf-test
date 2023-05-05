// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
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

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.Regressions</area>
    /// <priority>2</priority>


    [Test(2, "Animation.PropertyMethodEvent.Regressions", "IsCumulativeFromTest")]
    public class IsCumulativeFromTest : XamlTest
    {

        #region Test case members

        private Canvas          _animatedElement0;
        private Canvas          _animatedElement1;
        private Canvas          _animatedElement2;
        private Canvas          _animatedElement3;
        private Canvas          _animatedElement4;
        private DispatcherTimer _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          IsCumulativeFromTest Constructor
        ******************************************************************************/
        public IsCumulativeFromTest() : base(@"IsCumulativeFrom.xaml")
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
            _animatedElement0 = (Canvas)RootElement.FindName("animatedCanvas0");
            _animatedElement1 = (Canvas)RootElement.FindName("animatedCanvas1");
            _animatedElement2 = (Canvas)RootElement.FindName("animatedCanvas2");
            _animatedElement3 = (Canvas)RootElement.FindName("animatedCanvas3");
            _animatedElement4 = (Canvas)RootElement.FindName("animatedCanvas4");
            
            if (_animatedElement0 == null
                || _animatedElement1 == null
                || _animatedElement2 == null
                || _animatedElement3 == null 
                || _animatedElement4 == null)
            {
                GlobalLog.LogEvidence("The animated elements were not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The animated elements were found.");

                _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _aTimer.Tick += new EventHandler(OnTick);
                _aTimer.Interval = new TimeSpan(0,0,0,0,3500);
                _aTimer.Start();
                GlobalLog.LogEvidence("----DispatcherTimer Started----");
                
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

            double actValue0 = (double)_animatedElement0.GetValue(Canvas.WidthProperty);
            double actValue1 = (double)_animatedElement1.GetValue(Canvas.WidthProperty);
            double actValue2 = (double)_animatedElement2.GetValue(Canvas.WidthProperty);
            double actValue3 = (double)_animatedElement3.GetValue(Canvas.WidthProperty);
            double actValue4 = (double)_animatedElement4.GetValue(Canvas.WidthProperty);
            double expValue = 150d;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value : " + expValue);
            GlobalLog.LogEvidence("Actual Value [Canvas 0] : " + actValue0);
            GlobalLog.LogEvidence("Actual Value [Canvas 1] : " + actValue1);
            GlobalLog.LogEvidence("Actual Value [Canvas 2] : " + actValue2);
            GlobalLog.LogEvidence("Actual Value [Canvas 3] : " + actValue3);
            GlobalLog.LogEvidence("Actual Value [Canvas 4] : " + actValue4);
            
            if (   actValue0 == expValue
                && actValue1 == expValue
                && actValue2 == expValue
                && actValue3 == expValue
                && actValue4 == expValue)
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
