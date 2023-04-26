// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Methods in Events: Timing Methods *****************
*   Description:
*          Tests that all Timing-level Methods can be called via all Animation Events.  The input parameters define
*          all combinations of Methods x Events:
*          ( only three events are available: CurrentStateInvalidated, 
*          CurrentGlobalSpeedInvalidated, CurrentTimeInvalidated.  The old events are therefore
*          defined by checking certain properties via these three events.)
*               Events  (3):  CurrentStateInvalidated,CurrentGlobalSpeedInvalidated, CurrentTimeInvalidated.
*               Methods (6):  Begin(), Pause(), Seek(), Resume(), Deactivate(), Stop().
*          The Animation consists of an OpacityAnimation on a SolidColorBush for a DockPanel
*          containing a Button.
*          This version sets Begin(0), DA.ClockController.Seek(TimeSpan.FromMilliseconds(100),TimeSeekOrigin.BeginTime), and Deactivate(1).
*   Pass Conditions:
*          The test case will Pass if the events fire appropriately when the methods are invoked.
*
*   Framework:          A CLR executable is created.
*   Area:               Animation/Timing
*
*   Dependencies:       TestRuntime.dll
*   Support Files:
* 
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
    /// Verify that Animation-related Methods can be called via all Animation Events.
    /// </description>
    /// </summary>
    [Test(2, "Animation.LowLevelScenarios.Methods", "TimingMethods1Test")]

    class TimingMethods1Test : WindowTest
    {
        #region Test case members
        
        private Canvas                          _body;
        private DockPanel                       _FP;
        private AnimationClock                  _DA;
        private Clock                           _TLC;
        private int                             _BEGIN_TIME          = 1000;
        private int                             _DURATION_TIME       = 1500;
        private int                             _DURATION_TIME_TL2   = 5000; //Timeline used for verification.
        private TimeSpan                        _SEEK_TIME           = TimeSpan.FromMilliseconds(100);
        private string                          _expectedEvents      = "";

        private int                             _currentStateInvalidatedCalls                 = 0;
        private int                             _currentTimeInvalidatedCalls                  = 0;
        private int                             _currentGlobalSpeedInvalidatedCalls           = 0;

        private bool                            _methodInvoked       = false;
        private string                          _animMethod          = null;
        private string                          _animEvent           = null;
        private bool                            _testPassed          = false;
          
        #endregion


        #region Constructor

        [Variation("State", "Begin")]
        [Variation("State", "Pause")]
        [Variation("State", "Seek")]
        [Variation("State", "Resume")]
        [Variation("State", "Deactivate")]
        [Variation("State", "Stop")]

        [Variation("Speed", "Begin", Priority=0)]
        [Variation("Speed", "Pause")]
        [Variation("Speed", "Seek")]
        [Variation("Speed", "Resume")]
        [Variation("Speed", "Deactivate", Priority=0)]
        [Variation("Speed", "Stop")]

        [Variation("Time", "Begin")]
        [Variation("Time", "Pause")]
        [Variation("Time", "Seek")]
        [Variation("Time", "Resume")]
        [Variation("Time", "Deactivate")]
        [Variation("Time", "Stop")]


        /******************************************************************************
        * Function:          TimingMethods1Test Constructor
        ******************************************************************************/
        public TimingMethods1Test(string testValue1, string testValue2)
        {
            _animMethod = testValue1;
            _animEvent = testValue2;
            InitializeSteps += new TestStep(CheckInputParameters);
            RunSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Animate);
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
        /// Initialize: create a new Window and add content to it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("--- Initialize ---");
            
            Window.Width        = 350;
            Window.Height       = 350;
            Window.Title        = "TimingMethods1 Test";
            Window.Left         = 0;
            Window.Top          = 0;

            _body = new Canvas();
            _body.Width               = 350;
            _body.Height              = 350;
            _body.Background          = Brushes.Azure;

            _FP  = new DockPanel();
            _body.Children.Add(_FP);
            _FP.Width               = 100;
            _FP.Height              = 100;
            _FP.Background          = Brushes.BlueViolet;

            Button BT  = new Button();
            _body.Children.Add(BT);
            BT.Content = "Button";

            Window.Content = _body;

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          Animate
        ******************************************************************************/
        /// <summary>
        /// Animate: create a DoubleAnimation, create an AnimationClock, and start it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult Animate()
        {
            CreateExtraTimelines();
            
            DoubleAnimation anim = new DoubleAnimation();
            anim.From                = 1;
            anim.To                  = 0;
            anim.BeginTime           = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            anim.Duration            = new Duration(TimeSpan.FromMilliseconds(_DURATION_TIME));

            anim.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
            anim.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            anim.CurrentTimeInvalidated        += new EventHandler(OnCurrentTimeInvalidated);

            SolidColorBrush SCB = new SolidColorBrush();
            SCB.Color = Colors.BlueViolet;
            _FP.Background = SCB;

            _DA = anim.CreateClock();
            SCB.ApplyAnimationClock(SolidColorBrush.OpacityProperty, _DA);

            GlobalLog.LogStatus("--- DoubleAnimation Created ---");
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          CreateExtraTimelines
        ******************************************************************************/
        /// <summary>
        /// Add two extra Timelines for test purposes.
        /// </summary>
        private void CreateExtraTimelines()          
        {
            //Root timeline.
            ParallelTimeline root = new ParallelTimeline();
            root.BeginTime           = TimeSpan.FromMilliseconds(0);
            root.Duration            = Duration.Forever;

            //This Timeline is used for verification after the animation is finished.
            ParallelTimeline TL2 = new ParallelTimeline();
            TL2.BeginTime                = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            TL2.Duration                 = new Duration(TimeSpan.FromMilliseconds(_DURATION_TIME_TL2));
            TL2.CurrentStateInvalidated  += new EventHandler(OnCurrentStateTL2);
            root.Children.Add(TL2);

            _TLC = root.CreateClock();

            GlobalLog.LogStatus("--- Extra Timelines Created ---");
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
            GlobalLog.LogStatus("--- OnCurrentGlobalSpeedInvalidated --- " + ((Clock)sender).CurrentGlobalSpeed);
            _currentGlobalSpeedInvalidatedCalls++;
            if (!_methodInvoked)
            {
                GlobalLog.LogStatus("--- SelectMethod called --- ");
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
                      _DA.Controller.Begin();
                      break;
                 case "Pause":
                      GlobalLog.LogStatus("**Pause Invoked**");
                      _DA.Controller.Pause();
                      break;                                   
                 case "Seek":
                      GlobalLog.LogStatus("**Seek Invoked**");
                      _DA.Controller.Seek(_SEEK_TIME,TimeSeekOrigin.BeginTime);
                      break;                                   
                 case "Resume":
                      GlobalLog.LogStatus("**Resume Invoked**");
                      _DA.Controller.Resume();
                      break;                                   
                 case "Deactivate":
                      GlobalLog.LogStatus("**Deactivate Invoked**");
                      _DA.Controller.SkipToFill();
                      break;                                   
                 case "Stop":
                      GlobalLog.LogStatus("**Stop Invoked**");
                      _DA.Controller.Stop();
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
                GlobalLog.LogStatus("--- nd TestCase ---");

                GlobalLog.LogEvidence("Actual Events:");
                GlobalLog.LogEvidence("currentStateInvalidated Calls=" + _currentStateInvalidatedCalls);
                GlobalLog.LogEvidence("currentGlobalSpeedInvalidated Calls=" + _currentGlobalSpeedInvalidatedCalls);
                GlobalLog.LogEvidence("currentTimeInvalidated Calls=" + _currentTimeInvalidatedCalls);

                if (_animMethod == "State")
                {
                    switch (_animEvent)
                    {
                        case "Begin":
                            _testPassed = (_currentStateInvalidatedCalls==3 && _currentGlobalSpeedInvalidatedCalls==3 && _currentTimeInvalidatedCalls>0);
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
                            _testPassed = (_currentStateInvalidatedCalls==3 && _currentGlobalSpeedInvalidatedCalls==3 && _currentTimeInvalidatedCalls>0);
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
                            _testPassed = (_currentStateInvalidatedCalls==3 && _currentGlobalSpeedInvalidatedCalls==3 && _currentTimeInvalidatedCalls>0);
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
