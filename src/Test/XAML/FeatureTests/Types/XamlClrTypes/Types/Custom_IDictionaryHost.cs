// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom IDictionaryHost
    /// </summary>
    public class Custom_IDictionaryHost
    {
        /// <summary>
        /// Custom IDictionary
        /// </summary>
        private readonly Custom_IDictionary _dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_IDictionaryHost"/> class.
        /// </summary>
        public Custom_IDictionaryHost()
        {
            _dictionary = new Custom_IDictionary();
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        public Custom_IDictionary Dictionary
        {
            get
            {
                return _dictionary;
            }
        }
    }
}
