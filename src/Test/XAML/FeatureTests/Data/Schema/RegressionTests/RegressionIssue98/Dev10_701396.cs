// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xaml;
using System.Xml;
using AssemblyA;
using AssemblyB;

namespace Microsoft.Test.Xaml.Schema
{
    [SchemaTest]
    class RegressionIssue98
    {
        // Regression test
        public void Run()
        {
            List<Assembly> referenceAssemblies = new List<Assembly>();
            referenceAssemblies.Add(typeof(TypeA).Assembly);
            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext(referenceAssemblies);

            // This line adds a non-reference assembly to _xmlNsInfo collection in XamlSchemaContext
            IList<string> list = xamlSchemaContext.GetXamlType(typeof(TypeB)).GetXamlNamespaces();

            string xaml = @"<TypeA xmlns='http://c' />";
            TextReader textReader = new StringReader(xaml);
            using (XmlReader xmlReader = XmlReader.Create(textReader))
            {
                XamlXmlReader xamlReader = new XamlXmlReader(xmlReader, xamlSchemaContext);
                XamlObjectWriter xamlWriter = new XamlObjectWriter(xamlSchemaContext);
                try
                {
                    XamlServices.Transform(xamlReader, xamlWriter);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Unexpected exception: " + exception.ToString());
                    Environment.Exit(1);
                }
            }

            Environment.Exit(0);
        }
    }
}
