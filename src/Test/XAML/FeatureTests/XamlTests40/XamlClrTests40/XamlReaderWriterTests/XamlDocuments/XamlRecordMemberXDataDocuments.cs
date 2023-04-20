// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments
{
    using System.Collections;
    using System.IO;
    using System.Windows.Markup;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Types.ContentProperties;
    using Microsoft.Test.Xaml.Types.IXmlSerializableTypes;

    internal class XamlRecordMemberXDataDocuments
    {
        public static NodeList GetXDataDocument(string name, bool use2008Namespace)
        {
            string xml = @"<IXmlSerializable xmlns='clr-namespace:System.Xml.Serialization;assembly=System.Xml'>blah</IXmlSerializable>";
            XmlReader reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Auto
            });

            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(TypeContaingingIXmlSerializableProperty)),
                    new StartMember(typeof(TypeContaingingIXmlSerializableProperty), "IxmlProperty"),
                        new ValueNode(reader),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetMixedValues(string name, bool contentProperty)
        {
            XmlReader reader = XmlReader.Create(new StringReader("blah"), new XmlReaderSettings()
                                                                              {
                                                                                  ConformanceLevel = ConformanceLevel.Auto
                                                                              });
            return new NodeList(name)
            {
                new StartObject(typeof(ArrayListContentProperty)),
                    new StartMember(typeof(ArrayListContentProperty), "ContentProperty"){ TestMetadata = {{NodeListXmlWriter.SkipWritingTagProperty, contentProperty}}},
                        new StartObject(typeof(ArrayList)),
                            new StartMember(XamlLanguage.Items),
                                new ValueNode("blah"),
                                GetPoint(false),
                                new ValueNode("blah2"),
                                GetPoint(false),
                                new ValueNode(reader),
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetMixedValuesNoXData(string name, bool contentProperty)
        {
            string contentPropertyName = contentProperty ? null : "ContentProperty";
            XmlReader reader = XmlReader.Create(new StringReader("blah"), new XmlReaderSettings()
                                                                              {
                                                                                  ConformanceLevel = ConformanceLevel.Auto
                                                                              });
            return new NodeList(name)
            {
                new StartObject(typeof(ArrayListContentProperty)),
                    new StartMember(typeof(ArrayListContentProperty), contentPropertyName),
                        new ValueNode("blah"),
                        GetPoint(false),
                        new ValueNode("blah2"),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetInterspesedMember(string name)
        {
            return new NodeList(name)
            {
                new StartObject(typeof(InterspersedMemberType)),
                    new StartMember(typeof(InterspersedMemberType), "ContentProperty"),
                        new ValueNode("blah"),
                        new StartMember(typeof(InterspersedMemberType), "Data"),
                            new ValueNode("This should not be here"),
                        new EndMember(),
                        new ValueNode("blah2"),
                        GetPoint(false),
                    new EndMember(),
                    
                new EndObject(),
            };
        }

        public static NodeList GetPoint(bool useAttributes)
        {
            return new NodeList("Point")
            {
                new StartObject(typeof(Point)),
                    new StartMember(typeof(Point), "X"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode(42),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode(3),
                    new EndMember(),
                new EndObject(),
            };
        }
    }

    [ContentProperty("ContentProperty")]
    public class InterspersedMemberType
    {
        private readonly ArrayList _contentProperty = new ArrayList();
        public ArrayList ContentProperty
        {
            get
            {
                return _contentProperty;
            }
        }

        public string Data { get; set; }
    }
}
