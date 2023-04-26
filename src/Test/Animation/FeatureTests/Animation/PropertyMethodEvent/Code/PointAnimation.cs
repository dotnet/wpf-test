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
    /// Verify PointAnimation applied to LinearGradientBrushes on multiple Borders
    /// </description>
    /// </summary>
    
    [Test(0, "Animation.PropertyMethodEvent.UnitTests", "PointAnimationTest")]
    public class PointAnimationTest : WindowTest
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
        * Function:          PointAnimationTest Constructor
        ******************************************************************************/
        public PointAnimationTest()
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
            Window.Title                = "PointAnimation"; 

            int[] times = new int[]{2000,3500,4000,4500,5500};
            _myClockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);

            PointAnimation anim1 = new PointAnimation();
            anim1.By                = new Point(1.0, 0.0);
            anim1.BeginTime         = TimeSpan.FromMilliseconds(3000);
            anim1.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim1.FillBehavior      = FillBehavior.HoldEnd;

            LinearGradientBrush brush1 = new LinearGradientBrush();
            brush1.StartPoint       = new Point(0.0, 0.0);
            brush1.EndPoint         = new Point(0.0, 1.0);
            brush1.MappingMode      = BrushMappingMode.RelativeToBoundingBox;
            brush1.GradientStops    = new GradientStopCollection();
            brush1.GradientStops.Add(new GradientStop(Colors.Red, 0.0));
            brush1.GradientStops.Add(new GradientStop(Colors.Blue, 1.0));
            _clock1 = anim1.CreateClock();
            brush1.ApplyAnimationClock(LinearGradientBrush.EndPointProperty, _clock1);

            Border myBorder = new Border();
            myBorder.Width          = 50;
            myBorder.Height         = 50;
            myBorder.Background     = brush1;

            PointAnimation anim2 = new PointAnimation();
            anim2.From              = new Point(1.0, 0.0);
            anim2.To                = new Point(0.0, 1.0);
            anim2.BeginTime         = TimeSpan.FromMilliseconds(3000);
            anim2.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim2.FillBehavior      = FillBehavior.HoldEnd;

            LinearGradientBrush brush2 = new LinearGradientBrush();
            brush2.StartPoint       = new Point(0.0, 0.0);
            brush2.EndPoint         = new Point(1.0, 1.0);
            brush2.MappingMode      = BrushMappingMode.RelativeToBoundingBox;
            brush2.GradientStops    = new GradientStopCollection();
            brush2.GradientStops.Add(new GradientStop(Colors.Orange, 0.0));
            brush2.GradientStops.Add(new GradientStop(Colors.Purple, 1.0));
            _clock2 = anim2.CreateClock();
            brush2.ApplyAnimationClock(LinearGradientBrush.EndPointProperty, _clock2);

            Border myBorder2 = new Border();
            myBorder2.Width          = 50;
            myBorder2.Height         = 50;
            myBorder2.Background     = brush2;
            Canvas.SetTop(myBorder2,200);

            PointAnimation anim3 = new PointAnimation();
            anim3.To                = new Point(1.0, 0.0);
            anim3.BeginTime         = TimeSpan.FromMilliseconds(3000);
            anim3.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim3.FillBehavior      = FillBehavior.HoldEnd;

            LinearGradientBrush brush3 = new LinearGradientBrush();
            brush3.StartPoint       = new Point(0.0, 0.0);
            brush3.EndPoint         = new Point(0.0, 1.0);
            brush3.MappingMode      = BrushMappingMode.RelativeToBoundingBox;
            brush3.GradientStops    = new GradientStopCollection();
            brush3.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
            brush3.GradientStops.Add(new GradientStop(Colors.Green, 1.0));
            _clock3 = anim3.CreateClock();
            brush3.ApplyAnimationClock(LinearGradientBrush.EndPointProperty, _clock3);

            Border myBorder3 = new Border();
            myBorder3.Width          = 50;
            myBorder3.Height         = 50;
            myBorder3.Background     = brush3;
            Canvas.SetTop(myBorder3,300);

            Canvas myCanvas = new Canvas();
            myCanvas.Width      = 50.0;
            myCanvas.Height     = 50.0;

            myCanvas.Children.Add(myBorder);  
            myCanvas.Children.Add(myBorder2);  
            myCanvas.Children.Add(myBorder3);  

            Window.Content = myCanvas;

            _sideBySide = new SideBySideVerifier(Window);
            _sideBySide.RegisterAnimation(brush1,LinearGradientBrush.EndPointProperty, _clock1);
            _sideBySide.RegisterAnimation(brush2,LinearGradientBrush.EndPointProperty, _clock2);
            _sideBySide.RegisterAnimation(brush3,LinearGradientBrush.EndPointProperty, _clock3);

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
                _resultInfo+="*********************\n";
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
