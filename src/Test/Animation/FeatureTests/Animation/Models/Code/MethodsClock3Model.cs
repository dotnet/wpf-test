// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
* ***************  MIL Animation Test *******************************************************
*     Major Actions:      Verify a sequence of Animation methods applied on the separate Ticks
*                         [Technique:  CreateClock/ApplyAnimationClock]
*                         [Animation:  MatrixAnimationUsingPath on a Path's MatrixTransform.MatrixProperty]
*                         Sequence of actions:  the model code controls the number and type of 
*                         action invoked.  A DispatcherTimer is used to control the timing of
*                         the actions, i.e., an action will occur every .5 seconds.  To prevent the
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
*     Support Files:      AnimationMethods1.xtc file, which specifies the test cases [must be available at run time]
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
using System.Windows.Shapes;
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
    /// Verify Animation Methods on separate Ticks, using a MatrixAnimationUsingPath
    /// </description>
    /// </summary>
    [Test(3, "Animation.Models.AnimationClock", "MethodsClock3Test", SupportFiles=@"FeatureTests\Animation\SeparateTicksModel.xtc")]

    class MethodsClock3Model : WindowModel
    {
        #region Test case members

        private DispatcherTimer             _aTimer;
        private int                         _dispatcherTickCount = 0;
        private int                         _testNumber          = 0;
        private Canvas                      _body;
        private Decorator                   _dec;
        private Path                        _path1;
        private Button                      _button;
        private MatrixTransform             _MT;
        private MatrixAnimationUsingPath    _animMatrix;
        private AnimationClock              _clock;
        private ClockState                  _previousState           = ClockState.Stopped;
        private bool                        _previouslyPaused        = false;
        private bool                        _zeroState               = false;
        private int                         _expectedEventFirings    = 0;
        private int                         _actualEventFirings      = 0;
        private bool                        _actionPerformed         = false;

        private TimeSpan                    _DURATION_TIMESPAN       = TimeSpan.FromMilliseconds(7000);
        private TimeSpan                    _SEEK_TIME               = TimeSpan.FromMilliseconds(750);
        private TimeSpan                    _TIMER_INTERVAL          = TimeSpan.FromMilliseconds(1500);
        private double                      _MAX_VALUE               = 99999d;
        
        private string                      _methodApplied           = "";
        private ActualResults               _actual;
        private ExpectedResults             _expected;
        private bool                        _testPassed              = true;
        private bool                        _cumulativeResult        = true;
        private string                      _outString               = "";

        public struct ActualResults
        {
            public ClockState           currentState;
            public Nullable<double>     currentProgress;
            public Nullable<TimeSpan>   currentTime;
            public Nullable<double>     currentGlobalSpeed;
            public Nullable<int>        currentIteration;
            public bool                 isPaused;
            
            public ActualResults(   ClockState          state,
                                    Nullable<double>    progress,
                                    Nullable<TimeSpan>  time,
                                    Nullable<double>    speed,
                                    Nullable<int>       iteration,
                                    bool                paused
                                )
            {
               currentState         = state;
               currentProgress      = progress;
               currentTime          = time;
               currentGlobalSpeed   = speed;
               currentIteration     = iteration;
               isPaused             = paused;
            }
        }
        
        public struct ExpectedResults
        {
            public Nullable<double>     currentProgress;
            public Nullable<TimeSpan>   currentTime;
            public Nullable<double>     currentGlobalSpeed;
            public Nullable<int>        currentIteration;
            public bool                 isPaused;
            public string               currentProgressString;
            public string               currentTimeString;
            public string               currentGlobalSpeedString;
            public string               currentIterationString;
            
            public ExpectedResults( Nullable<double>    progress,
                                    Nullable<TimeSpan>  time,
                                    Nullable<double>    speed,
                                    Nullable<int>       iteration,
                                    bool                paused,
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
               currentProgressString    = progressString;
               currentTimeString        = timeString;
               currentGlobalSpeedString = speedString;
               currentIterationString   = iterationString;
            }
        }

        #endregion


        #region Contructors

        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1,   15)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    16,   30)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    31,   45)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    46,   60)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    61,   75)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    76,   90)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    261,   275)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    476,   490)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    676,   690)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    791,   800)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    901,   915)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1031,   1045)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1146,   1160)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1276,   1290)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1401,   1415)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1516,   1530)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1761,   1775)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1876,   1890)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    1991,   2000)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    2216,   2230)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    2376,   2390)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    2531,   2545)]
        [Variation("SeparateTicksModel", "SeparateTicksModel.xtc",    2676,   2690)]

        public MethodsClock3Model(string xtcFileName) : this(xtcFileName, -1) { }

        public MethodsClock3Model(string xtcFileName, int testCaseNumber)
            : this(xtcFileName, testCaseNumber, testCaseNumber) { }

        public MethodsClock3Model(string xtcFileName, int startTestCaseNumber, int endTestCaseNumber)
            : this("", xtcFileName, startTestCaseNumber, endTestCaseNumber) { }

        public MethodsClock3Model(string modelName, string xtcFileName, int beginTestCaseNumber, int endTestCaseNumber)
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
            _outString += "-- OnInitialize\n";


            ClearStructs();
            
            Window.Title           = "Animation Methods Test";
            Window.Left            = 0;
            Window.Top             = 0;
            Window.Height          = 300;
            Window.Width           = 150;
            Window.WindowStyle     = WindowStyle.None;

            _body  = new Canvas();
            _body.Background     = Brushes.LemonChiffon;
            Window.Content      = _body;

            //Create the to-be-animated control.
            _button = new Button();
            _button.Height      = 20d;
            _button.Width       = 20d;
            _button.Content     = "A";
            _button.FontSize    = 16d;

            //Create a Path, with a PathGeometry.
            _path1  = CreatePath();
            _body.Children.Add(_path1);

            _dec = new Decorator();
            _body.Children.Add(_dec);
            _dec.Child = _button;
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
            //outString += "-- OnBeginCase\n";

            _testNumber++;
            _outString +="-------------------------------------------------\n";
            _outString +="-------------------------------------------------\n";
            _outString +="BEGIN TEST #" + _testNumber + "\n";
            _outString +="-------------------------------------------------\n";
            
            //Create the Animation.
            _animMatrix = CreateAnimation();
            
            //Create a MatrixTransform and add it to the Decorator.
            _MT = CreateMatrixTransform(ref _dec);

            //Create an AnimationClock and apply it to the MatrixTransform.
            _clock = _animMatrix.CreateClock();
            _MT.ApplyAnimationClock(MatrixTransform.MatrixProperty, _clock);
            
            //Start a separate Timer to control the timing of method invocation.
            _dispatcherTickCount = 0;  //Needs to be reset for every case.
            _aTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
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
            //Need to skip the first time this fires, which happens before the first Action.
            if (_actionPerformed)
            {
                _outString += "-- OnGetCurrentState\n";

                string stateStopped = "";
                string stateActive  = "";
                string stateFilling = "";

                _actual.currentState = _clock.CurrentState;

                switch (_actual.currentState)
                {
                    case ClockState.Stopped :
                        stateStopped    = "stopped_true";
                        stateActive     = "active_false";
                        stateFilling    = "filling_false";
                        break;
                    case ClockState.Active :
                        stateStopped    = "stopped_false";
                        stateActive     = "active_true";
                        stateFilling    = "filling_false";
                        break;
                    case ClockState.Filling :
                        stateStopped    = "stopped_false";
                        stateActive     = "active_false";
                        stateFilling    = "filling_true";
                        break;
                    default:
                        break;
                }

                e.State[ "stopped_state" ] = stateStopped;
                e.State[ "active_state" ]  = stateActive;
                e.State[ "filling_state" ] = stateFilling;

                //Verify results in addition to CurrentState.
                VerifyAnimation();
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
        /// EndCase: Overrides the Model's EndCase, to verify the test case result.
        /// </summary>
        /// <returns>True</returns>
        public override bool EndCase(State endCase)
        {
            _outString += "EndCase\n";

            //Stop the Timer and proceed to verify.
            _aTimer.Stop();

            //Remove the AnimationClock and the animation's CurrentStateInvalidated event handler.
            _animMatrix.CurrentStateInvalidated    -= new EventHandler(OnCurrentState);
            _MT.ApplyAnimationClock(MatrixTransform.MatrixProperty, null);

            //Final event check.
            _outString += "Events Result : " + (_actualEventFirings == _expectedEventFirings) + "  - Act: " + _actualEventFirings         + " Exp: " + _expectedEventFirings + "\n";
            _outString += "-------------------------------------------------\n";
            if (_actualEventFirings != _expectedEventFirings)
            {
                _testPassed = (false && _testPassed);
            }

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
            
            //Reset default values.
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
        private bool begin_action( State endState, State inParameters, State outParameters)
        {
            _outString +="-- Begin Action --\n";

            _clock.Controller.Begin();
            _methodApplied = "Begin";
            _actionPerformed = true;

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
        private bool pause_action( State endState, State inParameters, State outParameters)
        {
            _outString +="-- Pause Action --\n";

            _clock.Controller.Pause();
            _methodApplied = "Pause";
            _actionPerformed = true;

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
        private bool resume_action( State endState, State inParameters, State outParameters)
        {
            _outString +="-- Resume Action --\n";

            _clock.Controller.Resume();
            _methodApplied = "Resume";
            _actionPerformed = true;

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
        private bool seek_action( State endState, State inParameters, State outParameters)
        {
            _outString +="-- Seek Action --\n";

            _clock.Controller.Seek(_SEEK_TIME,TimeSeekOrigin.BeginTime);
            _methodApplied = "Seek";
            _actionPerformed = true;

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
        private bool skiptofill_action( State endState, State inParameters, State outParameters)
        {
            _outString +="-- SkipToFill Action --\n";

            _clock.Controller.SkipToFill();
            _methodApplied = "SkipToFill";
            _actionPerformed = true;

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
        private bool stop_action( State endState, State inParameters, State outParameters)
        {
            _outString +="-- Stop Action --\n";

            _clock.Controller.Stop();
            _methodApplied = "Stop";
            _actionPerformed = true;

            //Wait before continuing to invoke the next method.
            WaitForSignal("VerifyResults");

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
        /// <returns> Succeess status. </returns>
        private bool remove_action( State endState, State inParameters, State outParameters)
        {
            _outString +="-- Remove Action --\n";

            _clock.Controller.Remove();
            _methodApplied = "Remove";
            _actionPerformed = true;

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
            _outString +="-- Tick #" + _dispatcherTickCount + " -- Now verify results --\n";

            Signal("VerifyResults", (TestResult)(_cumulativeResult ? TestResult.Pass : TestResult.Fail));
        }
        
        /******************************************************************************
        * Function:          OnCurrentState
        ******************************************************************************/
        /// <summary>
        /// Invoked when the CurrentStateInvalidated event fires on the Animation.
        /// Verification is carried out here.
        /// </summary>
        /// <returns>  </returns>
        private void OnCurrentState(object sender, EventArgs e)
        {
            _outString +="-- CurrentStateInvalidated fired: " + ((Clock)sender).CurrentState + "\n";
            _actualEventFirings++;
        }
        
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateAnimation: Creates an Animation.
        /// </summary>
        /// <returns></returns>
        private MatrixAnimationUsingPath CreateAnimation()
        {
            MatrixAnimationUsingPath animMatrix = new MatrixAnimationUsingPath();
            animMatrix.BeginTime                = null;
            animMatrix.Duration                 = new Duration(_DURATION_TIMESPAN);
            animMatrix.IsAdditive               = false;
            animMatrix.IsAngleCumulative        = true;
            animMatrix.DoesRotateWithTangent    = false;
            animMatrix.IsOffsetCumulative       = true;

            //Add a PathGeometry to the MatrixAnimation.
            //It is also assigned to a Path element to render a path the animation will follow.
            animMatrix.PathGeometry = CreatePathGeometry();
            animMatrix.CurrentStateInvalidated    += new EventHandler(OnCurrentState);

            return animMatrix;
        }
        
        /******************************************************************************
        * Function:          CreateMatrixTransform
        ******************************************************************************/
        /// <summary>
        /// CreateMatrixTransform: Adds a TransformGroup to a Decorator.
        /// </summary>
        /// <returns>MatrixTransform</returns>
        private MatrixTransform CreateMatrixTransform(ref Decorator decorator)
        {
            //Create a TranslateTransform.
            TranslateTransform TT = new TranslateTransform();
            TT.X    = -7.5;
            TT.Y    = -7.5;

            //Create a MatrixTransform.
            _MT = new MatrixTransform();

            //Create a TranformGroup, and add the two Transforms to it.
            TransformGroup TGRP = new TransformGroup();
            TGRP.Children.Add(TT);
            TGRP.Children.Add(_MT);

            //Create a Decorator and add the TransformGroup to it.
            decorator.Margin          = new Thickness(15,15,15,15);
            decorator.RenderTransform = TGRP;

            return _MT;
        }
        
        /******************************************************************************
        * Function:          CreatePath
        ******************************************************************************/
        /// <summary>
        /// CreatePath: Creates and returns a Path with a PathGeometry.
        /// </summary>
        /// <returns>Path</returns>
        private Path CreatePath()
        {
            Path path  = new Path();
            Canvas.SetTop  (path, 15d);
            Canvas.SetLeft (path, 15d);
            path.Stroke             = Brushes.SteelBlue;
            path.StrokeThickness    = 2d;

            //Create a PathGeometry and assign it to the Path's Data property, to display the path.
            //It is also assigned to the MatrixAnimation.
            path.Data   = CreatePathGeometry();

            return path;
        }
        
        /******************************************************************************
        * Function:          CreatePathGeometry
        ******************************************************************************/
        /// <summary>
        /// CreatePathGeometry: Creates and returns a PathGeometry.
        /// </summary>
        /// <returns>PathGeometry</returns>
        private PathGeometry CreatePathGeometry()
        {
            //Create a PathFigureCollection.
            PathFigureCollection PFC1 = SpecialObjects.CreatePathFigureCollection();

            //Create a PathGeometry: animation.
            //Assign it a ScaleTransform and PathFigureCollection.
            ScaleTransform scaleTransform1 = new ScaleTransform();
            scaleTransform1.ScaleX      = 1.5;
            scaleTransform1.ScaleY      = 1.5;

            PathGeometry pathGeometry   = new PathGeometry();
            pathGeometry.Transform      = scaleTransform1;
            pathGeometry.Figures        = PFC1;

            return pathGeometry;
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
            bool b1, b2, b3, b4, b5;
            string actualCurrentTimeString ="";

            //Get actual values.
            _actual.currentProgress      = _clock.CurrentProgress;
            _actual.currentTime          = _clock.CurrentTime;
            _actual.currentGlobalSpeed   = _clock.CurrentGlobalSpeed;
            _actual.currentIteration     = _clock.CurrentIteration;
            _actual.isPaused             = _clock.IsPaused;

            //Get expected values.
            DetermineExpectedValues();

            //Pass or fail the test case.
            //NOTE1: CurrentState is verified by the model code, not via the following checks.
            //NOTE2: If a non-zero/non-null CurrentProgress/CurrentTime expected (MAX_VALUE found),
            //will not check for exact values.
            if (_expected.currentProgress == _MAX_VALUE)
            {
                b1 = ( (_actual.currentProgress > 0) && (_actual.currentProgress < 1) );
            }
            else
            {
                b1 = (_actual.currentProgress == _expected.currentProgress);
            }
            if (_expected.currentTime == TimeSpan.FromMilliseconds(_MAX_VALUE))
            {
                b2 = ( (_actual.currentTime > new TimeSpan(0)) && (_actual.currentTime < ((TimeSpan)_DURATION_TIMESPAN)) );
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
            b5 = (_actual.isPaused               == _expected.isPaused);

            _outString +="       CurrentProgress     - Act: " + _actual.currentProgress    + " Exp: " + _expected.currentProgressString + "\n";
            _outString +="       CurrentTime         - Act: " + actualCurrentTimeString   + " Exp: " + _expected.currentTimeString + "\n";
            _outString +="       CurrentGlobalSpeed  - Act: " + _actual.currentGlobalSpeed + " Exp: " + _expected.currentGlobalSpeedString + "\n";
            _outString +="       CurrentIteration    - Act: " + _actual.currentIteration   + " Exp: " + _expected.currentIterationString + "\n";
            _outString +="       IsPaused            - Act: " + _actual.isPaused           + " Exp: " + _expected.isPaused + "\n";

            bool tickResult = (b1 && b2 && b3 && b4 && b5);
            _testPassed = tickResult && _testPassed;

            _outString +="-------------------------------------------------\n";
            _outString +="Tick #" + _dispatcherTickCount + ": " + tickResult + "  [" + b1 + "/" + b2 + "/" + b3 + "/" + b4 + "/" + b5 + "]\n";
            _outString +="-------------------------------------------------\n";

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

            switch (_methodApplied)
            {
                case "Begin" :
                    _expectedEventFirings++;
                    if (FoundPauseBegin())
                    {
                        ExpectZero();
                    }
                    else
                    {
                        ExpectActive();
                    }
                    break;
                
                case "Pause" :
                    _previouslyPaused = true;
                    if (_zeroState)
                    {
                        ExpectZero();
                    }
                    else
                    {
                        ExpectPreviousState();
                    }
                    break;
                
                case "Resume" :
                    _previouslyPaused    = false;
                    _zeroState           = false;
                    ExpectPreviousState();
                    break;
                
                case "Seek" :
                    _zeroState           = false;
                    _expectedEventFirings++;
                    ExpectActive();
                    break;

                case "SkipToFill" :
                    _zeroState           = false;
                    _expectedEventFirings++;
                    ExpectFilling();
                    break;

                case "Stop" :
                    _zeroState           = false;
                    if (_previousState != ClockState.Stopped)
                    {
                        _expectedEventFirings++;
                    }
                    ExpectStopped();
                    break;

                case "Remove" :
                    _zeroState           = false;
                    if (_previousState != ClockState.Stopped)
                    {
                        _expectedEventFirings++;
                    }
                    ExpectStopped();
                    break;

                default:
                    break;
            }

            //A paused animation will remain paused, regardless of other actions (except Resume).
            //This means overriding expected values [previouslyPaused set to true if a Pause action
            //was just applied]. Exception: if the CurrentState is Stopped, CurrentGlobalSpeed is null.
            if ( _previouslyPaused )
            {
                if ( _clock.CurrentState != ClockState.Stopped )
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
            _outString +="-- CurrentState:          " + _clock.CurrentState + "\n";
            _outString +="-- IsPaused:              " + _clock.IsPaused + "\n";
            _outString +="-- methodApplied:         " + _methodApplied + "\n";
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
        }

        /******************************************************************************
        * Function:          ExpectZero
        ******************************************************************************/
        private void ExpectZero()
        {
            _expected.currentProgress          = 0;
            _expected.currentTime              = TimeSpan.FromMilliseconds(0);
            _expected.currentGlobalSpeed       = 0;
            _expected.currentIteration         = 1;
            _expected.currentProgressString    = "0";
            _expected.currentTimeString        = "0";
            _expected.currentGlobalSpeedString = "0";
            _expected.currentIterationString   = "1";
        }

        /******************************************************************************
        * Function:          FoundPauseBegin
        ******************************************************************************/
        /// <summary>
        /// Looks at the action history to determine whether a Begin action is affected
        /// by a previous Pause action.
        /// </summary>
        /// <returns> A boolean value, indicating whether or not a Pause/Begin combo occured </returns>
        private bool FoundPauseBegin()
        {
            //This routine is looking for scenarios in which CurrentState becomes Active, but
            //CurrentProgress/CurrentGlobalSpeed etc. do not advance.  This can happen when
            //a Begin followed by a Pause, with possibly intermediate Begin, Pause,
            //or SkipToFill calls.

            if (_methodApplied == "Begin" && _previouslyPaused)
            {
                _zeroState = true;
                return true;
            }
            else
            {
                return false;
            }
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

            ClearStructs();
        }

        /******************************************************************************
        * Function:          ClearStructs
        ******************************************************************************/
        /// <summary>
        /// Resets global structs to default values.
        /// </summary>
        /// <returns></returns>
        private void ClearStructs()
        {
            _actual          = new ActualResults  (ClockState.Stopped, 0d, new TimeSpan(0), 0d, 0, false);
            _expected        = new ExpectedResults(0d, new TimeSpan(0), 0d, 0, false, "", "", "", "");
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
            _actionPerformed         = false;
            _testPassed              = true;
            _previousState           = ClockState.Stopped;
            _previouslyPaused        = false;
            _zeroState               = false;
            _actualEventFirings      = 0;
            _expectedEventFirings    = 0;
            ResetTick();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            _clock                   = null;

            //Forcing a delay before each test case to allow time for Removed clocks to be GC'd in
            //any previous test case.
//            WaitForSignal(2500);
        }
        #endregion
    }
}
