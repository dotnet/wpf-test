// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DRT;
using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace DRTMil2D
{
    public sealed class DRTMil2DBrushSuite : DrtTestSuite
    {
        public class TestDrawingVisual : DrawingVisual
        {
            private Rect _location;

            public TestDrawingVisual()
            {
                _location = DRTMil2DBrushSuite.m_screenCells.Next;

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

        public void TestBrushInPen(Brush brush, double thickness, Rect r)
        {
            // Shrink rectangle by 1/4 to account for portion of stroke
            // that lies outside of the rect it is filling
            r = DRTMil2D.ResizeRectangle(r, 0.75, 0.75);
            Pen pen = new Pen(brush, thickness);

            // Draw a horizontal line
            _ctx.DrawLine(pen, r.TopLeft, r.TopRight);

            // Draw a vertical line
            _ctx.DrawLine(pen, r.TopLeft, r.BottomLeft);
            // Draw a line that isn't axis aligned
            _ctx.DrawLine(pen, r.TopLeft, r.BottomRight);
        }

        // Default value for fEmptyBrush == false
        public void TestBrush(Brush brush)
        {
            TestBrush(brush, false);
        }

        public void TestBrush(Brush brush, bool fEmptyBrush)
        {
            Rect stroke;
            Rect fill;

            if (!fEmptyBrush)
            {
                // Use the next cell
                stroke = m_screenCells.Next;
                fill = m_screenCells.Next;
            }
            else
            {
                // Use reseved cell for empty brushes
                stroke = fill = m_screenCells[0];
            }

            // Test brush in a pen with 0 thickness
            TestBrushInPen(brush, 0, m_screenCells[0]);

            // Test brush in a pen with non-zero thickness
            TestBrushInPen(brush, m_penThickness, stroke);

            // Use brush to fill a rectangle
            _ctx.DrawRectangle(brush, null /* No Pen */, fill);

        }

        void TestRelativeTransforms(Brush brush)
        {
            // Draw brush without transform
            _ctx.DrawRectangle(brush, null /* No Pen */, m_screenCells.Next);

            Brush brushCopy = brush.Clone();

            Matrix relativeMatrix = new Matrix();
            relativeMatrix.SetIdentity();

            // The effect of DrawingBrush.Transform is the
            // opposite of the transform in other brushes

            // Use a non-180 degree angle to visualize that the transform
            // is not inverted
            relativeMatrix.RotateAt(90, 0.5,  0.5);
            relativeMatrix.Translate(-1.0, -1.0);

            // Rotate about the origin 180 degrees & offset by translation
            brushCopy.RelativeTransform = new MatrixTransform(relativeMatrix);
            // Undo translation set by relative transform
            brushCopy.Transform = new TranslateTransform(m_screenCells.CellWidth, m_screenCells.CellHeight);

            TestBrush(brushCopy);
        }

        void TestLinearGradientGeometry(LinearGradientBrush lgBrush)
        {
            // Draw brush with integer start/endpoint
            Rect r = m_screenCells.Next;

            int left = (int) (r.Left + (r.Width / 4.0));
            int top = (int) (r.Top + (r.Height / 4.0));
            int width = (int) (r.Width / 2.0);

            lgBrush.MappingMode = BrushMappingMode.Absolute;
            lgBrush.StartPoint = new Point(left, top);
            lgBrush.EndPoint = new Point(left + width, top);

            DrawLinearGradientGeometry(lgBrush, r, left, top, width);

            // Draw brush at - 0.5
            r = m_screenCells.Next;

            left = (int) (r.Left + (r.Width / 4.0));
            top = (int) (r.Top + (r.Height / 4.0));
            double newWidth = (double)width - 0.5;

            lgBrush.StartPoint = new Point(left, top);
            lgBrush.EndPoint = new Point((double)left + newWidth, top);
            DrawLinearGradientGeometry(lgBrush, r, left, top, newWidth);

            // Draw brush at - 1.0
            r = m_screenCells.Next;

            left = (int) (r.Left + (r.Width / 4.0));
            top = (int) (r.Top + (r.Height / 4.0));
            newWidth = (double)width - 1.0;

            lgBrush.StartPoint = new Point(left, top);
            lgBrush.EndPoint = new Point((double)left + newWidth, top);

            DrawLinearGradientGeometry(lgBrush, r, left, top, newWidth);

        }

        void DrawLinearGradientGeometry(LinearGradientBrush lgBrush, Rect r, double left, double top, double width)
        {
            LinearGradientBrush lgBrushCopy = lgBrush.Clone();

            // Draw rectangle in cell using the brush
            _ctx.DrawRectangle(lgBrushCopy, null /* No Pen */, r);

            // Draw red rectangle at
            _ctx.DrawRectangle(
                Brushes.Red,
                null,
                new Rect( new Point(left, top), new Size(width, 1)));
        }

        void TestRadialGradientGeometry(RadialGradientBrush rgBrush)
        {
            TestRadialGradientGeometry(rgBrush, false);
            TestRadialGradientGeometry(rgBrush, true);
        }

        void TestRadialGradientGeometry(RadialGradientBrush rgBrush, bool fFocalTest)
        {
            RadialGradientBrush rgBrushCopy = rgBrush.Clone();

            Rect r = m_screenCells.Next;

            int centerX = (int)( r.Left + (r.Width / 2.0));
            int centerY = (int)( r.Top + (r.Height / 2.0));
            int radiusX   = (int) (r.Width * 0.375);
            int radiusY   = (int) (r.Height * 0.375);

            rgBrushCopy.MappingMode = BrushMappingMode.Absolute;
            rgBrushCopy.Center = new Point(centerX, centerY);
            rgBrushCopy.RadiusX = radiusX;
            rgBrushCopy.RadiusY = radiusY;

            if (fFocalTest)
            {
                // Calculate a distance from the center in the X direction
                // that is close to the perimeter.
                int focalXDelta = (int)(((double)radiusX) / 2.0);

                // Same for Y
                int focalYDelta = (int)(((double)radiusY) / 2.0);

                rgBrushCopy.GradientOrigin = new Point(centerX - focalXDelta, centerY - focalYDelta);
            }
            else
            {
                // GradientOrigin is same as center
                rgBrushCopy.GradientOrigin = new Point(centerX, centerY);
            }

            // Draw rectangle in next cell using the brush
            _ctx.DrawRectangle(rgBrushCopy, null /* No Pen */, r);

            // Draw red rectangles

            _ctx.DrawRectangle(
                Brushes.Red,
                null,
                new Rect( rgBrushCopy.Center, new Size(radiusX, 1)));

            _ctx.DrawRectangle(
                Brushes.Red,
                null,
                new Rect( rgBrushCopy.Center, new Size(1, radiusY)));
        }

        void TestTileBrushGeometry(TileBrush tileBrush)
        {
            Rect r = m_screenCells.Next;

            double viewPortWidth = Math.Floor(r.Width / 2.0);
            double viewPortHeight = Math.Floor(r.Height / 2.0);

            tileBrush.TileMode = TileMode.Tile;
            tileBrush.ViewportUnits = BrushMappingMode.Absolute;

            // Test the tile brush with an integer-based width & height
            DrawTileBrushGeometry(
                tileBrush,
                r,
                viewPortWidth,
                viewPortHeight);

            // Test the tile brush with non-integer width and height
            DrawTileBrushGeometry(
                tileBrush,
                m_screenCells.Next,
                viewPortWidth + 0.5,
                viewPortHeight + 0.5);

        }

        DrawingVisual CreateBrushVisual()
        {
            // Setup Visual for VisualBrushes
            DrawingVisual brushVisual = new DrawingVisual();
            DrawingContext brushDrawingContext = brushVisual.RenderOpen();
            DRTMil2DDrawingSuite.FillDrawingContext(brushDrawingContext);
            brushDrawingContext.Close();

            return brushVisual;
        }

        DrawingVisual CreatePinkBrushVisual()
        {
            // Setup Visual for VisualBrushes
            DrawingVisual brushVisual = new DrawingVisual();
            DrawingContext brushDrawingContext = brushVisual.RenderOpen();
            brushDrawingContext.DrawRectangle(Brushes.HotPink, null, new Rect(0,0,1,1));
            brushDrawingContext.Close();

            return brushVisual;
        }

        void DrawTileBrushGeometry(TileBrush tileBrush, Rect r, double viewPortWidth, double viewPortHeight)
        {

            // Calculate view port
            double viewPortLeftOffset = Math.Floor(r.Width / 4.0);
            double viewPortTopOffset = Math.Floor(r.Height / 4.0);

            Rect viewPort = new Rect(
                Math.Floor(r.Left) + viewPortLeftOffset,
                Math.Floor(r.Top) + viewPortTopOffset,
                viewPortWidth,
                viewPortHeight);

            tileBrush.Viewport = viewPort;

            // Draw tile brush
            _ctx.DrawRectangle(tileBrush, null, r);


            // Draw red rectangles with the same width & height as the viewport

            _ctx.DrawRectangle(
                Brushes.Red,
                null,
                new Rect ( viewPort.Left, viewPort.Top+5.0, viewPort.Width, 1.0) );

            _ctx.DrawRectangle(
                Brushes.Red,
                null,
                new Rect ( viewPort.Left+5.0, viewPort.Top, 1.0, viewPort.Height) );

        }

        void TestTileBrushBaseTile(TileBrush inputBrush)
        {
            // Set properties on input that will be used by all tests
            inputBrush.Stretch = Stretch.None;
            inputBrush.Viewport = new Rect (0.25, 0.25, 0.5, 0.5);

            RotateTransform relativeCenterRotate = new RotateTransform(45, /* centerX = */ 0.5, /* centerY = */ 0.5);

            // Test non-tiled case
            {

                TileBrush nonTiledBrush = inputBrush.Clone();

                nonTiledBrush.TileMode = TileMode.None;
                _ctx.DrawRectangle(nonTiledBrush, null, m_screenCells.Next);
            }

            // Test tiled case
            {

                TileBrush tiledBrush = inputBrush.Clone();

                tiledBrush.TileMode = TileMode.Tile;
                _ctx.DrawRectangle(tiledBrush, null, m_screenCells.Next);
            }

            // Test rotated non-tiled case
            //
            // The tile brush code makes some assumptions about the axis-alignment of
            // the viewbox->viewport transform.  Test the tiled case to ensure those
            // aren't carried over to the user-specified Brush Transform.
            {

                TileBrush rotatedNonTiledBrush = inputBrush.Clone();

                rotatedNonTiledBrush.TileMode = TileMode.None;
                rotatedNonTiledBrush.RelativeTransform = relativeCenterRotate;
                _ctx.DrawRectangle(rotatedNonTiledBrush, null, m_screenCells.Next);
            }


            // Test rotated tiled case
            //
            // The tile brush code makes some assumptions about the axis-alignment of
            // the viewbox->viewport transform.  Test the tiled case to ensure those
            // aren't carried over to the user-specified Brush Transform.
            {

                TileBrush rotatedTiledBrush = inputBrush.Clone();

                rotatedTiledBrush.TileMode = TileMode.Tile;
                rotatedTiledBrush.RelativeTransform = relativeCenterRotate;
                _ctx.DrawRectangle(rotatedTiledBrush, null, m_screenCells.Next);
            }

            //
            // Test that the entire Viewport is tiled, even if empty space exists
            // between the mapped content & viewport boundry
            //
            {
                TileBrush paddedBrush = inputBrush.Clone();

                paddedBrush.Viewport = new Rect (0.0, 0.0, 0.2, 0.5);
                paddedBrush.TileMode = TileMode.Tile;
                paddedBrush.Stretch = Stretch.Uniform;

                _ctx.DrawRectangle(paddedBrush, null, m_screenCells.Next);
            }
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

            BitmapSource tulipImage = BitmapFrame.Create(new Uri(@"DrtFiles\DrtMil2D\tulip.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
            BitmapSource glyphImage = BitmapFrame.Create(new Uri(@"DrtFiles\DrtMil2D\cut.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);

            // Setup gradient stop collection
            GradientStopCollection gsStops = new GradientStopCollection();
            gsStops.Add(new GradientStop(Colors.DarkGray, 0.0));
            gsStops.Add(new GradientStop(Colors.White, 0.0));
            gsStops.Add(new GradientStop(Colors.Blue, 1.0));
            gsStops.Add(new GradientStop(Colors.DarkGray, 1.0));

            // Setup drawing for DrawingBrushes
            DrawingGroup brushDrawing = new DrawingGroup();

            using (DrawingContext brushDrawingContext = brushDrawing.Open()) // Test DrawingContext.IDisposable
            {
                DRTMil2DDrawingSuite.FillDrawingContext(brushDrawingContext);
            }

            // Reserve 1st cell for empty brushes
            m_screenCells.Reserve(1);

            // Test empty brushes
            {
                // Test w/ empty SolidColorBrush
                TestBrush(new SolidColorBrush(), true);

                // 


                TestBrush(new LinearGradientBrush(), true);
                TestBrush(new RadialGradientBrush(), true);

                // Test w/ empty image brush
                TestBrush(new ImageBrush(), true);

                // Test w/ empty DrawingBrush
                TestBrush (new DrawingBrush(), true);

                TestBrush(new VisualBrush(), true);
            }

            // Test simple brushes
            {
                // Test w/ simple SolidColorBrush
                TestBrush(new SolidColorBrush(Colors.BlueViolet));

                // Test w/ simple linear gradient brush
                TestBrush(new LinearGradientBrush(Colors.Red, Colors.Cyan, 0));

                // Test w/ simple radial gradient brush
                TestBrush(new RadialGradientBrush(Colors.Red, Colors.Cyan));

                // Test w/ default image brush
                TestBrush(new ImageBrush(tulipImage));

                // Create simple drawing

                TestBrush( new DrawingBrush(brushDrawing));

                TestBrush (new VisualBrush(CreateBrushVisual()));
            }

            // Test simple brushes w/ opacity set
            {
                Brush currentBrush;

                // Test w/ simple SolidColorBrush
                currentBrush = new SolidColorBrush(Colors.BlueViolet);
                currentBrush.Opacity = 0.5;
                TestBrush(currentBrush);

                currentBrush.Opacity = 0.5;
                TestBrush(currentBrush);

                // Test w/ simple linear gradient brush
                currentBrush = new LinearGradientBrush(Colors.Red, Colors.Cyan, 0);
                currentBrush.Opacity = 0.5;
                TestBrush(currentBrush);

                // Test w/ simple radial gradient brush
                currentBrush = new RadialGradientBrush(Colors.Red, Colors.Cyan);
                currentBrush.Opacity = 0.5;
                TestBrush(currentBrush);

                // Test w/ default image brush
                currentBrush = new ImageBrush(tulipImage);
                currentBrush.Opacity = 0.5;
                TestBrush(currentBrush);

                // Test w/ simple DrawingBrush
                currentBrush = new DrawingBrush(brushDrawing);
                currentBrush.Opacity = 0.5;
                TestBrush(currentBrush);

                // Test w/ simple VisualBrush
                currentBrush = new VisualBrush(CreateBrushVisual());
                currentBrush.Opacity = 0.5;
                TestBrush(currentBrush);

            }

            // Test opacity set on both stroke & fill
            {
                //
                // Test with brushes that don't use effects to support opacity
                //
                {

                    // Create solid stroke brush
                    Brush strokeBrush = new SolidColorBrush(Colors.Red);
                    strokeBrush.Opacity = 0.75;

                    // Create LinearGradient fill brush
                    Brush fillBrush = new LinearGradientBrush(Colors.Red, Colors.Blue, 0);
                    fillBrush.Opacity = 0.5;

                    // Reduce rect size so stroke doesn't go out of bounds
                    Rect r = DRTMil2D.ResizeRectangle(m_screenCells.Next, 0.75, 0.75);

                    // Draw rectangle
                    _ctx.DrawRectangle(fillBrush, new Pen(strokeBrush, m_penThickness), r);
                }

                //
                // Test with brushes that do use effects to support opacity (texture brushes)
                //
                {
                    ImageBrush strokeBrush = new ImageBrush(tulipImage);
                    strokeBrush.Stretch = Stretch.None;
                    strokeBrush.Opacity = 0.75;

                    ImageBrush fillBrush = new ImageBrush(tulipImage);
                    fillBrush.Opacity = 0.5;

                    // Reduce rect size so stroke doesn't go out of bounds
                    Rect r = DRTMil2D.ResizeRectangle(m_screenCells.Next, 0.75, 0.75);

                    // Draw rectangle
                    _ctx.DrawRectangle(fillBrush, new Pen(strokeBrush, m_penThickness), r);
                }
            }

            {
                // Test Glass Brush w/ brush that doesn't implement opacity with effects
                SolidColorBrush brush = new SolidColorBrush(Color.FromScRgb(1.0f, 1.0f, 1.0f, 1.0f));
                brush.Opacity = 255.5;
                TestBrush(brush);

                // Test Glass Brush w/ brush that does implement opacity with effects
                ImageBrush imageBrush = new ImageBrush(tulipImage);
                imageBrush.Opacity = 255.5;
                TestBrush(imageBrush);
            }


            // Relative brush transforms tests
            {
                // Test Translated, Scaled, & Skewed RelativeTransform

                Matrix relativeMatrix = new Matrix();
                relativeMatrix.SetIdentity();
                relativeMatrix.Translate(-0.5, -0.5);
                relativeMatrix.Scale(0.5, 0.5);
                relativeMatrix.Skew(10, 10);
                relativeMatrix.Translate(0.5, 0.5);

                ImageBrush ib = new ImageBrush(tulipImage);
                ib.RelativeTransform = new MatrixTransform(relativeMatrix);

                _ctx.DrawRectangle(ib, null, m_screenCells.Next);
            }
            {

                // Test with radial gradient brush
                RadialGradientBrush radialBrush = new RadialGradientBrush(gsStops);

                radialBrush.RadiusX = .4;
                radialBrush.RadiusY = .2;

                TestRelativeTransforms(radialBrush);

                // Test with linear gradient brush
                LinearGradientBrush lgBrush = new LinearGradientBrush(gsStops);
                lgBrush.StartPoint = new Point(0.25, 0.25);
                lgBrush.EndPoint = new Point(0.75, 0.75);

                TestRelativeTransforms(lgBrush);

                // Test with image brush
                ImageBrush imageBrush = new ImageBrush(tulipImage);

                // Use rectanglular viewport to visualize that the viewbox->viewport transform
                // is applied before the brush transform
                imageBrush.Viewport = new Rect(0.125, 0, 0.75, 1.0);

                TestRelativeTransforms(imageBrush);

                // Test with drawing brush

                // Create Drawing

                // Create DrawingBrush from drawing & test
                DrawingBrush drawingBrush = new DrawingBrush(brushDrawing);

                // Use rectanglular viewport to visualize that the viewbox->viewport transform
                // is applied before the brush transform
                drawingBrush.Viewport = new Rect(0.125, 0, 0.75, 1.0);

                TestRelativeTransforms(drawingBrush);
            }

            // Test TileBrush geometry
            {
                // Test image brush
                TestTileBrushGeometry(new ImageBrush(tulipImage));

                // Test uniform tiled imagebrush with 96x48dpi image
                BitmapSource tulip96x192dpiImage = BitmapFrame.Create(new Uri("drtfiles\\drtmil2d\\tulip96x192dpi.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
                ImageBrush ib = new ImageBrush(tulip96x192dpiImage);
                ib.Stretch = Stretch.Uniform;
                TestTileBrushGeometry(ib);

                // Test drawing brush
                TestTileBrushGeometry(new DrawingBrush(brushDrawing));
            }

            // Test scRGB gradient interpolation
            {
                LinearGradientBrush lgBrush = new LinearGradientBrush(Colors.Red, Colors.Cyan, 0);
                lgBrush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;
                TestBrush(lgBrush);

                RadialGradientBrush rgBrush = new RadialGradientBrush(Colors.Red, Colors.Cyan);
                rgBrush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;
                TestBrush(rgBrush);
            }

            // Test gradient brush spread method geometry's for correctness
            {
                //
                // Test LinearGradientBrush
                //

                // Setup LinearGradientBrush
                LinearGradientBrush lgBrush = new LinearGradientBrush(gsStops);

                // Test Pad LinearGradientBrush
                lgBrush.SpreadMethod = GradientSpreadMethod.Pad;
                TestLinearGradientGeometry(lgBrush);

                // Test Reflect LinearGradientBrush
                lgBrush.SpreadMethod = GradientSpreadMethod.Reflect;
                TestLinearGradientGeometry(lgBrush);

                // Test Repeat LinearGradientBrush
                lgBrush.SpreadMethod = GradientSpreadMethod.Repeat;
                TestLinearGradientGeometry(lgBrush);

                //
                // Test radial gradient brush
                //

                RadialGradientBrush rgBrush = new RadialGradientBrush(gsStops);

                // Test Pad RadialGradientBrush
                rgBrush.SpreadMethod = GradientSpreadMethod.Pad;
                TestRadialGradientGeometry(rgBrush);

                // Test Reflect RadialGradientBrush
                rgBrush.SpreadMethod = GradientSpreadMethod.Reflect;
                TestRadialGradientGeometry(rgBrush);

                // Test Repeat RadialGradientBrush
                rgBrush.SpreadMethod = GradientSpreadMethod.Repeat;
                TestRadialGradientGeometry(rgBrush);
            }

            // Test continuity of reflect & repeat wrap modes amoungst multiple geometry
            // by drawing duplicate rectangles side-by-side
            {
                LinearGradientBrush lgBrush = new LinearGradientBrush(
                    gsStops,
                    new Point(0.25, 0.25),
                    new Point(0.75, 0.25));

                lgBrush.SpreadMethod = GradientSpreadMethod.Reflect;
                _ctx.DrawRectangle(lgBrush, null /* No Pen */, m_screenCells.Next);
                _ctx.DrawRectangle(lgBrush, null /* No Pen */, m_screenCells.Next);

                lgBrush = lgBrush.Clone();
                lgBrush.SpreadMethod = GradientSpreadMethod.Repeat;
                _ctx.DrawRectangle(lgBrush, null /* No Pen */, m_screenCells.Next);
                _ctx.DrawRectangle(lgBrush, null /* No Pen */, m_screenCells.Next);

            }

            // Place radial gradients with all 3 wrap modes side by side
            {
                RadialGradientBrush rgBrush = new RadialGradientBrush(gsStops);
                rgBrush.RadiusX = 0.375;
                rgBrush.RadiusY = 0.375;

                rgBrush.SpreadMethod = GradientSpreadMethod.Pad;
                _ctx.DrawRectangle(rgBrush, null, m_screenCells.Next);

                rgBrush = rgBrush.Clone();
                rgBrush.SpreadMethod = GradientSpreadMethod.Reflect;
                _ctx.DrawRectangle(rgBrush, null, m_screenCells.Next);

                rgBrush = rgBrush.Clone();
                rgBrush.SpreadMethod = GradientSpreadMethod.Repeat;
                _ctx.DrawRectangle(rgBrush, null, m_screenCells.Next);
            }

            // Test degenerate gradient brushes with all wrap modes
            {
                Point center = new Point(0.5, 0.5);

                // Test linear gradient brush
                LinearGradientBrush lgBrush = new LinearGradientBrush(gsStops, center, center);

                lgBrush.SpreadMethod = GradientSpreadMethod.Pad;
                TestBrush(lgBrush);

                lgBrush = lgBrush.Clone();
                lgBrush.SpreadMethod = GradientSpreadMethod.Reflect;
                TestBrush(lgBrush);

                lgBrush = lgBrush.Clone();
                lgBrush.SpreadMethod = GradientSpreadMethod.Repeat;
                TestBrush(lgBrush);

                // Test RadialGradientBrush
                RadialGradientBrush rgBrush = new RadialGradientBrush(gsStops);
                rgBrush.Center = center;
                rgBrush.RadiusX = 0;
                rgBrush.RadiusY = 0;

                rgBrush.SpreadMethod = GradientSpreadMethod.Pad;
                TestBrush(rgBrush);

                rgBrush = rgBrush.Clone();
                rgBrush.SpreadMethod = GradientSpreadMethod.Reflect;
                TestBrush(rgBrush);

                rgBrush = rgBrush.Clone();
                rgBrush.SpreadMethod = GradientSpreadMethod.Repeat;
                TestBrush(rgBrush);
            }

            // Test focal brush
            {
                Point center = new Point(0.5, 0.5);
                Point GradientOrigin = new Point(0.0, 0.0);
                RadialGradientBrush rgBrush = new RadialGradientBrush(gsStops);

                Rect r = m_screenCells.Next;
                int centerX = (int) (r.Left + (r.Width / 2.0));
                int centerY = (int) (r.Top + (r.Height / 2.0));

                // Test focal brush with inside circle at unit boundry
                rgBrush.MappingMode = BrushMappingMode.Absolute;
                rgBrush.Center = new Point(centerX, centerY);
                rgBrush.GradientOrigin = new Point (centerX + 8, centerY + 8);
                rgBrush.RadiusX = r.Width * .375;
                rgBrush.RadiusY = rgBrush.RadiusX;

                _ctx.DrawRectangle(rgBrush, null, r);

                // Test focal brush with inside circle at half unit boundry
                r = m_screenCells.Next;
                centerX = (int) (r.Left + (r.Width / 2.0));
                centerY = (int) (r.Top + (r.Height / 2.0));

                // Test focal brush with inside circle at unit boundry
                rgBrush = new RadialGradientBrush(gsStops);
                rgBrush.MappingMode = BrushMappingMode.Absolute;
                rgBrush.Center = new Point(centerX, centerY);
                rgBrush.GradientOrigin = new Point ((double)centerX + 8.5, (double)centerY + 8.5);
                rgBrush.RadiusX = r.Width * .375;
                rgBrush.RadiusY = rgBrush.RadiusX;

                _ctx.DrawRectangle(rgBrush, null, r);

                // Test focal brush with GradientOrigin outside of circle
                rgBrush = new RadialGradientBrush(gsStops);
                rgBrush.Center = center;
                rgBrush.GradientOrigin = new Point(0.0, 0.0);
                rgBrush.RadiusX = 0.375;
                rgBrush.RadiusY = 0.375;

                TestBrush(rgBrush);

                // Test degenerate focal brush
                rgBrush = new RadialGradientBrush(gsStops);
                rgBrush.Center = center;
                rgBrush.GradientOrigin = GradientOrigin;
                rgBrush.RadiusX = 0;
                rgBrush.RadiusY = 0;

                TestBrush(rgBrush);
            }

            // Test linear brush with multiple coincident points
            {
                GradientStopCollection gsCoincidentStops = new GradientStopCollection();

                gsCoincidentStops.Add(new GradientStop(Colors.Beige, 0.0));
                gsCoincidentStops.Add(new GradientStop(Colors.White, 0.0));
                gsCoincidentStops.Add(new GradientStop(Colors.Red, 0.0));

                gsCoincidentStops.Add(new GradientStop(Colors.Yellow, 0.5));
                gsCoincidentStops.Add(new GradientStop(Colors.Black, 0.5));
                gsCoincidentStops.Add(new GradientStop(Colors.Blue, 0.5));

                gsCoincidentStops.Add(new GradientStop(Colors.Green, 1.0));
                gsCoincidentStops.Add(new GradientStop(Colors.DarkGray, 1.0));
                gsCoincidentStops.Add(new GradientStop(Colors.Brown, 1.0));

                LinearGradientBrush lgBrush = new LinearGradientBrush(gsCoincidentStops);
                TestBrush(lgBrush);
            }

            // Test LinearGradient angle constructors
            {
                LinearGradientBrush lgBrush = new LinearGradientBrush(Colors.Yellow, Colors.Green, 90.0);
                TestBrush(lgBrush);

                lgBrush = new LinearGradientBrush(gsStops, 90.0);
                TestBrush(lgBrush);
            }

            // Test all permutations of 2 stops
            {
                // The green stop is placed such that solid color rectangles will
                // be green (not red), and rectangles that are interpolated
                // will go from green to red (not red to green).

                const int numCases = 17;
                TwoStopTest[] testData = new TwoStopTest[numCases];

                testData[0].positionOne = -0.5;
                testData[0].positionTwo = -0.5;
                testData[0].greenFirst = false;

                testData[1].positionOne = -0.5;
                testData[1].positionTwo = -0.3;
                testData[1].greenFirst = false;

                testData[2].positionOne = -0.5;
                testData[2].positionTwo = 0.0;
                testData[2].greenFirst = false;

                testData[3].positionOne = -0.5;
                testData[3].positionTwo = 0.5;
                testData[3].greenFirst = true;

                testData[4].positionOne = -0.5;
                testData[4].positionTwo = 1.0;
                testData[4].greenFirst = true;

                testData[5].positionOne = -0.5;
                testData[5].positionTwo = 1.5;
                testData[5].greenFirst = true;

                testData[6].positionOne = 0.0;
                testData[6].positionTwo = 0.0;
                testData[6].greenFirst = false;

                testData[7].positionOne = 0.0;
                testData[7].positionTwo = 0.5;
                testData[7].greenFirst = true;

                testData[8].positionOne = 0.0;
                testData[8].positionTwo = 1.0;
                testData[8].greenFirst = true;

                testData[9].positionOne = 0.0;
                testData[9].positionTwo = 1.5;
                testData[9].greenFirst = true;

                testData[10].positionOne = 0.5;
                testData[10].positionTwo = 0.5;
                testData[10].greenFirst = true;

                testData[11].positionOne = 0.5;
                testData[11].positionTwo = 0.6;
                testData[11].greenFirst = true;

                testData[12].positionOne = 0.5;
                testData[12].positionTwo = 1.0;
                testData[12].greenFirst = true;

                testData[13].positionOne = 0.5;
                testData[13].positionTwo = 1.5;
                testData[13].greenFirst = true;

                testData[14].positionOne = 1.0;
                testData[14].positionTwo = 1.0;
                testData[14].greenFirst = true;

                testData[15].positionOne = 1.0;
                testData[15].positionTwo = 1.5;
                testData[15].greenFirst = true;

                testData[15].positionOne = 1.5;
                testData[15].positionTwo = 1.5;
                testData[15].greenFirst = true;

                for(int i = 0; i < numCases; i++)
                {
                    GradientStopCollection twoStops = new GradientStopCollection();

                    if (testData[i].greenFirst)
                    {
                        twoStops.Add(new GradientStop(Colors.Green, testData[i].positionOne));
                        twoStops.Add(new GradientStop(Colors.Red, testData[i].positionTwo));
                    }
                    else
                    {
                        twoStops.Add(new GradientStop(Colors.Red, testData[i].positionOne));
                        twoStops.Add(new GradientStop(Colors.Green, testData[i].positionTwo));
                    }

                    LinearGradientBrush lgBrush = new LinearGradientBrush(twoStops);

                    _ctx.DrawRectangle(lgBrush, null, m_screenCells.Next);
                }
            }

            // 

            {
                // Test w/ 1-stop linear gradient
                LinearGradientBrush lgBrush = new LinearGradientBrush();
                lgBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.5));

                TestBrush(lgBrush);

                // Test w/ 1-stop radial gradient
                RadialGradientBrush rgBrush = new RadialGradientBrush();
                rgBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.5));

                TestBrush(rgBrush);
            }

            // TileBrush tests.
            {
                // Test with a 0 0 0 0 viewbox
                ImageBrush imageBrush = new ImageBrush(tulipImage);
                imageBrush.Viewbox = new Rect(0, 0, 0, 0);

                TestBrush(imageBrush);
            }

            {
                // Test with an empty viewbox
                ImageBrush imageBrush = new ImageBrush(tulipImage);
                imageBrush.Viewbox = Rect.Empty;

                TestBrush(imageBrush, true);
            }

            {
                // Test with an empty viewport
                ImageBrush imageBrush = new ImageBrush(tulipImage);
                imageBrush.Viewport = Rect.Empty;

                TestBrush(imageBrush, true);
            }

            // Test ImageBrush with 48x48 DPI image
            {
                BitmapSource tulip48dpiImage = BitmapFrame.Create(new Uri("drtfiles\\drtmil2d\\tulip48dpi.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);

                ImageBrush ib = new ImageBrush(tulip48dpiImage);
                ib.ViewboxUnits = BrushMappingMode.Absolute;
                ib.Viewbox = new Rect(100, 100, 100, 100);

                _ctx.DrawRectangle(ib, null, m_screenCells.Next);
            }

            // Test unstretched ImageBrush with 96x48 DPI image
            {
                BitmapSource tulip96x192dpiImage = BitmapFrame.Create(new Uri("drtfiles\\drtmil2d\\tulip96x192dpi.png", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);

                ImageBrush ib = new ImageBrush(tulip96x192dpiImage);
                ib.Stretch = Stretch.None;

                _ctx.DrawRectangle(ib, null, m_screenCells.Next);
            }

            // Test DrawingBrush Viewbox with large units
            {
                // Test ViewboxUnits == RelativeToBoundingBox
                DrawingGroup dg = new DrawingGroup();
                DrawingContext dc = dg.Open();

                DRTMil2DDrawingSuite.FillDrawingContextWithLargeUnits(dc);

                dc.Close();

                _ctx.DrawRectangle(new DrawingBrush(dg), null, m_screenCells.Next);

                // Test ViewboxUnits == Absolute
                DrawingBrush db = new DrawingBrush(dg);
                db.Viewbox = new Rect(500, 500, 500, 500);
                db.ViewboxUnits = BrushMappingMode.Absolute;

                _ctx.DrawRectangle(db, null, m_screenCells.Next);
            }

            // Test VisualBrush Viewbox with large units
            {
                // Test ViewboxUnits == RelativeToBoundingBox
                DrawingVisual dv = new DrawingVisual();
                DrawingContext dc = dv.RenderOpen();

                DRTMil2DDrawingSuite.FillDrawingContextWithLargeUnits(dc);

                dc.Close();

                _ctx.DrawRectangle(new VisualBrush(dv), null, m_screenCells.Next);

                // Test ViewboxUnits == Absolute
                VisualBrush vb = new VisualBrush(dv);
                vb.Viewbox = new Rect(500, 500, 500, 500);
                vb.ViewboxUnits = BrushMappingMode.Absolute;

                _ctx.DrawRectangle(vb, null, m_screenCells.Next);
            }

            // Test intermediate render target creation when the Viewport is small
            {
                //
                // First, use the smallest value that will allow the intermediate
                // to be created
                //
                double avoidIsCloseReal = (1.192092896e-07F * 10);

                DrawingBrush db = new DrawingBrush(brushDrawing);
                db.Viewport = new Rect(0.0, 0.0, avoidIsCloseReal, avoidIsCloseReal);
                db.ViewportUnits = BrushMappingMode.Absolute;

                _ctx.DrawRectangle(db, null, m_screenCells[0]);

                //
                // Next, use the largest value that will cause the brush to
                // avoid creating an intermediate
                //

                double isCloseRealBoundry = (1.192092896e-07F * 9);

                db = new DrawingBrush(brushDrawing);
                db.Viewport = new Rect(0.0, 0.0, isCloseRealBoundry, isCloseRealBoundry);
                db.ViewportUnits = BrushMappingMode.Absolute;

                _ctx.DrawRectangle(db, null, m_screenCells[0]);

                // Test a 0, 0, 0, 0 Viewport
                db = new DrawingBrush(brushDrawing);
                db.Viewport = new Rect(0.0, 0.0, 0.0, 0.0);
                db.ViewportUnits = BrushMappingMode.Absolute;

                _ctx.DrawRectangle(db, null, m_screenCells[0]);
            }

            //
            // Test tilebrushes with non-invertible Transforms
            //
            {
                Transform degenerateTransform = new MatrixTransform(0, 0, 0, 0, 0, 0);

                // Test w/ a linear gradient brush
                LinearGradientBrush lgBrush = new LinearGradientBrush(Colors.Red, Colors.Blue, 0);
                lgBrush.Transform = degenerateTransform;
                _ctx.DrawRectangle(lgBrush, null, m_screenCells.Next);

                // Test w/ a radial gradient brush
                RadialGradientBrush rgBrush = new RadialGradientBrush(Colors.Red, Colors.Blue);
                rgBrush.Transform = degenerateTransform;
                _ctx.DrawRectangle(rgBrush, null, m_screenCells.Next);

                // Test w/ a image brush
                ImageBrush imageBrush = new ImageBrush(tulipImage);
                imageBrush.Transform = degenerateTransform;
                _ctx.DrawRectangle(imageBrush, null, m_screenCells[0]);

                // Test w/ a DrawingBrush
                DrawingBrush drawingBrush = new DrawingBrush(brushDrawing);
                drawingBrush.Transform = degenerateTransform;
                _ctx.DrawRectangle(drawingBrush, null, m_screenCells[0]);

                // Test w/ a VisualBrush
                VisualBrush visualBrush = new VisualBrush(CreateBrushVisual());
                visualBrush.Transform = degenerateTransform;
                _ctx.DrawRectangle(visualBrush, null, m_screenCells[0]);
            }

            // Test a TileBrush with both transforms set to NULL
            {
                ImageBrush imageBrush = new ImageBrush(tulipImage);
                imageBrush.Transform = null;
                imageBrush.RelativeTransform = null;

                _ctx.DrawRectangle(imageBrush, null, m_screenCells.Next);
            }

            // 


            {
                // Create a pink visual that will be very visible if the Visual
                // isn't changed properly during RunTest()
                _testChangedVisualBrush = new VisualBrush(CreatePinkBrushVisual());

                _ctx.DrawRectangle(_testChangedVisualBrush, null, m_screenCells.Next);
            }

            // 


            {
                LinearGradientBrush lgBrush = new LinearGradientBrush();

                lgBrush.GradientStops.Add( new GradientStop(Colors.Red, 0.0));
                lgBrush.GradientStops.Add( new GradientStop(Colors.Blue, 0.986679554));
                lgBrush.GradientStops.Add( new GradientStop(Colors.Blue, 0.9866800904));
                lgBrush.GradientStops.Add( new GradientStop(Colors.Blue, 0.986681));
                lgBrush.GradientStops.Add( new GradientStop(Colors.Blue, 1.0));

                _ctx.DrawRectangle(lgBrush, null, m_screenCells.Next);
            }

            // 


            {

                DrawingBrush drawingBrush = new DrawingBrush();

                drawingBrush.Drawing = brushDrawing;

                drawingBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                drawingBrush.Viewport = new Rect(
                    -Int32.MaxValue,
                    -Int32.MaxValue,
                    Int32.MaxValue,
                    Int32.MaxValue
                    );

                // Render to a RenderTargetBitmap so that CSwRenderTargetSurface::CreateRenderTargetBitmap is hit
                // (otherwise a meta or desktop RT returns E_INVALIDARG)

                DrawingVisual dv = new DrawingVisual();
                DrawingContext dvCtx = dv.RenderOpen();

                dvCtx.DrawRectangle(drawingBrush, null, m_screenCells[0]);
                dvCtx.Close();

                RenderTargetBitmap id = new RenderTargetBitmap((int)m_screenCells[0].Width, (int)m_screenCells[0].Height, 96.0, 96.0, PixelFormats.Pbgra32);
                id.Render(dv);

                // Use a desktop RT
                _ctx.DrawRectangle(drawingBrush, null, m_screenCells[0]);
            }

            // 

            {
                VisualBrush vb = new VisualBrush(CreateBrushVisual());
                vb.ViewboxUnits = BrushMappingMode.Absolute;
                vb.Stretch = Stretch.Uniform;
                vb.Viewbox = new Rect(0.2, 0.2, 0.5, 0.5);

                _ctx.DrawRectangle(vb, null, m_screenCells.Next);
            }

            //
            // Test TileBrush base tile functionality.
            //

            //  Test ImageBrush with a 100x100 checkered image
            {
                BitmapSource checkeredImage = BitmapFrame.Create(new Uri("drtfiles\\drtmil2d\\tile_checkered.PNG", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);
                TestTileBrushBaseTile(new ImageBrush(checkeredImage));
            }

            // Test DrawingBrush with 100x100 checker pattern
            {
                DrawingGroup dg = new DrawingGroup();
                DrawingContext dc = dg.Open();

                dc.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0.0, 0.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Green,
                    null,
                    new Rect(50.0, 0.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Purple,
                    null,
                    new Rect(0.0, 50.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Orange,
                    null,
                    new Rect(50.0, 50.0, 50.0, 50.0));

                dc.Close();

                TestTileBrushBaseTile(new DrawingBrush(dg));
            }

            // Test VisualBrush with 100x100 checker pattern
            {
                DrawingVisual dv = new DrawingVisual();
                DrawingContext dc = dv.RenderOpen();

                dc.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0.0, 0.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Green,
                    null,
                    new Rect(50.0, 0.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Purple,
                    null,
                    new Rect(0.0, 50.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Orange,
                    null,
                    new Rect(50.0, 50.0, 50.0, 50.0));

                dc.Close();

                TestTileBrushBaseTile(new VisualBrush(dv));
            }

            // Test ImageBrush with 100x100 checker pattern DrawingImage
            //      B G
            //      G B
            {
                DrawingGroup dg = new DrawingGroup();
                DrawingContext dc = dg.Open();

                dc.DrawRectangle(
                    Brushes.Blue,
                    null,
                    new Rect(0.0, 0.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Green,
                    null,
                    new Rect(50.0, 0.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Green,
                    null,
                    new Rect(0.0, 50.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Blue,
                    null,
                    new Rect(50.0, 50.0, 50.0, 50.0));

                dc.Close();

                TestTileBrushBaseTile(new ImageBrush(new DrawingImage(dg)));
            }

            // Test ImageBrush with a DrawingImage that has no content
            {
                TestTileBrushBaseTile(new ImageBrush(new DrawingImage(null)));
            }

            // Test DrawingImage with 100x100 checker pattern
            //      W G
            //      G W
            {
                DrawingGroup dg = new DrawingGroup();
                DrawingContext dc = dg.Open();

                dc.DrawRectangle(
                    Brushes.White,
                    null,
                    new Rect(0.0, 0.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Green,
                    null,
                    new Rect(50.0, 0.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.Green,
                    null,
                    new Rect(0.0, 50.0, 50.0, 50.0));

                dc.DrawRectangle(
                    Brushes.White,
                    null,
                    new Rect(50.0, 50.0, 50.0, 50.0));

                dc.Close();

                _ctx.DrawImage(new DrawingImage(dg), m_screenCells.Next);
            }

            // Test DrawingImage with a GlyphRunDrawing
            {
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();
                drawingContext.DrawText(new FormattedText("abc", System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface("Comic Sans MS"), 54, Brushes.Red), new Point( 0, 0 ));
                drawingContext.Close();

                Drawing readback = VisualTreeHelper.GetDrawing(drawingVisual);

                _ctx.DrawImage(new DrawingImage(readback), m_screenCells.Next);
            }

            // Test DrawingImage with some "bad" rects
            {
                DrawingGroup dg = new DrawingGroup();
                DrawingContext dc = dg.Open();

                dc.DrawRectangle(Brushes.Red, null, new Rect(0, 0, 20, 20));
                dc.Close();

                DrawingImage di = new DrawingImage(dg);

                _ctx.DrawImage(di, Rect.Empty);
                _ctx.DrawImage(di, new Rect(0, 0, 0, 0));
            }

            // Test ImageBrush intermediate-surface bounds
            {
                //
                // Create boilerplate brushes with mapped image bounds that are exactly
                // equal to the viewport.
                //
                BitmapSource tinyCheckered = BitmapFrame.Create(new Uri("drtfiles\\drtmil2d\\tiny_checkered.PNG", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.Default);

                ImageBrush notTiled = new ImageBrush(tinyCheckered);

                // These tests are small, so use a single screen cell for all of them
                Rect partitionedCell = m_screenCells.Next;

                AreaPartitioner cellPartitioner = new AreaPartitioner(
                    partitionedCell,
                    7,
                    7
                    );

                Rect r = cellPartitioner.Next;

                // Create & display non-tiled brush
                notTiled.Viewport = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0);
                notTiled.ViewportUnits = BrushMappingMode.Absolute;
                notTiled.AlignmentX = AlignmentX.Left;
                notTiled.AlignmentY = AlignmentY.Top;
                notTiled.Stretch= Stretch.None;

                _ctx.DrawRectangle(notTiled, null, r);

                // Create & display tiled brush
                ImageBrush tiled = notTiled.Clone();
                r = cellPartitioner.Next;
                tiled.Viewport  = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0);
                tiled.TileMode = TileMode.Tile;

                _ctx.DrawRectangle(tiled, null, r);

                // Test non-intermediate side of viewport clipping boundry
                {
                    // Test non-tiled clipped brush
                    double intermediateSurfaceBoundry = 1.0 / 1024.0;
                    ImageBrush notTiledBoundry = notTiled.Clone();
                    r = cellPartitioner.Next;

                    notTiledBoundry.Viewport  = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0 - intermediateSurfaceBoundry);

                    _ctx.DrawRectangle(notTiledBoundry, null, r);

                    // Test tiled clipped brush
                    ImageBrush tiledBoundry = tiled.Clone();
                    r = cellPartitioner.Next;

                    tiledBoundry.Viewport  = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0 - intermediateSurfaceBoundry);

                    _ctx.DrawRectangle(tiledBoundry, null, r);
                }

                // Test side of the viewport clipping boundry which creates intermediates
                {
                    // Test non-tiled clipped brush (doesn't use intermediate)

                    double intermediateSurfaceBoundry = 1.0 / 1000.0;
                    ImageBrush notTiledBoundry = notTiled.Clone();
                    r = cellPartitioner.Next;

                    notTiledBoundry.Viewport  = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0 - intermediateSurfaceBoundry);

                    _ctx.DrawRectangle(notTiledBoundry, null, r);

                    // Test tiled clipped brush (does use intermediate)

                    ImageBrush tiledBoundry = tiled.Clone();
                    r = cellPartitioner.Next;

                    tiledBoundry.Viewport  = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0 - intermediateSurfaceBoundry);

                    _ctx.DrawRectangle(tiledBoundry, null, r);
                }

                // Test non-intermediate side of viewport padding boundry
                {
                    // Test non-tiled padded brush

                    double intermediateSurfaceBoundry = 1.0 / 1024.0;
                    ImageBrush notTiledBoundry = notTiled.Clone();
                    r = cellPartitioner.Next;

                    notTiledBoundry.Viewport  = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0 + intermediateSurfaceBoundry);

                    _ctx.DrawRectangle(notTiledBoundry, null, r);

                    // Test tiled padded brush

                    ImageBrush tiledBoundry = tiled.Clone();
                    r = cellPartitioner.Next;

                    tiledBoundry.Viewport  = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0 + intermediateSurfaceBoundry);

                    _ctx.DrawRectangle(tiledBoundry, null, r);
                }

                // Test side of the viewport padding boundry which creates intermediates
                {
                    // Test non-tiled padded brush (doesn't use intermediaes)
                    double intermediateSurfaceBoundry = 1.0 / 1000.0;
                    ImageBrush notTiledBoundry = notTiled.Clone();
                    r = cellPartitioner.Next;

                    notTiledBoundry.Viewport  = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0 + intermediateSurfaceBoundry);

                    _ctx.DrawRectangle(notTiledBoundry, null, r);

                    // Test tiled padded brush (does use intermediates)
                    ImageBrush tiledBoundry = tiled.Clone();
                    r = cellPartitioner.Next;

                    tiledBoundry.Viewport  = new Rect(r.Left + 1.0, r.Top + 1.0, 2.0, 2.0 + intermediateSurfaceBoundry);

                    _ctx.DrawRectangle(tiledBoundry, null, r);
                }
            }

            // 

            {
                VisualBrush visualBrush = new VisualBrush(CreateBrushVisual());

                visualBrush.Viewbox = Rect.Empty;

                _ctx.DrawRectangle(visualBrush, null, m_screenCells[0]);
            }

            // 

            {
                DrawingVisual visual = CreateBrushVisual();

                // Rotate by 45 degrees, which will cause the bounding box
                // to have a different size if the transform isn't included
                visual.Transform = new RotateTransform(45.0);

                // Any offset should be canceled out by the auto-calculation
                // functionality.
                visual.Offset = new Vector(1000.0, 2000.0);

                // Clip out the left 2/3 of the visual
                visual.Clip = new RectangleGeometry(new Rect(0.65, 0.0, 0.35, 1.0));

                _ctx.DrawRectangle(new VisualBrush(visual), null, m_screenCells.Next);
            }

            // 

            {
                ImageBrush ib = new ImageBrush(tulipImage);

                ib.ViewboxUnits = BrushMappingMode.Absolute;
                ib.Viewbox = new Rect (0, 0, 50, 50);
                ib.TileMode = TileMode.Tile;
                ib.Viewport = new Rect (0, 0, 0.5, 0.5);

                _ctx.DrawRectangle(ib, null, m_screenCells.Next);
            }
        }

        public DRTMil2DBrushSuite() : base("Brush")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext ctx = visual.RenderOpen();
            Render(ctx);
            ctx.Close();

            _rootVisual = visual;

            DRT.RootElement = _rootVisual;
            DRT.ShowRoot();

            return new DrtTest[]  { new DrtTest(RunTest)};
        }

        void RunTest()
        {
            DRT.WaitForCompleteRender(); // Wait until original Visual is displayed
            _testChangedVisualBrush.Visual = CreateBrushVisual();

            if (DRTMil2D.IsInteractive)
            {
                DRT.Pause(5000);
            }
            else
            {
                DRT.WaitForCompleteRender();
            }
            Console.WriteLine("    DRTMil2DBrushSuite.RunTest() completed.");
        }

        internal struct TwoStopTest
        {
            internal double positionOne;
            internal double positionTwo;
            internal bool greenFirst;
        };

        internal static AreaPartitioner m_screenCells;

        private VisualBrush _testChangedVisualBrush;

        private Visual _rootVisual;
        private DrawingContext _ctx;

        private const int m_penThickness  = 6;
    }
}

