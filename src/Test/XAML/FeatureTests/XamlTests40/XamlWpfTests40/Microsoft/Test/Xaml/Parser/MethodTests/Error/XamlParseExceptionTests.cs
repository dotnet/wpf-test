// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
    * CLASS:          XamlParseExceptionTests
    ******************************************************************************/

    /// <summary>
    /// Class for XamlParseExceptionTests
    /// </summary>
    public class XamlParseExceptionTests : XamlTestType
    {
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Verifies XamlParseExceptions.
        /// </summary>
        public override void Run()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;

            GlobalLog.LogStatus("TEST #1: Verify throwing XamlParseException, passing a Message parameter.");
            VerifyExceptionMessage();

            GlobalLog.LogStatus("TEST #2: Verify throwing XamlParseException, passing Message and InnerException parameters.");
            VerifyExceptionMessageAndInnerException();

            GlobalLog.LogStatus("TEST #3: Verify throwing XamlParseException, with no parameters.");
            VerifyExceptionNoParameters();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessage
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlParseException by throwing the exception, passing a Message parameter.
        /// </summary>
        private void VerifyExceptionMessage()
        {
            string expExceptionMessage = "message";

            try
            {
                throw new XamlParseException(expExceptionMessage);
            }
            catch (XamlParseException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException, null);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessageAndInnerException
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlParseException by throwing the exception, passing Message and InnerException parameters.
        /// </summary>
        private void VerifyExceptionMessageAndInnerException()
        {
            string expExceptionMessage = "message";
            string innerExceptionMessage = "innermessage";

            XamlParseException newException = new XamlParseException(innerExceptionMessage);

            try
            {
                throw new XamlParseException(expExceptionMessage, newException);
            }
            catch (XamlParseException exception)
            {
                Assert.AreEqual(exception.Message, expExceptionMessage);
                Assert.AreEqual(exception.InnerException.ToString(), "System.Xaml.XamlParseException: " + innerExceptionMessage);
            }
        }

        /******************************************************************************
        * Function:          VerifyExceptionNoParameters
        ******************************************************************************/

        /// <summary>
        ///  Verifies XamlParseException by throwing the exception, with no parameters passed.
        /// </summary>
        private void VerifyExceptionNoParameters()
        {
            try
            {
                throw new XamlParseException();
            }
            catch (XamlParseException exception)
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

    /// <summary>
    /// XamlParseExceptionMethod Tests 
    /// </summary>
    public class XamlParseExceptionMethodTests
    {
        /// <summary>
        /// ExceptionFromTemplate test.
        /// </summary>
        public void ExceptionFromTemplate()
        {
            string xaml = @"
                <Window xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                        xmlns:local='clr-namespace:Microsoft.Test.Xaml.MethodTests'>
                    <Button>
                        <Button.Template>
                            <ControlTemplate>
                                <local:Class1/>
                            </ControlTemplate>
                        </Button.Template>
                    </Button>
                </Window>";

            ExceptionHelper.ExpectException(
                delegate
                {
                    Window window = System.Windows.Markup.XamlReader.Parse(xaml) as Window;
                    window.Show();
                },
                new System.Windows.Markup.XamlParseException());
        }
    }

    /// <summary>
    /// Type for ExceptionFromTemplate test.
    /// </summary>
    public class Class1 : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the Class1 class.
        /// </summary>
        public Class1()
        {
            throw new Exception("Dummy Exception");
        }
    }
}
