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
    public static class XamlDomServices
    {
        public static XamlDomObject Load(string fileName)
        {
            return Load(fileName, new XamlSchemaContext());
        }


        public static XamlDomObject Load(string fileName, XamlSchemaContext schemaContext)
        {
            XamlDomObject rootObject = null;
            using (XmlReader xr = XmlReader.Create(fileName))
            {
                XamlXmlReaderSettings xxrs = new XamlXmlReaderSettings();
                xxrs.ProvideLineInfo = true;
                XamlXmlReader xxr = new XamlXmlReader(xr, schemaContext, xxrs);

                rootObject = ((XamlDomObject)Load(xxr));
            }
            return rootObject;
        }

        public static XamlDomNode Load(XamlReader xxr)
        {
            IXamlLineInfo ixli = xxr as IXamlLineInfo;

            XamlDomWriter dw = new XamlDomWriter(xxr.SchemaContext);
            XamlServices.Transform(xxr, dw);
            return dw.RootNode;
        }

        public static void Save(XamlDomObject rootObjectNode, string fileName)
        {
            XamlSchemaContext schemaContext = rootObjectNode.Type.SchemaContext;
            XamlDomReader dr = new XamlDomReader(rootObjectNode, schemaContext);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            using (XmlWriter xw = XmlWriter.Create(fileName, xws))
            {
                XamlXmlWriter xxw = new XamlXmlWriter(xw, schemaContext);
                XamlServices.Transform(dr, xxw);
            }
        }
    }
}
