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
    /// A custom effect that pass 8 (maximal number allowed) input properties to ps 3 shader
    /// </summary>
    public class MultiInputPS3Effect : MultiInputEffect
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public MultiInputPS3Effect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.ShaderRenderMode = ShaderRenderMode.HardwareOnly;
            pixelShader.UriSource = new Uri(@"pack://application:,,,/ShaderEffects;component/MultiTextureShader3.ps");

            this.PixelShader = pixelShader;
            UpdateShaderValue(Input4Property);
            UpdateShaderValue(Input5Property);
            UpdateShaderValue(Input6Property);
            UpdateShaderValue(Input7Property);
        }


        /// <summary/>
        public Brush Input4
        {
            get { return (Brush)GetValue(Input4Property); }
            set { SetValue(Input4Property, value); }
        }

        // Using a DependencyProperty as the backing store for InputProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Input4Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input4", typeof(MultiInputPS3Effect), 4);


        /// <summary/>
        public Brush Input5
        {
            get { return (Brush)GetValue(Input5Property); }
            set { SetValue(Input5Property, value); }
        }

        public static readonly DependencyProperty Input5Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input5", typeof(MultiInputPS3Effect), 5);



        /// <summary/>
        public Brush Input6
        {
            get { return (Brush)GetValue(Input6Property); }
            set { SetValue(Input6Property, value); }
        }

        public static readonly DependencyProperty Input6Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input6", typeof(MultiInputPS3Effect), 6);



        /// <summary/>
        public Brush Input7
        {
            get { return (Brush)GetValue(Input7Property); }
            set { SetValue(Input7Property, value); }
        }

        public static readonly DependencyProperty Input7Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input7", typeof(MultiInputPS3Effect), 7);
    }
}