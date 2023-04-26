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
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent</area>
    /// <priority>2</priority>
    /// <description>
    /// API tests that don't fit into existing test frameworks.
    /// </description>
    /// </summary>
    [Test(2, "Animation.PropertyMethodEvent", "OddsAndEndsTest")]
    public class OddsAndEndsTest : WindowTest
    {
        #region Test case members

        private TextBlock                       _textblock1;
        private SolidColorBrush                 _SCB;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          OddsAndEndsTest Constructor
        ******************************************************************************/
        public OddsAndEndsTest()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Test1);
            RunSteps += new TestStep(Test2);
            RunSteps += new TestStep(Test3);
            RunSteps += new TestStep(Test4);
            RunSteps += new TestStep(Test5);
            RunSteps += new TestStep(Test6);
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
            Window.Height               = 100;
            Window.Width                = 150;

            Canvas body = new Canvas();
            body.Background = Brushes.DodgerBlue;
            body.Height     = 100d;
            body.Width      = 150d;
            
            _textblock1 = new TextBlock();
            body.Children.Add(_textblock1);
            _textblock1.Text = "Avalon!";
            Canvas.SetTop  (_textblock1, 10d);
            Canvas.SetLeft (_textblock1, 10d);   

            _SCB = new SolidColorBrush();
            _SCB.Color = Colors.White;
            _SCB.Opacity = 1d;
            _textblock1.Foreground = _SCB;

            Window.Content = body;

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          Test1
        ******************************************************************************/
        /// <summary>
        /// Test1: Call ShouldSerializeStoredWeakReference() on Animatable.
        /// </summary>
        /// <returns>Returns success if the test passes</returns>
        TestResult Test1()
        {
            bool actValue = Animatable.ShouldSerializeStoredWeakReference(_textblock1);
            bool expValue = false;
            
            GlobalLog.LogEvidence("--Test 1-- Actual Value:   " + actValue);
            GlobalLog.LogEvidence("--Test 1-- Expected Value: " + expValue);
            
            if (actValue == expValue)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          Test2
        ******************************************************************************/
        /// <summary>
        /// Test2: Call GetCurrentValue() on an AnimationTimeline.
        /// </summary>
        /// <returns>Returns success if the test passes</returns>
        TestResult Test2()
        {
            DoubleAnimation anim1 = new DoubleAnimation();
            anim1.To                = 0d;
            anim1.BeginTime         = TimeSpan.FromMilliseconds(0);
            anim1.Duration          = new Duration(TimeSpan.FromMilliseconds(100));

            AnimationTimeline AT = (AnimationTimeline)anim1;
            AnimationClock clock = AT.CreateClock();
            _SCB.ApplyAnimationClock(SolidColorBrush.OpacityProperty, clock);
            
            double actValue = (double)AT.GetCurrentValue(0d, 0d, clock);

            GlobalLog.LogEvidence("--Test 2-- Actual Value:   " + actValue);
            GlobalLog.LogEvidence("--Test 2-- Expected Value: < 1");
            
            if (actValue < 1)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          Test3
        ******************************************************************************/
        /// <summary>
        /// Test3: Apply op_Implicit to KeyTime.
        /// </summary>
        /// <returns>Returns success if the test passes</returns>
        TestResult Test3()
        {
            TimeSpan timeSpan1 = TimeSpan.FromSeconds(1);
            KeyTime keyTime3 = timeSpan1;
            
            TimeSpan actValue = keyTime3.TimeSpan;
            TimeSpan expValue = timeSpan1;
            
            GlobalLog.LogEvidence("--Test 3-- Actual Value:   " + actValue);
            GlobalLog.LogEvidence("--Test 3-- Expected Value: " + expValue);
            
            if (actValue == expValue)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          Test4
        ******************************************************************************/
        /// <summary>
        /// Test4: Apply op_Inequality to KeyTime.
        /// </summary>
        /// <returns>Returns success if the test passes</returns>
        TestResult Test4()
        {
            TimeSpan timeSpan2 = TimeSpan.FromSeconds(2);
            
            KeyTime keyTime4a = timeSpan2;
            KeyTime keyTime4b = new KeyTime();
            keyTime4b = KeyTime.FromTimeSpan(timeSpan2);
            
            bool actValue = (keyTime4a != keyTime4b);
            bool expValue = false;
            
            GlobalLog.LogEvidence("--Test 4-- Actual Value:   " + actValue);
            GlobalLog.LogEvidence("--Test 4-- Expected Value: " + expValue);
            
            if (actValue == expValue)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          Test5
        ******************************************************************************/
        /// <summary>
        /// Test5: Invoke InterpolateValue on a Rotation3DKeyFrame.
        /// </summary>
        /// <returns>Returns success if the test passes</returns>
        TestResult Test5()
        {
            QuaternionRotation3D quaternion1 = new QuaternionRotation3D(new Quaternion(3, 0, 2, 5));
            DiscreteRotation3DKeyFrame rotationKF = new DiscreteRotation3DKeyFrame();
            rotationKF.Value = quaternion1;
            
            QuaternionRotation3D quaternion2 = (QuaternionRotation3D)rotationKF.InterpolateValue(quaternion1, 1.0);
            
            Quaternion actValue = quaternion2.Quaternion;
            Quaternion expValue = quaternion1.Quaternion;
            
            GlobalLog.LogEvidence("--Test 5-- Actual Value:   " + actValue);
            GlobalLog.LogEvidence("--Test 5-- Expected Value: " + expValue);
            
            if (actValue == expValue)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          Test6
        ******************************************************************************/
        /// <summary>
        /// Test6: Apply set_Item to StringKeyFrameCollection.
        /// </summary>
        /// <returns>Returns success if the test passes</returns>
        TestResult Test6()
        {
            StringKeyFrameCollection stringKFC = new StringKeyFrameCollection();
            DiscreteStringKeyFrame stringKF1 = new DiscreteStringKeyFrame("Avalon",KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)));
            DiscreteStringKeyFrame stringKF2 = new DiscreteStringKeyFrame(".NET 3.0",KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)));
            stringKFC.Add(stringKF1);
            stringKFC[0] = stringKF2;

            string actValue = stringKFC[0].Value;
            string expValue = stringKF2.Value;

            GlobalLog.LogEvidence("--Test 6-- Actual Value:   " + actValue);
            GlobalLog.LogEvidence("--Test 6-- Expected Value: " + expValue);
            
            if (actValue == expValue)
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
