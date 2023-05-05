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
    /// Custom IXSCPPropertyObject
    /// </summary>
    [TypeConverter(typeof(Custom_IXamlSchemaContextProviderConverter))]
    public class Custom_IXSCPPropertyObject
    {
        /// <summary>
        /// Gets or sets the schema context.
        /// </summary>
        /// <value>The schema context.</value>
        public XamlSchemaContext SchemaContext { get; set; }
    }

    /// <summary>
    /// Custom IXSCPObject
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_IXSCPObject
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public Custom_IXSCPPropertyObject Content { get; set; }
    }
}
