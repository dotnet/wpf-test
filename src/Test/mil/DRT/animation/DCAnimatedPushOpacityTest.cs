// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DRTAnimation
{
    public class DCAnimatedPushOpacityTest : FrameworkElement
    {
        private static Size s_size = new Size(60,60);

        private DoubleAnimation _opacityAnim;

        public DCAnimatedPushOpacityTest()
        {
            _opacityAnim = new DoubleAnimation(0.0, 1.0, new TimeSpan(0, 0, 0, 0, 1000));
            _opacityAnim.AutoReverse = true;
            _opacityAnim.RepeatBehavior = RepeatBehavior.Forever;
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

            ctx.PushOpacity(
                0.5,
                _opacityAnim.CreateClock());

            ctx.DrawRectangle(
                Brushes.Yellow,
                DRTAnimation.TestPen,
                new Rect(10,10,40,40));

            ctx.Pop();
        }
    }
}
