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
using Microsoft.Test.Xaml.Utilities;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Stroke
{
    /// <summary/>
    public static class PolygonStrokeGraphics_Verify
    {
        /// <summary>
        /// Verification routine for PolygonStrokeGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("Polygon Stroke Verify ...");

            Polygon myPolygon = (Polygon) LogicalTreeHelper.FindLogicalNode(myPanel, "Polygon");
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myPolygon).Stroke)).Color, Colors.Blue, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myPolygon).Fill)).Color, Colors.Red, ref result);
            VerifyElement.VerifyDouble(((Shape) myPolygon).StrokeThickness, 1, ref result);
            VerifyElement.VerifyInt((int)((Shape) myPolygon).StrokeLineJoin, (int) PenLineJoin.Miter, ref result);
            VerifyElement.VerifyDouble(((Shape) myPolygon).StrokeMiterLimit, 3, ref result);

            GlobalLog.LogStatus("Polygon Stroke Verify: Points ...");
            PointCollection myPoints = myPolygon.Points;

            VerifyElement.VerifyInt(myPoints.Count, 23, ref result);

            VerifyElement.VerifyPoint(myPoints[0], new Point(15, 10), ref result);
            VerifyElement.VerifyPoint(myPoints[1], new Point(50, 30), ref result);
            VerifyElement.VerifyPoint(myPoints[2], new Point(50, 25), ref result);
            VerifyElement.VerifyPoint(myPoints[3], new Point(45, 20), ref result);

            VerifyElement.VerifyPoint(myPoints[4], new Point(45, 15), ref result);
            VerifyElement.VerifyPoint(myPoints[5], new Point(50, 10), ref result);
            VerifyElement.VerifyPoint(myPoints[6], new Point(55, 10), ref result);
            VerifyElement.VerifyPoint(myPoints[7], new Point(60, 15), ref result);

            VerifyElement.VerifyPoint(myPoints[22], new Point(15, 10), ref result);

            return result;
        }
    }
}
