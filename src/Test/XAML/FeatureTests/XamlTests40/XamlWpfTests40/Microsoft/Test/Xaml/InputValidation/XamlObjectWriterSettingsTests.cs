// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xaml;
using Microsoft.Test;

namespace Microsoft.Test.Xaml.InputValidation
{
    /// <summary>
    /// Tests for input validation of XamlObjectWriterSettings members
    /// </summary>
    public class XamlObjectWriterSettingsTests
    {
        /// <summary>
        /// Tests constructor inputs
        /// </summary>
        public void ConstructorTest()
        {
            ExceptionHelper.ExpectException<ArgumentNullException>(() => new XamlObjectWriterSettings(null), new ArgumentNullException("settings"));
        }
    }
}
