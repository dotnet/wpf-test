// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Markup;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.Xaml.Utilities;
    using Microsoft.Test.Xaml.XamlReaderWriterTests;
    
    public class ClassTests
    {
        #region TypeWithVariousNumberOfPropertiesTest

        private List<Type> _typeWithVariousNumberOfPropertiesTestTypes;

        private List<Type> TypeWithVariousNumberOfPropertiesTestTypes
        {
            get
            {
                if (this._typeWithVariousNumberOfPropertiesTestTypes == null)
                {
                    this._typeWithVariousNumberOfPropertiesTestTypes = new List<Type>();
                    this._typeWithVariousNumberOfPropertiesTestTypes.Add(typeof(ClassType));
                    this._typeWithVariousNumberOfPropertiesTestTypes.Add(typeof(ClassType1));
                    this._typeWithVariousNumberOfPropertiesTestTypes.Add(typeof(ClassType2));
                    this._typeWithVariousNumberOfPropertiesTestTypes.Add(typeof(ClassType3));
                    this._typeWithVariousNumberOfPropertiesTestTypes.Add(typeof(ClassType4));
                    this._typeWithVariousNumberOfPropertiesTestTypes.Add(typeof(ClassType5));
                    this._typeWithVariousNumberOfPropertiesTestTypes.Add(typeof(ClassType8));
                    this._typeWithVariousNumberOfPropertiesTestTypes.Add(typeof(TypeInRootNamespace));
                }
                return this._typeWithVariousNumberOfPropertiesTestTypes;
            }
        }

        [TestCaseGenerator()]
        public void TypeWithVariousNumberOfPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.TypeWithVariousNumberOfPropertiesTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeWithVariousNumberOfPropertiesTestMethod(string instanceID)
        {
            // object comparison of System.Type fails through roundtrips, this instance needs to be manually verified
            if (instanceID == "Microsoft.Test.Xaml.Types.ClassType8.Instance1")
            {
                var instance = (PropertyDefinition)XamlTestDriver.GetInstance(this.TypeWithVariousNumberOfPropertiesTestTypes, instanceID).Target;
                var after = (PropertyDefinition)XamlTestDriver.Roundtrip(instance);

                XamlObjectComparer.CompareObjects(instance.Modifier, after.Modifier);
                XamlObjectComparer.CompareObjects(instance.Name, after.Name);

                if (instance.Attributes.Count != after.Attributes.Count)
                {
                    throw new DataTestException("Counts don't match");
                }

                for (int i = 0; i < instance.Attributes.Count; i++)
                {
                    if (!instance.Attributes[i].Equals(after.Attributes[i]))
                    {
                        throw new DataTestException("Attributes don't match");
                    }
                }

            }
            else
            {
                XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.TypeWithVariousNumberOfPropertiesTestTypes, instanceID));
            }
        }

        #endregion

        #region NestedTypeTest

        private List<Type> _nestedTypeTestTypes;

        private List<Type> NestedTypeTestTypes
        {
            get
            {
                if (this._nestedTypeTestTypes == null)
                {
                    this._nestedTypeTestTypes = new List<Type>();
                    this._nestedTypeTestTypes.Add(typeof(ClassType_Nested1.NestedType1));
                    this._nestedTypeTestTypes.Add(typeof(ClassType_Nested1.NestedType2));
                    this._nestedTypeTestTypes.Add(typeof(ClassType_Nested1.NestedType3));
                    this._nestedTypeTestTypes.Add(typeof(ClassType_Nested1.NestedType4.NestedNestedType1));
                    //this.nestedTypeTestTypes.Add(typeof(ClassType_Nested1.NestedType4.NestedNestedType2));
                    this._nestedTypeTestTypes.Add(typeof(ClassType_Nested2));
                }
                return this._nestedTypeTestTypes;
            }
        }

        [TestCaseGenerator()]
        public void NestedTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.NestedTypeTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void NestedTypeTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.NestedTypeTestTypes, instanceID));
        }

        #endregion

        #region TypeWithDifferentAccessModifier

        [TestCaseGenerator]
        public void TypeWithDifferentAccessModifier(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._typeWithDifferentAccessModifierTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeWithDifferentAccessModifierMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._typeWithDifferentAccessModifierTypes, instanceID));
        }

        private readonly List<Type> _typeWithDifferentAccessModifierTypes = new List<Type>
                                                                               {
                                                                                   typeof (ClassType6)
                                                                               };

        #endregion

        #region StaticTypeAndPropertyTest

        [TestCaseGenerator]
        public void StaticTypeAndPropertyTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._staticTypeAndPropertyTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void StaticTypeAndPropertyTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._staticTypeAndPropertyTestTypes, instanceID));
        }

        private readonly List<Type> _staticTypeAndPropertyTestTypes = new List<Type>
                                                                         {
                                                                             typeof (ClassType7)
                                                                         };

        #endregion

        #region TypeComposistionTest

        [TestCaseGenerator]
        public void TypeComposistionTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._typeComposistionTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeComposistionTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._typeComposistionTestTypes, instanceID));
        }

        private readonly List<Type> _typeComposistionTestTypes = new List<Type>
                                                                    {
                                                                        typeof (Composition10)
                                                                    };

        #endregion

        #region TypeInheritanceTest

        private List<Type> _typeInheritanceTestTypes;

        private List<Type> TypeInheritanceTestTypes
        {
            get
            {
                if (this._typeInheritanceTestTypes == null)
                {
                    this._typeInheritanceTestTypes = new List<Type>();
                    this._typeInheritanceTestTypes.Add(typeof(ClassType_Inheritance2));
                    this._typeInheritanceTestTypes.Add(typeof(ClassType_Inheritance3));
                    this._typeInheritanceTestTypes.Add(typeof(ClassType_Inheritance5));
                    this._typeInheritanceTestTypes.Add(typeof(ClassType_Inheritance6));
                }
                return _typeInheritanceTestTypes;
            }
        }

        [TestCaseGenerator]
        public void TypeInheritanceTest(AddTestCaseEventHandler addCase)
        {
            //Type with different depth of inheritance
            XamlTestDriver.GenerateTestCases(addCase, this.TypeInheritanceTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeInheritanceTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.TypeInheritanceTestTypes, instanceID));
        }

        #endregion

        [TestCase]
        public void XmlReaderRepro7396Test()
        {
            ClassType5 data = new ClassType5();
            using (MemoryStream stream = new MemoryStream())
            {
                XamlTestDriver.Serialize(data, stream);
                stream.Position = 0;
                XmlDocument objectDoc = new XmlDocument();
                objectDoc.Load(stream);

                string xmlDoc =
                    String.Format(CultureInfo.InvariantCulture, @"<?xml version=""1.0"" encoding=""utf-8""?><Root>{0}</Root>",
                                  objectDoc.DocumentElement.OuterXml);

                using (XmlReader reader = XmlReader.Create(new StringReader(xmlDoc)))
                {
                    reader.MoveToContent();
                    reader.ReadStartElement("Root");
                    reader.MoveToContent();

                    ClassType5 data2 = (ClassType5)XamlTestDriver.Deserialize(reader.ReadSubtree());
                    CompareResult result = TreeComparer.CompareLogical(data, data2);
                    if (result != CompareResult.Equivalent)
                    {
                        throw new DataTestException("Objects didn't match.");
                    }
                }
            }
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.Functional)]
        public void XmlReaderRepro26022Test()
        {
            int index = -1;
            try
            {
                for (int i = 0; i < Trace.Listeners.Count; i++)
                {
                    if (Trace.Listeners[i].GetType() ==
                        typeof(System.Diagnostics.DefaultTraceListener))
                    {
                        index = i;
                    }
                }

                if (index != -1)
                {
                    Trace.Listeners.RemoveAt(index);
                }

                ClassType5 data = new ClassType5();
                using (MemoryStream stream = new MemoryStream())
                {
                    XamlTestDriver.Serialize(data, stream);
                    stream.Position = 0;
                    XmlDocument objectDoc = new XmlDocument();
                    objectDoc.Load(stream);

                    string xmlDoc =
                        String.Format(CultureInfo.InvariantCulture, @"<?xml version=""1.0"" encoding=""utf-8""?><Root>{0}</Root>",
                                      objectDoc.DocumentElement.OuterXml);

                    using (XmlReader reader = XmlReader.Create(new StringReader(xmlDoc)))
                    {
                        reader.MoveToContent();
                        reader.ReadStartElement("Root");
                        reader.MoveToContent();

                        ExceptionHelpers.CheckForException(typeof(System.Xaml.XamlParseException),
                                                           () => XamlTestDriver.Deserialize(reader));
                    }
                }
            }
            finally
            {
                if (index != -1)
                {
                    Trace.Listeners.Add(new System.Diagnostics.DefaultTraceListener());
                }
            }
        }

        public struct NestedStruct
        {
            public struct InnerStruct
            {
                public int[] varrint;

                public InnerStruct(bool set)
                {
                    varrint = new int[5]
                                  {
                                      0, 1, 2, 3, 4
                                  };
                }
            }
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void NestedStructTest()
        {
            Microsoft.Test.Xaml.XamlTests.ClassTests.NestedStruct.InnerStruct foo = new Microsoft.Test.Xaml.XamlTests.ClassTests.NestedStruct.InnerStruct(true);
            string msg = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml);
            XamlTestDriver.RoundTripCompare(foo, msg);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void NonPublicType()
        {
            XamlTestDriver.RoundTripCompare(new NonPublicPerson("hello"),
                                    Exceptions.GetMessage("ObjectReader_TypeNotVisible", WpfBinaries.SystemXaml));
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void ImplicitCollection()
        {
            Bar bar = new Bar();
            bar.Add(1);
            bar.Add(2);
            bar.Add(3);

            XamlTestDriver.RoundTripCompare(bar);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void AtomInSequence()
        {
            List<object> foo = new List<object>()
                                   {
                                       new object(),
                                       "Hello world",
                                       new TimeSpan(),
                                   };

            // note that "Hello world" is an atom in between a list of records //
            string xaml = @"<List Capacity='4'
            xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
            xmlns:s='clr-namespace:System;assembly=mscorlib' 
            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
            x:TypeArguments='s:Object' >
              <s:Object />Hello world<s:TimeSpan>00:00:00</s:TimeSpan>
            </List>";

            XamlTestDriver.XamlFirstCompareObjects(xaml, foo);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void ImplicitDictionaryPositive()
        {
            MyDictionary2<int, string> obj = new MyDictionary2<int, string>();
            obj.Add(4, "5");
            obj.Add(int.MaxValue, "Hello world");

            XamlTestDriver.RoundTripCompare(obj);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void ShouldSerialize()
        {
            var obj = new ShouldSerializeSample()
                          {
                              StringProperty = "foobar",
                              StringProperty1 = "imlost"
                          };

            string xaml = XamlServices.Save(obj);
            if (xaml.Contains("imlost"))
            {
                throw new TestCaseFailedException("Should serialize was false but property was serialized");
            }
            if (!xaml.Contains("foobar"))
            {
                throw new TestCaseFailedException("Should serialize was true but property not serialized");
            }
        }

        [TestCase]
        public void ShouldSerializeArray()
        {
            var obj = new ShouldSerializeArray();

            string xaml = XamlServices.Save(obj);
            if (xaml.Contains("StringArrayProperty"))
            {
                throw new TestCaseFailedException("Should serialize was false but property was serialized");
            }
        }

        [TestCase]
        public void ShouldSerializeList()
        {
            var obj = new ShouldSerializeList();

            string xaml = XamlServices.Save(obj);
            if (xaml.Contains("StringListProperty"))
            {
                throw new TestCaseFailedException("Should serialize was false but property was serialized");
            }
        }

        [TestCase]
        public void ShouldSerializeDictionary()
        {
            var obj = new ShouldSerializeDictionary();

            string xaml = XamlServices.Save(obj);
            if (xaml.Contains("DictionaryProperty"))
            {
                throw new TestCaseFailedException("Should serialize was false but property was serialized");
            }
        }

        [TestCase]
        public void DeserializeXClass()
        {
            NodeList doc = InitializedClass("MyClass");
            string xaml = doc.NodeListToXaml();
            Tracer.LogTrace(xaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), Exceptions.GetMessage("XClassMustMatchRootInstance", WpfBinaries.SystemXaml), () => XamlServices.Parse(xaml));
        }

        [TestCase]
        public void EmptyStringInList()
        {
            var list = new List<string>();
            list.Add(string.Empty);
            list.Add("Hello");
            list.Add(string.Empty);

            XamlTestDriver.RoundTripCompare(list);
        }

        /// <summary>
        /// Verify write only properties are set on read
        /// </summary>
        [TestCase]
        public void WriteOnlyProperty()
        {
            string xaml = "<WriteOnlyPropertyClass Data='blah' xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'/>";

            var result = (WriteOnlyPropertyClass)XamlServices.Parse(xaml);
            if (result.GetData() != "blah")
            {
                throw new DataTestException("Expected: blah Actual: " + result.GetData());
            }
        }

        public static NodeList InitializedClass(string xClassName)
        {
            return new NodeList()
            {
                new NamespaceNode("x2", Namespaces.NamespaceV2),
                new StartObject(typeof(ClassType2)),
                    new StartMember(XamlLanguage.Class),
                        new ValueNode(xClassName),
                    new EndMember(),
                    new StartMember(XamlLanguage.ClassModifier),
                        new ValueNode("public"),
                    new EndMember(),
                    new StartMember(typeof(ClassType2), "Category"),
                        new StartObject(typeof(ClassType1)),
                            new StartMember(typeof(ClassType1), "Category"),
                                new ValueNode("Some category"),
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };
        }
    }

    internal class NonPublicPerson
    {
        private readonly string _name;

        public NonPublicPerson(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
