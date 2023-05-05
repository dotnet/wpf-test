// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xaml;
using Microsoft.Test;
using Microsoft.Test.Globalization;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    /// Tests for IXamlTypeResolver
    /// </summary>
    public static class IXamlTypeResolverTests
    {
        public static void RegressionIssue138()
        {
            string xaml = @"
<local:TypeHolder 
    xmlns:local=""clr-namespace:Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests;assembly=" + "XamlWpfTests40" +
    @""" xmlns:un=""clr-namespace:ConsoleApplication1;assembly=Unknown""
    local:TypeHolder.Type=""un:TypeHolder"" >
</local:TypeHolder>";
            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Parse(xaml), new XamlObjectWriterException(), "TypeConverterFailed", WpfBinaries.SystemXaml);
        }
    }

    /// <summary>
    /// Type used in test case
    /// </summary>
    public class TypeHolder
    {
        /// <summary>
        /// Gets or sets property of type Type used to invoke TypeTypeConverter which uses IXamlTypeResolver
        /// </summary>
        public Type Type { get; set; }
    }
}
