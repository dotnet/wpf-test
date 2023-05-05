// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    /// Add coverage for System.Windows.Markup.ServiceProviders
    /// </summary>
    public class ServiceProvidersTest
    {
        /// <summary>
        /// Add coverage for a few methods in System.Windows.Markup.ServiceProviders 
        /// </summary>
        public static void ServiceProvidersCoverage()
        {
            // there's a namespace in the project called ServiceProvides, so fully qualify the name
            var providers = new System.Windows.Markup.ServiceProviders();
            string service = "service";
            providers.AddService(typeof(string), service);

            Assert.AreEqual((string)providers.GetService(typeof(string)), service);
        }
    }
}
