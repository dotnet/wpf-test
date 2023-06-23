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
    /// CircleEaseOut Test
    /// </summary>
    [Test(0, "EasingFunctions", "CircleEaseTest", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class CircleEaseTest : EasingFunctionTestBase
    {
        #region Constructors
        public CircleEaseTest()
            : base("CircleEase", "EaseInOut", "Button", "Height")
        {

        }

        [Variation("EaseIn", "Button", "Height")]
        [Variation("EaseOut", "Label", "Width")]
        [Variation("EaseInOut", "Button", "Height")]        
        public CircleEaseTest(String EasingMode, String EasingControl, String EasingProperty)
            : base("CircleEase", EasingMode, EasingControl, EasingProperty)
        {

        }
        #endregion

        override public void SetEasingFunctions()
        {
            if (easingMode == "EaseOut")
            {
                progressAnimation.EasingFunction = new CircleEase();
            }
            else if (easingMode == "EaseIn")
            {
                progressAnimation.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn };
            }
            else if (easingMode == "EaseInOut")
            {
                progressAnimation.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut };
            }
        }


        override public double Calculate(double normalizedTime)
        {
            return 1.0 - Math.Sqrt(1.0 - normalizedTime * normalizedTime);
        }
    }
}
