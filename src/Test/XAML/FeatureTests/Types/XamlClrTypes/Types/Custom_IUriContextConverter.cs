// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom IUriContextConverter
    /// </summary>
    public class Custom_IUriContextConverter : TypeConverter
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
            return true;
        }

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <param name="value">The object value.</param>
        /// <returns>object value</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo cultureInfo, object value)
        {
            Custom_IUriContext customIUriCntxt = new Custom_IUriContext();
            IUriContext iUriCntxt = (IUriContext) context.GetService(typeof(IUriContext));
            customIUriCntxt.TextValue = value.ToString();
            customIUriCntxt.UriProperty = iUriCntxt.BaseUri;
            return customIUriCntxt;
        }
    }

    /// <summary>
    /// Custom_Tag having Custom_IUriContext property
    /// </summary>
    public class Custom_Tag
    {
        /// <summary>
        /// Gets or sets the context property.
        /// </summary>
        /// <value>The context property.</value>
        public Custom_IUriContext ContextProperty { get; set; }
    }
}
