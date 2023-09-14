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
using MS.Internal;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ElasticEaseOut Test
    /// </summary>
    [Test(0, "EasingFunctions", "ElasticEaseTest", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class ElasticEaseTest : EasingFunctionTestBase
    {

        #region Constructors
        public ElasticEaseTest()
            : base("ElasticEase", "EaseInOut", "Button", "Height")
        {

        }

        [Variation("EaseIn", "Label", "Width", 1, 1)]
        [Variation("EaseOut", "Label", "Height", 2, 1)]
        [Variation("EaseIn", "Button", "Height", 3, 2)]
        [Variation("EaseOut", "Button", "Width", 4, 3)]
        [Variation("EaseInOut", "Button", "Height", 1, 1)]        
        public ElasticEaseTest(String EasingMode, String EasingControl, String EasingProperty, int Oscillations, double Springiness)
            : base("ElasticEase", EasingMode, EasingControl, EasingProperty)
        {
            this.Oscillations = Oscillations;
            this.Springiness = Springiness;
        }
        #endregion

        int Oscillations;
        double Springiness;

        override public void SetEasingFunctions()
        {
            if (easingMode == "EaseOut")
            {
                progressAnimation.EasingFunction = new ElasticEase() { Oscillations = this.Oscillations, Springiness = this.Springiness };
            }
            else if (easingMode == "EaseIn")
            {
                progressAnimation.EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseIn, Oscillations = this.Oscillations, Springiness = this.Springiness };
            }
            else if (easingMode == "EaseInOut")
            {
                progressAnimation.EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseInOut, Oscillations = this.Oscillations, Springiness = this.Springiness };
            }
        }


        override public double Calculate(double normalizedTime)
        {
            double oscillations = Math.Max(0.0, (double)Oscillations);
            double springiness = Math.Max(0.0, Springiness);
            double expo;
            if (springiness == 0)
            {
                expo = normalizedTime;
            }
            else
            {
                expo = (Math.Exp(springiness * normalizedTime) - 1.0) / (Math.Exp(springiness) - 1.0);
            }

            return expo * (Math.Sin((Math.PI * 2.0 * oscillations + Math.PI * 0.5) * normalizedTime));
        }
    }
}
