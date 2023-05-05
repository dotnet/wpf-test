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
    /// <area>Storyboards.LowLevelScenarios.Parameterless</area>
    /// <priority>1</priority>
    /// <description>
    /// Verifying events fire correctly when a Storyboard's "parameterless" methods are invoked.
    /// </description>
    /// </summary>
    [Test(1, "Storyboards.LowLevelScenarios.Parameterless", "ParameterlessAllMethodsTest")]
    public class ParameterlessAllMethodsTest : WindowTest
    {
        #region Test case members

        private TextBlock                       _textBlock1          = null;
        private SolidColorBrush                 _SCB                 = null;
        private RadialGradientBrush             _RGB                 = null;
        private Storyboard                      _storyboard1;
        private Storyboard                      _storyboard2;
        private Storyboard                      _storyboard3;
        private TimeSpan                        _BEGIN_TIME          = TimeSpan.FromSeconds(0);
        private TimeSpan                        _DURATION_TIME       = TimeSpan.FromSeconds(2);
        private DispatcherTimer                 _aTimer              = null;
        private int                             _tickCount           = 0;
        private int[]                           _eventDoubleCount    = new int[5];
        private int[]                           _eventColorCount     = new int[5];
        private int[]                           _eventPointCount     = new int[5];
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ParameterlessAllMethodsTest Constructor
        ******************************************************************************/
        public ParameterlessAllMethodsTest()
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
            body.Background = Brushes.DodgerBlue;
            body.Height     = 300d;
            body.Width      = 300d;

            _textBlock1 = new TextBlock();
            body.Children.Add(_textBlock1);
            _textBlock1.Text           = "Avalon!";
            _textBlock1.FontSize       = 48d;
            _textBlock1.Width          = 200d;

            _SCB = new SolidColorBrush(Colors.MidnightBlue);
            _textBlock1.Foreground = _SCB;

            _RGB = new RadialGradientBrush();
            _RGB.GradientStops.Add(new GradientStop(Colors.LightBlue, 0.0));
            _RGB.GradientStops.Add(new GradientStop(Colors.Lavender,  0.2));
            _RGB.GradientStops.Add(new GradientStop(Colors.Purple,    0.8));
            _RGB.GradientOrigin  = new Point(0.0, 0.0);
            _textBlock1.Background = _RGB;

            ColorAnimation animColor = CreateColorAnimation();    //Animate the Foreground of the TextBlock.
            PointAnimation animPoint = CreatePointAnimation();    //Animate the Background of the TextBlock.
            DoubleAnimation animDouble = CreateDoubleAnimation(); //Animate the Width of the TextBlock.

            _storyboard1 = new Storyboard();
            _storyboard2 = new Storyboard();
            _storyboard3 = new Storyboard();

            _storyboard1.Name = "ColorAnimation";
            _storyboard2.Name = "PointAnimation";
            _storyboard3.Name = "DoubleAnimation";

            AttachEvents(_storyboard1);
            AttachEvents(_storyboard2);
            AttachEvents(_storyboard3);

            _storyboard1.Children.Add(animColor);
            _storyboard2.Children.Add(animPoint);
            _storyboard3.Children.Add(animDouble);

            PropertyPath path1 = new PropertyPath("(0).(1)", TextBlock.ForegroundProperty, SolidColorBrush.ColorProperty);
            PropertyPath path2 = new PropertyPath("(0).(1)", TextBlock.BackgroundProperty, RadialGradientBrush.GradientOriginProperty);
            PropertyPath path3 = new PropertyPath("(0)", TextBlock.WidthProperty);

            Storyboard.SetTargetProperty(_storyboard1, path1);
            Storyboard.SetTargetProperty(_storyboard2, path2);
            Storyboard.SetTargetProperty(_storyboard3, path3);

            Storyboard.SetTarget(_storyboard1, _textBlock1);
            Storyboard.SetTarget(_storyboard2, _textBlock1);
            Storyboard.SetTarget(_storyboard3, _textBlock1);

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
        private DoubleAnimation CreateDoubleAnimation()
        {
            DoubleAnimation animDouble = new DoubleAnimation();
            animDouble.Name             = "DoubleAnimation";
            animDouble.By               = 50d;
            animDouble.BeginTime        = _BEGIN_TIME;
            animDouble.Duration         = new Duration(_DURATION_TIME);

            return animDouble;
        }

        /******************************************************************************
        * Function:          CreateColorAnimation
        ******************************************************************************/
        /// <summary>
        /// Create a ColorAnimation and its AnimationClock.
        /// </summary>
        /// <returns></returns>
        private ColorAnimation CreateColorAnimation()
        {
            ColorAnimation animColor = new ColorAnimation();
            animColor.Name          = "ColorAnimation";
            animColor.To            = Colors.HotPink;
            animColor.BeginTime     = _BEGIN_TIME;
            animColor.Duration      = new Duration(_DURATION_TIME);

            return animColor;
        }

        /******************************************************************************
        * Function:          CreatePointAnimation
        ******************************************************************************/
        /// <summary>
        /// Create a PointAnimation and its AnimationClock.
        /// </summary>
        /// <returns></returns>
        private PointAnimation CreatePointAnimation()
        {
            PointAnimation animPoint = new PointAnimation();
            animPoint.Name          = "PointAnimation";
            animPoint.By            = new Point(0.8, 0.8);
            animPoint.BeginTime     = _BEGIN_TIME;
            animPoint.Duration      = new Duration(_DURATION_TIME);

            return animPoint;
        }

        /******************************************************************************
        * Function:          AttachEventsToStoryboard
        ******************************************************************************/
        private void AttachEvents(Storyboard story)
        {
            story.CurrentStateInvalidated          += new EventHandler(OnCurrentState);
            story.CurrentTimeInvalidated           += new EventHandler(OnCurrentTime);
            story.CurrentGlobalSpeedInvalidated    += new EventHandler(OnCurrentGlobalSpeed);
            story.Completed                        += new EventHandler(OnCompleted);
            story.RemoveRequested                  += new EventHandler(OnRemoveRequested);
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
          
            switch (name)
            {
                case "ColorAnimation" :
                    IncrementCount(ref _eventColorCount, eventFired);
                    break;
                case "PointAnimation" :
                    IncrementCount(ref _eventPointCount, eventFired);
                    break;
                case "DoubleAnimation" :
                    IncrementCount(ref _eventDoubleCount, eventFired);
                    break;
                default:
                    throw new TestValidationException("ERROR!!! The 2nd argument to RecordEvents is incorrect.");
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
                default:
                    throw new TestValidationException("ERROR!!! The 2nd argument to IncrementCount is incorrect.");
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
                _storyboard1.Begin();
                _storyboard2.Begin();
                _storyboard3.Begin();
            }
            else if (_tickCount == 2)
            {
                GlobalLog.LogStatus("--------Seek");
                _storyboard1.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
                _storyboard2.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
                _storyboard3.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 3)
            {
                GlobalLog.LogStatus("--------Pause");
                _storyboard1.Pause();
                _storyboard2.Pause();
                _storyboard3.Pause();
            }
            else if (_tickCount == 4)
            {
                GlobalLog.LogStatus("--------Resume");
                _storyboard1.Resume();
                _storyboard2.Resume();
                _storyboard3.Resume();
            }
            else if (_tickCount == 5)
            {
                GlobalLog.LogStatus("--------SkipToFill");
                _storyboard1.SkipToFill();
                _storyboard2.SkipToFill();
                _storyboard3.SkipToFill();
            }
            else if (_tickCount == 6)
            {
                GlobalLog.LogStatus("--------Stop");
                _storyboard1.Stop();
                _storyboard2.Stop();
                _storyboard3.Stop();
            }
            else if (_tickCount == 7)
            {
                GlobalLog.LogStatus("--------SeekAlignedToLastTick");
                _storyboard1.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(500), TimeSeekOrigin.BeginTime);
                _storyboard2.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(500), TimeSeekOrigin.BeginTime);
                _storyboard3.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(500), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 8)
            {
                GlobalLog.LogStatus("--------Remove");
                _storyboard1.Remove();
                _storyboard2.Remove();
                _storyboard3.Remove();
            }
            else
            {
                GlobalLog.LogStatus("--------CheckResults");
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
