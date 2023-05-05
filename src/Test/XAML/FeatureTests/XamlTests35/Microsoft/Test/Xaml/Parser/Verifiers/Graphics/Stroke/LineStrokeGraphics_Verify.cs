// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using Microsoft.Test.Logging;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Stroke
{
    /// <summary/>
    public static class LineStrokeGraphics_Verify
    {
        /// <summary>
        /// Verification routine for LineStrokeGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("Path Stroke Verify ...");

            Line line1 = (Line) LogicalTreeHelper.FindLogicalNode(myPanel, "Line1");
            VerifyElement.VerifyBool(null == line1, false, ref result);
            VerifyElement.VerifyDouble(line1.X1, 40, ref result);
            VerifyElement.VerifyDouble(line1.Y1, 250, ref result);
            VerifyElement.VerifyDouble(line1.X2, 250, ref result);
            VerifyElement.VerifyDouble(line1.Y2, 250, ref result);

            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) line1).Stroke)).Color, Colors.Red, ref result);
            VerifyElement.VerifyInt((int)((Shape) line1).StrokeStartLineCap, (int) PenLineCap.Flat, ref result);
            VerifyElement.VerifyInt((int)((Shape) line1).StrokeEndLineCap, (int) PenLineCap.Flat, ref result);
            VerifyElement.VerifyDouble(((Shape) line1).StrokeThickness, 45, ref result);

            Line line2 = (Line) LogicalTreeHelper.FindLogicalNode(rootElement, "Line2");

            VerifyElement.VerifyBool(null == line2, false, ref result);
            VerifyElement.VerifyDouble(line2.X1, 40, ref result);
            VerifyElement.VerifyDouble(line2.Y1, 350, ref result);
            VerifyElement.VerifyDouble(line2.X2, 250, ref result);
            VerifyElement.VerifyDouble(line2.Y2, 350, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) line2).Stroke)).Color, Colors.Purple, ref result);
            VerifyElement.VerifyInt((int)((Shape) line2).StrokeStartLineCap, (int) PenLineCap.Round, ref result);
            VerifyElement.VerifyInt((int)((Shape) line2).StrokeEndLineCap, (int) PenLineCap.Round, ref result);
            VerifyElement.VerifyDouble(((Shape) line2).StrokeThickness, 45, ref result);

            Line line3 = (Line) LogicalTreeHelper.FindLogicalNode(rootElement, "Line3");

            VerifyElement.VerifyBool(null == line3, false, ref result);
            VerifyElement.VerifyDouble(line3.X1, 40, ref result);
            VerifyElement.VerifyDouble(line3.Y1, 400, ref result);
            VerifyElement.VerifyDouble(line3.X2, 250, ref result);
            VerifyElement.VerifyDouble(line3.Y2, 400, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) line3).Stroke)).Color, Colors.Yellow, ref result);
            VerifyElement.VerifyInt((int)((Shape) line3).StrokeStartLineCap, (int) PenLineCap.Square, ref result);
            VerifyElement.VerifyInt((int)((Shape) line3).StrokeEndLineCap, (int) PenLineCap.Square, ref result);
            VerifyElement.VerifyDouble(((Shape) line3).StrokeThickness, 45, ref result);

            return result;
        }
    }
}
