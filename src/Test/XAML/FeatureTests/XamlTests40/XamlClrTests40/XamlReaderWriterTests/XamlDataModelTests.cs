// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Utilities;

    public class DuplicateMemberTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDataModelDocuments.GetDuplicateMemberDoc("Attribute", true), 
                               XamlDataModelDocuments.GetDuplicateMemberDoc("Element", false), 
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                ICollection<TestCaseInfo> testCases = GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);

                foreach (TestCaseInfo testCase in
                    testCases.Where<TestCaseInfo>(tc => tc.TestID.Contains("NodeWriterXamlXmlReaderDriver+Attribute")))
                {
                    testCase.ExpectedResult = false;
                    testCase.ExpectedMessage = Exceptions.GetMessage("Xml_DupAttributeName", typeof(XmlWriter).Assembly, "System.Xml");
                }

                foreach (TestCaseInfo testCase in
                    testCases.Where<TestCaseInfo>(tc => tc.TestID.Contains("XamlXmlWriter")))
                {
                    testCase.ExpectedResult = false;
                    testCase.ExpectedMessage = Exceptions.GetMessage("XamlXmlWriterDuplicateMember", WpfBinaries.SystemXaml);
                }

                return testCases;
            }
        }

        [TestCaseGenerator()]
        public void DuplicateMemberTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void DuplicateMemberTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class MemberDifferentTypeTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDataModelDocuments.GetMemberDifferentTypeDoc("Attribute", true),
                               XamlDataModelDocuments.GetMemberDifferentTypeDoc("Element", false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                return GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
        }

        [TestCaseGenerator()]
        public void MemberDifferentTypeTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void MemberDifferentTypeTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class BasicRecordContainingMemberTests : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               XamlDataModelDocuments.GetUnnamedMemberDoc("UnnamedMember"),
                               XamlDataModelDocuments.GetMultipleMembersDoc("Multiple+Attribute", true),
                               XamlDataModelDocuments.GetMultipleMembersDoc("Multiple+Element", false),
                               XamlDataModelDocuments.GetMixNamedUnnamedMembersDoc("Mixed+Attribute", true),
                               XamlDataModelDocuments.GetMixNamedUnnamedMembersDoc("Mixed+Element", false),
                           };
            }
        }

        public override ICollection<TestCaseInfo> TestCases
        {
            get
            {
                return GetTestCaseInfo(MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
        }

        [TestCaseGenerator()]
        public void BasicRecordContainingMemberTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void BasicRecordContainingMemberTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }

    public class DtdDocumentTests
    {
        // [DISABLED]
        // [TestCase]
        public void DtdTest()
        {
            string expectedMessage = Exceptions.GetMessage("Xml_DtdIsProhibitedEx", typeof(XmlWriter).Assembly, "System.Xml");
            ExceptionHelpers.CheckForException(typeof(XmlException), expectedMessage, () => XamlServices.Parse(XamlDataModelDocuments.GetDtdDocument()));
        }
    }
}
