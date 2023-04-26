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
    /// <area>Animation.PropertyMethodEvent.Regressions</area>


    [Test(2, "Animation.PropertyMethodEvent.Regressions", "FreezableChangedTest")]
    public class FreezableChangedTest : WindowTest
    {
        #region Test case members

        private ToolTip                         _tooltip1;
        private Button                          _button1;
        private DoubleAnimationUsingKeyFrames   _animKeyFrame;
        private AnimationClock                  _clock;
        private double                          _baseValue       = 50d;
        private double                          _fromValue       = 50d;
        private double                          _toValue         = 3d;
        private bool                            _changedFired    = false;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          FreezableChangedTest Constructor
        ******************************************************************************/
        public FreezableChangedTest()
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
            body.Background = Brushes.SpringGreen;
            body.Height     = 300d;
            body.Width      = 300d;

            _button1 = new Button();
            body.Children.Add(_button1);
            _button1.SetValue(Canvas.LeftProperty, 150d);
            _button1.SetValue(Canvas.TopProperty, 150d);
            _button1.Height            = 50d;
            _button1.Width             = 100d;
            
            _tooltip1 = new ToolTip();
            _button1.ToolTip = _tooltip1;
            _tooltip1.Content            = "Avalon!";
            _tooltip1.HorizontalOffset   = 10d;
            _tooltip1.VerticalOffset     = 10d;
            _tooltip1.Height             = 50d;
            _tooltip1.Width              = _baseValue;
            
            _animKeyFrame = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            DKFC.Add(new DiscreteDoubleKeyFrame(_fromValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,0))));
            DKFC.Add(new DiscreteDoubleKeyFrame(_toValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,500))));
            _animKeyFrame.KeyFrames = DKFC;
            _animKeyFrame.Changed += new EventHandler(OnChanged);

            _animKeyFrame.BeginTime      = TimeSpan.FromMilliseconds(0);
            _animKeyFrame.Duration       = new Duration(TimeSpan.FromMilliseconds(500));

            Window.Content = body;
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          OnChanged
        ******************************************************************************/
        /// <summary>
        /// Event handler fired when the animated property changes.
        /// </summary>
        /// <returns></returns>
        private void OnChanged(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----Changed Fired----");
            _changedFired = true;
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
            }
            else if (_tickCount == 2)
            {
                _clock = _animKeyFrame.CreateClock();
                _tooltip1.ApplyAnimationClock(FrameworkElement.WidthProperty, _clock);
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

            double actValue = (double)_tooltip1.GetValue(FrameworkElement.WidthProperty);

            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value:       " + _toValue);
            GlobalLog.LogEvidence("Actual Value:         " + actValue);
            GlobalLog.LogEvidence("Changed Event fired:  " + _changedFired);
            
            if (actValue == _toValue && _changedFired)
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
