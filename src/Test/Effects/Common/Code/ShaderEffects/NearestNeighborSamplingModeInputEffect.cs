// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/************************************************* 
 * Purpose: Test shader effect with Input0 of NearestNeighbor SamplingMode
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Effect with two texture input. Shader equally split the whole image into 3 rows. 
    /// The first row would be from Input0, which is of NearestNeighbor ScalingMode. The third row uses default input, and the second row is mixed. 
    /// </summary>
    public class NearestNeighborSamplingModeInputEffect : DoubleInputEffect
    {
        public override Brush Input0
        {
            get { return (Brush)GetValue(AlternativeInputProperty); }
            set { SetValue(AlternativeInputProperty, value); }
        }

        public static readonly DependencyProperty AlternativeInputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("AlternativeInput", typeof(NearestNeighborSamplingModeInputEffect), 0, SamplingMode.NearestNeighbor);
    }
}