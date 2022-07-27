// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;                // for UIContext WindowsBase.dll
using System.Security;
using System.Security.Permissions;
using System.Collections;              // for ArrayList
using System.IO;                       // for File, Stream

using System.Globalization;

using System.Windows.Documents;        // for FixedPage PresentationFramework.dll
using System.Windows;                  // for Size           WindowsBase.dll
using System.Windows.Markup;    // for ParserContext PresentationFramework.dll
using System.Windows.Controls;         // for FixedPanel    PresentationFramework.dll
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;           // for Glyphs
using System.Windows.Media.Animation;

using System.Diagnostics;
using System.Printing;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO.Packaging;

using DRT;

namespace Microsoft.PrintTest
{
    internal class MyTextWriterTraceListener : TextWriterTraceListener
    {
        public MyTextWriterTraceListener(TextWriter writer)
            : base(writer)
        {
        }

        public override void WriteLine(String s)
        {
        }

        public override void Flush()
        {
        }
    }
    
    public class BrushTest : DrawingVisual
    {
        public BrushTest() : base()
        {
            using (DrawingContext ctx = RenderOpen())
            {
                // DrawingBrush with differetn Alignment and Stretch modes
                DrawingGroup brushDrawing = new DrawingGroup();
                DrawingContext c = brushDrawing.Open();

                c.DrawRectangle(Brushes.Blue,   null, new Rect(0,  0, 24, 24));
                c.DrawRectangle(Brushes.Yellow, null, new Rect(24, 0, 24, 24));
                c.Close();

                DrawingBrush db = new DrawingBrush(brushDrawing);

                db.Viewbox       = new Rect(0, 0, 48, 24);
                db.TileMode      = TileMode.Tile;
                db.ViewportUnits = BrushMappingMode.Absolute;

                Stretch[] modes = { Stretch.None, Stretch.Fill, Stretch.Uniform, Stretch.UniformToFill };

                Pen black = new Pen(Brushes.Black, 1);

                for (int i = 0; i < 4; i++)
                {
                    db.Stretch = modes[i];

                    double y = 40 + 100 * i;

                    db.Viewport = new Rect(40, y, 60, 40);
                    db.AlignmentX = AlignmentX.Left;
                    db.AlignmentY = AlignmentY.Top;
                    ctx.DrawRectangle(db, black, new Rect(40, y, 120, 80));

                    db.Viewport = new Rect(180, y, 60, 40);
                    db.AlignmentX = AlignmentX.Center;
                    db.AlignmentY = AlignmentY.Center;
                    ctx.DrawRectangle(db, black, new Rect(180, y, 120, 80));

                    db.Viewport = new Rect(320, y, 60, 40);
                    db.AlignmentX = AlignmentX.Right;
                    db.AlignmentY = AlignmentY.Bottom;
                    ctx.DrawRectangle(db, black, new Rect(320, y, 120, 80));
                }
            }
        }
    }

    public class ProfileHelper 
    {
        internal static readonly string WindowsColorProfiles = GetSystemColorProfilesDirectory();

        ///<summary>
        /// returns stream when trying to open profiles from a string
        ///</summary>
        internal static Uri GetUriFromProfileName(string filename)
        {
            Uri profileUri = null;

            if (File.Exists(filename))
            {
                profileUri = new Uri("file://" + Directory.GetCurrentDirectory() + "/" + filename);
            }
            else if (File.Exists(WindowsColorProfiles + filename))
            {
                profileUri = new Uri("file://"+WindowsColorProfiles+filename);
            }
            else
            {
                throw new ArgumentNullException("fileStream");
            }

            return profileUri;
        }

        [EnvironmentPermission(SecurityAction.Assert, Read = "windir")]
        private static string GetSystemColorProfilesDirectory()
        {
            string s = Environment.GetEnvironmentVariable("windir") + @"\system32\spool\drivers\color\";

            return s.ToLower(CultureInfo.InvariantCulture);
        }
    }
    
