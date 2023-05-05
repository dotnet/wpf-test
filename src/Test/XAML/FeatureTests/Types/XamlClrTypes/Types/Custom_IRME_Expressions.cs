// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// CustomExpression class for IRME cases
    /// </summary>
    public class DimensionalExpression
    {
        /// <summary> Length value</summary>
        private readonly int _length;

        /// <summary> Widthe value</summary>
        private readonly int _width;

        /// <summary> Height value </summary>
        private readonly int _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionalExpression"/> class.
        /// </summary>
        /// <param name="l">The Length l.</param>
        /// <param name="w">The width w.</param>
        /// <param name="h">The height h.</param>
        public DimensionalExpression(int l, int w, int h)
        {
            _length = l;
            _width = w;
            _height = h;
        }

        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <value>The area value .</value>
        public int Area
        {
            get
            {
                return _length * _width;
            }
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        /// <value>The volume.</value>
        public int Volume
        {
            get
            {
                return _length * _width * _height;
            }
        }
    }
}
