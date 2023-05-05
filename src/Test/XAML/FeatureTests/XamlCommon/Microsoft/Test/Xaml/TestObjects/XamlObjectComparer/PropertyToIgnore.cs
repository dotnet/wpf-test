// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer
{
    /// <summary>
    /// Property to Ignore
    /// </summary>
    public class PropertyToIgnore
    {
        /// <summary>
        /// Initializes a new instance of the PropertyToIgnore class.
        /// </summary>
        public PropertyToIgnore()
        {
            this.Owner = null;
        }

        /// <summary>
        /// Gets or sets the Owner property
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets the WhatToIgnore property
        /// </summary>
        public IgnoreProperty WhatToIgnore { get; set; }
    }
}
