// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup.Localizer;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows.Automation;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Resources;

using Microsoft.Test;
using Microsoft.Test.Serialization;
#endregion
namespace Microsoft.Test.Globalization
{
    public class CommonLib
    {

        #region private
        // input markup tags
        private const string ElementTag = "Element";
        private const string PropertyTag = "Property";
        private const string DefAttributeTag = "DefAttribute";
        private const string ComplexPropertyTag = "ComplexProperty";
        private const string XmlnsPropertyTag = "XmlnsProperty";
        private const string AssemblyAttribute = "Assembly";
        private const string NameAttribute = "Name";
        private const string OwnerAttribute = "Owner";
        private const string ValueAttribute = "Value";
        #endregion
        #region LoadTestFile
        /// <summary>
        /// This function creates a stream for a supplied file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>

        public static Stream LoadTestFile(string filename)
        {
            string testfile = filename;
            Stream stream = File.OpenRead(testfile);
            return stream;
        }
        #endregion
        #region ErrorLogShow
        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        public static void ErrorLogShow(string log)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + log);
            System.Console.ResetColor();
        }
        #endregion
        #region FindElementByID (from DRTBase)
        /// <summary>
        /// Search the visual and logical trees looking for a node with
        /// a given Name.
        /// </summary>
        /// <param name="id">id of desired node</param>
        /// <param name="node">starting node for the search</param>
        public static DependencyObject FindElementByID(string id, DependencyObject node)
        {
            return FindElementByPropertyValue(FrameworkElement.NameProperty, id, node, true);
        }
        /// <summary>
        /// Search the visual and logical trees looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        private static DependencyObject FindElementByPropertyValue(DependencyProperty dp, object value, DependencyObject node, bool includeNode)
        {
            if (node == null)
                return null;

            // see if the node itself has the right value
            if (includeNode)
            {
                object nodeValue = node.GetValue(dp);
                if (Object.Equals(value, nodeValue))
                    return node;
            }

            DependencyObject result;
            DependencyObject child;

            // if not, recursively look at the logical children
            foreach (object currentChild in LogicalTreeHelper.GetChildren(node))
            {
                child = currentChild as DependencyObject;
                result = FindElementByPropertyValue(dp, value, child, true);
                if (result != null)
                    return result;
            }

            // then the visual children
            Visual vNode = node as Visual;
            if (vNode != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(vNode);
                for(int i = 0; i < count; i++)
                {
                    child = VisualTreeHelper.GetChild(vNode, i) as DependencyObject;
                    result = FindElementByPropertyValue(dp, value, child, true);
                    if (result != null)
                        return result;
                }
            }
            // not found
            return null;
        }
        #endregion
        #region ConstructBamlStream (DRTLocalization)
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Stream ConstructBamlStream(string fileName)
        {
            Console.WriteLine("Constructing test baml stream using BamlWriter API");
            MemoryStream bamlStream = null;
            try
            {
                bamlStream = new MemoryStream();
                BamlWriterWrapper writer = new BamlWriterWrapper(bamlStream);
                XmlDocument document = new XmlDocument();
                document.Load(fileName);
                XmlNode root = document.FirstChild;
                while (root != null && root.NodeType != XmlNodeType.Element)
                {
                    root = root.NextSibling; // find the first element node.
                }
                if (root != null)
                {
                    Console.WriteLine("Not a valid File.");
                    return null;
                }
                // write start document
                writer.WriteStartDocument();
                // writer out the body of the baml
                ConvertXmlElementToBamlRecord((XmlElement)root, writer);
                // write end document
                writer.WriteEndDocument();
                bamlStream.Seek(0, SeekOrigin.Begin);
                return bamlStream;
            }
            catch (Exception e)
            {
                // clean up
                if (bamlStream != null)
                {
                    bamlStream.Close();
                    bamlStream = null;
                }

                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="writer"></param>
        public void ConvertXmlElementToBamlRecord(XmlElement node, BamlWriterWrapper writer)
        {
            string assembly = null;
            string name = null;
            string owner = null;
            string propertyValue = null;

            switch (node.Name)
            {
                case ElementTag:
                    {
                        assembly = node.GetAttribute(AssemblyAttribute);
                        name = node.GetAttribute(NameAttribute);
                        writer.WriteStartElement(assembly, name, false, false);

                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            XmlNode child = node.ChildNodes[i];
                            if (child.NodeType == XmlNodeType.Element)
                            {
                                ConvertXmlElementToBamlRecord((XmlElement)child, writer);
                            }
                            else if (child.NodeType == XmlNodeType.Text)
                            {
                                writer.WriteText((child.Value.TrimStart()).TrimEnd(),"","");
                            }
                        }

                        writer.WriteEndElement();
                        break;
                    }
                case ComplexPropertyTag:
                    {
                        assembly = node.GetAttribute(AssemblyAttribute);
                        name = node.GetAttribute(NameAttribute);
                        owner = node.GetAttribute(OwnerAttribute);

                        writer.WriteStartComplexProperty(
                            assembly,
                            owner,
                            name
                            );

                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            XmlNode child = node.ChildNodes[i];
                            if (child.NodeType == XmlNodeType.Element)
                            {
                                ConvertXmlElementToBamlRecord((XmlElement)child, writer);
                            }
                        }

                        writer.WriteEndComplexProperty();
                        break;
                    }
                case PropertyTag:
                    {
                        assembly = node.GetAttribute(AssemblyAttribute);
                        name = node.GetAttribute(NameAttribute);
                        owner = node.GetAttribute(OwnerAttribute);
                        propertyValue = node.GetAttribute(ValueAttribute);

                        writer.WriteProperty(
                            assembly,
                            owner,
                            name,
                            propertyValue,
                            "Default"
                            );
                        break;
                    }
                case DefAttributeTag:
                    {
                        name = node.GetAttribute(NameAttribute);
                        propertyValue = node.GetAttribute(ValueAttribute);
                        writer.WriteDefAttribute(name, propertyValue);
                        break;
                    }
                case XmlnsPropertyTag:
                    {
                        name = node.GetAttribute(NameAttribute);
                        propertyValue = node.GetAttribute(ValueAttribute);
                        writer.WriteXmlnsProperty(name, propertyValue);
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion
        #region DumpTestBamlStream (DRTLocalization)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        public static void DumpTestBamlStream(Stream stream, string filename)
        {
            // also dump the baml stream we are testing against
            using (Stream bamlDumpStream = File.Open(filename, FileMode.Create))
            {
                stream.Seek(0, SeekOrigin.Begin);
                byte[] bamlContent = new byte[stream.Length];
#pragma warning disable CA2022 // Avoid inexact read
                stream.Read(bamlContent, 0, bamlContent.Length);
#pragma warning restore CA2022

                using (BinaryWriter binaryWriter = new BinaryWriter(bamlDumpStream))
                {
                    binaryWriter.Write(bamlContent);
                }
            }
        }
        #endregion
        #region CompareBamlStreams (DRTLocalization)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public int CompareBamlStreams(Stream first, Stream second)
        {
            first.Seek(0, SeekOrigin.Begin);
            second.Seek(0, SeekOrigin.Begin);
            if (first.Length != second.Length)
            {
                return (int)(first.Length - second.Length);
            }
            byte[] firstContent = new byte[first.Length];
#pragma warning disable CA2022 // Avoid inexact read
            first.Read(firstContent, 0, firstContent.Length);
#pragma warning restore CA2022
            byte[] secondContent = new byte[second.Length];
#pragma warning disable CA2022 // Avoid inexact read
            second.Read(secondContent, 0, secondContent.Length);
#pragma warning restore CA2022

            for (int i = 0; i < firstContent.Length; i++)
            {
                if (firstContent[i] != secondContent[i])
                {
                    return firstContent[i] - secondContent[i];
                }
            }
            return 0;
        }
        #endregion
        #region ParseBamlStreamList
        /// <summary>
        /// Extracts BAML files from "filename" (dll) and places them in "extractdir"
        /// TODO:  support for .resources ; return array of baml file names
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="extractdir"></param>
        public static ArrayList ParseBamlStreamList(string filename, string extractdir)
        {

            // for a dll, it is the same idea
            Console.WriteLine("Loading assembly from" + filename);
            Assembly assembly = Assembly.LoadFrom(filename);
            s_bamlfilelist = new ArrayList();
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                ResourceLocation resourceLocation = assembly.GetManifestResourceInfo(resourceName).ResourceLocation;

                // if this resource is in another assemlby, we will skip it
                if ((resourceLocation & ResourceLocation.ContainedInAnotherAssembly) != 0)
                {
                    continue;   // in resource assembly, we don't have resource that is contained in another assembly
                }

                Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
                using (ResourceReader reader = new ResourceReader(resourceStream))
                {
                    EnumerateBamlInResources(reader, resourceName, extractdir);
                }
            }
            return s_bamlfilelist;

        }

	public static UIElement LoadXamlFromSiteOfOrigin(string filename)
	{
            Uri resourceUri = new Uri("pack://siteoforigin:,,,/" + filename, UriKind.RelativeOrAbsolute);	
	    Stream s = Application.GetRemoteStream(resourceUri).Stream;
	    UIElement root = XamlReader.Load(s) as UIElement;
	    return root;
	}

        static ArrayList s_bamlfilelist;
        //--------------------------------
        // private function
        //--------------------------------
        private static void EnumerateBamlInResources(ResourceReader reader, string resourceName, string dirpath)
        {
            
            foreach (DictionaryEntry entry in reader)
            {
                string name = entry.Key as string;

                if (IsResourceEntryBamlStream(name, entry.Value))
                {
                    s_bamlfilelist.Add(name);
                    if (!Directory.Exists(dirpath))
                    {
                        Directory.CreateDirectory(dirpath);
                       
                    }
                    DumpTestBamlStream((Stream) entry.Value, dirpath + "/" + name);


                }
            }
        }
 
    private static bool IsResourceEntryBamlStream(string name, object value)
        {

            string extension = Path.GetExtension(name);
            //need something here!!!!
            if (string.Compare(extension, ".baml", true, CultureInfo.InvariantCulture) == 0)
            {
                //it has .Baml at the end
                Type type = value.GetType();

                if (typeof(Stream).IsAssignableFrom(type))
                {
                    Console.WriteLine("Found " + name);
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region DumpBamlToString
        /// <summary>
        /// Dump out Baml information to console.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string DumpBamlToString(string filename)
        {
            string baml = null;
            Stream stream = LoadTestFile(filename);
            BamlReaderWrapper Reader = new BamlReaderWrapper(stream);
            while (Reader.Read())
            {
                // Set depth and indent accordingly
                switch (Reader.NodeType)
                {
                    case "StartElement":
                    case "StartComplexProperty":
                    case "StartConstructor":
                        break;
                }
                baml += Reader.NodeType.ToString();
                // Write additional information
                switch (Reader.NodeType)
                {
                    case "StartElement":
                    case "StartComplexProperty":
                    case "StartConstructor":
                        baml += "  ";
                        baml += Reader.Name;
                        baml += "\r\n";
                        break;

                    case "EndElement":
                    case "EndComplexProperty":
                    case "EndConstructor":
                        baml += "  ";
                        baml += Reader.Name;
                        baml += "\r\n";
                        break;

                    case "LiteralContent":
                    case "Text":
                        baml += " \"" + Reader.Value + "\"";
                        baml += "\r\n";
                        break;

                    case "PIMapping":
                        baml += " XmlNamespace=" + Reader.XmlNamespace +
                                      " ClrNamespace=" + Reader.ClrNamespace +
                                      " Assembly=" + Reader.AssemblyName;
                        baml += "\r\n";
                        break;
                }
                baml += "";
                if (Reader.HasProperties)
                {
                    Reader.MoveToFirstProperty();
                    do
                    {
                        baml += Reader.NodeType.ToString();
                        baml += "  ";
                        baml += Reader.Name;
                        baml += " = " + Reader.Value + "\r\n";
                    }
                    while (Reader.MoveToNextProperty());
                }
            }
            Reader.Close();
            stream.Close();
            return baml;
        }
        #endregion
        #region GetAssemblyVersion
        /// <summary>
        /// GetAssemblyVersion
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetAssemblyVersion(string filename)
        {
            Assembly currentAssembly = Assembly.LoadFile(filename);
            return currentAssembly.GetName().Version.ToString(4);
        }
        #endregion
    }
}

