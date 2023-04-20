// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

namespace Microsoft.Test.Xaml.Schema
{
    /*
     * Verify that XmlnsDefinitions dont leak across assemblies
     */
    [SchemaTest]
    class MultipleAssemblies : GetXamlNamespacesTest
    {
        public MultipleAssemblies() : base(typeof(ClassB))
        {
            ClassA classA = new ClassA();
            ClassB classB = new ClassB();
            ClassC classC = new ClassC();
            classA = null;
            classB = null;
            classC = null;
        }

        protected override void SetExpectedNamespacesOverride()
        {
            ExpectedNamespaces.Add("http://xmlnsB");
        }
    }
}

