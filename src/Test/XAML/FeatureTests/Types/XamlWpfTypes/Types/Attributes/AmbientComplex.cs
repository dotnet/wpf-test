// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// AmbientWithContent class
    /// </summary>
    [ContentProperty("Content")]
    public class AmbientWithContent : AmbientSingleProp
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; set; }
    }

    /// <summary>
    /// AmbientWithAmbientTCProp class
    /// </summary>
    public class AmbientWithAmbientTCProp : AmbientSingleProp
    {
        /// <summary>
        /// Gets or sets the ATC.
        /// </summary>
        /// <value>The ATC value.</value>
        public AmbientTC ATC { get; set; }
    }

    /// <summary>
    /// NonAmbientWithAmbientTCProp class
    /// </summary>
    public class NonAmbientWithAmbientTCProp
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text value.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the ATC.
        /// </summary>
        /// <value>The ATC value.</value>
        public AmbientTC ATC { get; set; }
    }
}
