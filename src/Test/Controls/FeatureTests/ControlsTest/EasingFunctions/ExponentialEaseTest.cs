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
    /// ExponentialEaseOut Test
    /// </summary>
    [Test(0, "EasingFunctions", "ExponentialEaseTest", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class ExponentialEaseTest : EasingFunctionTestBase
    {
        Double exponent;

        #region Constructors
        public ExponentialEaseTest()
            : base("ExponentialEase", "EaseInOut", "Button", "Height")
        {

        }

        [Variation("EaseIn", "Button", "Width", 1)]
        [Variation("EaseOut", "Button", "Height", 2)]
        [Variation("EaseIn", "Label", "Height", 3)]
        [Variation("EaseOut", "Label", "Width", 4)]
        [Variation("EaseInOut", "Button", "Height", 5)]
        [Variation("EaseInOut", "Label", "Height", 6)]        
        public ExponentialEaseTest(String EasingMode, String EasingControl, String EasingProperty, Double exponent)
            : base("ExponentialEase", EasingMode, EasingControl, EasingProperty)
        {
            this.exponent = exponent;
        }
        #endregion

        override public void SetEasingFunctions()
        {
            if (easingMode == "EaseOut")
            {
                progressAnimation.EasingFunction = new ExponentialEase() { Exponent = exponent };
            }
            else if (easingMode == "EaseIn")
            {
                progressAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn, Exponent = exponent };
            }
            else if (easingMode == "EaseInOut")
            {
                progressAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut, Exponent = exponent };
            }
        }


        override public double Calculate(double normalizedTime)
        {
            if (exponent == 0.0)
            {
                return normalizedTime;
            }
            else
            {
                return (Math.Exp(exponent * normalizedTime) - 1.0) / (Math.Exp(exponent) - 1.0);
            }
        }
    }
}
