// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Error
{
    /******************************************************************************
    * CLASS:          BadAttachedPropertyTest
    ******************************************************************************/

    /// <summary>
    /// Class for testing an expected error when an inappropriate attached property is specified in markup.
    /// </summary>
    public class BadAttachedPropertyTest : XamlTestType
    {
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Verifies Xaml Exceptions.
        /// </summary>
        public override void Run()
        {
            GlobalLog.LogStatus("TEST: Verify the exception thrown for an inappropriate attached property.");

            FrameworkElement fe = new FrameworkElement();
            fe = null;

            string xamlFileName = DriverState.DriverParameters["TestParams"];

            XmlReader xmlReader = XmlReader.Create(xamlFileName);
            ExceptionHelper.ExpectException<XamlParseException>(delegate { XamlReader.Load(xmlReader); }, ValidateException);

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Validates the exception thrown
        /// </summary>
        /// <param name="exception">exception to validate</param>
        public void ValidateException(XamlParseException exception)
        {
            string expected = string.Format(Exceptions.GetMessage("LineNumberAndPosition", WpfBinaries.SystemXaml), Exceptions.GetMessage("CantSetUnknownProperty", WpfBinaries.SystemXaml), 5, 13);
            if (!Exceptions.CompareMessage(exception.Message, expected))
            {
                throw new TestValidationException("Expected Message: " + expected + ", Actual Message: " + exception.Message);
            }
        }
    }
}
