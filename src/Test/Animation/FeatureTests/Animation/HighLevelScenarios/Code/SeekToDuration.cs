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

    [Test(2, "Animation.HighLevelScenarios.Regressions", "SeekToDurationTest")]
    public class SeekToDurationTest : WindowTest
    {
        #region Test case members

        private VisualVerifier      _verifier;
        private Button              _button1             = null;
        private DependencyProperty  _dp                  = Button.WidthProperty;
        private AnimationClock      _clock1              = null;
        private string              _windowTitle         = "Animation";
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        private bool                _testPassed          = true;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          SeekToDurationTest Constructor
        ******************************************************************************/
        public SeekToDurationTest()
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
            Window.Width        = 600d;
            Window.Height       = 400d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.Title        = _windowTitle; 
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            Canvas body = new Canvas();
            body.Background = Brushes.RoyalBlue;

            _button1 = new Button();
            body.Children.Add(_button1);
            _button1.Content         = "button";
            _button1.Height          = 50d;
            _button1.Width           = 10d;
            _button1.Background      = Brushes.PowderBlue;
            Canvas.SetTop(_button1, 20d);
            
            Window.Content = body;

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
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromSeconds(1);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");


            DoubleAnimation animDouble = new DoubleAnimation();                                             
            animDouble.BeginTime        = TimeSpan.FromMilliseconds(0);
            animDouble.Duration         = new Duration(TimeSpan.FromSeconds(5));
            animDouble.By               = 500d;
            animDouble.RepeatBehavior   = new RepeatBehavior(TimeSpan.FromSeconds(20));

            _clock1 = animDouble.CreateClock();
            _button1.ApplyAnimationClock(_dp, _clock1);
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
                _clock1.Controller.Seek( TimeSpan.FromSeconds(5), TimeSeekOrigin.BeginTime );
                _clock1.Controller.Pause();
            }
            else if (_tickCount == 2)
            {
                _testPassed = CheckResults(10d, "After Seek") && _testPassed;
                _clock1.Controller.Stop();
                _clock1.Controller.Resume();
                _clock1.Controller.Begin();
            }
            else if (_tickCount == 3)
            {
                _clock1.Controller.Pause();

            }
            else if (_tickCount == 4)
            {
                _clock1.Controller.Seek( TimeSpan.FromSeconds(5), TimeSeekOrigin.BeginTime );
            }
            else if (_tickCount == 5)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();

                _testPassed = CheckResults(10d, "After Pause/Seek") && _testPassed;

                if (_testPassed)
                {
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    Signal("TestFinished", TestResult.Fail);
                }
            }
        }

        /******************************************************************************
        * Function:          CheckResults
        ******************************************************************************/
        /// <summary>
        /// Compares expected vs. actual values.
        /// </summary>
        /// <returns>A boolean, indicating pass or fail</returns>
        private bool CheckResults(double expValue, string desc)
        {
            double tolerance    = 5d;
            double actValue     = (double)_button1.GetValue(_dp);

            GlobalLog.LogEvidence("------ RESULTS [" + desc + "] ------");
            GlobalLog.LogEvidence(" Actual   : " + actValue);
            GlobalLog.LogEvidence(" Expected : " + expValue);

            if (actValue <= expValue+tolerance && actValue >= expValue-tolerance)
            {
                return true;
            }
            else
            {
                return false;
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
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }

        #endregion
    }
}
