// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;

namespace Microsoft.Test.Xaml.Parser.MethodTests
{
    /******************************************************************************
    * CLASS:          XamlBackgroundReaderExceptions
    ******************************************************************************/

    /// <summary>
    /// Class for XamlBackgroundReaderExceptions
    /// </summary>
    public class XamlBackgroundReaderExceptions : XamlTestType
    {
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Verifies XamlBackgroundReader features.
        /// </summary>
        public override void Run()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;

            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();

            XamlXmlReaderSettings xxrSettings = new XamlXmlReaderSettings();
            xxrSettings.ProvideLineInfo = true;

            GlobalLog.LogStatus("TEST #1: Verify passing a null reader to a XamlBackgroundReader.");
            VerifyNullReader();

            GlobalLog.LogStatus("TEST #2: Verify an exception is thrown when a Thread is already started.");
            VerifyThreadAlreadyStarted(xamlSchemaContext, xxrSettings);

            GlobalLog.LogStatus("TEST #3: Verify an exception on the background thread is rethrown.");
            VerifyException(xamlSchemaContext, xxrSettings);

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          VerifyNullReader
        ******************************************************************************/

        /// <summary>
        /// Verifies the Exception produced when passing null to the XamlBackgroundReader constructor.
        /// </summary>
        private void VerifyNullReader()
        {
            bool test3Passed = true;

            try
            {
                XamlBackgroundReader xamlBackgroundReader3 = new XamlBackgroundReader(null);
                test3Passed = false;
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ArgumentNullException))
                {
                    GlobalLog.LogEvidence("FAIL: Test #1 failed. Expected: " + typeof(ArgumentNullException) + " / Actual: " + ex.GetType());
                    GlobalLog.LogEvidence("Actual Exception Message: " + ex.Message + "\n\n");
                    test3Passed = false;
                }
            }

            if (!test3Passed)
            {
                throw new TestValidationException("FAIL: Test #1 failed.  The expected Exception did not occur.");
            }
        }

        /******************************************************************************
        * Function:          VerifyThreadAlreadyStarted
        ******************************************************************************/

        /// <summary>
        /// Verifies that an InvalidOperationException is thrown when a thread is already started.
        /// </summary>
        /// <param name="xamlSchemaContext">A xamlSchemaContext to be passed to XamlXamlReader.</param>
        /// <param name="xxrSettings">A XamlXmlReaderSettings to be passed to XamlXamlReader.</param>
        private void VerifyThreadAlreadyStarted(XamlSchemaContext xamlSchemaContext, XamlXmlReaderSettings xxrSettings)
        {
            bool test4Passed = true;

            string xamlFile = DriverState.DriverParameters["XamlFile"] + ".xaml";
            XamlXmlReader xamlXmlReader = new XamlXmlReader(XmlReader.Create(xamlFile), xamlSchemaContext, xxrSettings);

            XamlBackgroundReader xbr = new XamlBackgroundReader(xamlXmlReader);
            xbr.StartThread();
            XamlObjectWriter xow = new XamlObjectWriter(xbr.SchemaContext);
            XamlServices.Transform(xbr, xow);

            try
            {
                xbr.StartThread();
                test4Passed = false;
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(InvalidOperationException))
                {
                    GlobalLog.LogEvidence("FAIL: Test #2 failed. Expected: " + typeof(InvalidOperationException) + " / Actual: " + ex.GetType());
                    GlobalLog.LogEvidence("Actual Exception Message: " + ex.Message + "\n\n");
                    test4Passed = false;
                }
            }

            if (!test4Passed)
            {
                throw new TestValidationException("FAIL: Test #2 failed.  The expected Exception did not occur.");
            }
        }

        /******************************************************************************
        * Function:          VerifyException
        ******************************************************************************/

        /// <summary>
        /// Verifies that an Exception occuring in XamlBackgroundReader is rethrown.
        /// </summary>
        /// <param name="xamlSchemaContext">A xamlSchemaContext to be passed to XamlXamlReader.</param>
        /// <param name="xxrSettings">A XamlXmlReaderSettings to be passed to XamlXamlReader.</param>
        private void VerifyException(XamlSchemaContext xamlSchemaContext, XamlXmlReaderSettings xxrSettings)
        {
            bool test5Passed = true;

            string xamlString = @"
                <DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'> 
                    <Button Content='{' />
                </DockPanel>";

            XamlXmlReader xamlXmlReader = new XamlXmlReader(XmlReader.Create(new StringReader(xamlString)), xamlSchemaContext, xxrSettings);
            XamlBackgroundReader xbr = new XamlBackgroundReader(xamlXmlReader);
            xbr.StartThread();
            XamlObjectWriter xow = new XamlObjectWriter(xbr.SchemaContext);

            try
            {
                XamlServices.Transform(xbr, xow);
                test5Passed = false;
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(XamlParseException))
                {
                    GlobalLog.LogEvidence("FAIL: Test #3 failed. Expected: " + typeof(XamlParseException) + " / Actual: " + ex.GetType());
                    GlobalLog.LogEvidence("Actual Exception Message: " + ex.Message + "\n\n");
                    test5Passed = false;
                }
            }

            if (!test5Passed)
            {
                throw new TestValidationException("FAIL: Test #3 failed.  The expected Exception did not occur.");
            }
        }
    }
}
