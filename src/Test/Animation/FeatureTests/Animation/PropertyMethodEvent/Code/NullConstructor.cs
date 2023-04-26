// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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


    [Test(2, "Animation.PropertyMethodEvent.Regressions", "NullConstructorTest")]
    public class NullConstructorTest : WindowTest
    {

        #region Test case members

            private DispatcherTimer     _aTimer      = null;
            private TextBox             _textbox1    = null;
            private double              _baseValue   = 150d;

        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          NullConstructorTest Constructor
        ******************************************************************************/
        public NullConstructorTest()
        {
            RunSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the page content, an Animation, and a corresponding Clock.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult CreateTree()
        {
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body  = new Canvas();
            body.Background = Brushes.Magenta;
            Window.Content = body;

            _textbox1 = new TextBox();
            _textbox1.Height     = 150d;
            _textbox1.Width      = _baseValue;
            _textbox1.Text       = "Avalon!";
            _textbox1.FontSize   = 36d;
            _textbox1.Background = Brushes.SpringGreen;
            body.Children.Add(_textbox1);
            
            DoubleAnimation animation = new DoubleAnimation();

            AnimationClock clock1 = animation.CreateClock();
            _textbox1.ApplyAnimationClock(TextBox.WidthProperty, clock1);

            return TestResult.Pass;
        }

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
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
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult Verify()
        {
            WaitForSignal("AnimationDone");

            double actValue = (double)_textbox1.GetValue(TextBox.WidthProperty);
            double expValue = _baseValue;  //No animation should occur.
            
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
