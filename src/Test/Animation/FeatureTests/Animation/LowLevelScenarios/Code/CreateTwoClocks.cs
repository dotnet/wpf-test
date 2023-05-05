// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Navigation;
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

    [Test(2, "Animation.LowLevelScenarios.Regressions", "CreateTwoClocksTest")]

    class CreateTwoClocksTest : WindowTest
    {
        #region Test case members

        private string                          _inputString = "";
        private DispatcherTimer                 _aTimer      = null;
        private Canvas                          _body        = null;
        private Point                           _baseValue   = new Point(200, 200);
        private Point                           _toValue     = new Point(300, 300);
        private ArcSegment                      _AS          = null;
        private AnimationClock                  _AC          = null;

        #endregion


        #region Constructor

        [Variation("TwoClocks")]
        [Variation("TwoApplys")]
        [Variation("TwoClocksAndApplys")]

        /******************************************************************************
        * Function:          CreateTwoClocksTest Constructor
        ******************************************************************************/
        public CreateTwoClocksTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: create a new Window and add content to it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("---Initialize---");

            Window.Left             = 0d;
            Window.Top              = 0d;
            Window.Height           = 400d;
            Window.Width            = 400d;
            Window.WindowStyle      = WindowStyle.None;

            _body  = new Canvas();
            Window.Content = _body;
            _body.Width              = 400d;
            _body.Height             = 400d;
            _body.Background         = Brushes.Navy;
            Canvas.SetTop  (_body, 0d);
            Canvas.SetLeft (_body, 0d);

            Path path1 = new Path();
            path1.Fill                = Brushes.MediumSeaGreen;
            path1.StrokeThickness     = 5;
            path1.Stroke              = Brushes.LawnGreen;

            _AS = new ArcSegment ();     
            _AS.IsLargeArc       = true;
            _AS.Size             = new Size(50, 50);
            _AS.SweepDirection   = SweepDirection.Counterclockwise;
            _AS.RotationAngle    = 50;
            _AS.Point            = _baseValue;

            PathFigure PF1 = new PathFigure ();
            PF1.StartPoint = new Point(30, 30);
            PF1.Segments.Add(_AS);

            PathGeometry PG = new PathGeometry ();
            PG.Figures.Add(PF1);

            path1.Data = PG;
            _body.Children.Add(path1);

            GlobalLog.LogStatus("---Window created---");

            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns success if the elements were found</returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,3000);
            _aTimer.Start();
            GlobalLog.LogEvidence("---DispatcherTimer Started---");

            PointAnimation animPoint = new PointAnimation();
            animPoint.To                     = _toValue;
            animPoint.BeginTime              = TimeSpan.FromMilliseconds(0);
            animPoint.Duration               = new Duration(TimeSpan.FromMilliseconds(1500));

            switch (_inputString)
            {
                case "TwoClocks" :
                    _AC = animPoint.CreateClock();
                    _AC = animPoint.CreateClock();
                    _AS.ApplyAnimationClock(ArcSegment.PointProperty, _AC);
                    break;
                case "TwoApplys" :
                    _AC = animPoint.CreateClock();
                    _AS.ApplyAnimationClock(ArcSegment.PointProperty, _AC);
                    _AS.ApplyAnimationClock(ArcSegment.PointProperty, _AC);
                    break;
                case "TwoClocksAndApplys" :
                    _AC = animPoint.CreateClock();
                    _AC = animPoint.CreateClock();
                    _AS.ApplyAnimationClock(ArcSegment.PointProperty, _AC);
                    _AS.ApplyAnimationClock(ArcSegment.PointProperty, _AC);
                    break;
            }

            return TestResult.Pass;
        }

        /******************************************************************************
           * Function:          OnTick
           ******************************************************************************/
        /// <summary>
        /// OnTick: Signals that the test is finished, and that verfication can begin.
        /// </summary>
        private void OnTick(object sender, EventArgs e)
        {
            _aTimer.Stop();
            Signal("TestFinished", TestResult.Pass);
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            Point actValue = (Point)_AS.GetValue(ArcSegment.PointProperty);
            Point expValue = _toValue;
            
            GlobalLog.LogEvidence("---Verifying the Animation---");
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
