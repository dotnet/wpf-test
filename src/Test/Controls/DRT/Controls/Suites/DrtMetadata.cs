// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;

namespace DRT
{
    public class DrtMetadataSuite : DrtTestSuite
    {
        public DrtMetadataSuite() : base("Metadata")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            Border border = new Border();
            border.Background = Brushes.White;
            DRT.RootElement = border;

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            border.Child = panel;

            TextBlock title = new TextBlock();
            title.Text = "Verifying Controls Metadata ...";
            panel.Children.Add(title);

            DRT.ShowRoot();

            return new DrtTest[]
            {
                new DrtTest(CheckMetadata),
            };
        }


        /// <summary>
        ///     
        ///     
        /// </summary>
        private void CheckMetadata()
        {
            Console.WriteLine();
            Console.WriteLine("---------- Reading Metadata ----------");

            // Read the metadata file and check class attributes
            if (File.Exists(XMLFileName))
            {
                FileStream xmlFile = File.Open(XMLFileName, FileMode.Open, FileAccess.Read);
                if (xmlFile != null)
                {
                    try
                    {
                        XmlTextReader reader = new XmlTextReader(xmlFile);
                        string currentClassName = "";
                        string currentPartName = "";
                        string currentPartType = "";
                        bool isTemplatePartAttribute = false;
                        bool isStyleTypedProperty = false;
                        while (reader.Read())
                        {
                            if (reader.Name == "TemplatePartAttribute")
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    Console.WriteLine("Verify TemplatePartAttribute");
                                    isTemplatePartAttribute = true;
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    isTemplatePartAttribute = false;
                                }
                            }

                            else if (reader.Name == "StyleTypedProperty")
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    Console.WriteLine("Verify StyleTypedProperty");
                                    isStyleTypedProperty = true;
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    isStyleTypedProperty = false;
                                }
                            }

                            else if (reader.Name == "Class")
                            {
                                if (reader.MoveToFirstAttribute())
                                {
                                    if (reader.Name == "Name")
                                        currentClassName = reader.Value;
                                }
                            }

                            else if (reader.Name == "Part")
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "Name")
                                        currentPartName = reader.Value;
                                    if (reader.Name == "Type")
                                        currentPartType = reader.Value;
                                }

                                // Verify currentClassName, currentPartName, currentPartType
                                if (isTemplatePartAttribute)
                                {
                                    Console.WriteLine("Class name: " + currentClassName + "    Part: " + currentPartName + "   TargetType: " + currentPartType);
                                    VerifyTemplatePartAttribute(currentClassName, currentPartName, currentPartType);
                                }
                            }

                            else if (reader.Name == "Property")
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "Name")
                                        currentPartName = reader.Value;
                                    if (reader.Name == "StyleTargetType")
                                        currentPartType = reader.Value;
                                }

                                // Verify currentClassName, currentPartName, currentPartType
                                if (isStyleTypedProperty)
                                {
                                    Console.WriteLine("Class name: " + currentClassName + "    Property: " + currentPartName + "   StyleTargetType: " + currentPartType);
                                    VerifyStyleTypedPropertyAttribute(currentClassName, currentPartName, currentPartType);
                                }
                            }


                        }
                    }
                    finally
                    {
                        xmlFile.Close();
                    }
                }
            }
        }

        private void VerifyTemplatePartAttribute(string currentClassName, string currentPartName, string currentPartType)
        {
            Assembly PresentationFrameworkAssembly = typeof(System.Windows.FrameworkElement).Assembly;
            Assembly PresentationCoreAssembly = typeof(System.Windows.Media.Visual).Assembly;

            Type classType = PresentationFrameworkAssembly.GetType(currentClassName);
            if (classType == null)
                classType = PresentationCoreAssembly.GetType(currentClassName);
            DRT.Assert(classType != null, "Class type " + currentClassName + " not found");

            Type partType = PresentationFrameworkAssembly.GetType(currentPartType);
            if (partType == null)
                partType = PresentationCoreAssembly.GetType(currentPartType);
            DRT.Assert(partType != null, "Part type " + currentPartType + " not found");

            object[] templatePartAttributes = classType.GetCustomAttributes(typeof(TemplatePartAttribute), true);
            bool found = false;
            for (int i = 0; i < templatePartAttributes.Length; i++)
            {
                TemplatePartAttribute attr = templatePartAttributes[i] as TemplatePartAttribute;
                if (attr.Name == currentPartName && attr.Type == partType)
                    found = true;
            }
            DRT.Assert(found, "TemplatePartAttribute not found for " + "Class name=" + currentClassName + " Part=" + currentPartName + " TargetType=" + currentPartType);
        }

        private void VerifyStyleTypedPropertyAttribute(string currentClassName, string propertyName, string styleTargetTypeName)
        {
            Assembly PresentationFrameworkAssembly = typeof(System.Windows.FrameworkElement).Assembly;
            Assembly PresentationCoreAssembly = typeof(System.Windows.Media.Visual).Assembly;

            Type classType = PresentationFrameworkAssembly.GetType(currentClassName);
            if (classType == null)
                classType = PresentationCoreAssembly.GetType(currentClassName);
            DRT.Assert(classType != null, "Class type " + currentClassName + " not found");

            Type styleTargetType = PresentationFrameworkAssembly.GetType(styleTargetTypeName);
            if (styleTargetType == null)
                styleTargetType = PresentationCoreAssembly.GetType(styleTargetTypeName);
            DRT.Assert(styleTargetType != null, "Part type " + styleTargetTypeName + " not found");

            object[] styleTypedPropertyAttributes = classType.GetCustomAttributes(typeof(StyleTypedPropertyAttribute), true);
            bool found = false;
            for (int i = 0; i < styleTypedPropertyAttributes.Length; i++)
            {
                StyleTypedPropertyAttribute attr = styleTypedPropertyAttributes[i] as StyleTypedPropertyAttribute;
                if (attr.Property == propertyName && attr.StyleTargetType == styleTargetType)
                    found = true;
            }
            DRT.Assert(found, "StyleTypedProperty not found for " + "Class name=" + currentClassName + " Property=" + propertyName + " StyleTargetType=" + styleTargetTypeName);
        }

        private const string XMLFileName = @"DrtFiles\Controls\DrtMetadata.xml";
    }


 
}
