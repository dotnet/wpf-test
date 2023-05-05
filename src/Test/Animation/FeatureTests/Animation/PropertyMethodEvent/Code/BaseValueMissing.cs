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
    /// <area>Animation.PropertyMethodEvent.Regressions</area>

    [Test(2, "Animation.PropertyMethodEvent.Regressions", "BaseValueMissingTest")]
    public class BaseValueMissingTest : WindowTest
    {

        #region Test case members
            private DispatcherTimer     _aTimer      = null;
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          BaseValueMissingTest Constructor
        ******************************************************************************/
        public BaseValueMissingTest()
        {
            RunSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
            _aTimer.Start();
            
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
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the page content, an Animation, and a corresponding Clock.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult CreateTree()
        {
            Window.Width        = 350d;
            Window.Height       = 600d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body  = new Canvas();
            Window.Content = body;

            TextBlock textblock1 = new TextBlock();
            body.Children.Add(textblock1);
            
            DoubleAnimation animation = new DoubleAnimation();
            animation.To               = 500d;
            animation.BeginTime        = TimeSpan.FromMilliseconds(0);
            animation.Duration         = new Duration(TimeSpan.FromMilliseconds(50));

            AnimationClock clock1 = animation.CreateClock();

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException), "Inner");                
            textblock1.ApplyAnimationClock(TextBlock.WidthProperty, clock1);
            
            WaitForSignal("AnimationDone");
                       
            return TestResult.Pass;
        }

        #endregion
    }
}
