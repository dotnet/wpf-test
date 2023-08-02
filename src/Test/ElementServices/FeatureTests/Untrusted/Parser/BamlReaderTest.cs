// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Serialization;

using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Markup;
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Permissions;

using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// Read a Baml file using BamlReaderWrapper and verify the contents of various nodes.
    /// </summary>
    public class BamlReaderTest
    {
        #region BamlReader1
        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void BamlReader1()
        {   
            string xamlName = "BamlReaderWriterTest.xaml";

            // Compile the XAML into BAML
            string bamlPath = ParserUtil.CompileXamlToBaml(xamlName);

            // Read the BAML using ReadBaml. Perform all the validations in the callback. 
            BamlHelper.ReadBaml(bamlPath, new BamlHelper.BamlNodeCallback(ReadBamlNodes));

            // If all validations succeed (none throws an exception), then we pass the test.
            // If any of them throws an exception, we don't reach here.
            CoreLogger.LogTestResult(true, "Test passed.");
        }

        /// <summary>
        /// Callback function for receiving Baml node information.
        /// We do all the validations in this function, and throw an exception
        /// if something doesn't validate.
        /// </summary>
        /// <param name="actualData"></param>
        /// <param name="writer"></param>
        public BamlNodeAction ReadBamlNodes(BamlNodeData actualData, BamlWriterWrapper writer)
        {
            // We will put the expected values in the expectedData instance,
            // and then compare them with the actual values received in actualData
            BamlNodeData expectedData = new BamlNodeData();

            if (actualData.NodeType == "Property")
            {
                switch (actualData.LocalName)
                {    
                    case "MyTransparency":
                        {
                            expectedData.Name = "Avalon.Test.CoreUI.Parser.MyClass.MyTransparency";
                            expectedData.LocalName = "MyTransparency";
                            expectedData.Prefix = "cc";
                            expectedData.XmlNamespace = "core";
              expectedData.AssemblyName = typeof(BamlReaderTest).Assembly.GetName().FullName;
              expectedData.ClrNamespace = "Avalon.Test.CoreUI.Parser";
                            expectedData.Value = "1.0";    

                            CompareBamlNodes(actualData, expectedData);
                        }
                        break;
                    
                    
                    // Button's Foreground
                    case "Foreground":
                        {
                            expectedData.Name = actualData.Name;
                            expectedData.LocalName = "Foreground";
                            expectedData.Prefix = String.Empty;
              expectedData.XmlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                            expectedData.AssemblyName = actualData.AssemblyName;
                            expectedData.ClrNamespace = actualData.ClrNamespace;
                            expectedData.Value = "{GreenBrush}";

                            CompareBamlNodes(actualData, expectedData);
                        }
                        break;
                }
                
            }
            // Values should be the same for StartComplexProperty and EndComplexProperty
            else if (actualData.NodeType == "StartComplexProperty" || actualData.NodeType == "EndComplexProperty")
            {
                switch (actualData.LocalName)
                {    
                    // Normal complex property. SolidColorBrush.Color
                    case "Color" :
                        {
                            expectedData.Name = "System.Windows.Media.SolidColorBrush.Color";
                            expectedData.LocalName = "Color";
                            expectedData.Prefix = String.Empty;
              expectedData.XmlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                            expectedData.AssemblyName = typeof(UIElement).Assembly.GetName().FullName;
                            expectedData.ClrNamespace = "System.Windows.Media";
                            expectedData.Value = String.Empty;

                            CompareBamlNodes(actualData, expectedData);
                        }
                        break;

                    // Complex IList property. ListBox.Items
                    case "Items":
                        {
                            // Should the following be ListBox instead of ItemsControl? 
                            // Check the conversation with Peterost.
                            expectedData.Name = "System.Windows.Controls.ItemsControl.Items";
                            expectedData.LocalName = "Items";
              expectedData.Prefix = String.Empty;
              expectedData.XmlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                            expectedData.AssemblyName = typeof(FrameworkElement).Assembly.GetName().FullName;
                            expectedData.ClrNamespace = "System.Windows.Controls";
                            expectedData.Value = "";

                            CompareBamlNodes(actualData, expectedData);
                        }
                        break;
                }
            }
            // Values should be the same for StartElement and EndElement
            else if (actualData.NodeType == "StartElement" || actualData.NodeType == "EndElement")
            {
                switch (actualData.LocalName)
                {
                    case "Button":
                        {
                            expectedData.Name = "System.Windows.Controls.Button";
                            expectedData.LocalName = "Button";
                            expectedData.Prefix = String.Empty;
              expectedData.XmlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                            expectedData.AssemblyName = typeof(FrameworkElement).Assembly.GetName().FullName;
                            expectedData.ClrNamespace = "System.Windows.Controls";
                            expectedData.Value = String.Empty;

                            CompareBamlNodes(actualData, expectedData);
                        }
                        break;
                }
            }
            // PIMapping 
            else if (actualData.NodeType == "PIMapping")
            {
                switch (actualData.XmlNamespace)
                {
                    case "core":
                        {
                            expectedData.Name = "Mapping";
                            expectedData.LocalName = "Mapping";
                            expectedData.Prefix = String.Empty;
                            expectedData.XmlNamespace = "core";
              expectedData.AssemblyName = typeof(BamlReaderTest).Assembly.GetName().FullName;
              expectedData.ClrNamespace = "Avalon.Test.CoreUI.Parser";
              expectedData.Value = "XmlNamespace=\"core\" ClrNamespace=\"Avalon.Test.CoreUI.Parser\" Assembly=\"+typeof(this)).Assembly.GetName(.FullName + \"";

              CompareBamlNodes(actualData, expectedData);
                        }
                        break;
                }
            }
            // XmlnsProperty 
            else if (actualData.NodeType == "XmlnsProperty")
            {
                switch (actualData.Prefix)
                {
                    case "":
                        {
                            expectedData.Name = "xmlns";
                            expectedData.LocalName = "xmlns";
                            expectedData.Prefix = "";
                            expectedData.XmlNamespace = String.Empty;
                            expectedData.AssemblyName = String.Empty;
                            expectedData.ClrNamespace = String.Empty;
                            expectedData.Value = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

                            CompareBamlNodes(actualData, expectedData);
                        }
                        break;

                    case "ctrl":
                        {
                            expectedData.Name = "xmlns:ctrl";
                            expectedData.LocalName = "xmlns:ctrl";
                            expectedData.Prefix = "ctrl";
                            expectedData.XmlNamespace = String.Empty;
                            expectedData.AssemblyName = String.Empty;
                            expectedData.ClrNamespace = String.Empty;
                            expectedData.Value = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

                            CompareBamlNodes(actualData, expectedData);
                        }
                        break;

                    // 

                        /*
                    case "tef":
                        {
                            expectedData.Name = "xmlns:tef";
                            expectedData.LocalName = "xmlns:tef";
                            expectedData.Prefix = "tef";
                            expectedData.XmlNamespace = String.Empty;
                            expectedData.AssemblyName = String.Empty;
                            expectedData.ClrNamespace = String.Empty;
                            expectedData.Value ="http://schemas.microsoft.com/winfx/2006/xaml";

                            CompareBamlNodes(actualData, expectedData);
                        }
                        break;
                        */
                }
            }
            // DefAttribute
            else if (actualData.NodeType == "DefAttribute")
            {
                switch (actualData.Value)
                {
                    case "GreenColor":
                        {
                            expectedData.Name = "Name";
                            expectedData.LocalName = "Name";
                            expectedData.Prefix = "tef";
                            expectedData.XmlNamespace ="http://schemas.microsoft.com/winfx/2006/xaml";
                            expectedData.AssemblyName = String.Empty;
                            expectedData.ClrNamespace = String.Empty;
                            expectedData.Value = "GreenColor";

                            CompareBamlNodes(actualData, expectedData);
                        }
                        break;
                }
            }
            // IncludeReference
            else if (actualData.NodeType == "IncludeReference")
            {
                expectedData.Name = String.Empty;
                expectedData.LocalName = String.Empty;
                expectedData.Prefix = String.Empty;
                expectedData.XmlNamespace = String.Empty;
                expectedData.AssemblyName = String.Empty;
                expectedData.ClrNamespace = String.Empty;
                expectedData.Value = "{GreenColor}";

                CompareBamlNodes(actualData, expectedData);
            }

            return BamlNodeAction.Continue;
        }

        /// <summary>
        /// A function that compares the actual field values of a BamlNodeData with those expected,
        /// and throws an exception at the first mis-match.
        /// </summary>
        /// <param name="actualData">Actual data</param>
        /// <param name="expectedData">Expected data</param>
        private static void CompareBamlNodes(BamlNodeData actualData, BamlNodeData expectedData)
        {
            // Compare Name
            if(expectedData.Name != actualData.Name)
            {
                throw new Microsoft.Test.TestValidationException("Field different from expected in Baml node type " + actualData.NodeType + "\n"
                + " Name (expected): " + expectedData.Name + "\n"
                    + " Name (found): " + actualData.Name);                
            }

            // Compare LocalName
            if (expectedData.LocalName != actualData.LocalName)
            {
                throw new Microsoft.Test.TestValidationException("Field different from expected in Baml node type " + actualData.NodeType + "\n" 
                    + " LocalName (expected): " + expectedData.LocalName + "\n" 
                    + " LocalName (found): " + actualData.LocalName);
            }

            // Compare ClrNamespace
            if (expectedData.ClrNamespace != actualData.ClrNamespace)
            {
                throw new Microsoft.Test.TestValidationException("Field different from expected in Baml node type " + actualData.NodeType + "\n" 
                    + " ClrNamespace (expected): " + expectedData.ClrNamespace + "\n" 
                    + " ClrNamespace (found): " + actualData.ClrNamespace);
            }

            // Compare XmlNamespace
            if (expectedData.XmlNamespace != actualData.XmlNamespace)
            {
                throw new Microsoft.Test.TestValidationException("Field different from expected in Baml node type " + actualData.NodeType + "\n" 
                    + " XmlNamespace (expected): " + expectedData.XmlNamespace + "\n" 
                    + " XmlNamespace (found): " + actualData.XmlNamespace);
            }

            // Compare AssemblyName
            if (expectedData.AssemblyName != actualData.AssemblyName)
            {
                throw new Microsoft.Test.TestValidationException("Field different from expected in Baml node type " + actualData.NodeType + "\n" 
                    + " AssemblyName (expected): " + expectedData.AssemblyName + "\n" 
                    + " AssemblyName (found): " + actualData.AssemblyName);
            }

            // Compare Prefix
            if (expectedData.Prefix != actualData.Prefix)
            {
                throw new Microsoft.Test.TestValidationException("Field different from expected in Baml node type " + actualData.NodeType + "\n" 
                    + " Prefix (expected): " + expectedData.Prefix + "\n" 
                    + " Prefix (found): " + actualData.Prefix);
            }

            // Compare Value
            if (expectedData.Value != actualData.Value)
            {
                throw new Microsoft.Test.TestValidationException("Field different from expected in Baml node type " + actualData.NodeType + "\n" 
                    + " Value (expected): " + expectedData.Value + "\n" 
                    + " Value (found): " + actualData.Value);
            }
        }
        #endregion BamlReader1
    }
}
