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
using System.Windows.Shapes;
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
    // [Test(2, "Animation.HighLevelScenarios.Regressions", "SeekDuringPauseTest")]
    public class SeekDuringPauseTest : WindowTest
    {
        #region Test case members

        private VisualVerifier      _verifier;
        private Border              _borderCA;
        private Button              _buttonBegin;
        private Button              _buttonPause;
        private Button              _buttonSeek;
        private AnimationClock      _clock;
        private string              _windowTitle         = "Animation";
        private Color               _borderColor         = Colors.BlueViolet;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          SeekDuringPauseTest Constructor
        ******************************************************************************/
        public SeekDuringPauseTest()
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
            Window.Width        = 600d;
            Window.Height       = 400d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.Title        = _windowTitle; 
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            Canvas body  = new Canvas();
            Border borderBody = new Border();
            borderBody.Width            = 600d;
            borderBody.Height           = 400d;
            borderBody.Background       = new SolidColorBrush(Colors.Moccasin);
            body.Children.Add(borderBody);

            _buttonBegin = new Button();
            body.Children.Add(_buttonBegin);
            _buttonBegin.Content = "Begin";
            _buttonBegin.Width           = 90d;
            _buttonBegin.Height          = 40d;
            _buttonBegin.Background  = new SolidColorBrush(Colors.LightGreen);
            Canvas.SetTop  (_buttonBegin, 50d);
            Canvas.SetLeft (_buttonBegin, 20d);
            _buttonBegin.Click += new RoutedEventHandler(OnBegin); 

            _buttonPause = new Button();
            body.Children.Add(_buttonPause);
            _buttonPause.Content = "Pause";
            _buttonPause.Width           = 90d;
            _buttonPause.Height          = 40d;
            _buttonPause.Background  = new SolidColorBrush(Colors.LightBlue);
            Canvas.SetTop  (_buttonPause, 50d);
            Canvas.SetLeft (_buttonPause, 120d);
            _buttonPause.Click += new RoutedEventHandler(OnPause); 

            _buttonSeek = new Button();
            body.Children.Add(_buttonSeek);
            _buttonSeek.Content = "Seek";
            _buttonSeek.Width            = 90d;
            _buttonSeek.Height           = 40d;
            _buttonSeek.Background   = new SolidColorBrush(Colors.MistyRose);
            Canvas.SetTop  (_buttonSeek, 50d);
            Canvas.SetLeft (_buttonSeek, 220d);
            _buttonSeek.Click += new RoutedEventHandler(OnSeek); 

            Canvas CA  = new Canvas();
            body.Children.Add(CA);
            CA.Width        = 111d;
            CA.Height       = 111d;

            _borderCA        = new Border();
            _borderCA.Width          = 111d;
            _borderCA.Height         = 111d;
            _borderCA.Background     = new SolidColorBrush(_borderColor);
            Canvas.SetTop  (_borderCA, 180d);
            Canvas.SetLeft (_borderCA, 50d);
            CA.Children.Add(_borderCA);

            Window.Content = body;

            return TestResult.Pass;
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
            StartAnimation();

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromMilliseconds(500);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");
        }

        /******************************************************************************
        * Function:          StartAnimation
        ******************************************************************************/
        /// <summary>
        /// Create and begin the Animation.
        /// </summary>
        /// <returns></returns>
        private void StartAnimation()
        {
            DoubleAnimation animTrans = new DoubleAnimation();                                             
            animTrans.BeginTime         = null;
            animTrans.Duration          = new Duration(TimeSpan.FromMilliseconds(8000));
            animTrans.From              = 111d;
            animTrans.To                = 333d;
            
            _clock = animTrans.CreateClock();
            _borderCA.ApplyAnimationClock(Border.WidthProperty, _clock);
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
                UserInput.MouseLeftClickCenter(_buttonBegin);
            }
            else if (_tickCount == 3)
            {
                UserInput.MouseLeftClickCenter(_buttonPause);
            }
            else if (_tickCount == 5)
            {
                //This Seek will not resume the Animation; calling Resume() is necessary;
                UserInput.MouseLeftClickCenter(_buttonSeek);
            }
            else if (_tickCount == 7)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();

                bool testPassed = CheckColor();
               
                if (testPassed)
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
           * Function:          OnBegin
           ******************************************************************************/
        private void OnBegin(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----Begin----");
            _clock.Controller.Begin();
        }

        /******************************************************************************
           * Function:          OnSeek
           ******************************************************************************/
        private void OnSeek(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----Seek----");
            _clock.Controller.Seek(TimeSpan.FromMilliseconds(2000), TimeSeekOrigin.BeginTime);
        }

        /******************************************************************************
           * Function:          OnPause
           ******************************************************************************/
        private void OnPause(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----Pause----");
            _clock.Controller.Pause();
        }

        /******************************************************************************
           * Function:          CheckColor
           ******************************************************************************/
        /// <summary>
        /// CheckColor: retrieve the color at the specified point and compare it to the expected color.
        /// </summary>
        private bool CheckColor()
        {
            bool passed         = true;
            int x1              = 191;
            int y1              = 231;
            
            //At each Tick, check the color at the specified point.
            Color actualColor = _verifier.getColorAtPoint(x1, y1);
            Color expectedColor = _borderColor;
            
            float tolerance  = 0.001f;

            passed = AnimationUtilities.CompareColors(expectedColor, actualColor, tolerance);

            GlobalLog.LogEvidence("---------- Result at (" + x1 + "," + y1 + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actualColor.ToString());
            GlobalLog.LogEvidence(" Expected : " + expectedColor.ToString());

            return passed;
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
