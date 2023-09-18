// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
/******************************************************************* 
 * Purpose: ClearTypeHint testing
 ********************************************************************/
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    //the various content types from our test vector in spec
    public enum OpacityContentType
    {
        TiledDrawingBrush,
        VisualBrush,
        DrawingBrush_TextBlock,
        DrawingBrush_GlyphRunDrawing,
        DrawingVisual,
        DrawingBrush_Label,
        TiledVisualBrush
    }
    /// <summary>
    /// Verification for ClearTypeHint, which allows us to force Cleartype rendering when the 
    ///  framework would normally disallow it.
    /// </summary>
    [Test(0, "ClearType", "ClearTypeHintDev10", // Positional variables for the subarea and name.
        Area = "2D", // Named variable for the area.
        SupportFiles = @"FeatureTests\Text\Data\times.ttf",
        Description = "Verification for ClearTypeHint")] // Named variable for the desc. 
    public class ClearTypeHintPushOpacitytests : WindowTest
    {
        /// <summary>
        /// Constructor
        /// Takes these inputs:
        /// 1. The full path and name of the xaml file to load and render.
        /// 2. Whether we expect the text to render with cleartype.
        /// 3. The row in the image to scan to detect cleartype - this is unfortunately different for each case
        /// Eventually these should include the full set of Factors from the test spec.
        /// </summary>




        #region variations
        [Variation(true, true, 40, false, OpacityContentType.TiledDrawingBrush)]
        [Variation(true, true, 40, false, OpacityContentType.VisualBrush)]
        [Variation(true, true, 30, false, OpacityContentType.DrawingBrush_TextBlock)]
        [Variation(true, true, 30, false, OpacityContentType.DrawingBrush_GlyphRunDrawing)]
        [Variation(true, true, 40, true, OpacityContentType.DrawingVisual)]
        [Variation(true, true, 30, false, OpacityContentType.DrawingBrush_Label)]
        [Variation(true, true, 40, false, OpacityContentType.TiledVisualBrush)]
        #endregion
        public ClearTypeHintPushOpacitytests(
                                    bool clearTypeExpected,
                                    bool transparentWindow,
                                    int rowToScan,
                                    bool RTBRequested,
                                    OpacityContentType contentRequested
                                    )
            : base(transparentWindow) // construct the base class (WindowTest) with a transparent window
        {
            this._clearTypeExpected = clearTypeExpected;
            this._transparentWindow = transparentWindow;
            this._rowToScan = rowToScan;
            this._RTBRequested = RTBRequested;
            this._contentRequested = contentRequested;

            InitializeSteps += new TestStep(CreateOpacityPage);

            RunSteps += new TestStep(RenderContent);

            if (RTBRequested)
            {
                RunSteps += new TestStep(RenderContentToRTB);// these have to be sequential
            }

            RunSteps += new TestStep(CaptureWindow);

            RunSteps += new TestStep(Verification);
        }

        #region test steps

        private TestResult CreateOpacityPage()
        {
            OpacityOpPage OpPage = new OpacityOpPage(this._contentRequested);
            this.Window.Content = OpPage;
            return TestResult.Pass;
        }

        /// <returns></returns>
        private TestResult RenderContent()
        {
            Status("Rendering ...");
            Window.Height = 100;
            Window.Width = 200;
            WaitFor(100);
            return TestResult.Pass;
        }

        /// <returns></returns>
        private TestResult RenderContentToRTB()
        {
            //render to an RTB if requested
            double DPI = Microsoft.Test.Display.Monitor.Dpi.x;//x and y will always have same DPI - ask Gerhard
            RenderTargetBitmap RTB = new RenderTargetBitmap((int)Window.Width, (int)Window.Height, DPI, DPI, PixelFormats.Default);
            RTB.Render(Window);
            //now that we've rendered the Window contents into the render target bitmap,
            //use those as the contents of the window so that we can capture it.
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = RTB;
            Window.Content = img;
            WaitFor(100);//wait again for the render.

            return TestResult.Pass;
        }

        /// <returns>TestResult</returns>
        private TestResult CaptureWindow()
        {
            Status("Capturing window...");
            _capture = ImageUtility.CaptureElement(Window.Content as UIElement);
            return TestResult.Pass;
        }

        /// <summary>
        /// Detect cleartype and compare to expected result.
        /// Saves the rendered image and xaml if there is an error.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Verification()
        {
            bool clearTypeDetected = false;
            //walk through the bitmap and detect blue-red shift (or just non greyscale)
            Status("Checking all pixels on midline in the rendered text for red-blue shift.");
            clearTypeDetected = ClearTypeDetector.CheckForRedBlueShift(_capture, this._rowToScan);

            TestResult result = TestResult.Unknown;
            Status("Cleartype detected: [" + clearTypeDetected + "] Cleartype expected: [" + _clearTypeExpected + "]");

            if (clearTypeDetected == _clearTypeExpected)
            {
                Status("Results match expected behaviour - PASS");
                result = TestResult.Pass;
            }
            if (clearTypeDetected != _clearTypeExpected)
            {
                Status("Results DO NOT match expected behaviour - FAIL");
                //log the image  
                //we can open up the xaml file in xamlpad!
                string saveFile = this._contentRequested.ToString() + ".bmp";
                Status("Saving captured image to " + saveFile);
                ImageAdapter img = new ImageAdapter(_capture);
                ImageUtility.ToImageFile(img, saveFile);
                Log.LogFile(saveFile);
                result = TestResult.Fail;
            }
            return result;
        }
        #endregion

        #region member variables
        private Bitmap _capture;                     // captured image of our rendered text

        //Factors from test spec
        private bool _transparentWindow;             // should the root window be transparent
        private bool _clearTypeExpected;             // do we expect CT to be present
        private int _rowToScan;                      // the row of the rendered image to scan for CT
        private bool _RTBRequested;                  // should this test perform a RenderTargetBitmap step
        OpacityContentType _contentRequested;               // the type of opacity DC op requested
        #endregion
    /// <summary>
    /// This is a custom page that populates itself with one of the types of content from the 'OpacityContentType' enum.
    /// </summary>
    private class OpacityOpPage : Page
    {
        public OpacityOpPage(OpacityContentType content)
            : base()
        {
            System.Windows.Media.Pen shapeOutlinePen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 2);
            shapeOutlinePen.Freeze();

            // Create a DrawingGroup
            DrawingGroup dGroup = new DrawingGroup();

            // Obtain a DrawingContext from 
            // the DrawingGroup.
            using (DrawingContext dc = dGroup.Open())
            {
                dc.PushOpacity(0.99);
                TextBlock tb = new TextBlock();
                tb.Text = "Clear";
                RenderOptions.SetClearTypeHint(tb, ClearTypeHint.Enabled); 
                Button CTb = new Button();
                CTb.Content = tb;
                VisualBrush CTvb = new VisualBrush();
                CTvb.Visual = CTb;
                RectangleGeometry rg = new RectangleGeometry(new Rect(0, 0, 100, 100));
                System.Windows.Media.Pen p = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1);
                dc.DrawGeometry(CTvb, p, rg);
                dc.Pop();
            }

            // Display the drawing using an image control.
            System.Windows.Controls.Image theImage = new System.Windows.Controls.Image();
            DrawingImage dImageSource = new DrawingImage(dGroup);
            theImage.Source = dImageSource;

            //populate the page with different content or background based on input
            //ideally use the above drawinggroup as nested content inside whatever
            //container was requested.

            VisualBrush vb = new VisualBrush(theImage);
            DrawingBrush db = new DrawingBrush(dGroup);
            Button b = new Button();
            b.Height = 50;
            b.Width = 50;
            StackPanel sp = new StackPanel();

            switch (content)
            {
                case OpacityContentType.DrawingBrush_GlyphRunDrawing:
                    b.Background = db;
                    sp.Children.Add(b);
                    GlyphRunDrawing grd;
                    //details of the glyphrundrawing hidden away in a helper method because none of them are relevant to this test.
                    //we just need A glyphrundrawing - I used the stock one from the MSDN page.
                    grd = CreateGlyphRunDrawing(); 
                    DrawingImage di = new DrawingImage(grd);
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    img.Source = di;
                    sp.Children.Add(img);
                    this.Content = sp;

                    break;
                case OpacityContentType.DrawingBrush_Label:
                    b.Background = db;
                    sp.Children.Add(b);
                    Label l = new Label();
                    l.Content="foo";
                    sp.Children.Add(l);
                    this.Content = sp;
                    break;
                case OpacityContentType.DrawingBrush_TextBlock:
                    b.Background = db;
                    sp.Children.Add(b);
                    TextBlock tb = new TextBlock();
                    tb.Text = "foo";
                    sp.Children.Add(tb);
                    this.Content = sp;
                    break;
                case OpacityContentType.DrawingVisual:
                    this.Content = theImage;//already done : D
                    break;
                case OpacityContentType.TiledDrawingBrush:
                    db.TileMode = TileMode.Tile;
                    this.Background = db;
                    break;
                case OpacityContentType.TiledVisualBrush:
                    vb.TileMode = TileMode.Tile;
                    this.Background = vb;
                    break;
                case OpacityContentType.VisualBrush:
                    this.Background = vb;
                    //so that has the dg nested in a vb and brushed in the bg
                    break;
            }
        }

        //Create a GlyphRunDrawing for test content
        private static GlyphRunDrawing CreateGlyphRunDrawing()
        {
            GlyphRunDrawing grd;
            grd = new GlyphRunDrawing(System.Windows.Media.Brushes.Black, new GlyphRun(
                new GlyphTypeface(new Uri("pack://siteoforigin:,,,/times.ttf")),
                    0,
                    false,
                    13.333333333333334,
                    new ushort[] { 43, 72, 79, 79, 82, 3, 58, 82, 85, 79, 71 },
                    new System.Windows.Point(0, 12.29),
                    new double[]{
                                9.62666666666667, 7.41333333333333, 2.96, 
                                2.96, 7.41333333333333, 3.70666666666667, 
                                12.5866666666667, 7.41333333333333, 
                                4.44, 2.96, 7.41333333333333},
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                    )
                );

            return grd;
        }
    }
        
    }

}

