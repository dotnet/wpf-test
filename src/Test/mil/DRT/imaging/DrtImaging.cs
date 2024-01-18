// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Copyright (C) Microsoft Corporation.  All rights reserved.
// Description: DRT for images 
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;
using System.ComponentModel; // for typeconverter
using System.Globalization;
using System.Windows.Interop;

namespace DRT
{
    public sealed class DrtImaging : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DrtImaging();

            int result = drt.Run(args);
            drt = null;

            // This is done so if leak detection is enabled we should have released everything on exit.
            GC.Collect(GC.MaxGeneration);

            return result;
        }

        private DrtImaging()
        {
            WindowTitle ="DrtImaging";
            TeamContact ="WPF";
            Contact ="Microsoft";
            DrtName ="DrtImaging";
            DelayOutput = false;

            Suites = new DrtTestSuite[] {
                new BitmapImageTests(),
                null
            };
        }

        internal static string DrtFiles
        {
            get { return @".\drtfiles\drtimaging\"; }
        }
    }

    /// <summary>
    /// Non-seekable stream
    /// </summary>
    internal class NonSeekableStream : Stream
    {
        Stream _baseStream;

        internal NonSeekableStream(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        public override bool CanRead  { get { return _baseStream.CanRead; } }
        public override bool CanSeek  { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override long Length   { get { throw new NotImplementedException(); } }
        public override long Position { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public override void Flush()  { throw new NotImplementedException(); }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
        public override void SetLength(long newLength) { throw new NotImplementedException(); }

        public override int Read(
            byte[] buffer,
            int offset,
            int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public override void Write(
           byte[] buffer,
           int offset,
           int count)
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginRead(
           byte[] buffer,
           int offset,
           int count,
           AsyncCallback callback,
           object state)
        {
            return _baseStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(
            IAsyncResult asyncResult)
        {
            return _baseStream.EndRead(asyncResult);
        }
    }

    public class MyBitmapSource : BitmapSource
    {
        public MyBitmapSource(int width, int height)
        {
            _pixelWidth = width;
            _pixelHeight = height;
        }

        public override PixelFormat Format
        {
            get
            {
                return PixelFormats.Pbgra32;
            }
        }

        public override int PixelWidth
        {
            get
            {
                return _pixelWidth;
            }
        }

        public override int PixelHeight
        {
            get
            {
                return _pixelHeight;
            }
        }

        public override double DpiX
        {
            get
            {
                return 96.0;
            }
        }

        public override double DpiY
        {
            get
            {
                return 96.0;
            }
        }

        public override BitmapPalette Palette
        {
            get
            {
                return null;
            }
        }

        public override void CopyPixels(Int32Rect sourceRect, Array pixels, int stride, int offset)
        {
            byte [] bytes = (byte [])pixels;

            if (sourceRect.Width == 0 || sourceRect.Height == 0)
            {
                sourceRect.X = 0;
                sourceRect.Y = 0;
                sourceRect.Width = PixelWidth;
                sourceRect.Height = PixelHeight;
            }

            for (int y = 0; y < sourceRect.Height; ++y)
            {
                for (int x = 0; x < sourceRect.Width; ++x)
                {
                    byte a,r,g,b;

                    GetColorAtPosition(sourceRect.X+x, sourceRect.Y+y, out a, out r, out g, out b);
                    bytes[offset+4*x+y*stride+0] = a;
                    bytes[offset+4*x+y*stride+1] = r;
                    bytes[offset+4*x+y*stride+2] = g;
                    bytes[offset+4*x+y*stride+3] = b;
                }
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new MyBitmapSource(0, 0);
        }

        protected override void CloneCore(Freezable sourceFreezable)
        {
            MyBitmapSource sourceBitmapSource = (MyBitmapSource) sourceFreezable;
            base.CloneCore(sourceFreezable);

            _pixelWidth = sourceBitmapSource.PixelWidth;
            _pixelHeight = sourceBitmapSource.PixelHeight;
        }

        protected override void CloneCurrentValueCore(Freezable sourceFreezable)
        {
            MyBitmapSource sourceBitmapSource = (MyBitmapSource) sourceFreezable;
            base.CloneCurrentValueCore(sourceFreezable);

            _pixelWidth = sourceBitmapSource.PixelWidth;
            _pixelHeight = sourceBitmapSource.PixelHeight;
        }

        private void GetColorAtPosition(int x, int y, out byte a, out byte r, out byte g, out byte b)
        {
            float xf = ((float)x)/PixelWidth;
            float yf = ((float)y)/PixelHeight;

            float af = xf;
            float rf = (1.0f-xf);
            float gf = yf;
            float bf = (1.0f-yf);

            a = (byte)(af*255);
            r = (byte)(rf*255);
            g = (byte)(gf*255);
            b = (byte)(bf*255);
        }

        private int _pixelWidth;
        private int _pixelHeight;
    }

    public class ImageVisual: System.Windows.Media.DrawingVisual
    {
        public ImageVisual(ImageSource imageSource) : base()
        {
            _imageSource = imageSource;

            using (System.Windows.Media.DrawingContext ctx = RenderOpen())
            {
                Render(ctx);
            }
        }

        void Render (System.Windows.Media.DrawingContext ctx)
        {
            ctx.DrawImage(_imageSource, new Rect(0, 0, _imageSource.Width, _imageSource.Height));
        }

        private ImageSource _imageSource;
    }

    #region VectorPage : DrawingVisual
    public class VectorPage : System.Windows.Media.DrawingVisual
    {
        public VectorPage () : base()
        {
            using (System.Windows.Media.DrawingContext ctx = RenderOpen())
            {
                Render(ctx);
            }
        }

        void Render (System.Windows.Media.DrawingContext ctx)
        {
            const float inch = 96.0f;

            if (null == ctx) return;

            System.Windows.Media.Color gray = System.Windows.Media.Color.FromScRgb (1.0f, 0.5f, 0.5f, 0.5f);

            ctx.DrawRectangle (new System.Windows.Media.SolidColorBrush (gray), null, new Rect (inch / 2, inch / 2, inch * 7.5f, inch * 10));

            System.Windows.Media.Color blue = System.Windows.Media.Color.FromScRgb (1.0f, 0.0f, 0.0f, 1.0f);
            System.Windows.Media.Color red = System.Windows.Media.Color.FromScRgb (1.0f, 1.0f, 0.0f, 0.0f);
            System.Windows.Media.Color yellow = System.Windows.Media.Color.FromScRgb (1.0f, 1.0f, 1.0f, 0.0f);
            System.Windows.Media.Brush colorBrush = new System.Windows.Media.SolidColorBrush (blue);
            System.Windows.Media.Brush horGradientBrush = new System.Windows.Media.LinearGradientBrush (red, yellow, 0);
            System.Windows.Media.Brush verGradientBrush = new System.Windows.Media.LinearGradientBrush (yellow, blue, 90);
            System.Windows.Media.Brush radGradientBrush = new System.Windows.Media.RadialGradientBrush (blue, yellow);

            // RectangleGeometry rectGeom = new RectangleGeometry (new Rect (10.0, 10.0, 100.0, 100.0), 10.0, 10.0);
            // ctx.DrawGeometry (null, new Pen (Brushes.Black, 1), rectGeom);
            float r = 1;

            ctx.DrawRoundedRectangle (colorBrush, null, new Rect (inch, inch, inch * 1.5, inch * 1.5), r * inch / 8, r * inch / 8);
            ctx.DrawRoundedRectangle (horGradientBrush, null, new Rect (inch, inch * 3, inch * 1.5, inch * 1.5), r * inch / 8, r * inch / 8);
            ctx.DrawRoundedRectangle (verGradientBrush, null, new Rect (inch, inch * 5, inch * 1.5, inch * 1.5), r * inch / 8, r * inch / 8);
            ctx.DrawRoundedRectangle (verGradientBrush, null, new Rect (inch * 3, inch, inch * 1.5, inch * 1.5), r * inch / 2, r * inch / 2);
            ctx.DrawRoundedRectangle (radGradientBrush, null, new Rect (inch * 3, inch * 3, inch * 1.5, inch * 1.5), r * inch / 2, r * inch / 2);
        }
    }
    #endregion

    #region VectorPage2 : DrawingVisual
    public class VectorPage2 : System.Windows.Media.DrawingVisual
    {
        public VectorPage2() : base()
        {
            using (System.Windows.Media.DrawingContext ctx = RenderOpen())
            {
                Render(ctx);
            }
        }

        void Render(System.Windows.Media.DrawingContext ctx)
        {
            if (null == ctx) return;

            System.Windows.Media.Color white = System.Windows.Media.Color.FromScRgb(1.0f, 1.0f, 1.0f, 1.0f);

            ctx.DrawRectangle(new System.Windows.Media.SolidColorBrush(white), null, new Rect(0, 0, 800, 100));

            System.Windows.Media.Color black = System.Windows.Media.Color.FromScRgb(1.0f, 0.0f, 0.0f, 0.0f);
            System.Windows.Media.Brush blackBrush = new System.Windows.Media.SolidColorBrush(black);
            System.Windows.Media.Pen blackPen = new System.Windows.Media.Pen(blackBrush, 1.0);

            System.Windows.Media.PathGeometry pg = new System.Windows.Media.PathGeometry();
            System.Windows.Media.PathFigure pf = new System.Windows.Media.PathFigure();

            pf.StartPoint = new Point(-35, 20);
            pf.Segments.Add(new LineSegment(new Point(-15, 30), true));
            pf.Segments.Add(new LineSegment(new Point(15, 30), true));
            pf.Segments.Add(new LineSegment(new Point(35, 20), true));
            pf.Segments.Add(new LineSegment(new Point(35, -20), true));
            pf.Segments.Add(new LineSegment(new Point(15, -30), true));
            pf.Segments.Add(new LineSegment(new Point(-15, -30), true));
            pf.Segments.Add(new LineSegment(new Point(-35, -20), true));
            pf.IsClosed = true;
            pg.Figures.Add(pf);

            float translate = 40.41f;
            for (float angle = 0; angle <= 90; angle += 10, translate += 80)
            {
                System.Windows.Media.TransformGroup grp = new System.Windows.Media.TransformGroup();
                grp.Children.Add(new System.Windows.Media.RotateTransform(angle));
                grp.Children.Add(new System.Windows.Media.TranslateTransform(translate, 50));
                pg.Transform = grp;
                ctx.DrawGeometry(null, blackPen, pg);
            }
        }
    }
    #endregion

    public class BitmapImageTests : DrtTestSuite
    {
        public BitmapImageTests() : base("Imaging Tests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            DrawingVisual visual = new DrawingVisual();

            _rootVisual = visual;
            DRT.RootElement = visual;
            DRT.ShowRoot();

            return new DrtTest[] {
                new DrtTest(Test_D3DImage),
                new DrtTest(Test_D3DImage_Force9),
                new DrtTest(Test_MetadataPolicyComponent),
                new DrtTest(Test_DecoderTest1),
                new DrtTest(Test_DecoderTest2),
                new DrtTest(Test_DecoderTest3),
                new DrtTest(Test_DecoderTest4),
                new DrtTest(Test_StreamRelease),
                new DrtTest(Test_SaveGPS),
                new DrtTest(Test_Metadata),
                new DrtTest(Test_Metadata_Inplace),
                new DrtTest(Test_ExtendedPfBitmap),
                new DrtTest(Test_Thumbnail),
                new DrtTest(Test_TiffThumbnail),
                new DrtTest(Test_RegressionTifNewSubFileType),
                new DrtTest(Test_UseEmbeddedColorContext),
                new DrtTest(Test_LossLessRotation),
                new DrtTest(Test_WriteableBitmap),
                new DrtTest(Test_InteropBitmap),
                new DrtTest(Test_AsyncDownload),
                new DrtTest(Test_CroppedBitmap),
                new DrtTest(Test_CustomLoader),
                new DrtTest(Test_ImmediateModeRendering),
                new DrtTest(Test_SaveTifAsGif),
                new DrtTest(Test_SaveTifAsWmp),
                new DrtTest(Test_SaveWmpAsTif),
                new DrtTest(Test_64bppBMP),
                new DrtTest(Test_LoadImageWithTypeConverter),
                new DrtTest(Test_Palette),
                new DrtTest(Test_Icons),
                new DrtTest(Test_Sections),
                new DrtTest(Test_Rotations),
                new DrtTest(Test_Encoders),
                new DrtTest(Test_DelayCreation),
                new DrtTest(Test_CustomBitmapSource),
                new DrtTest(Test_PixelFormats),
                new DrtTest(Test_Cloning),
                new DrtTest(Test_Cache),
                new DrtTest(Test_Alpha),
                new DrtTest(Test_WmpCanonical),
                new DrtTest(Test_WmpSpecific),
                new DrtTest(Test_BitmapWithoutMemoryPressure)
            };
        }

        private void Test_BitmapWithoutMemoryPressure()
        {
            Console.WriteLine("Test Creating Bitmap with 0 Memory Pressure");

            int width = 1;
            int height = width;
            PixelFormat pe = PixelFormats.Indexed1;
            int stride = (width * pe.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[height * stride];

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Blue);
            colors.Add(System.Windows.Media.Colors.Green);
            BitmapPalette myPalette = new BitmapPalette(colors);

            // Creates a new empty image with the pre-defined palette
            BitmapSource image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Indexed1,
                myPalette,
                pixels,
                stride);
        }

        private void Test_WmpCanonical()
        {
            Console.WriteLine("Test Wmp Canonical");
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\24bppRGBBigEndian.tif", UriKind.RelativeOrAbsolute));
            WmpBitmapEncoder wmpEncoder = new WmpBitmapEncoder();
            wmpEncoder.ImageQualityLevel = 0.7f;
            wmpEncoder.FlipVertical = true;

            Stream imageStreamDest = new System.IO.FileStream("24bppRGBBigEndian.wdp", FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            wmpEncoder.Frames.Add(BitmapFrame.Create(image));
            wmpEncoder.Save(imageStreamDest);
            imageStreamDest.Close();
        }

        private void Test_WmpSpecific()
        {
            Console.WriteLine("Test Wmp Specific");
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\Siren_light_indicator_orange.tif", UriKind.RelativeOrAbsolute));
            WmpBitmapEncoder wmpEncoder = new WmpBitmapEncoder();
            wmpEncoder.UseCodecOptions = true;
            wmpEncoder.QualityLevel = 13;
            wmpEncoder.OverlapLevel = 2;
            wmpEncoder.SubsamplingLevel = 2;
            wmpEncoder.HorizontalTileSlices = 500;
            wmpEncoder.VerticalTileSlices = 500;

            Stream imageStreamDest = new System.IO.FileStream("Siren_light_indicator_orange.wdp", FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            wmpEncoder.Frames.Add(BitmapFrame.Create(image));
            wmpEncoder.Save(imageStreamDest);
            imageStreamDest.Close();
        }

        private void Test_Alpha()
        {
            Console.WriteLine("Test Alpha");
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\Siren_light_indicator_orange.tif", UriKind.RelativeOrAbsolute));
            Debug.Assert(image.Format == PixelFormats.Bgra32, "Expected Pixel format is ARGB");

            OpenContext();
            RenderImage(image);
            CloseContext();
        }

        private void Test_WriteableBitmap()
        {
            Console.WriteLine("Test WriteableBitmap");
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute));

            WriteableBitmap bitmap = new WriteableBitmap(image);

            OpenContext();
            RenderImage(image);
            RenderImage(bitmap);
            CloseContext();

            int width = 10;
            int height = 10;
            int stride = ((width * image.Format.BitsPerPixel) + 7) / 8;
            int arraySize = stride * height;

            // Fill the array with random values.
            byte[] array = new byte[arraySize];
            Random rand = new Random();
            rand.NextBytes(array);

            // Write the array to the bitmap using the old API.
            Int32Rect destRect = new Int32Rect(50, 50, width, height);
            bitmap.WritePixels(destRect, array, stride, 0);

            // Read the array back from the bitmap.
            byte[] chkArray = new byte[arraySize];
            bitmap.CopyPixels(destRect, chkArray, stride, 0);

            for (int i = 0; i < arraySize; i++)
            {
                DRT.Assert(array[i] == chkArray[i], "Back buffer contents do not match what was written using the old API.");
            }

            // Write the array to the bitmap using the new API.
            Int32Rect sourceRect = new Int32Rect(0, 0, width, height);
            bitmap.WritePixels(sourceRect, array, stride, 50, 50);

            // Read the array back from the bitmap.
            bitmap.CopyPixels(destRect, chkArray, stride, 0);

            for (int i = 0; i < arraySize; i++)
            {
                DRT.Assert(array[i] == chkArray[i], "Back buffer contents do not match what was written using the new API.");
            }
        }

        private void Test_InteropBitmap()
        {
            Console.WriteLine("Test InteropBitmap");

            // Simple functional test for a fast pixel format.
            {
                WriteableInteropBitmap fastBitmap = new WriteableInteropBitmap(500,500,PixelFormats.Bgr32);
                fastBitmap.FillQuadrant(0, Colors.Red, true);
                fastBitmap.FillQuadrant(1, Colors.Green, false);
                fastBitmap.FillQuadrant(2, Colors.Blue, true);
                fastBitmap.FillQuadrant(3, Colors.White, false);

                OpenContext();
                RenderImage(fastBitmap.Bitmap);
                CloseContext();

                fastBitmap.Dispose();
                fastBitmap = null;
            }

            // Simple functional test for a slow pixel format.
            {
                WriteableInteropBitmap slowBitmap = new WriteableInteropBitmap(500,500,PixelFormats.Cmyk32);
                slowBitmap.FillQuadrant(0, Colors.Red, true);
                slowBitmap.FillQuadrant(1, Colors.Green, false);
                slowBitmap.FillQuadrant(2, Colors.Blue, true);
                slowBitmap.FillQuadrant(3, Colors.White, false);

                OpenContext();
                RenderImage(slowBitmap.Bitmap);
                CloseContext();

                slowBitmap.Dispose();
                slowBitmap = null;
            }

            // There are additional things to test that are not appropriate
            // for DRTs:
            //
            // Memory usage pattern
            // Using a slow pixel format will result in a large saw-tooth
            // memory usage pattern because we allocate new bitmaps on
            // every update.  The old bitmaps are kept alive until the GC
            // kicks in and finalizes the managed owners. Using a fast pixel
            // format avoids this problem because we just update the content
            // of the existing bitmap.  But this is hard to verify from a
            // DRT because it depends so heavily on the unpredictable
            // timing of garbage collection.  This was actually the source
            // of a regression from 3.5 to 4.0.
            //
            // Correctly updating the screen
            // Writing new content into the bitmap and calling Invalidate
            // should cause the screen to update.  Verifying this would
            // require reading back from the screen, which can be very
            // fragile due to different DPI, resolution, color depth,
            // popups, etc.  This is better suited for our BVT tests which
            // run in a more controlled environment and have lots of render
            // output tests.  Note that this was actually the source of a
            // bug in 4.0 where usage of an image brush where the image
            // was an InteropBitmap would not update correctly due to a
            // bug in the native composition code.
        }

        private void Test_AsyncDownload()
        {
            Console.WriteLine("Test AsyncDownload");
            Stream s = new NonSeekableStream(new FileStream(@"drtfiles\drtimaging\contact.ico", FileMode.Open, FileAccess.Read, FileShare.Read));
            BitmapFrame frame = BitmapFrame.Create(s, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);

            OpenContext();
            RenderImage(frame);
            CloseContext();

            frame.DownloadCompleted += new EventHandler(OnDownloaded);
        }

        private void OnDownloaded(object sender, EventArgs e)
        {
            object o = ((BitmapFrame)sender).Metadata;
            DRT.Assert(((BitmapFrame)sender).Decoder.Frames.Count == 9,
                    String.Format("Expected contact.ico to have 9 frames, {0} found.", ((BitmapFrame)sender).Decoder.Frames.Count));
        }

        private void Test_DecoderTest1()
        {
            Console.WriteLine("Test DecoderTest1");

            BitmapImage image1 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyAI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image2 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyAN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image3 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyB0AlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image4 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyB0AlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image5 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyB0NoAlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image6 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyB0NoAlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image7 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image8 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyLazyAlphaI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image9 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyLazyAlphaN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image10 = new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image11 = new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyW0AlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image12 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image13 = new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyW0AlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image14 = new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyW0NoAlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image15 = new BitmapImage(new Uri("drtfiles\\drtimaging\\16bppGreyW0NoAlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image16 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyB0UncompressedLittleEndian(WhiteOnBlack).tif", UriKind.RelativeOrAbsolute));
            BitmapImage image17 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyG3B0LittleEndian(WhiteOnBlack).tif", UriKind.RelativeOrAbsolute));
            BitmapImage image18 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyG3W0LittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image19 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyHuffmanB0LittleEndian(BkOnWhSpecial).tif", UriKind.RelativeOrAbsolute));
            BitmapImage image20 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyHuffmanW0LittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image21 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image22 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyLazyAlphaI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image23 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyLazyAlphaN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image24 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyLZWB0LittleEndian(WhiteOnBlack).tif", UriKind.RelativeOrAbsolute));
            BitmapImage image25 = new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyLZWW0LittleEndian.tif", UriKind.RelativeOrAbsolute));

            OpenContext();
            RenderImage(image1);
            RenderImage(image2);
            RenderImage(image3);
            RenderImage(image4);
            RenderImage(image5);
            RenderImage(image6);
            RenderImage(image7);
            RenderImage(image8);
            RenderImage(image9);
            RenderImage(image10);
            RenderImage(image11);
            RenderImage(image12);
            RenderImage(image13);
            RenderImage(image14);
            RenderImage(image15);
            RenderImage(image16);
            RenderImage(image17);
            RenderImage(image18);
            RenderImage(image19);
            RenderImage(image20);
            RenderImage(image21);
            RenderImage(image22);
            RenderImage(image23);
            RenderImage(image24);
            RenderImage(image25);
            CloseContext();
        }

        private void Test_DecoderTest2()
        {
            Console.WriteLine("Test DecoderTest2");

            BitmapImage image1 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyPackBitsB0LittleEndian(WhiteOnBlack).tif", UriKind.RelativeOrAbsolute));
            BitmapImage image2 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyPackBitsW0LittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image3 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppGreyW0UncompressedLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image4 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppIndexeAI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image5 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppIndexedAN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image6 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppIndexedI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image7 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\1bppIndexedN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image8 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\24bppRGBBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image9 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\24bppRGBI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image10 = new BitmapImage(new Uri("drtfiles\\drtimaging\\24bppRGBLazyAlphaI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image11 = new BitmapImage(new Uri("drtfiles\\drtimaging\\24bppRGBLazyAlphaN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image12 = new BitmapImage(new Uri("drtfiles\\drtimaging\\24bppRGBLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image13 = new BitmapImage(new Uri("drtfiles\\drtimaging\\24bppRGBN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image14 = new BitmapImage(new Uri("drtfiles\\drtimaging\\2bppGreyLazyAlphaN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image15 = new BitmapImage(new Uri("drtfiles\\drtimaging\\2bppGreyN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image16 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppCMYKNoAlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image17 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppCMYKNoAlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image18 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppGreyAI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image19 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppGreyAN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image20 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppGreyB0AlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image21 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppGreyB0AlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image22 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppGreyW0AlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image23 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppGreyW0AlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image24 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppRGBABigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image25 = new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppRGBAI.png", UriKind.RelativeOrAbsolute));

            OpenContext();
            RenderImage(image1);
            RenderImage(image2);
            RenderImage(image3);
            RenderImage(image4);
            RenderImage(image5);
            RenderImage(image6);
            RenderImage(image7);
            RenderImage(image8);
            RenderImage(image9);
            RenderImage(image10);
            RenderImage(image11);
            RenderImage(image12);
            RenderImage(image13);
            RenderImage(image14);
            RenderImage(image15);
            RenderImage(image16);
            RenderImage(image17);
            RenderImage(image18);
            RenderImage(image19);
            RenderImage(image20);
            RenderImage(image21);
            RenderImage(image22);
            RenderImage(image23);
            RenderImage(image24);
            RenderImage(image25);
            CloseContext();
        }

        private void Test_DecoderTest3()
        {
            Console.WriteLine("Test DecoderTest3");

            BitmapImage image1 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppRGBALittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image2 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\32bppRGBAN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image3 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\40bppCMYKAlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image4 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\40bppCMYKAlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image5 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\48bppRGBBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image6 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\48bppRGBI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image7 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\48bppRGBLazyAlphaI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image8 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\48bppRGBLazyAlphaN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image9 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\48bppRGBLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image10 = new BitmapImage(new Uri("drtfiles\\drtimaging\\48bppRGBN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image11 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppGreyI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image12 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppGreyLazyAlphaI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image13 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppGreyLazyAlphaN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image14 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppGreyN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image15 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppIndexedAI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image16 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppIndexedAN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image17 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppIndexedI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image18 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppIndexedLZWLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image19 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppIndexedN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image20 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppIndexedPackbitsLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image21 = new BitmapImage(new Uri("drtfiles\\drtimaging\\4bppIndexedUncompressedLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image22 = new BitmapImage(new Uri("drtfiles\\drtimaging\\64bppCMYKNoAlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image23 = new BitmapImage(new Uri("drtfiles\\drtimaging\\64bppCMYKNoAlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image24 = new BitmapImage(new Uri("drtfiles\\drtimaging\\64bppRGBABigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image25 = new BitmapImage(new Uri("drtfiles\\drtimaging\\64bppRGBAI.png", UriKind.RelativeOrAbsolute));

            OpenContext();
            RenderImage(image1);
            RenderImage(image2);
            RenderImage(image3);
            RenderImage(image4);
            RenderImage(image5);
            RenderImage(image6);
            RenderImage(image7);
            RenderImage(image8);
            RenderImage(image9);
            RenderImage(image10);
            RenderImage(image11);
            RenderImage(image12);
            RenderImage(image13);
            RenderImage(image14);
            RenderImage(image15);
            RenderImage(image16);
            RenderImage(image17);
            RenderImage(image18);
            RenderImage(image19);
            RenderImage(image20);
            RenderImage(image21);
            RenderImage(image22);
            RenderImage(image23);
            RenderImage(image24);
            RenderImage(image25);
            CloseContext();
        }

        private void Test_DecoderTest4()
        {
            Console.WriteLine("Test DecoderTest4");

            BitmapImage image1 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\64bppRGBALittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image2 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\64bppRGBAN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image3 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\80bppCMYKAlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image4 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\80bppCMYKAlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image5 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppGreyB0NoAlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image6 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppGreyB0NoAlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image7 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppGreyI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image8 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppGreyLazyAlphaI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image9 =  new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppGreyLazyAlphaN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image10 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppGreyN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image11 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppGreyW0NoAlphaBigEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image12 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppGreyW0NoAlphaLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image13 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppIndexedAI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image14 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppIndexedAN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image15 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppIndexedI.png", UriKind.RelativeOrAbsolute));
            BitmapImage image16 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppIndexedLZWLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image17 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppIndexedN.png", UriKind.RelativeOrAbsolute));
            BitmapImage image18 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppIndexedUncompressedLittleEndian.tif", UriKind.RelativeOrAbsolute));
            BitmapImage image19 = new BitmapImage(new Uri("drtfiles\\drtimaging\\8bppPackbitsLZWLittleEndian.tif", UriKind.RelativeOrAbsolute));

            OpenContext();
            RenderImage(image1);
            RenderImage(image2);
            RenderImage(image3);
            RenderImage(image4);
            RenderImage(image5);
            RenderImage(image6);
            RenderImage(image7);
            RenderImage(image8);
            RenderImage(image9);
            RenderImage(image10);
            RenderImage(image11);
            RenderImage(image12);
            RenderImage(image13);
            RenderImage(image14);
            RenderImage(image15);
            RenderImage(image16);
            RenderImage(image17);
            RenderImage(image18);
            RenderImage(image19);
            CloseContext();
        }

        private void Test_StreamRelease()
        {
            Console.WriteLine("Test StreamRelease");

            File.Copy("drtfiles\\drtimaging\\tulip.jpg","foo.jpg", true);
            BitmapFrame image = BitmapFrame.Create(new Uri("foo.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            File.Delete("foo.jpg");

            File.Copy("drtfiles\\drtimaging\\tulip.jpg","bar.jpg", true);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(Directory.GetCurrentDirectory() + @"\bar.jpg", UriKind.RelativeOrAbsolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            File.Delete("bar.jpg");

            OpenContext();
            RenderImage(image);
            RenderImage(bitmap);
            CloseContext();
        }

        private void Test_CroppedBitmap()
        {
            Console.WriteLine("Test CroppedBitmap");
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute));
            CroppedBitmap crop = new CroppedBitmap();

            Int32Rect rect = new Int32Rect(0, 0, 5, 5);

            crop.BeginInit();
            crop.SourceRect = rect;
            crop.Source = image;
            crop.EndInit();

            OpenContext();
            RenderImage(image);
            RenderImage(crop);
            CloseContext();

            BitmapEncoder encoder = new BmpBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(crop));

            Stream imageStreamDest = new System.IO.FileStream("Cropped.bmp", FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            encoder.Save(imageStreamDest);
        }

        private void Test_ExtendedPfBitmap()
        {
            Console.WriteLine("Test ExtendedPfBitmap");
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute));
            FormatConvertedBitmap fmt = new FormatConvertedBitmap(image, PixelFormats.Cmyk32, null, 0.0);

            CachedBitmap cache = new CachedBitmap(fmt, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

            BitmapSource wdpPreservePF = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\6channel_noprof_noalpha.wdp", UriKind.RelativeOrAbsolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

            OpenContext();
            RenderImage(image);
            RenderImage(fmt);
            RenderImage(cache);
            RenderImage(wdpPreservePF);
            CloseContext();

            DRT.Assert( cache.Format == PixelFormats.Cmyk32 );

            BitmapSource cmyk = BitmapSource.Create(10, 10, 96, 96, PixelFormats.Cmyk32, null, new byte[10 * 10 * 4], 40);

            DRT.Assert( cmyk.Format == PixelFormats.Cmyk32 );
        }


        private void Test_CustomLoader()
        {
            Console.WriteLine("Test CustomerLoader");
            BitmapImage custom = new BitmapImage();

            TypeConverter uriTypeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Uri));

            custom.BeginInit();
            custom.DecodePixelHeight = 400;
            custom.DecodePixelWidth = 300;
            custom.Rotation = System.Windows.Media.Imaging.Rotation.Rotate0;
            custom.UriSource = (Uri)uriTypeConverter.ConvertFromInvariantString(null,"drtfiles\\drtimaging\\Fountain.jpg");
            custom.EndInit();

            OpenContext();
            RenderImage(custom);
            CloseContext();

            int width = custom.PixelWidth;
            int height = custom.PixelHeight;

            DRT.Assert(width == 300,
                    String.Format("Expected width == 300. Actual width == {0}", width));
            DRT.Assert(height == 400,
                    String.Format("Expected width == 400. Actual width == {0}", height));

            BitmapEncoder encoder = new BmpBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(custom));

            Stream imageStreamDest = new System.IO.FileStream("TestCustom.bmp", FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            encoder.Save(imageStreamDest);
        }

        // see if we can draw on an image
        private void Test_ImmediateModeRendering ()
        {
            Console.WriteLine("Test ImmediateModeRendering");
            RenderTargetBitmap id = new RenderTargetBitmap(300, 300, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            System.Windows.Media.Visual v = new VectorPage ();

            id.Render (v);

            OpenContext();
            RenderImage(id);
            RenderImage(id.Clone());
            CloseContext();

            BitmapEncoder encoder = new BmpBitmapEncoder();

            encoder.Frames.Add (BitmapFrame.Create(id));

            Stream imageStreamDest = new System.IO.FileStream ("Fountain.bmp", FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            encoder.Save (imageStreamDest);
        }

        private void Test_SaveTifAsGif ()
        {
            Console.WriteLine("Test SaveTifAsGif");
            Stream imageStreamSource = new System.IO.FileStream ("drtfiles\\drtimaging\\a-b-2.tif", FileMode.Open, FileAccess.Read, FileShare.Read);
            Stream imageStreamDest = new System.IO.FileStream ("a-b-2.gif", FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            BitmapDecoder decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.None, BitmapCacheOption.Default);
            GifBitmapEncoder encoder = new GifBitmapEncoder();

            String tiffMimeType = decoder.CodecInfo.MimeTypes;
            String gifMimeType = encoder.CodecInfo.MimeTypes;
            String tiffExtensions = decoder.CodecInfo.FileExtensions;

            DRT.Assert(tiffMimeType =="image/tiff,image/tif","Unexpected TIFF mime type.");
            DRT.Assert(gifMimeType =="image/gif","Unexpected GIF mime type.");
            DRT.Assert(tiffExtensions ==".tiff,.tif","Unexpected TIFF extensions");

            encoder.Frames = decoder.Frames;

            encoder.Save(imageStreamDest);

            BitmapFrame frame = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\40bppCMYKAlphaBigEndian.tif", UriKind.RelativeOrAbsolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            GifBitmapEncoder gifEncoder = new GifBitmapEncoder();
            gifEncoder.Frames.Add(frame);

            Stream imageStreamDest2 = new System.IO.FileStream("output.gif", FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            gifEncoder.Save(imageStreamDest2);


        }


        private void Test_SaveTifAsWmp()
        {
            Console.WriteLine("Test SaveTifAsWmp");
            Stream imageStreamSource = new System.IO.FileStream("drtfiles\\drtimaging\\a-b-3.tif", FileMode.Open, FileAccess.Read, FileShare.Read);
            Stream imageStreamDest = new System.IO.FileStream("a-b-3.wdp", FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            BitmapDecoder decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.None, BitmapCacheOption.Default);
            WmpBitmapEncoder encoder = new WmpBitmapEncoder();

            String tiffMimeType = decoder.CodecInfo.MimeTypes;
            String wmpMimeType = encoder.CodecInfo.MimeTypes;

            DRT.Assert(tiffMimeType =="image/tiff,image/tif","Unexpected TIFF mime type.");

            // Remove"image/wmphoto, image/wdp" once final codec is checked in
            DRT.Assert(wmpMimeType =="image/wmphoto" || wmpMimeType =="image/vnd.ms-photo" || wmpMimeType =="image/wdp","Unexpected WMP mime type.");

            encoder.Frames = decoder.Frames;
            encoder.Save(imageStreamDest);

            imageStreamDest.Close();
        }


        private void Test_SaveWmpAsTif()
        {
            Console.WriteLine("Test SaveWmpAsTif");
            Stream imageStreamSource = new System.IO.FileStream("a-b-3.wdp", FileMode.Open, FileAccess.Read, FileShare.Read);
            Stream imageStreamDest = new System.IO.FileStream("a-b-3.tif", FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            BitmapDecoder decoder = new WmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.None, BitmapCacheOption.Default);
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();

            String wmpMimeType = decoder.CodecInfo.MimeTypes;
            String tiffMimeType = encoder.CodecInfo.MimeTypes;

            // Remove"image/wmphoto, image/wdp" once final codec is checked in
            DRT.Assert(wmpMimeType =="image/wmphoto" || wmpMimeType =="image/vnd.ms-photo" || wmpMimeType =="image/wdp","Unexpected WMP mime type.");
            DRT.Assert(tiffMimeType =="image/tiff,image/tif","Unexpected TIFF mime type.");

            encoder.Frames = decoder.Frames;
            encoder.Save(imageStreamDest);
        }


        private void Test_LoadImageWithTypeConverter()
        {
            Console.WriteLine("Test LoadImageWithTypeConverter");
            TypeConverter bitmapImageTypeConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(BitmapImage));
            BitmapSource bitmapImage = (BitmapSource)bitmapImageTypeConverter.ConvertFromInvariantString (null,"drtfiles\\drtimaging\\Fountain.jpg");

            int width = bitmapImage.PixelWidth;
            int height = bitmapImage.PixelHeight;

            DRT.Assert(width == 2560,
                    String.Format("Actual width = {0}. Expected width = 2560.", width));
            DRT.Assert(height == 1920,
                    String.Format("Actual height = {0}. Expected height = 1920.", width));

            double dpiX = bitmapImage.DpiX;
            double dpiY = bitmapImage.DpiY;

            string header ="Calling bitmapImageTypeConverter.ConvertFromInvarientString(null, \"Fountain.jpg\").";

            /*
            ** The below two asserts will fail in a normal setup and should pass in test environment.
            ** In normal setup, dpiX and dpiY unit will be 72.
            */
            DRT.Assert(dpiX == 96,
                    String.Format(header +"Actual dpiX = {0}. Expected dpiX = 96.", dpiX));
            DRT.Assert(dpiY == 96,
                    String.Format(header +"Actual dpiY = {0}. Expected dpiY = 96.", dpiY));

            System.Windows.Media.PixelFormat format = bitmapImage.Format;

            OpenContext();
            RenderImage(bitmapImage);
            CloseContext();
        }

        private void Test_UseEmbeddedColorContext()
        {
            Console.WriteLine("Test UseEmbeddedColorContext");

            string jpegFile = "drtfiles\\drtimaging\\ColorProfileJPg.jpg";

            Stream imageStream = new FileStream(jpegFile, FileMode.Open, FileAccess.Read, FileShare.Read);

            BitmapFrame bsrcIgnoreColor = BitmapFrame.Create(new Uri(jpegFile, UriKind.RelativeOrAbsolute), BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
            DRT.Assert(bsrcIgnoreColor.ColorContexts.Count == 2);

            foreach (ColorContext cc in bsrcIgnoreColor.ColorContexts)
            {
                using (Stream s = cc.OpenProfileStream())
                {
                    // All we're hoping for is that OpenProfileStream returns a stream without crashing
                    DRT.Assert(s != null);
                }
            }

            BitmapSource bsrc = BitmapFrame.Create(imageStream);
            BitmapFrame bsrcFrame = (BitmapFrame)bsrc;
            ColorContext sourceColorContext = bsrcFrame.ColorContexts[0];

            ColorContext destColorContext = new ColorContext(System.Windows.Media.PixelFormats.Bgra32);

            DRT.Assert((destColorContext != null) || (sourceColorContext != null),
                String.Format("ColorContext is null"));

            ColorConvertedBitmap ccb = new ColorConvertedBitmap(bsrc, sourceColorContext, destColorContext, PixelFormats.Pbgra32);

            jpegFile = "drtfiles\\drtimaging\\Olympus.jpg";

            Stream imageStreamNoColor = new FileStream(jpegFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapFrame bsrcNoColor = BitmapFrame.Create(imageStreamNoColor);
            // WIC v1 returns 1 and WIC v2 returns 2
            DRT.Assert(bsrcNoColor.ColorContexts.Count == 1 || bsrcNoColor.ColorContexts.Count == 2);
            foreach (ColorContext cc in bsrcNoColor.ColorContexts)
            {
                using (Stream s = cc.OpenProfileStream())
                {
                    // All we're hoping for is that OpenProfileStream returns a stream without crashing
                    DRT.Assert(s != null);
                }
            }

            ReadOnlyCollection<ColorContext> colorcontexts = new ReadOnlyCollection<ColorContext>(bsrcFrame.ColorContexts);

            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute));

            // image is being used for both the main frame and the thumbnail
            //BitmapFrame frame = BitmapFrame.Create(image, image, null, colorcontexts);
            BitmapFrame frame = BitmapFrame.Create(image, image, null, colorcontexts);

            // Photon cases with embedded profiles
            BitmapSource wdpIgnoreColor1 = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\RGB24bppProfile.wdp", UriKind.RelativeOrAbsolute), BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
            BitmapSource wdpIgnoreColor2 = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\RGB48bppProfile.wdp", UriKind.RelativeOrAbsolute), BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
            BitmapSource wdpUseColor1 = BitmapFrame.Create(new FileStream("drtfiles\\drtimaging\\RGB24bppProfile.wdp", FileMode.Open, FileAccess.Read, FileShare.Read));
            BitmapSource wdpUseColor2 = BitmapFrame.Create(new FileStream("drtfiles\\drtimaging\\RGB48bppProfile.wdp", FileMode.Open, FileAccess.Read, FileShare.Read));

            byte[] pixels = new byte[80];

            wdpIgnoreColor1.CopyPixels(new Int32Rect(0,0,10,1), pixels, 80, 0);
            DRT.Assert(pixels[0] >= 0xFD && pixels[1] >= 0xFD && pixels[2] >= 0xFD,
                    String.Format("Background of this bitmap should be white."));
            wdpIgnoreColor2.CopyPixels(new Int32Rect(0, 0, 10, 1), pixels, 80, 0);
            DRT.Assert(pixels[0] >= 0xFD && pixels[1] >= 0xFD && pixels[2] >= 0xFD,
                    String.Format("Background of this bitmap should be white."));
            wdpUseColor1.CopyPixels(new Int32Rect(0, 0, 10, 1), pixels, 80, 0);
            DRT.Assert(!(pixels[0] >= 0xFD && pixels[1] >= 0xFD && pixels[2] >= 0xFD),
                    String.Format("Background of this bitmap should NOT be white."));
            wdpUseColor2.CopyPixels(new Int32Rect(0, 0, 10, 1), pixels, 80, 0);
            DRT.Assert(!(pixels[0] >= 0xFD && pixels[1] >= 0xFD && pixels[2] >= 0xFD),
                    String.Format("Background of this bitmap should NOT be white."));

            OpenContext();
            RenderImage(bsrcIgnoreColor);
            RenderImage(bsrc);
            RenderImage(bsrcFrame);
            RenderImage(ccb);
            RenderImage(image);
            RenderImage(wdpIgnoreColor1);
            RenderImage(wdpIgnoreColor2);
            RenderImage(wdpUseColor1);
            RenderImage(wdpUseColor2);
            CloseContext();

            BitmapEncoder encoder = new TiffBitmapEncoder();

            encoder.Frames.Add(frame);

            Stream imageStreamDest = new System.IO.FileStream("colorcontexts.tiff", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

            encoder.Save(imageStreamDest);



        }

        private void Test_Palette()
        {
            Console.WriteLine("Test Palette");

            // Try reading a palette from an image

            Stream imageStreamSource = new System.IO.FileStream("drtfiles\\drtimaging\\Cars256.bmp", FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapDecoder decoder = new BmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];
            BitmapPalette palette = bitmapSource.Palette;

            OpenContext();
            RenderImage(bitmapSource);

            DRT.Assert(palette.Colors.Count == 256,
                    String.Format("Expected Cars256.bmp to have a 256 color palette, {0} colors found", palette.Colors.Count));

            IList<System.Windows.Media.Color> palColor = palette.Colors;

            DRT.Assert(palColor != null,
                    String.Format("Call to palette.Colors returned null."));
            DRT.Assert(palColor.Count == 256,
                    String.Format("Call to palette.Colors produced array of {0} colors, 256 expected.", palColor.Count));


            // Try creating a new image with a custom palette.

            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Blue);

            int i;

            palette = new BitmapPalette(colors);

            int width = 128;
            int height = width;
            int stride = width/8;

            byte[] pixels = new byte[height*stride];

            for (i = 0; i < height*stride; ++i)
            {
                if (i < height*stride/2)
                {
                    pixels[i] = 0x00;
                }
                else
                {
                    pixels[i] = 0xff;
                }
            }

            BitmapSource image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                System.Windows.Media.PixelFormats.Indexed1,
                palette,
                pixels,
                stride);

            RenderImage(image);

            FileStream stream = new FileStream("newPalette.tiff", FileMode.Create);

            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);

            stream.Close();

            // Try creating an image with a predefined palette.

            palette = BitmapPalettes.WebPalette;

            stride = width;

            pixels = new byte[height*stride];
            for (i = 0; i < height*stride; ++i)
            {
                if (i < height*stride/2)
                {
                    pixels[i] = 0x00;
                }
                else
                {
                    pixels[i] = 0x55;
                }
            }

            image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                System.Windows.Media.PixelFormats.Indexed8,
                palette,
                pixels,
                stride);

            RenderImage(image);

            stream = new FileStream("newPalette2.tiff", FileMode.Create);

            encoder = new TiffBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);

            stream.Close();


            // Try creating a palette from a non-palettized image.

            BitmapImage image2 = new BitmapImage();
            image2.BeginInit();
            image2.UriSource= new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute);
            image2.EndInit();

            RenderImage(image2);

            palette = new BitmapPalette(image2, 111);

            DRT.Assert(palette.Colors.Count == 111);
            CloseContext();

        }

        private void Test_Icons()
        {
            Console.WriteLine("Test Icons");
            Stream imageStreamSource = new FileStream(@"drtfiles\drtimaging\contact.ico", FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapDecoder decoder = new IconBitmapDecoder(imageStreamSource, BitmapCreateOptions.None, BitmapCacheOption.Default);

            DRT.Assert(decoder != null,
"Could not load image \"contact.ico\"");
            DRT.Assert(decoder.Frames.Count == 9,
                    String.Format("Expected contact.ico to have 9 frames, {0} found.", decoder.Frames.Count));

            OpenContext();
            for (int i = 0; i < decoder.Frames.Count; i++)
            {
                BitmapSource src = decoder.Frames[i];
                RenderImage(src);

                BitmapSource thumb = decoder.Frames[i].Thumbnail;
                DRT.Assert(thumb != null,"Expected icon frame to have thumbnail");
                PixelFormat fmt = thumb.Format;
                RenderImage(thumb);
            }
            CloseContext();
        }

         private void Test_Sections()
         {
             Console.WriteLine("Test Sections");
             uint width = 0xd0;
             uint height = 0xcf;
             uint size = width*height*3;

             string filename = "DrtFiles\\DrtImaging\\earth.bmp";

             IntPtr hFile = CreateFile(filename, 0x80000000, 1, IntPtr.Zero, 3, 0, IntPtr.Zero);
             if (hFile == new IntPtr(-1))
             {
                 int lastError = Marshal.GetLastWin32Error();
                 Console.WriteLine("   *** WARNING: Failed to get handle for {0}, error code {1}.  Skipping Sections test", filename, lastError);
                 return;
             }

             IntPtr hSection = CreateFileMapping(hFile, IntPtr.Zero, 0x2, 0, 0, IntPtr.Zero);
             if (hSection == IntPtr.Zero)
             {
                 int lastError = Marshal.GetLastWin32Error();
                 Console.WriteLine("   *** WARNING: Failed to get handle for memory section.  Skipping Sections test", filename, lastError);
                 CloseHandle(hFile);
                 return;
             }

             BitmapSource image = System.Windows.Interop.Imaging.CreateBitmapSourceFromMemorySection(
                 hSection,
                 (int)width,
                 (int)height,
                 System.Windows.Media.PixelFormats.Bgr24,
                 (int)width*3,
                 0x36);

             CloseHandle(hSection);
             CloseHandle(hFile);

             OpenContext();
             RenderImage(image);
             CloseContext();
         }

        private void Test_Rotations()
        {
            Console.WriteLine("Test Rotations");
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute));

            Transform [] transforms =
                new Transform[] {
                    new RotateTransform(0.0),
                    new RotateTransform(90.0),
                    new RotateTransform(180.0),
                    new RotateTransform(270.0)
                };

            string [] names = new string[] {
"rotate0",
"rotate1",
"rotate2",
"rotate3"
            };

            OpenContext();
            RenderImage(image);
            for (int i = 0; i < 4; ++i)
            {
                TransformedBitmap bitmap = new TransformedBitmap();
                TransformGroup trans = new TransformGroup();
                trans.Children.Add(new ScaleTransform(2.0, 1.0));
                trans.Children.Add(transforms[i]);

                bitmap.BeginInit();
                bitmap.Transform = trans;
                bitmap.Source = image;
                bitmap.EndInit();

                RenderImage(bitmap);
                BitmapEncoder encoder = new BmpBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                Stream imageStreamDest = new System.IO.FileStream(names[i] +".bmp", FileMode.Create, FileAccess.ReadWrite, FileShare.None);

                encoder.Save(imageStreamDest);
            }
            CloseContext();
        }

        private void Test_Encoders()
        {
            Console.WriteLine("Test Encoders");
            BitmapFrame frame = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute));

            BitmapEncoder [] encoders = new BitmapEncoder[] {
                   new BmpBitmapEncoder(),
                   new GifBitmapEncoder(),
                   new JpegBitmapEncoder(),
                   new PngBitmapEncoder(),
                   new TiffBitmapEncoder()
            };

            string [] names = new string[] {
"encode.bmp",
"encode.gif",
"encode.jpg",
"encode.png",
"encode.tif"
            };

            for (int i = 0; i < 5; ++i)
            {
                BitmapEncoder encoder = encoders[i];

                encoder.Frames.Add(frame);

                Stream imageStreamDest = new System.IO.FileStream(names[i], FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

                encoder.Save(imageStreamDest);

                imageStreamDest.Close();
            }

            OpenContext();
            for (int i = 0; i < 5; ++i)
            {
                BitmapImage image = new BitmapImage(new Uri(names[i], UriKind.RelativeOrAbsolute));
                RenderImage(image);
            }
            CloseContext();
        }

        private void Test_DelayCreation()
        {
            Console.WriteLine("Test DelayCreation");
            //
            // Test 1: Ensure that DelayCreated bitmaps still work like regular bitmaps
            //
            BitmapFrame frame = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.DelayCreation,
                BitmapCacheOption.Default
                );

            DRT.Assert(frame.PixelWidth == 100);

            BitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(frame);

            Stream imageStreamDest = new System.IO.FileStream("delay.png", FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            encoder.Save(imageStreamDest);

            imageStreamDest.Close();

            OpenContext();
            RenderImage(frame);
            CloseContext();
        }



        private void Test_TiffThumbnail()
        {
            Console.WriteLine("Test TiffThumbnail");
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\a-b-3.tif", UriKind.RelativeOrAbsolute));

            // image is being used for both the main frame and the thumbnail
            BitmapFrame frame = BitmapFrame.Create(image, image, null, null);

            OpenContext();
            RenderImage(image);
            RenderImage(frame);

            BitmapEncoder encoder = new TiffBitmapEncoder();

            encoder.Frames.Add(frame);

            Stream imageStreamDest = new System.IO.FileStream("thumbnail.tiff", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

            encoder.Save(imageStreamDest);

            imageStreamDest.Close();

            BitmapFrame frame2 = BitmapFrame.Create(new Uri("thumbnail.tiff", UriKind.RelativeOrAbsolute));

            BitmapSource thumbnail = frame2.Thumbnail;

            RenderImage(frame2);
            RenderImage(thumbnail);

            DRT.Assert(thumbnail != null);

            CloseContext();
        }

        private void Test_RegressionTifNewSubFileType()
        {
            Console.WriteLine("Test RegressionTifNewSubFileType");

            // this is the test case to handle Tif NewSubFileType
            // add this test case will ensure the Tiff handle IFD thumbnail and primary frame well.
            // recommened by Thomas Olson
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\offinmay.tif", UriKind.RelativeOrAbsolute));

            BitmapFrame frame = BitmapFrame.Create(image, null, null, null);

            OpenContext();
            RenderImage(image);
            RenderImage(frame);

            BitmapEncoder encoder = new TiffBitmapEncoder();

            encoder.Frames.Add(frame);

            Stream imageStreamDest = new System.IO.FileStream("offinmay.tiff", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

            encoder.Save(imageStreamDest);

            imageStreamDest.Close();

            CloseContext();
        }

        private void Test_64bppBMP()
        {
            Console.WriteLine("Test BMP64");

            OpenContext();

            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\ff_semitransparent.bmp", UriKind.RelativeOrAbsolute));
            RenderImage(image);

            BitmapEncoder encoder = new BmpBitmapEncoder();
            BitmapFrame frame = BitmapFrame.Create(image, image, null, null);
            encoder.Frames.Add(frame);

            Stream imageStreamDest = new System.IO.FileStream("foo99.bmp", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            encoder.Save(imageStreamDest);

            imageStreamDest.Close();

            image = new BitmapImage(new Uri("foo99.bmp", UriKind.RelativeOrAbsolute));
            RenderImage(image);

            CloseContext();
        }

        private void Test_Thumbnail()
        {
            Console.WriteLine("Test Thumbnail");
            BitmapImage image = new BitmapImage(new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute));

            // image is being used for both the main frame and the thumbnail
            BitmapFrame frame = BitmapFrame.Create(image, image, null, null);

            OpenContext();
            RenderImage(image);
            RenderImage(frame);

            BitmapEncoder encoder = new JpegBitmapEncoder();

            encoder.Frames.Add(frame);

            Stream imageStreamDest = new System.IO.FileStream("thumbnail.jpg", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

            encoder.Save(imageStreamDest);

            imageStreamDest.Close();

            BitmapFrame frame2 = BitmapFrame.Create(new Uri("thumbnail.jpg", UriKind.RelativeOrAbsolute));

            BitmapSource thumbnail = frame2.Thumbnail;

            RenderImage(frame2);
            RenderImage(thumbnail);

            DRT.Assert(thumbnail != null);

            BitmapFrame frame3 = BitmapFrame.Create(
                new Uri("drtfiles\\drtimaging\\a-b-2.tif", UriKind.RelativeOrAbsolute),
                System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat,
                System.Windows.Media.Imaging.BitmapCacheOption.Default
                );

            BitmapSource thumbnail2 = frame3.Thumbnail;

            DRT.Assert(thumbnail2 == null);

            RenderImage(frame3);
            RenderImage(thumbnail2);

            BitmapFrame frame4 = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\Protest_B_8.jpg", UriKind.RelativeOrAbsolute));

            BitmapSource thumbnail3 = frame4.Thumbnail;

            RenderImage(frame4);
            RenderImage(thumbnail3);

            DRT.Assert(thumbnail3 != null);

            CloseContext();
        }

        private void Test_CustomBitmapSource()
        {
            Console.WriteLine("Test CustomBitmapSource");
            // Immediate mode rendering...
            MyBitmapSource bitmapSource = new MyBitmapSource(100, 200);
            Stream imageDest = new System.IO.FileStream("custom.jpg", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            Stream imageWithBVMDest = new System.IO.FileStream("custom_withBVM.jpg", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

            OpenContext();
            RenderImage(bitmapSource);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            encoder.Save(imageDest);

            // Retained mode rendering...
            RenderTargetBitmap id = new RenderTargetBitmap(100, 200, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            ImageVisual image = new ImageVisual(bitmapSource);

            id.Render (image);

            RenderImage(id);
            RenderImage(id.Clone());
            CloseContext();

            BitmapEncoder encoder2 = new JpegBitmapEncoder();

            encoder2.Frames.Add(BitmapFrame.Create(id));

            encoder2.Save (imageWithBVMDest);
        }

        private void Test_PixelFormats()
        {
            Console.WriteLine("Test PixelFormats");
            PixelFormat[] pixelFormatList = new PixelFormat[]
            {
                PixelFormats.Indexed2,
                PixelFormats.Indexed4,
                PixelFormats.Indexed8,
                PixelFormats.BlackWhite,
                PixelFormats.Gray2,
                PixelFormats.Gray4,
                PixelFormats.Gray8,
                PixelFormats.Bgr555,
                PixelFormats.Bgr565,
                PixelFormats.Bgr101010,
                PixelFormats.Bgr24,
                PixelFormats.Rgb24,
                PixelFormats.Bgr32,
                PixelFormats.Bgra32,
                PixelFormats.Pbgra32,
                PixelFormats.Rgb48,
                PixelFormats.Rgba64,
                PixelFormats.Prgba64,
                PixelFormats.Gray16,
                PixelFormats.Gray32Float,
                PixelFormats.Rgb128Float,
                PixelFormats.Rgba128Float,
                PixelFormats.Prgba128Float,
                PixelFormats.Cmyk32
            };

            int[] pixelFormatInfo = new int[]
            {
                2, 1, /* PixelFormats.Indexed2 */
                4, 1, /* PixelFormats.Indexed4 */
                8, 1, /* PixelFormats.Indexed8 */
                1, 1, /* PixelFormats.BlackWhite */
                2, 1, /* PixelFormats.Gray2 */
                4, 1, /* PixelFormats.Gray4 */
                8, 1, /* PixelFormats.Gray8 */
                16, 3, /* PixelFormats.Bgr555 */
                16, 3, /* PixelFormats.Bgr565 */
                32, 3, /* PixelFormats.Bgr101010 */
                24, 3, /* PixelFormats.Bgr24 */
                24, 3, /* PixelFormats.Rgb24 */
                32, 3, /* PixelFormats.Bgr32 */
                32, 4, /* PixelFormats.Bgra32 */
                32, 4, /* PixelFormats.Pbgra32 */
                48, 3, /* PixelFormats.Rgb48 */
                64, 4, /* PixelFormats.Rgba64 */
                64, 4, /* PixelFormats.Prgba64 */
                16, 1, /* PixelFormats.Gray16 */
                32, 1, /* PixelFormats.Gray32Float */
                128, 3, /* PixelFormats.Rgb128Float */
                128, 4, /* PixelFormats.Rgba128Float */
                128, 4, /* PixelFormats.Prgba128Float */
                32, 4 /* PixelFormats.Cmyk32 */
            };
            DRT.Assert(pixelFormatList.Length*2 == pixelFormatInfo.Length);

            BitmapFrame frame = BitmapFrame.Create(
                new Uri("drtfiles\\drtimaging\\tulip.jpg", UriKind.RelativeOrAbsolute),
                System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat,
                System.Windows.Media.Imaging.BitmapCacheOption.Default
                );

            OpenContext();
            for (int i=0; i<pixelFormatList.Length; i++)
            {
                int bpp = pixelFormatList[i].BitsPerPixel;
                DRT.Assert(pixelFormatInfo[2*i] == bpp);

                IList<PixelFormatChannelMask> masks = pixelFormatList[i].Masks;
                DRT.Assert(pixelFormatInfo[2*i+1] == masks.Count);

                if (pixelFormatList[i].ToString().IndexOf("Indexed") == -1)
                {
                    FormatConvertedBitmap fcb = new FormatConvertedBitmap(frame, pixelFormatList[i], null, 0.0);
                    RenderImage(fcb);
                }
            }
            CloseContext();
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private void Test_Metadata_Enumerate_All(BitmapMetadata metadata, bool fDoSet)
        {
            String format = metadata.Format;
            String location = metadata.Location;

            foreach(String query in metadata)
            {
                DRT.Assert(metadata.ContainsQuery(query));

                object objVal = metadata.GetQuery(query);
                if (objVal is BitmapMetadata)
                {
                    Test_Metadata_Enumerate_All(objVal as BitmapMetadata, fDoSet);
                }

                if (fDoSet)
                {
                    metadata.SetQuery(query, objVal);
                }
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private void Test_BitmapMetadata_Equal(BitmapMetadata metadata1, BitmapMetadata metadata2)
        {
            DRT.Assert(metadata1.Format == metadata2.Format);

            IEnumerator<String> enumerator =
                ((IEnumerable<String>)metadata2).GetEnumerator();

            foreach (string q1 in metadata1)
            {
                enumerator.MoveNext();
                string q2 = enumerator.Current;
                DRT.Assert(q1 == q2);

                object o1 = metadata1.GetQuery(q1);
                object o2 = metadata2.GetQuery(q2);
                DRT.Assert(o1.GetType() == o2.GetType());
            }
        }

        private void Test_MetadataPolicyComponent()
        {
            Console.WriteLine("Test MetadataPolicyComponent");

            Stream jpgStream = new System.IO.FileStream("drtfiles\\drtimaging\\tulip.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            JpegBitmapDecoder jpgDecoder = new JpegBitmapDecoder(jpgStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame jpgFrame = jpgDecoder.Frames[0];
            BitmapMetadata jpgMetadata = jpgFrame.Metadata as BitmapMetadata;

            Stream output_jpgStreamNew = new System.IO.FileStream("output_tulip.jpg", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            output_jpgStreamNew.SetLength(0);
            JpegBitmapEncoder jpgEncoderNew = new JpegBitmapEncoder();
            BitmapMetadata jpgMetadataNew = new BitmapMetadata("jpg");

            string[] strAuthors = {"abc","def","ghi" };
            string[] strKeywords = {"NoBugToday","NoBugTomorrow","NoBugAfterShip" };

            String strDateTime = DateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo);

            ReadOnlyCollection<String> collectionAuthors = (new ReadOnlyCollection<String>(strAuthors));
            ReadOnlyCollection<String> collectionKeywords = (new ReadOnlyCollection<String>(strKeywords));
            String[] strResult = new String[collectionAuthors.Count];
            collectionAuthors.CopyTo(strResult, 0);

            jpgMetadataNew.Author = collectionAuthors;

            ReadOnlyCollection<String> resultcollectionAuthors = jpgMetadataNew.Author;

            jpgMetadataNew.Keywords = collectionKeywords;

            jpgMetadataNew.DateTaken = strDateTime;

            String strDate = jpgMetadataNew.DateTaken;

            ReadOnlyCollection<String> resultKeywords = jpgMetadataNew.Keywords;

            jpgEncoderNew.Frames.Add(BitmapFrame.Create(jpgDecoder.Frames[0], null, jpgMetadataNew, null));
            jpgEncoderNew.Save(output_jpgStreamNew);
            output_jpgStreamNew.Flush();
            output_jpgStreamNew.Close();
        }

        private void Test_Metadata()
        {

            Console.WriteLine("Test Metadata");
            // ------------------------- Test get metadata from GPS, TIFF, JPG, PNG.
            Stream jpgStreamgps = new System.IO.FileStream("drtfiles\\drtimaging\\gps.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            JpegBitmapDecoder jpgDecodergps = new JpegBitmapDecoder(jpgStreamgps, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame jpgFramegps = jpgDecodergps.Frames[0];
            BitmapMetadata jpgMetadatagps = jpgFramegps.Metadata as BitmapMetadata;


            DRT.Assert(jpgMetadatagps.ContainsQuery("/app1/ifd/GPS/{uint=0}"));
            DRT.Assert(jpgMetadatagps.ContainsQuery("/app1/ifd/{uint=34853}/{uint=0}"));
            Test_Metadata_Enumerate_All(jpgMetadatagps, false);

            Stream pngStream = new System.IO.FileStream("drtfiles\\drtimaging\\avalon.png", FileMode.Open, FileAccess.Read, FileShare.Read);
            PngBitmapDecoder pngDecoder = new PngBitmapDecoder(pngStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame pngFrame = pngDecoder.Frames[0];
            BitmapMetadata pngMetadata = pngFrame.Metadata as BitmapMetadata;
            String str = pngMetadata.GetQuery("/tEXt/Software") as String;
            DRT.Assert((str as String) =="Microsoft Office","Unexpected metadata in avalon.png");
            DRT.Assert(!pngMetadata.ContainsQuery("/junk"));
            Test_Metadata_Enumerate_All(pngMetadata, false);
            DRT.Assert(pngMetadata.IsFrozen);
            DRT.Assert(((BitmapMetadata)((BitmapMetadata)pngMetadata.GetQuery("/tEXt"))).IsFrozen);

            Stream jpgStream = new System.IO.FileStream("drtfiles\\drtimaging\\tulip.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            JpegBitmapDecoder jpgDecoder = new JpegBitmapDecoder(jpgStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame jpgFrame = jpgDecoder.Frames[0];
            BitmapMetadata jpgMetadata = jpgFrame.Metadata as BitmapMetadata;
            DRT.Assert((byte)jpgMetadata.GetQuery("/app0/{uint=5}") == 0);
            DRT.Assert(jpgMetadata.ContainsQuery("/app0"));
            Test_Metadata_Enumerate_All(jpgMetadata, false);

            Stream tiffStream = new System.IO.FileStream("drtfiles\\drtimaging\\a-b-2.tif", FileMode.Open, FileAccess.Read, FileShare.Read);
            TiffBitmapDecoder tiffDecoder = new TiffBitmapDecoder(tiffStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame tiffFrame = tiffDecoder.Frames[0];
            BitmapMetadata tiffMetadata = tiffFrame.Metadata as BitmapMetadata;
            DRT.Assert((ushort)tiffMetadata.GetQuery("/ifd/{uint=284}") == 1);
            Test_Metadata_Enumerate_All(tiffMetadata, false);

            Stream tiffStreamIptc = new System.IO.FileStream("drtfiles\\drtimaging\\ambrosia.tif", FileMode.Open, FileAccess.Read, FileShare.Read);
            TiffBitmapDecoder tiffDecoderIPTC = new TiffBitmapDecoder(tiffStreamIptc, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame tiffFrameIPTC = tiffDecoderIPTC.Frames[0];
            BitmapMetadata tiffMetadataIPTC = tiffFrameIPTC.Metadata as BitmapMetadata;
            object kw = tiffMetadataIPTC.GetQuery("/ifd/iptc/Keywords");
            DRT.Assert(kw != null && kw is Array);
            //Test_Metadata_Enumerate_All(tiffMetadata, false);

            // ------------------------- Test encoding image w/metadata for TIFF, JPG, PNG.
            Stream output_pngStream = new System.IO.FileStream("output_avalon.png", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            output_pngStream.SetLength(0);
            pngEncoder.Frames.Add(pngDecoder.Frames[0]);
            pngEncoder.Save(output_pngStream);
            output_pngStream.Flush();
            output_pngStream.Close();

            Stream output_jpgStream = new System.IO.FileStream("output_tulip.jpg", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            output_jpgStream.SetLength(0);
            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgMetadata = new BitmapMetadata("jpg");
            jpgMetadata.SetQuery("/app1/ifd/{uint=1000}", 12345);
            jpgMetadata.SetQuery("/app1/ifd/{uint=1001}", 23456);
            jpgMetadata.SetQuery("/app1/ifd/{uint=1002}", 34567);
            jpgMetadata.SetQuery("/app1/ifd/PaddingSchema:padding", (UInt32)4096);
            jpgEncoder.Frames.Add(BitmapFrame.Create(jpgDecoder.Frames[0], null, jpgMetadata, null));
            jpgEncoder.Save(output_jpgStream);
            output_jpgStream.Flush();
            output_jpgStream.Close();

            Stream output_tiffStream = new System.IO.FileStream("output_a-b-2.tif", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            TiffBitmapEncoder tiffEncoder = new TiffBitmapEncoder();
            output_tiffStream.SetLength(0);
            tiffMetadata = new BitmapMetadata("tiff");
            tiffMetadata.SetQuery("/ifd/{ushort=1000}", 9999);
            tiffMetadata.SetQuery("/ifd/{uint=1001}", 23456);
            tiffMetadata.SetQuery("/ifd/{uint=1002}", 34567);
            tiffMetadata.SetQuery("/ifd/PaddingSchema:padding", (UInt32)4096);
            tiffMetadata.SetQuery("/ifd/exif", new BitmapMetadata("exif"));
            tiffMetadata.SetQuery("/ifd/exif/PaddingSchema:padding", (UInt32)4096);
            tiffEncoder.Frames.Add(BitmapFrame.Create(tiffDecoder.Frames[0], null, tiffMetadata, null));
            tiffEncoder.Save(output_tiffStream);
            output_tiffStream.Flush();
            output_tiffStream.Close();

            output_pngStream = null;
            pngEncoder = null;
            pngMetadata = null;

            output_jpgStream = null;
            jpgEncoder = null;
            jpgMetadata = null;

            output_tiffStream = null;
            tiffEncoder = null;
            tiffMetadata = null;

            GC.Collect();
        }

        private void Test_Metadata_Inplace()
        {
            Console.WriteLine("Test Metadata_Inplace");
            bool succeeded = false;

            // Verify opening as URI, which is Read Only, does not allow in-place editing.
            succeeded = false;
            try
            {
                BitmapFrame frame = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\avalon.png", UriKind.RelativeOrAbsolute));

                InPlaceBitmapMetadataWriter inplaceWriter = frame.CreateInPlaceBitmapMetadataWriter();
            }
            catch (InvalidOperationException)
            {
                // This exception is expected.
                succeeded = true;
            }
            DRT.Assert(succeeded);

            // Verify opening as stream which is read-only, does not allow in-place editing.
            succeeded = false;
            try
            {
                Stream stream = new System.IO.FileStream("drtfiles\\drtimaging\\avalon.png", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapFrame frame = decoder.Frames[0];

                InPlaceBitmapMetadataWriter inplaceWriter = frame.CreateInPlaceBitmapMetadataWriter();
            }
            catch (InvalidOperationException)
            {
                // This exception is expected.
                succeeded = true;
            }
            DRT.Assert(succeeded);

            // ------------------------- Test InPlaceBitmapMetadataWriter
            Stream pngStream = new System.IO.FileStream("output_avalon.png", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            PngBitmapDecoder pngDecoder = new PngBitmapDecoder(pngStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame pngFrame = pngDecoder.Frames[0];
            InPlaceBitmapMetadataWriter pngInplace = pngFrame.CreateInPlaceBitmapMetadataWriter();
            pngInplace.SetQuery("/tEXt/Software","MICROSOFT OFFICE");
            DRT.Assert(pngInplace.TrySave());

            // Test Enumerator
            Test_Metadata_Enumerate_All(pngInplace, true);

            Stream jpgStream = new System.IO.FileStream("output_tulip.jpg", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            JpegBitmapDecoder jpgDecoder = new JpegBitmapDecoder(jpgStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame jpgFrame = jpgDecoder.Frames[0];
            InPlaceBitmapMetadataWriter jpgInplace = jpgFrame.CreateInPlaceBitmapMetadataWriter();
            jpgInplace.SetQuery("/app1/ifd/{uint=2000}", 23456);
            jpgInplace.RemoveQuery("/app1/ifd/{uint=1000}");
            jpgInplace.SetQuery("/app1/ifd/{uint=2001}", (byte)'a');
            jpgInplace.SetQuery("/app1/ifd/{uint=2002}", (char)'b');
            jpgInplace.SetQuery("/app1/ifd/{uint=2003}", (int)(-13245678));
            jpgInplace.SetQuery("/app1/ifd/{uint=2004}", (uint)87654321);
            jpgInplace.SetQuery("/app1/ifd/{uint=2005}", (short)(-1234));
            jpgInplace.SetQuery("/app1/ifd/{uint=2006}", (ushort)0xfedc);
            jpgInplace.SetQuery("/app1/ifd/{uint=2007}", (Int64)(-123456789123456789));
            jpgInplace.SetQuery("/app1/ifd/{uint=2008}", (UInt64)0xfedcfedcfedcfedc);
            jpgInplace.SetQuery("/app1/ifd/{uint=2009}", (String)"Test String");
            DRT.Assert(!jpgInplace.IsFrozen);
            DRT.Assert(!((BitmapMetadata)jpgInplace.GetQuery("/app1")).IsFrozen);
            DRT.Assert(jpgInplace.TrySave());

            // Test Enumerator
            Test_Metadata_Enumerate_All(jpgInplace, true);

            Stream tiffStream = new System.IO.FileStream("output_a-b-2.tif", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            TiffBitmapDecoder tiffDecoder = new TiffBitmapDecoder(tiffStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapFrame tiffFrame = tiffDecoder.Frames[0];
            InPlaceBitmapMetadataWriter tiffInplace = tiffFrame.CreateInPlaceBitmapMetadataWriter();
            tiffInplace.SetQuery("/ifd/{uint=2000}", 23456);
            tiffInplace.RemoveQuery("/ifd/{uint=1001}");
            tiffInplace.SetQuery("/ifd/{uint=2001}", (byte)'a');
            tiffInplace.SetQuery("/ifd/{uint=2002}", (sbyte)-100);
            tiffInplace.SetQuery("/ifd/{uint=2003}", (char)'x');
            tiffInplace.SetQuery("/ifd/{uint=2004}", (short)(-1234));
            tiffInplace.SetQuery("/ifd/{uint=2005}", (ushort)0xfedc);
            tiffInplace.SetQuery("/ifd/{uint=2006}", (String)"Test String");
            tiffInplace.SetQuery("/ifd/{uint=2007}", (int)(-13245678));
            tiffInplace.SetQuery("/ifd/{uint=2008}", (uint)87654321);
            tiffInplace.SetQuery("/ifd/{uint=2009}", (Int64)(-123456789123456789));
            tiffInplace.SetQuery("/ifd/{uint=2010}", (UInt64)0xfedcfedcfedcfedc);
            tiffInplace.SetQuery("/ifd/{uint=2011}", (float)3.14159f);
            tiffInplace.SetQuery("/ifd/{uint=2012}", (double)3.14159);
            tiffInplace.SetQuery("/ifd/{uint=2013}", new BitmapMetadataBlob(new byte[] { 0x00, 0x01, 0x02, 0x03 }));
            BitmapMetadata exifMetadata = (BitmapMetadata) tiffInplace.GetQuery("/ifd/exif");

            exifMetadata.SetQuery("/{uint=2020}", new byte[] { 80, 100, 200 });
            exifMetadata.SetQuery("/{uint=2021}", new sbyte[] { -80, -100, 100 });
            exifMetadata.SetQuery("/{uint=2022}", new char[] { 'a', 'b', 'c' });
            exifMetadata.SetQuery("/{uint=2023}", new ushort[] { 0x1000, 0x8000, 0xf000 });
            exifMetadata.SetQuery("/{uint=2024}", new short[] { -1000, 2000, -30000 });
            exifMetadata.SetQuery("/{uint=2025}", new String[] {"This","Is","A","Test" });
            exifMetadata.SetQuery("/{uint=2026}", new uint[] { 0x20000000, 0x80000000, 0xffff0000 });
            unchecked { exifMetadata.SetQuery("/{uint=2027}", new int[] { (int)0x200000000, (int)0x80000000, (int)0xffff0000 }); }
            exifMetadata.SetQuery("/{uint=2028}", new UInt64[] { 0x2000000000000000, 0x8000000000000000, 0xffffffff00000000 });
            unchecked { exifMetadata.SetQuery("/{uint=2029}", new Int64[] { (Int64)0x2000000000000000, (Int64)0x8000000000000000, (Int64)0xffffffff00000000 }); }

            exifMetadata.SetQuery("/{uint=2030}", new float[] { 1.2345f, 2.3456f, 3.456f });
            exifMetadata.SetQuery("/{uint=2031}", new double[] { 1.2345, 2.3456, 3.4567 });
            exifMetadata.SetQuery("/{uint=2032}", new char[][] {
                ((String)"Test").ToCharArray(),"Again".ToCharArray() });

            DRT.Assert(tiffInplace.TrySave());

            // Test Enumerator
            Test_Metadata_Enumerate_All(tiffInplace, true);

            // Test InPlaceBitmapMetadataWriter.Clone()
            succeeded = false;
            try
            {
                InPlaceBitmapMetadataWriter writerCopy = tiffInplace.Clone();
            }
            catch (InvalidOperationException)
            {
                succeeded = true;
            }
            DRT.Assert(succeeded);

            BitmapMetadata exifMetadata2 = (BitmapMetadata) tiffInplace.GetQuery("/ifd/exif");

            // Test BitmapMetadata.Clone()
            BitmapMetadata exifCopy = exifMetadata.Clone();

            // Even though it's a deep copy, the location is reflective of where the bitmap metadata came from.
            // exifMetadata came from the location"/ifd/exif"
            // exifCopy came from the root of exifMetadata"/"s
            DRT.Assert(exifCopy.Location =="/exif");
            DRT.Assert(exifMetadata.Location =="/ifd/exif");

            Test_BitmapMetadata_Equal(exifMetadata, exifMetadata2);

            // Test unmanaged IWICMetadataWriter* QL Copy()
            Test_BitmapMetadata_Equal(exifCopy, exifMetadata);

            // Test managed BitmapMetadataWriter QL Copy()
            Test_BitmapMetadata_Equal(exifCopy, exifCopy.Clone());

            // Test unmanaged IWICMetadataBlockReader* QL Copy()
            BitmapMetadata tiffCopy = ((BitmapMetadata)tiffFrame.Metadata).Clone();
            Test_BitmapMetadata_Equal((BitmapMetadata)tiffFrame.Metadata, tiffCopy);

            Test_BitmapMetadata_Equal(tiffCopy, tiffCopy.Clone());
        }

        private void Test_LossLessRotation()
        {
            Console.WriteLine("Test LossLessRotation");
            System.Windows.Media.Imaging.BitmapFrame nonLosslessImage = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\img_0002.jpg", UriKind.RelativeOrAbsolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

            OpenContext();
            RenderImage(nonLosslessImage);
            CloseContext();

            System.Windows.Media.Imaging.JpegBitmapEncoder nonLossslessEncoder = new System.Windows.Media.Imaging.JpegBitmapEncoder();
            nonLossslessEncoder.Frames.Add(nonLosslessImage);
            nonLossslessEncoder.Rotation = System.Windows.Media.Imaging.Rotation.Rotate90;

            using (FileStream stm = new FileStream("nonLossless.jpg", FileMode.Create, FileAccess.ReadWrite))
            {
                nonLossslessEncoder.Save(stm);
            }

            System.Windows.Media.Imaging.BitmapFrame image = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\lossless.jpg", UriKind.RelativeOrAbsolute));

            OpenContext();
            RenderImage(image);
            CloseContext();

            System.Windows.Media.Imaging.JpegBitmapEncoder jpeg1 = new System.Windows.Media.Imaging.JpegBitmapEncoder();
            jpeg1.Frames.Add(image);
            jpeg1.Rotation = System.Windows.Media.Imaging.Rotation.Rotate90;
            jpeg1.Rotation = System.Windows.Media.Imaging.Rotation.Rotate180;
            jpeg1.FlipHorizontal = true;
            jpeg1.Rotation = System.Windows.Media.Imaging.Rotation.Rotate270;
            jpeg1.Rotation = System.Windows.Media.Imaging.Rotation.Rotate90;
            jpeg1.FlipVertical = true;
            jpeg1.FlipVertical = false;


            using (FileStream stm = new FileStream("1.jpg", FileMode.Create, FileAccess.ReadWrite))
            {
                jpeg1.Save(stm);
            }

            System.Windows.Media.Imaging.JpegBitmapEncoder jpeg2 = new System.Windows.Media.Imaging.JpegBitmapEncoder();
            jpeg2.Frames.Add(image);
            jpeg2.Rotation = System.Windows.Media.Imaging.Rotation.Rotate180;

            using (FileStream stm = new FileStream("2.jpg", FileMode.Create, FileAccess.ReadWrite))
            {
                jpeg2.Save(stm);
            }

            System.Windows.Media.Imaging.JpegBitmapEncoder jpeg3 = new System.Windows.Media.Imaging.JpegBitmapEncoder();
            jpeg3.Frames.Add(image);
            jpeg3.Rotation = System.Windows.Media.Imaging.Rotation.Rotate270;

            using (FileStream stm = new FileStream("3.jpg", FileMode.Create, FileAccess.ReadWrite))
            {
                jpeg3.Save(stm);
            }

            System.Windows.Media.Imaging.JpegBitmapEncoder jpeg4 = new System.Windows.Media.Imaging.JpegBitmapEncoder();
            jpeg4.Frames.Add(image);
            jpeg4.FlipHorizontal = true;

            using (FileStream stm = new FileStream("4.jpg", FileMode.Create, FileAccess.ReadWrite))
            {
                jpeg4.Save(stm);
            }

            System.Windows.Media.Imaging.JpegBitmapEncoder jpeg5 = new System.Windows.Media.Imaging.JpegBitmapEncoder();
            jpeg5.Frames.Add(image);
            jpeg5.FlipVertical = true;

            using (FileStream stm = new FileStream("5.jpg", FileMode.Create, FileAccess.ReadWrite))
            {
                jpeg5.Save(stm);
            }
        }

        private void Test_Cloning()
        {
            Console.WriteLine("Test Cloning");
            OpenContext();

            BitmapFrame tifFrame = BitmapFrame.Create(new Uri(@"drtfiles\\drtimaging\\48RGBBig.tif", UriKind.RelativeOrAbsolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            BitmapFrame newFrame = System.Windows.Media.Imaging.BitmapFrame.Create(tifFrame);

            RenderImage(tifFrame);
            RenderImage(newFrame);

            FileStream pngStream = new FileStream(@"drtfiles\\drtimaging\\cloud1.png", FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapFrame pngFrame = BitmapFrame.Create(pngStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            BitmapFrame newFrame2 = System.Windows.Media.Imaging.BitmapFrame.Create(pngFrame);

            RenderImage(pngFrame);
            RenderImage(newFrame2);

            BitmapSource source = null;
            Stream imageStreamSource = new System.IO.FileStream("drtfiles\\drtimaging\\a-b-3.tif", FileMode.Open, FileAccess.Read, FileShare.Read);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = imageStreamSource;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();

            imageStreamSource.Close();
            source = bi.Clone();

            RenderImage(bi);
            RenderImage(source);

            BitmapFrame frame = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\avalon.png", UriKind.RelativeOrAbsolute));
            source = frame.Clone();

            RenderImage(frame);
            RenderImage(source);

            Stream imageStreamSource2 = new System.IO.FileStream("drtfiles\\drtimaging\\a-b-3.tif", FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapDecoder decoder = BitmapDecoder.Create(imageStreamSource2, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            for (int i = 0; i < decoder.Frames.Count; i++)
            {
                BitmapSource src = decoder.Frames[i];
                RenderImage(src);
                RenderImage(src.Clone());
            }

            CloseContext();
        }

        private void Test_Cache()
        {
            Console.WriteLine("Test Cache");
            OpenContext();

            BitmapFrame frame = BitmapFrame.Create(new Uri("drtfiles\\drtimaging\\avalon.png", UriKind.RelativeOrAbsolute));
            BitmapDecoder decoder = BitmapDecoder.Create(
                new Uri("drtfiles\\drtimaging\\avalon.png", UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.IgnoreImageCache,
                BitmapCacheOption.Default
                );

            RenderImage(frame);
            RenderImage(decoder.Frames[0]);

            CloseContext();
        }

        private void Test_SaveGPS()
        {
            Console.WriteLine("Test SaveGPS");
            Stream imageStreamSource = new System.IO.FileStream("drtfiles\\drtimaging\\gps.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
            Stream imageStreamDest = new System.IO.FileStream("gpsoutput.jpg", FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            BitmapDecoder decoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.None, BitmapCacheOption.Default);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            encoder.Frames = decoder.Frames;

            encoder.Save(imageStreamDest);
        }

        [DllImport("DrtImagingD3D.dll")]
        static extern int Init(out IntPtr pSurface, bool forceXDDM);
        [DllImport("DrtImagingD3D.dll")]
        static extern int Render();
        [DllImport("DrtImagingD3D.dll")]
        static extern void Destroy();

        // Basic D3DImage test that uses a 9Ex device when WDDM is present or 9 when not
        private void Test_D3DImage()
        {
            Console.WriteLine("Test D3DImage 9Ex");
            Test_D3DImage_Common(false);
        }

        // Same as Test_D3DImage except it forces a 9 device. On XP, this will be the same as Test_D3DImage
        private void Test_D3DImage_Force9()
        {
            Console.WriteLine("Test D3DImage 9");
            Test_D3DImage_Common(true);
        }

        private void Test_D3DImage_Common(bool forceXDDM)
        {
            D3DImage d3di = new D3DImage();

            // 1. Test stardard D3DImage
            IntPtr surface;
            DRT.Assert(Init(out surface, forceXDDM) >= 0, "Unable to create unmanaged rendering core for D3DImage");

            d3di.Lock();
            d3di.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface);
            DRT.Assert(Render() >= 0, "D3DImage renderer failed");
            d3di.AddDirtyRect(new Int32Rect(0, 0, d3di.PixelWidth, d3di.PixelHeight));
            d3di.Unlock();

            OpenContext();
            RenderImage(d3di);

            // 2. RTB D3DImage
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                ctx.DrawImage(d3di, new Rect(0, 0, d3di.PixelWidth, d3di.PixelHeight));
            }

            RenderTargetBitmap rtb = new RenderTargetBitmap(d3di.PixelWidth, d3di.PixelHeight, 96.0, 96.0, PixelFormats.Default);
            rtb.Render(dv);

            RenderImage(rtb);
            CloseContext();

            DRT.WaitForCompleteRender();

            // 3. Release unmanaged stuff
            Destroy();
        }

        private void OpenContext()
        {
            DRT.Assert(_ctx == null);
            _ctx = _rootVisual.RenderOpen();
        }

        private void CloseContext()
        {
            DRT.Assert(_ctx != null);
            _ctx.Close();
            _ctx = null;
            _currentXOffset = 0.0;
            _currentYOffset = 0.0;
        }

        private void RenderImage(ImageSource source)
        {
            DRT.Assert(_ctx != null);
            if ((_currentXOffset + _imageRenderWidth) > 800.0)
            {
                _currentXOffset = 0.0;
                _currentYOffset += _imageRenderHeight;
            }

            _ctx.DrawImage(source, new Rect(_currentXOffset, _currentYOffset, _imageRenderWidth, _imageRenderHeight));
            _currentXOffset += _imageRenderWidth;
            DRT.WaitForCompleteRender(); // Wait until original Visual is displayed
        }


        DrawingVisual _rootVisual;
        DrawingContext _ctx;
        private double _imageRenderWidth = 100.0;
        private double _imageRenderHeight = 100.0;
        private double _currentXOffset = 0.0;
        private double _currentYOffset = 0.0;


        [SecurityCritical, SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", EntryPoint ="CloseHandle")]
        internal static extern uint CloseHandle(
            IntPtr hObject);

        [SecurityCritical, SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint ="CreateFileW")]
        internal static extern IntPtr CreateFile(
            [MarshalAs(UnmanagedType.LPWStr)] string lpFilename,
            UInt32 dwDesiredAccess,
            UInt32 dwShareMode,
            IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [SecurityCritical, SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint ="CreateFileMapping")]
        internal static extern IntPtr CreateFileMapping(
            IntPtr hFile,
            IntPtr lpAttributes,
            UInt32 flProtect,
            UInt32 dwMaximumSizeHigh,
            UInt32 dwMaximumSizeLow,
            IntPtr lpName);
    }
}
























