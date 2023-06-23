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
    /// BackEaseOut Test
    /// </summary>
    [Test(0, "EasingFunctions", "BackEaseTest", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class BackEaseTest : EasingFunctionTestBase
    {
        Double amplitude;

        #region Constructors
        public BackEaseTest()
            : base("BackEase", "EaseInOut", "Button", "Height")
        {

        }

        [Variation("EaseIn", "Button", "Width", 0)]
        [Variation("EaseIn","Button","Height",1)]
        [Variation("EaseOut", "Label", "Height",3)]
        [Variation("EaseOut", "Label", "Height", 5)]
        [Variation("EaseInOut","Button","Height",0)]
        // Disabled due to instability on test automation runs.  
        [Variation("EaseInOut", "Label", "Width", 4, Disabled=true)]
        public BackEaseTest(String EasingMode, String EasingControl, String EasingProperty, Double amplitude)
            : base("BackEase", EasingMode, EasingControl, EasingProperty)
        {
            this.amplitude = amplitude;
        }
        #endregion

        override public void SetEasingFunctions()
        {
            if (easingMode == "EaseOut")
            {
                progressAnimation.EasingFunction = new BackEase() { Amplitude = amplitude };
            }
            else if (easingMode == "EaseIn")
            {
                progressAnimation.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseIn, Amplitude = amplitude };
            }
            else if (easingMode == "EaseInOut")
            {
                progressAnimation.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut, Amplitude = amplitude };
            }
        }


        override public double Calculate(double normalizedTime)
        {
            amplitude = Math.Max(0.0, amplitude);
            return Math.Pow(normalizedTime, 3.0) - normalizedTime * amplitude * Math.Sin(Math.PI * normalizedTime);
        }
    }
}
