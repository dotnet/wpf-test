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
    /// <area>Animation.HighLevelScenarios.Regressions</area>
    /// <priority>0</priority>
    /// <description>
    /// Regression Test: "Animations could not be reused."
    /// </description>
    /// </summary>
    [Test(0, "Animation.HighLevelScenarios.Regressions", "AnimationReuseTest")]
    public class AnimationReuseTest : XamlTest
    {

        #region Test case members

        private Ellipse             _animatedElement1;
        private Ellipse             _animatedElement2;
        private TranslateTransform  _tt;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          AnimationReuseTest Constructor
        ******************************************************************************/
        public AnimationReuseTest() : base(@"AnimationReuse.xaml")
        {
            InitializeSteps += new TestStep(Animate);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Animate
        ******************************************************************************/
        /// <summary>
        /// Retrieves the animated elements from the markup, and then begins the Animations.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult Animate()
        {
            _animatedElement1 = (Ellipse)RootElement.FindName("WEllipse");
            _animatedElement2 = (Ellipse)RootElement.FindName("HEllipse");
            
            if (_animatedElement1 == null || _animatedElement2 == null)
            {
                GlobalLog.LogEvidence("An animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The animated elements were found.");
                
                DoubleAnimation PosAni = new DoubleAnimation();

                PosAni.By           = 600;
                PosAni.AutoReverse  = false ;
                PosAni.Duration     = new Duration(TimeSpan.FromSeconds(1));
                _tt = _animatedElement1.RenderTransform as TranslateTransform;
                _tt.BeginAnimation(TranslateTransform.XProperty, PosAni);

                _animatedElement1.UpdateLayout();

                PosAni.By           = -200;
                PosAni.AutoReverse  = true;
                PosAni.Duration     = new Duration(TimeSpan.FromSeconds(1));
                _tt = _animatedElement2.RenderTransform as TranslateTransform;
                _tt.BeginAnimation(TranslateTransform.XProperty, PosAni);
                
                return TestResult.Pass;
            }
        }

        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,3000);
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

            double actValue = (double)_tt.GetValue(TranslateTransform.XProperty);
            double expValue = 0d;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value  : " + actValue);
            
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
