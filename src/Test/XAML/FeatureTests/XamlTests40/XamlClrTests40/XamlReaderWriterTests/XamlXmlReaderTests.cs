// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;

    public class XamlXmlReaderTests
    {
        /// <summary>
        /// When the GetNamespacesInScope of the XmlReader returns
        /// null, XamlXmlReader was throwing Null ref - this is a
        /// regression test for WF Designer
        /// </summary>
        [SecurityLevel(SecurityLevel.FullTrust)]
        [TestCase]
        public void NullNamescopeTest()
        {
            string xaml = XamlServices.Save(32);
            using (XmlReader xmlReader = new NullScopeXmlReader(new StringReader(xaml)))
            {
                IXmlNamespaceResolver resolver = xmlReader as IXmlNamespaceResolver;
                var foo = resolver.GetNamespacesInScope(XmlNamespaceScope.Local);
                using (XamlXmlReader reader = new XamlXmlReader(xmlReader))
                {
                    Tracer.LogTrace(reader.ToString());
                }
            }
        }

        /// <summary>
        /// IsWhitespaceSignificantCollection should always be true on Unknown types
        /// </summary>
        [TestCase]
        public void UnknownTypesAndIsWhitespaceSignificantCollection()
        {
            string[] xamlDocs = 
            {
                @"<Foo xmlns='Unknown'><Foo.Bar> <Baz/></Foo.Bar></Foo>",
                @"<Foo xml:space='preserve' xmlns='Unknown'><Foo.Bar> <Baz/></Foo.Bar></Foo>"
            };

            foreach (var xaml in xamlDocs)
            {
                using (XamlXmlReader reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml))))
                {
                    while (reader.Read())
                    {
                        switch(reader.NodeType)
                        {
                            case XamlNodeType.StartObject:
                                if (reader.Type.IsUnknown)
                                {
                                    if (!reader.Type.IsWhitespaceSignificantCollection)
                                    {
                                        throw new DataTestException("Unknown XamlType not a IsWhitespaceSignificantCollection");
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

    }

    public class NullScopeXmlReader : XmlTextReader, IXmlNamespaceResolver
    {
        public NullScopeXmlReader(TextReader textReader)
            : base(textReader)
        {

        }
        public new IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
        {
            return null;
        }
    }
}
