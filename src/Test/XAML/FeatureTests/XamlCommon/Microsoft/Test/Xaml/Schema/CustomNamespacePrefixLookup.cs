// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema
{
    using System.Xaml;

    /// <summary>
    /// A very simple NamespacePrefixLookup service provider
    /// </summary>
    public class CustomNamespacePrefixLookup : INamespacePrefixLookup
    {
        /// <summary>
        /// Return prefix for a given namespace.
        /// </summary>
        /// <param name="ns">Input namespace</param>
        /// <returns>Prefix string.</returns>
        public string LookupPrefix(string ns)
        {
           // Return namespace string itself as prefix to enable roundtrip scenario
           return ns;
        }
    }
}
