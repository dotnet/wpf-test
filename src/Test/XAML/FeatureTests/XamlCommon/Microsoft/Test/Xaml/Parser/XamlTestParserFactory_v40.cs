// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser
{
    /// <summary>
    /// Factory class for IXamlTestParser classes
    /// </summary>
    public static class XamlTestParserFactory
    {
        /// <summary>
        /// Returns an IXamlTestParser based on the Mode driver parameter for the test
        /// If Mode is null, it defaults to the WPFXamlParser
        /// </summary>
        /// <returns>IXamlTestParser value</returns>
        public static IXamlTestParser Create()
        {
            if (!String.IsNullOrEmpty(DriverState.DriverParameters["Mode"]))
            {
                switch (DriverState.DriverParameters["Mode"].ToLowerInvariant())
                {
                    case "wpf":
                        return new WPFXamlParser();
                    case "sdx":
                        return new SDXParser();
                    default:
                        throw new InvalidOperationException(DriverState.DriverParameters["Mode"] + "is invalid");
                }
            }
            else
            {
                GlobalLog.LogStatus("Mode parameter not found defaulting to WPFXamlParser");
                return new WPFXamlParser();
            }
        }
    }
}
