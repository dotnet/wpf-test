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
    /// <priority>2</priority>
    /// <description>
    /// Regression Test: "Assert at Clock.AdjustBeginTime when invoking Pause() after ApplyAnimationClock()"
    /// </description>
    /// </summary>
    [Test(2, "Animation.PropertyMethodEvent.Regressions", "ApplyAnimationPauseTest")]
    public class ApplyAnimationPauseTest : WindowTest
    {
        #region Test case members

        private TextBox             _textbox1;
        private AnimationClock      _clock;
        private double              _fromValue       = 60d;
        private double              _toValue         = 120d;
        private DispatcherTimer     _aTimer          = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ApplyAnimationPauseTest Constructor
        ******************************************************************************/
        public ApplyAnimationPauseTest()
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
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            Canvas body  = new Canvas();
            Window.Content = body;

            _textbox1 = new TextBox();
            body.Children.Add(_textbox1);
            _textbox1.Height  = 60d;
            _textbox1.Width   = 100d;
            
            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            DoubleAnimation animHeight = new DoubleAnimation();
            animHeight.BeginTime          = null;
            animHeight.Duration           = new Duration(TimeSpan.FromMilliseconds(2000));
            animHeight.From               = _fromValue;
            animHeight.To                 = _toValue;
            
            _clock = animHeight.CreateClock();
            _textbox1.ApplyAnimationClock(TextBox.HeightProperty, _clock);
            
            _clock.Controller.Pause();

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,3000);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");
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

            double actValue = (double)_textbox1.GetValue(TextBox.HeightProperty);
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + _fromValue); //No change will occur, due to the Pause().
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue == _fromValue)
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
