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
    /// Custom_Clr_RNPA_DKPA class
    /// </summary>
    [RuntimeNameProperty("Name")]
    [DictionaryKeyProperty("Key")]
    public class Custom_Clr_RNPA_DKPA : Custom_Clr_StringID
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_RNPA_DKPA"/> class.
        /// </summary>
        public Custom_Clr_RNPA_DKPA()
        {
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key value.</value>
        public string Key { get; set; }
    }
}
