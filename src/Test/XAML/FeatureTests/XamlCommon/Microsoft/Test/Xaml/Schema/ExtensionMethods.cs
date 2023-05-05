// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Xaml.Schema
{
    using System.Xaml;
    using System.Xaml.Schema;

    /// <summary>
    /// Container class for extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Convert XamlTypeName object tree to string.
        /// </summary>
        /// <param name="xamlTypeName">XamlTypeName instance to serialize</param>
        /// <returns>Serialized string.</returns>
        public static string Serialize(this XamlTypeName xamlTypeName)
        {
            string name = xamlTypeName.Name ?? "[null]";
            string ns = xamlTypeName.Namespace ?? "[null]";
            string typeArguments = null;
            if (xamlTypeName.TypeArguments != null && xamlTypeName.TypeArguments.Count != 0)
            {
                int index;
                for (index = 0; index < xamlTypeName.TypeArguments.Count - 1; index++)
                {
                    typeArguments += xamlTypeName.TypeArguments[index].Serialize() + ",";
                }

                typeArguments += xamlTypeName.TypeArguments[index].Serialize();
            }

            string ret = name;
            if (ns != string.Empty)
            {
                ret = ns + ":" + ret;
            }

            if (typeArguments != null)
            {
                ret = ret + "(" + typeArguments + ")";
            }

            return ret;
        }
    }
}
