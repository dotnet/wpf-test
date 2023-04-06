// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace PomParser.Baml.Common
{
    internal static class BamlFlowDirectionResourceManager
    {
        internal static IList<BamlFlowDirectionResource> Load(Stream input)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(input);
            XmlNode root = doc.DocumentElement;
            if (root == null || root.Name != FlowDirectionResourcesTag)
            {
                throw new ArgumentException("Invalid flow direction resource file");                        
            }

            List<BamlFlowDirectionResource> list = new List<BamlFlowDirectionResource>(root.ChildNodes.Count);
            foreach (XmlNode child in root.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element && child.Name == ResourceTag)
                {
                    XmlElement element = child as XmlElement;
                    
                    // In <Resource> tag now, continue to read attributes.
                    string name = element.GetAttribute(NameAttribute);
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new ArgumentException("Invalid flow direction resource name");
                    }

                    string type = element.GetAttribute(TypeAttribute);
                    if (string.IsNullOrEmpty(type))
                    {
                        throw new ArgumentException("Invalid flow direction resource type" );
                    }

                    string flowDirectionString = element.GetAttribute(FlowDirectionAttribute);
                    if (string.IsNullOrEmpty(flowDirectionString))
                    {
                        throw new ArgumentException("Invalid flow direction");
                    }

                    BamlFlowDirection flowDirection = (BamlFlowDirection)Enum.Parse(typeof(BamlFlowDirection), flowDirectionString);

                    string isRootString = element.GetAttribute(IsRootAttribute);                    

                    list.Add(
                        new BamlFlowDirectionResource(
                            name,
                            type,
                            flowDirection,
                            !string.IsNullOrEmpty(isRootString)
                        )
                    );                        
                }
            }
            
            return list;
        }

        internal static void Save(IList<BamlFlowDirectionResource> resources, Stream output)
        {
            using (XmlWriter writer = new XmlTextWriter(output, new UTF8Encoding(true)))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(FlowDirectionResourcesTag);

                foreach (BamlFlowDirectionResource resource in resources)
                {
                    writer.WriteStartElement(ResourceTag);
                    writer.WriteAttributeString(NameAttribute, resource.ResourceId);
                    writer.WriteAttributeString(TypeAttribute, resource.ResourceType);
                    writer.WriteAttributeString(
                        FlowDirectionAttribute, 
                        resource.FlowDirection.ToString()
                        );

                    if (resource.IsRoot)
                    {
                        writer.WriteAttributeString(IsRootAttribute, "1");
                    }
                        
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private const string FlowDirectionResourcesTag = "FlowDirectionResources";
        private const string ResourceTag               = "Resource";
        private const string NameAttribute             = "Name";
        private const string TypeAttribute             = "Type";
        private const string FlowDirectionAttribute    = "FlowDirection";
        private const string IsRootAttribute           = "IsRoot";
        
    }
}
