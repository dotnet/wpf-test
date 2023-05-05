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
    public static class TransformCollectionTransformsGraphics_Verify
    {
        /// <summary>
        /// Verification routine for TransformCollectionTransformsGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("TransformDecorator Stroke Verify ...");

            Decorator myTransformDecorator = (Decorator) LogicalTreeHelper.FindLogicalNode(myPanel, "TransformDecorator");

            VerifyElement.VerifyBool(null == myTransformDecorator, false, ref result);
            //VerifyElement.VerifyBool(myTransformDecorator.AffectsLayout, false);
            GlobalLog.LogStatus("Transform Collection Verify : transform ...");

            TransformGroup myTransforms = myTransformDecorator.RenderTransform as TransformGroup;

            VerifyElement.VerifyBool(null == myTransforms.Children, false, ref result);
            VerifyElement.VerifyInt(myTransforms.Children.Count, 6, ref result);

            GlobalLog.LogStatus("Transform 1:  " + myTransforms.Children[0].GetType().Name);

            TranslateTransform myTransform1 = myTransforms.Children[0] as TranslateTransform;
            VerifyElement.VerifyBool(null == myTransform1, false, ref result);
            VerifyElement.VerifyDouble(myTransform1.X, 10, ref result);
            VerifyElement.VerifyDouble(myTransform1.Y, 10, ref result);

            GlobalLog.LogStatus("Transform 2:  " + myTransforms.Children[1].GetType().Name);

            ScaleTransform myTransform2 = myTransforms.Children[1] as ScaleTransform;
            VerifyElement.VerifyBool(null == myTransform2, false, ref result);
            VerifyElement.VerifyDouble(myTransform2.ScaleX, 1.5, ref result);
            VerifyElement.VerifyDouble(myTransform2.ScaleY, 0.75, ref result);

            GlobalLog.LogStatus("Transform 3:  " + myTransforms.Children[2].GetType().Name);
            SkewTransform myTransform3 = myTransforms.Children[2] as SkewTransform;
            VerifyElement.VerifyBool(null == myTransform3, false, ref result);
            VerifyElement.VerifyDouble(((SkewTransform) myTransform3).AngleX, 1.5, ref result);
            VerifyElement.VerifyDouble(((SkewTransform) myTransform3).AngleY, 0.9, ref result);

            GlobalLog.LogStatus("Transform 4:  " + myTransforms.Children[3].GetType().Name);
            RotateTransform myTransform4 = myTransforms.Children[3] as RotateTransform;
            VerifyElement.VerifyBool(null == myTransform4, false, ref result);
            VerifyElement.VerifyDouble(myTransform4.Angle, 25, ref result);

            GlobalLog.LogStatus("Transform 5:  " + myTransforms.Children[4].GetType().Name);

            ScaleTransform myTransform5 = myTransforms.Children[4] as ScaleTransform;
            VerifyElement.VerifyBool(null == myTransform5, false, ref result);
            VerifyElement.VerifyDouble(myTransform5.ScaleX, 1.2, ref result);
            VerifyElement.VerifyDouble(myTransform5.ScaleY, 1.3, ref result);

            GlobalLog.LogStatus("Transform 6:  " + myTransforms.Children[5].GetType().Name);
            RotateTransform myTransform6 = myTransforms.Children[5] as RotateTransform;
            VerifyElement.VerifyBool(null == myTransform6, false, ref result);
            VerifyElement.VerifyDouble(myTransform6.Angle, 2, ref result);
            VerifyElement.VerifyDouble(myTransform6.CenterX, 45, ref result);
            VerifyElement.VerifyDouble(myTransform6.CenterY, 45, ref result);

            GlobalLog.LogStatus("Rectangle: ");
            Rectangle myRectangle = (Rectangle) LogicalTreeHelper.FindLogicalNode(rootElement, "Rectangle");

            VerifyElement.VerifyBool(null == myRectangle, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myRectangle).Stroke)).Color, Colors.Blue, ref result);
            VerifyElement.VerifyDouble(((Shape) myRectangle).StrokeThickness, 2, ref result);
            VerifyElement.VerifyDouble(myRectangle.Width, 100, ref result);
            VerifyElement.VerifyDouble(myRectangle.Height, 100, ref result);
            VerifyElement.VerifyDouble(Canvas.GetTop(myRectangle), 100, ref result);
            VerifyElement.VerifyDouble(Canvas.GetLeft(myRectangle), 125, ref result);

            return result;
        }
    }
}
