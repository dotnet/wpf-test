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
    /// <area>Animation.Layers.Regressions</area>


    [Test(2, "Animation.Layers.Regressions", "TriggerLayersTest")]
    public class TriggerLayersTest : XamlTest
    {

        #region Test case members

        private Rectangle           _animatedElement;
        private Button              _focusElement;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        private Color               _actValuePrevious;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          TriggerLayersTest Constructor
        ******************************************************************************/
        public TriggerLayersTest() : base(@"TriggerLayers.xaml")
        {
            InitializeSteps += new TestStep(GetElement);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          GetElement
        ******************************************************************************/
        /// <summary>
        /// Retrieves the animated element from the markup, and then starts a Timer to control the
        /// timing of the verification.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult GetElement()
        {
            _animatedElement = (Rectangle)AnimationUtilities.FindElement(RootElement, "Rectangle1");
            _focusElement    = (Button)AnimationUtilities.FindElement(RootElement, "button2");
            
            if (_animatedElement == null)
            {
                GlobalLog.LogEvidence("The animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The animated element was found.");
                
                _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _aTimer.Tick += new EventHandler(OnTick);
                _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
                _aTimer.Start();
                GlobalLog.LogStatus("----DispatcherTimer Started----");
                
                return TestResult.Pass;
            }
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
                UserInput.MouseMove(_animatedElement,20,20);
            }
            else if (_tickCount == 2)
            {
                _actValuePrevious = GetActualFillColor(); //Get the color after the first animation.
                UserInput.MouseLeftClickCenter(_focusElement);
            }
            else if (_tickCount == 3)
            {
                UserInput.MouseMove(_animatedElement,20,20);
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

            SolidColorBrush brush = (SolidColorBrush)_animatedElement.GetValue(Shape.FillProperty);
            Color actValueFinal = GetActualFillColor();
            
            Color expValuePrevious = Colors.Red;
            Color expValueFinal = Colors.White;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value [1st Animation]: " + expValuePrevious);
            GlobalLog.LogEvidence("Actual Value   [1st Animation]: " + _actValuePrevious);
            GlobalLog.LogEvidence("Expected Value [2nd Animation]: " + expValueFinal);
            GlobalLog.LogEvidence("Actual Value   [2nd Animation]: " + actValueFinal);
            
            if (_actValuePrevious == expValuePrevious && actValueFinal == expValueFinal)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          GetActualFillColor
        ******************************************************************************/
        /// <summary>
        /// Determines the color of the animatedElement.
        /// </summary>
        /// <returns></returns>
        Color GetActualFillColor()
        {
            SolidColorBrush brush = (SolidColorBrush)_animatedElement.GetValue(Shape.FillProperty);
            Color color = Color.FromRgb(brush.Color.R, brush.Color.G, brush.Color.B);
            return color;
        }
        
        #endregion
    }
}
