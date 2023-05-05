// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments;

    internal partial class XamlRecordMemberXDataTests
    {
        public class XDataTests : XamlReaderWriterTest
        {
            public override IEnumerable<NodeList> NodeLists
            {
                get
                {
                    return new List<NodeList>
                               {
                                   XamlRecordMemberXDataDocuments.GetXDataDocument("2008", true),
                                   XamlRecordMemberXDataDocuments.GetXDataDocument("2006", false),
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

            // Reevaulate XData tests
            //[TestCaseGenerator()]
            public void XDataTest(AddTestCaseEventHandler addCase)
            {
                XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
            }

            public void XDataTestMethod(string instanceId)
            {
                XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
            }
        }

        public partial class RecordMemberTests : XamlReaderWriterTest
        {
            public override IEnumerable<NodeList> NodeLists
            {
                get
                {
                    return new List<NodeList>
                               {
                                   XamlRecordMemberXDataDocuments.GetMixedValues("Unwrapped", true),
                                   XamlRecordMemberXDataDocuments.GetMixedValues("Wrapped", false),
                                   XamlRecordMemberXDataDocuments.GetMixedValuesNoXData("Unwrapped+NoXData", true),
                                   XamlRecordMemberXDataDocuments.GetMixedValuesNoXData("Wrapped+NoXData", false),
                                   XamlRecordMemberXDataDocuments.GetInterspesedMember("Interspersed")
                               };
                }
            }

            public override ICollection<TestCaseInfo> TestCases
            {
                get
                {
                    ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);

                    foreach (TestCaseInfo testCase in testCases.Where((tc) => tc.TestID.Contains("XamlReader+XmlWriter+Interspersed")))
                    {
                        testCase.ExpectedResult = false;
                        testCase.ExpectedMessage = "Validation errors:\nUnexpected step(s) found UserTrace: Member:{clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments;assembly=XamlClrTypes}InterspersedMemberType: (actual 2/req 1/opt 0)\r\nUnexpected step(s) found UserTrace: EndMember:{clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments;assembly=XamlClrTypes}InterspersedMemberType: (actual 2/req 1/opt 0)\r\n";
                    }

                    foreach (TestCaseInfo testCase in testCases.Where((tc) => tc.TestID.Contains("XamlTextWriter+Interspersed")))
                    {
                        testCase.ExpectedResult = false;
                        testCase.ExpectedMessage = Exceptions.GetMessage("XamlWriterWriteNotSupportedInCurrentState", WpfBinaries.SystemXaml);
                    }

                    return testCases;
                }
            }

            // Reevaulate XData tests
            //[TestCaseGenerator()]
            public void RecordMemberTest(AddTestCaseEventHandler addCase)
            {
                XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
            }

            public void RecordMemberTestMethod(string instanceId)
            {
                XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
            }
        }
    }
}
