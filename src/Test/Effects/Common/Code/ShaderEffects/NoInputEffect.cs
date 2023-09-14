// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/************************************************* 
 * Purpose: Test shader effect without input
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using System.Windows.Media.Effects;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// A custom effect without any texture property 
    /// </summary>
    public class NoInputEffect : ShaderEffect
    {
        public NoInputEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"pack://application:,,,/ShaderEffects;component/MultiTextureShader.ps");
            this.PixelShader = pixelShader;
        }
    }
}