// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types.Repro
{
    /// <summary>
    /// InheritedContent Type1
    /// </summary>
    [ContentProperty("Content")]
    public class InheritedContentType1
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value.</value>
        public string Name { get; set; }
    }

    /// <summary>
    /// InheritedContent Type2
    /// </summary>
    [ContentProperty("Content")]
    public class InheritedContentType2 : InheritedContentType1
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public new object Content { get; set; }
    }

    /// <summary>
    /// InheritedContent Type3
    /// </summary>
    [ContentProperty("Content")]
    public class InheritedContentType3 : InheritedContentType1
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public new string Content { get; set; }
    }
}
