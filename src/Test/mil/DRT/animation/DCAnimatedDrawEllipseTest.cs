// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DRTAnimation
{
    public class DCAnimatedDrawEllipseTest : FrameworkElement
    {
        private static Size s_size = new Size(240,60);

        private AnimationClock _doubleAnim;
        private AnimationClock _pointAnim;
        private AnimationClock _pointAnim2;
        private Pen _pen;

        public DCAnimatedDrawEllipseTest()
        {
            DoubleAnimation doubleAnim = new DoubleAnimation(0, 20, TimeSpan.FromSeconds(1));
            doubleAnim.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnim.AutoReverse = true;

            PointAnimation pointAnim = new PointAnimation(new Point(10,10), new Point(50,50), TimeSpan.FromSeconds(1));
            pointAnim.RepeatBehavior = RepeatBehavior.Forever;
            pointAnim.AutoReverse = true;

            PointAnimation pointAnim2 = new PointAnimation(new Point(210,10), new Point(250,50), TimeSpan.FromSeconds(1));
            pointAnim2.RepeatBehavior = RepeatBehavior.Forever;
            pointAnim2.AutoReverse = true;

            _doubleAnim = doubleAnim.CreateClock();
            _pointAnim = pointAnim.CreateClock();
            _pointAnim2 = pointAnim2.CreateClock();

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

            ctx.DrawEllipse(
                Brushes.Yellow,
                _pen,
                new Point(30,30), _pointAnim,
                20, null,
                20, null);

            ctx.DrawEllipse(
                Brushes.Azure,
                _pen,
                new Point(90,30), null,
                20, _doubleAnim,
                20, null);

            ctx.DrawEllipse(
                Brushes.GreenYellow,
                _pen,
                new Point(150,30), null,
                20, null,
                20, _doubleAnim);

            ctx.DrawEllipse(
                Brushes.Coral,
                _pen,
                new Point(210, 30), _pointAnim2,
                20, _doubleAnim,
                20, _doubleAnim);
        }
    }
}
