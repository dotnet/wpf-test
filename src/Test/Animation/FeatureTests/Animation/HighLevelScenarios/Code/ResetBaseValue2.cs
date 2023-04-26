// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Display;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    [Test(2, "Animation.HighLevelScenarios.Regressions", "ResetBaseValue2Test")]
    public class ResetBaseValue2Test : WindowTest
    {
        #region Test case members

        private VisualVerifier      _verifier;
        private string              _inputString         = "";
        private TranslateTransform  _translateTransform;
        private AnimationClock      _clock;
        private string              _windowTitle         = "Animation";
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        [Variation("By", Priority=0)]
        [Variation("To")]
        [Variation("From")]
        [Variation("FromTo")]
        [Variation("FromBy")]
        
        /******************************************************************************
        * Function:          ResetBaseValue2Test Constructor
        ******************************************************************************/
        public ResetBaseValue2Test(string testValue)
        {
            _inputString    = testValue;
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
            Window.Width        = 500d;
            Window.Height       = 250d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.Title        = _windowTitle; 
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            Canvas body = new Canvas();
            Window.Content = body;
            body.Background = Brushes.Cornsilk;

            DockPanel dock1 = new DockPanel();
            body.Children.Add(dock1);
            dock1.Width         = 50d;
            dock1.Height        = 50d;
            dock1.Background    = Brushes.Purple;

            _translateTransform = new TranslateTransform();
            _translateTransform.X     = 100;
            _translateTransform.Y     = 0;

            dock1.RenderTransform = _translateTransform;
            
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,500);
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
            animTrans.BeginTime            = null;
            animTrans.Duration             = new Duration(TimeSpan.FromSeconds(1));

            //In all cases, the DO is initially set to animate to 200d.
            switch (_inputString)
            {
                case "By":
                    animTrans.By            = 100d;
                    break;
                case "To":
                    animTrans.To            = 200d;
                    break;
                case "From":
                    _translateTransform.X    = 200d;
                    animTrans.From          = 100d;
                    break;
                case "FromTo":
                    animTrans.From          = 100d;
                    animTrans.To            = 200d;
                    break;
                case "FromBy":
                    animTrans.From          = 100d;
                    animTrans.By            = 100;
                    break;
            }
            
            _clock = animTrans.CreateClock();
            _translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, _clock);
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
                _clock.Controller.Seek(TimeSpan.FromSeconds(2), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 2)
            {
                _translateTransform.X = 300d;  //Reset Base value.
            }
            else
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
           * Function:          CheckColor
           ******************************************************************************/
        /// <summary>
        /// CheckColor: retrieve the color at the specified point and compare it to the expected color.
        /// </summary>
        private bool CheckColor()
        {
            bool passed        = true;
            int x1             = 0;
            switch (_inputString)
            {
                case "By":
                    x1          = 425;  //Changing the base value should reset X.
                    break;
                case "To":
                    x1          = 225;  //Changing the base value should have no effect.
                    break;
                case "From":
                    x1          = 325;  //Changing the base value should reset X.
                    break;
                case "FromTo":
                    x1          = 225;  //Changing the base value should have no effect.
                    break;
                case "FromBy":
                    x1          = 225;  //Changing the base value should have no effect.
                    break;
            }
            int y1 = 25;
            
            x1 = x1 + 8;
            y1 = y1 + 8;
            //At each Tick, check the color at the specified point.

            // adjust for DPI
            x1 = (int)Monitor.ConvertLogicalToScreen(Dimension.Width, x1);

            Color actualColor = _verifier.getColorAtPoint(x1, y1);
            Color expectedColor = Colors.Purple;
            
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
