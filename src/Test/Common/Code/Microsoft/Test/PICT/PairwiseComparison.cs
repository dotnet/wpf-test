// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;

    /// <summary>
    /// The type of comparison
    /// </summary>
    /// <remarks>
    /// subset of System.CodeDom.CodeBinaryOperatorType
    /// </remarks>
    enum PairwiseComparison
    {
        /// <summary>
        /// Equal
        /// </summary>
        Equal = 0,
        /// <summary>
        /// NotEqual
        /// </summary>
        NotEqual = 1,
        /// <summary>
        /// Greater than (&gt;) using string or numerical comparison as appropriate
        /// </summary>
        GreaterThan = 2,
        /// <summary>
        /// Greater than or equal (&gt;=) using string or numerical comparison as appropriate
        /// </summary>
        GreaterThanOrEqual = 3,
        /// <summary>
        /// Less than (&lt;) using string or numerical comparison as appropriate
        /// </summary>
        LessThan = 4,
        /// <summary>
        /// Less than or equal to (&lt;=) using string or numerical comparison as appropriate
        /// </summary>
        LessThanOrEqual = 5
    }
}
