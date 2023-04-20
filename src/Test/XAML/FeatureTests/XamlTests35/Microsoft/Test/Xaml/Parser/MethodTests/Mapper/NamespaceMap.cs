// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Mapper
{

    /// <summary>
    /// XamlTypeMapper tests
    /// </summary>
    /// <remarks>
    /// These testcases parse a Xaml that doesn't have any xmlns declarations or 
    /// (Xmlns -> Clrns + Assembly) mappings.
    /// 
    /// We populate XamlTypeMapper instances with the mapping information, and then set it
    /// on ParserContext.XamlTypeMapper property. We then use that ParserContext to parse the
    /// xaml.
    /// 
    /// Different testcases use different ways of populating the mapping information in the
    /// XamlTypeMapper
    /// </remarks>
    public class NamespaceMapTest
    {
        #region Mapper_AddMappingPITest
        /// <summary>
        /// This testcase populates the mapping information through the use of 
        /// XamlTypeMapper.AddMappingProcessingInstruction()
        /// </summary>
        public void Mapper_AddMappingPITest()
        {
            CustomParserContext pCtx = new CustomParserContext();
            pCtx.XamlTypeMapper = GetMapperWithCustomMappings();
            WPFXamlParser parser = new WPFXamlParser();
            object root = parser.LoadXaml(_testXaml, pCtx);
        }
        #endregion Mapper_AddMappingPITest

        #region Mapper_AddMappingPIAsyncTest
        /// <summary>
        /// This testcase populates the mapping information through the use of 
        /// XamlTypeMapper.AddMappingProcessingInstruction()
        /// </summary>
        public void Mapper_AddMappingPIAsyncTest()
        {
            CustomParserContext pCtx = new CustomParserContext();
            pCtx.XamlTypeMapper = GetMapperWithCustomMappings();
            object root = new System.Windows.Markup.XamlReader().LoadAsync(File.OpenRead(_testXaml), pCtx);
        }
        #endregion Mapper_AddMappingPITest

        #region Mapper_GetTypeTest
        /// <summary>
        /// Testcase for XamlTypeMapper.GetType()
        /// </summary>
         public void Mapper_GetTypeTest()
        {         
            string errorMesg = "XamlTypeMapper.GetType() failed";
            XamlTypeMapper mapper = GetMapperWithCustomMappings();

            Type t1 = mapper.GetType("http://test/System", "Array");
            Assert(typeof(System.Array) == t1, errorMesg + " for System.Array");

            Type t2 = mapper.GetType(_avalonNS, "ListBox");
            Assert(typeof(System.Windows.Controls.ListBox) == t2, errorMesg + " for System.Windows.Controls.ListBox");

            Type t3 = mapper.GetType(_xamlNS, "ParserContext");
            Assert(typeof(System.Windows.Markup.ParserContext) == t3, errorMesg + " for System.Windows.Markup.ParserContext");

            // Try a struct
            Type t4 = mapper.GetType(_avalonNS, "Color");
            Assert(typeof(System.Windows.Media.Color) == t4, errorMesg + " for System.Windows.Media.Color");

            // and an Enum
            Type t5 = mapper.GetType(_xamlNS, "XamlWriterMode");
            Assert(typeof(System.Windows.Markup.XamlWriterMode) == t5, errorMesg + " for System.Windows.Markup.XamlWriterMode");

            // An abstract type
            Type t6 = mapper.GetType("http://test/System", "TimeZone");
            Assert(typeof(System.TimeZone) == t6, errorMesg + " for System.TimeZone");

            // An Xml namespace defined using XmlnsDefitionAttribute on an assembly.
            // We need to load the assembly in order for the attribute to come into effect.
            Assembly.LoadFrom("ParserTestControlsV1.dll");
            Type t7 = mapper.GetType("http://ns.controlstore.com/avalon/2005", "TransButton");
            Assert(t7.Name == "TransButton", errorMesg + " for Com.ControlStore.TransButton"); 
            Assert(t7.Assembly.GetName().Name == "ParserTestControlsV1", errorMesg + " for Com.ControlStore.TransButton");
            Assert(t7.Namespace == "Com.ControlStore", errorMesg + " for Com.ControlStore.TransButton");

            // An internal type. We use XamlTypeMapper.AllowInternalType(), but 
            // that mechanism is only supposed to work in Full Trust, and not in Partial Trust.


            string securityLevel = DriverState.DriverParameters["SecurityLevel"];
	
            bool isFullTrust = false;

            if (String.Compare(securityLevel,"FullTrust", true) == 0)
            {
                isFullTrust = true;
            }


            if (isFullTrust )
            {
                Type t8 = mapper.GetType("http://test/XamlTests", "Custom_InternalButton");
                //

            }
            else // Partial trust
            {
                try
                {
                    Type t8 = mapper.GetType("http://test/XamlTests", "Custom_InternalButton");
                    throw new Microsoft.Test.TestValidationException("XamlParseException expected, but not thrown"
                        + " when trying XamlTypeMapper.GetType() on an internal type in Partial trust");
                }
                catch (XamlParseException){ }
            }            
        }
        #endregion Mapper_GetTypeTest

        #region Mapper_AllowInternalTypeTest
        /// <summary>
        /// Test for XamlTypeMapper.AllowInternalType()
        /// That API is a way to allow parsing of internal types, only in Full Trust.
        /// We try to parse different markups having internal types, and make sure 
        /// that they parse fine in Full Trust, and fail to parse in Partial Trust.
        /// </summary>        
        public void Mapper_AllowInternalTypeTest()
        {
            // Combine the base xaml file with xaml snippets to create the test xaml file.
            for (int i = 0; i < 4; i++) // We have 4 snippets
            {
                // We start from a barebone base file - which just contains the root
                // "Page" tag - and then build on it.
                XmlDocument tempDoc = new XmlDocument();
                tempDoc.PreserveWhitespace = true;
                tempDoc.Load(_baseXamlFile);
                XmlElement tempDocRoot = tempDoc.DocumentElement;

                // Read a snippet from the snippet file. 
                XmlDocument snipDoc = new XmlDocument();
                snipDoc.PreserveWhitespace = true;
                snipDoc.Load(_snippetFile);

                XmlElement snippet = snipDoc.SelectSingleNode("//*[@SnippetID='Snippet" + i + "']") as XmlElement;
                snippet.RemoveAttribute("SnippetID");

                // Append the snippet to the root of the base xaml.
                tempDocRoot.AppendChild((XmlElement)tempDoc.ImportNode(snippet, true));

                // Save the xaml to a temp file
                tempDoc.Save(_tempXamlFile);

                // Create a ParserContext to parse the xaml.
                // CustomMapper derives from XamlTypeMapper, and uses AllowInternalType()
                // to allow one internal type: Avalon.Test.CoreUI.Parser.Custom_InternalButton
                CustomMapper mapper = new CustomMapper();
                ParserContext pc = new ParserContext();
                
                pc.XamlTypeMapper = mapper;
                mapper._internalTypeAllowed = false;

            string securityLevel = DriverState.DriverParameters["SecurityLevel"];
	
            bool isFullTrust = false;

            if (String.Compare(securityLevel,"FullTrust", true) == 0)
            {
                isFullTrust = true;
            }



                // Parse the xaml
                if (isFullTrust )
                {
                    // Parsing should be successful
                    WPFXamlParser parser = new WPFXamlParser();            
                    parser.LoadXaml(_tempXamlFile, pc);

                    // CustomMapper should have been asked whether to allow internal types.
                    if (!mapper._internalTypeAllowed)
                    {
                        throw new Microsoft.Test.TestValidationException("Parser allowed an internal type to parse, "
                            + "without asking permission for it.");
                    }
                }
                else // Partial trust. Parsing should fail.
                {
                    try
                    {
                        WPFXamlParser parser = new WPFXamlParser();            
                        parser.LoadXaml(_tempXamlFile, pc);
                        throw new Microsoft.Test.TestValidationException("XamlParseException expected, but not thrown. "
                            + "Parser allowed an internal type to parse in Partial Trust.");
                    }
                    catch (XamlParseException xpe) 
                    {
                        if (!xpe.Message.Contains("'Custom_InternalButton' type is not public"))
                        {
                            throw new Microsoft.Test.TestValidationException("Parser threw the following unexpected exception "
                                + "while trying to parse an internal type in Partial Trust. "
                                + xpe.Message);
                        }
                    }

                    if (mapper._internalTypeAllowed)
                    {
                        throw new Microsoft.Test.TestValidationException("Parser called CustomMapper.AllowInternalType, "
                            + "which it's not supposed to do in Partial Trust.");
                    }
                }            
            }
        }
        #endregion Mapper_AllowInternalTypeTest

        #region Context_XmlSpaceTest
        /// <summary>
        /// This testcase sets ParserContext.XmlSpace to "preserve", and
        /// then verifies that whitespaces in the markup were preserved.
        /// </summary>
        /// 
        public void Context_XmlSpaceTest()
        {
            CustomParserContext pCtx = new CustomParserContext();
            pCtx.XamlTypeMapper = GetMapperWithCustomMappings();
            pCtx.XmlSpace = "preserve";
            WPFXamlParser parser = new WPFXamlParser();            
            object root = parser.LoadXaml(_testXaml, pCtx);

            // Verify that whitespace was preserved.
            TextBlock TextBlock0 = (TextBlock)LogicalTreeHelper.FindLogicalNode(root as DependencyObject, "TextBlock0");
            if (TextBlock0.Text != "Hello \t \t world")
            {
                throw new Microsoft.Test.TestValidationException("ParserContext.XmlSpace was set to 'preserve',"
                    + " but whitespace wasn't preserved.");
            }                
        }
        #endregion Context_XmlSpaceTest

        #region Context_XmlLangTest
        /// <summary>
        /// This testcase sets ParserContext.XmlLang to a non-default value ("mr-IN"), and
        /// then verifies that it is equivalent to setting xml:lang on the root.
        /// 



        public void Context_XmlLangTest()
        {
            CustomParserContext pCtx = new CustomParserContext();
            pCtx.XamlTypeMapper = GetMapperWithCustomMappings();
            pCtx.XmlLang = "mr-IN";
            WPFXamlParser parser = new WPFXamlParser();
            object root = parser.LoadXaml(_testXaml, pCtx);

            // Verify the XmlLangProperty on root (for DockPanel, it's Language).
            XmlLanguage rootLanguage = (root as DockPanel).Language;
            if (rootLanguage.ToString() != "mr-IN")
            {
                throw new Microsoft.Test.TestValidationException("ParserContext.XmlLang was set to 'mr-IN',"
                    + " but root's XmlLangProperty has value '" + rootLanguage + "'");
            }
        }
        #endregion Context_XmlLangTest

        #region Mapper_ConstructorTest
        /// <summary>
        /// This test uses the XamlTypeMapper constructor with 2 params. The second
        /// param is NamespaceMapEntry[], and that's what the test uses to populate 
        /// the mapping information.
        /// </summary>
        public void Mapper_ConstructorTest()
        {
            CustomParserContext pCtx = new CustomParserContext();    
            NamespaceMapEntry[] nsEntries =                 
            {
                    new NamespaceMapEntry(s_xtMapping[0], s_xtMapping[2], s_xtMapping[1]),
                    new NamespaceMapEntry(s_trMapping[0], s_trMapping[2], s_trMapping[1]),
                    new NamespaceMapEntry(s_sysMapping[0], s_sysMapping[2], s_sysMapping[1]),
                    new NamespaceMapEntry(s_ptcMapping[0], s_ptcMapping[2], s_ptcMapping[1])
            };
            string[] assemblies = {};
            pCtx.XamlTypeMapper = new XamlTypeMapper( assemblies, nsEntries );

            WPFXamlParser parser = new WPFXamlParser();
            object root = parser.LoadXaml(_testXaml, pCtx);          
        }
        #endregion Mapper_ConstructorTest

        #region Mapper_SetAssemblyPathTest
        /// <summary>
        /// Here we test XamlTypeMapper.SetAssemblyPath() API, using the following steps:
        /// 1. We populate the mapping information through AddMappingProcessingInstruction().
        /// 2. We move one of the required assemblies from the current folder to a subfolder.
        /// 3. We parse the Xaml and make sure it throws an error (since the assembly is no
        ///    longer in current folder, parser can't find it).
        /// 4. We specify the assembly path using SetAssemblyPath().
        /// 5. We parse the Xaml again and make sure it works fine this time.
        /// 6. We move the assembly back from the subfolder to the current folder.
        /// </summary>
        /// 
        public void Mapper_SetAssemblyPathTest()
        {
            CustomParserContext pCtx = new CustomParserContext();
            pCtx.XamlTypeMapper = GetMapperWithCustomMappings();
            WPFXamlParser parser = new WPFXamlParser();
            
            // Move the assembly to a temporary subfolder
            string tempFolderPath = "__NamespaceMapTestFolder";
            Directory.CreateDirectory(tempFolderPath);
            string ptcAssembly = s_ptcMapping[2] + ".dll";
            GlobalLog.LogStatus("Moving " + ptcAssembly + " to " + tempFolderPath);
            File.Move(ptcAssembly, Path.Combine(tempFolderPath, ptcAssembly));

            try
            {
                try
                {
                    GlobalLog.LogStatus("Loading the Xaml file.");
                    parser.LoadXaml(_testXaml, pCtx);
                    throw new Microsoft.Test.TestValidationException("XamlParseException expected, but not thrown.");
                }
                catch(XamlParseException e)
                {
                    GlobalLog.LogStatus("Caught following XamlParseException as expected: " + e.Message);
                }

                pCtx = new CustomParserContext();
                pCtx.XamlTypeMapper = GetMapperWithCustomMappings(); 
                string assemblyPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), tempFolderPath), ptcAssembly);
                pCtx.XamlTypeMapper.SetAssemblyPath(s_ptcMapping[2], assemblyPath);

                GlobalLog.LogStatus("Loading the Xaml file again after SetAssemblyPath()");
                object root = parser.LoadXaml(_testXaml, pCtx);
            }
            finally
            {
                GlobalLog.LogStatus("Moving " + ptcAssembly + " from " + tempFolderPath + " back to current folder.");
                File.Move(Path.Combine(tempFolderPath, ptcAssembly), ptcAssembly);
                Directory.Delete(tempFolderPath, true);
            }
        }
        #endregion Mapper_SetAssemblyPathTest

        #region GetMapperWithCustomMappings
        /// <summary>
        /// Return a XamlTypeMapper with all the mapping information populated.
        /// </summary>
        /// <returns></returns>
        private static XamlTypeMapper GetMapperWithCustomMappings()
        {
            XamlTypeMapper mapper = new CustomMapper();
            mapper.AddMappingProcessingInstruction(s_xtMapping[0], s_xtMapping[1], s_xtMapping[2]);
            mapper.AddMappingProcessingInstruction(s_sysMapping[0], s_sysMapping[1], s_sysMapping[2]);
            mapper.AddMappingProcessingInstruction(s_trMapping[0], s_trMapping[1], s_trMapping[2]);
            mapper.AddMappingProcessingInstruction(s_ptcMapping[0], s_ptcMapping[1], s_ptcMapping[2]);
            return mapper;
        }
        #endregion GetMapperWithCustomMappings

        #region CustomParserContext
        /// <summary>
        /// A custom ParserContext that pre-populates the XmlnsDictionary with the 
        /// xmlns declarations we need.
        /// </summary>
        public class CustomParserContext : ParserContext
        {
            /// <summary>
            /// 
            /// </summary>
            public CustomParserContext()
                : base()
            {
                PopulateXmlnsDictionary();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="xpc"></param>
            public CustomParserContext(XmlParserContext xpc)
                : base(xpc)
            {
                PopulateXmlnsDictionary();
            }

            private void PopulateXmlnsDictionary()
            {
                XmlnsDictionary.Add("", _avalonNS);
                XmlnsDictionary.Add("x", _xamlNS);
                XmlnsDictionary.Add("cc", s_xtMapping[0]);
                XmlnsDictionary.Add("sys", s_sysMapping[0]);
                XmlnsDictionary.Add("cmn", s_trMapping[0]);
                XmlnsDictionary.Add("ptc", s_ptcMapping[0]);
            }
        }
        #endregion CustomParserContext

        #region CustomMapper
        /// <summary>
        /// A custom XamlTypeMapper that allows parsing of one particular internal type
        /// using XamlTypeMapper.AllowInternalType, for test purposes.
        /// Otherwise, parsing doesn't allow internal types (needs public types)
        /// </summary>
        public class CustomMapper : XamlTypeMapper
        {
            /// <summary>
            /// C'tor
            /// </summary>
            public CustomMapper()
                : base(new string[] { })
            {
            }

            /// <summary>
            /// 
            /// </summary>
            protected override bool AllowInternalType(Type type)
            {
                if (type.FullName == "Microsoft.Test.Xaml.Types.Custom_InternalButton")
                {
                    _internalTypeAllowed = true;
                    return true;
                }

                return base.AllowInternalType(type);
            }

            internal bool _internalTypeAllowed = false;
        }
        #endregion CustomMapper

        /// <summary>
        /// Throws an exception with the given error message, if the condition is false.
        /// </summary>
        /// <param name="condition">Given condition</param>
        /// <param name="errorMesg">Error message for the exception to be thrown</param>
        private static void Assert(bool condition, String errorMesg)
        {
            if (!condition)
            {
                throw new Microsoft.Test.TestValidationException(errorMesg);
            }
        }

        // Xmlns declarations and mappings needed by the Xaml we use.
        private const string _avalonNS = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private const string _xamlNS = "http://schemas.microsoft.com/winfx/2006/xaml";
        private static readonly string[] s_xtMapping = { "http://test/XamlTests", "Microsoft.Test.Xaml.Types", "XamlWpfTypes"};
        private static readonly string[] s_trMapping = { "http://test/TestRuntime", "Microsoft.Test.Serialization.CustomElements", "TestRuntime" };
        private static readonly string[] s_sysMapping = { "http://test/System", "System", "mscorlib" };
        private static readonly string[] s_ptcMapping = { "http://test/ParserTestControls", "Com.ControlStore", "ParserTestControlsV1" /*,Version=1.1.0.0,Culture=neutral,PublicKeyToken=a069dee354d95f76"*/ };

        private const string _testXaml = "NamespaceMapTest.xaml";
        private const string _snippetFile = "InternalTypeTest_MarkupSnippets.xaml";
        private const string _baseXamlFile = "InternalTypeTestBase.xaml";
        private const string _tempXamlFile = "__InternalTypeTestTempFile.xaml";        
        private SerializationHelper _helper = new SerializationHelper();
    }
}
