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
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>

    [Test(2, "Storyboards.LowLevelScenarios.Regressions", "EventsWithDFRTest")]
    public class EventsWithDFRTest : WindowTest
    {

        #region Test case members

        private Canvas              _body;
        private Rectangle           _rectangle;
        private bool                _currentTimeCheck    = true;
        private bool                _currentStateCheck   = true;
        private bool                _currentTimeFired    = false;
        private bool                _currentStateFired   = false;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          EventsWithDFRTest Constructor
        ******************************************************************************/
        public EventsWithDFRTest()
        {
            InitializeSteps += new TestStep(CreateTree);
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
            NameScope.SetNameScope(Window, new NameScope());

            Window.Width        = 450d;
            Window.Height       = 450d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _body = new Canvas();
            _body.Background = Brushes.Indigo;

            _rectangle = new Rectangle();
            _body.Children.Add(_rectangle);
            _rectangle.Width = 100;
            _rectangle.Height = 100;
            _rectangle.Fill = Brushes.DodgerBlue;

            Window.Content = _body;
            
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
            DoubleAnimation animation = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(0.1));
            animation.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            animation.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
            
            Storyboard storyboard = new Storyboard();
            storyboard.Name = "story";
            storyboard.Children.Add(animation);
            
            Timeline.SetDesiredFrameRate(storyboard,30);
            
            PropertyPath path1 = new PropertyPath("(0)", new DependencyProperty[] { Rectangle.OpacityProperty });

            Storyboard.SetTargetProperty(storyboard, path1);
            
            BeginStoryboard beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");

            beginStory.Storyboard = storyboard;
            beginStory.Storyboard.Begin(_rectangle);
            
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1500);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");
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
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---OnCurrentStateInvalidated: " + ((Clock)sender).CurrentState);
            
            _currentStateFired = true;

            if ( ((Clock)sender).CurrentState != ClockState.Active && ((Clock)sender).CurrentState != ClockState.Filling )
            {
                _currentStateCheck = false;
            }
        }

        /******************************************************************************
        * Function:          OnCurrentTimeInvalidated
        ******************************************************************************/
        private void OnCurrentTimeInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---OnCurrentTimeInvalidated: " + ((Clock)sender).CurrentTime);
            
            _currentTimeFired = true;

            if ( ((Clock)sender).CurrentTime == null )
            {
                _currentTimeCheck = false;
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

            GlobalLog.LogEvidence("-----Verifying Event Firing-----");
            GlobalLog.LogEvidence("currentStateFired: " + _currentStateFired);
            GlobalLog.LogEvidence("currentTimeFired:  " + _currentTimeFired);
            GlobalLog.LogEvidence("currentStateCheck: " + _currentStateCheck);
            GlobalLog.LogEvidence("currentTimeCheck:  " + _currentTimeCheck);
            
            if (_currentStateCheck && _currentTimeCheck && _currentStateFired && _currentTimeFired)
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
