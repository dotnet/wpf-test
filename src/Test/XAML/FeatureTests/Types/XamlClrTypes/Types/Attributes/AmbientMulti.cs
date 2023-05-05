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
    /// Custom type for the Multiple ambient property scenario
    /// </summary>
    public class AmbientMultiProp : AmbientSingleProp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientMultiProp"/> class.
        /// </summary>
        public AmbientMultiProp()
        {
        }

        /// <summary>
        /// Gets or sets the num.
        /// </summary>
        /// <value>The num value.</value>
        [Ambient]
        public int Num { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name  value.</value>
        public string Name { get; set; }
    }

    /// <summary>
    /// Custom type for the Multiple ambient collection scenario
    /// </summary>
    public class AmbientMultiCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientMultiCollection"/> class.
        /// </summary>
        public AmbientMultiCollection()
        {
            ObjectCollection = new Collection<object>();
            StringCollection = new Collection<string>();
        }

        /// <summary>
        /// Gets or sets the object collection.
        /// </summary>
        /// <value>The object collection.</value>
        [Ambient]
        public Collection<object> ObjectCollection { get; set; }

        /// <summary>
        /// Gets or sets the string collection.
        /// </summary>
        /// <value>The string collection.</value>
        [Ambient]
        public Collection<string> StringCollection { get; set; }
    }
}
