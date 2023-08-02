using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Animation;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// QuarticEaseOut Test
    /// </summary>
    [Test(0, "EasingFunctions", "QuarticEaseTest", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class QuarticEaseTest : EasingFunctionTestBase
    {
        #region Constructors
        public QuarticEaseTest()
            : base("QuarticEase", "EaseInOut", "Button", "Height")
        {

        }

        [Variation("EaseIn", "Button", "Width")]
        [Variation("EaseOut", "Label", "Height")]
        [Variation("EaseInOut", "Button", "Height")]        
        public QuarticEaseTest(String EasingMode, String EasingControl, String EasingProperty)
            : base("QuarticEase", EasingMode, EasingControl, EasingProperty)
        {

        }
        #endregion

        override public void SetEasingFunctions()
        {
            if (easingMode == "EaseOut")
            {
                progressAnimation.EasingFunction = new QuarticEase();
            }
            else if (easingMode == "EaseIn")
            {
                progressAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseIn };
            }
            else if (easingMode == "EaseInOut")
            {
                progressAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseInOut };
            }
        }


        override public double Calculate(double normalizedTime)
        {
            return normalizedTime * normalizedTime * normalizedTime * normalizedTime;
        }
    }
}
