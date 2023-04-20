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
    public static class AmbientSphereTwoLights_Verify
    {
        /// <summary>
        /// Verification routine for DrawingBrushesGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Verifying ViewPort3D: View ...");
            Viewport3D view = (Viewport3D)LogicalTreeHelper.FindLogicalNode(rootElement, "View");
            if (null == view)
            {
                GlobalLog.LogEvidence("null == Viewport3D");
                result = false;
            }

            Camera camera = view.Camera as Camera;
            if (null == camera)
            {
                GlobalLog.LogEvidence("null == camera");
                result = false;
            }

            ModelVisual3D model = view.Children[0] as ModelVisual3D;
            if (null == model)
            {
                GlobalLog.LogEvidence("null == model");
                result = false;
            }

            Model3DGroup modelGroup = model.Content as Model3DGroup;
            if (null == modelGroup)
            {
                GlobalLog.LogEvidence("null == modelGroup");
                result = false;
            }

            if (modelGroup.Children.Count != 6)
            {
                GlobalLog.LogEvidence("modelGroup.Children.Count != [6] Actual["+modelGroup.Children.Count+"]");
                result = false;
            }

            return result;
        }
    }
}
