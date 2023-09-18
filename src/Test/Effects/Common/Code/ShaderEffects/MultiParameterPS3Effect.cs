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
    public class MultiParameterPS3Effect : MultiParameterShaderEffect
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MultiParameterPS3Effect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("pack://application:,,,/ShaderEffects;component/MultiParameterShader3.ps");
            pixelShader.ShaderRenderMode = ShaderRenderMode.HardwareOnly;

            this.PixelShader = pixelShader;

            //bool
            UpdateShaderValue(Bool0Property);
            UpdateShaderValue(Bool1Property);
            UpdateShaderValue(Bool2Property);
            UpdateShaderValue(Bool3Property);
            UpdateShaderValue(Bool4Property);
            UpdateShaderValue(Bool5Property);
            UpdateShaderValue(Bool6Property);
            UpdateShaderValue(Bool7Property);
            UpdateShaderValue(Bool8Property);
            UpdateShaderValue(Bool9Property);
            UpdateShaderValue(Bool10Property);
            UpdateShaderValue(Bool11Property);
            UpdateShaderValue(Bool12Property);
            UpdateShaderValue(Bool13Property);
            UpdateShaderValue(Bool14Property);
            UpdateShaderValue(Bool15Property); 

            //int
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
            //double
            UpdateShaderValue(Double10Property);
            UpdateShaderValue(Double50Property);
            UpdateShaderValue(Double125Property);
            UpdateShaderValue(Double180Property);
            UpdateShaderValue(Double200Property);
            UpdateShaderValue(Double220Property);
            UpdateShaderValue(Double223Property);
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

#region double properties

        /// <summary>
        /// Property of type double. 
        /// </summary>
        public double Double10
        {
            get { return (double)GetValue(Double10Property); }
            set { SetValue(Double10Property, value); }
        }

        public static readonly DependencyProperty Double10Property =
             DependencyProperty.Register("Double10", typeof(double), typeof(MultiParameterPS3Effect),
             new UIPropertyMetadata(10000.0, PixelShaderConstantCallback(10)));

        /// <summary>
        /// Property of type double. 
        /// </summary>
        public double Double50
        {
            get { return (double)GetValue(Double50Property); }
            set { SetValue(Double50Property, value); }
        }

        public static readonly DependencyProperty Double50Property =
             DependencyProperty.Register("Double50", typeof(double), typeof(MultiParameterPS3Effect),
             new UIPropertyMetadata(50000.0, PixelShaderConstantCallback(50)));

        public double Double125
        {
            get { return (double)GetValue(Double125Property); }
            set { SetValue(Double125Property, value); }
        }

        public static readonly DependencyProperty Double125Property =
             DependencyProperty.Register("Double125", typeof(double), typeof(MultiParameterPS3Effect),
             new UIPropertyMetadata(125000.0, PixelShaderConstantCallback(125)));


        public double Double180
        {
            get { return (double)GetValue(Double180Property); }
            set { SetValue(Double180Property, value); }
        }

        public static readonly DependencyProperty Double180Property =
             DependencyProperty.Register("Double180", typeof(double), typeof(MultiParameterPS3Effect),
             new UIPropertyMetadata(180000.0, PixelShaderConstantCallback(180)));

        public double Double200
        {
            get { return (double)GetValue(Double200Property); }
            set { SetValue(Double200Property, value); }
        }

        public static readonly DependencyProperty Double200Property =
             DependencyProperty.Register("Double200", typeof(double), typeof(MultiParameterPS3Effect),
             new UIPropertyMetadata(100000.0, PixelShaderConstantCallback(200)));

        public double Double220
        {
            get { return (double)GetValue(Double220Property); }
            set { SetValue(Double220Property, value); }
        }

        public static readonly DependencyProperty Double220Property =
             DependencyProperty.Register("Double220", typeof(double), typeof(MultiParameterPS3Effect),
             new UIPropertyMetadata(22000.0, PixelShaderConstantCallback(220)));

        public double Double223
        {
            get { return (double)GetValue(Double223Property); }
            set { SetValue(Double223Property, value); }
        }

        public static readonly DependencyProperty Double223Property =
             DependencyProperty.Register("Double223", typeof(double), typeof(MultiParameterPS3Effect),
             new UIPropertyMetadata(20.0, PixelShaderConstantCallback(223)));

