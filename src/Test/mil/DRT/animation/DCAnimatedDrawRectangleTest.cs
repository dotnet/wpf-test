// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DRTAnimation
{
    public class DCAnimatedDrawRectangleTest : FrameworkElement
    {
        private static Size s_size = new Size(60,60);

        private AnimationClock _rectAnim;
        private Pen _pen;

        public DCAnimatedDrawRectangleTest()
        {
            RectAnimation rectAnim = new RectAnimation(new Rect(10,10,15,15), new Rect(10,10,50,50), TimeSpan.FromSeconds(1));
            rectAnim.RepeatBehavior = RepeatBehavior.Forever;
            rectAnim.AutoReverse = true;

            _rectAnim = rectAnim.CreateClock();

            _pen = new Pen(Brushes.Red, 2.0);
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

            ctx.DrawRectangle(
                Brushes.Yellow,
                _pen,
                new Rect(10,10,40,40), _rectAnim);
        }
    }
}
