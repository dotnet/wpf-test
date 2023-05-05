// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
    /// <area>Animation.HighLevelScenarios.Regressions</area>


    [Test(2, "Animation.HighLevelScenarios.Regressions", "ReplaceOpacityTest")]
    public class ReplaceOpacityTest : WindowTest
    {
        #region Test case members

        private Rectangle           _rectangle1          = null;
        private double              _toValue             = 0.05d;
        private string              _windowTitle         = "Animation";
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          ReplaceOpacityTest Constructor
        ******************************************************************************/
        public ReplaceOpacityTest()
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

            Canvas body = new Canvas();
            body.Background = Brushes.MidnightBlue;

            _rectangle1 = new Rectangle();
            body.Children.Add(_rectangle1);
            _rectangle1.Height        = 250d;
            _rectangle1.Width         = 250d;
            _rectangle1.Fill          = Brushes.Moccasin;
            
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
            StartAnimation(0.55d);

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromMilliseconds(1500);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");
        }

        /******************************************************************************
        * Function:          StartAnimation
        ******************************************************************************/
        /// <summary>
        /// Create and begin the Animation.
        /// </summary>
        /// <returns></returns>
        private void StartAnimation(double value)
        {
            DoubleAnimationUsingKeyFrames animKeyFrame = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            DKFC.Add(new LinearDoubleKeyFrame(1d,      KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            DKFC.Add(new LinearDoubleKeyFrame(value, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));
            animKeyFrame.KeyFrames = DKFC;
            
            _rectangle1.BeginAnimation(Rectangle.OpacityProperty, animKeyFrame);
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
                _rectangle1.ClearValue(Rectangle.OpacityProperty);
                StartAnimation(_toValue);

            }
            else if (_tickCount == 2)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();

                double expOpacity = _toValue;
                double actOpacity = _rectangle1.Opacity;

                GlobalLog.LogEvidence("---------- Result ------");
                GlobalLog.LogEvidence(" Actual   : " + actOpacity);
                GlobalLog.LogEvidence(" Expected : " + expOpacity);

                if (actOpacity == expOpacity)
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
