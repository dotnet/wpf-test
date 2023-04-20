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
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Transform
{
    /// <summary/>
    public static class RotateTransformsGraphics_Verify
    {
        /// <summary>
        /// Verification routine for RotateTransformGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("Transform Scale Verify ...");

            Decorator myTransformDecorator = (Decorator) LogicalTreeHelper.FindLogicalNode(myPanel, "TransformDecorator");
            VerifyElement.VerifyBool(null == myTransformDecorator, false, ref result);

            GlobalLog.LogStatus("Transform Scale Verify : transform ...");

            RotateTransform myTransform = myTransformDecorator.RenderTransform as RotateTransform;

            VerifyElement.VerifyDouble(myTransform.Angle, 25, ref result);
            Line line1 = (Line) LogicalTreeHelper.FindLogicalNode(rootElement, "Line");

            VerifyElement.VerifyBool(null == line1, false, ref result);
            VerifyElement.VerifyDouble(line1.X1, 450, ref result);
            VerifyElement.VerifyDouble(line1.Y1, 50, ref result);
            VerifyElement.VerifyDouble(line1.X2, 55, ref result);
            VerifyElement.VerifyDouble(line1.Y2, 340, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) line1).Stroke)).Color, Colors.Yellow, ref result);
            VerifyElement.VerifyDouble(((Shape) line1).StrokeThickness, 20, ref result);

            return result;
        }
    }
}
