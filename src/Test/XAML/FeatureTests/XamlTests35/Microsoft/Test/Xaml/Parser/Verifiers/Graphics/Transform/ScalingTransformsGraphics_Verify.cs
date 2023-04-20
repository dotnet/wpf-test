// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Xaml.Utilities;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Transform
{
    /// <summary/>
    public static class ScalingTransformsGraphics_Verify
    {
        /// <summary>
        /// Verification routine for ScalingTransformGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("Transform Scale Verify ...");

            Decorator myTransformDecorator = (Decorator) LogicalTreeHelper.FindLogicalNode(myPanel, "TransformDecorator");

            VerifyElement.VerifyBool(null == myTransformDecorator, false, ref result);
            GlobalLog.LogStatus("Transform Scale Verify : transform ...");

            ScaleTransform myTransform = myTransformDecorator.LayoutTransform as ScaleTransform;

            VerifyElement.VerifyDouble(myTransform.ScaleX, 2, ref result);
            VerifyElement.VerifyDouble(myTransform.ScaleY, 2, ref result);
            GlobalLog.LogStatus("Verify Rectangle ...");

            Path myEllipse = (Path) LogicalTreeHelper.FindLogicalNode(rootElement, "Ellipse");

            VerifyElement.VerifyBool(null == myEllipse, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myEllipse).Stroke)).Color,
                Color.FromArgb(0xaa, 0x33, 0xff, 0x33), ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myEllipse).Fill)).Color, Colors.Yellow, ref result);
            VerifyElement.VerifyDouble(((Shape) myEllipse).StrokeThickness, 3, ref result);

            return result;
        }
    }
}
