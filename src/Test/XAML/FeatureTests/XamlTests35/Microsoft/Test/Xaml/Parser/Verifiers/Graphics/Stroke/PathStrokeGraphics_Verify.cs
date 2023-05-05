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
    public static class PathStrokeGraphics_Verify
    {
        /// <summary>
        /// Verification routine for PathStrokeGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("Path Stroke Verify ...");

            Path path1 = (Path) LogicalTreeHelper.FindLogicalNode(myPanel, "Path1");
            VerifyElement.VerifyBool(null == path1, false, ref result);

            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path1).Stroke)).Color, Colors.Blue, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path1).Fill)).Color, Color.FromRgb(0xff, 0x22, 0x0), ref result);

            VerifyElement.VerifyDouble(((Shape) path1).StrokeThickness, 10, ref result);
            VerifyElement.VerifyInt((int)((Shape) path1).StrokeStartLineCap, (int) PenLineCap.Round, ref result);
            VerifyElement.VerifyInt((int)((Shape) path1).StrokeEndLineCap, (int) PenLineCap.Flat, ref result);
            VerifyElement.VerifyInt((int)((Shape) path1).StrokeDashCap, (int) PenLineCap.Triangle, ref result);
            VerifyElement.VerifyInt((int)((Shape) path1).StrokeLineJoin, (int) PenLineJoin.Round, ref result);
            VerifyElement.VerifyDouble(((Shape) path1).StrokeDashOffset, 1, ref result);
            VerifyElement.VerifyInt(((Shape) path1).StrokeDashArray.Count, 6, ref result);
            VerifyElement.VerifyDouble(((Shape) path1).StrokeDashArray[0], 1.5, ref result);
            VerifyElement.VerifyDouble(((Shape) path1).StrokeDashArray[1], 2.0, ref result);
            VerifyElement.VerifyDouble(((Shape) path1).StrokeDashArray[2], 3.0, ref result);
            VerifyElement.VerifyDouble(((Shape) path1).StrokeDashArray[3], 2.0, ref result);
            VerifyElement.VerifyDouble(((Shape) path1).StrokeDashArray[4], 1.0, ref result);
            VerifyElement.VerifyDouble(((Shape) path1).StrokeDashArray[5], 2.0, ref result);

            StreamGeometry myStream   = path1.Data as StreamGeometry;
            PathGeometry   myGeometry = PathGeometry.CreateFromGeometry(myStream);
            VerifyElement.VerifyBool(null == myGeometry, false, ref result);
            PathFigureCollection myFigures = myGeometry.Figures;
            VerifyElement.VerifyBool(null == myFigures, false, ref result);
            VerifyElement.VerifyInt(myFigures.Count, 1, ref result);
            PathFigure myFigure = myFigures[0];
            VerifyElement.VerifyBool(null == myFigure, false, ref result);
            PathSegmentCollection mySegments = myFigure.Segments;
            VerifyElement.VerifyBool(null == mySegments, false, ref result);
            VerifyElement.VerifyInt(mySegments.Count, 1, ref result);

            myStream = path1.Clip as StreamGeometry;
            myGeometry = PathGeometry.CreateFromGeometry(myStream);
            VerifyElement.VerifyBool(null == myGeometry, false, ref result);
            myFigures = myGeometry.Figures;
            VerifyElement.VerifyBool(null == myFigures, false, ref result);
            VerifyElement.VerifyInt(myFigures.Count, 1, ref result);
            myFigure = myFigures[0];
            VerifyElement.VerifyBool(null == myFigure, false, ref result);
            mySegments = myFigure.Segments;
            VerifyElement.VerifyBool(null == mySegments, false, ref result);
            VerifyElement.VerifyInt(mySegments.Count, 1, ref result);

            GlobalLog.LogStatus("Path Stroke Verify: path2 ...");
            Path path2 = (Path) LogicalTreeHelper.FindLogicalNode(rootElement, "Path2");

            VerifyElement.VerifyBool(null == path1, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path1).Stroke)).Color, Colors.Blue, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path1).Fill)).Color, Color.FromRgb(0xff, 0x22, 0x0), ref result);

            GlobalLog.LogStatus("Path Stroke Verify: path2 - shape ...");
            VerifyElement.VerifyDouble(((Shape) path2).StrokeThickness, 20, ref result);
            VerifyElement.VerifyInt((int)((Shape) path2).StrokeStartLineCap, (int) PenLineCap.Flat, ref result);
            VerifyElement.VerifyInt((int)((Shape) path2).StrokeEndLineCap, (int) PenLineCap.Triangle, ref result);
            VerifyElement.VerifyInt((int)((Shape) path2).StrokeDashCap, (int) PenLineCap.Round, ref result);
            VerifyElement.VerifyInt((int)((Shape) path2).StrokeLineJoin, (int) PenLineJoin.Bevel, ref result);
            VerifyElement.VerifyDouble(((Shape) path2).StrokeDashOffset, 0, ref result);
            VerifyElement.VerifyInt(((Shape) path2).StrokeDashArray.Count, 2, ref result);
            VerifyElement.VerifyDouble(((Shape) path2).StrokeDashArray[0], 1.0, ref result);
            VerifyElement.VerifyDouble(((Shape) path2).StrokeMiterLimit, 100, ref result);
            VerifyElement.VerifyDouble(((Shape) path2).StrokeDashArray[1], 2.0, ref result);

            GlobalLog.LogStatus("Path Stroke Verify: path2 - data ...");
            myStream = path2.Data as StreamGeometry;
            myGeometry = PathGeometry.CreateFromGeometry(myStream);
            VerifyElement.VerifyBool(null == myGeometry, false, ref result);
            myFigures = myGeometry.Figures;
            VerifyElement.VerifyBool(null == myFigures, false, ref result);
            VerifyElement.VerifyInt(myFigures.Count, 1, ref result);
            myFigure = myFigures[0];
            VerifyElement.VerifyBool(null == myFigure, false, ref result);
            mySegments = myFigure.Segments;
            VerifyElement.VerifyBool(null == mySegments, false, ref result);
            VerifyElement.VerifyInt(mySegments.Count, 1, ref result);

            return result;
        }
    }
}
