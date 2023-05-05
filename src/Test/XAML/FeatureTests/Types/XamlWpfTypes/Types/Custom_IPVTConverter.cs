// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom IPVTConverter
    /// </summary>
    public class Custom_IPVTConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            IProvideValueTarget ipvt = (IProvideValueTarget) context.GetService(typeof(IProvideValueTarget));
            Custom_IPVTObject retObj = new Custom_IPVTObject();
            retObj.TargetObject = ipvt.TargetObject;
            retObj.TargetProperty = ipvt.TargetProperty;

            if (ipvt.TargetProperty is DependencyProperty)
            {
                retObj.TargetPropertyType = "DependencyProperty";
            }
            else if (ipvt.TargetProperty is MethodInfo) // For attached propeties, this code will not be hit until attached properties have been implemented outside of WPF
            {
                retObj.TargetPropertyType = "MethodInfo";
            }
            else if (ipvt.TargetProperty is PropertyInfo)
            {
                retObj.TargetPropertyType = "PropertyInfo";
            }
            else
            {
                retObj.TargetPropertyType = ipvt.TargetProperty.ToString();
            }

            return retObj;
        }
    }
}
