// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

namespace Microsoft.Test.Xaml.Schema
{
    /*
     * Verify that XmlnsPrefix rule2 works across assemblies
     */
    [SchemaTest]
    class MultipleAssembliesRule2 : GetPreferredPrefixTest
    {
        public MultipleAssembliesRule2() : base("http://xmlns1")
        {
            //Load AssemblyA and AssemblyB
            ClassA classA = new ClassA();
            ClassB classB = new ClassB();
            classA = null;
            classB = null;

            ExpectedPrefix = "prefix1";
        }
    }
}

