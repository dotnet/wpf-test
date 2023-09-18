// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// This is the content which will change in some way - we'll have various different types of visible content.
    /// Not every change type will make sense with every content type.
    /// </summary>
    public abstract class ChangeableContent
    {
        /// <summary>
        /// Change the color of the main visible portion of the test content from red to green, 
        /// to provoke cache regeneration.
        /// </summary>
        /// <returns>TestResult indicating if there were any problems.</returns
        public abstract TestResult ChangeColor();

        /// <summary>
        /// Go through all necessary building to make this test content and wrap it in a UIElement that can 
        /// be set as the Content of a window. This content should be ready to render.
        /// </summary>
        /// <param name="req">Requirements struct, specifies any special costruction parameters.</param>
        /// <returns>a FrameworkElement, such as a StackPanel</returns>
        public abstract FrameworkElement Construct(Requirements req);

        /// <summary>
        /// Display the content. Provoke a render.
        /// </summary>
        /// <returns></returns>
        public virtual TestResult Display() // render the content
        {
            return TestResult.Pass;
        }

        /// <summary>
        /// Trigger an animation on an element of the content that will result in the 
        /// visual output changing from red to green, and invalidate the cache.
        /// </summary>
        /// <returns>TestResult indicating if there were any problems.</returns>
        public virtual TestResult ChangeAnimation()
        {
            B.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            return TestResult.Pass;
        }

        /// <summary>
        /// Change the cleartype setting on the cache - this may or may not provoke regeneration.
        /// </summary>
        public virtual void ChangeClearType()
        {
            if (cache is BitmapCache)
            {
                cache.EnableClearType = !cache.EnableClearType;
            }

        }
        /// <summary>
        /// Use the button as a visual on a bitmapCacheBrush and 
        /// use that brush on a button, and return that. 
        /// </summary>
        /// <param name="b">The button to wrap up in a BCB and a Button</param>
        /// <param name="enableCache">whether to enable the cache in the BCB</param>
        /// <returns></returns>
        public virtual Button WrapUIElementInBCB(UIElement e, bool enableCache)
        {
            Button b = new Button();
            BitmapCacheBrush bcb = new BitmapCacheBrush(e); 
            if (enableCache)
            {
                    bcb.BitmapCache = new BitmapCache(1);
            }
            b.Background = bcb;
            b.BeginInit();
            b.Height = 100;
            b.Width = 100;
            b.EndInit();
            return b;
        }

        /// <summary>
        /// Change the RenderAtScale property of the cache, to provoke texture reallocation and rendering.
        /// </summary>
        public void ChangeRenderAtScale()
        {
            if (cache is BitmapCache)
            {
                cache.RenderAtScale = renderAtScale + 0.1f;
            }
        }

        /// <summary>
        /// Pass in a binding that will attach to the visible element.
        /// </summary>
        /// <param name="bind"></param>
        public void BindColor(Binding bind)
        {
            DisplayButton.SetBinding(Button.BackgroundProperty, bind);//accept the binding given.
        }

        public double RenderAtScale
        {
            set
            {
                // if the cache exists, set it directly
                if (cache is BitmapCache)
                {
                    cache.RenderAtScale = value;
                }
                //otherwise, save the intended value to be used when we create the cache
                renderAtScale = value;
            }
        }

        public Button DisplayButton { get { return _displayButton; } set { _displayButton = value; } }
        public SolidColorBrush B { get { return b; } set { b = value; } }//the property that we change in our tests is the color of this brush
        public SolidColorBrush b;
        public static readonly int NormalWidth = 100;
        public static readonly int NormalHeight = 100;
        public static readonly int MultiMonWidth = 3000;

        protected BitmapCache CreateCache(double renderScale)
        {
            BitmapCache myCache = new BitmapCache();
            myCache.RenderAtScale = renderScale;
            myCache.EnableClearType = false;
            return myCache;
        }

        protected virtual void CreateAnimation()
        {
            ca = new ColorAnimation();
            ca.From = Colors.Red;
            ca.To = Colors.Green;
            ca.Duration = TimeSpan.FromMilliseconds(0);
            ca.RepeatBehavior = RepeatBehavior.Forever;
            ca.AutoReverse = false;
        }

        protected ColorAnimation ca;

        protected BitmapCache cache;
        protected double renderAtScale;
        private Button _displayButton;
    }

    /// <summary>
    /// Detects, through various means, whether the cache was regenerated.
    /// </summary> 
    abstract class ChangeDetector
    {
        public abstract TestResult DetectBefore(Window w);
        public abstract TestResult DetectAfter();
        public abstract bool VerifyChanges(Requirements r, TestLog log);
    }

    /// <summary>
    /// Changes our content - different types of changes will be done by derived classes.
    /// </summary>
    public abstract class Changer
    {
        public abstract TestResult Change();
        public void SetContent(ChangeableContent content) { this.content = content; }
        protected ChangeableContent content;
    }

    /// <summary>
    /// How large/small to scale our content through the RenderScale property on the BitmapCache
    /// </summary>
    public enum RenderScale { Neg, Zero, OnePixelElement, One, Normal, FillMemory, TooLarge }

    /// <summary>
    /// What size of UIElement to pipe through the BitMapCache
    /// </summary>
    public enum UIElementSize { Zero, Thin, OnePixel, Normal, FullScreen, MaxHwTexSize, OverMaxHwTexSize, MaxSoftTexSize }

    /// <summary>
    /// How we should invalidate the cache.
    /// </summary>
    public enum InvalidatingProperty { Animation, Code, DataBinding, Layout, Theme, RenderScale, EnableClearType }

    /// <summary>
    /// What rendering context the cache should be in.
    /// </summary>
    public enum Context { TileBrush, VP2DV3D, TransformedBrush, AnimatedTransformedBrush, OpacityClippedOutput, LayeredWindows, RenderTargetBitmap, MultipleMonitor, MediaElement }

    /// <summary>
    /// What type of change detection we need to use
    /// </summary>
    public enum Detector { Visual, ETW }

    /// <summary>
    /// The concretization of our test vectors for the factory. This is how the test tells the factory what it needs.
    /// </summary>
    public struct Requirements
    {
        public RenderScale renderScale;
        public UIElementSize uIElementSize;
        public InvalidatingProperty invalidatingProperty;
        public bool snapsToDevicePixels;
        public Context context;
        public Detector detectorType;
        public bool successExpected;
        public int cacheRegensExpected;
        public float screenUpdateAreaExpected;

        // The next three are for BitmapCacheBrush testing
        public bool cacheDisabled; // disabling/enabling the cache on the content inside a BCB which has its own cache is an important case
        public bool bitmapCacheBrush; // we'll wrap the content in a BCB 
        public bool cacheOnBitmapCacheBrush; // the BCB behaves differently when it does or doesnt have a cache on it
    }

}
