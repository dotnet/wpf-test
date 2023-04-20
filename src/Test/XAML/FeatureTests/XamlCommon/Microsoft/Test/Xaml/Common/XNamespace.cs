// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// XNamespace equivalent so that we dont have to 
    /// use the Linq dll which is not in the client sku
    /// </summary>
    public class XNamespace
    {
        /// <summary>
        /// Gets or sets the namespace name
        /// </summary>
        public string NamespaceName { get; set; }

        /// <summary>
        /// Get the XNamespace for the given string namespace
        /// </summary>
        /// <param name="namespaceName">namespace name</param>
        /// <returns>XNamespace instance</returns>
        public static XNamespace Get(string namespaceName)
        {
            XNamespace namesp = new XNamespace() { NamespaceName = namespaceName };
            return namesp;
        }
        
        /// <summary>
        /// implicit operator
        /// </summary>
        /// <param name="namespaceName">namespace name</param>
        /// <returns>gets the XNamespace</returns>
        [CLSCompliant(false)]
        public static implicit operator XNamespace(string namespaceName)
        {
            if (namespaceName == null)
            {
                return null;
            }

            return Get(namespaceName);
        }
    }
}
