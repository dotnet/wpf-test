// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
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
        static BamlReaderWrapper s_reader;
        static bool              s_verbose = false;
        static bool              s_convertOnly = false;
        static bool              s_roundtrip = false;
        static Assembly          s_assemblyPBT;

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

            s_assemblyPBT = WrapperUtil.FindAssemblyInDomain("PresentationBuildTasks");

            if (args.Length == 0)
            {
                 args = new string[1];
                 args[0] = "test.xaml";
            }
            else
            {
                s_verbose = (args.Length == 2 && args[1] == "v");
                s_convertOnly = (args.Length == 2 && args[1] == "c");
                s_roundtrip = (args.Length == 2 && args[1] == "r");
            }

            string filename = args[0];
            if (filename.Length < 6)
            {
                Console.WriteLine("Illegal filename : " + filename);
                return 1;
            }


            try
            {
               if (filename.ToUpper(CultureInfo.GetCultureInfo("en-us")).LastIndexOf(".XAML") ==
                   filename.Length-5)
               {
                   if (!s_convertOnly && !s_roundtrip && s_verbose)
                   {
                       ParseXml(filename);
                   }
                   WrapperUtil.ConvertToBaml(filename);
                   filename = filename.Substring(0,filename.Length-5) + ".baml";
               }
               if (s_roundtrip)
               {
                   DumpToNewBaml(filename);
               }
               else if (!s_convertOnly)
               {
                   DumpText(filename);
               }
            }
            catch (Exception e)
            {
                Console.WriteLine("BamlDump failed!!!\nOwner is BChapman\n" + e);
                
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
            s_reader = new BamlReaderWrapper(fs);
            string outFile = filename.Substring(0,filename.Length-5) + "_.baml";
            System.IO.Stream stream = File.OpenWrite(outFile);
            BamlWriterWrapper writer = new BamlWriterWrapper(stream);

            while (s_reader.Read())
            {
                switch (s_reader.NodeType)
                {
                    case "StartDocument":
                        writer.WriteStartDocument();
                        break;

                    case "EndDocument":
                        writer.WriteEndDocument();
                        break;

                    case "StartElement":
                        writer.WriteStartElement(s_reader.AssemblyName, s_reader.Name, s_reader.IsInjected, s_reader.CreateUsingTypeConverter);
                        break;

                    case "StartComplexProperty":
                        writer.WriteStartComplexProperty(
                                             s_reader.AssemblyName,
                                             s_reader.Name.Substring(0, s_reader.Name.LastIndexOf('.')),
                                             s_reader.LocalName);
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
                        writer.WriteLiteralContent(s_reader.Value);
                        break;

                    case "IncludeReference":
                        Console.Write(" \"" + s_reader.Value + "\"");
                        break;

                    case "Text":
                        writer.WriteText(s_reader.Value, s_reader.TypeConverterAssemblyName, s_reader.TypeConverterName);
                        break;

                    case "Event":
                        writer.WriteEvent(s_reader.Name, s_reader.Value);
                        break;

                    case "RoutedEvent":
                        writer.WriteRoutedEvent(
                                         s_reader.AssemblyName,
                                         s_reader.Name.Substring(0, s_reader.Name.LastIndexOf('.')),
                                         s_reader.LocalName,
                                         s_reader.Value);
                        break;

                    case "PIMapping":
                        writer.WritePIMapping(s_reader.XmlNamespace,
                                              s_reader.ClrNamespace,
                                              s_reader.AssemblyName);
                        break;
                }

                if (s_reader.HasProperties)
                {
                    s_reader.MoveToFirstProperty();
                    do
                    {
                        if (s_reader.NodeType == "ConnectionId")
                        {
                            writer.WriteConnectionId(s_reader.ConnectionId);
                        }
                        else if (s_reader.NodeType == "Property")
                        {
                            writer.WriteProperty(s_reader.AssemblyName,
                                                 s_reader.Name.Substring(0, s_reader.Name.LastIndexOf('.')),
                                                 s_reader.LocalName,
                                                 s_reader.Value,
                                                 s_reader.AttributeUsage);
                        }
                        else if (s_reader.NodeType == "DefAttribute")
                        {
                            writer.WriteDefAttribute(s_reader.Name,
                                                     s_reader.Value);
                        }
                        else if (s_reader.NodeType == "PresentationOptionsAttribute")
                        {
                            writer.WritePresentationOptionsAttribute(s_reader.Name,
                                                     s_reader.Value);
                        }                        
                        else if (s_reader.NodeType == "ContentProperty")
                        {
                            writer.WriteContentProperty(s_reader.AssemblyName,
                                                 s_reader.Name.Substring(0, s_reader.Name.LastIndexOf('.')),
                                                 s_reader.LocalName);
                        }
                        else
                        {
                            writer.WriteXmlnsProperty(s_reader.LocalName,
                                                      s_reader.Value);
                        }
                    }
                    while (s_reader.MoveToNextProperty());
                }
            }

            s_reader.Close();
            writer.Close();
        }

        // Dump text representation of baml file to console
        private static void DumpText(string filename)
        {
            // Load the baml file using the BamlReader and display its contents to the console.
            Stream fs = LoadTestFile(filename);
            s_reader = new BamlReaderWrapper(fs);
            int depth = 0;
            while (s_reader.Read())
            {
                // Set depth and indent accordingly
                switch (s_reader.NodeType)
                {
                    case "StartElement":
                    case "StartComplexProperty":
                    case "StartConstructor":
                        depth++;
                        break;
                }
                Indent(depth);
                Console.Write(s_reader.NodeType.ToString());

                // Write additional information
                switch (s_reader.NodeType)
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
                        Console.Write(" \"" + s_reader.Value + "\"");
                        break;

                    case "PIMapping":
                        Console.Write(" XmlNamespace=" + s_reader.XmlNamespace +
                                      " ClrNamespace=" + s_reader.ClrNamespace +
                                      " Assembly=" + s_reader.AssemblyName);
                        break;

                }

                Console.WriteLine("");

                if (s_reader.HasProperties)
                {
                    s_reader.MoveToFirstProperty();
                    do
                    {
                        Indent(depth+2);
                        Console.Write(s_reader.NodeType.ToString());
                        Console.Write("  ");
                        WritePrefixedName();
                        if (s_verbose)
                            Console.WriteLine("  Value=" + s_reader.Value);
                        else
                            Console.WriteLine(" = " + s_reader.Value);
                    }
                    while (s_reader.MoveToNextProperty());
                }
            }

            s_reader.Close();

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
            if (s_verbose)
            {
                Console.Write("XmlNamespace=" + s_reader.XmlNamespace);
                Console.Write("  ClrNamespace=" + s_reader.ClrNamespace);
                Console.Write("  Assembly=" + s_reader.AssemblyName);
                Console.Write("  Prefix=" + s_reader.Prefix);
                Console.Write("  Name=" + s_reader.Name);
                Console.Write("  LocalName=" + s_reader.LocalName);
                //Console.Write("  ReadState=" + Reader.ReadState);
            }
            else
            {
                Console.Write(s_reader.Name);
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










