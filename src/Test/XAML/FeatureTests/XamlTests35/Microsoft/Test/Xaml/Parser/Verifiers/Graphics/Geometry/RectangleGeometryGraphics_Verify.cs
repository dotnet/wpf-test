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
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Geometry
{
    /// <summary/>
    public static class RectangleGeometryGraphics_Verify
    {
        /// <summary>
        /// Verification method for RectangleGeometry in graphics
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;
            GlobalLog.LogStatus("RectangleGeometry ...");

            Path path = (Path) LogicalTreeHelper.FindLogicalNode(myPanel, "Path");

            VerifyElement.VerifyBool(null == path, false, ref result);

            RectangleGeometry myGeometry = path.Data as RectangleGeometry;

            VerifyElement.VerifyBool(null == myGeometry, false, ref result);
            VerifyElement.VerifyRect(myGeometry.Rect, new Rect(325, 225, 175, 75), ref result);
            VerifyElement.VerifyDouble(myGeometry.RadiusX, 10, ref result);
            VerifyElement.VerifyDouble(myGeometry.RadiusY, 5, ref result);
            return result;
        }
    }
}
