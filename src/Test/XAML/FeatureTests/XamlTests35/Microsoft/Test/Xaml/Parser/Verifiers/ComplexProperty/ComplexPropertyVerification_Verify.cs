// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.ComplexProperty
{
    /// <summary/>
    public static class ComplexPropertyVerification_Verify
    {
        /// <summary>
        /// Verify complex properties.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            Button button0 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");
            Button button1 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");

            // Verify Button0 properties
            Color button0color = (button0.Background as SolidColorBrush).Color;
            if (!((button0color.A == 100) && (button0color.G == 255)))
            {
                GlobalLog.LogEvidence("!(Button0Color.A == 100) && (Button0Color.G == 255)");
                result = false;
            }

            // Verify Button1 properties
            GradientStopCollection stops = (button1.Background as LinearGradientBrush).GradientStops;
            if (stops.Count != 3)
            {
                GlobalLog.LogEvidence("Stops.Count != 3");
                result = false;
            }

            if ((stops[0] as GradientStop).Offset != 0.2)
            {
                GlobalLog.LogEvidence("(Stops[0] as GradientStop).Offset != 0.2");
                result = false;
            }
            if ((stops[1] as GradientStop).Offset != 0.4)
            {
                GlobalLog.LogEvidence("(Stops[1] as GradientStop).Offset != 0.4");
                result = false;
            }
            if ((stops[2] as GradientStop).Offset != 0.6)
            {
                GlobalLog.LogEvidence("(Stops[2] as GradientStop).Offset != 0.6");
                result = false;
            }

            return result;
        }
    }
}
