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
    public static class LineGeometryGraphics_Verify
    {
        /// <summary>
        /// Verification method for LineGeometry in graphics
        ///</summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("RectangleGeometry ...");

            Path path = (Path) LogicalTreeHelper.FindLogicalNode(myPanel, "Path");

            VerifyElement.VerifyBool(null == path, false, ref result);

            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path).Stroke)).Color, Colors.Orange, ref result);
            VerifyElement.VerifyDouble(((Shape) path).StrokeThickness, 20, ref result);
            VerifyElement.VerifyInt((int)((Shape) path).StrokeStartLineCap, (int) PenLineCap.Flat, ref result);
            VerifyElement.VerifyInt((int)((Shape) path).StrokeEndLineCap, (int) PenLineCap.Triangle, ref result);

            LineGeometry myGeometry = path.Data as LineGeometry;

            VerifyElement.VerifyBool(null == myGeometry, false, ref result);
            VerifyElement.VerifyPoint(myGeometry.StartPoint, new Point(350, 25), ref result);
            VerifyElement.VerifyPoint(myGeometry.EndPoint, new Point(500, 75), ref result);
            return result;
        }
    }
}
