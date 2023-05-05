// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom Root class
    /// </summary>
    [ContentProperty("Content")]
    [RuntimeNameProperty("Name")]
    public class CustomRoot
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
    /// CustomRootWithCollection class
    /// </summary>
    [ContentProperty("Content")]
    [RuntimeNameProperty("Name")]
    public class CustomRootWithCollection
    {
        /// <summary>List of content nodes </summary>
        private readonly List<object> _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRootWithCollection"/> class.
        /// </summary>
        public CustomRootWithCollection()
        {
            _content = new List<object>();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public List<object> Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value.</value>
        public string Name { get; set; }
    }
}
