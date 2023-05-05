// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xaml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types.ContentProperties;
    using Microsoft.Test.Xaml.Utilities;

    public class ContentPropertyTests
    {
        #region PrimitiveContentProperties

        private readonly List<Type> _primitiveContentPropertiesTypes = new List<Type>
                                                                          {
                                                                              typeof (StringContentProperty),
                                                                              typeof (ShadowedContentProperty),
                                                                              typeof (StringContentPropertyWClass),
                                                                              typeof (BooleanContentProperty),
                                                                              typeof (Int8ContentProperty),
                                                                              typeof (Int16ContentProperty),
                                                                              typeof (Int32ContentProperty),
                                                                              typeof (Int64ContentProperty),
                                                                              typeof (UInt8ContentProperty),
                                                                              typeof (UInt16ContentProperty),
                                                                              typeof (UInt32ContentProperty),
                                                                              typeof (UInt64ConentProperty),
                                                                              typeof (SingleContentProperty),
                                                                              typeof (DoubleContentProperty),
                                                                              typeof (DecimalContentProperty),
                                                                              typeof (DateTimeConentProperty),
                                                                              typeof (DateTimeOffsetContentProperty),
                                                                              typeof (CharContentProperty),
                                                                              typeof (EnumContentProperty)
                                                                          };

        [TestCaseGenerator()]
        public void PrimitiveContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._primitiveContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void PrimitiveContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._primitiveContentPropertiesTypes, instanceID));
        }

        #endregion

        #region WrappedPrimitiveContentProperties

        private readonly List<Type> _wrappedPrimitiveContentPropertiesTypes = new List<Type>
                                                                                 {
                                                                                     typeof (WrappedPrimitiveContentProperties),
                                                                                 };

        [TestCaseGenerator()]
        public void WrappedPrimitiveContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._wrappedPrimitiveContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void WrappedPrimitiveContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._wrappedPrimitiveContentPropertiesTypes, instanceID));
        }

        #endregion

        #region ClassContentProperties

        private readonly List<Type> _classContentPropertiesTypes = new List<Type>
                                                                      {
                                                                          typeof (ClassContentProperty),
                                                                          typeof (EmptyClassContentProperty),
                                                                          typeof (StructContentProperty),
                                                                          typeof (EmptyStructContentProperty)
                                                                      };

        [TestCaseGenerator()]
        public void ClassContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._classContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void ClassContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._classContentPropertiesTypes, instanceID));
        }

        #endregion

        #region WrappedClassContentProperties

        private readonly List<Type> _wrappedClassContentPropertiesTypes = new List<Type>
                                                                             {
                                                                                 typeof (WrappedClassContentProperties)
                                                                             };

        [TestCaseGenerator()]
        public void WrappedClassContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._wrappedClassContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void WrappedClassContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._wrappedClassContentPropertiesTypes, instanceID));
        }

        #endregion

        #region TypeConverterContentProperties

        private readonly List<Type> _typeConverterContentPropertiesTypes = new List<Type>
                                                                              {
                                                                                  typeof (TypeConverterContentProperty),
                                                                                  typeof (UselessConverterContainer), 
                                                                              };

        // [DISABLED]
        // [TestCaseGenerator()]
        public void TypeConverterContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._typeConverterContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeConverterContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._typeConverterContentPropertiesTypes, instanceID));
        }

        #endregion

        #region WrappedTypeConverterContentProperties

        private readonly List<Type> _wrappedTypeConverterContentPropertiesTypes = new List<Type>
                                                                                     {
                                                                                         typeof (WrappedTypeConverterContentProperties),
                                                                                     };

        [TestCaseGenerator()]
        public void WrappedTypeConverterContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._wrappedTypeConverterContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void WrappedTypeConverterContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._wrappedTypeConverterContentPropertiesTypes, instanceID));
        }

        #endregion

        #region GenericContentProperties

        private readonly List<Type> _genericContentPropertiesTypes = new List<Type>
                                                                        {
                                                                            typeof (GenericContentProperty<string>),
                                                                        };

        [TestCaseGenerator()]
        public void GenericContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._genericContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void GenericContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._genericContentPropertiesTypes, instanceID));
        }

        #endregion

        #region WrappedGenericContentProperties

        private readonly List<Type> _wrappedGenericContentPropertiesTypes = new List<Type>
                                                                               {
                                                                                   typeof (WrappedGenericContentProperties),
                                                                               };

        // [DISABLED]
        // [TestCaseGenerator()]
        public void WrappedGenericContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._wrappedGenericContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void WrappedGenericContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._wrappedGenericContentPropertiesTypes, instanceID));
        }

        #endregion

        #region CollectionContentProperties

        private readonly List<Type> _collectionContentPropertiesTypes = new List<Type>
                                                                           {
                                                                               typeof (PrimitiveArraryContentProperty),
                                                                               typeof (ClassArrayContentProperty),
                                                                               // [DISABLED]
                                                                            //    typeof (ArrayListContentProperty),
                                                                               typeof (HashtableContentProperty),
                                                                               typeof (IListContentProperty),
                                                                               typeof (IListTContentProperty),
                                                                               typeof (ListTContentProperty),
                                                                               typeof (IDictionaryContentProperty),
                                                                               typeof (IDictionaryKVContentProperty),
                                                                               typeof (DictionaryKVContentProperty)
                                                                           };

        [TestCaseGenerator()]
        public void CollectionContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._collectionContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void CollectionContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._collectionContentPropertiesTypes, instanceID));
        }

        #endregion

        #region WrappedCollectionContentProperties

        private readonly List<Type> _wrappedCollectionContentPropertiesTypes = new List<Type>
                                                                                  {
                                                                                      typeof (WrappedCollectionContentProperties)
                                                                                  };

        [TestCaseGenerator()]
        public void WrappedCollectionContentPropertiesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._wrappedCollectionContentPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void WrappedCollectionContentPropertiesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._wrappedCollectionContentPropertiesTypes, instanceID));
        }

        #endregion

        [TestCase]
        public void TypeWithoutContentPropertyErrorTest()
        {
            string xaml = "<SomeTypeWithoutAContentProperty xmlns='clr-namespace:Microsoft.Test.Xaml.Types.ContentProperties;assembly=XamlClrTypes'>blah</SomeTypeWithoutAContentProperty>";
            string error = Exceptions.GetMessage("TypeHasNoContentProperty", WpfBinaries.SystemXaml);

            ExceptionHelpers.CheckForException(
                typeof(XamlObjectWriterException),
                Exceptions.GetMessage("TypeHasNoContentProperty", WpfBinaries.SystemXaml),
                () => XamlServices.Parse(xaml));

        }

        /// <summary>
        /// content property contains type converter
        /// the object.member tag should not be written out.
        /// </summary>
        [TestCase]
        public void ContentPropertyWithUselessTC()
        {
            UselessConverterContainer instance = new UselessConverterContainer()
            {
                Element = new Element(),
            };
            string xaml = XamlServices.Save(instance);
            if (xaml.Contains("UselessConverterContainer.Element"))
            {
                throw new TestCaseFailedException("Content property tag UselessConverterContainer.Element written out when not expected. Xaml is " + xaml);
            }
        }
    }
}
