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
    /// <area>Animation.LowLevelScenarios.Regressions</area>
    /// <priority>2</priority>
    /// <description>
    /// Regression Test: "Animation: Additive animations with different begin times are composed incorrectly"
    /// Scenario: verify the effect of order of multiple animations animating the same property.
    /// NOTE: the design has changed, such that the order in code now overrides the BeginTime
    /// order, rather than the other way around.
    /// </description>
    /// </summary>
    [Test(2, "Animation.LowLevelScenarios.Regressions", "AnimationOrderTest")]
    public class AnimationOrderTest : WindowTest
    {
        #region Test case members

        private Canvas                  _body        = null;
        private DispatcherTimer         _aTimer      = null;
        private double                  _toValue1    = 0d;
        private double                  _toValue2    = 400d;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          AnimationOrderTest Constructor
        ******************************************************************************/
        public AnimationOrderTest()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            Window.Width        = 300d;
            Window.Height       = 400d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            _body = new Canvas();
            _body.Height     = 0d;
            _body.Background = Brushes.OrangeRed;

            Rectangle rectangle = new Rectangle();
            _body.Children.Add(rectangle);
            rectangle.Fill              = Brushes.Moccasin;
            rectangle.Height            = 150d;
            rectangle.Width             = 150d;
            rectangle.StrokeThickness   = 1d;
            rectangle.Stroke            = Brushes.Black;
            
            DoubleAnimation anim1 = new DoubleAnimation();
            anim1.BeginTime      = TimeSpan.FromMilliseconds(1);
            anim1.Duration       = new Duration(TimeSpan.FromSeconds(1));
            anim1.To             = _toValue1;
            _body.BeginAnimation(Canvas.HeightProperty, anim1);
            
            DoubleAnimation anim2 = new DoubleAnimation();
            anim2.BeginTime      = TimeSpan.FromMilliseconds(0);
            anim2.Duration       = new Duration(TimeSpan.FromSeconds(2));
            anim2.To             = _toValue2;
            _body.BeginAnimation(Canvas.HeightProperty, anim2);

            Window.Content = _body;
            
            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromSeconds(3);
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

            double actValue = (double)_body.GetValue(Canvas.HeightProperty);
            double expValue = _toValue2;
            
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
