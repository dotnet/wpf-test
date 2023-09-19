// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Globalization;
using System.Collections;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class DrawingGroupStructureTest : CoreGraphicsTest
    {
        private DrawingVisual _drawingVisual;
        private DrawingContext _drawingContext;
        private ArrayList _drawingContextCommands;
        private DrawingGroupStructureVerifier _verifier;
        private int _needPop;
        private DrawingGroup _drawingGroup;

        /// <summary/>
        public override void RunTheTest()
        {
            TestDrawingGroupStructure1();
            TestDrawingGroupStructure2();
            TestDrawingGroupStructure3();
            TestDrawingGroupStructure4();
            TestDrawingGroupStructure5();
            TestDrawingGroupStructure6();
            TestDrawingGroupStructure7();
            TestDrawingGroupStructure8();
            TestDrawingGroupStructure9();
            TestDrawingGroupStructure10();
            TestDrawingGroupStructure11();
            TestDrawingGroupStructure12();
            TestDrawingGroupStructure13();
            TestDrawingGroupStructure14();
            TestDrawingGroupStructure15();
            TestDrawingGroupStructure16();
            TestDrawingGroupStructure17();
            TestDrawingGroupStructure18();
            TestDrawingGroupStructure19();
            TestDrawingGroupStructure20();
            TestDrawingGroupStructure21();
            TestDrawingGroupStructure22();
            TestDrawingGroupStructure23();
            TestDrawingGroupStructure24();
            TestDrawingGroupStructure25();
            TestDrawingGroupStructure26();
        }

        private void SetupDrawingVisual()
        {
            _drawingVisual = new DrawingVisual();
            _drawingContext = _drawingVisual.RenderOpen();
            _drawingContextCommands = new ArrayList();
            _verifier = new DrawingGroupStructureVerifier(this);
            _needPop = 0;
        }

        private void SetupDrawingGroup()
        {
            _drawingGroup = new DrawingGroup();
            _drawingContext = _drawingGroup.Open();
            _drawingContextCommands = new ArrayList();
            _verifier = new DrawingGroupStructureVerifier(this);
            _needPop = 0;
        }

        private void TestDrawingGroupStructure1()
        {
            Log("Testing DrawingGroupStructure1...");
            // this test case has every draw commands

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(0, 0), new Point(100, 100));
            AddDrawEllipse(Brushes.Pink, new Pen(Brushes.Yellow, 4), new Point(15, 15), 3, 8);
            AddDrawDrawing(Const2D.geometryDrawing1);
            AddDrawDrawing(Const2D.imageDrawing1);
            AddDrawDrawing(Const2D.glyphRunDrawing1);
            AddDrawDrawing(Const2D.videoDrawing1);
            AddDrawDrawing(Const2D.drawingGroup1);
            AddDrawRectangle(Brushes.Blue, new Pen(Brushes.Green, 5), new Rect(0, 0, 10, 10));
            AddDrawRoundedRectangle(Brushes.Black, new Pen(Brushes.White, 2), new Rect(0, 0, 10, 11), 5, 3);
            AddDrawGeometry(Brushes.Cyan, new Pen(Brushes.Red, 9), new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35));
            AddDrawGlyphRun(Brushes.Red, Const2D.glyphRun1);
            AddDrawText(Const2D.formattedText1, new Point(1.5, -1.5));
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5));
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure2()
        {
            Log("Testing DrawingGroupStructure2...");
            // this test case has PushTransform followed by every draw commands

            SetupDrawingVisual();

            // *** put test cases here
            AddPushTransform(Const2D.translate10);
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(0, 0), new Point(100, 100));
            AddDrawEllipse(Brushes.Pink, new Pen(Brushes.Yellow, 4), new Point(15, 15), 3, 8);
            AddDrawDrawing(Const2D.geometryDrawing1);
            AddDrawDrawing(Const2D.imageDrawing1);
            AddDrawDrawing(Const2D.glyphRunDrawing1);
            AddDrawDrawing(Const2D.videoDrawing1);
            AddDrawDrawing(Const2D.drawingGroup1);
            AddDrawRectangle(Brushes.Blue, new Pen(Brushes.Green, 5), new Rect(0, 0, 10, 10));
            AddDrawRoundedRectangle(Brushes.Black, new Pen(Brushes.White, 2), new Rect(0, 0, 10, 11), 5, 3);
            AddDrawGeometry(Brushes.Cyan, new Pen(Brushes.Red, 9), new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35));
            AddDrawGlyphRun(Brushes.Red, Const2D.glyphRun1);
            AddDrawText(Const2D.formattedText1, new Point(1.5, -1.5));
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5));
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure3()
        {
            Log("Testing DrawingGroupStructure3...");
            // this test case has PushOpacity followed by every draw commands

            SetupDrawingVisual();

            // *** put test cases here
            AddPushOpacity(0.5);
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(0, 0), new Point(100, 100));
            AddDrawEllipse(Brushes.Pink, new Pen(Brushes.Yellow, 4), new Point(15, 15), 3, 8);
            AddDrawDrawing(Const2D.geometryDrawing1);
            AddDrawDrawing(Const2D.imageDrawing1);
            AddDrawDrawing(Const2D.glyphRunDrawing1);
            AddDrawDrawing(Const2D.videoDrawing1);
            AddDrawDrawing(Const2D.drawingGroup1);
            AddDrawRectangle(Brushes.Blue, new Pen(Brushes.Green, 5), new Rect(0, 0, 10, 10));
            AddDrawRoundedRectangle(Brushes.Black, new Pen(Brushes.White, 2), new Rect(0, 0, 10, 11), 5, 3);
            AddDrawGeometry(Brushes.Cyan, new Pen(Brushes.Red, 9), new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35));
            AddDrawGlyphRun(Brushes.Red, Const2D.glyphRun1);
            AddDrawText(Const2D.formattedText1, new Point(1.5, -1.5));
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5));
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure4()
        {
            Log("Testing DrawingGroupStructure4...");
            // this test case has PushClip followed by every draw commands

            SetupDrawingVisual();

            // *** put test cases here
            AddPushClip(new RectangleGeometry(new Rect(1.5, -1.5, 1.5, 1.5)));
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(0, 0), new Point(100, 100));
            AddDrawEllipse(Brushes.Pink, new Pen(Brushes.Yellow, 4), new Point(15, 15), 3, 8);
            AddDrawDrawing(Const2D.geometryDrawing1);
            AddDrawDrawing(Const2D.imageDrawing1);
            AddDrawDrawing(Const2D.glyphRunDrawing1);
            AddDrawDrawing(Const2D.videoDrawing1);
            AddDrawDrawing(Const2D.drawingGroup1);
            AddDrawRectangle(Brushes.Blue, new Pen(Brushes.Green, 5), new Rect(0, 0, 10, 10));
            AddDrawRoundedRectangle(Brushes.Black, new Pen(Brushes.White, 2), new Rect(0, 0, 10, 11), 5, 3);
            AddDrawGeometry(Brushes.Cyan, new Pen(Brushes.Red, 9), new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35));
            AddDrawGlyphRun(Brushes.Red, Const2D.glyphRun1);
            AddDrawText(Const2D.formattedText1, new Point(1.5, -1.5));
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5));
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure5()
        {
            Log("Testing DrawingGroupStructure5...");
            // this test case has Pushs and Pops

            SetupDrawingVisual();

            // *** put test cases here
            AddPushTransform(Const2D.translate10);
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(0, 0), new Point(100, 100));
            AddPushOpacity(0.5);
            AddDrawEllipse(Brushes.Pink, new Pen(Brushes.Yellow, 4), new Point(15, 15), 3, 8);
            AddPushClip(new RectangleGeometry(new Rect(1.5, -1.5, 1.5, 1.5)));
            AddDrawDrawing(Const2D.geometryDrawing1);
            AddPushGuidelineSet(Const2D.guidelineSet1);
            AddDrawText(Const2D.formattedText1, new Point(1.5, -1.5));
            AddPushTransform(Const2D.translate10);
            AddDrawDrawing(Const2D.imageDrawing1);
            AddPushOpacity(0.5);
            AddDrawDrawing(Const2D.glyphRunDrawing1);
            AddPushClip(new RectangleGeometry(new Rect(1.5, -1.5, 1.5, 1.5)));
            AddDrawDrawing(Const2D.videoDrawing1);
            AddPushGuidelineSet(Const2D.guidelineSet1);
            AddDrawText(Const2D.formattedText1, new Point(1.5, -1.5));
            AddPop();
            AddDrawDrawing(Const2D.drawingGroup1);
            AddPop();
            AddDrawRectangle(Brushes.Blue, new Pen(Brushes.Green, 5), new Rect(0, 0, 10, 10));
            AddPop();
            AddDrawRoundedRectangle(Brushes.Black, new Pen(Brushes.White, 2), new Rect(0, 0, 10, 11), 5, 3);
            AddPop();
            AddDrawGeometry(Brushes.Cyan, new Pen(Brushes.Red, 9), new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddPop();
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35));
            AddPop();
            AddDrawGlyphRun(Brushes.Red, Const2D.glyphRun1);
            AddPop();
            AddDrawText(Const2D.formattedText1, new Point(1.5, -1.5));
            AddPop();
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5));
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure6()
        {
            Log("Testing DrawingGroupStructure6...");
            // this test case has Pushs without Pop

            SetupDrawingVisual();

            // *** put test cases here
            AddPushTransform(Const2D.translate10);
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(0, 0), new Point(100, 100));
            AddPushOpacity(0.5);
            AddDrawEllipse(Brushes.Pink, new Pen(Brushes.Yellow, 4), new Point(15, 15), 3, 8);
            AddPushClip(new RectangleGeometry(new Rect(1.5, -1.5, 1.5, 1.5)));
            AddDrawDrawing(Const2D.geometryDrawing1);
            AddPushTransform(Const2D.translate10);
            AddDrawDrawing(Const2D.imageDrawing1);
            AddPushOpacity(0.5);
            AddDrawDrawing(Const2D.glyphRunDrawing1);
            AddPushClip(new RectangleGeometry(new Rect(1.5, -1.5, 1.5, 1.5)));
            AddDrawDrawing(Const2D.videoDrawing1);
            AddPushGuidelineSet(Const2D.guidelineSet1);
            AddDrawDrawing(Const2D.drawingGroup1);
            AddDrawRectangle(Brushes.Blue, new Pen(Brushes.Green, 5), new Rect(0, 0, 10, 10));
            AddDrawRoundedRectangle(Brushes.Black, new Pen(Brushes.White, 2), new Rect(0, 0, 10, 11), 5, 3);
            AddDrawGeometry(Brushes.Cyan, new Pen(Brushes.Red, 9), new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35));
            AddDrawGlyphRun(Brushes.Red, Const2D.glyphRun1);
            AddDrawText(Const2D.formattedText1, new Point(1.5, -1.5));
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5));
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure7()
        {
            Log("Testing DrawingGroupStructure7...");
            // this test case has consecutive Pushs

            SetupDrawingVisual();

            // *** put test cases here
            AddPushTransform(Const2D.translate10);
            AddPushTransform(Const2D.translate10);
            AddPushOpacity(0.5);
            AddPushOpacity(0.5);
            AddPushClip(new RectangleGeometry(new Rect(1.5, -1.5, 1.5, 1.5)));
            AddPushClip(new RectangleGeometry(new Rect(1.5, -1.5, 1.5, 1.5)));
            AddPushGuidelineSet(Const2D.guidelineSet1);
            AddPushGuidelineSet(Const2D.guidelineSet1);
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(0, 0), new Point(100, 100));
            AddDrawEllipse(Brushes.Pink, new Pen(Brushes.Yellow, 4), new Point(15, 15), 3, 8);
            AddDrawDrawing(Const2D.geometryDrawing1);
            AddDrawDrawing(Const2D.imageDrawing1);
            AddDrawDrawing(Const2D.glyphRunDrawing1);
            AddDrawDrawing(Const2D.videoDrawing1);
            AddDrawDrawing(Const2D.drawingGroup1);
            AddDrawRectangle(Brushes.Blue, new Pen(Brushes.Green, 5), new Rect(0, 0, 10, 10));
            AddDrawRoundedRectangle(Brushes.Black, new Pen(Brushes.White, 2), new Rect(0, 0, 10, 11), 5, 3);
            AddDrawGeometry(Brushes.Cyan, new Pen(Brushes.Red, 9), new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35));
            AddDrawGlyphRun(Brushes.Red, Const2D.glyphRun1);
            AddDrawText(Const2D.formattedText1, new Point(1.5, -1.5));
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5));
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure8()
        {
            Log("Testing DrawingGroupStructure8...");
            // this test case has Draw with Animations

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            AddDrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            AddDrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            AddDrawRoundedRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);
            AddDrawRoundedRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);
            AddDrawEllipse(Brushes.Red, new Pen(Brushes.Blue, 5), new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);
            AddDrawEllipse(Brushes.Red, new Pen(Brushes.Blue, 5), new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35), Const2D.rectAnimationClock);
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35), Const2D.rectAnimationClock);
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5), Const2D.rectAnimationClock);
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5), Const2D.rectAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure9()
        {
            Log("Testing DrawingGroupStructure9...");
            // This test case has Draw, Push with animations and Pop

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            AddDrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            AddDrawRoundedRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);
            AddDrawEllipse(Brushes.Red, new Pen(Brushes.Blue, 5), new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35), Const2D.rectAnimationClock);
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5), Const2D.rectAnimationClock);
            AddPushOpacity(0.5, Const2D.doubleAnimationClock);
            AddPushOpacity(0.5, Const2D.doubleAnimationClock);
            AddPushOpacity(0.5, Const2D.doubleAnimationClock);
            AddPop();
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            AddDrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            AddDrawRoundedRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);
            AddDrawEllipse(Brushes.Red, new Pen(Brushes.Blue, 5), new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35), Const2D.rectAnimationClock);
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5), Const2D.rectAnimationClock);
            AddPop();
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            AddDrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            AddDrawRoundedRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);
            AddDrawEllipse(Brushes.Red, new Pen(Brushes.Blue, 5), new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35), Const2D.rectAnimationClock);
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5), Const2D.rectAnimationClock);
            AddPop();
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            AddDrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            AddDrawRoundedRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);
            AddDrawEllipse(Brushes.Red, new Pen(Brushes.Blue, 5), new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35), Const2D.rectAnimationClock);
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5), Const2D.rectAnimationClock);
            AddPushOpacity(0.5, Const2D.doubleAnimationClock);
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            AddDrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            AddDrawRoundedRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);
            AddDrawEllipse(Brushes.Red, new Pen(Brushes.Blue, 5), new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), new Rect(9, 4, 59, 35), Const2D.rectAnimationClock);
            AddDrawVideo(Const2D.MediaPlayer, new Rect(1.5, -1.5, 1.5, 1.5), Const2D.rectAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure10()
        {
            Log("Testing DrawingGroupStructure10...");
            // this test case has Null in DrawLine

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawLine(null, new Point(10, 10), new Point(50, 50));
            AddDrawLine(null, new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure11()
        {
            Log("Testing DrawingGroupStructure11...");
            // this test case has Nill in DrawRectangle

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawRectangle(null, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10));
            AddDrawRectangle(null, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);

            AddDrawRectangle(Brushes.Red, null, new Rect(10, 10, 10, 10));
            AddDrawRectangle(Brushes.Red, null, new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);

            AddDrawRectangle(null, null, new Rect(10, 10, 10, 10));
            AddDrawRectangle(null, null, new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure12()
        {
            Log("Testing DrawingGroupStructure12...");
            // this test case has Null in DrawRoundedRectangle

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawRoundedRectangle(null, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), 5, 6);
            AddDrawRoundedRectangle(null, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);

            AddDrawRoundedRectangle(Brushes.Red, null, new Rect(10, 10, 10, 10), 5, 6);
            AddDrawRoundedRectangle(Brushes.Red, null, new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);

            AddDrawRoundedRectangle(null, null, new Rect(10, 10, 10, 10), 5, 6);
            AddDrawRoundedRectangle(null, null, new Rect(10, 10, 10, 10), Const2D.rectAnimationClock, 5, Const2D.doubleAnimationClock, 6, Const2D.doubleAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure13()
        {
            Log("Testing DrawingGroupStructure13...");
            // this test case has Null in DrawEllipse

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawEllipse(null, new Pen(Brushes.Blue, 5), new Point(10, 10), 5, 7);
            AddDrawEllipse(null, new Pen(Brushes.Blue, 5), new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);

            AddDrawEllipse(Brushes.Red, null, new Point(10, 10), 5, 7);
            AddDrawEllipse(Brushes.Red, null, new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);

            AddDrawEllipse(null, null, new Point(10, 10), 5, 7);
            AddDrawEllipse(null, null, new Point(10, 10), Const2D.pointAnimationClock, 5, Const2D.doubleAnimationClock, 7, Const2D.doubleAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure14()
        {
            Log("Testing DrawingGroupStructure14...");
            // this test case has Null in DrawGeometry

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawGeometry(null, new Pen(Brushes.Red, 9), new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddDrawGeometry(Brushes.Cyan, null, new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddDrawGeometry(Brushes.Cyan, new Pen(Brushes.Red, 9), null);
            AddDrawGeometry(null, null, new RectangleGeometry(new Rect(10, 10, 50, 50)));
            AddDrawGeometry(null, new Pen(Brushes.Red, 9), null);
            AddDrawGeometry(Brushes.Cyan, null, null);
            AddDrawGeometry(null, null, null);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure15()
        {
            Log("Testing DrawingGroupStructure15...");
            // this test case has Null in DrawImage

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawImage(null, new Rect(9, 4, 59, 35));
            AddDrawImage(null, new Rect(9, 4, 59, 35), Const2D.rectAnimationClock);

            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), Rect.Empty);
            AddDrawImage(new BitmapImage(new Uri("wood.bmp", UriKind.RelativeOrAbsolute)), Rect.Empty, Const2D.rectAnimationClock);

            AddDrawImage(null, Rect.Empty);
            AddDrawImage(null, Rect.Empty, Const2D.rectAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure16()
        {
            Log("Testing DrawingGroupStructure16...");
            // this test case has Null in DrawGlyphRun

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawGlyphRun(null, Const2D.glyphRun1);
            AddDrawGlyphRun(Brushes.Red, null);
            AddDrawGlyphRun(null, null);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure17()
        {
            Log("Testing DrawingGroupStructure17...");
            // this test case has Null in DrawVideo

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawVideo(null, new Rect(1.5, -1.5, 1.5, 1.5));
            AddDrawVideo(null, new Rect(1.5, -1.5, 1.5, 1.5), Const2D.rectAnimationClock);

            AddDrawVideo(Const2D.MediaPlayer, Rect.Empty);
            AddDrawVideo(Const2D.MediaPlayer, Rect.Empty, Const2D.rectAnimationClock);

            AddDrawVideo(null, Rect.Empty);
            AddDrawVideo(null, Rect.Empty, Const2D.rectAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure18()
        {
            Log("Testing DrawingGroupStructure18...");
            // this test case has Null in DrawDrawing

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawDrawing(null);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure19()
        {
            Log("Testing DrawingGroupStructure19...");
            // this test case has Null and IdentityTransform in PushTransform

            SetupDrawingVisual();

            // *** put test cases here
            AddPushTransform(null);
            AddPushTransform(Transform.Identity);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure20()
        {
            Log("Testing DrawingGroupStructure20...");

            SetupDrawingVisual();

            // *** put test cases here
            AddPushOpacity(0);
            AddPushOpacity(0, Const2D.doubleAnimationClock);

            AddPushOpacity(1);
            AddPushOpacity(1, Const2D.doubleAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure21()
        {
            Log("Testing DrawingGroupStructure21...");
            // this test case has Null in PushClip

            SetupDrawingVisual();

            // *** put test cases here
            AddPushClip(null);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure22()
        {
            Log("Testing DrawingGroupStructure22...");
            // this test case has Null in DrawText

            SetupDrawingVisual();

            // *** put test cases here
            AddDrawText(null, new Point(1.5, -1.5));
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure23()
        {
            Log("Testing DrawingGroupStructure23...");
            // this test case makes sure that calling Open clears the old content

            SetupDrawingGroup();

            // *** put test cases here
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            AddClose();
            AddOpen();
            AddDrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingGroup();
        }

        private void TestDrawingGroupStructure24()
        {
            Log("Testing DrawingGroupStructure24...");
            // this test case makes sure that calling Append appends to the old content

            SetupDrawingGroup();

            // *** put test cases here
            AddDrawLine(new Pen(Brushes.Red, 5), new Point(10, 10), Const2D.pointAnimationClock, new Point(50, 50), Const2D.pointAnimationClock);
            AddClose();
            AddAppend();
            AddDrawRectangle(Brushes.Red, new Pen(Brushes.Red, 5), new Rect(10, 10, 10, 10), Const2D.rectAnimationClock);
            // *** put test cases here

            FinishAndVerifyDrawingGroup();
        }

        private void TestDrawingGroupStructure25()
        {
            Log("Testing DrawingGroupStructure25...");
            // this test case has Null in PushGuidelineSet

            SetupDrawingVisual();

            // *** put test cases here
            AddPushGuidelineSet(null);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void TestDrawingGroupStructure26()
        {
            Log("Testing DrawingGroupStructure26...");

            SetupDrawingVisual();

            // *** put test cases here
            AddPushEffect(Const2D.bitmapEffect1, Const2D.bitmapEffectInput1);
            // *** put test cases here

            FinishAndVerifyDrawingVisual();
        }

        private void FinishAndVerifyDrawingVisual()
        {
            _drawingContext.Close();

            for (int i = 0; i < _needPop; i++)
            {
                _drawingContextCommands.Add(new PopCommand());
            }

            DrawingGroup readBack = VisualTreeHelper.GetDrawing(_drawingVisual);
            _verifier.VerifyDrawingGroup(readBack, _drawingContextCommands);
        }

        private void FinishAndVerifyDrawingGroup()
        {
            _drawingContext.Close();

            for (int i = 0; i < _needPop; i++)
            {
                _drawingContextCommands.Add(new PopCommand());
            }

            _verifier.VerifyDrawingGroup(_drawingGroup, _drawingContextCommands);
        }

        private void AddDrawDrawing(Drawing drawing)
        {
            _drawingContext.DrawDrawing(drawing);

            if (!ObjectUtils.Equals(drawing, null))
            {
                _drawingContextCommands.Add(new DrawDrawingCommand(drawing));
            }
        }

        private void AddDrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            _drawingContext.DrawEllipse(brush, pen, center, radiusX, radiusY);

            if (!ObjectUtils.Equals(brush, null) || !ObjectUtils.Equals(pen, null))
            {
                _drawingContextCommands.Add(new DrawEllipseCommand(brush, pen, center, radiusX, radiusY));
            }
        }

        private void AddDrawEllipse(Brush brush, Pen pen, Point center, AnimationClock centerAnimation, double radiusX, AnimationClock radiusXAnimation, double radiusY, AnimationClock radiusYAnimation)
        {
            _drawingContext.DrawEllipse(brush, pen, center, centerAnimation, radiusX, radiusXAnimation, radiusY, radiusYAnimation);

            if (!ObjectUtils.Equals(brush, null) || !ObjectUtils.Equals(pen, null))
            {
                _drawingContextCommands.Add(new DrawEllipseCommand(brush, pen, center, centerAnimation, radiusX, radiusXAnimation, radiusY, radiusYAnimation));
            }
        }

        private void AddDrawGeometry(Brush brush, Pen pen, Geometry geometry)
        {
            _drawingContext.DrawGeometry(brush, pen, geometry);

            if ((!ObjectUtils.Equals(brush, null) || !ObjectUtils.Equals(pen, null)) && !ObjectUtils.Equals(geometry, null))
            {
                _drawingContextCommands.Add(new DrawGeometryCommand(brush, pen, geometry));
            }
        }

        private void AddDrawGlyphRun(Brush foregroundBrush, GlyphRun glyphRun)
        {
            _drawingContext.DrawGlyphRun(foregroundBrush, glyphRun);

            if (!ObjectUtils.Equals(foregroundBrush, null) && !ObjectUtils.Equals(glyphRun, null))
            {
                _drawingContextCommands.Add(new DrawGlyphRunCommand(foregroundBrush, glyphRun));
            }
        }

        private void AddDrawImage(ImageSource imageSource, Rect rect)
        {
            _drawingContext.DrawImage(imageSource, rect);

            if (!ObjectUtils.Equals(imageSource, null))
            {
                _drawingContextCommands.Add(new DrawImageCommand(imageSource, rect));
            }
        }

        private void AddDrawImage(ImageSource imageSource, Rect rect, AnimationClock rectAnimation)
        {
            _drawingContext.DrawImage(imageSource, rect, rectAnimation);

            if (!ObjectUtils.Equals(imageSource, null))
            {
                _drawingContextCommands.Add(new DrawImageCommand(imageSource, rect, rectAnimation));
            }
        }

        private void AddDrawLine(Pen pen, Point point1, Point point2)
        {
            _drawingContext.DrawLine(pen, point1, point2);

            if (!ObjectUtils.Equals(pen, null))
            {
                _drawingContextCommands.Add(new DrawLineCommand(pen, point1, point2));
            }
        }

        private void AddDrawLine(Pen pen, Point point1, AnimationClock point1Animation, Point point2, AnimationClock point2Animation)
        {
            _drawingContext.DrawLine(pen, point1, point1Animation, point2, point2Animation);

            if (!ObjectUtils.Equals(pen, null))
            {
                _drawingContextCommands.Add(new DrawLineCommand(pen, point1, point1Animation, point2, point2Animation));
            }
        }

        private void AddDrawRectangle(Brush brush, Pen pen, Rect rect)
        {
            _drawingContext.DrawRectangle(brush, pen, rect);

            if (!ObjectUtils.Equals(brush, null) || !ObjectUtils.Equals(pen, null))
            {
                _drawingContextCommands.Add(new DrawRectangleCommand(brush, pen, rect));
            }
        }

        private void AddDrawRectangle(Brush brush, Pen pen, Rect rect, AnimationClock rectAnimation)
        {
            _drawingContext.DrawRectangle(brush, pen, rect, rectAnimation);

            if (!ObjectUtils.Equals(brush, null) || !ObjectUtils.Equals(pen, null))
            {
                _drawingContextCommands.Add(new DrawRectangleCommand(brush, pen, rect, rectAnimation));
            }
        }

        private void AddDrawRoundedRectangle(Brush brush, Pen pen, Rect rect, double radiusX, double radiusY)
        {
            _drawingContext.DrawRoundedRectangle(brush, pen, rect, radiusX, radiusY);

            if (!ObjectUtils.Equals(brush, null) || !ObjectUtils.Equals(pen, null))
            {
                _drawingContextCommands.Add(new DrawRoundedRectangleCommand(brush, pen, rect, radiusX, radiusY));
            }
        }

        private void AddDrawRoundedRectangle(Brush brush, Pen pen, Rect rect, AnimationClock rectAnimation, double radiusX, AnimationClock radiusXAnimation, double radiusY, AnimationClock radiusYAnimation)
        {
            _drawingContext.DrawRoundedRectangle(brush, pen, rect, rectAnimation, radiusX, radiusXAnimation, radiusY, radiusYAnimation);

            if (!ObjectUtils.Equals(brush, null) || !ObjectUtils.Equals(pen, null))
            {
                _drawingContextCommands.Add(new DrawRoundedRectangleCommand(brush, pen, rect, rectAnimation, radiusX, radiusXAnimation, radiusY, radiusYAnimation));
            }
        }

        private void AddDrawText(FormattedText formattedText, Point origin)
        {
            _drawingContext.DrawText(formattedText, origin);

            if (!ObjectUtils.Equals(formattedText, null))
            {
                DoubleCollection guidelinesY = new DoubleCollection();
                guidelinesY.Add(origin.Y + formattedText.Baseline);
                guidelinesY.Add(0);
                GuidelineSet guidelineSet = new GuidelineSet();
                guidelineSet.GuidelinesY = guidelinesY;

                // Since Text will freeze the GuidelineSet they create internally, in order to match it,
                // the GuidelineSet we create has to be frozen
                guidelineSet.Freeze();

                _drawingContextCommands.Add(new PushGuidelineSetCommand(guidelineSet));
                _drawingContextCommands.Add(new DrawTextCommand(formattedText, origin));
                _drawingContextCommands.Add(new PopCommand());
            }
        }

        private void AddDrawVideo(MediaPlayer mediaPlayer, Rect rect)
        {
            _drawingContext.DrawVideo(mediaPlayer, rect);

            if (!ObjectUtils.Equals(mediaPlayer, null))
            {
                _drawingContextCommands.Add(new DrawVideoCommand(mediaPlayer, rect));
            }
        }



        private void AddDrawVideo(MediaPlayer mediaPlayer, Rect rect, AnimationClock rectAnimation)
        {
            _drawingContext.DrawVideo(mediaPlayer, rect, rectAnimation);

            if (!ObjectUtils.Equals(mediaPlayer, null))
            {
                _drawingContextCommands.Add(new DrawVideoCommand(mediaPlayer, rect, rectAnimation));
            }
        }

        private void AddPop()
        {
            _needPop--;
            _drawingContext.Pop();
            _drawingContextCommands.Add(new PopCommand());
        }

        private void AddPushClip(Geometry clip)
        {
            _needPop++;
            _drawingContext.PushClip(clip);
            _drawingContextCommands.Add(new PushClipCommand(clip));
        }

        private void AddPushOpacity(double opacity)
        {
            _needPop++;
            _drawingContext.PushOpacity(opacity);
            _drawingContextCommands.Add(new PushOpacityCommand(opacity));
        }

        private void AddPushOpacity(double opacity, AnimationClock opacityAnimation)
        {
            _needPop++;
            _drawingContext.PushOpacity(opacity, opacityAnimation);
            _drawingContextCommands.Add(new PushOpacityCommand(opacity, opacityAnimation));
        }

        private void AddPushTransform(Transform transform)
        {
            _needPop++;
            _drawingContext.PushTransform(transform);
            _drawingContextCommands.Add(new PushTransformCommand(transform));
        }

        private void AddPushGuidelineSet(GuidelineSet guidelineSet)
        {
            _needPop++;
            _drawingContext.PushGuidelineSet(guidelineSet);
            _drawingContextCommands.Add(new PushGuidelineSetCommand(guidelineSet));
        }

        private void AddPushEffect(BitmapEffect effect, BitmapEffectInput effectInput)
        {
            _needPop++;
#pragma warning disable 0618
            _drawingContext.PushEffect(effect, effectInput);
#pragma warning restore 0618
            _drawingContextCommands.Add(new PushEffectCommand(effect, effectInput));
        }

        private void AddClose()
        {
            _drawingContext.Close();
        }

        private void AddOpen()
        {
            _drawingContext = _drawingGroup.Open();
            _drawingContextCommands.Clear();
        }

        private void AddAppend()
        {
            _drawingContext = _drawingGroup.Append();
        }

    }
}

