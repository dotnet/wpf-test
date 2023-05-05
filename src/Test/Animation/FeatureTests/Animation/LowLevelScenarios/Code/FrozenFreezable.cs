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
    /// <priority>2</priority>

    [Test(2, "Animation.LowLevelScenarios.Regressions", "FrozenFreezableTest")]
    public class FrozenFreezableTest : XamlTest
    {

        #region Test case members

        private EllipseGeometry     _animatedGeometry;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          FrozenFreezableTest Constructor
        ******************************************************************************/
        public FrozenFreezableTest() : base(@"FrozenFreezable.xaml")
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
            _animatedGeometry = (EllipseGeometry)RootElement.FindName("ellipseGeo1");
            
            if (_animatedGeometry == null)
            {
                GlobalLog.LogEvidence("The animated geometry was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The animated geometry was found.");
                
                _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _aTimer.Tick += new EventHandler(OnTick);
                _aTimer.Interval = new TimeSpan(0,0,0,0,1500);
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

            Point actValue = (Point)_animatedGeometry.GetValue(EllipseGeometry.CenterProperty);
            double expValue = 100d;
            
            //Because animatedGeometry is frozen, the Point value returned by GetValue will not
            //reflect the change due to the Animation.  It will be the original base value.
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value (y):  " + expValue);
            GlobalLog.LogEvidence("Actual Value   (y):  " + actValue.Y);
            
            if (actValue.Y == expValue)
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
