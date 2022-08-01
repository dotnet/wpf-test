// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Markup;

namespace DRTAnimation
{
    public class DCAnimatedDrawDrawingTest : FrameworkElement
    {
        private static Size s_size = new Size(100,100);
        private DrawingGroup _drawing;
        private AnimationClock _pointAnim;

        public DCAnimatedDrawDrawingTest()
        {
            PointAnimation pointAnim = new PointAnimation(new Point(10,10), new Point(50,50), TimeSpan.FromSeconds(1));
            pointAnim.RepeatBehavior = RepeatBehavior.Forever;
            pointAnim.AutoReverse = true;

            _pointAnim = pointAnim.CreateClock();

            _drawing = new DrawingGroup();

            DrawingContext ctx = _drawing.Open();
            ctx.DrawLine(
                new Pen(Brushes.Red, 2.0),
                new Point(10,10), null,
                new Point(50,50), _pointAnim);
            ctx.Close();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return s_size;
        }

        protected override void OnRender(DrawingContext ctx)
        {
            ctx.DrawRectangle(
                null,
                DRTAnimation.BorderPen,
                new Rect(5, 5, s_size.Width - 5, s_size.Height - 5));

            ctx.DrawDrawing(
                _drawing);
        }
    }
}
