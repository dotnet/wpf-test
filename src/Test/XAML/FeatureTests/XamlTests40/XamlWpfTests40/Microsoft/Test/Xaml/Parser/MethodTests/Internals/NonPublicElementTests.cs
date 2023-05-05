// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Xaml;
using System.Xaml.Permissions;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Internals
{
    /// <summary>
    /// InternalElement Tests
    /// </summary>
    public static class NonPublicElementTests
    {
        /// <summary>
        /// Entry point for Protected load tests
        /// </summary>
        public static void RunProtectedLoadTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            string xamlFile = DriverState.DriverParameters["TestParams"];
            string trustLevel = DriverState.DriverParameters["SecurityLevel"];
            XmlReader xmlReader = XmlReader.Create(xamlFile);
            XamlXmlReaderSettings settings = new XamlXmlReaderSettings();

            settings.AllowProtectedMembersOnRoot = true;
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader, settings);

            XamlObjectWriter ow = new XamlObjectWriter(xtr.SchemaContext);

            if (trustLevel == "FullTrust")
            {
                XamlServices.Transform(xtr, ow);
            }
            else
            {
                string resourceId;
                switch (xamlFile)
                {
                    case "Protected_Collections.xaml":
                        resourceId = "GetValue";
                        break;
                    case "Protected_Event.xaml":
                        resourceId = "TypeConverterFailed";
                        break;
                    default:
                        resourceId = "SetValue";
                        break;
                }

                ExceptionHelper.ExpectException(() => XamlServices.Transform(xtr, ow), new XamlObjectWriterException(), resourceId, Globalization.WpfBinaries.SystemXaml);
                return;
            }

            object rootElement = ow.Result;
            switch (xamlFile)
            {
                case "Protected_Ambient.xaml":
                    VerifyAmbient(rootElement);
                    break;
                case "Protected_Basic.xaml":
                    VerifyBasic(rootElement);
                    break;
                case "Protected_Collections.xaml":
                    VerifyCollections(rootElement);
                    break;
                case "Protected_Content.xaml":
                    VerifyContent(rootElement);
                    break;
                case "Protected_MarkupExtension.xaml":
                    VerifyMarkupExtension(rootElement);
                    break;
                case "Protected_TypeConverter.xaml":
                    VerifyTypeConverter(rootElement);
                    break;
                case "Protected_Event.xaml":
                    VerifyEvent(rootElement);
                    break;
                default:
                    GlobalLog.LogEvidence("Unknown xaml file: " + xamlFile);
                    break;
            }
        }

        /// <summary>
        /// verify exception is thrown when AllowProtectedMembersOnRoot isn't set
        /// </summary>
        public static void RunProtectedLoadNegativeTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            string xamlFile = DriverState.DriverParameters["TestParams"];
            XmlReader xmlReader = XmlReader.Create(xamlFile);
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);

            XamlObjectWriter ow = new XamlObjectWriter(xtr.SchemaContext);
            ExceptionHelper.ExpectException(() => XamlServices.Transform(xtr, ow), new XamlObjectWriterException(), "CantSetUnknownProperty", Globalization.WpfBinaries.SystemXaml);

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// verify setting a non root protected member throws
        /// </summary>
        public static void RunProtectedLoadNestedNegativeTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            string xamlFile = DriverState.DriverParameters["TestParams"];

            XamlXmlReaderSettings settings = new XamlXmlReaderSettings();
            settings.AllowProtectedMembersOnRoot = true;

            XmlReader xmlReader = XmlReader.Create(xamlFile);
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader, settings);

            XamlObjectWriter ow = new XamlObjectWriter(xtr.SchemaContext);
            ExceptionHelper.ExpectException(() => XamlServices.Transform(xtr, ow), new XamlObjectWriterException(), "CantSetUnknownProperty", Globalization.WpfBinaries.SystemXaml);

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// creates a queue of xaml nodes that include a private property and verify XamlObjectWriter can set it
        /// XamlXmlReader doesn't support this so starting from XAML isn't possible
        /// </summary>
        public static void RunPrivateLoadTest()
        {
            var writerSettings = new XamlObjectWriterSettings
            {
                AccessLevel = XamlAccessLevel.PrivateAccessTo(typeof(Custom_Private))
            };
            var objectWriter = new XamlObjectWriter(new XamlSchemaContext(), writerSettings);
            var nodeQueue = new XamlNodeQueue(objectWriter.SchemaContext);
            var writer = nodeQueue.Writer;

            XamlType xamlType = writer.SchemaContext.GetXamlType(typeof(Custom_Private));
            writer.WriteStartObject(xamlType);
            writer.WriteStartMember(xamlType.GetMember("ProtectedString"));
            writer.WriteValue("ProtectedValue");
            writer.WriteEndMember();

            Type privateType = typeof(Custom_Private);
            PropertyInfo privateProperty = privateType.GetProperty("PrivateString", BindingFlags.Instance | BindingFlags.NonPublic);
            XamlMember privateMember = new XamlMember(privateProperty, writer.SchemaContext);
            writer.WriteStartMember(privateMember);
            writer.WriteValue("PrivateValue");
            writer.WriteEndMember();
            writer.WriteEndObject();

            XamlServices.Transform(nodeQueue.Reader, objectWriter);

            var result = (Custom_Private)objectWriter.Result;

            if ((string)result.GetNonpublicProperty("PrivateString") != "PrivateValue")
            {
                Fail("PrivateString not set correctly: " + result.GetNonpublicProperty("PrivateString"));
            }

            if ((string)result.GetNonpublicProperty("ProtectedString") != "ProtectedValue")
            {
                Fail("ProtectedString not set correctly: " + result.GetNonpublicProperty("ProtectedString"));
            }
        }

        /// <summary>
        /// creates a queue of xaml nodes that include a private property and verify XamlObjectWriter can set it
        /// only set AssemblyAccess and verify it fails
        /// </summary>
        public static void RunPrivateLoadNegativeTest()
        {
            var writerSettings = new XamlObjectWriterSettings
            {
                AccessLevel = XamlAccessLevel.AssemblyAccessTo(FindLoadedAssembly("XamlCommon"))
            };
            var objectWriter = new XamlObjectWriter(new XamlSchemaContext(), writerSettings);
            var nodeQueue = new XamlNodeQueue(objectWriter.SchemaContext);
            var writer = nodeQueue.Writer;

            XamlType xamlType = writer.SchemaContext.GetXamlType(typeof(Custom_Private));
            writer.WriteStartObject(xamlType);
            writer.WriteStartMember(xamlType.GetMember("ProtectedString"));
            writer.WriteValue("ProtectedValue");
            writer.WriteEndMember();

            Type privateType = typeof(Custom_Private);
            PropertyInfo privateProperty = privateType.GetProperty("PrivateString", BindingFlags.Instance | BindingFlags.NonPublic);
            XamlMember privateMember = new XamlMember(privateProperty, writer.SchemaContext);
            writer.WriteStartMember(privateMember);
            writer.WriteValue("PrivateValue");
            writer.WriteEndMember();
            writer.WriteEndObject();

            try
            {
                XamlServices.Transform(nodeQueue.Reader, objectWriter);
                throw new TestValidationException("Expected exception not thrown.");
            }
            catch (XamlObjectWriterException)
            {
                TestLog.Current.Result = TestResult.Pass;
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
            var element = (Custom_Protected_ProtectedAmbient)rootElement;
            if ((int)element.GetNonpublicProperty("ProtectedAmbientInt") != 5)
            {
                Fail("Wrong value returned: " + element.GetNonpublicProperty("ProtectedAmbientInt"));
            }

            TestLog.Current.Result = TestResult.Pass;
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
            Custom_Type_ProtectedProp root = (Custom_Type_ProtectedProp)rootElement;

            if (root.GetProtectedString() != "ProtectedStringTest2")
            {
                Fail("ProtectedString: ProtectedString was not ProtectedStringTest2");
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
            Custom_Protected_Collections protectedCollectionsElement = (Custom_Protected_Collections)rootElement;
            if (protectedCollectionsElement.Name != "Test1")
            {
                Fail("Test1: Name was not Test1");
            }

            Custom_Protected element1 = (Custom_Protected)((List<object>)protectedCollectionsElement.GetNonpublicProperty("ListRO"))[0];
            if (element1.Name != "Test2")
            {
                Fail("Test2: Name was not Test2");
            }

            Custom_Protected element2 = (Custom_Protected)((List<object>)protectedCollectionsElement.GetNonpublicProperty("ListRW"))[0];
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
            var protectedContent = (Custom_Protected_ProtectedContent)rootElement;
            var content = (Custom_Protected)protectedContent.GetNonpublicProperty("Content");

            if (content.Name != "Test2")
            {
                Fail("Test2:  Name was not Test2");
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
            var content = (Custom_Protected)rootElement;

            if ((string)content.GetNonpublicProperty("ProtectedString") != "MEtest")
            {
                Fail("Test1: ProtectedString was not MEtest " + content.GetNonpublicProperty("ProtectedString"));
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
            var content = (Custom_Protected_ProtectedTypeProp)rootElement;
            var protectedValue = (Custom_Protected)content.GetNonpublicProperty("Custom_ProtectedProperty");
            
            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verfies:
        /// internal Event
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyEvent(object rootElement)
        {
            var content = (Custom_Type_ProtectedEvent)rootElement;

            if (!content.EventAdded())
            {
                Fail("Event not added.");
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Fails the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void Fail(string message)
        {
            TestLog.Current.Result = TestResult.Fail;
            throw new TestValidationException(message);
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
