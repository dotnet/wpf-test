// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom IUriContext
    /// </summary>
    [TypeConverter(typeof(Custom_IUriContextConverter))]
    public class Custom_IUriContext : IUriContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_IUriContext"/> class.
        /// </summary>
        public Custom_IUriContext()
        {
        }

        /// <summary>
        /// Gets or sets the base URI.
        /// </summary>
        /// <value>The base URI.</value>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the text value.
        /// </summary>
        /// <value>The text value.</value>
        public string TextValue { get; set; }

        /// <summary>
        /// Gets or sets the URI property.
        /// </summary>
        /// <value>The URI property.</value>
        public Uri UriProperty { get; set; }
    }
}
