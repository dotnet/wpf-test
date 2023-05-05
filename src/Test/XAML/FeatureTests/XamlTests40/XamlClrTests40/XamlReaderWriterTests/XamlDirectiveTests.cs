// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: System.Windows.Markup.XmlnsDefinition("http://testnamespace", "TestNamespace")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://testnamespace", "x")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://testnamespace2", "TestNamespace2")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://testnamespace2", "x2")]

namespace TestNamespace
{
    public class ClassInTestNamespace
    {
        public string Data { get; set; }
    }
}

namespace TestNamespace2
{
    public class ClassInTestNamespace
    {
        public string Data { get; set; }
    }
}

namespace Microsoft.Test.Xaml.XamlReaderWriterTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xaml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.Xaml.Utilities;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments;
    using Microsoft.Test.Xaml.XamlTests;
    
    public class NamedRecordTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetNamedRecord("Attribute+2008", true, true),
                               XamlDirectiveDocuments.GetNamedRecord("Attribute+2006", true, false),
                               XamlDirectiveDocuments.GetNamedRecord("Element+2008", false, true),
                               XamlDirectiveDocuments.GetNamedRecord("Element+2006", false, false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void NamedRecordTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void NamedRecordTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class InvalidNamedRecordTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetEmptyName("Empty+Attribute+2008", true, true),
                               XamlDirectiveDocuments.GetEmptyName("Empty+Attribute+2006", true, false), 
                               XamlDirectiveDocuments.GetEmptyName("Empty+Element+2008", false, true), 
                               XamlDirectiveDocuments.GetEmptyName("Empty+Element+2006", false, false),
                               XamlDirectiveDocuments.GetNullName("Null+2008", true),
                               XamlDirectiveDocuments.GetNullName("Null+2006", false),
                               XamlDirectiveDocuments.GetPointName("Point+2008", true),
                               XamlDirectiveDocuments.GetPointName("Point+2006", false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);

                // XamlTextReader never writes the prefix in element form hence 
                // removing those tests as they will not match the expected traces
                List<TestCaseInfo> testsToRemove = new List<TestCaseInfo>();
                foreach (TestCaseInfo test in testCases)
                {
                    if (test.TestID.Contains("XamlXmlReader") && test.TestID.Contains("Element"))
                    {
                        testsToRemove.Add(test);
                    }
                }
                foreach (TestCaseInfo test in testsToRemove)
                {
                    testCases.Remove(test);
                }

                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void InvalidNamedRecordTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void InvalidNamedRecordTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class ReferencedRecordTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetReferencedRecord("Attribute+Explicit", true, true),
                               XamlDirectiveDocuments.GetReferencedRecord("Attribute+Implicit", true, false),
                               XamlDirectiveDocuments.GetReferencedRecord("Element+Explicit", false, true),
                               XamlDirectiveDocuments.GetReferencedRecord("Element+Implicit", false, false),
                               // Cannot write the invalid member name since it wont be in the XamlType //
                               // new NamedXamlDocument { Document = XamlDirectiveDocuments.GetReferencedRecordWithInvalidMember(true), Name = "Attribute+Invalid"},
                               // new NamedXamlDocument { Document = XamlDirectiveDocuments.GetReferencedRecordWithInvalidMember(false), Name = "Element+Invalid"}
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void ReferencedRecordTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void ReferencedRecordTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class RecordInitializationTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetCreateFromRecord("Attribute+CreateFrom", true),
                               XamlDirectiveDocuments.GetCreateFromRecord("Element+CreateFrom", false),
                               XamlDirectiveDocuments.GetMethodRecord("Attribute+Method+0", true, 0),
                               XamlDirectiveDocuments.GetMethodRecord("Attribute+Method+1", true, 1),
                               XamlDirectiveDocuments.GetMethodRecord("Attribute+Method+2", true, 2),
                               XamlDirectiveDocuments.GetMethodRecord("Element+Method+0", false, 0),
                               XamlDirectiveDocuments.GetMethodRecord("Element+Method+1", false, 1),
                               XamlDirectiveDocuments.GetMethodRecord("Element+Method+2", false, 2),
                               XamlDirectiveDocuments.GetCtorRecord("Ctor+0", 0),
                               XamlDirectiveDocuments.GetCtorRecord("Ctor+1", 1),
                               XamlDirectiveDocuments.GetCtorRecord("Ctor+2", 2),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void RecordInitializationTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void RecordInitializationTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class FactoryMethodRecordInitializationTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                              XamlDirectiveDocuments.GetCreateFromAndMethodRecord("Attribute+CreateFrom+Method", true),
                              XamlDirectiveDocuments.GetCreateFromAndMethodRecord("Element+CreateFrom+Method", false),
                              XamlDirectiveDocuments.GetCreateFromAndArgumentRecord("Attribute+CreateFrom+Args", true),
                              XamlDirectiveDocuments.GetCreateFromAndArgumentRecord("Element+CreateFrom+Args", false), 
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
               
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void InvalidRecordInitializationTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void InvalidRecordInitializationTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class DictionaryTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetDictionaryDocument("Element+2008", false, true), 
                               XamlDirectiveDocuments.GetDictionaryDocument("Element+2006", false, false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void DictionaryTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void DictionaryTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class InvalidKeyTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetKeyAtRoot("KeyAtRoot+2008", true),
                               XamlDirectiveDocuments.GetKeyAtRoot("KeyAtRoot+2006", false),
                               XamlDirectiveDocuments.GetInvalidKeyDoc("InvalidKeys+2008", true),
                               XamlDirectiveDocuments.GetInvalidKeyDoc("InvalidKeys+2006", false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void InvalidKeyTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void InvalidKeyTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class XamlTypeTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetTypeDoc("Attribute+2008+Explicit", true, true, true),
                               XamlDirectiveDocuments.GetTypeDoc("Attribute+2008+Implicit", true, true, false),
                               XamlDirectiveDocuments.GetTypeDoc("Attribute+2006+Explicit", true, false, true),
                               XamlDirectiveDocuments.GetTypeDoc("Attribute+2006+Implicit", true, false, false),
                               XamlDirectiveDocuments.GetTypeDoc("Element+2008+Explicit", false, true, true),
                               XamlDirectiveDocuments.GetTypeDoc("Element+2008+Implicit", false, true, false),
                               XamlDirectiveDocuments.GetTypeDoc("Element+2006+Explicit", false, false, true),
                               XamlDirectiveDocuments.GetTypeDoc("Element+2006+Implicit", false, false, false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void XamlTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void XamlTypeTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW
            )]
        public void XamlTypeDeserialization()
        {
            List<string> errors = new List<string>();
            foreach (NodeList doc in NodeLists)
            {
                string xaml = doc.NodeListToXml();
                try
                {
                    var obj = XamlServices.Parse(xaml);
                    Type value = obj as Type;
                    if (value == null || value != typeof(string))
                    {
                        errors.Add("Failed for " + doc.Name);
                    }
                }
                catch (Exception ex)
                {
                    errors.Add("Failed for " + doc.Name + " Exception - " + ex.ToString());
                }
            }

            if (errors.Count > 0)
            {
                throw new DataTestException(string.Join("\r\n", errors.ToArray()));
            }
        }
    }

    public class TypeArgumentTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetTypeArgDoc("Attribute+2006", true, true),
                               XamlDirectiveDocuments.GetTypeArgDoc("Element+2006", false, true),
                               XamlDirectiveDocuments.GetTypeArgDoc("Attribute+2008", true, false),
                               XamlDirectiveDocuments.GetTypeArgDoc("Element+2008", false, false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                ICollection<TestCaseInfo> filteredTestCases = new List<TestCaseInfo>();
                
                // [DISABLED] : Filtered some tests out
                foreach(TestCaseInfo testCase in testCases)
                {
                    if(!testCase.TestID.Contains("NodeWriterXamlXmlReaderDriver+Element"))
                    {
                        filteredTestCases.Add(testCase);
                    }
                }
                return filteredTestCases;
            }
        }

        [TestCaseGenerator()]
        public void TypeArgumentTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void TypeArgumentTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class ArrayTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetArrayDoc("Attribute+2008+Explicit", true, true, true),
                               XamlDirectiveDocuments.GetArrayDoc("Attribute+2008+Implicit", true, true, false),
                               XamlDirectiveDocuments.GetArrayDoc("Attribute+2006+Explicit", true, false, true), 
                               XamlDirectiveDocuments.GetArrayDoc("Attribute+2006+Implicit", true, false, false),
                               XamlDirectiveDocuments.GetArrayDoc("Element+2008+Explicit", false, true, true), 
                               XamlDirectiveDocuments.GetArrayDoc("Element+2008+Implicit", false, true, false), 
                               XamlDirectiveDocuments.GetArrayDoc("Element+2006+Explicit", false, false, true),
                               XamlDirectiveDocuments.GetArrayDoc("Element+2006+Implicit", false, false, false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void ArrayTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void ArrayTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class InvalidArrayTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetMissingTypeArrayDoc("2008+Explicit", true, true),
                               XamlDirectiveDocuments.GetMissingTypeArrayDoc("2008+Implicit", true, false),
                               XamlDirectiveDocuments.GetMissingTypeArrayDoc("2006+Explicit", false, true),
                               XamlDirectiveDocuments.GetMissingTypeArrayDoc("2006+Implicit", false, false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void InvalidArrayTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void InvalidArrayTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }

        // 
        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void MissingTypeForArrayTest()
        {
            NodeList doc = XamlDirectiveDocuments.GetMissingTypeArrayDoc("MissingTypeArray", false, false);

            string xaml = doc.NodeListToXaml();

            string expectedMessage = Exceptions.GetMessage("ProvideValue", WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => XamlServices.Parse(xaml));
        }

        // 
        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void IncorrectDirectiveNamespaceTest()
        {
            Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments.Point p = new Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments.Point
                          {
                              X = 42,
                              Y = 3
                          };

            string xaml = @"<Point X='42' Y='3' 
                            XNamespaceName='MyName' 
                            xmlns='clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments;assembly=XamlClrTests40' />";
            string expectedMessage =
               Exceptions.GetMessage("CantSetUnknownProperty", WpfBinaries.SystemXaml);

            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => XamlServices.Parse(xaml));
        }

        // 
        //[TestCase(
        //     Owner = "Microsoft",
        //     Category = TestCategory.IDW,
        //     TestType = TestType.Automated
        //     )]
        //public void InvalidLookupPrefixCallTest()
        //{
        //    XamlWriter writer = new XamlXmlWriter(new StringWriter());
        //    writer.WriteStartRecord(Namespaces.Namespace2006 + "Array");
        //    writer.WriteStartMember("Type", Namespaces.Namespace2006 + "Array");

        //    string expectedMessage =
        //        ResourceHelper.ExtractString("System.Runtime.Xaml.dll;System.Runtime.Xaml.SR;XamlWriterAssignPrefixIncorrectOrder");

        //    ExceptionHelpers.CheckForException(typeof(InvalidOperationException), expectedMessage, () => writer.LookupPrefix(Namespaces.NamespaceBuiltinTypes));
        //}


        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void NoReservedPrefixesTest()
        {
            var ns1 = new ObjectContainer() { Content = new TestNamespace.ClassInTestNamespace() };
            var ns2 = new ObjectContainer() { Content = new TestNamespace2.ClassInTestNamespace() };

            // We do not reserve any prefixes in the new stack //
            string xaml = XamlServices.Save(ns1);
            if (!xaml.Contains("xmlns:x=\"http://testnamespace\""))
            {
                throw new DataTestException("prefix x is not used: " + xaml);
            }

            xaml = XamlServices.Save(ns2);
            if (!xaml.Contains("xmlns:x2=\"http://testnamespace2\""))
            {
                throw new DataTestException("prefix x2 is not used: " + xaml);
            }
        }

        // [DISABLED]
        // [TestCase(
        //     Owner = "Microsoft",
        //     Category = TestCategory.IDW,
        //     TestType = TestType.Automated
        //     )]
        public void XmlnsOnMemberTest()
        {
            string xaml = @"<List x:TypeArguments='p:ClassInTestNamespace' Capacity='4' xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' xmlns:p='http://testnamespace2' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <p:ClassInTestNamespace>
                                    <p1:ClassInTestNamespace.Data xmlns:p1='http://testnamespace2'>
                                        Blah
                                    </p1:ClassInTestNamespace.Data>
                                </p:ClassInTestNamespace>
                            </List>";

            string expectedMessage =
               Exceptions.GetMessage("XmlnsNotAllowedOnMemberElementLineInfo", WpfBinaries.SystemXaml);

            ExceptionHelpers.CheckForException(typeof(System.Xaml.XamlParseException), expectedMessage, () => XamlServices.Parse(xaml));
        }

        // Revist this once we have the partial trust story for the system.xaml stack
        //[TestCase(
        //     Owner = "Microsoft",
        //     Category = TestCategory.IDW,
        //     TestType = TestType.Automated
        //     )]
        // public void AllowPartiallyTrustedCallersTest()
        // {
        //     if (typeof(XamlServices).Assembly.GetCustomAttributes(typeof(AllowPartiallyTrustedCallersAttribute), true).Length == 0)
        //     {
        //         throw new DataTestException("APTCA not found.");
        //     }
        // }
    }

    public class OtherDirectivesTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetOtherDirectivesDoc("Attribute+2008+Explicit", true, true, true),
                               XamlDirectiveDocuments.GetOtherDirectivesDoc("Attribute+2008+Implicit", true, true, false),
                               XamlDirectiveDocuments.GetOtherDirectivesDoc("Attribute+2006+Explicit", true, false, true),
                               XamlDirectiveDocuments.GetOtherDirectivesDoc("Attribute+2006+Implicit", true, false, false),
                               XamlDirectiveDocuments.GetOtherDirectivesDoc("Element+2008+Explicit", false, true, true),
                               XamlDirectiveDocuments.GetOtherDirectivesDoc("Element+2008+Implicit", false, true, false),
                               XamlDirectiveDocuments.GetOtherDirectivesDoc("Element+2006+Explicit", false, false, true),
                               XamlDirectiveDocuments.GetOtherDirectivesDoc("Element+2006+Implicit", false, false, false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void OtherDirectivesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void OtherDirectivesTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class InvalidFieldModifierTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetInvalidFieldModifierDoc("Attribute+2008", true, true),
                               XamlDirectiveDocuments.GetInvalidFieldModifierDoc("Attribute+2006", true, false),
                               XamlDirectiveDocuments.GetInvalidFieldModifierDoc("Element+2008", false, true),
                               XamlDirectiveDocuments.GetInvalidFieldModifierDoc("Element+2006", false, false), 
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator]
        public void InvalidFieldModifierTest(AddTestCaseEventHandler addCase)
        {
            // FieldModifier value isn't a string
            // FieldModifier not in a record with a Name member
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void InvalidFieldModifierTestMethod(string instanceId)
        {
            // FieldModifier value isn't a string
            // FieldModifier not in a record with a Name member
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class StaticTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDirectiveDocuments.GetStaticDoc("Attribute+2008+Explicit", true, true, true),
                               XamlDirectiveDocuments.GetStaticDoc("Attribute+2008+Implicit", true, true, false),
                               XamlDirectiveDocuments.GetStaticDoc("Attribute+2006+Explicit", true, false, true),
                               XamlDirectiveDocuments.GetStaticDoc("Attribute+2006+Implicit", true, false, false),
                               XamlDirectiveDocuments.GetStaticDoc("Element+2008+Explicit", false, true, true),
                               XamlDirectiveDocuments.GetStaticDoc("Element+2008+Implicit", false, true, false),
                               XamlDirectiveDocuments.GetStaticDoc("Element+2006+Explicit", false, false, true),
                               XamlDirectiveDocuments.GetStaticDoc("Element+2006+Implicit", false, false, false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
                return testCases;
            }
        }

        [TestCaseGenerator]
        public void StaticTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void StaticTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }
}
