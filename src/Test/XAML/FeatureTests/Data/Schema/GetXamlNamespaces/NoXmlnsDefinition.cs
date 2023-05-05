// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

namespace Microsoft.Test.Xaml.Schema
{
    [SchemaTest]
    class NoXmlnsDefinition : GetXamlNamespacesTest
    {
        public NoXmlnsDefinition() : base(typeof(FooType)) { }

        protected override void SetExpectedNamespacesOverride()
        {
            //Nothing expected except clr-namespace uri
        }
    }
}

