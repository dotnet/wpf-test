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
    /// SineEaseOut Test
    /// </summary>
    [Test(0, "EasingFunctions", "SineEaseTest", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class SineEaseTest : EasingFunctionTestBase
    {
        #region Constructors        
        public SineEaseTest() : base("SineEase","EaseInOut", "Button", "Height")
        {
            
        }
        
        [Variation("EaseIn", "Button", "Height")]
        [Variation("EaseOut", "Label", "Width")]
        [Variation("EaseInOut", "Label", "Width")]        
        public SineEaseTest(String EasingMode, String EasingControl, String EasingProperty)
            : base("SineEase", EasingMode, EasingControl, EasingProperty)
        {
            
        }
        #endregion

        override public void SetEasingFunctions()
        {
            if (easingMode == "EaseOut")
            {
                progressAnimation.EasingFunction = new SineEase();
            }
            else if (easingMode == "EaseIn")
            {
                progressAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            }
            else if (easingMode == "EaseInOut")
            {
                progressAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
            }            
        }
        

        override public double Calculate(double normalizedTime)
        {
            return 1.0 - Math.Sin(Math.PI * 0.5 * (1 - normalizedTime));
        }
    }    
}
