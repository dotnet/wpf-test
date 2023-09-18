// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Test ImageBrush and related dependencies on Imaging.
    /// </summary>
    public class ImagePartnerTest : DiffuseMaterialTest
    {
        /// <summary/>
        public override Visual GetWindowContent()
        {
            Brush brush2D = ((DiffuseMaterial)((GeometryModel3D)parameters.Model).Material).Brush;
            if (brush2D is ImageBrush)
            {
                // we want a unified DPI setting
                ImageBrush imageBrush2d = brush2D as ImageBrush;
                TextureGenerator.ForceDpiOnBrush(ref imageBrush2d);
                brush2D = imageBrush2d;
            }

            if (isControlRender)
            {
                // render a simple 2D rectangle with the given brush
                Rectangle r = new Rectangle(new Rect(0, 0, WindowWidth, WindowHeight), brush2D);
                return r.Visual;
            }
            else
            {
                // render the brush to an image, then use that ImageBrush on a 2D rectangle
                BitmapSource id = TextureGenerator.RenderBrushToImageData(brush2D, WindowWidth, WindowHeight);
                ImageBrush ib = new ImageBrush(id);
                Rectangle r = new Rectangle(new Rect(0, 0, WindowWidth, WindowHeight), ib);
                return r.Visual;
            }
        }
    }
}