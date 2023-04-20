// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.MethodTests
{
    /******************************************************************************
    * CLASS:          XamlNamespaceExceptionTests
    ******************************************************************************/

    /// <summary>
    /// Class for XamlNamespaceExceptionTests
    /// </summary>
    public class XamlNamespaceExceptionTests : XamlTestType
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

            GlobalLog.LogStatus("TEST #1: Verify throwing XamlNamespaceException, passing a Message parameter.");
            VerifyExceptionMessage();

            GlobalLog.LogStatus("TEST #2: Verify throwing XamlNamespaceException, passing Message and InnerException parameters.");
            VerifyExceptionMessageAndInnerException();

            GlobalLog.LogStatus("TEST #3: Verify throwing XamlNamespaceException, passing Namespace and Message parameters.");
            VerifyExceptionNamespaceAndMessage();

            GlobalLog.LogStatus("TEST #4: Verify throwing XamlNamespaceException, passing Namespace, Message, and InnerException parameters.");
            VerifyExceptionNamespaceAndMessageAndInnerException();

            GlobalLog.LogStatus("TEST #5: Verify throwing XamlNamespaceException, with no parameters.");
            VerifyExceptionNoParameters();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessage
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlNamespaceException by throwing the exception, passing a Message parameter.
        /// </summary>
        private void VerifyExceptionMessage()
        {
            string expExceptionMessage = "message";

            try
            {
                throw new XamlNamespaceException(expExceptionMessage);
            }
            catch (XamlNamespaceException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessageAndInnerException
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlNamespaceException by throwing the exception, passing Message and InnerException parameters.
        /// </summary>
        private void VerifyExceptionMessageAndInnerException()
        {
            string expExceptionMessage = "message";
            string innerExceptionMessage = "innermessage";

            XamlNamespaceException newException = new XamlNamespaceException(innerExceptionMessage);

            try
            {
                throw new XamlNamespaceException(expExceptionMessage, newException);
            }
            catch (XamlNamespaceException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException.ToString(), "System.Xaml.XamlNamespaceException: " + innerExceptionMessage);

                if (exception.Namespace != null)
                {
                    GlobalLog.LogEvidence("FAIL (TEST #2): Incorrect Namespace returned by the Exception. Expected: '' / Actual: " + exception.Namespace);
                    throw new TestValidationException("FAIL: Test #2 failed.");
                }
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionNamespaceAndMessage
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlNamespaceException by throwing the exception, passing Namespace and Message parameters.
        /// </summary>
        private void VerifyExceptionNamespaceAndMessage()
        {
            string expNamespace = "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'";
            string expExceptionMessage = "message";
            try
            {
                throw new XamlNamespaceException(expNamespace, expExceptionMessage);
            }
            catch (XamlNamespaceException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException, null);

                if (exception.Namespace != expNamespace)
                {
                    GlobalLog.LogEvidence("FAIL (TEST #3): Incorrect Namespace returned by the Exception. Expected: " + expNamespace + " / Actual: " + exception.Namespace);
                    throw new TestValidationException("FAIL: Test #3 failed.");
                }
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionNamespaceAndMessageAndInnerException
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlNamespaceException by throwing the exception, passing Namespace, Message, and InnerException parameters.
        /// </summary>
        private void VerifyExceptionNamespaceAndMessageAndInnerException()
        {
            string expNamespace = "xmlns";
            string expExceptionMessage = "message";
            string innerExceptionMessage = "innermessage";

            XamlNamespaceException newException = new XamlNamespaceException(innerExceptionMessage);

            try
            {
                throw new XamlNamespaceException(expNamespace, expExceptionMessage, newException);
            }
            catch (XamlNamespaceException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException.ToString(), "System.Xaml.XamlNamespaceException: " + innerExceptionMessage);

                if (exception.Namespace != expNamespace)
                {
                    GlobalLog.LogEvidence("FAIL (TEST #4): Incorrect Namespace returned by the Exception. Expected: " + expNamespace + " / Actual: " + exception.Namespace);
                    throw new TestValidationException("FAIL: Test #4 failed.");
                }
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionNoParameters
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlNamespaceException by throwing the exception, with no parameters passed.
        /// </summary>
        private void VerifyExceptionNoParameters()
        {
            string expNamespace = null;
            string expExceptionMessage = "Exception of type 'System.Xaml.XamlNamespaceException' was thrown.";

            try
            {
                throw new XamlNamespaceException();
            }
            catch (XamlNamespaceException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException, null);

                if (exception.Namespace != expNamespace)
                {
                    GlobalLog.LogEvidence("FAIL (TEST #5): Incorrect Namespace returned by the Exception. Expected: " + expNamespace + " / Actual: " + exception.Namespace);
                    throw new TestValidationException("FAIL: Test #5 failed.");
                }
            }
        }
    }
}
