// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//   This is very simple type with Value property.
//
// Owner: Microsoft
 
//

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;
using System.Windows.Markup;


namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// This is very simple type with Value property.
    /// </summary>
    [TypeConverter(typeof(CustomPropertyTypeConverter))]
    public class CustomPropertyType
    {
        #region Constructors

        /// <summary>
        /// Default constructor for CustomPropertyType.
        /// </summary>
        public CustomPropertyType()
        {
        }

        /// <summary>
        /// CustomPropertyType - The constructor accepts the color of the brush
        /// </summary>
        /// <param name="value"> The string value. </param>
        public CustomPropertyType(string value)
        {
            _value=value;
        }
        /// <summary>
        /// Property Value
        /// </summary>
        /// <value></value>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        #endregion Constructors
        string _value = String.Empty;
    }
}

