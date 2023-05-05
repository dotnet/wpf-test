// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Xaml.Common;

    /// <summary>
    /// Set of commonly used constants
    /// </summary>
    public static class Namespaces
    {
        /// <summary>
        /// 2006 Xaml Namespace
        /// </summary>
        public const string Namespace2006 = "http://schemas.microsoft.com/winfx/2006/xaml";

        /// <summary>
        /// Map 2008 namespace to 2006 namespace until 2009 namespace is available //
        ///  public const string NamespaceV2 = "http://schemas.microsoft.com/netfx/2008/xaml"; 
        /// </summary>
        public const string NamespaceV2 = Namespace2006;

        /// <summary>
        /// Map 2008 namespace to 2006 namespace until 2009 namespace is available // 
        /// </summary>
        public const string NamespaceBuiltinTypes = Namespace2006;

        /// <summary>
        /// Serialization schmea target namespace
        /// </summary>
        public const string SerializationSchemaTargetNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";

        /// <summary>
        /// Create an XName from the given CLR type
        /// </summary>
        /// <param name="type">The clr type</param>
        /// <returns>XName instance</returns>
        public static XName GetXNameFromType(Type type)
        {
            string namespaceFormat =
                "clr-namespace:{0};assembly={1}";

            string typeName = type.Name;

            // strip off the generic stuff (ie GenericTypeName`2)
            if (typeName.Contains("`"))
            {
                typeName = typeName.Substring(0, typeName.IndexOf('`') - 1);
            }

            XName name = XName.Get(typeName, String.Format(CultureInfo.InvariantCulture, namespaceFormat, type.Namespace, type.Assembly.GetName().Name));
            return name;
        }
    }
}
