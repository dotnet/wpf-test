// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
* File: BamlDump.cs
*
*  Dump baml file to Console.
*
*
\***************************************************************************/

using System;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Markup;
using MS.Internal;
using System.Windows.Media;
using System.Xml;
using System.Globalization;

namespace DRT
{

    // Baml dumper
    [System.Runtime.InteropServices.ComVisible(false)]
    public class BamlDump
    {
        static BamlReaderWrapper Reader;
        static bool              Verbose = false;
        static bool              ConvertOnly = false;
        static bool              Roundtrip = false;
        static Assembly          assemblyPBT;

        [STAThread]
        public static int Main(string[] args)
        {
            if (args.Length > 2 ||
                (args.Length == 1 &&
                 (args[0] == "/?" || args[0] == "?")))
            {
                Console.WriteLine("Usage:  BamlDump filename [v|c|r]");
                Console.WriteLine("  Dump of Baml file to console using BamlReader.");
                Console.WriteLine("  If Xaml file is given, it will first be converted to BAML");
                Console.WriteLine("  'v' dumps more verbose output");
                Console.WriteLine("  'c' converts xaml to baml only");
                Console.WriteLine("  'r' reads baml and writes out a new baml file using BamlWriter");
                Console.WriteLine("      The new file has '_' appended to base name");
                return 0;
            }

            assemblyPBT = WrapperUtil.FindAssemblyInDomain("PresentationBuildTasks");

            if (args.Length == 0)
            {
                 args = new string[1];
                 args[0] = "test.xaml";
            }
            else
            {
                Verbose = (args.Length == 2 && args[1] == "v");
                ConvertOnly = (args.Length == 2 && args[1] == "c");
                Roundtrip = (args.Length == 2 && args[1] == "r");
            }

            string filename = args[0];
            if (filename.Length < 6)
            {
                Console.WriteLine("Illegal filename : " + filename);
                return 1;
            }


            try
            {
               if (filename.ToUpper(new CultureInfo("en-us", false)).LastIndexOf(".XAML") ==
                   filename.Length-5)
               {
                   if (!ConvertOnly && !Roundtrip && Verbose)
                   {
                       ParseXml(filename);
                   }
                   WrapperUtil.ConvertToBaml(filename);
                   filename = filename.Substring(0,filename.Length-5) + ".baml";
               }
               if (Roundtrip)
               {
                   DumpToNewBaml(filename);
               }
               else if (!ConvertOnly)
               {
                   DumpText(filename);
               }
            }
            catch (Exception e)
            {
                Console.WriteLine("BamlDump failed!!!\nOwner is Microsoft\n" + e);
                
                // PreSharp Complaint 6500 - do not handle null-ref or SEH exceptions.
                //
                // This should be CriticalExceptions.IsCriticalException, but that requires
                // linking to WindowsBase, which is a heavy requirement for "shared" code.
                //
                if(e is NullReferenceException || e is System.Runtime.InteropServices.SEHException)
                {
                    throw;
                }
                else
                {
                    return 1;
                }
            }

            return 0;
        }

        // Read in one baml file using BamlReader and write to another baml file using BamlWriter
        private static void DumpToNewBaml(string filename)
        {
            // Load the baml file using the BamlReader and display its contents to the console.
            Stream fs = LoadTestFile(filename);
            Reader = new BamlReaderWrapper(fs);
            string outFile = filename.Substring(0,filename.Length-5) + "_.baml";
            System.IO.Stream stream = File.OpenWrite(outFile);
            BamlWriterWrapper writer = new BamlWriterWrapper(stream);

            while (Reader.Read())
            {
                switch (Reader.NodeType)
                {
                    case "StartDocument":
                        writer.WriteStartDocument();
                        break;

                    case "EndDocument":
                        writer.WriteEndDocument();
                        break;

                    case "StartElement":
                        writer.WriteStartElement(Reader.AssemblyName, Reader.Name, Reader.IsInjected, Reader.CreateUsingTypeConverter);
                        break;

                    case "StartComplexProperty":
                        writer.WriteStartComplexProperty(
                                             Reader.AssemblyName,
                                             Reader.Name.Substring(0, Reader.Name.LastIndexOf('.')),
                                             Reader.LocalName);
                        break;

                    case "EndElement":
                        writer.WriteEndElement();
                        break;

                    case "EndComplexProperty":
                        writer.WriteEndComplexProperty();
                        break;

                    case "StartConstructor":
                        writer.WriteStartConstructor();
                        break;

                    case "EndConstructor":
                        writer.WriteEndConstructor();
                        break;

                    case "LiteralContent":
                        writer.WriteLiteralContent(Reader.Value);
                        break;

                    case "IncludeReference":
                        Console.Write(" \"" + Reader.Value + "\"");
                        break;

                    case "Text":
                        writer.WriteText(Reader.Value, Reader.TypeConverterAssemblyName, Reader.TypeConverterName);
                        break;

                    case "Event":
                        writer.WriteEvent(Reader.Name, Reader.Value);
                        break;

                    case "RoutedEvent":
                        writer.WriteRoutedEvent(
                                         Reader.AssemblyName,
                                         Reader.Name.Substring(0, Reader.Name.LastIndexOf('.')),
                                         Reader.LocalName,
                                         Reader.Value);
                        break;

                    case "PIMapping":
                        writer.WritePIMapping(Reader.XmlNamespace,
                                              Reader.ClrNamespace,
                                              Reader.AssemblyName);
                        break;
                }

                if (Reader.HasProperties)
                {
                    Reader.MoveToFirstProperty();
                    do
                    {
                        if (Reader.NodeType == "ConnectionId")
                        {
                            writer.WriteConnectionId(Reader.ConnectionId);
                        }
                        else if (Reader.NodeType == "Property")
                        {
                            writer.WriteProperty(Reader.AssemblyName,
                                                 Reader.Name.Substring(0, Reader.Name.LastIndexOf('.')),
                                                 Reader.LocalName,
                                                 Reader.Value,
                                                 Reader.AttributeUsage);
                        }
                        else if (Reader.NodeType == "DefAttribute")
                        {
                            writer.WriteDefAttribute(Reader.Name,
                                                     Reader.Value);
                        }
                        else if (Reader.NodeType == "PresentationOptionsAttribute")
                        {
                            writer.WritePresentationOptionsAttribute(Reader.Name,
                                                     Reader.Value);
                        }                        
                        else if (Reader.NodeType == "ContentProperty")
                        {
                            writer.WriteContentProperty(Reader.AssemblyName,
                                                 Reader.Name.Substring(0, Reader.Name.LastIndexOf('.')),
                                                 Reader.LocalName);
                        }
                        else
                        {
                            writer.WriteXmlnsProperty(Reader.LocalName,
                                                      Reader.Value);
                        }
                    }
                    while (Reader.MoveToNextProperty());
                }
            }

            Reader.Close();
            writer.Close();
        }

