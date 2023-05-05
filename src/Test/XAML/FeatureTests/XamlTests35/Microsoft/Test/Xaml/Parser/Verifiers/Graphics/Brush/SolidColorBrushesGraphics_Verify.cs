// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Test.Logging;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Brush
{
    /// <summary/>
    public static class SolidColorBrushesGraphics_Verify
    {
        /// <summary>
        /// Verification routine for SolidColorBrushesGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Verifying SolidColorBrush ...");

            Polygon polygon1 = (Polygon) LogicalTreeHelper.FindLogicalNode(rootElement, "Polygon1");

            if (null == polygon1)
            {
                GlobalLog.LogEvidence("null == polygon1");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Shape) polygon1).Stroke)).Color, Colors.Black))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Shape)polygon1).Stroke)).Color, Colors.Black)");
                result = false;
            }
            if (((Shape) polygon1).StrokeThickness != 2)
            {
                GlobalLog.LogEvidence("((Shape)polygon1).StrokeThickness != 2");
                result = false;
            }
            if (polygon1.Points.Count != 6)
            {
                GlobalLog.LogEvidence("polygon1.Points.Count != 6");
                result = false;
            }
            if (!polygon1.Points.Contains(new Point(400, 10)))
            {
                GlobalLog.LogEvidence("!polygon1.Points.Contains(new Point(400, 10))");
                result = false;
            }
            if (!polygon1.Points.Contains(new Point(450, 35)))
            {
                GlobalLog.LogEvidence("!polygon1.Points.Contains(new Point(450, 35))");
                result = false;
            }
            if (!polygon1.Points.Contains(new Point(450, 65)))
            {
                GlobalLog.LogEvidence("!polygon1.Points.Contains(new Point(450, 65))");
                result = false;
            }
            if (!polygon1.Points.Contains(new Point(400, 90)))
            {
                GlobalLog.LogEvidence("!polygon1.Points.Contains(new Point(400, 90))");
                result = false;
            }
            if (!polygon1.Points.Contains(new Point(350, 65)))
            {
                GlobalLog.LogEvidence("!polygon1.Points.Contains(new Point(350, 65))");
                result = false;
            }
            if (!polygon1.Points.Contains(new Point(350, 35)))
            {
                GlobalLog.LogEvidence("!polygon1.Points.Contains(new Point(350, 35))");
                result = false;
            }

            SolidColorBrush solidColorBrush = ((Shape) polygon1).Fill as SolidColorBrush;

            if (null == solidColorBrush)
            {
                GlobalLog.LogEvidence("null == solidColorBrush");
                result = false;
            }
            if (!Color.Equals(solidColorBrush.Color, Colors.Green))
            {
                GlobalLog.LogEvidence("!Color.Equals(solidColorBrush.Color, Colors.Green)");
                result = false;
            }
            if (((System.Windows.Media.Brush) solidColorBrush).Opacity != 0.7)
            {
                GlobalLog.LogEvidence("((Brush)solidColorBrush).Opacity != 0.7");
                result = false;
            }
            return result;
        }
    }
}
