// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;   //UserInput
using Microsoft.Test.RenderingVerification; //Bitmap Comparisons
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>

    [Test(2, "Storyboards.LowLevelScenarios.Regressions", "PauseAfterCompletedTest")]
    public class PauseAfterCompletedTest : XamlTest
    {

        #region Test case members

        private string                      _inputString         = "";
        private Button                      _animatedElement     = null;
        private DispatcherTimer             _aTimer              = null;
        private int                         _tickCount           = 0;
        private string                      _windowTitle         = "Animation Regression Test";
        private ImageComparator             _imageCompare        = new ImageComparator();
        private Rectangle                   _clientRect;
        
        private Bitmap _beforeCapture;
        private Bitmap _afterCapture;

        AnimationValidator _myValidator = new AnimationValidator();
        
        #endregion


        #region Constructor

        [Variation(@"PauseBefore")]
        [Variation(@"PauseAfter")]

        /******************************************************************************
        * Function:          PauseAfterCompletedTest Constructor
        ******************************************************************************/
        public PauseAfterCompletedTest(string testValue) : base(@"PauseAfterCompleted.xaml")
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(GetAnimatedElement);
            RunSteps += new TestStep(StartTimer);
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

            //clientRect = new System.Drawing.Rectangle((int)Window.Left, (int)Window.Top, (int)Window.Width, (int)Window.Height);              
            _clientRect = new System.Drawing.Rectangle(0, 0, 600, 600);              

            //Take a picture of the screen before the Animation starts.
            _beforeCapture = CaptureTheScreen();
            WaitForSignal("CaptureDone");

            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          GetAnimatedElement
        ******************************************************************************/
        /// <summary>
        /// Looks in the Markup for the Animated element.
        /// </summary>
        /// <returns>TestResult=Success if the element is found</returns>
        private TestResult GetAnimatedElement()          
        {
            _animatedElement = (Button)LogicalTreeHelper.FindLogicalNode((Page)Window.Content,"button1");

            if (_animatedElement == null)
            {
                GlobalLog.LogEvidence("The animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The element was found.");
                return TestResult.Pass;
            }
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult=Success</returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,500);
            _aTimer.Start();
            GlobalLog.LogEvidence("----DispatcherTimer Started----");

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
            
            switch (_tickCount)
            {
                case 1:
                    UserInput.MouseMove(_animatedElement,20,20);  //Begin.
                    break;
                case 2:
                    if (_inputString == "PauseBefore")
                    {
                        //Pause the Animation before it has completed.
                        UserInput.MouseLeftClickCenter(_animatedElement);  //Pause.
                    }
                    break;
                case 6:
                    if (_inputString == "PauseAfter")
                    {
                        //Pause the Animation after it has completed.
                        UserInput.MouseLeftClickCenter(_animatedElement);  //Pause.
                    }
                    break;
                case 8:
                    //Take a picture of the screen after the Animation is done.
                    _afterCapture = CaptureTheScreen();
                    WaitForSignal("CaptureDone");
                    
                    Signal("AnimationDone", TestResult.Pass);
                    _aTimer.Stop();
                    break;
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
