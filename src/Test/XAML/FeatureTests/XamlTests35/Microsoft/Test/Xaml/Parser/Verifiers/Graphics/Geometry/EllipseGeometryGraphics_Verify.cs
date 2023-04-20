// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using Microsoft.Test.Logging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Geometry
{
    /// <summary/>
    public static class EllipseGeometryGraphics_Verify
    {
        /// <summary>
        /// EllipseGeometryGraphics_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("RectangleGeometry ...");

            Path path = (Path) LogicalTreeHelper.FindLogicalNode(myPanel, "Path");

            if (null == path)
            {
                GlobalLog.LogEvidence("null == path");
                result = false;
            }

            EllipseGeometry myGeometry = path.Data as EllipseGeometry;

            if (null == myGeometry)
            {
                GlobalLog.LogEvidence("null == myGeometry");
                result = false;
            }
            if (!(myGeometry.Center.Equals(new Point(60, 300))))
            {
                GlobalLog.LogEvidence("!(myGeometry.Center.Equals(new Point(60, 300)))");
                result = false;
            }
            if (myGeometry.RadiusX != 50)
            {
                GlobalLog.LogEvidence("myGeometry.RadiusX != 50");
                result = false;
            }
            if (myGeometry.RadiusY != 75)
            {
                GlobalLog.LogEvidence("myGeometry.RadiusY != 75");
                result = false;
            }
            return result;
        }
    }
}
