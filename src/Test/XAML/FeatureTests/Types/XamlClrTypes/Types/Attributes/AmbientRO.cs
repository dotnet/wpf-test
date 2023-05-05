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
    /// Custom type for the ambient Read-Only Property scenario
    /// </summary>
    public class AmbientROProp
    {
        /// <summary>
        /// DateTime created
        /// </summary>
        private readonly DateTime _created;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientROProp"/> class.
        /// </summary>
        public AmbientROProp()
        {
            _created = DateTime.Now;
        }

        /// <summary>
        /// Gets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        [Ambient]
        public DateTime CreationTime
        {
            get
            {
                return _created;
            }
        }
    }

    /// <summary>
    /// Custom type for the ambient Read-Only collection scenario
    /// </summary>
    public class AmbientROCollection
    {
        /// <summary>
        /// String Collection
        /// </summary>
        private readonly Collection<string> _strCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientROCollection"/> class.
        /// </summary>
        public AmbientROCollection()
        {
            _strCollection = new Collection<string>();
        }

        /// <summary>
        /// Gets the string collection.
        /// </summary>
        /// <value>The string collection.</value>
        [Ambient]
        public Collection<string> StringCollection
        {
            get
            {
                return _strCollection;
            }
        }
    }
}
