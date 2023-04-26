// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Storyboard Parameterless Test *****************
*     Description:
*          Tests animation when using Storyboard.SetTarget(dependencyObject)/Begin()
*     Pass Conditions:
*       (a) GetCurrentValue from the AnimationClock
*       (b) GetValue from the animated DO
*       (c) CurrentStateInvalidated fired correctly
*       (d) various Storyboard properties
*     Note:
*          For details of the new feature being introduced in .Net3.5-SP1, refer to the test spec
*          located at:  Test\Animation\Specifications\Storyboard Parameterless Methods Test Spec.docx
*
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dev Owner:          Microsoft
*     Dependencies:       TestRuntime.dll
*     Support Files:
* *******************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
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
    /// Verifying the following for SetTarget(dependencyObject)/Begin() scenarios:
    /// (a) GetCurrentValue from the AnimationClock
    /// (b) GetValue from the animated DO
    /// (c) CurrentStateInvalidated fired correctly
    /// (d) various Storyboard properties
    /// </description>
    /// </summary>
    [Test(1, "Storyboards.LowLevelScenarios", "ParameterlessTest")]
    public class ParameterlessTest : WindowTest
    {
        #region Test case members

        private string                      _targetObject    = null;
        private string                      _targetName      = "storyboardTarget";
        private Storyboard                  _storyboard;
        private int                         _actBegunCount   = 0;
        private int                         _actEndedCount   = 0;
        private double                      _animateTo       = 18d;
        private double                      _actGetCurrentValue;
        private PropertyPath                _expPath;
        private DependencyProperty          _dp;
        private DependencyObject            _dobj;
        private double                      _BEGINTIME       = 500d;
        private double                      _DURATION        = 1000d;
        private int                         _REPETITIONS     = 2;
        
        #endregion


        #region Constructor

        [Variation("FrameworkElement")]
        [Variation("FrameworkContentElement")]
        
        /******************************************************************************
        * Function:          ParameterlessTest Constructor
        ******************************************************************************/
        public ParameterlessTest(string testValue1)
        {
            _targetObject    = testValue1;
            RunSteps += new TestStep(CreateTree);
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
        /// <returns>Returns TestResult.Pass</returns>
        TestResult CreateTree()
        {
            TextBox textbox1 = null;
            Paragraph paragraph1 = null;

            Window.Width                = 900;
            Window.Height               = 800;
            Window.Title                = "Storyboard Methods Testing";
            Window.Left                 = 0;
            Window.Top                  = 0;
            Window.Topmost              = true;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            NameScope.SetNameScope(Window, new NameScope());

            Canvas body = new Canvas();
            body.Width               = 900;
            body.Height              = 800;
            body.Background          = Brushes.BurlyWood;

            if (_targetObject == "FrameworkElement")
            {
                textbox1  = new TextBox();
                textbox1.Width               = 150d;
                textbox1.Height              = 50d;
                textbox1.Background          = Brushes.Lavender;
                textbox1.FontSize            = 24d;
                textbox1.Text                = "WPF!";
                textbox1.Name                = _targetName;
                
                Canvas.SetTop  (textbox1, 50d);
                Canvas.SetLeft (textbox1, 50d);
                body.Children.Add(textbox1);
                Window.RegisterName(textbox1.Name, textbox1);
                
                _dp = TextBox.FontSizeProperty;
                _dobj = textbox1;
            }
            else if (_targetObject == "FrameworkContentElement")
            {
                FlowDocumentReader flowDocReader1 = new FlowDocumentReader();
                flowDocReader1.Height        = 200d;
                flowDocReader1.Width         = 300d;
                flowDocReader1.Background    = Brushes.SpringGreen;
                Canvas.SetTop  (flowDocReader1, 50d);
                Canvas.SetLeft (flowDocReader1, 50d);

                body.Children.Add(flowDocReader1);

                FlowDocument flowDocument1 = new FlowDocument();
                flowDocument1.Background        = Brushes.DodgerBlue;
                flowDocReader1.Document         = flowDocument1;

                paragraph1 = new Paragraph(new Run("Windows Presentation Foundation"));
                paragraph1.FontSize = 24d;
                paragraph1.BorderThickness = new Thickness(5d);
                paragraph1.Name = _targetName;
                flowDocument1.Blocks.Add(paragraph1);
                Window.RegisterName(paragraph1.Name, paragraph1);
                
                _dp = Paragraph.FontSizeProperty;
                _dobj = paragraph1;
            }

            //------------------- CREATE ANIMATION------------------------------
            DoubleAnimation animDouble = new DoubleAnimation();
            animDouble.From             = 24d;
            animDouble.To               = _animateTo;
            animDouble.BeginTime        = TimeSpan.FromMilliseconds(_BEGINTIME);
            animDouble.Duration         = new Duration(TimeSpan.FromMilliseconds(_DURATION));
            animDouble.FillBehavior     = FillBehavior.HoldEnd;
            animDouble.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);

            //------------------- CREATE STORYBOARD-----------------------------
            GlobalLog.LogStatus("----Create Storyboard----");
            _storyboard = new Storyboard();
            _storyboard.Name = "story";
            _storyboard.RepeatBehavior = new RepeatBehavior(_REPETITIONS);
            _storyboard.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidatedStory);

            _storyboard.Children.Add(animDouble);
            _expPath = new PropertyPath( "(0)", _dp );
            Storyboard.SetTargetProperty(_storyboard, _expPath);
            Storyboard.SetTarget(_storyboard, _dobj); //SetTarget takes the to-be-animated DO.

            Window.Content = body;

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked when the .xaml page renders.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---OnContentRendered---Begin the Storyboard");

            _storyboard.Begin();
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---OnCurrentStateInvalidated--- CurrentState: " + ((Clock)sender).CurrentState);
            
            //Retrieve the actual value of AnimationClock for later verification.
            if (((Clock)sender).CurrentState == ClockState.Filling)
            {
                AnimationClock DAC = (AnimationClock)sender;
                _actGetCurrentValue = (double)DAC.GetCurrentValue(0d, 0d);
            }
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidatedStory
           ******************************************************************************/
        private void OnCurrentStateInvalidatedStory(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---OnCurrentStateInvalidatedStory--- CurrentState: " + ((Clock)sender).CurrentState);
            
            if (((Clock)sender).CurrentState == ClockState.Active)
            {
                _actBegunCount += 1;
            }
            else if (((Clock)sender).CurrentState == ClockState.Filling)
            {
                _actEndedCount += 1;

                Signal("TestFinished", TestResult.Ignore);
            }
        }

        /******************************************************************************
        * Function:          VerifyStoryboardAPIs
        ******************************************************************************/
        /// <summary>
        /// Verifies Storyboard properties.
        /// </summary>
        /// <returns> Boolean: pass/fail</returns>
        private bool VerifyStoryboardAPIs()
        {
            GlobalLog.LogStatus("VerifyStoryboardAPIs");

            bool storyCorrect = true;

            if (_storyboard.GetCurrentState() != ClockState.Filling)
                storyCorrect = false;

            if (_storyboard.GetCurrentGlobalSpeed() != 0)
                storyCorrect = false;

            if (_storyboard.GetCurrentIteration() != _REPETITIONS)
                storyCorrect = false;

            if (_storyboard.GetCurrentProgress()!= 1)
                storyCorrect = false;

            if (_storyboard.GetIsPaused()!= false)
                storyCorrect = false;

            if (Storyboard.GetTargetName(_storyboard) != null) //SetTargetName was -not- called, so expecting null.
                storyCorrect = false;

            if ( Storyboard.GetTarget(_storyboard) != _dobj)
                storyCorrect = false;

            double expTime = _BEGINTIME + _DURATION; 
            if (((TimeSpan)_storyboard.GetCurrentTime()).TotalMilliseconds != expTime)
                storyCorrect = false;

            PropertyPath actPath = Storyboard.GetTargetProperty(_storyboard);
            DependencyProperty actDP = (DependencyProperty)actPath.PathParameters[0];
            string actName = (string)actDP.Name;

            DependencyProperty expDP = (DependencyProperty)_expPath.PathParameters[0];
            string expName = (string)expDP.Name;
            if (actName != expName)
                storyCorrect = false;

            GlobalLog.LogEvidence(" CurrentState   Actual: " + _storyboard.GetCurrentState()        + "  Expected: Filling");
            GlobalLog.LogEvidence(" GlobalSpeed    Actual: " + _storyboard.GetCurrentGlobalSpeed()  + "  Expected: 0");
            GlobalLog.LogEvidence(" Iteration:     Actual: " + _storyboard.GetCurrentIteration()    + "  Expected: " + _REPETITIONS);
            GlobalLog.LogEvidence(" Progress:      Actual: " + _storyboard.GetCurrentProgress()     + "  Expected: 1");
            GlobalLog.LogEvidence(" Time:          Actual: " + ((TimeSpan)_storyboard.GetCurrentTime()).TotalMilliseconds + "  Expected: " + expTime);
            GlobalLog.LogEvidence(" Target         Actual: " + Storyboard.GetTarget(_storyboard)    + "  Expected: " + _dobj);
            GlobalLog.LogEvidence(" TargetName     Actual: " + Storyboard.GetTargetName(_storyboard)+ "  Expected: "); //Expect null.
            GlobalLog.LogEvidence(" TargetProperty Actual: " + actName                             + "  Expected: " + expName);

            return storyCorrect;
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult object, indicating Pass or Fail</returns>
        TestResult Verify()
        {
            double expGetCurrentValue   = _animateTo;
            int expEndedCount           = 1;
            int expBegunCount           = 1;
            double expGetValue          = _animateTo;

            bool getCurrentValueCorrect = false;
            bool eventCountCorrect      = false;
            bool getValueCorrect        = false;
            bool storyPropsCorrect      = false;

            TestResult result = WaitForSignal("TestFinished");

            // (1) Verify GetCurrentValue via the Animation's OnCurrentStateInvalidated event.
            
            if (_actGetCurrentValue == expGetCurrentValue)
            {
                getCurrentValueCorrect = true;
            }

            // (2) Verify the Storyboard's CurrentStateInvalidated event.
            if (_actEndedCount == expEndedCount && _actBegunCount == expBegunCount)
            {
                eventCountCorrect = true;
            }

            // (3) Verify the value of the animated dp;
            double actGetValue = (double)_dobj.GetValue(_dp);
            if (actGetValue == expGetValue)
            {
                getValueCorrect = true;
            }

            GlobalLog.LogEvidence("begunCount----Actual:       " + _actBegunCount);
            GlobalLog.LogEvidence("begunCount----Expected:     " + expBegunCount);
            GlobalLog.LogEvidence("endedCount----Actual:       " + _actEndedCount);
            GlobalLog.LogEvidence("endedCount----Actual:       " + expEndedCount);
            GlobalLog.LogEvidence("GetCurrentValue (Actual):   " + _actGetCurrentValue);
            GlobalLog.LogEvidence("GetCurrentValue (Expected): " + expGetCurrentValue);
            GlobalLog.LogEvidence("GetValue (Actual):          " + actGetValue);
            GlobalLog.LogEvidence("GetValue (Expected):        " + expGetValue);

            // (4) Verify Storyboard properties.
            storyPropsCorrect = VerifyStoryboardAPIs();

            GlobalLog.LogEvidence("----------- FINAL RESULTS -----------");
            GlobalLog.LogEvidence("CurrentValue correct:  " + getCurrentValueCorrect);
            GlobalLog.LogEvidence("EventCount   correct:  " + eventCountCorrect);
            GlobalLog.LogEvidence("Value        correct:  " + getValueCorrect);
            GlobalLog.LogEvidence("Storyboard   correct:  " + storyPropsCorrect);

            if (getCurrentValueCorrect && eventCountCorrect && getValueCorrect && storyPropsCorrect)
            {
                result = TestResult.Pass;
            }
            else
            {
                result = TestResult.Fail;
            }

            return result;
        }

        #endregion
    }
}
