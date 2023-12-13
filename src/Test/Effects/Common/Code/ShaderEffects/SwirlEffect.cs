// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Diagnostics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// A effect copied from Greg's EffectLibrary, located under : 
    /// %sdxroot%\wpf\Test\Effects\FeatureTests\code\ShaderEffects
    /// The only difference is we are not using his Global to make Uri. 
    /// </summary>
    public class SwirlEffect : ShaderEffect
    {
        public SwirlEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"pack://application:,,,/ShaderEffects;component/Swirl.ps");
            this.PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(CenterProperty);
            UpdateShaderValue(SwirlStrengthProperty);
            UpdateShaderValue(AngleFrequencyProperty);

            _generalTransform = new SwirlGeneralTransform(this);
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(SwirlEffect), 0);


        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(SwirlEffect),
                    new UIPropertyMetadata(new Point(0.5, 0.5), PixelShaderConstantCallback(0)));


        public double SwirlStrength
        {
            get { return (double)GetValue(SwirlStrengthProperty); }
            set { SetValue(SwirlStrengthProperty, value); }
        }

        public static readonly DependencyProperty SwirlStrengthProperty =
            DependencyProperty.Register("SwirlStrength", typeof(double), typeof(SwirlEffect), 
                                        new UIPropertyMetadata(0.5, PixelShaderConstantCallback(1)));


        public double AngleFrequency
        {
            get { return (double)GetValue(AngleFrequencyProperty); }
            set { SetValue(AngleFrequencyProperty, value); }
        }

        public static readonly DependencyProperty AngleFrequencyProperty =
            DependencyProperty.Register("AngleFrequency", typeof(double), typeof(SwirlEffect), 
                                        new UIPropertyMetadata(1.0, PixelShaderConstantCallback(2)));



        protected override GeneralTransform EffectMapping
        {
            get
            {
                return _generalTransform;
            }
        }

        /// <summary>
        /// For transforming input and tree transformations.
        /// </summary>
        private class SwirlGeneralTransform : GeneralTransform
        {
            public SwirlGeneralTransform(SwirlEffect eff)
            {
                _eff = eff;
            }

            public override GeneralTransform Inverse
            {
                get
                {
                    // Cache this since it can get called often
                    if (_myInverseTransform == null)
                    {
                        _myInverseTransform = (SwirlGeneralTransform)this.Clone();
                        _myInverseTransform._isInverse = !this._isInverse;
                    }
                    return _myInverseTransform;
                }
            }

            public override Rect TransformBounds(Rect rect)
            {
                // For this operation, the bounds is the bounding box of the 4 transformed points. 
                // Need to transform each of them, and then circumscribe.  This is true for both the 
                // forward and the inverse.

                Point tl, tr, bl, br;
                if (TryTransform(rect.TopLeft, out tl) &&
                    TryTransform(rect.TopRight, out tr) &&
                    TryTransform(rect.BottomLeft, out bl) &&
                    TryTransform(rect.BottomRight, out br))
                {
                    double maxX = Math.Max(tl.X, Math.Max(tr.X, Math.Max(bl.X, br.X)));
                    double minX = Math.Min(tl.X, Math.Min(tr.X, Math.Min(bl.X, br.X)));

                    double maxY = Math.Max(tl.Y, Math.Max(tr.Y, Math.Max(bl.Y, br.Y)));
                    double minY = Math.Min(tl.Y, Math.Min(tr.Y, Math.Min(bl.Y, br.Y)));

                    return new Rect(minX, minY, maxX - minX, maxY - minY);
                }
                else
                {
                    return Rect.Empty;
                }
            }

            public override bool TryTransform(Point inPoint, out Point result)
            {
                // Exactly follows what the HLSL shader itself does.

                Vector dir = inPoint - _eff.Center;
                double l = dir.Length;
                dir.Normalize();

                double angle = Math.Atan2(dir.Y, dir.X);

                double inverseFactor = _isInverse ? 1 : -1;
                double newAngle = angle + inverseFactor * _eff.SwirlStrength * l;

                double angleFrequency = _eff.AngleFrequency;
                double xAmt = Math.Cos(angleFrequency * newAngle) * l;
                double yAmt = Math.Sin(angleFrequency * newAngle) * l;

                result = _eff.Center + new Vector(xAmt, yAmt);

                return true;
            }

            protected override Freezable CreateInstanceCore()
            {
                return new SwirlGeneralTransform(_eff) { _isInverse = _isInverse };
            }

            private readonly SwirlEffect _eff;
            private bool _isInverse = false;
            private SwirlGeneralTransform _myInverseTransform = null;
        }


        private SwirlGeneralTransform _generalTransform;
    }


}
