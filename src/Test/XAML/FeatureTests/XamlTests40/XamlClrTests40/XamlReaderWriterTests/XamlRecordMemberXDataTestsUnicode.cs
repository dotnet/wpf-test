// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests
{
    using System;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Infrastructure.Test;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Utilities;

    internal partial class XamlRecordMemberXDataTests
    {
        public partial class RecordMemberTests : XamlReaderWriterTest
        {
            [TestCase(
                Owner = "Microsoft",
                Category = TestCategory.IDW,
                TestType = TestType.Automated
                )]
            public void MemberNameIsValidClrButNotXmlTest()
            {
                var data = new ClassWithInvalidMemberNameForXml()
                               {
                                   Pro‿p1 = "blah"
                               };

                var expected = Exceptions.GetMessage("MemberHasInvalidXamlName", WpfBinaries.SystemXaml);
                ExceptionHelpers.CheckForException(typeof (ArgumentException), expected, () => XamlServices.Save(data));
            }
        }
    }

    public class ClassWithInvalidMemberNameForXml
    {
        public string Pro‿p1 { get; set; }
    }
}
