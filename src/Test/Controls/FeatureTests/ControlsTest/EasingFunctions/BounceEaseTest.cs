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
    /// BounceEaseOut Test
    /// </summary>
    [Test(0, "EasingFunctions", "BounceEaseTest", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class BounceEaseTest : EasingFunctionTestBase
    {        

        #region Constructors
        public BounceEaseTest()
            : base("BounceEase", "EaseInOut", "Button", "Height")
        {

        }
        //////////////////////////////////////////////////////////////////////////////////////        
        // DISABLEDUNSTABLETEST:
        // TestName:BounceEaseTest(EaseIn,Button,Width,3,2)
        // Area: Controls   SubArea: EasingFunctions
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: “findstr /snip DISABLEDUNSTABLETEST” 
        /////////////////////////////////////////////////////////////////////////////////////
        //[Variation("EaseIn", "Button", "Width", 3, 2)]
        [Variation("EaseOut", "Button", "Height", 4,3)]
        [Variation("EaseIn", "Label", "Width", 1,1)]
        [Variation("EaseOut", "Label", "Height", 2, 2)]
        [Variation("EaseInOut", "Label", "Width", 3, 1)]
        [Variation("EaseInOut", "Button", "Height", 4, 2)]        
        public BounceEaseTest(String EasingMode, String EasingControl, String EasingProperty, int bounces, double bounciness)
            : base("BounceEase", EasingMode, EasingControl, EasingProperty)
        {
            this.Bounces = bounces;
            this.Bounciness = bounciness;
        }
        #endregion

        int Bounces;
        double Bounciness;
        
        override public void SetEasingFunctions()
        {
            if (easingMode == "EaseOut")
            {
                progressAnimation.EasingFunction = new BounceEase() { Bounces = this.Bounces, Bounciness = this.Bounciness };
            }
            else if (easingMode == "EaseIn")
            {
                progressAnimation.EasingFunction = new BounceEase() { EasingMode = EasingMode.EaseIn, Bounces = this.Bounces, Bounciness = this.Bounciness };
            }
            else if (easingMode == "EaseInOut")
            {
                progressAnimation.EasingFunction = new BounceEase() { EasingMode = EasingMode.EaseInOut, Bounces = this.Bounces, Bounciness = this.Bounciness };
            }
        }


        override public double Calculate(double normalizedTime)
        {
            double bounces = Math.Max(0.0, (double)Bounces);
            if (Bounciness <= 1.0)
                Bounciness = 1.001;
            double pow = Math.Pow(Bounciness, bounces);
            double oneMinusBounciness = 1.0 - Bounciness;
            double sumOfUnits = (1.0 - pow) / oneMinusBounciness + pow * 0.5;
            double unitAtT = normalizedTime * sumOfUnits;
            double bounceAtT = Math.Log(-unitAtT * (1.0 - Bounciness) + 1.0, Bounciness);
            double start = Math.Floor(bounceAtT);
            double end = start + 1.0;
            double startTime = (1.0 - Math.Pow(Bounciness, start)) / (oneMinusBounciness * sumOfUnits);
            double endTime = (1.0 - Math.Pow(Bounciness, end)) / (oneMinusBounciness * sumOfUnits);
            double midTime = (startTime + endTime) * 0.5;
            double timeRelativeToPeak = normalizedTime - midTime;
            double radius = midTime - startTime;
            double amplitude = Math.Pow(1.0 / Bounciness, (bounces - start));
            return (-amplitude / (radius * radius)) * (timeRelativeToPeak - radius) * (timeRelativeToPeak + radius);
        }
    }
}
