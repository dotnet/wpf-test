// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Geometry
{
    /// <summary/>
    public static class CombinedGeometryGraphics_Verify
    {
        /// <summary>
        /// Verification method for CombinedGeometry in graphics
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            Path path1 = (Path) LogicalTreeHelper.FindLogicalNode(myPanel, "Path1");

            VerifyElement.VerifyBool(null == path1, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path1).Fill)).Color, Colors.Red, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) path1).Stroke)).Color, Colors.White, ref result);
            VerifyElement.VerifyDouble(((Shape) path1).StrokeThickness, 3, ref result);

            CombinedGeometry myGeometries = path1.Data as CombinedGeometry;
            VerifyElement.VerifyBool(null == myGeometries, false, ref result);
            VerifyElement.VerifyInt((int) myGeometries.GeometryCombineMode, (int) GeometryCombineMode.Xor, ref result);

            RectangleGeometry rectangleGeometry1 = myGeometries.Geometry1 as RectangleGeometry;
            RectangleGeometry rectangleGeometry2 = myGeometries.Geometry2 as RectangleGeometry;

            VerifyElement.VerifyRect(rectangleGeometry1.Rect, new Rect(0, 0, 100, 100), ref result);
            VerifyElement.VerifyRect(rectangleGeometry2.Rect, new Rect(50, 50, 100, 100), ref result);
            return result;
        }
    }
}
