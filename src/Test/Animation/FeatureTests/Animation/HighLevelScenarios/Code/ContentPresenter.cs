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
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Regressions</area>


    [Test(2, "Animation.HighLevelScenarios.Regressions", "ContentPresenterTest")]
    public class ContentPresenterTest : XamlTest
    {

        #region Test case members

        private string                              _windowTitle         = "Animation Regression Test";
        private ImageComparator                     _imageCompare        = new ImageComparator();
        private DispatcherTimer                     _aTimer              = null;
        private int                                 _tickCount           = 0;
        private Rectangle                           _clientRect;
        
        private Bitmap                              _beforeCapture;
        private Bitmap                              _afterCapture;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          ContentPresenterTest Constructor
        ******************************************************************************/
        public ContentPresenterTest() : base(@"ContentPresenter.xaml")
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
        /// <returns>TestResult.Pass</returns>
        TestResult Initialize()
        {
            Window.Left                 = 0d;
            Window.Top                  = 0d;
            Window.Height               = 640;
            Window.Width                = 480;
            Window.Title = _windowTitle;

            //clientRect = new System.Drawing.Rectangle((int)Window.Left, (int)Window.Top, (int)Window.Width, (int)Window.Height);              
            _clientRect = new System.Drawing.Rectangle(0, 0, 640, 480);              

            //Take a picture of the screen before the Animation starts.
            _beforeCapture = CaptureTheScreen();
            WaitForSignal("CaptureDone");

            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult StartTimer()
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
            
            if (_tickCount == 8 )
            {
                //Take a picture of the screen after the Animation is done.
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
        /// <returns>A TestResult, indicating Pass or Fail</returns>
        TestResult Verify()
        {
            WaitForSignal("AnimationDone");

            GlobalLog.LogStatus("Animation Done -- Start Bitmap Comparison");

            bool pass1 = _imageCompare.Compare(new ImageAdapter(_beforeCapture), new ImageAdapter(_afterCapture), true);

            if ( !pass1 ) 
            { 
                GlobalLog.LogEvidence("All screen captures were different");
                return TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Before and After animation captures were identical");
                return TestResult.Fail;
            }
        }
        
        #endregion
    }
}
