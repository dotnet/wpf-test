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
    /// <area>Animation.PropertyMethodEvent.Regressions</area>
    /// <priority>2</priority>
    /// <description>
    /// Regression Test "MIL (Animation): animation events fail to fire for some animations when the animating object is outside the visible area"
    /// Scenario:  event firing when the animated element is outside the rendering area.
    /// </description>
    /// </summary>
    [Test(2, "Animation.PropertyMethodEvent.Regressions", "AnimationOutsideTest")]
    public class AnimationOutsideTest : WindowTest
    {
        #region Test case members

        private DispatcherTimer                 _aTimer          = null;
        private PointAnimation                  _pointAnim;
        private EllipseGeometry                 _EG;
        private int                             _stateCount      = 0;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          AnimationOutside1Test Constructor
        ******************************************************************************/
        public AnimationOutsideTest()
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
            body.Background = Brushes.SpringGreen;
            body.Height     = 200d;
            body.Width      = 200d;

            Path path = new Path();
            path.Fill               = Brushes.Blue;
            path.Stroke             = Brushes.Black;
            path.StrokeThickness    = 5d;
            
            _EG = new EllipseGeometry();
            _EG.Center       = new Point(300, 300);
            _EG.RadiusX      = 25d;
            _EG.RadiusY      = 50d;

            _pointAnim = new PointAnimation();
            _pointAnim.From                      = new Point(300, 300);
            _pointAnim.To                        = new Point(250, 250);
            _pointAnim.Duration                  = new Duration(TimeSpan.FromMilliseconds(750));
            _pointAnim.BeginTime                 = TimeSpan.FromMilliseconds(0);;
            _pointAnim.CurrentStateInvalidated  += new EventHandler(OnCurrentStateInvalidated);

            path.Data = _EG;
            body.Children.Add(path);

            Window.Content = body;
            
            return TestResult.Pass;
        }

        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----CurrentStateInvalidated: " + ((Clock)sender).CurrentState);
            _stateCount++;
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
                _EG.BeginAnimation(EllipseGeometry.CenterProperty, _pointAnim);
            }
            else if (_tickCount == 3)
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

            int actValue = _stateCount;
            int expValue = 2;

            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Count:     " + expValue);
            GlobalLog.LogEvidence("Actual Count:       " + actValue);
            
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
