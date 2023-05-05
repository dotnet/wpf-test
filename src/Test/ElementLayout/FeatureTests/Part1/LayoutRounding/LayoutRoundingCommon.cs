// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Logging;

namespace Microsoft.Test.ElementLayout.FeatureTests.Part1
{   
    public class LayoutRoundingCommon
    {
        private const double allowablePrecisionDiff = 0.0000000000001;
        /// <summary>
        /// Determines if a length is properly rounded applying calculations for dpi if applicable.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsLengthRounded(double testValue, bool isWidth)
        {
            if (Microsoft.Test.Display.Monitor.Dpi.x != 96 || Microsoft.Test.Display.Monitor.Dpi.y != 96)
            {
                // See UIElement.cs  - RoundRect(Rect rect, UIElement element)
                // In LayoutRounding non Standard (96) Dpi ActualHeight/ActualWidth values are determined from the following formula:
                // Example is a Height == 45 on 120 dpi
                //  1. Length X dpi ratio = dpi adjusted value ---> 45 x 120/96 = 56.25
                //  2. dpi adjusted value is then rounded ---> 56.25 becomes 56.0
                //  3. Rounded length X inverse dpi ---> 56.0 x 96/120 = 44.8
                // So, to verify an ActualWidth/ActualHeight is valid under non standard dpi follow this:
                //  1. ActualLength  X  DpiValue/96
                if (isWidth)
                {
                    testValue = (testValue * Microsoft.Test.Display.Monitor.Dpi.x) / 96;
                }
                else
                {
                    testValue = (testValue * Microsoft.Test.Display.Monitor.Dpi.y) / 96;
                }
                
                testValue = ClearDoublePrecisionDiff(testValue);
            }
            
            if (testValue != Math.Truncate(testValue))            
            {               
                // Some non standard dpi calculations will produce a length just off, but close enough
                // Adjust by an allowable value and try verification again
                double adjustedHigh = testValue + allowablePrecisionDiff;
                double adjustedLow = testValue - allowablePrecisionDiff;
                if (adjustedHigh != Math.Truncate(adjustedHigh) && adjustedLow != Math.Truncate(adjustedLow))
                {
                    TestLog.Current.LogStatus("Rounding failure. adjustedHigh = {0} adjustedLow = {1}", adjustedHigh, adjustedLow);
                    TestLog.Current.LogStatus("testValue.Truncate = {0}", Math.Truncate(testValue).ToString());
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        // Because of the special dpi calculations, occasionally there will be precision differences that will mess up basic 
        // double comparison and evaluations like Math.Truncate
        // To work around this convert double to string and back to double.
        public static double ClearDoublePrecisionDiff(double doubleToClear)
        {
            string doubleString = doubleToClear.ToString();
            Double.TryParse(doubleString, out doubleToClear);
            return doubleToClear;
        }         
    }
}
