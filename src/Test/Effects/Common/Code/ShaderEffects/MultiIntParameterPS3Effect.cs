// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test shader effect with multiple int parameters PS 3.0 shader.
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// A custom effect with multiple int properties passing through registers. 
    /// </summary>
    public class MultiIntParameterPS3Effect : ShaderEffect
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MultiIntParameterPS3Effect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("pack://application:,,,/ShaderEffects;component/MultiIntParameterShader3.ps");
            pixelShader.ShaderRenderMode = ShaderRenderMode.HardwareOnly;

            this.PixelShader = pixelShader;

            // Update properties
            UpdateShaderValue(Int0Property);
            UpdateShaderValue(Int1Property);
            UpdateShaderValue(Int2Property);
            UpdateShaderValue(Int3Property);
            UpdateShaderValue(Int4Property);
            UpdateShaderValue(Int5Property);
            UpdateShaderValue(Int6Property);
            UpdateShaderValue(Int7Property);
            UpdateShaderValue(Int8Property);
            UpdateShaderValue(Int9Property);
            UpdateShaderValue(Int10Property);
            UpdateShaderValue(Int11Property);
            UpdateShaderValue(Int12Property);
            UpdateShaderValue(Int13Property);
            UpdateShaderValue(Int14Property);
            UpdateShaderValue(Int15Property); 
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


#region int properties
        public static readonly DependencyProperty Int0Property = DependencyProperty.Register(
            "Int0", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(0)));

        public int Int0
        {
            get { return (int)GetValue(Int0Property); }
            set { SetValue(Int0Property, value); }
        }

        public static readonly DependencyProperty Int1Property = DependencyProperty.Register(
            "Int1", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(1)));

        public int Int1
        {
            get { return (int)GetValue(Int1Property); }
            set { SetValue(Int1Property, value); }
        }

        public static readonly DependencyProperty Int2Property = DependencyProperty.Register(
            "Int2", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(2)));

        public int Int2
        {
            get { return (int)GetValue(Int2Property); }
            set { SetValue(Int2Property, value); }
        }

        public static readonly DependencyProperty Int3Property = DependencyProperty.Register(
             "Int3", typeof(int), typeof(MultiIntParameterPS3Effect),
             new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(3)));

        public int Int3
        {
            get { return (int)GetValue(Int3Property); }
            set { SetValue(Int3Property, value); }
        }

        public static readonly DependencyProperty Int4Property = DependencyProperty.Register(
            "Int4", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(4)));

        public int Int4
        {
            get { return (int)GetValue(Int4Property); }
            set { SetValue(Int4Property, value); }
        }

        public static readonly DependencyProperty Int5Property = DependencyProperty.Register(
            "Int5", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(5)));

        public int Int5
        {
            get { return (int)GetValue(Int5Property); }
            set { SetValue(Int5Property, value); }
        }

        public static readonly DependencyProperty Int6Property = DependencyProperty.Register(
            "Int6", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(6)));

        public int Int6
        {
            get { return (int)GetValue(Int6Property); }
            set { SetValue(Int6Property, value); }
        }

        public static readonly DependencyProperty Int7Property = DependencyProperty.Register(
            "Int7", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(7)));

        public int Int7
        {
            get { return (int)GetValue(Int7Property); }
            set { SetValue(Int7Property, value); }
        }

        public static readonly DependencyProperty Int8Property = DependencyProperty.Register(
            "Int8", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(8)));

        public int Int8
        {
            get { return (int)GetValue(Int8Property); }
            set { SetValue(Int8Property, value); }
        }

        public static readonly DependencyProperty Int9Property = DependencyProperty.Register(
            "Int9", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(9)));

        public int Int9
        {
            get { return (int)GetValue(Int9Property); }
            set { SetValue(Int9Property, value); }
        }

        public static readonly DependencyProperty Int10Property = DependencyProperty.Register(
            "Int10", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(10)));

        public int Int10
        {
            get { return (int)GetValue(Int10Property); }
            set { SetValue(Int10Property, value); }
        }
        public static readonly DependencyProperty Int11Property = DependencyProperty.Register(
            "Int11", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(11)));

        public int Int11
        {
            get { return (int)GetValue(Int11Property); }
            set { SetValue(Int11Property, value); }
        }
        public static readonly DependencyProperty Int12Property = DependencyProperty.Register(
            "Int12", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(12)));

        public int Int12
        {
            get { return (int)GetValue(Int12Property); }
            set { SetValue(Int12Property, value); }
        }

        public static readonly DependencyProperty Int13Property = DependencyProperty.Register(
            "Int13", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(13)));

        public int Int13
        {
            get { return (int)GetValue(Int13Property); }
            set { SetValue(Int13Property, value); }
        }

        public static readonly DependencyProperty Int14Property = DependencyProperty.Register(
            "Int14", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(14)));

        public int Int14
        {
            get { return (int)GetValue(Int14Property); }
            set { SetValue(Int14Property, value); }
        }

        public static readonly DependencyProperty Int15Property = DependencyProperty.Register(
            "Int15", typeof(int), typeof(MultiIntParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(15)));

        public int Int15
        {
            get { return (int)GetValue(Int15Property); }
            set { SetValue(Int15Property, value); }
        }
#endregion
    }
}