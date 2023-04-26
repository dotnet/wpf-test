// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "CompositionTargetRenderingTest")]
    public class CompositionTargetRenderingTest : WindowTest
    {
        #region Test case members

        private Button              _button1             = null;
        private TextBox             _textbox1            = null;
        private Stopwatch           _stopWatch           = new Stopwatch();
        private double              _frameCounter        = 0;
        private TimeSpan            _renderingTime;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          CompositionTargetRenderingTest Constructor
        ******************************************************************************/
        public CompositionTargetRenderingTest()
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
            Window.Width        = 400d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.RoyalBlue;

            _button1 = new Button();
            body.Children.Add(_button1);
            _button1.Height          = 50d;
            _button1.Width           = 10d;
            _button1.Background      = Brushes.Fuchsia;
            Canvas.SetTop(_button1, 70d);
            
            _textbox1 = new TextBox();
            body.Children.Add(_textbox1);
            Canvas.SetTop(_textbox1, 200d);
            
            Window.Content = body;

            CompositionTarget.Rendering += UpdateWidth;

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          UpdateWidth
        ******************************************************************************/
        /// <summary>
        /// Called just before frame is rendered to allow custom drawing.
        /// </summary>
        /// <returns></returns>
        protected void UpdateWidth(object sender, EventArgs e)
        {
            _frameCounter++;

            if (_frameCounter == 1)
            {
                // Starting timing.
                _stopWatch.Start();
            }
            else if (_frameCounter == 180)
            {
                _stopWatch.Stop();
                Signal("AnimationDone", TestResult.Pass);
            }

            // Determine frame rate in fps (frames per second).
            long frameRate = (long)(_frameCounter / _stopWatch.Elapsed.TotalSeconds);
            if (frameRate > 0)
            {
            //    // Update elapsed time, number of frames, and frame rate.
            //    GlobalLog.LogStatus("--StopWatch.Elapsed: " +  stopWatch.Elapsed.ToString());
            //    GlobalLog.LogStatus("--FrameCounter:      " +  frameCounter.ToString());
            //    GlobalLog.LogStatus("--FrameRate:         " +  frameRate.ToString());
                _button1.Content = frameRate.ToString();
            }

            //Animate the Button's Width.
            _button1.Width = _button1.Width + 1d;

            RenderingEventArgs r = (RenderingEventArgs)e;
            _renderingTime = r.RenderingTime;
            _textbox1.Text = _renderingTime.ToString();
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

            double actValue1    = (double)_button1.GetValue(Button.WidthProperty);
            double expValue1    = 190d;
            TimeSpan actValue2  = _renderingTime;
            TimeSpan expValue2  = TimeSpan.FromMilliseconds(1500);

            bool b1 = (actValue1 == expValue1);
            bool b2 = (actValue2 > expValue2);

            GlobalLog.LogEvidence(" Actual GetValue   : " + actValue1);
            GlobalLog.LogEvidence(" Expected GetValue : " + expValue1);
            GlobalLog.LogEvidence(" Actual RenderingTime  :    " + actValue2);
            GlobalLog.LogEvidence(" Expected RenderingTime : > " + expValue2);
            GlobalLog.LogEvidence("---------------------------------------------------");
            GlobalLog.LogEvidence("--FINAL RESULTS: " + b1 + " / " + b2);
            GlobalLog.LogEvidence("---------------------------------------------------");

            if (b1 && b2)
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
