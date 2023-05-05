// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
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
    /// <area>Animation.HighLevelScenarios.Regressions</area>
    /// <priority>2</priority>
    /// <description>
    /// Regression Test: "Animations added to Changables off of Elements not enabled"
    /// </description>
    /// </summary>

    // [DISABLE WHILE PORTING]
    // [Test(2, "Animation.HighLevelScenarios.Regressions", "AddAnimationsTest")]
    public class AddAnimationsTest : WindowTest
    {
        #region Test case members

        private TextBlock                       _textblock1;
        private SolidColorBrush                 _SCB;
        private double                          _toOpacity       = 0;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          AddAnimationsTest Constructor
        ******************************************************************************/
        public AddAnimationsTest()
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
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.DodgerBlue;
            body.Height     = 300d;
            body.Width      = 300d;
            
            _textblock1 = new TextBlock();
            body.Children.Add(_textblock1);
            _textblock1.Text = "Avalon!";
            Canvas.SetTop  (_textblock1, 100d);
            Canvas.SetLeft (_textblock1, 100d);   

            _SCB = new SolidColorBrush();
            _SCB.Color = Colors.White;
            _textblock1.Foreground = _SCB;

            Window.Content = body;
            
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
            
            if (_tickCount == 1)
            {
                _SCB.BeginAnimation(SolidColorBrush.ColorProperty, null);
            }
            else if (_tickCount == 2)
            {
                ColorAnimation animColor = new ColorAnimation();
                animColor.From              = Colors.Green;
                animColor.To                = Colors.Red;
                animColor.BeginTime         = TimeSpan.FromMilliseconds(0);
                animColor.Duration          = new Duration(TimeSpan.FromMilliseconds(250));
                
                _SCB.BeginAnimation(SolidColorBrush.ColorProperty, animColor);
            }
            else if (_tickCount == 3)
            {
                _SCB.BeginAnimation(SolidColorBrush.OpacityProperty, null);
            }
            else if (_tickCount == 4)
            {
                DoubleAnimation animOpacity = new DoubleAnimation();
                animOpacity.From            = 1d;
                animOpacity.To              = _toOpacity;
                animOpacity.BeginTime       = TimeSpan.FromMilliseconds(0);
                animOpacity.Duration        = new Duration(TimeSpan.FromMilliseconds(250));
                
                _SCB.BeginAnimation(SolidColorBrush.OpacityProperty, animOpacity);
            }
            else
            {
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

            SolidColorBrush brush = (SolidColorBrush)_textblock1.GetValue(TextBlock.ForegroundProperty);
            double actValue2 = (double)brush.Opacity;
            double expValue2 = _toOpacity;
            
            Color actValue1 = Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
            Color expValue1 = (Color)brush.Color;
            
            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Color:   " + expValue1);
            GlobalLog.LogEvidence("Actual Color:     " + actValue1);
            GlobalLog.LogEvidence("Expected Opacity: " + expValue2);
            GlobalLog.LogEvidence("Actual Opacity:   " + actValue2);
            
            if (actValue1 == expValue1 && actValue2 == expValue2)
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
