// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Xaml;
    using Microsoft.Infrastructure.Test;
    using Microsoft.Test;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.Xaml.Utilities;

    public class TypeConverterTests
    {
        [TestCase]
        public void TypeConverterReturnNullTest()
        {
            // If type converter returns null, it is changed to String.Empty
            // See GetValueFromStringTypeConverter(...) in prodcut source code XamlWriteHelper
            // This is for V1 WPF compatibility.

            Tracer.Trace("[Microsoft.Test.Xaml.XamlTests.TypeConverterTests.TypeConverterReturnNullTest]", "Running ...");
            Zoo zoo = new Zoo();
            Zoo roundTrippedZoo = (Zoo)XamlTestDriver.Roundtrip(zoo);

            if (zoo.Tiger.NickName != null)
            {
                throw new DataTestException("For TypeConverter returning null test, zoo.Tiger.NickName must be null.");
            }

            if (roundTrippedZoo.Tiger.NickName != string.Empty)
            {
                throw new DataTestException(String.Format(CultureInfo.InvariantCulture, "zoo.Tiger.NickName is not changed to string.Empty. It has the value of '{0}'", zoo.Tiger.NickName));
            }
            Tracer.Trace("[Microsoft.Test.Xaml.XamlTests.TypeConverterTests.TypeConverterReturnNullTest]", "Passed.");
        }

        #region BasicTypeConverterTest

        private List<Type> _basicTypeConverterTestTypes;

        private List<Type> BasicTypeConverterTestTypes
        {
            get
            {
                if (this._basicTypeConverterTestTypes == null)
                {
                    this._basicTypeConverterTestTypes = new List<Type>();
                    this._basicTypeConverterTestTypes.Add(typeof(Zoo1));
                    this._basicTypeConverterTestTypes.Add(typeof(Zoo2));
                    this._basicTypeConverterTestTypes.Add(typeof(Zoo3));
                    this._basicTypeConverterTestTypes.Add(typeof(UsesNested));
                    this._basicTypeConverterTestTypes.Add(typeof(ContainerClassForInternalTypeTest));
                    this._basicTypeConverterTestTypes.Add(typeof(ContentContainer));
                }
                return this._basicTypeConverterTestTypes;
            }
        }

        [TestCaseGenerator]
        public void BasicTypeConverterTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.BasicTypeConverterTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void BasicTypeConverterTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.BasicTypeConverterTestTypes, instanceID));
        }

        #endregion

        #region TypeConverterOnDifferentScopeTest

        private List<Type> _typeConverterOnDifferentScopeTestTypes;

        private List<Type> TypeConverterOnDifferentScopeTestTypes
        {
            get
            {
                if (this._typeConverterOnDifferentScopeTestTypes == null)
                {
                    this._typeConverterOnDifferentScopeTestTypes = new List<Type>();
                    this._typeConverterOnDifferentScopeTestTypes.Add(typeof(Zoo));

                    // [DISABLED]
                    // this.typeConverterOnDifferentScopeTestTypes.Add(typeof(Tiger));
                }
                return _typeConverterOnDifferentScopeTestTypes;
            }
        }

        [TestCaseGenerator]
        public void TypeConverterOnDifferentScopeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.TypeConverterOnDifferentScopeTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeConverterOnDifferentScopeTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.TypeConverterOnDifferentScopeTestTypes, instanceID));
        }

        #endregion

        #region TypeConverWithNonXamlSerializerFriendlyTypeTest

        private List<Type> _typeConverWithNonXamlSerializerFriendlyTypeTestTypes;

        private List<Type> TypeConverWithNonXamlSerializerFriendlyTypeTestTypes
        {
            get
            {
                if (this._typeConverWithNonXamlSerializerFriendlyTypeTestTypes == null)
                {
                    this._typeConverWithNonXamlSerializerFriendlyTypeTestTypes = new List<Type>();
                    this._typeConverWithNonXamlSerializerFriendlyTypeTestTypes.Add(typeof(Manager));
                }
                return this._typeConverWithNonXamlSerializerFriendlyTypeTestTypes;
            }
        }

        [TestCaseGenerator]
        public void TypeConverWithNonXamlSerializerFriendlyTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.TypeConverWithNonXamlSerializerFriendlyTypeTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeConverWithNonXamlSerializerFriendlyTypeTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.TypeConverWithNonXamlSerializerFriendlyTypeTestTypes, instanceID));
        }

        #endregion

        #region TypeConverterAndStringTest

        private List<Type> _typeConverterAndStringTestTypes;

        private List<Type> TypeConverterAndStringTestTypes
        {
            get
            {
                if (this._typeConverterAndStringTestTypes == null)
                {
                    this._typeConverterAndStringTestTypes = new List<Type>();
                    this._typeConverterAndStringTestTypes.Add(typeof(Frog));
                }
                return _typeConverterAndStringTestTypes;
            }
        }

        [TestCaseGenerator]
        public void TypeConverterAndStringTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.TypeConverterAndStringTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeConverterAndStringTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.TypeConverterAndStringTestTypes, instanceID));
        }

        #endregion

        private List<Type> TypeConverterAndEnumTestTypes
        {
            get
            {
                return new List<Type>
                           {
                               typeof (ClassWithEnumWithoutTypeConverter),
                               typeof (ClassWithEnumWithTypeConverter)
                           };
            }
        }

        [TestCaseGenerator]
        public void TypeConverterAndEnumTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.TypeConverterAndEnumTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeConverterAndEnumTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.TypeConverterAndEnumTestTypes, instanceID));
        }

        private List<Type> IncorrectlyImplementedTypeConverterTestTypes
        {
            get
            {
                return new List<Type>
                           {
                               typeof (ClassWithBadTypeConverter),
                           };
            }
        }

        [TestCaseGenerator]
        public void IncorrectlyImplementedTypeConverterTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.IncorrectlyImplementedTypeConverterTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void IncorrectlyImplementedTypeConverterTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.IncorrectlyImplementedTypeConverterTestTypes, instanceID));
        }

        [TestCase(Owner = "Microsoft",
            Category = TestCategory.Functional,
            TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated,
            Title = @"Necessary TypeConverter does not exist")]
        public void MissingTypeConverterTest()
        {
            string xaml = @"<ClassType2 xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes' Category='category' />";
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), () => XamlServices.Parse(xaml));
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated
            )]
        public void CtorArgWStringTypeConverter()
        {
            CtorArgsWStringTypeConverter foo = new CtorArgsWStringTypeConverter(new MemberWStringTypeConverter
                                                                                    {
                                                                                        A = 1,
                                                                                        B = 2
                                                                                    });

            try
            {
                XamlServices.Save(foo);
                throw new DataTestException("Expected exception not caught");
            }
            catch(XamlObjectReaderException xore)
            {
                if (xore.InnerException.GetType() != typeof(DataTestException))
                {
                    throw new DataTestException("Unexpected inner exception caught.  Expected:  ConvertTo called. Actual:  " + xore);
                }
                if (xore.InnerException.Message != "ConvertTo called.")
                {
                    throw new DataTestException("Unexpected exception caught.  Expected:  ConvertTo called. Actual:  " + xore.InnerException.Message);
                }
            }
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated
            )]
        public void CtorArgWMarkupExtensionTypeConverter()
        {
            CtorArgsWMarkupExtensionTypeConverter foo = new CtorArgsWMarkupExtensionTypeConverter(new MemberWMarkupExtensionTypeConverter
                                                                                                      {
                                                                                                          A = 1,
                                                                                                          B = 2
                                                                                                      });

            try
            {
                XamlServices.Save(foo);
                throw new DataTestException("Expected exception not caught");
            }
            catch (XamlObjectReaderException xore)
            {
                if (xore.InnerException.GetType() != typeof(DataTestException))
                {
                    throw new DataTestException("Unexpected inner exception caught.  Expected:  ConvertTo called. Actual:  " + xore);
                }
                if (xore.InnerException.Message != "ConvertTo called.")
                {
                    throw new DataTestException("Unexpected exception caught.  Expected:  ConvertTo called. Actual:  " + xore.InnerException.Message);
                }
            }
        }

        /// <summary>
        /// This needs to be full trust since we use an instance descriptor
        /// More info
        /// </summary>
        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated
            )]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void CtorArgWInstanceDescriptorTypeConverter()
        {
            CtorArgsWInstanceDescriptorTypeConverter foo = new CtorArgsWInstanceDescriptorTypeConverter(MemberWInstanceDescriptorTypeConverter.Create(1,2));

            try
            {
                XamlServices.Save(foo);
                throw new DataTestException("Expected exception not caught");
            }
            catch (XamlObjectReaderException xore)
            {
                if (xore.InnerException.GetType() != typeof(DataTestException))
                {
                    throw new DataTestException("Unexpected inner exception caught.  Expected:  ConvertTo called. Actual:  " + xore);
                }
                if (xore.InnerException.Message != "ConvertTo called.")
                {
                    throw new DataTestException("Unexpected exception caught.  Expected:  ConvertTo called. Actual:  " + xore.InnerException.Message);
                }
            }
        }

        //[TestCase(
        //     Owner = "Microsoft",
        //     Category = TestCategory.IDW,
        //     TestType = TestType.Automated
        //     )]
        //public void CtorArgWXamlTemplateTypeConverter()
        //{
        //    CtorArgsWXamlTemplateTypeConverter foo = new CtorArgsWXamlTemplateTypeConverter(new MemberWXamlTemplateTypeConverter { A = 1, B = 2 });

        //    try
        //    {
        //        XamlServices.Save(foo);
        //        throw new DataTestException("Expected exception not caught");
        //    }
        //    catch (DataTestException dte)
        //    {
        //        if (dte.Message != "ConvertTo called.")
        //        {
        //            throw new DataTestException("Unexpected exception caught.  Expected:  ConvertTo called. Actual:  " + dte.Message);
        //        }
        //    }
        //}

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated
            )]
        public void XmlnsSerializationTest()
        {
            Tiger t = new Tiger("Billy");
            string xaml = XamlServices.Save(t);

            if (xaml.Contains("xmlns:"))
            {
                throw new DataTestException("There shouldn't be any prefix definitions in :" + xaml);
            }
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated
            )]
        public void TypeConverterOnEmptyTypeTest()
        {
            EmptyClassContainer container = new EmptyClassContainer
                                                {
                                                    ConverterOnProperty = new EmptyClass(),
                                                    ConverterOnType = new EmptyClassWithTypeConverter()
                                                };

            string xamlContainer = XamlServices.Save(container);
            string xamlTop = XamlServices.Save(container.ConverterOnType);

            if (!xamlTop.Contains("This is an empty class"))
            {
                throw new DataTestException("Class not serialized corrected, expected This is an empty class to appear: " + xamlTop);
            }

            if (xamlContainer.IndexOf("This is an empty class") == xamlContainer.LastIndexOf("This is an empty class"))
            {
                throw new DataTestException("Class not serialized corrected, expected This is an empty class to appear twice: " + xamlContainer);
            }
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated
            )]
        public void FullSyntaxOnTypeConverterTest()
        {
            ImaginaryWrapper wrapperExpected = new ImaginaryWrapper
                                                   {
                                                       Number = new ImaginaryNumber
                                                                    {
                                                                        Real = 3,
                                                                        Imaginary = 42
                                                                    }
                                                   };
            string xamlFull = @"<ImaginaryWrapper xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'>
                              <ImaginaryWrapper.Number>
                                <ImaginaryNumber Imaginary='42' Real='3' />
                              </ImaginaryWrapper.Number>
                            </ImaginaryWrapper>";

            string xamlShort = @"<ImaginaryWrapper Number='3+42i' xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'></ImaginaryWrapper>";

            ImaginaryWrapper wrapperFull = (ImaginaryWrapper)XamlServices.Parse(xamlFull);
            ImaginaryWrapper wrapperShort = (ImaginaryWrapper)XamlServices.Parse(xamlShort);
            XamlObjectComparer.CompareObjects(wrapperFull, wrapperExpected);
            XamlObjectComparer.CompareObjects(wrapperShort, wrapperExpected);

            ExceptionHelpers.CheckForException(typeof(XamlObjectReaderException), () => XamlServices.Save(wrapperFull));
        }

        /// <summary>
        /// Have both initialization and member nodes.
        /// i.e provide a member as well on a type converted object.
        /// Regression test
        /// </summary>
        [TestCase]
        public void TypeConvertedAndProperty()
        {
            string xaml = @"<ImaginaryWrapper xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'>
                               <ImaginaryWrapper.Number>
                                  <ImaginaryNumber>
                                     3+42i
                                     <ImaginaryNumber.Real>5</ImaginaryNumber.Real>
                                  </ImaginaryNumber>
                               </ImaginaryWrapper.Number>
                             </ImaginaryWrapper>";
            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Parse(xaml), new XamlObjectWriterException(), "SettingPropertiesIsNotAllowed", WpfBinaries.SystemXaml);
        }

        [TestCase]
        public void TypeConverterAndCPATest()
        {
            // CPA should be chosen over the type conveter //
            CPAAndTypeConverterContainer obj = new CPAAndTypeConverterContainer() { NameCPA = "Hello World" };
            string xaml = XamlTestDriver.RoundTripCompare(obj);
            if (xaml.Contains("ContertedThroughTypeConverter"))
            {
                throw new TestCaseFailedException("TypeConverter was chosen over the ContentProperty attribute");
            }
        }

        [TestCase]
        public void TypeConverterAndCPAonCollectionTest()
        {
            // CPA should be chosen over the type conveter //
            var obj = new CPAonCollectionTypeConverterContainer() { NameCPA = { "Hello World" } };
            string xaml = XamlTestDriver.RoundTripCompare(obj);
            if (xaml.Contains("ContertedThroughTypeConverter"))
            {
                throw new TestCaseFailedException("TypeConverter was chosen over the ContentProperty attribute");
            }
        }

        #region XamlNamespaceResolverConverter

        private List<Type> _xamlNamespaceResolverConverterTestTypes = new List<Type>
        {
            typeof(XamlNamespaceResolverContainer)
        };

        // test the use of the IXamlNamespaceResolver service
        [TestCaseGenerator]
        public void XamlNamespaceResolverConverterTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._xamlNamespaceResolverConverterTestTypes, MethodInfo.GetCurrentMethod());
        }

        // object comparison of System.Type fails through roundtrips, this instance needs to be manually verified
        public void XamlNamespaceResolverConverterTestMethod(string instanceID)
        {
            var instance = (XamlNamespaceResolverContainer)XamlTestDriver.GetInstance(this._xamlNamespaceResolverConverterTestTypes, instanceID).Target;
            var after = (XamlNamespaceResolverContainer)XamlTestDriver.Roundtrip(instance);

            if (instance.Types == null && after.Types == null)
            {
                return;
            }

            if (instance.Types == null || after.Types == null)
            {
                throw new DataTestException("Types collection not found after serialization.");
            }

            if (instance.Types.Count() != after.Types.Count())
            {
                throw new DataTestException("Counts don't match");
            }

            foreach (var type in after.Types)
            {
                if (!instance.Types.Contains(type))
                {
                    throw new DataTestException(type.ToString() + " not found after serialization.");
                }
            }
        }
        #endregion

        /// <summary>
        /// attribute directives (like x:Name) weren't allowed on type converted in v3.5 but are allowed in v4.0+
        /// </summary>
        [TestCase]
        public void DirectiveAttributesTest()
        {
            string xaml = @"<TypeWithDirectiveAttributes  
                                 x:Name='runtimeName' x:Uid='id' xml:lang='en-us' 
                                 xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes' 
                                 xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>Name1</TypeWithDirectiveAttributes>";

            var expected = new TypeWithDirectiveAttributes
            {
                Data = "Name1",
                RuntimeName = "runtimeName",
                Uid = "id",
                XmlLang = "en-us"
            };

            XamlObjectComparer.CompareObjects(expected, XamlServices.Parse(xaml));
        }

        /// <summary>
        /// Regression test
        /// </summary>
        [TestCase]
        public void Key_IDictionary_NoTC()
        {
            string key_IDictionary_NoTC = @"<Dictionaries x:TypeArguments='x:Int32, x:String'
                                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                              <Dictionaries.IDictionary>
                                                <x:String>
                                                  <x:Key>
                                                    <x:Int32>1</x:Int32>
                                                  </x:Key>
                                                  2
                                                </x:String>
                                              </Dictionaries.IDictionary>
                                            </Dictionaries>";

            var obj = new Dictionaries<int, string>()
            {
                IDictionary = {
                    { 1, "2"},
                },
            };
            XamlTestDriver.XamlFirstCompareObjects(key_IDictionary_NoTC, obj);
        }

        /// <summary>
        /// Regression test
        /// </summary>
        [TestCase]
        public void Key_IDictionary_TC()
        {
            string key_IDictionary_TC = @"<Dictionaries x:TypeArguments='x:Int32, x:String'
                                           xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                                           xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                          <Dictionaries.IDictionary>
                                            <x:String x:Key='1'>2</x:String>
                                          </Dictionaries.IDictionary>
                                        </Dictionaries>";

            var obj = new Dictionaries<int, string>()
            {
                IDictionary = {
                    { 1, "2"},
                },
            };

            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Parse(key_IDictionary_TC), new XamlObjectWriterException());
        }

        /// <summary>
        /// Regression test
        /// </summary>
        [TestCase]
        public void GenericIDictionary_NoTC()
        {
            string key_GenericIDictionary_NoTC = @"<Dictionaries x:TypeArguments='x:Int32, x:String'
                                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                              <Dictionaries.GenericIDictionary>
                                                <x:String>
                                                  <x:Key>
                                                    <x:Int32>1</x:Int32>
                                                  </x:Key>
                                                  2
                                                </x:String>
                                              </Dictionaries.GenericIDictionary>
                                            </Dictionaries>";
    
            var obj = new Dictionaries<int, string>()
            {
                GenericIDictionary = {
                    { 1, "2"},
                },
            };
            XamlTestDriver.XamlFirstCompareObjects(key_GenericIDictionary_NoTC, obj);
        }

        /// <summary>
        /// Regression test
        /// </summary>
        [TestCase]
        public void Key_GenericIDictionary_TC()
        {
            string key_GenericIDictionary_TC = @"<Dictionaries x:TypeArguments='x:Int32, x:String'
                                                    xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                                                    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                                  <Dictionaries.GenericIDictionary>
                                                    <x:String x:Key='1'>2</x:String>
                                                  </Dictionaries.GenericIDictionary>
                                                </Dictionaries>";

            var obj = new Dictionaries<int, string>()
            {
                GenericIDictionary = {
                    { 1, "2"},
                },
            };
            XamlTestDriver.XamlFirstCompareObjects(key_GenericIDictionary_TC, obj);
        }

        /// <summary>
        /// Regression test
        /// </summary>
        [TestCase]
        public void Key_Dictionary_NoTC()
        {
            string key_Dictionary_NoTC = @"<Dictionaries x:TypeArguments='x:Int32, x:String'
                                            xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                          <Dictionaries.Dictionary>
                                            <x:String>
                                              <x:Key>
                                                <x:Int32>1</x:Int32>
                                              </x:Key>
                                              2
                                            </x:String>
                                          </Dictionaries.Dictionary>
                                        </Dictionaries>";

            var obj = new Dictionaries<int, string>()
            {
                Dictionary = {
                    { 1, "2"},
                },
            };
            XamlTestDriver.XamlFirstCompareObjects(key_Dictionary_NoTC, obj);
        }

        /// <summary>
        /// Regression test
        /// </summary>
        [TestCase]
        public void Key_Dictionary_TC()
        {
            string Key_Dictionary_TC = @"<Dictionaries x:TypeArguments='x:Int32, x:String'
                                            xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                          <Dictionaries.Dictionary>
                                            <x:String x:Key='1'>2</x:String>
                                          </Dictionaries.Dictionary>
                                        </Dictionaries>";

            var obj = new Dictionaries<int, string>()
            {
                Dictionary = {
                    { 1, "2"},
                },
            };
            XamlTestDriver.XamlFirstCompareObjects(Key_Dictionary_TC, obj);
        }

        /// <summary>
        /// Regression test
        /// </summary>
        [TestCase]
        public void Key_MyDictionary_NoTC()
        {
            string key_MyDictionary_NoTC = @"<Dictionaries x:TypeArguments='x:Int32, x:String'
                                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                              <Dictionaries.MyDictionary>
                                                <x:String>
                                                  <x:Key>
                                                    <x:Int32>1</x:Int32>
                                                  </x:Key>
                                                  2
                                                </x:String>
                                              </Dictionaries.MyDictionary>
                                            </Dictionaries>";

            var obj = new Dictionaries<int, string>()
            {
                MyDictionary = {
                    { 1, "2"},
                },
            };
            var obj2 = XamlServices.Parse(key_MyDictionary_NoTC);
            XamlObjectComparer.CompareObjects(obj, obj2);
        }

        /// <summary>
        /// Regression test and 768820
        /// </summary>
        [TestCase]
        public void Key_MyDictionary_TC()
        {
            string key_MyDictionary_TC = @"<Dictionaries x:TypeArguments='x:Int32, x:String'
                                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                              <Dictionaries.MyDictionary>
                                                <x:String x:Key='1'>2</x:String>
                                              </Dictionaries.MyDictionary>
                                            </Dictionaries>";

            var obj = new Dictionaries<int, string>()
            {
                MyDictionary = {
                    { 1, "2"},
                },
            };
            var obj2 = XamlServices.Parse(key_MyDictionary_TC);
            XamlObjectComparer.CompareObjects(obj, obj2);
        }

        /// <summary>
        /// Verify XamlSetTypeConverters are called on load
        /// </summary>
        [TestCase]
        public void XamlSetTypeConverterTest()
        {
            var target = new DerivedWithSetTypeConverter { DerivedProperty = new Tiger("Billy") };
            var after = (DerivedWithSetTypeConverter)XamlTestDriver.Roundtrip(target);

            Assert.AreEqual(DerivedWithSetTypeConverter.BasePropertyValue, after.BaseProperty);
        }
    }
}
