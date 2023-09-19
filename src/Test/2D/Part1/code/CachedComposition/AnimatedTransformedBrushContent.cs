// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Provide a transformed brush context for the cached brush.
    /// </summary>

    class AnimatedTransformedBrushContent : ChangeableContent
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
            DisplayButton = new Button();
            DisplayButton.Background = B;
            DisplayButton.Height = 100;
            DisplayButton.Width = 100;

            //this is the button that will have the BitmapCache on it
            cache = CreateCache(renderAtScale);
            if (!req.cacheDisabled) { DisplayButton.CacheMode = cache; }



            //use the created button in a visual brush
            _vb = new VisualBrush();
            _vb.Visual = DisplayButton;
            _rt = new RotateTransform();
            _vb.Transform = _rt;

            //paint a button with that visual brush
            Button OuterButton = new Button();
            OuterButton.Background = _vb;
            OuterButton.Height = 100;
            OuterButton.Width = 100;

            if (req.bitmapCacheBrush)
            {
                OuterButton = WrapUIElementInBCB(OuterButton, req.cacheOnBitmapCacheBrush);
            }
            //add this button to a stackpanel
            StackPanel s = new StackPanel();
            s.Children.Add(OuterButton);
            return s;
        }

        protected override void CreateAnimation()
        {
            _da = new DoubleAnimation(-45, 45, TimeSpan.FromMilliseconds(500));
            _da.RepeatBehavior = RepeatBehavior.Forever;
            _da.AutoReverse = true;
        }

        public override TestResult ChangeAnimation()
        {
            _rt.BeginAnimation(RotateTransform.AngleProperty, _da);

            DispatcherHelper.DoEvents(1000);//let it animate
            return TestResult.Pass;
        }

        public override TestResult ChangeColor()
        {
            B.Color = Colors.Green;
            return TestResult.Pass;
        }

        private DoubleAnimation _da;
        private RotateTransform _rt;
        private VisualBrush _vb;

        #endregion
    }
}
