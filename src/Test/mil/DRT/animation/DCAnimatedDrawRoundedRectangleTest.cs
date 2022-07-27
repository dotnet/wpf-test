// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DRTAnimation
{
    public class DCAnimatedDrawRoundedRectangleTest : FrameworkElement
    {
        private static Size s_size = new Size(240,60);

        private AnimationClock _doubleAnim;
        private AnimationClock _rectAnim;
        private AnimationClock _rectAnim2;
        private Pen _pen;

        public DCAnimatedDrawRoundedRectangleTest()
        {
            RectAnimation rectAnim = new RectAnimation(new Rect(10,10,15,15), new Rect(10,10,40,40), TimeSpan.FromSeconds(1));
            rectAnim.RepeatBehavior = RepeatBehavior.Forever;
            rectAnim.AutoReverse = true;

            RectAnimation rectAnim2 = new RectAnimation(new Rect(190,10,15,15), new Rect(190,10,40,40), TimeSpan.FromSeconds(1));
            rectAnim2.RepeatBehavior = RepeatBehavior.Forever;
            rectAnim2.AutoReverse = true;

            DoubleAnimation doubleAnim = new DoubleAnimation(20, 5, TimeSpan.FromSeconds(1));
            doubleAnim.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnim.AutoReverse = true;

            _doubleAnim = doubleAnim.CreateClock();
            _rectAnim = rectAnim.CreateClock();
            _rectAnim2 = rectAnim2.CreateClock();

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

            ctx.DrawRoundedRectangle(
                Brushes.Yellow,
                _pen,
                new Rect(10,10,40,40), _rectAnim,
                10, null,
                10, null);

            ctx.DrawRoundedRectangle(
                Brushes.Salmon,
                _pen,
                new Rect(70, 10, 40, 40), null,
                10, _doubleAnim,
                10, null);

            ctx.DrawRoundedRectangle(
                Brushes.PeachPuff,
                _pen,
                new Rect(130, 10, 40, 40), null,
                10, null,
                10, _doubleAnim);

            ctx.DrawRoundedRectangle(
                Brushes.Orchid,
                _pen,
                new Rect(190, 10, 40, 40), _rectAnim2,
                10, _doubleAnim,
                10, _doubleAnim);
        }
    }
}