#endregion

#region bool properties
        public static readonly DependencyProperty Bool0Property = DependencyProperty.Register(
            "Bool0", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(0)));

        public bool Bool0
        {
            get { return (bool)GetValue(Bool0Property); }
            set { SetValue(Bool0Property, value); }
        }

        public static readonly DependencyProperty Bool1Property = DependencyProperty.Register(
            "Bool1", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(1)));

        public bool Bool1
        {
            get { return (bool)GetValue(Bool1Property); }
            set { SetValue(Bool1Property, value); }
        }

        public static readonly DependencyProperty Bool2Property = DependencyProperty.Register(
            "Bool2", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(2)));

        public bool Bool2
        {
            get { return (bool)GetValue(Bool2Property); }
            set { SetValue(Bool2Property, value); }
        }

        public static readonly DependencyProperty Bool3Property = DependencyProperty.Register(
             "Bool3", typeof(bool), typeof(MultiParameterPS3Effect),
             new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(3)));

        public bool Bool3
        {
            get { return (bool)GetValue(Bool3Property); }
            set { SetValue(Bool3Property, value); }
        }

        public static readonly DependencyProperty Bool4Property = DependencyProperty.Register(
            "Bool4", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(4)));

        public bool Bool4
        {
            get { return (bool)GetValue(Bool4Property); }
            set { SetValue(Bool4Property, value); }
        }

        public static readonly DependencyProperty Bool5Property = DependencyProperty.Register(
            "Bool5", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(5)));

        public bool Bool5
        {
            get { return (bool)GetValue(Bool5Property); }
            set { SetValue(Bool5Property, value); }
        }

        public static readonly DependencyProperty Bool6Property = DependencyProperty.Register(
            "Bool6", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(6)));

        public bool Bool6
        {
            get { return (bool)GetValue(Bool6Property); }
            set { SetValue(Bool6Property, value); }
        }

        public static readonly DependencyProperty Bool7Property = DependencyProperty.Register(
            "Bool7", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(7)));

        public bool Bool7
        {
            get { return (bool)GetValue(Bool7Property); }
            set { SetValue(Bool7Property, value); }
        }

        public static readonly DependencyProperty Bool8Property = DependencyProperty.Register(
            "Bool8", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(8)));

        public bool Bool8
        {
            get { return (bool)GetValue(Bool8Property); }
            set { SetValue(Bool8Property, value); }
        }

        public static readonly DependencyProperty Bool9Property = DependencyProperty.Register(
            "Bool9", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(9)));

        public bool Bool9
        {
            get { return (bool)GetValue(Bool9Property); }
            set { SetValue(Bool9Property, value); }
        }

        public static readonly DependencyProperty Bool10Property = DependencyProperty.Register(
            "Bool10", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(10)));

        public bool Bool10
        {
            get { return (bool)GetValue(Bool10Property); }
            set { SetValue(Bool10Property, value); }
        }
        public static readonly DependencyProperty Bool11Property = DependencyProperty.Register(
            "Bool11", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(11)));

        public bool Bool11
        {
            get { return (bool)GetValue(Bool11Property); }
            set { SetValue(Bool11Property, value); }
        }
        public static readonly DependencyProperty Bool12Property = DependencyProperty.Register(
            "Bool12", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(12)));

        public bool Bool12
        {
            get { return (bool)GetValue(Bool12Property); }
            set { SetValue(Bool12Property, value); }
        }

        public static readonly DependencyProperty Bool13Property = DependencyProperty.Register(
            "Bool13", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(13)));

        public bool Bool13
        {
            get { return (bool)GetValue(Bool13Property); }
            set { SetValue(Bool13Property, value); }
        }

        public static readonly DependencyProperty Bool14Property = DependencyProperty.Register(
            "Bool14", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(14)));

        public bool Bool14
        {
            get { return (bool)GetValue(Bool14Property); }
            set { SetValue(Bool14Property, value); }
        }

        public static readonly DependencyProperty Bool15Property = DependencyProperty.Register(
            "Bool15", typeof(bool), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(false, ShaderEffect.PixelShaderConstantCallback(15)));

        public bool Bool15
        {
            get { return (bool)GetValue(Bool15Property); }
            set { SetValue(Bool15Property, value); }
        }
