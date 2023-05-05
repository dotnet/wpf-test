// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Xaml;
using System.Xaml.Permissions;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.LocalAssembly
{
    /// <summary>
    /// LocalAssembly Tests
    /// </summary>
    public static class LocalAssemblyTests
    {
        /// <summary>
        /// Verifies a clr-namespace works in xaml with no assembly=
        /// </summary>
        public static void RunClrnsNoAssemblyTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            string xamlFile = DriverState.DriverParameters["TestParams"];
            XmlReader xmlReader = XmlReader.Create(xamlFile);
            XamlXmlReaderSettings settings = new XamlXmlReaderSettings();
            settings.LocalAssembly = XamlTestHelper.FindLoadedAssembly("XamlClrTypes");
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader, settings);

            // internal types only work in partial trust with certain permissions asserted
            // other tests should work with the default partial trust permissions
            XamlObjectWriter ow;
            if (xamlFile == "Clrns_NoAssembly_InternalTypes.xaml")
            {
                XamlObjectWriterSettings writerSettings = new XamlObjectWriterSettings
                {
                    AccessLevel = XamlAccessLevel.AssemblyAccessTo(settings.LocalAssembly)
                };

                ow = new XamlObjectWriter(xtr.SchemaContext, writerSettings);

                XamlServices.Transform(xtr, ow);
            }
            else
            {
                ow = new XamlObjectWriter(xtr.SchemaContext);
                XamlServices.Transform(xtr, ow);
            }

            object rootElement = ow.Result;
            switch (xamlFile)
            {
                case "Clrns_NoAssembly.xaml":
                    VerifyClrns_NoAssembly(rootElement);
                    break;

                case "Clrns_NoAssembly_InternalTypes.xaml":
                    VerifyClrns_NoAssembly_InternalTypes(rootElement);
                    break;

                default:
                    GlobalLog.LogEvidence("Unknown xaml file: " + xamlFile);
                    break;
            }
        }

        /// <summary>
        /// Verifies an exception is thrown when LocalAssembly is set
        /// while in partial trust
        /// </summary>
        public static void RunPartialTrustLocalAssemblyTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            string xamlFile = DriverState.DriverParameters["TestParams"];
            XmlReader xmlReader = XmlReader.Create(xamlFile);
            XamlXmlReaderSettings settings = new XamlXmlReaderSettings();
            ExceptionHelper.ExpectException(delegate { settings.LocalAssembly = XamlTestHelper.FindLoadedAssembly("XamlCommon"); }, new Exception());
        }

        /// <summary>
        /// Verifies the CLRNS_ no assembly.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyClrns_NoAssembly(object rootElement)
        {
            CustomRoot root = (CustomRoot) rootElement;
            Custom_Clr_StringID element1 = (Custom_Clr_StringID) root.Content;
            if (element1.ID != "Test1")
            {
                GlobalLog.LogEvidence("ID was not Test1");
                TestLog.Current.Result = TestResult.Fail;
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verifies the CLRNS_ no assembly_ internal types.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        private static void VerifyClrns_NoAssembly_InternalTypes(object rootElement)
        {
            CustomRoot root = (CustomRoot) rootElement;
            Custom_Internal element1 = (Custom_Internal) root.Content;
            if (element1.Name != "Test1")
            {
                GlobalLog.LogEvidence("Name was not Test1");
                TestLog.Current.Result = TestResult.Fail;
            }

            TestLog.Current.Result = TestResult.Pass;
        }
    }
}
