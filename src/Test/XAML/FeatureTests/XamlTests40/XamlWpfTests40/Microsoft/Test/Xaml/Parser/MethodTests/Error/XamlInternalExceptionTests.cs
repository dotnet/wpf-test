// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Xaml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.MethodTests
{
    /******************************************************************************
    * CLASS:          XamlInternalExceptionTests
    ******************************************************************************/

    /// <summary>
    /// Class for XamlInternalExceptionTests
    /// </summary>
    public class XamlInternalExceptionTests : XamlTestType
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

            GlobalLog.LogStatus("TEST #1: Verify throwing XamlInternalException, passing a Message parameter.");
            VerifyExceptionMessage();

            GlobalLog.LogStatus("TEST #2: Verify throwing XamlInternalException, passing Message and InnerException parameters.");
            VerifyExceptionMessageAndInnerException();

            GlobalLog.LogStatus("TEST #3: Verify throwing XamlInternalException, with no parameters.");
            VerifyExceptionNoParameters();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessage
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlInternalException by throwing the exception, passing a Message parameter.
        /// </summary>
        private void VerifyExceptionMessage()
        {
            string exceptionMessage = "message";

            try
            {
                throw new XamlInternalException(exceptionMessage);
            }
            catch (XamlInternalException exception)
            {
                Assert.AreEqual(exception.Message, "Internal XAML system error: " + exceptionMessage);
                Assert.AreEqual(exception.InnerException, null);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessageAndInnerException
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlInternalException by throwing the exception, passing Message and InnerException parameters.
        /// </summary>
        private void VerifyExceptionMessageAndInnerException()
        {
            string exceptionMessage = "message";
            string innerExceptionMessage = "innermessage";

            XamlInternalException newException = new XamlInternalException(innerExceptionMessage);

            try
            {
                throw new XamlInternalException(exceptionMessage, newException);
            }
            catch (XamlInternalException exception)
            {
                Assert.AreEqual(exception.Message, "Internal XAML system error: " + exceptionMessage);
                Assert.AreEqual(exception.InnerException.ToString(), "System.Xaml.XamlInternalException: Internal XAML system error: " + innerExceptionMessage);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionNoParameters
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlInternalException by throwing the exception, with no parameters passed.
        /// </summary>
        private void VerifyExceptionNoParameters()
        {
            string expExceptionMessage = "Internal XAML system error: ";

            try
            {
                throw new XamlInternalException();
            }
            catch (XamlInternalException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException, null);
            }
        }
    }
}
