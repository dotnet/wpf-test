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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "DeleteDuringAnimationTest")]
    public class DeleteDuringAnimationTest : WindowTest
    {
        #region Test case members

        private VisualVerifier      _verifier;
        private Canvas              _body                = null;
        private Button              _button1             = null;
        private SolidColorBrush     _SCB                 = null;
        private AnimationClock      _clock1              = null;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          DeleteDuringAnimationTest Constructor
        ******************************************************************************/
        public DeleteDuringAnimationTest()
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
            Window.Width        = 600d;
            Window.Height       = 400d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.Title        = "Animation"; 
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            _body = new Canvas();
            _body.Background = Brushes.MediumBlue;

            _button1 = new Button();
            _body.Children.Add(_button1);
            _button1.Content         = "Avalon!";
            _button1.FontSize        = 48d;

            _SCB = new SolidColorBrush(Colors.DarkOrchid);
            _button1.Background = _SCB;

            Window.Content = _body;

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
            ColorAnimation animColor = new ColorAnimation();                                             
            animColor.BeginTime        = TimeSpan.FromMilliseconds(0);
            animColor.Duration         = new Duration(TimeSpan.FromSeconds(5));
            animColor.To               = Colors.MistyRose;

            _clock1 = animColor.CreateClock();
            _SCB.ApplyAnimationClock(SolidColorBrush.ColorProperty, _clock1);
        }

        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns></returns>
        TestResult StartTimer()
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
            _tickCount++;
            
            if (_tickCount == 1)
            {
                _body.Children.Remove(_button1);
            }
            else if (_tickCount == 2)
            {
                Signal("TestFinished", TestResult.Pass);
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
            WaitForSignal("TestFinished");

            float tolerance = 0.10f;
            int x           = 35;
            int y           = 30;

            Color actColor = _verifier.getColorAtPoint(x,y);

            Color expColor = Colors.MediumBlue;
            bool testPassed = AnimationUtilities.CompareColors(expColor, actColor, tolerance);

            GlobalLog.LogEvidence("---------- Result at (" + x + "," + y + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actColor.ToString());
            GlobalLog.LogEvidence(" Expected : " + expColor.ToString());

            if (testPassed)
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
