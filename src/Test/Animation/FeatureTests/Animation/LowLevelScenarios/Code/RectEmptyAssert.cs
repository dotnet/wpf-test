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

    [Test(2, "Animation.LowLevelScenarios.Regressions", "RectEmptyAssertTest")]
    public class RectEmptyAssertTest : WindowTest
    {

        #region Test case members

        private DispatcherTimer     _aTimer          = null;
        private RectangleGeometry   _rectGeometry;
        private AnimationClock      _clock1;

        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          RectEmptyAssertTest Constructor
        ******************************************************************************/
        public RectEmptyAssertTest()
        {
            RunSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(StartAnimation);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the page content, an Animation, and a corresponding Clock.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult CreateTree()
        {
            Window.Width        = 350d;
            Window.Height       = 600d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body  = new Canvas();
            Window.Content = body;

            _rectGeometry = new RectangleGeometry();
            _rectGeometry.RadiusX        = 100d;
            _rectGeometry.RadiusY        = 100d;

            Path path = new Path();
            path.Data               = _rectGeometry;
            path.StrokeThickness    = 2;
            path.Stroke             = Brushes.Black;
            body.Children.Add(path);

            RectAnimation anim = new RectAnimation();
            anim.To              = new Rect( 10, 10, 100, 100 );
            anim.Duration        = new Duration(TimeSpan.FromMilliseconds(2000));
            anim.BeginTime       = TimeSpan.FromMilliseconds(0);
            anim.RepeatBehavior  = RepeatBehavior.Forever;
            anim.AutoReverse     = true;

            _clock1 = anim.CreateClock();

            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns></returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1500);
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
            _aTimer.Stop();
            Signal("AnimationDone", TestResult.Pass);
        }
        
        /******************************************************************************
        * Function:          StartAnimation
        ******************************************************************************/
        /// <summary>
        /// Starts the Animation, which should throw an exception.
        /// </summary>
        /// <returns></returns>
        private TestResult StartAnimation()
        {
            SetExpectedErrorTypeInStep(typeof(System.InvalidOperationException), "Inner");
            _rectGeometry.ApplyAnimationClock(RectangleGeometry.RectProperty, _clock1);
            
            WaitForSignal("AnimationDone");

            return TestResult.Pass;
        }

        #endregion
    }
}
