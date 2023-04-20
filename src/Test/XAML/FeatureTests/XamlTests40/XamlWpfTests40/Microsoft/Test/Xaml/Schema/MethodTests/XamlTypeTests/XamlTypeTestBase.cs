// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Xaml;
using System.Xaml.Schema;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Base class for all XamlType tests
    /// </summary>
    public class XamlTypeTestBase
    {
        /// <summary>
        /// Initializes a new instance of the XamlTypeTestBase class
        /// </summary>
        public XamlTypeTestBase()
        {
            SchemaContext = new XamlSchemaContext();
        }

        /// <summary>
        /// Gets or sets xaml schema context
        /// </summary>
        protected XamlSchemaContext SchemaContext { get; set; }

        /// <summary>
        /// Tests IsArray
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsArrayTest(Type testType, bool expectedValue)
        {
            Validate<bool>(GetXamlType(testType).IsArray, expectedValue);
        }

        /// <summary>
        /// Tests IsConstructible
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsConstructibleTest(Type testType, bool expectedValue)
        {
            Validate<bool>(GetXamlType(testType).IsConstructible, expectedValue);
        }

        /// <summary>
        /// Tests IsGeneric
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsGenericTest(Type testType, bool expectedValue)
        {
            Validate<bool>(GetXamlType(testType).IsGeneric, expectedValue);
        }

        /// <summary>
        /// Tests IsMarkupExtension
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsMarkupExtensionTest(Type testType, bool expectedValue)
        {
            Validate<bool>(GetXamlType(testType).IsMarkupExtension, expectedValue);
        }

        /// <summary>
        /// Tests IsNamescope
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsNamescopeTest(Type testType, bool expectedValue)
        {
            Validate<bool>(GetXamlType(testType).IsNameScope, expectedValue);
        }

        /// <summary>
        /// Tests IsNullable
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsNullableTest(Type testType, bool expectedValue)
        {
            Validate<bool>(GetXamlType(testType).IsNullable, expectedValue);
        }

        /// <summary>
        /// Tests IsPublic
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsPublicTest(Type testType, bool expectedValue)
        {
            Validate<bool>(GetXamlType(testType).IsPublic, expectedValue);
        }

        /// <summary>
        /// Tests IsUnknown
        /// </summary>
        /// <param name="xmlns">Xmlns URI string</param>
        /// <param name="name">Name of the type</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsUnknownTest(string xmlns, string name, bool expectedValue)
        {
            IsUnknownTest(xmlns, name, null, expectedValue);
        }

        /// <summary>
        /// Tests IsUnknown
        /// </summary>
        /// <param name="xmlns">Xmlns URI string</param>
        /// <param name="name">Name of the type</param>
        /// <param name="typeArguments">Type arguments</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsUnknownTest(string xmlns, string name, IList<XamlType> typeArguments, bool expectedValue)
        {
            IsUnknownTest(xmlns, name, typeArguments, SchemaContext, expectedValue);
        }

        /// <summary>
        /// Tests IsUnknown
        /// </summary>
        /// <param name="xmlns">Xmlns URI string</param>
        /// <param name="name">Name of the type</param>
        /// <param name="typeArguments">Type arguments</param>
        /// <param name="xamlSchemaContext">Xaml schema context</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsUnknownTest(string xmlns, string name, IList<XamlType> typeArguments, XamlSchemaContext xamlSchemaContext, bool expectedValue)
        {
            XamlType xamlType = new XamlType(xmlns, name, typeArguments, xamlSchemaContext);
            Validate<bool>(xamlType.IsUnknown, expectedValue);
        }

        /// <summary>
        /// Tests IsXData
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsXDataTest(Type testType, bool expectedValue)
        {
            Validate<bool>(GetXamlType(testType).IsXData, expectedValue);
        }

        /// <summary>
        /// Tests Name
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void NameTest(Type testType, string expectedValue)
        {
            Validate<string>(GetXamlType(testType).Name, expectedValue);
        }

        /// <summary>
        /// Tests PreferredXamlNamespace
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected value</param>
        public void PreferredXamlNamespaceTest(Type testType, string expectedValue)
        {
            Validate<string>(GetXamlType(testType).PreferredXamlNamespace, expectedValue);
        }

        /// <summary>
        /// Tests TemplateConverter
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedTemplateConverterName">Name of expected TemplateConverter</param>
        public void TemplateConverterTest(Type testType, string expectedTemplateConverterName)
        {
            XamlType xamlType = GetXamlType(testType);
            Validate<string>((xamlType.DeferringLoader != null ? xamlType.DeferringLoader.Name : null), expectedTemplateConverterName);
        }

        /// <summary>
        /// Tests TextSyntax
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedTextSyntaxName">Expected TextSyntax name</param>
        public void TextSyntaxTest(Type testType, string expectedTextSyntaxName)
        {
            Validate<string>(GetXamlType(testType).TypeConverter.Name, expectedTextSyntaxName);
        }

        /// <summary>
        /// Tests TypeArguments
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedTypeArgumentNames">Expected type argument names</param>
        public void TypeArgumentsTest(Type testType, string[] expectedTypeArgumentNames)
        {
            ValidateCollectionOrdered(GetXamlType(testType).TypeArguments, expectedTypeArgumentNames);
        }

        /// <summary>
        /// Tests UnderlyingType
        /// </summary>
        /// <param name="testType">Type to test</param>
        public void UnderlyingTypeTest(Type testType)
        {
            Validate<Type>(GetXamlType(testType).UnderlyingType, testType);
        }

        /// <summary>
        /// Tests MarkupExtensionReturnType
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedMarkupExtensionReturnTypeName">Expected markup extension return type</param>
        public void MarkupExtensionReturnTypeTest(Type testType, string expectedMarkupExtensionReturnTypeName)
        {
            Validate<string>(GetXamlType(testType).MarkupExtensionReturnType.Name, expectedMarkupExtensionReturnTypeName);
        }

        /// <summary>
        /// Tests ContentProperty
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedPropertyName">Expected property name</param>
        public void ContentPropertyTest(Type testType, string expectedPropertyName)
        {
            Validate<string>(GetXamlType(testType).ContentProperty.Name, expectedPropertyName);
        }

        /// <summary>
        /// Tests ContentWrappers
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedContentWrapperNames">Expected content wrapper names</param>
        public void ContentWrappersTest(Type testType, string[] expectedContentWrapperNames)
        {
            ValidateCollectionUnordered(GetXamlType(testType).ContentWrappers, expectedContentWrapperNames);
        }

        /// <summary>
        /// Tests AllowedContentTypes
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedAllowedContentTypeNames">Expected allowed content type names</param>
        public void AllowedContentTypesTest(Type testType, string[] expectedAllowedContentTypeNames)
        {
            ValidateCollectionUnordered(GetXamlType(testType).AllowedContentTypes, expectedAllowedContentTypeNames);
        }

        /// <summary>
        /// Tests IsCollection and ItemType
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedIsCollection">Expected value of IsCollection</param>
        /// <param name="expectedItemTypeName">Expected value of ItemType</param>
        public void IsCollectionAndItemTypeTest(Type testType, bool expectedIsCollection, string expectedItemTypeName)
        {
            XamlType xamlType = GetXamlType(testType);
            Validate<bool>(xamlType.IsCollection, expectedIsCollection);

            if (xamlType.ItemType == null)
            {
                GlobalLog.LogEvidence("ItemType is null");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                Validate<string>(xamlType.ItemType.Name, expectedItemTypeName);
            }
        }

        /// <summary>
        /// Tests IsCollection and ItemType throwing SchemaException
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedIsCollection">Expected value of IsCollection</param>
        public void IsCollectionAndItemTypeExceptionTest(Type testType, bool expectedIsCollection)
        {
            XamlType xamlType = GetXamlType(testType);
            Validate<bool>(xamlType.IsCollection, expectedIsCollection);

            ExceptionHelper.ExpectException(delegate { object o = xamlType.ItemType; }, new XamlSchemaException());
        }

        /// <summary>
        /// Tests IsDictionary, KeyType and ItemType
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedIsDictionary">Expected value of IsDictionary</param>
        /// <param name="expectedKeyTypeName">Expected value of KeyType</param>
        /// <param name="expectedItemTypeName">Expected value of ItemType</param>
        public void IsDictionaryAndKeyTypeAndItemTypeTest(Type testType, bool expectedIsDictionary, string expectedKeyTypeName, string expectedItemTypeName)
        {
            XamlType xamlType = GetXamlType(testType);
            Validate<bool>(xamlType.IsDictionary, expectedIsDictionary);

            if (xamlType.KeyType == null)
            {
                GlobalLog.LogEvidence("KeyType is null");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                Validate<string>(xamlType.KeyType.Name, expectedKeyTypeName);
            }

            if (xamlType.ItemType == null)
            {
                GlobalLog.LogEvidence("ItemType is null");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                Validate<string>(xamlType.ItemType.Name, expectedItemTypeName);
            }
        }

        /// <summary>
        /// Tests IsDictionary and ItemType throwing SchemaException
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedIsDictionary">Expected value of IsDictionary</param>
        public void IsDictionaryAndItemTypeExceptionTest(Type testType, bool expectedIsDictionary)
        {
            XamlType xamlType = GetXamlType(testType);
            Validate<bool>(xamlType.IsDictionary, expectedIsDictionary);

            ExceptionHelper.ExpectException(delegate { object o = xamlType.KeyType; }, new XamlSchemaException());
            ExceptionHelper.ExpectException(delegate { object o = xamlType.ItemType; }, new XamlSchemaException());
        }

        /// <summary>
        /// Tests GetAddMethod
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="contentType">Input parameter to GetAddMethod</param>
        /// <param name="expectedParameterTypeNames">Expected parameter types</param>
        public void GetAddMethodTest(Type testType, Type contentType, string[] expectedParameterTypeNames)
        {
            XamlType xamlType = GetXamlType(testType);
            XamlType contentXamlType = GetXamlType(contentType);

            MethodInfo methodInfo = xamlType.Invoker.GetAddMethod(contentXamlType);

            if (!Validate<string>(methodInfo.ReturnType.Name, "Void") || !Validate<string>(methodInfo.Name, "Add"))
            {
                return;
            }

            if (xamlType.IsCollection == false && xamlType.IsDictionary == false)
            {
                if (methodInfo != null)
                {
                    GlobalLog.LogEvidence("Add method found for Non-Collection/Non-Dictionary");
                    TestLog.Current.Result = TestResult.Fail;
                    return;
                }
            }
            else
            {
                int expectedParameterCount = 0;
                if (xamlType.IsCollection == true)
                {
                    GlobalLog.LogDebug("xamlType.IsCollection is true");
                    expectedParameterCount = 1;
                }
                else
                {
                    GlobalLog.LogDebug("xamlType.IsDictionary is true");
                    expectedParameterCount = 2;
                }

                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length != expectedParameterCount)
                {
                    GlobalLog.LogEvidence("Invalid number of parameters: " + parameters.Length.ToString(CultureInfo.CurrentCulture) + ", Expected=" + expectedParameterCount.ToString(CultureInfo.CurrentCulture));
                    TestLog.Current.Result = TestResult.Fail;
                    return;
                }

                if (expectedParameterTypeNames.Length != expectedParameterCount)
                {
                    GlobalLog.LogEvidence("Invalid parameterTypeNames value passed by testcase. Ignoring parameter type checking");
                    return;
                }

                for (int index = 0; index < parameters.Length; index++)
                {
                    Validate<string>(parameters[index].ParameterType.Name, expectedParameterTypeNames[index]);
                }
            }
        }

        /// <summary>
        /// Tests GetEnumeratorMethod
        /// </summary>
        /// <param name="testType">Type to test</param>
        public void GetEnumeratorMethodTest(Type testType)
        {
            XamlType xamlType = GetXamlType(testType);
            MethodInfo methodInfo = xamlType.Invoker.GetEnumeratorMethod();

            if (!Validate<string>(methodInfo.ReturnType.Name, "IEnumerator") || !Validate<string>(methodInfo.Name, "GetEnumerator"))
            {
                return;
            }

            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters != null && parameters.Length != 0)
            {
                GlobalLog.LogEvidence("Invalid number of parameters: " + parameters.Length.ToString(CultureInfo.CurrentCulture) + ", Expected=0");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }
            else
            {
                GlobalLog.LogEvidence("No parameters as expected");
            }
        }

        /// <summary>
        /// Tests GetPositionalParameters
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="parameterCount">Number of constructor arguments</param>
        /// <param name="expectedPositionalParameterTypeNames">Expected positional parameter types</param>
        public void GetPositionalParametersTest(Type testType, int parameterCount, string[] expectedPositionalParameterTypeNames)
        {
            ValidateCollectionOrdered(GetXamlType(testType).GetPositionalParameters(parameterCount), expectedPositionalParameterTypeNames);
        }

        /// <summary>
        /// Tests IsAssignableFrom
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="parameterType">Input parameter to IsAssignableFrom</param>
        /// <param name="expectedValue">Expected value</param>
        public void IsAssignableFromTest(Type testType, Type parameterType, bool expectedValue)
        {
            Validate<bool>(GetXamlType(parameterType).CanAssignTo(GetXamlType(testType)), expectedValue);
        }

        /// <summary>
        /// Tests ToString
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedValue">Expected string</param>
        public void ToStringTest(Type testType, string expectedValue)
        {
            Validate<string>(GetXamlType(testType).ToString(), expectedValue);
        }

        /// <summary>
        /// Tests GetProperty
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="propertyName">Property to get</param>
        /// <param name="expectedPropertyType">Expected property's type</param>
        public void GetMemberTest(Type testType, string propertyName, Type expectedPropertyType)
        {
            XamlMember xamlProperty = GetXamlType(testType).GetMember(propertyName);
            Validate<Type>(xamlProperty.Type.UnderlyingType, expectedPropertyType);
            Validate<string>(xamlProperty.Name, propertyName);
        }

        /// <summary>
        /// Tests GetProperty
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="argument">Input argument to GetProperty</param>
        /// <param name="expectedPropertyName">Name of expected property</param>
        /// <param name="expectedPropertyType">Type of expected property</param>
        public void GetAliasedPropertyTest(Type testType, XamlDirective argument, string expectedPropertyName, Type expectedPropertyType)
        {
            XamlMember xamlProperty = GetXamlType(testType).GetAliasedProperty(argument);
            Validate<Type>(xamlProperty.Type.UnderlyingType, expectedPropertyType);
            Validate<string>(xamlProperty.Name, expectedPropertyName);
        }

        /// <summary>
        /// Tests GetAttachableProperty
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="propertyName">Property to get</param>
        /// <param name="expectedPropertyType">Expectected property's type</param>
        public void GetAttachableMemberTest(Type testType, string propertyName, Type expectedPropertyType)
        {
            XamlMember xamlProperty = GetXamlType(testType).GetAttachableMember(propertyName);
            Validate<Type>(xamlProperty.Type.UnderlyingType, expectedPropertyType);
            Validate<string>(xamlProperty.Name, propertyName);
        }

        /// <summary>
        /// Tests GetAllMembers
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedMemberNames">Names of expected members</param>
        public void GetAllMembersTest(Type testType, string[] expectedMemberNames)
        {
            ValidateCollectionUnordered(GetXamlType(testType).GetAllMembers(), expectedMemberNames);
        }

        /// <summary>
        /// Tests GetAllAttachableMembers
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedMemberNames">Names of expected attachable members</param>
        public void GetAllAttachableMembersTest(Type testType, string[] expectedMemberNames)
        {
            ValidateCollectionUnordered(GetXamlType(testType).GetAllAttachableMembers(), expectedMemberNames);
        }

        /// <summary>
        /// Tests GetXamlNamespaces
        /// </summary>
        /// <param name="testType">Type to test</param>
        /// <param name="expectedNamespaces">Array of expected namespaces</param>
        public void GetXamlNamespacesTest(Type testType, string[] expectedNamespaces)
        {
            ValidateCollectionOrdered(GetXamlType(testType).GetXamlNamespaces(), expectedNamespaces);
        }

        #region Helper functions

        /// <summary>
        /// Validates any generic type
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="testValue">Value to test</param>
        /// <param name="expectedValue">Expected value</param>
        /// <returns>True if both match</returns>
        public bool Validate<T>(T testValue, T expectedValue)
        {
            GlobalLog.LogEvidence("Expected value: " + (expectedValue != null ? expectedValue.ToString() : "null"));
            GlobalLog.LogEvidence("Returned value: " + (testValue != null ? testValue.ToString() : "null"));
            if ((testValue != null && !testValue.Equals(expectedValue))
                || (expectedValue != null && !expectedValue.Equals(testValue)))
            {
                TestLog.Current.Result = TestResult.Fail;
                return false;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
                return true;
            }
        }

        /// <summary>
        /// Compare two collections in order
        /// </summary>
        /// <param name="xamlTypes">List of XamlTypes</param>
        /// <param name="expectedNames">Expected names</param>
        public void ValidateCollectionOrdered(IList<XamlType> xamlTypes, string[] expectedNames)
        {
            List<string> inputList = new List<string>();
            List<string> expectedList = new List<string>(expectedNames);

            foreach (XamlType xamlType in xamlTypes)
            {
                inputList.Add(xamlType.Name);
            }

            ValidateCollectionOrdered(inputList, expectedList);
        }

        /// <summary>
        /// Compare two collections in order
        /// </summary>
        /// <param name="inputList">Input list</param>
        /// <param name="expectedList">Expected list</param>
        public void ValidateCollectionOrdered(IList<string> inputList, IList<string> expectedList)
        {
            if (inputList.Count != expectedList.Count)
            {
                GlobalLog.LogEvidence("Expected " + expectedList.Count.ToString(CultureInfo.CurrentCulture) + " elements in list");
                GlobalLog.LogEvidence("Expected collection: ");
                PrintCollection(expectedList);
                GlobalLog.LogEvidence("Got " + inputList.Count.ToString(CultureInfo.CurrentCulture) + " elements in list");
                GlobalLog.LogEvidence("Got collection: ");
                PrintCollection(inputList);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            for (int index = 0; index < inputList.Count; index++)
            {
                Validate<string>(inputList[index], expectedList[index]);
            }
        }

        /// <summary>
        /// Compare two collections unordered
        /// </summary>
        /// <param name="xamlTypes">ReadOnlyCollection of XamlTypes</param>
        /// <param name="expectedNames">Expected names</param>
        public void ValidateCollectionUnordered(ICollection<XamlType> xamlTypes, string[] expectedNames)
        {
            List<string> inputList = new List<string>();
            List<string> expectedList = new List<string>(expectedNames);

            foreach (XamlType xamlType in xamlTypes)
            {
                inputList.Add(xamlType.Name);
            }

            ValidateCollectionUnordered(inputList, expectedList);
        }

        /// <summary>
        /// Compare two collections unordered
        /// </summary>
        /// <typeparam name="T">Type argument</typeparam>
        /// <param name="xamlProperties">IEnumrable of XamlProperties</param>
        /// <param name="expectedPropertyNames">Expected property names</param>
        public void ValidateCollectionUnordered<T>(ICollection<T> xamlProperties, string[] expectedPropertyNames) where T : XamlMember
        {
            List<string> inputList = new List<string>();
            List<string> expectedList = new List<string>(expectedPropertyNames);

            foreach (XamlMember xamlProperty in xamlProperties)
            {
                inputList.Add(xamlProperty.Name);
            }

            ValidateCollectionUnordered(inputList, expectedList);
        }

        /// <summary>
        /// Compare two collections unordered
        /// </summary>
        /// <param name="inputList">Input list</param>
        /// <param name="expectedList">Expected list</param>
        public void ValidateCollectionUnordered(IList<string> inputList, IList<string> expectedList)
        {
            bool testPassed = true;

            if (inputList.Count != expectedList.Count)
            {
                GlobalLog.LogEvidence("Expected " + expectedList.Count.ToString(CultureInfo.CurrentCulture) + " elements in collection");
                GlobalLog.LogEvidence("Expected collection: ");
                PrintCollection(expectedList);
                GlobalLog.LogEvidence("Got " + inputList.Count.ToString(CultureInfo.CurrentCulture) + " elements in collection");
                GlobalLog.LogEvidence("Got collection: ");
                PrintCollection(inputList);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            for (int index = 0; index < inputList.Count; index++)
            {
                if (!expectedList.Contains(inputList[index]))
                {
                    GlobalLog.LogEvidence("Got unexpected type " + inputList[index]);
                    testPassed = false;
                }
                else
                {
                    GlobalLog.LogEvidence("Got expected type " + inputList[index]);
                }
            }

            if (testPassed == true)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Prints a collection
        /// </summary>
        /// <param name="list">List of strings</param>
        public void PrintCollection(IList<string> list)
        {
            foreach (string item in list)
            {
                GlobalLog.LogEvidence(item);
            }
        }

        /// <summary>
        /// GetXamlType from current schema context
        /// </summary>
        /// <param name="testType">Type object</param>
        /// <returns>Obtained XamlType</returns>
        public XamlType GetXamlType(Type testType)
        {
            GlobalLog.LogDebug("Type is " + testType.ToString());
            return SchemaContext.GetXamlType(testType);
        }

        #endregion
    }
}
