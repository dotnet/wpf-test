// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// UPAObject Test class
    /// </summary>
    [UidProperty("ObjectProperty")]
    public class UPAObject : CustomUDIObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPAObject"/> class.
        /// </summary>
        public UPAObject()
        {
        }

        /// <summary>
        /// Gets or sets the object property.
        /// </summary>
        /// <value>The object property.</value>
        public object ObjectProperty { get; set; }
    }
}
