// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    using System.IO;
    using System.Xaml;
    using System.Xml;

    /// <summary>
    /// Collection of transformations from NodeList to 
    /// other formats (xaml, xml, object)
    /// </summary>
    public static class NodeListTransforms
    {
        /// <summary>
        /// Create a node list given an object instance
        /// Uses XamlServices.Transform(XamlObjectReader,XamlNodeWriter [test code])
        /// </summary>
        /// <param name="instance">instance of the object</param>
        /// <returns>the created NodeList</returns>
        public static NodeList ObjectToNodeList(object instance)
        {
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            using (XamlObjectReader objectReader = new XamlObjectReader(instance, schemaContext))
            {
                using (XamlNodeWriter nodeWriter = new XamlNodeWriter(schemaContext))
                {
                    XamlServices.Transform(objectReader, nodeWriter);
                    return nodeWriter.Result;
                }
            }
        }

        /// <summary>
        /// Create xaml string given a NodeList
        /// Uses XamlServices.Transform(XamlNodeReader [test code], XamlXmlWriter)
        /// </summary>
        /// <param name="nodes">nodes to convert</param>
        /// <returns>string rep of nodelist</returns>
        public static string NodeListToXaml(this NodeList nodes)
        {
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            StringWriter strWriter = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(strWriter, settings))
            {
                using (XamlXmlWriter writer = new XamlXmlWriter(strWriter, schemaContext))
                {
                    using (XamlNodeReader reader = new XamlNodeReader(nodes, schemaContext))
                    {
                        XamlServices.Transform(reader, writer);
                        return strWriter.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Create an object given a NodeList
        /// Uses XamlServices.Transform(XamlNodeReader [test code], XamlXmlWriter)
        /// </summary>
        /// <param name="nodes">nodes to convert</param>
        /// <returns>object rep of nodelist</returns>
        public static object NodeListToObject(this NodeList nodes)
        {
            XamlSchemaContext schemaContext = new XamlSchemaContext();

            using (XamlObjectWriter writer = new XamlObjectWriter(schemaContext))
            {
                using (XamlNodeReader reader = new XamlNodeReader(nodes, schemaContext))
                {
                    XamlServices.Transform(reader, writer);
                    return writer.Result;
                }
            }            
        }

        /// <summary>
        /// Create Xml string given a NodeList using the
        /// NodeListXmlWriter (test code)
        /// </summary>
        /// <param name="nodes">nodes to convert to xml</param>
        /// <returns>xml rep as string</returns>
        public static string NodeListToXml(this NodeList nodes)
        {
            NodeListXmlWriter nodeListWriter = new NodeListXmlWriter();
            return nodeListWriter.WriteNodes(nodes);
        }
    }
}
