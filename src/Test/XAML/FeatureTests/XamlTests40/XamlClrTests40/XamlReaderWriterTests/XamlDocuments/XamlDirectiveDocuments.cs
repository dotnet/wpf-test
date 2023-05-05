// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Markup;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Types.ContentProperties;

    internal class XamlDirectiveDocuments
    {
        public static NodeList GetNamedRecord(string name, bool useAttribute, bool use2008Namespace)
        {
            NodeList child = new NodeList("child") { new ValueNode("MyName") };
            return GetNamedPoint(name, child, useAttribute, use2008Namespace);
        }

        public static NodeList GetEmptyName(string name, bool useAttribute, bool use2008Namespace)
        {
            NodeList child = new NodeList()
            {
                new ValueNode("") { TestMetadata = {{NodeListXmlWriter.SkipWritingTagProperty, !useAttribute}}},
            };
            return GetNamedPoint(name, child, useAttribute, use2008Namespace);
        }

        public static NodeList GetNullName(string name, bool use2008Namespace)
        {
            NodeList child = new NodeList()
            {
                new StartObject(typeof(NullExtension)),
                new EndObject(),
            };
            return GetNamedPoint(name, child, false, use2008Namespace);
        }

        public static NodeList GetPointName(string name, bool use2008Namespace)
        {
            return GetNamedPoint(name, XamlRecordMemberXDataDocuments.GetPoint(false), false, use2008Namespace);
        }

        public static NodeList GetNamedPoint(string name, NodeList nameChild, bool useAttribute, bool use2008Namespace)
        {
            return new NodeList(name)
            {
                new NamespaceNode("dir", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                    new StartMember(XamlLanguage.Name){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttribute}}},
                        nameChild,
                    new EndMember(),
                    new StartMember(typeof(Point), "X"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttribute}} },
                        new ValueNode(42),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttribute}} },
                        new ValueNode(3),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetReferencedRecord(string name, bool useAttributes, bool explicitMemberName)
        {
            NodeList reference = new NodeList(name)
            {
                new StartObject(XamlLanguage.Reference),
                    new StartMember(XamlLanguage.Name)
                    {
                        TestMetadata = 
                        {
                            {NodeListXmlWriter.WriteAsAttributeProperty, useAttributes},
                            {NodeListXmlWriter.SkipWritingTagProperty, explicitMemberName}
                        }
                    },
                        new ValueNode("MyName"),
                    new EndMember(),
                new EndObject(),
            };

            return GetReferencedRecord(name, reference, useAttributes);
        }

        public static NodeList GetReferencedRecord(string name, NodeList referenceRecord, bool useAttributes)
        {
            return new NodeList(name)
            {
                new StartObject(typeof(ArrayListContentProperty)),
                    new StartMember(typeof(ArrayListContentProperty), "ContentProperty") { TestMetadata = {{NodeListXmlWriter.SkipWritingTagProperty, true}}},
                        new GetObject(),
                            new StartMember(XamlLanguage.Items),
                                GetNamedRecord("NamedRecord", useAttributes, false),
                                referenceRecord,
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetReferencedRecordWithInvalidMember(bool useAttributes)
        {
            NodeList reference = new NodeList()
            {
                new StartObject(XamlLanguage.Reference),
                    new StartMember(XamlLanguage.Reference.UnderlyingType, "SomeRandomName") { TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("MyName"),
                    new EndMember(),
                new EndObject(),
            };
            return GetReferencedRecord("somename", reference, useAttributes);
        }

        public static NodeList GetCreateFromRecord(string name, bool useAttributes)
        {
            return new NodeList(name)
            {
                new NamespaceNode("xaml", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                    new StartMember(XamlLanguage.FactoryMethod){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("urn:somerandomuri"),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetMethodRecord(string name, bool useAttributes, int args)
        {
            NodeList doc = new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                    new StartMember(XamlLanguage.FactoryMethod){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("Point.CreatePoint"),
                    new EndMember(),
                    CreateArgument(args),
                new EndObject(),
            };

            return doc;
        }

        public static NodeList GetCtorRecord(string name, int args)
        {
            NodeList doc = new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                    CreateArgument(args),
                new EndObject(),
            };

            return doc;
        }

        public static NodeList CreateArgument(int argCount)
        {
            NodeList members = NodeList.Empty;
            if (argCount != 0)
            {
                members.Add(new StartMember(XamlLanguage.Arguments));
                for (int i = 0; i < argCount; i++)
                {
                    NodeList arg = new NodeList()
                    {
                        new StartObject(XamlLanguage.Int32),
                            new StartMember(XamlLanguage.Initialization),
                                new ValueNode(i),
                            new EndMember(),
                        new EndObject(),
                    };

                    members.Add(arg);
                }
                members.Add(new EndMember());
            }

            return members;
        }

        public static NodeList GetCreateFromAndMethodRecord(string name, bool useAttributes)
        {
            NodeList arguments = CreateArgument(2);

            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                    new StartMember(XamlLanguage.FactoryMethod){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("Point.CreatePoint"),
                    new EndMember(),
                    arguments,
                new EndObject(),
            };
        }

        public static NodeList GetCreateFromAndArgumentRecord(string name, bool useAttributes)
        {
            NodeList arguments = CreateArgument(2);

            return new NodeList(name)
            {
                new NamespaceNode("ns", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                    new StartMember(XamlLanguage.FactoryMethod){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("urn:somerandomuri"),
                    new EndMember(),
                    arguments,
                new EndObject(),
            };
        }

        public static NodeList GetKey(string name, bool useAttributes, bool use2008Namespace, NodeList value)
        {
            return new NodeList(name)
            {
                new StartMember(XamlLanguage.Key){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                    value,
                new EndMember(),
            };
        }

        public static NodeList GetDictionaryDocument(string name, bool useAttributes, bool use2008Namespace)
        {
            NodeList value1 = new NodeList("value1")
            {
                new StartObject(typeof(string)),
                    new StartMember(XamlLanguage.Initialization),
                        new ValueNode("Some value"),
                    new EndMember(),
                    GetKey("Key", useAttributes, use2008Namespace, new NodeList(){new ValueNode("Some key")}),
                new EndObject(),
            };


            NodeList value2 = new NodeList("Point")
            {
                new StartObject(typeof(Point)),
                    new StartMember(typeof(Point), "X"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode(42),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode(3),
                    new EndMember(),
                    GetKey("Key", false, use2008Namespace, XamlRecordMemberXDataDocuments.GetPoint(useAttributes)),
                new EndObject(),
            };

            NodeList value3 = new NodeList("value3")
            {
                new StartObject(typeof(NullExtension)),
                    GetKey("Key", useAttributes, use2008Namespace, new NodeList
                                                             {
                                                                new ValueNode("Some key"),
                                                             }),
                new EndObject(),
            };

            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(Container)),
                    new StartMember(typeof(Container), "Stuff"),
                        new GetObject(),
                            new StartMember(XamlLanguage.Items),
                                value1,
                                value2,
                                value3,
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetKeyAtRoot(string name, bool use2008Namespace)
        {
            NodeList root = new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                    GetKey("Key", false, use2008Namespace, new NodeList(){new ValueNode("SomeKey")}),
                    new StartMember(typeof(Point), "X"),
                        new ValueNode(42),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"),
                        new ValueNode(3),
                    new EndMember(),
                new EndObject(),
            };

            return root;
        }

        public static NodeList GetInvalidKeyDoc(string name, bool use2008Namespace)
        {
            NodeList value1 = new NodeList()
            {
                 new StartObject(typeof(Point)),
                    new StartMember(typeof(Point), "X"),
                        new ValueNode(42),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"),
                        new ValueNode(3),
                    new EndMember(),
                    GetKey("Key", false, use2008Namespace, XamlRecordMemberXDataDocuments.GetPoint(false)),
                new EndObject(),
            };

            NodeList value2 = XamlRecordMemberXDataDocuments.GetPoint(false);

            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(Container)),
                    new StartMember(typeof(Container), "Stuff"),
                        new GetObject(),
                            new StartMember(XamlLanguage.Items),
                                new ValueNode("some value"),
                                value1,
                                value2,
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetTypeDoc(string name, bool useAttributes, bool use2008Namespace, bool useExplicit)
        {
            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceBuiltinTypes),
                new StartObject(typeof(TypeExtension)),
                    new StartMember(typeof(TypeExtension), "TypeName"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("x2:String"),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetTypeArgDoc(string name, bool useAttributes, bool use2006Namespace)
        {
            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceBuiltinTypes),
                new StartObject(typeof(GenericClass)),
                    new StartMember(XamlLanguage.TypeArguments)
                    { 
                        TestMetadata =
                        {
                            {NodeListXmlWriter.WriteAsAttributeProperty, useAttributes},
                            {NodeListXmlWriter.DoNotExpectTagProperty, true},
                        }
                    },
                        new ValueNode("x2:Int32")
                        { 
                            TestMetadata = 
                            {
                                {NodeListXmlWriter.DoNotExpectTagProperty, true},
                            }
                        },
                    new EndMember() { TestMetadata = {{NodeListXmlWriter.DoNotExpectTagProperty, true}}} ,
                    new StartMember(typeof(GenericClass<int>), "Stuff"),
                        new ValueNode(42),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetArrayDoc(string name, bool useAttributes, bool use2008Namespace, bool useExplicit)
        {
            // namespace 2008 is unused currently //
            XamlType arrayType = XamlLanguage.Array;

            return new NodeList(name)
            {
               new StartObject(arrayType),
                    new StartMember(arrayType.GetMember("Type")){ TestMetadata = { {NodeListXmlWriter.WriteAsAttributeProperty, useAttributes} }},
                      new ValueNode("String"),
                    new EndMember(),
                    new StartMember(arrayType.GetMember("Items")){ TestMetadata = {{NodeListXmlWriter.SkipWritingTagProperty, !useExplicit}} },
                        new GetObject(),
                            new StartMember(XamlLanguage.Items),
                                new StartObject(XamlLanguage.String),
                                    new StartMember(XamlLanguage.Initialization),
                                        new ValueNode("some String"),
                                    new EndMember(),
                                new EndObject(),
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
               new EndObject(),
            };
        }

        public static NodeList GetMissingTypeArrayDoc(string name, bool use2008Namespace, bool useExplicit)
        {
            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceBuiltinTypes),
                new StartObject(typeof(ArrayExtension)),
                    new StartMember(typeof(ArrayExtension), "Items"),
                        new GetObject(),
                            new StartMember(XamlLanguage.Items){ TestMetadata = {{NodeListXmlWriter.SkipWritingTagProperty, !useExplicit }}},
                                new StartObject(XamlLanguage.String),
                                    new StartMember(XamlLanguage.Initialization),
                                        new ValueNode("some string"),
                                    new EndMember(),
                                new EndObject(),
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetOtherDirectivesDoc(string name, bool useAttributes, bool use2008Namespace, bool useExplicit)
        {
            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                    new StartMember(XamlLanguage.Class){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("SomeClass"),
                    new EndMember(),
                    new StartMember(XamlLanguage.Code){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("Some code"),
                    new EndMember(),
                    new StartMember(XamlLanguage.Subclass){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("SomeSubClass"),
                    new EndMember(),
                    new StartMember(XamlLanguage.ClassModifier){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("internal"),
                    new EndMember(),
                    new StartMember(XamlLanguage.FieldModifier){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode("internal"),
                    new EndMember(),
                    new StartMember(typeof(Point), "X"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}} },
                        new ValueNode(3),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}} },
                        new ValueNode(3),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetInvalidFieldModifierDoc(string name, bool useAttributes, bool use2008Namespace)
        {
            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                     new StartMember(XamlLanguage.FieldModifier){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}}},
                        new ValueNode(3),
                    new EndMember(),
                    new StartMember(typeof(Point), "X"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}} },
                        new ValueNode(3),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, useAttributes}} },
                        new ValueNode(3),
                    new EndMember(),
                new EndObject(),
            };
        }

        public static NodeList GetStaticDoc(string name, bool useAttributes, bool use2008Namespace, bool useExplicit)
        {
            StartMember member;
            if (useExplicit)
            {
                member = new StartMember(typeof(StaticExtension), "Member")
                            {
                                TestMetadata = 
                                {
                                    {NodeListXmlWriter.WriteAsAttributeProperty, useAttributes},
                                }
                            };
            }
            else
            {
                member = new StartMember(XamlLanguage.Initialization)
                {
                    TestMetadata = 
                                {
                                    {NodeListXmlWriter.WriteAsAttributeProperty, useAttributes},
                                    {NodeListXmlWriter.SkipWritingTagProperty, true},
                                }
                };
            }

            return new NodeList(name)
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(Point)),
                    new StartMember(typeof(Point), "X"),
                        new StartObject(typeof(StaticExtension)),
                            member,
                                new ValueNode("blah"),
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                    new StartMember(typeof(Point), "Y"),
                        new ValueNode(3),
                    new EndMember(),
                new EndObject(),
            };
        }
    }

    public class Container
    {
        private readonly Dictionary<object, object> _dict = new Dictionary<object, object>();
        public IDictionary Stuff
        {
            get
            {
                return _dict;
            }
        }
    }

    public class GenericClass
    {
    }

    public class GenericClass<T>
    {
        public T Stuff { get; set; }
    }
}