    public class ContextColorTest : DrawingVisual
    {
        public ContextColorTest() : base()
        {
            using (DrawingContext ctx = RenderOpen())
            {
                float[] rgb_in = new float[] { 0.1f, 0.4f, 0.7f };
                Brush b1 = new SolidColorBrush(Color.FromValues(rgb_in, ProfileHelper.GetUriFromProfileName("sRGB Color Space Profile.icm")));
                ctx.DrawRectangle(b1, null, new Rect(10, 10, 20, 20));

                Brush b2 = new SolidColorBrush(Color.FromAValues(0.4f, rgb_in, ProfileHelper.GetUriFromProfileName("sRGB Color Space Profile.icm")));
                ctx.DrawRectangle(b2, null, new Rect(20, 20, 30, 30));

                float[] cmyk_in = new float[] { 0.1f, 0.4f, 0.7f, 1.0f };
                Brush b3 = new SolidColorBrush(Color.FromValues(cmyk_in, ProfileHelper.GetUriFromProfileName(@"DrtFiles\VisualSerialization\testCMYK1.icc")));
                ctx.DrawRectangle(b3, null, new Rect(30, 30, 40, 40));

                Brush b4 = new SolidColorBrush(Color.FromAValues(0.5f, cmyk_in, ProfileHelper.GetUriFromProfileName(@"DrtFiles\VisualSerialization\testCMYK1.icc")));
                ctx.DrawRectangle(b4, null, new Rect(40, 40, 50, 50));
            }
        }
    }
    
    public class SampleImage : DrawingVisual
    {
        public SampleImage() : base()
        {
            using (DrawingContext ctx = RenderOpen())
            {
                BitmapImage tulip = new BitmapImage(new Uri("drtfiles\\VisualSerialization\\tulip.jpg", UriKind.RelativeOrAbsolute));

                // ctx.DrawRectangle(Brushes.Blue, null, new Rect(90, 90, 220, 220));
                ctx.DrawImage(tulip, new Rect(100, 100, 200, 200));
            }

            Color blue    = Color.FromScRgb(0.1f, 0.0f, 0.0f, 1.0f);
            Color yellow = Color.FromScRgb(0.9f, 1.0f, 1.0f, 0.0f);

            Brush horGradientBrush = new LinearGradientBrush(blue, yellow, 0);

            OpacityMask = horGradientBrush;
        }
    }

    public class Smily : DrawingVisual
    {
        public Smily() : base()
        {
            using (DrawingContext ctx = RenderOpen())
            {
                // Draw checkered green background squares
                ctx.DrawRectangle(
                    Brushes.Green,
                    null,
                    new Rect(0.0, 0.0, 0.5, 0.5));

                ctx.DrawRectangle(
                    Brushes.Green,
                    null,
                    new Rect(0.5, 0.5, 0.5, 0.5));

                // Draw large yellow 'face' circle
                ctx.DrawEllipse(
                    Brushes.Yellow,
                    new Pen(Brushes.Black, 0.02),
                    new Point(0.5, 0.5),
                    0.4, 0.4);

                // Draw the 'smile'
                GeometryConverter converter = new GeometryConverter();
                Geometry geometry = (Geometry)converter.ConvertFromString("M 0.25 0.6 C 0.4 0.75 0.6 0.75 0.75 0.6");

                ctx.DrawGeometry(
                    null,
                    new Pen(Brushes.Black, 0.02),
                    geometry);

                // Draw the 'eyes'
                ctx.DrawEllipse(
                    Brushes.Black,
                    null,
                    new Point(0.4, 0.4),
                    0.03, 0.125);

                ctx.DrawEllipse(
                    Brushes.Black,
                    null,
                    new Point(0.6, 0.4),
                    0.03, 0.125);

            }
        }

    }
    
    public class SampleDrawingVisual : DrawingVisual
    {
        public SampleDrawingVisual() : base()
        {
            using (DrawingContext ctx = RenderOpen())
            {
                Render(ctx);
            }
        }

        static Drawing GetDrawing(Transform trans)
        {
            // Setup drawing for DrawingBrushes
            DrawingGroup brushDrawing = new DrawingGroup();
            DrawingContext ctx = brushDrawing.Open();

            ctx.PushTransform(trans);

            // Draw checkered green background squares
            ctx.DrawRectangle(
                Brushes.Green,
                null,
                new Rect(0.0, 0.0, 0.5, 0.5));

            ctx.DrawRectangle(
                Brushes.Green,
                null,
                new Rect(0.5, 0.5, 0.5, 0.5));

            // Draw large yellow 'face' circle
            ctx.DrawEllipse(
                Brushes.Yellow,
                new Pen(Brushes.Black, 0.02),
                new Point(0.5, 0.5),
                0.4, 0.4);

            // Draw the 'smile'
            GeometryConverter converter = new GeometryConverter();
            Geometry geometry = (Geometry)converter.ConvertFromString("M 0.25 0.6 C 0.4 0.75 0.6 0.75 0.75 0.6");

            ctx.DrawGeometry(
                null,
                new Pen(Brushes.Black, 0.02),
                geometry);

            // Draw the 'eyes'
            ctx.DrawEllipse(
                Brushes.Black,
                null,
                new Point(0.4, 0.4),
                0.03, 0.125);

            ctx.DrawEllipse(
                Brushes.Black,
                null,
                new Point(0.6, 0.4),
                0.03, 0.125);

            ctx.Pop();
            ctx.Close();

            return brushDrawing;
        }

