// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// XamlBaseFrame class
    /// </summary>
    public class XamlBaseFrame
    {
        /// <summary>
        /// local Prefixes
        /// </summary>
        private Dictionary<string, string> _localPrefixes;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlBaseFrame"/> class.
        /// </summary>
        public XamlBaseFrame()
        {
            Previous = null;
            CurrentType = null;
            CurrentProperty = null;
        }

        /*
        public ReaderFrame(ReaderFrame previous, List<string> prefixList)
        {
            _localPrefixes = new List<string>(prefixList);
            _previous = previous;
            _currentType = null;
            _currentProperty = null;
        }
         */

        /// <summary>
        /// Gets or sets the type of the current.
        /// </summary>
        /// <value>The type of the current.</value>
        public XamlType CurrentType { get; set; }

        /// <summary>
        /// Gets or sets the current property.
        /// </summary>
        /// <value>The current property.</value>
        public XamlMember CurrentProperty { get; set; }

        /// <summary>
        /// Gets the local prefixes.
        /// </summary>
        /// <value>The local prefixes.</value>
        public Dictionary<string, string> LocalPrefixes
        {
            get
            {
                if (_localPrefixes == null)
                {
                    _localPrefixes = new Dictionary<string, string>();
                }

                return _localPrefixes;
            }
        }

        /// <summary>
        /// Gets or sets the previous.
        /// </summary>
        /// <value>The previous.</value>
        public XamlBaseFrame Previous { get; set; }
    }
}
