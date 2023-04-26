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
    /// <area>Animation.PropertyMethodEvent.Regressions</area>

    [Test(2, "Animation.PropertyMethodEvent.Regressions", "SpeedRatioAssertTest")]
    public class SpeedRatioAssertTest : XamlTest
    {

        #region Test case members

        private Frame               _animatedElement;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        public SpeedRatioAssertTest(): this(@"SpeedRatioAssertNegative.xaml")
        {
        }

        [Variation(@"SpeedRatioAssertNegative.xaml")]
        [Variation(@"SpeedRatioAssertZero.xaml")]
        /******************************************************************************
        * Function:          SpeedRatioAssertTest Constructor
        ******************************************************************************/
        public SpeedRatioAssertTest(string fileName) : base(fileName)
        {
            InitializeSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(GetAnimatedElement);
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
        * Function:          GetAnimatedElement
        ******************************************************************************/
        /// <summary>
        /// Looks in the Markup for the element that is animated.
        /// </summary>
        /// <returns>The element that is animated in Markup</returns>
        private TestResult GetAnimatedElement()          
        {
            WaitForSignal("AnimationDone");

            DependencyObject control = (DependencyObject)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"AnimateTemplate");
            if (control == null)
            {
                GlobalLog.LogEvidence("----AnimateTemplate was not found.");
                return TestResult.Fail;
            }
            else
            {
                _animatedElement = (Frame)((Control)control).Template.FindName("ControlTemplateRoot", (Control)control);
                
                if (_animatedElement == null)
                {
                    GlobalLog.LogEvidence("----The animated element was not found.");
                    return TestResult.Fail;
                }
                else
                {
                    GlobalLog.LogEvidence("----The animated element was found.");
                    return TestResult.Pass;
                }
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
            SolidColorBrush brush = (SolidColorBrush)_animatedElement.GetValue(Frame.BackgroundProperty);
            Color actValue = Color.FromRgb(brush.Color.R, brush.Color.G, brush.Color.B);
            Color expValue = Colors.Red;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
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