        static DrawingBrush GetDrawingBrush()
        {
            return new DrawingBrush(GetDrawing(new ScaleTransform(1, 1)));
        }

        public static  void Render(DrawingContext ctx)
        {
            const double inch = 96.0;

            if (null == ctx) return;

            Color gray = Color.FromScRgb(1.0f, 0.5f, 0.5f, 0.5f);

            ctx.DrawRectangle(new SolidColorBrush(gray), null, new Rect(inch / 2, inch / 2, inch * 6, inch * 7));

            Color blue = Color.FromScRgb(1.0f, 0.0f, 0.0f, 1.0f);
            Color red = Color.FromScRgb(1.0f, 1.0f, 0.0f, 0.0f);
            Color yellow = Color.FromScRgb(1.0f, 1.0f, 1.0f, 0.0f);

            Brush colorBrush = new SolidColorBrush(blue);

            ColorAnimation canim;

            // Red to Yellow animation
            canim = new ColorAnimation(Colors.Red, Colors.Yellow, new TimeSpan(0, 0, 0, 0, 1000));
            canim.RepeatBehavior = RepeatBehavior.Forever;
            canim.AutoReverse = true;

//          colorBrush.GetAnimations(SolidColorBrush.ColorProperty).Add(canim);

            Brush horGradientBrush = new LinearGradientBrush(red, yellow, 0);
            Brush verGradientBrush = new LinearGradientBrush(yellow, blue, 90);

            Brush radGradientBrush = new RadialGradientBrush(blue, yellow);

            // RectangleGeometry rectGeom = new RectangleGeometry (new Rect (10.0, 10.0, 100.0, 100.0), 10.0, 10.0);
            // ctx.DrawGeometry (null, new Pen (Brushes.Black, 1), rectGeom);

            double r = 1;

            // 11
            ctx.DrawRoundedRectangle(colorBrush, null,
                    new Rect(inch, inch, inch * 1.5, inch * 1.5),
                    r * inch / 8, r * inch / 8);

            // 13
            ctx.DrawRoundedRectangle(horGradientBrush, null,
                    new Rect(inch, inch * 3, inch * 1.5, inch * 1.5),
                    r * inch / 8, r * inch / 8);

            // 15
            ctx.DrawRoundedRectangle(verGradientBrush, null,
                    new Rect(inch, inch * 5, inch * 1.5, inch * 1.5),
                    r * inch / 8, r * inch / 8);

            // 31
            ctx.DrawRoundedRectangle(verGradientBrush, null,
                    new Rect(inch * 3, inch, inch * 1.5, inch * 1.5),
                    r * inch / 2, r * inch / 2);

            // 33
            ctx.DrawRoundedRectangle(radGradientBrush, null,
                    new Rect(inch * 3, inch * 3, inch * 1.5, inch * 1.5),
                    r * inch / 2, r * inch / 2);

            // 35
            ctx.DrawRoundedRectangle(GetDrawingBrush(), null,
                new Rect(inch * 3, inch * 5, inch * 1.5, inch * 1.5),
                r * inch / 2, r * inch / 2);

            // 51
            ctx.DrawRoundedRectangle(new VisualBrush(new Smily()), null,
                new Rect(inch * 5, inch * 1, inch * 1.5, inch * 1.5),
                r * inch / 2, r * inch / 2);

            // 53
            ctx.PushTransform(new TranslateTransform(inch * 5, inch * 3));
            ctx.DrawDrawing(GetDrawing(new ScaleTransform(100, 100)));
            ctx.Pop();
        }
    }

    public class GeometriesDrawingVisual : DrawingVisual
    {
        public GeometriesDrawingVisual()
            : base()
        {
            using (DrawingContext ctx = RenderOpen())
            {
                Render(ctx);
            }
        }

