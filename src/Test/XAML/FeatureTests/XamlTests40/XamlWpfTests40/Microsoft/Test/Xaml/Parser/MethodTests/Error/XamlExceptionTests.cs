// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.MethodTests
{
    /******************************************************************************
    * CLASS:          XamlExceptionTests
    ******************************************************************************/

    /// <summary>
    /// Class for XamlExceptionTests
    /// </summary>
    public class XamlExceptionTests : XamlTestType
    {
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Verifies Xaml Exceptions.
        /// </summary>
        public override void Run()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;

            GlobalLog.LogStatus("TEST #1: Verify XamlException properties.");
            VerifyProperties();

            GlobalLog.LogStatus("TEST #2: Verify throwing XamlException, passing a Message parameter.");
            VerifyExceptionMessage();

            GlobalLog.LogStatus("TEST #3: Verify throwing XamlException, passing Message and InnerException parameters.");
            VerifyExceptionMessageAndInnerException();

            GlobalLog.LogStatus("TEST #4: Verify throwing XamlException, passing Message, InnerException, LineNumber, and LinePosition parameters.");
            VerifyLinePositionZeroAndLinePos();

            GlobalLog.LogStatus("TEST #5: Verify throwing XamlException, passing LinePosition of 0.");
            VerifyLinePositionZero();

            GlobalLog.LogStatus("TEST #6: Verify throwing XamlException, with no parameters.");
            VerifyNoParameters();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          VerifyProperties
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlException properties.
        /// </summary>
        private void VerifyProperties()
        {
            bool testPassed = true;

            string xamlFileName = DriverState.DriverParameters["XamlFileName"];

            if (String.IsNullOrEmpty(xamlFileName))
            {
                throw new TestSetupException("ERROR: xamlFileName cannot be null.");
            }

            if (!File.Exists(xamlFileName))
            {
                throw new TestSetupException("ERROR: the Xaml file specified does not exist.");
            }

            XmlReader xmlReader = XmlReader.Create(xamlFileName);
            XamlXmlReader reader = new XamlXmlReader(xmlReader);
            XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);

            try
            {
                XamlServices.Transform(reader, writer);

                GlobalLog.LogEvidence("FAIL: No exception was thrown.");
                testPassed = false;
            }
            catch (XamlException exception)
            {
                if (exception.LineNumber != 0)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect LineNumber");
                    GlobalLog.LogEvidence("Expected LineNumber:   0 / Actual LineNumber:   " + exception.LineNumber);
                    testPassed = false;
                }

                if (exception.LinePosition != 1)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect LinePosition");
                    GlobalLog.LogEvidence("Expected LinePosition:   1 / Actual LinePosition:   " + exception.LinePosition);
                    testPassed = false;
                }

                if (exception.InnerException != null)
                {
                    GlobalLog.LogEvidence("FAIL (TEST #1): Incorrect InnerException returned by the Exception. Expected: null" + " / Actual: " + exception.InnerException);
                    testPassed = false;
                }
            }
            catch (Exception ex)
            {
                GlobalLog.LogEvidence("FAIL: The wrong exception was thrown.\nExpected: XamlException / Actual: " + ex);
                testPassed = false;
            }

            if (!testPassed)
            {
                throw new TestValidationException("FAIL: Test #1 failed.");            
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessage
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlException by throwing the exception, passing Message.
        /// </summary>
        private void VerifyExceptionMessage()
        {
            string exceptionMessage = "message";

            try
            {
                throw new XamlException(exceptionMessage);
            }
            catch (XamlException exception)
            {
                Assert.AreEqual(exception.Message, exceptionMessage);
                Assert.AreEqual(exception.LineNumber, 0);
                Assert.AreEqual(exception.LinePosition, 0);
                Assert.AreEqual(exception.InnerException, null);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessageAndInnerException
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlException by throwing the exception, passing Message and InnerException parameters.
        /// </summary>
        private void VerifyExceptionMessageAndInnerException()
        {
            string exceptionMessage = "message";
            string innerExceptionMessage = "innermessage";

            XamlException newException = new XamlException(innerExceptionMessage);

            try
            {
                throw new XamlException(exceptionMessage, newException);
            }
            catch (XamlException exception)
            {
                Assert.AreEqual(exception.Message, exceptionMessage);
                Assert.AreEqual(exception.InnerException.ToString(), "System.Xaml.XamlException: " + innerExceptionMessage);
                Assert.AreEqual(exception.LineNumber, 0);
                Assert.AreEqual(exception.LinePosition, 0);
            }
        }

        /**********************************************************************************
        * Function:          VerifyLinePositionZeroAndLinePos
        **********************************************************************************/

        /// <summary>
        ///  Verifies throwing XamlException, passing Message, InnerException, LineNumber, and LinePosition parameters.
        /// </summary>
        private void VerifyLinePositionZeroAndLinePos()
        {
            string exceptionMessage = "message";
            string innerExceptionMessage = "innermessage";
            int lineNo = 2;
            int linePos = 5;

            XamlException newException = new XamlException(innerExceptionMessage);

            try
            {
                throw new XamlException(exceptionMessage, newException, lineNo, linePos);
            }
            catch (XamlException exception)
            {
                if (!exception.Message.Contains(lineNo.ToString()) || !exception.Message.Contains(linePos.ToString()))
                {
                    GlobalLog.LogEvidence("FAIL: LineNumber and/or LinePosition missing from Exception Message. Actual: " + exception.Message);
                    throw new TestValidationException("FAIL: Test #4 failed.");
                }

                Assert.AreEqual(exception.InnerException.ToString(), "System.Xaml.XamlException: " + innerExceptionMessage);
                Assert.AreEqual(exception.LineNumber, lineNo);
                Assert.AreEqual(exception.LinePosition, linePos);
            }
        }

        /**********************************************************************************
        * Function:          VerifyLinePositionZero
        **********************************************************************************/

        /// <summary>
        ///  Verifies throwing XamlException, passing LinePosition of 0.
        /// </summary>
        private void VerifyLinePositionZero()
        {
            string exceptionMessage = "message";
            string innerExceptionMessage = "innermessage";
            int lineNo = 100;
            int linePos = 0;

            XamlException newException = new XamlException(innerExceptionMessage);

            try
            {
                throw new XamlException(exceptionMessage, newException, lineNo, linePos);
            }
            catch (XamlException exception)
            {
                if (!exception.Message.Contains(lineNo.ToString()))
                {
                    GlobalLog.LogEvidence("FAIL: LineNumber and/or LinePosition missing from Exception Message. Actual: " + exception.Message);
                    throw new TestValidationException("FAIL: Test #5 failed.");
                }

                Assert.AreEqual(exception.InnerException.ToString(), "System.Xaml.XamlException: " + innerExceptionMessage);
                Assert.AreEqual(exception.LineNumber, lineNo);
                Assert.AreEqual(exception.LinePosition, linePos);
            }
        }

        /******************************************************************************
        * Function:          VerifyNoParameters
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlException by throwing the exception, with no parameters passed.
        /// </summary>
        private void VerifyNoParameters()
        {
            try
            {
                throw new XamlException();
            }
            catch (XamlException exception)
            {
                Assembly mscorlibAssembly = Assembly.GetAssembly(typeof(object));
                if (!Exceptions.CompareMessage(exception.Message, "Exception_WasThrown", mscorlibAssembly, "mscorlib"))
                {
                    throw new TestValidationException("Unexpected exception message");
                }

                Assert.AreEqual(exception.InnerException, null);
                Assert.AreEqual(exception.LineNumber, 0);
                Assert.AreEqual(exception.LinePosition, 0);
            }
        }
    }
}
