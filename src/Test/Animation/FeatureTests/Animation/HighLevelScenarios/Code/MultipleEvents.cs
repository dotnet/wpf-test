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
    /// <area>Animation.HighLevelScenarios.Regressions</area>
	/// Scenario: the same event handler used for multiple animations.


    [Test(2, "Animation.HighLevelScenarios.Regressions", "MultipleEventsTest")]
    public class MultipleEventsTest : WindowTest
    {
        #region Test case members

        private string                          _inputString         = "";
        private TextBox                         _textbox1            = null;
        private SolidColorBrush                 _SCB                 = null;
        private RadialGradientBrush             _RGB                 = null;
        private DoubleAnimation                 _animDouble;
        private ColorAnimation                  _animColor;
        private PointAnimation                  _animPoint;
        private AnimationClock                  _clockDouble         = null;
        private AnimationClock                  _clockColor          = null;
        private AnimationClock                  _clockPoint          = null;
        private TimeSpan                        _BEGIN_TIME          = TimeSpan.FromSeconds(0);
        private TimeSpan                        _DURATION_TIME       = TimeSpan.FromSeconds(2);
        private DispatcherTimer                 _aTimer              = null;
        private int                             _tickCount           = 0;
        private int[]                           _eventDoubleCount    = new int[5];
        private int[]                           _eventColorCount     = new int[5];
        private int[]                           _eventPointCount     = new int[5];
        
        #endregion


        #region Constructor

        [Variation("Animation", Priority = 0, Keywords = "MicroSuite")]
        [Variation("Clock")]
        
        /******************************************************************************
        * Function:          MultipleEventsTest Constructor
        ******************************************************************************/
        public MultipleEventsTest(string testValue)
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
            body.Background = Brushes.MistyRose;
            body.Height     = 300d;
            body.Width      = 300d;

            _textbox1 = new TextBox();
            body.Children.Add(_textbox1);
            _textbox1.Text           = "Avalon!";
            _textbox1.FontSize       = 48d;
            _textbox1.Width          = 200d;

            _SCB = new SolidColorBrush(Colors.MidnightBlue);
            _textbox1.Foreground = _SCB;

            _RGB = new RadialGradientBrush();
            _RGB.GradientStops.Add(new GradientStop(Colors.LightBlue, 0.0));
            _RGB.GradientStops.Add(new GradientStop(Colors.Lavender,  0.2));
            _RGB.GradientStops.Add(new GradientStop(Colors.Purple,    0.8));
            _RGB.GradientOrigin  = new Point(0.0, 0.0);
            _textbox1.Background = _RGB;

            CreateColorAnimation();         //Animate the Foreground of the TextBox.
            CreatePointAnimation();         //Animate the Background of the TextBox.
            CreateDoubleAnimation();        //Animate the Width of the TextBox.

            Window.Content = body;
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          CreateDoubleAnimation
        ******************************************************************************/
        /// <summary>
        /// Create a DoubleAnimation and its AnimationClock.
        /// </summary>
        /// <returns></returns>
        private void CreateDoubleAnimation()
        {
            _animDouble = new DoubleAnimation();
            _animDouble.Name             = "DoubleAnimation";
            _animDouble.By               = 50d;
            _animDouble.BeginTime        = _BEGIN_TIME;
            _animDouble.Duration         = new Duration(_DURATION_TIME);

            if (_inputString == "Animation")
            {
                _animDouble = (DoubleAnimation)AttachEventsToAnimation(_animDouble);
            }

            _clockDouble = _animDouble.CreateClock();

            if (_inputString == "Clock")
            {
                AttachEventsToClock(ref _clockDouble);
            }
        }

        /******************************************************************************
        * Function:          CreateColorAnimation
        ******************************************************************************/
        /// <summary>
        /// Create a ColorAnimation and its AnimationClock.
        /// </summary>
        /// <returns></returns>
        private void CreateColorAnimation()
        {
            _animColor = new ColorAnimation();
            _animColor.Name          = "ColorAnimation";
            _animColor.To            = Colors.HotPink;
            _animColor.BeginTime     = _BEGIN_TIME;
            _animColor.Duration      = new Duration(_DURATION_TIME);

            if (_inputString == "Animation")
            {
                _animColor = (ColorAnimation)AttachEventsToAnimation(_animColor);
            }

            _clockColor = _animColor.CreateClock();

            if (_inputString == "Clock")
            {
                AttachEventsToClock(ref _clockColor);
            }
        }

        /******************************************************************************
        * Function:          CreatePointAnimation
        ******************************************************************************/
        /// <summary>
        /// Create a PointAnimation and its AnimationClock.
        /// </summary>
        /// <returns></returns>
        private void CreatePointAnimation()
        {
            _animPoint = new PointAnimation();
            _animPoint.Name          = "PointAnimation";
            _animPoint.By            = new Point(0.8, 0.8);
            _animPoint.BeginTime     = _BEGIN_TIME;
            _animPoint.Duration      = new Duration(_DURATION_TIME);

            if (_inputString == "Animation")
            {
                _animPoint = (PointAnimation)AttachEventsToAnimation(_animPoint);
            }

            _clockPoint = _animPoint.CreateClock();

            if (_inputString == "Clock")
            {
                AttachEventsToClock(ref _clockPoint);
            }
        }

        /******************************************************************************
        * Function:          AttachEventsToAnimation
        ******************************************************************************/
        private AnimationTimeline AttachEventsToAnimation(AnimationTimeline animationTimeline)
        {
            animationTimeline.CurrentStateInvalidated          += new EventHandler(OnCurrentState);
            animationTimeline.CurrentTimeInvalidated           += new EventHandler(OnCurrentTime);
            animationTimeline.CurrentGlobalSpeedInvalidated    += new EventHandler(OnCurrentGlobalSpeed);
            animationTimeline.Completed                        += new EventHandler(OnCompleted);
            animationTimeline.RemoveRequested                  += new EventHandler(OnRemoveRequested);

            return animationTimeline;
        }

        /******************************************************************************
        * Function:          AttachEventsToClock
        ******************************************************************************/
        private void AttachEventsToClock(ref AnimationClock animationClock)
        {
            animationClock.CurrentStateInvalidated          += new EventHandler(OnCurrentState);
            animationClock.CurrentTimeInvalidated           += new EventHandler(OnCurrentTime);
            animationClock.CurrentGlobalSpeedInvalidated    += new EventHandler(OnCurrentGlobalSpeed);
            animationClock.Completed                        += new EventHandler(OnCompleted);
            animationClock.RemoveRequested                  += new EventHandler(OnRemoveRequested);
        }

        /******************************************************************************
        * Function:          OnCurrentState
        ******************************************************************************/
        private void OnCurrentState(object sender, EventArgs args)
        {
            RecordEvents((Clock)sender, "CurrentState");
        }

        /******************************************************************************
        * Function:          OnCurrentTime
        ******************************************************************************/
        private void OnCurrentTime(object sender, EventArgs args)
        {
            RecordEvents((Clock)sender, "CurrentTime");
        }

        /******************************************************************************
        * Function:          OnCurrentGlobalSpeed
        ******************************************************************************/
        private void OnCurrentGlobalSpeed(object sender, EventArgs args)
        {
            RecordEvents((Clock)sender, "CurrentGlobalSpeed");
        }

        /******************************************************************************
        * Function:          OnCompleted
        ******************************************************************************/
        private void OnCompleted(object sender, EventArgs args)
        {
            RecordEvents((Clock)sender, "Completed");
        }

        /******************************************************************************
        * Function:          OnRemoveRequested
        ******************************************************************************/
        private void OnRemoveRequested(object sender, EventArgs args)
        {
            RecordEvents((Clock)sender, "RemoveRequested");
        }

        /******************************************************************************
        * Function:          RecordEvents
        ******************************************************************************/
        /// <summary>
        /// Keeps track of event firing for each animation and each type of event.
        /// Each clock has an associated int array containing the number of times each of
        /// the five events fire.
        /// </summary>
        /// <returns></returns>
        private void RecordEvents(Clock clock, string eventFired)
        {
            string name = clock.Timeline.Name;
            
            //if (eventFired != "CurrentTime")
            //    GlobalLog.LogStatus("----" + eventFired + ":\t\t" + name);
            
            switch (name)
            {
                case "DoubleAnimation" :
                    IncrementCount(ref _eventDoubleCount, eventFired);
                    break;
                case "ColorAnimation" :
                    IncrementCount(ref _eventColorCount, eventFired);
                    break;
                case "PointAnimation" :
                    IncrementCount(ref _eventPointCount, eventFired);
                    break;
            }
        }

        /******************************************************************************
        * Function:          IncrementCount
        ******************************************************************************/
        private void IncrementCount(ref int[] count, string animationEvent)
        {
            switch (animationEvent)
            {
                case "CurrentState" :
                    count[0]++;
                    break;
                case "CurrentTime" :
                    count[1]++;
                    break;
                case "CurrentGlobalSpeed" :
                    count[2]++;
                    break;
                case "Completed" :
                    count[3]++;
                    break;
                case "RemoveRequested" :
                    count[4]++;
                    break;
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,500);
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
                GlobalLog.LogStatus("--------Begin");
                _textbox1.ApplyAnimationClock(FrameworkElement.WidthProperty, _clockDouble);
                _SCB.ApplyAnimationClock(SolidColorBrush.ColorProperty, _clockColor);
                _RGB.ApplyAnimationClock(RadialGradientBrush.GradientOriginProperty, _clockPoint);

            }
            else if (_tickCount == 2)
            {
                GlobalLog.LogStatus("--------Seek");
                _clockDouble.Controller.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
                _clockColor.Controller.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
                _clockPoint.Controller.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 3)
            {
                GlobalLog.LogStatus("--------Pause");
                _clockDouble.Controller.Pause();
                _clockColor.Controller.Pause();
                _clockPoint.Controller.Pause();
            }
            else if (_tickCount == 4)
            {
                GlobalLog.LogStatus("--------Resume");
                _clockDouble.Controller.Resume();
                _clockColor.Controller.Resume();
                _clockPoint.Controller.Resume();
            }
            else if (_tickCount == 5)
            {
                GlobalLog.LogStatus("--------SkipToFill");
                _clockDouble.Controller.SkipToFill();
                _clockColor.Controller.SkipToFill();
                _clockPoint.Controller.SkipToFill();
            }
            else if (_tickCount == 6)
            {
                GlobalLog.LogStatus("--------Stop");
                _clockDouble.Controller.Stop();
                _clockColor.Controller.Stop();
                _clockPoint.Controller.Stop();
            }
            else if (_tickCount == 7)
            {
                GlobalLog.LogStatus("--------SeekAlignedToLastTick");
                _clockDouble.Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(500), TimeSeekOrigin.BeginTime);
                _clockColor.Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(500), TimeSeekOrigin.BeginTime);
                _clockPoint.Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(500), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 8)
            {
                GlobalLog.LogStatus("--------Remove");
                _clockDouble.Controller.Remove();
                _clockColor.Controller.Remove();
                _clockPoint.Controller.Remove();
            }
            else
            {
                _aTimer.Stop();
                CheckResults();
            }
        }

        /******************************************************************************
        * Function:          CheckResults
        ******************************************************************************/
        /// <summary>
        /// Compares expected to actual values.
        /// </summary>
        /// <returns></returns>
        private void CheckResults()
        {
            int expState    = 6;
            int expSpeed    = 8;
            int expCompeted = 2;
            int expRemove   = 1;
            
            GlobalLog.LogEvidence("---------EXPECTED COUNTS---------");
            GlobalLog.LogEvidence("----State:     " + _eventDoubleCount[0]);
            GlobalLog.LogEvidence("----Time: not checked");
            GlobalLog.LogEvidence("----Speed:     " + _eventDoubleCount[2]);
            GlobalLog.LogEvidence("----Completed: " + _eventDoubleCount[3]);
            GlobalLog.LogEvidence("----Remove:    " + _eventDoubleCount[4]);
            GlobalLog.LogEvidence("---------------------------------");

            GlobalLog.LogEvidence("----------ACTUAL COUNTS----------");
            GlobalLog.LogEvidence("----Double / State:     " + _eventDoubleCount[0]);
            GlobalLog.LogEvidence("----Double / Time:      " + _eventDoubleCount[1]);
            GlobalLog.LogEvidence("----Double / Speed:     " + _eventDoubleCount[2]);
            GlobalLog.LogEvidence("----Double / Completed: " + _eventDoubleCount[3]);
            GlobalLog.LogEvidence("----Double / Remove:    " + _eventDoubleCount[4]);
            GlobalLog.LogEvidence("---------------------------------");

            GlobalLog.LogEvidence("----Color / State:     " + _eventColorCount[0]);
            GlobalLog.LogEvidence("----Color / Time:      " + _eventColorCount[1]);
            GlobalLog.LogEvidence("----Color / Speed:     " + _eventColorCount[2]);
            GlobalLog.LogEvidence("----Color / Completed: " + _eventColorCount[3]);
            GlobalLog.LogEvidence("----Color / Remove:    " + _eventColorCount[4]);
            GlobalLog.LogEvidence("---------------------------------");

            GlobalLog.LogEvidence("----Point / State:     " + _eventPointCount[0]);
            GlobalLog.LogEvidence("----Point / Time:      " + _eventPointCount[1]);
            GlobalLog.LogEvidence("----Point / Speed:     " + _eventPointCount[2]);
            GlobalLog.LogEvidence("----Point / Completed: " + _eventPointCount[3]);
            GlobalLog.LogEvidence("----Point / Remove:    " + _eventPointCount[4]);
            GlobalLog.LogEvidence("---------------------------------");

            //CurrentStateInvalidated.
            bool b0 = (_eventDoubleCount[0] == expState    && _eventColorCount[0] == expState    && _eventPointCount[0] == expState );

            //CurrentTimeInvalidated: checking that the event fired the same for each animation,
            //rather than for actual times fired.
            bool b1 = (_eventDoubleCount[1] == _eventColorCount[1] && _eventColorCount[1] == _eventPointCount[1] );

            //CurrentGlobalSpeedInvalidated.
            bool b2 = (_eventDoubleCount[2] == expSpeed    && _eventColorCount[2] == expSpeed    && _eventPointCount[2] == expSpeed );

            //Completed.
            bool b3 = (_eventDoubleCount[3] == expCompeted && _eventColorCount[3] == expCompeted && _eventPointCount[3] == expCompeted );

            //RemoveRequested.
            bool b4 = (_eventDoubleCount[4] == expRemove   && _eventColorCount[4] == expRemove   && _eventPointCount[4] == expRemove );

            GlobalLog.LogEvidence("-----------RESULTS-----------");
            GlobalLog.LogEvidence(b0 + " / " + b1 + " / " + b2 + " / " + b3 + " / " + b4);

            if (b0 && b1 && b2 && b3 && b4)
            {
                Signal("AnimationDone", TestResult.Pass);
            }
            else
            {
                Signal("AnimationDone", TestResult.Fail);
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
            return WaitForSignal("AnimationDone");
        }

        #endregion
    }
}
