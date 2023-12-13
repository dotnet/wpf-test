// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a base type to support toolable combinatorial test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/TestTypes/ManagedCombinatorialTestCase.cs $")]

namespace Test.Uis.TestTypes
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Reflection;

    using Test.Uis.Management;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// Provides management services to combinatorial cases.
    /// </summary>
    public abstract class ManagedCombinatorialTestCase: CustomCombinatorialTestCase
    {
        #region Protected methods.

        /// <summary>Gets the dimensions to combine off the TestCaseData table.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            return TestCaseData.Dimensions;
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            MapValuesToInstance(values);
            return true;
        }

        #endregion Protected methods.

        #region Protected properties.

        /// <summary>TestCaseData used to manage this test case.</summary>
        protected TestCaseData TestCaseData
        {
            get
            {
                if (this._testCaseData == null)
                {
                    Type testCaseType;  // Most-derived type of test case.

                    testCaseType = this.GetType();
                    this._testCaseData = TestCaseData.GetTestCaseData(testCaseType, ConfigurationSettings.Current);
                }
                return this._testCaseData;
            }
        }

        #endregion Protected properties.

        #region Private methods.

        /// <summary>
        /// Maps values from the specified Hashtable to instance fields on the
        /// test case.
        /// </summary>
        private void MapValuesToInstance(Hashtable values)
        {
            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            Type type;      // Most-derived type of test case.
            Type fieldType; // Type in which the field is declared.

            type = this.GetType();
            foreach(DictionaryEntry entry in values)
            {
                string memberName;  // Field name.
                FieldInfo field;    // Field object.

                memberName = (string)entry.Key;
                fieldType = type;
                do
                {
                    field = fieldType.GetField(memberName, BindingFlags.NonPublic |
                        BindingFlags.Public | BindingFlags.Instance);
                    if (field == null)
                    {
                        fieldType = fieldType.BaseType;
                    }
                } while (field == null && fieldType != null);

                if (field == null)
                {
                    throw new InvalidOperationException(
                        "Unable to find field " + memberName + " on class " + type);
                }
                field.SetValue(this, entry.Value);
            }
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>TestCaseData used to manage this test case.</summary>
        private TestCaseData _testCaseData;

        #endregion Private fields.
    }
}