#endregion

#region int properties
        public static readonly DependencyProperty Int0Property = DependencyProperty.Register(
            "Int0", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(0)));

        public int Int0
        {
            get { return (int)GetValue(Int0Property); }
            set { SetValue(Int0Property, value); }
        }

        public static readonly DependencyProperty Int1Property = DependencyProperty.Register(
            "Int1", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(1)));

        public int Int1
        {
            get { return (int)GetValue(Int1Property); }
            set { SetValue(Int1Property, value); }
        }

        public static readonly DependencyProperty Int2Property = DependencyProperty.Register(
            "Int2", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(2)));

        public int Int2
        {
            get { return (int)GetValue(Int2Property); }
            set { SetValue(Int2Property, value); }
        }

        public static readonly DependencyProperty Int3Property = DependencyProperty.Register(
             "Int3", typeof(int), typeof(MultiParameterPS3Effect),
             new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(3)));

        public int Int3
        {
            get { return (int)GetValue(Int3Property); }
            set { SetValue(Int3Property, value); }
        }

        public static readonly DependencyProperty Int4Property = DependencyProperty.Register(
            "Int4", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(4)));

        public int Int4
        {
            get { return (int)GetValue(Int4Property); }
            set { SetValue(Int4Property, value); }
        }

        public static readonly DependencyProperty Int5Property = DependencyProperty.Register(
            "Int5", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(5)));

        public int Int5
        {
            get { return (int)GetValue(Int5Property); }
            set { SetValue(Int5Property, value); }
        }

        public static readonly DependencyProperty Int6Property = DependencyProperty.Register(
            "Int6", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(6)));

        public int Int6
        {
            get { return (int)GetValue(Int6Property); }
            set { SetValue(Int6Property, value); }
        }

        public static readonly DependencyProperty Int7Property = DependencyProperty.Register(
            "Int7", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(7)));

        public int Int7
        {
            get { return (int)GetValue(Int7Property); }
            set { SetValue(Int7Property, value); }
        }

        public static readonly DependencyProperty Int8Property = DependencyProperty.Register(
            "Int8", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(8)));

        public int Int8
        {
            get { return (int)GetValue(Int8Property); }
            set { SetValue(Int8Property, value); }
        }

        public static readonly DependencyProperty Int9Property = DependencyProperty.Register(
            "Int9", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(9)));

        public int Int9
        {
            get { return (int)GetValue(Int9Property); }
            set { SetValue(Int9Property, value); }
        }

        public static readonly DependencyProperty Int10Property = DependencyProperty.Register(
            "Int10", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(10)));

        public int Int10
        {
            get { return (int)GetValue(Int10Property); }
            set { SetValue(Int10Property, value); }
        }
        public static readonly DependencyProperty Int11Property = DependencyProperty.Register(
            "Int11", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(11)));

        public int Int11
        {
            get { return (int)GetValue(Int11Property); }
            set { SetValue(Int11Property, value); }
        }
        public static readonly DependencyProperty Int12Property = DependencyProperty.Register(
            "Int12", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(12)));

        public int Int12
        {
            get { return (int)GetValue(Int12Property); }
            set { SetValue(Int12Property, value); }
        }

        public static readonly DependencyProperty Int13Property = DependencyProperty.Register(
            "Int13", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(13)));

        public int Int13
        {
            get { return (int)GetValue(Int13Property); }
            set { SetValue(Int13Property, value); }
        }

        public static readonly DependencyProperty Int14Property = DependencyProperty.Register(
            "Int14", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(14)));

        public int Int14
        {
            get { return (int)GetValue(Int14Property); }
            set { SetValue(Int14Property, value); }
        }

        public static readonly DependencyProperty Int15Property = DependencyProperty.Register(
            "Int15", typeof(int), typeof(MultiParameterPS3Effect),
            new PropertyMetadata(0, ShaderEffect.PixelShaderConstantCallback(15)));

        public int Int15
        {
            get { return (int)GetValue(Int15Property); }
            set { SetValue(Int15Property, value); }
        }
#endregion
    }
}