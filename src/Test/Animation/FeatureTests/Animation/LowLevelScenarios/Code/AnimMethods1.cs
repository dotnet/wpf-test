// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Methods in Events: Animation Methods *****************
*   Description:
*          Tests that certain Animations-related Methods can be called via all Animation Events.
*          These particular methods are on the UIElement.
*          The input parameters define the following combinations of Methods x Events:
*          ( three events are available: CurrentStateInvalidated, 
*          CurrentGlobalSpeedInvalidated, CurrentTimeInvalidated.  The old events are therefore
*          defined by checking certain properties via these three events.)
*               "Events" (8):  Begun, Paused, Seeked, Resumed, Repeated, Reversed, Changed, Ended.
*               Methods  (2):  BeginAnimation, GetAnimationBaseValue
*          The Animation consists of a DoubleAnimation on a TextBox's Top property.
*   Pass Conditions:
*          The test case will Pass if the events fire appropriately when the methods are invoked,
*          and the relevant APIs return the correct value.
*   How verified:
*          The result of the comparisons between actual and expected values is passed to
*          the logger.
*
*   Framework:          A CLR executable is created.
*   Area:               Animation/Timing
*   Dependencies:       TestRuntime.dll

*/
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Shapes;
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
    [Test(2, "Animation.LowLevelScenarios.Methods", "AnimMethods1Test")]

    class AnimMethods1Test : WindowTest
    {
        #region Test case members
        
        private Canvas                          _body;
        private TextBox                         _TB;
        private DoubleAnimation                 _anim1;
        private DoubleAnimation                 _anim2;
        private AnimationClock                  _anim1Clock          = null;
        private AnimationClock                  _anim2Clock          = null;
        private Clock                           _TLC;
        private int                             _BEGIN_TIME          = 2000;
        private int                             _DURATION_TIME       = 1000;
        private int                             _DURATION_TIME_TL1   = 400;  //Timeline used for invoking anim methods.
        private int                             _DURATION_TIME_TL2   = 5000; //Timeline used for verification.
        private int                             _SEEK_TIME           = 150;
        private double                          _ASSIGNED_TOP        = 50d;
        private string                          _baseValue;
        private string                          _expectedEvents      = "";

        private int                             _beg                 = 0;
        private int                             _end                 = 0;
        private int                             _cha                 = 0;
        private int                             _pau                 = 0;
        private int                             _rep                 = 0;
        private int                             _res                 = 0;
        private int                             _rev                 = 0;

        private int                             _iterationCount      = 0;
        private double                          _lastSpeed           = 0;
        private bool                            _lastPaused          = false;
        private bool                            _seekInvoked         = false;
        private bool                            _resumedInvoked      = false;
        private bool                            _hasAnimation        = true;
        private string                          _animEvent           = null;
        private string                          _animMethod          = null;
        private bool                            _testPassed          = false;

        #endregion


        #region Constructor

        [Variation("Begun", "AddAnimation", Priority=0)]
        [Variation("Begun", "BaseValue")]
        [Variation("Paused", "AddAnimation", Priority=0)]
        [Variation("Paused", "BaseValue")]
        [Variation("Seeked", "AddAnimation")]
        [Variation("Seeked", "BaseValue")]
        [Variation("Resumed", "AddAnimation")]
        [Variation("Resumed", "BaseValue")]
        [Variation("Repeated", "AddAnimation")]
        [Variation("Repeated", "BaseValue")]
        [Variation("Reversed", "AddAnimation")]
        [Variation("Reversed", "BaseValue")]
        [Variation("Changed", "AddAnimation", Priority=0)]
        [Variation("Changed", "BaseValue")]
        [Variation("Ended", "AddAnimation")]
        [Variation("Ended", "BaseValue")]


        /******************************************************************************
        * Function:          AnimMethods1Test Constructor
        ******************************************************************************/
        public AnimMethods1Test(string testValue1, string testValue2)
        {
            _animEvent = testValue1;
            _animMethod = testValue2;
            InitializeSteps += new TestStep(CheckInputParameters);
            RunSteps += new TestStep(CreateWindow);
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
            bool        arg1Found   = false;
            bool        arg2Found   = false;
            string      errMessage  = "";
            string[]    expList1    = { "Begun", "Paused", "Seeked", "Resumed", "Repeated", "Reversed", "Changed", "Ended" };
            string[]    expList2    = { "AddAnimation", "BaseValue", };

            arg1Found = AnimationUtilities.CheckInputString(_animEvent, expList1, ref errMessage);
            if (errMessage != "")
            {
                GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 1st Parameter: " + errMessage);
            }
            else
            {
                arg2Found = AnimationUtilities.CheckInputString(_animMethod, expList2, ref errMessage);
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
        * Function:          CreateWindow
        ******************************************************************************/
        /// <summary>
        /// Initialize: create a new Window and add content to it.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        TestResult CreateWindow()
        {
            GlobalLog.LogStatus("--- CreateWindow ---");

            Window.Width        = 350;
            Window.Height       = 350;
            Window.Title        = "Animation Methods Test";
            Window.Left         = 0;
            Window.Top          = 0;

            _body = new Canvas();
            _body.Width               = 350;
            _body.Height              = 350;
            _body.Background          = Brushes.Azure;

            _TB  = new TextBox();
            _body.Children.Add(_TB);
            _TB.Name                  = "textbox";
            _TB.Width                 = 100;
            _TB.Height                = 40;
            _TB.Background            = Brushes.BlueViolet;
            _TB.Text                  = "Avalon!";
            _TB.Foreground            = Brushes.LightGreen;
            _TB.FontSize              = 24;
            Canvas.SetTop  (_TB, _ASSIGNED_TOP);
            Canvas.SetLeft (_TB, 50d);

            Window.Content = _body;

            CreateExtraTimelines();
            CreateDoubleAnimation();
            
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

            //This Timeline is used for invoking Pause/Seek/Resume methods on the Animation.
            ParallelTimeline TL1 = new ParallelTimeline();
            TL1.BeginTime                 = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            TL1.Duration                  = new Duration(TimeSpan.FromMilliseconds(_DURATION_TIME_TL1));
            TL1.CurrentStateInvalidated  += new EventHandler(OnCurrentStateTL1);
            root.Children.Add(TL1);

            //This Timeline is used for verification after the animation is finished.
            ParallelTimeline TL2 = new ParallelTimeline();
            TL2.BeginTime                 = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            TL2.Duration                  = new Duration(TimeSpan.FromMilliseconds(_DURATION_TIME_TL2));
            TL2.CurrentStateInvalidated  += new EventHandler(OnCurrentStateTL2);
            root.Children.Add(TL2);

            _TLC = root.CreateClock();

            GlobalLog.LogStatus("--- Extra Timelines Created ---");
        }          

        /******************************************************************************
        * Function:          CreateDoubleAnimation
        ******************************************************************************/
        /// <summary>
        /// Add a DoubleAnimation to the TextBox.
        /// </summary>
        private void CreateDoubleAnimation()
        {
            _anim1 = new DoubleAnimation();
            _anim1.By                 = 100;
            _anim1.BeginTime          = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            _anim1.Duration           = new Duration(TimeSpan.FromMilliseconds(_DURATION_TIME));
            _anim1.RepeatBehavior     = new RepeatBehavior(2);
            _anim1.AutoReverse        = true;
            
            _anim1Clock = _anim1.CreateClock();
            _TB.ApplyAnimationClock(Canvas.TopProperty, _anim1Clock);

            AttachHandlers(_anim1Clock);

            GlobalLog.LogStatus("--- DoubleAnimation Created ---");
        }

        /******************************************************************************
        * Function:          AttachHandlers
        ******************************************************************************/
        /// <summary>
        /// Attaches event handlers to the animation timelineclock.
        /// </summary>
        private Clock AttachHandlers(Clock clock)
        {
            clock.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
            clock.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            clock.CurrentTimeInvalidated        += new EventHandler(OnCurrentTimeInvalidated);
            clock.CurrentTimeInvalidated        += new EventHandler(OnCurrentTimeInvalidatedRepeat);

            return clock;
        }
          
        /******************************************************************************
        * Function:          InvokeMethod
        ******************************************************************************/
        /// <summary>
        /// Calls the appropriate routine to invoke an animation-related method on the UIElement.
        /// It is called via one of the events triggered via Timing methods on a separate timeline.
        /// But it is not called for a "Seeked" scenario; Seek() fires all events.
        /// </summary>
        private void InvokeMethod()
        {
            GlobalLog.LogStatus("--- Invoke Method ---");
            switch (_animMethod)
            {
                case "AddAnimation":
                    AddAnim();
                    break;
                case "BaseValue":
                    _baseValue = GetBaseValue().ToString();
                    break;
                default:
                    break;
            }
        }
          
        /******************************************************************************
        * Function:          AddAnim
        ******************************************************************************/
        /// <summary>
        /// Adds an Animation to the UIElement and also gets an AnimationCollection Count.
        /// </summary>
        private void AddAnim()
        {   

            //Define a second Animation, but don't add it yet.
            _anim2 = new DoubleAnimation();
            _anim2.By                 = 1;
            _anim2.BeginTime          = TimeSpan.FromMilliseconds(0);
            _anim2.Duration           = new Duration(TimeSpan.FromMilliseconds(1000));
            
            _anim2Clock = _anim2.CreateClock();

            _hasAnimation = _TB.HasAnimatedProperties && _hasAnimation;  //Should be 1 animation from before.
            GlobalLog.LogStatus("--- HasAnimatedProperties check 1: " + _hasAnimation);
            
            _TB.ApplyAnimationClock(Canvas.TopProperty, null);
            _hasAnimation = !_TB.HasAnimatedProperties && _hasAnimation;  //Should be 0 animations.
            GlobalLog.LogStatus("--- HasAnimatedProperties check 2: " + _hasAnimation);

            _TB.ApplyAnimationClock(Canvas.LeftProperty, _anim2Clock);
            _hasAnimation = _TB.HasAnimatedProperties && _hasAnimation;  //Should be 1 animation.
            GlobalLog.LogStatus("--- HasAnimatedProperties check 3: " + _hasAnimation);

            _TB.ApplyAnimationClock(Canvas.LeftProperty, null);
            _hasAnimation = !_TB.HasAnimatedProperties && _hasAnimation;  //Should be 0 animations.
            GlobalLog.LogStatus("--- HasAnimatedProperties check 4: " + _hasAnimation);
            
            _TB.ApplyAnimationClock(Canvas.TopProperty, _anim2Clock);
            _hasAnimation = _TB.HasAnimatedProperties && _hasAnimation;  //Should be 1 animation.
            GlobalLog.LogStatus("--- HasAnimatedProperties check 5: " + _hasAnimation);
        }     

        /******************************************************************************
        * Function:          GetBaseValue
        ******************************************************************************/
        /// <summary>
        /// Returns the local animation.
        /// </summary>
        private Object GetBaseValue()
        {
            return _TB.GetAnimationBaseValue(Canvas.TopProperty);
        }
          
        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        /// <summary>
        /// The OnCurrentStateInvalidated event handler for the DoubleAnimation.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            if (!_seekInvoked)
            {
                if (((Clock)sender).CurrentState == ClockState.Active)
                {
                    _beg += 1;
                    GlobalLog.LogStatus("---Begun---");

                    if (_animEvent == "Begun" && _beg == 1)
                    {
                         InvokeMethod();
                    }
                }
                else
                {
                    _end += 1;          
                    GlobalLog.LogStatus("---Ended---");

                    if (_animEvent == "Ended" && _end == 1)
                    {
                        InvokeMethod();
                    }
                }
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
            //GlobalLog.LogStatus("---OnCurrentGlobalSpeedInvalidated---------");
            
            //There is no simple way to know if Seek() was invoked, so employing a flag to do so.
            if (_seekInvoked)
            {
                InvokeMethod();
                _seekInvoked = false;
                return;
            }

            double globalSpeed  = GetCurrentGlobalSpeed(((Clock)sender));

            //Using the Clock's CurrentGlobalSpeed property to determine if the Timeline reverses.
            //CurrentGlobalSpeed changes to -1 when the clock reverses.
            if (((Clock)sender).CurrentGlobalSpeed != null)
            {
                if (globalSpeed < 0)
                {
                    //Avoid signalling repeated "Reversed" during Acceleration/Deceleration, by
                    //counting 'Reversed' only when the sign of the CurrentGlobalSpeed property changes.
                    if (Math.Sign(globalSpeed) != Math.Sign(_lastSpeed))
                    {
                        _rev += 1;          
                        GlobalLog.LogStatus("---Reversed---");

                        if (_animEvent == "Reversed" && _rev == 1)
                        {
                            InvokeMethod();
                        }
                    }
                }
                _lastSpeed = globalSpeed;
            }

            //Check if the animation is paused and/or resumed.
                if (((Clock)sender).IsPaused)
                {
                    _pau += 1;
                    GlobalLog.LogStatus("---Paused---"); 

                    if (_animEvent == "Paused" && _pau == 1)
                    {
                        InvokeMethod();
                    }
                }
                _lastPaused = ((Clock)sender).IsPaused;
            
            //Cannot use IsPaused to determine if the animation resumed, because Pause()
            //and Resume() were invoked together, so that IsPaused is never true.
            if (_resumedInvoked)
            {
                _res += 1;
                GlobalLog.LogStatus("---Resumed---");               

                if (_animEvent == "Resumed" && _res == 1)
                {
                    InvokeMethod();
                    _resumedInvoked = false;
                }
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
            if (!_seekInvoked)
            {
                _cha += 1;

                if (_animEvent == "Changed" && _cha == 2)
                {
                    GlobalLog.LogStatus("---Changed--- " + _cha);
                    InvokeMethod();
                }
            }
        }

        /*****************************************************************************
            * Function:     OnCurrentTimeInvalidatedRepeat
            *****************************************************************************/
        /// <summary>
        /// </summary>
        /// <param>sender: the TimelineClock associated with the event</param>
        /// <returns></returns>
        public void OnCurrentTimeInvalidatedRepeat(object sender, EventArgs e)
        {
            if (!_seekInvoked)
            {
                //Using the Clock's CurrentIteration property to determine if the Timeline repeats.
                //CurrentIteration changes to 1 when the Timeline begins, and -1 when it ends.
                //It also increments when the Timeline repeats.

                if ((((Clock)sender).CurrentIteration > _iterationCount))
                {
                    if (_iterationCount > 0)
                    {
                        _rep += 1;          
                        GlobalLog.LogStatus("---Repeated---");

                        if (_animEvent == "Repeated" && _rep == 1)
                        {
                            InvokeMethod();
                        }
                    }
                    _iterationCount++;
                }
            }
        }

        public double GetCurrentGlobalSpeed(Clock clock)
        {
            if (clock.CurrentGlobalSpeed == null)
            {
                //Handle a null CurrentGlobalSpeed.
                return 99999;
            }
            else
            {
                return (double)clock.CurrentGlobalSpeed;
            }
        }
          
        /******************************************************************************
        * Function:          OnCurrentStateTL1
        ******************************************************************************/
        /// <summary>
        /// The OnCurrentStateTL1 event handler for the extra Timeline, TL1.
        /// It is being used to invoke certain methods DURATION_TIME_TL1 ms after
        /// the anim begins, in order to enter into the Paused, Seeked, and Resumed states.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateTL1(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != ClockState.Active)
            {
                GlobalLog.LogStatus("---OnCurrentStateTL1---" + _animEvent);

                switch (_animEvent)
                {
                    case "Paused":
                        _anim1Clock.Controller.Pause();
                        break;                                   
                    case "Seeked":
                        //Invoking Seek() will fire all events.
                        _seekInvoked = true;
                        _anim1Clock.Controller.Seek(TimeSpan.FromMilliseconds(_SEEK_TIME),TimeSeekOrigin.BeginTime);
                        break;                                   
                    case "Resumed":
                        //The CurrentGlobalSpeedInvalidated event will only fire once, which means
                        //only the "Resumed" event will be recognized.
                        _anim1Clock.Controller.Pause();
                        _resumedInvoked = true;
                        _anim1Clock.Controller.Resume();
                        break;                                   
                    default:
                        break;
                }
            }
        }

        /******************************************************************************
        * Function:          CompareBaseValue
        ******************************************************************************/
        private bool CompareBaseValue()
        {
            GlobalLog.LogEvidence("\nActual BaseValue: " + _baseValue + "\nExpected BaseValue: " + _ASSIGNED_TOP.ToString());
            return (_baseValue == _ASSIGNED_TOP.ToString());
        }
          
        /******************************************************************************
        * Function:          OnCurrentStateTL2
        ******************************************************************************/
        /// <summary>
        /// The CurrentStateInvalidated event handler for the verification Timeline,
        /// used to verify that events fired correctly and pass or fail the test case.
        /// </summary>
        /// <returns></returns>
        private void OnCurrentStateTL2(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != ClockState.Active)
            {
                GlobalLog.LogStatus("---OnCurrentStateTL2---");

                //NOTE:  the Paused and Resumed events will fire at least once (due to ClockController.Pause(), Seek(), and
                //ClockController.Resume() methods being invoked via TL1), except when certain methods are executed in the Animation's
                //BeginIn event.  Also, because AutoReverse=True and RepeatCount=2 for the Animation, the Reversed and
                //Repeated events will fire at least once, unless otherwise prevented.

                bool b1   = false; //Event firing.
                bool b2   = false; 

                if (_animEvent == "Begun")
                {
                    switch (_animMethod)
                    {
                        case "AddAnimation":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = _hasAnimation;
                            break;
                        case "BaseValue":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = CompareBaseValue();
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument. (1)");
                            break;
                    }
                    _testPassed = (b1 && b2);
                }
                else if (_animEvent == "Paused")
                {
                    //This event is invoked via TL1, at a time of DURATION_TIME_TL1 after the Animation has started.
                    switch (_animMethod)
                    {
                        case "AddAnimation":
                            b1 = (_beg==1 && _pau==1 && _res==0 && _rep==0 && _rev==0 && _cha>0 && _end==0);
                            _expectedEvents = "beg=1\npau=1\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = _hasAnimation;
                            break;
                        case "BaseValue":
                            b1 = (_beg==1 && _pau==1 && _res==0 && _rep==0 && _rev==0 && _cha>0 && _end==0);
                            _expectedEvents = "beg=1\npau=1\nres=0\nrep=0\nrev=0\ncha>0\nend=0";
                            b2 = CompareBaseValue();
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument. (2)");
                            break;
                    }
                    _testPassed = (b1 && b2);
                }
                else if (_animEvent == "Seeked")
                {
                    //This event is invoked via TL1, at a time of DURATION_TIME_TL1 after the Animation has started.
                    switch (_animMethod)
                    {
                        case "AddAnimation":
                            b1 = (_beg==2 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=2\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = _hasAnimation;
                            break;
                        case "BaseValue":
                            b1 = (_beg==2 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=2\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = CompareBaseValue();
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument. (3)");
                            break;
                    }
                    _testPassed = (b1 && b2);
                }
                else if (_animEvent == "Resumed")
                {
                    //This event is invoked via TL1, at a time of DURATION_TIME_TL1 after the Animation has started.
                    //It is preceded by a pause, to ensure it will fire.
                    switch (_animMethod)
                    {
                        case "AddAnimation":
                            b1 = (_beg==1 && _pau==0 && _res==1 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=1\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = _hasAnimation;
                            break;
                        case "BaseValue":
                            b1 = (_beg==1 && _pau==0 && _res==1 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=1\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = CompareBaseValue();
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument. (4)");
                            break;
                    }
                    _testPassed = (b1 && b2);
                }
                else if (_animEvent == "Repeated")
                {
                    switch (_animMethod)
                    {
                        case "AddAnimation":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = _hasAnimation;
                            break;
                        case "BaseValue":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = CompareBaseValue();
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument. (5)");
                            break;
                    }
                    _testPassed = (b1 && b2);
                }
                else if (_animEvent == "Reversed")
                {
                    switch (_animMethod)
                    {
                        case "AddAnimation":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = _hasAnimation;
                            break;
                        case "BaseValue":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = CompareBaseValue();
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument. (6)");
                            break;
                    }
                    _testPassed = (b1 && b2);
                }
                else if (_animEvent == "Changed")
                {
                    switch (_animMethod)
                    {
                        case "AddAnimation":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = _hasAnimation;
                            break;
                        case "BaseValue":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = CompareBaseValue();
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument. (7)");
                            break;
                    }
                    _testPassed = (b1 && b2);
                }
                else if (_animEvent == "Ended")
                {
                    switch (_animMethod)
                    {
                        case "AddAnimation":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = _hasAnimation;
                            break;
                        case "BaseValue":
                            b1 = (_beg==1 && _pau==0 && _res==0 && _rep==1 && _rev==2 && _cha>0 && _end==1);
                            _expectedEvents = "beg=1\npau=0\nres=0\nrep=1\nrev=2\ncha>0\nend=1";
                            b2 = CompareBaseValue();
                            break;
                        default:
                            HandleError("ERROR!!! OnCurrentStateTL2: Unexpected failure to match argument. (1)");
                            break;
                    }
                    _testPassed = (b1 && b2);
                }

                GlobalLog.LogEvidence("Actual Events:");
                GlobalLog.LogEvidence("beg=" + _beg);
                GlobalLog.LogEvidence("pau=" + _pau);
                GlobalLog.LogEvidence("res=" + _res);
                GlobalLog.LogEvidence("rep=" + _rep);
                GlobalLog.LogEvidence("rev=" + _rev);
                GlobalLog.LogEvidence("cha=" + _cha);
                GlobalLog.LogEvidence("end=" + _end);
                GlobalLog.LogEvidence("\nExpected Events:\n" + _expectedEvents);

                GlobalLog.LogEvidence("----------b1 result: " + b1 );
                GlobalLog.LogEvidence("----------b2 result: " + b2 );

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
