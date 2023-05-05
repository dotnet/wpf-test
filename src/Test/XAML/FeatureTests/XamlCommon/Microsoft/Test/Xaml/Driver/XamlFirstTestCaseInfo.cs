// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Driver
{
    using System;

    /// <summary>
    /// Xaml first test case information
    /// </summary>
    public class XamlFirstTestCaseInfo : TestCaseInfo
    {
        /// <summary>
        /// Initializes a new instance of the XamlFirstTestCaseInfo class
        /// </summary>
        public XamlFirstTestCaseInfo()
        {
            this.TestDriver = TestDrivers.XamlDoubleRoundtripDriver;
        }

        /// <summary>
        /// Gets or sets the InspectMethod delegate
        /// </summary>
        public Action<object> InspectMethod { get; set; }
    }
}
