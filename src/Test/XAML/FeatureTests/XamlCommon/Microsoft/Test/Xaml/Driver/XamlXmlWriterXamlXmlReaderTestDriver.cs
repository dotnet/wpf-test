// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xaml;
using System.Xml;
using Microsoft.Test.CDFInfrastructure;
using Microsoft.Test.Xaml.Common;
using Microsoft.Test.Xaml.Common.XamlOM;
using Microsoft.Test.Xaml.Driver;

namespace Microsoft.Test.Xaml.Driver
{
    /// <summary>
    /// Test Driver
    /// * Write the input NodeList using XamlXmlWriter 
    /// * Read the NodeList using XamlXmlReader 
    /// * Validate the NodeLists
    /// </summary>
    public class XamlXmlWriterXamlXmlReaderTestDriver : XamlTestDriverBase
    {
        /// <summary>
        /// Execute the test case
        /// </summary>
        /// <param name="source">source test identifier</param>
        /// <param name="testCaseInfo">test case information</param>
        protected override void ExecuteTest(string source, TestCaseInfo testCaseInfo)
        {
            this.TraceCache.WriteLine("NodeWriterXamlXmlReaderTestDriver: Test started - " + source);

            NodeList xamlNodes = (NodeList)testCaseInfo.Target;

            this.TraceCache.WriteLine("Input Node List");
            this.TraceCache.WriteLine(xamlNodes.ToString());

            // Strip out ignored nodes //
            NodeList inputNodes = NodeList.Empty;
            bool ignored = false;
            foreach (Node node in xamlNodes)
            {
                if (!node.TestMetadata.GetBoolProperty(TestMetadata.XXWIgnoreNode))
                {
                    inputNodes.Add(node);
                }
                else
                {
                    ignored = true;
                }
            }

            if (ignored)
            {
                this.TraceCache.WriteLine("Input Node List after Ignores");
                this.TraceCache.WriteLine(inputNodes.ToString());
            }

            // Write node list to Xaml //
            XamlSchemaContext xsc = new XamlSchemaContext();
            string xaml = string.Empty;
            using (XamlNodeReader xamlNodeReader = new XamlNodeReader(inputNodes, xsc))
            {
                StringWriter stringWriter = new StringWriter();
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = true,
                };

                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
                {
                    using (XamlXmlWriter xamlXmlWriter = new XamlXmlWriter(xmlWriter, xsc))
                    {
                        XamlServices.Transform(xamlNodeReader, xamlXmlWriter);
                    }
                }

                xaml = stringWriter.ToString();
            }

            this.TraceCache.WriteLine("Xaml is ");
            this.TraceCache.WriteLine(xaml);
            this.TraceCache.WriteLine(string.Empty);

            // Read from Xaml into a NodeList //
            StringReader stringReader = new StringReader(xaml);
            NodeList actualNodes = null;
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                using (XamlXmlReader xamlXmlReader = new XamlXmlReader(xmlReader, xsc))
                {
                    using (XamlNodeWriter xamlNodeWriter = new XamlNodeWriter(xsc))
                    {
                        XamlServices.Transform(xamlXmlReader, xamlNodeWriter);
                        actualNodes = xamlNodeWriter.Result;
                    }
                }
            }

            // Strip NS nodes //
            NodeList expectedNodes = new NodeList();
            foreach (Node node in xamlNodes)
            {
                if (node.GetType() != typeof(NamespaceNode) && !NodeListXmlWriter.GetDoNotExpectTag(node.TestMetadata))
                {
                    expectedNodes.Add(node);
                }
            }

            this.TraceCache.WriteLine("Expected Nodes:");
            this.TraceCache.WriteLine(expectedNodes.ToString());
            this.TraceCache.WriteLine("Actual Nodes:");
            this.TraceCache.WriteLine(actualNodes.ToString());

            if (expectedNodes.ToString() != actualNodes.ToString())
            {
                throw new TestCaseFailedException("Nodes did not match");
            }

            this.TraceCache.WriteLine("NodeWriterXamlXmlReaderTestDriver: Test Successful");
        }
    }
}

