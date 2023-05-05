// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;

[assembly:XmlnsDefinition("http://b", "AssemblyB")]
[assembly:XmlnsCompatibleWith("http://a", "http://b")]

namespace AssemblyB
{
    public class TypeB
    {
    }
}