        // Dump text representation of baml file to console
        private static void DumpText(string filename)
        {
            // Load the baml file using the BamlReader and display its contents to the console.
            Stream fs = LoadTestFile(filename);
            Reader = new BamlReaderWrapper(fs);
            int depth = 0;
            while (Reader.Read())
            {
                // Set depth and indent accordingly
                switch (Reader.NodeType)
                {
                    case "StartElement":
                    case "StartComplexProperty":
                    case "StartConstructor":
                        depth++;
                        break;
                }
                Indent(depth);
                Console.Write(Reader.NodeType.ToString());

                // Write additional information
                switch (Reader.NodeType)
                {
                    case "StartElement":
                    case "StartComplexProperty":
                    case "StartConstructor":
                        Console.Write("  ");
                        WritePrefixedName();
                        break;

                    case "EndElement":
                    case "EndComplexProperty":
                    case "EndConstructor":
                        depth--;
                        Console.Write("  ");
                        WritePrefixedName();
                        break;

                    case "LiteralContent":
                    case "Text":
                        Console.Write(" \"" + Reader.Value + "\"");
                        break;

                    case "PIMapping":
                        Console.Write(" XmlNamespace=" + Reader.XmlNamespace +
                                      " ClrNamespace=" + Reader.ClrNamespace +
                                      " Assembly=" + Reader.AssemblyName);
                        break;

                }

                Console.WriteLine("");

                if (Reader.HasProperties)
                {
                    Reader.MoveToFirstProperty();
                    do
                    {
                        Indent(depth+2);
                        Console.Write(Reader.NodeType.ToString());
                        Console.Write("  ");
                        WritePrefixedName();
                        if (Verbose)
                            Console.WriteLine("  Value=" + Reader.Value);
                        else
                            Console.WriteLine(" = " + Reader.Value);
                    }
                    while (Reader.MoveToNextProperty());
                }
            }

            Reader.Close();

        }

        // Indent a number of spaces
        private static void Indent(int indent)
        {
            for (int i=0; i<indent; i++)
                Console.Write(" ");
        }

        // Write current reader name plus prefix, if there is one.
        private static void WritePrefixedName()
        {
            if (Verbose)
            {
                Console.Write("XmlNamespace=" + Reader.XmlNamespace);
                Console.Write("  ClrNamespace=" + Reader.ClrNamespace);
                Console.Write("  Assembly=" + Reader.AssemblyName);
                Console.Write("  Prefix=" + Reader.Prefix);
                Console.Write("  Name=" + Reader.Name);
                Console.Write("  LocalName=" + Reader.LocalName);
                //Console.Write("  ReadState=" + Reader.ReadState);
            }
            else
            {
                Console.Write(Reader.Name);
            }
        }

        // Loads up a test file.
        // File is relative to the TestFileDir
        private static Stream LoadTestFile(string fileName)
        {
            string testFile = fileName;
            System.IO.Stream stream = File.OpenRead(testFile);

            return stream;
        }

        // Read the passed xmlfile using XmlTextReader and display contents
        private static void ParseXml(string filename)
        {
            XmlTextReader xmlrdr = new XmlTextReader(filename);
            while (xmlrdr.Read())
            {
                Indent(xmlrdr.Depth);
                Console.Write(xmlrdr.NodeType.ToString());
                Console.Write(" " + xmlrdr.NamespaceURI + " | " + xmlrdr.Prefix + " > " + xmlrdr.Name + " - " + xmlrdr.LocalName + " = " + xmlrdr.Value);
                Console.WriteLine("");

                if (xmlrdr.HasAttributes)
                {
                    xmlrdr.MoveToFirstAttribute();
                    do
                    {
                        Indent(xmlrdr.Depth+1);
                        Console.Write(" " + xmlrdr.NamespaceURI + " | " + xmlrdr.Prefix + " > " + xmlrdr.Name + " - " + xmlrdr.LocalName + " = " + xmlrdr.Value);
                        Console.WriteLine("");
                    }
                    while (xmlrdr.MoveToNextAttribute());
                }
            }

            xmlrdr.Close();
        }

    }

}










