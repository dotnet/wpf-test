// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Test.Graphics
{
    public class SolidGreenHardwareOnlyEffect : ShaderEffect
    {
        public SolidGreenHardwareOnlyEffect()
        {
            PixelShader = s_pixelShader;
            PixelShader.ShaderRenderMode = ShaderRenderMode.HardwareOnly;
        }

        private static PixelShader s_pixelShader =
             new PixelShader() { UriSource = new Uri("pack://application:,,,/2D_Dev10;component/Effects/solidGreen.ps") };
    }
}