        private static void Draw(DrawingContext ctx, Geometry g, double y)
        {
            ctx.PushTransform(new TranslateTransform(0, y));
            ctx.DrawGeometry(
                null,
                new Pen(Brushes.Black, 2),
                g);
            ctx.Pop();
            ctx.PushTransform(new TranslateTransform(200, y));
            ctx.DrawGeometry(
                Brushes.Red,
                null,
                g);
            ctx.Pop();
            ctx.PushTransform(new TranslateTransform(400, y));
            ctx.DrawGeometry(
                Brushes.Red,
                new Pen(Brushes.Black, 2),
                g);
            ctx.Pop();
        }

        public static void Render(DrawingContext ctx)
        {
            Geometry g;

            if (null == ctx) return;

            {
                PathFigure p1 = new PathFigure();
                p1.StartPoint = new Point(0, 0);
                PolyLineSegment ps1 = new PolyLineSegment();
                ps1.Points.Add(new Point(100, 0));
                ps1.Points.Add(new Point(100, 100));
                ps1.Points.Add(new Point(0, 100));
                p1.Segments.Add(ps1);
                PathGeometry g1 = new PathGeometry();
                g1.Figures.Add(p1);

                PathFigure p2 = new PathFigure();
                p2.StartPoint = new Point(50, 50);
                PolyLineSegment ps2 = new PolyLineSegment();
                ps2.Points.Add(new Point(150, 50));
                ps2.Points.Add(new Point(150, 150));
                ps2.Points.Add(new Point(50, 150));
                ps2.IsStroked = true;
                p2.Segments.Add(ps2);
                g1.Figures.Add(p2);


                PathFigure p3 = new PathFigure();
                p3.StartPoint = new Point(0, 200);
                PolyLineSegment ps3 = new PolyLineSegment();
                ps3.Points.Add(new Point(100, 200));
                ps3.Points.Add(new Point(100, 300));
                ps3.Points.Add(new Point(0, 300));
                p3.Segments.Add(ps3);
                PathGeometry g2 = new PathGeometry();
                g2.Figures.Add(p3);

                PathFigure p4 = new PathFigure();
                p4.StartPoint = new Point(50, 250);
                PolyLineSegment ps4 = new PolyLineSegment();
                ps4.Points.Add(new Point(150, 250));
                ps4.Points.Add(new Point(150, 350));
                ps4.Points.Add(new Point(50, 350));
                ps4.IsStroked = true;
                p4.Segments.Add(ps4);
                p4.IsClosed = true;
                g2.Figures.Add(p4);

                g = new CombinedGeometry(GeometryCombineMode.Union, g1, g2);
                g.Transform = new ScaleTransform(0.5, 0.4);

                Draw(ctx, g, 0);

                g = new GeometryGroup();
                g.Transform = new ScaleTransform(0.5, 0.4);
                ((GeometryGroup)g).Children.Add(g1);
                ((GeometryGroup)g).Children.Add(g2);

                Draw(ctx, g, 200);
            }
            {
                PathFigure p1 = new PathFigure();
                p1.StartPoint = new Point(0, 0);
                PolyLineSegment ps1 = new PolyLineSegment();
                ps1.Points.Add(new Point(100, 0));
                ps1.Points.Add(new Point(100, 100));
                ps1.Points.Add(new Point(0, 100));
                p1.Segments.Add(ps1);
                PathGeometry g1 = new PathGeometry();
                g1.Figures.Add(p1);

                PathFigure p2 = new PathFigure();
                p2.StartPoint = new Point(50, 50);
                PolyLineSegment ps2 = new PolyLineSegment();
                ps2.Points.Add(new Point(150, 50));
                ps2.Points.Add(new Point(150, 150));
                ps2.Points.Add(new Point(50, 150));
                ps2.IsStroked = true;
                p2.Segments.Add(ps2);
                g1.Figures.Add(p2);

                g1.FillRule = FillRule.Nonzero;


                PathFigure p3 = new PathFigure();
                p3.StartPoint = new Point(0, 200);
                PolyLineSegment ps3 = new PolyLineSegment();
                ps3.Points.Add(new Point(100, 200));
                ps3.Points.Add(new Point(100, 300));
                ps3.Points.Add(new Point(0, 300));
                p3.Segments.Add(ps3);
                PathGeometry g2 = new PathGeometry();
                g2.Figures.Add(p3);

                PathFigure p4 = new PathFigure();
                p4.StartPoint = new Point(50, 250);
                PolyLineSegment ps4 = new PolyLineSegment();
                ps4.Points.Add(new Point(150, 250));
                ps4.Points.Add(new Point(150, 350));
                ps4.Points.Add(new Point(50, 350));
                ps4.IsStroked = true;
                p4.Segments.Add(ps4);
                p4.IsClosed = true;
                g2.Figures.Add(p4);

                g2.FillRule = FillRule.Nonzero;

                g = new CombinedGeometry(GeometryCombineMode.Union, g1, g2);
                g.Transform = new ScaleTransform(0.5, 0.4);

                Draw(ctx, g, 400);

                g = new GeometryGroup();
                g.Transform = new ScaleTransform(0.5, 0.4);
                ((GeometryGroup)g).FillRule = FillRule.Nonzero;
                ((GeometryGroup)g).Children.Add(g1);
                ((GeometryGroup)g).Children.Add(g2);

                Draw(ctx, g, 600);
            }
        }
    }

