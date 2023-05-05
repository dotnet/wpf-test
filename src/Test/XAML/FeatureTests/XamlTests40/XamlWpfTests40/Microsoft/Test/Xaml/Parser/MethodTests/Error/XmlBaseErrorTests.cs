// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xaml;
using System.Xaml.Schema;
using System.Xml;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Error
{
    /******************************************************************************
    * Class:          XmlBaseErrorTests
    ******************************************************************************/

    /// <summary>
    /// xml:base tests (Regression test)
    /// </summary>
    public class XmlBaseErrorTests : XamlTestType
    {
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Verifies Xaml Exceptions.
        /// </summary>
        public override void Run()
        {
            GlobalLog.LogEvidence("TEST #1: Verify xmlBase on child.");
            VerifyOnChild();

            GlobalLog.LogEvidence("TEST #2: Verify xmlBase on root and child.");
            VerifyOnRootAndChild();

            GlobalLog.LogEvidence("TEST #3: Verify xmlBase in markup and in code via XamlXmlReaderSettings.");
            VerifyWithSettings();

            GlobalLog.LogEvidence("TEST #4: Verify xmlBase via XamlServices.Transform - String.");
            VerifyWithXamlServicesTransformString();

            GlobalLog.LogEvidence("TEST #5: Verify loading via XamlServices.Load(fileName).");
            VerifyWithXamlServicesLoad();

            GlobalLog.LogEvidence("TEST #6: Verify loading via XamlServices.Transform - FileName.");
            VerifyWithXamlServicesTransformFileName();

            GlobalLog.LogEvidence("TEST #7: Verify loading via XamlServices.Transform - FileStream.");
            VerifyWithXamlServicesTransformFileStream();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          TEST 1: VerifyOnChild
        ******************************************************************************/

        /// <summary>
        ///  Verifies the exception thrown when xml:base is specified on a child element in markup.
        /// </summary>
        private void VerifyOnChild()
        {
            string xamlString = @"
            <ObjectContainer xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'>
                <ObjectContainer.Content>
                    <Bar xml:base='http://foo' />
                </ObjectContainer.Content>
            </ObjectContainer>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            XamlXmlReader reader = new XamlXmlReader(xmlReader, schemaContext);
            XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);

            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Transform(reader, writer), new XamlObjectWriterException(), "CannotSetBaseUri", WpfBinaries.SystemXaml);
        }

        /******************************************************************************
        * Function:          TEST 2: VerifyOnRootAndChild
        ******************************************************************************/

        /// <summary>
        ///  Verifies the exception thrown when xml:base is specified on both root and child elements in markup.
        /// </summary>
        private void VerifyOnRootAndChild()
        {
            string xamlString = @"
            <ObjectContainer xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes' xml:base='http://foo'>
                <ObjectContainer.Content>
                    <Bar xml:base='http://foo' />
                </ObjectContainer.Content>
            </ObjectContainer>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            XamlXmlReader reader = new XamlXmlReader(xmlReader, schemaContext);
            XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);

            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Transform(reader, writer), new XamlObjectWriterException(), "CannotSetBaseUri", WpfBinaries.SystemXaml);
        }

        /******************************************************************************
        * Function:          TEST 3: VerifyWithSettings
        ******************************************************************************/

        /// <summary>
        ///  Verifies the exception thrown when xml:base is specified in both markup (on the root) and code.
        /// </summary>
        private void VerifyWithSettings()
        {
            string xamlString = @"
            <ObjectContainer xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes' xml:base='http://foo'>
                <ObjectContainer.Content>
                    <Bar />
                </ObjectContainer.Content>
            </ObjectContainer>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            XamlXmlReaderSettings settings = new XamlXmlReaderSettings();
            settings.BaseUri = new Uri("file:///E:/Pics/A/");
            XamlXmlReader reader = new XamlXmlReader(xmlReader, schemaContext, settings);
            XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);

            string xamlNS = "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes";
            XamlTypeName xamlTypeName = new XamlTypeName(xamlNS, "ObjectContainer");
            XamlSchemaContext xSC = new XamlSchemaContext();
            XamlType xamlType = xSC.GetXamlType(xamlTypeName);
            XamlMember xamlMember = XamlLanguage.Base;

            XamlDuplicateMemberException duplicateException = new XamlDuplicateMemberException(xamlMember, xamlType);
            ExceptionHelper.ExpectException<XamlDuplicateMemberException>(() => XamlServices.Transform(reader, writer), duplicateException, "DuplicateMemberSet", WpfBinaries.SystemXaml);
        }

        /******************************************************************************
        * Function:          TEST 4: VerifyWithXamlServicesTransformString
        ******************************************************************************/

        /// <summary>
        ///  Verifies the exception thrown XamlServices.Transform is called, with a reader based on a string.
        /// </summary>
        private void VerifyWithXamlServicesTransformString()
        {
            string xamlString = @"
            <ObjectContainer xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes' xml:base='http://foo'>
                <ObjectContainer.Content>
                    <Bar />
                </ObjectContainer.Content>
            </ObjectContainer>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            XamlXmlReader reader = new XamlXmlReader(xmlReader, schemaContext);
            XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);
            XamlServices.Transform(reader, writer);

            if (writer.Result == null)
            {
                throw new Microsoft.Test.TestValidationException("XamlObjectWriter returned a null result.");
            }
        }

        /******************************************************************************
        * Function:          TEST 5: VerifyWithXamlServicesLoad
        ******************************************************************************/

        /// <summary>
        ///  Verifies the exception thrown when XamlServices.Load(fileName) is executed.
        /// </summary>
        private void VerifyWithXamlServicesLoad()
        {
            string fileName = GetFileName();

            string xamlNS = "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes";
            XamlTypeName xamlTypeName = new XamlTypeName(xamlNS, "ObjectContainer");
            XamlSchemaContext xSC = new XamlSchemaContext();
            XamlType xamlType = xSC.GetXamlType(xamlTypeName);
            XamlMember xamlMember = XamlLanguage.Base;

            XamlDuplicateMemberException duplicateException = new XamlDuplicateMemberException(xamlMember, xamlType);
            ExceptionHelper.ExpectException<XamlDuplicateMemberException>(() => XamlServices.Load(fileName), duplicateException, "DuplicateMemberSet", WpfBinaries.SystemXaml);
        }

        /******************************************************************************
        * Function:          TEST 6: VerifyWithXamlServicesTransform - FileName
        ******************************************************************************/

        /// <summary>
        ///  Verifies xml:base on the root when XamlServices.Transform() is executed.
        /// </summary>
        private void VerifyWithXamlServicesTransformFileName()
        {
            string fileName = GetFileName();

            XmlReader xmlReader = XmlReader.Create(fileName);
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            XamlXmlReader reader = new XamlXmlReader(xmlReader, schemaContext);
            XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);

            string xamlNS = "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes";
            XamlTypeName xamlTypeName = new XamlTypeName(xamlNS, "ObjectContainer");
            XamlSchemaContext xSC = new XamlSchemaContext();
            XamlType xamlType = xSC.GetXamlType(xamlTypeName);
            XamlMember xamlMember = XamlLanguage.Base;

            XamlDuplicateMemberException duplicateException = new XamlDuplicateMemberException(xamlMember, xamlType);
            ExceptionHelper.ExpectException<XamlDuplicateMemberException>(() => XamlServices.Transform(reader, writer), duplicateException, "DuplicateMemberSet", WpfBinaries.SystemXaml);
        }

        /******************************************************************************
        * Function:          TEST 7: VerifyWithXamlServicesTransform-FileStream
        ******************************************************************************/

        /// <summary>
        ///  Verifies xml:base on the root when XamlServices.Transform() is executed, when the .xaml file is loaded
        ///  via a FileStream.
        /// </summary>
        private void VerifyWithXamlServicesTransformFileStream()
        {
            string fileName = GetFileName();

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            XmlReader xmlReader = XmlReader.Create(fileStream);
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            XamlXmlReader reader = new XamlXmlReader(xmlReader, schemaContext);
            XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);
            XamlServices.Transform(reader, writer);

            if (writer.Result == null)
            {
                throw new Microsoft.Test.TestValidationException("XamlObjectWriter returned a null result.");
            }
        }

        /******************************************************************************
        * Function:          GetFileName
        ******************************************************************************/

        /// <summary>
        ///  Retrieve the name of the .xaml file to be tested.
        /// </summary>
        /// <returns>The requested file name.</returns>
        private string GetFileName()
        {
            string xamlFileName = DriverState.DriverParameters["XamlFileName"];

            if (String.IsNullOrEmpty(xamlFileName))
            {
                throw new TestSetupException("ERROR: xamlFileName cannot be null.");
            }

            if (!File.Exists(xamlFileName))
            {
                throw new TestSetupException("ERROR: the Xaml file specified does not exist.");
            }

            return xamlFileName;
        }
    }
}
