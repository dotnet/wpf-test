// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DRT;
using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace DRTMil2D
{
    public sealed class DRTMil2DDrawingSuite : DrtTestSuite
    {
        public class TestDrawingVisual : DrawingVisual
        {
            private Rect _location;

            public TestDrawingVisual()
            {
                _location = m_screenCells[0];

                using (DrawingContext ctx = RenderOpen())
                {
                    ctx.DrawRectangle(
                        Brushes.HotPink,
                        null,
                        _location
                        );
                }
            }
        }

        public class TestHitTestDrawingVisual : DrawingVisual
        {
            public TestHitTestDrawingVisual(double size, bool insertClip)
            {
                using (DrawingContext ctx = RenderOpen())
                {
                    //
                    // Layout (x = square, . = gap): .R.G.B.O
                    //
                    // if insertClip == true, B will be removed.
                    //

                    Rect square = new Rect(0,0,size,size);

                    ctx.PushTransform(new TranslateTransform(size,0));
                    ctx.DrawRectangle(
                        Brushes.Red,
                        null,
                        square
                        );
                    ctx.PushTransform(new TranslateTransform(2*size,0));

                    if (insertClip)
                    {
                        ctx.PushClip(new RectangleGeometry(square));
                    }

                    ctx.PushTransform(new TranslateTransform(2*size,0));
                    ctx.DrawRectangle(
                        Brushes.Blue,
                        null,
                        square
                        );

                    ctx.Pop(); // transform

                    ctx.DrawRectangle(
                        Brushes.Green,
                        null,
                        square
                        );

                    if (insertClip)
                    {
                        ctx.Pop(); // clip
                    }

                    ctx.Pop(); // transform
                    ctx.Pop(); // transform

                    ctx.PushTransform(new TranslateTransform(7*size,0));
                    ctx.DrawRectangle(
                        Brushes.Orange,
                        null,
                        square
                        );
                    ctx.Pop(); // transform
                }
            }
        }

        internal static void FillDrawingContext(DrawingContext drawingContext, Rect destination)
        {
            // The Draw calls in FillDrawingContext Draw into the (0,0,1,1) unit
            // square.  To place the content into the destination, we apply
            // a rectangle transform
            drawingContext.PushTransform(
                new MatrixTransform(
                    DRTMil2D.MatrixRectangleTransform(
                        new Rect(0,0,1,1),
                        destination
                        ))
                );

            FillDrawingContext(drawingContext);

            // Pop rectangle transform
            drawingContext.Pop();
        }

        internal static void FillDrawingContext(DrawingContext drawingContext)
        {
            FillDrawingContext(drawingContext, 1.0);
        }

        internal static void FillDrawingContextWithLargeUnits(DrawingContext drawingContext)
        {
            FillDrawingContext(drawingContext, 1000.0);
        }

        internal static void FillDrawingContext(DrawingContext drawingContext, double scaleFactor)
        {
            // Draw checkered green background squares
            drawingContext.DrawRectangle(
                Brushes.Green,
                null,
                new Rect(0.0, 0.0, 0.5 * scaleFactor, 0.5 * scaleFactor));

            drawingContext.DrawRectangle(
                Brushes.Green,
                null,
                new Rect(0.5 * scaleFactor, 0.5 * scaleFactor, 0.5 * scaleFactor, 0.5 * scaleFactor));

            // Draw large yellow 'face' circle
            drawingContext.DrawEllipse(
                Brushes.Yellow,
                new Pen(Brushes.Black, 0.02 * scaleFactor),
                new Point(0.5 * scaleFactor, 0.5 * scaleFactor),
                0.4 * scaleFactor, 0.4 * scaleFactor);

            // Draw the 'smile'

            PathFigure smileFigure = new PathFigure();

            smileFigure.StartPoint = new Point(0.25 * scaleFactor, 0.6 * scaleFactor);
            smileFigure.Segments.Add(new BezierSegment(
                new Point(0.4 * scaleFactor, 0.75 * scaleFactor),
                new Point(0.6 * scaleFactor, 0.75 * scaleFactor),
                new Point(0.75 * scaleFactor, 0.6 * scaleFactor),
                true
                ));

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(smileFigure);

            drawingContext.DrawGeometry(
                null,
                new Pen(Brushes.Black, 0.02 * scaleFactor),
                geometry);

            // Draw the 'eyes'
            drawingContext.DrawEllipse(
                Brushes.Black,
                null,
                new Point(0.4 * scaleFactor, 0.4 * scaleFactor),
                0.03 * scaleFactor, 0.125 * scaleFactor);

            drawingContext.DrawEllipse(
                Brushes.Black,
                null,
                new Point(0.6 * scaleFactor, 0.4 * scaleFactor),
                0.03 * scaleFactor, 0.125 * scaleFactor);
        }

        void TestDrawingContext(DrawingContext drawingContext)
        {
            BitmapSource image = BitmapFrame.Create(new Uri(@"DrtFiles\DrtMil2D\tulip.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
            Rect r;

            TimeSpan duration = new TimeSpan(0,0,DRTMil2D.AnimationLength);
            Brush brush = Brushes.Blue;
            Pen pen = new Pen(brush, 10);

            r = DRTMil2D.ResizeRectangle(m_screenCells.Next, 0.75, 0.75);

            //
            //
            // Test stack operations
            //
            //

            //
            // Test popping without pushing
            //
            {
                bool exceptionThrown = false;

                try
                {
                    drawingContext.Pop();
                }
                catch(InvalidOperationException)
                {
                    exceptionThrown = true;
                }

                DRT.Assert(exceptionThrown);
            }

            //
            // Test popping too many times after pushing once
            //
            {
                bool exceptionThrown = false;

                try
                {
                    drawingContext.PushOpacity(0.5);
                    drawingContext.Pop();
                    drawingContext.Pop();
                }
                catch(InvalidOperationException)
                {
                    exceptionThrown = true;
                }

                DRT.Assert(exceptionThrown);
            }

            //
            // Test drawing after pushing a null clip & transform
            //
            {
                drawingContext.PushClip(null);
                drawingContext.PushTransform(null);
                drawingContext.DrawRectangle(
                    new LinearGradientBrush(Colors.Blue, Colors.Red, 45),
                    null,
                    m_screenCells.Next
                    );

                drawingContext.Pop();
                drawingContext.Pop();
            }

            //
            // Test drawing after pushing a null clip
            //
            {
                drawingContext.PushClip(null);
                drawingContext.DrawRectangle(
                    new LinearGradientBrush(Colors.Blue, Colors.Red, 45),
                    null,
                    m_screenCells.Next
                    );

                drawingContext.Pop();
            }

            //
            // Test drawing after pushing a null transform
            //
            {
                drawingContext.PushTransform(null);
                drawingContext.DrawRectangle(
                    new LinearGradientBrush(Colors.Blue, Colors.Red, 45),
                    null,
                    m_screenCells.Next
                    );

                drawingContext.Pop();
            }

            //
            //
            // Test every Draw* API
            //
            //


            //
            // Test DrawLine
            //

            // Test non-animate DrawLine
            drawingContext.DrawLine(
                pen,
                r.TopLeft,
                r.BottomRight
                );

            r = DRTMil2D.ResizeRectangle(m_screenCells.Next, 0.75, 0.75);

            PointAnimation point0Animation = new PointAnimation(r.TopLeft, r.TopRight, duration);
            point0Animation.RepeatBehavior = RepeatBehavior.Forever;
            point0Animation.AutoReverse = true;

            PointAnimation point1Animation = new PointAnimation(r.BottomRight, r.BottomLeft, duration);
            point1Animation.RepeatBehavior = RepeatBehavior.Forever;
            point1Animation.AutoReverse = true;

            // Test animate DrawLine
            drawingContext.DrawLine(
                pen,
                r.TopLeft,
                (AnimationClock) point0Animation.CreateClock(),
                r.BottomRight,
                (AnimationClock) point1Animation.CreateClock()
                );

            //
            // Test DrawRectangle
            //

            // Test PushOpacity
            drawingContext.PushOpacity(0.5);

            // Test non-animate DrawRectangle
            drawingContext.DrawRectangle(
                brush,
                null,
                m_screenCells.Next
                );

            drawingContext.Pop();

            // Test animate PushOpacity

            DoubleAnimation opacityAnimation = new DoubleAnimation(1.0, 0.5, duration);
            opacityAnimation.RepeatBehavior = RepeatBehavior.Forever;
            opacityAnimation.AutoReverse = true;

            drawingContext.PushOpacity(
                1.0,
                (AnimationClock) opacityAnimation.CreateClock()
                );

            // Test animate Draw Rectangle
            r = m_screenCells.Next;
            RectAnimation rectAnimation = new RectAnimation(
                r,
                DRTMil2D.ResizeRectangle(r, 0.75, 0.75),
                duration
                );
            rectAnimation.RepeatBehavior = RepeatBehavior.Forever;
            rectAnimation.AutoReverse = true;

            drawingContext.DrawRectangle(
                brush,
                null,
                r,
                (AnimationClock) rectAnimation.CreateClock()
                );

            drawingContext.Pop();

            //
            // Test DrawRoundedRectangle
            //

            // Test non-animate version
            r = m_screenCells.Next;
            double radiusX = r.Width / 3;
            double radiusY = r.Height / 3;

            drawingContext.DrawRoundedRectangle(
                brush,
                null,
                r,
                radiusX,
                radiusY
                );

            // Test animate version
            r = m_screenCells.Next;
            rectAnimation = new RectAnimation(
                r,
                DRTMil2D.ResizeRectangle(r, 0.75, 0.75),
                duration
                );
            rectAnimation.RepeatBehavior = RepeatBehavior.Forever;
            rectAnimation.AutoReverse = true;

            radiusX = r.Width / 3;
            radiusY = r.Height / 3;
            double radiusXTo = (r.Width * 0.75) / 3;
            double radiusYTo = (r.Height * 0.75) / 3;

            DoubleAnimation radiusXAnimation = new DoubleAnimation(radiusX, radiusXTo, duration);
            radiusXAnimation.RepeatBehavior = RepeatBehavior.Forever;
            radiusXAnimation.AutoReverse = true;

            DoubleAnimation radiusYAnimation = new DoubleAnimation(radiusY, radiusYTo, duration);
            radiusYAnimation.RepeatBehavior = RepeatBehavior.Forever;
            radiusYAnimation.AutoReverse = true;

            drawingContext.DrawRoundedRectangle(
                brush,
                null,
                r,
                (AnimationClock) rectAnimation.CreateClock(),
                radiusX,
                (AnimationClock) radiusXAnimation.CreateClock(),
                radiusY,
                (AnimationClock) radiusYAnimation.CreateClock()
                );

            //
            // Test DrawEllipse
            //

            // Test non-animate version
            r = m_screenCells.Next;
            drawingContext.DrawEllipse(
                brush,
                null,
                DRTMil2D.CalculateCenter(r),
                r.Width / 2,
                r.Height / 2
                );

            // Test animate version

            r = m_screenCells.Next;
            Point center = DRTMil2D.CalculateCenter(r);
            PointAnimation centerAnimation = new PointAnimation(
                center,
                new Point(r.Right, r.Top),
                duration
                );

            centerAnimation.RepeatBehavior = RepeatBehavior.Forever;
            centerAnimation.AutoReverse = true;

            radiusX = r.Width / 2;
            radiusY = r.Height / 2;

            radiusXAnimation = new DoubleAnimation(radiusX, 0, duration);
            radiusXAnimation.RepeatBehavior = RepeatBehavior.Forever;
            radiusXAnimation.AutoReverse = true;

            radiusYAnimation = new DoubleAnimation(radiusY, 0, duration);
            radiusYAnimation.RepeatBehavior = RepeatBehavior.Forever;
            radiusYAnimation.AutoReverse = true;

            drawingContext.DrawEllipse(
                brush,
                null,
                center,
                (AnimationClock) centerAnimation.CreateClock(),
                radiusX,
                (AnimationClock) radiusXAnimation.CreateClock(),
                radiusY,
                (AnimationClock) radiusYAnimation.CreateClock()
                );

            //
            // Test DrawGeometry
            //

            // Test PushTransform
            r = m_screenCells.Next;
            drawingContext.PushTransform(
                new ScaleTransform(0.5, 0.5, r.TopLeft.X, r.TopLeft.Y)
                );

            drawingContext.DrawGeometry(
                brush,
                null,
                new RectangleGeometry(r)
                );

            drawingContext.Pop();

            //
            // Test DrawImage
            //

            // Test PushClip
            r = m_screenCells.Next;
            drawingContext.PushClip(
                new RectangleGeometry(DRTMil2D.ResizeRectangle(r, 0.5, 0.5))
                );

            // Test non-animate DrawImage
            drawingContext.DrawImage(
                image,
                r
                );

            drawingContext.Pop();


            r = m_screenCells.Next;
            rectAnimation = new RectAnimation(r, DRTMil2D.ResizeRectangle(r, 0.75, 0.75), duration);
            rectAnimation.RepeatBehavior = RepeatBehavior.Forever;
            rectAnimation.AutoReverse = true;

            // Test animate DrawImage
            drawingContext.DrawImage(
                image,
                r,
                (AnimationClock) rectAnimation.CreateClock()
                );

            //
            // Test DrawGlyphs
            //

            _ctx.PushTransform(
                new MatrixTransform(
                    DRTMil2D.MatrixRectangleTransform(
                        _glyphRun.ComputeInkBoundingBox(),
                        m_screenCells.Next
                        )
                ));

            _ctx.DrawGlyphRun(Brushes.Yellow, _glyphRun);

            _ctx.Pop();

            //
            // Test a DrawDrawing
            //
            {

                r = m_screenCells.Next;

                DrawingGroup drawingGroup  = new DrawingGroup();
                DrawingContext ddc = drawingGroup.Open();

                FillDrawingContext(ddc, new Rect(0, 0, r.Width/2, r.Height/2));
                ddc.Close();

                center = DRTMil2D.CalculateCenter(r);
                drawingGroup.Transform = new TranslateTransform(center.X, center.Y);

                drawingContext.DrawDrawing(
                   drawingGroup
                   );
            }

        }

        internal static Canvas LayoutShape(Shape shapeToLayout)
        {
            // Create a Canvas to support Canvas.Top & Canvas.Left
            Canvas container = new Canvas();
            container.Children.Add(shapeToLayout);

            // Layout the shape
            container.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            container.Arrange(new Rect(container.DesiredSize));
            container.UpdateLayout();

            return container;
        }

        private void Render(DrawingContext ctx)
        {
            _ctx = ctx;

            m_screenCells = new AreaPartitioner(
                DRTMil2D.WindowWidth,
                DRTMil2D.WindowHeight,
                40,
                30
                );

            // Reserve 1st cell for empty drawings
            m_screenCells.Reserve(1);

            BitmapSource tulipImage = BitmapFrame.Create(new Uri(@"DrtFiles\DrtMil2D\tulip.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);

            RunDrawingObjectTests();

            //
            // Test Copying a Drawing with no depenedent resources.
            //
            {
                DrawingGroup drawing = new DrawingGroup();
                DrawingContext dc = drawing.Open();

                // Push opacity is the only drawing instruction that will create a RenderData
                // without passing in a Freezable.  E.g., the Draw* instructions won't
                // create the RenderData if both the brush & pen are null.
                dc.PushOpacity(0.5);
                dc.Pop();

                dc.Close();

                // Test copying of RenderData with no resources.
                drawing.Clone();
            }

            // Test DrawingVisual that remains as RenderData
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext dc = visual.RenderOpen();

                TestDrawingContext(dc);

                dc.Close();

                // Don't call .DrawingContent so that content remains RenderData

                // Test Visual.Clip
                visual.Clip = new RectangleGeometry(visual.ContentBounds);

                VisualCollection children = _rootVisual.Children;
                children.Add(visual);
            }

            // Test RenderData->DrawingGroup conversion via
            // DrawingVisual.DrawingContent
            {

                //
                // Create & populate a DrawingVisual
                //

                DrawingVisual visual = new DrawingVisual();

                DrawingContext dc = visual.RenderOpen();

                TestDrawingContext(dc);

                dc.Close();

                //
                // Test DrawingVisual.Drawing and conversion of RenderData to Drawing
                //

                DrawingGroup group = visual.Drawing;

                // We populated content, so the content returned should not be null
                DRT.Assert (group != null);

                //
                // Test VisualTreeHelper.GetDrawing on a DrawingVisual
                //

                group = VisualTreeHelper.GetDrawing(visual);

                // We populated content, so the content returned should not be null
                DRT.Assert(group != null);

                //
                // Add this visual the the root visual
                //

                VisualCollection children = _rootVisual.Children;
                children.Add(visual);
            }

            // Test DrawingVisual.DrawingContent
            {
                DrawingVisual visual = new TestDrawingVisual();

                // Test DrawingVisual.GetDrawing
                DrawingGroup content = VisualTreeHelper.GetDrawing(visual);

                // We populated content, so the content returned should not be null
                DRT.Assert(content != null);
            }

            //
            // Test clip with a non-null and null clip on the stack
            //
            {
                DrawingGroup drawing = new DrawingGroup();
                DrawingContext dc = drawing.Open();

                dc.PushClip(new RectangleGeometry(new Rect(0.25, 0.25, 0.5, 0.5)));
                dc.PushClip(null);
                dc.DrawRectangle(
                    new LinearGradientBrush(Colors.Blue, Colors.Red, 45),
                    null,
                    new Rect(0,0,1,1)
                    );

                dc.Pop();
                dc.Pop();

                dc.Close();

                _ctx.DrawRectangle(new DrawingBrush(drawing), null, m_screenCells.Next);

                // Get bounds to test BoundsDrawingContextWalker with this drawing
                Rect r = drawing.Bounds;
                DRT.Assert (r == new Rect(0.25,0.25,0.5,0.5));
            }

            //
            // Test clip with a non-null and null transform on the stack
            //
            {
                DrawingGroup drawing = new DrawingGroup();
                DrawingContext dc = drawing.Open();

                dc.PushTransform(new ScaleTransform(2.0, 2.0));
                dc.PushTransform(null);
                dc.DrawRectangle(
                    new LinearGradientBrush(Colors.Blue, Colors.Red, 45),
                    null,
                    new Rect(0,0,1,1)
                    );

                dc.Pop();
                dc.Pop();

                dc.Close();

                _ctx.DrawRectangle(new DrawingBrush(drawing), null, m_screenCells.Next);

                // Get bounds to test BoundsDrawingContextWalker with this drawing
                Rect r = drawing.Bounds;
                Console.WriteLine(r.ToString());
                DRT.Assert (r == new Rect(0,0,2,2));
            }

            //
            // Test hit-testing with null transforms & clips
            //
            {
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext dc = drawingVisual.RenderOpen();
                Rect cell = m_screenCells.Next;

                dc.PushClip(null);
                dc.PushTransform(null);
                dc.DrawRectangle(
                    new LinearGradientBrush(Colors.Yellow, Colors.Red, 45),
                    null,
                    cell
                    );

                dc.Pop();
                dc.Pop();

                dc.Close();

                // Get bounds to test BoundsDrawingContextWalker with this drawing
                Rect r = drawingVisual.ContentBounds;
                DRT.Assert(r == cell);

                PointHitTestResult hitTest = (PointHitTestResult) drawingVisual.HitTest(cell.TopLeft);
                DRT.Assert((hitTest != null) &&
                            (hitTest.PointHit == cell.TopLeft));

                VisualCollection children = _rootVisual.Children;
                children.Add(drawingVisual);
            }

            //
            // Test hit-testing with null and non-null transforms & clips
            //
            {
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext dc = drawingVisual.RenderOpen();
                Rect cell = m_screenCells.Next;
                Rect clip = DRTMil2D.ResizeRectangle(cell, 0.75, 0.75);

                dc.PushClip(new RectangleGeometry(clip));
                dc.PushClip(null);
                dc.PushTransform(new ScaleTransform(2.0, 2.0, cell.TopLeft.X, cell.TopLeft.Y));
                dc.PushTransform(null);
                dc.DrawRectangle(
                    new LinearGradientBrush(Colors.Yellow, Colors.Red, 45),
                    null,
                    cell
                    );

                dc.Pop();
                dc.Pop();
                dc.Pop();
                dc.Pop();

                dc.Close();

                // Get bounds to test BoundsDrawingContextWalker with this drawing
                Rect r = drawingVisual.ContentBounds;
                Console.WriteLine(r.ToString());
                DRT.Assert(r == clip);

                PointHitTestResult hitTest = (PointHitTestResult) drawingVisual.HitTest(clip.TopLeft);
                DRT.Assert((hitTest != null) &&
                            (hitTest.PointHit == clip.TopLeft));

                VisualCollection children = _rootVisual.Children;
                children.Add(drawingVisual);
            }

            // Test OpacityMask
            {
                Rect r = m_screenCells.Next;
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext dc = drawingVisual.RenderOpen();

                FillDrawingContext(dc, r);
                dc.Close();

                drawingVisual.OpacityMask = new LinearGradientBrush(
                    Colors.White,
                    Colors.Transparent,
                    90
                    );

                VisualCollection children = _rootVisual.Children;
                children.Add(drawingVisual);
            }


            //
            // Test all rectangle primitives with Rect.Empty
            //
            {
                // Common values used by tests
                VisualCollection children = _rootVisual.Children;

                // These tests shouldn't render anything, so use a noticable color in-case
                // they do
                Brush fillBrush = Brushes.Pink;
                Rect r = Rect.Empty;

                // Shapes.Rectangle does now allow Rect.Empty because
                // it derives its properties from FrameworkElement

                // Test DrawRectangle w/ Rect.Empty
                _ctx.DrawRectangle(fillBrush, null, r);

                // Test RectangleGeometry w/ Rect.Empty
                _ctx.DrawGeometry(fillBrush, null, new RectangleGeometry(r));
            }

            //
            // Test all rectangle primitives with negative radii
            //
            {
                // Common values used by tests
                VisualCollection children = _rootVisual.Children;

                Brush fillBrush = Brushes.Orange;
                Rect r = m_screenCells.Next;
                double radiusX = r.Width / -3;  // Negative radiusX
                double radiusY = r.Height / -3; // Negative radiusY

                // Test Shapes.Rectangle with negative radii
                Rectangle rectangleShape = new Rectangle();
                rectangleShape.Fill = fillBrush;

                Canvas.SetTop(rectangleShape, r.Top);
                Canvas.SetLeft(rectangleShape, r.Left);
                rectangleShape.Width = r.Width;
                rectangleShape.Height = r.Height;

                rectangleShape.RadiusX = radiusX;
                rectangleShape.RadiusY = radiusY;

                children.Add(LayoutShape(rectangleShape));

                // Test DrawRoundedRectangle w/ negative radii
                r = m_screenCells.Next;
                _ctx.DrawRoundedRectangle(fillBrush, null, r, radiusX, radiusY);

                // Test RectangleGeometry w/ negative radii
                r = m_screenCells.Next;
                _ctx.DrawGeometry(
                    fillBrush,
                    null,
                    new RectangleGeometry(r, radiusX, radiusY)
                    );

            }

            //
            // Test RectangleGeometry bounds with negative radii
            //
            {
                Rect r = new Rect (0, 0, 50, 50);
                double radius = 24;

                Pen pen = new Pen(Brushes.Black, 0.5);

                Transform transform = new RotateTransform(45.0);

                RectangleGeometry positiveRadii = new RectangleGeometry(
                    r,
                    radius,
                    radius
                    );

                // The radii won't affect the bounding calculation unless the
                // geometry is under a rotation or skew
                positiveRadii.Transform = transform;

                RectangleGeometry negativeRadii = new RectangleGeometry(
                    r,
                    -radius,
                    -radius
                    );

                // The radii won't affect the bounding calculation unless the
                // geometry is under a rotation or skew
                negativeRadii.Transform = transform;

                // Test that the bounds of both geometries are the same
                DRT.Assert(positiveRadii.GetRenderBounds(pen) == negativeRadii.GetRenderBounds(pen));
            }

            //
            // Test EllipseGeometry bounds with negative radii
            //
            {
                Point center = new Point(25, 25);
                double radius = 24;

                Pen pen = new Pen(Brushes.Black, 0.5);

                EllipseGeometry positiveRadii = new EllipseGeometry(
                    center,
                    radius,
                    radius
                    );

                EllipseGeometry negativeRadii = new EllipseGeometry(
                    center,
                    -radius,
                    -radius
                    );

                // Test that the bounds of both geometries are the same
                DRT.Assert(positiveRadii.GetRenderBounds(pen) == negativeRadii.GetRenderBounds(pen));
            }

            //
            // Test that during a DrawingContext bounding walk, the animated values
            // of sub-properties are maintained.
            //
            {
                // First, create a DrawingGroup containing a 0, 0, 50, 50 filled rectangle
                DrawingGroup dg = new DrawingGroup();
                DrawingContext dc = dg.Open();

                dc.DrawRectangle(Brushes.Red, null, new Rect(0, 0, 50, 50));
                dc.Close();

                // Create a Transform with a 0,0 base value
                TranslateTransform transform = new TranslateTransform(0,0);

                // Create an constant 'animation' that will always evaluate to 25
                DoubleAnimation xAnimation = new DoubleAnimation(25, 25, new TimeSpan(0,0,0));
                xAnimation.FillBehavior = FillBehavior.HoldEnd;

                AnimationClock clock = xAnimation.CreateClock();

                // Add the 'animation' to the transform property
                transform.ApplyAnimationClock(TranslateTransform.XProperty, clock);

                // Before the transform is set, the Bounds.Left is 0
                DRT.Assert(dg.Bounds.Left == 0);

                // Apply the transform
                dg.Transform = transform;

                // Animations only begin at the next tick so the animated value won't
                // updated till then.  A workaround is to seek aligned to last tick.
                clock.Controller.SeekAlignedToLastTick(TimeSpan.Zero, TimeSeekOrigin.BeginTime);

                // The animated Transform.X property should shift the bounds by 25
                DRT.Assert(dg.Bounds.Left == 25);
            }

            //
            // Test PushOpacityMask
            //
            {
                Rect r = m_screenCells.Next;

                _ctx.DrawRectangle(
                    new LinearGradientBrush(Color.FromScRgb(1, 0, 0, 1), Color.FromScRgb(1, 0, 0.5f, 0), -45),
                    null,
                    r
                    );

                GeometryDrawing gd = new GeometryDrawing();
                StreamGeometry g = new StreamGeometry();

                using (StreamGeometryContext sctx = g.Open())
                {
                    sctx.BeginFigure(new Point(0,0), true, true);
                    sctx.LineTo(new Point(1,1), true, false);
                    sctx.LineTo(new Point(1,0), true, false);
                    sctx.LineTo(new Point(0,1), true, false);
                }

                gd.Geometry = g;
                gd.Brush = Brushes.White;

                DrawingBrush db = new DrawingBrush();
                db.Drawing = gd;

                _ctx.PushOpacityMask(db);

                Rect rFrom = r;
                rFrom.X += r.Width * 0.25;
                rFrom.Width *= 0.5;

                Rect rTo = r;
                rTo.Y += r.Height * 0.25;
                rTo.Height *= 0.5;

                RectAnimation rectAnimation = new RectAnimation(rFrom, rTo, new TimeSpan(0,0,DRTMil2D.AnimationLength));
                rectAnimation.RepeatBehavior = RepeatBehavior.Forever;
                rectAnimation.AutoReverse = true;

                _ctx.DrawRectangle(
                    new LinearGradientBrush(Color.FromScRgb(1, 1, 0, 0), Color.FromScRgb(1, 1, 1, 0), 90),
                    null, // Pen
                    r,
                    (AnimationClock) rectAnimation.CreateClock()
                    );

                _ctx.Pop();
            }

            //
            // Test nested PushOpacityMask
            //
            {
                Rect r = m_screenCells.Next;

                _ctx.DrawRectangle(
                    new LinearGradientBrush(Color.FromScRgb(1, 0, 0, 1), Color.FromScRgb(1, 0, 0.5f, 0), -45),
                    null,
                    r
                    );

                RadialGradientBrush rgb = new RadialGradientBrush(Color.FromScRgb(1,1,1,1),
                                                                  Color.FromScRgb(0,0,0,0));

                _ctx.PushOpacityMask(rgb);

                Rect rFrom = r;
                rFrom.X += r.Width * 0.25;
                rFrom.Width *= 0.5;

                Rect rTo = r;
                rTo.Y += r.Height * 0.25;
                rTo.Height *= 0.5;

                RectAnimation rectAnimation = new RectAnimation(rFrom, rTo, new TimeSpan(0,0,DRTMil2D.AnimationLength));
                rectAnimation.RepeatBehavior = RepeatBehavior.Forever;
                rectAnimation.AutoReverse = true;

                _ctx.DrawRectangle(
                    new LinearGradientBrush(Color.FromScRgb(1, 1, 0, 0), Color.FromScRgb(1, 1, 1, 0), 90),
                    null, // Pen
                    r,
                    (AnimationClock) rectAnimation.CreateClock()
                    );

                GeometryDrawing gd = new GeometryDrawing();
                StreamGeometry g = new StreamGeometry();

                using (StreamGeometryContext sctx = g.Open())
                {
                    sctx.BeginFigure(new Point(0,0), true, true);
                    sctx.LineTo(new Point(1,1), true, false);
                    sctx.LineTo(new Point(0,1), true, false);
                    sctx.LineTo(new Point(1,0), true, false);
                }

                gd.Geometry = g;
                gd.Brush = Brushes.White;

                DrawingBrush db = new DrawingBrush();
                db.Drawing = gd;

                _ctx.PushOpacityMask(db);

                Rect rInner = new Rect(rFrom.X, r.Y, rFrom.Width, r.Height);

                _ctx.DrawRectangle(
                    new RadialGradientBrush(Color.FromScRgb(1,0,1,0.5f),
                                            Color.FromScRgb(1,1,0.5f,0)),
                    null, // Pen
                    rInner,
                    null // Rect clock
                    );

                _ctx.Pop();

                _ctx.Pop();
            }

            RunDrawingConsistencyTests();

            RunRegressionTests();
        }

        private void RunDrawingObjectTests()
        {
            BitmapSource tulipImage = BitmapFrame.Create(new Uri(@"DrtFiles\DrtMil2D\tulip.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);

            //
            // Test simple Drawings
            //

            // Test simple GeometryDrawing
            {
                Rect r = m_screenCells.Next;
                GeometryDrawing geometryDrawing = new GeometryDrawing(
                    Brushes.LightGoldenrodYellow,
                    null,
                    new RectangleGeometry(r));

                _ctx.DrawDrawing(geometryDrawing);
            }

            // Test DrawingGroup
            {
                Rect r = m_screenCells.Next;

                DrawingGroup drawingGroup = new DrawingGroup();

                // Add first child

                GeometryDrawing geometryDrawing = new GeometryDrawing();
                geometryDrawing.Geometry = new RectangleGeometry(r);
                geometryDrawing.Brush = Brushes.LightGoldenrodYellow;
                drawingGroup.Children.Add(geometryDrawing);

                // Add second child
                Rect strokeRect = DRTMil2D.ResizeRectangle(r, 0.75, 0.75);
                geometryDrawing = new GeometryDrawing(
                    null,
                    new Pen(Brushes.Red, m_penThickness),
                    new LineGeometry(
                        new Point(strokeRect.Left, strokeRect.Top),
                        new Point(strokeRect.Right, strokeRect.Bottom))
                     );
                drawingGroup.Children.Add(geometryDrawing);

                // Draw DrawingGroup

                _ctx.DrawDrawing(drawingGroup);
            }

            // Test ImageDrawing
            {
                ImageDrawing imageDrawing = new ImageDrawing(
                    tulipImage,
                    m_screenCells.Next
                    );

                _ctx.DrawDrawing(imageDrawing);
            }

            // Test GlyphsDrawing
            {
                _ctx.PushTransform(
                    new MatrixTransform(
                        DRTMil2D.MatrixRectangleTransform(
                            _glyphRun.ComputeInkBoundingBox(),
                            m_screenCells.Next
                            )
                    ));

                GlyphRunDrawing glyphRunDrawing = new GlyphRunDrawing(Brushes.Yellow, _glyphRun);

                _ctx.DrawDrawing(glyphRunDrawing);

                _ctx.Pop();
            }

            //
            // Test DrawingGroup properties
            //

            // Test Opacity
            {
                Rect r = m_screenCells.Next;
                GeometryDrawing geometryDrawing = new GeometryDrawing(
                    new ImageBrush(tulipImage),
                    null,
                    new RectangleGeometry(r));

                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children.Add(geometryDrawing);
                drawingGroup.Opacity = 0.5f;

                _ctx.DrawDrawing(drawingGroup);
            }

            // Test Transform
            {
                Rect r = DRTMil2D.ResizeRectangle(m_screenCells.Next, 0.5, 0.5);
                GeometryDrawing geometryDrawing = new GeometryDrawing(
                    new ImageBrush(tulipImage),
                    null,
                    new RectangleGeometry(r));
                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children.Add(geometryDrawing);

                drawingGroup.Transform = new RotateTransform(
                    45,
                    /* centerX = */ r.Left + (r.Width / 2),
                    /* centerY = */ r.Top + (r.Height / 2)
                    );

                _ctx.DrawDrawing(drawingGroup);
            }

            // Test ClipGeometry
            {
                Rect r = m_screenCells.Next;
                GeometryDrawing geometryDrawing = new GeometryDrawing(
                    new ImageBrush(tulipImage),
                    null,
                    new RectangleGeometry(r));
                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children.Add(geometryDrawing);

                drawingGroup.ClipGeometry = new RectangleGeometry(DRTMil2D.ResizeRectangle(r, 0.75, 0.75));

                _ctx.DrawDrawing(drawingGroup);
            }

            // Test all properties used together
            {
                Rect r = DRTMil2D.ResizeRectangle(m_screenCells.Next, 0.6, 0.6);
                GeometryDrawing geometryDrawing = new GeometryDrawing(
                    new ImageBrush(tulipImage),
                    null,
                    new RectangleGeometry(r));
                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children.Add(geometryDrawing);

                drawingGroup.Opacity = 0.5;
                drawingGroup.Transform = new RotateTransform(
                    45,
                    /* centerX = */ r.Left + (r.Width / 2),
                    /* centerY = */ r.Top + (r.Height / 2)
                    );
                drawingGroup.ClipGeometry = new RectangleGeometry(DRTMil2D.ResizeRectangle(r, 0.75, 0.75));

                _ctx.DrawDrawing(drawingGroup);
            }

        }

        private void RunDrawingConsistencyTests()
        {
            BitmapSource tulipImage = BitmapFrame.Create(new Uri(@"DrtFiles\DrtMil2D\tulip.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);

            // Test Open() with 0 Children
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection children = drawingGroup.Children;

                DrawingContext dc = drawingGroup.Open();
                dc.Close();

                DRT.Assert(drawingGroup.Children.Count == 0);

                // Open should always replace the Children collection
                DRT.Assert(children != drawingGroup.Children);
            }

            // Test Append() with 0 Children
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection children = drawingGroup.Children;

                DrawingContext dc = drawingGroup.Append();
                dc.Close();

                DRT.Assert(drawingGroup.Children.Count == 0);

                // Append shouldn't replace the children collection
                DRT.Assert(children == drawingGroup.Children);
            }

            // Test Open() with 1 Child
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection children = drawingGroup.Children;

                DrawingContext dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.Close();

                DRT.Assert(drawingGroup.Children.Count == 1);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);

                // Open should always replace the Children collection
                DRT.Assert(children != drawingGroup.Children);
            }

            // Test Append() with 1 Child
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection children = drawingGroup.Children;

                DrawingContext dc = drawingGroup.Append();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.Close();

                DRT.Assert(drawingGroup.Children.Count == 1);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);

                // Append shouldn't replace the children collection
                DRT.Assert(children == drawingGroup.Children);
            }

            // Test Open() with 1 pre-existing Child
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingContext dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.Close();

                DrawingCollection children = drawingGroup.Children;

                dc = drawingGroup.Open();
                dc.DrawImage(tulipImage, m_screenCells[0]);
                dc.Close();

                // Open should always replace the Children collection
                DRT.Assert(children != drawingGroup.Children);
                DRT.Assert(drawingGroup.Children.Count == 1);
                DRT.Assert(drawingGroup.Children[0] is ImageDrawing);

            }

            // Test Append() with 1 pre-existing Child
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingContext dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.Close();

                DrawingCollection children = drawingGroup.Children;

                dc = drawingGroup.Append();
                dc.DrawImage(tulipImage, m_screenCells[0]);
                dc.Close();

                // Append should append to the current collection
                DRT.Assert(children == drawingGroup.Children);

                DRT.Assert(drawingGroup.Children.Count == 2);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);
                DRT.Assert(drawingGroup.Children[1] is ImageDrawing);
            }

            // Test Open() with two children
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection children = drawingGroup.Children;

                DrawingContext dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.DrawImage(tulipImage, m_screenCells[0]);
                dc.Close();

                DRT.Assert(drawingGroup.Children.Count == 2);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);
                DRT.Assert(drawingGroup.Children[1] is ImageDrawing);

                // Open should always replace the Children collection
                DRT.Assert(children != drawingGroup.Children);
            }

            // Test Append() with two children
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection children = drawingGroup.Children;

                DrawingContext dc = drawingGroup.Append();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.DrawImage(tulipImage, m_screenCells[0]);
                dc.Close();

                DRT.Assert(drawingGroup.Children.Count == 2);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);
                DRT.Assert(drawingGroup.Children[1] is ImageDrawing);

                // Append shouldn't replace the children collection
                DRT.Assert(children == drawingGroup.Children);
            }

            // Test Open() with two children and one pre-existing child
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingContext dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.Close();

                DrawingCollection children = drawingGroup.Children;

                dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Blue, null, m_screenCells[0]);
                dc.DrawImage(tulipImage, m_screenCells[0]);
                dc.Close();

                DRT.Assert(drawingGroup.Children.Count == 2);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);
                DRT.Assert(((GeometryDrawing)drawingGroup.Children[0]).Brush == Brushes.Blue);
                DRT.Assert(drawingGroup.Children[1] is ImageDrawing);

                // Open should always replace the Children collection
                DRT.Assert(children != drawingGroup.Children);
            }

            // Test Append() with two children and one pre-existing child
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingContext dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.Close();

                DrawingCollection children = drawingGroup.Children;

                dc = drawingGroup.Append();
                dc.DrawRectangle(Brushes.Blue, null, m_screenCells[0]);
                dc.DrawImage(tulipImage, m_screenCells[0]);
                dc.Close();

                // Append should append to the current collection
                DRT.Assert(children == drawingGroup.Children);

                DRT.Assert(drawingGroup.Children.Count == 3);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);
                DRT.Assert(drawingGroup.Children[1] is GeometryDrawing);
                DRT.Assert(((GeometryDrawing)drawingGroup.Children[0]).Brush == Brushes.Red);
                DRT.Assert(((GeometryDrawing)drawingGroup.Children[1]).Brush == Brushes.Blue);
                DRT.Assert(drawingGroup.Children[2] is ImageDrawing);
            }

            // Test single Push call
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection children = drawingGroup.Children;

                DrawingContext dc = drawingGroup.Open();
                dc.PushOpacity(0.5);
                dc.Close();

                DRT.Assert(drawingGroup.Opacity == 1.0);
                DRT.Assert(drawingGroup.Children.Count == 1);
                DRT.Assert(drawingGroup.Children[0] is DrawingGroup);
                DRT.Assert(((DrawingGroup)drawingGroup.Children[0]).Opacity == 0.5);
                DRT.Assert(((DrawingGroup)drawingGroup.Children[0]).Children.Count == 0);

                // Open should always replace the Children collection
                DRT.Assert(children != drawingGroup.Children);
            }

            // Test Push call with another root element
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection children = drawingGroup.Children;

                DrawingContext dc = drawingGroup.Open();
                dc.PushOpacity(0.5);
                dc.Pop();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.Close();

                DRT.Assert(drawingGroup.Opacity == 1.0);
                DRT.Assert(drawingGroup.Children.Count == 2);
                DRT.Assert(drawingGroup.Children[0] is DrawingGroup);
                DRT.Assert(((DrawingGroup)drawingGroup.Children[0]).Opacity == 0.5);
                DRT.Assert(((DrawingGroup)drawingGroup.Children[0]).Children.Count == 0);
                DRT.Assert(drawingGroup.Children[1] is GeometryDrawing);

                // Open should always replace the Children collection
                DRT.Assert(children != drawingGroup.Children);
            }


            // Test Push call with a single root element, that has children
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection children = drawingGroup.Children;

                DrawingContext dc = drawingGroup.Open();
                dc.PushOpacity(0.5);
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.Pop();
                dc.Close();

                DRT.Assert(drawingGroup.Opacity == 1.0);
                DRT.Assert(drawingGroup.Children.Count == 1);
                DRT.Assert(drawingGroup.Children[0] is DrawingGroup);
                DRT.Assert(((DrawingGroup)drawingGroup.Children[0]).Opacity == 0.5);
                DRT.Assert(((DrawingGroup)drawingGroup.Children[0]).Children.Count == 1);
                DRT.Assert(((DrawingGroup)drawingGroup.Children[0]).Children[0] is GeometryDrawing);

                // Open should always replace the Children collection
                DRT.Assert(children != drawingGroup.Children);
            }

            // Ensure that Append is rolled back if an Exception is thrown during
            // a Changed event.
            {
                bool exceptionThrown = false;

                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingContext dc = drawingGroup.Open();

                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.DrawRectangle(Brushes.Blue, null, m_screenCells[0]);

                dc.Close();

                drawingGroup.Children.Changed += new EventHandler(ChangedThrowingHandler);

                dc = drawingGroup.Append();

                dc.DrawImage(tulipImage, m_screenCells[0]);

                try
                {
                    dc.Close();
                }
                catch (Exception)
                {
                    exceptionThrown = true;

                    DRT.Assert(drawingGroup.Children.Count == 2);
                    DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);
                    DRT.Assert(drawingGroup.Children[1] is GeometryDrawing);
                }

                DRT.Assert(exceptionThrown);
            }

            // Ensure a Changed event isn't fired on the Children collection
            // during Open()
            {
                bool exceptionThrown = false;

                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children.Changed += new EventHandler(ChangedThrowingHandler);

                DrawingContext dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.DrawRectangle(Brushes.Blue, null, m_screenCells[0]);

                try
                {
                    dc.Close();
                }
                catch (Exception)
                {
                    exceptionThrown = true;
                }

                DRT.Assert(!exceptionThrown);

                DRT.Assert(drawingGroup.Children.Count == 2);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);
                DRT.Assert(drawingGroup.Children[1] is GeometryDrawing);
            }

            // Ensure an exception isn't thrown during Open() if the Children
            // collection is null
            {
                bool exceptionThrown = false;

                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children = null;

                DrawingContext dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.DrawRectangle(Brushes.Blue, null, m_screenCells[0]);

                try
                {
                    dc.Close();
                }
                catch (Exception)
                {
                    exceptionThrown = true;
                }

                DRT.Assert(!exceptionThrown);

                DRT.Assert(drawingGroup.Children.Count == 2);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);
                DRT.Assert(drawingGroup.Children[1] is GeometryDrawing);
            }

            // Ensure an exception isn't thrown during Open() if the Children
            // collection is Frozen
            {
                bool exceptionThrown = false;

                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection frozenCollection = new DrawingCollection();
                frozenCollection.Freeze();
                drawingGroup.Children = frozenCollection;

                DrawingContext dc = drawingGroup.Open();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.DrawRectangle(Brushes.Blue, null, m_screenCells[0]);

                try
                {
                    dc.Close();
                }
                catch (Exception)
                {
                    exceptionThrown = true;
                }

                DRT.Assert(!exceptionThrown);

                DRT.Assert(drawingGroup.Children.Count == 2);
                DRT.Assert(drawingGroup.Children[0] is GeometryDrawing);
                DRT.Assert(drawingGroup.Children[1] is GeometryDrawing);
            }


            // Ensure an exception is thrown during Append() if the Children
            // collection is null
            {
                bool exceptionThrown = false;

                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children = null;

                DrawingContext dc = drawingGroup.Append();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.DrawRectangle(Brushes.Blue, null, m_screenCells[0]);

                try
                {
                    dc.Close();
                }
                catch (InvalidOperationException)
                {
                    exceptionThrown = true;
                }

                DRT.Assert(exceptionThrown);

                DRT.Assert(drawingGroup.Children == null);
            }

            // Ensure an exception is thrown during Append() if the Children
            // collection is Frozen
            {
                bool exceptionThrown = false;

                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingCollection frozenCollection = new DrawingCollection();
                frozenCollection.Add(new GlyphRunDrawing());
                frozenCollection.Freeze();
                drawingGroup.Children = frozenCollection;

                DrawingContext dc = drawingGroup.Append();
                dc.DrawRectangle(Brushes.Red, null, m_screenCells[0]);
                dc.DrawRectangle(Brushes.Blue, null, m_screenCells[0]);

                try
                {
                    dc.Close();
                }
                catch (InvalidOperationException)
                {
                    exceptionThrown = true;
                }

                DRT.Assert(exceptionThrown);

                DRT.Assert(drawingGroup.Children.IsFrozen);
                DRT.Assert(drawingGroup.Children.Count == 1);
                DRT.Assert(drawingGroup.Children[0] is GlyphRunDrawing);
            }

        }

        private void RunRegressionTests()
        {
            //
            // 


            {
                //
                // Create a ChangeableReference brush and put it in use on a Drawing
                //
                SolidColorBrush brush = new SolidColorBrush(Colors.Red);

                DrawingGroup drawing = new DrawingGroup();
                DrawingContext dc = drawing.Open();

                // Use Brush
                dc.DrawRectangle(brush, null, new Rect(0, 0, 1, 1));
                dc.Close();

                // Verify the brush is still Freezable after the Use
                if ( brush.IsFrozen)
                {
                    throw new InvalidOperationException("ChangeableReference brush is frozen after use.");
                }

                //
                // Call Freeze on the drawing and verify that the brush
                // is also frozen
                //

                drawing.Freeze();

                if (!brush.IsFrozen)
                {
                    throw new InvalidOperationException("Freeze wasn't propagated to dependent brush");
                }
            }

            //
            // 


            {
                // Create brush
                SolidColorBrush brush = new SolidColorBrush(Colors.Green);

                // Create Drawing that uses Brush
                DrawingGroup drawing = new DrawingGroup();
                DrawingContext dc = drawing.Open();
                dc.DrawRectangle(brush, null, new Rect(0, 0, 1, 1));
                dc.Close();

                // Make a clone of Drawing
                Drawing drawing2 = drawing.Clone();

                // Change color of the brush used by the drawing
                brush.Color = Colors.HotPink;

                // Draw the cloned drawing
                //
                // The color of the brush held onto by the second drawing shouldn't
                // have been affected by the color change after the clone (it should
                // still be Green).
                _ctx.DrawRectangle(new DrawingBrush(drawing2), null, m_screenCells.Next);

            }

            //
            // 


            {
                DrawingGroup drawingGroup = new DrawingGroup();
                DrawingContext dc = drawingGroup.Open();

                dc.PushOpacity(0.5);
                dc.DrawRectangle(Brushes.Blue, null, new Rect(0,0,1,1));
                dc.Close();

                _ctx.DrawRectangle(new DrawingBrush(drawingGroup), null, m_screenCells.Next);
            }

            // 


            {
                DrawingGroup dg = new DrawingGroup();
                DrawingContext dc = dg.Append();
                dc.Close();
            }

            // 

            {
                // Define the FIXED16 boundry constants
                const int FIXED16_INT_MAX =  ( (1 << 15) - 1);
                const int FIXED16_INT_MAX_PLUS_1 = FIXED16_INT_MAX+1;
                const int FIXED16_INT_MAX_HALF = ( (1 << 15) - 1) / 2;
                const int FIXED16_INT_MAX_HALF_PLUS_1 = FIXED16_INT_MAX_HALF + 1;


                // Allocate an array of transparent pixels
                int cbPixel = PixelFormats.Bgra32.BitsPerPixel / 8;
                byte[] pixels = new byte[(FIXED16_INT_MAX_PLUS_1) * cbPixel];


                //
                // Test a bitmap that is one pixel large in width than
                // 16.16 fixed can handle
                //
                {
                    BitmapSource largerThanFixed16Width = BitmapSource.Create(
                        FIXED16_INT_MAX_PLUS_1,
                        1,
                        96.0,
                        96.0,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        FIXED16_INT_MAX_PLUS_1 * cbPixel
                        );

                    // Test non-tiled image
                    _ctx.DrawImage(
                        largerThanFixed16Width,
                        new Rect(0, 0, FIXED16_INT_MAX_PLUS_1, 1)
                        );

                    BitmapSource largerThanFixed16WidthTiled = BitmapSource.Create(
                        FIXED16_INT_MAX_HALF_PLUS_1,
                        1,
                        96.0,
                        96.0,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        FIXED16_INT_MAX_HALF_PLUS_1 * cbPixel
                        );


                    // Test tiled image
                    ImageBrush ib = new ImageBrush(largerThanFixed16WidthTiled);
                    ib.TileMode = TileMode.FlipXY;

                    _ctx.DrawRectangle(
                        ib,
                        null,
                        new Rect(0, 0, FIXED16_INT_MAX_HALF_PLUS_1, 1)
                        );
                }

                //
                // Test a bitmap that is one pixel large in height than
                // 16.16 fixed can handle
                //
                {
                    BitmapSource largerThanFixed16Height= BitmapSource.Create(
                        1,
                        FIXED16_INT_MAX_PLUS_1,
                        96.0,
                        96.0,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        cbPixel
                        );

                    // Test non-tiled image

                    _ctx.DrawImage(
                        largerThanFixed16Height,
                        new Rect(0, 0, 1, FIXED16_INT_MAX_PLUS_1)
                        );

                    // Test tiled image

                    BitmapSource largerThanFixed16HeightTiled = BitmapSource.Create(
                        1,
                        FIXED16_INT_MAX_HALF_PLUS_1,
                        96.0,
                        96.0,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        cbPixel
                        );

                    ImageBrush ib = new ImageBrush(largerThanFixed16HeightTiled);
                    ib.TileMode = TileMode.FlipXY;

                    _ctx.DrawRectangle(
                        ib,
                        null,
                        new Rect(0, 0, 1, FIXED16_INT_MAX_HALF_PLUS_1)
                        );
                }

                //
                // Test a bitmap that is the largest width 16.16 fixed
                // can handle
                //
                {
                    BitmapSource exactlyFixed16Width = BitmapSource.Create(
                        FIXED16_INT_MAX,
                        1,
                        96.0,
                        96.0,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        FIXED16_INT_MAX * cbPixel
                        );

                    // Test non-tiled image
                    _ctx.DrawImage(
                        exactlyFixed16Width,
                        new Rect(0, 0, FIXED16_INT_MAX, 1)
                        );

                    BitmapSource exactlyFixed16WidthTiled = BitmapSource.Create(
                        FIXED16_INT_MAX_HALF,
                        1,
                        96.0,
                        96.0,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        FIXED16_INT_MAX_HALF * cbPixel
                        );

                    // Test tiled image
                    ImageBrush ib = new ImageBrush(exactlyFixed16WidthTiled);
                    ib.TileMode = TileMode.FlipXY;

                    _ctx.DrawRectangle(
                        ib,
                        null,
                        new Rect(0, 0, FIXED16_INT_MAX_HALF, 1)
                        );
                }

                //
                // Test a bitmap that is the largest height 16.16 fixed
                // can handle
                //
                {
                    BitmapSource exactlyFixed16Height= BitmapSource.Create(
                        1,
                        FIXED16_INT_MAX,
                        96.0,
                        96.0,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        cbPixel
                        );

                    // Test non-tiled image
                    _ctx.DrawImage(
                        exactlyFixed16Height,
                        new Rect(0, 0, 1, FIXED16_INT_MAX)
                        );

                    // Test tiled image

                    BitmapSource exactlyFixed16HeightTiled = BitmapSource.Create(
                        1,
                        FIXED16_INT_MAX_HALF,
                        96.0,
                        96.0,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        cbPixel
                        );

                    ImageBrush ib = new ImageBrush(exactlyFixed16HeightTiled);
                    ib.TileMode = TileMode.FlipXY;

                    _ctx.DrawRectangle(
                        ib,
                        null,
                        new Rect(0, 0, 1, FIXED16_INT_MAX_HALF)
                        );
                }
            }

            // 

            {
                {
                    PathGeometry pg = new PathGeometry();
                    pg.AddGeometry(new RectangleGeometry(new Rect(new Point(10, 10), new Size(10, 10))));
                    IntersectionDetail result = pg.FillContainsWithDetail(new LineGeometry(new Point(15, 5), new Point(15,30)));

                    DRT.Assert(result == IntersectionDetail.Intersects);
                }

                {
                    PathGeometry pg = new PathGeometry();
                    pg.AddGeometry(new RectangleGeometry(new Rect(new Point(10, 10), new Size(10, 10))));
                    IntersectionDetail result = pg.FillContainsWithDetail(new LineGeometry(new Point(5,15), new Point(30,15)));

                    DRT.Assert(result == IntersectionDetail.Intersects);
                }
            }

            // 


            {
                {
                    RectangleGeometry r1 = new RectangleGeometry( new Rect( 0, 0, 10, 20 ) );
                    RectangleGeometry r2 = new RectangleGeometry( new Rect( 5, 0, 10, 20 ) );
                    GeometryGroup g = new GeometryGroup();
                    g.Children.Add( r1 );
                    g.Children.Add( r2 );
                    g.FillRule = FillRule.Nonzero;
                    double a4 = g.GetArea();

                    DRT.Assert(Math.Abs(a4 - 300.0) < .001);
                }

                {
                    RectangleGeometry r1 = new RectangleGeometry( new Rect( 0, 0, 10, 20 ) );
                    RectangleGeometry r2 = new RectangleGeometry( new Rect( 5, 0, 10, 20 ) );
                    GeometryGroup g = new GeometryGroup();
                    g.Children.Add( r1 );
                    g.Children.Add( r2 );
                    g.FillRule = FillRule.EvenOdd;
                    double a4 = g.GetArea();

                    DRT.Assert(Math.Abs(a4 - 200.0) < .001);
                }
            }

            // 



            {
                {
                    double size = 10.0;

                    Visual visual = new TestHitTestDrawingVisual(size, false /* no clipping */);
                    TransformGroup transformGroup = new TransformGroup();
                    Transform translate = new TranslateTransform(size, 0);

                    // Visual should look like .x.x.x

                    EllipseGeometry hitBall = new EllipseGeometry( new Point(size/2,size/2), 1, 1 );

                    // HitTest against space
                    hitBall.Transform = transformGroup;
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);

                    // HitTest against Red square
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == true);

                    // HitTest against space
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);

                    // HitTest against Green square
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == true);

                    // HitTest against space
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);

                    // HitTest against Blue square
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == true);

                    // HitTest against space
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);

                    // HitTest against Orange square
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == true);

                    // HitTest against space
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);
                }

                // Now try with clipping
                {
                    double size = 10.0;

                    Visual visual = new TestHitTestDrawingVisual(size, true /* with clipping */);
                    TransformGroup transformGroup = new TransformGroup();
                    Transform translate = new TranslateTransform(size, 0);

                    // Visual should look like .x.x..

                    EllipseGeometry hitBall = new EllipseGeometry( new Point(size/2,size/2), 1, 1 );

                    // HitTest against space
                    hitBall.Transform = transformGroup;
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);

                    // HitTest against Red square
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == true);

                    // HitTest against space
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);

                    // HitTest against Green square
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == true);

                    // HitTest against space
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);

                    // HitTest against Blue square (but clipped out)
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);

                    // HitTest against space
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);

                    // HitTest against Orange square
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == true);

                    // HitTest against space
                    transformGroup.Children.Add(translate);
                    DRT.Assert(HitTestVisualWithGeometry(visual, hitBall) == false);
                }
            }
        }

        bool HitTestVisualWithGeometry(Visual visual, Geometry hitGeometry)
        {
            _lastGeometryHitTestResult = null;

            VisualTreeHelper.HitTest(visual,
                null, // No filter
                new HitTestResultCallback(GeoHitTestResult),
                new GeometryHitTestParameters(hitGeometry)
                );

            if (_lastGeometryHitTestResult == null)
            {
                return false;
            }
            else
            {
                DRT.Assert(visual == _lastGeometryHitTestResult.VisualHit);

                return true;
            }
        }

        public HitTestResultBehavior GeoHitTestResult(HitTestResult result)
        {
            _lastGeometryHitTestResult = (GeometryHitTestResult)result;

            return System.Windows.Media.HitTestResultBehavior.Stop;
        }

        private GeometryHitTestResult _lastGeometryHitTestResult;

        void ChangedThrowingHandler(object sender, EventArgs e)
        {
            throw new Exception("User-generated exception");
        }


        public DRTMil2DDrawingSuite() : base("Drawing")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            //
            // Create GlyphRun used by tests
            //

            double scale = 1.0;
            double emSize = scale * 26.666666666666668;

            _glyphRun = new GlyphRun(
                    new GlyphTypeface(DRTMil2D.FontsUri, StyleSimulations.BoldSimulation),
                    0,          // bidi level
                    false,      // isSideways
                    emSize,
                    new ushort[] { 4 }, // glyph indices
                    new Point(0,0),
                    new double[] { scale * 7.4666666666666668 },    // advance widths
                    null,               // offsets
                    new char[] {'!'},
                    null,               // device font name
                    null,               // cluster map
                    null,               // caretStops
                    null                // culture info
                    );
            //
            // Create main Visual
            //

            DrawingVisual visual = new DrawingVisual();
            _rootVisual = visual;

            DrawingContext ctx = visual.RenderOpen();
            Render(ctx);

            //
            // Create progress bar
            //

            double progressBarWidth = m_screenCells.CellWidth;
            double progressBarHeight = m_screenCells.CellHeight;

            Rect fromRect = new Rect(
                    0,
                    DRTMil2D.WindowHeight - progressBarHeight - 20,
                    progressBarWidth,
                    progressBarHeight
                    );

            Rect toRect = new Rect(
                    fromRect.X,
                    fromRect.Y,
                    DRTMil2D.WindowWidth,
                    fromRect.Height
                    );

            RectangleGeometry rectangle = new RectangleGeometry(fromRect);

            RectAnimation rectAnimation = new RectAnimation(
                fromRect,
                toRect,
                new TimeSpan(0, 0, 0, 0, DRTMil2D.AnimationLength * 1000),
                FillBehavior.HoldEnd);

            rectAnimation.BeginTime = null;
            rectAnimation.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);

            _progressBarAnimation = rectAnimation.CreateClock();

            rectangle.ApplyAnimationClock(RectangleGeometry.RectProperty, _progressBarAnimation);

            LinearGradientBrush lgBrush = new LinearGradientBrush(
                Colors.LightGray,
                Colors.SteelBlue,
                0
                );

            lgBrush.MappingMode = BrushMappingMode.Absolute;
            lgBrush.StartPoint = new Point (0,0);
            lgBrush.EndPoint = new Point(DRTMil2D.WindowWidth, 0);

            GeometryDrawing geometryDrawing = new GeometryDrawing(lgBrush, null, rectangle);

            ctx.DrawDrawing(
                geometryDrawing
                );

            //
            // Set visual as root & display
            //

            ctx.Close();

            DRT.RootElement = _rootVisual;
            DRT.ShowRoot();

            return new DrtTest[]  { new DrtTest(RunTest)};
        }

        void RunTest()
        {
            _progressBarAnimation.Controller.Begin();

            DRT.Suspend();
        }

        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            Clock clock = (Clock)sender;

            if (clock.CurrentState != ClockState.Active)
            {
                DRT.Resume();

                Console.WriteLine("    DRTMil2DDrawingSuite.RunTest() completed.");
            }
        }

        internal static AreaPartitioner m_screenCells;

        private GlyphRun _glyphRun;

        private AnimationClock _progressBarAnimation;

        private DrawingVisual _rootVisual;
        private DrawingContext _ctx;

        private const int m_penThickness  = 10;
    }
}

