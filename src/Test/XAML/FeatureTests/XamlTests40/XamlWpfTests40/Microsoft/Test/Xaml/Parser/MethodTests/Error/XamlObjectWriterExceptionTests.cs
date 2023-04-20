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
    * CLASS:          XamlObjectWriterExceptionTests
    ******************************************************************************/

    /// <summary>
    /// Class for XamlObjectWriterExceptionTests
    /// </summary>
    public class XamlObjectWriterExceptionTests : XamlTestType
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

            GlobalLog.LogStatus("TEST #1: Verify throwing XamlObjectWriterException, passing a Message parameter.");
            VerifyExceptionMessage();

            GlobalLog.LogStatus("TEST #2: Verify throwing XamlObjectWriterException, passing Message and InnerException parameters.");
            VerifyExceptionMessageAndInnerException();

            GlobalLog.LogStatus("TEST #3: Verify throwing XamlObjectWriterException, with no parameters.");
            VerifyExceptionNoParameters();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessage
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlObjectWriterException by throwing the exception, passing a Message parameter.
        /// </summary>
        private void VerifyExceptionMessage()
        {
            string expExceptionMessage = "message";

            try
            {
                throw new XamlObjectWriterException(expExceptionMessage);
            }
            catch (XamlObjectWriterException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException, null);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessageAndInnerException
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlObjectWriterException by throwing the exception, passing Message and InnerException parameters.
        /// </summary>
        private void VerifyExceptionMessageAndInnerException()
        {
            string expExceptionMessage = "message";
            string innerExceptionMessage = "innermessage";

            XamlObjectWriterException newException = new XamlObjectWriterException(innerExceptionMessage);

            try
            {
                throw new XamlObjectWriterException(expExceptionMessage, newException);
            }
            catch (XamlObjectWriterException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException.ToString(), "System.Xaml.XamlObjectWriterException: " + innerExceptionMessage);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionNoParameters
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlObjectWriterException by throwing the exception, with no parameters passed.
        /// </summary>
        private void VerifyExceptionNoParameters()
        {
            try
            {
                throw new XamlObjectWriterException();
            }
            catch (XamlObjectWriterException exception)
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
