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

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Shapes
{
    /// <summary/>
    public static class GraphicsShapes_Verify
    {
        /// <summary>
        /// Verification routine for ShapesGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("Shapes Verify ...");

            GlobalLog.LogStatus("Verify Path ...");

            Path path = (Path) LogicalTreeHelper.FindLogicalNode(myPanel, "Path");

            VerifyElement.VerifyBool(null == path, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path).Stroke)).Color, Colors.Yellow, ref result);
            VerifyElement.VerifyDouble(((Shape) path).StrokeThickness, 6, ref result);
            StreamGeometry myStream   = path.Data as StreamGeometry;
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

            GlobalLog.LogStatus("Verify Ellipse ...");
            Path myEllipse = (Path) LogicalTreeHelper.FindLogicalNode(rootElement, "Ellipse");
            VerifyElement.VerifyBool(null == myEllipse, false, ref result);

            return result;
        }
    }
}
