// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
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
    /// <area>Animation.PropertyMethodEvent.Constructors</area>
    /// <priority>0</priority>
    /// <description>
    /// Verify Constructors for ByteAnimation.
    /// </description>
    /// </summary>
    
    [Test(0, "Animation.PropertyMethodEvent.Constructors", "ByteConstructorsTest")]
    public class ByteConstructorsTest : WindowTest
    {
        #region Test case members

        private ByteAnimation   _anim1;
        private ByteAnimation   _anim2;
        private ByteAnimation   _anim3;
        private ByteAnimation   _anim4;
        private AnimationClock  _animClock1;
        private AnimationClock  _animClock2;
        private AnimationClock  _animClock3;
        private AnimationClock  _animClock4;
        private Clock _tlc;

        private Byte            _baseValue           = 0;
        private Byte            _setValue            = 2;

        private TextBlock       _myText;
        private string          _resultString        = "Animations: ";
        private bool            _testPassed          = true;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ByteConstructorsTest Constructor
        ******************************************************************************/
        public ByteConstructorsTest()
        {
            InitializeSteps += new TestStep(StartTest);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult StartTest()
        {
            GlobalLog.LogStatus("---StartTest---");
            Window.Width = 500;
            Window.Height = 500;
            Window.Title = "This is my new animation window"; 


            _myText  = new TextBlock();
            _myText.Text += Environment.NewLine + "Running Animation Constructors Test";

            Window.Content = _myText;


            _anim1 = new ByteAnimation(_setValue,new Duration(TimeSpan.FromMilliseconds(10000)));
            _animClock1 = (AnimationClock)_anim1.CreateClock(); 

            _anim2 = new ByteAnimation(_setValue,new Duration(TimeSpan.FromMilliseconds(10000)),FillBehavior.HoldEnd);
            _animClock2 = (AnimationClock)_anim2.CreateClock(); 

            _anim3 = new ByteAnimation(_baseValue,_setValue,new Duration(TimeSpan.FromMilliseconds(10000)));
            _animClock3 = (AnimationClock)_anim3.CreateClock(); 

            _anim4 = new ByteAnimation(_baseValue,_setValue,new Duration(TimeSpan.FromMilliseconds(10000)),FillBehavior.HoldEnd);
            _animClock4 = (AnimationClock)_anim4.CreateClock(); 

            GlobalLog.LogStatus("Byte Animations created & parented to Timeline.Root");
            _myText.Text += Environment.NewLine + "Animations created & parented to Timeline.Root";

            // verification timer

            ParallelTimeline tn = new ParallelTimeline();
            tn.BeginTime = TimeSpan.FromMilliseconds(0);
            tn.Duration = new Duration(TimeSpan.FromMilliseconds(5500));

            tn.CurrentStateInvalidated += new EventHandler(OnTimelineEnded);
            _tlc = tn.CreateClock();
            
            return TestResult.Pass;
        }


        private void OnTimelineEnded(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != ClockState.Active)  
            {         
                GlobalLog.LogEvidence("\n To Constructor GetValue result: " + _animClock1.GetCurrentValue(_baseValue, _baseValue));
                if ((Byte)_animClock1.GetCurrentValue(_baseValue, _baseValue) == _baseValue)
                { _testPassed = false; _resultString += "\nGetValue on To constructor returned the base value"; }

                GlobalLog.LogEvidence("\n To Constructor GetValue result: " + _animClock2.GetCurrentValue(_baseValue, _baseValue));
                if ((Byte)_animClock2.GetCurrentValue(_baseValue, _baseValue) == _baseValue)
                { _testPassed = false; _resultString += "\nGetValue on To with Fill constructor returned the base value"; }

                GlobalLog.LogEvidence("\n To Constructor GetValue result: " + _animClock3.GetCurrentValue(_baseValue, _baseValue));
                if ((Byte)_animClock3.GetCurrentValue(_baseValue, _baseValue) == _baseValue)
                { _testPassed = false; _resultString += "\nGetValue on FromTo constructor returned the base value"; }

                GlobalLog.LogEvidence("\n To Constructor GetValue result: " + _animClock4.GetCurrentValue(_baseValue, _baseValue));
                if ((Byte)_animClock4.GetCurrentValue(_baseValue, _baseValue) == _baseValue)
                { _testPassed = false; _resultString += "\nGetValue on FromTo with Fill constructor returned the base value"; }


                // check for each individual event
                if (_resultString == "Animations: ")
                {
                    _testPassed = true;
                    _resultString += " all constructors worked\n\n";
                }

                Signal("AnimationDone", TestResult.Pass);
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
            WaitForSignal("AnimationDone");
            
            GlobalLog.LogEvidence(_resultString);

            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion
    }
}
