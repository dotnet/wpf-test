// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Base class for all xaml fuzzing operations
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class XamlFuzzer : FuzzerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public XamlFuzzer(Random random)
            : base(random)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public XamlFuzzer(XmlElement xmlElement, Random random)
            : base(random)
        {
        }

        /// <summary>
        /// </summary>
        public virtual void FuzzXaml(string sourceFilePath, string destinationFilePath)
        {
            switch (random.Next(6))
            {
                case 0:
                    this.ShuffleTags(sourceFilePath);
                    break;
                case 1:
                    this.FuzzRandom(sourceFilePath);
                    break;
                case 2:
                    this.InjectUnknownTags(sourceFilePath, null);
                    break;
                case 3:
                    this.InjectUnknownTags(sourceFilePath, "ControlTemplate");
                    break;
                case 4:
                    this.InjectUnknownTags(sourceFilePath, "Style");
                    break;
                case 5:
                    this.FuzzAttributeValues(sourceFilePath);
                    break;
            }

            File.Copy(_fuzzedXamlPath, destinationFilePath, true);
        }

        private void FuzzRandom(string fileName)
        {
            Stream xamlStream = File.OpenRead(fileName);
            int inc = 10;
            int read = 0;
            Stream outStream = new FileStream(_fuzzedXamlPath, FileMode.Create, FileAccess.Write);
            byte[] buffer = new byte[inc];
            while (0 < (read = xamlStream.Read(buffer, 0, inc)))
            {
                if (random.Next(200) < 1)
                {
                    random.NextBytes(buffer);
                }
                outStream.Write(buffer, 0, read);
            }

            xamlStream.Close();
            outStream.Close();
        }
        private void InjectUnknownTags(string fileName, string targetParent)
        {
            string selectionString = ".//*";
            if (targetParent != null)
                selectionString = ".//" + targetParent;

            int injectLimit = random.Next(3);
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            List<XmlNode> allNodes = CopyXmlNodeListToList(doc.SelectNodes(selectionString));

            int index = 0;
            XmlElement node = null;

            for (int i = 0; i < injectLimit && allNodes.Count > 0; i++)
            {
                index = random.Next(allNodes.Count);
                node = (XmlElement)allNodes[index];

                // Sometimes skip.
                if (random.Next(2) == 0)
                    continue;

                string tagName = "";
                int injectType = random.Next(3);
                switch (injectType)
                {
                    case 0:
                        tagName = node.Name + ".Foo";
                        break;
                    case 1:
                        tagName = "Foo";
                        break;
                    case 2:
                        tagName = "Foo.Foo";
                        break;
                }

                XmlElement element = doc.CreateElement(tagName);
                node.PrependChild(element);
            }

            _SaveXmlDocumentToFile(doc);
        }
        private void ShuffleTags(string fileName)
        {
            int shuffleCount = random.Next(50);

            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            List<XmlNode> allNodes = CopyXmlNodeListToList(doc.SelectNodes(".//*"));

            // Shuffle multiple times if nodes were found.
            int nodeCount = allNodes.Count;
            for (int i = 0; i < shuffleCount && allNodes.Count > 1; i++)
            {
                int index1 = random.Next(allNodes.Count);
                XmlElement node1 = (XmlElement)allNodes[index1];
                allNodes.RemoveAt(index1);
                int index2 = random.Next(allNodes.Count);
                XmlElement node2 = (XmlElement)allNodes[index2];
                allNodes.RemoveAt(index2);

                XmlNode parent1 = node1.ParentNode;
                parent1.RemoveChild(node1);

                XmlNode parent2 = node2.ParentNode;
                parent2.RemoveChild(node2);

                // Clone the nodes so we avoid circular references if
                // either node is in the same ancestory chain.
                node1 = (XmlElement)node1.Clone();
                node2 = (XmlElement)node2.Clone();

                parent1.PrependChild(node2);
                parent2.PrependChild(node1);
            }

            _SaveXmlDocumentToFile(doc);
        }
        private void FuzzAttributeValues(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            List<XmlNode> allNodes = CopyXmlNodeListToList(doc.SelectNodes(".//@*"));

            if (allNodes.Count > 0)
            {
                int index = random.Next(allNodes.Count);
                XmlAttribute attrib = (XmlAttribute)allNodes[index];

                attrib.Value = this.GetFuzz();
            }

            _SaveXmlDocumentToFile(doc);
        }
        private static List<XmlNode> CopyXmlNodeListToList(XmlNodeList nodeList)
        {
            List<XmlNode> list = new List<XmlNode>();

            foreach (XmlNode node in nodeList)
            {
                list.Add(node);
            }

            return list;
        }
        private void _SaveXmlDocumentToFile(XmlDocument xmlDocument)
        {
            Stream outStream = new FileStream(_fuzzedXamlPath, FileMode.Create, FileAccess.Write);
            xmlDocument.Save(outStream);
            outStream.Close();
        }

        private readonly string _fuzzedXamlPath = "_xamlFuzzerTemp.xaml";
    }
}

