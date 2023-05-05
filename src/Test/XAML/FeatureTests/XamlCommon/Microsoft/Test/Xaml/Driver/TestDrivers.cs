// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Driver
{
    /// <summary>
    /// Test drivers available
    /// </summary>
    public enum TestDrivers
    {
        /// <summary>
        /// Default is Object double roundtrip driver
        /// </summary>
        Default = ObjectDoubleRoundtripDriver,

        /// <summary>
        /// Object double roundtrip
        /// </summary>
        ObjectDoubleRoundtripDriver = 0,

        /// <summary>
        /// Xaml double round trip
        /// </summary>
        XamlDoubleRoundtripDriver,

        /// <summary>
        /// XamlXmlWriter, XamlXmlReader driver
        /// </summary>
        XamlXmlWriterXamlXmlReaderDriver,

        /// <summary>
        /// NodeWriter XamlXmlReader driver
        /// </summary>
        NodeWriterXamlXmlReaderDriver,
    }
}
