// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
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


    [Test(2, "Animation.ManualTestCase.Regressions", "WindowTitleTest")]
    public class WindowTitleTest : WindowTest
    {
        #region Test case members

        private string _toValue  = "C";
        
        #endregion

        #region Constructor
        
        /******************************************************************************
        * Function:          WindowTitleTest Constructor
        ******************************************************************************/
        public WindowTitleTest()
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
        /// Creates the Window content.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            Window.Width            = 600d;
            Window.Height           = 300d;
            Window.Left             = 0d;
            Window.Top              = 0d;
            Window.Title            = "FOO";
            Window.ShowInTaskbar    = true;

            Canvas body = new Canvas();
            body.Background = Brushes.MidnightBlue;

            TextBlock textblock1 = new TextBlock();
            body.Children.Add(textblock1);
            textblock1.Foreground   = Brushes.White;
            textblock1.FontSize     = 24d;
            textblock1.Text         = "***MANUAL TEST CASE***\n\nCheck the Taskbar for the animation.";

            Window.Content = body;

            StringAnimationUsingKeyFrames animString = new StringAnimationUsingKeyFrames();
            StringKeyFrameCollection SKFC = new StringKeyFrameCollection();
            SKFC.Add(new DiscreteStringKeyFrame("A",KeyTime.FromPercent(0f)));
            SKFC.Add(new DiscreteStringKeyFrame("B",KeyTime.FromPercent(0.5f)));
            SKFC.Add(new DiscreteStringKeyFrame(_toValue, KeyTime.FromPercent(0.9f)));
            animString.KeyFrames = SKFC;
            animString.BeginTime        = TimeSpan.FromSeconds(0);
            animString.Duration         = new Duration(TimeSpan.FromSeconds(2));
            animString.RepeatBehavior   = new RepeatBehavior(8d);
            animString.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);

            Window.BeginAnimation(Window.TitleProperty, animString);
            
            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----CurrentStateInvalidated----" + ((Clock)sender).CurrentState);

            if (((Clock)sender).CurrentState == ClockState.Filling)
            {
                Signal("AnimationDone", TestResult.Pass);
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

            string actValue = (string)Window.GetValue(Window.TitleProperty);
            string expValue = _toValue;
            
            GlobalLog.LogEvidence("--------RESULTS--------");
            GlobalLog.LogEvidence("Act Value:   " + actValue);
            GlobalLog.LogEvidence("Exp Value:   " + expValue);

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
