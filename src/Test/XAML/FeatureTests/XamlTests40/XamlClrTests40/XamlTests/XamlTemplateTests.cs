// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Builders;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Comparers;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.Xaml.Types.IXmlSerializableTypes;
    using Microsoft.Test.Xaml.Types.XamlTemplate;
    using XT = Microsoft.Test.Xaml.Types.XamlTemplate;
    
    public class XamlTemplateTests
    {
        private class XamlTemplateTarget
        {
            public Type HostType { get; set; }
            public string HostParamType { get; set; }
            public object TemplateContent { get; set; }
        }

        private readonly List<Type> _hostTypes = new List<Type>
                                                      {
                                                          typeof(FuncHost<>),
                                                          typeof(FactoryHost<>),
                                                          typeof(FactoryHostWContentType<>),
                                                          typeof(ClassHost<>),
                                                          typeof(DerivedClassHost<>),
                                                          //   



                                                      };

        private ICollection<TestCaseInfo> GetTestCases(List<KeyValuePair<string, object>> typesAndValues)
        {
            ICollection<TestCaseInfo> testCases = (from hostType in _hostTypes
                                                   from typeAndValue in typesAndValues
                                                   select new TestCaseInfo
                                                              {
                                                                  Target = new XamlTemplateTarget
                                                                               {
                                                                                   HostType = hostType,
                                                                                   HostParamType = typeAndValue.Key,
                                                                                   TemplateContent = typeAndValue.Value
                                                                               },
                                                                  TestID = hostType.Name + "+" + typeAndValue.Key
                                                              }).ToList();

            foreach (TestCaseInfo test in testCases)
            {
                XamlTemplateTarget target = (XamlTemplateTarget)test.Target;
                // [DISABLED] : Known Bug / Test Case Hang ?
                if (test.TestID.Contains("GenericType6"))
                {
                    test.BugNumber = 0;
                }

                if (test.TestID.Contains("scg:List(x2:String)") ||
                    test.TestID.Contains("s:Array(x2:String"))
                {
                    test.BugNumber = 0;
                }
            }

            return testCases;
        }

        private readonly List<KeyValuePair<string, object>> _primitiveTypes = new List<KeyValuePair<string, object>>
                                                                                 {
                                                                                     new KeyValuePair<string, object>("x2:Int32", 42),
                                                                                     new KeyValuePair<string, object>("x2:Boolean", true),
                                                                                     new KeyValuePair<string, object>("s:DateTime", DateTime.Now),
                                                                                     new KeyValuePair<string, object>("s:DateTimeOffset", DateTimeOffset.Now),
                                                                                     new KeyValuePair<string, object>("s:TimeSpan", TimeSpan.MinValue),
                                                                                     new KeyValuePair<string, object>("x2:String", "Hello"),
                                                                                     new KeyValuePair<string, object>("x2:Double", 3.14),
                                                                                     new KeyValuePair<string, object>("s:Guid", Guid.Empty),
                                                                                 };

        [TestCaseGenerator]
        public void PrimitiveTemplateTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, GetTestCases(_primitiveTypes), MethodInfo.GetCurrentMethod());
        }

        public void PrimitiveTemplateTypeTestMethod(string instanceID)
        {
            RunTest(XamlTestDriver.GetInstance(GetTestCases(_primitiveTypes), instanceID));
        }

        private readonly List<KeyValuePair<string, object>> _classTypes = new List<KeyValuePair<string, object>>
                                                                             {
                                                                                 new KeyValuePair<string, object>("types:ClassType2", new ClassType2
                                                                                                                                            {
                                                                                                                                                Category = new ClassType1
                                                                                                                                                               {
                                                                                                                                                                   Category = "SomeCategory"
                                                                                                                                                               }
                                                                                                                                            }),
                                                                                 new KeyValuePair<string, object>("types:ClassType1", new ClassType1
                                                                                                                                            {
                                                                                                                                                Category = "SomeCategory"
                                                                                                                                            }),
                                                                                 new KeyValuePair<string, object>("types:ClassType3", null),
                                                                                 // [DISABLED]
                                                                                //  new KeyValuePair<string, object>("types:GenericType6(x2:String)", new GenericType6<string>
                                                                                //                                                                               {
                                                                                //                                                                                   DerivedInfo = "Something", Info1 = 42, Info2 = "else"
                                                                                //                                                                               }),
                                                                             };

        [TestCaseGenerator]
        public void ClassTemplateTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, GetTestCases(_classTypes), MethodInfo.GetCurrentMethod());
        }

        public void ClassTemplateTypeTestMethod(string instanceID)
        {
            RunTest(XamlTestDriver.GetInstance(GetTestCases(_classTypes), instanceID));
        }

        private readonly List<KeyValuePair<string, object>> _collectionTypes = new List<KeyValuePair<string, object>>
                                                                                  {
                                                                                      new KeyValuePair<string, object>("scg:List(x2:String)",
                                                                                                                       new List<string>
                                                                                                                           {
                                                                                                                               String.Empty,
                                                                                                                               null,
                                                                                                                               "stuff"
                                                                                                                           }),
                                                                                      new KeyValuePair<string, object>("s:Array(x2:String)",
                                                                                                                       new string[]
                                                                                                                           {
                                                                                                                               String.Empty,
                                                                                                                               null,
                                                                                                                               "stuff"
                                                                                                                           }),
                                                                                      new KeyValuePair<string, object>("sc:ArrayList",
                                                                                                                       new ArrayList
                                                                                                                           {
                                                                                                                               new ClassType1
                                                                                                                                   {
                                                                                                                                       Category = "category"
                                                                                                                                   },
                                                                                                                               null,
                                                                                                                               "stuff"
                                                                                                                           }),
                                                                                      new KeyValuePair<string, object>("scg:Dictionary(x2:String, x2:Int32)",
                                                                                                                       new Dictionary<string, int>
                                                                                                                           {
                                                                                                                               {
                                                                                                                                   String.Empty, int.MaxValue
                                                                                                                                   },
                                                                                                                               {
                                                                                                                                   "stuff", int.MinValue
                                                                                                                                   },
                                                                                                                               {
                                                                                                                                   "more stuff", 42
                                                                                                                                   }
                                                                                                                           }),
                                                                                  };

        // [DISABLED]
        // [TestCaseGenerator]
        public void CollectionTemplateTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, GetTestCases(_collectionTypes), MethodInfo.GetCurrentMethod());
        }

        public void CollectionTemplateTypeTestMethod(string instanceID)
        {
            RunTest(XamlTestDriver.GetInstance(GetTestCases(_collectionTypes), instanceID));
        }

        private readonly List<KeyValuePair<string, object>> _atomTypes = new List<KeyValuePair<string, object>>
                                                                            {
                                                                                new KeyValuePair<string, object>("x2:String", "some value")
                                                                            };

        public void AtomTemplateTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, GetTestCases(_atomTypes), MethodInfo.GetCurrentMethod());
        }

        public void AtomTemplateTypeTestMethod(string instanceID)
        {
            XamlTemplateTarget target = XamlTestDriver.GetInstance(GetTestCases(_atomTypes), instanceID).Target as XamlTemplateTarget;

            if (target == null)
            {
                throw new DataTestException("Invalid target for test case.  Expected XamlTemplateTarget.");
            }


            NodeList doc = XamlTemplateHostDocuments.GetHostDocument(target.HostType,
                                                                         target.HostParamType,
                                                                         "Template",
                                                                         new NodeList()
                                                                         {
                                                                             new ValueNode(target.TemplateContent),
                                                                         });

            string xaml = doc.NodeListToXml();
            Verify(xaml, target.TemplateContent);
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void CollectionOfPrimitiveTemplatesTest()
        {
            NodeList doc = XamlTemplateHostDocuments.GetHostDocument(typeof(ListClassHost<>),
                                                                         "x2:String",
                                                                         "Templates",
                                                                         GetTemplateXaml(null));

            string xaml = doc.NodeListToXml();

            xaml = xaml.Replace("<x2:NullExtension />", "<x2:String>hello</x2:String><x2:String>there</x2:String>");

            // Templates need to work on a singular property.  
            // Not as one of many items in a collection. 
            try
            {
                ListClassHost<string> host = (ListClassHost<string>)XamlServices.Parse(xaml);

                Compare(host.Templates[0].Evaluate(), "hello");
                Compare(host.Templates[1].Evaluate(), "there");

                string xaml2 = XamlServices.Save(host);
                ListClassHost<string> host2 = (ListClassHost<string>)XamlServices.Parse(xaml2);

                Compare(host2.Templates[0].Evaluate(), "hello");
                Compare(host2.Templates[1].Evaluate(), "there");
            }
            catch (XamlObjectWriterException)
            {
                return;
            }
            throw new TestCaseFailedException("Expected exception not caught");
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void CollectionOfClassTemplatesTest()
        {
            NodeList doc = XamlTemplateHostDocuments.GetHostDocument(typeof(ListClassHost<>),
                                                                         "types:ClassType2",
                                                                         "Templates",
                                                                         GetTemplateXaml(null));

            string xaml = doc.NodeListToXml();
            ClassType2 firstElement = new ClassType2
                                          {
                                              Category = new ClassType1
                                                             {
                                                                 Category = "category1"
                                                             }
                                          };

            ClassType2 secondElement = new ClassType2
                                           {
                                               Category = new ClassType1
                                                              {
                                                                  Category = "category2"
                                                              }
                                           };

            xaml = xaml.Replace("<x2:NullExtension />", XamlServices.Save(firstElement) + XamlServices.Save(secondElement));

            // Templates need to work on a singular property.  
            // Not as one of many items in a collection. 
            try
            {
                ListClassHost<ClassType2> host = (ListClassHost<ClassType2>)XamlServices.Parse(xaml);

                Compare(host.Templates[0].Evaluate(), firstElement);
                Compare(host.Templates[1].Evaluate(), secondElement);

                string xaml2 = XamlServices.Save(host);
                ListClassHost<ClassType2> host2 = (ListClassHost<ClassType2>)XamlServices.Parse(xaml2);

                Compare(host2.Templates[0].Evaluate(), firstElement);
                Compare(host2.Templates[1].Evaluate(), secondElement);
            }
            catch (XamlObjectWriterException)
            {
                return;
            }
            throw new TestCaseFailedException("Expected exception not caught");
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void NestedTemplateTest()
        {
            //ClassHost<ClassHost<string>>

            #region Document

            NodeList templateContent = new NodeList()
            {
                new NamespaceNode("x2", Namespaces.NamespaceBuiltinTypes),
                new StartObject(typeof(ClassHost<string>)),
                    new StartMember(XamlLanguage.TypeArguments){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, true}}},
                        new ValueNode("x2:String"),
                    new EndMember(),
                    new StartMember(typeof(ClassHost<string>), "Template"),
                        new StartObject(XamlLanguage.String),
                            new StartMember(XamlLanguage.Initialization),
                                new ValueNode("hello"),
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };
            NodeList doc = XamlTemplateHostDocuments.GetHostDocument(typeof(ClassHost<string>),
                                                                     "ClassHost(x2:String)",
                                                                     "Template",
                                                                     templateContent);


            #endregion

            string xaml = doc.NodeListToXml();

            ClassHost<ClassHost<string>> host = (ClassHost<ClassHost<string>>)XamlServices.Parse(xaml);
            string value = host.Template.Evaluate().Template.Evaluate();
            Compare(value, "hello");

            ClassHost<ClassHost<string>> host2 = (ClassHost<ClassHost<string>>)XamlServices.Parse(XamlServices.Save(host));

            value = host.Template.Evaluate().Template.Evaluate();
            Compare(value, "hello");
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void IXmlSerializableTemplateTest()
        {
            NodeList doc = XamlTemplateHostDocuments.GetHostDocument(typeof(ClassHost<>),
                                                                         "ixml:TypeContaingingIXmlSerializableProperty",
                                                                         "Template",
                                                                         GetTemplateXaml(null));

            string xaml = doc.NodeListToXml();

            TypeContaingingIXmlSerializableProperty original = new TypeContaingingIXmlSerializableProperty();
            string data = @"<?Processing Inst?>
                    <!-- some comments -->
                    <some xmlns=""boo""><xml isparsed=""true""/></some>";

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Auto
            };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                original.IxmlProperty.ReadXml(reader);
            }

            xaml = xaml.Replace("<x2:NullExtension />", XamlServices.Save(original));
            Tracer.LogTrace(xaml);

            ClassHost<TypeContaingingIXmlSerializableProperty> host = (ClassHost<TypeContaingingIXmlSerializableProperty>)XamlServices.Parse(xaml);

            Compare(host.Template.Evaluate(), original);
        }

        [TestCase(
             Owner = "Microsoft",
             Category = TestCategory.IDW,
             TestType = TestType.Automated
             )]
        public void NullTemplateTest()
        {
            FactoryHostWContentType<object> obj = new FactoryHostWContentType<object>();
            string xaml = XamlServices.Save(obj);
        }

        [TestCase(
             Owner = "Microsoft",
             Category = TestCategory.IDW,
             TestType = TestType.Automated
             )]
        public void MarkupExtensionTemplateTest()
        {
            NodeList doc = XamlTemplateHostDocuments.GetHostDocument(typeof(ClassHost<>),
                                                                         "SimpleMEClassContainer",
                                                                         "Template",
                                                                         GetTemplateXaml(null));

            string xaml = doc.NodeListToXml();

            SimpleMEClassContainer original = new SimpleMEClassContainer()
                                                  {
                                                      SimpleMEClass = new SimpleMEClass()
                                                                          {
                                                                              Address = "city1",
                                                                              AptNo = 210
                                                                          }
                                                  };

            xaml = xaml.Replace("<x2:NullExtension />", XamlServices.Save(original));
            ClassHost<SimpleMEClassContainer> host = (ClassHost<SimpleMEClassContainer>)XamlServices.Parse(xaml);

            SimpleMEClassContainer container1 = host.Template.Evaluate();
            SimpleMEClassContainer container2 = host.Template.Evaluate();

            Compare(container1, original);
            Compare(container2, original);
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void TypeConverterTemplateTest()
        {
            Zoo1 zoo = new Zoo1
                           {
                               Animals = new List<Animal>
                                             {
                                                 new Animal
                                                     {
                                                         Name = "Billy",
                                                         Number = 3
                                                     },
                                                 new Animal
                                                     {
                                                         Name = "Tommy",
                                                         Number = 4
                                                     }
                                             }
                           };

            NodeList doc = XamlTemplateHostDocuments.GetHostDocument(typeof(ClassHost<Zoo1>),
                                                                         "types:Zoo1",
                                                                         "Template",
                                                                         GetTemplateXaml(null));

            string xaml = doc.NodeListToXml();

            xaml = xaml.Replace("<x2:NullExtension />", XamlServices.Save(zoo));
            ClassHost<Zoo1> host = (ClassHost<Zoo1>)XamlServices.Parse(xaml);

            Compare(zoo, host.Template.Evaluate());

            ClassHost<Zoo1> host2 = (ClassHost<Zoo1>)XamlServices.Parse(XamlServices.Save(host));
            Compare(zoo, host2.Template.Evaluate());
        }

        //[TestCase(
        //    Owner = "Microsoft",
        //    Category = TestCategory.IDW,
        //    TestType = TestType.Automated
        //    )]
        //public void ReferenceOutOfTemplate()
        //{
        //    NodeList templateContent = new NodeList()
        //    {
        //        new StartObject(XamlLanguage.Reference),
        //            new StartMember(XamlLanguage.Initialization),
        //                new Value("SomeReference"),
        //            new EndMember(),
        //        new EndObject(),
        //    };
        //    NodeList doc = XamlTemplateHostDocuments.GetHostDocument(typeof(ClassHost<>),
        //                                                                 "prefix1:ClassType2",
        //                                                                 "Template",
        //                                                                 templateContent);

        //    GraphNodeMember classData = (GraphNodeMember) (doc.Root.Children.First((node) => node is GraphNodeMember && ((GraphNodeMember) node).MemberName == "ClassData"));
        //    classData.Children[0].Children.Add(
        //        new GraphNodeMember
        //            {
        //                TypeName = Constants.Directive2006Type,
        //                MemberName = "Name",
        //                MemberType = MemberType.Directive,
        //                Children =
        //                    {
        //                        new GraphNodeAtom
        //                            {
        //                                Value = "SomeReference"
        //                            }
        //                    }
        //            });

        //    string xaml;
        //    using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
        //    {
        //        doc.Save(new WrappedXmlWriter(writer));
        //        xaml = writer.ToString();
        //    }

        //    ClassHost<ClassType2> host = (ClassHost<ClassType2>) XamlServices.Parse(xaml);
        //    if (host.ClassData != host.Template.Evaluate())
        //    {
        //        throw new DataTestException("Expected references to be equal");
        //    }
        //}

        //[TestCase(
        //    Owner = "Microsoft",
        //    Category = TestCategory.IDW,
        //    TestType = TestType.Automated
        //    )]
        //public void ReferenceIntoTemplate()
        //{
        //    XamlDocument doc = XamlTemplateHostDocuments.GetHostDocument("ClassHost",
        //                                                                 "prefix1:ClassType2",
        //                                                                 "Template",
        //                                                                 GetTemplateXaml(new ClassType2
        //                                                                                     {
        //                                                                                         Category = new ClassType1
        //                                                                                                        {
        //                                                                                                            Category = "Stuff"
        //                                                                                                        }
        //                                                                                     }));

        //    GraphNodeMember classData = (GraphNodeMember) (doc.Root.Children.First((node) => node is GraphNodeMember && ((GraphNodeMember) node).MemberName == "ClassData"));
        //    classData.Children[0] = new GraphNodeRecord
        //                                {
        //                                    RecordName = XName.Get("Reference", Namespaces.Namespace2006),
        //                                    Children =
        //                                        {
        //                                            new GraphNodeMember
        //                                                {
        //                                                    MemberName = null,
        //                                                    Children =
        //                                                        {
        //                                                            new GraphNodeAtom
        //                                                                {
        //                                                                    Value = "SomeReference"
        //                                                                }
        //                                                        }
        //                                                }
        //                                        }
        //                                };

        //    GraphNodeMember templateData = (GraphNodeMember) (doc.Root.Children.First((node) => node is GraphNodeMember && ((GraphNodeMember) node).MemberName == "Template"));

        //    templateData.Children[0].Children.Add(
        //        new GraphNodeMember
        //            {
        //                TypeName = Constants.Directive2006Type,
        //                MemberName = "Name",
        //                MemberType = MemberType.Directive,
        //                Children =
        //                    {
        //                        new GraphNodeAtom
        //                            {
        //                                Value = "SomeReference"
        //                            }
        //                    }
        //            });

        //    string xaml;
        //    using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
        //    {
        //        doc.Save(new WrappedXmlWriter(writer));
        //        xaml = writer.ToString();
        //    }

        //    try
        //    {
        //        ClassHost<ClassType2> host = (ClassHost<ClassType2>) XamlServices.Parse(xaml);
        //        throw new DataTestException("Expected unable to find reference exception.  Actually suceeded");
        //    }
        //    catch (XamlObjectWriterException xowe)
        //    {
        //        string expected = Exceptions.GetMessage("UnresolvedForwardReferences", WpfBinaries.SystemXaml);
        //        if (!Helper.IsExceptionStringMatch(expected, xowe.Message))
        //        {
        //            throw new DataTestException("Unexpected message.  \r\nExpected: " + expected + "\r\nActual:  " + xowe.Message);
        //        }
        //    }
        //}

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void OverridenXamlTemplate()
        {
            NodeList doc = XamlTemplateHostDocuments.GetHostDocument(typeof(OverridenClassHost<String>),
                                                                         "x2:String",
                                                                         "Template",
                                                                         GetTemplateXaml("stuff"));

            string xaml = doc.NodeListToXml();

            OverridenClassHost<string> host = (OverridenClassHost<string>)XamlServices.Parse(xaml);

            try
            {
                host.Template.Evaluate();
                throw new DataTestException("Expected exception.  Actually succeeded.");
            }
            catch (NotSupportedException)
            {
            }
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void OverridenXamlTemplateWithTypeConverter()
        {
            NodeList doc = XamlTemplateHostDocuments.GetHostDocumentNoTypeArgs(typeof(PointXamlTemplateWrapper),
                                                                                   "Point",
                                                                                   new NodeList()
                                                                                   {
                                                                                       new StartObject(typeof(XT.PointWithXamlTemplate)),
                                                                                        new StartMember(typeof(XT.PointWithXamlTemplate), "X"),
                                                                                            new ValueNode(42),
                                                                                        new EndMember(),
                                                                                        new StartMember(typeof(XT.PointWithXamlTemplate), "Y"),
                                                                                            new ValueNode(3),
                                                                                        new EndMember(),
                                                                                       new EndObject(),
                                                                                   });

            string xaml = doc.NodeListToXml();

            var host = XamlServices.Parse(xaml);
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void OverridenTypeConverterWithXamlTemplate()
        {
            NodeList doc = XamlTemplateHostDocuments.GetHostDocumentNoTypeArgs(typeof(PointInstanceDescriptorWrapper),
                                                                                   "Point",
                                                                                   new NodeList()
                                                                                   {
                                                                                       new StartObject(typeof(PointWithInstanceDescriptor)),
                                                                                        new StartMember(typeof(PointWithInstanceDescriptor), "X"),
                                                                                            new ValueNode(42),
                                                                                        new EndMember(),
                                                                                        new StartMember(typeof(PointWithInstanceDescriptor), "Y"),
                                                                                            new ValueNode(3),
                                                                                        new EndMember(),
                                                                                       new EndObject(),
                                                                                   });

            string xaml = doc.NodeListToXml();

            try
            {
                var host = XamlServices.Parse(xaml);
                throw new DataTestException("Expected exception.  Actually succeeded.");
            }
            catch (XamlObjectWriterException)
            {
            }
        }

        public static void RunTest(TestCaseInfo info)
        {
            XamlTemplateTarget target = info.Target as XamlTemplateTarget;

            if (target == null)
            {
                throw new DataTestException("Invalid target for test case.  Expected XamlTemplateTarget.  Got " + info.Target.GetType().Name);
            }

            string xaml = BuildXaml(target);
            Tracer.LogTrace(xaml);
            Verify(xaml, target.TemplateContent);
        }

        private static string BuildXaml(XamlTemplateTarget target)
        {
            string xaml;
            if (target.TemplateContent is Dictionary<string, int>)
            {
                NodeList doc = XamlTemplateHostDocuments.GetHostDocument(target.HostType,
                                                                             target.HostParamType,
                                                                             "Template",
                                                                             GetTemplateXaml(null));

                xaml = doc.NodeListToXml();

                string dictionary = XamlServices.Save(target.TemplateContent);
                xaml = xaml.Replace("<x2:NullExtension />", dictionary);
            }
            else
            {
                NodeList doc = XamlTemplateHostDocuments.GetHostDocument(target.HostType,
                                                                             target.HostParamType,
                                                                             "Template",
                                                                             GetTemplateXaml(target.TemplateContent));

                xaml = doc.NodeListToXml();
            }

            Tracer.LogTrace("Template xaml = " + xaml);
            return xaml;
        }

        private static void Verify(string xaml, object templateContent)
        {
            var host = XamlServices.Parse(xaml);
            object template1 = XamlHostBase.GetTemplateValue(host);
            object template2 = XamlHostBase.GetTemplateValue(host);

            string xaml2 = XamlServices.Save(host);
            var host2 = XamlServices.Parse(xaml2);
            object template3 = XamlHostBase.GetTemplateValue(host);

            Compare(((XamlHostBase)host).IntData, ((XamlHostBase)host2).IntData);
            Compare(((XamlHostBase)host).ClassData, ((XamlHostBase)host2).ClassData);
            Compare(templateContent, template1);
            Compare(template1, template2);
            Compare(template2, template3);
        }

        private static void Compare(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
            {
                return;
            }

            if (obj1 == null || obj2 == null)
            {
                throw new DataTestException("Only one was null.");
            }

            GraphCompareResults results = ObjectGraphComparer.XamlCompare(obj1, obj2);
            if (!results.Passed)
            {
                foreach (CompareError error in results.Errors)
                {
                    Tracer.Trace("Template compare", error.Error.Message);
                }
                throw new DataTestException("Compare failed.");
            }
        }

        private static NodeList GetTemplateXaml(object template)
        {
            if (template == null)
            {
                return new NodeList()
                {
                    new StartObject(XamlLanguage.Null),
                    new EndObject(),
                };
            }

            return NodeListTransforms.ObjectToNodeList(template);
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void XamlTemplateOnPrimitiveTypeTest()
        {
            var before = new HostWithTemplateOnPrimitive
                             {
                                 ClassData = new ClassType2
                                                 {
                                                     Category = new ClassType1
                                                                    {
                                                                        Category = "blah"
                                                                    }
                                                 },
                                 IntData = int.MaxValue
                             };

            var after = XamlTestDriver.RoundtripAndCompareObjects(before);
            XamlTestDriver.RoundtripAndCompareObjects(after);
        }
    }

    public class HostWithTemplateOnPrimitive
    {
        [System.Windows.Markup.XamlDeferLoad(typeof(XamlTemplateForPrimitiveTypeConverter), typeof(ClassType2))]
        public ClassType2 ClassData { get; set; }

        [System.Windows.Markup.XamlDeferLoad(typeof(XamlTemplateForPrimitiveTypeConverter), typeof(int))]
        public int IntData { get; set; }
    }

    public class XamlTemplateForPrimitiveTypeConverter : XamlDeferringLoader
    {
        public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            return XamlServices.Load(xamlReader);
        }

        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            return new XamlXmlReader(XmlTextReader.Create(new StringReader(XamlServices.Save(value))));
        }
    }
}
