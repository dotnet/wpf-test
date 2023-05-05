// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using System.Threading;
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

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.LowLevelScenarios.Regressions</area>
    /// <priority>2</priority>
    /// <description>
    /// Regression Test: "ColorAnimations doesn't animates the Alpha channel from fully opaque(#FF) to transparent(#00) on a SolidColorBrush"
    /// </description>
    /// </summary>
    [Test(2, "Animation.LowLevelScenarios.Regressions", "AlphaChannelTest")]
    public class AlphaChannelTest : WindowTest
    {

        #region Test case members

        private string                  _inputString = "";
        private DispatcherTimer         _aTimer      = null;
        private Ellipse                 _ellipse1;
        private SolidColorBrush         _brush;
        private Byte                    _trans       = Byte.Parse("00", NumberStyles.HexNumber);
        private Byte                    _white       = Byte.Parse("FF", NumberStyles.HexNumber);
        private Color                   _fromValue;
        private Color                   _toValue;
        
        #endregion


        #region Constructor
        
        [Variation("From00ToFF")]
        [Variation("FromFFTo00")]

        /******************************************************************************
        * Function:          AlphaChannelTest Constructor
        ******************************************************************************/
        public AlphaChannelTest(string testValue)
        {
            _inputString = testValue;
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
            TestResult testResult = TestResult.Pass;

            Window.Left                 = 0d;
            Window.Top                  = 0d;
            Window.Height               = 100;
            Window.Width                = 100;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            Canvas body  = new Canvas();
            body.Background = Brushes.LightSeaGreen;

            _ellipse1 = new Ellipse();
            _ellipse1.Fill       = _brush;
            _ellipse1.Width      = 50d;
            _ellipse1.Height     = 50d;

            body.Children.Add(_ellipse1);
            
            Window.Content = body;
            
            //Set Animation to/from values, depending on the input parameter.
            switch (_inputString)
            {
                case ("From00ToFF") :
                    _fromValue = Color.FromArgb(_trans, _white, _white, _white);
                    _toValue   = Color.FromArgb(_white, _white, _white, _white);
                    break;
                case ("FromFFTo00") :
                    _fromValue = Color.FromArgb(_white, _white, _white, _white);
                    _toValue   = Color.FromArgb(_trans, _white, _white, _white);
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! Incorrect input test parameter");
                    testResult = TestResult.Fail;
                    break;
            }

            return testResult;
        }
          
        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// Starting the Animation here.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----OnContentRendered----");

            ColorAnimation anim = new ColorAnimation(_fromValue, _toValue, TimeSpan.FromMilliseconds(1000));
            anim.BeginTime = TimeSpan.FromMilliseconds(0);

            _brush = new SolidColorBrush();
            _brush.Color = Colors.Blue;
            _ellipse1.Fill = _brush;
            _brush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
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
            Signal("AnimationDone", TestResult.Pass);
            _aTimer.Stop();
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

            Color actValue = Color.FromArgb(_brush.Color.A, _brush.Color.R, _brush.Color.G, _brush.Color.B);
            Color expValue = _toValue;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if ( actValue == expValue ) 
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
