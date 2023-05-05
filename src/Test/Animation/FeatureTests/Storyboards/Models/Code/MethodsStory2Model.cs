// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Animation Test *******************************************************
*     Major Actions:      Verify a sequence of Animation methods applied on the separate Ticks
*                         [Technique:  Storyboard]
*                         [Animation:  StringAnimationUsingKeyFrames on ComboBox.Text]
*                         Sequence of actions:  the model code controls the number and type of 
*                         action invoked.  A DispatcherTimer is used to control the timing of
*                         the actions, i.e., an action will occur every 1.5 seconds.  To prevent the
*                         model code from immediately applying actions, a signal system is used:
*                         only when the Timer ticks will the next action be triggered.
*     Pass Conditions:    The test passes if Clock properties are returned correctly.
*                         Note that there may be multiple sub-tests for this overall test,
*                         each sub-test invoking a different ordered set of Animation methods.
*     How verified:       at the end of each test, several Clock properties are read and 
*                         compared to expected values.
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll, AnimationFramework.dll
*     Support Files:      SeparateTicksModel.xtc file, which specifies the test cases [must be available at run time]

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
    /// Verify Animation Methods on separate Ticks, using a StringAnimationUsingKeyFrames Storyboard
    /// </description>
    /// </summary>
    [Test(3, "Storyboard.Models.Storyboard", "MethodsStory2Model", SupportFiles=@"FeatureTests\Animation\SeparateTicksModel.xtc")]

    class MethodsStory2Model : WindowModel
    {
        #region Test case members

        private DispatcherTimer                 _aTimer;
        private int                             _dispatcherTickCount = 0;
        private int                             _testNumber          = 0;
        private Canvas                          _body;
        private ComboBox                        _combobox1;
        private Storyboard                      _storyboard;
        private DependencyProperty              _dp;
        private StringAnimationUsingKeyFrames   _animString;
        private ClockState                      _previousState       = ClockState.Stopped;
        private bool                            _previouslyBegun     = false;
        private bool                            _previouslyPaused    = false;
        private int                             _removeCount         = 0;

        private TimeSpan                        _DURATION_TIMESPAN   = TimeSpan.FromMilliseconds(7000);
        private TimeSpan                        _SEEK_TIME           = TimeSpan.FromMilliseconds(750);
        private TimeSpan                        _TIMER_INTERVAL      = TimeSpan.FromMilliseconds(1500);
        private double                          _MAX_VALUE           = 99999d;
        
        private string                          _methodApplied       = "";
        private ActualResults                   _actual;
        private ExpectedResults                 _expected;
        private bool                            _testPassed          = true;
        private bool                            _cumulativeResult    = true;
        private string                          _outString           = "";

        public struct ActualResults
        {
            public ClockState           currentState;
            public Nullable<double>     currentProgress;
            public Nullable<TimeSpan>   currentTime;
            public Nullable<double>     currentGlobalSpeed;
            public Nullable<int>        currentIteration;
            public bool                 isPaused;
            public bool                 eventFired;
            
            public ActualResults(   ClockState          state,
                                    Nullable<double>    progress,
                                    Nullable<TimeSpan>  time,
                                    Nullable<double>    speed,
                                    Nullable<int>       iteration,
                                    bool                paused,
                                    bool                fired
                                )
            {
               currentState         = state;
               currentProgress      = progress;
               currentTime          = time;
               currentGlobalSpeed   = speed;
               currentIteration     = iteration;
               isPaused             = paused;
               eventFired           = fired;
            }
        }
        
        public struct ExpectedResults
        {
            public Nullable<double>     currentProgress;
            public Nullable<TimeSpan>   currentTime;
            public Nullable<double>     currentGlobalSpeed;
            public Nullable<int>        currentIteration;
            public bool                 isPaused;
            public bool                 eventFired;
            public string               currentProgressString;
            public string               currentTimeString;
            public string               currentGlobalSpeedString;
            public string               currentIterationString;
            
            public ExpectedResults( Nullable<double>    progress,
                                    Nullable<TimeSpan>  time,
                                    Nullable<double>    speed,
                                    Nullable<int>       iteration,
                                    bool                paused,
                                    bool                fired,
                                    string              progressString,
                                    string              timeString,
                                    string              speedString,
                                    string              iterationString
                                  )
            {
               currentProgress          = progress;
               currentTime              = time;
               currentGlobalSpeed       = speed;
               currentIteration         = iteration;
               isPaused                 = paused;
               eventFired               = fired;
               currentProgressString    = progressString;
               currentTimeString        = timeString;
               currentGlobalSpeedString = speedString;
               currentIterationString   = iterationString;
            }
        }

        #endregion


        #region Contructors


        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1, 9)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    21, 25)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    27, 33)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    35, 41)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    43, 49)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    59, 66)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    69, 74)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    76, 82)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    84, 90)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    92, 98)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    843, 849)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1021, 1037)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1325, 1340)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1481, 1494)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1621, 1625)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1627, 1633)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1941, 1951)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    2043, 2049)]

        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1181, 1190)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1191, 1200)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1741, 1750)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1751, 1760)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1841, 1850)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1851, 1860)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    2141, 2150)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    2151, 2160)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    2241, 2250)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    2251, 2260)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    601, 610)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    611, 620)]



        public MethodsStory2Model(string xtcFileName) : this(xtcFileName, -1) { }

        public MethodsStory2Model(string xtcFileName, int testCaseNumber)
            : this(xtcFileName, testCaseNumber, testCaseNumber) { }

        public MethodsStory2Model(string xtcFileName, int startTestCaseNumber, int endTestCaseNumber)
            : this("", xtcFileName, startTestCaseNumber, endTestCaseNumber) { }

        public MethodsStory2Model(string modelName, string xtcFileName, int beginTestCaseNumber, int endTestCaseNumber)
            : base(modelName, xtcFileName, beginTestCaseNumber, endTestCaseNumber)
        {
            // happens once before all xtc test case run
            OnInitialize += new EventHandler(OnInitialize_Handler);
            // happens when each xtc test case run
            OnBeginCase += new StateEventHandler(OnBeginCase_Handler);
            OnGetCurrentState += new StateEventHandler(OnGetCurrentState_Handler);
            OnEndCase += new StateEventHandler(OnEndCase_Handler);

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
            _outString += "--OnInitialize\n";


            _actual   = new ActualResults  (ClockState.Stopped, 0d, new TimeSpan(0), 0d, 0, false, false);
            _expected = new ExpectedResults(0d, new TimeSpan(0), 0d, 0, false, false, "", "", "", "");
            
            Window.Title           = "Storyboard Methods Test";
            Window.Left            = 0;
            Window.Top             = 0;
            Window.Height          = 100;
            Window.Width           = 200;
            Window.WindowStyle     = WindowStyle.None;

            NameScope.SetNameScope(Window, new NameScope());

            _body  = new Canvas();
            _body.Background     = Brushes.Purple;
            Window.Content      = _body;

            //Create the to-be-animated control.
            _combobox1 = new ComboBox();
            _combobox1.IsEditable    = true;
            _combobox1.FontSize      = 24d;
            _combobox1.Background    = Brushes.Moccasin;
            _combobox1.Name          = "Combo";
            _body.Children.Add(_combobox1);
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
            //outString += "-- OnBeginCase\n" );

            _testNumber++;
            _outString +="-------------------------------------------------\n";
            _outString +="-------------------------------------------------\n";
            _outString +="BEGIN TEST #" + _testNumber + "\n";
            _outString +="-------------------------------------------------\n";
            
            _dp = ComboBox.TextProperty;
            
            //Create the Animation.
            _animString = CreateAnimation();

            //Create a Storyboard.
            _storyboard = new Storyboard();
            _storyboard.Name = "story";
            _storyboard.Children.Add(_animString);
            
            PropertyPath path = new PropertyPath("(0)", _dp);
            
            Storyboard.SetTargetProperty(_animString, path);
            Window.RegisterName(_combobox1.Name, _combobox1);
            Storyboard.SetTargetName(_animString, _combobox1.Name);
            
            //Start a separate Timer to control the timing of method invocation.
            _dispatcherTickCount = 0;  //Needs to be reset for every case.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = _TIMER_INTERVAL;
            _aTimer.Start();
            _outString +="-- DispatcherTimer Started --\n";
        }

        /******************************************************************************
        * Function:          OnGetCurrentState_Handler
        ******************************************************************************/
        /// <summary>
        /// This event handler is invoked after every action to retrieve current state of the
        ///modeled system.
        /// </summary>
        /// <returns></returns>
        private void OnGetCurrentState_Handler(object sender, StateEventArgs e)
        {
            _outString += "-- OnGetCurrentState\n";

            string stateStopped = "";
            string stateActive  = "";
            string stateFilling = "";
            
            if (_previouslyBegun)
            {
                _actual.currentState = _storyboard.GetCurrentState(_combobox1);

                switch (_actual.currentState)
                {
                    case ClockState.Stopped :
                        stateStopped = "stopped_true";
                        stateActive  = "active_false";
                        stateFilling = "filling_false";
                        break;
                    case ClockState.Active :
                        stateStopped = "stopped_false";
                        stateActive  = "active_true";
                        stateFilling = "filling_false";
                        break;
                    case ClockState.Filling :
                        stateStopped = "stopped_false";
                        stateActive  = "active_false";
                        stateFilling = "filling_true";
                        break;
                    default:
                        break;
                }

                if (_removeCount == 2)
                {
                    //HACK-HACK: ordinarily, the model code verifies the clock's CurrentState.
                    //However, to avoid changing the original .mbt, this check will be done here
                    //instead, when the scenario involves two Removes not followed by a Begin.
                    if (_actual.currentState != ClockState.Stopped)
                    {
                        _testPassed = false;
                    }
                }
                else
                {
                    //Pass the actual values of CurrentState for the model to verify.
                    e.State[ "stopped_state" ] = stateStopped;
                    e.State[ "active_state" ]  = stateActive;
                    e.State[ "filling_state" ] = stateFilling;
                }

                VerifyAnimation();
            }
            else
            {
                //METHOD APPLIED WHEN THE STORYBOARD HAS NOT YET BEGUN:
                //If a storyboard has not yet begun via the Begin() method, then GetCurrentState etc.
                //cannot be called on it because no Clock exists. So, will force a pass for
                //CurrentState. This could be taken care of in the model by ensuring that the first
                //method applied is always Begin(), but this way the test includes conditions where
                //the other methods are applied first.
                _previousState       = ClockState.Stopped;
            }
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
            //outString += "-- OnEndCase\n";
            
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
            //outString += "-- EndCase\n";

            //Stop the Timer and proceed to verify.
            _aTimer.Stop();

            if (_testPassed)
            {
                _outString +="END TEST #" + _testNumber + "                       RESULT: PASSED\n";
            }
            else
            {
                _outString +="END TEST #" + _testNumber + "                       RESULT: FAILED\n";
            }
            _outString +="-------------------------------------------------\n";

            _cumulativeResult = (_testPassed && _cumulativeResult);

            //Reset global variables, including testPassed.
            ResetCase();
            
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
        private bool begin_action( State endState, State inParameters, State outParameters )
        {
            _outString +="-- Begin Action --\n";

            _methodApplied       = "Begin";
            _removeCount         = 0;        //Restart.
            
            //Unlike Clocks, each Begin() method on a Storyboard creates a new Clock.  The old clock
            //remains as long as an event is bound to it.  So, the following logic will totally
            //remove the old Storyboard, events and all, before adding a new one.
            if (_previouslyBegun)
            {
                DeleteStoryboard();
            }
            _storyboard.Begin(_combobox1, HandoffBehavior.SnapshotAndReplace, true);
            _previouslyBegun     = true;
            
            //Wait before continuing to invoke the next method.
            WaitForSignal("VerifyResults");
            
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

            _methodApplied = "Pause";

            _storyboard.Pause(_combobox1);
            
            //Wait before continuing to invoke the next method.
            WaitForSignal("VerifyResults");

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

            _methodApplied = "Resume";

            _storyboard.Resume(_combobox1);
            
            //Wait before continuing to invoke the next method.
            WaitForSignal("VerifyResults");

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

            _methodApplied   = "Seek";

            _storyboard.Seek(_combobox1, _SEEK_TIME,TimeSeekOrigin.BeginTime);
            
            //Wait before continuing to invoke the next method.
            WaitForSignal("VerifyResults");

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

            _methodApplied   = "SkipToFill";

            _storyboard.SkipToFill(_combobox1);
            
            //Wait before continuing to invoke the next method.
            WaitForSignal("VerifyResults");

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

            _methodApplied = "Stop";

            _storyboard.Stop(_combobox1);
            
            //Wait before continuing to invoke the next method.
            WaitForSignal("VerifyResults");

            return true;
        }

        /******************************************************************************
        * Function:          remove_action
        ******************************************************************************/
        /// <summary>
        /// Handler for stop_action
        /// </summary>
        /// <param name="endState">      Expected end state.       </param>
        /// <param name="inParameters">  Input action parameters.  </param>
        /// <param name="outParameters"> Output action parameters. </param>
        /// <returns> Succeess status. </returns>
        private bool remove_action( State endState, State inParameters, State outParameters )
        {
            _outString +="-- Remove Action --\n";

            _methodApplied       = "Remove";
            _removeCount++;

            _storyboard.Remove(_combobox1);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            //Wait before continuing to invoke the next method.
            WaitForSignal("VerifyResults");

            return true;
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
            _outString +="-- Tick #" + _dispatcherTickCount + "\n";
            
            Signal("VerifyResults", (TestResult)(_cumulativeResult ? TestResult.Pass : TestResult.Fail));
        }
        
        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        /// <summary>
        /// Invoked when the CurrentStateInvalidated event fires on the Animation.
        /// Verification is carried out here.
        /// </summary>
        /// <returns>  </returns>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            _outString +="-- CurrentStateInvalidated fired: " + ((Clock)sender).CurrentState + "\n";
            _actual.eventFired = true;
        }
        
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateAnimation: Creates an Animation.
        /// </summary>
        /// <returns></returns>
        private StringAnimationUsingKeyFrames CreateAnimation()
        {
            StringAnimationUsingKeyFrames animString = new StringAnimationUsingKeyFrames();

            StringKeyFrameCollection SKFC = new StringKeyFrameCollection();
            SKFC.Add(new DiscreteStringKeyFrame("W" , KeyTime.FromPercent(0f)));
            SKFC.Add(new DiscreteStringKeyFrame("P" , KeyTime.FromPercent(0.1f)));
            SKFC.Add(new DiscreteStringKeyFrame("F" , KeyTime.FromPercent(0.15f)));
            animString.KeyFrames = SKFC;
                              
            animString.BeginTime            = new TimeSpan(0);
            animString.Duration             = new Duration(_DURATION_TIMESPAN);

            animString.CurrentStateInvalidated    += new EventHandler(OnCurrentStateInvalidated);
            
            return animString;
        }

        /******************************************************************************
        * Function:          VerifyAnimation
        ******************************************************************************/
        /// <summary>
        /// Verify multiple animation-related APIs.
        /// </summary>
        /// <param></param>
        /// <returns> </returns>
        protected void VerifyAnimation()
        {
            bool b1, b2, b3, b4, b5, b6;
            string actualCurrentTimeString ="";

            if (_previouslyBegun)
            {
                //Get actual values.
                _actual.currentProgress      = _storyboard.GetCurrentProgress(_combobox1);
                _actual.currentTime          = _storyboard.GetCurrentTime(_combobox1);
                _actual.currentGlobalSpeed   = _storyboard.GetCurrentGlobalSpeed(_combobox1);
                _actual.currentIteration     = _storyboard.GetCurrentIteration(_combobox1);
                _actual.isPaused             = _storyboard.GetIsPaused(_combobox1);
            }
            else
            {
                //Cannot retrieve actual State/Time/GlobalSpeed from a Storyboard that
                //has not started, so will force a Stopped state.
                _actual.currentProgress      = null;
                _actual.currentTime          = null;
                _actual.currentGlobalSpeed   = null;
                _actual.currentIteration     = null;
                _actual.isPaused             = false;
            }
            
            //Get expected values.
            DetermineExpectedValues();

            //Pass or fail the test case.
            //NOTE1: CurrentState is verified by the model code, not via the following checks.
            //NOTE2: If a non-zero/non-null CurrentProgress/CurrentTime expected, not checking
            //for exact values.
            if (_expected.currentProgress == _MAX_VALUE)
            {
                b1 = ( (_actual.currentProgress >= 0) && (_actual.currentProgress < 1) );
            }
            else
            {
                b1 = (_actual.currentProgress == _expected.currentProgress);
            }
            
            if (_expected.currentTime == TimeSpan.FromMilliseconds(_MAX_VALUE))
            {
                b2 = ( (_actual.currentTime >= new TimeSpan(0)) && (_actual.currentTime < ((TimeSpan)_DURATION_TIMESPAN)) );
            }
            else
            {
                b2 = (_actual.currentTime == _expected.currentTime);
            }
            if (_actual.currentTime == null)
            {
                actualCurrentTimeString = "";
            }
            else
            {
                actualCurrentTimeString = ((TimeSpan)_actual.currentTime).TotalMilliseconds.ToString();
            }

            b3 = (_actual.currentGlobalSpeed     == _expected.currentGlobalSpeed);
            b4 = (_actual.currentIteration       == _expected.currentIteration);
            
            if (_removeCount > 0)
            {
                //If the Remove method has been applied, and the clock not begun again, IsPaused 
                //is indeterminate (see resolution of 
                b5 = true;
            }
            else
            {
                b5 = (_actual.isPaused               == _expected.isPaused);
            }
            
            //DISABLING event checking, until the Remove method is available.
            //b6 = (actual.eventFired             == expected.eventFired);
            b6 = true;

            _outString +="       CurrentProgress     - Act: " + _actual.currentProgress    + " Exp: " + _expected.currentProgressString + "\n";
            _outString +="       CurrentTime         - Act: " + actualCurrentTimeString   + " Exp: " + _expected.currentTimeString + "\n";
            _outString +="       CurrentGlobalSpeed  - Act: " + _actual.currentGlobalSpeed + " Exp: " + _expected.currentGlobalSpeedString + "\n";
            _outString +="       CurrentInteration   - Act: " + _actual.currentIteration   + " Exp: " + _expected.currentIterationString + "\n";
            _outString +="       IsPaused            - Act: " + _actual.isPaused           + " Exp: " + _expected.isPaused + "\n";
            //DISABLING. outString +="-EVENT FIRING        - Act: " + actual.eventFired         + " Exp: " + expected.eventFired + "\n";
            _outString +="-EVENT FIRING        - NYI\n";

            bool tickResult = (b1 && b2 && b3 && b4 && b5 && b6);
            _testPassed = tickResult && _testPassed;

            string resultString = "Tick #" + _dispatcherTickCount + ": " + tickResult + "  [" + b1 + "/" + b2 + "/" + b3 + "/" + b4 + "/" + b5 + "/" + b6 + "]";
            _outString += "-------------------------------------------------\n";
            _outString += resultString + "\n";
            _outString += "-------------------------------------------------\n";
            
            //Reset global variables for the next Tick.
            ResetTick();
        }
        
        /******************************************************************************
        * Function:          DetermineExpectedValues
        ******************************************************************************/
        /// <summary>
        /// Calculates the expected state, given an Animation method applied and the CurrentState.
        /// </summary>
        /// <returns></returns>
        private void DetermineExpectedValues()
        {
            //NOTE: MAX_VALUE is used as a flag to indicate the type of verification used later.
            
            if (!_previouslyBegun)
            {
                //Cannot retrieve actual State/Time/GlobalSpeed from a Storyboard that
                //has not started, so will "expect" a Stopped state.
                _outString +="-- CurrentState:          Storyboard not started\n";
                _expected.currentProgress          = null;
                _expected.currentTime              = null;
                _expected.currentGlobalSpeed       = null;
                _expected.currentIteration         = null;
                _expected.currentProgressString    = "";
                _expected.currentTimeString        = ""; 
                _expected.currentGlobalSpeedString = "";
                _expected.currentIterationString   = "";
            }
            else
            {
                switch (_methodApplied)
                {
                    case "Begin" :
                        _previouslyPaused                    = false; //For a Storyboard, Begin reapplies the Clock.
                        _expected.isPaused                   = false;
                        _expected.eventFired                 = true;
                        ExpectActive();
                        break;

                    case "Pause" :
                        _previouslyPaused                    = true;
                        _expected.eventFired                 = false;
                        ExpectPreviousState();
                        
                        if (_actual.currentState != ClockState.Stopped)
                        {
                            _expected.isPaused                   = true;
                            _expected.currentGlobalSpeed         = 0;
                            _expected.currentGlobalSpeedString   = "0";
                        }
                        break;

                    case "Resume" :
                        _previouslyPaused                    = false;
                        _expected.isPaused                   = false;
                        _expected.eventFired                 = false;
                        ExpectPreviousState();
                        if (_previouslyPaused && _actual.currentState != ClockState.Stopped)
                        {
                            _expected.currentGlobalSpeed         = 1;
                            _expected.currentGlobalSpeedString   = "1";
                        }
                        break;

                    case "Seek" :
                        _expected.eventFired = true;
                        if (_removeCount == 2)
                        {
                            //Once a Storyboard is (twice) removed and GC'ed, must Begin() to make it accessible.
                            ExpectStopped();
                        }
                        else
                        {
                            ExpectActive();
                        }
                        break;

                    case "SkipToFill" :
                        _expected.eventFired = true;
                        if (_removeCount == 2)
                        {
                            //Once a Storyboard is (twice) removed and GC'ed, must Begin() to make it accessible.
                            ExpectStopped();
                        }
                        else
                        {
                            ExpectFilling();
                        }
                        break;

                    case "Stop" :
                        if (_previousState == ClockState.Stopped)
                        {
                            _expected.eventFired = false;
                        }
                        else
                        {
                            _expected.eventFired = true;
                        }
                        ExpectStopped();
                        break;

                    case "Remove" :
                        if (_previousState == ClockState.Stopped)
                        {
                            _expected.eventFired = false;
                        }
                        else
                        {
                            _expected.eventFired = true;
                        }
                        ExpectStopped();
                        break;

                    default:
                        break;
                }

                //A paused animation will remain paused, regardless of other actions, with two 
                //exceptions:  Begin (which recreates an AnimationClock) and Resume.  This means 
                //overriding expected values [previouslyPaused set to true if a Pause action was just
                //applied]. Exception: if the CurrentState is Stopped, CurrentGlobalSpeed is null.
                if ( _previouslyPaused && _methodApplied != "Begin")
                {
                    if ( _actual.currentState != ClockState.Stopped )
                    {
                        _expected.currentGlobalSpeed         = 0;
                        _expected.currentGlobalSpeedString   = "0";
                    }
                    _expected.isPaused                   = true;
                }
                else
                {
                    _expected.isPaused                   = false;
                }

                _outString +="-- Previous CurrentState: " + _previousState + "\n";
                _outString +="-- CurrentState:          " + _actual.currentState + "\n";
                _outString +="-- methodApplied:         " + _methodApplied + "\n";
            }
        }

        /******************************************************************************
        * Function:          ExpectPreviousState
        ******************************************************************************/
        private void ExpectPreviousState()
        {
            switch (_previousState)
            {
                case ClockState.Stopped :
                    ExpectStopped();
                    break;
                case ClockState.Active  :
                    ExpectActive();
                    break;
                case ClockState.Filling :
                    ExpectFilling();
                    break;
                default:
                    break;
            }
        }

        /******************************************************************************
        * Function:          ExpectActive
        ******************************************************************************/
        private void ExpectActive()
        {
            _expected.currentProgress          = _MAX_VALUE;
            _expected.currentTime              = TimeSpan.FromMilliseconds(_MAX_VALUE);
            _expected.currentGlobalSpeed       = 1;
            _expected.currentIteration         = 1;
            _expected.currentProgressString    = ">0";
            _expected.currentTimeString        = ">0";
            _expected.currentGlobalSpeedString = "1";
            _expected.currentIterationString   = "1";
        }

        /******************************************************************************
        * Function:          ExpectFilling
        ******************************************************************************/
        private void ExpectFilling()
        {
            _expected.currentProgress          = 1;
            _expected.currentTime              = _DURATION_TIMESPAN;
            _expected.currentGlobalSpeed       = 0;
            _expected.currentIteration         = 1;
            _expected.currentProgressString    = "1";
            _expected.currentTimeString        = _DURATION_TIMESPAN.TotalMilliseconds.ToString(); 
            _expected.currentGlobalSpeedString = "0";
            _expected.currentIterationString   = "1";
        }

        /******************************************************************************
        * Function:          ExpectStopped
        ******************************************************************************/
        private void ExpectStopped()
        {
            _expected.currentProgress          = null;
            _expected.currentTime              = null;
            _expected.currentGlobalSpeed       = null;
            _expected.currentIteration         = null;
            _expected.currentProgressString    = "";
            _expected.currentTimeString        = ""; 
            _expected.currentGlobalSpeedString = "";
            _expected.currentIterationString   = "";
            _expected.isPaused                 = false;
        }
        
        /******************************************************************************
        * Function:          ResetTick
        ******************************************************************************/
        /// <summary>
        /// Resets global variables after each Tick, i.e. after each Method has been applied.
        /// </summary>
        /// <returns></returns>
        private void ResetTick()
        {
            //Save the state of the Animation as previousState, to be considered after the next Action.
            _previousState = _actual.currentState;
            
            //Now reset the structs.
            _actual          = new ActualResults  (ClockState.Stopped, 0d, new TimeSpan(0), 0d, 0, false, false);
            _expected        = new ExpectedResults(0d, new TimeSpan(0), 0d, 0, false, false, "", "", "", "");
        }

        /******************************************************************************
        * Function:          DeleteStoryboard
        ******************************************************************************/
        /// <summary>
        /// Truly removes a Storyboard by calling Remove() and then garbage-collecting.
        /// The purpose of doing so is to ensure there is only one Storyboard at any given time.
        /// </summary>
        /// <returns></returns>
        private void DeleteStoryboard()
        {
            _storyboard.CurrentStateInvalidated    -= new EventHandler(OnCurrentStateInvalidated);
            _storyboard.Remove(_combobox1);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            //Bind the event for subsequent actions.
            _storyboard.CurrentStateInvalidated   += new EventHandler(OnCurrentStateInvalidated);
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
            _testPassed          = true;
            _previousState       = ClockState.Stopped;
            _methodApplied       = "";
            _previouslyBegun     = false;
            _previouslyPaused    = false;
            _removeCount         = 0;
            
            ResetTick();
            
            _animString.CurrentStateInvalidated    -= new EventHandler(OnCurrentStateInvalidated);
            
            if (_storyboard != null)
            {
                _storyboard.Remove(_combobox1);
                _combobox1.BeginAnimation(_dp, null);
                _storyboard = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        #endregion
    }
}
