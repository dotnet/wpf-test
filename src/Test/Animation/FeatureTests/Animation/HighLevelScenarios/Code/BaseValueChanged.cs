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
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;     //Bitmap comparisons.
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "BaseValueChangedTest")]
    public class BaseValueChangedTest : WindowTest
    {

        #region Test case members

        private string                              _windowTitle         = "Animation Regression Test";
        private ImageComparator                     _imageCompare        = new ImageComparator();
        private DispatcherTimer                     _aTimer              = null;
        private int                                 _tickCount           = 0;
        private SolidColorBrush                     _brush;
        private System.Drawing.Rectangle            _clientRect;
        
        private Bitmap                              _beforeCapture;
        private Bitmap                              _afterCapture;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          BaseValueChangedTest Constructor
        ******************************************************************************/
        public BaseValueChangedTest()
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
        /// Carries out initialization of the Window.
        /// </summary>
        /// <returns>Returns TestResult</returns>
        TestResult Initialize()
        {
            Window.Left                 = 0d;
            Window.Top                  = 0d;
            Window.Height               = 640;
            Window.Width                = 480;
            Window.Title = _windowTitle;

            Canvas body  = new Canvas();
            body.Background = System.Windows.Media.Brushes.WhiteSmoke;
            Window.Content = body;

            Ellipse ellipse1 = new Ellipse();
            
            ColorAnimation anim = new ColorAnimation(Colors.Green, TimeSpan.FromMilliseconds(500));
            anim.RepeatBehavior     = RepeatBehavior.Forever;
            anim.AutoReverse        = true;
            anim.BeginTime          = null;

            _brush = new SolidColorBrush();
            _brush.Color = Colors.Red;
            _brush.BeginAnimation(SolidColorBrush.ColorProperty, anim);

            ellipse1.Fill = _brush;
            ellipse1.Width = 100d;
            ellipse1.Height = 50d;
            Canvas.SetTop(ellipse1, 100d);

            body.Children.Add(ellipse1);

            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns success if the elements were found</returns>
        TestResult StartTimer()
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
            
            GlobalLog.LogStatus("**********Tick #" + _tickCount);
            
            if (_tickCount == 1 )
            {
                _clientRect = new System.Drawing.Rectangle(0, 0, 640, 480);

                //Take a picture of the screen before changing the Base Value.
                //The Animation has finished and is in a Filling state.
                _beforeCapture = CaptureTheScreen();
                WaitForSignal("CaptureDone");
            }
            else if (_tickCount == 2 )
            {
                _brush.Color = Colors.Blue;
            }
            else
            {
                //Take a picture of the screen after changing the Base Value.
                _afterCapture = CaptureTheScreen();
                WaitForSignal("CaptureDone");

                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
            }
        }


        /******************************************************************************
           * Function:          CaptureTheScreen
          ******************************************************************************/
        /// <summary>
        /// CaptureTheScreen: gets a screen capture and checks for null;
        /// </summary>
        /// <returns>A Bitmap, used for animation verification</returns>
        private Bitmap CaptureTheScreen()
        {
            Bitmap tempBitmap = ImageUtility.CaptureScreen(_clientRect);
            Signal("CaptureDone", TestResult.Pass);

            return tempBitmap;
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

            bool pass1 = _imageCompare.Compare(new ImageAdapter(_beforeCapture), new ImageAdapter(_afterCapture), true);
            bool pass2 = (_brush.Color == Colors.Blue);
            
            if ( !pass1 && pass2) 
            { 
                GlobalLog.LogEvidence("All screen captures were different");
                GlobalLog.LogEvidence("Brush base value changed to Blue");
                return TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Before and After animation captures were identical");
                GlobalLog.LogEvidence("and/or Brush base value not changed to Blue");
                return TestResult.Fail;
            }
        }
        
        #endregion
    }
}
