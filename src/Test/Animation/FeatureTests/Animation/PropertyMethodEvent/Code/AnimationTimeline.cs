// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Animation Unit Test *****************
*     Pass Conditions:
*          Each property/method for AnimationTimeline returns the expected value.
*
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing

*     Dependencies:       TestRuntime.dll, AnimationFramework.dll
*     Support Files:          
**********************************************************/
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
    /// <area>Animation.PropertyMethodEvent.AnimationTimeline</area>
    /// <priority>0</priority>
    /// <description>
    /// Verify properties and methods on an AnimationTimeline object.
    /// </description>
    /// </summary>
    [Test(0, "Animation.PropertyMethodEvent.AnimationTimeline", "AnimationTimelineTest")]
    public class AnimationTimelineTest : WindowTest
    {
        #region Test case members

        private AnimationTimeline       _AT;
        private AnimationTimeline       _AT2;
        private AnimationClock          _clock           = null;
        private SolidColorBrush         _scBrush1;
        private string                  _resultInfo      = "[";
        private bool                    _testPassed      = true;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          AnimationOutside1Test Constructor
        ******************************************************************************/
        public AnimationTimelineTest()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTest);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
           * Function:          Initialize
           ******************************************************************************/
        /// <summary>
        /// Initialize: adds content to a Window and creates an Animation.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("---Initialize---");

            Window.Width    = 100d;
            Window.Height   = 100d;
            Window.Top      = 0d;
            Window.Left     = 0d;
            Window.Title    = "animation"; 

            Canvas body = new Canvas();
            Window.Content = body;

            DoubleAnimation anim1 = new DoubleAnimation();
            anim1.By                = .1;
            anim1.BeginTime         = TimeSpan.FromMilliseconds(0);
            anim1.Duration          = new Duration(TimeSpan.FromMilliseconds(500));
            anim1.IsAdditive        = true;
            anim1.IsCumulative      = true;

            _scBrush1 = new SolidColorBrush(Colors.Black);
            _scBrush1.Opacity = .5;
            body.Background = _scBrush1;

            _AT = (AnimationTimeline)anim1;
            
            return TestResult.Pass;
        }

        /******************************************************************************
           * Function:          StartTest
           ******************************************************************************/
        /// <summary>
        /// StartTest: checks properties and starts the Animation.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult StartTest()
        {
            //TEST 1: CreateClock------------------------------------------------------------------
            _AT.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            _clock = _AT.CreateClock();
            
            bool bClock = (_clock != null);
            if (bClock)
            {
                _testPassed = true;
                _resultInfo += "PASS/";
            }
            else
            {
                _testPassed = false;
                _resultInfo += "FAIL/";
            }
            GlobalLog.LogEvidence("[1] CreateClock(): Act: (clock != null) : " + bClock);


            //TEST 2: Clone------------------------------------------------------------------------
            _AT2 = _AT.Clone();
            
            bool bClone = (_AT2 != null);
            if (bClone)
            {
                _resultInfo += "PASS/";
            }
            else
            {
                _testPassed = false;
                _resultInfo += "FAIL/";
            }
            GlobalLog.LogEvidence("[2] Clone(): Act: (clone != null) : " + bClone);


            //TEST 3a: TargetPropertyType-1--------------------------------------------------------
            string expTPT1 = "System.Double";
            string actTPT1 = _AT.TargetPropertyType.ToString();
            if (actTPT1 == expTPT1)
            {
                _resultInfo += "PASS/";
            }
            else
            {
                _testPassed = false;
                _resultInfo += "FAIL/";
            }
            GlobalLog.LogEvidence("[3a] TargetPropertyType 1 - Act: " + actTPT1 + " / Exp: " + expTPT1);


            //TEST 3b: TargetPropertyType-2--------------------------------------------------------
            string expTPT2 = "System.Double";
            string actTPT2 = _AT.TargetPropertyType.ToString();
            if (actTPT2 == expTPT2)
            {
                _resultInfo += "PASS/";
            }
            else
            {
                _testPassed = false;
                _resultInfo += "FAIL/";
            }
            GlobalLog.LogEvidence("[3b] TargetPropertyType 2 - Act: " + actTPT2 + " / Exp: " + expTPT2);


            //TEST 4a: IsDestinationDefault-1--------------------------------------------------------------
            bool expIsDestinationDefault1 = false;
            bool actIsDestinationDefault1 = _AT.IsDestinationDefault;
            if (actIsDestinationDefault1 == expIsDestinationDefault1)
            {
                _resultInfo += "PASS/";
            }
            else
            {
                _testPassed = false;
                _resultInfo += "FAIL/";
            }
            GlobalLog.LogEvidence("[4a] IsDestinationDefault 1 - Act: " + actIsDestinationDefault1 + " / Exp: " + expIsDestinationDefault1);


            //TEST 4b: IsDestinationDefault-2--------------------------------------------------------------
            bool expIsDestinationDefault2 = false;
            bool actIsDestinationDefault2 = _AT2.IsDestinationDefault;
            if (actIsDestinationDefault2 == expIsDestinationDefault2)
            {
                _resultInfo += "PASS/";
            }
            else
            {
                _testPassed = false;
                _resultInfo += "FAIL/";
            }
            GlobalLog.LogEvidence("[4b] IsDestinationDefault 1 - Act: " + actIsDestinationDefault2 + " / Exp: " + expIsDestinationDefault2);


            //TEST 5a: GetCurrentValue-1----------------------------------------------------------------
            double expGetCurrentValue1 = 0d;
            double actGetCurrentValue1 = (double)_AT.GetCurrentValue(0d, 0d, _clock);
            if (actGetCurrentValue1 == expGetCurrentValue1)
            {
                _resultInfo += "PASS/";
            }
            else
            {
                _testPassed = false;
                _resultInfo += "FAIL/";
            }
            GlobalLog.LogEvidence("[5a] GetCurrentValue 1 - Act: " + actGetCurrentValue1 + " / Exp: " + expGetCurrentValue1);


            //TEST 5b: GetCurrentValue-2----------------------------------------------------------------
            double expGetCurrentValue2 = 0d;
            double actGetCurrentValue2 = (double)_AT2.GetCurrentValue(0d, 0d, _clock);
            if (actGetCurrentValue2 == expGetCurrentValue2)
            {
                _resultInfo += "PASS/";
            }
            else
            {
                _testPassed = false;
                _resultInfo += "FAIL/";
            }
            GlobalLog.LogEvidence("[5b] GetCurrentValue 2 - Act: " + actGetCurrentValue2 + " / Exp: " + expGetCurrentValue2);


            _scBrush1.ApplyAnimationClock(SolidColorBrush.OpacityProperty, _clock);

            return TestResult.Pass;
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Used for validating GetCurrentValue.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {               
            //GlobalLog.LogStatus("CurrentStateInvalidated:  " + ((Clock)sender).CurrentState);
            
            if ( ((Clock)sender).CurrentState == ClockState.Filling)
            {
                //TEST 5c: GetCurrentValue-3----------------------------------------------------------------
                double expGetCurrentValue3 = .6d;
                double actGetCurrentValue3 = (double)_AT.GetCurrentValue(.5d, 0d, _clock);
                if (actGetCurrentValue3 == expGetCurrentValue3)
                {
                    _resultInfo += "PASS/";
                }
                else
                {
                    _testPassed = false;
                    _resultInfo += "FAIL/";
                }
                GlobalLog.LogEvidence("[5c] GetCurrentValue 3 - Act: " + actGetCurrentValue3 + " / Exp: " + expGetCurrentValue3);

                
                //TEST 5d: GetCurrentValue-4----------------------------------------------------------------
                double expGetCurrentValue4 = .6d;
                double actGetCurrentValue4 = (double)_AT2.GetCurrentValue(.5d, 0d, _clock);
                if (actGetCurrentValue4 == expGetCurrentValue4)
                {
                    _resultInfo += "PASS]";
                }
                else
                {
                    _testPassed = false;
                    _resultInfo += "FAIL]";
                }
                GlobalLog.LogEvidence("[5d] GetCurrentValue 4 - Act: " + actGetCurrentValue4 + " / Exp: " + expGetCurrentValue4);

                GlobalLog.LogEvidence(_resultInfo);

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
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }

        #endregion
    }
}
