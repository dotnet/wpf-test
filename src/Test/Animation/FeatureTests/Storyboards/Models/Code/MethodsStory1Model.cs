// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Animation Test *******************************************************
*     Major Actions:      Verify a sequence of Animation methods applied on the same Tick
*                         [Technique:  Storyboards.]
*                         [Animation:  ColorAnimation of a CheckBox's Background property.]
*     Pass Conditions:    The test passes if Clock properties are returned correctly.
*                         Note that there may be multiple sub-tests for this overall test,
*                         each sub-test invoking a different ordered set of Animation methods.
*     How verified:       at the end of each test, several Clock properties are read and 
*                         compared to expected values.
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll, AnimationFramework.dll
*     Support Files:      SingleTickModel.xtc file, which specifies the test cases [must be available at run time]

*     NOTE:  all event testing code is commented out; event firing is unpredictable in this test framework.
*/
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
    /// <area>Storyboard.Models.Storyboard</area>
    /// <priority>3</priority>
    /// <description>
    /// Verify Animation Methods on the same Tick, using a ColorAnimation in a Storyboard
    /// </description>
    /// </summary>
    [Test(3, "Storyboard.Models.Storyboard", "MethodsStory1Model", SupportFiles=@"FeatureTests\Animation\SingleTickModel.xtc")]

    class MethodsStory1Model : WindowModel
    {
        #region Test case members

        private DispatcherTimer             _aTimer;
        private int                         _dispatcherTickCount         = 0;
        private CheckBox                    _checkbox;
        private ColorAnimation              _animColor;
        private Storyboard                  _storyboard;
        private DependencyProperty          _dp;
        private ClockState                  _expectedCurrentState        = ClockState.Stopped;
        //private int                         expectedEvent               = 0;
        //private int                         actualEvent                 = 0;
        private bool                        _storyboardBegun             = false;
        private bool                        _expectedIsPaused            = false;
        private ArrayList                   _actionList;
        private bool                        _cumulativeResult            = true;

        private TimeSpan                    _BEGIN_TIME      = TimeSpan.FromMilliseconds(0);
        private Duration                    _DURATION_TIME   = new Duration(TimeSpan.FromMilliseconds(5000));
        private TimeSpan                    _SEEK_TIME       = TimeSpan.FromMilliseconds(500);
        private TimeSpan                    _TIMER_INTERVAL  = TimeSpan.FromMilliseconds(1000);

        #endregion


        #region Contructors

        [Variation("SingleTickModel", "SingleTickModel.xtc",    1,  20)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    21,  40)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    41,  60)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    381,  400)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    661,  680)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    881,  900)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    1081,  1100)]
        [Variation("SingleTickModel", "SingleTickModel.xtc",    1381,  1400)]

        public MethodsStory1Model(string xtcFileName) : this(xtcFileName, -1) { }

        public MethodsStory1Model(string xtcFileName, int testCaseNumber)
            : this(xtcFileName, testCaseNumber, testCaseNumber) { }

        public MethodsStory1Model(string xtcFileName, int startTestCaseNumber, int endTestCaseNumber)
            : this("", xtcFileName, startTestCaseNumber, endTestCaseNumber) { }

        public MethodsStory1Model(string modelName, string xtcFileName, int beginTestCaseNumber, int endTestCaseNumber)
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

        /// <summary>
        /// Initialize the model.
        /// </summary>
        /// <returns></returns>
        private void OnInitialize_Handler(object sender, EventArgs e)
        {
            GlobalLog.LogStatus( "OnInitialize" );


            _actionList = new ArrayList();
            
            Window.Title           = "Storyboard Methods Test";
            Window.Left            = 0;
            Window.Top             = 0;
            Window.Height          = 300;
            Window.Width           = 150;
            Window.WindowStyle     = WindowStyle.None;

            NameScope.SetNameScope(Window, new NameScope());

            Canvas body  = new Canvas();
            Window.Content = body;

            //Create the to-be-animated control.
            _checkbox = new CheckBox();
            body.Children.Add(_checkbox);
            _checkbox.Content    = "Avalon!";
            _checkbox.FontSize   = 24d;
            _checkbox.Name       = "CB";
        }

        /// <summary>
        /// OnBeginCase_Handler: Fires when each test case begins.
        /// </summary>
        /// <returns></returns>
        void OnBeginCase_Handler(object sender, StateEventArgs e)
        {
            GlobalLog.LogStatus( "OnBeginCase" );
            
            _dp = SolidColorBrush.ColorProperty;

            //Create an Animation.
            _animColor = new ColorAnimation();
            _animColor.BeginTime          = _BEGIN_TIME;
            _animColor.Duration           = _DURATION_TIME;
            _animColor.From               = Colors.Purple;
            _animColor.To                 = Colors.HotPink;
            
            SolidColorBrush SCB1 = new SolidColorBrush();
            SCB1.Color = Colors.Purple;
            _checkbox.Background = SCB1;

            //Create a Storyboard.
            _storyboard = new Storyboard();
            _storyboard.Name         = "story";
            _storyboard.CurrentStateInvalidated    += new EventHandler(OnCurrentStateInvalidated);
            _storyboard.Children.Add(_animColor);
            
            PropertyPath path = new PropertyPath("(0).(1)", CheckBox.BackgroundProperty, _dp);
            
            Storyboard.SetTargetProperty(_animColor, path);
            Window.RegisterName(_checkbox.Name, _checkbox);
            Storyboard.SetTargetName(_animColor, _checkbox.Name);
            
            //Start a separate Timer to control the timing of verification.
            _dispatcherTickCount = 0;  //Needs to be reset for every case.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = _TIMER_INTERVAL;
            _aTimer.Start();
            GlobalLog.LogStatus("-- DispatcherTimer Started --");
        }

        /// <summary>
        /// OnEndCase_Handler: Fires when each test case ends.
        /// </summary>
        /// <returns></returns>
        void OnEndCase_Handler(object sender, StateEventArgs e)
        {
            GlobalLog.LogStatus( "OnEndCase" );
            
            EndCase(e.State);
        }

        /// <summary>
        /// OnEndCase: Overrides the Model's EndCase, to verify the test case result.
        /// </summary>
        /// <returns>True</returns>
        public override bool EndCase(State endCase)
        {
            GlobalLog.LogStatus( "EndCase" );
            bool b1, b2, b3, b4, b5, b6;
            bool actualIsPaused = false;
            ClockState          actualCurrentState              = ClockState.Stopped;
            Nullable<double>    actualCurrentProgress           = null;
            Nullable<TimeSpan>  actualCurrentTime               = null;
            Nullable<double>    actualCurrentGlobalSpeed        = null;
            Nullable<double>    expectedCurrentGlobalSpeed      = null;
            Nullable<double>    actualCurrentIteration          = null;
            Nullable<double>    expectedCurrentIteration        = null;
            string              expectedCurrentProgressString   = "";
            string              expectedCurrentTimeString       = "";
            
            //Wait before verifying results.  A DispatcherTimer event is used to wait 1 second
            //before resuming the test and verifying results.  This is necessary to ensure that
            //at least one TimeManager tick occurs so the animation methods take place.
            WaitForSignal("VerifyResults");

            if (!_storyboardBegun)
            {
                //Cannot retrieve actual State/Time/GlobalSpeed from a Storyboard that
                //has not started, so will force a Stopped state.
                b1 = (actualCurrentState == ClockState.Stopped);
                b2 = (actualCurrentProgress == null);
                b3 = (actualCurrentTime     == null);
                expectedCurrentProgressString = "";
                expectedCurrentTimeString     = "";
                b4 = (actualCurrentGlobalSpeed  == null);
                b5 = (actualCurrentIteration    == null);
                b6 = (actualIsPaused            == false);
            }
            else
            {
                //Retrieve the actual ClockState after the final Animation method has been invoked.
                actualCurrentState     = _storyboard.GetCurrentState(_checkbox);
                actualCurrentProgress       = _storyboard.GetCurrentProgress(_checkbox);
                actualCurrentTime           = _storyboard.GetCurrentTime(_checkbox);
                actualCurrentGlobalSpeed    = _storyboard.GetCurrentGlobalSpeed(_checkbox);
                actualCurrentIteration      = _storyboard.GetCurrentIteration(_checkbox);
                actualIsPaused              = _storyboard.GetIsPaused(_checkbox);

                //Pass or fail the test case.            
                if (actualCurrentState == ClockState.Stopped)
                {
                    b1 = (actualCurrentState == _expectedCurrentState);
                    b2 = (actualCurrentProgress == null);
                    b3 = (actualCurrentTime     == null);
                    expectedCurrentProgressString = "";
                    expectedCurrentTimeString     = "";
                    expectedCurrentGlobalSpeed  = null;
                    expectedCurrentIteration    = null;
                    //expectedEvent--;
                }
                else if (DetermineStartPause())
                {
                    //Special case: Begin, followed by Pause will not start the Animation.
                    b1 = (actualCurrentState == _expectedCurrentState);
                    b2 = (actualCurrentProgress == 0);
                    b3 = (actualCurrentTime     == new TimeSpan(0));
                    expectedCurrentProgressString   = "0";
                    expectedCurrentTimeString       = "0";
                    expectedCurrentGlobalSpeed      = 0;
                    expectedCurrentIteration        = 1;
                }
                else
                {
                    b1 = (actualCurrentState == _expectedCurrentState);
                    b2 = (actualCurrentProgress > 0);
                    b3 = (actualCurrentTime     > new TimeSpan(0));
                    expectedCurrentProgressString   = ">0";
                    expectedCurrentTimeString       = ">0";
                    if (actualCurrentState == ClockState.Filling)
                    {
                        expectedCurrentGlobalSpeed = 0;
                    }
                    else
                    {
                        expectedCurrentGlobalSpeed = 1;
                    }
                    expectedCurrentIteration        = 1;
                }

                if (_expectedIsPaused && (_expectedCurrentState != ClockState.Stopped) )
                {
                    //Unless the State is stopped, if the Animation pauses, CurrentGlobalSpeed should be 0.
                    expectedCurrentGlobalSpeed      = 0;
                }

                b4 = (actualCurrentGlobalSpeed  == expectedCurrentGlobalSpeed);
                b5 = (actualCurrentIteration    == expectedCurrentIteration);
                b6 = (actualIsPaused            == _expectedIsPaused);

            }
            //b7 = (actualEvent               == expectedEvent);
            
            GlobalLog.LogEvidence("-------------------------------------------------");
            GlobalLog.LogEvidence("-CurrentState        - Act: " + actualCurrentState       + " Exp: " + _expectedCurrentState);
            GlobalLog.LogEvidence("-CurrentProgress     - Act: " + actualCurrentProgress    + " Exp: " + expectedCurrentProgressString);
            GlobalLog.LogEvidence("-CurrentTime         - Act: " + actualCurrentTime        + " Exp: " + expectedCurrentTimeString);
            GlobalLog.LogEvidence("-CurrentGlobalSpeed  - Act: " + actualCurrentGlobalSpeed + " Exp: " + expectedCurrentGlobalSpeed);
            GlobalLog.LogEvidence("-CurrentIteration    - Act: " + actualCurrentIteration   + " Exp: " + expectedCurrentIteration);
            GlobalLog.LogEvidence("-IsPaused            - Act: " + actualIsPaused           + " Exp: " + _expectedIsPaused);
            //GlobalLog.LogEvidence("-EVENT FIRING        - Act: " + actualEvent + " Exp: " + expectedEvent);
            GlobalLog.LogEvidence("-------------------------------------------------");
            GlobalLog.LogEvidence("RESULTS: " + b1 + "/" + b2 + "/" + b3 + "/" + b4 + "/" + b5 + "/" + b6);
            GlobalLog.LogEvidence("-------------------------------------------------");
            
            ResetCase();
            
            string resultString = "RESULTS: " + b1 + "/" + b2 + "/" + b3 + "/" + b4 + "/" + b5 + "/" + b6;
            bool testResult = (b1 && b2 && b3 && b4 && b5 && b6);
            if (testResult)
            {
                resultString += "  PASSED";
            }
            else
            {
                resultString += "  FAILED";
            }
            GlobalLog.LogEvidence(resultString);
            GlobalLog.LogEvidence("-------------------------------------------------");
            
            _cumulativeResult = (testResult && _cumulativeResult);

            return true;
        }

        /// <summary>
        /// Returns the final result to the frmwk for one or more test cases.
        /// </summary>
        /// <returns> true </returns>
        public override bool CleanUp()
        {
            GlobalLog.LogEvidence("-------------------------------------------------");
            //Communicate to Model whether the tests pass or fail as a whole.
            //This allows -all- requested tests to run, even though one of them fails.
            if (_cumulativeResult)
            {
                GlobalLog.LogEvidence( "CUMULATIVE RESULT:  PASSED" );
            }
            else
            {
                GlobalLog.LogEvidence( "CUMULATIVE RESULT:  FAILED" );
            }
            return _cumulativeResult;
        }

        #endregion


        #region Action steps

        /// <summary>
        /// Handler for begin_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool begin_action( State endState, State inParameters, State outParameters )
        {
            GlobalLog.LogStatus("-- Begin Action --");

            _storyboard.Begin(_checkbox, HandoffBehavior.SnapshotAndReplace, true);
            
            _storyboardBegun     = true;

            UpdateExpectedState("Begin");
            
            return true;
        }

        /// <summary>
        /// Handler for pause_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool pause_action( State endState, State inParameters, State outParameters )
        {
            GlobalLog.LogStatus("-- Pause Action --");

            _storyboard.Pause(_checkbox);

            UpdateExpectedState("Pause");

            return true;
        }

        /// <summary>
        /// Handler for resume_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool resume_action( State endState, State inParameters, State outParameters )
        {
            GlobalLog.LogStatus("-- Resume Action --");

            _storyboard.Resume(_checkbox);
            
            UpdateExpectedState("Resume");

            return true;
        }

        /// <summary>
        /// Handler for seek_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool seek_action( State endState, State inParameters, State outParameters )
        {
            GlobalLog.LogStatus("-- Seek Action --");

            _storyboard.Seek(_checkbox, _SEEK_TIME,TimeSeekOrigin.BeginTime);
            
            UpdateExpectedState("Seek");

            return true;
        }

        /// <summary>
        /// Handler for skiptofill_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool skiptofill_action( State endState, State inParameters, State outParameters )
        {
            GlobalLog.LogStatus("-- SkipToFill Action --");

            _storyboard.SkipToFill(_checkbox);
            
            UpdateExpectedState("SkipToFill");

            return true;
        }

        /// <summary>
        /// Handler for stop_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool stop_action( State endState, State inParameters, State outParameters )
        {
            GlobalLog.LogStatus("-- Stop Action --");

            _storyboard.Stop(_checkbox);
            
            UpdateExpectedState("Stop");

            return true;
        }

        /// <summary>
        /// Handler for remove_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool remove_action( State endState, State inParameters, State outParameters )
        {
            GlobalLog.LogStatus("-- Remove Action --");

            _storyboard.Remove(_checkbox);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            UpdateExpectedState("Remove");

            return true;
        }
        
        /// <summary>
        /// Calculates the expected state, given an Animation method applied.
        /// </summary>
        /// <param name="method">      The Animation method invoked       </param>
        /// <returns> The expected state. </returns>
        private void UpdateExpectedState( string method )
        {
            //NOTE1: calculation of the expected CurrentState is indentical to the State-based
            //calculations specified in the Model.
            
            //First, update the running list of actions (methods), used later for verification.
            _actionList.Add(method);

            //Next, updated expected variables, according to the effect of a method given the
            //expectedCurrentState, which is a running estimate, not actual, because the
            //states are hypothetical.  The current state won't actually change until after the
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
                    //expectedEvent++;
                    _expectedIsPaused    = false; //For a Storyboard, a Begin restarts the Animation.
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
                    break;

                case "SkipToFill" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Filling;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Filling;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Filling;
                            break;
                        default:
                            break;
                    }
                    //expectedEvent--;
                    break;

                case "Stop" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Stopped;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Stopped;
                            //expectedEvent++;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Stopped;
                            //expectedEvent++;
                            break;
                        default:
                            break;
                    }
                    //expectedEvent--;
                    break;

                case "Remove" :
                    switch (_expectedCurrentState)
                    {
                        case ClockState.Stopped :
                            _expectedCurrentState    = ClockState.Stopped;
                            break;
                        case ClockState.Active  :
                            _expectedCurrentState    = ClockState.Stopped;
                            //expectedEvent++;
                            break;
                        case ClockState.Filling :
                            _expectedCurrentState    = ClockState.Stopped;
                            //expectedEvent++;
                            break;
                        default:
                            break;
                    }
                    //expectedEvent--;
                    break;

                default:
                    break;
            }
        }
        
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns>  </returns>
        private void OnTick(object sender, EventArgs e)
        {
            _dispatcherTickCount++;
            GlobalLog.LogStatus("-- Tick #" + _dispatcherTickCount);
            
            if (_dispatcherTickCount == 2)
            {
                _aTimer.Stop();
                
                //Go ahead and verify results; the TimeManager should have ticked for the animation.
                Signal("VerifyResults", (TestResult)(_cumulativeResult ? TestResult.Pass : TestResult.Fail));
            }
        }
        
        /// <summary>
        /// Invoked when the CurrentStateInvalidated event fires on the Animation.
        /// Used for debugging purposes.
        /// </summary>
        /// <returns>  </returns>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("-- CurrentStateInvalidated fired: " + ((Clock)sender).CurrentState);

            //Only counting event firings for Active state.
            //if (((Clock)sender).CurrentState == ClockState.Active)
            //{
            //    actualEvent++;
            //}
        }
        
        /// <summary>
        /// Calculates whether or not the beginPause flag should be set to true.
        /// </summary>
        /// <returns> A boolean value, indicating whether or not a Begin/Pause combo occured </returns>
        private bool DetermineStartPause()
        {
            //This routine is looking for scenarios in which CurrentState becomes Active, but
            //CurrentProgress/CurrentGlobalSpeed etc. do not advance.  This can happen when
            //a Begin followed by a Pause, with possibly intermediate Begin, Pause,
            //SkipToFill, and/or Resume calls.
            //However, with a Storyboard, a Begin after a Pause will restart the Animation.

            string action   = "";
            bool beginFound = false;
            bool pauseFound = false;
            
            for (int i=0; i<_actionList.Count; i+=1)
            {
                action = (string)_actionList[i];
                
                if (action == "Begin")
                {
                    beginFound = true;
                    if (pauseFound)
                    {
                        pauseFound = false;  //A Begin after a Pause will restart the Animation.
                    }
                }
                if (action == "Pause")
                {
                    if (beginFound)
                    {
                        //The Animation must have already been started for the Pause to count.
                        pauseFound = true;
                    }
                }
                if (action == "Resume")
                {
                    pauseFound = false;
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
        * Function:          ResetCase
        ******************************************************************************/
        /// <summary>
        /// Resets global variables after each test case.
        /// </summary>
        /// <returns></returns>
        private void ResetCase()
        {
            //Reset default values.
            _expectedCurrentState        = ClockState.Stopped;
            //expectedEvent               = 0;
            //actualEvent                 = 0;
            _storyboardBegun             = false;
            _expectedIsPaused            = false;
            _actionList.Clear();

            _animColor.CurrentStateInvalidated    -= new EventHandler(OnCurrentStateInvalidated);
            
            if (_storyboard != null)
            {
                GlobalLog.LogStatus("-- Removing Storyboard");
                _storyboard.Remove(_checkbox);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                _checkbox.BeginAnimation(_dp, null);
                _storyboard = null;
            }
        }
        #endregion
    }
}
