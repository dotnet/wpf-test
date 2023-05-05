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
    * CLASS:          XamlDuplicateMemberExceptionTests
    ******************************************************************************/

    /// <summary>
    /// Class for XamlDuplicateMemberExceptionTests
    /// </summary>
    public class XamlDuplicateMemberExceptionTests : XamlTestType
    {
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Verifies XamlDuplicateMemberExceptions.
        /// </summary>
        public override void Run()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;

            GlobalLog.LogStatus("TEST #1: Verify throwing XamlDuplicateMemberException, passing a Message parameter.");
            VerifyExceptionMessage();

            GlobalLog.LogStatus("TEST #2: Verify throwing XamlDuplicateMemberException, passing Message and InnerException parameters.");
            VerifyExceptionMessageAndInnerException();

            GlobalLog.LogStatus("TEST #3: Verify throwing XamlDuplicateMemberException, with no parameters.");
            VerifyExceptionNoParameters();

            GlobalLog.LogStatus("TEST #4: Verify throwing XamlDuplicateMemberException, with null parameters.");
            VerifyExceptionNullParameters();

            GlobalLog.LogStatus("TEST #5: Verify throwing XamlDuplicateMemberException, with valid parameters.");
            VerifyExceptionValidParameters();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessage
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlDuplicateMemberException by throwing the exception, passing a Message parameter.
        /// </summary>
        private void VerifyExceptionMessage()
        {
            string expExceptionMessage = "message";

            try
            {
                throw new XamlDuplicateMemberException(expExceptionMessage);
            }
            catch (XamlDuplicateMemberException exception)
            {
                Assert.AreEqual(expExceptionMessage, exception.Message);
                Assert.AreEqual(null, exception.InnerException);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessageAndInnerException
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlDuplicateMemberException by throwing the exception, passing Message and InnerException parameters.
        /// </summary>
        private void VerifyExceptionMessageAndInnerException()
        {
            string expExceptionMessage = "message";
            string innerExceptionMessage = "innermessage";

            XamlDuplicateMemberException newException = new XamlDuplicateMemberException(innerExceptionMessage);

            try
            {
                throw new XamlDuplicateMemberException(expExceptionMessage, newException);
            }
            catch (XamlDuplicateMemberException exception)
            {
                Assert.AreEqual(expExceptionMessage, exception.Message);
                Assert.AreEqual("System.Xaml.XamlDuplicateMemberException: " + innerExceptionMessage, exception.InnerException.ToString());
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionNoParameters
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlDuplicateMemberException by throwing the exception, with no parameters passed.
        /// </summary>
        private void VerifyExceptionNoParameters()
        {
            try
            {
                throw new XamlDuplicateMemberException();
            }
            catch (XamlDuplicateMemberException exception)
            {
                Assembly mscorlibAssembly = Assembly.GetAssembly(typeof(object));
                if (!Exceptions.CompareMessage(exception.Message, "Exception_WasThrown", mscorlibAssembly, "mscorlib"))
                {
                    throw new TestValidationException("Unexpected exception message");
                }

                Assert.AreEqual(null, exception.InnerException);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionNullParameters
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlDuplicateMemberException by throwing the exception, with null parameters passed.
        /// </summary>
        private void VerifyExceptionNullParameters()
        {
            try
            {
                throw new XamlDuplicateMemberException((XamlMember)null, (XamlType)null);
            }
            catch (XamlDuplicateMemberException exception)
            {
                if (!Exceptions.CompareMessage(exception.Message, "DuplicateMemberSet", WpfBinaries.SystemXaml))
                {
                    throw new TestValidationException("Unexpected exception message");
                }

                Assert.AreEqual(null, exception.InnerException);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionValidParameters
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlDuplicateMemberException by throwing the exception, with valid parameters passed.
        /// </summary>
        private void VerifyExceptionValidParameters()
        {
            try
            {
                XamlSchemaContext xSC = new XamlSchemaContext();
                string xNS = @"http://XamlTestTypes";
                XamlType xamlType = xSC.GetXamlType(xNS, "CustomRoot");
                XamlMember xamlMember = xamlType.GetMember("Name");

                throw new XamlDuplicateMemberException(xamlMember, xamlType);
            }
            catch (XamlDuplicateMemberException exception)
            {
                if (!Exceptions.CompareMessage(exception.Message, "DuplicateMemberSet", WpfBinaries.SystemXaml))
                {
                    throw new TestValidationException("Unexpected exception message");
                }

                Assert.AreEqual(null, exception.InnerException);
            }
        }
    }
}
