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
using System.Windows.Shapes;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Transform
{
    /// <summary/>
    public static class SkewTransformsGraphics_Verify
    {
        /// <summary>
        /// Verification routine for SkewTransformGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("Transform Skew Verify ...");

            Decorator myTransformDecorator = (Decorator) LogicalTreeHelper.FindLogicalNode(myPanel, "TransformDecorator");

            VerifyElement.VerifyBool(null == myTransformDecorator, false, ref result);
            GlobalLog.LogStatus("TransformDecorator Stroke Verify : transform ... " + myTransformDecorator.LayoutTransform.GetType().Name);

            TransformGroup myTransforms = myTransformDecorator.LayoutTransform as TransformGroup;
            VerifyElement.VerifyBool(null == myTransforms.Children, false, ref result);
            VerifyElement.VerifyInt(myTransforms.Children.Count, 2, ref result);

            GlobalLog.LogStatus("Transform type:  " + myTransforms.Children[0].GetType().Name);

            SkewTransform myTransform = myTransforms.Children[0] as SkewTransform;
            GlobalLog.LogStatus("Anglex:  " + myTransform.AngleX + "Angley:  " + myTransform.AngleY + "Centerx: " + myTransform.CenterX + "Centery: " + myTransform.CenterY);

            VerifyElement.VerifyBool(null == myTransform, false, ref result);

            VerifyElement.VerifyDouble(myTransform.AngleY, -20, ref result);

            GlobalLog.LogStatus("Second trandform:  " + myTransforms.Children[1].GetType().Name);
            SkewTransform myTransform2 = myTransforms.Children[1] as SkewTransform;

            GlobalLog.LogStatus("Anglex:  " + myTransform2.AngleX + "Angley:  " + myTransform2.AngleY + "Centerx: " + myTransform2.CenterX + "Centery: " + myTransform2.CenterY);

            VerifyElement.VerifyBool(null == myTransform2, false, ref result);
            VerifyElement.VerifyDouble(myTransform2.AngleX, -20, ref result);
            GlobalLog.LogStatus("Verify Rectangle ...");
            Rectangle myRectangle = (Rectangle) LogicalTreeHelper.FindLogicalNode(rootElement, "Rectangle");
            VerifyElement.VerifyBool(null == myRectangle, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myRectangle).Stroke)).Color, Colors.Red, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myRectangle).Fill)).Color, Colors.Pink, ref result);
            VerifyElement.VerifyDouble(((Shape) myRectangle).StrokeThickness, 3, ref result);
            VerifyElement.VerifyDouble(myRectangle.Width, 150, ref result);
            VerifyElement.VerifyDouble(myRectangle.Height, 50, ref result);
            VerifyElement.VerifyDouble(Canvas.GetTop(myRectangle), 10, ref result);
            VerifyElement.VerifyDouble(Canvas.GetLeft(myRectangle), 5, ref result);
            return result;
        }
    }
}
