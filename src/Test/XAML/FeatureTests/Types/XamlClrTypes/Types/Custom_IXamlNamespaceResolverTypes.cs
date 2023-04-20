// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom XamlNamespaceResolverExtension
    /// </summary>
    public class Custom_XamlNamespaceResolverExtension : MarkupExtension
    {
        /// <summary>
        /// Prefix string
        /// </summary>
        private readonly string _prefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_XamlNamespaceResolverExtension"/> class.
        /// </summary>
        public Custom_XamlNamespaceResolverExtension()
        {
            _prefix = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_XamlNamespaceResolverExtension"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public Custom_XamlNamespaceResolverExtension(string prefix)
        {
            this._prefix = prefix;
        }

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>object value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IXamlNamespaceResolver resolver = (IXamlNamespaceResolver) serviceProvider.GetService(typeof(IXamlNamespaceResolver));
            return resolver.GetNamespace(_prefix);
        }
    }

    /// <summary>
    /// Custom XamlNamespaceResolverConverter
    /// </summary>
    public class Custom_XamlNamespaceResolverConverter : TypeConverter
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
            string prefix = value as string;
            IXamlNamespaceResolver resolver = (IXamlNamespaceResolver) context.GetService(typeof(IXamlNamespaceResolver));
            return resolver.GetNamespace(prefix);
        }
    }

    /// <summary>
    /// Custom XamlNamespaceResolverTestObject
    /// </summary>
    public class Custom_XamlNamespaceResolverTestObject
    {
        /// <summary>
        /// Gets or sets the ME prop.
        /// </summary>
        /// <value>The ME prop.</value>
        public string MEProp { get; set; }

        /// <summary>
        /// Gets or sets the TC prop.
        /// </summary>
        /// <value>The TC prop.</value>
        [TypeConverter(typeof(Custom_XamlNamespaceResolverConverter))]
        public string TCProp { get; set; }
    }
}
