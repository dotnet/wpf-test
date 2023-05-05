// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// This is very simple type with Value property.
    /// </summary>
    [TypeConverter(typeof(CustomPropertyTypeConverter))]
    public class CustomPropertyType
    {
        /// <summary>
        /// value string
        /// </summary>
        private string _value = String.Empty;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPropertyType"/> class.
        /// </summary>
        public CustomPropertyType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPropertyType"/> class.
        /// </summary>
        /// <param name="value">The string value.</param>
        public CustomPropertyType(string value)
        {
            this._value = value;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                this._value = value;
            }
        }

        #endregion Constructors
    }
}
