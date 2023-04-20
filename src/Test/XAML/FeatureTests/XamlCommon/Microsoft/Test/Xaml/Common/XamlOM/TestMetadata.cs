// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    /// <summary>
    /// Holds test matadata properties for product readers/writers
    /// </summary>
    public static class TestMetadata
    {
        /// <summary>
        /// Dont expect this node when reading back Xaml written using XamlXmlWriter
        /// </summary>
        public const string XXWIgnoreNode = "XXWIgnoreNode";
    }
}
