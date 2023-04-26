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
using Microsoft.Test.Input;   //UserInput
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.LowLevelScenarios.Regressions</area>

    [Test(2, "Animation.ManualTestCase.Regressions", "ToolTipOnButtonTest")]
    public class ToolTipOnButtonTest : WindowTest
    {
        #region Test case members

        private ToolTip                         _tooltip1;
        private Button                          _button1;
        private RotateTransform                 _rotateTransform1    = null;
        private DispatcherTimer                 _aTimer              = null;
        private int                             _tickCount           = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ToolTipOnButtonTest Constructor
        ******************************************************************************/
        public ToolTipOnButtonTest()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(EndTest);
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
            Window.Width        = 600d;
            Window.Height       = 400d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.Wheat;

            TextBlock textblock1 = new TextBlock();
            body.Children.Add(textblock1);
            textblock1.Foreground   = Brushes.MidnightBlue;
            textblock1.Text         = "MANUAL TEST: the test case fails if ToolTip artifacts appear during the Button's rotation.";

            _button1 = new Button();
            body.Children.Add(_button1);
            _button1.SetValue(Canvas.LeftProperty, 100d);
            _button1.SetValue(Canvas.TopProperty, 100d);
            _button1.Height            = 50d;
            _button1.Width             = 200d;

            _rotateTransform1 = new RotateTransform();
            _rotateTransform1.Angle      = 0d;
            _rotateTransform1.CenterX    = 150d;
            _rotateTransform1.CenterY    = 150d;

            _button1.RenderTransform = _rotateTransform1;

            _tooltip1 = new ToolTip();
            _button1.ToolTip = _tooltip1;
            _tooltip1.Content            = "I am a ToolTip";

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
                UserInput.MouseMove(_button1,10,10);
                _tooltip1.IsOpen = true;

                DoubleAnimation animRotate1  = new DoubleAnimation();
                animRotate1.BeginTime           = TimeSpan.FromSeconds(0);
                animRotate1.Duration            = new Duration(TimeSpan.FromSeconds(10));
                animRotate1.From                = 0;
                animRotate1.To                  = 360;

                _rotateTransform1.BeginAnimation(RotateTransform.AngleProperty, animRotate1);
            }
            else if (_tickCount == 5)
            {
                _aTimer.Stop();
                Signal("AnimationDone", TestResult.Pass);
            }
        }

        /******************************************************************************
        * Function:          EndTest
        ******************************************************************************/
        TestResult EndTest()
        {
            WaitForSignal("AnimationDone");
            GlobalLog.LogEvidence("----Automatic Pass----\nTest actually passes if no rendering artifacts are observed.");
            return TestResult.Pass;

        }
        #endregion
    }
}
