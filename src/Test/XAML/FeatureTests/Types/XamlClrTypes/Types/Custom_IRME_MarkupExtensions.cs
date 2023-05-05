// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// DimensionExtensionBase class
    /// </summary>
    public abstract class DimensionExtensionBase : MarkupExtension
    {
        /// <summary> Length value</summary>
        private readonly int _length;

        /// <summary> Widthe value</summary>
        private readonly int _width;

        /// <summary> Height value </summary>
        private readonly int _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionExtensionBase"/> class.
        /// </summary>
        /// <param name="l">The length l.</param>
        /// <param name="w">The width w.</param>
        protected DimensionExtensionBase(int l, int w)
        {
            _length = l;
            _width = w;
            _height = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionExtensionBase"/> class.
        /// </summary>
        /// <param name="l">The length l.</param>
        /// <param name="w">The width w.</param>
        /// <param name="h">The height h.</param>
        protected DimensionExtensionBase(int l, int w, int h)
        {
            _length = l;
            _width = w;
            _height = h;
        }

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>object value </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _length + _width + _height;
        }

        /// <summary>
        /// Provides the expression.
        /// </summary>
        /// <returns>DimensionalExpression value </returns>
        public DimensionalExpression ProvideExpression()
        {
            return new DimensionalExpression(_length, _width, _height);
        }
    }

    /// <summary>
    /// CustomMarkupExtension class for IRME cases
    /// </summary>
    [MarkupExtensionReturnType(typeof(int))]
    public class DimensionExtension : DimensionExtensionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionExtension"/> class.
        /// </summary>
        /// <param name="l">The length l.</param>
        /// <param name="w">The width w.</param>
        public DimensionExtension(int l, int w)
            : base(l, w)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionExtension"/> class.
        /// </summary>
        /// <param name="l">The length l.</param>
        /// <param name="w">The width w.</param>
        /// <param name="h">The height h.</param>
        public DimensionExtension(int l, int w, int h)
            : base(l, w, h)
        {
        }
    }

    /// <summary>
    /// CustomMarkupExtension_NoMERT class for IRME cases
    /// </summary>
    public class DimensionNoMERTExtension : DimensionExtensionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionNoMERTExtension"/> class.
        /// </summary>
        /// <param name="l">The length l.</param>
        /// <param name="w">The width w.</param>
        public DimensionNoMERTExtension(int l, int w)
            : base(l, w)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionNoMERTExtension"/> class.
        /// </summary>
        /// <param name="l">The length l.</param>
        /// <param name="w">The width w.</param>
        /// <param name="h">The height h.</param>
        public DimensionNoMERTExtension(int l, int w, int h)
            : base(l, w, h)
        {
        }
    }
}
