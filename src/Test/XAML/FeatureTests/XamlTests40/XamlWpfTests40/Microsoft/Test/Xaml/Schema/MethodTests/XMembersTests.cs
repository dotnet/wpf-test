// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Schema.MethodTests
{
    /// <summary>
    /// x:Members tests
    /// </summary>
    public class XMembersTests
    {
        /// <summary>
        /// Xaml schema context
        /// </summary>
        private readonly XamlSchemaContext _xamlSchemaContext = new XamlSchemaContext();

        /// <summary>
        /// Non-generic type test
        /// </summary>
        public void NonGenericTypeTest()
        {
            RoundTrip(typeof(string));
        }

        /// <summary>
        /// Generic type test
        /// </summary>
        public void GenericTypeTest()
        {
            RoundTrip(typeof(List<string>));
        }

        /// <summary>
        /// Non-generic unknown type test
        /// </summary>
        public void NonGenericUnknownTypeTest()
        {
            RoundTrip(new XamlType("http://UnknownNs", "NonGenericUnknownType", null, _xamlSchemaContext));
        }

        /// <summary>
        /// Generic unknown type with known type arguments test
        /// </summary>
        public void GenericUnknownTypeWithKnownTypeArgumentsTest()
        {
            RoundTrip(new XamlType("http://UnknownNs", "GenericUnknownTypeWithKnownTypeArguments", new XamlType[] { _xamlSchemaContext.GetXamlType(typeof(string)) }, _xamlSchemaContext));
        }

        /// <summary>
        /// Generic unknown type with unknown type arguments test
        /// </summary>
        public void GenericUnknownTypeWithUnknownTypeArgumentsTest()
        {
            RoundTrip(new XamlType("http://UnknownNs", "GenericUnknownTypeWithUnknownTypeArguments", new XamlType[] { new XamlType("http://UnknownNs", "UnknownType", null, _xamlSchemaContext) }, _xamlSchemaContext));
        }

        #region Private Helpers

        /// <summary>
        /// Roundtrip helper function
        /// </summary>
        /// <param name="type">Input type</param>
        private void RoundTrip(Type type)
        {
            RoundTrip(_xamlSchemaContext.GetXamlType(type));
        }

        /// <summary>
        /// Roundtrip helper function
        /// </summary>
        /// <param name="xamlType">Input XamlType</param>
        private void RoundTrip(XamlType xamlType)
        {
            PropertyDefinition property = new PropertyDefinition();
            property.Modifier = "public";
            property.Type = xamlType;
            property.Name = "CustomProperty";

            PropertyDefinition roundTripped = (PropertyDefinition)XamlServices.Parse(XamlServices.Save(property));
            Validate<string>(roundTripped.Modifier, property.Modifier);
            
            if (property.Type.TypeArguments != null && property.Type.TypeArguments.Count > 0)
            {
                if (property.Type.TypeArguments.Count != roundTripped.Type.TypeArguments.Count)
                {
                    GlobalLog.LogEvidence("Number of typearguments didn't match. Expected: " + property.Type.TypeArguments.Count + " Got: " + roundTripped.Type.TypeArguments.Count);
                }

                for (int index = 0; index < property.Type.TypeArguments.Count; index++)
                {
                    Validate<string>(roundTripped.Type.TypeArguments[index].Name, property.Type.TypeArguments[index].Name);
                }
            }

            if (property.Type.IsUnknown == false)
            {
                Validate<Type>(roundTripped.Type.UnderlyingType, property.Type.UnderlyingType);
            }
            else
            {
                Validate<string>(roundTripped.Type.PreferredXamlNamespace, property.Type.PreferredXamlNamespace);
                Validate<string>(roundTripped.Type.Name, property.Type.Name);
            }

            Validate<string>(roundTripped.Name, property.Name);
        }

        /// <summary>
        /// Validation helper function
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="testValue">Test value</param>
        /// <param name="expectedValue">Expected value</param>
        private void Validate<T>(T testValue, T expectedValue)
        {
            GlobalLog.LogEvidence("Expected value: " + expectedValue.ToString());
            GlobalLog.LogEvidence("Returned value: " + testValue.ToString());
            if (!testValue.Equals(expectedValue))
            {
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        #endregion
    }
}
