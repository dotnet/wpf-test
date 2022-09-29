// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace D2Payloads
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;


    /// <summary>
    /// 
    /// </summary> 
    public sealed class FixedDocSourceConverter: TypeConverter
    {
        //-------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// CanConvertFrom - Returns whether or not this class can convert from a given type.
        /// </summary>
        /// <returns>
        /// bool - True if thie converter can convert from the provided type, false if not.
        /// </returns>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="sourceType"> The Type being queried for support. </param>
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            // We can only handle strings, and uri
            TypeCode tc = Type.GetTypeCode(sourceType);
            switch (tc)
            {
                case TypeCode.String:
                    return true;
                default: 
                    return false;
            }
        }

        /// <summary>
        /// CanConvertTo - Returns whether or not this class can convert to a given type.
        /// </summary>
        /// <returns>
        /// bool - True if this converter can convert to the provided type, false if not.
        /// </returns>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="destinationType"> The Type being queried for support. </param>
        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType) 
        {
            // We can convert to an InstanceDescriptor or to a string.
            if (destinationType == typeof(InstanceDescriptor) ||
                destinationType == typeof(string)) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ConvertFrom - Attempt to convert to a Length from the given object
        /// </summary>
        /// <returns>
        /// The Length which was constructed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the example object is not null and is not a valid type
        /// which can be converted to a Length.
        /// </exception>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="source"> The object to convert to a Length. </param>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, 
                                           CultureInfo cultureInfo, 
                                           object source)
        {
            if (source != null)
            {
                if (source is string) 
                {
                    return FixedDocData.FromString(typeDescriptorContext, cultureInfo, (string)source);
                }
            }
            throw GetConvertFromException(source);
        }

        /// <summary>
        /// ConvertTo - Attempt to convert a Length to the given type
        /// </summary>
        /// <returns>
        /// The object which was constructed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An ArgumentNullException is thrown if the example object is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An ArgumentException is thrown if the example object is not null and is not a Brush,
        /// or if the destinationType isn't one of the valid destination types.
        /// </exception>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The Length to convert. </param>
        /// <param name="destinationType">The type to which to convert the Length instance. </param>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, 
                                         CultureInfo cultureInfo,
                                         object value,
                                         Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (    value != null
                &&  value is FixedDocSource )
            {
                FixedDocSource docSource = (FixedDocSource)value;
                if (destinationType == typeof(string)) 
                {
                    return docSource.Source.ToString();
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    return docSource.ToInstanceDescriptor();
                }
            }
            throw GetConvertToException(value, destinationType);
        }
        #endregion 

        //-------------------------------------------------------------------
        //
        //  Internal Methods
        //
        //-------------------------------------------------------------------

        #region Internal Methods
        #endregion

    }
}
