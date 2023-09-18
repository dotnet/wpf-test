// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Apply the cache to a RTB.
    /// </summary>
    class RenderTargetBitmapContent : ChangeableContent
    {

        #region Public methods

        public override FrameworkElement Construct(Requirements req)
        {
            //create the animation so that we can change it later if requested
            CreateAnimation();

            //create and save a brush so that we can change it later to invalidate the cache
            B = new SolidColorBrush();
            B.Color = Colors.Red;

            //use the created brush inside a button
            _innerButton = new Button();
            _innerButton.BeginInit();
            _innerButton.Background = B;
            _innerButton.Height = NormalHeight;
            _innerButton.Width = NormalWidth;
            _innerButton.EndInit();
            _innerButton.Measure(new Size(NormalWidth, NormalHeight));
            _innerButton.Arrange(new Rect(new Size(NormalWidth, NormalHeight)));
            _innerButton.UpdateLayout();

            //this is the button that will have the BitmapCache on it
            cache = CreateCache(renderAtScale);
            if (!req.cacheDisabled) { _innerButton.CacheMode = cache; }

            if (req.bitmapCacheBrush)
            {
                _innerButton = WrapUIElementInBCB(_innerButton, req.cacheOnBitmapCacheBrush);
            }

            _RTB = new RenderTargetBitmap(
                NormalHeight,
                NormalWidth,
                Microsoft.Test.Display.Monitor.Dpi.x,
                Microsoft.Test.Display.Monitor.Dpi.x,
                System.Windows.Media.PixelFormats.Default);
            _RTB.Render(_innerButton);
            DispatcherHelper.DoEvents(System.Windows.Threading.DispatcherPriority.Render);//push the first render.
            _ib = new ImageBrush(_RTB);
            DisplayButton = new Button();
            DisplayButton.BeginInit();
            DisplayButton.Width = NormalWidth;
            DisplayButton.Height = NormalHeight;
            DisplayButton.EndInit();
            _innerButton.Measure(new Size(NormalWidth, NormalHeight));
            _innerButton.Arrange(new Rect(new Size(NormalWidth, NormalHeight)));
            _innerButton.UpdateLayout();
            DispatcherHelper.DoEvents(System.Windows.Threading.DispatcherPriority.Render);//push the first render.
            DisplayButton.Background = _ib;
            _s = new StackPanel();
            _s.Children.Add(DisplayButton);
            return _s;
        }

        public override TestResult Display() { return TestResult.Pass; }

        public override TestResult ChangeColor()
        {
            B.Color = Colors.Green;
            DispatcherHelper.DoEvents(100);
            _RTB = new RenderTargetBitmap(
                 NormalHeight,
                 NormalWidth,
                 Microsoft.Test.Display.Monitor.Dpi.x,
                 Microsoft.Test.Display.Monitor.Dpi.x,
                 System.Windows.Media.PixelFormats.Default);
            _RTB.Render(_innerButton);
            DispatcherHelper.DoEvents(System.Windows.Threading.DispatcherPriority.Render);//push the first render.
            _ib = new ImageBrush(_RTB);
            DisplayButton.Background = _ib;
            _s.Children.Clear();//we may not need to do these two steps
            _s.Children.Add(DisplayButton);
            return TestResult.Pass;
        }

        public override TestResult ChangeAnimation()
        {
            B.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            DispatcherHelper.DoEvents(100);
            _RTB = new RenderTargetBitmap(
                 NormalHeight,
                 NormalWidth,
                 Microsoft.Test.Display.Monitor.Dpi.x,
                 Microsoft.Test.Display.Monitor.Dpi.x,
                 System.Windows.Media.PixelFormats.Default);
            _RTB.Render(_innerButton);
            DispatcherHelper.DoEvents(System.Windows.Threading.DispatcherPriority.Render);//push the first render.
            ImageBrush ib = new ImageBrush(_RTB);
            ib = new ImageBrush(_RTB);
            DisplayButton.Background = ib;
            _s.Children.Clear();//we may not need to do these two steps
            _s.Children.Add(DisplayButton);
            return TestResult.Pass;
        }

        public override void ChangeClearType()
        {
            if (cache is BitmapCache)
            {
                cache.EnableClearType = !cache.EnableClearType;
            }
        }

        #endregion

        #region members

        ImageBrush _ib;
        StackPanel _s;
        Button _innerButton;
        RenderTargetBitmap _RTB;

        #endregion
    }
}
