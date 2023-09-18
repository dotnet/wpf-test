// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: A ShaderEffect with ScaleTransform. 
 * Owner: Microsoft 
 ********************************************************************/
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;
using System;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// A Effect with Scale Transform. 
    /// </summary>
    public class ScaleEffect : ShaderEffect
    {
        public ScaleEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"pack://application:,,,/ShaderEffects;component/ScaleShader.ps");
            
            UpdateShaderValue(CenterProperty);
            UpdateShaderValue(ScaleProperty);

            UpdateShaderValue(InputProperty);
            this.PixelShader = pixelShader;
            ScaleGeneralTransform transform = new ScaleGeneralTransform(this);
            _generalTransform = (GeneralTransform)transform;
        }

        protected override GeneralTransform EffectMapping
        {
            get
            {
                return _generalTransform;
            }
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
        // Using a DependencyProperty as the backing store for InputProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ScaleEffect), 0);


        public Vector Scale
        {
            get { return (Vector)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(Vector), typeof(ScaleEffect),
                    new UIPropertyMetadata(new Vector(0.5, 0.5), PixelShaderConstantCallback(0)));


        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(ScaleEffect),
                    new UIPropertyMetadata(new Point(0, 0), PixelShaderConstantCallback(1)));

        private GeneralTransform _generalTransform;

        private class ScaleGeneralTransform : GeneralTransform
        {
            public ScaleGeneralTransform(ScaleEffect eff)
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
                        _myInverseTransform = (ScaleGeneralTransform)this.Clone();
                        _myInverseTransform._isInverse = !this._isInverse;
                    }
                    return _myInverseTransform;
                }
            }

            public override Rect TransformBounds(Rect rect)
            {
                Point topLeft, bottomRight;

                TryTransform(rect.TopLeft, out topLeft);
                TryTransform(rect.BottomRight, out bottomRight);

                return new Rect(topLeft, bottomRight);
            }

            public override bool TryTransform(Point inPoint, out Point result)
            {
                // In this particular case, the inverse transform is the same as the forward
                // transform.

                Point center = _eff.Center;
                double scaleX, scaleY;
                if (_isInverse)
                {
                    scaleX = 1 / _eff.Scale.X;
                    scaleY = 1 / _eff.Scale.Y;
                }
                else
                {
                    scaleX = _eff.Scale.X;
                    scaleY = _eff.Scale.Y;
                }
                result = new Point((center.X + (inPoint.X - center.X) * scaleX), (center.Y + (inPoint.Y - center.Y) * scaleY));

                return true;
            }

            protected override Freezable CreateInstanceCore()
            {
                return new ScaleGeneralTransform(_eff) { _isInverse = _isInverse };
            }


            private readonly ScaleEffect _eff;
            private bool _isInverse = false;
            private ScaleGeneralTransform _myInverseTransform = null;
        }
    }

    /// <summary>
    /// A general transform for ScaleEffect. 
    /// </summary>
    public class ScaleGeneralTransform : GeneralTransform
    {
        public ScaleGeneralTransform(ScaleEffect eff)
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
                    _myInverseTransform = (ScaleGeneralTransform)this.Clone();
                    _myInverseTransform._isInverse = !this._isInverse;
                }
                return _myInverseTransform;
            }
        }

        public override Rect TransformBounds(Rect rect)
        {
            // This particular effect keeps axis aligned lines axis aligned, so transformation of the rect is just
            // transformation of its corner points.
            Point tl, br;
            Rect result;
            bool ok1 = TryTransform(rect.TopLeft, out tl);
            bool ok2 = TryTransform(rect.BottomRight, out br);
            if (ok1 && ok2)
            {
                result = new Rect(tl, br);
            }
            else
            {
                result = Rect.Empty;
            }

            return result;
        }

        public override bool TryTransform(Point inPoint, out Point result)
        {
            // In this particular case, the inverse transform is the same as the forward
            // transform.


            // If inside the ellipse, calculate that magnification/minification
            Point center = _eff.Center;
            Vector ray = inPoint - center;

            // Inverse maps a point from after the effect was applied to the point that it came from before the effect.
            // Non-inverse maps where a point before the effect is applied goes after the effect is applied.
            double scaleFactorX = _isInverse ? _eff.Scale.X : 1.0 / _eff.Scale.X;
            double scaleFactorY = _isInverse ? _eff.Scale.Y : 1.0 / _eff.Scale.Y;


            result = new Point(scaleFactorX * (inPoint.X - center.X) + center.X, scaleFactorY * (inPoint.Y - center.Y) + center.Y);


            return true;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new ScaleGeneralTransform(_eff) { _isInverse = _isInverse };
        }

        private bool IsPointInEllipse(Point pt, Point center, Size radii)
        {
            Vector ray = pt - center;
            double rayPctX = ray.X / radii.Width;
            double rayPctY = ray.Y / radii.Height;

            // Normally would take sqrt() for length, but since we're comparing 
            // to 1.0, it doesn't matter.
            double pctLength = rayPctX * rayPctX + rayPctY * rayPctY;

            return pctLength <= 1.0;
        }

        private readonly ScaleEffect _eff;
        private bool _isInverse = false;
        private ScaleGeneralTransform _myInverseTransform = null;
    }
}