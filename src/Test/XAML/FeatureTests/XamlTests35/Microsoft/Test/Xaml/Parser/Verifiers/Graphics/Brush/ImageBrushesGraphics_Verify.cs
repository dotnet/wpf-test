// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Serialization.CustomElements;
using Microsoft.Test.Logging;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Graphics.Brush
{
    /// <summary/>
    public static class ImageBrushesGraphics_Verify
    {
        /// <summary>
        /// Verify ImageBrushesGraphics.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool         result  = true;
            CustomCanvas myPanel = rootElement as CustomCanvas;
            GlobalLog.LogStatus("Verifying image Brush ...");
            FrameworkElement fe         = myPanel as FrameworkElement;
            Rectangle        rectangle1 = fe.FindName("Rectangle1") as Rectangle;
            ImageBrush       iBrush     = rectangle1.Fill as ImageBrush;
            if (null == iBrush)
            {
                GlobalLog.LogEvidence("null == iBrush");
                result = false;
            }
            ImageSource iSource = iBrush.ImageSource;
            if (iSource.ToString() != "pack://siteoforigin:,,,/puppies.jpg")
            {
                GlobalLog.LogEvidence("iSource.ToString() != pack://siteoforigin:,,,/puppies.jpg");
                result = false;
            }
            return result;
        }
    }
}
