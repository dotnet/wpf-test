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

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Brush
{
    /// <summary/>
    public static class DrawingBrushesGraphics_Verify
    {
        /// <summary>
        /// Verification routine for DrawingBrushesGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Verifying drawing Brush: Ellipse2 ...");
            Path ellipse2 = (Path) LogicalTreeHelper.FindLogicalNode(rootElement, "Ellipse2");
            if (null == ellipse2)
            {
                GlobalLog.LogEvidence("null == ellipse2");
                result = false;
            }

            DrawingBrush myDrawingBrush = ellipse2.Fill as DrawingBrush;
            if (null == myDrawingBrush)
            {
                GlobalLog.LogEvidence("null == myDrawingBrush");
                result = false;
            }
            if (((DrawingGroup) myDrawingBrush.Drawing).Children.Count != 2)
            {
                GlobalLog.LogEvidence("((DrawingGroup)myDrawingBrush.Drawing).Children.Count != 2");
                result = false;
            }
            GeometryDrawing drawing1 = ((DrawingGroup) myDrawingBrush.Drawing).Children[0] as GeometryDrawing;
            if (null == drawing1)
            {
                GlobalLog.LogEvidence("null == drawing1");
                result = false;
            }
            GeometryDrawing drawing2 = ((DrawingGroup) myDrawingBrush.Drawing).Children[1] as GeometryDrawing;
            if (null == drawing2)
            {
                GlobalLog.LogEvidence("null == drawing2");
                result = false;
            }

            DrawingBrush innerBrush = drawing2.Brush as DrawingBrush;
            if (((DrawingGroup) innerBrush.Drawing).Children.Count != 3)
            {
                GlobalLog.LogEvidence("((DrawingGroup)innerBrush.Drawing).Children.Count != 3");
                result = false;
            }
            return result;
        }
    }
}
