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
    /// <area>Animation.PropertyMethodEvent.Regressions</area>


    [Test(2, "Animation.PropertyMethodEvent.Regressions", "NullClockControllerTeset")]
    public class NullClockControllerTest : WindowTest
    {
        #region Test case members

        private string              _inputString     = "";
        private TextBox             _textbox         = null;
        private double              _fromValue       = 50d;
        private double              _toValue         = 250d;
        private bool                _pass1           = false;
        private bool                _pass2           = false;
        private DispatcherTimer     _aTimer          = null;
        
        #endregion


        #region Constructor
        
        [Variation("BeginAnimation")]
        [Variation("Storyboard")]
        
        /******************************************************************************
        * Function:          NullClockControllerTest Constructor
        ******************************************************************************/
        public NullClockControllerTest(string testValue)
        {
            _inputString = testValue;
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
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            Canvas body  = new Canvas();
            Window.Content = body;

            _textbox = new TextBox();
            body.Children.Add(_textbox);
            _textbox.Height  = 50d;
            _textbox.Width   = 100d;
            
            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.BeginTime          = TimeSpan.FromMilliseconds(0);
            anim.Duration           = new Duration(TimeSpan.FromMilliseconds(1000));
            anim.From               = _fromValue;
            anim.To                 = _toValue;
            anim.CurrentStateInvalidated       += new EventHandler(OnState);
            
            if (_inputString == "BeginAnimation")
            {
                _textbox.BeginAnimation(TextBox.WidthProperty, anim);
            }
            else
            {
                Storyboard storyboard = new Storyboard();
                storyboard.Name = "story";
                storyboard.Children.Add(anim);

                PropertyPath path1  = new PropertyPath("(0)", new DependencyProperty[] { TextBox.WidthProperty });
                Storyboard.SetTargetProperty(anim, path1);

                storyboard.Begin(_textbox, HandoffBehavior.SnapshotAndReplace, true);
            }
            
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1500);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");
        }
        
        /******************************************************************************
        * Function:          OnState
        ******************************************************************************/
        /// <summary>
        /// The handler for the Animation's CurrentStateInvalidated event.
        /// </summary>
        /// <returns></returns>
        private void OnState(object sender, EventArgs e)
        {
            Clock clock = ((Clock)sender);
            
            GlobalLog.LogStatus("----clock.CurrentState: " + clock.CurrentState);
            
            if (clock.CurrentState == ClockState.Filling)
            {
                GlobalLog.LogEvidence("----clock.CurrentGlobalSpeed: " + clock.CurrentGlobalSpeed);
                GlobalLog.LogEvidence("----clock.Controller:         " + clock.Controller);
                
                _pass1 = (clock.CurrentGlobalSpeed == 0d);
                if (_inputString == "BeginAnimation")
                {
                    _pass2 = (clock.Controller != null);
                }
                else if(_inputString == "Storyboard")
                {
                    _pass2 = (clock.Controller == null);
                }
            }
        }
        
        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)          
        {
            Signal("AnimationDone", TestResult.Pass);
            _aTimer.Stop();
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
            
            GlobalLog.LogEvidence("CurrentGlobalSpeed=0: " + _pass1);
            GlobalLog.LogEvidence("Controller:           " + _pass2);
            
            if (_pass1 && _pass2)
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
