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
    /// Verify ColorAnimation applied to SolidColorBrushes on multiple Canvases
    /// </description>
    /// </summary>
    
    // [DISABLE WHILE PORTING]
    //[Test(0, "Animation.PropertyMethodEvent.UnitTests", "ColorAnimationTest")]
    public class ColorAnimationTest : WindowTest
    {
        #region Test case members

        private ClockManager        _myClockManager;
        private SideBySideVerifier  _sideBySide;

        private bool                _testPassed          = true;
        private string              _resultInfo          = "Results: ";
        
        private AnimationClock      _clock1              = null;
        private AnimationClock      _clock2              = null;
        private AnimationClock      _clock3              = null;

        private ColorAnimation      _anim1               = new ColorAnimation();
        private ColorAnimation      _anim2               = new ColorAnimation();
        private ColorAnimation      _anim3               = new ColorAnimation();
        
        private SolidColorBrush     _scBrush1;
        private SolidColorBrush     _scBrush2;
        private SolidColorBrush     _scBrush3;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ColorAnimationTest Constructor
        ******************************************************************************/
        public ColorAnimationTest()
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
            Window.Title                = "ColorAnimation"; 
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            Canvas innerCanvas1 = new Canvas();
            Canvas.SetTop(innerCanvas1,0);
            Border border1 = new Border();
            _scBrush1 = new SolidColorBrush(Colors.Blue);
            border1.Background = _scBrush1;
            border1.BorderThickness = new Thickness(2.0);
            border1.Width = 200.0;
            border1.Height = 200.0;
            innerCanvas1.Children.Add(border1);  

            Canvas innerCanvas2 = new Canvas();
            Canvas.SetTop(innerCanvas2,200);
            Border border2 = new Border();
            _scBrush2 = new SolidColorBrush(Colors.Red);
            border2.Background = _scBrush2;
            border2.BorderThickness = new Thickness(2.0);
            border2.Width = 200.0;
            border2.Height = 200.0;
            innerCanvas2.Children.Add(border2);  

            Canvas innerCanvas3 = new Canvas();
            Canvas.SetLeft(innerCanvas3,200);
            Border border3 = new Border();
            _scBrush3 = new SolidColorBrush(Colors.Yellow);
            border3.Background = _scBrush3;
            border3.BorderThickness = new Thickness(2.0);
            border3.Width = 200.0;
            border3.Height = 200.0;
            innerCanvas3.Children.Add(border3); 

            Canvas myCanvas = new Canvas();
            myCanvas.Children.Add(innerCanvas1);
            myCanvas.Children.Add(innerCanvas2);
            myCanvas.Children.Add(innerCanvas3);

            Window.Content = myCanvas;
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// OnContentRendered: used to create and start an Animation after the page content is present.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            _anim1.By = Colors.Red;
            _anim1.BeginTime = TimeSpan.FromMilliseconds(2000);
            _anim1.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            _anim1.AutoReverse  = false;
            _anim1.FillBehavior = FillBehavior.HoldEnd;

            _clock1 = _anim1.CreateClock();
            _scBrush1.ApplyAnimationClock(System.Windows.Media.SolidColorBrush.ColorProperty, _clock1);


            _anim2.From = Colors.Bisque;
            _anim2.To = Colors.Salmon;
            _anim2.BeginTime = TimeSpan.FromMilliseconds(2000);
            _anim2.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            _anim2.AutoReverse  = false;
            _anim2.FillBehavior = FillBehavior.HoldEnd;

            _clock2 = _anim2.CreateClock();
            _scBrush2.ApplyAnimationClock(System.Windows.Media.SolidColorBrush.ColorProperty, _clock2);


            _anim3.To = Colors.Blue;
            _anim3.BeginTime = TimeSpan.FromMilliseconds(2000);
            _anim3.Duration = new Duration(TimeSpan.FromMilliseconds(2000));
            _anim3.AutoReverse  = false;
            _anim3.FillBehavior = FillBehavior.HoldEnd;

            _clock3 = _anim3.CreateClock();
            _scBrush3.ApplyAnimationClock(System.Windows.Media.SolidColorBrush.ColorProperty, _clock3);

            _sideBySide = new SideBySideVerifier(Window);
            _sideBySide.RegisterAnimation(_scBrush1,SolidColorBrush.ColorProperty, _clock1);
            _sideBySide.RegisterAnimation(_scBrush2,SolidColorBrush.ColorProperty, _clock2);
            _sideBySide.RegisterAnimation(_scBrush3,SolidColorBrush.ColorProperty, _clock3);

            int[] times = new int[]{1500,2050,3000,3950,4050};
            _myClockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);

            _myClockManager.hostManager.Resume();
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
                _testPassed = false; _resultInfo+="\n*********************\n";
            }

            _resultInfo += _sideBySide.verboseLog;

            if (e.lastTick) 
            {
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
