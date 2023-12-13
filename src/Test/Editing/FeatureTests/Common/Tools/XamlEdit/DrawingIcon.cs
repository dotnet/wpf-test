// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;

namespace XamlPadEdit
{
    public class DrawingIcon : UIElement
    {
        public static readonly DependencyProperty DrawingProperty =
                DependencyProperty.Register(
                        "Drawing",
                        typeof(Drawing),
                        typeof(DrawingIcon),
                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Drawing Drawing
        {
            get { return (Drawing)GetValue(DrawingProperty); }
            set { SetValue(DrawingProperty, value); }
        }

        protected override Size MeasureCore(Size availableSize)
        {
            Drawing drawing = Drawing;
            if (drawing != null)
            {
                Rect rect = drawing.Bounds;
                return new Size(Math.Min(availableSize.Width, rect.Width), Math.Min(availableSize.Height, rect.Height));
            }

            return base.MeasureCore(availableSize);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawDrawing(Drawing);
        }
    }
}
