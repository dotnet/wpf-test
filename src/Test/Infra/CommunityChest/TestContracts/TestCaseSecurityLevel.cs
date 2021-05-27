// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test
{
    // 



    /// <summary>
    /// Set the Security Level for the Test Cases Run.
    /// </summary>    
    public enum TestCaseSecurityLevel
    {
        /// <summary>
        /// Fallback to default value set on TestInfo or assembly attribute.
        /// </summary>
        Unset,
        /// <summary>
        /// Run all test cases under partial trust.
        /// </summary>
        PartialTrust,
        /// <summary>
        /// Run test cases with full trust.
        /// </summary>
        FullTrust
    }
}
