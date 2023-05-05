// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Brush
{
    /// <summary/>
    public static class GradientBrushesGraphics_Verify
    {
        /// <summary>
        /// Verification routine for GradientBrushesGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            //Verifying image RadialGradienBrush
            GlobalLog.LogStatus("Verifying image RadialGradienBrush ...");
            FrameworkElement fe = rootElement as FrameworkElement;

            Path                ellipse1 = fe.FindName("Ellipse1") as Path;
            RadialGradientBrush rgBrush  = ((Shape) ellipse1).Fill as RadialGradientBrush;
            if (null == rgBrush)
            {
                GlobalLog.LogEvidence("null == rgBrush");
                result = false;
            }
            if (rgBrush.Opacity != 0.5)
            {
                GlobalLog.LogEvidence("rgBrush.Opacity != 0.5");
                result = false;
            }
            GradientStopCollection stops = rgBrush.GradientStops;

            if (stops.Count != 3)
            {
                GlobalLog.LogEvidence("stops.Count != 3");
                result = false;
            }
            GradientStop stop1 = stops[0];
            if (null == stop1)
            {
                GlobalLog.LogEvidence("null == stop1");
                result = false;
            }
            if (!Color.Equals(stop1.Color, Colors.Red))
            {
                GlobalLog.LogEvidence("!Color.Equals(stop1.Color, Colors.Red)");
                result = false;
            }
            if (stop1.Offset != 0)
            {
                GlobalLog.LogEvidence("stop1.Offset != 0");
                result = false;
            }

            GradientStop stop2 = stops[1];
            if (null == stop2)
            {
                GlobalLog.LogEvidence("null == stop2");
                result = false;
            }
            if (!Color.Equals(stop2.Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(stop2.Color, Colors.Yellow)");
                result = false;
            }
            if (stop2.Offset != 1)
            {
                GlobalLog.LogEvidence("stop2.Offset != 1");
                result = false;
            }

            GradientStop stop3 = stops[2];
            if (null == stop3)
            {
                GlobalLog.LogEvidence("null == stop3");
                result = false;
            }
            if (!Color.Equals(stop3.Color, Colors.Blue))
            {
                GlobalLog.LogEvidence("!Color.Equals(stop3.Color, Colors.Blue)");
                result = false;
            }
            if (stop3.Offset != 0.5)
            {
                GlobalLog.LogEvidence("stop3.Offset != 0.5");
                result = false;
            }

            GlobalLog.LogStatus("Verifying image LinearGradienBrush ...");

            Line                line1   = fe.FindName("Line1") as Line;
            LinearGradientBrush lgBrush = ((Shape) line1).Stroke as LinearGradientBrush;
            if (null == lgBrush)
            {
                GlobalLog.LogEvidence("null == lgBrush");
                result = false;
            }
            if (lgBrush.Opacity != 0.5)
            {
                GlobalLog.LogEvidence("lgBrush.Opacity != 0.5");
                result = false;
            }

            stops = lgBrush.GradientStops;
            if (null == stops)
            {
                GlobalLog.LogEvidence("null == stops");
                result = false;
            }

            if (stops.Count != 4)
            {
                GlobalLog.LogEvidence("stops.Count != 4");
                result = false;
            }

            stop1 = stops[0];
            if (null == stop1)
            {
                GlobalLog.LogEvidence("null == stop1");
                result = false;
            }
            if (!Color.Equals(stop1.Color, Colors.Green))
            {
                GlobalLog.LogEvidence("!Color.Equals(stop1.Color, Colors.Green)");
                result = false;
            }
            if (stop1.Offset != 0)
            {
                GlobalLog.LogEvidence("stop1.Offset != 0");
                result = false;
            }

            stop2 = stops[1];
            if (null == stop2)
            {
                GlobalLog.LogEvidence("null == stop2");
                result = false;
            }
            if (!Color.Equals(stop2.Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(stop2.Color, Colors.Yellow)");
                result = false;
            }
            if (stop2.Offset != 1)
            {
                GlobalLog.LogEvidence("stop2.Offset != 1");
                result = false;
            }

            stop3 = stops[2];
            if (null == stop3)
            {
                GlobalLog.LogEvidence("null == stop3");
                result = false;
            }
            if (!Color.Equals(stop3.Color, Colors.Purple))
            {
                GlobalLog.LogEvidence("!Color.Equals(stop3.Color, Colors.Purple)");
                result = false;
            }
            if (stop3.Offset != 0.5)
            {
                GlobalLog.LogEvidence("stop3.Offset != 0.5");
                result = false;
            }

            GradientStop stop4 = stops[3];
            if (null == stop4)
            {
                GlobalLog.LogEvidence("null == stop4");
                result = false;
            }
            if (!Color.Equals(stop4.Color, Colors.White))
            {
                GlobalLog.LogEvidence("!Color.Equals(stop4.Color, Colors.White)");
                result = false;
            }
            if (stop4.Offset != 0.2)
            {
                GlobalLog.LogEvidence("stop4.Offset != 0.2");
                result = false;
            }
            return result;
        }
    }
}
