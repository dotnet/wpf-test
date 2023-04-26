// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
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
    /// <area>Animation.LowLevelScenarios.Regressions</area>

    [Test(2, "Animation.LowLevelScenarios.Regressions", "ClockRemovalTest")]
    public class ClockRemovalTest : WindowTest
    {
        #region Test case members

        private ListBox                         _listbox1;
        private SolidColorBrush                 _SCB;
        private AnimationClock                  _clock;
        private Nullable<Double>                _progress;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ClockRemovalTest Constructor
        ******************************************************************************/
        public ClockRemovalTest()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
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

            Canvas body = new Canvas();
            body.Background = Brushes.Black;
            body.Height     = 300d;
            body.Width      = 300d;
            
            _listbox1 = new ListBox();
            body.Children.Add(_listbox1);
            _listbox1.Height = 150d;
            _listbox1.Width  = 150d;
            Canvas.SetTop  (_listbox1, 50d);
            Canvas.SetLeft (_listbox1, 50d);   

            _SCB = new SolidColorBrush();
            _SCB.Color = Colors.OrangeRed;
            _listbox1.Background = _SCB;

            Window.Content = body;
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,500);
            _aTimer.Start();
            
            GlobalLog.LogStatus("----DispatcherTimer Started----");
            
            return TestResult.Pass;
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
            _tickCount++;
            
            if (_tickCount == 1)
            {
                ColorAnimation animColor = new ColorAnimation();
                animColor.To                = Colors.Blue;
                animColor.BeginTime         = TimeSpan.FromMilliseconds(0);
                animColor.Duration          = new Duration(TimeSpan.FromMilliseconds(250));

                _clock = animColor.CreateClock();
                _SCB.ApplyAnimationClock(SolidColorBrush.ColorProperty, _clock);
                _clock.Controller.Begin();
            }
            else if (_tickCount == 2)
            {
                _clock.Controller.Remove();
            }
            else if (_tickCount == 5)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
            }
            else
            {
                _progress = _clock.CurrentProgress;
                GlobalLog.LogStatus("----CurrentProgress: " + _progress);
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
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected CurrentProgress null: True");
            GlobalLog.LogEvidence("Actual CurrentProgress null:   " + (_progress == null).ToString());
            
            if (_progress == null)
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
