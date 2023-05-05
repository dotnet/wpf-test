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
    /// <priority>2</priority>


    [Test(2, "Animation.PropertyMethodEvent.Regressions", "GetAnimationBaseValue1Test")]
    public class GetAnimationBaseValue1Test : WindowTest
    {
        #region Test case members

        private string                          _inputString     = "";
        private ToolTip                         _tooltip1;
        private Button                          _button1;
        private RadialGradientBrush             _RGB;
        private PointAnimation                  _animPoint;
        private AnimationClock                  _clock;
        private Point                           _expBaseValue;
        private Point                           _baseValue       = new Point(1,1);
        private Point                           _fromValue       = new Point(0,0);
        private Point                           _toValue         = new Point(50,50);
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        [Variation("BaseValueSet", Priority=0)]
        [Variation("NoBaseValueSet")]
        
        /******************************************************************************
        * Function:          GetAnimationBaseValue1Test Constructor
        ******************************************************************************/
        public GetAnimationBaseValue1Test(string testValue)
        {
            _inputString = testValue;
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
            _tooltip1.Width              = 75d;
            
            _RGB = new RadialGradientBrush();
            _RGB.GradientStops.Add(new GradientStop(Colors.Blue, 0.0));
            _RGB.GradientStops.Add(new GradientStop(Colors.LightGreen, 1.0));
            _RGB.Center          = new Point(0.5, 0.5);
            _RGB.RadiusX         = 1;
            _RGB.RadiusY         = 1;
            if (_inputString == "BaseValueSet")
            {
                _RGB.GradientOrigin  = _baseValue;
                _expBaseValue = _baseValue;
            }
            else
            {
                _expBaseValue = new Point(0.5,0.5);
            }

            _animPoint = new PointAnimation();
            _animPoint.From             = _fromValue;
            _animPoint.To               = _toValue;
            _animPoint.BeginTime        = TimeSpan.FromMilliseconds(0);
            _animPoint.Duration         = new Duration(TimeSpan.FromMilliseconds(750));

            _tooltip1.Background = _RGB;

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,1500);
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
            }
            else if (_tickCount == 2)
            {
                _clock = _animPoint.CreateClock();
                _RGB.ApplyAnimationClock(RadialGradientBrush.GradientOriginProperty, _clock);
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

            RadialGradientBrush brush = (RadialGradientBrush)_tooltip1.Background;
            Point actValue = brush.GradientOrigin;
            Point expValue = _toValue;

            Point actBaseValue = (Point)brush.GetAnimationBaseValue(RadialGradientBrush.GradientOriginProperty);

            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value:       " + expValue);
            GlobalLog.LogEvidence("Actual Value:         " + actValue);
            GlobalLog.LogEvidence("Expected Base Value:  " + _expBaseValue);
            GlobalLog.LogEvidence("Actual Base Value:    " + actBaseValue);
            
            if (actValue == expValue && actBaseValue == _expBaseValue)
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
