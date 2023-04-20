// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Xaml.Schema
{
    using System.Collections.Generic;
    using System.Xaml;

    /// <summary>
    /// A very simple IXamlNamespaceResolver.
    /// </summary>
    public class CustomXamlNamespaceResolver : IXamlNamespaceResolver
    {
        /// <summary>
        /// IXamlNamespace implementation
        /// </summary>
        /// <param name="prefix">Xmlns prefix.</param>
        /// <returns>Xaml namespaces corresponding to prefix.</returns>
        public string GetNamespace(string prefix)
        {
            return prefix;
        }

        /// <summary>
        /// Gets the namespace prefixes.
        /// </summary>
        /// <returns>IEnumerable NamespaceDeclaration </returns>
        public IEnumerable<NamespaceDeclaration> GetNamespacePrefixes()
        {
            return new List<NamespaceDeclaration>();
        }
    }
}
