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
    public static class TranslatingTransformsGraphics_Verify
    {
        /// <summary>
        /// Verification routine for TranslatingTransformGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("TransformDecorator Stroke Verify ...");

            Decorator myTransformDecorator = (Decorator) LogicalTreeHelper.FindLogicalNode(myPanel, "TransformDecorator");

            VerifyElement.VerifyBool(null == myTransformDecorator, false, ref result);
            //VerifyElement.VerifyBool(myTransformDecorator.AffectsLayout, false);
            GlobalLog.LogStatus("TransformDecorator Stroke Verify : transform ...");

            TranslateTransform myTransform = myTransformDecorator.RenderTransform as TranslateTransform;

            VerifyElement.VerifyBool(null == myTransform, false, ref result);

            VerifyElement.VerifyDouble(myTransform.X, 250, ref result);
            VerifyElement.VerifyDouble(myTransform.Y, 50, ref result);

            Path myEllipse = (Path) LogicalTreeHelper.FindLogicalNode(rootElement, "Ellipse");
            VerifyElement.VerifyBool(null == myEllipse, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myEllipse).Stroke)).Color, Colors.Yellow, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myEllipse).Fill)).Color, Colors.Purple, ref result);
            VerifyElement.VerifyDouble(((Shape) myEllipse).StrokeThickness, 3, ref result);
            EllipseGeometry pathData = myEllipse.Data as EllipseGeometry;
            VerifyElement.VerifyBool(null == pathData, false, ref result);
            VerifyElement.VerifyDouble(pathData.Bounds.Left, 125, ref result);
            VerifyElement.VerifyDouble(pathData.Bounds.Right, 175, ref result);
            VerifyElement.VerifyDouble(pathData.Bounds.Top, 25, ref result);
            VerifyElement.VerifyDouble(pathData.Bounds.Bottom, 75, ref result);

            return result;
        }
    }
}
