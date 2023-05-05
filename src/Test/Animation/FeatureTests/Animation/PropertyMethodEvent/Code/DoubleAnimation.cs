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
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.UnitTests</area>
    /// <priority>0</priority>
    /// <description>
    /// Verify DoubleAnimation applied to SolidColorBrushes on multiple Borders
    /// </description>
    /// </summary>
    
    [Test(0, "Animation.PropertyMethodEvent.UnitTests", "DoubleAnimationTest")]
    public class DoubleAnimationTest : WindowTest
    {
        #region Test case members

        private ClockManager        _myClockManager;
        private SideBySideVerifier  _sideBySide;

        private bool                _testPassed          = true;
        private string              _resultInfo          = "Results: ";
        
        private AnimationClock      _clock1              = null;
        private AnimationClock      _clock2              = null;
        private AnimationClock      _clock3              = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          DoubleAnimationTest Constructor
        ******************************************************************************/
        public DoubleAnimationTest()
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

            Window.Width                = 500;
            Window.Height               = 500;
            Window.Title                = "DoubleAnimation"; 

            int[] times = new int[]{4500,5050,6000,6950,7050};
            _myClockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);

            Canvas myCanvas = new Canvas();

            Window.Content = (myCanvas);

            DoubleAnimation anim1 = new DoubleAnimation();
            anim1.By            = 1;
            anim1.BeginTime     = TimeSpan.FromMilliseconds(5000);
            anim1.Duration      = new Duration(TimeSpan.FromMilliseconds(2000));
            anim1.FillBehavior  = FillBehavior.HoldEnd;

            SolidColorBrush scBrush1 = new SolidColorBrush(Colors.Red);
            scBrush1.Opacity = .5;
            
            _clock1 = anim1.CreateClock();
            scBrush1.ApplyAnimationClock(SolidColorBrush.OpacityProperty, _clock1);

            Border border = new Border();
            border.Background           = scBrush1;
            border.BorderThickness      = new Thickness(2.0);
            border.Width                = 200.0;
            border.Height               = 200.0;

            myCanvas.Children.Add(border);  

            DoubleAnimation anim2 = new DoubleAnimation();
            anim2.From          = .3;
            anim2.To            = .7;
            anim2.BeginTime     = TimeSpan.FromMilliseconds(5000);
            anim2.Duration      = new Duration(TimeSpan.FromMilliseconds(2000));
            anim2.FillBehavior  = FillBehavior.HoldEnd;

            SolidColorBrush scBrush2 = new SolidColorBrush(Colors.Red);
            scBrush2.Opacity = 1;

            _clock2 = anim2.CreateClock();
            scBrush2.ApplyAnimationClock(SolidColorBrush.OpacityProperty, _clock2);

            Border border2 = new Border();
            border2.Background          = scBrush2;
            border2.BorderThickness     = new Thickness(2.0);
            border2.Width               = 200.0;
            border2.Height              = 200.0;
            Canvas.SetLeft(border,225.0);

            myCanvas.Children.Add(border2);  

            DoubleAnimation anim3 = new DoubleAnimation();
            anim3.To            = .9;
            anim3.BeginTime     = TimeSpan.FromMilliseconds(5000);
            anim3.Duration      = new Duration(TimeSpan.FromMilliseconds(2000));

            SolidColorBrush scBrush3 = new SolidColorBrush(Colors.Red);
            scBrush3.Opacity = .2;

            _clock3 = anim3.CreateClock();
            scBrush3.ApplyAnimationClock(SolidColorBrush.OpacityProperty, _clock3);

            Border border3 = new Border();
            border3.Background          = scBrush3;
            border3.BorderThickness     = new Thickness(2.0);
            border3.Width               = 200.0;
            border3.Height              = 200.0;
            Canvas.SetTop(border3,225.0);

            myCanvas.Children.Add(border3);  

            _sideBySide = new SideBySideVerifier(Window);
            _sideBySide.RegisterAnimation(scBrush1, SolidColorBrush.OpacityProperty, _clock1);
            _sideBySide.RegisterAnimation(scBrush2, SolidColorBrush.OpacityProperty, _clock2);
            _sideBySide.RegisterAnimation(scBrush3, SolidColorBrush.OpacityProperty, _clock3);

            _myClockManager.hostManager.Resume();
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          OnTimeTicked
        ******************************************************************************/
        /// <summary>
        /// OnTimeTicked:  Verifies the Animation at specified intervals.
        /// </summary>
        private void OnTimeTicked(object sender, TimeControlArgs e)
        {
            bool tempResult = _sideBySide.Verify(e.curTime);
            if (!tempResult)
            {
                _testPassed = false;
                _resultInfo+="\n*********************\n";
            }

            _resultInfo += _sideBySide.verboseLog;
            GlobalLog.LogEvidence(_resultInfo);

            if (e.lastTick) 
            {
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
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }
        #endregion
    }
}
