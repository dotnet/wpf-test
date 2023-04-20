// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Dimension
{
    /// <summary/>
    public static class Rings_Verify
    {
        /// <summary>
        /// Verification routine for DrawingBrushesGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Verifying Grid Resources ...");
            ResourceDictionary resources = ((Grid)rootElement).Resources;
            if (null == resources)
            {
                GlobalLog.LogEvidence("null == resources");
                result = false;
            }

            MeshGeometry3D geometry3D = resources["mesh"] as MeshGeometry3D;
            if (null == geometry3D)
            {
                GlobalLog.LogEvidence("null == geometry3D");
                result = false;
            }

            if (resources.Count != 15)
            {
                GlobalLog.LogEvidence("modelGroup.Children.Count != [15] Actual["+resources.Count+"]");
                result = false;
            }

            Camera camera = resources["camera"] as Camera;
            if (null == camera)
            {
                GlobalLog.LogEvidence("null == camera");
                result = false;
            }

            return result;
        }
    }
}
