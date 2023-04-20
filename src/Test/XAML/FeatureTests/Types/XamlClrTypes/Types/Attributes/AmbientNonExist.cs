// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// Custom type for the non-existent ambient property scenario
    /// </summary>
    /// [AmbientProperty("NonExistentProperty")]  no longer possible to do this.
    public class AmbientNonExist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientNonExist"/> class.
        /// </summary>
        public AmbientNonExist()
        {
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text value.</value>
        public string Text { get; set; }
    }
}
