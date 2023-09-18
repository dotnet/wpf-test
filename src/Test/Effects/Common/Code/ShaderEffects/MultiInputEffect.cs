// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test shader effect with multiple inputs.
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// A custom effect that pass 4 (maximal number allowed) input properties to shader
    /// </summary>
    public class MultiInputEffect : DoubleInputEffect
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MultiInputEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"pack://application:,,,/ShaderEffects;component/MultiTextureShader.ps");

            this.PixelShader = pixelShader;
            UpdateShaderValue(Input2Property);
            UpdateShaderValue(Input3Property);
        }

        /// <summary>
        /// The third input 
        /// </summary>
        public Brush Input2
        {
            get { return (Brush)GetValue(Input2Property); }
            set { SetValue(Input2Property, value); }
        }

        // Using a DependencyProperty as the backing store for InputProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Input2Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input2", typeof(MultiInputEffect), 2);


        /// <summary/>
        public Brush Input3
        {
            get { return (Brush)GetValue(Input3Property); }
            set { SetValue(Input3Property, value); }
        }

        public static readonly DependencyProperty Input3Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input3", typeof(MultiInputEffect), 3);
    }
}