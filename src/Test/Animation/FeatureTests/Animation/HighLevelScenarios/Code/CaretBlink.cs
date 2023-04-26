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
    /// <area>Animation.LowLevelScenarios.Regressions</area>
    /// <priority>2</priority>

    [Test(2, "Animation.ManualTestCase.Regressions", "CaretBlinkTest")]
    public class CaretBlinkTest : WindowTest
    {

        #region Test case members
        private DispatcherTimer     _aTimer          = null;
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          CaretBlinkTest Constructor
        ******************************************************************************/
        public CaretBlinkTest()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StopTest);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the Window content.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            Window.Width            = 600d;
            Window.Height           = 400d;
            Window.Left             = 0d;
            Window.Top              = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.MidnightBlue;

            TextBlock textblock1 = new TextBlock();
            body.Children.Add(textblock1);
            textblock1.Foreground   = Brushes.White;
            textblock1.FontSize     = 18d;
            textblock1.Text         = "***MANUAL TEST CASE***\n\nTab back and forth between the TextBox and the Button.\nThe test case passes if the caret blinks steadily.";
            Canvas.SetLeft(textblock1, 50d);
            Canvas.SetTop(textblock1, 5d);

            TextBox textbox1 = new TextBox();
            body.Children.Add(textbox1);
            textbox1.FontSize   = 34d;
            textbox1.Height     = 50d;
            textbox1.Width      = 400d;
            textbox1.Text       = "This is a test";
            Canvas.SetLeft(textbox1, 50d);
            Canvas.SetTop(textbox1, 125d);

            Button button1 = new Button();
            body.Children.Add(button1);
            button1.FontSize    = 48d;
            button1.Content     = "Tab Here";
            Canvas.SetLeft(button1, 50d);
            Canvas.SetTop(button1, 250d);
            
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
            _aTimer.Interval = TimeSpan.FromSeconds(5);
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
            Signal("TimeOut", TestResult.Pass);
            _aTimer.Stop();
        }

        /******************************************************************************
        * Function:          StopTest
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        TestResult StopTest()
        {
            WaitForSignal("TimeOut");
            
            GlobalLog.LogStatus("----Test Has Timed Out----");
            
            return TestResult.Pass;
        }

        #endregion
    }
}
