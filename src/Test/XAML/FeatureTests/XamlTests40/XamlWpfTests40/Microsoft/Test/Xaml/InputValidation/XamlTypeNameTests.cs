// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xaml;
using System.Xaml.Schema;
using Microsoft.Test;

namespace Microsoft.Test.Xaml.InputValidation
{
    /// <summary>
    /// Tests for input validation of XamlTypeName members
    /// </summary>
    public class XamlTypeNameTests
    {
        /// <summary>
        /// Tests constructor inputs
        /// </summary>
        public void ConstructorTest()
        {
            ExceptionHelper.ExpectException<ArgumentNullException>(() => new XamlTypeName(null), new ArgumentNullException("xamlType"));
        }
    }
}
