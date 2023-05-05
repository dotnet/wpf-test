// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Windows;
using System.Xaml;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.MethodTests
{
    /******************************************************************************
    * CLASS:          XamlSchemaExceptionTests
    ******************************************************************************/

    /// <summary>
    /// Class for XamlSchemaExceptionTests
    /// </summary>
    public class XamlSchemaExceptionTests : XamlTestType
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

            GlobalLog.LogStatus("TEST #1: Verify throwing XamlSchemaException, passing a Message parameter.");
            VerifyExceptionMessage();

            GlobalLog.LogStatus("TEST #2: Verify throwing XamlSchemaException, passing Message and InnerException parameters.");
            VerifyExceptionMessageAndInnerException();

            GlobalLog.LogStatus("TEST #3: Verify throwing XamlSchemaException, with no parameters.");
            VerifyExceptionNoParameters();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessage
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlSchemaException by throwing the exception, passing a Message parameter.
        /// </summary>
        private void VerifyExceptionMessage()
        {
            string expExceptionMessage = "message";

            try
            {
                throw new XamlSchemaException(expExceptionMessage);
            }
            catch (XamlSchemaException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException, null);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessageAndInnerException
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlSchemaException by throwing the exception, passing Message and InnerException parameters.
        /// </summary>
        private void VerifyExceptionMessageAndInnerException()
        {
            string expExceptionMessage = "message";
            string innerExceptionMessage = "innermessage";

            XamlSchemaException newException = new XamlSchemaException(innerExceptionMessage);

            try
            {
                throw new XamlSchemaException(expExceptionMessage, newException);
            }
            catch (XamlSchemaException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException.ToString(), "System.Xaml.XamlSchemaException: " + innerExceptionMessage);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionNoParameters
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlSchemaException by throwing the exception, with no parameters passed.
        /// </summary>
        private void VerifyExceptionNoParameters()
        {
            try
            {
                throw new XamlSchemaException();
            }
            catch (XamlSchemaException exception)
            {
                Assembly mscorlibAssembly = Assembly.GetAssembly(typeof(object));
                if (!Exceptions.CompareMessage(exception.Message, "Exception_WasThrown", mscorlibAssembly, "mscorlib"))
                {
                    throw new TestValidationException("Unexpected exception message");
                }

                Assert.AreEqual(exception.InnerException, null);
            }
        }
    }
}
