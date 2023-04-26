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
using System.Windows.Input;
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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "MouseEventsOnPathTest")]
    public class MouseEventsOnPathTest : WindowTest
    {
        #region Test case members

        private Canvas              _canvas1;
        private Path                _path1;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        private bool                _redClicked          = false;
        private bool                _orangeClicked       = false;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          MouseEventsOnPathTest Constructor
        ******************************************************************************/
        public MouseEventsOnPathTest()
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
            Window.Height       = 500d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Window.Content = Content();
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          Content
        ******************************************************************************/
        /// <summary>
        /// Creates the animated elements and their Animations.
        /// </summary>
        /// <returns>Returns a UIElement containing the animated elements</returns>
        public UIElement Content()
        {
            Canvas body = new Canvas();
         
            _canvas1 = new Canvas();
            _canvas1.Width = 80;
            _canvas1.Height = 80;
            _canvas1.Background = Brushes.Red;
            Canvas.SetTop(_canvas1, 150);
            _canvas1.MouseLeftButtonDown += new MouseButtonEventHandler(CanvasClicked);
            
            DoubleAnimation da = new DoubleAnimation(20, 100, new Duration(TimeSpan.FromMilliseconds(30000)));
            da.FillBehavior = FillBehavior.HoldEnd;
            _canvas1.BeginAnimation(Canvas.LeftProperty, da);
            
            _path1 = new Path();
            _path1.Fill = Brushes.Orange;
            _path1.MouseLeftButtonDown += new MouseButtonEventHandler(Path1Clicked);

            EllipseGeometry eg1 = new EllipseGeometry(new Point(10, 10), 35, 35);
            PointAnimation pa1 = new PointAnimation(new Point(0, 0), new Point(600, 200), new Duration(TimeSpan.FromMilliseconds(30000)));
            pa1.FillBehavior = FillBehavior.HoldEnd;
            eg1.BeginAnimation(EllipseGeometry.CenterProperty, pa1);

            _path1.Data = eg1;

            body.Children.Add(_canvas1);
            body.Children.Add(_path1);

            return body;
        }

        private void CanvasClicked(object sender, MouseButtonEventArgs e)
        {
            GlobalLog.LogStatus("---Red Canvas Clicked---");
            _redClicked = true;
        }

        private void Path1Clicked(object sender, MouseButtonEventArgs e)
        {
            GlobalLog.LogStatus("---Orange Path Clicked---");
            _orangeClicked = true;
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult</returns>
        TestResult StartTimer()
        {
            GlobalLog.LogStatus("---The animated element was found---");

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,750);
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
                UserInput.MouseLeftDown(_canvas1);
            }
            else if (_tickCount == 2)
            {
                UserInput.MouseLeftDown(_path1);
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

            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Red Clicked:    " + _redClicked);
            GlobalLog.LogEvidence("Orange Clicked: " + _orangeClicked);
            
            if (_redClicked && _orangeClicked)
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
