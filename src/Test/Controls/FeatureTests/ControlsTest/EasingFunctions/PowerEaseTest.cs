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
    /// PowerEaseOut Test
    /// </summary>
    [Test(0, "EasingFunctions", "PowerEaseTest", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class PowerEaseTest : EasingFunctionTestBase
    {
        #region Constructors
        public PowerEaseTest()
            : base("PowerEase", "EaseInOut", "Button", "Height")
        {

        }

        [Variation("EaseIn", "Label", "Height", 2)]
        [Variation("EaseOut", "Label", "Width", 2.5)]
        [Variation("EaseIn", "Button", "Width", 4)]
        [Variation("EaseOut", "Button", "Height", 5)]
        [Variation("EaseInOut", "Label", "Width",1.5)]        
        public PowerEaseTest(String EasingMode, String EasingControl, String EasingProperty, double power)
            : base("PowerEase", EasingMode, EasingControl, EasingProperty)
        {
            this.power = power;
        }
        #endregion

        double power;

        override public void SetEasingFunctions()
        {
            if (easingMode == "EaseOut")
            {
                progressAnimation.EasingFunction = new PowerEase() { Power = power };
            }
            else if (easingMode == "EaseIn")
            {
                progressAnimation.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseIn, Power = power };
            }
            else if (easingMode == "EaseInOut")
            {
                progressAnimation.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = power };
            }
        }


        override public double Calculate(double normalizedTime)
        {
            power = Math.Max(0.0, power);
            return Math.Pow(normalizedTime, power);
        }        
    }
}
