// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xaml;
using System.Xaml.Schema;

namespace Microsoft.Test.Xaml
{
    /// <summary>
    /// Extension Methods
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Gets the xamlType.
        /// </summary>
        /// <param name="schemaContext">The schema context.</param>
        /// <param name="xamlNamespace">The xaml namespace.</param>
        /// <param name="name">The name  .</param>
        /// <returns> XamlType value </returns>
        internal static XamlType GetXamlType(
            this XamlSchemaContext schemaContext, string xamlNamespace, string name)
        {
            XamlTypeName typeName = new XamlTypeName(xamlNamespace, name);
            return schemaContext.GetXamlType(typeName);
        }
    }
}
