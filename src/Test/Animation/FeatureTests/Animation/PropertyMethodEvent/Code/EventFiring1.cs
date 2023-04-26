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
using System.Windows.Documents;
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


    [Test(2, "Animation.PropertyMethodEvent.Regressions", "EventFiring1Test")]
    public class EventFiring1Test : WindowTest
    {
        #region Test case members

        Button                      _BN  = null;
        private int                 _stateCount      = 0;
        private int                 _speedCount      = 0;
        private DispatcherTimer     _aTimer          = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          EventFiring1Test Constructor
        ******************************************************************************/
        public EventFiring1Test()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content and starts a DispatcherTimer used for controlling 
        /// verification of the Animation.
        /// </summary>
        /// <returns>Returns success</returns>
        TestResult CreateTree()
        {
            Window.Width        = 200d;
            Window.Height       = 150d;

            Canvas body  = new Canvas();
            body.Background = Brushes.Lavender;
            Window.Content = body;

            _BN  = new Button();
            _BN.Content      ="Animate";
            _BN.Opacity      = 0d;
            _BN.Width      = 50d;
            _BN.Height     = 50d;
            body.Children.Add(_BN);

            DoubleAnimation anim = new DoubleAnimation();                                   
            anim.BeginTime          = TimeSpan.FromMilliseconds(3000);
            anim.Duration           = new Duration(TimeSpan.FromMilliseconds(500));
            anim.From               = 0d;
            anim.To                 = 1d;
            anim.CurrentStateInvalidated        += new EventHandler(OnCurrentStateInvalidated);
            anim.CurrentGlobalSpeedInvalidated  += new EventHandler(OnCurrentGlobalSpeedInvalidated);

            _BN.BeginAnimation(Button.OpacityProperty, anim);
            
            //Start the Timer.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,4000);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");

            return TestResult.Pass;
        }

        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---CurrentStateInvalidated: " + ((Clock)sender).CurrentState);
            _stateCount++;
        }

        private void OnCurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---CurrentGlobalSpeed: " + ((Clock)sender).CurrentGlobalSpeed);
            _speedCount++;
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
            
            int expCount = 2;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("CurrentStateInvalidated Expected: " + expCount);
            GlobalLog.LogEvidence("CurrentStateInvalidated Actual:   " + _stateCount);
            GlobalLog.LogEvidence("CurrentGlobalSpeedInvalidated Expected: " + expCount);
            GlobalLog.LogEvidence("CurrentGlobalSpeedInvalidated Actual:   " + _speedCount);
            
            if (_stateCount == expCount && _speedCount == expCount)
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
