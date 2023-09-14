// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Basic ShaderEffect takes no parameter, but 
 * exposes PixelShader so that it cannot assigned in xaml.
 * Owner: Microsoft 
 ********************************************************************/
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// a class inherits ShaderEffect and exposes 
    /// PixelShader property.
    /// </summary>
    public class BasicShaderEffect : ShaderEffect
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public BasicShaderEffect()
        {
            UpdateShaderValue(InputProperty);
        }

        /// <summary>
        /// PixelShader property, make the same property in 
        /// ShaderEffect public
        /// </summary>
        public new PixelShader PixelShader
        {
            get
            {
                return base.PixelShader;
            }
            set
            {
                base.PixelShader = value;
            }
        }

        /// <summary>
        /// Propety to specity input Textture. Not specifying a value results in using default input. 
        /// </summary>
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(BasicShaderEffect), 0);


        /// <summary>
        ///     Shadows inherited Clone() with a strongly typed
        ///     version for convenience.
        /// </summary>
        public new BasicShaderEffect Clone()
        {
            return (BasicShaderEffect)base.Clone();
        }

        /// <summary>
        ///     Shadows inherited CloneCurrentValue() with a strongly typed
        ///     version for convenience.
        /// </summary>
        public new BasicShaderEffect CloneCurrentValue()
        {
            return (BasicShaderEffect)base.CloneCurrentValue();
        }

    }
}

