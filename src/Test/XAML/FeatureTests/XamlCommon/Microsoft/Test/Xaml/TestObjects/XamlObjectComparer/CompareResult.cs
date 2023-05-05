// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer
{
    /// <summary>
    /// Result of the compare operation
    /// </summary>
    public enum CompareResult
    {
        /// <summary>
        /// Two xmls are equivalent
        /// </summary>
        Equivalent,

        /// <summary>
        /// Two xmls are not equivalent
        /// </summary>
        Different
    }
}
