// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test shader effect with multiple parameters.
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Effects;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// A custom effect with parameter of all allowed type. 
    /// Note: can only validate double and float with value less than 1000
    ///       and other type with value less than 100 with visual validation. 
    /// </summary>
    public class MultiParameterShaderEffect : ShaderEffect
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MultiParameterShaderEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("pack://application:,,,/ShaderEffects;component/MultiParameterShader.ps");
   
            this.PixelShader = pixelShader;
            this.DdxUvDdyUvRegisterIndex = 9;

            UpdateShaderValue(FloatValueProperty);
            UpdateShaderValue(DoubleValueProperty);
            UpdateShaderValue(PointValueProperty);
            UpdateShaderValue(Point3DValueProperty);
            UpdateShaderValue(Point4DValueProperty);
            UpdateShaderValue(Vector3DValueProperty);
            UpdateShaderValue(VectorValueProperty);
            UpdateShaderValue(SizeValueProperty);
            UpdateShaderValue(ColorValueProperty);

            UpdateShaderValue(InputProperty);
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
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(MultiParameterShaderEffect), 0);

        /// <summary>
        /// Property of type float. In shader, value larger than 1000 will be truncated.
        /// </summary>
        public float FloatValue
        {
            get { return (float)GetValue(FloatValueProperty); }
            set { SetValue(FloatValueProperty, value); }
        }

        public static readonly DependencyProperty FloatValueProperty =
             DependencyProperty.Register("FloatValue", typeof(float), typeof(MultiParameterShaderEffect),
             new UIPropertyMetadata((float)500.0, PixelShaderConstantCallback(0)));

        /// <summary>
        /// Property of type double. In shader, value larger than 1000 will be truncated.
        /// </summary>
        public double DoubleValue
        {
            get { return (double)GetValue(DoubleValueProperty); }
            set { SetValue(DoubleValueProperty, value); }
        }

        public static readonly DependencyProperty DoubleValueProperty =
             DependencyProperty.Register("DoubleValue", typeof(double), typeof(MultiParameterShaderEffect),
             new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

        /// <summary>
        /// Property of type Point. In shader, X and Y value larger than 100 will be truncated.
        /// </summary>
        public Point PointValue
        {
            get { return (Point)GetValue(PointValueProperty); }
            set { SetValue(PointValueProperty, value); }
        }

        public static readonly DependencyProperty PointValueProperty =
             DependencyProperty.Register("PointValue", typeof(Point), typeof(MultiParameterShaderEffect),
             new UIPropertyMetadata(new Point(1.0, 1.0), PixelShaderConstantCallback(2)));

        /// <summary>
        /// Property of type Point3D. In shader, X, Y, and Y value larger than 100 will be truncated.
        /// </summary>
        public Point3D Point3DValue
        {
            get { return (Point3D)GetValue(Point3DValueProperty); }
            set { SetValue(Point3DValueProperty, value); }
        }

        public static readonly DependencyProperty Point3DValueProperty =
             DependencyProperty.Register("Point3DValue", typeof(Point3D), typeof(MultiParameterShaderEffect),
             new UIPropertyMetadata(new Point3D(1.0,1.0,1.0), PixelShaderConstantCallback(3)));

        /// <summary>
        /// Property of type Point4D. In shader, X, Y, Z, and W value larger than 100 will be truncated.
        /// </summary>
        public Point4D Point4DValue
        {
            get { return (Point4D)GetValue(Point4DValueProperty); }
            set { SetValue(Point4DValueProperty, value); }
        }

        public static readonly DependencyProperty Point4DValueProperty =
             DependencyProperty.Register("Point4DValue", typeof(Point4D), typeof(MultiParameterShaderEffect),
             new UIPropertyMetadata(new Point4D(1.0, 1.0, 1.0, 1.0), PixelShaderConstantCallback(4)));
        
        /// <summary>
        /// Property of type Vector3D. In shader, X, Y, and Z value larger than 100 will be truncated.
        /// </summary>
        public Vector3D Vector3DValue
        {
            get { return (Vector3D)GetValue(Vector3DValueProperty); }
            set { SetValue(Vector3DValueProperty, value); }
        }

        public static readonly DependencyProperty Vector3DValueProperty =
             DependencyProperty.Register("Vector3DValue", typeof(Vector3D), typeof(MultiParameterShaderEffect),
             new UIPropertyMetadata(new Vector3D(1.0,1.0,1.0), PixelShaderConstantCallback(5)));

        /// <summary>
        /// Property of type Size. In shader, component value larger than 100 will be truncated.
        /// </summary>
        public Size SizeValue
        {
            get { return (Size)GetValue(SizeValueProperty); }
            set { SetValue(SizeValueProperty, value); }
        }

        public static readonly DependencyProperty SizeValueProperty =
            DependencyProperty.Register("SizeValue", typeof(Size), typeof(MultiParameterShaderEffect),
                    new UIPropertyMetadata(new Size(20, 10), PixelShaderConstantCallback(6)));

        /// <summary>
        /// Property of type Vector. In shader, component value larger than 100 will be truncated.
        /// </summary>
        public Vector VectorValue
        {
            get { return (Vector)GetValue(VectorValueProperty); }
            set { SetValue(VectorValueProperty, value); }
        }

        public static readonly DependencyProperty VectorValueProperty =
            DependencyProperty.Register("VectorValue", typeof(Vector), typeof(MultiParameterShaderEffect),
                    new UIPropertyMetadata(new Vector(1, 1), PixelShaderConstantCallback(7)));

        /// <summary>
        /// Property of type Color. 
        /// </summary>
        public Color ColorValue
        {
            get { return (Color)GetValue(ColorValueProperty); }
            set { SetValue(ColorValueProperty, value); }
        }

        public static readonly DependencyProperty ColorValueProperty =
            DependencyProperty.Register("ColorValue", typeof(Color), typeof(MultiParameterShaderEffect),
                    new UIPropertyMetadata(Colors.Black, PixelShaderConstantCallback(8)));
    }
}