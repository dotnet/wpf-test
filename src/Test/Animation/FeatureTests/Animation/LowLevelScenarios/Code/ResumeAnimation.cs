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
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;   //UserInput
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.LowLevelScenarios.Regressions</area>
    /// <priority>0</priority>


    [Test(0, "Animation.LowLevelScenarios.Regressions", "ResumeAnimationTest")]
    public class ResumeAnimationTest : WindowTest
    {

        #region Test case members

        private Canvas              _animatedElement;
        private Button              _buttonStart;
        private Button              _buttonPause;
        private Button              _buttonResume;
        private Canvas              _body;
        private double              _fromValue           = 100d;
        private double              _toValue             = 150d;
        private Clock               _AC;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          ResumeAnimationTest Constructor
        ******************************************************************************/
        public ResumeAnimationTest()
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
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _body  = new Canvas();
            Window.Content = _body;

            _buttonStart = new Button();
            _body.Children.Add(_buttonStart);
            _buttonStart.Content = "Start";
            _buttonStart.Click += new RoutedEventHandler(OnStart); 
            _buttonStart.Background  = new SolidColorBrush(Colors.LightGreen);
            Canvas.SetTop  (_buttonStart, 50d);
            Canvas.SetLeft (_buttonStart, 20d);

            _buttonPause = new Button();
            _body.Children.Add(_buttonPause);
            _buttonPause.Content = "Pause";
            _buttonPause.Click += new RoutedEventHandler(OnPause); 
            _buttonPause.Background  = new SolidColorBrush(Colors.LightBlue);
            Canvas.SetTop  (_buttonPause, 100d);
            Canvas.SetLeft (_buttonPause, 20d);          

            _buttonResume = new Button();
            _body.Children.Add(_buttonResume);
            _buttonResume.Content = "Resume";
            _buttonResume.Click += new RoutedEventHandler(OnResume); 
            _buttonResume.Background  = new SolidColorBrush(Colors.Lavender);
            Canvas.SetTop  (_buttonResume, 150d);
            Canvas.SetLeft (_buttonResume, 20d);          

            _animatedElement = new Canvas();
            _animatedElement.Height           = 100d;
            _animatedElement.Width            = _fromValue;
            _animatedElement.Background       = new SolidColorBrush(Colors.BlueViolet);
            Canvas.SetTop  (_animatedElement, 0d);
            Canvas.SetLeft (_animatedElement, 80d);          
            _body.Children.Add(_animatedElement);

            GlobalLog.LogStatus("----Window created----");
            
            return TestResult.Pass;
        }
        
        private void OnStart(object sender, RoutedEventArgs e)
        {
            _AC.Controller.Begin();
        }           
        private void OnPause(object sender, RoutedEventArgs e)
        {
            _AC.Controller.Pause();
        }
        private void OnResume(object sender, RoutedEventArgs e)
        {
            _AC.Controller.Resume();
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
            anim.BeginTime      = TimeSpan.FromSeconds(0);
            anim.Duration       = new Duration(TimeSpan.FromSeconds(2));
            anim.From           = _fromValue;
            anim.To             = _toValue;

            ParallelTimeline pa = new ParallelTimeline();
            pa.BeginTime      = null;
            pa.Children.Add(anim);
            
            _AC = pa.CreateClock();
            _animatedElement.ApplyAnimationClock(Canvas.WidthProperty, (AnimationClock)((ClockGroup)_AC).Children[0]);
            //AC = anim.CreateClock();
            //animatedElement.ApplyAnimationClock(Canvas.WidthProperty, AC);


            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
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
            _tickCount++;
            
            if (_tickCount == 1)
            {
                UserInput.MouseLeftClickCenter(_buttonStart);
            }
            else if (_tickCount == 2)
            {
                UserInput.MouseLeftClickCenter(_buttonPause);
            }
            else if (_tickCount == 3)
            {
                UserInput.MouseLeftClickCenter(_buttonResume);
            }
            else
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
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

            double actValue = (double)_animatedElement.GetValue(Canvas.WidthProperty);
            double expValue = 125d;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: > " + expValue);
            GlobalLog.LogEvidence("Actual Value:     " + actValue);
            
            if (actValue > expValue)
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
