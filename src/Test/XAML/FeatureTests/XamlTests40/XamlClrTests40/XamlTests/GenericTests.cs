// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Infrastructure.Test;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;

    public class GenericTests
    {
        #region GenericWithDifferentGenericParameterTest

        private List<Type> _genericWithDifferentGenericParameterTestTypes;

        private List<Type> GenericWithDifferentGenericParameterTestTypes
        {
            get
            {
                if (this._genericWithDifferentGenericParameterTestTypes == null)
                {
                    this._genericWithDifferentGenericParameterTestTypes = new List<Type>();
                    this._genericWithDifferentGenericParameterTestTypes.Add(typeof (GenericContainerType1));
                    this._genericWithDifferentGenericParameterTestTypes.Add(typeof (GenericContainerType3));
                }
                return this._genericWithDifferentGenericParameterTestTypes;
            }
        }

        [TestCaseGenerator]
        public void GenericWithDifferentGenericParameterTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.GenericWithDifferentGenericParameterTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void GenericWithDifferentGenericParameterTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.GenericWithDifferentGenericParameterTestTypes, instanceID));
        }

        #endregion

        #region GenericWithCollectionParameterTest

        private List<Type> _genericWithCollectionParameterTestTypes;

        public List<Type> GenericWithCollectionParameterTestTypes
        {
            get
            {
                if (this._genericWithCollectionParameterTestTypes == null)
                {
                    this._genericWithCollectionParameterTestTypes = new List<Type>();
                    this._genericWithCollectionParameterTestTypes.Add(typeof (GenericContainerType2<int, int, int>)); // generic parameter here is really dummy, reflection needs it to invoke the static method on the type
                    this._genericWithCollectionParameterTestTypes.Add(typeof (GenericContainerType4));
                    this._genericWithCollectionParameterTestTypes.Add(typeof (GenericContainerType5));
                }
                return this._genericWithCollectionParameterTestTypes;
            }
        }

        [TestCaseGenerator]
        public void GenericWithCollectionParameterTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.GenericWithCollectionParameterTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void GenericWithCollectionParameterTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.GenericWithCollectionParameterTestTypes, instanceID));
        }

        #endregion

        #region GenericWithInheritanceTest

        private readonly List<Type> _genericWithInheritanceTestTypes = new List<Type>
                                                                          {
                                                                              typeof (GenericType4<int, int>),
                                                                              typeof (GenericType5),
                                                                              typeof (GenericType6<int>)
                                                                          };

        [TestCaseGenerator]
        public void GenericWithInheritanceTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._genericWithInheritanceTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void GenericWithInheritanceTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._genericWithInheritanceTestTypes, instanceID));
        }

        #endregion

        #region NestedGenericParameterTest

        private readonly List<Type> _nestedGenericParameterTestTypes = new List<Type>
                                                                          {
                                                                              typeof (GenericType_Outer2)
                                                                          };

        [TestCaseGenerator]
        public void NestedGenericParameterTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._nestedGenericParameterTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void NestedGenericParameterTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._nestedGenericParameterTestTypes, instanceID));
        }

        #endregion

        #region NestedGenericTypeTest

        private readonly List<Type> _nestedGenericTypeTestTypes = new List<Type>
                                                                     {
                                                                         typeof (GenericType_Outer1<int>)
                                                                     };

        [TestCaseGenerator]
        public void NestedGenericTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._nestedGenericTypeTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void NestedGenericTypeTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._nestedGenericTypeTestTypes, instanceID));
        }

        #endregion

        #region GenericOnTopTest

        private readonly List<Type> _genericOnTopTestTypes = new List<Type>
                                                                {
                                                                    typeof (GenericContainerType7)
                                                                };

        [TestCaseGenerator]
        public void GenericOnTopTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._genericOnTopTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void GenericOnTopTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._genericOnTopTestTypes, instanceID));
        }

        #endregion

        #region CollectionOfGenericTest

        private readonly List<Type> _collectionOfGenericTestTypes = new List<Type>
                                                                       {
                                                                           typeof (GenericContainerType2<int, int, int>)
                                                                       };

        [TestCaseGenerator]
        public void CollectionOfGenericTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._collectionOfGenericTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void CollectionOfGenericTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._collectionOfGenericTestTypes, instanceID));
        }

        #endregion

        #region RecursiveGenericTest

        private List<Type> _recursiveGenericTestTypes;

        private List<Type> RecursiveGenericTestTypes
        {
            get
            {
                if (this._recursiveGenericTestTypes == null)
                {
                    this._recursiveGenericTestTypes = new List<Type>();
                    this._recursiveGenericTestTypes.Add(typeof (GenericContainerType6));
                    this._recursiveGenericTestTypes.Add(typeof (GenericType3));
                }
                return this._recursiveGenericTestTypes;
            }
        }

        [TestCaseGenerator]
        public void RecursiveGenericTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.RecursiveGenericTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void RecursiveGenericTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.RecursiveGenericTestTypes, instanceID));
        }

        #endregion

        #region GenericWithConstructorArgTest

        private readonly List<Type> _genericWithConstructorArgTestTypes = new List<Type>
                                                                             {
                                                                                 typeof (GenericType7<int, int>)
                                                                             };

        [TestCaseGenerator]
        public void GenericWithConstructorArgTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._genericWithConstructorArgTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void GenericWithConstructorArgTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._genericWithConstructorArgTestTypes, instanceID));
        }

        #endregion

        #region GenericWithShadowedProperties

        private readonly List<Type> _genericWithShadowedPropertiesTypes = new List<Type>
                                                                             {
                                                                                 typeof (GenericType8)
                                                                             };

        [TestCaseGenerator]
        public void GenericWithShadowedPropertiesTypesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._genericWithShadowedPropertiesTypes, MethodInfo.GetCurrentMethod());
        }

        public void GenericWithShadowedPropertiesTypesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._genericWithShadowedPropertiesTypes, instanceID));
        }

        #endregion
       
        [TestCase(Keywords="MicroSuite")]
        public void GenericSerializationRepro15130()
        {
            GenericContainerType3 instance1 = new GenericContainerType3();
            instance1.Infos = new GenericType1<GenericType1<int, double, Guid>, GenericType2<bool, long, ulong>>();
            instance1.Infos.Info1 = new GenericType1<int, double, Guid>();
            GenericType1<int, double, Guid> info1 = new GenericType1<int, double, Guid>();
            info1.Info1 = 100;
            info1.Info2 = 100.001;
            info1.Info3 = Guid.Empty;
            instance1.Infos.Info1 = info1;
            instance1.Infos.Info2 = new GenericType2<bool, long, ulong>();
            instance1.Infos.Info2.Info1 = true;
            instance1.Infos.Info2.Info2 = (long) -0.000001;
            instance1.Infos.Info2.Info3 = (ulong) 0.000001;

            GenericContainerType6 instance2 = new GenericContainerType6();
            instance2.Info = new GenericType1<GenericType1<int, double>, GenericType1<int, double>>();
            GenericType1<int, double> item = new GenericType1<int, double>();
            item.Info1 = 100;
            item.Info2 = 100.001;
            instance2.Info.Info1 = item;
            instance2.Info.Info2 = item;

            XamlTestDriver.Roundtrip(instance1);
            XamlTestDriver.Roundtrip(instance2);
        }
    }
}
