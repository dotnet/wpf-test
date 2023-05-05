// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Xaml;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public static class DomServices
    {
        public static ObjectNode Load(string fileName)
        {
            return Load(fileName, new XamlSchemaContext());
        }


        public static ObjectNode Load(string fileName, XamlSchemaContext schemaContext)
        {
            XmlReader xr = XmlReader.Create(fileName);
            XamlXmlReaderSettings xxrs = new XamlXmlReaderSettings();
            xxrs.ProvideLineInfo = true;
            XamlXmlReader xxr = new XamlXmlReader(xr, schemaContext, xxrs);

            return ((ObjectNode)Load(xxr));
        }

        public static DomNode Load(XamlReader xxr)
        {
            IXamlLineInfo ixli = xxr as IXamlLineInfo;

            DomWriter dw = new DomWriter(xxr.SchemaContext);
            while (xxr.Read())
            {
                dw.WriteNode(xxr);
                if (ixli != null)
                {
                    if (dw.LastObjectNodeWritten != null && dw.LastObjectNodeWritten.LineNumber == 0)
                    {
                        dw.LastObjectNodeWritten.LineNumber = ixli.LineNumber;
                        dw.LastObjectNodeWritten.LinePosition = ixli.LinePosition;
                    }
                    else if (dw.LastMemberNodeWritten != null && dw.LastMemberNodeWritten.LineNumber == 0)
                    {
                        dw.LastMemberNodeWritten.LineNumber = ixli.LineNumber;
                        dw.LastMemberNodeWritten.LinePosition = ixli.LinePosition;
                    }
                }
            }
            return dw.RootNode;
        }

        public static void WriteDomIntoFile(ObjectNode rootObjectNode, string fileName)
        {
            //write it back out
            XamlSchemaContext schemaContext = rootObjectNode.Type.SchemaContext;
            DomReader dr = new DomReader(rootObjectNode, schemaContext);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create(fileName, xws);
            XamlXmlWriter xxw = new XamlXmlWriter(xw, schemaContext);
            XamlServices.Transform(dr, xxw);
        }
    }
}
