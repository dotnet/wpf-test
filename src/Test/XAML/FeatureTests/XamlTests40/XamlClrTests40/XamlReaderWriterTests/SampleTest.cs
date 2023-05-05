// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments;

    public class SampleTest : XamlReaderWriterTest
    {
        public override IEnumerable<NodeList> NodeLists
        {
            get
            {
                return new List<NodeList>
                           {
                               SampleDocuments.SampleDoc,
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
        public void SampleXamlReaderWriterTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, TestCases, MethodInfo.GetCurrentMethod());
        }

        public void SampleXamlReaderWriterTestMethod(string instanceId)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(TestCases, instanceId));
        }
    }
}
