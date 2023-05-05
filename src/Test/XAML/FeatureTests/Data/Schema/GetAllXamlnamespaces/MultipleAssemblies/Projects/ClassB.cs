// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;

[assembly: XmlnsDefinition("http://xmlns1", "NamespaceA1")]
[assembly: XmlnsDefinition("http://xmlns1", "NamespaceA2")]
[assembly: XmlnsDefinition("http://xmlns2", "NamespaceA1")]
[assembly: XmlnsDefinition("http://xmlns4", "NamespaceB1")]

namespace NamespaceB1
{
    public class B1
    {
        public B1()
        {
        }
    }
}
