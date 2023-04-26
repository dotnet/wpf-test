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


    [Test(2, "Animation.LowLevelScenarios.Regressions", "NoBaseValueOnWidthTest")]
    public class NoBaseValueOnWidthTest : WindowTest
    {
        #region Test case members

        private string                          _inputString     = "";
        private Button                          _button1;
        private DoubleAnimation                 _animDouble;
        private double                          _fromValue       = 0d;
        private double                          _toValue         = 50d;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        [Variation("ContentButNoWidthSet")]
        [Variation("HeightOnlySet")]
        [Variation("NoHeightWidthSet")]
        
        /******************************************************************************
        * Function:          NoBaseValueOnWidthTest Constructor
        ******************************************************************************/
        public NoBaseValueOnWidthTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
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
            TestResult testResult = TestResult.Pass;

            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.MediumSeaGreen;
            body.Height     = 300d;
            body.Width      = 300d;

            _button1 = new Button();
            body.Children.Add(_button1);
            _button1.SetValue(Canvas.LeftProperty, 50d);
            _button1.SetValue(Canvas.TopProperty, 50d);

            switch (_inputString)
            {
                case "ContentButNoWidthSet" :
                    _button1.Content = "Avalon";
                    break;
                case "HeightOnlySet" :
                    _button1.Height = 50d;
                    break;
                case "NoHeightWidthSet" :
                    //Set neither Height nor Width on the Button.
                    break;
                default :
                    GlobalLog.LogEvidence("ERRROR!!! Incorrect input parameter.");
                    testResult = TestResult.Fail;
                    break;
            }
            
            Window.Content = body;
            
            return testResult;
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
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
            
            if (_tickCount == 1)
            {
                _animDouble = new DoubleAnimation();
                _animDouble.BeginTime      = TimeSpan.FromMilliseconds(0);
                _animDouble.Duration       = new Duration(TimeSpan.FromMilliseconds(250));
                _animDouble.From           = _fromValue;
                _animDouble.To             = _toValue;
                _button1.BeginAnimation(Button.WidthProperty, _animDouble);
            }
            else
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
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
            WaitForSignal("AnimationDone");

            double actValue = (double)_button1.GetValue(Button.WidthProperty);

            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value:       " + _toValue);
            GlobalLog.LogEvidence("Actual Value:         " + actValue);
            
            if (actValue == _toValue)
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
