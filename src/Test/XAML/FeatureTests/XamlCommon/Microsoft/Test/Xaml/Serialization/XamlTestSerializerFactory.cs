// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Serialization;

namespace Microsoft.Test.Xaml.Serialization
{
    /// <summary>
    /// Factory class for IXamlTestSerializer classes
    /// </summary>
    public static class XamlTestSerializerFactory
    {
        /// <summary>
        /// Returns an IXamlTestSerializer based on the Mode driver parameter for the test
        /// If Mode is null, it defaults to the WPFXamlSerializer
        /// </summary>
        /// <returns>IXamlTestSerializer based on the Mode driver parameter for the test</returns>
        public static IXamlTestSerializer Create()
        {
            if (!String.IsNullOrEmpty(DriverState.DriverParameters["Mode"]))
            {
                switch (DriverState.DriverParameters["Mode"].ToLowerInvariant())
                {
                    case "wpf":
                        return new WPFXamlSerializer();
                    default:
                        throw new InvalidOperationException(DriverState.DriverParameters["Mode"] + "is invalid");
                }
            }
            else
            {
                GlobalLog.LogStatus("Mode parameter not found defaulting to WPFXamlSerilizer");
                return new WPFXamlSerializer();
            }
        }
    }
}
