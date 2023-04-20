// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xaml;
using Microsoft.Test;

namespace Microsoft.Test.Xaml.InputValidation
{
    /// <summary>
    /// Tests for input validation of XamlObjectWriter members
    /// </summary>
    public class XamlObjectWriterTests
    {
        /// <summary>
        /// Tests constructor inputs
        /// </summary>
        public void ConstructorTest()
        {
            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();
            XamlObjectWriterSettings xamlObjectWriterSettings = new XamlObjectWriterSettings();

            ExceptionHelper.ExpectException<ArgumentNullException>(() => new XamlObjectWriter((XamlSchemaContext)null), new ArgumentNullException("schemaContext"));

            ExceptionHelper.ExpectException<ArgumentNullException>(() => new XamlObjectWriter((XamlSchemaContext)null, xamlObjectWriterSettings), new ArgumentNullException("schemaContext"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => new XamlObjectWriter((XamlSchemaContext)null, null), new ArgumentNullException("schemaContext"));
        }
    }
}
