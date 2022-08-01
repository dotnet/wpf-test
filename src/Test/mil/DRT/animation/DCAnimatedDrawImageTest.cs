// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace DRTAnimation
{
    public class DCAnimatedDrawImageTest : FrameworkElement
    {
        private static Size s_size = new Size(200,100);

        private BitmapSource _bitmapImage;
        private AnimationClock _rectAnim;

        public DCAnimatedDrawImageTest()
        {
            // useEmbeddedProfile = true removed, as constructor no longer exists.
            // "default" is now false.
            _bitmapImage = BitmapFrame.Create(new FileStream(@"DrtFiles\DrtAnimation\angels.jpg", FileMode.Open, FileAccess.Read), BitmapCreateOptions.None, BitmapCacheOption.Default);

            RectAnimation rectAnim = new RectAnimation(new Rect(10,10,15,15), new Rect(10,10,180,80), TimeSpan.FromSeconds(1));
            rectAnim.RepeatBehavior = RepeatBehavior.Forever;
            rectAnim.AutoReverse = true;

            _rectAnim = rectAnim.CreateClock();
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

            ctx.DrawImage(
                _bitmapImage,
                new Rect(10,10,180,80), _rectAnim);
        }
    }
}


