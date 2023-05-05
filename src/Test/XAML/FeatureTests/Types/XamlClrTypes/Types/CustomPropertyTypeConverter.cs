// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Converter that Check culture.
    /// </summary>
    public class CustomPropertyTypeConverter : TypeConverter
    {
        /// <summary>
        /// ConvertTo - Attempt to convert a CustomPropertyType to the given type
        /// </summary>
        /// <param name="typeDescriptorContext">The ITypeDescriptorContext for this call.</param>
        /// <param name="cultureInfo">The CultureInfo which is respected when converting.</param>
        /// <param name="value">The object to convert to a Brush.</param>
        /// <param name="destinationType">The type to which this will convert the Brush instance.</param>
        /// <returns>The object which was constructed.</returns>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                throw new Exception("Can only convert from string");
            }

            if (!cultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                throw new Exception("Culture passed to CustomPropertyTypeConverter.ConvertTo should be Invariant.");
            }

            return ((CustomPropertyType) value).Value;
        }

        /// <summary>
        /// ConvertFrom - Attempt to convert to a Brush from the given object
        /// </summary>
        /// <param name="typeDescriptorContext">The ITypeDescriptorContext for this call.</param>
        /// <param name="cultureInfo">The CultureInfo which is respected when converting.</param>
        /// <param name="value">The object to convert to a CustomPropertyType.</param>
        /// <returns>The Brush which was constructed.</returns>
        /// <exception cref="NotSupportedException">
        /// A NotSupportedException is thrown if the example object is null or is not a valid type
        /// which can be converted to a Brush.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value)
        {
            if (!cultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                throw new Exception("Culture passed to CustomPropertyTypeConverter.ConvertFrom should be Invariant.");
            }

            return new CustomPropertyType(value as string);
        }

        /// <summary>
        /// Can ConvertFrom
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.Equals(typeof(string)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Can ConvertTo
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.Equals(typeof(string)))
            {
                return true;
            }

            return false;
        }
    }
}
