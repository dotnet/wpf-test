// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//   CustomPropertyTypeConverter, We check the culture parameter passed in is us-en
//
// Owner: Microsoft
 
//
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Markup;
using System.ComponentModel;
namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    ///     Converter that Check culture.
    /// </summary>
    public class CustomPropertyTypeConverter : TypeConverter
    {

        /// <summary>
        /// ConvertTo - Attempt to convert a CustomPropertyType to the given type
        /// </summary>
        /// <returns>
        /// The object which was constructed.
        /// </returns>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The object to convert to a Brush. </param>
        /// <param name="destinationType"> The type to which this will convert the Brush instance. </param>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext,
                                         CultureInfo cultureInfo,
                                         object value,
                                         Type destinationType)
        {
            if (destinationType != typeof(String))
                throw new Exception("Can only convert from string");

#if TESTBUILD_NET_ATLEAST_46
            if (!cultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                throw new Exception("Culture passed to CustomPropertyTypeConverter.ConvertTo should be Invariant.");
            }
#else
            if (!cultureInfo.Equals(CultureInfo.GetCultureInfo("en-us")))
            {
                throw new Exception("Culture passed to CustomPropertyTypeConverter.ConvertTo should be en-us.");
            }
#endif

            return ((CustomPropertyType)value).Value;
        }

        /// <summary>
        /// ConvertFrom - Attempt to convert to a Brush from the given object
        /// </summary>
        /// <returns>
        /// The Brush which was constructed.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// A NotSupportedException is thrown if the example object is null or is not a valid type
        /// which can be converted to a Brush.
        /// </exception>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The object to convert to a CustomPropertyType. </param>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
#if TESTBUILD_NET_ATLEAST_46
            if (!cultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                throw new Exception("Culture passed to CustomPropertyTypeConverter.ConvertFrom should be Invariant.");
            }
#else
            if (!cultureInfo.Equals(CultureInfo.GetCultureInfo("en-us")))
            {
                throw new Exception("Culture passed to CustomPropertyTypeConverter.ConvertFrom should be en-us.");
            }
#endif

            return new CustomPropertyType(value as string);
        }
        /// <summary>
        /// CanConvertFrom
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {

            if(sourceType.Equals(typeof(String)))
               return true;
            return false;

        }

        /// <summary>
        /// CanConvertTo
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if(destinationType.Equals(typeof(String)))
               return true;
            return false;
        }
    }
}

