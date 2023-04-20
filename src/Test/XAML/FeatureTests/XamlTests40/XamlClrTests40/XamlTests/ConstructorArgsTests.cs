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

    public class ConstructorArgsTests
    {
        #region ConstructorArgNameCasingTest

        private List<Type> _constructorArgNameCasingTestTypes;

        private List<Type> ConstructorArgNameCasingTestTypes
        {
            get
            {
                if (this._constructorArgNameCasingTestTypes == null)
                {
                    this._constructorArgNameCasingTestTypes = new List<Type>();
                    this._constructorArgNameCasingTestTypes.Add(typeof (ConstructorArgType_ArgNameCasing1));
                    this._constructorArgNameCasingTestTypes.Add(typeof (ConstructorArgType_ArgNameCasing2));
                    this._constructorArgNameCasingTestTypes.Add(typeof (ConstructorArgType_ArgNameCasing3));
                    this._constructorArgNameCasingTestTypes.Add(typeof (ConstructorArgType_ArgNameCasing4));
                    this._constructorArgNameCasingTestTypes.Add(typeof (ConstructorArgType_ArgNameCasing5));
                    this._constructorArgNameCasingTestTypes.Add(typeof (ConstructorArgType_ArgNameCasing6));
                }
                return this._constructorArgNameCasingTestTypes;
            }
        }

        [TestCaseGenerator]
        public void ConstructorArgNameCasingTest(AddTestCaseEventHandler addCase)
        {
            //Constructor arg name casing
            XamlTestDriver.GenerateTestCases(addCase, this.ConstructorArgNameCasingTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ConstructorArgNameCasingTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.ConstructorArgNameCasingTestTypes, instanceID));
        }

        #endregion

        #region ConstructorArgWithInheritanceTest

        private List<Type> _constructorArgWithInheritanceTestTypes;

        private List<Type> ConstructorArgWithInheritanceTestTypes
        {
            get
            {
                if (this._constructorArgWithInheritanceTestTypes == null)
                {
                    this._constructorArgWithInheritanceTestTypes = new List<Type>();
                    this._constructorArgWithInheritanceTestTypes.Add(typeof (Blogger));
                    this._constructorArgWithInheritanceTestTypes.Add(typeof (ScientificBook));
                    this._constructorArgWithInheritanceTestTypes.Add(typeof (DerivedImmutableAddressWithoutArgs));
                    this._constructorArgWithInheritanceTestTypes.Add(typeof (DerivedImmutableAddressWithoutArgs2)); // inheritance hierarchy of 3
                    this._constructorArgWithInheritanceTestTypes.Add(typeof (DerivedConstructorArg));
                }
                return this._constructorArgWithInheritanceTestTypes;
            }
        }

        [TestCaseGenerator]
        public void ConstructorArgWithInheritanceTest(AddTestCaseEventHandler addCase)
        {
            // Constructor arg name casing with inheritance
            XamlTestDriver.GenerateTestCases(addCase, this.ConstructorArgWithInheritanceTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ConstructorArgWithInheritanceTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.ConstructorArgWithInheritanceTestTypes, instanceID));
        }

        #endregion

        #region ConstructorArgWithReadOnlyPropertyTest

        private readonly List<Type> _constructorArgWithReadOnlyPropertyTestTypes = new List<Type>
                                                                                      {
                                                                                          typeof (ConstructorArgType_ROProperty2),
                                                                                          typeof (ConstructorArgType_ROProperty2_Collection),
                                                                                          typeof (ConstructorArgType_ROProperty2_Enum),
                                                                                          typeof (ConstructorArgType_ROProperty2_Struct),
                                                                                          typeof (ConstructorArgType_ROProperty2_Class),
                                                                                          typeof (ConstructorArgType_ROProperty3)
                                                                                      };

        [TestCaseGenerator]
        public void ConstructorArgWithReadOnlyPropertyTest(AddTestCaseEventHandler addCase)
        {
            // Constructor arg name casing with inheritance
            XamlTestDriver.GenerateTestCases(addCase, this._constructorArgWithReadOnlyPropertyTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ConstructorArgWithReadOnlyPropertyTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._constructorArgWithReadOnlyPropertyTestTypes, instanceID));
        }

        #endregion

        #region ConstructorArgWithReadWritePropertyTest

        private readonly List<Type> _constructorArgWithReadWritePropertyTestTypes = new List<Type>
                                                                                       {
                                                                                           typeof (ConstructorArgType_ROProperty1)
                                                                                       };

        [TestCaseGenerator]
        public void ConstructorArgWithReadWritePropertyTest(AddTestCaseEventHandler addCase)
        {
            // Constructor arg name casing with inheritance
            XamlTestDriver.GenerateTestCases(addCase, this._constructorArgWithReadWritePropertyTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ConstructorArgWithReadWritePropertyTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._constructorArgWithReadWritePropertyTestTypes, instanceID));
        }

        #endregion

        #region ConstructorArgumentAttributeWithReadOnlyPropertyTest

        private readonly List<Type> _constructorArgumentAttributeWithReadOnlyPropertyTestTypes = new List<Type>
                                                                                                    {
                                                                                                        typeof (ConstructorArgType_ROProperty4)
                                                                                                    };

        [TestCaseGenerator]
        public void ConstructorArgumentAttributeWithReadOnlyPropertyTest(AddTestCaseEventHandler addCase)
        {
            // Constructor arg name casing with inheritance
            XamlTestDriver.GenerateTestCases(addCase, this._constructorArgumentAttributeWithReadOnlyPropertyTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ConstructorArgumentAttributeWithReadOnlyPropertyTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._constructorArgumentAttributeWithReadOnlyPropertyTestTypes, instanceID));
        }

        #endregion

        #region ConstructorArgumentAttributeWithReadOnlyPropertyNotInConstructorTest

        private readonly List<Type> _constructorArgumentAttributeWithReadOnlyPropertyNotInConstructorTestTypes = new List<Type>
                                                                                                                    {
                                                                                                                        typeof (ConstructorArgType_ROProperty5),
                                                                                                                        typeof (ConstructorArgType_ROProperty6)
                                                                                                                    };

        [TestCaseGenerator]
        public void ConstructorArgumentAttributeWithReadOnlyPropertyNotInConstructorTest(AddTestCaseEventHandler addCase)
        {
            // Constructor arg name casing with inheritance
            XamlTestDriver.GenerateTestCases(addCase, this._constructorArgumentAttributeWithReadOnlyPropertyNotInConstructorTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ConstructorArgumentAttributeWithReadOnlyPropertyNotInConstructorTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._constructorArgumentAttributeWithReadOnlyPropertyNotInConstructorTestTypes, instanceID));
        }

        #endregion

        #region ConstructorArgumentAttributeWithReadWritePropertyTest

        private readonly List<Type> _constructorArgumentAttributeWithReadWritePropertyTestTypes = new List<Type>
                                                                                                     {
                                                                                                         typeof (ConstructorArgType_ROProperty7)
                                                                                                     };

        [TestCaseGenerator]
        public void ConstructorArgumentAttributeWithReadWritePropertyTest(AddTestCaseEventHandler addCase)
        {
            // Constructor arg name casing with inheritance
            XamlTestDriver.GenerateTestCases(addCase, this._constructorArgumentAttributeWithReadWritePropertyTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ConstructorArgumentAttributeWithReadWritePropertyTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._constructorArgumentAttributeWithReadWritePropertyTestTypes, instanceID));
        }

        #endregion

        #region ConstructorArgWithReadOnlyPropertyTest_R

        private List<Type> _constructorArgWithROPropertyTestTypes_R;
        public List<Type> ConstructorArgWithROPropertyTestTypes_R
        {
            get
            {
                if (this._constructorArgWithROPropertyTestTypes_R == null)
                {
                    this._constructorArgWithROPropertyTestTypes_R = new List<Type>();
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R1));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R2));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R3));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R4));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R5));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R6));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R62));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R7));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R8));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R9));
                    this._constructorArgWithROPropertyTestTypes_R.Add(typeof (ConstructorArgType_ROProperty_R10));
                }
                return this._constructorArgWithROPropertyTestTypes_R;
            }
        }

        [TestCaseGenerator]
        public void ConstructorArgWithROPropertyTest_R(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.ConstructorArgWithROPropertyTestTypes_R, MethodInfo.GetCurrentMethod());
        }

        public void ConstructorArgWithROPropertyTest_RMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.ConstructorArgWithROPropertyTestTypes_R, instanceID));
        }

        #endregion ConstructorArgWithRWPropertyTest

        #region ConstructorArgWithRWPropertyTest_R

        private List<Type> _constructorArgWithRWPropertyTestTypes_R;
        public List<Type> ConstructorArgWithRWPropertyTestTypes_R
        {
            get
            {
                if (this._constructorArgWithRWPropertyTestTypes_R == null)
                {
                    this._constructorArgWithRWPropertyTestTypes_R = new List<Type>();
                    this._constructorArgWithRWPropertyTestTypes_R.Add(typeof (ConstructorArgWithRWPropertyTest_R1));
                }
                return this._constructorArgWithRWPropertyTestTypes_R;
            }
        }

        [TestCaseGenerator]
        public void ConstructorArgWithRWPropertyTest_R(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.ConstructorArgWithRWPropertyTestTypes_R, MethodInfo.GetCurrentMethod());
        }

        public void ConstructorArgWithRWPropertyTest_RMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.ConstructorArgWithRWPropertyTestTypes_R, instanceID));
        }

        #endregion
    }
}
