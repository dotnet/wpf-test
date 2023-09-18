// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Regression test for Regression_Bug40
    /// </summary>
    // commented ignored tests
    // [Test(1, "Regression", "Regression_Bug40",
    //     Area = "2D",
    //     Description = @"Regression test for Regression_Bug40 : 
    //            Difference(Different Size and blurry) on RenderDataContext, DrawingGroupDrawingContext when render a geometry stroke with brush that has text")]
    public class Regression_Bug40 : WindowTest
    {
        private readonly int _waitTimeInMilliseconds = 1000;
        private Rectangle _rectangle;
        private ImageAdapter _firstAdapter;
        private ImageAdapter _secondAdapter;

        /// <summary>
        /// Constructor
        /// </summary>
        [Variation()]
        public Regression_Bug40()
        {
            InitializeSteps += new TestStep(CreateWindowContent);

            RunSteps += new TestStep(CapureFirstWindow);

            RunSteps += new TestStep(CapureSecondWindow);

            RunSteps += new TestStep(Verification);
        }

        /// <summary>
        /// Create a Rectangle as window content.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult CreateWindowContent()
        {
            _rectangle = new Rectangle();
            _rectangle.Width = 200;
            _rectangle.Height = 200;

            Window.Content = _rectangle;

            return TestResult.Pass;
        }

        private TestResult CapureFirstWindow()
        {
            Visual visual = CreateVisualWithDrawGeometry();
            VisualBrush vb = new VisualBrush(visual);
            _rectangle.Fill = vb;

            WaitFor(_waitTimeInMilliseconds);

            _firstAdapter = new ImageAdapter(ImageUtility.CaptureElement(_rectangle));

            return TestResult.Pass;
        }

        private TestResult CapureSecondWindow()
        {
            Visual visual = CreateVisualWithDrawGeometry();
            VisualBrush vb = new VisualBrush(visual);
            _rectangle.Fill = vb;

            WaitFor(_waitTimeInMilliseconds);

            _secondAdapter = new ImageAdapter(ImageUtility.CaptureElement(_rectangle));

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify Renderings of scenario create with two methods are the same.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Verification()
        {
            ImageComparator comparator = new ImageComparator();
            if (!comparator.Compare(_firstAdapter, _secondAdapter))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Create a visual with DrawGeometry.
        /// </summary>
        /// <returns>visual</returns>
        private Visual CreateVisualWithDrawGeometry()
        {
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();

            Pen pen = CreatePen();

            RectangleGeometry rectGeometry = CreateRectGeometry();

            dc.DrawRectangle(Brushes.White, null, new Rect(-10, -10, 500, 500));
            dc.DrawGeometry(null, pen, rectGeometry);
            dc.Close();

            return dv;
        }

        /// <summary>
        /// Create a visual with Drawing a DrawingGroup containding the geometry. 
        /// </summary>
        /// <returns>visual</returns>
        private Visual CreateVisualWithDrawDrawing()
        {
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();

            DrawingGroup dg = new DrawingGroup();
            DrawingContext dgdc = dg.Open();

            Pen pen = CreatePen();

            RectangleGeometry rectGeometry = CreateRectGeometry();

            dgdc.DrawGeometry(null, pen, rectGeometry);
            dgdc.Close();

            dc.DrawRectangle(Brushes.White, null, new Rect(-10, -10, 500, 500));
            dc.DrawDrawing(dg);
            dc.Close();

            return dv;
        }

        private RectangleGeometry CreateRectGeometry()
        {
            RectangleGeometry rectGeometry = new RectangleGeometry();
            rectGeometry.Rect = new Rect(100, 100, 110, 120);
            rectGeometry.Transform = new MatrixTransform(1, 0, 0, 0.8, 0, 0);

            return rectGeometry;
        }

        private Pen CreatePen()
        {
            DrawingBrush db = new DrawingBrush();
            db.TileMode = TileMode.Tile;
            db.Transform = new MatrixTransform(1, 0, 0, 0.8, 0, 0);
            GlyphRunDrawing glyphRunDrawing = new GlyphRunDrawing();
            glyphRunDrawing.ForegroundBrush = Brushes.Red;
            glyphRunDrawing.GlyphRun = CreateGlyphRun();
            db.Drawing = glyphRunDrawing;

            Pen pen = new Pen();
            pen.Brush = db;
            pen.Thickness = 100;

            return pen;
        }

        private GlyphRun CreateGlyphRun()
        {
            Point origin = new Point(5, 20);
            double fontSize = 8;
            string unicodeString = "We-will, we will Rock U!@~!";

            Uri fontUri = new Uri("pack://application:,,,/2D_Part1;;component/Resources/times.ttf");
            GlyphTypeface glyphTypeface = new GlyphTypeface(fontUri);

            IList<ushort> glyphIndices = new List<ushort>();
            for (int i = 0; i < unicodeString.Length; ++i)
            {
                glyphIndices.Add(glyphTypeface.CharacterToGlyphMap[unicodeString[i]]);
            }

            IList<double> advanceWidths = new List<double>();
            for (int i = 0; i < glyphIndices.Count; ++i)
            {
                advanceWidths.Add(glyphTypeface.AdvanceWidths[glyphIndices[i]] * fontSize);
            }

            GlyphRun glyphRun = new GlyphRun(
                   glyphTypeface,   //glyph typeface
                   0,               //bidi level
                   false,           //sideways
                   fontSize,        //font size
                   glyphIndices,    //glyph indices
                   origin,          //baseline origin
                   advanceWidths,   //advance widths
                   null,            //glyph offsets
                   unicodeString.ToCharArray(),   //test string
                   null,            //device font name
                   null,            //cmap
                   null,            //caret stop
                   null);             //culture info

            return glyphRun;
        }
    }
}