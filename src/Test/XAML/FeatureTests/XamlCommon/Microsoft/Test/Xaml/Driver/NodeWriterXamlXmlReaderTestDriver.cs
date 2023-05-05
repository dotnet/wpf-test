// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Driver
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;

    /// <summary>
    /// Test Driver:
    /// * Write the given NodeList using the NodeListXmlWriter (test code)
    /// * Read the Xml using XamlXmlReader (product code)
    /// * validate the NodeList
    /// </summary>
    public class NodeWriterXamlXmlReaderTestDriver : XamlTestDriverBase
    {
        /// <summary>
        /// Execute the test
        /// </summary>
        /// <param name="source">test case idenfier</param>
        /// <param name="testCaseInfo">test case information</param>
        protected override void ExecuteTest(string source, TestCaseInfo testCaseInfo)
        {
            this.TraceCache.WriteLine("XamlXmlWriterXamlXmlReaderTestDriver: Test started - " + source);

            var inputNodes = (NodeList)testCaseInfo.Target;
            this.TraceCache.WriteLine("Input Node List:");
            this.TraceCache.WriteLine(inputNodes.ToString());

            //// Write node list to Xml //
            NodeListXmlWriter nodeListXmlWriter = new NodeListXmlWriter();
            string xml = nodeListXmlWriter.WriteNodes(inputNodes);
            NodeList expectedNodeList = nodeListXmlWriter.ExpectedNodeList;

            this.TraceCache.WriteLine("Created Xaml is ");
            this.TraceCache.WriteLine(xml);
            this.TraceCache.WriteLine(string.Empty);

            //// Read from Xaml into a nodelist //
            XamlSchemaContext xsc = new XamlSchemaContext();
            StringReader stringReader = new StringReader(xml);
            NodeList actualNodeList = null;
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                using (XamlXmlReader reader2 = new XamlXmlReader(xmlReader, xsc))
                {
                    using (XamlNodeWriter writer2 = new XamlNodeWriter(xsc))
                    {
                        XamlServices.Transform(reader2, writer2);
                        actualNodeList = writer2.Result;
                    }
                }
            }

            this.TraceCache.WriteLine("Expected Nodes:");
            this.TraceCache.WriteLine(expectedNodeList.ToString());
            this.TraceCache.WriteLine("Actual Nodes:");
            this.TraceCache.WriteLine(actualNodeList.ToString());

            if (expectedNodeList.ToString() != actualNodeList.ToString())
            {
                string message = string.Format(
                                                CultureInfo.InvariantCulture,
                                                "Nodes do not match.\r\nExpected:\r\n {0}\r\nActual:\r\n{1}",
                                                expectedNodeList.ToString(),
                                                actualNodeList.ToString());

                throw new TestCaseFailedException(message);
            }

            this.TraceCache.WriteLine("XamlXmlWriterXamlXmlReaderTestDriver: Test Succeeded");
        }
    }
}
