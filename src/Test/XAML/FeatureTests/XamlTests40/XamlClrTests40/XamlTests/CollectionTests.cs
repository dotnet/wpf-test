// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Xaml;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Utilities;

    public class CollectionTests
    {
        #region BasicCollectionTest

        private List<Type> _basicCollectionTestTypes;

        private List<Type> BasicCollectionTestTypes
        {
            get
            {
                if (this._basicCollectionTestTypes == null)
                {
                    this._basicCollectionTestTypes = new List<Type>();
                    // the following two scenarios should behave the same
                    this._basicCollectionTestTypes.Add(typeof (CollectionContainerType1)); //RW collection initialized in type
                    this._basicCollectionTestTypes.Add(typeof (CollectionContainerType2)); //RW collection not initilized in type
                }
                return this._basicCollectionTestTypes;
            }
        }

        [TestCaseGenerator]
        public void BasicCollectionTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.BasicCollectionTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void BasicCollectionTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.BasicCollectionTestTypes, instanceID));
        }

        #endregion

        #region CollectionInterfaceAsMemberTest

        private readonly List<Type> _collectionInterfaceAsMemberTestTypes = new List<Type>
                                                                               {
                                                                                   typeof (CollectionContainerType12),
                                                                                   typeof (CollectionContainerType13)
                                                                               };

        [TestCaseGenerator]
        public void CollectionInterfaceAsMemberTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._collectionInterfaceAsMemberTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void CollectionInterfaceAsMemberTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._collectionInterfaceAsMemberTestTypes, instanceID));
        }

        #endregion

        #region CollectionWithInheritanceTest

        private readonly List<Type> _collectionWithInheritanceTestTypes = new List<Type>
                                                                             {
                                                                                 typeof (CollectionContainerType14),
                                                                                 typeof (CollectionContainerType15),
                                                                                 typeof (CollectionContainerType16),
                                                                                 typeof(CollectionContainerType24<object,object>),
                                                                                 typeof(CollectionContainerType25)
                                                                             };

        [TestCaseGenerator]
        public void CollectionWithInheritanceTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._collectionWithInheritanceTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void CollectionWithInheritanceTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._collectionWithInheritanceTestTypes, instanceID));
        }

        #endregion

        #region GenericCollectionTest

        [TestCaseGenerator]
        public void GenericCollectionTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._genericCollectionTestTypes, MethodInfo.GetCurrentMethod());
        }

        private readonly List<Type> _genericCollectionTestTypes = new List<Type>
                                                                     {
                                                                         typeof (CollectionContainerType8),
                                                                         typeof(GenericDictionary<int, string>),
                                                                     };

        public void GenericCollectionTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._genericCollectionTestTypes, instanceID));
        }

        #endregion

        #region CollectionCompositionTest

        [TestCaseGenerator]
        public void CollectionCompositionTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._collectionCompositionTestTypes, MethodInfo.GetCurrentMethod());
        }

        private readonly List<Type> _collectionCompositionTestTypes = new List<Type>
                                                                         {
                                                                             typeof (CollectionContainerType6),
                                                                             typeof (CollectionContainerType7),
                                                                         };

        public void CollectionCompositionTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._collectionCompositionTestTypes, instanceID));
        }

        #endregion

        #region MultipleKindCollectionTest

        [TestCaseGenerator]
        public void MultipleKindCollectionTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._multipleKindCollectionTestTypes, MethodInfo.GetCurrentMethod());
        }

        private readonly List<Type> _multipleKindCollectionTestTypes = new List<Type>
                                                                          {
                                                                              typeof (CollectionContainerType5GoodEnumerator),
                                                                              typeof (CollectionContainerType5BadEnumerator)
                                                                          };

        public void MultipleKindCollectionTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._multipleKindCollectionTestTypes, instanceID));
        }

        #endregion

        #region ReadOnlyPropertyWithCollectionTest

        private List<Type> _readOnlyPropertyWithCollectionTestTypes;

        private List<Type> ReadOnlyPropertyWithCollectionTestTypes
        {
            get
            {
                if (this._readOnlyPropertyWithCollectionTestTypes == null)
                {
                    this._readOnlyPropertyWithCollectionTestTypes = new List<Type>();
                    this._readOnlyPropertyWithCollectionTestTypes.Add(typeof (CollectionContainerType3)); // RO collection initialized in type
                    this._readOnlyPropertyWithCollectionTestTypes.Add(typeof (CollectionContainerType4)); // RO collection not initialized in type - negative
                    this._readOnlyPropertyWithCollectionTestTypes.Add(typeof(CollectionContainerType21)); // collection with protected setter
                }
                return _readOnlyPropertyWithCollectionTestTypes;
            }
        }

        [TestCaseGenerator]
        public void ReadOnlyPropertyWithCollectionTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.ReadOnlyPropertyWithCollectionTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ReadOnlyPropertyWithCollectionTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.ReadOnlyPropertyWithCollectionTestTypes, instanceID));
        }

        #endregion

        [TestCase(Owner = "Microsoft",
            Category = TestCategory.Functional,
            TestType = TestType.Automated,
            Title = @"Readonly collection")]
        public void ReadOnlyCollectionTest()
        {
            CollectionContainerType18 data = new CollectionContainerType18();
            data.AddItem(new ItemType
                             {
                                 ItemName = "stuff", Price = 3.14
                             });
            data.AddItem(new ItemType
                             {
                                 ItemName = "more stuff", Price = 6.28
                             });

            string xaml = XamlServices.Save(data);
            ExceptionHelpers.CheckForException(typeof (XamlObjectWriterException), () => XamlServices.Parse(xaml));
        }

        //[TestCase(Owner = "Microsoft",
        //          Category = TestCategory.Functional, 
        //          TestType = TestType.NotComplete,
        //          Title = @"ContnetWrapperAttribute")]
        //public void ContentWrapperAttributeTest() 
        //{
        //    throw new NotImplementedException("Test case method ContentWrapperAttribute test is not implemented.");
        //}

        #region CollectionWithConstructorArgTest

        private readonly List<Type> _collectionWithConstructorArgTestTypes = new List<Type>
                                                                                {
                                                                                    typeof (CollectionContainerType17)
                                                                                };

        [TestCaseGenerator]
        public void CollectionWithConstructorArgTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._collectionWithConstructorArgTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void CollectionWithConstructorArgTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._collectionWithConstructorArgTestTypes, instanceID));
        }

        #endregion

        private readonly List<Type> _systemCollectionTestTypes = new List<Type>
                                                                    {
                                                                        typeof (CollectionContainerType19)
                                                                    };

        [TestCaseGenerator]
        public void SystemCollectionTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._systemCollectionTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void SystemCollectionTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._systemCollectionTestTypes, instanceID));
        }

        #region CaseInsenstiveHashtableTest

        private readonly List<Type> _caseInsenstiveHashtableTestTypes = new List<Type>
                                                                           {
                                                                               typeof (CollectionContainerType11)
                                                                           };

        [TestCaseGenerator]
        public void CaseInsenstiveHashtableTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._caseInsenstiveHashtableTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void CaseInsenstiveHashtableTestMethod(string instanceID)
        {
            IDictionary dictionaryOriginal = (IDictionary) XamlTestDriver.GetInstance(this._caseInsenstiveHashtableTestTypes, instanceID).Target;
            IDictionary dictionaryAfter = (IDictionary) XamlTestDriver.Roundtrip(dictionaryOriginal);
            string[] keys = {
                                "key1", "key2", "key3"
                            };
            foreach (string key in keys)
            {
                object upperCase = dictionaryAfter[key.ToUpper(CultureInfo.InvariantCulture)];
                object lowerCase = dictionaryAfter[key.ToLower(CultureInfo.InvariantCulture)];
                if (TreeComparer.CompareLogical(upperCase, lowerCase) != CompareResult.Equivalent)
                {
                    throw new DataTestException("Dictionary is no longer case insensitive for key " + key);
                }
            }
        }

        #endregion

        #region DictionaryItemWTypeConverterRepro9241Test

        [TestCaseGenerator]
        public void DictionaryItemWTypeConverterRepro9241Test(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._dictionaryItemWTypeConverterRepro9241TestTypes, MethodInfo.GetCurrentMethod());
        }

        private readonly List<Type> _dictionaryItemWTypeConverterRepro9241TestTypes = new List<Type>
                                                                                         {
                                                                                             typeof (CollectionContainerType10),
                                                                                         };

        public void DictionaryItemWTypeConverterRepro9241TestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._dictionaryItemWTypeConverterRepro9241TestTypes, instanceID));
        }

        #endregion

        #region SerializeArrayRepro10359Test

        [TestCaseGenerator]
        public void SerializeArrayRepro10359Test(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._serializeArrayRepro10359TestTypes, MethodInfo.GetCurrentMethod());
        }

        private readonly List<Type> _serializeArrayRepro10359TestTypes = new List<Type>
                                                                            {
                                                                                typeof (CollectionContainerType9),
                                                                            };

        public void SerializeArrayRepro10359TestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._serializeArrayRepro10359TestTypes, instanceID));
        }

        #endregion

        
        #region ImplementCollectionInterfaceTest
        private readonly List<Type> _implementCollectionInterfaceTestTypes = new List<Type>
                                                                    {
                                                                        typeof (CollectionContainerType20)
                                                                    };
        [TestCaseGenerator]
        public void ImplementCollectionInterfaceTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._implementCollectionInterfaceTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ImplementCollectionInterfaceTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._implementCollectionInterfaceTestTypes, instanceID));
        }
        #endregion

        #region CollectionOfTypeTest
        private readonly List<Type> _collectionOfTypeTestTypes = new List<Type>
                                                                    {
                                                                        typeof (CollectionContainerType22)
                                                                    };
        [TestCaseGenerator]
        public void CollectionOfTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._collectionOfTypeTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void CollectionOfTypeTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._collectionOfTypeTestTypes, instanceID));
        }
        #endregion

        #region ReferencesInDictionaryTest
        private readonly List<Type> _referencesInDictionaryTestTypes = new List<Type>
                                                                    {
                                                                        typeof (CollectionContainerType23)
                                                                    };
        [TestCaseGenerator]
        public void ReferencesInDictionaryTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._referencesInDictionaryTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ReferencesInDictionaryTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._referencesInDictionaryTestTypes, instanceID));
        }
        #endregion
    }
}
