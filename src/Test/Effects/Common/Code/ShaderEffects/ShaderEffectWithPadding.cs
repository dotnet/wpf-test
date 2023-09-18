// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// a class inherits BasicShaderEffect and exposes 
    /// Padding properties.
    /// </summary>
    public class ShaderEffectWithPadding : BasicShaderEffect
    {
        public new double PaddingLeft
        {
            get
            {
                return base.PaddingLeft;
            }
            set
            {
                base.PaddingLeft = value;
            }
        }
        public new double PaddingRight
        {
            get
            {
                return base.PaddingRight;
            }
            set
            {
                base.PaddingRight = value;
            }
        }
        public new double PaddingTop
        {
            get
            {
                return base.PaddingTop;
            }
            set
            {
                base.PaddingTop = value;
            }
        }
        public new double PaddingBottom
        {
            get
            {
                return base.PaddingBottom;
            }
            set
            {
                base.PaddingBottom = value;
            }
        }

    }
}

