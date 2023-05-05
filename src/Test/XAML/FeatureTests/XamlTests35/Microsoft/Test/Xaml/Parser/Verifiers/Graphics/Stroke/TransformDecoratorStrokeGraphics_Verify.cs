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
using Microsoft.Test.Xaml.Utilities;
using System.Windows.Shapes;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Stroke
{
    /// <summary/>
    public static class TransformDecoratorStrokeGraphics_Verify
    {
        /// <summary>
        /// Verification routine for TransformDecoratorStrokeGraphics.xaml.
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
            TransformGroup myTransforms = myTransformDecorator.RenderTransform as TransformGroup;
            VerifyElement.VerifyBool(null == myTransforms, false, ref result);
            VerifyElement.VerifyInt(myTransforms.Children.Count, 2, ref result);

            TranslateTransform myTranslateTransform = myTransforms.Children[0] as TranslateTransform;
            VerifyElement.VerifyDouble(myTranslateTransform.X, 200, ref result);
            VerifyElement.VerifyDouble(myTranslateTransform.Y, 0, ref result);

            ScaleTransform myScaleTransform = myTransforms.Children[1] as ScaleTransform;
            VerifyElement.VerifyDouble(myScaleTransform.ScaleX, 2, ref result);
            VerifyElement.VerifyDouble(myScaleTransform.ScaleY, 2, ref result);
            GlobalLog.LogStatus("TransformDecorator Stroke Verify : polyline ...");

            Polyline myPolyline = (Polyline) LogicalTreeHelper.FindLogicalNode(rootElement, "Polyline");
            VerifyElement.VerifyBool(null == myPolyline, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myPolyline).Stroke)).Color, Colors.Blue, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myPolyline).Fill)).Color, Colors.Yellow, ref result);
            VerifyElement.VerifyDouble(((Shape) myPolyline).StrokeThickness, 3, ref result);
            VerifyElement.VerifyInt((int)((Shape) myPolyline).StrokeLineJoin, (int) PenLineJoin.Miter, ref result);
            VerifyElement.VerifyDouble(((Shape) myPolyline).StrokeMiterLimit, 1, ref result);

            GlobalLog.LogStatus("TransformDecorator Stroke Verify : points in Polyline ...");

            PointCollection myPoints = myPolyline.Points;

            VerifyElement.VerifyInt(myPoints.Count, 6, ref result);

            VerifyElement.VerifyPoint(myPoints[0], new Point(10, 10), ref result);
            VerifyElement.VerifyPoint(myPoints[1], new Point(60, 30), ref result);
            VerifyElement.VerifyPoint(myPoints[2], new Point(10, 50), ref result);
            VerifyElement.VerifyPoint(myPoints[3], new Point(60, 70), ref result);

            VerifyElement.VerifyPoint(myPoints[4], new Point(10, 90), ref result);
            VerifyElement.VerifyPoint(myPoints[5], new Point(30, 25), ref result);

            return result;
        }
    }
}
