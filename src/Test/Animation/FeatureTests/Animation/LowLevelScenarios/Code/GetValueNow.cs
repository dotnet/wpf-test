// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/********************************************************************************************

********************************************************************************************/
using System;
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
    /// <priority>2</priority>

    [Test(2, "Animation.LowLevelScenarios.Regressions", "GetValueNowTest")]
    public class GetValueNowTest : WindowTest
    {
        #region Test case members

        private Rectangle           _rectangle;
        private SolidColorBrush     _SCB;
        private double              _baseValue               = 0.2d;
        private double              _fromValue               = 0.5d;
        private double              _toValue                 = 0.9d;
        private double              _actValue1               = 0;
        private double              _actValue2               = 0;
        private DispatcherTimer     _aTimer                  = null;
        private int                 _tickCount               = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          GetValueNowTest Constructor
        ******************************************************************************/
        public GetValueNowTest()
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
            body.Background = Brushes.Moccasin;

            _rectangle = new Rectangle();
            body.Children.Add(_rectangle);
            _rectangle.SetValue(Canvas.LeftProperty, 10d);
            _rectangle.SetValue(Canvas.TopProperty, 10d);
            _rectangle.Height            = 150d;
            _rectangle.Width             = 150d;
            _rectangle.Name              = "TheRectangle";
            _rectangle.StrokeThickness   = 2d;
            _rectangle.Stroke            = Brushes.Sienna;
            
            Window.Content = body;

            DoubleAnimation animOpacity = new DoubleAnimation();
            animOpacity.From                  = _fromValue;
            animOpacity.To                    = _toValue;
            animOpacity.BeginTime             = TimeSpan.FromSeconds(0);
            animOpacity.Duration              = new Duration(TimeSpan.FromSeconds(2));
            animOpacity.RepeatBehavior        = new RepeatBehavior(2);
            animOpacity.AutoReverse           = true;

            _SCB = new SolidColorBrush();
            _SCB.Color       = Colors.OrangeRed;
            _SCB.Opacity     = _baseValue;
            _rectangle.Fill = _SCB;

            AnimationClock AC = animOpacity.CreateClock();
            _SCB.ApplyAnimationClock(SolidColorBrush.OpacityProperty, AC);

            _actValue1 = (double)_SCB.GetValue(SolidColorBrush.OpacityProperty);
            
            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns></returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromMilliseconds(100);
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

            switch (_tickCount)
            {
                case 1:
                    _actValue2 = (double)_SCB.GetValue(SolidColorBrush.OpacityProperty);
                    break;
                case 2:
                    _aTimer.Stop();
                    Signal("AnimationDone", TestResult.Pass);
                    break;
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

            double expValue = _fromValue;

            bool b1 = (_actValue1 == _baseValue);
            bool b2 = (_actValue2 >  _fromValue);

            GlobalLog.LogEvidence("----RESULTS----");
            GlobalLog.LogEvidence("Act Value 1:         " + _actValue1);
            GlobalLog.LogEvidence("Exp Value 1 (Base):  " + _baseValue);

            GlobalLog.LogEvidence("Act Value 2:         " + _actValue2);
            GlobalLog.LogEvidence("Exp Value 2 (From):  " + _fromValue);

            GlobalLog.LogEvidence("Act Value 1 == Exp (Base) Value  RESULT: " + b1);
            GlobalLog.LogEvidence("Act Value 2 >  Exp (From) Value  RESULT: " + b2);

            if ( b1 && b2 )
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
