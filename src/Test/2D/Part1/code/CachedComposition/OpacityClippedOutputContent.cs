// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Apply an opacity mask to the cached brush.
    /// </summary>
    class OpacityClippedOutputContent : ChangeableContent
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
            DisplayButton.OpacityMask = new LinearGradientBrush(Colors.Red, Colors.Transparent, new Point(0, 0), new Point(1, 1));

            //this is the button that will have the BitmapCache on it
            cache = CreateCache(renderAtScale);
            if (!req.cacheDisabled) { DisplayButton.CacheMode = cache; }

            Button OuterButton = DisplayButton;
            
            if (req.bitmapCacheBrush)
            {
                OuterButton = WrapUIElementInBCB(OuterButton, req.cacheOnBitmapCacheBrush);
            }

            //add this button to a stackpanel
            StackPanel s = new StackPanel();
            s.Children.Add(OuterButton);

            return s;
        }

        public override TestResult Display() { return TestResult.Pass; }

        public override TestResult ChangeColor()
        {
            B.Color = Colors.Green;
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
    }
}
