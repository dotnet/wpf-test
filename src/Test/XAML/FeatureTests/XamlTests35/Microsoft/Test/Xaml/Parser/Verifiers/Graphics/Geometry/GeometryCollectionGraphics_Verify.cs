// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using System.Windows.Shapes;
using Microsoft.Test.Xaml.Utilities;
using System.Windows.Media;
using System.Windows;
using Microsoft.Test.Logging;

namespace XamlTests.Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Geometry
{
    /// <summary/>
    public static class GeometryCollectionGraphics_Verify
    {
        /// <summary>
        /// Verification method for EllipseGeometry in graphics
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result       = true;
            CustomCanvas customCanvas = rootElement as CustomCanvas;

            GlobalLog.LogStatus("RectangleGeometry ...");

            Path path = (Path) LogicalTreeHelper.FindLogicalNode(customCanvas, "Path");

            VerifyElement.VerifyBool(null == path, false, ref result);

            EllipseGeometry myGeometry = path.Data as EllipseGeometry;

            VerifyElement.VerifyBool(null == myGeometry, false, ref result);
            VerifyElement.VerifyPoint(myGeometry.Center, new Point(60, 300), ref result);
            VerifyElement.VerifyDouble(myGeometry.RadiusX, 50, ref result);
            VerifyElement.VerifyDouble(myGeometry.RadiusY, 75, ref result);

            return result;
        }
    }
}
