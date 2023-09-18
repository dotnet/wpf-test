// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/************************************************* 
 * Purpose: Test shader effect with two input property. 
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
    /// The first row would be from FirstInput, The third row uses SecondInput, and the second row is mixed. 
    /// </summary>
    public class DoubleInputEffect : ShaderEffectWithPadding
    {
        /// <summary/>
        public DoubleInputEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"pack://application:,,,/ShaderEffects;component/DoubleTextureShader.ps");
            this.PixelShader = pixelShader;

            UpdateShaderValue(Input0Property);
            UpdateShaderValue(Input1Property);
        }

        /// <summary>
        /// First input
        /// </summary>
        public virtual Brush Input0
        {
            get { return (Brush)GetValue(Input0Property); }
            set { SetValue(Input0Property, value); }
        }

        public static readonly DependencyProperty Input0Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input0", typeof(DoubleInputEffect), 0);

        /// <summary>
        /// Second input
        /// </summary>
        public Brush Input1
        {
            get { return (Brush)GetValue(Input1Property); }
            set { SetValue(Input1Property, value); }
        }

        public static readonly DependencyProperty Input1Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input1", typeof(DoubleInputEffect), 1);
    }
}
