// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Negative tests for Effect : contains a list of negative Effects used by negative tests. 
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace Microsoft.Test.Effects
{
    /// <summary />
    public class ShaderEffectOverRegisterLimit : MultiParameterShaderEffect
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ShaderEffectOverRegisterLimit()
        {
            UpdateShaderValue(OverRegisterLimitProperty);
        }


        public double OverRegisterLimit
        {
            get { return (double)GetValue(OverRegisterLimitProperty); }
            set { SetValue(OverRegisterLimitProperty, value); }
        }

        public static readonly DependencyProperty OverRegisterLimitProperty =
             DependencyProperty.Register("OverRegisterLimit", typeof(double), typeof(ShaderEffectOverRegisterLimit),
             new UIPropertyMetadata(1.0, PixelShaderConstantCallback(80)));

    }

    /// <summary />
    public class ShaderEffectWithIllegalConstantType : MultiParameterShaderEffect
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ShaderEffectWithIllegalConstantType()
        {
            UpdateShaderValue(IllegalShaderConstantTypeValueProperty);
        }


        public int IllegalShaderConstantTypeValue
        {
            get { return (int)GetValue(IllegalShaderConstantTypeValueProperty); }
            set { SetValue(IllegalShaderConstantTypeValueProperty, value); }
        }

        public static readonly DependencyProperty IllegalShaderConstantTypeValueProperty =
             DependencyProperty.Register("IllegalShaderConstantTypeValue", typeof(int), typeof(ShaderEffectWithIllegalConstantType),
             new UIPropertyMetadata(1, PixelShaderConstantCallback(0)));
    }

    /// <summary />
    public class EffectSendingInputThroughIllegalRegister : DoubleInputEffect
    {
        public void UpdateInputProperty(DependencyProperty dp)
        {
            UpdateShaderValue(dp);
        }

        public static readonly DependencyProperty Input_2Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input_2", typeof(EffectSendingInputThroughIllegalRegister), -2, SamplingMode.Bilinear);

        public static readonly DependencyProperty Input16Property =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input16", typeof(EffectSendingInputThroughIllegalRegister), 16, SamplingMode.Bilinear);
    }
}