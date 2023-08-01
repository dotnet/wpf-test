// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Serialization;

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;

using System.Reflection;
using System.Collections;
using System.Xml;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Parser
{
    #region Class DefUidTest
    /// <summary>
    /// A class to test the handling of Def:Uid attributes (PS #12392)
    /// </summary>
    /// <remarks>
    /// This class checks that Def:Uid are honored for UIElements and discarded for others.
    /// </remarks>
    [TestDefaults]
    public class DefUidTest
    {
        /// <summary>
        /// Check that x:Uid properties are handled correctly when LoadXml() is used 
        /// to create the tree. 
        /// This means:
        /// 1. DefinitionProperties.UidProperty is set properly on derivatives of UIElement
        /// 2. DefinitionProperties.UidProperty is not set (returns default value) on others
        /// </summary>
        public void CheckXaml()
        {
            String XamlFilename = "DefUidTest.xaml";

            // The following uses LoadXml() to load the XAML
            DependencyObject root = (DependencyObject)ParseXamlFile(XamlFilename);
            CoreLogger.LogStatus("xaml loaded.");

            // Get all the nodes with IDs
            Hashtable nodesWithIDs = new Hashtable();
            TreeHelper.FindNodesWithIds(root, nodesWithIDs);
            CoreLogger.LogStatus("Got nodes with IDs.");

            // Use UIElement.UidProperty to get the value of Uid
            Assembly presentationFxAssembly = Assembly.GetAssembly(typeof(UIElement));
            Type DefinitionPropertiesType = presentationFxAssembly.GetType("System.Windows.UIElement");
            DependencyProperty DefUidProperty = DependencyPropertyFromName("Uid", DefinitionPropertiesType);

            // Get the Button, and check that DefinitionProperties.UidProprety has the right value
            Button Button0 = (Button)nodesWithIDs["Button0"];
            String ButtonDefUid = Button0.GetValue(DefUidProperty) as String;
            Assert(ButtonDefUid == "1", "Button's Def:Uid not set properly");

            // Get the Column and check that DefinitionProperties.UidProprety is the default value
            // of String.Empty
            TableColumn Column0 = (TableColumn)nodesWithIDs["Column0"];
            String ColumnDefUid = Column0.GetValue(DefUidProperty) as String;
            Assert(ColumnDefUid == String.Empty, "Column's Def:Uid not set properly");
        }

        ///// <summary>
        ///// Compile the XAML into BAML,
        ///// and check that x:Uid properties are present in BAML.
        ///// This is because according to RogerCh, x:Uids are handled when tree is created 
        ///// from BAML, not when BAML is created from XAML. 
        ///// </summary>
        //[TestAttribute(0, @"Parser\Simple", TestCaseSecurityLevel.FullTrust, "CheckBaml", SupportFiles=@"FeatureTests\ElementServices\DefUidTest.xaml", Area="XAML", Disabled=true)]
        //public void CheckBaml()
        //{
        //    String XamlFilename = "DefUidTest.xaml";

        //    CompilerHelper compiler = new CompilerHelper();
        //    // Cleanup old compile directories and files if necessary.
        //    compiler.CleanUpCompilation();
        //    compiler.AddDefaults();

        //    // Compile xaml 
        //    CoreLogger.LogStatus("Starting Compilation.....");
        //    compiler.CompileApp(XamlFilename);

        //    // Load compiled app (.exe) in current AppDomain. 
        //    // It contains the root type needed to read the BAML.
        //    Assembly.LoadFile(compiler.CompiledExecutablePath);

        //    // Read the BAML using ReadBaml. 
        //    BamlHelper.ReadBaml(compiler.BamlPath, new BamlHelper.BamlNodeCallback(ReadBamlNodes));

        //    // Verify that expected Def:Uids are present and unexpected are absent.                
        //    Assert(defUid1found && !defUid2found && defUid3found && defUid5found, "Def:Uids expected in BAML but not found.");
        //}

        // Flags used to indicate whether particular x:Uids are found in the BAML.
        // These are declared outside of ReadBamlNodes(), because that function is called 
        // multiple times, and thus cannot verify them at once.
        //private bool defUid1found = false;
        //private bool defUid2found = false;
        //private bool defUid3found = false;
        //private bool defUid5found = false;
        private DependencyProperty DependencyPropertyFromName(string propertyName, Type propertyType)
        {
            FieldInfo fi = propertyType.GetField(propertyName + "Property", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return (fi != null)?fi.GetValue(null) as DependencyProperty:null;
        }
         
        /// <summary>
        /// Callback function for receiving Baml node information.
        /// When a particular Def:Uid is found, we set one of the above flags
        /// accordingly.
        /// </summary>
        /// <param name="nodeData">BAML node data received</param>
        /// <param name="writer">We don't use this here.</param>
        private BamlNodeAction ReadBamlNodes(BamlNodeData nodeData, BamlWriterWrapper writer)
        {
            //if (nodeData.NodeType == "DefAttribute")
            //{
            //    if (nodeData.Name == "Uid" && nodeData.Value == "1")
            //        defUid1found = true;
            //    else if (nodeData.Name == "Uid" && nodeData.Value == "2")
            //        defUid2found = true;
            //    else if(nodeData.Name == "Uid" && nodeData.Value == "3")
            //        defUid3found = true;
            //    else if(nodeData.Name == "Uid" && nodeData.Value == "5")
            //        defUid5found = true;
            //}

            return BamlNodeAction.Continue;
        }        

        /// <summary>
        /// Verify that styles having x:Uids load fine. There was a 


        public void CheckStyle()
        {
            String XamlFilename = "DefUidInStyle.xaml";

            // The following uses LoadXml() to load the XAML
            // We are just verifying that the file can load.
            // If an exception is thrown, the test fails.
            object root = ParseXamlFile(XamlFilename);
        }

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

        #region ParseXamlFile
        /// <summary>
        /// ParseXamlFile is used to LoadXml(xaml).
        /// </summary>
        /// <param name="filename">File to be parsed</param>
        private Object ParseXamlFile(string filename)
        {
            // using Security Warehouse
            Stream xamlFileStream = File.OpenRead(filename);
            
            // see if it loads
            CoreLogger.LogStatus("Parse XAML using Stream..." + filename);
            ParserContext pc = new ParserContext();
            pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
            Object root = System.Windows.Markup.XamlReader.Load(xamlFileStream, pc);

            // done with the stream
            xamlFileStream.Close();
            return root;
        }
        #endregion ParseXamlFile
    }
    #endregion Class DefUidTest
}
