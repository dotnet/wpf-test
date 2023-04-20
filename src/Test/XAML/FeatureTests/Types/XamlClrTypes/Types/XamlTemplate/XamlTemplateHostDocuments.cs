// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.XamlTemplate
{
    using System;
    using System.Xaml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    
    public static class XamlTemplateHostDocuments
    {
        public static NodeList GetHostDocument(Type hostType, string typeArguments, string templateMemberName, NodeList template)
        {
            return new NodeList()
            {   
                new NamespaceNode("x2", Namespaces.NamespaceBuiltinTypes),
                new NamespaceNode("types", "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes"),
                new NamespaceNode("ixml", "clr-namespace:Microsoft.Test.Xaml.Types.IXmlSerializableTypes;assembly=XamlClrTypes"),
                new NamespaceNode("sc", "clr-namespace:System.Collections;assembly=mscorlib"),
                new NamespaceNode("scg", "clr-namespace:System.Collections.Generic;assembly=mscorlib"),
                new NamespaceNode("s", "clr-namespace:System;assembly=mscorlib"),
                new StartObject(hostType),
                    new StartMember(XamlLanguage.TypeArguments){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, true}}},
                        new ValueNode(typeArguments),
                    new EndMember(),
                    new StartMember(hostType, "IntData"),
                        new ValueNode(42),
                    new EndMember(),
                    GetClassData(hostType),
                    new StartMember(hostType, templateMemberName),
                        template,
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetHostDocumentNoTypeArgs(Type hostType, string templateMemberName, NodeList template)
        {
            return new NodeList()
            {
                new NamespaceNode("x2", Namespaces.NamespaceBuiltinTypes),
                new NamespaceNode("types", "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes"),
                new NamespaceNode("sc", "clr-namespace:System.Collections;assembly=mscorlib"),
                new NamespaceNode("scg", "clr-namespace:System.Collections.Generic;assembly=mscorlib"),
                new NamespaceNode("s", "clr-namespace:System;assembly=mscorlib"),
                new StartObject(hostType),  
                    new StartMember(hostType, "IntData"),
                        new ValueNode(42),
                    new EndMember(),
                    GetClassData(hostType),
                    new StartMember(hostType, templateMemberName),
                        template,
                    new EndMember(),

                new EndObject(),
            };
        }

        private static NodeList GetClassData(Type type)
        {
            return new NodeList()
            {
                new StartMember(type, "ClassData"),
                    new StartObject(typeof(ClassType2)),
                        new StartMember(typeof(ClassType2), "Category"),
                            new StartObject(typeof(ClassType1)),
                                new StartMember(typeof(ClassType1), "Category"),
                                    new ValueNode("Some category"),
                                new EndMember(),
                            new EndObject(),
                        new EndMember(),
                    new EndObject(),
                new EndMember(),
            };
        }
    }
}
