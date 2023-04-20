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

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Geometry
{
    /// <summary/>
    public static class PathGeometryGraphics_Verify
    {
        /// <summary>
        /// Verification method for PathGeometry in graphics
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;
            GlobalLog.LogStatus("PathGeometry ...");

            Path path1 = (Path) LogicalTreeHelper.FindLogicalNode(myPanel, "Path1");

            VerifyElement.VerifyBool(null == path1, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path1).Fill)).Color, Color.FromArgb(0x40, 0x00, 0xff, 0x00), ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path1).Stroke)).Color, Colors.Yellow, ref result);

            PathGeometry myGeometry = path1.Data as PathGeometry;
            VerifyElement.VerifyBool(null == myGeometry, false, ref result);
            VerifyElement.VerifyInt((int) myGeometry.FillRule, (int) FillRule.EvenOdd, ref result);

            TranslateTransform myTranslateTransform = myGeometry.Transform as TranslateTransform;
            VerifyElement.VerifyBool(null == myTranslateTransform, false, ref result);
            VerifyElement.VerifyDouble(myTranslateTransform.X, 225, ref result);
            VerifyElement.VerifyDouble(myTranslateTransform.Y, 25, ref result);

            PathFigureCollection myPathFigureCollection = myGeometry.Figures;
            VerifyElement.VerifyBool(null == myPathFigureCollection, false, ref result);

            VerifyElement.VerifyInt(myPathFigureCollection.Count, 1, ref result);
            PathFigure myPathFigure = myPathFigureCollection[0];
            VerifyElement.VerifyBool(null == myPathFigure, false, ref result);
            VerifyElement.VerifyBool(myPathFigure.IsFilled, true, ref result);

            PathSegmentCollection myPathSegmentCollection = myPathFigure.Segments;
            VerifyElement.VerifyBool(null == myPathSegmentCollection, false, ref result);
            VerifyElement.VerifyInt(myPathSegmentCollection.Count, 7, ref result);

            LineSegment myLineSegment = myPathSegmentCollection[0] as LineSegment;
            VerifyElement.VerifyBool(null == myLineSegment, false, ref result);
            VerifyElement.VerifyPoint(myLineSegment.Point, new Point(100, 0), ref result);

            BezierSegment myBezierSegment = myPathSegmentCollection[1] as BezierSegment;
            VerifyElement.VerifyBool(null == myBezierSegment, false, ref result);
            VerifyElement.VerifyPoint(myBezierSegment.Point1, new Point(125, 25), ref result);
            VerifyElement.VerifyPoint(myBezierSegment.Point2, new Point(125, 75), ref result);
            VerifyElement.VerifyPoint(myBezierSegment.Point3, new Point(100, 100), ref result);

            QuadraticBezierSegment myQuadraticBezierSegment = myPathSegmentCollection[2] as QuadraticBezierSegment;
            VerifyElement.VerifyBool(null == myQuadraticBezierSegment, false, ref result);
            VerifyElement.VerifyPoint(myQuadraticBezierSegment.Point1, new Point(50, 50), ref result);
            VerifyElement.VerifyPoint(myQuadraticBezierSegment.Point2, new Point(0, 100), ref result);
            ArcSegment myArcSegment = myPathSegmentCollection[3] as ArcSegment;
            VerifyElement.VerifyBool(null == myArcSegment, false, ref result);
            VerifyElement.VerifyPoint(myArcSegment.Point, new Point(100, 150), ref result);
            VerifyElement.VerifySize(myArcSegment.Size, new Size(100, 100), ref result);
            VerifyElement.VerifyDouble(myArcSegment.RotationAngle, 45, ref result);
            VerifyElement.VerifyBool(myArcSegment.IsLargeArc, false, ref result);
            PolyLineSegment myPolyLineSegment = myPathSegmentCollection[4] as PolyLineSegment;
            VerifyElement.VerifyBool(null == myPolyLineSegment, false, ref result);
            PointCollection myPoints = myPolyLineSegment.Points;
            VerifyElement.VerifyBool(null == myPoints, false, ref result);
            VerifyElement.VerifyInt(myPoints.Count, 2, ref result);
            VerifyElement.VerifyPoint(myPoints[0], new Point(100, 175), ref result);
            VerifyElement.VerifyPoint(myPoints[1], new Point(0, 175), ref result);

            return result;
        }
    }
}