    class MyRender : Render
    {
        public MyRender()
            : base(96, "visualserializationtest.html", "Visual Serialization Test", "Original", "S0 Reloaded")
        {
        }

        public void Test(string name, Visual v, double width, double height, string comment)
        {
            if (v is UIElement)
            {
                Toolbox.SaveAsXml(name + "_org.xaml", v, true);
            }

            string filename = name + "_s0.xaml";

            Toolbox.SaveAsXml(filename, v, false);

            Size size;

            Visual after = Toolbox.LoadVisual(filename, out size);

            Compare(v, after, name, width, height, comment, 0);
        }

        public void Test(string name, Visual v, double width, double height, string comment, bool compare)
        {
            if (v is UIElement)
            {
                Toolbox.SaveAsXml(name + "_org.xaml", v, true);
            }

            string filename = name + "_s0.xaml";

            Toolbox.SaveAsXml(filename, v, false);

            if (compare)
            {
                Size size;

                Visual after = Toolbox.LoadVisual(filename, out size);

                Compare(v, after, name, width, height, comment, 0);
            }
        }
    }

    public class StopWatch
    {
        double   _total = 0;
        DateTime _startTime;
        
        public void Start()
        {
            _startTime = DateTime.Now;
        }

        public void Stop()
        {
            TimeSpan elapsed = DateTime.Now - _startTime;

            _total += elapsed.TotalSeconds;

            Console.WriteLine("{0} seconds", _total);
        }
    }

    public class VisualSerializationDRT : DrtBase
    {
        static string  s_filename;
        static bool    s_delay;
        static DrtBase s_instance;
        
        static FixedDocument CreateDocumentPaginator()
        {
            FixedDocument testFixedPanel = new FixedDocument();

        //  double pageWidth = 240.0;
        //  double pageHeight = 240.0 * 1.618;

            PageContent pageContent = CreateFixedTextPage();
        //  testFixedPanel.PageWidth = pageWidth;
        //  testFixedPanel.PageHeight = pageHeight;
            
            FixedPage pageVisual = pageContent.GetPageRoot(false);

            pageVisual.Width = 200.0;
            pageVisual.Height = 200.0 * 1.618;
            testFixedPanel.Pages.Add(pageContent);

            PageContent anotherPageContent = CreateFixedTextPage();
            FixedPage anotherPageVisual = anotherPageContent.GetPageRoot(false);
            anotherPageVisual.Width = 200.0;
            anotherPageVisual.Height = 200.0 * 1.618;
            testFixedPanel.Pages.Add(anotherPageContent);

            return testFixedPanel;
        }

        static void SetEllipse(Ellipse shape, double cx, double cy, double rx, double ry)
        {
            Thickness thick = new Thickness();
            thick.Left = cx - rx;
            thick.Top  = cy - ry;

            shape.Margin = thick;
            shape.Width  = rx * 2;
            shape.Height = ry * 2;
        }

        static FixedPage CreateGradientBrushTest()
        {
            FixedPage fixedPage = new FixedPage();

            Canvas canvas = new Canvas();

            RadialGradientBrush brush = new RadialGradientBrush();
            
            brush.GradientStops.Add(new GradientStop(Colors.Black, 0));
            brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.5));
            brush.GradientStops.Add(new GradientStop(Colors.Red, 1));

            brush.SpreadMethod = GradientSpreadMethod.Repeat;
            brush.Center = new Point(0.5, 0.5);
            brush.RadiusX = 0.2;
            brush.RadiusY = 0.2;
            brush.GradientOrigin = new Point(0.5, 0.5);
            
