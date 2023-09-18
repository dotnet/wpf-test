// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class GeometryDrawingTest : CoreGraphicsTest
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
                TestBrush();
                TestGeometry();
                TestPen();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor GeometryDrawing()...");

            TestConstructorWith();

            Log("Testing Constructor GeometryDrawing( Brush, Pen, Geometry )...");

            TestConstructorWith(Brushes.Red, new Pen(Brushes.Red, 1.5), new LineGeometry(new Point(-10, -10), new Point(10, 10)));
            TestConstructorWith(Const2D.linearGradientBrush1, new Pen(Const2D.linearGradientBrush1, 1.5), new EllipseGeometry(new Point(10, 10), 10, 10));
            TestConstructorWith(Const2D.radialGradientBrush1, new Pen(Const2D.radialGradientBrush1, 1.5), new RectangleGeometry(new Rect(new Point(-1.5, -1.5), new Point(1.5, 1.5))));
            TestConstructorWith(Const2D.imageBrush1, new Pen(Const2D.imageBrush1, 1.5), new CombinedGeometry(GeometryCombineMode.Exclude, new LineGeometry(), new LineGeometry(), new RotateTransform(45)));
            TestConstructorWith(Const2D.drawingBrush1, new Pen(Const2D.drawingBrush1, 1.5), new GeometryGroup());
            TestConstructorWith(Const2D.visualBrush1, new Pen(Const2D.visualBrush1, 1.5), new PathGeometry());
        }

        private void TestConstructorWith()
        {
            GeometryDrawing theirAnswer = new GeometryDrawing();
            if (!ObjectUtils.Equals(theirAnswer.Brush, null) ||
                !ObjectUtils.Equals(theirAnswer.Pen, null) ||
                !ObjectUtils.Equals(theirAnswer.Geometry, null) ||
                failOnPurpose)
            {
                AddFailure("Constructor GeometryDrawing() failed");
                Log("*** Expected: GeometryDrawing.Brush = {0}, GeometryDrawing.Pen = {1}, GeometryDrawing.Geometry = {2}", null, null, null);
                Log("*** Actual:   GeometryDrawing.Brush = {0}, GeometryDrawing.Pen = {1}, GeometryDrawing.Geometry = {2}", theirAnswer.Brush, theirAnswer.Pen, theirAnswer.Geometry);
            }
        }

        private void TestConstructorWith(Brush brush, Pen pen, Geometry geometry)
        {
            GeometryDrawing theirAnswer = new GeometryDrawing(brush, pen, geometry);

            if (!ObjectUtils.Equals(theirAnswer.Brush, brush) ||
                !ObjectUtils.Equals(theirAnswer.Pen, pen) ||
                !ObjectUtils.Equals(theirAnswer.Geometry, geometry) ||
                failOnPurpose)
            {
                AddFailure("Constructor GeometryDrawing( Brush, Pen, Geometry ) failed");
                Log("***Expected: GeometryDrawing.Brush = {0}, GeometryDrawing.Pen = {1}, GeometryDrawing.Geometry = {2}", brush, pen, geometry);
                Log("***Actual:   GeometryDrawing.Brush = {0}, GeometryDrawing.Pen = {1}, GeometryDrawing.Geometry = {2}", theirAnswer.Brush, theirAnswer.Pen, theirAnswer.Geometry);
                return;
            }
        }

        private void TestBrush()
        {
            Log("Testing get/set Brush Property...");

            TestBrushWith(Brushes.Red);
            TestBrushWith(new SolidColorBrush(Colors.Blue));
            TestBrushWith(Const2D.linearGradientBrush1);
            TestBrushWith(Const2D.radialGradientBrush1);
            TestBrushWith(Const2D.imageBrush1);
            TestBrushWith(Const2D.drawingBrush1);
            TestBrushWith(Const2D.visualBrush1);
        }

        private void TestBrushWith(Brush brush)
        {
            GeometryDrawing theirAnswer = new GeometryDrawing();
            theirAnswer.Brush = brush;

            if (!ObjectUtils.Equals(theirAnswer.Brush, brush) || failOnPurpose)
            {
                AddFailure("get/set Brush Property failed");
                Log("***Expected: GeometryDrawing.Brush = {0}", brush);
                Log("***Actual:   GeometryDrawing.Brush = {0}", theirAnswer.Brush);
                return;
            }
        }

        private void TestPen()
        {
            Log("Testing get/set Pen Property...");

            TestPenWith(new Pen(Brushes.Red, 1.5));
            TestPenWith(new Pen(new SolidColorBrush(Colors.Blue), 1.5));
            TestPenWith(new Pen(Const2D.linearGradientBrush1, 1.5));
            TestPenWith(new Pen(Const2D.radialGradientBrush1, 1.5));
            TestPenWith(new Pen(Const2D.imageBrush1, 1.5));
            TestPenWith(new Pen(Const2D.drawingBrush1, 1.5));
            TestPenWith(new Pen(Const2D.visualBrush1, 1.5));
        }

        private void TestPenWith(Pen pen)
        {
            GeometryDrawing theirAnswer = new GeometryDrawing();

            theirAnswer.Pen = pen;

            if (!ObjectUtils.Equals(theirAnswer.Pen, pen) || failOnPurpose)
            {
                AddFailure("get/set Pen Property failed");
                Log("***Expected: GeometryDrawing.Pen = {0}", pen);
                Log("***Actual:   GeometryDrawing.Pen = {0}", theirAnswer.Pen);
                return;
            }
        }

        private void TestGeometry()
        {
            Log("Testing get/set Geometry Property...");

            TestGeometryWith(new LineGeometry(new Point(-5, -5), new Point(5, 5)));
            TestGeometryWith(new EllipseGeometry(new Point(10, 10), 10, 10));
            TestGeometryWith(new RectangleGeometry(new Rect(new Point(-1.5, -1.5), new Point(1.5, 1.5))));
            TestGeometryWith(new CombinedGeometry(GeometryCombineMode.Exclude, new LineGeometry(), new LineGeometry(), new RotateTransform(45)));
            TestGeometryWith(new GeometryGroup());
            TestGeometryWith(new PathGeometry());
        }

        private void TestGeometryWith(Geometry geometry)
        {
            GeometryDrawing theirAnswer = new GeometryDrawing();

            theirAnswer.Geometry = geometry;

            if (!ObjectUtils.Equals(theirAnswer.Geometry, geometry) || failOnPurpose)
            {
                AddFailure("get/set Geometry Property failed");
                Log("***Expected: GeometryDrawing.Geoemtry = {0}", geometry);
                Log("***Actual:   GeometryDrawing.Geometry = {0}", theirAnswer.Geometry);
                return;
            }
        }

        private void RunTest2()
        {
            TestConstructor2();
            TestBrush2();
            TestGeometry2();
            TestPen2();
        }

        private void TestConstructor2()
        {
            Log("P2 Testing Constructor GeometryDrawing( Brush, Pen, Geometry )...");

            TestConstructorWith(null, new Pen(Brushes.Green, 1.5), new LineGeometry(new Point(-10, -10), new Point(10, 10))); // null Brush
            TestConstructorWith(Brushes.Red, null, new LineGeometry(new Point(-10, -10), new Point(10, 10))); // null Pen
            TestConstructorWith(Brushes.Red, new Pen(Brushes.Green, 1.5), null); // null Geometry
            TestConstructorWith(null, null, new LineGeometry(new Point(-10, -10), new Point(10, 10))); // null Brush and Pen
            TestConstructorWith(null, null, null); // all null
        }

        private void TestBrush2()
        {
            Log("P2 Testing get/set Brush Property...");

            TestBrushWith(null);
        }

        private void TestGeometry2()
        {
            Log("P2 Testing get/set Geometry Property...");

            TestGeometryWith(null);
            TestGeometryWith(new RectangleGeometry(Rect.Empty));
        }

        private void TestPen2()
        {
            Log("P2 Testing get/set Pen Property...");

            TestPenWith(null);
            TestPenWith(new Pen(null, 1.5));
            TestPenWith(new Pen(null, 0));
        }
    }
}