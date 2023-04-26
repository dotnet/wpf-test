// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    public class ContentElementB : AvalonTest
    {
        private Window                      _navWin;
        private DispatcherTimer             _aTimer                  = null;  //Used for Timing method verification.
        private int                         _dispatcherTickCount     = 0;
        private AnimationClock              _clock1                  = null;
        private Paragraph                   _animElement             = null;
        private Storyboard                  _storyboard              = null;
        private DoubleAnimation             _doubleAnim              = null;
        private string                      _animationRequest        = "";
        private ClockState                  _actState;
        private ClockState                  _actCompleted;
        private Nullable<double>            _actSpeed                = null;
        private DependencyProperty          _dp                      = Paragraph.TextIndentProperty;
        private double                      _BY_VALUE                = 50d;
        private double                      _INITIAL_VALUE           = 0d;
        private TimeSpan                    _TIMER_INTERVAL          = new TimeSpan(0,0,0,0,1000);
        private bool                        _testPassed              = false;
        

        public ContentElementB()
        {
        }
        
        /******************************************************************************
           * Function:          StartTest
           ******************************************************************************/
        /// <summary>
        /// StartTest: Start a Dispatcher Timer to handle the animation.
        /// Objects are obtained from the Markup that has loaded.
        ///     animType:       the type of Animation carried out
        /// </summary>
        /// <returns></returns>
        public bool StartTest(string animType, NavigationWindow nWin)
        {
            _animationRequest    = animType;
            _navWin              = nWin;
            
            GlobalLog.LogStatus("-------------StartTest-------------");
                    
            NameScope.SetNameScope(_navWin, new NameScope());

            //Retrieve to-be-animated element from the Markup page
            _animElement = (Paragraph)LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWin.Content,"Animate");
            
            GlobalLog.LogStatus("*************Test Parameters*************");
            GlobalLog.LogStatus("AnimationRequest:  " + _animationRequest);
            GlobalLog.LogStatus("*****************************************");
            
            //Start the test case.
            StartAnimation();

            //Use a DispatcherTimer to initiate an Animation.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = _TIMER_INTERVAL;
            _aTimer.Start();

            WaitForSignal("TestFinished");
            
            return _testPassed;  //Determined by verification initiated via the DispatcherTimer.
        }
        
        /******************************************************************************
           * Function:          OnTick
           ******************************************************************************/
        /// <summary>
        /// OnTick: starts verification of the Animation.
        /// </summary>
        private void OnTick(object sender, EventArgs e)          
        {
            _dispatcherTickCount++;
            GlobalLog.LogStatus("Tick #" + _dispatcherTickCount);
            
            if (_dispatcherTickCount == 1)
            {
                if (_animationRequest == "AnimationClockHandoffStopAtEnd")
                {
                    _clock1.Controller.Stop();
                }
                else if(_animationRequest == "AnimationClockHandoffRemoveAtEnd")
                {
                    _clock1.Controller.Remove();
                }
            }
            else if (_dispatcherTickCount == 2)
            {
                _aTimer.Stop();
                _testPassed = FinishTest();
                
                Signal("TestFinished", TestResult.Pass);
            }
        }
        /******************************************************************************
           * Function:          CreateAnimation
           ******************************************************************************/
        /// <summary>
        /// CreateAnimation: create and return a DoubleAnimation
        /// </summary>
        /// <returns></returns>
        private DoubleAnimation CreateAnimation()          
        {
            GlobalLog.LogStatus("-------------CreateAnimation-------------");

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.By             = _BY_VALUE;
            doubleAnimation.BeginTime      = TimeSpan.FromMilliseconds(0);
            doubleAnimation.Duration       = new Duration(TimeSpan.FromMilliseconds(750));
            doubleAnimation.CurrentStateInvalidated         += new EventHandler(OnCurrentStateInvalidated);
            doubleAnimation.CurrentGlobalSpeedInvalidated   += new EventHandler(OnCurrentGlobalSpeedInvalidated);
            doubleAnimation.Completed                       += new EventHandler(OnCompleted);

            return doubleAnimation;
        }

        /******************************************************************************
        * Function:          CreateStoryboard
        ******************************************************************************/
        /// <summary>
        /// Creates Storyboard, ready to be animated.
        /// </summary>
        /// <returns> A Storyboard </returns>
        private Storyboard CreateStoryboard(DoubleAnimation animDouble)
        {
            GlobalLog.LogStatus("-------------CreateStoryboard-------------");
            
            Storyboard story = new Storyboard();
            story.Name                     = "storyboard";
            story.Children.Add(animDouble);

            PropertyPath path  = new PropertyPath("(0)", _dp);
            Storyboard.SetTargetProperty(story, path);
            Storyboard.SetTargetName(story, _animElement.Name);

            return story;
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Used to validate event firing.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {               
            GlobalLog.LogStatus("CurrentStateInvalidated:  " + ((Clock)sender).CurrentState);
            _actState = ((Clock)sender).CurrentState;
        }

        /******************************************************************************
           * Function:          OnCurrentGlobalSpeedInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentGlobalSpeedInvalidated: Used to validate event firing.
        /// </summary>
        private void OnCurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {
            _actSpeed = (Nullable<double>)((Clock)sender).CurrentGlobalSpeed;
            
            GlobalLog.LogStatus("CurrentGlobalSpeedInvalidated:  " + _actSpeed);
        }

        /******************************************************************************
           * Function:          OnCompleted
           ******************************************************************************/
        /// <summary>
        /// OnCompleted: Used to validate event firing.
        /// </summary>
        private void OnCompleted(object sender, EventArgs e)
        {               
            GlobalLog.LogStatus("Completed:  " + ((Clock)sender).CurrentState);
            _actCompleted = ((Clock)sender).CurrentState;
        }

        /******************************************************************************
           * Function:          StartAnimation
           ******************************************************************************/
        /// <summary>
        /// StartAnimation: 
        ///     animationRequest: specifies how the animation is to be started
        /// </summary>
        /// <returns></returns>
        public void StartAnimation()          
        {
            GlobalLog.LogStatus("-------------StartAnimation-------------");

            _doubleAnim = CreateAnimation();
            
            switch (_animationRequest)
            {
                case "BeginAnimation" :
                    _animElement.BeginAnimation(_dp, _doubleAnim);
                    break;

                case "BeginAnimationHandoff" :
                    _animElement.BeginAnimation(_dp, _doubleAnim, HandoffBehavior.Compose);
                    break;

                case "AnimationClock" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1);
                    break;

                case "AnimationClockPauseResume" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1);
                    _clock1.Controller.Pause();
                    _clock1.Controller.Resume();
                    break;

                case "AnimationClockSeek" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1);
                    _clock1.Controller.Seek(TimeSpan.FromMilliseconds(2000),TimeSeekOrigin.BeginTime);
                    break;

                case "AnimationClockSeekAligned" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1);
                    _clock1.Controller.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(2000),TimeSeekOrigin.BeginTime);
                    break;

                case "AnimationClockStop" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1);
                    _clock1.Controller.Stop();
                    break;

                case "AnimationClockSkipToFill" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1);
                    _clock1.Controller.SkipToFill();
                    break;

                case "AnimationClockRemove" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1);
                    _clock1.Controller.Remove();
                    break;

                case "AnimationClockHandoff" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1, HandoffBehavior.SnapshotAndReplace);
                    break;

                case "AnimationClockHandoffPause" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1, HandoffBehavior.SnapshotAndReplace);
                    _clock1.Controller.Pause();
                    break;

                case "AnimationClockHandoffStopAtEnd" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1, HandoffBehavior.SnapshotAndReplace);
                    break;

                case "AnimationClockHandoffRemoveAtEnd" :
                    _clock1 = _doubleAnim.CreateClock();
                    _animElement.ApplyAnimationClock(_dp, _clock1, HandoffBehavior.SnapshotAndReplace);
                    break;

                case "BeginStoryboard" :
                    _storyboard = CreateStoryboard(_doubleAnim);
                    _animElement.BeginStoryboard(_storyboard);
                    break;

                case "BeginStoryboardHandoff" :
                    _storyboard = CreateStoryboard(_doubleAnim);
                    _animElement.BeginStoryboard(_storyboard, HandoffBehavior.Compose);
                    break;

                case "BeginStoryboardHandoffControllable" :
                    _storyboard = CreateStoryboard(_doubleAnim);
                    _animElement.BeginStoryboard(_storyboard, HandoffBehavior.Compose, true);
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR!! StartAnimation: The Animation requested was not found. \n");
                    Application.Current.Shutdown();
                    break;
            }
        }
        
        /******************************************************************************
           * Function:          FinishTest
           ******************************************************************************/
        /// <summary>
        /// FinishTest: pass or fail the test case
        /// </summary>
        /// <returns>A boolean, indicating pass or fail</returns>
        private bool FinishTest()          
        {
        
            // (1) Verify the value of the animated property after the animation has finished.
            double actValue = (double)_animElement.GetValue(_dp);
            double expValue;
            if (_animationRequest == "AnimationClockPause"
            ||  _animationRequest == "AnimationClockStop"
            ||  _animationRequest == "AnimationClockRemove"
            ||  _animationRequest == "AnimationClockHandoffPause"
            ||  _animationRequest == "AnimationClockHandoffStopAtEnd"
            ||  _animationRequest == "AnimationClockHandoffRemoveAtEnd")
            {
                expValue = _INITIAL_VALUE;
            }
            else
            {
                expValue = _BY_VALUE;
            }
            bool valueCorrect = (actValue == expValue);


            // (2) Verify GetAnimationBaseValue().
            double actBase = (double)_animElement.GetAnimationBaseValue(_dp);
            double expBase = _INITIAL_VALUE;
            bool baseCorrect = (actBase == expBase);


            // (3) Verify HasAnimatedProperties.
            bool actHas = _animElement.HasAnimatedProperties;
            bool expHas;
            if (_animationRequest == "AnimationClockHandoffRemoveAtEnd"
            ||  _animationRequest == "AnimationClockRemove")
            {
                expHas = false;
            }
            else
            {
                expHas = true;
            }
            bool hasCorrect = (actHas == expHas);


            // (4) Verify CurrentState via the last CurrentStateInvalidated event firing.
            // NOTE: CurrentStateInvalidated never fires when immediately invoking Stop, so
            // the default ClockState "Active" is expected.
            ClockState expState;
            if (_animationRequest == "AnimationClockPause"
            ||  _animationRequest == "AnimationClockHandoffPause"
            ||  _animationRequest == "AnimationClockStop"
            ||  _animationRequest == "AnimationClockRemove")
            {
                expState = ClockState.Active;
            }
            else if (_animationRequest == "AnimationClockHandoffStopAtEnd"
                 ||  _animationRequest == "AnimationClockHandoffRemoveAtEnd")
            {
                expState = ClockState.Stopped;
            }
            else
            {
                expState = ClockState.Filling;
            }
            bool stateCorrect = (_actState == expState);


            // (5) Verify CurrentGlobalSpeed via the last CurrentGlobalSpeedInvalidated event firing.
            Nullable<double> expSpeed;
            if (_animationRequest == "AnimationClockHandoffStopAtEnd"
            ||  _animationRequest == "AnimationClockStop"
            ||  _animationRequest == "AnimationClockRemove"
            ||  _animationRequest == "AnimationClockHandoffRemoveAtEnd")
            {
                expSpeed = null;
            }
            else
            {
                expSpeed = 0d;
            }
            bool speedCorrect = (_actSpeed == expSpeed);


            // (6) Verify CurrentState via the Completed event firing.
            // NOTE: when Stop, Pause or SeekAlignedToLastTick are invoked, Completed never fires, 
            // so the default ClockState value of "Active" is used. The same is true when the
            // AnimationClock is immediately removed.
            ClockState expCompleted;
            if (_animationRequest == "AnimationClockHandoffPause"
            ||  _animationRequest == "AnimationClockStop"
            ||  _animationRequest == "AnimationClockRemove")
            {
                expCompleted = ClockState.Active;
            }
            else if(_animationRequest == "AnimationClockHandoffRemoveAtEnd")
            {
                expCompleted = ClockState.Stopped;
            }
            else
            {
                expCompleted = ClockState.Filling;
            }
            
            bool completedCorrect = (_actCompleted == expCompleted);

            GlobalLog.LogEvidence("---------------------RESULTS---------------------");
            GlobalLog.LogEvidence("GetValue               Exp: " + expValue     + "      /Act: " + actValue);
            GlobalLog.LogEvidence("GetAnimationBaseValue  Exp: " + expBase      + "       /Act: " + actBase);
            GlobalLog.LogEvidence("HasAnimatedProperties  Exp: " + expHas       + "    /Act: " + actHas);
            GlobalLog.LogEvidence("CurrentState           Exp: " + expState     + " /Act: " + _actState);
            GlobalLog.LogEvidence("CurrentGlobalSpeed     Exp: " + expSpeed     + "       /Act: " + _actSpeed);
            GlobalLog.LogEvidence("CurrentState-Completed Exp: " + expCompleted + " /Act: " + _actCompleted);
            GlobalLog.LogEvidence("-------------------------------------------------");

            GlobalLog.LogEvidence("--Test 1: GetValue                -- " + valueCorrect);
            GlobalLog.LogEvidence("--Test 2: GetAnimationBaseValue   -- " + baseCorrect);
            GlobalLog.LogEvidence("--Test 3: HasAnimatedProperties   -- " + hasCorrect);
            GlobalLog.LogEvidence("--Test 4: CurrentState            -- " + stateCorrect);
            GlobalLog.LogEvidence("--Test 5: CurrentGlobalSpeed      -- " + speedCorrect);
            GlobalLog.LogEvidence("--Test 6: CurrentState-Completed  -- " + completedCorrect);
            
            if (valueCorrect && baseCorrect && hasCorrect && stateCorrect && speedCorrect && completedCorrect)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
