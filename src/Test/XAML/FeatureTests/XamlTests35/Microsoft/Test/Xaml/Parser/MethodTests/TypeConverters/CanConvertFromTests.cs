// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.TypeConverters
{

    /// <summary>
    /// CultureInfoIetfLanguageTagConverter Test
    /// </summary> 
    public class CultureInfoIetfLanguageTagConverterTest
    {
        /// <summary>
        /// CultureInfoIetfLanguageTagConverter CanConvertFrom Test
        /// </summary>
        [Test(2, @"Parser", TestCaseSecurityLevel.PartialTrust, "CultureInfoIetfLanguageTagConverter.CanConvertFrom", Area = "XAML")]
        public void VerifyLanguageTagConvertFrom()
        {
            CultureInfoIetfLanguageTagConverter c = new CultureInfoIetfLanguageTagConverter();

            if (!c.CanConvertFrom(null,typeof(string)))
            {
                throw new Microsoft.Test.TestValidationException("CultureInfoIetfLanguageTagConverter.CanConvertFrom must convert from string");
            }

            if (c.CanConvertFrom(null,typeof(object)))
            {
                throw new Microsoft.Test.TestValidationException("CultureInfoIetfLanguageTagConverter.CanConvertFrom must convert from object");
            }

            return;
        }
    }
}
