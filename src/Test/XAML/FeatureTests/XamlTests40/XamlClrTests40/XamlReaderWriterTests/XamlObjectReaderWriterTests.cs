// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows.Markup;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.Xaml.Types.ObjectReader;
    using Microsoft.Test.Xaml.Utilities;
    using Microsoft.Test.Xaml.XamlTests;
    using Glob = Microsoft.Test.Globalization;
    
    public class XamlObjectReaderWriterTests
    {
        [TestCase]
        public void XamlObjectReaderObjectPropTest()
        {
            ItemType item1 = new ItemType { ItemName = "item1", Price = double.MaxValue };
            ItemType item2 = new ItemType { ItemName = "item2", Price = double.MinValue };
            var items = new Dictionary<ItemType, ItemType>
            {
                { item1, item2 }
            };

            var expectedObjects = new Queue<object>();
            expectedObjects.Enqueue(items);
            expectedObjects.Enqueue(item2);
            expectedObjects.Enqueue(item1);

            XamlObjectReaderObjectPropTestCore(items, expectedObjects);
        }

        [TestCase]
        public void XamlObjectReaderObjectPropWGetObjectTest()
        {
            ItemType item1 = new ItemType { ItemName = "item1", Price = double.MaxValue };
            ItemType item2 = new ItemType { ItemName = "item2", Price = double.MinValue };
            var items = new CollectionContainerType18();
            items.AddItem(item1);
            items.AddItem(item2);

            var expectedObjects = new Queue<object>();
            expectedObjects.Enqueue(items);
            expectedObjects.Enqueue(item1);
            expectedObjects.Enqueue(item2);

            XamlObjectReaderObjectPropTestCore(items, expectedObjects);
        }

        private void XamlObjectReaderObjectPropTestCore(object target, Queue<object> expectedObjects)
        {
            var reader = new XamlObjectReader(target);

            while (reader.Read())
            {
                if (reader.NodeType == XamlNodeType.StartObject)
                {
                    if (expectedObjects.Count == 0)
                    {
                        throw new DataTestException("Unexpected object found.");
                    }

                    var actualObject = expectedObjects.Dequeue();
                    if (!object.ReferenceEquals(reader.Instance, actualObject))
                    {
                        throw new DataTestException("Expected object not found.");
                    }
                }
                else
                {
                    if (reader.Instance != null)
                    {
                        throw new DataTestException("Object property should be null except for StartObject nodes");
                    }
                }
            }


            if (expectedObjects.Count > 0)
            {
                throw new DataTestException("Encountered fewer objects than expected.");
            }
        }

        [TestCase]
        public void ObjectWriterEventTest()
        {
            ItemType item1 = new ItemType { ItemName = "item1", Price = double.MaxValue };
            ItemType item2 = new ItemType { ItemName = "item2", Price = double.MinValue };
            var items = new Dictionary<ItemType, ItemType>
            {
                { item1, item2 }
            };

            ObjectWriterEventTestCore(new XamlObjectReader(items));
        }

        [TestCase]
        public void ObjectWriterEventWGetObjectTest()
        {
            var item1 = new ItemType { ItemName = "item1", Price = double.MaxValue };
            var item2 = new ItemType { ItemName = "item2", Price = double.MinValue };
            var target = new TypeWithReadOnlyCollectionAndTypeConverter
            {
                Items = { { item1, item2 } },
                Tiger = new Tiger("Billy")
            };

            ObjectWriterEventTestCore(new XamlObjectReader(target));
        }

        /// <summary>
        /// Trying to serialize an internal type without setting the LocalAssembly property should throw
        /// </summary>
        [TestCase]
        public void SaveInternalTypeNoLocalAssembly()
        {
            var target = new ClassType6 { Data = "blah" };

            string expectedMessage = Glob.Exceptions.GetMessage("ObjectReader_TypeNotVisible", Glob.WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectReaderException), expectedMessage, () => XamlServices.Save(target));
        }

        /// <summary>
        /// Serializing an internal type after setting the LocalAssembly should succeed.  This will only work in full trust.
        /// </summary>
        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void SaveInternalTypeWithLocalAssembly()
        {
            var target = new ClassType6 { Data = "blah" };

            var xxrSettings = new XamlXmlReaderSettings
            {
                LocalAssembly = typeof(ClassType6).Assembly
            };

            var xorSettings = new XamlObjectReaderSettings
            {
                LocalAssembly = typeof(ClassType6).Assembly
            };

            var xaml = new StringWriter();

            using (var xor = new XamlObjectReader(target, xorSettings))
            using (var xxw = new XamlXmlWriter(xaml, xor.SchemaContext))
            {
                XamlServices.Transform(xor, xxw);
            }

            using (var xxr = new XamlXmlReader(XmlReader.Create(new StringReader(xaml.ToString())), xxrSettings))
            using (var xow = new XamlObjectWriter(xxr.SchemaContext))
            {
                XamlServices.Transform(xxr, xow);
                XamlObjectComparer.CompareObjects(target, xow.Result);
            }
        }

        /// <summary>
        /// Regression test: "Failure to load PropertyElement on generic type with local assembly namespace"
        /// Variation 1:  Assembly specified via XamlXmlReaderSettings by specifying the type.
        /// </summary>
        [TestCase]
        public void LoadGenericLocalAssemblyInSettings()
        {
            const string xaml =
            @"<GenericType9 x:TypeArguments='x:String' xmlns='clr-namespace:Microsoft.Test.Xaml.Types' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
              <GenericType9.Value>Hello, World!</GenericType9.Value>
            </GenericType9>";

            XamlXmlReaderSettings xrs = new XamlXmlReaderSettings{ LocalAssembly = typeof(GenericType9<>).Assembly };
            var generated = (GenericType9<string>)XamlServices.Load(new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), xrs));

            Assert.AreEqual("Hello, World!", generated.Value);
        }

        /// <summary>
        /// Regression test: "Failure to load PropertyElement on generic type with local assembly namespace"
        /// Variation 2:  Assembly specified via XamlXmlReaderSettings, setting LocalAssembly using 'FindLoadedAssembly'.
        /// </summary>
        [TestCase]
        public void LoadGenericFindLoadedAssembly()
        {
            const string xaml =
            @"<GenericType9 x:TypeArguments='x:String' xmlns='clr-namespace:Microsoft.Test.Xaml.Types' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
              <GenericType9.Value>Hello, World!</GenericType9.Value>
            </GenericType9>";

            XamlXmlReaderSettings xrs = new XamlXmlReaderSettings{};
            xrs.LocalAssembly = XamlTestHelper.FindLoadedAssembly("XamlClrTypes");
            var generated = (GenericType9<string>)XamlServices.Load(new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), xrs));

            Assert.AreEqual("Hello, World!", generated.Value);
        }

        /// <summary>
        /// Regression test: "Failure to load PropertyElement on generic type with local assembly namespace"
        /// Variation 3:  Assembly specified both on the Namespace and setting LocalAssembly using 'FindLoadedAssembly'.
        /// </summary>
        [TestCase]
        public void LoadGenericFindLoadedAssemblySetInMarkup()
        {
            const string xaml =
            @"<GenericType9 x:TypeArguments='x:String' xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
              <GenericType9.Value>Hello, World!</GenericType9.Value>
            </GenericType9>";

            XamlXmlReaderSettings xrs = new XamlXmlReaderSettings{};
            xrs.LocalAssembly = XamlTestHelper.FindLoadedAssembly("XamlClrTypes");
            var generated = (GenericType9<string>)XamlServices.Load(new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), xrs));

            Assert.AreEqual("Hello, World!", generated.Value);
        }

        /// <summary>
        /// Regression test: "Failure to load PropertyElement on generic type with local assembly namespace"
        /// Variation 4:  Assembly specified multiple ways.
        /// </summary>
        [TestCase]
        public void LoadGenericInSettingsFindLoadedAssemblySetInMarkup()
        {
            const string xaml =
            @"<GenericType9 x:TypeArguments='x:String' xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
              <GenericType9.Value>Hello, World!</GenericType9.Value>
            </GenericType9>";

            XamlXmlReaderSettings xrs = new XamlXmlReaderSettings{ LocalAssembly = typeof(GenericType9<>).Assembly };
            xrs.LocalAssembly = XamlTestHelper.FindLoadedAssembly("XamlClrTypes");
            var generated = (GenericType9<string>)XamlServices.Load(new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), xrs));

            Assert.AreEqual("Hello, World!", generated.Value);
        }

        /// <summary>
        /// Regression test: "Failure to load PropertyElement on generic type with local assembly namespace"
        /// Variation 5:  Assembly specified on the Namespace only.
        /// </summary>
        [TestCase]
        public void LoadGenericAssemblySetInMarkup()
        {
            const string xaml =
            @"<GenericType9 x:TypeArguments='x:String' xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
              <GenericType9.Value>Hello, World!</GenericType9.Value>
            </GenericType9>";

            XamlXmlReaderSettings xrs = new XamlXmlReaderSettings{};
            var generated = (GenericType9<string>)XamlServices.Load(new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), xrs));

            Assert.AreEqual("Hello, World!", generated.Value);
        }

        /// <summary>
        /// Serializing a type with an internal type argument.  This will only work in full trust.
        /// </summary>
        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void SaveGenericWithInternalTypeArgsWithLocalAssembly()
        {
            var target = new GenericType1<ClassType6, ClassType6>
            {
                Info1 = new ClassType6 { Data = "stuff" },
                Info2 = new ClassType6 { Data = "stuff2" }
            };

            var xxrSettings = new XamlXmlReaderSettings
            {
                LocalAssembly = typeof(ClassType6).Assembly
            };

            var xorSettings = new XamlObjectReaderSettings
            {
                LocalAssembly = typeof(ClassType6).Assembly
            };

            var xaml = new StringWriter();

            using (var xor = new XamlObjectReader(target, xorSettings))
            using (var xxw = new XamlXmlWriter(xaml, xor.SchemaContext))
            {
                XamlServices.Transform(xor, xxw);
            }

            using (var xxr = new XamlXmlReader(XmlReader.Create(new StringReader(xaml.ToString())), xxrSettings))
            using (var xow = new XamlObjectWriter(xxr.SchemaContext))
            {
                XamlServices.Transform(xxr, xow);
                XamlObjectComparer.CompareObjects(target, xow.Result);
            }
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void SaveInternalTypeWithInternalCtorLocalAssembly()
        {
            var xorSettings = new XamlObjectReaderSettings { LocalAssembly = typeof(InternalTypeWithInternalCtor).Assembly };
            var xaml = new StringWriter();

            using (var xor = new XamlObjectReader(new InternalTypeWithInternalCtor(), xorSettings))
            using (var xxw = new XamlXmlWriter(xaml, xor.SchemaContext))
            {
                XamlServices.Transform(xor, xxw);
            }         
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void SavePublicTypeWithInternalCtorLocalAssembly()
        {
            var settings = new XamlObjectReaderSettings { LocalAssembly = typeof(PublicClassWithInternalCtor).Assembly };
            ExceptionHelper.ExpectException<XamlObjectReaderException>(() => new XamlObjectReader(new PublicClassWithInternalCtor(), settings), new XamlObjectReaderException());
        }

        [TestCase]
        public void SimpleNameConverter()
        {
            var nameElement = new NameElement();
            nameElement.Container = new HoldsOneElement { Element = nameElement };
            string generated = XamlServices.Save(nameElement);
            string expected = @"<NameElement x:Name=""__ReferenceID0"" Container=""__ReferenceID0"" xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />";
            Assert.AreEqual(expected, generated);
        }

        [TestCase]
        public void SimpleNameConverterWithRuntimeName()
        {
            var nameElement = new NameElementWithRuntimeName() { Name = "Foo" };
            nameElement.Container = new HoldsOneElement { Element = nameElement };
            string generated = XamlServices.Save(nameElement);
            string expected = @"<NameElementWithRuntimeName Container=""Foo"" Name=""Foo"" xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes"" />";
            Assert.AreEqual(expected, generated);
        }

        [TestCase]
        public void NameRequestedInInvisibleNameScope()
        {
            var foo = new Element();

            var bar = new NameElement();
            bar.Container = new HoldsOneElement { Element = foo };

            var ns = new NameScope { Content = foo };
            var ns2 = new NameScope { Content = bar };
            var arr = new object[] { ns, ns2 };

            var generated = XamlServices.Save(arr);

            var expected = @"<x:Array Type=""x:Object"" xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <NameScope>
    <Element />
  </NameScope>
  <NameScope>
    <NameElement Container=""__ReferenceID0"" />
  </NameScope>
</x:Array>";

            Assert.AreEqual(expected, generated);
        }

        [TestCase]
        public void NameRequestedInInvisibleNameScope2()
        {
            var foo = new Element();

            var bar = new NameElement();
            bar.Container = new HoldsOneElement { Element = foo };

            var ns = new ObjectContainer { Content = bar };
            var ns2 = new NameScope { Content = foo };
            var arr = new object[] { ns2, ns };

            var generated = XamlServices.Save(arr);

            var expected =
@"<x:Array Type=""x:Object"" xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes"" xmlns:p=""http://XamlTestTypes"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <NameScope>
    <Element />
  </NameScope>
  <p:ObjectContainer>
    <p:ObjectContainer.Content>
      <NameElement Container=""__ReferenceID0"" />
    </p:ObjectContainer.Content>
  </p:ObjectContainer>
</x:Array>";

            Assert.AreEqual(expected, generated);
        }

        [TestCase]
        public void NameRequestedForObjectInRootNameScope()
        {
            var foo = new Element();

            var bar = new NameElement();
            bar.Container = new HoldsOneElement { Element = foo };

            var ns = new ObjectContainer { Content = foo };
            var ns2 = new NameScope { Content = bar };
            var arr = new object[] { ns, ns2 };

            var generated = XamlServices.Save(arr);

            var expected =
@"<x:Array Type=""x:Object"" xmlns=""http://XamlTestTypes"" xmlns:mtxto=""clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <ObjectContainer>
    <ObjectContainer.Content>
      <mtxto:Element x:Name=""__ReferenceID0"" />
    </ObjectContainer.Content>
  </ObjectContainer>
  <mtxto:NameScope>
    <mtxto:NameElement Container=""__ReferenceID0"" />
  </mtxto:NameScope>
</x:Array>";

            Assert.AreEqual(expected, generated);
        }

        [TestCase]
        public void MultipleGetNames()
        {
            var foo = new Element();

            var bar = new NameElement();
            bar.Container = new HoldsOneElement { Element = foo };

            var bar2 = new NameElement();
            bar2.Container = new HoldsOneElement { Element = foo };

            var arr = new object[] { bar, bar2, foo };

            var generated = XamlServices.Save(arr);

            var expected =
@"<x:Array Type=""x:Object"" xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <NameElement Container=""__ReferenceID0"" />
  <NameElement Container=""__ReferenceID0"" />
  <Element x:Name=""__ReferenceID0"" />
</x:Array>";

            Assert.AreEqual(expected, generated);
        }

        [TestCase]
        public void MultipleGetNamesInDifferentNamescopes()
        {
            var foo = new Element();

            var bar = new NameElement();
            bar.Container = new HoldsOneElement { Element = foo };

            var bar2 = new NameElement();
            bar2.Container = new HoldsOneElement { Element = foo };

            var ns = new NameScope { Content = bar2 };

            var arr = new object[] { bar, ns, foo };

            var generated = XamlServices.Save(arr);

            var expected =
@"<x:Array Type=""x:Object"" xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <NameElement Container=""__ReferenceID0"" />
  <NameScope>
    <NameElement Container=""__ReferenceID0"" />
  </NameScope>
  <Element x:Name=""__ReferenceID0"" />
</x:Array>";

            Assert.AreEqual(expected, generated);
        }

        [TestCase]
        public void NameReferenceConverter()
        {
            var foo = new Element();
            var bar = new NameReferencedHoldsTwoElements { One = foo, Two = foo };
            string generated = XamlServices.Save(bar);
            string expected =
@"<NameReferencedHoldsTwoElements Two=""__ReferenceID0"" xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <NameReferencedHoldsTwoElements.One>
    <Element x:Name=""__ReferenceID0"" />
  </NameReferencedHoldsTwoElements.One>
</NameReferencedHoldsTwoElements>";
            Assert.AreEqual(expected, generated);
        }

        [TestCase]
        public void NameReferenceConverterRoundtrip()
        {
            var foo = new Element();
            var bar = new NameReferencedHoldsTwoElements { One = foo, Two = foo };
            string xaml = XamlTestDriver.RoundTripCompare(bar);

            NameReferencedHoldsTwoElements loaded = (NameReferencedHoldsTwoElements)XamlServices.Parse(xaml);
            if (loaded.One.GetHashCode() != loaded.Two.GetHashCode())
            {
                throw new TestCaseFailedException("Referenced objects are not the same object");
            }
        }

        /// <summary>
        /// Regression test
        /// </summary>
        [TestCase]
        public void InitOnNoTCMember()
        {   
            ExceptionHelper.ExpectException<XamlObjectWriterException>( ()=>
                {   
                    var xsc = new XamlSchemaContext();
                    XamlObjectWriter xow = new XamlObjectWriter(xsc);
                    xow.WriteStartObject(xsc.GetXamlType(typeof(Element)));
                        xow.WriteStartMember(XamlLanguage.Initialization);
                            xow.WriteValue("Hello");
                        xow.WriteEndMember();
                    xow.WriteEndObject();
                },
                new XamlObjectWriterException());
        }

        /// <summary>
        /// Regression test
        /// XamlObjectReader should unwrap generic dictionaries if the dictionary type maches the property type
        /// </summary>
        [TestCase]
        public void IgnoreTagOnReadWriteDictionary()
        {
            DictionaryContainer root = new DictionaryContainer();
            root.Dict = new Dictionary<string, int>() { { "A", 1 } };

            XamlTestDriver.RoundTripCompareExamineXaml(
                                 root,
                                 new string[]
                                {
                                    @"/t:DictionaryContainer/t:DictionaryContainer.Dict/x2:Int32"
                                },
                                 new Dictionary<string, string>
                                {
                                    {"t", "clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes"}
                                });
        }

        /// <summary>
        /// Regression test 
        /// :XamlObjectReader doesn't wrap exceptions from reflection
        /// </summary>
        [TestCase]
        public void BadGetter()
        {
            BadGetter badGetter = new BadGetter();
            ExceptionHelper.ExpectException<XamlObjectReaderException>(() => XamlServices.Save(badGetter), new XamlObjectReaderException());
        }

        /// <summary>
        /// Regression test
        /// WPF Compat: Shouldn't treat Gotten Objects as namescopes
        /// 
        /// x:Name is on a property (which is not a namescope) - the name should be on the root
        /// </summary>
        [TestCase]
        public void NameScopeProperty1()
        {
            NameScopeArrayListHolder root = (NameScopeArrayListHolder)XamlServices.Parse(@"
                            <NameScopeArrayListHolder 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                              <NameScopeArrayListHolder.ArrayList>
                                <x:Int32 x:Name='foo'/>
                              </NameScopeArrayListHolder.ArrayList>
                            </NameScopeArrayListHolder>");
            Assert.AreEqual((root as INameScope).FindName("foo"), 0);
            // ArrayList holds an instanc eof NameScopeArrayList which is an INameScope
            Assert.IsNull((root.ArrayList as INameScope).FindName("foo"));
        }

        /// <summary>
        /// Regression test
        /// WPF Compat: Shouldn't treat Gotten Objects as namescopes
        /// x:Name is on a property (which is a namescope and GO) - the name should be on the property
        /// </summary>
        [TestCase]
        public void NameScopeProperty2()
        {
            NameScopeArrayListHolder n = (NameScopeArrayListHolder)XamlServices.Parse(@"
                            <NameScopeArrayListHolder 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes'
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                              <NameScopeArrayListHolder.NameScopeArrayList>
                                <x:Int32 x:Name='foo'/>
                              </NameScopeArrayListHolder.NameScopeArrayList>
                            </NameScopeArrayListHolder>");
            Assert.IsNull((n as INameScope).FindName("foo"));
            // ArrayList holds an instanc eof NameScopeArrayList which is an INameScope
            Assert.AreEqual((n.NameScopeArrayList as INameScope).FindName("foo"), 0);
        }

        /// <summary>
        /// Regression test
        /// Issue related to xml:space, discovered while 
        /// piping from XamlXmlReader to XamlXmlWriter
        /// </summary>
        [TestCase]
        public void DuplicateXmlSpace()
        {
            string value = "Hello\r\nGoodbye";

            string xaml = XamlServices.Save(value);
            Tracer.LogTrace("original xaml is  " + xaml);

            var reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml)));
            
            StringWriter stringWriter = new StringWriter();
            var xmlWriter = new XamlXmlWriter(XmlWriter.Create(stringWriter), reader.SchemaContext);

            // This should not throw duplicate property exception 
            // due to multiple xml:space properties
            XamlServices.Transform(reader, xmlWriter);

            string xaml2 = stringWriter.GetStringBuilder().ToString();
            Tracer.LogTrace("Transformed xaml is  " + xaml2);

            string value2 = (string)XamlServices.Parse(xaml2);
            
            if (!string.Equals(value2,"Hello\nGoodbye"))
            {
                Tracer.LogTrace("Value = " + value);
                Tracer.LogTrace("Value2 = " + value2);
                throw new TestCaseFailedException("Original and roundtripped strings did not match");
            }
        }

        /// <summary>
        /// Verify line info is preserved when piping through a node list
        /// </summary>
        [TestCase]
        public void XamlNodeListProvideLineInfo()
        {
            const string xaml = "<UnknownType />";
            var settings = new XamlXmlReaderSettings { ProvideLineInfo = true };
            var reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), settings);
            var nodeList = new XamlNodeList(reader.SchemaContext);
            XamlServices.Transform(reader, nodeList.Writer);

            try
            {
                XamlServices.Load(nodeList.GetReader());
                throw new TestCaseFailedException("Expected exception not thrown.");
            }
            catch (XamlObjectWriterException xowe)
            {
                if (xowe.LineNumber != 1 || xowe.LinePosition != 2)
                {
                    throw new TestCaseFailedException(string.Format("Expected line 1, position 2.  Actual line {0}, position {1}.", xowe.LineNumber, xowe.LinePosition));
                }
            }
        }

        /// <summary>
        /// Verify line info is preserved when piping through a node queue
        /// </summary>
        [TestCase]
        public void XamlNodeQueueProvideLineInfo()
        {
            const string xaml = "<UnknownType />";
            var settings = new XamlXmlReaderSettings { ProvideLineInfo = true };
            var reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), settings);
            var nodeQueue = new XamlNodeQueue(reader.SchemaContext);
            XamlServices.Transform(reader, nodeQueue.Writer);

            try
            {
                XamlServices.Load(nodeQueue.Reader);
                throw new TestCaseFailedException("Expected exception not thrown.");
            }
            catch (XamlObjectWriterException xowe)
            {
                if (xowe.LineNumber != 1 || xowe.LinePosition != 2)
                {
                    throw new TestCaseFailedException(string.Format("Expected line 1, position 2.  Actual line {0}, position {1}.", xowe.LineNumber, xowe.LinePosition));
                }
            }            
        }

        /// <summary>
        /// Verify XamlXmlReaderSettings Copy constructor copies CloseInput value.
        /// This test verifies other XamlXmlReaderSettings properties as well.
        /// </summary>
        [TestCase]
        public void XamlXmlReaderSettingsCopy()
        {
            var settings1 = new XamlXmlReaderSettings();
            
            // Assigning non-default values to XamlXmlReaderSettings properties.
            settings1.CloseInput = true;
            settings1.SkipXmlCompatibilityProcessing = true;
            settings1.XmlSpacePreserve = true;
            settings1.XmlLang = "de-DE";
            settings1.LocalAssembly = typeof(GenericType9<>).Assembly;
            settings1.BaseUri = new Uri("file:///E:/Pics/A/");
            settings1.IgnoreUidsOnPropertyElements = true;
            settings1.ProvideLineInfo = true;
            settings1.ValuesMustBeString = true;

            var settings2 = new XamlXmlReaderSettings(settings1);

            if (settings2.CloseInput != settings1.CloseInput
                || settings2.SkipXmlCompatibilityProcessing != settings1.SkipXmlCompatibilityProcessing
                || settings2.XmlSpacePreserve != settings1.XmlSpacePreserve
                || settings2.XmlLang != settings1.XmlLang
                || settings2.LocalAssembly != settings1.LocalAssembly
                || settings2.BaseUri != settings1.BaseUri
                || settings2.IgnoreUidsOnPropertyElements != settings1.IgnoreUidsOnPropertyElements
                || settings2.ProvideLineInfo != settings1.ProvideLineInfo
                || settings2.ValuesMustBeString != settings1.ValuesMustBeString)
            {
                string message = "\n---IGNORE IF THE RUNTIME IS 4.0 OR LOWER ----";
                message += "\nCloseInput               -- Expected: " + settings1.CloseInput + " / Actual: " + settings2.CloseInput;
                message += "\nSkipXmlCompatibilityProcessing -- Expected: " + settings1.SkipXmlCompatibilityProcessing + " / Actual: " + settings1.SkipXmlCompatibilityProcessing;
                message += "\nXmlSpacePreserve               -- Expected: " + settings1.XmlSpacePreserve + " / Actual: " + settings2.XmlSpacePreserve;
                message += "\nIgnoreUidsOnPropertyElements   -- Expected: " + settings1.IgnoreUidsOnPropertyElements + " / Actual: " + settings2.IgnoreUidsOnPropertyElements;
                message += "\nProvideLineInfo                -- Expected: " + settings1.ProvideLineInfo + " / Actual: " + settings2.ProvideLineInfo;
                message += "\nValuesMustBeString             -- Expected: " + settings1.ValuesMustBeString + " / Actual: " + settings2.ValuesMustBeString;
                message += "\nXmlLang                        -- Expected: " + settings1.XmlLang + " / Actual: " + settings2.XmlLang;
                message += "\nLocalAssembly                  -- Expected: " + settings1.LocalAssembly + " / Actual: " + settings2.LocalAssembly;
                message += "\nBaseUri                        -- Expected: " + settings1.BaseUri + " / Actual: " + settings2.BaseUri;
                throw new TestCaseFailedException("FAIL: XamlXmlReaderSettings Copy constructor failed to copy CloseInput value" + message);
            }
          
        }

        /// <summary>
        /// Provide coverage of XamlNodeList.Clear
        /// </summary>
        [TestCase]
        public void XamlNodeListClear()
        {
            const string xaml = "<UnknownType />";
            var settings = new XamlXmlReaderSettings { ProvideLineInfo = true };
            var reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), settings);
            var nodeList = new XamlNodeList(reader.SchemaContext);
            XamlServices.Transform(reader, nodeList.Writer);

            Assert.IsTrue(nodeList.Count > 0);

            nodeList.Clear();
            Assert.IsTrue(nodeList.Count == 0);
        }

        [TestCase]
        public void GetObjectAtRoot()
        {
            NodeList nodes = new NodeList()
            {
                new GetObject(),
            };

            string expectedMessage = Glob.Exceptions.GetMessage("NoPropertyInCurrentFrame_GO_noType", Glob.WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => nodes.NodeListToObject());
            
        }

        [TestCase]
        public void EndObjectAtRoot()
        {
            NodeList nodes = new NodeList()
            {
                new EndObject(),
            };

            string expectedMessage = Glob.Exceptions.GetMessage("NoTypeInCurrentFrame_EO", Glob.WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => nodes.NodeListToObject());
        }

        [TestCase]
        public void EndMemberAtRoot()
        {
            NodeList nodes = new NodeList()
            {
                new EndMember(),
                new EndObject(),
            };

            string expectedMessage = Glob.Exceptions.GetMessage("NoPropertyInCurrentFrame_EM_noType", Glob.WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => nodes.NodeListToObject());
        }

        [TestCase]
        public void ValueAtRoot()
        {
            NodeList nodes = new NodeList()
            {
                new ValueNode("20"),
                new EndMember(),
                new EndObject(),
            };

            string expectedMessage = Glob.Exceptions.GetMessage("NoPropertyInCurrentFrame_V_noType", Glob.WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => nodes.NodeListToObject());
        }

        [TestCase]
        public void StartMemberAtRoot()
        {
            NodeList nodes = new NodeList()
            {
                new StartMember(typeof(Point), "X"),
                new ValueNode("20"),
                new EndMember(),
                new EndObject(),
            };

            string expectedMessage = Glob.Exceptions.GetMessage("NoTypeInCurrentFrame_SM", Glob.WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => nodes.NodeListToObject());
        }

        [TestCase]
        public void NamespaceBeforeVNeg()
        {
            NodeList nodes = new NodeList()
            {
                new StartObject(typeof(Point)),
                new StartMember(typeof(Point), "X"),
                new NamespaceNode("clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40", "foo"),
                new ValueNode("20"),
                new EndMember(),
                new EndObject(),
            };

            string expectedMessage = Glob.Exceptions.GetMessage("NoTypeInCurrentFrame_EO", Glob.WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => nodes.NodeListToObject());
        }

        [TestCase]
        public void NamespaceBeforeEMNeg()
        {
            NodeList nodes = new NodeList()
            {
                new StartObject(typeof(Point)),
                new StartMember(typeof(Point), "X"),
                new ValueNode("20"),
                new NamespaceNode("clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40", "foo"),
                new EndMember(),
                new EndObject(),
            };

            string expectedMessage = Glob.Exceptions.GetMessage("ValueMustBeFollowedByEndMember", Glob.WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => nodes.NodeListToObject());
        }

        [TestCase]
        public void NamespaceBeforeEONeg()
        {
            NodeList nodes = new NodeList()
            {
                new StartObject(typeof(Point)),
                new StartMember(typeof(Point), "X"),
                new ValueNode("20"),
                new EndMember(),
                new NamespaceNode("clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40", "foo"),
                new EndObject(),
            };

            string expectedMessage = Glob.Exceptions.GetMessage("NoPropertyInCurrentFrame_NS", Glob.WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => nodes.NodeListToObject());
        }

        private enum ObjectWriterEvents
        {
            AfterBeginInit,
            BeforeProperties,
            AfterProperties,
            AfterEndInit
        }

        private void CheckEventType(ObjectWriterEvents actual, Queue<ObjectWriterEvents> expectedEvents)
        {
            if (expectedEvents.Count == 0)
            {
                throw new DataTestException("Unexpected event fired: " + actual.ToString());
            }

            var expected = expectedEvents.Dequeue();
            if (expected != actual)
            {
                throw new DataTestException("Expected event of type: " + expected.ToString() + " Actual: " + actual.ToString());
            }
        }

        private void ObjectWriterEventTestCore(System.Xaml.XamlReader reader)
        {
            var expectedEvents = new Queue<ObjectWriterEvents>();
            var beginObjectNodes = new Stack<XamlNodeType>();
            var writerSettings = new XamlObjectWriterSettings
            {
                AfterBeginInitHandler = (sender, args) => { CheckEventType(ObjectWriterEvents.AfterBeginInit, expectedEvents); },
                AfterEndInitHandler = (sender, args) => { CheckEventType(ObjectWriterEvents.AfterEndInit, expectedEvents); },
                BeforePropertiesHandler = (sender, args) => { CheckEventType(ObjectWriterEvents.BeforeProperties, expectedEvents); },
                AfterPropertiesHandler = (sender, args) => { CheckEventType(ObjectWriterEvents.AfterProperties, expectedEvents); },
            };

            using (var writer = new XamlObjectWriter(reader.SchemaContext, writerSettings))
            {
                var previousNode = XamlNodeType.None;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XamlNodeType.GetObject:
                        case XamlNodeType.StartObject:
                            beginObjectNodes.Push(reader.NodeType);
                            break;
                        case XamlNodeType.EndObject:
                            if (beginObjectNodes.Pop() == XamlNodeType.StartObject)
                            {
                                expectedEvents.Enqueue(ObjectWriterEvents.AfterProperties);
                                expectedEvents.Enqueue(ObjectWriterEvents.AfterEndInit);
                            }
                            break;
                        case XamlNodeType.StartMember:
                            if (previousNode == XamlNodeType.StartObject)
                            {
                                expectedEvents.Enqueue(ObjectWriterEvents.AfterBeginInit);
                                expectedEvents.Enqueue(ObjectWriterEvents.BeforeProperties);
                            }
                            break;
                    }

                    previousNode = reader.NodeType;
                    writer.WriteNode(reader);
                    if (expectedEvents.Count > 0)
                    {
                        Tracer.Trace("ObjectWriterEventTest", "Expected events not fired.");
                        foreach (var eventType in expectedEvents)
                        {
                            Tracer.Trace("ObjectWriterEventTest", eventType.ToString());
                        }
                        throw new DataTestException("Expected events not fired.");
                    }
                }
            }
        }
    }

    public class TypeWithReadOnlyCollectionAndTypeConverter
    {
        Dictionary<ItemType, ItemType> _items = new Dictionary<ItemType, ItemType>();

        public Dictionary<ItemType, ItemType> Items { get { return _items; } }

        public Tiger Tiger { get; set; }
    }

    //internal class, not allowed to declare private classes
    internal class ClassType6
    {
        public string Data { get; set; }
        protected string ProtectedData { get; set; }
        private string PrivateData { get; set; }

        public ClassType6()
        {
            Data = "my data";
            ProtectedData = "protected data";
            PrivateData = "private data";
        }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ClassType6 instance1 = new ClassType6();
            // [DISABLED]
            // testCases.Add(new TestCaseInfo
            // {
            //     Target = instance1,
            //     TestID = instanceIDPrefix + 1,
            // });

            return testCases;
        }
    }

    internal class InternalTypeWithInternalCtor
    {
        internal InternalTypeWithInternalCtor() { }
    }

    public class PublicClassWithInternalCtor
    {
        internal PublicClassWithInternalCtor() { }
    }
}
