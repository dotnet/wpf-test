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
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>

    [Test(2, "Storyboards.LowLevelScenarios.Regressions", "PropertyPathNonFETest")]
    public class PropertyPathNonFETest : XamlTest
    {
        #region Test case members

        private int                         _tickCount           = 0;
        private DispatcherTimer             _aTimer              = null;
        private string                      _windowTitle         = "Animation Regression Test";
        private ImageComparator             _imageCompare        = new ImageComparator();
        private Rectangle                   _clientRect;
        
        private Bitmap _beforeCapture;
        private Bitmap _afterCapture;

        AnimationValidator _myValidator = new AnimationValidator();
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          PropertyPathNonFETest Constructor
        ******************************************************************************/
        public PropertyPathNonFETest() : base(@"PropertyPathNonFE.xaml")
        {
            InitializeSteps += new TestStep(Initialize);
            InitializeSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Carries out initialization.
        /// </summary>
        /// <returns>Returns TestResult</returns>
        TestResult Initialize()
        {
            Window.Left                 = 0d;
            Window.Top                  = 0d;
            Window.Height               = 600;
            Window.Width                = 600;
            Window.Title = _windowTitle;

            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult</returns>
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
            
            GlobalLog.LogStatus("**********Tick #" + _tickCount);
            
            if (_tickCount == 1 )
            {

                //clientRect = new System.Drawing.Rectangle((int)Window.Left, (int)Window.Top, (int)Window.Width, (int)Window.Height);              
                _clientRect = new System.Drawing.Rectangle(0, 0, 600, 600);              

                //Take a picture of the screen before the Animation starts.
                _beforeCapture = CaptureTheScreen();
                WaitForSignal("CaptureDone");
            }
            else if (_tickCount == 7 )
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
        private System.Drawing.Bitmap CaptureTheScreen()
        {
            System.Drawing.Bitmap tempBitmap = ImageUtility.CaptureScreen(_clientRect);
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
