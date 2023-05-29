// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.CoreInput.Common;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// All property engine verifiers in this class
    /// </summary>
    public static partial class Verifiers
    {
        private static int s_verifyStep = 0;
        private static int GetVerificationStep(int totalSteps)
        {
            Debug.Assert(s_verifyStep < totalSteps, "Internal state error.");
            int getStep = ++s_verifyStep;
            if (s_verifyStep == totalSteps)
            {
                s_verifyStep = 0;
            }
            CoreLogger.LogStatus("Step " + getStep.ToString());
            return getStep;
        }

        /// <summary>
        /// Commly used routine that validate Button.Background (of up to 3 buttons)
        /// against the expected Brush value
        /// </summary>
        /// <param name="btn1">The first button to validate. null to ignore</param>
        /// <param name="brush1">The expected Background for the first button</param>
        /// <param name="btn2">The second button to validate. null to ignore</param>
        /// <param name="brush2">The expected Background for the second button</param>
        /// <param name="btn3">The third button to validate. null to ignore</param>
        /// <param name="brush3">The expected Background for the third button</param>
        private static void ValidateThreeButtonBackground(Button btn1, Brush brush1, Button btn2, Brush brush2, Button btn3, Brush brush3)
        {
            ValidateButtonBackgroundAssert(btn1, brush1);
            ValidateButtonBackgroundAssert(btn2, brush2);
            ValidateButtonBackgroundAssert(btn3, brush3);
        }

        private static void ValidateThreeButtonWidth(Button btn1, double width1, Button btn2, double width2, Button btn3, double width3)
        {
            Debug.Assert(btn1.Width == width1, "Btn1.Width as expected");
            Debug.Assert(btn2.Width == width2, "Btn2.Width as expected");
            Debug.Assert(btn3.Width == width3, "Btn3.Width as expected");
        }

        private static void ValidateThreeButtonHeight(Button btn1, double height1, Button btn2, double height2, Button btn3, double height3)
        {
            Debug.Assert(btn1.Height == height1, "Btn1.Height as expected");
            Debug.Assert(btn2.Height == height2, "Btn2.Height as expected");
            Debug.Assert(btn3.Height == height3, "Btn3.Height as expected");
        }

        /// <summary>
        /// Provide more logging information
        /// </summary>
        /// <param name="btn">The button to validate</param>
        /// <param name="expectedBackground"></param>
        private static void ValidateButtonBackgroundAssert(Button btn, Brush expectedBackground)
        {
            if (btn != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Button.Background of ");
                if (!String.IsNullOrEmpty(btn.Name))
                {
                    sb.Append(btn.Name);
                }
                else
                {
                    sb.Append("{unnamed)");
                }
                sb.Append(" is");
                if (expectedBackground == s_brushLightBlue)
                {
                    sb.Append(" LightBlue.");
                }
                else if (expectedBackground == s_brushLightGreen)
                {
                    sb.Append(" LightGreen.");
                }
                else if (expectedBackground == s_brushIndigo)
                {
                    sb.Append(" Indigo.");
                }
                else if (expectedBackground == s_brushRed)
                {
                    sb.Append(" Red.");
                }
                else if (expectedBackground == s_brushBlue)
                {
                    sb.Append(" Blue.");
                }
                else if (expectedBackground == s_brushGreen)
                {
                    sb.Append(" Green.");
                }
                else
                {
                    Debug.Fail("Please update ValidateButtonBackgroundAssert");
                    sb.Append(" as expected.");
                }

                //Supports SolidColorBrush only, for easy comparison
                Debug.Assert(btn.Background is SolidColorBrush, "Supports SolidColorBrush only in ValidateThreeButtonBackgroundAssert.");
                Debug.Assert(expectedBackground is SolidColorBrush, "Supports SolidColorBrush only in ValidateThreeButtonBackgroundAssert.");
                CoreLogger.LogStatus("Expected Color: " + ((SolidColorBrush)expectedBackground).Color.ToString());
                CoreLogger.LogStatus("Actual Color: " + ((SolidColorBrush)btn.Background).Color.ToString());
                Debug.Assert(((SolidColorBrush)btn.Background).Color == ((SolidColorBrush)expectedBackground).Color, sb.ToString());
            }
        }

        private static SolidColorBrush s_brushLightGreen = Brushes.LightGreen;
        private static SolidColorBrush s_brushLightBlue = Brushes.LightBlue;
        private static SolidColorBrush s_brushIndigo = Brushes.Indigo;
        private static SolidColorBrush s_brushRed = Brushes.Red;
        private static SolidColorBrush s_brushBlue = Brushes.Blue;
        private static SolidColorBrush s_brushGreen = Brushes.Green;
    }
}
