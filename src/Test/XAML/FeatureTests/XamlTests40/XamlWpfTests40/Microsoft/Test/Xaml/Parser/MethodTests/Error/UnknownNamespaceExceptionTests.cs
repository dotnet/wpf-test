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
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Error
{
    /******************************************************************************
    * CLASS:          UnknownNamespaceExceptionTests
    ******************************************************************************/

    /// <summary>
    /// Class for UnknownNamespaceExceptionTests.
    /// Regression test
    /// </summary>
    public class UnknownNamespaceExceptionTests : XamlTestType
    {
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Verifies Xaml Exceptions.
        /// </summary>
        public override void Run()
        {
            GlobalLog.LogStatus("TEST: Verify an unknown Namespace.");
            VerifyUnknonwNS();

            TestLog.Current.Result = TestResult.Pass;
        }

        /******************************************************************************
        * Function:          VerifyExceptionMessageAndInnerException
        ******************************************************************************/

        /// <summary>
        ///  Verifies that a XamlObjectWriterException is thrown given an unknown Namespace.
        /// </summary>
        private void VerifyUnknonwNS()
        {
           string xaml = @"
                <local:TypeHolder 
                 xmlns:local=""clr-namespace:XamlTestsDev10;assembly=XamlWpfTests40""
                 xmlns:un=""clr-namespace:XamlTestsDev10;assembly=Unknown""
                 local:TypeHolder.Type=""un:TypeHolder"" >
                </local:TypeHolder>";

            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Parse(xaml), new System.Xaml.XamlObjectWriterException("TypeNotFound"));
        }
    }

    /******************************************************************************
    * CLASS:          TypeHolder
    ******************************************************************************/

    /// <summary>
    ///  Class for testing an Unknown Namespace.
    /// </summary>
    public class TypeHolder
    {
        /// <summary>Gets or sets a simple type used for testing an unknown Namspce</summary>
        public Type Type { get; set; }
    }
}
