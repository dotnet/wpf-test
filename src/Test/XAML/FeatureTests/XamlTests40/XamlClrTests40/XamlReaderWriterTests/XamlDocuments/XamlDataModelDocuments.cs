// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Types.AttachedProperties;
    using Microsoft.Test.Xaml.Types.ContentProperties;

    public class XamlDataModelDocuments
    {
        public static NodeList GetDuplicateMemberDoc(string name, bool useAttributes)
        {
            return new NodeList(name)
                    {
                        new StartObject(typeof(Point)),
                            new StartMember(typeof(Point), "X"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                                new ValueNode(42),
                            new EndMember(),
                            new StartMember(typeof(Point), "X"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                                new ValueNode(3),
                            new EndMember(),
                        new EndObject(),
                    };
        }

        public static NodeList GetMemberDifferentTypeDoc(string name, bool useAttributes)
        {
            return new NodeList(name)
            {
                new NamespaceNode("ap",  "clr-namespace:Microsoft.Test.Xaml.Types.AttachedProperties;assembly=XamlClrTypes"),
                new StartObject(typeof(Point)),
                    new StartMember(typeof(AttachedPropertySource), "BoolProp"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("True"),
                    new EndMember(),
                    new StartMember(typeof(Point), "X"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode(42),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode(3),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetUnnamedMemberDoc(string name)
        {
            return new NodeList(name)
            {
                new StartObject(typeof(StringContentProperty)),
                    new StartMember(typeof(StringContentProperty), "ContentProperty"){ TestMetadata = {{ NodeListXmlWriter.SkipWritingTagProperty, true}}},
                        new ValueNode("some string"),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetMultipleMembersDoc(string name, bool useAttributes)
        {
            return new NodeList(name)
            {
                new StartObject(typeof(Point)),
                    new StartMember(typeof(Point), "X"){ TestMetadata = {{ NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode(42),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"){ TestMetadata = {{ NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode(3),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetMixNamedUnnamedMembersDoc(string name, bool useAttributes)
        {
            return new NodeList(name)
            {
                new NamespaceNode("as", Namespaces.GetXNameFromType(typeof(AttachedPropertySource)).Namespace.NamespaceName),
                new StartObject(typeof(StringContentProperty)),
                    new StartMember(typeof(AttachedPropertySource), "BoolProp"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("True"),
                    new EndMember(),
                    new StartMember(typeof(AttachedPropertySource), "StringProp"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("Some String"),
                    new EndMember(),
                    new StartMember(typeof(StringContentProperty), "ContentProperty"){ TestMetadata = {{NodeListXmlWriter.SkipWritingTagProperty, true}}},
                        new ValueNode("Some Content"),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static string GetDtdDocument()
        {
            return @"<?xml version=""1.0"" encoding=""UTF-16""?>
                    <! DOCTYPE Array [
                        <!ENTITY entity1 ""This is an entity."">
                    ]>
                    <x:Array xmlns:r=""clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes"" 
                             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                             xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
                      <x:Array.Type>r:ClassType1</x:Array.Type>
                      <r:ClassType1><r:ClassType1.Category>blah&entity1;</r:ClassType1.Category></r:ClassType1>
                    </x:Array>";
        }
    }
}
