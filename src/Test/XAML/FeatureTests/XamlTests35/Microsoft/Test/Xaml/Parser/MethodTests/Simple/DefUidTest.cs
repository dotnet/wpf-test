// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Serialization;
using System;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Parser;
using System.IO.Packaging;


namespace Microsoft.Test.Xaml.Parser.MethodTests.Simple
{

#region Class DefUidTest

    /// <summary>
    /// A class to test the handling of Def:Uid attributes
    /// </summary>
    /// <remarks>
    /// This class checks that Def:Uid are honored for UIElements and discarded for others.
    /// </remarks>
    public class DefUidTest
    {

        /// <summary>
        /// Verify that styles having x:Uids load fine.
        /// </summary>
        public void CheckStyle()
        {
            String xamlfilename = "DefUidInStyle.xaml";

            // The following uses LoadXml() to load the XAML
            // We are just verifying that the file can load.
            // If an exception is thrown, the test fails.
            ParseXamlFile(xamlfilename);
        }
        /// <summary>
        /// Check that x:Uid properties are handled correctly when LoadXml() is used 
        /// to create the tree. 
        /// This means:
        /// 1. DefinitionProperties.UidProperty is set properly on derivatives of UIElement
        /// 2. DefinitionProperties.UidProperty is not set (returns default value) on others
        /// </summary>[TestAttribute(0, @"Parser\Simple", "CheckXaml", SupportFiles=@"FeatureTests\ElementServices\DefUidTest.xaml", Area="XAML")]
        public void CheckXaml()
        {
            String xamlfilename = "DefUidTest.xaml";

            // The following uses LoadXml() to load the XAML
            DependencyObject root = (DependencyObject) ParseXamlFile(xamlfilename);
            GlobalLog.LogStatus("xaml loaded.");

            // Get the DefinitionProperties.UidProprety object. 
            // DefinitionProperties is internal, so we have to use Reflection
            // First get the PresentationFramework assembly, then get the DefinitionProperties type.
            Assembly presentationFxAssembly = Assembly.GetAssembly(typeof(UIElement));
            Type               definitionpropertiestype = presentationFxAssembly.GetType("System.Windows.UIElement");
            DependencyProperty defuidproperty           = DependencyPropertyFromName("Uid", definitionpropertiestype);


            // Get the Button, and check that DefinitionProperties.UidProprety has the right value
            Button button0      = (Button) LogicalTreeHelper.FindLogicalNode(root, "Button0");
            String buttondefuid = button0.GetValue(defuidproperty) as String;
            Assert(buttondefuid == "1", "Button's Def:Uid not set properly");

            // Get the Column and check that DefinitionProperties.UidProprety is the default value
            // of String.Empty
            TableColumn column0      = (TableColumn) LogicalTreeHelper.FindLogicalNode(root, "Column0");
            String      columndefuid = column0.GetValue(defuidproperty) as String;
            Assert(columndefuid == String.Empty, "Column's Def:Uid not set properly");
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
                throw new TestValidationException(errorMesg);
            }
        }

#region ParseXamlFile

#endregion

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
            return (fi != null) ? fi.GetValue(null) as DependencyProperty : null;
        }
#region ParseXamlFile

        /// <summary>
        /// ParseXamlFile is used to LoadXml(xaml).
        /// </summary>
        /// <param name="filename">File to be parsed</param>
        private Object ParseXamlFile(string filename)
        {
            IXamlTestParser parser = XamlTestParserFactory.Create();
            Object root = parser.LoadXaml(filename, null);
            return root;
        }

#endregion ParseXamlFile

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
    }

#endregion Class DefUidTest
}
