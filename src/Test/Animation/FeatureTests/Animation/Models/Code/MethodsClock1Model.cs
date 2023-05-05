// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Animation Test *******************************************************
*     Major Actions:      Verify a sequence of Animation methods applied on the same Tick
*                         [Technique:  CreateClock/ApplyAnimationClock]
*                         [Animation:  DoubleAnimation of a TextBox's Height property.]
*     Pass Conditions:    The test passes if Clock properties are returned correctly.
*                         Note that there may be multiple sub-tests for this overall test,
*                         each sub-test invoking a different ordered set of Animation methods.
*     How verified:       at the end of each test, several Clock properties are read and 
*                         compared to expected values.
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing

*     Dependencies:       TestRuntime.dll, AnimationFramework.dll
*     Support Files:      SingleTickModel.xtc file, which specifies the test cases [must be available at run time]
g.
*********************************************************************************************/
using System;
using System.Xml;
using System.Data;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Modeling;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.Models.AnimationClock</area>
    /// <priority>3</priority>
    /// <description>
    /// Verify Animation Methods on the same Tick, using a DoubleAnimation
    /// </description>
    /// </summary>
    [Test(3, "Animation.Models.AnimationClock", "MethodsClock1Model", SupportFiles=@"FeatureTests\Animation\SingleTickModel.xtc")]

    class MethodsClock1Model : WindowModel
    {
        #region Test case members

        private DispatcherTimer             _aTimer;
        private int                         _dispatcherTickCount         = 0;
        private int                         _testNumber                  = 0;
        private TextBox                     _textbox;
        private DoubleAnimation             _animHeight;
        private AnimationClock              _clock;
        private ClockState                  _expectedCurrentState        = ClockState.Stopped;
        private ClockState                  _actualInitialCurrentState;
        private bool                        _expectedEvent               = false;
        private bool                        _actualEvent                 = false;
        private bool                        _expectedIsPaused            = false;
        private ArrayList                   _actionList;
        private bool                        _cumulativeResult            = false;
        private string                      _outString                   = "";

        private Duration                    _DURATION_TIME   = new Duration(TimeSpan.FromMilliseconds(2000));
        private TimeSpan                    _SEEK_TIME       = TimeSpan.FromMilliseconds(250);
        private TimeSpan                    _TIMER_INTERVAL  = TimeSpan.FromMilliseconds(500);

        #endregion


        #region Contructors

        [Variation("SingleTickModel", "SingleTickModel.xtc",    1,  50)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    951,  1000)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    1251, 1300)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    1401, 1450)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    1551, 1553)]

        public MethodsClock1Model(string xtcFileName) : this(xtcFileName, -1) { }

        public MethodsClock1Model(string xtcFileName, int testCaseNumber)
            : this(xtcFileName, testCaseNumber, testCaseNumber) { }

        public MethodsClock1Model(string xtcFileName, int startTestCaseNumber, int endTestCaseNumber)
            : this("", xtcFileName, startTestCaseNumber, endTestCaseNumber) { }

        public MethodsClock1Model(string modelName, string xtcFileName, int beginTestCaseNumber, int endTestCaseNumber)
            : base(modelName, xtcFileName, beginTestCaseNumber, endTestCaseNumber)
        {
            // happens once before all xtc test cases run
            OnInitialize += new EventHandler(OnInitialize_Handler);
            // happens when each xtc test case run
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);
            //NOTE: for this test, in which Animation methods are "bundled" together in a single
            //Tick, i.e., there is no time interval between the methods (actions) invoked, 
            //the expected clock.CurrentState is relevant only for the -last- method
            //applied; but the effect of the last method depends on the methods applied before.
            //So, every action as it occurs, which corresponds to an Animation method, will update
            //the expected current state. Consequently, the pass/fail decision will not be made
            //in CurrentState, but at the end of the test case, when the last method applied (and the 
            //resulting state) is known.  This check by the model is forced to succeed.

            //Add Action Handlers
            AddAction("begin_action",       new ActionHandler(begin_action));
            AddAction("pause_action",       new ActionHandler(pause_action));
            AddAction("resume_action",      new ActionHandler(resume_action));
            AddAction("seek_action",        new ActionHandler(seek_action));
            AddAction("skiptofill_action",  new ActionHandler(skiptofill_action));
            AddAction("stop_action",        new ActionHandler(stop_action));
            AddAction("remove_action",      new ActionHandler(remove_action));
        }

        #endregion


        #region Model setup and clean-up events

        /******************************************************************************
        * Function:          OnInitialize_Handler
        ******************************************************************************/
        /// <summary>
        /// Initialize the model.
        /// </summary>
        /// <returns></returns>
        private void OnInitialize_Handler(object sender, EventArgs e)
        {
            _outString += " OnInitialize\n";

            _actionList = new ArrayList();

            Window.Title           = "MethodsClock1 Test";
            Window.Left            = 0;
            Window.Top             = 0;
            Window.Height          = 300;
            Window.Width           = 150;
            Window.WindowStyle     = WindowStyle.None;

            Canvas body  = new Canvas();
            Window.Content = body;

            //Create the to-be-animated control.
            _textbox = new TextBox();
            body.Children.Add(_textbox);
            _textbox.Height  = 60d;
            _textbox.Width   = 100d;

            _cumulativeResult = true;
        }

        /******************************************************************************
        * Function:          OnBeginCase_Handler
        ******************************************************************************/
        /// <summary>
        /// OnBeginCase_Handler: Fires when each test case begins.
        /// </summary>
        /// <returns></returns>
        void OnBeginCase_Handler(object sender, StateEventArgs e)
        {
            //outString += " OnBeginCase\n";


            _testNumber++;
            _outString +="-------------------------------------------------\n";
            _outString +="-------------------------------------------------\n";
            _outString +="BEGIN TEST #" + _testNumber + "\n";
            _outString +="-------------------------------------------------\n";

            //Reset default values.
            _expectedCurrentState        = ClockState.Stopped;
            _expectedEvent               = false;
            _actualEvent                 = false;
            _expectedIsPaused            = false;
            _clock                       = null;
            _actionList.Clear();

            //Create an Animation.
            _animHeight = new DoubleAnimation();
            _animHeight.BeginTime          = null;
            _animHeight.Duration           = _DURATION_TIME;
            _animHeight.From               = 60d;
            _animHeight.To                 = 120d;
            _animHeight.CurrentStateInvalidated    += new EventHandler(OnCurrentState);
            
            //Create an AnimationClock.
            _clock = _animHeight.CreateClock();
            _textbox.ApplyAnimationClock(TextBox.HeightProperty, _clock);

            //Record the intitial CurrentState.
            _actualInitialCurrentState = _clock.CurrentState;
            
            //Start a separate Timer to control the timing of verification.
            _dispatcherTickCount = 0;  //Needs to be reset for every case.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = _TIMER_INTERVAL;
            _aTimer.Start();

            _outString +="-- DispatcherTimer Started --\n";
        }

        /******************************************************************************
        * Function:          OnEndCase_Handler
        ******************************************************************************/
        /// <summary>
        /// OnEndCase_Handler: Fires when each test case ends.
        /// </summary>
        /// <returns></returns>
        void OnEndCase_Handler(object sender, StateEventArgs e)
        {
            //outString += "-OnEndCase\n";
            
            EndCase(e.State);
        }

        /******************************************************************************
        * Function:          EndCase
        ******************************************************************************/
        /// <summary>
        /// OnEndCase: Overrides the Model's EndCase, to verify the test case result.
        /// </summary>
        /// <returns>True</returns>
        public override bool EndCase(State endCase)
        {
            //outString += "-EndCase\n";
            
            bool b1, b2, b3, b4, b5, b6, b7, b8, actualIsPaused;
            ClockState actualFinalCurrentState;
            Nullable<double>    actualCurrentProgress;
            Nullable<TimeSpan>  actualCurrentTime;
            Nullable<double>    actualCurrentGlobalSpeed,   expectedCurrentGlobalSpeed;
            Nullable<int>       actualCurrentIteration,     expectedCurrentIteration;
            string              expectedCurrentProgressString, expectedCurrentTimeString;
            
            //Wait before verifying results.  A DispatcherTimer event is used to wait 1 second
            //before resuming the test and verifying results.  This is necessary to ensure that
            //at least one TimeManager tick occurs so the animation methods take place.
            WaitForSignal("VerifyResults");
            
            //Retrieve the actual ClockState after the final Animation method has been invoked.
            actualFinalCurrentState     = _clock.CurrentState;
            actualCurrentProgress       = _clock.CurrentProgress;
            actualCurrentTime           = _clock.CurrentTime;
            actualCurrentGlobalSpeed    = _clock.CurrentGlobalSpeed;
            actualCurrentIteration      = _clock.CurrentIteration;
            actualIsPaused              = _clock.IsPaused;
           
            //Remove the AnimationClock and the animation's CurrentStateInvalidated event handler.
            _animHeight.CurrentStateInvalidated    -= new EventHandler(OnCurrentState);
            _textbox.ApplyAnimationClock(TextBox.HeightProperty, null);

            //Pass or fail the test case.            
            b1 = (_actualInitialCurrentState == ClockState.Stopped);
            b2 = (actualFinalCurrentState == _expectedCurrentState);


            if (DetermineSkipToFill())
            {
                if ((actualFinalCurrentState == ClockState.Filling) || (actualFinalCurrentState == ClockState.Stopped))
                {
                    b2 = true;
                }
                else
                {
                    b2 = false;
                }
            }
            
            if (actualFinalCurrentState == ClockState.Stopped)
            {
                b3 = (actualCurrentProgress == null);
                b4 = (actualCurrentTime     == null);
                expectedCurrentProgressString = "";
                expectedCurrentTimeString     = "";
                expectedCurrentGlobalSpeed  = null;
                expectedCurrentIteration    = null;
                _expectedEvent               = false;
            }

            else if (DetermineStartPause())
            {
                //Special case: Begin, followed by Pause will not start the Animation.
                b3 = (actualCurrentProgress == 0);
                b4 = (actualCurrentTime     == new TimeSpan(0));
                expectedCurrentProgressString   = "0";
                expectedCurrentTimeString       = "0";
                expectedCurrentGlobalSpeed      = 0;
                expectedCurrentIteration        = 1;
            }
            else
            {
                b3 = (actualCurrentProgress > 0);
                b4 = (actualCurrentTime     > new TimeSpan(0));
                expectedCurrentProgressString   = ">0";
                expectedCurrentTimeString       = ">0";
                if (actualFinalCurrentState == ClockState.Filling)
                {
                    expectedCurrentGlobalSpeed = 0;
                }
                else
                {
                    expectedCurrentGlobalSpeed = 1;
                }
                expectedCurrentIteration        = 1;

            }
            
            if (_expectedIsPaused && (actualFinalCurrentState != ClockState.Stopped) )
            {
                //Unless the State is stopped, if the Animation pauses, CurrentGlobalSpeed should be 0.
                expectedCurrentGlobalSpeed      = 0;
            }
            
            b5 = (actualCurrentGlobalSpeed  == expectedCurrentGlobalSpeed);
            b6 = (actualCurrentIteration    == expectedCurrentIteration);
            b7 = (actualIsPaused            == _expectedIsPaused);
            b8 = (_actualEvent               == _expectedEvent);
            
            _outString +="-------------------------------------------------\n";
            _outString +="   CurrentState [INIT ]- Act: " + _actualInitialCurrentState + " Exp: Stopped\n";
            _outString +="   CurrentState [FINAL]- Act: " + actualFinalCurrentState + " Exp: " + _expectedCurrentState + "\n";
            _outString +="   CurrentProgress     - Act: " + actualCurrentProgress + " Exp: " + expectedCurrentProgressString + "\n";
            _outString +="   CurrentTime         - Act: " + actualCurrentTime + " Exp: " + expectedCurrentTimeString + "\n";
            _outString +="   CurrentGlobalSpeed  - Act: " + actualCurrentGlobalSpeed + " Exp: " + expectedCurrentGlobalSpeed + "\n";
            _outString +="   CurrentIteration    - Act: " + actualCurrentIteration + " Exp: " + expectedCurrentIteration + "\n";
            _outString +="   IsPaused            - Act: " + actualIsPaused + " Exp: " + _expectedIsPaused + "\n";
            _outString +="   EVENT FIRING        - Act: " + _actualEvent + " Exp: " + _expectedEvent + "\n";
            _outString +="-------------------------------------------------\n";
            _outString +="RESULTS: " + b1 + "/" + b2 + "/" + b3 + "/" + b4 + "/" + b5 + "/" + b6 + "/" + b7 + "/" + b8 + "\n";
            _outString +="-------------------------------------------------\n";
            
            bool testResult = (b1 && b2 && b3 && b4 && b5 && b6 && b7 && b8);

            if (testResult)
            {
                _outString +="END TEST #" + _testNumber + "                       RESULT: PASSED\n";
            }
            else
            {
                _outString +="END TEST #" + _testNumber + "                       RESULT: FAILED\n";
            }
            _outString += "-------------------------------------------------\n";
            
            _cumulativeResult = (testResult && _cumulativeResult);

            return true;
        }

        /******************************************************************************
        * Function:          CleanUp
        ******************************************************************************/
        /// <summary>
        /// Returns the final result to the frmwk for one or more test cases.
        /// </summary>
        /// <returns> true </returns>
        public override bool CleanUp()
        {
            _outString +="-------------------------------------------------\n";
            //Communicate to Model whether the tests pass or fail as a whole.
            //This allows -all- requested tests to run, even though one of them fails.
            if (_cumulativeResult)
            {
                _outString += "CUMULATIVE RESULT:  PASSED\n";
            }
            else
            {
                _outString += "CUMULATIVE RESULT:  FAILED\n";
            }

            //Print out all messages.
            GlobalLog.LogEvidence( _outString );

            return _cumulativeResult;
        }

        #endregion


        #region Action steps

        /******************************************************************************
        * Function:          begin_action
        ******************************************************************************/
        /// <summary>
        /// Handler for begin_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool begin_action( State endState, State inParameters, State outParameters  )
        {
            _outString +="-- Begin Action --\n";

            _clock.Controller.Begin();
            
            UpdateExpectedState("Begin");
            
            return true;
        }

        /******************************************************************************
        * Function:          pause_action
        ******************************************************************************/
        /// <summary>
        /// Handler for pause_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool pause_action( State endState, State inParameters, State outParameters )
        {
            _outString +="-- Pause Action --\n";

            _clock.Controller.Pause();
            
            UpdateExpectedState("Pause");

            return true;
        }

        /******************************************************************************
        * Function:          resume_action
        ******************************************************************************/
        /// <summary>
        /// Handler for resume_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool resume_action( State endState, State inParameters, State outParameters )
        {
            _outString +="-- Resume Action --\n";

            _clock.Controller.Resume();
            
            UpdateExpectedState("Resume");

            return true;
        }

        /******************************************************************************
        * Function:          seek_action
        ******************************************************************************/
        /// <summary>
        /// Handler for seek_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool seek_action( State endState, State inParameters, State outParameters )
        {
            _outString +="-- Seek Action --\n";

            _clock.Controller.Seek(_SEEK_TIME,TimeSeekOrigin.BeginTime);
            
            UpdateExpectedState("Seek");

            return true;
        }

        /******************************************************************************
        * Function:          skiptofill_action
        ******************************************************************************/
        /// <summary>
        /// Handler for skiptofill_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool skiptofill_action( State endState, State inParameters, State outParameters )
        {
            _outString +="-- SkipToFill Action --\n";

            _clock.Controller.SkipToFill();
            
            UpdateExpectedState("SkipToFill");

            return true;
        }

        /******************************************************************************
        * Function:          stop_action
        ******************************************************************************/
        /// <summary>
        /// Handler for stop_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool stop_action( State endState, State inParameters, State outParameters )
        {
            _outString +="-- Stop Action --\n";

            _clock.Controller.Stop();
            
            UpdateExpectedState("Stop");

            return true;
        }

        /******************************************************************************
        * Function:          remove_action
        ******************************************************************************/
        /// <summary>
        /// Handler for remove_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// 
        /// <returns> Succeess status. </returns>
        /// 
        private bool remove_action( State endState, State inParameters, State outParameters )
        {
            _outString +="-- Remove Action --\n";

            _clock.Controller.Remove();
            
            UpdateExpectedState("Remove");

            return true;
        }

        #endregion

        #region Additional Routines
        
        /******************************************************************************
        * Function:          UpdateExpectedState
        ******************************************************************************/
        /// <summary>
        /// Calculates the expected state, given an Animation method applied.
        /// </summary>
        /// <param name="method">      The Animation method invoked       </param>
        /// <returns> The expected state. </returns>
        private void UpdateExpectedState( string method )
        {
            //NOTE1: calculation of the expected CurrentState is indentical to the State-based
            //calculations specified in the Model.
            //NOTE2: the CurrentStateInvalidated event should fire only when the CurrentState
            //is invalidated, so expectedEvent is set accordingly, per method invoked.
            
            //First, update the running list of actions (methods), used later for verification.
            _actionList.Add(method);

            //Next, updated expected variables, according to the effect of a method given the
            //expectedCurrentState, which is a running estimate, not actual, because the
            //states are hypothetical.  The clock's state won't actually change until after the
            //final method is applied and the AnimationClock ticks.
            switch (method)
            {
                case "Begin" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Active;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Active;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Active;
                            break;
                        default:
                            break;
                    }
                    _expectedEvent       = true;
                    break;
                
                case "Pause" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Stopped;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Active;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Filling;
                            break;
                        default:
                            break;
                    }
                    _expectedIsPaused    = true;
                    break;
                
                case "Resume" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Stopped;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Active;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Filling;
                            break;
                        default:
                            break;
                    }
                    _expectedIsPaused    = false;
                    break;
                
                case "Seek" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Active;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Active;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Active;
                            break;
                        default:
                            break;
                    }
                    _expectedEvent       = true;
                    break;

                case "SkipToFill" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Filling;
                            _expectedEvent           = true;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Filling;
                            _expectedEvent           = true;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Filling;
                            //expectedEvent           = false;
                            break;
                        default:
                            break;
                    }
                    break;

                case "Stop" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Stopped;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Stopped;
                            _expectedEvent           = true;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Stopped;
                            _expectedEvent           = true;
                            break;
                        default:
                            break;
                    }
                    break;

                case "Remove" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Stopped;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Stopped;
                            _expectedEvent           = true;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Stopped;
                            _expectedEvent           = true;
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns>  </returns>
        private void OnTick(object sender, EventArgs e)
        {
            _dispatcherTickCount++;
            
            if (_dispatcherTickCount == 2)
            {
                _aTimer.Stop();
                
                //Go ahead and verify results; the TimeManager should have ticked for the animation.
                Signal("VerifyResults", (TestResult)(_cumulativeResult ? TestResult.Pass : TestResult.Fail));
            }
        }
        
        /******************************************************************************
        * Function:          OnCurrentState
        ******************************************************************************/
        /// <summary>
        /// Invoked when the CurrentStateInvalidated event fires on the Animation.
        /// Used for debugging purposes.
        /// </summary>
        /// <returns>  </returns>
        private void OnCurrentState(object sender, EventArgs e)
        {
            _outString +="-- CurrentStateInvalidated fired: " + ((Clock)sender).CurrentState + "\n";
            _actualEvent = true;
        }
        
        /******************************************************************************
        * Function:          DetermineStartPause
        ******************************************************************************/
        /// <summary>
        /// Looks at the action history to determine whether a Begin action is affected
        /// by a previous Pause action.
        /// </summary>
        /// <returns> A boolean value, indicating whether or not a Begin/Pause combo occured </returns>
        private bool DetermineStartPause()
        {
            //This routine is looking for scenarios in which CurrentState becomes Active, but
            //CurrentProgress/CurrentGlobalSpeed etc. do not advance.  This can happen when
            //a Begin followed by a Pause, with possibly intermediate Begin, Pause,
            //or SkipToFill calls.
            string action   = "";
            bool beginFound = false;
            bool pauseFound = false;
            
            for (int i=0; i<_actionList.Count; i+=1)
            {
                action = (string)_actionList[i];
                
                if (action == "Begin")
                {
                    beginFound = true;
                }
                if (_expectedIsPaused)
                {
                    if (beginFound)
                    {
                        //The Animation must have already been started for the Pause to count.
                        pauseFound = true;
                    }
                }
                
                if (beginFound)
                {
                    if (action == "Seek" || action == "SkipToFill" || action == "Stop" || action == "Remove")
                    {
                        //Any of these will "invalidate" the scenario, e.g., Begin-Pause-Resume.
                        beginFound = false;
                    }
                }
            }
            if (beginFound && pauseFound)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /******************************************************************************
        * Function:          DetermineSkipToFill
        ******************************************************************************/
        /// <summary>
        /// Looks at the action history to determine whether a Begin action is affected
        /// by a previous Pause action.
        /// </summary>
        /// <returns> A boolean value, indicating whether or not a Begin/Pause combo occured </returns>
        private bool DetermineSkipToFill()
        {

            string action           = "";
            bool skipToFillFound    = false;
            bool stopFound          = false;
            bool removeFound        = false;
            
            for (int i=0; i<_actionList.Count; i+=1)
            {
                action = (string)_actionList[i];
                
                if (action == "Stop")
                {
                    stopFound = true;
                }
                else if (action == "Remove")
                {
                    removeFound = true;
                }
                else if (action == "Begin" || action == "Seek")
                {
                    skipToFillFound = false;    //Reset.
                }
                else if (action == "SkipToFill")
                {
                    if (stopFound || removeFound)
                    {
                        skipToFillFound = true;
                    }
                }

                if (skipToFillFound)
                {
                    if (action == "Seek" || action == "Begin")
                    {
                        //Any of these will "invalidate" the scenario.
                        skipToFillFound = false;
                    }
                }
            }
            if (skipToFillFound)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
