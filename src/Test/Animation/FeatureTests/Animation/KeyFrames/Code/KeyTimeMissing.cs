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
    /// <area>Animation.KeyFrames.Regressions</area>


    [Test(2, "Animation.KeyFrames.Regressions", "KeyTimeMissingTest")]
    public class KeyTimeMissingTest : WindowTest
    {
        #region Test case members

        private VisualVerifier                  _verifier;
        private string                          _inputString         = "";
        private ListBox                         _listbox1            = null;
        private ObjectAnimationUsingKeyFrames   _animKeyFrame        = null;
        private Color                           _listboxBackground   = Colors.SpringGreen;
        private DispatcherTimer                 _aTimer              = null;
        
        #endregion


        #region Constructor

        [Variation("TimeMissing")]
        [Variation("ValueMissing")]
        
        /******************************************************************************
        * Function:          KeyTimeMissingTest Constructor
        ******************************************************************************/
        public KeyTimeMissingTest(string testValue)
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
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.Title        = "Animation"; 

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            Canvas body = new Canvas();
            body.Background = new SolidColorBrush(_listboxBackground);
            body.Height     = 300d;
            body.Width      = 300d;
            
            _listbox1 = new ListBox();
            body.Children.Add(_listbox1);
            _listbox1.SelectionMode      = SelectionMode.Multiple;
            _listbox1.Visibility         = Visibility.Visible;
            _listbox1.Height             = 250d;
            _listbox1.Width              = 150d;
            _listbox1.FontSize           = 18d;
            
            Visibility initialEnum = Visibility.Collapsed;
//            Visibility finalEnum   = Visibility.Visible;

            KeyTime keyTime1 = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1));
            KeyTime keyTime2 = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2));

            DiscreteObjectKeyFrame keyFrame1 = new DiscreteObjectKeyFrame();
            keyFrame1.Value = initialEnum;
            keyFrame1.KeyTime = keyTime1;

            DiscreteObjectKeyFrame keyFrame2 = new DiscreteObjectKeyFrame();
            if (_inputString == "TimeMissing")
            {
                //Set Value, but not KeyTime.
                keyFrame2.Value = initialEnum;
            }
            else
            {
                //Set KeyTime, but not Value.  This will throw an exception when the animation starts.
                keyFrame2.KeyTime = keyTime2;
            }

            _animKeyFrame = new ObjectAnimationUsingKeyFrames();
            ObjectKeyFrameCollection OKFC = new ObjectKeyFrameCollection();
            OKFC.Add(keyFrame1);
            OKFC.Add(keyFrame2);
            _animKeyFrame.KeyFrames = OKFC;

            Window.Content = body;

            if (_inputString == "TimeMissing")
            {
                _listbox1.BeginAnimation(ListBox.VisibilityProperty, _animKeyFrame);
            }

            return TestResult.Pass;
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,4000);
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
            if (_inputString == "ValueMissing")
            {
                SetExpectedErrorTypeInStep(typeof(InvalidOperationException), "Inner");                
                _listbox1.BeginAnimation(ListBox.VisibilityProperty, _animKeyFrame);

                WaitForSignal("AnimationDone");
                return TestResult.Pass;
            }
            else
            {
                WaitForSignal("AnimationDone");

                float tolerance = 0.10f;
                int x           = 125;
                int y           = 75;

                Color actColor = _verifier.getColorAtPoint(x,y);

                Color expColor = _listboxBackground;
                bool testPassed = AnimationUtilities.CompareColors(expColor, actColor, tolerance);

                GlobalLog.LogEvidence("---------- Result at (" + x + "," + y + ") ------");
                GlobalLog.LogEvidence(" Actual   : " + actColor.ToString());
                GlobalLog.LogEvidence(" Expected : " + expColor.ToString());

                if (testPassed)
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
        }

        #endregion
    }
}
