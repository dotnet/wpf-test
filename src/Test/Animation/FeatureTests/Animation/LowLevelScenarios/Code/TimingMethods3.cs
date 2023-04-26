// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Methods in Events: Timing Methods *****************
*   Description:
*          Tests that all Timing-level Methods can be called via all Animation Events, using a 
*          Storyboard.  The input parameters define all combinations of Methods x Events:
*          (only three events are available: CurrentStateInvalidated,
*          CurrentGlobalSpeedInvalidated, CurrentTimeInvalidated.  The old events are therefore
*          defined by checking certain properties via these three events.)
*               Events  (3):  CurrentStateInvalidated,CurrentGlobalSpeedInvalidated, CurrentTimeInvalidated.
*               Methods (6):  Begin(), Pause(), Seek(), Resume(), Deactivate(), Stop().
*          The events are bound to the Clock, rather than the animation.
*          This version sets ClockController.Seek(TimeSpan.FromMilliseconds(100) andTimeSeekOrigin.BeginTime)).
*   Pass Conditions:
*          The test case will Pass if the events fire appropriately when the methods are invoked.
*
*   Framework:          A CLR executable is created.
*   Area:               Animation/Timing
*   Dependencies:       TestRuntime.dll
*   Support Files:
* *******************************************************/
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// <area>Animation.LowLevelScenarios.Methods</area>
    /// <priority>2</priority>
    /// <description>
    /// Verify that Time-related Methods can be called via all Animation Events, using a Storyboard.
    /// </description>
    /// </summary>
    [Test(2, "Animation.LowLevelScenarios.Methods", "TimingMethods3Test")]

    class TimingMethods3Test : WindowTest
    {
        #region Test case members
        
        private Canvas                          _body;
        private TextBox                         _textbox;
        private Clock                           _TLC;
        private ParallelTimeline                _TL2;
        private Storyboard                      _storyboard;
        private BeginStoryboard                 _beginStory;

        private TimeSpan                        _BEGIN_TIME        = TimeSpan.FromMilliseconds(0);
        private TimeSpan                        _DURATION_TIME     = TimeSpan.FromMilliseconds(3000);
        private TimeSpan                        _SEEK_TIME         = TimeSpan.FromMilliseconds(750);
        private TimeSpan                        _DURATION_TIME_TL2 = TimeSpan.FromMilliseconds(5000); //Timeline used for verification.
        private string                          _expectedEvents    = "";

        private int                             _currentStateInvalidatedCalls               = 0;
        private int                             _currentTimeInvalidatedCalls                = 0;
        private int                             _currentGlobalSpeedInvalidatedCalls         = 0;

        private bool                            _methodInvoked     = false;
        private string                          _animMethod        = null;
        private string                          _animEvent         = null;
        private bool                            _testPassed        = false;
          
        #endregion


        #region Constructor

        [Variation("State", "Begin", Priority=0)]
        [Variation("State", "Pause")]
        [Variation("State", "Seek")]
        [Variation("State", "Resume")]
        [Variation("State", "Deactivate")]
        [Variation("State", "Stop")]

        [Variation("Speed", "Begin")]
        [Variation("Speed", "Pause", Priority=0)]
        [Variation("Speed", "Seek", Priority=0)]
        [Variation("Speed", "Resume")]
        [Variation("Speed", "Deactivate")]
        [Variation("Speed", "Stop")]

        [Variation("Time", "Begin")]
        [Variation("Time", "Pause")]
        [Variation("Time", "Seek")]
        [Variation("Time", "Resume", Priority=0)]
        [Variation("Time", "Deactivate")]
        [Variation("Time", "Stop")]


        /******************************************************************************
        * Function:          TimingMethods3Test Constructor
        ******************************************************************************/
        public TimingMethods3Test(string testValue1, string testValue2)
        {
            _animMethod = testValue1;
            _animEvent = testValue2;
            InitializeSteps += new TestStep(CheckInputParameters);
            RunSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Test Steps

        /******************************************************************************
        * Function:          CheckInputParameters
        ******************************************************************************/
        /// <summary>
        /// CheckInputParameters: checks for a valid input string.
        /// </summary>
        /// <returns>Returns TestResult</returns>
        TestResult CheckInputParameters()
        {
            bool     arg1Found  = false;
            bool     arg2Found  = false;
            string   errMessage = "";
            string[] expList1   = { "State", "Speed", "Time" };
            string[] expList2   = { "Begin", "Pause", "Seek", "Resume", "Deactivate", "Stop" };

            arg1Found = AnimationUtilities.CheckInputString(_animMethod, expList1, ref errMessage);
            if (errMessage != "")
            {
                GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 1st Parameter: " + errMessage);
            }
            else
            {
                arg2Found = AnimationUtilities.CheckInputString(_animEvent, expList2, ref errMessage);
                if (errMessage != "")
                {
                    GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 2nd Parameter: " + errMessage);
                }
            }

            if (arg1Found && arg2Found)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: creates a new Window and adds content to it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("--- Initialize ---");

            NameScope.SetNameScope(Window, new NameScope());

            Window.Width        = 350;
            Window.Height       = 350;
            Window.Title        = "TimingMethods3 Test";
            Window.Left         = 0;
            Window.Top          = 0;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _body = new Canvas();
            _body.Width               = 350;
            _body.Height              = 350;
            _body.Background          = Brushes.Moccasin;

            _textbox = new TextBox();
            _textbox.Name    = "TextBox1";
            _textbox.Width   = 50d;
            _body.Children.Add(_textbox);

            Window.Content = _body;

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when content is shown.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("--- OnContentRendered ---");

            CreateDoubleAnimation();
            CreateExtraTimeline();

            //Begin the verification clock and the storyboard.
            GlobalLog.LogStatus("--- Starting Timelines ---");
            _TLC.Controller.Begin();
            _beginStory.Storyboard.Begin(_textbox, HandoffBehavior.SnapshotAndReplace, true);

        }

        /******************************************************************************
        * Function:          CreateExtraTimelines
        ******************************************************************************/
        /// <summary>
        /// Add an extra ParallelTimeline for test purposes.
        /// </summary>
        private void CreateExtraTimeline()
        {
            //This Timeline is used for verification after the animation is finished.
            _TL2 = new ParallelTimeline();
            _TL2.BeginTime                = null;
            _TL2.Duration                 = new Duration(_DURATION_TIME_TL2);
            _TL2.CurrentStateInvalidated  += new EventHandler(OnCurrentStateTL2);

            _TLC = _TL2.CreateClock();

            GlobalLog.LogStatus("--- Extra Timeline Created ---");
        }

        /******************************************************************************
        * Function:          CreateDoubleAnimation
        ******************************************************************************/
        /// <summary>
        /// Create a Path element and add a RectAnimation to it.
        /// </summary>
        private void CreateDoubleAnimation()
        {
            GlobalLog.LogStatus("--- CreateDoubleAnimation ---");

            DoubleAnimation animTranslate = new DoubleAnimation();
            animTranslate.BeginTime            = _BEGIN_TIME;
            animTranslate.Duration             = new Duration(_DURATION_TIME);
            animTranslate.To                   = 5;
            animTranslate.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
            animTranslate.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            animTranslate.CurrentTimeInvalidated        += new EventHandler(OnCurrentTimeInvalidated);

            TranslateTransform translateTransform = new TranslateTransform();
            translateTransform.X     = 120;
            translateTransform.Y     = 120;
            _textbox.RenderTransform    = translateTransform;

            LinearGradientBrush LGB = new LinearGradientBrush();
            LGB.StartPoint = new Point(0.0, 0.0);
            LGB.EndPoint   = new Point(1.0, 1.0);
            LGB.MappingMode = BrushMappingMode.RelativeToBoundingBox;

            GradientStop GS1 = new GradientStop(Colors.RoyalBlue, 0.0);
            GradientStop GS2 = new GradientStop(Colors.SteelBlue, 0.2);
            GradientStop GS3 = new GradientStop(Colors.LightBlue, 1.0);
            GradientStopCollection GSC = new GradientStopCollection();
            GSC.Add(GS1);
            GSC.Add(GS2);
            GSC.Add(GS3);

            LGB.GradientStops = GSC;
            _textbox.Background = LGB;

            _storyboard = new Storyboard();
            _storyboard.Name = "story";
            _storyboard.Children.Add(animTranslate);

            PropertyPath path  = new PropertyPath("(0).(1)", TextBox.RenderTransformProperty, TranslateTransform.XProperty);
            Storyboard.SetTargetProperty(animTranslate, path);

            _beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");
            _beginStory.Storyboard   = _storyboard;
        }


        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        /// <summary>
        /// The OnCurrentStateInvalidated event handler for the DoubleAnimation.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("--- OnCurrentStateInvalidated --- " + ((Clock)sender).CurrentState);

            _currentStateInvalidatedCalls++;
            if (!_methodInvoked)
            {
                SelectMethod();
                _methodInvoked = true;
            }
        }

        /*****************************************************************************
            * Function:     OnCurrentGlobalSpeedInvalidated
            *****************************************************************************/
        /// <summary>
        /// </summary>
        /// <param>sender: the TimelineClock associated with the event</param>
        /// <returns></returns>
        public void OnCurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("--- OnCurrentGlobalSpeedInvalidated --- " + ((Clock)sender).CurrentProgress);
            _currentGlobalSpeedInvalidatedCalls++;
            if (!_methodInvoked)
            {
                GlobalLog.LogStatus("--- electMethod called --- ");
                SelectMethod();
                _methodInvoked = true;
            }
        }

        /******************************************************************************
        * Function:          OnCurrentTimeInvalidated
        ******************************************************************************/
        /// <summary>
        /// The Changed event handler for the DoubleAnimation.
        /// </summary>
        private void OnCurrentTimeInvalidated(object sender, EventArgs e)
        {
            _currentTimeInvalidatedCalls++;
            if (!_methodInvoked)
            {
                SelectMethod();
                _methodInvoked = true;
            }
        }

        /*****************************************************************************
            * Function:     SelectMethod
            *****************************************************************************/
        /// <summary>
        /// </summary>
        /// <param>Determine which ClockController method to apply, based on an input parameter</param>
        /// <returns></returns>
        public void SelectMethod()
        {
            switch (_animEvent)
            {
                 case "Begin":
                      GlobalLog.LogStatus("**Begin Invoked**");
                      _beginStory.Storyboard.Begin(_textbox, HandoffBehavior.SnapshotAndReplace, true);
                      break;
                 case "Pause":
                      GlobalLog.LogStatus("**Pause Invoked**");
                      _beginStory.Storyboard.Pause(_textbox);
                      break;
                 case "Seek":
                      GlobalLog.LogStatus("**Seek Invoked**");
                      _beginStory.Storyboard.Seek(_textbox, _SEEK_TIME, TimeSeekOrigin.BeginTime);
                      break;
                 case "Resume":
                      GlobalLog.LogStatus("**Resume Invoked**");
                      _beginStory.Storyboard.Resume(_textbox);
                      break;
                 case "Deactivate":
                      GlobalLog.LogStatus("**Deactivate Invoked**");
                      _beginStory.Storyboard.SkipToFill(_textbox);
                      break;
                 case "Stop":
                      GlobalLog.LogStatus("**Stop Invoked**");
                      _beginStory.Storyboard.Stop(_textbox);
                      break;
                 default:
                      HandleError("ERROR!!! SelectMethod: Unexpected failure to match argument.(1)");
                      break;
            }
        }
        /******************************************************************************
        * Function:          OnCurrentStateTL2
        ******************************************************************************/
        /// <summary>
        /// The Ended event handler for the verification Timeline, used to verify that events fired correctly
        /// and pass or fail the test case.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateTL2(object sender, EventArgs e)
        {

            if (((Clock)sender).CurrentState != ClockState.Active)
            {
                GlobalLog.LogStatus("--- End TestCase ---");

                GlobalLog.LogEvidence("Actual Events:");
                GlobalLog.LogEvidence("currentStateInvalidated Calls =" + _currentStateInvalidatedCalls);
                GlobalLog.LogEvidence("currentGlobalSpeedInvalidated Calls=" + _currentGlobalSpeedInvalidatedCalls);
                GlobalLog.LogEvidence("currentTimeInvalidated Calls=" + _currentTimeInvalidatedCalls);

                if (_animMethod == "State")
                {
                    switch (_animEvent)
                    {
                        case "Begin":
                            _testPassed = (_currentStateInvalidatedCalls>=3 && _currentGlobalSpeedInvalidatedCalls>=3 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=3\ncurrentGlobalSpeedInvalidatedCalls=3\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Pause":
                            _testPassed = (_currentStateInvalidatedCalls==1 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=1\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Seek":
                            //Seek() fires all events.
                            _testPassed = (_currentStateInvalidatedCalls==3 && _currentGlobalSpeedInvalidatedCalls==3 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=3\ncurrentGlobalSpeedInvalidatedCalls=3\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Resume":
                            _testPassed = (_currentStateInvalidatedCalls==2 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=2\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Deactivate":
                            _testPassed = (_currentStateInvalidatedCalls==2 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=2\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Stop":
                            _testPassed = (_currentStateInvalidatedCalls==2 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);   //
                            _expectedEvents = "currentStateInvalidatedCalls=2\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument.(2)");
                            break;
                    }
                }
                else if (_animMethod == "Speed")
                {
                    switch (_animEvent)
                    {
                        case "Begin":
                            _testPassed = (_currentStateInvalidatedCalls>=3 && _currentGlobalSpeedInvalidatedCalls>=3 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls>=3\ncurrentGlobalSpeedInvalidatedCalls>=3\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Pause":
                            _testPassed = (_currentStateInvalidatedCalls==1 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=1\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Seek":
                            //Seek() fires all events.
                            _testPassed = (_currentStateInvalidatedCalls==3 && _currentGlobalSpeedInvalidatedCalls==3 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=3\ncurrentGlobalSpeedInvalidatedCalls=3\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Resume":
                            _testPassed = (_currentStateInvalidatedCalls==2 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=2\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Deactivate":
                            _testPassed = (_currentStateInvalidatedCalls==2 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);   //
                            _expectedEvents = "currentStateInvalidatedCalls=2\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Stop":
                            _testPassed = (_currentStateInvalidatedCalls==2 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);   //
                            _expectedEvents = "currentStateInvalidatedCalls=2\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument.(3)");
                        break;
                    }
                }
                else if (_animMethod == "Time")
                {
                    switch (_animEvent)
                    {
                        case "Begin":
                            _testPassed = (_currentStateInvalidatedCalls>=3 && _currentGlobalSpeedInvalidatedCalls>=3 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=3\ncurrentGlobalSpeedInvalidatedCalls=3\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Pause":
                            _testPassed = (_currentStateInvalidatedCalls==1 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=1\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Seek":
                            //Seek() fires all events.
                            _testPassed = (_currentStateInvalidatedCalls==3 && _currentGlobalSpeedInvalidatedCalls==3 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=3\ncurrentGlobalSpeedInvalidatedCalls=3\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Resume":
                            _testPassed = (_currentStateInvalidatedCalls==2 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=2\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Deactivate":
                            _testPassed = (_currentStateInvalidatedCalls==2 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=2\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        case "Stop":
                            _testPassed = (_currentStateInvalidatedCalls==2 && _currentGlobalSpeedInvalidatedCalls==2 && _currentTimeInvalidatedCalls>0);
                            _expectedEvents = "currentStateInvalidatedCalls=2\ncurrentGlobalSpeedInvalidatedCalls=2\ncurrentTimeInvalidatedCalls>0";
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument.(4)");
                        break;
                    }
                }
                else
                {
                    HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match first argument.(5)");
                }

                GlobalLog.LogEvidence("\nExpected Events:\n" + _expectedEvents);
                
                if (_testPassed)
                {
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    Signal("TestFinished", TestResult.Fail);
                }
            }
        }

        /******************************************************************************
        * Function:          HandleError
        ******************************************************************************/
        /// <summary>
        /// Fails the test case when an Error occurs.
        /// </summary>
        /// <returns></returns>
        private void HandleError(string message)
        {
            GlobalLog.LogEvidence(message);
            _testPassed = false;
            Signal("TestFinished", TestResult.Fail);
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

            return result;
        }
        #endregion
    }
}
