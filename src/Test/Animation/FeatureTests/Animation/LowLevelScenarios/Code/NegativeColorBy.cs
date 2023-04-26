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
using System.Windows.Documents;
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

    // [DISABLE WHILE PORTING]
    //[Test(2, "Animation.LowLevelScenarios.Regressions", "NegativeColorByTest")]
    public class NegativeColorByTest : WindowTest
    {

        #region Test case members

        private DispatcherTimer         _aTimer      = null;
        private Paragraph               _paragraph1  = null;
        private SolidColorBrush         _brush;
        private Color                   _baseValue   = Color.FromScRgb(1f, 1f, 1f, 1f);
        private Color                   _byValue     = Color.FromScRgb(1f, -0.5f, -0.5f, -0.5f);
        private Color                   _toValue     = Color.FromScRgb(2f, 0.5f, 0.5f, 0.5f);
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          NegativeColorByTest Constructor
        ******************************************************************************/
        public NegativeColorByTest()
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
            Window.Height               = 300;
            Window.Width                = 400;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            Canvas body  = new Canvas();
            body.Background = Brushes.DarkGray;

            FlowDocumentReader flowDocReader1 = new FlowDocumentReader();
            flowDocReader1.Height        = 200d;
            flowDocReader1.Width         = 300d;
            flowDocReader1.Background    = Brushes.SpringGreen;
            Canvas.SetTop  (flowDocReader1, 50d);
            Canvas.SetLeft (flowDocReader1, 50d);

            body.Children.Add(flowDocReader1);

            FlowDocument flowDocument1 = new FlowDocument();
            flowDocument1.Background        = Brushes.DodgerBlue;
            flowDocReader1.Document         = flowDocument1;

            _paragraph1 = new Paragraph(new Run("Windows Presentation Foundation"));
            _paragraph1.BorderThickness = new Thickness(5d);
            flowDocument1.Blocks.Add(_paragraph1);

            Window.Content = body;

            return TestResult.Pass;
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

            ColorAnimation anim = new ColorAnimation();
            anim.By         = _byValue;
            anim.BeginTime  = TimeSpan.FromMilliseconds(0);
            anim.Duration   = new Duration(TimeSpan.FromSeconds(2));

            _brush = new SolidColorBrush();
            _brush.Color = _baseValue;
            _paragraph1.BorderBrush = _brush;
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,5000);
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

            Color actValue = Color.FromScRgb(_brush.Color.ScA, _brush.Color.ScR, _brush.Color.ScG, _brush.Color.ScB);
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
