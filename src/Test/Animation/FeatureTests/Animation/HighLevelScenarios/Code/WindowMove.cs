// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    // [DISABLE WHILE PORTING]
    // [Test(2, "Animation.HighLevelScenarios.Regressions", "WindowMoveTest")]

    class WindowMoveTest : WindowTest
    {
        #region Test case members

        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;

        private Int16               _initialLeft         = 400;    
        private Int16               _initialTop          = 20;    
        private Int16               _initialWidth        = 200;  
        private Int16               _initialHeight       = 550;

        private double              _midLeft             = 200d;    
        private double              _midWidth            = 100d;    

        private double              _finalLeft           = 0d;    
        private double              _finalWidth          = 50d;    

        #endregion


        #region Constructor


        /******************************************************************************
        * Function:          WindowMoveTest Constructor
        ******************************************************************************/
        public WindowMoveTest()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }
        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize:  Navigate to the first .xaml page.
        /// </summary>
        /// <returns></returns>
        TestResult Initialize()
        {
            Window.Width        = Convert.ToDouble(_initialWidth);
            Window.Height       = Convert.ToDouble(_initialHeight);
            Window.Left         = Convert.ToDouble(_initialLeft);
            Window.Top          = Convert.ToDouble(_initialTop);

            NameScope.SetNameScope(Window, new NameScope());

            Grid body  = new Grid();
            Window.Content = body;

            Button button1 = new Button();
            button1.Background = Brushes.LightGreen;
            body.Children.Add(button1);

            DoubleAnimation anim1 = new DoubleAnimation();
            anim1.To              = _midWidth;
            anim1.BeginTime       = TimeSpan.FromMilliseconds(0);
            anim1.Duration        = new Duration(TimeSpan.FromMilliseconds(300));

            DoubleAnimation anim2 = new DoubleAnimation();
            anim2.To              = _midLeft;
            anim2.BeginTime       = TimeSpan.FromMilliseconds(0);
            anim2.Duration        = new Duration(TimeSpan.FromMilliseconds(300));

            //MouseEnter Storyboard.
            BeginStoryboard  beginStory1 = AnimationUtilities.CreateBeginStoryboard(Window, "story1");
            Storyboard storyboard1 = new Storyboard();
            storyboard1.Children.Add(anim1);
            storyboard1.Children.Add(anim2);
            Storyboard.SetTargetProperty(anim1, new PropertyPath(Window.WidthProperty));
            Storyboard.SetTargetProperty(anim2, new PropertyPath(Canvas.LeftProperty));
            beginStory1.Storyboard = storyboard1;

            //MouseLeave Storyboard.
            anim1.To              = _finalWidth;
            anim2.To              = _finalLeft;
            BeginStoryboard  beginStory2 = AnimationUtilities.CreateBeginStoryboard(Window, "story2");
            Storyboard storyboard2 = new Storyboard();
            storyboard2.Children.Add(anim1);
            storyboard2.Children.Add(anim2);
            Storyboard.SetTargetProperty(anim1, new PropertyPath(Window.WidthProperty));
            Storyboard.SetTargetProperty(anim2, new PropertyPath(Canvas.LeftProperty));
            beginStory2.Storyboard = storyboard2;

            EventTrigger mouseEnterTrigger = AnimationUtilities.CreateEventTrigger(Window.MouseEnterEvent, beginStory1);
            EventTrigger mouseLeaveTrigger = AnimationUtilities.CreateEventTrigger(Window.MouseLeaveEvent, beginStory2);
            
            Window.Triggers.Add(mouseEnterTrigger);
            Window.Triggers.Add(mouseLeaveTrigger);

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
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
                int x = _initialLeft + (_initialWidth / 2);
                int y = _initialTop  + (_initialHeight / 2);
                UserInput.MouseMove(x+ 50, y);
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

            double actValue1 = (double)Window.GetValue(Window.WidthProperty);
            double expValue1 = _finalWidth;
            double actValue2 = (double)Window.GetValue(Window.LeftProperty);
            double expValue2 = Convert.ToDouble(_finalLeft);
            
            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Value [Width]:  " + expValue1);
            GlobalLog.LogEvidence("Actual Value   [Width]:  " + actValue1);
            GlobalLog.LogEvidence("Expected Value [Left]:   " + expValue2);
            GlobalLog.LogEvidence("Actual Value   [Left]:   " + actValue2);
            
            if (actValue1 == expValue1 && actValue2 == expValue2)
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
