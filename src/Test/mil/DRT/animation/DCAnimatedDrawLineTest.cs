// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DRTAnimation
{
    public class DCAnimatedDrawLineTest : FrameworkElement
    {
        private static Size s_size = new Size(60,60);

        private AnimationClock _point0Anim;
        private AnimationClock _point1Anim;
        private Pen _pen;

        public DCAnimatedDrawLineTest()
        {
            PointAnimation point0Anim = new PointAnimation(new Point(10,10), new Point(10,50), TimeSpan.FromSeconds(1));
            point0Anim.RepeatBehavior = RepeatBehavior.Forever;
            point0Anim.AutoReverse = true;

            PointAnimation point1Anim = new PointAnimation(new Point(50,50), new Point(50,10), TimeSpan.FromSeconds(1));
            point1Anim.RepeatBehavior = RepeatBehavior.Forever;
            point1Anim.AutoReverse = true;

            _point0Anim = point0Anim.CreateClock();
            _point1Anim = point1Anim.CreateClock();

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

            ctx.DrawLine(
                _pen,
                new Point(10,10), _point0Anim,
                new Point(50,50), _point1Anim);
        }
    }
}
