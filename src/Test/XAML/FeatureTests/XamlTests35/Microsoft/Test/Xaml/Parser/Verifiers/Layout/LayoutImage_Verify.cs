// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization.CustomElements;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Layout
{
    /// <summary/>
    public static class LayoutImage_Verify
    {
        /// <summary>
        /// LayoutImage_Verify
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Inside LayoutXamlVerifiers.LayoutImageVerify()...");

            CustomCanvas myPanel = rootElement as CustomCanvas;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be CustomCanvas");
                result = false;
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 1)
            {
                GlobalLog.LogEvidence("myChildren.Count != 1");
                result = false;
            }

            Image myImage = myChildren[0] as Image;

            if (null == myImage)
            {
                GlobalLog.LogEvidence("Should be Image");
                result = false;
            }

            if ((int)(myImage.HorizontalAlignment) != (int)(HorizontalAlignment.Center))
            {
                GlobalLog.LogEvidence("(int)(myImage.HorizontalAlignment)!= (int)(System.Windows.HorizontalAlignment.Center)");
                result = false;
            }
            if ((int)(myImage.VerticalAlignment) != (int)(VerticalAlignment.Center))
            {
                GlobalLog.LogEvidence("(int)(myImage.VerticalAlignment)!= (int)(VerticalAlignment.Center)");
                result = false;
            }
            if ((int)(myImage.Stretch) != (int)(Stretch.None))
            {
                GlobalLog.LogEvidence("(int)(myImage.Stretch)!= (int)(Stretch.None)");
                result = false;
            }
            if (((FrameworkElement) myImage).Height != 400)
            {
                GlobalLog.LogEvidence("((FrameworkElement)myImage).Height!= 400");
                result = false;
            }
            if (((FrameworkElement) myImage).Width != 400)
            {
                GlobalLog.LogEvidence("((FrameworkElement)myImage).Width!= 400");
                result = false;
            }
            BitmapImage imgsrc = new BitmapImage(new Uri("avalon.png", UriKind.RelativeOrAbsolute));
            if (imgsrc.PixelHeight != ((BitmapSource) myImage.Source).PixelHeight)
            {
                GlobalLog.LogEvidence("imgsrc.PixelHeight!= ((BitmapSource)myImage.Source).PixelHeight");
                result = false;
            }
            if (imgsrc.PixelWidth != ((BitmapSource) myImage.Source).PixelWidth)
            {
                GlobalLog.LogEvidence("imgsrc.PixelWidth != ((BitmapSource)myImage.Source).PixelWidth");
                result = false;
            }
            return result;
        }
    }
}
