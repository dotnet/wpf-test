// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: A ShaderEffect that expose Padding propteries, get the color by Average 10 points 
 *          on the line toward an EndPoint. 
 * Owner: Microsoft 
 ********************************************************************/
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;
using System;

namespace Microsoft.Test.Effects
{
    public class AveragePaddingEffect : ShaderEffectWithPadding
    {
        public AveragePaddingEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(@"pack://application:,,,/ShaderEffects;component/average.ps");
            this.PixelShader = pixelShader;
            UpdateShaderValue(EndPointProperty);
        }


        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(AveragePaddingEffect),
                    new UIPropertyMetadata(new Point(0.5, 0.5), PixelShaderConstantCallback(0)));
    }
}