            Rectangle r = new Rectangle();
            r.Fill = brush;
            Thickness thick = new Thickness();
            thick.Left = 100;
            thick.Top  = 100;
            r.Margin = thick;
            r.Width = 400;
            r.Height = 400;

            canvas.Children.Add(r);

            LinearGradientBrush linear = new LinearGradientBrush();

            linear.GradientStops.Add(new GradientStop(Colors.Blue, 0));
            linear.GradientStops.Add(new GradientStop(Colors.Yellow, 1));

            linear.SpreadMethod = GradientSpreadMethod.Reflect;
            linear.StartPoint = new Point(0, 0);
            linear.EndPoint = new Point(0.06125, 0);
            linear.Opacity = 0.5;

            r = new Rectangle();
            r.Fill = linear;
            thick = new Thickness();
            thick.Left = 200;
            thick.Top = 200;
            r.Margin = thick;
            r.Width = 400;
            r.Height = 400;

            canvas.Children.Add(r);

            fixedPage.Children.Add(canvas);

            Size sz = new Size(8.5 * 96, 11 * 96);

            fixedPage.Measure(sz);
            fixedPage.Arrange(new Rect(new Point(), sz));
            fixedPage.UpdateLayout();

            return fixedPage;
        }

        static FixedPage CreateFixedPage(bool full)
        {
            FixedPage fixedPage = new FixedPage();

            Canvas canvas1 = new Canvas();

            // Top-Left
            TextBlock label = new TextBlock();
            label.Foreground = Brushes.Red;
         // label.ContentEnd.InsertText("TopLeft");
            Canvas.SetTop(label, 100);
            Canvas.SetLeft(label, 0);
            canvas1.Children.Add(label);

            if (full)
            {
                // Bottom-Right
                label = new TextBlock();
                label.Foreground = Brushes.Green;
           //   label.ContentEnd.InsertText("BottomRight");
                Canvas.SetBottom(label, 0);
                Canvas.SetRight(label, 0);
                canvas1.Children.Add(label);

                // Top-Right
                label = new TextBlock();
                label.Foreground = Brushes.Blue;
          //    label.ContentEnd.InsertText("TopRight");
                Canvas.SetTop(label, 0);
                Canvas.SetRight(label, 0);
                canvas1.Children.Add(label);

                // Bottom-Left
                label = new TextBlock();
                label.Foreground = Brushes.Cyan;
          //    label.ContentEnd.InsertText("BottomLeft");
                Canvas.SetBottom(label, 0);
                Canvas.SetLeft(label, 0);
                canvas1.Children.Add(label);
            }

            //
            // Adding a rectangle to the page
            //
            FileStream fs = new FileStream("drtfiles\\VisualSerialization\\avalon.png", FileMode.Open, FileAccess.Read);
            BitmapImage avalon = new BitmapImage(new Uri("drtfiles\\VisualSerialization\\avalon.png", UriKind.RelativeOrAbsolute));
            // BitmapImage avalon = new BitmapImage(fs), BitmapCreateOptions.None, BitmapCacheOption.Default);

            Rectangle firstRectangle = new Rectangle();
            firstRectangle.Fill = new ImageBrush(avalon);
            Thickness thick = new Thickness();
            thick.Left = 100;
            thick.Top = 100;
            firstRectangle.Margin = thick;
            firstRectangle.Width = 100;
            firstRectangle.Height = 100;

            // ((IAddChild)fixedPage)->AddChild(firstRectangle);
            canvas1.Children.Add(firstRectangle);

            if (full)
            {
                //
                //Adding a button to the page
                //
                Button firstButton = new Button();
                firstButton.Background = Brushes.Red;
                firstButton.BorderBrush = Brushes.Black;
                firstButton.BorderThickness = new Thickness(2);
                firstButton.Content = "I am button 1...";
                firstButton.FontSize = 16;

                canvas1.Children.Add(firstButton);
            }

            //
            // Adding an Ellipse
            //
            Ellipse firstEllipse = new Ellipse();
            SolidColorBrush firstSolidColorBrush = new SolidColorBrush(Colors.DarkCyan);
            firstSolidColorBrush.Opacity = 0.5;
            firstEllipse.Fill = firstSolidColorBrush;
            SetEllipse(firstEllipse, 200, 200, 100, 100);

            canvas1.Children.Add(firstEllipse);

            //
            // Adding an Ellipse with contextcolor
            //
            float[] rgb_in = new float[] { 1.0f, 0.5f, 0.0f };
            Ellipse contextEllipse1 = new Ellipse();
            SolidColorBrush context1Brush = new SolidColorBrush(Color.FromValues(rgb_in, ProfileHelper.GetUriFromProfileName("sRGB Color Space Profile.icm")));
            contextEllipse1.Fill = context1Brush;
            SetEllipse(contextEllipse1, 300, 300, 100, 100);

            canvas1.Children.Add(contextEllipse1);

#if false
            //
            // Adding an Ellipse with contextcolor
            //
            Ellipse contextEllipseScRGB = new Ellipse();
            SolidColorBrush scRGBBrush = new SolidColorBrush(Color.FromScRgb(0.4f,0.5f,0.6f,0.7f));
            contextEllipseScRGB.Fill = scRGBBrush;
            SetEllipse(contextEllipseScRGB, 240, 350, 100, 100);

            canvas1.Children.Add(contextEllipseScRGB);
#endif            

            //
            // Adding an Ellipse with contextcolor
            //
            Ellipse contextEllipse2 = new Ellipse();
            SolidColorBrush context2Brush = new SolidColorBrush(Color.FromAValues(0.5f, rgb_in, ProfileHelper.GetUriFromProfileName("sRGB Color Space Profile.icm")));
            contextEllipse2.Fill = context2Brush;
            SetEllipse(contextEllipse2, 300, 300, 80, 80);

            canvas1.Children.Add(contextEllipse2);

            //
            // Adding an Ellipse with contextcolor
            //
            float[] cmyk_in = new float[] { 1.0f, 0.5f, 0.0f, 0.7f };
            Ellipse contextEllipse3 = new Ellipse();
            SolidColorBrush context3Brush = new SolidColorBrush(Color.FromValues(cmyk_in, ProfileHelper.GetUriFromProfileName(@"DrtFiles\VisualSerialization\testCMYK1.icc")));
            contextEllipse3.Fill = context3Brush;
            SetEllipse(contextEllipse3, 300, 300, 60, 60);

            canvas1.Children.Add(contextEllipse3);

            //
            // Adding an Ellipse with contextcolor
            //
            Ellipse contextEllipse4 = new Ellipse();
            SolidColorBrush context4Brush = new SolidColorBrush(Color.FromAValues(0.5f, cmyk_in, ProfileHelper.GetUriFromProfileName(@"DrtFiles\VisualSerialization\testCMYK1.icc")));
            contextEllipse4.Fill = context4Brush;
            SetEllipse(contextEllipse4, 300, 300, 40, 40);

            canvas1.Children.Add(contextEllipse4);

            fixedPage.Children.Add(canvas1);

            Size sz = new Size(8.5 * 96, 11 * 96);

            fixedPage.Measure(sz);
            fixedPage.Arrange(new Rect(new Point(), sz));
            fixedPage.UpdateLayout();

            return fixedPage;
        }

        static PageContent  CreateFixedTextPage()
        {
            PageContent pageContent  = new PageContent();
            FixedPage fixedPage = CreateFixedPage(true);

            ((IAddChild)pageContent).AddChild(fixedPage);
            
            return pageContent;
        }

        public static void TestPrintableArchivableDocumentReachPackage()
        {
            Console.WriteLine("TestPrintableArchivableDocumentReachPackage");

            Package container = Package.Open("test.xps", FileMode.Create);

            XpsDocument rp = new XpsDocument(container);

            XpsDocumentWriter pad = XpsDocument.CreateXpsDocumentWriter(rp);

            //write a sample FixedDocument
            FixedDocument  testDocument = CreateDocumentPaginator();
            pad.Write(testDocument);

            container.Close();
        }

        public static void TestPrintableArchivableDocumentPrintQueue()
        {
            Console.WriteLine("TestPrintableArchivableDocumentPrintQueue");

            PrintQueue queue = PrintHelp.GetDefaultPrintQueue(s_instance);

            XpsDocumentWriter pad = PrintQueue.CreateXpsDocumentWriter(queue);

            //write a sample FixedDocument
            FixedDocument testDocument = CreateDocumentPaginator();
            pad.Write(testDocument);
        }

        public static void SerializeToFile()
        {
            MyRender render = new MyRender();
            
            FixedPage page = CreateFixedPage(true);

            Toolbox.SaveAsXml("fixedpage_org.xaml", page, true);

            render.Test("FixedPage", page, 6 * 96, 6 * 96, "FixedPage");

            render.Test("Alignment", new BrushTest(), 6 * 96, 6 * 96, "TileBrush Alignment");

            render.Test("ContextColor", new ContextColorTest(), 6 * 96, 6 * 96, "Context Color");

            render.Test("Drawingvisual", new SampleDrawingVisual(), 7 * 96, 8 * 96, "DrawingVisual");

            render.Test("OpacityMask", new SampleImage(), 4 * 96, 4 * 96, "OpacityMask");

            render.Test("GradientBrush", CreateGradientBrushTest(), 6 * 96, 6 * 96, "GradientBrush");

            render.Test("Geometries", new GeometriesDrawingVisual(), 7 * 96, 8 * 96, "Geometries");

            render.Close();
        }

        static public void ExternalFileSerialization()
        {
            Debug.Assert(s_filename != null);

            if (s_delay)
            {
                for (int i = 0; i < 5; i++)
                {
                    Size s;

                    Visual v = Toolbox.Load(s_filename, out s);

                    if (v != null)
                    {
                        UIElement p = v as UIElement;

                        if (p != null)
                        {
                            Toolbox.UpdateLayout(p, s);
                            Console.WriteLine("DesiredSize ={0}", p.DesiredSize);
                        }
                    }
                }
            }

            MyRender render = new MyRender();
    
            string nameonly = s_filename;
            
            if (s_filename.LastIndexOf('.') > 1)
            {
                s_filename.Substring(0, s_filename.LastIndexOf('.'));
            }
            
            Size size;

            Visual page = Toolbox.LoadVisual(s_filename, out size);

            render.Test(nameonly, page, size.Width, size.Height, nameonly, s_filename != "xpslimit");

            render.Close();
        }

        [STAThread]
        public static int Main(string[] args)
        {

            DrtBase drt = new VisualSerializationDRT();

            VisualSerializationDRT.s_instance = drt;

            return drt.Run(args);
        }

        private VisualSerializationDRT()
        {
            WindowTitle = "DrtVisualSerialization";
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = "DrtVisualSerialization";

            Suites = new DrtTestSuite[]{
                        new SerializationToFileTest(),
                        new SerializationToContainerTest()
                        };
        }

        // Return true if handled.
        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
            {
                return true;
            }

            if (option)
            {
                switch (arg)    // arg is lower-case, no leading - or /
                {
                    case "d": 
                        s_delay = true;
                        break;

                    case "p":
                        Suites = new DrtTestSuite[]{
                                        new SerializationToFileTest(),
                                        new SerializationToPrintQueueTest()
                                        };
                        break;

                    default:
                        return false;
                }
                
                return true;
            }
            else                            // non-option argument:   <filename>
            {
                s_filename = args [k++];
                
                Suites = new DrtTestSuite[]{new ExternalFileSerializationTest()};
                
                return true;
            }
        }

        // Print a description of command line arguments.  Derived classes should
        // override this to describe their own arguments, and then call
        // base.PrintOptions() to get the DrtBase description.
        protected override void PrintOptions()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("  filename ...  serialize named external file");
            Console.WriteLine("  -d       ...  add delay to make layout more complete");
            Console.WriteLine("  -p       ...  write to print queue instead of container");
            Console.WriteLine("");

            base.PrintOptions();
        }
    }

    public sealed class SerializationToFileTest : DrtTestSuite
    {
        public SerializationToFileTest() : base("SerializationToFile")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]{new DrtTest(VisualSerializationDRT.SerializeToFile)};
        }
    }

    public sealed class SerializationToContainerTest : DrtTestSuite
    {
        public SerializationToContainerTest(): base("SerializationToContainer")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] { new DrtTest(VisualSerializationDRT.TestPrintableArchivableDocumentReachPackage) };
        }
    }

    public sealed class SerializationToPrintQueueTest : DrtTestSuite
    {
        public SerializationToPrintQueueTest()
            : base("SerializationToPrintQueue")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] { new DrtTest(VisualSerializationDRT.TestPrintableArchivableDocumentPrintQueue) };
        }
    }


    public sealed class ExternalFileSerializationTest : DrtTestSuite
    {
        public ExternalFileSerializationTest()
            : base("ExternalFileSerializationTest")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] { new DrtTest(VisualSerializationDRT.ExternalFileSerialization) };
        }
    }

}
