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
using Microsoft.Test.Input;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "MessageBoxInEventTest")]
    public class MessageBoxInEventTest : WindowTest
    {
        #region Test case members

        private Canvas                          _body            = null;
        private DrawingBrush                    _drawBrush       = null;
        private LineGeometry                    _lineGeometry    = null;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        private Point                           _toValue         = new Point(0d, 5d);
        private Button                          _button1         = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          MessageBoxInEvent1Test Constructor
        ******************************************************************************/
        public MessageBoxInEventTest()
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

            _body = new Canvas();
            _body.Background = Brushes.Lavender;
            
            _lineGeometry = new LineGeometry();
            _lineGeometry.StartPoint = new Point(0d, 0.2d);
            _lineGeometry.EndPoint   = new Point(1d, 0.2d);

            GeometryDrawing geoDrawing = new GeometryDrawing();
            geoDrawing.Pen = new Pen( Brushes.HotPink, 3d );
            geoDrawing.Geometry = _lineGeometry;

            _drawBrush = new DrawingBrush();
            _drawBrush.Drawing = geoDrawing;
            
            _button1 = new Button();
            _body.Children.Add(_button1);
            _button1.Content     = "Avalon!";
            _button1.FontSize    = 36d;
            _button1.Background  = _drawBrush;
            Canvas.SetTop  (_button1, 10d);
            Canvas.SetLeft (_button1, 10d);

            Window.Content = _body;

            PointAnimation pointAnim = new PointAnimation();
            pointAnim.From                      = new Point(0d, 0.2d);
            pointAnim.To                        = _toValue;
            pointAnim.Duration                  = new Duration(TimeSpan.FromMilliseconds(1500));
            pointAnim.BeginTime                 = TimeSpan.FromMilliseconds(0);
            pointAnim.CurrentStateInvalidated  += new EventHandler(OnCurrentStateInvalidated);
            
            _lineGeometry.BeginAnimation(LineGeometry.StartPointProperty, pointAnim);

            return TestResult.Pass;
        }

        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            AnimationClock AC = (AnimationClock)((Clock)sender);
            GlobalLog.LogStatus("---CurrentStateInvalidated: " + AC.CurrentState);
            if (AC.CurrentState == ClockState.Filling)
            {
                MessageBox.Show("Stop");

                Button button2 = new Button();
                _body.Children.Add(button2);
                button2.Content     = "WPF!";
                button2.FontSize    = 36d;
                button2.Background  = _drawBrush;
                Canvas.SetTop  (button2, 150d);
                Canvas.SetLeft (button2, 150d);
            }
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
            _aTimer.Start();
            
            GlobalLog.LogStatus("---DispatcherTimer Started---");
            
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
                UserInput.MouseLeftClickCenter(_body);
                UserInput.KeyPress("Enter", true);
                UserInput.KeyPress("Enter", false);
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

            Point actValue = (Point)_lineGeometry.CloneCurrentValue().StartPoint;
            Point expValue = _toValue;

            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected :     " + expValue);
            GlobalLog.LogEvidence("Actual :       " + actValue);
            
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
