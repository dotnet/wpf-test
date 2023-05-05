// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// Custom type for the single ambient property scenario
    /// </summary>
    public class AmbientSingleProp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientSingleProp"/> class.
        /// </summary>
        public AmbientSingleProp()
        {
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text value.</value>
        [Ambient]
        public string Text { get; set; }
    }

    /// <summary>
    /// Custom type for the single ambient collection scenario
    /// </summary>
    public class AmbientSingleCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientSingleCollection"/> class.
        /// </summary>
        public AmbientSingleCollection()
        {
            ObjectCollection = new Collection<object>();
        }

        /// <summary>
        /// Gets or sets the object collection.
        /// </summary>
        /// <value>The object collection.</value>
        [Ambient]
        public Collection<object> ObjectCollection { get; set; }
    }
}
