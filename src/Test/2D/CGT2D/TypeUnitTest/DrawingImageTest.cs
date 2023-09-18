// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class DrawingImageTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            if (priority > 0)
            {
                RunTest2();
            }
            else
            {
                TestConstructor();
                TestDrawing();
                TestWidthHeight();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor DrawingImage()...");

            TestConstructorWith();

            Log("Testing Constructor DrawingImage( Drawing )...");

            TestConstructorWith(new GeometryDrawing());
            TestConstructorWith(Const2D.geometryDrawing1);
            TestConstructorWith(new ImageDrawing());
            TestConstructorWith(Const2D.imageDrawing1);
            TestConstructorWith(new GlyphRunDrawing());
            TestConstructorWith(Const2D.glyphRunDrawing1);
            TestConstructorWith(new DrawingGroup());
            TestConstructorWith(Const2D.drawingGroup1);
        }



        private void TestConstructorWith()
        {
            DrawingImage theirAnswer = new DrawingImage();

            if (!ObjectUtils.Equals(theirAnswer.Drawing, null) || failOnPurpose)
            {
                AddFailure("Constructor DrawingImage() failed");
                Log("*** Expected: DrawingImage.Drawing = {0}", null);
                Log("*** Actual:   DrawingImage.Drawing = {0}", theirAnswer.Drawing);
            }
        }

        private void TestConstructorWith(Drawing drawing)
        {
            DrawingImage theirAnswer = new DrawingImage(drawing);

            if (!ObjectUtils.Equals(theirAnswer.Drawing, drawing) || failOnPurpose)
            {
                AddFailure("Constructor DrawingImage( Drawing ) failed");
                Log("***Expected: DrawingImage.Drawing = {0}", drawing);
                Log("***Actual:   DrawingImage.Drawing = {0}", theirAnswer.Drawing);
            }
        }

        private void TestDrawing()
        {
            Log("Testing get/set Drawing Property...");

            TestDrawingWith(new GeometryDrawing());
            TestDrawingWith(Const2D.geometryDrawing1);
            TestDrawingWith(new ImageDrawing());
            TestDrawingWith(Const2D.imageDrawing1);
            TestDrawingWith(new GlyphRunDrawing());
            TestDrawingWith(Const2D.glyphRunDrawing1);
            TestDrawingWith(new DrawingGroup());
            TestDrawingWith(Const2D.drawingGroup1);
        }

        private void TestDrawingWith(Drawing drawing)
        {
            DrawingImage theirAnswer = new DrawingImage();
            theirAnswer.Drawing = drawing;

            if (!ObjectUtils.Equals(theirAnswer.Drawing, drawing) || failOnPurpose)
            {
                AddFailure("get/set Drawing failed");
                Log("***Expected: DrawingImage.Drawing = {0}", drawing);
                Log("***Actual:   DrawingImage.Drawing = {0}", theirAnswer.Drawing);
            }
        }

        private void TestWidthHeight()
        {
            Log("Testing Width, Height Properties...");

            TestWidthHeightWith(new GeometryDrawing());
            TestWidthHeightWith(Const2D.geometryDrawing1);
            TestWidthHeightWith(new ImageDrawing());
            TestWidthHeightWith(Const2D.imageDrawing1);
            TestWidthHeightWith(new GlyphRunDrawing());
            TestWidthHeightWith(Const2D.glyphRunDrawing1);
            TestWidthHeightWith(new DrawingGroup());
            TestWidthHeightWith(Const2D.drawingGroup1);
        }

        private void TestWidthHeightWith(Drawing drawing)
        {
            DrawingImage drawingImage = new DrawingImage(drawing);
            double theirWidth = drawingImage.Width;
            double theirHeight = drawingImage.Height;

            double myWidth;
            double myHeight;
            if (ObjectUtils.Equals(drawing, null) || drawing.Bounds.IsEmpty)
            {
                myWidth = 0;
                myHeight = 0;
            }
            else
            {
                myWidth = drawing.Bounds.Width;
                myHeight = drawing.Bounds.Height;
            }

            if (!MathEx.Equals(theirWidth, myWidth) || !MathEx.Equals(theirHeight, myHeight) || failOnPurpose)
            {
                AddFailure("Width, Height properties failed");
                Log("***Expected: DrawingImage.Width = {0}, DrawingImage.Height = {1}", myWidth, myHeight);
                Log("***Actual:   DrawingImage.Width = {0}, DrawingImage.Height = {1}", theirWidth, theirHeight);
            }
        }

        private void RunTest2()
        {
            TestConstructor2();
            TestDrawing2();
            TestWidthHeight2();
        }

        private void TestConstructor2()
        {
            Log("P2 Testing Constructor DrawingImage( Drawing )...");

            TestConstructorWith(null);
            TestConstructorWith(Const2D.drawingGroupNullChildren);
        }

        private void TestDrawing2()
        {
            Log("P2 Testing get/set Drawing Property...");

            TestDrawingWith(null);
            TestDrawingWith(Const2D.drawingGroupNullChildren);
        }

        private void TestWidthHeight2()
        {
            Log("P2 Testing Width, Height Properties...");

            TestWidthHeightWith(null);
            TestWidthHeightWith(Const2D.drawingGroupNullChildren);
        }
    }
}