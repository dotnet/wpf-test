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
    public static class MatrixTransformsGraphics_Verify
    {
        /// <summary>
        /// Verification routine for MatrixTransformsGraphics.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;

            GlobalLog.LogStatus("Matrix Transform Verify ...");

            Decorator myTransformDecorator = (Decorator) LogicalTreeHelper.FindLogicalNode(myPanel, "TransformDecorator");

            VerifyElement.VerifyBool(null == myTransformDecorator, false, ref result);

            GlobalLog.LogStatus("Transform Collection Verify : transform ...");
            
			//blocked
            //TransformGroup myGroup = myTransformDecorator.LayoutTransform as TransformGroup;

            //VerifyElement.VerifyBool(null == myGroup.Children, false);
            //VerifyElement.VerifyInt(myGroup.Children.Count, 1);

            //MatrixTransform myTransform = myGroup.Children[0] as MatrixTransform;

            //VerifyElement.VerifyBool(Matrix.Equals(myTransform.Matrix, new Matrix(1, 0, 1, 1, 50, 90)), true);

            GlobalLog.LogStatus("Rectangle: ");
            Rectangle myRectangle = (Rectangle) LogicalTreeHelper.FindLogicalNode(rootElement, "Rectangle");

            VerifyElement.VerifyBool(null == myRectangle, false, ref result);
            VerifyElement.VerifyColor(((SolidColorBrush)(((Shape) myRectangle).Stroke)).Color, Colors.Red, ref result);
            VerifyElement.VerifyDouble(((Shape) myRectangle).StrokeThickness, 3, ref result);
            VerifyElement.VerifyDouble(myRectangle.Width, 100, ref result);
            VerifyElement.VerifyDouble(myRectangle.Height, 50, ref result);
            VerifyElement.VerifyDouble(Canvas.GetTop(myRectangle), 50, ref result);
            VerifyElement.VerifyDouble(Canvas.GetLeft(myRectangle), 450, ref result);

            return result;
        }
    }
}
