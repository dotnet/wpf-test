// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xaml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.Xaml.Types.SchemaExtensibility;
    using Microsoft.Test.Xaml.XamlTests;

    public class SchemaExtensibilityTests
    {
        #region BasicSchemaExtensibility
        private List<Type> _basicSchemaExtensibilityTestTypes =
            new List<Type>
                {
                    typeof(SchemaExtensibilityHolder)
                };

        // [DISABLED]
        // [TestCaseGenerator]
        public void BasicSchemaExtensibilityTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._basicSchemaExtensibilityTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void BasicSchemaExtensibilityTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._basicSchemaExtensibilityTestTypes, instanceID));
        }

        #endregion

        #region NegativeSchemaExtensibility
        private List<Type> _negativeSchemaExtensibilityTestTypes =
            new List<Type>
                {
                    typeof(SchemaExtensibilityNegativeTests)
                };

        // [DISABLED] : Some variations pass, some fail
        // [TestCaseGenerator]
        public void NegativeSchemaExtensibilityTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._negativeSchemaExtensibilityTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void NegativeSchemaExtensibilityTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._negativeSchemaExtensibilityTestTypes, instanceID));
        }

        #endregion

        [TestCase]
        public void TryGetCompatibleNamespaceTest()
        {
            string compatNamespace;
            string validClrNamespace = "clr-namespace:Microsoft.Test.Xaml.XamlTests;assembly=XamlClrTypes";
            string validHttpNamespace = "http://testnamespace";
            string unknownNamespace = "urn:TheUnknownNamespace";
            var context = new XamlSchemaContext();

            context.TryGetCompatibleXamlNamespace(validClrNamespace, out compatNamespace);
            if (compatNamespace != validClrNamespace)
            {
                throw new Exception("Expected: " + validClrNamespace + " Actual: " + compatNamespace);
            }

            context.TryGetCompatibleXamlNamespace(validHttpNamespace, out compatNamespace);
            if (compatNamespace != validHttpNamespace)
            {
                throw new Exception("Expected: " + validHttpNamespace + " Actual: " + compatNamespace);
            }

            context.TryGetCompatibleXamlNamespace(unknownNamespace, out compatNamespace);
            if (compatNamespace != null)
            {
                throw new Exception("Expected: null Actual: " + compatNamespace);
            }
        }

        /// <summary>
        /// By default IsUnknown == (type.UnderlyingType != null), CustomXamlType has been modified
        /// so that if underlying type is null then IsUnknown is false
        /// </summary>
        [TestCase]
        public void LookupIsUnknownTest()
        {
            var context = new CustomSchemaContext();
            var type = new CustomXamlType(context);

            if (type.IsUnknown)
            {
                throw new DataTestException("Expected type.IsUnknown == false.");
            }
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void CollectibleAssembliesAreCollectedTest()
        {
            XamlSchemaContext xsc = new XamlSchemaContext();

            // .NET Core 3.0, this is no longer relevant in .NET Core.
            // AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Foo, Version=1.0"), AssemblyBuilderAccess.RunAndCollect);

            // bool isLoaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Foo") != null;

            // if (!isLoaded)
            // {
                // throw new DataTestException("DynamicAssembly not loaded when it should be.");
            // }

            // GC.Collect();
            // GC.WaitForPendingFinalizers();
            // GC.Collect();

            // isLoaded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Foo") != null;

            // if (isLoaded)
            // {
                // throw new DataTestException("DynamicAssembly loaded when it should not be.");
            // }
        }

        /// <summary>
        /// Add coverage for XamlSchemaContext.GetAllXamlTypes
        /// </summary>
        [TestCase]
        // this assembly has types that derive from types with inheritance demands so GetAllXamlTypes needs full trust
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void GetAllXamlTypesInXmlnsTest()
        {
            GetAllXamlTypesBase("http://testnamespace", "TestNamespace");
        }

        /// <summary>
        /// Add coverage for XamlSchemaContext.GetAllXamlTypes
        /// </summary>
        [TestCase]
        // this assembly has types that derive from types with inheritance demands so GetAllXamlTypes needs full trust
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void GetAllXamlTypesInClrnsTest()
        {
            GetAllXamlTypesBase("clr-namespace:TestNamespace;assembly=XamlClrTests40", "TestNamespace");
        }

        public void GetAllXamlTypesBase(string xamlNs, string clrNs)
        {
            var context = new XamlSchemaContext();
            int xamlCount = context.GetAllXamlTypes(xamlNs).Count;
            int reflectionCount = Assembly.GetExecutingAssembly().GetTypes().Count((t) => t.Namespace == clrNs);
            
            if (xamlCount != reflectionCount)
            {
                throw new DataTestException(string.Format("Expected {0} types. Actual {1} types.", reflectionCount, xamlCount));
            }
            
        }

        /// <summary>
        /// Add coverage for XamlDuplicateMemberException.GetObjectData (called by the runtime serialization stack)
        /// </summary>
        // [DISABLED]
        // [TestCase]
        // BinaryFormatter requires full trust
        [SecurityLevel(SecurityLevel.FullTrust)]
        
        public void XamlDuplicateMemberSerializabilityTest()
        {
            var context = new XamlSchemaContext();
            var classType1 = context.GetXamlType(typeof(ClassType1));
            var category = classType1.GetMember("Category");

            var exception = new XamlDuplicateMemberException(category, classType1);
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();

            formatter.Serialize(stream, exception);
            stream.Position = 0;
            var newException = (XamlDuplicateMemberException)formatter.Deserialize(stream);

            if (newException.DuplicateMember != category ||
                newException.ParentType != classType1)
            {
                throw new DataTestException("Exception did not successfully roundtrip.");
            }
        }
    }
}
