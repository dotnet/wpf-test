// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Apply the cache to a media element.
    /// </summary>
    class MediaElementContent : ChangeableContent
    {

        #region Public methods

        public override FrameworkElement Construct(Requirements req)
        {
            _me = new MediaElement();
            _me.LoadedBehavior = MediaState.Manual;
            _me.Source = new Uri(@"red2SecondsGreen30.wmv", UriKind.Relative);
            cache = CreateCache(renderAtScale);
            if (!req.cacheDisabled) { _me.CacheMode = cache; }

            if (req.bitmapCacheBrush)
            {
                VisualBrush vb = new VisualBrush(_me);
                Button innerButton = new Button();
                innerButton.Background = vb;
                innerButton.Height = 100;
                innerButton.Width = 100;
                Button OuterButton = WrapUIElementInBCB(innerButton, req.cacheOnBitmapCacheBrush);
                return OuterButton;
            }

            return _me;
        }

        public override TestResult Display()
        {           
            //first, wait for the code-coverage slowed pipeline to open the video.
            _synch = false;
            _me.MediaOpened += new RoutedEventHandler(me_MediaOpened);
            _me.Play();
            while (!_synch)
            {
                DispatcherHelper.DoEvents(100);//wait in 0.1 second increments to avoid spinlocking too hard
            }
            return TestResult.Pass;
        }

        public override TestResult ChangeColor()
        {
            //now that the video's started, let it play until it gets into the green section   
            DispatcherHelper.DoEvents(6000);
            return TestResult.Pass;
        }

        private void me_MediaOpened(object sender, RoutedEventArgs e)
        {
            _synch = true;
        }

        //not sure what an animation on a video could be. 
        //so, this will be a no-op.
        public override TestResult ChangeAnimation()
        {
            return TestResult.Unknown;
        }

        #endregion

        #region members

        private MediaElement _me;
        private bool _synch;

        #endregion

    }
}
