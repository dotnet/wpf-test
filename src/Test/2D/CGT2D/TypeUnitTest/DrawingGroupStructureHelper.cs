// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Windows.Media.Animation;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class DrawDrawingCommand
    {
        /// <summary/>
        public Drawing drawing;

        /// <summary/>
        public DrawDrawingCommand(Drawing drawing)
        {
            this.drawing = drawing;
        }
    }

    /// <summary/>
    public class DrawEllipseCommand
    {
        /// <summary/>
        public Brush brush;
        /// <summary/>
        public Pen pen;
        /// <summary/>
        public Point center;
        /// <summary/>
        public AnimationClock centerAnimation;
        /// <summary/>
        public double radiusX;
        /// <summary/>
        public AnimationClock radiusXAnimation;
        /// <summary/>
        public double radiusY;
        /// <summary/>
        public AnimationClock radiusYAnimation;

        /// <summary/>
        public DrawEllipseCommand(Brush brush, Pen pen, Point center, double radiusX, double radiusY) : this(brush, pen, center, null, radiusX, null, radiusY, null) { }
        /// <summary/>
        public DrawEllipseCommand(Brush brush, Pen pen, Point center, AnimationClock centerAnimation, double radiusX, AnimationClock radiusXAnimation, double radiusY, AnimationClock radiusYAnimation)
        {
            this.brush = brush;
            this.pen = pen;
            this.center = center;
            this.centerAnimation = centerAnimation;
            this.radiusX = radiusX;
            this.radiusXAnimation = radiusXAnimation;
            this.radiusY = radiusY;
            this.radiusYAnimation = radiusYAnimation;
        }
    }

    /// <summary/>
    public class DrawGeometryCommand
    {
        /// <summary/>
        public Brush brush;
        /// <summary/>
        public Pen pen;
        /// <summary/>
        public Geometry geometry;

        /// <summary/>
        public DrawGeometryCommand(Brush brush, Pen pen, Geometry geometry)
        {
            this.brush = brush;
            this.pen = pen;
            this.geometry = geometry;
        }
    }

    /// <summary/>
    public class DrawGlyphRunCommand
    {
        /// <summary/>
        public Brush foregroundBrush;
        /// <summary/>
        public GlyphRun glyphRun;

        /// <summary/>
        public DrawGlyphRunCommand(Brush foregroundBrush, GlyphRun glyphRun)
        {
            this.foregroundBrush = foregroundBrush;
            this.glyphRun = glyphRun;
        }
    }

    /// <summary/>
    public class DrawImageCommand
    {
        /// <summary/>
        public ImageSource imageSource;
        /// <summary/>
        public Rect rect;
        /// <summary/>
        public AnimationClock rectAnimation;

        /// <summary/>
        public DrawImageCommand(ImageSource imageSource, Rect rect) : this(imageSource, rect, null) { }
        /// <summary/>
        public DrawImageCommand(ImageSource imageSource, Rect rect, AnimationClock rectAnimation)
        {
            this.imageSource = imageSource;
            this.rect = rect;
            this.rectAnimation = rectAnimation;
        }
    }

    /// <summary/>
    public class DrawLineCommand
    {
        /// <summary/>
        public Pen pen;
        /// <summary/>
        public Point point1;
        /// <summary/>
        public AnimationClock point1Animation;
        /// <summary/>
        public Point point2;
        /// <summary/>
        public AnimationClock point2Animation;

        /// <summary/>
        public DrawLineCommand(Pen pen, Point point1, Point point2) : this(pen, point1, null, point2, null) { }
        /// <summary/>
        public DrawLineCommand(Pen pen, Point point1, AnimationClock point1Animation, Point point2, AnimationClock point2Animation)
        {
            this.pen = pen;
            this.point1 = point1;
            this.point1Animation = point1Animation;
            this.point2 = point2;
            this.point2Animation = point2Animation;
        }
    }

    /// <summary/>
    public class DrawRectangleCommand
    {
        /// <summary/>
        public Brush brush;
        /// <summary/>
        public Pen pen;
        /// <summary/>
        public Rect rect;
        /// <summary/>
        public AnimationClock rectAnimation;

        /// <summary/>
        public DrawRectangleCommand(Brush brush, Pen pen, Rect rect) : this(brush, pen, rect, null) { }
        /// <summary/>
        public DrawRectangleCommand(Brush brush, Pen pen, Rect rect, AnimationClock rectAnimation)
        {
            this.brush = brush;
            this.pen = pen;
            this.rect = rect;
            this.rectAnimation = rectAnimation;
        }
    }

    /// <summary/>
    public class DrawRoundedRectangleCommand
    {
        /// <summary/>
        public Brush brush;
        /// <summary/>
        public Pen pen;
        /// <summary/>
        public Rect rect;
        /// <summary/>
        public AnimationClock rectAnimation;
        /// <summary/>
        public double radiusX;
        /// <summary/>
        public AnimationClock radiusXAnimation;
        /// <summary/>
        public double radiusY;
        /// <summary/>
        public AnimationClock radiusYAnimation;

        /// <summary/>
        public DrawRoundedRectangleCommand(Brush brush, Pen pen, Rect rect, double radiusX, double radiusY) : this(brush, pen, rect, null, radiusX, null, radiusY, null) { }
        /// <summary/>
        public DrawRoundedRectangleCommand(Brush brush, Pen pen, Rect rect, AnimationClock rectAnimation, double radiusX, AnimationClock radiusXAnimation, double radiusY, AnimationClock radiusYAnimation)
        {
            this.brush = brush;
            this.pen = pen;
            this.rect = rect;
            this.rectAnimation = rectAnimation;
            this.radiusX = radiusX;
            this.radiusXAnimation = radiusXAnimation;
            this.radiusY = radiusY;
            this.radiusYAnimation = radiusYAnimation;
        }
    }

    /// <summary/>
    public class DrawTextCommand
    {
        /// <summary/>
        public FormattedText formattedText;
        /// <summary/>
        public Point origin;

        /// <summary/>
        public DrawTextCommand(FormattedText formattedText, Point origin)
        {
            this.formattedText = formattedText;
            this.origin = origin;
        }
    }

    /// <summary/>
    public class DrawVideoCommand
    {
        /// <summary/>
        public MediaPlayer mediaPlayer;
        /// <summary/>
        public Rect rect;
        /// <summary/>
        public AnimationClock rectAnimation;

        /// <summary/>
        public DrawVideoCommand(MediaPlayer mediaPlayer, Rect rect) : this(mediaPlayer, rect, null) { }
        /// <summary/>
        public DrawVideoCommand(MediaPlayer mediaPlayer, Rect rect, AnimationClock rectAnimation)
        {
            this.mediaPlayer = mediaPlayer;
            this.rect = rect;
            this.rectAnimation = rectAnimation;
        }
    }

    /// <summary/>
    public class PushClipCommand
    {
        /// <summary/>
        public Geometry clip;

        /// <summary/>
        public PushClipCommand(Geometry clip)
        {
            this.clip = clip;
        }
    }

    /// <summary/>
    public class PushOpacityCommand
    {
        /// <summary/>
        public double opacity;
        /// <summary/>
        public AnimationClock opacityAnimation;

        /// <summary/>
        public PushOpacityCommand(double opacity) : this(opacity, null) { }
        /// <summary/>
        public PushOpacityCommand(double opacity, AnimationClock opacityAnimation)
        {
            this.opacity = opacity;
            this.opacityAnimation = opacityAnimation;
        }
    }

    /// <summary/>
    public class PushTransformCommand
    {
        /// <summary/>
        public Transform transform;

        /// <summary/>
        public PushTransformCommand(Transform transform)
        {
            this.transform = transform;
        }
    }

    /// <summary/>
    public class PushGuidelineSetCommand
    {
        /// <summary/>
        public GuidelineSet guidelineSet;

        /// <summary/>
        public PushGuidelineSetCommand(GuidelineSet guidelineSet)
        {
            this.guidelineSet = guidelineSet;
        }
    }

    /// <summary/>
    public class PushEffectCommand
    {
        /// <summary/>
        public BitmapEffect effect;
        /// <summary/>
        public BitmapEffectInput effectInput;

        /// <summary/>
        public PushEffectCommand(BitmapEffect effect, BitmapEffectInput effectInput)
        {
            this.effect = effect;
            this.effectInput = effectInput;
        }
    }

    /// <summary/>
    public class PopCommand { }

    /// <summary/>
    public class DrawingGroupStructureVerifier
    {
        /// <summary/>
        private CoreGraphicsTest _test;

        /// <summary/>
        public DrawingGroupStructureVerifier(CoreGraphicsTest test)
        {
            this._test = test;
        }

        /// <summary/>
        public void Log(string fmt, params object[] args)
        {
            _test.Log(fmt, args);
        }

        /// <summary/>
        public void AddFailure(string fmt, params object[] args)
        {
            _test.AddFailure(fmt, args);
        }

        /// <summary/>
        public void VerifyDrawingGroup(DrawingGroup drawingGroup, ArrayList commands)
        {
            if (MathEx.Equals(commands.Count, 0))
            {
                if (drawingGroup != null)
                {
                    AddFailure("DrawingGroup Structure Test failed");
                    Log("***Expected: {0}", null);
                    Log("***Actual: {0}", drawingGroup);
                }
                return;
            }
            else if (drawingGroup.Opacity != 1 || drawingGroup.Transform != null || drawingGroup.ClipGeometry != null)
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***Expected: Opacity = {0}, Transform = {1}, ClipGeometry = {2}", 1, null, null);
                Log("***Actual: Opacity = {0}, Transform = {1}, ClipGeometry = {2}", drawingGroup.Opacity, drawingGroup.Transform, drawingGroup.ClipGeometry);
            }
            VerifyDrawingCollection(drawingGroup.Children, commands);

            if (!MathEx.Equals(commands.Count, 0))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***There is no more Drawing in the structure, but there is {0} commands left", commands.Count);
            }
        }

        /// <summary/>
        public void VerifyDrawingCollection(DrawingCollection drawingCollection, ArrayList commands)
        {
            foreach (Drawing d in drawingCollection)
            {
                if (d is GeometryDrawing)
                {
                    GeometryDrawingMatch(d as GeometryDrawing, commands[0]);
                    commands.RemoveAt(0);
                }
                else if (d is ImageDrawing)
                {
                    ImageDrawingMatch(d as ImageDrawing, commands[0]);
                    commands.RemoveAt(0);
                }
                else if (d is GlyphRunDrawing)
                {
                    GlyphRunDrawingMatch(d as GlyphRunDrawing, commands[0]);
                    commands.RemoveAt(0);
                }
                else if (d is VideoDrawing)
                {
                    VideoDrawingMatch(d as VideoDrawing, commands[0]);
                    commands.RemoveAt(0);
                }
                else if (d is DrawingGroup)
                {
                    DrawingGroupMatch(d as DrawingGroup, commands);
                }
                else // unknown DrawingType
                {
                    AddFailure("DrawingGroup Structure Test failed");
                    Log("***Unknown Drawing type");
                }
            }
        }

        private void VideoDrawingMatch(VideoDrawing videoDrawing, object command)
        {
            if (command is DrawDrawingCommand)
            {
                Match(videoDrawing, command as DrawDrawingCommand);
            }
            else if (command is DrawVideoCommand)
            {
                Match(videoDrawing, command as DrawVideoCommand);
            }
            else
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***Expected: DrawVideo || DrawDrawing");
            }
        }

        private void DrawingGroupMatch(DrawingGroup drawingGroup, ArrayList commands)
        {
            if (commands[0] is DrawDrawingCommand)
            {
                Match(drawingGroup, commands[0] as DrawDrawingCommand);
                commands.RemoveAt(0);
            }
            else if (commands[0] is PushOpacityCommand)
            {
                Match(drawingGroup, commands[0] as PushOpacityCommand);
                commands.RemoveAt(0);
                VerifyDrawingCollection(drawingGroup.Children, commands);
                HasPopCommand(commands[0]);
                commands.RemoveAt(0);
            }
            else if (commands[0] is PushTransformCommand)
            {
                Match(drawingGroup, commands[0] as PushTransformCommand);
                commands.RemoveAt(0);
                VerifyDrawingCollection(drawingGroup.Children, commands);
                HasPopCommand(commands[0]);
                commands.RemoveAt(0);
            }
            else if (commands[0] is PushClipCommand)
            {
                Match(drawingGroup, commands[0] as PushClipCommand);
                commands.RemoveAt(0);
                VerifyDrawingCollection(drawingGroup.Children, commands);
                HasPopCommand(commands[0]);
                commands.RemoveAt(0);
            }
            else if (commands[0] is PushGuidelineSetCommand)
            {
                Match(drawingGroup, commands[0] as PushGuidelineSetCommand);
                commands.RemoveAt(0);
                VerifyDrawingCollection(drawingGroup.Children, commands);
                HasPopCommand(commands[0]);
                commands.RemoveAt(0);
            }
            else if (commands[0] is PushEffectCommand)
            {
                Match(drawingGroup, commands[0] as PushEffectCommand);
                commands.RemoveAt(0);
                VerifyDrawingCollection(drawingGroup.Children, commands);
                HasPopCommand(commands[0]);
                commands.RemoveAt(0);
            }
            else
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***Expected: DrawDrawing || PushOpacity || PushTransform || PushClip || PushGuidelineSet");
            }
        }

        private void HasPopCommand(object command)
        {
            if (!(command is PopCommand))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***Expected: Pop()");
            }
        }

        private void GeometryDrawingMatch(GeometryDrawing geometryDrawing, object command)
        {
            if (command is DrawDrawingCommand)
            {
                Match(geometryDrawing, command as DrawDrawingCommand);
            }
            else if (command is DrawEllipseCommand)
            {
                Match(geometryDrawing, command as DrawEllipseCommand);
            }
            else if (command is DrawGeometryCommand)
            {
                Match(geometryDrawing, command as DrawGeometryCommand);
            }
            else if (command is DrawLineCommand)
            {
                Match(geometryDrawing, command as DrawLineCommand);
            }
            else if (command is DrawRectangleCommand)
            {
                Match(geometryDrawing, command as DrawRectangleCommand);
            }
            else if (command is DrawRoundedRectangleCommand)
            {
                Match(geometryDrawing, command as DrawRoundedRectangleCommand);
            }
            else
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***Expected: DrawLine || DrawEllipse || DrawRectangle || DrawRoundedRectangle || DrawDrawing");
            }
        }

        private void ImageDrawingMatch(ImageDrawing imageDrawing, object command)
        {
            if (command is DrawDrawingCommand)
            {
                Match(imageDrawing, command as DrawDrawingCommand);
            }
            else if (command is DrawImageCommand)
            {
                Match(imageDrawing, command as DrawImageCommand);
            }
            else
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***Expected: DrawImage || DrawDrawing");
            }
        }

        private void GlyphRunDrawingMatch(GlyphRunDrawing glyphRunDrawing, object command)
        {
            if (command is DrawDrawingCommand)
            {
                Match(glyphRunDrawing, command as DrawDrawingCommand);
            }
            else if (command is DrawGlyphRunCommand)
            {
                Match(glyphRunDrawing, command as DrawGlyphRunCommand);
            }
            else if (command is DrawTextCommand)
            {
                Match(glyphRunDrawing, command as DrawTextCommand);
            }
            else
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***Expected: DrawText || DrawGlyphRun || DrawDrawing");
            }
        }

        private void Match(GeometryDrawing geometryDrawing, DrawLineCommand command)
        {
            LineGeometry lineGeometry = geometryDrawing.Geometry as LineGeometry;

            if (ObjectUtils.Equals(lineGeometry, null) ||
                !ObjectUtils.Equals(geometryDrawing.Brush, null) ||
                !ObjectUtils.Equals(geometryDrawing.Pen, command.pen) ||
                !ObjectUtils.Equals(lineGeometry.StartPoint, command.point1) ||
                !ObjectUtils.Equals(lineGeometry.EndPoint, command.point2))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***DrawLine doesn't match");
            }
        }

        private void Match(GeometryDrawing geometryDrawing, DrawGeometryCommand command)
        {
            if (!ObjectUtils.Equals(geometryDrawing.Brush, command.brush) ||
                !ObjectUtils.Equals(geometryDrawing.Pen, command.pen) ||
                !ObjectUtils.Equals(geometryDrawing.Geometry, command.geometry))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***DrawGeometry doesn't match ");
            }
        }

        private void Match(GeometryDrawing geometryDrawing, DrawRectangleCommand command)
        {
            RectangleGeometry rectangleGeometry = geometryDrawing.Geometry as RectangleGeometry;

            if (ObjectUtils.Equals(rectangleGeometry, null) ||
                !ObjectUtils.Equals(geometryDrawing.Brush, command.brush) ||
                !ObjectUtils.Equals(geometryDrawing.Pen, command.pen) ||
                !ObjectUtils.Equals(rectangleGeometry.Rect, command.rect))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***DrawRectangle doesn't match ");
            }
        }

        private void Match(GeometryDrawing geometryDrawing, DrawRoundedRectangleCommand command)
        {
            RectangleGeometry rectangleGeometry = geometryDrawing.Geometry as RectangleGeometry;

            if (ObjectUtils.Equals(rectangleGeometry, null) ||
                !ObjectUtils.Equals(geometryDrawing.Brush, command.brush) ||
                !ObjectUtils.Equals(geometryDrawing.Pen, command.pen) ||
                !ObjectUtils.Equals(rectangleGeometry.Rect, command.rect) ||
                !ObjectUtils.Equals(rectangleGeometry.RadiusX, command.radiusX) ||
                !ObjectUtils.Equals(rectangleGeometry.RadiusY, command.radiusY))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***DrawRoundedRectangle doesn't match");
            }
        }

        private void Match(GeometryDrawing geometryDrawing, DrawEllipseCommand command)
        {
            EllipseGeometry ellipseGeometry = geometryDrawing.Geometry as EllipseGeometry;

            if (ObjectUtils.Equals(ellipseGeometry, null) ||
                !ObjectUtils.Equals(geometryDrawing.Brush, command.brush) ||
                !ObjectUtils.Equals(geometryDrawing.Pen, command.pen) ||
                !ObjectUtils.Equals(ellipseGeometry.Center, command.center) ||
                !ObjectUtils.Equals(ellipseGeometry.RadiusX, command.radiusX) ||
                !ObjectUtils.Equals(ellipseGeometry.RadiusY, command.radiusY))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***DrawEllipse doesn't match");
            }
        }

        private void Match(ImageDrawing imageDrawing, DrawImageCommand command)
        {
            if (!ObjectUtils.Equals(imageDrawing.ImageSource, command.imageSource) ||
                !ObjectUtils.Equals(imageDrawing.Rect, command.rect))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***DrawImage doesn't match");
            }
        }

        private void Match(GlyphRunDrawing glyphRunDrawing, DrawGlyphRunCommand command)
        {
            if (!ObjectUtils.Equals(glyphRunDrawing.ForegroundBrush, command.foregroundBrush) ||
                !ObjectUtils.Equals(glyphRunDrawing.GlyphRun, command.glyphRun))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***DrawGlyphRun doesn't match");
            }
        }

        private void Match(GlyphRunDrawing glyphRunDrawing, DrawTextCommand command)
        {
            // we don't verify the GlyphRunDrawing content
            return;
        }

        private void Match(VideoDrawing videoDrawing, DrawVideoCommand command)
        {
            if (!ObjectUtils.Equals(videoDrawing.Player, command.mediaPlayer) ||
                !ObjectUtils.Equals(videoDrawing.Rect, command.rect))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***DrawVideo doesn't match");
            }
        }

        private void Match(Drawing drawing, DrawDrawingCommand command)
        {
            if (!ObjectUtils.Equals(drawing, command.drawing))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***DrawDrawing doesn't match");
            }
        }

        private void Match(DrawingGroup drawingGroup, PushTransformCommand command)
        {
            if (!ObjectUtils.Equals(drawingGroup.Transform, command.transform) ||
                !ObjectUtils.Equals(drawingGroup.Opacity, 1.0) ||
                !ObjectUtils.Equals(drawingGroup.ClipGeometry, null) ||
                !ObjectUtils.Equals(drawingGroup.GuidelineSet, null))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***PushTransform doesn't match");
            }
        }

        private void Match(DrawingGroup drawingGroup, PushOpacityCommand command)
        {
            if (!ObjectUtils.Equals(drawingGroup.Opacity, command.opacity) ||
                !ObjectUtils.Equals(drawingGroup.Transform, null) ||
                !ObjectUtils.Equals(drawingGroup.ClipGeometry, null) ||
                !ObjectUtils.Equals(drawingGroup.GuidelineSet, null))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***PushOpacity doesn't match");
            }
        }

        private void Match(DrawingGroup drawingGroup, PushClipCommand command)
        {
            if (!ObjectUtils.Equals(drawingGroup.ClipGeometry, command.clip) ||
                !ObjectUtils.Equals(drawingGroup.Transform, null) ||
                !ObjectUtils.Equals(drawingGroup.Opacity, 1.0) ||
                !ObjectUtils.Equals(drawingGroup.GuidelineSet, null))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***PushClip doesn't match");
            }
        }

        private void Match(DrawingGroup drawingGroup, PushGuidelineSetCommand command)
        {
            if (!ObjectUtils.DeepEquals(drawingGroup.GuidelineSet, command.guidelineSet) ||
                !ObjectUtils.Equals(drawingGroup.Transform, null) ||
                !ObjectUtils.Equals(drawingGroup.ClipGeometry, null) ||
                !ObjectUtils.Equals(drawingGroup.Opacity, 1.0))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***PushGuidelineSet doesn't match");
            }
        }

        private void Match(DrawingGroup drawingGroup, PushEffectCommand command)
        {
            if (!ObjectUtils.DeepEquals(drawingGroup.BitmapEffect, command.effect) ||
                !ObjectUtils.DeepEquals(drawingGroup.BitmapEffectInput, command.effectInput) ||
                !ObjectUtils.Equals(drawingGroup.Transform, null) ||
                !ObjectUtils.Equals(drawingGroup.ClipGeometry, null) ||
                !ObjectUtils.Equals(drawingGroup.Opacity, 1.0))
            {
                AddFailure("DrawingGroup Structure Test failed");
                Log("***PushEffect doesn't match");
            }
        }
    }
}
