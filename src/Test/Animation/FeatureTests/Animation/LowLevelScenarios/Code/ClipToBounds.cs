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

    [Test(2, "Animation.LowLevelScenarios.Regressions", "ClipToBoundsTest")]
    public class ClipToBoundsTest : WindowTest
    {
        #region Test case members

        private Canvas                          _canvas2;
        private BooleanAnimationUsingKeyFrames  _animKeyFrame;
        private AnimationClock                  _clock;
        private bool                            _baseValue       = false;
        private bool                            _fromValue       = false;
        private bool                            _toValue         = true;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ClipToBoundsTest Constructor
        ******************************************************************************/
        public ClipToBoundsTest()
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
            Window.Width        = 200d;
            Window.Height       = 200d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.MistyRose;
            body.Height     = 200d;
            body.Width      = 200d;

            _canvas2 = new Canvas();
            body.Children.Add(_canvas2);
            _canvas2.Height          = 35d;
            _canvas2.Width           = 70d;
            _canvas2.ClipToBounds    = _baseValue;
            _canvas2.Background      = Brushes.HotPink;
            
            Button button1 = new Button();
            _canvas2.Children.Add(button1);
            button1.FontSize        = 48d;
            button1.Content         = "Avalon!";
            
            _animKeyFrame = new BooleanAnimationUsingKeyFrames();
            BooleanKeyFrameCollection DKFC = new BooleanKeyFrameCollection();
            DKFC.Add(new DiscreteBooleanKeyFrame(_fromValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,0))));
            DKFC.Add(new DiscreteBooleanKeyFrame(_toValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,500))));
            _animKeyFrame.KeyFrames = DKFC;

            _animKeyFrame.BeginTime      = TimeSpan.FromMilliseconds(0);
            _animKeyFrame.Duration       = new Duration(TimeSpan.FromMilliseconds(500));

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
                _clock = _animKeyFrame.CreateClock();
                _canvas2.ApplyAnimationClock(Canvas.ClipToBoundsProperty, _clock);
                _clock.Controller.Begin();
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

            bool actValue = (bool)_canvas2.GetValue(Canvas.ClipToBoundsProperty);
            bool expValue = _toValue;
            
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
