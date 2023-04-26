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
    /// <area>Storyboards.PropertyMethodEvent.Regressions</area>

    [Test(2, "Storyboards.PropertyMethodEvent.Regressions", "BeginTimeOnStoryboardTest")]
    public class BeginTimeOnStoryboardTest : XamlTest
    {

        #region Test case members

        private string                  _inputString     = "";
        private Int32                   _actValueTick1   = 0;
        private Int32                   _actValueTick2   = 0;
        private DispatcherTimer         _aTimer          = null;
        private Rectangle               _animatedElement = null;
        private int                     _tickCount       = 0;
        
        #endregion


        #region Constructor

        public BeginTimeOnStoryboardTest(): this(@"BeginTimeOnStoryboard.xaml")
        {
        }

        [Variation(@"BeginTimeOnStoryboard.xaml")]
        [Variation(@"BeginTimeOnStoryboardNull.xaml")]

        /******************************************************************************
        * Function:          BeginTimeOnStoryboardTest Constructor
        ******************************************************************************/
        public BeginTimeOnStoryboardTest(string fileName) : base(fileName)
        {
            _inputString = fileName;
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
        /// Carries out initialization of the Window.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult Initialize()
        {
            Window.Left                 = 0d;
            Window.Top                  = 0d;
            Window.Height               = 300;
            Window.Width                = 500;

            return TestResult.Pass;
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
            //First, retrieve the animated Rectangle.
            _animatedElement = (Rectangle)AnimationUtilities.FindElement(RootElement, "r1");

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
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
            _tickCount++;
            
            //GlobalLog.LogStatus("**********Tick #" + tickCount);
            
            //Retrieve the animated dp value before and after the Animation, to
            // validate a non-zero BeginTime on the Storyboard in Markup.
            if (_tickCount == 1 )
            {
                _actValueTick1 = (Int32)_animatedElement.GetValue(Canvas.ZIndexProperty);
            }
            else if (_tickCount == 8 )
            {
                _actValueTick2 = (Int32)_animatedElement.GetValue(Canvas.ZIndexProperty);
                
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
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
            WaitForSignal("AnimationDone");
            
            Int32 expValueTick1 = 1;
            Int32 expValueTick2 = 0;
            if (_inputString == "BeginTimeOnStoryboard.xaml")
            {
                expValueTick2 = 5;
            }
            else if (_inputString == "BeginTimeOnStoryboardNull.xaml")
            {
                //BeginTime is set to null, so no animation should occur.
                expValueTick2 = 1;
            }
            
            GlobalLog.LogEvidence("Expected Value [1]: " + expValueTick1);
            GlobalLog.LogEvidence("Actual Value   [1]: " + _actValueTick1);
            GlobalLog.LogEvidence("Expected Value [2]: " + expValueTick2);
            GlobalLog.LogEvidence("Actual Value   [2]: " + _actValueTick2);

            bool b1 = (_actValueTick1 == expValueTick1);
            bool b2 = (_actValueTick2 == expValueTick2);
            
            if ( b1 && b2 ) 
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
