// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Xaml;
using System.Xaml.Permissions;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Internals
{
    /// <summary>
    /// InternalElement Tests
    /// </summary>
    public static class InternalElementTests
    {
        /// <summary>
        /// Entry point for Internal load tests
        /// </summary>
        public static void RunInternalLoadTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            string xamlFile = DriverState.DriverParameters["TestParams"];
            XmlReader xmlReader = XmlReader.Create(xamlFile);
            XamlXmlReaderSettings settings = new XamlXmlReaderSettings();
            settings.LocalAssembly = FindLoadedAssembly("XamlClrTypes");
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader, settings);
            XamlObjectWriterSettings writerSettings = new XamlObjectWriterSettings
            {
                AccessLevel = XamlAccessLevel.AssemblyAccessTo(settings.LocalAssembly)
            };

            XamlObjectWriter ow = new XamlObjectWriter(xtr.SchemaContext, writerSettings);

            XamlServices.Transform(xtr, ow);
            object rootElement = ow.Result;
            switch (xamlFile)
            {
                case "Internal_Ambient.xaml":
                    VerifyAmbient(rootElement);
                    break;
                case "Internal_Basic.xaml":
                    VerifyBasic(rootElement);
                    break;
                case "Internal_Collections.xaml":
                    VerifyCollections(rootElement);
                    break;
                case "Internal_Content.xaml":
                    VerifyContent(rootElement);
                    break;
                case "Internal_Ctor.xaml":
                    VerifyCtor(rootElement);
                    break;
                case "Internal_MarkupExtension.xaml":
                    VerifyMarkupExtension(rootElement);
                    break;
                case "Internal_Name.xaml":
                    VerifyName(rootElement);
                    break;
                case "Internal_RootElement.xaml":
                    VerifyRootElement(rootElement);
                    break;
                case "Internal_TypeConverter.xaml":
                    VerifyTypeConverter(rootElement);
                    break;
                case "Internal_Event.xaml":
                    VerifyEvent(rootElement);
                    break;
                default:
                    GlobalLog.LogEvidence("Unknown xaml file: " + xamlFile);
                    break;
            }
        }

        /// <summary>
        /// Verifies :  Ability to set internal ambient properties
        /// Internal ambient properties work in internal ME
        /// Internal ambient properties work in internal TC
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyAmbient(object rootElement)
        {
            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;
            foreach (object obj in root.Content)
            {
                Custom_Internal_InternalAmbient element = (Custom_Internal_InternalAmbient) obj;
                switch (element.Name)
                {
                    case "Test1":
                        if (element.InternalAmbientInt != 5)
                        {
                            Fail("Test1: element.InternalAmbientInt != 5");
                        }

                        break;
                    case "Test2":
                        if (element.InternalAmbientInt != 6)
                        {
                            Fail("Test2: element.InternalAmbientInt != 6");
                        }

                        Custom_Internal element1 = (Custom_Internal) element.Content;
                        if (element1.InternalString != "intstr6")
                        {
                            Fail("Test2: InternalString was not intstr6");
                        }

                        break;
                    case "Test3":
                        if (element.InternalAmbientInt != 7)
                        {
                            Fail("Test3: element.InternalAmbientInt != 7");
                        }

                        Custom_Internal_InternalTypeProp element2 = (Custom_Internal_InternalTypeProp) element.Content;
                        if (element2.Custom_InternalProperty.InternalString != "tcTest7")
                        {
                            Fail("Test3: Custom_InternalProperty.InternalString was not tcTest7");
                        }

                        break;
                    default:
                        Fail("Unknown Name: " + element.Name);
                        break;
                }

                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Verfies:
        /// Internal Type
        /// Internal Type w/ public prop
        /// Internal Type w/ internal prop
        /// Internal Type w/ directive
        /// Public type w/ internal prop
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyBasic(object rootElement)
        {
            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;
            Custom_Internal element1 = (Custom_Internal) root.Content[0]; 

            // Internal Type
            if (!string.IsNullOrEmpty(element1.Name)) 
            {
                Fail("Test0: Name was not empty");
            }

            Custom_Internal element2 = (Custom_Internal) root.Content[1];
            if (element2.Name != "Test1") // Internal Type, public prop
            {
                Fail("Test1: Name was not Test1");
            }

            Custom_Internal element3 = (Custom_Internal) root.Content[2];
            if (element3.Name != "Test2") // Internal Type, directive
            {
                Fail("Test2: Name was not Test2");
            }

            Custom_Internal element4 = (Custom_Internal) root.Content[3];
            if (element4.InternalString != "internalStringTest") // Internal Type, internal prop
            {
                Fail("Test3: InternalString was not internalStringTest");
            }

            Custom_Type_InternalProp element5 = (Custom_Type_InternalProp) root.Content[4];
            if (element5.InternalString != "internalStringTest2") // Public type, internal prop
            {
                Fail("Test4: InternalString was not internalStringTest2");
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verfies:
        /// Internal RO collection
        /// Internal RW collection
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyCollections(object rootElement)
        {
            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;
            Custom_Internal_Collections internalCollectionsElement = (Custom_Internal_Collections) root.Content[0];
            if (internalCollectionsElement.Name != "Test1")
            {
                Fail("Test1: Name was not Test1");
            }

            Custom_Internal element1 = (Custom_Internal) internalCollectionsElement.ListRO[0];
            if (element1.Name != "Test2")
            {
                Fail("Test2: Name was not Test2");
            }

            Custom_Internal element2 = (Custom_Internal) internalCollectionsElement.ListRW[0];
            if (element2.Name != "Test3")
            {
                Fail("Test3: Name was not Test3");
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verfies:
        /// internal content
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyContent(object rootElement)
        {
            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;
            Custom_Internal parentElement1 = (Custom_Internal) root.Content[0];
            Custom_Internal element1 = (Custom_Internal) parentElement1.Content;
            if (element1.Name != "Test1")
            {
                Fail("Test1: Name was not Test1");
            }

            Custom_Internal_InternalContent parentElement2 = (Custom_Internal_InternalContent) root.Content[1];
            Custom_Internal element2 = (Custom_Internal) parentElement2.Content;
            if (element2.Name != "Test2")
            {
                Fail("Test2: Name was not Test2");
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verfies:
        /// internal ctor
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyCtor(object rootElement)
        {
            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;
            Custom_Internal_InternalCtor element1 = (Custom_Internal_InternalCtor) root.Content[0];

            if (element1.Name != "Test1")
            {
                Fail("Test1: Name was not Test1");
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verfies:
        /// internal MarkupExtension
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyMarkupExtension(object rootElement)
        {
            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;
            Custom_Internal element1 = (Custom_Internal) root.Content[0];

            if (element1.InternalString != "MEtest")
            {
                Fail("Test1: InternalString was not MEtest " + element1.InternalString);
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verfies:
        /// internal RuntimeName
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyName(object rootElement)
        {
            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;
            Custom_Internal_InternalName element1 = (Custom_Internal_InternalName) root.Content[0];

            if (element1.Name != "Test1")
            {
                Fail("Test1: Name was not Test1");
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verfies:
        /// internal root element
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyRootElement(object rootElement)
        {
            Custom_Internal root = (Custom_Internal) rootElement;
            Custom_Internal element1 = (Custom_Internal) root.Content;

            if (element1.Name != "Test1")
            {
                Fail("Test1: Name was not Test1");
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verfies:
        /// internal TypeConverter
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyTypeConverter(object rootElement)
        {
            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;
            Custom_Internal_InternalTypeProp element1 = (Custom_Internal_InternalTypeProp) root.Content[0];

            if (element1.Custom_InternalProperty.InternalString != "TCtest")
            {
                Fail("Test1: Custom_InternalProperty.InternalString was not TCtest");
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verfies:
        /// internal Event
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyEvent(object rootElement)
        {
            Custom_Type_InternalEvent root = rootElement as Custom_Type_InternalEvent;
            
            if (root == null)
            {
                Fail("Type deserialized was not a Custom_Type_InternalEvent");
                return;
            }

            if (!root.EventAdded())
            {
                Fail("Event not set on instance.");
                return;
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Fails the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void Fail(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                GlobalLog.LogEvidence(message);
            }

            TestLog.Current.Result = TestResult.Fail;
        }

        /// <summary>
        /// Finds the loaded assembly.
        /// </summary>
        /// <param name="asmName">Name of the asm.</param>
        /// <returns> Assembly object </returns>
        private static Assembly FindLoadedAssembly(string asmName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                if (asm.FullName.Contains(asmName))
                {
                    return asm;
                }
            }

            return null;
        }
    }
}
